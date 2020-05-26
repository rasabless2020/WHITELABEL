namespace WHITELABEL.Web.ServiceApi.RECHARGE.PORTIQUE
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Newtonsoft.Json;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Collections.Specialized;
    using WHITELABEL.Data;
    using WHITELABEL.Web.Helper;
    using WHITELABEL.Web.Configuration;
    using System.Text.RegularExpressions;
    using System.Runtime.Serialization;
    using System.Web.Http;
    using System.Net;
    using System.IO;
    using Newtonsoft.Json.Linq;


    public class DMR_API_EXECUTE
    {
        public string GetResponseAPI()
        {
            var request = (HttpWebRequest)WebRequest.Create("http://180.179.20.116:8030/RemitMoney/mtransfer?msg=E06003~9a67aabdebb04dbaad~RahulSharma~9903116214~Candy~0~0");
            request.Method = "GET";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            var response = (HttpWebResponse)request.GetResponse();
            string content = string.Empty;
            using (var stream = response.GetResponseStream())
            {
                using (var sr = new StreamReader(stream))
                {
                    content = sr.ReadToEnd();
                }
            }
            var releases = (content);
            return releases;
        }
        public static string CreateDMRSender(string mobileno, string CustomerName)
        {

            ConfigDetails obj = new ConfigDetails();
            string objval = ConfigDetails.ADMIN_PASSWORD;
            string msgcode = ConfigDetails.DMR_AddSenderCodeE06003;
            string ReqId = "randomno";
            string AuthCode = ConfigDetails.Recharge_AUTHORIZATION_CODE_URLAPI;
            //var request = (HttpWebRequest)WebRequest.Create("http://180.179.20.116:8030/RemitMoney/mtransfer?E06003~E1hgYt898557843789~ Abc589654123 ~7699999821~Sandy~ NA~ N");
            var request = (HttpWebRequest)WebRequest.Create("http://180.179.20.116:8030/RemitMoney/mtransfer?msg=" + msgcode + "~" + AuthCode + "~" + ReqId + "~" + mobileno + "~" + CustomerName + "~NA~NA");
            request.Method = "GET";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            var response = (HttpWebResponse)request.GetResponse();
            string content = string.Empty;
            using (var stream = response.GetResponseStream())
            {
                using (var sr = new StreamReader(stream))
                {
                    content = sr.ReadToEnd();
                }
            }
            var releases = (content);
            return releases;
        }
        public static string SenderExistsChecking(string mobileno , string CustomerName)
        {

            ConfigDetails obj = new ConfigDetails();
            string objval = ConfigDetails.ADMIN_PASSWORD;
            string msgcode = ConfigDetails.DMR_AddSenderCodeE06003;
            string ReqId = "randomno";
            string AuthCode = ConfigDetails.Recharge_AUTHORIZATION_CODE_URLAPI;
            //var request = (HttpWebRequest)WebRequest.Create("http://180.179.20.116:8030/RemitMoney/mtransfer?E06003~E1hgYt898557843789~ Abc589654123 ~7699999821~Sandy~ NA~ N");
            var request = (HttpWebRequest)WebRequest.Create("http://180.179.20.116:8030/RemitMoney/mtransfer?msg=" + msgcode + "~" + AuthCode + "~"+ ReqId + "~"+ mobileno + "~Value1~Value2");
            request.Method = "GET";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            var response = (HttpWebResponse)request.GetResponse();
            string content = string.Empty;
            using (var stream = response.GetResponseStream())
            {
                using (var sr = new StreamReader(stream))
                {
                    content = sr.ReadToEnd();
                }
            }
            var releases = (content);
            return releases;
        }
    }
    
}
    
