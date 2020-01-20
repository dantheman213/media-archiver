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
    }
}
