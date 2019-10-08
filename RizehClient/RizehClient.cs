using Microsoft.Win32;
using Parsnet.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace Parsnet
{
    public partial class RizehClient : Form
    {
        protected delegate DialogResult delShowMessageBox(string title, string msg, MessageBoxButtons buttons = MessageBoxButtons.OK);
        protected delegate void delChangeDataTable(DataTable table, DataGridView grid);
        protected delegate void delProccessing(Dictionary<string, object> dic);
        protected delegate void delShowConfirmEmailForm();
        protected delegate void delLoading(bool show);

        bool ProductsLoaded = false;
        bool UserPaymentsLoaded = false;

        private Thread thLoad;

        private void Start()
        {
            SystemId = GetSystemId();

            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            Version = fvi.FileVersion;

            thLoad.Abort();
        }

        public RizehClient()
        {
            thLoad = new Thread(new ThreadStart(Start));
            thLoad.Start();

            server = new TcpClient();

            InitializeComponent();
            webBrowser1.DocumentText = Settings.Default.Note1;
        }

        private void RizehClient_Load(object sender, EventArgs e)
        {
            this.Icon = Resources.rizeh;
            mynotifyicon.Icon = this.Icon;
            wbScores.DocumentText = Settings.Default.TopScores;
            wbSteps.DocumentText = Settings.Default.BottomScore;

            splProduct.SplitterDistance = (splProduct.Width / 2) - 10;

            txtEmail.Text = "alireza@yahoo.com";
            txtPassword.Text = "13642312";

            WriteRegistrySetting();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (FormIsValid())
            {
                try
                {
                    Loading(true);

                    if (server == null || server.Connected == false)
                    {
                        ConnectToserver();
                    }

                    SendMessage(Commands.Login, new { email = txtEmail.Text, password = txtPassword.Text });
                    sslStream.BeginRead(buffer, 0, server.ReceiveBufferSize, ReceiveMessage, null);
                }
                catch (Exception ex)
                {
                    Loading(false);
                    MessageBox.Show(ex.Message);
                }
            }

        }

        private void btnSignup_Click(object sender, EventArgs e)
        {
            if (FormIsValid())
            {
                try
                {
                    Loading(true);

                    if (server == null || server.Connected == false)
                    {
                        ConnectToserver();
                    }

                    SendMessage(Commands.Signup, new { email = txtEmail.Text, password = txtPassword.Text });
                    sslStream.BeginRead(buffer, 0, server.ReceiveBufferSize, ReceiveMessage, null);

                }
                catch (Exception ex)
                {
                    Loading(false);
                    MessageBox.Show(ex.Message);
                }
            }

        }

        private void RizehClient_Resize(object sender, EventArgs e)
        {
            linkLabel1.Left = (this.Width / 2) - (linkLabel1.Width / 2);

            if (FormWindowState.Minimized == this.WindowState)
            {
                this.Hide();
            }
        }

        private void cbRunatStartup_CheckedChanged(object sender, EventArgs e)
        {
            RegistryKey root = Registry.CurrentUser.OpenSubKey("SOFTWARE", true);
            RegistryKey regKey = root.OpenSubKey("Rizeh", true);
            if (regKey == null)
            {
                regKey = root.CreateSubKey("Rizeh");
            }

            bool Checked = cbRunatStartup.Checked;
            regKey.SetValue("AutoStart", Checked);
            regKey.Flush();

            if (Checked)
            {
                //var path = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                //Registry.SetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Run", "RizehClient", path);
            }
        }

        private void lblForget_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("");
        }

        private void RizehClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            Disconnect();
            mynotifyicon.Visible = false;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.rizeh.com");
        }

        private void btnRegisterSite_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtUrl.Text))
            {
                errEmail.SetIconAlignment(txtUrl, ErrorIconAlignment.MiddleLeft);
                errEmail.SetIconPadding(txtUrl, 10);
                errEmail.SetError(txtUrl, "نشانی وب سایت یا وبلاگ را وارد کنید");
                return;
            }

            Regex reg = new Regex("http(s)?://([\\w-]+\\.)+[\\w-]+(/[\\w- ./?%&=]*)?");
            if (reg.IsMatch(txtUrl.Text))
            {
                var data = new
                 {
                     url = txtUrl.Text,
                     topic = txtTopic.Text,
                     description = txtDescription.Text
                 };

                SendMessage(Commands.RegisterSite, data);
            }
            else
            {
                errEmail.SetIconAlignment(txtUrl, ErrorIconAlignment.MiddleLeft);
                errEmail.SetIconPadding(txtUrl, 10);
                errEmail.SetError(txtUrl, "وب سایت یا وبلاگ وارد شده معتبر نیست");
            }

        }

        public void Loading(bool show)
        {
            if (this.InvokeRequired)
            {
                delLoading delg = new delLoading(Loading);
                this.Invoke(delg, new object[] { show });
            }
            else
            {
                Loader.Visible = show;
                txtEmail.Enabled = !show;
                txtPassword.Enabled = !show;
                btnLogin.Enabled = !show;
                btnSignup.Enabled = !show;
            }
        }

        public DialogResult ShowMessageBox(string title, string msg, MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            if (this.InvokeRequired)
            {
                delShowMessageBox delg = new delShowMessageBox(ShowMessageBox);
                return (DialogResult)this.Invoke(delg, new object[] { title, msg, buttons });
            }
            else
            {
                return MessageBox.Show(msg, title, buttons);
            }

        }

        public void ShowConfirmEmailForm()
        {
            if (this.InvokeRequired)
            {
                delShowConfirmEmailForm delg = new delShowConfirmEmailForm(ShowConfirmEmailForm);
                this.Invoke(delg);
            }
            else
            {
                var dialog = new EmailConfirm();
                dialog.Owner = this;
                dialog.ShowDialog();
            }
        }

        public void DoLogin(Dictionary<string, object> dic)
        {
            if (this.InvokeRequired)
            {
                delProccessing delg = new delProccessing(DoLogin);
                this.Invoke(delg, new object[] { dic });
            }
            else
            {
                int totalUers = (int)dic["totalUsers"];
                int sites = (int)dic["sites"];
                int onlines = (int)dic["onlines"];
                int scores = (int)dic["scores"];
                string url = dic["url"].ToString();

                DataSet ds = new DataSet();
                ds.ReadXml(new StringReader(dic["dtSites"].ToString()));
                DataTable dt = (ds.Tables.Count > 0) ? ds.Tables[0] : new DataTable();

                tlsCurentUser.Text = txtEmail.Text.ToLower();
                tlsOnlines.Text = string.Format("{0:#,##0}", onlines);
                tlsTotalSites.Text = string.Format("{0:#,##0}", sites);
                tlsUsers.Text = string.Format("{0:#,##0}", totalUers);
                tlsScores.Text = string.Format("{0:#,##0}", scores);
                tlsCurrentSite.Text = url;
                WebSurf.Url = new Uri(url);
                dtgUserWebSites.DataSource = dt;

                SurfPanel.Visible = true;
                MyWesitePanel.Visible = false;
                MainPanel.Visible = true;
                StartPanel.Visible = false;

                VisitTimer.Enabled = true;
            }
        }

        protected void UpdateStatus(Dictionary<string, object> dic)
        {

            if (this.InvokeRequired)
            {
                var delg = new delProccessing(UpdateStatus);
                this.Invoke(delg, new object[] { dic });
            }
            else
            {
                int totalUers = (int)dic["totalUsers"];
                int sites = (int)dic["sites"];
                int onlines = (int)dic["onlines"];
                int scores = (int)dic["scores"];

                tlsOnlines.Text = string.Format("{0:#,##0}", onlines);
                tlsTotalSites.Text = string.Format("{0:#,##0}", sites);
                tlsUsers.Text = string.Format("{0:#,##0}", totalUers);
                tlsScores.Text = string.Format("{0:#,##0}", scores);
            }
        }

        private void mynotifyicon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Maximized;
            this.Activate();
        }

        private void txtUrl_TextChanged(object sender, EventArgs e)
        {
            errEmail.SetError(txtUrl, "");
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("آیا میخواهید خارج شوید؟", "خروج از حساب", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                MainPanel.Visible = false;
                StartPanel.Visible = true;
                Disconnect();
            }
        }

        private void dtgUserWebSites_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (MessageBox.Show("سایت یا وبلاگ حذف خواهد شد. آیا مطمئن هستید؟", "حذف سایت/وبلاگ", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var id = e.Row.Cells["SiteId"].Value;
                SendMessage(Commands.DeleteSite, new { id = id });
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void dtgUserWebSites_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var row = dtgUserWebSites.Rows[e.RowIndex];
            var cell = dtgUserWebSites[e.ColumnIndex, e.RowIndex];
            var id = row.Cells["SiteId"].Value;
            var name = cell.OwningColumn.DataPropertyName;
            var value = cell.Value.ToString();
            if (String.IsNullOrEmpty(value) && name == "IsActive")
            {
                value = "false";
            }

            SendMessage(Commands.UpdateSite, new { Id = id, Cell = name, Value = value });
        }

        private void VisitTimer_Tick(object sender, EventArgs e)
        {
            if (Progress.Value == Progress.Maximum)
            {
                VisitTimer.Enabled = false;
                SendMessage(Commands.NextSite, new { success = true });
            }
            else
            {
                Progress.Value += 1;
            }
        }

        private void btnSurf_Click(object sender, EventArgs e)
        {
            MyWesitePanel.Visible = false;
            MyPaymentsPanel.Visible = false;
            SalePanel.Visible = false;
            SurfPanel.Visible = true;
        }

        private void btnMangeSites_Click(object sender, EventArgs e)
        {
            SendMessage(Commands.UserWebsiteList, new { success = true });

            SalePanel.Visible = false;
            SurfPanel.Visible = false;
            MyPaymentsPanel.Visible = false;
            MyWesitePanel.Visible = true;
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {

        }

        private void btnSale_Click(object sender, EventArgs e)
        {
            if (ProductsLoaded == false)
            {
                SendMessage(Commands.ProductList, new { success = true });
            }

            SurfPanel.Visible = false;
            MyWesitePanel.Visible = false;
            MyPaymentsPanel.Visible = false;
            SalePanel.Visible = true;

        }

        private void btnMyPayment_Click(object sender, EventArgs e)
        {
            SendMessage(Commands.UserPayments, new { success = true });

            SalePanel.Visible = false;
            SurfPanel.Visible = false;
            MyWesitePanel.Visible = false;
            MyPaymentsPanel.Visible = true;
        }

        private void ChangeDataTable(DataTable table, DataGridView grid)
        {
            if (this.InvokeRequired)
            {
                var delg = new delChangeDataTable(ChangeDataTable);
                this.Invoke(delg, new object[] { table, grid });
            }
            else
            {
                grid.DataSource = table;
            }
        }
    }
}
