namespace Parsnet
{
    using HtmlAgilityPack;
    using Parsnet.Properties;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Linq;
    using System.Net;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security.Authentication;
    using System.Text;
    using System.Threading;
    using System.Web.Script.Serialization;

    public class NetClient : INotifyPropertyChanged
    {
        public event LossConnectionHandler LossConnection;
        public event ExeptionOccuredHandler ExeptionOccured;
        public event PropertyChangedEventHandler PropertyChanged;
        public event LossConnectionHandler Discunnected;

        #region PrivateMembers
        SslStream sslStream;
        private TcpClient client;
        private byte[] data = new byte[2048];
        private String partialStr = String.Empty;
        private const int LF = 10;

        private RizehServer ServerForm;

        private int InvalidCmdCount = 0;
        private DataTable tblVisitedSites;
        private int LastSiteId = 0;

        private IAsyncResult recv_result;
        #endregion

        #region PublicMembers
        public Boolean Connected
        {
            get
            {
                return ConnectionState();
            }
        }

        public String ClientIP { get; private set; }

        public bool IsAuthenticated { get; private set; }

        public int UserId { get; private set; }

        public Commands LastCommand { get; private set; }

        public DateTime StartTime { get; private set; }

        public DateTime LastRequestTime { get; private set; }
        #endregion



