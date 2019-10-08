using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Parsnet
{
    public partial class FileDownloader : Form
    {
        private Thread Start;
        private string fileName;

        bool IsComplete = false;
        string url = "";
        string savePath;

        public FileDownloader(string path, string name)
        {
            InitializeComponent();

            url = path;
            fileName = name;
            savePath = String.Format("{0}\\{1}", Directory.GetCurrentDirectory(), fileName);

            DownloadBar.Value = 0;

            Start = new Thread(new ThreadStart(DownloadStart));
            Start.Start();
        }

        private void DownloadStart()
        {
            try
            {
                WebClient client = new WebClient();
                Uri uri = new Uri(url);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressCallback);
                client.DownloadFileAsync(uri, savePath);
            }
            catch (Exception ex)
            {

            }
        }

        private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            IsComplete = true;
            if (e.Error == null && e.Cancelled == false)
            {
                if (MessageBox.Show("نسخه جدید ریزه دریافت شد ، اکنون اجرا میکنید؟", "اتمام دانلود") == System.Windows.Forms.DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(savePath);
                }
            }
            else
            {
                MessageBox.Show("عملیات دریافت فایل متوقف شد ، دوباره تلاش کنید.", "خطای دریافت فایل");
            }

            this.Close();
        }



        private void DownloadProgressCallback(object sender, DownloadProgressChangedEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
             {
                 double bytesIn = double.Parse(e.BytesReceived.ToString());
                 double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
                 double percentage = bytesIn / totalBytes * 100;

                 lblTotalBits.Text = String.Format("{0:##,#} KB of {1:##,#} KB", (e.BytesReceived / 1024), (e.TotalBytesToReceive / 1024));
                 DownloadBar.Value = int.Parse(Math.Truncate(percentage).ToString());
             });
        }

        private void FileDownloader_Load(object sender, EventArgs e)
        {

        }

        private void FileDownloader_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsComplete == true)
            {
                Owner.Close();
            }
            else if (MessageBox.Show("دانلود فایل متوقف خواهد شد ، آیا مطمئن هستید؟", "انصراف", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                Owner.Close();
            }
            else
            {
                e.Cancel = true;
            }
        }

    }
}
