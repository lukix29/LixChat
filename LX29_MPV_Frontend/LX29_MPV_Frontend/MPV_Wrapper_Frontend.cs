﻿using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace LX29_MPV
{
    //public enum MPV_ComType
    //{
    //    show_text,
    //    set_property,
    //    get_property,
    //    client_name,
    //    get_time_us,
    //    drop_buffers,

    //    //observe_property,
    //    //unobserve_property,
    //    //request_log_messages,
    //    //enable_event,
    //    //disable_event,
    //    get_version
    //}

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
        audio_codec_name,
        drop_buffers
    }

    public class MpvLib : IDisposable
    {
        public IntPtr _mpvHandle;
        private const int MpvFormatString = 1;
        private IntPtr _libMpvDll;
        private MpvCommand _mpvCommand;
        private MpvCreate _mpvCreate;
        private MpvFree _mpvFree;
        private MpvGetPropertystring _mpvGetPropertyString;
        private MpvInitialize _mpvInitialize;
        private MpvSetOption _mpvSetOption;
        private MpvSetOptionString _mpvSetOptionString;
        private MpvSetProperty _mpvSetProperty;
        private MpvTerminateDestroy _mpvTerminateDestroy;

        //private IntPtr MainWindowHandle;
        //private Process process = null;

        public MpvLib(IntPtr handle)
        {
            if (_mpvHandle != IntPtr.Zero)
                _mpvTerminateDestroy(_mpvHandle);

            LoadMpvDynamic();
            if (_libMpvDll == IntPtr.Zero)
                return;

            _mpvHandle = _mpvCreate.Invoke();
            if (_mpvHandle == IntPtr.Zero)
                return;

            _mpvInitialize.Invoke(_mpvHandle);
            _mpvSetOptionString(_mpvHandle, GetUtf8Bytes("keep-open"), GetUtf8Bytes("no"));
            _mpvSetOptionString(_mpvHandle, GetUtf8Bytes("af"), GetUtf8Bytes("format=channels=2.0"));

            int format = 4;
            var windowId = handle.ToInt64();
            _mpvSetOption(_mpvHandle, GetUtf8Bytes("wid"), format, ref windowId);
            //socketName = "mpv_" + name;// +"_" + rdName(8);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int MpvCommand(IntPtr mpvHandle, IntPtr strings);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr MpvCreate();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void MpvFree(IntPtr data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int MpvGetPropertystring(IntPtr mpvHandle, byte[] name, int format, ref IntPtr data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int MpvInitialize(IntPtr mpvHandle);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int MpvSetOption(IntPtr mpvHandle, byte[] name, int format, ref long data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int MpvSetOptionString(IntPtr mpvHandle, byte[] name, byte[] value);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int MpvSetProperty(IntPtr mpvHandle, byte[] name, int format, ref byte[] data);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int MpvTerminateDestroy(IntPtr mpvHandle);

        public bool IsRunning
        {
            get
            {
                if (_mpvHandle != IntPtr.Zero)
                {
                    var prop = GetProperty(MPV_Property.volume);
                    return prop != null;
                }
                return false;
            }
        }

        //public System.Drawing.Rectangle Position
        //{
        //    get
        //    {
        //        if (IsRunning && MainWindowHandle != IntPtr.Zero)
        //        {
        //            return NativeMethods.GetWindowRect(MainWindowHandle);
        //        }
        //        return System.Drawing.Rectangle.Empty;
        //    }
        //}

        public void Dispose()
        {
            Dispose(true);
        }

        public bool Dispose(bool dispose)
        {
            if (dispose)
            {
                try
                {
                    if (_mpvHandle != IntPtr.Zero)
                    {
                        _mpvTerminateDestroy(_mpvHandle);
                    }
                }
                finally
                {
                    _mpvHandle = IntPtr.Zero;
                }
            }
            return true;
        }

        public string GetProperty(MPV_Property property)
        {
            if (_mpvHandle == IntPtr.Zero)
                return null;

            var lpBuffer = IntPtr.Zero;
            var name = Enum.GetName(typeof(MPV_Property), property).Replace("_", "-");
            _mpvGetPropertyString(_mpvHandle, GetUtf8Bytes(name), MpvFormatString, ref lpBuffer);
            var value = Marshal.PtrToStringAnsi(lpBuffer);
            _mpvFree(lpBuffer);
            return value;
        }

        public void Pause(bool enable)
        {
            SetProperty(MPV_Property.pause, enable);
        }

        public void SetProperty(MPV_Property property, object value)
        {
            if (_mpvHandle == IntPtr.Zero)
                return;

            string val = value.ToString();
            if (value is bool)
            {
                val = ((bool)value) ? "yes" : "no";
            }

            var name = Enum.GetName(typeof(MPV_Property), property).Replace("_", "-");

            var buff = GetUtf8Bytes(val);
            _mpvSetProperty(_mpvHandle, GetUtf8Bytes(name), MpvFormatString, ref buff);
        }

        public void SetVolume(int volume)
        {
            SetProperty(MPV_Property.volume, Math.Max(0, Math.Min(120, volume)));
        }

        public bool Start(string fileName, int volume = 100, int cacheSecs = 10, int cache = 32000)
        {
            try
            {
                _start(fileName);
                SetVolume(volume);
                return true;
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        Start(fileName, volume, cacheSecs, cache);
                        break;
                }
            }
            return false;
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi, BestFitMapping = false)]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi, BestFitMapping = false)]
        internal static extern IntPtr LoadLibrary(string dllToLoad);

        private static IntPtr AllocateUtf8IntPtrArrayWithSentinel(string[] arr, out IntPtr[] byteArrayPointers)
        {
            int numberOfStrings = arr.Length + 1; // add extra element for extra null pointer last (sentinel)
            byteArrayPointers = new IntPtr[numberOfStrings];
            IntPtr rootPointer = Marshal.AllocCoTaskMem(IntPtr.Size * numberOfStrings);
            for (int index = 0; index < arr.Length; index++)
            {
                var bytes = GetUtf8Bytes(arr[index]);
                IntPtr unmanagedPointer = Marshal.AllocHGlobal(bytes.Length);
                Marshal.Copy(bytes, 0, unmanagedPointer, bytes.Length);
                byteArrayPointers[index] = unmanagedPointer;
            }
            Marshal.Copy(byteArrayPointers, 0, rootPointer, numberOfStrings);
            return rootPointer;
        }

        private static byte[] GetUtf8Bytes(string s)
        {
            return Encoding.UTF8.GetBytes(s + "\0");
        }

        private void _start(string url)
        {
            DoMpvCommand("loadfile", url);
        }

        //if (HasStarted)
        //{
        //    //{ "command": ["command_name", "param1", "param2", ...] }
        //    var com = "{ \"command\": [\"stop\"] }";
        //    SendRaw(com);
        //    return true;
        //}
        //Stop();

        //string intPtr = " ";
        //if (handle != IntPtr.Zero)
        //{
        //    intPtr = " --wid=" + handle.ToString() + " ";
        //}
        //string cash = "";
        //if (cache > 0)
        //{
        //    cash = " --cache-initial=" + cache +
        //            " --cache-backbuffer=" + cache +
        //            " --cache-default=" + cache +
        //            " --demuxer-readahead-secs=" + cacheSecs;
        //}
        //string geom = "";
        //if (!rect.IsEmpty)
        //{
        //    Screen sc = Screen.FromRectangle(rect);

        //    if (rect.X < sc.Bounds.X + 10)
        //    {
        //        rect.X = sc.Bounds.X + 10;
        //    }
        //    if (rect.Right > sc.Bounds.Right - 20)
        //    {
        //        rect.Width = sc.Bounds.Width - 20;
        //    }
        //    if (rect.Y < sc.Bounds.Y + 10)
        //    {
        //        rect.Y = sc.Bounds.Y + 10;
        //    }
        //    if (rect.Bottom > sc.Bounds.Bottom - 20)
        //    {
        //        rect.Height = sc.Bounds.Height - 20;
        //    }
        //    geom = " --geometry=" + rect.Width + "x" + rect.Height +
        //        ((rect.X < 0) ? "-" : "+") + Math.Abs(rect.X) +
        //        ((rect.Y < 0) ? "-" : "+") + Math.Abs(rect.Y);
        //}
        //pipe = new NamedPipeClientStream(".", socketName,
        //      PipeDirection.InOut, PipeOptions.Asynchronous,
        //      TokenImpersonationLevel.Anonymous);

        //process = new Process();
        private void DoMpvCommand(params string[] args)
        {
            IntPtr[] byteArrayPointers;
            var mainPtr = AllocateUtf8IntPtrArrayWithSentinel(args, out byteArrayPointers);
            _mpvCommand(_mpvHandle, mainPtr);
            foreach (var ptr in byteArrayPointers)
            {
                Marshal.FreeHGlobal(ptr);
            }
            Marshal.FreeHGlobal(mainPtr);
        }

        private object GetDllType(Type type, string name)
        {
            IntPtr address = GetProcAddress(_libMpvDll, name);
            if (address != IntPtr.Zero)
                return Marshal.GetDelegateForFunctionPointer(address, type);
            return null;
        }

        private void LoadMpvDynamic()
        {
            _libMpvDll = LoadLibrary("mpv-1.dll");
            _mpvCreate = (MpvCreate)GetDllType(typeof(MpvCreate), "mpv_create");
            _mpvInitialize = (MpvInitialize)GetDllType(typeof(MpvInitialize), "mpv_initialize");
            _mpvTerminateDestroy = (MpvTerminateDestroy)GetDllType(typeof(MpvTerminateDestroy), "mpv_terminate_destroy");
            _mpvCommand = (MpvCommand)GetDllType(typeof(MpvCommand), "mpv_command");
            _mpvSetOption = (MpvSetOption)GetDllType(typeof(MpvSetOption), "mpv_set_option");
            _mpvSetOptionString = (MpvSetOptionString)GetDllType(typeof(MpvSetOptionString), "mpv_set_option_string");
            _mpvGetPropertyString = (MpvGetPropertystring)GetDllType(typeof(MpvGetPropertystring), "mpv_get_property");
            _mpvSetProperty = (MpvSetProperty)GetDllType(typeof(MpvSetProperty), "mpv_set_property");
            _mpvFree = (MpvFree)GetDllType(typeof(MpvFree), "mpv_free");
        }
    }
}