using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JackTheVideoRipper
{
    public partial class FrameMain : Form
    {
        public static Settings settings;

        private static Dictionary<string, ProcessUpdateRow> dict = new Dictionary<string, ProcessUpdateRow>();
        private static System.Threading.Timer listItemRowsUpdateTimer;

        public FrameMain()
        {
            InitializeComponent();
        }
        
        private void checkDependencies()
        {
            if (!YouTubeDL.isInstalled())
            {
                var result = MessageBox.Show("yt-dlp is required. Install?", "Install Core Component", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    var f = new FrameYTDLDependencyInstall();
                    f.ShowDialog();
                    // TODO ?
                    MessageBox.Show("Components have been installed successfully!", "Required Components Installed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    f.Close();
                }
                else
                {
                    MessageBox.Show("Required components not installed! This app will NOT behave correctly because its critical dependencies are missing.", "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (!FFmpeg.isInstalled())
            {
                MessageBox.Show("Could not find FFmpeg on your system. This app will NOT behave correctly because its critical dependencies are missing. Please reinstall this app.", "Required components not installed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (!AtomicParsley.isInstalled())
            {
                MessageBox.Show("Could not find AtomicParsley on your system.. This app will NOT behave correctly because its critical dependencies are missing. Please reinstall this app.", "Required components not installed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        
        private void loadConfig()
        {
            if (!Settings.Exists())
            {
                Directory.CreateDirectory(Settings.dir);
                File.WriteAllText(Settings.filePath, JsonConvert.SerializeObject(Settings.generateDefaultSettings()));
            }

            var json = File.ReadAllText(Settings.filePath);
            settings = JsonConvert.DeserializeObject<Settings>(json);
        }

        private void FrameMain_Load(object sender, EventArgs e)
        {
            loadConfig();
            
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

          
            foreach (ProcessUpdateRow pur in dict.Values)
            {
                if (pur.proc == null || (pur.proc != null && pur.started && pur.proc.HasExited)) 
                {
                    // TODO: optimize
                    BeginInvoke(new Action(() =>
                    {
                        string status = "Complete";
                        if (pur.proc == null || pur.proc.ExitCode > 0)
                        {
                            status = "Error";
                            pur.finished = true;
                            timerProcessLimit_Tick(null, null);
                        }

                        string str = pur.item.SubItems[1].Text.Trim();
                        if (str != "Error" && str != "Complete")
                        {
                            status = "Complete";
                            pur.item.SubItems[4].Text = "100%"; // Progress
                            pur.item.SubItems[5].Text = ""; // Download Speed
                            pur.item.SubItems[6].Text = "00:00"; // ETA
                            pur.finished = true;
                            timerProcessLimit_Tick(null, null);
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
                                    timerProcessLimit_Tick(null, null);
                                }
                            }), null);
                                
                            pur.proc.Kill();
                        }
                    }

                    pur.cursor += 1;
                }
            }

            locked = false;
        }

        private void downloadVideoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            downloadMediaDialog("video");
        }

        private void downloadAudioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            downloadMediaDialog("audio");
        }

        private void downloadMediaDialog(string type)
        {
            var f = new FrameNewMedia(type);

            if (f.ShowDialog() == DialogResult.OK)
            {
                addMediaItemRow(f.title, f.type, f.url, f.opts, f.filePath);
                timerProcessLimit_Tick(null, null);
            }
        }

        private void addMediaItemRow(string title, string type, string url, string opts, string filePath)
        {
            var li = new ListViewItem(new string[] { title, "Waiting", type, "-", "", "0%", "0.0 KB/s", url, filePath });
            li.Tag = Common.RandomString(5) + DateTime.UtcNow.Ticks;
            listItems.Items.Add(li);

            Process p = YouTubeDL.run(opts);
            ProcessUpdateRow pur = new ProcessUpdateRow();
            pur.proc = p;
            pur.item = listItems.Items[listItems.Items.Count - 1];
            pur.results = new List<string>
            {
                "" // intentional
            };
            
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
            Common.openFolder(settings.defaultDownloadPath);
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
                            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
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
                if (item.SubItems[1].Text != "Complete" && item.SubItems[1].Text != "Error")
                {
                    if (MessageBox.Show("You have pending downloads, are you sure you want to exit?", "Verify Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        // kill all processes
                        foreach(var pur in dict.Values)
                        {
                            try
                            {
                                if (pur.proc != null && !pur.proc.HasExited)
                                {
                                    Common.KillProcessAndChildren(pur.proc.Id);
                                }
                            }
                            catch
                            {
                                // do nothing
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
            downloadVideoToolStripMenuItem_Click(sender, e);
        }

        private void toolStripButtonDownloadAudio_Click(object sender, EventArgs e)
        {
            downloadAudioToolStripMenuItem_Click(sender, e);
        }
        
        private void downloadFFmpegToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://www.ffmpeg.org/download.html") { UseShellExecute = true });
        }

        private void downloadHandbrakeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://handbrake.fr/downloads.php") { UseShellExecute = true });
        }

        private void downloadVLCPlayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://www.videolan.org/vlc") { UseShellExecute = true });
        }
        
        private void downloadAtomicParsleyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("http://atomicparsley.sourceforge.net") { UseShellExecute = true });
        }

        private void timerStatusBar_Tick(object sender, EventArgs e)
        {
            toolBarLabelCpu.Text = String.Format("CPU: {0}", Common.getCpuUsagePercentage());
            toolBarLabelMemory.Text = String.Format("Available Memory: {0}", Common.getAvailableMemory());
            toolBarLabelNetwork.Text = String.Format("Network Ingress: {0}", Common.getNetworkTransfer());
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // if sender obj is bool then version being checked on startup passively and dont show dialog that it's up to date
            var result = AppUpdate.checkForNewAppVersion();
            if (result != null)
            {
                if (result.isNewerVersionAvailable)
                {
                    var dialogResponse = MessageBox.Show(String.Format("New Version JackTheVideoRipper {0} Available! View Download Page?", result.version), "New Version Available", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (dialogResponse == DialogResult.Yes)
                    {
                        Process.Start(new ProcessStartInfo("https://github.com/dantheman213/JackTheVideoRipper/releases") { UseShellExecute = true });
                    }
                }
                else if (!result.isNewerVersionAvailable && !sender.GetType().Equals(typeof(bool)))
                {
                    MessageBox.Show("App is currently up to date!", "Version Current", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            } 
            else if (result == null && !sender.GetType().Equals(typeof(bool)))
            {
                MessageBox.Show("Unable to communicate with Github!", "Can't download version manifest", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FrameMain_Shown(object sender, EventArgs e)
        {
            checkDependencies();

            Task.Run(() =>
            {
                YouTubeDL.checkForUpdates();
            });
        }

        private void downloadVS2010RedistributableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://www.microsoft.com/en-us/download/confirmation.aspx?id=5555") { UseShellExecute = true });
        }

        private void openDownloadFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Common.openFolder(settings.defaultDownloadPath);
        }
        
        private void downloadBatchManualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string urls = null;
            if (sender.GetType().Equals(typeof(string)))
            {
                urls = (string)sender;
            }

            var f = new FrameNewMediaBatch(urls);
            var result = f.ShowDialog();

            if (result == DialogResult.OK)
            {
                if (f.items != null && f.items.Count > 0)
                {
                    foreach(var item in f.items)
                    {
                        addMediaItemRow(item.title, f.type, item.url, item.opts, item.filePath);
                    }

                    queueBatchDownloads();
                }
            }
        }

        private void queueBatchDownloads()
        {
            for (int i = 0; i < settings.maxConcurrentDownloads; i++)
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(300);
                timerProcessLimit_Tick(null, null);
            }
        }

        private void downloadBatchDocumentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var d = new OpenFileDialog();
            d.InitialDirectory = settings.defaultDownloadPath;
            d.Filter = "All Files (*.*)|*.*";

            if (d.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(d.FileName))
                {
                    var payload = File.ReadAllText(d.FileName);
                    if (!String.IsNullOrEmpty(payload))
                    {
                        var items = Import.getAllUrlsFromPayload(payload);
                        var result = String.Join("\r\n", items);

                        downloadBatchManualToolStripMenuItem_Click(result, e);
                    }
                }
            }
        }

        private void downloadBatchYouTubePlaylistlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var d = new FrameImportPlaylist();
            if (d.ShowDialog() == DialogResult.OK)
            {
                string url = d.url;
                var items = YouTubeDL.getPlaylistMetadata(url);

                string result = "";
                foreach (var item in items)
                {
                    result += String.Format("https://www.youtube.com/watch?v={0}\r\n", item.id);
                }

                downloadBatchManualToolStripMenuItem_Click(result, e);
            }
        }

        private void timerCheckForUpdates_Tick(object sender, EventArgs e)
        {
            timerCheckForUpdates.Enabled = false;
            checkForUpdatesToolStripMenuItem_Click(false, e);
        }

        private void timerProcessLimit_Tick(object sender, EventArgs e)
        {
            int total = listItems.Items.Count;

            if (total > 0)
            {
                int active = 0;
                int done = 0;
                ProcessUpdateRow nextDownload = null;

                foreach (var pur in dict.Values)
                {
                    if (pur.started && !pur.finished)
                    {
                        active += 1;
                    }
                    else if (pur.started && pur.finished)
                    {
                        done += 1;
                    }
                    else if (!pur.started && !pur.finished)
                    {
                        if (nextDownload == null)
                        {
                            nextDownload = pur;
                        }
                    }
                }

                if (total - done > 0)
                {
                    if (active < settings.maxConcurrentDownloads)
                    {
                        foreach (var pur in dict.Values)
                        {
                            if (nextDownload != null && pur != nextDownload)
                            {
                                continue;
                            }

                            if (!pur.started)
                            {
                                pur.proc.Start();
                                pur.started = true;

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

                                //active += 1;
                                timerProcessLimit.Enabled = false;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = new FrameSettings();
            if (f.ShowDialog() == DialogResult.OK)
            {
                queueBatchDownloads();
            }
        }
    }
}
