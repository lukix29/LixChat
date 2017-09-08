using LX29_ChatClient;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;

namespace LX29_Helpers
{
    public enum MPV_ComType
    {
        show_text,
        set_property,
        get_property,
        client_name,
        get_time_us,

        //observe_property,
        //unobserve_property,
        //request_log_messages,
        //enable_event,
        //disable_event,
        get_version
    }

    public enum MPV_Property
    {
        none,
        pause,
        volume,
        mute,
        cache,
        cache_size,
        audio_speed_correction,
        video_speed_correction,
        display_sync_active,
        filename,
        file_size,
        estimated_frame_count,
        estimated_frame_number,
        path,
        media_title,
        file_format,
        current_demuxer,
        stream_path,
        stream_pos,
        stream_end,
        duration,
        avsync,
        total_avsync_change,
        drop_frame_count,
        vo_drop_frame_count,
        mistimed_frame_count,
        vsync_ratio,
        vo_delayed_frame_count,
        percent_pos,
        time_pos,
        time_remaining,
        audio_pts,
        playtime_remaining,
        playback_time,
        chapter,
        filtered_metadata,
        cache_free,
        cache_used,
        cache_speed,
        cache_idle,
        demuxer_cache_duration,
        demuxer_cache_time,
        demuxer_cache_idle,
        demuxer_via_network,
        paused_for_cache,
        cache_buffering_state,
        eof_reached,
        seeking,
        mixer_active,
        ao_volume,
        ao_mute,
        video_bitrate,
        audio_bitrate,
        sub_bitrate,
        packet_video_bitrate,
        packet_audio_bitrate,
        packet_sub_bitrate,
        audio_codec,
        audio_codec_name
    }

    public class MPV_Wrapper : IDisposable
    {
        public const string WindowIdentifier = "-Stream @ LixChat";
        public static readonly string MPVinputConfig = Settings.pluginDir + "\\MPV\\portable_config\\input.config";

        public static readonly string MPVPATH = Settings.pluginDir + "\\MPV\\mpv.exe";
        private static int leftBH = 0;

        private static int topBH = 0;

        private readonly string socketName = "";

        private NamedPipeClientStream pipe = null;

        private Process process = null;

        //private string rdName(int length)
        //{
        //    Random rd = new Random();
        //    return rd.NextName(length);
        //}
        public MPV_Wrapper(string name)
        {
            socketName = "mpv_" + name;// +"_" + rdName(8);
        }

        public bool HasExited
        {
            get { return !IsRunning; }
        }

        public bool HasStarted
        {
            get;
            private set;
        }

        public int Id
        {
            get { return process.Id; }
        }

        public bool IsRunning
        {
            get
            {
                try { return (process != null && !process.HasExited); }
                catch { return false; }
            }
        }

        public IntPtr MainWindowHandle
        {
            get { return process.MainWindowHandle; }
        }

        public System.Drawing.Rectangle Position
        {
            get
            {
                if (!HasExited)
                {
                    return NativeMethods.GetWindowRect(process.MainWindowHandle);
                }
                return System.Drawing.Rectangle.Empty;
            }
        }

        public string SocketName
        {
            get { return socketName; }
        }

        public static void SetBorderSize(System.Drawing.Size souter, System.Drawing.Size sinner)
        {
            leftBH = ((souter.Width - sinner.Width) / 2);
            topBH = souter.Height - sinner.Height - leftBH;
        }

        public static bool StartAlone(string Title, string fileName, System.Drawing.Rectangle rect = new System.Drawing.Rectangle())
        {
            return StartAlone(Title, fileName, 100, 0, IntPtr.Zero, 10, rect);
        }

