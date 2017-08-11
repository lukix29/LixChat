using LX29_ChatClient.Channels;
using LX29_Twitch.JSON_Parser;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace LX29_ChatClient.Emotes
{
    public enum BadgeOrigin
    {
        Twitch = 0,
        FFZ = 1,
        FFZ_AP = 2,
    }

    public class Badge : BadgeBase
    {
        private static readonly SolidBrush ffz_brush = new SolidBrush(Color.FromArgb(117, 80, 0));

        public Badge(string name, string type, Dictionary<string, string> urls)
        {
            Origin = BadgeOrigin.FFZ_AP;
            Type = name;
            Name = type;
            URLS = new Dictionary<string, EmoteImage>();
            URLS.Add("1", new EmoteImage(urls, Name, EmoteOrigin.Badge));
        }

        public Badge(LX29_Twitch.JSON_Parser.JSON.FFZ_Emotes.Badge badge)
        {
            Origin = BadgeOrigin.FFZ;
            Type = badge.name.Trim() + "_FFZ";
            Name = Type;
            URLS = new Dictionary<string, EmoteImage>();
            URLS.Add("1", new EmoteImage(badge.urls, Name, EmoteOrigin.Badge));
        }

        public Badge(JSON.Twitch_Badges.BadgeData lst)
        {
            Origin = BadgeOrigin.Twitch;
            Type = "None";
            Name = lst.Name.Trim();
            URLS = new Dictionary<string, EmoteImage>();
            foreach (var kvp in lst.versions)
            {
                Type = kvp.Value.description;
                URLS.Add(kvp.Key, new EmoteImage(kvp.Value.urls, Name, EmoteOrigin.Badge));
            }
        }

        private Dictionary<string, EmoteImage> URLS
        {
            get;
            set;
        }

        public void Draw(BadgeBase b, Graphics g, float X, float Y, float Width, float Height)
        {
            var img = GetImage(b);
            if (img != null)
            {
                switch (Origin)
                {
                    case BadgeOrigin.FFZ:
                        g.FillRectangle(ffz_brush, X, Y, Width, Height);
                        break;

                    case BadgeOrigin.FFZ_AP:
                        g.FillRectangle(Brushes.MediumBlue, X, Y, Width, Height);
                        break;
                }
                img.Draw(g, X, Y, Width, Height, Settings.EmoteQuality);
            }
        }

        public EmoteImage GetImage(BadgeBase b)
        {
            if (URLS.ContainsKey(b.Version))
            {
                return URLS[b.Version];
            }
            else if (URLS.Count > 0)
            {
                return URLS.First().Value;
            }
            return null;
        }
    }

    public class BadgeBase
    {
        public BadgeBase()
        {
        }

        public BadgeBase(string name, string type, string version, BadgeOrigin origin)
        {
            Name = name.Trim();
            Type = type.Trim();
            Origin = origin;
            if (!version.IsEmpty())
            {
                Version = version.Trim();
            }
            else
            {
                Version = "1";
            }
        }

        public string Name
        {
            get;
            set;
        }

        public BadgeOrigin Origin
        {
            get;
            set;
        }

        public string Type
        {
            get;
            set;
        }

        public string Version
        {
            get;
            set;
        }
    }

    public class BadgeCollection
    {
        private static Dictionary<string, Badge> badges;

        public BadgeCollection()
        {
            badges = new Dictionary<string, Badge>();
        }

        public int Count
        {
            get { return badges.Count; }
        }

        public List<FFZBadgeUser> FFZ_Users
        {
            get;
            private set;
        }

        public Badge this[BadgeBase name]
        {
            get
            {
                if (name.Type.Equals("subscriber", StringComparison.OrdinalIgnoreCase))
                {
                    if (badges.ContainsKey(name.Name))
                    {
                        return badges[name.Name];
                    }
                }
                else
                {
                    if (badges.ContainsKey(name.Type))
                    {
                        return badges[name.Type];
                    }
                }
                return null;
            }
        }

        public void Fetch_Channel_Badges(ChannelInfo ci = null)
        {
            try
            {
                string url = (ci == null) ? "https://badges.twitch.tv/v1/badges/global/display" : "https://badges.twitch.tv/v1/badges/channels/" + ci.ID + "/display";
                using (WebClient wc = new WebClient())
                {
                    wc.Proxy = null;
                    wc.Headers.Add("Client-ID", LX29_Twitch.Api.TwitchApi.CLIENT_ID);
                    using (JsonTextReader reader = new JsonTextReader(new StreamReader(wc.OpenRead(url))))
                    {
                        JsonSerializer jser = new JsonSerializer();
                        if (!reader.Read() || !reader.Read() || !reader.Read()) return;
                        while (true)
                        {
                            if (!reader.Read())
                                break;
                            if (reader.TokenType == JsonToken.PropertyName)
                            {
                                var name = (ci == null) ? reader.Value.ToString() : ci.Name;

                                if (!reader.Read())
                                    break;

                                var obj = jser.Deserialize<JSON.Twitch_Badges.BadgeData>(reader);
                                obj.Name = name;

                                Badge badge = new Badge(obj);
                                if (!badges.ContainsKey(name))
                                {
                                    badges.Add(name, badge);
                                }
                            }
                        }
                    }
                    //}
                    //else
                    //{
                    //    var result = JSON.ParseBadges(str);
                    //    if (result.badge_sets.subscriber != null)
                    //    {
                    //        string name = ci.Name;
                    //        Badge badge = new Badge(name, result);
                    //        if (!badges.ContainsKey(name))
                    //        {
                    //            badges.Add(name, badge);
                    //        }
                    //    }
                    //}
                }
            }
            catch (Exception x)
            {
                //switch (x.Handle())
                //{
                //    case System.Windows.Forms.MessageBoxResult.Retry:
                //        Fetch_Channel_Badges(ci);
                //        break;
                //}
            }
        }

        public void Parse_FFZ_Addon_Badges()
        {
            try
            {
                WebClient wc = new WebClient();
                wc.Proxy = null;
                string temp = wc.DownloadString("https://cdn.ffzap.download/supporters.json");
                wc.Dispose();
                var result = JSON.ParseFFZAddonBadges(temp);

                //FFZ_Users = new List<FFZBadgeUser>();
                var badge = result.badges[0];
                Badge b = new Badge(badge.name + "_ffzap", badge.title, badge.urls);
                badges.Add(b.Type, b);
                foreach (var user in result.users)
                {
                    FFZ_Users.Add(new FFZBadgeUser(user.username, b));
                }
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case System.Windows.Forms.MessageBoxResult.Retry:
                        Parse_FFZ_Addon_Badges();
                        break;
                }
            }
        }

        public void Parse_FFZ_Badges()
        {
            try
            {
                WebClient wc = new WebClient();
                wc.Proxy = null;
                string temp = wc.DownloadString("http://api.frankerfacez.com/v1/badges");
                wc.Dispose();
                var result = JSON.ParseFFZBadges(temp);

                FFZ_Users = new List<FFZBadgeUser>();
                foreach (var badge in result.badges)
                {
                    Badge b = new Badge(badge);
                    badges.Add(b.Name, b);
                    var user = result.users[badge.id];
                    foreach (var u in user)
                    {
                        FFZ_Users.Add(new FFZBadgeUser(u, b));
                    }
                }
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case System.Windows.Forms.MessageBoxResult.Retry:
                        Parse_FFZ_Badges();
                        break;
                }
            }
        }
    }

    public class FFZBadgeUser
    {
        public FFZBadgeUser(string user, Badge badge)
        {
            Badge = badge;
            Username = user;
        }

        public Badge Badge
        {
            get;
            private set;
        }

        public string Username
        {
            get;
            private set;
        }
    }
}