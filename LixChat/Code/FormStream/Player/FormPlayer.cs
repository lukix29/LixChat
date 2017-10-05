using LX29_ChatClient.Channels;
using LX29_Helpers;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LX29_Twitch.Forms
{
    public partial class FormPlayer : Form
    {
        private bool isCLosing = false;
        private bool moveMouseDown = false;

        private Rectangle oldSize = new Rectangle();
        private bool resizeMouseDown = false;
        private ChannelInfo stream = null;

        public FormPlayer()
        {
            InitializeComponent();
        }

        public void Show(ChannelInfo si, string quality)
        {
            try
            {
                stream = si;

                quality = quality.ToUpper().Trim();
                chatPanel1.ChatView.SetChannel(si.ApiResult, LX29_ChatClient.MsgType.All_Messages);

                this.Text = stream.DisplayName;

                playerControl.Start(quality, si);
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        Show(si, quality);
                        break;

                    case MessageBoxResult.Abort:
                        this.Close();
                        return;
                }
            }

            base.Show();
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_ShowChat_Click(object sender, EventArgs e)
        {
            splitContainer.Panel2Collapsed = !splitContainer.Panel2Collapsed;
            chatPanel1.Pause = splitContainer.Panel2Collapsed;
            btn_ShowChat.Text = (splitContainer.Panel2Collapsed) ? "Show Chat" : "Hide Chat";
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            btn_Close.Visible = true;
        }

        private void FormPreviewPopout_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (isCLosing) return;
                isCLosing = true;
                playerControl.Stop();
            }
            catch { }
        }

        private void FormPreviewPopout_Load(object sender, EventArgs e)
        {
            panelMove.BackgroundImage = Cursors.SizeAll.GetImage();
            panelMove.BackgroundImageLayout = ImageLayout.Stretch;
        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            resizeMouseDown = true;
        }

        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (resizeMouseDown)
            {
                Size parent = this.Size;
                int W = Math.Max(800, Cursor.Position.X - this.Left);
                int H = Math.Max((int)(800 / 1.777777777), Cursor.Position.Y - this.Top);

                this.Width = W;

                this.Height = H;
            }
        }

        private void panel2_MouseUp(object sender, MouseEventArgs e)
        {
            resizeMouseDown = false;
        }

        private void panelMove_MouseDown_1(object sender, MouseEventArgs e)
        {
            moveMouseDown = true;
        }

        private void panelMove_MouseEnter(object sender, EventArgs e)
        {
            panelMove.Visible = true;
        }

        private void panelMove_MouseMove_1(object sender, MouseEventArgs e)
        {
            if (moveMouseDown)
            {
                Size parent = this.Size;
                int X = Cursor.Position.X - (panelMove.Right);
                int Y = Cursor.Position.Y - (panelMove.Bottom);
                this.Location = new Point(X, Y);
            }
        }

        private void panelMove_MouseUp_1(object sender, MouseEventArgs e)
        {
            moveMouseDown = false;
        }

        private void playerControl1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (playerControl.SetFullscreen())
                {
                    oldSize = this.Bounds;

                    this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

                    Rectangle r = Screen.GetBounds(this);
                    this.Location = r.Location;
                    this.Size = r.Size;

                    panelMove.Visible = panelResize.Visible = false;
                    this.TopMost = true;
                    this.BringToFront();
                }
                else
                {
                    if (!playerControl.IsOnTop)
                    {
                        this.TopMost = false;
                    }
                    if (!playerControl.IsBorderless)
                    {
                        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                    }
                    this.Location = oldSize.Location;
                    this.Size = oldSize.Size;

                    panelMove.Visible = panelResize.Visible = true;
                }
            }
            catch
            {
            }
        }

        private void playerControl1_OnBordelessChanged(bool value)
        {
            if (value)
            {
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            }
            else
            {
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            }
            panelMove.Visible = panelResize.Visible = btn_Close.Visible = value;
        }

        private void playerControl1_OnPlayerControlsShown(bool shown, bool isFullscreen)
        {
            if (playerControl.IsBorderless)
            {
                panelMove.Visible = shown && !isFullscreen;
                btn_Close.Visible = shown;
                panelResize.Visible = shown && !isFullscreen;
            }
            btn_ShowChat.Visible = shown;
        }

        private void playerControl1_OnTopMostChanged(bool value)
        {
            if (!value)
            {
                this.TopMost = false;
            }
            else
            {
                this.TopMost = true;
                this.BringToFront();
            }
        }
    }
}