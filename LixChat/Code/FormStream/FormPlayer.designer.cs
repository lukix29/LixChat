namespace LX29_Twitch.Forms
{
    partial class FormPlayer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPlayer));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panelVideo = new System.Windows.Forms.Panel();
            this.panelVideoControls = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btn_Chat = new System.Windows.Forms.Button();
            this.cb_Pause = new System.Windows.Forms.CheckBox();
            this.comBox_previewQuali = new System.Windows.Forms.ComboBox();
            this.cB_OnTop = new System.Windows.Forms.CheckBox();
            this.cB_Borderless = new System.Windows.Forms.CheckBox();
            this.panelResize = new System.Windows.Forms.Panel();
            this.panelMove = new System.Windows.Forms.Panel();
            this.volumeControl1 = new LX29_Twitch.Forms.VolumeControl();
            this.chatPanel1 = new LX29_ChatClient.Forms.ChatPanel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panelVideoControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 200;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(1490, 849);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 22);
            this.button1.TabIndex = 11;
            this.button1.Text = "Close";
            this.button1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panelVideo);
            this.splitContainer1.Panel1.Controls.Add(this.panelVideoControls);
            this.splitContainer1.Panel1.Controls.Add(this.button1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panelResize);
            this.splitContainer1.Panel2.Controls.Add(this.chatPanel1);
            this.splitContainer1.Size = new System.Drawing.Size(1298, 686);
            this.splitContainer1.SplitterDistance = 898;
            this.splitContainer1.TabIndex = 13;
            // 
            // panelVideo
            // 
            this.panelVideo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelVideo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.panelVideo.Location = new System.Drawing.Point(0, 0);
            this.panelVideo.Name = "panelVideo";
            this.panelVideo.Size = new System.Drawing.Size(898, 651);
            this.panelVideo.TabIndex = 17;
            this.panelVideo.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.panelVideo_MouseDoubleClick);
            // 
            // panelVideoControls
            // 
            this.panelVideoControls.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.panelVideoControls.Controls.Add(this.btnClose);
            this.panelVideoControls.Controls.Add(this.btn_Chat);
            this.panelVideoControls.Controls.Add(this.cb_Pause);
            this.panelVideoControls.Controls.Add(this.comBox_previewQuali);
            this.panelVideoControls.Controls.Add(this.cB_OnTop);
            this.panelVideoControls.Controls.Add(this.cB_Borderless);
            this.panelVideoControls.Controls.Add(this.volumeControl1);
            this.panelVideoControls.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelVideoControls.Location = new System.Drawing.Point(0, 652);
            this.panelVideoControls.Name = "panelVideoControls";
            this.panelVideoControls.Size = new System.Drawing.Size(898, 34);
            this.panelVideoControls.TabIndex = 1;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClose.AutoSize = true;
            this.btnClose.BackColor = System.Drawing.Color.Black;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(591, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(53, 28);
            this.btnClose.TabIndex = 14;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btn_Chat
            // 
            this.btn_Chat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_Chat.AutoSize = true;
            this.btn_Chat.BackColor = System.Drawing.Color.Black;
            this.btn_Chat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Chat.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Chat.Location = new System.Drawing.Point(538, 3);
            this.btn_Chat.Name = "btn_Chat";
            this.btn_Chat.Size = new System.Drawing.Size(47, 28);
            this.btn_Chat.TabIndex = 13;
            this.btn_Chat.Text = "Chat";
            this.btn_Chat.UseVisualStyleBackColor = false;
            this.btn_Chat.Click += new System.EventHandler(this.btn_Chat_Click);
            // 
            // cb_Pause
            // 
            this.cb_Pause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cb_Pause.Appearance = System.Windows.Forms.Appearance.Button;
            this.cb_Pause.BackColor = System.Drawing.Color.Black;
            this.cb_Pause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cb_Pause.Location = new System.Drawing.Point(3, 3);
            this.cb_Pause.Name = "cb_Pause";
            this.cb_Pause.Size = new System.Drawing.Size(54, 28);
            this.cb_Pause.TabIndex = 15;
            this.cb_Pause.Text = "Pause";
            this.cb_Pause.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cb_Pause.UseVisualStyleBackColor = false;
            this.cb_Pause.CheckedChanged += new System.EventHandler(this.cb_Pause_CheckedChanged);
            // 
            // comBox_previewQuali
            // 
            this.comBox_previewQuali.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.comBox_previewQuali.BackColor = System.Drawing.Color.Black;
            this.comBox_previewQuali.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comBox_previewQuali.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comBox_previewQuali.ForeColor = System.Drawing.Color.White;
            this.comBox_previewQuali.FormattingEnabled = true;
            this.comBox_previewQuali.Location = new System.Drawing.Point(63, 6);
            this.comBox_previewQuali.Name = "comBox_previewQuali";
            this.comBox_previewQuali.Size = new System.Drawing.Size(86, 24);
            this.comBox_previewQuali.TabIndex = 7;
            this.comBox_previewQuali.SelectedIndexChanged += new System.EventHandler(this.comBox_previewQuali_SelectedIndexChanged);
            // 
            // cB_OnTop
            // 
            this.cB_OnTop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cB_OnTop.AutoSize = true;
            this.cB_OnTop.BackColor = System.Drawing.Color.Black;
            this.cB_OnTop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cB_OnTop.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cB_OnTop.ForeColor = System.Drawing.Color.White;
            this.cB_OnTop.Location = new System.Drawing.Point(375, 7);
            this.cB_OnTop.Name = "cB_OnTop";
            this.cB_OnTop.Size = new System.Drawing.Size(65, 20);
            this.cB_OnTop.TabIndex = 9;
            this.cB_OnTop.Text = "On Top";
            this.cB_OnTop.UseVisualStyleBackColor = false;
            this.cB_OnTop.CheckedChanged += new System.EventHandler(this.cB_OnTop_CheckedChanged);
            // 
            // cB_Borderless
            // 
            this.cB_Borderless.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cB_Borderless.AutoSize = true;
            this.cB_Borderless.BackColor = System.Drawing.Color.Black;
            this.cB_Borderless.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cB_Borderless.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cB_Borderless.ForeColor = System.Drawing.Color.White;
            this.cB_Borderless.Location = new System.Drawing.Point(446, 7);
            this.cB_Borderless.Name = "cB_Borderless";
            this.cB_Borderless.Size = new System.Drawing.Size(86, 20);
            this.cB_Borderless.TabIndex = 10;
            this.cB_Borderless.Text = "Borderless";
            this.cB_Borderless.UseVisualStyleBackColor = false;
            this.cB_Borderless.CheckedChanged += new System.EventHandler(this.cB_Borderless_CheckedChanged);
            // 
            // panelResize
            // 
            this.panelResize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.panelResize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(20)))), ((int)(((byte)(40)))));
            this.panelResize.BackgroundImage = global::LX29_LixChat.Properties.Resources.resize;
            this.panelResize.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelResize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelResize.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.panelResize.Location = new System.Drawing.Point(377, 668);
            this.panelResize.Name = "panelResize";
            this.panelResize.Size = new System.Drawing.Size(20, 20);
            this.panelResize.TabIndex = 14;
            this.panelResize.Visible = false;
            this.panelResize.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel2_MouseDown);
            this.panelResize.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel2_MouseMove);
            this.panelResize.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel2_MouseUp);
            // 
            // panelMove
            // 
            this.panelMove.BackColor = System.Drawing.Color.Black;
            this.panelMove.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelMove.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelMove.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.panelMove.Location = new System.Drawing.Point(0, 0);
            this.panelMove.Name = "panelMove";
            this.panelMove.Size = new System.Drawing.Size(20, 20);
            this.panelMove.TabIndex = 14;
            this.panelMove.Visible = false;
            this.panelMove.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelMove_MouseDown_1);
            this.panelMove.MouseEnter += new System.EventHandler(this.panelMove_MouseEnter);
            this.panelMove.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelMove_MouseMove_1);
            this.panelMove.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelMove_MouseUp_1);
            // 
            // volumeControl1
            // 
            this.volumeControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.volumeControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.volumeControl1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.volumeControl1.ForeColor = System.Drawing.Color.Gray;
            this.volumeControl1.Location = new System.Drawing.Point(155, 7);
            this.volumeControl1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.volumeControl1.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.volumeControl1.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.volumeControl1.Name = "volumeControl1";
            this.volumeControl1.Size = new System.Drawing.Size(214, 20);
            this.volumeControl1.TabIndex = 12;
            this.volumeControl1.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // chatPanel1
            // 
            this.chatPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.chatPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chatPanel1.Location = new System.Drawing.Point(0, 0);
            this.chatPanel1.Name = "chatPanel1";
            this.chatPanel1.Size = new System.Drawing.Size(396, 686);
            this.chatPanel1.TabIndex = 0;
            // 
            // FormPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1298, 686);
            this.Controls.Add(this.panelMove);
            this.Controls.Add(this.splitContainer1);
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "FormPlayer";
            this.ShowIcon = false;
            this.Text = "FormPreviewPopout";
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(250)))), ((int)(((byte)(105)))));
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormPreviewPopout_FormClosing);
            this.Load += new System.EventHandler(this.FormPreviewPopout_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panelVideoControls.ResumeLayout(false);
            this.panelVideoControls.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        //private System.Windows.Forms.Panel panel1;
        private LX29_ChatClient.Forms.ChatPanel chatPanel1;
        private System.Windows.Forms.Panel panelVideo;
        private System.Windows.Forms.CheckBox cb_Pause;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btn_Chat;
        private VolumeControl volumeControl1;
        private System.Windows.Forms.ComboBox comBox_previewQuali;
        private System.Windows.Forms.CheckBox cB_Borderless;
        private System.Windows.Forms.CheckBox cB_OnTop;
        private System.Windows.Forms.Panel panelVideoControls;
        private System.Windows.Forms.Panel panelResize;
        private System.Windows.Forms.Panel panelMove;

    }
}