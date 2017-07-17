using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        //public readonly static AddError Error = new AddError(AddErrorInfo.None);
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
        public TwitchUser(string token)
        {
            Selected = false;
            Token = token;
            ApiResult = TwitchApi.GetUserIDFromToken(token);
        }

        public ApiResult ApiResult
        {
            get;
            private set;
        }

        public string ID
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

        public override string ToString()
        {
            return (Selected ? "#" : "") + Token;
        }
    }

    public class TwitchUserCollection
    {
        private string filePath = "";
        private List<TwitchUser> users = new List<TwitchUser>();

        public TwitchUserCollection(string FileName)
        {
            filePath = FileName;
        }

        //public delegate void FetchedToken(string token);

        //public event FetchedToken OnFetchedToken;

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

        public AddError Add(string token)
        {
            try
            {
                if (!users.Any(t => t.Token.Equals(token)))
                {
                    TwitchUser user = new TwitchUser(token);
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
            input.OnTokenReceived += (token) =>
            {
                if (isdoun) return;
                isdoun = true;
                var result = Add(token);
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
                await Task.Delay(2000);
                Main.TopMost = false;
            }));
        }

        public void Load(System.Windows.Forms.Form Main, Action action = null)
        {
            if (Load() == AddError.None)
            {
                if (action != null)
                {
                    Task.Run(action);
                }
            }
            else
            {
                FetchNewToken(Main, action, true);
            }
        }

        public AddError Load()
        {
            if (File.Exists(filePath))
            {
                var sa = File.ReadAllLines(filePath);
                if (sa.Length > 0)
                {
                    try
                    {
                        foreach (var line in sa)
                        {
                            //Stream aufnahme funktion!! mal machen halt
                            TwitchUser user = new TwitchUser(line.Trim('#'));
                            if (line.StartsWith("#"))
                            {
                                user.Selected = true;
                            }
                            users.Add(user);
                        }
                        return AddError.None;
                    }
                    catch { }
                }
                File.Delete(filePath);
            }
            return AddError.NotExist;
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
                if (user.Selected) sb.Append("#");
                sb.AppendLine(user.ToString());
            }
            File.WriteAllText(filePath, sb.ToString());
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
    }
}