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

            public ChatUser Get(string name, string channel, bool create = false)
            {
                try
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
                    if (create) return new ChatUser(name, channel);
                }
                catch
                {
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

        public class MessageBuffer : IDisposable
        {
            //private System.Threading.ReaderWriterLockSlim
            private static readonly object _readerWriterLock = new object();//new System.Threading.ReaderWriterLockSlim(System.Threading.LockRecursionPolicy.SupportsRecursion);

            private bool isLoading = true;
            private Dictionary<string, Dictionary<int, ChatMessage>> messages = new Dictionary<string, Dictionary<int, ChatMessage>>();//new List<ChatMessage>();
            private DataContext sql;

            public MessageBuffer()
            {
                try
                {
                    AllCount = new Dictionary<string, int>();
                    string path = Settings.caonfigBaseDir + "Database.db";
                    if (!File.Exists(path))
                    {
                        File.WriteAllBytes(path, LX29_LixChat.Properties.Resources.Cache);
                    }
                    sql = new DataContext(new SQLiteConnection(@"Data Source=" + path));
                    //Load Previous Messages
                    var tbl = sql.GetTable<CacheMessage>();
                    foreach (var t in tbl)
                    {
                        if (!messages.ContainsKey(t.Channel))
                        {
                            messages.Add(t.Channel, new Dictionary<int, ChatMessage>());
                            AllCount.Add(t.Channel, 0);
                        }
                        messages[t.Channel].Add(AllCount[t.Channel], new ChatMessage(t));
                        AllCount[t.Channel]++;
                    }
                }
                catch
                {
                }
                isLoading = false;
            }

            public Dictionary<string, int> AllCount
            {
                get;
                private set;
            }

            public IEnumerable<ChatMessage> AllMessages
            {
                get { return messages.Values.SelectMany(t => t.Values); }
            }

            private bool enableCaching
            {
                get { return Settings.MessageCaching; }
            }

            public IEnumerable<ChatMessage> this[string channel]
            {
                get { return messages[channel].Values; }
            }

            public void Add(ChatMessage msg)
            {
                try
                {
                    string channel = msg.Channel;
                    if (!messages.ContainsKey(channel))
                    {
                        AllCount.Add(channel, 0);
                        messages.Add(channel, new Dictionary<int, ChatMessage>());
                    }
                    int cnt = AllCount[channel];
                    if (!messages[channel].ContainsKey(cnt))
                    {
                        messages[channel].Add(cnt, msg);
                    }
                    else
                    {
                        messages[channel][cnt] = msg;
                    }
                    AllCount[channel] = cnt + 1;
                    while (messages.Count > Settings.ChatHistory)
                    {
                        int min = messages[channel].Keys.Min();
                        messages[channel].Remove(min);
                    }

                    if (enableCaching)
                    {
                        System.Threading.Tasks.Task.Run(() =>
                            {
                                while (isLoading) System.Threading.Thread.Sleep(100);

                                RunInsertSQL(new CacheMessage(msg, cnt));
                            });
                        //var msg = sql.GetTable<CacheMessage>();
                        //msg.InsertOnSubmit(new CacheMessage(message, cnt));
                        //sql.SubmitChanges(ConflictMode.FailOnFirstConflict);
                    }
                }
                catch
                {
                }
            }

            public bool ContainsKey(string channel)
            {
                return messages.ContainsKey(channel);
            }

            public bool ContainsKey(string channel, int index)
            {
                return messages[channel].ContainsKey(index);
            }

            public int Count(string channel, Func<ChatMessage, bool> a)
            {
                return messages[channel].Values.Count(a);
            }

            public void Dispose()
            {
                try
                {
                    sql.Connection.Close();
                    sql.Connection.Dispose();
                    sql.Dispose();
                }
                catch
                {
                }
            }

            public List<ChatMessage> GetMessages(string channel, int start, int end = -1, Func<ChatMessage, bool> a = null)
            {
                try
                {
                    while (isLoading) System.Threading.Thread.Sleep(100);
                    int allcnt = this.AllCount[channel];
                    if (messages.Count <= 0) return new List<ChatMessage>();
                    if (start < 0) start = Math.Max(0, allcnt - 256);
                    if (end < 0) end = Math.Min(allcnt, Math.Max(allcnt, start + 256));
                    Dictionary<int, ChatMessage> list = new Dictionary<int, ChatMessage>();

                    //int id = ChatClient.Channels[channel].ID;

                    //var sqlTable = sql.GetTable<CacheMessage>();
                    for (int i = start; i < end; i++)
                    {
                        if (messages[channel].ContainsKey(i))
                        {
                            var msg = messages[channel][i];
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
                        else if (enableCaching)
                        {
                            //var marr = sqlTable.Where(t => (t.Channel == id));
                            //if (marr.Count() > 0)
                            //{
                            // var m = marr.ToList().FirstOrDefault(t => (t.Index == i));
                            var msg = RunSelectSQL(channel, i);// new ChatMessage(m);

                            if (msg == null)
                                continue;

                            if (a != null)
                            {
                                if (a.Invoke(msg))
                                    list.Add(i, msg);
                            }
                            else
                            {
                                list.Add(i, msg);
                            }
                            // }
                        }
                    }
                    if (enableCaching) messages[channel] = list;
                    return list.Values.ToList();
                }
                catch
                {
                }
                return new List<ChatMessage>();
                //Settings.ChatHistory
            }

            public IEnumerable<ChatMessage> Where(string channel, Func<ChatMessage, bool> a)
            {
                return messages[channel].Values.Where(a);
            }

            private void RunInsertSQL(CacheMessage msg, int cnt = 0)
            {
                try
                {
                    lock (_readerWriterLock)
                    {
                        var tbl = sql.GetTable<CacheMessage>();

                        var m = tbl.Where(t => t.ID.Equals(msg.ID)).ToList().FirstOrDefault();
                        if (m != null)
                        {
                            m.From(msg);
                        }
                        else
                        {
                            tbl.InsertOnSubmit(msg);
                        }
                        sql.SubmitChanges(ConflictMode.FailOnFirstConflict);
                    }
                }
                catch
                {
                    System.Threading.Thread.Sleep(10);
                    if (cnt >= 10) throw new TimeoutException("RunInsertSQL more than 10 tries.");
                    RunInsertSQL(msg, cnt + 1);
                }

                //if (isbreaked)
                //{
                //    System.Threading.Thread.Sleep(10);
                //    if (cnt >= 10) throw new TimeoutException("RunInsertSQL more than 10 tries.");
                //    RunInsertSQL(msg, cnt + 1);
                //}
            }

            private ChatMessage RunSelectSQL(string channel, int index)
            {
                ChatMessage m = null;
                try
                {
                    lock (_readerWriterLock)
                    {
                        //Function to acess database
                        var sqlTable = sql.GetTable<CacheMessage>();
                        var marr = sqlTable.Where(t => t.Channel.Equals(channel)).ToList();
                        if (marr.Count > 0)
                        {
                            var msg = marr.FirstOrDefault(t => (t.Index == index));
                            if (msg != null)
                            {
                                //_readerWriterLock.ExitReadLock();
                                m = new ChatMessage(msg);
                            }
                            //_readerWriterLock.ExitReadLock();
                        }
                    }
                }
                catch
                {
                }
                return m;
            }

            //public List<ChatMessage> Where(Func<ChatMessage, bool> a)//, int start)
            //{
            //    return messages.Values.Where(a).ToList(); //GetMessages(start, this.AllCount, a);
            //}
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

            private MessageBuffer messages = new MessageBuffer();//new Dictionary<string, List<ChatMessage>>();

            private Dictionary<string, List<ChatMessage>> whisper = new Dictionary<string, List<ChatMessage>>();

            public MessageCollection()
            {
                Notifications.Load();
            }

            public delegate void WhisperReceivedHandler(ChatMessage message);

            public event WhisperReceivedHandler OnWhisperReceived;

            public MessageBuffer Values
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
                Add(m, executeActions);
            }

            public void Add(string Channel, string Message, params MsgType[] types)
            {
                Add(Channel, Message, false, types);
            }

            public void Add(ChatMessage msg, bool executeActions)
            {
                try
                {
                    if (msg.IsEmpty) return;
                    string channelName = msg.Channel;
                    //lock (syncRootMessage)
                    {
                        if (msg.IsType(MsgType.HL_Messages))
                        {
                            Notifications.Highlight(channelName);
                        }
                        if (msg.IsType(MsgType.Whisper))
                        {
                            AddWhisper(channelName, msg);

                            Notifications.Whisper(channelName);
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
                            messages.Add(msg);

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
                    //if (!messages.ContainsKey(channelName))
                    //{
                    //    AddChannel(channelName);
                    //}

                    Add(msg, true);
                }
            }

            //public void AddChannel(string Channel)
            //{
            //    if (!messages.ContainsKey(Channel))
            //    {
            //        messages.Add(Channel, null);
            //        messages[Channel] = new MessageBuffer(Channel);
            //    }
            //}

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
                    //lock (syncRootMessage)
                    {
                        if (type != MsgType.Whisper)
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
                    //lock (syncRootMessage)
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

            public List<ChatMessage> GetMessages(string channel, string name, MsgType type, int start = -1)
            {
                channel = channel.ToLower().Trim();
                if (string.IsNullOrEmpty(name))
                {
                    if (messages.ContainsKey(channel))
                    {
                        if (type == MsgType.All_Messages)
                        {
                            return messages.GetMessages(channel, start);
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

            public int MessageCount(string channel)
            {
                return messages.AllCount[channel];
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
                if (Settings.BeepOnHighlight)
                {
                    highlightSound.Play();
                    var form = channels[channel].ChatForm;
                    if (form != null)
                    {
                        FlashWindowEx(form);
                    }
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
                if (Settings.BeepOnWhisper)
                {
                    whisperSound.Play();
                    //FlashWindowEx(new ApplicationContext().MainForm
                }
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