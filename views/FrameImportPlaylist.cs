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
    public partial class FrameImportPlaylist : Form
    {
        public string url;

        public FrameImportPlaylist()
        {
            InitializeComponent();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonImport_Click(object sender, EventArgs e)
        {
            string url = textUrl.Text.Trim();
            if (!String.IsNullOrEmpty(url))
            {
                if (Common.isValidURL(url))
                {
                    this.url = url;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }
    }
}
