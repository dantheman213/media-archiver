using System;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;

namespace JackTheVideoRipper
{
    class FFmpeg
    {
        private static string downloadURL = "https://github.com/dantheman213/JackTheVideoRipper/raw/master/install/ffmpeg.zip";
        private static string binName = "ffmpeg.exe";
        private static string zipName = "ffmpeg.zip";
        private static string zipPath = Common.AppPath + "\\" + zipName;
        private static string installPath = "C:\\Program Files\\ffmpeg\\bin";

        public static bool isInstalled()
        {
            string result = Environment.GetEnvironmentVariable("PATH");

            if (result.IndexOf("ffmpeg", StringComparison.CurrentCultureIgnoreCase) > -1)
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
            using (WebClient c = new WebClient())
            {
                if (File.Exists(zipPath))
                {
                    File.Delete(zipPath);
                }
                c.DownloadFile(downloadURL, zipPath);

                using (ZipArchive archive = ZipFile.OpenRead(zipPath))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        try
                        {
                            string filePath = Path.Combine(Common.AppPath, entry.FullName);
                            if (File.Exists(filePath))
                            {
                                File.Delete(filePath);
                            }
                            entry.ExtractToFile(filePath);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }
            }

            string srcFilePath = String.Format("{0}\\{1}", Common.AppPath, binName);
            string destFilePath = String.Format("{0}\\{1}", installPath, binName);
            
            if (File.Exists(srcFilePath))
            {
                Directory.CreateDirectory(installPath);

                if (File.Exists(destFilePath))
                {
                    File.Delete(destFilePath);
                }
                File.Copy(srcFilePath, destFilePath, true);
            }

            CLI.addToPathEnv(installPath);
        }
    }
}
