using Parsnet.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Parsnet
{
    public partial class RizehServer : Form
    {
        #region Members
        bool ServerRuning = true;

        private static Regex digitsOnly = new Regex(@"[^\d]");

        public static X509Certificate serverCertificate = null;
        public string certPath = @"c:\Rizeh.cer";

        public Thread RuningServerThread;

        private TcpListener Listener;

        public static BindingList<NetClient> Clients;

        public static RizehServer ServerForm;

        private DateTimePicker datePiker;
        #endregion



        public RizehServer()
        {
            InitializeComponent();

            Clients = new BindingList<NetClient>();
            Clients.ListChanged += new ListChangedEventHandler(this.Clients_ListChanged);
            this.dgvOnlineUsers.AutoGenerateColumns = false;
            this.dgvUsers.AutoGenerateColumns = false;
            this.dgvSitesList.AutoGenerateColumns = false;
            this.dgvDeletedSitesList.AutoGenerateColumns = false;
            this.dgvProductList.AutoGenerateColumns = false;
            this.dgvPayList.AutoGenerateColumns = false;
            this.RuningServerThread = new System.Threading.Thread(new System.Threading.ThreadStart(this.RunServer));
            this.RuningServerThread.IsBackground = true;
            this.RuningServerThread.Start();
            this.dgvOnlineUsers.DataSource = Clients;

            datePiker = new DateTimePicker()
             {
                 Visible = false,
                 CustomFormat = "yyyy/MM/dd",
                 Format = DateTimePickerFormat.Custom,
                 RightToLeft = System.Windows.Forms.RightToLeft.Yes,
                 RightToLeftLayout = true,
                 Anchor = AnchorStyles.Right | AnchorStyles.Top,
                 Font = new System.Drawing.Font("Tahoma", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(178)))
             };

            datePiker.ValueChanged += new EventHandler(datePiker_ValueChanged);
            dgvUsers.Controls.Add(datePiker);
        }

        void datePiker_ValueChanged(object sender, EventArgs e)
        {
            dgvUsers.CurrentCell.Value = datePiker.Value;
            datePiker.Visible = false;
        }


        private void RunServer()
        {
            serverCertificate = X509Certificate.CreateFromCertFile(certPath);

            Listener = new TcpListener(IPAddress.Any, Settings.Default.ServerPort);
            Listener.Start();

            while (ServerRuning == true)
            {
                try
                {
                    var client = new NetClient(Listener.AcceptTcpClient(), this);
                    lock (Clients)
                    {
                        if (Clients.Any(c => c.ClientIP == client.ClientIP) == false)
                        {
                            client.Discunnected += new LossConnectionHandler(client_Discunnected);
                            client.LossConnection += new LossConnectionHandler(client_LossConnection);
                            client.ExeptionOccured += new ExeptionOccuredHandler(client_ExeptionOccured);

                            Clients.Add(client);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteError(MethodInfo.GetCurrentMethod(), ex);
                }
            }

        }

        /*________________________________________________________________________________________________________________________*/

        private void client_ExeptionOccured(string method, string data)
        {
            Log.WriteError(method, data);
        }

        private void client_LossConnection(NetClient client)
        {
            RemoveClient(client);
        }

        private void client_Discunnected(NetClient client)
        {
            RemoveClient(client);
        }

        /*________________________________________________________________________________________________________________________*/

        private void RizehServer_Load(object sender, EventArgs e)
        {

        }

        private void Clients_ListChanged(object sender, ListChangedEventArgs e)
        {
        }


        private void npdScoreStep_ValueChanged(object sender, EventArgs e)
        {
            Settings.Default.ScoreStep = this.npdScoreStep.Value;
            Settings.Default.Save();
        }

        private async void nudMajor_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown down = (NumericUpDown)sender;
            string name = down.Name;
            switch (name)
            {
                case "nudMajor":
                    {
                        Settings.Default.Major = this.nudMajor.Value;
                        break;
                    }
                case "nudMinor":
                    {
                        Settings.Default.Minor = this.nudMinor.Value;
                        break;
                    }
                case "nudBuild":
                    {
                        Settings.Default.Build = this.nudBuild.Value;
                        break;
                    }
                case "nudRevision":
                    {
                        Settings.Default.Revision = this.nudRevision.Value;
                        break;
                    }
            }


            Settings.Default.Save();
            string clientFilePath = Settings.Default.ClientFilePath;

            await Task.Run(() =>
            {
                var list = Clients.Where(c => c.IsAuthenticated == true);
                foreach (var client in list)
                {
                    client.SendCommand(Commands.CheckVersion, new { result = false, path = clientFilePath });
                    client.Disconnect();
                }
            });

        }

        public static void ProccessUserWebsiteList(int id)
        {
            try
            {
                var client = Clients.SingleOrDefault(c => c.UserId == id);
                if (client != null)
                {
                    string userWebsites = DatabaseManager.GetUserWebsites(id);
                    client.SendCommand(Commands.UserWebsiteList, new { dtSites = userWebsites });
                }
            }
            catch (Exception exception)
            {
                Log.WriteError(MethodBase.GetCurrentMethod(), exception);
            }
        }

        private void RemoveClient(NetClient client)
        {
            lock (Clients)
            {
                var item = Clients.SingleOrDefault(c => c.ClientIP == client.ClientIP);
                if (item != null)
                {
                    Clients.Remove(item);
                }
            }

            var totalOnlines = Clients.Where(c => c.IsAuthenticated == true).Count();

            UpdateClientsStatus("onlines", totalOnlines);
        }

        private void RizehServer_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                this.ServerRuning = false;
                foreach (NetClient client in Clients)
                {
                    client.Disconnect();
                }
                Clients = null;
                this.Listener.Stop();
                this.RuningServerThread.Abort();
            }
            catch (Exception exception)
            {
                Log.WriteError(MethodBase.GetCurrentMethod(), exception);
            }
        }

        private async void btnSendMessage_Click(object sender, EventArgs e)
        {
            await Task.Run(() => SendAdminMessage());
            this.txtAdminMsg.Text = "";
        }

        private void dgvOnlineUsers_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            try
            {
                if ((e.ColumnIndex == 0) && (e.RowIndex >= 0))
                {
                    e.PaintBackground(e.CellBounds, true);
                    TextRenderer.DrawText(e.Graphics, e.FormattedValue.ToString(), e.CellStyle.Font, e.CellBounds, e.CellStyle.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
                    e.Handled = true;
                }
            }
            catch (Exception exception)
            {
                Log.WriteError(MethodBase.GetCurrentMethod(), exception);
                e.Handled = false;
            }
        }

        private void dgvOnlineUsers_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void dgvOnlineUsers_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
        }

        private async void dgvPayList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView view = (DataGridView)sender;
            if ((view.Columns[e.ColumnIndex] is DataGridViewButtonColumn) && (e.RowIndex >= 0))
            {
                await Task.Run(() =>
                {
                    PayLine line = new PayLine();
                    string str = view.Rows[e.RowIndex].Cells["TransId"].Value.ToString();
                    string str2 = view.Rows[e.RowIndex].Cells["IdGet"].Value.ToString();

                    view.Rows[e.RowIndex].Cells["Status"].Value = line.Get(str, str2).ToString();
                });
            }
        }

        private async void tabManage_SelectedIndexChanged(object sender, EventArgs e)
        {
            string name = this.tabManage.SelectedTab.Name;
            object source = await Task.Run(() => GetData(name));

            switch (name)
            {
                case "tpOnlineUsers":
                    {
                        this.dgvUsers.DataSource = null;
                        this.dgvSitesList.DataSource = null;
                        this.dgvDeletedSitesList.DataSource = null;
                        this.dgvPayList.DataSource = null;
                        this.dgvProductList.DataSource = null;
                        break;
                    }

                case "tpUsers":
                    {
                        this.dgvUsers.DataSource = source;
                        this.dgvSitesList.DataSource = null;
                        this.dgvDeletedSitesList.DataSource = null;
                        this.dgvPayList.DataSource = null;
                        this.dgvProductList.DataSource = null;
                        break;
                    }
                case "tpWebsites":
                    {
                        this.dgvSitesList.DataSource = source;
                        this.dgvUsers.DataSource = null;
                        this.dgvDeletedSitesList.DataSource = null;
                        this.dgvPayList.DataSource = null;
                        this.dgvProductList.DataSource = null;
                        break;
                    }
                case "tpDeletedSites":
                    {
                        this.dgvDeletedSitesList.DataSource = source;
                        this.dgvUsers.DataSource = null;
                        this.dgvSitesList.DataSource = null;
                        this.dgvPayList.DataSource = null;
                        this.dgvProductList.DataSource = null;
                        break;
                    }
                case "tpPays":
                    {
                        this.dgvPayList.DataSource = source;
                        this.dgvUsers.DataSource = null;
                        this.dgvSitesList.DataSource = null;
                        this.dgvDeletedSitesList.DataSource = null;
                        this.dgvProductList.DataSource = null;
                        break;
                    }
                case "tpProducts":
                    {
                        this.dgvProductList.DataSource = source;
                        this.dgvUsers.DataSource = null;
                        this.dgvSitesList.DataSource = null;
                        this.dgvDeletedSitesList.DataSource = null;
                        this.dgvPayList.DataSource = null;
                        break;
                    }
            }

        }

        private void timCheckConnect_Tick(object sender, EventArgs e)
        {
            try
            {
                lock (Clients)
                {
                    var items = Clients.Where(c => DateTime.Now.Subtract(c.LastRequestTime).Seconds > 30);
                    foreach (var item in items)
                    {
                        Clients.Remove(item);
                    }
                }
            }
            catch (Exception exception)
            {
                Log.WriteError(MethodBase.GetCurrentMethod(), exception);
            }
        }

        private void txtAdminMsg_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == Keys.A))
            {
                if (sender != null)
                {
                    ((TextBox)sender).SelectAll();
                }
                e.Handled = true;
            }
        }

        public static void UpdateClientsStatus(string key, int value)
        {
            foreach (NetClient client in Clients)
            {
                client.UpdateStatus(key, value);
            }
        }

        public void UpdateRequestLog(string item)
        {
        }

        private void dgvUsers_KeyDown(object sender, KeyEventArgs e)
        {

        }


        private Object GetData(string name)
        {
            Object result = null;

            switch (name)
            {
                case "tpUsers":
                    {
                        result = DatabaseManager.GetUsers(0, 100);
                        var list = DatabaseManager.GetUserStateList();
                        StateId.DisplayMember = "Title";
                        StateId.ValueMember = "StateId";
                        StateId.DataSource = list;
                        break;
                    }
                case "tpWebsites":
                    {
                        result = DatabaseManager.GetSitesList(0, 100, false);
                        break;
                    }
                case "tpDeletedSites":
                    {
                        result = DatabaseManager.GetSitesList(0, 100, true);
                        break;
                    }
                case "tpPays":
                    {
                        result = DatabaseManager.GetPaymentsList(0, 100);
                        break;
                    }
                case "tpProducts":
                    {
                        result = DatabaseManager.GetProductsListAdmin();
                        var list = DatabaseManager.GetProductTypeList();
                        ProductType.DisplayMember = "Title";
                        ProductType.ValueMember = "ProductTypeId";
                        ProductType.DataSource = list;
                        break;
                    }
            }

            return result;
        }

        private void SendAdminMessage()
        {
            if (this.cbToAll.Checked)
            {
                var list = RizehServer.Clients.Where(c => c.IsAuthenticated == true);

                foreach (var client in list)
                {
                    try
                    {
                        client.SendCommand(Commands.AdminMsg, new { msg = this.txtAdminMsg.Text });
                    }
                    catch (Exception ex)
                    {
                        Log.WriteError(MethodInfo.GetCurrentMethod(), ex);
                    }
                }
            }
            else if (this.dgvOnlineUsers.SelectedRows.Count > 0)
            {
                string ip = this.dgvOnlineUsers.SelectedRows[0].Cells["IP"].Value.ToString();
                var client = Clients.SingleOrDefault(c => c.ClientIP == ip);
                if (client != null)
                {
                    client.SendCommand(Commands.AdminMsg, new { msg = this.txtAdminMsg.Text });
                }
            }
            else
            {
                MessageBox.Show("یک کاربر انتخاب کنید");
            }
        }

        private void dgvUsers_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            var grid = (DataGridViewEx)sender;
            if (grid.SelectedCells.Count > 0)
            {
                if (grid.SelectedCells[0].ColumnIndex == 5)
                {
                    e.Control.KeyPress -= new KeyPressEventHandler(OnlyNumberControl_KeyPress);
                    e.Control.KeyPress += new KeyPressEventHandler(OnlyNumberControl_KeyPress);
                }
                else
                {
                    e.Control.KeyPress -= new KeyPressEventHandler(OnlyNumberControl_KeyPress);
                }

                if (grid.SelectedCells[0].ColumnIndex == 6)
                {
                    e.Control.KeyPress += new KeyPressEventHandler(DateTimeControl_KeyPress);
                    var rect = grid.GetCellDisplayRectangle(grid.SelectedCells[0].ColumnIndex, grid.SelectedCells[0].RowIndex, false);
                    datePiker.Location = new Point(rect.X + 5, rect.Y + 5);
                    datePiker.Width = rect.Width - 10;
                    try
                    {
                        datePiker.Value = DateTime.Parse(grid.CurrentCell.Value.ToString());
                    }
                    catch (Exception ex)
                    {
                        Log.WriteError(MethodInfo.GetCurrentMethod(), ex);
                    }

                    datePiker.Visible = true;
                }
                else
                {
                    datePiker.Visible = false;
                }
            }
        }

        void DateTimeControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            var IsDeleteKey = (e.KeyChar == (char)Keys.Delete || e.KeyChar == (char)Keys.Back);
            datePiker.Visible = !IsDeleteKey;
            e.Handled = IsDeleteKey;
        }

        void OnlyNumberControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void dgvProductList_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            var grid = (DataGridViewEx)sender;

            if (grid.CurrentCell.ColumnIndex == 3 || grid.CurrentCell.ColumnIndex == 4 || grid.CurrentCell.ColumnIndex == 5)
            {
                e.Control.KeyPress -= new KeyPressEventHandler(OnlyNumberControl_KeyPress);
                e.Control.KeyPress += new KeyPressEventHandler(OnlyNumberControl_KeyPress);
            }
            else
            {
                e.Control.KeyPress -= new KeyPressEventHandler(OnlyNumberControl_KeyPress);
            }
        }


        private void dgvUsers_RowLeave(object sender, DataGridViewCellEventArgs e)
        {
            var row = dgvUsers.Rows[e.RowIndex];

            if (row != null)
            {
                var user = new Users()
                {
                    UserId = (int)row.Cells["UserId"].Value,
                    IsApproved = (bool)row.Cells["IsApproved"].Value,
                    StateId = (int)row.Cells["StateId"].Value,
                    ScoreStep = (int)row.Cells["ScoreSteps"].Value,
                    ConfirmCode = (string)row.Cells["ConfirmCode"].Value,
                    ExpireDate = (String.IsNullOrEmpty(row.Cells["ExpireDate"].Value.ToString()) == false) ? (DateTime?)row.Cells["ExpireDate"].Value : (DateTime?)null,
                };

                if (DatabaseManager.UpdateUser(user) == false)
                {
                    dgvUsers.CancelEdit();
                }
            }
        }



        private void dgvUsers_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            var grid = (DataGridViewEx)sender;
            grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dgvSitesList_RowLeave(object sender, DataGridViewCellEventArgs e)
        {
            var row = dgvSitesList.Rows[e.RowIndex];

            if (row != null)
            {
                var site = new Sites()
                {
                    SiteId = (int)row.Cells["SiteId"].Value,
                    Topic = (row.Cells["Topic"].Value == null) ? String.Empty : row.Cells["Topic"].Value.ToString(),
                    Description = (row.Cells["Description"].Value == null) ? String.Empty : row.Cells["Description"].Value.ToString(),
                    Url = row.Cells["Url"].Value.ToString(),
                    IsBlocked = (bool)row.Cells["IsBlocked"].Value,
                    IsActive = (bool)row.Cells["IsActive"].Value
                };

                if (DatabaseManager.UpdateSiteByAdmin(site) == false)
                {
                    dgvSitesList.CancelEdit();
                }
            }
        }


    }
}
