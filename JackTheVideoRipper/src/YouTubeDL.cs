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
        private static string binPath = Common.AppPath + "\\" + binName;
        private static string downloadURL = "https://yt-dl.org/downloads/latest/youtube-dl.exe";
        private static string titleFormat = "%(title)s.%(ext)s";

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

        public static Process downloadVideo(string url)
        {
            return CLI.runCommand(binName + " -f best --no-check-certificate -o " + defaultDownloadPath + "\\" + titleFormat + " " + url);
        }

        public static Process downloadAudio(string url)
        {
            return CLI.runCommand(binName + " -f bestaudio -x --audio-format mp3 --no-check-certificate -o " + defaultDownloadPath + "\\" + titleFormat + " " + url);
        }
    }
}
