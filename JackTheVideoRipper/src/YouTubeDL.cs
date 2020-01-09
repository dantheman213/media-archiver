using System;
using System.IO;
using System.Net;

namespace JackTheVideoRipper
{
    class YouTubeDL
    {
        private static string binName = "youtube-dl.exe";
        private static string binPath = Common.AppPath + "\\" + binName;
        private static string downloadURL = "https://yt-dl.org/downloads/latest/youtube-dl.exe";
        public static string defaultDownloadPath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Downloads") + "\\JackTheVideoRipper";

        private static bool exists()
        {
            return File.Exists(binPath);
        }

        public static void checkDownload()
        {
            if (!exists())
            {
                using (WebClient c = new WebClient())
                {
                    c.DownloadFile(downloadURL, binPath);
                }
            }
        }

        public static void checkForUpdates()
        {
            if (exists())
            {
                CLI.runCommand(binName + " -U");
            }
        }

        public static void downloadVideo(string url)
        {
            CLI.runCommand(binName + " -f (\"bestvideo[width >= 1920]\"/bestvideo)+bestaudio/best --no-check-certificate -o " + defaultDownloadPath + "\\%(title)s.%(ext)s " + url);
        }

        public static void downloadAudio(string url)
        {
            CLI.runCommand(binName + " -f bestaudio -x --audio-format mp3 --no-check-certificate -o " + defaultDownloadPath + "\\%(title)s.%(ext)s " + url);
        }
    }
}
