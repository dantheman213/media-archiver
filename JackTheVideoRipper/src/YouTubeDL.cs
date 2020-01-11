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
            string qualityOpts = "-f best";
            if (Common.isFfmpegInstalled())
            {
                qualityOpts = "-f bestvideo+bestaudio --recode-video mp4";
            }
            string cmd = binName + " " + qualityOpts + " --no-check-certificate --add-metadata -o " + defaultDownloadPath + "\\" + titleFormat + " " + url;
            return CLI.runCommand(cmd);
        }

        public static Process downloadAudio(string url)
        {
            string cmd = binName + " -f bestaudio -x --audio-format mp3 --no-check-certificate --add-metadata --embed-thumbnail -o " + defaultDownloadPath + "\\" + titleFormat + " " + url;
            return CLI.runCommand(cmd);
        }
    }
}
