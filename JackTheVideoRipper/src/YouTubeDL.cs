using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.AccessControl;
using Newtonsoft.Json;

namespace JackTheVideoRipper
{
    class YouTubeDL
    {
        public static string defaultDownloadPath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Downloads");
        private static string binName = "youtube-dl.exe";
        private static string installPath = "C:\\Program Files\\youtube-dl\\bin";
        private static string binPath = String.Format("{0}\\{1}", installPath, binName);
        private static string downloadURL = "https://yt-dl.org/downloads/latest/youtube-dl.exe";

        public static bool isInstalled()
        {
            string result = Environment.GetEnvironmentVariable("PATH");

            if (result.IndexOf("youtube-dl", StringComparison.CurrentCultureIgnoreCase) > -1)
            {
                return true;
            }

            return false;
        }

        public static void checkDownload()
        {
            if (!isInstalled())
            {
                downloadAndInstall();
            }
        }

        public static void downloadAndInstall()
        {
            if (!File.Exists(binPath))
            {
                Directory.CreateDirectory(installPath);

                // change perms
                var directory = new DirectoryInfo(installPath);
                var security = directory.GetAccessControl();

                security.AddAccessRule(
                    new FileSystemAccessRule(Environment.UserDomainName + "\\" + Environment.UserName,
                    FileSystemRights.Modify,
                    AccessControlType.Deny));
                directory.SetAccessControl(security);

                // Download binary to directory
                using (WebClient c = new WebClient())
                {
                    c.DownloadFile(downloadURL, binPath);
                }
            }

            CLI.addToPathEnv(installPath);
        }

        public static void checkForUpdates()
        {
            if (isInstalled())
            {
                CLI.runCommand(binName + " -U");
            }
        }

        public static Process run(string opts)
        {
            return CLI.runYouTubeCommand(opts);
        }
        
        public static string downloadThumbnail(string thumbnailUrl)
        {
            if (Common.isValidURL(thumbnailUrl))
            {
                string tmpDir = Path.GetTempPath();
                string tmpFileName = String.Format("jtvr_thumbnail_{0}.jpg", DateTime.Now.ToString("yyyyMMddhmmsstt"));
                string tmpFilePath = tmpDir + "\\" + tmpFileName;

                if (File.Exists(tmpFilePath))
                {
                    File.Delete(tmpFilePath);
                }

                using (WebClient c = new WebClient())
                {
                    c.DownloadFile(thumbnailUrl, tmpFilePath);
                }

                return tmpFilePath;
            }

            return null;
        }

        public static MediaInfoData getMediaData(string url)
        {
            string opts = "-s --no-warnings --no-cache-dir --print-json " + url;
            var p = CLI.runYouTubeCommand(opts);
            string json = p.StandardOutput.ReadToEnd().Trim();
            return JsonConvert.DeserializeObject<MediaInfoData>(json);
        }
    }
}
