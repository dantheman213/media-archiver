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
                    //process.Start(); // TODO: remove?
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

        public static bool hasStartedButNotFinished(Process p)
        {
            bool isRunning = false;

            try
            {
                isRunning = !p.HasExited && p.Threads.Count > 0;
            }
            catch (SystemException ex)
            {
                isRunning = false;
            }

            return isRunning;
        }

        public static bool hasNotStarted(Process p)
        {
            try
            {
                if (!p.HasExited && p.Threads.Count > 0)
                {
                    return false;
                }
            }
            catch(InvalidOperationException ioe)
            {
                if (ioe.ToString().ToLower().IndexOf("no process is associated with this object") > -1) {
                    return true;
                }
            }
            
            return false;
        }
    }
}
