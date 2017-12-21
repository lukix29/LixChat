using LX29_MPV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace mpv
{
    public partial class Form1 : Form
    {
        private Point lastMouse = new Point();
        private bool mouseDown = false;
        private MpvLib mpv;
        private bool mute = false;
        private Rectangle oldBounds = new Rectangle();
        private FormWindowState oldState = FormWindowState.Normal;
        private string url = "";
        private Dictionary<string, string> arguments = new Dictionary<string, string>();

        private static Size zoomSize = new Size(640, 360);
        //private Graphics gZoom;
        //private void CreateZoomControl()
        //{
        //    if (!zoom.IsDisposed) zoom.Close();
        //    zoom = new Form();
        //    var pb = new PictureBox();
        //    pb.Name = "pb_main";
        //    pb.SizeMode = PictureBoxSizeMode.StretchImage;
        //    pb.Dock = DockStyle.Fill;
        //    pb.BackColor = Color.Black;
        //    zoom.ShowIcon = false;
        //    zoom.ShowInTaskbar = false;
        //    zoom.Controls.Add(pb);
        //    zoom.ClientSize = zoomSize;
        //    zoom.FormBorderStyle = FormBorderStyle.SizableToolWindow;
        //    zoom.BackColor = Color.Black;
        //}

        public Form1(string[] args)
        {
            try
            {
                InitializeComponent();
                //CreateZoomControl();
                //gZoom = panel3.CreateGraphics();
                //panel3.Size = zoomSize;

                panel2.MouseWheel += Form1_MouseWheel;
                this.MouseWheel += Form1_MouseWheel;

                if (args.Length > 0)
                {
                    arguments = new Dictionary<string, string>();
                    foreach (var arg in args)
                    {
                        var vals = arg.Split('=');
                        arguments.Add(vals[0].ToLower(), vals[1]);
                    }
                }
                SetPanel2();

                mpv = new MpvLib(panel2.Handle);
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message);
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
        }

        //private void SetZoomImage(Bitmap b)
        //{
        //    var pb = zoom.Controls.Find("pb_main", false);
        //    if (pb.Length > 0)
        //    {
        //        ((PictureBox)pb[0]).Image = new Bitmap(b);
        //    }
        //}

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (mpv != null)
                mpv.Dispose();
            //MessageBox.Show("Disposed");
        }

        private void StartTimer()
        {
            System.Threading.Timer t = null;
            t = new System.Threading.Timer(new System.Threading.TimerCallback((o) =>
                {
                    if (this.IsDisposed)
                    {
                        t.Change(-1, -1);
                        t.Dispose();
                    }
                    if (!mpv.IsRunning)
                    {
                        Program.ExitApplication(2);
                    }
                }), null, 5000, 5000);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (arguments.Count > 0)
            {
                if (arguments.ContainsKey("title"))
                {
                    this.Text = arguments["title"] + " | MPV-Frontend";
                }
                if (arguments.ContainsKey("bounds"))
                {
                    var bounds = arguments["bounds"].ParseRectangleScreenSafe();
                    if (!bounds.IsEmpty)
                    {
                        this.Location = bounds.Location;
                        this.Size = bounds.Size;
                    }
                }
                if (arguments.ContainsKey("url"))
                {
                    url = arguments["url"];
                }
                else if (arguments.ContainsKey("channel"))
                {
                    LX29_Twitch.Api.Video.VideoInfoCollection vic = new LX29_Twitch.Api.Video.VideoInfoCollection();
                    vic.LoadVideoInfos(arguments["channel"]);
                    if (arguments.ContainsKey("quality"))
                        url = vic[arguments["quality"].ToUpper()].URL;
                    else
                        url = vic["SOURCE"].URL;
                }

                if (!string.IsNullOrEmpty(url))
                {
                    if (!mpv.Start(url))
                    {
                        Program.ExitApplication(1);
                        return;
                    }
                    StartTimer();
                }
                else Program.ExitApplication(1);
            }
#if DEBUG
            else
            {
                LX29_Twitch.Api.Video.VideoInfoCollection vic = new LX29_Twitch.Api.Video.VideoInfoCollection();
                vic.LoadVideoInfos("monstercat");

                url = vic["SOURCE"].URL;
                if (mpv.Start(url))
                {
                    StartTimer();
                }
                else
                {
                    Program.ExitApplication(1);
                }
            }
#else
            else
            {
                Program.ExitApplication(1);
            }
#endif
        }

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (isZoomPressed)
            {
                zoomFac += Math.Sign(e.Delta);

                zoomFac = Math.Max(1, Math.Min(10, zoomFac));

                panel2.Size = zoomSize;

                panel2.Width = (int)(panel2.Width * zoomFac);
                panel2.Height = (int)(panel2.Height * zoomFac);
            }
            else
            {
                var value = mpv.GetProperty(MPV_Property.volume);
                if (!string.IsNullOrEmpty(value))
                {
                    var vol = (int)float.Parse(value, CultureInfo.InvariantCulture);
                    if (e.Delta > 0)
                    {
                        vol += 1;
                    }
                    else if (e.Delta < 0)
                    {
                        vol -= 1;
                    }

                    vol = Math.Max(0, Math.Min(120, vol));

                    mpv.SetProperty(MPV_Property.volume, vol);

                    panel1.ShowInfo(new InfoData(InfoType.Volume, vol), mute, mpv);
                }
            }
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            SetPanel2();
        }

        private void panel2_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                mpv.SetProperty(MPV_Property.mute, !mute ? "yes" : "no");
                mute = !mute;

                panel1.ShowInfo(new InfoData(InfoType.Text, mute ? "Mute: Yes" : "Mute: No"), mute, mpv);
            }
        }

        private void panel2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (oldBounds.IsEmpty)
            {
                oldState = this.WindowState;
                oldBounds = this.Bounds;

                var b = Screen.FromPoint(this.PointToScreen(e.Location)).Bounds;
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

                if (this.WindowState == FormWindowState.Maximized)
                {
                    this.WindowState = FormWindowState.Normal;
                    this.WindowState = FormWindowState.Maximized;
                }
                else
                {
                    this.Size = b.Size;
                    this.Location = b.Location;
                }
            }
            else
            {
                var b = oldBounds;
                oldBounds = Rectangle.Empty;

                this.Location = b.Location;
                this.Size = b.Size;

                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                this.WindowState = oldState;
            }
        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && oldBounds.IsEmpty)
            {
                mouseDown = true;
            }
        }

        private static double zoomFac = 4;

        //private Form zoom = new Form();
        //private Bitmap zoom = new Bitmap(
        //    (int)(zoomSize.Width / zoomFac),
        //    (int)(zoomSize.Height / zoomFac));

        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                int X = Cursor.Position.X - (panel2.Left + panel2.Width / 2);
                int Y = Cursor.Position.Y - (panel2.Top + panel2.Height / 2);
                this.Location = new Point(X, Y);
            }
            else
            {
                if (lastMouse.X != e.Location.X || lastMouse.Y != e.Location.Y)
                {
                    lastMouse = e.Location;
                    SetPanel2();
                    panel1.ShowInfo(mpv);
                }
            }
        }

        private void panel2_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void SetPanel2()
        {
            panel1.Height = (int)((this.Height / 100.0) * 4.0);
            panel1.Top = this.ClientSize.Height - panel1.Height;
            panel1.Left = 0;
            panel1.Width = this.ClientSize.Width;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            SetPanel2();
        }

        private bool isZoomPressed = false;

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Z)
            {
                isZoomPressed = !isZoomPressed;
                if (isZoomPressed)
                {
                    panel2.Dock = DockStyle.None;
                    zoomSize = panel2.Size;
                    panel2.Width = (int)(panel2.Width * zoomFac);
                    panel2.Height = (int)(panel2.Height * zoomFac);
                }
                else
                {
                    panel2.Dock = DockStyle.Fill;
                    panel2.Size = zoomSize;
                }
                timer2.Enabled = isZoomPressed;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (isZoomPressed)
            {
                float w = panel2.Width / (float)this.Width;
                float h = panel2.Height / (float)this.Height;

                var p0 = Cursor.Position;
                p0 = this.PointToClient(p0);

                float x = (p0.X - panel2.Width / 2.0f) * w;
                float y = (p0.Y - panel2.Height / 2.0f) * h;

                panel2.Left = (int)x;
                panel2.Top = (int)y;
            }
        }
    }
}