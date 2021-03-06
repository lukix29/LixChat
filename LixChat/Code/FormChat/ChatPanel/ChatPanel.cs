﻿using LX29_Twitch.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LX29_ChatClient.Forms
{
    public partial class ChatPanel : UserControl
    {
        private int lastMessageScrollIndex = -1;

        private string lastSearch = "";

        private bool lockListSearch = false;

        //private Dictionary<string, EmoteSearchResult> searchResult = new Dictionary<string, EmoteSearchResult>();

        //private Task searchTask = null;

        //private bool stopTask = false;

        //private int lastMessageScrollMax = 0;
        //private List<ChatMessage> sentMessages = new List<ChatMessage>();
        public ChatPanel()
        {
            InitializeComponent();
            lstB_Search.DrawMode = DrawMode.OwnerDrawFixed;
        }

        public delegate void WhisperSent(object sender, ChatMessage message);

        public event WhisperSent OnWhisperSent;

        public ApiResult Channel
        {
            get { return this.chatView1.Channel; }
        }

        public ChatView ChatView
        {
            get { return this.chatView1; }
        }

        [ReadOnly(true)]
        [Browsable(false)]
        public bool Pause
        {
            get { return chatView1.Pause; }
            set { chatView1.Pause = value; }
        }

        //public bool ShowInput
        //{
        //    get { return rTB_Send.Visible; }
        //    set
        //    {
        //        rTB_Send.Visible = value;
        //        if (!value)
        //        {
        //            chatView1.Anchor = AnchorStyles.None;
        //            chatView1.Dock = DockStyle.Fill;
        //        }
        //        else
        //        {
        //            chatView1.Dock = DockStyle.None;
        //            chatView1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
        //        }
        //    }
        //}

        //public bool SmallInput
        //{
        //    get { return smallInput; }
        //    set
        //    {
        //        smallInput = value;
        //        if (smallInput)
        //        {
        //            rTB_Send.MinimumSize = new Size(50, 25);
        //            rTB_Send.MaximumSize = new Size(Int16.MaxValue, 25);
        //            rTB_Send.Height = 25;
        //        }
        //        else
        //        {
        //            rTB_Send.MinimumSize = new Size(50, 50);
        //            rTB_Send.MaximumSize = new Size(Int16.MaxValue, 50);
        //            rTB_Send.Height = 50;
        //        }
        //    }
        //}

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Tab)
            {
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private async void btn_Send_Click(object sender, EventArgs e)

        {
            try
            {
                if (rTB_Send.TextLength > 0)
                {
                    var text = rTB_Send.Text;
                    rTB_Send.Clear();

                    var msg = ChatMessage.Empty;

                    await System.Threading.Tasks.Task.Run(() =>
                     {
                         if (string.IsNullOrEmpty(chatView1.UserMessageName))
                         {
                             msg = ChatClient.SendMessage(text, Channel.ID);
                         }
                         else
                         {
                             msg = ChatClient.SendWhisper(text, chatView1.UserMessageName);
                         }
                     });

                    if (!msg.IsEmpty)
                    {
                        if (msg.IsType(MsgType.Whisper))
                        {
                            OnWhisperSent?.Invoke(this, msg);
                        }
                    }
                }
            }
            catch (Exception x)
            {
                x.Handle("Error while Sending Message!", true);
            }
        }

        private void ChatHistory(KeyEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(lastSearch)) return;

                e.SuppressKeyPress = true;

                var sa = ChatClient.Messages.Get(MsgType.Outgoing, Channel.Name);
                var list = sa.OrderBy(a => a.SendTime.Ticks).Reverse().ToList();
                int cnt = list.Count();
                if (cnt > 0)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.Down:
                            {
                                lastMessageScrollIndex--;
                                if (lastMessageScrollIndex < 0)
                                {
                                    rTB_Send.Clear();
                                    lastMessageScrollIndex = -1;
                                }
                                else
                                {
                                    lastMessageScrollIndex = Math.Max(0, lastMessageScrollIndex);

                                    rTB_Send.Text = list.ElementAtOrDefault(lastMessageScrollIndex).Message;
                                }
                            }
                            break;

                        case Keys.Up:
                            {
                                lastMessageScrollIndex++;
                                lastMessageScrollIndex = Math.Min(cnt - 1, lastMessageScrollIndex);
                                rTB_Send.Text = list[lastMessageScrollIndex].Message;
                            }
                            break;
                    }
                }
            }
            catch
            {
            }
        }

        private void ChatPanel_Load(object sender, EventArgs e)
        {
            try
            {
                chatView1.OnLinkClicked += chatView1_OnLinkClicked;
                chatView1.OnUserNameClicked += chatView1_OnUserNameClicked;
                chatView1.OnEmoteClicked += chatView1_OnEmoteClicked;

                userInfoPanel1.Font = new Font(chatView1.Font.FontFamily, 10);

                rTB_Send.AddContextMenu();
            }
            catch (Exception x)
            {
                x.Handle();
            }
        }

        private void chatView1_MouseDown(object sender, MouseEventArgs e)

        {
            userInfoPanel1.Hide();
        }

        private void chatView1_MouseUp(object sender, MouseEventArgs e)

        {
            lstB_Search.Visible = false;
            lastSearch = "";
        }

        private void chatView1_OnEmoteClicked(ChatView sender, Emotes.Emote emote)

        {
            rTB_Send.AppendText(" " + emote.Name);
        }

        private void chatView1_OnLinkClicked(ChatView sender, string url)

        {
            Settings.StartBrowser(url);
        }

        private void chatView1_OnUserNameClicked(ChatView sender, ChatUser user, Point mouse)

        {
            Point location = this.PointToClient(mouse);
            userInfoPanel1.Show(sender.Channel, user, location);
        }

        private void lstB_Search_DrawItem(object sender, DrawItemEventArgs e)

        {
            try
            {
                if (e.Index < 0) return;

                e.DrawBackground();
                e.DrawFocusRectangle();

                int y = e.Bounds.Y;

                Rectangle rec = new Rectangle(0, y, e.Bounds.Height, e.Bounds.Height);
                if (lstB_Search.ClientRectangle.IntersectsWith(rec))
                {
                    int x = 0;
                    var res = (ChatClient.EmoteSearchResult)lstB_Search.Items[e.Index];
                    if (res.IsEmote)
                    {
                        var em = (Emotes.IEmoteBase)res.Result;
                        x += 5;
                        em.Draw(e.Graphics, x, y, e.Bounds.Height, e.Bounds.Height);
                        x += 5 + e.Bounds.Height;
                    }
                    TextRenderer.DrawText(e.Graphics, res.Name, e.Font, new Point(x, y), e.ForeColor);
                }
            }
            catch { }
        }

        private void lstB_Search_MouseDoubleClick(object sender, MouseEventArgs e)

        {
            lstB_Search.SuspendLayout();
            selectName();
            lstB_Search.Visible = false;
            rTB_Send.Focus();
            rTB_Send.Select(rTB_Send.TextLength, 0);
            lstB_Search.ResumeLayout();
        }

        private void lstB_Search_SelectedIndexChanged(object sender, EventArgs e)

        {
            if (!lockListSearch)
            {
                lstB_Search.SuspendLayout();
                selectName();
                lstB_Search.Visible = false;
                rTB_Send.Focus();
                rTB_Send.Select(rTB_Send.TextLength, 0);
                lstB_Search.ResumeLayout();
            }
        }

        private void lstB_Search_VisibleChanged(object sender, EventArgs e)

        {
            if (!lstB_Search.Visible)
            {
                lastSearch = "";
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)

        {
            //rTB_Send.SuspendLayout();
            //string text = rTB_Send.Text;
            //int sel = rTB_Send.SelectionStart;
            //rTB_Send.SelectAll();
            //rTB_Send.SelectedText = text.RemoveControlChars();
            //rTB_Send.SelectionStart = sel;
            //rTB_Send.ResumeLayout();
        }

        private void rTB_Send_KeyDown(object sender, KeyEventArgs e)

        {
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
            {
                if (lstB_Search.Visible)
                {
                    if (lstB_Search.Items.Count > 0)
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.Down:
                                if (lstB_Search.SelectedIndex + 1 >= lstB_Search.Items.Count)
                                    lstB_Search.SelectedIndex = 0;
                                else lstB_Search.SelectedIndex++;
                                break;

                            case Keys.Up:
                                if (lstB_Search.SelectedIndex - 1 < 0)
                                    lstB_Search.SelectedIndex = lstB_Search.Items.Count - 1;
                                else lstB_Search.SelectedIndex--;
                                break;
                        }
                    }
                    else
                    {
                        lastSearch = "";
                        //searchResult.Clear();
                    }
                }
                else
                {
                    lastSearch = "";
                    //searchResult.Clear();
                    ChatHistory(e);
                }
            }
        }

        private void rTB_Send_KeyUp(object sender, KeyEventArgs e)

        {
            try
            {
                if (e.KeyData == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    if (!string.IsNullOrEmpty(lastSearch))
                    {
                        selectName();
                    }
                    else
                    {
                        btn_Send.PerformClick();
                    }
                    lastSearch = "";
                    //if (searchResult != null) searchResult.Clear();
                }
                else if (e.KeyData == Keys.Tab)
                {
                    lockListSearch = true;
                    if (string.IsNullOrEmpty(lastSearch))
                    {
                        e.SuppressKeyPress = true;

                        var arr = rTB_Send.Text.Trim().Split(" ");
                        lastSearch = arr.Select(t => t.Trim()).Last();

                        if (lastSearch.Length > 2)
                        {
                            Task.Run(() => SearchFor());
                        }
                    }
                    else
                    {
                        if (lstB_Search.Visible)
                        {
                            if (lstB_Search.Items.Count == 1)
                            {
                                selectName();
                            }
                            else
                            {
                                if (lstB_Search.SelectedIndex + 1 >= lstB_Search.Items.Count)
                                    lstB_Search.SelectedIndex = lstB_Search.Items.Count == 0 ? -1 : 0;
                                else lstB_Search.SelectedIndex++;
                            }
                        }
                        else
                        {
                            lastSearch = "";
                        }
                    }
                }
                else if (e.KeyData == Keys.Space)
                {
                    selectName();
                }
                else if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
                {
                }
                else
                {
                    timer1.Interval = 1000;
                    lastMessageScrollIndex = -1;
                    lastSearch = "";
                    // if (searchResult != null) searchResult.Clear();
                    lstB_Search.Visible = false;
                }
            }
            catch (Exception x)
            {
                switch (x.Handle())
                {
                    case MessageBoxResult.Retry:
                        rTB_Send_KeyUp(sender, e);
                        break;
                }
            }
        }

        private List<ChatClient.EmoteSearchResult> Search(string s, int cnt = 0)
        {
            try
            {
                if (cnt >= 5)
                    return null;
                string search = s.ToLower().Trim();

                //System.Text.StringBuilder sb = new System.Text.StringBuilder();
                //var watch = new System.Diagnostics.Stopwatch();
                //watch.Start();

                var userTask = ChatClient.ChatUsers.Find(search, Channel.Name);

                //sb.AppendLine("Users: " + watch.ElapsedMilliseconds.ToString("N0"));
                //watch.Restart();

                var emoteTask = ChatClient.Emotes.Values.Find(s, Channel.Name)
                    .Select(t => new ChatClient.EmoteSearchResult(t, t.Name, true));

                //sb.AppendLine("Emotes: " + watch.ElapsedMilliseconds.ToString("N0"));
                //watch.Restart();

                userTask.AddRange(emoteTask);

                //sb.AppendLine("Concat: " + watch.ElapsedMilliseconds.ToString("N0"));
                //watch.Restart();

                //MessageBox.Show(sb.ToString());
                return userTask;
            }
            catch
            {
                return Search(s, cnt + 1);
            }
        }

        private void SearchFor()
        {
            try
            {
                //var watch = new System.Diagnostics.Stopwatch();
                //var sb = new System.Text.StringBuilder();
                //watch.Start();

                var searchResult = Search(lastSearch.ToLower());

                //sb.AppendLine(watch.ElapsedMilliseconds.ToString("N0") + "ms Search");
                //watch.Restart();

                this.Invoke(new Action(() =>
                {
                    try
                    {
                        if (searchResult != null)
                        {
                            lstB_Search.BeginUpdate();
                            lstB_Search.Items.Clear();
                            foreach (var result in searchResult)
                            {
                                if (result.IsEmpty)
                                {
                                    continue;
                                }
                                //sb.AppendLine(watch.ElapsedMilliseconds.ToString("N0") + "ms " + result.Name);
                                //watch.Restart();

                                if (result.Result is Emotes.Emote em)
                                {
                                    if (em.IsGif)
                                    {
                                        if (Settings.AnimateGifInSearch) timer1.Interval = 100;
                                    }
                                }
                                lstB_Search.Items.Add(result);
                            }
                            //sb.AppendLine(watch.ElapsedMilliseconds.ToString("N0") + "ms");
                            //watch.Restart();

                            lstB_Search.SelectedIndex = 0;
                            if (searchResult.Count() == 1)
                            {
                                selectName();
                            }
                            else
                            {
                                lstB_Search.Visible = true;
                                lstB_Search.Location = new Point(0, chatView1.Bottom - lstB_Search.Height);
                            }
                        }
                    }
                    catch
                    {
                    }
                    finally
                    {
                        lstB_Search.EndUpdate();
                        lstB_Search.Invalidate();
                    }
                }));
                //sb.AppendLine(watch.ElapsedMilliseconds.ToString("N0") + "ms");
                //MessageBox.Show(sb.ToString());
            }
            catch
            {
            }
            finally
            {
                lockListSearch = false;
            }
        }

        private void selectName()

        {
            try
            {
                if ((lstB_Search.SelectedIndex >= 0 && !string.IsNullOrEmpty(lastSearch)))
                {
                    var text = rTB_Send.Text.Trim();
                    var repl = (ChatClient.EmoteSearchResult)lstB_Search.SelectedItem;
                    if (!repl.IsEmpty)
                    {
                        text = text.Replace(lastSearch, repl.Name) + " ";

                        timer1.Interval = 1000;
                        rTB_Send.Clear();
                        rTB_Send.AppendText(text);
                        lstB_Search.Visible = false;
                        rTB_Send.Focus();
                        rTB_Send.Select(rTB_Send.TextLength - 1, 0);
                    }
                }
            }
            finally
            {
                lastSearch = "";
            }
        }

        private void SetMsgType(ToolStripButton tsb, MsgType MessageType)
        {
            try
            {
                //string nme = tsb.Name;
                //if (Enum.TryParse<MsgType>(nme, out MessageType))
                //{
                chatView1.SetAllMessages(MessageType);
                //}
                //else
                //{
                //    chatView1.SetAllMessages(MsgType.Whisper, null, nme);
                //}
                chatView1.ViewAllEmotes = false;
                // if ()
                // {
                //     item.BackColor = UserColors.ChatBackground;
                //     item.ForeColor = Color.FromArgb(250, 50, 50);
                // }
                //else
                // {
                tsb.BackColor = UserColors.ChatBackground;
                tsb.ForeColor = Color.White;
                //}
                chatView1.RefreshMessages();
            }
            catch
            {
            }
        }

        //private void SearchFor()
        //{
        //    if (searchTask != null && !searchTask.IsCompleted)
        //    {
        //        stopTask = true;
        //    }
        //    searchTask = Task.Run(() =>
        //      {
        //          try
        //          {
        //              if (stopTask)
        //              {
        //                  lstB_Search.Items.Clear();
        //                  return;
        //              }
        //              var searchResult = search(lastSearch.ToLower());
        //              if (searchResult != null)
        //              {
        //                  this.Invoke(new Action(() =>
        //                  {
        //                      try
        //                      {
        //                          lstB_Search.Items.Clear();
        //                          foreach (var result in searchResult)
        //                          {
        //                              if (stopTask)
        //                              {
        //                                  lstB_Search.Items.Clear();
        //                                  return;
        //                              }
        //                              if (result.Result is Emotes.Emote)
        //                              {
        //                                  var em = (Emotes.Emote)result.Result;
        //                                  if (em.IsGif)
        //                                  {
        //                                      if (Settings.AnimateGifInSearch) timer1.Interval = 200;
        //                                  }
        //                              }
        //                              lstB_Search.Items.Add(result);
        //                          }
        //                          lstB_Search.SelectedIndex = 0;
        //                          if (searchResult.Count() == 1)
        //                          {
        //                              selectName();
        //                          }
        //                          else
        //                          {
        //                              lstB_Search.Visible = true;
        //                              lstB_Search.Location = new Point(0, chatView1.Bottom - lstB_Search.Height);
        //                          }
        //                      }
        //                      catch
        //                      {
        //                      }
        //                  }));
        //                  if (stopTask)
        //                  {
        //                      lstB_Search.Visible = false;
        //                      lstB_Search.Items.Clear();
        //                      return;
        //                  }
        //              }
        //          }
        //          finally
        //          {
        //              lockListSearch = false;
        //              stopTask = false;
        //          }
        //      });
        //}

        private void timer1_Tick(object sender, EventArgs e)

        {
            try
            {
                if (lstB_Search.Visible)
                    lstB_Search.Invalidate();

                if (Channel != null)
                {
                    var chan = ChatClient.Channels[Channel.ID];
                    if (chan.HasSlowMode)
                    {
                        TimeSpan ts = DateTime.Now.Subtract(chan.LastSendMessageTime);
                        int tts = chan.SlowMode - (int)ts.TotalSeconds;

                        if (tts <= chan.SlowMode && tts > 0)
                        {
                            btn_Send.Text = (chan.SlowMode - (int)ts.TotalSeconds) + "s";// > 0 ? @"{mm\:ss}" : @"{ss}", ts);
                            return;
                        }
                    }
                    btn_Send.Text = "Chat";
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Highlights
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click(object sender, EventArgs e)

        {
            SetMsgType(toolStripButton2, MsgType.HL_Messages);
        }

        /// <summary>
        /// All MEssages
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton2_Click(object sender, EventArgs e)

        {
            SetMsgType(toolStripButton2, MsgType.All_Messages);
        }

        /// <summary>
        /// Outgoing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton3_Click(object sender, EventArgs e)

        {
            SetMsgType(toolStripButton2, MsgType.Outgoing);
        }

        /// <summary>
        /// Emotes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tS_Btn_Emotes_Click(object sender, EventArgs e)

        {
            chatView1.ViewAllEmotes = true;
        }
    }
}