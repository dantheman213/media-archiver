using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JackTheVideoRipper
{
    class Common
    {
        public static string AppPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        public static void openFolderWithFileSelect(string filePath)
        {
            Process.Start("explorer.exe", @String.Format("/select, \"{0}\"", filePath));
        }

        public static void openFolder(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    Arguments = folderPath,
                    FileName = "explorer.exe"
                };

                Process.Start(startInfo);
            }
            else
            {
                MessageBox.Show(string.Format("{0} Directory does not exist!", folderPath));
            }
        }

        public static string getAppVersion()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            return String.Format("v{0}", version.Substring(0, version.LastIndexOf(".")));
        }

        public static bool isValidURL(string URL)
        {
            string Pattern = @"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";
            Regex Rgx = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return Rgx.IsMatch(URL);
        }

        public static bool isValidYouTubeURL(string s)
        {
            if (isValidURL(s))
            {
                // TODO: add support for other websites that youtube-dl supports

                Uri uri = new Uri(s);
                if (uri.Host == "youtube.com" || uri.Host == "www.youtube.com")
                {
                    return true;
                }
            }

            return false;
        }

        public static bool isFfmpegInstalled()
        {
            string result = Environment.GetEnvironmentVariable("PATH");

            if (result.IndexOf("ffmpeg", StringComparison.CurrentCultureIgnoreCase) > -1)
            {
                return true;
            }

            return false;
        }

        public static string getYouTubeVideoTitle(string url)
        {
            // TODO: Replace unicode chars like \\u0026 with their real char &
            HttpClient client = new HttpClient();
            using (HttpResponseMessage response = client.GetAsync(url).Result)
            {
                using (HttpContent content = response.Content)
                {
                    string result = content.ReadAsStringAsync().Result;
                    int i = result.IndexOf("\\\"title\\\":\\\"");
                    if (i > -1)
                    {
                        i += 12;
                        int k = result.IndexOf("\\\"", i);
                        if (k > -1)
                        {
                            return result.Substring(i, (k - i));
                        }
                    }
                }
            }

            return null;
        }

        public static string formatTitleForFileName(string title)
        {
            if (String.IsNullOrEmpty(title))
            {
                return "";
            }

            Regex rgx = new Regex("[^a-zA-Z0-9]");
            return rgx.Replace(title, "").Trim();
        }
    }
}
