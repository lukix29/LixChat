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
        private int hideCnt = 0;
        private bool isCLosing = false;

        private bool isFullscreen = false;

        //private bool lockMute = false;

        //private bool mouseDown = false;

        private Point mousePoint = Point.Empty;
        private bool moveMouseDown = false;
        private Point oldMousePoint = Point.Empty;

        private Rectangle oldSize = new Rectangle();

        private bool resizeMouseDown = false;
        private ChannelInfo stream = null;

        //private int oldVolume = 100;
        public FormPlayer()
        {
            InitializeComponent();
            panelVideo.MouseEnter += panelVideo_MouseEnter;
            panelVideo.MouseMove += panelVideo_MouseMove;
        }

        private MPV_Wrapper mpv
        {
            get
            {
                if (stream == null) return null;
                return stream.MPV;
            }
        }

        public void Show(ChannelInfo si, string quality)
        {
            base.Show();

            try
            {
                stream = si;

                quality = quality.ToUpper().Trim();
                chatPanel1.ChatView.SetChannel(si, LX29_ChatClient.MsgType.All_Messages);

                this.Text = stream.DisplayName;

                IntPtr handle = panelVideo.Handle;
                int volume = (int)volumeControl1.Value;

                Task.Run(() =>
                {
                    var qualis = stream.StreamURLS.KeysUpper;

                    this.BeginInvoke(new Action(() =>
                        {
                            //panelVideo.BackgroundImage = si.PreviewImage;
                            comBox_previewQuali.Items.Clear();
                            comBox_previewQuali.Items.AddRange(qualis);
                            comBox_previewQuali.SelectedItem = quality;
                        }));

                    startStream(quality, volume, handle);
                });
                timer1.Enabled = true;
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
        }

        private void btn_Chat_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel2Collapsed = !splitContainer1.Panel2Collapsed;
            chatPanel1.Pause = splitContainer1.Panel2Collapsed;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cB_Borderless_CheckedChanged(object sender, EventArgs e)
        {
            if (cB_Borderless.Checked)
            {
                panelMove.Visible = panelResize.Visible = true;
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            }
            else
            {
                panelMove.Visible = panelResize.Visible = false;
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            }
            //LX29Settings.SetValue(SettingsNames.Preview_Borderless, cB_Borderless.Checked);
        }

        private void cB_OnTop_CheckedChanged(object sender, EventArgs e)
        {
            if (!cB_OnTop.Checked)
            {
                this.TopMost = false;
            }
            else
            {
                this.TopMost = true;
                this.BringToFront();
            }
            // LX29Settings.SetValue(SettingsNames.Preview_Borderless, cB_OnTop.Checked);
        }

        private void cb_Pause_CheckedChanged(object sender, EventArgs e)
        {
            mpv.Pause(cb_Pause.Checked);
        }

        private void comBox_previewQuali_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (mpv.IsRunning)
                {
                    string url = stream.StreamURLS[comBox_previewQuali.SelectedItem.ToString()].URL;

                    mpv.Start(url, (int)volumeControl1.Value, Int16.MaxValue, panelVideo.Handle);
                }
            }
            catch { }
        }

        private void FormPreviewPopout_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (isCLosing) return;
                isCLosing = true;
                mpv.Stop();
            }
            catch { }
        }

        private void FormPreviewPopout_Load(object sender, EventArgs e)
        {
            panelMove.BackgroundImage = Cursors.SizeAll.GetImage();
            panelMove.BackgroundImageLayout = ImageLayout.Stretch;
            this.volumeControl1.TextSelector = (d) =>
            {
                return d.ToString("F0");
            };
            volumeControl1.ValueChanged += volumeControl1_ValueChanged;
            // LX29Settings.SettingsChanged += LX29Settings_SettingsChanged;
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

        private void panelVideo_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!isFullscreen)
            {
                cB_OnTop.Visible = false;
                cB_Borderless.Visible = false;
                panelMove.Visible = panelResize.Visible = false;
                oldSize = new Rectangle(this.Top, this.Left, this.Width, this.Height);
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

                Rectangle r = Screen.GetBounds(this);
                this.Location = r.Location;
                this.Size = r.Size;

                this.TopMost = true;
                this.BringToFront();
            }
            else
            {
                panelMove.Visible = panelResize.Visible = true;
                cB_OnTop.Visible = true;
                cB_Borderless.Visible = true;
                if (!cB_OnTop.Checked)
                {
                    this.TopMost = false;
                }
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;

                this.Location = oldSize.Location;
                this.Size = oldSize.Size;
            }
            isFullscreen = !isFullscreen;
        }

        private void panelVideo_MouseEnter(object sender, EventArgs e)
        {
            showStreamControls(true);
        }

        private void panelVideo_MouseMove(object sender, MouseEventArgs e)
        {
            if (Math.Abs(mousePoint.X - oldMousePoint.X) > 4 && Math.Abs(mousePoint.Y - oldMousePoint.Y) > 4)
            {
                showStreamControls(true);
            }
        }

        private void showStreamControls(bool visible)
        {
            if (!visible)
            {
                panelVideoControls.Visible = false;

                panelMove.Visible = false;
                panelVideo.Dock = DockStyle.Fill;
            }
            else
            {
                if (!isFullscreen)
                {
                    cB_Borderless.Visible =
                     cB_OnTop.Visible = true;
                    if (cB_Borderless.Checked)
                    {
                        panelMove.Visible = true;
                    }
                }
                else
                {
                    cB_Borderless.Visible =
                     cB_OnTop.Visible = false;
                }
                panelVideo.Dock = DockStyle.None;
                panelVideo.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                panelVideo.Location = new Point(0, 0);
                panelVideo.Size = new Size(splitContainer1.Panel1.Width, panelVideoControls.Top - 1);

                panelVideoControls.Visible = true;
            }
        }

        private void startStream(string quality, int volume, IntPtr handle)
        {
            //mpv = new MPV_Wrapper(stream.Name + DateTime.Now.Ticks);
            var video = stream.StreamURLS[quality];
            if (video != null)
            {
                mpv.Start(video.URL, volume, Int16.MaxValue, handle);
            }
            else
            {
                LX29_MessageBox.Show("Stream is Offline");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (panelVideoControls.ClientRectangle.Contains(panelVideoControls.PointToClient(Cursor.Position)))
                {
                    showStreamControls(true);
                }
                else if (!splitContainer1.Panel1.ClientRectangle.Contains(splitContainer1.Panel1.PointToClient(Cursor.Position)))
                {
                    if (!comBox_previewQuali.DroppedDown)
                    {
                        showStreamControls(false);
                    }
                }
                else
                {
                    if (Math.Abs(mousePoint.X - oldMousePoint.X) < 5 && Math.Abs(mousePoint.Y - oldMousePoint.Y) < 5)
                    {
                        if (hideCnt++ > 15)
                        {
                            showStreamControls(false);
                            hideCnt = 0;
                            oldMousePoint = mousePoint;
                        }
                    }
                    else
                    {
                        showStreamControls(true);
                    }
                }
            }
            catch { }
        }

        private void volumeControl1_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                mpv.SetVolume((int)volumeControl1.Value);
            }
            catch { }
        }
    }
}