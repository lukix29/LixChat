using LX29_Twitch.Api;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

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

    public class DashboardData : IDisposable
    {
        public static readonly Dictionary<SubType, string> SubTierNames = new Dictionary<SubType, string>()
        {
            {SubType.NoSub, "None"},
            {SubType.Prime, "Prime"},
            {SubType.Tier1, "5$"},
            {SubType.Tier2, "10$"},
            {SubType.Tier3, "25$"}
        };

        public List<Tipeee.DonationHost> DonationHosts = new List<Tipeee.DonationHost>();

        private string summit

        {
            get
            {
#if DEBUG
                return "gronkh";
#else
                return Channel.Name;
#endif
            }
        }

        private string _tipeKey = "";

        public DashboardData()
        {
            Chatters = new Dictionary<string, Viewer>();
            ChatClient.OnUserNoticeReceived += ChatClient_OnUserNoticeReceived;

            var dt = Channel.GetValue<DateTime>(ApiInfo.stream_created_at);
            StreamStart = dt.Ticks > 0 ? dt : DateTime.MaxValue;
        }

        public delegate void UserNoticeReceived(NoticeMessage notice);

        public event UserNoticeReceived OnUserNoticeReceived;

        [JsonIgnore]
        public Channels.ChannelInfo Channel
        {
            get { return ChatClient.Channels[User.ID]; }
        }

        [JsonIgnore]
        public HashSet<NoticeMessage> ChatSubs
        {
            get
            {
                if (ChatClient.UserNotices.ContainsKey(summit))
                    return ChatClient.UserNotices[summit];
                else
                    return new HashSet<NoticeMessage>();
            }
        }

        [JsonIgnore]
        public int ChatterCount
        {
            get { return Chatters.Count; }
        }

        [JsonIgnore]
        public Dictionary<string, Viewer> Chatters
        {
            get;
            private set;
        }

        [JsonIgnore]
        public int DonationAmount
        {
            get { return DonationHosts.Where(t => !t.is_host).Sum(t => t.amount); }
        }

        [JsonIgnore]
        public IEnumerable<Tipeee.DonationHost> DonationSinceStreamStart
        {
            get
            {
                return DonationHosts
                  .Where(t => !t.is_host && t.created_at.Subtract(StreamStart).TotalSeconds > 0);
            }
        }

        [JsonIgnore]
        public int FollowCount
        {
            get { return Channel.ApiResult.GetValue<int>(ApiInfo.followers); }
        }

        [JsonIgnore]
        public int HostAmount
        {
            get { return DonationHosts.Where(t => t.is_host).Sum(t => t.amount); }
        }

        [JsonIgnore]
        public IEnumerable<Tipeee.DonationHost> HostSinceStreamStart
        {
            get
            {
                return DonationHosts
                  .Where(t => t.is_host && t.created_at.Subtract(StreamStart).TotalSeconds > 0);
            }
        }

        public bool IsTipeeeEnabled
        {
            get { { return !string.IsNullOrEmpty(_tipeKey); } }
        }

        [JsonIgnore]
        public int ReSubsSinceStreamStart
        {
            get
            {
                var ts = StreamStart;
                var sec = Subs.Count(t => (t.created_at.Subtract(ts).TotalSeconds > 0) && (t.sub_plan.Contains("resub")));
                return sec;
            }
        }

        public DateTime StreamStart
        {
            get;
            set;
        }

        [JsonIgnore]
        public int SubCount
        {
            get { return Subs.Count; }
        }

        [JsonIgnore]
        public List<Sub> Subs
        {
            get;
            private set;
        }

        [JsonIgnore]
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

        public string TipeeeKey
        {
            get { return _tipeKey; }
            set
            {
                _tipeKey = value;
            }
        }

        [JsonIgnore]
        public TwitchUser User
        {
            get { return TwitchUserCollection.Selected; }
        }

        [JsonIgnore]
        public HashSet<NoticeMessage> UserBans
        {
            get
            {
                if (ChatClient.UserBans.ContainsKey(summit))
                    return ChatClient.UserBans[summit];
                else
                    return new HashSet<NoticeMessage>();
            }
        }

        [JsonIgnore]
        public int ViewerCount
        {
            get { return Channel.ApiResult.GetValue<int>(ApiInfo.viewers); }
        }

        public static DashboardData Load(string file)
        {
            if (System.IO.File.Exists(file))
            {
                var json = LX29_Cryptography.LX29Crypt.Decrypt(System.IO.File.ReadAllBytes(file));
                var data = JsonConvert.DeserializeObject<DashboardData>(json);
                return data;
            }
            return new DashboardData();
        }

        public bool Dispose(bool b)
        {
            if (b)
            {
                Dispose();
            }
            return b;
        }

        public void Dispose()
        {
            ChatClient.OnUserNoticeReceived -= ChatClient_OnUserNoticeReceived;
        }

        public void GetSubscriptions()
        {
            try
            {
                TwitchUserCollection.CheckToken(false);
                Dictionary<string, Viewer> subs = new Dictionary<string, Viewer>();
                //var fols = downloadFollows();

                Subs = downloadSubs();
                //ysdfgh;
                var con = getsubs();

                //SubCount = con.Count;

                while (ChatClient.ChatUsers.Count(User.Name) == 0)
                {
                    Task.Delay(500).Wait();
                }

                var users = GetChatUsers(User.Name);
                List<string> noID = new List<string>();
                foreach (var u in users)
                {
                    Viewer v = Chatters.ContainsKey(u.name) ? Chatters[u.name] : new Viewer();
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
                if (ids != null)
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

        public void LoadTipeee()
        {
            try
            {
                if (string.IsNullOrEmpty(_tipeKey)) return;
                DonationHosts.Clear();
                //https://www.tipeeestream.com/dashboard/api-key
                string url = "https://api.tipeeestream.com/v1.0/events.json?apiKey=" + TipeeeKey + "&type[]=donation&type[]=hosting";
                using (var wc = new System.Net.WebClient())
                {
                    wc.Proxy = null;
                    string json = wc.DownloadString(url);
                    var donations = JsonConvert.DeserializeObject<Tipeee.JSON>(json);
                    foreach (var di in donations.datas.items)
                    {
                        Tipeee.DonationHost don = new Tipeee.DonationHost()
                        {
                            type = di.type,
                            amount = di.is_host ? di.parameters.viewers : di.amount,
                            created_at = di.created_at,
                            currency = di.is_host ? " Viewer" : di.parameters.currency,
                            message = di.parameters.message,
                            name = di.is_host ? di.parameters.hostname : di.parameters.username
                        };
                        if (!don.name.Equals(User.Name))
                        {
                            DonationHosts.Add(don);
                        }
                        else
                        {
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public void Save()
        {
            System.IO.File.WriteAllBytes(Settings._dataDir + "dashboard.json",
                LX29_Cryptography.LX29Crypt.Encrypt(JsonConvert.SerializeObject(this)));
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

        private void ChatClient_OnUserNoticeReceived(NoticeMessage notice)
        {
            //if (notice.IsSub)
            //{
            //    Sub sub = new Sub()
            //    {
            //        created_at = notice.CreatedAt,
            //        _id = notice.CreatedAt.Ticks.ToString(),
            //        sub_plan = Enum.GetName(typeof(SubType), notice.Type),
            //        sub_plan_name = notice.Value.ToString(),
            //        user = new User() { name = notice.Name }
            //    };
            //    //if (Subs == null) Subs = new List<Sub>();
            //    //Subs.Add(sub);
            //    //getsubs();
            //}
            OnUserNoticeReceived?.Invoke(notice);
        }

        private List<Sub> downloadFollows()

        {
            string url = "https://api.twitch.tv/kraken/channels/"
                + User.ID + "/follows?direction=desc";

            string delimiter = (url.Contains("?")) ? "&" : "?";
            string s = downloadString(url + delimiter + "limit=100");

            string tt = s.GetBetween("\"_total\":", ",");
            int.TryParse(tt, out int total);

            List<Sub> subs = new List<Sub>();
            var cfj = JsonConvert.DeserializeObject<ChannelSubJson>(s);
            subs.AddRange(cfj.follows);

            while (true)
            {
                if (string.IsNullOrEmpty(cfj._cursor))
                    break;

                s = downloadString(url + delimiter + "limit=100&cursor=" + cfj._cursor);
                cfj = JsonConvert.DeserializeObject<ChannelSubJson>(s);
                subs.AddRange(cfj.follows);
                break;
            }
            //subs.ForEach(t => t.is_follow = true);
            return subs;
        }

        private string downloadString(string url, string tokken = "")

        {
            try
            {
                using (var webclient = new WebClient())
                {
                    webclient.Proxy = null;
                    webclient.Encoding = System.Text.Encoding.UTF8;
                    webclient.Headers.Add("Accept: application/vnd.twitchtv.v5+json");
                    webclient.Headers.Add("Client-ID: " + TwitchApi.CLIENT_ID);
                    if (!string.IsNullOrEmpty(tokken)) webclient.Headers.Add("Authorization: OAuth " + tokken);

                    string s = webclient.DownloadString(url);
                    return s;
                }
            }
            catch
            {
            }
            return "";
        }

        private List<Sub> downloadSubs()

        {
            string token = User.Token;
            string url = "https://api.twitch.tv/kraken/channels/"
                + User.ID + "/subscriptions?direction=desc";

            int limit = 100;
            //int offset = 0;
            string delimiter = (url.Contains("?")) ? "&" : "?";
            string s = downloadString(url + delimiter + "limit=" + limit, token);
            string tt = s.GetBetween("\"_total\":", ",");
            int.TryParse(tt, out int total);

            int parts = total / limit;

            List<Sub> subs = new List<Sub>();
            var csj = JsonConvert.DeserializeObject<ChannelSubJson>(s);
            subs.AddRange(csj.subscriptions);
            if (subs.Count >= limit)
            {
                //offset += subs.Count;
                Parallel.For(1, parts + 1, new Action<int>((i) =>
                //for (int i = 1; i <= parts; i++)
                {
                    var st = TwitchApi.downloadString(url + delimiter + "limit=" + limit + "&offset=" + (i * limit), token);
                    var temp = JsonConvert.DeserializeObject<ChannelSubJson>(st).subscriptions;
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

        private Dictionary<string, Sub> getsubs()

        {
            Dictionary<string, Sub> con = new Dictionary<string, Sub>();
            foreach (var s in Subs)
            {
                if (con.ContainsKey(s.user.name))
                {
                    if (int.Parse(s.sub_plan) > int.Parse(con[s.user.name].sub_plan))
                    {
                        con[s.user.name] = s;
                    }
                }
                else
                {
                    con.Add(s.user.name, s);
                }
            }
            Subs = con.Values.ToList();
            return con;
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

    public class Tipeee
    {
        public class DonationHost
        {
            public int amount { get; set; }

            public DateTime created_at { get; set; }

            public string currency { get; set; }

            public bool is_host

            { get { return string.IsNullOrEmpty(message); } }

            public string message { get; set; }

            public string name { get; set; }

            public string type { get; set; }
        }

        //public class Host
        //{
        //    public DateTime created_at { get; set; }
        //}

        public class JSON
        {
            public Datas datas { get; set; }

            public string message { get; set; }

            public class Currency
            {
                public bool available { get; set; }

                public string code { get; set; }

                public string label { get; set; }

                public string symbol { get; set; }
            }

            public class Datas
            {
                public List<Item> items { get; set; }

                public int total_count { get; set; }
            }

            public class Item
            {
                private User _user = new User();

                public string @ref { get; set; }

                public int amount { get { return parameters.amount; } }

                public DateTime created_at { get; set; }

                public bool display { get; set; }

                public string formattedAmount { get; set; }

                public int id { get; set; }

                public DateTime inserted_at { get; set; }

                public bool is_host { get { return type.Equals("hosting"); } }

                public Parameters parameters { get; set; }

                public string type { get; set; }

                public User user { get; set; }
            }

            public class Parameters
            {
                public int amount { get; set; }

                public string currency { get; set; }

                public int fees { get; set; }

                public string formattedMessage { get; set; }

                public string hostname { get; set; }

                public string identifier { get; set; }

                public string message { get; set; }

                public int paypalCampaign { get; set; }

                public string username { get; set; }

                public int viewers { get; set; }
            }

            //public class Provider
            //{
            //    public string code { get; set; }
            //    public DateTime connectedAt { get; set; }
            //    public DateTime created_at { get; set; }
            //    public string id { get; set; }
            //    public DateTime last_follow_update { get; set; }
            //    public string username { get; set; }
            //}

            //public class User
            //{
            //    //public string avatar { get; set; }
            //    //public string country { get; set; }
            //    public DateTime created_at { get; set; }
            //    //public Currency currency { get; set; }
            //    //public DateTime hasPayment { get; set; }
            //    public int id { get; set; }
            //    //public List<Provider> providers { get; set; }
            //    //public string pseudo { get; set; }
            //    //public DateTime session_at { get; set; }
            //    public string username { get; set; }
            //}
        }
    }

    public class User
    {
        public int _id { get; set; }

        public DateTime created_at { get; set; }

        public string display_name { get; set; }

        public string DisplayName => display_name ?? name;

        public int id { get { return _id; } set { _id = value; } }

        public string logo { get; set; }

        public string name { get; set; }

        public DateTime updated_at { get; set; }

        public string username { get { return name; } set { name = value; } }

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
            return x.user._id == y.user._id && int.Parse(x.sub_plan) != int.Parse(y.sub_plan);
        }

        public int GetHashCode(Sub obj)
        {
            return obj.user._id;
        }
    }
}