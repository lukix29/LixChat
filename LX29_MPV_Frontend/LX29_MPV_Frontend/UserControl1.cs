using LX29_MPV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

namespace mpv
{
    public enum InfoType
    {
        Cache,
        Volume,
        Text,
        None
    }

    public struct InfoData
    {
        public object Data;

        public InfoType Typ;

        public InfoData(InfoType typ, object data)
        {
            Data = data;
            Typ = typ;
        }

        public static InfoData Empty
        {
            get { return new InfoData() { Typ = InfoType.None, Data = null }; }
        }

        public bool IsEmpty
        {
            get { return Data == null; }
        }
    }

    public partial class InfoControl : UserControl
    {
        //private float cacheInfo = 0.0f;
        private InfoData info = new InfoData();

        private LX29_MPV.MpvLib mpv = null;
        private Dictionary<MPV_Property, object> mpvProperties = new Dictionary<MPV_Property, object>();
        private bool mute = false;

        //private TimeSpan timeInfo = new TimeSpan();
        //private float videoBitrate = 0.0f;
        private SolidBrush volumeBG = new SolidBrush(Color.FromArgb(100, 100, 100));

        public InfoControl()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            foreach (var val in Enum.GetValues(typeof(MPV_Property)))
            {
                mpvProperties.Add((MPV_Property)val, null);
            }
        }

        public void ShowInfo(InfoData info, bool Mute, LX29_MPV.MpvLib mpv)
        {
            this.mpv = mpv;

            //StringBuilder sb = new StringBuilder();
            //foreach (var p in Enum.GetValues(typeof(MPV_Property)))
            //{
            //    var v = mpv.GetProperty((MPV_Property)p);
            //    if (!string.IsNullOrEmpty(v))
            //    {
            //        sb.AppendLine(Enum.GetName(typeof(MPV_Property), (MPV_Property)p) + ":\t\t" + v);
            //    }
            //}
            //MessageBox.Show(sb.ToString());

            mute = Mute;
            this.info = info;
            this.Refresh();
            this.Visible = true;
            timer1.Enabled = true;
            timer2.Enabled = true;
        }

        public void ShowInfo(LX29_MPV.MpvLib mpv)
        {
            this.mpv = mpv;
            this.Refresh();
            this.Visible = true;
            timer1.Enabled = true;
            timer2.Enabled = true;
        }

        //protected override void OnMouseHover(EventArgs e)
        //{
        //    this.Visible = true;
        //    timer1.Enabled = true;
        //    timer2.Enabled = true;
        //    base.OnMouseHover(e);
        //}

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                e.Graphics.SetGraphicQuality(true, true);
                if (!info.IsEmpty)
                {
                    switch (info.Typ)
                    {
                        case InfoType.Volume:
                            e.Graphics.FillRectangle(volumeBG, 0, 0,
                                LXMath.Map((int)info.Data, 0, 120, 0, e.ClipRectangle.Width), e.ClipRectangle.Height);

                            DrawString("Volume: " + info.Data + "%" + (mute ? " (Muted)" : ""), 25f, e.Graphics, StringAlignment.Center, StringAlignment.Center);
                            break;

                        case InfoType.Text:
                            DrawString(info.Data.ToString(), 25f, e.Graphics, StringAlignment.Center, StringAlignment.Center);
                            break;
                    }
                }
                DrawString(TimeSpan.FromSeconds((float)mpvProperties[MPV_Property.time_pos]).ToString(@"hh\:mm\:ss"),
                    16f, e.Graphics, StringAlignment.Near, StringAlignment.Center);

                DrawString("Bitrate:" + ((float)mpvProperties[MPV_Property.audio_bitrate] + (float)mpvProperties[MPV_Property.video_bitrate]).SizeSuffix(3) +
                    "/s Cache: " + ((float)mpvProperties[MPV_Property.demuxer_cache_duration]).ToString("F0") + "s"
                    , 16f, e.Graphics, StringAlignment.Far, StringAlignment.Center);
            }
            catch
            {
            }
            base.OnPaint(e);
        }

        private void DrawString(string info, float size, Graphics g, StringAlignment Walign, StringAlignment Halign)
        {
            using (GraphicsPath gp = new GraphicsPath())
            using (Pen pen = new Pen(Color.Black, 4))
            {
                gp.AddString(info, this.Font.FontFamily, (int)this.Font.Style, size,
                    new Rectangle(0, 0, this.Width, this.Height),
                    new StringFormat() { Alignment = Walign, LineAlignment = Halign });

                g.DrawPath(pen, gp);
                g.FillPath(Brushes.White, gp);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            info = InfoData.Empty;
            if (!this.ClientRectangle.Contains(this.PointToClient(Cursor.Position)))
            {
                timer2.Enabled = false;
                timer1.Enabled = false;
                this.Visible = false;
            }
            //}
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (mpv != null)
            {
                foreach (var val in Enum.GetValues(typeof(MPV_Property)))
                {
                    var s = mpv.GetProperty((MPV_Property)val);
                    if (!string.IsNullOrEmpty(s))
                    {
                        float result = 0.0f;
                        if (float.TryParse(s, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out result))
                        {
                            mpvProperties[(MPV_Property)val] = result;
                        }
                        else
                        {
                            mpvProperties[(MPV_Property)val] = s;
                        }
                    }
                }
            }
            this.Visible = true;
            this.Refresh();
            //ShowInfo(new InfoData(InfoType.Cache, val));
        }
    }
}