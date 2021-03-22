using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace unajma2
{
    public partial class Form1 : Form
    {
        private bool ejecutarControlador = true;
        public Form1()
        {
            InitializeComponent();
            MessageBox.Show("Bienvenido al Examen Virtual de la \n" +
                            "                  UNAJMA - 2021 \n" +
                            "MANTÉN ABIERTO EL PROGRAMA \n " +
                            "          DURANTE EL EXAMEN!");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label4.ForeColor = Color.DarkBlue;
            label4.Text = "CARGANDO...";
            txtInfoProgram.ForeColor = Color.OrangeRed;
            txtInfoProgram.Text = "VERIFICANDO CREDENCIALES...";

            //Instancia de conexion Servidor remoto
            Conexion ObjConex = new Conexion();

            //Usuario y password capturados de la interfaz de usuario
            String txtUser = txtUsuario.Text.Trim();
            String txtPass = txtPassword.Text.Trim();


            if (txtUser != "" && txtPass != "")
            {

                //Consulta Sql para identificar los credenciales del postulante
                String consulta = $"SELECT * FROM adm_proceso_postulante p WHERE p.numerodocumento LIKE '%{txtUser}%' " +
                        $" AND p.clavel LIKE '%{txtPass}%'";

                //Usamos el metodo para ejecutar consultas del objeto conexion que se instancio mas arriba. 
                //Esto devuelve un objeto que mas adelante se puede recorrer con un bucle
                MySqlDataReader reader = ObjConex.ejecutar_consulta(consulta);

                //verificamos si existen registro en la base de datos
                if (reader.HasRows)
                {
                    //recorremos los registros para capturar los datos y poder manipularlos
                    while (reader.Read())
                    {
                        txtDni_r.Text = reader.GetString("numerodocumento");
                        txtNombre_r.Text = reader.GetString("nombrecompleto");
                        break;
                    }
                    //ejecutamos la funcion para cerrar la conexion a la base de datos y liberar la memoria
                    ObjConex.cerrar_conexion();

                    //Obteniendo todos los procesos en ejecucion
                    Process[] processlist = Process.GetProcesses();

                    //Controla la existencia de programas invalidos en el computador del postulante
                    bool ExisteProgramas = false;
                    String ListProg_inv = "";   //Procesos invalidos 
                    String ListProg_inv_en_ejec = "";   //Procesos invalidos en ejecucion
                    String ListProg_t = "";     // todos los procesos
                    String ListProg_t_en_ejec = "";     // todos los programas en ejecucion
                    //Numero de procesos en ejecucion, que se imprime en un cuadro de texto
                    //txtBd.Text = ""+processlist.Length;
                    label4.Text = "Proceso activos en total " + processlist.Length;

                    //Limpiamos el listbox
                    listBox1.Items.Clear();

                    //recorremos todos los porcesos en ejecucion
                    foreach (Process theprocess in processlist)
                    {
                        ListProg_t += "* " + theprocess.ProcessName + "<br>";

                        if (theprocess.MainWindowTitle != "")
                            ListProg_t_en_ejec += "* " + theprocess.MainWindowTitle + " - " + theprocess.ProcessName + "<br>";

                        //Capturamos los programas invalidos
                        //theprocess.ProcessName == "notepad" || theprocess.ProcessName == "ok"                        
                        if (EsProgramaInvalido(theprocess.ProcessName))
                        {
                            ExisteProgramas = true;
                            ListProg_inv += "* " + theprocess.ProcessName + "<br>";

                            if (theprocess.MainWindowTitle != "")
                                ListProg_inv_en_ejec += "* " + theprocess.MainWindowTitle + " - " + theprocess.ProcessName + "<br>";
                        }

                        listBox1.Items.Add(theprocess.ProcessName + " - " + theprocess.MainWindowTitle
                            + " - " + theprocess.BasePriority + " - ");
                    }

                    //Recortando el tamanio de la cadena en el caso de que sea muy largo
                    ListProg_inv_en_ejec = cortarCadena(ListProg_inv_en_ejec, 500);
                    ListProg_inv = cortarCadena(ListProg_inv, 500);
                    ListProg_t_en_ejec = cortarCadena(ListProg_t_en_ejec, 500);
                    ListProg_t = "cargando . . .";

                    //ENVIAR AL SERVIDOR LA INFORMACION CAPTURADA.
                    //String consulta2 = $"INSERT INTO control_programas " +
                    //    $"(id, dni, v_abiertas, pin_enproceso, tp_enproceso, tp_instalados, fecha_registro) " +
                    //    $"VALUES (NULL, '{txtDni_r.Text.Trim()}', '{ListProg_inv_en_ejec}', '{ListProg_inv}', '{ListProg_t_en_ejec}', '{ListProg_t}', current_timestamp())";

                    //Ejecutando consulta de insercion
                    //ObjConex.ejecutar_insert(consulta2);

                    //DETECTAR SI ES DESKTOP O LAPTOP
                    String tipoPC;
                    if (SystemInformation.PowerStatus.BatteryChargeStatus == BatteryChargeStatus.NoSystemBattery)
                    {
                        //desktop 
                        tipoPC = "desktop";
                        Console.WriteLine("Desktop");
                    }
                    else
                    {
                        //laptop
                        tipoPC = "laptop";
                        Console.WriteLine("laptop");
                    }
                    //------------ OK tipo pc

                    //---- enviar primera vez
                    String dniEstudent = txtDni_r.Text.Trim();
                    int j = 1;

                    if (enviarProgramasInfo(j, dniEstudent, tipoPC))
                    {
                        Console.WriteLine("env existe programas ");
                    }
                    else
                    {
                        Console.WriteLine("env No existe progrmas");
                    }


                    //Codigo que   permite controlar periodicamente los programas abiertos en la PC del postulante
                    if (this.ejecutarControlador)
                    {
                        //--- MessageBox.Show("Iniciando setInterval");
                        var task = Task.Run(async () => {
                            int i = 1;
                            int time_interval = 20; // en este caso cada media hora
                            bool ctrl_interval = true; // si es false, controla cada media hora
                            for (; ; )
                            {
                                i++;
                                await Task.Delay(60000 * 10); // intervalo cada un minuto * n

                                Console.WriteLine("Control programas " + i);
                                if (ctrl_interval || (i == time_interval || i == (time_interval * 2)
                                        || i == (time_interval * 3) || i == (time_interval * 4)
                                        || i == (time_interval * 5) || i == (time_interval * 6)
                                        || i == (time_interval * 7) || i == (time_interval * 8)
                                        || i == (time_interval * 9) || i == (time_interval * 10)))
                                {
                                    if (enviarProgramasInfo(i, dniEstudent, tipoPC))
                                    {
                                        Console.WriteLine("env existe programas ");
                                    }
                                    else
                                    {
                                        Console.WriteLine("env No existe progrmas");
                                    }
                                }

                                if (CerrarProgramasInv())
                                {
                                    try
                                    {
                                        //Console.WriteLine("Programas cerrado!!");
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("Error: " + ex.Message);
                                    }
                                }

                            }
                        });

                        this.ejecutarControlador = !ejecutarControlador;
                    }

                    if (ExisteProgramas)
                    {
                        txtInfoProgram.Text = "SE ENCONTRARON PROGRAMAS INADECUADOS EN LA PC!";
                    }
                    else
                    {
                        txtInfoProgram.Text = "COMPUTADORA APTO!";
                        //MessageBox.Show("La computadora está apto para el examen vurtual de la UNAJMA ");
                    }
                }
                else
                {
                    txtInfoProgram.Text = "La cuenta no se encuentra Registrado!! o intentelo de nuevo.";
                    label4.Text = "...";
                    //MessageBox.Show("La cuenta no se encuentra Registrado!!");

                }

            }
            else
            {
                txtInfoProgram.Text = "Rellene los campos de USUARIO y CONTRASEÑA...";
                label4.Text = "...";
                //MessageBox.Show("Rellene los campos de USUARIO y CONTRASEÑA");
            }



        }


        private bool EsProgramaInvalido(String NomProg)
        {
            /*
            AnyDesk
            SupremoHelper, Supremo
            TeamViewer, TeamViewer_Desktop, TeamViewer_Service
            AA_v3
            IperiusRemote
            */
            bool result = false;
            String nombreProg = NomProg.ToLower();
            String[] listaProgramas = { "AnyDesk", "SupremoHelper", "Supremo", "TeamViewer", "TeamViewer_Desktop", "TeamViewer_Service", "AA_v3", "IperiusRemote" };

            foreach (String np in listaProgramas)
            {
                if (nombreProg.Contains(np.ToLower()))
                    return true;
            }

            return result;
        }


        protected bool enviarProgramasInfo(int i, String dniEstudent, String tipoPC)
        {

            Console.WriteLine("Control programas function" + dniEstudent + " - " + i);
            //Instancia de conexion Servidor remoto
            Conexion ObjConex = new Conexion();
            //------------------------------------------------------------------------------
            ////Obteniendo todos los procesos en ejecucion
            Process[] processlist = Process.GetProcesses();

            ////Controla la existencia de programas invalidos en el computador del postulante
            bool ExisteProgramas = false;
            String ListProg_inv = "" + i + ": ";   //Procesos invalidos 
            String ListProg_inv_en_ejec = "" + i + ": ";   //Procesos invalidos en ejecucion
            String ListProg_t = "" + i + ": ";     // todos los procesos
            String ListProg_t_en_ejec = "" + i + ": ";     // todos los programas en ejecucion

            //recorremos todos los porcesos en ejecucion
            foreach (Process theprocess in processlist)
            {
                ListProg_t += "* " + theprocess.ProcessName + "<br>";

                if (theprocess.MainWindowTitle != "")
                    ListProg_t_en_ejec += "* " + theprocess.MainWindowTitle + " - " + theprocess.ProcessName + "<br>";

                //Capturamos los programas invalidos              
                if (EsProgramaInvalido(theprocess.ProcessName))
                {
                    ExisteProgramas = true;
                    ListProg_inv += "* " + theprocess.ProcessName + "<br>";

                    if (theprocess.MainWindowTitle != "")
                        ListProg_inv_en_ejec += "* " + theprocess.MainWindowTitle + " - " + theprocess.ProcessName + "<br>";
                }

            }

            //Recortando el tamanio de la cadena en el caso de que sea muy largo
            ListProg_inv_en_ejec = cortarCadena(ListProg_inv_en_ejec, 500);
            ListProg_inv = cortarCadena(ListProg_inv, 500);
            ListProg_t_en_ejec = cortarCadena(ListProg_t_en_ejec, 500);
            ListProg_t = "* Es un ORDENADOR DE ESCRITORIO </br>";
            if (tipoPC == "laptop")
            {
                ListProg_t = "* Es una LAPTOP</br>";
            }


            //Detectando puertos, usbs conectados y HDMI
            //DETECTA LOS MONITORES HDMI CONECTADOS!!!
            String numHDMI;
            numHDMI = "";
            int cantHDMI = 0;
            ManagementClass mClass = new ManagementClass(@"\\localhost\ROOT\WMI:WmiMonitorConnectionParams");
            foreach (ManagementObject mObject in mClass.GetInstances())
            {
                numHDMI = "" + mObject["VideoOutputTechnology"]; // convirtiendo a string para la comparacion

                if (numHDMI == "5") //Because D3DKMDT_VOT_HDMI = 5
                {
                    cantHDMI++;
                    ListProg_t += "* HDMI Conectado. </br></br>";
                }
                else
                {
                    ListProg_t += "* No Hay HDMI. </br></br>";
                }
            }

            // ORIENTACION DE VULNERAVILIDADES!!
            if (tipoPC == "desktop")
            {
                //desktop
                if (cantHDMI >= 2)
                {
                    ListProg_t += "* (VERIFICAR) HDMI adicional conectados " + cantHDMI + ". </br>";
                }
            }
            else
            {
                //laptop
                if (cantHDMI >= 1)
                {
                    ListProg_t += "* (VERIFICAR) HDMI conectados " + cantHDMI + ".</br>";
                }
            }
            //---------------------- KO 1


            //DETECCIÓN DE PUERTOS DE USB CONECTADOS
            int cantUSB = 0;
            Usb oUsb = new Usb();
            List<USBInfo> lstUSBD = oUsb.GetUSBDevices();

            foreach (USBInfo elem in lstUSBD)
            {
                //ListProg_t += "* " + elem.Description + "</br>";
                Console.WriteLine("" + elem.Description);
                if (elem.Description == "Dispositivo de almacenamiento USB")
                {
                    ListProg_t += "* (verificar) " + elem.Description + "</br>";
                    Console.WriteLine("" + elem.Description);
                }
                if (elem.Description == "Dispositivo compuesto USB")
                {
                    cantUSB++;
                    ListProg_t += "* " + elem.Description + "</br>";
                    Console.WriteLine("" + elem.Description);
                }
            }
            //----------------------- KO 2

            // ORIENTACION DE VULNERAVILIDADES PUERTOS USBs!!
            if (tipoPC == "desktop")
            {
                //desktop
                if (cantUSB >= 3)
                {
                    ListProg_t += "* (VERIFICAR) Puertos USB. </br>";
                }
            }
            else
            {
                //laptop
                if (cantUSB >= 2)
                {
                    ListProg_t += "* (VERIFICAR) Puertos USB. </br>";
                }
            }
            //---------------------- KO 1



            //ENVIAR AL SERVIDOR LA INFORMACION CAPTURADA.
            String consulta3 = $"INSERT INTO control_programas " +
                $"(id, dni, v_abiertas, pin_enproceso, tp_enproceso, tp_instalados, fecha_registro) " +
                $"VALUES (NULL, '{dniEstudent}', '{ListProg_inv_en_ejec}', '{ListProg_inv}', '{ListProg_t_en_ejec}', '{ListProg_t}', current_timestamp())";

            //Ejecutando consulta de insercion
            ObjConex.ejecutar_insert(consulta3);

            return ExisteProgramas;
        }

        private bool CerrarProgramasInv()
        {
            bool result = false;
            bool ver_prog = false;
            string msj1 = "", msj2 = "";
            Process[] processlist = Process.GetProcesses();
            foreach (Process TheProcess in processlist)
            {
                if (EsProgramaInvalido(TheProcess.ProcessName))
                {
                    msj1 = "USTED ESTUBO UTILIZANDO PROGRAMAS INDEVIDOS";
                    if (TheProcess.MainWindowTitle != "")
                    {
                        msj2 = "DEBE CERRAR EL/LOS SIGUIENTES PROGRAMAS :";
                        msj2 += TheProcess.MainWindowTitle + ", ";
                        ver_prog = true;
                    }

                    result = true;
                }
            }
            if (result)
            {
                if (ver_prog)
                    setLabel1TextSafe(msj2, txtInfoProgram);
                else
                    setLabel1TextSafe(msj1, txtInfoProgram);

                Console.WriteLine(msj2);

            }
            else
            {
                setLabel1TextSafe("PC LIMPIO", txtInfoProgram);
            }

            return result;
        }


        //funcion para lista de programas instaldos en la pc del postulante
        private void ListaProgramasInstaladosDePC()
        {
            ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_Product");
            String LPInsta = "";
            foreach (ManagementObject mo in mos.Get())
            {
                LPInsta += mo["Name"] + "\n";
                Console.WriteLine(mo["Description"]);
            }
            MessageBox.Show("Programas instalados: \n" + LPInsta);
        }

        //Funcion que permite cambiar el valor del texto cuando el programa esta en ejecucion
        private void setLabel1TextSafe(string txt, Label label_num)
        {
            if (label_num.InvokeRequired)
                label_num.Invoke(new Action(() => label_num.Text = txt));
            else
                label_num.Text = txt;
        }

        //funcion que retorna una cadena string segun sugun el tamanio de la cadena ingresada por parametro
        private string cortarCadena(String texto, int cant)
        {
            String resText = texto;
            if (texto.Length >= cant)
            {
                resText = texto.Substring(0, 500);
            }

            return resText;
        }


    }



    // CLASE CONEXIÓN ------------------------------------
    class Conexion
    {

        private MySqlConnection conex;

        public Conexion()
        {
            //String servidor = "localhost";
            //String puerto = "3306";
            //String usuario = "root";
            //String password = "";
            //String bd = "admision_zet";

            String servidor = "51.79.1.235";
            String puerto = "3306";
            String usuario = "admision_zet";
            String password = "carlsen2020";
            String bd = "admision_zet";

            String cadenaConexion = "Database=" + bd +
                "; Data Source=" + servidor +
                "; Port=" + puerto +
                "; User Id=" + usuario +
                "; Password=" + password;


            MySqlConnection conexionBD = new MySqlConnection(cadenaConexion);

            this.conex = conexionBD;

        }

        public void ejecutar_insert(String cons_sql)
        {
            String consulta = cons_sql;

            MySqlCommand comando = new MySqlCommand(consulta);

            comando.Connection = this.conex;

            this.conex.Open();

            comando.ExecuteNonQuery();

            this.conex.Close();
        }

        public MySqlDataReader ejecutar_consulta(String cons_sql)
        {
            String consulta = cons_sql;

            MySqlCommand comando = new MySqlCommand(consulta);

            comando.Connection = this.conex;

            this.conex.Open();

            MySqlDataReader reader = comando.ExecuteReader();

            return reader;
        }

        public void cerrar_conexion()
        {
            this.conex.Close();
            //MessageBox.Show("Listo! \n Mantenga abierto el programa durante la evaluacion.");
        }

    }







    public class Usb
    {

        /// <summary>
        /// obtiene las usb de la computadora
        /// </summary>
        /// <returns></returns>
        public List<USBInfo> GetUSBDevices()
        {

            //creamos una lista de USBInfo
            List<USBInfo> lstDispositivos = new List<USBInfo>();

            //creamos un ManagementObjectCollection para obtener nuestros dispositivos
            ManagementObjectCollection collection;

            //utilizando la WMI clase Win32_USBHub obtenemos todos los dispositivos USB
            using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_USBHub"))

                //asignamos los dispositivos a nuestra coleccion
                collection = searcher.Get();

            //recorremos la colección
            foreach (var device in collection)
            {
                //asignamos el dispositivo a nuestra lista
                lstDispositivos.Add(new USBInfo(
                (string)device.GetPropertyValue("DeviceID"),
                (string)device.GetPropertyValue("PNPDeviceID"),
                (string)device.GetPropertyValue("Description")
                ));
            }

            //liberamos el objeto collection
            collection.Dispose();
            //regresamos la lista
            return lstDispositivos;
        }
    }




    /// <summary>
    /// clase para guardar las especificaciones de los dispositivos
    /// </summary>
    public class USBInfo
    {

        //constructor
        public USBInfo(string deviceID, string pnpDeviceID, string description)
        {
            this.DeviceID = deviceID;
            this.PnpDeviceID = pnpDeviceID;
            this.Description = description;
        }

        /// <summary>
        /// Device ID
        /// </summary>
        public string DeviceID { get; private set; }

        /// <summary>
        /// Pnp Device Id
        /// </summary>
        public string PnpDeviceID { get; private set; }

        /// <summary>
        /// Descripción del dispositivo o nombre
        /// </summary>
        public string Description { get; private set; }
    }




}
