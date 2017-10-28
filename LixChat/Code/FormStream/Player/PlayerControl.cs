using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LX29_ChatClient.Channels;
using LX29_MPV;

namespace LX29_LixChat.Code.FormStream.Player
{
    public partial class PlayerControl : UserControl
    {
        private bool _ShowOnTopBorderless = true;
        private int hideCnt = 0;

        private bool isFullscreen = false;

        private bool lockVolume = false;
        private Point mousePoint = Point.Empty;
        private Point oldMousePoint = Point.Empty;
        private int oldVolume = 100;

        public PlayerControl()
        {
            InitializeComponent();

            this.volumeControl1.TextSelector = (d) =>
            {
                return d.ToString("F0");
            };
        }

        public delegate void BordelessChanged(bool isBorderless);

        public delegate void OnTopChanged(bool isOnTop);

        public delegate void PlayerControlsShown(bool controlsVisible, bool isFullscreen);

        public event BordelessChanged OnBordelessChanged;

        public event PlayerControlsShown OnPlayerControlsShown;

        public event OnTopChanged OnTopMostChanged;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsBorderless
        {
            get { return cB_Borderless.Checked; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsFullscreen
        {
            get { return isFullscreen; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsOnTop
        {
            get { return cB_OnTop.Checked; }
        }

        public Image PreviewImage
        {
            get { return panelVideo.Image; }
            set { panelVideo.Image = value; }
        }

        public string Quality
        {
            get;
            set;
        }

        //public bool ShowControls
        //{
        //    get;
        //    set;
        //}
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(true)]
        public bool ShowOnTopBorderless
        {
            get { return _ShowOnTopBorderless; }
            set { _ShowOnTopBorderless = comBox_previewQuali.Visible = cB_OnTop.Visible = cB_Borderless.Visible = value; }
        }

        public PictureBoxSizeMode SizeMode
        {
            get { return panelVideo.SizeMode; }
            set { panelVideo.SizeMode = value; }
        }

        public ChannelInfo Stream
        {
            get;
            set;
        }

        private MpvLib mpv
        {
            get
            {
                return Stream.MPV;
            }
        }

        public bool SetFullscreen()
        {
            if (_ShowOnTopBorderless)
            {
                if (isFullscreen)
                {
                    cB_OnTop.Visible = false;
                    cB_Borderless.Visible = false;
                }
                else
                {
                    cB_OnTop.Visible = true;
                    cB_Borderless.Visible = true;
                }
            }
            isFullscreen = !isFullscreen;
            return isFullscreen;
        }

        public void ShowStreamControls(bool visible)
        {
            if (visible)
            {
                if (_ShowOnTopBorderless)
                {
                    if (!isFullscreen)
                    {
                        cB_Borderless.Visible =
                         cB_OnTop.Visible = true;
                    }
                    else
                    {
                        cB_Borderless.Visible =
                         cB_OnTop.Visible = false;
                    }
                }
                else
                {
                    cB_Borderless.Visible =
                        cB_OnTop.Visible = false;
                }
                if (OnPlayerControlsShown != null)
                    OnPlayerControlsShown(true, isFullscreen);

                panelVideoControls.Location = new Point(0, this.Bottom - panelVideoControls.Height);
                panelVideoControls.Visible = true;
                panelVideoControls.BringToFront();
            }
            else
            {
                panelVideoControls.Visible = false;

                if (OnPlayerControlsShown != null)
                    OnPlayerControlsShown(false, isFullscreen);
            }
        }

        public void Start(string quality, ChannelInfo Stream)
        {
            try
            {
                Quality = quality;
                this.UseWaitCursor = true;
                this.Stream = Stream;
                IntPtr handle = panelVideo.Handle;
                int volume = (int)volumeControl1.Value;

                Task.Run(() =>
                {
                    var qualis = Stream.StreamURLS.KeysUpper;

                    this.BeginInvoke(new Action(() =>
                    {
                        //panelVideo.BackgroundImage = si.PreviewImage;
                        comBox_previewQuali.Items.Clear();
                        comBox_previewQuali.Items.AddRange(qualis);
                        comBox_previewQuali.SelectedItem = quality;
                    }));

                    startStream(handle, volume);
                });
                timer1.Enabled = true;
            }
            catch
            {
            }
        }

        public void Stop()
        {
            mpv.Dispose();
        }

        private void btn_Pause_CheckedChanged(object sender, EventArgs e)
        {
            if (lockVolume) return;
            if (!cb_Pause.Checked)
            {
                cb_Pause.BackgroundImage = LX29_LixChat.Properties.Resources.play;
                if (!_ShowOnTopBorderless) mpv.Pause(true);
                else mpv.Pause(true);
            }
            else
            {
                cb_Pause.BackgroundImage = LX29_LixChat.Properties.Resources.pause;
                if (comBox_previewQuali.Visible)
                {
                    Quality = comBox_previewQuali.SelectedItem.ToString();
                }
                startStream(panelVideo.Handle, (int)volumeControl1.Value);
            }
        }

        private void cB_Borderless_CheckedChanged(object sender, System.EventArgs e)
        {
            if (OnBordelessChanged != null)
                OnBordelessChanged(cB_Borderless.Checked);
        }

        private void cB_Mute_CheckedChanged(object sender, EventArgs e)
        {
            lockVolume = true;
            if (cB_Mute.Checked)
            {
                oldVolume = (int)volumeControl1.Value;

                mpv.SetProperty(MPV_Property.mute, true);

                volumeControl1.Value = 0;
                cB_Mute.BackgroundImage = LX29_LixChat.Properties.Resources.Mute;
            }
            else
            {
                mpv.SetProperty(MPV_Property.mute, false);
                volumeControl1.Value = oldVolume;
                cB_Mute.BackgroundImage = LX29_LixChat.Properties.Resources.noMute;
            }
            lockVolume = false;
        }

        private void cB_OnTop_CheckedChanged(object sender, System.EventArgs e)
        {
            if (OnTopMostChanged != null)
                OnTopMostChanged(cB_OnTop.Checked);
        }

        private void comBox_previewQuali_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            try
            {
                Quality = comBox_previewQuali.SelectedItem.ToString();
                if (mpv.IsRunning)
                {
                    startStream(panelVideo.Handle, (int)volumeControl1.Value);
                }
            }
            catch { }
        }

        private void panelVideo_MouseDoubleClick_1(object sender, MouseEventArgs e)
        {
            this.OnMouseDoubleClick(e);
        }

        private void panelVideo_MouseHover(object sender, EventArgs e)
        {
            ShowStreamControls(false);
        }

        private void panelVideo_MouseMove(object sender, MouseEventArgs e)
        {
            if (Math.Abs(mousePoint.X - e.X) >= 2 || Math.Abs(mousePoint.Y - e.Y) >= 2)
            {
                ShowStreamControls(true);
            }
            mousePoint = e.Location;
        }

        private void setBtnPause(bool pause)
        {
            this.Invoke(new Action(() =>
            {
                lockVolume = true;

                if (pause) cb_Pause.BackgroundImage = LX29_LixChat.Properties.Resources.pause;
                else cb_Pause.BackgroundImage = LX29_LixChat.Properties.Resources.play;

                cb_Pause.Checked = pause;
                lockVolume = false;
            }));
        }

        private void startStream(IntPtr handle, int volume)
        {
            try
            {
                var video = Stream.StreamURLS[Quality];
                if (video != null)
                {
                    //int volume = (int)this.Invoke(new Func<int>(() => { return (int)volumeControl1.Value; }));
                    //IntPtr hndl = (IntPtr)this.Invoke(new Func<IntPtr>(() => { return panelVideo.Handle; }));
                    if (mpv.Start(video.URL, handle, volume))
                    {
                        setBtnPause(true);
                    }
                    else
                    {
                        setBtnPause(false);
                    }
                }
                else
                {
                    setBtnPause(false);
                    LX29_MessageBox.Show("Stream is Offline");
                }
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        startStream(handle, volume);
                        break;
                }
            }
            finally
            {
                this.Invoke(new Action(() => { this.UseWaitCursor = false; }));
            }
        }

        private void timer1_Tick_1(object sender, System.EventArgs e)
        {
            try
            {
                //if (ShowControls)
                //{
                //    showStreamControls(true);
                //    return;
                //}
                if (Math.Abs(mousePoint.X - oldMousePoint.X) < 5 && Math.Abs(mousePoint.Y - oldMousePoint.Y) < 5)
                {
                    if (hideCnt++ >= 5)
                    {
                        ShowStreamControls(false);
                        hideCnt = 0;
                    }
                }
                oldMousePoint = mousePoint;
            }
            catch { }
        }

        private void volumeControl1_Load(object sender, EventArgs e)
        {
        }

        private void volumeControl1_ValueChanged_1(object sender, EventArgs e)
        {
            if (lockVolume) return;
            try
            {
                if (volumeControl1.Value > 0)
                {
                    cB_Mute.Checked = false;
                    mpv.SetVolume((int)volumeControl1.Value);
                }
                else
                {
                    cB_Mute.Checked = true;
                }
            }
            catch { }
        }
    }
}