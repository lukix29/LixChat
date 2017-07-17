﻿using LX29_ChatClient;
using LX29_ChatClient.Channels;
using LX29_Helpers;
using LX29_Twitch.Api;
using LX29_TwitchChat.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LX29_TwitchChat
{
    public partial class Main : Form
    {
        private bool finishedLaoding = false;
        private string lastSelectedName = "";
        private bool lockChatSettings = false;
        //private bool lockRTB = false;

        private bool lockUpdateList = false;

        //private MPV_Wrapper mpv = new MPV_Wrapper("external");
        private MPV_Wrapper mpvPreview = new MPV_Wrapper("preview");

        //private int oldRefreshTime = 0;

        //private int treeMax = 0;

        public Main()
        {
            if (!Settings.Load())
            {
            }
            InitializeComponent();
            //lstB_Channels.DrawItem += listBox1_DrawItem;
            //lstB_Channels.MeasureItem += listBox1_MeasureItem;
        }

        public static Color[] ColorStructToList()
        {
            return typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public)
                                .Select(c => (Color)c.GetValue(null, null))
                                .ToArray();
        }

        public static PropertyInfo HasMethod(object objectToCheck, string methodName)
        {
            var type = objectToCheck.GetType();
            return type.GetProperty(methodName);
        }

        //MPV doesnt start!!!
        //public int dfs() { }

        protected void btn_OpenChat_Click(object sender, EventArgs e)
        {
            try
            {
                var sa = GetCurrentInfo();
                if (sa != null)
                {
                    foreach (var si in sa)
                    {
                        // cB_ChatPreview.Checked = false;
                        si.ShowChat();
                    }
                }
            }
            catch { }
        }

        protected void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var sa = GetCurrentInfo();
                if (sa != null)
                {
                    foreach (var si in sa)
                    {
                        ChatClient.Disconnect(si.Name);
                        while (ChatClient.HasJoined(si.Name)) ;
                        ChatClient.TryConnect(si.Name, true);
                    }
                    //this.LoadList(ChatClient.Channels);
                }
            }
            catch { }
        }

        protected void cB_AutoLogin_CheckedChanged(object sender, EventArgs e)
        {
            if (lockChatSettings) return;
            var sa = GetCurrentInfo();
            if (sa != null)
            {
                foreach (var si in sa)
                {
                    if (si.IsFixed)
                    {
                        continue;
                    }
                    si.AutoLoginChat = cB_AutoLogin.Checked;
                    if (si.AutoLoginChat)
                    {
                        ChatClient.TryConnect(si.Name);
                    }
                    ChatClient.SaveChannels();
                }
                //this.LoadList(ChatClient.Channels);
            }
        }

        protected void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                Bitmap bg = new Bitmap(1, 1);
                bg.SetPixel(0, 0, Color.FromArgb(40, 40, 40));
                toolStrip1.BackgroundImage = bg;

                //MessageBox.Show(DateTime.Now.Subtract(new DateTime(2000, 1, 1)).TotalSeconds.ToString("F0"));
                this.Text += " | (Build " + this.GetType().Assembly.GetLinkerTime().ToString() + ")";

                if (!System.IO.Directory.Exists(Settings.dataDir))
                {
                    System.IO.Directory.CreateDirectory(Settings.dataDir);
                }
                if (!System.IO.Directory.Exists(Settings.pluginDir))
                {
                    System.IO.Directory.CreateDirectory(Settings.pluginDir);
                }
                if (System.IO.File.Exists(MPV_Wrapper.MPVinputConfig))
                {
                    System.IO.File.WriteAllText(System.IO.Path.GetFullPath(Settings.pluginDir + "MPV\\portable_config\\input.conf"),
                   Resources.mpv_input_config);
                }
                MPV_Wrapper.SetBorderSize(this.Size, this.ClientSize);

                this.Bounds = Settings.MainBounds.MaxSize(this.Bounds);

                comBox_StreamQuali.SelectedIndex = 0;

                SetTreeView();

                ChatClient.ListLoaded += ChatClient_ListLoaded;
                LX29_ChatClient.Emotes.EmoteCollection.OnChannelLoaded += Emotes_LoadedChannel;

                ChatClient.INITIALIZE(this);

                finishedLaoding = true;
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        Application.Restart();
                        return;

                    case MessageBoxResult.Abort:
                        Application.Exit();
                        return;
                }
            }
        }

        protected void lstB_Channels_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            btn_OpenChat.PerformClick();
        }

        protected void lstB_Channels_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lockChatSettings) return;
            var sa = GetCurrentInfo();
            if (sa != null)
            {
                var si = sa[0];
                this.Invoke(new Action<ChannelInfo>(SetChatInfos), si);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (ChatClient.Channels.Any(t => t.Value.IsChatOpen) && !Updater.Updating)
            {
                if (LX29_MessageBox.Show("Some Chats are open, close anyway?", "Close Client?", MessageBoxButtons.YesNo) == MessageBoxResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            ChatClient.DisconnectAll();
            ChatClient.SaveChannels();

            base.OnFormClosing(e);
            //ChatClient.Emotes.Save();
        }

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {
            if (keyData != Keys.Tab)
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }
            return true;
        }

        protected void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                var sa = GetCurrentInfo();
                if (sa != null)
                {
                    var si = sa[0];
                    SetChatInfoBox(si);
                }
            }
            catch { }
        }

        protected void tSMi_Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        protected void tSMi_ReconnectChat_Click(object sender, EventArgs e)
        {
            Task.Run(new Action(delegate()
            {
                foreach (var name in ChatClient.Channels.Values)
                {
                    if (ChatClient.HasJoined(name.Name))
                    {
                        ChatClient.Disconnect(name.Name);
                        while (ChatClient.HasJoined(name.Name)) ;
                        ChatClient.TryConnect(name.Name, true);
                    }
                }
                UpdateList();
            }));
        }

        private void btn_AddChannel_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text.LastLine("/");
            textBox1.Clear();
            Task.Run(new Action(delegate()
                {
                    var error = ChatClient.AddChannel(name);
                    if (error != AddError.None)
                    {
                        LX29_MessageBox.Show("User " + Enum.GetName(typeof(AddError), error) + "!");
                    }
                }));
        }

        private void btn_AutoChatActions_Click(object sender, EventArgs e)
        {
            var sa = GetCurrentInfo();
            if (sa != null)
            {
                ChatClient.AutoActions.OpenChatActions(sa[0]);
            }
        }

        private void btn_Disconnect_Click(object sender, EventArgs e)
        {
            if (lockChatSettings) return;
            var sa = GetCurrentInfo();
            if (sa != null)
            {
                foreach (var si in sa)
                {
                    if (!si.IsFixed)
                    {
                        ChatClient.Disconnect(si.Name);
                    }
                }
                //this.LoadList(ChatClient.Channels);
            }
        }

        private void btn_External_Click(object sender, EventArgs e)
        {
            var sa = GetCurrentInfo();
            if (sa != null)
            {
                string quali = "SOURCE";
                if (comBox_StreamQuali.SelectedIndex >= 0)
                {
                    quali = comBox_StreamQuali.SelectedItem.ToString();
                }
                sa[0].ShowVideoPlayer(quali, true, SetProgressInfo);// Task.Run(() => StartMpvExternal(quali, sa[0]));
            }
        }

        private void btn_OpenBrowser_Click(object sender, EventArgs e)
        {
            var sa = GetCurrentInfo();
            if (sa != null)
            {
                Settings.StartBrowser("https://www.twitch.tv/" + sa[0].Name);
            }
        }

        private void btn_openSubpage_Click(object sender, EventArgs e)
        {
            var sa = GetCurrentInfo();
            if (sa != null)
            {
                if (!sa[0].SubInfo.IsEmpty && !sa[0].SubInfo.IsSub)
                {
                    //TwitchApi.GetSubscription(ChatClient.TwitchUsers.Selected.ID, sa[0].ID, ChatClient.TwitchUsers.Selected.Token);
                    Settings.StartBrowser(
                        "https://www.twitch.tv/products/" + sa[0].Name + "/ticket/new?ref=in_chat_subscriber_link");
                }
                else
                {
                    LX29_MessageBox.Show(sa[0].SubInfo.ToString(), sa[0].SubInfo.PlanName);
                }
            }
        }

        private void btn_removeChannel_Click(object sender, EventArgs e)
        {
            var sa = GetCurrentInfo();
            foreach (var s in sa)
            {
                var ci = s.Name;
                ChatClient.RemoveChannel(ci);
            }
            ChatClient.SaveChannels();
            this.UpdateList();
        }

        private void btn_Show_Video_Info_Click(object sender, EventArgs e)
        {
            var sa = GetCurrentInfo();
            if (sa != null)
            {
                Task.Run(() => LX29_MessageBox.Show(sa[0].StreamURLS.ToString()));
            }
        }

        private void btn_ShowPreview_Click(object sender, EventArgs e)
        {
            var sa = GetCurrentInfo();
            if (sa != null)
            {
                string quali = "SOURCE";
                if (comBox_StreamQuali.SelectedIndex >= 0)
                {
                    quali = comBox_StreamQuali.SelectedItem.ToString();
                }
                if (!MPV_Downloader.MPV_Exists)
                {
                    MPV_Downloader.DownloadMPV(SetProgressInfo, () => this.Invoke(new Action(() => btn_ShowPreview.PerformClick())));
                }
                else
                {
                    if (!mpvPreview.IsRunning)
                    {
                        var video = sa[0].StreamURLS[quali];
                        if (video != null)
                        {
                            mpvPreview.StartAsync(video.URL, 0, splitContainer_Preview.Panel1.Handle);
                        }
                    }
                    else
                    {
                        mpvPreview.Stop();
                    }
                }
            }
        }

        private void btn_ShowUsers_Click(object sender, EventArgs e)
        {
            var user = ChatClient.TwitchUsers.Values.FirstOrDefault(t => !t.ID.Equals(ChatClient.TwitchUsers.Selected.ID));
            var sa = GetCurrentInfo();
            if (sa != null)
            {
                new LX29_ChatClient.Forms.FormUsers().Show(sa[0]);
                //}
                //IRC_Client.IRC irc = new IRC_Client.IRC("irc.twitch.tv", sa[0].Name, user.Name, user.Token);
                //irc.ConnectAsync();
                //irc.ConnectionComplete += (s, o) =>
                //    {
                //        ChatClient.SendMessage("Test", user.Name, irc);
                //    };
            }
        }

        private void btn_StartStream_Click(object sender, EventArgs e)
        {
            var sa = GetCurrentInfo();
            if (sa != null)
            {
                string quali = "SOURCE";
                if (comBox_StreamQuali.SelectedIndex >= 0)
                {
                    quali = comBox_StreamQuali.SelectedItem.ToString();
                }
                sa[0].ShowVideoPlayer(quali);
            }
        }

        private void btn_Test_Click(object sender, EventArgs e)
        {
            //var sa = GetCurrentInfo();
            //ChatClient.SendMessage("/mods", sa[0].Name);
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            var sa = GetCurrentInfo();
            if (sa != null)
            {
                TwitchApi.FollowChannel(sa[0].ApiResult);
                SetChatInfos(sa[0]);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var sa = GetCurrentInfo();
            if (sa != null)
            {
                LX29_Twitch.Forms.FormStreamPanels panels = new LX29_Twitch.Forms.FormStreamPanels();
                panels.Show(sa[0]);
            }
        }

        //private void cB_ChatPreview_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (cB_ChatPreview.Checked)
        //    {
        //        chatPreview.Visible = true;
        //        chatPreview.BringToFront();
        //        this.BeginInvoke(new Action(() =>
        //        {
        //            chatPreview.ChatView.SetChannel(GetCurrentInfo()[0], MsgType.All_Messages);
        //            this.UpdateList();
        //        }));
        //    }
        //    else
        //    {
        //        chatPreview.ChatView.Stop();
        //        chatPreview.Visible = false;
        //        apiInfoPanel1.BringToFront();
        //    }
        //}

        private void cB_Favorite_CheckedChanged(object sender, EventArgs e)
        {
            if (lockChatSettings) return;
            var sa = GetCurrentInfo();
            if (sa != null)
            {
                foreach (var s in sa)
                {
                    s.IsFavorited = cB_Favorite.Checked;
                }
                this.lstB_Channels.Invalidate();
            }
        }

        private void cB_LogChat_CheckedChanged(object sender, EventArgs e)
        {
            if (lockChatSettings) return;
            var sa = GetCurrentInfo();
            if (sa != null)
            {
                foreach (var s in sa)
                {
                    s.LogChat = cB_LogChat.Checked;
                }
                ChatClient.SaveChannels();
                //this.LoadList(ChatClient.Channels);
            }
        }

        private void ChatClient_ListLoaded(int count, int max, string info)
        {
            try
            {
                if (lockUpdateList) return;
                lockUpdateList = true;
                Task.Run(() =>
                    {
                        UpdateList();

                        if (!string.IsNullOrEmpty(info)) SetProgressInfo(count, max, info);
                        lockUpdateList = false;
                    });
            }
            catch { }
        }

        private void cMS_ListBox_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        private void ColorControls(Control c, Color color, Color color1)
        {
            c.BackColor = color;
            if (c.HasChildren)
            {
                foreach (Control ci in c.Controls)
                {
                    ColorControls(ci, color, color1);
                }
            }
        }

        //private void ChatClient_ListUpdated()
        //{
        //    if (lockUpdateList) return;
        //    lockUpdateList = true;
        //    UpdateList();// ChatClient_ListLoaded(sa, 0, 0, string.Empty);
        //    lockUpdateList = false;
        //}
        private void comboBox1_MouseClick(object sender, MouseEventArgs e)
        {
            var sa = GetCurrentInfo();
            if (sa != null)
            {
                comBox_StreamQuali.BeginUpdate();
                comBox_StreamQuali.Items.Clear();
                comBox_StreamQuali.Items.AddRange(sa[0].StreamURLS.KeysUpper);
                comBox_StreamQuali.EndUpdate();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void Emotes_LoadedChannel(ChannelInfo ci, int count, int max, string info)
        {
            SetProgressInfo(count, max, info);
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private ChannelInfo[] GetCurrentInfo()
        {
            try
            {
                return (ChannelInfo[])this.Invoke(new Func<ChannelInfo[]>(() =>
                 {
                     return lstB_Channels.SelectedItems;
                 }));
            }
            catch { }
            return null;
        }

        private void lstB_Channels_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                    lstB_Channels.SelectedIndex++;
                    break;

                case Keys.Up:
                    lstB_Channels.SelectedIndex--;
                    break;
            }
        }

        private void lstB_Channels_Load(object sender, EventArgs e)
        {
        }

        private void Main_LocationChanged(object sender, EventArgs e)
        {
            if (finishedLaoding)
                Settings.MainLocation = this.Location;
        }

        private void Main_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void Main_ResizeEnd(object sender, EventArgs e)
        {
        }

        private void Main_SizeChanged(object sender, EventArgs e)
        {
            if (finishedLaoding)
                Settings.MainSize = this.Size;
        }

        private void pb_Preview_DoubleClick(object sender, EventArgs e)
        {
            btn_StartStream.PerformClick();
        }

        private void refreshChannelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task.Run(() => ChatClient.UpdateChannels());
        }

        private void SetChatInfoBox(ChannelInfo si)
        {
            foreach (TreeNode s in treeView1.Nodes)
            {
                string name = (s.Name.Equals("All") || s.Name.Equals("HL") ? s.Name + "_Messages" : s.Name);
                MsgType msgt = (MsgType)Enum.Parse(typeof(MsgType), name);
                //var node = treeView1.Nodes.Find(s, true);
                //if (msgt != MsgType.Whisper)
                {
                    s.Text = s.Name + ": " //+ new string(' ', treeMax - s.Name.Length)
                        + ChatClient.Messages.Count(si.Name, msgt);
                }
            }
        }

        private void SetChatInfos(ChannelInfo si)
        {
            if (lockChatSettings) return;
            this.SuspendLayout();
            lockChatSettings = true;

            pb_Preview.SizeMode = si.PreviewImage.IsGif() ? PictureBoxSizeMode.CenterImage : PictureBoxSizeMode.Zoom;

            pb_Preview.Image = si.PreviewImage;
            cB_AutoLogin.Checked = si.AutoLoginChat;
            cB_Favorite.Checked = si.IsFavorited;
            cB_LogChat.Checked = si.LogChat;
            btn_openSubpage.Visible = !si.SubInfo.IsEmpty;

            cB_AutoLogin.Enabled = btn_Disconnect.Enabled = !si.IsFixed;

            if (si.SubInfo.IsSub)
            {
                btn_openSubpage.Text = "Subscribed!";
            }
            else
            {
                btn_openSubpage.Text = "Subscribe";
            }
            apiInfoPanel1.SetChatInfos(si);
            SetChatInfoBox(si);
            //oldRefreshTime = 0;
            lockChatSettings = false;

            lbl_preview.Text = (si.IsOnline ? "Preview:" : "Offline");

            btn_Reconnect.Text = (si.IsChatConnected ? "Reconnect Chat" : "Connect Chat");

            btn_Follow.Text = (si.Followed) ? "Un-Follow" : "Follow";

            // if (cB_ChatPreview.Checked) chatPreview.ChatView.SetChannel(si, MsgType.All_Messages);

            if (mpvPreview.IsRunning && !si.Name.Equals(lastSelectedName, StringComparison.OrdinalIgnoreCase))
            {
                mpvPreview.Start(si.StreamURLS[comBox_StreamQuali.SelectedItem.ToString()].URL, 0, splitContainer_Preview.Panel1.Handle);
            }
            lastSelectedName = si.Name;
            this.ResumeLayout();
        }

        private void SetProgressInfo(int count, int max, string info)
        {
            try
            {
                this.Invoke(new Action(delegate()
                    {
                        tsLbl_Infotext.Visible = true;
                        tsLabel_Info.Visible = true;
                        tSProgBar_Loading.Visible = true;
                        tSProgBar_Loading.Maximum = max;
                        tSProgBar_Loading.Value = Math.Min(max, count);
                        if (!string.IsNullOrEmpty(info))
                        {
                            tsLbl_Infotext.Text = info;
                            if ((max > 0 && max == count) || info.ToLower().Contains("finished"))
                                timer2.Enabled = true;
                        }
                    }));
            }
            catch { }
        }

        private void SetTreeView()
        {
            List<TreeNode> list = new List<TreeNode>();
            var sa = new MsgType[] { MsgType.All_Messages, MsgType.HL_Messages, MsgType.Whisper, MsgType.Outgoing };
            //treeMax = sa.Max(t => Enum.GetName(typeof(MsgType), t).Replace("_Messages", "").Length);
            foreach (var si in sa)
            {
                var s = Enum.GetName(typeof(MsgType), si).Replace("_Messages", "");
                var n = new TreeNode(s + ": ");// + new string(' ', treeMax - s.Length));
                n.Name = s;
                list.Add(n);
            }
            //TreeNode node = new TreeNode("Messages", list.ToArray());
            treeView1.Nodes.AddRange(list.ToArray());
        }

        private void splitContainer2_Panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        //private void StartMpvExternal(string quali, ChannelInfo sa)
        //{
        //    MPV_StartTimer mst = new MPV_StartTimer();
        //    mst.Start(quali, sa, SetProgressInfo);
        //}

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                btn_AddChannel.PerformClick();
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            tsLabel_Info.Visible = false;
            tSProgBar_Loading.Visible = false;
            timer2.Enabled = false;
            tsLbl_Infotext.Visible = false;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Task.Run(() => ChatClient.FetchEmotes());
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //MsgType type = MsgType.All_Messages;
            //Enum.TryParse<MsgType>(treeView1.SelectedNode.Name, out type);
            //if (cB_ChatPreview.Checked)
            //{
            //    this.BeginInvoke(new Action(() => chatPreview.ChatView.SetAllMessages(type)));
            //}
        }

        private void tSMI_About_Click(object sender, EventArgs e)
        {
            LX29_MessageBox.Show("Programmed by Lukix29 " + DateTime.Now.Year);
        }

        private void tSMi_CacheEmotes_Click(object sender, EventArgs e)
        {
            Task.Run(() => ChatClient.Emotes.DownloadAllEmotes());
        }

        private void tSMi_LogOut_Click(object sender, EventArgs e)
        {
            LX29_Twitch.Api.Controls.FormTwitchUser users = new LX29_Twitch.Api.Controls.FormTwitchUser();
            users.OnChangedToken += users_OnChangedToken;
            users.Show(ChatClient.TwitchUsers);
            //File.Delete(ChatClient.dataDir + "auth.txt");
            //Application.Restart();
        }

        private void tSMI_OpenChatInBrowser_Click(object sender, EventArgs e)
        {
            var sa = GetCurrentInfo();
            if (sa != null)
            {
                foreach (var channel in sa)
                {
                    string arguments = "";
                    switch (Settings.BrowserName)
                    {
                        case "chrome":
                            arguments = "--new-window --app=";
                            break;

                        case "chromium":
                            arguments = "--new-window --app=";
                            break;

                        case "firefox":
                            arguments = "-new-window ";
                            break;
                    }
                    Settings.StartBrowser(arguments + "https://www.twitch.tv/" + channel.Name + "/chat");
                    ChatClient.TryConnect(channel.Name);
                }
            }
        }

        private void tSMi_ReloadEmotes_Click(object sender, EventArgs e)
        {
        }

        private void tSMi_ShowSettings_Click(object sender, EventArgs e)
        {
            controlSettings1.BringToFront();
            //controlSettings1.Location = new Point(0, toolStrip1.Bottom);
            //controlSettings1.Size = new Size(this.ClientSize.Width, this.ClientSize.Height - toolStrip1.Height);
            controlSettings1.Dock = DockStyle.Fill;
        }

        private void tSMi_SyncAll_Click(object sender, EventArgs e)
        {
            Task.Run(() => ChatClient.ReSyncFollows(false));
        }

        private void tsMi_SyncOnlyOnline_Click(object sender, EventArgs e)
        {
            Task.Run(() => ChatClient.ReSyncFollows(true));
        }

        private void tSMI_UpdateProgramm_Click(object sender, EventArgs e)
        {
            Task.Run(() => Updater.CheckUpdate(SetProgressInfo));
        }

        private void UpdateList()
        {
            var sa = GetCurrentInfo();
            if (lstB_Channels.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                    {
                        lstB_Channels.Invalidate();
                        if (sa != null)
                        {
                            SetChatInfos(sa[0]);
                        }
                    }));
            }
            else
            {
                lstB_Channels.Invalidate();
                if (sa != null)
                {
                    SetChatInfos(sa[0]);
                }
            }
        }

        private void users_OnChangedToken(TwitchUser new_user)
        {
            var sa = ChatClient.Channels.Where(t => ChatClient.HasJoined(t.Key));
            foreach (var si in sa)
            {
                ChatClient.Disconnect(si.Key);
                while (ChatClient.HasJoined(si.Key)) ;
                ChatClient.TryConnect(si.Key, true);
            }
        }

        //public class MPV_StartTimer
        //{
        //    public async void Start(string quali, ChannelInfo sa, Action<int, int, string> a)
        //    {
        //        int cnt = 0;
        //        while (true)
        //        {
        //            if (StartMpvExternal(quali, sa, a))
        //            {
        //                break;
        //            }
        //            else
        //            {
        //                cnt++;
        //                if (cnt >= 3)
        //                {
        //                    var res = LX29_MessageBox.Show("Stream is Offline", "", MessageBoxButtons.RetryCancel, MessageBoxIcon.Asterisk);
        //                    if (res == MessageBoxResult.Cancel)
        //                    {
        //                        break;
        //                    }
        //                    cnt = 0;
        //                }
        //                await Task.Delay(10000);
        //            }
        //        }
        //    }

        //    private bool StartMpvExternal(string quali, ChannelInfo sa, Action<int, int, string> a)
        //    {
        //        try
        //        {
        //            if (!MPV_Downloader.MPV_Exists)
        //            {
        //                MPV_Downloader.DownloadMPV(a, () => StartMpvExternal(quali, sa, a));
        //            }

        //            var sdf = sa.StreamURLS;
        //            if (!sdf.IsEmpty)
        //            {
        //                string url = sdf[quali].URL;
        //                MPV_Wrapper.StartAlone(sa.DisplayName, url, sa.PlayerPosition);
        //                return true;
        //            }
        //            else
        //            {
        //                return false;
        //            }
        //        }
        //        catch (Exception x)
        //        {
        //            switch (x.Handle())
        //            {
        //                case MessageBoxResult.Retry:
        //                    return StartMpvExternal(quali, sa, a);
        //            }
        //        }
        //        return false;
        //    }
        //}
    }
}