#define Drawing
#define Interop
#define Unsafe
#define ManagedGDI
#define ChatClient

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

        public static decimal Constrain(decimal x, decimal min, decimal max)
        {
            return Math.Max(min, Math.Max(max, x));
        }

        public static int Constrain(int x, int min, int max)
        {
            return Math.Max(min, Math.Max(max, x));
        }

        public static long Constrain(long x, long min, long max)
        {
            return Math.Max(min, Math.Max(max, x));
        }

        public static double Constrain(double x, double min, double max)
        {
            return Math.Max(min, Math.Max(max, x));
        }

        public static float Constrain(float x, float min, float max)
        {
            return Math.Max(min, Math.Max(max, x));
        }

        public static double Map(double x, double in_min, double in_max, double out_min, double out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        public static float Map(float x, float in_min, float in_max, float out_min, float out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        private static readonly string[] SizeSuffixes = { "B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB", "ZiB", "YiB" };

        public static string SizeSuffix(this long value, int decimalPlaces = 3)
        {
            return SizeSuffix((double)value, decimalPlaces);
            //if (value < 0) { return "-" + SizeSuffix(-value); }
            //if (value == 0) { return "0.0 bytes"; }

            //// mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            //int mag = (int)Math.Log(value, 1024);

            //// 1L << (mag * 10) == 2 ^ (10 * mag)
            //// [i.e. the number of bytes in the unit corresponding to mag]
            //decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            //// make adjustment when the value is large enough that
            //// it would round up to 1000 or more
            //if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            //{
            //    mag += 1;
            //    adjustedSize /= 1024;
            //}

            //return string.Format("{0:n" + decimalPlaces + "} {1}",
            //    adjustedSize,
            //    SizeSuffixes[mag]);
        }

        public static string SizeSuffix(this int value, int decimalPlaces = 3)
        {
            return SizeSuffix((double)value, decimalPlaces);
            //if (value < 0) { return "-" + SizeSuffix(-value); }
            //if (value == 0) { return "0.0 bytes"; }

            //// mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            //int mag = (int)Math.Log(value, 1024);

            //// 1L << (mag * 10) == 2 ^ (10 * mag)
            //// [i.e. the number of bytes in the unit corresponding to mag]
            //decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            //// make adjustment when the value is large enough that
            //// it would round up to 1000 or more
            //if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            //{
            //    mag += 1;
            //    adjustedSize /= 1024;
            //}

            //return string.Format("{0:n" + decimalPlaces + "} {1}",
            //    adjustedSize,
            //    SizeSuffixes[mag]);
        }

        public static string SizeSuffix(this float value, int decimalPlaces = 3)
        {
            return SizeSuffix((double)value, decimalPlaces);
            //if (value < 0) { return "-" + SizeSuffix(-value); }
            //if (value == 0) { return "0.0 bytes"; }

            //// mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            //int mag = (int)Math.Log(value, 1024);

            //// 1L << (mag * 10) == 2 ^ (10 * mag)
            //// [i.e. the number of bytes in the unit corresponding to mag]
            //decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            //// make adjustment when the value is large enough that
            //// it would round up to 1000 or more
            //if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            //{
            //    mag += 1;
            //    adjustedSize /= 1024;
            //}

            //return string.Format("{0:n" + decimalPlaces + "} {1}",
            //    adjustedSize,
            //    SizeSuffixes[mag]);
        }

        public static string SizeMag(this int value, int decimalPlaces = 3)
        {
            if (value < 0) { return "-" + SizeMag(-value); }
            if (value == 0) { return "0"; }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1000);

            // 1L << (mag * 10) == 2 ^ (10 * mag)
            // [i.e. the number of bytes in the unit corresponding to mag]
            double adjustedSize = (double)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1000;
            }

            if (Math.Abs(adjustedSize % 1) <= (double.Epsilon * 100))
            {
                decimalPlaces = 0;
            }

            return adjustedSize.ToString("N" + decimalPlaces) + SizeSuffixes[mag].ReplaceAll("", "B", "i");
        }

        public static string SizeSuffix(this double value, int decimalPlaces = 3)
        {
            if (value < 0) { return "-" + SizeSuffix(-value); }
            if (value == 0) { return "0 bytes"; }

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
        public static readonly Size maxSize = new Size(int.MaxValue, int.MaxValue);

        public static void DrawBitmap(this Graphics g, Bitmap image, float x, float y, float width, float height, bool drawHW)
        {
#if ManagedGDI
            if (drawHW)
            {
                ManagedGDI.AlphaBlend((Bitmap)image, g, new Rectangle((int)x, (int)y, (int)width, (int)height), 255);
            }
            else
#endif
            {
                g.DrawImage(image, x, y, width, height);
            }
        }

        public static Point Center(this Rectangle r)
        {
            return new Point(r.X + r.Width / 2, r.Y + r.Height / 2);
        }

        public static PointF Center(this RectangleF r)
        {
            return new PointF(r.X + r.Width / 2, r.Y + r.Height / 2);
        }

        public static Point Angle(this Rectangle r, float angle)
        {
            return new Point((int)((r.Width / 2) * Math.Cos(angle)),
            (int)((r.Height / 2) * Math.Sin(angle)));
        }

        public static bool IsGif(this Bitmap b)
        {
            return ImageFormat.Gif.Equals(b.RawFormat);
        }

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

#if Unsafe

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

#endif

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
                g.PixelOffsetMode = Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.SmoothingMode = Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
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

        public static DateTime GetOnlineLinkerTime(this Stream stream, out byte[] buffer)
        {
            buffer = new byte[512];
            stream.Read(buffer, 0, 512);

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

        public static MessageBoxResult Handle(this Exception e, string extraInfo = "", bool showMsgBox = false)
        {
            lock (errorLock)
            {
                string err = DateTime.Now.ToString() + "\r\n" + e.ToString() + "\r\n\r\n";
                try
                {
#if ChatClient
                    File.AppendAllText(LX29_ChatClient.Settings._dataDir + "Error.log", err);
#endif
                    var sa = e.ToString().Split("\r\n");
                    StringBuilder sb = new StringBuilder();

                    for (int i = 0; i < sa.Length; i++)
                    {
                        var s = sa[i];
                        var si = s.Trim()
                            .Replace("cs:", "cs-")
                            .Replace(" in ", "\r\n");
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
#if ChatClient
                    if (LX29_ChatClient.Settings.ShowErrors || showMsgBox)
                    {
                        return LX29_MessageBox.Show(sb.ToString(), "Error!", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);
                    }
                    else
                    {
                        if (errorCount >= 2)
                        {
                            errorCount = 0;
                            return MessageBoxResult.Ignore;
                        }
                    }
#else
                        return LX29_MessageBox.Show(sb.ToString(), "Error!", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);
#endif
                    errorCount++;
                    Threading.Thread.CurrentThread.Join(100);
                    return MessageBoxResult.Retry;
                }
                catch { }
                return MessageBoxResult.None;
            }
        }

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

        //public static bool IsEmpty(this string s)
        //{
        //    return string.IsNullOrEmpty(s);
        //}

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

        public static List<string> SplitAtSpecial(this string s)
        {
            List<string> output = new List<string>();
            StringBuilder sb = new StringBuilder();
            foreach (char c in s)
            {
                if (!char.IsLetterOrDigit(c) || !c.Equals('_'))
                {
                    if (sb.Length > 0)
                    {
                        output.Add(sb.ToString().Trim());
                        sb.Clear();
                    }
                }
                else
                {
                    sb.Append(c);
                }
            }
            return output;
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

#if Drawing

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

namespace System.Windows.Forms
{
    public struct LX29_MessageBoxResult
    {
        public MessageBoxResult Result;
        public string Value;

        public LX29_MessageBoxResult(MessageBoxResult result, string value)
        {
            Result = result;
            Value = value;
        }
    }

    public class LX29_MessageBox
    {
        public static MessageBoxResult Show(string text, string caption = "", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            using (MessageBoxForm f = new MessageBoxForm())
            {
                return f.ShowDialog(text, caption, buttons, icon).Result;
            }
        }

        public static LX29_MessageBoxResult Show(string text, bool ShowTextbox, string textBoxText, string textBoxTitle, string caption = "", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            using (MessageBoxForm f = new MessageBoxForm())
            {
                return f.ShowDialog(text, caption, buttons, icon, ShowTextbox, textBoxText, textBoxTitle);
            }
        }
    }

    public enum MessageBoxResult
    {
        OK,
        Cancel,
        Retry,
        Abort,
        Ignore,
        No,
        Yes,
        None
    }

    public partial class MessageBoxForm : Form
    {
        private MessageBoxResult diagRes = MessageBoxResult.None;

        public MessageBoxForm()
        {
            InitializeComponent();
            rtB_Main.LinkClicked += txtB_Main_LinkClicked;
            rtB_Main.AddContextMenu();
        }

        public new Color BackColor
        {
            get { return base.BackColor; }
            set
            {
                base.BackColor = value;
                rtB_Main.BackColor = value;
                button1.BackColor = Color.FromArgb(40, 40, 40);
                button2.BackColor = button1.BackColor;
                button3.BackColor = button1.BackColor;
            }
        }

        public new string Text
        {
            get { return rtB_Main.Text; }
            set
            {
                rtB_Main.Text = value;
            }
        }

        public string Title
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        public LX29_MessageBoxResult ShowDialog(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, bool ShowTextbox = false, string textBoxText = "", string textBoxTitle = "")
        {
            this.Text = text.Trim('\r', '\n', ' ');
            //var temp = text.Split("\r\n");
            //List<string> lines = new List<string>();
            //foreach (var line in temp)
            //{
            //    if (line.Length > 100)
            //    {
            //        lines.Add(line.Remove(101));
            //        lines.Add(line.Substring(100));
            //    }
            //    else
            //    {
            //        lines.Add(line);
            //    }
            //}
            //var msr = lines.OrderByDescending(t => t.Length).First();
            //Size s = TextRenderer.MeasureText(this.Text, new Font(this.Font.Name, this.Font.Size + 1.2f));
            //this.Width = Math.Max(350, s.Width);
            //this.Height = Math.Min(640, s.Height + (this.Height - button1.Top) + txtB_Main.Top);

            //520
            if (!ShowTextbox)
            {
                label1.Location = button1.Location;
                rtB_Main.Height = (label1.Top - rtB_Main.Top) - 5;
            }
            var border = this.GetBorderSize();
            var s = rtB_Main.GetPreferredSize(new System.Drawing.Size());
            int width = Math.Max(520, s.Width + (border.SmallBorder * 4) + rtB_Main.Left + (this.ClientSize.Width - rtB_Main.Right));
            int height = Math.Min(640, s.Height + (this.ClientSize.Height - label1.Top) +
                rtB_Main.Top + border.BigSmall + 10);

            Screen screen = Screen.FromControl(this);
            this.Width = Math.Min(screen.Bounds.Width - this.Left, width);
            this.Height = Math.Min(screen.Bounds.Height - this.Top, height);

            this.Title = caption;
            switch (icon)
            {
                case MessageBoxIcon.Error:
                    BackColor = Color.DarkRed;
                    System.Media.SystemSounds.Exclamation.Play();
                    break;

                case MessageBoxIcon.Question:
                    System.Media.SystemSounds.Question.Play();
                    break;

                case MessageBoxIcon.Asterisk:
                    System.Media.SystemSounds.Asterisk.Play();
                    break;
            }

            switch (buttons)
            {
                case MessageBoxButtons.OK:
                    button1.Text = "OK";
                    button2.Text = "";
                    button3.Text = "";
                    break;

                case MessageBoxButtons.OKCancel:
                    button1.Text = "OK";
                    button2.Text = "Cancel";
                    button3.Text = "";
                    break;

                case MessageBoxButtons.AbortRetryIgnore:
                    button1.Text = "Retry";
                    button2.Text = "Abort";
                    button3.Text = "Ignore";
                    break;

                case MessageBoxButtons.YesNoCancel:
                    button1.Text = "OK";
                    button2.Text = "No";
                    button3.Text = "Cancel";
                    break;

                case MessageBoxButtons.YesNo:
                    button1.Text = "Yes";
                    button2.Text = "No";
                    button3.Text = "";
                    break;

                case MessageBoxButtons.RetryCancel:
                    button1.Text = "Retry";
                    button2.Text = "Cancel";
                    button3.Text = "";
                    break;
            }
            button1.Visible = (button1.Text.Length > 0);
            button2.Visible = (button2.Text.Length > 0);
            button3.Visible = (button3.Text.Length > 0);

            textBox1.Visible = ShowTextbox;
            textBox1.Text = textBoxText;
            label1.Visible = ShowTextbox;
            label1.Text = textBoxTitle;
            if (ShowTextbox)
                textBox1.Show();

            this.BringToFront();
            this.TopMost = true;
            base.ShowDialog();
            return new LX29_MessageBoxResult(diagRes, textBox1.Text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            diagRes = (MessageBoxResult)Enum.Parse(typeof(MessageBoxResult), button1.Text);
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            diagRes = (MessageBoxResult)Enum.Parse(typeof(MessageBoxResult), button2.Text);
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            diagRes = (MessageBoxResult)Enum.Parse(typeof(MessageBoxResult), button3.Text);
            this.Close();
        }

        private void MessageBoxForm_Load(object sender, EventArgs e)
        {
            //txtB_Main.WordWrap = false;
            //txtB_Main.ContentsResized += txtB_Main_ContentsResized;
        }

        private void MessageBoxForm_Resize(object sender, EventArgs e)
        {
            base.Text = this.Size.ToString();
        }

        private void txtB_Main_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            LX29_ChatClient.Settings.StartBrowser(e.LinkText);
        }

        //private void txtB_Main_ContentsResized(object sender, ContentsResizedEventArgs e)
        //{
        //    var richTextBox = (RichTextBox)sender;
        //    richTextBox.Width = e.NewRectangle.Width;
        //    richTextBox.Height = e.NewRectangle.Height;

        //    this.Width = Math.Max(350, e.NewRectangle.Width);
        //    this.Height = Math.Min(640, e.NewRectangle.Height + (this.Height - button1.Top) + txtB_Main.Top);
        //}
    }

    partial class MessageBoxForm
    {
        private Button btn_Copy;

        private System.Windows.Forms.Button button1;

        private System.Windows.Forms.Button button2;

        private System.Windows.Forms.Button button3;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private Label label1;

        private RichTextBox rtB_Main;

        private TextBox textBox1;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.btn_Copy = new System.Windows.Forms.Button();
            this.rtB_Main = new System.Windows.Forms.RichTextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            //
            // button1
            //
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.AutoSize = true;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(12, 202);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button1.MinimumSize = new System.Drawing.Size(90, 30);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(90, 30);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            //
            // button2
            //
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.AutoSize = true;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(114, 202);
            this.button2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button2.MinimumSize = new System.Drawing.Size(90, 30);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(90, 30);
            this.button2.TabIndex = 2;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            //
            // button3
            //
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button3.AutoSize = true;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Location = new System.Drawing.Point(216, 202);
            this.button3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button3.MinimumSize = new System.Drawing.Size(90, 30);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(90, 30);
            this.button3.TabIndex = 3;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            //
            // btn_Copy
            //
            this.btn_Copy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Copy.AutoSize = true;
            this.btn_Copy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_Copy.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Copy.Location = new System.Drawing.Point(479, 202);
            this.btn_Copy.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_Copy.MinimumSize = new System.Drawing.Size(90, 30);
            this.btn_Copy.Name = "btn_Copy";
            this.btn_Copy.Size = new System.Drawing.Size(90, 30);
            this.btn_Copy.TabIndex = 4;
            this.btn_Copy.Text = "Copy Text";
            this.btn_Copy.UseVisualStyleBackColor = true;
            //
            // txtB_Main
            //
            this.rtB_Main.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtB_Main.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.rtB_Main.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtB_Main.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.rtB_Main.ForeColor = System.Drawing.Color.Gainsboro;
            this.rtB_Main.Location = new System.Drawing.Point(12, 12);
            this.rtB_Main.Name = "txtB_Main";
            this.rtB_Main.ReadOnly = true;
            this.rtB_Main.Size = new System.Drawing.Size(557, 142);
            this.rtB_Main.TabIndex = 6;
            this.rtB_Main.Text = "";
            //
            // textBox1
            //
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.ForeColor = System.Drawing.Color.Gainsboro;
            this.textBox1.Location = new System.Drawing.Point(12, 176);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(161, 22);
            this.textBox1.TabIndex = 8;
            this.textBox1.Visible = false;
            //
            // label1
            //
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 157);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 16);
            this.label1.TabIndex = 9;
            this.label1.Text = "Input";
            this.label1.Visible = false;
            //
            // MessageBoxForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.ClientSize = new System.Drawing.Size(581, 234);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.rtB_Main);
            this.Controls.Add(this.btn_Copy);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MessageBoxForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Load += new System.EventHandler(this.MessageBoxForm_Load);
            this.Resize += new System.EventHandler(this.MessageBoxForm_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion Windows Form Designer generated code
    }
}

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