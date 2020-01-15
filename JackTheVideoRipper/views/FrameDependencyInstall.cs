using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JackTheVideoRipper
{
    public partial class FrameDependencyInstall : Form
    {
        public FrameDependencyInstall()
        {
            InitializeComponent();
        }

        private void FrameDependencyInstall_Shown(object sender, EventArgs e)
        {
            YouTubeDL.downloadAndInstall();
            FFmpeg.downloadAndInstall();
            AtomicParsley.downloadAndInstall();
            this.Close();
        }
    }
}
