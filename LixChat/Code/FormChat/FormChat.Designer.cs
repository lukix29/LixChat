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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tS_Btn_Emotes = new System.Windows.Forms.ToolStripButton();
            this.tS_Btn_ChatSettings = new System.Windows.Forms.ToolStripButton();
            this.cMS_TextOptions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsMi_CopyText = new System.Windows.Forms.ToolStripMenuItem();
            this.tSMi_PasteText = new System.Windows.Forms.ToolStripMenuItem();
            this.controlSettings1 = new LX29_ChatClient.Forms.ControlSettings();
            this.chatPanel1 = new LX29_ChatClient.Forms.ChatPanel();
            this.toolStrip1.SuspendLayout();
            this.cMS_TextOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tS_Btn_Emotes,
            this.tS_Btn_ChatSettings});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(669, 25);
            this.toolStrip1.TabIndex = 15;
            this.toolStrip1.Text = "toolStrip1";
            this.toolStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.toolStrip1_ItemClicked);
            // 
            // tS_Btn_Emotes
            // 
            this.tS_Btn_Emotes.BackColor = System.Drawing.Color.Black;
            this.tS_Btn_Emotes.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tS_Btn_Emotes.ForeColor = System.Drawing.Color.White;
            this.tS_Btn_Emotes.Image = ((System.Drawing.Image)(resources.GetObject("tS_Btn_Emotes.Image")));
            this.tS_Btn_Emotes.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tS_Btn_Emotes.Name = "tS_Btn_Emotes";
            this.tS_Btn_Emotes.Size = new System.Drawing.Size(50, 22);
            this.tS_Btn_Emotes.Text = "Emotes";
            this.tS_Btn_Emotes.Click += new System.EventHandler(this.tS_Btn_Emotes_Click);
            // 
            // tS_Btn_ChatSettings
            // 
            this.tS_Btn_ChatSettings.BackColor = System.Drawing.Color.Black;
            this.tS_Btn_ChatSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tS_Btn_ChatSettings.ForeColor = System.Drawing.Color.White;
            this.tS_Btn_ChatSettings.Image = ((System.Drawing.Image)(resources.GetObject("tS_Btn_ChatSettings.Image")));
            this.tS_Btn_ChatSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tS_Btn_ChatSettings.Name = "tS_Btn_ChatSettings";
            this.tS_Btn_ChatSettings.Size = new System.Drawing.Size(53, 22);
            this.tS_Btn_ChatSettings.Text = "Settings";
            this.tS_Btn_ChatSettings.Click += new System.EventHandler(this.tS_Btn_ChatSettings_Click);
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
            // controlSettings1
            // 
            this.controlSettings1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.controlSettings1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))));
            this.controlSettings1.ForeColor = System.Drawing.Color.LightGray;
            this.controlSettings1.Location = new System.Drawing.Point(0, 28);
            this.controlSettings1.MinimumSize = new System.Drawing.Size(640, 270);
            this.controlSettings1.Name = "controlSettings1";
            this.controlSettings1.Size = new System.Drawing.Size(669, 460);
            this.controlSettings1.TabIndex = 23;
            this.controlSettings1.Visible = false;
            // 
            // chatPanel1
            // 
            this.chatPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chatPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.chatPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.chatPanel1.Location = new System.Drawing.Point(0, 28);
            this.chatPanel1.Name = "chatPanel1";
            this.chatPanel1.Size = new System.Drawing.Size(669, 634);
            this.chatPanel1.TabIndex = 22;
            this.chatPanel1.Load += new System.EventHandler(this.chatPanel1_Load);
            // 
            // FormChat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(669, 662);
            this.Controls.Add(this.controlSettings1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.chatPanel1);
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormChat";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Chat Manager";
            this.TransparencyKey = System.Drawing.Color.Fuchsia;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormChat_FormClosing);
            this.Load += new System.EventHandler(this.FormChat_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.cMS_TextOptions.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        //private System.Windows.Forms.WebBrowser htmlPanel;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ContextMenuStrip cMS_TextOptions;
        private System.Windows.Forms.ToolStripMenuItem tsMi_CopyText;
        private System.Windows.Forms.ToolStripMenuItem tSMi_PasteText;
        private System.Windows.Forms.ToolStripButton tS_Btn_ChatSettings;
        private ChatPanel chatPanel1;
        private ControlSettings controlSettings1;
        private System.Windows.Forms.ToolStripButton tS_Btn_Emotes;

    }
}