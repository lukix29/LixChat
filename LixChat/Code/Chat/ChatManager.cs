using LX29_ChatClient.Emotes;
using LX29_Twitch.Api;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace LX29_ChatClient
{
    public enum channel_mode
    {
        NONE = 0,
        subs_on = 1,//	This room is now in subscribers-only mode.
        subs_off = 2,//	This room is no longer in subscribers-only mode.
        slow_on = 3,//	This room is now in slow mode. You may send messages every slow_duration seconds.
        slow_off = 4,//	This room is no longer in slow mode.
        r9k_on = 5,//	This room is now in r9k mode.
        r9k_off = 6,//	This room is no longer in r9k mode.
        host_on = 7,//	Now hosting target_channel.
        host_off = 8,//	Exited host mode.
        emote_only_on = 9,//	This room is now in emote-only mode.
        emote_only_off = 10,//This room is no longer in emote-only mode.
        msg_channel_suspended = 11,//	This channel has been suspended.
    }

    public enum SortMode
    {
        Favorite = 0,
        Viewing = 1,
        Sub = 2,
        Chatlogin = 3,
    }

    public enum UserType : int
    {
        None = 0,
        NORMAL = 1,
        turbo = 2,
        premium = 3,
        subscriber = 4,
        moderator = 5,
        broadcaster = 6,
        global_mod = 7,
        admin = 8,
        staff = 9,
    }

    public class ChatUser
    {
        public readonly static ChatUser Emtpy = new ChatUser("", "");

        private Color Color = Color.White;
        private string displayName = "";

        private ApiResult result = null;

        private HashSet<UserType> typelist = new HashSet<UserType>();

        public ChatUser(string UserName, string channel, UserType type = UserType.NORMAL)
        {
            To_Timer = new TimeoutTimer();
            IsOnline = true;
            //DisplayName = UserName;
            Name = UserName.ToLower();
            Color = UserColors.RandomColor(Name);
            Badges = new BadgeBase[0];
            typelist = new HashSet<UserType>();
            typelist.Add(type);
            Channel = channel;
        }

        public ChatUser(string channel, Dictionary<irc_params, string> parameters, string name, TimeOutResult toResult)
        {
            Parse(channel, parameters, name, toResult);
        }

        public ApiResult ApiResult
        {
            get
            {
                if (result == null)
                {
                    result = TwitchApi.GetUserID(Name);
                    result = TwitchApi.GetStreamOrChannel(result.ID)[0];
                }
                return result;
            }
        }

        public BadgeBase[] Badges
        {
            get;
            private set;
        }

        public string Channel
        {
            get;
            private set;
        }

        public Channels.ChannelInfo ChannelInfo
        {
            get { return ChatClient.Channels[Channel]; }
        }

        public string DisplayName
        {
            get
            {
                if (displayName.IsEmpty())
                {
                    if (result != null && !result.IsEmpty)
                        displayName = result.GetValue<string>(ApiInfo.display_name);
                }
                if (displayName.IsEmpty())
                {
                    return Name;
                }
                else return displayName;
            }
            private set
            {
                displayName = value;
            }
        }

        public bool HasTimeOut
        {
            get { return To_Timer.Result.HasTimeOut; }
        }

        public string ID
        {
            get;
            private set;
        }

        public bool IsBanned
        {
            get { return To_Timer.Result.IsBanned; }
        }

        public bool IsEmpty
        {
            get { return (Name.Length == 0); }
        }

        public bool IsOnline
        {
            get;
            set;
        }

        public string Name
        {
            get;
            private set;
        }

        public TimeoutTimer To_Timer
        {
            private set;
            get;
        }

        public HashSet<UserType> Types
        {
            get
            {
                return typelist;
            }
        }

        public Color GetColor()
        {
            return UserColors.ChangeColorBrightness(Color);
        }

        public bool IsType(params UserType[] Type)
        {
            if (Type.Length > 1)
            {
                var gs = Type.Any(t => typelist.Contains(t));
                return gs;
            }
            else
            {
                return typelist.Contains(Type[0]);
            }
        }

        public void Parse(string channel, Dictionary<irc_params, string> parameters, string name, TimeOutResult toResult)
        {
            try
            {
                IsOnline = true;
                Color color = Color.White;// UserColors.RandomColor(name);
                if (parameters != null)
                {
                    if (parameters.ContainsKey(irc_params.user_id))
                    {
                        ID = parameters[irc_params.user_id];
                    }
                    else
                    {
                    }
                    if (parameters.ContainsKey(irc_params.display_name))
                    {
                        DisplayName = parameters[irc_params.display_name];
                    }
                    //else
                    //{
                    //    DisplayName = name;
                    //}
                    Name = name.ToLower();

                    if (parameters.ContainsKey(irc_params.color))
                    {
                        color = UserColors.ToColor(parameters[irc_params.color], Name);
                    }
                    else
                    {
                        color = UserColors.RandomColor(Name);
                    }

                    HashSet<UserType> tpyes = new HashSet<UserType>();
                    Badges = GetBadges(parameters, channel, Name, out tpyes);
                    if (tpyes.Count > 0)
                    {
                        foreach (var t in tpyes)
                        {
                            if (!typelist.Contains(t))
                            {
                                typelist.Add(t);
                            }
                        }
                    }
                }
                else
                {
                    color = UserColors.RandomColor(Name);
                }

                Color = color;

                Channel = channel;

                SetTimeOut(toResult);
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        Parse(channel, parameters, name, toResult);
                        break;
                }
            }
        }

        public void Set(ChatUser user)
        {
            typelist = user.typelist;
            Name = user.Name;
            IsOnline = true;
        }

        public void SetTimeOut(TimeOutResult toResult)
        {
            To_Timer = new TimeoutTimer(toResult);
            if (toResult.HasTimeOut)
            {
                System.Threading.Timer timer =
                    new System.Threading.Timer(new TimerCallback(delegate(object o)
                    {
                        To_Timer = new TimeoutTimer();
                    }),
                    To_Timer, toResult.TimeOutDuration * 1000, System.Threading.Timeout.Infinite);

                To_Timer.Timer = timer;
            }
        }

        //public void LoadApiInfo()
        //{
        //    if (ApiResult.IsEmpty)
        //    {
        //        if (ID))
        //        {
        //            ID = TwitchApi.GetUserID(Name);
        //        }
        //        ApiResult = TwitchApi.GetApiResult(ApiRequests.users, ID);
        //    }
        //}
        public override string ToString()
        {
            string delimiter = "";
            delimiter = typelist.Select(t => Enum.GetName(typeof(UserType), t)).Aggregate((i, j) => i + delimiter + j);
            return DisplayName + ", " + Channel + ", " + delimiter;
        }

        private static BadgeBase[] GetBadges(Dictionary<irc_params, string> parameters, string channel, string Name, out HashSet<UserType> types)
        {
            types = new HashSet<UserType>();

            List<BadgeBase> list = new List<BadgeBase>();
            if (parameters.ContainsKey(irc_params.badges))
            {
                string s = parameters[irc_params.badges];
                var temp = s.Split(",");
                foreach (var value in temp)
                {
                    string type = value.GetBetween("", "/");
                    string version = value.GetBetween("/", "");
                    var badge = new BadgeBase(channel, type, version, BadgeOrigin.Twitch);
                    list.Add(badge);
                }
            }
            if (ChatClient.Emotes.Badges != null && ChatClient.Emotes.Badges.FFZ_Users != null)
            {
                var ffz = ChatClient.Emotes.Badges.FFZ_Users.Where(t => t.Username.Equals(Name, StringComparison.OrdinalIgnoreCase)).ToArray();
                if (ffz.Length > 0)
                {
                    //FFZ AP Badges spackt iwas wo ich zu blöd binn
                    //   int i = null;
                    //steht auch noch wo anders
                    if (ffz.Length > 1)
                    {
                    }
                    if (ffz.Any(t => t.Badge.Origin == BadgeOrigin.FFZ_AP))
                    {
                    }
                    foreach (var ff in ffz)
                    {
                        var badge = new BadgeBase(ff.Badge.Name, ff.Badge.Type, ff.Badge.Version, BadgeOrigin.FFZ);
                        list.Add(badge);
                    }
                }
            }
            if (parameters.ContainsKey(irc_params.user_type))
            {
                string s = parameters[irc_params.user_type];
                if (s.Length > 0)
                {
                    UserType typ = UserType.NORMAL;
                    if (Enum.TryParse<UserType>(s, true, out typ))
                    {
                        if (!types.Contains(typ))
                            types.Add(typ);
                    }
                }
            }

            if (parameters.ContainsKey(irc_params.mod))
            {
                string s = parameters[irc_params.mod];
                if (s == "1")
                {
                    if (!types.Contains(UserType.moderator))
                        types.Add(UserType.moderator);
                }
            }

            if (parameters.ContainsKey(irc_params.subscriber))
            {
                string s = parameters[irc_params.subscriber];
                if (s == "1")
                {
                    if (!types.Contains(UserType.subscriber))
                        types.Add(UserType.subscriber);
                }
            }

            if (parameters.ContainsKey(irc_params.turbo))
            {
                string s = parameters[irc_params.turbo];
                if (s == "1")
                {
                    if (!types.Contains(UserType.turbo))
                        types.Add(UserType.turbo);
                }
            }
            return list.ToArray();
        }
    }

    public class TimeoutTimer
    {
        public TimeOutResult Result;
        public System.Threading.Timer Timer;

        public TimeoutTimer(TimeOutResult result)
        {
            Result = result;
        }

        public TimeoutTimer()
        {
            Result = TimeOutResult.Empty;
        }
    }

    public partial class UserColors
    {
        public static Color calculateColor(Color c, bool darkenedMode)
        {
            bool bgColor = false;
            double[] color = new double[] { c.R, c.G, c.B };
            //double[] bg = new double[] { ChatBackground1.R, ChatBackground1.G, ChatBackground1.B };
            for (int i = 20; i >= 0; i--)
            {
                bgColor = calculateColorBackground(color);
                if (bgColor == true && darkenedMode != true)
                    break;
                if (bgColor == false && darkenedMode == true)
                    break;
                color = calculateColorReplacement(color, bgColor);
            }
            return Color.FromArgb((int)color[0], (int)color[1], (int)color[2]);
        }

        private static bool calculateColorBackground(double[] color)
        {
            double r = color[0];
            double g = color[1];
            double b = color[2];
            double yiq = ((r * 299.0) + (g * 587.0) + (b * 114.0)) / 1000.0;
            return yiq >= 128.0 ? false : true;
        }

        private static double[] calculateColorReplacement(double[] color, bool light)
        {
            double factor = light ? Settings.UserColorBrigthness : -Settings.UserColorBrigthness;

            double r = color[0];
            double g = color[1];
            double b = color[2];
            var hsl = rgbToHsl(r, g, b);

            // more thoroughly lightens dark colors, with no problems at black
            double l = light ? (1.0 - (1.0 - factor) * (1.0 - hsl[2])) : ((1.0 + factor) * hsl[2]);
            l = Math.Min(Math.Max(0.0, l), 1.0);

            double s = Math.Max(0.0, Math.Min(1.0, Settings.UserColorSaturation * hsl[1]));

            var rgb = hslToRgb(hsl[0], s, l);
            return rgb;
        }

        private static double[] hslToRgb(double h, double s, double l)
        {
            if (s == 0)
            {
                double rgb = Math.Round(Math.Min(Math.Max(0, 255.0 * l), 255.0)); // achromatic
                return new double[] { rgb, rgb, rgb };
            }

            var q = l < 0.5 ? l * (1.0 + s) : l + s - l * s;
            var p = 2 * l - q;
            double r = Math.Round(Math.Min(Math.Max(0, 255.0 * hueToRgb(p, q, h + 1.0 / 3.0)), 255.0));
            double g = Math.Round(Math.Min(Math.Max(0, 255.0 * hueToRgb(p, q, h)), 255.0));
            double b = Math.Round(Math.Min(Math.Max(0, 255.0 * hueToRgb(p, q, h - 1.0 / 3.0)), 255.0));
            return new double[] { r, g, b };
        }

        private static double hueToRgb(double pp, double qq, double t)
        {
            if (t < 0.0) t += 1.0;
            if (t > 1.0) t -= 1.0;
            if (t < 1.0 / 6.0) return pp + (qq - pp) * 6.0 * t;
            if (t < 1.0 / 2.0) return qq;
            if (t < 2.0 / 3.0) return pp + (qq - pp) * (2.0 / 3.0 - t) * 6.0;
            return pp;
        }

        private static double[] rgbToHsl(double r, double g, double b)
        {
            r /= 255.0;
            g /= 255.0;
            b /= 255.0;
            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Max(g, b));
            double l = Math.Min(Math.Max(0.0, (max + min) / 2.0), 1.0);
            double d = Math.Min(Math.Max(0.0, max - min), 1.0);

            if (d == 0.0)
            {
                return new double[] { d, d, l }; // achromatic
            }

            double h = 0.0;

            if (max == r)
                h = Math.Min(Math.Max(0.0, (g - b) / d + (g < b ? 6.0 : 0.0)), 6.0);
            else if (max == g)
                h = Math.Min(Math.Max(0.0, (b - r) / d + 2.0), 6.0);
            else if (max == b)
                h = Math.Min(Math.Max(0.0, (r - g) / d + 4.0), 6.0);

            h /= 6.0;

            double s = l > 0.5 ? d / (2.0 * (1.0 - l)) : d / (2.0 * l);
            s = Math.Min(Math.Max(0.0, s), 1.0);

            return new double[] { h, s, l };
        }
    }

    public partial class UserColors
    {
        public readonly static Color ChatLine = Color.FromArgb(80, 80, 80);
        private static Color _ChatBackground = Color.FromArgb(0, 0, 0, 0);

        private static int lastColor = 0;

        private static Color[] standardColors = new Color[] {
            Color.Red,
            Color.FromArgb(50, 50, 200),
            Color.Green,
            Color.FromArgb(178, 34, 34),
            Color.Coral,
            Color.YellowGreen,
            Color.OrangeRed,
            Color.SeaGreen,
            Color.FromArgb(218, 165, 32),
            Color.Chocolate,
            Color.CadetBlue,
            Color.DodgerBlue,
            Color.HotPink,
            Color.BlueViolet,
            Color.SpringGreen
        };

        public static Color ChatBackground
        {
            get
            {
                if (lastColor != Settings.ChatBackGround)
                {
                    lastColor = Settings.ChatBackGround;
                    _ChatBackground = Color.FromArgb(lastColor);
                }
                return _ChatBackground;
            }
        }

        public static Color ChatBackground1
        {
            get { return Color.FromArgb(ChatBackground.R - 5, ChatBackground.G - 5, ChatBackground.B - 5); }
        }

        public static Color ChangeColorBrightness(Color color)
        {
            return calculateColor(color, true);
            ////Noch besser machen!!!!!
            //if (color.R < ChatBackground.R &&
            //    color.B < ChatBackground.B &&
            //    color.G < ChatBackground.G)
            //{
            //    color = ChatBackground;
            //}

            //var hslBG = new HSL(ChatBackground);
            //var hsl = new HSL(color);

            //if (hslBG.Brightness * 2 > hsl.Brightness)
            //{
            //    hsl.Brightness *= (2 * Settings.UserColorBrigthness);
            //}

            //if (color.B > 250 &&
            //    color.R < 100 &&
            //    color.G < 100)
            //{
            //    hsl.Saturation = 0.5;
            //}

            //hsl.Brightness *= Settings.UserColorBrigthness;
            //hsl.Saturation *= Settings.UserColorSaturation;

            //return hsl.ToColor();
        }

        public static Color RandomColor(string name)
        {
            if (name.IsEmpty())
            {
                return standardColors[0];
            }
            int n0 = Convert.ToInt32(name[0]);
            int n1 = Convert.ToInt32(name.Last());
            int l = standardColors.Length;
            int n = (n0 + n1) % l;
            n = Math.Max(0, Math.Min(l - 1, n));
            return standardColors[n];
        }

        public static Color ToColor(string hex, string name = "")
        {
            hex = hex.Replace("#", "");
            if (!hex.IsEmpty())
            {
                if (hex.Length < 6)
                {
                }
                int r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                int g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                int b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

                Color c = Color.FromArgb(r, g, b);
                return c;
            }
            if (name.IsEmpty())
            {
                return Color.Gray;
            }
            return RandomColor(name);
        }
    }
}