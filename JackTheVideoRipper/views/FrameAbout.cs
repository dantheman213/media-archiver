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
        }
    }
}
