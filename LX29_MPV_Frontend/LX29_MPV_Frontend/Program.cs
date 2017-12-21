using System;
using System.Windows.Forms;

namespace mpv
{
    internal static class Program
    {
        private static int exitCode = 0;

        public static int ExitCode
        {
            get { return exitCode; }
            set { exitCode = value; }
        }

        public static void ExitApplication(int exitCode)
        {
            Program.exitCode = exitCode;
            Application.Exit();
        }

        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        private static int Main(params string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(args));
            return exitCode;
        }
    }
}