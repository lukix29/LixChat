using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LX29_ChatClient.Addons
{
    public enum ChatActionType
    {
        Action,
        Channel,
        Cooldown,
        Enabled,
        CheckStart,
        Message,
        Username,
        Delay
    }

    public class AutoActions// : IEnumerable<ChatAction>
    {
        //        public const string ActionHelp =
        //"Username aliases:\r\n\tcase insensitive, * means any user can trigger the Action.\r\nKeywords for Action:\r\n\t&rd[minimum,maximum] = generates a random Number.\r\n\t&rd[FileName] = takes a random Line from \"Filename\".\r\n\t\t-File must be in Script Folder (can be found in Settings)\r\n\t&name = replace with caller name. (who has triggered the Action)\r\n\t&channel = replace with channel name.\r\n\t&word[index] replace with channel name";

        private List<ChatAction> chatactions = new List<ChatAction>();
        private LX29_ChatClient.Addons.FormAutoChatActions formAutoChatActions = null;

        public AutoActions()
        {
            chatactions = new List<ChatAction>();
            EnableActions = false;
        }

        public bool ChatActionShowing
        {
            get;
            set;
        }

        public bool EnableActions
        {
            get;
            set;
        }

        public ChatAction this[int index]
        {
            get { return chatactions[index]; }
            set { chatactions[index] = value; }
        }

        public bool ChangeChatAction(ChatAction action)
        {
            if (!action.IsEmpty)
            {
                if (!chatactions.Contains(action))
                {
                    chatactions.Add(action);
                    return true;
                }
                else
                {
                    var idx = chatactions.IndexOf(action);
                    chatactions[idx] = action;
                    return true;
                }
            }
            return false;
        }

        public void CheckActions(ChatMessage msg)
        {
            if (chatactions != null && chatactions.Count > 0)
            {
                try
                {
                    string username = msg.Name;

                    //if (username))
                    //    return;
                    List<ChatAction> list = chatactions.FindAll(c => (c.Username.ToLower().Contains(username)));

                    if (list != null && list.Count == 0)
                    {
                        list = chatactions.FindAll(c => c.Username.StartsWith("*"));
                    }
                    if (list != null && list.Count > 0)
                    {
                        string message = msg.Message.ToLower();
                        foreach (ChatAction ca in list)
                        {
                            //if (msg.IsType(ca.MsgType) &&
                            bool cd = (!ca.IsCooldown);
                            bool geq = ca.Channel.Equals("global", StringComparison.OrdinalIgnoreCase);
                            bool cheq = ca.Channel.Equals(msg.Channel, StringComparison.OrdinalIgnoreCase);
                            bool msgL = (ca.Message.Length > 0);
                            bool actgL = (ca.Action.Length > 0);
                            if (cd && (geq || cheq) && msgL && actgL)
                            {
                                if (ca.Contains(message))
                                {
                                    System.Threading.Tasks.Task.Run(async () =>
                                    {
                                        if (ca.Delay > 0) await System.Threading.Tasks.Task.Delay(ca.Delay * 1000);
                                        ChatClient.SendMessage(ca.GetAction(msg), msg.Channel);
                                    });
                                }
                            }
                        }
                        //endfor
                    }
                    //endif
                }
                catch (Exception x)
                {
                    switch (x.Handle())
                    {
                        case System.Windows.Forms.MessageBoxResult.Retry:
                            CheckActions(msg);
                            break;
                    }
                }
            }
        }

        public ChatAction FirstOrDefault(Func<ChatAction, bool> predicate)
        {
            return chatactions.FirstOrDefault(predicate);
        }

        public List<ChatAction> GetChannelActions(string Channel)
        {
            return chatactions.Where(t =>
                (t.Channel.Equals(Channel, StringComparison.OrdinalIgnoreCase))).ToList();
            //||t.Channel.Equals("global")));
        }

        //public ChatAction GetChatAction(Func<ChatAction, bool> predicate)
        //{
        //    return chatactions.FirstOrDefault(predicate);
        //}

        public List<ChatAction>.Enumerator GetEnumerator()
        {
            return chatactions.GetEnumerator();
        }

        public void Load()
        {
            string path = Settings.dataDir + "AutoActions.txt";
            if (System.IO.File.Exists(path))
            {
                chatactions.Clear();
                var sa = System.IO.File.ReadAllText(path);
                var di = CustomSettings.LoadList(sa);
                foreach (var val in di)
                {
                    string Channel = val["Channel"];
                    ChatAction action = new ChatAction(Channel);
                    action.Load(val);
                    chatactions.Add(action);
                }
            }
        }

        public void OpenChatActions(Channels.ChannelInfo name)
        {
            if (ChatActionShowing)
            {
                ChatActionShowing = false;
                if (!formAutoChatActions.IsDisposed)
                {
                    formAutoChatActions.Close();
                    formAutoChatActions.Dispose();
                }
            }
            else
            {
                formAutoChatActions = new LX29_ChatClient.Addons.FormAutoChatActions(name);
                formAutoChatActions.Show();
                ChatActionShowing = true;
            }
        }

        public void RemoveAt(Predicate<ChatAction> match)
        {
            chatactions.RemoveAll(match);
        }

        public void Save()
        {
            string path = Settings.dataDir + "AutoActions.txt";
            System.IO.File.WriteAllText(path, GetChatActions());
        }

        //public void SetChatAction(int idx, ChatAction action)
        //{
        //    if (idx >= 0 && idx < chatactions.Count)
        //    {
        //        chatactions[idx] = action;
        //    }
        //}

        //public IEnumerable<ChatAction> Where(Func<ChatAction, bool> predicate)
        //{
        //    return chatactions.Where(predicate);
        //}

        private string GetChatActions()
        {
            var list = chatactions.Select(t => t.Save());
            var s = CustomSettings.SaveList(list);
            //var s = CustomSettings.GetSettings<ChatAction, ChatActionType>(
            //chatactions, new Func<ChatAction, string>(t => t.Channel));
            return s;
        }
    }

    public class ChatAction : CustomSettings<ChatAction, ChatActionType>, IEquatable<ChatAction>
    {
        private DateTime lastAction_TMi = DateTime.MinValue;

        private int lastRDmsg = -1;

        public ChatAction(string channel)
        {
            Channel = channel;
        }

        public ChatAction(string[] input)
        {
            //if (!Load(input, this))
            //{
            //}
        }

        public ChatAction(string username, string channel, string message,
            string action, int cooldown, bool checkStart, int delay, bool enabled)
        {
            this.Action = action;
            this.Channel = channel;
            this.CheckStart = checkStart;
            this.Cooldown = cooldown;
            this.Delay = delay;
            this.Enabled = enabled;
            this.Message = message;
            this.Username = username;
        }

        public string Action
        {
            get;
            set;
        }

        public string Channel
        {
            get;
            set;
        }

        public bool CheckStart
        {
            get;
            set;
        }

        public int Cooldown
        {
            get;
            set;
        }

        public int Delay
        {
            get;
            set;
        }

        public bool Enabled
        {
            get;
            set;
        }

        public bool IsCooldown
        {
            get
            {
                if (DateTime.Now.Subtract(lastAction_TMi).TotalSeconds < (Cooldown + 0.5))
                {
                    return true;
                }
                return false;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return Username.IsEmpty() ||
                    Message.IsEmpty() ||
                    Action.IsEmpty();
            }
        }

        public string Message
        {
            get;
            set;
        }

        public string Username
        {
            get;
            set;
        }

        public bool Contains(string message)
        {
            if (CheckStart)
            {
                return message.StartsWith(Message.ToLower());
            }
            else
            {
                return message.Contains(Message.ToLower());
            }
        }

        public bool Equals(ChatAction obj)
        {
            return obj.ToString().Equals(this.ToString());
        }

        public bool Equals(string obj)
        {
            return obj.Equals(this.ToString());
        }

        public string GetAction(ChatMessage msg)
        {
            if (Action.StartsWith("//")) return "";
            string sout = Action;
            if (sout.Contains("&rd["))
            {
                Random rd = new Random();
                int min = 0;
                int max = 0;
                string mi = sout.GetBetween("&rd[", "]");
                string[] sa = mi.Split(",");
                if (int.TryParse(sa[0], out min) && int.TryParse(sa[1], out max))
                {
                    sout = sout.Replace("&rd[" + mi + "]", rd.Next(min, max + 1).ToString());
                }
                else
                {
                    if (File.Exists(Settings.scriptDir + mi))
                    {
                        var arr = File.ReadLines(Settings.scriptDir + mi, System.Text.Encoding.UTF8).Where(t => t.Length >= 3);
                        int index = lastRDmsg;
                        while (index == lastRDmsg)
                        {
                            lastRDmsg = rd.Next(0, arr.Count() - 1);
                        }
                        string lastLine = arr.ElementAt(lastRDmsg);
                        sout = sout.Replace("&rd[" + mi + "]", lastLine);
                    }
                }
            }
            if (sout.Contains("&word["))
            {
                int idx = 0;
                string mi = sout.GetBetween("&word[", "]");
                if (int.TryParse(mi, out idx))
                {
                    idx = Math.Max(0, Math.Min(msg.ChatWords.Count - 1, idx));
                    sout = sout.Replace("&word[" + mi + "]", msg.ChatWords[idx].Text);
                }
            }
            if (sout.Contains("&name"))
            {
                string name = msg.Name;
                if (!msg.User.IsEmpty)
                {
                    name = msg.User.DisplayName;
                }
                sout = sout.Replace("&name", name);
            }
            if (sout.Contains("&channel"))
            {
                string name = msg.Channel;
                ChatUser user = ChatClient.Users.Get(name, name);
                if (!user.IsEmpty)
                {
                    name = user.DisplayName;
                }
                sout = sout.Replace("&channel", name);
            }
            lastAction_TMi = DateTime.Now;
            return sout;
        }

        public bool Load(Dictionary<string, string> values)
        {
            return base.Load(values, this);
        }

        public string Save()
        {
            return base.Save(this);
        }

        public override string ToString()
        {
            return "USERNAME=" + Username + " | MESSAGE=" + Message + " | ACTION=" + Action;
        }
    }

    public class CustomAnalytics
    {
        //22:25  InzpektorGadget: Mehr Dschibbse für: lukix29 - 65 (29), tox1c90 - 94 (42), ellabama_ - 94 (42), oberst_l - 11 (5), cradoxlive - 94 (42), aceboy1984 - 94 (42), punishbear - 94 (42), o_neill90 - 94 (42), tim3ywimey - 94 (42), freestayl - 94 (42), impioi - 94 (42), leppits - 94 (42), lyngar - 94 (42), eraser_tm - 67 (30)
        ////22:23  InzpektorGadget: Obacht! lukix29 hat doch tatsächlich vor in die Dschibbstüte zu greifen! Willst du auch Dschibbs?! Schreibe !nom [1-42] für fettige Finger!
        //22:26  InzpektorGadget: Dschibbs : milleniumplyer hat 2.999 Dschibbs und 208h zugeschaut! PogChamp

        //public Dictionary<string,string>

        //public CustomAnalytics()
        //{
        //}
    }
}