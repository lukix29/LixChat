using IRC_Client;
using IRC_Client.Events;
using LX29_ChatClient.Addons;
using LX29_ChatClient.Channels;
using LX29_ChatClient.Emotes;
using LX29_Twitch.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LX29_ChatClient
{
    public static partial class ChatClient
    {
        public delegate void _JoinedChannel(string channel);

        public delegate void _PartedChannel(string channel);

        public delegate void _ReceivedHandler(ChatMessage message);

        public delegate void _TimeoutHandler(TimeOutResult result);

        public delegate void LoadedList(int count, int max, string info);

        public static event LoadedList ListLoaded;

        public static event _JoinedChannel OnChannelJoined;

        public static event _PartedChannel OnChannelParted;

        public static event _ReceivedHandler OnMessageReceived;

        public static event _TimeoutHandler OnTimeout;

        public static void ListUpdated()
        {
            if (ListLoaded != null)
                ListLoaded(0, 0, "");
        }

        private static void ChannelJoined(string channel)
        {
            if (OnChannelJoined != null)
                OnChannelJoined(channel);
        }

        private static void ChannelParted(string channel)
        {
            if (OnChannelParted != null)
                OnChannelParted(channel);
        }

        private static void MessageReceived(ChatMessage message)
        {
            if (OnMessageReceived != null)
                OnMessageReceived(message);
        }

        private static void UserHasTimeouted(TimeOutResult result)
        {
            if (OnTimeout != null)
                OnTimeout(result);
        }
    }

    public static partial class ChatClient
    {
        public const string ACTION = "ACTION";
        public const string CLEARCHAT = "CLEARCHAT";
        public const string GLOBALUSERSTATE = "GLOBALUSERSTATE";
        public const string NOTICE = "NOTICE";
        public const string PRIVMSG = "PRIVMSG";
        public const string ROOMSTATE = "ROOMSTATE";
        public const string USERNOTICE = "USERNOTICE";
        public const string USERSTATE = "USERSTATE";
        public const string WHISPER = "WHISPER";

        private const string BAN = "@ban-reason=";

        private const string TO = "@ban-duration=";

        private static msg_ids ParseMsgID(Dictionary<irc_params, string> parameters, string channelName, string raw)
        {
            if (parameters.ContainsKey(irc_params.msg_id))
            {
                msg_ids id = msg_ids.NONE;
                if (Enum.TryParse<msg_ids>(parameters[irc_params.msg_id], out id))
                {
                    if (Enum.IsDefined(typeof(channel_mode), (int)id))
                    {
                        channel_mode mode = (channel_mode)id;
                    }
                    var channel = channels[channelName];
                    channel.Modes.SetMode(id, raw);
                    switch (id)
                    {
                        case msg_ids.slow_on:
                            {
                                var time = raw.GetBefore(" seconds", " ");
                                int i = 0;
                                if (int.TryParse(time, out i))
                                {
                                    channel.SlowMode = i;
                                }
                            }
                            break;

                        case msg_ids.slow_off:
                            channel.SlowMode = 0;
                            break;
                    }
                    return id;
                }
            }
            return msg_ids.NONE;
        }

        private static Dictionary<irc_params, string> ParseParams(string raw, string spliType)
        {
            Dictionary<irc_params, string> parameters = new Dictionary<irc_params, string>();
            raw = raw.Remove(0, 1);
            raw = raw.Remove(raw.IndexOf(spliType)).Trim();
            int idpp = raw.LastIndexOf(":");
            if (idpp > 0)
            {
                raw = raw.Remove(idpp).Trim();
            }
            string[] saParm = raw.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string param in saParm)
            {
                string[] sa = param.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (sa.Length > 0)
                {
                    irc_params para;
                    string sp = sa[0].Replace("-", "_").Trim();
                    if (Enum.TryParse<irc_params>(sp, out para))
                    {
                        string si = "";
                        if (sa.Length > 1)
                        {
                            si = sa[1].Replace(" ", "\\s").Trim();
                        }
                        parameters.Add(para, si);
                    }
                }
            }
            return parameters;
        }

        private static bool TryParseRawMessage(string raw)//, out ChatUser[] users)
        {
            try
            {
                if (TryParseUserList(raw))
                {
                    return true;
                }
                string spliType = "";
                if (raw.ContainsAny(out spliType, false, ROOMSTATE, WHISPER, USERNOTICE, USERSTATE, PRIVMSG, CLEARCHAT, NOTICE, GLOBALUSERSTATE))
                {
                    string Name = "";
                    string Message = "";
                    //bool isTO = false;
                    string channelName = raw.GetBetween(spliType + " #", " :");
                    Dictionary<irc_params, string> parameters = ParseParams(raw, spliType);

                    msg_ids msgid = ParseMsgID(parameters, channelName, raw);

                    TimeOutResult tor = TimeOutResult.Empty;
                    switch (spliType)
                    {
                        case ROOMSTATE:
                            break;

                        case GLOBALUSERSTATE:
                            //File.AppendAllText("GLOBALUSERSTATE.txt", raw + "\r\n");
                            break;

                        case CLEARCHAT:
                            {
                                //@ban-duration=5;ban-reason=blacklisted\sspam,\sautomated\sby\sNightbot;room-id=112866535;target-user-id=37167762 :tmi.twitch.tv CLEARCHAT #food :toranbmac
                                //Ban Raw:
                                //@ban-reason=;room-id=79328905;target-user-id=154057194 :tmi.twitch.tv CLEARCHAT #lukix29 :plebx29
                                var name = raw.GetBefore("", ":");
                                tor = TimeOutResult.Parse(raw, name, channelName, parameters);
                                if (!tor.IsEmpty)
                                {
                                    Message = tor.Message;
                                    UserHasTimeouted(tor);
                                }
                            }
                            break;

                        case USERSTATE:
                            {
                                Name = "";
                                Message = "";
                            }
                            break;

                        case NOTICE:
                            {
                                //"@msg-id=host_on :tmi.twitch.tv NOTICE #inzaynia :Now hosting a_couple_streams."
                                Name = "";// raw.GetBetween("#", " ").Trim();
                                Message = raw.Substring("#" + channelName + " :");

                                //File.AppendAllText("NOTICE.txt", raw + "\r\n");
                            }
                            break;

                        case USERNOTICE:
                            {
                                Name = raw.GetBetween("login=", ";");
                                Message = raw.Substring("#" + channelName + " :");

                                string system_msg = "";
                                if (parameters.ContainsKey(irc_params.system_msg))
                                {
                                    system_msg = parameters[irc_params.system_msg].Replace("\\s", " ");
                                }
                                else
                                {
                                    string subType = "";
                                    string msg_id = "";
                                    string months = "";

                                    if (parameters.ContainsKey(irc_params.msg_id))
                                    {
                                        msg_id = parameters[irc_params.msg_id].Replace("_", " ").ToTitleCase();
                                    }
                                    if (parameters.ContainsKey(irc_params.msg_param_months))
                                    {
                                        months = parameters[irc_params.msg_param_months];
                                    }
                                    if (parameters.ContainsKey(irc_params.msg_param_sub_plan))
                                    {
                                        SubType st = SubType.None;
                                        if (Enum.TryParse<SubType>(parameters[irc_params.msg_param_sub_plan], out st))
                                        {
                                            subType = Enum.GetName(typeof(SubType), st);
                                        }
                                    }
                                    system_msg = subType + " " + msg_id + " " + months + "x";
                                }
                                SendSilentMessage(system_msg, channelName, MsgType.UserNotice);
                            }
                            break;

                        case WHISPER:
                            {
                                Name = raw.GetBetween(raw.LastIndexOf("user-type="), ":", "!");
                                Message = raw.GetBetween(raw.LastIndexOf(spliType), ":", "");
                                channelName = Name;
                            }
                            break;

                        default:
                            {
                                Name = raw.GetBetween(raw.LastIndexOf("user-type="), ":", "!");
                                int ms = (raw.LastIndexOf(spliType + " #" + channelName) + spliType.Length + channelName.Length + 4);
                                if (ms < raw.Length)
                                {
                                    Message = raw.Substring(ms).Trim();
                                }
                                //File.AppendAllText("DEFAULT.txt", raw + "\r\n");
                            }
                            break;
                    }

                    if (string.IsNullOrEmpty(Name))
                    {
                        if (parameters.ContainsKey(irc_params.display_name))
                        {
                            Name = parameters[irc_params.display_name].ToLower();
                        }
                    }

                    users.Add(channelName, parameters, Name, tor);
                    messages.Add(channelName, parameters, Name, Message, spliType, tor);
                }
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        TryParseRawMessage(raw);
                        break;
                }
            }
            return false;
        }

        private static bool TryParseUserList(string raw)
        {
            if (raw.StartsWith(":"))
            {
                if (raw.Contains("353"))
                {
                    //Dictionary<string, ChatUser> list = new Dictionary<string, ChatUser>();
                    string channel = raw.GetBetween("#", " ").ToLower();
                    string n = raw.Substring(raw.LastIndexOf(':') + 1);
                    string[] names = n.Split(" ");
                    foreach (string s in names)
                    {
                        users.Add(s, channel);
                    }
                }
                else if (raw.Contains("jtv MODE"))
                {
                    string channel = raw.GetBetween("#", " ").ToLower();
                    string name = raw.Split(" ").Last();
                    users.Add(name, channel);
                }
            }
            return false;
        }
    }

    public static partial class ChatClient
    {
        private static TwitchUserCollection _twitchUsers = new TwitchUserCollection(Settings.dataDir + "auth.txt");

        public static ApiResult SelfApiResult
        {
            get { return _twitchUsers.Selected; }
        }

        public static string SelfUserName
        {
            get { return _twitchUsers.Selected.Name; }
        }

        public static string SelfUserToken
        {
            get { return _twitchUsers.Selected.Token; }
        }

        public static TwitchUserCollection TwitchUsers
        {
            get { return _twitchUsers; }
        }
    }

    public static partial class ChatClient
    {
        //private static int connectCnt = 0;

        private static DateTime lastSend = DateTime.MinValue;
        private static Dictionary<string, object> lockChannels = new Dictionary<string, object>();

        private static int reconectTimeout = 8000;
        private static SortMode[] sortArr;

        public static void Disconnect(IRC client)
        {
            while (!client.NetworkErrorIsNull)
                client.NetworkError -= client_NetworkError;

            while (!client.ConnectionCompleteIsNull)
                client.ConnectionComplete -= client_ConnectionComplete;

            while (!client.UserJoinedChannelIsNull)
                client.UserJoinedChannel -= client_UserJoinedChannel;

            while (!client.UserKickedIsNull)
                client.UserKicked -= client_UserKicked;

            while (!client.RawMessageRecievedIsNull)
                client.RawMessageRecieved -= client_RawMessageRecieved;

            client.Quit();
            client.Dispose();

            while (!client.UserPartedChannelIsNull)
                client.UserPartedChannel -= client_UserPartedChannel;

            while (!client.UserQuitIsNull)
                client.UserQuit -= client_UserQuit;
        }

        public static void Disconnect(string channel)
        {
            try
            {
                channel = channel.ToLower().Trim();
                if (clients.ContainsKey(channel))
                {
                    var ci = clients[channel];

                    Disconnect(ci);

                    clients.Remove(channel);
                }
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        Disconnect(channel);
                        break;
                }
            }
        }

        public static void DisconnectAll()
        {
            //stop = true;
            Task.Run(() =>
                {
                    var sa = clients.Values.Select(t => t.Channel).ToArray();
                    foreach (var s in sa)
                    {
                        if (!channels[s].IsFixed)
                        {
                            Disconnect(s);
                        }
                    }
                });
        }

        public static ChannelInfo[] GetSortedChannelNames()
        {
            try
            {
                if (channels.Count > 0)
                {
                    //if (isSnycing) return null;
                    var ordered = channels.Values.OrderByDescending(t => t.StreamType);
                    for (int y = 0; y < sortArr.Length; y++)
                    {
                        switch (sortArr[y])
                        {
                            case SortMode.Viewing:
                                ordered = ordered.ThenByDescending(t => t.IsViewing);
                                break;

                            case SortMode.Favorite:
                                ordered = ordered.ThenByDescending(t => t.IsFavorited);
                                break;

                            case SortMode.Chatlogin:
                                ordered = ordered.ThenByDescending(t => t.IsChatConnected);
                                break;

                            case SortMode.Sub:
                                ordered = ordered.ThenByDescending(t => t.SubInfo.IsSub);
                                break;
                        }
                    }
                    var arr = ordered.ToArray();
                    return arr;
                }
            }
            catch
            {
                if (channels.Count > 0)
                {
                    return channels.Values.ToArray();
                }
            }
            return null;
        }

        public static bool HasJoined(string channel)
        {
            return clients.ContainsKey(channel);
        }

        public static ChatMessage SendMessage(string Message, string channel)
        {
            channel = channel.ToLower().Trim();
            if (clients.ContainsKey(channel))
            {
                return SendMessage(Message, null, clients[channel]);
            }
            return ChatMessage.Empty;
        }

        /// <summary>
        /// Sends a Message only to the Chatbox, NOT to Twitch IRC
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="Channel"></param>
        /// <param name="WithUserName">Show Username "00:00:00 Name: Message" (else "00:00:00 Message")</param>
        /// <returns></returns>
        public static bool SendSilentMessage(string Message, string Channel, params MsgType[] types)
        {
            try
            {
                Message = Message.ReplaceAll(" ", "  ", "\t", "\r", "\n", "\0").Trim();
                if (Message.Length > 0)
                {
                    if (Message.StartsWith("/"))
                    {
                        Message = "." + Message.Substring(1);
                    }
                    if (Message.Length > 500)
                    {
                        Message = Message.Remove(500);
                    }
                    Channel = Channel.ToLower().Trim();

                    messages.Add(Channel, Message, types);
                    return true;
                }
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        return SendSilentMessage(Message, Channel);
                }
            }
            return false;
        }

        public static ChatMessage SendWhisper(string Message, string name)
        {
            Message = Message.ReplaceAll(" ", "  ", "\t", "\r", "\n", "\0").Trim();
            string rawMessage = (Message.StartsWith("/w")) ? Message.RemoveUntil(" ", Message.IndexOf(" ") + 1) : Message;
            try
            {
                if (Message.Length > 0)
                {
                    string wi = (Message.StartsWith("/w")) ? "" : "/w " + name + " ";
                    Message = wi + Message;
                    //string Channel = clients.Keys.First();
                    string username = SelfUserName.ToLower();// ChatClient.SelfApiResult.GetValue<string>(LX29_TwitchApi.ApiInfo.name);
                    ChatUser me = ChatClient.Users.Get(username, "");
                    ChatMessage m = new ChatMessage(rawMessage, me, name, true);
                    m.Types.Add(MsgType.Whisper);
                    var client = clients.First().Value;
                    client.SendMessage(Message, false);
                    messages.AddWhisper(name, m);
                    LogMessage(name, rawMessage);
                    return m;
                }
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        return SendWhisper(rawMessage, name);
                }
            }
            return ChatMessage.Empty;
        }

        public static void TryConnect(string channel, bool force = false)
        {
            try
            {
                channel = GetOnlyName(channel).ToLower().Trim();
                lock (channels[channel].LockObject)
                {
                    if (!clients.ContainsKey(channel) || force)
                    {
                        connect(channel);
                    }
                }
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        TryConnect(channel, force);
                        break;
                }
            }
            //connectCnt = Math.Max(0, connectCnt - 1);
        }

        private static void client_ConnectionComplete(object sender, EventArgs e)
        {
            IRC client = (IRC)sender;

            //var info = channels[client.Channel];

            client.RawMessageRecieved += client_RawMessageRecieved;
        }

        private static void client_NetworkError(object sender, Exception e)
        {
            try
            {
                //var ex = e as System.Net.Sockets.SocketException;
                //if (ex != null)
                //// if (!(ex.ErrorCode == 10057))
                //{
                IRC c = (IRC)sender;

                c.Quit();
                Task.Run(async () =>
                {
                    SendSilentMessage("Error Conecting to Chat, reconecting in 2s!", c.Channel);
                    await Task.Delay(2000);
                    SendSilentMessage("Reconecting now!", c.Channel);
                    Disconnect(c);
                    connect(c.Channel);
                });

                ListUpdated();
                //}
            }
            catch (Exception x) { x.Handle(); }
        }

        private static void client_RawMessageRecieved(object sender, RawMessageEventArgs e)
        {
            IRC c = (IRC)sender;
            try
            {
                //if (e.Message.Contains("JOIN") && e.Message.Contains(SelfUserName))
                {
                    if (!clients.ContainsKey(c.Channel))
                    {
                        c.NetworkError += client_NetworkError;
                        c.UserJoinedChannel += client_UserJoinedChannel;
                        c.UserKicked += client_UserKicked;
                        c.UserPartedChannel += client_UserPartedChannel;
                        c.UserQuit += client_UserQuit;

                        Task.Run(() => Emotes.LoadChannelEmotes(c.Channel));

                        clients.Add(c.Channel, c);

                        Task.Run(() => FetchChatUsers(c.Channel));
                        ListUpdated();
                    }
                }
            }
            catch (Exception x)
            {
                x.Handle();
            }
            TryParseRawMessage(e.Message);
            LogMessage(c.Channel, e.Message);
        }

        private static void client_UserJoinedChannel(object sender, IrcChannelUserEventArgs e)
        {
            if (e.User.Nick.Equals(SelfUserName, StringComparison.OrdinalIgnoreCase))
            {
                ChannelJoined(e.Channel);
            }
            users.Add(e.User.Nick, e.Channel);
        }

        private static void client_UserKicked(object sender, IrcChannelUserEventArgs e)
        {
            if (e.User.Nick.Equals(SelfUserName, StringComparison.OrdinalIgnoreCase))
            {
                Disconnect(e.Channel);
                ChannelParted(e.Channel);
                ListUpdated();
            }
            users.SetOffline(e.User.Nick, e.Channel);
        }

        private static void client_UserPartedChannel(object sender, IrcChannelUserEventArgs e)
        {
            if (e.User.Nick.Equals(SelfUserName, StringComparison.OrdinalIgnoreCase))
            {
                Disconnect(e.Channel);
                ChannelParted(e.Channel);
                ListUpdated();
            }
            users.SetOffline(e.User.Nick, e.Channel);
        }

        private static void client_UserQuit(object sender, IrcChannelUserEventArgs e)
        {
            if (e.User.Nick.Equals(SelfUserName, StringComparison.OrdinalIgnoreCase))
            {
                Disconnect(e.Channel);
                ChannelParted(e.Channel);
                ListUpdated();
            }
            users.SetOffline(e.User.Nick, e.Channel);
        }

        private static void connect(string channel)
        {
            Disconnect(channel);

            IRC client = new IRC("irc.twitch.tv", channel, SelfUserName, SelfUserToken);

            client.ConnectionComplete += client_ConnectionComplete;

            client.ConnectAsync();

            Task.Run(async () =>
            {
                await Task.Delay(reconectTimeout);
                if (!clients.ContainsKey(client.Channel))
                {
                    reconectTimeout = Math.Min(32000, reconectTimeout * 2);
                    SendSilentMessage("Chat Conecting Timeout (" + (reconectTimeout / 1000) + "s)!", channel);
                    Disconnect(client);
                    connect(client.Channel);
                }
                else
                {
                    reconectTimeout = 8000;
                }
            });
        }

        private static void FetchChatUsers(string channelName)
        {
            var cus = TwitchApi.GetChatUsers(channelName);
            var cur = users.Get(channelName);

            var diff = cus.Keys.Except(cur.Keys).Select(t => cus[t]);
            var diff2 = cur.Keys.Except(cus.Keys).Select(t => cur[t]);
            foreach (var user in diff)
            {
                users.Add(user);
            }
            foreach (var user in diff2)
            {
                user.IsOnline = false;
            }
        }

        private static void LogMessage(ChannelInfo channel, string message)
        {
            if (channel.LogChat)
            {
                File.AppendAllText(Settings.chatLogDir + channel.Name.ToLower() + ".log", message + "\r\n");
            }
        }

        private static void LogMessage(string channel, string message)
        {
            try
            {
                if (channels.ContainsKey(channel))
                {
                    LogMessage(channels[channel], message);
                }
            }
            catch
            {
            }
        }

        private static ChatMessage SendMessage(string Message, string user, IRC_Client.IRC irc)
        {
            if (string.IsNullOrEmpty(Message) ||
                DateTime.Now.Subtract(lastSend).TotalSeconds < 1.0) return ChatMessage.Empty;

            lastSend = DateTime.Now;
            string Channel = irc.Channel;
            try
            {
                var channelInfo = channels[Channel];
                if (Message.StartsWith("/w"))
                {
                    return SendWhisper(Message, Message.GetBetween("/w ", " "));
                }
                Message = Message.ReplaceAll(" ", "  ", "\t", "\r", "\n", "\0").Trim();
                if (Message.Length > 0)
                {
                    if (Message.StartsWith("/"))
                    {
                        Message = "." + Message.Substring(1).Trim();
                    }
                    if (Message.Length > 500)
                    {
                        Message = Message.Remove(500);
                    }

                    user = (string.IsNullOrEmpty(user) ? SelfUserName.ToLower() : user);
                    ChatUser me = ChatClient.Users.Get(user, Channel);

                    bool mod = me.Types.Any(t => (((int)t) >= (int)UserType.moderator));

                    TimeSpan sec = DateTime.Now.Subtract(channelInfo.LastSendMessageTime);
                    if (!channelInfo.HasSlowMode ||
                        sec.TotalSeconds > channelInfo.SlowMode)
                    {
                        channelInfo.LastSendMessageTime = DateTime.Now;

                        ChatMessage m = new ChatMessage(Message, me, Channel, true);
                        messages.Add(Channel, m, true);
                        LogMessage(Channel, Message);

                        if (!irc.SendMessage(Message, true, mod))
                        {
                            me = ChatUser.Emtpy;
                            Message = "Message was NOT sent! Automatic Global-Ban-Protection.";
                            m = new ChatMessage(Message, me, Channel, false);
                            messages.Add(Channel, m, false);
                        }
                        return m;
                    }
                }
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        return SendMessage(Message, user, irc);
                }
            }
            return ChatMessage.Empty;
        }
    }

    public static partial class ChatClient
    {
        public class MessageCollection// : IDictionary<string, Dictionary<string, ChatUser>>
        {
            private int maxMessages = UInt16.MaxValue;

            private Dictionary<string, int> messageCount = new Dictionary<string, int>();

            private Dictionary<string, List<ChatMessage>> messages = new Dictionary<string, List<ChatMessage>>();

            private Dictionary<string, List<ChatMessage>> whisper = new Dictionary<string, List<ChatMessage>>();

            public delegate void WhisperReceivedHandler(ChatMessage message);

            public event WhisperReceivedHandler OnWhisperReceived;

            public Dictionary<string, List<ChatMessage>> Whispers
            {
                get { return whisper; }
            }

            public List<ChatMessage> this[string channel, string name, MsgType type]
            {
                get
                {
                    channel = channel.ToLower().Trim();
                    if (string.IsNullOrEmpty(name))
                    {
                        if (messages.ContainsKey(channel))
                        {
                            if (type == MsgType.All_Messages)
                            {
                                return messages[channel];
                            }
                            else
                            {
                                return messages[channel].Where(t => t.IsType(type)).ToList();
                            }
                        }
                    }
                    else
                    {
                        name = name.ToLower().Trim();
                        if (whisper.ContainsKey(name))
                        {
                            return whisper[name];
                        }
                        else
                        {
                            return messages[channel].Where(t => t.Name.Equals(name)).ToList();
                        }
                    }
                    return null;
                }
            }

            public void Add(string Channel, string Message, params MsgType[] types)
            {
                Channel = Channel.ToLower().Trim();
                ChatMessage m = new ChatMessage(Message, ChatUser.Emtpy, Channel, true, types);
                Add(Channel, m, false);
            }

            public void Add(string channelName, ChatMessage msg, bool executeActions)
            {
                try
                {
                    if (msg.IsEmpty) return;
                    lock (syncRootMessage)
                    {
                        if (msg.IsType(MsgType.Whisper))
                        {
                            AddWhisper(channelName, msg);
                        }
                        else
                        {
                            if (!messages.ContainsKey(channelName))
                            {
                                AddChannel(channelName);
                            }
                            messages[channelName].Add(msg);
                            messageCount[channelName]++;

                            while (messages.Count > maxMessages)
                            {
                                messages[channelName].RemoveAt(0);
                            }
                        }
                    }

                    if (executeActions)
                    {
                        if (AutoActions.EnableActions)
                        {
                            AutoActions.CheckActions(msg);
                        }

                        LX29_ChatClient.Addons.Scripts.ScriptClassCollection.ForEachAll(msg);
                    }
                    MessageReceived(msg);
                }
                catch (Exception x)
                {
                    x.Handle("", false);
                }
            }

            public void Add(string channelName,
                Dictionary<irc_params, string> parameters,
               string name, string Message, string outerMessageType, TimeOutResult isTO)
            {
                if (!string.IsNullOrEmpty(Message) &&
                    (channelName.Length > 0 && parameters.Count > 0))
                {
                    //Emotes.ParseEmoteFromMessage(parameters, Message, channelName);
                    ChatMessage msg =
                        new ChatMessage(Message, name, channelName, isTO, outerMessageType, parameters);
                    if (!messages.ContainsKey(channelName))
                    {
                        AddChannel(channelName);
                    }

                    Add(channelName, msg, true);
                }
            }

            public void AddChannel(string Channel)
            {
                if (!messages.ContainsKey(Channel))
                    messages.Add(Channel, new List<ChatMessage>());

                if (!messageCount.ContainsKey(Channel))
                    messageCount.Add(Channel, 0);
            }

            public void AddWhisper(string name, ChatMessage msg)
            {
                if (!users.Contains(name))
                {
                    users.Add(new ChatUser(name, name));
                }
                if (!whisper.ContainsKey(name))
                {
                    whisper.Add(name, new List<ChatMessage>());
                    whisper[name].Add(msg);

                    if (!msg.Name.Equals(ChatClient.SelfUserName))
                    {
                        if (OnWhisperReceived != null)
                            OnWhisperReceived(msg);
                    }
                }
                else
                {
                    if (!whisper[name].Any(
                        t => ((int)msg.SendTime.Subtract(t.SendTime).TotalSeconds) == 0
                        && msg.Name.Equals(t.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        whisper[name].Add(msg);
                        if (!msg.Name.Equals(ChatClient.SelfUserName))
                        {
                            if (OnWhisperReceived != null)
                                OnWhisperReceived(msg);
                        }
                    }
                }
            }

            public int Count(string channel, MsgType type, string name = "")
            {
                try
                {
                    channel = channel.ToLower().Trim();
                    lock (syncRootMessage)
                    {
                        if (type != MsgType.Whisper)
                        {
                            if (!string.IsNullOrEmpty(channel))
                            {
                                if (messages.ContainsKey(channel))
                                {
                                    if (string.IsNullOrEmpty(name))
                                    {
                                        return messages[channel].Count(t => t.IsType(type));
                                    }
                                    else
                                    {
                                        return messages[channel]
                                             .Count(t => (t.IsType(type) &&
                                                 t.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
                                    }
                                }
                            }
                            else
                            {
                                return messages.Sum(t => t.Value.Count);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(name))
                            {
                                if (whisper.ContainsKey(name))
                                {
                                    return whisper[name].Count;
                                }
                            }
                            else
                            {
                                return whisper.Sum(t => t.Value.Count);
                            }
                        }
                    }
                }
                catch { }
                return 0;
            }

            public IEnumerable<ChatMessage> Get(MsgType type, string channel, string name = "")
            {
                try
                {
                    channel = channel.ToLower().Trim();
                    lock (syncRootMessage)
                    {
                        if (type != MsgType.Whisper)
                        {
                            if (messages.ContainsKey(channel))
                            {
                                if (string.IsNullOrEmpty(name))
                                {
                                    if (messages[channel].Count(t => t.IsType(type)) > 0)
                                    {
                                        return messages[channel].Where(t => t.IsType(type));
                                    }
                                }
                                else
                                {
                                    if (messages[channel]
                                        .Count(t => (t.IsType(type) &&
                                            t.Name.Equals(name, StringComparison.OrdinalIgnoreCase))) > 0)
                                    {
                                        return messages[channel].Where(t => (t.IsType(type) &&
                                            t.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (whisper.ContainsKey(name))
                            {
                                return whisper[name];
                            }
                            //if (specialMessages.ContainsKey(channel))
                            //{
                            //    if (string.IsNullOrEmpty(name))
                            //    {
                            //        var msgs = specialMessages[channel].FindAll(cm => (cm.IsType(type)));
                            //        if (type == MsgType.Highlight)
                            //        {
                            //            msgs = msgs.Concat(specialMessages[channel]
                            //                .FindAll(cm => (cm.IsType(MsgType.Outgoing)))
                            //                .OrderBy(cm => (cm.SendTMi))).ToList();
                            //        }
                            //        if (msgs.Count > 0) return msgs;
                            //    }
                            //    else
                            //    {
                            //        var msgs = specialMessages[channel].FindAll(cm => (cm.IsType(type) && cm.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
                            //        if (type == MsgType.Highlight)
                            //        {
                            //            msgs = msgs.Concat(specialMessages[channel]
                            //                .FindAll(cm => (cm.IsType(MsgType.Outgoing) && cm.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                            //                .OrderBy(cm => (cm.SendTMi))).ToList();
                            //        }
                            //        if (msgs.Count > 0) return msgs;
                            //    }
                            // }
                        }
                    }
                }
                catch { }
                return null;
            }

            public int MessageCount(string channel)
            {
                return messageCount[channel.ToLower().Trim()];
            }

            public int MessageCount(string channel, string user)
            {
                return messages[channel].Count(t => t.Name.Equals(user, StringComparison.OrdinalIgnoreCase));
            }
        }
    }

    public static partial class ChatClient
    {
        public static AutoActions AutoActions = new AutoActions();
        public static EmoteCollection Emotes = new EmoteCollection();
        public static QuickTextCollection QuickText = new QuickTextCollection();
        private static readonly object syncRootMessage = new object();
        private static readonly object syncRootUsers = new object();

        private static List<string> chatHighlights = new List<string>();

        private static Dictionary<string, IRC> clients = new Dictionary<string, IRC>();
        private static MessageCollection messages = new MessageCollection();

        //private static Dictionary<string, List<ChatMessage>> messages = new Dictionary<string, List<ChatMessage>>();
        //private static Dictionary<string, List<ChatMessage>> whisper = new Dictionary<string, List<ChatMessage>>();
        private static ChatUserCollection users = new ChatUserCollection();

        public static Dictionary<string, ChannelInfo> Channels
        {
            get { return channels; }
            set { channels = value; }
        }

        public static List<string> ChatHighlights
        {
            get { return chatHighlights; }
        }

        public static Dictionary<string, IRC> Clients
        {
            get { return clients; }
        }

        public static MessageCollection Messages
        {
            get { return messages; }
        }

        public static ChatUserCollection Users
        {
            get { return users; }
        }

        public static void AddChatHighlightWord(string word, bool save = true)
        {
            if (string.IsNullOrEmpty(word)) return;
            if (!chatHighlights.Contains(word.ToLower()))
            {
                chatHighlights.Add(word.ToLower());
                if (save) SaveChatHighlightWord();
            }
        }

        public static void ClearChatHighlightWord()
        {
            chatHighlights.RemoveAll(t => !t.Equals(SelfUserName));
        }

        public static void FetchEmotes()
        {
            //File.Delete(Settings.dataDir + "Emotes.txt");
            Directory.Delete(Settings.emoteDir, true);
            Emotes.FetchEmotes(channels.Values.ToList(), true);
        }

        public static IEnumerable<ChannelInfo> GetChannels(IEnumerable<string> names)
        {
            return channels.Where(kvp => names.Any(t => t.Equals(kvp.Key, StringComparison.OrdinalIgnoreCase))).Select(t => t.Value);
        }

        public static void LoadChatHighlightWords()
        {
            if (File.Exists(Settings.dataDir + "HighlightKeywords.txt"))
            {
                var lines = File.ReadAllLines(Settings.dataDir + "HighlightKeywords.txt");
                foreach (var line in lines)
                {
                    AddChatHighlightWord(line, false);
                }
            }
            AddChatHighlightWord(SelfUserName);
            AddChatHighlightWord(SelfUserName.RemoveNonCharsAndDigits());
        }

        public static void SaveChatHighlightWord()
        {
            File.WriteAllLines(Settings.dataDir + "HighlightKeywords.txt", chatHighlights);
        }

        private static void addChannel(string Channel)
        {
            try
            {
                lock (syncRootMessage)
                {
                    if (Channel.Length == 0) return;
                    Channel = Channel.ToLower().Trim();

                    users.Append(Channel);

                    messages.AddChannel(Channel);
                    //if (!messages.ContainsKey(Channel))
                    //    messages.Add(Channel, new List<ChatMessage>());

                    ////if (!specialMessages.ContainsKey(Channel))
                    ////    specialMessages.Add(Channel, new List<ChatMessage>());

                    //if (!messageCount.ContainsKey(Channel))
                    //    messageCount.Add(Channel, 0);
                }
                //if (!HLmessageCount.ContainsKey(Channel))
                //    HLmessageCount.Add(Channel, 0);
            }
            catch { }
        }
    }

    public static partial class ChatClient
    {
        public class ChatUserCollection// : IDictionary<string, Dictionary<string, ChatUser>>
        {
            private Dictionary<string, Dictionary<string, ChatUser>> users;

            public ChatUserCollection()
            {
                users = new Dictionary<string, Dictionary<string, ChatUser>>();
            }

            public ChatUser Self
            {
                get { return Get(ChatClient.SelfUserName, ""); }
            }

            public void Add(ChatUser user)
            {
                try
                {
                    if (user.IsEmpty)
                    {
                        return;
                    }
                    lock (syncRootUsers)
                    {
                        string channel = user.Channel;
                        string name = user.Name;
                        if (users.ContainsKey(channel))
                        {
                            if (!users[channel].ContainsKey(name))
                            {
                                users[channel].Add(name, user);
                            }
                        }
                        else
                        {
                            addChannel(channel);
                            Add(user);
                        }
                    }
                }
                catch { }
            }

            public void Add(string channel,
               Dictionary<irc_params, string> parameters, string name)
            {
                Add(channel, parameters, name, TimeOutResult.Empty);
            }

            public void Add(string name, string channel)
            {
                Add(channel, null, name);
            }

            public void Add(string channel,
                Dictionary<irc_params, string> parameters,
                string name, TimeOutResult toResult)
            {
                try
                {
                    if (channel.Length == 0 || name.Length == 0)
                    {
                        return;
                    }
                    lock (syncRootUsers)
                    {
                        if (users.ContainsKey(channel))
                        {
                            if (!users[channel].ContainsKey(name))
                            {
                                ChatUser cu;
                                if (!toResult.IsEmpty)
                                {
                                }
                                if (parameters != null)
                                {
                                    cu = new ChatUser(channel, parameters, name, toResult);
                                }
                                else
                                {
                                    cu = new ChatUser(name, channel);
                                }
                                users[channel].Add(name, cu);
                            }
                            else
                            {
                                if (!toResult.IsEmpty)
                                {
                                }
                                users[channel][name].Parse(channel, parameters, name, toResult);
                            }
                        }
                        else
                        {
                            addChannel(channel);
                            Add(channel, parameters, name, toResult);
                        }
                    }
                }
                catch { }
            }

            public void Append(string channel)
            {
                if (!users.ContainsKey(channel))
                {
                    users.Add(channel, new Dictionary<string, ChatUser>());
                }
            }

            public bool Contains(string name)
            {
                return users.Any(t => t.Value.ContainsKey(name));
            }

            public int Count(string channel)
            {
                return users[channel].Count;
            }

            public int Count(string channel, Func<ChatUser, bool> predicate)
            {
                channel = channel.ToLower().Trim();
                return users[channel].Values.Count(predicate);
            }

            public IEnumerable<ChatUser> Find(string name, string channel)
            {
                if (users.ContainsKey(channel))
                {
                    return users[channel].Values.Where(t => t.Name.StartsWith(name));
                }
                return new List<ChatUser>();
            }

            public ChatUser Get(string name, string channel)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    channel = channel.ToLower().Trim();
                    name = name.ToLower().Trim();
                    if (string.IsNullOrEmpty(channel))
                    {
                        foreach (string s in users.Keys)
                        {
                            if (users[s].ContainsKey(name))
                            {
                                return users[s][name];
                            }
                        }
                    }
                    else if (users[channel].ContainsKey(name))
                    {
                        return users[channel][name];
                    }
                }
                return ChatUser.Emtpy;
            }

            public Dictionary<string, ChatUser> Get(string channel)
            {
                if (users.ContainsKey(channel.ToLower().Trim()))
                {
                    return users[channel.ToLower().Trim()];
                }
                return null;
            }

            public string[] GetAllNames()
            {
                return users.SelectMany(t => t.Value).Select(t => t.Key).ToArray();
            }

            public void SetOffline(string key, string channel)
            {
                if (users.ContainsKey(channel))
                {
                    if (users[channel].ContainsKey(key))
                    {
                        users[channel][key].IsOnline = false;
                        //foreach (ChatMessage m in messages[channel])
                        //{
                        //}
                    }
                }
                else
                {
                    addChannel(channel);
                    Add(key, channel);
                    users[channel][key].IsOnline = false;
                }
            }

            private IEnumerable<ChatUser> Users(string channel)
            {
                return users[channel].Values;
            }
        }
    }
}