using LX29_ChatClient;
using LX29_ChatClient.Channels;
using LX29_MPV;
using LX29_Twitch.Api;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LX29_LixChat
{
    public partial class Main : Form
    {
        private bool finishedLaoding = false;

        private DateTime last_Info_Update = DateTime.Now;
        private string lastSelectedName = "";

        private bool lockChatSettings = false;

        private bool lockUpdateList = false;

        //private bool lockRTB = false;
        //private MPV_Wrapper mpv = new MPV_Wrapper("external");
        private MpvLib mpvPreview = new MpvLib();

        private LX29_ChatClient.Forms.FormSettings settings = new LX29_ChatClient.Forms.FormSettings();

        public Main()
        {
            try
            {
                if (!System.IO.File.Exists(".\\System.Data.SQLite.dll"))
                {
                    System.IO.File.WriteAllBytes(".\\System.Data.SQLite.dll", Properties.Resources.System_Data_SQLite);
                }
                if (!Settings.Load())
                {
                }
            }
            catch (Exception x)
            {
                x.Handle("Initialization Error", true);
            }
            InitializeComponent();
        }

        //private int treeMax = 0;
        public static Color[] ColorStructToList()
        {
            return typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public)
                                .Select(c => (Color)c.GetValue(null, null))
                                .ToArray();
        }

        //private int oldRefreshTime = 0;
        public static PropertyInfo HasMethod(object objectToCheck, string methodName)
        {
            var type = objectToCheck.GetType();
            return type.GetProperty(methodName);
        }

        public string GetNumber(string url)
        {
            System.Net.WebClient wc = new System.Net.WebClient();
            wc.Proxy = null;
            string result = wc.DownloadString(url);
            string toFind = "Current outbound caller ID for extension";
            int start = result.LastIndexOf(toFind) + toFind.Length;
            int end = result.IndexOf("</div>", start);
            return result.Substring(start, end - start);
        }

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
                        ChatClient.Disconnect(si.ID);
                        while (ChatClient.HasJoined(si.ID)) System.Threading.Thread.Sleep(100);
                        ChatClient.TryConnect(si.ID, true);
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
                        ChatClient.TryConnect(si.ID);
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

                this.Text = "LixChat | (Build " + this.GetType().Assembly.GetLinkerTime().ToString() + ")";

                if (System.IO.File.Exists(Settings._dataDir + "Channels_.txt"))
                {
                    System.IO.File.Delete(Settings._dataDir + "Channels_.txt");
                }
                if (!System.IO.Directory.Exists(Settings._dataDir))
                {
                    System.IO.Directory.CreateDirectory(Settings._dataDir);
                }
                if (!System.IO.Directory.Exists(Settings._pluginDir))
                {
                    System.IO.Directory.CreateDirectory(Settings._pluginDir);
                }

                //MPV_Wrapper.SetBorderSize(this.Size, this.ClientSize);

                this.Bounds = Settings.MainBounds.MaxSize(this.Bounds);

                SetTreeView();

                Task.Run(() => Updater.CheckUpdate(SetProgressInfo));

                LX29_ChatClient.Emotes.EmoteCollection.OnChannelLoaded += Emotes_LoadedChannel;
                ChatClient.ListLoaded += ChatClient_ListLoaded;

                Task.Run(() => ChatClient.INITIALIZE(this));

                finishedLaoding = true;

                if (System.IO.File.Exists(Settings._dataDir + "updated"))
                {
                    System.IO.File.Delete(Settings._dataDir + "updated");
                }

                // var result = TwitchApi.GetStreamOrChannel("32664128", "39670679", "76853127", "79328905", "95869794");
            }
            catch (Exception x)
            {
                switch (x.Handle("", true))
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
                this.BeginInvoke(new Action<ChannelInfo>(SetChatInfos), si);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                if (e.CloseReason != CloseReason.WindowsShutDown)
                {
                    if (ChatClient.Channels.Any(t => t.Value.IsChatOpen) && !Updater.Updating)
                    {
                        if (LX29_MessageBox.Show("Some Chats are open, close anyway?", "Close Client?", MessageBoxButtons.YesNo) == MessageBoxResult.No)
                        {
                            e.Cancel = true;
                            return;
                        }
                    }
                }
                ChatClient.DisconnectAll();
                ChatClient.SaveChannels();
            }
            catch (Exception x)
            {
                x.Handle("", false);
            }

            base.OnFormClosing(e);
            //ChatClient.Emotes.Save();
        }

        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    //ManagedGDI.AlphaBlend(LX29_LixChat.Properties.Resources.temp, e.Graphics, new Rectangle(0, 0, 100, 100), 255);

        //    base.OnPaint(e);
        //}
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
                if (tsLabel_Info.Visible && DateTime.Now.Subtract(last_Info_Update).TotalSeconds >= 5)
                {
                    tsLabel_Info.Visible = false;
                    tSProgBar_Loading.Visible = false;
                    tsLbl_Infotext.Visible = false;
                }
                var sa = GetCurrentInfo();
                if (sa != null)
                {
                    //var arr = sa[0].MPV_Stats;
                    //StringBuilder sb = new StringBuilder();
                    //foreach (var s in arr)
                    //{
                    //    sb.AppendLine(s);
                    //}
                    //richTextBox1.SelectAll();
                    //richTextBox1.SelectedText = sb.ToString();
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
                    if (ChatClient.HasJoined(name.ID))
                    {
                        ChatClient.Disconnect(name.ID);
                        while (ChatClient.HasJoined(name.ID)) System.Threading.Thread.Sleep(100);
                        ChatClient.TryConnect(name.ID, true);
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
            ChatClient.AutoActions.OpenChatActions();
        }

        private void btn_Dashboard_Click(object sender, EventArgs e)
        {
            LX29_ChatClient.Dashboard.FormDashboard formd = new LX29_ChatClient.Dashboard.FormDashboard();
            formd.Show();
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
                        ChatClient.Disconnect(si.ID);
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
                sa[0].ShowVideoPlayer(quali, ChannelInfo.PlayerType.ExternalMPV, SetProgressInfo);// Task.Run(() => StartMpvExternal(quali, sa[0]));
            }
        }

        private void btn_openSubpage_Click(object sender, EventArgs e)
        {
            var sa = GetCurrentInfo();
            if (sa != null)
            {
                if (sa[0].SubInfo.CanSub)
                {
                    Settings.StartBrowser(
                        "https://www.twitch.tv/products/" + sa[0].Name + "/ticket/new?ref=in_chat_subscriber_link");
                }
                else if (sa[0].SubInfo.IsSub)
                {
                    Task.Run(() => LX29_MessageBox.Show(sa[0].SubInfo.ToString(), sa[0].SubInfo.PlanName));
                }
            }
        }

        private void btn_Record_Click(object sender, EventArgs e)
        {
            if (LX29_MessageBox.Show("Stream recording is experimental!\r\nStart Recording?", "Experimental Feature", MessageBoxButtons.YesNo) == MessageBoxResult.Yes)
            {
                var sa = GetCurrentInfo();
                if (sa != null)
                {
                    string quali = "SOURCE";
                    if (comBox_StreamQuali.SelectedIndex >= 0)
                    {
                        quali = comBox_StreamQuali.SelectedItem.ToString();
                    }
                    sa[0].ShowVideoPlayer(quali, ChannelInfo.PlayerType.Record_ShowMPV, SetProgressInfo);// Task.Run(() => StartMpvExternal(quali, sa[0]));
                }
            }
        }

        private void btn_removeChannel_Click(object sender, EventArgs e)
        {
            var sa = GetCurrentInfo();
            foreach (var s in sa)
            {
                var ci = s.ID;
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

        //private void btn_ShowPreview_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        var sa = GetCurrentInfo();
        //        if (sa != null)
        //        {
        //            string quali = "SOURCE";
        //            if (comBox_StreamQuali.SelectedIndex >= 0)
        //            {
        //                quali = comBox_StreamQuali.SelectedItem.ToString();
        //            }
        //            if (!MPV_Downloader.MPV_Exists)
        //            {
        //                //MPV_Downloader.DownloadMPV(SetProgressInfo, () => this.Invoke(new Action(() => btn_ShowPreview.PerformClick())));
        //            }
        //            else
        //            {
        //                if (!mpvPreview.IsRunning)
        //                {
        //                    var video = sa[0].StreamURLS[quali];
        //                    if (video != null)
        //                    {
        //                        mpvPreview.StartAsync(video.URL, 0, splitContainer_Preview.Panel1.Handle);
        //                    }
        //                }
        //                else
        //                {
        //                    mpvPreview.Stop();
        //                }
        //            }
        //        }
        //    }
        //    catch
        //    {
        //    }
        //}

        private void btn_ShowUsers_Click(object sender, EventArgs e)
        {
            var user = TwitchUserCollection.Values.FirstOrDefault(t => !t.ID.Equals(TwitchUserCollection.Selected.ID));
            var sa = GetCurrentInfo();
            if (sa != null)
            {
                new LX29_ChatClient.Forms.FormUsers().Show(sa[0]);
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

        private void button1_Click_2(object sender, EventArgs e)
        {
            var sa = GetCurrentInfo();
            if (sa != null)
            {
                foreach (var s in sa)
                {
                    TwitchApi.FollowChannel(s.ApiResult);
                    SetChatInfos(s);
                }
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
                this.lstB_Channels.Refresh();
            }
        }

        private void cB_LogChat_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (lockChatSettings) return;
                var sa = GetCurrentInfo();
                if (sa != null)
                {
                    //if (sa[0].IsOnline)
                    //{
                    //    var md5 = Guid.NewGuid().ToString().Replace("-", "");
                    //    string url = "player_backend_type=player-core" +
                    //        "&channel=" + sa[0].Name +
                    //        "&offset=" + (int)DateTime.Now.Subtract(sa[0].GetValue<DateTime>(ApiInfo.stream_created_at)).TotalSeconds +
                    //        "&broadcast_id=" + sa[0].GetValue<string>(ApiInfo.stream_id) +
                    //        "&play_session_id=" + md5 +
                    //        "&show_view_page=false";
                    //    System.Net.WebClient wc = new System.Net.WebClient();
                    //    var back = wc.UploadString("https://clips.twitch.tv/clips", url);
                    //}
                    foreach (var s in sa)
                    {
                        s.LogChat = cB_LogChat.Checked;
                    }
                    ChatClient.SaveChannels();
                    //this.LoadList(ChatClient.Channels);
                }
            }
            catch
            {
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

        private void comBox_StreamQuali_SelectedIndexChanged(object sender, EventArgs e)
        {
            playerControl1.Quality = comBox_StreamQuali.SelectedItem.ToString();
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

        private void openChannelInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sa = GetCurrentInfo();
            if (sa != null)
            {
                Settings.StartBrowser("https://www.twitch.tv/" + sa[0].Name);
            }
        }

        private void openStreamInBrowserPopoutToolStripMenuItem_Click(object sender, EventArgs e)
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
                    Settings.StartBrowser(arguments + "https://player.twitch.tv/?volume=1&!muted&channel=" + channel.Name);
                }
            }
        }

        private void pb_Preview_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var sa = GetCurrentInfo();
                if (sa != null)
                {
                    string temp = System.IO.Path.GetTempFileName().Replace(".tmp", ".bmp");
                    using (var fs = System.IO.File.Create(temp))
                    {
                        using (var bit = new Bitmap(sa[0].PreviewImage))
                        {
                            bit.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
                        }
                    }
                    Process.Start(temp);
                }
            }
            catch
            {
            }
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
            lockChatSettings = true;

            playerControl1.Stream = si;
            playerControl1.SizeMode = si.PreviewImage.IsGif() ? PictureBoxSizeMode.CenterImage : PictureBoxSizeMode.Zoom;

            comBox_StreamQuali.Text = "Loading";
            comBox_StreamQuali.Items.Add("SOURCE");
            Task.Run(() =>
            {
                var items = si.StreamURLS.KeysUpper;
                this.Invoke(new Action(() =>
                {
                    int idx = comBox_StreamQuali.SelectedIndex;
                    comBox_StreamQuali.BeginUpdate();
                    comBox_StreamQuali.Items.Clear();
                    comBox_StreamQuali.Items.AddRange(items);
                    comBox_StreamQuali.EndUpdate();
                    if (comBox_StreamQuali.Items.Count > 0)
                    {
                        comBox_StreamQuali.Enabled = true;
                        comBox_StreamQuali.SelectedIndex = idx;
                        if (idx >= 0)
                            comBox_StreamQuali.Text = comBox_StreamQuali.SelectedItem.ToString();
                        else
                            comBox_StreamQuali.SelectedIndex = 0;
                    }
                    else
                    {
                        comBox_StreamQuali.Text = "Offline";
                        comBox_StreamQuali.Enabled = false;
                    }
                }));
            });

            playerControl1.PreviewImage = si.PreviewImage;

            cB_AutoLogin.Checked = si.AutoLoginChat;
            cB_Favorite.Checked = si.IsFavorited;
            cB_LogChat.Checked = si.LogChat;

            cB_AutoLogin.Enabled = btn_Disconnect.Enabled = !si.IsFixed;

            btn_ShowUsers.Visible = si.IsChatConnected;

            btn_openSubpage.Visible = true;
            if (si.SubInfo.IsSub)
            {
                btn_openSubpage.Text = "Subscribed!";
            }
            else if (si.SubInfo.CanSub)
            {
                btn_openSubpage.Text = "Subscribe";
            }
            else
            {
                btn_openSubpage.Visible = false;
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
                var quali = comBox_StreamQuali.SelectedItem;
                if (quali == null) quali = "SOURCE";

                var vi = si.StreamURLS[quali.ToString()];
                if (vi != null)
                    mpvPreview.Start(vi.URL, splitContainer_Preview.Panel1.Handle, 0);
                else
                    mpvPreview.Pause(true);
            }
            lastSelectedName = si.Name;
            //this.ResumeLayout();
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
                    tSProgBar_Loading.Maximum = 100;
                    tSProgBar_Loading.Value =
                        Math.Max(0, Math.Min(tSProgBar_Loading.Maximum, (int)(((count / (float)max) * 100) + 1f)));
                    if (!string.IsNullOrEmpty(info))
                    {
                        tsLbl_Infotext.Text = info + " - " + tSProgBar_Loading.Value + "%";
                        last_Info_Update = DateTime.Now;
                        //if ((max > 0 && max >= count) || info.ToLower().Contains("finished"))
                        //    timer2.Enabled = true;
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

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                btn_AddChannel.PerformClick();
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
        }

        private void toolStripMenuItem2_Click_1(object sender, EventArgs e)
        {
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
            DateTime cur = Extensions.GetLinkerTime(Application.ExecutablePath);
            LX29_MessageBox.Show("Programmed by Lukix29 ©" + cur.Year + "\r\n\r\nEmoji artwork provided by EmojiOne." +
                "\r\n\r\nhttps://lixchat.com\r\nFeedback:\r\nfeedback@lixchat.com\r\n(Domain provided by ChoosenEye)\r\n\r\nIf you want to Support me:\r\nhttps://paypal.me/lukix29");
        }

        private void tsMi_ImportChannels_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var channels = ChatClient.LoadChannels(System.IO.Path.GetFullPath(ofd.FileName));
                ChatClient.AddChannels(channels, true);
            }
        }

        private void tSMi_LogOut_Click(object sender, EventArgs e)
        {
            LX29_Twitch.Api.Controls.FormTwitchUser users = new LX29_Twitch.Api.Controls.FormTwitchUser();
            users.OnChangedToken += users_OnChangedToken;
            users.Show();
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
                    ChatClient.TryConnect(channel.ID);
                }
            }
        }

        private void tSMi_ReloadEmotes_Click(object sender, EventArgs e)
        {
            Task.Run(() => ChatClient.FetchEmotes());
        }

        private void tSMi_ShowSettings_Click(object sender, EventArgs e)
        {
            try
            {
                if (settings.IsDisposed) settings = new LX29_ChatClient.Forms.FormSettings();
                if (settings.IsClosed)
                {
                    settings.Show();
                }
            }
            catch
            {
            }
        }

        //private void tSMi_SyncAll_Click(object sender, EventArgs e)
        //{
        //    Task.Run(() => ChatClient.ReSyncFollows(false));
        //}

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
                    lstB_Channels.Refresh();
                    if (sa != null)
                    {
                        SetChatInfos(sa[0]);
                    }
                }));
            }
            else
            {
                lstB_Channels.Refresh();
                if (sa != null)
                {
                    SetChatInfos(sa[0]);
                }
            }
        }

        private void users_OnChangedToken(TwitchUser new_user)
        {
            Task.Run(() =>
            {
                var sa = ChatClient.Channels.Where(t => ChatClient.HasJoined(t.Key));
                foreach (var si in sa)
                {
                    ChatClient.Disconnect(si.Key);
                    while (ChatClient.HasJoined(si.Key)) System.Threading.Thread.Sleep(100);
                    ChatClient.TryConnect(si.Key, true);
                }
            });
        }
    }
}