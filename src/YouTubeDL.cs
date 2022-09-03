using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.AccessControl;
using JackTheVideoRipper.src;
using Newtonsoft.Json;
using static System.Environment;

namespace JackTheVideoRipper
{
    class YouTubeDL
    {
        public static string defaultDownloadPath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Downloads");
        private static string binName = "yt-dlp.exe";
        public static string installPath = String.Format("{0}\\JackTheVideoRipper\\bin", Environment.GetFolderPath(SpecialFolder.CommonApplicationData));
        public static string binPath = String.Format("{0}\\{1}", installPath, binName);
        private static string downloadURL = "https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe";

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
                CLI.runCommand(binName + " -U", YouTubeDL.installPath);
                var previousVersion = Settings.Data.lastVersionYouTubeDL;
                var currentVersion = getVersion();

                if (previousVersion == "")
                {
                    Settings.Data.lastVersionYouTubeDL = currentVersion;
                    Settings.Save();
                    return;
                }

                if (previousVersion != currentVersion)
                {
                    Settings.Data.lastVersionYouTubeDL = currentVersion;
                    Settings.Save();
                    MessageBox.Show(String.Format("Dependency yt-dlp has been upgraded from {0} to {1}!", previousVersion, currentVersion), "yt-dlp update", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
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
                var urlExt = thumbnailUrl.Substring(thumbnailUrl.LastIndexOf(".") + 1).ToLower();
                // allow jpg and png but don't allow webp since we'll convert that below
                if (urlExt == "webp")
                {
                    urlExt = "jpg";
                }
                string tmpFileName = String.Format("jtvr_thumbnail_{0}.{1}", DateTime.Now.ToString("yyyyMMddhmmsstt"), urlExt); // TODO: get extension from URL rather than hard coding
                string tmpFilePath = tmpDir + tmpFileName;
                
                // popular format for saving thumbnails these days but PictureBox in WinForms can't handle it :( so we'll convert to jpg
                if (thumbnailUrl.EndsWith("webp"))
                {
                    var tmpWebpFileName = String.Format("jtvr_thumbnail_{0}.webp", DateTime.Now.ToString("yyyyMMddhmmsstt"));
                    var tmpWebpFilePath = tmpDir + tmpWebpFileName;

                    if (File.Exists(tmpWebpFilePath))
                    {
                        File.Delete(tmpWebpFilePath);
                    }

                    using (WebClient c = new WebClient())
                    {
                        c.DownloadFile(thumbnailUrl, tmpWebpFilePath);
                    }

                    FFmpeg.convertImageToJpg(tmpWebpFilePath, tmpFilePath);
                }
                else
                {
                    if (File.Exists(tmpFilePath))
                    {
                        File.Delete(tmpFilePath);
                    }

                    using (WebClient c = new WebClient())
                    {
                        c.DownloadFile(thumbnailUrl, tmpFilePath);
                    }
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

        public static string getTitle(string url)
        {
            string opts = "--get-title " + url;
            var p = CLI.runYouTubeCommand(binPath, opts);
            p.Start();
            return p.StandardOutput.ReadToEnd().Trim();
        }
    }
}
