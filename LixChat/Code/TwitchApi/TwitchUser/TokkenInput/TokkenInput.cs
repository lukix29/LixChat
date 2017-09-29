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

        private Token_HTTP_Server server = new Token_HTTP_Server(TempHtmlPath, 12685);

        public TokkenInput()
        {
            InitializeComponent();
        }

        public delegate void SessionIDReceived(string SessionID);

        public delegate void TokenAbort();

        public event SessionIDReceived OnSessionIDReceived;

        public event TokenAbort OnTokenAbort;

        public string SessionID
        {
            get;
            private set;
        }

        private static string TempHtmlPath
        {
            get { return System.IO.Path.GetTempPath() + "lixchat\\"; }
        }

        public new void BringToFront()
        {
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
                System.IO.File.WriteAllText(TempHtmlPath + "success.html", LX29_LixChat.Properties.Resources.success);

                proc = Settings.StartBrowser(LX29_Twitch.Api.TwitchApi.AuthApiUrl);
                server.ReceivedToken += server_ReceivedToken;
                server.Start();
            }
            base.BringToFront();
        }

        private void btn_Abort_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            timer1.Dispose();
            this.Visible = false;
            if (OnTokenAbort != null)
                OnTokenAbort();
            Application.Exit();
        }

        private void btn_retry_Click(object sender, EventArgs e)
        {
            server.Stop();
            server.Start();
            proc.Close();
            proc = Settings.StartBrowser(LX29_Twitch.Api.TwitchApi.AuthApiUrl);
        }

        // HKEY_LOCAL_MACHINE\SOFTWARE\Clients\StartMenuInternet
        private void login(string sessionID)
        {
            SessionID = sessionID;
            //server.Stop();
            timer1.Enabled = false;
            timer1.Dispose();
            this.Dispose();
            if (OnSessionIDReceived != null)
                OnSessionIDReceived(SessionID);
        }

        private void server_ReceivedToken(string sessionID)
        {
            this.Invoke(new Action(() =>
            {
                login(sessionID);
                btn_Login.PerformClick();
            }));
            server.ReceivedToken -= server_ReceivedToken;
        }

        private void TokkenInput_Load(object sender, EventArgs e)
        {
            Clipboard.Clear();
        }
    }
}