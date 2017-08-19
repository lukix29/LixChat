using LX29_Twitch.JSON_Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace LX29_Twitch.Api
{
    public static class TwitchApi
    {
        public const string CLIENT_ID = "n0soi61qkugd4mjlq618r243k686o69";

        private static Random rd = new Random();

        public static string AuthApiUrl
        {
            get
            {
                return "https://api.twitch.tv/kraken/oauth2/authorize?response_type=token&client_id="
                    + CLIENT_ID + "&redirect_uri=http://localhost:12685&force_verify=true&" +
                    "scope=chat_login+user_subscriptions+user_read+user_follows_edit";
            }
        }

        public static string User_ID
        {
            get { return LX29_ChatClient.ChatClient.TwitchUsers.Selected.ID; }
        }

        public static string User_Token
        {
            get { return LX29_ChatClient.ChatClient.TwitchUsers.Selected.Token; }
        }

        public static bool FollowChannel(ApiResult channelID, string userID = "", string userToken = "")
        {
            try
            {
                if (userID.IsEmpty()) userID = User_ID;
                if (userToken.IsEmpty()) userToken = User_Token;

                bool wasFollowed = channelID.Followed;
                string param = (wasFollowed) ? "DELETE" : "PUT";
                string raw = uploadString(
                    "https://api.twitch.tv/kraken/users/" + userID + "/follows/channels/" + channelID.ID, param, userToken);
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

        //public static JSON.Twitch_Badges.BadgeData GetChannelBadges(string channelID)
        //{
        //    string url = "https://api.twitch.tv/kraken/chat/" + channelID + "/badges";
        //    string s = downloadString(url, User_Token);

        //    return JSON.ParseBadges(s);
        //}

        public static Dictionary<string, LX29_ChatClient.ChatUser> GetChatUsers(string ChannelName)
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

            return users.Select(t => new LX29_ChatClient.ChatUser(t.Name, ChannelName, t.UType)).ToDictionary(t => t.Name);
        }

        public static List<ApiResult> GetFollowedStreams(string userID = "", string userToken = "")
        {
            if (userID.IsEmpty()) userID = User_ID;
            if (userToken.IsEmpty()) userToken = User_Token;

            List<ApiResult> list = getResults("https://api.twitch.tv/kraken/users/" + User_ID + "/follows/channels", User_Token);
            string sb = getChannelList(list);
            var str = getResults("https://api.twitch.tv/kraken/streams?channel=" + sb, User_Token);
            return Combine(list, str);
        }

        public static List<ApiResult> GetStreamOrChannel(params string[] Channel_ID)
        {
            Dictionary<ApiInfo, string> arr = new Dictionary<ApiInfo, string>();
            string url = "";

            var channels = getChannelList(Channel_ID);
            url = "https://api.twitch.tv/kraken/streams/?channel=" + channels + "&limit=100&stream_type=all";
            //url = "https://api.twitch.tv/kraken/streams/" + Channel_ID;
            var json = downloadString(url, User_Token);
            var res = JSON.Parse(json);

            var Rest = Channel_ID.Where(t => !res.Any(t0 => t0.ID.Equals(t))).ToArray();
            foreach (var id in Rest)
            {
                url = "https://api.twitch.tv/kraken/channels/" + id;
                json = downloadString(url, User_Token);
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
            string json = downloadString("https://api.twitch.tv/api/channels/" + ChannelName + "/panels", null, 3);
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

        public static SubResult GetSubscription(string channel_ID, string userID = "", string userToken = "")
        {
            if (userID.IsEmpty()) userID = User_ID;
            if (userToken.IsEmpty()) userToken = User_Token;
            var res = downloadString(
                "https://api.twitch.tv/kraken/users/" + userID + "/subscriptions/" + channel_ID, userToken, 5, false);
            return SubResult.Parse(res);
        }

        public static IEnumerable<JSON.Twitch_Api.Emoticon> GetUserEmotes(string userID = "", string userToken = "")
        {
            if (userID.IsEmpty()) userID = User_ID;
            if (userToken.IsEmpty()) userToken = User_Token;

            string s = downloadString("https://api.twitch.tv/kraken/users/" + userID + "/emotes", userToken);
            return JSON.ParseTwitchEmotes(s);
        }

        public static List<ApiResult> GetUserID(params string[] name)
        {
            string url = "https://api.twitch.tv/kraken/users?login=" + getChannelList(name);
            string id = downloadString(url);

            return JSON.Parse(id);
        }

        public static ApiResult GetUserID(string name)
        {
            string url = "https://api.twitch.tv/kraken/users?login=" + name;
            string id = downloadString(url);
            var res = JSON.Parse(id);
            if (res.Count > 0)
            {
                return res[0];
            }
            return ApiResult.Empty;
        }

        public static ApiResult GetUserIDFromToken(string Token)
        {
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            wc.Proxy = null;
            string result = wc.DownloadString("https://api.twitch.tv/kraken?oauth_token=" + Token);
            result = result.GetBetween("https://api.twitch.tv/kraken/users/", "\"");

            return GetUserID(result);
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

        private static string downloadString(string url, string tokken = null, int api_version = 5, bool handleError = true)
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
                        if (!tokken.IsEmpty())
                        {
                            webclient.Headers.Add("Authorization: OAuth " + tokken);
                        }
                    }
                    return webclient.DownloadString(url);
                }
            }
            catch (WebException x)
            {
                int code = 0;
                var info = TwitchApiErrors.GetError(x, out code);

                if (handleError)
                {
                    if (code == (int)HttpStatusCode.GatewayTimeout || code == (int)HttpStatusCode.RequestTimeout)
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

        private static string getChannelList(IEnumerable<string> results)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var a in results)
            {
                sb.Append(a + ",");
            }
            if (sb.Length > 0) sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        private static string getChannelList(IEnumerable<ApiResult> results)
        {
            var ids = results.Select(t => t.ID);
            return getChannelList(ids);
        }

        private static List<ApiResult> getResults(string Url, string token)
        {
            int total = 100;
            int limit = 100;
            int offset = 0;
            string s = downloadString(Url + ((Url.Contains("?") ? "&" : "?")) + "limit=" + limit + "&sortby=last_broadcast", token);
            string tt = s.GetBetween("\"_total\":", ",");
            int.TryParse(tt, out total);

            List<ApiResult> list = new List<ApiResult>();
            list.AddRange(JSON.Parse(s));
            if (list.Count >= limit)
            {
                offset += list.Count;
                while (true)
                {
                    s = downloadString(Url + ((Url.Contains("?") ? "&" : "?")) + "limit=" + limit + "&sortby=last_broadcast&offset=" + offset, token);
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
                        if (!tokken.IsEmpty())
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
                int code = 0;
                var info = TwitchApiErrors.GetError(x, out code);
                switch (x.Handle(info))
                {
                    case System.Windows.Forms.MessageBoxResult.Retry:
                        return uploadString(url, param, tokken, api_version);
                }
            }
            return null;
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