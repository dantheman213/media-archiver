using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private void downloadAsVideoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string videoUrl = Interaction.InputBox("YouTube URL:", "Download Media As Video", "", -1, -1);
            if (videoUrl != "")
            {
                var li = new ListViewItem(new string[] { "TODO", "Waiting", "Video", "TODO", "TODO", "TODO", "TODO", videoUrl, YouTubeDL.defaultDownloadPath });
                li.Tag = DateTime.Now.ToString("yyyyMMddhmmsstt");
                listItems.Items.Add(li);

                Process p = YouTubeDL.downloadVideo(videoUrl);
                ProcessUpdateRow pur = new ProcessUpdateRow();
                pur.proc = p;
                pur.item = listItems.Items[listItems.Items.Count - 1];
                dict.Add(li.Tag.ToString(), pur);
            }
        }

        private void FrameMain_Load(object sender, EventArgs e)
        {
            YouTubeDL.checkDownload();
            YouTubeDL.checkForUpdates();
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
                if (pur.proc.HasExited)
                {
                    // TODO: optimize
                   BeginInvoke(new Action(() =>
                    {
                        pur.item.SubItems[1].Text = "Complete";
                        listItems.Invalidate();
                        listItems.Update();
                        listItems.Refresh();
                        Application.DoEvents();
                    }), null);
                    break;
                }
                string line = pur.proc.StandardOutput.ReadLine();
                if (!String.IsNullOrEmpty(line))
                {
                    // Console.WriteLine(line);
                    string l = Regex.Replace(line, @"\s+", " ");
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
                        BeginInvoke(new Action(() =>
                        {
                            pur.item.SubItems[1].Text = "Downloading";
                            listItems.Invalidate();
                            listItems.Update();
                            listItems.Refresh();
                            Application.DoEvents();
                        }), null);
                    }
                }
            }
         
           
            locked = false;
        }

        private void downloadAsAudioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string videoUrl = Interaction.InputBox("YouTube URL:", "Download Media As Audio", "", -1, -1);
            if (videoUrl != "")
            {
                listItems.Items.Add(new ListViewItem(new string[] { "TODO", "Waiting", "Audio", "TODO", "TODO", "TODO", "TODO", videoUrl, YouTubeDL.defaultDownloadPath }));
                YouTubeDL.downloadAudio(videoUrl);
            }
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
