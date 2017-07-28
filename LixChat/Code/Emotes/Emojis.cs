using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LX29_ChatClient.Channels;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace LX29_ChatClient.Emotes.Emojis
{
    public class Emoji
    {
        public string ID;
        public Image Image;
        public string Name;

        public Emoji(string Name, string ID)
        { }
    }

    public class Emojis
    {
        //private Dictionary<string, EmoteImage> emojis = new Dictionary<string, EmoteImage>();

        //public void Load()
        //{
        //    string line;
        //    using (StringReader sr = new StringReader(LX29_LixChat.Properties.Resources.emojis))
        //    {
        //        while ((line = sr.ReadLine()) != null)
        //        {
        //            var arr = line.Split('=');
        //            emojis.Add(arr[0],new EmoteImage())
        //        }
        //    }
        //}
    }
}