using System;
using LX29_ChatClient;
using LX29_ChatClient.Channels;
using LX29_ChatClient.Emotes;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Reflection;

namespace InternalScripts
{
    public class CommandScripts
    {
        //private static HashSet<string> permits = new HashSet<string>();

        //public static object Exit(ChatMessage Message)
        //{
        //    if (Message.IsType(MsgType.Outgoing))
        //    {
        //        switch (Message.ChatWords[0].Text.ToLower())
        //        {
        //            case "!exit":
        //                {
        //                    switch (Message.ChatWords[1].Text.ToLower())
        //                    {
        //                        case "app":
        //                            Application.Exit();
        //                            return true;

        //                        case "chat":
        //                            ChatClient.Channels[Message.Channel].CloseChat();
        //                            return true;
        //                    }
        //                }
        //                break;

        //            case "!start":
        //                {
        //                }
        //                break;
        //        }
        //    }
        //    return false;
        //}

        //public static object Permit(ChatMessage Message)
        //{
        //    if (Message.User != null && !Message.User.IsEmpty)
        //    {
        //        if (Message.User.IsType(UserType.moderator))
        //        {
        //            if (Message.Words[0].Equals("!permit", StringComparison.OrdinalIgnoreCase))
        //            {
        //                string name = Message.Words[1];
        //                if (!permits.Contains(name))
        //                {
        //                    permits.Add(name);
        //                    return true;
        //                }
        //                else
        //                {
        //                    //Show that user is already permitted
        //                }
        //            }
        //        }
        //        else if (permits.Contains(Message.User.Name))
        //        {
        //            if (LX29_Tools.IsLink(Message.Message)) //Message.Words.Any(t=>LX29_Tools.IsLink(t))
        //            {
        //                permits.Remove(Message.User.Name);
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}
    }
}