using log4net;
using NonFactors.Mvc.Grid;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Controllers;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;

namespace WHITELABEL.Web.Areas.Super.Controllers
{
    [Authorize]
    public class SuperAccountDetailsController : SuperBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        //public void initpage()
        //{
        //    try
        //    {
        //        ViewBag.ControllerName = "Super Account Details";

        //        SystemClass sclass = new SystemClass();
        //        string userID = sclass.GetLoggedUser();
        //        long userid = long.Parse(userID);
        //        var dbmain = new DBContext();
        //        if (userID != null && userID != "")
        //        {
        //            TBL_MASTER_MEMBER currUser = dbmain.TBL_MASTER_MEMBER.SingleOrDefault(c => c.MEM_ID == userid && c.ACTIVE_MEMBER == true && c.MEMBER_ROLE==3);
        //            if (currUser != null)
        //            {
        //                Session["SuperDistributorId"] = currUser.MEM_ID;
        //                // Session["UserName"] = currUser.UserName;
        //            }
        //            else
        //            {
        //                Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //                return;
        //            }
        //        }
        //        if (Session["SuperDistributorId"] == null)
        //        {
        //            //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //            //Response.Redirect(Url.Action("Index", "StockistDashboard", new { area = "SuperStockist" }));
        //            Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //            return;
        //        }
        //        bool Islogin = false;

        //        if (Session["SuperDistributorId"] != null)
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
                ViewBag.ControllerName = "Super";
                if (Session["SuperDistributorId"] == null)
                {
                    Response.Redirect(Url.Action("Logout", "Login", new { area = "" }));
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
                Console.WriteLine(e.InnerException);
                return;
            }
        }

        // GET: Super/SuperAccountDetails
        public ActionResult Index()
        {
            if (Session["SuperDistributorId"] != null)
            {
                initpage();
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
        public PartialViewResult IndexGrid()
        {
            try
            {
                //var db = new DBContext();
                //var accountstatement = (from mem in db.TBL_MASTER_MEMBER
                //                        join ac in db.TBL_ACCOUNTS on mem.MEM_ID equals ac.MEM_ID
                //                        join role in db.TBL_MASTER_MEMBER_ROLE on mem.MEMBER_ROLE equals role.ROLE_ID
                //                        select new
                //                        {
                //                            sln = ac.ACC_NO,
                //                            username = mem.UName,
                //                            memberType = ac.MEMBER_TYPE,
                //                            transtype = ac.TRANSACTION_TYPE,
                //                            transdate = ac.TRANSACTION_DATE,
                //                            transtime = ac.TRANSACTION_TIME,
                //                            TransDR_CR = ac.DR_CR,
                //                            amount = ac.AMOUNT,
                //                            Opening = ac.OPENING,
                //                            Closing = ac.CLOSING,
                //                            RecNo = ac.REC_NO,
                //                            Narration = ac.NARRATION
                //                        }).AsEnumerable().Select(z => new TBL_ACCOUNTS()
                //                        {
                //                            ACC_NO = z.sln,
                //                            UserName = z.username,
                //                            MEMBER_TYPE = z.memberType,
                //                            TRANSACTION_TYPE = z.transtype,
                //                            TRANSACTION_DATE = z.transdate,
                //                            timevalue = z.transtime.ToString("HH:mm:ss"),
                //                            DR_CR = z.TransDR_CR,
                //                            AMOUNT = z.amount,
                //                            OPENING = z.Opening,
                //                            CLOSING = z.Closing,
                //                            REC_NO = z.RecNo,
                //                            NARRATION = z.Narration
                //                        }).ToList();

                //return PartialView(accountstatement);
                return PartialView(CreateExportableGrid());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult CompanyAccountStatement()
        {
            //return View();
            if (Session["SuperDistributorId"] != null)
            {
                initpage();
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
                //return File(fileContents: package.GetAsByteArray(), contentType: "application/unknown");
            }
        }

        private IGrid<TBL_ACCOUNTS> CreateExportableGrid()
        {
            var db = new DBContext();
            var accountstatement = (from mem in db.TBL_MASTER_MEMBER
                                    join ac in db.TBL_ACCOUNTS on mem.MEM_ID equals ac.MEM_ID
                                    join role in db.TBL_MASTER_MEMBER_ROLE on mem.MEMBER_ROLE equals role.ROLE_ID
                                    where mem.CREATED_BY == MemberCurrentUser.MEM_ID
                                    select new
                                    {
                                        sln = ac.ACC_NO,
                                        username = mem.UName,
                                        memberType = ac.MEMBER_TYPE,
                                        transtype = ac.TRANSACTION_TYPE,
                                        transdate = ac.TRANSACTION_DATE,
                                        transtime = ac.TRANSACTION_TIME,
                                        TransDR_CR = ac.DR_CR,
                                        amount = ac.AMOUNT,
                                        Opening = ac.OPENING,
                                        Closing = ac.CLOSING,
                                        RecNo = ac.REC_NO,
                                        Narration = ac.NARRATION,
                                        GST=ac.GST,
                                        TDS=ac.TDS
                                    }).AsEnumerable().Select(z => new TBL_ACCOUNTS()
                                    {
                                        ACC_NO = z.sln,
                                        UserName = z.username,
                                        MEMBER_TYPE = z.memberType,
                                        TRANSACTION_TYPE = z.transtype,
                                        TRANSACTION_DATE = z.transdate,
                                        timevalue = z.transtime.ToString("HH:mm:ss"),
                                        DR_CR = z.TransDR_CR,
                                        AMOUNT = z.amount,
                                        OPENING = z.Opening,
                                        CLOSING = z.Closing,
                                        REC_NO = z.RecNo,
                                        NARRATION = z.Narration,
                                        GST = z.GST,
                                        TDS = z.TDS
                                    }).ToList();


            IGrid<TBL_ACCOUNTS> grid = new Grid<TBL_ACCOUNTS>(accountstatement);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.ACC_NO).Titled("Trans Id");
            grid.Columns.Add(model => model.UserName).Titled("User Name");
            grid.Columns.Add(model => model.MEMBER_TYPE).Titled("Member");

            grid.Columns.Add(model => model.TRANSACTION_TYPE).Titled("Trans Type");
            grid.Columns.Add(model => model.AMOUNT).Titled("AMOUNT");
            grid.Columns.Add(model => model.TRANSACTION_DATE).Titled("Trans Date").Formatted("{0:d}").MultiFilterable(true);
            grid.Columns.Add(model => model.timevalue).Titled("Time").Formatted("{0:d}").MultiFilterable(true); ;
            grid.Columns.Add(model => model.DR_CR).Titled("DR_CR");
            grid.Columns.Add(model => model.AMOUNT).Titled("AMOUNT");
            grid.Columns.Add(model => model.OPENING).Titled("OPENING");
            grid.Columns.Add(model => model.CLOSING).Titled("CLOSING");

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