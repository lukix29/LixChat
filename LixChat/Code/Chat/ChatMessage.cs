using LX29_ChatClient.Emotes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using LX29_ChatClient.Addons;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

using System.IO;

using System.Linq;

using System.Media;
using System.Runtime.InteropServices;

using System.Windows.Forms;

using LinqToSqlShared.Mapping;
using System.Data.Linq.Mapping;
using System.Data.Linq;
using System.Data.SQLite;

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

    public enum irc_params
    {
        color,
        badges,
        emotes,
        display_name,
        mod,
        subscriber,
        tmi_sent_ts,
        turbo,
        user_type,
        ban_duration,
        ban_reason,
        room_id,
        user_id,
        target_user_id,
        msg_param_months,
        system_msg,
        msg_id,
        id,
        msg_param_sub_plan,
        msg_param_sub_plan_name
    }

    public enum msg_ids
    {
        NONE = 0,
        resub = 19,
        subs_on = 1,//	This room is now in subscribers-only mode.
        already_subs_on = 1,//	This room is already in subscribers-only mode.
        subs_off = 2,//	This room is no longer in subscribers-only mode.
        already_subs_off = 2,//	This room is not in subscribers-only mode.
        msg_slowmode = 3,//	This room is now in slow mode. You may send messages every slow_duration seconds.

        r9k_on = 5,//	This room is now in r9k mode.
        already_r9k_on = 5,//	This room is already in r9k mode.
        r9k_off = 6,//	This room is no longer in r9k mode.
        already_r9k_off = 6,//	This room is not in r9k mode.
        slow_on = 3,
        slow_off = 4,//	This room is no longer in slow mode.
        emote_only_on = 9,//	This room is now in emote-only mode.
        already_emote_only_on = 9,//	This room is already in emote-only mode.
        emote_only_off = 10,//This room is no longer in emote-only mode.
        already_emote_only_off = 10,//	This room is not in emote-only mode.
        host_on = 7,//	Now hosting target_channel.
        bad_host_hosting = 7,//	This channel is already hosting target_channel.
        host_off = 8,//	Exited host mode.
        hosts_remaining = 7,//	number host commands remaining this half hour.

        msg_channel_suspended = 11,//	This channel has been suspended.
        timeout_success = 12,//	target_user has been timed out for ban_duration seconds.
        untimeout_success = 13,//	target_user is no longer timed out.
        ban_success = 14,//target_user is now banned from this room.
        unban_success = 15,//	target_user is no longer banned from this room.
        bad_unban_no_ban = 16,//	target_user is not banned from this room.
        already_banned = 17,//	target_user is already banned in this room.
        unrecognized_cmd = 18
    }

    public enum MsgType
    {
        None = 0,
        All_Messages = 1,
        HL_Messages = 2,
        Whisper = 3,
        Action = 4,
        UserNotice = 5,
        Notice = 6,
        State = 7,
        Outgoing = 8,
        Clearchat = 9
    }

    public struct TimeOutResult
    {
        public static readonly TimeOutResult Empty = new TimeOutResult("", "", false, false, -1, "");
        public readonly string Channel;
        public readonly bool HasTimeOut;
        public readonly bool IsBanned;
        public readonly string Name;
        public readonly string Reason;
        public readonly int TimeOutDuration;

        public TimeOutResult(string name, string channel, bool timeout, bool banned, int timeoutDuration, string reason)
        {
            Name = name;
            Channel = channel;
            HasTimeOut = timeout;
            IsBanned = banned;
            TimeOutDuration = timeoutDuration;
            Reason = reason;
        }

        public bool IsEmpty
        {
            get { return !HasTimeOut && !IsBanned; }
        }

        public string Message
        {
            get
            {
                string name = Name;
                if (IsBanned)
                {
                    return name + " has been banned from " + Channel + "!";
                }
                else if (HasTimeOut)
                {
                    string msg = name + " has been timed out for " + TimeOutDuration + "s";
                    if (!string.IsNullOrEmpty(Reason))
                    {
                        msg += " (" + Reason.Replace("\\s", " ") + ")";
                    }
                    return msg;
                }
                return string.Empty;
            }
        }

        public static TimeOutResult Parse(string raw, string Name, string Channel, Dictionary<irc_params, string> parameters)
        {
            try
            {
                bool HasTimeOut = false;
                bool IsBanned = false;
                int TimeOutSeconds = 0;
                string reason = "";
                TimeOutResult tresult = TimeOutResult.Empty;

                Name = raw.GetBetween(raw.LastIndexOf(":") + 1, raw.Length);
                if (parameters.ContainsKey(irc_params.ban_reason)
                    && !parameters.ContainsKey(irc_params.ban_duration))
                {
                    IsBanned = true;
                    reason = parameters[irc_params.ban_reason].Replace("\\s", " ");
                }
                else if (parameters.ContainsKey(irc_params.ban_duration))
                {
                    HasTimeOut = true;
                    TimeOutSeconds = int.Parse(parameters[irc_params.ban_duration]);
                    if (parameters.ContainsKey(irc_params.ban_reason))
                    {
                        reason = parameters[irc_params.ban_reason].Replace("\\s", " ");
                    }
                }
                if (IsBanned || HasTimeOut)
                {
                    tresult = new TimeOutResult(Name, Channel, HasTimeOut, IsBanned, TimeOutSeconds, reason);
                    //rtzuio
                    var user = ChatClient.Users.Get(Name, Channel);
                    if (!user.IsEmpty)
                    {
                        if (user.HasTimeOut)//.To_Timer.Result.TimeOutDuration >= TimeOutSeconds)
                        {
                            return TimeOutResult.Empty;
                        }
                        else if (user.To_Timer.Result.TimeOutDuration <= 0 && tresult.IsEmpty)
                        {
                            return TimeOutResult.Empty;
                        }
                        user.SetTimeOut(tresult);
                    }

                    var msgs = ChatClient.Messages.Get(MsgType.All_Messages, Channel);
                    if (msgs != null && msgs.Count() > 0)
                    {
                        msgs = msgs.Where(t => t.Name.Equals(Name, StringComparison.OrdinalIgnoreCase));
                        foreach (var cm in msgs)
                        {
                            cm.Timeout = tresult;
                        }
                    }
                }
                return tresult;
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        Parse(raw, Name, Channel, parameters);
                        break;
                }
            }
            return TimeOutResult.Empty;
        }
    }

    [Table(Name = "Message")]
    public class CacheMessage : IEquatable<CacheMessage>, IComparer<CacheMessage>
    {
        public CacheMessage(ChatMessage msg, int index)
        {
            Message = msg.Message;
            Channel = msg.Channel;
            Name = msg.Name;
            Time = msg.SendTime;
            Timeout = !msg.Timeout.IsEmpty;
            Index = index;
            Types = msg.Types.Select(t => (byte)t).ToArray();
            ID = Channel + "_" + index;

            if (msg.User.Badges.Length > 0)
            {
                Badges = msg.User.Badges.Select(t => t.ToString()).Aggregate((t0, t1) => t0 + " " + t1);
            }
        }

        public CacheMessage()
        {
        }

        [Column(Name = "Badges")]
        public string Badges { get; set; }

        [Column(Name = "Channel")]
        public string Channel { get; set; }

        [Column(Name = "ID", IsPrimaryKey = true)]
        public string ID { get; set; }

        [Column(Name = "Index")]
        public int Index { get; set; }

        [Column(Name = "Message")]
        public string Message { get; set; }

        [Column(Name = "Name")]
        public string Name { get; set; }

        [Column(Name = "Time", DbType = "DateTime")]
        public DateTime Time { get; set; }

        [Column(Name = "Timeout")]
        public bool Timeout { get; set; }

        [Column(Name = "Types")]
        public byte[] Types { get; set; }

        public int Compare(CacheMessage x, CacheMessage y)
        {
            return x.Index - y.Index;
        }

        public bool Equals(CacheMessage m2)
        {
            return ID.Equals(m2.ID);
        }

        public void From(CacheMessage m)
        {
            Badges = m.Badges;

            Channel = m.Channel;

            ID = m.ID;

            Index = m.Index;

            Message = m.Message;

            Name = m.Name;

            Time = m.Time;
            Timeout = m.Timeout;

            Types = m.Types;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        //public static DateTime ConvertFromUnixTimestamp(double timestamp)
        //{
        //    DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        //    return origin.AddSeconds(timestamp);
        //}

        //public static double ConvertToUnixTimestamp(DateTime date)
        //{
        //    DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        //    TimeSpan diff = date.ToUniversalTime() - origin;
        //    return Math.Floor(diff.TotalSeconds);
        //}
    }

    public class ChatMessage : IEquatable<ChatMessage>, IComparer<ChatMessage>
    {
        public const string ACTION = "ACTION";
        public const string CLEARCHAT = "CLEARCHAT";
        public const string GLOBALUSERSTATE = "GLOBALUSERSTATE";
        public const string NOTICE = "NOTICE";
        public const string PRIVMSG = "PRIVMSG";
        public const string USERNOTICE = "USERNOTICE";
        public const string USERSTATE = "USERSTATE";
        public const string WHISPER = "WHISPER";

        public static readonly ChatMessage Empty = new ChatMessage();

        public ChatMessage()
        {
            Message = "";
            Name = "";
            Channel = "";
            SendTime = DateTime.MaxValue;
        }

        public ChatMessage(CacheMessage msg)
        {
            Channel = msg.Channel;
            Name = msg.Name;
            Message = msg.Message;
            User = (ChatClient.Users != null) ? ChatClient.Users.Get(Name, Channel, true) : new ChatUser(msg.Name, msg.Channel);
            SendTime = msg.Time;

            //Message = msg.Message;
            //Channel = ChatClient.Channels[msg.Channel].ID;
            //Name = msg.Name;
            //Time = msg.SendTime;
            //Timeout = !msg.Timeout.IsEmpty;
            //Index = index;
            //Types = msg.Types.Select(t => (byte)t).ToArray();
            //ID = Channel + "_" + index;

            //if (msg.User.Badges.Length > 0)
            //{
            //    Badges = msg.User.Badges.Select(t => t.ToString()).Aggregate((t0, t1) => t0 + " " + t1);
            //}
        }

        public ChatMessage(string message, string user, string channel, TimeOutResult toResult, string outerMessageType, Dictionary<irc_params, string> parameters)
        {
            Types = new HashSet<MsgType>();
            if (parameters.ContainsKey(irc_params.msg_id))
            {
                msg_ids ids = msg_ids.NONE;
                if (Enum.TryParse<msg_ids>(parameters[irc_params.msg_id], out ids))
                {
                    Message_ID = ids;
                }
                else
                {
                    Message_ID = msg_ids.NONE;
                }
            }
            if (user == ChatClient.SelfUserName)
            {
                Types.Add(MsgType.Outgoing);
            }
            if (message.Length > 0)
            {
                if (message.Contains(ACTION))
                {
                    Types.Add(MsgType.Action);
                    message = message.ReplaceAll("", "\u0001", ACTION).Trim();
                }
                if (outerMessageType.Equals(WHISPER))
                {
                    Types.Add(MsgType.Whisper);
                    message = message.ReplaceAll("", "\u0001", WHISPER).Trim();
                }
                else
                {
                    string msg = " " + message.ToLower() + " ";
                    if (!user.Equals(ChatClient.SelfUserName, StringComparison.OrdinalIgnoreCase)
                        && ChatClient.ChatHighlights
                        .Any(t => System.Text.RegularExpressions.Regex
                            .IsMatch(msg, @"\W" + t + @"\W")))
                    {
                        Types.Add(MsgType.HL_Messages);
                    }
                }
            }
            switch (outerMessageType)
            {
                case GLOBALUSERSTATE:
                    break;

                case WHISPER:
                    Types.Add(MsgType.Whisper);// IsWhisper = true;
                    message = message.ReplaceAll("", "\u0001", WHISPER).Trim();
                    break;

                case NOTICE:
                    Types.Add(MsgType.Notice);
                    break;

                case USERNOTICE:
                    Types.Add(MsgType.UserNotice);
                    break;

                case USERSTATE:
                    Types.Add(MsgType.State);
                    break;

                case CLEARCHAT:
                    Types.Add(MsgType.Clearchat);
                    break;
            }

            if (parameters.ContainsKey(irc_params.tmi_sent_ts))
            {
                long t = long.Parse(parameters[irc_params.tmi_sent_ts]);
                DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                SendTime = epoch.AddMilliseconds(t).ToLocalTime();
            }

            ChatWords = ChatClient.Emotes.ParseEmoteFromMessage(parameters, message, channel, Types);

            Parameters = parameters;
            Message = message;

            if (Message.StartsWith(".")) Message = Message.Remove(0, 1);

            Channel = channel;
            Timeout = toResult;

            //if (Types.Count == 0)
            //{
            //    Types.Add(MsgType.All_Messages);
            //}

            Name = user.ToLower();
            User = ChatClient.Users.Get(Name, channel);
            SendTime = DateTime.Now;
        }

        public ChatMessage(string message, ChatUser user, string channel, bool isSent, params MsgType[] types)
        {
            Types = new HashSet<MsgType>();
            this.Message = message;

            Channel = channel;
            Timeout = TimeOutResult.Empty;

            Name = user.Name.ToLower();

            User = user;

            SendTime = DateTime.Now;
            if (this.Message.StartsWith(".me"))
            {
                Types.Add(MsgType.Action);
                this.Message = this.Message.Remove(0, 3);
            }
            if (Message.StartsWith(".")) Message = Message.Remove(0, 1);

            if (isSent)
            {
                Types.Add(MsgType.Outgoing); //IsSentMessage = isSent;// (name.ToLower() == StreamManager.UserName);
                if (message.StartsWith("/w"))
                {
                    message = message.Substring(2);
                    Types.Add(MsgType.Whisper);
                }
            }
            if (types != null)
            {
                foreach (var t in types)
                {
                    if (!Types.Contains(t))
                    {
                        Types.Add(t);
                    }
                }
            }
            ChatWords = ChatClient.Emotes.ParseEmoteFromMessage(null, message, channel, Types);
        }

        [JsonIgnore]
        public string Channel
        {
            get;
            private set;
        }

        [JsonIgnore]
        public List<ChatWord> ChatWords
        {
            get;
            private set;
        }

        [JsonIgnore]
        public bool IsEmpty
        {
            get
            {
                return (Message.Length == 0) || Channel.Length == 0;
            }
        }

        //public Emote[] Emotes
        //{
        //    get { return ChatWords.Where(t => t.IsEmote).Select(t => t.Emote).ToArray(); }
        //}
        [JsonIgnore]
        public bool IsMessageEmpty
        {
            get
            {
                return string.IsNullOrEmpty(Message.Trim());
            }
        }

        [JsonIgnore]
        public bool IsNormal
        {
            get { return Types.Count == 0; }
        }

        [JsonIgnore]
        public int Length
        {
            get { return Message.Length; }
        }

        public string Message
        {
            get;
            private set;
        }

        [JsonIgnore]
        public msg_ids Message_ID
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        [JsonIgnore]
        public Dictionary<irc_params, string> Parameters
        {
            get;
            private set;
        }

        public DateTime SendTime
        {
            get;
            private set;
        }

        [JsonIgnore]
        public TimeOutResult Timeout
        {
            get;
            set;
        }

        [JsonIgnore]
        public HashSet<MsgType> Types
        {
            get;
            set;
        }

        [JsonIgnore]
        public ChatUser User
        {
            get;
            private set;
        }

        [JsonIgnore]
        public string[] Words
        {
            get { return ChatWords.Select(t => t.Text).ToArray(); }
        }

        public ChatWord this[int Index]
        {
            get { return ChatWords[Index]; }
        }

        public int Compare(ChatMessage x, ChatMessage y)
        {
            return (int)(x.SendTime.Ticks - y.SendTime.Ticks);
        }

        public bool Equals(ChatMessage m)
        {
            return (m.SendTime.Second == SendTime.Second && m.SendTime.Minute == SendTime.Minute) &&
                m.Name.Equals(Name, StringComparison.OrdinalIgnoreCase);
        }

        public string GetString()
        {
            return SendTime.ToLongTimeString() + " " + Name + ": " + Message;
        }

        public bool IsType(params MsgType[] Type)
        {
            if (Type.Length > 1)
            {
                var gs = Type.Any(t => Types.Contains(t));
                return gs;
            }
            else
            {
                if (Type[0] == MsgType.All_Messages) return true;
                return Types.Contains(Type[0]);
            }
        }

        public void ReloadEmotes()
        {
            try
            {
                ChatWords = ChatWords.Select(t =>
                {
                    if (!t.IsEmote)
                    {
                        return new ChatWord(t.Text, Channel);//typelist.Contains(MsgType.Outgoing));
                    }
                    else return t;
                }).ToList();
            }
            catch
            {
            }
        }

        public override string ToString()
        {
            return SendTime.ToLongTimeString() + " " + Name + ": " + Message;
        }

        private string USERNAMELIST(string ChannelName)
        {
            return ChatClient.SelfUserName + " = #" + ChannelName;
        }
    }

    public class ChatWord
    {
        public ChatWord(TempEmoteWord input, string word)
        {
            if (input.Emote == null)
            {
                Text = word;
            }
            else
            {
                Text = input.Emote.Name;
                Emote = new EmoteBase[] { input.Emote };
                IsEmote = true;
            }
        }

        public ChatWord(string name, string channel, bool outgoing)
        {
            Text = name;

            string temp = name;
            var em = ChatClient.Emotes.GetEmote(name, channel, outgoing);
            if (em != null && em.Count() > 0)
            {
                Emote = em.ToArray();
                IsEmote = true;
            }
        }

        public ChatWord(string name, string channel)
        {
            Text = name;

            var em = ChatClient.Emotes.Values.All.Where(t => t.Name.Equals(name));
            if (em != null && em.Count() > 0)
            {
                Emote = em.ToArray();
                IsEmote = true;
            }
        }

        public EmoteBase[] Emote
        {
            get;
            private set;
        }

        public bool IsEmote
        {
            get;
            private set;
        }

        public string Text
        {
            get;
            private set;
        }

        public override string ToString()
        {
            return Text;
        }

        public struct TempEmoteWord
        {
            public EmoteBase Emote;

            public int End;
            public int Start;

            public TempEmoteWord(int Start, int End, EmoteBase Emote)
            {
                this.Start = Start;
                this.End = End;
                this.Emote = Emote;
            }
        }
    }
}