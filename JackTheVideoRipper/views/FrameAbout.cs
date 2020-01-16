using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JackTheVideoRipper
{
    public partial class FrameAbout : Form
    {
        private static string projectUrl = "https://github.com/dantheman213/JackTheVideoRipper";

        public FrameAbout()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo(projectUrl);
            Process.Start(sInfo);
        }

        private void FrameAbout_Load(object sender, EventArgs e)
        {
            labelVersion.Text = Common.getAppVersion();

            try
            {
                var p = CLI.runYouTubeCommand(YouTubeDL.binPath, "--list-extractors");
                var lines ="* " + p.StandardOutput.ReadToEnd().Trim().Replace("\n", "\r\n* ");
                textExtractors.Text = lines;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                textExtractors.Text = "ERROR: Can't get list of supported services.";
            }
        }
    }
}
