using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        
        private void checkDependencies()
        {
            if (!FFmpeg.isInstalled() || !YouTubeDL.isInstalled() || !AtomicParsley.isInstalled())
            {
                DialogResult result = DialogResult.No;
                if (!YouTubeDL.isInstalled())
                {
                    result = MessageBox.Show("Could not find youtube-dl on your system. Other components may also be missing. Install all required missing components?", "Required components not installed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                }
                else if (!FFmpeg.isInstalled())
                {
                    result = MessageBox.Show("Could not find FFmpeg on your system. Other components may also be missing. Install required missing components?", "Required components not installed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                }
                else if (!FFmpeg.isInstalled())
                {
                    result = MessageBox.Show("Could not find AtomicParsley on your system. Other components may also be missing. Install required missing components?", "Required components not installed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                }

                if (result == DialogResult.Yes)
                {
                    var f = new FrameDependencyInstall();
                    f.ShowDialog();
                    // TODO ?
                    MessageBox.Show("Components have been installed successfully! Please reboot your computer for changes to take effect.", "Required Components Installed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    f.Close();
                }
                else
                {
                    MessageBox.Show("Required components not installed! This app will NOT behave correctly because its critical dependencies are missing.", "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        
        private void FrameMain_Load(object sender, EventArgs e)
        {
            this.Text = String.Format("JackTheVideoRipper {0}", Common.getAppVersion());
            listItemRowsUpdateTimer = new System.Threading.Timer(updateListItemRows, null, 0, 800);
            
            timerStatusBar_Tick(sender, e);
            timerStatusBar.Enabled = true;
        }

        private static bool locked = false;
        private void updateListItemRows(object state)
        {
            if (locked)
            {
                return;
            }
            locked = true;

            try
            {
                foreach (ProcessUpdateRow pur in dict.Values)
                {
                    if (pur.proc == null || (pur.proc != null && pur.proc.HasExited)) 
                    {
                        // TODO: optimize
                        BeginInvoke(new Action(() =>
                        {
                            string status = "Complete";
                            if (pur.proc == null || pur.proc.ExitCode > 0)
                            {
                                status = "Error";
                            }

                            string str = pur.item.SubItems[1].Text.Trim();
                            if (str != "Error" && str != "Complete")
                            {
                                status = "Complete";
                                pur.item.SubItems[4].Text = "100%"; // Progress
                                pur.item.SubItems[5].Text = ""; // Download Speed
                                pur.item.SubItems[6].Text = "00:00"; // ETA
                            }

                            if (str != status)
                            {
                                pur.item.SubItems[1].Text = status;
                            }
                        }), null);

                        continue;
                    }

                    if (pur.results != null && pur.results.Count - 1 >= pur.cursor)
                    {
                        string line = pur.results[pur.cursor];
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
                                    }
                                }), null);
                            }
                            else if (l.IndexOf("[download]") > -1)
                            {
                                if (l.IndexOf("%") > -1 && parts.Length >= 8)
                                {
                                    // download messages stream fast, bump the cursor up to one of the latest messages, if it exists...
                                    // only start skipping cursor ahead once download messages have started otherwise important info could be skipped
                                    if (pur.cursor + 10 < pur.results.Count)
                                    {
                                        for (int i = pur.results.Count; i > pur.results.Count - 10; i--)
                                        {
                                            if (l.IndexOf("[download]") > -1)
                                            {
                                                pur.cursor = i;
                                                break;
                                            }
                                        }
                                    }

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
                                    }), null);
                                }
                            }
                            else if (l.ToLower().IndexOf("error") > -1 || l.Substring(0, 21) == "Usage: youtube-dl.exe")
                            {
                                Console.WriteLine("error " + l);
                                BeginInvoke(new Action(() =>
                                {
                                    if (pur.item.SubItems[1].Text != "Error")
                                    {
                                        pur.item.SubItems[1].Text = "Error";
                                        pur.item.SubItems[5].Text = ""; // Download Speed
                                        pur.item.SubItems[6].Text = "00:00"; // ETA
                                    }
                                }), null);

                                pur.proc.Kill();
                            }
                        }

                        pur.cursor += 1;
                    }
                }
            }
            catch(Exception ex)
            {
                // TODO?
                Console.WriteLine(ex);
            }

            locked = false;
        }

        private void downloadAsVideoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            downloadMediaDialog("video");
        }

        private void downloadAsAudioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            downloadMediaDialog("audio");
        }

        private void downloadMediaDialog(string type)
        {
            var f = new FrameNewMedia();
            if (f.ShowDialog() == DialogResult.OK)
            {
                addMediaItemRow(f.title, f.type, f.url, f.opts, f.filePath);
            }
        }

        private void addMediaItemRow(string title, string type, string url, string opts, string filePath)
        {
            var li = new ListViewItem(new string[] { title, "Waiting", type, "-", "", "0%", "0.0 KB/s", url, filePath });
            li.Tag = DateTime.Now.ToString("yyyyMMddhmmsstt");
            listItems.Items.Add(li);

            Process p = YouTubeDL.run(opts);
            ProcessUpdateRow pur = new ProcessUpdateRow();
            pur.proc = p;
            pur.item = listItems.Items[listItems.Items.Count - 1];
            pur.results = new List<string>
            {
                "" // intentional
            };

            Task.Run(() =>
            {
                // spawns a new thread to read standard out data
                while (pur.proc != null && !pur.proc.HasExited)
                {
                    pur.results.Add(pur.proc.StandardOutput.ReadLine());
                }
            });

            Task.Run(() =>
            {
                // spawns a new thread to read error stream data
                while (pur.proc != null && !pur.proc.HasExited)
                {
                    string line = pur.proc.StandardError.ReadLine();
                    if (!String.IsNullOrEmpty(line))
                    {
                        pur.results.Add(line);
                    }
                }
            });

            dict.Add(li.Tag.ToString(), pur);

            int index = 0; // video
            if (type == "audio")
            {
                index = 1;
            }
            pur.item.ImageIndex = index;
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filePath = listItems.SelectedItems[0].SubItems[8].Text;
            if (!String.IsNullOrEmpty(filePath))
            {
                // TODO: fix file pathing issue
                if (File.Exists(filePath))
                {
                    Common.openFolderWithFileSelect(filePath);
                }
                else if (File.Exists(filePath + ".part"))
                {
                    Common.openFolderWithFileSelect(filePath + ".part");
                }
                return;
            }

            // couldn't find folder, rolling back to just the folder with no select
            Console.WriteLine(String.Format("couldn't find file to open at {0}", filePath));
            Common.openFolder(YouTubeDL.defaultDownloadPath);
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
            this.Close();
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
                        // kill all processes
                        foreach(var pur in dict.Values)
                        {
                            if (pur.proc != null && !pur.proc.HasExited)
                            {
                                Common.KillProcessAndChildren(pur.proc.Id);
                            }
                        }

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
        
        private void downloadAtomicParsleyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://atomicparsley.sourceforge.net/");
        }

        private void timerStatusBar_Tick(object sender, EventArgs e)
        {
            toolBarLabelCpu.Text = String.Format("CPU: {0}", Common.getCpuUsagePercentage());
            toolBarLabelMemory.Text = String.Format("Availble Memory: {0}", Common.getAvailableMemory());
            toolBarLabelNetwork.Text = String.Format("Network Ingress: {0}", Common.getNetworkTransfer());
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string versionResult = AppUpdate.checkForNewAppVersion();
            if (!String.IsNullOrEmpty(versionResult))
            {
                var result = MessageBox.Show(String.Format("New Version JackTheVideoRipper {0} Available! View Download Page?", versionResult), "New Version Available", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    Process.Start("https://github.com/dantheman213/JackTheVideoRipper/releases");
                }
            }
            else if (versionResult == "" && !sender.GetType().Equals(typeof(bool)))
            {
                // if object sender is bool then it's silent
                MessageBox.Show("App is currently up to date!", "Version Current", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (versionResult == null)
            {
                MessageBox.Show("Unable to communicate with Github!", "Can't download version manifest", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FrameMain_Shown(object sender, EventArgs e)
        {
            checkDependencies();
            YouTubeDL.checkForUpdates();

            checkForUpdatesToolStripMenuItem_Click(false, e);
        }
    }
}
