using LX29_Twitch.JSON_Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;

namespace LX29_Twitch.Api
{
    public static class TwitchApi
    {
        public const string CLIENT_ID = "w828cjyt1gjkeravbpi2vu4s1k4831";//"n0soi61qkugd4mjlq618r243k686o69";
        //public const string CLIENT_ID = "n0soi61qkugd4mjlq618r243k686o69";

        private static string _uuid = null;
        private static Random rd = new Random();

        //public static string SessionID
        //{
        //    get { return LX29_ChatClient.ChatClient.TwitchUsers.Selected.SessionID; }
        //}

        public static int User_ID
        {
            get { return TwitchUserCollection.Selected.ID; }
        }

        public static string UUID
        {
            get
            {
                if (string.IsNullOrEmpty(_uuid))
                {
                    bool elec = System.IO.File.Exists("elec.txt");

                    _uuid = elec ? "electrinchen_test" : LX29_Tools.GetUniqueHash();
                }
                return _uuid;
            }
        }

        public static string AuthApiUrl(bool streamer)
        {
            return "https://api.lixchat.com/lix/authorize.php?guid=" + UUID + ((streamer) ? "scope=streamer" : "scope=viewer");

            // return "https://api.twitch.tv/kraken/oauth2/authorize?response_type=token&client_id=" + CLIENT_ID + "&redirect_uri=http://localhost:12685&force_verify=true&scope=chat_login+user_subscriptions+user_read+user_follows_edit";
        }

