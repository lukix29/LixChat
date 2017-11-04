using LX29_Twitch.Api;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LX29_Twitch.JSON_Parser
{
    public static class JSON
    {
        public static T DeserializeObject<T>(string input)
        {
            return JsonConvert.DeserializeObject<T>(input);
        }

        public static List<ApiResult> Parse(string input)
        {
            try
            {
                List<ApiResult> lust = new List<ApiResult>();
                var dataSet = JsonConvert.DeserializeObject<Twitch_Api.Root>(input);

                if (dataSet.follows != null)
                {
                    foreach (var follow in dataSet.follows)
                    {
                        lust.Add(new ApiResult(follow));
                    }
                }
                else if (dataSet.users != null)
                {
                    foreach (var user in dataSet.users)
                    {
                        lust.Add(new ApiResult(user));
                    }
                }
                else if (dataSet.streams != null)
                {
                    foreach (var stream in dataSet.streams)
                    {
                        lust.Add(new ApiResult(stream));
                    }
                }
                else if (dataSet.stream != null)
                {
                    lust.Add(new ApiResult(dataSet.stream));
                }
                else if (dataSet.user != null)
                {
                    lust.Add(new ApiResult(dataSet.user));
                }
                else if (dataSet.follow != null)
                {
                    lust.Add(new ApiResult(dataSet.follow));
                }
                else if (!string.IsNullOrEmpty(dataSet._id))
                {
                    lust.Add(new ApiResult(dataSet));
                }
                else
                {
                }
                var list = lust.Where(t => !t.IsEmpty).ToList();

                return list;
            }
            catch (Exception x)
            {
                x.Handle("", false);
            }
            return null;
        }

        public static Twitch_Api.TokenBase ParseAuth(string input)
        {
            return JsonConvert.DeserializeObject<Twitch_Api.TokenBase>(input);
        }

        public static Twitch_Badges.BadgeData ParseBadges(string input)
        {
            return JsonConvert.DeserializeObject<Twitch_Badges.BadgeData>(input);
        }

        public static BTTV_Emotes ParseBttvEmotes(string input)
        {
            var dataSet = JsonConvert.DeserializeObject<BTTV_Emotes>(input);
            return dataSet;
        }

        public static Twitch_Api.ChatterList ParseChatters(string input)
        {
            try
            {
                var dataSet = JsonConvert.DeserializeObject<Twitch_Api.ChatterList>(input);
                return dataSet;
            }
            catch { }
            char c = input[41];
            return new Twitch_Api.ChatterList();
        }

        public static FFZ_Emotes.FFZ_Addon_Pack_Badges ParseFFZAddonBadges(string input)
        {
            return JsonConvert.DeserializeObject<FFZ_Emotes.FFZ_Addon_Pack_Badges>(input);
        }

        public static FFZ_Emotes.Badges ParseFFZBadges(string input)
        {
            return JsonConvert.DeserializeObject<FFZ_Emotes.Badges>(input);
        }

        public static List<FFZ_Emotes.Emoticon> ParseFFZEmotes(string input)
        {
            var lines = input.GetBetween("\"sets\":", "\"users\"").Split("\"icon\":")
                .Where(t => t.Contains("\"emoticons\":[")).Select(t => "[" + t.GetBetween("[", "]") + "]").ToArray();

            List<FFZ_Emotes.Emoticon> list = new List<FFZ_Emotes.Emoticon>();
            foreach (var line in lines)
            {
                try
                {
                    var emso = JsonConvert.DeserializeObject<FFZ_Emotes.Emoticon[]>(line);
                    list.AddRange(emso);
                }
                catch { }
            }
            return list;
        }

        public static List<PanelResult> ParsePanels(string input)
        {
            try
            {
                var panels = JsonConvert.DeserializeObject<List<Twitch_Api.Panel>>(input);
                List<PanelResult> list = new List<PanelResult>();
                foreach (var s in panels)
                {
                    list.Add(new PanelResult(s));
                }
                return list;
            }
            catch { }
            return null;
        }

        public static Twitch_Api.Subscription ParseSub(string input)
        {
            var dataSet = JsonConvert.DeserializeObject<Twitch_Api.Subscription>(input);
            return dataSet;
        }

        public static IEnumerable<Twitch_Api.Emoticon> ParseTwitchEmotes(string input)
        {
            try
            {
                input = input.RemoveStart("{\"emoticon_sets\":");
                if (input.EndsWith("}")) input = input.Remove(input.Length - 1);
                var list = JsonConvert.DeserializeObject<Dictionary<string, List<Twitch_Api.Emoticon>>>(input);
                //foreach (var e in list)
                //{
                //    string id = e.Value.id.ToString();
                //    if (LX29_ChatClient.Emotes.EmoteCollection.StandardEmotes.ContainsKey(id))
                //    {
                //        e.Value.code = LX29_ChatClient.Emotes.EmoteCollection.StandardEmotes[id];
                //    }
                //}
                //return list;
                return list.SelectMany(t => t.Value.Select(
                    i => new Twitch_Api.Emoticon { code = i.code, id = i.id, emoticon_set = t.Key }));
            }
            catch
            {
            }
            return null;
            //var sets = input.ReplaceAll("", "{\"emoticon_sets\":{", "}}").Split("],").Select(t => t + "]").ToArray();

            //List<LX29_Twitch.Api.EmoteApiInfo> emotes = new List<LX29_Twitch.Api.EmoteApiInfo>();
            //foreach (var s in sets)
            //{
            //    string setid = s.GetBetween("\"", "\"");

            //    var set = s.Trim("]");

            //    string arr = "[" + GetBetween(set, "[");

            //    if (!arr.EndsWith("]")) arr += "]";

            //    try
            //    {
            //        var array = JArray.Parse(arr);
            //        foreach (var n in array)
            //        {
            //            var emid = n.First.First.Value<int>();
            //            var emcode = n.Last.First.Value<string>();
            //            emotes.Add(new LX29_Twitch.Api.EmoteApiInfo(emcode, emid.ToString(), setid));
            //        }
            //    }
            //    catch { }
            //}
            //return emotes;
        }

        //private static string GetBetween(string input, string left, string right = "")
        //{
        //    try
        //    {
        //        int start = 0;
        //        if (!input.Contains(left))
        //        {
        //            return "";
        //        }

        //        start = Math.Max(0, Math.Min(input.Length - 1, start));

        //        string output = "";

        //        int i0 = input.IndexOf(left, start) + left.Length;
        //        int i1 = input.IndexOf(right, i0);
        //        if (right.Length > 0 && i1 >= 0)
        //        {
        //            if (i0 <= i1)
        //            {
        //                output = input.Substring(i0, (i1 - i0) - 1);
        //            }
        //        }
        //        else
        //        {
        //            output = input.Substring(i0);
        //        }
        //        return output;
        //    }
        //    catch { }
        //    return "";
        //}

        public class BTTV_Emotes
        {
            public List<object> bots { get; set; }

            public List<Emote> emotes { get; set; }

            public int status { get; set; }

            public string urlTemplate { get; set; }

            public class Emote
            {
                public string channel { get; set; }
                public string code { get; set; }
                public string id { get; set; }
                public string imageType { get; set; }
                public Restrictions restrictions { get; set; }
            }

            public class Restrictions
            {
                public List<object> channels { get; set; }
                public string emoticonSet { get; set; }
                public List<object> games { get; set; }
            }
        }

        public class FFZ_Emotes
        {
            public class Badge
            {
                public string color { get; set; }
                public object css { get; set; }
                public int id { get; set; }
                public string image { get; set; }
                public string name { get; set; }
                public string replaces { get; set; }
                public int slot { get; set; }
                public string title { get; set; }
                public Dictionary<string, string> urls { get; set; }

                public Dictionary<string, string> URLS
                {
                    get
                    {
                        return urls.ToDictionary(t => t.Key,
                            t0 => (t0.Value.StartsWith("https:") ? t0.Value : "https:" + t0.Value));
                    }
                }
            }

            public class Badges
            {
                public List<Badge> badges { get; set; }
                public Dictionary<int, List<string>> users { get; set; }
            }

            public class Emoticon
            {
                public bool @public { get; set; }
                public object css { get; set; }
                public int height { get; set; }
                public bool hidden { get; set; }
                public int id { get; set; }
                public object margins { get; set; }
                public bool modifier { get; set; }
                public string name { get; set; }
                public object offset { get; set; }
                public Owner owner { get; set; }

                public Dictionary<string, string> urls { get; set; }
                public int width { get; set; }
            }

            public class FFZ_Addon_Pack_Badges
            {
                public List<Badge> badges { get; set; }
                public List<FFZ_Addon_Pack_User> users { get; set; }
            }

            public class FFZ_Addon_Pack_User
            {
                public string badge_color { get; set; }
                public int level { get; set; }
                public string username { get; set; }

                public override string ToString()
                {
                    return username + " " + level + " " + badge_color;
                }
            }

            public class Owner
            {
                public int _id { get; set; }
                public string display_name { get; set; }
                public string name { get; set; }
            }
        }

        public class Twitch_Api
        {
            public class Authorization
            {
                public string created_at { get; set; }
                public List<string> scopes { get; set; }
                public string updated_at { get; set; }
            }

            public partial class Channel
            {
                [JsonProperty("_id")]
                public long _id { get; set; }

                [JsonProperty("broadcaster_language")]
                public string broadcaster_language { get; set; }

                [JsonProperty("broadcaster_type")]
                public string broadcaster_type { get; set; }

                [JsonProperty("created_at")]
                public string created_at { get; set; }

                [JsonProperty("description")]
                public string description { get; set; }

                [JsonProperty("display_name")]
                public string display_name { get; set; }

                [JsonProperty("followers")]
                public long followers { get; set; }

                [JsonProperty("game")]
                public string game { get; set; }

                [JsonProperty("language")]
                public string language { get; set; }

                [JsonProperty("logo")]
                public string logo { get; set; }

                [JsonProperty("mature")]
                public bool mature { get; set; }

                [JsonProperty("name")]
                public string name { get; set; }

                [JsonProperty("partner")]
                public bool partner { get; set; }

                [JsonProperty("profile_banner")]
                public string profile_banner { get; set; }

                [JsonProperty("profile_banner_background_color")]
                public string profile_banner_background_color { get; set; }

                [JsonProperty("status")]
                public string status { get; set; }

                [JsonProperty("updated_at")]
                public string updated_at { get; set; }

                [JsonProperty("url")]
                public string url { get; set; }

                [JsonProperty("video_banner")]
                public string video_banner { get; set; }

                [JsonProperty("views")]
                public long views { get; set; }
            }

            public class ChatterList
            {
                public int chatter_count { get; set; }
                public Dictionary<string, List<string>> chatters { get; set; }
            }

            public class Chatters
            {
                public List<string> admins { get; set; }
                public List<string> global_mods { get; set; }
                public List<string> moderators { get; set; }
                public List<string> staff { get; set; }
                public List<string> viewers { get; set; }
            }

            public class Emoticon
            {
                public string code { get; set; }
                public string emoticon_set { get; set; }
                public string id { get; set; }
            }

            public class Follow
            {
                public Channel channel { get; set; }
                public DateTime created_at { get; set; }
                public bool notifications { get; set; }
            }

            public class Panel
            {
                public int _id { get; set; }
                public string channel { get; set; }
                public PanelData data { get; set; }
                public int display_order { get; set; }
                public string html_description { get; set; }
                public string kind { get; set; }
                public int user_id { get; set; }
            }

            public class PanelData
            {
                public string description { get; set; }
                public string image { get; set; }
                public string link { get; set; }
                public string title { get; set; }
            }

            public partial class Preview
            {
                [JsonProperty("large")]
                public string large { get; set; }

                [JsonProperty("medium")]
                public string medium { get; set; }

                [JsonProperty("small")]
                public string small { get; set; }

                [JsonProperty("template")]
                public string template { get; set; }
            }

            public class Root
            {
                public int _total { get; set; }
                public Channel channel { get; set; }
                public List<Channel> channels { get; set; }
                public Follow follow { get; set; }
                public List<Follow> follows { get; set; }
                public Stream stream { get; set; }
                public List<Stream> streams { get; set; }
                public User user { get; set; }
                public List<User> users { get; set; }

                #region only

                public string _id { get; set; }
                public string broadcaster_language { get; set; }
                public string broadcaster_type { get; set; }
                //public string broadcaster_type { get; set; }

                public string created_at { get; set; }

                public string description { get; set; }

                public string display_name { get; set; }

                public int followers { get; set; }

                public string game { get; set; }

                public string language { get; set; }

                public string logo { get; set; }

                public bool mature { get; set; }

                public string name { get; set; }

                public bool partner { get; set; }

                public string profile_banner { get; set; }

                public object profile_banner_background_color { get; set; }

                public string status { get; set; }

                public string updated_at { get; set; }

                public string url { get; set; }

                public string video_banner { get; set; }

                public int views { get; set; }

                #endregion only
            }

            public partial class Stream
            {
                [JsonProperty("_id")]
                public long _id { get; set; }

                [JsonProperty("average_fps")]
                public double average_fps { get; set; }

                [JsonProperty("broadcast_platform")]
                public string broadcast_platform { get; set; }

                [JsonProperty("channel")]
                public Channel channel { get; set; }

                [JsonProperty("community_id")]
                public string community_id { get; set; }

                [JsonProperty("community_ids")]
                public string[] community_ids { get; set; }

                [JsonProperty("created_at")]
                public string created_at { get; set; }

                [JsonProperty("delay")]
                public long delay { get; set; }

                [JsonProperty("game")]
                public string game { get; set; }

                [JsonProperty("is_playlist")]
                public bool is_playlist { get; set; }

                [JsonProperty("preview")]
                public Preview preview { get; set; }

                [JsonProperty("stream_type")]
                public string stream_type { get; set; }

                [JsonProperty("video_height")]
                public long video_height { get; set; }

                [JsonProperty("viewers")]
                public long viewers { get; set; }
            }

            public class Subscription
            {
                public string _id { get; set; }
                public Channel channel { get; set; }
                public string created_at { get; set; }
                public string sub_plan { get; set; }
                public string sub_plan_name { get; set; }
                public User user { get; set; }
            }

            public class Token
            {
                public Authorization authorization { get; set; }
                public string client_id { get; set; }
                public string user_id { get; set; }
                public string user_name { get; set; }
                public bool valid { get; set; }
            }

            public class TokenBase
            {
                public string _id { get { return token.user_id; } }
                public string name { get { return token.user_name; } }
                public Token token { get; set; }
            }

            public class User
            {
                public int _id { get; set; }
                public DateTime created_at { get; set; }
                public string display_name { get; set; }
                public string logo { get; set; }
                public string name { get; set; }
                public string type { get; set; }
                public DateTime updated_at { get; set; }
            }
        }

        public class Twitch_Badges
        {
            public class Badge
            {
                public bool IsEnabled { get; set; }
                public string Name { get; set; }
            }

            public class BadgeData
            {
                public string Name { get; set; }
                public Dictionary<string, Data> versions { get; set; }
            }

            public class Data
            {
                public string click_action { get; set; }
                public string click_url { get; set; }
                public string description { get; set; }
                public string image_url_1x { get; set; }
                public string image_url_2x { get; set; }
                public string image_url_4x { get; set; }
                public string title { get; set; }

                public Dictionary<string, string> urls
                {
                    get
                    {
                        return new Dictionary<string, string>
                        {
                            {"1", image_url_1x },
                            {"2", image_url_2x },
                            {"3", image_url_4x }
                        };
                    }
                }
            }

            public class SavedBadge
            {
                public List<Badge> badges { get; set; }
                public int count { get; set; }
                public string created { get; set; }
            }
        }
    }
}