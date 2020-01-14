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

                    string recommendedVideoFormat = "";
                    string recommendedAudioFormat = "";
                    var videoFormatList = new List<string>();
                    var audioFormatList = new List<string>();
                    foreach (var format in info.formats)
                    {
                        if (!String.IsNullOrEmpty(format.width) && !String.IsNullOrEmpty(format.height)) {
                            var codec = ((!String.IsNullOrEmpty(format.vcodec) && format.vcodec != "none") ? format.vcodec : "unknwon codec");
                            var str = String.Format("{0} x {1} / {2} ({3})", format.width, format.height, format.ext, codec);
                          
                            if (info.requestedFormats != null && String.IsNullOrEmpty(recommendedVideoFormat))
                            {
                                str += " [Recommended]";
                                recommendedVideoFormat = str;
                            }
                            else
                            {
                                videoFormatList.Add(str);
                            }
                        }

                        if (!String.IsNullOrEmpty(format.acodec) && format.acodec != "none")
                        {
                            var bitrate = (String.IsNullOrEmpty(format.abr) ? "unknown bitrate" : format.abr + " kbps");
                            var str = String.Format("{0} / {1} / {2}", bitrate, format.ext, format.acodec);
                          
                            if (info.requestedFormats != null && String.IsNullOrEmpty(recommendedAudioFormat))
                            {
                                str += " [Recommended]";
                                recommendedAudioFormat = str;
                            }
                            else
                            {
                                audioFormatList.Add(str);
                            }
                        }
                    }

                    if (!String.IsNullOrEmpty(recommendedVideoFormat))
                    {
                        cbVideoFormat.Items.Add(recommendedVideoFormat);
                    }
                    videoFormatList.Sort((x, y) => Int32.Parse(x.Split(' ')[0]).CompareTo(Int32.Parse(y.Split(' ')[0])));
                    videoFormatList.Reverse(); // TODO: optimze this out
                    foreach (var item in videoFormatList)
                    {
                        cbVideoFormat.Items.Add(item);
                    }

                    if (!String.IsNullOrEmpty(recommendedAudioFormat))
                    {
                        cbAudioFormat.Items.Add(recommendedAudioFormat);
                    }
                    audioFormatList.Sort((x, y) => Int32.Parse(x.Split(' ')[0]).CompareTo(Int32.Parse(y.Split(' ')[0])));
                    audioFormatList.Reverse(); // TODO: optimze this out
                    foreach (var item in audioFormatList)
                    {
                        cbAudioFormat.Items.Add(item);
                    }
                    
                    if (cbVideoFormat.Items.Count < 1)
                    {
                        cbVideoFormat.Items.Add("(no video metadata could be extracted)");
                    }
                    cbVideoFormat.SelectedIndex = 0;
                    if (cbAudioFormat.Items.Count < 1)
                    {
                        cbAudioFormat.Items.Add("(no audio metadata could be extracted)");
                    }
                    cbAudioFormat.SelectedIndex = 0;
                    if (cbVideoEncoder.Items.Count > 0)
                    {
                        cbVideoEncoder.SelectedIndex = 0;
                    }
                    if (cbAudioEncoder.Items.Count > 0)
                    {
                        cbAudioEncoder.SelectedIndex = 0;
                    }
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
