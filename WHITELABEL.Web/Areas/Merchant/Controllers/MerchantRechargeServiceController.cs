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
using WHITELABEL.Web.Controllers;
using System.Web.UI;
using System.Text.RegularExpressions;
using static WHITELABEL.Web.Helper.CyberPlateAPIHelper;
using System.Text;
using CyberPlatOpenSSL;

namespace WHITELABEL.Web.Areas.Merchant.Controllers
{
    [Authorize]
    public class MerchantRechargeServiceController : MerchantBaseController
    {
        // GET: Merchant/MerchantRechargeService
        OpenSSL ssl = new OpenSSL(); StringBuilder str = new StringBuilder();
        public static string SessionNo = DateTime.Parse(DateTime.Now.ToString()).ToString("dddMMMddyyyyHHmmss");
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        public void initpage()
        {
            try
            {
                ViewBag.ControllerName = "Merchant";
                
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

        public ActionResult Index()
        {
            if (Session["MerchantUserId"] != null)
            {
                var db = new DBContext();
                initpage();
                var checkList = (from user in db.TBL_WHITELABLE_SERVICE
                                 join serv in db.TBL_SETTINGS_SERVICES_MASTER on user.SERVICE_ID equals serv.SLN
                                 where user.MEMBER_ID == CurrentMerchant.MEM_ID
                                 select new ServiceList
                                 {
                                     ServiceName = serv.SERVICE_NAME,
                                     ServiceStatus = user.ACTIVE_SERVICE
                                 }).ToList();
                //ViewBag.ActiveServiceList = checkList;
                var checkoutlet = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(c => c.OUTLETID).FirstOrDefault();
                if (checkoutlet != null)
                {
                    ViewBag.Outletcheck = checkoutlet;
                }
                else
                {
                    ViewBag.Outletcheck = "";
                }
              
                var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "PREPAID" || x.TYPE == "POSTPAID" && x.STATUS==0 && x.OPTION4== "CYBERPLAT").OrderBy(c => c.TYPE).ToList();
                ViewBag.operatorList = OperatorList;
                var ElectricityOperator = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "ELECTRICITY" && x.STATUS == 0 && x.OPTION4 == "CYBERPLAT").OrderBy(c => c.TYPE).ToList();
                ViewBag.ElectricityOperator = ElectricityOperator;
                var WaterOperator = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "WATER" && x.STATUS == 0 && x.OPTION4 == "CYBERPLAT").OrderBy(c => c.TYPE).ToList();
                ViewBag.WaterOperator = WaterOperator;
                var DTHOperator = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "DTH" && x.STATUS == 0 && x.OPTION4 == "CYBERPLAT").OrderBy(c => c.TYPE).ToList();
                ViewBag.DTHOperator = DTHOperator;
                var LANDLINEOperator = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "LANDLINE" && x.STATUS == 0 && x.OPTION4 == "CYBERPLAT").OrderBy(c => c.TYPE).ToList();
                ViewBag.LANDLINEOperator = LANDLINEOperator;
                var BROADBANDOperator = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "BROADBAND" && x.STATUS == 0 && x.OPTION4 == "CYBERPLAT").OrderBy(c => c.TYPE).ToList();
                ViewBag.BROADBANDOperator = BROADBANDOperator;
                var GasOperator = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "GAS" && x.STATUS == 0 && x.OPTION4 == "CYBERPLAT").OrderBy(c => c.TYPE).ToList();
                ViewBag.GasOperator = GasOperator;
                Session["MerchantDMRId"] = null;
                Session.Remove("MerchantDMRId");
                return View(checkList);
            }
            else
            {
                FormsAuthentication.SignOut();
                Session["MerchantUserId"] = null;
                Session["MerchantUserName"] = null;
                Session.Clear();
                Session.Remove("MerchantUserId");
                Session.Remove("MerchantUserName");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }

        public ActionResult BindAllMobileOperator()
        {
            initpage();
               var db = new DBContext();
            var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "PREPAID" || x.TYPE == "POSTPAID" && x.STATUS == 0 && x.OPTION4 == "CYBERPLAT").OrderBy(c => c.TYPE).ToList();
            ViewBag.operatorList = OperatorList;
            return PartialView();
        }

        [HttpPost]
        public JsonResult OpenAllProviderList(string radioValue)
        {
            initpage();
            try
            {
                var db = new DBContext();
                var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == radioValue).OrderBy(c => c.TYPE).ToList();
                //var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "PREPAID" || x.TYPE == "POSTPAID").OrderBy(c => c.TYPE).ToList();
                ViewBag.operatorList = OperatorList;
                return Json(new { Result = "true" });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]
        public ActionResult OperatorsDetails(string customerId)
        {
            initpage();
            var db = new DBContext();
            var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == customerId).OrderBy(c => c.TYPE).FirstOrDefault();
            //var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "PREPAID" || x.TYPE == "POSTPAID").OrderBy(c => c.TYPE).ToList();
            ViewBag.operatorList = OperatorList;
            return PartialView("_OperatorsDetails", OperatorList);
            //return PartialView("~/Areas/Merchant/Views/MerchantRechargeService/OperatorsDetails.cshtml", OperatorList);
            //return PartialView("~/Merchant/MerchantRechargeService/OperatorsDetails.cshtml", OperatorList);
        }
        //public ActionResult CheckOperator(string EmployeeId)
        //public ActionResult CheckOperator()
        public ActionResult CheckOperator(string OperatorType)
        {
            initpage();
            try
            {
                var db = new DBContext();
                if (OperatorType == "POSTPAID")
                {
                    var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "POSTPAID" && x.OPTION4 == "CYBERPLAT").OrderBy(c => c.SERVICE_NAME).ToList();
                    //var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "PREPAID" || x.TYPE == "POSTPAID").OrderBy(c => c.TYPE).ToList();
                    ViewBag.operatorList = OperatorList;
                }
                else
                {
                    //var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "MobileOperator").OrderBy(c => c.SERVICE_NAME).ToList();
                    //var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "PREPAID" || x.TYPE == "POSTPAID").OrderBy(c => c.TYPE).ToList();
                    var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "PREPAID" && x.OPTION4 == "CYBERPLAT").OrderBy(c => c.TYPE).ToList();
                    ViewBag.operatorList = OperatorList;
                }
                
                return PartialView("CheckOperator");
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        // Mobile recharge section

        

        public ActionResult BrowseJioPlan(string ServiceName,string MobileNo)
        {
            initpage();
            try
            {
                string Outputres = string.Empty;
                var db = new DBContext();
                //db.Database.ExecuteSqlCommand("TRUNCATE TABLE JIO_PLAN_LIST");
                var getPlanList = db.TBL_JIO_PLAN_LIST.ToList();
                if (getPlanList.Count > 0)
                {
                    ViewBag.joiPlanList = getPlanList;
                }
                else
                {
                    var GetServiceURL = db.TBL_SERVICE_PROVIDERS.Where(x => x.SERVICE_NAME == ServiceName && x.OPTION4 == "CYBERPLAT").FirstOrDefault();
                    string keyPath = Server.MapPath("~/Test CERT/myprivatekey.pfx");
                    string JoiPlan = CyberPlatRelianceJIORecharge._strValidationRelianceJIOFetchPlanRequest(MobileNo, "10");
                    ssl.message = ssl.Sign_With_PFX(JoiPlan, keyPath, "rahul123");
                    ssl.htmlText = ssl.CallCryptoAPI(ssl.message, GetServiceURL.OPTION1);
                    Outputres = "URL:\r\n" + GetServiceURL.OPTION1 + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;
                    string myString = ssl.htmlText;
                    string[] GetPlan = ssl.GetResponseInArray(myString);
                    string returnUrl = Server.UrlDecode(GetPlan[6]);
                    var Objsettng = returnUrl.Remove(0, 8);
                    JObject json = JObject.Parse(Objsettng);
                    var ResponseResult = JObject.Parse(Objsettng);
                    var errorCode = ResponseResult["planOffering"].ToString();
                    string errorCodeMgs = errorCode.ToString();
                    foreach (var Banklistitem in ResponseResult["planOffering"])
                    {
                        string billingType = Banklistitem["billingType"].ToString();
                        string Name = Banklistitem["name"].ToString();
                        string Cercile = Banklistitem["circle"].ToString();
                        string id_Val = Banklistitem["id"].ToString();
                        string price = Banklistitem["price"].ToString();
                        string description = Banklistitem["description"].ToString();
                        string jio_user = Banklistitem["jio_user"].ToString();
                        TBL_JIO_PLAN_LIST objjio = new TBL_JIO_PLAN_LIST
                        {
                            billingType = billingType,
                            circle = Cercile,
                            name = Name,
                            id_Val = id_Val,
                            price = price,
                            description = description,
                            entrydate = DateTime.Now,
                            jio_user = jio_user
                        };
                        db.TBL_JIO_PLAN_LIST.Add(objjio);
                        db.SaveChanges();
                        
                    }
                    var Getplan = db.TBL_JIO_PLAN_LIST.ToList();
                    ViewBag.joiPlanList = getPlanList;
                }
               

                


                return PartialView("BrowseJioPlan");
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        #region Mobile Recharge


        public ActionResult MobileRecharge()
        {
            if (Session["MerchantUserId"] != null)
            {
                initpage();
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login", new { area = "" });
            }

        } 
        public ActionResult GetOperatorName(string query)
        {
            return Json(_GetOperator(query), JsonRequestBehavior.AllowGet);
        }
        private List<Autocomplete> _GetOperator(string query)
        {
            List<Autocomplete> people = new List<Autocomplete>();
            var db = new DBContext();
            try
            {
                var results = (from p in db.TBL_OPERATOR_MASTER
                               where (p.OPERATORNAME).Contains(query)
                               orderby p.OPERATORNAME
                               select p).ToList();
                foreach (var r in results)
                {
                    // create objects
                    Autocomplete Username = new Autocomplete();
                    //Username.FromUser = string.Format("{0} {1}", r.UName);
                    Username.Name = (r.OPERATORNAME);
                    Username.Id = r.PRODUCTID;

                    people.Add(Username);
                }

            }
            catch (EntityCommandExecutionException eceex)
            {
                if (eceex.InnerException != null)
                {
                    throw eceex.InnerException;
                }
                throw;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return people;
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

        public static string GetUniqueKey(string userID)
        {
            string resultString = Regex.Match(userID, @"\d+").Value;

            long ticks = DateTime.Now.Ticks;
            string result = resultString + ticks.ToString();
            return result.ToString();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> PostMobileRecharge(MobileRechargeModel objval)
        {
            //initpage();
            try
            {
                string keyPath = Server.MapPath("~/Test CERT/myprivatekey.pfx");
                string OutputResponse = string.Empty;
                var db = new DBContext();
                var VendorInfo = db.TBL_VENDOR_MASTER.FirstOrDefault(x => x.ID == 1);
                string IpAddress = string.Empty;
                if (objval.IpAddress != null)
                {
                    if (objval.IpAddress != "")
                    {
                        IpAddress = objval.IpAddress.Replace("\"", "");
                    }
                    else
                    {
                        IpAddress = "";
                    }
                }
                else
                {
                    IpAddress = "";
                }
                string AccountId_Val = System.Configuration.ConfigurationManager.AppSettings["MOSACCONTID"];
                var getWhiteLevelID = await (from mrnt in db.TBL_MASTER_MEMBER
                                             join dist in db.TBL_MASTER_MEMBER on mrnt.INTRODUCER equals dist.MEM_ID
                                             //join super in db.TBL_MASTER_MEMBER on dist.INTRODUCER equals super.MEM_ID
                                             join WBL in db.TBL_MASTER_MEMBER on dist.UNDER_WHITE_LEVEL equals WBL.MEM_ID
                                             where mrnt.MEM_ID == CurrentMerchant.MEM_ID
                                             select new
                                             {
                                                 WHTBalance = WBL.BALANCE
                                             }).FirstOrDefaultAsync();
                if (getWhiteLevelID.WHTBalance >= objval.RechargeAmt)
                {
                    var check_walletAmt = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).FirstOrDefaultAsync();
                    if (objval.RechargeAmt <= check_walletAmt.BALANCE)
                    {
                        string OperatorName = Request.Form["OperatorName"];
                        string operatorId = Request.Form["OperatorId"];
                        string CircleCode = Request.Form["CircleCodeId"];
                        string CircleName_Value = Request.Form["CircleName"];
                        long merchantid = Convert.ToInt64(Session["MerchantUserId"].ToString());
                        string Scheme = objval.PrepaidRecharge;
                        string schemeCode = string.Empty;
                        string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
                        if (Scheme == "PREPAID")
                        {
                            #region CyberPlat
                            schemeCode = "RR";
                            string transType = string.Empty;
                            string JioPlanId = string.Empty;
                            if (objval.JIO_Plan_Id != null)
                            {
                                if (objval.JIO_Plan_Id != "")
                                {
                                    JioPlanId = objval.JIO_Plan_Id;
                                }
                                else
                                { JioPlanId = ""; }
                            }
                            else
                            { JioPlanId = ""; }
                            string SessionNo = DateTime.Parse(DateTime.Now.ToString()).ToString("dddMMMddyyyyHHmmss");
                            var GetServiceURL = db.TBL_SERVICE_PROVIDERS.Where(x => x.SERVICE_NAME == objval.OperatorName && x.OPTION4 == "CYBERPLAT").FirstOrDefault();
                            var PaymentValidation = CyberPlateAPI._strValidationRequestLogs(SessionNo, objval.ContactNo, objval.RechargeAmt.ToString(), objval.OperatorName, JioPlanId);
                            //var PaymentValidation = CyberPlateAPI._strValidationRequestLogs(objval.ContactNo, objval.RechargeAmt.ToString(), objval.OperatorName, JioPlanId);
                            ssl.message = ssl.Sign_With_PFX(PaymentValidation, keyPath, "rahul123");
                            ssl.htmlText = ssl.CallCryptoAPI(ssl.message, GetServiceURL.OPTION1);
                            OutputResponse = "URL:\r\n" + GetServiceURL.OPTION1 + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;
                            string Out_response = ssl.htmlText;
                            string[] response_Value = ParseReportCode(Out_response);
                            string ReturnOutpot = response_Value[5];
                            string ErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(ReturnOutpot);
                            //var PaymentValidation = PaymentAPI.Validation(agentId, objval.RechargeAmt.ToString(), operatorId, objval.ContactNo, Outletid, objval.ContactNo);
                            //if (PaymentValidation == "TXN")
                            if (ReturnOutpot == "0")
                            {
                                CommissionDistributionHelper objChkComm = new CommissionDistributionHelper();
                                bool ComCheck = await objChkComm.DeductAmountFromMerchant(merchantid, "MOBILE", objval.RechargeAmt, objval.RechargeAmt, objval.RechargeAmt, operatorId, "Mobile Recharge", IpAddress, sRandomOTP, objval.ContactNo, "InstantPay", "MobileOperator");
                                if (ComCheck == true)
                                {
                                    string Outletid = string.Empty;
                                    string RechargeAmt = string.Empty;
                                    decimal AmountRecharge = Convert.ToDecimal(objval.RechargeAmt.ToString());
                                    RechargeAmt = objval.RechargeAmt.ToString() + ".00";
                                    //string payURL = "https://in.cyberplat.com/cgi-bin/rjio/rjio_pay.cgi";
                                    string payURL = GetServiceURL.OPTION2;
                                    payURL = payURL.Replace("_check", "");
                                    var Recharge = CyberPlateAPI._strValidationRequestLogs(SessionNo, objval.ContactNo, RechargeAmt, objval.OperatorName, JioPlanId);
                                    //var Recharge = CyberPlateAPI._strValidationRequestLogs(objval.ContactNo, RechargeAmt, objval.OperatorName, JioPlanId);
                                    ssl.message = ssl.Sign_With_PFX(Recharge, keyPath, "rahul123");
                                    ssl.htmlText = ssl.CallCryptoAPI(ssl.message, payURL);
                                    OutputResponse = "URL:\r\n" + GetServiceURL.OPTION1 + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;
                                    string RechargeOut_response = ssl.htmlText;
                                    string[] Recharge_response_Value = ParseReportCode(RechargeOut_response);
                                    string RechargeReturnOutpot = Recharge_response_Value[5];
                                    string RechargeReturnTransId = Recharge_response_Value[7];
                                    //string RechargeReturnAuthId = Recharge_response_Value[8];
                                    //string RechargeReturnTransStatus = Recharge_response_Value[9];
                                    string Certno = Recharge_response_Value[2];
                                    string RechargeErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(RechargeReturnOutpot);
                                    string Rcrgeresp = Convert.ToString(Recharge);
                                    if (RechargeReturnOutpot == "0")
                                    {
                                        //string StatusCheckApiUrl = GetServiceURL.OPTION3;
                                        ////var StatusCheckAPI = CyberPlateAPI._strValidationRequestLogs(objval.ContactNo, RechargeAmt, objval.OperatorName, JioPlanId);
                                        //var StatusCheckAPI = CyberPlateAPI._strStatusRequestLogs();
                                        //ssl.message = ssl.Sign_With_PFX(StatusCheckAPI, keyPath, "rahul123");
                                        //ssl.htmlText = ssl.CallCryptoAPI(ssl.message, StatusCheckApiUrl);
                                        //string RechargeOut_Status = ssl.htmlText;
                                        //string[] Recharge_response_Status = ParseReportCode(RechargeOut_Status);
                                        //string RechargeReturnOutputStatus = Recharge_response_Status[5];
                                        //string RechargeStatusErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(RechargeReturnOutputStatus);
                                        //if (RechargeReturnOutputStatus == "0")
                                        //{
                                        CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                        bool checkComm = await objComm.AllMemberCommissionDistribution(merchantid, "MOBILE", AmountRecharge, AmountRecharge, AmountRecharge, operatorId, "Prepaid Recharge", IpAddress, sRandomOTP);
                                        //    bool checkComm =await objComm.AllMemberCommissionDistribution(merchantid, "MOBILE", objval.RechargeAmt, objval.RechargeAmt, objval.RechargeAmt, operatorId, "Mobile Recharge", IpAddress, sRandomOTP);                                        
                                        if (checkComm == true)
                                        {
                                            var ApiResponse = db.TBL_INSTANTPAY_RECHARGE_RESPONSE.FirstOrDefault(x => x.CORELATIONID == sRandomOTP);
                                            ApiResponse.Ipay_Id = RechargeReturnTransId;
                                            ApiResponse.AgentId = Certno;
                                            ApiResponse.Opr_Id = operatorId;
                                            ApiResponse.AccountNo = objval.ContactNo;
                                            ApiResponse.Sp_Key = objval.OperatorName;
                                            ApiResponse.Trans_Amt = decimal.Parse(objval.RechargeAmt.ToString());
                                            ApiResponse.Charged_Amt = decimal.Parse(objval.RechargeAmt.ToString());
                                            ApiResponse.Opening_Balance = decimal.Parse(objval.RechargeAmt.ToString());
                                            ApiResponse.DateVal = System.DateTime.Now;
                                            ApiResponse.Status = RechargeErrorReturn;
                                            ApiResponse.Res_Code = RechargeReturnOutpot;
                                            ApiResponse.res_msg = RechargeErrorReturn;
                                            ApiResponse.RechargeType = objval.PrepaidRecharge;
                                            ApiResponse.RechargeResponse = Rcrgeresp;
                                            ApiResponse.ERROR_TYPE = "SUCCESS";
                                            ApiResponse.ISREVERSE = "Yes";
                                            ApiResponse.ISCOMMISSIONDISBURSE = "Yes";
                                            db.Entry(ApiResponse).State = System.Data.Entity.EntityState.Modified;
                                            db.SaveChanges();
                                            return Json("Transaction Successfull");
                                        }
                                        else
                                        {
                                            return Json("Transaction Failed");
                                        }
                                        ////return Json(outputval);
                                        //return Json(RechargeErrorReturn);
                                        //}
                                        //else
                                        //{
                                        //    CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                        //    string Statusval = objComm.RefundCommissionInWallet(sRandomOTP, "SERVICE");
                                        //    if (Statusval == "Return Success")
                                        //    {
                                        //        //return Json("Transaction Failed");
                                        //        return Json(RechargeErrorReturn);
                                        //    }
                                        //    else
                                        //    {
                                        //        //return Json(ErrorDescription);
                                        //        return Json(RechargeErrorReturn);
                                        //    }
                                        //    //return Json(ErrorDescription);
                                        //    return Json(RechargeErrorReturn);
                                        //}
                                    }
                                    else
                                    {
                                        CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                        string Statusval = objComm.RefundCommissionInWallet(sRandomOTP, "SERVICE");
                                        if (Statusval == "Return Success")
                                        {
                                            //return Json("Transaction Failed");
                                            return Json(RechargeErrorReturn);
                                        }
                                        else
                                        {
                                            //return Json(ErrorDescription);
                                            return Json(RechargeErrorReturn);
                                        }
                                        //return Json(ErrorDescription);
                                        return Json(RechargeErrorReturn);
                                    }
                                }
                                else
                                {
                                    return Json("Transaction Failed");
                                }
                            }
                            else
                            {
                                //return Json(PaymentValidation);
                                return Json(ErrorReturn);
                            }
                            #endregion
                        }
                        else
                        {
                            schemeCode = "PP";
                            var outletid = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.OUTLETID).FirstOrDefault();
                            if (outletid != null)
                            {
                                //var OperatorKey = db.TBL_SERVICE_PROVIDERS.Where(x => x.SERVICE_NAME.Contains(operatorId) && x.TYPE == "POSTPAID").Select(z => z.SERVICE_KEY).FirstOrDefault();
                                var OperatorKey = db.TBL_SERVICE_PROVIDERS.Where(x => x.SERVICE_NAME.Contains(OperatorName) && x.TYPE == "POSTPAID").Select(z => z.SERVICE_KEY).FirstOrDefault();

                                CommissionDistributionHelper objChkComm = new CommissionDistributionHelper();
                                //bool ComCheck = await objChkComm.DeductAmountFromMerchant(merchantid, "MOBILE", objval.RechargeAmt, objval.RechargeAmt, objval.RechargeAmt, operatorId, "Mobile PostPaid Recharge", IpAddress, sRandomOTP, objval.ContactNo, "Multilink", "POSTPAID");
                                bool ComCheck = await objChkComm.DeductAmountFromMerchant(merchantid, "MOBILE", objval.RechargeAmt, objval.RechargeAmt, objval.RechargeAmt, OperatorKey, "Mobile Recharge", IpAddress, sRandomOTP, objval.ContactNo, "Instantpay", Scheme);
                                if (ComCheck == true)
                                {
                                    string GeoLocation = string.Empty;
                                    string geoLoc = string.Empty;
                                    //if (objval.geolocation != "" || objval.geolocation != null)
                                    if (objval.geolocation != null)
                                    {
                                        if (objval.geolocation != "")
                                        {
                                            geoLoc = objval.geolocation.Replace("\"", "");
                                        }
                                        else
                                        {
                                            geoLoc = "";
                                        }
                                    }
                                    else
                                    {
                                        geoLoc = "";
                                    }
                                    var Recharge = BBPSPaymentAPI.BBPSBillPaymentPOSTPAID(operatorId, objval.ContactNo, objval.ContactNo, objval.RechargeAmt.ToString(), geoLoc, outletid, objval.Reference_Id);
                                    //var Recharge = BBPSPaymentAPI.BBPSBillPaymentPOSTPAID(OperatorKey, objval.ContactNo, objval.ContactNo, objval.RechargeAmt.ToString(), objval.geolocation, outletid, sRandomOTP);
                                    string Rcrgeresp = Convert.ToString(Recharge);

                                    string ErrorDescription = Recharge.status;
                                    string errorcodeValue = Recharge.statuscode;
                                    string errorcode = string.IsNullOrEmpty(errorcodeValue) ? ErrorDescription : errorcodeValue;//res.res_code;                            
                                    if (errorcode == "TXN" || errorcode == "TUP" || errorcode == "ERR")
                                    {
                                        string status = Recharge.status;
                                        string statusCode = Recharge.statuscode;
                                        var ipat_id = Recharge.data.ipay_id.Value;
                                        decimal trans_amt = decimal.Parse(Convert.ToString(Recharge.data.trans_amt.Value));
                                        //decimal chrg_amt = 0;
                                        decimal chrg_amt = decimal.Parse(Convert.ToString(Recharge.data.charged_amt.Value));
                                        //decimal.TryParse(Recharge.charged_amt.Value.ToString(), out chrg_amt);
                                        decimal Charged_Amt = decimal.Parse(Convert.ToString(chrg_amt));
                                        //decimal Charged_Amt = decimal.Parse(Convert.ToString(Recharge.charged_amt.Value));
                                        decimal openAmt = 0;
                                        decimal.TryParse(Recharge.data.opening_bal.Value, out openAmt);
                                        decimal Opening_Balance = decimal.Parse(Convert.ToString(openAmt));
                                        //decimal Charged_Amt = decimal.Parse(Convert.ToString(Recharge.data.charged_amt.Value));
                                        //decimal Opening_Balance = decimal.Parse(Convert.ToString(Recharge.data.opening_bal.Value));
                                        DateTime datevalue = Convert.ToDateTime(Recharge.data.datetime.Value);
                                        string agentId_Value = Recharge.data.agent_id.Value;
                                        string Operator_ID = Recharge.data.opr_id.Value;
                                        string outputval = Recharge.status;

                                        //var client = new WebClient();
                                        //var content = client.DownloadString("https://test.rdsncor.com/InstantPayCallBackAPIStatus?ipay_id=" + ipat_id + "&agent_id=" + Recharge.data.agent_id.Value + "&opr_id=" + Recharge.data.opr_id.Value + "&status=" + status + "&res_code=" + Recharge.statuscode + "&res_msg=" + status + "");
                                        //var callUrlRespResult = JObject.Parse(content);
                                        //string urlcalbackresp = callUrlRespResult.statuscode.Value;
                                        //if (urlcalbackresp == "TXN")
                                        //{

                                        //}

                                        CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                        bool checkComm = await objComm.AllMemberCommissionDistribution(merchantid, "MOBILE", objval.RechargeAmt, objval.RechargeAmt, objval.RechargeAmt, operatorId, "Mobile Postpaid Recharge", IpAddress, sRandomOTP);
                                        //bool checkComm =await objComm.AllMemberCommissionDistribution(merchantid, "MOBILE", objval.RechargeAmt, objval.RechargeAmt, objval.RechargeAmt, OperatorKey, "Mobile Postpaid Recharge", IpAddress, sRandomOTP);
                                        if (checkComm == true)
                                        {
                                            var ApiResponse = db.TBL_INSTANTPAY_RECHARGE_RESPONSE.FirstOrDefault(x => x.CORELATIONID == sRandomOTP);
                                            ApiResponse.Ipay_Id = ipat_id;
                                            ApiResponse.AgentId = agentId_Value;
                                            ApiResponse.Opr_Id = Operator_ID;
                                            ApiResponse.AccountNo = objval.ContactNo;
                                            ApiResponse.Sp_Key = Recharge.data.sp_key;
                                            ApiResponse.Trans_Amt = decimal.Parse(trans_amt.ToString());
                                            ApiResponse.Charged_Amt = decimal.Parse(Charged_Amt.ToString());
                                            ApiResponse.Opening_Balance = decimal.Parse(Opening_Balance.ToString());
                                            ApiResponse.DateVal = System.DateTime.Now;
                                            ApiResponse.Status = Recharge.data.status;
                                            ApiResponse.Res_Code = statusCode;
                                            ApiResponse.res_msg = status;
                                            ApiResponse.RechargeType = objval.PrepaidRecharge;
                                            ApiResponse.RechargeResponse = Rcrgeresp;
                                            ApiResponse.ERROR_TYPE = "SUCCESS";
                                            ApiResponse.ISREVERSE = "Yes";
                                            ApiResponse.ISCOMMISSIONDISBURSE = "Yes";
                                            db.Entry(ApiResponse).State = System.Data.Entity.EntityState.Modified;
                                            db.SaveChanges();
                                            return Json("Transaction Successfull");
                                        }
                                        else
                                        {
                                            return Json("Transaction Failed");
                                        }
                                        return Json(outputval);
                                    }
                                    else
                                    {
                                        CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                        string Statusval = objComm.RefundCommissionInWallet(sRandomOTP, "SERVICE");
                                        if (Statusval == "Return Success")
                                        {
                                            return Json("Transaction Failed");
                                        }
                                        else
                                        {
                                            return Json(ErrorDescription);
                                        }
                                        return Json(ErrorDescription);
                                    }
                                }
                                else
                                {
                                    return Json("Transaction Failed");
                                }
                            }
                            else
                            {
                                string msgval = "Please generate outlet id.. ";
                                return Json(msgval);
                            }
                        }
                    }
                    else
                    {
                        var msg = "Can't procceed with transaction.You don't have sufficient balance.";
                        return Json(msg);
                    }
                }
                else
                {
                    var msg = "Insufficient balance in master wallet.";
                    return Json(msg);
                }


            }
            catch (Exception ex)
            {
                //var msg = Recharge.ipay_errordesc.Value;
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- PostMobileRecharge(POST) Line No:- 230", ex.InnerException);
                var msg = "Please try again after 15 minute";
                return Json(msg);
                throw ex;
            }
        }
        [HttpPost]
        public async Task<JsonResult> AutoComplete(string prefix, string OperatorType)
        {
            try
            {
                var db = new DBContext();
                if (OperatorType == "PREPAID")
                {
                    var OperatorValue = await (from oper in db.TBL_SERVICE_PROVIDERS
                                               where oper.SERVICE_NAME.StartsWith(prefix) && oper.TYPE == "PREPAID" && oper.OPTION4 == "CYBERPLAT"
                                               select new
                                               {
                                                   //label = oper.SERVICE_NAME + "-" + oper.RECHTYPE,
                                                   label = oper.SERVICE_NAME,
                                                   val = oper.SERVICE_KEY,
                                                   image = oper.IMAGE
                                               }).ToListAsync();
                    return Json(OperatorValue);
                }
                else
                {
                    var OperatorValue = await (from oper in db.TBL_SERVICE_PROVIDERS
                                               where oper.SERVICE_NAME.StartsWith(prefix) && oper.TYPE == "POSTPAID" && oper.OPTION4 == "CYBERPLAT"
                                               select new
                                               {
                                                   //label = oper.SERVICE_NAME + "-" + oper.RECHTYPE,
                                                   label = oper.SERVICE_NAME,
                                                   val = oper.SERVICE_KEY,
                                                   image = oper.IMAGE
                                               }).ToListAsync();
                    return Json(OperatorValue);
                }
                
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- AutoComplete(POST) Line No:- 252", ex);
                throw ex;
            }
        }
        [HttpPost]
        public async Task<JsonResult> CircleName(string prefix)
        {
            try
            {
                var db = new DBContext();
                var OperatorValue = await (from oper in db.TBL_CIRCLE_OPERATOR
                                           where oper.CIRCLE_NAME.StartsWith(prefix)
                                           select new
                                           {
                                               //label = oper.SERVICE_NAME + "-" + oper.RECHTYPE,
                                               label = oper.CIRCLE_NAME,
                                               val = oper.CIRCLE_CODE
                                           }).ToListAsync();
                return Json(OperatorValue);
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- AutoComplete(POST) Line No:- 252", ex);
                throw ex;
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetPostPaidBillInformation(string AccountNo, string MobileNo, string GeoLocation, string ServiceKey,string Amount)
        {

            var db = new DBContext();
            string keyPath = Server.MapPath("~/Test CERT/myprivatekey.pfx");
            string OutputResponse = string.Empty;
            string SessionNo = DateTime.Parse(DateTime.Now.ToString()).ToString("dddMMMddyyyyHHmmss");

            string OperatorName = Request.Form["txtOperator"];
            string operatorId = Request.Form["OperatorId"];
            string Service_key = ServiceKey;
            const string agentId = "74Y104314";
            long merchantid = CurrentMerchant.MEM_ID;
            string geoLoc = string.Empty;

            if (GeoLocation != "" || GeoLocation != null)
            {
                geoLoc = GeoLocation.Replace("\"", "");
            }
            else
            {
                geoLoc = "";
            }
            #region PostPaid Bill Payment
            string AmountAllAmt = "4.32";
            var ApiUrl = db.TBL_SERVICE_PROVIDERS.Where(x => x.SERVICE_NAME == Service_key && x.OPTION4 == "CYBERPLAT").FirstOrDefault();
            //string AgentIdCyberplate = "BD01BD11AGT000000001";
            string AgentIdCyberplate = System.Configuration.ConfigurationManager.AppSettings["CyberPlateAgentId"];
            //string GetServiceURL = "https://in.cyberplat.com/cgi-bin/bu/bu_pay_check.cgi/340";
            string GetServiceURL = ApiUrl.OPTION1;
            var PaymentValidationBillFetch = CyberPlatBBPSBillUtilityRecharge._strValidationBBPSFetchBillUtility(SessionNo, AccountNo, "NA", "1.00", AgentIdCyberplate, "Nupoor", "Pansare", "test@gmail.com", MobileNo, "28.6139,78.5555", "492013", "123456");
            ssl.message = ssl.Sign_With_PFX(PaymentValidationBillFetch, keyPath, "rahul123");
            ssl.htmlText = ssl.CallCryptoAPI(ssl.message, GetServiceURL);
            OutputResponse = "URL:\r\n" + GetServiceURL + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;
            string Out_response = ssl.htmlText;
            string[] response_Value = BBPSParseReportCode(Out_response);

            

            string ReturnOutpot = response_Value[5];
            
            string ErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(ReturnOutpot);
            #endregion           
            if (ReturnOutpot == "0")
            {
                string Final_ErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(ReturnOutpot);
                string returnUrl = Server.UrlDecode(response_Value[5]);
                //var Objsettng = returnUrl.Remove(0, 8);
                var Objsettng = returnUrl;
                JObject json = JObject.Parse(Objsettng);
                var ResponseResult = JObject.Parse(Objsettng);
                dynamic FinalResp = ResponseResult;

                var data = JsonConvert.SerializeObject(FinalResp);
                //return Json(data, JsonRequestBehavior.AllowGet);
                return Json(new { Result = "0", data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //var data = JsonConvert.SerializeObject(PaymentValidation);
                var data = ErrorReturn;
                //return Json(ErrorReturn, JsonRequestBehavior.AllowGet);
                return Json(new { Result = "1", data = ErrorReturn }, JsonRequestBehavior.AllowGet);
            }

            //var Pincode = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.PINCODE).FirstOrDefault();            
            //var outletid = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.OUTLETID).FirstOrDefault();
            //string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
            //var PaymentValidation = BBPSPaymentAPI.BBPSBillPaymentValidation(ServiceKey, MobileNo, MobileNo, Amount, geoLoc, outletid, sRandomOTP);
            //string errordesc = PaymentValidation.statuscode;
            //if (errordesc == "TXN")
            //{
            //    var data = JsonConvert.SerializeObject(PaymentValidation);
            //    //return Json(data, JsonRequestBehavior.AllowGet);
            //    return Json(data, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    var data = JsonConvert.SerializeObject(PaymentValidation);
            //    return Json(data, JsonRequestBehavior.AllowGet);
            //}

        }



        //End Mobile recharge section      
        #endregion

        // DTH Recharge    
        #region DTH Recharge  
        [HttpPost]
        public async Task<JsonResult> DTHCircleName(string prefix)
        {
            initpage();
            try
            {
                var db = new DBContext();
                var OperatorValue = await (from oper in db.TBL_CIRCLE_OPERATOR
                                           where oper.CIRCLE_NAME.StartsWith(prefix)
                                           select new
                                           {
                                               //label = oper.SERVICE_NAME + "-" + oper.RECHTYPE,
                                               label = oper.CIRCLE_NAME,
                                               val = oper.CIRCLE_CODE
                                           }).ToListAsync();
                return Json(OperatorValue);
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- AutoComplete(POST) Line No:- 252", ex);
                throw ex;
            }
        }
        [HttpPost]
        public async Task<JsonResult> AutoDTHRechargeComplete(string prefix)
        {
            try
            {
                var db = new DBContext();
                var OperatorValue = await (from oper in db.TBL_SERVICE_PROVIDERS
                                           where oper.SERVICE_NAME.StartsWith(prefix) && oper.TYPE == "DTHOPERATOR" && oper.OPTION4 == "CYBERPLAT"
                                           select new
                                           {
                                               label = oper.SERVICE_NAME,
                                               val = oper.SERVICE_KEY,
                                               image = oper.IMAGE
                                           }).ToListAsync();

                return Json(OperatorValue);
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- AutoDTHRechargeComplete(POST) Line No:- 293", ex);
                throw ex;
            }
            //var db = new DBContext();
            //var OperatorValue = (from oper in db.TBL_OPERATOR_MASTER
            //                     where oper.OPERATORNAME.StartsWith(prefix) && oper.OPERATORTYPE == "DTH"
            //                     select new
            //                     {
            //                         label = oper.OPERATORNAME + "-" + oper.RECHTYPE,
            //                         val = oper.PRODUCTID
            //                     }).ToList();

            //return Json(OperatorValue);
        }
        public ActionResult DTHRecharge()
        {
            initpage();

           
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult POSTDTHRecharge(DTHRechargeModel objval)
        public async Task<JsonResult> POSTDTHRecharge(MobileRechargeModel objval)
        {
            initpage();
            try
            {
                string keyPath = Server.MapPath("~/Test CERT/myprivatekey.pfx");
                string OutputResponse = string.Empty;
                //string OperatorName = Request.Form["DTHOperatorName"];
                //    string operatorId = Request.Form["DTHOperatorId"];
                //const string agentId = "2"; 
                string IpAddress = string.Empty;
                if (objval.IpAddress != null)
                {
                    IpAddress = objval.IpAddress.Replace("\"", "");
                }
                else
                {
                    IpAddress = "";
                }

                string AccountId_Val = System.Configuration.ConfigurationManager.AppSettings["MOSACCONTID"];
                var db = new DBContext();
                var getWhiteLevelID = await (from mrnt in db.TBL_MASTER_MEMBER
                                             join dist in db.TBL_MASTER_MEMBER on mrnt.INTRODUCER equals dist.MEM_ID
                                             //join super in db.TBL_MASTER_MEMBER on dist.INTRODUCER equals super.MEM_ID
                                             join WBL in db.TBL_MASTER_MEMBER on dist.UNDER_WHITE_LEVEL equals WBL.MEM_ID
                                             where mrnt.MEM_ID == CurrentMerchant.MEM_ID
                                             select new
                                             {
                                                 WHTBalance = WBL.BALANCE
                                             }).FirstOrDefaultAsync();
                if (getWhiteLevelID.WHTBalance >= objval.RechargeAmt)
                {

                    //var check_walletAmt = db.TBL_ACCOUNTS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                    var check_walletAmt = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).FirstOrDefaultAsync();
                    if (objval.RechargeAmt <= check_walletAmt.BALANCE)
                    {
                        string OperatorName = Request.Form["OperatorName"];
                        string operatorId = Request.Form["DTHOperatorId"];
                        string CircleCode = Request.Form["DTHCircleCodeId"];
                        long merchantid = 0;
                        long.TryParse(Session["MerchantUserId"].ToString(), out merchantid);

                        string transType = string.Empty;
                        string Scheme = objval.PrepaidRecharge;
                        //string[] saAllowedCharacters = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
                        //string sRandomOTP = GenerateRandomOTP(6, saAllowedCharacters);
                        string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());

                        decimal AllAmount = 0;
                        string AddAmt = "4.32";
                        AllAmount = Math.Round(((Convert.ToDecimal(objval.RechargeAmt) + Convert.ToDecimal(AddAmt))), 2);
                        string SessionNo = DateTime.Parse(DateTime.Now.ToString()).ToString("dddMMMddyyyyHHmmss");
                        var GetServiceURL = db.TBL_SERVICE_PROVIDERS.Where(x => x.SERVICE_NAME == objval.OperatorName && x.OPTION4 == "CYBERPLAT").FirstOrDefault();
                        var PaymentValidation = CyberPlatDTHRecharge._strValidationDTHRequest(SessionNo, objval.ContactNo, objval.RechargeAmt.ToString(), AllAmount.ToString());
                        //var PaymentValidation = CyberPlateAPI._strValidationRequestLogs(objval.ContactNo, objval.RechargeAmt.ToString(), objval.OperatorName, JioPlanId);
                        ssl.message = ssl.Sign_With_PFX(PaymentValidation, keyPath, "rahul123");
                        ssl.htmlText = ssl.CallCryptoAPI(ssl.message, GetServiceURL.OPTION1);
                        OutputResponse = "URL:\r\n" + GetServiceURL.OPTION1 + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;
                        string Out_response = ssl.htmlText;
                        string[] response_Value = ParseReportCode(Out_response);
                        string ReturnOutpot = response_Value[5];
                        string ErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(ReturnOutpot);
                        CommissionDistributionHelper objChkComm = new CommissionDistributionHelper();
                        if (ReturnOutpot == "0")
                        {
                            bool ComCheck = await objChkComm.DeductAmountFromMerchant(merchantid, "UTILITY", objval.RechargeAmt, objval.RechargeAmt, objval.RechargeAmt, operatorId, "DTH Recharge", IpAddress, sRandomOTP, objval.ContactNo, "Multilink", "DTHOPERATOR");                            
                            if (ComCheck == true)
                            {

                                //var Prepaidoutletid = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.OUTLETID).FirstOrDefault();
                                //var PaymentValidation = PaymentAPI.Validation(agentId, objval.RechargeAmt.ToString(), operatorId, objval.ContactNo, Prepaidoutletid, objval.ContactNo);
                                //if (PaymentValidation == "TXN")
                                //{
                                //var Recharge = PaymentAPI.Payment(agentId, objval.RechargeAmt.ToString(), operatorId, objval.ContactNo, Prepaidoutletid, objval.ContactNo);
                                //string Rcrgeresp = Convert.ToString(Recharge);

                                //string ErrorDescription = Recharge.status;
                                //string errorcodeValue = Recharge.res_code;
                                //string errorcode = string.IsNullOrEmpty(errorcodeValue) ? ErrorDescription : errorcodeValue;//res.res_code;                            
                                //if (errorcode == "TXN" || errorcode == "TUP" || errorcode == "ERR")
                                string Outletid = string.Empty;
                                string RechargeAmt = string.Empty;
                                decimal AmountRecharge = Convert.ToDecimal(objval.RechargeAmt.ToString());
                                RechargeAmt = objval.RechargeAmt.ToString() + ".00";
                                //string payURL = "https://in.cyberplat.com/cgi-bin/rjio/rjio_pay.cgi";
                                string payURL = GetServiceURL.OPTION2;
                                payURL = payURL.Replace("_check", "");
                                var Recharge = CyberPlatDTHRecharge._strValidationDTHRequest(SessionNo, objval.ContactNo, objval.RechargeAmt.ToString(), AllAmount.ToString());                                
                                ssl.message = ssl.Sign_With_PFX(Recharge, keyPath, "rahul123");
                                ssl.htmlText = ssl.CallCryptoAPI(ssl.message, payURL);
                                OutputResponse = "URL:\r\n" + GetServiceURL.OPTION1 + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;
                                string RechargeOut_response = ssl.htmlText;
                                string[] Recharge_response_Value = ParseReportCode(RechargeOut_response);
                                string RechargeReturnOutpot = Recharge_response_Value[5];
                                string RechargeReturnTransId = Recharge_response_Value[7];
                                //string RechargeReturnAuthId = Recharge_response_Value[8];
                                //string RechargeReturnTransStatus = Recharge_response_Value[9];
                                string Certno = Recharge_response_Value[2];
                                string RechargeErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(RechargeReturnOutpot);
                                string Rcrgeresp = Convert.ToString(Recharge);
                                if (RechargeReturnOutpot == "0")
                                {
                                    //string status = Recharge.status;
                                    //string statusCode = Recharge.res_code;
                                    //var ipat_id = Recharge.data.ipay_id.Value;
                                    //decimal trans_amt = decimal.Parse(Convert.ToString(Recharge.trans_amt.Value));
                                    //decimal chrg_amt = 0;
                                    //decimal.TryParse(Recharge.charged_amt.Value, out chrg_amt);
                                    //decimal Charged_Amt = decimal.Parse(Convert.ToString(chrg_amt));
                                    //decimal openAmt = 0;
                                    //decimal.TryParse(Recharge.opening_bal.Value, out openAmt);
                                    //decimal Opening_Balance = decimal.Parse(Convert.ToString(openAmt));
                                    ////decimal Opening_Balance = decimal.Parse(Convert.ToString(Recharge.opening_bal.Value));
                                    //DateTime datevalue = Convert.ToDateTime(Recharge.datetime.Value);
                                    //string agentId_Value = Recharge.agent_id;
                                    //string Operator_ID = Recharge.opr_id;
                                    //string outputval = Recharge.status;
                                    CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                    bool checkComm = await objComm.AllMemberCommissionDistribution(merchantid, "UTILITY", objval.RechargeAmt, objval.RechargeAmt, objval.RechargeAmt, operatorId, "DTH Recharge", IpAddress, sRandomOTP);
                                    //bool checkComm =await objComm.AllMemberCommissionDistribution(merchantid, "MOBILE", objval.RechargeAmt, objval.RechargeAmt, objval.RechargeAmt, OperatorKey, "Mobile Postpaid Recharge", IpAddress, sRandomOTP);
                                    if (checkComm == true)
                                    {
                                        var ApiResponse = db.TBL_INSTANTPAY_RECHARGE_RESPONSE.FirstOrDefault(x => x.CORELATIONID == sRandomOTP);
                                        ApiResponse.Ipay_Id = RechargeReturnTransId;
                                        ApiResponse.AgentId = Certno;
                                        ApiResponse.Opr_Id = operatorId;
                                        ApiResponse.AccountNo = objval.ContactNo;
                                        ApiResponse.Sp_Key = objval.OperatorName;
                                        ApiResponse.Trans_Amt = decimal.Parse(objval.RechargeAmt.ToString());
                                        ApiResponse.Charged_Amt = decimal.Parse(objval.RechargeAmt.ToString());
                                        ApiResponse.Opening_Balance = decimal.Parse(objval.RechargeAmt.ToString());
                                        ApiResponse.DateVal = System.DateTime.Now;
                                        ApiResponse.Status = RechargeErrorReturn;
                                        ApiResponse.Res_Code = RechargeReturnOutpot;
                                        ApiResponse.res_msg = RechargeErrorReturn;
                                        ApiResponse.RechargeType = objval.PrepaidRecharge;
                                        ApiResponse.RechargeResponse = Rcrgeresp;
                                        ApiResponse.ERROR_TYPE = "SUCCESS";
                                        ApiResponse.ISREVERSE = "Yes";
                                        ApiResponse.ISCOMMISSIONDISBURSE = "Yes";
                                        db.Entry(ApiResponse).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                        //ApiResponse.Ipay_Id = ipat_id;
                                        //ApiResponse.AgentId = agentId_Value;
                                        //ApiResponse.Opr_Id = Operator_ID;
                                        //ApiResponse.AccountNo = objval.ContactNo;
                                        //ApiResponse.Sp_Key = Recharge.sp_key;
                                        //ApiResponse.Trans_Amt = decimal.Parse(trans_amt.ToString());
                                        //ApiResponse.Charged_Amt = decimal.Parse(Charged_Amt.ToString());
                                        //ApiResponse.Opening_Balance = decimal.Parse(Opening_Balance.ToString());
                                        //ApiResponse.DateVal = System.DateTime.Now;
                                        //ApiResponse.Status = Recharge.status;
                                        //ApiResponse.Res_Code = statusCode;
                                        //ApiResponse.res_msg = status;
                                        //ApiResponse.RechargeType = objval.PrepaidRecharge;
                                        //ApiResponse.RechargeResponse = Rcrgeresp;
                                        //ApiResponse.ERROR_TYPE = "SUCCESS";
                                        //ApiResponse.ISREVERSE = "Yes";
                                        //ApiResponse.ISCOMMISSIONDISBURSE = "Yes";
                                        //db.Entry(ApiResponse).State = System.Data.Entity.EntityState.Modified;
                                        //db.SaveChanges();
                                        return Json("Transaction Successfull");
                                    }
                                    else
                                    {
                                        return Json("Transaction Failed");
                                    }
                                    return Json(RechargeErrorReturn);
                                }
                                else
                                {
                                    CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                    string Statusval = objComm.RefundCommissionInWallet(sRandomOTP, "SERVICE");
                                    if (Statusval == "Return Success")
                                    {
                                        return Json("Transaction Failed");
                                    }
                                    else
                                    {
                                        return Json(RechargeErrorReturn);
                                    }
                                    return Json(RechargeErrorReturn);
                                }
                            }
                            else
                            {
                                return Json("Transaction Failed");
                            }
                        }
                        else
                        {
                            return Json(ErrorReturn);
                        }   
                    }
                    else
                    {
                        var msg = "Can't procceed with transaction.You don't have sufficient balance.";
                        return Json(msg);
                    }
                }
                else
                {
                    var msg = "Insufficient balance in master wallet.";
                    return Json(msg);
                }
            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return Json(msg);
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- POSTDTHRecharge(POST) Line No:- 392", ex);
                throw ex;
            }
        }
        #endregion

        // Land Line recharge
        #region Landline Recharge
        public ActionResult LandlineRecharge()
        {
            initpage();
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> AutoLandlineRechargeComplete(string prefix)
        {
            try
            {
                var db = new DBContext();
                var OperatorValue = await (from oper in db.TBL_SERVICE_PROVIDERS
                                           where oper.SERVICE_NAME.StartsWith(prefix) && oper.TYPE == "LANDLINE" && oper.OPTION4 == "CYBERPLAT"
                                           select new
                                           {
                                               label = oper.SERVICE_NAME,
                                               val = oper.SERVICE_KEY,
                                               image = oper.IMAGE
                                           }).ToListAsync();

                return Json(OperatorValue);
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- AutoLandlineRechargeComplete(POST) Line No:- 421", ex);
                throw ex;
            }
            //var db = new DBContext();
            //var OperatorValue = (from oper in db.TBL_OPERATOR_MASTER
            //                     where oper.OPERATORNAME.StartsWith(prefix) && oper.OPERATORTYPE == "LANDLINE"
            //                     select new
            //                     {
            //                         label = oper.OPERATORNAME + "-" + oper.RECHTYPE,
            //                         val = oper.PRODUCTID
            //                     }).ToList();

            //return Json(OperatorValue);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult PostLindlineRecharge(LandlineRecharge objval)
        public async Task<JsonResult> PostLindlineRecharge(LandlineRecharge objval)
        {
            initpage();
            try
            {
                var db = new DBContext();
                //var check_walletAmt = db.TBL_ACCOUNTS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                var getWhiteLevelID = await (from mrnt in db.TBL_MASTER_MEMBER
                                             join dist in db.TBL_MASTER_MEMBER on mrnt.INTRODUCER equals dist.MEM_ID
                                             //join super in db.TBL_MASTER_MEMBER on dist.INTRODUCER equals super.MEM_ID
                                             join WBL in db.TBL_MASTER_MEMBER on dist.UNDER_WHITE_LEVEL equals WBL.MEM_ID
                                             where mrnt.MEM_ID == CurrentMerchant.MEM_ID
                                             select new
                                             {
                                                 WHTBalance = WBL.BALANCE
                                             }).FirstOrDefaultAsync();
                if (getWhiteLevelID.WHTBalance >= objval.RechargeAmt)
                {
                    var check_walletAmt = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).FirstOrDefaultAsync();
                    if (objval.RechargeAmt <= check_walletAmt.BALANCE)
                    {
                        string GeoLocation = string.Empty;
                        string geoLoc = string.Empty;
                        string IpAddress = string.Empty;
                        //if (objval.geolocation != "" || objval.geolocation != null)
                        if (objval.IpAddress != null)
                        {
                            if (objval.IpAddress != "")
                            {
                                IpAddress = objval.IpAddress.Replace("\"", "");
                            }
                            else
                            {
                                IpAddress = "";
                            }
                        }
                        else
                        {
                            IpAddress = "";
                        }
                        if (objval.geolocation != null )
                        {
                            if (objval.geolocation != "")
                            {
                                geoLoc = objval.geolocation.Replace("\"", "");
                            }
                            else
                            {
                                geoLoc = "";
                            }                            
                        }
                        else
                        {
                            geoLoc = "";
                        }
                        string OperatorName = Request.Form["OperatorName"];
                        string operatorId = Request.Form["LandlineOperatorId"];
                        const string agentId = "74Y104314";
                        long merchantid = 0;
                        long.TryParse(Session["MerchantUserId"].ToString(), out merchantid);
                        var outletid = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.OUTLETID).FirstOrDefault();
                        if (outletid != null)
                        {
                            string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
                            CommissionDistributionHelper objChkComm = new CommissionDistributionHelper();
                            bool ComCheck =await objChkComm.DeductAmountFromMerchant(merchantid, "UTILITY", objval.RechargeAmt, objval.RechargeAmt, objval.RechargeAmt, operatorId, "LandLine Recharge", IpAddress, sRandomOTP, objval.ContactNo, "Instantpay", "LANDLINE");
                            if (ComCheck == true)
                            {
                                
                                var Recharge = BBPSPaymentAPI.BBPSBillPaymentLANDLINE(operatorId, objval.CustomerNo, objval.ContactNo, objval.RechargeAmt.ToString(), geoLoc, outletid, objval.LandLineRefId);
                                string LandlineResponse = Convert.ToString(Recharge);
                                string ErrorDescription = Recharge.status;
                                string errorcodeValue = Recharge.statuscode;
                               
                                string errorcode = string.IsNullOrEmpty(errorcodeValue) ? ErrorDescription : errorcodeValue;
                                //string errorcode = string.IsNullOrEmpty(Recharge.res_code.Value) ? Recharge.res_msg.Value : Recharge.res_code.Value;//res.res_code;
                                if (errorcode == "TXN" || errorcode == "TUP" )
                                {
                                    string status = Recharge.status;
                                    var ipat_id = Recharge.data.ipay_id.Value;
                                    var opr_idVal = Recharge.data.opr_id;
                                    var RechMsgStatus = Recharge.data.status;
                                    decimal trans_amt = decimal.Parse(Convert.ToString(Recharge.data.trans_amt.Value));
                                    decimal Charged_Amt = decimal.Parse(Convert.ToString(Recharge.data.charged_amt.Value));
                                    decimal Opening_Balance = decimal.Parse(Convert.ToString(Recharge.data.opening_bal.Value));
                                    DateTime datevalue = Convert.ToDateTime(Recharge.data.datetime.Value);
                                    string agentId_Value = Recharge.data.agent_id;
                                    string Operator_ID = Recharge.data.opr_id;
                                    string statusCode = Recharge.statuscode;
                                    string outputval = Recharge.status;
                                    
                                    DMR_API_Response objLandlineResp = new DMR_API_Response()
                                    {
                                        serviceTax = "0",
                                        clientRefId = ipat_id,
                                        fee = "0",
                                        initiatorId = ipat_id,
                                        accountNumber = objval.AccountNo,
                                        txnStatus = status,
                                        name = opr_idVal,
                                        ifscCode = trans_amt.ToString(),
                                        impsRespCode = trans_amt.ToString(),
                                        impsRespMessage = status,
                                        txnId = ipat_id,
                                        timestamp = RechMsgStatus
                                        //serviceTax = "10",
                                        //clientRefId = "23026041",
                                        //fee = "00",
                                        //initiatorId = "23026041",
                                        //accountNumber = "9903116214",
                                        //txnStatus = "00",
                                        //name = "AIRTEL",
                                        //ifscCode = "10714.41",
                                        //impsRespCode = "10714.41",
                                        //impsRespMessage = "00",
                                        //txnId = "23026041",
                                        //timestamp = "00"
                                    };

                                    CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                    //bool checkComm = objComm.AllMemberCommissionDistribution(merchantid, "MOBILE", objval.RechargeAmt, objval.RechargeAmt, objval.RechargeAmt, OperatorKey, "Mobile Postpaid Recharge", objval.IpAddress, sRandomOTP);


                                    bool checkComm =await objComm.AllMemberCommissionDistribution(merchantid, "UTILITY", objval.RechargeAmt, Charged_Amt, Opening_Balance, operatorId, "LandLine Bill", IpAddress, sRandomOTP, objLandlineResp, LandlineResponse);
                                    if (checkComm == true)
                                    {
                                        var ApiResponse = db.TBL_INSTANTPAY_RECHARGE_RESPONSE.FirstOrDefault(x => x.CORELATIONID == sRandomOTP);
                                        ApiResponse.Ipay_Id = ipat_id;
                                        ApiResponse.AgentId = agentId_Value;
                                        ApiResponse.Opr_Id = Operator_ID;
                                        ApiResponse.AccountNo = Recharge.data.account_no;
                                        ApiResponse.Sp_Key = Recharge.data.sp_key;
                                        ApiResponse.Trans_Amt = decimal.Parse(trans_amt.ToString());
                                        ApiResponse.Charged_Amt = decimal.Parse(Charged_Amt.ToString());
                                        ApiResponse.Opening_Balance = decimal.Parse(Opening_Balance.ToString());
                                        ApiResponse.DateVal = System.DateTime.Now;
                                        ApiResponse.Status = Recharge.data.status;
                                        ApiResponse.Res_Code = statusCode;
                                        ApiResponse.res_msg = status;
                                        ApiResponse.RechargeType = "LANDLINE";
                                        ApiResponse.RechargeResponse = LandlineResponse;
                                        ApiResponse.ERROR_TYPE = "SUCCESS";
                                        ApiResponse.ISREVERSE = "Yes";
                                        ApiResponse.ISCOMMISSIONDISBURSE = "Yes";
                                        db.Entry(ApiResponse).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                        //return Json(outputval);
                                        return Json("Transaction Successfull");
                                    }
                                    else
                                    {
                                        return Json("Transaction Failed");
                                    }
                                    //TBL_INSTANTPAY_RECHARGE_RESPONSE insta = new TBL_INSTANTPAY_RECHARGE_RESPONSE()
                                    //{
                                    //    Ipay_Id = ipat_id,
                                    //    AgentId = agentId_Value,
                                    //    Opr_Id = Operator_ID,
                                    //    AccountNo = Recharge.data.account_no,
                                    //    Sp_Key = Recharge.data.sp_key,
                                    //    Trans_Amt = trans_amt,
                                    //    Charged_Amt = Charged_Amt,
                                    //    Opening_Balance = Opening_Balance,
                                    //    DateVal = System.DateTime.Now,
                                    //    Status = Recharge.data.status,
                                    //    Res_Code = Recharge.statuscode,
                                    //    res_msg = Recharge.status,
                                    //    Mem_ID = merchantid,
                                    //    RechargeType = "LANDLINE",
                                    //    IpAddress = objval.IpAddress,
                                    //    API_Name = "Instantpay"
                                    //};
                                    //db.TBL_INSTANTPAY_RECHARGE_RESPONSE.Add(insta);
                                    //await db.SaveChangesAsync();                            
                                    //var client = new WebClient();
                                    //var content = client.DownloadString("http://test.rdsncor.com/InstantPayCallBackAPIStatus?ipay_id=" + ipat_id + "&agent_id=" + agentId_Value + "&opr_id=" + Operator_ID + "&status=" + status + "&res_code=" + statusCode + "&res_msg=" + status + "");
                                    //var callUrlRespResult = JObject.Parse(content);
                                    //string urlcalbackresp = callUrlRespResult.statuscode.Value;
                                    //if (urlcalbackresp == "TXN")
                                    //{
                                    //    string UniqueIdgen = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
                                    //    CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                    //    bool checkComm = objComm.AllMemberCommissionDistribution(merchantid, "UTILITY", objval.RechargeAmt, Charged_Amt, Opening_Balance, operatorId, "LandLine Bill", objval.IpAddress, UniqueIdgen);

                                    //}
                                    //return Json(outputval);
                                }
                                else
                                {
                                    CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                    string Statusval = objComm.RefundCommissionInWallet(sRandomOTP, "SERVICE");
                                    if (Statusval == "Return Success")
                                    {
                                        return Json("Transaction Failed");
                                    }
                                    else
                                    {
                                        return Json(ErrorDescription);
                                    }
                                    return Json(ErrorDescription);
                                }
                            }
                            else
                            {
                                return Json("Transaction Failed");
                            }
                        }
                        else
                        {
                            string msgval = "Please generate outlet id.. ";
                            return Json(msgval);
                        }
                    }
                    else
                    {
                        var msg = "Can't procceed with transaction.You don't have sufficient balance.";
                        return Json(msg);
                    }
                }
                else
                {
                    var msg = "Insufficient balance in master wallet.";
                    return Json(msg);
                }   
            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return Json(msg);
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- PostLindlineRecharge(POST) Line No:- 469", ex);
                throw ex;
            }
        }


        [HttpPost]
        public async Task<JsonResult> GetLandlineBillInformation(string AccountNo, string MobileNo, string GeoLocation, string ServiceKey,  string CityName,string landlineAmt)
        {
            initpage();
            var db = new DBContext();

            string OperatorName = Request.Form["hfLandlineOperator"];
            string operatorId = Request.Form["LandlineOperatorId"];
            string Service_key = ServiceKey;
            const string agentId = "74Y104314";
            long merchantid = CurrentMerchant.MEM_ID;
            var Pincode = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.PINCODE).FirstOrDefault();
            //string option9 = objval.geolocation + "|" + Pincode;
            var outletid = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.OUTLETID).FirstOrDefault();
            string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
            var PaymentValidation = BBPSPaymentAPI.BBPSBillPaymentElectricityValidation(Service_key, MobileNo, "", AccountNo, landlineAmt, GeoLocation, outletid, CityName, sRandomOTP);

            string errordesc = PaymentValidation.status;
            //var PaymentValidation = ElectricityPaymentAPI.ElectricityValidation(agentId, objval.RechargeAmt.ToString(), objval.MobileNo.ToString(), operatorId, objval.CustomerId, option9, outletid.ToString());
            if (PaymentValidation.statuscode == "TXN")
            {
                var data = JsonConvert.SerializeObject(PaymentValidation);
                return Json(data, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var data = JsonConvert.SerializeObject(PaymentValidation);
                return Json(data, JsonRequestBehavior.AllowGet);
            }



            //var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == EmployeeId).OrderBy(c => c.TYPE).ToList();
            ////var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "PREPAID" || x.TYPE == "POSTPAID").OrderBy(c => c.TYPE).ToList();
            //ViewBag.GetElectricityInfo = OperatorList;
            //return Json("");
        }


        #endregion

        // Data Card Recharge
        #region Data Card Recharge
        public ActionResult DatacardRecharge()
        {
            initpage();
            return View();
        }
        #endregion

        #region Broadband segment
        public ActionResult BroadbandRecharge()
        {
            initpage();
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> AutoBroadbandRechargeComplete(string prefix)
        {
            try
            {
                var db = new DBContext();
                var OperatorValue = await (from oper in db.TBL_SERVICE_PROVIDERS
                                           where oper.SERVICE_NAME.StartsWith(prefix) && oper.TYPE == "BROADBAND" && oper.OPTION4 == "CYBERPLAT"
                                           select new
                                           {
                                               label = oper.SERVICE_NAME,
                                               val = oper.SERVICE_KEY,
                                               image = oper.IMAGE
                                           }).ToListAsync();

                return Json(OperatorValue);
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- AutoBroadbandRechargeComplete(POST) Line No:- 510", ex);
                throw ex;
            }
        }
        //[HttpPost]
        //public async Task<JsonResult> GetBillBroadbandInformation(string AccountNo, string MobileNo, string GeoLocation, string ServiceKey, string Unitno)
        //{
        //    initpage();
        //    var db = new DBContext();

        //    string OperatorName = Request.Form["txtBroadbandOperator"];
        //    string operatorId = Request.Form["broadbandoperId"];
        //    string Service_key = ServiceKey;
        //    const string agentId = "74Y104314";
        //    long merchantid = CurrentMerchant.MEM_ID;
        //    var Pincode = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.PINCODE).FirstOrDefault();
        //    //string option9 = objval.geolocation + "|" + Pincode;
        //    var outletid = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.OUTLETID).FirstOrDefault();
        //    string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
        //    var PaymentValidation = BBPSPaymentAPI.BBPSBillPaymentElectricityValidation(Service_key, MobileNo, Unitno, AccountNo, "", GeoLocation, outletid, "", sRandomOTP);
        //    string errordesc = PaymentValidation.status;

        //    //var PaymentValidation = ElectricityPaymentAPI.ElectricityValidation(agentId, objval.RechargeAmt.ToString(), objval.MobileNo.ToString(), operatorId, objval.CustomerId, option9, outletid.ToString());
        //    if (PaymentValidation.statuscode == "TXN")
        //    {
        //        var data = JsonConvert.SerializeObject(PaymentValidation);
        //        return Json(data, JsonRequestBehavior.AllowGet);

        //    }
        //    else
        //    {
        //        var data = JsonConvert.SerializeObject(PaymentValidation);
        //        return Json(data, JsonRequestBehavior.AllowGet);
        //    }

        //    //var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == EmployeeId).OrderBy(c => c.TYPE).ToList();
        //    ////var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "PREPAID" || x.TYPE == "POSTPAID").OrderBy(c => c.TYPE).ToList();
        //    //ViewBag.GetElectricityInfo = OperatorList;
        //    //return Json("");
        //}
        [HttpPost]
        public async Task<JsonResult> GetBROADBANDBillInformation(string AccountNo, string MobileNo, string GeoLocation, string ServiceKey, string landlineAmt)
        {
            initpage();
            var db = new DBContext();

            //string OperatorName = Request.Form["hfLandlineOperator"];
            //string operatorId = Request.Form["LandlineOperatorId"];
            string OperatorName = Request.Form["broadbandperatorID"];
            string operatorId = Request.Form["broadbandoperId"];
            string Service_key = ServiceKey;
            const string agentId = "74Y104314";
            long merchantid = CurrentMerchant.MEM_ID;
            var Pincode = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.PINCODE).FirstOrDefault();
            //string option9 = objval.geolocation + "|" + Pincode;
            var outletid = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.OUTLETID).FirstOrDefault();
            string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
            var PaymentValidation = BBPSPaymentAPI.BBPSBillPaymentElectricityValidation(Service_key, MobileNo, "", AccountNo, landlineAmt, GeoLocation, outletid, "", sRandomOTP);

            string errordesc = PaymentValidation.status;
            //var PaymentValidation = ElectricityPaymentAPI.ElectricityValidation(agentId, objval.RechargeAmt.ToString(), objval.MobileNo.ToString(), operatorId, objval.CustomerId, option9, outletid.ToString());
            if (PaymentValidation.statuscode == "TXN")
            {
                var data = JsonConvert.SerializeObject(PaymentValidation);
                return Json(data, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var data = JsonConvert.SerializeObject(PaymentValidation);
                return Json(data, JsonRequestBehavior.AllowGet);
            }



            //var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == EmployeeId).OrderBy(c => c.TYPE).ToList();
            ////var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "PREPAID" || x.TYPE == "POSTPAID").OrderBy(c => c.TYPE).ToList();
            //ViewBag.GetElectricityInfo = OperatorList;
            //return Json("");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> PostBroadbandRecharge(BroadbandViewModel objval)
        {
            initpage();
            try
            {
                var db = new DBContext();
                var getWhiteLevelID = await (from mrnt in db.TBL_MASTER_MEMBER
                                             join dist in db.TBL_MASTER_MEMBER on mrnt.INTRODUCER equals dist.MEM_ID
                                             //join super in db.TBL_MASTER_MEMBER on dist.INTRODUCER equals super.MEM_ID
                                             join WBL in db.TBL_MASTER_MEMBER on dist.UNDER_WHITE_LEVEL equals WBL.MEM_ID
                                             where mrnt.MEM_ID == CurrentMerchant.MEM_ID
                                             select new
                                             {
                                                 WHTBalance = WBL.BALANCE
                                             }).FirstOrDefaultAsync();
                if (getWhiteLevelID.WHTBalance >= objval.RechargeAmount)
                {
                    var check_walletAmt = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).FirstOrDefaultAsync();
                    if (objval.RechargeAmount <= check_walletAmt.BALANCE)
                    {
                        string IpAddress = string.Empty;
                        if (objval.IpAddress != null)
                        {
                            if (objval.IpAddress != "")
                            {
                                IpAddress = objval.IpAddress.Replace("\"", "");
                            }
                            else
                            {
                                IpAddress = "";
                            }
                        }
                        else
                        {
                            IpAddress = "";
                        }
                        string GeoLocation = string.Empty;
                        string geoLoc = string.Empty;
                        //if (objval.geolocation != "" || objval.geolocation != null)
                        if (objval.geolocation != null)
                        {
                            if (objval.geolocation != "")
                            {
                                geoLoc = objval.geolocation.Replace("\"", "");
                            }
                            else
                            {
                                geoLoc = "";
                            }
                        }
                        else
                        {
                            geoLoc = "";
                        }
                        string OperatorName = Request.Form["txtBroadbandOperator"];
                        string operatorId = Request.Form["broadbandperatorID"];
                        const string agentId = "74Y104314";
                        string REfID = Request.Form["Broadband_referenceID"];
                        string REfID_Val = Request.Form["BroadbandReferenceId"];
                        long merchantid = 0;
                        long.TryParse(Session["MerchantUserId"].ToString(), out merchantid);
                        var outletid = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.OUTLETID).FirstOrDefault();
                        if (outletid != null)
                        {

                            string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
                            CommissionDistributionHelper objChkComm = new CommissionDistributionHelper();
                            bool ComCheck = await objChkComm.DeductAmountFromMerchant(merchantid, "UTILITY", objval.RechargeAmount, objval.RechargeAmount, objval.RechargeAmount, operatorId, "BroadBand Recharge", IpAddress, sRandomOTP, objval.AccountNo, "Instantpay", "BROADBAND");
                            if (ComCheck == true)
                            {
                                //var Recharge = BBPSPaymentAPI.BBPSBillPaymentLANDLINE(operatorId, objval.CustomerNo, objval.ContactNo, objval.RechargeAmt.ToString(), geoLoc, outletid, objval.LandLineRefId);
                                var Recharge = BBPSPaymentAPI.BBPSBillPaymentBroadBand(operatorId, objval.PhoneNo,  objval.AccountNo, objval.RechargeAmount.ToString(), geoLoc, outletid, objval.BroadbandrefNo);
                                string Broadbandres = Convert.ToString(Recharge);
                                string ErrorDescription = Recharge.ipay_errordesc;
                                string errorcodeValue = Recharge.statuscode;
                                string errorcode = string.IsNullOrEmpty(errorcodeValue) ? ErrorDescription : errorcodeValue;
                                //if (errorcode == "TXN" || errorcode == "TUP")
                                if (errorcode == "TXN" || errorcode == "TUP" || errorcode== "ERR")
                                {
                                    string status = Recharge.status;
                                    //var ipat_id = Recharge.data.ipay_id.Value;
                                    //var opr_idVal = Recharge.data.opr_id.Value;
                                    //var RechMsgStatus = Recharge.data.status; 
                                    //decimal trans_amt = decimal.Parse(Convert.ToString(Recharge.trans_amt.Value));
                                    //decimal Charged_Amt = decimal.Parse(Convert.ToString(Recharge.charged_amt.Value));
                                    //decimal Opening_Balance = decimal.Parse(Convert.ToString(Recharge.opening_bal.Value));
                                    //DateTime datevalue = Convert.ToDateTime(Recharge.datetime.Value);
                                    //string Operator_ID = Recharge.opr_id.Value;
                                    //string agentId_Value = Recharge.agent_id.Value;
                                    //string statusCode = Recharge.res_code.Value;
                                    //string outputval = Recharge.res_msg;

                                    //string status = Recharge.status;
                                    var ipat_id = Recharge.data.ipay_id.Value;
                                    var opr_idVal = Recharge.data.opr_id;
                                    var RechMsgStatus = Recharge.data.status;
                                    decimal trans_amt = decimal.Parse(Convert.ToString(Recharge.data.trans_amt.Value));
                                    decimal Charged_Amt = decimal.Parse(Convert.ToString(Recharge.data.charged_amt.Value));
                                    decimal Opening_Balance = decimal.Parse(Convert.ToString(Recharge.data.opening_bal.Value));
                                    DateTime datevalue = Convert.ToDateTime(Recharge.data.datetime.Value);
                                    string agentId_Value = Recharge.data.agent_id;
                                    string Operator_ID = Recharge.data.opr_id;
                                    string statusCode = Recharge.statuscode;
                                    string outputval = Recharge.status;


                                    DMR_API_Response objBroadBandResp = new DMR_API_Response()
                                    {
                                        serviceTax = "0",
                                        clientRefId = ipat_id,
                                        fee = "0",
                                        initiatorId = ipat_id,
                                        accountNumber = objval.AccountNo,
                                        txnStatus = status,
                                        name = opr_idVal,
                                        ifscCode = trans_amt.ToString(),
                                        impsRespCode = trans_amt.ToString(),
                                        impsRespMessage = status,
                                        txnId = ipat_id,
                                        timestamp = RechMsgStatus
                                        //serviceTax = "10",
                                        //clientRefId = "23026041",
                                        //fee = "00",
                                        //initiatorId = "23026041",
                                        //accountNumber = "9903116214",
                                        //txnStatus = "00",
                                        //name = "AIRTEL",
                                        //ifscCode = "10714.41",
                                        //impsRespCode = "10714.41",
                                        //impsRespMessage = "00",
                                        //txnId = "23026041",
                                        //timestamp = "00"
                                    };

                                    CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                    //bool checkComm = objComm.AllMemberCommissionDistribution(merchantid, "UTILITY", objval.RechargeAmt, Charged_Amt, Opening_Balance, operatorId, "LandLine Bill", objval.IpAddress, sRandomOTP);                                    
                                    bool checkComm = await objComm.AllMemberCommissionDistribution(merchantid, "UTILITY", objval.RechargeAmount, Charged_Amt, Opening_Balance, operatorId, "BROADBAND", IpAddress, sRandomOTP, objBroadBandResp, Broadbandres);
                                    //bool checkComm = await objComm.AllMemberCommissionDistribution(merchantid, "UTILITY", objval.RechargeAmount, 0, 0, operatorId, "BROADBAND", IpAddress, sRandomOTP, objBroadBandResp, Broadbandres);
                                    if (checkComm == true)
                                    {
                                        var ApiResponse = db.TBL_INSTANTPAY_RECHARGE_RESPONSE.FirstOrDefault(x => x.CORELATIONID == sRandomOTP);
                                        ApiResponse.Ipay_Id = ipat_id;
                                        ApiResponse.AgentId = agentId_Value;
                                        ApiResponse.Opr_Id = Operator_ID;
                                        ApiResponse.AccountNo = Recharge.data.account_no;
                                        ApiResponse.Sp_Key = Recharge.data.sp_key;
                                        ApiResponse.Trans_Amt = decimal.Parse(trans_amt.ToString());
                                        ApiResponse.Charged_Amt = decimal.Parse(Charged_Amt.ToString());
                                        ApiResponse.Opening_Balance = decimal.Parse(Opening_Balance.ToString());
                                        ApiResponse.DateVal = System.DateTime.Now;
                                        ApiResponse.Status = Recharge.data.status;
                                        ApiResponse.Res_Code = statusCode;
                                        ApiResponse.res_msg = status;
                                        ApiResponse.RechargeType = "BROADBAND";
                                        ApiResponse.RechargeResponse = Broadbandres;
                                        ApiResponse.ERROR_TYPE = "SUCCESS";
                                        ApiResponse.ISREVERSE = "Yes";
                                        ApiResponse.ISCOMMISSIONDISBURSE = "Yes";
                                        db.Entry(ApiResponse).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                        //return Json(outputval);
                                        return Json("Transaction Successfull");
                                    }
                                    else
                                    {
                                        return Json("Transaction Failed");
                                    }

                                    //TBL_INSTANTPAY_RECHARGE_RESPONSE insta = new TBL_INSTANTPAY_RECHARGE_RESPONSE()
                                    //{
                                    //    Ipay_Id = ipat_id,
                                    //    AgentId = Recharge.agent_id.Value,
                                    //    Opr_Id = Recharge.opr_id.Value,
                                    //    AccountNo = Recharge.account_no.Value,
                                    //    Sp_Key = Recharge.sp_key.Value,
                                    //    Trans_Amt = trans_amt,
                                    //    Charged_Amt = Charged_Amt,
                                    //    Opening_Balance = Opening_Balance,
                                    //    DateVal = System.DateTime.Now,
                                    //    Status = Recharge.status.Value,
                                    //    Res_Code = Recharge.res_code.Value,
                                    //    res_msg = Recharge.res_msg.Value,
                                    //    Mem_ID = merchantid,
                                    //    RechargeType = "BROARDBAND",
                                    //    IpAddress = objval.IpAddress,
                                    //    API_Name = "Instantpay"
                                    //};
                                    //db.TBL_INSTANTPAY_RECHARGE_RESPONSE.Add(insta);
                                    //await db.SaveChangesAsync();
                                    //var client = new WebClient();
                                    //var content = client.DownloadString("https://test.rdsncor.com/InstantPayCallBackAPIStatus?ipay_id=" + ipat_id + "&agent_id=" + agentId_Value + "&opr_id=" + Operator_ID + "&status=" + status + "&res_code=" + statusCode + "&res_msg=" + status + "");

                                    //var callUrlRespResult = JObject.Parse(content);
                                    //string urlcalbackresp = callUrlRespResult.statuscode.Value;
                                    //if (urlcalbackresp == "TXN")
                                    //{
                                    //    string UniqueIdgen = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
                                    //    CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                    //    bool checkComm = objComm.AllMemberCommissionDistribution(merchantid, "UTILITY", objval.RechargeAmount, Charged_Amt, Opening_Balance, operatorId, "DTH Recharge", objval.IpAddress, UniqueIdgen);
                                    //}

                                    return Json(outputval);
                                }
                                else
                                {
                                    CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                    string Statusval = objComm.RefundCommissionInWallet(sRandomOTP, "SERVICE");
                                    if (Statusval == "Return Success")
                                    {
                                        return Json("Transaction Failed");
                                    }
                                    else
                                    {
                                        return Json(ErrorDescription);
                                    }
                                    return Json(ErrorDescription);
                                }

                            }
                            else
                            {
                                return Json("Transaction Failed");
                            }
                        }
                        else
                        {
                            string msgval = "Please generate outlet id.. ";
                            return Json(msgval);
                        }
                    }
                    else
                    {
                        var msg = "Can't procceed with transaction.You don't have sufficient balance.";
                        return Json(msg);
                    }
                }
                else
                {
                    var msg = "Insufficient balance in master wallet.";
                    return Json(msg);
                }
            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return Json(msg);
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- PostBroadbandRecharge(POST) Line No:- 547", ex);
                throw ex;
            }
        }
        #endregion#

        #region Electricity Recharge section
        public ActionResult ElectricityBillPayment()
        {
            initpage();
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> AutoElectricityBillService(string prefix)
        {
            try
            {
                var db = new DBContext();
                var OperatorValue = await (from oper in db.TBL_SERVICE_PROVIDERS
                                           where oper.SERVICE_NAME.StartsWith(prefix) && oper.TYPE == "ELECTRICITY" && oper.OPTION4 == "CYBERPLAT"
                                           select new
                                           {
                                               label = oper.SERVICE_NAME,
                                               val = oper.SERVICE_KEY,
                                               image = oper.IMAGE
                                           }).ToListAsync();

                return Json(OperatorValue);
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- AutoElectricityBillService(POST) Line No:- 576", ex);
                throw ex;
            }
        }
        [HttpPost]
        public async Task<JsonResult> GetBillInformation(string AccountNo, string MobileNo, string GeoLocation, string ServiceKey, string Unitno, string CityName,string FName,string LName,string EmailId,string PinNo)
        {
            initpage();
            string SessionNo = DateTime.Parse(DateTime.Now.ToString()).ToString("dddMMMddyyyyHHmmss");
            var db = new DBContext();
            string keyPath = Server.MapPath("~/Test CERT/myprivatekey.pfx");
            string OutputResponse = string.Empty;
            string OperatorName = Request.Form["txtElectricityOperator"];
            string operatorId = Request.Form["ElectricityoperId"];
            string Service_key = ServiceKey;
            const string agentId = "74Y104314";
            long merchantid = CurrentMerchant.MEM_ID;
            var Pincode = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.PINCODE).FirstOrDefault();
            //string option9 = objval.geolocation + "|" + Pincode;
            var outletid = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.OUTLETID).FirstOrDefault();
            string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString()); 

            #region Electrity Bill Payment
            string AmountAllAmt = "4.32";

            var ApiUrl = db.TBL_SERVICE_PROVIDERS.Where(x => x.SERVICE_NAME == Service_key && x.OPTION4 == "CYBERPLAT").FirstOrDefault();
            //string AgentIdCyberplate = "10p9714";
            //string AgentIdCyberplate = "BD01BD11AGT000000001";
            string AgentIdCyberplate = System.Configuration.ConfigurationManager.AppSettings["CyberPlateAgentId"];
            //string GetServiceURL = "https://in.cyberplat.com/cgi-bin/bu/bu_pay_check.cgi/340";
            string GetServiceURL = ApiUrl.OPTION1;
            var PaymentValidationBillFetch = CyberPlatBBPSBillUtilityRecharge._strValidationBBPSFetchBillUtility(SessionNo, AccountNo, "4572", "1.00", AgentIdCyberplate, FName, LName, EmailId, MobileNo, "28.5506,77.2692", "492013", "123456");
            ssl.message = ssl.Sign_With_PFX(PaymentValidationBillFetch, keyPath, "rahul123");
            ssl.htmlText = ssl.CallCryptoAPI(ssl.message, GetServiceURL);
            OutputResponse = "URL:\r\n" + GetServiceURL + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;
            string Out_response = ssl.htmlText;
            string[] response_Value = ParseReportCode(Out_response);  //BBPSParseReportCode
            string ReturnOutpot = response_Value[5];
            string ErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(ReturnOutpot);

            #endregion


            //FinalOutpot == "224"var PaymentValidation = BBPSPaymentAPI.BBPSBillPaymentElectricityValidation(Service_key, MobileNo, Unitno, AccountNo, "", GeoLocation, outletid, CityName, sRandomOTP);

            //string errordesc = PaymentValidation.status;
            ////var PaymentValidation = ElectricityPaymentAPI.ElectricityValidation(agentId, objval.RechargeAmt.ToString(), objval.MobileNo.ToString(), operatorId, objval.CustomerId, option9, outletid.ToString());
            //if (PaymentValidation.statuscode == "TXN")
            if (ReturnOutpot == "0")
            {
                string Final_ErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(ReturnOutpot);
                string[] GetJsonRes = ConvertEncodeStringInJson(Out_response);
                string returnUrl = Server.UrlDecode(GetJsonRes[0]);
                //string returnUrl = Server.UrlDecode(response_Value[8]);  //Old Code
                //var Objsettng = returnUrl.Remove(0, 8);
                var Objsettng = returnUrl;
                JObject json = JObject.Parse(Objsettng);
                var ResponseResult = JObject.Parse(Objsettng);
                dynamic FinalResp = ResponseResult;

                var data = JsonConvert.SerializeObject(FinalResp);
                //return Json(data, JsonRequestBehavior.AllowGet);
                return Json(new { Result = "0", data = data }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //var data = JsonConvert.SerializeObject(PaymentValidation);
                var data = ErrorReturn;
                //return Json(ErrorReturn, JsonRequestBehavior.AllowGet);
                return Json(new { Result = "1", data = ErrorReturn }, JsonRequestBehavior.AllowGet);
            }



            //var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == EmployeeId).OrderBy(c => c.TYPE).ToList();
            ////var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "PREPAID" || x.TYPE == "POSTPAID").OrderBy(c => c.TYPE).ToList();
            //ViewBag.GetElectricityInfo = OperatorList;
            //return Json("");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> PostElectricityBill(ElectricityViewModel objval)
        {
            initpage();
            try
            {
                string SessionNo = DateTime.Parse(DateTime.Now.ToString()).ToString("dddMMMddyyyyHHmmss");
                string keyPath = Server.MapPath("~/Test CERT/myprivatekey.pfx");
                string OutputResponse = string.Empty;
                string FinalOutputRes = string.Empty;
                var db = new DBContext();
                var getWhiteLevelID = await (from mrnt in db.TBL_MASTER_MEMBER
                                             join dist in db.TBL_MASTER_MEMBER on mrnt.INTRODUCER equals dist.MEM_ID
                                             //join super in db.TBL_MASTER_MEMBER on dist.INTRODUCER equals super.MEM_ID
                                             join WBL in db.TBL_MASTER_MEMBER on dist.UNDER_WHITE_LEVEL equals WBL.MEM_ID
                                             where mrnt.MEM_ID == CurrentMerchant.MEM_ID
                                             select new
                                             {
                                                 WHTBalance = WBL.BALANCE
                                             }).FirstOrDefaultAsync();
                if (getWhiteLevelID.WHTBalance >= objval.RechargeAmt)
                {  //var check_walletAmt = db.TBL_ACCOUNTS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                    var check_walletAmt = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).FirstOrDefaultAsync();
                    if (objval.RechargeAmt <= check_walletAmt.BALANCE)
                    {

                        string IpAddress = string.Empty;
                        if (objval.IpAddress != null)
                        {
                            if (objval.IpAddress != "")
                            {
                                IpAddress = objval.IpAddress.Replace("\"", "");
                            }
                            else
                            {
                                IpAddress = "";
                            }
                        }
                        else
                        {
                            IpAddress = "";
                        }
                        string GeoLocation = string.Empty;
                        string geoLoc = string.Empty;
                        //if (objval.geolocation != "" || objval.geolocation != null)
                        if (objval.geolocation != null)
                        {
                            if (objval.geolocation != "")
                            {
                                geoLoc = objval.geolocation.Replace("\"", "");
                            }
                            else
                            {
                                geoLoc = "";
                            }
                        }
                        else
                        {
                            geoLoc = "";
                        }

                        string OperatorName = Request.Form["txtElectricityOperator"];
                        string operatorId = Request.Form["ElectricityoperId"];

                        string REfID = Request.Form["ELECreferenceID"];
                        string REfID_Val = Request.Form["REfferenceElecId"];
                        const string agentId = "74Y104314";
                        long merchantid = CurrentMerchant.MEM_ID;
                        #region Electrity Bill Payment
                        //string AgentIdCyberplateElectricity = "BD01BD11AGT000000001";
                        string AgentIdCyberplateElectricity = System.Configuration.ConfigurationManager.AppSettings["CyberPlateAgentId"];
                        var GetServiceURL = db.TBL_SERVICE_PROVIDERS.Where(x => x.SERVICE_NAME == objval.Service_Name && x.OPTION4 == "CYBERPLAT").FirstOrDefault();

                        var PaymentValidation = CyberPlatBBPSBillUtilityRecharge._strValidationBBPSFetchBillUtility(SessionNo, objval.CustomerId, "4572", objval.RechargeAmt.ToString(), AgentIdCyberplateElectricity, objval.CUSTOMER_FIRST_NAME, objval.CUSTOMER_LAST_NAME, objval.CUSTOMER_EMAIL_ID, objval.MobileNo, "28.5506,77.2692", objval.PIN, "123456");
                        ssl.message = ssl.Sign_With_PFX(PaymentValidation, keyPath, "rahul123");
                        ssl.htmlText = ssl.CallCryptoAPI(ssl.message, GetServiceURL.OPTION1);
                        OutputResponse = "URL:\r\n" + GetServiceURL.OPTION1 + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;
                        string Out_response = ssl.htmlText;
                        string[] response_Value = ParseReportCode(Out_response);
                        string ReturnOutpot = response_Value[5];
                        string ErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(ReturnOutpot);

                        #endregion

                        var Pincode = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.PINCODE).FirstOrDefault();
                        string option9 = objval.geolocation + "|" + Pincode;
                        var outletid = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.OUTLETID).FirstOrDefault();
                        //if (outletid != null)
                        if (ReturnOutpot == "0")
                        {
                            string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
                            CommissionDistributionHelper objChkComm = new CommissionDistributionHelper();
                            bool ComCheck = await objChkComm.DeductAmountFromMerchant(merchantid, "UTILITY", objval.RechargeAmt, objval.RechargeAmt, objval.RechargeAmt, operatorId, "ELECTRICITY", IpAddress, sRandomOTP, objval.CustomerId, "Instantpay", "ELECTRICITY");
                            if (ComCheck == true)
                            {
                                var Recharge = CyberPlatBBPSBillUtilityRecharge._strValidationBBPSFetchBillUtility(SessionNo, objval.CustomerId, "4572", objval.RechargeAmt.ToString(), AgentIdCyberplateElectricity, objval.CUSTOMER_FIRST_NAME, objval.CUSTOMER_LAST_NAME, objval.CUSTOMER_EMAIL_ID, objval.MobileNo, "28.5506,77.2692", objval.PIN, "123456");
                                ssl.message = ssl.Sign_With_PFX(Recharge, keyPath, "rahul123");
                                ssl.htmlText = ssl.CallCryptoAPI(ssl.message, GetServiceURL.OPTION2);
                                FinalOutputRes = "URL:\r\n" + GetServiceURL.OPTION2 + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;
                                string FinalOut_response = ssl.htmlText;
                                string[] Finalresponse_Value = ParseReportCode(FinalOut_response);
                                string FianlReturnOutpot = Finalresponse_Value[5];
                                string FinalErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(FianlReturnOutpot);



                                //var Recharge = BBPSPaymentAPI.BBPSBillPaymentELECTRICITY(operatorId, objval.MobileNo, objval.BillUnit, objval.CustomerId, objval.RechargeAmt.ToString(), geoLoc, outletid, REfID_Val, objval.City, sRandomOTP);
                                //string Electricityres = Convert.ToString(Recharge);
                                //string ErrorDescription = Recharge.ipay_errordesc;
                                //string errorcode = string.IsNullOrEmpty(Recharge.statuscode.Value) ? Recharge.status.Value : Recharge.statuscode.Value;//res.res_code;
                                //if (errorcode == "TXN" || errorcode == "TUP" || errorcode == "ERR")
                                if (FianlReturnOutpot == "0")
                                {
                                    string Final_ErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(FinalErrorReturn);

                                    string[] GetJsonRes = ConvertEncodeStringInJson(FinalOut_response);
                                    string returnUrl = Server.UrlDecode(GetJsonRes[0]);
                                    //string returnUrl = Server.UrlDecode(Finalresponse_Value[8]);  //Old
                                    var Objsettng = returnUrl;
                                    JObject json = JObject.Parse(Objsettng);
                                    var ResponseResult = JObject.Parse(Objsettng);
                                    dynamic Doremit = ResponseResult;
                                    string resstring = Convert.ToString(ResponseResult);
                                    #region Cyberplate Response
                                    var errorcode = Doremit.payment_status;
                                    string status = Doremit.payment_status;
                                    var ipat_id = Doremit.paymentid;
                                    var bbps_ref_no = Doremit.bbps_ref_no;
                                    var ValidationId = Doremit.validationid;
                                    var RechMsgStatus = Doremit.payment_status;
                                    var opr_idVal = Doremit.billerid;
                                    decimal trans_amt = decimal.Parse(Convert.ToString(Doremit.payment_amount.Value));
                                    //decimal Charged_Amt = decimal.Parse(Convert.ToString(Doremit.data.charged_amt.Value));
                                    //decimal Opening_Balance = decimal.Parse(Convert.ToString(Doremit.data.opening_bal.Value));
                                    //DateTime datevalue = Convert.ToDateTime(Doremit.data.datetime.Value);
                                    string outputval = Doremit.payment_status;
                                    #endregion


                                    //var errorcode = Doremit.statuscode;
                                    //string status = Doremit.status;
                                    //var ipat_id = Doremit.data.ipay_id.Value;
                                    //var RechMsgStatus = Doremit.data.status;
                                    //var opr_idVal = Doremit.data.opr_id;
                                    //decimal trans_amt = decimal.Parse(Convert.ToString(Doremit.data.trans_amt.Value));
                                    //decimal Charged_Amt = decimal.Parse(Convert.ToString(Doremit.data.charged_amt.Value));
                                    //decimal Opening_Balance = decimal.Parse(Convert.ToString(Doremit.data.opening_bal.Value));
                                    //DateTime datevalue = Convert.ToDateTime(Doremit.data.datetime.Value);
                                    //string outputval = Doremit.status;

                                    //string status = Recharge.status;
                                    //var ipat_id = Recharge.data.ipay_id.Value;
                                    //var RechMsgStatus = Recharge.data.status;
                                    //var opr_idVal = Recharge.data.opr_id;
                                    //decimal trans_amt = decimal.Parse(Convert.ToString(Recharge.data.trans_amt.Value));
                                    //decimal Charged_Amt = decimal.Parse(Convert.ToString(Recharge.data.charged_amt.Value));
                                    //decimal Opening_Balance = decimal.Parse(Convert.ToString(Recharge.data.opening_bal.Value));
                                    //DateTime datevalue = Convert.ToDateTime(Recharge.data.datetime.Value);
                                    //string outputval = Recharge.status;


                                    DMR_API_Response objElectricityResp = new DMR_API_Response()
                                    {
                                        serviceTax = "0",
                                        clientRefId = ipat_id,
                                        fee = "0",
                                        initiatorId = bbps_ref_no,
                                        accountNumber = objval.CustomerId,
                                        txnStatus = status,
                                        name = opr_idVal,
                                        ifscCode = trans_amt.ToString(),
                                        impsRespCode = trans_amt.ToString(),
                                        impsRespMessage = status,
                                        txnId = ValidationId,
                                        timestamp = RechMsgStatus
                                        //serviceTax = "10",
                                        //clientRefId = "23026041",
                                        //fee = "00",
                                        //initiatorId = "23026041",
                                        //accountNumber = "9903116214",
                                        //txnStatus = "00",
                                        //name = "AIRTEL",
                                        //ifscCode = "10714.41",
                                        //impsRespCode = "10714.41",
                                        //impsRespMessage = "00",
                                        //txnId = "23026041",
                                        //timestamp = "00"
                                    };

                                    CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                    //bool checkComm = await objComm.AllMemberCommissionDistribution(merchantid, "UTILITY", objval.RechargeAmt, Charged_Amt, Opening_Balance, operatorId, "ELECTRICITY Bill", IpAddress, sRandomOTP, objElectricityResp, resstring);
                                    bool checkComm = await objComm.AllMemberCommissionDistribution(merchantid, "UTILITY", objval.RechargeAmt, trans_amt, trans_amt, operatorId, "ELECTRICITY Bill", IpAddress, sRandomOTP, objElectricityResp, resstring);

                                    if (checkComm == true)
                                    {
                                        var ApiResponse = db.TBL_INSTANTPAY_RECHARGE_RESPONSE.FirstOrDefault(x => x.CORELATIONID == sRandomOTP);
                                        ApiResponse.Ipay_Id = ipat_id;
                                        //ApiResponse.AgentId = Recharge.data.agent_id.Value;
                                        //ApiResponse.Opr_Id = Recharge.data.opr_id.Value;
                                        //ApiResponse.AccountNo = Recharge.data.account_no;
                                        //ApiResponse.Sp_Key = Recharge.data.sp_key;
                                        ApiResponse.AgentId = Doremit.data.agent_id.Value;
                                        ApiResponse.Opr_Id = Doremit.data.opr_id.Value;
                                        ApiResponse.AccountNo = Doremit.data.account_no;
                                        ApiResponse.Sp_Key = Doremit.data.sp_key;
                                        ApiResponse.Trans_Amt = decimal.Parse(trans_amt.ToString());
                                        //ApiResponse.Charged_Amt = decimal.Parse(Charged_Amt.ToString());
                                        //ApiResponse.Opening_Balance = decimal.Parse(Opening_Balance.ToString());
                                        ApiResponse.Charged_Amt = decimal.Parse(trans_amt.ToString());
                                        ApiResponse.Opening_Balance = decimal.Parse(trans_amt.ToString());
                                        ApiResponse.DateVal = System.DateTime.Now;
                                        //ApiResponse.Status = Recharge.data.status;
                                        //ApiResponse.Res_Code = Recharge.statuscode;
                                        ApiResponse.Status = Doremit.data.status;
                                        ApiResponse.Res_Code = Doremit.statuscode;
                                        ApiResponse.res_msg = status;
                                        ApiResponse.RechargeType = "ELECTRICITY";
                                        //ApiResponse.RechargeResponse = Electricityres;
                                        ApiResponse.RechargeResponse = resstring;
                                        ApiResponse.ERROR_TYPE = "SUCCESS";
                                        ApiResponse.ISREVERSE = "Yes";
                                        ApiResponse.ISCOMMISSIONDISBURSE = "Yes";
                                        db.Entry(ApiResponse).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                        //return Json(outputval);
                                        return Json("Transaction Successfull");
                                    }
                                    else
                                    {
                                        return Json("Transaction Failed");
                                    }
                                }
                                //else if (errorcode == "ERR")  //FianlReturnOutpot   FinalErrorReturn
                                else if (FianlReturnOutpot == "ERR")
                                {
                                    CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                    string Statusval = objComm.RefundCommissionInWallet(sRandomOTP, "SERVICE");
                                    if (Statusval == "Return Success")
                                    {
                                        return Json("Transaction Failed");
                                    }
                                    else
                                    {
                                        //return Json(ErrorDescription);
                                        return Json(FinalErrorReturn);
                                    }
                                    //return Json(ErrorDescription);
                                    return Json(FinalErrorReturn);
                                }
                                else
                                {
                                    CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                    string Statusval = objComm.RefundCommissionInWallet(sRandomOTP, "SERVICE");
                                    if (Statusval == "Return Success")
                                    {
                                        return Json("Transaction Failed");
                                    }
                                    else
                                    {
                                        //return Json(ErrorDescription);
                                        return Json(FinalErrorReturn);
                                    }
                                    //return Json(ErrorDescription);
                                    return Json(FinalErrorReturn);
                                }
                            }
                            else
                            {
                                return Json("Transaction Failed");
                            }
                        }
                        else
                        {
                            //string msgval = "Please generate outlet id.. ";
                            string msgval = ErrorReturn;
                            return Json(msgval);
                        }
                    }
                    else
                    {
                        var msg = "Can't procceed with transaction.You don't have sufficient balance.";
                        return Json(msg);
                    }
                }
                else
                {
                    var msg = "Insufficient balance in master wallet.";
                    return Json(msg);
                }
            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return Json(msg);
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- PostElectricityBill(POST) Line No:- 613", ex);
                throw ex;
            }
        }


        #endregion

        #region Gass Bill Payment
        public ActionResult GasBillPayment()
        {
            initpage();
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> AutoGasBillService(string prefix)
        {
            try
            {
                var db = new DBContext();
                var OperatorValue = await (from oper in db.TBL_SERVICE_PROVIDERS
                                           where oper.SERVICE_NAME.StartsWith(prefix) && oper.TYPE == "GAS" && oper.OPTION4 == "CYBERPLAT"
                                           select new
                                           {
                                               label = oper.SERVICE_NAME,
                                               val = oper.SERVICE_KEY,
                                               image = oper.IMAGE
                                           }).ToListAsync();

                return Json(OperatorValue);
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- AutoGasBillService(POST) Line No:- 646", ex);
                throw ex;
            }
        }

        //[HttpPost]
        //public async Task<JsonResult> GetGasBillInformation(string AccountNo, string MobileNo, string GeoLocation, string ServiceKey)
        //{
        //    initpage();
        //    var db = new DBContext();

        //    string OperatorName = Request.Form["txtGassServiceOperator"];
        //    string operatorId = Request.Form["GassServiceOperId"];
        //    string Service_key = ServiceKey;
        //    const string agentId = "74Y104314";
        //    long merchantid = CurrentMerchant.MEM_ID;
        //    var Pincode = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.PINCODE).FirstOrDefault();
        //    //string option9 = objval.geolocation + "|" + Pincode;
        //    var outletid = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.OUTLETID).FirstOrDefault();
        //    string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
        //    var PaymentValidation = BBPSPaymentAPI.BBPSBillPaymentElectricityValidation(Service_key, MobileNo, "", AccountNo, "", GeoLocation, outletid, "", sRandomOTP);
        //    string errordesc = PaymentValidation.status;

        //    //var PaymentValidation = ElectricityPaymentAPI.ElectricityValidation(agentId, objval.RechargeAmt.ToString(), objval.MobileNo.ToString(), operatorId, objval.CustomerId, option9, outletid.ToString());
        //    if (PaymentValidation.statuscode == "TXN")
        //    {
        //        var data = JsonConvert.SerializeObject(PaymentValidation);
        //        return Json(data, JsonRequestBehavior.AllowGet);

        //    }
        //    else
        //    {
        //        var data = JsonConvert.SerializeObject(PaymentValidation);
        //        return Json(data, JsonRequestBehavior.AllowGet);
        //    }

        //    //var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == EmployeeId).OrderBy(c => c.TYPE).ToList();
        //    ////var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "PREPAID" || x.TYPE == "POSTPAID").OrderBy(c => c.TYPE).ToList();
        //    //ViewBag.GetElectricityInfo = OperatorList;
        //    //return Json("");
        //}
        [HttpPost]
        public async Task<JsonResult> GetGasBillInformation(string AccountNo, string MobileNo, string GeoLocation, string ServiceKey, string GasBillAmt, string FName, string LName, string EmailId, string PinNo)
        {
            initpage();
            string SessionNo = DateTime.Parse(DateTime.Now.ToString()).ToString("dddMMMddyyyyHHmmss");
            var db = new DBContext();
            string keyPath = Server.MapPath("~/Test CERT/myprivatekey.pfx");
            string OutputResponse = string.Empty;
            string OperatorName = Request.Form["GassServiceOperId"];
            string operatorId = Request.Form["GassServiceOperId"];
            string Service_key = ServiceKey;
            const string agentId = "74Y104314";
            long merchantid = CurrentMerchant.MEM_ID;
            //var Pincode = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.PINCODE).FirstOrDefault();            
            //var outletid = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.OUTLETID).FirstOrDefault();
            string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
            //var PaymentValidation = BBPSPaymentAPI.BBPSBillPaymentElectricityValidation(Service_key, MobileNo, "", AccountNo, GasBillAmt, GeoLocation, outletid, "", sRandomOTP);
            //string errordesc = PaymentValidation.status;


            #region Gss Bill Payment
            string AmountAllAmt = "4.32";

            var ApiUrl = db.TBL_SERVICE_PROVIDERS.Where(x => x.SERVICE_NAME == Service_key && x.OPTION4 == "CYBERPLAT").FirstOrDefault();
            //string AgentIdCyberplate = "10p9714";
            //string AgentIdCyberplate = "BD01BD11AGT000000001";
            string AgentIdCyberplate = System.Configuration.ConfigurationManager.AppSettings["CyberPlateAgentId"];
            //string GetServiceURL = "https://in.cyberplat.com/cgi-bin/bu/bu_pay_check.cgi/340";
            string GetServiceURL = ApiUrl.OPTION1;
            var PaymentValidationBillFetch = CyberPlatBBPSBillUtilityRecharge._strValidationBBPSFetchBillUtility(SessionNo, AccountNo, "4572", "1.00", AgentIdCyberplate, FName, LName, EmailId, MobileNo, "28.5506,77.2692", "492013", "123456");
            ssl.message = ssl.Sign_With_PFX(PaymentValidationBillFetch, keyPath, "rahul123");
            ssl.htmlText = ssl.CallCryptoAPI(ssl.message, GetServiceURL);
            OutputResponse = "URL:\r\n" + GetServiceURL + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;
            string Out_response = ssl.htmlText;
            string[] response_Value = ParseReportCode(Out_response);  //BBPSParseReportCode
            string ReturnOutpot = response_Value[5];
            string ErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(ReturnOutpot);

            #endregion

            //if (PaymentValidation.statuscode == "TXN")
            if (ReturnOutpot == "0")
            {
                string Final_ErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(ReturnOutpot);
                string[] GetJsonRes = ConvertEncodeStringInJson(Out_response);
                string returnUrl = Server.UrlDecode(GetJsonRes[0]);
                //string returnUrl = Server.UrlDecode(response_Value[8]);  //Old Code
                //var Objsettng = returnUrl.Remove(0, 8);
                var Objsettng = returnUrl;
                JObject json = JObject.Parse(Objsettng);
                var ResponseResult = JObject.Parse(Objsettng);
                dynamic FinalResp = ResponseResult;
                var data = JsonConvert.SerializeObject(FinalResp);                
                return Json(new { Result = "0", data = data }, JsonRequestBehavior.AllowGet);
                //var data = JsonConvert.SerializeObject(PaymentValidation);
                //return Json(data, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var data = ErrorReturn;                
                return Json(new { Result = "1", data = ErrorReturn }, JsonRequestBehavior.AllowGet);
                //var data = JsonConvert.SerializeObject(PaymentValidation);
                //return Json(data, JsonRequestBehavior.AllowGet);
            }



            //var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == EmployeeId).OrderBy(c => c.TYPE).ToList();
            ////var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "PREPAID" || x.TYPE == "POSTPAID").OrderBy(c => c.TYPE).ToList();
            //ViewBag.GetElectricityInfo = OperatorList;
            //return Json("");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> PostGasBillPayment(GasBillPaymentViewModel objval)
        {
            initpage();
            try
            {
                string SessionNo = DateTime.Parse(DateTime.Now.ToString()).ToString("dddMMMddyyyyHHmmss");
                var db = new DBContext();
                string keyPath = Server.MapPath("~/Test CERT/myprivatekey.pfx");
                string OutputResponse = string.Empty;
                string FinalOutputRes = string.Empty;
                var getWhiteLevelID = await (from mrnt in db.TBL_MASTER_MEMBER
                                             join dist in db.TBL_MASTER_MEMBER on mrnt.INTRODUCER equals dist.MEM_ID
                                             //join super in db.TBL_MASTER_MEMBER on dist.INTRODUCER equals super.MEM_ID
                                             join WBL in db.TBL_MASTER_MEMBER on dist.UNDER_WHITE_LEVEL equals WBL.MEM_ID
                                             where mrnt.MEM_ID == CurrentMerchant.MEM_ID
                                             select new
                                             {
                                                 WHTBalance = WBL.BALANCE
                                             }).FirstOrDefaultAsync();
                if (getWhiteLevelID.WHTBalance >= objval.RechargeAmount)
                { //var check_walletAmt = db.TBL_ACCOUNTS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                    var check_walletAmt = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).FirstOrDefaultAsync();

                    if (objval.RechargeAmount <= check_walletAmt.BALANCE)
                    {
                        string IpAddress = string.Empty;
                        if (objval.IpAddress != null)
                        {
                            IpAddress = objval.IpAddress.Replace("\"", "");
                        }
                        else
                        {
                            IpAddress = "";
                        }
                        string GeoLocation = string.Empty;
                        string geoLoc = string.Empty;
                        if (objval.geolocation != "" || objval.geolocation != null)
                        {
                            geoLoc = objval.geolocation.Replace("\"", "");
                        }
                        else
                        {
                            geoLoc = "";
                        }
                        string OperatorName = Request.Form["txtGassServiceOperator"];
                        string operatorId = Request.Form["GassServiceOperId"];
                        const string agentId = "74Y104314";
                        string REfID = Request.Form["Gas_referenceID"];
                        string REfID_Val = Request.Form["GassReferenceId"];

                        long merchantid = CurrentMerchant.MEM_ID;
                        //var Pincode = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.PINCODE).FirstOrDefault();
                        //string option9 = objval.geolocation + "|" + Pincode;
                        //var outletid = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.OUTLETID).FirstOrDefault();
                        //if (outletid != null)
                        //{

                        #region Gss Bill Payment
                        string AmountAllAmt = "4.32";

                        var ApiUrl = db.TBL_SERVICE_PROVIDERS.Where(x => x.SERVICE_NAME == objval.Service_Name && x.OPTION4 == "CYBERPLAT").FirstOrDefault();
                        //string AgentIdCyberplate = "10p9714";
                        //string AgentIdCyberplate = "BD01BD11AGT000000001";
                        string AgentIdCyberplate = System.Configuration.ConfigurationManager.AppSettings["CyberPlateAgentId"];
                        //string GetServiceURL = "https://in.cyberplat.com/cgi-bin/bu/bu_pay_check.cgi/340";
                        string GetServiceURL = ApiUrl.OPTION1;
                        var PaymentValidationBillFetch = CyberPlatBBPSBillUtilityRecharge._strValidationBBPSFetchBillUtility(SessionNo, objval.CustomerID, "4572", "1.00", AgentIdCyberplate, objval.CUSTOMER_FIRST_NAME, objval.CUSTOMER_LAST_NAME, objval.CUSTOMER_EMAIL_ID, objval.ContactNo, "28.5506,77.2692", objval.PIN, "123456");
                        ssl.message = ssl.Sign_With_PFX(PaymentValidationBillFetch, keyPath, "rahul123");
                        ssl.htmlText = ssl.CallCryptoAPI(ssl.message, GetServiceURL);
                        OutputResponse = "URL:\r\n" + GetServiceURL + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;
                        string Out_response = ssl.htmlText;
                        string[] response_Value = ParseReportCode(Out_response);  //BBPSParseReportCode
                        string ReturnOutpot = response_Value[5];
                        string ErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(ReturnOutpot);
                        #endregion
                        if (ReturnOutpot == "0")
                        {
                            string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
                            CommissionDistributionHelper objChkComm = new CommissionDistributionHelper();
                            bool ComCheck = await objChkComm.DeductAmountFromMerchant(merchantid, "UTILITY", objval.RechargeAmount, objval.RechargeAmount, objval.RechargeAmount, operatorId, "GAS", IpAddress, sRandomOTP, objval.CustomerID, "Instantpay", "GAS");
                            if (ComCheck == true)
                            {
                                //var Recharge = BBPSPaymentAPI.BBPSBillPaymentGASBILL(operatorId, objval.ContactNo, objval.CustomerID, objval.RechargeAmount.ToString(), geoLoc, outletid, objval.GassReferenceNo);                                
                                //string GASRespo = Convert.ToString(Recharge);                                
                                //string ErrorDescription = Recharge.ipay_errordesc;
                                //string errorcodeValue = Recharge.statuscode;
                                //string errorcode = string.IsNullOrEmpty(errorcodeValue) ? ErrorDescription : errorcodeValue;
                                var Recharge = CyberPlatBBPSBillUtilityRecharge._strValidationBBPSFetchBillUtility(SessionNo, objval.CustomerID, "4572", "1.00", AgentIdCyberplate, objval.CUSTOMER_FIRST_NAME, objval.CUSTOMER_LAST_NAME, objval.CUSTOMER_EMAIL_ID, objval.ContactNo, "28.5506,77.2692", objval.PIN, "123456");
                                ssl.message = ssl.Sign_With_PFX(Recharge, keyPath, "rahul123");
                                ssl.htmlText = ssl.CallCryptoAPI(ssl.message, ApiUrl.OPTION2);
                                FinalOutputRes = "URL:\r\n" + ApiUrl.OPTION2 + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;
                                string FinalOut_response = ssl.htmlText;
                                string[] Finalresponse_Value = ParseReportCode(FinalOut_response);
                                string FianlReturnOutpot = Finalresponse_Value[5];
                                string FinalErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(FianlReturnOutpot);



                                //if (errorcode == "TXN" || errorcode == "TUP")
                                if (FianlReturnOutpot == "0")
                                {
                                    string Final_ErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(FinalErrorReturn);

                                    string[] GetJsonRes = ConvertEncodeStringInJson(FinalOut_response);
                                    string returnUrl = Server.UrlDecode(GetJsonRes[0]);
                                    //string returnUrl = Server.UrlDecode(Finalresponse_Value[8]);  //Old
                                    var Objsettng = returnUrl;
                                    JObject json = JObject.Parse(Objsettng);
                                    var ResponseResult = JObject.Parse(Objsettng);
                                    dynamic Doremit = ResponseResult;
                                    string GASRespo = Convert.ToString(ResponseResult);
                                    #region Cyberplate Response
                                    var errorcode = Doremit.payment_status;
                                    string status = Doremit.payment_status;
                                    var ipat_id = Doremit.paymentid;
                                    var customerid = Doremit.customerid;
                                    var bbps_ref_no = Doremit.bbps_ref_no;
                                    var ValidationId = Doremit.validationid;
                                    var RechMsgStatus = Doremit.payment_status;
                                    var opr_idVal = Doremit.billerid;
                                    decimal trans_amt = decimal.Parse(Convert.ToString(Doremit.payment_amount.Value));
                                    //decimal Charged_Amt = decimal.Parse(Convert.ToString(Doremit.data.charged_amt.Value));
                                    //decimal Opening_Balance = decimal.Parse(Convert.ToString(Doremit.data.opening_bal.Value));
                                    //DateTime datevalue = Convert.ToDateTime(Doremit.data.datetime.Value);
                                    string outputval = Doremit.payment_status;
                                    #endregion

                                    //string status = Recharge.status;
                                    //var ipat_id = Recharge.data.ipay_id.Value;
                                    //var opr_idVal = Recharge.data.opr_id;
                                    //var RechMsgStatus = Recharge.data.status;
                                    //decimal trans_amt = decimal.Parse(Convert.ToString(Recharge.data.trans_amt.Value));
                                    //decimal Charged_Amt = decimal.Parse(Convert.ToString(Recharge.data.charged_amt.Value));
                                    //decimal Opening_Balance = decimal.Parse(Convert.ToString(Recharge.data.opening_bal.Value));
                                    //DateTime datevalue = Convert.ToDateTime(Recharge.data.datetime.Value);
                                    //string agentId_Value = Recharge.data.agent_id;
                                    //string Operator_ID = Recharge.data.opr_id;
                                    //string statusCode = Recharge.statuscode;
                                    //string outputval = Recharge.status;
                                    DMR_API_Response ObjGassBillRes = new DMR_API_Response()
                                    {
                                        serviceTax = "0",
                                        clientRefId = customerid,
                                        fee = "0",
                                        initiatorId = ipat_id,
                                        accountNumber = objval.CustomerID,
                                        txnStatus = status,
                                        name = opr_idVal,
                                        ifscCode = trans_amt.ToString(),
                                        impsRespCode = trans_amt.ToString(),
                                        impsRespMessage = status,
                                        txnId = ipat_id,
                                        timestamp = RechMsgStatus
                                        //serviceTax = "10",
                                        //clientRefId = "23026041",
                                        //fee = "00",
                                        //initiatorId = "23026041",
                                        //accountNumber = "9903116214",
                                        //txnStatus = "00",
                                        //name = "AIRTEL",
                                        //ifscCode = "10714.41",
                                        //impsRespCode = "10714.41",
                                        //impsRespMessage = "00",
                                        //txnId = "23026041",
                                        //timestamp = "00"
                                    };

                                    CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                    bool checkComm = await objComm.AllMemberCommissionDistribution(merchantid, "UTILITY", objval.RechargeAmount, trans_amt, trans_amt, operatorId, "GAS Bill", IpAddress, sRandomOTP, ObjGassBillRes, GASRespo);
                                    if (checkComm == true)
                                    {
                                        var ApiResponse = db.TBL_INSTANTPAY_RECHARGE_RESPONSE.FirstOrDefault(x => x.CORELATIONID == sRandomOTP);
                                        ApiResponse.Ipay_Id = ipat_id;
                                        //ApiResponse.AgentId = Recharge.data.agent_id.Value;
                                        //ApiResponse.Opr_Id = Recharge.data.opr_id.Value;
                                        //ApiResponse.AccountNo = Recharge.data.account_no.Value;
                                        //ApiResponse.Sp_Key = Recharge.data.sp_key.Value;
                                        ApiResponse.AgentId = Doremit.data.agent_id.Value;
                                        ApiResponse.Opr_Id = Doremit.data.opr_id.Value;
                                        ApiResponse.AccountNo = Doremit.data.account_no;
                                        ApiResponse.Sp_Key = Doremit.data.sp_key;
                                        ApiResponse.Trans_Amt = decimal.Parse(trans_amt.ToString());
                                        ApiResponse.Charged_Amt = decimal.Parse(trans_amt.ToString());
                                        ApiResponse.Opening_Balance = decimal.Parse(trans_amt.ToString());
                                        ApiResponse.DateVal = System.DateTime.Now;
                                        //ApiResponse.Status = Recharge.data.status;
                                        //ApiResponse.Res_Code = Recharge.statuscode;
                                        ApiResponse.Status = Doremit.data.status;
                                        ApiResponse.Res_Code = Doremit.statuscode;
                                        ApiResponse.res_msg = status;
                                        ApiResponse.RechargeType = "GAS";
                                        ApiResponse.RechargeResponse = GASRespo;
                                        ApiResponse.ERROR_TYPE = "SUCCESS";
                                        ApiResponse.ISREVERSE = "Yes";
                                        ApiResponse.ISCOMMISSIONDISBURSE = "Yes";
                                        db.Entry(ApiResponse).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                        //return Json(outputval);
                                        return Json("Transaction Successfull");
                                    }
                                    else
                                    {
                                        return Json("Transaction Failed");
                                    }
                                }
                                else
                                {
                                    CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                    string Statusval = objComm.RefundCommissionInWallet(sRandomOTP, "SERVICE");
                                    if (Statusval == "Return Success")
                                    {
                                        return Json("Transaction Failed");
                                    }
                                    else
                                    {
                                        return Json(FinalErrorReturn);
                                    }
                                    return Json(FinalErrorReturn);
                                }
                            }
                            else
                            {
                                return Json("Transaction Failed");
                            }
                        }
                        else
                        {
                            string msgval = ErrorReturn;
                            return Json(msgval);
                        }
                    }
                    else
                    {
                        var msg = "Can't procceed with transaction.You don't have sufficient balance.";
                        return Json(msg);
                    }
                }
                else
                {
                    var msg = "Insufficient balance in master wallet.";
                    return Json(msg);
                }  
            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return Json(msg);
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- PostGasBillPayment(POST) Line No:- 682", ex);
                throw ex;
            }
        }
        #endregion

        #region Insurance Payment
        public ActionResult InsurancePayment()
        {
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> AutoInsuranceService(string prefix)
        {
            try
            {
                var db = new DBContext();
                var OperatorValue = await (from oper in db.TBL_SERVICE_PROVIDERS
                                           where oper.SERVICE_NAME.StartsWith(prefix) && oper.TYPE == "INSURANCE" && oper.OPTION4 == "CYBERPLAT"
                                           select new
                                           {
                                               label = oper.SERVICE_NAME,
                                               val = oper.SERVICE_KEY
                                           }).ToListAsync();

                return Json(OperatorValue);
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- AutoInsuranceService(POST) Line No:- 713", ex);
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult PostInsurancerecharge(InsuranceViewModel objval)
        {
            try
            {
                string OperatorName = Request.Form["txtInsuranceServiceOperator"];
                string operatorId = Request.Form["InsuranceOperId"];
                const string agentId = "2";
                var PaymentValidation = PaymentAPI.Validation(agentId, objval.PolicyAmount.ToString(), operatorId, objval.PolicyNo,"","");
                if (PaymentValidation == "TXN")
                {
                    //var Recharge = PaymentAPI.Payment(agentId, objval.RechargeAmt.ToString(), "ATP", objval.ContactNo);
                    var Recharge = PaymentAPI.Payment(agentId, objval.PolicyAmount.ToString(), operatorId, objval.PolicyNo, "", "");
                    if (Recharge == "TXN")
                    {
                        var ipat_id = Recharge;
                        Session["msgCheck"] = "Transaction Successful";
                    }
                    else
                    {
                        Session["msgCheck"] = Recharge;
                    }
                    return Json(Recharge);
                }
                else
                {
                    return Json(PaymentValidation);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- PostInsurancerecharge(POST) Line No:- 750", ex);
                throw ex;
            }

        }
        #endregion

        #region Recharge Details list
        public ActionResult RechargeTransactionList()
        {
            initpage();
            if (Session["MerchantUserId"] != null)
            {
                Session["MerchantDMRId"] = null;
                Session.Remove("MerchantDMRId");
                return View();
            }
            else
            {
                Session["MerchantUserId"] = null;
                Session["MerchantUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("MerchantUserId");
                Session.Remove("MerchantUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
            //return View();
        }
        public PartialViewResult PartialRechargeInfoList()
        {
            var db = new DBContext();
            //var Mem_ID = long.Parse(CurrentMerchant.MEM_ID);
            //var RechargeTransaction = db.TBL_MULTILINK_API_RESPONSE.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).ToList();
            var RechargeTransaction = db.TBL_INSTANTPAY_RECHARGE_RESPONSE.Where(x => x.Mem_ID == CurrentMerchant.MEM_ID).ToList();
            return PartialView("PartialRechargeInfoList", RechargeTransaction);
            //return PartialView(DisplayRechargeransaction());
        }
        private IGrid<TBL_INSTANTPAY_RECHARGE_RESPONSE> DisplayRechargeransaction()
        {
            try
            {
                var db = new DBContext();

                var Mem_ID = long.Parse(Session["MerchantUserId"].ToString());
                var RechargeTransaction = db.TBL_INSTANTPAY_RECHARGE_RESPONSE.Where(x => x.Mem_ID == Mem_ID).ToList();

                ////var bankdetails = db.TBL_SETTINGS_BANK_DETAILS.Where(x => x.ISDELETED == 0  && x.MEM_ID==MemberCurrentUser.MEM_ID).ToList();
                //var bankdetails = db.TBL_REMITTER_BENEFICIARY_INFO.Where(x => x.IsActive == 0 && x.RemitterID == remitterid).ToList();

                IGrid<TBL_INSTANTPAY_RECHARGE_RESPONSE> grid = new Grid<TBL_INSTANTPAY_RECHARGE_RESPONSE>(RechargeTransaction);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.Ipay_Id).Titled("IPAY_ID").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.AgentId).Titled("AGENT_ID").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.Opr_Id).Titled("OPR_ID").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.AccountNo).Titled("ACCOUNT_NO.").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.Sp_Key).Titled("SERVICE PROVIDER KEY").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.Trans_Amt).Titled("TRANS_AMT").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.Charged_Amt).Titled("CHARGED_AMT").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.Opening_Balance).Titled("CLOSING BAL.").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.DateVal).Titled("RECHARGE DATE").Filterable(true).Sortable(true).MultiFilterable(true);
                grid.Columns.Add(model => model.RechargeType).Titled("RECHARGE TYPE").Filterable(true).Sortable(true).MultiFilterable(true);
                //grid.Columns.Add(model => model.Status).Titled("STATUS").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.ID).Titled("STATUS").Encoded(false).Filterable(false).Sortable(false)
                    .RenderedAs(model => "<label class='label " + (model.Status == "SUCCESS" ? "label-success" : model.Status == "FAILED" ? "label-danger" : "label-info") + "'> " + model.Status + " </label>");
                grid.Columns.Add(model => model.res_msg).Titled("RECHARGE STATUS").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.RechargeType).Titled("RECHARGE TYPE").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.ID).Titled("").Encoded(false).Filterable(false).Sortable(false)
                //    .RenderedAs(model => "<a href='javascript:void(0)' class='btn btn-denger btn-xs' onclick='DeActivateBeneficiary(" + model.ID + ");return 0;'>DELETE</a>");
                grid.Pager = new GridPager<TBL_INSTANTPAY_RECHARGE_RESPONSE>(grid);
                grid.Processors.Add(grid.Pager);
                grid.Pager.RowsPerPage = 10;

                //foreach (IGridColumn column in grid.Columns)
                //{
                //    column.Filter.IsEnabled = true;
                //    column.Sort.IsEnabled = true;
                //}

                return grid;


            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- DisplayRechargeransaction(POST) Line No:- 823", ex);
                throw ex;
            }

        }
        #endregion


        #region water Bill Payment
        public ActionResult WaterSupplyPayment()
        {
            initpage();
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> AutoWaterBillService(string prefix)
        {
            try
            {
                var db = new DBContext();
                var OperatorValue = await (from oper in db.TBL_SERVICE_PROVIDERS
                                           where oper.SERVICE_NAME.StartsWith(prefix) && oper.TYPE == "WATER" && oper.OPTION4 == "CYBERPLAT"
                                           select new
                                           {
                                               label = oper.SERVICE_NAME,
                                               val = oper.SERVICE_KEY,
                                               image = oper.IMAGE
                                           }).ToListAsync();

                return Json(OperatorValue);
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- AutoElectricityBillService(POST) Line No:- 576", ex);
                throw ex;
            }
        }

        //[HttpPost]
        //public async Task<JsonResult> GetWaterBillInformation(string AccountNo, string MobileNo, string GeoLocation, string ServiceKey)
        //{
        //    initpage();
        //    var db = new DBContext();

        //    string OperatorName = Request.Form["txtWaterSupplyOperator"];
        //    string operatorId = Request.Form["WaterSupplyoperId"];
        //    string Service_key = ServiceKey;
        //    const string agentId = "74Y104314";
        //    long merchantid = CurrentMerchant.MEM_ID;
        //    var Pincode = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.PINCODE).FirstOrDefault();
        //    //string option9 = objval.geolocation + "|" + Pincode;
        //    var outletid = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.OUTLETID).FirstOrDefault();
        //    string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
        //    var PaymentValidation = BBPSPaymentAPI.BBPSBillPaymentWaterBillValidation(Service_key, MobileNo, "", AccountNo, "11", GeoLocation, outletid, sRandomOTP);
        //    string errordesc = PaymentValidation.status;

        //    //var PaymentValidation = ElectricityPaymentAPI.ElectricityValidation(agentId, objval.RechargeAmt.ToString(), objval.MobileNo.ToString(), operatorId, objval.CustomerId, option9, outletid.ToString());
        //    if (PaymentValidation.statuscode == "TXN")
        //    {
        //        var data = JsonConvert.SerializeObject(PaymentValidation);
        //        return Json(data, JsonRequestBehavior.AllowGet);

        //    }
        //    else
        //    {
        //        var data = JsonConvert.SerializeObject(PaymentValidation);
        //        return Json(data, JsonRequestBehavior.AllowGet);
        //    }

        //    //var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == EmployeeId).OrderBy(c => c.TYPE).ToList();
        //    ////var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "PREPAID" || x.TYPE == "POSTPAID").OrderBy(c => c.TYPE).ToList();
        //    //ViewBag.GetElectricityInfo = OperatorList;
        //    //return Json("");
        //}
        [HttpPost]
        public async Task<JsonResult> GetWaterBillInformation(string AccountNo, string MobileNo, string GeoLocation, string ServiceKey, string WaterBillAmt, string FName, string LName, string EmailId, string PinNo)
        {
            initpage();
            string SessionNo = DateTime.Parse(DateTime.Now.ToString()).ToString("dddMMMddyyyyHHmmss");
            var db = new DBContext();
            string keyPath = Server.MapPath("~/Test CERT/myprivatekey.pfx");
            string OutputResponse = string.Empty;
            //string OperatorName = Request.Form["hfLandlineOperator"];
            //string operatorId = Request.Form["LandlineOperatorId"];
            string OperatorName = Request.Form["txtWaterSupplyOperator"];
            string operatorId = Request.Form["WaterSupplyoperId"];
            string Service_key = ServiceKey;
            const string agentId = "74Y104314";
            //long merchantid = CurrentMerchant.MEM_ID;
            //var Pincode = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.PINCODE).FirstOrDefault();
            ////string option9 = objval.geolocation + "|" + Pincode;
            //var outletid = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.OUTLETID).FirstOrDefault();
            //string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
            //var PaymentValidation = BBPSPaymentAPI.BBPSBillPaymentElectricityValidation(Service_key, MobileNo, "", AccountNo, WaterBillAmt, GeoLocation, outletid, "", sRandomOTP);

            //string errordesc = PaymentValidation.status;
            ////var PaymentValidation = ElectricityPaymentAPI.ElectricityValidation(agentId, objval.RechargeAmt.ToString(), objval.MobileNo.ToString(), operatorId, objval.CustomerId, option9, outletid.ToString());
            //if (PaymentValidation.statuscode == "TXN")
            //{
            //    var data = JsonConvert.SerializeObject(PaymentValidation);
            //    return Json(data, JsonRequestBehavior.AllowGet);

            //}
            //else
            //{
            //    var data = JsonConvert.SerializeObject(PaymentValidation);
            //    return Json(data, JsonRequestBehavior.AllowGet);
            //}
            #region Water Bill Payment
            string AmountAllAmt = "4.32";
            var ApiUrl = db.TBL_SERVICE_PROVIDERS.Where(x => x.SERVICE_NAME == Service_key && x.OPTION4 == "CYBERPLAT").FirstOrDefault();
            //string AgentIdCyberplate = "10p9714";
            //string AgentIdCyberplate = "BD01BD11AGT000000001";
            string AgentIdCyberplate = System.Configuration.ConfigurationManager.AppSettings["CyberPlateAgentId"];
            //string GetServiceURL = "https://in.cyberplat.com/cgi-bin/bu/bu_pay_check.cgi/340";
            string GetServiceURL = ApiUrl.OPTION1;
            var PaymentValidationBillFetch = CyberPlatBBPSBillUtilityRecharge._strValidationBBPSFetchBillUtility(SessionNo, AccountNo, "4572", "1.00", AgentIdCyberplate, FName, LName, EmailId, MobileNo, "28.5506,77.2692", "492013", "123456");
            ssl.message = ssl.Sign_With_PFX(PaymentValidationBillFetch, keyPath, "rahul123");
            ssl.htmlText = ssl.CallCryptoAPI(ssl.message, GetServiceURL);
            OutputResponse = "URL:\r\n" + GetServiceURL + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;
            string Out_response = ssl.htmlText;
            string[] response_Value = ParseReportCode(Out_response);  //BBPSParseReportCode
            string ReturnOutpot = response_Value[5];
            string ErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(ReturnOutpot);

            #endregion

            //if (PaymentValidation.statuscode == "TXN")
            if (ReturnOutpot == "0")
            {
                string Final_ErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(ReturnOutpot);
                string[] GetJsonRes = ConvertEncodeStringInJson(Out_response);
                string returnUrl = Server.UrlDecode(GetJsonRes[0]);
                //string returnUrl = Server.UrlDecode(response_Value[8]);  //Old Code
                //var Objsettng = returnUrl.Remove(0, 8);
                var Objsettng = returnUrl;
                JObject json = JObject.Parse(Objsettng);
                var ResponseResult = JObject.Parse(Objsettng);
                dynamic FinalResp = ResponseResult;
                var data = JsonConvert.SerializeObject(FinalResp);
                return Json(new { Result = "0", data = data }, JsonRequestBehavior.AllowGet);
                
            }
            else
            {
                var data = ErrorReturn;
                return Json(new { Result = "1", data = ErrorReturn }, JsonRequestBehavior.AllowGet);
                
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> PostWaterSypplyBillPayment(WaterSupplyPaymentModel objval)
        {
            initpage();
            try
            {
                string SessionNo = DateTime.Parse(DateTime.Now.ToString()).ToString("dddMMMddyyyyHHmmss");
                var db = new DBContext();
                string keyPath = Server.MapPath("~/Test CERT/myprivatekey.pfx");
                string OutputResponse = string.Empty;
                string FinalOutputRes = string.Empty;
                var getWhiteLevelID = await (from mrnt in db.TBL_MASTER_MEMBER
                                             join dist in db.TBL_MASTER_MEMBER on mrnt.INTRODUCER equals dist.MEM_ID
                                             //join super in db.TBL_MASTER_MEMBER on dist.INTRODUCER equals super.MEM_ID
                                             join WBL in db.TBL_MASTER_MEMBER on dist.UNDER_WHITE_LEVEL equals WBL.MEM_ID
                                             where mrnt.MEM_ID == CurrentMerchant.MEM_ID
                                             select new
                                             {
                                                 WHTBalance = WBL.BALANCE
                                             }).FirstOrDefaultAsync();
                if (getWhiteLevelID.WHTBalance >= objval.RechargeAmt)
                {
                    //var check_walletAmt = db.TBL_ACCOUNTS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                    var check_walletAmt = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).FirstOrDefaultAsync();

                    if (objval.RechargeAmt <= check_walletAmt.BALANCE)
                    {
                        string IpAddress = string.Empty;
                        if (objval.IpAddress != null)
                        {
                            IpAddress = objval.IpAddress.Replace("\"", "");
                        }
                        else
                        {
                            IpAddress = "";
                        }
                        string GeoLocation = string.Empty;
                        string geoLoc = string.Empty;
                        if (objval.geolocation != "" || objval.geolocation != null)
                        {
                            geoLoc = objval.geolocation.Replace("\"", "");
                        }
                        else
                        {
                            geoLoc = "";
                        }

                        string OperatorName = Request.Form["txtWaterSupplyOperator"];
                        string operatorId = Request.Form["WaterSupplyoperId"];
                        const string agentId = "74Y104314";
                        string Ref_ID = Request.Form["Water_referenceID"];
                        string Ref_IDValue = Request.Form["Water_ref_Name"];
                        long merchantid = CurrentMerchant.MEM_ID;                        
                        //var Pincode = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.PINCODE).FirstOrDefault();
                        //string option9 = objval.geolocation + "|" + Pincode;
                        //var outletid = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.OUTLETID).FirstOrDefault();
                        //if (outletid != null)
                        //{
                            string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
                        #region Wate Brill Payment
                        string AmountAllAmt = "4.32";

                        var ApiUrl = db.TBL_SERVICE_PROVIDERS.Where(x => x.SERVICE_NAME == objval.Service_Name && x.OPTION4 == "CYBERPLAT").FirstOrDefault();
                        //string AgentIdCyberplate = "10p9714";
                        //string AgentIdCyberplate = "BD01BD11AGT000000001";
                        string AgentIdCyberplate = System.Configuration.ConfigurationManager.AppSettings["CyberPlateAgentId"];
                        //string GetServiceURL = "https://in.cyberplat.com/cgi-bin/bu/bu_pay_check.cgi/340";
                        string GetServiceURL = ApiUrl.OPTION1;
                        var PaymentValidationBillFetch = CyberPlatBBPSBillUtilityRecharge._strValidationBBPSFetchBillUtility(SessionNo, objval.CustomerId, "4572", "1.00", AgentIdCyberplate, objval.CUSTOMER_FIRST_NAME, objval.CUSTOMER_LAST_NAME, objval.CUSTOMER_EMAIL_ID, objval.MobileNo, "28.5506,77.2692", objval.PIN, "123456");
                        ssl.message = ssl.Sign_With_PFX(PaymentValidationBillFetch, keyPath, "rahul123");
                        ssl.htmlText = ssl.CallCryptoAPI(ssl.message, GetServiceURL);
                        OutputResponse = "URL:\r\n" + GetServiceURL + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;
                        string Out_response = ssl.htmlText;
                        string[] response_Value = ParseReportCode(Out_response);  //BBPSParseReportCode
                        string ReturnOutpot = response_Value[5];
                        string ErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(ReturnOutpot);
                        #endregion

                        if (ReturnOutpot == "0")
                        {
                            CommissionDistributionHelper objChkComm = new CommissionDistributionHelper();
                            bool ComCheck = await objChkComm.DeductAmountFromMerchant(merchantid, "UTILITY", objval.RechargeAmt, objval.RechargeAmt, objval.RechargeAmt, operatorId, "WATER", IpAddress, sRandomOTP, objval.CustomerId, "Instantpay", "WATER");
                            if (ComCheck == true)
                            {
                                //var Recharge = BBPSPaymentAPI.BBPSBillPaymentWATER(operatorId, objval.MobileNo, objval.CustomerId, objval.RechargeAmount.ToString(), geoLoc, outletid, objval.WaterRefNo);
                                //string WATERRespo = Convert.ToString(Recharge);
                                //string ErrorDescription = Recharge.ipay_errordesc;
                                //string errorcodeValue = Recharge.statuscode;
                                //string errorcode = string.IsNullOrEmpty(errorcodeValue) ? ErrorDescription : errorcodeValue;
                                //if (errorcode == "TXN" || errorcode == "TUP")
                                var Recharge = CyberPlatBBPSBillUtilityRecharge._strValidationBBPSFetchBillUtility(SessionNo, objval.CustomerId, "4572", "1.00", AgentIdCyberplate, objval.CUSTOMER_FIRST_NAME, objval.CUSTOMER_LAST_NAME, objval.CUSTOMER_EMAIL_ID, objval. MobileNo, "28.5506,77.2692", objval.PIN, "123456");
                                ssl.message = ssl.Sign_With_PFX(Recharge, keyPath, "rahul123");
                                ssl.htmlText = ssl.CallCryptoAPI(ssl.message, ApiUrl.OPTION2);
                                FinalOutputRes = "URL:\r\n" + ApiUrl.OPTION2 + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;
                                string FinalOut_response = ssl.htmlText;
                                string[] Finalresponse_Value = ParseReportCode(FinalOut_response);
                                string FianlReturnOutpot = Finalresponse_Value[5];
                                string FinalErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(FianlReturnOutpot);



                                //if (errorcode == "TXN" || errorcode == "TUP")
                                if (FianlReturnOutpot == "0")
                                {
                                    //string status = Recharge.status;
                                    //var ipat_id = Recharge.data.ipay_id.Value;
                                    //var opr_idVal = Recharge.data.opr_id;
                                    //var RechMsgStatus = Recharge.data.status;
                                    //decimal trans_amt = decimal.Parse(Convert.ToString(Recharge.data.trans_amt.Value));
                                    //decimal Charged_Amt = decimal.Parse(Convert.ToString(Recharge.data.charged_amt.Value));
                                    //decimal Opening_Balance = decimal.Parse(Convert.ToString(Recharge.data.opening_bal.Value));
                                    //DateTime datevalue = Convert.ToDateTime(Recharge.data.datetime.Value);
                                    //string agentId_Value = Recharge.data.agent_id;
                                    //string Operator_ID = Recharge.data.opr_id;
                                    //string statusCode = Recharge.statuscode;
                                    //string outputval = Recharge.status;
                                    string Final_ErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(FinalErrorReturn);
                                    string[] GetJsonRes = ConvertEncodeStringInJson(FinalOut_response);
                                    string returnUrl = Server.UrlDecode(GetJsonRes[0]);
                                    //string returnUrl = Server.UrlDecode(Finalresponse_Value[8]);  //Old
                                    var Objsettng = returnUrl;
                                    JObject json = JObject.Parse(Objsettng);
                                    var ResponseResult = JObject.Parse(Objsettng);
                                    dynamic Doremit = ResponseResult;
                                    string WATERRespo = Convert.ToString(ResponseResult);
                                    #region Cyberplate Response
                                    var errorcode = Doremit.payment_status;
                                    string status = Doremit.payment_status;
                                    var ipat_id = Doremit.paymentid;
                                    var customerid = Doremit.customerid;
                                    var bbps_ref_no = Doremit.bbps_ref_no;
                                    var ValidationId = Doremit.validationid;
                                    var RechMsgStatus = Doremit.payment_status;
                                    var opr_idVal = Doremit.billerid;
                                    decimal trans_amt = decimal.Parse(Convert.ToString(Doremit.payment_amount.Value));
                                    //decimal Charged_Amt = decimal.Parse(Convert.ToString(Doremit.data.charged_amt.Value));
                                    //decimal Opening_Balance = decimal.Parse(Convert.ToString(Doremit.data.opening_bal.Value));
                                    //DateTime datevalue = Convert.ToDateTime(Doremit.data.datetime.Value);
                                    string outputval = Doremit.payment_status;
                                    #endregion

                                    DMR_API_Response objWaterResp = new DMR_API_Response()
                                    {
                                        serviceTax = "0",
                                        clientRefId = ipat_id,
                                        fee = "0",
                                        initiatorId = ipat_id,
                                        accountNumber = objval.CustomerId,
                                        txnStatus = status,
                                        name = opr_idVal,
                                        ifscCode = trans_amt.ToString(),
                                        impsRespCode = trans_amt.ToString(),
                                        impsRespMessage = status,
                                        txnId = ipat_id,
                                        timestamp = RechMsgStatus
                                        //serviceTax = "10",
                                        //clientRefId = "23026041",
                                        //fee = "00",
                                        //initiatorId = "23026041",
                                        //accountNumber = "9903116214",
                                        //txnStatus = "00",
                                        //name = "AIRTEL",
                                        //ifscCode = "10714.41",
                                        //impsRespCode = "10714.41",
                                        //impsRespMessage = "00",
                                        //txnId = "23026041",
                                        //timestamp = "00"
                                    };

                                    CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                    bool checkComm = await objComm.AllMemberCommissionDistribution(merchantid, "UTILITY", objval.RechargeAmt, trans_amt, trans_amt, operatorId, "WATER Bill", IpAddress, sRandomOTP, objWaterResp, WATERRespo);
                                    if (checkComm == true)
                                    {
                                        var ApiResponse = db.TBL_INSTANTPAY_RECHARGE_RESPONSE.FirstOrDefault(x => x.CORELATIONID == sRandomOTP);
                                        ApiResponse.Ipay_Id = ipat_id;
                                        //ApiResponse.AgentId = Recharge.data.agent_id.Value;
                                        //ApiResponse.Opr_Id = Recharge.data.opr_id.Value;
                                        //ApiResponse.AccountNo = Recharge.data.account_no.Value;
                                        //ApiResponse.Sp_Key = Recharge.data.sp_key.Value;
                                        ApiResponse.AgentId = Doremit.data.agent_id.Value;
                                        ApiResponse.Opr_Id = Doremit.data.opr_id.Value;
                                        ApiResponse.AccountNo = Doremit.data.account_no;
                                        ApiResponse.Sp_Key = Doremit.data.sp_key;
                                        ApiResponse.Trans_Amt = decimal.Parse(trans_amt.ToString());
                                        ApiResponse.Charged_Amt = decimal.Parse(trans_amt.ToString());
                                        ApiResponse.Opening_Balance = decimal.Parse(trans_amt.ToString());
                                        ApiResponse.DateVal = System.DateTime.Now;
                                        //ApiResponse.Status = Recharge.data.status;
                                        //ApiResponse.Res_Code = Recharge.statuscode;
                                        ApiResponse.Status = Doremit.data.status;
                                        ApiResponse.Res_Code = Doremit.statuscode;
                                        ApiResponse.res_msg = status;
                                        ApiResponse.RechargeType = "WATER";
                                        ApiResponse.RechargeResponse = WATERRespo;
                                        ApiResponse.ERROR_TYPE = "SUCCESS";
                                        ApiResponse.ISREVERSE = "Yes";
                                        ApiResponse.ISCOMMISSIONDISBURSE = "Yes";
                                        db.Entry(ApiResponse).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                        //return Json(outputval);
                                        return Json("Transaction Successfull");
                                    }
                                    else
                                    {
                                        return Json("Transaction Failed");
                                    }
                                }
                                else
                                {
                                    CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                    string Statusval = objComm.RefundCommissionInWallet(sRandomOTP, "SERVICE");
                                    if (Statusval == "Return Success")
                                    {
                                        return Json("Transaction Failed");
                                    }
                                    else
                                    {
                                        return Json(FinalErrorReturn);
                                    }
                                    return Json(FinalErrorReturn);
                                }
                            }
                            else
                            {
                                return Json("Transaction Failed");
                            }
                        }
                        else
                        {
                            string msgval = ErrorReturn;
                            return Json(msgval);
                        }
                    }
                    else
                    {
                        var msg = "Can't procceed with transaction.You don't have sufficient balance.";
                        return Json(msg);
                    }
                }
                else
                {
                    var msg = "Insufficient balance in master wallet.";
                    return Json(msg);
                }


            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return Json(msg);
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- PostElectricityBill(POST) Line No:- 613", ex);
                throw ex;
            }


        }


        #endregion


        #region Get Merchant Outlet Information
        [HttpPost]
        public JsonResult GetMerchantOutletInformation()
        {
            initpage();
            try
            {
                initpage();
                var dbCon = new DBContext();
                var data = dbCon.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).FirstOrDefault();
                if (data != null)
                {
                    var GetOutletInfo = dbCon.TBL_MERCHANT_OUTLET_INFORMATION.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
                    if (GetOutletInfo == null)
                    {
                        return Json(data);
                    }
                    else
                    {
                        return Json("NotFound");
                    }
                }
                else
                {
                    return Json("NotFound");
                }
               
            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return Json(msg);
                Logger.Error("Controller:-  PostOutletInformation(Merchant Outlet), method:- GenerateOutletOTP(POST) Line No:- 118", ex);
                throw ex;
            }

        }


        [HttpPost]
        public async Task<JsonResult> PostRegisterOutletData(string PanNo, string OTP )
        {

            try
            {
                var db = new DBContext();
                var GetMerinfo = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
                //string mobileno = Request.Form["Reg_Mobile"].Remove(',');
                var PaymentValidation = OutletApi.RegisterOutlet(GetMerinfo.MEMBER_MOBILE, OTP, GetMerinfo.EMAIL_ID, GetMerinfo.COMPANY, GetMerinfo.MEMBER_NAME, GetMerinfo.ADDRESS, GetMerinfo.PIN, PanNo);
                if (PaymentValidation.statuscode == "TXN")
                {
                    string statuscode = PaymentValidation.statuscode;
                    string outletid = PaymentValidation.data.outlet_id;
                    string Outletname= PaymentValidation.data.outlet_name;
                    string OutletPancard = PaymentValidation.data.pan_no;
                    string OutletStatus = PaymentValidation.data.outlet_status;
                    string OutletKycStatus = PaymentValidation.data.kyc_status;
                    var checkoutletid = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.OUTLETID == outletid).FirstOrDefault();
                    if (checkoutletid == null)
                    {
                        TBL_MERCHANT_OUTLET_INFORMATION objmsg = new TBL_MERCHANT_OUTLET_INFORMATION()
                        {
                            MEM_ID = CurrentMerchant.MEM_ID,
                            OUTLETID = outletid,
                            MOBILE = GetMerinfo.MEMBER_MOBILE,
                            EMAIL = GetMerinfo.EMAIL_ID,
                            OUTLETNAME = Outletname,
                            CONTACTPERSON = PaymentValidation.data.contact_person.Value,
                            //AADHAARNO = PaymentValidation.data.aadhaar_no.Value,
                            PANCARDNO = PaymentValidation.data.pan_no.Value,
                            KYC_STATUS = 0,
                            OUTLET_STATUS = 0,
                            INSERTED_DATE = System.DateTime.Now,
                            INSERTED_BY = CurrentMerchant.MEM_ID,
                            PINCODE = GetMerinfo.PIN

                        };
                        db.TBL_MERCHANT_OUTLET_INFORMATION.Add(objmsg);
                        await db.SaveChangesAsync();
                    }
                    return Json(statuscode);
                }
                else
                {
                    string statuscode = PaymentValidation.status;
                    return Json(statuscode);
                }
            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return Json(msg);
                throw;
            }
        }
        #endregion


        #region For Mobile
        public ActionResult MobileRechargeServices()
        {
            if (Session["MerchantUserId"] != null)
            {
                initpage();
                return View();
            }
            else
            {
                FormsAuthentication.SignOut();
                Session["MerchantUserId"] = null;
                Session["MerchantUserName"] = null;
                Session.Clear();
                Session.Remove("MerchantUserId");
                Session.Remove("MerchantUserName");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
              
        }
        #endregion
        #region DTH
        public ActionResult DTHServices()
        {
            try
            {
                if (Session["MerchantUserId"] != null)
                {
                    var db = new DBContext();
                    var DTHOperator = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "DTH" && x.STATUS == 0 && x.OPTION4 == "CYBERPLAT").OrderBy(c => c.TYPE).ToList();
                    ViewBag.DTHOperator = DTHOperator;
                    return View();
                }
                else
                {
                    FormsAuthentication.SignOut();
                    Session["MerchantUserId"] = null;
                    Session["MerchantUserName"] = null;
                    Session.Clear();
                    Session.Remove("MerchantUserId");
                    Session.Remove("MerchantUserName");
                    return RedirectToAction("Index", "Login", new { area = "" });
                }                   
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region LandLine
        public ActionResult LandlineServices()
        {
            try
            {
                if (Session["MerchantUserId"] != null)
                {
                    var db = new DBContext();
                    var LANDLINEOperator = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "LANDLINE" && x.STATUS == 0 && x.OPTION4 == "CYBERPLAT").OrderBy(c => c.TYPE).ToList();
                    ViewBag.LANDLINEOperator = LANDLINEOperator;
                    return View();
                }
                else
                {
                    FormsAuthentication.SignOut();
                    Session["MerchantUserId"] = null;
                    Session["MerchantUserName"] = null;
                    Session.Clear();
                    Session.Remove("MerchantUserId");
                    Session.Remove("MerchantUserName");
                    return RedirectToAction("Index", "Login", new { area = "" });
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }
        #endregion
        #region Broadband
        public ActionResult BroadbandServices()
        {
            try
            {
                if (Session["MerchantUserId"] != null)
                {
                    var db = new DBContext();
                    var BROADBANDOperator = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "BROADBAND" && x.STATUS == 0 && x.OPTION4 == "CYBERPLAT").OrderBy(c => c.TYPE).ToList();
                    ViewBag.BROADBANDOperator = BROADBANDOperator;
                    return View();
                }
                else
                {
                    FormsAuthentication.SignOut();
                    Session["MerchantUserId"] = null;
                    Session["MerchantUserName"] = null;
                    Session.Clear();
                    Session.Remove("MerchantUserId");
                    Session.Remove("MerchantUserName");
                    return RedirectToAction("Index", "Login", new { area = "" });
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion
        #region Electricity
        public ActionResult ElectricityServices()
        {
            try
            {
                if (Session["MerchantUserId"] != null)
                {
                    var db = new DBContext();
                    var ElectricityOperator = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "ELECTRICITY" && x.STATUS == 0 && x.OPTION4 == "CYBERPLAT").OrderBy(c => c.TYPE).ToList();
                    ViewBag.ElectricityOperator = ElectricityOperator;
                    return View();
                }
                else
                {
                    FormsAuthentication.SignOut();
                    Session["MerchantUserId"] = null;
                    Session["MerchantUserName"] = null;
                    Session.Clear();
                    Session.Remove("MerchantUserId");
                    Session.Remove("MerchantUserName");
                    return RedirectToAction("Index", "Login", new { area = "" });
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
           
        }
        #endregion
        #region Water
        public ActionResult WaterServices()
        {          
            try
            {
                if (Session["MerchantUserId"] != null)
                {
                    var db = new DBContext();
                    var WaterOperator = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "WATER" && x.STATUS == 0 && x.OPTION4 == "CYBERPLAT").OrderBy(c => c.TYPE).ToList();
                    ViewBag.WaterOperator = WaterOperator;
                    return View();
                }
                else
                {
                    FormsAuthentication.SignOut();
                    Session["MerchantUserId"] = null;
                    Session["MerchantUserName"] = null;
                    Session.Clear();
                    Session.Remove("MerchantUserId");
                    Session.Remove("MerchantUserName");
                    return RedirectToAction("Index", "Login", new { area = "" });
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion
        #region Gas
        public ActionResult GasServices()
        {
            try
            {
                if (Session["MerchantUserId"] != null)
                {
                    var db = new DBContext();

                    var GasOperator = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "GAS" && x.STATUS == 0 && x.OPTION4 == "CYBERPLAT").OrderBy(c => c.TYPE).ToList();
                    ViewBag.GasOperator = GasOperator;
                    return View();
                }
                else
                {
                    FormsAuthentication.SignOut();
                    Session["MerchantUserId"] = null;
                    Session["MerchantUserName"] = null;
                    Session.Clear();
                    Session.Remove("MerchantUserId");
                    Session.Remove("MerchantUserName");
                    return RedirectToAction("Index", "Login", new { area = "" });
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion


        #region CyberPlat Response converted into string Array  
        private static string[] ParseReportCode(string reportCode)
        {
            const int FIRST_VALUE_ONLY_SEGMENT = 3;
            const int GRP_SEGMENT_NAME = 1;
            const int GRP_SEGMENT_VALUE = 2;
            Regex reportCodeSegmentPattern = new Regex(@"\s*([^\}\{=\s]+)(?:=\[?([^\s\]\}]+)\]?)?");            
            //Regex reportCodeSegmentPattern = new Regex(@"(\s*([^\}\{=\s]+)(?:=\[?([^\s\]\}]+)\]?)?)|(\n)");
            Match matchReportCodeSegment = reportCodeSegmentPattern.Match(reportCode);

            List<string> parsedCodeSegmentElements = new List<string>();
            int segmentCount = 0;
            while (matchReportCodeSegment.Success)
            {
                if (++segmentCount < FIRST_VALUE_ONLY_SEGMENT)
                {
                    string segmentName = matchReportCodeSegment.Groups[GRP_SEGMENT_NAME].Value;
                    parsedCodeSegmentElements.Add(segmentName);
                }
                string segmentValue = matchReportCodeSegment.Groups[GRP_SEGMENT_VALUE].Value;
                if (segmentValue.Length > 0) parsedCodeSegmentElements.Add(segmentValue);
                matchReportCodeSegment = matchReportCodeSegment.NextMatch();
            }
            return parsedCodeSegmentElements.ToArray();
        }

        private static string[] BBPSParseReportCode(string reportCode)
        {
            const int FIRST_VALUE_ONLY_SEGMENT = 3;
            const int GRP_SEGMENT_NAME = 1;
            const int GRP_SEGMENT_VALUE = 2;
            Regex reportCodeSegmentPattern = new Regex(@"\s*([^\}\{=\s]+)(?:=\[?([^\s\]\}]+)\]?)?");
            //Regex reportCodeSegmentPattern = new Regex(@"\s*(\{([^()]|())*\})(?:=\[?([^\s\]\}]+)\]?)?");  // For Get Json Response
            //1. \{([^()]|())*\}
            Match matchReportCodeSegment = reportCodeSegmentPattern.Match(reportCode);

            List<string> parsedCodeSegmentElements = new List<string>();
            int segmentCount = 0;
            while (matchReportCodeSegment.Success)
            {
                if (++segmentCount < FIRST_VALUE_ONLY_SEGMENT)
                {
                    string segmentName = matchReportCodeSegment.Groups[GRP_SEGMENT_NAME].Value;
                    parsedCodeSegmentElements.Add(segmentName);
                }
                string segmentValue = matchReportCodeSegment.Groups[GRP_SEGMENT_VALUE].Value;
                if (segmentValue.Length > 0) parsedCodeSegmentElements.Add(segmentValue);
                matchReportCodeSegment = matchReportCodeSegment.NextMatch();
            }
            return parsedCodeSegmentElements.ToArray();
        }
        private static string[] ConvertEncodeStringInJson(string reportCode)
        {
            const int FIRST_VALUE_ONLY_SEGMENT = 3;
            const int GRP_SEGMENT_NAME = 1;
            const int GRP_SEGMENT_VALUE = 2;
            //Regex reportCodeSegmentPattern = new Regex(@"\s*([^\}\{=\s]+)(?:=\[?([^\s\]\}]+)\]?)?");
            Regex reportCodeSegmentPattern = new Regex(@"\s*(\{([^()]|())*\})(?:=\[?([^\s\]\}]+)\]?)?");  // For Get Json Response
            //1. \{([^()]|())*\}
            Match matchReportCodeSegment = reportCodeSegmentPattern.Match(reportCode);

            List<string> parsedCodeSegmentElements = new List<string>();
            int segmentCount = 0;
            while (matchReportCodeSegment.Success)
            {
                if (++segmentCount < FIRST_VALUE_ONLY_SEGMENT)
                {
                    string segmentName = matchReportCodeSegment.Groups[GRP_SEGMENT_NAME].Value;
                    parsedCodeSegmentElements.Add(segmentName);
                }
                string segmentValue = matchReportCodeSegment.Groups[GRP_SEGMENT_VALUE].Value;
                if (segmentValue.Length > 0) parsedCodeSegmentElements.Add(segmentValue);
                matchReportCodeSegment = matchReportCodeSegment.NextMatch();
            }
            return parsedCodeSegmentElements.ToArray();
        }

        #endregion


    }
    public class Location
    {
        public string IPAddress { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public string CityName { get; set; }
        public string RegionName { get; set; }
        public string ZipCode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string TimeZone { get; set; }
    }
}