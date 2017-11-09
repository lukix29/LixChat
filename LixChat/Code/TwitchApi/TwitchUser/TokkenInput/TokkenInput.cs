using LX29_ChatClient;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace LX29_Twitch.Api.Controls
{
    public partial class TokkenInput : UserControl
    {
        private const int minTokenLength = 28;

        private Process proc = new Process();

        private Token_HTTP_Server server = new Token_HTTP_Server(TempHtmlPath, 12685);

        private bool streamer = false;

        public TokkenInput()
        {
            InitializeComponent();
        }

        public delegate void SessionIDReceived(string SessionID, bool streamer);

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

        //public new void BringToFront()
        //{
        //    base.BringToFront();
        //}

        private void btn_Abort_Click(object sender, EventArgs e)
        {
            if (server.IsStarted)
            {
                server.Stop();
                btn_Streamer.Enabled = true;
                btn_Viewer.Enabled = true;
            }
            else
            {
                timer1.Enabled = false;
                timer1.Dispose();
                this.Visible = false;
                if (OnTokenAbort != null)
                    OnTokenAbort();
                Application.Exit();
            }
        }

        private void btn_Streamer_Click(object sender, EventArgs e)
        {
            streamer = true;
            StartAuth(LX29_Twitch.Api.TwitchApi.AuthApiUrl(streamer));
        }

        private void btn_Viewer_Click(object sender, EventArgs e)
        {
            streamer = false;
            StartAuth(LX29_Twitch.Api.TwitchApi.AuthApiUrl(streamer));
        }

        private void label1_Click(object sender, EventArgs e)
        {
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
                OnSessionIDReceived(SessionID, streamer);
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

        private void StartAuth(string url)
        {
            btn_Streamer.Enabled = false;
            btn_Viewer.Enabled = false;
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

                proc = Settings.StartBrowser(url);
                server.ReceivedToken += server_ReceivedToken;
                server.Start();
            }
        }

        private void TokkenInput_Load(object sender, EventArgs e)
        {
            Clipboard.Clear();
        }
    }
}