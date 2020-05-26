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
using WHITELABEL.Web.Areas.Super.Models;
using WHITELABEL.Web.Controllers;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;

namespace WHITELABEL.Web.Areas.Super.Controllers
{
    [Authorize]
    public class SuperRequisitionReportController : SuperBaseController
    {
        // GET: Super/SuperRequisitionReport
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        public void initpage()
        {
            try
            {
                ViewBag.ControllerName = "Super Requisition";
                SystemClass sclass = new SystemClass();
                string userID = sclass.GetLoggedUser();
                long userid = long.Parse(userID);
                var dbmain = new DBContext();
                if (userID != null && userID != "")
                {

                    TBL_MASTER_MEMBER currUser = dbmain.TBL_MASTER_MEMBER.SingleOrDefault(c => c.MEM_ID == userid && c.ACTIVE_MEMBER == true && c.MEMBER_ROLE == 3);

                    if (currUser != null)
                    {
                        Session["SuperDistributorId"] = currUser.MEM_ID;
                        // Session["UserName"] = currUser.UserName;
                    }
                    else
                    {
                        Response.Redirect(Url.Action("Index", "SuperLogin", new { area = "Super" }));
                        return;
                    }
                }
                if (Session["SuperDistributorId"] == null)
                {
                    //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
                    //Response.Redirect(Url.Action("Index", "StockistDashboard", new { area = "SuperStockist" }));
                    Response.Redirect(Url.Action("Index", "SuperLogin", new { area = "Super" }));
                    return;
                }
                bool Islogin = false;

                if (Session["SuperDistributorId"] != null)
                {
                    Islogin = true;
                    ViewBag.CurrentUserId = MemberCurrentUser.MEM_ID;
                }
                ViewBag.Islogin = Islogin;

            }
            catch (Exception e)
            {
                //ViewBag.UserName = CurrentUser.UserId;
                Console.WriteLine(e.InnerException);
                return;

            }
        }

