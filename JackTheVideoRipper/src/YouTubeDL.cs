using System;
using System.Diagnostics;
using System.IO;
using System.Net;

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
                qualityOpts = "-f bestvideo+bestaudio --recode-video mp4";
            }
            string f = titleFormat + extFormat;
            if (!String.IsNullOrEmpty(fileName))
            {
                f = fileName + extFormat;
            }
            string opts = qualityOpts + " --no-check-certificate --add-metadata -o " + defaultDownloadPath + "\\" + f + " " + url;
            return CLI.runYouTubeCommand(opts);
        }

        public static Process downloadAudio(string url, string fileName = "")
        {
            string f = titleFormat + extFormat;
            if (!String.IsNullOrEmpty(fileName))
            {
                f = fileName + extFormat;
            }
            string opts = "-f bestaudio -x --audio-format mp3 --no-check-certificate --add-metadata --embed-thumbnail -o " + defaultDownloadPath + "\\" + f + " " + url;
            return CLI.runYouTubeCommand(opts);
        }
    }
}
