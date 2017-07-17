using LX29_ChatClient.Channels;
using System;
using System.Linq;
using System.Windows.Forms;

namespace LX29_ChatClient.Addons
{
    public partial class QuickTextControl : UserControl
    {
        public QuickTextControl()
        {
            InitializeComponent();
        }

        public ChannelInfo Channel
        {
            get;
            private set;
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            if (rtb_add.TextLength > 0)
            {
                ChatClient.QuickText.Add(new QuickText(Channel.Name, rtb_add.Text));
            }
        }

        private void QuickTextControl_Load(object sender, EventArgs e)
        {
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void SetQuickText(ChannelInfo channel)
        {
            Channel = channel;
            var texts = ChatClient.QuickText.Where(t => t.Channel.Equals(channel.Name, StringComparison.OrdinalIgnoreCase));
            lstB_Main.Items.Clear();
            foreach (var text in texts)
            {
                lstB_Main.Items.Add(text.Message);
            }
        }
    }
}