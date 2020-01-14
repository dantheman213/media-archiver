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
                    cbVideoFormat.Items.Clear();
                    cbAudioFormat.Items.Clear();

                    if (info.requestedFormats != null && info.requestedFormats.Count > 0)
                    {
                        info.formats.Insert(0, info.requestedFormats[0]);

                        if (info.requestedFormats.Count > 1)
                        {
                            info.formats.Insert(0, info.requestedFormats[1]);
                        }
                    }
                   
                    foreach (var format in info.formats)
                    {
                        if (!String.IsNullOrEmpty(format.width) && !String.IsNullOrEmpty(format.height) && !String.IsNullOrEmpty(format.vcodec) && format.vcodec != "none") {
                            var str = String.Format("{0} / {1} x {2} ({3})", format.ext, format.width, format.height, format.vcodec);
                            if (cbVideoFormat.Items.Count > 0 && cbVideoFormat.Items[0].ToString() == str)
                            {
                                // skip
                            }
                            else
                            {
                                if (info.requestedFormats != null && cbVideoFormat.Items.Count == 0)
                                {
                                    str += " [BEST]";
                                }
                                cbVideoFormat.Items.Add(str);
                            }
                        }

                        if (!String.IsNullOrEmpty(format.acodec) && format.acodec != "none")
                        {
                            var bitrate = (String.IsNullOrEmpty(format.abr) ? "unknown bitrate" : format.abr + " kbps");
                            var str = String.Format("{0} / {1} / {2}", format.ext, bitrate, format.acodec);
                            if (cbAudioFormat.Items.Count > 0 && cbAudioFormat.Items[0].ToString() == str)
                            {
                                // skip
                            }
                            else
                            {
                                if (info.requestedFormats != null && cbAudioFormat.Items.Count == 0)
                                {
                                    str += " [BEST]";
                                }
                                cbAudioFormat.Items.Add(str);
                            }
                        }
                    }

                    cbVideoFormat.SelectedIndex = 0;
                    cbAudioFormat.SelectedIndex = 0;
                    cbVideoEncoder.SelectedIndex = 0;
                    cbAudioEncoder.SelectedIndex = 0;
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
