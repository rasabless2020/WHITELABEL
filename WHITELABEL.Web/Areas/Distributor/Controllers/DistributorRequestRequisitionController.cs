using log4net;
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


namespace WHITELABEL.Web.Areas.Distributor.Controllers
{
    [Authorize]
    public class DistributorRequestRequisitionController : DistributorBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        //public void initpage()
        //{
        //    try
        //    {
        //        ViewBag.ControllerName = "Distributor Requisition";
        //        SystemClass sclass = new SystemClass();
        //        string userID = sclass.GetLoggedUser();
        //        long userid = long.Parse(userID);
        //        var dbmain = new DBContext();
        //        if (userID != null && userID != "")
        //        {
        //            TBL_MASTER_MEMBER currUser = dbmain.TBL_MASTER_MEMBER.SingleOrDefault(c => c.MEM_ID == userid && c.ACTIVE_MEMBER == true);
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

        // GET: Distributor/DistributorRequestRequisition
        public ActionResult Index()
        {
            if (Session["DistributorUserId"] != null)
            {
                initpage();
                return View();
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
        public PartialViewResult IndexGrid(string DateFrom = "", string Date_To = "")
        {
            try
            {
                var db = new DBContext();               
                return PartialView(CreateExportableGridINExcel(DateFrom,Date_To));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpGet]
        public FileResult GridExportIndex()
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = CreateExportableGridINExcel();
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

        private IGrid<TBL_BALANCE_TRANSFER_LOGS> CreateExportableGridINExcel(string DateFrom="",string Date_To="")
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
                                       where x.STATUS == "Pending" && x.FROM_MEMBER == MemberCurrentUser.MEM_ID && x.TO_MEMBER == y.INTRODUCER && x.REQUEST_DATE >= Date_From_Val && x.REQUEST_DATE <= Date_To_Val
                                       select new
                                       {
                                           Touser = "SUPER DISTRIBUTOR",
                                           transid = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           SLN = x.SLN,
                                           PAY_MODE = x.PAYMENT_METHOD
                                       }).AsEnumerable().Select(z => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           ToUser = z.Touser,
                                           TransactionID = z.transid,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           SLN = z.SLN,
                                           PAYMENT_METHOD = z.PAY_MODE
                                       }).ToList();

                IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = new Grid<TBL_BALANCE_TRANSFER_LOGS>(transactionlist);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.TransactionID).Titled("Trans Id");
                grid.Columns.Add(model => model.ToUser).Titled("To User");
                grid.Columns.Add(model => model.FromUser).Titled("From Member");
                grid.Columns.Add(model => model.REQUEST_DATE).Titled("Req Date").Formatted("{0:yyyy-MM-dd}").MultiFilterable(true);
                grid.Columns.Add(model => model.AMOUNT).Titled("Amount");
                grid.Columns.Add(model => model.PAYMENT_METHOD).Titled("Pay Mode");
                grid.Columns.Add(model => model.BANK_ACCOUNT).Titled("Bank Acnt");
                grid.Columns.Add(model => model.TRANSACTION_DETAILS).Titled("Description");

