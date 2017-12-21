using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LX29_ChatClient.Dashboard
{
    public partial class FormDashboard : Form
    {
        private const string copTipTxt = "Copy Tipee API-Key here (https://goo.gl/t8MXwj)";

        //private BufferedGraphics bgc = null;
        private SolidBrush brushBG = new SolidBrush(Color.FromArgb(40, 40, 40));

        private DashboardData data = new DashboardData();

        private Dictionary<string, clickdata> hiderects = new Dictionary<string, clickdata>();

        private bool panel1MouseDown = false;

        private bool showUserNumbers = true;

        public FormDashboard()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            System.Reflection.MethodInfo aProp = typeof(System.Windows.Forms.Control)
                .GetMethod("SetStyle", System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            aProp.Invoke(listView1, new object[] { ControlStyles.OptimizedDoubleBuffer, true });
            aProp.Invoke(listView1, new object[] { ControlStyles.AllPaintingInWmPaint, true });
            aProp.Invoke(listView1, new object[] { ControlStyles.ResizeRedraw, true });
        }

        private void AddToListView(int value, string name)
        {
            if (listView1.LargeImageList == null) listView1.LargeImageList = new ImageList();

            listView1.LargeImageList.Images.Add(name, new Bitmap(40, 40, System.Drawing.Imaging.PixelFormat.Format1bppIndexed));

            var item = new ListViewItem(value.SizeMag(CultureInfo.CurrentCulture, 2), name) { Name = name };

            if (!hiderects.ContainsKey(name))
            {
                hiderects.Add(item.Name, new clickdata() { rec = new Rectangle(), item = item, hidden = false });
            }

            listView1.Items.Add(item);
        }

        private void btn_CloseSettings_Click(object sender, EventArgs e)
        {
            panelSettings.Visible = false;
        }

        private void btn_SaveApiKey_Click(object sender, EventArgs e)
        {
            string text = textBox1.Text.Trim();
            if (text.Equals(copTipTxt)) return;

            //System.IO.File.WriteAllText(Settings._dataDir + "tipeee.txt", text);
            data.TipeeeKey = text;
            data.Save();
            //textBox1.Visible = false;
            //btn_SaveApiKey.Visible = false;
            Task.Run(() =>
            {
                data.LoadTipeee();
                this.Invoke(new Action(() =>
                {
                    panelEvents.Visible = !string.IsNullOrEmpty(data.TipeeeKey);
                    SetEventControl();
                }));
            });
        }

        private void cB_ShowuserNumers_CheckedChanged(object sender, EventArgs e)
        {
            showUserNumbers = cB_ShowuserNumers.Checked;
            //hideNumbersToolStripMenuItem.Text = showUserNumbers ? "Hide Numbers" : "Show Numbers";
            SetControls();
        }

        #region Drawing

        private void ChatEventsDrawItem(NoticeMessage Index, Rectangle Bounds, Graphics g)
        {
            var ev = Index;

            int x = Bounds.X;

            using (var font = new Font(listBox1.Font, FontStyle.Bold))
            {
                var txt = ev.Name + " ";
                SizeF size = g.MeasureString(txt, listBox1.Font);
                g.DrawString(txt, font, Brushes.Gainsboro, x, Bounds.Y);
                x += (int)size.Width;

                txt = DashboardData.SubTierNames[ev.Type] + " ";
                size = g.MeasureString(txt, listBox1.Font);
                g.DrawString(txt, listBox1.Font, Brushes.Gainsboro, x, Bounds.Y);
                x += (int)size.Width;

                txt = "Sub ";
                size = g.MeasureString(txt, listBox1.Font);
                g.DrawString(txt, listBox1.Font, Brushes.Gainsboro, x, Bounds.Y);
                x += (int)size.Width;

                txt = ev.Value.SingleMag("Month");
                size = g.MeasureString(txt, listBox1.Font);
                g.DrawString(txt, font, Brushes.Gainsboro, x, Bounds.Y);
                x += (int)size.Width;

                if (!string.IsNullOrEmpty(ev.Message))
                {
                    txt = "\"" + ev.Message + "\"";
                    size = g.MeasureString(txt, listBox1.Font);
                    g.DrawString(txt, listBox1.Font, Brushes.Gainsboro, Bounds, new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Far, Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap });
                    x += (int)size.Width;
                }

                string delta = DateTime.Now.Subtract(ev.CreatedAt).SingleDuration() + " ago";
                using (var fago = new Font(listBox1.Font.FontFamily, 10))
                {
                    g.DrawString(delta, fago, Brushes.LightGray, Bounds, new StringFormat() { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Near });
                }
            }
        }

        private void DrawTipeee(Tipeee.DonationHost Index, Rectangle Bounds, Graphics g)
        {
            var dh = Index;
            int x = Bounds.X;

            using (var font = new Font(listBox1.Font, FontStyle.Bold))
            {
                var txt = dh.name + " ";
                SizeF size = g.MeasureString(txt, listBox1.Font);
                g.DrawString(txt, font, Brushes.Gainsboro, x, Bounds.Y);
                x += (int)size.Width;

                txt = dh.type + " ";
                size = g.MeasureString(txt, listBox1.Font);
                g.DrawString(txt, listBox1.Font, Brushes.Gainsboro, x, Bounds.Y);
                x += (int)size.Width;

                if (dh.is_host) txt = dh.amount.SizeMag(CultureInfo.CurrentCulture, 0, "N") + dh.currency + " ";
                else txt = dh.amount.SizeMag(CultureInfo.CurrentCulture, 2, "C") + " ";
                size = g.MeasureString(txt, listBox1.Font);
                g.DrawString(txt, font, Brushes.Gainsboro, x, Bounds.Y);
                x += (int)size.Width;

                if (!string.IsNullOrEmpty(dh.message))
                {
                    txt = "\"" + dh.message + "\"";
                    size = g.MeasureString(txt, listBox1.Font);
                    g.DrawString(txt, listBox1.Font, Brushes.Gainsboro, Bounds, new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Far, Trimming = StringTrimming.EllipsisCharacter });
                    x += (int)size.Width;
                }

                string delta = DateTime.Now.Subtract(dh.created_at).SingleDuration() + " ago";
                using (var fago = new Font(listBox1.Font.FontFamily, 10))
                {
                    g.DrawString(delta, fago, Brushes.LightGray, Bounds, new StringFormat() { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Near });
                }
            }
        }

        private void listBox_BanEvents_DrawItem(object sender, DrawItemEventArgs e)
        {
            var ev = (NoticeMessage)listBox_BanEvents.Items[e.Index];

            e.DrawBackground();

            string txt = "";
            SizeF size = SizeF.Empty;
            string delta = DateTime.Now.Subtract(ev.CreatedAt).SingleDuration() + " ago";
            using (var fago = new Font(listBox1.Font.FontFamily, 10))
            {
                e.Graphics.DrawString(delta, fago, Brushes.LightGray, e.Bounds,
                    new StringFormat() { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Near });

                if (ev.Value > 0)
                {
                    txt = "for " + TimeSpan.FromSeconds(ev.Value).SingleDuration();//.SizeMag(0);
                    size = e.Graphics.MeasureString(txt, fago);
                    e.Graphics.DrawString(txt, fago, Brushes.LightGray, e.Bounds,
                        new StringFormat() { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Far });
                }
                //size = e.Graphics.MeasureString(txt, listBox1.Font);
                //e.Graphics.DrawString(txt, font, Brushes.Gainsboro, x, e.Bounds.Y);
                //x += (int)size.Width;
            }

            Rectangle Bounds = e.Bounds;
            Bounds.Width = (int)(Bounds.Width - size.Width) - 5;

            using (var font = new Font(listBox1.Font, FontStyle.Bold))
            {
                txt = (ev.IsBan ? "Ban" : "TO");
                size = e.Graphics.MeasureString(txt, listBox1.Font);
                e.Graphics.DrawString(txt, listBox1.Font, Brushes.Gainsboro, Bounds,
                    new StringFormat() { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center });

                Bounds.Width -= (int)size.Width;

                txt = ev.Name + " ";
                // size = e.Graphics.MeasureString(txt, listBox1.Font);
                e.Graphics.DrawString(txt, font, Brushes.Gainsboro, Bounds,
                    new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near, Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap });

                // Bounds.Width -= (int)size.Width;

                if (!string.IsNullOrEmpty(ev.Message))
                {
                    txt = "\"" + ev.Message + "\"";
                    e.Graphics.DrawString(txt, listBox1.Font, Brushes.Gainsboro, Bounds, new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Far, Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap });
                }
            }
            e.Graphics.DrawRectangle(Pens.DarkGray, e.Bounds);
        }

        private void listBox_BanEvents_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight *= 2;
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            //var a = new { Type = false, Index = 0 };
            var a1 = (listboxitem)listBox1.Items[e.Index];

            e.DrawBackground();

            if (a1.Type)
            {
                DrawTipeee((Tipeee.DonationHost)a1.Value, e.Bounds, e.Graphics);
            }
            else
            {
                ChatEventsDrawItem((NoticeMessage)a1.Value, e.Bounds, e.Graphics);
            }
            e.Graphics.DrawRectangle(Pens.DarkGray, e.Bounds);
        }

        #endregion Drawing

        private void data_OnUserNoticeReceived(NoticeMessage notice)
        {
            this.Invoke(new Action(() =>
            {
                //if (!notice.IsSub)
                //{
                UpdateBanList();
                //}
                //else
                //{
                UpdateEventList();
                //}
            }));
        }

        private void enterTipeeeKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void FormDashboard_Load(object sender, EventArgs e)
        {
            panelSettings.Dock = DockStyle.Fill;
            //panelEvents.Visible = !string.IsNullOrEmpty(data.TipeeeKey);

            pictureBox1.Visible = true;
            pictureBox1.BringToFront();
            pictureBox1.Size = new Size(128, 128);
            pictureBox1.Location = new Point(this.ClientSize.Width / 2 - pictureBox1.Width / 2, this.ClientSize.Height / 2 - pictureBox1.Height / 2);

            listBox1.DrawMode = DrawMode.OwnerDrawVariable;
            listBox1.DrawItem += listBox1_DrawItem;
            listBox1.MeasureItem += listBox1_MeasureItem;

            //listBox_SubEvents.DrawMode = DrawMode.OwnerDrawVariable;
            //listBox_SubEvents.DrawItem += listBox_ChatEvents_DrawItem;
            //listBox_SubEvents.MeasureItem += listBox_ChatEvents_MeasureItem;

            listBox_BanEvents.DrawMode = DrawMode.OwnerDrawVariable;
            listBox_BanEvents.DrawItem += listBox_BanEvents_DrawItem;
            listBox_BanEvents.MeasureItem += listBox_BanEvents_MeasureItem;

            treeView1.DrawMode = TreeViewDrawMode.OwnerDrawText;
            treeView1.DrawNode += treeView1_DrawNode;
            treeView1.Scrollable = false;

            listView1.OwnerDraw = true;
            listView1.DrawItem += listView1_DrawItem;

            data = DashboardData.Load(Settings._dataDir + "dashboard.json");

            data.OnUserNoticeReceived += data_OnUserNoticeReceived;

            Task.Run(() => LoadEvents());
            Task.Run(() => LoadSubs());

            //Bitmap b0 = new Bitmap("test.jpg");
            //System.IO.MemoryStream ms = new System.IO.MemoryStream();
            //b0.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            //ms.Seek(0, 0);
            //for (int i = 0; i < 10000; i++)
            //{
            //    Bitmap b = new Bitmap(Bitmap.FromStream(ms));
            //    ms.Dispose();
            //    ms = new System.IO.MemoryStream();
            //    b.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            //    ms.Seek(0, 0);
            //}
            //b0 = new Bitmap(ms);
            //b0.Save("testNEW.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
        }

        private void hideNumbersToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void listBox_BanEvents_MouseHover(object sender, EventArgs e)
        {
            if (listBox_BanEvents.SelectedIndex >= 0)
            {
                var un = ((NoticeMessage)listBox_BanEvents.Items[listBox_BanEvents.SelectedIndex]);
                toolTip1.Show(un.Message, listBox_BanEvents, this.PointToClient(Cursor.Position));
            }
        }

        //private void listBox_ChatEvents_MeasureItem(object sender, MeasureItemEventArgs e)
        //{
        //    var dh = data.UserNotices[e.Index];
        //    if (!string.IsNullOrEmpty(dh.Message))
        //    {
        //        e.ItemHeight *= 2;
        //    }
        //}

        //private void listBox_ChatEvents_MouseHover(object sender, EventArgs e)
        //{
        //    if (listBox_SubEvents.SelectedIndex >= 0)
        //    {
        //        var un = data.ChatSubs[listBox_SubEvents.SelectedIndex];
        //        toolTip1.Show(un.Message, listBox_SubEvents, listBox_SubEvents.PointToClient(Cursor.Position));
        //    }
        //}

        private void listBox1_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            var a1 = (listboxitem)listBox1.Items[e.Index];

            var msg = "";
            if (a1.Type)
            {
                msg = ((Tipeee.DonationHost)a1.Value).message;
            }
            else
            {
                msg = ((NoticeMessage)a1.Value).Message;
            }
            if (!string.IsNullOrEmpty(msg))
            {
                e.ItemHeight *= 2;
            }
        }

        private void listBox1_MouseHover(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                var un = data.DonationHosts[listBox1.SelectedIndex];
                toolTip1.Show(un.message, listBox1, listBox1.PointToClient(Cursor.Position));
            }
        }

        private void listView1_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            try
            {
                e.Graphics.SetGraphicQuality(true, true);
                Rectangle rec = new Rectangle(e.Bounds.X + e.Bounds.Height / 4, e.Bounds.Y + 5, e.Bounds.Height, e.Bounds.Height);

                hiderects[e.Item.Name].rec = rec;

                bool canDraw = ChatClient.Emotes.Badges.ContainsKey(data.User.Name);

                Emotes.EmoteImage b = null;
                if (canDraw) b = ChatClient.Emotes.Badges.GetSubBadge(data.User.Name);
                //listView1.Height = rec.Height + 25;
                switch (e.Item.ImageKey)
                {
                    case "sub":
                        {
                            if (canDraw) b.Draw(e.Graphics, rec);
                        }
                        break;

                    case "follow":
                        {
                            e.Graphics.DrawImage(LX29_LixChat.Properties.Resources.follow, rec);
                        }
                        break;

                    case "viewer":
                        {
                            e.Graphics.DrawImage(LX29_LixChat.Properties.Resources.viewer, rec);
                        }
                        break;

                    case "chatter":
                        {
                            e.Graphics.DrawImage(LX29_LixChat.Properties.Resources.chat, rec);
                        }
                        break;

                    case "newsubs":
                        {
                            if (canDraw) b.Draw(e.Graphics, rec);
                            using (Font f = new Font("Calibri", 12f, FontStyle.Bold))
                            {
                                using (var gp = new System.Drawing.Drawing2D.GraphicsPath())
                                using (Pen pen = new Pen(Color.Black, 2))
                                {
                                    gp.AddString("NEW", FontFamily.GenericSansSerif, (int)FontStyle.Bold, 12f, rec,
                                        new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

                                    e.Graphics.DrawPath(pen, gp);
                                    e.Graphics.FillPath(Brushes.White, gp);
                                }
                                //e.Graphics.DrawString("NEW", f, Brushes.Gainsboro, rec,
                                //    new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                            }
                        }
                        break;
                }

                //e.Item.
                var text = hiderects[e.Item.Name].hidden ? "Hidden" : e.Item.Text;

                int w2 = e.Bounds.Height;
                e.Graphics.DrawString(text, listView1.Font, Brushes.Gainsboro,
                    new Rectangle(e.Bounds.X - w2 / 4, rec.Bottom, w2 * 2, w2),
                    new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near });
            }
            catch
            {
            }
        }

        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
            foreach (var rec in hiderects)
            {
                if (rec.Value.rec.Contains(e.Location))
                {
                    rec.Value.hidden = !rec.Value.hidden;
                    listView1.Refresh();
                    break;
                }
            }
        }

        private void LoadEvents()
        {
            //Task.Run(() => LoadTipeee());
            try
            {
                data.LoadTipeee();
                this.Invoke(new Action(SetEventControl));
            }
            catch
            {
            }
        }

        private void LoadSubs()
        {
            try
            {
                data.GetSubscriptions();
                this.Invoke(new Action(SetControls));
            }
            catch
            {
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            panel1MouseDown = true;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (panel1MouseDown)
            {
                Rectangle parent = this.ClientRectangle;
                int W = Math.Max(100, panelEventsResize.Left + e.X);
                int H = Math.Max(100, panelEventsResize.Top + e.Y);
                if (panelEvents.Left + W <= parent.Right)
                {
                    panelEvents.Width = W;
                }
                if (panelEvents.Top + H <= parent.Bottom)
                {
                    panelEvents.Height = H;
                }
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            panel1MouseDown = false;
        }

        private void SetControls()
        {
            try
            {
                var names = data.Chatters.Values;

                //Split Users at Type
                List<TreeNode> mods = new List<TreeNode>();
                List<TreeNode> cat = new List<TreeNode>();
                List<TreeNode> subs = new List<TreeNode>();
                foreach (var name in names)
                {
                    if (name.user.usertype >= UserType.moderator)
                    {
                        mods.Add(new TreeNode() { Name = name.user.name, Text = name.user.DisplayName + new string(' ', 32) });
                    }
                    else if (name.is_sub)
                    {
                        subs.Add(new TreeNode() { Name = name.user.name, Text = name.user.DisplayName + new string(' ', 32) });
                    }
                    else
                    {
                        cat.Add(new TreeNode() { Name = name.user.name, Text = name.user.DisplayName + new string(' ', 32) });
                    }
                }

                //Add User Listbox Nodes
                treeView1.BeginUpdate();
                treeView1.Nodes.Clear();

                treeView1.Nodes.Add(new TreeNode("Moderators " + (showUserNumbers ? "(" + mods.Count + ")" : ""), mods.ToArray()));
                treeView1.Nodes.Add(new TreeNode("Subscribers " + (showUserNumbers ? "(" + subs.Count + ")" : ""), subs.ToArray()));
                treeView1.Nodes.Add(new TreeNode("Chatters " + (showUserNumbers ? "(" + cat.Count + ")" : ""), cat.ToArray()));

                treeView1.EndUpdate();
                treeView1.ExpandAll();
                treeView1.Nodes[0].EnsureVisible();

                panelMain.Visible = true;
                pictureBox1.Visible = false;

                //Add Channel Info Data
                listView1.BeginUpdate();
                listView1.Items.Clear();
                AddToListView(data.SubCount, "sub");
                AddToListView(data.SubsSinceStreamStart, "newsubs");
                AddToListView(data.FollowCount, "follow");
                AddToListView(data.ChatterCount, "chatter");
                AddToListView(data.ViewerCount, "viewer");

                listView1.EndUpdate();

                timerRefresh.Enabled = true;
                timer1.Enabled = true;
            }
            catch
            {
            }
        }

        private void SetEventControl()
        {
            //listBox_SubEvents.BeginUpdate();
            //listBox_SubEvents.Items.Clear();
            //foreach (var n in data.ChatSubs)
            //{
            //    listBox_SubEvents.Items.Add(n.Name);
            //}
            //listBox_SubEvents.EndUpdate();
            UpdateBanList();

            textBox1.Text = (data.IsTipeeeEnabled) ? data.TipeeeKey : copTipTxt;
            splitContainer2.SplitterDistance = (data.IsTipeeeEnabled) ? 110 : 1;

            UpdateEventList();
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            //if (textBox1.Text.Equals(copTipTxt))
            //{
            //    textBox1.Clear();
            //}
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control && e.KeyCode == Keys.V))
            {
                textBox1.Clear();
            }
            else if (textBox1.Text.Equals(copTipTxt) && (!e.Control || e.KeyCode == Keys.V))
            {
                textBox1.Clear();
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.TextLength < 3)
            {
                textBox1.Text = copTipTxt;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            treeView1.Refresh();
            listView1.Refresh();
            if (ChatClient.Emotes.Badges.ContainsKey(data.User.Name))
            {
                if (ChatClient.Emotes.Badges.GetSubBadge(data.User.Name).IsLoaded)
                {
                    timer1.Enabled = false;
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            listView1.Refresh();
            treeView1.Refresh();
            timer2.Enabled = false;
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            this.Text = "Dashboard " + DateTime.Now.ToShortTimeString();
        }

        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            timerRefresh.Enabled = false;
            pictureBox1.Visible = true;
            pictureBox1.Dock = DockStyle.None;
            pictureBox1.Size = new Size(16, 16);
            pictureBox1.Location = new Point();
            Task.Run(() => LoadSubs());
            Task.Run(() => LoadEvents());
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            panelSettings.Visible = !panelSettings.Visible;
            panelSettings.BringToFront();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                //this.Invalidate();
                var name = e.Node.Name;
                if (data.Chatters.ContainsKey(name))
                {
                    userInfoPanel1.SetUser(data.User.ApiResult, data.Chatters[name].user);

                    data.Chatters[name].follow = LX29_Twitch.Api.TwitchApi.GetFollow(data.User.ID, data.Chatters[name].user._id);
                }

                this.Refresh();
            }
            catch (Exception x)
            {
                x.Handle();
            }
        }

        private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            try
            {
                //if (bgc == null) bgc = BufferedGraphicsManager.Current.Allocate(e.Graphics, e.Bounds);
                Graphics g = e.Graphics;
                var name = e.Node.Name;// listBox1.Items[e.Index].ToString();
                if (data.Chatters.ContainsKey(name))
                {
                    var sub = data.Chatters[name];
                    int height = e.Bounds.Height - 4;
                    int x = e.Bounds.X;
                    int y = e.Bounds.Y;
                    Rectangle rec = new Rectangle(x, y + 3, height, height);
                    g.SetGraphicQuality(true, true);

                    // g.FillRectangle(brushBG, e.Bounds);

                    try
                    {
                        if (ChatClient.Emotes.Badges.ContainsKey(data.User.Name))
                        {
                            if (sub.user.usertype >= UserType.moderator)
                            {
                                var img = ChatClient.Emotes.Badges.GetImage(sub.user.usertype);
                                img.Draw(g, rec);
                                x += height + 1;
                                rec.X = x;
                            }
                            if (sub.is_sub)
                            {
                                var img = ChatClient.Emotes.Badges.GetSubBadge(data.User.Name);
                                img.Draw(g, rec);
                                x += height + 1;
                                rec.X = x;
                            }
                            if (sub.is_follow)
                            {
                                g.DrawImage(LX29_LixChat.Properties.Resources.follow, rec);
                                x += height + 1;
                                rec.X = x;
                            }
                            //if(sub.user.)
                        }
                    }
                    catch { }

                    if (e.State > 0)
                    {
                        g.FillRectangle(SystemBrushes.Highlight, x, y, e.Bounds.Width, e.Bounds.Height);
                    }
                    //var username = sub.user.DisplayName != null ? sub.user.display_name : sub.user.name;
                    g.DrawString(sub.user.DisplayName, treeView1.Font, Brushes.Gainsboro, x, y);
                }
                else
                {
                    g.DrawString(e.Node.Text, treeView1.Font, Brushes.Gainsboro, e.Bounds.X, e.Bounds.Y);
                }
            }
            catch
            {
            }
        }

        private void treeView1_MouseClick(object sender, MouseEventArgs e)
        {
            //listView1.Invalidate();
            treeView1.Refresh();
        }

        private void treeView1_MouseLeave(object sender, EventArgs e)
        {
            timer2.Enabled = true;
        }

        private void UpdateBanList()
        {
            var bans = data.UserBans.OrderBy(t => DateTime.Now.Subtract(t.CreatedAt).TotalSeconds);
            listBox_BanEvents.BeginUpdate();
            listBox_BanEvents.Items.Clear();
            foreach (var n in bans)
            {
                listBox_BanEvents.Items.Add(n);
            }
            listBox_BanEvents.EndUpdate();
        }

        private void UpdateEventList()
        {
            var arr = data.DonationHosts.Select((a, i) => new { time = a.created_at, value = true, OBJECT = (object)a })
                .Concat(data.ChatSubs.Select((a, i) => new { time = a.CreatedAt, value = false, OBJECT = (object)a }))
                //.Concat(data.Subs.Select((a, i) => new
                //{
                //    time = a.created_at,
                //    value = false,
                //    index = (object)new NoticeMessage()
                //    {
                //        CreatedAt = a.created_at,
                //        Type = LX29_Twitch.Api.SubType.Prime, //change to actual subtype
                //        Message = a.sub_plan,
                //        Name = a.user.name
                //    }
                //}))
                .OrderByDescending(t => t.time.Ticks);

            listBox1.BeginUpdate();
            listBox1.Items.Clear();
            foreach (var ai in arr)
            {
                var a = new listboxitem() { Type = ai.value, Value = ai.OBJECT };
                listBox1.Items.Add(a);
            }
            listBox1.EndUpdate();

            label1.Text = "All Donate (" + data.DonationHosts.Count(t => !t.is_host) + ")\r\n" + data.DonationAmount.SizeMag(CultureInfo.CurrentCulture, 2, "C");
            label2.Text = "New Donate (" + data.DonationSinceStreamStart.Count() + ")\r\n" + data.DonationSinceStreamStart.Sum(t => t.amount).SizeMag(CultureInfo.CurrentCulture, 2, "C");
            label3.Text = "All Hosts (" + data.DonationHosts.Count(t => t.is_host) + ")\r\n" + data.HostAmount.SizeMag(CultureInfo.CurrentCulture, 2) + " Viewer";
            label4.Text = "New Hosts (" + data.HostSinceStreamStart.Count() + ")\r\n" + data.HostSinceStreamStart.Sum(t => t.amount).SizeMag(CultureInfo.CurrentCulture, 2) + " Viewer";
        }

        private class clickdata
        {
            public bool hidden { get; set; }
            public ListViewItem item { get; set; }
            public Rectangle rec { get; set; }
        }

        private class listboxitem
        {
            public bool Type { get; set; }
            public object Value { get; set; }
        }
    }
}