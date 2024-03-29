﻿using log4net;
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
using WHITELABEL.Web.Areas.Distributor.Models;
using WHITELABEL.Web.Controllers;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;
using WHITELABEL.Web.Models;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace WHITELABEL.Web.Areas.Distributor.Controllers
{
    [Authorize]
    public class DistributorTransactionReportController : DistributorBaseController
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
        //            TBL_MASTER_MEMBER currUser = dbmain.TBL_MASTER_MEMBER.SingleOrDefault(c => c.MEM_ID == userid && c.ACTIVE_MEMBER == true && c.MEMBER_ROLE == 4);
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

        // GET: Distributor/DistributorTransactionReport
        public ActionResult Index()
        {
            if (Session["DistributorUserId"] != null)
            {
                initpage();
                var db = new DBContext();
                var memberService = (from x in db.TBL_MASTER_MEMBER
                                     where x.INTRODUCER == MemberCurrentUser.MEM_ID
                                     select new
                                     {
                                         MEM_ID = x.MEM_ID,
                                         UName = x.UName,
                                         MObileNo=x.MEMBER_MOBILE
                                     }).AsEnumerable().Select(z => new MemberView
                                     {
                                         IDValue = z.MEM_ID.ToString(),
                                         TextValue = z.UName+"-"+z.MObileNo
                                     }).ToList().Distinct();
                ViewBag.MemberService = new SelectList(memberService, "IDValue", "TextValue");
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
        public PartialViewResult IndexGrid(string search = "", string Status = "", string DateFrom = "", string Date_To = "")
        {
            initpage();
            var db = new DBContext();
            if (search == "" && Status == "")
            {
                var transactionlistvalue = DistributorTransactionReportModel.GetDistributorUnderAllRetailerCommission(MemberCurrentUser.MEM_ID.ToString(), Status, DateFrom, Date_To);
                return PartialView("IndexGrid", transactionlistvalue);
            }
            else if (search == "" && Status != "")
            {
                var transactionlistvalue = DistributorTransactionReportModel.GetDistributorUnderAllRetailerCommission(MemberCurrentUser.MEM_ID.ToString(), Status, DateFrom, Date_To);
                return PartialView("IndexGrid", transactionlistvalue);
            }
            else if (search != "" && Status != "")
            {
                var transactionlistvalue = DistributorTransactionReportModel.GetDistributorUnderRetailerCommission(search, Status, DateFrom, Date_To);
                return PartialView("IndexGrid", transactionlistvalue);
            }
            else if (search != "" && Status == "")
            {
                var transactionlistvalue = DistributorTransactionReportModel.GetDistributorUnderRetailerCommission(search, Status, DateFrom, Date_To);
                return PartialView("IndexGrid", transactionlistvalue);
            }
            else
            {
                return PartialView("IndexGrid", "");
            }
            //if (search != "")
            //{
            //    if (Status == "Requisition")   //Mobile Recharge,  DMR
            //    {
            //        Status = "";
            //    }
            //    else
            //    {
            //        Status = Status;
            //    }

            //    long Mer_id = long.Parse(search.ToString());
            //    long SerialNo = 0;
            //    if (Status == "Mobile Recharge" || Status == "DMR")
            //    {
            //        var transactionlistvalue = (from x in db.TBL_ACCOUNTS
            //                                    join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
            //                                    where y.INTRODUCER == MemberCurrentUser.MEM_ID && (x.MEM_ID == Mer_id && x.TRANSACTION_TYPE == Status) || (x.MEM_ID == MemberCurrentUser.MEM_ID && x.TRANSACTION_TYPE == Status)
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
            //        return PartialView("IndexGrid", transactionlistvalue);
            //    }
                
            //    else if (Status == "Requisition")
            //    {
            //        var transactionlistvalue = (from x in db.TBL_ACCOUNTS
            //                                    join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
            //                                    where y.INTRODUCER == MemberCurrentUser.MEM_ID && (x.MEM_ID == Mer_id && x.TRANSACTION_TYPE != "DMR" && x.TRANSACTION_TYPE != "Mobile Recharge") || (x.MEM_ID == MemberCurrentUser.MEM_ID && x.TRANSACTION_TYPE != "DMR" && x.TRANSACTION_TYPE != "Mobile Recharge")
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
            //        return PartialView("IndexGrid", transactionlistvalue);
            //    }
            //    else
            //    {
            //        var transactionlistvalue = (from x in db.TBL_ACCOUNTS
            //                                    join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
            //                                    where y.INTRODUCER == MemberCurrentUser.MEM_ID && (x.MEM_ID == Mer_id) || (x.MEM_ID == MemberCurrentUser.MEM_ID)
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
            //        return PartialView("IndexGrid", transactionlistvalue);
            //    }
            //}
            //else
            //{

            //    if (Status == "Mobile Recharge" || Status == "DMR")
            //    {
            //        var transactionlistvalue = (from x in db.TBL_ACCOUNTS
            //                                    join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
            //                                    where y.INTRODUCER == MemberCurrentUser.MEM_ID && x.TRANSACTION_TYPE == Status
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
            //        return PartialView("IndexGrid", transactionlistvalue);
            //    }
            //    else if (Status == "Requisition")
            //    {
            //        var transactionlistvalue = (from x in db.TBL_ACCOUNTS
            //                                    join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
            //                                    where y.INTRODUCER == MemberCurrentUser.MEM_ID && x.TRANSACTION_TYPE != "DMR" && x.TRANSACTION_TYPE != "Mobile Recharge"
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
            //        return PartialView("IndexGrid", transactionlistvalue);
            //    }
            //    else
            //    {
            //        var transactionlistvalue = (from x in db.TBL_ACCOUNTS
            //                                    join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
            //                                    where y.INTRODUCER == MemberCurrentUser.MEM_ID || (x.MEM_ID == MemberCurrentUser.MEM_ID)
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
            //        return PartialView("IndexGrid", transactionlistvalue);
            //    }
            //}
            //var transactionlistvalue = null;
        }

        [HttpGet]
        public FileResult ExportIndexDisDistributorAdminReport(string Disid, string statusval, string DateFrom = "", string Date_To = "")
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;
                var db = new DBContext();
                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_ACCOUNTS> grid = CreateExportDisDistributorTableGrid(Disid, statusval, DateFrom, Date_To);
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
        private IGrid<TBL_ACCOUNTS> CreateExportDisDistributorTableGrid(string Disid, string statusval,string DateFrom,string Date_To)
        {
            var db = new DBContext();
            var MerchantList = new List<TBL_ACCOUNTS>();
            if (Disid == "" && statusval == "")
            {
                //long mem_id = long.Parse(Disid.ToString());
                MerchantList = DistributorTransactionReportModel.GetDistributorUnderAllRetailerCommission(MemberCurrentUser.MEM_ID.ToString(), statusval, DateFrom, Date_To);
            }
            else if (Disid == "" && statusval != "")
            {
                //long mem_id = long.Parse(Disid.ToString());
                MerchantList = DistributorTransactionReportModel.GetDistributorUnderAllRetailerCommission(MemberCurrentUser.MEM_ID.ToString(), statusval, DateFrom, Date_To);
            }
            else if (Disid != "" && statusval == "")
            {
                //long mem_id = long.Parse(Disid.ToString());
                MerchantList = DistributorTransactionReportModel.GetDistributorUnderRetailerCommission(Disid, statusval, DateFrom, Date_To);
            }
            else if (Disid != "" && statusval != "")
            {
                //long mem_id = long.Parse(Disid.ToString());
                MerchantList = DistributorTransactionReportModel.GetDistributorUnderRetailerCommission(Disid, statusval, DateFrom, Date_To);
            }
            IGrid<TBL_ACCOUNTS> grid = new Grid<TBL_ACCOUNTS>(MerchantList);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.SerialNo).Titled("Sln No.");
            grid.Columns.Add(model => model.UserName).Titled("User Name");
            //grid.Columns.Add(model => model.MEMBER_TYPE).Titled("Merchant Type");
            grid.Columns.Add(model => model.TRANSACTION_TYPE).Titled("Transaction Type");
            grid.Columns.Add(model => model.TRANSACTION_DATE).Titled("Transaction Date").Formatted("{0:yyyy-MM-dd}");
            //grid.Columns.Add(model => model.DR_CR).Titled("DR/CR");
            grid.Columns.Add(model => model.OPENING).Titled("Opening");
            grid.Columns.Add(model => model.CR_Col).Titled("CR");
            grid.Columns.Add(model => model.DR_Col).Titled("DR");
            //grid.Columns.Add(model => model.AMOUNT).Titled("Amount");
            //grid.Columns.Add(model => model.NARRATION).Titled("Narration");
            grid.Columns.Add(model => model.CLOSING).Titled("Closing");
            grid.Columns.Add(model => model.COMM_AMT).Titled("Commission Amt.");
            grid.Pager = new GridPager<TBL_ACCOUNTS>(grid);
            grid.Processors.Add(grid.Pager);
            grid.Pager.RowsPerPage = 10000000;

            foreach (IGridColumn column in grid.Columns)
            {
                column.Filter.IsEnabled = true;
                column.Sort.IsEnabled = true;
            }

            return grid;

        }

        public ActionResult DistributorCommissionReport()
        {
            if (Session["DistributorUserId"] != null)
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
                Session["DistributorUserId"] = null;
                Session["DistributorUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("DistributorUserId");
                Session.Remove("DistributorUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
            
        }
        public PartialViewResult DistributorCommissionGrid( string status = "",string DateFrom="",string Date_To="")
        {
            var transactionlistvalue = DistributorTransactionReportModel.GetDistributorCommission(MemberCurrentUser.MEM_ID.ToString(), status, DateFrom, Date_To);
            return PartialView("DistributorCommissionGrid", transactionlistvalue);
        }
        [HttpGet]
        public FileResult ExportIndexOnlyDistributorAdminReport(string statusval, string DateFrom = "", string Date_To = "")
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;
                var db = new DBContext();


                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_ACCOUNTS> grid = CreateExportOnlyDistributorTableGrid(statusval, DateFrom, Date_To);
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
        private IGrid<TBL_ACCOUNTS> CreateExportOnlyDistributorTableGrid(string statusval, string DateFrom = "", string Date_To = "")
        {
            var db = new DBContext();
            //var MerchantList = new List<TBL_ACCOUNTS>();
            var MerchantList =  DistributorTransactionReportModel.GetDistributorCommission(MemberCurrentUser.MEM_ID.ToString(), statusval, DateFrom, Date_To);
            IGrid<TBL_ACCOUNTS> grid = new Grid<TBL_ACCOUNTS>(MerchantList);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.SerialNo).Titled("Sln No.");
            grid.Columns.Add(model => model.UserName).Titled("User Name");
            //grid.Columns.Add(model => model.MEMBER_TYPE).Titled("Merchant Type");
            grid.Columns.Add(model => model.TRANSACTION_TYPE).Titled("Transaction Type");
            grid.Columns.Add(model => model.TRANSACTION_DATE).Titled("Transaction Date").Formatted("{0:yyyy-MM-dd}");
            //grid.Columns.Add(model => model.DR_CR).Titled("DR/CR");
            //grid.Columns.Add(model => model.AMOUNT).Titled("Amount");
            //grid.Columns.Add(model => model.NARRATION).Titled("Narration");
            grid.Columns.Add(model => model.OPENING).Titled("Opening");
            grid.Columns.Add(model => model.CR_Col).Titled("CR");
            grid.Columns.Add(model => model.DR_Col).Titled("DR");
            grid.Columns.Add(model => model.CLOSING).Titled("Closing");
            grid.Columns.Add(model => model.COMM_AMT).Titled("Commission Amt.");
            grid.Pager = new GridPager<TBL_ACCOUNTS>(grid);
            grid.Processors.Add(grid.Pager);
            //grid.Pager.RowsPerPage = 6;
            grid.Pager.RowsPerPage = 10000000;

            foreach (IGridColumn column in grid.Columns)
            {
                column.Filter.IsEnabled = true;
                column.Sort.IsEnabled = true;
            }

            return grid;

        }
        public ActionResult DistributorDailyTransactionReport()
        {
            initpage();
            return View();
        }
        public PartialViewResult DailyTransIndexGrid(string search = "", string Status = "")
        {
            initpage();
            var db = new DBContext();
            if (search != "")
            {
                if (Status == "Requisition")   //Mobile Recharge,  DMR
                {
                    Status = "";
                }
                else
                {
                    Status = Status;
                }

                long Mer_id = long.Parse(search.ToString());
                long SerialNo = 0;
                if (Status == "Mobile Recharge" || Status == "DMR")
                {
                    var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                                join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                                where y.INTRODUCER == MemberCurrentUser.MEM_ID && (x.MEM_ID == Mer_id && x.TRANSACTION_TYPE == Status) || (x.MEM_ID == MemberCurrentUser.MEM_ID && x.TRANSACTION_TYPE == Status)
                                                select new
                                                {
                                                    SLN = x.ACC_NO,
                                                    MerchantName = y.UName,
                                                    MemberType = x.MEMBER_TYPE,
                                                    Trans_Type = x.TRANSACTION_TYPE,
                                                    Trans_Date = x.TRANSACTION_DATE,
                                                    DR_CR = x.DR_CR,
                                                    Amount = x.AMOUNT,
                                                    Narration = x.NARRATION,
                                                    OpeningAmt = x.OPENING,
                                                    Closing = x.CLOSING,
                                                    CommissionAmt = x.COMM_AMT
                                                }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                                {
                                                    SerialNo = index + 1,
                                                    ACC_NO = z.SLN,
                                                    UserName = z.MerchantName,
                                                    MEMBER_TYPE = z.MemberType,
                                                    TRANSACTION_TYPE = z.Trans_Type,
                                                    TRANSACTION_DATE = z.Trans_Date,
                                                    DR_CR = z.DR_CR,
                                                    AMOUNT = z.Amount,
                                                    NARRATION = z.Narration,
                                                    OPENING = z.OpeningAmt,
                                                    CLOSING = z.Closing,
                                                    COMM_AMT = z.CommissionAmt
                                                }).ToList();
                    return PartialView("DailyTransIndexGrid", transactionlistvalue);
                }

                else if (Status == "DailyTransIndexGrid")
                {
                    var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                                join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                                where y.INTRODUCER == MemberCurrentUser.MEM_ID && (x.MEM_ID == Mer_id && x.TRANSACTION_TYPE != "DMR" && x.TRANSACTION_TYPE != "Mobile Recharge") || (x.MEM_ID == MemberCurrentUser.MEM_ID && x.TRANSACTION_TYPE != "DMR" && x.TRANSACTION_TYPE != "Mobile Recharge")
                                                select new
                                                {
                                                    SLN = x.ACC_NO,
                                                    MerchantName = y.UName,
                                                    MemberType = x.MEMBER_TYPE,
                                                    Trans_Type = x.TRANSACTION_TYPE,
                                                    Trans_Date = x.TRANSACTION_DATE,
                                                    DR_CR = x.DR_CR,
                                                    Amount = x.AMOUNT,
                                                    Narration = x.NARRATION,
                                                    OpeningAmt = x.OPENING,
                                                    Closing = x.CLOSING,
                                                    CommissionAmt = x.COMM_AMT
                                                }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                                {
                                                    SerialNo = index + 1,
                                                    ACC_NO = z.SLN,
                                                    UserName = z.MerchantName,
                                                    MEMBER_TYPE = z.MemberType,
                                                    TRANSACTION_TYPE = z.Trans_Type,
                                                    TRANSACTION_DATE = z.Trans_Date,
                                                    DR_CR = z.DR_CR,
                                                    AMOUNT = z.Amount,
                                                    NARRATION = z.Narration,
                                                    OPENING = z.OpeningAmt,
                                                    CLOSING = z.Closing,
                                                    COMM_AMT = z.CommissionAmt
                                                }).ToList();
                    return PartialView("DailyTransIndexGrid", transactionlistvalue);
                }
                else
                {
                    var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                                join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                                where y.INTRODUCER == MemberCurrentUser.MEM_ID && (x.MEM_ID == Mer_id) || (x.MEM_ID == MemberCurrentUser.MEM_ID)
                                                select new
                                                {
                                                    SLN = x.ACC_NO,
                                                    MerchantName = y.UName,
                                                    MemberType = x.MEMBER_TYPE,
                                                    Trans_Type = x.TRANSACTION_TYPE,
                                                    Trans_Date = x.TRANSACTION_DATE,
                                                    DR_CR = x.DR_CR,
                                                    Amount = x.AMOUNT,
                                                    Narration = x.NARRATION,
                                                    OpeningAmt = x.OPENING,
                                                    Closing = x.CLOSING,
                                                    CommissionAmt = x.COMM_AMT
                                                }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                                {
                                                    SerialNo = index + 1,
                                                    ACC_NO = z.SLN,
                                                    UserName = z.MerchantName,
                                                    MEMBER_TYPE = z.MemberType,
                                                    TRANSACTION_TYPE = z.Trans_Type,
                                                    TRANSACTION_DATE = z.Trans_Date,
                                                    DR_CR = z.DR_CR,
                                                    AMOUNT = z.Amount,
                                                    NARRATION = z.Narration,
                                                    OPENING = z.OpeningAmt,
                                                    CLOSING = z.Closing,
                                                    COMM_AMT = z.CommissionAmt
                                                }).ToList();
                    return PartialView("DailyTransIndexGrid", transactionlistvalue);
                }
            }
            else
            {

                if (Status == "Mobile Recharge" || Status == "DMR")
                {
                    var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                                join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                                where y.INTRODUCER == MemberCurrentUser.MEM_ID && x.TRANSACTION_TYPE == Status
                                                select new
                                                {
                                                    SLN = x.ACC_NO,
                                                    MerchantName = y.UName,
                                                    MemberType = x.MEMBER_TYPE,
                                                    Trans_Type = x.TRANSACTION_TYPE,
                                                    Trans_Date = x.TRANSACTION_DATE,
                                                    DR_CR = x.DR_CR,
                                                    Amount = x.AMOUNT,
                                                    Narration = x.NARRATION,
                                                    OpeningAmt = x.OPENING,
                                                    Closing = x.CLOSING,
                                                    CommissionAmt = x.COMM_AMT
                                                }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                                {
                                                    SerialNo = index + 1,
                                                    ACC_NO = z.SLN,
                                                    UserName = z.MerchantName,
                                                    MEMBER_TYPE = z.MemberType,
                                                    TRANSACTION_TYPE = z.Trans_Type,
                                                    TRANSACTION_DATE = z.Trans_Date,
                                                    DR_CR = z.DR_CR,
                                                    AMOUNT = z.Amount,
                                                    NARRATION = z.Narration,
                                                    OPENING = z.OpeningAmt,
                                                    CLOSING = z.Closing,
                                                    COMM_AMT = z.CommissionAmt
                                                }).ToList();
                    return PartialView("DailyTransIndexGrid", transactionlistvalue);
                }
                else if (Status == "Requisition")
                {
                    var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                                join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                                where y.INTRODUCER == MemberCurrentUser.MEM_ID && x.TRANSACTION_TYPE != "DMR" && x.TRANSACTION_TYPE != "Mobile Recharge"
                                                select new
                                                {
                                                    SLN = x.ACC_NO,
                                                    MerchantName = y.UName,
                                                    MemberType = x.MEMBER_TYPE,
                                                    Trans_Type = x.TRANSACTION_TYPE,
                                                    Trans_Date = x.TRANSACTION_DATE,
                                                    DR_CR = x.DR_CR,
                                                    Amount = x.AMOUNT,
                                                    Narration = x.NARRATION,
                                                    OpeningAmt = x.OPENING,
                                                    Closing = x.CLOSING,
                                                    CommissionAmt = x.COMM_AMT
                                                }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                                {
                                                    SerialNo = index + 1,
                                                    ACC_NO = z.SLN,
                                                    UserName = z.MerchantName,
                                                    MEMBER_TYPE = z.MemberType,
                                                    TRANSACTION_TYPE = z.Trans_Type,
                                                    TRANSACTION_DATE = z.Trans_Date,
                                                    DR_CR = z.DR_CR,
                                                    AMOUNT = z.Amount,
                                                    NARRATION = z.Narration,
                                                    OPENING = z.OpeningAmt,
                                                    CLOSING = z.Closing,
                                                    COMM_AMT = z.CommissionAmt
                                                }).ToList();
                    return PartialView("DailyTransIndexGrid", transactionlistvalue);
                }
                else
                {
                    var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                                join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                                where y.INTRODUCER == MemberCurrentUser.MEM_ID || (x.MEM_ID == MemberCurrentUser.MEM_ID)
                                                select new
                                                {
                                                    SLN = x.ACC_NO,
                                                    MerchantName = y.UName,
                                                    MemberType = x.MEMBER_TYPE,
                                                    Trans_Type = x.TRANSACTION_TYPE,
                                                    Trans_Date = x.TRANSACTION_DATE,
                                                    DR_CR = x.DR_CR,
                                                    Amount = x.AMOUNT,
                                                    Narration = x.NARRATION,
                                                    OpeningAmt = x.OPENING,
                                                    Closing = x.CLOSING,
                                                    CommissionAmt = x.COMM_AMT
                                                }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                                {
                                                    SerialNo = index + 1,
                                                    ACC_NO = z.SLN,
                                                    UserName = z.MerchantName,
                                                    MEMBER_TYPE = z.MemberType,
                                                    TRANSACTION_TYPE = z.Trans_Type,
                                                    TRANSACTION_DATE = z.Trans_Date,
                                                    DR_CR = z.DR_CR,
                                                    AMOUNT = z.Amount,
                                                    NARRATION = z.Narration,
                                                    OPENING = z.OpeningAmt,
                                                    CLOSING = z.Closing,
                                                    COMM_AMT = z.CommissionAmt
                                                }).ToList();
                    return PartialView("DailyTransIndexGrid", transactionlistvalue);
                }
            }
        }


        public ActionResult GetDatewiseClosingReport()
        {
            if (Session["DistributorUserId"] != null)
            {                
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
        public PartialViewResult IndexGridGetDatewiseClosingReport()
        {
            initpage();
            var db = new DBContext();
            DateTime Dateval = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
            DateTime Val_date = Convert.ToDateTime(DateTime.Now.AddDays(-1));
            DateTime PreviousDate = Convert.ToDateTime(Val_date.ToString("yyyy-MM-dd"));

            List<DistributorAccouontsModel> AcoountsDetailsList = new List<DistributorAccouontsModel>();
            DataTable dtCSV = new DataTable();
            string connstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection con = new SqlConnection(connstring);
            ////string queryVal = "select t.ACC_NO,t.MEM_ID, t.TRANSACTION_TIME,t.OPENING ,t.AMOUNT, t.CLOSING from ACCOUNTS t inner join ( select MEM_ID, max(TRANSACTION_TIME) as MaxDate from ACCOUNTS  group by MEM_ID) tm on t.MEM_ID = tm.MEM_ID and t.TRANSACTION_TIME = tm.MaxDate where t.TRANSACTION_DATE = '2020-03-14'";
            string queryVal = "select t.ACC_NO,t.MEM_ID, t.TRANSACTION_TIME,t.OPENING ,t.AMOUNT, t.CLOSING from ACCOUNTS t inner join ( select MEM_ID, max(TRANSACTION_TIME) as MaxDate from ACCOUNTS  group by MEM_ID) tm on t.MEM_ID = tm.MEM_ID and t.TRANSACTION_TIME = tm.MaxDate where t.TRANSACTION_DATE = '"+ PreviousDate + "'";

            //string queryVal = "select t.ACC_NO,t.MEM_ID, t.TRANSACTION_TIME,t.OPENING ,t.AMOUNT, t.CLOSING from ACCOUNTS t inner join ( select MEM_ID, max(TRANSACTION_TIME) as MaxDate from ACCOUNTS  group by MEM_ID) tm on t.MEM_ID = tm.MEM_ID and t.TRANSACTION_TIME = tm.MaxDate where t.TRANSACTION_DATE = '" + Dateval + "'";
            SqlDataAdapter data = new SqlDataAdapter(queryVal, con);
            data.SelectCommand.CommandTimeout = 2000;
            data.Fill(dtCSV);
            if (dtCSV.Rows.Count > 0)
            {
                long MEM_IDval = 0;
                decimal OpeningAmt = 0;
                decimal ClosinggAmt = 0;
                decimal Tran_amt = 0;
                long SlNo = 0;
                for (int i = 0; i < dtCSV.Rows.Count; i++)
                {
                    MEM_IDval = long.Parse(dtCSV.Rows[i][1].ToString());
                    OpeningAmt = decimal.Parse(dtCSV.Rows[i][3].ToString());
                    Tran_amt = decimal.Parse(dtCSV.Rows[i][4].ToString());
                    ClosinggAmt = decimal.Parse(dtCSV.Rows[i][5].ToString());
                    SlNo = SlNo+ 1;
                    DistributorAccouontsModel objacnt = new DistributorAccouontsModel();
                    objacnt.UserName = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == MEM_IDval).MEM_UNIQUE_ID;
                    objacnt.MEMBER_Name= db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == MEM_IDval).MEMBER_NAME;
                    objacnt.COMPANYNAME = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == MEM_IDval).COMPANY;
                    objacnt.MEM_ID = MEM_IDval;
                    objacnt.OpeningAmt = OpeningAmt;
                    objacnt.Trans_Amt = Tran_amt;
                    objacnt.ClosingAmt = ClosinggAmt;
                    objacnt.Trans_Date = dtCSV.Rows[i][2].ToString();
                    objacnt.SlNo = SlNo;

                    AcoountsDetailsList.Add(objacnt);
                }
            }
            string val = string.Empty;
            return PartialView("IndexGridGetDatewiseClosingReport", AcoountsDetailsList);
        }

    }
}