using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace updater
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                string source = args[0];
                string dest = args[1];
                var procs = Process.GetProcesses().Where(t => t.StartInfo.FileName.Equals("lixchat.exe", StringComparison.OrdinalIgnoreCase));
                foreach (var proc in procs)
                {
                    try
                    {
                        proc.Close();
                        proc.Kill();
                    }
                    catch
                    {
                    }
                }
                File.Copy(source, dest, true);
            }
            catch (Exception x)
            {
                try
                {
                    Console.Clear();
                    Console.WriteLine(x.ToString());
                    Console.WriteLine();
                    Console.WriteLine("Retry (y) or Abort (n)");
                    string resp = Console.ReadLine().Trim();
                    if (resp.StartsWith("y"))
                    {
                        Main(args);
                    }
                    else
                    {
                        Process.Start("LixChat.exe");
                    }//Main(args);
                }
                catch
                {
                }
            }
        }
    }
}