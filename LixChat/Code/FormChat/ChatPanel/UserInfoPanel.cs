using LX29_Twitch.Api;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LX29_ChatClient.Forms
{
    public partial class UserInfoPanel : UserControl
    {
        private static ApiInfo[] sortArray = new ApiInfo[]{
                ApiInfo.name, ApiInfo.followers, ApiInfo.game ,
                ApiInfo.status , ApiInfo.viewers, ApiInfo.follow_created_at,
                ApiInfo.video_height , ApiInfo.average_fps, ApiInfo.views, ApiInfo.partner,
                ApiInfo.language, ApiInfo.created_at, ApiInfo.sub_plan};

        private bool mouseDown = false;

        private bool mouseMoveDown = false;

        public UserInfoPanel()
        {
            InitializeComponent();
            panel2.BackgroundImage = Cursors.SizeAll.GetImage();
            panel2.BackgroundImageLayout = ImageLayout.Stretch;
            panel2.BackgroundImage.Save("test.bmp");
        }

        public new Font Font
        {
            get { return apiInfoPanel1.Font; }
            set
            {
                apiInfoPanel1.Font = value;
                chatView1.Font = value;
            }
        }

        public new void Hide()
        {
            chatView1.Pause = true;
            mouseDown = false;
            mouseMoveDown = false;
            base.Hide();
        }

        public void Show(ChatUser user, Point location)
        {
            //this.Location = location;
            chatView1.ShowName = false;
            chatView1.Pause = false;
            //Get Chat State (slowmode, r9k, etc)
            //check if chat conenction is lost and reconnect
            //make this movable and resizeable and better
            //own whisper dingens
            //int i = null;
            chatView1.SetChannel(user.ChannelInfo, MsgType.All_Messages);
            chatView1.SetAllMessages(MsgType.All_Messages, null, user.Name);
            label1.Text = user.DisplayName;
            apiInfoPanel1.InfosToShow = sortArray;
            Task.Run(() =>
            {
                var result = user.ApiResult;
                this.BeginInvoke(new Action(() => apiInfoPanel1.SetChatInfos(result, "\r\n")));
            });
            base.Show();
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                Size parent = this.Parent.Size;
                int W = Math.Max(100, panel1.Left + e.X);
                int H = Math.Max(100, panel1.Top + e.Y);
                if (this.Left + W <= parent.Width)
                {
                    this.Width = W;
                }
                if (this.Top + H <= parent.Height)
                {
                    this.Height = H;
                }
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            mouseMoveDown = true;
        }

        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseMoveDown)
            {
                Size parent = this.Parent.Size;
                int X = Math.Max(0, Math.Min(parent.Width - this.Width, this.Left + e.X));
                int Y = Math.Max(0, Math.Min(parent.Height - this.Height, this.Top + e.Y));
                this.Location = new Point(X, Y);
            }
        }

        private void panel2_MouseUp(object sender, MouseEventArgs e)
        {
            mouseMoveDown = false;
        }

        private void splitContainer1_MouseDown(object sender, MouseEventArgs e)
        {
        }

        private void splitContainer1_MouseMove(object sender, MouseEventArgs e)
        {
        }

        private void splitContainer1_MouseUp(object sender, MouseEventArgs e)
        {
        }

        private void UserInfoPanel_Load(object sender, EventArgs e)
        {
        }
    }
}