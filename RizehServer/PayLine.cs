namespace Parsnet
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;

    public class PayLine
    {
        //private string api = "c1d1b-e459d-a7a8f-8aee6-b9765ae79ef482dbe7eb19b2022f";
        private string api = "f5fc5-fc9b8-33eec-a8d1c-659aea9ac6a623b4275af3def47a";
        private string redirect = "http://www.rizeh.com/payresult.aspx";
        private string second_url = "http://payline.ir/payment/gateway-result-second";
        private string url = "http://payline.ir/payment/gateway-send";

        public int Get(string trans_id, string id_get)
        {
            int result = -1;
            try
            {
                WebRequest request = WebRequest.Create(this.second_url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                string s = "api=" + this.api + "&trans_id=" + trans_id + "&id_get=" + id_get;
                byte[] bytes = Encoding.UTF8.GetBytes(s);
                request.ContentLength = bytes.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
                StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream());
                string str3 = reader.ReadToEnd();
                reader.Close();
                if (!int.TryParse(str3, out result))
                {
                    result = -1;
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        public int Send(double amount)
        {
            int result = -1;
            try
            {
                WebRequest request = WebRequest.Create(this.url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                string s = string.Concat(new object[] { "api=", this.api, "&amount=", amount, "&redirect=", this.redirect });
                byte[] bytes = Encoding.UTF8.GetBytes(s);
                request.ContentLength = bytes.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
                StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream());
                string str2 = reader.ReadToEnd();
                reader.Close();
                if (!int.TryParse(str2, out result))
                {
                    result = -1;
                }
            }
            catch (Exception)
            {
            }
            return result;
        }
    }
}

