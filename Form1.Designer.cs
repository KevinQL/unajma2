
namespace unajma2
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.button2 = new System.Windows.Forms.Button();
            this.txtUsuario = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.txtDni_r = new System.Windows.Forms.TextBox();
            this.txtNombre_r = new System.Windows.Forms.TextBox();
            this.txtInfoProgram = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(258, 150);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(211, 62);
            this.button2.TabIndex = 0;
            this.button2.Text = "INICIAR CONTROL";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // txtUsuario
            // 
            this.txtUsuario.Location = new System.Drawing.Point(296, 57);
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.Size = new System.Drawing.Size(125, 20);
            this.txtUsuario.TabIndex = 1;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(296, 98);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(125, 20);
            this.txtPassword.TabIndex = 2;
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(177, 292);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(382, 134);
            this.listBox1.TabIndex = 3;
            // 
            // txtDni_r
            // 
            this.txtDni_r.Location = new System.Drawing.Point(172, 262);
            this.txtDni_r.Name = "txtDni_r";
            this.txtDni_r.Size = new System.Drawing.Size(110, 20);
            this.txtDni_r.TabIndex = 4;
            // 
            // txtNombre_r
            // 
            this.txtNombre_r.Location = new System.Drawing.Point(301, 263);
            this.txtNombre_r.Name = "txtNombre_r";
            this.txtNombre_r.Size = new System.Drawing.Size(257, 20);
            this.txtNombre_r.TabIndex = 5;
            // 
            // txtInfoProgram
            // 
            this.txtInfoProgram.AutoSize = true;
            this.txtInfoProgram.Location = new System.Drawing.Point(174, 236);
            this.txtInfoProgram.Name = "txtInfoProgram";
            this.txtInfoProgram.Size = new System.Drawing.Size(35, 13);
            this.txtInfoProgram.TabIndex = 6;
            this.txtInfoProgram.Text = "label1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 452);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "label2";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(756, 474);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtInfoProgram);
            this.Controls.Add(this.txtNombre_r);
            this.Controls.Add(this.txtDni_r);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtUsuario);
            this.Controls.Add(this.button2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox txtUsuario;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TextBox txtDni_r;
        private System.Windows.Forms.TextBox txtNombre_r;
        private System.Windows.Forms.Label txtInfoProgram;
        private System.Windows.Forms.Label label4;
    }
}

