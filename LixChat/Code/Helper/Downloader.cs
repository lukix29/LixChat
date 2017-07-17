using LX29_ChatClient;
using LX29_TwitchChat.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace LX29_Downloader
{
    public class Downloader
    {
        public static Action OnFinished;
        public static Action<int, int, string> progressAction;
        private static int lastPercent = -1;

        public static bool MPV_Exists
        {
            get { return File.Exists(Path.GetFullPath(LX29_TwitchChat.Settings.pluginDir + "MPV\\mpv.exe")); }
        }

        public static void DownloadMPV(Action<int, int, string> a, Action onFinished)
        {
            try
            {
                OnFinished = onFinished;
                progressAction = a;
                progressAction.Invoke(0, 100, "Downloading MPV.7z");
                WebClient wc = new WebClient();
                wc.Proxy = null;
                //string html = wc.DownloadString("https://sourceforge.net/projects/mpv-player-windows/files/");

                wc.DownloadFileCompleted += wc_DownloadFileCompleted;
                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                wc.DownloadFileAsync(new Uri("https://sourceforge.net/projects/mpv-player-windows/files/latest/download?source=files"),//"https://sourceforge.net/projects/mpv-player-windows/files/64bit/mpv-x86_64-20170219-git-e69b69add.7z"),
                    Path.GetFullPath(LX29_TwitchChat.Settings.pluginDir + "mpv.7z"));
            }
            catch
            {
            }
        }

        private static void wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            // Task.Factory.StartNew(new Action(delegate()
            //  {
            Process p = new Process();
            p.StartInfo.WorkingDirectory = Path.GetFullPath(LX29_TwitchChat.Settings.pluginDir);
            p.StartInfo.FileName = "7za.exe";
            p.StartInfo.Arguments = "x mpv.7z -oMPV -y";
            p.Start();
            int fc = 5;
            progressAction.Invoke(fc, fc, "Extracting MPV.7z");
            p.WaitForExit(60000);
            try
            {
                Directory.Delete(Path.GetFullPath(LX29_TwitchChat.Settings.pluginDir + "MPV\\installer"), true);
                File.Delete(Path.GetFullPath(LX29_TwitchChat.Settings.pluginDir + "mpv.7z"));

                Directory.CreateDirectory(Path.GetFullPath(LX29_TwitchChat.Settings.pluginDir + "MPV\\portable_config"));
                File.WriteAllText(Path.GetFullPath(LX29_TwitchChat.Settings.pluginDir + "MPV\\portable_config\\input.conf"),
                    Resources.mpv_input_config);
            }
            catch { }
            // }));
            OnFinished.Invoke();
        }

        private static void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (lastPercent < e.ProgressPercentage)
            {
                progressAction.Invoke((int)e.BytesReceived, (int)e.TotalBytesToReceive, "Downloading MPV.7z");
                lastPercent = e.ProgressPercentage;
            }
        }
    }
}