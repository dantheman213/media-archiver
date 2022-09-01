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
    public partial class FrameYTDLDependencyInstall : Form
    {
        public FrameYTDLDependencyInstall()
        {
            InitializeComponent();
        }

        private void FrameYTDLDependencyInstall_Shown(object sender, EventArgs e)
        {
            Application.DoEvents();

            YouTubeDL.downloadAndInstall();
            this.Close();
        }

        private void FrameYTDLDependencyInstall_Load(object sender, EventArgs e)
        {

        }
    }
}
