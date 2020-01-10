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
                if (pur.paint && pur.proc.HasExited)
                {
                    // TODO: optimize
                    pur.paint = false;
                    BeginInvoke(new Action(() =>
                    {
                        pur.item.SubItems[1].Text = "Complete";
                        pur.item.SubItems[4].Text = "100%"; // Progress
                        pur.item.SubItems[5].Text = ""; // Download Speed
                        pur.item.SubItems[6].Text = "0:00"; // ETA
                        listItems.Invalidate();
                        listItems.Update();
                        listItems.Refresh();
                        Application.DoEvents();
                    }), null);
                }
                if (pur.proc.HasExited)
                {
                    continue;
                }

                string line = pur.proc.StandardOutput.ReadLine();
                if (!String.IsNullOrEmpty(line))
                {
                    // Console.WriteLine(line);
                    string l = Regex.Replace(line, @"\s+", " ");
                    string[] parts = l.Split(' ');
                    if (l.IndexOf("[youtube]") > -1)
                    {
                        BeginInvoke(new Action(() =>
                        {
                            pur.item.SubItems[1].Text = "Reading Metadata";
                            
                            listItems.Invalidate();
                            listItems.Update();
                            listItems.Refresh();
                            Application.DoEvents();
                        }), null);
                    }
                    else if (l.IndexOf("[download]") > -1)
                    {
                       if (l.IndexOf("%") > -1 && parts.Length >= 8)
                        {
                            BeginInvoke(new Action(() =>
                            {
                                pur.item.SubItems[1].Text = "Downloading";
                                pur.item.SubItems[3].Text = parts[3]; // Size
                                pur.item.SubItems[4].Text = parts[1]; // Progress
                                pur.item.SubItems[5].Text = parts[5]; // Download Speed
                                pur.item.SubItems[6].Text = parts[7]; // ETA
                                listItems.Invalidate();
                                listItems.Update();
                                listItems.Refresh();
                                Application.DoEvents();
                            }), null);
                        } else if (l.IndexOf("Destination") > -1)
                        {
                            int start = l.IndexOf(": ") + 2;
                            string filePath = l.Substring(start, l.Length - start);
                            string fileName = filePath.Substring(filePath.LastIndexOf('\\') + 1);
                            fileName = fileName.Substring(0, fileName.LastIndexOf("."));
                            BeginInvoke(new Action(() =>
                            {
                                pur.item.SubItems[0].Text = fileName; // Title
                                pur.item.SubItems[8].Text = filePath; // Path
                                listItems.Invalidate();
                                listItems.Update();
                                listItems.Refresh();
                                Application.DoEvents();
                            }), null);
                        }
                    }
                    else if (l.IndexOf("error") > -1)
                    {
                        // TODO
                        Console.WriteLine("error " + l);
                    }
                }
            }
         
           
            locked = false;
        }

        private void downloadAsVideoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string videoUrl = Interaction.InputBox("YouTube URL:", "Download Media As Video", "", -1, -1);
            if (videoUrl != "")
            {
                var li = new ListViewItem(new string[] { "", "Waiting", "Video", "-", "", "0%", "0.0 KB/s", videoUrl, "" });
                li.Tag = DateTime.Now.ToString("yyyyMMddhmmsstt");
                listItems.Items.Add(li);

                Process p = YouTubeDL.downloadVideo(videoUrl);
                ProcessUpdateRow pur = new ProcessUpdateRow();
                pur.paint = true;
                pur.proc = p;
                pur.item = listItems.Items[listItems.Items.Count - 1];
                dict.Add(li.Tag.ToString(), pur);
            }
        }

        private void downloadAsAudioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string videoUrl = Interaction.InputBox("YouTube URL:", "Download Media As Audio", "", -1, -1);
            if (videoUrl != "")
            {
                var li = new ListViewItem(new string[] { "", "Waiting", "Audio", "-", "", "0%", "0.0 KB/s", videoUrl, "" });
                li.Tag = DateTime.Now.ToString("yyyyMMddhmmsstt");
                listItems.Items.Add(li);

                Process p = YouTubeDL.downloadAudio(videoUrl);
                ProcessUpdateRow pur = new ProcessUpdateRow();
                pur.paint = true;
                pur.proc = p;
                pur.item = listItems.Items[listItems.Items.Count - 1];
                dict.Add(li.Tag.ToString(), pur);
            }
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
    }
}
