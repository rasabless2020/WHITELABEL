using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Models;
using WHITELABEL.Web.Helper;
using System.Data.Entity.Core;
using WHITELABEL.Web.Areas.Merchant.Models;
using WHITELABEL.Web.ServiceApi.RECHARGE.PORTIQUE;
using System.Threading.Tasks;
using static WHITELABEL.Web.Helper.InstantPayApi;
using NonFactors.Mvc.Grid;
using log4net;
using com.mobileware.transxt;
using Newtonsoft.Json;

namespace WHITELABEL.Web.Areas.Merchant.Controllers
{
    public class MerchantDMR_DetailsController : MerchantBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        public void initpage()
        {
            try
            {
                ViewBag.ControllerName = "Merchant Dashboard";
                SystemClass sclass = new SystemClass();
                string userID = sclass.GetLoggedUser();
                long userid = long.Parse(userID);
                var dbmain = new DBContext();
                if (userID != null && userID != "")
                {
                    TBL_MASTER_MEMBER currUser = dbmain.TBL_MASTER_MEMBER.SingleOrDefault(c => c.MEM_ID == userid && c.MEMBER_ROLE == 5 && c.ACTIVE_MEMBER == true);
                    if (currUser != null)
                    {
                        Session["MerchantUserId"] = currUser.MEM_ID;
                        // Session["UserName"] = currUser.UserName;
                    }
                    else
                    {
                        Response.Redirect(Url.Action("Index", "MerchantLogin", new { area = "Merchant" }));
                        return;
                    }
                }
                if (Session["MerchantUserId"] == null)
                {
                    //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
                    Response.Redirect(Url.Action("Index", "MerchantLogin", new { area = "Merchant" }));
                    return;
                }
                bool Islogin = false;
                if (Session["MerchantUserId"] != null)
                {
                    Islogin = true;
                    ViewBag.CurrentUserId = CurrentMerchant.MEM_ID;
                }
                ViewBag.Islogin = Islogin;
            }
            catch (Exception e)
            {
                //ViewBag.UserName = CurrentUser.UserId;
                Console.WriteLine(e.InnerException);
                return;
            }
        }

