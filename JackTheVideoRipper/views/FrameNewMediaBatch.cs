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
        }

        private void buttonDownload_Click(object sender, EventArgs e)
        {
            this.urls = textUrls.Text.Trim();
            if (!String.IsNullOrWhiteSpace(this.urls))
            {
                this.type = "video"; // TODO: +audio"; ?

                var s = this.urls.Replace("\r", "").Split('\n');
                foreach(var url in s)
                {
                    // TODO: improve
                    string opts = String.Format("-f bestvideo+bestaudio/best {2} -i --no-check-certificate --prefer-ffmpeg --no-warnings --restrict-filenames {2} {3} -o {4} {5}", (cbVideoEncoder.Enabled && cbVideoEncoder.SelectedIndex > 0 ? "--recode-video " + cbVideoEncoder.Text.Trim() : ""), (chkBoxWriteMetadata.Checked ? "--add-metadata" : ""), (chkBoxEmbedThumbnail.Checked ? "--embed-thumbnail" : ""), (chkBoxIncludeAds.Checked ? "--include-ads" : ""), FrameMain.settings.defaultDownloadPath + "\\%(title)s.%(ext)s", url);

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
    }
}
