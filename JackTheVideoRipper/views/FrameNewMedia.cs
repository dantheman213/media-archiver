using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JackTheVideoRipper
{
    public partial class FrameNewMedia : Form
    {
        private string startType;

        public string title;
        public string type;
        public string url;
        public string opts;
        public string filePath;

        private string lastValidUrl = null;
        private Dictionary<string, string> videoIdLookupTable;
        private Dictionary<string, string> audioIdLookupTable;

        public FrameNewMedia(string type)
        {
            this.startType = type;
            InitializeComponent();
        }

        private void FrameNewMedia_Load(object sender, EventArgs e)
        {
            if (this.startType == "audio")
            {
                chkBoxExportVideo.Checked = false;
            }
        }

        private List<Task<bool>> taskTypeQueue = new List<Task<bool>>();
        private async void textUrl_TextChanged(object sender, EventArgs e)
        {
            async Task<bool> isStillTyping()
            {
                Application.DoEvents();

                int taskCount = taskTypeQueue.Count;
                string oldStr = textUrl.Text;
                await Task.Delay(1800);

                if ((oldStr != textUrl.Text) || (taskCount != taskTypeQueue.Count - 1))
                {
                    return true;
                }
                
                return false;
            }

            taskTypeQueue.Add(isStillTyping());
            if (await taskTypeQueue[taskTypeQueue.Count - 1])
                return;

            // typing appears to have stopped, continue
            taskTypeQueue.Clear();
            ingestMediaUrl();
        }

        private void ingestMediaUrl()
        {
            var f = new FrameCheckMetadata();
            try
            {
                string url = textUrl.Text.Trim();
                if (url != lastValidUrl && Common.isValidURL(url))
                {
                    lastValidUrl = url;

                    this.Enabled = false;
                    f.Show();
                    Application.DoEvents();

                    var info = YouTubeDL.getMediaData(url);
                    string thumbnailFilePath = YouTubeDL.downloadThumbnail(info.thumbnail);
                    pbPreview.ImageLocation = thumbnailFilePath;

                    labelTitle.Text = info.title;
                    labelDescription.Text = info.description;
                    // TODO: may need to be revised now that using --restrict-filenames flag in youtube-dl
                    textLocation.Text = FrameMain.settings.defaultDownloadPath + "\\" + String.Format("{0}{1}", Common.stripIllegalFileNameChars(info.filename.Substring(0, info.filename.LastIndexOf('.'))), info.filename.Substring(info.filename.LastIndexOf('.')));

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
                        videoIdLookupTable = new Dictionary<string, string>();
                        audioIdLookupTable = new Dictionary<string, string>();

                        foreach (var format in info.formats)
                        {
                            if (!String.IsNullOrEmpty(format.width) && !String.IsNullOrEmpty(format.height))
                            {
                                string codec = ((!String.IsNullOrEmpty(format.vcodec) && format.vcodec != "none") ? format.vcodec : "unknwon codec");
                                string tbr = ((!String.IsNullOrEmpty(format.tbr)) ? Math.Floor(Convert.ToDecimal(format.tbr)).ToString() + "k" : "---"); // rounds down
                                string fps = ((!String.IsNullOrEmpty(format.fps)) ? format.fps + "fps" : "---");
                                string note = ((!String.IsNullOrEmpty(format.formateNote)) ? format.formateNote : "---");
                                string str = String.Format("{0} x {1} / {2} / {3} / {4} / {5} {6}", format.width.PadRight(4), format.height.PadLeft(4), tbr.PadRight(7), format.ext.PadRight(5), note.PadRight(6), fps.PadLeft(6), codec);

                                if (info.requestedFormats != null && String.IsNullOrEmpty(recommendedVideoFormat))
                                {
                                    str += " [Recommended]";
                                    recommendedVideoFormat = str;
                                }
                                else
                                {
                                    videoFormatList.Add(str);
                                }

                                if (!videoIdLookupTable.ContainsKey(str))
                                {
                                    videoIdLookupTable.Add(str, format.formatId);
                                }
                            }

                            if (!String.IsNullOrEmpty(format.acodec) && format.acodec != "none")
                            {
                                var bitrate = (String.IsNullOrEmpty(format.abr) ? "---" : format.abr + " kbps");
                                var sampleRate = (String.IsNullOrEmpty(format.asr) ? "---" : format.asr + "Hz");
                                var str = String.Format("{0} / {1} / {2} / {3}", bitrate.PadRight(9), sampleRate.PadLeft(8), format.ext.PadRight(5), format.acodec);

                                if (info.requestedFormats != null && String.IsNullOrEmpty(recommendedAudioFormat))
                                {
                                    str += " [Recommended]";
                                    recommendedAudioFormat = str;
                                }
                                else
                                {
                                    audioFormatList.Add(str);
                                }

                                if (!audioIdLookupTable.ContainsKey(str))
                                {
                                    audioIdLookupTable.Add(str, format.formatId);
                                }
                            }
                        }

                        if (!String.IsNullOrEmpty(recommendedVideoFormat))
                        {
                            cbVideoFormat.Items.Add(recommendedVideoFormat);
                        }

                        try
                        {
                            videoFormatList.Sort((x, y) => Int32.Parse(x.Trim().Split(' ')[0]).CompareTo(Int32.Parse(y.Trim().Split(' ')[0])));
                        }
                        catch (FormatException ex)
                        {
                            Console.WriteLine(ex);
                        }
                        videoFormatList.Reverse(); // TODO: optimze this out
                        foreach (var item in videoFormatList)
                        {
                            cbVideoFormat.Items.Add(item);
                        }

                        if (!String.IsNullOrEmpty(recommendedAudioFormat))
                        {
                            cbAudioFormat.Items.Add(recommendedAudioFormat);
                        }

                        try
                        {
                            audioFormatList.Sort((x, y) => Int32.Parse(x.Trim().Split(' ')[0]).CompareTo(Int32.Parse(y.Trim().Split(' ')[0])));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }

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
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                MessageBox.Show("Unable to detect metadata!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            f.Close();
            this.Enabled = true;
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
            this.url = textUrl.Text.Trim();
            this.filePath = textLocation.Text.Trim();

            if (!String.IsNullOrEmpty(this.url))
            {
                if (chkBoxEmbedThumbnail.Checked)
                {
                    if (cbVideoEncoder.Enabled && cbVideoEncoder.SelectedIndex > 0 && cbVideoEncoder.Text.Trim() != "mp4")
                    {
                        MessageBox.Show("Can not embed thumbnails in non mp4 containers!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else if (cbAudioEncoder.Enabled && cbAudioEncoder.SelectedIndex > 0 && (cbAudioEncoder.Text.Trim() != "mp3" && cbAudioEncoder.Text.Trim() != "m4a"))
                    {
                        MessageBox.Show("Can only embed thumbnails in mp3 and m4a containers!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                string videoFormatId = videoIdLookupTable[cbVideoFormat.Text];
                string audioFormatId = audioIdLookupTable[cbAudioFormat.Text];

                int k = this.filePath.LastIndexOf('.');
                string fileNameFormatted = this.filePath;
                if (k > -1)
                {
                    fileNameFormatted = this.filePath.Substring(0, k);
                }
                string filePathTemplate = String.Format("{0}.%(ext)s", fileNameFormatted); // youtube-dl doesn't like it when you provide --audio-format and extension in -o together
                string optAuth = "";
                if (!String.IsNullOrEmpty(textUsername.Text) && !String.IsNullOrEmpty(textPassword.Text)) {
                    optAuth = String.Format("--username {0} --password {1}", textUsername.Text, textPassword.Text);
                }
                string optEncode = (cbVideoEncoder.Enabled && cbVideoEncoder.SelectedIndex > 0 ? "--recode-video " + cbVideoEncoder.Text.Trim() : "");
                string optMetadata = (chkBoxWriteMetadata.Checked ? "--add-metadata" : "");
                string optAds = (chkBoxIncludeAds.Checked ? "--include-ads" : "");
                string optEmbedThumbnail = (chkBoxEmbedThumbnail.Checked ? "--embed-thumbnail" : "");
                string optGeneral = "-i --no-check-certificate --prefer-ffmpeg --no-warnings --restrict-filenames";
                if ((chkBoxExportVideo.Checked && chkBoxExportAudio.Checked) || (chkBoxExportVideo.Checked && !chkBoxExportAudio.Checked))
                {
                    // video and audio
                    // TODO: split video/audio and video only
                    this.opts = String.Format("-f {0}+{1}/best {2} {3} {4} {5} {6} {7} -o {8} {9}", videoFormatId, audioFormatId, optEncode, optGeneral, optMetadata, optEmbedThumbnail, optAds, optAuth, filePathTemplate, url);
                    this.type = "video"; // TODO: +audio"; ?
                }               
                else if (!chkBoxExportVideo.Checked && chkBoxExportAudio.Checked)
                {
                    // audio only
                    this.opts = String.Format("-f {0} -x --audio-format {1} --audio-quality 0 {2} {3} {4} -o {4} {5}", audioFormatId, cbAudioEncoder.Text.Trim(), optGeneral, optMetadata, optAds, filePathTemplate, url);
                    this.type = "audio";
                }

                this.title = labelTitle.Text.Trim();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                // TODO?
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void buttonLocationBrowse_Click(object sender, EventArgs e)
        {
            var d = new SaveFileDialog();
            d.InitialDirectory = FrameMain.settings.defaultDownloadPath;
            d.Filter = "All files (*.*)|*.*";

            var result = d.ShowDialog();
            if (result == DialogResult.OK)
            {
                textLocation.Text = d.FileName;
            }
        }

        private void chkBoxExportAudio_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkBoxExportAudio.Checked && !chkBoxExportVideo.Checked)
            {
                chkBoxExportAudio.Checked = true;
            }

            if (chkBoxExportAudio.Checked && !chkBoxExportVideo.Checked)
            {
                cbAudioEncoder.Enabled = true;
            }
            
            if (chkBoxExportAudio.Checked)
            {
                cbAudioFormat.Enabled = true;
                cbAudioEncoder_TextChanged(sender, e); // TODO: fix
            }
            else
            {
                cbAudioFormat.Enabled = false;
                cbAudioEncoder.Enabled = false;
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
                cbVideoFormat.Enabled = true;
                cbVideoEncoder.Enabled = true;
                cbAudioEncoder.Enabled = false;
                cbVideoEncoder_TextChanged(sender, e); // TODO: fix
            }
            else
            {
                cbVideoFormat.Enabled = false;
                cbVideoEncoder.Enabled = false;
                cbAudioEncoder.Enabled = true;

                tabImportType.SelectedTab = tabPageAudio;
            }
        }

        private void cbVideoEncoder_TextChanged(object sender, EventArgs e)
        {
            if (cbVideoEncoder.Enabled && cbVideoEncoder.SelectedIndex > 0)
            {
                string filePath = textLocation.Text.Trim();
                filePath = String.Format("{0}{1}", filePath.Substring(0, filePath.LastIndexOf('.') + 1), cbVideoEncoder.Text.Trim());
                textLocation.Text = filePath;
            }
        }

        private void cbVideoFormat_TextChanged(object sender, EventArgs e)
        {
            if (cbVideoFormat.Enabled && cbVideoEncoder.SelectedIndex == 0)
            {
                try
                {
                    string filePath = textLocation.Text.Trim();
                    filePath = String.Format("{0}{1}", filePath.Substring(0, filePath.LastIndexOf('.') + 1), cbVideoFormat.Text.Split('/')[2].Trim());
                    textLocation.Text = filePath;
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        private void cbAudioFormat_TextChanged(object sender, EventArgs e)
        {
            if (!chkBoxExportVideo.Checked && cbAudioFormat.Enabled && cbAudioEncoder.SelectedIndex == 0)
            {
                try
                {
                    string filePath = textLocation.Text.Trim();
                    filePath = String.Format("{0}{1}", filePath.Substring(0, filePath.LastIndexOf('.') + 1), cbAudioFormat.Text.Split('/')[2].Trim());
                    textLocation.Text = filePath;
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        private void cbAudioEncoder_TextChanged(object sender, EventArgs e)
        {
            if (!chkBoxExportVideo.Checked && cbAudioEncoder.Enabled && cbAudioEncoder.SelectedIndex > 0)
            {
                string filePath = textLocation.Text.Trim();
                filePath = String.Format("{0}{1}", filePath.Substring(0, filePath.LastIndexOf('.') + 1), cbAudioEncoder.Text.Trim());
                textLocation.Text = filePath;
            }
        }
    }
}
