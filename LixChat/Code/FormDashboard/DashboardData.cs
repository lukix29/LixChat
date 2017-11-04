using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LX29_Twitch.Api;
using LX29_Twitch.JSON_Parser;
using Newtonsoft.Json;

namespace LX29_ChatClient.Dashboard
{
    public class ChannelSubJson
    {
        public string _cursor { get; set; }
        public int _total { get; set; }

        public List<Sub> follows { get; set; }

        //public List<FollowSub> follows { get { return subscriptions; } set { subscriptions = value; } }
        public List<Sub> subscriptions { get; set; }
    }

    public class DashboardData
    {
        public DashboardData()
        {
        }

        public Channels.ChannelInfo Channel
        {
            get { return ChatClient.Channels[User.ID]; }
        }

        public int ChatterCount
        {
            get { return Chatters.Count; }
        }

        public Dictionary<string, Viewer> Chatters
        {
            get;
            private set;
        }

        public int FollowCount
        {
            get { return Channel.ApiResult.GetValue<int>(ApiInfo.followers); }
        }

        public int ReSubsSinceStreamStart
        {
            get
            {
                var ts = Channel.ApiResult.GetValue<DateTime>(ApiInfo.stream_created_at);
                var sec = Subs.Count(t => (t.created_at.Subtract(ts).TotalSeconds > 0) && (t.sub_plan.Contains("resub")));
                return sec;
            }
        }

        public int SubCount
        {
            get;
            private set;
        }

        public List<Sub> Subs
        {
            get;
            private set;
        }

        public int SubsSinceStreamStart
        {
            get
            {
                //Add resub/sub duration info from chat when sub occured to SubList info
                //(save that info somehow before refresh)
                var ts = Channel.ApiResult.GetValue<DateTime>(ApiInfo.stream_created_at);
                if (ts.Ticks > 0)
                {
                    var sec = Subs.Count(t => t.created_at.Subtract(ts).TotalSeconds > 0);
                    return sec;
                }
                return 0;
            }
        }

        public TwitchUser User
        {
            get { return TwitchUserCollection.Selected; }
        }

        public int ViewerCount
        {
            get { return Channel.ApiResult.GetValue<int>(ApiInfo.viewers); }
        }

        public void GetSubscriptions()
        {
            try
            {
                TwitchUserCollection.CheckToken(false);
                Dictionary<string, Viewer> subs = new Dictionary<string, Viewer>();
                //var fols = downloadFollows();
                Subs = downloadSubs();
                var con = Subs.Distinct(new DistinctItemComparer()).ToDictionary(t => t.user.name);// fols.Concat(sbs);

                SubCount = Subs.Count;

                //FollowCount = Channel.ApiResult.GetValue<int>(ApiInfo.followers);// fols.Count;

                while (ChatClient.Users.Count(User.Name) == 0)
                {
                    Task.Delay(500).Wait();
                }

                var users = GetChatUsers(User.Name);
                List<string> noID = new List<string>();
                foreach (var u in users)
                {
                    Viewer v = new Viewer();
                    if (con.ContainsKey(u.name))
                    {
                        v.sub = con[u.name];
                        v.user = con[u.name].user;
                        v.user.usertype = u.usertype;
                    }
                    else
                    {
                        v.user = u;
                        noID.Add(u.name);
                    }
                    if (u.name.Equals(User.Name))
                    {
                        v.user.usertype = UserType.broadcaster;
                    }

                    subs.Add(u.name, v);
                }
                var ids = TwitchApi.GetUserID(noID.ToArray());
                foreach (var id in ids)
                {
                    subs[id.Name].user.FromApiResult(id);
                }

                Chatters = subs;
            }
            catch
            {
            }
        }

        private static IEnumerable<User> GetChatUsers(string ChannelName)
        {
            string values = TwitchApi.downloadString("https://tmi.twitch.tv/group/user/" + ChannelName + "/chatters").ToString()
                .ReplaceAll("", " ", "\n", "\"").Replace("_links:{},", "");
            //var result = JSON.ParseChatters(values);
            //if (result != null && result.chatters != null)
            //{
            //    var users = result.chatters.moderators.Select(t => new LX29_ChatClient.ChatUser(t, ChannelName, LX29_ChatClient.UserType.moderator));
            //    users.Concat(result.chatters.staff.Select(t => new LX29_ChatClient.ChatUser(t, ChannelName, LX29_ChatClient.UserType.staff)));
            //    users.Concat(result.chatters.admins.Select(t => new LX29_ChatClient.ChatUser(t, ChannelName, LX29_ChatClient.UserType.admin)));
            //    users.Concat(result.chatters.global_mods.Select(t => new LX29_ChatClient.ChatUser(t, ChannelName, LX29_ChatClient.UserType.global_mod)));
            //    users.Concat(result.chatters.viewers.Select(t => new LX29_ChatClient.ChatUser(t, ChannelName, LX29_ChatClient.UserType.NORMAL)));

            //    return users;
            //}
            //return new List<LX29_ChatClient.ChatUser>();
            var users = values.GetBetween("moderators:[", "]").Split(",")
               .Select(t => new { UType = LX29_ChatClient.UserType.moderator, Name = t }).ToList();

            users.AddRange(values.GetBetween("staff:[", "]").Split(",")
                .Select(t => new { UType = LX29_ChatClient.UserType.staff, Name = t }));

            users.AddRange(values.GetBetween("admins:[", "]").Split(",")
                .Select(t => new { UType = LX29_ChatClient.UserType.admin, Name = t }));

            users.AddRange(values.GetBetween("global_mods:[", "]").Split(",")
                .Select(t => new { UType = LX29_ChatClient.UserType.global_mod, Name = t }));

            users.AddRange(values.GetBetween("viewers:[", "]").Split(",")
                .Select(t => new { UType = LX29_ChatClient.UserType.NORMAL, Name = t }));

            return users.Select(t => new User() { name = t.Name, usertype = t.UType });
        }

