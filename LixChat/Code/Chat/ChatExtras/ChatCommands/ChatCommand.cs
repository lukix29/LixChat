using LX29_Helpers;
using System;
using System.Collections.Generic;

namespace LX29_ChatClient.Addons.Commands
{
    public enum CommandValueType
    {
        Command,
    }

    public class ChatCommand : CustomSettings<ChatCommand, CommandValueType>, IEquatable<ChatCommand>
    {
        public ChatCommand(string command)
        {
            Command = command;
        }

        public ChatCommand(string[] input)
        {
            if (!Load(input, this))
            {
            }
        }

        public string Command
        {
            get;
            set;
        }

        public bool Equals(ChatCommand obj)
        {
            return obj.ToString().Equals(ToString());
        }

        public string Save()
        {
            return base.Save(this);
        }

        public override string ToString()
        {
            return "Command=" + Command;
        }
    }

    public class ChatCommandCollection : IEnumerable<ChatCommand>
    {
        private List<ChatCommand> list = new List<ChatCommand>();

        public ChatCommandCollection()
        {
        }

        public ChatCommand this[int index]
        {
            get { return list[index]; }
            set { list.Insert(index, value); }
        }

        public void Add(ChatCommand item)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
            }
        }

        public IEnumerator<ChatCommand> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Load()
        {
            string path = Settings.dataDir + "ChatCommand.txt";
            if (System.IO.File.Exists(path))
            {
                var sa = System.IO.File.ReadAllLines(path);
                list = CustomSettings.Load<ChatCommand, CommandValueType>(
                    sa, ((a, s) => a.Command.Equals(s)), (t => new ChatCommand(t)));
            }
        }

        public void Remove(int index)
        {
            list.RemoveAt(index);
        }

        public void Save()
        {
            string path = Settings.dataDir + "ChatCommand.txt";
            var s = CustomSettings.GetSettings<ChatCommand, CommandValueType>(
                list, new Func<ChatCommand, string>(t => t.Command));
            System.IO.File.WriteAllText(path, s);
        }
    }
}