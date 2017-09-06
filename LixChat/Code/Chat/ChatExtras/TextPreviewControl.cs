using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LX29_ChatClient.Addons
{
    public partial class TextPreviewControl : TextBox
    {
        private bool blink = false;
        private string restText = "";

        public TextPreviewControl()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

        public string RestText
        {
            get { return restText; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    restText = value;
                    this.Refresh();
                }
            }
        }

        public void SearchArray(IEnumerable<string> names)
        {
            string result = names.FirstOrDefault(t => t.StartsWith(Text));
            RestText = result;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Text += restText.Replace(Text, "");

                int idx = Math.Max(0, Text.Length);
                this.Select(idx, 1);
            }
            base.OnKeyDown(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            int idx = Math.Max(0, Text.Length);
            this.Select(idx, 1);
            this.Refresh();
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            this.Refresh();
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            int idx = Math.Max(0, Text.Length);
            this.Select(idx, 1);
            this.Refresh();
            base.OnMouseUp(mevent);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            float y = 0;
            float x = ClientRectangle.X + 2;
            SizeF siz = e.Graphics.MeasureText("A", Font);
            y = ClientSize.Height / 2f - siz.Height / 2f;

            HideCaret(this.Handle);
            e.Graphics.Clear(BackColor);
            if (Text.Length > 0)
            {
                siz = e.Graphics.MeasureText(Text, Font);
                e.Graphics.DrawText(Text, Font, ForeColor, x, y);
                x += siz.Width;
            }
            if (blink) e.Graphics.FillRectangle(Brushes.Black, x + 1, y, 1, siz.Height);

            if (Text.Length > 0)
            {
                x += 4;
                e.Graphics.DrawText(restText.Replace(Text, ""), Font, Color.Gray, x, y);
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            this.Refresh();
            base.OnTextChanged(e);
        }

        [DllImport("user32.dll")]
        private static extern bool HideCaret(IntPtr hWnd);

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.Focused)
            {
                blink = !blink;
            }
            else
            {
                blink = false;
            }
            this.Invalidate();
        }
    }
}