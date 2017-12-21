using Newtonsoft.Json;
using System;
using System.Collections.Generic;

//using System.Collections.Generic;

//using System.IO;

//using System.Linq;

namespace LX29_ChatClient.Addons
{
    public class QuickTextCollection
    {
        public QuickTextCollection()
        {
            Values = new Dictionary<string, string>();
        }

        [JsonRequired]
        public Dictionary<string, string> Values
        {
            get;
            set;
        }

        public string this[string key]
        {
            get { return Values[key]; }
            //set { list.Insert(index, value); }
        }

        public static QuickTextCollection Load()
        {
            try
            {
                string path = Settings._dataDir + "QuickText.json";
                if (System.IO.File.Exists(path))
                {
                    return JsonConvert.DeserializeObject<QuickTextCollection>(System.IO.File.ReadAllText(path));
                }
            }
            catch { }
            return new QuickTextCollection();
        }

        public void Add(string key, string item)
        {
            if (!Values.ContainsKey(key))
            {
                Values.Add(key, item);
                Save();
            }
        }

        public string CheckMessage(string input)
        {
            var words = input.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in words)
            {
                if (Values.ContainsKey(word))
                {
                    return Values[word];
                }
            }
            return input;
        }

        public void Remove(string key)
        {
            Values.Remove(key);
            Save();
        }

        public void Save()
        {
            string path = Settings._dataDir + "QuickText.json";
            var json = JsonConvert.SerializeObject(this);
            System.IO.File.WriteAllText(path, json);
        }
    }
}