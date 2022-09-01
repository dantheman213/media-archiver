namespace JackTheVideoRipper
{
    partial class FrameMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrameMain));
            this.listItems = new System.Windows.Forms.ListView();
            this.cTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cProgress = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cDownloadSpeed = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cETA = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cURL = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.youTubeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadVideoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadAudioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.batchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadBatchManualToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadBatchDocumentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadBatchYouTubePlaylistlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.openDownloadFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.checkForUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.downloadVLCPlayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadHandbrakeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.downloadVS2010RedistributableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadFFmpegToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadAtomicParsleyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuListItems = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.convertMediaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.openFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openURLInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openMediaInPlayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolBar = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonDownloadVideo = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDownloadAudio = new System.Windows.Forms.ToolStripButton();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.toolbarLabelStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolBarLabelCpu = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolBarLabelMemory = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolBarLabelNetwork = new System.Windows.Forms.ToolStripStatusLabel();
            this.timerStatusBar = new System.Windows.Forms.Timer(this.components);
            this.timerCheckForUpdates = new System.Windows.Forms.Timer(this.components);
            this.timerProcessLimit = new System.Windows.Forms.Timer(this.components);
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.menuStrip.SuspendLayout();
            this.contextMenuListItems.SuspendLayout();
            this.toolBar.SuspendLayout();
            this.statusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // listItems
            // 
            this.listItems.BackColor = System.Drawing.SystemColors.Window;
            this.listItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.cTitle,
            this.cStatus,
            this.cType,
            this.cSize,
            this.cProgress,
            this.cDownloadSpeed,
            this.cETA,
            this.cURL,
            this.cPath});
            this.listItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listItems.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listItems.FullRowSelect = true;
            this.listItems.HideSelection = false;
            this.listItems.Location = new System.Drawing.Point(0, 84);
            this.listItems.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.listItems.MultiSelect = false;
            this.listItems.Name = "listItems";
            this.listItems.Size = new System.Drawing.Size(2900, 1526);
            this.listItems.SmallImageList = this.imgList;
            this.listItems.TabIndex = 0;
            this.listItems.UseCompatibleStateImageBehavior = false;
            this.listItems.View = System.Windows.Forms.View.Details;
            this.listItems.DoubleClick += new System.EventHandler(this.listItems_DoubleClick);
            this.listItems.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listItems_MouseClick);
            // 
            // cTitle
            // 
            this.cTitle.Text = "Title";
            this.cTitle.Width = 400;
            // 
            // cStatus
            // 
            this.cStatus.Text = "Status";
            this.cStatus.Width = 140;
            // 
            // cType
            // 
            this.cType.Text = "Type";
            // 
            // cSize
            // 
            this.cSize.Text = "Size";
            this.cSize.Width = 130;
            // 
            // cProgress
            // 
            this.cProgress.Text = "Progress";
            this.cProgress.Width = 120;
            // 
            // cDownloadSpeed
            // 
            this.cDownloadSpeed.Text = "Download Speed";
            this.cDownloadSpeed.Width = 157;
            // 
            // cETA
            // 
            this.cETA.Text = "ETA";
            this.cETA.Width = 147;
            // 
            // cURL
            // 
            this.cURL.Text = "URL";
            this.cURL.Width = 300;
            // 
            // cPath
            // 
            this.cPath.Text = "Path";
            this.cPath.Width = 300;
            // 
            // imgList
            // 
            this.imgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgList.ImageStream")));
            this.imgList.TransparentColor = System.Drawing.Color.Transparent;
            this.imgList.Images.SetKeyName(0, "movie.ico");
            this.imgList.Images.SetKeyName(1, "song.ico");
            // 
            // menuStrip
            // 
            this.menuStrip.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.youTubeToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip.Size = new System.Drawing.Size(2900, 42);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.toolStripSeparator5,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(72, 38);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(359, 44);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // youTubeToolStripMenuItem
            // 
            this.youTubeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.downloadVideoToolStripMenuItem,
            this.downloadAudioToolStripMenuItem,
            this.batchToolStripMenuItem,
            this.toolStripSeparator3,
            this.openDownloadFolderToolStripMenuItem});
            this.youTubeToolStripMenuItem.Name = "youTubeToolStripMenuItem";
            this.youTubeToolStripMenuItem.Size = new System.Drawing.Size(102, 38);
            this.youTubeToolStripMenuItem.Text = "&Media";
            // 
            // downloadVideoToolStripMenuItem
            // 
            this.downloadVideoToolStripMenuItem.Name = "downloadVideoToolStripMenuItem";
            this.downloadVideoToolStripMenuItem.Size = new System.Drawing.Size(397, 44);
            this.downloadVideoToolStripMenuItem.Text = "Download &Video";
            this.downloadVideoToolStripMenuItem.Click += new System.EventHandler(this.downloadVideoToolStripMenuItem_Click);
            // 
            // downloadAudioToolStripMenuItem
            // 
            this.downloadAudioToolStripMenuItem.Name = "downloadAudioToolStripMenuItem";
            this.downloadAudioToolStripMenuItem.Size = new System.Drawing.Size(397, 44);
            this.downloadAudioToolStripMenuItem.Text = "Download &Audio";
            this.downloadAudioToolStripMenuItem.Click += new System.EventHandler(this.downloadAudioToolStripMenuItem_Click);
            // 
            // batchToolStripMenuItem
            // 
            this.batchToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.downloadBatchManualToolStripMenuItem,
            this.downloadBatchDocumentToolStripMenuItem,
            this.downloadBatchYouTubePlaylistlToolStripMenuItem});
            this.batchToolStripMenuItem.Name = "batchToolStripMenuItem";
            this.batchToolStripMenuItem.Size = new System.Drawing.Size(397, 44);
            this.batchToolStripMenuItem.Text = "Download &Batch";
            // 
            // downloadBatchManualToolStripMenuItem
            // 
            this.downloadBatchManualToolStripMenuItem.Name = "downloadBatchManualToolStripMenuItem";
            this.downloadBatchManualToolStripMenuItem.Size = new System.Drawing.Size(322, 44);
            this.downloadBatchManualToolStripMenuItem.Text = "&Manual";
            this.downloadBatchManualToolStripMenuItem.Click += new System.EventHandler(this.downloadBatchManualToolStripMenuItem_Click);
            // 
            // downloadBatchDocumentToolStripMenuItem
            // 
            this.downloadBatchDocumentToolStripMenuItem.Name = "downloadBatchDocumentToolStripMenuItem";
            this.downloadBatchDocumentToolStripMenuItem.Size = new System.Drawing.Size(322, 44);
            this.downloadBatchDocumentToolStripMenuItem.Text = "Document";
            this.downloadBatchDocumentToolStripMenuItem.Click += new System.EventHandler(this.downloadBatchDocumentToolStripMenuItem_Click);
            // 
            // downloadBatchYouTubePlaylistlToolStripMenuItem
            // 
            this.downloadBatchYouTubePlaylistlToolStripMenuItem.Name = "downloadBatchYouTubePlaylistlToolStripMenuItem";
            this.downloadBatchYouTubePlaylistlToolStripMenuItem.Size = new System.Drawing.Size(322, 44);
            this.downloadBatchYouTubePlaylistlToolStripMenuItem.Text = "YouTube Playlist";
            this.downloadBatchYouTubePlaylistlToolStripMenuItem.Click += new System.EventHandler(this.downloadBatchYouTubePlaylistlToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(394, 6);
            // 
            // openDownloadFolderToolStripMenuItem
            // 
            this.openDownloadFolderToolStripMenuItem.Name = "openDownloadFolderToolStripMenuItem";
            this.openDownloadFolderToolStripMenuItem.Size = new System.Drawing.Size(397, 44);
            this.openDownloadFolderToolStripMenuItem.Text = "&Open Download Folder";
            this.openDownloadFolderToolStripMenuItem.Click += new System.EventHandler(this.openDownloadFolderToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.toolStripSeparator1,
            this.checkForUpdatesToolStripMenuItem,
            this.toolStripSeparator2,
            this.downloadVLCPlayerToolStripMenuItem,
            this.downloadHandbrakeToolStripMenuItem,
            this.toolStripSeparator4,
            this.downloadVS2010RedistributableToolStripMenuItem,
            this.downloadFFmpegToolStripMenuItem,
            this.downloadAtomicParsleyToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(85, 38);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(517, 44);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(514, 6);
            // 
            // checkForUpdatesToolStripMenuItem
            // 
            this.checkForUpdatesToolStripMenuItem.Name = "checkForUpdatesToolStripMenuItem";
            this.checkForUpdatesToolStripMenuItem.Size = new System.Drawing.Size(517, 44);
            this.checkForUpdatesToolStripMenuItem.Text = "Check For Updates";
            this.checkForUpdatesToolStripMenuItem.Click += new System.EventHandler(this.checkForUpdatesToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(514, 6);
            // 
            // downloadVLCPlayerToolStripMenuItem
            // 
            this.downloadVLCPlayerToolStripMenuItem.Name = "downloadVLCPlayerToolStripMenuItem";
            this.downloadVLCPlayerToolStripMenuItem.Size = new System.Drawing.Size(517, 44);
            this.downloadVLCPlayerToolStripMenuItem.Text = "Download VLC Player";
            this.downloadVLCPlayerToolStripMenuItem.Click += new System.EventHandler(this.downloadVLCPlayerToolStripMenuItem_Click);
            // 
            // downloadHandbrakeToolStripMenuItem
            // 
            this.downloadHandbrakeToolStripMenuItem.Name = "downloadHandbrakeToolStripMenuItem";
            this.downloadHandbrakeToolStripMenuItem.Size = new System.Drawing.Size(517, 44);
            this.downloadHandbrakeToolStripMenuItem.Text = "Download Handbrake";
            this.downloadHandbrakeToolStripMenuItem.Click += new System.EventHandler(this.downloadHandbrakeToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(514, 6);
            // 
            // downloadVS2010RedistributableToolStripMenuItem
            // 
            this.downloadVS2010RedistributableToolStripMenuItem.Name = "downloadVS2010RedistributableToolStripMenuItem";
            this.downloadVS2010RedistributableToolStripMenuItem.Size = new System.Drawing.Size(517, 44);
            this.downloadVS2010RedistributableToolStripMenuItem.Text = "Download VS 2010 Redistributable";
            this.downloadVS2010RedistributableToolStripMenuItem.Click += new System.EventHandler(this.downloadVS2010RedistributableToolStripMenuItem_Click);
            // 
            // downloadFFmpegToolStripMenuItem
            // 
            this.downloadFFmpegToolStripMenuItem.Name = "downloadFFmpegToolStripMenuItem";
            this.downloadFFmpegToolStripMenuItem.Size = new System.Drawing.Size(517, 44);
            this.downloadFFmpegToolStripMenuItem.Text = "Download FFmpeg";
            this.downloadFFmpegToolStripMenuItem.Click += new System.EventHandler(this.downloadFFmpegToolStripMenuItem_Click);
            // 
            // downloadAtomicParsleyToolStripMenuItem
            // 
            this.downloadAtomicParsleyToolStripMenuItem.Name = "downloadAtomicParsleyToolStripMenuItem";
            this.downloadAtomicParsleyToolStripMenuItem.Size = new System.Drawing.Size(517, 44);
            this.downloadAtomicParsleyToolStripMenuItem.Text = "Download AtomicParsley";
            this.downloadAtomicParsleyToolStripMenuItem.Click += new System.EventHandler(this.downloadAtomicParsleyToolStripMenuItem_Click);
            // 
            // contextMenuListItems
            // 
            this.contextMenuListItems.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuListItems.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.convertMediaToolStripMenuItem,
            this.toolStripMenuItem1,
            this.openFolderToolStripMenuItem,
            this.openURLInBrowserToolStripMenuItem,
            this.openMediaInPlayerToolStripMenuItem});
            this.contextMenuListItems.Name = "contextMenuListItems";
            this.contextMenuListItems.Size = new System.Drawing.Size(322, 162);
            // 
            // convertMediaToolStripMenuItem
            // 
            this.convertMediaToolStripMenuItem.Name = "convertMediaToolStripMenuItem";
            this.convertMediaToolStripMenuItem.Size = new System.Drawing.Size(321, 38);
            this.convertMediaToolStripMenuItem.Text = "Convert Media";
            this.convertMediaToolStripMenuItem.Click += new System.EventHandler(this.convertMediaToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(318, 6);
            // 
            // openFolderToolStripMenuItem
            // 
            this.openFolderToolStripMenuItem.Name = "openFolderToolStripMenuItem";
            this.openFolderToolStripMenuItem.Size = new System.Drawing.Size(321, 38);
            this.openFolderToolStripMenuItem.Text = "Open Folder";
            this.openFolderToolStripMenuItem.Click += new System.EventHandler(this.openFolderToolStripMenuItem_Click);
            // 
            // openURLInBrowserToolStripMenuItem
            // 
            this.openURLInBrowserToolStripMenuItem.Name = "openURLInBrowserToolStripMenuItem";
            this.openURLInBrowserToolStripMenuItem.Size = new System.Drawing.Size(321, 38);
            this.openURLInBrowserToolStripMenuItem.Text = "Open URL In Browser";
            this.openURLInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openURLInBrowserToolStripMenuItem_Click);
            // 
            // openMediaInPlayerToolStripMenuItem
            // 
            this.openMediaInPlayerToolStripMenuItem.Name = "openMediaInPlayerToolStripMenuItem";
            this.openMediaInPlayerToolStripMenuItem.Size = new System.Drawing.Size(321, 38);
            this.openMediaInPlayerToolStripMenuItem.Text = "Open Media In Player";
            this.openMediaInPlayerToolStripMenuItem.Click += new System.EventHandler(this.openMediaInPlayerToolStripMenuItem_Click);
            // 
            // toolBar
            // 
            this.toolBar.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonDownloadVideo,
            this.toolStripButtonDownloadAudio});
            this.toolBar.Location = new System.Drawing.Point(0, 42);
            this.toolBar.Name = "toolBar";
            this.toolBar.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.toolBar.Size = new System.Drawing.Size(2900, 42);
            this.toolBar.TabIndex = 2;
            this.toolBar.Text = "toolStrip1";
            // 
            // toolStripButtonDownloadVideo
            // 
            this.toolStripButtonDownloadVideo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonDownloadVideo.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonDownloadVideo.Image")));
            this.toolStripButtonDownloadVideo.ImageTransparentColor = System.Drawing.Color.Black;
            this.toolStripButtonDownloadVideo.Name = "toolStripButtonDownloadVideo";
            this.toolStripButtonDownloadVideo.Size = new System.Drawing.Size(46, 36);
            this.toolStripButtonDownloadVideo.ToolTipText = "Download Video";
            this.toolStripButtonDownloadVideo.Click += new System.EventHandler(this.toolStripButtonDownloadVideo_Click);
            // 
            // toolStripButtonDownloadAudio
            // 
            this.toolStripButtonDownloadAudio.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonDownloadAudio.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonDownloadAudio.Image")));
            this.toolStripButtonDownloadAudio.ImageTransparentColor = System.Drawing.Color.Black;
            this.toolStripButtonDownloadAudio.Name = "toolStripButtonDownloadAudio";
            this.toolStripButtonDownloadAudio.Size = new System.Drawing.Size(46, 36);
            this.toolStripButtonDownloadAudio.ToolTipText = "Download Audio";
            this.toolStripButtonDownloadAudio.Click += new System.EventHandler(this.toolStripButtonDownloadAudio_Click);
            // 
            // statusBar
            // 
            this.statusBar.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolbarLabelStatus,
            this.toolBarLabelCpu,
            this.toolBarLabelMemory,
            this.toolBarLabelNetwork});
            this.statusBar.Location = new System.Drawing.Point(0, 1568);
            this.statusBar.Name = "statusBar";
            this.statusBar.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusBar.Size = new System.Drawing.Size(2900, 42);
            this.statusBar.TabIndex = 3;
            this.statusBar.Text = "statusStrip1";
            // 
            // toolbarLabelStatus
            // 
            this.toolbarLabelStatus.Name = "toolbarLabelStatus";
            this.toolbarLabelStatus.Size = new System.Drawing.Size(96, 32);
            this.toolbarLabelStatus.Text = "Waiting";
            // 
            // toolBarLabelCpu
            // 
            this.toolBarLabelCpu.Name = "toolBarLabelCpu";
            this.toolBarLabelCpu.Size = new System.Drawing.Size(113, 32);
            this.toolBarLabelCpu.Text = "CPU: N/A";
            // 
            // toolBarLabelMemory
            // 
            this.toolBarLabelMemory.Name = "toolBarLabelMemory";
            this.toolBarLabelMemory.Size = new System.Drawing.Size(262, 32);
            this.toolBarLabelMemory.Text = "Available Memory: N/A";
            // 
            // toolBarLabelNetwork
            // 
            this.toolBarLabelNetwork.Name = "toolBarLabelNetwork";
            this.toolBarLabelNetwork.Size = new System.Drawing.Size(241, 32);
            this.toolBarLabelNetwork.Text = "Network Ingress: N/A";
            // 
            // timerStatusBar
            // 
            this.timerStatusBar.Interval = 2500;
            this.timerStatusBar.Tick += new System.EventHandler(this.timerStatusBar_Tick);
            // 
            // timerCheckForUpdates
            // 
            this.timerCheckForUpdates.Enabled = true;
            this.timerCheckForUpdates.Interval = 1000;
            this.timerCheckForUpdates.Tick += new System.EventHandler(this.timerCheckForUpdates_Tick);
            // 
            // timerProcessLimit
            // 
            this.timerProcessLimit.Interval = 3500;
            this.timerProcessLimit.Tick += new System.EventHandler(this.timerProcessLimit_Tick);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(359, 44);
            this.settingsToolStripMenuItem.Text = "&Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(356, 6);
            // 
            // FrameMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2900, 1610);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.listItems);
            this.Controls.Add(this.toolBar);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FrameMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "JackTheVideoRipper";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrameMain_FormClosing);
            this.Load += new System.EventHandler(this.FrameMain_Load);
            this.Shown += new System.EventHandler(this.FrameMain_Shown);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.contextMenuListItems.ResumeLayout(false);
            this.toolBar.ResumeLayout(false);
            this.toolBar.PerformLayout();
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listItems;
        private System.Windows.Forms.ColumnHeader cTitle;
        private System.Windows.Forms.ColumnHeader cSize;
        private System.Windows.Forms.ColumnHeader cProgress;
        private System.Windows.Forms.ColumnHeader cDownloadSpeed;
        private System.Windows.Forms.ColumnHeader cETA;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem youTubeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downloadVideoToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader cURL;
        private System.Windows.Forms.ToolStripMenuItem downloadAudioToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader cType;
        private System.Windows.Forms.ColumnHeader cPath;
        private System.Windows.Forms.ContextMenuStrip contextMenuListItems;
        private System.Windows.Forms.ToolStripMenuItem openFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertMediaToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ColumnHeader cStatus;
        private System.Windows.Forms.ImageList imgList;
        private System.Windows.Forms.ToolStripMenuItem openURLInBrowserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openMediaInPlayerToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolBar;
        private System.Windows.Forms.ToolStripButton toolStripButtonDownloadVideo;
        private System.Windows.Forms.ToolStripButton toolStripButtonDownloadAudio;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem downloadFFmpegToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downloadHandbrakeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downloadVLCPlayerToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel toolbarLabelStatus;
        private System.Windows.Forms.ToolStripStatusLabel toolBarLabelCpu;
        private System.Windows.Forms.ToolStripStatusLabel toolBarLabelMemory;
        private System.Windows.Forms.Timer timerStatusBar;
        private System.Windows.Forms.ToolStripMenuItem checkForUpdatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem downloadAtomicParsleyToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolBarLabelNetwork;
        private System.Windows.Forms.ToolStripMenuItem downloadVS2010RedistributableToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem openDownloadFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem batchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downloadBatchManualToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downloadBatchDocumentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downloadBatchYouTubePlaylistlToolStripMenuItem;
        private System.Windows.Forms.Timer timerCheckForUpdates;
        private System.Windows.Forms.Timer timerProcessLimit;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
    }
}

