using LX29_ChatClient.Addons;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.SQLite;
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
            private List<ChatUser> users;
            private static readonly object syncRoot = new object();

            public ChatUserCollection()
            {
                users = new List<ChatUser>();
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
                    lock (syncRoot)
                    {
                        string channel = user.Channel;
                        string name = user.Name;
                        if (!Contains(name, channel))
                        {
                            users.Add(user);
                        }
                        else
                        {
                            var u = users.FindIndex(t => t.Channel.Equals(channel) && t.Name.Equals(name));
                            users[u] = user;
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
                //    if (!Contains(name, channel))
                //    {
                //        var user = new ChatUser(channel, parameters, name, toResult);
                //        Add(user);
                //    }
                //}

                try
                {
                    if (channel.Length == 0 || name.Length == 0)
                    {
                        return;
                    }
                    lock (syncRoot)
                    {
                        if (!Contains(name, channel))
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
                            users.Add(cu);
                        }
                        else
                        {
                            var user = users.First(t => t.Channel.Equals(channel) && t.Name.Equals(name));
                            user.Parse(channel, parameters, name, toResult);
                        }
                    }
                }
                catch { }
            }

            //public void Append(string channel)
            //{
            //    if (!users.ContainsKey(channel))
            //    {
            //        users.Add(channel, new Dictionary<string, ChatUser>());
            //    }
            //}
            public ChatUser FirstOrDefault(string name, string channel)
            {
                lock (syncRoot)
                {
                    return users.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && t.Channel.Equals(channel, StringComparison.OrdinalIgnoreCase));
                }
            }

            public bool Contains(string name, string channel)
            {
                lock (syncRoot)
                {
                    return users.Any(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && t.Channel.Equals(channel, StringComparison.OrdinalIgnoreCase));
                }
            }

            public int Count(string channel)
            {
                lock (syncRoot)
                {
                    return users.Count(t => t.Channel.Equals(channel, StringComparison.OrdinalIgnoreCase));
                }
            }

            public int Count(string channel, Func<ChatUser, bool> predicate)
            {
                lock (syncRoot)
                {
                    return users.Where(t => t.Channel.Equals(channel, StringComparison.OrdinalIgnoreCase)).Count(predicate);
                }
            }

            public IEnumerable<ChatUser> Find(string name, string channel)
            {
                lock (syncRoot)
                {
                    return users.Where(t => t.Channel.Equals(channel, StringComparison.OrdinalIgnoreCase) && t.Name.StartsWith(name));
                }
            }

            public ChatUser Get(string name, string channel, bool create = false)
            {
                try
                {
                    if (!string.IsNullOrEmpty(name))
                    {
                        if (!string.IsNullOrEmpty(channel))
                        {
                            channel = channel.ToLower().Trim();
                            name = name.ToLower().Trim();
                            lock (syncRoot)
                            {
                                var user = users.FirstOrDefault(t => t.Channel.Equals(channel) && t.Name.Equals(name));
                                if (user != null && !user.IsEmpty)
                                {
                                    return user;
                                }
                            }
                            //foreach (string s in users.Keys)
                            //{
                            //    if (users.ContainsKey(name))
                            //    {
                            //        return users[name];
                            //    }
                            //}
                        }
                        //else if (users.ContainsKey(channel) && users[channel].ContainsKey(name))
                        //{
                        //    return users[channel][name];
                        //}
                    }
                    if (create) return new ChatUser(name, channel);
                }
                catch
                {
                }
                return ChatUser.Emtpy;
            }

            public IEnumerable<ChatUser> Get(string channel)
            {
                lock (syncRoot)
                {
                    return users.Where(t => t.Channel.Equals(channel, StringComparison.OrdinalIgnoreCase));
                }
                //if (users.ContainsKey(channel.ToLower().Trim()))
                //{
                //    return users[channel.ToLower().Trim()];
                //}
                //return null;
            }

            public string[] GetAllNames()
            {
                lock (syncRoot)
                {
                    return users.Select(t => t.Name).ToArray();
                }
            }

            public void SetOffline(string name, string channel)
            {
                //var user = users.FirstOrDefault(t=>t.cha)
                if (Contains(name, channel))
                {
                    Get(name, channel).IsOnline = false;//    users[channel][key].IsOnline = false;
                }
                else
                {
                    //addChannel(channel);
                    Add(name, channel);
                    Get(name, channel).IsOnline = false;
                }
            }

            private List<ChatUser> Users(string channel)
            {
                return users;
            }
        }

        public class MessageBuffer : IDisposable
        {
            private static readonly InlineComparer<KeyValuePair<int, ChatMessage>> dupComparer =
                new InlineComparer<KeyValuePair<int, ChatMessage>>((i1, i2) => i1.Value.SendTime.Ticks == i1.Value.SendTime.Ticks, i2 => (int)i2.Value.SendTime.Ticks);

            private Dictionary<string, int> AllCount = new Dictionary<string, int>();
            private bool isLoading = true;
            private Dictionary<string, Dictionary<int, ChatMessage>> messages = new Dictionary<string, Dictionary<int, ChatMessage>>();//new List<ChatMessage>();
            private SQL_Handler<CacheMessage> sql;

            public MessageBuffer()
            {
                //int i = null;
                //TODO
                //Dont count internal messages
                //dont count resub as outgoing

                //Fix Cinema Mode view
                //Add top-chat-controls to chatpanel????????????
            }

            public Dictionary<string, List<ChatMessage>> AllMessages
            {
                get { return messages.ToDictionary(t => t.Key, v => v.Value.Values.ToList()); }
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
                    string channel = msg.Channel_Name;
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
                    //Int16.MaxValue / 2 = 16383
                    AllCount[channel] = (cnt + 1 > (Int16.MaxValue / 2)) ? 0 : cnt + 1;

                    while (messages.Count > Settings.ChatHistory)
                    {
                        int min = messages[channel].Keys.Min();
                        messages[channel].Remove(min);
                    }

                    if (enableCaching)
                    {
                        if (!isLoading)
                        {
                            sql.Add(new CacheMessage(msg, cnt));
                        }
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

            public int Count(string channel)
            {
                return AllCount[channel];
            }

            public void Dispose()
            {
                try
                {
                    sql.Dispose();
                }
                catch
                {
                }
            }

            public List<ChatMessage> GetMessages(string channel, int start, int end)//, Func<ChatMessage, bool> a = null)
            {
                try
                {
                    //int allmax = 256;
                    if (isLoading) return null;
                    int allcnt = this.AllCount[channel];
                    if (messages.Count <= 0) return new List<ChatMessage>();
                    start = Math.Min(end, Math.Max(0, start));
                    end = Math.Min(allcnt, Math.Max(end, start));

                    var list = messages[channel].Where(t => t.Key >= start && t.Key < end);
                    if (enableCaching)
                    {
                        //if (list.Count() < (end - start))
                        {
                            var sqlMsges = sql.Where(t => (t.Channel.Equals(channel) && (t.Index >= start && t.Index < end)));
                            if (sqlMsges.Count > 0)
                            {
                                list = list.Union(sqlMsges.Select(t => new KeyValuePair<int, ChatMessage>(t.Index, new ChatMessage(t))), dupComparer);
                            }
                        }
                        var temp = list.ToList();
                        foreach (var m in temp)
                        {
                            if (messages[channel].ContainsKey(m.Key))
                            {
                                messages[channel][m.Key] = m.Value;
                            }
                            else
                            {
                                messages[channel].Add(m.Key, m.Value);
                            }
                        }
                        // messages[channel] = temp.ToDictionary(t => t.Key, t => t.Value);
                    }
                    return messages[channel].Values.ToList();// list.Select(t => t.Value).ToList();
                }
                catch
                {
                }
                return null;
            }

            public void Load()
            {
                string path = Settings._caonfigBaseDir + "Cache.db";
                try
                {
                    AllCount = new Dictionary<string, int>();
                    //Load Previous Messages
                    FileInfo fi = new FileInfo(path);
                    if (!fi.Exists || fi.Length == 0)
                    {
                        File.WriteAllBytes(path, LX29_LixChat.Properties.Resources.Cache);
                    }
                    sql = new SQL_Handler<CacheMessage>(path, (tbl, msg) =>
                        {
                            try
                            {
                                var m = tbl.Where(t => t.Equals(msg));
                                if (m != null)
                                {
                                    var l = m.ToList();
                                    if (l.Count > 0)
                                    {
                                        var mm = l[0];
                                        mm.From(msg);
                                        return true;
                                    }
                                }
                            }
                            catch
                            {
                            }
                            return false;
                        });// SQLiteConnection(@"Data Source=" + path);
                    if (Settings.MessageCaching)
                    {
                        System.Threading.Tasks.Task.Run(() =>
                        {
                            try
                            {
                                using (var context = sql.GetContext())
                                {
                                    var tbl = context.GetTable<CacheMessage>();
                                    foreach (var t in tbl)
                                    {
                                        if (!messages.ContainsKey(t.Channel))
                                        {
                                            messages.Add(t.Channel, new Dictionary<int, ChatMessage>());
                                            AllCount.Add(t.Channel, 0);
                                        }
                                        int cnt = AllCount[t.Channel];
                                        if (cnt <= Settings.ChatHistory)
                                        {
                                            var msg = new ChatMessage(t);
                                            messages[t.Channel].Add(cnt, msg);

                                            AllCount[t.Channel] = (cnt + 1 > (Int16.MaxValue / 2)) ? 0 : cnt + 1;
                                            if (OnMessageReceived != null)
                                                OnMessageReceived(msg);
                                        }
                                    }
                                }
                            }
                            catch
                            {
                            }
                            isLoading = false;
                        });
                    }
                    else
                    {
                        isLoading = false;
                    }
                }
                catch (Exception x)
                {
                    x.Handle("", true);
                    File.Delete(path);
#if DEBUG
                    Load();
#endif
                }
                finally
                {
                    if (!Settings.MessageCaching) isLoading = false;
                }
            }

            public IEnumerable<ChatMessage> Where(string channel, Func<ChatMessage, bool> a)
            {
                return messages[channel].Values.Where(a);
            }

            public class InlineComparer<T> : IEqualityComparer<T>
            {
                private readonly Func<T, T, bool> getEquals;
                private readonly Func<T, int> getHashCode;

                public InlineComparer(Func<T, T, bool> equals, Func<T, int> hashCode)
                {
                    getEquals = equals;
                    getHashCode = hashCode;
                }

                public bool Equals(T x, T y)
                {
                    return getEquals(x, y);
                }

                public int GetHashCode(T obj)
                {
                    return getHashCode(obj);
                }
            }
        }

        public class MessageCollection // : IDictionary<string, Dictionary<string, ChatUser>>
        {
            private MessageBuffer messages;//new Dictionary<string, List<ChatMessage>>();

            private Dictionary<string, List<ChatMessage>> whisper = new Dictionary<string, List<ChatMessage>>();

            public MessageCollection()
            {
                messages = new MessageBuffer();
                messages.Load();
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
                    string channelName = msg.Channel_Name;

                    if (msg.IsType(MsgType.HL_Messages))
                    {
                        Notifications.Highlight(msg.Channel.ID);
                    }
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

                        messages.Add(msg);
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
                //if (!users.Contains(name, name))
                //{
                //    users.Add(new ChatUser(name, name));
                //}
                if (!whisper.ContainsKey(name))
                {
                    whisper.Add(name, new List<ChatMessage>());
                    whisper[name].Add(msg);

                    if (!msg.Name.Equals(ChatClient.SelfUserName))
                    {
                        Notifications.Whisper(name);
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
                            Notifications.Whisper(name);
                            if (OnWhisperReceived != null)
                                OnWhisperReceived(msg);
                        }
                    }
                }
            }

            public int Count(string channel, MsgType type = MsgType.All_Messages, string name = "")
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
                                    if (type == MsgType.All_Messages) return messages.Count(channel);
                                    else return messages[channel].Count(t => t.IsType(type));
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

            public List<ChatMessage> GetMessages(string channel, string name, MsgType type, int start, int end)
            {
                channel = channel.ToLower().Trim();
                if (string.IsNullOrEmpty(name))
                {
                    if (messages.ContainsKey(channel))
                    {
                        if (type == MsgType.All_Messages)
                        {
                            return messages.GetMessages(channel, start, end);
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

            //public int MessageCount(string channel, string user)
            //{
            //    return messages[channel].Count(t => t.Name.Equals(user, StringComparison.OrdinalIgnoreCase));
            //}
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

            public static void Highlight(int channel)
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
                whisperSound = new SoundPlayer(Settings._resourceDir + "whisper_alert.wav");
                highlightSound = new SoundPlayer(Settings._resourceDir + "highlight_alert.wav");
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

        public class SQL_Handler<T> where T : class
        {
            private static readonly object _readerWriterLock = new object();
            private List<T> buffer = new List<T>();
            private int bufferWriteDelay = 5000;
            private Func<Table<T>, T, bool> insertComparer;
            private SQLiteConnection sql;
            private LXTimer timer;

            public SQL_Handler(string path, Func<Table<T>, T, bool> comparer)
            {
                insertComparer = comparer;
                //string path = Settings.caonfigBaseDir + databaseFile;
                sql = new SQLiteConnection(@"Data Source=" + path);
                timer = new LXTimer(new Action<LXTimer>(bufferWrite), bufferWriteDelay, bufferWriteDelay);
            }

            public int BufferWriteDelay
            {
                get { return bufferWriteDelay; }
                set { bufferWriteDelay = Math.Max(100, value); }
            }

            public void Add(T item)
            {
                buffer.Add(item);
            }

            public void Dispose()
            {
                try
                {
                    timer.Change(-1, -1);
                    sql.Close();
                    sql.Dispose();
                }
                catch
                {
                }
            }

            public DataContext GetContext()
            {
                return new DataContext(sql);
            }

            public List<T> Where(Func<T, bool> select)
            {
                try
                {
                    List<T> list = new List<T>();

                    using (DataContext dc = new DataContext(sql))
                    {
                        var sqlTable = dc.GetTable<T>();
                        lock (_readerWriterLock)
                        {
                            var te = sqlTable.Where(select);
                            if (te != null)
                            {
                                list = te.ToList();
                            }
                        }
                    }
                    return list;
                }
                catch
                {
                }
                return null;
            }

            private void bufferWrite(LXTimer t)
            {
                t.Change(-1, 0);
                try
                {
                    int cnt = buffer.Count;
                    if (cnt > 0)
                    {
                        using (DataContext dc = new DataContext(sql))
                        {
                            var tbl = dc.GetTable<T>();
                            for (int i = 0; i < cnt; i++)
                            {
                                if (!insertComparer(tbl, buffer[i]))
                                {
                                    tbl.InsertOnSubmit(buffer[i]);
                                }
                            }
                            lock (_readerWriterLock)
                            {
                                buffer.RemoveRange(0, cnt);
                                dc.SubmitChanges(ConflictMode.FailOnFirstConflict);
                            }
                        }
                    }
                }
                catch
                {
                }
                t.Change(bufferWriteDelay, bufferWriteDelay);
            }
        }

        public class WhisperCollection
        {
            private DataContext sql;

            public WhisperCollection()
            {
                string path = Settings._caonfigBaseDir + "Whisper.db";
                if (!File.Exists(path))
                {
                    File.WriteAllBytes(path, LX29_LixChat.Properties.Resources.Cache);
                }
                sql = new DataContext(new SQLiteConnection(@"Data Source=" + path));
            }

            [Table(Name = "Whisper")]
            public class Whisper
            {
                public Whisper(ChatMessage msg)
                {
                    Message = msg.Message;
                    Name = msg.Name;
                    Time = msg.SendTime.Ticks;

                    Outgoing = msg.IsType(MsgType.Outgoing);
                }

                [Column(Name = "Message")]
                public string Message { get; set; }

                [Column(Name = "Name")]
                public string Name { get; set; }

                [Column(Name = "Outgoing")]
                public bool Outgoing { get; set; }

                [Column(Name = "Time", IsPrimaryKey = true)]
                public long Time { get; set; }
            }
        }
    }
}