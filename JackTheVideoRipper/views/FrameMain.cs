using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using JackTheVideoRipper.src.models;
using Microsoft.VisualBasic;

namespace JackTheVideoRipper
{
    public partial class FrameMain : Form
    {
        private static Dictionary<string, ProcessUpdateRow> dict = new Dictionary<string, ProcessUpdateRow>();
        private static System.Threading.Timer listItemRowsUpdateTimer;

        public FrameMain()
        {
            InitializeComponent();
        }
        
        private void FrameMain_Load(object sender, EventArgs e)
        {
            YouTubeDL.checkDownload();
            YouTubeDL.checkForUpdates();
            this.Text = String.Format("JackTheVideoRipper {0}", Common.getAppVersion());
            listItemRowsUpdateTimer = new System.Threading.Timer(updateListItemRows, null, 0, 250);

            if (!Common.isFfmpegInstalled())
            {
                MessageBox.Show("Could not find FFmpeg installed! Please install FFmpeg for the best experience while using this app.", "FFmpeg is not installed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private static bool locked = false;
        private void updateListItemRows(object state)
        {
            if (locked)
            {
                return;
            }
            locked = true;

            foreach (ProcessUpdateRow pur in dict.Values)
            {
                if (pur.proc == null)
                {
                    continue;
                }
                if (pur.proc.HasExited)
                {
                    // TODO: optimize
                    BeginInvoke(new Action(() =>
                    {
                        if (pur.item.SubItems[1].Text != "Complete")
                        {
                            pur.item.SubItems[1].Text = "Complete";
                            pur.item.SubItems[4].Text = "100%"; // Progress
                            pur.item.SubItems[5].Text = ""; // Download Speed
                            pur.item.SubItems[6].Text = "0:00"; // ETA
                            updateListUI();
                        }
                    }), null);
                    continue;
                }

                string line = pur.proc.StandardOutput.ReadLine();
                if (!String.IsNullOrEmpty(line))
                {
                    string l = Regex.Replace(line, @"\s+", " ");
                    string[] parts = l.Split(' ');
                    if (l.IndexOf("[youtube]") > -1)
                    {
                        BeginInvoke(new Action(() =>
                        {
                            if (pur.item.SubItems[1].Text != "Reading Metadata")
                            {
                                pur.item.SubItems[1].Text = "Reading Metadata";
                                updateListUI();
                            }
                        }), null);
                    }
                    else if (l.IndexOf("[ffmpeg]") > -1)
                    {
                        BeginInvoke(new Action(() =>
                        {
                            if (pur.item.SubItems[1].Text != "Transcoding")
                            {
                                pur.item.SubItems[1].Text = "Transcoding";
                                pur.item.SubItems[4].Text = "99%"; // Progress
                                pur.item.SubItems[5].Text = ""; // Download Speed
                                pur.item.SubItems[6].Text = "0:01"; // ETA
                                updateListUI();
                            }
                        }), null);
                    }
                    else if (l.IndexOf("[download]") > -1)
                    {
                       if (l.IndexOf("%") > -1 && parts.Length >= 8)
                        {
                            BeginInvoke(new Action(() =>
                            {
                                if (pur.item.SubItems[1].Text != "Downloading")
                                {
                                    pur.item.SubItems[1].Text = "Downloading";
                                }
                                if (pur.item.SubItems[3].Text == "" || pur.item.SubItems[3].Text == "-")
                                {
                                    pur.item.SubItems[3].Text = parts[3]; // Size
                                }
                                if (parts[1].Trim() != "100%")
                                {
                                    pur.item.SubItems[4].Text = parts[1]; // Progress
                                }
                                pur.item.SubItems[5].Text = parts[5]; // Download Speed
                                if (parts[7].Trim() != "00:00")
                                {
                                    pur.item.SubItems[6].Text = parts[7]; // ETA
                                }
                                updateListUI();
                            }), null);
                        } else if (l.IndexOf("Destination") > -1)
                        {
                            int start = l.IndexOf(": ") + 2;
                            string filePath = l.Substring(start, l.Length - start);
                            string fileName = filePath.Substring(filePath.LastIndexOf('\\') + 1);
                            fileName = fileName.Substring(0, fileName.LastIndexOf("."));
                            BeginInvoke(new Action(() =>
                            {
                                if (pur.item.SubItems[0].Text == "" || pur.item.SubItems[8].Text == "")
                                {
                                    pur.item.SubItems[0].Text = fileName; // Title
                                    pur.item.SubItems[8].Text = filePath; // Path
                                    updateListUI();
                                }
                            }), null);
                        }
                    }
                    else if (l.IndexOf("error", StringComparison.CurrentCultureIgnoreCase) > -1)
                    {
                        Console.WriteLine("error " + l);
                        BeginInvoke(new Action(() =>
                        {
                            if (pur.item.SubItems[1].Text != "Error")
                            {
                                pur.item.SubItems[1].Text = "Error";
                                pur.item.SubItems[5].Text = ""; // Download Speed
                                pur.item.SubItems[6].Text = "00:00"; // ETA
                                updateListUI();
                            }
                        }), null);
                        pur.proc = null;
                    }
                }
            }
           
            locked = false;
        }

        private void updateListUI()
        {
            listItems.Invalidate();
            listItems.Update();
            listItems.Refresh();
            Application.DoEvents();
        }

        private void downloadAsVideoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string videoUrl = Interaction.InputBox("YouTube URL:", "Download Media As Video", "", -1, -1);
            if (String.IsNullOrEmpty(videoUrl))
            {
                return;
            }
            if (!Common.isValidYouTubeURL(videoUrl))
            {
                MessageBox.Show("Invalid YouTube URL!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
           
            var li = new ListViewItem(new string[] { "", "Waiting", "Video", "-", "", "0%", "0.0 KB/s", videoUrl, "" });
            li.Tag = DateTime.Now.ToString("yyyyMMddhmmsstt");
            listItems.Items.Add(li);

            Process p = YouTubeDL.downloadVideo(videoUrl);
            ProcessUpdateRow pur = new ProcessUpdateRow();
            pur.proc = p;
            pur.item = listItems.Items[listItems.Items.Count - 1];
            dict.Add(li.Tag.ToString(), pur);

            pur.item.ImageIndex = 0;
        }

        private void downloadAsAudioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string videoUrl = Interaction.InputBox("YouTube URL:", "Download Media As Audio", "", -1, -1);
            if (String.IsNullOrEmpty(videoUrl))
            {
                return;
            }
            if (!Common.isValidYouTubeURL(videoUrl))
            {
                MessageBox.Show("Invalid YouTube URL!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
          
            var li = new ListViewItem(new string[] { "", "Waiting", "Audio", "-", "", "0%", "0.0 KB/s", videoUrl, "" });
            li.Tag = DateTime.Now.ToString("yyyyMMddhmmsstt");
            listItems.Items.Add(li);

            Process p = YouTubeDL.downloadAudio(videoUrl);
            ProcessUpdateRow pur = new ProcessUpdateRow();
            pur.proc = p;
            pur.item = listItems.Items[listItems.Items.Count - 1];
            dict.Add(li.Tag.ToString(), pur);

            pur.item.ImageIndex = 1;
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filePath = listItems.SelectedItems[0].SubItems[8].Text;
            if (String.IsNullOrEmpty(filePath))
            {
                Console.WriteLine("file does not exist to open in explorer");
                Common.openFolder(YouTubeDL.defaultDownloadPath);
            }
            if (!File.Exists(filePath))
            {
                filePath = filePath + ".part";
                if (!File.Exists(filePath))
                {
                    // MessageBox.Show("Unable to find file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Console.WriteLine(String.Format("couldn't find file to open at {0}", filePath));
                    Common.openFolder(YouTubeDL.defaultDownloadPath);
                    return;
                }
            }
            Common.openFolderWithFileSelect(filePath);
        }

        private void listItems_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listItems.FocusedItem.Bounds.Contains(e.Location))
                {
                    contextMenuListItems.Show(Cursor.Position);
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (Form f = new FrameAbout())
            {
                f.ShowDialog();
            }
        }

        private void convertMediaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (Form f = new FrameConvert())
            {
                f.ShowDialog();
            }
        }

        private void openURLInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listItems.SelectedItems.Count > 0)
            {
                string url = listItems.SelectedItems[0].SubItems[7].Text;
                if (!String.IsNullOrEmpty(url))
                {
                    Process.Start(url);
                }
            }
        }

        private void openMediaInPlayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listItems.SelectedItems.Count > 0)
            {
                if (listItems.SelectedItems[0].SubItems[1].Text == "Complete")
                {
                    string filePath = listItems.SelectedItems[0].SubItems[8].Text;
                    if (!String.IsNullOrEmpty(filePath))
                    {
                        if (File.Exists(filePath))
                        {
                            Process.Start(filePath);
                        }
                    }
                }
            }
        }

        private void listItems_DoubleClick(object sender, EventArgs e)
        {
            if (listItems.SelectedItems.Count > 0)
            {
                openMediaInPlayerToolStripMenuItem_Click(sender, e);
            }
        }

        private void FrameMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (ListViewItem item in listItems.Items)
            {
                if (item.SubItems[1].Text != "Complete")
                {
                    if (MessageBox.Show("You have pending downloads, are you sure you want to exit?", "Verify Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        e.Cancel = false;                        
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                    return;
                }
            }
        }

        private void toolStripButtonDownloadVideo_Click(object sender, EventArgs e)
        {
            downloadAsVideoToolStripMenuItem_Click(sender, e);
        }

        private void toolStripButtonDownloadAudio_Click(object sender, EventArgs e)
        {
            downloadAsAudioToolStripMenuItem_Click(sender, e);
        }
        
        private void downloadFFmpegToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.ffmpeg.org/download.html");
        }

        private void downloadHandbrakeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://handbrake.fr/downloads.php");
        }

        private void downloadVLCPlayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.videolan.org/vlc/");
        }
    }
}
