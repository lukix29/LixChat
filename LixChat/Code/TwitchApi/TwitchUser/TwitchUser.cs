﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace LX29_Twitch.Api
{
    public enum AddErrorInfo
    {
        None,
        Exists,
        NotExist,
        Error,
        CantBeEmpty,
    }

    public struct AddError
    {
        public readonly static AddError CantBeEmpty = new AddError(AddErrorInfo.CantBeEmpty, "");

        public readonly static AddError Error = new AddError(AddErrorInfo.Error, "");
        public readonly static AddError Exists = new AddError(AddErrorInfo.Exists, "");

        public readonly static AddError None = new AddError(AddErrorInfo.None, "");
        public readonly static AddError NotExist = new AddError(AddErrorInfo.NotExist, "");

        public readonly AddErrorInfo ErrorInfo;

        public readonly string Info;

        private static Dictionary<AddErrorInfo, string> AddErrorStrings = null;

        public AddError(AddErrorInfo error, string info)
        {
            try
            {
                if (AddErrorStrings == null)
                {
                    AddErrorStrings = new Dictionary<AddErrorInfo, string>()
                    {
                        {AddErrorInfo.None,"No Error."},
                        {AddErrorInfo.Exists,"Item already Exists in List!"},
                        {AddErrorInfo.NotExist,"Item doesn't Exist!"},
                        {AddErrorInfo.Error,"Error while adding."},
                        {AddErrorInfo.CantBeEmpty,"Item can't be Empty!"},
                    };
                }
                ErrorInfo = error;
                Info = AddErrorStrings[ErrorInfo];
                if (!string.IsNullOrEmpty(info))
                {
                    Info += "\r\n" + info;
                }
            }
            catch (Exception x)
            {
                ErrorInfo = AddErrorInfo.Error;
                Info = AddErrorStrings[ErrorInfo] + "\r\n" + x.Message;
            }
        }

        public static bool operator !=(AddError a, AddError b)
        {
            return a.ErrorInfo != b.ErrorInfo;
        }

        public static bool operator ==(AddError a, AddError b)
        {
            return a.ErrorInfo == b.ErrorInfo;
        }

        public override bool Equals(object obj)
        {
            return this == (AddError)obj;
        }

        public override int GetHashCode()
        {
            return this.ErrorInfo.GetHashCode();
        }
    }

    public class TwitchUser
    {
        public TwitchUser(string sessionID)
        {
            Selected = false;
            SessionID = sessionID;
            string tok = "";
            ApiResult = TwitchApi.GetUserIDFromSessionID(SessionID, out tok);
            if (string.IsNullOrEmpty(tok))
                throw new NullReferenceException("TwitchUser CTOR");
            Token = tok;
        }

        public ApiResult ApiResult
        {
            get;
            private set;
        }

        public int ID
        {
            get { return ApiResult.ID; }
        }

        public string Name
        {
            get { return ApiResult.Name; }
        }

        public bool Selected
        {
            get;
            set;
        }

        public string SessionID
        {
            get;
            private set;
        }

        public string Token
        {
            get;
            private set;
        }

        public static implicit operator ApiResult(TwitchUser user)
        {
            return user.ApiResult;
        }

        public T GetValue<T>(ApiInfo type)
        {
            return ApiResult.GetValue<T>(type);
        }

        public void SetToken(string token)
        {
            Token = token;
        }

        public override string ToString()
        {
            return (Selected ? "#" : "") + SessionID;
        }
    }

    public class TwitchUserCollection
    {
        private string filePath = "";
        private DateTime lastCheck = DateTime.MinValue;
        private List<TwitchUser> users = new List<TwitchUser>();

        public TwitchUserCollection(string FileName)
        {
            filePath = FileName;
        }

        public TwitchUser Selected
        {
            get { return users.Find(t => t.Selected); }
        }

        public List<TwitchUser> Values
        {
            get { return users; }
        }

        public TwitchUser this[int index]
        {
            get { return users[index]; }
        }

        public AddError Add(string sessionID)
        {
            try
            {
                if (!users.Any(t => t.SessionID.Equals(sessionID)))
                {
                    TwitchUser user = new TwitchUser(sessionID);
                    if (!users.Any(t => t.ID.Equals(user.ID)))
                    {
                        users.Add(user);
                        if (users.Count == 1)
                        {
                            users[0].Selected = true;
                        }
                        Save();
                        return AddError.None;
                    }
                }
            }
            catch (Exception x)
            {
                return new AddError(AddErrorInfo.Error, x.Message);
            }
            return AddError.Exists;
        }

        public bool CheckToken(bool reconnect)
        {
            try
            {
                if (DateTime.Now.Subtract(lastCheck).TotalSeconds < 5.0)
                {
                    return true;
                }
                string result = "";
                using (WebClient webclient = new WebClient())
                {
                    webclient.Proxy = null;
                    webclient.Encoding = Encoding.UTF8;

                    webclient.Headers.Add("Accept: application/vnd.twitchtv.v5+json");
                    webclient.Headers.Add("Client-ID: " + TwitchApi.CLIENT_ID);
                    result = webclient.DownloadString(
                        "https://api.twitch.tv/kraken?oauth_token=" + Selected.Token);
                }
                var res = LX29_Twitch.JSON_Parser.JSON.ParseAuth(result);
                if (!res.token.valid)
                {
                    RefreshSelectedToken(reconnect);
                }
                lastCheck = DateTime.Now;
                return res.token.valid;
            }
            catch
            {
                return false;
            }
        }

        public void FetchNewToken(System.Windows.Forms.Form Main, Action action = null, bool showBrowserSelector = false)
        {
            Main.Invoke(new Action(delegate()
                      {
                          Main.TopMost = true;
                          Main.BringToFront();
                      }));
            LX29_Twitch.Api.Controls.TokkenInput input = new LX29_Twitch.Api.Controls.TokkenInput();
            input.Dock = System.Windows.Forms.DockStyle.Fill;
            bool isdoun = false;
            input.OnTokenAbort += () =>
                {
                    Main.Invoke(new Action(() =>
                    {
                        Main.Controls.Remove(input);
                        Main.TopMost = true;
                        Main.BringToFront();
                        Main.TopMost = false;
                    }));
                };
            input.OnSessionIDReceived += (sessionID) =>
            {
                if (isdoun) return;
                isdoun = true;
                var result = Add(sessionID);
                if (result == AddError.None)
                {
                    if (action != null)
                    {
                        Task.Run(action);
                    }
                    Main.Invoke(new Action(() =>
                    {
                        Main.Controls.Remove(input);
                        Main.TopMost = true;
                        Main.BringToFront();
                        Main.TopMost = false;
                    }));
                }
                else
                {
                    var t = System.Windows.Forms.LX29_MessageBox.Show(result.Info, "Error", System.Windows.Forms.MessageBoxButtons.RetryCancel);

                    Main.Invoke(new Action(() => Main.Controls.Remove(input)));

                    if (t == System.Windows.Forms.MessageBoxResult.Retry)
                    {
                        FetchNewToken(Main, action, showBrowserSelector);
                    }
                }
            };
            Main.Invoke(new Action(async () =>
            {
                Main.Controls.Add(input);
                input.BringToFront();
                await Task.Delay(1000);
                Main.TopMost = false;
            }));
        }

        public async void Load(System.Windows.Forms.Form Main, Action action = null)
        {
            while (!LX29_Tools.HasInternetConnection) await Task.Delay(1000);
            var err = Load();
            if (err == AddError.None)
            {
                if (action != null)
                {
                    await Task.Run(action);
                }
            }
            else if (err == AddError.Error)
            {
                if (err.Info.Contains("NullReferenceException: GetUserIDFromSessionID"))
                {
                    FetchNewToken(Main, action, true);
                    return;
                }
                throw new Exception(err.Info);
            }
            else
            {
                FetchNewToken(Main, action, true);
            }
        }

        public void RefreshSelectedToken(bool reconnect)
        {
            try
            {
                string tok = TwitchApi.TokenFromSessionID(Selected.SessionID);
                if (string.IsNullOrEmpty(tok))
                    throw new NullReferenceException();
                Selected.SetToken(tok);
                Save();
                if (reconnect)
                {
                    Task.Run(() => LX29_ChatClient.ChatClient.Reconnect());
                }
            }
            catch (Exception x)
            {
                x.Handle("", true);
            }
        }

        public AddError Remove(Predicate<TwitchUser> predicate)
        {
            if (users.Count - 1 > 0)
            {
                var user = users.FindIndex(0, predicate);
                if (user >= 0)
                {
                    users.RemoveAt(user);
                    return AddError.None;
                }
                return AddError.NotExist;
            }
            return AddError.CantBeEmpty;
        }

        public void Save()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var user in users)
            {
                sb.AppendLine(user.ToString());
            }
            File.WriteAllBytes(filePath, LX29_Crypt.LX29Crypt.Encrypt(sb.ToString()));
        }

        public AddError SetSelected(Func<TwitchUser, bool> predicate)
        {
            var idx = users.FindIndex(t => t.Selected);
            var user = users.FirstOrDefault(predicate);
            if (user != null)
            {
                users[idx].Selected = false;
                user.Selected = true;
                Save();
                return AddError.None;
            }
            return AddError.NotExist;
        }

        private AddError Load()
        {
            if (File.Exists(filePath))
            {
                string si = LX29_Crypt.LX29Crypt.Decrypt(File.ReadAllBytes(filePath));
                var sa = si.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (sa.Length > 0)
                {
                    try
                    {
                        foreach (var line in sa)
                        {
                            TwitchUser user = new TwitchUser(line.Trim('#'));
                            if (line.StartsWith("#"))
                            {
                                user.Selected = true;
                            }
                            users.Add(user);
                        }
                        return AddError.None;
                    }
                    catch (Exception x)
                    {
                        return new AddError(AddErrorInfo.Error, x.ToString());
                    }
                }
                File.Delete(filePath);
            }
            return AddError.NotExist;
        }
    }
}