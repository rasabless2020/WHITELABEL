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
using static WHITELABEL.Web.Helper.InstantPayApi;
using static WHITELABEL.Web.Helper.TransXT_DMR_API;
using NonFactors.Mvc.Grid;
using OfficeOpenXml;
using System.Threading.Tasks;
using System.Data.Entity;
using log4net;
using System.Web.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Web.Script.Serialization;
using System.Device.Location;
using com.mobileware.transxt;

namespace WHITELABEL.Web.Areas.Merchant.Controllers
{
    [Authorize]
    public class Mobileware_DMR_APIController : MerchantBaseController
    {
        // GET: Merchant/MerchantRechargeService
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        public void initpage()
        {
            try
            {
                ViewBag.ControllerName = "Merchant Dashboard";
             
                if (Session["MerchantUserId"] == null)
                {
                    //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
                    Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
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
        // GET: Merchant/Mobileware_DMR_API
        public ActionResult Index()
        {
            try
            {

                //// Create OTP 
                //string dmrPayload = "{\"customerId\":\"6290665805\",\"otpType\":\"1\",\"txnId\":\"\",\"agentCode\":\"1\"}";
                //var OTPCall = TransXT_DMR_API.GenerateOTP("6290665805", "1", "");
                //string OTP = OTPCall.txnId.Value;
                ////Create Customer
                //string OTP1 = OTPCall.response.otp.Value;
                //var CreateCustomerCall = TransXT_DMR_API.CreateCustomer("6290665805", "Rahul Sharma", "Kolkata", "1987-01-10", OTP1);
                //var CustomerInfo = CreateCustomerCall.response.customerId.Value;
                //// Fetch Customer
                //var FetchCustomer = TransXT_DMR_API.FetchCustomer(CustomerInfo);// Working
                //var FetchCustomer11 = TransXT_DMR_API.FetchCustomer("6290665805");
                //////create recipient



                //var CreateRecipient = TransXT_DMR_API.AddRecipient("6290665805", "31923879504", "SBIN0001899", "6290665805", "Rahul Sharma", "2");

                ////Fetch Recipient
                //var fetchRecipient = TransXT_DMR_API.FetchRecipient("3034256");
                //// Fetch Allm recipient
                //` var fetchAllRecipient = TransXT_DMR_API.FetchAllRecipient("6290665805");
                ////Recipient Enquiry
                //var RecipientEnquiry = TransXT_DMR_API.RecipientEnquiry("6290665805", "31923879504", "SBIN0001899", "2", "3034256", "INR","1");

                //// Delete Recipient
                //var DeleteRecipient = TransXT_DMR_API.DeleteRecipient("3034254", "6290665805");
                //// Get Bank Details
                var BankDetails = TransXT_DMR_API.BankDetails("SBIN0001899");
                var AllBankDetails = TransXT_DMR_API.AllBankDetails();


                //var GetOtpval = TransXT_DMR_API.GenerateOTP("6290665805","1", "N15919075084544803");
                string[] saAllowedCharacters = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
                string sRandomOTP = GenerateRandomOTP(12, saAllowedCharacters);
                var Doremit = TransXT_DMR_API.TransactiondoRemit("3034255","6290665805","100.00", sRandomOTP, "INR","1", "8240148377", "Test","9903116214","Rahul", "31923879504", "SBIN0001899");

                return View();
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }

        private static string GenerateRandomOTP(int iOTPLength, string[] saAllowedCharacters)
        {
            string sOTP = String.Empty;
            string sTempChars = String.Empty;
            Random rand = new Random();
            for (int i = 0; i < iOTPLength; i++)
            {
                int p = rand.Next(0, saAllowedCharacters.Length);
                sTempChars = saAllowedCharacters[rand.Next(0, saAllowedCharacters.Length)];
                sOTP += sTempChars;
            }
            return sOTP;
        }
    }
} 