using Microsoft.Win32;
using Parsnet.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace Parsnet
{
    public partial class MainForm : Form
    {
        protected delegate DialogResult delShowMessageBox(string title, string msg, MessageBoxButtons buttons = MessageBoxButtons.OK);

        bool MustClose = false;
        bool ProductsLoaded = false;

        bool DoOffline = true;

        int SelectedStep = 0;
        int SelectedScore = 0;

        string email = "";
        string password = "";

        private Thread threadStart;

        Server server;

        /*____________________________________________________________________________________________________________________________________________*/

        public MainForm()
        {
            threadStart = new Thread(new ThreadStart(Start));
            threadStart.IsBackground = true;
            threadStart.Start();

            InitializeComponent();

            dtgMypayment.AutoGenerateColumns = false;
            dtgUserWebSites.AutoGenerateColumns = false;

            wbStart.DocumentText = Resources.Start;
            wbScores.DocumentText = Resources.TopScores;
            wbSteps.DocumentText = Resources.BottomScore;
            wbAddSiteNote.DocumentText = Resources.Note1;

            wbAddSiteNote.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(DocumentCompleted);
        }

        private void Start()
        {
            server = new Server(this);
            server.ExeptionOccured += new ExeptionOccuredHandler(server_ExeptionOccured);
            server.LossConnection += new EventHandler(server_LossConnection);
            server.ConnectFaild += new EventHandler(server_ConnectFaild);
            server.ConnectStart += new EventHandler(server_ConnectStart);
            server.ConnectSuccess += new EventHandler(server_ConnectSuccess);
            threadStart.Abort();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Icon = Resources.rizeh;
            mynotifyicon.Icon = this.Icon;

            splProduct.SplitterDistance = (splProduct.Width / 2) - 10;
        }

        private void btnCopyCode_Click(object sender, HtmlElementEventArgs e)
        {
            var html = wbAddSiteNote.Document;
            var elm = html.GetElementById("txtCode");
            if (elm != null)
            {
                try
                {
                    var text = elm.GetAttribute("value");
                    Clipboard.SetText(text);
                    MessageBox.Show("کد در حافظه کپی شد");
                }
                catch (Exception ex)
                { }
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (FormIsValid())
            {
                errEmail.SetError(txtEmail, "");
                errPassword.SetError(txtPassword, "");

                Loading(true);
                SendMessage(Commands.Login, new { email = txtEmail.Text, password = txtPassword.Text });
            }
        }

        private void btnSignup_Click(object sender, EventArgs e)
        {
            if (FormIsValid())
            {
                errEmail.SetError(txtEmail, "");
                errPassword.SetError(txtPassword, "");

                Loading(true);
                SendMessage(Commands.Signup, new { email = txtEmail.Text, password = txtPassword.Text });
            }
        }

        private void txtEmailPassword_Enter(object sender, EventArgs e)
        {
            var box = (TextBox)sender;
            box.BackColor = Color.FromArgb(3, 143, 105);
            if (box.Name == "txtEmail")
            {
                tbpEmail.BackColor = Color.FromArgb(3, 143, 105);
            }
            else
            {
                tbpPassword.BackColor = Color.FromArgb(3, 143, 105);
            }
        }

        private void txtEmailPassword_Leave(object sender, EventArgs e)
        {
            var box = (TextBox)sender;
            box.BackColor = Color.FromArgb(3, 166, 120);
            if (box.Name == "txtEmail")
            {
                tbpEmail.BackColor = Color.FromArgb(3, 166, 120);
            }
            else
            {
                tbpPassword.BackColor = Color.FromArgb(3, 166, 120);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.rizeh.com");
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("آیا میخواهید خارج شوید؟", "خروج از حساب", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                DoOffline = false;
                ReconnectTimer.Enabled = false;
                Loading(false);

                MainPanel.Visible = false;
                StartPanel.Visible = true;

                server.Disconnect();
            }
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            var frmPassword = new ChangePassword();
            frmPassword.ShowDialog(this);
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

        private void btnMangeSites_Click(object sender, EventArgs e)
        {
            SendMessage(Commands.UserWebsiteList, new { success = true });

            SalePanel.Visible = false;
            SurfPanel.Visible = false;
            MyPaymentsPanel.Visible = false;
            MyWesitePanel.Visible = true;
        }

        private void btnSurf_Click(object sender, EventArgs e)
        {
            MyWesitePanel.Visible = false;
            MyPaymentsPanel.Visible = false;
            SalePanel.Visible = false;
            SurfPanel.Visible = true;
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

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MustClose)
            {
                mynotifyicon.Visible = false;
                ReconnectTimer.Enabled = false;
                server.Disconnect();
            }
            else
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void btnRegisterSite_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtUrl.Text))
            {
                errEmail.SetIconAlignment(tbpUrl, ErrorIconAlignment.MiddleLeft);
                errEmail.SetIconPadding(tbpUrl, 10);
                errEmail.SetError(tbpUrl, "نشانی وب سایت یا وبلاگ را وارد کنید");
                return;
            }

            Regex reg = new Regex("http(s)?://([\\w-]+\\.)+[\\w-]+(/[\\w- ./?%&=]*)?");
            if (reg.IsMatch(tbpUrl.Text))
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
                errEmail.SetIconAlignment(tbpUrl, ErrorIconAlignment.MiddleLeft);
                errEmail.SetIconPadding(tbpUrl, 10);
                errEmail.SetError(tbpUrl, "وب سایت یا وبلاگ وارد شده معتبر نیست");
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            linkLabel1.Left = (this.Width / 2) - (linkLabel1.Width / 2);

            btnSaleScore.Left = (pnlBtnSaleScore.Width / 2) - (btnSaleScore.Width / 2);
            btnSaleStep.Left = (pnlBtnSaleStep.Width / 2) - (btnSaleStep.Width / 2);

            if (FormWindowState.Minimized == this.WindowState)
            {
                this.Hide();
            }
        }

        private void mynotifyicon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            this.Show();
            this.Activate();
        }

        private void DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var html = wbAddSiteNote.Document;
            var elm = html.GetElementById("btnCopyCode");
            var textBox = html.GetElementById("txtCode");
            if (elm != null)
            {
                html.GetElementById("btnCopyCode").Click += new HtmlElementEventHandler(btnCopyCode_Click);
            }
            if (textBox != null)
            {
                var value = String.Format("<input type=\"hidden\" id=\"rizeh\" value=\"{0}\" />", RondomText(8));
                textBox.SetAttribute("value", value);
            }
        }

        private void txtDescription_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == Keys.A))
            {
                if (sender != null)
                    ((TextBox)sender).SelectAll();
                e.Handled = true;
            }
        }

        private void btnSaleScore_Click(object sender, EventArgs e)
        {
            var btnType = (Button)sender;
            int ProductId = (btnType.Name == "btnSaleScore") ? SelectedScore : SelectedStep;

            if (ProductId == 0)
            {
                ShowMessageBox("انتخاب بسته", "یک بسته برای خرید انتخاب کنید");
            }
            else
            {
                btnType.Enabled = false;
                SendMessage(Commands.Pays, new { id = ProductId });
            }
        }

        private void rbScores_CheckedChanged(object sender, EventArgs e)
        {
            var rb = (RadioButton)sender;
            if (rb.Checked)
            {
                var box = (int[])rb.Tag;
                if (box[1] == 1)
                {
                    SelectedScore = box[0];
                }
                else
                {
                    SelectedStep = box[0];
                }
            }
        }

        private void lblForget_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Regex EmailRegex = new Regex(@"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,24}))$");

            if (String.IsNullOrEmpty(txtEmail.Text))
            {
                errEmail.SetError(txtEmail, "آدرس ایمیل را وارد کنید.");
            }
            else if (EmailRegex.IsMatch(txtEmail.Text) == false)
            {
                errEmail.SetError(txtEmail, "آدرس ایمیل وارد شده معتبر نمیباشد");
            }
            else
            {
                Loading(true);
                SendMessage(Commands.ChangePassword, new { email = txtEmail.Text });
            }
        }

        private void wbStart_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            wbStart.Visible = true;
        }

        private void cbRunatStartup_CheckedChanged(object sender, EventArgs e)
        {
            tsmAutoStart.Checked = cbRunatStartup.Checked;

            try
            {
                RegistryKey StartRoot = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                if (cbRunatStartup.Checked)
                {
                    StartRoot.SetValue("Rizeh", Application.ExecutablePath.ToString());
                }
                else
                {
                    StartRoot.DeleteValue("Rizeh", false);
                }

                RegistryKey root = Registry.CurrentUser.OpenSubKey("SOFTWARE", true);
                RegistryKey regKey = root.OpenSubKey("Rizeh", true);

                if (regKey == null)
                {
                    regKey = root.CreateSubKey("Rizeh");
                    regKey.SetValue("AutoStart", "False");
                    regKey.SetValue("Remember", "False");
                }
                else
                {
                    regKey.SetValue("AutoStart", cbRunatStartup.Checked);
                }


                regKey.Flush();
            }
            catch (Exception ex)
            {

            }
        }

        private void Logo_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.rizeh.com");
        }

        private void tsmExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("آیا میخواهید از برنامه خارج شوید؟", "خروج از برنامه", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                DoOffline = false;
                MustClose = true;
                this.Close();
            }
        }

        private void tsmExitAccount_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("آیا میخواهید از حساب خود خارج شوید؟", "خروج از حساب", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                DoOffline = false;

                try
                {
                    RegistryKey root = Registry.CurrentUser.OpenSubKey("SOFTWARE", true);
                    RegistryKey regKey = root.OpenSubKey("Rizeh", true);
                    if (regKey != null)
                    {
                        regKey.SetValue("Remember", false);
                        regKey.DeleteValue("Email", false);
                        regKey.DeleteValue("Password", false);
                    }

                    regKey.Flush();
                }
                catch (Exception ex)
                {

                }

                MustClose = true;
                this.Close();
            }
        }

        private void tsmShow_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            this.Show();

        }

        /*_________________________________________________________________________________________________________________________________*/

        private void server_LossConnection(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                var delg = new Action<object, EventArgs>(server_LossConnection);
                this.Invoke(delg, new object[] { sender, e });
            }
            else
            {
                if (DoOffline == true)
                {
                    lblErrors.Visible = true;
                    lblErrors.Text = "ارتباط با سرور قطع شد.";
                    GoOffline();
                }
                else
                {
                    ReconnectTimer.Enabled = false;
                }

                Loading(false);
            }
        }

        private void server_ExeptionOccured(string method, string data)
        {
            Loading(false);
            //MessageBox.Show(data, method);
        }

        private void server_ConnectFaild(object sender, EventArgs e)
        {
            ChangeMessage("ارتباط با سرور برقرار نشد.");
        }

        private void server_ConnectSuccess(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                var delg = new Action<object, EventArgs>(server_ConnectSuccess);
                this.Invoke(delg, new object[] { sender, e });
            }
            else
            {
                lblErrors.Visible = false;
            }
        }

        private void server_ConnectStart(object sender, EventArgs e)
        {
            ChangeMessage("در حال اتصال به سرور ...");
        }

        private void ChangeMessage(string msg)
        {
            if (this.InvokeRequired)
            {
                var delg = new Action<string>(ChangeMessage);
                this.Invoke(delg, new object[] { msg });
            }
            else
            {
                lblErrors.Visible = true;
                lblErrors.Text = msg;
            }
        }

        /*_________________________________________________________________________________________________________________________________*/

        public void SendMessage(Commands cmd, object data, AsyncCallback callback = null)
        {
            //var delg = new delSendMessage(SendTask);
            //delg.BeginInvoke(cmd, data, new AsyncCallback(SendCompleted), null);
            var delg = new Action<Commands, object>(SendTask);
            delg.BeginInvoke(cmd, data, new AsyncCallback(SendCompleted), null);
        }

        private void SendCompleted(IAsyncResult ar)
        {
            Loading(false);
        }

        private void SendTask(Commands cmd, object data)
        {
            server.SendMessage(cmd, data);
        }

        /*_________________________________________________________________________________________________________________________________*/

        private void Loading(bool show)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    var delg = new Action<bool>(Loading);
                    this.Invoke(delg, new object[] { show });
                }
                else
                {
                    txtEmail.Enabled = !show;
                    txtPassword.Enabled = !show;
                    btnLogin.Enabled = !show;
                    btnSignup.Enabled = !show;
                    lblForget.Enabled = !show;
                    loader.Visible = show;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private bool FormIsValid()
        {
            bool result = true;
            Regex EmailRegex = new Regex(@"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,24}))$");
            Regex PasswordRegex = new Regex(@".{4,16}");

            errPassword.SetError(txtPassword, "");
            errEmail.SetError(txtEmail, "");

            if (String.IsNullOrEmpty(txtEmail.Text))
            {
                errEmail.SetIconAlignment(tbpEmail, ErrorIconAlignment.MiddleLeft);
                errEmail.SetIconPadding(tbpEmail, 10);
                errEmail.SetError(tbpEmail, "ایمیل خود را وارد کنید");
                result = false;
            }
            else if (EmailRegex.IsMatch(txtEmail.Text) == false)
            {
                errEmail.SetIconAlignment(tbpEmail, ErrorIconAlignment.MiddleLeft);
                errEmail.SetIconPadding(tbpEmail, 10);
                errEmail.SetError(tbpEmail, "ایمیل وارد شده معتبر نیست");
                result = false;
            }

            if (String.IsNullOrEmpty(txtPassword.Text))
            {
                errPassword.SetIconAlignment(tbpPassword, ErrorIconAlignment.MiddleLeft);
                errPassword.SetIconPadding(tbpPassword, 10);
                errPassword.SetError(tbpPassword, "رمز عبور خود را وارد کنید");
                result = false;
            }
            else if (PasswordRegex.IsMatch(txtPassword.Text) == false)
            {
                errPassword.SetIconAlignment(tbpPassword, ErrorIconAlignment.MiddleLeft);
                errPassword.SetIconPadding(tbpPassword, 10);
                errPassword.SetError(tbpPassword, "رمز عبور حداقل 4 و حداکثر 16 کارکتر است");
                result = false;
            }

            return result;
        }

        private void WriteRegistrySetting()
        {
            try
            {
                RegistryKey root = Registry.CurrentUser.OpenSubKey("SOFTWARE", true);
                RegistryKey regKey = root.OpenSubKey("Rizeh", true);

                if (regKey == null)
                {
                    regKey = root.CreateSubKey("Rizeh");
                    regKey.SetValue("AutoStart", "False");
                    regKey.SetValue("Remember", "False");

                    regKey.Flush();
                }
                else
                {
                    var remember = Boolean.Parse(regKey.GetValue("Remember", "False").ToString());
                    var auto = Boolean.Parse(regKey.GetValue("AutoStart", "False").ToString());

                    if (remember)
                    {
                        txtEmail.Text = regKey.GetValue("Email", "").ToString();
                        txtPassword.Text = regKey.GetValue("Password", "").ToString();

                        DoAutoLogin();
                    }

                    cbRunatStartup.Checked = auto;
                    cbRemember.Checked = remember;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void DoAutoLogin()
        {
            Loading(true);
            ReconnectTimer.Enabled = true;

            var delg = new Action(AutoLoginTask);
            delg.BeginInvoke(null, null);
        }

        private void AutoLoginTask()
        {
            while (server == null)
            {
                Thread.Sleep(200);
            }
            SendMessage(Commands.Login, new { email = txtEmail.Text, password = txtPassword.Text });
        }

        private void EnableSaleButtons()
        {
            if (this.InvokeRequired)
            {
                var delg = new Action(EnableSaleButtons);
                this.Invoke(delg);
            }
            else
            {
                btnSaleScore.Enabled = true;
                btnSaleStep.Enabled = true;
            }
        }

        public void DoNewVersion(string path, string fileName)
        {
            if (InvokeRequired)
            {
                var delg = new Action<string, string>(DoNewVersion);
                this.Invoke(delg, new object[] { path, fileName });
            }
            else
            {
                StartPanel.Visible = true;
                MainPanel.Visible = false;

                if (ShowMessageBox("نسخه جدید", "شما باید نسخه جدید نرم افزار را دانلود کنید. الان دانلود می کنید؟", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    MustClose = true;
                    this.Hide();
                    ReconnectTimer.Enabled = false;
                    server.Disconnect();

                    mynotifyicon.Visible = false;
                    var downloader = new FileDownloader(path, fileName);
                    downloader.Show(this);
                }
                else
                {
                    server.Disconnect();
                    ForceClose();
                }
            }
        }

        public void ForceClose()
        {
            if (InvokeRequired)
            {
                var delg = new Action(ForceClose);
                this.Invoke(delg);
            }
            else
            {
                MustClose = true;
                this.Close();
            }
        }

        public void ProccessProductList(Dictionary<string, object> dic)
        {
            if (this.InvokeRequired)
            {
                var delg = new Action<Dictionary<string, object>>(ProccessProductList);
                this.Invoke(delg, new object[] { dic });
            }
            else
            {
                ProductsLoaded = true;
                var sRow = 1;
                var vRow = 1;

                DataSet ds = new DataSet();
                ds.ReadXml(new StringReader(dic["list"].ToString()));
                DataTable dt = (ds.Tables.Count > 0) ? ds.Tables[0] : new DataTable();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var row = dt.Rows[i];
                    var ProductId = int.Parse(row["ProductId"].ToString());
                    var ProductType = int.Parse(row["ProductTypeId"].ToString());
                    var ProductName = row["ProductName"].ToString();
                    var ProductCost = int.Parse(row["Cost"].ToString());
                    var ProductDiscount = int.Parse(row["Discount"].ToString());
                    var ProductPeriod = int.Parse(row["Period"].ToString());

                    var rbScores = new RadioButton()
                    {
                        AutoSize = true,
                        Dock = System.Windows.Forms.DockStyle.Fill,
                        Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(178))),
                        Margin = new System.Windows.Forms.Padding(5),
                        Size = new System.Drawing.Size(210, 30),
                        TabStop = true,
                        Text = ProductName,
                        UseVisualStyleBackColor = true,
                        Tag = new int[] { ProductId, ProductType }
                    };

                    rbScores.CheckedChanged += new EventHandler(rbScores_CheckedChanged);

                    var lblDiscount = new Label()
                    {
                        Dock = System.Windows.Forms.DockStyle.Fill,
                        Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(178))),
                        Margin = new System.Windows.Forms.Padding(5),
                        Size = new System.Drawing.Size(130, 30),
                        TextAlign = System.Drawing.ContentAlignment.MiddleCenter
                    };

                    var lblCost = new Label()
                    {
                        Dock = System.Windows.Forms.DockStyle.Fill,
                        Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(178))),
                        Margin = new System.Windows.Forms.Padding(5),
                        Size = new System.Drawing.Size(150, 30),
                        Text = String.Format("{0:#,##0} تومان", ProductCost),
                        TextAlign = System.Drawing.ContentAlignment.MiddleCenter
                    };

                    if (ProductType == 1)
                    {
                        lblDiscount.Text = (ProductDiscount > 0) ? String.Format("{0:#,##0} درصد تخفیف!", ProductDiscount) : "بدون تخفیف";

                        tblScores.Controls.Add(rbScores, 0, sRow);
                        tblScores.Controls.Add(lblDiscount, 1, sRow);
                        tblScores.Controls.Add(lblCost, 2, sRow);

                        sRow += 1;
                    }
                    else
                    {
                        lblDiscount.Text = String.Format("{0} روز", ProductPeriod);
                        var nRow = tblSteps.RowCount;

                        tblSteps.Controls.Add(rbScores, 0, vRow);
                        tblSteps.Controls.Add(lblDiscount, 1, vRow);
                        tblSteps.Controls.Add(lblCost, 2, vRow);

                        vRow += 1;
                    }
                }

            }
        }

        public void ProccessNextSite(Dictionary<string, object> dic)
        {
            if (this.InvokeRequired)
            {
                var delg = new Action<Dictionary<string, object>>(ProccessNextSite);
                this.Invoke(delg, new object[] { dic });
            }
            else
            {
                var url = dic["url"].ToString();
                WebSurf.Url = new Uri(url);
                tlsCurrentSite.Text = url;
                Progress.Value = 0;
                VisitTimer.Enabled = true;

                int totalUsers = (int)dic["totalUsers"];
                int sites = (int)dic["sites"];
                int onlines = (int)dic["onlines"];
                int scores = (int)dic["scores"];

                tlsOnlines.Text = string.Format("{0:#,##0}", onlines);
                tlsTotalSites.Text = string.Format("{0:#,##0}", sites);
                tlsUsers.Text = string.Format("{0:#,##0}", totalUsers);
                tlsScores.Text = string.Format("{0:#,##0}", scores);
            }
        }

        public void ProcessRegisterSite(Dictionary<string, object> dic)
        {
            var result = (InsertSiteResult)dic["result"];
            string title = "";
            string msg = "";
            switch (result)
            {
                case InsertSiteResult.Error:
                    {
                        title = "خطای ثبت";
                        msg = "در عملیات ثبت سایت خطایی رخ داده است ، دوباره تلاش کنید";
                        break;
                    }
                case InsertSiteResult.NoCredits:
                    {
                        title = "خطای ثبت";
                        msg = "اعتبار شما برای ثبت سایت/وبلاگ کافی نیست" + "\n" + "حداقل امتیاز برای ثبت سایت/وبلاگ ، 1000 امتیاز است";
                        break;
                    }
                case InsertSiteResult.Success:
                    {
                        title = "ثبت سایت/وبلاگ";
                        msg = "سایت/وبلاگ شما با موفقیت ثبت گردید";

                        var dataRow = (Dictionary<string, object>)dic["record"];
                        var dt = (DataTable)dtgUserWebSites.DataSource;
                        var row = dt.NewRow();
                        row["No"] = dt.Rows.Count + 1;
                        row["SiteId"] = dataRow["SiteId"];
                        row["Url"] = dataRow["Url"];
                        row["Topic"] = dataRow["Topic"];
                        row["Description"] = dataRow["Description"];
                        row["RegisterDate"] = ShamsiDate.ToShortShamsiDate((DateTime)dataRow["RegisterDate"]).ToString();
                        row["TotalVisits"] = 0;
                        row["IsActive"] = dataRow["IsActive"];

                        dt.Rows.Add(row);

                        break;
                    }
                case InsertSiteResult.InvalidCode:
                    {
                        title = "خطای ثبت";
                        msg = "کد اعتبارسنجی در وب سایت/وبلاگ شما یافت نشد ، لطفا از وجود آن در سورس سایت/وبلاگ خود اطمینان حاصل کنید";
                        break;
                    }
            }

            ShowMessageBox(title, msg);
        }

        public void ProcessSignup(Dictionary<string, object> dic)
        {
            Loading(false);

            int result = int.Parse(dic["result"].ToString());

            if (result == 0)
            {
                ShowMessageBox("خطای نام نویسی", "خطایی در روند نام نویسی شما پیش آمده است ، دوباره تلاش کنید");
            }
            else if (result == -1)
            {
                ShowMessageBox("خطای نام نویسی", "این آدرس ایمیل قبلا نام نویسی شده است");
            }
            else
            {
                ShowConfirmEmailForm();
            }
        }

        public void ProcessConfirmEmail(Dictionary<string, object> dic)
        {
            if ((bool)dic["result"])
            {
                ShowMessageBox("تایید ایمیل", "آدرس ایمیل شما تایید شد ، اکنون میتوانید وارد شوید");
            }
            else
            {
                if (ShowMessageBox("خطای تایید", "کد وارد شده معتبر نیست ، دوباره تلاش می کنید؟", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    ShowConfirmEmailForm();
                }
            }
        }

        public void ProcessResetPassword(Dictionary<string, object> dic)
        {
            var result = (bool)dic["result"];
            var msg = "";
            if (result == true)
            {
                msg = "ایمیلی حاوی لینک بازنشانی رمز عبور به آدرس ایمیل شما ارسال شد. برای تغییر رمز عبور خود بر روی آن کلیک کنید";
            }
            else
            {
                msg = "ایمیل وارد شده در پایگاه داده ی ما یافت نشد";
            }
            ShowMessageBox("بازنشانی رمز عبور", msg);
        }

        public void ProcessLogin(Dictionary<string, object> dic)
        {
            Loading(false);

            var result = (LoginStatus)dic["result"];
            switch (result)
            {
                case LoginStatus.InvalidUsername:
                case LoginStatus.InvalidPassword:
                    {
                        ShowMessageBox("خطای ورود", "نام کاربری یا رمز عبور اشتباه است");
                        break;
                    }
                case LoginStatus.IsLockedOut:
                    {
                        ShowMessageBox("خطای ورود", "حساب کاربری شما توسط سرور بسته شده است");
                        break;
                    }
                case LoginStatus.IsNotApproved:
                    {
                        if (ShowMessageBox("خطای ورود", "شما آدرس ایمیل خود را تایید نکرده اید ، اکنون میخواهید تایید کنید؟", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            ShowConfirmEmailForm();
                        }
                        break;
                    }
                case LoginStatus.IsValid:
                    {

                        try
                        {
                            RegistryKey root = Registry.CurrentUser.OpenSubKey("SOFTWARE", true);
                            RegistryKey regKey = root.OpenSubKey("Rizeh", true);
                            if (regKey == null)
                                regKey = root.CreateSubKey("Rizeh");

                            regKey.SetValue("Remember", cbRemember.Checked);
                            if (cbRemember.Checked)
                            {
                                regKey.SetValue("Email", txtEmail.Text);
                                regKey.SetValue("Password", txtPassword.Text);
                            }
                            else
                            {
                                regKey.DeleteValue("Email");
                                regKey.DeleteValue("Password");
                            }

                            regKey.Flush();
                        }
                        catch (Exception ex)
                        { }

                        DoLogin(dic);
                        break;
                    }
                default:
                    {
                        ShowMessageBox("خطای ورود", "خطایی در عملیات ورود رخ داده است ، دوباره تلاش کنید");
                        break;
                    }
            }
        }

        public DialogResult ShowMessageBox(string title, string msg, MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            if (this.InvokeRequired)
            {
                var delg = new delShowMessageBox(ShowMessageBox);
                return (DialogResult)this.Invoke(delg, new object[] { title, msg, buttons });
            }
            else
            {
                return MessageBox.Show(msg, title, buttons);
            }

        }

        private void ShowConfirmEmailForm()
        {
            if (this.InvokeRequired)
            {
                var delg = new Action(ShowConfirmEmailForm);
                this.Invoke(delg);
            }
            else
            {
                var dialog = new EmailConfirm();
                dialog.Owner = this;
                dialog.ShowDialog();
            }
        }

        public void UpdateStatus(Dictionary<string, object> dic)
        {
            if (this.InvokeRequired)
            {
                var delg = new Action<Dictionary<string, object>>(UpdateStatus);
                this.Invoke(delg, new object[] { dic });
            }
            else
            {
                int totalUers = dic.ContainsKey("totalUsers") ? (int)dic["totalUsers"] : int.Parse(tlsUsers.Text.Replace(",", ""));
                int sites = dic.ContainsKey("sites") ? (int)dic["sites"] : int.Parse(tlsTotalSites.Text.Replace(",", ""));
                int onlines = dic.ContainsKey("onlines") ? (int)dic["onlines"] : int.Parse(tlsOnlines.Text.Replace(",", ""));
                int scores = dic.ContainsKey("scores") ? (int)dic["scores"] : int.Parse(tlsScores.Text.Replace(",", ""));

                tlsOnlines.Text = string.Format("{0:#,##0}", onlines);
                tlsTotalSites.Text = string.Format("{0:#,##0}", sites);
                tlsUsers.Text = string.Format("{0:#,##0}", totalUers);
                tlsScores.Text = string.Format("{0:#,##0}", scores);
            }
        }

        public void ProcessUserWebsiteList(Dictionary<string, object> dic)
        {
            DataSet ds = new DataSet();
            ds.ReadXml(new StringReader(dic["dtSites"].ToString()));
            DataTable dt = (ds.Tables.Count > 0) ? ds.Tables[0] : new DataTable();
            ChangeDataTable(dt, dtgUserWebSites);
        }

        public void ProcessUserPayments(Dictionary<string, object> dic)
        {
            DataSet ds = new DataSet();
            ds.ReadXml(new StringReader(dic["dtPayments"].ToString()));
            DataTable dt = (ds.Tables.Count > 0) ? ds.Tables[0] : new DataTable();
            if (dt.Columns["PayDate"] != null)
            {
                dt.Columns["PayDate"].DataType = typeof(DateTime);
            }
            ChangeDataTable(dt, dtgMypayment);
        }

        public void ProcessPays(Dictionary<string, object> dic)
        {
            var result = (bool)dic["result"];
            if (result)
                System.Diagnostics.Process.Start(dic["url"].ToString());
            else
                ShowMessageBox("خطا در پرداخت", "در عملیات پرداخت خطایی رخ داده است ، مجددا تلاش کنید");

            EnableSaleButtons();
        }

        public void ProcessChangePassword(Dictionary<string, object> dic)
        {
            var result = (ChangePasswordResult)dic["result"];
            switch (result)
            {
                case ChangePasswordResult.InvalidOldPassword:
                    {
                        ShowMessageBox("خطای تغییر رمز", "رمز عبور قدیم شما درست نمیباشد");
                        break;
                    }
                case ChangePasswordResult.Success:
                    {
                        ShowMessageBox("تغییر رمز", "رمز شما با موفقیت تغییر یافت");
                        break;
                    }
                case ChangePasswordResult.Fail:
                    {
                        ShowMessageBox("خطای تغییر رمز", "در عملیات تغییر رمز خطایی رخ داد ، دوباره تلاش کنید");
                        break;
                    }
            }
        }

        private void ChangeDataTable(DataTable table, DataGridView grid)
        {
            if (this.InvokeRequired)
            {
                var delg = new Action<DataTable, DataGridView>(ChangeDataTable);
                this.Invoke(delg, new object[] { table, grid });
            }
            else
            {
                grid.DataSource = table;
            }
        }

        private void DoLogin(Dictionary<string, object> dic)
        {
            if (this.InvokeRequired)
            {
                var delg = new Action<Dictionary<string, object>>(DoLogin);
                this.Invoke(delg, new object[] { dic });
            }
            else
            {
                ReconnectTimer.Enabled = false;
                DoOffline = true;

                email = txtEmail.Text;
                password = txtPassword.Text;

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

        private string RondomText(int len, bool onlyNumber = false)
        {
            var chars = (onlyNumber) ? "0123456789" : "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";

            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, len)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());

            return result;
        }

        private void ReconnectTimer_Tick(object sender, EventArgs e)
        {
            Loading(true);
            SendMessage(Commands.Login, new { email = email, password = password });
        }

        private void GoOffline()
        {
            server.Disconnect();

            try
            {
                if (this.InvokeRequired)
                {
                    var delg = new Action(GoOffline);
                    this.Invoke(delg);
                }
                else
                {
                    ReconnectTimer.Enabled = true;
                    StartPanel.Visible = true;
                    MainPanel.Visible = false;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            WriteRegistrySetting();
        }

        private void tsmAutoStart_Click(object sender, EventArgs e)
        {
            tsmAutoStart.Checked = !tsmAutoStart.Checked;
            cbRunatStartup.Checked = tsmAutoStart.Checked;
        }

        private void cbRemember_CheckedChanged(object sender, EventArgs e)
        {

        }

    }
}
