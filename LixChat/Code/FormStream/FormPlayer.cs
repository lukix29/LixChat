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

        private bool mouseDown = false;

        private Point mousePoint = Point.Empty;
        private Point oldMousePoint = Point.Empty;

        private Rectangle oldSize = new Rectangle();

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

                //cB_Borderless.Checked = LX29Settings.GetValue<bool>(SettingsNames.Preview_Borderless);
                //cB_OnTop.Checked = LX29Settings.GetValue<bool>(SettingsNames.Preview_On_Top);
                //this.Location = LX29Settings.GetValue<Point>(SettingsNames.Preview_Location);
                //this.Size = LX29Settings.GetValue<Size>(SettingsNames.Preview_Size);

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

        private void btn_Chat_Click_1(object sender, EventArgs e)
        {
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
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            }
            else
            {
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            }
            //LX29Settings.SetValue(SettingsNames.Preview_Borderless, cB_Borderless.Checked);
        }

        //private void cB_Mute_CheckedChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (!lockMute)
        //        {
        //            if (cB_Mute.Checked)
        //            {
        //                oldVolume = (int)volumeControl1.Value;
        //                volumeControl1.Value = 0;
        //            }
        //            else
        //            {
        //                volumeControl1.Value = oldVolume;
        //            }
        //            mpv.SetVolume(cB_Mute.Checked);
        //        }
        //    }
        //    catch { }
        //}

        private void cB_Borderless_CheckedChanged_1(object sender, EventArgs e)
        {
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

        private void cB_OnTop_CheckedChanged_1(object sender, EventArgs e)
        {
        }

        private void cb_Pause_CheckedChanged(object sender, EventArgs e)
        {
            mpv.Pause(cb_Pause.Checked);
        }

        private void cb_Pause_CheckedChanged_1(object sender, EventArgs e)
        {
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

        private void comBox_previewQuali_SelectedIndexChanged_1(object sender, EventArgs e)
        {
        }

        private void FormPlayer_MouseEnter(object sender, EventArgs e)
        {
        }

        private void FormPreviewPopout_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (isCLosing) return;
                isCLosing = true;
                mpv.Stop();

                // StreamManager.FireEvent(StreamManagerEventType.Updated, stream);
            }
            catch { }
        }

        private void FormPreviewPopout_Load(object sender, EventArgs e)
        {
            this.volumeControl1.TextSelector = (d) =>
            {
                return d.ToString("F0");
            };
            volumeControl1.ValueChanged += volumeControl1_ValueChanged;
            // LX29Settings.SettingsChanged += LX29Settings_SettingsChanged;
        }

        private void FormPreviewPopout_LocationChanged(object sender, EventArgs e)
        {
            // LX29Settings.SetValue(SettingsNames.Preview_Location, this.Location);
            // LX29Settings.SetValue(SettingsNames.Preview_Size, this.Size);
        }

        private void FormPreviewPopout_Resize(object sender, EventArgs e)
        {
            //float f = 1280f / 720f;
            //Size s = new Size(this.Width - this.ClientSize.Width, this.Height - this.ClientSize.Height);
            //this.Size = new Size((this.ClientSize.Width + s.Width), (int)(this.ClientSize.Width / f) + s.Height);
        }

        private void FormPreviewPopout_ResizeEnd(object sender, EventArgs e)
        {
            // LX29Settings.SetValue(SettingsNames.Preview_Location, this.Location);
            // LX29Settings.SetValue(SettingsNames.Preview_Size, this.Size);
        }

        private void LX29Settings_SettingsChanged()
        {
            // cB_Borderless.Checked = LX29Settings.GetValue<bool>(SettingsNames.Preview_Borderless);
            // this.Location = LX29Settings.GetValue<Point>(SettingsNames.Preview_Location);
            // this.Size = LX29Settings.GetValue<Size>(SettingsNames.Preview_Size);
            // cB_OnTop.Checked = LX29Settings.GetValue<bool>(SettingsNames.Preview_On_Top);
        }

        private void panel1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!isFullscreen)
            {
                cB_OnTop.Visible = false;
                cB_Borderless.Visible = false;
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

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
        }

        private void panel1_MouseLeave(object sender, EventArgs e)
        {
            showStreamControls(false);
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void panelVideo_MouseEnter(object sender, EventArgs e)
        {
            showStreamControls(true);
        }

        private void panelVideo_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown && !isFullscreen)
            {
                int x = Cursor.Position.X - this.Width / 2;
                int y = Cursor.Position.Y - this.Height / 2;
                this.Location = new Point(x, y);
            }
            mousePoint = e.Location;
            if (Math.Abs(mousePoint.X - oldMousePoint.X) > 4 && Math.Abs(mousePoint.Y - oldMousePoint.Y) > 4)
            {
                showStreamControls(true);
            }
        }

        private void showStreamControls(bool visible)
        {
            if (!visible)
            {
                cb_Pause.Visible =
                btnClose.Visible =
                          cB_Borderless.Visible =
                              cB_OnTop.Visible =
                          btn_Chat.Visible =
                              comBox_previewQuali.Visible =
                              button1.Visible =
                              volumeControl1.Visible = false;
            }
            else
            {
                if (!isFullscreen)
                {
                    cB_Borderless.Visible =
                        cB_OnTop.Visible = true;
                }
                cb_Pause.Visible =
                btnClose.Visible =
                btn_Chat.Visible =
                comBox_previewQuali.Visible =
                button1.Visible =
                volumeControl1.Visible = true;
                cb_Pause.BringToFront();
                btnClose.BringToFront();
                cB_Borderless.BringToFront();
                cB_OnTop.BringToFront();
                btn_Chat.BringToFront();
                comBox_previewQuali.BringToFront();
                button1.BringToFront();
                volumeControl1.BringToFront();
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
                if (!panelVideo.ClientRectangle.Contains(panelVideo.PointToClient(Cursor.Position)))
                {
                    if (!comBox_previewQuali.DroppedDown)
                    {
                        showStreamControls(false);
                    }
                }
                else
                {
                    //showStreamControls(true);

                    if (Math.Abs(mousePoint.X - oldMousePoint.X) < 4 && Math.Abs(mousePoint.Y - oldMousePoint.Y) < 4)
                    {
                        if (hideCnt++ > 15)
                        {
                            showStreamControls(false);
                            hideCnt = 0;
                        }
                    }
                    oldMousePoint = mousePoint;
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