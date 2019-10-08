namespace Parsnet
{
    partial class FileDownloader
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
            this.DownloadBar = new System.Windows.Forms.ProgressBar();
            this.lblTotalBits = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // DownloadBar
            // 
            this.DownloadBar.Location = new System.Drawing.Point(45, 29);
            this.DownloadBar.Name = "DownloadBar";
            this.DownloadBar.Size = new System.Drawing.Size(300, 20);
            this.DownloadBar.TabIndex = 0;
            // 
            // lblTotalBits
            // 
            this.lblTotalBits.AutoSize = true;
            this.lblTotalBits.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(178)));
            this.lblTotalBits.Location = new System.Drawing.Point(48, 61);
            this.lblTotalBits.Name = "lblTotalBits";
            this.lblTotalBits.Size = new System.Drawing.Size(70, 14);
            this.lblTotalBits.TabIndex = 1;
            this.lblTotalBits.Text = "0KB Of 0KB";
            // 
            // FileDownloader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 115);
            this.Controls.Add(this.lblTotalBits);
            this.Controls.Add(this.DownloadBar);
            this.Font = new System.Drawing.Font("Tahoma", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(178)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FileDownloader";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "دانلود نسخه جدید ریزه";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FileDownloader_FormClosing);
            this.Load += new System.EventHandler(this.FileDownloader_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar DownloadBar;
        private System.Windows.Forms.Label lblTotalBits;
    }
}