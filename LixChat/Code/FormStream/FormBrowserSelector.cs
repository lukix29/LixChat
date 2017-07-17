using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LX29_Twitch.Forms
{
    public partial class FormBrowserSelector : Form
    {
        private Dictionary<string, string> browsers = new Dictionary<string, string>();

        private bool canClose = false;

        public FormBrowserSelector()
        {
            InitializeComponent();
        }

        public DialogResult Abort
        {
            get;
            private set;
        }

        public string BrowserName
        {
            get;
            private set;
        }

        public string BrowserPath
        {
            get;
            private set;
        }

        public Button ButtonCopy
        {
            get { return btn_copy; }
        }

        public Button ButtonSelect
        {
            get { return btn_Select; }
        }

        public string Title
        {
            get { return label1.Text; }
            set { label1.Text = value; }
        }

        public new void ShowDialog()
        {
            try
            {
                Abort = System.Windows.Forms.DialogResult.None;
                browsers = LX29_Tools.GetInstalledBrowsers();
                listView1.LargeImageList = new ImageList();
                listView1.LargeImageList.ImageSize = new Size(48, 48);
                ListViewItem lvimain = null;
                var defaultBrowser = LX29_Tools.GetSystemDefaultBrowser();
                foreach (var s in browsers)
                {
                    var icon = Icon.ExtractAssociatedIcon(s.Value);
                    var bitmap = icon.ToBitmap().MakeUnTransparent(this.BackColor, 200);
                    //bitmap.MakeTransparent(Color.White, 0.7f);

                    listView1.LargeImageList.Images.Add(s.Key, bitmap);

                    ListViewItem lvi = new ListViewItem(s.Key.ToTitleCase(), s.Key);

                    listView1.Items.Add(lvi);
                    if (s.Value.Equals(defaultBrowser, StringComparison.OrdinalIgnoreCase))
                    {
                        lvimain = lvi;
                        BrowserPath = s.Value;
                        BrowserName = s.Key;
                    }
                }
                var list = listView1.Items.Cast<ListViewItem>().Where(t => t.Text.ToLower().Contains(BrowserName)).ToList();
                if (list.Count > 0)
                {
                    list[0].Selected = true;
                }
                listView1.MultiSelect = false;
                listView1.Select();
                base.BringToFront();
                base.TopMost = true;
                base.ShowDialog();
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        ShowDialog();
                        break;

                    case MessageBoxResult.Abort:
                        Application.Exit();
                        return;
                }
            }
        }

        private void btn_copy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(LX29_Twitch.Api.TwitchApi.AuthApiUrl);
            canClose = true;
            Abort = DialogResult.Yes;
            this.Close();
        }

        private void btn_Select_Click(object sender, EventArgs e)
        {
            canClose = true;
            this.Close();
        }

        private void FormBrowserSelector_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (canClose) return;
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Abort = DialogResult.Abort;
            }
        }

        private void FormBrowserSelector_Load(object sender, EventArgs e)
        {
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string name = listView1.SelectedItems[0].Text.ToLower();
                BrowserPath = browsers[name];
                BrowserName = name;
            }
            catch
            {
            }
        }
    }
}