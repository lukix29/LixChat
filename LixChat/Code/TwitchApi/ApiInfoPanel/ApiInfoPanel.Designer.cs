namespace LX29_Twitch.Forms
{
    partial class ApiInfoPanel
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
            this.rTB_Infos = new System.Windows.Forms.RichTextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lbl_OnlineTime = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rTB_Infos
            // 
            this.rTB_Infos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.rTB_Infos.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rTB_Infos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rTB_Infos.ForeColor = System.Drawing.Color.Gainsboro;
            this.rTB_Infos.Location = new System.Drawing.Point(0, 0);
            this.rTB_Infos.Name = "rTB_Infos";
            this.rTB_Infos.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.rTB_Infos.Size = new System.Drawing.Size(331, 427);
            this.rTB_Infos.TabIndex = 0;
            this.rTB_Infos.Text = "";
            this.rTB_Infos.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rTB_Infos_LinkClicked);
            this.rTB_Infos.TextChanged += new System.EventHandler(this.rTB_Infos_TextChanged);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lbl_OnlineTime
            // 
            this.lbl_OnlineTime.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.lbl_OnlineTime.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lbl_OnlineTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbl_OnlineTime.ForeColor = System.Drawing.Color.Gainsboro;
            this.lbl_OnlineTime.Location = new System.Drawing.Point(0, 0);
            this.lbl_OnlineTime.Name = "lbl_OnlineTime";
            this.lbl_OnlineTime.ReadOnly = true;
            this.lbl_OnlineTime.Size = new System.Drawing.Size(331, 15);
            this.lbl_OnlineTime.TabIndex = 2;
            this.lbl_OnlineTime.WordWrap = false;
            this.lbl_OnlineTime.SizeChanged += new System.EventHandler(this.lbl_OnlineTime_SizeChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lbl_OnlineTime);
            this.splitContainer1.Panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer1_Panel1_Paint);
            this.splitContainer1.Panel1MinSize = 15;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.rTB_Infos);
            this.splitContainer1.Size = new System.Drawing.Size(331, 455);
            this.splitContainer1.SplitterDistance = 25;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 3;
            this.splitContainer1.FontChanged += new System.EventHandler(this.splitContainer1_FontChanged);
            // 
            // ApiInfoPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.splitContainer1);
            this.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.Name = "ApiInfoPanel";
            this.Size = new System.Drawing.Size(331, 455);
            this.Load += new System.EventHandler(this.ApiInfoPanel_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rTB_Infos;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox lbl_OnlineTime;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}
