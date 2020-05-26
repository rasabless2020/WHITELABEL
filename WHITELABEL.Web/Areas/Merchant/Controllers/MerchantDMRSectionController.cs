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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Web.Security;
using System.Net;
using System.Text.RegularExpressions;
using static WHITELABEL.Web.Helper.CyberPlateAPIHelper;
using System.Text;
using CyberPlatOpenSSL;
namespace WHITELABEL.Web.Areas.Merchant.Controllers
{
    [Authorize]
    public class MerchantDMRSectionController : MerchantBaseController
    {
        OpenSSL ssl = new OpenSSL(); StringBuilder str = new StringBuilder();
        public static string SessionNo = DateTime.Parse(DateTime.Now.ToString()).ToString("dddMMMddyyyyHHmmss");
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        //public void initpage()
        //{
        //    try
        //    {
        //        ViewBag.ControllerName = "Merchant Dashboard";
        //        SystemClass sclass = new SystemClass();
        //        string userID = sclass.GetLoggedUser();
        //        long userid = long.Parse(userID);
        //        var dbmain = new DBContext();
        //        if (userID != null && userID != "")
        //        {
        //            TBL_MASTER_MEMBER currUser = dbmain.TBL_MASTER_MEMBER.SingleOrDefault(c => c.MEM_ID == userid && c.MEMBER_ROLE == 5 && c.ACTIVE_MEMBER == true);
        //            if (currUser != null)
        //            {
        //                Session["MerchantUserId"] = currUser.MEM_ID;
        //                // Session["UserName"] = currUser.UserName;
        //            }
        //            else
        //            {
        //                Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //                return;
        //            }
        //        }
        //        if (Session["MerchantUserId"] == null)
        //        {
        //            //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //            Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //            return;
        //        }
        //        bool Islogin = false;
        //        if (Session["MerchantUserId"] != null)
        //        {
        //            Islogin = true;
        //            ViewBag.CurrentUserId = CurrentMerchant.MEM_ID;
        //        }
        //        ViewBag.Islogin = Islogin;
        //    }
        //    catch (Exception e)
        //    {
        //        //ViewBag.UserName = CurrentUser.UserId;
        //        Console.WriteLine(e.InnerException);
        //        return;
        //    }
        //}
        public void initpage()
        {
            try
            {
                if (DomainNameCheck.DomainChecking(Session["DOMAINNAME"].ToString(), Request.Url.Host) == false)
                {
                    Response.Redirect(Url.Action("DomainError", "Login", new { area = "" }));
                }
                ViewBag.ControllerName = "Merchant";
                if (Session["MerchantUserId"] == null)
                {
                    Response.Redirect(Url.Action("Logout", "Login", new { area = "" }));
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
                Console.WriteLine(e.InnerException);
                return;
            }
        }

        [HttpPost]
        public JsonResult GetCustomerInformation(string Custmerid)
        {
            initpage();
            try
            {
                var db = new DBContext();
                //// Fetch Customer
                var FetchCustomer = TransXT_DMR_API.FetchCustomer(Custmerid);
                Session["DMRMobileNo"] = Custmerid;
                //var ResponseResult = JObject.Parse(FetchCustomer);
                //var data = JsonConvert.SerializeObject(FetchCustomer);
                //var FetchCustomer11 = TransXT_DMR_API.FetchCustomer("6290665805");
                return Json(FetchCustomer, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return Json(msg);
                Logger.Error("Controller:-  GetCustomerInformation(Merchant DMR), method:- GetCustomerInformation(POST) Line No:- 90", ex);
                throw ex;
            }
            
        }
        [HttpPost]
        public JsonResult GenerateOTP(string Custmerid)
        {
            initpage();
            try
            {
                var db = new DBContext();
                //// Fetch Customer
                var OTPCall = TransXT_DMR_API.GenerateOTP(Custmerid, "1", "");
                var errmsg = OTPCall.errorMsg.Value;
                if (errmsg == "SUCCESS")
                {
                    return Json("OTP Send to your Mobile no.", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(errmsg, JsonRequestBehavior.AllowGet);
                }
                //return Json(OTPCall, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return Json(msg);
                Logger.Error("Controller:-  GetCustomerInformation(Merchant DMR), method:- GenerateOTP(POST) Line No:- 118", ex);
                throw ex;
            }

        }

        // GET: Merchant/MerchantDMRSection
        public ActionResult Index()
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

        //public ActionResult CreateCustomer(string CustId="")
        public ActionResult CreateCustomer()
        {
            //initpage();
            try
            {
                string dmrid = Session["DMRMobileNo"].ToString();
                //string dmrid = "9903246541";
                if (dmrid != "")
                //if (CustId != "")
                {

                    //#region Verifiy OTP Code
                    //string SessionNoObj = DateTime.Parse(DateTime.Now.ToString()).ToString("dddMMMddyyyyHHmmss");
                    //string OutputResponse = string.Empty;
                    //string URL_ValidateRemitter = "https://in.cyberplat.com/cgi-bin/instp/instp_pay_check.cgi";
                    //string keyPath = Server.MapPath("~/Test CERT/myprivatekey.pfx");
                    //var PaymentValidation_AddRem = CyberPlatDMTAPICall._strGetRemitterRegistrationValidation(SessionNoObj, "24", "6290665805", "1331793", "613438", "1.00");
                    //ssl.message = ssl.Sign_With_PFX(PaymentValidation_AddRem, keyPath, "rahul123");
                    //ssl.htmlText = ssl.CallCryptoAPI(ssl.message, URL_ValidateRemitter);
                    //OutputResponse = "URL:\r\n" + URL_ValidateRemitter + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;
                    //string Out_response = ssl.htmlText;
                    //string[] response_Value = ParseReportCode(Out_response);
                    //string ReturnOutpot = response_Value[5];
                    //string ErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(ReturnOutpot);

                    //string CretRemitterRes = string.Empty;
                    //var RemitterPaymentValidation = CyberPlatDMTAPICall._strGetRemitterRegistrationValidation(SessionNoObj, "24", "6290665805", "1331793", "613438", "1.00");
                    //ssl.message = ssl.Sign_With_PFX(RemitterPaymentValidation, keyPath, "rahul123");
                    //ssl.htmlText = ssl.CallCryptoAPI(ssl.message, "https://in.cyberplat.com/cgi-bin/instp/instp_pay.cgi");
                    //CretRemitterRes = "URL:\r\n" + URL_ValidateRemitter + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;

                    //string FinalOut_response = ssl.htmlText;

                    //#endregion


                    //var OTPCall = TransXT_DMR_API.GenerateOTP(CustId, "1", "");
                    TransXT_ADDCustomerModal objval = new TransXT_ADDCustomerModal();
                    objval.MobileNumber = dmrid;
                    //objval.MobileNumber = CustId;
                    //objval.OTP = OTPCall.response.otp.Value;
                    return View(objval);
                    //return View();
                }
                else
                {
                    return RedirectToAction("Index", "MerchantDMRSection", new { area = "Merchant" });
                }
               
            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return View();
                Logger.Error("Controller:-  GetCustomerInformation(Merchant DMR), method:- CreateCustomer(POST) Line No:- 165", ex);
                throw ex;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> PostCreateCustomer(TransXT_ADDCustomerModal objval)
        {
            initpage();
            string valuemsg = string.Empty;
            try
            {
                //var db = new DBContext();
                string SessionNoObj = DateTime.Parse(DateTime.Now.ToString()).ToString("dddMMMddyyyyHHmmss");




                string OutputResponse = string.Empty;
                string URL_ValidateRemitter = "https://in.cyberplat.com/cgi-bin/instp/instp_pay_check.cgi";
                string keyPath = Server.MapPath("~/Test CERT/myprivatekey.pfx");
                var PaymentValidation_AddRem = CyberPlatDMTAPICall._strGetRemitterRegistration(SessionNoObj, "0", objval.MobileNumber, objval.SurName, objval.CustomerName, objval.PinCode, "1.00", "1.00");
                ssl.message = ssl.Sign_With_PFX(PaymentValidation_AddRem, keyPath, "rahul123");
                ssl.htmlText = ssl.CallCryptoAPI(ssl.message, URL_ValidateRemitter);
                OutputResponse = "URL:\r\n" + URL_ValidateRemitter + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;
                string Out_response = ssl.htmlText;
                string[] response_Value = ParseReportCode(Out_response);
                string ReturnOutpot = response_Value[5];
                string ErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(ReturnOutpot);

                string CretRemitterRes = string.Empty;
                var RemitterPaymentValidation = CyberPlatDMTAPICall._strGetRemitterRegistration(SessionNoObj, "0", objval.MobileNumber, objval.SurName, objval.CustomerName, objval.PinCode, "1.00", "1.00");
                ssl.message = ssl.Sign_With_PFX(RemitterPaymentValidation, keyPath, "rahul123");
                ssl.htmlText = ssl.CallCryptoAPI(ssl.message, "https://in.cyberplat.com/cgi-bin/instp/instp_pay.cgi");
                CretRemitterRes = "URL:\r\n" + URL_ValidateRemitter + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;

                string FinalOut_response = ssl.htmlText;
                string[] Final_response_Value = ParseReportCode(FinalOut_response);
                string FinalOutpot = Final_response_Value[3];
                string Final_ErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(FinalOutpot);
                string returnUrl = Server.UrlDecode(Final_response_Value[5]);
                //var Objsettng = returnUrl.Remove(0, 8);
                var Objsettng = returnUrl;
                JObject json = JObject.Parse(Objsettng);
                var ResponseResult = JObject.Parse(Objsettng);
                dynamic FinalResp = ResponseResult;
                var beneficiarylist = FinalResp.data.beneficiary;
                var remitterInfo = FinalResp.statuscode;
                //var remitterInfo ="TXN";
                if (remitterInfo == "TXN")
                {
                    var statusmsg = FinalResp.statuscode;
                    string MsgDetails = FinalResp.status;
                    string RemtID = FinalResp.id;
                    //var statusmsg = "TXN";
                    //string MsgDetails = "OTP sent successfully";
                    //string RemtID = "6658487";
                    return Json(new { msgDetail = MsgDetails, RemitterIDVALUE = RemtID }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var statusmsg = FinalResp.statuscode;
                    string MsgDetails = FinalResp.status;
                    //var statusmsg = "TXN";
                    //string MsgDetails = "OTP sent successfully";
                    return Json(new { msgDetail = MsgDetails, RemitterIDVALUE = "0" }, JsonRequestBehavior.AllowGet);
                }





                //string Outletid = "";
                //const string agentId = "2";
                //long merchantid = 0;
                //long.TryParse(Session["MerchantUserId"].ToString(), out merchantid);
                //var db = new DBContext();
                //string errmsg = string.Empty;
                //var GetOutletid = await db.TBL_MERCHANT_OUTLET_INFORMATION.FirstOrDefaultAsync(x => x.MEM_ID == CurrentMerchant.MEM_ID);
                //Outletid = GetOutletid.OUTLETID;
                //var PaymentValidation = MoneyTransferAPI.RemitterRegistration(objval.MobileNumber, objval.CustomerName, objval.SurName, objval.PinCode, Outletid);
                //string resstring = Convert.ToString(PaymentValidation);
                //if (PaymentValidation.statuscode == "TXN")
                //{
                //    var ipat_id = PaymentValidation.data.remitter.id.Value;
                //    var RemitterId = ipat_id;
                //    string API_Response = Convert.ToString(PaymentValidation);
                //    //double consumedlimit = double.Parse(Convert.ToDouble(PaymentValidation.data.remitter.consumedlimit.Value));
                //    //double remaininglimit = double.Parse(Convert.ToDouble(PaymentValidation.data.remitter.remaininglimit.Value));

                //    TBL_DMR_CUSTOMER_DETAILS objAddCust = new TBL_DMR_CUSTOMER_DETAILS()
                //    {
                //        MEM_ID = CurrentMerchant.MEM_ID,
                //        CUSTOMER_MOBILE = objval.MobileNumber,
                //        CUSTOMER_NAME = objval.CustomerName,
                //        ADDRESS = "",
                //        DOB = "",
                //        CREATED_DATE = System.DateTime.Now,
                //        STATUS = 0,
                //        TRANSACTIONLIMIT = 0,
                //        REMITTER_ID= RemitterId,
                //        API_RESPONSE= API_Response
                //     };
                //    db.TBL_DMR_CUSTOMER_DETAILS.Add(objAddCust);
                //    await db.SaveChangesAsync();
                //    //TBL_DMR_REMITTER_INFORMATION objremittar = new TBL_DMR_REMITTER_INFORMATION()
                //    //{
                //    //    RemitterID = RemitterId,
                //    //    Name = objval.CustomerName,
                //    //    MobileNo = objval.MobileNumber,
                //    //    Address = "",
                //    //    Pincode = objval.PinCode,
                //    //    City = "",
                //    //    State = "",
                //    //    KYCStatus = 0,
                //    //    ConsumedLimited = 0,
                //    //    RemainingLimit = 0,
                //    //    Status = 0,
                //    //    StatusCode = "",
                //    //    MEM_ID = merchantid,
                //    //    InsertedDate = DateTime.Now,
                //    //    UpdateStatus = 0,
                //    //    Total = 0,
                //    //    Perm_txn_limit = 0,
                //    //    KYCDocs = ""

                //    //};
                //    //db.TBL_DMR_REMITTER_INFORMATION.Add(objremittar);
                //    //await db.SaveChangesAsync();

                //    Session["MerchantDMRId"] = ipat_id;
                //    errmsg = PaymentValidation.statuscode;
                //    string MsgDetails= PaymentValidation.status;
                //    string RemtID = RemitterId;
                //    if (errmsg == "TXN")
                //    {
                //        return Json(new { msgDetail = MsgDetails,RemitterIDVALUE= RemtID },JsonRequestBehavior.AllowGet);

                //        //return Json("Customer Created", JsonRequestBehavior.AllowGet);   RemitterId_val = RemitterId
                //    }
                //    else
                //    {
                //        return Json(MsgDetails, JsonRequestBehavior.AllowGet);
                //    }

                //    //DateTime DOB_Val = Convert.ToDateTime(objval.DOB);
                //    //string DOB = DOB_Val.ToString("yyyy-MM-dd");
                //    //var CreateCustomerCall = TransXT_DMR_API.CreateCustomer(objval.MobileNumber, objval.CustomerName, objval.Address, DOB, objval.OTP);
                //    //valuemsg = JsonConvert.SerializeObject(CreateCustomerCall);
                //    //var CustomerInfo = CreateCustomerCall.response.customerId.Value;
                //    //var txnId = CreateCustomerCall.txnId.Value;
                //    //var errmsg = CreateCustomerCall.errorMsg.Value;
                //    //string val = "";
                //    //TBL_DMR_CUSTOMER_DETAILS objAddCust = new TBL_DMR_CUSTOMER_DETAILS()
                //    //{
                //    //    MEM_ID=CurrentMerchant.MEM_ID,
                //    //    CUSTOMER_MOBILE = CustomerInfo,
                //    //    CUSTOMER_NAME = objval.CustomerName,
                //    //    ADDRESS = objval.Address,
                //    //    DOB= DOB,
                //    //    CREATED_DATE=System.DateTime.Now,
                //    //    STATUS=0,
                //    //    TRANSACTIONLIMIT=0
                //    //};
                //    //db.TBL_DMR_CUSTOMER_DETAILS.Add(objAddCust);
                //    //await db.SaveChangesAsync();
                //    //if (errmsg == "SUCCESS")
                //    //{
                //    //    return Json("Customer Created", JsonRequestBehavior.AllowGet);
                //    //}
                //    //else
                //    //{
                //    //    return Json(errmsg, JsonRequestBehavior.AllowGet);
                //    //}
                //}
                //else
                //{
                //    errmsg = PaymentValidation.statuscode;
                //    string MsgDetails = PaymentValidation.status;
                    
                //    return Json(new { msgDetail = MsgDetails, RemitterIDVALUE = "0" }, JsonRequestBehavior.AllowGet);
                //}
            }
            catch (Exception ex)
            {
                var msg = valuemsg;
                return Json(msg);
                Logger.Error("Controller:-  GetCustomerInformation(Merchant DMR), method:- PostCreateCustomer(POST) Line No:- 209", ex);
                throw ex;
            }
        }
        public ActionResult AddRecipient(string CustId = "")
        {
            initpage();
            try
            {
                if (CustId != "")
                {
                    //var fetchRecipient = TransXT_DMR_API.FetchRecipient("3034265");
                    //var fetchAllRecipient = TransXT_DMR_API.FetchAllRecipient("9659874569");
                    string custid = Decrypt.DecryptMe(CustId);
                    TBL_DMR_RECIPIENT_DETAILS obnRecipient = new TBL_DMR_RECIPIENT_DETAILS();
                    obnRecipient.CUSTOMER_ID = custid;
                    return View(obnRecipient);
                }
                else
                {
                    return RedirectToAction("Index", "MerchantDMRSection", new { area = "Merchant" });
                }
            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return View();
                Logger.Error("Controller:-  GetCustomerInformation(Merchant DMR), method:- AddRecipient(POST) Line No:- 235", ex);
                throw ex;
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> PostData_AddRecipient(TBL_DMR_RECIPIENT_DETAILS objRecipient)
        {
            initpage();
            string valuemsg = string.Empty;
            try
            {
                var db = new DBContext();

                #region Add New Beneficiary Code

                string SessionNoObj = DateTime.Parse(DateTime.Now.ToString()).ToString("dddMMMddyyyyHHmmss");
                var RemitterId = db.TBL_DMR_CUSTOMER_DETAILS.FirstOrDefault(x => x.CUSTOMER_MOBILE == objRecipient.CUSTOMER_ID).REMITTER_ID;
                string OutputResponse = string.Empty;
                string URL_ValidateRemitter = "https://in.cyberplat.com/cgi-bin/instp/instp_pay_check.cgi";
                string keyPath = Server.MapPath("~/Test CERT/myprivatekey.pfx");
                var PaymentValidation_AddRem = CyberPlatDMTAPICall._strBeneficiaryRegistration(SessionNoObj, "4", RemitterId, objRecipient.BENEFICIARY_LASTNAME, objRecipient.BENEFICIARY_NAME, objRecipient.BENEFICIARY_MOBILE, objRecipient.ACCOUNT_NO, objRecipient.IFSC_CODE, "1.00");
                ssl.message = ssl.Sign_With_PFX(PaymentValidation_AddRem, keyPath, "rahul123");
                ssl.htmlText = ssl.CallCryptoAPI(ssl.message, URL_ValidateRemitter);
                OutputResponse = "URL:\r\n" + URL_ValidateRemitter + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;
                string Out_response = ssl.htmlText;
                string[] response_Value = ParseReportCode(Out_response);
                string ReturnOutpot = response_Value[5];
                string ErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(ReturnOutpot);

                string CretRemitterRes = string.Empty;
                var RemitterPaymentValidation = CyberPlatDMTAPICall._strBeneficiaryRegistration(SessionNoObj, "4", RemitterId, objRecipient.BENEFICIARY_LASTNAME, objRecipient.BENEFICIARY_NAME, objRecipient.BENEFICIARY_MOBILE, objRecipient.ACCOUNT_NO, objRecipient.IFSC_CODE, "1.00");
                ssl.message = ssl.Sign_With_PFX(RemitterPaymentValidation, keyPath, "rahul123");
                ssl.htmlText = ssl.CallCryptoAPI(ssl.message, "https://in.cyberplat.com/cgi-bin/instp/instp_pay.cgi");
                CretRemitterRes = "URL:\r\n" + URL_ValidateRemitter + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;

                string FinalOut_response = ssl.htmlText;
                string[] Final_response_Value = ParseReportCode(FinalOut_response);
                string FinalOutpot = Final_response_Value[3];
                if (FinalOutpot == "224")
                {
                    string FinalOutpot_Msg = Final_response_Value[6];
                    string msg = "Invalid Account Number. or Invalid Service Provider. or Invalid Beneficiary IFSC. Please check data befor add Beneficiary";
                    return Json(msg, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string Final_ErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(FinalOutpot);
                    string returnUrl = Server.UrlDecode(Final_response_Value[5]);
                    //var Objsettng = returnUrl.Remove(0, 8);
                    var Objsettng = returnUrl;
                    JObject json = JObject.Parse(Objsettng);
                    var ResponseResult = JObject.Parse(Objsettng);
                    dynamic FinalResp = ResponseResult;
                    string resstring = Convert.ToString(ResponseResult);
                    var beneficiaryId_Val = FinalResp.data.beneficiary.id;
                    var RemitterId_Val = FinalResp.data.remitter.id;
                    var remitterInfo = FinalResp.statuscode;

                    TBL_DMR_RECIPIENT_DETAILS objbeneficiary = new TBL_DMR_RECIPIENT_DETAILS()
                    {
                        MEM_ID = CurrentMerchant.MEM_ID,
                        CUSTOMER_ID = objRecipient.CUSTOMER_ID,
                        BENEFICIARY_NAME = objRecipient.BENEFICIARY_NAME +" "+objRecipient.BENEFICIARY_LASTNAME,
                        ACCOUNT_NO = objRecipient.ACCOUNT_NO,
                        IFSC_CODE = objRecipient.IFSC_CODE,
                        BENEFICIARY_MOBILE = objRecipient.BENEFICIARY_MOBILE,
                        BENEFICIARY_TYPE = "2",
                        CREATE_DATE = System.DateTime.Now,
                        STATUS = 0,
                        TRANSACTIONID = "",
                        RECIPIENT_ID = RemitterId_Val,
                        ISVERIFIED = 0,
                        EMAIL_ID = "beneficiary@gmail.com",
                        REMARKS = objRecipient.REMARKS,
                        BENEFICIARY_ID = beneficiaryId_Val,
                        API_RESPONSE = resstring
                    };
                    db.TBL_DMR_RECIPIENT_DETAILS.Add(objbeneficiary);
                    await db.SaveChangesAsync();


                    return Json("Beneficiary Added Successfully.", JsonRequestBehavior.AllowGet);
                }
             

                #endregion



                //var checkrecipientlist = db.TBL_DMR_RECIPIENT_DETAILS.Count(x => x.CUSTOMER_ID == objRecipient.CUSTOMER_ID);
                //if (checkrecipientlist <= 10)
                //{

                //    var Mem_IDval = long.Parse(CurrentMerchant.MEM_ID.ToString());
                //    var remitterid = Session["MerchantDMRId"].ToString();
                //    var GetOutletid = await db.TBL_MERCHANT_OUTLET_INFORMATION.FirstOrDefaultAsync(x => x.MEM_ID == CurrentMerchant.MEM_ID);
                //    string outletId_value = string.Empty;
                //    outletId_value = GetOutletid.OUTLETID.ToString();
                //    string emailid = GetOutletid.EMAIL;
                //    var PaymentValidation = MoneyTransferAPI.BeneficiaryRegistration(remitterid, objRecipient.BENEFICIARY_MOBILE, objRecipient.BENEFICIARY_NAME, objRecipient.IFSC_CODE, objRecipient.ACCOUNT_NO, outletId_value);
                    
                //    string errorcode = string.IsNullOrEmpty(PaymentValidation.statuscode.Value) ? PaymentValidation.statuscode.Value : PaymentValidation.statuscode.Value;//res.res_code;
                //    var TransMsg = PaymentValidation.status.Value;                                                                                                                                       //if (PaymentValidation != "Unknown Error")
                //    string resstring = Convert.ToString(PaymentValidation);                                                                                                                                           //{
                //                                                                                                                                                                                                      //if (PaymentValidation.statuscode == "TXN")
                //    if (errorcode == "TXN")
                //    {
                //        var ipat_id = PaymentValidation.data.remitter.id;
                //        var Benificiary_id = PaymentValidation.data.beneficiary.id.Value;
                //        var RemitterId = ipat_id.Value;

                //        TBL_DMR_RECIPIENT_DETAILS objbeneficiary = new TBL_DMR_RECIPIENT_DETAILS()
                //        {
                //            MEM_ID = CurrentMerchant.MEM_ID,
                //            CUSTOMER_ID = objRecipient.CUSTOMER_ID,
                //            BENEFICIARY_NAME = objRecipient.BENEFICIARY_NAME,
                //            ACCOUNT_NO = objRecipient.ACCOUNT_NO,
                //            IFSC_CODE = objRecipient.IFSC_CODE,
                //            BENEFICIARY_MOBILE = objRecipient.BENEFICIARY_MOBILE,
                //            BENEFICIARY_TYPE = "2",
                //            CREATE_DATE = System.DateTime.Now,
                //            STATUS = 0,                            
                //            TRANSACTIONID = "",
                //            RECIPIENT_ID = RemitterId,
                //            ISVERIFIED = 0,
                //            EMAIL_ID = emailid,
                //            REMARKS = objRecipient.REMARKS,
                //            BENEFICIARY_ID= Benificiary_id,
                //            API_RESPONSE= resstring
                //        };
                //        db.TBL_DMR_RECIPIENT_DETAILS.Add(objbeneficiary);
                //        await db.SaveChangesAsync();
                //        var msg = PaymentValidation.status.Value;
                //        var msgcode = PaymentValidation.statuscode.Value;
                        
                //        if (objRecipient.VerifyBeneficiary == true)
                //        {
                //            var BeneficiaryValidation = MoneyTransferAPI.BeneficiaryRegistrationResendOTP(RemitterId, Benificiary_id, outletId_value);
                //            msg = BeneficiaryValidation.status.Value;
                //        }
                        

                //        //return Json("Beneficiary is added successfully.",JsonRequestBehavior.AllowGet);
                //        return Json(new { remitterid = remitterid, beneficiaryid = Benificiary_id, status = msg, msgcode = msgcode, CheckSts= objRecipient.VerifyBeneficiary });

                //    }
                //    else
                //    {
                //        return Json(new { remitterid = "", beneficiaryid = "", status = TransMsg, msgcode = PaymentValidation.statuscode.Value, CheckSts = false });
                //        // return Json(TransMsg, JsonRequestBehavior.AllowGet);                        
                //        //return Json(new { remitterid = "", beneficiaryid = "", status = PaymentValidation.status.Value, msgcode = PaymentValidation.statuscode.Value });
                //        ///return Json(PaymentValidation.status);
                //    }

                //    //var CreateRecipient = TransXT_DMR_API.AddRecipient(objRecipient.CUSTOMER_ID, objRecipient.ACCOUNT_NO, objRecipient.IFSC_CODE, objRecipient.BENEFICIARY_MOBILE, objRecipient.BENEFICIARY_NAME, "2");
                //    //string vvv = "";
                //    //valuemsg = JsonConvert.SerializeObject(CreateRecipient);
                //    //var Transid = CreateRecipient.txnId.Value;
                //    //var recipientId = CreateRecipient.response.recipientId.Value;
                //    //string bal = string.Empty;
                //    //TBL_DMR_RECIPIENT_DETAILS rectDetails = new TBL_DMR_RECIPIENT_DETAILS()
                //    //{
                //    //    MEM_ID = CurrentMerchant.MEM_ID,
                //    //    CUSTOMER_ID = objRecipient.CUSTOMER_ID,
                //    //    BENEFICIARY_NAME = objRecipient.BENEFICIARY_NAME,
                //    //    ACCOUNT_NO = objRecipient.ACCOUNT_NO,
                //    //    IFSC_CODE = objRecipient.IFSC_CODE,
                //    //    BENEFICIARY_MOBILE = objRecipient.BENEFICIARY_MOBILE,
                //    //    BENEFICIARY_TYPE = "2",
                //    //    CREATE_DATE = System.DateTime.Now,
                //    //    STATUS = 0,
                //    //    TRANSACTIONID = Transid,
                //    //    RECIPIENT_ID = recipientId,
                //    //    ISVERIFIED = 0,
                //    //    EMAIL_ID = objRecipient.EMAIL_ID,
                //    //    REMARKS = objRecipient.REMARKS
                //    //};
                //    //db.TBL_DMR_RECIPIENT_DETAILS.Add(rectDetails);
                //    //await db.SaveChangesAsync();
                //    //var errmsg = CreateRecipient.errorMsg.Value;
                //    //if (errmsg == "SUCCESS")
                //    //{
                //    //    return Json("Beneficiary Added Successfully.", JsonRequestBehavior.AllowGet);
                //    //}
                //    //else
                //    //{
                //    //    return Json(errmsg, JsonRequestBehavior.AllowGet);
                //    //}
                //}
                //else
                //{
                //    return Json("You add atleast 10 recipient information.", JsonRequestBehavior.AllowGet);
                //}
            }
            catch (Exception ex)
            {
                var msg = valuemsg;
                return Json(msg);
                Logger.Error("Controller:-  GetCustomerInformation(Merchant DMR), method:- PostData_AddRecipient(POST) Line No:- 286", ex);
                throw ex;
            }

        }

        [HttpGet]
        //public PartialViewResult GetAllRecipientLIst(string CustId)
        public PartialViewResult GetAllRecipientLIst()
        {
            try
            {
                string CustId= Session["DMRMobileNo"].ToString();
                //string RecipientID = Request.QueryString["CustId"];
                if (CustId != "")
                {
                    // Only grid query values will be available here.
                    var db = new DBContext();
                    //var BeneficiaryList = db.TBL_DMR_RECIPIENT_DETAILS.Where(x => x.CUSTOMER_ID == CustId && x.STATUS == 0).ToList();

                    var BeneficiaryList11 = (from x in db.TBL_DMR_RECIPIENT_DETAILS
                                           where x.CUSTOMER_ID == CustId && x.STATUS == 0
                                           select new
                                           {
                                               ID = x.ID,
                                               MEM_ID = x.MEM_ID,
                                               CUSTOMER_ID = x.CUSTOMER_ID,
                                               RECIPIENT_ID = x.RECIPIENT_ID,
                                               BENEFICIARY_NAME = x.BENEFICIARY_NAME,
                                               ACCOUNT_NO = x.ACCOUNT_NO,
                                               IFSC_CODE = x.IFSC_CODE,
                                               BENEFICIARY_MOBILE = x.BENEFICIARY_MOBILE,
                                               CREATE_DATE = x.CREATE_DATE,
                                               BENEFICIARY_TYPE = x.BENEFICIARY_TYPE,
                                               STATUS = x.STATUS,
                                               TRANSACTIONID = x.TRANSACTIONID,
                                               ISVERIFIED = x.ISVERIFIED,
                                               REMARKS = x.REMARKS,
                                               EMAIL_ID = x.EMAIL_ID,
                                               ChargedAmount = (db.TBL_ACCOUNT_VERIFICATION_TABLE.Where(d => d.APINAME == "Instantpay").Select(c => c.APPLIED_AMT_TO_MERCHANT).FirstOrDefault()).ToString()
                                           }).ToList();

                    var BeneficiaryList = (from x in db.TBL_DMR_RECIPIENT_DETAILS
                                           where x.CUSTOMER_ID == CustId && x.STATUS == 0
                                           select new
                                           {
                                               ID = x.ID,
                                               MEM_ID = x.MEM_ID,
                                               CUSTOMER_ID = x.CUSTOMER_ID,
                                               RECIPIENT_ID = x.RECIPIENT_ID,
                                               BENEFICIARY_NAME = x.BENEFICIARY_NAME,
                                               ACCOUNT_NO = x.ACCOUNT_NO,
                                               IFSC_CODE = x.IFSC_CODE,
                                               BENEFICIARY_MOBILE = x.BENEFICIARY_MOBILE,
                                               CREATE_DATE = x.CREATE_DATE,
                                               BENEFICIARY_TYPE = x.BENEFICIARY_TYPE,
                                               STATUS = x.STATUS,
                                               TRANSACTIONID = x.TRANSACTIONID,
                                               ISVERIFIED = x.ISVERIFIED,
                                               REMARKS = x.REMARKS,
                                               EMAIL_ID = x.EMAIL_ID,
                                               ChargedAmount = (db.TBL_ACCOUNT_VERIFICATION_TABLE.Where(d => d.APINAME == "Instantpay").Select(c => c.APPLIED_AMT_TO_MERCHANT).FirstOrDefault()).ToString()
                                           }).AsEnumerable().Select(z => new TBL_DMR_RECIPIENT_DETAILS
                                           {
                                               ID = z.ID,
                                               MEM_ID = z.MEM_ID,
                                               CUSTOMER_ID = z.CUSTOMER_ID,
                                               RECIPIENT_ID = z.RECIPIENT_ID,
                                               BENEFICIARY_NAME = z.BENEFICIARY_NAME,
                                               ACCOUNT_NO = z.ACCOUNT_NO,
                                               IFSC_CODE = z.IFSC_CODE,
                                               BENEFICIARY_MOBILE = z.BENEFICIARY_MOBILE,
                                               CREATE_DATE = z.CREATE_DATE,
                                               BENEFICIARY_TYPE = z.BENEFICIARY_TYPE,
                                               STATUS = z.STATUS,
                                               TRANSACTIONID = z.TRANSACTIONID,
                                               ISVERIFIED = z.ISVERIFIED,
                                               REMARKS = z.REMARKS,
                                               EMAIL_ID = z.EMAIL_ID,
                                               ChargedAmount = z.ChargedAmount
                                           }).ToList();


                    return PartialView("GetAllRecipientLIst", BeneficiaryList);
                }
                else
                {
                    // Only grid query values will be available here.
                    var db = new DBContext();
                    var BeneficiaryList = db.TBL_DMR_RECIPIENT_DETAILS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID && x.STATUS == 0).ToList();
                    return PartialView("GetAllRecipientLIst", BeneficiaryList);
                }
               
            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                
                Logger.Error("Controller:-  GetCustomerInformation(Merchant DMR), method:- GetAllRecipientLIst(GET) Line No:- 305", ex);
                throw ex;
            }
        }

        [HttpGet]
        public PartialViewResult GetBeneficiaryList()
        {
            try
            {
                string CustId = Session["DMRMobileNo"].ToString();
                //string RecipientID = Request.QueryString["CustId"];
                if (CustId != "")
                {
                    // Only grid query values will be available here.
                    var db = new DBContext();
                    //var BeneficiaryList = db.TBL_DMR_RECIPIENT_DETAILS.Where(x => x.CUSTOMER_ID == CustId && x.STATUS == 0).ToList(); TBL_DMR_RECIPIENT_DETAILS

                    var BeneficiaryList11 = (from x in db.TBL_DMR_RECIPIENT_DETAILS
                                             where x.CUSTOMER_ID == CustId 
                                             //where x.CUSTOMER_ID == CustId && x.ISVERIFIED==0 && x.STATUS==0
                                             select new
                                             {
                                                 ID = x.ID,
                                                 MEM_ID = x.MEM_ID,
                                                 CUSTOMER_ID = x.CUSTOMER_ID,
                                                 RECIPIENT_ID = x.RECIPIENT_ID,
                                                 BENEFICIARY_NAME = x.BENEFICIARY_NAME,
                                                 ACCOUNT_NO = x.ACCOUNT_NO,
                                                 IFSC_CODE = x.IFSC_CODE,
                                                 BENEFICIARY_MOBILE = x.BENEFICIARY_MOBILE,
                                                 CREATE_DATE = x.CREATE_DATE,
                                                 BENEFICIARY_TYPE = x.BENEFICIARY_TYPE,
                                                 STATUS = x.STATUS,
                                                 TRANSACTIONID = x.TRANSACTIONID,
                                                 ISVERIFIED = x.ISVERIFIED,
                                                 REMARKS = x.REMARKS,
                                                 EMAIL_ID = x.EMAIL_ID,
                                                 ChargedAmount = (db.TBL_ACCOUNT_VERIFICATION_TABLE.Where(d => d.APINAME == "Instantpay").Select(c => c.APPLIED_AMT_TO_MERCHANT).FirstOrDefault()).ToString()
                                             }).ToList();

                    var BeneficiaryList = (from x in db.TBL_DMR_RECIPIENT_DETAILS
                                               //where x.CUSTOMER_ID == CustId && x.STATUS == 0
                                           where x.CUSTOMER_ID == CustId && x.STATUS == 0
                                           select new
                                           {
                                               ID = x.ID,
                                               MEM_ID = x.MEM_ID,
                                               CUSTOMER_ID = x.CUSTOMER_ID,
                                               RECIPIENT_ID = x.RECIPIENT_ID,
                                               BENEFICIARY_NAME = x.BENEFICIARY_NAME,
                                               ACCOUNT_NO = x.ACCOUNT_NO,
                                               IFSC_CODE = x.IFSC_CODE,
                                               BENEFICIARY_MOBILE = x.BENEFICIARY_MOBILE,
                                               CREATE_DATE = x.CREATE_DATE,
                                               BENEFICIARY_TYPE = x.BENEFICIARY_TYPE,
                                               STATUS = x.STATUS,
                                               TRANSACTIONID = x.TRANSACTIONID,
                                               ISVERIFIED = x.ISVERIFIED,
                                               REMARKS = x.REMARKS,
                                               EMAIL_ID = x.EMAIL_ID,
                                               Benef_Id=x.BENEFICIARY_ID
                                               //ChargedAmount = (db.TBL_ACCOUNT_VERIFICATION_TABLE.Where(d => d.APINAME == "MobileWare").Select(c => c.APPLIED_AMT_TO_MERCHANT).FirstOrDefault()).ToString()
                                           }).AsEnumerable().Select(z => new TBL_DMR_RECIPIENT_DETAILS
                                           {
                                               ID = z.ID,
                                               MEM_ID = z.MEM_ID,
                                               CUSTOMER_ID = z.CUSTOMER_ID,
                                               RECIPIENT_ID = z.RECIPIENT_ID,
                                               BENEFICIARY_NAME = z.BENEFICIARY_NAME,
                                               ACCOUNT_NO = z.ACCOUNT_NO,
                                               IFSC_CODE = z.IFSC_CODE,
                                               BENEFICIARY_MOBILE = z.BENEFICIARY_MOBILE,
                                               CREATE_DATE = z.CREATE_DATE,
                                               BENEFICIARY_TYPE = z.BENEFICIARY_TYPE,
                                               STATUS = z.STATUS,
                                               TRANSACTIONID = z.TRANSACTIONID,
                                               ISVERIFIED = z.ISVERIFIED,
                                               REMARKS = z.REMARKS,
                                               EMAIL_ID = z.EMAIL_ID,
                                               BENEFICIARY_ID=z.Benef_Id
                                               //ChargedAmount = z.ChargedAmount
                                           }).ToList();


                    return PartialView("GetBeneficiaryList", BeneficiaryList);
                }
                else
                {
                    // Only grid query values will be available here.
                    var db = new DBContext();
                    //var BeneficiaryList = db.TBL_DMR_RECIPIENT_DETAILS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID && x.STATUS == 0).ToList();
                    var BeneficiaryList = db.TBL_DMR_RECIPIENT_DETAILS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).ToList();
                    return PartialView("GetBeneficiaryList", BeneficiaryList);
                }

            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";

                Logger.Error("Controller:-  GetCustomerInformation(Merchant DMR), method:- GetAllRecipientLIst(GET) Line No:- 305", ex);
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult DeleteRecipientInformation(string RecipientId,string CustomerId)
        {
            initpage();
            try
            {
                var db = new DBContext();
                var Mem_IDval = long.Parse(CurrentMerchant.MEM_ID.ToString());
                //var remitterid = Session["MerchantDMRId"].ToString();

                #region Delete Account
                string SessionNoObj = DateTime.Parse(DateTime.Now.ToString()).ToString("dddMMMddyyyyHHmmss");
                string OutputResponse = string.Empty;
                string URL_ValidateRemitter = "https://in.cyberplat.com/cgi-bin/instp/instp_pay_check.cgi";
                string keyPath = Server.MapPath("~/Test CERT/myprivatekey.pfx");
                var PaymentValidation_AddRem = CyberPlatDMTAPICall._strBeneficiaryDelete(SessionNoObj, "6", RecipientId, CustomerId, "1.00", "1.00");
                ssl.message = ssl.Sign_With_PFX(PaymentValidation_AddRem, keyPath, "rahul123");
                ssl.htmlText = ssl.CallCryptoAPI(ssl.message, URL_ValidateRemitter);
                OutputResponse = "URL:\r\n" + URL_ValidateRemitter + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;
                string Out_response = ssl.htmlText;
                string[] response_Value = ParseReportCode(Out_response);
                string ReturnOutpot = response_Value[5];
                string ErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(ReturnOutpot);

                string CretRemitterRes = string.Empty;
                var RemitterPaymentValidation = CyberPlatDMTAPICall._strBeneficiaryDelete(SessionNoObj, "6", RecipientId, CustomerId, "1.00", "1.00");
                ssl.message = ssl.Sign_With_PFX(RemitterPaymentValidation, keyPath, "rahul123");
                ssl.htmlText = ssl.CallCryptoAPI(ssl.message, "https://in.cyberplat.com/cgi-bin/instp/instp_pay.cgi");
                CretRemitterRes = "URL:\r\n" + URL_ValidateRemitter + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;

                string FinalOut_response = ssl.htmlText;
                string[] Final_response_Value = ParseReportCode(FinalOut_response);
                string FinalOutpot = Final_response_Value[3];
                string Final_ErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(FinalOutpot);
                string returnUrl = Server.UrlDecode(Final_response_Value[5]);
                //var Objsettng = returnUrl.Remove(0, 8);
                var Objsettng = returnUrl;
                JObject json = JObject.Parse(Objsettng);
                var ResponseResult = JObject.Parse(Objsettng);
                dynamic FinalResp = ResponseResult;
                //var beneficiarylist = FinalResp.data.beneficiary;
                var errorcode = FinalResp.statuscode;
                #endregion


                //var Outletid = db.TBL_MERCHANT_OUTLET_INFORMATION.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID).OUTLETID;
                //string out_letid = Outletid.ToString();
                //var PaymentValidation = MoneyTransferAPI.BeneficiaryDelete(RecipientId, CustomerId, out_letid);
                //string errorcode = string.IsNullOrEmpty(PaymentValidation.statuscode.Value) ? PaymentValidation.statuscode.Value : PaymentValidation.statuscode.Value;//res.res_code;                
                //string errorcode = "TXN";
                if (errorcode == "TXN")
                {
                    var ipat_id = RecipientId;
                    var Benificiary_id = CustomerId;
                    var RemitterId = ipat_id;
                    //var OTPID = PaymentValidation.data.otp.Value;
                    //var msg = PaymentValidation.status.Value;
                    //var msgcode = PaymentValidation.statuscode.Value;
                    var OTPID = FinalResp.data.otp.Value;
                    var msg = FinalResp.status.Value;
                    var msgcode = FinalResp.statuscode.Value;
                    return Json(new { remitterid = RecipientId, beneficiaryid = CustomerId, status = msg, msgcode = msgcode, OTPID_Status = OTPID });
                    //return Json(new { remitterid = "741720", beneficiaryid = "1249076", status = "Delete Info", msgcode = "TXN", OTPID_Status = "1" });
                }
                else
                {
                    return Json(new { remitterid = "", beneficiaryid = "", status = "", msgcode = "Please Try Again Later", OTPID_Status = "0" });                  
                }
            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return Json(msg);
                Logger.Error("Controller:-  GetCustomerInformation(Merchant DMR), method:- DeleteRecipientInformation(POST) Line No:- 338", ex);
                throw ex;
            }
            
        }

        [HttpPost]
        public JsonResult DeleteBenefAcnt(string RecipientId, string CustomerId,string OtpVal)
        {
            initpage();
            try
            {
                var db = new DBContext();
                var Mem_IDval = long.Parse(CurrentMerchant.MEM_ID.ToString());
                var remitterid = Session["MerchantDMRId"].ToString();
                var Outletid = db.TBL_MERCHANT_OUTLET_INFORMATION.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID).OUTLETID;
                string out_letid = Outletid.ToString();
                var PaymentValidation = MoneyTransferAPI.BeneficiaryDelete(RecipientId, CustomerId, out_letid);
                string errorcode = string.IsNullOrEmpty(PaymentValidation.statuscode.Value) ? PaymentValidation.statuscode.Value : PaymentValidation.statuscode.Value;//res.res_code;                
                //string errorcode = "TXN";
                if (errorcode == "TXN")
                {
                    var ipat_id = RecipientId;
                    var Benificiary_id = CustomerId;
                    var RemitterId = ipat_id;
                    var OTPID = PaymentValidation.data.otp.Value;
                    var msg = PaymentValidation.status.Value;
                    var msgcode = PaymentValidation.statuscode.Value;
                    return Json(new { remitterid = RecipientId, beneficiaryid = CustomerId, status = msg, msgcode = msgcode, OTPID_Status = OTPID });

                }
                else
                {
                    return Json(new { remitterid = "", beneficiaryid = "", status = "", msgcode = "Please Try Again Later", OTPID_Status = "0" });
                    
                }


            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return Json(msg);
                Logger.Error("Controller:-  GetCustomerInformation(Merchant DMR), method:- DeleteRecipientInformation(POST) Line No:- 338", ex);
                throw ex;
            }

        }

        [HttpPost]
        public async Task<JsonResult> RecipientEnquiry(string RecipientId, string CustomerId)
        {
            try
            {
                initpage();
                var db = new DBContext();
                var GSTTAX = await db.TBL_TAX_MASTERS.FirstOrDefaultAsync(x => x.TAX_NAME == "GST");
                decimal Walletamt = 0;
                decimal subWaltamt = 0;
                decimal Mer_Balance = 0;
                var mastinfo = await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == CurrentMerchant.MEM_ID);
                decimal.TryParse(mastinfo.BALANCE.ToString(), out Mer_Balance);
                string MER_BENE_VERIFY_AMT = System.Configuration.ConfigurationManager.AppSettings["MER_BENE_VERIFY_AMT"];
                decimal Mer_BENE_VRY_AMT = 0;
                decimal.TryParse(MER_BENE_VERIFY_AMT, out Mer_BENE_VRY_AMT);

                
                decimal GSTCALCULATED = Math.Round(((Mer_BENE_VRY_AMT * GSTTAX.TAX_VALUE) / 118),2);

                string CUST_BENE_VERIFY_RTN_AMT = System.Configuration.ConfigurationManager.AppSettings["CUST_BENE_VERIFY_RTN_AMT"];
                decimal CUST_BENE_VRY_AMT = 0;
                decimal.TryParse(CUST_BENE_VERIFY_RTN_AMT, out CUST_BENE_VRY_AMT);
                string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
                var getRecipientInfo = await db.TBL_DMR_RECIPIENT_DETAILS.Where(x => x.RECIPIENT_ID == RecipientId && x.BENEFICIARY_ID == CustomerId).FirstOrDefaultAsync();

                var checkAccount = db.TBL_DMR_RECIPIENT_DETAILS.Where(x => x.RECIPIENT_ID == RecipientId && x.BENEFICIARY_ID == CustomerId && x.ACCOUNT_NO == getRecipientInfo.ACCOUNT_NO).FirstOrDefault();
                if (checkAccount.ISVERIFIED == 0)
                {

                    #region verification of bank acnt

                    string SessionNoObj = DateTime.Parse(DateTime.Now.ToString()).ToString("dddMMMddyyyyHHmmss");                    
                    string OutputResponse = string.Empty;
                    string URL_ValidateRemitter = "https://in.cyberplat.com/cgi-bin/instp/instp_pay_check.cgi";
                    string keyPath = Server.MapPath("~/Test CERT/myprivatekey.pfx");
                    var PaymentValidation_AddRem = CyberPlatDMTAPICall._strBeneficiaryAccountVerification(SessionNoObj, "10", checkAccount.CUSTOMER_ID, checkAccount.ACCOUNT_NO, checkAccount.IFSC_CODE, CUST_BENE_VERIFY_RTN_AMT, MER_BENE_VERIFY_AMT);
                    ssl.message = ssl.Sign_With_PFX(PaymentValidation_AddRem, keyPath, "rahul123");
                    ssl.htmlText = ssl.CallCryptoAPI(ssl.message, URL_ValidateRemitter);
                    OutputResponse = "URL:\r\n" + URL_ValidateRemitter + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;
                    string Out_response = ssl.htmlText;
                    string[] response_Value = ParseReportCode(Out_response);
                    string ReturnOutpot = response_Value[5];
                    string ErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(ReturnOutpot);

                    string CretRemitterRes = string.Empty;
                    var RemitterPaymentValidation = CyberPlatDMTAPICall._strBeneficiaryAccountVerification(SessionNoObj, "10", checkAccount.CUSTOMER_ID, checkAccount.ACCOUNT_NO, checkAccount.IFSC_CODE, CUST_BENE_VERIFY_RTN_AMT, MER_BENE_VERIFY_AMT);
                    ssl.message = ssl.Sign_With_PFX(RemitterPaymentValidation, keyPath, "rahul123");
                    ssl.htmlText = ssl.CallCryptoAPI(ssl.message, "https://in.cyberplat.com/cgi-bin/instp/instp_pay.cgi");
                    CretRemitterRes = "URL:\r\n" + URL_ValidateRemitter + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;

                    string FinalOut_response = ssl.htmlText;
                    string[] Final_response_Value = ParseReportCode(FinalOut_response);
                    string FinalOutpot = Final_response_Value[5];
                    string Final_ErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(FinalOutpot);
                    string returnUrl = Server.UrlDecode(Final_response_Value[8]);
                    var Objsettng = returnUrl;
                    JObject json = JObject.Parse(Objsettng);
                    var ResponseResult = JObject.Parse(Objsettng);
                    dynamic FinalResp = ResponseResult;
                    string resstring = Convert.ToString(ResponseResult);
                    var errorcode = FinalResp.statuscode;
                    //var RemitterId_Val = FinalResp.data.remitter.id;
                    #endregion







                    //var PaymentValidation = MoneyTransferAPI.BeneficiaryAccountVerification(checkAccount.CUSTOMER_ID, checkAccount.ACCOUNT_NO.ToString(), checkAccount.IFSC_CODE.ToString(), agentId);
                    //string errorcode = string.IsNullOrEmpty(PaymentValidation.statuscode.Value) ? PaymentValidation.statuscode.Value : PaymentValidation.statuscode.Value;//res.res_code;
                    if (errorcode == "TXN")
                    {
                        var ipat_id = checkAccount.RECIPIENT_ID;
                        var Benificiary_id = checkAccount.BENEFICIARY_ID;
                        var RemitterId = ipat_id;
                        var msg = FinalResp.status.Value;
                        var msgcode = FinalResp.statuscode.Value;
                        var Verification_Status = FinalResp.data.verification_status.Value;
                        var bankrefno = FinalResp.data.bankrefno.Value;
                        var ipay_id = FinalResp.data.ipay_id.Value;


                        var customerInfo = await db.TBL_DMR_CUSTOMER_DETAILS.Where(x => x.CUSTOMER_MOBILE == getRecipientInfo.CUSTOMER_ID).FirstOrDefaultAsync();
                        Walletamt = customerInfo.TRANSACTIONLIMIT;
                        subWaltamt = (Convert.ToDecimal(Walletamt) - 1);

                        decimal openingAmt = 0;
                        decimal closingAmt = 0;
                        decimal deductedamt = 0;
                        decimal RecipientverifyAmt = 0;
                        decimal UpdateMer_Bal = 0;
                        decimal RecipientMerchantverifyAmt = 0;
                        var Acountdetails = db.TBL_ACCOUNTS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).OrderByDescending(z => z.ACC_NO).FirstOrDefault();
                        if (Acountdetails != null)
                        {
                            decimal.TryParse(Acountdetails.OPENING.ToString(), out openingAmt);
                            decimal.TryParse(Acountdetails.CLOSING.ToString(), out closingAmt);
                            //deductedamt = closingAmt - RecipientverifyAmt;
                            deductedamt = closingAmt - Mer_BENE_VRY_AMT;
                            UpdateMer_Bal = Mer_Balance - (Mer_BENE_VRY_AMT);
                        }
                        else
                        {
                            UpdateMer_Bal = Mer_Balance;
                            mastinfo.BALANCE = UpdateMer_Bal;
                        }
                        decimal WLP_CLOSE = 0;
                        decimal WLP_CLOSE_AMT_Deduction = 0;
                        decimal WLPBalUpdate = 0;
                        decimal WLP_Balance = 0;
                        var WMP_Balance = await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == mastinfo.UNDER_WHITE_LEVEL);
                        decimal.TryParse(WMP_Balance.BALANCE.ToString(), out WLP_Balance);
                        var WLPAMt = await db.TBL_ACCOUNTS.Where(x => x.MEM_ID == mastinfo.UNDER_WHITE_LEVEL).OrderByDescending(z => z.ACC_NO).FirstOrDefaultAsync();
                        if (WLPAMt != null)
                        {
                            WLP_CLOSE = WLPAMt.CLOSING;
                            WLP_CLOSE_AMT_Deduction = WLP_CLOSE - Mer_BENE_VRY_AMT;
                        }
                        else
                        {
                            WLP_CLOSE = WLPAMt.CLOSING;
                            WLP_CLOSE_AMT_Deduction = WLP_Balance - Mer_BENE_VRY_AMT;
                        }
                        WLPBalUpdate = WLP_Balance - Mer_BENE_VRY_AMT;
                        getRecipientInfo.ISVERIFIED = 1;
                        getRecipientInfo.WLP_GST_OUTPUT = GSTCALCULATED;
                        getRecipientInfo.MER_GST_INPUT = GSTCALCULATED;
                        getRecipientInfo.TIMESTAMP = DateTime.Now;
                        getRecipientInfo.VERIFY_BENE_CHARGE = Mer_BENE_VRY_AMT;
                        getRecipientInfo.RETURN_BACK_TO_CUST_CHARGE = CUST_BENE_VRY_AMT;
                        db.Entry(getRecipientInfo).State = System.Data.Entity.EntityState.Modified;
                        customerInfo.TRANSACTIONLIMIT = subWaltamt;
                        db.Entry(customerInfo).State = System.Data.Entity.EntityState.Modified;
                        mastinfo.BALANCE = UpdateMer_Bal;
                        db.Entry(mastinfo).State = System.Data.Entity.EntityState.Modified;

                        TBL_ACCOUNTS objMERacnt = new TBL_ACCOUNTS()
                        {
                            API_ID = 0,
                            MEM_ID = CurrentMerchant.MEM_ID,
                            MEMBER_TYPE = "RETAILER",
                            TRANSACTION_TYPE = "Bank Account Verification",
                            TRANSACTION_DATE = DateTime.Now,
                            TRANSACTION_TIME = DateTime.Now,
                            DR_CR = "DR",
                            //AMOUNT = RecipientverifyAmt,
                            AMOUNT = Mer_BENE_VRY_AMT,
                            NARRATION = "Amount Deduction on Bank Account Verification",
                            OPENING = closingAmt,
                            CLOSING = deductedamt,
                            REC_NO = 0,
                            COMM_AMT = 0,
                            GST = 0,
                            TDS = 0,
                            IPAddress = "",
                            SERVICE_ID = 0,
                            CORELATIONID = sRandomOTP
                        };
                        db.TBL_ACCOUNTS.Add(objMERacnt);

                        TBL_ACCOUNTS WLPObj = new TBL_ACCOUNTS()
                        {
                            API_ID = 0,
                            MEM_ID = (long)mastinfo.UNDER_WHITE_LEVEL,
                            MEMBER_TYPE = "WHITE LEVEL",
                            TRANSACTION_TYPE = "Bank Account Verification",
                            TRANSACTION_DATE = DateTime.Now,
                            TRANSACTION_TIME = DateTime.Now,
                            DR_CR = "DR",
                            //AMOUNT = RecipientverifyAmt,
                            AMOUNT = Mer_BENE_VRY_AMT,
                            NARRATION = "Amount Deduction on Bank Account Verification",
                            OPENING = WLP_CLOSE,
                            CLOSING = WLP_CLOSE_AMT_Deduction,
                            REC_NO = 0,
                            COMM_AMT = 0,
                            GST = 0,
                            TDS = 0,
                            IPAddress = "",
                            SERVICE_ID = 0,
                            CORELATIONID = sRandomOTP
                        };
                        db.TBL_ACCOUNTS.Add(WLPObj);
                        await db.SaveChangesAsync();
                        WMP_Balance.BALANCE = WLPBalUpdate;
                        db.Entry(WMP_Balance).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        return Json(new { Result = "true" });

                    }
                    else
                    {
                        return Json(new { Result = "Varified" });
                    }
                }
                else
                {
                    return Json(new { Result = "Varified" });
                }
            }
            catch (Exception ex)
            {
                var msg = "Something is going worng";
                return Json(msg);
                Logger.Error("Controller:-  GetCustomerInformation(Merchant DMR), method:- RecipientEnquiry(POST) Line No:- 378", ex);
                throw ex;
            }

        }

        public ActionResult GetRecipient()
        {
            initpage();
            return View();
        }
        public ActionResult MoneyTransfer(string CustId="")
        {
            initpage();
            try
            {
                if (CustId != "")
                {
                    ViewBag.GoogleCapchaInt = TransXT_DMR_API.CaptchaSecretKey;
                    string RecipientId = Decrypt.DecryptMe(CustId);
                    var db = new DBContext();
                    var CustomerInfo = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
                    var recipentInfo = db.TBL_DMR_RECIPIENT_DETAILS.Where(x => x.RECIPIENT_ID == RecipientId).FirstOrDefault();
                    //var recipentInfo = db.TBL_DMR_RECIPIENT_DETAILS.Where(x => x.RECIPIENT_ID == "3034264").FirstOrDefault();
                    TransXTDMR_Transaction objTransxt = new TransXTDMR_Transaction();
                    objTransxt.customerId = recipentInfo.CUSTOMER_ID;
                    objTransxt.recSeqId = RecipientId;
                    //objTransxt.recSeqId = "3034264";
                    objTransxt.RecipientName = recipentInfo.BENEFICIARY_NAME;
                    objTransxt.RecipientMobileNo = recipentInfo.BENEFICIARY_MOBILE;
                    objTransxt.RecipientAccountNo = recipentInfo.ACCOUNT_NO;
                    objTransxt.RecipientIFSCCode = recipentInfo.IFSC_CODE;
                    objTransxt.SenderMobileNo = CustomerInfo.MEMBER_MOBILE;
                    objTransxt.SenderName = CustomerInfo.MEMBER_NAME;
                    return View(objTransxt);
                }
                else
                {
                    return RedirectToAction("Index", "MerchantDMRSection", new { area = "Merchant" });
                }   
                
            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return View();
                Logger.Error("Controller:-  GetCustomerInformation(Merchant DMR), method:- MoneyTransfer(POST) Line No:- 415", ex);
                throw ex;
            }
        }
        public static CaptchaResponse ValidateCaptcha(string response)
        {
            string secret = System.Web.Configuration.WebConfigurationManager.AppSettings["recaptchaPrivateKey"];
            var client = new WebClient();
            var jsonResult = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));
            return JsonConvert.DeserializeObject<CaptchaResponse>(jsonResult.ToString());
        }
        public static string GetUniqueKey(string userID)
        {
            string resultString = Regex.Match(userID, @"\d+").Value;

            long ticks = DateTime.Now.Ticks;
            string result = resultString + ticks.ToString();
            return result.ToString();
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<JsonResult> PostTransferAmountToRecipient(TransXTDMR_Transaction objtrns)
        {
            //initpage();
            string valuemsg = string.Empty;
            try
            {
                //CaptchaResponse response = ValidateCaptcha(Request["g-recaptcha-response"]);
                //if (response.Success && ModelState.IsValid)

                string IpAddress = string.Empty;
                if (objtrns.IpAddress != null)
                {
                    IpAddress = objtrns.IpAddress.Replace("\"", "");
                }
                else
                {
                    IpAddress = "";
                }

                bool trigger = true;
                if (trigger == true)
                {
                    decimal balAmt = decimal.Parse(objtrns.amount);
                    var db = new DBContext();
                    var mem = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).FirstOrDefault();
                    #region Checking Wallet Amount with Transaction amt with 1 % Transaction charge
                    decimal Transactionamt = decimal.Parse(objtrns.amount);
                    string DMRGSTPerValue = System.Configuration.ConfigurationManager.AppSettings["DMRGSTPERCENTAGEComm"];
                    decimal DMRFixedGST = 0;
                    decimal.TryParse(DMRGSTPerValue, out DMRFixedGST);
                    decimal Tranacharge = (Transactionamt * DMRFixedGST) / 100;
                    decimal TotalTransAMt = Transactionamt + Tranacharge;
                    #endregion
                    var AvailableBalcheck = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).Select(z => z.BALANCE).FirstOrDefaultAsync();
                    if (AvailableBalcheck >= TotalTransAMt)
                    {
                        var getWhiteLevelID = await (from mrnt in db.TBL_MASTER_MEMBER
                                                     join dist in db.TBL_MASTER_MEMBER on mrnt.INTRODUCER equals dist.MEM_ID
                                                     //join super in db.TBL_MASTER_MEMBER on dist.INTRODUCER equals super.MEM_ID
                                                     join WBL in db.TBL_MASTER_MEMBER on dist.UNDER_WHITE_LEVEL equals WBL.MEM_ID
                                                     where mrnt.MEM_ID == CurrentMerchant.MEM_ID
                                                     select new
                                                     {
                                                         WHTBalance = WBL.BALANCE
                                                     }).FirstOrDefaultAsync();
                        if (getWhiteLevelID.WHTBalance >= TotalTransAMt)
                        {
                            //var checkbalance = TransXT_DMR_API.FetchCustomer(objtrns.customerId);
                            //decimal balAvailable = 0;
                            //decimal totalMonthlyLimitValue = 0;
                            //var AvailBal = checkbalance.response.walletbal.Value;
                            //var totalMonthlyLimit = checkbalance.response.totalMonthlyLimit.Value;
                            //decimal.TryParse(AvailBal.ToString(), out balAvailable);

                            //totalMonthlyLimit = Convert.ToDecimal(totalMonthlyLimit);

                            //if (balAvailable <= totalMonthlyLimit)
                            //{
                            string TransAmount = string.Empty;
                            TransAmount = objtrns.amount + ".00";
                            long merchantid = 0;
                            long.TryParse(Session["MerchantUserId"].ToString(), out merchantid);
                            decimal Trans_AmountVal = 0;
                            decimal.TryParse(TransAmount, out Trans_AmountVal);

                            string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
                            CommissionDistributionHelper objChkComm = new CommissionDistributionHelper();
                            bool ComCheck = await objChkComm.DeductAmountFromMerchant(merchantid, "DMR", Transactionamt, Tranacharge, Tranacharge, "DMR", "Money Transfer", IpAddress, sRandomOTP, objtrns.recSeqId, objtrns.customerId, "DMT");
                            if (ComCheck == true)
                            {
                                decimal AllAmount = 0;
                                string AddAmt = "4.32";
                                AllAmount =Math.Round(((Convert.ToDecimal(objtrns.amount) + Convert.ToDecimal(AddAmt))),2);
                                string Amount_ALL = AllAmount.ToString();
                                #region CyberPlat Money Transfer Code
                                string SessionNoObj = DateTime.Parse(DateTime.Now.ToString()).ToString("dddMMMddyyyyHHmmss");
                                string OutputResponse = string.Empty;
                                string URL_ValidateRemitter = "https://in.cyberplat.com/cgi-bin/instp/instp_pay_check.cgi";
                                string keyPath = Server.MapPath("~/Test CERT/myprivatekey.pfx");
                                var PaymentValidation_AddRem = CyberPlatDMTAPICall._strFundTransfer(SessionNoObj, "3", objtrns.RecipientMobileNo, objtrns.PaymentMode, objtrns.BeneficiaryId, objtrns.amount, Amount_ALL);
                                ssl.message = ssl.Sign_With_PFX(PaymentValidation_AddRem, keyPath, "rahul123");
                                ssl.htmlText = ssl.CallCryptoAPI(ssl.message, URL_ValidateRemitter);
                                OutputResponse = "URL:\r\n" + URL_ValidateRemitter + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;
                                string Out_response = ssl.htmlText;
                                string[] response_Value = ParseReportCode(Out_response);
                                string ReturnOutpot = response_Value[5];
                                string ErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(ReturnOutpot);

                                string CretRemitterRes = string.Empty;
                                var RemitterPaymentValidation = CyberPlatDMTAPICall._strFundTransfer(SessionNoObj, "3", objtrns.RecipientMobileNo, objtrns.PaymentMode, objtrns.BeneficiaryId, objtrns.amount, Amount_ALL);
                                ssl.message = ssl.Sign_With_PFX(RemitterPaymentValidation, keyPath, "rahul123");
                                ssl.htmlText = ssl.CallCryptoAPI(ssl.message, "https://in.cyberplat.com/cgi-bin/instp/instp_pay.cgi");
                                CretRemitterRes = "URL:\r\n" + URL_ValidateRemitter + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;

                                string FinalOut_response = ssl.htmlText;
                                string[] Final_response_Value = ParseReportCode(FinalOut_response);
                                string FinalOutpot = Final_response_Value[5];
                                if (FinalOutpot == "224")
                                {
                                    string malue_msg = "Invalid Beneficiaryid. Or Invalid Account Number. Or Invalid Beneficiary IFSC";
                                    return Json(malue_msg, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    string Final_ErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(FinalOutpot);
                                    string returnUrl = Server.UrlDecode(Final_response_Value[8]);
                                    var Objsettng = returnUrl;
                                    JObject json = JObject.Parse(Objsettng);
                                    var ResponseResult = JObject.Parse(Objsettng);
                                    dynamic Doremit = ResponseResult;
                                    string resstring = Convert.ToString(ResponseResult);
                                    var errorcode = Doremit.statuscode;

                                    //var Doremit = MoneyTransferAPI.FundTransfer(objtrns.customerId, objtrns.BeneficiaryId, agentId, TransAmount.ToString(), objtrns.PaymentMode);
                                    //var Doremit = TransXT_DMR_API.TransactiondoRemit(objtrns.recSeqId, objtrns.customerId, TransAmount, sRandomOTP, "INR", "1", mem.MEMBER_MOBILE, mem.MEMBER_NAME, objtrns.RecipientMobileNo, objtrns.RecipientName, objtrns.RecipientAccountNo, objtrns.RecipientIFSCCode);
                                    valuemsg = JsonConvert.SerializeObject(Doremit);
                                    //var errmsg = Doremit.errorMsg.Value;
                                    var errmsg = Doremit.statuscode.Value;

                                    if (errmsg == "TXN")
                                    {
                                        long refno = 0;
                                        long.TryParse(Doremit.data.ref_no.Value.ToString(), out refno);
                                        var Ref_number= Doremit.data.ref_no;
                                        string RefNOVal = Convert.ToString(Ref_number);
                                        double Servicetax = 0;
                                        double.TryParse(Doremit.data.charged_amt.Value.ToString(), out Servicetax);
                                        double amount_Outpoy = 0;
                                        double.TryParse(Doremit.data.amount.Value.ToString(), out amount_Outpoy);
                                        string Serv_amt = Servicetax.ToString();
                                        string Amt_vvAl = amount_Outpoy.ToString();
                                        string Clientrefno = Doremit.data.ipay_id.Value;
                                        string initiatorId = Doremit.ipay_uuid.Value;
                                        string txnStatus = Doremit.status.Value;
                                        string BenNameVal = Doremit.data.name.Value;
                                        //string IfscCode = Doremit.data.ipay_id.Value;
                                        string AcntNo= objtrns.RecipientAccountNo;
                                        string IfscCode = objtrns.RecipientIFSCCode;
                                        string impsRespCode = Doremit.statuscode.Value;
                                        string impsRespMessage = Doremit.status.Value;
                                        string txnId_Val = Doremit.orderid.Value;
                                        string timestamp = Doremit.status.Value;
                                        DMR_API_Response objdmrResp = new DMR_API_Response()
                                        {
                                            serviceTax = Serv_amt,
                                            clientRefId = Clientrefno,
                                            fee = Amt_vvAl,
                                            //initiatorId = initiatorId,
                                            initiatorId = RefNOVal,
                                            accountNumber = AcntNo,
                                            txnStatus = txnStatus,
                                            name = BenNameVal,
                                            ifscCode = IfscCode,
                                            impsRespCode = impsRespCode,
                                            impsRespMessage = impsRespMessage,
                                            txnId = txnId_Val,
                                            timestamp = timestamp
                                            //serviceTax = Doremit.response.serviceTax.Value,
                                            //clientRefId = Doremit.response.clientRefId.Value,
                                            //fee = Doremit.response.fee.Value,
                                            //initiatorId = Doremit.response.initiatorId.Value,
                                            //accountNumber = Doremit.response.accountNumber.Value,
                                            //txnStatus = Doremit.response.txnStatus.Value,
                                            //name = Doremit.response.name.Value,
                                            //ifscCode = Doremit.response.ifscCode.Value,
                                            //impsRespCode = Doremit.response.impsRespCode.Value,
                                            //impsRespMessage = Doremit.response.impsRespMessage.Value,
                                            //txnId = Doremit.response.txnId.Value,
                                            //timestamp = Doremit.response.timestamp.Value
                                            //serviceTax = "0",
                                            //clientRefId = "W13719201171222669",
                                            //fee = "00",
                                            //initiatorId = "4",
                                            //accountNumber = "31923879504",
                                            //txnStatus = "00",
                                            //name = "Mr  RAHUL  SHARMA",
                                            //ifscCode = "SBIN0001899",
                                            //impsRespCode = "00",
                                            //impsRespMessage = "00",
                                            //txnId = "920117665902",
                                            //timestamp = "20/07/2019 17:16:10"
                                        };

                                        CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                                        bool checkComm = await objComm.AllMemberCommissionDistribution(merchantid, "DMR", Trans_AmountVal, Trans_AmountVal, Trans_AmountVal, "DMR", "Money Transfer", IpAddress, sRandomOTP, objdmrResp, valuemsg);
                                        if (checkComm == true)
                                        {

                                            return Json("Amount transfer successfully.", JsonRequestBehavior.AllowGet);
                                        }
                                        else
                                        {
                                            return Json("Transaction Failed");
                                        }
                                        //if (errmsg == "SUCCESS")
                                        //{
                                        //    return Json("Amount transfer successfully.", JsonRequestBehavior.AllowGet);
                                        //}
                                        //else
                                        //{
                                        //    //return Json(errmsg, JsonRequestBehavior.AllowGet);
                                        //    var errmsgVal = Doremit.status.Value;
                                        //    return Json(errmsgVal, JsonRequestBehavior.AllowGet);
                                        //}
                                    }
                                    else
                                    {
                                        string Statusval = string.Empty;
                                        CommissionDistributionHelper objComm = new CommissionDistributionHelper();

                                        DMR_API_Response objdmrResp = null;
                                        Statusval = objComm.RefundCommissionInWallet(sRandomOTP, "DMR", "");


                                        if (Statusval == "Return Success")
                                        {
                                            var errmsgVal = Doremit.status.Value;
                                            return Json(errmsgVal, JsonRequestBehavior.AllowGet);
                                        }
                                        else
                                        {
                                            return Json("Try again after sometime.", JsonRequestBehavior.AllowGet);
                                        }
                                    }
                                }
                                

                                #endregion

                            }
                            else
                            {
                                return Json("Transaction Failed. Plerase try again after sometime.", JsonRequestBehavior.AllowGet);
                            }
                            //}
                            //else
                            //{
                            //    string msg = "This customer id:-" + objtrns.customerId + " has crossed the limit. Please add new customer Id.";
                            //    return Json(msg, JsonRequestBehavior.AllowGet);
                            //}
                        }
                        else
                        {
                            string msg = "Insufficient amount in White Lable.";
                            return Json(msg, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        string msg = "Insuffient balance in Merchant Wallet.";
                        return Json(msg, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    string msg = "Your Google reCaptcha validation failed";
                    return Json(msg, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var msg = valuemsg;
                return Json(msg);
                Logger.Error("Controller:-  GetCustomerInformation(Merchant DMR), method:- PostTransferAmountToRecipient(POST) Line No:- 494", ex);
                throw ex;
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

        public ActionResult MoneyTransferList()
        {
            initpage();
            if (Session["MerchantUserId"] != null)
            {
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
        [HttpGet]
        public PartialViewResult GetAllTransactionLIst()
        {
            try
            {
                // Only grid query values will be available here.
                var db = new DBContext();
                var BeneficiaryList = db.TBL_TRANSXT_DMR_TRANSACTION_LIST.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).ToList();
                return PartialView("GetAllTransactionLIst", BeneficiaryList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult GEtAll_TransactionInformation()
        {
            initpage();
            try
            {
                var db = new DBContext();
                //var list = TransXT_DMR_API.SearchTransaction("", "O91519096064152720");
                //var list = TransXT_DMR_API.SearchTransaction("", "");
                var Transactionlist = db.TBL_DMR_TRANSACTION_LOGS.Where(x => x.MER_ID == CurrentMerchant.MEM_ID && x.TXN_ID!="").ToList().OrderByDescending(z=>z.TRANSACTION_DATE);
                
                return Json(Transactionlist, JsonRequestBehavior.AllowGet);
                
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        //public ActionResult CustomerDetails(string CustId="")
        public ActionResult CustomerDetails()
        {
            initpage();
            try
            {
                string CustId = Session["DMRMobileNo"].ToString();
                if (CustId != "")
                {
                    var db = new DBContext();
                    //var FetchCustomer11 = TransXT_DMR_API.FetchCustomer(CustId);
                    //var CustomerWallet = FetchCustomer11.response.walletbal.Value;
                    var datainfo = db.TBL_DMR_CUSTOMER_DETAILS.Where(x => x.CUSTOMER_MOBILE == CustId).FirstOrDefault();
                    //var info = db.TBL_DMR_CUSTOMER_DETAILS.Where(x => x.CUSTOMER_MOBILE == CustId).FirstOrDefault();
                    //info.TRANSACTIONLIMIT = decimal.Parse(datainfo.TRANSACTIONLIMIT);
                    //db.Entry(info).State = System.Data.Entity.EntityState.Modified;
                    //db.SaveChanges();
                    //var checkbalance = TransXT_DMR_API.FetchCustomer(CustId);

                    decimal remaining_limit = 0;
                    decimal.TryParse(datainfo.REMAINING_LIMIT.ToString(),out remaining_limit);

                    decimal Consumend_limit = 0;
                    decimal.TryParse(datainfo.CONSUMED_LIMIT.ToString(), out Consumend_limit);

                    decimal balAvailable = 0;
                    var AvailBal = datainfo.TRANSACTIONLIMIT;
                    decimal.TryParse(AvailBal.ToString(), out balAvailable);
                    TransXT_ADDCustomerModal objCustdetails = new TransXT_ADDCustomerModal();
                    objCustdetails.MobileNumber = datainfo.CUSTOMER_MOBILE;
                    objCustdetails.CustomerName = datainfo.CUSTOMER_NAME;
                    objCustdetails.Address = datainfo.ADDRESS;
                    objCustdetails.TRANSACTIONLIMIT = balAvailable;
                    objCustdetails.Remaining_Limit = remaining_limit;
                    objCustdetails.Consumed_Limit = Consumend_limit;
                    objCustdetails.DOB = null;
                    return View(objCustdetails);
                }
                else
                {
                    return RedirectToAction("Index", "MerchantDMRSection", new { area = "Merchant" });
                }
                return View();
            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return View();
                Logger.Error("Controller:-  GetCustomerInformation(Merchant DMR), method:- CustomerDetails(POST) Line No:- 588", ex);
                throw ex;
            }            //}
            //catch (Exception ex)
            //{
            //    var msg = "Please try again after 15 minute";
            //    return View();
            //    Logger.Error("Controller:-  GetCustomerInformation(Merchant DMR), method:- CustomerDetails(POST) Line No:- 588", ex);
            //    throw ex;
            //}
        }
        public ActionResult PrintDMRInvoice(string txnid,string RefClientid)
        {
            return View();
        }


        public ActionResult SearchDMRCustomer()
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
        [HttpPost]
        public async Task<JsonResult> GetDMRCustomerInformation(GetDMRCustomerInfo objsub)
        {
            try
            {
                var db = new DBContext();
                string SessionNoObj = DateTime.Parse(DateTime.Now.ToString()).ToString("dddMMMddyyyyHHmmss");
                string OutputResponse = string.Empty;
                string URL_ValidateRemitter = "https://in.cyberplat.com/cgi-bin/instp/instp_pay_check.cgi";
                string keyPath = Server.MapPath("~/Test CERT/myprivatekey.pfx");
                var PaymentValidation = CyberPlatDMTAPICall._strGetRemitterDetails(SessionNoObj, "5", objsub.CustomerMobileNo, "1.00");
                ssl.message = ssl.Sign_With_PFX(PaymentValidation, keyPath, "rahul123");
                ssl.htmlText = ssl.CallCryptoAPI(ssl.message, URL_ValidateRemitter);
                OutputResponse = "URL:\r\n" + URL_ValidateRemitter + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;
                string Out_response = ssl.htmlText;
                string[] response_Value = ParseReportCode(Out_response);
                string ReturnOutpot = response_Value[4];
                string ErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(ReturnOutpot);


                string FinalOutput = string.Empty;
                string FinalCheckURL_Remitter = "https://in.cyberplat.com/cgi-bin/instp/instp_pay.cgi";
                var FinalRemitterCheck = CyberPlatDMTAPICall._strGetRemitterDetails(SessionNoObj, "5", objsub.CustomerMobileNo, "1.00");
                ssl.message = ssl.Sign_With_PFX(FinalRemitterCheck, keyPath, "rahul123");
                ssl.htmlText = ssl.CallCryptoAPI(ssl.message, FinalCheckURL_Remitter);
                FinalOutput = "URL:\r\n" + FinalCheckURL_Remitter + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;
                string FinalOut_response = ssl.htmlText;
                string[] Final_response_Value = ParseReportCode(FinalOut_response);
                string FinalOutpot = Final_response_Value[3];
                string Final_ErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(FinalOutpot);
                if (FinalOutpot == "224")
                {
                    Session["DMRMobileNo"] = objsub.CustomerMobileNo;
                    string OutPutMsg_VAl = string.Empty;
                    string Error_msg = Final_response_Value[6];
                    if (Error_msg == "Remitter")
                    {
                        OutPutMsg_VAl = "Remitter Not Found";
                    }
                    else
                    {
                        OutPutMsg_VAl = "Remitter Not Found";
                    }
                    return Json(OutPutMsg_VAl, JsonRequestBehavior.AllowGet);
                }
                else if (FinalOutpot == "0")
                {
                    Session["DMRMobileNo"] = objsub.CustomerMobileNo;

                    string returnUrl = Server.UrlDecode(Final_response_Value[5]);
                    //var Objsettng = returnUrl.Remove(0, 8);
                    var Objsettng = returnUrl;
                    JObject json = JObject.Parse(Objsettng);
                    var ResponseResult = JObject.Parse(Objsettng);
                    dynamic FinalResp = ResponseResult;
                    var Oouputmessage = FinalResp.status;
                    if (Oouputmessage == "OTP sent successfully")
                    {
                        var statusmsg = FinalResp.statuscode;
                        string MsgDetails = FinalResp.status;
                        string RemtID = FinalResp.data.remitter.id;
                        return Json(new { msgDetail = MsgDetails, RemitterIDVALUE = RemtID }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var beneficiarylist = FinalResp.data.beneficiary;
                        var remitterInfo = FinalResp.data.remitter;
                        var checkRemitterStatus = await db.TBL_DMR_CUSTOMER_DETAILS.Where(x => x.CUSTOMER_MOBILE == objsub.CustomerMobileNo).FirstOrDefaultAsync();
                        if (checkRemitterStatus != null)
                        {
                            string Add = remitterInfo.address + "," + remitterInfo.city + "," + remitterInfo.pincode + "," + remitterInfo.state;
                            decimal REMAINING_LIMIT = Convert.ToDecimal(remitterInfo.remaininglimit);
                            decimal CONSUMED_LIMIT = Convert.ToDecimal(remitterInfo.consumedlimit);
                            checkRemitterStatus.REMAINING_LIMIT = REMAINING_LIMIT;
                            checkRemitterStatus.CONSUMED_LIMIT = CONSUMED_LIMIT;
                            checkRemitterStatus.ADDRESS = Add;
                            checkRemitterStatus.CREATED_DATE = DateTime.Now;
                            db.Entry(checkRemitterStatus).State = System.Data.Entity.EntityState.Modified;
                            await db.SaveChangesAsync();
                        }
                        else
                        {
                            string RemitterId = remitterInfo.id;
                            decimal REMAINING_LIMIT = Convert.ToDecimal(remitterInfo.remaininglimit);
                            decimal CONSUMED_LIMIT = Convert.ToDecimal(remitterInfo.consumedlimit);
                            decimal TRANSACTION_LIMIT = Convert.ToDecimal(remitterInfo.perm_txn_limit);
                            string CustomerName = remitterInfo.name;
                            string CustomerMobile = remitterInfo.mobile;
                            int IsVarified = 0;
                            string varifiedIs = remitterInfo.is_verified;
                            int.TryParse(varifiedIs, out IsVarified);
                            string Add = remitterInfo.address + "," + remitterInfo.city + "," + remitterInfo.pincode + "," + remitterInfo.state;
                            string RemitterRes = Convert.ToString(remitterInfo);
                            TBL_DMR_CUSTOMER_DETAILS objcust_Value = new TBL_DMR_CUSTOMER_DETAILS()
                            {
                                MEM_ID = CurrentMerchant.MEM_ID,
                                CUSTOMER_NAME = CustomerName,
                                CUSTOMER_MOBILE = CustomerMobile,
                                ADDRESS = Add,
                                DOB = "",
                                CREATED_DATE = DateTime.Now,
                                STATUS = IsVarified,
                                REMITTER_ID = RemitterId,
                                API_RESPONSE = RemitterRes,
                                REMAINING_LIMIT = REMAINING_LIMIT,
                                CONSUMED_LIMIT = CONSUMED_LIMIT,
                                TRANSACTIONLIMIT = TRANSACTION_LIMIT
                            };
                            db.TBL_DMR_CUSTOMER_DETAILS.Add(objcust_Value);
                            await db.SaveChangesAsync();
                        }
                        foreach (var listitem in beneficiarylist)
                        {
                            string BenefiRes = Convert.ToString(beneficiarylist);
                            string beneid = listitem.id.Value;
                            string beneAcntid = listitem.account.Value;
                            var benelist = await db.TBL_DMR_RECIPIENT_DETAILS.Where(x => x.ACCOUNT_NO == beneAcntid).FirstOrDefaultAsync();
                            if (benelist != null)
                            {
                                benelist.RECIPIENT_ID = checkRemitterStatus.REMITTER_ID;
                                benelist.ACCOUNT_NO = listitem.account.Value;
                                benelist.BENEFICIARY_MOBILE = listitem.mobile.Value;
                                benelist.IFSC_CODE = listitem.ifsc.Value;
                                benelist.API_RESPONSE = BenefiRes;
                                benelist.BENEFICIARY_ID = listitem.id.Value;
                                benelist.CREATE_DATE = DateTime.Now;
                                db.Entry(benelist).State = System.Data.Entity.EntityState.Modified;
                                await db.SaveChangesAsync();
                            }
                            else
                            {
                                string BenefiResVal = Convert.ToString(beneficiarylist);
                                string BeneName = listitem.name.Value;
                                string mobile = listitem.mobile.Value;
                                string account = listitem.account.Value;
                                string bank = listitem.bank.Value;
                                string status = listitem.status.Value;
                                string last_success_date = listitem.last_success_date.Value;
                                string last_success_name = listitem.last_success_name.Value;
                                string last_success_imps = listitem.last_success_imps.Value;
                                string ifsc = listitem.ifsc.Value;
                                string imps = listitem.imps.Value;
                                string BeneId = listitem.id.Value;
                                TBL_DMR_RECIPIENT_DETAILS objremben = new TBL_DMR_RECIPIENT_DETAILS()
                                {
                                    RECIPIENT_ID = objsub.CustomerMobileNo,
                                    MEM_ID = CurrentMerchant.MEM_ID,
                                    CUSTOMER_ID = objsub.CustomerMobileNo,
                                    BENEFICIARY_MOBILE = mobile,
                                    BENEFICIARY_NAME = BeneName,
                                    ACCOUNT_NO = account,
                                    IFSC_CODE = ifsc,
                                    BENEFICIARY_TYPE = "",
                                    CREATE_DATE = DateTime.Now,
                                    STATUS = Convert.ToInt32(status),
                                    TRANSACTIONID = "",
                                    ISVERIFIED = Convert.ToInt32(status),
                                    EMAIL_ID = "test@test.com",
                                    REMARKS = "",
                                    WLP_ID = 0,
                                    SUPER_ID = CurrentMerchant.UNDER_WHITE_LEVEL,
                                    DIST_ID = CurrentMerchant.INTRODUCER,
                                    VERIFY_BENE_CHARGE = 0,
                                    RETURN_BACK_TO_CUST_CHARGE = 0,
                                    TIMESTAMP = DateTime.Now,
                                    CORELATION_ID = "",
                                    WLP_GST_INPUT = 0,
                                    WLP_GST_OUTPUT = 0,
                                    MER_GST_INPUT = 0,
                                    MER_GST_OUTPUT = 0,
                                    API_RESPONSE = BenefiResVal,
                                    BENEFICIARY_ID = BeneId
                                };
                                db.TBL_DMR_RECIPIENT_DETAILS.Add(objremben);
                                await db.SaveChangesAsync();
                            }
                        }

                        return Json(new { msgDetail = "Transaction Successfully", RemitterIDVALUE = "0" }, JsonRequestBehavior.AllowGet);
                    }

                    //return Json("Transaction Successfully", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { msgDetail = "Transaction Successfully", RemitterIDVALUE = "0" }, JsonRequestBehavior.AllowGet);
                    //return Json("Transaction Successfully", JsonRequestBehavior.AllowGet);
                }









                //var RemitterPaymentValidation = CyberPlatDMTAPICall._strGetRemitterRegistration(SessionNo, "0","9903116214","Sharma","Rahul","700119", "1.00", "1.00");
                //ssl.message = ssl.Sign_With_PFX(RemitterPaymentValidation, keyPath, "rahul123");
                //ssl.htmlText = ssl.CallCryptoAPI(ssl.message, "https://in.cyberplat.com/cgi-bin/instp/instp_pay_check.cgi");
                //OutputResponse = "URL:\r\n" + URL_ValidateRemitter + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;




                //const string agentId = "2";
                ////string OperatorName = Request.Form["OperatorName"];
                ////string operatorId = Request.Form["OperatorId"];
                //var db = new DBContext();
                //var GetOutletId = db.TBL_MERCHANT_OUTLET_INFORMATION.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID).OUTLETID;
                ////var PaymentValidationtest = MoneyTransferAPI.RemitterDetails(objsub.CustomerMobileNo, GetOutletId);
                ////if (PaymentValidationtest.status == "OTP sent successfully" && PaymentValidationtest.statuscode == "TXN")
                ////{
                ////    Session["DMRMobileNo"] = objsub.CustomerMobileNo;
                ////    return Json("Data not found in database", JsonRequestBehavior.AllowGet);
                ////}
                ////else
                ////{
                ////var checkloginStatus = await db.TBL_DMR_REMITTER_INFORMATION.Where(x => x.MobileNo == objsub.CustomerMobileNo).FirstOrDefaultAsync();
                //var checkloginStatus = await db.TBL_DMR_CUSTOMER_DETAILS.Where(x => x.CUSTOMER_MOBILE == objsub.CustomerMobileNo).FirstOrDefaultAsync();
                //if (checkloginStatus!=null)
                ////if (checkloginStatus.STATUS == 0)
                //{
                //    Session["DMRMobileNo"] = objsub.CustomerMobileNo;
                //    var PaymentValidation = MoneyTransferAPI.RemitterDetails(objsub.CustomerMobileNo, GetOutletId);
                //    if (PaymentValidation.statuscode == "TXN")
                //    {
                //        //checkloginStatus.Address = PaymentValidation.data.remitter.address;
                //        //checkloginStatus.City = PaymentValidation.data.remitter.city;
                //        //checkloginStatus.State = PaymentValidation.data.remitter.state;
                //        //checkloginStatus.State = PaymentValidation.data.remitter.state;
                //        //checkloginStatus.KYCStatus = PaymentValidation.data.remitter.kycstatus;
                //        checkloginStatus.TRANSACTIONLIMIT = Convert.ToDecimal(PaymentValidation.data.remitter.remaininglimit);
                //        //checkloginStatus.RemainingLimit = Convert.ToDecimal(PaymentValidation.data.remitter.remaininglimit);
                //        //var limit = PaymentValidation.data.remitter_limit[0];
                //        //var limitTotal = limit.limit.total;
                //        //checkloginStatus.Total = Convert.ToDecimal(limitTotal);
                //        //checkloginStatus.KYCDocs = PaymentValidation.data.remitter.kycdocs;
                //        //checkloginStatus.Perm_txn_limit = Convert.ToDecimal(PaymentValidation.data.remitter.perm_txn_limit);
                //        //checkloginStatus.UpdateStatus = 1;
                //        db.Entry(checkloginStatus).State = System.Data.Entity.EntityState.Modified;
                //        await db.SaveChangesAsync();
                //        string RemitterId = PaymentValidation.data.remitter.id.Value;
                //        var beneficiarylist = PaymentValidation.data.beneficiary;
                //        foreach (var listitem in beneficiarylist)
                //        {
                //            string resstring = Convert.ToString(listitem);
                //            string beneid = listitem.id.Value;
                //            //var benelist = await db.TBL_REMITTER_BENEFICIARY_INFO.Where(x => x.BeneficiaryID == beneid).FirstOrDefaultAsync();
                //            var benelist = await db.TBL_DMR_RECIPIENT_DETAILS.Where(x => x.BENEFICIARY_ID == beneid).FirstOrDefaultAsync();
                //            if (benelist != null)
                //            {
                //                benelist.RECIPIENT_ID = RemitterId;
                //                benelist.ACCOUNT_NO = listitem.account.Value;
                //                benelist.BENEFICIARY_MOBILE = listitem.mobile.Value;
                //                benelist.IFSC_CODE = listitem.ifsc.Value;
                //                benelist.API_RESPONSE = resstring;
                //                benelist.BENEFICIARY_ID = listitem.id.Value;
                //                db.Entry(benelist).State = System.Data.Entity.EntityState.Modified;
                //                await db.SaveChangesAsync();
                //            }
                //            else
                //            {
                //                string BeneName = listitem.name.Value;
                //                string mobile = listitem.mobile.Value;
                //                string account = listitem.account.Value;
                //                string bank = listitem.bank.Value;
                //                string status = listitem.status.Value;
                //                string last_success_date = listitem.last_success_date.Value;
                //                string last_success_name = listitem.last_success_name.Value;
                //                string last_success_imps = listitem.last_success_imps.Value;
                //                string ifsc = listitem.ifsc.Value;
                //                string imps = listitem.imps.Value;
                //                string BeneId = listitem.id.Value;
                //                TBL_DMR_RECIPIENT_DETAILS objremben = new TBL_DMR_RECIPIENT_DETAILS()
                //                {
                //                    RECIPIENT_ID = RemitterId,
                //                    MEM_ID=CurrentMerchant.MEM_ID,
                //                    CUSTOMER_ID= objsub.CustomerMobileNo,
                //                    BENEFICIARY_MOBILE= mobile,
                //                    BENEFICIARY_NAME= BeneName,
                //                    ACCOUNT_NO= account,
                //                    IFSC_CODE= ifsc,
                //                    BENEFICIARY_TYPE="",
                //                    CREATE_DATE=DateTime.Now,
                //                    STATUS=Convert.ToInt32(status),
                //                    TRANSACTIONID="",
                //                    ISVERIFIED= Convert.ToInt32(status),
                //                    EMAIL_ID="test@test.com",
                //                    REMARKS="",
                //                    WLP_ID=0,
                //                    SUPER_ID=CurrentMerchant.UNDER_WHITE_LEVEL,
                //                    DIST_ID=CurrentMerchant.INTRODUCER,
                //                    VERIFY_BENE_CHARGE=0,
                //                    RETURN_BACK_TO_CUST_CHARGE=0,
                //                    TIMESTAMP=DateTime.Now,
                //                    CORELATION_ID="",
                //                    WLP_GST_INPUT=0,
                //                    WLP_GST_OUTPUT=0,
                //                    MER_GST_INPUT=0,
                //                    MER_GST_OUTPUT=0,
                //                    API_RESPONSE= resstring,
                //                    BENEFICIARY_ID= BeneId
                //                };
                //                db.TBL_DMR_RECIPIENT_DETAILS.Add(objremben);
                //                //TBL_REMITTER_BENEFICIARY_INFO objremben = new TBL_REMITTER_BENEFICIARY_INFO()
                //                //{
                //                //    BeneficiaryID= beneid,
                //                //    BeneficiaryName=BeneName,
                //                //    Mobile=mobile,
                //                //    Account=account,
                //                //    Bank=bank,
                //                //    IFSC=ifsc,
                //                //    Status=Convert.ToInt32(status),
                //                //    IMPS=Convert.ToInt32(imps),
                //                //    Last_Success_Date= last_success_date,
                //                //    Last_success_Name=last_success_name,
                //                //    Last_Sucess_IMPS=last_success_imps,
                //                //    RemitterID= checkloginStatus.REMITTER_ID,
                //                //    MEM_ID=CurrentMerchant.MEM_ID,
                //                //    IsActive=1,
                //                //    Verification_Status="Approve",
                //                //    BankRefNo="",
                //                //    Ipay_Id=""
                //                //};
                //                //db.TBL_REMITTER_BENEFICIARY_INFO.Add(objremben);
                //                await db.SaveChangesAsync();
                //            }
                //        }
                //        var ipat_id = PaymentValidation.data.remitter.id;


                //        var BankNameList = MoneyTransferAPI.GetBankDetails("", GetOutletId);
                //        var GetAllBankName = BankNameList.data;
                //        var Getbank = db.TBL_BANK_MASTER.Count();
                //        if (Getbank == 0)
                //        {
                //            foreach (var Banklistitem in GetAllBankName)
                //            {
                //                string SLn = Banklistitem.id.Value;
                //                string bankName = Banklistitem.bank_name.Value;
                //                string imps_enabled = Banklistitem.imps_enabled.Value;
                //                string aeps_enabled = Banklistitem.aeps_enabled.Value;
                //                string bank_sort_name = Banklistitem.bank_sort_name.Value;
                //                string branch_ifsc = Banklistitem.branch_ifsc.Value;
                //                string ifsc_alias = Banklistitem.ifsc_alias.Value;
                //                string bank_iin = Banklistitem.bank_iin.Value;
                //                string is_down = Banklistitem.is_down.Value;                                
                //                TBL_BANK_MASTER objbnkmas = new TBL_BANK_MASTER()
                //                {
                //                    SLNO= SLn,
                //                    BankName= bankName,
                //                    BankCode= bank_sort_name,
                //                    IMPS= imps_enabled,
                //                    NEFT= aeps_enabled,
                //                    AccountVerification="No",
                //                    BANK_IFSC= branch_ifsc,
                //                    BANK_IIN= bank_iin,
                //                    IS_DOWN= is_down,
                //                    IFSC_ALIAS= ifsc_alias
                //                };
                //                db.TBL_BANK_MASTER.Add(objbnkmas);
                //                await db.SaveChangesAsync();

                //            }

                //        }


                //            Session["MerchantDMRId"] = ipat_id.Value;
                //        //return Json(PaymentValidation, JsonRequestBehavior.AllowGet);
                //        ////return RedirectToAction("DMRInformation", "MerchantDMRDashboard", new { area = "Merchant" });


                //        return Json("Customer Data is Available", JsonRequestBehavior.AllowGet);
                //    }
                //    else
                //    {
                //        //return Json(PaymentValidation);
                //        ViewBag.Message = "Invalid Credential or Access Denied";
                //        var msg = "Invalid Credential or Access Denied";
                //        return Json(msg);
                //        //return View();
                //    }
                //}
                //else
                //{
                //    Session["DMRMobileNo"] = objsub.CustomerMobileNo;
                //    return Json("Data not found in database", JsonRequestBehavior.AllowGet);
                //    ////Session["MerchantDMRId"] = checkloginStatus.RemitterID;
                //    //var msg = "Please try again after 15 minute";
                //    //return Json(msg);
                //    ////return RedirectToAction("DMRInformation", "MerchantDMRDashboard", new { area = "Merchant" });
                //}
                ////}
            }
            catch (Exception ex)
            {
                //throw ex;
                ViewBag.Message = "Invalid Credential or Access Denied";
                Logger.Error("Controller:-  MerchantDMRLogin(Merchant), method:- Index(POST) Line No:- 133", ex);
                var msg = "Please try again after 15 minute";
                return Json(msg);
                //return View();
            }



            //initpage();
            //try
            //{
            //    var db = new DBContext();
            //    //// Fetch Customer
            //    var FetchCustomer = TransXT_DMR_API.FetchCustomer(objsub.CustomerMobileNo);
            //    Session["DMRMobileNo"] = objsub.CustomerMobileNo;
            //    //var ResponseResult = JObject.Parse(FetchCustomer);
            //    //var data = JsonConvert.SerializeObject(FetchCustomer);
            //    //var FetchCustomer11 = TransXT_DMR_API.FetchCustomer("6290665805");
            //    return Json(FetchCustomer, JsonRequestBehavior.AllowGet);
            //}
            //catch (Exception ex)
            //{
            //    var msg = "Please try again after 15 minute";
            //    return Json(msg);
            //    Logger.Error("Controller:-  GetCustomerInformation(Merchant DMR), method:- GetCustomerInformation(POST) Line No:- 90", ex);
            //    throw ex;
            //}

        }

        [HttpPost]
        public async Task<JsonResult> GetBeneficiaryInformation(string RecipientId, string CustomerId)
        {
            initpage();
            try
            {
                var db = new DBContext();
                //var RecipientInformation = await db.TBL_DMR_RECIPIENT_DETAILS.Where(x => x.RECIPIENT_ID == RecipientId && x.CUSTOMER_ID == CustomerId).FirstOrDefaultAsync();                
                var RecipientInformation = await db.TBL_DMR_RECIPIENT_DETAILS.Where(x => x.RECIPIENT_ID == RecipientId && x.BENEFICIARY_ID == CustomerId).FirstOrDefaultAsync();
                return Json(RecipientInformation,JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return Json(msg);
                Logger.Error("Controller:-  GetCustomerInformation(Merchant DMR), method:- DeleteRecipientInformation(POST) Line No:- 338", ex);
                throw ex;
            }

        }

        [HttpPost]
        public async Task<JsonResult> GetBeneficiaryValidationInformation(string RecipientId, string CustomerId)
        {
            initpage();
            try
            {
                var db = new DBContext();
                //var RecipientInformation = await db.TBL_DMR_RECIPIENT_DETAILS.Where(x => x.RECIPIENT_ID == RecipientId && x.CUSTOMER_ID == CustomerId).FirstOrDefaultAsync();                
                var RecipientInformation = await db.TBL_DMR_RECIPIENT_DETAILS.Where(x => x.RECIPIENT_ID == RecipientId && x.BENEFICIARY_ID == CustomerId).FirstOrDefaultAsync();
                return Json(RecipientInformation, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return Json(msg);
                Logger.Error("Controller:-  GetCustomerInformation(Merchant DMR), method:- DeleteRecipientInformation(POST) Line No:- 338", ex);
                throw ex;
            }

        }

        [HttpPost]
        public JsonResult PrintTransferAmountInvoice(string txnID,string RefId)
        {            
            try
            {
                var db = new DBContext();
                //var list = TransXT_DMR_API.SearchTransaction("", "O91519096064152720");
                var Transactionlist = (from Comp in db.TBL_MASTER_MEMBER join mem in db.TBL_MASTER_MEMBER on Comp.MEM_ID equals mem.UNDER_WHITE_LEVEL
                                       join dmt in db.TBL_DMR_TRANSACTION_LOGS on mem.MEM_ID equals dmt.MER_ID
                                       join Retailer in db.TBL_MASTER_MEMBER on dmt.MER_ID equals Retailer.MEM_ID
                                       join benRef in db.TBL_DMR_RECIPIENT_DETAILS on dmt.RECIPIENT_ID equals benRef.RECIPIENT_ID
                                       join host in db.TBL_WHITE_LEVEL_HOSTING_DETAILS on mem.UNDER_WHITE_LEVEL equals host.MEM_ID
                                       
                                       where dmt.TXN_ID == txnID && dmt.CLIENT_REF_ID == RefId && dmt.MER_ID == CurrentMerchant.MEM_ID
                                       select new
                                       {
                                           CompanyLogo= Comp.LOGO,
                                           CompanyName= (db.TBL_MASTER_MEMBER.Where(c => c.MEM_ID == CurrentMerchant.MEM_ID).Select(d=>d.COMPANY).FirstOrDefault()),
                                           CompanyMobileNo = (db.TBL_MASTER_MEMBER.Where(c => c.MEM_ID == CurrentMerchant.MEM_ID).Select(x => x.MEMBER_MOBILE).FirstOrDefault()),
                                           CompanyAddress= Retailer.ADDRESS,
                                           BeneficiaryName=dmt.NAME,
                                           //BeneficiaryMobile=(db.TBL_DMR_RECIPIENT_DETAILS.Where(c=>c.RECIPIENT_ID==dmt.RECIPIENT_ID).Select(x=>x.BENEFICIARY_MOBILE).FirstOrDefault()),
                                           BeneficiaryMobile=benRef.BENEFICIARY_MOBILE,
                                           //beneficiaryaccountno = dmt.ACCOUNT_NO,
                                           //beneficiaryifsccode = dmt.IFSC_CODE,
                                           BeneficiaryAccountNo = dmt.ACCOUNT_NO,
                                           BeneficiaryIFSCCode = dmt.IFSC_CODE,
                                           SenderName = dmt.SENDER_NAME,
                                           SenderMObile = dmt.SENDER_MOBILE_NO,
                                           TransferAmt=dmt.TRANSFER_AMT,
                                           TransactionDate=dmt.TRANSACTION_DATE,
                                           PoweredBy= Comp.COMPANY,
                                           CompGST = Comp.COMPANY_GST_NO,
                                           Website = host.DOMAIN,
                                           Email= Comp.EMAIL_ID,
                                           TransactionId=dmt.TXN_ID,
                                           TransactionStatus = dmt.TRANSACTION_STATUS,
                                       }).AsEnumerable().Select(z => new PrintInvoice
                                       {
                                           CompanyLogo =z.CompanyLogo.Remove(0, 1),
                                           CompanyName =z.CompanyName,
                                           CompanyAddress=z.CompanyAddress,
                                           MobileNo=z.CompanyMobileNo,
                                           BeneficiaryName=z.BeneficiaryName,
                                           BeneficiaryMobile=z.BeneficiaryMobile,
                                           BeneficiaryAccountNo=z.BeneficiaryAccountNo,
                                           BeneficiaryIFSCCode=z.BeneficiaryIFSCCode,
                                           SenderName=z.SenderName,
                                           SenderMobile=z.SenderMObile,
                                           TransferAmount=z.TransferAmt,
                                           TransactionDate=z.TransactionDate.ToString(),
                                           PowerBy=z.PoweredBy,
                                           GSTNo=z.CompGST,
                                           Website=z.Website,
                                           EmailID=z.Email,
                                           TransactionId=z.TransactionId,
                                           TransactionStatus = z.TransactionStatus
                                       }).FirstOrDefault();


                return Json(Transactionlist, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                throw;
            }
        }
                
        // Instant pay API Call
        [HttpPost]
        public async Task<JsonResult> ValidateCustomerMobileNo(string MobileNo, string RemitterId, string OTP_Val)
        {
            try
            {
                initpage();
                var db = new DBContext();

                #region Verifiy OTP Code
                string SessionNoObj = DateTime.Parse(DateTime.Now.ToString()).ToString("dddMMMddyyyyHHmmss");
                string OutputResponse = string.Empty;
                string URL_ValidateRemitter = "https://in.cyberplat.com/cgi-bin/instp/instp_pay_check.cgi";
                string keyPath = Server.MapPath("~/Test CERT/myprivatekey.pfx");
                var PaymentValidation_AddRem = CyberPlatDMTAPICall._strGetRemitterRegistrationValidation(SessionNoObj, "24", MobileNo, RemitterId, OTP_Val, "1.00");
                ssl.message = ssl.Sign_With_PFX(PaymentValidation_AddRem, keyPath, "rahul123");
                ssl.htmlText = ssl.CallCryptoAPI(ssl.message, URL_ValidateRemitter);
                OutputResponse = "URL:\r\n" + URL_ValidateRemitter + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;
                string Out_response = ssl.htmlText;
                string[] response_Value = ParseReportCode(Out_response);
                string ReturnOutpot = response_Value[5];
                string ErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(ReturnOutpot);

                string CretRemitterRes = string.Empty;
                var RemitterPaymentValidation = CyberPlatDMTAPICall._strGetRemitterRegistrationValidation(SessionNoObj, "24", MobileNo, RemitterId, OTP_Val, "1.00");
                ssl.message = ssl.Sign_With_PFX(RemitterPaymentValidation, keyPath, "rahul123");
                ssl.htmlText = ssl.CallCryptoAPI(ssl.message, "https://in.cyberplat.com/cgi-bin/instp/instp_pay.cgi");
                CretRemitterRes = "URL:\r\n" + URL_ValidateRemitter + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;

                string FinalOut_response = ssl.htmlText;

                string[] Final_response_Value = ParseReportCode(FinalOut_response);
                string FinalOutpot = Final_response_Value[3];
                string Final_ErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(FinalOutpot);
                string returnUrl = Server.UrlDecode(Final_response_Value[5]);
                //var Objsettng = returnUrl.Remove(0, 8);
                var Objsettng = returnUrl;
                JObject json = JObject.Parse(Objsettng);
                var ResponseResult = JObject.Parse(Objsettng);
                dynamic FinalResp = ResponseResult;
                //var beneficiarylist = FinalResp.data.beneficiary;
                //var remitterInfo = FinalResp.data.remitter;
                string Resoutput = Convert.ToString(ResponseResult);


                var statusmsg = FinalResp.statuscode;
                string MsgDetails = FinalResp.status;
                string RemtID = FinalResp.data.remitter.id;
                if (statusmsg == "TXN")
                {
                    TBL_DMR_CUSTOMER_DETAILS objAddCust = new TBL_DMR_CUSTOMER_DETAILS()
                    {
                        MEM_ID = CurrentMerchant.MEM_ID,
                        CUSTOMER_MOBILE = MobileNo,
                        CUSTOMER_NAME = "",
                        ADDRESS = "",
                        DOB = "",
                        CREATED_DATE = System.DateTime.Now,
                        STATUS = 0,
                        TRANSACTIONLIMIT = 0,
                        REMITTER_ID = RemtID,
                        API_RESPONSE = Resoutput,
                        REMAINING_LIMIT=0,
                        CONSUMED_LIMIT=0
                        
                    };
                    db.TBL_DMR_CUSTOMER_DETAILS.Add(objAddCust);
                    await db.SaveChangesAsync();
                    return Json("Customer Mobile No is verified", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Customer Mobile No is verified", JsonRequestBehavior.AllowGet);
                }

                #endregion



                //string OTP_Val_1 = "626211";
                //var GetOutletId = db.TBL_MERCHANT_OUTLET_INFORMATION.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID).OUTLETID;
                //var ValidateCustomerRegistration = MoneyTransferAPI.Remitter_Validation(MobileNo, RemitterId, OTP_Val, GetOutletId);
                //if (ValidateCustomerRegistration.statuscode == "TXN")
                //{
                //    var IsVarified = ValidateCustomerRegistration.data.remitter.is_verified;
                //    var RemitterIdVarified = ValidateCustomerRegistration.data.remitter.id;
                //    string Remitterid = RemitterIdVarified.ToString();
                //    var GetDMTCUstomer = await db.TBL_DMR_CUSTOMER_DETAILS.FirstOrDefaultAsync(x => x.MEM_ID == CurrentMerchant.MEM_ID && x.REMITTER_ID == Remitterid);
                //    GetDMTCUstomer.STATUS = IsVarified;
                //    db.Entry(GetDMTCUstomer).State = System.Data.Entity.EntityState.Modified;
                //    await db.SaveChangesAsync();
                //    return Json("Customer Mobile No is verified", JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //    var errormsg = ValidateCustomerRegistration.status;
                //    return Json(errormsg, JsonRequestBehavior.AllowGet);
                //}
                    
            }
            catch (Exception ex)
            {
                var msg = "Something is going worng";
                return Json(msg);
                Logger.Error("Controller:-  GetCustomerInformation(Merchant DMR), method:- RecipientEnquiry(POST) Line No:- 378", ex);
                throw ex;
            }

        }

        [HttpPost]
        public JsonResult ResendDMROTP(string RemitterId, string BeneficiaryId)
        {
            try
            {
                var db = new DBContext();
                var Meroutlet = db.TBL_MERCHANT_OUTLET_INFORMATION.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID).OUTLETID;
                string Outletid = Meroutlet.ToString();
                var BeneficiaryValidation = MoneyTransferAPI.BeneficiaryRegistrationResendOTP(RemitterId, BeneficiaryId, Outletid);

                if (BeneficiaryValidation.statuscode == "TXN")
                {
                    string msg = BeneficiaryValidation.status;
                    string Msg_val = "OTP Send to your Mobile no.";
                    return Json(Msg_val, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string msg = BeneficiaryValidation.status;
                    return Json(msg, JsonRequestBehavior.AllowGet);
                }
                //return Json("");
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MerchantDMRDashboard(Merchant), method:- ResendDMROTP(POST) Line No:- 259", ex);
                return Json("Error");
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult ValidateBeneficiary(string RemId, string BeneId, string OTP_Val)
        {
            try
            {
                var db = new DBContext();
                var MerchantOutletId = db.TBL_MERCHANT_OUTLET_INFORMATION.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID).OUTLETID;
                var BeneficiaryValidation = MoneyTransferAPI.BeneficiaryRegistrationValidate(RemId, BeneId, OTP_Val, MerchantOutletId.ToString());
                if (BeneficiaryValidation.statuscode == "TXN")
                {
                    var BenefiInfo = db.TBL_DMR_RECIPIENT_DETAILS.FirstOrDefault(x=>x.RECIPIENT_ID==BeneId  );
                    BenefiInfo.ISVERIFIED = 1;
                    db.Entry(BenefiInfo).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    //var GSTTAX = db.TBL_TAX_MASTERS.FirstOrDefault(x => x.TAX_NAME == "GST");
                    //decimal Walletamt = 0;
                    //decimal subWaltamt = 0;
                    //var mastinfo =  db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
                    //string MER_BENE_VERIFY_AMT = System.Configuration.ConfigurationManager.AppSettings["MER_BENE_VERIFY_AMT"];
                    //decimal Mer_BENE_VRY_AMT = 0;
                    //decimal.TryParse(MER_BENE_VERIFY_AMT, out Mer_BENE_VRY_AMT);


                    //decimal GSTCALCULATED = Math.Round(((Mer_BENE_VRY_AMT * GSTTAX.TAX_VALUE) / 118), 2);

                    //string CUST_BENE_VERIFY_RTN_AMT = System.Configuration.ConfigurationManager.AppSettings["CUST_BENE_VERIFY_RTN_AMT"];
                    //decimal CUST_BENE_VRY_AMT = 0;
                    //decimal.TryParse(CUST_BENE_VERIFY_RTN_AMT, out CUST_BENE_VRY_AMT);
                    //string sRandomOTP = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
                    ////var getRecipientInfo =  db.TBL_DMR_RECIPIENT_DETAILS.Where(x => x.RECIPIENT_ID == RecipientId && x.CUSTOMER_ID == CustomerId).FirstOrDefault();

                    //var checkAccount = db.TBL_DMR_RECIPIENT_DETAILS.Where(x => x.RECIPIENT_ID == RemId && x.CUSTOMER_ID == BenefiInfo.CUSTOMER_ID && x.ACCOUNT_NO == BenefiInfo.ACCOUNT_NO).FirstOrDefault();
                    //if (checkAccount.ISVERIFIED == 0)
                    //{
                    //    var RecipientEnquiry = TransXT_DMR_API.RecipientEnquiry(CustomerId, getRecipientInfo.ACCOUNT_NO, getRecipientInfo.IFSC_CODE, "2", RecipientId, "INR", "1");
                    //    var customerInfo =  db.TBL_DMR_CUSTOMER_DETAILS.Where(x => x.CUSTOMER_MOBILE == CustomerId).FirstOrDefault();
                    //    Walletamt = customerInfo.TRANSACTIONLIMIT;
                    //    subWaltamt = (Convert.ToDecimal(Walletamt) - 1);
                    //    var errmsg = RecipientEnquiry.errorMsg.Value;
                    //    decimal openingAmt = 0;
                    //    decimal closingAmt = 0;
                    //    decimal deductedamt = 0;
                    //    decimal RecipientverifyAmt = 0;
                    //    decimal UpdateMer_Bal = 0;
                    //    decimal RecipientMerchantverifyAmt = 0;
                    //    var Accountverification = db.TBL_ACCOUNT_VERIFICATION_TABLE.FirstOrDefault(x => x.APINAME == "MobileWare");
                    //    decimal.TryParse(Accountverification.APPLIED_AMT_TO_MERCHANT.ToString(), out RecipientverifyAmt);
                    //    decimal.TryParse(Accountverification.APPLIED_AMT_TO_MERCHANT.ToString(), out RecipientMerchantverifyAmt);
                    //    var Acountdetails = db.TBL_ACCOUNTS.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                    //    if (Acountdetails != null)
                    //    {
                    //        decimal.TryParse(Acountdetails.OPENING.ToString(), out openingAmt);
                    //        decimal.TryParse(Acountdetails.CLOSING.ToString(), out closingAmt);
                    //        //deductedamt = closingAmt - RecipientverifyAmt;
                    //        deductedamt = closingAmt - Mer_BENE_VRY_AMT;
                    //        UpdateMer_Bal = Convert.ToDecimal(mastinfo.BALANCE) - (Mer_BENE_VRY_AMT);
                    //    }
                    //    decimal WLP_CLOSE = 0;
                    //    decimal WLP_CLOSE_AMT_Deduction = 0;
                    //    var WLPAMt =  db.TBL_ACCOUNTS.Where(x => x.MEM_ID == mastinfo.UNDER_WHITE_LEVEL).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                    //    WLP_CLOSE = WLPAMt.CLOSING;
                    //    WLP_CLOSE_AMT_Deduction = WLP_CLOSE - Mer_BENE_VRY_AMT;
                    //    decimal WLPBalUpdate = 0;
                    //    var WMP_Balance =  db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == WLPAMt.MEM_ID);
                    //    WLPBalUpdate = Convert.ToDecimal(WMP_Balance.BALANCE) - Mer_BENE_VRY_AMT;
                    //    //var errmsg = "SUCCESS";
                    //    if (errmsg == "SUCCESS")
                    //    {
                    //        getRecipientInfo.ISVERIFIED = 1;
                    //        getRecipientInfo.WLP_GST_OUTPUT = GSTCALCULATED;
                    //        getRecipientInfo.MER_GST_INPUT = GSTCALCULATED;
                    //        getRecipientInfo.TIMESTAMP = DateTime.Now;
                    //        getRecipientInfo.VERIFY_BENE_CHARGE = Mer_BENE_VRY_AMT;
                    //        getRecipientInfo.RETURN_BACK_TO_CUST_CHARGE = CUST_BENE_VRY_AMT;
                    //        db.Entry(getRecipientInfo).State = System.Data.Entity.EntityState.Modified;
                    //        customerInfo.TRANSACTIONLIMIT = subWaltamt;
                    //        db.Entry(customerInfo).State = System.Data.Entity.EntityState.Modified;
                    //        mastinfo.BALANCE = UpdateMer_Bal;
                    //        db.Entry(mastinfo).State = System.Data.Entity.EntityState.Modified;

                    //        TBL_ACCOUNTS objMERacnt = new TBL_ACCOUNTS()
                    //        {
                    //            API_ID = 0,
                    //            MEM_ID = CurrentMerchant.MEM_ID,
                    //            MEMBER_TYPE = "RETAILER",
                    //            TRANSACTION_TYPE = "Bank Account Verification",
                    //            TRANSACTION_DATE = DateTime.Now,
                    //            TRANSACTION_TIME = DateTime.Now,
                    //            DR_CR = "DR",
                    //            //AMOUNT = RecipientverifyAmt,
                    //            AMOUNT = Mer_BENE_VRY_AMT,
                    //            NARRATION = "Amount Deduction on Bank Account Verification",
                    //            OPENING = closingAmt,
                    //            CLOSING = deductedamt,
                    //            REC_NO = 0,
                    //            COMM_AMT = 0,
                    //            GST = 0,
                    //            TDS = 0,
                    //            IPAddress = "",
                    //            SERVICE_ID = 0,
                    //            CORELATIONID = sRandomOTP
                    //        };
                    //        db.TBL_ACCOUNTS.Add(objMERacnt);

                    //        TBL_ACCOUNTS WLPObj = new TBL_ACCOUNTS()
                    //        {
                    //            API_ID = 0,
                    //            MEM_ID = (long)mastinfo.UNDER_WHITE_LEVEL,
                    //            MEMBER_TYPE = "WHITE LEVEL",
                    //            TRANSACTION_TYPE = "Bank Account Verification",
                    //            TRANSACTION_DATE = DateTime.Now,
                    //            TRANSACTION_TIME = DateTime.Now,
                    //            DR_CR = "DR",
                    //            //AMOUNT = RecipientverifyAmt,
                    //            AMOUNT = Mer_BENE_VRY_AMT,
                    //            NARRATION = "Amount Deduction on Bank Account Verification",
                    //            OPENING = WLP_CLOSE,
                    //            CLOSING = WLP_CLOSE_AMT_Deduction,
                    //            REC_NO = 0,
                    //            COMM_AMT = 0,
                    //            GST = 0,
                    //            TDS = 0,
                    //            IPAddress = "",
                    //            SERVICE_ID = 0,
                    //            CORELATIONID = sRandomOTP
                    //        };
                    //        db.TBL_ACCOUNTS.Add(WLPObj);
                    //         db.SaveChanges();
                    //        WMP_Balance.BALANCE = WLPBalUpdate;
                    //        db.Entry(WMP_Balance).State = System.Data.Entity.EntityState.Modified;
                    //        db.SaveChanges();
                    //        return Json(new { Result = "true" });
                    //    }
                    //    else
                    //    {
                    //        return Json(errmsg, JsonRequestBehavior.AllowGet);
                    //    }
                    //}
                    //else
                    //{
                    //    return Json(new { Result = "Varified" });
                    //}




                    return Json(BeneficiaryValidation.status.Value);
                }
                else
                {
                    return Json(BeneficiaryValidation.status.Value);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MerchantDMRDashboard(Merchant), method:- ValidateBeneficiary(POST) Line No:- 236", ex);
                return Json("Error");
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult ValidateToDeleteBeneficiary(string RemId, string BeneId, string OTP_Val)
        {
            try
            {
                var db = new DBContext();


                #region Delete Account
                string SessionNoObj = DateTime.Parse(DateTime.Now.ToString()).ToString("dddMMMddyyyyHHmmss");
                string OutputResponse = string.Empty;
                string URL_ValidateRemitter = "https://in.cyberplat.com/cgi-bin/instp/instp_pay_check.cgi";
                string keyPath = Server.MapPath("~/Test CERT/myprivatekey.pfx");
                var PaymentValidation_AddRem = CyberPlatDMTAPICall._strBeneficiaryDeleteValidation(SessionNoObj, "23", RemId, BeneId, OTP_Val, "1.00", "1.00");
                ssl.message = ssl.Sign_With_PFX(PaymentValidation_AddRem, keyPath, "rahul123");
                ssl.htmlText = ssl.CallCryptoAPI(ssl.message, URL_ValidateRemitter);
                OutputResponse = "URL:\r\n" + URL_ValidateRemitter + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;
                string Out_response = ssl.htmlText;
                string[] response_Value = ParseReportCode(Out_response);
                string ReturnOutpot = response_Value[5];
                string ErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(ReturnOutpot);

                string CretRemitterRes = string.Empty;
                var RemitterPaymentValidation = CyberPlatDMTAPICall._strBeneficiaryDeleteValidation(SessionNoObj, "23", RemId, BeneId, OTP_Val, "1.00", "1.00");
                ssl.message = ssl.Sign_With_PFX(RemitterPaymentValidation, keyPath, "rahul123");
                ssl.htmlText = ssl.CallCryptoAPI(ssl.message, "https://in.cyberplat.com/cgi-bin/instp/instp_pay.cgi");
                CretRemitterRes = "URL:\r\n" + URL_ValidateRemitter + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;

                string FinalOut_response = ssl.htmlText;
                string[] Final_response_Value = ParseReportCode(FinalOut_response);
                string FinalOutpot = Final_response_Value[3];
                string Final_ErrorReturn = CyberPlatErrorCode.GetCyberPlatGenericError(FinalOutpot);
                string returnUrl = Server.UrlDecode(Final_response_Value[5]);
                //var Objsettng = returnUrl.Remove(0, 8);
                var Objsettng = returnUrl;
                JObject json = JObject.Parse(Objsettng);
                var ResponseResult = JObject.Parse(Objsettng);
                dynamic FinalResp = ResponseResult;
                //var beneficiarylist = FinalResp.data.beneficiary;
                var errorcode = FinalResp.statuscode;
                #endregion



                //var Outlet_Id = db.TBL_MERCHANT_OUTLET_INFORMATION.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID).OUTLETID;
                //string Id_Outlet = Outlet_Id.ToString();
                //var BeneficiaryValidation = MoneyTransferAPI.BeneficiaryDeleteValidate(RemId, BeneId, OTP_Val, Id_Outlet);
                if (errorcode == "TXN")
                {                    
                  
                    var deletebeneficiaryaccount = db.TBL_DMR_RECIPIENT_DETAILS.Where(x => x.BENEFICIARY_ID == BeneId).FirstOrDefault();
                    //deletebeneficiaryaccount.ISVERIFIED = 1;
                    deletebeneficiaryaccount.STATUS = 1;
                    db.Entry(deletebeneficiaryaccount).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json("Account is delete successfully",JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Please try again later.", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MerchantDMRDashboard(Merchant), method:- ValidateBeneficiary(POST) Line No:- 236", ex);
                return Json("Error");
                throw ex;
            }
        }


        [HttpPost]
        public JsonResult GetAccountInformation(string AccountNo)
        {
            try
            {
                var db = new DBContext();
                var Meroutlet = db.TBL_MERCHANT_OUTLET_INFORMATION.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID).OUTLETID;
                string Outletid = Meroutlet.ToString();
                var BeneficiaryValidation = MoneyTransferAPI.GetBankDetails(AccountNo, Outletid);

                if (BeneficiaryValidation.statuscode == "TXN")
                {
                    string msg = BeneficiaryValidation.status;
                    string Msg_val = "OTP Send to your Mobile no.";
                    return Json(Msg_val, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string msg = BeneficiaryValidation.status;
                    return Json(msg, JsonRequestBehavior.AllowGet);
                }
                //return Json("");
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MerchantDMRDashboard(Merchant), method:- ResendDMROTP(POST) Line No:- 259", ex);
                return Json("Error");
                throw ex;
            }
        }


        [HttpPost]
        public async Task<JsonResult> BankNameList(string prefix)
        {
            try
            {
                var db = new DBContext();
                var OperatorValue = await (from oper in db.TBL_BANK_MASTER
                                           where oper.BankName.StartsWith(prefix)
                                           select new
                                           {
                                               //label = oper.SERVICE_NAME + "-" + oper.RECHTYPE,
                                               label = oper.BankName,
                                               val = oper.ID
                                           }).ToListAsync();
                return Json(OperatorValue);
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- AutoComplete(POST) Line No:- 252", ex);
                throw ex;
            }
        }


        #region CyberPlat Response converted into string Array  
        private static string[] ParseReportCode(string reportCode)
        {
            const int FIRST_VALUE_ONLY_SEGMENT = 3;
            const int GRP_SEGMENT_NAME = 1;
            const int GRP_SEGMENT_VALUE = 2;
            Regex reportCodeSegmentPattern = new Regex(@"\s*([^\}\{=\s]+)(?:=\[?([^\s\]\}]+)\]?)?");
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
}