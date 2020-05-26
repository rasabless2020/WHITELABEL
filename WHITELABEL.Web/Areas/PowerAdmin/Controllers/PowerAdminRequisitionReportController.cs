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
    public class PowerAdminRequisitionReportController : PoweradminbaseController
    {
        // GET: PowerAdmin/PowerAdminRequisitionReport
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
        #region White Level RequisitionReport
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
        public PartialViewResult Indexgrid(string search = "", string Status = "")
        {
            if (Status == "" && search == "")
            {
                var Adminrequisition = PowerAdminRquisitionReportModel.GetAllAdminRequisitionReport("0", Status);
                return PartialView("Indexgrid", Adminrequisition);
            }
            else if (Status != "" && search == "")
            {
                var Adminrequisition = PowerAdminRquisitionReportModel.GetAllAdminRequisitionReport("0", Status);
                return PartialView("Indexgrid", Adminrequisition);
            }
            else if (Status == "" && search != "")
            {
                var Adminrequisition = PowerAdminRquisitionReportModel.GetAdminRequisitionReport(search, Status);
                return PartialView("Indexgrid", Adminrequisition);
            }
            else if (Status != "" && search != "")
            {
                var Adminrequisition = PowerAdminRquisitionReportModel.GetAdminRequisitionReport(search, Status);
                return PartialView("Indexgrid", Adminrequisition);
            }
            return PartialView("Indexgrid", "");
        }
        [HttpGet]
        public FileResult ExportWhiteLevelIndex(string Disid, string statusval)
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = CreateExporWhiteLevelGrid(Disid,statusval);
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
        private IGrid<TBL_BALANCE_TRANSFER_LOGS> CreateExporWhiteLevelGrid(string Disid, string statusval)
        {
            var db = new DBContext();
            var Adminrequisition = new List<TBL_BALANCE_TRANSFER_LOGS>();
            if (Disid == "" && statusval == "")
            {
                Adminrequisition = PowerAdminRquisitionReportModel.GetAllAdminRequisitionReport("0", statusval);
            }
            else if (Disid == "" && statusval != "")
            {
                Adminrequisition = PowerAdminRquisitionReportModel.GetAllAdminRequisitionReport("0", statusval);
            }
            else if (Disid != "" && statusval == "")
            {
                Adminrequisition = PowerAdminRquisitionReportModel.GetAdminRequisitionReport(Disid, statusval);
            }
            else if (Disid != "" && statusval != "")
            {
                Adminrequisition = PowerAdminRquisitionReportModel.GetAdminRequisitionReport(Disid, statusval);
            }
            
            IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = new Grid<TBL_BALANCE_TRANSFER_LOGS>(Adminrequisition);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.Serial_No).Titled("Se Id");
            //grid.Columns.Add(model => model.ToUser).Titled("To User");
            grid.Columns.Add(model => model.FromUser).Titled("User Name");
            grid.Columns.Add(model => model.REQUEST_DATE).Titled("Req Date").Formatted("{0:d}").MultiFilterable(true);
            grid.Columns.Add(model => model.AMOUNT).Titled("Amount");
            grid.Columns.Add(model => model.BANK_ACCOUNT).Titled("Bank Acnt");
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
        #endregion
        #region Super Distributor Report
        public ActionResult PowerAdminSuperRequisition()
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
        public PartialViewResult SuperIndexgrid(string WhiteLevel="", string Super = "", string Status = "")
        {
            if (WhiteLevel=="" && Status == "" && Super == "")
            {
                var Adminrequisition = PowerAdminRquisitionReportModel.GetAllSuperRequisitionReport("0", Status);
                return PartialView("SuperIndexgrid", Adminrequisition);
            }
            else if (WhiteLevel == "" && Status != "" && Super == "")
            {
                var Adminrequisition = PowerAdminRquisitionReportModel.GetAllSuperRequisitionReport("0", Status);
                return PartialView("SuperIndexgrid", Adminrequisition);
            }
            else if (WhiteLevel!="" && Status == "" && Super != "")
            {
                var Adminrequisition = PowerAdminRquisitionReportModel.GetSuperRequisitionReport(WhiteLevel, Super, Status);
                return PartialView("SuperIndexgrid", Adminrequisition);
            }
            else if (WhiteLevel != "" && Status != "" && Super != "")
            {
                var Adminrequisition = PowerAdminRquisitionReportModel.GetSuperRequisitionReport(WhiteLevel, Super, Status);
                return PartialView("SuperIndexgrid", Adminrequisition);
            }
            else if (WhiteLevel != "" && Status == "" && Super == "")
            {
                var Adminrequisition = PowerAdminRquisitionReportModel.GetSuperRequisitionReport(WhiteLevel, Super, Status);
                return PartialView("SuperIndexgrid", Adminrequisition);
            }
            else if (WhiteLevel != "" && Status != "" && Super == "")
            {
                var Adminrequisition = PowerAdminRquisitionReportModel.GetSuperRequisitionReport(WhiteLevel, Super, Status);
                return PartialView("SuperIndexgrid", Adminrequisition);
            }

            return PartialView("SuperIndexgrid", "");
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
        [HttpGet]
        public FileResult ExportSuperDisIndex(string Whitelevel, string Disid, string statusval)
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = CreateExportSuperDisGrid(Whitelevel, Disid, statusval);
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
        private IGrid<TBL_BALANCE_TRANSFER_LOGS> CreateExportSuperDisGrid(string Whitelevel, string Disid, string statusval)
        {
            var db = new DBContext();
            var Adminrequisition = new List<TBL_BALANCE_TRANSFER_LOGS>();
            if (Whitelevel == "" && statusval == "" && Disid == "")
            {
                Adminrequisition = PowerAdminRquisitionReportModel.GetAllSuperRequisitionReport("0", statusval);
                
            }
            else if (Whitelevel == "" && statusval != "" && Disid == "")
            {
                Adminrequisition = PowerAdminRquisitionReportModel.GetAllSuperRequisitionReport("0", statusval);
                
            }
            else if (Whitelevel != "" && statusval == "" && Disid != "")
            {
                 Adminrequisition = PowerAdminRquisitionReportModel.GetSuperRequisitionReport(Whitelevel, Disid, statusval);
                
            }
            else if (Whitelevel != "" && statusval != "" && Disid != "")
            {
                Adminrequisition = PowerAdminRquisitionReportModel.GetSuperRequisitionReport(Whitelevel, Disid, statusval);
                
            }
            else if (Whitelevel != "" && statusval == "" && Disid == "")
            {
                Adminrequisition = PowerAdminRquisitionReportModel.GetSuperRequisitionReport(Whitelevel, Disid, statusval);
                
            }
            else if (Whitelevel != "" && statusval != "" && Disid == "")
            {
                Adminrequisition = PowerAdminRquisitionReportModel.GetSuperRequisitionReport(Whitelevel, Disid, statusval);
                
            }
            //if (Disid == "" && statusval == "")
            //{
            //    Adminrequisition = PowerAdminRquisitionReportModel.GetAllSuperRequisitionReport("0", statusval);
            //}
            //else if (Disid == "" && statusval != "")
            //{
            //    Adminrequisition = PowerAdminRquisitionReportModel.GetAllSuperRequisitionReport("0", statusval);
            //}
            //else if (Disid != "" && statusval == "")
            //{
            //    Adminrequisition = PowerAdminRquisitionReportModel.GetSuperRequisitionReport(Disid, statusval);
            //}
            //else if (Disid != "" && statusval != "")
            //{
            //    Adminrequisition = PowerAdminRquisitionReportModel.GetSuperRequisitionReport(Disid, statusval);
            //}

            IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = new Grid<TBL_BALANCE_TRANSFER_LOGS>(Adminrequisition);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.Serial_No).Titled("Se Id");
            //grid.Columns.Add(model => model.ToUser).Titled("To User");
            grid.Columns.Add(model => model.FromUser).Titled("User Name");
            grid.Columns.Add(model => model.REQUEST_DATE).Titled("Req Date").Formatted("{0:d}").MultiFilterable(true);
            grid.Columns.Add(model => model.REQUEST_TIME).Titled("Req Time").Formatted("{0:d}").MultiFilterable(true);
            grid.Columns.Add(model => model.AMOUNT).Titled("Amount");
            grid.Columns.Add(model => model.BANK_ACCOUNT).Titled("Bank Acnt");
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
        #endregion
        #region Distributor Requisition Report
        public ActionResult PowerAdminDistributorRequisition()
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
        public PartialViewResult DistributorIndexgrid(string WhiteLevel="", string SuperId="", string Distributor = "", string Status = "")
        {
            if (WhiteLevel=="" && SuperId == ""&& Status == "" && Distributor == "")
            {
                var Adminrequisition = PowerAdminRquisitionReportModel.GetAllDistributorRequisitionReport("0", Status);
                return PartialView("DistributorIndexgrid", Adminrequisition);
            }
            else if (WhiteLevel == "" && SuperId == "" && Status != "" && Distributor == "")
            {
                var Adminrequisition = PowerAdminRquisitionReportModel.GetAllDistributorRequisitionReport("0", Status);
                return PartialView("DistributorIndexgrid", Adminrequisition);
            }
            else if (Status == "" && Distributor == "" && WhiteLevel!="" && SuperId=="")
            {
                var Adminrequisition = PowerAdminRquisitionReportModel.GetDistributorRequisitionReport(WhiteLevel, SuperId, Distributor, Status);
                return PartialView("DistributorIndexgrid", Adminrequisition);
            }
            else if (Status != "" && Distributor != "" && WhiteLevel != "" && SuperId != "")
            {
                var Adminrequisition = PowerAdminRquisitionReportModel.GetDistributorRequisitionReport(WhiteLevel, SuperId, Distributor, Status);
                return PartialView("DistributorIndexgrid", Adminrequisition);
            }
            else if (WhiteLevel != "" && SuperId != "" && Status == "" && Distributor != "")
            {
                var Adminrequisition = PowerAdminRquisitionReportModel.GetDistributorRequisitionReport(WhiteLevel, SuperId, Distributor, Status);
                return PartialView("DistributorIndexgrid", Adminrequisition);
            }
            else if (WhiteLevel != "" && SuperId != "" && Status == "" && Distributor == "")
            {
                var Adminrequisition = PowerAdminRquisitionReportModel.GetDistributorRequisitionReport(WhiteLevel, SuperId, Distributor, Status);
                return PartialView("DistributorIndexgrid", Adminrequisition);
            }
            return PartialView("DistributorIndexgrid", "");
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
        [HttpGet]
        public FileResult ExportDistributorIndex(string WhiteLevel, string SuperId, string Disid, string statusval)
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = CreateExportdDistributorGrid(WhiteLevel, SuperId, Disid, statusval);
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
        private IGrid<TBL_BALANCE_TRANSFER_LOGS> CreateExportdDistributorGrid(string WhiteLevel, string SuperId,  string Disid, string statusval)
        {
            var db = new DBContext();
            var Adminrequisition = new List<TBL_BALANCE_TRANSFER_LOGS>();
            if (WhiteLevel=="" && SuperId=="" && Disid == "" && statusval == "")
            {
                Adminrequisition = PowerAdminRquisitionReportModel.GetAllDistributorRequisitionReport("0", statusval);
            }
            else if (WhiteLevel == "" && SuperId == "" && Disid == "" && statusval != "")
            {
                Adminrequisition = PowerAdminRquisitionReportModel.GetAllDistributorRequisitionReport("0", statusval);
            }
            else if (statusval == "" && Disid == "" && WhiteLevel != "" && SuperId == "")
            {
                Adminrequisition = PowerAdminRquisitionReportModel.GetDistributorRequisitionReport(WhiteLevel, SuperId, Disid, statusval);
                
            }
            else if (statusval != "" && Disid != "" && WhiteLevel != "" && SuperId != "")
            {
                Adminrequisition = PowerAdminRquisitionReportModel.GetDistributorRequisitionReport(WhiteLevel, SuperId, Disid, statusval);
                
            }
            else if (WhiteLevel != "" && SuperId != "" && statusval == "" && Disid != "")
            {
                Adminrequisition = PowerAdminRquisitionReportModel.GetDistributorRequisitionReport(WhiteLevel, SuperId, Disid, statusval);
                
            }
            else if (WhiteLevel != "" && SuperId != "" && statusval == "" && Disid == "")
            {
                Adminrequisition = PowerAdminRquisitionReportModel.GetDistributorRequisitionReport(WhiteLevel, SuperId, Disid, statusval);

            }


            //else if (Disid != "" && statusval == "")
            //{
            //    Adminrequisition = PowerAdminRquisitionReportModel.GetDistributorRequisitionReport(Disid, statusval);
            //}
            //else if (Disid != "" && statusval != "")
            //{
            //    Adminrequisition = PowerAdminRquisitionReportModel.GetDistributorRequisitionReport(Disid, statusval);
            //}

            IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = new Grid<TBL_BALANCE_TRANSFER_LOGS>(Adminrequisition);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.Serial_No).Titled("Se Id");
            //grid.Columns.Add(model => model.ToUser).Titled("To User");
            grid.Columns.Add(model => model.FromUser).Titled("User Name");
            grid.Columns.Add(model => model.REQUEST_DATE).Titled("Req Date").Formatted("{0:d}").MultiFilterable(true);
            grid.Columns.Add(model => model.REQUEST_TIME).Titled("Req Time").Formatted("{0:d}").MultiFilterable(true);
            grid.Columns.Add(model => model.AMOUNT).Titled("Amount");
            grid.Columns.Add(model => model.BANK_ACCOUNT).Titled("Bank Acnt");
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
        #endregion

        #region Merchant Requisition Report
        public ActionResult PowerAdminMerchantRequisition()
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
        public PartialViewResult MerchantIndexgrid(string WhiteLevel="", string SuperId="", string Distributor="",  string Merchant = "", string Status = "")
        {
            if (WhiteLevel == "" && SuperId == "" && Status == "" && Merchant=="" && Distributor == "")
            {
                var Adminrequisition = PowerAdminRquisitionReportModel.GetAllMerchantRequisitionReport("0", Status);
                return PartialView("MerchantIndexgrid", Adminrequisition);
            }
            else if (Status != "" && WhiteLevel == "" && SuperId == "" && Distributor == "" && Merchant == "")
            {
                var Adminrequisition = PowerAdminRquisitionReportModel.GetAllMerchantRequisitionReport("0", Status);
                return PartialView("MerchantIndexgrid", Adminrequisition);
            }
            else if (Status != "" && WhiteLevel != "" && SuperId == "" && Distributor == "" && Merchant == "")
            {
                //var Adminrequisition = PowerAdminRquisitionReportModel.GetMerchantRequisitionReport(Merchant, Status);
                var Adminrequisition = PowerAdminRquisitionReportModel.GetMerchantRequisitionReport(WhiteLevel, SuperId, Distributor, Merchant, Status);
                return PartialView("MerchantIndexgrid", Adminrequisition);
            }
            else if (Status == "" && WhiteLevel != "" && SuperId == "" && Distributor == "" && Merchant == "")
            {
                //var Adminrequisition = PowerAdminRquisitionReportModel.GetMerchantRequisitionReport(Merchant, Status);
                var Adminrequisition = PowerAdminRquisitionReportModel.GetMerchantRequisitionReport(WhiteLevel, SuperId, Distributor, Merchant, Status);
                return PartialView("MerchantIndexgrid", Adminrequisition);
            }
            else if (Status == "" && WhiteLevel != "" && SuperId != "" && Distributor == "" && Merchant == "")
            {
                //var Adminrequisition = PowerAdminRquisitionReportModel.GetMerchantRequisitionReport(Merchant, Status);
                var Adminrequisition = PowerAdminRquisitionReportModel.GetMerchantRequisitionReport(WhiteLevel, SuperId, Distributor, Merchant, Status);
                return PartialView("MerchantIndexgrid", Adminrequisition);
            }
            else if (Status != "" && WhiteLevel != "" && SuperId != "" && Distributor == "" && Merchant == "")
            {
                //var Adminrequisition = PowerAdminRquisitionReportModel.GetMerchantRequisitionReport(Merchant, Status);
                var Adminrequisition = PowerAdminRquisitionReportModel.GetMerchantRequisitionReport(WhiteLevel, SuperId, Distributor, Merchant, Status);
                return PartialView("MerchantIndexgrid", Adminrequisition);
            }
            else if (Status != "" && WhiteLevel != "" && SuperId != "" && Distributor != "" && Merchant == "")
            {
                //var Adminrequisition = PowerAdminRquisitionReportModel.GetMerchantRequisitionReport(Merchant, Status);
                var Adminrequisition = PowerAdminRquisitionReportModel.GetMerchantRequisitionReport(WhiteLevel, SuperId, Distributor, Merchant, Status);
                return PartialView("MerchantIndexgrid", Adminrequisition);
            }
            else if (Status == "" && WhiteLevel != "" && SuperId != "" && Distributor != "" && Merchant == "")
            {
                //var Adminrequisition = PowerAdminRquisitionReportModel.GetMerchantRequisitionReport(Merchant, Status);
                var Adminrequisition = PowerAdminRquisitionReportModel.GetMerchantRequisitionReport(WhiteLevel, SuperId, Distributor, Merchant, Status);
                return PartialView("MerchantIndexgrid", Adminrequisition);
            }
            else if (WhiteLevel != "" && SuperId != "" && Distributor != "" && Status != "" && Merchant != "")
            {
                var Adminrequisition = PowerAdminRquisitionReportModel.GetMerchantRequisitionReport(WhiteLevel, SuperId, Distributor, Merchant, Status);
                return PartialView("MerchantIndexgrid", Adminrequisition);
            }
            else if (WhiteLevel != "" && SuperId != "" && Distributor != "" && Status == "" && Merchant != "")
            {
                var Adminrequisition = PowerAdminRquisitionReportModel.GetMerchantRequisitionReport(WhiteLevel, SuperId, Distributor, Merchant, Status);
                return PartialView("MerchantIndexgrid", Adminrequisition);
            }
            return PartialView("MerchantIndexgrid", "");
        }
        [HttpGet]
        public FileResult ExportMerchantIndex( string WhiteLevel,string SuperId, string Disid, string Merchant, string statusval)
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = CreateExportdMerchantGrid(WhiteLevel, SuperId, Disid, Merchant, statusval);
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
        private IGrid<TBL_BALANCE_TRANSFER_LOGS> CreateExportdMerchantGrid(string WhiteLevel,string SuperId, string Disid, string Merchant, string statusval)
        {
            var db = new DBContext();
            var Adminrequisition = new List<TBL_BALANCE_TRANSFER_LOGS>();

            if (WhiteLevel == "" && SuperId == "" && statusval == "" && Merchant == "" && Disid == "")
            {
                Adminrequisition = PowerAdminRquisitionReportModel.GetAllMerchantRequisitionReport("0", statusval);
                
            }
            else if (statusval != "" && WhiteLevel == "" && SuperId == "" && Disid == "" && Merchant == "")
            {
                Adminrequisition = PowerAdminRquisitionReportModel.GetAllMerchantRequisitionReport("0", statusval);
                
            }
            else if (statusval != "" && WhiteLevel != "" && SuperId == "" && Disid == "" && Merchant == "")
            {
                //var Adminrequisition = PowerAdminRquisitionReportModel.GetMerchantRequisitionReport(Merchant, Status);
                Adminrequisition = PowerAdminRquisitionReportModel.GetMerchantRequisitionReport(WhiteLevel, SuperId, Disid, Merchant, statusval);
                
            }
            else if (statusval == "" && WhiteLevel != "" && SuperId == "" && Disid == "" && Merchant == "")
            {
                //var Adminrequisition = PowerAdminRquisitionReportModel.GetMerchantRequisitionReport(Merchant, Status);
                 Adminrequisition = PowerAdminRquisitionReportModel.GetMerchantRequisitionReport(WhiteLevel, SuperId, Disid, Merchant, statusval);
                
            }
            else if (statusval == "" && WhiteLevel != "" && SuperId != "" && Disid == "" && Merchant == "")
            {
                //var Adminrequisition = PowerAdminRquisitionReportModel.GetMerchantRequisitionReport(Merchant, Status);
                Adminrequisition = PowerAdminRquisitionReportModel.GetMerchantRequisitionReport(WhiteLevel, SuperId, Disid, Merchant, statusval);
                
            }
            else if (statusval != "" && WhiteLevel != "" && SuperId != "" && Disid == "" && Merchant == "")
            {
                //var Adminrequisition = PowerAdminRquisitionReportModel.GetMerchantRequisitionReport(Merchant, Status);
                Adminrequisition = PowerAdminRquisitionReportModel.GetMerchantRequisitionReport(WhiteLevel, SuperId, Disid, Merchant, statusval);
                
            }
            else if (statusval != "" && WhiteLevel != "" && SuperId != "" && Disid != "" && Merchant == "")
            {
                //var Adminrequisition = PowerAdminRquisitionReportModel.GetMerchantRequisitionReport(Merchant, Status);
                Adminrequisition = PowerAdminRquisitionReportModel.GetMerchantRequisitionReport(WhiteLevel, SuperId, Disid, Merchant, statusval);
                
            }
            else if (statusval == "" && WhiteLevel != "" && SuperId != "" && Disid != "" && Merchant == "")
            {
                //var Adminrequisition = PowerAdminRquisitionReportModel.GetMerchantRequisitionReport(Merchant, Status);
                Adminrequisition = PowerAdminRquisitionReportModel.GetMerchantRequisitionReport(WhiteLevel, SuperId, Disid, Merchant, statusval);
                
            }
            else if (WhiteLevel != "" && SuperId != "" && Disid != "" && statusval != "" && Merchant != "")
            {
                Adminrequisition = PowerAdminRquisitionReportModel.GetMerchantRequisitionReport(WhiteLevel, SuperId, Disid, Merchant, statusval);
                
            }
            else if (WhiteLevel != "" && SuperId != "" && Disid != "" && statusval == "" && Merchant != "")
            {
                Adminrequisition = PowerAdminRquisitionReportModel.GetMerchantRequisitionReport(WhiteLevel, SuperId, Disid, Merchant, statusval);
                
            }


            //if (Disid == "" && statusval == "")
            //{
            //    Adminrequisition = PowerAdminRquisitionReportModel.GetAllMerchantRequisitionReport("0", statusval);
            //}
            //else if (Disid == "" && statusval != "")
            //{
            //    Adminrequisition = PowerAdminRquisitionReportModel.GetAllMerchantRequisitionReport("0", statusval);
            //}
            //else if (Disid != "" && statusval == "")
            //{
            //    Adminrequisition = PowerAdminRquisitionReportModel.GetMerchantRequisitionReport(Disid, statusval);
            //}
            //else if (Disid != "" && statusval != "")
            //{
            //    Adminrequisition = PowerAdminRquisitionReportModel.GetMerchantRequisitionReport(Disid, statusval);
            //}

            IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = new Grid<TBL_BALANCE_TRANSFER_LOGS>(Adminrequisition);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.Serial_No).Titled("Se Id");
            //grid.Columns.Add(model => model.ToUser).Titled("To User");
            grid.Columns.Add(model => model.FromUser).Titled("User Name");
            grid.Columns.Add(model => model.REQUEST_DATE).Titled("Req Date").Formatted("{0:d}").MultiFilterable(true);
            grid.Columns.Add(model => model.REQUEST_TIME).Titled("Req Time").Formatted("{0:d}").MultiFilterable(true);
            grid.Columns.Add(model => model.AMOUNT).Titled("Amount");
            grid.Columns.Add(model => model.BANK_ACCOUNT).Titled("Bank Acnt");
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


        #endregion
    }
}