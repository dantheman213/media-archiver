using JackTheVideoRipper.src;
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
    public partial class FrameNewMediaBatch : Form
    {
        private string startUrls;
        public string urls;
        public string type;

        public List<DownloadMediaItem> items = new List<DownloadMediaItem>();

        public FrameNewMediaBatch(string urls = "")
        {
            InitializeComponent();
            if (!String.IsNullOrEmpty(urls))
            {
                string filter = urls.Replace("\r", "").Replace("\n", "\r\n"); // make sure any copy pasting from other sources still has proper windows newlines
                this.startUrls = filter;
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrameNewMediaBatch_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(this.startUrls))
            {
                textUrls.Text = this.startUrls;
            }

            textLocation.Text = Settings.Data.defaultDownloadPath;

            cbAudioQuality.SelectedIndex = 0;
            cbAudioEncoder.SelectedIndex = 0;

            cbVideoQuality.SelectedIndex = 0;
            cbVideoEncoder.SelectedIndex = 0;
        }

        private void buttonDownload_Click(object sender, EventArgs e)
        {
            this.urls = textUrls.Text.Trim();
            if (!String.IsNullOrWhiteSpace(this.urls))
            {
                if (chkBoxExportVideo.Checked && chkBoxExportAudio.Checked)
                {
                    // video and audio
                    // TODO: fix
                    this.type = "video"; // TODO: +audio"; ?
                }
                else if (chkBoxExportVideo.Checked && !chkBoxExportAudio.Checked)
                {
                    // video only
                    this.type = "video";
                }
                else if (!chkBoxExportVideo.Checked && chkBoxExportAudio.Checked)
                {
                    // audio only
                    this.type = "audio";
                }

                string videoFormat = "bestvideo";
                if (cbVideoEncoder.Text.Trim().ToLower() == "low")
                {
                    videoFormat = "worstvideo";
                }

                string audioFormat = "bestaudio";
                if (cbAudioEncoder.Text.Trim().ToLower() == "low")
                {
                    audioFormat = "worstaudio";
                }

                string filePathTemplate = String.Format("{0}\\%(title)s.%(ext)s", textLocation.Text.Trim());

                var s = this.urls.Replace("\r", "").Split('\n');
                foreach(var url in s)
                {
                    if (!Common.isValidURL(url))
                    {
                        MessageBox.Show(String.Format("Invalid URL detected: {0}", url), "Invalid URL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.items.Clear();
                        return;
                    }

                    // TODO: improve
                    // string opts = String.Format("-f bestvideo+bestaudio/best {2} -i --no-check-certificate --prefer-ffmpeg --no-warnings --restrict-filenames {2} {3} -o {4} {5}", (cbVideoEncoder.Enabled && cbVideoEncoder.SelectedIndex > 0 ? "--recode-video " + cbVideoEncoder.Text.Trim() : ""), (chkBoxWriteMetadata.Checked ? "--add-metadata" : ""), (chkBoxEmbedThumbnail.Checked ? "--embed-thumbnail" : ""), (chkBoxIncludeAds.Checked ? "--include-ads" : ""), FrameMain.settings.defaultDownloadPath + "\\%(title)s.%(ext)s", url);

                    string opts = "";
                    if (chkBoxExportVideo.Checked && chkBoxExportAudio.Checked)
                    {
                        // video and audio
                        opts = String.Format("-f {0}+{1}/best {2} -i --no-check-certificate --prefer-ffmpeg --no-warnings --restrict-filenames {2} {3} {4} {5} -o {6} {7}", videoFormat, audioFormat, (cbVideoEncoder.Enabled && cbVideoEncoder.SelectedIndex > 0 ? "--recode-video " + cbVideoEncoder.Text.Trim() : ""), (chkBoxWriteMetadata.Checked ? "--add-metadata" : ""), (chkBoxEmbedThumbnail.Checked ? "--embed-thumbnail" : ""), (chkBoxIncludeAds.Checked ? "--include-ads" : ""), filePathTemplate, url);
                    }
                    else if (chkBoxExportVideo.Checked && !chkBoxExportAudio.Checked)
                    {
                        // video only
                        opts = String.Format("-f {0} {1} -i --no-check-certificate --prefer-ffmpeg --no-warnings --restrict-filenames {2} {3} {4} -o {5} {6}", videoFormat, (cbVideoEncoder.Enabled && cbVideoEncoder.SelectedIndex > 0 ? "--recode-video " + cbVideoEncoder.Text.Trim() : ""), (chkBoxWriteMetadata.Checked ? "--add-metadata" : ""), (chkBoxEmbedThumbnail.Checked ? "--embed-thumbnail" : ""), (chkBoxIncludeAds.Checked ? "--include-ads" : ""), filePathTemplate, url);
                    }
                    else if (!chkBoxExportVideo.Checked && chkBoxExportAudio.Checked)
                    {
                        // audio only
                        opts = String.Format("-f {0} -x --audio-format {1} --audio-quality 0 -i --no-check-certificate --prefer-ffmpeg --no-warnings --restrict-filenames {2} {3} {4} -o {5} {6}", audioFormat, cbAudioEncoder.Text.Trim(), (chkBoxWriteMetadata.Checked ? "--add-metadata" : ""), (chkBoxIncludeAds.Checked ? "--include-ads" : ""), (chkBoxEmbedThumbnail.Checked ? "--embed-thumbnail" : ""), filePathTemplate, url);
                    }
                    
                    this.items.Add(new DownloadMediaItem
                    {
                        title = "",
                        filePath = "",
                        opts = opts,
                        url = url
                    });
                }
                
          
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void buttonLocationBrowse_Click(object sender, EventArgs e)
        {
            string path = textLocation.Text.Trim();

            var f = new FolderBrowserDialog();
            f.SelectedPath = path;
            if (f.ShowDialog() == DialogResult.OK)
            {
                textLocation.Text = f.SelectedPath.Trim();
            }
        }

        private void chkBoxExportVideo_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkBoxExportVideo.Checked && !chkBoxExportAudio.Checked)
            {
                chkBoxExportVideo.Checked = true;
            }

            if (chkBoxExportVideo.Checked)
            {
                cbVideoEncoder.Enabled = true;
                cbVideoQuality.Enabled = true;
            }
            else if (!chkBoxExportVideo.Checked)
            {
                cbVideoEncoder.Enabled = false;
                cbVideoQuality.Enabled = false;
            }

            if (chkBoxExportAudio.Checked && !chkBoxExportVideo.Checked)
            {
                cbAudioEncoder.Enabled = true;
            }
            else
            {
                cbAudioEncoder.Enabled = false;
            }
        }

        private void chkBoxExportAudio_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkBoxExportVideo.Checked && !chkBoxExportAudio.Checked)
            {
                chkBoxExportAudio.Checked = true;
            }

            if (chkBoxExportAudio.Checked && chkBoxExportVideo.Checked)
            {
                cbAudioEncoder.Enabled = false;
            }
            else if (chkBoxExportAudio.Checked && !chkBoxExportVideo.Checked)
            {
                cbAudioEncoder.Enabled = true;
            }

            if (!chkBoxExportAudio.Checked)
            {
                cbAudioEncoder.Enabled = false;
                cbAudioQuality.Enabled = false;
            }
            else
            {
                cbAudioQuality.Enabled = true;
            }
        }
    }
}
