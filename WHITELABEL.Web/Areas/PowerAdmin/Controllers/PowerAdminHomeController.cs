using NonFactors.Mvc.Grid;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Models;

namespace WHITELABEL.Web.Areas.PowerAdmin.Controllers
{
    [Authorize]
    public class PowerAdminHomeController : PoweradminbaseController
    {
        public void initpage()
        {
            try
            {
                ViewBag.ControllerName = "Power Admin Home";
                SystemClass sclass = new SystemClass();
                string userID = sclass.GetLoggedUser();
                long userid = long.Parse(userID);
                var dbmain = new DBContext();
                if (userID != null && userID != "")
                {
                    TBL_AUTH_ADMIN_USER currUser = dbmain.TBL_AUTH_ADMIN_USERS.SingleOrDefault(c => c.USER_ID == userid && c.ACTIVE_USER == true);
                    if (currUser != null)
                    {
                        Session["PowerAdminUserId"] = currUser.USER_ID;
                    }
                }
                if (Session["PowerAdminUserId"] == null)
                {
                    Response.Redirect(Url.Action("Index", "PowerAdminLogin", new { area = "PowerAdmin" }));
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
        // GET: PowerAdmin/PowerAdminHome
       
        public ActionResult Index()
        {
            if (Session["PowerAdminUserId"] != null)
            {
                try
                {
                    initpage();
                    
                    var db = new DBContext();
                    var Memblistdb = db.TBL_MASTER_MEMBER.Where(x => x.UNDER_WHITE_LEVEL == 0).Take(7).ToList().OrderByDescending(x => x.JOINING_DATE);
                    ViewBag.whitelevel = Memblistdb;
                    var requisitionlist = (from acnt in db.TBL_BALANCE_TRANSFER_LOGS
                                           join mem in db.TBL_MASTER_MEMBER on acnt.FROM_MEMBER equals mem.MEM_ID
                                           where acnt.TO_MEMBER == 0
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
                    var Bankdetails = db.TBL_SETTINGS_BANK_DETAILS.Where(x => x.MEM_ID == 0).ToList();
                    ViewBag.BankDetailsList = Bankdetails;
                    return View();
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Exception", "ErrorHandler", new { area = "" });
                    throw ex;
                }
            }
            else
            {
                Session["PowerAdminUserId"] = null;
                Session["PowerAdminUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("PowerAdminUserId");
                Session.Remove("PowerAdminUserName");
                Session.Remove("UserType");
                Response.Redirect(Url.Action("Index", "PowerAdminLogin", new { area = "PowerAdmin" }));
                return View();
            }

        }

        public PartialViewResult MemberGrid()
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
        private IGrid<TBL_MASTER_MEMBER> ExporMembertableGrid()
        {
            var dbcontext = new DBContext();
            var memberinfo = dbcontext.TBL_MASTER_MEMBER.Where(x=>x.UNDER_WHITE_LEVEL==0 && x.INTRODUCER== 1).ToList().OrderByDescending(x => x.JOINING_DATE);
            IGrid<TBL_MASTER_MEMBER> grid = new Grid<TBL_MASTER_MEMBER>(memberinfo);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.UName).Titled("User Name").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.COMPANY).Titled("Company").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.MEMBER_MOBILE).Titled("Mobile").Filterable(true).Sortable(true);          
            grid.Pager = new GridPager<TBL_MASTER_MEMBER>(grid);
            grid.Processors.Add(grid.Pager);
            grid.Pager.RowsPerPage =5;

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
                                   where x.STATUS == "Pending" && x.TO_MEMBER == 0
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
            grid.Columns.Add(model => model.TransactionID).Titled("Trans Id");
            grid.Columns.Add(model => model.FromUser).Titled("User");
            grid.Columns.Add(model => model.REQUEST_DATE).Titled("Req Date").Formatted("{0:d}").MultiFilterable(true);
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
            var bankdetails = db.TBL_SETTINGS_BANK_DETAILS.Where(x => x.MEM_ID == 0).ToList().OrderByDescending(x=>x.CREATED_DATE);

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


        public ActionResult Exportdata()
        {
            return View(CreateExportableGrid());
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
                IGrid<TBL_ACCOUNTS> grid = CreateExportableGrid();
                ExcelWorksheet sheet = package.Workbook.Worksheets["Data"];

                foreach (IGridColumn column in grid.Columns)
                {
                    sheet.Cells[1, col].Value = column.Title;
                    sheet.Column(col++).Width = 18;
                }

                foreach (IGridRow<TBL_ACCOUNTS> gridRow in grid.Rows)
                {
                    col = 1;
                    foreach (IGridColumn column in grid.Columns)
                        sheet.Cells[row, col++].Value = column.ValueFor(gridRow);

                    row++;
                }
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                //return File(package.GetAsByteArray(), "application/unknown");
            }
        }

        private IGrid<TBL_ACCOUNTS> CreateExportableGrid()
        {
            var db = new DBContext();

            var list = db.TBL_ACCOUNTS.Where(x=>x.MEM_ID== 100000).ToList();
            IGrid<TBL_ACCOUNTS> grid = new Grid<TBL_ACCOUNTS>(list);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.MEM_ID).Titled("Mem Id");
            grid.Columns.Add(model => model.MEMBER_TYPE).Titled("Type");
            grid.Columns.Add(model => model.DR_CR).Titled("DR_CR");
            grid.Columns.Add(model => model.OPENING).Titled("Opening Amt");
            grid.Columns.Add(model => model.CLOSING).Titled("Closing");
            grid.Columns.Add(model => model.TRANSACTION_DATE).Titled("Date");
            grid.Columns.Add(model => model.AMOUNT).Titled("Amount");
            grid.Columns.Add(model => model.COMM_AMT).Titled("Commission");
            grid.Pager = new GridPager<TBL_ACCOUNTS>(grid);
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
}