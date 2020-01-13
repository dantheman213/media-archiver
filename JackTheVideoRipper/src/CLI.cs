using System;
using System.Diagnostics;

namespace JackTheVideoRipper
{
    class CLI
    {
        public static Process runCommand(string command, string workingDir = "")
        {
            Process process = new Process();
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

            return process;
        }

        public static Process runYouTubeCommand(string opts)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "youtube-dl.exe";
            startInfo.Arguments = opts;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.CreateNoWindow = true;
            process.StartInfo = startInfo;
            process.Start();

            return process;
        }

        public static Process runElevatedSystemCommand(string command, string workingDir = "")
        {
            Process process = new Process();
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

            return process;
        }

        public static void addToPathEnv(string pathElement)
        {
            string currentPath = System.Environment.GetEnvironmentVariable("PATH");
            string newPath = String.Format("{0};{1}", currentPath, pathElement);
            Environment.SetEnvironmentVariable("PATH", newPath, EnvironmentVariableTarget.Machine);
        }
    }
}
