namespace JackTheVideoRipper
{
    partial class FrameAbout
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
            this.label1 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.labelVersion = new System.Windows.Forms.Label();
            this.textExtractors = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textDependencies = new System.Windows.Forms.TextBox();
            this.labelYouTubeDL = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(464, 52);
            this.label1.TabIndex = 0;
            this.label1.Text = "Jack The Video Ripper";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel1.Location = new System.Drawing.Point(636, 18);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(111, 25);
            this.linkLabel1.TabIndex = 1;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Visit Github";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.BackColor = System.Drawing.Color.Transparent;
            this.labelVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelVersion.Location = new System.Drawing.Point(468, 28);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(75, 29);
            this.labelVersion.TabIndex = 2;
            this.labelVersion.Text = "v0.0.0";
            // 
            // textExtractors
            // 
            this.textExtractors.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textExtractors.Location = new System.Drawing.Point(12, 143);
            this.textExtractors.Multiline = true;
            this.textExtractors.Name = "textExtractors";
            this.textExtractors.ReadOnly = true;
            this.textExtractors.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textExtractors.Size = new System.Drawing.Size(735, 430);
            this.textExtractors.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 116);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(152, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "Supported Services:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 583);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(116, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "Dependencies:";
            // 
            // textDependencies
            // 
            this.textDependencies.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textDependencies.Location = new System.Drawing.Point(12, 610);
            this.textDependencies.Multiline = true;
            this.textDependencies.Name = "textDependencies";
            this.textDependencies.ReadOnly = true;
            this.textDependencies.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textDependencies.Size = new System.Drawing.Size(735, 229);
            this.textDependencies.TabIndex = 5;
            this.textDependencies.Text = "* Visual C++ 2010 Redistributable (x86)\r\n* youtube-dl\r\n* ffmpeg\r\n* AtomicParsley";
            // 
            // labelYouTubeDL
            // 
            this.labelYouTubeDL.AutoSize = true;
            this.labelYouTubeDL.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelYouTubeDL.ForeColor = System.Drawing.Color.Red;
            this.labelYouTubeDL.Location = new System.Drawing.Point(16, 70);
            this.labelYouTubeDL.Name = "labelYouTubeDL";
            this.labelYouTubeDL.Size = new System.Drawing.Size(152, 25);
            this.labelYouTubeDL.TabIndex = 7;
            this.labelYouTubeDL.Text = "youtube-dl 0.0.0";
            // 
            // FrameAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(759, 851);
            this.Controls.Add(this.labelYouTubeDL);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textDependencies);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textExtractors);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrameAbout";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            this.Load += new System.EventHandler(this.FrameAbout_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.TextBox textExtractors;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textDependencies;
        private System.Windows.Forms.Label labelYouTubeDL;
    }
}