using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using LX29_ChatClient;

namespace LX29_ChatClient
{
    public partial class FormDownloader : Form
    {
        private string DownloadFileName = "";
        private string FileName = "";
        private string Url = "";

        private WebClient wc = new WebClient();

        public FormDownloader()
        {
            InitializeComponent();
        }

        public bool FinishedDownloading
        {
            get;
            private set;
        }

        public void ShowDialog(string info, string url, string fileName)
        {
            Url = url;
            FileName = Path.GetFullPath(fileName);
            DownloadFileName = Path.GetFullPath(FileName);

            string dir = Path.GetDirectoryName(DownloadFileName);
            Directory.CreateDirectory(dir);

            lbl_Pre_Info.Text = info;

            base.ShowDialog();
        }

        public void StartDownload()
        {
            FinishedDownloading = false;
            lbl_Name.Text = "Downloading " + Path.GetFileName(FileName);

            wc.CancelAsync();
            wc.Dispose();
            wc = new WebClient();
            wc.Proxy = null;

            wc.DownloadProgressChanged += wc_DownloadProgressChanged;
            wc.DownloadFileCompleted += wc_DownloadFileCompleted;

            wc.DownloadFileAsync(new Uri(Url), DownloadFileName);
        }

        private void btn_Start_Click(object sender, EventArgs e)
        {
            progressBar1.Visible = true;
            lbl_Info.Visible = true;
            lbl_Name.Visible = true;

            btn_Abort.Visible = true;
            btn_Start.Visible = false;
            lbl_Pre_Info.Visible = false;

            StartDownload();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            wc.CancelAsync();
            wc.Dispose();
            FinishedDownloading = true;
            this.Close();
        }

        private void FormDownloader_Load(object sender, EventArgs e)
        {
        }

        private void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                {
                    e.Error.Handle(Url, true);
                }
                wc.Dispose();

                Process p = new Process();
                //p.StartInfo.WorkingDirectory = Path.GetFullPath(FileName);
                p.StartInfo.FileName = Settings._pluginDir + "7za.exe";
                p.StartInfo.Arguments = "x \"" + DownloadFileName + "\" -o\"" +
                    Path.GetDirectoryName(FileName).Replace(".7z", ".exe") + "\" -y";
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.UseShellExecute = false;
                p.Start();
                p.WaitForExit(60000);
                try
                {
                    //Directory.Delete(Path.GetFullPath(Settings._pluginDir + "MPV\\installer"), true);
                    File.Delete(DownloadFileName);
                }
                catch { }
            }
            finally
            {
                FinishedDownloading = true;
                this.Close();
            }
        }

        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            var wc = (WebClient)sender;

            progressBar1.Maximum = (int)e.TotalBytesToReceive;
            progressBar1.Value = (int)e.BytesReceived;
            lbl_Info.Text = e.ProgressPercentage + "% - " + ((int)e.BytesReceived).SizeSuffix() + "/" + ((int)e.TotalBytesToReceive).SizeSuffix();
        }
    }
}