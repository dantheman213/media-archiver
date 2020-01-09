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
            this.cType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cProgress = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cDownloadSpeed = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cETA = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cURL = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
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
            this.cStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuStrip1.SuspendLayout();
            this.contextMenuListItems.SuspendLayout();
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
            this.listItems.FullRowSelect = true;
            this.listItems.HideSelection = false;
            this.listItems.Location = new System.Drawing.Point(0, 33);
            this.listItems.Name = "listItems";
            this.listItems.Size = new System.Drawing.Size(1600, 1035);
            this.listItems.TabIndex = 0;
            this.listItems.UseCompatibleStateImageBehavior = false;
            this.listItems.View = System.Windows.Forms.View.Details;
            this.listItems.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listItems_MouseClick);
            // 
            // cTitle
            // 
            this.cTitle.Text = "Title";
            this.cTitle.Width = 400;
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
            this.cProgress.Width = 223;
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
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.youTubeToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1600, 33);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
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
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(61, 29);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(146, 30);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // contextMenuListItems
            // 
            this.contextMenuListItems.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuListItems.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.convertMediaToolStripMenuItem,
            this.toolStripMenuItem1,
            this.openFolderToolStripMenuItem});
            this.contextMenuListItems.Name = "contextMenuListItems";
            this.contextMenuListItems.Size = new System.Drawing.Size(201, 70);
            // 
            // convertMediaToolStripMenuItem
            // 
            this.convertMediaToolStripMenuItem.Name = "convertMediaToolStripMenuItem";
            this.convertMediaToolStripMenuItem.Size = new System.Drawing.Size(200, 30);
            this.convertMediaToolStripMenuItem.Text = "Convert Media";
            this.convertMediaToolStripMenuItem.Click += new System.EventHandler(this.convertMediaToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(197, 6);
            // 
            // openFolderToolStripMenuItem
            // 
            this.openFolderToolStripMenuItem.Name = "openFolderToolStripMenuItem";
            this.openFolderToolStripMenuItem.Size = new System.Drawing.Size(200, 30);
            this.openFolderToolStripMenuItem.Text = "Open Folder";
            this.openFolderToolStripMenuItem.Click += new System.EventHandler(this.openFolderToolStripMenuItem_Click);
            // 
            // cStatus
            // 
            this.cStatus.Text = "Status";
            this.cStatus.Width = 140;
            // 
            // FrameMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1600, 1068);
            this.Controls.Add(this.listItems);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FrameMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "JackTheVideoRipper";
            this.Load += new System.EventHandler(this.FrameMain_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.contextMenuListItems.ResumeLayout(false);
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
        private System.Windows.Forms.MenuStrip menuStrip1;
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
    }
}

