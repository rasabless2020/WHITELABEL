using NonFactors.Mvc.Grid;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;

namespace WHITELABEL.Web.Controllers
{
    public class RequisitionController : BaseController
    {
        public void initpage()
        {
            try
            {
                ViewBag.ControllerName = "Requisition Details";
                SystemClass sclass = new SystemClass();
                string userID = sclass.GetLoggedUser();
                long userid = long.Parse(userID);
                var dbmain = new DBContext();
                if (userID != null && userID != "")
                {
                    TBL_AUTH_ADMIN_USER currUser = dbmain.TBL_AUTH_ADMIN_USERS.SingleOrDefault(c => c.USER_ID == userid && c.ACTIVE_USER == true);

                    if (currUser != null)
                    {
                        Session["UserId"] = currUser.USER_ID;
                        // Session["UserName"] = currUser.UserName;
                    }
                }
                if (Session["UserId"] == null)
                {
                    Response.Redirect("~/Login/LogOut");
                    return;
                }
                bool Islogin = false;

                if (Session["UserId"] != null)
                {
                    Islogin = true;
                    ViewBag.CurrentUserId = CurrentUser.USER_ID;
                }
                ViewBag.Islogin = Islogin;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException);
                return;

            }
        }
        // GET: Requisition
        public ActionResult Index()
        {
            initpage();
            return View();
        }
        public PartialViewResult IndexGrid()
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
                return PartialView(CreateExportableGridINExcel());
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

