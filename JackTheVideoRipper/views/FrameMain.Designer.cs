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
            this.downloadAsVideoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadAsAudioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuListItems = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.convertMediaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.openFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openURLInBrowserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openMediaInPlayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolBar = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonDownloadVideo = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDownloadAudio = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.downloadFFmpegToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadHandbrakeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadVLCPlayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            this.contextMenuListItems.SuspendLayout();
            this.toolBar.SuspendLayout();
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
            this.listItems.Location = new System.Drawing.Point(0, 72);
            this.listItems.MultiSelect = false;
            this.listItems.Name = "listItems";
            this.listItems.Size = new System.Drawing.Size(2175, 1216);
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
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.youTubeToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(2175, 33);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(50, 29);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(123, 30);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // youTubeToolStripMenuItem
            // 
            this.youTubeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.downloadAsVideoToolStripMenuItem,
            this.downloadAsAudioToolStripMenuItem});
            this.youTubeToolStripMenuItem.Name = "youTubeToolStripMenuItem";
            this.youTubeToolStripMenuItem.Size = new System.Drawing.Size(73, 29);
            this.youTubeToolStripMenuItem.Text = "&Media";
            // 
            // downloadAsVideoToolStripMenuItem
            // 
            this.downloadAsVideoToolStripMenuItem.Name = "downloadAsVideoToolStripMenuItem";
            this.downloadAsVideoToolStripMenuItem.Size = new System.Drawing.Size(256, 30);
            this.downloadAsVideoToolStripMenuItem.Text = "Download As Video";
            this.downloadAsVideoToolStripMenuItem.Click += new System.EventHandler(this.downloadAsVideoToolStripMenuItem_Click);
            // 
            // downloadAsAudioToolStripMenuItem
            // 
            this.downloadAsAudioToolStripMenuItem.Name = "downloadAsAudioToolStripMenuItem";
            this.downloadAsAudioToolStripMenuItem.Size = new System.Drawing.Size(256, 30);
            this.downloadAsAudioToolStripMenuItem.Text = "Download As Audio";
            this.downloadAsAudioToolStripMenuItem.Click += new System.EventHandler(this.downloadAsAudioToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.toolStripSeparator1,
            this.downloadFFmpegToolStripMenuItem,
            this.downloadHandbrakeToolStripMenuItem,
            this.downloadVLCPlayerToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(61, 29);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(252, 30);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
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
            this.contextMenuListItems.Size = new System.Drawing.Size(255, 130);
            // 
            // convertMediaToolStripMenuItem
            // 
            this.convertMediaToolStripMenuItem.Name = "convertMediaToolStripMenuItem";
            this.convertMediaToolStripMenuItem.Size = new System.Drawing.Size(254, 30);
            this.convertMediaToolStripMenuItem.Text = "Convert Media";
            this.convertMediaToolStripMenuItem.Click += new System.EventHandler(this.convertMediaToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(251, 6);
            // 
            // openFolderToolStripMenuItem
            // 
            this.openFolderToolStripMenuItem.Name = "openFolderToolStripMenuItem";
            this.openFolderToolStripMenuItem.Size = new System.Drawing.Size(254, 30);
            this.openFolderToolStripMenuItem.Text = "Open Folder";
            this.openFolderToolStripMenuItem.Click += new System.EventHandler(this.openFolderToolStripMenuItem_Click);
            // 
            // openURLInBrowserToolStripMenuItem
            // 
            this.openURLInBrowserToolStripMenuItem.Name = "openURLInBrowserToolStripMenuItem";
            this.openURLInBrowserToolStripMenuItem.Size = new System.Drawing.Size(254, 30);
            this.openURLInBrowserToolStripMenuItem.Text = "Open URL In Browser";
            this.openURLInBrowserToolStripMenuItem.Click += new System.EventHandler(this.openURLInBrowserToolStripMenuItem_Click);
            // 
            // openMediaInPlayerToolStripMenuItem
            // 
            this.openMediaInPlayerToolStripMenuItem.Name = "openMediaInPlayerToolStripMenuItem";
            this.openMediaInPlayerToolStripMenuItem.Size = new System.Drawing.Size(254, 30);
            this.openMediaInPlayerToolStripMenuItem.Text = "Open Media In Player";
            this.openMediaInPlayerToolStripMenuItem.Click += new System.EventHandler(this.openMediaInPlayerToolStripMenuItem_Click);
            // 
            // toolBar
            // 
            this.toolBar.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonDownloadVideo,
            this.toolStripButtonDownloadAudio});
            this.toolBar.Location = new System.Drawing.Point(0, 33);
            this.toolBar.Name = "toolBar";
            this.toolBar.Size = new System.Drawing.Size(2175, 39);
            this.toolBar.TabIndex = 2;
            this.toolBar.Text = "toolStrip1";
            // 
            // toolStripButtonDownloadVideo
            // 
            this.toolStripButtonDownloadVideo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonDownloadVideo.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonDownloadVideo.Image")));
            this.toolStripButtonDownloadVideo.ImageTransparentColor = System.Drawing.Color.Black;
            this.toolStripButtonDownloadVideo.Name = "toolStripButtonDownloadVideo";
            this.toolStripButtonDownloadVideo.Size = new System.Drawing.Size(36, 36);
            this.toolStripButtonDownloadVideo.ToolTipText = "Download Video";
            this.toolStripButtonDownloadVideo.Click += new System.EventHandler(this.toolStripButtonDownloadVideo_Click);
            // 
            // toolStripButtonDownloadAudio
            // 
            this.toolStripButtonDownloadAudio.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonDownloadAudio.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonDownloadAudio.Image")));
            this.toolStripButtonDownloadAudio.ImageTransparentColor = System.Drawing.Color.Black;
            this.toolStripButtonDownloadAudio.Name = "toolStripButtonDownloadAudio";
            this.toolStripButtonDownloadAudio.Size = new System.Drawing.Size(36, 36);
            this.toolStripButtonDownloadAudio.ToolTipText = "Download Audio";
            this.toolStripButtonDownloadAudio.Click += new System.EventHandler(this.toolStripButtonDownloadAudio_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(249, 6);
            // 
            // downloadFFmpegToolStripMenuItem
            // 
            this.downloadFFmpegToolStripMenuItem.Name = "downloadFFmpegToolStripMenuItem";
            this.downloadFFmpegToolStripMenuItem.Size = new System.Drawing.Size(270, 30);
            this.downloadFFmpegToolStripMenuItem.Text = "Download FFmpeg";
            this.downloadFFmpegToolStripMenuItem.Click += new System.EventHandler(this.downloadFFmpegToolStripMenuItem_Click);
            // 
            // downloadHandbrakeToolStripMenuItem
            // 
            this.downloadHandbrakeToolStripMenuItem.Name = "downloadHandbrakeToolStripMenuItem";
            this.downloadHandbrakeToolStripMenuItem.Size = new System.Drawing.Size(270, 30);
            this.downloadHandbrakeToolStripMenuItem.Text = "Download Handbrake";
            this.downloadHandbrakeToolStripMenuItem.Click += new System.EventHandler(this.downloadHandbrakeToolStripMenuItem_Click);
            // 
            // downloadVLCPlayerToolStripMenuItem
            // 
            this.downloadVLCPlayerToolStripMenuItem.Name = "downloadVLCPlayerToolStripMenuItem";
            this.downloadVLCPlayerToolStripMenuItem.Size = new System.Drawing.Size(270, 30);
            this.downloadVLCPlayerToolStripMenuItem.Text = "Download VLC Player";
            this.downloadVLCPlayerToolStripMenuItem.Click += new System.EventHandler(this.downloadVLCPlayerToolStripMenuItem_Click);
            // 
            // FrameMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2175, 1288);
            this.Controls.Add(this.listItems);
            this.Controls.Add(this.toolBar);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Name = "FrameMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "JackTheVideoRipper";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrameMain_FormClosing);
            this.Load += new System.EventHandler(this.FrameMain_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.contextMenuListItems.ResumeLayout(false);
            this.toolBar.ResumeLayout(false);
            this.toolBar.PerformLayout();
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
        private System.Windows.Forms.ToolStripMenuItem downloadAsVideoToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader cURL;
        private System.Windows.Forms.ToolStripMenuItem downloadAsAudioToolStripMenuItem;
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
    }
}

