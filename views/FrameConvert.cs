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
    public partial class FrameConvert : Form
    {
        private static string handBrakeUrl = "https://handbrake.fr/";

        public FrameConvert()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo(handBrakeUrl);
            Process.Start(sInfo);
        }
    }
}
