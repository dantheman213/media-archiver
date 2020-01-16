using System;
using System.Diagnostics;

namespace JackTheVideoRipper
{
    class CLI
    {
        public static Process runCommand(string command, string workingDir = "")
        {
            Process process = new Process();
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = "/C " + command;
                startInfo.WorkingDirectory = ((workingDir != "") ? workingDir : Common.AppPath);
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardError = true;
                startInfo.RedirectStandardOutput = true;
                startInfo.CreateNoWindow = true;
                process.StartInfo = startInfo;
                process.Start();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            return process;
        }

        public static Process runYouTubeCommand(string bin, string opts)
        {
            Process process = new Process();

            if (!String.IsNullOrEmpty(bin))
            {
                try
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.FileName = bin;
                    startInfo.Arguments = opts;
                    startInfo.UseShellExecute = false;
                    startInfo.RedirectStandardError = true;
                    startInfo.RedirectStandardOutput = true;
                    startInfo.CreateNoWindow = true;
                    process.StartInfo = startInfo;
                    process.Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
         
            return process;
        }

        public static Process runElevatedSystemCommand(string command, string workingDir = "")
        {
            Process process = new Process();
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = "/C " + command;
                startInfo.WorkingDirectory = ((workingDir != "") ? workingDir : Common.AppPath);
                startInfo.UseShellExecute = true;
                startInfo.CreateNoWindow = true;
                startInfo.Verb = "runas";
                process.StartInfo = startInfo;
                process.Start();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
       
            return process;
        }
    }
}
