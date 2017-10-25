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
        public delegate void _JoinedChannel(int channel);

        public delegate void _PartedChannel(int channel);

        public delegate void _ReceivedHandler(ChatMessage msg);

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

        private static void ChannelJoined(int channel)
        {
            if (OnChannelJoined != null)
                OnChannelJoined(channel);
        }

        private static void ChannelParted(int channel)
        {
            if (OnChannelParted != null)
                OnChannelParted(channel);
        }

        private static void MessageReceived(ChatMessage msg)
        {
            if (OnMessageReceived != null)
                OnMessageReceived(msg);
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

        //private static msg_ids ParseMsgID(Dictionary<irc_params, string> parameters, string channelName, string raw)
        //{
        //    if (parameters.ContainsKey(irc_params.msg_id))
        //    {
        //        msg_ids id = msg_ids.NONE;
        //        if (Enum.TryParse<msg_ids>(parameters[irc_params.msg_id], out id))
        //        {
        //            if (Enum.IsDefined(typeof(channel_mode), (int)id))
        //            {
        //                channel_mode mode = (channel_mode)id;
        //            }
        //            var channel = channels[channelName];
        //            channel.Modes.SetMode(id, raw);
        //            switch (id)
        //            {
        //                case msg_ids.slow_on:
        //                    {
        //                        var time = raw.GetBefore(" seconds", " ");
        //                        int i = 0;
        //                        if (int.TryParse(time, out i))
        //                        {
        //                            channel.SlowMode = i;
        //                        }
        //                    }
        //                    break;

        //                case msg_ids.slow_off:
        //                    channel.SlowMode = 0;
        //                    break;
        //            }
        //            return id;
        //        }
        //    }
        //    return msg_ids.NONE;
        //}

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

                    //msg_ids msgid = ParseMsgID(parameters, channelName, raw);

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
                                        SubType st = SubType.NoSub;
                                        if (Enum.TryParse<SubType>(parameters[irc_params.msg_param_sub_plan], out st))
                                        {
                                            subType = Enum.GetName(typeof(SubType), st);
                                        }
                                    }
                                    system_msg = subType + " " + msg_id + " " + months + "x";
                                }
                                //SendSilentMessage(system_msg, channelName, MsgType.UserNotice);
                                //messages.Add(channelName, parameters, Name, Message, spliType, tor);
                                messages.Add(channelName, system_msg, true, MsgType.UserNotice);
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
                    users.Add(new ChatUser(name, channel, UserType.moderator));
                }
            }
            return false;
        }
    }

    public static partial class ChatClient
    {
        private static TwitchUserCollection _twitchUsers = new TwitchUserCollection(Settings._dataDir + "auth.txt");

        public static string SelfUserName
        {
            get { return _twitchUsers.Selected.Name; }
        }

        public static TwitchUserCollection TwitchUsers
        {
            get { return _twitchUsers; }
        }
    }

    public static partial class ChatClient
    {
        //private static int connectCnt = 0;

        private readonly static string[] commands = new string[] { ".ban", ".timeout", ".unban" };
        private readonly static object lockChannels = new object();
        private static DateTime lastSend = DateTime.MinValue;
        private static Dictionary<int, int> reconTimeouts = new Dictionary<int, int>();
        private static SortMode[] sortArr;

        public static long ReceivedBytes
        {
            get { return clients.Sum(t => t.Value.ReceivedBytes); }
        }

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

        public static void Disconnect(int channel)
        {
            try
            {
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
                    var sa = clients.Values.Select(t => t._Channel).ToArray();
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

        public static bool HasJoined(int channel)
        {
            return clients.ContainsKey(channel);
        }

        public static void Reconnect()
        {
            var sa = ChatClient.Channels.Where(t => ChatClient.HasJoined(t.Key));
            foreach (var si in sa)
            {
                ChatClient.Disconnect(si.Key);
                while (ChatClient.HasJoined(si.Key)) System.Threading.Thread.Sleep(100);
                ChatClient.TryConnect(si.Key, true);
            }
        }

        public static ChatMessage SendMessage(string Message, int channel)
        {
            //channel = channel.ToLower().Trim();
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
                    var client = clients.FirstOrDefault(t => t.Value.Channel_Name.Equals("lukix29", StringComparison.OrdinalIgnoreCase)).Value;
                    //Message = "." + Message.Remove(0, 1);
                    client.SendMessage(Message, false);
                    messages.AddWhisper(name, m);
                    // LogMessage(name, rawMessage);
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

        public static void TryConnect(int channel, bool force = false)
        {
            if (!channels.ContainsKey(channel)) return;
            try
            {
                //channel = GetOnlyName(channel).ToLower().Trim();
                lock (lockChannels)
                {
                    if (!clients.ContainsKey(channel) || force)
                    {
                        connect(channel, channels[channel].Name);
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

        private static void client_AddClient(IRC c, string Message)
        {
            try
            {
                if (Message.Contains("JOIN") && Message.Contains(SelfUserName))
                {
                    bool hasAdded = false;
                    lock (lockChannels)
                    {
                        if (!clients.ContainsKey(c._Channel))
                        {
                            c.NetworkError += client_NetworkError;
                            c.UserJoinedChannel += client_UserJoinedChannel;
                            c.UserKicked += client_UserKicked;
                            c.UserPartedChannel += client_UserPartedChannel;
                            c.UserQuit += client_UserQuit;

                            if (clients == null) clients = new Dictionary<int, IRC>();
                            clients.Add(c._Channel, c);
                            hasAdded = true;
                        }
                    }
                    if (hasAdded)
                    {
                        reconTimeouts[c._Channel] = 8000;
                        Task.Run(() => FetchChatUsers(c.Channel_Name));
                        ListUpdated();
                    }
                }
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        client_AddClient(c, Message);
                        break;
                }
            }
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
                    SendSilentMessage("Error Conecting to Chat, reconecting in " + (reconTimeouts[c._Channel] / 1000) + "s!", c.Channel_Name);
                    await Task.Delay(reconTimeouts[c._Channel]);
                    reconTimeouts[c._Channel] = Math.Min(32000, reconTimeouts[c._Channel] * 2);
                    SendSilentMessage("Reconecting now!", c.Channel_Name);
                    Disconnect(c);
                    connect(c._Channel, c.Channel_Name);
                });

                ListUpdated();
                //}
            }
            catch (Exception x) { x.Handle(); }
        }

        private static void client_RawMessageRecieved(object sender, RawMessageEventArgs e)
        {
            IRC c = (IRC)sender;
            client_AddClient(c, e.Message);
            TryParseRawMessage(e.Message);
            LogMessage(c._Channel, e.Message);
        }

        private static void client_UserJoinedChannel(object sender, IrcChannelUserEventArgs e)
        {
            if (e.User.Equals(SelfUserName, StringComparison.OrdinalIgnoreCase))
            {
                ChannelJoined(e.Channel);
            }
            var name = ((IRC)sender).Channel_Name;
            users.Add(e.User, name);
        }

        private static void client_UserKicked(object sender, IrcChannelUserEventArgs e)
        {
            if (e.User.Equals(SelfUserName, StringComparison.OrdinalIgnoreCase))
            {
                Disconnect(e.Channel);
                ChannelParted(e.Channel);
                ListUpdated();
            }
            var name = ((IRC)sender).Channel_Name;
            users.SetOffline(e.User, name);
        }

        private static void client_UserPartedChannel(object sender, IrcChannelUserEventArgs e)
        {
            if (e.User.Equals(SelfUserName, StringComparison.OrdinalIgnoreCase))
            {
                Disconnect(e.Channel);
                ChannelParted(e.Channel);
                ListUpdated();
            }
            var name = ((IRC)sender).Channel_Name;
            users.SetOffline(e.User, name);
        }

        private static void client_UserQuit(object sender, IrcChannelUserEventArgs e)
        {
            if (e.User.Equals(SelfUserName, StringComparison.OrdinalIgnoreCase))
            {
                Disconnect(e.Channel);
                ChannelParted(e.Channel);
                ListUpdated();
            }
            var name = ((IRC)sender).Channel_Name;
            users.SetOffline(e.User, name);
        }

        private static void connect(int channel, string channelName)
        {
            try
            {
                Disconnect(channel);

                IRC client = new IRC("irc.twitch.tv", channelName, channel);

                client.ConnectionComplete += client_ConnectionComplete;

                client.ConnectAsync();

                Task.Run(async () =>
                {
                    if (!reconTimeouts.ContainsKey(channel))
                    {
                        reconTimeouts.Add(channel, 8000);
                    }
                    await Task.Delay(reconTimeouts[channel]);
                    if (!clients.ContainsKey(client._Channel))
                    {
                        SendSilentMessage("Chat Conecting Timeout (" + (reconTimeouts[channel] / 1000) + "s)!", channelName);
                        reconTimeouts[channel] = Math.Min(64000, reconTimeouts[channel] * 2);
                        Disconnect(client);
                        connect(channel, channelName);
                    }
                    else
                    {
                        reconTimeouts[channel] = 8000;
                    }
                });
            }
            catch (Exception x)
            {
            }
        }

        private static void FetchChatUsers(string channelName)
        {
            var cus = TwitchApi.GetChatUsers(channelName);
            var cur = users.Get(channelName);
            //  File.WriteAllLines("users.txt", cus.Keys.ToArray());
            try
            {
                if (cus == null) return;
                if (cur == null)
                {
                    foreach (var user in cus)
                    {
                        users.Add(user.Value);
                    }
                }
                else
                {
                    var diff = cus.Keys.Except(cur.Keys).Select(t => cus[t]);
                    foreach (var user in diff)
                    {
                        users.Add(user);
                    }
                    var diff2 = cur.Keys.Except(cus.Keys).Select(t => cur[t]);
                    foreach (var user in diff2)
                    {
                        user.IsOnline = false;
                    }
                }
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        FetchChatUsers(channelName);
                        break;
                }
            }
        }

        private static void LogMessage(ChannelInfo channel, string message)
        {
            if (channel.LogChat)
            {
                File.AppendAllText(Settings._chatLogDir + channel.Name.ToLower() + ".log", message + "\r\n");
            }
        }

        private static void LogMessage(int channel, string message)
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
            var ChannelName = irc.Channel_Name;
            var Channel = irc._Channel;
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

                    user = (string.IsNullOrEmpty(user)) ? SelfUserName.ToLower() : user;
                    ChatUser me = ChatClient.Users.Get(user, ChannelName);

                    bool mod = me.Types.Any(t => (((int)t) >= (int)UserType.moderator));

                    TimeSpan sec = DateTime.Now.Subtract(channelInfo.LastSendMessageTime);
                    if (!channelInfo.HasSlowMode ||
                        sec.TotalSeconds > channelInfo.SlowMode)
                    {
                        channelInfo.LastSendMessageTime = DateTime.Now;

                        ChatMessage m = new ChatMessage(Message, me, ChannelName, true);
                        if (!commands.Any(t => Message.ToLower().StartsWith(t)))
                        {
                            messages.Add(m, true);
                        }
                        LogMessage(Channel, Message);

                        if (!irc.SendMessage(Message, true, mod))
                        {
                            me = ChatUser.Emtpy;
                            Message = "Message was NOT sent! Automatic Global-Ban-Protection.";
                            m = new ChatMessage(Message, me, ChannelName, false);
                            messages.Add(m, false);
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
        public static AutoActions AutoActions = new AutoActions();
        public static EmoteCollection Emotes = new EmoteCollection();
        public static QuickTextCollection QuickText = new QuickTextCollection();
        private static readonly object syncRootMessage = new object();
        private static readonly object syncRootUsers = new object();

        private static List<string> chatHighlights = new List<string>();

        private static Dictionary<int, IRC> clients = new Dictionary<int, IRC>();
        private static MessageCollection messages;// = new MessageCollection();

        //private static Dictionary<string, List<ChatMessage>> messages = new Dictionary<string, List<ChatMessage>>();
        //private static Dictionary<string, List<ChatMessage>> whisper = new Dictionary<string, List<ChatMessage>>();
        private static ChatUserCollection users = new ChatUserCollection();

        public static Dictionary<int, ChannelInfo> Channels
        {
            get { return channels; }
            set { channels = value; }
        }

        public static List<string> ChatHighlights
        {
            get { return chatHighlights; }
        }

        public static Dictionary<int, IRC> Clients
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
            var bitmap = LX29_LixChat.Properties.Resources.loading;
        }

        public static void ClearChatHighlightWord()
        {
            chatHighlights.Clear();
        }

        public static void FetchEmotes()
        {
            //File.Delete(Settings.dataDir + "Emotes.txt");
            Emotes.Values.Dispose();
            Emotes.FetchEmotes(channels.Values.ToList(), true);
        }

        //public static IEnumerable<ChannelInfo> GetChannels(IEnumerable<string> names)
        //{
        //    return channels.Where(kvp => names.Any(t => t.Equals(kvp.Key, StringComparison.OrdinalIgnoreCase))).Select(t => t.Value);
        //}

        public static void LoadChatHighlightWords()
        {
            if (File.Exists(Settings._dataDir + "HighlightKeywords.txt"))
            {
                var lines = File.ReadAllLines(Settings._dataDir + "HighlightKeywords.txt");
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
            File.WriteAllLines(Settings._dataDir + "HighlightKeywords.txt", chatHighlights);
        }

        private static void addChannel(string channel)
        {
            try
            {
                lock (syncRootMessage)
                {
                    if (channel.Length == 0) return;
                    var ch = channel.ToLower().Trim();

                    users.Append(ch);

                    //messages.AddChannel(Channel);
                }
            }
            catch { }
        }
    }
}