        public ActionResult Index()
        {
            if (Session["SuperDistributorId"] != null)
            {
                initpage();
                var db = new DBContext();
                var memberService = (from x in db.TBL_MASTER_MEMBER
                                     where x.INTRODUCER == MemberCurrentUser.MEM_ID
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
                return View();
            }
            else
            {
                Session["SuperDistributorId"] = null;
                Session["SuperDistributorUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("SuperDistributorId");
                Session.Remove("SuperDistributorUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "SuperLogin", new { area = "Super" });
            }

        }
        public PartialViewResult IndexGrid(string search = "")
        {
            var transactionlist = SuperRequisitionModel.GetSuperMerchantRequisitionReport(MemberCurrentUser.MEM_ID.ToString(), search);
            return PartialView("IndexGrid", transactionlist);
        }

        [HttpGet]
        public FileResult ExportSuperLevelIndex(string statusval)
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = CreateExportSuperLevelGrid(statusval);
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
        private IGrid<TBL_BALANCE_TRANSFER_LOGS> CreateExportSuperLevelGrid(string statusval)
        {
            var db = new DBContext();
            var Adminrequisition = SuperRequisitionModel.GetSuperMerchantRequisitionReport(MemberCurrentUser.MEM_ID.ToString(), statusval);
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

        public ActionResult DistributorRequisition()
        {
            if (Session["SuperDistributorId"] != null)
            {
                initpage();
                var db = new DBContext();
                var memberService = (from x in db.TBL_MASTER_MEMBER
                                     where x.INTRODUCER == MemberCurrentUser.MEM_ID
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
                return View();
            }
            else
            {
                Session["SuperDistributorId"] = null;
                Session["SuperDistributorUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("SuperDistributorId");
                Session.Remove("SuperDistributorUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "SuperLogin", new { area = "Super" });
            }
        }
        public PartialViewResult DistributorIndexGrid(string search = "",string Status="")
        {
            if (search == "" && Status == "")
            {
                var transactionlist = SuperRequisitionModel.GetAllDistributorRequisitionReport(MemberCurrentUser.MEM_ID.ToString(), Status);
                return PartialView("DistributorIndexGrid", transactionlist);
            }
            else if (search == "" && Status != "")
            {
                var transactionlist = SuperRequisitionModel.GetAllDistributorRequisitionReport(MemberCurrentUser.MEM_ID.ToString(), Status);
                return PartialView("DistributorIndexGrid", transactionlist);
            }
            else if (search != "" && Status != "")
            {
                var transactionlist = SuperRequisitionModel.GetDistributorRequisitionReport(search, Status);
                return PartialView("DistributorIndexGrid", transactionlist);
            }
            else if (search != "" && Status == "")
            {
                var transactionlist = SuperRequisitionModel.GetDistributorRequisitionReport(search, Status);
                return PartialView("DistributorIndexGrid", transactionlist);
            }
            else
            {
                var transactionlist = "";
                return PartialView("DistributorIndexGrid", transactionlist);
            }
        }

        [HttpGet]
        public FileResult ExportSuperLevelDisIndex(string DisId,  string statusval)
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = CreateExportSuperDisLevelGrid(DisId, statusval);
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
        private IGrid<TBL_BALANCE_TRANSFER_LOGS> CreateExportSuperDisLevelGrid(string DisId, string statusval)
        {
            var db = new DBContext();
            var Adminrequisition = new List<TBL_BALANCE_TRANSFER_LOGS>();
            if (statusval == "" && DisId == "")
            {
                Adminrequisition = SuperRequisitionModel.GetAllDistributorRequisitionReport(MemberCurrentUser.MEM_ID.ToString(), statusval);
            }
            else if (statusval != "" && DisId == "")
            {
                Adminrequisition = SuperRequisitionModel.GetAllDistributorRequisitionReport(MemberCurrentUser.MEM_ID.ToString(), statusval);
            }
            else if (statusval == "" && DisId != "")
            {
                Adminrequisition = SuperRequisitionModel.GetDistributorRequisitionReport(DisId, statusval);
            }
            else if (statusval != "" && DisId != "")
            {
                Adminrequisition = SuperRequisitionModel.GetDistributorRequisitionReport(DisId, statusval);
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

        public ActionResult RetailerRequisitionRepost()
        {
            if (Session["SuperDistributorId"] != null)
            {
                initpage();
                var db = new DBContext();
                var memberService = (from x in db.TBL_MASTER_MEMBER
                                     where x.INTRODUCER == MemberCurrentUser.MEM_ID
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
                return View();
            }
            else
            {
                Session["SuperDistributorId"] = null;
                Session["SuperDistributorUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("SuperDistributorId");
                Session.Remove("SuperDistributorUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "SuperLogin", new { area = "Super" });
            }
        }
        public PartialViewResult RetailerRequisitionGrid(string search="",string Status="")
        {

            if (search == "" && Status == "")
            {
                var transactionlist = SuperRequisitionModel.GetAllRetailerRequisitionReport(MemberCurrentUser.MEM_ID.ToString(), Status);
                return PartialView("RetailerRequisitionGrid", transactionlist);
            }
            else if (search == "" && Status != "")
            {
                var transactionlist = SuperRequisitionModel.GetAllRetailerRequisitionReport(MemberCurrentUser.MEM_ID.ToString(), Status);
                return PartialView("RetailerRequisitionGrid", transactionlist);
            }
            else if (search != "" && Status != "")
            {
                var transactionlist = SuperRequisitionModel.GetMerchantRequisitionReport(search, Status);
                return PartialView("RetailerRequisitionGrid", transactionlist);
            }
            else if (search != "" && Status == "")
            {
                var transactionlist = SuperRequisitionModel.GetMerchantRequisitionReport(search, Status);
                return PartialView("RetailerRequisitionGrid", transactionlist);
            }
            else
            {
                var transactionlist = "";
                return PartialView("RetailerRequisitionGrid", transactionlist);
            }

            //var RetailerList = SuperRequisitionModel.GetAllRetailerRequisitionReport(MemberCurrentUser.MEM_ID.ToString(), Status);
            //return PartialView("RetailerRequisitionGrid", RetailerList);
        }

        [HttpGet]
        public FileResult ExportSuperLevelMerchantIndex(string DisId, string statusval)
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = CreateExportSuperMerchantLevelGrid(DisId, statusval);
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
        private IGrid<TBL_BALANCE_TRANSFER_LOGS> CreateExportSuperMerchantLevelGrid(string DisId, string statusval)
        {
            var db = new DBContext();
            var Adminrequisition = new List<TBL_BALANCE_TRANSFER_LOGS>();
            if (statusval == "" && DisId == "")
            {
                Adminrequisition = SuperRequisitionModel.GetAllRetailerRequisitionReport(MemberCurrentUser.MEM_ID.ToString(), statusval);
            }
            else if (statusval != "" && DisId == "")
            {
                Adminrequisition = SuperRequisitionModel.GetAllRetailerRequisitionReport(MemberCurrentUser.MEM_ID.ToString(), statusval);
            }
            else if (statusval == "" && DisId != "")
            {
                Adminrequisition = SuperRequisitionModel.GetMerchantRequisitionReport(DisId, statusval);
            }
            else if (statusval != "" && DisId != "")
            {
                Adminrequisition = SuperRequisitionModel.GetMerchantRequisitionReport(DisId, statusval);
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
    }
}