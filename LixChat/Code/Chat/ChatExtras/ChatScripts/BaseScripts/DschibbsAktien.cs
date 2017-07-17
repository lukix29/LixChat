using LX29_ChatClient;
using System;
using System.IO;
using System.Windows.Forms;

namespace InternalScripts
{
    public class DschibbsAktien
    {
        public static object LogDschibbs(ChatMessage Message)
        {
            if (Message.Name.Equals("inzpektorgadget", StringComparison.OrdinalIgnoreCase))
            {
                if (Message.Message.ContainsAny("Dschibbs", "zugeschaut"))
                {
                    File.AppendAllText(Settings.dataDir + "Dschibbs.txt", Message.Message + "\r\n");
                    return true;
                }
            }
            return false;
        }
    }
}