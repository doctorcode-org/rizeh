using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Management;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace Parsnet
{
    partial class RizehClient
    {
        SslStream sslStream;

        string Version;

        string serverAddress = "127.0.0.1";
        int portNo = 7070;
        TcpClient server;
        byte[] buffer;
        string partialStr;

        string SystemId;

        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public void ReceiveMessage(IAsyncResult ar)
        {
            try
            {
                int bytesRead;
                lock (sslStream)
                {
                    bytesRead = sslStream.EndRead(ar);
                }

                if (bytesRead < 1)
                {
                    MessageBox.Show("ارتباط با سرور قطع شد.");
                    //ارتباط با سرور قطع شده است
                    return;
                }
                else
                {
                    string messageReceived;
                    int i = 0;
                    int start = 0;

                    while (buffer[i] != 0)
                    {
                        //---do not scan more than what is read---
                        if (i + 1 > bytesRead)
                        {
                            break;
                        }
                        //---if LF is detected---
                        if (buffer[i] == 10)
                        {
                            messageReceived = partialStr + Encoding.UTF8.GetString(buffer, start, i - start) + Environment.NewLine;
                            ParseResponse(messageReceived);
                            start = i + 1;
                        }
                        i += 1;
                    }

                    //---partial request---
                    if (start != i)
                    {
                        partialStr = Encoding.UTF8.GetString(buffer, start, i - start);
                    }
                }

                sslStream.BeginRead(buffer, 0, server.ReceiveBufferSize, ReceiveMessage, null);
            }
            catch (Exception ex)
            {

            }
        }

        private void ConnectToserver()
        {
            try
            {
                if (server == null)
                {
                    server = new TcpClient();
                }

                if (server.Connected == false)
                {
                    server.Connect(serverAddress, portNo);
                    buffer = new byte[server.ReceiveBufferSize];
                    sslStream = new SslStream(server.GetStream(), true, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                    try
                    {
                        sslStream.AuthenticateAsClient(serverAddress);
                    }
                    catch (AuthenticationException e)
                    {
                        server.Close();
                    }
                }
            }
            catch (Exception ex)
            {
            }

        }

        public void Disconnect()
        {
            try
            {
                if (server != null && server.Connected)
                {
                    server.GetStream().Close();
                    server.Close();
                    server = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        public void SendMessage(Commands msg, object data)
        {
            try
            {
                var cmd = new Message()
                {
                    Command = msg,
                    Data = data,
                    SystemId = SystemId
                };

                var js = new JavaScriptSerializer();
                string strCommend = js.Serialize(cmd);

                var dataToSend = Encoding.UTF8.GetBytes(strCommend + "\n");

                lock (sslStream)
                {
                    sslStream.Write(dataToSend, 0, dataToSend.Length);
                    sslStream.Flush();
                }
            }
            catch (Exception ex)
            {
            }
        }


        public void ParseResponse(string msg)
        {
            try
            {
                var js = new JavaScriptSerializer();
                var jsonString = msg.Trim().Substring(1);
                var obj = js.Deserialize<Message>(msg);
                Dictionary<string, object> dic = (Dictionary<string, object>)obj.Data;

                switch (obj.Command)
                {
                    case Commands.Login:
                        {
                            ProcessLogin(dic);
                            break;
                        }
                    case Commands.Signup:
                        {
                            ProcessSignup(dic);
                            break;
                        }
                    case Commands.Confirm:
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

                            break;
                        }
                    case Commands.RegisterSite:
                        {
                            ProcessRegisterSite(dic);
                            break;
                        }
                    case Commands.UpdateStatus:
                        {
                            UpdateStatus(dic);
                            break;
                        }
                    case Commands.NextSite:
                        {
                            ProccessNextSite(dic);
                            break;
                        }
                    case Commands.UserWebsiteList:
                        {
                            DataSet ds = new DataSet();
                            ds.ReadXml(new StringReader(dic["dtSites"].ToString()));
                            DataTable dt = (ds.Tables.Count > 0) ? ds.Tables[0] : new DataTable();
                            ChangeDataTable(dt, dtgUserWebSites);
                            break;
                        }
                    case Commands.UserPayments:
                        {
                            DataSet ds = new DataSet();
                            ds.ReadXml(new StringReader(dic["dtPayments"].ToString()));
                            DataTable dt = (ds.Tables.Count > 0) ? ds.Tables[0] : new DataTable();
                            ChangeDataTable(dt, dtgMypayment);
                            break;
                        }
                    case Commands.ProductList:
                        {
                            ProccessProductList(dic);
                            break;
                        }
                    default:
                        break;
                }


            }
            catch (Exception ex)
            {

            }
        }

        private void ProccessProductList(Dictionary<string, object> dic)
        {
            if (this.InvokeRequired)
            {
                var delg = new delProccessing(ProccessProductList);
                this.Invoke(delg, new object[] { dic });
            }
            else
            {
                ProductsLoaded = true;
                double totalRowScores = 0;
                double totalRowSteps = 0;

                DataSet ds = new DataSet();
                ds.ReadXml(new StringReader(dic["list"].ToString()));
                DataTable dt = (ds.Tables.Count > 0) ? ds.Tables[0] : new DataTable();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var row = dt.Rows[i];
                    var ProductType = int.Parse(row["ProductTypeId"].ToString());
                    var ProductName = int.Parse(row["ProductName"].ToString());
                    var ProductCost = int.Parse(row["Cost"].ToString());
                    var ProductDiscount = int.Parse(row["Discount"].ToString());
                    var ProductPeriod = int.Parse(row["Period"].ToString());

                    var panel = new Panel();
                    panel.BackColor = Color.FromArgb(252, 238, 33);
                    panel.ForeColor = Color.FromArgb(64, 64, 64);
                    panel.Padding = new Padding(10);
                    panel.Size = new Size(240, 180);

                    var lblProductName = new Label();
                    lblProductName.Text = (ProductType == 1) ? String.Format("{0:#,##0} امتیاز", ProductName) : String.Format("بازدید + {0}", ProductName);
                    lblProductName.Font = new Font("Tahoma", 25F, FontStyle.Bold, GraphicsUnit.Pixel, ((byte)(178)));
                    lblProductName.Dock = DockStyle.Top;
                    lblProductName.Size = new Size(220, 45);
                    lblProductName.TextAlign = ContentAlignment.MiddleCenter;

                    var lblCost = new Label();
                    lblCost.Text = String.Format("{0:#,##0} تومان", ProductCost);
                    lblCost.Font = new Font("Tahoma", 21F, FontStyle.Regular, GraphicsUnit.Pixel, ((byte)(178)));
                    lblCost.Dock = DockStyle.Top;
                    lblCost.Size = new Size(220, 35);
                    lblCost.TextAlign = ContentAlignment.MiddleCenter;

                    var lblDiscount = new Label();
                    if (ProductType == 1)
                    {
                        lblDiscount.Text = (ProductDiscount > 0) ? String.Format("{0:#,##0} درصد تخفیف!", ProductDiscount) : "بدون تخفیف";
                    }
                    else
                    {
                        lblDiscount.Text = String.Format("{0} روز", ProductPeriod);
                    }


                    lblDiscount.Font = new Font("Tahoma", 17F, FontStyle.Regular, GraphicsUnit.Pixel, ((byte)(178)));
                    lblDiscount.Dock = DockStyle.Top;
                    lblDiscount.Size = new Size(220, 35);
                    lblDiscount.TextAlign = ContentAlignment.MiddleCenter;

                    var btnPay = new Button();
                    btnPay.BackColor = Color.Transparent;
                    btnPay.BackgroundImage = global::Parsnet.Properties.Resources.paybtn;
                    btnPay.Cursor = Cursors.Hand;
                    btnPay.FlatAppearance.BorderSize = 0;
                    btnPay.FlatStyle = FlatStyle.Flat;
                    btnPay.Font = new Font("Tahoma", 18F, FontStyle.Bold, GraphicsUnit.Pixel, ((byte)(178)));
                    btnPay.Location = new Point(57, 135);
                    btnPay.Tag = row["ProductId"];
                    //btnPay.Name = "button1";
                    btnPay.Size = new Size(127, 33);
                    btnPay.UseVisualStyleBackColor = false;
                    btnPay.Click += new System.EventHandler(this.btnSale_Click);

                    panel.Controls.Add(lblDiscount);
                    panel.Controls.Add(lblCost);
                    panel.Controls.Add(lblProductName);
                    panel.Controls.Add(btnPay);

                    if (ProductType == 1)
                    {
                        flpScores.Controls.Add(panel);
                        totalRowScores += 1;
                    }
                    else
                    {
                        flpSteps.Controls.Add(panel);
                        totalRowSteps += 1;
                    }
                }

                var pCount = pnlScores.Width / 240;
                flpScores.Width = pCount * 250;
                flpScores.Height = (int)(Math.Ceiling(totalRowScores / pCount) * 190);
                var x = (pnlScores.Width / 2) - (flpScores.Width / 2);
                flpScores.Location = new Point(x, 0);

                pCount = pnlSteps.Width / 240;
                flpSteps.Width = pCount * 250;
                flpSteps.Height = (int)(Math.Ceiling(totalRowSteps / pCount) * 190);
                x = (pnlSteps.Width / 2) - (flpSteps.Width / 2);
                flpSteps.Location = new Point(x, 0);
            }
        }

        private void ProccessNextSite(Dictionary<string, object> dic)
        {
            if (this.InvokeRequired)
            {
                var delg = new delProccessing(ProccessNextSite);
                this.Invoke(delg, new object[] { dic });
            }
            else
            {
                var url = dic["url"].ToString();
                WebSurf.Url = new Uri(url);
                tlsCurrentSite.Text = url;
                Progress.Value = 0;
                VisitTimer.Enabled = true;
            }
        }

        private void ProcessRegisterSite(Dictionary<string, object> dic)
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

        private void ProcessSignup(Dictionary<string, object> dic)
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

        private void ProcessLogin(Dictionary<string, object> dic)
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


        private bool FormIsValid()
        {
            bool result = true;
            Regex EmailRegex = new Regex(@"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,24}))$");
            Regex PasswordRegex = new Regex(@".{8,16}");

            errPassword.SetError(txtPassword, "");
            errEmail.SetError(txtEmail, "");

            if (String.IsNullOrEmpty(txtEmail.Text))
            {
                errEmail.SetIconAlignment(txtEmail, ErrorIconAlignment.MiddleLeft);
                errEmail.SetIconPadding(txtEmail, 10);
                errEmail.SetError(txtEmail, "ایمیل خود را وارد کنید");
                result = false;
            }
            else if (EmailRegex.IsMatch(txtEmail.Text) == false)
            {
                errEmail.SetIconAlignment(txtEmail, ErrorIconAlignment.MiddleLeft);
                errEmail.SetIconPadding(txtEmail, 10);
                errEmail.SetError(txtEmail, "ایمیل وارد شده معتبر نیست");
                result = false;
            }

            if (String.IsNullOrEmpty(txtPassword.Text))
            {
                errPassword.SetIconAlignment(txtPassword, ErrorIconAlignment.MiddleLeft);
                errPassword.SetIconPadding(txtPassword, 10);
                errPassword.SetError(txtPassword, "رمز عبور خود را وارد کنید");
                result = false;
            }
            else if (PasswordRegex.IsMatch(txtPassword.Text) == false)
            {
                errPassword.SetIconAlignment(txtPassword, ErrorIconAlignment.MiddleLeft);
                errPassword.SetIconPadding(txtPassword, 10);
                errPassword.SetError(txtPassword, "رمز عبور حداقل 8 و حداکثر 16 کارکتر است");
                result = false;
            }

            return result;
        }

        private string GetSystemId()
        {
            string cpuInfo = string.Empty;
            ManagementClass mc = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = mc.GetInstances();

            foreach (ManagementObject mo in moc)
            {
                cpuInfo = mo.Properties["processorID"].Value.ToString();
                break;
            }

            string drive = "C";
            ManagementObject dsk = new ManagementObject(@"win32_logicaldisk.deviceid=""" + drive + @":""");
            dsk.Get();
            string volumeSerial = dsk["VolumeSerialNumber"].ToString();

            return cpuInfo + volumeSerial;
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
                }
                else
                {
                    var remember = Boolean.Parse(regKey.GetValue("Remember", "False").ToString());
                    var auto = Boolean.Parse(regKey.GetValue("AutoStart", "False").ToString());

                    if (remember)
                    {
                        txtEmail.Text = regKey.GetValue("Email", "").ToString();
                        txtPassword.Text = regKey.GetValue("Password", "").ToString();
                    }

                    cbRunatStartup.Checked = auto;
                    cbRemember.Checked = remember;
                }

                regKey.Flush();
            }
            catch (Exception ex)
            {

            }
        }

    }
}
