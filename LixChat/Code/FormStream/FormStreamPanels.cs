using LX29_ChatClient.Channels;
using LX29_Twitch.Api;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LX29_Twitch.Forms
{
    public partial class FormStreamPanels : Form
    {
        public FormStreamPanels()
        {
            InitializeComponent();
        }

        public static Panel CreatePanel(string title, Image image, string imgUrl, string text)
        {
            var panel = new Panel();
            var label1 = new Label();
            var pictureBox1 = new PictureBox();
            var richTextBox1 = new RichTextBox();
            var webBrowser1 = new WebBrowser();
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Width = 350;

            if (!string.IsNullOrEmpty(title))
            {
                label1.Anchor = AnchorStyles.None;
                label1.FlatStyle = FlatStyle.Flat;
                label1.ForeColor = Color.WhiteSmoke;
                label1.Font = new Font("Microsoft Sans Serif", 14.25F);
                label1.Text = title;
                label1.TextAlign = ContentAlignment.MiddleCenter;
                label1.Width = panel.Width;
            }
            if (image != null)
            {
                pictureBox1.Anchor = AnchorStyles.None;
                pictureBox1.Width = panel.Width;
                pictureBox1.Height = image.Height;
                //pictureBox1.BorderStyle = BorderStyle.FixedSingle;
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox1.TabStop = false;
                pictureBox1.Image = image;
                if (!string.IsNullOrEmpty(imgUrl))
                {
                    pictureBox1.MouseEnter += delegate(object sender, EventArgs e)
                    {
                        pictureBox1.Cursor = Cursors.Hand;
                    };
                    pictureBox1.MouseLeave += delegate(object sender, EventArgs e)
                    {
                        pictureBox1.Cursor = Cursors.Arrow;
                    };
                    pictureBox1.MouseClick += delegate(object sender, MouseEventArgs e)
                    {
                        System.Diagnostics.Process.Start(imgUrl);
                    };
                }
            }
            if (!string.IsNullOrEmpty(text))
            {
                webBrowser1.Height = panel.Width * 2;
                webBrowser1.Width = panel.Width;
                webBrowser1.BackColor = Color.FromArgb(40, 40, 40);
                webBrowser1.Font = new Font("Arial", 10F);
                webBrowser1.ForeColor = Color.WhiteSmoke;
                webBrowser1.Anchor = AnchorStyles.None;
                StringBuilder html = new StringBuilder();
                html.AppendLine("<html>");
                html.AppendLine("<head>");
                html.AppendLine("</head>");
                html.AppendLine("<body style='font-family=\"Arial\"' bgcolor=\"rgb(40,40,40)\">");
                html.AppendLine(text.Replace("\\n", "").
                    Replace("<p>", "<p style=\"color:white;\">").
                    Replace("<li>", "<li style=\"color:white;\">"));
                html.AppendLine("</body></html>");
                webBrowser1.DocumentText = html.ToString();
                webBrowser1.DocumentCompleted += delegate(object sender, WebBrowserDocumentCompletedEventArgs e)
                    {
                        webBrowser1.Height = webBrowser1.Document.Window.Size.Height;
                    };
                webBrowser1.Navigating += delegate(object sender, WebBrowserNavigatingEventArgs e)
                {
                    e.Cancel = true;
                    System.Diagnostics.Process.Start(e.Url.ToString());
                };
            }

            int y = 0;
            panel.Height = 0;
            if (!string.IsNullOrEmpty(title))
            {
                label1.Location = new Point();
                panel.Height = label1.Height;
                y = label1.Height;
                panel.Controls.Add(label1);
            }
            if (image != null)
            {
                panel.Height += pictureBox1.Height;
                panel.Controls.Add(pictureBox1);
            }
            if (!string.IsNullOrEmpty(text))
            {
                panel.Height += webBrowser1.Height;
                panel.Controls.Add(webBrowser1);
            }
            label1.Location = new Point();
            pictureBox1.Location = new Point(0, y);
            webBrowser1.Location = new Point(0, pictureBox1.Bottom);
            //panel.Controls.Add(pictureBox1);
            //if (!text))
            //{
            //    panel.Controls.Add(richTextBox1);
            //}

            return panel;
        }

        public void Show(ChannelInfo info)
        {
            var results = TwitchApi.GetStreamPanels(info.Name);
            List<TableLayoutPanel> tables = new List<TableLayoutPanel>();
            foreach (var result in results)
            {
                var table = CreatePanel(result.Title, result.Image, result.Link, result.HTML);
                if (table.Controls.Count > 0)
                {
                    this.flowLayoutPanel1.Controls.Add(table);
                }
            }
            this.flowLayoutPanel1.VerticalScroll.Value = 0;
            base.Show();
        }

        private void FormStreamPanels_Load(object sender, EventArgs e)
        {
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
        }
    }
}