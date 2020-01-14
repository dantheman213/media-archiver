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
    public partial class FrameNewMedia : Form
    {
        private static string lastValidUrl = null;

        public FrameNewMedia()
        {
            InitializeComponent();
        }

        private void FrameNewMedia_Load(object sender, EventArgs e)
        {
           
        }

        private void textUrl_TextChanged(object sender, EventArgs e)
        {
            string url = textUrl.Text.Trim();
            if (url != lastValidUrl && Common.isValidURL(url))
            {
                lastValidUrl = url;

                var info = YouTubeDL.getMediaData(url);
                string thumbnailFilePath = YouTubeDL.downloadThumbnail(info.thumbnail);
                pbPreview.ImageLocation = thumbnailFilePath;

                labelTitle.Text = info.title;
                labelDescription.Text = info.description;
                if (info.formats != null && info.formats.Count > 0)
                {
                    cbFormat.Items.Clear();
                    cbFormat.Items.Add("bestvideo+bestaudio/best"); // best option for video and audio

                    cbVideoFormat.Items.Clear();
                    cbVideoFormat.Items.Add("Use Gobal Format Default For Video");

                    cbAudioFormat.Items.Clear();
                    cbAudioFormat.Items.Add("Use Gobal Format Default For Audio");

                    foreach (var format in info.formats)
                    {
                        string video = "no video";
                        if (!String.IsNullOrEmpty(format.width) && !String.IsNullOrEmpty(format.height) && !String.IsNullOrEmpty(format.vcodec) && format.vcodec != "none") {
                            video = String.Format("{0} x {1} ({2})", format.width, format.height, format.vcodec);
                            cbVideoFormat.Items.Add(String.Format("{0} / {1}", format.ext, video));
                        }
                        string audio = "no audio";
                        if (!String.IsNullOrEmpty(format.acodec) && format.acodec != "none")
                        {
                            audio = String.Format("{0} kbps / {1}", format.abr, format.acodec);
                            cbAudioFormat.Items.Add(String.Format("{0} / {1}", format.ext, audio));
                        }

                        if (video != "no video" && audio != "no audio")
                        {
                            // Add to global format list
                            cbFormat.Items.Add(String.Format("{0} / {1} / {2}", format.ext, video, audio));
                        }
                    }

                    cbFormat.SelectedIndex = 0;
                    cbVideoFormat.SelectedIndex = 0;
                    cbAudioFormat.SelectedIndex = 0;
                    cbEncoder.SelectedIndex = 0;
                }
            }
        }

        private void timerPostLoad_Tick(object sender, EventArgs e)
        {
            timerPostLoad.Enabled = false;

            string clipboard = Clipboard.GetText().Trim();
            if (Common.isValidURL(clipboard))
            {
                textUrl.Text = clipboard;
                textUrl_TextChanged(sender, e);
            }
        }

        private void buttonDownload_Click(object sender, EventArgs e)
        {

        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
