using LX29_Twitch.JSON_Parser;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace LX29_Twitch.Api
{
    public enum ApiInfo
    {
        None = 0,
        _id,
        follow,
        average_fps,
        background,
        banner,
        broadcaster_language,
        created_at,
        delay,
        display_name,
        followers,
        game,
        is_playlist,
        language,
        large,
        logo,
        mature,
        name,
        description,
        partner,
        profile_banner,
        profile_banner_background_color,
        status,
        stream_created_at,
        follow_created_at,
        stream_id,
        updated_at,
        stream_type,
        url,
        video_banner,
        video_height,
        viewers,
        views,
        sub_plan,
        sub_plan_name
    }

    public enum StreamType
    {
        None = 0,
        live = 2,
        watch_party = 1
    }

    //public enum FFZEmoteInfo
    //{
    //    css,
    //    height,
    //    hidden,
    //    id,
    //    margins,
    //    name,
    //    owner,
    //    urls,
    //    width,
    //}

    public enum SubType
    {
        NoSubProgram = -2000,
        NoSub = -1000,
        Prime = 0000,
        Tier1 = 1000,
        Tier2 = 2000,
        Tier3 = 3000
    }

    //public class ApiResult
    //{
    //    public static readonly ApiResult Empty = new ApiResult(new Dictionary<ApiInfo, object>());

    //    private static readonly ApiInfo[] apiInfoSort = new ApiInfo[]{
    //        ApiInfo.display_name ,ApiInfo.url, ApiInfo.game ,
    //        ApiInfo.status , ApiInfo.viewers ,ApiInfo.follow_created_at, ApiInfo.followers,
    //        ApiInfo.video_height , ApiInfo.average_fps, ApiInfo.views};

    //    private Dictionary<ApiInfo, object> values;

    //    public ApiResult(ApiResult result, ApiResult result1)
    //    {
    //        values = result.values;

    //        foreach (var t in result1.values)
    //        {
    //            if (!values.ContainsKey(t.Key))
    //            {
    //                values.Add(t.Key, t.Value);
    //            }
    //        }
    //    }

    //    public ApiResult(Dictionary<ApiInfo, string> ChannelValues, Dictionary<ApiInfo, string> StreamValues = null)
    //    {
    //        //EmoteSets = emoteSets;
    //        this.values = new Dictionary<ApiInfo, object>();

    //        if (ChannelValues != null && ChannelValues.Count > 0)
    //        {
    //            foreach (var channel in ChannelValues)
    //            {
    //                var o = ConvertValue(channel.Value, channel.Key);
    //                values.Add(channel.Key, o);
    //            }
    //        }

    //        if (StreamValues != null && StreamValues.Count > 0)
    //        {
    //            foreach (var stream in StreamValues)
    //            {
    //                if (stream.Key == ApiInfo._id)
    //                {
    //                    if (!values.ContainsKey(ApiInfo.stream_id))
    //                    {
    //                        var o = ConvertValue(stream.Value, ApiInfo.stream_id);
    //                        values.Add(ApiInfo.stream_id, o);
    //                    }
    //                }
    //                else if (stream.Key == ApiInfo.created_at)
    //                {
    //                    if (!values.ContainsKey(ApiInfo.stream_created_at))
    //                    {
    //                        var o = ConvertValue(stream.Value, ApiInfo.stream_created_at);
    //                        values.Add(ApiInfo.stream_created_at, o);
    //                    }
    //                }
    //                else if (stream.Key == ApiInfo.large)
    //                {
    //                    if (!values.ContainsKey(ApiInfo.large))
    //                    {
    //                        var o = ConvertValue(stream.Value, ApiInfo.large);
    //                        values.Add(ApiInfo.large, o);
    //                    }
    //                }
    //                else if (!values.ContainsKey(stream.Key))
    //                {
    //                    var o = ConvertValue(stream.Value, stream.Key);
    //                    values.Add(stream.Key, o);
    //                }
    //            }
    //        }
    //    }

    //    public ApiResult(Dictionary<ApiInfo, object> ChannelValues, Dictionary<ApiInfo, object> StreamValues = null)
    //    {
    //        //EmoteSets = emoteSets;
    //        this.values = new Dictionary<ApiInfo, object>();

    //        if (ChannelValues != null && ChannelValues.Count > 0)
    //        {
    //            foreach (var channel in ChannelValues)
    //            {
    //                values.Add(channel.Key, channel.Value);
    //            }
    //        }

    //        if (StreamValues != null && StreamValues.Count > 0)
    //        {
    //            foreach (var stream in StreamValues)
    //            {
    //                if (stream.Key == ApiInfo._id)
    //                {
    //                    if (!values.ContainsKey(ApiInfo.stream_id))
    //                    {
    //                        values.Add(ApiInfo.stream_id, stream.Value);
    //                    }
    //                }
    //                else if (stream.Key == ApiInfo.created_at)
    //                {
    //                    if (!values.ContainsKey(ApiInfo.stream_created_at))
    //                    {
    //                        values.Add(ApiInfo.stream_created_at, stream.Value);
    //                    }
    //                }
    //                else if (stream.Key == ApiInfo.large)
    //                {
    //                    if (!values.ContainsKey(ApiInfo.large))
    //                    {
    //                        values.Add(ApiInfo.large, stream.Value);
    //                    }
    //                }
    //                else if (!values.ContainsKey(stream.Key))
    //                {
    //                    values.Add(stream.Key, stream.Value);
    //                }
    //            }
    //        }
    //    }

    //    public bool Followed
    //    {
    //        get { return GetValue<bool>(ApiInfo.follow); }
    //    }

    //    public string ID
    //    {
    //        get { return GetValue<string>(ApiInfo._id); }
    //    }

    //    public string Infos
    //    {
    //        get
    //        {
    //            StringBuilder sb = new StringBuilder();

    //            var names = Enum.GetNames(typeof(ApiInfo)).ToList();
    //            foreach (var ai in apiInfoSort)
    //            {
    //                string name = Enum.GetName(typeof(ApiInfo), ai);
    //                if (values.ContainsKey(ai))
    //                {
    //                    string s = values[ai].ToString();
    //                    if (s.Length > 0)
    //                    {
    //                        sb.Append(name.Replace("_", " ").ToTitleCase() + ": ");

    //                        sb.AppendLine(s);
    //                        sb.AppendLine();
    //                    }
    //                }
    //                names.Remove(name);
    //            }
    //            foreach (var Key in names)
    //            {
    //                ApiInfo info = (ApiInfo)Enum.Parse(typeof(ApiInfo), Key);
    //                if (values.ContainsKey(info))
    //                {
    //                    string s = values[info].ToString();
    //                    if (s.Length > 0)
    //                    {
    //                        sb.Append(Key.Replace("_", " ").ToTitleCase() + ": ");

    //                        sb.AppendLine(s);
    //                        sb.AppendLine();
    //                    }
    //                }
    //            }
    //            return sb.ToString();
    //        }
    //    }

    //    public bool IsEmpty
    //    {
    //        get { return values.Count == 0; }
    //    }

    //    public bool IsOnline
    //    {
    //        get { return values.ContainsKey(ApiInfo.stream_id); }
    //    }

    //    public string Name
    //    {
    //        get { return GetValue<string>(ApiInfo.name); }
    //    }

    //    public TimeSpan OnlineTime
    //    {
    //        get
    //        {
    //            DateTime dt = (IsOnline) ? GetValue<DateTime>(ApiInfo.stream_created_at) : GetValue<DateTime>(ApiInfo.created_at);
    //            return DateTime.Now.Subtract(dt);
    //        }
    //    }

    //    public string OnlineTimeString
    //    {
    //        get
    //        {
    //            return ((IsOnline) ? "Online: " : "Created: ") +
    //                OnlineTime.ToString((OnlineTime.TotalDays >= 1.0) ? @"%d'd 'hh':'mm':'ss" : @"hh\:mm\:ss");
    //        }
    //    }

    //    public Dictionary<ApiInfo, object> Values
    //    {
    //        get { return values; }
    //    }

    //    public T GetValue<T>(ApiInfo type)
    //    {
    //        if (values.ContainsKey(type))
    //        {
    //            return (T)values[type];
    //        }
    //        return default(T);
    //    }

    //    public void ResetStreamStatus()
    //    {
    //        if (values.ContainsKey(ApiInfo.stream_id))
    //            values.Remove(ApiInfo.stream_id);
    //        if (values.ContainsKey(ApiInfo.stream_created_at))
    //            values.Remove(ApiInfo.stream_created_at);
    //        if (values.ContainsKey(ApiInfo.large))
    //            values.Remove(ApiInfo.large);
    //    }

    //    private object ConvertValue(string si, ApiInfo type)
    //    {
    //        if (si.Length == 0) return si;
    //        bool b = false;
    //        if (type == ApiInfo.created_at || type == ApiInfo.updated_at
    //            || type == ApiInfo.stream_created_at || type == ApiInfo.follow_created_at)
    //        {
    //            string s = si.Replace("T", " ").Replace("Z", "");
    //            return DateTime.Parse(s).ToLocalTime();
    //        }
    //        else if (type == ApiInfo.name)
    //        {
    //            return si;
    //        }
    //        else if (type == ApiInfo.language)
    //        {
    //            string s = si;
    //            if (si == "null")
    //            {
    //                s = "English";
    //            }
    //            else
    //            {
    //                CultureInfo ci = CultureInfo.GetCultureInfo(s, s);
    //                if (CultureInfo.CurrentCulture.EnglishName.Contains(ci.EnglishName))
    //                {
    //                    s = ci.NativeName;
    //                }
    //                else
    //                {
    //                    s = ci.EnglishName;
    //                }
    //            }
    //            return s;
    //        }
    //        else if (type == ApiInfo.followers || type == ApiInfo.viewers ||
    //            type == ApiInfo.views || type == ApiInfo.video_height || type == ApiInfo.average_fps)
    //        {
    //            string s = (si.Contains('.')) ? si.Remove(si.IndexOf('.')) : si;
    //            int i = 0;
    //            if (int.TryParse(s, out i))
    //            {
    //                s = i.ToString("N0");
    //                if (type == ApiInfo.video_height)
    //                {
    //                    s += "p";
    //                }
    //                else if (type == ApiInfo.average_fps)
    //                {
    //                    s += "fps";
    //                }
    //                return s;
    //            }
    //        }
    //        else if (type == ApiInfo.sub_plan)
    //        {
    //            SubType subtype = SubType.Prime;
    //            if (Enum.TryParse<SubType>(si, out subtype))
    //            {
    //            }
    //            return subtype;
    //        }
    //        else if (bool.TryParse(si, out b))
    //        {
    //            return b;
    //        }
    //        return si;
    //    }
    //}

    public class ApiResult
    {
        public static readonly ApiResult Empty = new ApiResult();

        private static readonly ApiInfo[] apiInfoSort = new ApiInfo[]{
            ApiInfo.display_name , ApiInfo.game ,
            ApiInfo.status , ApiInfo.viewers ,ApiInfo.followers,ApiInfo.follow_created_at,
            ApiInfo.video_height , ApiInfo.average_fps, ApiInfo.views, ApiInfo.partner,
            ApiInfo.language, ApiInfo.created_at, ApiInfo.sub_plan, ApiInfo._id};

        private Dictionary<ApiInfo, object> values = new Dictionary<ApiInfo, object>();

        public ApiResult()
        {
            values = new Dictionary<ApiInfo, object>();
        }

        public ApiResult(JSON.Twitch_Api.User user)
        {
            var typ = user.GetType();
            var props = typ.GetProperties();
            SetValues<JSON.Twitch_Api.User>(props, user);
        }

        public ApiResult(JSON.Twitch_Api.TokenBase user)
        {
            var typ = user.GetType();
            var props = typ.GetProperties();
            SetValues<JSON.Twitch_Api.TokenBase>(props, user);
        }

        public ApiResult(ApiResult result, ApiResult result1)
        {
            values = result.values;

            foreach (var t in result1.values)
            {
                if (!values.ContainsKey(t.Key))
                {
                    values.Add(t.Key, t.Value);
                }
            }
        }

        public ApiResult(JSON.Twitch_Api.Stream stream)
        {
            var typ = stream.GetType();
            var props = typ.GetProperties();
            SetValues<JSON.Twitch_Api.Stream>(props, stream);
            if (stream.channel != null)
            {
                props = stream.channel.GetType().GetProperties();
                SetValues<JSON.Twitch_Api.Channel>(props, stream.channel);
            }
            if (stream.preview != null)
            {
                props = stream.preview.GetType().GetProperties();
                SetValues<JSON.Twitch_Api.Preview>(props, stream.preview);
            }
        }

        public ApiResult(JSON.Twitch_Api.Root root)
        {
            var typ = root.GetType();
            var props = typ.GetProperties();
            SetValues<JSON.Twitch_Api.Root>(props, root);
        }

        public ApiResult(JSON.Twitch_Api.Subscription sub)
        {
            var typ = sub.GetType();
            var props = typ.GetProperties();
            SetValues<JSON.Twitch_Api.Subscription>(props, sub);
            if (sub.channel != null)
            {
                props = sub.channel.GetType().GetProperties();
                SetValues<JSON.Twitch_Api.Channel>(props, sub.channel);
            }
        }

        public ApiResult(JSON.Twitch_Api.Follow follow)
        {
            var typ = follow.GetType();
            var props = typ.GetProperties();
            SetValues<JSON.Twitch_Api.Follow>(props, follow);
            values.Add(ApiInfo.follow, true);
            if (follow.channel != null)
            {
                //Chat login delay ist zu lange!!!!!!!!!
                //int i = null;
                props = follow.channel.GetType().GetProperties();
                SetValues<JSON.Twitch_Api.Channel>(props, follow.channel);
            }
        }

        public ApiResult(JSON.Twitch_Api.Channel channel)
        {
            var typ = channel.GetType();
            var props = typ.GetProperties();
            SetValues<JSON.Twitch_Api.Channel>(props, channel);
        }

        public bool Followed
        {
            get { return GetValue<bool>(ApiInfo.follow); }
        }

        public int ID
        {
            get { return GetValue<int>(ApiInfo._id); }
        }

        public string Infos
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                var names = Enum.GetNames(typeof(ApiInfo)).ToList();
                foreach (var ai in apiInfoSort)
                {
                    string name = Enum.GetName(typeof(ApiInfo), ai);
                    if (values.ContainsKey(ai))
                    {
                        string s = values[ai].ToString();
                        if (s.Length > 0)
                        {
                            sb.Append(name.Replace("_", " ").ToTitleCase() + ": ");

                            sb.AppendLine(s);
                            sb.AppendLine();
                        }
                    }
                    names.Remove(name);
                }
                foreach (var Key in names)
                {
                    ApiInfo info = (ApiInfo)Enum.Parse(typeof(ApiInfo), Key);
                    if (values.ContainsKey(info))
                    {
                        string s = values[info].ToString();
                        if (s.Length > 0)
                        {
                            sb.Append(Key.Replace("_", " ").ToTitleCase() + ": ");

                            sb.AppendLine(s);
                            sb.AppendLine();
                        }
                    }
                }
                return sb.ToString();
            }
        }

        public bool IsEmpty
        {
            get { return values.Count == 0; }
        }

        public bool IsOnline
        {
            get { return values.ContainsKey(ApiInfo.stream_id); }
        }

        public string Name
        {
            get { return GetValue<string>(ApiInfo.name); }
        }

        public TimeSpan OnlineTime
        {
            get
            {
                DateTime dt = (IsOnline) ? GetValue<DateTime>(ApiInfo.stream_created_at) : GetValue<DateTime>(ApiInfo.created_at);
                return DateTime.Now.Subtract(dt);
            }
        }

        public string OnlineTimeString
        {
            get
            {
                return ((IsOnline) ? "Online: " : "Created: ") +
                    OnlineTime.ToString((OnlineTime.TotalDays >= 1.0) ? @"%d'd 'hh':'mm':'ss" : @"hh\:mm\:ss");
            }
        }

        public Dictionary<ApiInfo, object> Values
        {
            get { return values; }
            set { values = value; }
        }

        public T GetValue<T>(ApiInfo type)
        {
            try
            {
                if (values.ContainsKey(type))
                {
                    return (T)values[type];
                }
            }
            catch { }
            return default(T);
        }

        public string Info(params ApiInfo[] infos)
        {
            return Info("", infos);
        }

        public string Info(string newLine, params ApiInfo[] infos)
        {
            StringBuilder sb = new StringBuilder();
            var infs = (infos.Length == 0) ? apiInfoSort : infos;
            foreach (var info in infs)
            {
                if (values.ContainsKey(info))
                {
                    var val = values[info];
                    string s = val.ToString();
                    if (s.Length > 0)
                    {
                        sb.Append(Enum.GetName(typeof(ApiInfo), info).Replace("_", " ").ToTitleCase() + ": ");

                        sb.AppendLine(s + newLine);
                    }
                }
            }
            return sb.ToString();
        }

        public void ResetStreamStatus()
        {
            if (values.ContainsKey(ApiInfo.stream_id))
                values.Remove(ApiInfo.stream_id);
            if (values.ContainsKey(ApiInfo.stream_created_at))
                values.Remove(ApiInfo.stream_created_at);
            if (values.ContainsKey(ApiInfo.large))
                values.Remove(ApiInfo.large);
            if (values.ContainsKey(ApiInfo.stream_type))
                values.Remove(ApiInfo.stream_type);
        }

        private object ConvertValue(object si, ApiInfo type)
        {
            if (si == null) return string.Empty;
            bool b = false;
            int val = 0;
            if (type == ApiInfo.stream_type)
            {
                StreamType streamType = StreamType.None;
                if (Enum.TryParse<StreamType>(si.ToString(), out streamType))
                {
                }
                else
                {
                }
                return streamType;
            }
            else if (type == ApiInfo.created_at || type == ApiInfo.updated_at
                || type == ApiInfo.stream_created_at || type == ApiInfo.follow_created_at)
            {
                string s = si.ToString().Replace("T", " ").Replace("Z", "");
                return DateTime.Parse(s).ToLocalTime();
            }
            else if (type == ApiInfo.name)
            {
                return si;
            }
            else if (type == ApiInfo.language)
            {
                string s = si.ToString();
                if (si.Equals("null"))
                {
                    s = "English";
                }
                else
                {
                    CultureInfo ci = CultureInfo.GetCultureInfo(s, s);
                    if (CultureInfo.CurrentCulture.EnglishName.Contains(ci.EnglishName))
                    {
                        s = ci.NativeName;
                    }
                    else
                    {
                        s = ci.EnglishName;
                    }
                }
                return s;
            }
            else if (type == ApiInfo.followers || type == ApiInfo.viewers ||
                type == ApiInfo.views || type == ApiInfo.video_height || type == ApiInfo.average_fps)
            {
                string s = (si.ToString().Contains('.')) ? si.ToString().Remove(si.ToString().IndexOf('.')) : si.ToString();
                int i = 0;
                if (int.TryParse(s, out i))
                {
                    s = i.ToString("N0");
                    if (type == ApiInfo.video_height)
                    {
                        s += "p";
                    }
                    else if (type == ApiInfo.average_fps)
                    {
                        s += "fps";
                    }
                    return s;
                }
            }
            else if (type == ApiInfo.sub_plan)
            {
                SubType subtype = SubType.Prime;
                if (Enum.TryParse<SubType>(si.ToString(), out subtype))
                {
                }
                return subtype;
            }
            else if (si != null && bool.TryParse(si.ToString(), out b))
            {
                return b;
            }
            else if (si != null && int.TryParse(si.ToString(), out val))
            {
                return val;
            }
            return ((si != null) ? si.ToString() : "");
        }

        private void SetValues<T>(System.Reflection.PropertyInfo[] props, T channel)
        {
            foreach (var prop in props)
            {
                var info = ApiInfo.None;
                if (Enum.TryParse<ApiInfo>(prop.Name, out info))
                {
                    if (typeof(T).IsEquivalentTo(typeof(JSON.Twitch_Api.Stream)))
                    {
                        switch (info)
                        {
                            case ApiInfo._id:
                                info = ApiInfo.stream_id;
                                break;

                            case ApiInfo.created_at:
                                info = ApiInfo.stream_created_at;
                                break;
                        }
                    }
                    object o = prop.GetValue(channel);
                    o = ConvertValue(o, info);
                    //if (o is int || o is string && string.IsNullOrEmpty(((string)o)))
                    //{
                    //    ApiInfo[] test = new ApiInfo[] { ApiInfo.name, ApiInfo._id };

                    //    if (test.Any(t => t == info))
                    //    {
                    //        values.Clear();
                    //        break;
                    //    }
                    //}
                    if (!values.ContainsKey(info))
                        values.Add(info, o);
                }
            }
        }
    }

    public class PanelResult : IDisposable
    {
        //Change everything that can be parsed to newtonsoft-json
        //private int i = null;

        private System.Drawing.Image image = null;
        private JSON.Twitch_Api.Panel value;

        public PanelResult(JSON.Twitch_Api.Panel panel)
        {
            value = panel;
            //values = new Dictionary<PanelInfo, object>();
            //foreach (var s in infos)
            //{
            //    object o = s.Value;
            //    int i = 0;
            //    bool b = false;
            //    if (int.TryParse(s.Value, out i))
            //    {
            //        o = i;
            //    }
            //    else if (bool.TryParse(s.Value, out b))
            //    {
            //        o = b;
            //    }
            //    else
            //    {
            //        o = s.Value.Trim(" ", "{", "}", "null");
            //    }
            //    if (!values.ContainsKey(s.Key))
            //    {
            //        values.Add(s.Key, o);
            //    }
            //}
        }

        public string Decription
        {
            get { return value.data.description; }
        }

        public string HTML
        {
            get { return value.html_description; }
        }

        public System.Drawing.Image Image
        {
            get
            {
                if (image == null)
                {
                    try
                    {
                        string url = value.data.image;
                        if (!string.IsNullOrEmpty(url))
                        {
                            image = new System.Drawing.Bitmap(1, 1);
                            WebClient wc = new WebClient();
                            wc.Proxy = null;
                            var t = wc.DownloadData(url);
                            //var wait = t.GetAwaiter();
                            //wait.OnCompleted(new Action(delegate()
                            //{
                            using (var ms = new MemoryStream(t))
                            {
                                image = System.Drawing.Bitmap.FromStream(ms);
                            }
                            wc.Dispose();
                            //}));
                        }
                    }
                    catch { image = null; }
                    return image;
                }
                else return image;
            }
        }

        public int Index
        {
            get { return value.display_order; }
        }

        public string Link
        {
            get { return value.data.link; }
        }

        public string Title
        {
            get { return value.data.title; }
        }

        public int User_ID
        {
            get { return value.user_id; }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool Dispose(bool dispose)
        {
            if (image != null) image.Dispose();
            if (true) GC.SuppressFinalize(this);
            return dispose;
        }
    }

    public class SubResult
    {
        public static readonly SubResult NonSub = new SubResult() { Type = SubType.NoSub };
        public static readonly SubResult NoSubProgram = new SubResult() { Type = SubType.NoSubProgram };

        private static readonly byte[] subTypePrice = new byte[] { 0, 5, 10, 25 };

        public SubResult()
        {
            Base = ApiResult.Empty;
        }

        public SubResult(ApiResult result)
        {
            Base = result;
            Type = Base.GetValue<SubType>(ApiInfo.sub_plan);
        }

        public ApiResult Base
        {
            get;
            private set;
        }

        public bool CanSub
        {
            get { return Type == SubType.NoSub; }
        }

        public string ChannelName
        {
            get { return Base.Name; }
        }

        //Last Sub Date
        public DateTime Date
        {
            get
            {
                if (Base.Values.ContainsKey(ApiInfo.created_at))
                {
                    return Base.GetValue<DateTime>(ApiInfo.created_at);
                }
                return DateTime.MinValue;
            }
        }

        //Latest Date for resubscription
        public DateTime FutureResubDate
        {
            get
            {
                if (!Date.Equals(DateTime.MinValue))
                {
                    return (Type == SubType.Prime) ? SubEndDate.AddDays(20) : SubEndDate.AddDays(30);
                }
                else
                {
                    return DateTime.MinValue;
                }
            }
        }

        //If user can sub & is sub, True
        public bool IsSub
        {
            get { return (int)Type >= 0; }
        }

        //Sub Plan Name
        public string PlanName
        {
            get { return Base.GetValue<string>(ApiInfo.sub_plan_name); }
        }

        //Sub end Date, e.g. losing badge & emotes
        public DateTime SubEndDate
        {
            get
            {
                if (!Date.Equals(DateTime.MinValue))
                {
                    //frag mi ned, auf twitch steht prime läuft nach 30 tagen aus, aber alle
                    //anderen die ich extra nachgezählt habe nach 32,
                    //also sicherheitshalber mal so machen XD
                    return Date.AddDays(30);
                }
                else
                {
                    return DateTime.MinValue;
                }
            }
        }

        //Sub Type (Prime=0, Tier1=1000, Tier2=2000, Tier3=3000)
        public SubType Type
        {
            get;// { return Base.GetValue<SubType>(ApiInfo.sub_plan); }
            private set;
        }

        public static SubResult Parse(string res)
        {
            if (res.Equals("404"))
            {
                //Has sub-Programm but User is no Sub (404 == User has no sub at Channel)
                return SubResult.NonSub;
            }
            else if (res.Equals("422"))
            {
                //Has NO sub-Programm (422 == Channel no sub programm)
                return SubResult.NoSubProgram;
            }
            else
            {
                //Parse Sub Info
                var apires = JSON_Parser.JSON.ParseSub(res.ToString());

                return new SubResult(new ApiResult(apires));
            }
        }

        public override string ToString()
        {
            int price = subTypePrice[Math.Max(0, ((int)Type - 1000) + 1)];
            return "Type:\t" + Enum.GetName(typeof(SubType), Type) + "\r\n" +
                "Price:\t" + price + "$\r\n" +
                "Date:\t" + Date.ToShortDateString() + "\r\n" +
                "End:\t" + SubEndDate.ToShortDateString() + "\r\n" +
                "Resub:\t" + FutureResubDate.ToShortDateString() + "\r\n";
        }
    }
}