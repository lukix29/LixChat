﻿using System.Drawing;

namespace System.Windows.Forms
{
    public enum MessageBoxResult
    {
        OK,
        Cancel,
        Retry,
        Abort,
        Ignore,
        No,
        Yes,
        None
    }

    public partial class MessageBoxForm : Form
    {
        private MessageBoxResult diagRes = MessageBoxResult.None;

        public MessageBoxForm()
        {
            InitializeComponent();
            rtB_Main.LinkClicked += txtB_Main_LinkClicked;
            rtB_Main.AddContextMenu();
        }

        public new Color BackColor
        {
            get { return base.BackColor; }
            set
            {
                base.BackColor = value;
                rtB_Main.BackColor = value;
                button1.BackColor = Color.FromArgb(40, 40, 40);
                button2.BackColor = button1.BackColor;
                button3.BackColor = button1.BackColor;
            }
        }

        public new string Text
        {
            get { return rtB_Main.Text; }
            set
            {
                rtB_Main.Text = value;
            }
        }

        public string Title
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        public LX29_MessageBoxResult ShowDialog(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, bool ShowTextbox = false, string textBoxText = "", string textBoxTitle = "")
        {
            this.Text = text.Trim('\r', '\n', ' ');
            //var temp = text.Split("\r\n");
            //List<string> lines = new List<string>();
            //foreach (var line in temp)
            //{
            //    if (line.Length > 100)
            //    {
            //        lines.Add(line.Remove(101));
            //        lines.Add(line.Substring(100));
            //    }
            //    else
            //    {
            //        lines.Add(line);
            //    }
            //}
            //var msr = lines.OrderByDescending(t => t.Length).First();
            //Size s = TextRenderer.MeasureText(this.Text, new Font(this.Font.Name, this.Font.Size + 1.2f));
            //this.Width = Math.Max(350, s.Width);
            //this.Height = Math.Min(640, s.Height + (this.Height - button1.Top) + txtB_Main.Top);

            //520
            if (!ShowTextbox)
            {
                label1.Location = button1.Location;
                rtB_Main.Height = (label1.Top - rtB_Main.Top) - 5;
            }
            var border = this.GetBorderSize();
            var s = rtB_Main.GetPreferredSize(new System.Drawing.Size());
            int width = Math.Max(520, s.Width + (border.SmallBorder * 4) + rtB_Main.Left + (this.ClientSize.Width - rtB_Main.Right));
            int height = Math.Min(640, s.Height + (this.ClientSize.Height - label1.Top) +
                rtB_Main.Top + border.BigSmall + 10);

            Screen screen = Screen.FromControl(this);
            this.Width = Math.Min(screen.Bounds.Width - this.Left, width);
            this.Height = Math.Min(screen.Bounds.Height - this.Top, height);

            this.Title = caption;
            switch (icon)
            {
                case MessageBoxIcon.Error:
                    BackColor = Color.DarkRed;
                    System.Media.SystemSounds.Exclamation.Play();
                    break;

                case MessageBoxIcon.Question:
                    System.Media.SystemSounds.Question.Play();
                    break;

                case MessageBoxIcon.Asterisk:
                    System.Media.SystemSounds.Asterisk.Play();
                    break;
            }

            switch (buttons)
            {
                case MessageBoxButtons.OK:
                    button1.Text = "OK";
                    button2.Text = "";
                    button3.Text = "";
                    break;

                case MessageBoxButtons.OKCancel:
                    button1.Text = "OK";
                    button2.Text = "Cancel";
                    button3.Text = "";
                    break;

                case MessageBoxButtons.AbortRetryIgnore:
                    button1.Text = "Retry";
                    button2.Text = "Abort";
                    button3.Text = "Ignore";
                    break;

                case MessageBoxButtons.YesNoCancel:
                    button1.Text = "OK";
                    button2.Text = "No";
                    button3.Text = "Cancel";
                    break;

                case MessageBoxButtons.YesNo:
                    button1.Text = "Yes";
                    button2.Text = "No";
                    button3.Text = "";
                    break;

                case MessageBoxButtons.RetryCancel:
                    button1.Text = "Retry";
                    button2.Text = "Cancel";
                    button3.Text = "";
                    break;
            }
            button1.Visible = (button1.Text.Length > 0);
            button2.Visible = (button2.Text.Length > 0);
            button3.Visible = (button3.Text.Length > 0);

            textBox1.Visible = ShowTextbox;
            textBox1.Text = textBoxText;
            label1.Visible = ShowTextbox;
            label1.Text = textBoxTitle;
            if (ShowTextbox)
                textBox1.Show();

            this.BringToFront();
            this.TopMost = true;
            base.ShowDialog();
            return new LX29_MessageBoxResult(diagRes, textBox1.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            diagRes = (MessageBoxResult)Enum.Parse(typeof(MessageBoxResult), button1.Text);
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            diagRes = (MessageBoxResult)Enum.Parse(typeof(MessageBoxResult), button2.Text);
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            diagRes = (MessageBoxResult)Enum.Parse(typeof(MessageBoxResult), button3.Text);
            this.Close();
        }

        private void MessageBoxForm_Load(object sender, EventArgs e)
        {
            //txtB_Main.WordWrap = false;
            //txtB_Main.ContentsResized += txtB_Main_ContentsResized;
        }

        private void MessageBoxForm_Resize(object sender, EventArgs e)
        {
            base.Text = this.Size.ToString();
        }

        private void txtB_Main_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            LX29_ChatClient.Settings.StartBrowser(e.LinkText);
        }

        //private void txtB_Main_ContentsResized(object sender, ContentsResizedEventArgs e)
        //{
        //    var richTextBox = (RichTextBox)sender;
        //    richTextBox.Width = e.NewRectangle.Width;
        //    richTextBox.Height = e.NewRectangle.Height;

        //    this.Width = Math.Max(350, e.NewRectangle.Width);
        //    this.Height = Math.Min(640, e.NewRectangle.Height + (this.Height - button1.Top) + txtB_Main.Top);
        //}
    }
}

namespace System.Windows.Forms
{
    public struct LX29_MessageBoxResult
    {
        public MessageBoxResult Result;
        public string Value;

        public LX29_MessageBoxResult(MessageBoxResult result, string value)
        {
            Result = result;
            Value = value;
        }
    }

    public class LX29_MessageBox
    {
        public static MessageBoxResult Show(string text, string caption = "", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            using (MessageBoxForm f = new MessageBoxForm())
            {
                return f.ShowDialog(text, caption, buttons, icon).Result;
            }
        }

        public static LX29_MessageBoxResult Show(string text, bool ShowTextbox, string textBoxText, string textBoxTitle, string caption = "", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            using (MessageBoxForm f = new MessageBoxForm())
            {
                return f.ShowDialog(text, caption, buttons, icon, ShowTextbox, textBoxText, textBoxTitle);
            }
        }
    }
}