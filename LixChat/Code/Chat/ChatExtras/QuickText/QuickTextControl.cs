using System;
using System.Windows.Forms;

namespace LX29_ChatClient.Addons
{
    public partial class QuickTextControl : UserControl
    {
        public QuickTextControl()
        {
            InitializeComponent();
        }

        private string selectedKey
        {
            get
            {
                return lstB_Main.SelectedItem.ToString().GetBetween("-  ", " ");
            }
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            if (rtb_add.TextLength > 0 && txtB_Alias.TextLength > 1)
            {
                ChatClient.QuickText.Add(txtB_Alias.Text, rtb_add.Text);
                loadListBox();
            }
        }

        private void btn_remove_Click(object sender, EventArgs e)
        {
            if (lstB_Main.SelectedIndex >= 0)
            {
                ChatClient.QuickText.Remove(selectedKey);
                loadListBox();
            }
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void loadListBox()
        {
            try
            {
                lstB_Main.BeginUpdate();
                int selidx = lstB_Main.SelectedIndex;
                lstB_Main.Items.Clear();
                int idx = 0;
                string maxlenstr = "D" + ChatClient.QuickText.Values.Count.ToString().Length;
                foreach (var kvp in ChatClient.QuickText.Values)
                {
                    idx++;
                    lstB_Main.Items.Add(idx.ToString(maxlenstr) + " -  " + kvp.Key + "  =>  " + kvp.Value);
                }
                lstB_Main.SelectedIndex = selidx;
            }
            catch
            {
            }
            finally
            {
                lstB_Main.EndUpdate();
            }
        }

        private void lstB_Main_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstB_Main.SelectedIndex >= 0)
            {
                var key = selectedKey;
                rtb_add.Text = ChatClient.QuickText[key];
                txtB_Alias.Text = key;
            }
        }

        private void QuickTextControl_Load(object sender, EventArgs e)
        {
            rtb_add.AddContextMenu();
            loadListBox();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void txtB_Alias_TextChanged(object sender, EventArgs e)
        {
            if (txtB_Alias.TextLength > 0)
            {
                if (char.IsLetterOrDigit(txtB_Alias.Text[0]))
                {
                    txtB_Alias.Text = txtB_Alias.Text.Insert(0, "/");
                    txtB_Alias.Select(txtB_Alias.TextLength, 1);
                }
            }
        }
    }
}