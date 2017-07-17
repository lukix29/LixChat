using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace LX29_SettingsIO
{
    public enum SettingsNames : int
    {
        Last_Update_Time = 0,
        Window_Location,
        MPV_Buffer_Kb,
        Sort_By_Online,
        Sort_By_Viewing,

        //Sync_Follow,
        Word_Wrap,

        Manager_Update_Interval,
        Preview_Borderless,
        Preview_On_Top,
        Preview_Location,
        Preview_Size,
        Show_Errors,
        MPV_Buffer_Seconds,
        Chat_Highlight_Words,
        Stream_Update_Interval,
        Max_Api_Calls,
        Threaded_Api_Calls,
    }

    public static class LX29Settings
    {
        //public struct ObjType
        //{
        //    public object Object;
        //    public Type Type;
        //    public ObjType(object O)
        //    {
        //        Object = O;
        //        Type = O.GetType();
        //    }
        //    public ObjType(object O, Type t)
        //    {
        //        Object = O;
        //        Type = t;
        //    }
        //    //public static explicit operator Point(ObjType o)
        //    //{
        //    //    return ParsePoint(o.Object.ToString());
        //    //}
        //    //public static explicit operator Size(ObjType o)
        //    //{
        //    //    return ParseSize(o.Object.ToString());
        //    //}
        //}
        private const char SplitChar = '\t';

        private static string FILENAME = "settings.ini";

        private static Dictionary<SettingsNames, object> values = new Dictionary<SettingsNames, object>();

        public delegate void SettingsChangedEvent();

        public static event SettingsChangedEvent SettingsChanged;

        private static object[] StandardSettings
        {
            get
            {
                object[] oa = new object[Enum.GetNames(typeof(SettingsNames)).Length];
                oa[(int)SettingsNames.Last_Update_Time] = DateTime.MinValue;
                oa[(int)SettingsNames.MPV_Buffer_Kb] = 128000;
                oa[(int)SettingsNames.MPV_Buffer_Seconds] = 10;
                oa[(int)SettingsNames.Window_Location] = new Point(10, 10);
                oa[(int)SettingsNames.Sort_By_Online] = true;
                oa[(int)SettingsNames.Sort_By_Viewing] = false;
                //oa[(int)SettingsNames.Sync_Follow] = false;
                oa[(int)SettingsNames.Word_Wrap] = true;
                oa[(int)SettingsNames.Manager_Update_Interval] = 3;
                oa[(int)SettingsNames.Preview_Borderless] = false;
                oa[(int)SettingsNames.Preview_On_Top] = false;
                oa[(int)SettingsNames.Preview_Location] = new Point(50, 50);
                oa[(int)SettingsNames.Preview_Size] = new Size(800, 470);
                oa[(int)SettingsNames.Show_Errors] = (Environment.UserName.ToLower().Contains("lukix"));
                oa[(int)SettingsNames.Chat_Highlight_Words] = " ";
                oa[(int)SettingsNames.Stream_Update_Interval] = 1;
                oa[(int)SettingsNames.Max_Api_Calls] = 5;
                oa[(int)SettingsNames.Threaded_Api_Calls] = true;
                return oa;
            }
        }

        public static void ClearSettings(bool save)
        {
            try
            {
                values = new Dictionary<SettingsNames, object>();
                foreach (SettingsNames name in Enum.GetValues(typeof(SettingsNames)))
                {
                    values.Add(name, StandardSettings[(int)name]);
                }
                if (save) SaveSettings(false);
            }
            catch (Exception x)
            {
                x.Handle();
            }
        }

        public static T ConvertValue<T>(object value)
        {
            try
            {
                //if (value is Point)
                //{
                //    value = ParsePoint(value.ToString());
                //}
                //else if (value is Size)
                //{
                //    value = ParseSize(value.ToString());
                //}
                //else if (value is Rectangle)
                //{
                //    value = ParseRectangle(value.ToString());
                //}
                if (value is T)
                {
                    return (T)value;
                }
                else
                {
                    try
                    {
                        return (T)Convert.ChangeType(value, typeof(T));
                    }
                    catch (InvalidCastException)
                    {
                        return default(T);
                    }
                }
            }
            catch
            {
                return default(T);
            }
        }

        public static Control[] GetControls(Rectangle form, Graphics g, out int height)
        {
            try
            {
                Size size = form.Size;
                int lastY = 10;
                List<Control> list = new List<Control>();
                Font font = new Font("Microsoft Sans Serif", 12f);

                string[] sa = Enum.GetNames(typeof(SettingsNames));
                Array.Sort(sa);
                Dictionary<SettingsNames, object> dic = new Dictionary<SettingsNames, object>();
                foreach (string s in sa)
                {
                    SettingsNames sn = (SettingsNames)Enum.Parse(typeof(SettingsNames), s);
                    dic.Add(sn, values[sn]);
                }

                foreach (KeyValuePair<SettingsNames, object> kvp in dic)
                {
                    string name = Enum.GetName(typeof(SettingsNames), kvp.Key).Replace("_", " ");

                    TextBox l = new TextBox();
                    l.Location = new Point(10, lastY);
                    l.Font = font;
                    l.ForeColor = Color.White;
                    l.BackColor = Color.Black;
                    l.BorderStyle = BorderStyle.None;
                    l.Size = g.MeasureString(name, l.Font).ToSize();
                    Point siz0 = new Point(l.Size.Width + 30, 0);
                    l.Text = name;
                    l.ReadOnly = true;
                    list.Add(l);
                    Size siz = new Size((size.Width - l.Width) - 30, l.Height);
                    if (kvp.Value.GetType() == typeof(bool))
                    {
                        bool bvalue = (bool)kvp.Value;
                        CheckBox t = new CheckBox();
                        t.Name = name;
                        t.Location = new Point(siz0.X, lastY);
                        t.Checked = bvalue;

                        t.CheckedChanged += delegate(object o, EventArgs ei)
                        {
                            CheckBox cb = (CheckBox)o;
                            SettingsNames n = kvp.Key;
                            SetValue(n, cb.Checked);

                            if (SettingsChanged != null)
                                SettingsChanged();
                        };
                        list.Add(t);
                    }
                    else if (kvp.Value.GetType() == typeof(DateTime))
                    {
                        TextBox t = new TextBox();
                        t.ForeColor = Color.White;
                        t.BackColor = Color.Black;
                        t.BorderStyle = BorderStyle.FixedSingle;
                        t.Text = kvp.Value.ToString();
                        t.Location = new Point(siz0.X, lastY);
                        t.Size = siz;

                        t.Leave += delegate(object o, EventArgs e)
                        {
                            DateTime dt = DateTime.MinValue;
                            if (DateTime.TryParse(t.Text, out dt))
                            {
                                SetValue(kvp.Key, dt);

                                if (SettingsChanged != null)
                                    SettingsChanged();
                            }
                            else
                            {
                                t.Text = kvp.Value.ToString();
                                MessageBox.Show("Not a DateTime!");
                            }
                        };

                        list.Add(t);
                    }
                    else if (kvp.Value.GetType() == typeof(Point) || kvp.Value.GetType() == typeof(Size))
                    {
                        IntXY_Ctrl nudX = new IntXY_Ctrl();
                        nudX.BorderStyle = BorderStyle.None;

                        nudX.Value = kvp.Value;

                        nudX.Location = new Point(0, 0);
                        Panel p = new Panel();
                        p.Controls.Add(nudX);
                        p.BorderStyle = BorderStyle.None;
                        p.Size = nudX.Size = siz;
                        p.Location = new Point(siz0.X, lastY);

                        nudX.ValueChanged += delegate(object o, EventArgs e)
                        {
                            SetValue(kvp.Key, nudX.Value);

                            if (SettingsChanged != null)
                                SettingsChanged();
                        };

                        list.Add(p);
                    }
                    else if (kvp.Value.GetType() == typeof(int))
                    {
                        NumericUpDown t = new NumericUpDown();
                        t.ForeColor = Color.White;
                        t.BackColor = Color.Black;
                        t.Maximum = Int32.MaxValue;
                        t.BorderStyle = BorderStyle.FixedSingle;
                        t.Value = (int)kvp.Value;
                        t.Location = new Point(siz0.X, lastY);
                        t.Size = siz;

                        t.ValueChanged += delegate(object o, EventArgs e)
                        {
                            SetValue(kvp.Key, t.Value);

                            if (SettingsChanged != null)
                                SettingsChanged();
                        };

                        list.Add(t);
                    }
                    else
                    {
                        TextBox t = new TextBox();
                        t.ForeColor = Color.White;
                        t.BackColor = Color.Black;
                        t.BorderStyle = BorderStyle.FixedSingle;
                        t.Text = kvp.Value.ToString();
                        t.Location = new Point(siz0.X, lastY);
                        t.Size = siz;

                        t.Leave += delegate(object o, EventArgs e)
                        {
                            SetValue(kvp.Key, t.Text);

                            if (SettingsChanged != null)
                                SettingsChanged();
                        };

                        list.Add(t);
                    }

                    lastY += l.Height + 20;
                }
                height = lastY;
                return list.ToArray();
            }
            catch (Exception x)
            {
                x.Handle();
            }
            height = 0;
            return null;
        }

        public static T GetValue<T>(SettingsNames type)
        {
            if (values.ContainsKey(type))
            {
                var v = ConvertValue<T>(values[type]);
                return v;
            }
            return (T)StandardSettings[(int)type];
        }

        public static void LoadSettings()
        {
            try
            {
                ClearSettings(false);
                if (File.Exists(FILENAME))
                {
                    string[] sinput = File.ReadAllLines(FILENAME);

                    foreach (string sii in sinput)
                    {
                        string[] sa = sii.Split(new char[] { SplitChar }, StringSplitOptions.RemoveEmptyEntries);
                        SettingsNames name = (SettingsNames)Enum.Parse(typeof(SettingsNames), sa[0], true);

                        if (sa[2].Contains("Point"))
                        {
                            //Type t = typeof(Point);
                            object v = ParsePoint(sa[1]);
                            values[name] = v;
                        }
                        else if (sa[2].Contains("Size"))
                        {
                            //Type t = typeof(Size);
                            object v = ParseSize(sa[1]);
                            values[name] = v;
                        }
                        else
                        {
                            Type t = Type.GetType(sa[2], true, true);
                            object v = Convert.ChangeType(sa[1], t);
                            values[name] = v;
                        }
                    }

                    if (SettingsChanged != null)
                        SettingsChanged();
                }
            }
            catch (Exception x)
            {
                x.Handle();
                ClearSettings(true);
            }
        }

        public static Point ParsePoint(string s)
        {
            int x = 0;
            int y = 0;
            string[] sa = s.Split(new char[] { '=', ',', '}' }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                Int32.TryParse(sa[1], out x);
                Int32.TryParse(sa[3], out y);
            }
            catch { return Point.Empty; }
            return new Point(x, y);
        }

        public static Size ParseSize(string s)
        {
            int w = 0;
            int h = 0;
            string[] sa = s.Split(new char[] { '=', ',', '}' }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                Int32.TryParse(sa[1], out w);
                Int32.TryParse(sa[3], out h);
            }
            catch { return Size.Empty; }
            return new Size(w, h);
        }

        public static void SaveSettings(bool fireChanged)
        {
            StreamWriter sw = new StreamWriter(File.Create(FILENAME));

            foreach (SettingsNames name in Enum.GetValues(typeof(SettingsNames)))
            {
                string s = Enum.GetName(typeof(SettingsNames), name);
                Type t = values[name].GetType();
                sw.WriteLine(s + SplitChar + SplitChar + values[name] + SplitChar + SplitChar + t.ToString());
            }
            sw.Close();
            if (fireChanged)
            {
                if (SettingsChanged != null)
                    SettingsChanged();
            }
        }

        public static void SetValue(SettingsNames type, object value)
        {
            values[type] = value;
            SaveSettings(false);
        }
    }
}