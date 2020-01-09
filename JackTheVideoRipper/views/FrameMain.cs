using System;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace JackTheVideoRipper
{
    public partial class FrameMain : Form
    {
        public FrameMain()
        {
            InitializeComponent();
        }

        private void downloadAsVideoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string videoUrl = Interaction.InputBox("YouTube URL:", "Download Media As Video", "", -1, -1);
            if (videoUrl != "")
            {
                listItems.Items.Add(new ListViewItem(new string[] { "TODO", "Video", "TODO", "TODO", "TODO", "TODO", videoUrl, YouTubeDL.defaultDownloadPath }));
                YouTubeDL.downloadVideo(videoUrl);
            }
        }

        private void FrameMain_Load(object sender, EventArgs e)
        {
            YouTubeDL.checkDownload();
            YouTubeDL.checkForUpdates();
        }

        private void downloadAsAudioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string videoUrl = Interaction.InputBox("YouTube URL:", "Download Media As Audio", "", -1, -1);
            if (videoUrl != "")
            {
                listItems.Items.Add(new ListViewItem(new string[] { "TODO", "Audio", "TODO", "TODO", "TODO", "TODO", videoUrl, YouTubeDL.defaultDownloadPath }));
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
    }
}
