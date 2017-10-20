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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPlayer));
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.btn_Close = new System.Windows.Forms.Button();
            this.btn_ShowChat = new System.Windows.Forms.Button();
            this.playerControl = new LX29_LixChat.Code.FormStream.Player.PlayerControl();
            this.panelResize = new System.Windows.Forms.Panel();
            this.chatPanel1 = new LX29_ChatClient.Forms.ChatPanel();
            this.panelMove = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.btn_Close);
            this.splitContainer.Panel1.Controls.Add(this.btn_ShowChat);
            this.splitContainer.Panel1.Controls.Add(this.playerControl);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.panelResize);
            this.splitContainer.Panel2.Controls.Add(this.chatPanel1);
            this.splitContainer.Size = new System.Drawing.Size(1298, 686);
            this.splitContainer.SplitterDistance = 900;
            this.splitContainer.TabIndex = 13;
            // 
            // btn_Close
            // 
            this.btn_Close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Close.BackgroundImage = global::LX29_LixChat.Properties.Resources.close;
            this.btn_Close.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btn_Close.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_Close.Location = new System.Drawing.Point(877, 3);
            this.btn_Close.Name = "btn_Close";
            this.btn_Close.Size = new System.Drawing.Size(20, 20);
            this.btn_Close.TabIndex = 2;
            this.btn_Close.UseVisualStyleBackColor = true;
            this.btn_Close.Visible = false;
            this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
            this.btn_Close.MouseEnter += new System.EventHandler(this.button1_MouseEnter);
            // 
            // btn_ShowChat
            // 
            this.btn_ShowChat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_ShowChat.AutoSize = true;
            this.btn_ShowChat.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_ShowChat.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_ShowChat.ForeColor = System.Drawing.Color.Gainsboro;
            this.btn_ShowChat.Location = new System.Drawing.Point(810, 652);
            this.btn_ShowChat.Name = "btn_ShowChat";
            this.btn_ShowChat.Size = new System.Drawing.Size(87, 29);
            this.btn_ShowChat.TabIndex = 1;
            this.btn_ShowChat.Text = "Show Chat";
            this.btn_ShowChat.UseVisualStyleBackColor = true;
            this.btn_ShowChat.Click += new System.EventHandler(this.btn_ShowChat_Click);
            // 
            // playerControl
            // 
            this.playerControl.AutoSize = true;
            this.playerControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.playerControl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.playerControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.playerControl.Location = new System.Drawing.Point(0, 0);
            this.playerControl.Name = "playerControl";
            this.playerControl.Size = new System.Drawing.Size(900, 686);
            this.playerControl.TabIndex = 0;
            this.playerControl.OnBordelessChanged += new LX29_LixChat.Code.FormStream.Player.PlayerControl.BordelessChanged(this.playerControl1_OnBordelessChanged);
            this.playerControl.OnPlayerControlsShown += new LX29_LixChat.Code.FormStream.Player.PlayerControl.PlayerControlsShown(this.playerControl1_OnPlayerControlsShown);
            this.playerControl.OnTopMostChanged += new LX29_LixChat.Code.FormStream.Player.PlayerControl.OnTopChanged(this.playerControl1_OnTopMostChanged);
            this.playerControl.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.playerControl1_MouseDoubleClick);
            // 
            // panelResize
            // 
            this.panelResize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.panelResize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(20)))), ((int)(((byte)(40)))));
            this.panelResize.BackgroundImage = global::LX29_LixChat.Properties.Resources.resize;
            this.panelResize.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelResize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelResize.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.panelResize.Location = new System.Drawing.Point(375, 668);
            this.panelResize.Name = "panelResize";
            this.panelResize.Size = new System.Drawing.Size(20, 20);
            this.panelResize.TabIndex = 14;
            this.panelResize.Visible = false;
            this.panelResize.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel2_MouseDown);
            this.panelResize.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel2_MouseMove);
            this.panelResize.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel2_MouseUp);
            // 
            // chatPanel1
            // 
            this.chatPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.chatPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chatPanel1.Location = new System.Drawing.Point(0, 0);
            this.chatPanel1.Name = "chatPanel1";
            this.chatPanel1.Size = new System.Drawing.Size(394, 686);
            this.chatPanel1.TabIndex = 0;
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
            // FormPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1298, 686);
            this.Controls.Add(this.panelMove);
            this.Controls.Add(this.splitContainer);
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "FormPlayer";
            this.ShowIcon = false;
            this.Text = "FormPreviewPopout";
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(250)))), ((int)(((byte)(105)))));
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormPreviewPopout_FormClosing);
            this.Load += new System.EventHandler(this.FormPreviewPopout_Load);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        //private System.Windows.Forms.Panel panel1;
        private LX29_ChatClient.Forms.ChatPanel chatPanel1;
        private System.Windows.Forms.Panel panelResize;
        private System.Windows.Forms.Panel panelMove;
        private LX29_LixChat.Code.FormStream.Player.PlayerControl playerControl;
        private System.Windows.Forms.Button btn_ShowChat;
        private System.Windows.Forms.Button btn_Close;

    }
}