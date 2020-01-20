using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using static System.Environment;

namespace JackTheVideoRipper
{
    class AtomicParsley
    {
        private static string binName = "AtomicParsley.exe";
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
