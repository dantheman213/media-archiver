using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JackTheVideoRipper
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            bool firstRun = false;
            using (Mutex mtex = new Mutex(true, "JackTheVideoRipper", out firstRun))
            {
                if (firstRun)
                {
                    if (Environment.OSVersion.Version.Major >= 6)
                        SetProcessDPIAware(); // fixes blurry text on some screens

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new FrameMain());
                } else
                {
                    MessageBox.Show("Already running!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
    }
}
