using System.Runtime.InteropServices;

namespace System.Drawing
{
    public class GDI32
    {
        private const byte AC_SRC_ALPHA = 0x1;

        private const int AC_SRC_OVER = 0x0;

        /// <summary>
        ///     Specifies a raster-operation code. These codes define how the color data for the
        ///     source rectangle is to be combined with the color data for the destination
        ///     rectangle to achieve the final color.
        /// </summary>
        public enum TernaryRasterOperations : uint
        {
            /// <summary>dest = source</summary>
            SRCCOPY = 0xcc0020,

            /// <summary>dest = source OR dest</summary>
            SRCPAINT = 0xee0086,

            /// <summary>dest = source AND dest</summary>
            SRCAND = 0x8800c6,

            /// <summary>dest = source XOR dest</summary>
            SRCINVERT = 0x660046,

            /// <summary>dest = source AND (NOT dest)</summary>
            SRCERASE = 0x440328,

            /// <summary>dest = (NOT source)</summary>
            NOTSRCCOPY = 0x330008,

            /// <summary>dest = (NOT src) AND (NOT dest)</summary>
            NOTSRCERASE = 0x1100a6,

            /// <summary>dest = (source AND pattern)</summary>
            MERGECOPY = 0xc000ca,

            /// <summary>dest = (NOT source) OR dest</summary>
            MERGEPAINT = 0xbb0226,

            /// <summary>dest = pattern</summary>
            PATCOPY = 0xf00021,

            /// <summary>dest = DPSnoo</summary>
            PATPAINT = 0xfb0a09,

            /// <summary>dest = pattern XOR dest</summary>
            PATINVERT = 0x5a0049,

            /// <summary>dest = (NOT dest)</summary>
            DSTINVERT = 0x550009,

            /// <summary>dest = BLACK</summary>
            BLACKNESS = 0x42,

            /// <summary>dest = WHITE</summary>
            WHITENESS = 0xff0062,

            /// <summary>
            /// Capture window as seen on screen.  This includes layered windows
            /// such as WPF windows with AllowsTransparency="true"
            /// </summary>
            CAPTUREBLT = 0x40000000
        }

        [DllImport("gdi32.dll", EntryPoint = "GdiAlphaBlend")]
        public static extern bool AlphaBlend(IntPtr hdcDest, int nXOriginDest, int nYOriginDest, int nWidthDest, int nHeightDest, IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc,
        BLENDFUNCTION blendFunction);

        [DllImport("coredll.dll")]
        public static extern int BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

        [DllImport("gdi32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("Gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);

        [DllImport("gdi32.dll")]
        public static extern bool StretchBlt(IntPtr hdcDest, int nXOriginDest, int nYOriginDest, int nWidthDest, int nHeightDest, IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc,
        TernaryRasterOperations dwRop);

        //
        // currently defined blend operation
        //
        //
        // currently defined alpha format
        //
        [StructLayout(LayoutKind.Sequential)]
        public struct BLENDFUNCTION
        {
            private byte BlendOp;
            private byte BlendFlags;
            private byte SourceConstantAlpha;

            private byte AlphaFormat;

            public BLENDFUNCTION(byte alpha)
                : this(0, 0, alpha, 1)
            {
            }

            public BLENDFUNCTION(byte op, byte flags, byte alpha, byte format)
            {
                BlendOp = op;
                BlendFlags = flags;
                SourceConstantAlpha = alpha;
                AlphaFormat = format;
            }
        }
    }

    public class ManagedGDI
    {
        public static void AlphaBlend(Bitmap srcImage, Graphics dest, Rectangle destRect, byte alpha)
        {
            IntPtr hBmp = srcImage.GetHbitmap();
            IntPtr destDc = dest.GetHdc();

            IntPtr oldObj = default(IntPtr);
            IntPtr memDC = GDI32.CreateCompatibleDC(destDc);
            bool @bool = false;

            oldObj = GDI32.SelectObject(memDC, hBmp);

            var _with1 = destRect;
            @bool = //GDI32.StretchBlt(destDc, _with1.X, _with1.Y, _with1.Width, _with1.Height, memDC, 0, 0, srcImage.Width, srcImage.Height,
                    //GDI32.TernaryRasterOperations.SRCERASE);
                GDI32.AlphaBlend(destDc, _with1.X, _with1.Y, _with1.Width, _with1.Height, memDC, 0, 0, srcImage.Width, srcImage.Height,
            new GDI32.BLENDFUNCTION(alpha));

            //Place stock object back into memory DC
            GDI32.SelectObject(memDC, oldObj);

            GDI32.DeleteDC(memDC);
            dest.ReleaseHdc(destDc);

            GDI32.DeleteObject(hBmp);
        }
    }
}