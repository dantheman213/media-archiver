using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.AccessControl;
using Newtonsoft.Json;
using static System.Environment;

namespace JackTheVideoRipper
{
    class YouTubeDL
    {
        public static string defaultDownloadPath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Downloads");
        private static string binName = "youtube-dl.exe";
        private static string installPath = String.Format("{0}\\JackTheVideoRipper\\bin", Environment.GetFolderPath(SpecialFolder.CommonApplicationData));
        public static string binPath = String.Format("{0}\\{1}", installPath, binName);
        private static string downloadURL = "https://yt-dl.org/downloads/latest/youtube-dl.exe";

        public static bool isInstalled()
        {
            if (File.Exists(binPath))
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
                
                // Download binary to directory
                using (WebClient c = new WebClient())
                {
                    c.DownloadFile(downloadURL, binPath);
                }
            }
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
            return CLI.runYouTubeCommand(binPath, opts);
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

        public static List<PlaylistInfoItem> getPlaylistMetadata(string url)
        {
            string opts = "-i --no-warnings --no-cache-dir --dump-json --flat-playlist --skip-download --yes-playlist " + url;
            var p = CLI.runYouTubeCommand(binPath, opts);
            p.Start();

            string json = p.StandardOutput.ReadToEnd().Trim();
            // youtube-dl returns an individual json object per line
            json = "[" + json;
            int i = json.IndexOf('\n');
            while (i > -1)
            {
                json = json.Insert(i, ",");
                i = json.IndexOf('\n', i + 2);
            }
            json += "]";
            return JsonConvert.DeserializeObject<List<PlaylistInfoItem>>(json);
        }

        public static MediaInfoData getMediaData(string url)
        {
            string opts = "-s --no-warnings --no-cache-dir --print-json " + url;
            var p = CLI.runYouTubeCommand(binPath, opts);
            p.Start();
            string json = p.StandardOutput.ReadToEnd().Trim();
            return JsonConvert.DeserializeObject<MediaInfoData>(json);
        }

        public static string getExtractors()
        {
            string opts = "--list-extractors";
            var p = CLI.runYouTubeCommand(binPath, opts);
            p.Start();
            return p.StandardOutput.ReadToEnd().Trim();
        }

        public static string getVersion()
        {
            string opts = "--version";
            var p = CLI.runYouTubeCommand(binPath, opts);
            p.Start();
            return p.StandardOutput.ReadToEnd().Trim();
        }
    }
}
