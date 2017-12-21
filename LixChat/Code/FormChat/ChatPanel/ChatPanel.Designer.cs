namespace LX29_ChatClient.Forms
{
    partial class ChatPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChatPanel));
            this.rTB_Send = new System.Windows.Forms.RichTextBox();
            this.lstB_Search = new System.Windows.Forms.ListBox();
            this.btn_Send = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.chatView1 = new LX29_ChatClient.Forms.ChatView();
            this.userInfoPanel1 = new LX29_ChatClient.Forms.UserInfoPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.tS_Btn_Emotes = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rTB_Send
            // 
            this.rTB_Send.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rTB_Send.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.rTB_Send.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rTB_Send.DetectUrls = false;
            this.rTB_Send.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rTB_Send.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.rTB_Send.Location = new System.Drawing.Point(5, 519);
            this.rTB_Send.MaxLength = 500;
            this.rTB_Send.MinimumSize = new System.Drawing.Size(10, 10);
            this.rTB_Send.Name = "rTB_Send";
            this.rTB_Send.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rTB_Send.Size = new System.Drawing.Size(564, 53);
            this.rTB_Send.TabIndex = 1;
            this.rTB_Send.Text = "";
            this.rTB_Send.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            this.rTB_Send.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rTB_Send_KeyDown);
            this.rTB_Send.KeyUp += new System.Windows.Forms.KeyEventHandler(this.rTB_Send_KeyUp);
            // 
            // lstB_Search
            // 
            this.lstB_Search.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lstB_Search.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.lstB_Search.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstB_Search.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lstB_Search.FormattingEnabled = true;
            this.lstB_Search.ItemHeight = 19;
            this.lstB_Search.Location = new System.Drawing.Point(3, 319);
            this.lstB_Search.Name = "lstB_Search";
            this.lstB_Search.Size = new System.Drawing.Size(266, 194);
            this.lstB_Search.TabIndex = 2;
            this.lstB_Search.Visible = false;
            this.lstB_Search.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lstB_Search_DrawItem);
            this.lstB_Search.SelectedIndexChanged += new System.EventHandler(this.lstB_Search_SelectedIndexChanged);
            this.lstB_Search.VisibleChanged += new System.EventHandler(this.lstB_Search_VisibleChanged);
            this.lstB_Search.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstB_Search_MouseDoubleClick);
            // 
            // btn_Send
            // 
            this.btn_Send.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_Send.AutoEllipsis = true;
            this.btn_Send.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(65)))), ((int)(((byte)(164)))));
            this.btn_Send.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Send.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Send.ForeColor = System.Drawing.Color.White;
            this.btn_Send.Location = new System.Drawing.Point(5, 578);
            this.btn_Send.Name = "btn_Send";
            this.btn_Send.Size = new System.Drawing.Size(48, 30);
            this.btn_Send.TabIndex = 5;
            this.btn_Send.TabStop = false;
            this.btn_Send.Text = "Chat";
            this.btn_Send.UseVisualStyleBackColor = false;
            this.btn_Send.Click += new System.EventHandler(this.btn_Send_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // chatView1
            // 
            this.chatView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chatView1.BackColor = System.Drawing.Color.Black;
            this.chatView1.Location = new System.Drawing.Point(0, 21);
            this.chatView1.Name = "chatView1";
            this.chatView1.Size = new System.Drawing.Size(572, 492);
            this.chatView1.TabIndex = 0;
            this.chatView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.chatView1_MouseDown);
            this.chatView1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.chatView1_MouseUp);
            // 
            // userInfoPanel1
            // 
            this.userInfoPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(20)))), ((int)(((byte)(40)))));
            this.userInfoPanel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.userInfoPanel1.Location = new System.Drawing.Point(96, 111);
            this.userInfoPanel1.Name = "userInfoPanel1";
            this.userInfoPanel1.Size = new System.Drawing.Size(339, 148);
            this.userInfoPanel1.SplitterDistance = 178;
            this.userInfoPanel1.TabIndex = 6;
            this.userInfoPanel1.Visible = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.pictureBox1.Image = global::LX29_LixChat.Properties.Resources.loading;
            this.pictureBox1.Location = new System.Drawing.Point(4, 384);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(128, 128);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton2,
            this.toolStripButton1,
            this.toolStripButton3,
            this.tS_Btn_Emotes});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(572, 25);
            this.toolStrip1.TabIndex = 16;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.BackColor = System.Drawing.Color.Black;
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton2.ForeColor = System.Drawing.Color.White;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(79, 22);
            this.toolStripButton2.Text = "All Messages";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.BackColor = System.Drawing.Color.Black;
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.ForeColor = System.Drawing.Color.White;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(66, 22);
            this.toolStripButton1.Text = "Highlights";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.BackColor = System.Drawing.Color.Black;
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton3.ForeColor = System.Drawing.Color.White;
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(62, 22);
            this.toolStripButton3.Text = "Outgoing";
            this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton3_Click);
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
            // ChatPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.userInfoPanel1);
            this.Controls.Add(this.btn_Send);
            this.Controls.Add(this.lstB_Search);
            this.Controls.Add(this.rTB_Send);
            this.Controls.Add(this.chatView1);
            this.Name = "ChatPanel";
            this.Size = new System.Drawing.Size(572, 612);
            this.Load += new System.EventHandler(this.ChatPanel_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private LX29_ChatClient.Forms.ChatView chatView1 = new ChatView();
        private System.Windows.Forms.RichTextBox rTB_Send;
        private System.Windows.Forms.ListBox lstB_Search;
        private System.Windows.Forms.Button btn_Send;
        private System.Windows.Forms.Timer timer1;
        private UserInfoPanel userInfoPanel1 = new UserInfoPanel();
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tS_Btn_Emotes;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
    }
}