        public static bool StartAlone(string Title, string fileName, int volume = 100, int cache = 0, IntPtr handle = new IntPtr(), int cacheSecs = 10, System.Drawing.Rectangle rect = new System.Drawing.Rectangle())
        {
            //string serr = "";
            //string sout = "";
            try
            {
                string intPtr = " ";
                if (handle != IntPtr.Zero)
                {
                    intPtr = " --wid=" + handle.ToString() + " ";
                }
                string cash = "";
                if (cache > 0)
                {
                    cash = " --cache-initial=" + cache +
                            " --cache-backbuffer=" + cache +
                            " --cache=" + cache +
                            " --cache-secs=" + cacheSecs; //
                }
                string geom = "";
                if (!rect.IsEmpty)
                {
                    Screen sc = Screen.FromRectangle(rect);
                    if (rect.Y < sc.Bounds.Y)
                    {
                        rect.Y = sc.Bounds.Y + topBH;
                    }
                    if (rect.X < sc.Bounds.X)
                    {
                        rect.X = sc.Bounds.X + leftBH;
                    }
                    geom = " --geometry=" + rect.X + ":" + rect.Y;
                    //geom = " --geometry=" + rect.Width + "x" + rect.Height +
                    //    ((rect.X < 0) ? "-" : "+") + Math.Abs(rect.X) +
                    //    ((rect.Y < 0) ? "-" : "+") + Math.Abs(rect.Y);
                }

                Process process = new Process();
                //process.StartInfo.RedirectStandardError = true;
                //process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WorkingDirectory = Settings.pluginDir + "\\MPV";
                process.StartInfo.FileName = MPVPATH;
                process.StartInfo.Arguments = (" --osc=yes --no-ytdl --no-taskbar-progress --af=format=channels=2.0"
                    + geom + " --title=\"" + Title + "\"" +
                    cash + " --hls-bitrate=max" +
                    " --volume=" + Math.Max(0, Math.Min(100, volume)) +
                    intPtr + fileName);
                process.Start();

                bool HasStarted = false;
                DateTime timeOut = DateTime.Now;
                while (true)
                {
                    try
                    {
                        //if (StreamManager.IsClosing)
                        //{
                        //    process.Kill();
                        //    break;
                        //}
                        System.Threading.Thread.Sleep(100);
                        if (DateTime.Now.Subtract(timeOut).TotalSeconds > 10 ||
                            ((handle == IntPtr.Zero) ? process.MainWindowHandle : process.Handle) != IntPtr.Zero)
                        {
                            if (!process.HasExited)
                            {
                                HasStarted = true;
                            }
                            break;
                        }
                        if (process.HasExited)
                        {
                            break;
                        }
                    }
                    catch { break; }
                }
                return HasStarted;
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        return StartAlone(Title, fileName, volume, cache, handle, cacheSecs, rect);
                }
            }
            return false;
        }

