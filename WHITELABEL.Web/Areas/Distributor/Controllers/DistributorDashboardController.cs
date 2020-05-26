using log4net;
using NonFactors.Mvc.Grid;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Controllers;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;
namespace WHITELABEL.Web.Areas.Distributor.Controllers
{
    [Authorize]       
    public class DistributorDashboardController : DistributorBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        //public void initpage()
        //{
        //    try
        //    {
        //        ViewBag.ControllerName = "Distributor Dashboard";
        //        SystemClass sclass = new SystemClass();
        //        string userID = sclass.GetLoggedUser();
        //        long userid = long.Parse(userID);
        //        var dbmain = new DBContext();
        //        if (userID != null && userID != "")
        //        {
        //            TBL_MASTER_MEMBER currUser = dbmain.TBL_MASTER_MEMBER.SingleOrDefault(c => c.MEM_ID == userid && c.ACTIVE_MEMBER == true && c.MEMBER_ROLE==4);
        //            if (currUser != null)
        //            {
        //                Session["DistributorUserId"] = currUser.MEM_ID;
        //                // Session["UserName"] = currUser.UserName;
        //            }
        //            else
        //            {
        //                Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //                return;
        //            }
        //        }
        //        if (Session["DistributorUserId"] == null)
        //        {
        //            //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //            Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //            return;
        //        }
        //        bool Islogin = false;
        //        if (Session["DistributorUserId"] != null)
        //        {
        //            Islogin = true;
        //            ViewBag.CurrentUserId = MemberCurrentUser.MEM_ID;
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
                ViewBag.ControllerName = "Distributor";
                if (Session["DistributorUserId"] == null)
                {
                    Response.Redirect(Url.Action("Logout", "Login", new { area = "" }));
                    return;
                }
                bool Islogin = false;

                if (Session["DistributorUserId"] != null)
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


        // GET: Distributor/DistributorDashboard
        public ActionResult Index()
        {
            if (Session["DistributorUserId"] != null)
            {
                try
                {
                    initpage();
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
                                               transactionId = acnt.TransactionID,
                                               tranDate = acnt.REQUEST_DATE,
                                               userName = mem.UName
                                           }).AsEnumerable().Select(z => new GetRequisitiondetails()
                                           {
                                               TransId = z.tranid.ToString(),
                                               TransactionID = z.transactionId,
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
                    Logger.Error("Controller:-  DistributorDashboard(Distributor), method:- Index (GET) Line No:- 101", ex);
                    return RedirectToAction("Exception", "ErrorHandler", new { area = "" });
                    throw ex;
                }
            }
            else
            {
                Session["DistributorUserId"] = null;
                Session["DistributorUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("DistributorUserId");
                Session.Remove("DistributorUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });
            }   
        }
        
        // For Member List
        public PartialViewResult MemberGrid()
        {
            try
            {
                return PartialView(ExporMembertableGrid());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private IGrid<TBL_MASTER_MEMBER> ExporMembertableGrid()
        {
            long userid = MemberCurrentUser.MEM_ID;
            var dbcontext = new DBContext();
            var memberinfo = dbcontext.TBL_MASTER_MEMBER.Where(x => x.INTRODUCER == userid).ToList().OrderByDescending(x => x.JOINING_DATE);
            IGrid<TBL_MASTER_MEMBER> grid = new Grid<TBL_MASTER_MEMBER>(memberinfo);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.UName).Titled("UserName").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.MEMBER_NAME).Titled("MEMBER NAME").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.COMPANY).Titled("Company").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.MEMBER_MOBILE).Titled("Mobile").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.BALANCE).Titled("BALANCE").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.BLOCKED_BALANCE).Titled("BLK BLNC").Filterable(true).Sortable(true);
            grid.Pager = new GridPager<TBL_MASTER_MEMBER>(grid);
            grid.Processors.Add(grid.Pager);
            grid.Pager.RowsPerPage = 5;
            foreach (IGridColumn column in grid.Columns)
            {
                column.Filter.IsEnabled = true;
                column.Sort.IsEnabled = true;
            }
            return grid;
        }
        // For Requisition List
        public PartialViewResult RequisitionGridlist()
        {
            try
            {
                return PartialView(RequistionDetailsExcel());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private IGrid<TBL_BALANCE_TRANSFER_LOGS> RequistionDetailsExcel()
        {
            var db = new DBContext();
            var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                   join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                   where x.STATUS == "Pending" && x.TO_MEMBER == MemberCurrentUser.MEM_ID
                                   select new
                                   {
                                       Touser = "PowerAdmin",
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
                                   }).ToList().OrderByDescending(x => x.REQUEST_DATE);

            IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = new Grid<TBL_BALANCE_TRANSFER_LOGS>(transactionlist);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            //grid.Columns.Add(model => model.TransactionID).Titled("Trans Id");
            grid.Columns.Add(model => model.ToUser).Titled("To User");
            grid.Columns.Add(model => model.FromUser).Titled("From Member");
            grid.Columns.Add(model => model.REQUEST_DATE).Titled("Req Date").Formatted("{0:d}").MultiFilterable(true);
            grid.Columns.Add(model => model.AMOUNT).Titled("Amount");
            //grid.Columns.Add(model => model.BANK_ACCOUNT).Titled("Bank Acnt");
            //grid.Columns.Add(model => model.TRANSACTION_DETAILS).Titled("Pay Method");

            grid.Pager = new GridPager<TBL_BALANCE_TRANSFER_LOGS>(grid);
            grid.Processors.Add(grid.Pager);
            grid.Pager.RowsPerPage = 5;
            foreach (IGridColumn column in grid.Columns)
            {
                column.Filter.IsEnabled = true;
                column.Sort.IsEnabled = true;
            }
            return grid;
        }
        // For Bank List
        public PartialViewResult BankPartilGridBind()
        {
            try
            {
                return PartialView(CreateExportableGrid());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private IGrid<TBL_SETTINGS_BANK_DETAILS> CreateExportableGrid()
        {
            var db = new DBContext();
            var bankdetails = db.TBL_SETTINGS_BANK_DETAILS.Where(x => x.ISDELETED == 0 && x.MEM_ID == MemberCurrentUser.MEM_ID).ToList().OrderByDescending(x => x.CREATED_DATE);
            IGrid<TBL_SETTINGS_BANK_DETAILS> grid = new Grid<TBL_SETTINGS_BANK_DETAILS>(bankdetails);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.BANK).Titled("Bank").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.IFSC).Titled("IFSC").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.ACCOUNT_NO).Titled("Account No").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.BRANCH).Titled("BRANCH").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.CONTACT).Titled("CONTACT").Filterable(true).Sortable(true);            
            grid.Pager = new GridPager<TBL_SETTINGS_BANK_DETAILS>(grid);
            grid.Processors.Add(grid.Pager);
            grid.Pager.RowsPerPage = 5;
            foreach (IGridColumn column in grid.Columns)
            {
                column.Filter.IsEnabled = true;
                column.Sort.IsEnabled = true;
            }
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
                    return Json(walletamount, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //Session["openingAmt"] = "0";
                    //Session["closingAmt"] = "0";
                    //walletamount.CLOSING = 0;
                    //return Json(walletamount, JsonRequestBehavior.AllowGet);
                    return Json("0", JsonRequestBehavior.AllowGet);
                }
                //return Json(walletamount, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("");
                throw ex;
            }
        }

    }
}