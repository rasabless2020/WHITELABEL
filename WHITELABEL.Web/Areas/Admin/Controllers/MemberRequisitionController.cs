using log4net;
using NonFactors.Mvc.Grid;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
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
    public class MemberRequisitionController : AdminBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        //public void initpage()
        //{
        //    try
        //    {
        //        ViewBag.ControllerName = "Member Hosting";
        //        SystemClass sclass = new SystemClass();
        //        string userID = sclass.GetLoggedUser();
        //        long userid = long.Parse(userID);
        //        var dbmain = new DBContext();
        //        if (userID != null && userID != "")
        //        {
        //            TBL_MASTER_MEMBER currUser = dbmain.TBL_MASTER_MEMBER.SingleOrDefault(c => c.MEM_ID == userid && c.ACTIVE_MEMBER == true && c.MEMBER_ROLE==1);
        //            if (currUser != null)
        //            {
        //                Session["WhiteLevelUserId"] = currUser.MEM_ID;
        //                // Session["UserName"] = currUser.UserName;
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

        public ActionResult Index()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                initpage();
                return View();
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
        public PartialViewResult IndexGrid(string DateFrom="",string Date_To="")
        {
            try
            {
                var db = new DBContext();
                //var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                //                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID where x.STATUS== "Pending"
                //                       select new
                //                       {
                //                           Touser = "PowerAdmin",
                //                           FromUser = y.UName,
                //                           REQUEST_DATE = x.REQUEST_DATE,
                //                           AMOUNT = x.AMOUNT,
                //                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                //                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                //                           SLN = x.SLN
                //                       }).AsEnumerable().Select(z => new TBL_BALANCE_TRANSFER_LOGS
                //                       {
                //                           ToUser = z.Touser,
                //                           FromUser = z.FromUser,
                //                           AMOUNT=z.AMOUNT,
                //                           REQUEST_DATE = z.REQUEST_DATE,
                //                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                //                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                //                           SLN = z.SLN
                //                       }).ToList();
                //return PartialView("IndexGrid", transactionlist);
                return PartialView(CreateExportableGridINExcel(DateFrom, Date_To));
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MemberRequisition(Admin), method:- IndexGrid (GET) Line No:- 114", ex);
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
                                       where x.STATUS == "Pending" && x.TO_MEMBER == MemberCurrentUser.MEM_ID && x.REQUEST_DATE>= Date_From_Val && x.REQUEST_DATE<= Date_To_Val
                                       select new
                                       {
                                           Touser = "White Label",
                                           TransId = x.TransactionID,
                                           FromUser = y.UName,
                                           MemberRole = db.TBL_MASTER_MEMBER_ROLE.FirstOrDefault(c => c.ROLE_ID == y.MEMBER_ROLE).ROLE_NAME,
                                           Reference_NO = x.REFERENCE_NO,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           SLN = x.SLN,
                                           INSERTED_BY = x.INSERTED_BY,
                                           InsertedByUserName = (db.TBL_MASTER_MEMBER.Where(s => s.MEM_ID == x.INSERTED_BY).Select(a => a.UName).FirstOrDefault()),
                                           PayMode = x.PAYMENT_METHOD,
                                           CompanyName=y.COMPANY
                                       }).AsEnumerable().Select(z => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           ToUser = z.Touser,
                                           TransactionID = z.TransId,
                                           REFERENCE_NO = z.Reference_NO,
                                           FromUser = z.FromUser,
                                           MemberRole = z.MemberRole,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           SLN = z.SLN,
                                           INSERTED_USERNAME = z.InsertedByUserName,
                                           PAYMENT_METHOD = z.PayMode,
                                           CompanyName=z.CompanyName
                                       }).ToList().OrderByDescending(x => x.SLN);

                IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = new Grid<TBL_BALANCE_TRANSFER_LOGS>(transactionlist);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.TransactionID).Titled("Trans Id");
                grid.Columns.Add(model => model.ToUser).Titled("To User");
                grid.Columns.Add(model => model.FromUser).Titled("From Member");
                grid.Columns.Add(model => model.MemberRole).Titled("Member Type");
                grid.Columns.Add(model => model.CompanyName).Titled("Company Name");
                grid.Columns.Add(model => model.REFERENCE_NO).Titled("Reference No");
                grid.Columns.Add(model => model.REQUEST_DATE).Formatted("{0:yyyy-MM-dd}").Titled("Req Date");
                grid.Columns.Add(model => model.AMOUNT).Titled("Amount");
                grid.Columns.Add(model => model.PAYMENT_METHOD).Titled("Pay Mode");
                grid.Columns.Add(model => model.BANK_ACCOUNT).Titled("Bank Acnt");
                grid.Columns.Add(model => model.TRANSACTION_DETAILS).Titled("Txn Details");
                grid.Columns.Add(model => model.INSERTED_USERNAME).Titled("Inserted By");
                grid.Columns.Add(model => model.SLN).Titled("Edit").Encoded(false).Filterable(false).Sortable(false)
                    .RenderedAs(model => "<div style='text-align:center'><a href='" + @Url.Action("RequisitionDetails", "MemberRequisition", new { area = "Admin", transId = Encrypt.EncryptMe(model.SLN.ToString()) }) + "' title='Edit'><i class='fa fa-edit'></i></a></div>");
                grid.Columns.Add(model => model.SLN).Titled("Action").Encoded(false).Filterable(false).Sortable(false)
                        .RenderedAs(model => "<button class='btn btn-primary btn-xs' data-toggle='modal' data-target='.transd' id='transactionvalueAdminid' data-id=" + model.SLN + " onclick='getvalue(" + model.SLN + ");'>Approve</button>");

                //grid.Columns.Add(model => model.SLN).Titled("Action").Encoded(false).Filterable(false).Sortable(false)
                //        .RenderedAs(model => "<button class='btn btn-danger btn-xs'  id='AdmintransactionvalueAdminid' data-id=" + model.SLN + " onclick='TransactionDecline(" + model.SLN + ")'>Decline</button>");


                grid.Pager = new GridPager<TBL_BALANCE_TRANSFER_LOGS>(grid);
                grid.Processors.Add(grid.Pager);
                grid.Pager.RowsPerPage = 15;
                grid.Pager.CssClasses = "table table-striped";
                
                foreach (IGridColumn column in grid.Columns)
                {
                    column.Filter.IsEnabled = true;
                    column.Sort.IsEnabled = true;
                    
                    column.CssClasses = "table table-striped";
                }

                return grid;
            }
            else
            {
                DateTime Todaydate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.STATUS == "Pending" && x.TO_MEMBER == MemberCurrentUser.MEM_ID && x.REQUEST_DATE == Todaydate
                                       //where x.STATUS == "Pending" && x.TO_MEMBER == MemberCurrentUser.MEM_ID
                                       select new
                                       {
                                           Touser = "White Label",
                                           TransId = x.TransactionID,
                                           FromUser = y.UName,
                                           MemberRole = db.TBL_MASTER_MEMBER_ROLE.FirstOrDefault(c => c.ROLE_ID == y.MEMBER_ROLE).ROLE_NAME,
                                           Reference_NO = x.REFERENCE_NO,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           SLN = x.SLN,
                                           INSERTED_BY = x.INSERTED_BY,
                                           InsertedByUserName = (db.TBL_MASTER_MEMBER.Where(s => s.MEM_ID == x.INSERTED_BY).Select(a => a.UName).FirstOrDefault()),
                                           PayMode = x.PAYMENT_METHOD,
                                           CompanyName = y.COMPANY
                                       }).AsEnumerable().Select(z => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           ToUser = z.Touser,
                                           TransactionID = z.TransId,
                                           REFERENCE_NO = z.Reference_NO,
                                           FromUser = z.FromUser,
                                           MemberRole = z.MemberRole,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           SLN = z.SLN,
                                           INSERTED_USERNAME = z.InsertedByUserName,
                                           PAYMENT_METHOD = z.PayMode,
                                           CompanyName = z.CompanyName
                                       }).ToList().OrderByDescending(x => x.SLN);

                IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = new Grid<TBL_BALANCE_TRANSFER_LOGS>(transactionlist);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.TransactionID).Titled("Trans Id");
                grid.Columns.Add(model => model.ToUser).Titled("To User");
                grid.Columns.Add(model => model.FromUser).Titled("From Member");
                grid.Columns.Add(model => model.MemberRole).Titled("Member Type");
                grid.Columns.Add(model => model.CompanyName).Titled("Company Name");
                grid.Columns.Add(model => model.REFERENCE_NO).Titled("Reference No");
                grid.Columns.Add(model => model.REQUEST_DATE).Titled("Req Date").Formatted("{0:yyyy-MM-dd}").MultiFilterable(true);
                grid.Columns.Add(model => model.AMOUNT).Titled("Amount");
                grid.Columns.Add(model => model.PAYMENT_METHOD).Titled("Pay Mode");
                grid.Columns.Add(model => model.BANK_ACCOUNT).Titled("Bank Acnt");
                grid.Columns.Add(model => model.TRANSACTION_DETAILS).Titled("Txn Details");
                grid.Columns.Add(model => model.INSERTED_USERNAME).Titled("Inserted By");
                grid.Columns.Add(model => model.SLN).Titled("Edit").Encoded(false).Filterable(false).Sortable(false)
                    .RenderedAs(model => "<div style='text-align:center'><a href='" + @Url.Action("RequisitionDetails", "MemberRequisition", new { area = "Admin", transId = Encrypt.EncryptMe(model.SLN.ToString()) }) + "' title='Edit'><i class='fa fa-edit'></i></a></div>");
                grid.Columns.Add(model => model.SLN).Titled("Action").Encoded(false).Filterable(false).Sortable(false)
                        .RenderedAs(model => "<button class='btn btn-primary btn-xs' data-toggle='modal' data-target='.transd' id='transactionvalueAdminid' data-id=" + model.SLN + " onclick='getvalue(" + model.SLN + ");'>Approve</button>");

                //grid.Columns.Add(model => model.SLN).Titled("Action").Encoded(false).Filterable(false).Sortable(false)
                //        .RenderedAs(model => "<button class='btn btn-danger btn-xs'  id='AdmintransactionvalueAdminid' data-id=" + model.SLN + " onclick='TransactionDecline(" + model.SLN + ")'>Decline</button>");


                grid.Pager = new GridPager<TBL_BALANCE_TRANSFER_LOGS>(grid);
                grid.Processors.Add(grid.Pager);
                grid.Pager.RowsPerPage = 15;
                grid.Attributes.Add("css-classes", grid.Rows.CssClasses);
                grid.Attributes.Add("table table-striped", grid.Rows.CssClasses);
                

                //foreach (IGridColumn column in grid.Columns)
                //{
                //    column.Filter.IsEnabled = true;
                //    column.Sort.IsEnabled = true;
                //    //column.CssClasses = "table table-striped";
                //}


                return grid;
            }

            
        }

        public ActionResult RequisitionDetails(string transId = "")
        {
            initpage();
            if (Session["WhiteLevelUserId"] != null)
            {
                try
                {
                    var db = new DBContext();
                    if (transId == "")
                    {
                        var memberService = (from x in db.TBL_MASTER_MEMBER
                                                 //where x.CREATED_BY == MemberCurrentUser.MEM_ID
                                             where x.INTRODUCER == MemberCurrentUser.MEM_ID && x.MEMBER_ROLE==4
                                             select new
                                             {
                                                 MEM_ID = x.MEM_ID,
                                                 UName = x.MEMBER_NAME
                                             }).AsEnumerable().Select(z => new MemberView
                                             {
                                                 IDValue = z.MEM_ID.ToString(),
                                                 TextValue = z.UName
                                             }).ToList().Distinct();
                        ViewBag.MemberService = new SelectList(memberService, "IDValue", "TextValue");

                        var BankInformation = (from x in db.TBL_SETTINGS_BANK_DETAILS
                                               where x.MEM_ID == MemberCurrentUser.MEM_ID
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
                        string decripttransId = Decrypt.DecryptMe(transId);
                        long transID = long.Parse(decripttransId);
                        var TransactionInfo = db.TBL_BALANCE_TRANSFER_LOGS.Where(x => x.SLN == transID).FirstOrDefault();
                        var memberService = (from x in db.TBL_MASTER_MEMBER
                                             where x.MEM_ID == TransactionInfo.FROM_MEMBER
                                             select new
                                             {
                                                 MEM_ID = x.MEM_ID,
                                                 UName = x.UName
                                             }).AsEnumerable().Select(z => new MemberView
                                             {
                                                 IDValue = z.MEM_ID.ToString(),
                                                 TextValue = z.UName
                                             }).ToList().Distinct();
                        ViewBag.MemberService = new SelectList(memberService, "IDValue", "TextValue");

                        var BankInformation = (from x in db.TBL_SETTINGS_BANK_DETAILS
                                               where x.MEM_ID == MemberCurrentUser.MEM_ID
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
                        TransactionInfo.FromUser = memberService.Select(x => x.TextValue).FirstOrDefault();                        
                        TransactionInfo.PAYMENT_METHOD = TransactionInfo.PAYMENT_METHOD;                        
                        TransactionInfo.REQUEST_DATE = Convert.ToDateTime(TransactionInfo.REQUEST_DATE.ToString("yyyy-MM-dd").Substring(0, 10));                        
                        TransactionInfo.BANK_ACCOUNT = TransactionInfo.BANK_ACCOUNT;
                        TransactionInfo.TRANSACTION_DETAILS = TransactionInfo.TRANSACTION_DETAILS;
                        TransactionInfo.BANK_CHARGES = TransactionInfo.BANK_CHARGES;
                        ViewBag.checkbank = "1";
                        return View(TransactionInfo);
                    }

                }
                catch (Exception ex)
                {
                    Logger.Error("Controller:-  MemberRequisition(Admin), method:- RequisitionDetails (GET) Line No:- 293", ex);
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
        //[ValidateInput(true)]
        public async Task<ActionResult> RequisitionDetails(TBL_BALANCE_TRANSFER_LOGS objval)
        {
            initpage();////
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    //long userId = long.Parse(objval.FromUser);
                    // var membertype = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == userId).FirstOrDefault();
                    var checkAvailableMember = db.TBL_MASTER_MEMBER.Where(x=>x.MEM_ID==objval.FROM_MEMBER).FirstOrDefault();
                    if (checkAvailableMember != null)
                    {

                        var translist = await db.TBL_BALANCE_TRANSFER_LOGS.Where(x => x.SLN == objval.SLN).FirstOrDefaultAsync();
                        if (translist != null)
                        {
                            translist.REQUEST_DATE = objval.REQUEST_DATE;
                            translist.REQUEST_TIME = System.DateTime.Now;
                            translist.BANK_ACCOUNT = objval.BANK_ACCOUNT;
                            translist.AMOUNT = objval.AMOUNT;
                            translist.PAYMENT_METHOD = objval.PAYMENT_METHOD;
                            translist.TRANSFER_METHOD = "Cash";
                            translist.FromUser = "test";
                            translist.TRANSACTION_DETAILS = objval.TRANSACTION_DETAILS;
                            translist.BANK_CHARGES = objval.BANK_CHARGES;
                            translist.INSERTED_BY = MemberCurrentUser.MEM_ID;
                            db.Entry(translist).State = System.Data.Entity.EntityState.Modified;
                            await db.SaveChangesAsync();
                            //return RedirectToAction("Index");
                        }
                        else
                        {
                            //long fromuser = long.Parse(objval.FromUser);
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
                                objval.TransactionID = MemberCurrentUser.MEM_ID + "" + fromuser + DateTime.Now.ToString("yyyyMMdd") + "" + DateTime.Now.ToString("HHMMss");
                                objval.TO_MEMBER = MemberCurrentUser.MEM_ID;
                                objval.FROM_MEMBER = fromuser;
                                objval.REQUEST_DATE = Convert.ToDateTime(objval.REQUEST_DATE);
                                objval.REQUEST_TIME = System.DateTime.Now;
                                objval.BANK_ACCOUNT = objval.BANK_ACCOUNT;
                                objval.STATUS = "Pending";
                                objval.TRANSFER_METHOD = "Cash";
                                objval.BANK_CHARGES = objval.BANK_CHARGES;
                                objval.INSERTED_BY = MemberCurrentUser.MEM_ID;
                                db.TBL_BALANCE_TRANSFER_LOGS.Add(objval);
                                await db.SaveChangesAsync();
                            }

                            Logger.Info("");
                            //return RedirectToAction("Index");
                        }
                        ContextTransaction.Commit();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        var BankInformation = (from x in db.TBL_SETTINGS_BANK_DETAILS
                                               where x.MEM_ID == MemberCurrentUser.MEM_ID
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
                        ViewBag.msg = "Please provide valid super distributor user name";
                        return View("RequisitionDetails");
                    }
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    Logger.Error("Controller:-  MemberRequisition(Admin), method:- RequisitionDetails (POST) Line No:- 363", ex);
                    return RedirectToAction("Exception", "ErrorHandler", new { area = "" });
                    throw ex;
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeactivateTransactionDetails(string transid = "")
        {
            initpage();////
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    long TransID = long.Parse(transid);
                    var transDeactivate = await db.TBL_BALANCE_TRANSFER_LOGS.Where(x => x.SLN == TransID).FirstOrDefaultAsync();
                    transDeactivate.STATUS = "Decline";
                    db.Entry(transDeactivate).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();
                    ContextTransaction.Commit();
                    return Json(new { Result = "true" });
                }
                catch (Exception ex)
                {
                    Logger.Error("Controller:-  MemberRequisition(Admin), method:- DeactivateTransactionDetails (POST) Line No:- 389", ex);
                    ContextTransaction.Rollback();
                    return Json(new { Result = "false" });
                }
            }

        }

        [HttpPost]        
        public JsonResult getTransdata(string TransId = "")
        {
            initpage();////
            try
            {
                var db = new DBContext();
                long transid = long.Parse(TransId);
                //var listdetails = db.TBL_BALANCE_TRANSFER_LOGS.Where(x => x.SLN == transid).FirstOrDefault();
                var listdetails = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                   join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                   where x.SLN == transid
                                   select new
                                   {
                                       Touser = "PowerAdmin",
                                       FromUser = y.UName,
                                       REQUEST_DATE = x.REQUEST_DATE,
                                       AMOUNT = x.AMOUNT,
                                       BANK_ACCOUNT = x.BANK_ACCOUNT,
                                       TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                       SLN = x.SLN
                                   }).AsEnumerable().Select(z => new TBL_BALANCE_TRANSFER_LOGS
                                   {
                                       ToUser = z.Touser,
                                       FromUser = z.FromUser,
                                       AMOUNT = z.AMOUNT,
                                       REQUEST_DATE = z.REQUEST_DATE,
                                       BANK_ACCOUNT = z.BANK_ACCOUNT,
                                       TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                       SLN = z.SLN
                                   }).FirstOrDefault();

                return Json(new { Result = "true", data = listdetails });
            }
            catch (Exception ex)
            {
                var listdetails = new TBL_BALANCE_TRANSFER_LOGS();
                Logger.Error("Controller:-  MemberRequisition(Admin), method:- getTransdata (POST) Line No:- 433", ex);
                return Json(new { Result = "false", data = listdetails });
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<JsonResult> ChangeTransactionStatus(string trandate = "", string TransationStatus = "", string slnval = "")
        //public async Task<JsonResult> ChangeTransactionStatus(string slnval = "")
        public async Task<JsonResult> ChangeTransactionStatus(string slnval = "", string SettlementTYPE = "",string PaymentTrnId="")
        {
            initpage();////
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    string COrelationID = Settings.GetUniqueKey(MemberCurrentUser.MEM_ID.ToString());
                    long sln = long.Parse(slnval);
                    decimal addamount = 0;
                    decimal memb_bal = 0;
                    decimal trans_bal = 0;
                    decimal availableamt = 0;
                    decimal addavilamt = 0;
                    decimal closingamt = 0;
                    decimal addclosingamt = 0;

                    decimal Dist_MainBal = 0;
                    decimal MER_MainBal = 0;
                    decimal Trans_Req_Amount = 0;
                    decimal Dist_Sub_MainAmount = 0;
                    decimal Mer_Add_MainBalance = 0;
                    decimal DIST_CLOSING_AMT = 0;
                    decimal DIST_ADJ_CLOSING_AMT = 0;
                    decimal MER_CLOSING_AMT = 0;
                    decimal MER_ADJ_CLOSING_AMT = 0;

                    decimal CREDITED_REQ_AMT = 0;
                    decimal MER_CR_LMT_AMT = 0;
                    decimal MER_PER_CR_LIT_AMT = 0;
                    string MemberType = string.Empty;

                    var transinfo = await db.TBL_BALANCE_TRANSFER_LOGS.Where(x => x.SLN == sln).FirstOrDefaultAsync();
                    decimal.TryParse(transinfo.AMOUNT.ToString(), out Trans_Req_Amount);

                    #region For check Merchant Credit Balance
                    //var DIST_ACCOUNT = await db.TBL_ACCOUNTS.Where(x => x.MEM_ID == transinfo.TO_MEMBER).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefaultAsync();
                    var DIST_ACCOUNT = await db.TBL_ACCOUNTS.Where(x => x.MEM_ID == transinfo.TO_MEMBER).OrderByDescending(z => z.ACC_NO).FirstOrDefaultAsync();
                    if (DIST_ACCOUNT != null)
                    {
                        decimal.TryParse(DIST_ACCOUNT.CLOSING.ToString(), out DIST_CLOSING_AMT);
                    }
                    else
                    {
                        DIST_CLOSING_AMT = 0;
                    }

                    //var MER_ACCOUNT = await db.TBL_ACCOUNTS.Where(x => x.MEM_ID == transinfo.FROM_MEMBER).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefaultAsync();
                    var MER_ACCOUNT = await db.TBL_ACCOUNTS.Where(x => x.MEM_ID == transinfo.FROM_MEMBER).OrderByDescending(z => z.ACC_NO).FirstOrDefaultAsync();
                    if (MER_ACCOUNT != null)
                    {
                        decimal.TryParse(MER_ACCOUNT.CLOSING.ToString(), out MER_CLOSING_AMT);
                    }
                    else
                    {
                        MER_CLOSING_AMT = 0;
                    }

                    var Distributor_Info = await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == transinfo.TO_MEMBER);
                    decimal.TryParse(Distributor_Info.BALANCE.ToString(), out Dist_MainBal);
                    var Merchant_Info = await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == transinfo.FROM_MEMBER);
                    if (Merchant_Info.MEMBER_ROLE == 4)
                    {
                        MemberType = "DISTRIBUTOR";
                    }
                    else
                    {
                        MemberType = "RETAILER";
                    }
                    decimal.TryParse(Merchant_Info.BALANCE.ToString(), out MER_MainBal);
                    decimal.TryParse(Merchant_Info.CREDIT_LIMIT.ToString(), out MER_CR_LMT_AMT);
                    decimal.TryParse(Merchant_Info.RESERVED_CREDIT_LIMIT.ToString(), out MER_PER_CR_LIT_AMT);
                    if (Merchant_Info.CREDIT_LIMIT <= 0)
                    {

                        if (Dist_MainBal >= Trans_Req_Amount)
                        {
                            transinfo.STATUS = "Approve";
                            transinfo.APPROVAL_DATE = DateTime.Now;
                            transinfo.APPROVAL_TIME = DateTime.Now;
                            transinfo.FromUser = "test";
                            transinfo.REMARKS = SettlementTYPE;
                            transinfo.APPROVED_BY = "ADMIN TRAVELIQ";
                            transinfo.PAYMENT_TXN_DETAILS = PaymentTrnId;
                            db.Entry(transinfo).State = System.Data.Entity.EntityState.Modified;
                            //await db.SaveChangesAsync();

                            //Distributor Main Balanve Update and insert innfor in Account table
                            //Dist_Sub_MainAmount = Dist_MainBal - Trans_Req_Amount;
                            //Distributor_Info.BALANCE = Dist_Sub_MainAmount;
                            //db.Entry(Distributor_Info).State = System.Data.Entity.EntityState.Modified;
                            //if (DIST_CLOSING_AMT > 0)
                            //{
                            //    DIST_ADJ_CLOSING_AMT = DIST_CLOSING_AMT - Trans_Req_Amount;
                            //}
                            //else
                            //{
                            //    DIST_ADJ_CLOSING_AMT = 0;
                            //}
                            //TBL_ACCOUNTS DIST_objACCOUNT = new TBL_ACCOUNTS()
                            //{
                            //    API_ID = 0,
                            //    MEM_ID = transinfo.FROM_MEMBER,
                            //    MEMBER_TYPE = "DISTRIBUTOR",
                            //    TRANSACTION_TYPE = transinfo.PAYMENT_METHOD,
                            //    TRANSACTION_DATE = Convert.ToDateTime(transinfo.REQUEST_DATE),
                            //    TRANSACTION_TIME = DateTime.Now,
                            //    DR_CR = "DR",
                            //    //AMOUNT = decimal.Parse(transinfo.AMOUNT.ToString()),
                            //    AMOUNT = Trans_Req_Amount,
                            //    NARRATION = transinfo.TRANSACTION_DETAILS,
                            //    OPENING = DIST_CLOSING_AMT,
                            //    CLOSING = DIST_ADJ_CLOSING_AMT,
                            //    REC_NO = 0,
                            //    COMM_AMT = 0,
                            //    TDS = 0,
                            //    GST = 0,
                            //    IPAddress = "",
                            //    SERVICE_ID = 0,
                            //    CORELATIONID = COrelationID
                            //};
                            //db.TBL_ACCOUNTS.Add(DIST_objACCOUNT);
                            //END Distributor Main Balanve Update and insert innfor in Account table
                            Mer_Add_MainBalance = MER_MainBal + Trans_Req_Amount;
                            Merchant_Info.BALANCE = Mer_Add_MainBalance;
                            db.Entry(Merchant_Info).State = System.Data.Entity.EntityState.Modified;
                            if (MER_CLOSING_AMT > 0)
                            {
                                MER_ADJ_CLOSING_AMT = MER_CLOSING_AMT + Trans_Req_Amount;
                            }
                            else
                            {
                                MER_ADJ_CLOSING_AMT = Trans_Req_Amount;
                            }
                            TBL_ACCOUNTS MERCH_objACCOUNT = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = transinfo.FROM_MEMBER,
                                MEMBER_TYPE = MemberType,
                                //TRANSACTION_TYPE = transinfo.PAYMENT_METHOD,
                                TRANSACTION_TYPE ="DEPOSIT",
                                TRANSACTION_DATE = Convert.ToDateTime(transinfo.REQUEST_DATE),
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                //AMOUNT = decimal.Parse(transinfo.AMOUNT.ToString()),
                                AMOUNT = Trans_Req_Amount,
                                NARRATION = transinfo.TRANSACTION_DETAILS,
                                OPENING = MER_CLOSING_AMT,
                                CLOSING = MER_ADJ_CLOSING_AMT,
                                REC_NO = 0,
                                COMM_AMT = 0,
                                TDS = 0,
                                GST = 0,
                                IPAddress = "",
                                SERVICE_ID = 0,
                                CORELATIONID = COrelationID
                            };
                            db.TBL_ACCOUNTS.Add(MERCH_objACCOUNT);
                            await db.SaveChangesAsync();
                            ContextTransaction.Commit();
                            return Json("Transaction Approve.", JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            string msg = Distributor_Info.UName + " (Distributor) don't have sufficient balance to approve this transaction.";
                            return Json(msg, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        if (SettlementTYPE == "Full")
                        {
                            CREDITED_REQ_AMT = Trans_Req_Amount - MER_CR_LMT_AMT;
                            if (CREDITED_REQ_AMT > 0)
                            {
                                if (Dist_MainBal >= CREDITED_REQ_AMT)
                                {
                                    transinfo.STATUS = "Approve";
                                    transinfo.APPROVAL_DATE = DateTime.Now;
                                    transinfo.APPROVAL_TIME = DateTime.Now;
                                    transinfo.REMARKS = SettlementTYPE;
                                    transinfo.FromUser = "test";
                                    transinfo.APPROVED_BY = "ADMIN TRAVELIQ";
                                    transinfo.PAYMENT_TXN_DETAILS = PaymentTrnId;
                                    db.Entry(transinfo).State = System.Data.Entity.EntityState.Modified;

                                    ////Distributor Main Balanve Update and insert innfor in Account table
                                    //Dist_Sub_MainAmount = Dist_MainBal - CREDITED_REQ_AMT;
                                    //Distributor_Info.BALANCE = Dist_Sub_MainAmount;
                                    //db.Entry(Distributor_Info).State = System.Data.Entity.EntityState.Modified;
                                    //if (DIST_CLOSING_AMT > 0)
                                    //{
                                    //    DIST_ADJ_CLOSING_AMT = DIST_CLOSING_AMT - CREDITED_REQ_AMT;
                                    //}
                                    //else
                                    //{
                                    //    DIST_ADJ_CLOSING_AMT = 0;
                                    //}
                                    //TBL_ACCOUNTS DIST_objACCOUNT = new TBL_ACCOUNTS()
                                    //{
                                    //    API_ID = 0,
                                    //    MEM_ID = transinfo.FROM_MEMBER,
                                    //    MEMBER_TYPE = "DISTRIBUTOR",
                                    //    TRANSACTION_TYPE = transinfo.PAYMENT_METHOD,
                                    //    TRANSACTION_DATE = Convert.ToDateTime(transinfo.REQUEST_DATE),
                                    //    TRANSACTION_TIME = DateTime.Now,
                                    //    DR_CR = "DR",
                                    //    //AMOUNT = decimal.Parse(transinfo.AMOUNT.ToString()),
                                    //    AMOUNT = CREDITED_REQ_AMT,
                                    //    NARRATION = transinfo.TRANSACTION_DETAILS,
                                    //    OPENING = DIST_CLOSING_AMT,
                                    //    CLOSING = DIST_ADJ_CLOSING_AMT,
                                    //    REC_NO = 0,
                                    //    COMM_AMT = 0,
                                    //    TDS = 0,
                                    //    GST = 0,
                                    //    IPAddress = "",
                                    //    SERVICE_ID = 0,
                                    //    CORELATIONID = COrelationID
                                    //};
                                    //db.TBL_ACCOUNTS.Add(DIST_objACCOUNT);
                                    ////END Distributor Main Balanve Update and insert innfor in Account table
                                    Mer_Add_MainBalance = MER_MainBal + Trans_Req_Amount - CREDITED_REQ_AMT;
                                    Merchant_Info.BALANCE = Mer_Add_MainBalance;
                                    Merchant_Info.CREDIT_LIMIT = 0;
                                    //Merchant_Info.RESERVED_CREDIT_LIMIT = 0;
                                    db.Entry(Merchant_Info).State = System.Data.Entity.EntityState.Modified;
                                    if (MER_CLOSING_AMT > 0)
                                    {
                                        MER_ADJ_CLOSING_AMT = MER_CLOSING_AMT + Trans_Req_Amount;
                                    }
                                    else
                                    {
                                        MER_ADJ_CLOSING_AMT = Trans_Req_Amount;
                                    }
                                    TBL_ACCOUNTS MERCH_CreeditobjACCOUNT = new TBL_ACCOUNTS()
                                    {
                                        API_ID = 0,
                                        MEM_ID = transinfo.FROM_MEMBER,
                                        MEMBER_TYPE = MemberType,
                                        //TRANSACTION_TYPE = transinfo.PAYMENT_METHOD,
                                        TRANSACTION_TYPE = "DEPOSIT",
                                        TRANSACTION_DATE = Convert.ToDateTime(transinfo.REQUEST_DATE),
                                        TRANSACTION_TIME = DateTime.Now,
                                        DR_CR = "CR",
                                        //AMOUNT = decimal.Parse(transinfo.AMOUNT.ToString()),
                                        AMOUNT = Trans_Req_Amount,
                                        NARRATION = transinfo.TRANSACTION_DETAILS,
                                        OPENING = MER_CLOSING_AMT,
                                        CLOSING = MER_ADJ_CLOSING_AMT,
                                        REC_NO = 0,
                                        COMM_AMT = 0,
                                        TDS = 0,
                                        GST = 0,
                                        IPAddress = "",
                                        SERVICE_ID = 0,
                                        CORELATIONID = COrelationID
                                    };
                                    db.TBL_ACCOUNTS.Add(MERCH_CreeditobjACCOUNT);

                                    decimal MERCH_Deduct_CR_AMT = 0;

                                    //MERCH_Deduct_CR_AMT = MER_ADJ_CLOSING_AMT - CREDITED_REQ_AMT;
                                    MERCH_Deduct_CR_AMT = MER_ADJ_CLOSING_AMT - MER_CR_LMT_AMT;
                                    
                                    TBL_ACCOUNTS MERCH_objDECUR_AMT = new TBL_ACCOUNTS()
                                    {
                                        API_ID = 0,
                                        MEM_ID = transinfo.FROM_MEMBER,
                                        MEMBER_TYPE = MemberType,
                                        //TRANSACTION_TYPE = transinfo.PAYMENT_METHOD,
                                        TRANSACTION_TYPE = "CREDIT SETTLEMENT",
                                        TRANSACTION_DATE = Convert.ToDateTime(transinfo.REQUEST_DATE),
                                        TRANSACTION_TIME = DateTime.Now,
                                        DR_CR = "DR",
                                        //AMOUNT = decimal.Parse(transinfo.AMOUNT.ToString()),
                                        AMOUNT = MER_CR_LMT_AMT,
                                        NARRATION = transinfo.TRANSACTION_DETAILS,
                                        OPENING = MER_ADJ_CLOSING_AMT,
                                        CLOSING = MERCH_Deduct_CR_AMT,
                                        REC_NO = 0,
                                        COMM_AMT = 0,
                                        TDS = 0,
                                        GST = 0,
                                        IPAddress = "",
                                        SERVICE_ID = 0,
                                        CORELATIONID = COrelationID
                                    };
                                    db.TBL_ACCOUNTS.Add(MERCH_objDECUR_AMT);


                                    TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION OBJMERCHANTCREDIT_VAL = new TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION()
                                    {
                                        TO_MEM_ID = transinfo.TO_MEMBER,
                                        FROM_MEM_ID = transinfo.FROM_MEMBER,
                                        CREDIT_DATE = DateTime.Now,
                                        CREDIT_AMOUNT = MER_CR_LMT_AMT,
                                        CREDIT_NOTE_DESCRIPTION = "CREDIT LIMIT AMOUNT LOGS",
                                        CREDIT_STATUS = true,
                                        GST_VAL = 0,
                                        GST_AMOUNT = 0,
                                        TDS_AMOUNT = 0,
                                        TDS_VAL = 0,
                                        CREDIT_OPENING = 0,
                                        CREDITCLOSING = 0,
                                        CREDIT_TRN_TYPE = "DR",
                                        CORELATIONID = COrelationID
                                    };
                                    db.TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION.Add(OBJMERCHANTCREDIT_VAL);

                                    //TBL_MEMBER_CREDIT_ACCOUNT_LOGS MEM_CREDIT_ACNT_LOGS = new TBL_MEMBER_CREDIT_ACCOUNT_LOGS()
                                    //{
                                    //    MEM_ID = MemberCurrentUser.MEM_ID,
                                    //    CREDIT_ID = MemberCurrentUser.MEM_ID,
                                    //    CORELATIONID = COrelationID,
                                    //    CREDIT_TRAN_TYPE = "DR",
                                    //    USED_CREDIT_AMOUNT = CREDITED_REQ_AMT,
                                    //    CREDIT_OPENING_BALANCE = 0,
                                    //    CREDIT_CLOSING_BALANCE = 0,
                                    //    CREDIT_USED_DATE = DateTime.Now,
                                    //    IPADDRESS = "",
                                    //    WLP_ID = (long)Merchant_Info.UNDER_WHITE_LEVEL,
                                    //    DIST_ID = (long)Merchant_Info.INTRODUCER,
                                    //    SUPER_ID = 0,
                                    //    MER_ID = MemberCurrentUser.MEM_ID,
                                    //    NARRATION = "CREDIT LIMIT AMOUNT LOGS"
                                    //};
                                    //db.TBL_MEMBER_CREDIT_ACCOUNT_LOGS.Add(MEM_CREDIT_ACNT_LOGS);
                                    await db.SaveChangesAsync();
                                    ContextTransaction.Commit();
                                    return Json("Transaction Approve.", JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    string msg = "Credit Requested Amount is Insufficiant After Calculation Merchant Full Settlement Amount of Rs." + MER_CR_LMT_AMT;
                                    return Json(msg, JsonRequestBehavior.AllowGet);
                                    //CREDITED_REQ_AMT IS INSUFFICIANT AFTER CALULATION MERCHANT FULL SETTLEMENT AMOUNT OF RS. MER_CR_LMT_AMT
                                }
                            }
                            else if (CREDITED_REQ_AMT == 0)
                            {
                                transinfo.STATUS = "Approve";
                                transinfo.APPROVAL_DATE = DateTime.Now;
                                transinfo.APPROVAL_TIME = DateTime.Now;
                                transinfo.FromUser = "test";
                                transinfo.REMARKS = SettlementTYPE;
                                transinfo.APPROVED_BY = "ADMIN TRAVELIQ";
                                transinfo.PAYMENT_TXN_DETAILS = PaymentTrnId;
                                db.Entry(transinfo).State = System.Data.Entity.EntityState.Modified;


                                ////Distributor Main Balanve Update and insert innfor in Account table
                                //Dist_Sub_MainAmount = Dist_MainBal - CREDITED_REQ_AMT;
                                ////Dist_Sub_MainAmount =0;
                                //Distributor_Info.BALANCE = Dist_Sub_MainAmount;
                                //db.Entry(Distributor_Info).State = System.Data.Entity.EntityState.Modified;
                                //if (DIST_CLOSING_AMT > 0)
                                //{
                                //    DIST_ADJ_CLOSING_AMT = DIST_CLOSING_AMT - CREDITED_REQ_AMT;
                                //    //DIST_ADJ_CLOSING_AMT =0;
                                //}
                                //else
                                //{
                                //    DIST_ADJ_CLOSING_AMT = 0;
                                //}
                                //TBL_ACCOUNTS DIST_objACCOUNT = new TBL_ACCOUNTS()
                                //{
                                //    API_ID = 0,
                                //    MEM_ID = transinfo.FROM_MEMBER,
                                //    MEMBER_TYPE = "DISTRIBUTOR",
                                //    TRANSACTION_TYPE = transinfo.PAYMENT_METHOD,
                                //    TRANSACTION_DATE = Convert.ToDateTime(transinfo.REQUEST_DATE),
                                //    TRANSACTION_TIME = DateTime.Now,
                                //    DR_CR = "DR",
                                //    //AMOUNT = decimal.Parse(transinfo.AMOUNT.ToString()),
                                //    AMOUNT = CREDITED_REQ_AMT,
                                //    NARRATION = transinfo.TRANSACTION_DETAILS,
                                //    OPENING = DIST_CLOSING_AMT,
                                //    CLOSING = DIST_ADJ_CLOSING_AMT,
                                //    REC_NO = 0,
                                //    COMM_AMT = 0,
                                //    TDS = 0,
                                //    GST = 0,
                                //    IPAddress = "",
                                //    SERVICE_ID = 0,
                                //    CORELATIONID = COrelationID
                                //};
                                //db.TBL_ACCOUNTS.Add(DIST_objACCOUNT);
                                ////END Distributor Main Balanve Update and insert innfor in Account table
                                //Mer_Add_MainBalance = MER_MainBal + Trans_Req_Amount - CREDITED_REQ_AMT;
                                Mer_Add_MainBalance = MER_MainBal + Trans_Req_Amount - Trans_Req_Amount;
                                Merchant_Info.CREDIT_LIMIT = 0;
                                //Merchant_Info.RESERVED_CREDIT_LIMIT = 0;
                                Merchant_Info.BALANCE = Mer_Add_MainBalance;
                                db.Entry(Merchant_Info).State = System.Data.Entity.EntityState.Modified;
                                if (MER_CLOSING_AMT > 0)
                                {
                                    MER_ADJ_CLOSING_AMT = MER_CLOSING_AMT + Trans_Req_Amount;
                                }
                                else
                                {
                                    MER_ADJ_CLOSING_AMT = Trans_Req_Amount;
                                }
                                TBL_ACCOUNTS MERCH_CreeditobjACCOUNT = new TBL_ACCOUNTS()
                                {
                                    API_ID = 0,
                                    MEM_ID = transinfo.FROM_MEMBER,
                                    MEMBER_TYPE = MemberType,
                                    //TRANSACTION_TYPE = transinfo.PAYMENT_METHOD,
                                    TRANSACTION_TYPE = "DEPOSIT",
                                    TRANSACTION_DATE = Convert.ToDateTime(transinfo.REQUEST_DATE),
                                    TRANSACTION_TIME = DateTime.Now,
                                    DR_CR = "CR",
                                    //AMOUNT = decimal.Parse(transinfo.AMOUNT.ToString()),
                                    AMOUNT = Trans_Req_Amount,
                                    NARRATION = transinfo.TRANSACTION_DETAILS,
                                    OPENING = MER_CLOSING_AMT,
                                    CLOSING = MER_ADJ_CLOSING_AMT,
                                    REC_NO = 0,
                                    COMM_AMT = 0,
                                    TDS = 0,
                                    GST = 0,
                                    IPAddress = "",
                                    SERVICE_ID = 0,
                                    CORELATIONID = COrelationID
                                };
                                db.TBL_ACCOUNTS.Add(MERCH_CreeditobjACCOUNT);

                                decimal MERCH_Deduct_CR_AMT = 0;

                                //MERCH_Deduct_CR_AMT = MER_ADJ_CLOSING_AMT - CREDITED_REQ_AMT;
                                MERCH_Deduct_CR_AMT = MER_ADJ_CLOSING_AMT - Trans_Req_Amount;
                                
                                TBL_ACCOUNTS MERCH_objDECUR_AMT = new TBL_ACCOUNTS()
                                {
                                    API_ID = 0,
                                    MEM_ID = transinfo.FROM_MEMBER,
                                    MEMBER_TYPE = MemberType,
                                    //TRANSACTION_TYPE = transinfo.PAYMENT_METHOD,
                                    TRANSACTION_TYPE = "CREDIT SETTLEMENT",
                                    TRANSACTION_DATE = Convert.ToDateTime(transinfo.REQUEST_DATE),
                                    TRANSACTION_TIME = DateTime.Now,
                                    DR_CR = "DR",
                                    //AMOUNT = decimal.Parse(transinfo.AMOUNT.ToString()),
                                    AMOUNT = Trans_Req_Amount,
                                    NARRATION = transinfo.TRANSACTION_DETAILS,
                                    OPENING = MER_ADJ_CLOSING_AMT,
                                    CLOSING = MERCH_Deduct_CR_AMT,
                                    REC_NO = 0,
                                    COMM_AMT = 0,
                                    TDS = 0,
                                    GST = 0,
                                    IPAddress = "",
                                    SERVICE_ID = 0,
                                    CORELATIONID = COrelationID
                                };
                                db.TBL_ACCOUNTS.Add(MERCH_objDECUR_AMT);


                                TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION OBJMERCHANTCREDIT_VAL = new TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION()
                                {
                                    TO_MEM_ID = transinfo.TO_MEMBER,
                                    FROM_MEM_ID = transinfo.FROM_MEMBER,
                                    CREDIT_DATE = DateTime.Now,
                                    CREDIT_AMOUNT = Trans_Req_Amount,
                                    CREDIT_NOTE_DESCRIPTION = "CREDIT LIMIT AMOUNT LOGS",
                                    CREDIT_STATUS = true,
                                    GST_VAL = 0,
                                    GST_AMOUNT = 0,
                                    TDS_AMOUNT = 0,
                                    TDS_VAL = 0,
                                    CREDIT_OPENING = 0,
                                    CREDITCLOSING = 0,
                                    CREDIT_TRN_TYPE = "DR",
                                    CORELATIONID = COrelationID
                                };
                                db.TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION.Add(OBJMERCHANTCREDIT_VAL);

                                //TBL_MEMBER_CREDIT_ACCOUNT_LOGS MEM_CREDIT_ACNT_LOGS = new TBL_MEMBER_CREDIT_ACCOUNT_LOGS()
                                //{
                                //    MEM_ID = MemberCurrentUser.MEM_ID,
                                //    CREDIT_ID = MemberCurrentUser.MEM_ID,
                                //    CORELATIONID = COrelationID,
                                //    CREDIT_TRAN_TYPE = "DR",
                                //    USED_CREDIT_AMOUNT = CREDITED_REQ_AMT,
                                //    CREDIT_OPENING_BALANCE = 0,
                                //    CREDIT_CLOSING_BALANCE = 0,
                                //    CREDIT_USED_DATE = DateTime.Now,
                                //    IPADDRESS = "",
                                //    WLP_ID = (long)Merchant_Info.UNDER_WHITE_LEVEL,
                                //    DIST_ID = (long)Merchant_Info.INTRODUCER,
                                //    SUPER_ID = 0,
                                //    MER_ID = MemberCurrentUser.MEM_ID,
                                //    NARRATION = "CREDIT LIMIT AMOUNT LOGS"
                                //};
                                //db.TBL_MEMBER_CREDIT_ACCOUNT_LOGS.Add(MEM_CREDIT_ACNT_LOGS);
                                await db.SaveChangesAsync();
                                ContextTransaction.Commit();
                                return Json("Transaction Approve.", JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                string msg = "This payment requisition can't be update in full settlement mode. Requisition amount is less than the credit limit amount";
                                return Json(msg, JsonRequestBehavior.AllowGet);
                                //THIS PAYMENT REQUISITION CAN'T BE UPDATE IN FULL SETTLEMENT MODE. REQUISITION AMOUNT IS LESS THAN THE CREDIT LIMIT AMOUNT
                            }
                        }
                        else
                        {
                            decimal CHECK_DIST_VAL = 0.00M;
                            decimal MER_DEBIT_BAL = 0.00M;
                            decimal CREDIT_WALLET_SETTLEMENT_VAL = 0.00M;
                            if (MER_CR_LMT_AMT >= MER_PER_CR_LIT_AMT)
                            {
                                if (Trans_Req_Amount > (MER_CR_LMT_AMT - MER_PER_CR_LIT_AMT))
                                {
                                    MER_DEBIT_BAL = MER_CR_LMT_AMT - MER_PER_CR_LIT_AMT;
                                }
                                else
                                {
                                    MER_DEBIT_BAL = Trans_Req_Amount;
                                }
                                CHECK_DIST_VAL = Trans_Req_Amount - (MER_CR_LMT_AMT - MER_PER_CR_LIT_AMT);
                            }
                            else
                            {
                                if (Trans_Req_Amount > MER_CR_LMT_AMT)

                                {
                                    MER_DEBIT_BAL = MER_CR_LMT_AMT;
                                }
                                else

                                {
                                    MER_DEBIT_BAL = Trans_Req_Amount;
                                }
                                CHECK_DIST_VAL = Trans_Req_Amount - MER_CR_LMT_AMT;
                                CREDIT_WALLET_SETTLEMENT_VAL = MER_DEBIT_BAL;
                            }
                            if (CHECK_DIST_VAL > 0)
                            {
                                if (Dist_MainBal > CHECK_DIST_VAL)
                                {
                                    transinfo.STATUS = "Approve";
                                    transinfo.APPROVAL_DATE = DateTime.Now;
                                    transinfo.APPROVAL_TIME = DateTime.Now;
                                    transinfo.FromUser = "test";
                                    transinfo.REMARKS = SettlementTYPE;
                                    transinfo.APPROVED_BY = "ADMIN TRAVELIQ";
                                    transinfo.PAYMENT_TXN_DETAILS = PaymentTrnId;
                                    db.Entry(transinfo).State = System.Data.Entity.EntityState.Modified;

                                    ////Distributor Main Balanve Update and insert innfor in Account table
                                    //Dist_Sub_MainAmount = Dist_MainBal - CHECK_DIST_VAL;
                                    //Distributor_Info.BALANCE = Dist_Sub_MainAmount;
                                    //db.Entry(Distributor_Info).State = System.Data.Entity.EntityState.Modified;
                                    //if (DIST_CLOSING_AMT > 0)
                                    //{
                                    //    DIST_ADJ_CLOSING_AMT = DIST_CLOSING_AMT - CHECK_DIST_VAL;
                                    //}
                                    //else
                                    //{
                                    //    DIST_ADJ_CLOSING_AMT = 0;
                                    //}
                                    //TBL_ACCOUNTS DIST_objACCOUNT = new TBL_ACCOUNTS()
                                    //{
                                    //    API_ID = 0,
                                    //    MEM_ID = transinfo.FROM_MEMBER,
                                    //    MEMBER_TYPE = "DISTRIBUTOR",
                                    //    TRANSACTION_TYPE = transinfo.PAYMENT_METHOD,
                                    //    TRANSACTION_DATE = Convert.ToDateTime(transinfo.REQUEST_DATE),
                                    //    TRANSACTION_TIME = DateTime.Now,
                                    //    DR_CR = "DR",
                                    //    //AMOUNT = decimal.Parse(transinfo.AMOUNT.ToString()),
                                    //    AMOUNT = CHECK_DIST_VAL,
                                    //    NARRATION = transinfo.TRANSACTION_DETAILS,
                                    //    OPENING = DIST_CLOSING_AMT,
                                    //    CLOSING = DIST_ADJ_CLOSING_AMT,
                                    //    REC_NO = 0,
                                    //    COMM_AMT = 0,
                                    //    TDS = 0,
                                    //    GST = 0,
                                    //    IPAddress = "",
                                    //    SERVICE_ID = 0,
                                    //    CORELATIONID = COrelationID
                                    //};
                                    //db.TBL_ACCOUNTS.Add(DIST_objACCOUNT);
                                    ////END Distributor Main Balanve Update and insert innfor in Account table
                                    Mer_Add_MainBalance = MER_MainBal + Trans_Req_Amount - MER_DEBIT_BAL;
                                    //Merchant_Info.CREDIT_LIMIT = MER_DEBIT_BAL;
                                    decimal SUBCRLIMIAMT = 0;
                                    SUBCRLIMIAMT = MER_CR_LMT_AMT - MER_DEBIT_BAL;
                                    Merchant_Info.CREDIT_LIMIT = SUBCRLIMIAMT;
                                    Merchant_Info.BALANCE = Mer_Add_MainBalance;
                                    db.Entry(Merchant_Info).State = System.Data.Entity.EntityState.Modified;
                                    if (MER_CLOSING_AMT > 0)
                                    {
                                        MER_ADJ_CLOSING_AMT = MER_CLOSING_AMT + Trans_Req_Amount;
                                    }
                                    else
                                    {
                                        MER_ADJ_CLOSING_AMT = Trans_Req_Amount;
                                    }
                                    TBL_ACCOUNTS MERCH_CreeditobjACCOUNT = new TBL_ACCOUNTS()
                                    {
                                        API_ID = 0,
                                        MEM_ID = transinfo.FROM_MEMBER,
                                        MEMBER_TYPE = MemberType,
                                        //TRANSACTION_TYPE = transinfo.PAYMENT_METHOD,
                                        TRANSACTION_TYPE = "DEPOSIT",
                                        TRANSACTION_DATE = Convert.ToDateTime(transinfo.REQUEST_DATE),
                                        TRANSACTION_TIME = DateTime.Now,
                                        DR_CR = "CR",
                                        //AMOUNT = decimal.Parse(transinfo.AMOUNT.ToString()),
                                        AMOUNT = Trans_Req_Amount,
                                        NARRATION = transinfo.TRANSACTION_DETAILS,
                                        OPENING = MER_CLOSING_AMT,
                                        CLOSING = MER_ADJ_CLOSING_AMT,
                                        REC_NO = 0,
                                        COMM_AMT = 0,
                                        TDS = 0,
                                        GST = 0,
                                        IPAddress = "",
                                        SERVICE_ID = 0,
                                        CORELATIONID = COrelationID
                                    };
                                    db.TBL_ACCOUNTS.Add(MERCH_CreeditobjACCOUNT);

                                    decimal MERCH_Deduct_CR_AMT = 0;

                                    MERCH_Deduct_CR_AMT = MER_ADJ_CLOSING_AMT - MER_DEBIT_BAL;
                                    TBL_ACCOUNTS MERCH_objDECUR_AMT = new TBL_ACCOUNTS()
                                    {
                                        API_ID = 0,
                                        MEM_ID = transinfo.FROM_MEMBER,
                                        MEMBER_TYPE = MemberType,
                                        //TRANSACTION_TYPE = transinfo.PAYMENT_METHOD,
                                        TRANSACTION_TYPE = "CREDIT SETTLEMENT",
                                        TRANSACTION_DATE = Convert.ToDateTime(transinfo.REQUEST_DATE),
                                        TRANSACTION_TIME = DateTime.Now,
                                        DR_CR = "DR",
                                        //AMOUNT = decimal.Parse(transinfo.AMOUNT.ToString()),
                                        AMOUNT = MER_DEBIT_BAL,
                                        NARRATION = transinfo.TRANSACTION_DETAILS,
                                        OPENING = MER_ADJ_CLOSING_AMT,
                                        CLOSING = MERCH_Deduct_CR_AMT,
                                        REC_NO = 0,
                                        COMM_AMT = 0,
                                        TDS = 0,
                                        GST = 0,
                                        IPAddress = "",
                                        SERVICE_ID = 0,
                                        CORELATIONID = COrelationID
                                    };
                                    db.TBL_ACCOUNTS.Add(MERCH_objDECUR_AMT);


                                    TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION OBJMERCHANTCREDIT_VAL = new TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION()
                                    {
                                        TO_MEM_ID= transinfo.TO_MEMBER,
                                        FROM_MEM_ID =transinfo.FROM_MEMBER,
                                        CREDIT_DATE=DateTime.Now,
                                        CREDIT_AMOUNT= MER_DEBIT_BAL,
                                        CREDIT_NOTE_DESCRIPTION= "CREDIT LIMIT AMOUNT LOGS",
                                        CREDIT_STATUS=true,
                                        GST_VAL=0,
                                        GST_AMOUNT=0,
                                        TDS_AMOUNT=0,
                                        TDS_VAL=0,
                                        CREDIT_OPENING= MER_CR_LMT_AMT,
                                        CREDITCLOSING= SUBCRLIMIAMT,
                                        CREDIT_TRN_TYPE="DR",
                                        CORELATIONID= COrelationID
                                    };
                                    db.TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION.Add(OBJMERCHANTCREDIT_VAL);
                                

                                    //TBL_MEMBER_CREDIT_ACCOUNT_LOGS MEM_CREDIT_ACNT_LOGS = new TBL_MEMBER_CREDIT_ACCOUNT_LOGS()
                                    //{
                                    //    MEM_ID = MemberCurrentUser.MEM_ID,
                                    //    CREDIT_ID = MemberCurrentUser.MEM_ID,
                                    //    CORELATIONID = COrelationID,
                                    //    CREDIT_TRAN_TYPE = "DR",
                                    //    USED_CREDIT_AMOUNT = MER_DEBIT_BAL,
                                    //    CREDIT_OPENING_BALANCE = MER_CR_LMT_AMT,
                                    //    CREDIT_CLOSING_BALANCE = SUBCRLIMIAMT,
                                    //    CREDIT_USED_DATE = DateTime.Now,
                                    //    IPADDRESS = "",
                                    //    WLP_ID = (long)Merchant_Info.UNDER_WHITE_LEVEL,
                                    //    DIST_ID = (long)Merchant_Info.INTRODUCER,
                                    //    SUPER_ID = 0,
                                    //    MER_ID = MemberCurrentUser.MEM_ID,
                                    //    NARRATION = "CREDIT LIMIT AMOUNT LOGS"
                                    //};
                                    //db.TBL_MEMBER_CREDIT_ACCOUNT_LOGS.Add(MEM_CREDIT_ACNT_LOGS);
                                    await db.SaveChangesAsync();
                                    ContextTransaction.Commit();
                                    return Json("Transaction Approve.", JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    //CHECK_DIST_VAL IS INSUFFICIANT AFTER CALULATION MERCHANT PERTIAL SETTLEMENT AMOUNT OF RS. MER_CR_LMT_AMT
                                    string msg = "Rs. " + MER_DEBIT_BAL + " Credit Requested Amount is Insufficiant After Calculation Merchant Full Settlement Amount of Rs." + MER_CR_LMT_AMT;
                                    return Json(msg, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else if (CHECK_DIST_VAL == 0)
                            {
                                transinfo.STATUS = "Approve";
                                transinfo.APPROVAL_DATE = DateTime.Now;
                                transinfo.APPROVAL_TIME = DateTime.Now;
                                transinfo.FromUser = "test";
                                transinfo.REMARKS = SettlementTYPE;
                                transinfo.APPROVED_BY = "ADMIN TRAVELIQ";
                                transinfo.PAYMENT_TXN_DETAILS = PaymentTrnId;
                                db.Entry(transinfo).State = System.Data.Entity.EntityState.Modified;


                                ////Distributor Main Balanve Update and insert innfor in Account table
                                //Dist_Sub_MainAmount = Dist_MainBal - 0;
                                ////Dist_Sub_MainAmount = Dist_MainBal - CHECK_DIST_VAL;
                                ////Dist_Sub_MainAmount = 0;
                                //Distributor_Info.BALANCE = Dist_Sub_MainAmount;
                                //db.Entry(Distributor_Info).State = System.Data.Entity.EntityState.Modified;
                                //if (DIST_CLOSING_AMT > 0)
                                //{
                                //    DIST_ADJ_CLOSING_AMT = DIST_CLOSING_AMT - CHECK_DIST_VAL;
                                //    //DIST_ADJ_CLOSING_AMT = 0;
                                //}
                                //else
                                //{
                                //    DIST_ADJ_CLOSING_AMT = 0;
                                //}
                                //TBL_ACCOUNTS DIST_objACCOUNT = new TBL_ACCOUNTS()
                                //{
                                //    API_ID = 0,
                                //    MEM_ID = transinfo.FROM_MEMBER,
                                //    MEMBER_TYPE = "DISTRIBUTOR",
                                //    TRANSACTION_TYPE = transinfo.PAYMENT_METHOD,
                                //    TRANSACTION_DATE = Convert.ToDateTime(transinfo.REQUEST_DATE),
                                //    TRANSACTION_TIME = DateTime.Now,
                                //    DR_CR = "DR",
                                //    //AMOUNT = decimal.Parse(transinfo.AMOUNT.ToString()),
                                //    AMOUNT = CHECK_DIST_VAL,
                                //    NARRATION = transinfo.TRANSACTION_DETAILS,
                                //    OPENING = DIST_CLOSING_AMT,
                                //    CLOSING = DIST_ADJ_CLOSING_AMT,
                                //    REC_NO = 0,
                                //    COMM_AMT = 0,
                                //    TDS = 0,
                                //    GST = 0,
                                //    IPAddress = "",
                                //    SERVICE_ID = 0,
                                //    CORELATIONID = COrelationID
                                //};
                                //db.TBL_ACCOUNTS.Add(DIST_objACCOUNT);
                                //END Distributor Main Balanve Update and insert innfor in Account table
                                //Mer_Add_MainBalance = MER_MainBal + Trans_Req_Amount - CREDITED_REQ_AMT;
                                Mer_Add_MainBalance = MER_MainBal + Trans_Req_Amount - MER_DEBIT_BAL;
                                decimal SUBCRLIMIAMT = 0;
                                SUBCRLIMIAMT = MER_CR_LMT_AMT - MER_DEBIT_BAL;

                                Merchant_Info.CREDIT_LIMIT = SUBCRLIMIAMT;
                                //Merchant_Info.CREDIT_LIMIT = MER_DEBIT_BAL;
                                Merchant_Info.BALANCE = Mer_Add_MainBalance;
                                db.Entry(Merchant_Info).State = System.Data.Entity.EntityState.Modified;
                                if (MER_CLOSING_AMT > 0)
                                {
                                    MER_ADJ_CLOSING_AMT = MER_CLOSING_AMT + Trans_Req_Amount;
                                }
                                else
                                {
                                    MER_ADJ_CLOSING_AMT = Trans_Req_Amount;
                                }
                                TBL_ACCOUNTS MERCH_CreeditobjACCOUNT = new TBL_ACCOUNTS()
                                {
                                    API_ID = 0,
                                    MEM_ID = transinfo.FROM_MEMBER,
                                    MEMBER_TYPE = MemberType,
                                    //TRANSACTION_TYPE = transinfo.PAYMENT_METHOD,
                                    TRANSACTION_TYPE = "DEPOSIT",
                                    TRANSACTION_DATE = Convert.ToDateTime(transinfo.REQUEST_DATE),
                                    TRANSACTION_TIME = DateTime.Now,
                                    DR_CR = "CR",
                                    //AMOUNT = decimal.Parse(transinfo.AMOUNT.ToString()),
                                    AMOUNT = Trans_Req_Amount,
                                    NARRATION = transinfo.TRANSACTION_DETAILS,
                                    OPENING = MER_CLOSING_AMT,
                                    CLOSING = MER_ADJ_CLOSING_AMT,
                                    REC_NO = 0,
                                    COMM_AMT = 0,
                                    TDS = 0,
                                    GST = 0,
                                    IPAddress = "",
                                    SERVICE_ID = 0,
                                    CORELATIONID = COrelationID
                                };
                                db.TBL_ACCOUNTS.Add(MERCH_CreeditobjACCOUNT);

                                decimal MERCH_Deduct_CR_AMT = 0;

                                MERCH_Deduct_CR_AMT = MER_ADJ_CLOSING_AMT - MER_DEBIT_BAL;
                                TBL_ACCOUNTS MERCH_objDECUR_AMT = new TBL_ACCOUNTS()
                                {
                                    API_ID = 0,
                                    MEM_ID = transinfo.FROM_MEMBER,
                                    MEMBER_TYPE = MemberType,
                                    //TRANSACTION_TYPE = transinfo.PAYMENT_METHOD,
                                    TRANSACTION_TYPE = "CREDIT SETTLEMENT",
                                    TRANSACTION_DATE = Convert.ToDateTime(transinfo.REQUEST_DATE),
                                    TRANSACTION_TIME = DateTime.Now,
                                    DR_CR = "DR",
                                    //AMOUNT = decimal.Parse(transinfo.AMOUNT.ToString()),
                                    AMOUNT = MER_DEBIT_BAL,
                                    NARRATION = transinfo.TRANSACTION_DETAILS,
                                    OPENING = MER_ADJ_CLOSING_AMT,
                                    CLOSING = MERCH_Deduct_CR_AMT,
                                    REC_NO = 0,
                                    COMM_AMT = 0,
                                    TDS = 0,
                                    GST = 0,
                                    IPAddress = "",
                                    SERVICE_ID = 0,
                                    CORELATIONID = COrelationID
                                };
                                db.TBL_ACCOUNTS.Add(MERCH_objDECUR_AMT);

                                TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION OBJMERCHANTCREDIT_VAL = new TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION()
                                {
                                    TO_MEM_ID = transinfo.TO_MEMBER,
                                    FROM_MEM_ID = transinfo.FROM_MEMBER,
                                    CREDIT_DATE = DateTime.Now,
                                    CREDIT_AMOUNT = MER_DEBIT_BAL,
                                    CREDIT_NOTE_DESCRIPTION = "CREDIT LIMIT AMOUNT LOGS",
                                    CREDIT_STATUS = true,
                                    GST_VAL = 0,
                                    GST_AMOUNT = 0,
                                    TDS_AMOUNT = 0,
                                    TDS_VAL = 0,
                                    CREDIT_OPENING = MER_CR_LMT_AMT,
                                    CREDITCLOSING = SUBCRLIMIAMT,
                                    CREDIT_TRN_TYPE = "DR",
                                    CORELATIONID = COrelationID
                                };
                                db.TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION.Add(OBJMERCHANTCREDIT_VAL);

                                //TBL_MEMBER_CREDIT_ACCOUNT_LOGS MEM_CREDIT_ACNT_LOGS = new TBL_MEMBER_CREDIT_ACCOUNT_LOGS()
                                //{
                                //    MEM_ID = MemberCurrentUser.MEM_ID,
                                //    CREDIT_ID = MemberCurrentUser.MEM_ID,
                                //    CORELATIONID = COrelationID,
                                //    CREDIT_TRAN_TYPE = "DR",
                                //    USED_CREDIT_AMOUNT = MER_DEBIT_BAL,
                                //    CREDIT_OPENING_BALANCE = MER_CR_LMT_AMT,
                                //    CREDIT_CLOSING_BALANCE = SUBCRLIMIAMT,
                                //    CREDIT_USED_DATE = DateTime.Now,
                                //    IPADDRESS = "",
                                //    WLP_ID = (long)Merchant_Info.UNDER_WHITE_LEVEL,
                                //    DIST_ID = (long)Merchant_Info.INTRODUCER,
                                //    SUPER_ID = 0,
                                //    MER_ID = MemberCurrentUser.MEM_ID,
                                //    NARRATION = "CREDIT LIMIT AMOUNT LOGS"
                                //};
                                //db.TBL_MEMBER_CREDIT_ACCOUNT_LOGS.Add(MEM_CREDIT_ACNT_LOGS);
                                await db.SaveChangesAsync();
                                ContextTransaction.Commit();
                                return Json("Transaction Approve.", JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                transinfo.STATUS = "Approve";
                                transinfo.APPROVAL_DATE = DateTime.Now;
                                transinfo.APPROVAL_TIME = DateTime.Now;
                                transinfo.FromUser = "test";
                                transinfo.REMARKS = SettlementTYPE;
                                transinfo.APPROVED_BY = "ADMIN TRAVELIQ";
                                transinfo.PAYMENT_TXN_DETAILS = PaymentTrnId;
                                db.Entry(transinfo).State = System.Data.Entity.EntityState.Modified;


                                ////Distributor Main Balanve Update and insert innfor in Account table
                                //Dist_Sub_MainAmount = Dist_MainBal - 0;
                                ////Dist_Sub_MainAmount = Dist_MainBal - CHECK_DIST_VAL;
                                ////Dist_Sub_MainAmount = 0;
                                //Distributor_Info.BALANCE = Dist_Sub_MainAmount;
                                //db.Entry(Distributor_Info).State = System.Data.Entity.EntityState.Modified;
                                //if (DIST_CLOSING_AMT > 0)
                                //{
                                //    DIST_ADJ_CLOSING_AMT = DIST_CLOSING_AMT - CHECK_DIST_VAL;
                                //    //DIST_ADJ_CLOSING_AMT = 0;
                                //}
                                //else
                                //{
                                //    DIST_ADJ_CLOSING_AMT = 0;
                                //}
                                //TBL_ACCOUNTS DIST_objACCOUNT = new TBL_ACCOUNTS()
                                //{
                                //    API_ID = 0,
                                //    MEM_ID = transinfo.FROM_MEMBER,
                                //    MEMBER_TYPE = "DISTRIBUTOR",
                                //    TRANSACTION_TYPE = transinfo.PAYMENT_METHOD,
                                //    TRANSACTION_DATE = Convert.ToDateTime(transinfo.REQUEST_DATE),
                                //    TRANSACTION_TIME = DateTime.Now,
                                //    DR_CR = "DR",
                                //    //AMOUNT = decimal.Parse(transinfo.AMOUNT.ToString()),
                                //    AMOUNT = CHECK_DIST_VAL,
                                //    NARRATION = transinfo.TRANSACTION_DETAILS,
                                //    OPENING = DIST_CLOSING_AMT,
                                //    CLOSING = DIST_ADJ_CLOSING_AMT,
                                //    REC_NO = 0,
                                //    COMM_AMT = 0,
                                //    TDS = 0,
                                //    GST = 0,
                                //    IPAddress = "",
                                //    SERVICE_ID = 0,
                                //    CORELATIONID = COrelationID
                                //};
                                //db.TBL_ACCOUNTS.Add(DIST_objACCOUNT);
                                //END Distributor Main Balanve Update and insert innfor in Account table
                                //Mer_Add_MainBalance = MER_MainBal + Trans_Req_Amount - CREDITED_REQ_AMT;
                                decimal SUB_CREDItAMT = 0;
                                SUB_CREDItAMT = MER_CR_LMT_AMT - MER_DEBIT_BAL;

                                Mer_Add_MainBalance = MER_MainBal + Trans_Req_Amount - MER_DEBIT_BAL;
                                //Merchant_Info.CREDIT_LIMIT = MER_DEBIT_BAL;
                                Merchant_Info.CREDIT_LIMIT = SUB_CREDItAMT;
                                Merchant_Info.BALANCE = Mer_Add_MainBalance;
                                db.Entry(Merchant_Info).State = System.Data.Entity.EntityState.Modified;
                                if (MER_CLOSING_AMT > 0)
                                {
                                    MER_ADJ_CLOSING_AMT = MER_CLOSING_AMT + Trans_Req_Amount;
                                }
                                else
                                {
                                    MER_ADJ_CLOSING_AMT = Trans_Req_Amount;
                                }
                                TBL_ACCOUNTS MERCH_CreeditobjACCOUNT = new TBL_ACCOUNTS()
                                {
                                    API_ID = 0,
                                    MEM_ID = transinfo.FROM_MEMBER,
                                    MEMBER_TYPE = MemberType,
                                    //TRANSACTION_TYPE = transinfo.PAYMENT_METHOD,
                                    TRANSACTION_TYPE = "DEPOSIT",
                                    TRANSACTION_DATE = Convert.ToDateTime(transinfo.REQUEST_DATE),
                                    TRANSACTION_TIME = DateTime.Now,
                                    DR_CR = "CR",
                                    //AMOUNT = decimal.Parse(transinfo.AMOUNT.ToString()),
                                    AMOUNT = Trans_Req_Amount,
                                    NARRATION = transinfo.TRANSACTION_DETAILS,
                                    OPENING = MER_CLOSING_AMT,
                                    CLOSING = MER_ADJ_CLOSING_AMT,
                                    REC_NO = 0,
                                    COMM_AMT = 0,
                                    TDS = 0,
                                    GST = 0,
                                    IPAddress = "",
                                    SERVICE_ID = 0,
                                    CORELATIONID = COrelationID
                                };
                                db.TBL_ACCOUNTS.Add(MERCH_CreeditobjACCOUNT);

                                decimal MERCH_Deduct_CR_AMT = 0;

                                MERCH_Deduct_CR_AMT = MER_ADJ_CLOSING_AMT - MER_DEBIT_BAL;
                                TBL_ACCOUNTS MERCH_objDECUR_AMT = new TBL_ACCOUNTS()
                                {
                                    API_ID = 0,
                                    MEM_ID = transinfo.FROM_MEMBER,
                                    MEMBER_TYPE = MemberType,
                                    //TRANSACTION_TYPE = transinfo.PAYMENT_METHOD,
                                    TRANSACTION_TYPE = "CREDIT SETTLEMENT",
                                    TRANSACTION_DATE = Convert.ToDateTime(transinfo.REQUEST_DATE),
                                    TRANSACTION_TIME = DateTime.Now,
                                    DR_CR = "DR",
                                    //AMOUNT = decimal.Parse(transinfo.AMOUNT.ToString()),
                                    AMOUNT = MER_DEBIT_BAL,
                                    NARRATION = transinfo.TRANSACTION_DETAILS,
                                    OPENING = MER_ADJ_CLOSING_AMT,
                                    CLOSING = MERCH_Deduct_CR_AMT,
                                    REC_NO = 0,
                                    COMM_AMT = 0,
                                    TDS = 0,
                                    GST = 0,
                                    IPAddress = "",
                                    SERVICE_ID = 0,
                                    CORELATIONID = COrelationID
                                };
                                db.TBL_ACCOUNTS.Add(MERCH_objDECUR_AMT);
                                TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION OBJMERCHANTCREDIT_VAL = new TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION()
                                {
                                    TO_MEM_ID = transinfo.TO_MEMBER,
                                    FROM_MEM_ID = transinfo.FROM_MEMBER,
                                    CREDIT_DATE = DateTime.Now,
                                    CREDIT_AMOUNT = MER_DEBIT_BAL,
                                    CREDIT_NOTE_DESCRIPTION = "CREDIT LIMIT AMOUNT LOGS",
                                    CREDIT_STATUS = true,
                                    GST_VAL = 0,
                                    GST_AMOUNT = 0,
                                    TDS_AMOUNT = 0,
                                    TDS_VAL = 0,
                                    CREDIT_OPENING = MER_CR_LMT_AMT,
                                    CREDITCLOSING = SUB_CREDItAMT,
                                    CREDIT_TRN_TYPE = "DR",
                                    CORELATIONID = COrelationID
                                };
                                db.TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION.Add(OBJMERCHANTCREDIT_VAL);

                                //TBL_MEMBER_CREDIT_ACCOUNT_LOGS MEM_CREDIT_ACNT_LOGS = new TBL_MEMBER_CREDIT_ACCOUNT_LOGS()
                                //{
                                //    MEM_ID = MemberCurrentUser.MEM_ID,
                                //    CREDIT_ID = MemberCurrentUser.MEM_ID,
                                //    CORELATIONID = COrelationID,
                                //    CREDIT_TRAN_TYPE = "DR",
                                //    USED_CREDIT_AMOUNT = MER_DEBIT_BAL,
                                //    CREDIT_OPENING_BALANCE = MER_CR_LMT_AMT,
                                //    CREDIT_CLOSING_BALANCE = SUB_CREDItAMT,
                                //    CREDIT_USED_DATE = DateTime.Now,
                                //    IPADDRESS = "",
                                //    WLP_ID = (long)Merchant_Info.UNDER_WHITE_LEVEL,
                                //    DIST_ID = (long)Merchant_Info.INTRODUCER,
                                //    SUPER_ID = 0,
                                //    MER_ID = MemberCurrentUser.MEM_ID,
                                //    NARRATION = "CREDIT LIMIT AMOUNT LOGS"
                                //};
                                //db.TBL_MEMBER_CREDIT_ACCOUNT_LOGS.Add(MEM_CREDIT_ACNT_LOGS);
                                await db.SaveChangesAsync();
                                ContextTransaction.Commit();
                                return Json("Transaction Approve.", JsonRequestBehavior.AllowGet);
                            }

                        }
                    }
                    #endregion




                    //long sln = long.Parse(slnval);
                    //decimal addamount = 0;
                    //decimal memb_bal = 0;
                    //decimal trans_bal = 0;
                    //decimal availableamt = 0;
                    //decimal addavilamt = 0;
                    //decimal closingamt = 0;
                    //decimal addclosingamt = 0;
                    //string COrelationID = Settings.GetUniqueKey(MemberCurrentUser.MEM_ID.ToString());

                    //var transinfo = await db.TBL_BALANCE_TRANSFER_LOGS.Where(x => x.SLN == sln).FirstOrDefaultAsync();

                    //var checkAvailAmtWL = await (from valamt in db.TBL_MASTER_MEMBER                                                
                    //                             where valamt.MEM_ID == MemberCurrentUser.MEM_ID
                    //                             //where valamt.MEM_ID == transinfo.FROM_MEMBER
                    //                             //join mainbal in db.TBL_ACCOUNTS on valamt.INTRODUCER equals mainbal.MEM_ID
                    //                             //where valamt.MEM_ID == transinfo.FROM_MEMBER
                    //                             //select valamt).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefaultAsync();
                    //                             select valamt).FirstOrDefaultAsync();
                    ////var checkAvailAmtWL = await (from valamt in db.TBL_MASTER_MEMBER
                    ////                             join mainbal in db.TBL_ACCOUNTS on valamt.MEM_ID equals mainbal.MEM_ID
                    ////                             where valamt.MEM_ID == transinfo.TO_MEMBER
                    ////                             //where valamt.MEM_ID == transinfo.FROM_MEMBER
                    ////                             //join mainbal in db.TBL_ACCOUNTS on valamt.INTRODUCER equals mainbal.MEM_ID
                    ////                             //where valamt.MEM_ID == transinfo.FROM_MEMBER
                    ////                             //select valamt).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefaultAsync();
                    ////                             select valamt).FirstOrDefaultAsync();
                    //decimal WlBal = 0;
                    //decimal.TryParse(checkAvailAmtWL.BALANCE.ToString(), out WlBal);

                    //if (Convert.ToDecimal(transinfo.AMOUNT) <= Convert.ToDecimal(checkAvailAmtWL.BALANCE))
                    //{

                    //    var membtype = await (from mm in db.TBL_MASTER_MEMBER
                    //                    join
                    //                    rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                    //                    where mm.MEM_ID == transinfo.FROM_MEMBER
                    //                    select new
                    //                    {
                    //                        RoleId = mm.MEMBER_ROLE,
                    //                        roleName = rm.ROLE_NAME,
                    //                        Amount = mm.BALANCE
                    //                    }).FirstOrDefaultAsync();
                    //    //transinfo.REQUEST_DATE = Convert.ToDateTime(trandate);
                    //    transinfo.STATUS = "Approve";
                    //    transinfo.FromUser = "test";
                    //    transinfo.APPROVAL_DATE = DateTime.Now;
                    //    transinfo.APPROVAL_TIME = DateTime.Now;
                    //    transinfo.APPROVED_BY = "White Label";
                    //    db.Entry(transinfo).State = System.Data.Entity.EntityState.Modified;
                    //    await db.SaveChangesAsync();

                    //    decimal Mer_bal = 0;
                    //    decimal Add_mer_al = 0;
                    //    decimal GetCR_BalAmt = 0;
                    //    decimal GetRES_CRE_BAl = 0;
                    //    decimal REQAmt = 0;
                    //    decimal DeductResev_LimitAmtToTrasamt = 0;
                    //    decimal.TryParse(transinfo.AMOUNT.ToString(), out REQAmt);
                    //    decimal DedcuctCreditLimitAmt = 0;
                    //    decimal SubFullCreditAmt = 0;
                    //    decimal DeductedCreAmt = 0;
                    //    var memberlist = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == transinfo.FROM_MEMBER).FirstOrDefaultAsync();
                    //    decimal.TryParse(memberlist.BALANCE.ToString(), out Mer_bal);
                    //    decimal.TryParse(memberlist.CREDIT_LIMIT.ToString(), out GetCR_BalAmt);
                    //    decimal.TryParse(memberlist.RESERVED_CREDIT_LIMIT.ToString(), out GetRES_CRE_BAl);
                    //    Add_mer_al = Mer_bal + decimal.Parse(transinfo.AMOUNT.ToString());

                    //    if (SettlementTYPE == "Partial")
                    //    {
                    //        DeductResev_LimitAmtToTrasamt = GetCR_BalAmt - GetRES_CRE_BAl;
                    //        DeductedCreAmt = GetCR_BalAmt - DeductResev_LimitAmtToTrasamt;
                    //        DedcuctCreditLimitAmt = Add_mer_al - DeductResev_LimitAmtToTrasamt;
                    //        memberlist.CREDIT_LIMIT = DeductedCreAmt;
                    //        memberlist.BALANCE = DedcuctCreditLimitAmt;
                    //    }
                    //    else
                    //    {
                    //        DeductResev_LimitAmtToTrasamt = REQAmt - GetCR_BalAmt;
                    //        //SubFullCreditAmt = REQAmt - GetCR_BalAmt;
                    //        //DedcuctCreditLimitAmt = Add_mer_al - DeductResev_LimitAmtToTrasamt;
                    //        DedcuctCreditLimitAmt = Add_mer_al - GetCR_BalAmt;
                    //        memberlist.CREDIT_LIMIT = 0;
                    //        memberlist.RESERVED_CREDIT_LIMIT = 0;
                    //        memberlist.BALANCE = DedcuctCreditLimitAmt;
                    //    }
                    //    db.Entry(memberlist).State = System.Data.Entity.EntityState.Modified;
                    //    await db.SaveChangesAsync();

                    //    //var frommem = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == transinfo.FROM_MEMBER);
                    //    ////memb_bal = decimal.Parse(memberlist.BALANCE.ToString());
                    //    //decimal.TryParse(frommem.BALANCE.ToString(), out memb_bal);
                    //    //trans_bal = decimal.Parse(transinfo.AMOUNT.ToString());
                    //    //addamount = memb_bal + trans_bal;

                    //    //db.Entry(memberlist).State = System.Data.Entity.EntityState.Modified;
                    //    //db.SaveChanges();

                    //    decimal Opening_amt = 0;
                    //    decimal Closing_amt = 0;
                    //    decimal Trans_amt = 0;
                    //    decimal Amount = 0;
                    //    decimal AddAmount = 0;
                    //    decimal AddOpening_amt = 0;
                    //    decimal AddClosing_amt = 0;
                    //    var amtobj = await db.TBL_ACCOUNTS.Where(x => x.MEM_ID == transinfo.FROM_MEMBER).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefaultAsync();
                    //    if (amtobj != null)
                    //    {
                    //        Opening_amt = amtobj.OPENING;
                    //        closingamt = amtobj.CLOSING;
                    //        Amount = amtobj.AMOUNT;
                    //        AddAmount = decimal.Parse(transinfo.AMOUNT.ToString()) + closingamt;

                    //    }
                    //    else
                    //    {
                    //        AddAmount = decimal.Parse(transinfo.AMOUNT.ToString())+ decimal.Parse(membtype.Amount.ToString());
                    //    }
                    //    TBL_ACCOUNTS objacnt = new TBL_ACCOUNTS()
                    //    {
                    //        API_ID = 0,
                    //        MEM_ID = transinfo.FROM_MEMBER,
                    //        MEMBER_TYPE = membtype.roleName,
                    //        TRANSACTION_TYPE = transinfo.PAYMENT_METHOD,
                    //        TRANSACTION_DATE = Convert.ToDateTime(transinfo.REQUEST_DATE),
                    //        TRANSACTION_TIME = DateTime.Now,
                    //        DR_CR = "CR",
                    //        AMOUNT = decimal.Parse(transinfo.AMOUNT.ToString()),
                    //        NARRATION = transinfo.TRANSACTION_DETAILS,
                    //        OPENING = closingamt,
                    //        CLOSING = AddAmount,
                    //        REC_NO = 0,
                    //        COMM_AMT = 0,
                    //        GST=0,
                    //        TDS=0,
                    //        IPAddress="",
                    //        SERVICE_ID = 0,
                    //        CORELATIONID= COrelationID
                    //    };
                    //    db.TBL_ACCOUNTS.Add(objacnt);
                    //    TBL_ACCOUNTS MERCHANTCREDITAMTSLT = new TBL_ACCOUNTS()
                    //    {
                    //        API_ID = 0,
                    //        MEM_ID = transinfo.FROM_MEMBER,
                    //        MEMBER_TYPE = membtype.roleName,
                    //        TRANSACTION_TYPE = SettlementTYPE + "SETTLEMENT",
                    //        TRANSACTION_DATE = Convert.ToDateTime(transinfo.REQUEST_DATE),
                    //        TRANSACTION_TIME = DateTime.Now,
                    //        DR_CR = "CR",
                    //        AMOUNT = DeductResev_LimitAmtToTrasamt,
                    //        NARRATION = "CREDIT LIMIT SETTLEMENT",
                    //        OPENING = AddAmount,
                    //        CLOSING = DedcuctCreditLimitAmt,
                    //        REC_NO = 0,
                    //        COMM_AMT = 0,
                    //        TDS = 0,
                    //        GST = 0,
                    //        IPAddress = "",
                    //        SERVICE_ID = 0,
                    //        CORELATIONID = COrelationID
                    //    };
                    //    db.TBL_ACCOUNTS.Add(MERCHANTCREDITAMTSLT);
                    //    await db.SaveChangesAsync();
                    //    if (transinfo.BANK_CHARGES > 0)
                    //    {
                    //        decimal SubAmt_Val = AddAmount - Convert.ToDecimal(transinfo.BANK_CHARGES);
                    //        TBL_ACCOUNTS objcomval = new TBL_ACCOUNTS()
                    //        {
                    //            API_ID = 0,
                    //            MEM_ID = transinfo.FROM_MEMBER,
                    //            MEMBER_TYPE = membtype.roleName,
                    //            TRANSACTION_TYPE = transinfo.PAYMENT_METHOD,
                    //            TRANSACTION_DATE = Convert.ToDateTime(transinfo.REQUEST_DATE),
                    //            TRANSACTION_TIME = DateTime.Now,
                    //            DR_CR = "DR",
                    //            AMOUNT = decimal.Parse(transinfo.BANK_CHARGES.ToString()),
                    //            NARRATION = transinfo.TRANSACTION_DETAILS,
                    //            OPENING = objacnt.CLOSING,
                    //            CLOSING = SubAmt_Val,
                    //            REC_NO = 0,
                    //            COMM_AMT = 0,
                    //            GST=0,
                    //            TDS=0,
                    //            IPAddress="",
                    //            SERVICE_ID = 0,
                    //            CORELATIONID = COrelationID
                    //        };
                    //        db.TBL_ACCOUNTS.Add(objcomval);
                    //        await    db.SaveChangesAsync();          
                    //        var memberlistval = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == transinfo.FROM_MEMBER).FirstOrDefaultAsync();
                    //        memberlistval.BALANCE = SubAmt_Val;
                    //        db.Entry(memberlistval).State = System.Data.Entity.EntityState.Modified;
                    //        await db.SaveChangesAsync();
                    //    }
                    //    #region For White level
                    //    decimal AmtSub = 0;
                    //    decimal AmtCloseSub = 0;

                    //    AmtSub = decimal.Parse(checkAvailAmtWL.BALANCE.ToString());
                    //    AmtCloseSub = AmtSub - decimal.Parse(transinfo.AMOUNT.ToString()) - decimal.Parse(transinfo.BANK_CHARGES.ToString());

                    //    //TBL_ACCOUNTS objWLacnt = new TBL_ACCOUNTS()
                    //    //{
                    //    //    API_ID = 0,
                    //    //    MEM_ID = checkAvailAmtWL.MEM_ID,
                    //    //    MEMBER_TYPE = checkAvailAmtWL.MEMBER_TYPE,
                    //    //    TRANSACTION_TYPE = transinfo.PAYMENT_METHOD,
                    //    //    TRANSACTION_DATE = Convert.ToDateTime(transinfo.REQUEST_DATE),
                    //    //    TRANSACTION_TIME = DateTime.Now,
                    //    //    DR_CR = "DR",
                    //    //    AMOUNT = decimal.Parse(transinfo.AMOUNT.ToString()),
                    //    //    NARRATION = transinfo.TRANSACTION_DETAILS,
                    //    //    OPENING = AmtSub,
                    //    //    CLOSING = AmtCloseSub,
                    //    //    REC_NO = 0,
                    //    //    COMM_AMT = 0,
                    //    //    GST = 0,
                    //    //    TDS = 0,
                    //    //    IPAddress = "",
                    //    //    SERVICE_ID = 0,
                    //    //};
                    //    //db.TBL_ACCOUNTS.Add(objWLacnt);
                    //    //await db.SaveChangesAsync();
                    //    var updatewhitelevelAmt = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == checkAvailAmtWL.MEM_ID).FirstOrDefaultAsync();
                    //    //updatewhitelevelAmt.BALANCE = AmtCloseSub;
                    //    //db.Entry(updatewhitelevelAmt).State = System.Data.Entity.EntityState.Modified;
                    //    //await db.SaveChangesAsync();
                    //    #endregion

                    //    //var memberlist = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == transinfo.FROM_MEMBER).FirstOrDefaultAsync();
                    //    //memberlist.BALANCE = AddAmount;
                    //    //db.Entry(memberlist).State = System.Data.Entity.EntityState.Modified;
                    //    //await db.SaveChangesAsync();
                    //    Session["closingAmt"] = AmtCloseSub;
                    //    //#region For Whitelevel

                    //    //var WlAccount = await db.TBL_ACCOUNTS.Where(x => x.MEM_ID ==MemberCurrentUser.MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefaultAsync();
                    //    //decimal WLOpeningAmt = 0;
                    //    //decimal WLClosingAmt = 0;
                    //    //decimal WLTrnAmt = 0;
                    //    //if (WlAccount != null)
                    //    //{
                    //    //    WLOpeningAmt = WlAccount.OPENING;
                    //    //    WLClosingAmt = WlAccount.CLOSING;
                    //    //    WLTrnAmt=WLClosingAmt
                    //    //}
                    //    //#endregion
                    //    EmailHelper emailhelper = new EmailHelper();
                    //    string mailbody = "Hi " + memberlist.UName + ",<p>Your requisition approved by "+ updatewhitelevelAmt.UName + " .</p>";
                    //    emailhelper.SendUserEmail(memberlist.EMAIL_ID, "Requisition Approve", mailbody);

                    //    ContextTransaction.Commit();
                    //    return Json(new { Result = "true" });
                    //}
                    //else
                    //{
                    //    return Json(new { Result = "Pending" });
                    //}
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    Logger.Error("Controller:-  MemberRequisition(Admin), method:- ChangeTransactionStatus (POST) Line No:- 599", ex);
                    return Json(new { Result = "false" });
                }
            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<JsonResult> TransactionDecline(string trandate = "", string TransationStatus = "", string slnval = "")
        public async Task<JsonResult> TransactionDecline(string slnval = "",string PaymentTrnDetails = "")
        {
            initpage();////
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    //var db = new DBContext();
                    long sln = long.Parse(slnval);
                    var transinfo = await db.TBL_BALANCE_TRANSFER_LOGS.Where(x => x.SLN == sln).FirstOrDefaultAsync();
                    //transinfo.REQUEST_DATE = Convert.ToDateTime(trandate);
                    transinfo.STATUS = "Decline";
                    transinfo.FromUser = "test";
                    transinfo.APPROVAL_DATE = DateTime.Now;
                    transinfo.APPROVAL_TIME = DateTime.Now;
                    transinfo.TRANSACTION_DETAILS = PaymentTrnDetails;
                    transinfo.APPROVED_BY = "White Label";
                    db.Entry(transinfo).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();
                    EmailHelper emailhelper = new EmailHelper();
                    var memberlist = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == transinfo.FROM_MEMBER).FirstOrDefaultAsync();
                    string mailbody = "Hi " + memberlist.UName + ",<p>Your requisition has been declined.</p>";
                    emailhelper.SendUserEmail(memberlist.EMAIL_ID, "Requisition Decline", mailbody);
                    ContextTransaction.Commit();
                    return Json(new { Result = "true" });
                }
                catch (Exception ex)
                {
                    Logger.Error("Controller:-  MemberRequisition(Admin), method:- TransactionDecline (POST) Line No:- 631", ex);
                    ContextTransaction.Rollback();
                    return Json(new { Result = "false " });
                }
            }


        }


        public ActionResult DeclinedRequisition()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                initpage();
                return View();
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
        public PartialViewResult DeclinedIndexGrid(string DateFrom="",string Date_To="")
        {
            try
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
                                           where x.STATUS == "Decline" && x.TO_MEMBER == MemberCurrentUser.MEM_ID && x.APPROVAL_DATE>= Date_From_Val && x.APPROVAL_DATE<= Date_To_Val
                                           select new
                                           {
                                               Touser = "White Label",
                                               TransId = x.TransactionID,
                                               FromUser = y.UName,
                                               Reference_NO = x.REFERENCE_NO,
                                               REQUEST_DATE = x.REQUEST_DATE,
                                               AMOUNT = x.AMOUNT,
                                               BANK_ACCOUNT = x.BANK_ACCOUNT,
                                               TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                               SLN = x.SLN,
                                               PAY_MODE = x.PAYMENT_METHOD
                                           }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                           {
                                               Serial_No = index + 1,
                                               ToUser = z.Touser,
                                               TransactionID = z.TransId,
                                               REFERENCE_NO = z.Reference_NO,
                                               FromUser = z.FromUser,
                                               AMOUNT = z.AMOUNT,
                                               REQUEST_DATE = z.REQUEST_DATE,
                                               BANK_ACCOUNT = z.BANK_ACCOUNT,
                                               TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                               SLN = z.SLN,
                                               PAYMENT_METHOD = z.PAY_MODE
                                           }).ToList();
                    return PartialView("DeclinedIndexGrid", transactionlist);
                }
                else
                {
                    DateTime Todaydate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                    var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                           join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                           where x.STATUS == "Decline" && x.TO_MEMBER == MemberCurrentUser.MEM_ID && x.REQUEST_DATE == Todaydate
                                           select new
                                           {
                                               Touser = "White Label",
                                               TransId = x.TransactionID,
                                               FromUser = y.UName,
                                               Reference_NO = x.REFERENCE_NO,
                                               REQUEST_DATE = x.REQUEST_DATE,
                                               AMOUNT = x.AMOUNT,
                                               BANK_ACCOUNT = x.BANK_ACCOUNT,
                                               TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                               SLN = x.SLN,
                                               PAY_MODE = x.PAYMENT_METHOD
                                           }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                           {
                                               Serial_No = index + 1,
                                               ToUser = z.Touser,
                                               TransactionID = z.TransId,
                                               REFERENCE_NO = z.Reference_NO,
                                               FromUser = z.FromUser,
                                               AMOUNT = z.AMOUNT,
                                               REQUEST_DATE = z.REQUEST_DATE,
                                               BANK_ACCOUNT = z.BANK_ACCOUNT,
                                               TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                               SLN = z.SLN,
                                               PAYMENT_METHOD = z.PAY_MODE
                                           }).ToList();
                    return PartialView("DeclinedIndexGrid", transactionlist);
                }
                   
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MemberRequisition(Admin), method:- IndexGrid (GET) Line No:- 114", ex);
                throw ex;
            }

        }


        [ValidateAntiForgeryToken]
        //public async Task<JsonResult> TransactionDecline(string trandate = "", string TransationStatus = "", string slnval = "")
        public async Task<JsonResult> ActivateRequisition(string slnval = "")
        {
            initpage();////
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    //var db = new DBContext();
                    long sln = long.Parse(slnval);
                    var transinfo = await db.TBL_BALANCE_TRANSFER_LOGS.Where(x => x.SLN == sln).FirstOrDefaultAsync();
                    //transinfo.REQUEST_DATE = Convert.ToDateTime(trandate);
                    transinfo.STATUS = "Pending";
                    transinfo.FromUser = "test";
                    transinfo.APPROVAL_DATE = DateTime.Now;
                    transinfo.APPROVAL_TIME = DateTime.Now;
                    transinfo.APPROVED_BY = "Power Admin";
                    db.Entry(transinfo).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();
                    var memberlist = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == transinfo.FROM_MEMBER).FirstOrDefaultAsync();
                    EmailHelper emailhelper = new EmailHelper();
                    string mailbody = "Hi " + memberlist.UName + ",<p>Your requisition has been activated.</p>";
                    emailhelper.SendUserEmail(memberlist.EMAIL_ID, "Requisition Activated", mailbody);
                    ContextTransaction.Commit();
                    return Json(new { Result = "true" });
                }
                catch (Exception ex)
                {
                    Logger.Error("Controller:-  MemberRequisition(Admin), method:- TransactionDecline (POST) Line No:- 631", ex);
                    ContextTransaction.Rollback();
                    return Json(new { Result = "false " });
                }
            }


        }



        public ActionResult RequisitionReport()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                initpage();
                return View();
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
        public JsonResult DateWiseSearchRequisition(string Datefrom = "", string dateto = "")
        {
            try
            {
                var db = new DBContext();
                DateTime datefrom = DateTime.Parse(Datefrom, System.Globalization.CultureInfo.GetCultureInfo("en-Us").DateTimeFormat);
                DateTime DateTO = DateTime.Parse(dateto, System.Globalization.CultureInfo.GetCultureInfo("en-Us").DateTimeFormat);
                var memberinfo = db.TBL_BALANCE_TRANSFER_LOGS.Where(x => x.REQUEST_DATE >= datefrom && x.REQUEST_DATE <= DateTO).ToList();
                return Json(new { Result = "true", infor = memberinfo });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "false" });
            }

        }
        public string FormatDate(string date, string format)
        {
            try
            {
                return DateTime.Parse(date).ToString(format);
            }
            catch (FormatException)
            {
                return string.Empty;
            }
        }

        public PartialViewResult RequisitionGrid()
        {
            try
            {
                //var db = new DBContext();              
                //    var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                //                           join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID                                           
                //                           select new
                //                           {
                //                               Touser = "PowerAdmin",
                //                               FromUser = y.UName,
                //                               REQUEST_DATE = x.REQUEST_DATE,
                //                               AMOUNT = x.AMOUNT,
                //                               BANK_ACCOUNT = x.BANK_ACCOUNT,
                //                               TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                //                               STATUS = x.STATUS,
                //                               APPROVED_BY = x.APPROVED_BY,
                //                               APPROVAL_DATE = x.APPROVAL_DATE,
                //                               SLN = x.SLN
                //                           }).AsEnumerable().Select(z => new TBL_BALANCE_TRANSFER_LOGS
                //                           {
                //                               ToUser = z.Touser,
                //                               FromUser = z.FromUser,
                //                               AMOUNT = z.AMOUNT,
                //                               REQUEST_DATE = z.REQUEST_DATE,
                //                               BANK_ACCOUNT = z.BANK_ACCOUNT,
                //                               TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                //                               STATUS = z.STATUS,
                //                               APPROVED_BY = z.APPROVED_BY,
                //                               APPROVAL_DATE = z.APPROVAL_DATE,
                //                               SLN = z.SLN
                //                           }).ToList();
                ////return PartialView("IndexGrid", transactionlist);


                //var transactiondetails = db.TBL_BALANCE_TRANSFER_LOGS.ToList();
                return PartialView(CreateExportableGrid());


            }
            catch (Exception ex)
            {
                throw ex;
            }
            //return PartialView();
        }
        
        [HttpGet]
        public FileResult ExportIndex()
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = CreateExportableGrid();
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

        private IGrid<TBL_BALANCE_TRANSFER_LOGS> CreateExportableGrid()
        {
            var db = new DBContext();
            var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                   join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID where x.TO_MEMBER != 0
                                   select new
                                   {
                                       Touser = "White Label",
                                       transId=x.TransactionID,
                                       FromUser = y.UName,
                                       REQUEST_DATE = x.REQUEST_DATE,
                                       AMOUNT = x.AMOUNT,
                                       BANK_ACCOUNT = x.BANK_ACCOUNT,
                                       TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                       STATUS = x.STATUS,
                                       APPROVED_BY = x.APPROVED_BY,
                                       APPROVAL_DATE = x.APPROVAL_DATE,
                                       SLN = x.SLN
                                   }).AsEnumerable().Select(z => new TBL_BALANCE_TRANSFER_LOGS
                                   {
                                       ToUser = z.Touser,
                                       TransactionID=z.transId,
                                       FromUser = z.FromUser,
                                       AMOUNT = z.AMOUNT,
                                       REQUEST_DATE = z.REQUEST_DATE,
                                       BANK_ACCOUNT = z.BANK_ACCOUNT,
                                       TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                       STATUS = z.STATUS,
                                       APPROVED_BY = z.APPROVED_BY,
                                       APPROVAL_DATE = z.APPROVAL_DATE,
                                       SLN = z.SLN
                                   }).ToList();

            IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = new Grid<TBL_BALANCE_TRANSFER_LOGS>(transactionlist);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.TransactionID).Titled("Trans Id");
            grid.Columns.Add(model => model.ToUser).Titled("To User");
            grid.Columns.Add(model => model.FromUser).Titled("From Member");
            grid.Columns.Add(model => model.REQUEST_DATE).Titled("Req Date").Formatted("{0:yyyy-MM-dd}").MultiFilterable(true);
            grid.Columns.Add(model => model.AMOUNT).Titled("Amount");
            grid.Columns.Add(model => model.BANK_ACCOUNT).Titled("Bank Acnt");
            grid.Columns.Add(model => model.TRANSACTION_DETAILS).Titled("Pay Method");
            grid.Columns.Add(model => model.STATUS).Titled("STATUS");
            grid.Columns.Add(model => model.APPROVAL_DATE).Titled("Apprv/Decline Date").Formatted("{0:yyyy-MM-dd}").MultiFilterable(true);
            grid.Columns.Add(model => model.APPROVED_BY).Titled("Apprv By");
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
        
        public ActionResult GetPeople(string query)
        {
            return Json(_GetPeople(query), JsonRequestBehavior.AllowGet);
        }

        private List<Autocomplete> _GetPeople(string query)
        {
            List<Autocomplete> people = new List<Autocomplete>();
            var db = new DBContext();
            try
            {
                var results = (from p in db.TBL_MASTER_MEMBER
                               where (p.UName).Contains(query) && p.INTRODUCER == MemberCurrentUser.MEM_ID
                               orderby p.UName
                               select p).ToList();
                foreach (var r in results)
                {
                    // create objects
                    Autocomplete Username = new Autocomplete();

                    //Username.FromUser = string.Format("{0} {1}", r.UName);
                    Username.Name = (r.UName);
                    Username.Id = r.MEM_ID;

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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> CheckReferenceNo(string referenceno)
        {
            initpage();////
            var context = new DBContext();
            var User = await context.TBL_BALANCE_TRANSFER_LOGS.Where(model => model.REFERENCE_NO == referenceno && model.TO_MEMBER==MemberCurrentUser.MEM_ID).FirstOrDefaultAsync();
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


        [HttpPost]
        public JsonResult GetChannelMembername(string Prefix)
        {
            initpage();////
            var db = new DBContext();
            var MembarList = (from N in db.TBL_MASTER_MEMBER
                            where  N.INTRODUCER == MemberCurrentUser.MEM_ID &&  N.UName.StartsWith(Prefix)
                            select new { N.UName });
            return Json(MembarList, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ApprovedRequisition()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                initpage();
                return View();
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
        public PartialViewResult ApprovedIndexGrid(string status="", string DateFrom = "", string Date_To = "")
        {
            try
            {
                var db = new DBContext();
                if (status== "Pending" && DateFrom != "" && Date_To != "")
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
                                           where x.STATUS ==status && x.TO_MEMBER == MemberCurrentUser.MEM_ID && x.REQUEST_DATE >= Date_From_Val && x.REQUEST_DATE <= Date_To_Val
                                           select new
                                           {
                                               //Touser = "White Label",
                                               TransId = x.TransactionID,
                                               FromUser = y.UName,
                                               Reference_NO = x.REFERENCE_NO,
                                               REQUEST_DATE = x.REQUEST_DATE,
                                               AMOUNT = x.AMOUNT,
                                               BANK_ACCOUNT = x.BANK_ACCOUNT,
                                               TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                               SLN = x.SLN,
                                               STATUS = x.STATUS,
                                               PAY_MODE = x.PAYMENT_METHOD,
                                               CompanyNamey=y.COMPANY ,
                                               PaymentDetail=x.PAYMENT_TXN_DETAILS
                                           }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                           {
                                               Serial_No = index + 1,
                                               //ToUser = z.Touser,
                                               TransactionID = z.TransId,
                                               REFERENCE_NO = z.Reference_NO,
                                               FromUser = z.FromUser,
                                               AMOUNT = z.AMOUNT,
                                               REQUEST_DATE = z.REQUEST_DATE,
                                               BANK_ACCOUNT = z.BANK_ACCOUNT,
                                               TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                               SLN = z.SLN,
                                               STATUS = z.STATUS,
                                               PAYMENT_METHOD = z.PAY_MODE,
                                               CompanyName=z.CompanyNamey,
                                               PAYMENT_TXN_DETAILS=z.PaymentDetail
                                           }).ToList();
                    return PartialView("ApprovedIndexGrid", transactionlist);
                }
                else if (status == "Approve" && DateFrom != "" && Date_To != "")
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
                                           where x.TO_MEMBER == MemberCurrentUser.MEM_ID && x.APPROVAL_DATE >= Date_From_Val && x.APPROVAL_DATE <= Date_To_Val && x.STATUS == status
                                           select new
                                           {
                                               Touser = "White Label",
                                               TransId = x.TransactionID,
                                               FromUser = y.UName,
                                               Reference_NO = x.REFERENCE_NO,
                                               REQUEST_DATE = x.REQUEST_DATE,
                                               AMOUNT = x.AMOUNT,
                                               BANK_ACCOUNT = x.BANK_ACCOUNT,
                                               TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                               SLN = x.SLN,
                                               STATUS = x.STATUS,
                                               PAY_MODE = x.PAYMENT_METHOD,
                                               CompanyNamey = y.COMPANY,
                                               PaymentDetail = x.PAYMENT_TXN_DETAILS
                                           }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                           {
                                               Serial_No = index + 1,
                                               ToUser = z.Touser,
                                               TransactionID = z.TransId,
                                               REFERENCE_NO = z.Reference_NO,
                                               FromUser = z.FromUser,
                                               AMOUNT = z.AMOUNT,
                                               REQUEST_DATE = z.REQUEST_DATE,
                                               BANK_ACCOUNT = z.BANK_ACCOUNT,
                                               TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                               SLN = z.SLN,
                                               STATUS = z.STATUS,
                                               PAYMENT_METHOD = z.PAY_MODE,
                                               CompanyName = z.CompanyNamey,
                                               PAYMENT_TXN_DETAILS = z.PaymentDetail
                                           }).ToList();
                    return PartialView("ApprovedIndexGrid", transactionlist);
                }
                else if (status == "Decline" && DateFrom != "" && Date_To != "")
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
                                           where x.STATUS==status && x.TO_MEMBER == MemberCurrentUser.MEM_ID && x.APPROVAL_DATE >= Date_From_Val && x.APPROVAL_DATE <= Date_To_Val
                                           select new
                                           {
                                               Touser = "White Label",
                                               TransId = x.TransactionID,
                                               FromUser = y.UName,
                                               Reference_NO = x.REFERENCE_NO,
                                               REQUEST_DATE = x.REQUEST_DATE,
                                               AMOUNT = x.AMOUNT,
                                               BANK_ACCOUNT = x.BANK_ACCOUNT,
                                               TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                               SLN = x.SLN,
                                               STATUS = x.STATUS,
                                               PAY_MODE = x.PAYMENT_METHOD,
                                               CompanyNamey = y.COMPANY,
                                               PaymentDetail = x.PAYMENT_TXN_DETAILS
                                           }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                           {
                                               Serial_No = index + 1,
                                               ToUser = z.Touser,
                                               TransactionID = z.TransId,
                                               REFERENCE_NO = z.Reference_NO,
                                               FromUser = z.FromUser,
                                               AMOUNT = z.AMOUNT,
                                               REQUEST_DATE = z.REQUEST_DATE,
                                               BANK_ACCOUNT = z.BANK_ACCOUNT,
                                               TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                               SLN = z.SLN,
                                               STATUS = z.STATUS,
                                               PAYMENT_METHOD = z.PAY_MODE,
                                               CompanyName = z.CompanyNamey,
                                               PAYMENT_TXN_DETAILS = z.PaymentDetail
                                           }).ToList();
                    return PartialView("ApprovedIndexGrid", transactionlist);
                }
                else if (status == "" && DateFrom != "" && Date_To != "")
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
                                           where  x.TO_MEMBER == MemberCurrentUser.MEM_ID && x.REQUEST_DATE >= Date_From_Val && x.REQUEST_DATE <= Date_To_Val
                                           select new
                                           {
                                               Touser = "White Label",
                                               TransId = x.TransactionID,
                                               FromUser = y.UName,
                                               Reference_NO = x.REFERENCE_NO,
                                               REQUEST_DATE = x.REQUEST_DATE,
                                               AMOUNT = x.AMOUNT,
                                               BANK_ACCOUNT = x.BANK_ACCOUNT,
                                               TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                               SLN = x.SLN,
                                               STATUS = x.STATUS,
                                               PAY_MODE = x.PAYMENT_METHOD,
                                               CompanyNamey = y.COMPANY,
                                               PaymentDetail = x.PAYMENT_TXN_DETAILS
                                           }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                           {
                                               Serial_No = index + 1,
                                               ToUser = z.Touser,
                                               TransactionID = z.TransId,
                                               REFERENCE_NO = z.Reference_NO,
                                               FromUser = z.FromUser,
                                               AMOUNT = z.AMOUNT,
                                               REQUEST_DATE = z.REQUEST_DATE,
                                               BANK_ACCOUNT = z.BANK_ACCOUNT,
                                               TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                               SLN = z.SLN,
                                               STATUS = z.STATUS,
                                               PAYMENT_METHOD = z.PAY_MODE,
                                               CompanyName = z.CompanyNamey,
                                               PAYMENT_TXN_DETAILS = z.PaymentDetail
                                           }).ToList();
                    return PartialView("ApprovedIndexGrid", transactionlist);
                }
                else if (status != "" && DateFrom == "" && Date_To == "")
                {
                    
                    var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                           join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                           where x.TO_MEMBER == MemberCurrentUser.MEM_ID && x.STATUS==status
                                           select new
                                           {
                                               Touser = "White Label",
                                               TransId = x.TransactionID,
                                               FromUser = y.UName,
                                               Reference_NO = x.REFERENCE_NO,
                                               REQUEST_DATE = x.REQUEST_DATE,
                                               AMOUNT = x.AMOUNT,
                                               BANK_ACCOUNT = x.BANK_ACCOUNT,
                                               TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                               SLN = x.SLN,
                                               STATUS = x.STATUS,
                                               PAY_MODE = x.PAYMENT_METHOD,                                               
                                               CompanyNamey = y.COMPANY,
                                               PaymentDetail = x.PAYMENT_TXN_DETAILS
                                           }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                           {
                                               Serial_No = index + 1,
                                               ToUser = z.Touser,
                                               TransactionID = z.TransId,
                                               REFERENCE_NO = z.Reference_NO,
                                               FromUser = z.FromUser,
                                               AMOUNT = z.AMOUNT,
                                               REQUEST_DATE = z.REQUEST_DATE,
                                               BANK_ACCOUNT = z.BANK_ACCOUNT,
                                               TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                               SLN = z.SLN,
                                               STATUS = z.STATUS,
                                               PAYMENT_METHOD = z.PAY_MODE,
                                               CompanyName = z.CompanyNamey,
                                               PAYMENT_TXN_DETAILS = z.PaymentDetail
                                           }).ToList();
                    return PartialView("ApprovedIndexGrid", transactionlist);
                }
                else
                {
                    DateTime Todaydate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                    var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                           join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                           where x.TO_MEMBER == MemberCurrentUser.MEM_ID && x.REQUEST_DATE == Todaydate
                                           select new
                                           {
                                               //Touser = "White Label",
                                               TransId = x.TransactionID,
                                               FromUser = y.UName,
                                               Reference_NO = x.REFERENCE_NO,
                                               REQUEST_DATE = x.REQUEST_DATE,
                                               AMOUNT = x.AMOUNT,
                                               BANK_ACCOUNT = x.BANK_ACCOUNT,
                                               TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                               SLN = x.SLN,
                                               STATUS = x.STATUS,
                                               PAY_MODE = x.PAYMENT_METHOD,
                                               CompanyNamey = y.COMPANY,
                                               PaymentDetail = x.PAYMENT_TXN_DETAILS
                                           }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                           {
                                               Serial_No = index + 1,
                                               //ToUser = z.Touser,
                                               TransactionID = z.TransId,
                                               REFERENCE_NO = z.Reference_NO,
                                               FromUser = z.FromUser,
                                               AMOUNT = z.AMOUNT,
                                               REQUEST_DATE = z.REQUEST_DATE,
                                               BANK_ACCOUNT = z.BANK_ACCOUNT,
                                               TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                               SLN = z.SLN,
                                               STATUS = z.STATUS,
                                               PAYMENT_METHOD = z.PAY_MODE,
                                               CompanyName = z.CompanyNamey,
                                               PAYMENT_TXN_DETAILS = z.PaymentDetail
                                           }).ToList();
                    return PartialView("ApprovedIndexGrid", transactionlist);
                }

            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MemberRequisition(Admin), method:- IndexGrid (GET) Line No:- 114", ex);
                throw ex;
            }

        }
        public FileResult ApprovedExportIndexGrid(string statusval, string DateFrom = "", string Date_To = "")
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = ApproveExportableGridINExcel(statusval, DateFrom, Date_To);
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
        private IGrid<TBL_BALANCE_TRANSFER_LOGS> ApproveExportableGridINExcel(string status = "", string DateFrom = "", string Date_To = "")
        {
            try
            {
                var db = new DBContext();
                if (status == "Pending" && DateFrom != "" && Date_To != "")
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
                                           where x.STATUS == status && x.TO_MEMBER == MemberCurrentUser.MEM_ID && x.REQUEST_DATE >= Date_From_Val && x.REQUEST_DATE <= Date_To_Val
                                           select new
                                           {
                                               //Touser = "White Label",
                                               TransId = x.TransactionID,
                                               FromUser = y.UName,
                                               Reference_NO = x.REFERENCE_NO,
                                               REQUEST_DATE = x.REQUEST_DATE,
                                               AMOUNT = x.AMOUNT,
                                               BANK_ACCOUNT = x.BANK_ACCOUNT,
                                               TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                               SLN = x.SLN,
                                               STATUS = x.STATUS,
                                               PAY_MODE = x.PAYMENT_METHOD,
                                               CompanyNamey = y.COMPANY,
                                               PaymentDetail = x.PAYMENT_TXN_DETAILS
                                           }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                           {
                                               Serial_No = index + 1,
                                               //ToUser = z.Touser,
                                               TransactionID = z.TransId,
                                               REFERENCE_NO = z.Reference_NO,
                                               FromUser = z.FromUser,
                                               AMOUNT = z.AMOUNT,
                                               REQUEST_DATE = z.REQUEST_DATE,
                                               BANK_ACCOUNT = z.BANK_ACCOUNT,
                                               TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                               SLN = z.SLN,
                                               STATUS = z.STATUS,
                                               PAYMENT_METHOD = z.PAY_MODE,
                                               CompanyName = z.CompanyNamey,
                                               PAYMENT_TXN_DETAILS = z.PaymentDetail
                                           }).ToList();
                    IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = new Grid<TBL_BALANCE_TRANSFER_LOGS>(transactionlist);
                    grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                    grid.Query = Request.QueryString;
                    grid.Columns.Add(model => model.Serial_No).Titled("Sr Id");
                    grid.Columns.Add(model => model.FromUser).Titled("From Member");
                    grid.Columns.Add(model => model.CompanyName).Titled("Company Name");
                    grid.Columns.Add(model => model.REFERENCE_NO).Titled("Reference No");
                    grid.Columns.Add(model => model.REQUEST_DATE).Titled("Req Date").Formatted("{0:yyyy-MM-dd}");
                    //grid.Columns.Add(model => model.REQUEST_TIME).Titled("Req Time").Formatted("{0:T}");
                    grid.Columns.Add(model => model.AMOUNT).Titled("Amouont");
                    grid.Columns.Add(model => model.PAYMENT_METHOD).Titled("Pay Mode");
                    grid.Columns.Add(model => model.BANK_ACCOUNT).Titled("Bank Account");
                    grid.Columns.Add(model => model.TRANSACTION_DETAILS).Titled("Transaction Details");
                    grid.Columns.Add(model => model.PAYMENT_TXN_DETAILS).Titled("Payment Details");
                    grid.Columns.Add(model => model.STATUS).Titled("Status");
                    grid.Pager = new GridPager<TBL_BALANCE_TRANSFER_LOGS>(grid);
                    grid.Processors.Add(grid.Pager);
                    grid.Pager.RowsPerPage = 1000000;
                    return grid;
                }
                else if (status == "Approve" && DateFrom != "" && Date_To != "")
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
                                           where x.TO_MEMBER == MemberCurrentUser.MEM_ID && x.APPROVAL_DATE >= Date_From_Val && x.APPROVAL_DATE <= Date_To_Val && x.STATUS == status
                                           select new
                                           {
                                               Touser = "White Label",
                                               TransId = x.TransactionID,
                                               FromUser = y.UName,
                                               Reference_NO = x.REFERENCE_NO,
                                               REQUEST_DATE = x.REQUEST_DATE,
                                               AMOUNT = x.AMOUNT,
                                               BANK_ACCOUNT = x.BANK_ACCOUNT,
                                               TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                               SLN = x.SLN,
                                               STATUS = x.STATUS,
                                               PAY_MODE = x.PAYMENT_METHOD,
                                               CompanyNamey = y.COMPANY,
                                               PaymentDetail = x.PAYMENT_TXN_DETAILS
                                           }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                           {
                                               Serial_No = index + 1,
                                               ToUser = z.Touser,
                                               TransactionID = z.TransId,
                                               REFERENCE_NO = z.Reference_NO,
                                               FromUser = z.FromUser,
                                               AMOUNT = z.AMOUNT,
                                               REQUEST_DATE = z.REQUEST_DATE,
                                               BANK_ACCOUNT = z.BANK_ACCOUNT,
                                               TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                               SLN = z.SLN,
                                               STATUS = z.STATUS,
                                               PAYMENT_METHOD = z.PAY_MODE,
                                               CompanyName = z.CompanyNamey,
                                               PAYMENT_TXN_DETAILS = z.PaymentDetail
                                           }).ToList();
                    IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = new Grid<TBL_BALANCE_TRANSFER_LOGS>(transactionlist);
                    grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                    grid.Query = Request.QueryString;
                    grid.Columns.Add(model => model.Serial_No).Titled("Sr Id");
                    grid.Columns.Add(model => model.FromUser).Titled("From Member");
                    grid.Columns.Add(model => model.CompanyName).Titled("Company Name");
                    grid.Columns.Add(model => model.REFERENCE_NO).Titled("Reference No");
                    grid.Columns.Add(model => model.REQUEST_DATE).Titled("Req Date").Formatted("{0:yyyy-MM-dd}");
                    //grid.Columns.Add(model => model.REQUEST_TIME).Titled("Req Date").Formatted("{0:T}");
                    grid.Columns.Add(model => model.AMOUNT).Titled("Amouont");
                    grid.Columns.Add(model => model.PAYMENT_METHOD).Titled("Pay Mode");
                    grid.Columns.Add(model => model.BANK_ACCOUNT).Titled("Bank Account");
                    grid.Columns.Add(model => model.TRANSACTION_DETAILS).Titled("Transaction Details");
                    grid.Columns.Add(model => model.PAYMENT_TXN_DETAILS).Titled("Payment Details");
                    grid.Columns.Add(model => model.STATUS).Titled("Status");
                    grid.Pager = new GridPager<TBL_BALANCE_TRANSFER_LOGS>(grid);
                    grid.Processors.Add(grid.Pager);
                    grid.Pager.RowsPerPage = 1000000;
                    return grid;
                }
                else if (status == "Decline" && DateFrom != "" && Date_To != "")
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
                                           where x.STATUS == status && x.TO_MEMBER == MemberCurrentUser.MEM_ID && x.APPROVAL_DATE >= Date_From_Val && x.APPROVAL_DATE <= Date_To_Val
                                           select new
                                           {
                                               Touser = "White Label",
                                               TransId = x.TransactionID,
                                               FromUser = y.UName,
                                               Reference_NO = x.REFERENCE_NO,
                                               REQUEST_DATE = x.REQUEST_DATE,
                                               AMOUNT = x.AMOUNT,
                                               BANK_ACCOUNT = x.BANK_ACCOUNT,
                                               TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                               SLN = x.SLN,
                                               STATUS = x.STATUS,
                                               PAY_MODE = x.PAYMENT_METHOD,
                                               CompanyNamey = y.COMPANY,
                                               PaymentDetail = x.PAYMENT_TXN_DETAILS
                                           }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                           {
                                               Serial_No = index + 1,
                                               ToUser = z.Touser,
                                               TransactionID = z.TransId,
                                               REFERENCE_NO = z.Reference_NO,
                                               FromUser = z.FromUser,
                                               AMOUNT = z.AMOUNT,
                                               REQUEST_DATE = z.REQUEST_DATE,
                                               BANK_ACCOUNT = z.BANK_ACCOUNT,
                                               TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                               SLN = z.SLN,
                                               STATUS = z.STATUS,
                                               PAYMENT_METHOD = z.PAY_MODE,
                                               CompanyName = z.CompanyNamey,
                                               PAYMENT_TXN_DETAILS = z.PaymentDetail
                                           }).ToList();
                    IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = new Grid<TBL_BALANCE_TRANSFER_LOGS>(transactionlist);
                    grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                    grid.Query = Request.QueryString;
                    grid.Columns.Add(model => model.Serial_No).Titled("Sr Id");
                    grid.Columns.Add(model => model.FromUser).Titled("From Member");
                    grid.Columns.Add(model => model.CompanyName).Titled("Company Name");
                    grid.Columns.Add(model => model.REFERENCE_NO).Titled("Reference No");
                    grid.Columns.Add(model => model.REQUEST_DATE).Titled("Req Date").Formatted("{0:yyyy-MM-dd}");
                    //grid.Columns.Add(model => model.REQUEST_TIME).Titled("Req Date").Formatted("{0:T}");
                    grid.Columns.Add(model => model.AMOUNT).Titled("Amouont");
                    grid.Columns.Add(model => model.PAYMENT_METHOD).Titled("Pay Mode");
                    grid.Columns.Add(model => model.BANK_ACCOUNT).Titled("Bank Account");
                    grid.Columns.Add(model => model.TRANSACTION_DETAILS).Titled("Transaction Details");
                    grid.Columns.Add(model => model.PAYMENT_TXN_DETAILS).Titled("Payment Details");
                    grid.Columns.Add(model => model.STATUS).Titled("Status");
                    grid.Pager = new GridPager<TBL_BALANCE_TRANSFER_LOGS>(grid);
                    grid.Processors.Add(grid.Pager);
                    grid.Pager.RowsPerPage = 1000000;
                    return grid;
                }
                else if (status == "" && DateFrom != "" && Date_To != "")
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
                                           where x.TO_MEMBER == MemberCurrentUser.MEM_ID && x.REQUEST_DATE >= Date_From_Val && x.REQUEST_DATE <= Date_To_Val
                                           select new
                                           {
                                               Touser = "White Label",
                                               TransId = x.TransactionID,
                                               FromUser = y.UName,
                                               Reference_NO = x.REFERENCE_NO,
                                               REQUEST_DATE = x.REQUEST_DATE,
                                               AMOUNT = x.AMOUNT,
                                               BANK_ACCOUNT = x.BANK_ACCOUNT,
                                               TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                               SLN = x.SLN,
                                               STATUS = x.STATUS,
                                               PAY_MODE = x.PAYMENT_METHOD,
                                               CompanyNamey = y.COMPANY,
                                               PaymentDetail = x.PAYMENT_TXN_DETAILS
                                           }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                           {
                                               Serial_No = index + 1,
                                               ToUser = z.Touser,
                                               TransactionID = z.TransId,
                                               REFERENCE_NO = z.Reference_NO,
                                               FromUser = z.FromUser,
                                               AMOUNT = z.AMOUNT,
                                               REQUEST_DATE = z.REQUEST_DATE,
                                               BANK_ACCOUNT = z.BANK_ACCOUNT,
                                               TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                               SLN = z.SLN,
                                               STATUS = z.STATUS,
                                               PAYMENT_METHOD = z.PAY_MODE,
                                               CompanyName = z.CompanyNamey,
                                               PAYMENT_TXN_DETAILS = z.PaymentDetail
                                           }).ToList();
                    IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = new Grid<TBL_BALANCE_TRANSFER_LOGS>(transactionlist);
                    grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                    grid.Query = Request.QueryString;
                    grid.Columns.Add(model => model.Serial_No).Titled("Sr Id");
                    grid.Columns.Add(model => model.FromUser).Titled("From Member");
                    grid.Columns.Add(model => model.CompanyName).Titled("Company Name");
                    grid.Columns.Add(model => model.REFERENCE_NO).Titled("Reference No");
                    grid.Columns.Add(model => model.REQUEST_DATE).Titled("Req Date").Formatted("{0:yyyy-MM-dd}");
                    //grid.Columns.Add(model => model.REQUEST_TIME).Titled("Req Date").Formatted("{0:T}");
                    grid.Columns.Add(model => model.AMOUNT).Titled("Amouont");
                    grid.Columns.Add(model => model.PAYMENT_METHOD).Titled("Pay Mode");
                    grid.Columns.Add(model => model.BANK_ACCOUNT).Titled("Bank Account");
                    grid.Columns.Add(model => model.TRANSACTION_DETAILS).Titled("Transaction Details");
                    grid.Columns.Add(model => model.PAYMENT_TXN_DETAILS).Titled("Payment Details");
                    grid.Columns.Add(model => model.STATUS).Titled("Status");
                    grid.Pager = new GridPager<TBL_BALANCE_TRANSFER_LOGS>(grid);
                    grid.Processors.Add(grid.Pager);
                    grid.Pager.RowsPerPage = 1000000;
                    return grid;
                }
                else if (status != "" && DateFrom == "" && Date_To == "")
                {

                    var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                           join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                           where x.TO_MEMBER == MemberCurrentUser.MEM_ID && x.STATUS == status
                                           select new
                                           {
                                               Touser = "White Label",
                                               TransId = x.TransactionID,
                                               FromUser = y.UName,
                                               Reference_NO = x.REFERENCE_NO,
                                               REQUEST_DATE = x.REQUEST_DATE,
                                               AMOUNT = x.AMOUNT,
                                               BANK_ACCOUNT = x.BANK_ACCOUNT,
                                               TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                               SLN = x.SLN,
                                               STATUS = x.STATUS,
                                               PAY_MODE = x.PAYMENT_METHOD,
                                               CompanyNamey = y.COMPANY,
                                               PaymentDetail = x.PAYMENT_TXN_DETAILS
                                           }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                           {
                                               Serial_No = index + 1,
                                               ToUser = z.Touser,
                                               TransactionID = z.TransId,
                                               REFERENCE_NO = z.Reference_NO,
                                               FromUser = z.FromUser,
                                               AMOUNT = z.AMOUNT,
                                               REQUEST_DATE = z.REQUEST_DATE,
                                               BANK_ACCOUNT = z.BANK_ACCOUNT,
                                               TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                               SLN = z.SLN,
                                               STATUS = z.STATUS,
                                               PAYMENT_METHOD = z.PAY_MODE,
                                               CompanyName = z.CompanyNamey,
                                               PAYMENT_TXN_DETAILS = z.PaymentDetail
                                           }).ToList();
                    IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = new Grid<TBL_BALANCE_TRANSFER_LOGS>(transactionlist);
                    grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                    grid.Query = Request.QueryString;
                    grid.Columns.Add(model => model.Serial_No).Titled("Sr Id");
                    grid.Columns.Add(model => model.FromUser).Titled("From Member");
                    grid.Columns.Add(model => model.CompanyName).Titled("Company Name");
                    grid.Columns.Add(model => model.REFERENCE_NO).Titled("Reference No");
                    grid.Columns.Add(model => model.REQUEST_DATE).Titled("Req Date").Formatted("{0:yyyy-MM-dd}");
                    //grid.Columns.Add(model => model.REQUEST_TIME).Titled("Req Date").Formatted("{0:T}");
                    grid.Columns.Add(model => model.AMOUNT).Titled("Amouont");
                    grid.Columns.Add(model => model.PAYMENT_METHOD).Titled("Pay Mode");
                    grid.Columns.Add(model => model.BANK_ACCOUNT).Titled("Bank Account");
                    grid.Columns.Add(model => model.TRANSACTION_DETAILS).Titled("Transaction Details");
                    grid.Columns.Add(model => model.PAYMENT_TXN_DETAILS).Titled("Payment Details");
                    grid.Columns.Add(model => model.STATUS).Titled("Status");
                    grid.Pager = new GridPager<TBL_BALANCE_TRANSFER_LOGS>(grid);
                    grid.Processors.Add(grid.Pager);
                    grid.Pager.RowsPerPage = 1000000;
                    return grid;
                }
                else
                {
                    DateTime Todaydate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                    var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                           join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                           where x.TO_MEMBER == MemberCurrentUser.MEM_ID && x.REQUEST_DATE == Todaydate
                                           select new
                                           {
                                               //Touser = "White Label",
                                               TransId = x.TransactionID,
                                               FromUser = y.UName,
                                               Reference_NO = x.REFERENCE_NO,
                                               REQUEST_DATE = x.REQUEST_DATE,
                                               AMOUNT = x.AMOUNT,
                                               BANK_ACCOUNT = x.BANK_ACCOUNT,
                                               TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                               SLN = x.SLN,
                                               STATUS = x.STATUS,
                                               PAY_MODE = x.PAYMENT_METHOD,
                                               CompanyNamey = y.COMPANY,
                                               PaymentDetail = x.PAYMENT_TXN_DETAILS
                                           }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                           {
                                               Serial_No = index + 1,
                                               //ToUser = z.Touser,
                                               TransactionID = z.TransId,
                                               REFERENCE_NO = z.Reference_NO,
                                               FromUser = z.FromUser,
                                               AMOUNT = z.AMOUNT,
                                               REQUEST_DATE = z.REQUEST_DATE,
                                               BANK_ACCOUNT = z.BANK_ACCOUNT,
                                               TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                               SLN = z.SLN,
                                               STATUS = z.STATUS,
                                               PAYMENT_METHOD = z.PAY_MODE,
                                               CompanyName = z.CompanyNamey,
                                               PAYMENT_TXN_DETAILS = z.PaymentDetail
                                           }).ToList();
                    IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = new Grid<TBL_BALANCE_TRANSFER_LOGS>(transactionlist);
                    grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                    grid.Query = Request.QueryString;
                    grid.Columns.Add(model => model.Serial_No).Titled("Sr Id");
                    grid.Columns.Add(model => model.FromUser).Titled("From Member");
                    grid.Columns.Add(model => model.CompanyName).Titled("Company Name");
                    grid.Columns.Add(model => model.REFERENCE_NO).Titled("Reference No");
                    grid.Columns.Add(model => model.REQUEST_DATE).Titled("Req Date").Formatted("{0:yyyy-MM-dd}");
                    //grid.Columns.Add(model => model.REQUEST_TIME).Titled("Req Date").Formatted("{0:T}");
                    grid.Columns.Add(model => model.AMOUNT).Titled("Amouont");
                    grid.Columns.Add(model => model.PAYMENT_METHOD).Titled("Pay Mode");
                    grid.Columns.Add(model => model.BANK_ACCOUNT).Titled("Bank Account");
                    grid.Columns.Add(model => model.TRANSACTION_DETAILS).Titled("Transaction Details");
                    grid.Columns.Add(model => model.PAYMENT_TXN_DETAILS).Titled("Payment Details");
                    grid.Columns.Add(model => model.STATUS).Titled("Status");
                    grid.Pager = new GridPager<TBL_BALANCE_TRANSFER_LOGS>(grid);
                    grid.Processors.Add(grid.Pager);
                    grid.Pager.RowsPerPage = 1000000;
                    return grid;
                }

            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MemberRequisition(Admin), method:- IndexGrid (GET) Line No:- 114", ex);
                throw ex;
            }
        }

    }
}