        public NetClient(TcpClient ReceivedClient, RizehServer form)
        {
            try
            {
                ServerForm = form;

                client = ReceivedClient;

                sslStream = new SslStream(client.GetStream(), true);

                this.ClientIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                this.StartTime = DateTime.Now;

                OnPropertyChanged("ClientIP");
                OnPropertyChanged("StartTime");

                sslStream.AuthenticateAsServer(RizehServer.serverCertificate, false, SslProtocols.Tls, true);
                sslStream.ReadTimeout = 5000;
                sslStream.WriteTimeout = 5000;
                data = new byte[client.ReceiveBufferSize];
                recv_result = sslStream.BeginRead(data, 0, client.ReceiveBufferSize, ReceiveMessage, null);

                tblVisitedSites = new DataTable("VisitedSites");
                tblVisitedSites.Columns.Add("SiteId", typeof(int));
                tblVisitedSites.PrimaryKey = new DataColumn[1] { tblVisitedSites.Columns[0] };
            }
            catch (Exception ex)
            {
                OnExeptionOccured(MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }

        private Boolean ConnectionState()
        {
            if (client != null && client.Connected != false)
                return true;
            else
                return false;
        }


        private void ReceiveMessage(IAsyncResult ar)
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
                    OnLossConnection(this);
                    return;
                }
                else
                {
                    string messageReceived;
                    int i = 0;
                    int start = 0;

                    while (data[i] != 0)
                    {
                        //---do not scan more than what is read---
                        if (i + 1 > bytesRead)
                            break;

                        //---if LF is detected---
                        if ((int)data[i] == LF)
                        {
                            messageReceived = partialStr + Encoding.UTF8.GetString(data, start, (i - start));
                            AnalizeCommand(messageReceived);
                            start = i + 1;
                        }
                        i += 1;
                    }

                    if (start != i)
                    {
                        partialStr = Encoding.UTF8.GetString(data, start, (i - start));
                    }
                }

                recv_result = sslStream.BeginRead(data, 0, client.ReceiveBufferSize, ReceiveMessage, null);
            }
            catch (Exception ex)
            {
                OnExeptionOccured(MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }

        public void SendCommand(Commands cmd, object data)
        {
            var delg = new delSendCommand(SendTask);
            delg.BeginInvoke(cmd, data, null, null);
        }

        public void SendTask(Commands msg, object data)
        {
            try
            {
                var cmd = new Message()
                {
                    Command = msg,
                    Data = data
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
                OnExeptionOccured(MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void ProccessUserPayments(int id)
        {
            try
            {
                var dtPayments = DatabaseManager.GetUserPayments(id);
                SendCommand(Commands.UserPayments, new { dtPayments = dtPayments });
            }
            catch (Exception ex)
            {
                OnExeptionOccured(MethodInfo.GetCurrentMethod().Name, ex.Message);
            }

        }

        private void ProccessNextSite()
        {
            bool mustClear = false;
            var url = "http://www.rizeh.com";

            var vSite = DatabaseManager.GetRandomSite(this.UserId, LastSiteId, this.ClientIP, tblVisitedSites, out mustClear);
            var status = DatabaseManager.GetStatus(this.UserId);
            var siteOwnerId = DatabaseManager.GetSiteOwnerId(LastSiteId);

            if (mustClear)
            {
                tblVisitedSites.Clear();
            }

            if (vSite != null)
            {
                url = vSite.Url;
                LastSiteId = vSite.SiteId;

                if (tblVisitedSites.Rows.Find(vSite.SiteId) == null)
                {
                    var row = tblVisitedSites.NewRow();
                    row["SiteId"] = vSite.SiteId;
                    tblVisitedSites.Rows.Add(row);
                }

                var data = new
                {
                    url = url,
                    totalUsers = status.TotalUsers,
                    sites = status.TotalSites,
                    scores = status.Scores,
                    onlines = GetTotalOnlines() //RizehServer.TotalOnline
                };

                SendCommand(Commands.NextSite, data);


                //ارسال برای صاحب سایت
                try
                {
                    var target = RizehServer.Clients.SingleOrDefault(c => c.UserId == siteOwnerId);
                    if (target != null)
                    {
                        status = DatabaseManager.GetStatus(siteOwnerId);
                        target.UpdateStatus("scores", status.Scores);
                    }
                    RizehServer.ProccessUserWebsiteList(siteOwnerId);
                }
                catch (Exception ex)
                {
                    OnExeptionOccured(MethodInfo.GetCurrentMethod().Name, ex.Message);
                }

            }
            else
            {
                var data = new
                {
                    url = url,
                    totalUsers = status.TotalUsers,
                    sites = status.TotalSites,
                    scores = status.Scores,
                    onlines = GetTotalOnlines()
                };

                SendCommand(Commands.NextSite, data);
            }
        }

        private int GetTotalOnlines()
        {
            return RizehServer.Clients.Where(c => c.IsAuthenticated == true).Count();
        }

        private void UpdateSite(Dictionary<string, object> dic)
        {
            try
            {
                var id = Convert.ToInt32(dic["Id"]);
                var cell = dic["Cell"].ToString();
                var value = dic["Value"].ToString();
                string desc = null;
                string topic = null;
                bool? isActive = null;

                switch (cell)
                {
                    case "Description":
                        {
                            desc = value;
                            break;
                        }
                    case "Topic":
                        {
                            topic = value;
                            break;
                        }
                    case "IsActive":
                        {
                            isActive = Convert.ToBoolean(value);
                            break;
                        }
                    default:
                        {
                            return;
                        }
                }

                var result = DatabaseManager.UpdateSite(id, desc, topic, isActive);
            }
            catch (Exception ex)
            {
                OnExeptionOccured(MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void RegisterSite(Dictionary<string, object> dic)
        {
            var url = dic["url"].ToString();
            var topic = dic["topic"].ToString();
            var description = dic["description"].ToString();
            var result = InsertSiteResult.Error;
            HtmlNode hiden = null;

            var uri = new Uri(url);
            var webUrl = String.Format("{0}://{1}", uri.Scheme, uri.Host);

            try
            {
                var get = new HtmlWeb();
                var doc = get.Load(webUrl);
                hiden = doc.GetElementbyId("rizeh");
            }
            catch (Exception ex)
            {
                OnExeptionOccured(MethodInfo.GetCurrentMethod().Name, ex.Message);
            }

            if (hiden != null)
            {
                //var code = hiden.GetAttributeValue("value", "0");
                var site = new Sites()
                {
                    OwnerId = this.UserId,
                    Url = webUrl,
                    Topic = topic,
                    Description = description
                };

                var newSite = DatabaseManager.InsertSite(site, out result);
                if (newSite != null)
                {
                    SendCommand(Commands.RegisterSite, new { success = true, result = result, record = newSite });

                    var status = DatabaseManager.GetStatus(this.UserId);
                    RizehServer.UpdateClientsStatus("sites", status.TotalSites);
                }
                else
                {
                    SendCommand(Commands.RegisterSite, new { success = false, result = result });
                }
            }
            else
            {
                SendCommand(Commands.RegisterSite, new { success = false, result = InsertSiteResult.InvalidCode });
            }
        }

        private void ProccessConfirm(Dictionary<string, object> dic)
        {
            try
            {
                var result = DatabaseManager.ConfirmUser(dic["email"].ToString(), dic["code"].ToString());
                SendCommand(Commands.Confirm, new { result = result });
            }
            catch (Exception ex)
            {
                OnExeptionOccured(MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }

        public void Disconnect()
        {
            try
            {
                if (client.Connected)
                {
                    sslStream.Close();
                    sslStream = null;

                    client.GetStream().Close();
                    client.Close();
                    client = null;
                }
            }
            catch (Exception ex)
            {
                OnExeptionOccured(MethodInfo.GetCurrentMethod().Name, ex.Message);
            }

            OnDiscunnected(this);
        }

        private void ProccessLogin(string email, string password, string systemId)
        {
            int userId;

            var result = DatabaseManager.Login(email, password, this.ClientIP, systemId, out userId);

            if (result == LoginStatus.IsValid)
            {
                this.UserId = userId;
                OnPropertyChanged("UserId");

                this.IsAuthenticated = true;
                OnPropertyChanged("IsAuthenticated");

                var dtSites = DatabaseManager.GetUserWebsites(userId);
                var status = DatabaseManager.GetStatus(userId);

                //______________________________________________________________________________________________
                //واکشی سایت بعدی برای نمایش
                bool mustClear = false;
                var url = "http://www.rizeh.com";

                var vSite = DatabaseManager.GetRandomSite(this.UserId, LastSiteId, this.ClientIP, tblVisitedSites, out mustClear);
                if (mustClear)
                {
                    tblVisitedSites.Clear();
                }

                if (vSite != null)
                {
                    url = vSite.Url;
                    LastSiteId = vSite.SiteId;

                    if (tblVisitedSites.Rows.Find(vSite.SiteId) == null)
                    {
                        var row = tblVisitedSites.NewRow();
                        row["SiteId"] = vSite.SiteId;
                        tblVisitedSites.Rows.Add(row);
                    }
                }

                //______________________________________________________________________________________________

                var data = new
                {
                    result = result,
                    totalUsers = status.TotalUsers,
                    sites = status.TotalSites,
                    scores = status.Scores,
                    url = url,
                    onlines = GetTotalOnlines(),
                    dtSites = dtSites
                };

                SendCommand(Commands.Login, data);

                RizehServer.UpdateClientsStatus("onlines", RizehServer.Clients.Where(c => c.IsAuthenticated == true).Count());

                return;
            }
            else if (result == LoginStatus.IsNotApproved)
            {
                var code = DatabaseManager.GetConfirmCode(email);
                EmailManager.Send(email, "کد تایید ایمیل", code);
            }

            SendCommand(Commands.Login, new { result = result });
        }

        public void UpdateStatus(string key, int value)
        {
            var data = new { key = value };
            SendCommand(Commands.UpdateStatus, data);

            //var data = new
            //{
            //    totalUsers = status.TotalUsers,
            //    sites = status.TotalSites,
            //    scores = status.Scores,
            //    onlines = GetTotalOnlines()
            //};
        }

        private void AnalizeCommand(String msg)
        {
            try
            {
                var js = new JavaScriptSerializer();
                var obj = js.Deserialize<Message>(msg);

                if (CheckClientVersion(obj.Version) == false)
                {
                    return;
                }

                Dictionary<string, object> dic = (Dictionary<string, object>)obj.Data;

                this.LastRequestTime = DateTime.Now;
                this.LastCommand = obj.Command;

                OnPropertyChanged("LastRequest");
                OnPropertyChanged("LastCommand");



                switch (obj.Command)
                {
                    case Commands.Login:
                        {
                            string email = dic["email"].ToString();
                            string password = dic["password"].ToString();
                            ProccessLogin(email, password, obj.SystemId);
                            break;
                        }
                    case Commands.Signup:
                        {
                            string email = dic["email"].ToString();
                            string password = dic["password"].ToString();

                            var result = DatabaseManager.Signup(email, password);
                            SendCommand(Commands.Signup, new { result = result });
                            break;
                        }
                    case Commands.Confirm:
                        {
                            ProccessConfirm(dic);
                            break;
                        }
                    case Commands.RegisterSite:
                        {
                            RegisterSite(dic);
                            break;
                        }
                    case Commands.DeleteSite:
                        {
                            var siteId = Convert.ToInt32(dic["id"]);
                            var result = DatabaseManager.DeleteSite(siteId);
                            SendCommand(Commands.DeleteSite, new { result = result });
                            if (result == true)
                            {
                                var status = DatabaseManager.GetStatus(this.UserId);
                                RizehServer.UpdateClientsStatus("sites", status.TotalSites);
                            }
                            break;
                        }
                    case Commands.UpdateSite:
                        {
                            UpdateSite(dic);
                            break;
                        }
                    case Commands.NextSite:
                        {
                            ProccessNextSite();
                            break;
                        }
                    case Commands.UserWebsiteList:
                        {
                            RizehServer.ProccessUserWebsiteList(this.UserId);
                            break;
                        }
                    case Commands.UserPayments:
                        {
                            ProccessUserPayments(this.UserId);
                            break;
                        }
                    case Commands.ProductList:
                        {
                            var list = DatabaseManager.GetProductsList();
                            SendCommand(Commands.ProductList, new { list = list });
                            break;
                        }
                    case Commands.Pays:
                        {
                            var result = false;
                            var url = "";

                            var id = (int)dic["id"];
                            var cost = DatabaseManager.GetProductCost(id) * 10;
                            var payline = new PayLine();
                            var id_get = payline.Send(cost);

                            var pay = new Payments()
                            {
                                ProductId = id,
                                UserId = UserId,
                                IdGet = id_get,
                                Status = 0,
                                Amount = cost
                            };

                            result = DatabaseManager.InsertPayment(pay);

                            if (result == true && id_get > 0)
                            {
                                url = String.Format("http://payline.ir/payment/gateway-{0}", id_get);
                            }
                            else
                            {
                                result = false;
                            }

                            SendCommand(Commands.Pays, new { result = result, url = url });
                            break;
                        }
                    case Commands.ChangePassword:
                        {
                            var result = DatabaseManager.ChangePassword(UserId, dic["oldPassword"].ToString(), dic["newPassword"].ToString());
                            SendCommand(Commands.ChangePassword, new { result = result });
                            break;
                        }
                    case Commands.ResetPassword:
                        {
                            var email = dic["email"].ToString();
                            var userId = DatabaseManager.GetUserIdFromEmail(email);
                            if (userId > 0)
                            {
                                var key = Cryptor.EncryptRijndael(userId.ToString(), "Rizeh.com");
                                var link = String.Format("http://www.rizeh.com/reset.aspx?key={0}", key);
                                EmailManager.Send(email, "بازنشانی رمز عبور در ریزه", Settings.Default.ResetPasswordEmail.Replace("[Url]", link));
                                SendCommand(Commands.ResetPassword, new { result = true });
                            }
                            else
                            {
                                SendCommand(Commands.ResetPassword, new { result = false });
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

            }
            catch (Exception ex)
            {
                OnExeptionOccured(MethodInfo.GetCurrentMethod().Name, ex.Message);

                if (InvalidCmdCount >= 5)
                {
                    this.Disconnect();
                }
                else
                {
                    InvalidCmdCount = InvalidCmdCount + 1;
                }
            }
        }

        public static void Broadcast(Commands msg, object data, bool onlyAouth = true)
        {
            var list = (onlyAouth == true) ? RizehServer.Clients.Where(c => c.IsAuthenticated == true) : RizehServer.Clients;

            foreach (var tClient in list)
            {
                try
                {
                    tClient.SendCommand(msg, data);
                }
                catch (Exception ex)
                {
                    Log.WriteError(MethodInfo.GetCurrentMethod(), ex);
                }
            }
        }

        private bool CheckClientVersion(string version)
        {
            try
            {
                var ver = String.Format("{0}.{1}.{2}.{3}", Settings.Default.Major, Settings.Default.Minor, Settings.Default.Build, Settings.Default.Revision);
                var resultV = version.Equals(ver);
                var path = Settings.Default.ClientFilePath;
                var fileName = String.Format("RizehClient_{0}.exe", ver);
                if (resultV == false)
                {
                    SendCommand(Commands.CheckVersion, new { result = resultV, path = path, name = fileName });
                    Disconnect();
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                OnExeptionOccured(MethodInfo.GetCurrentMethod().Name, ex.Message);
                return true;
            }

        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void OnExeptionOccured(String method, String data)
        {
            if (ExeptionOccured != null)
            {
                ExeptionOccured(method, data);
            }
        }

        private void OnLossConnection(NetClient client)
        {
            if (LossConnection != null)
            {
                LossConnection(client);
            }
        }

        private void OnDiscunnected(NetClient client)
        {
            if (Discunnected != null)
            {
                Discunnected(client);
            }
        }

      
    }
}

