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
using WHITELABEL.Web.Areas.PowerAdmin.Models;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;

namespace WHITELABEL.Web.Areas.PowerAdmin.Controllers
{
    [Authorize]
    public class PowerAdminTransactionReportController : PoweradminbaseController
    {
        // GET: PowerAdmin/PowerAdminTransactionReport
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
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
                        // Session["UserName"] = currUser.UserName;
                    }
                }
                if (Session["PowerAdminUserId"] == null)
                {
                    //Response.Redirect("~/Login/LogOut");
                    Response.Redirect(Url.Action("Index", "PowerAdminLogin", new { area = "PowerAdmin" }));
                    return;
                }
                bool Islogin = false;
                if (Session["PowerAdminUserId"] != null)
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
        #region White Level Commission report
        public ActionResult Index()
        {
            if (Session["PowerAdminUserId"] != null)
            {
                initpage();
                var db = new DBContext();
                var memberService = (from x in db.TBL_MASTER_MEMBER
                                     join y in db.TBL_WHITE_LEVEL_HOSTING_DETAILS on x.MEM_ID equals y.MEM_ID
                                     where x.UNDER_WHITE_LEVEL == 0
                                     select new
                                     {
                                         MEM_ID = x.MEM_ID,
                                         UName = y.DOMAIN
                                     }).AsEnumerable().Select(z => new MemberView
                                     {
                                         IDValue = z.MEM_ID.ToString(),
                                         TextValue = z.UName
                                     }).ToList().Distinct();
                ViewBag.MemberService = new SelectList(memberService, "IDValue", "TextValue");
                return View();
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
        public PartialViewResult IndexGrid(string search, string status = "")
        {
            if (string.IsNullOrEmpty(search) && string.IsNullOrEmpty(status))
            {
                var listCommission = PowerAdminCommissionTransactionModel.GetAllPowerAdminCommissionReport("0", status);
                return PartialView("IndexGrid", listCommission);
            }
            else if (string.IsNullOrEmpty(search) && !string.IsNullOrEmpty(status))
            {
                var listCommission = PowerAdminCommissionTransactionModel.GetAllPowerAdminCommissionReport("0", status);
                return PartialView("IndexGrid", listCommission);
            }  //
            else if (!string.IsNullOrEmpty(search) && string.IsNullOrEmpty(status))
            {
                var listCommission = PowerAdminCommissionTransactionModel.GetPowerAdminCommissionReport(search.ToString(), status);
                return PartialView("IndexGrid", listCommission);
            }
            else if (!string.IsNullOrEmpty(search) && !string.IsNullOrEmpty(status))
            {
                var listCommission = PowerAdminCommissionTransactionModel.GetPowerAdminCommissionReport(search.ToString(), status);
                return PartialView("IndexGrid", listCommission);
            }
            return PartialView();
        }
        [HttpGet]
        public FileResult ExportIndexWhiteLevelAdminReport(string Disid, string statusval)
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;
                var db = new DBContext();


                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_ACCOUNTS> grid = CreateExportWlTableGrid(Disid, statusval);
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

                //return File(package.GetAsByteArray(), "application/unknown");
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }
        private IGrid<TBL_ACCOUNTS> CreateExportWlTableGrid(string Disid, string statusval)
        {
            var db = new DBContext();
            var MerchantList = new List<TBL_ACCOUNTS>();
            if (Disid == "" && statusval == "")
            {
                //long mem_id = long.Parse(Disid.ToString());
                MerchantList = PowerAdminCommissionTransactionModel.GetAllPowerAdminCommissionReport("0", statusval);               
            }
            else if (Disid == "" && statusval != "")
            {
                //long mem_id = long.Parse(Disid.ToString());
                MerchantList = PowerAdminCommissionTransactionModel.GetAllPowerAdminCommissionReport("0", statusval);                
            }
            else if (Disid != "" && statusval == "")
            {
                long mem_id = long.Parse(Disid.ToString());
                MerchantList = PowerAdminCommissionTransactionModel.GetPowerAdminCommissionReport(Disid.ToString(), statusval);
            }
            else if (Disid != "" && statusval != "")
            {
                long mem_id = long.Parse(Disid.ToString());
                MerchantList = PowerAdminCommissionTransactionModel.GetPowerAdminCommissionReport(Disid.ToString(), statusval);
            }
            IGrid<TBL_ACCOUNTS> grid = new Grid<TBL_ACCOUNTS>(MerchantList);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.SerialNo).Titled("Sln No.");
            grid.Columns.Add(model => model.UserName).Titled("User Name");
            //grid.Columns.Add(model => model.MEMBER_TYPE).Titled("Merchant Type");
            grid.Columns.Add(model => model.TRANSACTION_TYPE).Titled("Transaction Type");
            grid.Columns.Add(model => model.TRANSACTION_DATE).Titled("Transaction Date").Formatted("{0:d}");
            grid.Columns.Add(model => model.DR_CR).Titled("DR/CR");
            grid.Columns.Add(model => model.AMOUNT).Titled("Amount");
            //grid.Columns.Add(model => model.NARRATION).Titled("Narration");
            grid.Columns.Add(model => model.OPENING).Titled("Opening");
            grid.Columns.Add(model => model.CLOSING).Titled("Closing");
            grid.Columns.Add(model => model.COMM_AMT).Titled("Commission Amt.");
            grid.Pager = new GridPager<TBL_ACCOUNTS>(grid);
            grid.Processors.Add(grid.Pager);
            //grid.Pager.RowsPerPage = 6;

            foreach (IGridColumn column in grid.Columns)
            {
                column.Filter.IsEnabled = true;
                column.Sort.IsEnabled = true;
            }

            return grid;
            //string[] SuperMemId = null;
            //string[] DistributorMemId = null;
            //string[] MerchantMemId = null;
            //if (statusval == "")
            //{
            //    long mem_id = long.Parse(MemberCurrentUser.MEM_ID.ToString());
            //    var MerchantList = AdminTransactionViewModel.GetAdminCommissionReport(MemberCurrentUser.MEM_ID.ToString(), statusval);
            //    IGrid<TBL_ACCOUNTS> grid = new Grid<TBL_ACCOUNTS>(MerchantList);
            //    grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            //    grid.Query = Request.QueryString;

            //    grid.Columns.Add(model => model.SerialNo).Titled("Sln No.");
            //    grid.Columns.Add(model => model.UserName).Titled("User Name");
            //    //grid.Columns.Add(model => model.MEMBER_TYPE).Titled("Merchant Type");
            //    grid.Columns.Add(model => model.TRANSACTION_TYPE).Titled("Transaction Type");
            //    grid.Columns.Add(model => model.TRANSACTION_DATE).Titled("Transaction Date").Formatted("{0:d}");
            //    grid.Columns.Add(model => model.DR_CR).Titled("DR/CR");
            //    grid.Columns.Add(model => model.AMOUNT).Titled("Amount");
            //    //grid.Columns.Add(model => model.NARRATION).Titled("Narration");
            //    grid.Columns.Add(model => model.OPENING).Titled("Opening");
            //    grid.Columns.Add(model => model.CLOSING).Titled("Closing");
            //    grid.Columns.Add(model => model.COMM_AMT).Titled("Commission Amt.");
            //    grid.Pager = new GridPager<TBL_ACCOUNTS>(grid);
            //    grid.Processors.Add(grid.Pager);
            //    //grid.Pager.RowsPerPage = 6;

            //    foreach (IGridColumn column in grid.Columns)
            //    {
            //        column.Filter.IsEnabled = true;
            //        column.Sort.IsEnabled = true;
            //    }

            //    return grid;
            //}
            //else if (statusval != "")
            //{
            //    long mem_id = long.Parse(MemberCurrentUser.MEM_ID.ToString());
            //    var MerchantList = AdminTransactionViewModel.GetAdminCommissionReport(MemberCurrentUser.MEM_ID.ToString(), statusval);

            //    IGrid<TBL_ACCOUNTS> grid = new Grid<TBL_ACCOUNTS>(MerchantList);
            //    grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            //    grid.Query = Request.QueryString;

            //    grid.Columns.Add(model => model.SerialNo).Titled("Sln No.");
            //    grid.Columns.Add(model => model.UserName).Titled("User Name");
            //    //grid.Columns.Add(model => model.MEMBER_TYPE).Titled("Merchant Type");
            //    grid.Columns.Add(model => model.TRANSACTION_TYPE).Titled("Transaction Type");
            //    grid.Columns.Add(model => model.TRANSACTION_DATE).Titled("Transaction Date").Formatted("{0:d}");
            //    grid.Columns.Add(model => model.DR_CR).Titled("DR/CR");
            //    grid.Columns.Add(model => model.AMOUNT).Titled("Amount");
            //    //grid.Columns.Add(model => model.NARRATION).Titled("Narration");
            //    grid.Columns.Add(model => model.OPENING).Titled("Opening");
            //    grid.Columns.Add(model => model.CLOSING).Titled("Closing");
            //    grid.Columns.Add(model => model.COMM_AMT).Titled("Commission Amt.");
            //    grid.Pager = new GridPager<TBL_ACCOUNTS>(grid);
            //    grid.Processors.Add(grid.Pager);
            //    //grid.Pager.RowsPerPage = 6;

            //    foreach (IGridColumn column in grid.Columns)
            //    {
            //        column.Filter.IsEnabled = true;
            //        column.Sort.IsEnabled = true;
            //    }

            //    return grid;
            //}
            //else
            //{
            //    long mem_id = long.Parse(MemberCurrentUser.MEM_ID.ToString());
            //    var MerchantList = AdminTransactionViewModel.GetAdminCommissionReport(MemberCurrentUser.MEM_ID.ToString(), statusval);
            //    IGrid<TBL_ACCOUNTS> grid = new Grid<TBL_ACCOUNTS>(MerchantList);
            //    grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            //    grid.Query = Request.QueryString;

            //    grid.Columns.Add(model => model.SerialNo).Titled("Sln No.");
            //    grid.Columns.Add(model => model.UserName).Titled("User Name");
            //    //grid.Columns.Add(model => model.MEMBER_TYPE).Titled("Merchant Type");
            //    grid.Columns.Add(model => model.TRANSACTION_TYPE).Titled("Transaction Type");
            //    grid.Columns.Add(model => model.TRANSACTION_DATE).Titled("Transaction Date").Formatted("{0:d}");
            //    grid.Columns.Add(model => model.DR_CR).Titled("DR/CR");
            //    grid.Columns.Add(model => model.AMOUNT).Titled("Amount");
            //    //grid.Columns.Add(model => model.NARRATION).Titled("Narration");
            //    grid.Columns.Add(model => model.OPENING).Titled("Opening");
            //    grid.Columns.Add(model => model.CLOSING).Titled("Closing");
            //    grid.Columns.Add(model => model.COMM_AMT).Titled("Commission Amt.");
            //    grid.Pager = new GridPager<TBL_ACCOUNTS>(grid);
            //    grid.Processors.Add(grid.Pager);
            //    //grid.Pager.RowsPerPage = 6;

            //    foreach (IGridColumn column in grid.Columns)
            //    {
            //        column.Filter.IsEnabled = true;
            //        column.Sort.IsEnabled = true;
            //    }

            //    return grid;
            //}
        }
        [HttpPost]
        public JsonResult GetSuperDistributor(long Disid)
        {
            //string countrystring = "select * from tbl_state where countrycode='" + id + "'";
            long dis_Id = long.Parse(Disid.ToString());
            var db = new DBContext();
            var memberService = (from x in db.TBL_MASTER_MEMBER
                                 where x.INTRODUCER == dis_Id
                                 select new
                                 {
                                     MEM_ID = x.MEM_ID,
                                     UName = x.UName
                                 }).AsEnumerable().Select(z => new MemberView
                                 {
                                     IDValue = z.MEM_ID.ToString(),
                                     TextValue = z.UName
                                 }).ToList().Distinct();
            return Json(memberService, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetDistributor(long Disid)
        {
            //string countrystring = "select * from tbl_state where countrycode='" + id + "'";
            long dis_Id = long.Parse(Disid.ToString());
            var db = new DBContext();
            var memberService = (from x in db.TBL_MASTER_MEMBER
                                 where x.INTRODUCER == dis_Id
                                 select new
                                 {
                                     MEM_ID = x.MEM_ID,
                                     UName = x.UName
                                 }).AsEnumerable().Select(z => new MemberView
                                 {
                                     IDValue = z.MEM_ID.ToString(),
                                     TextValue = z.UName
                                 }).ToList().Distinct();
            return Json(memberService, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetMerchant(long Disid)
        {
            //string countrystring = "select * from tbl_state where countrycode='" + id + "'";
            long dis_Id = long.Parse(Disid.ToString());
            var db = new DBContext();
            var memberService = (from x in db.TBL_MASTER_MEMBER
                                 where x.INTRODUCER == dis_Id
                                 select new
                                 {
                                     MEM_ID = x.MEM_ID,
                                     UName = x.UName
                                 }).AsEnumerable().Select(z => new MemberView
                                 {
                                     IDValue = z.MEM_ID.ToString(),
                                     TextValue = z.UName
                                 }).ToList().Distinct();
            return Json(memberService, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Super Commission report
        public ActionResult PowerAdminSuperCommissionReport()
        {
            if (Session["PowerAdminUserId"] != null)
            {
                initpage();
                var db = new DBContext();
                var memberService = (from x in db.TBL_MASTER_MEMBER
                                     join y in db.TBL_WHITE_LEVEL_HOSTING_DETAILS on x.MEM_ID equals y.MEM_ID
                                     where x.UNDER_WHITE_LEVEL == 0
                                     select new
                                     {
                                         MEM_ID = x.MEM_ID,
                                         UName = y.DOMAIN
                                     }).AsEnumerable().Select(z => new MemberView
                                     {
                                         IDValue = z.MEM_ID.ToString(),
                                         TextValue = z.UName
                                     }).ToList().Distinct();
                ViewBag.MemberService = new SelectList(memberService, "IDValue", "TextValue");
                return View();
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
        public PartialViewResult PowerAdminSuperCommissionGrid(string whitelevel,string search, string status = "")
        {
            if (string.IsNullOrEmpty(whitelevel) && string.IsNullOrEmpty(search) && string.IsNullOrEmpty(status))
            {
                var listCommission = PowerAdminCommissionTransactionModel.GetAllPoweradminSuperCommissionReport("0", status);
                return PartialView("IndexGrid", listCommission);
            }
            else if (string.IsNullOrEmpty(whitelevel) && string.IsNullOrEmpty(search) && !string.IsNullOrEmpty(status))
            {
                var listCommission = PowerAdminCommissionTransactionModel.GetAllPoweradminSuperCommissionReport("0", status);
                return PartialView("IndexGrid", listCommission);
            }  //
            else if (!string.IsNullOrEmpty(whitelevel) && !string.IsNullOrEmpty(search) && string.IsNullOrEmpty(status))
            {
                var listCommission = PowerAdminCommissionTransactionModel.GetPoweradminSuperCommissionReport(whitelevel, search.ToString(), status);
                return PartialView("IndexGrid", listCommission);
            }
            else if (!string.IsNullOrEmpty(whitelevel) && !string.IsNullOrEmpty(search) && !string.IsNullOrEmpty(status))
            {
                var listCommission = PowerAdminCommissionTransactionModel.GetPoweradminSuperCommissionReport(whitelevel, search.ToString(), status);
                return PartialView("IndexGrid", listCommission);
            }
            else if (!string.IsNullOrEmpty(whitelevel) && string.IsNullOrEmpty(search) && string.IsNullOrEmpty(status))
            {
                var listCommission = PowerAdminCommissionTransactionModel.GetPoweradminSuperCommissionReport(whitelevel,search.ToString(), status);
                return PartialView("IndexGrid", listCommission);
            }
            else if (!string.IsNullOrEmpty(whitelevel) && string.IsNullOrEmpty(search) && !string.IsNullOrEmpty(status))
            {
                var listCommission = PowerAdminCommissionTransactionModel.GetPoweradminSuperCommissionReport(whitelevel, search.ToString(), status);
                return PartialView("IndexGrid", listCommission);
            }
            return PartialView();
        }
        [HttpGet]
        public FileResult ExportIndexPASuperAdminReport(string Whitelevel, string Disid, string statusval)
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;
                var db = new DBContext();
                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_ACCOUNTS> grid = CreateExportPASuperTableGrid(Whitelevel, Disid, statusval);
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

                //return File(package.GetAsByteArray(), "application/unknown");
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }
        private IGrid<TBL_ACCOUNTS> CreateExportPASuperTableGrid(string whitelevel, string Disid, string statusval)
        {
            var db = new DBContext();
            var MerchantList = new List<TBL_ACCOUNTS>();

            if (string.IsNullOrEmpty(whitelevel) && string.IsNullOrEmpty(Disid) && string.IsNullOrEmpty(statusval))
            {
                MerchantList = PowerAdminCommissionTransactionModel.GetAllPoweradminSuperCommissionReport("0", statusval);                
            }
            else if (string.IsNullOrEmpty(whitelevel) && string.IsNullOrEmpty(Disid) && !string.IsNullOrEmpty(statusval))
            {
                MerchantList = PowerAdminCommissionTransactionModel.GetAllPoweradminSuperCommissionReport("0", statusval);                
            }  //
            else if (!string.IsNullOrEmpty(whitelevel) && !string.IsNullOrEmpty(Disid) && string.IsNullOrEmpty(statusval))
            {
                MerchantList = PowerAdminCommissionTransactionModel.GetPoweradminSuperCommissionReport(whitelevel, Disid.ToString(), statusval);                
            }
            else if (!string.IsNullOrEmpty(whitelevel) && !string.IsNullOrEmpty(statusval) && !string.IsNullOrEmpty(statusval))
            {
                MerchantList = PowerAdminCommissionTransactionModel.GetPoweradminSuperCommissionReport(whitelevel, Disid.ToString(), statusval);                
            }
            else if (!string.IsNullOrEmpty(whitelevel) && string.IsNullOrEmpty(Disid) && string.IsNullOrEmpty(statusval))
            {
                MerchantList = PowerAdminCommissionTransactionModel.GetPoweradminSuperCommissionReport(whitelevel, Disid.ToString(), statusval);                
            }
            else if (!string.IsNullOrEmpty(whitelevel) && string.IsNullOrEmpty(Disid) && !string.IsNullOrEmpty(statusval))
            {
                MerchantList = PowerAdminCommissionTransactionModel.GetPoweradminSuperCommissionReport(whitelevel, Disid.ToString(), statusval);                
            }


            //if (Disid == "" && statusval == "")
            //{
            //    //long mem_id = long.Parse(Disid.ToString());
            //    MerchantList = PowerAdminCommissionTransactionModel.GetAllPoweradminSuperCommissionReport("0", statusval);
            //}
            //else if (Disid == "" && statusval != "")
            //{
            //    //long mem_id = long.Parse(Disid.ToString());
            //    MerchantList = PowerAdminCommissionTransactionModel.GetAllPoweradminSuperCommissionReport("0", statusval);
            //}
            //else if (Disid != "" && statusval == "")
            //{
            //    //long mem_id = long.Parse(Disid.ToString());
            //    MerchantList = PowerAdminCommissionTransactionModel.GetPoweradminSuperCommissionReport(Disid.ToString(), statusval);
            //}
            //else if (Disid != "" && statusval != "")
            //{
            //    //long mem_id = long.Parse(Disid.ToString());
            //    MerchantList = PowerAdminCommissionTransactionModel.GetPoweradminSuperCommissionReport(Disid.ToString(), statusval);
            //}
            IGrid<TBL_ACCOUNTS> grid = new Grid<TBL_ACCOUNTS>(MerchantList);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.SerialNo).Titled("Sln No.");
            grid.Columns.Add(model => model.UserName).Titled("User Name");
            //grid.Columns.Add(model => model.MEMBER_TYPE).Titled("Merchant Type");
            grid.Columns.Add(model => model.TRANSACTION_TYPE).Titled("Transaction Type");
            grid.Columns.Add(model => model.TRANSACTION_DATE).Titled("Transaction date").Formatted("{0:d}");
            grid.Columns.Add(model => model.DR_CR).Titled("DR/CR");
            grid.Columns.Add(model => model.AMOUNT).Titled("Amount");
            //grid.Columns.Add(model => model.NARRATION).Titled("Narration");
            grid.Columns.Add(model => model.OPENING).Titled("Opening");
            grid.Columns.Add(model => model.CLOSING).Titled("Closing");
            grid.Columns.Add(model => model.COMM_AMT).Titled("Commission Amt.");
            grid.Pager = new GridPager<TBL_ACCOUNTS>(grid);
            grid.Processors.Add(grid.Pager);
            //grid.Pager.RowsPerPage = 6;

            foreach (IGridColumn column in grid.Columns)
            {
                column.Filter.IsEnabled = true;
                column.Sort.IsEnabled = true;
            }

            return grid;
            
        }
        #endregion

        #region Distributor Commission Report
        public ActionResult PowerAdminDistributorCommissionReport()
        {
            if (Session["PowerAdminUserId"] != null)
            {
                initpage();
                var db = new DBContext();
                var memberService = (from x in db.TBL_MASTER_MEMBER
                                     join y in db.TBL_WHITE_LEVEL_HOSTING_DETAILS on x.MEM_ID equals y.MEM_ID
                                     where x.UNDER_WHITE_LEVEL == 0
                                     select new
                                     {
                                         MEM_ID = x.MEM_ID,
                                         UName = y.DOMAIN
                                     }).AsEnumerable().Select(z => new MemberView
                                     {
                                         IDValue = z.MEM_ID.ToString(),
                                         TextValue = z.UName
                                     }).ToList().Distinct();
                ViewBag.MemberService = new SelectList(memberService, "IDValue", "TextValue");
                return View();
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
        public PartialViewResult PowerAdminDistributorCommissionGrid(string Whitelevel="", string Supervalue = "",string Distributor="", string status = "")
        {
            if (string.IsNullOrEmpty(Whitelevel) && string.IsNullOrEmpty(Supervalue) && string.IsNullOrEmpty(Distributor) && !string.IsNullOrEmpty(status))
            {
                var listCommission = PowerAdminCommissionTransactionModel.GetAllPoweradminDistributorCommissionReport(Whitelevel, Supervalue, "0", status);
                return PartialView("IndexGrid", listCommission);
            }
            else if (string.IsNullOrEmpty(Whitelevel) && string.IsNullOrEmpty(Supervalue) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(status))
            {
                var listCommission = PowerAdminCommissionTransactionModel.GetAllPoweradminDistributorCommissionReport(Whitelevel, Supervalue, "0", status);
                return PartialView("IndexGrid", listCommission);
            }
            else if (!string.IsNullOrEmpty(Whitelevel) && string.IsNullOrEmpty(Supervalue) && string.IsNullOrEmpty(Distributor) && !string.IsNullOrEmpty(status))
            {
                var listCommission = PowerAdminCommissionTransactionModel.GetPoweradminDistributorCommissionReport(Whitelevel, Supervalue, Distributor.ToString(), status);
                return PartialView("IndexGrid", listCommission);
            }
            else if (!string.IsNullOrEmpty(Whitelevel) && string.IsNullOrEmpty(Supervalue) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(status))
            {
                var listCommission = PowerAdminCommissionTransactionModel.GetPoweradminDistributorCommissionReport(Whitelevel, Supervalue, Distributor.ToString(), status);
                return PartialView("IndexGrid", listCommission);
            }

            else if (!string.IsNullOrEmpty(Whitelevel) && !string.IsNullOrEmpty(Supervalue) && string.IsNullOrEmpty(Distributor) && !string.IsNullOrEmpty(status))
            {
                var listCommission = PowerAdminCommissionTransactionModel.GetPoweradminDistributorCommissionReport(Whitelevel, Supervalue, Distributor.ToString(), status);
                return PartialView("IndexGrid", listCommission);
            }
            else if (!string.IsNullOrEmpty(Whitelevel) && !string.IsNullOrEmpty(Supervalue) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(status))
            {
                var listCommission = PowerAdminCommissionTransactionModel.GetPoweradminDistributorCommissionReport(Whitelevel, Supervalue, Distributor.ToString(), status);
                return PartialView("IndexGrid", listCommission);
            }
            else if (!string.IsNullOrEmpty(Whitelevel) && !string.IsNullOrEmpty(Supervalue) && !string.IsNullOrEmpty(Distributor) && !string.IsNullOrEmpty(status))
            {
                var listCommission = PowerAdminCommissionTransactionModel.GetPoweradminDistributorCommissionReport(Whitelevel, Supervalue, Distributor.ToString(), status);
                return PartialView("IndexGrid", listCommission);
            }
            else if (!string.IsNullOrEmpty(Whitelevel) && !string.IsNullOrEmpty(Supervalue) && !string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(status))
            {
                var listCommission = PowerAdminCommissionTransactionModel.GetPoweradminDistributorCommissionReport(Whitelevel, Supervalue, Distributor.ToString(), status);
                return PartialView("IndexGrid", listCommission);
            }


            //    if (string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(status))
            //{
            //    var listCommission = PowerAdminCommissionTransactionModel.GetAllPoweradminDistributorCommissionReport("0", status);
            //    return PartialView("IndexGrid", listCommission);
            //}
            //else if (string.IsNullOrEmpty(Distributor) && !string.IsNullOrEmpty(status))
            //{
            //    var listCommission = PowerAdminCommissionTransactionModel.GetAllPoweradminDistributorCommissionReport("0", status);
            //    return PartialView("IndexGrid", listCommission);
            //}  //
            //else if (!string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(status))
            //{
            //    var listCommission = PowerAdminCommissionTransactionModel.GetPoweradminDistributorCommissionReport(search.ToString(), status);
            //    return PartialView("IndexGrid", listCommission);
            //}
            //else if (!string.IsNullOrEmpty(Distributor) && !string.IsNullOrEmpty(status))
            //{
            //    var listCommission = PowerAdminCommissionTransactionModel.GetPoweradminDistributorCommissionReport(search.ToString(), status);
            //    return PartialView("IndexGrid", listCommission);
            //}
            return PartialView();
        }
        [HttpGet]
        public FileResult ExportIndexPADistributorAdminReport(string Whitelevel, string Supervalue, string Disid, string statusval)
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;
                var db = new DBContext();


                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_ACCOUNTS> grid = CreateExportPADistributorTableGrid(Whitelevel, Supervalue, Disid, statusval);
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

                //return File(package.GetAsByteArray(), "application/unknown");
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }
        private IGrid<TBL_ACCOUNTS> CreateExportPADistributorTableGrid(string Whitelevel,string Supervalue, string Disid, string statusval)
        {
            var db = new DBContext();
            var MerchantList = new List<TBL_ACCOUNTS>();

            if (string.IsNullOrEmpty(Whitelevel) && string.IsNullOrEmpty(Supervalue) && string.IsNullOrEmpty(Disid) && !string.IsNullOrEmpty(statusval))
            {
                MerchantList = PowerAdminCommissionTransactionModel.GetAllPoweradminDistributorCommissionReport(Whitelevel, Supervalue, "0", statusval);
                
            }
            else if (string.IsNullOrEmpty(Whitelevel) && string.IsNullOrEmpty(Supervalue) && string.IsNullOrEmpty(Disid) && string.IsNullOrEmpty(statusval))
            {
                MerchantList = PowerAdminCommissionTransactionModel.GetAllPoweradminDistributorCommissionReport(Whitelevel, Supervalue, "0", statusval);
                
            }
            else if (!string.IsNullOrEmpty(Whitelevel) && string.IsNullOrEmpty(Supervalue) && string.IsNullOrEmpty(Disid) && !string.IsNullOrEmpty(statusval))
            {
                MerchantList = PowerAdminCommissionTransactionModel.GetPoweradminDistributorCommissionReport(Whitelevel, Supervalue, Disid.ToString(), statusval);
                
            }
            else if (!string.IsNullOrEmpty(Whitelevel) && string.IsNullOrEmpty(Supervalue) && string.IsNullOrEmpty(Disid) && string.IsNullOrEmpty(statusval))
            {
                MerchantList = PowerAdminCommissionTransactionModel.GetPoweradminDistributorCommissionReport(Whitelevel, Supervalue, Disid.ToString(), statusval);
                
            }

            else if (!string.IsNullOrEmpty(Whitelevel) && !string.IsNullOrEmpty(Supervalue) && string.IsNullOrEmpty(Disid) && !string.IsNullOrEmpty(statusval))
            {
                MerchantList = PowerAdminCommissionTransactionModel.GetPoweradminDistributorCommissionReport(Whitelevel, Supervalue, Disid.ToString(), statusval);
                
            }
            else if (!string.IsNullOrEmpty(Whitelevel) && !string.IsNullOrEmpty(Supervalue) && string.IsNullOrEmpty(Disid) && string.IsNullOrEmpty(statusval))
            {
                MerchantList = PowerAdminCommissionTransactionModel.GetPoweradminDistributorCommissionReport(Whitelevel, Supervalue, Disid.ToString(), statusval);
                
            }
            else if (!string.IsNullOrEmpty(Whitelevel) && !string.IsNullOrEmpty(Supervalue) && !string.IsNullOrEmpty(Disid) && !string.IsNullOrEmpty(statusval))
            {
                MerchantList = PowerAdminCommissionTransactionModel.GetPoweradminDistributorCommissionReport(Whitelevel, Supervalue, Disid.ToString(), statusval);
                
            }
            else if (!string.IsNullOrEmpty(Whitelevel) && !string.IsNullOrEmpty(Supervalue) && !string.IsNullOrEmpty(Disid) && string.IsNullOrEmpty(statusval))
            {
                MerchantList = PowerAdminCommissionTransactionModel.GetPoweradminDistributorCommissionReport(Whitelevel, Supervalue, Disid.ToString(), statusval);
                
            }



            //if (Disid == "" && statusval == "")
            //{
            //    //long mem_id = long.Parse(Disid.ToString());
            //    MerchantList = PowerAdminCommissionTransactionModel.GetAllPoweradminDistributorCommissionReport("0", statusval);
            //}
            //else if (Disid == "" && statusval != "")
            //{
            //    //long mem_id = long.Parse(Disid.ToString());
            //    MerchantList = PowerAdminCommissionTransactionModel.GetAllPoweradminDistributorCommissionReport("0", statusval);
            //}
            //else if (Disid != "" && statusval == "")
            //{
            //    //long mem_id = long.Parse(Disid.ToString());
            //    MerchantList = PowerAdminCommissionTransactionModel.GetPoweradminDistributorCommissionReport(Disid.ToString(), statusval);
            //}
            //else if (Disid != "" && statusval != "")
            //{
            //    //long mem_id = long.Parse(Disid.ToString());
            //    MerchantList = PowerAdminCommissionTransactionModel.GetPoweradminDistributorCommissionReport(Disid.ToString(), statusval);
            //}
            IGrid<TBL_ACCOUNTS> grid = new Grid<TBL_ACCOUNTS>(MerchantList);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.SerialNo).Titled("Sln No.");
            grid.Columns.Add(model => model.UserName).Titled("User Name");
            //grid.Columns.Add(model => model.MEMBER_TYPE).Titled("Merchant Type");
            grid.Columns.Add(model => model.TRANSACTION_TYPE).Titled("Transaction Type");
            grid.Columns.Add(model => model.TRANSACTION_DATE).Titled("Transaction Date").Formatted("{0:d}");
            grid.Columns.Add(model => model.DR_CR).Titled("DR/CR");
            grid.Columns.Add(model => model.AMOUNT).Titled("Amount");
            //grid.Columns.Add(model => model.NARRATION).Titled("Narration");
            grid.Columns.Add(model => model.OPENING).Titled("Opening");
            grid.Columns.Add(model => model.CLOSING).Titled("Closing");
            grid.Columns.Add(model => model.COMM_AMT).Titled("Commission Amt.");
            grid.Pager = new GridPager<TBL_ACCOUNTS>(grid);
            grid.Processors.Add(grid.Pager);
            //grid.Pager.RowsPerPage = 6;

            foreach (IGridColumn column in grid.Columns)
            {
                column.Filter.IsEnabled = true;
                column.Sort.IsEnabled = true;
            }

            return grid;

        }
        #endregion

        #region merchant Commission Report
        public ActionResult PowerAdminMerchantCommissionReport()
        {
            if (Session["PowerAdminUserId"] != null)
            {
                initpage();
                var db = new DBContext();
                var memberService = (from x in db.TBL_MASTER_MEMBER
                                     join y in db.TBL_WHITE_LEVEL_HOSTING_DETAILS on x.MEM_ID equals y.MEM_ID
                                     where x.UNDER_WHITE_LEVEL == 0
                                     select new
                                     {
                                         MEM_ID = x.MEM_ID,
                                         UName = y.DOMAIN
                                     }).AsEnumerable().Select(z => new MemberView
                                     {
                                         IDValue = z.MEM_ID.ToString(),
                                         TextValue = z.UName
                                     }).ToList().Distinct();
                ViewBag.MemberService = new SelectList(memberService, "IDValue", "TextValue");
                return View();
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
        public PartialViewResult PowerAdminMerchantCommissionGrid(string Whitelevel,string Supervalue, string Distributor, string Merchant, string status = "")
        {
            if (string.IsNullOrEmpty(Whitelevel) && string.IsNullOrEmpty(Supervalue) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(Merchant) && string.IsNullOrEmpty(status))
            {
                var listCommission = PowerAdminCommissionTransactionModel.GetPoweradminAllMerchantCommissionReport("0", status);
                return PartialView("PowerAdminMerchantCommissionGrid", listCommission);
            }
            else if (string.IsNullOrEmpty(Whitelevel) && string.IsNullOrEmpty(Supervalue) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(Merchant) && !string.IsNullOrEmpty(status))
            {
                var listCommission = PowerAdminCommissionTransactionModel.GetPoweradminAllMerchantCommissionReport("0", status);
                return PartialView("PowerAdminMerchantCommissionGrid", listCommission);
            }
            else if (!string.IsNullOrEmpty(Whitelevel) && string.IsNullOrEmpty(Supervalue) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(Merchant) && string.IsNullOrEmpty(status))
            {
                var listCommission = PowerAdminCommissionTransactionModel.GetPoweradminMerchantCommissionReport(Whitelevel, Supervalue, Distributor, Merchant.ToString(), status);
                return PartialView("PowerAdminMerchantCommissionGrid", listCommission);
            }
            else if (!string.IsNullOrEmpty(Whitelevel) && string.IsNullOrEmpty(Supervalue) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(Merchant) && !string.IsNullOrEmpty(status))
            {
                var listCommission = PowerAdminCommissionTransactionModel.GetPoweradminMerchantCommissionReport(Whitelevel, Supervalue, Distributor, Merchant.ToString(), status);
                return PartialView("PowerAdminMerchantCommissionGrid", listCommission);
            }
            else if (!string.IsNullOrEmpty(Whitelevel) && !string.IsNullOrEmpty(Supervalue) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(Merchant) && string.IsNullOrEmpty(status))
            {
                var listCommission = PowerAdminCommissionTransactionModel.GetPoweradminMerchantCommissionReport(Whitelevel, Supervalue, Distributor, Merchant.ToString(), status);
                return PartialView("PowerAdminMerchantCommissionGrid", listCommission);
            }
            else if (!string.IsNullOrEmpty(Whitelevel) && !string.IsNullOrEmpty(Supervalue) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(Merchant) && !string.IsNullOrEmpty(status))
            {
                var listCommission = PowerAdminCommissionTransactionModel.GetPoweradminMerchantCommissionReport(Whitelevel, Supervalue, Distributor, Merchant.ToString(), status);
                return PartialView("PowerAdminMerchantCommissionGrid", listCommission);
            }

            else if (!string.IsNullOrEmpty(Whitelevel) && !string.IsNullOrEmpty(Supervalue) && !string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(Merchant) && string.IsNullOrEmpty(status))
            {
                var listCommission = PowerAdminCommissionTransactionModel.GetPoweradminMerchantCommissionReport(Whitelevel, Supervalue, Distributor, Merchant.ToString(), status);
                return PartialView("PowerAdminMerchantCommissionGrid", listCommission);
            }
            else if (!string.IsNullOrEmpty(Whitelevel) && !string.IsNullOrEmpty(Supervalue) && !string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(Merchant) && !string.IsNullOrEmpty(status))
            {
                var listCommission = PowerAdminCommissionTransactionModel.GetPoweradminMerchantCommissionReport(Whitelevel, Supervalue, Distributor, Merchant.ToString(), status);
                return PartialView("PowerAdminMerchantCommissionGrid", listCommission);
            }

            else if (!string.IsNullOrEmpty(Whitelevel) && !string.IsNullOrEmpty(Supervalue) && !string.IsNullOrEmpty(Distributor) && !string.IsNullOrEmpty(Merchant) && string.IsNullOrEmpty(status))
            {
                var listCommission = PowerAdminCommissionTransactionModel.GetPoweradminMerchantCommissionReport(Whitelevel, Supervalue, Distributor, Merchant.ToString(), status);
                return PartialView("PowerAdminMerchantCommissionGrid", listCommission);
            }
            else if (!string.IsNullOrEmpty(Whitelevel) && !string.IsNullOrEmpty(Supervalue) && !string.IsNullOrEmpty(Distributor) && !string.IsNullOrEmpty(Merchant) && !string.IsNullOrEmpty(status))
            {
                var listCommission = PowerAdminCommissionTransactionModel.GetPoweradminMerchantCommissionReport(Whitelevel, Supervalue, Distributor, Merchant.ToString(), status);
                return PartialView("PowerAdminMerchantCommissionGrid", listCommission);
            }
            
            return PartialView();
        }
        [HttpGet]
        public FileResult ExportIndexPAMerchantAdminReport(string Whitelevel, string Supervalue, string Distributor, string Disid, string statusval)
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;
                var db = new DBContext();


                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_ACCOUNTS> grid = CreateExportPAMerchantTableGrid(Whitelevel, Supervalue, Distributor, Disid, statusval);
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

                //return File(package.GetAsByteArray(), "application/unknown");
                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }
        private IGrid<TBL_ACCOUNTS> CreateExportPAMerchantTableGrid(string Whitelevel, string Supervalue, string Distributor, string Disid, string statusval)
        {
            var db = new DBContext();
            var MerchantList = new List<TBL_ACCOUNTS>();

            if (string.IsNullOrEmpty(Whitelevel) && string.IsNullOrEmpty(Supervalue) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(Disid) && string.IsNullOrEmpty(statusval))
            {
                MerchantList = PowerAdminCommissionTransactionModel.GetPoweradminAllMerchantCommissionReport("0", statusval);
                
            }
            else if (string.IsNullOrEmpty(Whitelevel) && string.IsNullOrEmpty(Supervalue) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(Disid) && !string.IsNullOrEmpty(statusval))
            {
                MerchantList = PowerAdminCommissionTransactionModel.GetPoweradminAllMerchantCommissionReport("0", statusval);
                
            }
            else if (!string.IsNullOrEmpty(Whitelevel) && string.IsNullOrEmpty(Supervalue) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(Disid) && string.IsNullOrEmpty(statusval))
            {
                MerchantList = PowerAdminCommissionTransactionModel.GetPoweradminMerchantCommissionReport(Whitelevel, Supervalue, Distributor, Disid.ToString(), statusval);
                
            }
            else if (!string.IsNullOrEmpty(Whitelevel) && string.IsNullOrEmpty(Supervalue) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(Disid) && !string.IsNullOrEmpty(statusval))
            {
                MerchantList = PowerAdminCommissionTransactionModel.GetPoweradminMerchantCommissionReport(Whitelevel, Supervalue, Distributor, Disid.ToString(), statusval);
                
            }
            else if (!string.IsNullOrEmpty(Whitelevel) && !string.IsNullOrEmpty(Supervalue) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(Disid) && string.IsNullOrEmpty(statusval))
            {
                MerchantList = PowerAdminCommissionTransactionModel.GetPoweradminMerchantCommissionReport(Whitelevel, Supervalue, Distributor, Disid.ToString(), statusval);
                
            }
            else if (!string.IsNullOrEmpty(Whitelevel) && !string.IsNullOrEmpty(Supervalue) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(Disid) && !string.IsNullOrEmpty(statusval))
            {
                MerchantList = PowerAdminCommissionTransactionModel.GetPoweradminMerchantCommissionReport(Whitelevel, Supervalue, Distributor, Disid.ToString(), statusval);
            
            }

            else if (!string.IsNullOrEmpty(Whitelevel) && !string.IsNullOrEmpty(Supervalue) && !string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(Disid) && string.IsNullOrEmpty(statusval))
            {
                MerchantList = PowerAdminCommissionTransactionModel.GetPoweradminMerchantCommissionReport(Whitelevel, Supervalue, Distributor, Disid.ToString(), statusval);
            
            }
            else if (!string.IsNullOrEmpty(Whitelevel) && !string.IsNullOrEmpty(Supervalue) && !string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(Disid) && !string.IsNullOrEmpty(statusval))
            {
                MerchantList = PowerAdminCommissionTransactionModel.GetPoweradminMerchantCommissionReport(Whitelevel, Supervalue, Distributor, Disid.ToString(), statusval);
                
            }

            else if (!string.IsNullOrEmpty(Whitelevel) && !string.IsNullOrEmpty(Supervalue) && !string.IsNullOrEmpty(Distributor) && !string.IsNullOrEmpty(Disid) && string.IsNullOrEmpty(statusval))
            {
                MerchantList = PowerAdminCommissionTransactionModel.GetPoweradminMerchantCommissionReport(Whitelevel, Supervalue, Distributor, Disid.ToString(), statusval);
                
            }
            else if (!string.IsNullOrEmpty(Whitelevel) && !string.IsNullOrEmpty(Supervalue) && !string.IsNullOrEmpty(Distributor) && !string.IsNullOrEmpty(Disid) && !string.IsNullOrEmpty(statusval))
            {
                MerchantList = PowerAdminCommissionTransactionModel.GetPoweradminMerchantCommissionReport(Whitelevel, Supervalue, Distributor, Disid.ToString(), statusval);
                
            }



            //if (Disid == "" && statusval == "")
            //{
            //    //long mem_id = long.Parse(Disid.ToString());
            //    MerchantList = PowerAdminCommissionTransactionModel.GetPoweradminAllMerchantCommissionReport("0", statusval);
            //}
            //else if (Disid == "" && statusval != "")
            //{
            //    //long mem_id = long.Parse(Disid.ToString());
            //    MerchantList = PowerAdminCommissionTransactionModel.GetPoweradminAllMerchantCommissionReport("0", statusval);
            //}
            //else if (Disid != "" && statusval == "")
            //{
            //    //long mem_id = long.Parse(Disid.ToString());
            //    MerchantList = PowerAdminCommissionTransactionModel.GetPoweradminMerchantCommissionReport(Disid.ToString(), statusval);
            //}
            //else if (Disid != "" && statusval != "")
            //{
            //    //long mem_id = long.Parse(Disid.ToString());
            //    MerchantList = PowerAdminCommissionTransactionModel.GetPoweradminMerchantCommissionReport(Disid.ToString(), statusval);
            //}
            IGrid<TBL_ACCOUNTS> grid = new Grid<TBL_ACCOUNTS>(MerchantList);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.SerialNo).Titled("Sln No.");
            grid.Columns.Add(model => model.UserName).Titled("User Name");
            //grid.Columns.Add(model => model.MEMBER_TYPE).Titled("Merchant Type");
            grid.Columns.Add(model => model.TRANSACTION_TYPE).Titled("Transaction Type");
            grid.Columns.Add(model => model.TRANSACTION_DATE).Titled("Transaction Date").Formatted("{0:d}");
            grid.Columns.Add(model => model.DR_CR).Titled("DR/CR");
            grid.Columns.Add(model => model.AMOUNT).Titled("Amount");
            //grid.Columns.Add(model => model.NARRATION).Titled("Narration");
            grid.Columns.Add(model => model.OPENING).Titled("Opening");
            grid.Columns.Add(model => model.CLOSING).Titled("Closing");
            grid.Columns.Add(model => model.COMM_AMT).Titled("Commission Amt.");
            grid.Pager = new GridPager<TBL_ACCOUNTS>(grid);
            grid.Processors.Add(grid.Pager);
            //grid.Pager.RowsPerPage = 6;

            foreach (IGridColumn column in grid.Columns)
            {
                column.Filter.IsEnabled = true;
                column.Sort.IsEnabled = true;
            }

            return grid;

        }
        #endregion

    }
}