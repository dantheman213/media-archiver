using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JackTheVideoRipper.src
{
    internal class Settings
    {
        public static SettingsModel Data;

        public static void Save()
        {
            File.WriteAllText(SettingsModel.filePath, JsonConvert.SerializeObject(Data));
        }

        public static void Load()
        {
            if (!SettingsModel.Exists())
            {
                Directory.CreateDirectory(SettingsModel.dir);
                File.WriteAllText(SettingsModel.filePath, JsonConvert.SerializeObject(SettingsModel.generateDefaultSettings()));
            }

            var json = File.ReadAllText(SettingsModel.filePath);
            Data = JsonConvert.DeserializeObject<SettingsModel>(json);
        }
    }
}
