using System.Diagnostics;

namespace JackTheVideoRipper
{
    class CLI
    {
        public static void runCommand(string command, string workingDir = "")
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C " + command;
            startInfo.WorkingDirectory = ((workingDir != "") ? workingDir : Common.AppPath);
            process.StartInfo = startInfo;
            process.Start();
        }
    }
}
