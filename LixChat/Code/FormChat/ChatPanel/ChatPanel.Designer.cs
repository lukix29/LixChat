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
            this.rTB_Send = new System.Windows.Forms.RichTextBox();
            this.lstB_Search = new System.Windows.Forms.ListBox();
            this.btn_Send = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.chatView1 = new LX29_ChatClient.Forms.ChatView();
            this.userInfoPanel1 = new LX29_ChatClient.Forms.UserInfoPanel();
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
            this.chatView1.Location = new System.Drawing.Point(0, 0);
            this.chatView1.Name = "chatView1";
            this.chatView1.Size = new System.Drawing.Size(572, 513);
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
            this.userInfoPanel1.TabIndex = 6;
            this.userInfoPanel1.Visible = false;
            // 
            // ChatPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.Controls.Add(this.userInfoPanel1);
            this.Controls.Add(this.btn_Send);
            this.Controls.Add(this.lstB_Search);
            this.Controls.Add(this.rTB_Send);
            this.Controls.Add(this.chatView1);
            this.Name = "ChatPanel";
            this.Size = new System.Drawing.Size(572, 612);
            this.Load += new System.EventHandler(this.ChatPanel_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private LX29_ChatClient.Forms.ChatView chatView1 = new ChatView();
        private System.Windows.Forms.RichTextBox rTB_Send;
        private System.Windows.Forms.ListBox lstB_Search;
        private System.Windows.Forms.Button btn_Send;
        private System.Windows.Forms.Timer timer1;
        private UserInfoPanel userInfoPanel1 = new UserInfoPanel();
    }
}
