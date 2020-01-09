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
    }
}
