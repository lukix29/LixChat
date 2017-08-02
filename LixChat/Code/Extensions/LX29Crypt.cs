using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LX29_Helpers.Crypt
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
                    HashSet<string> temp = new HashSet<string>();
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
        private readonly byte[] Key = null;

        public LX29Crypt(string id)
        {
            Key = CreateKey(id);
        }

        public byte[] Decrypt(byte[] Input)
        {
            if (Input[0] != 22) return null;

            int Ktemp = Key.Length;
            int Ki = 0;
            List<byte> list = new List<byte>();

            Random rd = new Random(Key[0]);
            int seed = BitConverter.ToInt32(Key, rd.Next(0, Ktemp - 4));
            rd = new Random(seed);

            for (int i = 1; i < Input.Length; i += 2)
            {
                Ki = rd.Next(0, Ktemp);
                int b = BitConverter.ToInt16(Input, i) - Key[Ki];
                list.Add((byte)b);
            }
            return list.ToArray();
        }

        public byte[] Encrypt(byte[] Input)
        {
            int Ktemp = Key.Length;
            int Ki = 0;
            List<byte> list = new List<byte>();
            list.Add((byte)22);

            Random rd = new Random(Key[0]);
            int seed = BitConverter.ToInt32(Key, rd.Next(0, Ktemp - 4));
            rd = new Random(seed);

            for (int i = 0; i < Input.Length; i++)
            {
                Ki = rd.Next(0, Ktemp);
                int b = ((int)Input[i] + (int)Key[Ki]);
                list.AddRange(BitConverter.GetBytes((short)b));
            }
            return list.ToArray();
        }

        private byte[] CreateKey(string id)
        {
            //ManagementObjectSearcher MOS = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            //ManagementObjectCollection moc = MOS.Get();
            //string s = "";
            //foreach (ManagementObject mo in moc)
            //{
            //    s = mo["ProcessorID"].ToString();
            //    break;
            //}
            //long l = long.Parse(s, System.Globalization.NumberStyles.HexNumber);
            //byte[] ba = BitConverter.GetBytes(l);
            //int _seed = BitConverter.ToInt32(new byte[] { ba[0], ba[2], ba[4], ba[6] }, 0);
            int s = int.Parse(id);
            Random rd = new Random(s);
            var key = new byte[2048];
            rd.NextBytes(key);
            return key;
        }
    }
}