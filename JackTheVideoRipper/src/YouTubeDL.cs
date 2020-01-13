using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.AccessControl;

namespace JackTheVideoRipper
{
    class YouTubeDL
    {
        public static string defaultDownloadPath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Downloads");
        private static string binName = "youtube-dl.exe";
        private static string installPath = "C:\\Program Files\\youtube-dl\\bin";
        private static string binPath = String.Format("{0}\\{1}", installPath, binName);
        private static string downloadURL = "https://yt-dl.org/downloads/latest/youtube-dl.exe";
        private static string titleFormat = "%(title)s";
        private static string extFormat = ".%(ext)s";

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

        public static Process downloadVideo(string url, string fileName = "")
        {
            string qualityOpts = "-f best";
            if (FFmpeg.isInstalled())
            {
                qualityOpts = "-f bestvideo+bestaudio/best --recode-video mp4";
            }
            string f = titleFormat + extFormat;
            if (!String.IsNullOrEmpty(fileName))
            {
                f = fileName + extFormat;
            }
            string opts = qualityOpts + " -i --no-check-certificate --add-metadata -o " + defaultDownloadPath + "\\" + f + " " + url;
            return CLI.runYouTubeCommand(opts);
        }

        public static Process downloadAudio(string url, string fileName = "")
        {
            string f = titleFormat + extFormat;
            if (!String.IsNullOrEmpty(fileName))
            {
                f = fileName + extFormat;
            }
            string opts = "-f bestaudio -x --audio-format mp3 -i --no-check-certificate --no-warnings --add-metadata --embed-thumbnail -o " + defaultDownloadPath + "\\" + f + " " + url;
            return CLI.runYouTubeCommand(opts);
        }

        public static string downloadThumbnail(string url)
        {
            string opts = "-s --no-warnings --get-thumbnail --skip-download " + url;
            var p = CLI.runYouTubeCommand(opts);

            string possibleThumbnailUrl = p.StandardOutput.ReadToEnd().Trim();
            if (Common.isValidURL(possibleThumbnailUrl))
            {
                string tmpDir = Path.GetTempPath();
                string tmpFileName = String.Format("jtvr_thumbnail_{0}.jpg", DateTime.Now.ToString("yyyyMMddhmmsstt"));

                if (File.Exists(tmpFileName))
                {
                    File.Delete(tmpFileName);
                }

                using (WebClient c = new WebClient())
                {
                    c.DownloadFile(possibleThumbnailUrl, tmpFileName);
                }

                return tmpFileName;
            }

            return null;
        }

        public static string getTitle(string url)
        {
            string opts = "-e --no-warnings " + url;
            var p = CLI.runYouTubeCommand(opts);

            string possibleTitle = p.StandardOutput.ReadToEnd().Trim();
            if (!String.IsNullOrEmpty(possibleTitle))
            {
                return possibleTitle;
            }

            return null;
        }

        public static string getDescription(string url)
        {
            string opts = "--get-description --no-warnings " + url;
            var p = CLI.runYouTubeCommand(opts);

            string possibleDesc = p.StandardOutput.ReadToEnd().Trim();
            if (!String.IsNullOrEmpty(possibleDesc))
            {
                return possibleDesc;
            }

            return null;
        }
    }
}
