using System;
using System.Net;
using System.IO;
using System.IO.Compression;
using static System.Environment;

namespace JackTheVideoRipper
{
    class FFmpeg
    {
        private static string downloadURL = "https://github.com/dantheman213/JackTheVideoRipper/raw/master/install/ffmpeg.zip";
        private static string binName = "ffmpeg.exe";
        private static string installPath = String.Format("{0}\\ffmpeg\\bin", SpecialFolder.CommonApplicationData);
        private static string binPath = String.Format("{0}\\{1}", installPath, binName);

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
            string tmpDir = Path.GetTempPath();
            string tmpFileName = String.Format("ffmpeg_{0}.zip", DateTime.Now.ToString("yyyyMMddhmmsstt"));
            string zipPath = String.Format("{0}\\{1}", tmpDir, tmpFileName);

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

            string srcFilePath = String.Format("{0}\\{1}", tmpDir, binName);
            
            if (File.Exists(srcFilePath))
            {
                Directory.CreateDirectory(installPath);

                if (File.Exists(binPath))
                {
                    File.Delete(binPath);
                }
                File.Copy(srcFilePath, binPath, true);
                File.Delete(zipPath);
                File.Delete(srcFilePath);
            }
        }
    }
}
