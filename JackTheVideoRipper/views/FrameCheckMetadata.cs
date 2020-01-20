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
    public partial class FrameCheckMetadata : Form
    {
        public FrameCheckMetadata()
        {
            InitializeComponent();
        }

        private void timerPostLoad_Tick(object sender, EventArgs e)
        {
            // Timeout
            this.Close();
        }
    }
}
