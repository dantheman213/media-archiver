using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JackTheVideoRipper
{
    class Common
    {
        public static string AppPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        public static void openFolderWithFileSelect(string filePath)
        {
            Process.Start("explorer.exe", @String.Format("/select, \"{0}\"", filePath));
        }

        public static void openFolder(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    Arguments = folderPath,
                    FileName = "explorer.exe"
                };

                Process.Start(startInfo);
            }
            else
            {
                MessageBox.Show(string.Format("{0} Directory does not exist!", folderPath));
            }
        }

        public static string getAppVersion()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            return String.Format("v{0}", version.Substring(0, version.LastIndexOf(".")));
        }
    }
}
