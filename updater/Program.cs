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
                var procs = Process.GetProcesses();
                foreach (var proc in procs)
                {
                    string filename = Path.GetFileNameWithoutExtension(dest);
                    if (proc.ProcessName.Equals(filename))
                    {
                        proc.Kill();
                    }
                }
                File.Copy(source, dest, true);
                Process.Start(Path.GetFileName(dest));
                File.Delete(source);
            }
            catch (Exception x)
            {
                try
                {
                    Console.Clear();
                    Console.WriteLine(x.Message);
                    Console.WriteLine();
                    Console.WriteLine("Retry (y) or Abort (n)");
                    string resp = Console.ReadLine().Trim();
                    if (resp.StartsWith("y"))
                    {
                        Main(args);
                    }
                }
                catch (Exception xi)
                {
                    Console.WriteLine(xi.Message);
                    Console.WriteLine();
                    Console.WriteLine("Press any Key to exit.");
                    Console.ReadKey();
                }
            }
        }
    }
}