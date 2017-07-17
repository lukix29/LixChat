using LX29_ChatClient.Channels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LX29_ChatClient.Emotes
{
    public partial class EmoteCollection
    {
        public static readonly Dictionary<string, string> StandardEmotes = new Dictionary<string, string>()
        {
        {"1", ":)"},
        {"2", ":("},
        {"3", ":D"},
        {"4", ";("},
        {"5", ":z"},
        {"6", "o_o"},
        {"7", "B)"},
        {"8", ":o"},
        {"10",":/"},
        {"11",";)"},
        {"12",":p"},
        {"13",";p"},
        {"14","R)"},
        };

        public int MaxEmoteFetchCount = 6;

        public EmoteCollection()
        {
            //Task.Run(loadStdEmotes);
        }

        public delegate void ChannelLoaded(ChannelInfo ci, int count, int max, string info);

        public static event ChannelLoaded OnChannelLoaded;

        public BadgeCollection Badges
        {
            get;
            private set;
        }

        public bool Finished
        {
            get;
            private set;
        }

        public Emote Add(string id, string name, IEnumerable<string> url, string channel, EmoteOrigin origin, string channelName)
        {
            string emoteSet = "";
            var emSet = emotess.EmoteSets.FirstOrDefault(t => t.Name.Equals(channel, StringComparison.OrdinalIgnoreCase));
            if (emSet != null)
            {
                emoteSet = emSet.Set;
            }
            if (StandardEmotes.ContainsKey(id))
            {
                name = StandardEmotes[id];
            }
            Emote em = new Emote(id, name, url, channel, emoteSet, origin, channelName);
            return Add(em);
        }

        public Emote Add(Emote e)
        {
            if (e.IsEmpty) return null;

            if (!emotess.Contains(e))
            {
                lock (locjo)
                {
                    emotess.Add(e);
                }
                return e;
                //_append_to_save(e);
            }
            else
            {
                return emotess[e];
            }
        }

        public void DownloadAllEmotes()
        {
            if (!Finished) return;
            int cnt = 0;
            int length = emotess.All.Count();
            foreach (var emote in emotess.All)
            {
                emote.Image.DownloadImages();
                cnt++;
                if (OnChannelLoaded != null)
                    OnChannelLoaded(null, cnt, length, emote.Name);
            }
        }

        public void FetchEmotes(List<ChannelInfo> Channels, bool loadnew = false)
        {
            try
            {
                Finished = false;
                emotess = new Emoteionary();
                Badges = new BadgeCollection();
                loadedChannels = new HashSet<string>();

                if (!Directory.Exists(Settings.emoteDir))
                {
                    Directory.CreateDirectory(Settings.emoteDir);
                }

                _loaded_channel(null, 0, 1, "Loading Emote Infos");
                emotess.UserEmotes = LX29_Twitch.Api.TwitchApi.GetUserEmotes();
                emotess.UserEmotes.ForEach((a) =>
                {
                    string id = a.ID.ToString();
                    if (StandardEmotes.ContainsKey(id))
                    {
                        a.Name = StandardEmotes[id];
                    }
                });

                _load_EmoteSet_IDs();

                var tbadges = Task.Run(() => Badges.Fetch_Badges());

                //Color anpassbar für Form
                //Color der user besser machen
                //Optionen erweitern
                //Whisper Fenster
                //Den Chatplayer Player hier verbessern
                //Custom apiinfo panel anzeige anpassbar machen über settings
                //reconnect Timeout jedes mal erhöhen wenn es nicht ging (mit maximum 30s oder so)

                //int i = null;

                _loaded_channel(null, 0, 1, "Loading Global Emotes");
                var t2 = Task.Run(() => Badges.Parse_FFZ_Badges());
                var t0 = Task.Run(() => parse_FFZ());
                var t1 = Task.Run(() => parse_BTTV());
                var t3 = Task.Run(() => Badges.Parse_FFZ_Addon_Badges());

                var sets = new string(emotess.UserEmotes.Select(t => t.Set).Distinct().SelectMany(t => t + ",").ToArray()).Trim(',');

                parse_Twitch_Channel(sets);
                Task.WaitAll(t0, t1, t2);

                var chans = Channels.Where(s => s.IsOnline || s.AutoLoginChat || s.IsFavorited || s.SubInfo.IsSub).Select(t => t.Name).ToArray();
                _loadChannelEmotes(chans);

                if (!tbadges.IsCompleted)
                {
                    _loaded_channel(null, Channels.Count, Channels.Count, "Waiting for Badges");
                    tbadges.Wait();
                }

                Finished = true;

                _loaded_channel(null, emotess.Count, emotess.Count,
                    "Finished Loading of " + emotess.Count + " Emotes && " + Badges.Count + " Badges");
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        FetchEmotes(Channels, loadnew);
                        break;
                }
            }
        }

        public Emote GetEmote(string name, string channel, bool outgoing = false)
        {
            if (!Finished) return null;
            string id = null;
            if (outgoing)
            {
                var emi = emotess.UserEmotes.LastOrDefault(t => t.Name.Equals(name));
                if (emi != null)
                {
                    id = emi.ID;
                }
            }
            var em = emotess[id, name];
            if (em != null)
            {
                if (!(((em.Origin == EmoteOrigin.FFZ) || (em.Origin == EmoteOrigin.BTTV)) &&
                    ((!em.Channel.Contains(channel)) && (!em.Channel.ToLower().Contains("global")))))
                {
                    return em;
                }
                else
                {
                }
            }
            return null;
        }

        public IEnumerable<Emote> GetEmotes(string channel)
        {
            if (!Finished) return null;
            return emotess.GetEmotes(channel);
        }

        public void LoadChannelEmotes(params string[] channels)
        {
            if (Finished)
            {
                _loadChannelEmotes(channels);
            }
        }

        public List<ChatWord> ParseEmoteFromMessage(Dictionary<irc_params, string> parameters, string message, string channel, HashSet<MsgType> typeList)
        {
            try
            {
                if (!string.IsNullOrEmpty(message))
                {
                    Dictionary<int, ChatWord.TempWord> list = new Dictionary<int, ChatWord.TempWord>();
                    if (Finished && parameters != null && parameters.ContainsKey(irc_params.emotes))
                    {
                        string[] sem;
                        string[] sa;

                        string s = parameters[irc_params.emotes];
                        if (!string.IsNullOrEmpty(s))
                        {
                            try
                            {
                                sem = s.Split("/");

                                foreach (string em in sem)
                                {
                                    sa = em.Split(":");
                                    if (sa.Length > 1)
                                    {
                                        string id = (sa[0].Trim());
                                        var posarr = sa[1].Split(",");
                                        foreach (var pos in posarr)
                                        {
                                            var si = pos.Split("-");
                                            int i0 = int.Parse(si[0]);
                                            int i1 = int.Parse(si[1]);
                                            string nme = message.GetBetween(i0, i1).Trim();

                                            var channelName = channel;
                                            var emorig = EmoteOrigin.Twitch_Global;
                                            var setid = emotess.EmoteSets.FirstOrDefault(t => t.ID.Equals(id));
                                            if (setid != null)
                                            {
                                                channel = setid.Name;
                                                emorig = EmoteOrigin.Twitch;
                                            }
                                            else
                                            {
                                                channel = "NlC";
                                            }

                                            var urls = EmoteCollection.TWITCH_EMOTE_BASE_URL.Select(t => t.Replace("{id}", id));
                                            var addedEmote = ChatClient.Emotes.Add(id, nme, urls, channel, emorig, channelName);

                                            list.Add(i0, new ChatWord.TempWord(i0, i1, addedEmote));// nme, id, set));
                                        }
                                    }
                                }
                            }
                            catch { }
                        }
                    }

                    var ChatWords = new List<ChatWord>();
                    int index = 0;
                    string word = "";
                    while (true)
                    {
                        char c = message[index];
                        word += c;
                        if (char.IsWhiteSpace(c) || index == message.Length - 1)
                        {
                            if (word.Length > 0)
                            {
                                if (list.Count > 0 && list.ContainsKey(index - (word.Length - 1)))
                                {
                                    var item = list[index - (word.Length - 1)];
                                    ChatWords.Add(new ChatWord(item));
                                    index = item.End;
                                }
                                else
                                {
                                    ChatWords.Add(new ChatWord(word.Trim(), channel, typeList.Contains(MsgType.Outgoing)));
                                }
                                word = string.Empty;
                            }
                        }
                        index++;
                        if (index == message.Length) break;
                    }
                    return ChatWords;
                }
            }
            catch
            {
            }
            return new List<ChatWord>();
        }

        private void _loadChannelEmotes(params string[] channels)
        {
            try
            {
                int cnt = 0;
                List<Task<string>> tasks = new List<Task<string>>();
                foreach (var channel in channels)
                {
                    if (loadedChannels.Contains(channel))
                        continue;

                    _loaded_channel(null, cnt, channels.Count(), "Loading BTTV/FFZ Emotes && Channel Badges");

                    tasks.Add(Task.Run<string>(() =>
                    {
                        var channelinfo = ChatClient.Channels[channel];
                        Badges.Fetch_Sub_Badges(channelinfo);
                        parse_BTTV_Channel(channel);
                        parse_FFZ_Channel(channel);
                        return channel;
                    }));
                    _loaded_channel(null, cnt, channels.Count(), "Loading BTTV/FFZ Emotes && Channel Badges");
                    if (tasks.Count >= MaxEmoteFetchCount)
                    {
                        var index = Task.WaitAny(tasks.ToArray());
                        var result = tasks[index].Result;
                        tasks.RemoveAt(index);
                        loadedChannels.Add(result);
                        cnt++;
                        _loaded_channel(null, cnt, channels.Count(), "Loading BTTV/FFZ Emotes && Channel Badges");
                    }
                }
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        loadedChannels.RemoveWhere(t => channels.Any(n => n.Equals(t, StringComparison.OrdinalIgnoreCase)));
                        _loadChannelEmotes(channels);
                        break;
                }
            }
        }
    }
}