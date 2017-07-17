using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Threading;
using LX29_Drawing;
using LX29_Primitives;
using LX29_Math;

namespace LX29_Primitives
{
    public struct PolygonX
    {
        public static readonly PolygonX Empty = new PolygonX();
        private List<PointX> orig;
        private List<PointX> points;
        private List<PointX> edges;
        public readonly RectangleX OrigRectangle;

        public bool IsEmpty
        {
            get { return edges.Count == 0; }
        }

        public PolygonX(RectangleX rec)
        {
            points = new List<PointX>();
            edges = new List<PointX>();
            orig = new List<PointX>();
            points.Add(new PointX(rec.X, rec.Y));
            points.Add(new PointX(rec.CenterX, rec.Y));

            points.Add(new PointX(rec.Right, rec.Y));
            points.Add(new PointX(rec.Right, rec.CenterY));

            points.Add(new PointX(rec.Right, rec.Bottom));
            points.Add(new PointX(rec.CenterX, rec.Bottom));

            points.Add(new PointX(rec.X, rec.Bottom));
            points.Add(new PointX(rec.X, rec.CenterY));
            edges = BuildEdges(points);
            orig.AddRange(points.ToArray());
            OrigRectangle = rec;
        }
        public PolygonX(RectangleX rec, bool Circle)
        {
            points = new List<PointX>();
            edges = new List<PointX>();
            orig = new List<PointX>();
            int cnt = 360;
            float pi = (float)(Math.PI / 180.0);
            for (int i = 0; i < cnt; i += 45)
            {
                float x = (float)((rec.Width * 0.5) * Math.Cos(i * pi));
                float y = (float)((rec.Height * 0.5) * Math.Sin(i * pi));
                x += (float)(rec.X + rec.Width * 0.5);
                y += (float)(rec.Y + rec.Height * 0.5);
                points.Add(new PointX(x, y));
            }
            orig.AddRange(points.ToArray());
            edges = BuildEdges(points);
            OrigRectangle = rec;
        }

        public static List<PointX> BuildEdges(List<PointX> points)
        {
            PointX p1;
            PointX p2;
            List<PointX> edges = new List<PointX>();
            for (int i = 0; i < points.Count; i++)
            {
                p1 = points[i];
                if (i + 1 >= points.Count)
                {
                    p2 = points[0];
                }
                else
                {
                    p2 = points[i + 1];
                }
                edges.Add(p2 - p1);
            }
            return edges;
        }

        public bool Contains(float X, float Y)
        {
            bool isInside = false;

            for (int i = 0, j = points.Count - 1; i < points.Count; j = i++)
            {
                if (((points[i].Y > Y) != (points[j].Y > Y)) &&
                (X < (points[j].X - points[i].X) * (Y - points[i].Y) / (points[j].Y - points[i].Y) + points[i].X))
                {
                    return true;
                }
            }
            return isInside;
        }
        public bool Contains(PointX point)
        {
            return Contains(point.X, point.Y);
        }

        public List<PointX> Edges
        {
            get { return edges; }
        }

        public List<PointX> Points
        {
            get { return points; }
            set { points = value; }
        }

        public PointX Center
        {
            get
            {
                float totalX = 0;
                float totalY = 0;
                for (int i = 0; i < points.Count; i++)
                {
                    totalX += points[i].X;
                    totalY += points[i].Y;
                }

                return new PointX(totalX / (float)points.Count, totalY / (float)points.Count);
            }
        }

        public void Offset(PointX v)
        {
            Offset(v.X, v.Y);
        }

        public void Offset(float x, float y)
        {
            for (int i = 0; i < points.Count; i++)
            {
                PointX p = points[i];
                points[i] = new PointX(p.X + x, p.Y + y);
            }
        }
        public PointX[] GetOrigOffset(PointX v)
        {
            PointX[] pa = orig.ToArray();
            for (int i = 0; i < pa.Length; i++)
            {
                pa[i] += v;
            }
            return pa;
        }
        public void OrigOffset(PointX v)
        {
            for (int i = 0; i < orig.Count; i++)
            {
                points[i] = orig[i] + v;
            }
            edges = BuildEdges(points);
        }
        public PolygonX FactorizedOffset(float xi, float yi, float dx, float dy)
        {
            PolygonX pout = new PolygonX(RectangleX.Empty);
            pout.points.Clear();
            for (int i = 0; points.Count > i; i++)
            {
                PointX loc = points[i];
                loc.X = loc.X - xi;
                loc.Y = loc.Y - yi;
                loc.X *= dx;
                loc.Y *= dy;
                pout.points.Add(loc);
            }
            pout.edges = BuildEdges(pout.points);
            return pout;
        }
        public void DrawPolygon(Graphics g)
        {
            List<PointX> pa = points;
            //pa.Sort();
            for (int i = 0; pa.Count - 1 > i; i += 2)
            {
                g.DrawLine(Pens.Orange, (PointF)pa[i], (PointF)pa[i + 1]);
            }
        }

        public override string ToString()
        {
            string result = "";

            for (int i = 0; i < points.Count; i++)
            {
                if (result != "") result += " ";
                result += "{" + points[i].ToString() + "}";
            }

            return result;
        }
    }
    public struct RectangleX
    {
        public static readonly RectangleX Empty = new RectangleX();

        private float x;
        private float y;
        private float width;
        private float height;

