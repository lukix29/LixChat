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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cb_Pause = new System.Windows.Forms.CheckBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btn_Chat = new System.Windows.Forms.Button();
            this.comBox_previewQuali = new System.Windows.Forms.ComboBox();
            this.cB_Borderless = new System.Windows.Forms.CheckBox();
            this.cB_OnTop = new System.Windows.Forms.CheckBox();
            this.volumeControl1 = new LX29_Twitch.Forms.VolumeControl();
            this.chatPanel1 = new LX29_ChatClient.Forms.ChatPanel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panelVideo.SuspendLayout();
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
            this.button1.Location = new System.Drawing.Point(1481, 849);
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
            this.splitContainer1.Panel1.Controls.Add(this.button1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.chatPanel1);
            this.splitContainer1.Size = new System.Drawing.Size(1264, 686);
            this.splitContainer1.SplitterDistance = 889;
            this.splitContainer1.TabIndex = 13;
            // 
            // panelVideo
            // 
            this.panelVideo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.panelVideo.Controls.Add(this.groupBox1);
            this.panelVideo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelVideo.Location = new System.Drawing.Point(0, 0);
            this.panelVideo.Name = "panelVideo";
            this.panelVideo.Size = new System.Drawing.Size(889, 686);
            this.panelVideo.TabIndex = 17;
            // 
            // groupBox1
            // 
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox1.Location = new System.Drawing.Point(130, 239);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(286, 223);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Visible = false;
            // 
            // cb_Pause
            // 
            this.cb_Pause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cb_Pause.Appearance = System.Windows.Forms.Appearance.Button;
            this.cb_Pause.BackColor = System.Drawing.Color.Black;
            this.cb_Pause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cb_Pause.Location = new System.Drawing.Point(3, 658);
            this.cb_Pause.Name = "cb_Pause";
            this.cb_Pause.Size = new System.Drawing.Size(54, 25);
            this.cb_Pause.TabIndex = 15;
            this.cb_Pause.Text = "Pause";
            this.cb_Pause.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cb_Pause.UseVisualStyleBackColor = false;
            this.cb_Pause.CheckedChanged += new System.EventHandler(this.cb_Pause_CheckedChanged);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.AutoSize = true;
            this.btnClose.BackColor = System.Drawing.Color.Black;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(834, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(53, 28);
            this.btnClose.TabIndex = 14;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btn_Chat
            // 
            this.btn_Chat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Chat.AutoSize = true;
            this.btn_Chat.BackColor = System.Drawing.Color.Black;
            this.btn_Chat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Chat.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Chat.Location = new System.Drawing.Point(840, 656);
            this.btn_Chat.Name = "btn_Chat";
            this.btn_Chat.Size = new System.Drawing.Size(47, 28);
            this.btn_Chat.TabIndex = 13;
            this.btn_Chat.Text = "Chat";
            this.btn_Chat.UseVisualStyleBackColor = false;
            this.btn_Chat.Click += new System.EventHandler(this.btn_Chat_Click);
            // 
            // comBox_previewQuali
            // 
            this.comBox_previewQuali.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.comBox_previewQuali.BackColor = System.Drawing.Color.Black;
            this.comBox_previewQuali.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comBox_previewQuali.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comBox_previewQuali.ForeColor = System.Drawing.Color.White;
            this.comBox_previewQuali.FormattingEnabled = true;
            this.comBox_previewQuali.Location = new System.Drawing.Point(63, 659);
            this.comBox_previewQuali.Name = "comBox_previewQuali";
            this.comBox_previewQuali.Size = new System.Drawing.Size(86, 24);
            this.comBox_previewQuali.TabIndex = 7;
            this.comBox_previewQuali.SelectedIndexChanged += new System.EventHandler(this.comBox_previewQuali_SelectedIndexChanged);
            // 
            // cB_Borderless
            // 
            this.cB_Borderless.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cB_Borderless.AutoSize = true;
            this.cB_Borderless.BackColor = System.Drawing.Color.Black;
            this.cB_Borderless.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cB_Borderless.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cB_Borderless.ForeColor = System.Drawing.Color.White;
            this.cB_Borderless.Location = new System.Drawing.Point(748, 660);
            this.cB_Borderless.Name = "cB_Borderless";
            this.cB_Borderless.Size = new System.Drawing.Size(86, 20);
            this.cB_Borderless.TabIndex = 10;
            this.cB_Borderless.Text = "Borderless";
            this.cB_Borderless.UseVisualStyleBackColor = false;
            this.cB_Borderless.CheckedChanged += new System.EventHandler(this.cB_Borderless_CheckedChanged);
            // 
            // cB_OnTop
            // 
            this.cB_OnTop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cB_OnTop.AutoSize = true;
            this.cB_OnTop.BackColor = System.Drawing.Color.Black;
            this.cB_OnTop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cB_OnTop.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cB_OnTop.ForeColor = System.Drawing.Color.White;
            this.cB_OnTop.Location = new System.Drawing.Point(677, 660);
            this.cB_OnTop.Name = "cB_OnTop";
            this.cB_OnTop.Size = new System.Drawing.Size(65, 20);
            this.cB_OnTop.TabIndex = 9;
            this.cB_OnTop.Text = "On Top";
            this.cB_OnTop.UseVisualStyleBackColor = false;
            this.cB_OnTop.CheckedChanged += new System.EventHandler(this.cB_OnTop_CheckedChanged);
            // 
            // volumeControl1
            // 
            this.volumeControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.volumeControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.volumeControl1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.volumeControl1.ForeColor = System.Drawing.Color.Gray;
            this.volumeControl1.Location = new System.Drawing.Point(155, 660);
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
            this.volumeControl1.Size = new System.Drawing.Size(246, 20);
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
            this.chatPanel1.Size = new System.Drawing.Size(371, 686);
            this.chatPanel1.TabIndex = 0;
            // 
            // FormPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1264, 686);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btn_Chat);
            this.Controls.Add(this.cB_Borderless);
            this.Controls.Add(this.cB_OnTop);
            this.Controls.Add(this.cb_Pause);
            this.Controls.Add(this.volumeControl1);
            this.Controls.Add(this.comBox_previewQuali);
            this.Controls.Add(this.splitContainer1);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "FormPlayer";
            this.Text = "FormPreviewPopout";
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(250)))), ((int)(((byte)(105)))));
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormPreviewPopout_FormClosing);
            this.Load += new System.EventHandler(this.FormPreviewPopout_Load);
            this.ResizeEnd += new System.EventHandler(this.FormPreviewPopout_ResizeEnd);
            this.LocationChanged += new System.EventHandler(this.FormPreviewPopout_LocationChanged);
            this.MouseEnter += new System.EventHandler(this.FormPlayer_MouseEnter);
            this.Resize += new System.EventHandler(this.FormPreviewPopout_Resize);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panelVideo.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.GroupBox groupBox1;

    }
}