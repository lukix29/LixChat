using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LX29_ChatClient.Dashboard
{
    public partial class FormDashboard : Form
    {
        private SolidBrush brushBG = new SolidBrush(Color.FromArgb(40, 40, 40));
        private DashboardData data = new DashboardData();

        private Dictionary<string, clickdata> hiderects = new Dictionary<string, clickdata>();

        private int refreshCount = 0;

        private bool showUserNumbers = true;

        public FormDashboard()
        {
            InitializeComponent();
        }

        private void AddToListView(int value, string name)
        {
            if (listView1.LargeImageList == null) listView1.LargeImageList = new ImageList();

            listView1.LargeImageList.Images.Add(name, new Bitmap(40, 40, System.Drawing.Imaging.PixelFormat.Format1bppIndexed));

            var item = new ListViewItem(value.SizeMag(2), name) { Name = name };

            if (!hiderects.ContainsKey(name))
            {
                hiderects.Add(item.Name, new clickdata() { rec = new Rectangle(), item = item, hidden = false });
            }

            listView1.Items.Add(item);
        }

        private void FormDashboard_Load(object sender, EventArgs e)
        {
            pictureBox1.Visible = true;
            pictureBox1.BringToFront();
            pictureBox1.Dock = DockStyle.Fill;

            treeView1.DrawMode = TreeViewDrawMode.OwnerDrawText;
            treeView1.DrawNode += treeView1_DrawNode;

            listView1.OwnerDraw = true;
            listView1.DrawItem += listView1_DrawItem;

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
            showUserNumbers = !showUserNumbers;
            hideNumbersToolStripMenuItem.Text = showUserNumbers ? "Hide Numbers" : "Show Numbers";
            SetControls();
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
                            if (canDraw) b.Draw(e.Graphics, rec, Emotes.EmoteImageSize.Large);
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
                            if (canDraw) b.Draw(e.Graphics, rec, Emotes.EmoteImageSize.Large);
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
                        mods.Add(new TreeNode() { Name = name.user.name, Text = name.user.DisplayName });
                    }
                    else if (name.is_sub)
                    {
                        subs.Add(new TreeNode() { Name = name.user.name, Text = name.user.DisplayName });
                    }
                    else
                    {
                        cat.Add(new TreeNode() { Name = name.user.name, Text = name.user.DisplayName });
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            treeView1.Invalidate();
            listView1.Invalidate();
            if (ChatClient.Emotes.Badges.ContainsKey(data.User.Name))
            {
                timer1.Enabled = false;
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            listView1.Invalidate();
            treeView1.Invalidate();
            if (refreshCount++ >= 5)
            {
                timer2.Enabled = false;
                refreshCount = 0;
            }
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
            pictureBox1.Size = new Size(128, 128);
            pictureBox1.Location = new Point(this.ClientSize.Width / 2 - pictureBox1.Width / 2, this.ClientSize.Height / 2 - pictureBox1.Height / 2);
            Task.Run(() => LoadSubs());
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                var name = e.Node.Name;
                if (data.Chatters.ContainsKey(name))
                {
                    userInfoPanel1.SetUser(data.User.ApiResult, data.Chatters[name].user);

                    data.Chatters[name].follow = LX29_Twitch.Api.TwitchApi.GetFollow(data.User.ID, data.Chatters[name].user._id);
                }
            }
            catch (Exception x)
            {
                x.Handle();
            }
        }

        private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            var name = e.Node.Name;// listBox1.Items[e.Index].ToString();
            if (data.Chatters.ContainsKey(name))
            {
                var sub = data.Chatters[name];
                int height = e.Bounds.Height - 4;
                int x = e.Bounds.X;
                int y = e.Bounds.Y;
                Rectangle rec = new Rectangle(x, y + 3, height, height);
                e.Graphics.SetGraphicQuality(true, true);

                e.Graphics.FillRectangle(brushBG, e.Bounds);

                try
                {
                    if (ChatClient.Emotes.Badges.ContainsKey(data.User.Name))
                    {
                        if (sub.user.usertype >= UserType.moderator)
                        {
                            var img = ChatClient.Emotes.Badges.GetImage(sub.user.usertype);
                            img.Draw(e.Graphics, rec, Emotes.EmoteImageSize.Large);
                            x += height + 1;
                            rec.X = x;
                        }
                        if (sub.is_sub)
                        {
                            var img = ChatClient.Emotes.Badges.GetSubBadge(data.User.Name);
                            img.Draw(e.Graphics, rec, Emotes.EmoteImageSize.Large);
                            x += height + 1;
                            rec.X = x;
                        }
                        if (sub.is_follow)
                        {
                            e.Graphics.DrawImage(LX29_LixChat.Properties.Resources.follow, rec);
                            x += height + 1;
                            rec.X = x;
                        }
                        //if(sub.user.)
                    }
                }
                catch { }

                if (e.State > 0)
                {
                    e.Graphics.FillRectangle(SystemBrushes.Highlight, x, y, e.Bounds.Width, e.Bounds.Height);
                }
                //var username = sub.user.DisplayName != null ? sub.user.display_name : sub.user.name;
                e.Graphics.DrawString(sub.user.DisplayName, treeView1.Font, Brushes.Gainsboro, x, y);
            }
            else
            {
                e.Graphics.DrawString(e.Node.Text, treeView1.Font, Brushes.Gainsboro, e.Bounds.X, e.Bounds.Y);
            }
        }

        private void treeView1_MouseClick(object sender, MouseEventArgs e)
        {
            listView1.Invalidate();
            treeView1.Invalidate();
        }

        private void treeView1_MouseLeave(object sender, EventArgs e)
        {
            timer2.Enabled = true;
        }

        private class clickdata
        {
            public bool hidden { get; set; }
            public ListViewItem item { get; set; }
            public Rectangle rec { get; set; }
        }
    }
}