        private List<Sub> downloadFollows()
        {
            string url = "https://api.twitch.tv/kraken/channels/"
                + User.ID + "/follows?direction=desc";

            string delimiter = (url.Contains("?")) ? "&" : "?";
            string s = TwitchApi.downloadString(url + delimiter + "limit=100");

            int total = 0;
            string tt = s.GetBetween("\"_total\":", ",");
            int.TryParse(tt, out total);

            List<Sub> subs = new List<Sub>();
            var cfj = JsonConvert.DeserializeObject<ChannelSubJson>(s);
            subs.AddRange(cfj.follows);

            while (true)
            {
                if (string.IsNullOrEmpty(cfj._cursor))
                    break;

                s = TwitchApi.downloadString(url + delimiter + "limit=100&cursor=" + cfj._cursor);
                cfj = JsonConvert.DeserializeObject<ChannelSubJson>(s);
                subs.AddRange(cfj.follows);
                break;
            }
            //subs.ForEach(t => t.is_follow = true);
            return subs;
        }

        private List<Sub> downloadSubs()
        {
            string token = User.Token;
            string url = "https://api.twitch.tv/kraken/channels/"
                + User.ID + "/subscriptions?direction=desc";

            int total = 100;
            int limit = 100;
            //int offset = 0;
            string delimiter = (url.Contains("?")) ? "&" : "?";
            string s = TwitchApi.downloadString(url + delimiter + "limit=" + limit, token);
            string tt = s.GetBetween("\"_total\":", ",");
            int.TryParse(tt, out total);

            int parts = total / limit;

            List<Sub> subs = new List<Sub>();
            var csj = JsonConvert.DeserializeObject<ChannelSubJson>(s);
            subs.AddRange(csj.subscriptions);
            if (subs.Count >= limit)
            {
                //offset += subs.Count;
                Parallel.For(1, parts + 1, new Action<int>((i) =>
                {
                    s = TwitchApi.downloadString(url + delimiter + "limit=" + limit + "&offset=" + (limit * i), token);
                    var temp = JsonConvert.DeserializeObject<ChannelSubJson>(s).subscriptions;
                    if (temp.Count > 0)
                    {
                        //offset += temp.Count;
                        subs.AddRange(temp);
                    }
                    //else
                    //{
                    //    break;
                    //}
                    //if (subs.Count >= total)
                    //{
                    //    break;
                    //}
                }));
            }
            return subs;
        }
    }

    //public class FollowSub
    //{
    //    public string _id { get; set; }
    //    public DateTime created_at { get; set; }
    //    public bool is_follow { get; set; }
    //    public string sub_plan { get; set; }
    //    public string sub_plan_name { get; set; }
    //    public User user { get; set; }
    //}

    public class Sub
    {
        public string _id { get; set; }
        public DateTime created_at { get; set; }
        public string sub_plan { get; set; }
        public string sub_plan_name { get; set; }
        public User user { get; set; }
    }

    public class User
    {
        public int _id { get; set; }
        public DateTime created_at { get; set; }
        public string display_name { get; set; }

        public string DisplayName { get { return (display_name != null) ? display_name : name; } }
        public string logo { get; set; }

        public string name { get; set; }

        public DateTime updated_at { get; set; }

        public UserType usertype { get; set; }

        public void FromApiResult(ApiResult id)
        {
            _id = id.ID;
            created_at = id.GetValue<DateTime>(ApiInfo.created_at);
            updated_at = id.GetValue<DateTime>(ApiInfo.updated_at);
            display_name = id.GetValue<string>(ApiInfo.display_name);
        }
    }

    public class Viewer
    {
        public DateTime follow { get; set; }

        public bool is_follow { get { return follow.Ticks > 0; } }

        //public DateTime follow_created_at { get; set; }
        public bool is_sub { get { return sub != null; } }

        public Sub sub { get; set; }
        public User user { get; set; }
    }

    internal class DistinctItemComparer : IEqualityComparer<Sub>
    {
        public bool Equals(Sub x, Sub y)
        {
            return x.user._id == y.user._id;
        }

        public int GetHashCode(Sub obj)
        {
            return obj.user._id;
        }
    }
}