using System;
using System.Windows.Forms;

namespace LX29_TwitchChat
{
    internal static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Main());
            }
            catch (Exception x)
            {
                MessageBox.Show(x.ToString());
            }
        }
    }
}