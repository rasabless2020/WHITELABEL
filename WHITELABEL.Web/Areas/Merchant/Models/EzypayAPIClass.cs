namespace WHITELABEL.Web.Areas.Merchant.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Web;
    using System.Web.Http;
    using WHITELABEL.Data;

    public class EzypayAPIClass
    {
        public static dynamic GetRechargeAPI(string AuthCode,string productId,string mobile,decimal amount, string reqid,string Storeid)
        {
            try
            {
                var db = new DBContext();
                var apilink = db.TBL_API_SETTING.Where(x => x.NAME == "API_REQUESTURL").FirstOrDefault();
                //string url = apilink + "/customers/" + customerId + "/funding-sources";
                string url = apilink.VALUE+ "?AuthorisationCode="+ AuthCode + "&product="+productId+"&MobileNumber="+mobile+"&Amount="+amount+ "&RequestId="+reqid+"&StoreID="+Storeid;
                dynamic fundingSourceObj = GetEzypayResponse<string>(url, "GET", null);

                return fundingSourceObj;
            }
            catch (HttpResponseException hre)
            {
                switch ((int)hre.Response.StatusCode)
                {
                    case 403:
                        {
                            return JsonConvert.DeserializeObject("{\"error\" : \"true\", \"errormsg\" : \"Not authorized to list funding sources.\"}");
                        }
                    case 404:
                        {
                            return JsonConvert.DeserializeObject("{\"error\" : \"true\", \"errormsg\" : \"Customer not found.\"}");
                        }
                    default:
                        {
                            throw hre;
                        }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static dynamic GetEzypayResponse<T>(string url, string method, Dictionary<string, T> param)
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
                throw new HttpResponseException(response);
            }
        }
        public static class ClientHelper
        {
            public static HttpClient GetClient()
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.dwolla.v1.hal+json"));
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + EzypayAuthHelper.GetAuthToken());
                return client;
            }


        }
        public static class EzypayAuthHelper
        {
            public static string GetAuthToken()
            {
                var db = new DBContext();
                DateTime dt = new DateTime();
                var apilink = db.TBL_API_SETTING.Where(x => x.NAME == "API_REQUESTURL").FirstOrDefault();
                if ((DateTime.Now - dt).TotalSeconds >= 3600)
                {
                    return GenerateNewAuthToken();
                }
                else
                {
                    return "9a67aabdebb04dbaad";
                }
            }

            private static string GenerateNewAuthToken()
            {
                using (var client = new HttpClient())
                {
                    try
                    {
                        string accessToken = "";
                        var data = new Dictionary<string, string>
                    {
                        {"client_id", ""},
                        {"client_secret", ""},
                        {"grant_type", "client_credentials"}
                    };

                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.dwolla.v1.hal+json"));
                        var db = new DBContext();                        
                        var apilink = db.TBL_API_SETTING.Where(x => x.NAME == "API_REQUESTURL").FirstOrDefault();
                        DateTime currentTimeStamp = DateTime.Now;
                        HttpResponseMessage request = client.PostAsync(apilink.VALUE, new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")).Result;
                        string Response = JsonConvert.SerializeObject(new { request.Content.ReadAsStringAsync().Result, request });
                        dynamic jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(Response);
                        string statusCode = jsonData.request.StatusCode;
                        if (statusCode == "200")
                        {
                            dynamic jsonDataAccessToken = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonData.Result.ToString());
                            accessToken = jsonDataAccessToken.access_token;
                            if (accessToken != null)
                            {
                                //Settings.SetValue("DwollaToken", accessToken);
                                //Settings.SetValue("DwollaTokenTimeStamp", currentTimeStamp.ToString());
                            }
                        }

                        return accessToken;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }
    }
}