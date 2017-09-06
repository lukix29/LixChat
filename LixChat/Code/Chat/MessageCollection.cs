using LX29_ChatClient.Addons;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LX29_ChatClient
{
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
                            else
                            {
                                users[channel][name] = user;
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
                    else if (users.ContainsKey(channel) && users[channel].ContainsKey(name))
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

        public class MessageBuffer
        {
            private string channel = "";
            private Dictionary<int, ChatMessage> messages = new Dictionary<int, ChatMessage>();
            private FileStream mfile;
            private List<Tuple<long, long>> positions = new List<Tuple<long, long>>();

            public MessageBuffer(string channel)
            {
                this.channel = channel;
                mfile = new FileStream(Settings.chatLogDir + channel + ".cache", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                //if cache exists at start and last message is not too old load cache
            }

            public int Length
            {
                get;
                private set;
            }

            private bool enableCaching
            {
                get { return Settings.MessageCaching; }
            }

            public void Add(ChatMessage message)
            {
                try
                {
                    if (enableCaching)
                    {
                        string s = JsonConvert.SerializeObject(message);
                        byte[] buff = System.Text.Encoding.UTF8.GetBytes(s);

                        positions.Add(new Tuple<long, long>(mfile.Position, buff.Length));
                        //mfile.Lock(mfile.Position, buff.Length);

                        mfile.Write(buff, 0, buff.Length);
                        //mfile.Unlock(mfile.Position, buff.Length);
                    }
                    Length++;
                    messages.Add(Length - 1, message);

                    while (messages.Count > Settings.ChatHistory)
                    {
                        int min = messages.Keys.Min();
                        messages.Remove(min);
                    }
                }
                catch
                {
                }
            }

            public int Count(Func<ChatMessage, bool> a)
            {
                return messages.Values.Count(a);
            }

            public ChatMessage GetMessage(int index)
            {
                if (index < 0) return null;

                long pos = mfile.Position;
                var kvp = positions[index];
                byte[] ba = new byte[kvp.Item2];

                //mfile.Lock(kvp.Item1, kvp.Item2);
                mfile.Seek(kvp.Item1, 0);
                mfile.Read(ba, 0, ba.Length);
                mfile.Seek(pos, 0);
                //mfile.Unlock(kvp.Item1, kvp.Item2);

                var json = System.Text.Encoding.UTF8.GetString(ba);
                var msg = JsonConvert.DeserializeObject<RootObject>(json);
                var user = ChatClient.users.Get(msg.Name, channel);
                return new ChatMessage(msg.Message, user, channel,
                    false, msg.Types);
            }

            public List<ChatMessage> GetMessages(int start = 256, int end = -1, Func<ChatMessage, bool> a = null)
            {
                if (messages.Count <= 0) return new List<ChatMessage>();
                if (end < 0) end = this.Length - 1;
                if (start < 0) start = Math.Max(0, end - 256);
                Dictionary<int, ChatMessage> list = new Dictionary<int, ChatMessage>();
                for (int i = start; i <= end; i++)
                {
                    if (messages.ContainsKey(i))
                    {
                        if (a != null)
                        {
                            if (a.Invoke(messages[i]))
                                list.Add(i, messages[i]);
                        }
                        else
                        {
                            list.Add(i, messages[i]);
                        }
                    }
                    else if (enableCaching)
                    {
                        var msg = GetMessage(i);
                        if (a != null)
                        {
                            if (a.Invoke(msg))
                                list.Add(i, msg);
                        }
                        else
                        {
                            list.Add(i, msg);
                        }
                    }
                }
                if (enableCaching) messages = list;
                return list.Values.ToList();
                //Settings.ChatHistory
            }

            public IEnumerable<ChatMessage> Where(Func<ChatMessage, bool> a)
            {
                return messages.Values.Where(a);
            }

            public IEnumerable<ChatMessage> Where(Func<ChatMessage, bool> a, int start, int end)
            {
                return GetMessages(start, end, a);
            }

            public class RootObject
            {
                public List<int> _types { get; set; }

                public string Message { get; set; }

                public string Name { get; set; }

                public DateTime SendTime { get; set; }

                public MsgType[] Types
                {
                    get { return _types.Select(t => (MsgType)t).ToArray(); }
                }
            }
        }

        public class MessageCollection // : IDictionary<string, Dictionary<string, ChatUser>>
        {
            //private JsonSerializer jss = new JsonSerializer()
            //            {
            //                DefaultValueHandling = DefaultValueHandling.Ignore,
            //                ObjectCreationHandling = ObjectCreationHandling.Reuse,
            //                MissingMemberHandling = MissingMemberHandling.Ignore
            //            };

            //count bytes received in IRC-client

            private Dictionary<string, int> messageCount = new Dictionary<string, int>();

            private Dictionary<string, MessageBuffer> messages = new Dictionary<string, MessageBuffer>();//new Dictionary<string, List<ChatMessage>>();

            private Dictionary<string, List<ChatMessage>> whisper = new Dictionary<string, List<ChatMessage>>();

            public MessageCollection()
            {
                Notifications.Load();
            }

            public delegate void WhisperReceivedHandler(ChatMessage message);

            public event WhisperReceivedHandler OnWhisperReceived;

            public Dictionary<string, MessageBuffer> Values
            {
                get { return messages; }
            }

            public Dictionary<string, List<ChatMessage>> Whispers
            {
                get { return whisper; }
            }

            public void Add(string Channel, string Message, bool executeActions, params MsgType[] types)
            {
                Channel = Channel.ToLower().Trim();
                ChatMessage m = new ChatMessage(Message, ChatUser.Emtpy, Channel, true, types);
                Add(Channel, m, executeActions);
            }

            public void Add(string Channel, string Message, params MsgType[] types)
            {
                Add(Channel, Message, false, types);
            }

            public void Add(string channelName, ChatMessage msg, bool executeActions)
            {
                try
                {
                    if (msg.IsEmpty) return;
                    lock (syncRootMessage)
                    {
                        if (msg.IsType(MsgType.HL_Messages))
                        {
                            Notifications.Highlight(channelName);
                        }
                        if (msg.IsType(MsgType.Whisper))
                        {
                            AddWhisper(channelName, msg);
                            if (Settings.BeepOnWhisper)
                            {
                                Notifications.Whisper(channelName);
                            }
                        }
                        else
                        {
                            if (!messages.ContainsKey(channelName))
                            {
                                AddChannel(channelName);
                            }
                            //try
                            //{
                            //    string s = JsonConvert.SerializeObject(msg);
                            //    File.AppendAllText(channelName + ".cache", s + "\r\n");
                            //}
                            //catch
                            //{
                            //}
                            messages[channelName].Add(msg);
                            messageCount[channelName]++;

                            //while (messages.Count > Settings.ChatHistory)
                            //{
                            //    messages[channelName].RemoveAt(0);
                            //}
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
                    messages.Add(Channel, new MessageBuffer(Channel));

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
                    //Dont count internal messages
                    //dont count resub as outgoing
                    //int i = null;
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
                                return messages.Sum(t => t.Value.Length);
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
                            //    if (name))
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

            public List<ChatMessage> GetMessages(string channel, string name, MsgType type, int start = -1, int end = -1)
            {
                channel = channel.ToLower().Trim();
                if (string.IsNullOrEmpty(name))
                {
                    if (messages.ContainsKey(channel))
                    {
                        if (type == MsgType.All_Messages)
                        {
                            return messages[channel].GetMessages(start, end);
                        }
                        else
                        {
                            return messages[channel].Where(t => t.IsType(type), start, end).ToList();
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
                        return messages[channel].Where(t => t.Name.Equals(name), start, end).ToList();
                    }
                }
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

        public class Notifications
        {
            //Flash both the window caption and taskbar button.
            //This is equivalent to setting the FLASHW_CAPTION | FLASHW_TRAY flags.
            public const UInt32 FLASHW_ALL = 3;

            // Flash continuously until the window comes to the foreground.
            public const UInt32 FLASHW_TIMERNOFG = 12;

            private static SoundPlayer highlightSound;
            private static SoundPlayer whisperSound;

            // Do the flashing - this does not involve a raincoat.
            public static bool FlashWindowEx(Form form)
            {
                IntPtr hWnd = form.Handle;
                FLASHWINFO fInfo = new FLASHWINFO();

                fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
                fInfo.hwnd = hWnd;
                fInfo.dwFlags = FLASHW_ALL | FLASHW_TIMERNOFG;
                fInfo.uCount = UInt32.MaxValue;
                fInfo.dwTimeout = 0;

                return FlashWindowEx(ref fInfo);
            }

            public static void Highlight(string channel)
            {
                highlightSound.Play();
                var form = channels[channel].ChatForm;
                if (form != null)
                {
                    FlashWindowEx(form);
                }
            }

            public static void Load()
            {
                whisperSound = new SoundPlayer(Settings.resourceDir + "whisper_alert.wav");
                highlightSound = new SoundPlayer(Settings.resourceDir + "highlight_alert.wav");
                whisperSound.LoadAsync();
                highlightSound.LoadAsync();
            }

            public static void Whisper(string name)
            {
                whisperSound.Play();
                //FlashWindowEx(new ApplicationContext().MainForm
            }

            // To support flashing.
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

            [StructLayout(LayoutKind.Sequential)]
            public struct FLASHWINFO
            {
                public UInt32 cbSize;
                public IntPtr hwnd;
                public UInt32 dwFlags;
                public UInt32 uCount;
                public UInt32 dwTimeout;
            }
        }
    }
}