        // GET: Merchant/MerchantDMR_Details
        public ActionResult Index()
        {
            if (Session["MerchantDMRId"] == null)
            {
                //    string value = System.Configuration.ConfigurationManager.AppSettings["checkSumUrl"];
                //    //string OTPdmrUrl = "https://uat.transxtnow.com:8443/api/1.1/dmr/otp";
                //    string OTPdmrUrl = "https://api.transxtnow.com/api/1.1/dmr/otp";
                //    var GeneratingOtpParam = new Dictionary<string, dynamic> {

                //        {
                //            "payload", new Dictionary<string, string> {
                //                { "customerId", "6290665805"},
                //                { "otpType", "1"},
                //                { "txnId", ""},
                //                { "agentCode", "1"}
                //            }
                //        },
                //        {
                //            "checksum", ""
                //        }
                //    };

                //    string OTPvalueparameter = JsonConvert.SerializeObject(GeneratingOtpParam);               
                //    //string OTPdmrUrl = "https://uat.transxtnow.com:8443/transxt/dev/api/1.1/dmr/otp";
                //    string jsonobj = "{\"checksum\":\"\",\"payload\":{\"customerId\":\"6290665805\",\"otpType\":\"1\",\"txnId\":\"\",\"agentCode\":\"1\"}}";
                //    //https://uat.transxtnow.com:8443/transxt/dev
                //    string OTPresponse = TransXTCommunicator.execute(OTPvalueparameter, OTPdmrUrl);
                //    string OTPresponse1 = TransXTCommunicator.execute(jsonobj, OTPdmrUrl);

                //    //String dmrPayload = "{\"customerId\":\"9999999999\",\"agentCode\":\"1\"}";

                //    var param = new Dictionary<string, dynamic> {
                //        {
                //            "checksum", "957A9BDDFD839A2FD81E992BBEEAB1671A3B1A8EE5819751649BA41B559C413A"
                //        },
                //        {
                //            "payload", new Dictionary<string, string> {
                //                { "customerId", "9903116214"},
                //                { "name", "Rahul Sharma"},
                //                { "address", "Kolkata"},
                //                { "dateOfBirth", "2000-15-01"},
                //                { "otp","12356"},
                //                {"agentCode","1" }
                //            }

                //        }
                //    };
                //    string valueparameter = JsonConvert.SerializeObject(param);
                //    string dmrUrl= "https://uat.transxtnow.com:8443/api/1.1/dmr/createcustomer";
                //    string response = TransXTCommunicator.execute(valueparameter, dmrUrl);
                //    //var dmrCustomer = DMR_MobileWare.Mobileware_DMRApis.DMRAPI_Customer_Add("957A9BDDFD839A2FD81E992BBEEAB1671A3B1A8EE5819751649BA41B559C413A", "9903116214", "Rahul Sharma", "Kolkata", "1987-12-12", "12455");

                //    var param1 = new Dictionary<string, dynamic> {
                //        {
                //            "checksum", "957A9BDDFD839A2FD81E992BBEEAB1671A3B1A8EE5819751649BA41B559C413A"
                //        },
                //        {
                //            "payload", new Dictionary<string, string> {
                //                { "customerId", "9903116214"},
                //                { "udf1", "546465"},
                //                { "udf2", "546465"},
                //                { "recipientType","2"},
                //                {"clientRefId","1235485" },
                //                {"currency","INR" },
                //                {"channel","1" },
                //                {"agentCode","1" }
                //            }

                //        }
                //    };
                //    string valueparameter1 = JsonConvert.SerializeObject(param1);
                //    string dmrUrl1 = "https://uat.transxtnow.com:8443/api/1.1/dmr/recipientenquiry";
                //    string response1 = TransXTCommunicator.execute(valueparameter1, dmrUrl1);


                string CHECKSUM_URL = System.Configuration.ConfigurationManager.AppSettings["checkSumUrl"];
                string AUTH_URL = System.Configuration.ConfigurationManager.AppSettings["authUrl"];
                string AUTH_TOKEN = System.Configuration.ConfigurationManager.AppSettings["authToken"];
                string USER_ID = System.Configuration.ConfigurationManager.AppSettings["username"];
                string PASSWORD = System.Configuration.ConfigurationManager.AppSettings["password"];

                string CREATE_CUST_URL = "https://uat.transxtnow.com:8443/api/1.1/dmr/createcustomer";
                string PAYLOAD_URL_CREATE_CUST = "{\"customerId\":\"9685472536\",\"name\":\"Elly\",\"address\":\"Delhi\",\"dateOfBirth\":\"1997-09-26\",\"otp\":\"189322\",\"agentCode\":\"1\"}";
                //string OTPvalueparameter = JsonConvert.SerializeObject(PAYLOAD_URL_CREATE_CUST);
                Dictionary<string, string> SECRET = new Dictionary<string, string>();
                SECRET.Add("username", USER_ID);
                SECRET.Add("password", PASSWORD);

                //string RESPONSE = Encryption.generateChecksum(PAYLOAD_URL_CREATE_CUST, SECRET.ToString());
                String dmrPayload = "{\"customerId\":\"9999999999\",\"agentCode\":\"1\"}";
                string Url = "@"+ "https://uat.transxtnow.com:8443/transxt/dev/api/1.1/dmr/fetchcustomer";

                string response = TransXTCommunicator.execute(dmrPayload, Url);


                var GeneratingOtpParam11 = new Dictionary<string, dynamic>
                {
                                { "customerId", "6290665805"},
                                { "otpType", "1"},
                                { "txnId", ""},
                                { "agentCode", "1"}
                    };
                string OTPvalueparameter11 = JsonConvert.SerializeObject(GeneratingOtpParam11);

                string ObjParam= OTPvalueparameter11.Replace(("\\"), String.Empty);
                string ObjParam1 = "{'customerId':'6290665805','otpType':'1','txnId':'','agentCode':'1'}";
                var perso = JsonConvert.DeserializeObject<dynamic>(OTPvalueparameter11);
                string OTPdmrUrl11 = "https://uat.transxtnow.com:8443/api/1.1/dmr/otp";
                string jsonobj = "{\"checksum\":\"\",\"payload\":{\"customerId\":\"6290665805\",\"otpType\":\"1\",\"txnId\":\"\",\"agentCode\":\"1\"}}";
                //https://uat.transxtnow.com:8443/transxt/dev
                string OTPresponse33 = TransXTCommunicator.execute(OTPvalueparameter11, OTPdmrUrl11);




                return View();
            }
            else
            {
                Session["MerchantDMRId"] = null;
                Session.Remove("MerchantDMRId");
                Response.Redirect(Url.Action("Index", "MerchantDMRLogin", new { area = "Merchant" }));
                return View();
            }
        }
    }
}