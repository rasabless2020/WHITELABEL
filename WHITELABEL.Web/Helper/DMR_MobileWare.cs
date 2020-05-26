namespace WHITELABEL.Web.Helper
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;


    public static class DMR_MobileWare
    {
        public static class Mobileware_DMRApis
        {
            public static string root = "https://api.transxtnow.com/";
            public static string Authorization = "eyJhbGciOiJIUzUxMiJ9.eyJqdGkiOiIxMDgiLCJzdWIiOiJhdXRoIiwiaXNzIjoiVFJBTlNYVCJ9.OpZ9MHoX7f33pME7fk5JepGlb9Vxgxdf851oiKSwNicv_E90xNOPN8f4SvXLYZ6LDpFr5b0qygHj1VUa0UDOAQ";
            public static string agentCode = "1";
            public static dynamic DMRAPI_Customer_Add(string checksum, string customerId, string name, string address, string dateOfBirth, string otp)
            {
                try
                {
                    string url = root + "api/1.1/dmr/createcustomer";
                    var param = new Dictionary<string, dynamic> {
                    {
                        "checksum", checksum
                    },
                    {
                        "payload", new Dictionary<string, string> {
                            { "customerId", customerId},
                            { "name", name},
                            { "address", address},
                            { "dateOfBirth", dateOfBirth},
                            { "otp",otp},
                            {"agentCode",agentCode }
                        }

                    }
                };
                    string valueparameter = JsonConvert.SerializeObject(param);
                    var res = GetResponse(valueparameter, url, Authorization);

                    if (res.statuscode.Value == "TXN")
                    {
                        return res;
                    }
                    else
                    {
                        //return InstantPayError.GetError(res.statuscode.Value);
                        return res;
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }

            }

            public static dynamic GetResponse(string requestData, string url, string Authorization)
            {
                string responseXML = string.Empty;
                dynamic responsesult = null;
                try
                {
                    byte[] data = Encoding.UTF8.GetBytes(requestData);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    //request.Headers.Add("Accept-Encoding", "gzip");
                    request.Headers["Authorization"] = "bearer " + Authorization;
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(data, 0, data.Length);
                    dataStream.Close();
                    WebResponse webResponse = request.GetResponse();
                    var rsp = webResponse.GetResponseStream();
                    if (rsp == null)
                    {
                        //throw exception
                    }
                    using (StreamReader readStream = new StreamReader(new GZipStream(rsp, CompressionMode.Decompress)))
                    {
                        responsesult = JsonConvert.DeserializeObject(readStream.ReadToEnd());
                        //responseXML = JsonConvert.DeserializeXmlNode(readStream.ReadToEnd()).InnerXml;
                    }
                    return responsesult;
                }
                catch (WebException webEx)
                {
                    //get the response stream
                    WebResponse response = webEx.Response;
                    Stream stream = response.GetResponseStream();
                    String responseMessage = new StreamReader(stream).ReadToEnd();
                    return responseMessage;
                }
            }

        }




    }
    


}