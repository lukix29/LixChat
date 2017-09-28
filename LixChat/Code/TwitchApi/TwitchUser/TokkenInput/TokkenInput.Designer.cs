namespace LX29_Twitch.Api.Controls
{
    partial class TokkenInput
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btn_Login = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btn_retry = new System.Windows.Forms.Button();
            this.btn_Abort = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_Login
            // 
            this.btn_Login.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btn_Login.BackColor = System.Drawing.Color.DimGray;
            this.btn_Login.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_Login.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Login.Location = new System.Drawing.Point(7, 79);
            this.btn_Login.Name = "btn_Login";
            this.btn_Login.Size = new System.Drawing.Size(107, 41);
            this.btn_Login.TabIndex = 1;
            this.btn_Login.Text = "Log In";
            this.btn_Login.UseVisualStyleBackColor = false;
            this.btn_Login.Visible = false;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(484, 135);
            this.label1.TabIndex = 2;
            this.label1.Text = "Waiting for Login...";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btn_retry
            // 
            this.btn_retry.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btn_retry.BackColor = System.Drawing.Color.DimGray;
            this.btn_retry.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_retry.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_retry.Location = new System.Drawing.Point(7, 79);
            this.btn_retry.Name = "btn_retry";
            this.btn_retry.Size = new System.Drawing.Size(107, 41);
            this.btn_retry.TabIndex = 3;
            this.btn_retry.Text = "Retry";
            this.btn_retry.UseVisualStyleBackColor = false;
            this.btn_retry.Click += new System.EventHandler(this.btn_retry_Click);
            // 
            // btn_Abort
            // 
            this.btn_Abort.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btn_Abort.BackColor = System.Drawing.Color.DimGray;
            this.btn_Abort.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_Abort.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Abort.Location = new System.Drawing.Point(370, 79);
            this.btn_Abort.Name = "btn_Abort";
            this.btn_Abort.Size = new System.Drawing.Size(107, 41);
            this.btn_Abort.TabIndex = 4;
            this.btn_Abort.Text = "Abort";
            this.btn_Abort.UseVisualStyleBackColor = false;
            this.btn_Abort.Click += new System.EventHandler(this.btn_Abort_Click);
            // 
            // TokkenInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.Controls.Add(this.btn_Abort);
            this.Controls.Add(this.btn_retry);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_Login);
            this.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.Name = "TokkenInput";
            this.Size = new System.Drawing.Size(484, 135);
            this.Load += new System.EventHandler(this.TokkenInput_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_Login;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btn_retry;
        private System.Windows.Forms.Button btn_Abort;
    }
}
