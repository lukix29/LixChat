using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace LX29_Crypt
{
    public static class PermutClass
    {
        public static IEnumerable<string> GetPermutations(bool removeNoneChars = false, params string[] input)
        {
            HashSet<string> list = new HashSet<string>();
            Parallel.ForEach<string>(input, new Action<string>(delegate(string si) //for (int i = 4; i <= 8; i++)
            {
                try
                {
                    List<string> temp = new List<string>();
                    if (si.Length > 7)
                    {
                        temp.Add(si.Remove(7));
                        temp.Add(si.Substring(7));
                    }
                    else temp.Add(si);

                    Parallel.ForEach<string>(temp, new Action<string>(delegate(string siii)
                    {
                        var sa = getPermutations(siii, removeNoneChars);
                        sa = sa.Where(s => (!list.Contains(s)));
                        Parallel.ForEach<string>(sa, new Action<string>(delegate(string sii)
                        {
                            try
                            {
                                if (!string.IsNullOrEmpty(sii) && !list.Contains(sii))
                                {
                                    list.Add(sii);
                                }
                            }
                            catch { }
                        }));
                    }));
                }
                catch { }
            }));
            return list;
        }

        //public static int CalcCount(string input)
        //{
        //    int x = input.Length - 1;
        //    int count = 0;
        //    int calcount = 0;
        //    Action<int, int> a = null;
        //    a = new Action<int, int>(delegate(int k, int m)
        //    {
        //        if (k == m)
        //        {
        //            count++;
        //        }
        //        else
        //        {//int i = k; i <= m; i++
        //            Parallel.For(k, m + 1, new Action<int>(delegate(int i)
        //            {
        //                //Swap(ref list[k], ref list[i]);
        //                a.Invoke(k + 1, m);
        //                //Swap(ref list[k], ref list[i]);
        //                calcount++;
        //            }));
        //        }
        //    });
        //    a.Invoke(0, x);
        //    return count;
        //}
        private static IEnumerable<string> getPermutations(string input, bool removeNonChars)
        {
            string name;
            if (removeNonChars)
                name = new string(input.Where(Char.IsLetter).ToArray());
            else name = input;

            if (name.Length > 7)
            {
                name = name.Remove(7);
            }

            int x = name.Length - 1;

            HashSet<string> permuts = new HashSet<string>();
            Action<char[], int, int> a = null;
            a = new Action<char[], int, int>(delegate(char[] list, int k, int m)
            {
                if (k == m)
                {
                    string s = new string(list);
                    if (!permuts.Contains(s))
                    {
                        permuts.Add(s);
                    }
                }
                else
                    for (int i = k; i <= m; i++)
                    {
                        Swap(ref list[k], ref list[i]);
                        a.Invoke(list, k + 1, m);
                        Swap(ref list[k], ref list[i]);
                    }
            });
            a.Invoke(name.ToCharArray(), 0, x);

            return permuts;
        }

        private static void Swap(ref char a, ref char b)
        {
            if (a == b) return;

            a ^= b;
            b ^= a;
            a ^= b;
        }
    }

    public class LX29Crypt
    {
        private static byte[] Key = CreateKey();

        public static byte[] CreateKey()
        {
            try
            {
                ManagementObjectSearcher MOS = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
                ManagementObjectCollection moc = MOS.Get();
                string s = "";
                foreach (ManagementObject mo in moc)
                {
                    s = mo["ProcessorID"].ToString();
                    break;
                }
                long l = long.Parse(s, System.Globalization.NumberStyles.HexNumber);
                byte[] ba = BitConverter.GetBytes(l);
                int seed = BitConverter.ToInt32(new byte[] { ba[0], ba[2], ba[4], ba[6] }, 0);
                Random rd = new Random(seed);
                var key = new byte[128];
                rd.NextBytes(key);
                return key;
            }
            catch (Exception x)
            {
                x.Handle();
            }
            return null;
        }

        public static string Decrypt(byte[] Input)
        {
            if (Input[0] != 22) return "";

            byte[] Ktemp = Key;
            int Ki = 0;
            List<byte> list = new List<byte>();

            for (int i = 1; i < Input.Length; i += 2)
            {
                int b = BitConverter.ToInt16(Input, i) - Ktemp[Ki];
                list.Add((byte)b);
                Ki++;
                if (Ki >= Ktemp.Length)
                {
                    Ki = 0;
                }
            }
            string s = Encoding.UTF8.GetString(list.ToArray());
            return s;
        }

        public static byte[] Encrypt(string Input)
        {
            byte[] temp = Encoding.UTF8.GetBytes(Input);
            byte[] Ktemp = Key;
            int Ki = 0;
            List<byte> list = new List<byte>();
            list.Add((byte)22);
            for (int i = 0; i < temp.Length; i++)
            {
                int b = ((int)temp[i] + (int)Ktemp[Ki]);
                list.AddRange(BitConverter.GetBytes((short)b));
                Ki++;
                if (Ki >= Ktemp.Length)
                {
                    Ki = 0;
                }
            }
            return list.ToArray();
        }
    }
}