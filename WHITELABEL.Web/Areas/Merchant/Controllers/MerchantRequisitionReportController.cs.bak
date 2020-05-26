using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Models;
using WHITELABEL.Web.Helper;
using System.Data.Entity.Core;
using WHITELABEL.Web.Areas.Merchant.Models;
using WHITELABEL.Web.ServiceApi.RECHARGE.PORTIQUE;
using static WHITELABEL.Web.Helper.InstantPayApi;
using NonFactors.Mvc.Grid;
using OfficeOpenXml;
using System.Threading.Tasks;
using System.Data.Entity;
using log4net;

namespace WHITELABEL.Web.Areas.Merchant.Controllers
{
    [Authorize]
    public class MerchantRequisitionReportController : MerchantBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        public void initpage()
        {
            try
            {
                ViewBag.ControllerName = "Merchant Dashboard";
                SystemClass sclass = new SystemClass();
                string userID = sclass.GetLoggedUser();
                long userid = long.Parse(userID);
                var dbmain = new DBContext();
                if (userID != null && userID != "")
                {
                    TBL_MASTER_MEMBER currUser = dbmain.TBL_MASTER_MEMBER.SingleOrDefault(c => c.MEM_ID == userid && c.MEMBER_ROLE == 5 && c.ACTIVE_MEMBER == true);
                    if (currUser != null)
                    {
                        Session["MerchantUserId"] = currUser.MEM_ID;
                        // Session["UserName"] = currUser.UserName;
                    }
                    else
                    {
                        Response.Redirect(Url.Action("Index", "MerchantLogin", new { area = "Merchant" }));
                        return;
                    }
                }
                if (Session["MerchantUserId"] == null)
                {
                    //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
                    Response.Redirect(Url.Action("Index", "MerchantLogin", new { area = "Merchant" }));
                    return;
                }
                bool Islogin = false;
                if (Session["MerchantUserId"] != null)
                {
                    Islogin = true;
                    ViewBag.CurrentUserId = CurrentMerchant.MEM_ID;
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

        // GET: Merchant/MerchantRequisitionReport
        public ActionResult Index()
        {
            if (Session["MerchantUserId"] != null)
            {
                var db = new DBContext();
                var memberService = (from x in db.TBL_MASTER_MEMBER
                                     where x.MEM_ID == CurrentMerchant.MEM_ID
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
                Session["MerchantUserId"] = null;
                Session["MerchantUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("MerchantUserId");
                Session.Remove("MerchantUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "MerchantLogin", new { area = "Merchant" });
            }
        }

        public PartialViewResult IndexGrid(string search = "")
        {
            var db = new DBContext();
            if (search == "")
            {
                var transactionlistvalue = MerchantRequisitionReportViewModel.GetMerchantAllRequisitionReport(CurrentMerchant.MEM_ID.ToString(),search);
                return PartialView("IndexGrid", transactionlistvalue);
            }
            else
            {
                //var transactionlistvalue = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                //                            join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                //                            where x.FROM_MEMBER == CurrentMerchant.MEM_ID && x.STATUS==search
                //                            select new
                //                            {
                //                                Touser = "Merchant",
                //                                transid = x.TransactionID,
                //                                FromUser = y.UName,
                //                                REQUEST_DATE = x.REQUEST_DATE,
                //                                AMOUNT = x.AMOUNT,
                //                                BANK_ACCOUNT = x.BANK_ACCOUNT,
                //                                TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                //                                STATUS = x.STATUS,
                //                                APPROVED_BY = x.APPROVED_BY,
                //                                APPROVAL_DATE = x.APPROVAL_DATE,
                //                                SLN = x.SLN
                //                            }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                //                            {
                //                                Serial_No = index + 1,
                //                                ToUser = z.Touser,
                //                                TransactionID = z.transid,
                //                                FromUser = z.FromUser,
                //                                AMOUNT = z.AMOUNT,
                //                                REQUEST_DATE = z.REQUEST_DATE,
                //                                BANK_ACCOUNT = z.BANK_ACCOUNT,
                //                                TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                //                                STATUS = z.STATUS,
                //                                APPROVED_BY = z.APPROVED_BY,
                //                                APPROVAL_DATE = z.APPROVAL_DATE,
                //                                SLN = z.SLN
                //                            }).ToList();
                var transactionlistvalue = MerchantRequisitionReportViewModel.GetMerchantAllRequisitionReport(CurrentMerchant.MEM_ID.ToString(), search);
                return PartialView("IndexGrid", transactionlistvalue);
            }
        }

        [HttpGet]
        public FileResult ExportMerchantReqIndex(string statusval)
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = CreateExportMerchantReqGrid(statusval);
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
        private IGrid<TBL_BALANCE_TRANSFER_LOGS> CreateExportMerchantReqGrid(string statusval)
        {
            var db = new DBContext();
            var transactionlistvalue = MerchantRequisitionReportViewModel.GetMerchantAllRequisitionReport(CurrentMerchant.MEM_ID.ToString(), statusval);
            IGrid<TBL_BALANCE_TRANSFER_LOGS> grid = new Grid<TBL_BALANCE_TRANSFER_LOGS>(transactionlistvalue);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.Serial_No).Titled("Se Id");
            //grid.Columns.Add(model => model.ToUser).Titled("To User");
            grid.Columns.Add(model => model.FromUser).Titled("User Name");
            grid.Columns.Add(model => model.REQUEST_DATE).Titled("Req Date").Formatted("{0:d}").MultiFilterable(true);
            grid.Columns.Add(model => model.REQUEST_TIME).Titled("Req Date").Formatted("{0:T}").MultiFilterable(true);
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

    }
}