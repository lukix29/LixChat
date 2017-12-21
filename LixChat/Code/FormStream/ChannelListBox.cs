using LX29_ChatClient.Channels;
using LX29_Twitch.Api;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace LX29_ChatClient.Forms
{
    public partial class ChannelListBox : UserControl
    {
        private SolidBrush BackBrush = new SolidBrush(Color.Black);
        //private SolidBrush InverseBG = new SolidBrush(Color.White);

        private bool isMouseDown = false;

        private bool isScrolling = false;

        private int ItemHeight = 10;

        private object locko = new object();

        private int maxVisibleItems = 0;

        private int scrollOffset = 0;

        private Rectangle scrollRect = new Rectangle();

        private bool scrollVisible = false;

        private List<int> selIndices = new List<int>();

        private int selStartY = 0;

        public ChannelListBox()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }

        public delegate void OnSelectedIndexChanged(object sender, EventArgs e);

        public event OnSelectedIndexChanged SelectedIndexChanged;

        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                BackBrush = new SolidBrush(value);
                base.BackColor = value;
            }
        }

        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                ItemHeight = TextRenderer.MeasureText("A", value).Height;
                base.Font = value;
            }
        }

        public ChannelInfo[] Items
        {
            get;
            private set;
        }

        public int SelectedIndex
        {
            get { return (selIndices.Count > 0) ? selIndices[0] : 0; }
            set
            {
                int c = (Items != null) ? Items.Length - 1 : 0;
                int val = Math.Max(scrollOffset, Math.Min(c, value));
                if (selIndices.Count > 0) selIndices[0] = val;
                else selIndices.Add(val);
                SelectedIndexChanged?.Invoke(this, new EventArgs());
            }
        }

        public int[] SelectedIndices
        {
            get { return selIndices.ToArray(); }
        }

        public ChannelInfo[] SelectedItems
        {
            get
            {
                try
                {
                    if (Items != null && selIndices.Count > 0)
                    {
                        var items = Items.Select((item, index) => new { item, index })
                            .Where(value => SelectedIndices.Any(i => (i == value.index)))
                            .Select(t => t.item);
                        if (items.Count() > 0)
                        {
                            return items.ToArray();
                        }
                    }
                }
                catch { }
                return null;
            }
        }

        private bool isCtrPressed

        {
            get { return ModifierKeys.HasFlag(Keys.Control); }
        }

        private int scrollBarHeight

        {
            get
            {
                int length = (Items != null) ? Items.Length : 0;
                length /= ItemHeight;
                return Math.Max(40, Math.Min(this.ClientRectangle.Height, length));
            }
        }

        public new void Refresh()
        {
            try
            {
                var selItems = SelectedItems;
                Items = ChatClient.GetSortedChannelNames();
                if ((Items != null && selItems != null) && selIndices.Count > 0 && selItems.Length > 0 && Items.Length > 0)
                {
                    selIndices.Clear();
                    for (int i = 0; i < Items.Length; i++)
                    {
                        if (selItems.Any(t0 => Items[i].ID.Equals(t0.ID)))
                        {
                            selIndices.Add(i);
                        }
                    }
                }
            }
            catch { }
            finally
            {
                this.Invoke(new Action(() =>
                    {
                        base.Refresh();
                    }));
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            maxVisibleItems = (int)(1f + (this.ClientRectangle.Height / (float)ItemHeight));

            scrollRect = new Rectangle(this.ClientSize.Width - 12, 0, 10, scrollBarHeight);
            //this.KeyUp += ChannelListBox_KeyUp;
            ItemHeight = TextRenderer.MeasureText("A", Font).Height;
            base.OnLoad(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (new Rectangle(scrollRect.X - 5, 0, scrollRect.Width + 10, this.ClientSize.Height).
                    Contains(e.Location))
            {
                if (new Rectangle(scrollRect.X - 5, scrollRect.Y, scrollRect.Width + 10, scrollRect.Height).
                     Contains(e.Location))
                {
                    isScrolling = true;
                }
            }
            else
            {
                if (!isCtrPressed)
                    selIndices.Clear();
            }
            isMouseDown = true;
            selStartY = e.Y;
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            try
            {
                if (isMouseDown)
                {
                    if (isScrolling)
                    {
                        CalcScrolling(e.Y);
                    }
                    else if (!isScrolling)
                    {
                        int h = Math.Abs(e.Y - selStartY);
                        for (int yi = 0; yi <= h; yi++)
                        {
                            int y = yi + selStartY;
                            if (y >= this.ClientRectangle.Bottom)
                            {
                                scrollOffset = Math.Min(Items.Length - maxVisibleItems, scrollOffset + 1);
                            }
                            else if (y <= 0)
                            {
                                scrollOffset = Math.Max(0, scrollOffset - 1);
                            }
                            int index = scrollOffset + (y / ItemHeight);
                            if (!isCtrPressed)
                            {
                                if (!selIndices.Contains(index))
                                {
                                    selIndices.Add(index);
                                }
                            }
                            else
                            {
                                if (selIndices.Contains(index))
                                {
                                    selIndices.Remove(index);
                                }
                            }
                        }
                        selStartY = e.Y + 1;
                    }
                }
            }
            catch
            {
            }
            this.Refresh();
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            try
            {
                if (new Rectangle(scrollRect.X - 5, 0, scrollRect.Width + 10, this.ClientSize.Height).
                     Contains(e.Location))
                {
                    CalcScrolling(e.Y);
                }
                else
                {
                    if (!isScrolling)
                    {
                        int index = scrollOffset + (e.Y / ItemHeight);
                        if (!isCtrPressed)
                        {
                            if (!selIndices.Contains(index))
                            {
                                selIndices.Add(index);
                            }
                        }
                        else
                        {
                            if (selIndices.Contains(index))
                            {
                                selIndices.Remove(index);
                            }
                            else
                            {
                                selIndices.Add(index);
                            }
                        }
                        SelectedIndexChanged?.Invoke(this, new EventArgs());
                    }
                }
            }
            catch
            {
            }
            isScrolling = false;
            isMouseDown = false;
            this.Refresh();
            base.OnMouseUp(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            scrollOffset = Math.Max(0, Math.Min(Items.Length - maxVisibleItems, scrollOffset - Math.Sign(e.Delta)));

            int ys = LXMath.Map(scrollOffset, 0, Items.Length - maxVisibleItems, 0, this.ClientSize.Height);
            ys = Math.Min(this.ClientRectangle.Bottom - scrollBarHeight, Math.Max(0, ys - scrollBarHeight / 2));
            scrollRect = new Rectangle(this.ClientRectangle.Right - 12, ys, 10, scrollBarHeight);

            this.Refresh();
            base.OnMouseWheel(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                e.Graphics.SetGraphicQuality(true, true);
                int y = 0;
                if (Items != null && Items.Length > 0)
                {
                    maxVisibleItems = 0;
                    scrollVisible = true;
                    for (int i = scrollOffset; i < Items.Length; i++)
                    {
                        bool selected = selIndices.Any(t => (t == i));
                        DrawItem(Items[i], e.Graphics, y, selected);
                        y += ItemHeight;
                        if (y > this.ClientRectangle.Bottom)
                        {
                            scrollVisible = true;
                            break;
                        }
                        maxVisibleItems++;
                    }
                    if (maxVisibleItems == 0)
                    {
                        scrollOffset = 0;
                        scrollVisible = false;
                    }
                    if (scrollVisible)
                    {
                        e.Graphics.DrawRectangle(Pens.DarkGray, new Rectangle(scrollRect.X - 1, 0, scrollRect.Width + 1, this.ClientSize.Height - 1));
                        e.Graphics.FillRectangle(Brushes.DarkGray, scrollRect);
                    }
                    else
                    {
                        isScrolling = false;
                    }
                    return;
                }
                else
                {
                    e.Graphics.DrawString("Loading Channels", this.Font, Brushes.Gainsboro, e.ClipRectangle, new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                    return;
                }
            }
            catch (Exception x)
            {
                e.Graphics.DrawString(x.Message, this.Font, Brushes.Gainsboro, e.ClipRectangle, new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }
        }

        protected override void OnResize(EventArgs e)
        {
            maxVisibleItems = (int)(0.5f + (this.ClientRectangle.Height / (float)ItemHeight));
            scrollRect = new Rectangle(this.ClientRectangle.Right - 12, scrollRect.Y, 10, scrollBarHeight);
            base.OnResize(e);
        }

        private void CalcScrolling(int Y)
        {
            int ys = Math.Min(this.ClientRectangle.Bottom - scrollBarHeight, Math.Max(0, Y - scrollBarHeight / 2));
            scrollRect = new Rectangle(this.ClientRectangle.Right - 12, ys, 10, scrollBarHeight);

            var offset = LXMath.Map(Y, scrollBarHeight / 2,
                this.ClientSize.Height - scrollBarHeight / 2, 0, Items.Length - maxVisibleItems);
            scrollOffset = Math.Max(0, Math.Min(Items.Length - maxVisibleItems, offset));
        }

        private void ChannelListBox_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                    SelectedIndex++;
                    break;

                case Keys.Up:
                    SelectedIndex--;
                    break;
            }
        }

        private void DrawItem(ChannelInfo info, Graphics g, int y, bool selected)
        {
            int off = 2;
            Rectangle rec = new Rectangle(off, y,
                ItemHeight - (off * 2), ItemHeight - (off * 2));

            Rectangle bounds = new Rectangle(0, y - off / 2,
                this.ClientSize.Width - scrollRect.Width - 2, ItemHeight + off);

            if (selected)
            {
                g.FillRectangle(SystemBrushes.HotTrack, bounds);
            }
            else
            {
                g.FillRectangle(BackBrush, bounds);
            }

            var typ = info.ApiResult.GetValue<StreamType>(ApiInfo.stream_type);
            switch (typ)
            {
                case StreamType.live:
                    g.FillEllipse(Brushes.Green, rec);
                    break;

                case StreamType.watch_party:
                    g.FillEllipse(Brushes.DarkOrange, rec);
                    break;
            }
            g.DrawEllipse(Pens.DarkGray, rec);

            rec.X += rec.Right + off;

            string inf = "";
            if (info.IsChatConnected) inf = "C ";
            if (info.SubInfo.IsSub) inf += "S";
            inf = inf.Trim();

            Size siz = TextRenderer.MeasureText(inf, this.Font, Extensions.maxSize, TextFormatFlags.NoPadding);
            rec.Size = siz;
            rec.Height = ItemHeight - off * 2;
            rec.Width -= off * 2;

            g.DrawRectangle(Pens.DarkGray, rec);
            TextRenderer.DrawText(g, inf, this.Font, new Point(rec.X + off + 1, y - off / 2), Color.LightGray, TextFormatFlags.NoPadding);

            Color sb = (info.IsFavorited) ? Color.LightGreen : Color.WhiteSmoke;
            TextRenderer.DrawText(g, info.DisplayName, this.Font, new Point(rec.Right + off, y - off), sb);
        }

        private void timer1_Tick(object sender, EventArgs e)

        {
            this.Refresh();
        }
    }
}