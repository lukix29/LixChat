using System;
using System.Drawing;
using System.Windows.Forms;

namespace LX29_SettingsIO
{
    public partial class IntXY_Ctrl : UserControl
    {
        private object value;

        public IntXY_Ctrl()
        {
            InitializeComponent();
        }

        public delegate void ValueChangedEvent(object o, EventArgs e);

        public event EventHandler ValueChanged;

        public object Value
        {
            get { return value; }
            set
            {
                this.value = value;
                setNUD();
            }
        }

        private void IntXY_Ctrl_Load(object sender, EventArgs e)
        {
            numericUpDown1.Location = new Point(0, 0);
            numericUpDown1.BackColor = Color.FromArgb(30, 30, 30);
            label1.Location = new Point(numericUpDown1.Right, 0);
            numericUpDown2.Location = new Point(label1.Right, 0);
            numericUpDown2.BackColor = Color.FromArgb(30, 30, 30);
            this.Size = new Size(numericUpDown2.Right, numericUpDown1.Height);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            setValue();
            if (ValueChanged != null)
                ValueChanged(this, e);
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            setValue();
            if (ValueChanged != null)
                ValueChanged(this, e);
        }

        private void setNUD()
        {
            try
            {
                if (value is Point)
                {
                    Point p = (Point)value;
                    numericUpDown1.Value = p.X;
                    numericUpDown2.Value = p.Y;
                }
                else if (value is Size)
                {
                    Size s = (Size)value;
                    numericUpDown1.Value = s.Width;
                    numericUpDown2.Value = s.Height;
                }
            }
            catch
            {
            }
        }

        private void setValue()
        {
            decimal x = numericUpDown1.Value;
            decimal y = numericUpDown2.Value;
            if (value is Point)
            {
                value = new Point((int)x, (int)y);
            }
            else if (value is Size)
            {
                value = new Size((int)x, (int)y);
            }
        }
    }
}