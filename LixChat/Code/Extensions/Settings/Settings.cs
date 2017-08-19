using LX29_Twitch.Forms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace LX29_ChatClient
{
    public class CustomSettings<Tclass, Tenum>
    {
        public bool Load(Dictionary<string, string> lines, Tclass Class)
        {
            if (lines.Count > 0)
            {
                try
                {
                    foreach (var li in lines)
                    {
                        var _Value = li.Value;
                        var key = li.Key;
                        var typ = Class.GetType();
                        var prop = typ.GetProperty(key);
                        if (prop.CanWrite)
                        {
                            object val = _Value;
                            if (prop.PropertyType.BaseType.IsEquivalentTo(typeof(Enum)))
                            {
                                val = Enum.Parse(prop.PropertyType, _Value);
                            }
                            else if (prop.PropertyType.IsEquivalentTo(typeof(Rectangle)))
                            {
                                var rec = _Value.ParseRectangleScreenSafe();
                                val = rec;
                            }
                            else
                            {
                                val = Convert.ChangeType(_Value, prop.PropertyType);
                            }
                            prop.SetValue(Class, val);
                        }
                    }
                    return true;
                }
                catch
                {
                }
            }
            return false;
        }

        public string Save(Tclass Class)
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.TypeNameHandling = TypeNameHandling.Auto;
            serializer.DefaultValueHandling = DefaultValueHandling.Ignore;
            serializer.MissingMemberHandling = MissingMemberHandling.Ignore;
            serializer.ObjectCreationHandling = ObjectCreationHandling.Auto;
            var names = Enum.GetNames(typeof(Tenum));
            StringWriter sw = new StringWriter();
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                try
                {
                    var typ = Class.GetType();
                    writer.WriteStartObject();
                    foreach (var name in names)
                    {
                        var prop = typ.GetProperty(name);
                        writer.WritePropertyName(name);
                        writer.WriteValue(prop.GetValue(Class).ToString());
                    }
                    writer.WriteEndObject();
                }
                catch
                {
                }
            }
            string vari = sw.ToString();

            return vari;
        }
    }

    public class CustomSettings
    {
        public static List<Dictionary<string, string>> LoadList(string input)
        {
            try
            {
                var stuff = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(input);
                return stuff;
            }
            catch { }
            return null;
        }

        public static string SaveList(IEnumerable<string> list)
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.TypeNameHandling = TypeNameHandling.Auto;
            serializer.DefaultValueHandling = DefaultValueHandling.Ignore;
            serializer.MissingMemberHandling = MissingMemberHandling.Ignore;
            serializer.ObjectCreationHandling = ObjectCreationHandling.Auto;

            StringWriter sw = new StringWriter();
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.WriteStartArray();
                foreach (var item in list)
                {
                    writer.WriteRawValue(item);
                }
                writer.WriteEndArray();
            }
            var st = sw.ToString();
            return st;
        }
    }

    public class SettingClasses
    {
        public static readonly SettingClasses[] ChatBasic = new SettingClasses[]
        {
            new SettingClasses("_ChatHistory", "Chat History Amount", 100.0, Int16.MaxValue * 1.0, 1.0),
            new SettingClasses("_ShowTimeoutMessages", "Show Timeouts/Bans"),
            new SettingClasses("_ShowTimeStamp", "Show Time Stamp")
        };

        public static readonly SettingClasses[] EmoteBasic = new SettingClasses[]
        {
            new SettingClasses("_BadgePadding", "Badge Padding", 1.0, 100.0, 1.0),
            new SettingClasses("_BadgeSizeFac", "Badge Size", 0.1, 10, 0.1),
            new SettingClasses("_EmotePadding", "Emote Padding", 1.0, 100.0, 1.0),
            new SettingClasses("_EmoteSizeFac", "Emote Size", 0.1, 10, 0.1),
            new SettingClasses("_EmoteSize", "Emote Quality", 1.0, 3.0, 1.0),
            new SettingClasses("_HwEmoteDrawing", "Hardware Accel"),
            new SettingClasses("_AnimatedEmotes", "Animated Gif Emotes")
       };

        public static readonly SettingClasses[] PlayerBasic = new SettingClasses[]
        {
            new SettingClasses("_MpvBufferBytes", "Player Buffer(Kbytes)", 100.0, UInt16.MaxValue * 10.0, 1000.0),
            new SettingClasses("_MpvBufferSeconds", "Player Buffer(Sec)", 1.0, 120.0, 1.0)
        };

        public static readonly SettingClasses[] TextBasic = new SettingClasses[]
        {
            new SettingClasses("_ChatFontSize", "Chat Font Size", 4.0, 32.0, 0.1),
            new SettingClasses("_LinePadding", "Line Padding", 1.0, 100.0, 0.1),
            new SettingClasses("_LineSpacing", "Line Spacing", 1.0, 100.0, 0.1),
            new SettingClasses("_WordPadding", "Word Padding", 1.0, 100.0, 0.1),
            new SettingClasses("_TimePadding","Time Padding", 1.0, 100.0, 0.1)
        };

        public static readonly SettingClasses[] UserBasic = new SettingClasses[]
        {
            new SettingClasses("_UserColorBrigthness", "User Brigthness", 0.0, 2.0, 0.01),
            new SettingClasses("_UserColorSaturation", "User Saturation", 0.0, 2.0, 0.01)
        };

        public readonly int DecimalPlaces;
        public readonly double Inc;
        public readonly double Max;
        public readonly double Min;
        public readonly string Name;
        public readonly string Text;
        public readonly object Value;

        public SettingClasses(string name, string text)
            : this(name, text, 0, 0, 0)
        {
        }

        public SettingClasses(string name, string text, double min, double max, double inc)
        {
            Name = name;
            Text = text;
            Min = min;
            Max = max;
            Inc = inc;
            double etmp = (inc - (int)inc);
            if (etmp > 0)
            {
                DecimalPlaces = (inc - (int)inc).ToString().Length - 2;
            }
            else
            {
                DecimalPlaces = 0;
            }
        }

        public SettingClasses(SettingClasses classe, object value)
        {
            Name = classe.Name;
            Text = classe.Text;
            Min = classe.Min;
            Max = classe.Max;
            Inc = classe.Inc;
            DecimalPlaces = classe.DecimalPlaces;
            Value = value;
        }
    }

    public class Settings
    {
        public static readonly string caonfigBaseDir =
            Application.StartupPath.Contains("Debug") ?
            Path.GetFullPath(".\\") :
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\LixChat\\";

        public static readonly string chatLogDir = caonfigBaseDir + "Chatlogs\\";

        public static readonly string dataDir = caonfigBaseDir + "Data\\";

        public static readonly string emojiDir = ".\\Emojis\\";

        public static readonly string emoteDir = caonfigBaseDir + "Emotes\\";

        public static readonly string pluginDir = ".\\Plugins\\";

        public static readonly string scriptDir = caonfigBaseDir + "Scripts\\";

        private static readonly string settings_ini_path = caonfigBaseDir + "settings.ini";

        public static Dictionary<string, object> Standard
        {
            get;
            private set;
        }

        #region EmoteBadge

        private static bool _AnimatedEmotes = true;
        private static bool _AnimateGifInSearch = true;
        private static double _BadgePadding = 2;
        private static double _BadgeSizeFac = 0.9;
        private static double _EmotePadding = 2;
        private static double _EmoteSize = (int)Emotes.EmoteImageSize.Medium;
        private static double _EmoteSizeFac = 1.5;
        private static bool _HwEmoteDrawing = false;
        private static bool _ShowTimeStamp = true;

        public static bool AnimatedEmotes
        {
            get { return _AnimatedEmotes; }
            set
            {
                _AnimatedEmotes = value;
                Save();
            }
        }

        public static bool AnimateGifInSearch
        {
            get { return _AnimateGifInSearch; }
            set
            {
                _AnimateGifInSearch = value;
                Save();
            }
        }

        public static double BadgePadding
        {
            get { return _BadgePadding; }
            set
            {
                _BadgePadding = value;
                Save();
            }
        }

        public static double BadgeSizeFac
        {
            get { return _BadgeSizeFac; }
            set
            {
                _BadgeSizeFac = value;
                Save();
            }
        }

        public static double EmotePadding
        {
            get { return _EmotePadding; }
            set
            {
                _EmotePadding = value;
                Save();
            }
        }

        public static Emotes.EmoteImageSize EmoteQuality
        {
            get { return (Emotes.EmoteImageSize)(int)_EmoteSize; }
            set
            {
                _EmoteSize = (int)value;
                Save();
            }
        }

        public static double EmoteSizeFac
        {
            get { return _EmoteSizeFac; }
            set
            {
                _EmoteSizeFac = value;
                Save();
            }
        }

        public static bool HwEmoteDrawing
        {
            get { return _HwEmoteDrawing; }
            set
            {
                _HwEmoteDrawing = value;
                Save();
            }
        }

        public static bool ShowTimeStamp
        {
            get { return _ShowTimeStamp; }
            set
            {
                _ShowTimeStamp = value;
                Save();
            }
        }

        #endregion EmoteBadge

        #region Text

        private static double _ChatFontSize = 12.0;
        private static double _LinePadding = 5;
        private static double _LineSpacing = 5;
        private static double _TimePadding = 1;
        private static double _UserColorBrigthness = 0.05;
        private static double _UserColorSaturation = 1.0;
        private static double _WordPadding = 3;

        public static string ChatFontName
        {
            get { return _ChatFontName; }
            set
            {
                if (System.Drawing.FontFamily.Families
                    .Any(t => t.Name.Equals(value, StringComparison.OrdinalIgnoreCase)))
                {
                    _ChatFontName = value;
                    Save();
                }
            }
        }

        public static double ChatFontSize
        {
            get { return _ChatFontSize; }
            set
            {
                _ChatFontSize = Math.Max(3, Math.Min(72,
                    Math.Round(value, 1, MidpointRounding.AwayFromZero)));
                Save();
            }
        }

        public static double LinePadding
        {
            get { return _LinePadding; }
            set
            {
                _LinePadding = value;
                Save();
            }
        }

        public static double LineSpacing
        {
            get { return _LineSpacing; }
            set
            {
                _LineSpacing = value;
                Save();
            }
        }

        public static double TimePadding
        {
            get { return _TimePadding; }
            set
            {
                _TimePadding = value;
                Save();
            }
        }

        public static double WordPadding
        {
            get { return _WordPadding; }
            set
            {
                _WordPadding = value;
                Save();
            }
        }

        #endregion Text

        #region PrivateFields

        private static string _BrowserName = "";
        private static string _BrowserPath = "";
        private static int _ChatBackGround = Color.FromArgb(35, 35, 35).ToArgb();
        private static string _ChatFontName = "Calibri";
        private static Rectangle _MainBounds = Rectangle.Empty;
        private static double _MpvBufferBytes = 64000;
        private static double _MpvBufferSeconds = 10;
        private static bool _ShowErrors = false;
        private static bool _ShowTimeoutMessages = true;
        private static double _UpdateInterval = 60000;
        private int _ChatHistory = 1024;

        #endregion PrivateFields

        #region PublicProperties

        public static string BrowserName
        {
            get
            {
                if (_BrowserPath.IsEmpty())
                {
                    _BrowserPath = LX29_Tools.GetSystemDefaultBrowser();
                    _BrowserName = Path.GetFileNameWithoutExtension(_BrowserPath);
                    Save();
                }
                return _BrowserName.ToLower().Replace("google", "").Trim();
            }
            set
            {
                if (File.Exists(value))
                {
                    _BrowserName = value;
                    Save();
                }
            }
        }

        public static string BrowserPath
        {
            get
            {
                if (_BrowserPath.IsEmpty())
                {
                    _BrowserPath = LX29_Tools.GetSystemDefaultBrowser();
                    _BrowserName = Path.GetFileNameWithoutExtension(_BrowserPath).ToLower();
                    Save();
                }
                return _BrowserPath;
            }
            set
            {
                if (File.Exists(value))
                {
                    _BrowserPath = value;
                    Save();
                }
            }
        }

        public static int ChatBackGround
        {
            get { return _ChatBackGround; }
            set
            {
                _ChatBackGround = value;
                Save();
            }
        }

        public static Rectangle MainBounds
        {
            get { return _MainBounds; }
            set
            {
                _MainBounds = value;
                Save();
            }
        }

        public static Point MainLocation
        {
            get { return MainBounds.Location; }
            set { MainBounds = new Rectangle(value, _MainBounds.Size); }
        }

        public static Size MainSize
        {
            get { return MainBounds.Size; }
            set { MainBounds = new Rectangle(_MainBounds.Location, value); }
        }

        public static double MpvBufferBytes
        {
            get { return _MpvBufferBytes; }
            set
            {
                _MpvBufferBytes = value;
                Save();
            }
        }

        public static double MpvBufferSeconds
        {
            get { return _MpvBufferSeconds; }
            set
            {
                _MpvBufferSeconds = value;
                Save();
            }
        }

        public static bool ShowErrors
        {
            get { return _ShowErrors; }
            set
            {
                _ShowErrors = value;
                Save();
            }
        }

        public static bool ShowTimeoutMessages
        {
            get { return _ShowTimeoutMessages; }
            set
            {
                _ShowTimeoutMessages = value;
                Save();
            }
        }

        public static double UpdateInterval
        {
            get { return _UpdateInterval; }
            set
            {
                _UpdateInterval = Math.Max(30000, value);
                Save();
            }
        }

        public static double UserColorBrigthness
        {
            get { return _UserColorBrigthness; }
            set
            {
                _UserColorBrigthness = Math.Max(0, Math.Min(1, value));
                Save();
            }
        }

        public static double UserColorSaturation
        {
            get { return _UserColorSaturation; }
            set
            {
                _UserColorSaturation = Math.Max(0, Math.Min(1, value));
                Save();
            }
        }

        public int ChatHistory
        {
            get { return _ChatHistory; }
            set
            {
                _ChatHistory = value;
                Save();
            }
        }

        #endregion PublicProperties

        public static DialogResult GetBrowserPath(bool selectNew = true)
        {
            FormBrowserSelector form = new FormBrowserSelector();
            if (selectNew || _BrowserPath.IsEmpty())
            {
                if (selectNew)
                {
                    form.Title = "Select Standard Browser";
                    form.ButtonSelect.Text = "Select Browser";
                    form.ButtonCopy.Visible = false;
                }
                form.ShowDialog();
                if (form.Abort != DialogResult.Abort)
                {
                    BrowserPath = form.BrowserPath;
                    _BrowserName = form.BrowserName;
                }
            }
            return form.Abort;
        }

        public static List<SettingClasses> GetFields(SettingClasses[] keys)
        {
            var sa = typeof(Settings).GetFields(
                   System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                   .Where(t => keys.Any(t0 => t.Name.Equals(t0.Name)));
            var list = sa.Select(t => new SettingClasses(keys.First(t0 => t0.Name.Equals(t.Name)), t.GetValue(null))).ToList();
            return list;
        }

        public static bool Load()
        {
            try
            {
                Standard = typeof(Settings).GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                    .Where(t => t.Name.StartsWith("_")).ToDictionary(t => t.Name, t0 => t0.GetValue(null));

                Directory.CreateDirectory(chatLogDir);
            }
            catch { }
            try { Directory.CreateDirectory(dataDir); }
            catch { }
            try { Directory.CreateDirectory(emoteDir); }
            catch { }
            try { Directory.CreateDirectory(scriptDir); }
            catch { }
            try
            {
                if (!File.Exists(scriptDir + "TestScript.cs"))
                {
                    //string old = LX29_TwitchChat.Properties.Resources.MyScripts;
                    //string nwe = File.ReadAllText(scriptDir + "TestScript.cs");
                    //if (old.Equals(nwe))
                    //{
                    File.WriteAllText(scriptDir + "TestScript.cs", LX29_LixChat.Properties.Resources.MyScripts);
                    // }
                }
            }
            catch { }
            if (File.Exists(settings_ini_path))
            {
                var lines = File.ReadAllLines(settings_ini_path);
                if (lines.Length > 0)
                {
                    try
                    {
                        foreach (var line in lines)
                        {
                            var _Value = line.GetBetween("[", "]").Trim();
                            var key = line.GetBetween("", "[").Trim();
                            var typ = typeof(Settings);

                            var field = typ.GetField(key, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

                            object val = _Value;
                            if (field.FieldType.BaseType.IsEquivalentTo(typeof(Enum)))
                            {
                                val = Enum.Parse(field.FieldType, _Value);
                            }
                            else if (field.FieldType.IsEquivalentTo(typeof(Rectangle)))
                            {
                                var rec = _Value.ParseRectangleScreenSafe();
                                val = rec;
                            }
                            else
                            {
                                val = Convert.ChangeType(_Value, field.FieldType);
                            }
                            field.SetValue(null, val);

                            //prop.SetValue(null, value);
                        }
                        return true;
                    }
                    catch
                    {
                    }
                }
            }
            Save();
            return false;
        }

        public static void Save()
        {
            using (StreamWriter sw = new StreamWriter(File.Create(settings_ini_path)))
            {
                var sa = typeof(Settings).GetFields(
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

                foreach (var s in sa)
                {
                    if (s.Name.StartsWith("_"))
                    {
                        if (null != s)
                        {
                            var value = s.GetValue(null);
                            sw.WriteLine(s.Name + "\t[" + value + "]");
                        }
                    }
                }
            }
        }

        public static void SetValue(string Name, object value)
        {
            var sa = typeof(Settings).GetFields(
                  System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                  .FirstOrDefault(t => t.Name.Equals(Name));
            if (sa != null)
            {
                sa.SetValue(null, value);
            }
        }

        public static void SetValue(SettingClasses variable, object value)
        {
            var sa = typeof(Settings).GetFields(
                  System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                  .FirstOrDefault(t => t.Name.Equals(variable.Name));
            if (sa != null)
            {
                sa.SetValue(null, value);
            }
        }

        public static Process StartBrowser(string url)
        {
            //GetBrowserPath(false);
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.WorkingDirectory = Path.GetDirectoryName(BrowserPath);
            p.StartInfo.FileName = BrowserPath;
            p.StartInfo.Arguments = url;
            p.Start();
            return p;
        }
    }
}