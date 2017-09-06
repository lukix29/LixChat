using System;
using System.Windows.Forms;

namespace LX29_LixChat
{
    internal static class Program
    {
        /// <summary>
        /// Der Hauptei nstiegspunkt für die Anwendung.
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
                x.Handle("", true);
                Application.Exit();
            }
        }
    }
}