        public RectangleX(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public RectangleX(PointX Center, float width, float height)
        {
            this.x = Center.X - width * 0.5f;
            this.y = Center.Y - height * 0.5f;
            this.width = width;
            this.height = height;
        }
        public RectangleX(PointX location, SizeX size)
        {
            this.x = location.X;
            this.y = location.Y;
            this.width = size.Width;
            this.height = size.Height;
        }
        public RectangleX(Point location, Size size)
        {
            this.x = location.X;
            this.y = location.Y;
            this.width = size.Width;
            this.height = size.Height;
        }

        [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
        public static RectangleX FromLTRB(float left, float top, float right, float bottom)
        {
            return new RectangleX(left,
                                 top,
                                 right - left,
                                 bottom - top);
        }

        public PointX Location
        {
            [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
            get
            {
                return new PointX(X, Y);
            }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        public SizeX Size
        {
            [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
            get
            {
                return new SizeX(Width, Height);
            }
            set
            {
                this.Width = value.Width;
                this.Height = value.Height;
            }
        }

        public float X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }

        public float Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }

        public float Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }

        public float Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }

        public float Left
        {
            get
            {
                return x;
            }
        }

        public float Top
        {
            get
            {
                return y;
            }
        }

        public float Right
        {
            [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
            get
            {
                return x + width;
            }
        }

        public float Bottom
        {
            [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
            get
            {
                return y + height;
            }
        }

        public float Diagonale
        {
            get { return (float)Math.Sqrt(width * width + height * height); }
        }
        public bool IsEmpty
        {
            get
            {
                return (Width <= 0) || (Height <= 0);
            }
        }

        public PointX[] Edges
        {
            get
            {
                PointX[] pa = new PointX[4];
                pa[0] = new PointX(x, y);
                pa[1] = new PointX(Right, y);
                pa[2] = new PointX(Right, Bottom);
                pa[3] = new PointX(x, Bottom);
                return pa;
            }
        }
        public PointX[] InnerCircle
        {
            get
            {
                PointX[] pa = new PointX[360 / 10];
                int cnt = 360;
                float pi = (float)(Math.PI / 180.0);
                for (int i = 0; i < cnt; i += 10)
                {
                    float xi = (float)((width * 0.5) * Math.Cos(i * pi));
                    float yi = (float)((height * 0.5) * Math.Sin(i * pi));
                    xi += x + width * 0.5f;
                    yi += y + height * 0.5f;
                    pa[(int)(i * 0.1)] = new PointX(xi, yi);
                }
                return pa;
            }
        }
        public LineX[] Lines
        {
            get
            {
                LineX[] pa = new LineX[4];
                pa[0] = new LineX(x, y, Right, y);
                pa[1] = new LineX(Right, y, Right, Bottom);
                pa[2] = new LineX(Right, Bottom, x, Bottom);
                pa[3] = new LineX(x, Bottom, x, y);
                return pa;
            }
        }
        public PointX Center
        {
            get { return new PointX(x + width * 0.5f, y + height * 0.5f); }
        }
        public float CenterX
        {
            get { return x + width * 0.5f; }
        }
        public float CenterY
        {
            get { return y + height * 0.5f; }
        }
        /// <summary>
        /// Gets the Distance of two Rectangles from the Center + (Width+Height)*0.25
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public float DistanceTo(RectangleX rec)
        {
            float d = (float)Math.Sqrt(
                Math.Pow(rec.CenterX - this.CenterX, 2) + Math.Pow(rec.CenterY - this.CenterY, 2));
            d -= Math.Max(rec.width, rec.height) + Math.Max(width, height);
            return Math.Max(0, d);
        }
        public float DistanceTo(PointX p)
        {
            float d = (float)Math.Sqrt(
                Math.Pow(p.X - this.CenterX, 2) + Math.Pow(p.Y - this.CenterY, 2));
            d -= Math.Max(width, height);
            return Math.Max(0, d);
        }
        public void Multiply(float fac)
        {
            x -= width * fac;
            y -= height * fac;
            width += (width * 2.0f) * fac;
            height += (height * 2.0f) * fac;
        }
        public RectangleX ToScreen(float dx, float dy, float camX, float camY)
        {
            return new RectangleX(
            (x - camX) * dx,
            (y - camY) * dy,
            width * dx,
            height * dy);
        }
        public RectangleX FactorizedOffset(float xi, float yi, float dx, float dy)
        {
            RectangleX rec = new RectangleX(x, y, width, height);
            rec.x = rec.X - xi;
            rec.y = rec.Y - yi;
            rec.x *= dx;
            rec.y *= dy;
            rec.width *= dx;
            rec.height *= dx;
            return rec;
        }
        public override bool Equals(object obj)
        {
            if (!(obj is RectangleX))
                return false;

            RectangleX comp = (RectangleX)obj;

            return (comp.X == this.X) &&
                   (comp.Y == this.Y) &&
                   (comp.Width == this.Width) &&
                   (comp.Height == this.Height);
        }

        public static RectangleX operator *(RectangleX left, float right)
        {
            return new RectangleX(left.X * right,
                      left.Y * right,
                     left.Width * right,
                      left.Height * right);
        }
        public static bool operator ==(RectangleX left, RectangleX right)
        {
            return (left.X == right.X
                     && left.Y == right.Y
                     && left.Width == right.Width
                     && left.Height == right.Height);
        }

        public static bool operator !=(RectangleX left, RectangleX right)
        {
            return !(left == right);
        }

        public bool Contains(float x, float y)
        {
            return this.X <= x &&
            x < this.X + this.Width &&
            this.Y <= y &&
            y < this.Y + this.Height;
        }

        public bool Contains(PointX pt)
        {
            return Contains(pt.X, pt.Y);
        }

        public bool Contains(RectangleX rect)
        {
            return (this.X <= rect.X) &&
                   ((rect.X + rect.Width) <= (this.X + this.Width)) &&
                   (this.Y <= rect.Y) &&
                   ((rect.Y + rect.Height) <= (this.Y + this.Height));
        }

        public override int GetHashCode()
        {
            return unchecked((int)((UInt32)X ^
            (((UInt32)Y << 13) | ((UInt32)Y >> 19)) ^
            (((UInt32)Width << 26) | ((UInt32)Width >> 6)) ^
            (((UInt32)Height << 7) | ((UInt32)Height >> 25))));
        }

        public void Inflate(float x, float y)
        {
            this.X -= x;
            this.Y -= y;
            this.Width += 2.0f * x;
            this.Height += 2.0f * y;
        }

        public void Inflate(SizeX size)
        {
            Inflate(size.Width, size.Height);
        }

        public static RectangleX Inflate(RectangleX rect, float x, float y)
        {
            RectangleX r = rect;
            r.Inflate(x, y);
            return r;
        }

        public void Intersect(RectangleX rect)
        {
            RectangleX result = RectangleX.Intersect(rect, this);

            this.X = result.X;
            this.Y = result.Y;
            this.Width = result.Width;
            this.Height = result.Height;
        }

        public static RectangleX Intersect(RectangleX a, RectangleX b)
        {
            float x1 = Math.Max(a.X, b.X);
            float x2 = Math.Min(a.X + a.Width, b.X + b.Width);
            float y1 = Math.Max(a.Y, b.Y);
            float y2 = Math.Min(a.Y + a.Height, b.Y + b.Height);

            if (x2 >= x1 && y2 >= y1)
            {
                return new RectangleX(x1, y1, x2 - x1, y2 - y1);
            }
            return RectangleX.Empty;
        }
        public static RectangleX Unstuck(RectangleX a, RectangleX b)
        {
            float x1 = Math.Max(a.X, b.X);
            float x2 = Math.Min(a.X + a.Width, b.X + b.Width);
            float y1 = Math.Max(a.Y, b.Y);
            float y2 = Math.Min(a.Y + a.Height, b.Y + b.Height);

            if (x2 >= x1 && y2 >= y1)
            {
                return new RectangleX(a.Right, a.Bottom, b.width, b.height);
            }
            return RectangleX.Empty;
        }
        public bool IntersectsWith(RectangleX rect)
        {
            return (rect.x < x + width) &&
                   (x < (rect.x + rect.width)) &&
                   (rect.y < y + height) &&
                   (this.y < rect.y + rect.height);
        }

        public static RectangleX Union(RectangleX a, RectangleX b)
        {
            float x1 = Math.Min(a.X, b.X);
            float x2 = Math.Max(a.X + a.Width, b.X + b.Width);
            float y1 = Math.Min(a.Y, b.Y);
            float y2 = Math.Max(a.Y + a.Height, b.Y + b.Height);

            return new RectangleX(x1, y1, x2 - x1, y2 - y1);
        }
        public RectangleF RectangleF
        {
            get { return new RectangleF((float)x, (float)y, (float)width, (float)height); }
        }
        public Rectangle Rectangle
        {
            get { return new Rectangle((int)x, (int)y, (int)width, (int)height); }
        }
        public void Offset(PointX pos)
        {
            Offset(pos.X, pos.Y);
        }

        public void Offset(float x, float y)
        {
            this.X += x;
            this.Y += y;
        }

        public static implicit operator RectangleX(Rectangle r)
        {
            return new RectangleX(r.X, r.Y, r.Width, r.Height);
        }
        public static implicit operator RectangleX(RectangleF r)
        {
            return new RectangleX(r.X, r.Y, r.Width, r.Height);
        }

    }
    public struct SizeX
    {
        public static readonly SizeX Empty = new SizeX();
        private float width;
        private float height;

        public SizeX(SizeX size)
        {
            width = size.width;
            height = size.height;
        }

        public SizeX(PointX pt)
        {
            width = pt.X;
            height = pt.Y;
        }

        public SizeX(float width, float height)
        {
            this.width = width;
            this.height = height;
        }

        #region Operators
        public static SizeX operator *(SizeX sz1, SizeX sz2)
        {
            return Add(sz1, sz2);
        }
        public static SizeX operator *(SizeX sz1, float sz2)
        {
            return Multiply(sz1, sz2);
        }
        public static SizeX operator +(SizeX sz1, SizeX sz2)
        {
            return Add(sz1, sz2);
        }

        public static SizeX operator -(SizeX sz1, SizeX sz2)
        {
            return Subtract(sz1, sz2);
        }

        public static bool operator ==(SizeX sz1, SizeX sz2)
        {
            return sz1.Width == sz2.Width && sz1.Height == sz2.Height;
        }

        public static bool operator !=(SizeX sz1, SizeX sz2)
        {
            return !(sz1 == sz2);
        }

        public static explicit operator PointX(SizeX size)
        {
            return new PointX(size.Width, size.Height);
        }

        public static implicit operator SizeX(SizeF size)
        {
            return new SizeX(size.Width, size.Height);
        }

        public static SizeX Multiply(SizeX sz1, SizeX sz2)
        {
            return new SizeX(sz1.Width * sz2.Width, sz1.Height * sz2.Height);
        }
        public static SizeX Multiply(SizeX sz1, float sz2)
        {
            return new SizeX(sz1.Width * sz2, sz1.Height * sz2);
        }
        public static SizeX Multiply(SizeX sz1, float sz2X, float sz2Y)
        {
            return new SizeX(sz1.Width * sz2X, sz1.Height * sz2Y);
        }
        public static SizeX Add(SizeX sz1, SizeX sz2)
        {
            return new SizeX(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
        }
        public static SizeX Subtract(SizeX sz1, SizeX sz2)
        {
            return new SizeX(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
        }
        #endregion

        public bool IsEmpty
        {
            [System.Runtime.TargetedPatchingOptOutAttribute("Performance critical to inline across NGen image boundaries")]
            get
            {
                return width == 0 && height == 0;
            }
        }

        public float Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }

        public float Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is SizeX))
                return false;

            SizeX comp = (SizeX)obj;

            return (comp.Width == this.Width) &&
            (comp.Height == this.Height) &&
            (comp.GetType().Equals(GetType()));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public PointX ToPointD()
        {
            return (PointX)this;
        }

        public Size ToSize()
        {
            return Size.Truncate(SizeF);
        }

        public SizeF SizeF
        {
            get { return new SizeF((float)width, (float)height); }
        }

        public static bool operator >(SizeX left, SizeX right)
        {
            return (left.width > right.width && left.height > right.height);
        }
        public static bool operator <(SizeX left, SizeX right)
        {
            return (left.width < right.width && left.height < right.height);
        }
        public static SizeX operator +(SizeX left, float right)
        {
            return new SizeX((left.width + right), left.height + right);
        }
    }
    /// <summary>
    /// Converted from Java by Lukix29
    /// </summary>
    public struct LineX
    {
        public static readonly LineX Empty = new LineX();
        public bool IsEmpty
        {
            get
            {
                return (x1 == 0.0 && y1 == 0.0 && x2 == 0.0 && y2 == 0.0);
            }
        }
        #region Privates
        private float x1;
        private float y1;
        private float x2;
        private float y2;
        private void setValues(float x1, float y1, float x2, float y2)
        {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
        }
        private static float area2(float x1, float y1, float x2, float y2, float x3, float y3)
        {
            return (x2 - x1) * (y3 - y1) - (x3 - x1) * (y2 - y1);
        }
        private static bool between(float x1, float y1, float x2, float y2, float x3, float y3)
        {
            if (x1 != x2)
            {
                return (x1 <= x3 && x3 <= x2) || (x1 >= x3 && x3 >= x2);
            }
            else
            {
                return (y1 <= y3 && y3 <= y2) || (y1 >= y3 && y3 >= y2);
            }
        }
        private static bool lineIntersect(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {
            float a1, a2, a3, a4;

            // deal with special cases
            if ((a1 = area2(x1, y1, x2, y2, x3, y3)) == 0.0)
            {
                // check if p3 is between p1 and p2 OR
                // p4 is collinear also AND either between p1 and p2 OR at opposite ends
                if (between(x1, y1, x2, y2, x3, y3))
                {
                    return true;
                }
                else
                {
                    if (area2(x1, y1, x2, y2, x4, y4) == 0.0)
                    {
                        return between(x3, y3, x4, y4, x1, y1)
                               || between(x3, y3, x4, y4, x2, y2);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else if ((a2 = area2(x1, y1, x2, y2, x4, y4)) == 0.0)
            {
                // check if p4 is between p1 and p2 (we already know p3 is not
                // collinear)
                return between(x1, y1, x2, y2, x4, y4);
            }

            if ((a3 = area2(x3, y3, x4, y4, x1, y1)) == 0.0)
            {
                // check if p1 is between p3 and p4 OR
                // p2 is collinear also AND either between p1 and p2 OR at opposite ends
                if (between(x3, y3, x4, y4, x1, y1))
                {
                    return true;
                }
                else
                {
                    if (area2(x3, y3, x4, y4, x2, y2) == 0.0)
                    {
                        return between(x1, y1, x2, y2, x3, y3)
                               || between(x1, y1, x2, y2, x4, y4);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else if ((a4 = area2(x3, y3, x4, y4, x2, y2)) == 0.0)
            {
                // check if p2 is between p3 and p4 (we already know p1 is not
                // collinear)
                return between(x3, y3, x4, y4, x2, y2);
            }
            else
            {  // test for regular intersection
                return ((a1 > 0.0) ^ (a2 > 0.0)) && ((a3 > 0.0) ^ (a4 > 0.0));
            }
        }
        private static float lineDistSq(float x1, float y1, float x2, float y2, float px, float py)
        {
            float pd2 = (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);

            float x, y;
            if (pd2 == 0)
            {
                // Points are coincident.
                x = x1;
                y = y2;
            }
            else
            {
                float u = ((px - x1) * (x2 - x1) + (py - y1) * (y2 - y1)) / pd2;
                x = x1 + u * (x2 - x1);
                y = y1 + u * (y2 - y1);
            }

            return (x - px) * (x - px) + (y - py) * (y - py);
        }
        private static float lineDist(float x1, float y1, float x2, float y2, float px, float py)
        {
            return (float)Math.Sqrt(lineDistSq(x1, y1, x2, y2, px, py));
        }
        #endregion

        #region Geom-Functions
        public bool IntersectsLine(float x1, float y1, float x2, float y2)
        {
            return lineIntersect(x1, y1, x2, y2,
                                  x1, y1, x2, y2);
        }
        public bool IntersectsLine(LineX l)
        {
            return lineIntersect(x1, y1, x2, y2,
                                  l.x1, l.y1, l.x2, l.y2);
        }

        public float DistanceTo(float px, float py)
        {
            return lineDist(x1, y1, x2, y2, px, py);
        }
        public float DistanceTo(PointX p)
        {
            return lineDist(x1, y1, x2, y2, p.X, p.Y);
        }

        public bool Intersects(float x, float y, float w, float h)
        {
            if (w <= 0 || h <= 0)
                return false;
            float x1 = this.x1;
            float y1 = this.y1;
            float x2 = this.x2;
            float y2 = this.y2;

            if (x1 >= x && x1 <= x + w && y1 >= y && y1 <= y + h)
                return true;
            if (x2 >= x && x2 <= x + w && y2 >= y && y2 <= y + h)
                return true;

            float x3 = x + w;
            float y3 = y + h;

            return (lineIntersect(x1, y1, x2, y2, x, y, x, y3)
                    || lineIntersect(x1, y1, x2, y2, x, y3, x3, y3)
                    || lineIntersect(x1, y1, x2, y2, x3, y3, x3, y)
                    || lineIntersect(x1, y1, x2, y2, x3, y, x, y));
        }
        public bool Intersects(RectangleX r)
        {
            if (r.Contains(P2)) return true;
            return Intersects(r.X, r.Y, r.Width, r.Height);
        }
        public PointX Intersect(LineX l)
        {
            PointX intersection = new PointX(float.PositiveInfinity, float.PositiveInfinity);
            PointX b = P2 - P1;
            PointX d = l.P2 - l.P1;
            float bDotDPerp = b.X * d.Y - b.Y * d.X;

            // if b dot d == 0, it means the lines are parallel so have infinite intersection points
            if (bDotDPerp == 0)
                return intersection;

            PointX c = l.P1 - P1;
            float t = (c.X * d.Y - c.Y * d.X) / bDotDPerp;
            if (t < 0 || t > 1)
                return intersection;

            float u = (c.X * b.Y - c.Y * b.X) / bDotDPerp;
            if (u < 0 || u > 1)
                return intersection;

            intersection = P1 + (b * t);

            return intersection;
        }
        public PointX Intersect(RectangleX r)
        {
            return Intersect(r, 0);
        }
        public PointX Intersect(RectangleX r, int idx)
        {
            LineX[] pa = r.Lines;
            idx = Math.Max(0, Math.Min(pa.Length, idx));
            PointX pout = PointX.Empty;
            for (int i = idx; pa.Length > i; i++)
            {
                pout = Intersect(pa[i]);
                if (!pout.IsEmpty)
                {
                    return pout;
                }
            }
            return PointX.Empty;
        }
        public PointX Intersect(PolygonX r)
        {
            PointX pout = PointX.Empty;
            for (int i = 0; r.Points.Count - 1 > i; i++)
            {
                LineX l = new LineX(r.Points[i], r.Points[i + 1]);
                pout = Intersect(l);
                if (!pout.IsEmpty)
                {
                    return pout;
                }
            }
            return PointX.Empty;
        }
        public PointX Intersect(PointX[] r)
        {
            PointX pout = PointX.Empty;
            for (int i = 0; r.Length - 1 > i; i++)
            {
                LineX l = new LineX(r[i], r[i + 1]);
                pout = Intersect(l);
                if (!pout.IsEmpty)
                {
                    return pout;
                }
            }
            return PointX.Empty;
        }
        public PointX IntersectsCircle(RectangleX rec)
        {
            int cnt = 360;
            float pi = (float)(Math.PI / 180.0);
            PointX pout = PointX.Empty;
            float xo = 0;
            float yo = 0;
            for (int i = 0; i < cnt; i += 10)
            {
                float xi = (float)((rec.Width * 0.5) * Math.Cos(i * pi));
                float yi = (float)((rec.Height * 0.5) * Math.Sin(i * pi));
                xi += rec.X + rec.Width * 0.5f;
                yi += rec.Y + rec.Height * 0.5f;
                if (i > 0)
                {
                    LineX l = new LineX(xo, yo, xi, yi);
                    pout = Intersect(l);
                    if (!pout.IsEmpty)
                    {
                        return pout;
                    }
                }
                xo = xi;
                yo = yi;
            }
            return PointX.Empty;
        }
        public bool IntersectCircle(RectangleX rec)
        {
            return !IntersectsCircle(rec).IsEmpty;
        }
        #endregion

        public LineX(float x1, float y1, float x2, float y2)
        {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
        }
        public LineX(PointX p1, PointX p2)
        {
            x1 = p1.X;
            y1 = p1.Y;
            x2 = p2.X;
            y2 = p2.Y;
        }


        public RectangleX Bounds
        {
            get
            {
                float x = Math.Min(x1, x2);
                float y = Math.Min(y1, y2);
                float w = Math.Abs(x1 - x2);
                float h = Math.Abs(y1 - y2);
                return new RectangleX(x, y, w, h);
            }
        }
        public float Length
        {
            get
            {
                return P1.DistanceTo(x2, y2);
            }
        }

        public float X1
        {
            get { return x1; }
            set { x1 = value; }
        }
        public float Y1
        {
            get { return x1; }
            set { x1 = value; }
        }

        public float X2
        {
            get { return x2; }
            set { x2 = value; ;}
        }
        public float Y2
        {
            get { return y2; }
            set { y2 = value; }
        }

        public PointX P2
        {
            get { return new PointX(x2, y2); }
        }
        public PointX P1
        {
            get { return new PointX(x1, y1); }
        }
    }
    public struct PointX
    {
        public static readonly PointX Empty = new PointX();
        private float x;
        private float y;
        public PointX(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public bool IsEmpty
        {
            get
            {
                return x == 0f && y == 0f;
            }
        }

        public float X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }

        public float Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }

        public float Magnitude
        {
            get { return (float)Math.Sqrt(X * X + Y * Y); }
        }

        public void Normalize()
        {
            float magnitude = Magnitude;
            X = X / magnitude;
            Y = Y / magnitude;
        }

        public void MinMax(float min, float max)
        {
            X = Math.Max(min, Math.Min(X, max));
            Y = Math.Max(min, Math.Min(Y, max));
        }
        public PointX NewMinMax(float min, float max)
        {
            return new PointX(Math.Max(min, Math.Min(X, max)),
             X = Math.Max(min, Math.Min(X, max)));
        }
        public void Ceiling()
        {
            X = (float)(Math.Ceiling(X) * Math.Sign(X));
            Y = (float)(Math.Ceiling(Y) * Math.Sign(Y));
        }
        public PointX FactorizedOffset(float xi, float yi, float dx, float dy)
        {
            PointX rec = new PointX(x, y);
            rec.x = rec.X - xi;
            rec.y = rec.Y - yi;
            rec.x *= dx;
            rec.y *= dy;
            return rec;
        }
        public PointX GetNormalized()
        {
            float magnitude = Magnitude;

            return new PointX(X / magnitude, Y / magnitude);
        }

        public float DotProduct(PointX vector)
        {
            return this.X * vector.X + this.Y * vector.Y;
        }

        public float DistanceTo(PointX vector)
        {
            return (float)Math.Sqrt(Math.Pow(vector.X - this.X, 2) + Math.Pow(vector.Y - this.Y, 2));
        }
        public float DistanceTo(float x, float y)
        {
            return (float)Math.Sqrt(Math.Pow(x - this.X, 2) + Math.Pow(y - this.Y, 2));
        }

        #region Operators
        public static implicit operator Point(PointX p)
        {
            return new Point((int)p.X, (int)p.Y);
        }

        public static implicit operator PointF(PointX p)
        {
            return new PointF((float)p.X, (float)p.Y);
        }
        public static PointX operator +(PointX a, PointX b)
        {
            return new PointX(a.X + b.X, a.Y + b.Y);
        }

        public static PointX operator -(PointX a)
        {
            return new PointX(-a.X, -a.Y);
        }

        public static PointX operator -(PointX a, PointX b)
        {
            return new PointX(a.X - b.X, a.Y - b.Y);
        }

        public static PointX operator *(PointX a, float b)
        {
            return new PointX(a.X * b, a.Y * b);
        }
        public static PointX operator -(PointX a, float b)
        {
            return new PointX(a.X - b, a.Y - b);
        }

        public static PointX operator *(PointX a, int b)
        {
            return new PointX(a.X * b, a.Y * b);
        }
        public static PointX operator *(PointX a, PointX b)
        {
            return new PointX(a.x * b.x, a.y * b.y);
        }

        public static PointX operator +(PointX pt, SizeX sz)
        {
            return Add(pt, sz);
        }

        public static PointX operator +(PointX pt, float d)
        {
            return new PointX(pt.X + d, pt.Y + d);
        }
        public static PointX operator -(PointX pt, SizeX sz)
        {
            return Subtract(pt, sz);
        }

        public static bool operator ==(PointX left, PointX right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        public static bool operator !=(PointX left, PointX right)
        {
            return !(left == right);
        }
        public static bool operator <(PointX left, PointX right)
        {
            return (left.x < right.x && left.y < right.y);
        }
        public static bool operator >(PointX left, PointX right)
        {
            return (left.x > right.x && left.y > right.y);
        }

        public static PointX Add(PointX pt, SizeX sz)
        {
            return new PointX(pt.X + sz.Width, pt.Y + sz.Height);
        }

        public static PointX Subtract(PointX pt, SizeX sz)
        {
            return new PointX(pt.X - sz.Width, pt.Y - sz.Height);
        }

        public static PointX Add(PointX pt, PointX sz)
        {
            return new PointX(pt.X + sz.X, pt.Y + sz.Y);
        }

        public static PointX Subtract(PointX pt, PointX sz)
        {
            return new PointX(pt.X - sz.X, pt.Y - sz.Y);
        }

        public static implicit operator PointX(Point r)
        {
            return new PointX(r.X, r.Y);
        }

        public static implicit operator PointX(PointF r)
        {
            return new PointX(r.X, r.Y);
        }
        #endregion

        public PointF PointF
        {
            get { return new PointF((float)x, (float)y); }
        }
        public Point Point
        {
            get { return Point.Truncate(PointF); }
        }
        public override bool Equals(object obj)
        {
            if (!(obj is PointX)) return false;
            PointX comp = (PointX)obj;
            return
            comp.X == this.X &&
            comp.Y == this.Y &&
            comp.GetType().Equals(this.GetType());
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

namespace LX29_Drawing
{
    public struct ColorX
    {
        #region Colors
        public readonly static ColorX ActiveBorder = new ColorX(255, 180, 180, 180);
        public readonly static ColorX ActiveCaption = new ColorX(255, 153, 180, 209);
        public readonly static ColorX ActiveCaptionText = new ColorX(255, 0, 0, 0);
        public readonly static ColorX AppWorkspace = new ColorX(255, 171, 171, 171);
        public readonly static ColorX Control = new ColorX(255, 240, 240, 240);
        public readonly static ColorX ControlDark = new ColorX(255, 160, 160, 160);
        public readonly static ColorX ControlDarkDark = new ColorX(255, 105, 105, 105);
        public readonly static ColorX ControlLight = new ColorX(255, 227, 227, 227);
        public readonly static ColorX ControlLightLight = new ColorX(255, 255, 255, 255);
        public readonly static ColorX ControlText = new ColorX(255, 0, 0, 0);
        public readonly static ColorX Desktop = new ColorX(255, 0, 0, 0);
        public readonly static ColorX GrayText = new ColorX(255, 109, 109, 109);
        public readonly static ColorX Highlight = new ColorX(255, 51, 153, 255);
        public readonly static ColorX HighlightText = new ColorX(255, 255, 255, 255);
        public readonly static ColorX HotTrack = new ColorX(255, 0, 102, 204);
        public readonly static ColorX InactiveBorder = new ColorX(255, 244, 247, 252);
        public readonly static ColorX InactiveCaption = new ColorX(255, 191, 205, 219);
        public readonly static ColorX InactiveCaptionText = new ColorX(255, 67, 78, 84);
        public readonly static ColorX Info = new ColorX(255, 255, 255, 225);
        public readonly static ColorX InfoText = new ColorX(255, 0, 0, 0);
        public readonly static ColorX Menu = new ColorX(255, 240, 240, 240);
        public readonly static ColorX MenuText = new ColorX(255, 0, 0, 0);
        public readonly static ColorX ScrollBar = new ColorX(255, 200, 200, 200);
        public readonly static ColorX Window = new ColorX(255, 255, 255, 255);
        public readonly static ColorX WindowFrame = new ColorX(255, 100, 100, 100);
        public readonly static ColorX WindowText = new ColorX(255, 0, 0, 0);
        public readonly static ColorX Transparent = new ColorX(0, 255, 255, 255);
        public readonly static ColorX AliceBlue = new ColorX(255, 240, 248, 255);
        public readonly static ColorX AntiqueWhite = new ColorX(255, 250, 235, 215);
        public readonly static ColorX Aqua = new ColorX(255, 0, 255, 255);
        public readonly static ColorX Aquamarine = new ColorX(255, 127, 255, 212);
        public readonly static ColorX Azure = new ColorX(255, 240, 255, 255);
        public readonly static ColorX Beige = new ColorX(255, 245, 245, 220);
        public readonly static ColorX Bisque = new ColorX(255, 255, 228, 196);
        public readonly static ColorX Black = new ColorX(255, 0, 0, 0);
        public readonly static ColorX BlanchedAlmond = new ColorX(255, 255, 235, 205);
        public readonly static ColorX Blue = new ColorX(255, 0, 0, 255);
        public readonly static ColorX BlueViolet = new ColorX(255, 138, 43, 226);
        public readonly static ColorX Brown = new ColorX(255, 165, 42, 42);
        public readonly static ColorX BurlyWood = new ColorX(255, 222, 184, 135);
        public readonly static ColorX CadetBlue = new ColorX(255, 95, 158, 160);
        public readonly static ColorX Chartreuse = new ColorX(255, 127, 255, 0);
        public readonly static ColorX Chocolate = new ColorX(255, 210, 105, 30);
        public readonly static ColorX Coral = new ColorX(255, 255, 127, 80);
        public readonly static ColorX CornflowerBlue = new ColorX(255, 100, 149, 237);
        public readonly static ColorX Cornsilk = new ColorX(255, 255, 248, 220);
        public readonly static ColorX Crimson = new ColorX(255, 220, 20, 60);
        public readonly static ColorX Cyan = new ColorX(255, 0, 255, 255);
        public readonly static ColorX DarkBlue = new ColorX(255, 0, 0, 139);
        public readonly static ColorX DarkCyan = new ColorX(255, 0, 139, 139);
        public readonly static ColorX DarkGoldenrod = new ColorX(255, 184, 134, 11);
        public readonly static ColorX DarkGray = new ColorX(255, 169, 169, 169);
        public readonly static ColorX DarkGreen = new ColorX(255, 0, 100, 0);
        public readonly static ColorX DarkKhaki = new ColorX(255, 189, 183, 107);
        public readonly static ColorX DarkMagenta = new ColorX(255, 139, 0, 139);
        public readonly static ColorX DarkOliveGreen = new ColorX(255, 85, 107, 47);
        public readonly static ColorX DarkOrange = new ColorX(255, 255, 140, 0);
        public readonly static ColorX DarkOrchid = new ColorX(255, 153, 50, 204);
        public readonly static ColorX DarkRed = new ColorX(255, 139, 0, 0);
        public readonly static ColorX DarkSalmon = new ColorX(255, 233, 150, 122);
        public readonly static ColorX DarkSeaGreen = new ColorX(255, 143, 188, 139);
        public readonly static ColorX DarkSlateBlue = new ColorX(255, 72, 61, 139);
        public readonly static ColorX DarkSlateGray = new ColorX(255, 47, 79, 79);
        public readonly static ColorX DarkTurquoise = new ColorX(255, 0, 206, 209);
        public readonly static ColorX DarkViolet = new ColorX(255, 148, 0, 211);
        public readonly static ColorX DeepPink = new ColorX(255, 255, 20, 147);
        public readonly static ColorX DeepSkyBlue = new ColorX(255, 0, 191, 255);
        public readonly static ColorX DimGray = new ColorX(255, 105, 105, 105);
        public readonly static ColorX DodgerBlue = new ColorX(255, 30, 144, 255);
        public readonly static ColorX Firebrick = new ColorX(255, 178, 34, 34);
        public readonly static ColorX FloralWhite = new ColorX(255, 255, 250, 240);
        public readonly static ColorX ForestGreen = new ColorX(255, 34, 139, 34);
        public readonly static ColorX Fuchsia = new ColorX(255, 255, 0, 255);
        public readonly static ColorX Gainsboro = new ColorX(255, 220, 220, 220);
        public readonly static ColorX GhostWhite = new ColorX(255, 248, 248, 255);
        public readonly static ColorX Gold = new ColorX(255, 255, 215, 0);
        public readonly static ColorX Goldenrod = new ColorX(255, 218, 165, 32);
        public readonly static ColorX Gray = new ColorX(255, 128, 128, 128);
        public readonly static ColorX Green = new ColorX(255, 0, 128, 0);
        public readonly static ColorX GreenYellow = new ColorX(255, 173, 255, 47);
        public readonly static ColorX Honeydew = new ColorX(255, 240, 255, 240);
        public readonly static ColorX HotPink = new ColorX(255, 255, 105, 180);
        public readonly static ColorX IndianRed = new ColorX(255, 205, 92, 92);
        public readonly static ColorX Indigo = new ColorX(255, 75, 0, 130);
        public readonly static ColorX Ivory = new ColorX(255, 255, 255, 240);
        public readonly static ColorX Khaki = new ColorX(255, 240, 230, 140);
        public readonly static ColorX Lavender = new ColorX(255, 230, 230, 250);
        public readonly static ColorX LavenderBlush = new ColorX(255, 255, 240, 245);
        public readonly static ColorX LawnGreen = new ColorX(255, 124, 252, 0);
        public readonly static ColorX LemonChiffon = new ColorX(255, 255, 250, 205);
        public readonly static ColorX LightBlue = new ColorX(255, 173, 216, 230);
        public readonly static ColorX LightCoral = new ColorX(255, 240, 128, 128);
        public readonly static ColorX LightCyan = new ColorX(255, 224, 255, 255);
        public readonly static ColorX LightGoldenrodYellow = new ColorX(255, 250, 250, 210);
        public readonly static ColorX LightGray = new ColorX(255, 211, 211, 211);
        public readonly static ColorX LightGreen = new ColorX(255, 144, 238, 144);
        public readonly static ColorX LightPink = new ColorX(255, 255, 182, 193);
        public readonly static ColorX LightSalmon = new ColorX(255, 255, 160, 122);
        public readonly static ColorX LightSeaGreen = new ColorX(255, 32, 178, 170);
        public readonly static ColorX LightSkyBlue = new ColorX(255, 135, 206, 250);
        public readonly static ColorX LightSlateGray = new ColorX(255, 119, 136, 153);
        public readonly static ColorX LightSteelBlue = new ColorX(255, 176, 196, 222);
        public readonly static ColorX LightYellow = new ColorX(255, 255, 255, 224);
        public readonly static ColorX Lime = new ColorX(255, 0, 255, 0);
        public readonly static ColorX LimeGreen = new ColorX(255, 50, 205, 50);
        public readonly static ColorX Linen = new ColorX(255, 250, 240, 230);
        public readonly static ColorX Magenta = new ColorX(255, 255, 0, 255);
        public readonly static ColorX Maroon = new ColorX(255, 128, 0, 0);
        public readonly static ColorX MediumAquamarine = new ColorX(255, 102, 205, 170);
        public readonly static ColorX MediumBlue = new ColorX(255, 0, 0, 205);
        public readonly static ColorX MediumOrchid = new ColorX(255, 186, 85, 211);
        public readonly static ColorX MediumPurple = new ColorX(255, 147, 112, 219);
        public readonly static ColorX MediumSeaGreen = new ColorX(255, 60, 179, 113);
        public readonly static ColorX MediumSlateBlue = new ColorX(255, 123, 104, 238);
        public readonly static ColorX MediumSpringGreen = new ColorX(255, 0, 250, 154);
        public readonly static ColorX MediumTurquoise = new ColorX(255, 72, 209, 204);
        public readonly static ColorX MediumVioletRed = new ColorX(255, 199, 21, 133);
        public readonly static ColorX MidnightBlue = new ColorX(255, 25, 25, 112);
        public readonly static ColorX MintCream = new ColorX(255, 245, 255, 250);
        public readonly static ColorX MistyRose = new ColorX(255, 255, 228, 225);
        public readonly static ColorX Moccasin = new ColorX(255, 255, 228, 181);
        public readonly static ColorX NavajoWhite = new ColorX(255, 255, 222, 173);
        public readonly static ColorX Navy = new ColorX(255, 0, 0, 128);
        public readonly static ColorX OldLace = new ColorX(255, 253, 245, 230);
        public readonly static ColorX Olive = new ColorX(255, 128, 128, 0);
        public readonly static ColorX OliveDrab = new ColorX(255, 107, 142, 35);
        public readonly static ColorX Orange = new ColorX(255, 255, 165, 0);
        public readonly static ColorX OrangeRed = new ColorX(255, 255, 69, 0);
        public readonly static ColorX Orchid = new ColorX(255, 218, 112, 214);
        public readonly static ColorX PaleGoldenrod = new ColorX(255, 238, 232, 170);
        public readonly static ColorX PaleGreen = new ColorX(255, 152, 251, 152);
        public readonly static ColorX PaleTurquoise = new ColorX(255, 175, 238, 238);
        public readonly static ColorX PaleVioletRed = new ColorX(255, 219, 112, 147);
        public readonly static ColorX PapayaWhip = new ColorX(255, 255, 239, 213);
        public readonly static ColorX PeachPuff = new ColorX(255, 255, 218, 185);
        public readonly static ColorX Peru = new ColorX(255, 205, 133, 63);
        public readonly static ColorX Pink = new ColorX(255, 255, 192, 203);
        public readonly static ColorX Plum = new ColorX(255, 221, 160, 221);
        public readonly static ColorX PowderBlue = new ColorX(255, 176, 224, 230);
        public readonly static ColorX Purple = new ColorX(255, 128, 0, 128);
        public readonly static ColorX Red = new ColorX(255, 255, 0, 0);
        public readonly static ColorX RosyBrown = new ColorX(255, 188, 143, 143);
        public readonly static ColorX RoyalBlue = new ColorX(255, 65, 105, 225);
        public readonly static ColorX SaddleBrown = new ColorX(255, 139, 69, 19);
        public readonly static ColorX Salmon = new ColorX(255, 250, 128, 114);
        public readonly static ColorX SandyBrown = new ColorX(255, 244, 164, 96);
        public readonly static ColorX SeaGreen = new ColorX(255, 46, 139, 87);
        public readonly static ColorX SeaShell = new ColorX(255, 255, 245, 238);
        public readonly static ColorX Sienna = new ColorX(255, 160, 82, 45);
        public readonly static ColorX Silver = new ColorX(255, 192, 192, 192);
        public readonly static ColorX SkyBlue = new ColorX(255, 135, 206, 235);
        public readonly static ColorX SlateBlue = new ColorX(255, 106, 90, 205);
        public readonly static ColorX SlateGray = new ColorX(255, 112, 128, 144);
        public readonly static ColorX Snow = new ColorX(255, 255, 250, 250);
        public readonly static ColorX SpringGreen = new ColorX(255, 0, 255, 127);
        public readonly static ColorX SteelBlue = new ColorX(255, 70, 130, 180);
        public readonly static ColorX Tan = new ColorX(255, 210, 180, 140);
        public readonly static ColorX Teal = new ColorX(255, 0, 128, 128);
        public readonly static ColorX Thistle = new ColorX(255, 216, 191, 216);
        public readonly static ColorX Tomato = new ColorX(255, 255, 99, 71);
        public readonly static ColorX Turquoise = new ColorX(255, 64, 224, 208);
        public readonly static ColorX Violet = new ColorX(255, 238, 130, 238);
        public readonly static ColorX Wheat = new ColorX(255, 245, 222, 179);
        public readonly static ColorX White = new ColorX(255, 255, 255, 255);
        public readonly static ColorX WhiteSmoke = new ColorX(255, 245, 245, 245);
        public readonly static ColorX Yellow = new ColorX(255, 255, 255, 0);
        public readonly static ColorX YellowGreen = new ColorX(255, 154, 205, 50);
        public readonly static ColorX ButtonFace = new ColorX(255, 240, 240, 240);
        public readonly static ColorX ButtonHighlight = new ColorX(255, 255, 255, 255);
        public readonly static ColorX ButtonShadow = new ColorX(255, 160, 160, 160);
        public readonly static ColorX GradientActiveCaption = new ColorX(255, 185, 209, 234);
        public readonly static ColorX GradientInactiveCaption = new ColorX(255, 215, 228, 242);
        public readonly static ColorX MenuBar = new ColorX(255, 240, 240, 240);
        public readonly static ColorX MenuHighlight = new ColorX(255, 51, 153, 255);
        #endregion

        public static ColorX FromArgb(int a, int r, int g, int b)
        {
            return new ColorX(a, r, g, b);
        }
        public static ColorX FromArgb(int a, ColorX color)
        {
            return new ColorX(a, color.R, color.G, color.B);
        }
        public int R;
        public int G;
        public int B;
        public int A;
        public void Constrain()
        {
            if (R > 255) R = 255;
            else if (R < 0) R = 0;

            if (G > 255) G = 255;
            else if (G < 0) G = 0;

            if (B > 255) B = 255;
            else if (B < 0) B = 0;

            if (A > 255) A = 255;
            else if (A < 0) A = 0;
        }
        public ColorX(int a, int r, int g, int b)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
        public ColorX(int r, int g, int b)
        {
            R = r;
            G = g;
            B = b;
            A = 255;
        }
        public ColorX(Color c)
        {
            R = c.R;
            G = c.G;
            B = c.B;
            A = c.A;
        }
        public static implicit operator ColorX(Color c)
        {
            return new ColorX(c);
        }
        public static implicit operator Color(ColorX c)
        {
            return Color.FromArgb(c.A, c.R, c.G, c.B);
        }
    }

    public class DrawingX
    {
        private ImageX buffer;
        public DrawingX(ImageX Buffer)
        {
            buffer = Buffer;
        }
        public void DrawRectangle(int x, int y, int w, int h, ColorX c)
        {
            if (x > buffer.Size.Width || y > buffer.Size.Height) return;
            int r = RayMath.constrain(x + w, 0, buffer.Size.Width);
            int b = RayMath.constrain(y + h, 0, buffer.Size.Height);
            for (int xi = x; xi < r; xi++)
            {
                buffer.SetPixel(xi, y, c);
                buffer.SetPixel(xi, y + h, c);
            }
            for (int yi = y; yi < b; yi++)
            {
                buffer.SetPixel(x + w, yi, c);
                buffer.SetPixel(x, yi, c);
            }
        }
        public void DrawRectangle(float x, float y, float w, float h, ColorX c)
        {
            DrawRectangle((int)x, (int)y, (int)w, (int)h, c);
        }
        public void DrawRectangle(Rectangle rec, ColorX c)
        {
            DrawRectangle(rec.X, rec.Y, rec.Width, rec.Height, c);
        }
        public void DrawRectangle(RectangleF rec, ColorX c)
        {
            DrawRectangle(rec.X, rec.Y, rec.Width, rec.Height, c);
        }
        public void DrawRectangleLTRB(int x, int y, int r, int b, ColorX c)
        {
            DrawRectangle(Rectangle.FromLTRB(x, y, r, b), c);
        }
        public void FillRectangle(int x, int y, int w, int h, ColorX c)
        {
            if (x > buffer.Size.Width || y > buffer.Size.Height) return;
            int r = RayMath.constrain(x + w, 0, buffer.Size.Width);
            int b = RayMath.constrain(y + h, 0, buffer.Size.Height);
            for (int xi = x; xi < r; xi++)
            {
                for (int yi = y; yi < b; yi++)
                {
                    buffer.SetPixel(xi, yi, c);
                }
            }
        }
        public void FillRectangle(float x, float y, float w, float h, ColorX c)
        {
            FillRectangle((int)x, (int)y, (int)w, (int)h, c);
        }
        public void DrawXLine(int x0, int y, int x1, ColorX c)
        {
            if (y < 0 || y > buffer.Size.Height) return;
            x0 = RayMath.constrain(x0, 0, buffer.Size.Width);
            x1 = RayMath.constrain(x1, 0, buffer.Size.Width);
            for (int xi = x0; xi <= x1; xi++)
            {
                buffer.SetPixel(xi, y, c);
            }
        }
        public void DrawYLine(int x, int y0, int y1, ColorX c)
        {
            if (x < 0 || x > buffer.Size.Width) return;
            y0 = RayMath.constrain(y0, 0, buffer.Size.Height);
            y1 = RayMath.constrain(y1, 0, buffer.Size.Height);
            for (int yi = y0; yi <= y1; yi++)
            {
                buffer.SetPixel(x, yi, c);
            }
        }
        public void DrawLine(int x0, int y0, int x1, int y1, ColorX color)
        {
            bool steep = (Math.Abs(y1 - y0) > Math.Abs(x1 - x0));
            if (steep)
            {
                RayMath.Swap(ref x0, ref y0);
                RayMath.Swap(ref x1, ref y1);
            }

            if (x0 > x1)
            {
                RayMath.Swap(ref x0, ref x1);
                RayMath.Swap(ref y0, ref y1);
            }

            int dx, dy;
            dx = x1 - x0;
            dy = Math.Abs(y1 - y0);

            int err = dx / 2;
            int ystep;

            if (y0 < y1)
            {
                ystep = 1;
            }
            else
            {
                ystep = -1;
            }

            for (; x0 <= x1; x0++)
            {
                if (steep)
                {
                    buffer.SetPixel(y0, x0, color);
                }
                else
                {
                    buffer.SetPixel(x0, y0, color);
                }
                err -= dy;
                if (err < 0)
                {
                    y0 += ystep;
                    err += dx;
                }
            }
        }
        public void DrawCross(Point center, int radius, int width, ColorX c)
        {
            int r2 = radius / 2;
            for (int i = 0; i < width; i++)
            {
                DrawXLine(center.X - r2, center.Y + i, center.X + r2 + 1, c);
                DrawYLine(center.X + i, center.Y - r2, center.Y + r2 + 1, c);
            }
        }
        public void FillRectangle(Rectangle rec, ColorX c)
        {
            FillRectangle(rec.X, rec.Y, rec.Width, rec.Height, c);
        }
        public void FillRectangle(RectangleF rec, ColorX c)
        {
            FillRectangle((int)rec.X, (int)rec.Y, (int)rec.Width, (int)rec.Height, c);
        }
        public void Clear(ColorX c)
        {
            FillRectangle(0, 0, buffer.Width, buffer.Height, c);
        }

        //public static double CompareBitmaps(Bitmap Bitmap1, Bitmap Bitmap2, int chunkSize)
        //{
        //    byte[] ba1 = RayMath.GetBGRA(Bitmap1);
        //    Bitmap bi = Bitmap2;
        //    if (Bitmap1.Width != Bitmap2.Width || Bitmap1.Height != Bitmap2.Height)
        //    {
        //        bi = new Bitmap(bi, Bitmap1.Size);
        //    }
        //    byte[] ba2 = RayMath.GetBGRA(bi);
        //    double rd1 = 0.0;
        //    double gd1 = 0.0;
        //    double bd1 = 0.0;
        //    double rd2 = 0.0;
        //    double gd2 = 0.0;
        //    double bd2 = 0.0;

        //    int dw = chunkSize;// 0;
        //    int cntall = 0;
        //    for (int x = 0; x < Bitmap1.Width - dw; x += dw)
        //    {
        //        for (int h = 0; h < Bitmap1.Height - dw; h += dw)
        //        {
        //            for (int xi = x; xi < x + 10; xi++)
        //            {
        //                for (int y = h; y < h + 10; y++)
        //                {
        //                    ColorX c1 = GetPixel(xi, y, ba1, Bitmap1.Width, Bitmap1.PixelFormat);
        //                    ColorX c2 = GetPixel(xi, y, ba2, bi.Width, bi.PixelFormat);
        //                    rd1 += (c1.R / 255.0);
        //                    gd1 += (c1.G / 255.0);
        //                    bd1 += (c1.B / 255.0);
        //                    rd2 += (c2.R / 255.0);
        //                    gd2 += (c2.G / 255.0);
        //                    bd2 += (c2.B / 255.0);
        //                    cntall++;
        //                }
        //            }
        //        }
        //    }
        //    rd1 /= (double)cntall;
        //    gd1 /= (double)cntall;
        //    bd1 /= (double)cntall;
        //    rd2 /= (double)cntall;
        //    gd2 /= (double)cntall;
        //    bd2 /= (double)cntall;

        //    double d = (DivH(rd1, rd2) + DivH(gd1, gd2) + DivH(bd1, bd2)) / 3.0;
        //    return d;
        //}
        public static double DivH(double d, double d1)
        {
            return Math.Max(d, d1) / Math.Min(d, d1);
        }


        public void DrawImage(ImageX image, int x, int y)
        {
            DrawImage(image, x, y, image.Width, image.Height);
        }
        public void DrawImage(ImageX image, int x, int y, int w, int h)
        {
            try
            {
                if (x > buffer.Size.Width || y > buffer.Size.Height) return;

                float wf = (float)image.Width / (float)w;
                float hf = (float)image.Height / (float)h;

                for (int xi = 0; xi < image.Width; xi += (int)wf)
                {
                    for (int yi = 0; yi < image.Height; yi += (int)hf)
                    {
                        ColorX c = image.GetPixel(xi, yi);
                        buffer.SetPixel((int)(x + (xi / wf)), (int)(y + (yi / hf)), c);
                    }
                }
            }
            catch (Exception xe)
            {
                string s = xe.ToString();
            }
        }

        public void DrawImage(ImageX image, int x, int y, int w, int h, ColorX color)
        {
            if (x > buffer.Size.Width || y > buffer.Size.Height) return;

            float wf = image.Width / w;
            float hf = image.Height / h;

            if (image.BytesPerPixel > 1)
            {
                for (int xi = 0; xi < image.Width; xi += (int)wf)
                {
                    for (int yi = 0; yi < image.Height; yi += (int)hf)
                    {
                        if (image.GetAlpha(xi, yi) > 0)
                        {
                            buffer.SetPixel((int)(x + (xi / wf)), (int)(y + (yi / hf)), color);
                        }
                    }
                }
            }
            else
            {
                for (int xi = 0; xi < image.Width; xi += (int)wf)
                {
                    for (int yi = 0; yi < image.Height; yi += (int)hf)
                    {
                        int i = ((y * image.Width) + x);
                        if (image.Raw[i] > 0)
                        {
                            buffer.SetPixel((int)(x + (xi / wf)), (int)(y + (yi / hf)), color);
                        }
                    }
                }
            }
        }

        public void DrawString(string s, FontX font, ColorX c, int x, int y, float fontSize)
        {
            //s = s.Replace("ö", "o");
            //s = s.Replace("ä", "a");
            //s = s.Replace("ü", "u");
            string si = "";
            int lines = 1;
            for (int i = 0; s.Length > i; i++)
            {
                if (!char.IsControl(s[i]))
                {
                    si += s[i];
                }
                else if (s[i] == '\n')
                {
                    si += s[i];
                    lines++;
                }
            }
            int fontWidth = (int)(fontSize * font.Width);
            int fontHeight = (int)(fontSize * font.Height);
            int xw = x;
            for (int i = 0; si.Length > i; i++)
            {
                if (si[i] == '\n')
                {
                    y += fontHeight + 10;
                    xw = x;
                }
                else
                {
                    font.DrawChar(s[i], buffer, c, xw, y, fontWidth, fontHeight);
                    xw += (int)(font.Width * fontSize);
                }
            }
        }
        public Rectangle DrawString(string s, FontX font, float fontSize, ColorX c, Rectangle bounds, StringAlignment Alignment)
        {
            Size sf = MeasureString(s, font, fontSize);
            Rectangle rec = getLoc(Alignment, bounds, sf);
            DrawString(s, font, c, rec.X, rec.Y, fontSize);
            return rec;
        }
        public static Rectangle MeasureString(string s, FontX font, float fontSize, Rectangle bounds, StringAlignment Alignment)
        {
            Size sf = MeasureString(s, font, fontSize);
            Rectangle rec = getLoc(Alignment, bounds, sf);
            return rec;
        }
        public static Rectangle MeasureString(int charCount, int lineCnt, FontX font, float fontSize, Rectangle bounds, StringAlignment Alignment)
        {
            Size sf = MeasureString(charCount, lineCnt, font, fontSize);
            Rectangle rec = getLoc(Alignment, bounds, sf);
            return rec;
        }
        public enum StringAlignment
        {
            TopLeft,
            TopRight,
            BotLeft,
            BotRight,
            Centered,
            CentLeft,
            CentRight,
            CentTop,
            CentBot
        }
        public static Rectangle getLoc(StringAlignment pos, Rectangle bounds, Size textSize)
        {
            switch (pos)
            {
                case StringAlignment.TopLeft:
                    return new Rectangle(bounds.X, bounds.Y, textSize.Width, textSize.Height);
                case StringAlignment.TopRight:
                    return new Rectangle(bounds.Right - textSize.Width, bounds.Y, textSize.Width, textSize.Height);

                case StringAlignment.BotLeft:
                    return new Rectangle(bounds.X, bounds.Bottom - textSize.Height, textSize.Width, textSize.Height);
                case StringAlignment.BotRight:
                    return new Rectangle(bounds.Right - textSize.Width, bounds.Bottom - textSize.Height,
                        textSize.Width, textSize.Height);

                case StringAlignment.Centered:
                    return new Rectangle(bounds.X + ((bounds.Width / 2)) - (textSize.Width / 2),
                        bounds.Y + ((bounds.Height / 2)) - (textSize.Height / 2),
                        textSize.Width, textSize.Height);

                case StringAlignment.CentLeft:
                    return new Rectangle(bounds.X,
                        bounds.Y + ((bounds.Height / 2)) - (textSize.Height / 2),
                        textSize.Width, textSize.Height);
                case StringAlignment.CentRight:
                    return new Rectangle(bounds.Right - textSize.Width,
                        bounds.Y + ((bounds.Height / 2)) - (textSize.Height / 2),
                        textSize.Width, textSize.Height);

                case StringAlignment.CentTop:
                    return new Rectangle(bounds.X + ((bounds.Width / 2)) - (textSize.Width / 2),
                        bounds.Y,
                        textSize.Width, textSize.Height);
                case StringAlignment.CentBot:
                    return new Rectangle(bounds.X + ((bounds.Width / 2)) - (textSize.Width / 2),
                        bounds.Bottom - textSize.Height,
                        textSize.Width, textSize.Height);
            }
            return Rectangle.Empty;
        }
        public static Size MeasureString(string s, FontX font, float fontSize)
        {
            //string[] sa = s.Split('\n');
            int lines = 1;
            int max = 1;
            int cnt = 0;
            for (int i = 0; s.Length > i; i++)
            {
                if (!char.IsControl(s[i]))
                {
                    cnt++;
                }
                else
                {
                    if (cnt > max) max = cnt;
                    cnt = 0;

                    if (s[i] == '\n')
                    {
                        lines++;
                    }
                }
            }
            if (cnt > max) max = cnt;
            return new Size((int)(font.Width * fontSize * max), (int)((font.Height * lines) * fontSize) + 10);
        }
        public static Size MeasureString(int charCount, int lineCnt, FontX font, float fontSize)
        {
            return new Size((int)(font.Width * fontSize * charCount), (int)((font.Height * lineCnt) * fontSize) + 10);
        }
    }

    public class ImageX
    {
        private byte[] buffer;
        private Size size;
        private int bpPixel = 3;
        private DrawingX drawing;
        public DrawingX Drawing
        {
            get { return drawing; }
        }
        public int Length
        {
            get { return buffer.Length; }
        }
        public ImageX(int w, int h)
        {
            size = new Size(w, h);
            buffer = new byte[w * h * BytesPerPixel];
            drawing = new DrawingX(this);
        }
        public ImageX(int w, int h, int bytesPerPixel)
        {
            bpPixel = bytesPerPixel;
            size = new Size(w, h);
            buffer = new byte[w * h * BytesPerPixel];
            drawing = new DrawingX(this);
        }
        public ImageX()
        {
            buffer = new byte[0];
            size = new Size();
        }
        public ImageX(byte[] bgra, int w, int h, int bytesPerPixel)
        {
            buffer = bgra;
            size = new Size(w, h);
            bpPixel = bytesPerPixel;
        }
        public ImageX(Bitmap b)
        {
            Buffer = b;
            drawing = new DrawingX(this);
        }
        public Size Size
        {
            get { return size; }
        }
        public int Width
        {
            get { return size.Width; }
        }
        public int Height
        {
            get { return size.Height; }
        }
        public PixelFormat Format
        {
            get { return RayMath.GetPixelsPerByte(bpPixel); }
        }
        public int BytesPerPixel
        {
            get { return bpPixel; }
        }

        public ColorX GetPixel(int x, int y)
        {
            if (x >= size.Width) x = size.Width - 1;
            else if (x < 0) x = 0;
            if (y >= size.Height) y = size.Height - 1;
            else if (y < 0) y = 0;

            long i = ((y * size.Width) + x) * bpPixel;

            if (bpPixel == 1)
            {
                if (i >= 0 && i < buffer.Length)
                {
                    if (buffer[i] > 0)
                    {
                        return ColorX.White;
                    }
                    return Color.Transparent;
                }
            }

            if (i >= 0 && i + 2 < buffer.Length)
            {
                return new ColorX(((bpPixel > 3) ? buffer[i + 3] : 255), buffer[i + 2], buffer[i + 1], buffer[i]);
            }
            return Color.Transparent;
        }
        public int GetAlpha(int x, int y)
        {
            if (x >= size.Width) x = size.Width - 1;
            else if (x < 0) x = 0;
            if (y >= size.Height) y = size.Height - 1;
            else if (y < 0) y = 0;
            int i = ((y * size.Width) + x) * bpPixel;
            if (bpPixel == 1)
            {
                if (buffer[i] == 0)
                {
                    return 0;
                }
            }
            else
            {
                if (i >= 0 && i + 2 < buffer.Length && bpPixel > 3)
                {
                    return buffer[i + 3];
                }
            }
            return 255;
        }
        public int GetRed(int x, int y)
        {
            if (x >= size.Width) x = size.Width - 1;
            else if (x < 0) x = 0;
            if (y >= size.Height) y = size.Height - 1;
            else if (y < 0) y = 0;
            int i = ((y * size.Width) + x) * bpPixel;
            if (i >= 0 && i + 2 < buffer.Length && bpPixel > 3)
            {
                return buffer[i + 2];
            }
            return 255;
        }
        public int GetGreen(int x, int y)
        {
            if (x >= size.Width) x = size.Width - 1;
            else if (x < 0) x = 0;
            if (y >= size.Height) y = size.Height - 1;
            else if (y < 0) y = 0;
            int i = ((y * size.Width) + x) * bpPixel;
            if (i >= 0 && i + 2 < buffer.Length && bpPixel > 3)
            {
                return buffer[i + 1];
            }
            return 255;
        }
        public int GetBlue(int x, int y)
        {
            if (x >= size.Width) x = size.Width - 1;
            else if (x < 0) x = 0;
            if (y >= size.Height) y = size.Height - 1;
            else if (y < 0) y = 0;
            int i = ((y * size.Width) + x) * bpPixel;
            if (i >= 0 && i + 2 < buffer.Length && bpPixel > 3)
            {
                return buffer[i];
            }
            return 255;
        }
        public void SetPixel(int x, int y, ColorX c)
        {
            SetPixel(x, y, c.R, c.G, c.B, c.A);
        }
        public void SetPixelFast(int x, int y, int r, int g, int b)
        {
            //if (x >= size.Width) x = size.Width - 1;
            //else if (x < 0) x = 0;
            //if (y >= size.Height) y = size.Height - 1;
            //else if (y < 0) y = 0;

            if (r > 255) r = 255;
            if (g > 255) g = 255;
            if (b > 255) b = 255;
            int i = ((y * size.Width) + x) * bpPixel;

            buffer[i] = (byte)(b);
            buffer[i + 1] = (byte)(g);
            buffer[i + 2] = (byte)(r);
        }
        public void SetPixel(int x, int y, int r, int g, int b, int a)
        {
            if (x >= size.Width) x = size.Width - 1;
            else if (x < 0) x = 0;
            if (y >= size.Height) y = size.Height - 1;
            else if (y < 0) y = 0;

            if (r > 255) r = 255;
            if (g > 255) g = 255;
            if (b > 255) b = 255;
            if (a > 255) a = 255;
            //BGRA
            int i = ((y * size.Width) + x) * bpPixel;
            if (bpPixel == 1)
            {
                if (i >= 0 && i < buffer.Length)
                {
                    if (r > 0 || g > 0 || b > 0 || a > 0)
                    {
                        buffer[i] = 255;
                    }
                    else
                    {
                        buffer[i] = 0;
                    }
                }
            }

            if (i >= 0 && i + ((bpPixel > 3) ? 3 : 2) < buffer.Length)
            {
                if (a < 255)
                {
                    float af = (float)a / 255.0f;
                    b = (int)(af * b + (1.0f - af) * buffer[i + 0]);
                    g = (int)(af * g + (1.0f - af) * buffer[i + 1]);
                    r = (int)(af * r + (1.0f - af) * buffer[i + 2]);
                }
                buffer[i] = (byte)(b);
                buffer[i + 1] = (byte)(g);
                buffer[i + 2] = (byte)(r);
                if (bpPixel > 3)
                {
                    buffer[i + 3] = (byte)(a);
                }
            }
        }
        public Bitmap Buffer
        {
            get
            {
                return RayMath.GetBitmap(buffer, size, RayMath.GetPixelsPerByte(bpPixel));
            }
            set
            {
                bpPixel = RayMath.GetBytesPerPixel(value.PixelFormat);
                size = value.Size;
                buffer = RayMath.GetBGRA(value);
                drawing = new DrawingX(this);
            }
        }
        public byte[] Raw
        {
            get { return buffer; }
            set { buffer = value; }
        }
        public void CopyTo(IntPtr ptr)
        {
            Marshal.Copy(buffer, 0, ptr, buffer.Length);
        }

        public static explicit operator Bitmap(ImageX image)
        {
            return image.Buffer;
        }
        public static explicit operator ImageX(Bitmap image)
        {
            return new ImageX(image);
        }
    }

    public class FontX
    {
        private Size size;
        public int Width
        {
            get { return size.Width; }
        }
        public int Height
        {
            get { return size.Height; }
        }
        private ImageX image = new ImageX();
        private static string[] fontNames = new string[] { "kongtext", "november" };
        public static void CreateFontX(float fontSize)
        {
            byte minChar = 32;
            byte maxChar = 255;
            foreach (string s in fontNames)
            {
                try
                {
                    Font f = new Font(s, fontSize);
                    Bitmap bitmap = new Bitmap(200, 200);

                    Graphics g = Graphics.FromImage(bitmap);

                    SizeF sft = g.MeasureString("A", f);
                    Size charSize = new Size((int)Math.Ceiling(sft.Width), (int)Math.Ceiling(sft.Height));

                    g.Dispose();

                    bitmap = new Bitmap((int)(charSize.Width * (maxChar - minChar)), (int)(charSize.Height));

                    g = Graphics.FromImage(bitmap);
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                    g.Clear(Color.Black);
                    g.FillRectangle(Brushes.Red, charSize.Width, 0, 1, 1);
                    string sc = Encoding.GetEncoding(1252).GetString(new byte[] { 127 });
                    for (byte i = minChar; i < maxChar; i++)
                    {
                        string si = Encoding.GetEncoding(1252).GetString(new byte[] { i });
                        if (si != sc)
                        {
                            g.DrawString(si, f, Brushes.White, (i - minChar) * charSize.Width, 3);
                        }
                        //else
                        //{
                        //    g.FillRectangle(Brushes.White, (i - minChar) * charSize.Width, 0, charSize.Width, charSize.Height);
                        //}
                    }
                    g.Dispose();
                    bitmap.Save(s + ".png", ImageFormat.Png);
                }
                catch (Exception x)
                {
                    string sx = x.ToString();
                }
            }
        }
        public FontX(ImageX b)
        {
            image = b;
            size = GetFontSize(image);
        }
        public static Size GetFontSize(ImageX font)
        {
            int w = 26;
            int h = font.Height;

            while (w < font.Width)
            {
                if (font.GetRed(w, 0) > 0)
                {
                    break;
                }
                w++;
            }
            return new Size(w, h);
        }

        //public Size MeasureString(string s, float fontSize)
        //{
        //    //string[] sa = tempText.Split(new char[] { '\r', '\n' });
        //    return new Size((int)(Width * fontSize * s.Length), (int)(Height * fontSize));
        //}
        //public Size MeasureString(int charCount, float fontSize)
        //{
        //    //string[] sa = tempText.Split(new char[] { '\r', '\n' });

        //    return new Size((int)(Width * fontSize * charCount), (int)(Height * fontSize));
        //}
        //public void DrawChar(char c, ImageX toDrawOn, ColorX ColorX, int x, int y)
        //{
        //    int i = (int)c - 32;
        //    if (c == ' ') i = 1;
        //    int left = i * size.Width;
        //    int right = left + size.Width;

        //    for (int xi = left; xi < right; xi++)
        //    {
        //        for (int y = 0; size.Height > y; y++)
        //        {
        //            ColorX cOK = image.GetPixel(xi, y);
        //            if (cOK.A > 0 && cOK.R > 0 && cOK.G > 0 && cOK.B > 0)
        //            {
        //                toDrawOn.SetPixel(x + (xi - left), y + y, ColorX);
        //            }
        //        }
        //    }
        //}
        public void DrawChar(char c, ImageX toDrawOn, ColorX ColorX, int x, int y, int w, int h)
        {
            if (Char.IsControl(c)) return;
            if ((int)c > 126) return;
            int i = (int)c - 32;
            int left = i * size.Width;
            int right = left + size.Width;

            float wf = size.Width / w;
            float hf = size.Height / h;

            if (image.BytesPerPixel > 1)
            {
                for (int xi = left; xi < right; xi += (int)wf)
                {
                    for (int yi = 0; size.Height > yi; yi += (int)hf)
                    {
                        ColorX co = image.GetPixel(xi, yi);
                        if (co.A > 0 && co.R > 0 && co.G > 0 && co.B > 0)
                        {
                            toDrawOn.SetPixel((int)(x + (xi - left) / wf), (int)(y + yi / hf), ColorX);
                        }
                    }
                }
            }
            else
            {
                for (int xi = left; xi < right; xi += (int)wf)
                {
                    for (int yi = 0; size.Height > yi; yi += (int)hf)
                    {
                        int idx = ((yi * image.Width) + xi);
                        if (image.Raw[idx] > 0)
                        {
                            toDrawOn.SetPixel((int)(x + (xi - left) / wf), (int)(y + yi / hf), ColorX);
                        }
                    }
                }
            }
        }
    }

    //public class FontXDict
    //{
    //    private System.Resources.ResourceManager manager;
    //    public FontXDict(string fontInfo,System.Resources.ResourceManager resourceManger)
    //    {
    //        manager = resourceManger;
    //        LoadFontNames(fontInfo);// fontInfo);
    //    }
    //    private Dictionary<string, Size> fonts = new Dictionary<string, Size>();
    //    public int Count
    //    {
    //        get { return fonts.Count; }
    //    }
    //    private void LoadFontNames(string si)
    //    {
    //        StringReader sr = new StringReader(si);
    //        while (true)
    //        {
    //            string s = sr.ReadLine();
    //            if (s == null) break;
    //            string[] sa = s.Split(':');

    //            fonts.Add(sa[0], new Size(int.Parse(sa[1]), int.Parse(sa[2])));
    //        }
    //    }
    //    public FontX Load(string name)
    //    {
    //        foreach (KeyValuePair<string, Size> k in fonts)
    //        {
    //            if (k.Key.Contains(name))
    //            {
    //                Bitmap b = (Bitmap)manager.GetObject(k.Key);
    //                return new FontX(b, k.Result);
    //            }
    //        }
    //        return null;
    //    }
    //    public FontX Load(int Index, out string fontName)
    //    {
    //        if (Index >= Count || Index < 0) Index = 0;

    //        fontName = GetFontNames()[Index];

    //        return Load(fontName);
    //    }
    //    public string[] GetFontNames()
    //    {
    //        List<string> list = new List<string>();
    //        foreach (KeyValuePair<string, Size> k in fonts)
    //        {
    //            list.Add(k.Key);
    //        }
    //        return list.ToArray();
    //    }
    //}
}
