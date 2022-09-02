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
            this.cbVideoEncoder = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cbVideoFormat = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tabPageAudio = new System.Windows.Forms.TabPage();
            this.cbAudioEncoder = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cbAudioFormat = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.buttonDownload = new System.Windows.Forms.Button();
            this.chkBoxWriteMetadata = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkBoxIncludeAds = new System.Windows.Forms.CheckBox();
            this.chkBoxExportAudio = new System.Windows.Forms.CheckBox();
            this.chkBoxExportVideo = new System.Windows.Forms.CheckBox();
            this.chkEmbedSubs = new System.Windows.Forms.CheckBox();
            this.chkBoxEmbedThumbnail = new System.Windows.Forms.CheckBox();
            this.timerPostLoad = new System.Windows.Forms.Timer(this.components);
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonLocationBrowse = new System.Windows.Forms.Button();
            this.textLocation = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textPassword = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textUsername = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonGetCommand = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pbPreview)).BeginInit();
            this.tabImportType.SuspendLayout();
            this.tabPageVideo.SuspendLayout();
            this.tabPageAudio.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "URL:";
            // 
            // textUrl
            // 
            this.textUrl.Location = new System.Drawing.Point(147, 20);
            this.textUrl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textUrl.Name = "textUrl";
            this.textUrl.Size = new System.Drawing.Size(602, 31);
            this.textUrl.TabIndex = 1;
            this.textUrl.TextChanged += new System.EventHandler(this.textUrl_TextChanged);
            // 
            // pbPreview
            // 
            this.pbPreview.BackColor = System.Drawing.Color.Black;
            this.pbPreview.Location = new System.Drawing.Point(760, 15);
            this.pbPreview.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pbPreview.Name = "pbPreview";
            this.pbPreview.Size = new System.Drawing.Size(547, 346);
            this.pbPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbPreview.TabIndex = 2;
            this.pbPreview.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 85);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 25);
            this.label2.TabIndex = 3;
            this.label2.Text = "Title:";
            // 
            // labelTitle
            // 
            this.labelTitle.AutoEllipsis = true;
            this.labelTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelTitle.Location = new System.Drawing.Point(147, 69);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(602, 68);
            this.labelTitle.TabIndex = 4;
            this.labelTitle.Text = "N/A";
            // 
            // labelDescription
            // 
            this.labelDescription.AutoEllipsis = true;
            this.labelDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelDescription.Location = new System.Drawing.Point(147, 144);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(602, 218);
            this.labelDescription.TabIndex = 6;
            this.labelDescription.Text = "N/A";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 149);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 25);
            this.label4.TabIndex = 5;
            this.label4.Text = "Description:";
            // 
            // tabImportType
            // 
            this.tabImportType.Controls.Add(this.tabPageVideo);
            this.tabImportType.Controls.Add(this.tabPageAudio);
            this.tabImportType.Location = new System.Drawing.Point(13, 381);
            this.tabImportType.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabImportType.Name = "tabImportType";
            this.tabImportType.SelectedIndex = 0;
            this.tabImportType.Size = new System.Drawing.Size(736, 291);
            this.tabImportType.TabIndex = 7;
            // 
            // tabPageVideo
            // 
            this.tabPageVideo.Controls.Add(this.cbVideoEncoder);
            this.tabPageVideo.Controls.Add(this.label7);
            this.tabPageVideo.Controls.Add(this.cbVideoFormat);
            this.tabPageVideo.Controls.Add(this.label3);
            this.tabPageVideo.Location = new System.Drawing.Point(4, 34);
            this.tabPageVideo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPageVideo.Name = "tabPageVideo";
            this.tabPageVideo.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPageVideo.Size = new System.Drawing.Size(728, 253);
            this.tabPageVideo.TabIndex = 0;
            this.tabPageVideo.Text = "Video";
            this.tabPageVideo.UseVisualStyleBackColor = true;
            // 
            // cbVideoEncoder
            // 
            this.cbVideoEncoder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVideoEncoder.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.cbVideoEncoder.FormattingEnabled = true;
            this.cbVideoEncoder.Items.AddRange(new object[] {
            "(do not transcode)",
            "mkv",
            "mp4",
            "avi",
            "webm",
            "ogg",
            "flv"});
            this.cbVideoEncoder.Location = new System.Drawing.Point(109, 75);
            this.cbVideoEncoder.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbVideoEncoder.Name = "cbVideoEncoder";
            this.cbVideoEncoder.Size = new System.Drawing.Size(591, 28);
            this.cbVideoEncoder.TabIndex = 14;
            this.cbVideoEncoder.TextChanged += new System.EventHandler(this.cbVideoEncoder_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 75);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(80, 25);
            this.label7.TabIndex = 13;
            this.label7.Text = "Encoder:";
            // 
            // cbVideoFormat
            // 
            this.cbVideoFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVideoFormat.DropDownWidth = 600;
            this.cbVideoFormat.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.cbVideoFormat.FormattingEnabled = true;
            this.cbVideoFormat.Location = new System.Drawing.Point(109, 20);
            this.cbVideoFormat.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbVideoFormat.Name = "cbVideoFormat";
            this.cbVideoFormat.Size = new System.Drawing.Size(591, 28);
            this.cbVideoFormat.TabIndex = 12;
            this.cbVideoFormat.SelectedIndexChanged += new System.EventHandler(this.cbVideoFormat_SelectedIndexChanged);
            this.cbVideoFormat.TextChanged += new System.EventHandler(this.cbVideoFormat_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 25);
            this.label3.TabIndex = 11;
            this.label3.Text = "Format:";
            // 
            // tabPageAudio
            // 
            this.tabPageAudio.Controls.Add(this.cbAudioEncoder);
            this.tabPageAudio.Controls.Add(this.label8);
            this.tabPageAudio.Controls.Add(this.cbAudioFormat);
            this.tabPageAudio.Controls.Add(this.label6);
            this.tabPageAudio.Location = new System.Drawing.Point(4, 34);
            this.tabPageAudio.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPageAudio.Name = "tabPageAudio";
            this.tabPageAudio.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPageAudio.Size = new System.Drawing.Size(728, 253);
            this.tabPageAudio.TabIndex = 1;
            this.tabPageAudio.Text = "Audio";
            this.tabPageAudio.UseVisualStyleBackColor = true;
            // 
            // cbAudioEncoder
            // 
            this.cbAudioEncoder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAudioEncoder.Enabled = false;
            this.cbAudioEncoder.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.cbAudioEncoder.FormattingEnabled = true;
            this.cbAudioEncoder.Items.AddRange(new object[] {
            "(do not transcode)",
            "mp3",
            "aac",
            "flac",
            "m4a",
            "opus",
            "vorbis",
            "wav"});
            this.cbAudioEncoder.Location = new System.Drawing.Point(109, 75);
            this.cbAudioEncoder.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbAudioEncoder.Name = "cbAudioEncoder";
            this.cbAudioEncoder.Size = new System.Drawing.Size(591, 28);
            this.cbAudioEncoder.TabIndex = 16;
            this.cbAudioEncoder.TextChanged += new System.EventHandler(this.cbAudioEncoder_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(10, 79);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(80, 25);
            this.label8.TabIndex = 15;
            this.label8.Text = "Encoder:";
            // 
            // cbAudioFormat
            // 
            this.cbAudioFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAudioFormat.DropDownWidth = 600;
            this.cbAudioFormat.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.cbAudioFormat.FormattingEnabled = true;
            this.cbAudioFormat.Location = new System.Drawing.Point(109, 20);
            this.cbAudioFormat.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbAudioFormat.Name = "cbAudioFormat";
            this.cbAudioFormat.Size = new System.Drawing.Size(591, 28);
            this.cbAudioFormat.TabIndex = 14;
            this.cbAudioFormat.SelectedIndexChanged += new System.EventHandler(this.cbAudioFormat_SelectedIndexChanged);
            this.cbAudioFormat.TextChanged += new System.EventHandler(this.cbAudioFormat_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(73, 25);
            this.label6.TabIndex = 13;
            this.label6.Text = "Format:";
            // 
            // buttonDownload
            // 
            this.buttonDownload.Location = new System.Drawing.Point(1166, 865);
            this.buttonDownload.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonDownload.Name = "buttonDownload";
            this.buttonDownload.Size = new System.Drawing.Size(142, 48);
            this.buttonDownload.TabIndex = 8;
            this.buttonDownload.Text = "Download";
            this.buttonDownload.UseVisualStyleBackColor = true;
            this.buttonDownload.Click += new System.EventHandler(this.buttonDownload_Click);
            // 
            // chkBoxWriteMetadata
            // 
            this.chkBoxWriteMetadata.AutoSize = true;
            this.chkBoxWriteMetadata.Checked = true;
            this.chkBoxWriteMetadata.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBoxWriteMetadata.Location = new System.Drawing.Point(17, 175);
            this.chkBoxWriteMetadata.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkBoxWriteMetadata.Name = "chkBoxWriteMetadata";
            this.chkBoxWriteMetadata.Size = new System.Drawing.Size(160, 29);
            this.chkBoxWriteMetadata.TabIndex = 11;
            this.chkBoxWriteMetadata.Text = "Write Metadata";
            this.chkBoxWriteMetadata.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkBoxIncludeAds);
            this.groupBox1.Controls.Add(this.chkBoxExportAudio);
            this.groupBox1.Controls.Add(this.chkBoxExportVideo);
            this.groupBox1.Controls.Add(this.chkEmbedSubs);
            this.groupBox1.Controls.Add(this.chkBoxEmbedThumbnail);
            this.groupBox1.Controls.Add(this.chkBoxWriteMetadata);
            this.groupBox1.Location = new System.Drawing.Point(761, 522);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(547, 335);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Additional Options";
            // 
            // chkBoxIncludeAds
            // 
            this.chkBoxIncludeAds.AutoSize = true;
            this.chkBoxIncludeAds.Location = new System.Drawing.Point(283, 175);
            this.chkBoxIncludeAds.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkBoxIncludeAds.Name = "chkBoxIncludeAds";
            this.chkBoxIncludeAds.Size = new System.Drawing.Size(131, 29);
            this.chkBoxIncludeAds.TabIndex = 16;
            this.chkBoxIncludeAds.Text = "Include Ads";
            this.chkBoxIncludeAds.UseVisualStyleBackColor = true;
            // 
            // chkBoxExportAudio
            // 
            this.chkBoxExportAudio.AutoSize = true;
            this.chkBoxExportAudio.Checked = true;
            this.chkBoxExportAudio.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBoxExportAudio.Location = new System.Drawing.Point(17, 82);
            this.chkBoxExportAudio.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkBoxExportAudio.Name = "chkBoxExportAudio";
            this.chkBoxExportAudio.Size = new System.Drawing.Size(142, 29);
            this.chkBoxExportAudio.TabIndex = 15;
            this.chkBoxExportAudio.Text = "Export Audio";
            this.chkBoxExportAudio.UseVisualStyleBackColor = true;
            this.chkBoxExportAudio.CheckedChanged += new System.EventHandler(this.chkBoxExportAudio_CheckedChanged);
            // 
            // chkBoxExportVideo
            // 
            this.chkBoxExportVideo.AutoSize = true;
            this.chkBoxExportVideo.Checked = true;
            this.chkBoxExportVideo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBoxExportVideo.Location = new System.Drawing.Point(17, 45);
            this.chkBoxExportVideo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkBoxExportVideo.Name = "chkBoxExportVideo";
            this.chkBoxExportVideo.Size = new System.Drawing.Size(140, 29);
            this.chkBoxExportVideo.TabIndex = 14;
            this.chkBoxExportVideo.Text = "Export Video";
            this.chkBoxExportVideo.UseVisualStyleBackColor = true;
            this.chkBoxExportVideo.CheckedChanged += new System.EventHandler(this.chkBoxExportVideo_CheckedChanged);
            // 
            // chkEmbedSubs
            // 
            this.chkEmbedSubs.AutoSize = true;
            this.chkEmbedSubs.Location = new System.Drawing.Point(16, 250);
            this.chkEmbedSubs.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkEmbedSubs.Name = "chkEmbedSubs";
            this.chkEmbedSubs.Size = new System.Drawing.Size(138, 29);
            this.chkEmbedSubs.TabIndex = 13;
            this.chkEmbedSubs.Text = "Embed Subs";
            this.chkEmbedSubs.UseVisualStyleBackColor = true;
            // 
            // chkBoxEmbedThumbnail
            // 
            this.chkBoxEmbedThumbnail.AutoSize = true;
            this.chkBoxEmbedThumbnail.Location = new System.Drawing.Point(17, 212);
            this.chkBoxEmbedThumbnail.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkBoxEmbedThumbnail.Name = "chkBoxEmbedThumbnail";
            this.chkBoxEmbedThumbnail.Size = new System.Drawing.Size(182, 29);
            this.chkBoxEmbedThumbnail.TabIndex = 12;
            this.chkBoxEmbedThumbnail.Text = "Embed Thumbnail";
            this.chkBoxEmbedThumbnail.UseVisualStyleBackColor = true;
            // 
            // timerPostLoad
            // 
            this.timerPostLoad.Interval = 600;
            this.timerPostLoad.Tick += new System.EventHandler(this.timerPostLoad_Tick);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(1017, 865);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(142, 48);
            this.buttonCancel.TabIndex = 13;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonLocationBrowse);
            this.groupBox2.Controls.Add(this.textLocation);
            this.groupBox2.Location = new System.Drawing.Point(761, 405);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Size = new System.Drawing.Size(547, 110);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Location";
            // 
            // buttonLocationBrowse
            // 
            this.buttonLocationBrowse.Location = new System.Drawing.Point(442, 39);
            this.buttonLocationBrowse.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonLocationBrowse.Name = "buttonLocationBrowse";
            this.buttonLocationBrowse.Size = new System.Drawing.Size(98, 46);
            this.buttonLocationBrowse.TabIndex = 1;
            this.buttonLocationBrowse.Text = "Browse";
            this.buttonLocationBrowse.UseVisualStyleBackColor = true;
            this.buttonLocationBrowse.Click += new System.EventHandler(this.buttonLocationBrowse_Click);
            // 
            // textLocation
            // 
            this.textLocation.Location = new System.Drawing.Point(16, 45);
            this.textLocation.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textLocation.Name = "textLocation";
            this.textLocation.ReadOnly = true;
            this.textLocation.Size = new System.Drawing.Size(420, 31);
            this.textLocation.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textPassword);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.textUsername);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Location = new System.Drawing.Point(13, 680);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox3.Size = new System.Drawing.Size(736, 178);
            this.groupBox3.TabIndex = 15;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Authentication";
            // 
            // textPassword
            // 
            this.textPassword.Location = new System.Drawing.Point(113, 89);
            this.textPassword.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textPassword.Name = "textPassword";
            this.textPassword.PasswordChar = '•';
            this.textPassword.Size = new System.Drawing.Size(615, 31);
            this.textPassword.TabIndex = 5;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(11, 92);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(91, 25);
            this.label9.TabIndex = 4;
            this.label9.Text = "Password:";
            // 
            // textUsername
            // 
            this.textUsername.Location = new System.Drawing.Point(113, 38);
            this.textUsername.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textUsername.Name = "textUsername";
            this.textUsername.Size = new System.Drawing.Size(615, 31);
            this.textUsername.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 41);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(95, 25);
            this.label5.TabIndex = 2;
            this.label5.Text = "Username:";
            // 
            // buttonGetCommand
            // 
            this.buttonGetCommand.Location = new System.Drawing.Point(13, 865);
            this.buttonGetCommand.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonGetCommand.Name = "buttonGetCommand";
            this.buttonGetCommand.Size = new System.Drawing.Size(148, 48);
            this.buttonGetCommand.TabIndex = 16;
            this.buttonGetCommand.Text = "Get Command";
            this.buttonGetCommand.UseVisualStyleBackColor = true;
            this.buttonGetCommand.Click += new System.EventHandler(this.buttonGetCommand_Click);
            // 
            // FrameNewMedia
            // 
            this.AcceptButton = this.buttonDownload;
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(1320, 928);
            this.Controls.Add(this.buttonGetCommand);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.groupBox1);
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
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
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
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
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
        private System.Windows.Forms.CheckBox chkBoxIncludeAds;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonLocationBrowse;
        private System.Windows.Forms.TextBox textLocation;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textPassword;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textUsername;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button buttonGetCommand;
    }
}