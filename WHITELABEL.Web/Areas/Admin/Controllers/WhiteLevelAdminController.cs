using log4net;
using Newtonsoft.Json;
using NonFactors.Mvc.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;
using System.Web.UI;
using System.Web.UI.WebControls;
using CyberPlatOpenSSL;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace WHITELABEL.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class WhiteLevelAdminController : AdminBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);       
        public void initpage()
        {
            try
            {                
                if (DomainNameCheck.DomainChecking(Session["DOMAINNAME"].ToString(), Request.Url.Host) == false)
                {
                    Response.Redirect(Url.Action("DomainError", "Login", new { area = "" }));
                }
                ViewBag.ControllerName = "White Label";
                if (Session["WhiteLevelUserId"] == null)
                {
                    Response.Redirect(Url.Action("Logout", "Login", new { area = "" }));
                    return;
                }
                bool Islogin = false;

                if (Session["WhiteLevelUserId"] != null)
                {
                    Islogin = true;
                    ViewBag.CurrentUserId = MemberCurrentUser.MEM_ID;
                }
                ViewBag.Islogin = Islogin;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException);
                return;
            }
        }

        // GET: Admin/WhiteLevelAdmin    
        OpenSSL ssl = new OpenSSL();
        StringBuilder str = new StringBuilder();
        public static string SessionNo = DateTime.Parse(DateTime.Now.ToString()).ToString("dddMMMddyyyyHHmmss");
        private String _BalancestrRequest(string CERTNo,string SDCode, string APCode, string OPCode, string SessionNo)
        {
            StringBuilder _reqStr = new StringBuilder();
            #region Create Request
            _reqStr.Append("CERT=" + CERTNo + Environment.NewLine);
            _reqStr.Append("SD=" + SDCode + Environment.NewLine);
            _reqStr.Append("AP=" + APCode + Environment.NewLine);
            _reqStr.Append("OP=" + OPCode + Environment.NewLine);
            _reqStr.Append("SESSION="+SessionNo);
            #endregion
            return _reqStr.ToString();
        }
        public ActionResult Index()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                try
                {
                    initpage();
                    //string Session_No = DateTime.Parse(DateTime.Now.ToString()).ToString("dddMMMddyyyyHHmmss");

                    //string URL_PATH = "https://in.cyberplat.com/cgi-bin/mts_espp/mtspay_rest.cgi";
                    //#region For CyberPlat API Implementation Section
                    //ssl.CERTNo = "399D169D3A72AE32D370242EF0FAE5BA5C026421";
                    //string CertNo = "399D169D3A72AE32D370242EF0FAE5BA5C026421";

                    //string filePath = HttpContext.Server.MapPath("~/Test CERT/myprivatekey.pfx");                   


                    //string Output = string.Empty;
                    ////string keyPath_Val = Server.MapPath("Test CERT\\myprivatekey.pfx");
                    //string keyPath_Val = Server.MapPath("~/Test CERT/myprivatekey.pfx");                    
                    //string parameterval = _BalancestrRequest(CertNo, "349690", "351905", "351906", SessionNo);
                    
                    //ssl.message = ssl.Sign_With_PFX(parameterval, keyPath_Val, "rahul123");
                    //ssl.htmlText = ssl.CallCryptoAPI(ssl.message, URL_PATH);

                    //Output = "URL:\r\n" + URL_PATH + "\r\n\r\n" + "Request:\r\n" + ssl.message + "\r\n\r\nResponse:\r\n" + ssl.htmlText;
                    //string outval = ssl.htmlText;
                    //string[] outvalparam = ParseReportCode(outval);
                    //string uuu = DecodeServerName(outval);
                    ////var outval = ssl.htmlText;
                    ////var jsonString = JsonConvert.SerializeObject(outval);


                    ////Encoding encodingUTF8 = Encoding.UTF8;

                    ////// Encode string to byte array
                    ////Byte[] encodedBytes = encodingUTF8.GetBytes(outval);
                    ////Console.WriteLine("Encoded bytes: " +
                    ////                  BitConverter.ToString(encodedBytes));

                    ////// Decode to string
                    ////String decodedString = encodingUTF8.GetString(encodedBytes);

                    //#endregion





                    var db = new DBContext();                   
                    var Memblistdb = db.TBL_MASTER_MEMBER.Where(x => x.INTRODUCER == MemberCurrentUser.MEM_ID).Take(6).ToList().OrderByDescending(x => x.JOINING_DATE);
                    ViewBag.whitelevel = Memblistdb;
                    var availablebal = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).FirstOrDefault();
                    if (availablebal.BALANCE > 0)
                    {
                        ViewBag.AvailableBalance = availablebal.BALANCE;

                    }
                    else
                    {
                        ViewBag.AvailableBalance = 0;
                    }
                    var walletamount = db.TBL_ACCOUNTS.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                    if (walletamount != null)
                    {
                        ViewBag.openingAmt = walletamount.OPENING;
                        ViewBag.closingAmt = walletamount.CLOSING;
                    }
                    else
                    {
                        ViewBag.openingAmt = "0";
                        ViewBag.closingAmt = "0";
                    }
                    

                    var requisitionlist = (from acnt in db.TBL_BALANCE_TRANSFER_LOGS
                                           join mem in db.TBL_MASTER_MEMBER on acnt.FROM_MEMBER equals mem.MEM_ID
                                           where acnt.TO_MEMBER == MemberCurrentUser.MEM_ID
                                           select new
                                           {
                                               tranid = acnt.SLN,
                                               transactionid = acnt.TransactionID,
                                               tranDate = acnt.REQUEST_DATE,
                                               userName = mem.UName
                                           }).AsEnumerable().Select(z => new GetRequisitiondetails()
                                           {
                                               TransId = z.tranid.ToString(),
                                               TransactionID = z.transactionid,
                                               TransDate = z.tranDate.ToString("yyyy-MM-dd"),
                                               TransUserName = z.userName
                                           }).ToList().Take(6).OrderByDescending(z => z.TransDate);
                    ViewBag.RequisitionList = requisitionlist;
                    var BankListdb = db.TBL_SETTINGS_BANK_DETAILS.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).ToList();
                    ViewBag.BankDetailsList = BankListdb;

                    return View();
                }
                catch (Exception ex)
                {
                    Logger.Error("Controller:-  WhiteLevelAdmin(Admin), method:- Index (GET) Line No:- 94", ex);
                    return RedirectToAction("Exception", "ErrorHandler", new { area = "" });
                    throw ex;
                }
            }
            else
            {
                Session["WhiteLevelUserId"] = null;
                Session["WhiteLevelUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("WhiteLevelUserId");
                Session.Remove("WhiteLevelUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
                
        }
        public ActionResult AdminMemberInfo()
        {
            initpage();////
            if (Session["WhiteLevelUserId"] != null)
            {
                try
                {
                    return View();
                }
                catch (Exception ex)
                {
                    Logger.Error("Controller:-  WhiteLevelAdmin(Admin), method:- AdminMemberInfo (GET) Line No:- 121", ex);
                    return RedirectToAction("Exception", "ErrorHandler", new { area = "" });
                    throw ex;
                }
            }
            else
            {
                Session["WhiteLevelUserId"] = null;
                Session["WhiteLevelUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("WhiteLevelUserId");
                Session.Remove("WhiteLevelUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }

        public PartialViewResult MemberGrid()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                try
                {
                    var dbcontext = new DBContext();
                    //var memberinfo = dbcontext.TBL_MASTER_MEMBER.ToList().OrderByDescending(x=>x.JOINING_DATE);
                    //// Only grid query values will be available here.
                    //return PartialView("IndexGrid", memberinfo);

                    return PartialView(ExporMembertableGrid());
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                Session["WhiteLevelUserId"] = null;
                Session["WhiteLevelUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("WhiteLevelUserId");
                Session.Remove("WhiteLevelUserName");
                Session.Remove("UserType");
                RedirectToAction("Index", "Login", new { area = "" });
                return PartialView(ExporMembertableGrid());
            }

        }

        private IGrid<TBL_MASTER_MEMBER> ExporMembertableGrid()
        {
            var dbcontext = new DBContext();
            var memberinfo = dbcontext.TBL_MASTER_MEMBER.Where(x=>x.INTRODUCER==MemberCurrentUser.MEM_ID).ToList().OrderByDescending(x => x.JOINING_DATE);
            IGrid<TBL_MASTER_MEMBER> grid = new Grid<TBL_MASTER_MEMBER>(memberinfo);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.UName).Titled("UserName").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.COMPANY).Titled("Company").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.MEMBER_MOBILE).Titled("Mobile").Filterable(true).Sortable(true);
            grid.Pager = new GridPager<TBL_MASTER_MEMBER>(grid);
            grid.Processors.Add(grid.Pager);
            grid.Pager.RowsPerPage = 5;
            return grid;
        }

        public PartialViewResult RequisitionGrid()
        {

            try
            {
                var dbcontext = new DBContext();
                return PartialView(ExporRequisitionGrid());
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private IGrid<TBL_BALANCE_TRANSFER_LOGS> ExporRequisitionGrid()
        {
            var db = new DBContext();
            var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                   join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                   where x.STATUS == "Pending" && x.TO_MEMBER == MemberCurrentUser.MEM_ID
                                   select new
                                   {
                                       Touser = "White Label",
                                       TransId = x.TransactionID,
                                       FromUser = y.UName,
                                       REQUEST_DATE = x.REQUEST_DATE,
                                       AMOUNT = x.AMOUNT,
                                       BANK_ACCOUNT = x.BANK_ACCOUNT,
                                       TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                       SLN = x.SLN
                                   }).AsEnumerable().Select(z => new TBL_BALANCE_TRANSFER_LOGS
                                   {
                                       ToUser = z.Touser,
                                       TransactionID = z.TransId,
                                       FromUser = z.FromUser,
                                       AMOUNT = z.AMOUNT,
                                       REQUEST_DATE = z.REQUEST_DATE,
                                       BANK_ACCOUNT = z.BANK_ACCOUNT,
                                       TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                       SLN = z.SLN
                                   }).ToList();

            IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = new Grid<TBL_BALANCE_TRANSFER_LOGS>(transactionlist);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.TransactionID).Titled("Trans Id");
            //grid.Columns.Add(model => model.ToUser).Titled("To User");
            grid.Columns.Add(model => model.FromUser).Titled("From Member");
            grid.Columns.Add(model => model.REQUEST_DATE).Titled("Req Date").Formatted("{0:d}").MultiFilterable(true);
            //grid.Columns.Add(model => model.AMOUNT).Titled("Amount");
            //grid.Columns.Add(model => model.BANK_ACCOUNT).Titled("Bank Acnt");
            grid.Pager = new GridPager<TBL_BALANCE_TRANSFER_LOGS>(grid);
            grid.Processors.Add(grid.Pager);
            grid.Pager.RowsPerPage = 6;

            foreach (IGridColumn column in grid.Columns)
            {
                column.Filter.IsEnabled = true;
                column.Sort.IsEnabled = true;
            }

            return grid;
        }

        public PartialViewResult BankDetailsGrid()
        {
            try
            {
                var dbcontext = new DBContext();
                return PartialView(ExportBankDetailsGrid());
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private IGrid<TBL_SETTINGS_BANK_DETAILS> ExportBankDetailsGrid()
        {
            var db = new DBContext();
            var bankdetails = db.TBL_SETTINGS_BANK_DETAILS.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).ToList().OrderByDescending(x => x.CREATED_DATE);

            IGrid<TBL_SETTINGS_BANK_DETAILS> grid = new Grid<TBL_SETTINGS_BANK_DETAILS>(bankdetails);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.BANK).Titled("Bank").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.ACCOUNT_NO).Titled("Account No").Filterable(true).Sortable(true);
            grid.Columns.Add(model => (model.ISDELETED == 0 ? "Active" : "Deactive")).Titled("Status").Filterable(true).Sortable(true);
            grid.Pager = new GridPager<TBL_SETTINGS_BANK_DETAILS>(grid);
            grid.Processors.Add(grid.Pager);
            grid.Pager.RowsPerPage = 5;
            return grid;
        }
        [HttpPost]
        public JsonResult LoadAvailableBalance()
        {
            initpage();////
            try
            {
                var db = new DBContext();
                //var walletamount = db.TBL_ACCOUNTS.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                var walletamount = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).FirstOrDefault();
                if (walletamount != null)
                {
                    //Session["openingAmt"] = walletamount.OPENING;
                    //Session["closingAmt"] = walletamount.CLOSING;
                    //ViewBag.openingAmt = walletamount.OPENING;
                    //ViewBag.closingAmt = walletamount.CLOSING;
                    return Json(walletamount, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //Session["openingAmt"] = "0";
                    //Session["closingAmt"] = "0";
                    //ViewBag.openingAmt = "0";
                    //ViewBag.closingAmt = "0";
                    //walletamount.CLOSING = 0;
                    //return Json(walletamount, JsonRequestBehavior.AllowGet);
                    return Json("0", JsonRequestBehavior.AllowGet);
                }
                
            }
            catch (Exception ex)
            {
                return Json("");
                throw ex;
            }
        }

        //public ActionResult TestURL()
        //{
        //    return View();
        //}


        public static string DecodeServerName(string encodedServername)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(encodedServername));
        }

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
    }
}