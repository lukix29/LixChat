using LX29_ChatClient;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace LX29_Twitch.Api.Controls
{
    public partial class TokkenInput : UserControl
    {
        private const int minTokenLength = 28;

        private Process proc = new Process();

        private Token_HTTP_Server server = new Token_HTTP_Server(TempHtmlPath, 8080);

        public TokkenInput()
        {
            InitializeComponent();
        }

        public delegate void TokenAbort();

        public delegate void TokenReceived(string token);

        public event TokenAbort OnTokenAbort;

        public event TokenReceived OnTokenReceived;

        public string Token
        {
            get;
            private set;
        }

        private static string TempHtmlPath
        {
            get { return System.IO.Path.GetTempPath() + "lx29_tcvc\\"; }
        }

        public new void BringToFront()
        {
            base.BringToFront();
            bool startBrowser = true;
            var Abort = Settings.GetBrowserPath(false);
            if (Abort == DialogResult.Abort)
            {
                Application.Exit();
                return;
            }
            if (Abort == DialogResult.Yes)
            {
                startBrowser = false;
            }
            if (startBrowser)
            {
                System.IO.Directory.CreateDirectory(TempHtmlPath);
                System.IO.File.WriteAllText(TempHtmlPath + "index.html", LX29_TwitchChat.Properties.Resources.index);
                System.IO.File.WriteAllText(TempHtmlPath + "received.html", LX29_TwitchChat.Properties.Resources.received);
                System.IO.File.WriteAllText(TempHtmlPath + "success.html", LX29_TwitchChat.Properties.Resources.success);

                proc = Settings.StartBrowser(LX29_Twitch.Api.TwitchApi.AuthApiUrl);
                server.ReceivedToken += server_ReceivedToken;
                server.Start();
            }
        }

        private void btn_Abort_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            timer1.Dispose();
            textBox1.Text = "";
            this.Visible = false;
            if (OnTokenAbort != null)
                OnTokenAbort();
        }

        private void btn_retry_Click(object sender, EventArgs e)
        {
            proc.Close();
            proc = Settings.StartBrowser(LX29_Twitch.Api.TwitchApi.AuthApiUrl);
        }

        // HKEY_LOCAL_MACHINE\SOFTWARE\Clients\StartMenuInternet
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > minTokenLength)
            {
                Token = textBox1.Text.RemoveNonChars();
                //server.Stop();
                timer1.Enabled = false;
                timer1.Dispose();
                this.Dispose();
                if (OnTokenReceived != null)
                    OnTokenReceived(Token);
            }
        }

        private void server_ReceivedToken(string Token)
        {
            this.Invoke(new Action(() =>
            {
                textBox1.Text = Token;
                btn_Login.PerformClick();
            }));
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text.Equals("<Enter Token here>"))
            {
                textBox1.Clear();
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btn_Login.PerformClick();
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                textBox1.Text = "<Enter Token here>";
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > minTokenLength)
            {
                textBox1.Text = textBox1.Text.RemoveNonChars();
                btn_retry.Visible = false;
                btn_Login.Visible = true;
            }
            else
            {
                btn_retry.Visible = true;
                btn_Login.Visible = false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                string token = Clipboard.GetText().Trim();
                if (token.Length > minTokenLength && token.All(t => char.IsLetterOrDigit(t)))
                {
                    Clipboard.Clear();
                    textBox1.Text = token.RemoveNonChars();
                    btn_Login.PerformClick();
                }
            }
        }

        private void TokkenInput_Load(object sender, EventArgs e)
        {
            Clipboard.Clear();
        }
    }
}