        public static async Task<JSON.ClipData> CreateClip(int id)
        {
            try
            {
                using (WebClient webclient = new WebClient())
                {
                    webclient.Proxy = null;
                    webclient.Headers.Add("Authorization: Bearer " + TwitchUserCollection.Selected.Token);
                    var result = await webclient.UploadStringTaskAsync("https://api.twitch.tv/helix/clips?broadcaster_id=" + id, string.Empty);
                    var json = Newtonsoft.Json.Linq.JObject.Parse(result);
                    var clipID = json["data"][0].Value<string>("id");
                    var clipURL = json["data"][0].Value<string>("edit_url");

                    webclient.Headers.Clear();
                    webclient.Headers.Add("Client-ID: " + CLIENT_ID);

                    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                    stopwatch.Start();
                    while (stopwatch.ElapsedMilliseconds <= 30000)
                    {
                        await Task.Delay(1000);
                        try
                        {
                            result = await webclient.DownloadStringTaskAsync("https://api.twitch.tv/helix/clips?id=" + clipID);

                            if (JSON.ParseClip(result, out var data))
                            {
                                return data;
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch (Exception x)
            { }
            return null;
        }

        public static string downloadString(string url, string tokken = null, int api_version = 5, bool handleError = true)
        {
            try
            {
                using (WebClient webclient = new WebClient())
                {
                    webclient.Proxy = null;
                    webclient.Encoding = Encoding.UTF8;
                    if (url.Contains("api.twitch.tv"))
                    {
                        webclient.Headers.Add("Accept: application/vnd.twitchtv.v" + api_version + "+json");
                        webclient.Headers.Add("Client-ID: " + CLIENT_ID);
                        if (!string.IsNullOrEmpty(tokken))
                        {
                            webclient.Headers.Add("Authorization: OAuth " + tokken);
                        }
                    }
                    return webclient.DownloadString(url);
                }
            }
            catch (WebException x)
            {
                var info = TwitchApiErrors.GetError(x, out int code);

                if (handleError)
                {
                    if (!string.IsNullOrEmpty(tokken))
                    {
                        if (code == (int)HttpStatusCode.NotFound)
                        {
                            TwitchUserCollection.RefreshSelectedToken(true);
                            return downloadString(url, tokken, api_version);
                        }
                    }
                    if (code == (int)HttpStatusCode.GatewayTimeout || code == (int)HttpStatusCode.RequestTimeout || code == (int)HttpStatusCode.ServiceUnavailable)
                    {
                        return downloadString(url, tokken, api_version);
                    }
                    else
                    {
                        var res = x.Handle(info);
                        switch (res)
                        {
                            case System.Windows.Forms.MessageBoxResult.Retry:
                                return downloadString(url, tokken, api_version);
                        }
                    }
                }
                else
                {
                    return code.ToString();
                }
            }
            return string.Empty;
        }

        public static bool FollowChannel(ApiResult channelID)
        {
            try
            {
                bool wasFollowed = channelID.Followed;
                string param = (wasFollowed) ? "DELETE" : "PUT";
                string raw = uploadString(
                    "https://api.twitch.tv/kraken/users/" + User_ID + "/follows/channels/" + channelID.ID, param, TwitchUserCollection.Selected.Token);
                if (!wasFollowed)
                {
                    if (raw.Length > 0)
                    {
                        channelID.Values[ApiInfo.follow] = true;
                    }
                }
                else
                {
                    channelID.Values[ApiInfo.follow] = false;
                }
                return true;
            }
            catch
            {
            }
            return false;
        }

        public static IEnumerable<LX29_ChatClient.ChatUser> GetChatUsers(string ChannelName)
        {
            string values = downloadString("https://tmi.twitch.tv/group/user/" + ChannelName + "/chatters").ToString()
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

            return users.Select(t => new LX29_ChatClient.ChatUser(t.Name, ChannelName, t.UType));
        }

        public static DateTime GetFollow(int channel, int user)
        {
            string json = downloadString("https://api.twitch.tv/kraken/users/" + user + "/follows/channels/" + channel, null, 5, false);
            if (!json.Equals("404"))
            {
                var result = JSON.DeserializeObject<JSON.Twitch_Api.Follow>(json);

                return result.created_at;
            }
            return DateTime.MinValue;
        }

        public static List<ApiResult> GetFollowedStreams()
        {
            List<ApiResult> list = getResults("https://api.twitch.tv/kraken/users/" + User_ID + "/follows/channels?sortby=last_broadcast", TwitchUserCollection.Selected.Token);
            string sb = getChannelList(list);
            var str = getResults("https://api.twitch.tv/kraken/streams?sortby=last_broadcast&channel=" + sb, TwitchUserCollection.Selected.Token);
            return Combine(list, str);
        }

        public static List<ApiResult> GetStreamOrChannel(params int[] Channel_ID)
        {
            Dictionary<ApiInfo, string> arr = new Dictionary<ApiInfo, string>();
            string url = "";

            var channels = getChannelList(Channel_ID.Select(t => t.ToString()));
            url = "https://api.twitch.tv/kraken/streams/?channel=" + channels + "&limit=100&stream_type=all";
            var json = downloadString(url, "");
            var res = JSON.Parse(json);

            var Rest = Channel_ID.Where(t => !res.Any(t0 => t0.ID.ToString().Equals(t))).ToArray();
            foreach (var id in Rest)
            {
                url = "https://api.twitch.tv/kraken/channels/" + id;
                json = downloadString(url, "");
                var temp_res = JSON.Parse(json);
                if (temp_res.Count > 0)
                {
                    res.Add(temp_res[0]);
                }
            }
            return res;
        }

        public static List<PanelResult> GetStreamPanels(string ChannelName)
        {
            string json = downloadString("https://api.twitch.tv/api/channels/" + ChannelName + "/panels", null, 5);
            var list = JSON.ParsePanels(json);
            return list.OrderByDescending(t => t.Index).ToList();
        }

        public static List<ApiResult> GetStreams(IEnumerable<ApiResult> res)
        {
            string sb = getChannelList(res);
            string s = downloadString("https://api.twitch.tv/kraken/streams?channel=" + sb + "&limit=100");
            var str = JSON.Parse(s);
            return str;
        }

        public static SubResult GetSubscription(int channel_ID)
        {
            var res = downloadString(
                "https://api.twitch.tv/kraken/users/" + User_ID + "/subscriptions/" + channel_ID, TwitchUserCollection.Selected.Token, 5, false);
            return SubResult.Parse(res);
        }

        public static IEnumerable<JSON.Twitch_Api.Emoticon> GetUserEmotes()
        {
            string s = downloadString("https://api.twitch.tv/kraken/users/" + User_ID + "/emotes", TwitchUserCollection.Selected.Token);
            return JSON.ParseTwitchEmotes(s);
        }

        public static List<ApiResult> GetUserID(params string[] name)
        {
            List<ApiResult> list = new List<ApiResult>();
            List<string> temp = new List<string>(name);
            while (list.Count < name.Length)
            {
                string url = "https://api.twitch.tv/kraken/users?login=" + getChannelList(temp.Take(Math.Min(temp.Count, 100)));
                string id = downloadString(url);
                temp.RemoveRange(0, Math.Min(temp.Count, 100));

                var t = JSON.Parse(id);
                if (t != null)
                {
                    list.AddRange(t);
                }
                else
                {
                    break;
                }
            }
            return list;
        }

        public static ApiResult GetUserID(string name)
        {
            string url = "https://api.twitch.tv/kraken/users?login=" + name;
            string id = downloadString(url);
            var res = JSON.Parse(id);
            if (res != null && res.Count > 0)
            {
                return res[0];
            }
            return ApiResult.Empty;
        }

        public static ApiResult GetUserIDFromSessionID(string sessionID, out string token)
        {
            token = TokenFromSessionID(sessionID);
            if (string.IsNullOrEmpty(token))
            {
                token = TokenFromSessionID(sessionID);
            }
            string result = downloadString("https://api.twitch.tv/kraken?oauth_token=" + token, null, 5);
            var res = JSON.ParseAuth(result);
            if (!res.token.valid)
            {
                //   new KeyNotFoundException().Handle("Error while getting Token from Session-ID", true);
                throw new NullReferenceException("GetUserIDFromSessionID");
            }

            return new ApiResult(res);
        }

        public static string TokenFromSessionID(string sessionID)
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    wc.Proxy = null;
                    string url = "https://api.lixchat.com/lix/users/token.php?sessionid=" + sessionID;
                    return wc.DownloadString(url);
                }
            }
            catch (WebException x)
            {
                x.Handle("Error while getting Token from Session-ID", true);
            }
            return null;
        }

        private static List<ApiResult> Combine(IEnumerable<ApiResult> channels, IEnumerable<ApiResult> streams)
        {
            List<ApiResult> list = new List<ApiResult>();
            foreach (var channel in channels)
            {
                var stream = streams.FirstOrDefault(t => t.ID.Equals(channel.ID));
                if (stream != null)
                {
                    list.Add(new ApiResult(channel, stream));
                }
                else
                {
                    list.Add(channel);
                }
            }

            return list;
        }

        private static string getChannelList(IEnumerable<string> results)

        {
            StringBuilder sb = new StringBuilder();
            foreach (var a in results)
            {
                if (!a.Equals("0"))
                    sb.Append(a + ",");
            }
            if (sb.Length > 0) sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        private static string getChannelList(IEnumerable<ApiResult> results)

        {
            var ids = results.Where(t => t.ID > 0).Select(t => t.ID.ToString());
            return getChannelList(ids);
        }

        private static List<ApiResult> getResults(string Url, string token)

        {
            int limit = 100;
            int offset = 0;
            string delimiter = (Url.Contains("?")) ? "&" : "?";
            string s = downloadString(Url + delimiter + "limit=" + limit, token);
            string tt = s.GetBetween("\"_total\":", ",");
            int.TryParse(tt, out int total);

            List<ApiResult> list = new List<ApiResult>();
            list.AddRange(JSON.Parse(s));
            if (list.Count >= limit)
            {
                offset += list.Count;
                while (true)
                {
                    s = downloadString(Url + delimiter + "limit=" + limit + "&offset=" + offset, token);
                    var temp = JSON.Parse(s);
                    if (temp.Count > 0)
                    {
                        offset += temp.Count;
                        list.AddRange(temp);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return list;
        }

        private static string uploadString(string url, string param, string tokken = null, int api_version = 5)

        {
            try
            {
                using (WebClient webclient = new WebClient())
                {
                    webclient.Proxy = null;
                    webclient.Encoding = Encoding.UTF8;
                    if (url.Contains("api.twitch.tv"))
                    {
                        webclient.Headers.Add("Accept: application/vnd.twitchtv.v" + api_version + "+json");
                        webclient.Headers.Add("Client-ID: " + CLIENT_ID);
                        if (!string.IsNullOrEmpty(tokken))
                        {
                            webclient.Headers.Add("Authorization: OAuth " + tokken);
                        }
                    }
                    return webclient.UploadString(url, param, "");
                    //  return webclient.DownloadString(url);
                }
            }
            catch (WebException x)
            {
                var info = TwitchApiErrors.GetError(x, out int code);
                switch (x.Handle(info))
                {
                    case System.Windows.Forms.MessageBoxResult.Retry:
                        return uploadString(url, param, tokken, api_version);
                }
            }
            return null;
        }

        private class follow
        {
            public DateTime created_at { get; set; }
            public bool notifications { get; set; }
        }
    }

    public class TwitchApiErrors
    {
        private static Dictionary<int, string> dict = new Dictionary<int, string>()
        {
            { -10, "Generic Error."},
            { 400, "Request Not Valid. Something is wrong with the request."},
            { 401, "Unauthorized. The OAuth token does not have the correct scope or does not have the required permission on behalf of the specified user."},
            { 403, "Forbidden. This usually indicates that authentication was provided, but the authenticated user is not permitted to perform the requested operation.\r\nFor example, a user who is not a partner might have tried to start a commercial."},
            { 404, "Not Found. For example, the channel, user, or relationship could not be found."},
            { 422, "Unprocessable Entity. For example, for a user subscription endpoint, the specified channel does not have a subscription program."},
            { 429, "Too Many Requests."},
            { 500, "Internal Server Error."},
            { 503, "Service Unavailable. For example, the status of a game or ingest server cannot be retrieved."}
        };

        public static string GetError(WebException x, out int type)
        {
            type = 0;
            if (x.Response != null)
            {
                var res = x.Response as HttpWebResponse;
                int code = (int)res.StatusCode;
                type = code;
                if (dict.ContainsKey(code))
                {
                    return dict[code];
                }
            }
            return dict[-10];
        }
    }
}