using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace WHITELABEL.Web.Helper
{
    public static class MultilinkRechargeAPI
    {
        public static string root = "http://www.multilinkrecharge.com/ReCharge/JsonRechargeApi.asmx/";
        //public static string root = "http://www.multilinkrecharge.com/ReCharge/JsonRechargeApi.asmx?op=";
        public static string checkUrl = "http://free.mrechargesystem.com/ReCharge/JsonRechargeApi.asmx/StatusCheckByRequestId?";
        //http://free.mrechargesystem.com/ReCharge/JsonRechargeApi.asmx/StatusCheckByRequestId?
        public static string Pinno = "2019";
        public static string MobileNo = "9830864888";
        public static string TransactionID = "MOSRECH38319";
        public static class MultiLinkRechargeAPI
        {
            public static dynamic PaymentAPI(string RechargeType, string spkey, string amount,string RechargeNo,string CircleCode,string Uniqueid)
            {
                try
                {
                    string transactionstatus = string.Empty;
                    
                    string url = root + "Recharge?MobileNo=" + MobileNo + "&PinNumber=" + Pinno + "&RechargeType=" + RechargeType + "&ServiceName=" + spkey + "&Amount=" + amount + "&RechargeNumber=" + RechargeNo + "&TransId=" + Uniqueid + "&circle=" + CircleCode + "";
                    //string url = root + "Recharge?MobileNo="+Mobileno+"&PinNumber="+Pinno+"&RechargeType="+RechargeType +"&ServiceName="+spkey +"&Amount="+ amount + "&RechargeNumber="+RechargeNo+"&TransId="+TransactionID+"&circle="+CircleCode+"";
                    var res = GetResponse(url, "GET", new Dictionary<string, string>());
                    string errorcode = res.Status;
                    string des = res.Message;
                    if (errorcode == "0" ||errorcode == "2")
                    {
                        return res;
                    }
                    else
                    {
                        return res;
                    }
                    
                    //if(res.)
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            public static string CheckPaymentStatusAPI(string MobileNo,string PinNumber,string Command,string RequestId)  
            {
                try
                {
                    string transactionstatus = string.Empty;
                    string url = root + "MobileNo="+MobileNo+"&PinNumber="+PinNumber+"&Command="+Command+"&RequestId="+RequestId+"";
                    var res = GetResponse(url, "GET", new Dictionary<string, string>());
                    string errorcode = res.Status;
                    string des = res.Message;
                    if (errorcode == "2")
                    {
                        transactionstatus = errorcode;
                    }
                    else
                    {
                        transactionstatus = InstantPayError.GetError(errorcode);
                    }
                    return transactionstatus;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            public static dynamic MultiLinkTransactionStatusCheck(string Command, string RequestId)
            {
                try
                {
                    string transactionstatus = string.Empty;
                    string url = root + "StatusCheckByRequestId?MobileNo=" + MobileNo + "&PinNumber=" + Pinno + "&Command=" + Command + "&RequestId=" + RequestId + "";
                    //string url = root + "Recharge?MobileNo="+Mobileno+"&PinNumber="+Pinno+"&RechargeType="+RechargeType +"&ServiceName="+spkey +"&Amount="+ amount + "&RechargeNumber="+RechargeNo+"&TransId="+TransactionID+"&circle="+CircleCode+"";
                    var res = GetResponse(url, "GET", new Dictionary<string, string>());
                    string errorcode = res.Status;
                    string des = res.Message;
                    if (errorcode == "2")
                    {
                        return res;
                    }
                    else
                    {
                        return res;
                    }

                    //if(res.)
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }
        private static dynamic GetResponse<T>(string url, string method, Dictionary<string, T> param)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                using (HttpClient client = ClientHelper.GetClient())
                {
                    switch (method.ToUpper())
                    {
                        case "GET":
                            {
                                response = client.GetAsync(url).Result;
                                break;
                            }
                        case "POST":
                            {
                                response = client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json")).Result;
                                break;
                            }
                        default:
                            {
                                throw new NotImplementedException();
                            }
                    }
                    response.EnsureSuccessStatusCode();
                    return string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result) ? response : JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result);
                }
            }
            catch (Exception e)
            {
                // Handle exception
                throw e;
            }
        }
        public static class ClientHelper
        {
            public static HttpClient GetClient()
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                return client;
            }
            public static HttpClient GetDocClient()
            {
                HttpClient Docclient = new HttpClient();
                Docclient.DefaultRequestHeaders.Accept.Clear();
                Docclient.DefaultRequestHeaders.Add("Accept", "application/json");
                Docclient.DefaultRequestHeaders.Add("Content-Type", "application/json");
                Docclient.DefaultRequestHeaders.Add("ContentType", "multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW");
                return Docclient;
            }
        }
    }
}