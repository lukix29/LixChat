﻿using Newtonsoft.Json;
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

    public class AutoActions
    {
        private List<ChatAction> chatactions = new List<ChatAction>();
        //private AutoChatActionsControl formAutoChatActions = null;

        public AutoActions()
        {
            chatactions = new List<ChatAction>();
            //EnableActions = true;
        }

        //[JsonIgnore]
        //public bool ChatActionShowing
        //{
        //    get;
        //    set;
        //}

        //[JsonRequired]
        //public bool EnableActions
        //{
        //    get;
        //    private set;
        //}

        [JsonRequired]
        public List<ChatAction> Values
        {
            get { return chatactions; }
            set { chatactions = value; }
        }

        //[JsonIgnore]
        //public bool Loaded
        //{
        //    get;
        //    private set;
        //}

        public static AutoActions Load()
        {
            string path = Settings._dataDir + "AutoActions.json";

            var action = LoadOld();
            if (action == null)
            {
                if (System.IO.File.Exists(path))
                {
                    return JsonConvert.DeserializeObject<AutoActions>(System.IO.File.ReadAllText(path));
                }
            }
            return new AutoActions();
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
                            bool geq = ca.Channel.StartsWith("*");
                            bool cheq = ca.Channel.Equals(msg.Channel_Name, StringComparison.OrdinalIgnoreCase);
                            bool msgL = (ca.Message.Length > 0);
                            bool actgL = (ca.Action.Length > 0);
                            if (cd && (geq || cheq) && msgL && actgL)
                            {
                                if (ca.Contains(message))
                                {
                                    System.Threading.Tasks.Task.Run(async () =>
                                    {
                                        if (ca.Delay > 0) await System.Threading.Tasks.Task.Delay(ca.Delay * 1000);
                                        ChatClient.SendMessage(ca.GetAction(msg), msg.Channel.ID);
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

        public List<ChatAction> GetChannelActions()//string Channel)
        {
            return chatactions.Select(t => (ChatAction)t.Clone()).ToList();//.Where(t =>
            //(t.Channel.Equals(Channel, StringComparison.OrdinalIgnoreCase)));
            //||t.Channel.Equals("global")));
        }

        public List<ChatAction>.Enumerator GetEnumerator()
        {
            return chatactions.GetEnumerator();
        }

        //public void OpenChatActions()
        //{
        //    if (ChatActionShowing)
        //    {
        //        ChatActionShowing = false;
        //        if (!formAutoChatActions.IsDisposed)
        //        {
        //            // formAutoChatActions.Close();
        //            formAutoChatActions.Dispose();
        //        }
        //    }
        //    else
        //    {
        //        formAutoChatActions = new LX29_ChatClient.Addons.AutoChatActionsControl();
        //        formAutoChatActions.Show();
        //        ChatActionShowing = true;
        //    }
        //}

        public void Save(IEnumerable<ChatAction> actions)
        {
            try
            {
                chatactions.Clear();
                chatactions.AddRange(actions.Select(t => (ChatAction)t.Clone()));
                string path = Settings._dataDir + "AutoActions.json";
                //using (StreamWriter sw = new StreamWriter(path, false))
                //{
                //    foreach (var act in actions)
                //    {
                File.WriteAllText(path, Newtonsoft.Json.JsonConvert.SerializeObject(this));
                //    }
                //}
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case System.Windows.Forms.MessageBoxResult.Retry:
                        Save(actions);
                        break;
                }
            }
        }

        private static AutoActions LoadOld()
        {
            try
            {
                string path = Settings._dataDir + "AutoActions.txt";
                if (System.IO.File.Exists(path))
                {
                    AutoActions actions = new AutoActions();
                    string input = System.IO.File.ReadAllText(path);
                    if (!input.Contains("\r\n"))
                    {
                        input = input.Replace("},{", "}\r\n{").Replace("}]", "}").Replace("[{", "{");
                    }
                    var sa = input.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var val in sa)
                    {
                        ChatAction action = Newtonsoft.Json.JsonConvert.DeserializeObject<ChatAction>(val);
                        actions.Values.Add(action);
                    }
                    //System.IO.File.Delete(path);
                    return actions;
                }
            }
            catch
            {
            }
            return null;
        }
    }

    public class ChatAction //: CustomSettings<ChatAction, ChatActionType>, IEquatable<ChatAction>, ICloneable
    {
        private DateTime lastAction_TMi = DateTime.MinValue;

        private int lastRDmsg = -1;

        private Random rd = new Random(LX29_Cryptography.LX29Crypt.GetSeed());

        public ChatAction()
        {
            Channel = "";
            Username = "";
            Message = "";
            Action = "";
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

        [Newtonsoft.Json.JsonIgnore]
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

        [Newtonsoft.Json.JsonIgnore]
        public bool IsEmpty
        {
            get
            {
                return string.IsNullOrEmpty(Username) ||
                    string.IsNullOrEmpty(Message) ||
                    string.IsNullOrEmpty(Action);
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

        public object Clone()
        {
            return new ChatAction()
            {
                Channel = this.Channel,
                Action = this.Action,
                CheckStart = this.CheckStart,
                Cooldown = this.Cooldown,
                Delay = this.Delay,
                Enabled = this.Enabled,
                Message = this.Message,
                Username = this.Username,
                lastAction_TMi = this.lastAction_TMi,
                lastRDmsg = this.lastRDmsg
            };
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
            int cnt = 0;
            while (cnt < 32)
            {
                if (sout.Contains("&rd["))
                {
                    string mi = sout.GetBetween("&rd[", "]");
                    string[] sa = mi.Split(",");
                    if (int.TryParse(sa[0], out int min) && int.TryParse(sa[1], out int max))
                    {
                        sout = sout.Replace("&rd[" + mi + "]", rd.Next(min, max + 1).ToString());
                    }
                    else if (mi.Equals("user", StringComparison.OrdinalIgnoreCase))
                    {
                        var list = ChatClient.ChatUsers.Users[msg.Channel_Name];
                        getHumanRandom(0, list.Count);
                        string lastLine = list[lastRDmsg].DisplayName;
                        sout = sout.Replace("&rd[" + mi + "]", lastLine);
                    }
                    else if (mi.Length >= 3 && mi.Contains("."))
                    {
                        if (File.Exists(Settings._scriptDir + mi))
                        {
                            var arr = File.ReadLines(Settings._scriptDir + mi).Where(t => t.Length >= 3).ToList();
                            getHumanRandom(0, arr.Count - 1);
                            string lastLine = arr.ElementAt(lastRDmsg);
                            sout = sout.Replace("&rd[" + mi + "]", lastLine);
                        }
                    }
                }
                else if (sout.Contains("&word["))
                {
                    string mi = sout.GetBetween("&word[", "]");
                    if (int.TryParse(mi, out int idx))
                    {
                        idx = Math.Max(0, Math.Min(msg.ChatWords.Count - 1, idx));
                        sout = sout.Replace("&word[" + mi + "]", msg.ChatWords[idx].Text);
                    }
                }
                else if (sout.Contains("&name"))
                {
                    string name = msg.Name;
                    if (!msg.User.IsEmpty)
                    {
                        name = msg.User.DisplayName;
                    }
                    sout = sout.Replace("&name", name);
                }
                else if (sout.Contains("&channel"))
                {
                    string name = msg.Channel_Name;
                    ChatUser user = ChatClient.ChatUsers.Get(name, name);
                    if (!user.IsEmpty)
                    {
                        name = user.DisplayName;
                    }
                    sout = sout.Replace("&channel", name);
                }
                else break;
                cnt++;
            }
            lastAction_TMi = DateTime.Now;
            return sout;
        }

        public override string ToString()
        {
            return "USERNAME=" + Username + " | MESSAGE=" + Message + " | ACTION=" + Action;
        }

        private void getHumanRandom(int min, int max)
        {
            int index = lastRDmsg;
            while (index == lastRDmsg)
            {
                lastRDmsg = rd.Next(min, max);
            }
        }

        //public bool Load(Dictionary<string, string> values)
        //{
        //    return base.Load(values, this);
        //}

        //public string Save()
        //{
        //    return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        //}
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