        private IGrid<TBL_BALANCE_TRANSFER_LOGS> CreateExportableGridINExcel()
        {
            var db = new DBContext();
            var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                   join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                   where x.STATUS == "Pending"
                                   select new
                                   {
                                       Touser = "PowerAdmin",
                                       TransId=x.TransactionID,
                                       FromUser = y.UName,
                                       REQUEST_DATE = x.REQUEST_DATE,
                                       AMOUNT = x.AMOUNT,
                                       BANK_ACCOUNT = x.BANK_ACCOUNT,
                                       TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                       SLN = x.SLN
                                   }).AsEnumerable().Select(z => new TBL_BALANCE_TRANSFER_LOGS
                                   {
                                       ToUser = z.Touser,
                                       TransactionID=z.TransId,
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
            grid.Columns.Add(model => model.ToUser).Titled("To User");
            grid.Columns.Add(model => model.FromUser).Titled("From Member");
            grid.Columns.Add(model => model.REQUEST_DATE).Titled("Req Date").Formatted("{0:d}").MultiFilterable(true);
            grid.Columns.Add(model => model.AMOUNT).Titled("Amount");
            grid.Columns.Add(model => model.BANK_ACCOUNT).Titled("Bank Acnt");
            grid.Columns.Add(model => model.TRANSACTION_DETAILS).Titled("Pay Method");
            grid.Columns.Add(model => model.SLN).Titled("").Encoded(false).Filterable(false).Sortable(false)
                .RenderedAs(model => "<a href='" + @Url.Action("RequisitionDetails", "Requisition", new { transId = Encrypt.EncryptMe(model.SLN.ToString()) }) + "' class='btn btn-primary btn-xs'>Edit</a>");
            grid.Columns.Add(model => model.SLN).Titled("").Encoded(false).Filterable(false).Sortable(false)
                    .RenderedAs(model => "<button class='btn btn-primary btn-xs' data-toggle='modal' data-target='.transd' id='transactionvalueid' data-id=" + model.SLN + " onclick='getvalue(" + model.SLN + ");'>Approve</button>");
            grid.Columns.Add(model => model.SLN).Titled("").Encoded(false).Filterable(false).Sortable(false)
                    .RenderedAs(model => "<a href='javascript:void(0)' class='btn btn-denger btn-xs' onclick='DeactivateTransactionlist(" + model.SLN + ");return 0;'>Deactivate</a>");

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



        public ActionResult RequisitionDetails(string transId="")
        {
            try
            {
                var db = new DBContext();
                if (transId == "")
                { 
                    var memberService = (from x in db.TBL_MASTER_MEMBER
                                         where x.UNDER_WHITE_LEVEL == 0
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

                    var BankInformation = (from x in db.TBL_SETTINGS_BANK_DETAILS where x.MEM_ID== 0 && x.ISDELETED==0 
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
                    var memberService = (from x in db.TBL_MASTER_MEMBER where x.MEM_ID==TransactionInfo.FROM_MEMBER
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

                    //TransactionInfo.FromUser = TransactionInfo.FROM_MEMBER.ToString();
                    TransactionInfo.FromUser = memberService.Select(x => x.TextValue).FirstOrDefault();
                    //TransactionInfo.FromUser = TransactionInfo.FROM_MEMBER.ToString();
                    TransactionInfo.PAYMENT_METHOD = TransactionInfo.PAYMENT_METHOD;
                    TransactionInfo.REQUEST_DATE = Convert.ToDateTime(TransactionInfo.REQUEST_DATE.ToString("yyyy-MM-dd").Substring(0,10));
                    //TransactionInfo.BankInfo = TransactionInfo.BANK_ACCOUNT;
                    TransactionInfo.BANK_ACCOUNT = TransactionInfo.BANK_ACCOUNT;
                    TransactionInfo.TRANSACTION_DETAILS = TransactionInfo.TRANSACTION_DETAILS;
                    ViewBag.checkbank = "1";
                    return View(TransactionInfo);
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
        [HttpPost]
        public ActionResult RequisitionDetails(TBL_BALANCE_TRANSFER_LOGS objval)
        {
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    //long userId = long.Parse(objval.FromUser);
                    // var membertype = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == userId).FirstOrDefault();
                   // var vartempReviewDate = Convert.ToDateTime(Request["requisitiondate"].ToString());

                    var translist = db.TBL_BALANCE_TRANSFER_LOGS.Where(x => x.SLN == objval.SLN).FirstOrDefault();
                    if (translist != null)
                    {
                        translist.REQUEST_DATE = objval.REQUEST_DATE;
                        translist.REQUEST_TIME = System.DateTime.Now;
                        translist.BANK_ACCOUNT = objval.BANK_ACCOUNT;
                        translist.AMOUNT = objval.AMOUNT;
                        translist.PAYMENT_METHOD = objval.PAYMENT_METHOD;
                        translist.TRANSFER_METHOD = "Cash";
                        translist.TRANSACTION_DETAILS = objval.TRANSACTION_DETAILS;
                        db.Entry(translist).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        //return RedirectToAction("Index");
                    }
                    else
                    {
                        //long fromuser = long.Parse(objval.FromUser);
                        long fromuser = objval.FROM_MEMBER;
                        string timeval = DateTime.Now.ToString("hhmmss");
                        string dateval = DateTime.Now.ToString("yyyyMMdd");
                        objval.TransactionID = fromuser + "" + 0 + DateTime.Now.ToString("yyyyMMdd-HHMMss");
                        objval.TO_MEMBER = 0;
                        objval.FROM_MEMBER = fromuser;
                        objval.REQUEST_DATE = Convert.ToDateTime(objval.REQUEST_DATE);
                        objval.REQUEST_TIME = System.DateTime.Now;
                        objval.BANK_ACCOUNT = objval.BANK_ACCOUNT;
                        objval.STATUS = "Pending";
                        objval.TRANSFER_METHOD = "Cash";
                        db.TBL_BALANCE_TRANSFER_LOGS.Add(objval);
                        db.SaveChanges();
                        //return RedirectToAction("Index");
                    }
                    ContextTransaction.Commit();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    throw ex;
                }
            }
        }

        [HttpPost]
        public JsonResult DeactivateTransactionDetails(string transid = "")
        {
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    long TransID = long.Parse(transid);
                    var transDeactivate = db.TBL_BALANCE_TRANSFER_LOGS.Where(x => x.SLN == TransID).FirstOrDefault();
                    transDeactivate.STATUS = "Decline";
                    db.Entry(transDeactivate).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    ContextTransaction.Commit();
                    return Json(new { Result = "true" });
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    return Json(new { Result = "false" });
                }
            }
                
        }

        [HttpPost]
        public JsonResult getTransdata(string TransId="")
        {
            try {
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
                                       REQUEST_DATE =x.REQUEST_DATE,
                                       AMOUNT = x.AMOUNT,
                                       BANK_ACCOUNT = x.BANK_ACCOUNT,
                                       TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                       SLN = x.SLN
                                   }).AsEnumerable().Select(z => new TBL_BALANCE_TRANSFER_LOGS
                                   {
                                       ToUser = z.Touser,
                                       FromUser = z.FromUser,
                                       AMOUNT = z.AMOUNT,
                                       REQUEST_DATE = Convert.ToDateTime(z.REQUEST_DATE.ToString("yyyy-MM-dd")),
                                       BANK_ACCOUNT = z.BANK_ACCOUNT,
                                       TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                       SLN = z.SLN
                                   }).FirstOrDefault();

                return Json(new { Result = "true", data = listdetails });
            }
            catch (Exception ex)
            {
                 var listdetails=new  TBL_BALANCE_TRANSFER_LOGS();
                return Json(new { Result = "false", data = listdetails });
            }
           
        }
        [HttpPost]
        public JsonResult ChangeTransactionStatus(string trandate="",string TransationStatus="",string slnval="")
        {
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
               

                    long sln = long.Parse(slnval);
                    decimal addamount = 0;
                    decimal memb_bal = 0;
                    decimal trans_bal = 0;
                    decimal availableamt = 0;
                    decimal addavilamt = 0;
                    decimal closingamt = 0;
                    decimal addclosingamt = 0;
                    var transinfo = db.TBL_BALANCE_TRANSFER_LOGS.Where(x => x.SLN == sln).FirstOrDefault();
                    var memberlist = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == transinfo.FROM_MEMBER).FirstOrDefault();

                 

                    var membtype = (from mm in db.TBL_MASTER_MEMBER
                                    join
                                        rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                    where mm.MEM_ID == transinfo.FROM_MEMBER
                                    select new
                                    {
                                        RoleId = mm.MEMBER_ROLE,
                                        roleName = rm.ROLE_NAME
                                    }).FirstOrDefault();
                    //transinfo.REQUEST_DATE = Convert.ToDateTime(trandate);
                    transinfo.STATUS = "Approve";
                    transinfo.APPROVAL_DATE = DateTime.Now;
                    transinfo.APPROVAL_TIME = DateTime.Now;
                    transinfo.APPROVED_BY = "Power Admin";
                    db.Entry(transinfo).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    memb_bal = decimal.Parse(memberlist.BALANCE.ToString());
                    trans_bal = decimal.Parse(transinfo.AMOUNT.ToString());
                    addamount = memb_bal + trans_bal;
                    memberlist.BALANCE = addamount;
                    //db.Entry(memberlist).State = System.Data.Entity.EntityState.Modified;
                    //db.SaveChanges();
                    var amtobj = db.TBL_ACCOUNTS.Where(x => x.MEM_ID == transinfo.FROM_MEMBER).FirstOrDefault();
                    if (amtobj != null)
                    {
                        availableamt = amtobj.AMOUNT;
                        addavilamt = availableamt + trans_bal;
                        amtobj.AMOUNT = addavilamt;
                        closingamt = amtobj.CLOSING;
                        addclosingamt = closingamt + trans_bal;
                        amtobj.CLOSING = addclosingamt;
                        amtobj.OPENING = trans_bal;
                        db.Entry(amtobj).State= System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        TBL_ACCOUNTS objacnt = new TBL_ACCOUNTS()
                        {
                            API_ID = 0,
                            MEM_ID = transinfo.FROM_MEMBER,
                            MEMBER_TYPE = membtype.roleName,
                            TRANSACTION_TYPE = transinfo.PAYMENT_METHOD,
                            TRANSACTION_DATE = Convert.ToDateTime(transinfo.REQUEST_DATE),
                            TRANSACTION_TIME = DateTime.Now,
                            DR_CR = "CR",
                            AMOUNT = transinfo.AMOUNT,
                            NARRATION = transinfo.TRANSACTION_DETAILS,
                            OPENING = 0,
                            CLOSING = addamount,
                            REC_NO = 0,
                            COMM_AMT=0,
                            GST = 0,
                            TDS = 0
                        };
                        db.TBL_ACCOUNTS.Add(objacnt);
                        db.SaveChanges();
                    }
                    db.Entry(memberlist).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    ContextTransaction.Commit();
                    return Json(new { Result = "true" });
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    return Json(new { Result = "false " });
                }
            }
                
            
        }

        [HttpPost]
        public JsonResult TransactionDecline(string trandate = "", string TransationStatus = "", string slnval = "")
        {
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    //var db = new DBContext();
                    long sln = long.Parse(slnval);
                    var transinfo = db.TBL_BALANCE_TRANSFER_LOGS.Where(x => x.SLN == sln).FirstOrDefault();
                    //transinfo.REQUEST_DATE = Convert.ToDateTime(trandate);
                    transinfo.STATUS = "Decline";
                    transinfo.APPROVAL_DATE = DateTime.Now;
                    transinfo.APPROVAL_TIME = DateTime.Now;
                    transinfo.APPROVED_BY = "Power Admin";
                    db.Entry(transinfo).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    ContextTransaction.Commit();
                    return Json(new { Result = "true" });
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    return Json(new { Result = "false " });
                }
            }
                

        }

        public ActionResult RequisitionReport()
        {
            //try
            //{
            //    if (Datefrom != "")
            //    {
            //        DateTime datefrom = DateTime.Parse(Datefrom, System.Globalization.CultureInfo.GetCultureInfo("en-Us").DateTimeFormat);
            //        ViewBag.datefrom = datefrom.ToString("MM/dd/yyyy");
            //    }
            //    if (dateto != "")
            //    {
            //        DateTime DateTO = DateTime.Parse(dateto, System.Globalization.CultureInfo.GetCultureInfo("en-Us").DateTimeFormat);
            //        ViewBag.DateTo = DateTO.ToString("MM/dd/yyyy"); ;
            //    }
            //    var db = new DBContext();
            //    if (Datefrom != "" && dateto != null)
            //    {
            //        DateTime datefrom = DateTime.Parse(Datefrom, System.Globalization.CultureInfo.GetCultureInfo("en-Us").DateTimeFormat);
            //        DateTime DateTO = DateTime.Parse(dateto, System.Globalization.CultureInfo.GetCultureInfo("en-Us").DateTimeFormat);
            //        var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
            //                               join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
            //                               where x.REQUEST_DATE >= datefrom && x.REQUEST_DATE <= DateTO
            //                               select new
            //                               {
            //                                   Touser = "PowerAdmin",
            //                                   FromUser = y.UName,
            //                                   REQUEST_DATE = x.REQUEST_DATE,
            //                                   AMOUNT = x.AMOUNT,
            //                                   BANK_ACCOUNT = x.BANK_ACCOUNT,
            //                                   TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
            //                                   STATUS = x.STATUS,
            //                                   APPROVED_BY = x.APPROVED_BY,
            //                                   APPROVAL_DATE = x.APPROVAL_DATE,
            //                                   SLN = x.SLN
            //                               }).AsEnumerable().Select(z => new TBL_BALANCE_TRANSFER_LOGS
            //                               {
            //                                   ToUser = z.Touser,
            //                                   FromUser = z.FromUser,
            //                                   AMOUNT = z.AMOUNT,
            //                                   REQUEST_DATE = z.REQUEST_DATE,
            //                                   BANK_ACCOUNT = z.BANK_ACCOUNT,
            //                                   TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
            //                                   STATUS = z.STATUS,
            //                                   APPROVED_BY = z.APPROVED_BY,
            //                                   APPROVAL_DATE = z.APPROVAL_DATE,
            //                                   SLN = z.SLN
            //                               }).ToList();
            //        //return PartialView("IndexGrid", transactionlist);


            //        //var transactiondetails = db.TBL_BALANCE_TRANSFER_LOGS.ToList();
            //        return View(transactionlist);
            //    }
            //    else
            //    {
            //        ViewBag.datefrom = "";
            //        ViewBag.DateTo = "";
            //        var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
            //                               join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
            //                               select new
            //                               {
            //                                   Touser = "PowerAdmin",
            //                                   FromUser = y.UName,
            //                                   REQUEST_DATE = x.REQUEST_DATE,
            //                                   AMOUNT = x.AMOUNT,
            //                                   BANK_ACCOUNT = x.BANK_ACCOUNT,
            //                                   TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
            //                                   STATUS = x.STATUS,
            //                                   APPROVED_BY = x.APPROVED_BY,
            //                                   APPROVAL_DATE = x.APPROVAL_DATE,
            //                                   SLN = x.SLN
            //                               }).AsEnumerable().Select(z => new TBL_BALANCE_TRANSFER_LOGS
            //                               {
            //                                   ToUser = z.Touser,
            //                                   FromUser = z.FromUser,
            //                                   AMOUNT = z.AMOUNT,
            //                                   REQUEST_DATE = z.REQUEST_DATE,
            //                                   BANK_ACCOUNT = z.BANK_ACCOUNT,
            //                                   TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
            //                                   STATUS = z.STATUS,
            //                                   APPROVED_BY = z.APPROVED_BY,
            //                                   APPROVAL_DATE = z.APPROVAL_DATE,
            //                                   SLN = z.SLN
            //                               }).ToList();
            //        //return PartialView("IndexGrid", transactionlist);


            //        //var transactiondetails = db.TBL_BALANCE_TRANSFER_LOGS.ToList();
            //        return View(transactionlist);
            //    }

            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            return View();
            
        }

        [HttpPost]
        public JsonResult DateWiseSearchRequisition(string Datefrom="" , string dateto="" )
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
                return Json(new { Result = "false"});
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
                                   join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                   select new
                                   {
                                       Touser = "PowerAdmin",
                                       transid=x.TransactionID,
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
                                       TransactionID=z.transid,
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
            grid.Columns.Add(model => model.REQUEST_DATE).Titled("Req Date").Formatted("{0:d}").MultiFilterable(true); 
            grid.Columns.Add(model => model.AMOUNT).Titled("Amount");
            grid.Columns.Add(model => model.BANK_ACCOUNT).Titled("Bank Acnt");
            grid.Columns.Add(model => model.TRANSACTION_DETAILS).Titled("Pay Method");
            grid.Columns.Add(model => model.TRANSACTION_DETAILS).Titled("Pay Method");
            grid.Columns.Add(model => model.STATUS).Titled("STATUS");
            grid.Columns.Add(model => model.APPROVAL_DATE).Titled("Apprv/Decline Date").Formatted("{0:d}").MultiFilterable(true); 
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

        // Autopopulate textbox

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
                               where (p.UName).Contains(query) && p.UNDER_WHITE_LEVEL==0
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
        

    }
}