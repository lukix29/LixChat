using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace LX29_Twitch.Forms
{
    public partial class VolumeControl : UserControl
    {
        private Font font = new Font("Consolas", 12f, FontStyle.Bold);

        private StringFormat format;

        private decimal max = 100;
        private decimal min = 0;
        private bool mouseDown = false;

        private int mousePosX = 0;
        private decimal value = 0;

        public VolumeControl()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            TextSelector = new Func<decimal, string>(t => t.ToString());
        }

        public delegate void ValueChangedEvent(object o, EventArgs e);

        [EditorBrowsable]
        public event EventHandler ValueChanged;

        [EditorBrowsable]
        public decimal Maximum
        {
            get { return max; }
            set { max = value; }
        }

        [EditorBrowsable]
        public decimal Minimum
        {
            get { return min; }
            set { min = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<decimal, string> TextSelector
        {
            get;
            set;
        }

        [EditorBrowsable]
        public decimal Value
        {
            get { return value; }
            set
            {
                this.mousePosX = (int)Map(value, min, max, 0, this.Width);
                this.value = value;
                this.Refresh();
            }
        }

        public static decimal Map(decimal value, decimal fromSource, decimal toSource, decimal fromTarget, decimal toTarget)
        {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Subtract | Keys.OemMinus:
                    Setvalue(mousePosX - 1);
                    break;

                case Keys.Add | Keys.Oemplus:
                    Setvalue(mousePosX + 1);
                    break;
            }
            base.OnKeyDown(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            mouseDown = true;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            this.Focus();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (mouseDown)
            {
                Setvalue(e.X);
                //if (e.X > this.Width || e.X < 0)
                //{
                //    mouseDown = false;
                //}
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            mouseDown = false;
            Setvalue(e.X);
            base.OnMouseUp(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.Visible)
            {
                e.Graphics.FillRectangle(new SolidBrush(this.ForeColor), 0, 0, this.mousePosX, this.Height);
                e.Graphics.FillRectangle(new SolidBrush(this.BackColor), this.mousePosX, 0, Math.Max(0, this.Width - this.mousePosX), this.Height);
                string text = value.ToString();
                try
                {
                    text = TextSelector(value);
                }
                catch { }
                e.Graphics.DrawString(text, font, Brushes.Black, new Rectangle(0, 0, this.Width, this.Height), format);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            this.mousePosX = (int)Map(value, min, max, 0, this.Width);
            this.Refresh();
        }

        private void Setvalue(decimal x)
        {
            this.mousePosX = (int)Math.Max(0, Math.Min(this.Width, x));
            value = Math.Max(min, Math.Min(max, Map(x, 0, this.Width, min, max)));
            if (ValueChanged != null)
                ValueChanged(this, new EventArgs());
            this.Refresh();
        }

        private void VolumeControl_Load(object sender, EventArgs e)
        {
            format = new StringFormat();
            format.Alignment = StringAlignment.Near;
            format.LineAlignment = StringAlignment.Center;
            this.Refresh();
        }
    }
}