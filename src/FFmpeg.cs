using System;
using System.Net;
using System.IO;
using System.IO.Compression;
using static System.Environment;

namespace JackTheVideoRipper
{
    class FFmpeg
    {
        private static string binName = "ffmpeg.exe";
        private static string installPath = String.Format("{0}\\JackTheVideoRipper\\bin", Environment.GetFolderPath(SpecialFolder.CommonApplicationData));
        private static string binPath = String.Format("{0}\\{1}", installPath, binName);

        public static bool isInstalled()
        {
            if (File.Exists(binPath))
            {
                return true;
            }

            return false;
        }

        public static void convertImageToJpg(string inputPath, string outputPath)
        {
            CLI.runCommand(String.Format("{0} -i {1} -vf \"scale=1920:-1\" {2}", binPath, inputPath, outputPath));
            Thread.Sleep(1000); // allow the file to exist in the OS before querying for it otherwise sometimes its missed by app...
        }
    }
}
