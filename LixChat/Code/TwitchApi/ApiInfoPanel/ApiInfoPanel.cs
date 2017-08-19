using LX29_ChatClient;
using LX29_ChatClient.Channels;
using LX29_Twitch.Api;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace LX29_Twitch.Forms
{
    public partial class ApiInfoPanel : UserControl
    {
        private ApiResult channel = null;

        //private bool lockRTB = false;

        private int oldRefreshTime = 0;

        public ApiInfoPanel()
        {
            InitializeComponent();
            InfosToShow = new ApiInfo[0];
            rTB_Infos.AutoScrollOffset = new Point(0, 0);
            rTB_Infos.AddContextMenu();
            // Set maximum position of your panel beyond the point your panel items reach.
            // You'll have to change this size depending on the total size of items for your case.
            // rTB_Infos.ScrollBars VerticalScroll.Maximum = 280;
        }

        public new Font Font
        {
            get { return rTB_Infos.Font; }
            set
            {
                foreach (Control control in this.Controls)
                {
                    control.Font = value;
                }
            }
        }

        public ApiInfo[] InfosToShow
        {
            get;
            set;
        }

        public void SetChatInfos(ChannelInfo si)
        {
            SetChatInfos(si.ApiResult);
        }

        public void SetChatInfos(ApiResult si, string newLine = "\r\n")
        {
            channel = si;
            rTB_Infos.ResetText();
            var info = si.Info(newLine, InfosToShow);
            rTB_Infos.AppendText(info);
            lbl_OnlineTime.Text = si.OnlineTimeString;
        }

        private void ApiInfoPanel_Load(object sender, EventArgs e)
        {
        }

        private void lbl_OnlineTime_SizeChanged(object sender, EventArgs e)
        {
            splitContainer1.SplitterDistance = lbl_OnlineTime.Height;
        }

        private void rTB_Infos_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Settings.StartBrowser(e.LinkText);
        }

        private void rTB_Infos_TextChanged(object sender, EventArgs e)
        {
        }

        private void splitContainer1_FontChanged(object sender, EventArgs e)
        {
            splitContainer1.SplitterDistance = lbl_OnlineTime.Height;
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (channel != null)
                {
                    if ((int)channel.OnlineTime.TotalSeconds != oldRefreshTime)
                    {
                        lbl_OnlineTime.Text = channel.OnlineTimeString;
                        oldRefreshTime = (int)channel.OnlineTime.TotalSeconds;
                    }
                }
            }
            catch { }
        }
    }
}