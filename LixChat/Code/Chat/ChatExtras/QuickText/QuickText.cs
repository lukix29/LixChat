using System;
using System.Collections.Generic;

namespace LX29_ChatClient.Addons
{
    public enum QuickTextType
    {
        Channel,
        Message,
    }

    public class QuickText : CustomSettings<QuickText, QuickTextType>, IEquatable<QuickText>
    {
        public QuickText(string channel)
        {
            Channel = channel;
        }

        public QuickText(string channel, string message)
        {
            Channel = channel;
            Message = message;
        }

        public QuickText(string[] input)
        {
            //if (!Load(input, this))
            //{
            //}
        }

        public string Channel
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }

        public bool Equals(QuickText obj)
        {
            return obj.ToString().Equals(ToString());
        }

        public string Save()
        {
            return base.Save(this);
        }

        public override string ToString()
        {
            return "MESSAGE=" + Message;
        }
    }

    public class QuickTextCollection : IEnumerable<QuickText>
    {
        private List<QuickText> list = new List<QuickText>();

        public QuickTextCollection()
        {
        }

        public QuickText this[int index]
        {
            get { return list[index]; }
            set { list.Insert(index, value); }
        }

        public void Add(QuickText item)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
            }
        }

        public IEnumerator<QuickText> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Load()
        {
            string path = Settings.dataDir + "QuickText.txt";
            if (System.IO.File.Exists(path))
            {
                //var sa = System.IO.File.ReadAllLines(path);
                //list = CustomSettings.Load<QuickText, QuickTextType>(
                //    sa, ((a, s) => a.Channel.Equals(s)), (t => new QuickText(t)));
            }
        }

        public void Remove(int index)
        {
            list.RemoveAt(index);
        }

        public void Save()
        {
            //string path = Settings.dataDir + "QuickText.txt";
            //var s = CustomSettings.GetSettings<QuickText, QuickTextType>(
            //    list, new Func<QuickText, string>(t => t.Channel));
            //System.IO.File.WriteAllText(path, s);
        }
    }
}