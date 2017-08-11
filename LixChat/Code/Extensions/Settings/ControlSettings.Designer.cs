namespace LX29_ChatClient.Forms
{
    partial class ControlSettings
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel_RenderOptions = new System.Windows.Forms.FlowLayoutPanel();
            this.cB_AnimatedEmotes = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel_ChatOptions = new System.Windows.Forms.FlowLayoutPanel();
            this.btn_SelectFont = new System.Windows.Forms.Button();
            this.btn_SelectChatBG = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.rTB_HighlightWords = new System.Windows.Forms.RichTextBox();
            this.btn_SaveHL = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel_TextOptions = new System.Windows.Forms.FlowLayoutPanel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.flowLayoutPanel5 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel6 = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel9 = new System.Windows.Forms.FlowLayoutPanel();
            this.btn_SelectBrowser = new System.Windows.Forms.Button();
            this.btn_OpenScriptFolder = new System.Windows.Forms.Button();
            this.cb_ShowErrors = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_Close = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel_UserOptions = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel_PlayerOptions = new System.Windows.Forms.FlowLayoutPanel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox3.SuspendLayout();
            this.flowLayoutPanel_RenderOptions.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.flowLayoutPanel_ChatOptions.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.flowLayoutPanel5.SuspendLayout();
            this.flowLayoutPanel6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.flowLayoutPanel9.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.flowLayoutPanel_RenderOptions);
            this.flowLayoutPanel5.SetFlowBreak(this.groupBox3, true);
            this.groupBox3.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.ForeColor = System.Drawing.Color.LightGray;
            this.groupBox3.Location = new System.Drawing.Point(173, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(160, 262);
            this.groupBox3.TabIndex = 37;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Render Options";
            // 
            // flowLayoutPanel_RenderOptions
            // 
            this.flowLayoutPanel_RenderOptions.Controls.Add(this.cB_AnimatedEmotes);
            this.flowLayoutPanel_RenderOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel_RenderOptions.Location = new System.Drawing.Point(3, 17);
            this.flowLayoutPanel_RenderOptions.Name = "flowLayoutPanel_RenderOptions";
            this.flowLayoutPanel_RenderOptions.Size = new System.Drawing.Size(154, 242);
            this.flowLayoutPanel_RenderOptions.TabIndex = 0;
            // 
            // cB_AnimatedEmotes
            // 
            this.cB_AnimatedEmotes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cB_AnimatedEmotes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.cB_AnimatedEmotes.Checked = true;
            this.cB_AnimatedEmotes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cB_AnimatedEmotes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cB_AnimatedEmotes.Location = new System.Drawing.Point(3, 3);
            this.cB_AnimatedEmotes.Name = "cB_AnimatedEmotes";
            this.cB_AnimatedEmotes.Size = new System.Drawing.Size(132, 20);
            this.cB_AnimatedEmotes.TabIndex = 34;
            this.cB_AnimatedEmotes.TabStop = false;
            this.cB_AnimatedEmotes.Text = "Animated GiF\'s";
            this.cB_AnimatedEmotes.UseVisualStyleBackColor = false;
            this.cB_AnimatedEmotes.CheckedChanged += new System.EventHandler(this.cB_AnimatedEmotes_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.flowLayoutPanel_ChatOptions);
            this.flowLayoutPanel5.SetFlowBreak(this.groupBox2, true);
            this.groupBox2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.Color.LightGray;
            this.groupBox2.Location = new System.Drawing.Point(339, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(127, 262);
            this.groupBox2.TabIndex = 36;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Chat Options";
            // 
            // flowLayoutPanel_ChatOptions
            // 
            this.flowLayoutPanel_ChatOptions.Controls.Add(this.btn_SelectFont);
            this.flowLayoutPanel_ChatOptions.Controls.Add(this.btn_SelectChatBG);
            this.flowLayoutPanel_ChatOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel_ChatOptions.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel_ChatOptions.Location = new System.Drawing.Point(3, 17);
            this.flowLayoutPanel_ChatOptions.Name = "flowLayoutPanel_ChatOptions";
            this.flowLayoutPanel_ChatOptions.Size = new System.Drawing.Size(121, 242);
            this.flowLayoutPanel_ChatOptions.TabIndex = 0;
            // 
            // btn_SelectFont
            // 
            this.btn_SelectFont.BackColor = System.Drawing.Color.Black;
            this.btn_SelectFont.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_SelectFont.Location = new System.Drawing.Point(3, 3);
            this.btn_SelectFont.Name = "btn_SelectFont";
            this.btn_SelectFont.Size = new System.Drawing.Size(115, 30);
            this.btn_SelectFont.TabIndex = 28;
            this.btn_SelectFont.Text = "Select Font";
            this.btn_SelectFont.UseVisualStyleBackColor = false;
            this.btn_SelectFont.Click += new System.EventHandler(this.btn_SelectFont_Click);
            // 
            // btn_SelectChatBG
            // 
            this.btn_SelectChatBG.BackColor = System.Drawing.Color.Black;
            this.btn_SelectChatBG.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_SelectChatBG.Location = new System.Drawing.Point(3, 39);
            this.btn_SelectChatBG.Name = "btn_SelectChatBG";
            this.btn_SelectChatBG.Size = new System.Drawing.Size(115, 30);
            this.btn_SelectChatBG.TabIndex = 29;
            this.btn_SelectChatBG.Text = "Chat Background";
            this.btn_SelectChatBG.UseVisualStyleBackColor = false;
            this.btn_SelectChatBG.Click += new System.EventHandler(this.btn_SelectChatBG_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.flowLayoutPanel1);
            this.flowLayoutPanel5.SetFlowBreak(this.groupBox1, true);
            this.groupBox1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.LightGray;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(164, 262);
            this.groupBox1.TabIndex = 35;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Highlight Words";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.rTB_HighlightWords);
            this.flowLayoutPanel1.Controls.Add(this.btn_SaveHL);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 17);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(158, 242);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // rTB_HighlightWords
            // 
            this.rTB_HighlightWords.BackColor = System.Drawing.Color.Black;
            this.rTB_HighlightWords.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rTB_HighlightWords.ForeColor = System.Drawing.Color.Gainsboro;
            this.rTB_HighlightWords.Location = new System.Drawing.Point(3, 3);
            this.rTB_HighlightWords.Name = "rTB_HighlightWords";
            this.rTB_HighlightWords.Size = new System.Drawing.Size(152, 207);
            this.rTB_HighlightWords.TabIndex = 31;
            this.rTB_HighlightWords.Text = "";
            // 
            // btn_SaveHL
            // 
            this.btn_SaveHL.BackColor = System.Drawing.Color.Black;
            this.btn_SaveHL.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_SaveHL.Location = new System.Drawing.Point(3, 216);
            this.btn_SaveHL.Name = "btn_SaveHL";
            this.btn_SaveHL.Size = new System.Drawing.Size(152, 26);
            this.btn_SaveHL.TabIndex = 32;
            this.btn_SaveHL.Text = "Save Higlight Words";
            this.btn_SaveHL.UseVisualStyleBackColor = false;
            this.btn_SaveHL.Click += new System.EventHandler(this.btn_SaveHL_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.flowLayoutPanel_TextOptions);
            this.flowLayoutPanel5.SetFlowBreak(this.groupBox4, true);
            this.groupBox4.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.ForeColor = System.Drawing.Color.LightGray;
            this.groupBox4.Location = new System.Drawing.Point(472, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(155, 262);
            this.groupBox4.TabIndex = 38;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Text Options";
            // 
            // flowLayoutPanel_TextOptions
            // 
            this.flowLayoutPanel_TextOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel_TextOptions.Location = new System.Drawing.Point(3, 17);
            this.flowLayoutPanel_TextOptions.Name = "flowLayoutPanel_TextOptions";
            this.flowLayoutPanel_TextOptions.Size = new System.Drawing.Size(149, 242);
            this.flowLayoutPanel_TextOptions.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.flowLayoutPanel5);
            this.splitContainer1.Panel1MinSize = 270;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.flowLayoutPanel6);
            this.splitContainer1.Panel2MinSize = 185;
            this.splitContainer1.Size = new System.Drawing.Size(640, 463);
            this.splitContainer1.SplitterDistance = 270;
            this.splitContainer1.TabIndex = 39;
            // 
            // flowLayoutPanel5
            // 
            this.flowLayoutPanel5.Controls.Add(this.groupBox1);
            this.flowLayoutPanel5.Controls.Add(this.groupBox3);
            this.flowLayoutPanel5.Controls.Add(this.groupBox2);
            this.flowLayoutPanel5.Controls.Add(this.groupBox4);
            this.flowLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel5.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel5.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel5.Name = "flowLayoutPanel5";
            this.flowLayoutPanel5.Size = new System.Drawing.Size(640, 270);
            this.flowLayoutPanel5.TabIndex = 0;
            // 
            // flowLayoutPanel6
            // 
            this.flowLayoutPanel6.AutoSize = true;
            this.flowLayoutPanel6.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel6.Controls.Add(this.groupBox7);
            this.flowLayoutPanel6.Controls.Add(this.groupBox5);
            this.flowLayoutPanel6.Controls.Add(this.groupBox6);
            this.flowLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel6.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel6.Name = "flowLayoutPanel6";
            this.flowLayoutPanel6.Size = new System.Drawing.Size(640, 189);
            this.flowLayoutPanel6.TabIndex = 37;
            this.flowLayoutPanel6.Paint += new System.Windows.Forms.PaintEventHandler(this.flowLayoutPanel6_Paint);
            // 
            // groupBox7
            // 
            this.groupBox7.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.groupBox7.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox7.Controls.Add(this.flowLayoutPanel9);
            this.groupBox7.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox7.ForeColor = System.Drawing.Color.LightGray;
            this.groupBox7.Location = new System.Drawing.Point(3, 3);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(161, 180);
            this.groupBox7.TabIndex = 36;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Options";
            // 
            // flowLayoutPanel9
            // 
            this.flowLayoutPanel9.AutoSize = true;
            this.flowLayoutPanel9.Controls.Add(this.btn_SelectBrowser);
            this.flowLayoutPanel9.Controls.Add(this.btn_OpenScriptFolder);
            this.flowLayoutPanel9.Controls.Add(this.cb_ShowErrors);
            this.flowLayoutPanel9.Controls.Add(this.label1);
            this.flowLayoutPanel9.Controls.Add(this.btn_Close);
            this.flowLayoutPanel9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel9.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel9.Location = new System.Drawing.Point(3, 17);
            this.flowLayoutPanel9.Name = "flowLayoutPanel9";
            this.flowLayoutPanel9.Size = new System.Drawing.Size(155, 160);
            this.flowLayoutPanel9.TabIndex = 0;
            // 
            // btn_SelectBrowser
            // 
            this.btn_SelectBrowser.BackColor = System.Drawing.Color.Black;
            this.btn_SelectBrowser.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_SelectBrowser.Location = new System.Drawing.Point(3, 3);
            this.btn_SelectBrowser.Name = "btn_SelectBrowser";
            this.btn_SelectBrowser.Size = new System.Drawing.Size(149, 30);
            this.btn_SelectBrowser.TabIndex = 28;
            this.btn_SelectBrowser.Text = "Select Browser";
            this.btn_SelectBrowser.UseVisualStyleBackColor = false;
            this.btn_SelectBrowser.Click += new System.EventHandler(this.btn_SelectBrowser_Click);
            // 
            // btn_OpenScriptFolder
            // 
            this.btn_OpenScriptFolder.BackColor = System.Drawing.Color.Black;
            this.btn_OpenScriptFolder.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_OpenScriptFolder.Location = new System.Drawing.Point(3, 39);
            this.btn_OpenScriptFolder.Name = "btn_OpenScriptFolder";
            this.btn_OpenScriptFolder.Size = new System.Drawing.Size(149, 30);
            this.btn_OpenScriptFolder.TabIndex = 29;
            this.btn_OpenScriptFolder.Text = "Open Config Folder";
            this.btn_OpenScriptFolder.UseVisualStyleBackColor = false;
            this.btn_OpenScriptFolder.Click += new System.EventHandler(this.btn_OpenScriptFolder_Click);
            // 
            // cb_ShowErrors
            // 
            this.cb_ShowErrors.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cb_ShowErrors.Checked = true;
            this.cb_ShowErrors.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_ShowErrors.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cb_ShowErrors.Location = new System.Drawing.Point(3, 75);
            this.cb_ShowErrors.Name = "cb_ShowErrors";
            this.cb_ShowErrors.Size = new System.Drawing.Size(149, 20);
            this.cb_ShowErrors.TabIndex = 35;
            this.cb_ShowErrors.TabStop = false;
            this.cb_ShowErrors.Text = "Show Errors";
            this.toolTip1.SetToolTip(this.cb_ShowErrors, "if unchecked , trys 5 times again before ignoring");
            this.cb_ShowErrors.UseVisualStyleBackColor = false;
            this.cb_ShowErrors.CheckedChanged += new System.EventHandler(this.cb_ShowErrors_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(3, 98);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 24);
            this.label1.TabIndex = 36;
            this.label1.Text = "                     ";
            // 
            // btn_Close
            // 
            this.btn_Close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Close.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.btn_Close.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_Close.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Close.Location = new System.Drawing.Point(3, 125);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(149, 30);
            this.btn_Close.TabIndex = 29;
            this.btn_Close.Text = "Close Settings";
            this.btn_Close.UseVisualStyleBackColor = false;
            this.btn_Close.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.groupBox5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox5.Controls.Add(this.flowLayoutPanel_UserOptions);
            this.groupBox5.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox5.ForeColor = System.Drawing.Color.LightGray;
            this.groupBox5.Location = new System.Drawing.Point(170, 3);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(187, 180);
            this.groupBox5.TabIndex = 37;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "User Options";
            // 
            // flowLayoutPanel_UserOptions
            // 
            this.flowLayoutPanel_UserOptions.AutoSize = true;
            this.flowLayoutPanel_UserOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel_UserOptions.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel_UserOptions.Location = new System.Drawing.Point(3, 17);
            this.flowLayoutPanel_UserOptions.Name = "flowLayoutPanel_UserOptions";
            this.flowLayoutPanel_UserOptions.Size = new System.Drawing.Size(181, 160);
            this.flowLayoutPanel_UserOptions.TabIndex = 0;
            // 
            // groupBox6
            // 
            this.groupBox6.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.groupBox6.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox6.Controls.Add(this.flowLayoutPanel_PlayerOptions);
            this.flowLayoutPanel6.SetFlowBreak(this.groupBox6, true);
            this.groupBox6.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox6.ForeColor = System.Drawing.Color.LightGray;
            this.groupBox6.Location = new System.Drawing.Point(363, 3);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(177, 180);
            this.groupBox6.TabIndex = 38;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Player Options";
            // 
            // flowLayoutPanel_PlayerOptions
            // 
            this.flowLayoutPanel_PlayerOptions.AutoSize = true;
            this.flowLayoutPanel_PlayerOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel_PlayerOptions.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel_PlayerOptions.Location = new System.Drawing.Point(3, 17);
            this.flowLayoutPanel_PlayerOptions.Name = "flowLayoutPanel_PlayerOptions";
            this.flowLayoutPanel_PlayerOptions.Size = new System.Drawing.Size(171, 160);
            this.flowLayoutPanel_PlayerOptions.TabIndex = 0;
            // 
            // ControlSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.splitContainer1);
            this.ForeColor = System.Drawing.Color.LightGray;
            this.MinimumSize = new System.Drawing.Size(640, 270);
            this.Name = "ControlSettings";
            this.Size = new System.Drawing.Size(640, 463);
            this.SizeChanged += new System.EventHandler(this.ControlSettings_SizeChanged);
            this.groupBox3.ResumeLayout(false);
            this.flowLayoutPanel_RenderOptions.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.flowLayoutPanel_ChatOptions.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.flowLayoutPanel5.ResumeLayout(false);
            this.flowLayoutPanel6.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.flowLayoutPanel9.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        //private System.Windows.Forms.CheckBox cB_DrawThreaded;
        //private System.Windows.Forms.Button btn_ClearChat;
        //private System.Windows.Forms.Button btn_ReloadChat;
        private System.Windows.Forms.Button btn_SelectFont;
        private System.Windows.Forms.RichTextBox rTB_HighlightWords;
        private System.Windows.Forms.Button btn_SaveHL;
        private System.Windows.Forms.CheckBox cB_AnimatedEmotes;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel_RenderOptions;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel_ChatOptions;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel_TextOptions;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel5;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel9;
        private System.Windows.Forms.Button btn_SelectBrowser;
        //private LX29_Twitch.Forms.VolumeControl trackB_UserBrightness = new LX29_Twitch.Forms.VolumeControl();
        private System.Windows.Forms.Button btn_Close;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel6;
        private System.Windows.Forms.CheckBox cb_ShowErrors;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btn_OpenScriptFolder;
        private System.Windows.Forms.Button btn_SelectChatBG;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel_UserOptions;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel_PlayerOptions;
    }
}
