using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Parsnet
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                int TotalStart = 0;
                Process[] process_list = Process.GetProcesses();
                Process CurrentProcess = Process.GetCurrentProcess();
                foreach (Process process in process_list)
                {
                    if (process.ProcessName == CurrentProcess.ProcessName)
                        TotalStart++;
                }
                if (TotalStart > 1)
                    CurrentProcess.Kill();

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                var msg = "خطایی در اجرای برنامه رخ داده است ، برنامه بسته خواهد شد";// +"\n" + "خطای شماره: " + ex.Message;
                MessageBox.Show(msg);
            }

        }
    }
}
