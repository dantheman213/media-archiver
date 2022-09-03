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

            return String.Format("v{0}", version);
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
                try
                {
                    cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                } catch(Exception ex)
                {
                    // TBA
                    return "Unable to get CPU usage...";
                }

            }
            return cpuCounter.NextValue().ToString("0.00") + "%";
        }

        private static PerformanceCounter ramCounter;
        public static string getAvailableMemory()
        {
            if (ramCounter == null)
            {
                try
                {
                    ramCounter = new PerformanceCounter("Memory", "Available MBytes");
                }
                catch (Exception ex)
                {
                    return "Unable to get RAM usage...";
                }
               
            }
            return ramCounter.NextValue() + "MB";
        }

        private static List<PerformanceCounter> networkCounters;
        public static string getNetworkTransfer()
        {
            try
            {
                // get the network transfer in kbps or mbps automatically

                if (networkCounters == null)
                {
                    networkCounters = new List<PerformanceCounter>();

                    PerformanceCounterCategory pcg = new PerformanceCounterCategory("Network Interface");
                    foreach (var instance in pcg.GetInstanceNames())
                    {
                        networkCounters.Add(new PerformanceCounter("Network Interface", "Bytes Received/sec", instance));
                    }
                }

                double count = 0;
                foreach (var counter in networkCounters)
                {
                    count += Math.Round(counter.NextValue() / 1024, 2);
                }

                string suffix = "kbps";
                if (count >= 1000)
                {
                    suffix = "mbps";
                    count = Math.Round(count / 1000, 2);
                }

                return String.Format("{0} {1}", count, suffix);
            }
            catch (Exception ex)
            {
                return "Unable to get network transfer...";
            }           
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
                    try
                    {
                        KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
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

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string RemoveAllNonNumericValuesFromString(string str)
        {
            var nums = Regex.Replace(str, @"[^\d]", String.Empty);
            if (nums == "")
            {
                nums = "0";
            }

            return nums;
        }
    }
}
