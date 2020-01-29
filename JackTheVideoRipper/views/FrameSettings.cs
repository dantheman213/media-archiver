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
            var s = FrameMain.settings;

            numMaxConcurrent.Value = s.maxConcurrentDownloads;
            textLocation.Text = s.defaultDownloadPath;
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            var s = FrameMain.settings;
            s.maxConcurrentDownloads = (int)numMaxConcurrent.Value;
            s.defaultDownloadPath = textLocation.Text.Trim();
            save();

            this.Close();
        }

        private void save()
        {
            File.WriteAllText(Settings.filePath, JsonConvert.SerializeObject(FrameMain.settings));
        }
    }
}
