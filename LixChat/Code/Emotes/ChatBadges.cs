using LX29_ChatClient.Channels;
using LX29_Twitch.JSON_Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Net;
using System.Text;

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
            URLS.Add("1", new EmoteImage(urls, Name));
        }

        public Badge(LX29_Twitch.JSON_Parser.JSON.FFZ_Emotes.Badge badge)
        {
            Origin = BadgeOrigin.FFZ;
            Type = badge.name.Trim() + "_FFZ";
            Name = Type;
            URLS = new Dictionary<string, EmoteImage>();
            URLS.Add("1", new EmoteImage(badge.urls, Name));
        }

        public Badge(string name, JSON.Twitch_Badges.Versions lst)
        {
            Origin = BadgeOrigin.Twitch;
            Type = "None";
            Name = name.Trim();
            URLS = new Dictionary<string, EmoteImage>();
            foreach (var kvp in lst.versions)
            {
                Type = kvp.Value.description;
                URLS.Add(kvp.Key, new EmoteImage(kvp.Value.urls, Name));
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
                if (Origin == BadgeOrigin.FFZ)
                {
                    g.FillRectangle(ffz_brush, X, Y, Width, Height);
                }
                img.Draw(g, X, Y, Width, Height);
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
            if (!string.IsNullOrEmpty(version))
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

        public void Fetch_Badges(string url = "https://badges.twitch.tv/v1/badges/global/display", ChannelInfo ci = null)
        {
            try
            {
                WebClient wc = new WebClient();
                wc.Proxy = null;
                var str = wc.DownloadString(url);
                wc.Dispose();

                var result = JSON.ParseBadges(str);

                foreach (var kvp in result.badge_sets)
                {
                    string name = (ci != null) ? ci.Name : kvp.Key;

                    Badge badge = new Badge(name, kvp.Value);
                    if (!badges.ContainsKey(name))
                    {
                        badges.Add(name, badge);
                    }
                }
            }
            catch
            { }
        }

        public void Fetch_Sub_Badges(ChannelInfo ci)
        {
            try
            {
                string url = "https://badges.twitch.tv/v1/badges/channels/" + ci.ID + "/display";
                Fetch_Badges(url, ci);
            }
            catch
            {
            }
        }

        public void Parse_FFZ_Addon_Badges()
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
                if (user.username.Equals("cwar97", StringComparison.OrdinalIgnoreCase))
                {
                }
                FFZ_Users.Add(new FFZBadgeUser(user.username, b));
                //Badge b = new Badge(badge);
                //badges.Add(b.Name, b);
                //var user = result.users[badge.id];
                //foreach (var u in user)
                //{
                //    FFZ_Users.Add(new FFZBadgeUser(u, b));
                //}
            }
        }

        public void Parse_FFZ_Badges()
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