                grid.Columns.Add(model => model.SLN).Titled("Edit").Encoded(false).Filterable(false).Sortable(false)
                    .RenderedAs(model => "<div style='text-align:center'><a href='" + @Url.Action("RequisitionDetails", "DistributorRequestRequisition", new { area = "Distributor", transId = Encrypt.EncryptMe(model.SLN.ToString()) }) + "'  title='Edit'><i class='fa fa-edit'></i></a></div>");


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
            else
            {
                DateTime Todaydate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.STATUS == "Pending" && x.FROM_MEMBER == MemberCurrentUser.MEM_ID && x.TO_MEMBER == y.INTRODUCER && x.REQUEST_DATE == Todaydate
                                       select new
                                       {
                                           Touser = "SUPER DISTRIBUTOR",
                                           transid = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           SLN = x.SLN,
                                           PAY_MODE = x.PAYMENT_METHOD
                                       }).AsEnumerable().Select(z => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           ToUser = z.Touser,
                                           TransactionID = z.transid,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           SLN = z.SLN,
                                           PAYMENT_METHOD = z.PAY_MODE
                                       }).ToList();

                IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = new Grid<TBL_BALANCE_TRANSFER_LOGS>(transactionlist);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.TransactionID).Titled("Trans Id");
                grid.Columns.Add(model => model.ToUser).Titled("To User");
                grid.Columns.Add(model => model.FromUser).Titled("From Member");
                grid.Columns.Add(model => model.REQUEST_DATE).Titled("Req Date").Formatted("{0:yyyy-MM-dd}").MultiFilterable(true);
                grid.Columns.Add(model => model.AMOUNT).Titled("Amount");
                grid.Columns.Add(model => model.PAYMENT_METHOD).Titled("Pay Mode");
                grid.Columns.Add(model => model.BANK_ACCOUNT).Titled("Bank Acnt");
                grid.Columns.Add(model => model.TRANSACTION_DETAILS).Titled("Description");
                grid.Columns.Add(model => model.SLN).Titled("Edit").Encoded(false).Filterable(false).Sortable(false)
                    .RenderedAs(model => "<div style='text-align:center'><a href='" + @Url.Action("RequisitionDetails", "DistributorRequestRequisition", new { area = "Distributor", transId = Encrypt.EncryptMe(model.SLN.ToString()) }) + "'  title='Edit'><i class='fa fa-edit'></i></a></div>");
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
                
        }

        public ActionResult RequisitionDetails(string transId = "")
        {
            if (Session["DistributorUserId"] != null)
            {
                try
                {
                    initpage();////
                    var db = new DBContext();
                    var whiteleveluser = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).FirstOrDefault();
                    var introducerdetails = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == whiteleveluser.INTRODUCER).FirstOrDefault();
                    ViewBag.IntroducerName = introducerdetails.MEMBER_NAME;
                    ViewBag.IntroducerEmail = introducerdetails.EMAIL_ID;
                    ViewBag.IntroducerMobile = introducerdetails.MEMBER_MOBILE;
                    ViewBag.IntroducerMemberId = introducerdetails.MEM_ID;
                    if (transId == "")
                    {
                        var memberService = (from x in db.TBL_MASTER_MEMBER
                                             where x.CREATED_BY == MemberCurrentUser.MEM_ID
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
                        //var whiteleveluser = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).FirstOrDefault();
                        var BankInformation = (from x in db.TBL_SETTINGS_BANK_DETAILS
                                               where x.MEM_ID == whiteleveluser.INTRODUCER && x.ISDELETED == 0
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
                        //ViewBag.Introducer = whiteleveluser.UName;                        
                        ViewBag.Introducer = introducerdetails.UName;
                        ViewBag.checkbank = "0";
                        return View();
                    }
                    else
                    {
                        string decripttransId = Decrypt.DecryptMe(transId);
                        long transID = long.Parse(decripttransId);
                        var TransactionInfo = db.TBL_BALANCE_TRANSFER_LOGS.Where(x => x.SLN == transID).FirstOrDefault();
                       // var whiteleveluser = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).FirstOrDefault();
                        ViewBag.Introducer = whiteleveluser.UName;
                        var memberService = (from x in db.TBL_MASTER_MEMBER
                                             where x.MEM_ID == TransactionInfo.FROM_MEMBER
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
                                               where x.MEM_ID == whiteleveluser.INTRODUCER && x.ISDELETED == 0
                                               select new
                                               {
                                                   //BankID = x.SL_NO,
                                                   BankName = (x.BANK + "-" + x.ACCOUNT_NO)
                                               }).AsEnumerable().Select(z => new ViewBankDetails
                                               {
                                                   //BankID = z.BankID.ToString(),
                                                   BankName = z.BankName
                                               }).ToList().Distinct();
                        ViewBag.BankInformation = new SelectList(BankInformation, "BankName", "BankName");                       
                        TransactionInfo.FromUser = TransactionInfo.FROM_MEMBER.ToString();
                        long introducer = long.Parse(whiteleveluser.INTRODUCER.ToString());
                        TransactionInfo.FROM_MEMBER = introducer;
                        TransactionInfo.PAYMENT_METHOD = TransactionInfo.PAYMENT_METHOD;
                        TransactionInfo.REQUEST_DATE = Convert.ToDateTime(TransactionInfo.REQUEST_DATE.ToString("yyyy-MM-dd").Substring(0, 10));                        
                        TransactionInfo.BANK_ACCOUNT = TransactionInfo.BANK_ACCOUNT;
                        TransactionInfo.TRANSACTION_DETAILS = TransactionInfo.TRANSACTION_DETAILS;
                        ViewBag.checkbank = "1";
                        return View(TransactionInfo);
                    }

                }
                catch (Exception ex)
                {
                    Logger.Error("Controller:-  DistributorRequestRequisition(Distributor), method:- RequisitionDetails (GET) Line No:- 276", ex);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(true)]
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
                    var whiteleveluser =await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).FirstOrDefaultAsync();
                    var translist =await db.TBL_BALANCE_TRANSFER_LOGS.Where(x => x.SLN == objval.SLN).FirstOrDefaultAsync();
                    if (translist != null)
                    {
                        translist.REQUEST_DATE = Convert.ToDateTime(objval.REQUEST_DATE);
                        translist.REQUEST_TIME = System.DateTime.Now;
                        translist.BANK_ACCOUNT = objval.BANK_ACCOUNT;
                        translist.AMOUNT = objval.AMOUNT;
                        translist.PAYMENT_METHOD = objval.PAYMENT_METHOD;
                        translist.TRANSFER_METHOD = "Cash";
                        translist.TRANSACTION_DETAILS = objval.TRANSACTION_DETAILS;
                        translist.BANK_CHARGES = objval.BANK_CHARGES;
                        translist.INSERTED_BY = MemberCurrentUser.MEM_ID;
                        translist.FromUser = "test";
                        db.Entry(translist).State = System.Data.Entity.EntityState.Modified;
                       await db.SaveChangesAsync();
                        //return RedirectToAction("Index");
                    }
                    else
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
                            //long fromuser = long.Parse(objval.FromUser);
                            long fromuser = long.Parse(whiteleveluser.INTRODUCER.ToString());
                            objval.TransactionID = fromuser + "" + MemberCurrentUser.MEM_ID + DateTime.Now.ToString("yyyyMMdd") + "" + DateTime.Now.ToString("HHMMss");
                            objval.TO_MEMBER = fromuser;
                            objval.FROM_MEMBER = MemberCurrentUser.MEM_ID;
                            objval.REQUEST_DATE = Convert.ToDateTime(objval.REQUEST_DATE);
                            objval.REQUEST_TIME = System.DateTime.Now;
                            objval.BANK_ACCOUNT = objval.BANK_ACCOUNT;
                            objval.REFERENCE_NO = objval.REFERENCE_NO;
                            objval.STATUS = "Pending";
                            objval.TRANSFER_METHOD = "Cash";
                            objval.FromUser = "Test";
                            objval.BANK_CHARGES = objval.BANK_CHARGES;
                            objval.INSERTED_BY = MemberCurrentUser.MEM_ID;
                            db.TBL_BALANCE_TRANSFER_LOGS.Add(objval);
                            await db.SaveChangesAsync();
                            //return RedirectToAction("Index");
                        }
                    }
                    ContextTransaction.Commit();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    Logger.Error("Controller:-  DistributorRequestRequisition(Distributor), method:- RequisitionDetails (POST) Line No:- 341", ex);
                    return RedirectToAction("Exception", "ErrorHandler", new { area = "" });
                    throw ex;
                }
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