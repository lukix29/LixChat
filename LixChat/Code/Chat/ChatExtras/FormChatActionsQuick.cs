using System;
using System.Drawing;
using System.Windows.Forms;

namespace LX29_ChatClient.Addons
{
    public partial class FormChatActionsQuick : Form
    {
        public FormChatActionsQuick()
        {
            InitializeComponent();
        }

        private void FormChatActionsQuick_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!autoChatActionsControl1.Close())
            {
                e.Cancel = true;
            }
        }

        private void FormChatActionsQuick_Load(object sender, EventArgs e)
        {
            //tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            //tabControl1.DrawItem += tabControl1_DrawItem;
        }

        private void quickTextControl1_Load(object sender, EventArgs e)
        {
        }

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            tabControl1.Appearance = TabAppearance.Buttons;
            using (SolidBrush sbg = new SolidBrush(Color.FromArgb(40, 40, 40)))
            {
                var item = tabControl1.TabPages[e.Index];
                e.DrawBackground();
                e.Graphics.FillRectangle(sbg, e.Bounds);
                e.Graphics.DrawString(item.Text, e.Font, Brushes.Gainsboro, e.Bounds, new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }
        }
    }
}