namespace LX29_LixChat
{
    partial class Main
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
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
                //mpv.Dispose();
                mpvPreview.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.splitContainer_Preview = new System.Windows.Forms.SplitContainer();
            this.playerControl1 = new LX29_LixChat.Code.FormStream.Player.PlayerControl();
            this.lbl_preview = new System.Windows.Forms.Label();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.btn_StartStream = new System.Windows.Forms.Button();
            this.btn_External = new System.Windows.Forms.Button();
            this.btn_Record = new System.Windows.Forms.Button();
            this.comBox_StreamQuali = new System.Windows.Forms.ComboBox();
            this.btn_Show_Video_Info = new System.Windows.Forms.Button();
            this.splitC_Main = new System.Windows.Forms.SplitContainer();
            this.lstB_Channels = new LX29_ChatClient.Forms.ChannelListBox();
            this.cMS_ListBox = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tSMI_OpenChatInBrowser = new System.Windows.Forms.ToolStripMenuItem();
            this.openStreamInBrowserPopoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openChannelInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tLP_Preview = new System.Windows.Forms.TableLayoutPanel();
            this.pn_Preview = new System.Windows.Forms.Panel();
            this.splitContainer_ChatPreview = new System.Windows.Forms.SplitContainer();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btn_OpenChat = new System.Windows.Forms.Button();
            this.btn_Reconnect = new System.Windows.Forms.Button();
            this.cB_AutoLogin = new System.Windows.Forms.CheckBox();
            this.btn_Disconnect = new System.Windows.Forms.Button();
            this.cB_Favorite = new System.Windows.Forms.CheckBox();
            this.btn_Follow = new System.Windows.Forms.Button();
            this.btn_ShowStreamInfoPanels = new System.Windows.Forms.Button();
            this.btn_ShowUsers = new System.Windows.Forms.Button();
            this.btn_AutoChatActions = new System.Windows.Forms.Button();
            this.cB_LogChat = new System.Windows.Forms.CheckBox();
            this.btn_openSubpage = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.apiInfoPanel1 = new LX29_Twitch.Forms.ApiInfoPanel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btn_AddChannel = new System.Windows.Forms.Button();
            this.btn_removeChannel = new System.Windows.Forms.Button();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.tSMi_Accounts = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshChannelsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tSMi_ReloadEmotes = new System.Windows.Forms.ToolStripMenuItem();
            this.tSMi_Sync = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMi_SyncOnlyOnline = new System.Windows.Forms.ToolStripMenuItem();
            this.tSMi_SyncAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tSMi_ReconnectChat = new System.Windows.Forms.ToolStripMenuItem();
            this.tSMi_ShowSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.tSMi_Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.tSDDB_About_Menue = new System.Windows.Forms.ToolStripDropDownButton();
            this.tSMI_UpdateProgramm = new System.Windows.Forms.ToolStripMenuItem();
            this.tSMI_About = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsLbl_Infotext = new System.Windows.Forms.ToolStripLabel();
            this.tSProgBar_Loading = new System.Windows.Forms.ToolStripProgressBar();
            this.tsLabel_Info = new System.Windows.Forms.ToolStripLabel();
            this.toolTip_Main = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_Preview)).BeginInit();
            this.splitContainer_Preview.Panel1.SuspendLayout();
            this.splitContainer_Preview.Panel2.SuspendLayout();
            this.splitContainer_Preview.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitC_Main)).BeginInit();
            this.splitC_Main.Panel1.SuspendLayout();
            this.splitC_Main.Panel2.SuspendLayout();
            this.splitC_Main.SuspendLayout();
            this.cMS_ListBox.SuspendLayout();
            this.tLP_Preview.SuspendLayout();
            this.pn_Preview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_ChatPreview)).BeginInit();
            this.splitContainer_ChatPreview.Panel1.SuspendLayout();
            this.splitContainer_ChatPreview.Panel2.SuspendLayout();
            this.splitContainer_ChatPreview.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer_Preview
            // 
            resources.ApplyResources(this.splitContainer_Preview, "splitContainer_Preview");
            this.splitContainer_Preview.Name = "splitContainer_Preview";
            // 
            // splitContainer_Preview.Panel1
            // 
            this.splitContainer_Preview.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            resources.ApplyResources(this.splitContainer_Preview.Panel1, "splitContainer_Preview.Panel1");
            this.splitContainer_Preview.Panel1.Controls.Add(this.playerControl1);
            this.splitContainer_Preview.Panel1.Controls.Add(this.lbl_preview);
            this.splitContainer_Preview.Panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer2_Panel1_Paint);
            // 
            // splitContainer_Preview.Panel2
            // 
            this.splitContainer_Preview.Panel2.Controls.Add(this.flowLayoutPanel2);
            resources.ApplyResources(this.splitContainer_Preview.Panel2, "splitContainer_Preview.Panel2");
            // 
            // playerControl1
            // 
            resources.ApplyResources(this.playerControl1, "playerControl1");
            this.playerControl1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.playerControl1.Name = "playerControl1";
            this.playerControl1.PreviewImage = ((System.Drawing.Image)(resources.GetObject("playerControl1.PreviewImage")));
            this.playerControl1.Quality = null;
            this.playerControl1.ShowOnTopBorderless = false;
            this.playerControl1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.playerControl1.Stream = null;
            // 
            // lbl_preview
            // 
            resources.ApplyResources(this.lbl_preview, "lbl_preview");
            this.lbl_preview.BackColor = System.Drawing.Color.Transparent;
            this.lbl_preview.Name = "lbl_preview";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.flowLayoutPanel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowLayoutPanel2.Controls.Add(this.btn_StartStream);
            this.flowLayoutPanel2.Controls.Add(this.btn_External);
            this.flowLayoutPanel2.Controls.Add(this.btn_Record);
            this.flowLayoutPanel2.Controls.Add(this.comBox_StreamQuali);
            this.flowLayoutPanel2.Controls.Add(this.btn_Show_Video_Info);
            resources.ApplyResources(this.flowLayoutPanel2, "flowLayoutPanel2");
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            // 
            // btn_StartStream
            // 
            resources.ApplyResources(this.btn_StartStream, "btn_StartStream");
            this.btn_StartStream.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.btn_StartStream.ForeColor = System.Drawing.Color.Gainsboro;
            this.btn_StartStream.Name = "btn_StartStream";
            this.btn_StartStream.TabStop = false;
            this.btn_StartStream.UseVisualStyleBackColor = false;
            this.btn_StartStream.Click += new System.EventHandler(this.btn_StartStream_Click);
            // 
            // btn_External
            // 
            this.btn_External.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            resources.ApplyResources(this.btn_External, "btn_External");
            this.btn_External.ForeColor = System.Drawing.Color.Gainsboro;
            this.btn_External.Name = "btn_External";
            this.btn_External.TabStop = false;
            this.btn_External.UseVisualStyleBackColor = false;
            this.btn_External.Click += new System.EventHandler(this.btn_External_Click);
            // 
            // btn_Record
            // 
            resources.ApplyResources(this.btn_Record, "btn_Record");
            this.btn_Record.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.btn_Record.ForeColor = System.Drawing.Color.Gainsboro;
            this.btn_Record.Name = "btn_Record";
            this.btn_Record.TabStop = false;
            this.btn_Record.UseVisualStyleBackColor = false;
            this.btn_Record.Click += new System.EventHandler(this.btn_Record_Click);
            // 
            // comBox_StreamQuali
            // 
            this.comBox_StreamQuali.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            resources.ApplyResources(this.comBox_StreamQuali, "comBox_StreamQuali");
            this.comBox_StreamQuali.ForeColor = System.Drawing.Color.Gainsboro;
            this.comBox_StreamQuali.FormattingEnabled = true;
            this.comBox_StreamQuali.Name = "comBox_StreamQuali";
            this.comBox_StreamQuali.SelectedIndexChanged += new System.EventHandler(this.comBox_StreamQuali_SelectedIndexChanged);
            // 
            // btn_Show_Video_Info
            // 
            resources.ApplyResources(this.btn_Show_Video_Info, "btn_Show_Video_Info");
            this.btn_Show_Video_Info.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.btn_Show_Video_Info.ForeColor = System.Drawing.Color.Gainsboro;
            this.btn_Show_Video_Info.Name = "btn_Show_Video_Info";
            this.btn_Show_Video_Info.TabStop = false;
            this.btn_Show_Video_Info.UseVisualStyleBackColor = false;
            this.btn_Show_Video_Info.Click += new System.EventHandler(this.btn_Show_Video_Info_Click);
            // 
            // splitC_Main
            // 
            resources.ApplyResources(this.splitC_Main, "splitC_Main");
            this.splitC_Main.Name = "splitC_Main";
            // 
            // splitC_Main.Panel1
            // 
            this.splitC_Main.Panel1.Controls.Add(this.lstB_Channels);
            // 
            // splitC_Main.Panel2
            // 
            this.splitC_Main.Panel2.Controls.Add(this.tLP_Preview);
            // 
            // lstB_Channels
            // 
            this.lstB_Channels.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.lstB_Channels.ContextMenuStrip = this.cMS_ListBox;
            resources.ApplyResources(this.lstB_Channels, "lstB_Channels");
            this.lstB_Channels.Name = "lstB_Channels";
            this.lstB_Channels.SelectedIndex = 0;
            this.lstB_Channels.SelectedIndexChanged += new LX29_ChatClient.Forms.ChannelListBox.OnSelectedIndexChanged(this.lstB_Channels_SelectedIndexChanged);
            this.lstB_Channels.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstB_Channels_MouseDoubleClick);
            // 
            // cMS_ListBox
            // 
            this.cMS_ListBox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tSMI_OpenChatInBrowser,
            this.openStreamInBrowserPopoutToolStripMenuItem,
            this.openChannelInBrowserToolStripMenuItem});
            this.cMS_ListBox.Name = "cMS_ListBox";
            resources.ApplyResources(this.cMS_ListBox, "cMS_ListBox");
            this.cMS_ListBox.Opening += new System.ComponentModel.CancelEventHandler(this.cMS_ListBox_Opening);
            // 
            // tSMI_OpenChatInBrowser
            // 
            this.tSMI_OpenChatInBrowser.Name = "tSMI_OpenChatInBrowser";
            resources.ApplyResources(this.tSMI_OpenChatInBrowser, "tSMI_OpenChatInBrowser");
            this.tSMI_OpenChatInBrowser.Click += new System.EventHandler(this.tSMI_OpenChatInBrowser_Click);
            // 
            // openStreamInBrowserPopoutToolStripMenuItem
            // 
            this.openStreamInBrowserPopoutToolStripMenuItem.Name = "openStreamInBrowserPopoutToolStripMenuItem";
            resources.ApplyResources(this.openStreamInBrowserPopoutToolStripMenuItem, "openStreamInBrowserPopoutToolStripMenuItem");
            this.openStreamInBrowserPopoutToolStripMenuItem.Click += new System.EventHandler(this.openStreamInBrowserPopoutToolStripMenuItem_Click);
            // 
            // openChannelInBrowserToolStripMenuItem
            // 
            this.openChannelInBrowserToolStripMenuItem.Name = "openChannelInBrowserToolStripMenuItem";
            resources.ApplyResources(this.openChannelInBrowserToolStripMenuItem, "openChannelInBrowserToolStripMenuItem");
            this.openChannelInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openChannelInBrowserToolStripMenuItem_Click);
            // 
            // tLP_Preview
            // 
            resources.ApplyResources(this.tLP_Preview, "tLP_Preview");
            this.tLP_Preview.Controls.Add(this.pn_Preview, 0, 1);
            this.tLP_Preview.Controls.Add(this.splitContainer_ChatPreview, 0, 0);
            this.tLP_Preview.Name = "tLP_Preview";
            // 
            // pn_Preview
            // 
            resources.ApplyResources(this.pn_Preview, "pn_Preview");
            this.pn_Preview.Controls.Add(this.splitContainer_Preview);
            this.pn_Preview.Name = "pn_Preview";
            // 
            // splitContainer_ChatPreview
            // 
            resources.ApplyResources(this.splitContainer_ChatPreview, "splitContainer_ChatPreview");
            this.splitContainer_ChatPreview.Name = "splitContainer_ChatPreview";
            // 
            // splitContainer_ChatPreview.Panel1
            // 
            this.splitContainer_ChatPreview.Panel1.Controls.Add(this.flowLayoutPanel1);
            // 
            // splitContainer_ChatPreview.Panel2
            // 
            this.splitContainer_ChatPreview.Panel2.Controls.Add(this.apiInfoPanel1);
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.flowLayoutPanel1.Controls.Add(this.btn_OpenChat);
            this.flowLayoutPanel1.Controls.Add(this.btn_Reconnect);
            this.flowLayoutPanel1.Controls.Add(this.cB_AutoLogin);
            this.flowLayoutPanel1.Controls.Add(this.btn_Disconnect);
            this.flowLayoutPanel1.Controls.Add(this.cB_Favorite);
            this.flowLayoutPanel1.Controls.Add(this.btn_Follow);
            this.flowLayoutPanel1.Controls.Add(this.btn_ShowStreamInfoPanels);
            this.flowLayoutPanel1.Controls.Add(this.btn_ShowUsers);
            this.flowLayoutPanel1.Controls.Add(this.btn_AutoChatActions);
            this.flowLayoutPanel1.Controls.Add(this.cB_LogChat);
            this.flowLayoutPanel1.Controls.Add(this.btn_openSubpage);
            this.flowLayoutPanel1.Controls.Add(this.groupBox1);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.flowLayoutPanel1_Paint);
            // 
            // btn_OpenChat
            // 
            resources.ApplyResources(this.btn_OpenChat, "btn_OpenChat");
            this.btn_OpenChat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.btn_OpenChat.ForeColor = System.Drawing.Color.Gainsboro;
            this.btn_OpenChat.Name = "btn_OpenChat";
            this.btn_OpenChat.TabStop = false;
            this.btn_OpenChat.UseVisualStyleBackColor = false;
            this.btn_OpenChat.Click += new System.EventHandler(this.btn_OpenChat_Click);
            // 
            // btn_Reconnect
            // 
            resources.ApplyResources(this.btn_Reconnect, "btn_Reconnect");
            this.btn_Reconnect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.btn_Reconnect.ForeColor = System.Drawing.Color.Gainsboro;
            this.btn_Reconnect.Name = "btn_Reconnect";
            this.btn_Reconnect.TabStop = false;
            this.btn_Reconnect.UseVisualStyleBackColor = false;
            this.btn_Reconnect.Click += new System.EventHandler(this.button1_Click);
            // 
            // cB_AutoLogin
            // 
            resources.ApplyResources(this.cB_AutoLogin, "cB_AutoLogin");
            this.cB_AutoLogin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.cB_AutoLogin.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.cB_AutoLogin.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Navy;
            this.cB_AutoLogin.ForeColor = System.Drawing.Color.Gainsboro;
            this.cB_AutoLogin.Name = "cB_AutoLogin";
            this.cB_AutoLogin.TabStop = false;
            this.cB_AutoLogin.UseVisualStyleBackColor = false;
            this.cB_AutoLogin.CheckedChanged += new System.EventHandler(this.cB_AutoLogin_CheckedChanged);
            // 
            // btn_Disconnect
            // 
            resources.ApplyResources(this.btn_Disconnect, "btn_Disconnect");
            this.btn_Disconnect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.btn_Disconnect.ForeColor = System.Drawing.Color.Gainsboro;
            this.btn_Disconnect.Name = "btn_Disconnect";
            this.btn_Disconnect.TabStop = false;
            this.btn_Disconnect.UseVisualStyleBackColor = false;
            this.btn_Disconnect.Click += new System.EventHandler(this.btn_Disconnect_Click);
            // 
            // cB_Favorite
            // 
            resources.ApplyResources(this.cB_Favorite, "cB_Favorite");
            this.cB_Favorite.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.cB_Favorite.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.cB_Favorite.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Navy;
            this.cB_Favorite.ForeColor = System.Drawing.Color.Gainsboro;
            this.cB_Favorite.Name = "cB_Favorite";
            this.cB_Favorite.TabStop = false;
            this.toolTip_Main.SetToolTip(this.cB_Favorite, resources.GetString("cB_Favorite.ToolTip"));
            this.cB_Favorite.UseVisualStyleBackColor = false;
            this.cB_Favorite.CheckedChanged += new System.EventHandler(this.cB_Favorite_CheckedChanged);
            // 
            // btn_Follow
            // 
            this.btn_Follow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            resources.ApplyResources(this.btn_Follow, "btn_Follow");
            this.btn_Follow.ForeColor = System.Drawing.Color.Gainsboro;
            this.btn_Follow.Name = "btn_Follow";
            this.btn_Follow.TabStop = false;
            this.btn_Follow.UseVisualStyleBackColor = false;
            this.btn_Follow.Click += new System.EventHandler(this.button1_Click_2);
            // 
            // btn_ShowStreamInfoPanels
            // 
            this.btn_ShowStreamInfoPanels.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            resources.ApplyResources(this.btn_ShowStreamInfoPanels, "btn_ShowStreamInfoPanels");
            this.btn_ShowStreamInfoPanels.ForeColor = System.Drawing.Color.Gainsboro;
            this.btn_ShowStreamInfoPanels.Name = "btn_ShowStreamInfoPanels";
            this.btn_ShowStreamInfoPanels.TabStop = false;
            this.toolTip_Main.SetToolTip(this.btn_ShowStreamInfoPanels, resources.GetString("btn_ShowStreamInfoPanels.ToolTip"));
            this.btn_ShowStreamInfoPanels.UseVisualStyleBackColor = false;
            this.btn_ShowStreamInfoPanels.Click += new System.EventHandler(this.button2_Click);
            // 
            // btn_ShowUsers
            // 
            this.btn_ShowUsers.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            resources.ApplyResources(this.btn_ShowUsers, "btn_ShowUsers");
            this.btn_ShowUsers.ForeColor = System.Drawing.Color.Gainsboro;
            this.btn_ShowUsers.Name = "btn_ShowUsers";
            this.btn_ShowUsers.TabStop = false;
            this.toolTip_Main.SetToolTip(this.btn_ShowUsers, resources.GetString("btn_ShowUsers.ToolTip"));
            this.btn_ShowUsers.UseVisualStyleBackColor = false;
            this.btn_ShowUsers.Click += new System.EventHandler(this.btn_ShowUsers_Click);
            // 
            // btn_AutoChatActions
            // 
            this.btn_AutoChatActions.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            resources.ApplyResources(this.btn_AutoChatActions, "btn_AutoChatActions");
            this.btn_AutoChatActions.ForeColor = System.Drawing.Color.Gainsboro;
            this.btn_AutoChatActions.Name = "btn_AutoChatActions";
            this.btn_AutoChatActions.TabStop = false;
            this.toolTip_Main.SetToolTip(this.btn_AutoChatActions, resources.GetString("btn_AutoChatActions.ToolTip"));
            this.btn_AutoChatActions.UseVisualStyleBackColor = false;
            this.btn_AutoChatActions.Click += new System.EventHandler(this.btn_AutoChatActions_Click);
            // 
            // cB_LogChat
            // 
            resources.ApplyResources(this.cB_LogChat, "cB_LogChat");
            this.cB_LogChat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.cB_LogChat.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.cB_LogChat.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Navy;
            this.cB_LogChat.ForeColor = System.Drawing.Color.Gainsboro;
            this.cB_LogChat.Name = "cB_LogChat";
            this.cB_LogChat.TabStop = false;
            this.toolTip_Main.SetToolTip(this.cB_LogChat, resources.GetString("cB_LogChat.ToolTip"));
            this.cB_LogChat.UseVisualStyleBackColor = false;
            this.cB_LogChat.CheckedChanged += new System.EventHandler(this.cB_LogChat_CheckedChanged);
            // 
            // btn_openSubpage
            // 
            this.btn_openSubpage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            resources.ApplyResources(this.btn_openSubpage, "btn_openSubpage");
            this.btn_openSubpage.ForeColor = System.Drawing.Color.Gainsboro;
            this.btn_openSubpage.Name = "btn_openSubpage";
            this.btn_openSubpage.TabStop = false;
            this.btn_openSubpage.UseVisualStyleBackColor = false;
            this.btn_openSubpage.Click += new System.EventHandler(this.btn_openSubpage_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.treeView1);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.groupBox1.ForeColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // treeView1
            // 
            this.treeView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.treeView1, "treeView1");
            this.treeView1.ForeColor = System.Drawing.Color.Gainsboro;
            this.treeView1.LineColor = System.Drawing.Color.WhiteSmoke;
            this.treeView1.Name = "treeView1";
            this.treeView1.ShowPlusMinus = false;
            this.treeView1.ShowRootLines = false;
            this.treeView1.TabStop = false;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // apiInfoPanel1
            // 
            this.apiInfoPanel1.BackColor = System.Drawing.Color.Black;
            resources.ApplyResources(this.apiInfoPanel1, "apiInfoPanel1");
            this.apiInfoPanel1.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.apiInfoPanel1.InfosToShow = new LX29_Twitch.Api.ApiInfo[0];
            this.apiInfoPanel1.Name = "apiInfoPanel1";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // textBox1
            // 
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.textBox1.ForeColor = System.Drawing.Color.Gainsboro;
            this.textBox1.Name = "textBox1";
            this.textBox1.TabStop = false;
            this.textBox1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyUp);
            // 
            // btn_AddChannel
            // 
            resources.ApplyResources(this.btn_AddChannel, "btn_AddChannel");
            this.btn_AddChannel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.btn_AddChannel.ForeColor = System.Drawing.Color.Gainsboro;
            this.btn_AddChannel.Name = "btn_AddChannel";
            this.btn_AddChannel.TabStop = false;
            this.btn_AddChannel.UseVisualStyleBackColor = false;
            this.btn_AddChannel.Click += new System.EventHandler(this.btn_AddChannel_Click);
            // 
            // btn_removeChannel
            // 
            resources.ApplyResources(this.btn_removeChannel, "btn_removeChannel");
            this.btn_removeChannel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.btn_removeChannel.ForeColor = System.Drawing.Color.Gainsboro;
            this.btn_removeChannel.Name = "btn_removeChannel";
            this.btn_removeChannel.TabStop = false;
            this.btn_removeChannel.UseVisualStyleBackColor = false;
            this.btn_removeChannel.Click += new System.EventHandler(this.btn_removeChannel_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackgroundImage = global::LX29_LixChat.Properties.Resources.BLACK;
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1,
            this.tSDDB_About_Menue,
            this.toolStripSeparator1,
            this.tsLbl_Infotext,
            this.tSProgBar_Loading,
            this.tsLabel_Info});
            this.toolStrip1.Name = "toolStrip1";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tSMi_Accounts,
            this.toolStripMenuItem1,
            this.tSMi_Exit});
            resources.ApplyResources(this.toolStripDropDownButton1, "toolStripDropDownButton1");
            this.toolStripDropDownButton1.ForeColor = System.Drawing.Color.Gainsboro;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            // 
            // tSMi_Accounts
            // 
            this.tSMi_Accounts.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.tSMi_Accounts.ForeColor = System.Drawing.Color.Gainsboro;
            this.tSMi_Accounts.Name = "tSMi_Accounts";
            resources.ApplyResources(this.tSMi_Accounts, "tSMi_Accounts");
            this.tSMi_Accounts.Click += new System.EventHandler(this.tSMi_LogOut_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshChannelsToolStripMenuItem,
            this.tSMi_ReloadEmotes,
            this.tSMi_Sync,
            this.tSMi_ReconnectChat,
            this.tSMi_ShowSettings});
            this.toolStripMenuItem1.ForeColor = System.Drawing.Color.Gainsboro;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            // 
            // refreshChannelsToolStripMenuItem
            // 
            this.refreshChannelsToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.refreshChannelsToolStripMenuItem.ForeColor = System.Drawing.Color.Gainsboro;
            this.refreshChannelsToolStripMenuItem.Name = "refreshChannelsToolStripMenuItem";
            resources.ApplyResources(this.refreshChannelsToolStripMenuItem, "refreshChannelsToolStripMenuItem");
            this.refreshChannelsToolStripMenuItem.Click += new System.EventHandler(this.refreshChannelsToolStripMenuItem_Click);
            // 
            // tSMi_ReloadEmotes
            // 
            this.tSMi_ReloadEmotes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.tSMi_ReloadEmotes.ForeColor = System.Drawing.Color.Gainsboro;
            this.tSMi_ReloadEmotes.Name = "tSMi_ReloadEmotes";
            resources.ApplyResources(this.tSMi_ReloadEmotes, "tSMi_ReloadEmotes");
            this.tSMi_ReloadEmotes.Click += new System.EventHandler(this.tSMi_ReloadEmotes_Click);
            // 
            // tSMi_Sync
            // 
            this.tSMi_Sync.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.tSMi_Sync.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMi_SyncOnlyOnline,
            this.tSMi_SyncAll});
            resources.ApplyResources(this.tSMi_Sync, "tSMi_Sync");
            this.tSMi_Sync.ForeColor = System.Drawing.Color.Gainsboro;
            this.tSMi_Sync.Name = "tSMi_Sync";
            // 
            // tsMi_SyncOnlyOnline
            // 
            this.tsMi_SyncOnlyOnline.BackColor = System.Drawing.Color.Black;
            this.tsMi_SyncOnlyOnline.ForeColor = System.Drawing.Color.White;
            this.tsMi_SyncOnlyOnline.Name = "tsMi_SyncOnlyOnline";
            resources.ApplyResources(this.tsMi_SyncOnlyOnline, "tsMi_SyncOnlyOnline");
            this.tsMi_SyncOnlyOnline.Click += new System.EventHandler(this.tsMi_SyncOnlyOnline_Click);
            // 
            // tSMi_SyncAll
            // 
            this.tSMi_SyncAll.BackColor = System.Drawing.Color.Black;
            this.tSMi_SyncAll.ForeColor = System.Drawing.Color.White;
            this.tSMi_SyncAll.Name = "tSMi_SyncAll";
            resources.ApplyResources(this.tSMi_SyncAll, "tSMi_SyncAll");
            this.tSMi_SyncAll.Click += new System.EventHandler(this.tSMi_SyncAll_Click);
            // 
            // tSMi_ReconnectChat
            // 
            this.tSMi_ReconnectChat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.tSMi_ReconnectChat.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.tSMi_ReconnectChat, "tSMi_ReconnectChat");
            this.tSMi_ReconnectChat.ForeColor = System.Drawing.Color.Gainsboro;
            this.tSMi_ReconnectChat.Name = "tSMi_ReconnectChat";
            this.tSMi_ReconnectChat.Click += new System.EventHandler(this.tSMi_ReconnectChat_Click);
            // 
            // tSMi_ShowSettings
            // 
            this.tSMi_ShowSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.tSMi_ShowSettings.ForeColor = System.Drawing.Color.Gainsboro;
            this.tSMi_ShowSettings.Name = "tSMi_ShowSettings";
            resources.ApplyResources(this.tSMi_ShowSettings, "tSMi_ShowSettings");
            this.tSMi_ShowSettings.Click += new System.EventHandler(this.tSMi_ShowSettings_Click);
            // 
            // tSMi_Exit
            // 
            this.tSMi_Exit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.tSMi_Exit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.tSMi_Exit, "tSMi_Exit");
            this.tSMi_Exit.ForeColor = System.Drawing.Color.Gainsboro;
            this.tSMi_Exit.Name = "tSMi_Exit";
            this.tSMi_Exit.Click += new System.EventHandler(this.tSMi_Exit_Click);
            // 
            // tSDDB_About_Menue
            // 
            this.tSDDB_About_Menue.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tSDDB_About_Menue.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tSMI_UpdateProgramm,
            this.tSMI_About});
            this.tSDDB_About_Menue.ForeColor = System.Drawing.Color.Gainsboro;
            resources.ApplyResources(this.tSDDB_About_Menue, "tSDDB_About_Menue");
            this.tSDDB_About_Menue.Name = "tSDDB_About_Menue";
            // 
            // tSMI_UpdateProgramm
            // 
            this.tSMI_UpdateProgramm.BackColor = System.Drawing.Color.Black;
            this.tSMI_UpdateProgramm.ForeColor = System.Drawing.Color.White;
            this.tSMI_UpdateProgramm.Name = "tSMI_UpdateProgramm";
            resources.ApplyResources(this.tSMI_UpdateProgramm, "tSMI_UpdateProgramm");
            this.tSMI_UpdateProgramm.Click += new System.EventHandler(this.tSMI_UpdateProgramm_Click);
            // 
            // tSMI_About
            // 
            this.tSMI_About.BackColor = System.Drawing.Color.Black;
            this.tSMI_About.ForeColor = System.Drawing.Color.White;
            this.tSMI_About.Name = "tSMI_About";
            resources.ApplyResources(this.tSMI_About, "tSMI_About");
            this.tSMI_About.Click += new System.EventHandler(this.tSMI_About_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // tsLbl_Infotext
            // 
            this.tsLbl_Infotext.ForeColor = System.Drawing.Color.Gainsboro;
            this.tsLbl_Infotext.Name = "tsLbl_Infotext";
            resources.ApplyResources(this.tsLbl_Infotext, "tsLbl_Infotext");
            // 
            // tSProgBar_Loading
            // 
            resources.ApplyResources(this.tSProgBar_Loading, "tSProgBar_Loading");
            this.tSProgBar_Loading.Name = "tSProgBar_Loading";
            this.tSProgBar_Loading.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            // 
            // tsLabel_Info
            // 
            this.tsLabel_Info.Name = "tsLabel_Info";
            resources.ApplyResources(this.tsLabel_Info, "tsLabel_Info");
            // 
            // toolTip_Main
            // 
            this.toolTip_Main.AutoPopDelay = 2000;
            this.toolTip_Main.InitialDelay = 500;
            this.toolTip_Main.ReshowDelay = 100;
            this.toolTip_Main.UseAnimation = false;
            this.toolTip_Main.UseFading = false;
            // 
            // Main
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.Controls.Add(this.btn_removeChannel);
            this.Controls.Add(this.btn_AddChannel);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.splitC_Main);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.White;
            this.HelpButton = true;
            this.Name = "Main";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResizeEnd += new System.EventHandler(this.Main_ResizeEnd);
            this.LocationChanged += new System.EventHandler(this.Main_LocationChanged);
            this.SizeChanged += new System.EventHandler(this.Main_SizeChanged);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Main_MouseClick);
            this.splitContainer_Preview.Panel1.ResumeLayout(false);
            this.splitContainer_Preview.Panel1.PerformLayout();
            this.splitContainer_Preview.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_Preview)).EndInit();
            this.splitContainer_Preview.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.splitC_Main.Panel1.ResumeLayout(false);
            this.splitC_Main.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitC_Main)).EndInit();
            this.splitC_Main.ResumeLayout(false);
            this.cMS_ListBox.ResumeLayout(false);
            this.tLP_Preview.ResumeLayout(false);
            this.pn_Preview.ResumeLayout(false);
            this.splitContainer_ChatPreview.Panel1.ResumeLayout(false);
            this.splitContainer_ChatPreview.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_ChatPreview)).EndInit();
            this.splitContainer_ChatPreview.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private System.Windows.Forms.SplitContainer splitC_Main;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem tSMi_Exit;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripProgressBar tSProgBar_Loading;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel tsLabel_Info;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button btn_AddChannel;
        private System.Windows.Forms.Button btn_removeChannel;
        private System.Windows.Forms.ToolStripLabel tsLbl_Infotext;
        private System.Windows.Forms.TableLayoutPanel tLP_Preview;
        private System.Windows.Forms.Panel pn_Preview;
        private System.Windows.Forms.SplitContainer splitContainer_Preview;
        private System.Windows.Forms.Label lbl_preview;
        private System.Windows.Forms.SplitContainer splitContainer_ChatPreview;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btn_OpenChat;
        private System.Windows.Forms.Button btn_Reconnect;
        private System.Windows.Forms.CheckBox cB_AutoLogin;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Button btn_Disconnect;
        private System.Windows.Forms.Button btn_External;
        private System.Windows.Forms.CheckBox cB_Favorite;
        private System.Windows.Forms.Button btn_ShowStreamInfoPanels;
        private System.Windows.Forms.ToolStripMenuItem tSMi_Accounts;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Button btn_StartStream;
        private System.Windows.Forms.Button btn_Show_Video_Info;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem tSMi_ReloadEmotes;
        private System.Windows.Forms.Button btn_Follow;
        private System.Windows.Forms.ToolStripMenuItem tSMi_Sync;
        private System.Windows.Forms.ToolStripMenuItem tsMi_SyncOnlyOnline;
        private System.Windows.Forms.ToolStripMenuItem tSMi_SyncAll;
        private System.Windows.Forms.ToolStripMenuItem tSMi_ReconnectChat;
        private System.Windows.Forms.ToolTip toolTip_Main;
        private System.Windows.Forms.Button btn_ShowUsers;
        private System.Windows.Forms.ComboBox comBox_StreamQuali;
        private System.Windows.Forms.Button btn_AutoChatActions;
        private System.Windows.Forms.Button btn_openSubpage;
        private System.Windows.Forms.ToolStripMenuItem tSMi_ShowSettings;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cB_LogChat;
        private System.Windows.Forms.ToolStripDropDownButton tSDDB_About_Menue;
        private System.Windows.Forms.ContextMenuStrip cMS_ListBox;
        private System.Windows.Forms.ToolStripMenuItem tSMI_OpenChatInBrowser;
        private System.Windows.Forms.ToolStripMenuItem tSMI_UpdateProgramm;
        private System.Windows.Forms.ToolStripMenuItem tSMI_About;
        private System.Windows.Forms.Button btn_Record;
        private LX29_ChatClient.Forms.ChannelListBox lstB_Channels;
        private LX29_Twitch.Forms.ApiInfoPanel apiInfoPanel1;
        private System.Windows.Forms.ToolStripMenuItem refreshChannelsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openStreamInBrowserPopoutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openChannelInBrowserToolStripMenuItem;
        private Code.FormStream.Player.PlayerControl playerControl1;
    }
}

