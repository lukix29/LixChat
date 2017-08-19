using Microsoft.Win32;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace System
{
    public static class LX29_Tools
    {
        private static System.Text.RegularExpressions.Regex reg =
            new System.Text.RegularExpressions.Regex(@"^(http|https|ftp|)\://|[a-zA-Z0-9\-\.]+\.[a-zA-Z](:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$",
                                System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        public static bool HasInternetConnection
        {
            // There is no way you can reliably check if there is an internet connection, but we can come close
            get
            {
                bool result = false;

                try
                {
                    if (NetworkInterface.GetIsNetworkAvailable())
                    {
                        using (Ping p = new Ping())
                        {
                            result = (p.Send("8.8.8.8", 1000).Status == IPStatus.Success) || (p.Send("8.8.4.4", 1000).Status == IPStatus.Success) || (p.Send("4.2.2.1", 1000).Status == IPStatus.Success);
                        }
                    }
                }
                catch { }

                return result;
            }
        }

        public static List<string> StartupParameters
        {
            get
            {
                try
                {
                    List<string> startup_parameters_mixed = new List<string>();
                    startup_parameters_mixed.AddRange(Environment.GetCommandLineArgs());

                    List<string> startup_parameters_lower = new List<string>();
                    foreach (string s in startup_parameters_mixed)
                        startup_parameters_lower.Add(s.Trim().ToLower());

                    startup_parameters_mixed.Clear();

                    return startup_parameters_lower;
                }
                catch
                {
                    try { return new List<string>(Environment.GetCommandLineArgs()); }
                    catch { }
                }

                return new List<string>();
            }
        }

        public static void ExecuteIEnumerable<T>(IEnumerable<T> rest, Action<T, int> action)
        {
            int cnt = 0;
            List<Task> tasks = new List<Task>();
            foreach (var r in rest)
            {
                tasks.Add(Task.Run(() => action(r, cnt)));
                if (tasks.Count >= 4)
                {
                    int index = Task.WaitAny(tasks.ToArray());
                    tasks.RemoveAt(index);
                }
                cnt++;
            }
            if (tasks.Count > 0)
            {
                Task.WaitAll(tasks.ToArray());
                tasks.Clear();
            }
        }

        public static Dictionary<string, string> GetInstalledBrowsers()
        {
            Dictionary<string, string> browsers = new Dictionary<string, string>();
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Clients\StartMenuInternet"))
            {
                var sub = key.GetSubKeyNames();
                foreach (var s in sub)
                {
                    using (RegistryKey browser = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Clients\\StartMenuInternet\\" + s + "\\DefaultIcon"))
                    {
                        var value = browser.GetValue("").ToString();
                        value = value.Remove(value.LastIndexOf(','));

                        var file = System.IO.Path.GetFileNameWithoutExtension(s).ToLower();

                        browsers.Add(file, value);
                    }
                }
            }
            return browsers;
        }

        public static string GetSystemDefaultBrowser()
        {
            string name = string.Empty;
            RegistryKey regKey = null;

            try
            {
                regKey = Registry.ClassesRoot.OpenSubKey("HTTP\\shell\\open\\command", false);

                name = regKey.GetValue(null).ToString().ToLower().Replace("" + (char)34, "");

                if (!name.EndsWith("exe"))
                    name = name.Substring(0, name.LastIndexOf(".exe") + 4);
            }
            catch
            {
            }
            finally
            {
                if (regKey != null)
                    regKey.Close();
            }
            return name;
        }

        public static bool IsLink(string input)
        {
            return (input.Contains(".") && !input.Contains("..")
                            && reg.IsMatch(input));
        }
    }

    public class LXTimer : IDisposable
    {
        public LXTimer(Action<LXTimer> action, int dueTime, int interval)
        {
            Action = action;
            timer = new TTData();
            var t =
                 new System.Threading.Timer(
                     new TimerCallback((ob) => { Action.Invoke((LXTimer)ob); }),
                     this, dueTime, interval);
            timer.timer = t;
        }

        public Action<LXTimer> Action
        {
            get;
            private set;
        }

        private TTData timer
        {
            get;
            set;
        }

        public void Change(int dueTime, int signalTime)
        {
            timer.timer.Change(dueTime, signalTime);
        }

        public void Dispose(bool b)
        {
            if (b) Dispose();
        }

        public void Dispose()
        {
            if (timer.timer != null)
            {
                timer.timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                timer.timer.Dispose();
            }
        }

        public void Invoke()
        {
            Action.Invoke(this);
        }

        public class TTData
        {
            public System.Threading.Timer timer
            {
                get;
                set;
            }
        }
    }
}