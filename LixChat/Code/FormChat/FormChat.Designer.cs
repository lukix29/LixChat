namespace LX29_ChatClient.Forms
{
    partial class FormChat
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormChat));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.cMS_TextOptions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsMi_CopyText = new System.Windows.Forms.ToolStripMenuItem();
            this.tSMi_PasteText = new System.Windows.Forms.ToolStripMenuItem();
            this.chatPanel1 = new LX29_ChatClient.Forms.ChatPanel();
            this.cMS_TextOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 2000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // cMS_TextOptions
            // 
            this.cMS_TextOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMi_CopyText,
            this.tSMi_PasteText});
            this.cMS_TextOptions.Name = "cMS_TextOptions";
            this.cMS_TextOptions.Size = new System.Drawing.Size(103, 48);
            // 
            // tsMi_CopyText
            // 
            this.tsMi_CopyText.Name = "tsMi_CopyText";
            this.tsMi_CopyText.Size = new System.Drawing.Size(102, 22);
            this.tsMi_CopyText.Text = "Copy";
            // 
            // tSMi_PasteText
            // 
            this.tSMi_PasteText.Name = "tSMi_PasteText";
            this.tSMi_PasteText.Size = new System.Drawing.Size(102, 22);
            this.tSMi_PasteText.Text = "Paste";
            // 
            // chatPanel1
            // 
            this.chatPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.chatPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.chatPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chatPanel1.Location = new System.Drawing.Point(0, 0);
            this.chatPanel1.Name = "chatPanel1";
            this.chatPanel1.Size = new System.Drawing.Size(669, 662);
            this.chatPanel1.TabIndex = 22;
            this.chatPanel1.Load += new System.EventHandler(this.chatPanel1_Load);
            // 
            // FormChat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(669, 662);
            this.Controls.Add(this.chatPanel1);
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormChat";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Chat Manager";
            this.TransparencyKey = System.Drawing.Color.Fuchsia;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormChat_FormClosing);
            this.Load += new System.EventHandler(this.FormChat_Load);
            this.cMS_TextOptions.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        //private System.Windows.Forms.WebBrowser htmlPanel;
        private System.Windows.Forms.ContextMenuStrip cMS_TextOptions;
        private System.Windows.Forms.ToolStripMenuItem tsMi_CopyText;
        private System.Windows.Forms.ToolStripMenuItem tSMi_PasteText;
        private ChatPanel chatPanel1;

    }
}