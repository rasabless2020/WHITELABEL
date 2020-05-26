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
    public class SuperTransactionReportController : SuperBaseController 
    {
        // GET: Super/SuperTransactionReport
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
                                         UName = x.MEMBER_NAME
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
        public PartialViewResult IndexGridSuper(string search = "")
        {
            var db = new DBContext();
            var listCommission = SuperCommissionTransactionModel.GetAllSuperCommissionReport(MemberCurrentUser.MEM_ID.ToString(), search);
            return PartialView("IndexGridSuper", listCommission);
            //if (search == "Mobile Recharge" || search == "DMR")
            //{

            //    var transactionlistvalue = (from x in db.TBL_ACCOUNTS
            //                                join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
            //                                where x.MEM_ID == MemberCurrentUser.MEM_ID && x.TRANSACTION_TYPE == search
            //                                select new
            //                                {
            //                                    SLN = x.ACC_NO,
            //                                    MerchantName = y.UName,
            //                                    MemberType = x.MEMBER_TYPE,
            //                                    Trans_Type = x.TRANSACTION_TYPE,
            //                                    Trans_Date = x.TRANSACTION_DATE,
            //                                    DR_CR = x.DR_CR,
            //                                    Amount = x.AMOUNT,
            //                                    Narration = x.NARRATION,
            //                                    OpeningAmt = x.OPENING,
            //                                    Closing = x.CLOSING,
            //                                    CommissionAmt = x.COMM_AMT
            //                                }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
            //                                {
            //                                    SerialNo = index + 1,
            //                                    ACC_NO = z.SLN,
            //                                    UserName = z.MerchantName,
            //                                    MEMBER_TYPE = z.MemberType,
            //                                    TRANSACTION_TYPE = z.Trans_Type,
            //                                    TRANSACTION_DATE = z.Trans_Date,
            //                                    DR_CR = z.DR_CR,
            //                                    AMOUNT = z.Amount,
            //                                    NARRATION = z.Narration,
            //                                    OPENING = z.OpeningAmt,
            //                                    CLOSING = z.Closing,
            //                                    COMM_AMT = z.CommissionAmt
            //                                }).ToList();
            //    return PartialView("IndexGridSuper", transactionlistvalue);
            //}
            //else if (search == "Requisition")
            //{
            //    var transactionlistvalue = (from x in db.TBL_ACCOUNTS
            //                                join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
            //                                where x.MEM_ID == MemberCurrentUser.MEM_ID && x.TRANSACTION_TYPE!= "Mobile Recharge" & x.TRANSACTION_TYPE!= "DMR"
            //                                select new
            //                                {
            //                                    SLN = x.ACC_NO,
            //                                    MerchantName = y.UName,
            //                                    MemberType = x.MEMBER_TYPE,
            //                                    Trans_Type = x.TRANSACTION_TYPE,
            //                                    Trans_Date = x.TRANSACTION_DATE,
            //                                    DR_CR = x.DR_CR,
            //                                    Amount = x.AMOUNT,
            //                                    Narration = x.NARRATION,
            //                                    OpeningAmt = x.OPENING,
            //                                    Closing = x.CLOSING,
            //                                    CommissionAmt = x.COMM_AMT
            //                                }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
            //                                {
            //                                    SerialNo = index + 1,
            //                                    ACC_NO = z.SLN,
            //                                    UserName = z.MerchantName,
            //                                    MEMBER_TYPE = z.MemberType,
            //                                    TRANSACTION_TYPE = z.Trans_Type,
            //                                    TRANSACTION_DATE = z.Trans_Date,
            //                                    DR_CR = z.DR_CR,
            //                                    AMOUNT = z.Amount,
            //                                    NARRATION = z.Narration,
            //                                    OPENING = z.OpeningAmt,
            //                                    CLOSING = z.Closing,
            //                                    COMM_AMT = z.CommissionAmt
            //                                }).ToList();
            //    return PartialView("IndexGridSuper", transactionlistvalue);
            //}
            //else
            //{
            //    var transactionlistvalue = (from x in db.TBL_ACCOUNTS
            //                                join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
            //                                where x.MEM_ID == MemberCurrentUser.MEM_ID
            //                                select new
            //                                {
            //                                    SLN = x.ACC_NO,
            //                                    MerchantName = y.UName,
            //                                    MemberType = x.MEMBER_TYPE,
            //                                    Trans_Type = x.TRANSACTION_TYPE,
            //                                    Trans_Date = x.TRANSACTION_DATE,
            //                                    DR_CR = x.DR_CR,
            //                                    Amount = x.AMOUNT,
            //                                    Narration = x.NARRATION,
            //                                    OpeningAmt = x.OPENING,
            //                                    Closing = x.CLOSING,
            //                                    CommissionAmt = x.COMM_AMT
            //                                }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
            //                                {
            //                                    SerialNo = index + 1,
            //                                    ACC_NO = z.SLN,
            //                                    UserName = z.MerchantName,
            //                                    MEMBER_TYPE = z.MemberType,
            //                                    TRANSACTION_TYPE = z.Trans_Type,
            //                                    TRANSACTION_DATE = z.Trans_Date,
            //                                    DR_CR = z.DR_CR,
            //                                    AMOUNT = z.Amount,
            //                                    NARRATION = z.Narration,
            //                                    OPENING = z.OpeningAmt,
            //                                    CLOSING = z.Closing,
            //                                    COMM_AMT = z.CommissionAmt
            //                                }).ToList();
            //    return PartialView("IndexGridSuper", transactionlistvalue);

            //}

        }
        public FileResult ExportIndexAdminReport(string statusval)
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;
                var db = new DBContext();


                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_ACCOUNTS> grid = CreateExportAdminTableGrid(statusval);
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

        private IGrid<TBL_ACCOUNTS> CreateExportAdminTableGrid(string statusval)
        {
            var db = new DBContext();
            string[] SuperMemId = null;
            string[] DistributorMemId = null;
            string[] MerchantMemId = null;
            long mem_id = long.Parse(MemberCurrentUser.MEM_ID.ToString());
            var MerchantList = SuperCommissionTransactionModel.GetAllSuperCommissionReport(MemberCurrentUser.MEM_ID.ToString(), statusval);
            //var MerchantList = AdminTransactionViewModel.GetAdminCommissionReport(MemberCurrentUser.MEM_ID.ToString(), statusval);
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


        public ActionResult UnderDistributorReport()
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
                                         UName = x.MEMBER_NAME
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
        public PartialViewResult IndexgridDistributor(string search="",string Status="")
        {
            var db = new DBContext();
            if (string.IsNullOrEmpty(search) && string.IsNullOrEmpty(Status))
            {
                var listCommission = SuperCommissionTransactionModel.GetAllSuperDistributorCommissionReport(MemberCurrentUser.MEM_ID.ToString(), Status);
                return PartialView("IndexgridDistributor", listCommission);
            }
            else if (string.IsNullOrEmpty(search) && !string.IsNullOrEmpty(Status))
            {
                var listCommission = SuperCommissionTransactionModel.GetAllSuperDistributorCommissionReport(MemberCurrentUser.MEM_ID.ToString(), Status);
                return PartialView("IndexgridDistributor", listCommission);
            }
            else if (!string.IsNullOrEmpty(search) && string.IsNullOrEmpty(Status))
            {
                var listCommission = SuperCommissionTransactionModel.GetSuperDistributorCommissionReport(search.ToString(), Status);
                return PartialView("IndexgridDistributor", listCommission);
            }
            else if (!string.IsNullOrEmpty(search) && !string.IsNullOrEmpty(Status))
            {
                var listCommission = SuperCommissionTransactionModel.GetSuperDistributorCommissionReport(search.ToString(), Status);
                return PartialView("IndexgridDistributor", listCommission);
            }
            else
            {
                return PartialView("IndexgridDistributor", "");
            }
            //if (search != "" && (Status == "Mobile Recharge" || Status == "DMR"))
            //{
            //    long dis_id = long.Parse(search.ToString());
            //    var transactionlistvalue = (from x in db.TBL_ACCOUNTS
            //                                join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
            //                                where x.MEM_ID == dis_id && x.TRANSACTION_TYPE == Status
            //                                select new
            //                                {
            //                                    SLN = x.ACC_NO,
            //                                    MerchantName = y.UName,
            //                                    MemberType = x.MEMBER_TYPE,
            //                                    Trans_Type = x.TRANSACTION_TYPE,
            //                                    Trans_Date = x.TRANSACTION_DATE,
            //                                    DR_CR = x.DR_CR,
            //                                    Amount = x.AMOUNT,
            //                                    Narration = x.NARRATION,
            //                                    OpeningAmt = x.OPENING,
            //                                    Closing = x.CLOSING,
            //                                    CommissionAmt = x.COMM_AMT
            //                                }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
            //                                {
            //                                    SerialNo = index + 1,
            //                                    ACC_NO = z.SLN,
            //                                    UserName = z.MerchantName,
            //                                    MEMBER_TYPE = z.MemberType,
            //                                    TRANSACTION_TYPE = z.Trans_Type,
            //                                    TRANSACTION_DATE = z.Trans_Date,
            //                                    DR_CR = z.DR_CR,
            //                                    AMOUNT = z.Amount,
            //                                    NARRATION = z.Narration,
            //                                    OPENING = z.OpeningAmt,
            //                                    CLOSING = z.Closing,
            //                                    COMM_AMT = z.CommissionAmt
            //                                }).ToList();
            //    return PartialView("IndexgridDistributor", transactionlistvalue);
            //}
            //else if (search != "" && Status == "Requisition")
            //{
            //    long dis_id = long.Parse(search.ToString());
            //    var transactionlistvalue = (from x in db.TBL_ACCOUNTS
            //                                join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
            //                                where x.MEM_ID == dis_id && x.TRANSACTION_TYPE != "Mobile Recharge" && x.TRANSACTION_TYPE != "DMR"
            //                                select new
            //                                {
            //                                    SLN = x.ACC_NO,
            //                                    MerchantName = y.UName,
            //                                    MemberType = x.MEMBER_TYPE,
            //                                    Trans_Type = x.TRANSACTION_TYPE,
            //                                    Trans_Date = x.TRANSACTION_DATE,
            //                                    DR_CR = x.DR_CR,
            //                                    Amount = x.AMOUNT,
            //                                    Narration = x.NARRATION,
            //                                    OpeningAmt = x.OPENING,
            //                                    Closing = x.CLOSING,
            //                                    CommissionAmt = x.COMM_AMT
            //                                }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
            //                                {
            //                                    SerialNo = index + 1,
            //                                    ACC_NO = z.SLN,
            //                                    UserName = z.MerchantName,
            //                                    MEMBER_TYPE = z.MemberType,
            //                                    TRANSACTION_TYPE = z.Trans_Type,
            //                                    TRANSACTION_DATE = z.Trans_Date,
            //                                    DR_CR = z.DR_CR,
            //                                    AMOUNT = z.Amount,
            //                                    NARRATION = z.Narration,
            //                                    OPENING = z.OpeningAmt,
            //                                    CLOSING = z.Closing,
            //                                    COMM_AMT = z.CommissionAmt
            //                                }).ToList();
            //    return PartialView("IndexgridDistributor", transactionlistvalue);
            //}
            //else if (search == "" && Status != "")
            //{
            //    if (Status == "Mobile Recharge" || Status == "DMR")
            //    {
            //        var transactionlistvalue = (from x in db.TBL_ACCOUNTS
            //                                    join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
            //                                    where y.INTRODUCER == MemberCurrentUser.MEM_ID && x.TRANSACTION_TYPE==Status
            //                                    select new
            //                                    {
            //                                        SLN = x.ACC_NO,
            //                                        MerchantName = y.UName,
            //                                        MemberType = x.MEMBER_TYPE,
            //                                        Trans_Type = x.TRANSACTION_TYPE,
            //                                        Trans_Date = x.TRANSACTION_DATE,
            //                                        DR_CR = x.DR_CR,
            //                                        Amount = x.AMOUNT,
            //                                        Narration = x.NARRATION,
            //                                        OpeningAmt = x.OPENING,
            //                                        Closing = x.CLOSING,
            //                                        CommissionAmt = x.COMM_AMT
            //                                    }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
            //                                    {
            //                                        SerialNo = index + 1,
            //                                        ACC_NO = z.SLN,
            //                                        UserName = z.MerchantName,
            //                                        MEMBER_TYPE = z.MemberType,
            //                                        TRANSACTION_TYPE = z.Trans_Type,
            //                                        TRANSACTION_DATE = z.Trans_Date,
            //                                        DR_CR = z.DR_CR,
            //                                        AMOUNT = z.Amount,
            //                                        NARRATION = z.Narration,
            //                                        OPENING = z.OpeningAmt,
            //                                        CLOSING = z.Closing,
            //                                        COMM_AMT = z.CommissionAmt
            //                                    }).ToList();
            //        return PartialView("IndexgridDistributor", transactionlistvalue);
            //    }
            //    else
            //    {
            //        var transactionlistvalue = (from x in db.TBL_ACCOUNTS
            //                                    join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
            //                                    where y.INTRODUCER == MemberCurrentUser.MEM_ID && x.TRANSACTION_TYPE != "Mobile Recharge" && x.TRANSACTION_TYPE != "DMR"
            //                                    select new
            //                                    {
            //                                        SLN = x.ACC_NO,
            //                                        MerchantName = y.UName,
            //                                        MemberType = x.MEMBER_TYPE,
            //                                        Trans_Type = x.TRANSACTION_TYPE,
            //                                        Trans_Date = x.TRANSACTION_DATE,
            //                                        DR_CR = x.DR_CR,
            //                                        Amount = x.AMOUNT,
            //                                        Narration = x.NARRATION,
            //                                        OpeningAmt = x.OPENING,
            //                                        Closing = x.CLOSING,
            //                                        CommissionAmt = x.COMM_AMT
            //                                    }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
            //                                    {
            //                                        SerialNo = index + 1,
            //                                        ACC_NO = z.SLN,
            //                                        UserName = z.MerchantName,
            //                                        MEMBER_TYPE = z.MemberType,
            //                                        TRANSACTION_TYPE = z.Trans_Type,
            //                                        TRANSACTION_DATE = z.Trans_Date,
            //                                        DR_CR = z.DR_CR,
            //                                        AMOUNT = z.Amount,
            //                                        NARRATION = z.Narration,
            //                                        OPENING = z.OpeningAmt,
            //                                        CLOSING = z.Closing,
            //                                        COMM_AMT = z.CommissionAmt
            //                                    }).ToList();
            //        return PartialView("IndexgridDistributor", transactionlistvalue);
            //    }
            //}
            //else if (search != "" && Status == "")
            //{
            //    long dis_id = long.Parse(search.ToString());
            //    var transactionlistvalue = (from x in db.TBL_ACCOUNTS
            //                                join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
            //                                where x.MEM_ID == dis_id 
            //                                select new
            //                                {
            //                                    SLN = x.ACC_NO,
            //                                    MerchantName = y.UName,
            //                                    MemberType = x.MEMBER_TYPE,
            //                                    Trans_Type = x.TRANSACTION_TYPE,
            //                                    Trans_Date = x.TRANSACTION_DATE,
            //                                    DR_CR = x.DR_CR,
            //                                    Amount = x.AMOUNT,
            //                                    Narration = x.NARRATION,
            //                                    OpeningAmt = x.OPENING,
            //                                    Closing = x.CLOSING,
            //                                    CommissionAmt = x.COMM_AMT
            //                                }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
            //                                {
            //                                    SerialNo = index + 1,
            //                                    ACC_NO = z.SLN,
            //                                    UserName = z.MerchantName,
            //                                    MEMBER_TYPE = z.MemberType,
            //                                    TRANSACTION_TYPE = z.Trans_Type,
            //                                    TRANSACTION_DATE = z.Trans_Date,
            //                                    DR_CR = z.DR_CR,
            //                                    AMOUNT = z.Amount,
            //                                    NARRATION = z.Narration,
            //                                    OPENING = z.OpeningAmt,
            //                                    CLOSING = z.Closing,
            //                                    COMM_AMT = z.CommissionAmt
            //                                }).ToList();
            //    return PartialView("IndexgridDistributor", transactionlistvalue);
            //}
            //else
            //{
            //    var transactionlistvalue = (from x in db.TBL_ACCOUNTS
            //                                join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
            //                                where y.INTRODUCER == MemberCurrentUser.MEM_ID
            //                                select new
            //                                {
            //                                    SLN = x.ACC_NO,
            //                                    MerchantName = y.UName,
            //                                    MemberType = x.MEMBER_TYPE,
            //                                    Trans_Type = x.TRANSACTION_TYPE,
            //                                    Trans_Date = x.TRANSACTION_DATE,
            //                                    DR_CR = x.DR_CR,
            //                                    Amount = x.AMOUNT,
            //                                    Narration = x.NARRATION,
            //                                    OpeningAmt = x.OPENING,
            //                                    Closing = x.CLOSING,
            //                                    CommissionAmt = x.COMM_AMT
            //                                }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
            //                                {
            //                                    SerialNo = index + 1,
            //                                    ACC_NO = z.SLN,
            //                                    UserName = z.MerchantName,
            //                                    MEMBER_TYPE = z.MemberType,
            //                                    TRANSACTION_TYPE = z.Trans_Type,
            //                                    TRANSACTION_DATE = z.Trans_Date,
            //                                    DR_CR = z.DR_CR,
            //                                    AMOUNT = z.Amount,
            //                                    NARRATION = z.Narration,
            //                                    OPENING = z.OpeningAmt,
            //                                    CLOSING = z.Closing,
            //                                    COMM_AMT = z.CommissionAmt
            //                                }).ToList();
            //    return PartialView("IndexgridDistributor", transactionlistvalue);
            //}

        }

        [HttpGet]
        public FileResult ExportIndexSuperDistributorReport(string Disid, string statusval)
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;
                var db = new DBContext();


                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_ACCOUNTS> grid = CreateExportSuperDistributoTableGrid(Disid, statusval);
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
        private IGrid<TBL_ACCOUNTS> CreateExportSuperDistributoTableGrid(string Disid, string statusval)
        {
            var db = new DBContext();
            string[] SuperMemId = null;
            string[] DistributorMemId = null;
            string[] MerchantMemId = null;

            var MerchantList = new List<TBL_ACCOUNTS>();
            if (string.IsNullOrEmpty(Disid) && string.IsNullOrEmpty(statusval))
            {
                 MerchantList = SuperCommissionTransactionModel.GetAllSuperDistributorCommissionReport(MemberCurrentUser.MEM_ID.ToString(), statusval);
                
            }
            else if (string.IsNullOrEmpty(Disid) && !string.IsNullOrEmpty(statusval))
            {
                 MerchantList = SuperCommissionTransactionModel.GetAllSuperDistributorCommissionReport(MemberCurrentUser.MEM_ID.ToString(), statusval);
                
            }
            else if (!string.IsNullOrEmpty(Disid) && string.IsNullOrEmpty(statusval))
            {
                 MerchantList = SuperCommissionTransactionModel.GetSuperDistributorCommissionReport(Disid.ToString(), statusval);
                
            }
            else if (!string.IsNullOrEmpty(Disid) && !string.IsNullOrEmpty(statusval))
            {
                 MerchantList = SuperCommissionTransactionModel.GetSuperDistributorCommissionReport(Disid.ToString(), statusval);
                
            }
           
            long mem_id = long.Parse(MemberCurrentUser.MEM_ID.ToString());
               
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

        public ActionResult SuperMerchantTransactionReport()
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
                                         UName = x.MEMBER_NAME
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
        public PartialViewResult IndexGridMerchant(string search = "", string Status = "")
        {
            var db = new DBContext();
            if (string.IsNullOrEmpty(search) && string.IsNullOrEmpty(Status))
            {
                var MerchantList = SuperCommissionTransactionModel.GetAllSuperMerchantCommissionReport(MemberCurrentUser.MEM_ID.ToString(), Status);
                return PartialView("IndexGridMerchant", MerchantList);
            }
            else if (string.IsNullOrEmpty(search) && !string.IsNullOrEmpty(Status))
            {
                var MerchantList = SuperCommissionTransactionModel.GetAllSuperMerchantCommissionReport(MemberCurrentUser.MEM_ID.ToString(), Status);
                return PartialView("IndexGridMerchant", MerchantList);
            }
            else if (!string.IsNullOrEmpty(search) && string.IsNullOrEmpty(Status))
            {
                var MerchantList = SuperCommissionTransactionModel.GetSuperMerchantCommissionReport(search.ToString(), Status);
                return PartialView("IndexGridMerchant", MerchantList);
            }
            else if (!string.IsNullOrEmpty(search) && !string.IsNullOrEmpty(Status))
            {
                var MerchantList = SuperCommissionTransactionModel.GetSuperMerchantCommissionReport(search.ToString(), Status);
                return PartialView("IndexGridMerchant", MerchantList);
            }
            else
            {
                return PartialView("IndexGridMerchant", "");
            }

            //if (search != "" && (Status == "Mobile Recharge" || Status == "DMR"))
            //{
            //    long dis_id = long.Parse(search.ToString());
            //    var transactionlistvalue = (from x in db.TBL_ACCOUNTS
            //                                join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
            //                                where x.MEM_ID == dis_id && x.TRANSACTION_TYPE== Status
            //                                select new
            //                                {
            //                                    SLN = x.ACC_NO,
            //                                    MerchantName = y.UName,
            //                                    MemberType = x.MEMBER_TYPE,
            //                                    Trans_Type = x.TRANSACTION_TYPE,
            //                                    Trans_Date = x.TRANSACTION_DATE,
            //                                    DR_CR = x.DR_CR,
            //                                    Amount = x.AMOUNT,
            //                                    Narration = x.NARRATION,
            //                                    OpeningAmt = x.OPENING,
            //                                    Closing = x.CLOSING,
            //                                    CommissionAmt = x.COMM_AMT
            //                                }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
            //                                {
            //                                    SerialNo = index + 1,
            //                                    ACC_NO = z.SLN,
            //                                    UserName = z.MerchantName,
            //                                    MEMBER_TYPE = z.MemberType,
            //                                    TRANSACTION_TYPE = z.Trans_Type,
            //                                    TRANSACTION_DATE = z.Trans_Date,
            //                                    DR_CR = z.DR_CR,
            //                                    AMOUNT = z.Amount,
            //                                    NARRATION = z.Narration,
            //                                    OPENING = z.OpeningAmt,
            //                                    CLOSING = z.Closing,
            //                                    COMM_AMT = z.CommissionAmt
            //                                }).ToList();
            //    return PartialView("IndexGridMerchant", transactionlistvalue);
            //}
            //else if (search != "" && Status == "Requisition")
            //{
            //    long dis_id = long.Parse(search.ToString());
            //    var transactionlistvalue = (from x in db.TBL_ACCOUNTS
            //                                join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
            //                                where x.MEM_ID == dis_id && x.TRANSACTION_TYPE != "Mobile Recharge" && x.TRANSACTION_TYPE!="DMR"
            //                                select new
            //                                {
            //                                    SLN = x.ACC_NO,
            //                                    MerchantName = y.UName,
            //                                    MemberType = x.MEMBER_TYPE,
            //                                    Trans_Type = x.TRANSACTION_TYPE,
            //                                    Trans_Date = x.TRANSACTION_DATE,
            //                                    DR_CR = x.DR_CR,
            //                                    Amount = x.AMOUNT,
            //                                    Narration = x.NARRATION,
            //                                    OpeningAmt = x.OPENING,
            //                                    Closing = x.CLOSING,
            //                                    CommissionAmt = x.COMM_AMT
            //                                }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
            //                                {
            //                                    SerialNo = index + 1,
            //                                    ACC_NO = z.SLN,
            //                                    UserName = z.MerchantName,
            //                                    MEMBER_TYPE = z.MemberType,
            //                                    TRANSACTION_TYPE = z.Trans_Type,
            //                                    TRANSACTION_DATE = z.Trans_Date,
            //                                    DR_CR = z.DR_CR,
            //                                    AMOUNT = z.Amount,
            //                                    NARRATION = z.Narration,
            //                                    OPENING = z.OpeningAmt,
            //                                    CLOSING = z.Closing,
            //                                    COMM_AMT = z.CommissionAmt
            //                                }).ToList();
            //    return PartialView("IndexGridMerchant", transactionlistvalue);
            //}
            //else if (search == "" && Status != "")
            //{
            //    if (Status == "Mobile Recharge" || Status == "DMR")
            //    {
            //        long dis_id = long.Parse(search.ToString());
            //        var transactionlistvalue = (from x in db.TBL_ACCOUNTS
            //                                    join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
            //                                    where x.MEM_ID == dis_id && x.TRANSACTION_TYPE ==Status
            //                                    select new
            //                                    {
            //                                        SLN = x.ACC_NO,
            //                                        MerchantName = y.UName,
            //                                        MemberType = x.MEMBER_TYPE,
            //                                        Trans_Type = x.TRANSACTION_TYPE,
            //                                        Trans_Date = x.TRANSACTION_DATE,
            //                                        DR_CR = x.DR_CR,
            //                                        Amount = x.AMOUNT,
            //                                        Narration = x.NARRATION,
            //                                        OpeningAmt = x.OPENING,
            //                                        Closing = x.CLOSING,
            //                                        CommissionAmt = x.COMM_AMT
            //                                    }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
            //                                    {
            //                                        SerialNo = index + 1,
            //                                        ACC_NO = z.SLN,
            //                                        UserName = z.MerchantName,
            //                                        MEMBER_TYPE = z.MemberType,
            //                                        TRANSACTION_TYPE = z.Trans_Type,
            //                                        TRANSACTION_DATE = z.Trans_Date,
            //                                        DR_CR = z.DR_CR,
            //                                        AMOUNT = z.Amount,
            //                                        NARRATION = z.Narration,
            //                                        OPENING = z.OpeningAmt,
            //                                        CLOSING = z.Closing,
            //                                        COMM_AMT = z.CommissionAmt
            //                                    }).ToList();
            //        return PartialView("IndexGridMerchant", transactionlistvalue);
            //    }
            //    else
            //    {
            //        long dis_id = long.Parse(search.ToString());
            //        var transactionlistvalue = (from x in db.TBL_ACCOUNTS
            //                                    join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
            //                                    where x.MEM_ID == dis_id && x.TRANSACTION_TYPE != "Mobile Recharge" && x.TRANSACTION_TYPE != "DMR"
            //                                    select new
            //                                    {
            //                                        SLN = x.ACC_NO,
            //                                        MerchantName = y.UName,
            //                                        MemberType = x.MEMBER_TYPE,
            //                                        Trans_Type = x.TRANSACTION_TYPE,
            //                                        Trans_Date = x.TRANSACTION_DATE,
            //                                        DR_CR = x.DR_CR,
            //                                        Amount = x.AMOUNT,
            //                                        Narration = x.NARRATION,
            //                                        OpeningAmt = x.OPENING,
            //                                        Closing = x.CLOSING,
            //                                        CommissionAmt = x.COMM_AMT
            //                                    }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
            //                                    {
            //                                        SerialNo = index + 1,
            //                                        ACC_NO = z.SLN,
            //                                        UserName = z.MerchantName,
            //                                        MEMBER_TYPE = z.MemberType,
            //                                        TRANSACTION_TYPE = z.Trans_Type,
            //                                        TRANSACTION_DATE = z.Trans_Date,
            //                                        DR_CR = z.DR_CR,
            //                                        AMOUNT = z.Amount,
            //                                        NARRATION = z.Narration,
            //                                        OPENING = z.OpeningAmt,
            //                                        CLOSING = z.Closing,
            //                                        COMM_AMT = z.CommissionAmt
            //                                    }).ToList();
            //        return PartialView("IndexGridMerchant", transactionlistvalue);
            //    }
            //}
            //else if (search != "" && Status == "")
            //{
            //    long dis_id = long.Parse(search.ToString());
            //    var transactionlistvalue = (from x in db.TBL_ACCOUNTS
            //                                join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
            //                                where x.MEM_ID == dis_id 
            //                                select new
            //                                {
            //                                    SLN = x.ACC_NO,
            //                                    MerchantName = y.UName,
            //                                    MemberType = x.MEMBER_TYPE,
            //                                    Trans_Type = x.TRANSACTION_TYPE,
            //                                    Trans_Date = x.TRANSACTION_DATE,
            //                                    DR_CR = x.DR_CR,
            //                                    Amount = x.AMOUNT,
            //                                    Narration = x.NARRATION,
            //                                    OpeningAmt = x.OPENING,
            //                                    Closing = x.CLOSING,
            //                                    CommissionAmt = x.COMM_AMT
            //                                }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
            //                                {
            //                                    SerialNo = index + 1,
            //                                    ACC_NO = z.SLN,
            //                                    UserName = z.MerchantName,
            //                                    MEMBER_TYPE = z.MemberType,
            //                                    TRANSACTION_TYPE = z.Trans_Type,
            //                                    TRANSACTION_DATE = z.Trans_Date,
            //                                    DR_CR = z.DR_CR,
            //                                    AMOUNT = z.Amount,
            //                                    NARRATION = z.Narration,
            //                                    OPENING = z.OpeningAmt,
            //                                    CLOSING = z.Closing,
            //                                    COMM_AMT = z.CommissionAmt
            //                                }).ToList();
            //    return PartialView("IndexGridMerchant", transactionlistvalue);
            //}
            //else
            //{
            //    string[] searchTest = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == MemberCurrentUser.MEM_ID).Select(a => a.MEM_ID.ToString()).ToArray();
            //    var transactionlistvalue = (from x in db.TBL_ACCOUNTS
            //                                join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
            //                                where searchTest.Contains(y.INTRODUCER.ToString())
            //                                select new
            //                                {
            //                                    SLN = x.ACC_NO,
            //                                    MerchantName = y.UName,
            //                                   // MerchantName = db.TBL_MASTER_MEMBER.Where(c=>c.MEM_ID==x.MEM_ID).Select(s=>s.UName).FirstOrDefault(),
            //                                    MemberType = x.MEMBER_TYPE,
            //                                    Trans_Type = x.TRANSACTION_TYPE,
            //                                    Trans_Date = x.TRANSACTION_DATE,
            //                                    DR_CR = x.DR_CR,
            //                                    Amount = x.AMOUNT,
            //                                    Narration = x.NARRATION,
            //                                    OpeningAmt = x.OPENING,
            //                                    Closing = x.CLOSING,
            //                                    CommissionAmt = x.COMM_AMT
            //                                }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
            //                                {
            //                                    SerialNo = index + 1,
            //                                    ACC_NO = z.SLN,
            //                                    UserName = z.MerchantName,
            //                                    MEMBER_TYPE = z.MemberType,
            //                                    TRANSACTION_TYPE = z.Trans_Type,
            //                                    TRANSACTION_DATE = z.Trans_Date,
            //                                    DR_CR = z.DR_CR,
            //                                    AMOUNT = z.Amount,
            //                                    NARRATION = z.Narration,
            //                                    OPENING = z.OpeningAmt,
            //                                    CLOSING = z.Closing,
            //                                    COMM_AMT = z.CommissionAmt
            //                                }).ToList();
            //    return PartialView("IndexGridMerchant", transactionlistvalue);
            //}

        }

        [HttpGet]
        public FileResult ExportIndexSuperMerchantReport(string Disid, string statusval)
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;
                var db = new DBContext();


                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_ACCOUNTS> grid = CreateExportSuperMerchantTableGrid(Disid, statusval);
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
        private IGrid<TBL_ACCOUNTS> CreateExportSuperMerchantTableGrid(string Disid, string statusval)
        {
            var db = new DBContext();
            string[] SuperMemId = null;
            string[] DistributorMemId = null;
            string[] MerchantMemId = null;

            var MerchantList = new List<TBL_ACCOUNTS>();
            if (string.IsNullOrEmpty(Disid) && string.IsNullOrEmpty(statusval))
            {
                MerchantList = SuperCommissionTransactionModel.GetAllSuperMerchantCommissionReport(MemberCurrentUser.MEM_ID.ToString(), statusval);

            }
            else if (string.IsNullOrEmpty(Disid) && !string.IsNullOrEmpty(statusval))
            {
                MerchantList = SuperCommissionTransactionModel.GetAllSuperMerchantCommissionReport(MemberCurrentUser.MEM_ID.ToString(), statusval);

            }
            else if (!string.IsNullOrEmpty(Disid) && string.IsNullOrEmpty(statusval))
            {
                MerchantList = SuperCommissionTransactionModel.GetSuperMerchantCommissionReport(Disid.ToString(), statusval);

            }
            else if (!string.IsNullOrEmpty(Disid) && !string.IsNullOrEmpty(statusval))
            {
                MerchantList = SuperCommissionTransactionModel.GetSuperMerchantCommissionReport(Disid.ToString(), statusval);

            }

            long mem_id = long.Parse(MemberCurrentUser.MEM_ID.ToString());

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