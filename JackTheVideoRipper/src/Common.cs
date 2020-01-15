using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
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

        public static bool isValidURL(string URL)
        {
            string Pattern = @"^(http|http(s)?://)?([\w-]+\.)+[\w-]+[.com|.in|.org]+(\[\?%&=]*)?";
            Regex Rgx = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return Rgx.IsMatch(URL);
        }

        public static string formatTitleForFileName(string title)
        {
            if (String.IsNullOrEmpty(title))
            {
                return "";
            }

            Regex rgx = new Regex("[^a-zA-Z0-9]");
            return rgx.Replace(title, "").Trim();
        }

        private static PerformanceCounter cpuCounter;
        public static string getCpuUsagePercentage()
        {
            if (cpuCounter == null)
            {
                cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            }
            return cpuCounter.NextValue().ToString("0.00") + "%";
        }

        private static PerformanceCounter ramCounter;
        public static string getAvailableMemory()
        {
            if (ramCounter == null)
            {
                ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            }
            return ramCounter.NextValue() + "MB";
        }

        public static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static void KillProcessAndChildren(int pid)
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher ("Select * From Win32_Process Where ParentProcessID=" + pid);
                ManagementObjectCollection moc = searcher.Get();
                foreach (ManagementObject mo in moc)
                {
                    KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            try
            {
                Process proc = Process.GetProcessById(pid);
                proc.Kill();
            }
            catch (ArgumentException)
            {
                // Process already exited.
            }
        }

        public static string stripIllegalFileNameChars(string str)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            return rgx.Replace(str, "_").Replace(' ', '_');

            // return string.Join("_", str.Split(Path.GetInvalidFileNameChars()));
        }
    }
}
