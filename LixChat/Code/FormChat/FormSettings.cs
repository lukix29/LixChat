using System;
using System.Windows.Forms;

namespace LX29_ChatClient.Forms
{
    public partial class FormSettings : Form
    {
        public FormSettings()
        {
            IsClosed = true;
            InitializeComponent();
        }

        public bool IsClosed
        {
            get;
            private set;
        }

        public new void Show()
        {
            IsClosed = false;
            base.Show();
        }

        private void FormSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            IsClosed = true;
        }

        private void FormSettings_Load(object sender, EventArgs e)
        {
            controlSettings1.Show();
        }
    }
}