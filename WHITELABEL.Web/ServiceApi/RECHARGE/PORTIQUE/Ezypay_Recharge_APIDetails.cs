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
    
    public class Ezypay_Recharge_APIDetails
    {
        public static string APIRecharge()
        {         
            return "";
        }
        public static string GetResponseAPI()
        {
            ConfigDetails obj = new ConfigDetails();
            string objval = ConfigDetails.ADMIN_PASSWORD;
            string msgcode = ConfigDetails.DMR_AddSenderCodeE06003;
            string AuthCode = ConfigDetails.Recharge_AUTHORIZATION_CODE_URLAPI;            
            var request = (HttpWebRequest)WebRequest.Create("http://180.179.20.116:8030/RemitMoney/mtransfer?msg=" + msgcode + "~"+ AuthCode + "~RahulSharma~9903116214~Candy~0~0");
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
        public static string GetMobileRechargeResponseAPI(string AuthCode,string mobileno,string product,string Amount,string StoreId)
        {
            //HttpWebRequest requestpao;
            //requestpao = (HttpWebRequest)WebRequest.Create("https://api.myezypay.in/Ezypaywebservice/PushRequest.aspx?AuthorisationCode=9a67aabdebb04dbaad&product=1&MobileNumber=9903116214&Amount=10&RequestId=req123456&StoreID=12458");
            //requestpao.Method = "POST";
            //string myString = string.Empty;
            //try
            //{
            //    WebResponse responseaa = (HttpWebResponse)requestpao.GetResponse();
            //    Stream dataStream = responseaa.GetResponseStream();
            //    byte[] requestBody = ASCIIEncoding.ASCII.GetBytes(HttpUtility.UrlEncode("grant_type=client_credentials"));
            //    requestpao.ContentLength = requestBody.Length;
            //    dataStream.Write(requestBody, 0, requestBody.Length);
            //    StreamReader reader = new StreamReader(dataStream);
            //    myString = reader.ReadToEnd();
            //}
            //catch(Exception ex)
            //{ }                       

            //request1.Method = "POST";
            //request1.ContentType = "application/x-www-form-urlencoded";
            //request1.ContentLength = data.Length;
            //using (var stream = request1.GetRequestStream())
            //{
            //    stream.Write(data, 0, data.Length);
            //}
            //var response1 = (HttpWebResponse)request1.GetResponse();
            //var responseString = new StreamReader(response1.GetResponseStream()).ReadToEnd();


            ConfigDetails obj = new ConfigDetails();
            string objval = ConfigDetails.ADMIN_PASSWORD;
            string msgcode = ConfigDetails.DMR_AddSenderCodeE06003;            
            var request = (HttpWebRequest)WebRequest.Create("https://api.myezypay.in/Ezypaywebservice/PushRequest.aspx?AuthorisationCode=" + AuthCode + "&product=" + product + "&MobileNumber=" + mobileno + "&Amount=" + Amount + "&RequestId=req123456&StoreID=" + StoreId + "");
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

        public static string GetLandlineRechargeResponseAPI(string AuthCode, string mobileno, string product, string Amount, string StoreId,string CircleName,string acountno,string stdcode)
        {               
            var request = (HttpWebRequest)WebRequest.Create("https://api.myezypay.in/Ezypaywebservice/postpaidpush.aspx?AuthorisationCode="+ AuthCode + "&product="+ product + "&MobileNumber="+ mobileno + "&Amount="+ Amount + "&RequestId=req124596325&Circle="+CircleName+"&AcountNo="+acountno+"&StdCode="+stdcode+"");
            request.Method = "GET";            
            //request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";
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