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
                switch (x.Handle("", true))// LX29_MessageBox.Show(x.ToString(),"Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error))
                {
                    case MessageBoxResult.Retry:
                        System.Diagnostics.Process.Start(Application.ExecutablePath);
                        Application.Exit();
                        break;
                }
            }
        }
    }
}