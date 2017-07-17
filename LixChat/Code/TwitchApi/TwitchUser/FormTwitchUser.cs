using System;
using System.Drawing;
using System.Windows.Forms;

namespace LX29_Twitch.Api.Controls
{
    public partial class FormTwitchUser : Form
    {
        private TwitchUserCollection Users;

        public FormTwitchUser()
        {
            InitializeComponent();
        }

        public delegate void ChangedToken(TwitchUser new_user);

        public event ChangedToken OnChangedToken;

        public void Show(TwitchUserCollection collection)
        {
            Users = collection;
            foreach (var user in Users.Values)
            {
                listBox1.Items.Add(user.Name);
            }
            listBox1.SelectedIndex = 0;
            base.Show();
        }

        private void addUser()
        {
            int idx = listBox1.SelectedIndex;
            listBox1.Items.Clear();
            foreach (var user in Users.Values)
            {
                listBox1.Items.Add(user.Name);
            }
            listBox1.SelectedIndex = idx;
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            Users.FetchNewToken(this, () => this.Invoke(new Action(addUser)), false);
        }

        private void btn_Remove_Click(object sender, EventArgs e)
        {
            string name = listBox1.SelectedItem.ToString();
            var result = Users.Remove(t => t.Name.Equals(name));
            listBox1.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name = listBox1.SelectedItem.ToString();
            var result = Users.SetSelected(t => t.Name.Equals(name));
            if (result == AddError.None)
            {
                button1.BackColor = Color.DarkGreen;
            }
            listBox1.Invalidate();
            if (OnChangedToken != null)
                OnChangedToken(Users.Selected);
            //Handler reconecting chat
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void FormTwitchUser_Load(object sender, EventArgs e)
        {
            listBox1.DrawMode = DrawMode.OwnerDrawFixed;
            listBox1.DrawItem += listBox1_DrawItem;
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            var element = Users[e.Index];
            Rectangle bounds = e.Bounds;
            bounds.Y = bounds.Height * e.Index;
            if (element.Selected)
            {
                e.Graphics.FillRectangle(Brushes.DarkGreen, bounds);
            }
            else
            {
                e.DrawBackground();
                // e.Graphics.FillRectangle(Brushes.Black, bounds);
            }
            e.Graphics.DrawString(element.Name, e.Font, Brushes.WhiteSmoke, bounds.Location);

            //e.DrawFocusRectangle();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var name = listBox1.SelectedItem.ToString();
            var result = Users.Values.Find(t => t.Name.Equals(name));
            apiInfoPanel1.SetChatInfos(result.ApiResult);
            lbl_name.Text = result.GetValue<string>(ApiInfo.display_name);
            button1.BackColor = result.Selected ? Color.DarkGreen : Color.Black;
        }
    }
}