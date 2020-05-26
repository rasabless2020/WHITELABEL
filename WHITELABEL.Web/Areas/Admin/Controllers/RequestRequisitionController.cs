﻿using log4net;
using NonFactors.Mvc.Grid;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Controllers;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;

namespace WHITELABEL.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class RequestRequisitionController : AdminBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        //public void initpage()
        //{
        //    try
        //    {
        //        ViewBag.ControllerName = "Member Requisition";

        //        SystemClass sclass = new SystemClass();
        //        string userID = sclass.GetLoggedUser();
        //        long userid = long.Parse(userID);
        //        var dbmain = new DBContext();
        //        if (userID != null && userID != "")
        //        {
        //            TBL_MASTER_MEMBER currUser = dbmain.TBL_MASTER_MEMBER.SingleOrDefault(c => c.MEM_ID == userid && c.ACTIVE_MEMBER == true && c.MEMBER_ROLE==1 );
        //            if (currUser != null)
        //            {
        //                //Session["UserId"] = currUser.MEM_ID;
        //                Session["WhiteLevelUserId"] = currUser.MEM_ID;
        //            }
        //        }
        //        if (Session["WhiteLevelUserId"] == null)
        //        {
        //            //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //            Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //            return;
        //        }
        //        bool Islogin = false;
        //        if (Session["WhiteLevelUserId"] != null)
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


        // GET: Admin/RequestRequisition
        public ActionResult Index(string transId = "")
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                initpage();
                try
                {
                    var db = new DBContext();
                    var whiteleveluser = db.TBL_AUTH_ADMIN_USERS.Where(x => x.USER_ID == 1).FirstOrDefault();
                    ViewBag.Introducer = whiteleveluser.USER_NAME;
                    ViewBag.IntroducerName = whiteleveluser.USER_NAME;
                    ViewBag.IntroducerEmail = whiteleveluser.USER_EMAIL;
                    ViewBag.IntroducerMobile = whiteleveluser.USER_MOBILE;
                    ViewBag.IntroducerMemberId = whiteleveluser.USER_ID;

                    if (transId == "")
                    {
                        var BankInformation = (from x in db.TBL_SETTINGS_BANK_DETAILS
                                               where x.MEM_ID == 0 && x.ISDELETED == 0
                                               select new
                                               {
                                                   BankID = x.SL_NO,
                                                   BankName = (x.BANK + "-" + x.ACCOUNT_NO)
                                               }).AsEnumerable().Select(z => new ViewBankDetails
                                               {
                                                   BankID = z.BankID.ToString(),
                                                   BankName = z.BankName
                                               }).ToList().Distinct();
                        ViewBag.BankInformation = new SelectList(BankInformation, "BankID", "BankName");
                        ViewBag.checkbank = "0";
                        return View();
                    }
                    else
                    {
                        string valid = Decrypt.DecryptMe(transId);
                        long transIdval = long.Parse(valid);
                        var BankInformation = (from x in db.TBL_SETTINGS_BANK_DETAILS
                                               where x.MEM_ID == 0 && x.ISDELETED == 0
                                               select new
                                               {
                                                   BankID = x.SL_NO,
                                                   BankName = (x.BANK + "-" + x.ACCOUNT_NO)
                                               }).AsEnumerable().Select(z => new ViewBankDetails
                                               {
                                                   BankID = z.BankID.ToString(),
                                                   BankName = z.BankName
                                               }).ToList().Distinct();
                        ViewBag.BankInformation = new SelectList(BankInformation, "BankID", "BankName");
                        var translist = db.TBL_BALANCE_TRANSFER_LOGS.Where(x => x.SLN == transIdval).FirstOrDefault();
                        ViewBag.checkbank = "1";
                        translist.FromUser = translist.FROM_MEMBER.ToString();
                        translist.PAYMENT_METHOD = translist.PAYMENT_METHOD;
                        translist.REQUEST_DATE = Convert.ToDateTime(translist.REQUEST_DATE.ToString("yyyy-MM-dd").Substring(0, 10));                        
                        translist.BANK_ACCOUNT = translist.BANK_ACCOUNT;
                        translist.TRANSACTION_DETAILS = translist.TRANSACTION_DETAILS;
                        return View(translist);
                    }

                }
                catch (Exception ex)
                {
                    Logger.Error("Controller:-  RequestRequisition(Admin), method:- Index (GET) Line No:- 122", ex);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(true)]
        public async Task<ActionResult> SubmitRequestRequisition(TBL_BALANCE_TRANSFER_LOGS objval)
        {
            initpage();////
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var transinfo = await db.TBL_BALANCE_TRANSFER_LOGS.Where(x => x.SLN == objval.SLN).FirstOrDefaultAsync();
                    if (transinfo == null)
                    {
                        var checkrefNo = db.TBL_BALANCE_TRANSFER_LOGS.Where(x => x.REFERENCE_NO == objval.REFERENCE_NO).FirstOrDefault();
                        if (checkrefNo != null)
                        {
                            checkrefNo.REQUEST_DATE = objval.REQUEST_DATE;
                            checkrefNo.REQUEST_TIME = System.DateTime.Now;
                            checkrefNo.BANK_ACCOUNT = objval.BANK_ACCOUNT;
                            checkrefNo.AMOUNT = objval.AMOUNT;
                            checkrefNo.PAYMENT_METHOD = objval.PAYMENT_METHOD;
                            checkrefNo.TRANSFER_METHOD = "Cash";
                            checkrefNo.FromUser = "Admin";
                            checkrefNo.BANK_CHARGES = objval.BANK_CHARGES;
                            checkrefNo.TRANSACTION_DETAILS = objval.TRANSACTION_DETAILS;
                            checkrefNo.INSERTED_BY = MemberCurrentUser.MEM_ID;
                            db.Entry(checkrefNo).State = System.Data.Entity.EntityState.Modified;
                            await db.SaveChangesAsync();
                        }
                        else
                        {
                            long fromuser = objval.FROM_MEMBER;
                            //objval.TransactionID = MemberCurrentUser.MEM_ID + "" + fromuser + DateTime.Now.ToString("yyyyMMdd-HHMMss");
                            objval.TransactionID = MemberCurrentUser.MEM_ID + "" + fromuser + DateTime.Now.ToString("yyyyMMdd") + "" + DateTime.Now.ToString("HHMMss");
                            objval.TO_MEMBER = 0;
                            objval.FROM_MEMBER = MemberCurrentUser.MEM_ID;
                            objval.REQUEST_DATE = Convert.ToDateTime(objval.REQUEST_DATE);
                            objval.REQUEST_TIME = System.DateTime.Now;
                            objval.BANK_ACCOUNT = objval.BANK_ACCOUNT;
                            objval.STATUS = "Pending";
                            objval.TRANSFER_METHOD = "Cash";
                            objval.BANK_CHARGES = objval.BANK_CHARGES;
                            objval.FromUser = "test";
                            objval.INSERTED_BY = MemberCurrentUser.MEM_ID;
                            db.TBL_BALANCE_TRANSFER_LOGS.Add(objval);
                            await db.SaveChangesAsync();
                            //ContextTransaction.Commit();
                            //return View();
                        }
                    }
                    else
                    {
                        long fromuser = objval.FROM_MEMBER;
                        //objval.TransactionID = MemberCurrentUser.MEM_ID + "" + fromuser + DateTime.Now.ToString("yyyyMMdd-HHMMss");
                        //objval.TransactionID = MemberCurrentUser.MEM_ID + "" + fromuser + DateTime.Now.ToString("yyyyMMdd") + "" + DateTime.Now.ToString("HHMMss");
                        //objval.TO_MEMBER = 0;
                        //objval.FROM_MEMBER = MemberCurrentUser.MEM_ID;
                        transinfo.REQUEST_DATE = Convert.ToDateTime(objval.REQUEST_DATE);
                        transinfo.REQUEST_TIME = System.DateTime.Now;
                        transinfo.BANK_ACCOUNT = objval.BANK_ACCOUNT;
                        transinfo.STATUS = "Pending";
                        transinfo.TRANSFER_METHOD = "Cash";
                        transinfo.TRANSACTION_DETAILS = objval.TRANSACTION_DETAILS;
                        transinfo.BANK_CHARGES = objval.BANK_CHARGES;
                        transinfo.INSERTED_BY = MemberCurrentUser.MEM_ID;
                        transinfo.FromUser = "test";
                        //db.TBL_BALANCE_TRANSFER_LOGS.Add(objval);
                        db.Entry(transinfo).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();
                        //return View();
                    }
                    ContextTransaction.Commit();
                    return RedirectToAction("RequisitionDispay");
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    Logger.Error("Controller:-  RequestRequisition(Admin), method:- SubmitRequestRequisition (POST) Line No:- 192", ex);
                    return RedirectToAction("Exception", "ErrorHandler", new { area = "" });

                }
            }
            return View();
        }
        public ActionResult RequisitionDispay()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                try
                {
                    initpage();
                    return View();
                }
                catch (Exception ex)
                {
                    Logger.Error("Controller:-  RequestRequisition(Admin), method:- RequisitionDispay (POST) Line No:- 211", ex);
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
        public PartialViewResult IndexGrid(string DateFrom="" ,string Date_To="")
        {
            try
            {
                var db = new DBContext();                
                return PartialView(CreateExportableGridINExcel(DateFrom, Date_To));
            }
            catch (Exception ex)
            {
                throw ex;
            }           
        }
        [HttpGet]
        public FileResult GridExportIndex(string DateFrom = "", string Date_To = "")
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = CreateExportableGridINExcel(DateFrom, Date_To);
                ExcelWorksheet sheet = package.Workbook.Worksheets["Data"];

                foreach (IGridColumn column in grid.Columns)
                {
                    sheet.Cells[1, col].Value = column.Title;
                    sheet.Column(col++).Width = 18;
                }

                foreach (IGridRow<TBL_BALANCE_TRANSFER_LOGS> gridRow in grid.Rows)
                {
                    col = 1;
                    foreach (IGridColumn column in grid.Columns)
                        sheet.Cells[row, col++].Value = column.ValueFor(gridRow);

                    row++;
                }

                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                //return File(fileContents: package.GetAsByteArray(), contentType: "application/unknown");
            }
        }

        private IGrid<TBL_BALANCE_TRANSFER_LOGS> CreateExportableGridINExcel(string DateFrom = "", string Date_To = "")
        {
            var db = new DBContext();
            if (DateFrom != "" && Date_To != "")
            {
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.STATUS == "Pending" && x.TO_MEMBER == 0 && x.FROM_MEMBER == MemberCurrentUser.MEM_ID && x.REQUEST_DATE>= Date_From_Val && x.REQUEST_DATE<= Date_To_Val
                                       select new
                                       {
                                           Touser = "Power Admin",
                                           TransId = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           ReferenceNo = x.REFERENCE_NO,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           SLN = x.SLN,
                                           Pay_Mode = x.PAYMENT_METHOD
                                       }).AsEnumerable().Select(z => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           ToUser = z.Touser,
                                           TransactionID = z.TransId,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REFERENCE_NO = z.ReferenceNo,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           SLN = z.SLN,
                                           PAYMENT_METHOD = z.Pay_Mode
                                       }).ToList();

                IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = new Grid<TBL_BALANCE_TRANSFER_LOGS>(transactionlist);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.TransactionID).Titled("Trans Id");
                grid.Columns.Add(model => model.ToUser).Titled("To User");
                grid.Columns.Add(model => model.FromUser).Titled("From Member");
                grid.Columns.Add(model => model.REQUEST_DATE).Titled("Req Date").Formatted("{0:yyyy-MM-dd}").MultiFilterable(false);
                grid.Columns.Add(model => model.AMOUNT).Titled("Amount");
                grid.Columns.Add(model => model.PAYMENT_METHOD).Titled("Pay Mode");
                grid.Columns.Add(model => model.BANK_ACCOUNT).Titled("Bank Acnt");
                grid.Columns.Add(model => model.TRANSACTION_DETAILS).Titled("Txn Details");

                grid.Columns.Add(model => model.SLN).Titled("Edit").Encoded(false).Filterable(false).Sortable(false)
                    .RenderedAs(model => "<div style='text-align:center'><a href='" + @Url.Action("Index", "RequestRequisition", new { area = "Admin", transId = Encrypt.EncryptMe(model.SLN.ToString()) }) + "' title='Edit'><i class='fa fa-edit'></i></a>");
                grid.Pager = new GridPager<TBL_BALANCE_TRANSFER_LOGS>(grid);
                grid.Processors.Add(grid.Pager);
                grid.Pager.RowsPerPage = 15;

                //foreach (IGridColumn column in grid.Columns)
                //{
                //    column.Filter.IsEnabled = true;
                //    column.Sort.IsEnabled = true;
                //}

                return grid;
            }
            else
            {
                DateTime Todaydate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.STATUS == "Pending" && x.TO_MEMBER == 0 && x.FROM_MEMBER == MemberCurrentUser.MEM_ID && x.REQUEST_DATE == Todaydate
                                       select new
                                       {
                                           Touser = "Power Admin",
                                           TransId = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           ReferenceNo = x.REFERENCE_NO,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           SLN = x.SLN,
                                           Pay_Mode = x.PAYMENT_METHOD
                                       }).AsEnumerable().Select(z => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           ToUser = z.Touser,
                                           TransactionID = z.TransId,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REFERENCE_NO = z.ReferenceNo,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           SLN = z.SLN,
                                           PAYMENT_METHOD = z.Pay_Mode
                                       }).ToList();

                IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = new Grid<TBL_BALANCE_TRANSFER_LOGS>(transactionlist);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.TransactionID).Titled("Trans Id");
                grid.Columns.Add(model => model.ToUser).Titled("To User");
                grid.Columns.Add(model => model.FromUser).Titled("From Member");
                grid.Columns.Add(model => model.REQUEST_DATE).Titled("Req Date").Formatted("{0:yyyy-MM-dd}").MultiFilterable(false);
                grid.Columns.Add(model => model.AMOUNT).Titled("Amount");
                grid.Columns.Add(model => model.PAYMENT_METHOD).Titled("Pay Mode");
                grid.Columns.Add(model => model.BANK_ACCOUNT).Titled("Bank Acnt");
                grid.Columns.Add(model => model.TRANSACTION_DETAILS).Titled("Txn Details");

                grid.Columns.Add(model => model.SLN).Titled("Edit").Encoded(false).Filterable(false).Sortable(false)
                    .RenderedAs(model => "<div style='text-align:center'><a href='" + @Url.Action("Index", "RequestRequisition", new { area = "Admin", transId = Encrypt.EncryptMe(model.SLN.ToString()) }) + "' title='Edit'><i class='fa fa-edit'></i></a>");
                grid.Pager = new GridPager<TBL_BALANCE_TRANSFER_LOGS>(grid);
                grid.Processors.Add(grid.Pager);
                grid.Pager.RowsPerPage = 15;

                //foreach (IGridColumn column in grid.Columns)
                //{
                //    column.Filter.IsEnabled = true;
                //    column.Sort.IsEnabled = true;
                //}

                return grid;
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> CheckReferenceNo(string referenceno)
        {
            initpage();////
            var context = new DBContext();
            var User = await context.TBL_BALANCE_TRANSFER_LOGS.Where(model => model.REFERENCE_NO == referenceno && model.FROM_MEMBER == MemberCurrentUser.MEM_ID).FirstOrDefaultAsync();
            if (User != null)
            {
                var list = context.TBL_WHITE_LEVEL_HOSTING_DETAILS.Where(x => x.MEM_ID == User.FROM_MEMBER).FirstOrDefault();
                return Json(new { result = "unavailable" });
                //return Json(new { result = "unavailable",Mem_Name= list.DOMAIN,mem_Id =User.FROM_MEMBER,Req_Date=User.REQUEST_DATE,amt=User.AMOUNT,Bankid=User.BANK_ACCOUNT,paymethod=User.PAYMENT_METHOD,Transdetails=User.TRANSACTION_DETAILS,BankCharges=User.BANK_CHARGES});
            }
            else
            {
                return Json(new { result = "available" });
            }
        }
    }
}