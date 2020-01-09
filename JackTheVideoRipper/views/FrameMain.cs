using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace JackTheVideoRipper
{
    public partial class FrameMain : Form
    {
        private static List<Tuple<string, Process>> processes = new List<Tuple<string, Process>>();
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
                processes.Add(new Tuple<string, Process>(li.Tag.ToString(), p));

               
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

         
            BeginInvoke(new Action(() =>
            {
                foreach (ListViewItem item in listItems.Items)
                {
                    foreach (Tuple<string, Process> t in processes)
                    {
                        if (item.Tag.ToString() == t.Item1)
                        {
                            Process p = t.Item2;
                            if (p.HasExited)
                            {
                                // TODO: optimize
                                item.SubItems[1].Text = "Complete";
                                break;
                            }
                            string line = p.StandardOutput.ReadLine();
                            if (!String.IsNullOrEmpty(line))
                            {
                                // Console.WriteLine(line);
                                string l = Regex.Replace(line, @"\s+", " ");
                                if (l.IndexOf("[youtube]") > -1)
                                {
                                    item.SubItems[1].Text = "Reading Metadata";
                                    listItems.Invalidate();
                                    listItems.Update();
                                    listItems.Refresh();
                                    Application.DoEvents();
                                }
                                else if(l.IndexOf("[download]") > -1)
                                {
                                    item.SubItems[1].Text = "Downloading";
                                    listItems.Invalidate();
                                    listItems.Update();
                                    listItems.Refresh();
                                    Application.DoEvents();
                                }
                            }
                        }
                    }
                }
            }), null);
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
