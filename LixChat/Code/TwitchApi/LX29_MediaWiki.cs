using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace LX29_Wiki
{
    public class LX29_MediaWiki
    {
        private static void UploadImage(string LocalFilePath, string url, string token, string OnlineFileName = "")
        {
            try
            {
                LocalFilePath = Path.GetFullPath(LocalFilePath);

                if (string.IsNullOrEmpty(OnlineFileName))
                {
                    OnlineFileName = Path.GetFileName(LocalFilePath);
                }

                using (WebClient webclient = new WebClient())
                {
                    webclient.Proxy = null;
                    webclient.Encoding = Encoding.UTF8;
                    webclient.Headers.Add("Content-Type", "multipart/form-data");

                    url += "api.php?action=upload&filename=" + OnlineFileName + "&token=" + token;

                    //macht das gleiche nur aufgeteilt auf 2 funktionen
                    //byte[] data = File.ReadAllBytes(LocalFilePath);
                    //webclient.UploadData(url, null, data);

                    webclient.UploadFile(url, null, LocalFilePath);
                }
            }
            catch (WebException x)
            {
                MessageBox.Show(x.ToString());
            }
        }
    }
}