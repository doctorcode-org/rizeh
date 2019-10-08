using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.Script.Serialization;

namespace Parsnet
{
    public delegate void ExeptionOccuredHandler(String method, String data);

    public class Server
    {
        #region Field
        SslStream sslStream;
        String serverAddress = "62.60.138.31";
        int portNo = 7070;
        TcpClient server;
        Byte[] buffer;
        String partialStr;
        IAsyncResult recv_result;
        String systemId;
        String clientVersion;
        MainForm Owner;
        #endregion

        public event EventHandler LossConnection;
        public event EventHandler ConnectFaild;
        public event ExeptionOccuredHandler ExeptionOccured;
        public event EventHandler ConnectStart;
        public event EventHandler ConnectSuccess;
        public event EventHandler SendStart;
        public event EventHandler SendSuccess;

        public Server(MainForm form)
        {
            Owner = form;

            systemId = GetSystemId();

            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            clientVersion = fvi.FileVersion;


        }

        public bool Connect()
        {
            try
            {
                OnConnectStart();

                server = new TcpClient();
                server.Connect(serverAddress, portNo);

                buffer = new byte[server.ReceiveBufferSize];
                sslStream = new SslStream(server.GetStream(), true, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                try
                {
                    sslStream.AuthenticateAsClient(serverAddress);
                    sslStream.BeginRead(buffer, 0, server.ReceiveBufferSize, ReceiveAsync, null);
                }
                catch (AuthenticationException e)
                {
                    OnExeptionOccured(MethodInfo.GetCurrentMethod().Name, e.Message);
                    OnConnectFaild();
                    server.Close();
                    return false;
                }

                OnConnectSuccess();
                return true;
            }
            catch (Exception ex)
            {
                OnExeptionOccured(MethodInfo.GetCurrentMethod().Name, ex.Message);
                OnConnectFaild();
                return false;
            }
        }

        private void ReceiveAsync(IAsyncResult ar)
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
                    OnLossConnection();
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

                recv_result = sslStream.BeginRead(buffer, 0, server.ReceiveBufferSize, ReceiveAsync, null);
            }
            catch (Exception ex)
            {
                OnExeptionOccured(MethodInfo.GetCurrentMethod().Name, ex.Message);
                OnLossConnection();
            }
        }

        private void ParseResponse(string response)
        {
            try
            {
                var js = new JavaScriptSerializer();
                var jsonString = response.Trim().Substring(1);
                var obj = js.Deserialize<Message>(response);
                Dictionary<string, object> dic = (Dictionary<string, object>)obj.Data;

                switch (obj.Command)
                {
                    case Commands.Login:
                        {
                            Owner.ProcessLogin(dic);
                            break;
                        }
                    case Commands.Signup:
                        {
                            Owner.ProcessSignup(dic);
                            break;
                        }
                    case Commands.Confirm:
                        {
                            Owner.ProcessConfirmEmail(dic);
                            break;
                        }
                    case Commands.RegisterSite:
                        {
                            Owner.ProcessRegisterSite(dic);
                            break;
                        }
                    case Commands.UpdateStatus:
                        {
                            Owner.UpdateStatus(dic);
                            break;
                        }
                    case Commands.NextSite:
                        {
                            Owner.ProccessNextSite(dic);
                            break;
                        }
                    case Commands.UserWebsiteList:
                        {
                            Owner.ProcessUserWebsiteList(dic);
                            break;
                        }
                    case Commands.UserPayments:
                        {
                            Owner.ProcessUserPayments(dic);
                            break;
                        }
                    case Commands.ProductList:
                        {
                            Owner.ProccessProductList(dic);
                            break;
                        }
                    case Commands.AdminMsg:
                        {
                            Owner.ShowMessageBox("پیام سرور", dic["msg"].ToString());
                            break;
                        }
                    case Commands.CheckVersion:
                        {
                            var result = (bool)dic["result"];
                            if (result == false)
                            {
                                Owner.DoNewVersion(dic["path"].ToString(), dic["name"].ToString());
                            }
                            break;
                        }
                    case Commands.Pays:
                        {
                            Owner.ProcessPays(dic);
                            break;
                        }
                    case Commands.ChangePassword:
                        {
                            Owner.ProcessChangePassword(dic);
                            break;
                        }
                    case Commands.ResetPassword:
                        {
                            Owner.ProcessResetPassword(dic);
                            break;
                        }
                }


            }
            catch (Exception ex)
            {
                OnExeptionOccured(MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }

        public void SendMessage(Commands msg, object data)
        {
            try
            {
                var result = true;

                if (server == null || server.Connected == false)
                {
                    result = Connect();
                }

                if (result == true)
                {
                    var cmd = new Message()
                    {
                        Command = msg,
                        Data = data,
                        Version = clientVersion,
                        SystemId = systemId
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
                if (server != null && server.Connected)
                {
                    sslStream.Close();
                    server.Close();
                }
            }
            catch (Exception ex)
            {
                OnExeptionOccured(MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
            finally
            {
                server = null;
                sslStream = null;
            }
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private string GetSystemId()
        {
            try
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
            catch (Exception ex)
            {
                OnExeptionOccured(MethodInfo.GetCurrentMethod().Name, ex.Message);
                return String.Empty;
            }
        }


        public virtual void OnConnectStart()
        {
            if (ConnectStart != null)
            {
                ConnectStart(this, EventArgs.Empty);
            }
        }

        public virtual void OnConnectFaild()
        {
            if (ConnectFaild != null)
            {
                ConnectFaild(this, EventArgs.Empty);
            }
        }

        public virtual void OnLossConnection()
        {
            if (LossConnection != null)
            {
                LossConnection(this, EventArgs.Empty);
            }
        }

        public virtual void OnConnectSuccess()
        {
            if (ConnectSuccess != null)
            {
                ConnectSuccess(this, EventArgs.Empty);
            }
        }

        public virtual void OnExeptionOccured(String method, String data)
        {
            if (ExeptionOccured != null)
            {
                ExeptionOccured(method, data);
            }
        }
    }
}
