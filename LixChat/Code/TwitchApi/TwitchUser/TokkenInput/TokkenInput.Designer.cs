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
            this.btn_Abort = new System.Windows.Forms.Button();
            this.btn_Viewer = new System.Windows.Forms.Button();
            this.btn_Streamer = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btn_Login
            // 
            this.btn_Login.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btn_Login.BackColor = System.Drawing.Color.DimGray;
            this.btn_Login.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_Login.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Login.Location = new System.Drawing.Point(7, 198);
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
            this.label1.Size = new System.Drawing.Size(694, 372);
            this.label1.TabIndex = 2;
            this.label1.Text = "How you wanna use Lixchat";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // btn_Abort
            // 
            this.btn_Abort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Abort.BackColor = System.Drawing.Color.DimGray;
            this.btn_Abort.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_Abort.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Abort.Location = new System.Drawing.Point(580, 328);
            this.btn_Abort.Name = "btn_Abort";
            this.btn_Abort.Size = new System.Drawing.Size(107, 41);
            this.btn_Abort.TabIndex = 4;
            this.btn_Abort.Text = "Abort";
            this.btn_Abort.UseVisualStyleBackColor = false;
            this.btn_Abort.Click += new System.EventHandler(this.btn_Abort_Click);
            // 
            // btn_Viewer
            // 
            this.btn_Viewer.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btn_Viewer.BackColor = System.Drawing.Color.DimGray;
            this.btn_Viewer.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_Viewer.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Viewer.Location = new System.Drawing.Point(7, 114);
            this.btn_Viewer.Name = "btn_Viewer";
            this.btn_Viewer.Size = new System.Drawing.Size(168, 52);
            this.btn_Viewer.TabIndex = 5;
            this.btn_Viewer.Text = "as Viewer";
            this.btn_Viewer.UseVisualStyleBackColor = false;
            this.btn_Viewer.Click += new System.EventHandler(this.btn_Viewer_Click);
            // 
            // btn_Streamer
            // 
            this.btn_Streamer.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btn_Streamer.BackColor = System.Drawing.Color.DimGray;
            this.btn_Streamer.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_Streamer.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Streamer.Location = new System.Drawing.Point(527, 114);
            this.btn_Streamer.Name = "btn_Streamer";
            this.btn_Streamer.Size = new System.Drawing.Size(160, 52);
            this.btn_Streamer.TabIndex = 6;
            this.btn_Streamer.Text = "as Streamer";
            this.btn_Streamer.UseVisualStyleBackColor = false;
            this.btn_Streamer.Click += new System.EventHandler(this.btn_Streamer_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(188, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(328, 20);
            this.label2.TabIndex = 7;
            this.label2.Text = "(This can be changed in Settings afterwards.)";
            // 
            // TokkenInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btn_Streamer);
            this.Controls.Add(this.btn_Viewer);
            this.Controls.Add(this.btn_Abort);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_Login);
            this.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.Name = "TokkenInput";
            this.Size = new System.Drawing.Size(694, 372);
            this.Load += new System.EventHandler(this.TokkenInput_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_Login;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btn_Abort;
        private System.Windows.Forms.Button btn_Viewer;
        private System.Windows.Forms.Button btn_Streamer;
        private System.Windows.Forms.Label label2;
    }
}