        public void Dispose()
        {
            bool b = this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool Dispose(bool dispose)
        {
            try
            {
                process.Close();
                pipe.Dispose();
                return dispose;
            }
            catch
            {
            }
            return false;
        }

        public object GetProperty(MPV_Property name)
        {
            string s = SendCommand(MPV_ComType.get_property, name, null);
            if (s.Length > 0)
            {
                if (s.StartsWith("{\"data\":\""))
                {   //"{"data":"index-live","error":"success"}"
                    return s.GetBetween(":\"", "\",");
                }
                else
                {   //"{"data":100.000000,"error":"success"}"
                    string[] line = s.Split(new char[] { ':', ',', '\"', '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
                    if (line.Length == 4)
                    {
                        if (line[3].Contains("success"))
                        {
                            return _parse(line[1]);
                        }
                    }
                    else if (line.Length == 2)
                    {
                        return _parse(line[1]);
                    }
                }
            }
            return null;
        }

        public void Pause(bool enable)
        {
            SetProperty(MPV_Property.pause, enable);
        }

        public string ReadAll()
        {
            return _read('\0');
        }

        public string[] ReadAllLines()
        {
            return _read('\0').Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public string ReadLine()
        {
            return _read('\n');
        }

        public string ResetCache()
        {
            var command = "{ \"command\": [\"drop-buffers\"] }";
            SendRaw(command);
            return ReadLine();
        }

        public string SendCommand(MPV_ComType type, MPV_Property name = MPV_Property.none, object value = null)
        {
            //https://mpv.io/manual/master/#list-of-input-commands
            string command = "";
            if (type == MPV_ComType.show_text)
            {
                command = "show-text ${" + name.ToString().ToLower() + "}";
                SendRaw(command);
                return "";
            }
            else
            {
                string val = "";
                string nme = "";

                if (name != MPV_Property.none)
                    nme = ", \"" + name.ToString().ToLower().Replace("_", "-") + "\"";

                if (value != null)
                    val = ", " + value.ToString();

                string typ = "\"" + type.ToString().ToLower() + "\"";

                command = "{ \"command\": [" + typ + nme + val + "] }";
                SendRaw(command);
                return ReadLine();
            }
        }

        public void SetProcess(Process p)
        {
            try
            {
                process = p;
                if (pipe != null && pipe.IsConnected) pipe.Close();
                pipe = new NamedPipeClientStream(
                ".", socketName,
                    PipeDirection.InOut, PipeOptions.Asynchronous,
                    TokenImpersonationLevel.Anonymous);
                pipe.Connect(1000);
                HasStarted = true;
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        SetProcess(p);
                        break;

                    case MessageBoxResult.Abort:
                        Dispose();
                        return;
                }
            }
        }

        public string SetProperty(MPV_Property name, object value)
        {
            return SendCommand(MPV_ComType.set_property, name, value);
        }

        public void SetVolume(int volume)
        {
            SetProperty(MPV_Property.volume, Math.Max(0, Math.Min(100, volume)));
        }

        public void SetVolume(bool mute)
        {
            SetProperty(MPV_Property.volume, (mute ? 0 : 100));
        }

        public bool Start(string Title, string FileName)
        {
            return Start(Title, FileName, 100, 0, IntPtr.Zero);
        }

        public bool Start(string fileName)
        {
            return Start(fileName, 0, IntPtr.Zero);
        }

        public bool Start(string fileName, int volume)
        {
            return Start(fileName, volume, 0, IntPtr.Zero);
        }

        public bool Start(string fileName, int cache, IntPtr handle)
        {
            return Start(fileName, 100, cache, handle);
        }

        public bool Start(string Title, string fileName, int cache, int cacheSecs, System.Drawing.Rectangle rect)
        {
            return Start(Title, fileName, 100, cache, IntPtr.Zero, cacheSecs, rect);
        }

        public bool Start(string Title, string fileName, int volume, int cache, IntPtr handle, int cacheSecs = 10, System.Drawing.Rectangle rect = new System.Drawing.Rectangle())
        {
            //string serr = "";
            //string sout = "";
            try
            {
                //if (HasStarted)
                //{
                //    //{ "command": ["command_name", "param1", "param2", ...] }
                //    var com = "{ \"command\": [\"stop\"] }";
                //    SendRaw(com);
                //    return true;
                //}
                Stop();
                HasStarted = false;

                string intPtr = " ";
                if (handle != IntPtr.Zero)
                {
                    intPtr = " --wid=" + handle.ToString() + " ";
                }
                string cash = "";
                if (cache > 0)
                {
                    cash = " --cache-initial=" + cache +
                            " --cache-backbuffer=" + cache +
                            " --cache-default=" + cache +
                            " --demuxer-readahead-secs=" + cacheSecs;
                }
                string geom = "";
                if (!rect.IsEmpty)
                {
                    Screen sc = Screen.FromRectangle(rect);

                    if (rect.X < sc.Bounds.X + 10)
                    {
                        rect.X = sc.Bounds.X + 10;
                    }
                    if (rect.Right > sc.Bounds.Right - 20)
                    {
                        rect.Width = sc.Bounds.Width - 20;
                    }
                    if (rect.Y < sc.Bounds.Y + 10)
                    {
                        rect.Y = sc.Bounds.Y + 10;
                    }
                    if (rect.Bottom > sc.Bounds.Bottom - 20)
                    {
                        rect.Height = sc.Bounds.Height - 20;
                    }
                    geom = " --geometry=" + rect.Width + "x" + rect.Height +
                        ((rect.X < 0) ? "-" : "+") + Math.Abs(rect.X) +
                        ((rect.Y < 0) ? "-" : "+") + Math.Abs(rect.Y);
                }
                pipe = new NamedPipeClientStream(".", socketName,
                      PipeDirection.InOut, PipeOptions.Asynchronous,
                      TokenImpersonationLevel.Anonymous);

                process = new Process();
                //process.StartInfo.RedirectStandardError = true;
                //process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WorkingDirectory = Settings.pluginDir + "\\MPV";
                process.StartInfo.FileName = MPVPATH;
                process.StartInfo.Arguments = (@"--input-ipc-server=\\.\pipe\" + socketName +
                    " --osc=yes --no-ytdl --af=format=channels=2.0" + geom +//--no-osc
                    " --title=\"" + Title + WindowIdentifier + "\"" +
                    cash + " --hls-bitrate=max --network-timeout=10"//--loop-playlist=inf  --stop-playback-on-init-failure=no" +
                    + " --volume=" + Math.Max(0, Math.Min(100, volume)) +
                    intPtr + fileName);
                process.Start();

                DateTime timeOut = DateTime.Now;
                while (true)
                {
                    try
                    {
                        //if (StreamManager.IsClosing)
                        //{
                        //    process.Kill();
                        //    break;
                        //}
                        System.Threading.Thread.Sleep(100);
                        if (DateTime.Now.Subtract(timeOut).TotalSeconds > 10 ||
                            ((handle == IntPtr.Zero) ? process.MainWindowHandle : process.Handle) != IntPtr.Zero)
                        {
                            if (!process.HasExited)
                            {
                                HasStarted = true;
                            }
                            break;
                        }
                        if (process.HasExited)
                        {
                            break;
                        }
                    }
                    catch { break; }
                }
                if (HasStarted)
                {
                    try
                    {
                        pipe.Connect(1000);
                    }
                    catch (Exception x)
                    {
                        switch (x.Handle())
                        {
                            case MessageBoxResult.Retry:
                                Start(Title, fileName, volume, cache, handle, cacheSecs, rect);
                                break;

                            case MessageBoxResult.Abort:
                                Dispose();
                                process.Kill();
                                HasStarted = false;
                                break;
                        }
                    }
                }
                else
                {
                    //serr = process.StandardError.ReadToEnd();
                    //sout = process.StandardOutput.ReadToEnd();
                }

                return HasStarted;
            }
            catch (Exception x)
            {
                Stop();
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        Start(Title, fileName, volume, cache, handle, cacheSecs, rect);
                        break;
                }
            }
            return false;
        }

        public bool Start(string fileName, int volume, int cache, IntPtr handle)
        {
            return Start(Path.GetFileNameWithoutExtension(fileName), fileName, volume, cache, handle);
        }

        public System.Threading.Tasks.Task StartAsync(string fileName, int cache, IntPtr handle)
        {
            return System.Threading.Tasks.Task.Run(() => Start(fileName, 100, cache, handle));
        }

        public bool Stop()
        {
            try
            {
                pipe.Close();
                if (!process.HasExited)
                {
                    process.Kill();
                    while (!process.HasExited) ;
                }
                HasStarted = false;
                return true;
            }
            catch { }
            return false;
        }

        private object _parse(string s)
        {
            if (s.Equals("property unavailable"))
                return null;
            bool b = false;
            float d = 0;
            if (float.TryParse(s.Replace(".", ","), out d))
            {
                return d;
            }
            else if (bool.TryParse(s, out b))
            {
                return b;
            }
            return s;
        }

        private string _read(char until)
        {
            byte[] ba = new byte[pipe.InBufferSize];
            string sout = "";
            AsyncCallback callback = ar =>
            {
                int received = pipe.EndRead(ar);
                if (received > 0)
                {
                    sout = Encoding.UTF8.GetString(ba).Split(until)[0];
                }
            };
            if (sout.Length == 0) pipe.BeginRead(ba, 0, ba.Length, callback, null);
            DateTime dt = DateTime.Now;
            while (sout.Length == 0)
            {
                if (DateTime.Now.Subtract(dt).TotalSeconds > 5)
                {
                    break;
                }
            }
            return sout;
        }

        private void SendRaw(string s)
        {
            if (pipe.IsConnected)
            {
                byte[] ba = Encoding.UTF8.GetBytes(s.ToLower() + "\n");
                pipe.Write(ba, 0, ba.Length);
            }
        }
    }
}