using LX29_ChatClient;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace LX29_Helpers
{
    public class MPV_Downloader
    {
        public const string bit32 = "https://downloads.sourceforge.net/project/mpv-player-windows/64bit/mpv-x86_64-20170527-git-bc3365b.7z";
        public const string bit64 = "https://sourceforge.net/projects/mpv-player-windows/files/32bit/mpv-i686-20170527-git-bc3365b.7z";

        //private static bool finished = false;
        private static int lastPercent = -1;

        private static Action OnFinished;
        private static Action<int, int, string> progressAction;

        public static bool MPV_Exists
        {
            get { return File.Exists(Path.GetFullPath(Settings.pluginDir + "MPV\\mpv.exe")); }
        }

        public static void DownloadMPV(Action<int, int, string> a, Action onFinished)
        {
            try
            {
                //finished = false;
                OnFinished = onFinished;
                progressAction = a;
                progressAction.Invoke(0, 100, "Downloading MPV.7z");
                WebClient wc = new WebClient();
                wc.Proxy = null;

                wc.DownloadFileCompleted += wc_DownloadFileCompleted;
                wc.DownloadProgressChanged += wc_DownloadProgressChanged;

                wc.DownloadFileAsync(new Uri(Environment.Is64BitOperatingSystem ? bit64 : bit32),
                      Settings.pluginDir + "mpv.7z");
                //if (waitForFinish)
                //{
                //    while (!finished)
                //    {
                //        await System.Threading.Tasks.Task.Delay(100);
                //    }
                //}
                //wc.DownloadFileAsync(new Uri("https://sourceforge.net/projects/mpv-player-windows/files/latest/download?source=files"),
                //    Path.GetFullPath(Settings.pluginDir + "mpv.7z"));
            }
            catch
            {
            }
        }

        private static void wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            // Task.Run(new Action(delegate()
            //  {
            Process p = new Process();
            p.StartInfo.WorkingDirectory = Path.GetFullPath(Settings.pluginDir);
            p.StartInfo.FileName = "7za.exe";
            p.StartInfo.Arguments = "x mpv.7z -oMPV -y";
            p.Start();
            int fc = 5;
            progressAction.Invoke(fc, fc, "Extracting MPV.7z");
            p.WaitForExit(60000);
            try
            {
                Directory.Delete(Path.GetFullPath(Settings.pluginDir + "MPV\\installer"), true);
                File.Delete(Path.GetFullPath(Settings.pluginDir + "mpv.7z"));

                //Directory.CreateDirectory(Path.GetFullPath(Settings.pluginDir + "MPV\\portable_config"));
                //File.WriteAllText(Path.GetFullPath(Settings.pluginDir + "MPV\\portable_config\\input.conf"),
                //    Resources.mpv_input_config);
            }
            catch { }
            //finished = true;
            // }));
            if (OnFinished != null) OnFinished.Invoke();
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

    public class Updater
    {
        //public const string updateURL = "https://sourceforge.net/projects/twitchclient/files/Executables/LixChat.exe/download";

        public const string updateURL = "https://github.com/lukix29/LixChat/releases.atom";

        public static bool Updating
        {
            get;
            private set;
        }

        public static void CheckNightlyUpdate(Action<int, int, string> progAction, bool force = false)
        {
            try
            {
                string url = "https://github.com/lukix29/LixChat/raw/master/LixChat/Data/LixChat.exe";
                DateTime cur = Extensions.GetLinkerTime(Application.ExecutablePath);
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
                req.Proxy = null;
                string path = Path.GetFullPath(Path.GetTempFileName().Replace(".tmp", ".exe"));
                bool doUpdate = false;
                using (var resp = req.GetResponse())
                {
                    long length = resp.ContentLength;

                    using (var sr = resp.GetResponseStream())
                    {
                        byte[] buffer;
                        DateTime online = sr.GetOnlineLinkerTime(out buffer);
                        if (online.Ticks > cur.Ticks || force)
                        {
                            using (var fs = File.OpenWrite(path))
                            {
                                fs.Write(buffer, 0, buffer.Length);
                                sr.CopyTo(fs);
                            }
                            doUpdate = true;
                        }
                    }
                }
                if (doUpdate)
                {
                    if (LX29_MessageBox.Show("Update found!\r\nDownload now?", "Update", MessageBoxButtons.YesNo) == MessageBoxResult.Yes)
                    {
                        byte[] data;
                        using (WebClient wc = new WebClient())
                        {
                            wc.Proxy = null;
                            data = wc.DownloadData("https://github.com/lukix29/LixChat/raw/master/LixChat/Resources/updater.exe");
                        }
                        string updater = Path.GetTempFileName().Replace(".tmp", ".exe");
                        File.WriteAllBytes(updater, data);
                        Process.Start(updater, path + " " + Application.ExecutablePath);
                        Application.Exit();
                    }
                }
            }
            catch (Exception x)
            {
                switch (x.Handle("Nightly update Error!", true))
                {
                    case MessageBoxResult.Retry:
                        CheckNightlyUpdate(progAction);
                        break;
                }
            }
        }

        public static void CheckUpdate(Action<int, int, string> progAction, bool force = false)
        {
            if (Settings.DevUpdates)
            {
                CheckNightlyUpdate(progAction, force);
                return;
            }
            try
            {
                DateTime cur = Extensions.GetLinkerTime(Application.ExecutablePath);
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(updateURL);
                req.Proxy = null;
                string path = Path.GetFullPath(Path.GetTempPath() + "LixChat_Setup.msi");

                string line = "";
                bool doUpdate = false;
                string dlUrl = "";
                using (var resp = req.GetResponse())
                {
                    long length = resp.ContentLength;

                    using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            line = line.Trim().Replace("\"", "");
                            if (line.StartsWith("</entry>")) break;
                            if (line.StartsWith("<updated>"))
                            {
                                var tr = line.ReplaceAll(" ", "<updated>", "</updated>", "T").Replace("-", ".");
                                DateTime dt = DateTime.MinValue;
                                if (DateTime.TryParse(tr, out dt))
                                {
                                    int hrs = (int)dt.Subtract(cur).TotalHours;
                                    doUpdate = (hrs >= 12) || force;
                                    if (!doUpdate) break;
                                }
                            }
                            else if (doUpdate && line.StartsWith("<link"))
                            {
                                dlUrl = "https://github.com" + line.GetBetween("href=", "/>").Replace("tag", "download") + "/LixChat_Setup.msi";
                                progAction.Invoke(0, (int)length, "Downloading Update.");
                                break;
                            }
                        }
                    }
                }
                if (doUpdate)
                {
                    if (LX29_MessageBox.Show("Update found!\r\nDownload now?", "Update", MessageBoxButtons.YesNo) == MessageBoxResult.Yes)
                    {
                        // https://github.com/lukix29/LixChat/releases/download/1.0.7/LixChat_Setup.msi
                        using (WebClient wc = new WebClient())
                        {
                            wc.Proxy = null;
                            wc.DownloadProgressChanged += (sender, e) =>
                                {
                                    progAction.Invoke(e.ProgressPercentage, 100, "Downloading Update...");
                                };
                            wc.DownloadFileCompleted += (sender, e) =>
                            {
                                Updating = true;
                                Process.Start(path);
                                Application.Exit();
                            };
                            wc.DownloadFileAsync(new Uri(dlUrl), path);
                        }
                    }
                }
            }
            catch (Exception x)
            {
                switch (x.Handle("Update Error!", true))
                {
                    case MessageBoxResult.Retry:
                        CheckUpdate(progAction);
                        break;
                }
            }
        }

        //public static void CheckUpdate(Action<int, int, string> progAction)
        //{
        //    try
        //    {
        //        DateTime cur = Extensions.GetLinkerTime(Application.ExecutablePath);
        //        HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(updateURL);
        //        req.Proxy = null;
        //        string path = Path.GetFullPath(Path.GetTempPath() + "LixChat_temp.exe");
        //        bool doUpdate = false;
        //        using (var resp = req.GetResponse())
        //        {
        //            long length = resp.ContentLength;
        //            progAction.Invoke(0, (int)length, "Downloading Update.");

        //            using (Stream stream = resp.GetResponseStream())
        //            {
        //                byte[] buff = new byte[Extensions.BufferSize];
        //                stream.Read(buff, 0, Extensions.BufferSize);
        //                DateTime New = Extensions.GetLinkerTime(buff);

        //                if (New.Ticks > cur.Ticks)//CHange to >
        //                {
        //                    if (LX29_MessageBox.Show("Update found!\r\nDownload now?", "Update", MessageBoxButtons.YesNo) == MessageBoxResult.Yes)
        //                    {
        //                        var fileStream = File.Create(path, Extensions.BufferSize);
        //                        fileStream.Write(buff, 0, Extensions.BufferSize);
        //                        long strt = DateTime.Now.Ticks;
        //                        while ((DateTime.Now.Ticks - strt) / TimeSpan.TicksPerMinute < 5)
        //                        {
        //                            byte[] temp = new byte[Extensions.BufferSize];
        //                            int i = stream.Read(temp, 0, Extensions.BufferSize);

        //                            if (i <= 0) break;

        //                            fileStream.Write(temp, 0, i);

        //                            progAction.Invoke((int)fileStream.Length, (int)length, "Downloading Update.");
        //                        }
        //                        doUpdate = true;
        //                    }
        //                }
        //                else
        //                {
        //                    progAction.Invoke((int)length, (int)length, "No Update found.");
        //                }
        //            }
        //        }
        //        if (doUpdate)
        //        {
        //            //   File.WriteAllText(Path.GetFullPath(Settings.pluginDir + "MPV\\portable_config\\input.conf"),
        //            //Resources.mpv_input_config);
        //            Updating = true;
        //            string bat = Path.GetFullPath(Path.GetTempPath() + "update.bat");
        //            File.WriteAllText(bat, "@echo off\r\nxcopy \"" + path + "\" \"" + Application.ExecutablePath + "\" /y\r\n" +
        //               "start \"\" \"" + Application.ExecutablePath + "\"");
        //            Process.Start(bat);
        //            Application.Exit();
        //        }
        //    }
        //    catch (Exception x)
        //    {
        //        switch (x.Handle("Update Error!"))
        //        {
        //            case MessageBoxResult.Retry:
        //                CheckUpdate(progAction);
        //                break;
        //        }
        //    }
        //}
    }
}