using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Environment;

namespace JackTheVideoRipper
{
    public class SettingsModel
    {
        public static string dir = String.Format("{0}\\JackTheVideoRipper\\settings", Environment.GetFolderPath(SpecialFolder.CommonApplicationData));
        public static string filePath = String.Format("{0}\\settings.json", dir);
        public string defaultDownloadPath { get; set; }
        public int maxConcurrentDownloads { get; set; }
        public string lastVersionYouTubeDL { get; set; }

        public static bool Exists()
        {
            return File.Exists(filePath);
        }

        public static SettingsModel generateDefaultSettings()
        {
            var s = new SettingsModel();
            s.defaultDownloadPath = YouTubeDL.defaultDownloadPath;
            s.maxConcurrentDownloads = 5;
            s.lastVersionYouTubeDL = "";

            return s;
        }
    }
}
