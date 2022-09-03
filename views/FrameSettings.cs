using JackTheVideoRipper.src;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JackTheVideoRipper
{
    public partial class FrameSettings : Form
    {
        public FrameSettings()
        {
            InitializeComponent();
        }

        private void buttonLocationBrowse_Click(object sender, EventArgs e)
        {
            var f = new FolderBrowserDialog();
            var dir = textLocation.Text.Trim();
            f.SelectedPath = dir;
            if (f.ShowDialog() == DialogResult.OK)
            {
                textLocation.Text = f.SelectedPath;
            }
        }

        private void FrameSettings_Load(object sender, EventArgs e)
        {
            numMaxConcurrent.Value = Settings.Data.maxConcurrentDownloads;
            textLocation.Text = Settings.Data.defaultDownloadPath;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            Settings.Data.maxConcurrentDownloads = (int)numMaxConcurrent.Value;
            Settings.Data.defaultDownloadPath = textLocation.Text.Trim();
            save();

            this.Close();
        }

        private void save()
        {
            Settings.Save();
        }
    }
}
