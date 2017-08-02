#define Drawing
#define Interop

using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

using System.Net;

#if Interop

using System.Reflection;
using System.Runtime.InteropServices;

#endif

#if Drawing

using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;

#endif

namespace System
{
    public static class LXMath
    {
        public static decimal Map(decimal x, decimal in_min, decimal in_max, decimal out_min, decimal out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        public static long Map(long x, long in_min, long in_max, long out_min, long out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        public static int Map(int x, int in_min, int in_max, int out_min, int out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        public static double Map(double x, double in_min, double in_max, double out_min, double out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        public static float Map(float x, float in_min, float in_max, float out_min, float out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        private static readonly string[] SizeSuffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        public static string SizeSuffix(this long value, int decimalPlaces = 3)
        {
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return "0.0 bytes"; }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag)
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }

        public static string SizeSuffix(this int value, int decimalPlaces = 3)
        {
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return "0.0 bytes"; }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag)
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }

        public static string SizeSuffix(this float value, int decimalPlaces = 3)
        {
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return "0.0 bytes"; }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag)
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }
    }

    public static class Extensions
    {
        public const int BufferSize = 1024;

#if Drawing
        public static readonly Size maxSize = new Size(int.MaxValue, int.MaxValue);
#endif

        private static int errorCount = 0;

        private static object errorLock = new object();

        public static bool ContainsAny(this string s, params char[] items)
        {
            foreach (char si in items)
            {
                if (s.ToLower().Contains(si))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ContainsAny(this string s, params string[] items)
        {
            return s.ContainsAny(true, items);
        }

        public static bool ContainsAny(this string s, bool toLower, params string[] items)
        {
            string sinput = (toLower) ? s.ToLower() : s;
            foreach (string si in items)
            {
                string sii = (toLower) ? si.ToLower() : si;

                if (sinput.Contains(sii))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ContainsAny(this string s, out string find, bool toLower, params string[] items)
        {
            string sinput = (toLower) ? s.ToLower() : s;
            foreach (string si in items)
            {
                string sii = (toLower) ? si.ToLower() : si;

                if (sinput.Contains(si))
                {
                    find = si;
                    return true;
                }
            }
            find = "";
            return false;
        }

        public static int CountStrings(this string input, string stringToFind)
        {
            int pos = 0;
            int cnt = 0;
            while ((pos < input.Length) && (pos = input.IndexOf(stringToFind, pos)) != -1)
            {
                cnt++;
                pos += stringToFind.Length;
            }
            return cnt;
        }

        public static bool EqualsAny(this string sinput, out string find, StringComparison sc = StringComparison.Ordinal, params string[] items)
        {
            foreach (string si in items)
            {
                if (sinput.Equals(si, sc))
                {
                    find = si;
                    return true;
                }
            }
            find = "";
            return false;
        }

        public static string GetBefore(this string input, string Before, string Until)
        {
            int index0 = input.Length - 1;
            string temp = input;
            if (!string.IsNullOrEmpty(Before))
            {
                index0 = input.IndexOf(Before);

                if (index0 <= 0) return string.Empty;
                temp = input.Remove(index0);
            }

            int index1 = Math.Max(0, temp.LastIndexOf(Until));

            temp = temp.Substring(index1 + 1);

            return temp;
        }

        public static string GetBetween(this string input, int start, int end)
        {
            if (start < 0) return "";
            if (end > 0)
            {
                int l = Math.Max(0, Math.Min(input.Length - start, (end - start) + 1));
                return input.Substring(start, l);
            }
            else
            {
                return input.Substring(start);
            }
        }

        public static string GetBetween(this string input, string left, string right = "")// params string[] to_replace)
        {
            return input.GetBetween(0, left, right);
        }

        public static string GetBetween(this string input, int start, string left, string right = "")
        {
            try
            {
                if (!input.Contains(left))
                {
                    return "";
                }

                start = Math.Max(0, Math.Min(input.Length - 1, start));

                string output = "";

                int i0 = input.IndexOf(left, start) + left.Length;
                if (right.Length > 0 && input.IndexOf(right, i0) >= 0)
                {
                    int i1 = input.IndexOf(right, i0);
                    if (i0 <= i1)
                    {
                        output = input.Substring(i0, i1 - i0);
                    }
                }
                else
                {
                    output = input.Substring(i0);
                }
                return output;
            }
            catch { }
            return "";
        }

        public static string[] GetBetweens(this string input, string left, string right)// params string[] to_replace)
        {
            List<string> list = new List<string>();
            int index = 0;
            string betw = "";
            while (index >= 0)
            {
                int i0 = input.IndexOf(left, index) + 1;
                int i1 = input.IndexOf(right, i0 + left.Length);
                if (i0 > 0 && i1 > i0)
                {
                    betw = input.Substring(i0, i1 - i0);
                    index = i1 + right.Length;
                    if (!string.IsNullOrEmpty(betw))
                    {
                        list.Add(betw);
                    }
                }
                else break;
            }
            return list.ToArray();
        }

#if Drawing

        public static BorderSize GetBorderSize(this Form form)
        {
            int small = ((form.Width - form.ClientSize.Width) / 2);
            int big = form.Height - form.ClientSize.Height - small;
            return new BorderSize(big, small);
        }

        public static byte GetBitsPerPixel(this PixelFormat format)
        {
            return (byte)(Image.GetPixelFormatSize(format) / 8);
        }

        public static RectangleF GetCenteredRectangleF(float x, float y, float height,
             float image_width, float image_height)
        {
            return new RectangleF(x, y + (height - image_height) / 2f, image_width, image_height);
        }

        public static Bitmap GetImage(this Cursor c)
        {
            Bitmap b = new Bitmap(25, 25);
            using (Graphics g = Graphics.FromImage(b))
            {
                c.Draw(g, new Rectangle(0, 0, 25, 25));
            }
            return b;
        }

        #region DRAWING

        public static void AddContextMenu(this RichTextBox rtb)
        {
            if (rtb.ContextMenuStrip == null)
            {
                ContextMenuStrip cms = new ContextMenuStrip { ShowImageMargin = false };
                ToolStripMenuItem tsmiCut = new ToolStripMenuItem("Cut");
                tsmiCut.Click += (sender, e) => rtb.Cut();
                cms.Items.Add(tsmiCut);
                ToolStripMenuItem tsmiCopy = new ToolStripMenuItem("Copy");
                tsmiCopy.Click += (sender, e) => rtb.Copy();
                cms.Items.Add(tsmiCopy);
                ToolStripMenuItem tsmiPaste = new ToolStripMenuItem("Paste");
                tsmiPaste.Click += (sender, e) => rtb.Paste();
                cms.Items.Add(tsmiPaste);
                rtb.ContextMenuStrip = cms;
            }
        }

        public static Rectangle GetMinBoundsRect(this Rectangle[] int32Rects)
        {
            int xMin = (int)int32Rects.Min(s => s.X);
            int yMin = (int)int32Rects.Min(s => s.Y);
            int xMax = (int)int32Rects.Max(s => s.X + s.Width);
            int yMax = (int)int32Rects.Max(s => s.Y + s.Height);
            var int32Rect = new Rectangle(xMin, yMin, xMax - xMin, yMax - yMin);
            return int32Rect;
        }

        public static Rectangle GetMinBoundsRect(this RectangleF[] int32Rects)
        {
            int xMin = (int)int32Rects.Min(s => s.X);
            int yMin = (int)int32Rects.Min(s => s.Y);
            int xMax = (int)int32Rects.Max(s => s.X + s.Width);
            int yMax = (int)int32Rects.Max(s => s.Y + s.Height);
            var int32Rect = new Rectangle(xMin, yMin, xMax - xMin, yMax - yMin);
            return int32Rect;
        }

        public static Bitmap MakeTransparent(this Bitmap bitmap, Color c, float factor)
        {
            long ticks = DateTime.Now.Ticks;
            Size size = bitmap.Size;
            factor = Math.Max(0, Math.Min(1.0f, factor));
            int r = (int)(c.R * factor);
            int g = (int)(c.G * factor);
            int b = (int)(c.B * factor);
            //Bitmap newBitmap = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppPArgb);
            BitmapData bData_Orig = bitmap.LockBits(new Rectangle(0, 0, size.Width, size.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            //BitmapData bData_New = newBitmap.LockBits(new Rectangle(0, 0, size.Width, size.Height), ImageLockMode.ReadWrite, newBitmap.PixelFormat);

            byte bitsPerPixel = bData_Orig.PixelFormat.GetBitsPerPixel();
            //byte bitsPerPixel_New = (byte)(GetBitsPerPixel(bData_New.PixelFormat) / 8);

            unsafe
            {
                /*This time we convert the IntPtr to a ptr*/
                byte* scan0 = (byte*)bData_Orig.Scan0.ToPointer();
                //byte* scan1 = (byte*)bData_New.Scan0.ToPointer();

                for (int i = 0; i < size.Height; i++)
                {
                    for (int j = 0; j < size.Width; j++)
                    {
                        byte* data = scan0 + i * bData_Orig.Stride + j * bitsPerPixel;
                        //byte* dataNew = scan1 + i * bData_New.Stride + j * bitsPerPixel_New;

                        if (data[0] > b && data[1] > g && data[2] > r)
                        {
                            //data[0] = 0;
                            //data[1] = 0;
                            //data[2] = 0;
                            data[3] = 0;
                        }
                    }
                }
            }
            bitmap.UnlockBits(bData_Orig);
            return bitmap;
            // MessageBox.Show(((DateTime.Now.Ticks - ticks) / (decimal)TimeSpan.TicksPerMillisecond).ToString());
        }

        public static Bitmap MakeUnTransparent(this Bitmap bitmap, Color c, int fac)
        {
            long ticks = DateTime.Now.Ticks;
            Size size = bitmap.Size;

            byte r = c.R;
            byte g = c.G;
            byte b = c.B;
            //Bitmap newBitmap = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppPArgb);
            BitmapData bData_Orig = bitmap.LockBits(new Rectangle(0, 0, size.Width, size.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            //BitmapData bData_New = newBitmap.LockBits(new Rectangle(0, 0, size.Width, size.Height), ImageLockMode.ReadWrite, newBitmap.PixelFormat);

            byte bitsPerPixel = bData_Orig.PixelFormat.GetBitsPerPixel();
            //byte bitsPerPixel_New = (byte)(GetBitsPerPixel(bData_New.PixelFormat) / 8);

            unsafe
            {
                /*This time we convert the IntPtr to a ptr*/
                byte* scan0 = (byte*)bData_Orig.Scan0.ToPointer();
                //byte* scan1 = (byte*)bData_New.Scan0.ToPointer();

                for (int i = 0; i < size.Height; i++)
                {
                    for (int j = 0; j < size.Width; j++)
                    {
                        byte* data = scan0 + i * bData_Orig.Stride + j * bitsPerPixel;
                        //byte* dataNew = scan1 + i * bData_New.Stride + j * bitsPerPixel_New;

                        if (data[3] < fac)
                        {
                            data[0] = b;
                            data[1] = g;
                            data[2] = r;
                            data[3] = 255;
                        }
                    }
                }
            }
            bitmap.UnlockBits(bData_Orig);
            return bitmap;
            // MessageBox.Show(((DateTime.Now.Ticks - ticks) / (decimal)TimeSpan.TicksPerMillisecond).ToString());
        }

        public static void DrawText(this Graphics g, string text, Font font, Color b, float x, float y)
        {
            TextRenderer.DrawText(g, text.Replace("&", "&&"), font, new Point((int)x, (int)y), b,
                  TextFormatFlags.NoPadding | TextFormatFlags.Left | TextFormatFlags.TextBoxControl);
        }

        public static void DrawText(this Graphics g, string text, Font font, Color b, RectangleF bounds, TextFormatFlags centerStrFormat)
        {
            TextRenderer.DrawText(g, text.Replace("&", "&&"), font, Rectangle.Truncate(bounds), b, centerStrFormat);
        }

        public static SizeF MeasureText(this Graphics g, string text, Font font)
        {
            //return g.MeasureString(text, font);
            return TextRenderer.MeasureText(g, text, font, maxSize,
                TextFormatFlags.NoPadding | TextFormatFlags.Left | TextFormatFlags.TextBoxControl);
        }

        #endregion DRAWING

        public static Rectangle MaxSize(this Rectangle input, Rectangle max)
        {
            return new Rectangle(
                input.X,
                input.Y,
                Math.Max(input.Width, max.Width),
                Math.Max(input.Height, max.Height));
        }

        public static Rectangle ParseRectangleScreenSafe(this string s)
        {
            try
            {
                int w = 0;
                int h = 0;
                string[] sa = s.Split(new char[] { '=', ',', '}' }, StringSplitOptions.RemoveEmptyEntries);
                int x = 0;
                int y = 0;

                Int32.TryParse(sa[1], out x);
                Int32.TryParse(sa[3], out y);
                Int32.TryParse(sa[5], out w);
                Int32.TryParse(sa[7], out h);
                var rec = new Rectangle(x, y, w, h);

                int xi = Screen.AllScreens.Min(t => t.Bounds.X);
                int yi = Screen.AllScreens.Min(t => t.Bounds.Y);
                int r = Screen.AllScreens.Max(t => t.Bounds.Right);
                int b = Screen.AllScreens.Max(t => t.Bounds.Bottom);

                x = Math.Max(xi, Math.Min(r - rec.Width, x));
                y = Math.Max(yi, Math.Min(b - rec.Height, y));

                return new Rectangle(x, y, w, h);
            }
            catch { } return Rectangle.Empty;
        }

        public static Rectangle ScreenSafe(this Rectangle rec)
        {
            try
            {
                int x = Screen.AllScreens.Min(t => t.Bounds.X);
                int y = Screen.AllScreens.Min(t => t.Bounds.Y);
                int r = Screen.AllScreens.Max(t => t.Bounds.Right);
                int b = Screen.AllScreens.Max(t => t.Bounds.Bottom);

                x = Math.Max(x, Math.Min(r - rec.Width, rec.X));
                y = Math.Max(y, Math.Min(b - rec.Height, rec.Y));

                return new Rectangle(x, y, rec.Width, rec.Height);
            }
            catch { } return Rectangle.Empty;
        }

        public static void SetGraphicQuality(this Graphics g, bool textQuality, bool imageQuality)
        {
            g.TextRenderingHint = (textQuality ? System.Drawing.Text.TextRenderingHint.ClearTypeGridFit : System.Drawing.Text.TextRenderingHint.SingleBitPerPixel);

            if (imageQuality)
            {
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            }
            else
            {
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            }
        }

#endif

        public static DateTime GetLinkerTime(byte[] buffer)
        {
            if (buffer.Length < BufferSize) return DateTime.MinValue;

            const int c_PeHeaderOffset = 60;
            const int c_LinkerTimestampOffset = 8;

            var offset = BitConverter.ToInt32(buffer, c_PeHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(buffer, offset + c_LinkerTimestampOffset);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var linkTimeUtc = epoch.AddSeconds(secondsSince1970);

            var localTime = TimeZoneInfo.ConvertTimeFromUtc(linkTimeUtc, TimeZoneInfo.Local);

            return localTime;
        }

        public static DateTime GetLinkerTime(string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                byte[] ba = new byte[BufferSize];
                stream.Read(ba, 0, BufferSize);
                return GetLinkerTime(ba);
            }
        }

#if Interop

        public static DateTime GetLinkerTime(this Assembly assembly)
        {
            var filePath = assembly.Location;
            return GetLinkerTime(filePath);
        }

#endif

        public static DateTime GetOnlineLinkerTime(string url)
        {
            WebClient wc = new WebClient();
            wc.Proxy = null;
            var buffer = new byte[512];

            using (var stream = wc.OpenRead(url))
            {
                stream.Read(buffer, 0, 512);
            }
            wc.Dispose();

            const int c_PeHeaderOffset = 60;
            const int c_LinkerTimestampOffset = 8;
            var offset = BitConverter.ToInt32(buffer, c_PeHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(buffer, offset + c_LinkerTimestampOffset);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var linkTimeUtc = epoch.AddSeconds(secondsSince1970);

            var tz = TimeZoneInfo.Local;
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(linkTimeUtc, tz);

            return localTime;
        }

#if Drawing

        public static bool IsGif(this Bitmap b)
        {
            return ImageFormat.Gif.Equals(b.RawFormat);
        }

        public static MessageBoxResult Handle(this Exception e, string extraInfo = "", bool showMsgBox = true)
        {
            string err = DateTime.Now.ToString() + "\r\n" + e.ToString() + "\r\n\r\n";
            try
            {
                lock (errorLock)
                {
                    File.AppendAllText(LX29_ChatClient.Settings.dataDir + "Error.log", err);
                }
                if (showMsgBox)
                {
                    var sa = e.ToString().Split("\r\n");
                    StringBuilder sb = new StringBuilder();

                    for (int i = 0; i < sa.Length; i++)
                    {
                        var s = sa[i];
                        var si = s.Trim()
                            .Replace("cs:", "cs-")
                            .Replace(" in ", "\r\n");
                        ////.LastLine("\\");
                        //if (si.Contains(":\\"))
                        //{
                        //    var spl = si.GetBefore(":\\", " ");
                        //    si = si.Replace(spl, "\r\n" + spl);
                        //}

                        sb.AppendLine(si);
                        if (i == 0)
                        {
                            sb.AppendLine();
                        }
                    }
                    if (!string.IsNullOrEmpty(extraInfo))
                    {
                        sb.AppendLine();
                        sb.AppendLine(extraInfo);
                    }

                    if (LX29_ChatClient.Settings.ShowErrors)
                    {
                        if (errorCount >= 2)
                        {
                            errorCount = 0;
                            return LX29_MessageBox.Show(sb.ToString(), "Error!", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        if (errorCount >= 4)
                        {
                            errorCount = 0;
                            return MessageBoxResult.Ignore;
                        }
                    }
                    errorCount++;
                    Threading.Thread.CurrentThread.Join(1000);
                    return MessageBoxResult.Retry;
                }
            }
            catch { }
            return MessageBoxResult.None;
        }

#endif

        public static string HashCode(this string s)
        {
            return Encoding.UTF8.GetBytes(s).Select(t => t.ToString()).Aggregate((t, t0) => t + t0);
        }

        public static string LastLine(this string input, params string[] splitStrings)
        {
            string[] sa = input.Split(splitStrings, StringSplitOptions.RemoveEmptyEntries);
            if (sa.Length > 0)
            {
                return sa[sa.Length - 1];
            }
            return input;
        }

        public static string NextChar(this Random rd)
        {
            int t = rd.Next(0, 91);
            string c = "@";
            if (t < 30)
            {
                c = Encoding.ASCII.GetString(new byte[] { (byte)rd.Next(48, 58) });
            }
            else if (t < 60)
            {
                c = Encoding.ASCII.GetString(new byte[] { (byte)rd.Next(65, 91) });
            }
            else
            {
                c = Encoding.ASCII.GetString(new byte[] { (byte)rd.Next(97, 122) });
            }
            return c;
        }

        /// <summary>
        /// Read Until Char is Found
        /// </summary>
        /// <param name="sr"></param>
        /// <param name="until">Char until to read.</param>
        /// <returns></returns>
        public static string Read(this StringReader sr, char until, char[] arr)
        {
            try
            {
                string output = new string(arr);
                while (true)
                {
                    char c = ((char)sr.Read());//
                    output += c;
                    if (c.Equals(until))
                    {
                        break;
                    }
                }
                return output;
            }
            catch { }
            return "";
        }

        public static string Read(this StringReader sr, char until)
        {
            try
            {
                char[] buffer = new char[32];
                sr.Read(buffer, 0, 32);
                return sr.Read(until, buffer);
            }
            catch { }
            return "";
        }

        /// <summary>
        /// Read Until Char is Found
        /// </summary>
        /// <param name="sr"></param>
        /// <param name="until">Char until to read.</param>
        /// <returns></returns>
        public static string Read(this StreamReader sr, char until, char[] arr)
        {
            try
            {
                string output = new string(arr);
                while (true)
                {
                    char c = ((char)sr.Read());//
                    output += c;
                    if (c.Equals(until))
                    {
                        break;
                    }
                }
                return output;
            }
            catch { }
            return "";
        }

        public static string Read(this StreamReader sr, char until)
        {
            try
            {
                char[] buffer = new char[32];
                sr.Read(buffer, 0, 32);
                return sr.Read(until, buffer);
            }
            catch { }
            return "";
        }

        public static string RemoveControlChars(this string s)
        {
            string si = "";
            for (int i = 0; i < s.Length; i++)
            {
                if (!char.IsControl(s[i]))
                {
                    si += s[i];
                }
            }
            return si;
        }

        public static string RemoveLongNumbers(this string input, int minLength)
        {
            int cnt = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsNumber(input[i]))
                {
                    cnt++;
                }
                else
                {
                    if (cnt >= minLength)
                    {
                        input = input.Remove(i - cnt, cnt);
                    }
                    cnt = 0;
                }
            }
            return input;
        }

        public static string RemoveNonChars(this string s)
        {
            string si = "";
            for (int i = 0; i < s.Length; i++)
            {
                if (char.IsLetterOrDigit(s[i]))
                {
                    si += s[i];
                }
            }
            return si;
        }

        public static string RemoveNonCharsAndDigits(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                string si = "";
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsLetter(s[i]))
                    {
                        si += s[i];
                    }
                }
                return si;
            }
            return s;
        }

        public static string RemoveStart(this string input, params string[] toremove)
        {
            string output = input;
            foreach (string s in toremove)
            {
                while (output.StartsWith(s))
                {
                    output = output.Remove(0, s.Length);
                }
            }
            return output;
        }

        public static string RemoveUntil(this string input, string Until, int start = 0)
        {
            if (string.IsNullOrEmpty(input)) return input;
            int idx = input.IndexOf(Until, start);
            if (idx <= 0) return input;

            return input.Substring(idx + 1);
        }

        public static string ReplaceAll(this string input, string replace_to, bool caseSensitive, params string[] to_replace)
        {
            if (!caseSensitive)
            {
                string output = input;
                foreach (string s in to_replace)
                {
                    string sLow = s.ToLower();
                    string iLow = output.ToLower();
                    while (iLow.Contains(sLow))
                    {
                        int i0 = iLow.IndexOf(sLow);
                        int i1 = iLow.IndexOf(sLow[sLow.Length - 1], i0);
                        if (i0 <= i1)
                        {
                            output = output.Remove(i0, sLow.Length);
                            output = output.Insert(i0, replace_to);
                        }
                        iLow = output.ToLower();
                    }
                }
                return output;
            }
            else
            {
                return input.ReplaceAll(replace_to, to_replace);
            }
        }

        public static string ReplaceAll(this string input, string replace_to, params string[] to_replace)
        {
            if (to_replace.Length == 0) return input;
            string output = input;
            foreach (string s in to_replace)
            {
                output = output.Replace(s, replace_to);
            }
            return output;
        }

        public static string ReplaceBetween(this string input, int start, int end, string replace)
        {
            if (start < 0) return "";
            if (end > 0)
            {
                return input.Remove(start, (end - start) + 1).Insert(start, replace);
            }
            else
            {
                return input.Remove(start, (input.Length - start) + 1).Insert(start, replace);
            }
        }

        public static string ReplaceBetween(this string input, int start, int end, string left, string right, string toReplace)// params string[] to_replace)
        {
            string output = input;
            int i1 = 0;
            while (true)
            {
                if (!output.Contains(left) || !output.Contains(right)) break;

                string iLow = output.ToLower();

                int i0 = iLow.IndexOf(left, start);
                i1 = iLow.IndexOf(right, i0) + 1;
                if (i1 >= end) break;
                if (i0 <= i1)
                {
                    output = output.Remove(i0, i1 - i0);
                    output = output.Insert(i0, toReplace);
                }
            }
            return output;
        }

        public static string ReplaceEnd(this string input, string replace_to, params string[] to_replace)
        {
            string output = input;
            foreach (string s in to_replace)
            {
                while (true)
                {
                    if (output.EndsWith(s))
                    {
                        output = input.Remove(output.Length - s.Length);
                        output = input + replace_to;
                    }
                    else break;
                }
            }
            return output;
        }

        public static string ReplaceStart(this string input, string replace_to, params string[] to_replace)
        {
            string output = input;
            foreach (string s in to_replace)
            {
                while (true)
                {
                    if (output.StartsWith(s))
                    {
                        output = output.Remove(0, s.Length);
                        output = replace_to + output;
                    }
                    else break;
                }
            }
            return output;
        }

        public static string[] Split(this string s, params string[] sa)
        {
            return s.Split(sa, StringSplitOptions.RemoveEmptyEntries);
        }

        public static List<string> SplitWith(this string input, IEnumerable<string> sa)
        {
            List<string> list = new List<string>();
            List<int> idxs = new List<int>();
            foreach (string s in sa)
            {
                int idx = input.IndexOf(s);
                if (idx >= 0)
                {
                    idxs.Add(idx);
                }
            }
            idxs.Sort();
            for (int i = 0; i < idxs.Count; i++)
            {
                string s;
                if (i + 1 < idxs.Count)
                {
                    s = input.Substring(idxs[i], idxs[i + 1] - idxs[i]);
                }
                else
                {
                    s = input.Substring(idxs[i]);
                }
                if (!string.IsNullOrEmpty(s))
                {
                    list.Add(s);
                }
            }
            return list;
        }

        public static bool StartsWithAny(this string s, out string find, bool toLower, string appendsearch, params string[] items)
        {
            string sinput = (toLower) ? (s + appendsearch).ToLower() : (s + appendsearch);
            foreach (string si in items)
            {
                string sii = (toLower) ? si.ToLower() : si;

                if (sinput.StartsWith(si))
                {
                    find = si;
                    return true;
                }
            }
            find = "";
            return false;
        }

        public static string Substring(this string input, string index)
        {
            int idx = input.LastIndexOf(index);
            if (idx >= 0 && idx < input.Length)
            {
                return input.Substring(idx + index.Length);
            }
            return "";
        }

        public static string ToTitleCase(this string str)
        {
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
        }

        public static string Trim(this string input, params string[] toremove)
        {
            string output = input;
            foreach (string s in toremove)
            {
                if (output.StartsWith(s))
                {
                    output = output.Remove(0, s.Length);
                }
                if (output.EndsWith(s))
                {
                    output = output.Remove(output.Length - s.Length);
                }
            }
            return output;
        }

        public static string UppercaseFirst(this string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.ToLower().Substring(1);
        }

        public static double Constrain(this double d, double Min, double Max)
        {
            return Math.Max(Min, Math.Min(Max, d));
        }
    }

#if Interop

    public static class NativeMethods
    {
#if Drawing

        public static Rectangle GetWindowRect(IntPtr ID)
        {
            RECT r = new RECT();
            if (GetWindowRect(ID, out r))
            {
                return Rectangle.FromLTRB(r.Left, r.Top, r.Right, r.Bottom);
            }
            return Rectangle.Empty;
        }

#endif

        public static RECT GetWindowRECT(IntPtr ID)
        {
            RECT r = new RECT();
            if (GetWindowRect(ID, out r))
            {
                return r;
            }
            return new RECT();// Rectangle.Empty;
        }

        public static bool PrintWindow(IntPtr hwnd, IntPtr hDC)
        {
            return PrintWindow(hwnd, hDC, 0);
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
    }

#endif
}

namespace System.Windows.Forms
{
    public struct BorderSize
    {
        public readonly int BigBorder;

        public readonly int SmallBorder;

        public BorderSize(int big, int small)
        {
            BigBorder = big;
            SmallBorder = small;
        }

        public int BigSmall
        {
            get { return BigBorder + SmallBorder; }
        }
    }
}

#if Drawing

namespace System.Drawing
{
    public struct YIQ
    {
        private double y;

        public double Y
        {
            get { return y; }
            set { y = Math.Max(0.0, Math.Min(1.0, value)); }
        }

        private double i;

        public double I
        {
            get { return y; }
            set { y = Math.Max(0.0, Math.Min(1.0, value)); }
        }

        private double q;

        public double Q
        {
            get { return q; }
            set { q = Math.Max(0.0, Math.Min(1.0, value)); }
        }

        public YIQ(Color c)
        {
            y = 0.299 * c.R + 0.587 * c.G + 0.114 * c.B;
            i = 0.596 * c.R - 0.275 * c.G - 0.321 * c.B;
            q = 0.212 * c.R - 0.523 * c.G + 0.311 * c.B;
        }

        public Color ToColor()
        {
            double R = Y + 0.9563 * I + 0.6210 * Q;
            double G = Y - 0.2721 * I - 0.6474 * Q;
            double B = Y - 1.1070 * I + 1.7046 * Q;
            return Color.FromArgb((int)R.Constrain(0, 255), (int)G.Constrain(0, 255), (int)B.Constrain(0, 255));
        }
    }

    public struct HSL
    {
        private double _Hue;

        private double _Saturation;

        private double _Brightness;

        public double Hue
        {
            get { return _Hue; }
            set
            {
                _Hue = value;
                while (_Hue < 0.0) _Hue += 360.0;
                while (_Hue > 360.0) _Hue -= 360.0;
            }
        }

        public double Saturation
        {
            get { return _Saturation; }
            set { _Saturation = Math.Max(0.0, Math.Min(1.0, value)); }
        }

        public double Brightness
        {
            get { return _Brightness; }
            set { _Brightness = Math.Max(0.0, Math.Min(1.0, value)); }
        }

        public void Invert()
        {
            Hue += 180.0;
        }

        public Color ToColor()
        {
            try
            {
                double hue = _Hue;
                double saturation = _Saturation;
                double brightness = _Brightness;
                if (0.0 > hue || 360.0 < hue)
                {
                    while (hue < 0.0) hue += 360.0;
                    while (hue > 360.0) hue -= 360.0;
                }
                if (0.0 > saturation || 1.0 < saturation)
                {
                    saturation = Math.Max(0.0, Math.Min(1.0, saturation));
                }
                if (0.0 > brightness || 1.0 < brightness)
                {
                    saturation = Math.Max(0.0, Math.Min(1.0, brightness));
                }

                if (0.0 == saturation)
                {
                    return Color.FromArgb(Convert.ToInt32(brightness * 255),
                      Convert.ToInt32(brightness * 255), Convert.ToInt32(brightness * 255));
                }

                double fMax, fMid, fMin;
                int iSextant, iMax, iMid, iMin;

                if (0.5 < brightness)
                {
                    fMax = brightness - (brightness * saturation) + saturation;
                    fMin = brightness + (brightness * saturation) - saturation;
                }
                else
                {
                    fMax = brightness + (brightness * saturation);
                    fMin = brightness - (brightness * saturation);
                }

                iSextant = (int)Math.Floor(hue / 60.0);
                if (300.0 <= hue)
                {
                    hue -= 360.0;
                }
                hue /= 60.0;
                hue -= 2.0 * (float)Math.Floor(((iSextant + 1.0) % 6.0) / 2.0);
                if (0 == iSextant % 2)
                {
                    fMid = hue * (fMax - fMin) + fMin;
                }
                else
                {
                    fMid = fMin - hue * (fMax - fMin);
                }
                fMid = Math.Max(0, Math.Min(1f, fMid));

                iMax = (int)(fMax * 255);
                iMid = (int)(fMid * 255);
                iMin = (int)(fMin * 255);

                switch (iSextant)
                {
                    case 1:
                        return Color.FromArgb(iMid, iMax, iMin);

                    case 2:
                        return Color.FromArgb(iMin, iMax, iMid);

                    case 3:
                        return Color.FromArgb(iMin, iMid, iMax);

                    case 4:
                        return Color.FromArgb(iMid, iMin, iMax);

                    case 5:
                        return Color.FromArgb(iMax, iMin, iMid);

                    default:
                        return Color.FromArgb(iMax, iMid, iMin);
                }
            }
            catch
            {
            }
            return Color.White;
        }

        public HSL(Color c)
        {
            _Hue = c.GetHue();
            _Saturation = c.GetSaturation();
            _Brightness = c.GetBrightness();
        }

        public HSL(double Hue, double Saturation, double Brightness)
        {
            _Hue = Hue;
            _Saturation = Saturation;
            _Brightness = Brightness;
        }
    }
}

#endif