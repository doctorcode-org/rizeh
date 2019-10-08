namespace Parsnet
{
    using System;
    using System.Net;
    using System.Net.Mail;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;

    public static class EmailManager
    {
        private delegate void SendEmailDelegate(MailMessage m);

        public static void Send(string reciver, string subject, string body)
        {
            Send("noreply@rizeh.com", reciver, subject, body, "mail.rizeh.com", 0x19, false, "admin@rizeh.com", "13642312", true);
        }

        public static void Send(string sender, string reciver, string subject, string body, string host, int port, bool useSsl, string serverUsername, string serverPassword, bool Async)
        {
            try
            {
                MailMessage m = new MailMessage()
                {
                    Subject = subject,
                    SubjectEncoding = Encoding.UTF8,
                    BodyEncoding = Encoding.UTF8,
                    IsBodyHtml = true,
                    Body = body,
                    From = new MailAddress(sender)
                };

                m.To.Clear();
                m.To.Add(reciver);

                SmtpClient client = new SmtpClient
                {
                    Host = host,
                    Port = port,
                    EnableSsl = useSsl,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(serverUsername, serverPassword)
                };

                if (Async)
                {
                    var delg = new SendEmailDelegate(client.Send);
                    delg.BeginInvoke(m, new AsyncCallback(EmailManager.SendEmailCallback), delg);
                }
                else
                {
                    client.Send(m);
                }
            }
            catch (Exception exception)
            {
                Log.WriteError(MethodBase.GetCurrentMethod(), exception);
            }
        }

        private static void SendEmailCallback(IAsyncResult ar)
        {
            try
            {
                ((SendEmailDelegate)ar.AsyncState).EndInvoke(ar);
            }
            catch (Exception exception)
            {
                Log.WriteError(MethodBase.GetCurrentMethod(), exception);
            }
        }

       
    }
}

