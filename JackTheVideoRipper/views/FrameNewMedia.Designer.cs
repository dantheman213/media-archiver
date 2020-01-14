namespace JackTheVideoRipper
{
    partial class FrameNewMedia
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
            this.label1 = new System.Windows.Forms.Label();
            this.textUrl = new System.Windows.Forms.TextBox();
            this.pbPreview = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.labelTitle = new System.Windows.Forms.Label();
            this.labelDescription = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tabImportType = new System.Windows.Forms.TabControl();
            this.tabPageVideo = new System.Windows.Forms.TabPage();
            this.tabPageAudio = new System.Windows.Forms.TabPage();
            this.buttonDownload = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.cbFormat = new System.Windows.Forms.ComboBox();
            this.chkBoxWriteMetadata = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.timerPostLoad = new System.Windows.Forms.Timer(this.components);
            this.cbVideoFormat = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbAudioFormat = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.chkBoxEmbedThumbnail = new System.Windows.Forms.CheckBox();
            this.cbVideoEncoder = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.cbAudioEncoder = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.chkEmbedSubs = new System.Windows.Forms.CheckBox();
            this.chkBoxExportVideo = new System.Windows.Forms.CheckBox();
            this.chkBoxExportAudio = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbPreview)).BeginInit();
            this.tabImportType.SuspendLayout();
            this.tabPageVideo.SuspendLayout();
            this.tabPageAudio.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(74, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "URL:";
            // 
            // textUrl
            // 
            this.textUrl.Location = new System.Drawing.Point(127, 16);
            this.textUrl.Name = "textUrl";
            this.textUrl.Size = new System.Drawing.Size(547, 26);
            this.textUrl.TabIndex = 1;
            this.textUrl.TextChanged += new System.EventHandler(this.textUrl_TextChanged);
            // 
            // pbPreview
            // 
            this.pbPreview.BackColor = System.Drawing.Color.Black;
            this.pbPreview.Location = new System.Drawing.Point(684, 12);
            this.pbPreview.Name = "pbPreview";
            this.pbPreview.Size = new System.Drawing.Size(492, 277);
            this.pbPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbPreview.TabIndex = 2;
            this.pbPreview.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(72, 105);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Title:";
            // 
            // labelTitle
            // 
            this.labelTitle.AutoEllipsis = true;
            this.labelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTitle.Location = new System.Drawing.Point(120, 92);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(554, 54);
            this.labelTitle.TabIndex = 4;
            this.labelTitle.Text = "N/A";
            // 
            // labelDescription
            // 
            this.labelDescription.AutoEllipsis = true;
            this.labelDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelDescription.Location = new System.Drawing.Point(122, 152);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(552, 137);
            this.labelDescription.TabIndex = 6;
            this.labelDescription.Text = "N/A";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 156);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 20);
            this.label4.TabIndex = 5;
            this.label4.Text = "Description:";
            // 
            // tabImportType
            // 
            this.tabImportType.Controls.Add(this.tabPageVideo);
            this.tabImportType.Controls.Add(this.tabPageAudio);
            this.tabImportType.Location = new System.Drawing.Point(16, 295);
            this.tabImportType.Name = "tabImportType";
            this.tabImportType.SelectedIndex = 0;
            this.tabImportType.Size = new System.Drawing.Size(662, 285);
            this.tabImportType.TabIndex = 7;
            // 
            // tabPageVideo
            // 
            this.tabPageVideo.Controls.Add(this.cbVideoEncoder);
            this.tabPageVideo.Controls.Add(this.label7);
            this.tabPageVideo.Controls.Add(this.cbVideoFormat);
            this.tabPageVideo.Controls.Add(this.label3);
            this.tabPageVideo.Location = new System.Drawing.Point(4, 29);
            this.tabPageVideo.Name = "tabPageVideo";
            this.tabPageVideo.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageVideo.Size = new System.Drawing.Size(654, 252);
            this.tabPageVideo.TabIndex = 0;
            this.tabPageVideo.Text = "Video";
            this.tabPageVideo.UseVisualStyleBackColor = true;
            // 
            // tabPageAudio
            // 
            this.tabPageAudio.Controls.Add(this.cbAudioEncoder);
            this.tabPageAudio.Controls.Add(this.label8);
            this.tabPageAudio.Controls.Add(this.cbAudioFormat);
            this.tabPageAudio.Controls.Add(this.label6);
            this.tabPageAudio.Location = new System.Drawing.Point(4, 29);
            this.tabPageAudio.Name = "tabPageAudio";
            this.tabPageAudio.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageAudio.Size = new System.Drawing.Size(654, 252);
            this.tabPageAudio.TabIndex = 1;
            this.tabPageAudio.Text = "Audio";
            this.tabPageAudio.UseVisualStyleBackColor = true;
            // 
            // buttonDownload
            // 
            this.buttonDownload.Location = new System.Drawing.Point(1048, 589);
            this.buttonDownload.Name = "buttonDownload";
            this.buttonDownload.Size = new System.Drawing.Size(128, 38);
            this.buttonDownload.TabIndex = 8;
            this.buttonDownload.Text = "Download";
            this.buttonDownload.UseVisualStyleBackColor = true;
            this.buttonDownload.Click += new System.EventHandler(this.buttonDownload_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 51);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(114, 20);
            this.label5.TabIndex = 9;
            this.label5.Text = "Global Format:";
            // 
            // cbFormat
            // 
            this.cbFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFormat.FormattingEnabled = true;
            this.cbFormat.Location = new System.Drawing.Point(127, 48);
            this.cbFormat.Name = "cbFormat";
            this.cbFormat.Size = new System.Drawing.Size(547, 28);
            this.cbFormat.TabIndex = 10;
            // 
            // chkBoxWriteMetadata
            // 
            this.chkBoxWriteMetadata.AutoSize = true;
            this.chkBoxWriteMetadata.Checked = true;
            this.chkBoxWriteMetadata.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBoxWriteMetadata.Location = new System.Drawing.Point(15, 140);
            this.chkBoxWriteMetadata.Name = "chkBoxWriteMetadata";
            this.chkBoxWriteMetadata.Size = new System.Drawing.Size(144, 24);
            this.chkBoxWriteMetadata.TabIndex = 11;
            this.chkBoxWriteMetadata.Text = "Write Metadata";
            this.chkBoxWriteMetadata.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkBoxExportAudio);
            this.groupBox1.Controls.Add(this.chkBoxExportVideo);
            this.groupBox1.Controls.Add(this.chkEmbedSubs);
            this.groupBox1.Controls.Add(this.chkBoxEmbedThumbnail);
            this.groupBox1.Controls.Add(this.chkBoxWriteMetadata);
            this.groupBox1.Location = new System.Drawing.Point(684, 324);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(492, 252);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Additional Options";
            // 
            // timerPostLoad
            // 
            this.timerPostLoad.Enabled = true;
            this.timerPostLoad.Interval = 600;
            this.timerPostLoad.Tick += new System.EventHandler(this.timerPostLoad_Tick);
            // 
            // cbVideoFormat
            // 
            this.cbVideoFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVideoFormat.FormattingEnabled = true;
            this.cbVideoFormat.Location = new System.Drawing.Point(98, 16);
            this.cbVideoFormat.Name = "cbVideoFormat";
            this.cbVideoFormat.Size = new System.Drawing.Size(532, 28);
            this.cbVideoFormat.TabIndex = 12;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 20);
            this.label3.TabIndex = 11;
            this.label3.Text = "Format:";
            // 
            // cbAudioFormat
            // 
            this.cbAudioFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAudioFormat.FormattingEnabled = true;
            this.cbAudioFormat.Location = new System.Drawing.Point(98, 16);
            this.cbAudioFormat.Name = "cbAudioFormat";
            this.cbAudioFormat.Size = new System.Drawing.Size(532, 28);
            this.cbAudioFormat.TabIndex = 14;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(18, 19);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(64, 20);
            this.label6.TabIndex = 13;
            this.label6.Text = "Format:";
            // 
            // chkBoxEmbedThumbnail
            // 
            this.chkBoxEmbedThumbnail.AutoSize = true;
            this.chkBoxEmbedThumbnail.Checked = true;
            this.chkBoxEmbedThumbnail.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBoxEmbedThumbnail.Location = new System.Drawing.Point(15, 170);
            this.chkBoxEmbedThumbnail.Name = "chkBoxEmbedThumbnail";
            this.chkBoxEmbedThumbnail.Size = new System.Drawing.Size(163, 24);
            this.chkBoxEmbedThumbnail.TabIndex = 12;
            this.chkBoxEmbedThumbnail.Text = "Embed Thumbnail";
            this.chkBoxEmbedThumbnail.UseVisualStyleBackColor = true;
            // 
            // cbVideoEncoder
            // 
            this.cbVideoEncoder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVideoEncoder.FormattingEnabled = true;
            this.cbVideoEncoder.Items.AddRange(new object[] {
            "(Don\'t Transcode)",
            "mkv",
            "mp4",
            "avi",
            "webm",
            "ogg",
            "flv"});
            this.cbVideoEncoder.Location = new System.Drawing.Point(98, 50);
            this.cbVideoEncoder.Name = "cbVideoEncoder";
            this.cbVideoEncoder.Size = new System.Drawing.Size(532, 28);
            this.cbVideoEncoder.TabIndex = 14;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 53);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(73, 20);
            this.label7.TabIndex = 13;
            this.label7.Text = "Encoder:";
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(914, 589);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(128, 38);
            this.buttonCancel.TabIndex = 13;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // cbAudioEncoder
            // 
            this.cbAudioEncoder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAudioEncoder.FormattingEnabled = true;
            this.cbAudioEncoder.Items.AddRange(new object[] {
            "(Don\'t Transcode)",
            "mp3",
            "aac",
            "flac",
            "m4a",
            "opus",
            "vorbis",
            "wav"});
            this.cbAudioEncoder.Location = new System.Drawing.Point(98, 50);
            this.cbAudioEncoder.Name = "cbAudioEncoder";
            this.cbAudioEncoder.Size = new System.Drawing.Size(532, 28);
            this.cbAudioEncoder.TabIndex = 16;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 53);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(73, 20);
            this.label8.TabIndex = 15;
            this.label8.Text = "Encoder:";
            // 
            // chkEmbedSubs
            // 
            this.chkEmbedSubs.AutoSize = true;
            this.chkEmbedSubs.Checked = true;
            this.chkEmbedSubs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEmbedSubs.Location = new System.Drawing.Point(15, 200);
            this.chkEmbedSubs.Name = "chkEmbedSubs";
            this.chkEmbedSubs.Size = new System.Drawing.Size(127, 24);
            this.chkEmbedSubs.TabIndex = 13;
            this.chkEmbedSubs.Text = "Embed Subs";
            this.chkEmbedSubs.UseVisualStyleBackColor = true;
            // 
            // chkBoxExportVideo
            // 
            this.chkBoxExportVideo.AutoSize = true;
            this.chkBoxExportVideo.Checked = true;
            this.chkBoxExportVideo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBoxExportVideo.Location = new System.Drawing.Point(15, 36);
            this.chkBoxExportVideo.Name = "chkBoxExportVideo";
            this.chkBoxExportVideo.Size = new System.Drawing.Size(126, 24);
            this.chkBoxExportVideo.TabIndex = 14;
            this.chkBoxExportVideo.Text = "Export Video";
            this.chkBoxExportVideo.UseVisualStyleBackColor = true;
            // 
            // chkBoxExportAudio
            // 
            this.chkBoxExportAudio.AutoSize = true;
            this.chkBoxExportAudio.Checked = true;
            this.chkBoxExportAudio.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBoxExportAudio.Location = new System.Drawing.Point(15, 66);
            this.chkBoxExportAudio.Name = "chkBoxExportAudio";
            this.chkBoxExportAudio.Size = new System.Drawing.Size(126, 24);
            this.chkBoxExportAudio.TabIndex = 15;
            this.chkBoxExportAudio.Text = "Export Audio";
            this.chkBoxExportAudio.UseVisualStyleBackColor = true;
            // 
            // FrameNewMedia
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1188, 639);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cbFormat);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.buttonDownload);
            this.Controls.Add(this.tabImportType);
            this.Controls.Add(this.labelDescription);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pbPreview);
            this.Controls.Add(this.textUrl);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrameNewMedia";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New Media";
            this.Load += new System.EventHandler(this.FrameNewMedia_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbPreview)).EndInit();
            this.tabImportType.ResumeLayout(false);
            this.tabPageVideo.ResumeLayout(false);
            this.tabPageVideo.PerformLayout();
            this.tabPageAudio.ResumeLayout(false);
            this.tabPageAudio.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textUrl;
        private System.Windows.Forms.PictureBox pbPreview;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabControl tabImportType;
        private System.Windows.Forms.TabPage tabPageVideo;
        private System.Windows.Forms.TabPage tabPageAudio;
        private System.Windows.Forms.Button buttonDownload;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbFormat;
        private System.Windows.Forms.CheckBox chkBoxWriteMetadata;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Timer timerPostLoad;
        private System.Windows.Forms.ComboBox cbVideoFormat;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbAudioFormat;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox chkBoxEmbedThumbnail;
        private System.Windows.Forms.ComboBox cbVideoEncoder;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ComboBox cbAudioEncoder;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox chkEmbedSubs;
        private System.Windows.Forms.CheckBox chkBoxExportAudio;
        private System.Windows.Forms.CheckBox chkBoxExportVideo;
    }
}