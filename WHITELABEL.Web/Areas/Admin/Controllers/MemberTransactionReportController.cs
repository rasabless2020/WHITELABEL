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
using WHITELABEL.Web.Areas.Admin.Models;
using WHITELABEL.Web.Controllers;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;
namespace WHITELABEL.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class MemberTransactionReportController : AdminBaseController
    {
        // GET: Admin/MemberTransactionReport

        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        //public void initpage()
        //{
        //    try
        //    {
        //        ViewBag.ControllerName = "Member Requisition";

        //        SystemClass sclass = new SystemClass();
        //        string userID = sclass.GetLoggedUser();
        //        long userid = long.Parse(userID);
        //        var dbmain = new DBContext();
        //        if (userID != null && userID != "")
        //        {
        //            TBL_MASTER_MEMBER currUser = dbmain.TBL_MASTER_MEMBER.SingleOrDefault(c => c.MEM_ID == userid && c.ACTIVE_MEMBER == true && c.MEMBER_ROLE == 1);
        //            if (currUser != null)
        //            {
        //                //Session["UserId"] = currUser.MEM_ID;
        //                Session["WhiteLevelUserId"] = currUser.MEM_ID;
        //            }
        //        }
        //        if (Session["WhiteLevelUserId"] == null)
        //        {
        //            //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));

        //            Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //            return;
        //        }
        //        bool Islogin = false;
        //        if (Session["WhiteLevelUserId"] != null)
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
                ViewBag.ControllerName = "White Label";
                if (Session["WhiteLevelUserId"] == null)
                {
                    Response.Redirect(Url.Action("Logout", "Login", new { area = "" }));
                    return;
                }
                bool Islogin = false;

                if (Session["WhiteLevelUserId"] != null)
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

        public ActionResult Index()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                try
                {
                    initpage();
                    return View();
                }
                catch (Exception ex)
                {
                    Logger.Error("Controller:-  MemverRequisitionReport(Admin), method:- Index (GET) Line No:- 78", ex);
                    return RedirectToAction("Exception", "ErrorHandler", new { area = "" });
                    throw ex;
                }
            }
            else
            {
                Session["WhiteLevelUserId"] = null;
                Session["WhiteLevelUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("WhiteLevelUserId");
                Session.Remove("WhiteLevelUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        public PartialViewResult IndexGrid(string status="",string DateFrom="",string Date_To="")
        {
            var listCommission = AdminTransactionViewModel.GetAdminCommissionReport(MemberCurrentUser.MEM_ID.ToString(), status, DateFrom, Date_To);
            return PartialView("IndexGrid", listCommission);
        }

        public ActionResult SuperCommissionReport()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                try
                {
                    var db = new DBContext();
                    initpage();
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
                catch (Exception ex)
                {
                    Logger.Error("Controller:-  MemverRequisitionReport(Admin), method:- Index (GET) Line No:- 78", ex);
                    return RedirectToAction("Exception", "ErrorHandler", new { area = "" });
                    throw ex;
                }
            }
            else
            {
                Session["WhiteLevelUserId"] = null;
                Session["WhiteLevelUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("WhiteLevelUserId");
                Session.Remove("WhiteLevelUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        public PartialViewResult SuperCommissionGrid(string search="" , string status="")
        {
            if (string.IsNullOrEmpty(search) && string.IsNullOrEmpty(status))
            {
                var listCommission = AdminTransactionViewModel.GetAllSuperCommissionReport(MemberCurrentUser.MEM_ID.ToString(), status);
                return PartialView("SuperCommissionGrid", listCommission);
            }
            else if (string.IsNullOrEmpty(search) && !string.IsNullOrEmpty(status))
            {
                var listCommission = AdminTransactionViewModel.GetAllSuperCommissionReport(MemberCurrentUser.MEM_ID.ToString(), status);
                return PartialView("SuperCommissionGrid", listCommission);
            }  //
            else if (!string.IsNullOrEmpty(search) && string.IsNullOrEmpty(status))
            {
                var listCommission = AdminTransactionViewModel.GetSuperCommissionReport(search.ToString(), status);
                return PartialView("SuperCommissionGrid", listCommission);
            }
            else if (!string.IsNullOrEmpty(search) && !string.IsNullOrEmpty(status))
            {
                var listCommission = AdminTransactionViewModel.GetSuperCommissionReport(search.ToString(), status);
                return PartialView("SuperCommissionGrid", listCommission);
            }
            return PartialView();
        }

        public ActionResult DistributorCommissionReport()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                try
                {
                    initpage();
                    var db = new DBContext();
                    var memberService = (from x in db.TBL_MASTER_MEMBER
                                         where x.INTRODUCER==MemberCurrentUser.MEM_ID
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
                    //string[] SuperMemId = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == MemberCurrentUser.MEM_ID).Select(a => a.MEM_ID.ToString()).ToArray();
                    //string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(x => SuperMemId.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                    //var memberService = (from x in db.TBL_MASTER_MEMBER
                    //                     where SuperMemId.Contains(x.INTRODUCER.ToString())
                    //                     select new
                    //                     {
                    //                         MEM_ID = x.MEM_ID,
                    //                         UName = x.UName
                    //                     }).AsEnumerable().Select(z => new MemberView
                    //                     {
                    //                         IDValue = z.MEM_ID.ToString(),
                    //                         TextValue = z.UName
                    //                     }).ToList().Distinct();
                    //ViewBag.MemberService = new SelectList(memberService, "IDValue", "TextValue");
                    return View();
                }
                catch (Exception ex)
                {
                    Logger.Error("Controller:-  MemverRequisitionReport(Admin), method:- Index (GET) Line No:- 78", ex);
                    return RedirectToAction("Exception", "ErrorHandler", new { area = "" });
                    throw ex;
                }
            }
            else
            {
                Session["WhiteLevelUserId"] = null;
                Session["WhiteLevelUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("WhiteLevelUserId");
                Session.Remove("WhiteLevelUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        public PartialViewResult DistributorCommissionGrid(string Super="", string search = "", string status = "",string DateFrom = "",string Date_To = "")
        {
            if (string.IsNullOrEmpty(Super)&& string.IsNullOrEmpty(search) && string.IsNullOrEmpty(status))
            {
                var listCommission = AdminTransactionViewModel.GetAllDistributorCommissionReport(MemberCurrentUser.MEM_ID.ToString(), status, DateFrom, Date_To);
                return PartialView("DistributorCommissionGrid", listCommission);
            }
            else if (string.IsNullOrEmpty(Super) && string.IsNullOrEmpty(search) && !string.IsNullOrEmpty(status))
            {
                var listCommission = AdminTransactionViewModel.GetAllDistributorCommissionReport(MemberCurrentUser.MEM_ID.ToString(), status, DateFrom, Date_To);
                return PartialView("DistributorCommissionGrid", listCommission);
            }
            else if (!string.IsNullOrEmpty(Super) && !string.IsNullOrEmpty(search) && string.IsNullOrEmpty(status))
            {
                var listCommission = AdminTransactionViewModel.GetDistributorCommissionReport(Super,search.ToString(), status, DateFrom, Date_To);
                return PartialView("DistributorCommissionGrid", listCommission);
            }
            else if (!string.IsNullOrEmpty(Super) && !string.IsNullOrEmpty(search) && !string.IsNullOrEmpty(status))
            {
                var listCommission = AdminTransactionViewModel.GetDistributorCommissionReport(Super,search.ToString(), status, DateFrom, Date_To);
                return PartialView("DistributorCommissionGrid", listCommission);
            }
            else if (!string.IsNullOrEmpty(Super) && string.IsNullOrEmpty(search) && string.IsNullOrEmpty(status))
            {
                var listCommission = AdminTransactionViewModel.GetDistributorCommissionReport(Super, search.ToString(), status, DateFrom, Date_To);
                return PartialView("DistributorCommissionGrid", listCommission);
            }
            else if (!string.IsNullOrEmpty(Super) && string.IsNullOrEmpty(search) && !string.IsNullOrEmpty(status))
            {
                var listCommission = AdminTransactionViewModel.GetDistributorCommissionReport(Super, search.ToString(), status, DateFrom, Date_To);
                return PartialView("DistributorCommissionGrid", listCommission);
            }
            else if (Super=="" && search=="0" && status  =="" && DateFrom != "" && Date_To!="")
            {
                var listCommission = AdminTransactionViewModel.GetDistributorCommissionReport(Super, search.ToString(), status, DateFrom, Date_To);
                return PartialView("DistributorCommissionGrid", listCommission);
            }
            else if (string.IsNullOrEmpty(Super) && !string.IsNullOrEmpty(search) && !string.IsNullOrEmpty(status))
            {
                var listCommission = AdminTransactionViewModel.GetAllDistributorCommissionReport(MemberCurrentUser.MEM_ID.ToString(), status, DateFrom, Date_To);
                return PartialView("DistributorCommissionGrid", listCommission);
            }
            //
            return PartialView();
        }

        public ActionResult MerchantCommission()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                try
                {
                    initpage();
                    var db = new DBContext();
                    //string[] SuperMemId = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == MemberCurrentUser.MEM_ID).Select(a => a.MEM_ID.ToString()).ToArray();
                    //string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(x => SuperMemId.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                    //var memberService = (from x in db.TBL_MASTER_MEMBER
                    //                     where DistributorMemId.Contains(x.INTRODUCER.ToString())
                    //                     select new
                    //                     {
                    //                         MEM_ID = x.MEM_ID,
                    //                         UName = x.UName
                    //                     }).AsEnumerable().Select(z => new MemberView
                    //                     {
                    //                         IDValue = z.MEM_ID.ToString(),
                    //                         TextValue = z.UName
                    //                     }).ToList().Distinct();
                    //ViewBag.MemberService = new SelectList(memberService, "IDValue", "TextValue");
                    var memberService = (from x in db.TBL_MASTER_MEMBER
                                         where x.INTRODUCER==MemberCurrentUser.MEM_ID
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
                catch (Exception ex)
                {
                    Logger.Error("Controller:-  MemverRequisitionReport(Admin), method:- Index (GET) Line No:- 78", ex);
                    return RedirectToAction("Exception", "ErrorHandler", new { area = "" });
                    throw ex;
                }
            }
            else
            {
                Session["WhiteLevelUserId"] = null;
                Session["WhiteLevelUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("WhiteLevelUserId");
                Session.Remove("WhiteLevelUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        public PartialViewResult MerchantCommissionGrid(string Super = "", string Distributor = "" , string search="", string Status = "", string DateFrom = "", string Date_To = "")
        {
            if (string.IsNullOrEmpty(Super) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(search) && string.IsNullOrEmpty(Status))
            {
                var MerchantList = AdminTransactionViewModel.GetAllMerchantCommissionReport(MemberCurrentUser.MEM_ID.ToString(), Status, DateFrom, Date_To);
                return PartialView("MerchantCommissionGrid", MerchantList);
            }
            else if (string.IsNullOrEmpty(Super) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(search) && !string.IsNullOrEmpty(Status))
            {
                var MerchantList = AdminTransactionViewModel.GetAllMerchantCommissionReport(MemberCurrentUser.MEM_ID.ToString(), Status, DateFrom, Date_To);
                return PartialView("MerchantCommissionGrid", MerchantList);
            }

            else if (!string.IsNullOrEmpty(Super) && !string.IsNullOrEmpty(Distributor) && !string.IsNullOrEmpty(search) && string.IsNullOrEmpty(Status))
            {
                var MerchantList = AdminTransactionViewModel.GetMerchantCommissionReport(Super, Distributor, search.ToString(), Status, DateFrom, Date_To);
                return PartialView("MerchantCommissionGrid", MerchantList);
            }
            else if (!string.IsNullOrEmpty(Super) && !string.IsNullOrEmpty(Distributor) && !string.IsNullOrEmpty(search) && !string.IsNullOrEmpty(Status))
            {
                var MerchantList = AdminTransactionViewModel.GetMerchantCommissionReport(Super, Distributor, search.ToString(), Status, DateFrom, Date_To);
                return PartialView("MerchantCommissionGrid", MerchantList);
            }

            else if (!string.IsNullOrEmpty(Super) && !string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(search) && string.IsNullOrEmpty(Status))
            {
                var MerchantList = AdminTransactionViewModel.GetMerchantCommissionReport(Super, Distributor, search.ToString(), Status, DateFrom, Date_To);
                return PartialView("MerchantCommissionGrid", MerchantList);
            }
            else if (!string.IsNullOrEmpty(Super) && !string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(search) && !string.IsNullOrEmpty(Status))
            {
                var MerchantList = AdminTransactionViewModel.GetMerchantCommissionReport(Super, Distributor, search.ToString(), Status, DateFrom, Date_To);
                return PartialView("MerchantCommissionGrid", MerchantList);
            }

            else if (!string.IsNullOrEmpty(Super) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(search) && string.IsNullOrEmpty(Status))
            {
                var MerchantList = AdminTransactionViewModel.GetMerchantCommissionReport(MemberCurrentUser.MEM_ID.ToString(), Distributor, search.ToString(), Status, DateFrom, Date_To);
                return PartialView("MerchantCommissionGrid", MerchantList);
            }
            else if (!string.IsNullOrEmpty(Super) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(search) && !string.IsNullOrEmpty(Status))
            {
                var MerchantList = AdminTransactionViewModel.GetMerchantCommissionReport(Super, Distributor, search.ToString(), Status, DateFrom, Date_To);
                return PartialView("MerchantCommissionGrid", MerchantList);
            }
            else if (Super=="0" && Distributor=="" && search=="" && Status=="" && DateFrom!="" && Date_To!="")
            {
                var MerchantList = AdminTransactionViewModel.GetMerchantCommissionReport(Super, Distributor, search.ToString(), Status, DateFrom, Date_To);
                return PartialView("MerchantCommissionGrid", MerchantList);
            }
            return PartialView();
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
        public FileResult ExportIndexMerchantReport(string Super,string Distributor, string Disid,string statusval, string DateFrom = "", string Date_To = "")
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;
                var db = new DBContext();
                

                package.Workbook.Worksheets.Add("Data");                
                IGrid<TBL_ACCOUNTS> grid = CreateExportableGrid(Super, Distributor, Disid, statusval, DateFrom, Date_To);
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
        private IGrid<TBL_ACCOUNTS> CreateExportableGrid(string Super, string Distributor, string Disid, string statusval, string DateFrom , string Date_To)
        {
            var db = new DBContext();
            string[] SuperMemId = null;
            string[] DistributorMemId = null;
            string[] MerchantMemId = null;
            var MerchantList = new List<TBL_ACCOUNTS>();
            if (string.IsNullOrEmpty(Super) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(Disid) && string.IsNullOrEmpty(statusval))
            {
                 MerchantList = AdminTransactionViewModel.GetAllMerchantCommissionReport(MemberCurrentUser.MEM_ID.ToString(), statusval, DateFrom, Date_To);                
            }
            else if (string.IsNullOrEmpty(Super) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(Disid) && !string.IsNullOrEmpty(statusval))
            {
                 MerchantList = AdminTransactionViewModel.GetAllMerchantCommissionReport(MemberCurrentUser.MEM_ID.ToString(), statusval,  DateFrom, Date_To);                
            }
            else if (!string.IsNullOrEmpty(Super) && !string.IsNullOrEmpty(Distributor) && !string.IsNullOrEmpty(Disid) && string.IsNullOrEmpty(statusval))
            {
                 MerchantList = AdminTransactionViewModel.GetMerchantCommissionReport(Super, Distributor, Disid.ToString(), statusval, DateFrom, Date_To);                
            }
            else if (!string.IsNullOrEmpty(Super) && !string.IsNullOrEmpty(Distributor) && !string.IsNullOrEmpty(Disid) && !string.IsNullOrEmpty(statusval))
            {
                 MerchantList = AdminTransactionViewModel.GetMerchantCommissionReport(Super, Distributor, Disid.ToString(), statusval, DateFrom, Date_To);                
            }
            else if (!string.IsNullOrEmpty(Super) && !string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(Disid) && string.IsNullOrEmpty(statusval))
            {
                 MerchantList = AdminTransactionViewModel.GetMerchantCommissionReport(Super, Distributor, Disid.ToString(), statusval, DateFrom, Date_To);                
            }
            else if (!string.IsNullOrEmpty(Super) && !string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(Disid) && !string.IsNullOrEmpty(statusval))
            {
                 MerchantList = AdminTransactionViewModel.GetMerchantCommissionReport(Super, Distributor, Disid.ToString(), statusval, DateFrom, Date_To);                
            }
            else if (!string.IsNullOrEmpty(Super) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(Disid) && string.IsNullOrEmpty(statusval))
            {
                 MerchantList = AdminTransactionViewModel.GetMerchantCommissionReport(MemberCurrentUser.MEM_ID.ToString(), Distributor, Disid.ToString(), statusval, DateFrom, Date_To);                
            }
            else if (!string.IsNullOrEmpty(Super) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(Disid) && !string.IsNullOrEmpty(statusval))
            {
                 MerchantList = AdminTransactionViewModel.GetMerchantCommissionReport(Super, Distributor, Disid.ToString(), statusval, DateFrom, Date_To);                
            }
            else if (Super == "0" && Distributor == "" && Disid == "" && statusval == "" && DateFrom != "" && Date_To != "")
            {
                MerchantList = AdminTransactionViewModel.GetMerchantCommissionReport(Super, Distributor, Disid.ToString(), statusval, DateFrom, Date_To);
                //return PartialView("MerchantCommissionGrid", MerchantList);
            }
            IGrid<TBL_ACCOUNTS> grid = new Grid<TBL_ACCOUNTS>(MerchantList);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;

            grid.Columns.Add(model => model.SerialNo).Titled("Sln No.");
            grid.Columns.Add(model => model.UserName).Titled("User Name");
            //grid.Columns.Add(model => model.MEMBER_TYPE).Titled("Merchant Type");
            grid.Columns.Add(model => model.TRANSACTION_TYPE).Titled("Transaction Type");
            grid.Columns.Add(model => model.TRANSACTION_DATE).Titled("Transaction Date").Formatted("{0:yyyy-MM-dd}");
            grid.Columns.Add(model => model.OPENING).Titled("Opening");
            grid.Columns.Add(model => model.CR_Col).Titled("Cr");
            grid.Columns.Add(model => model.DR_Col).Titled("Dr");
            //grid.Columns.Add(model => model.NARRATION).Titled("Narration");
            grid.Columns.Add(model => model.CLOSING).Titled("Closing");
            grid.Columns.Add(model => model.COMM_AMT).Titled("Commission Amt.");
            grid.Pager = new GridPager<TBL_ACCOUNTS>(grid);
            grid.Processors.Add(grid.Pager);
            //grid.Pager.RowsPerPage = 6;
            grid.Pager.RowsPerPage = 100000000;
            foreach (IGridColumn column in grid.Columns)
            {
                column.Filter.IsEnabled = true;
                column.Sort.IsEnabled = true;
            }

            return grid;


            //if (Disid == "" && statusval == "")
            //{
            //    long mem_id = long.Parse(MemberCurrentUser.MEM_ID.ToString());
            //    var MerchantList = AdminTransactionViewModel.GetAllMerchantCommissionReport(MemberCurrentUser.MEM_ID.ToString(), statusval);
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
            //else if (Disid == "" && statusval != "")
            //{
            //    long mem_id = long.Parse(MemberCurrentUser.MEM_ID.ToString());
            //    var MerchantList = AdminTransactionViewModel.GetAllMerchantCommissionReport(MemberCurrentUser.MEM_ID.ToString(), statusval);

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
            //else if (Disid != "" && statusval == "")
            //{
            //    long mem_id = long.Parse(Disid.ToString());
            //    var MerchantList = AdminTransactionViewModel.GetMerchantCommissionReport(mem_id.ToString(), statusval);

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
            //else if (Disid != "" && statusval != "")
            //{
            //    long mem_id = long.Parse(Disid.ToString());
            //    var MerchantList = AdminTransactionViewModel.GetMerchantCommissionReport(mem_id.ToString(), statusval);
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
            //    var MerchantList = AdminTransactionViewModel.GetAllMerchantCommissionReport(MemberCurrentUser.MEM_ID.ToString(), statusval);
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
        // Export Super 
        public FileResult ExportIndexSuperReport(string Disid, string statusval)
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;
                var db = new DBContext();


                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_ACCOUNTS> grid = CreateExportSuperTableGrid(Disid, statusval);
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
        private IGrid<TBL_ACCOUNTS> CreateExportSuperTableGrid(string Disid, string statusval)
        {
            var db = new DBContext();
            string[] SuperMemId = null;
            string[] DistributorMemId = null;
            string[] MerchantMemId = null;
            if (Disid == "" && statusval == "")
            {
                long mem_id = long.Parse(MemberCurrentUser.MEM_ID.ToString());
                var MerchantList = AdminTransactionViewModel.GetAllSuperCommissionReport(MemberCurrentUser.MEM_ID.ToString(), statusval);
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
            else if (Disid == "" && statusval != "")
            {
                long mem_id = long.Parse(MemberCurrentUser.MEM_ID.ToString());
                var MerchantList = AdminTransactionViewModel.GetAllSuperCommissionReport(MemberCurrentUser.MEM_ID.ToString(), statusval);

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
            else if (Disid != "" && statusval == "")
            {
                long mem_id = long.Parse(Disid.ToString());
                var MerchantList = AdminTransactionViewModel.GetSuperCommissionReport(mem_id.ToString(), statusval);

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
            else if (Disid != "" && statusval != "")
            {
                long mem_id = long.Parse(Disid.ToString());
                var MerchantList = AdminTransactionViewModel.GetSuperCommissionReport(mem_id.ToString(), statusval);
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
            else
            {
                long mem_id = long.Parse(MemberCurrentUser.MEM_ID.ToString());
                var MerchantList = AdminTransactionViewModel.GetAllSuperCommissionReport(MemberCurrentUser.MEM_ID.ToString(), statusval);
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
        }
        // Admin/WL
        public FileResult ExportIndexAdminReport(string statusval, string DateFrom = "", string Date_To = "")
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;
                var db = new DBContext();


                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_ACCOUNTS> grid = CreateExportAdminTableGrid(statusval, DateFrom, Date_To);
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

        private IGrid<TBL_ACCOUNTS> CreateExportAdminTableGrid(string statusval, string DateFrom = "", string Date_To = "")
        {
            var db = new DBContext();
            string[] SuperMemId = null;
            string[] DistributorMemId = null;
            string[] MerchantMemId = null;
            if (statusval == "")
            {
                long mem_id = long.Parse(MemberCurrentUser.MEM_ID.ToString());
                var MerchantList = AdminTransactionViewModel.GetAdminCommissionReport(MemberCurrentUser.MEM_ID.ToString(), statusval, DateFrom, Date_To);
                IGrid<TBL_ACCOUNTS> grid = new Grid<TBL_ACCOUNTS>(MerchantList);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;

                grid.Columns.Add(model => model.SerialNo).Titled("Sln No.");
                grid.Columns.Add(model => model.UserName).Titled("User Name");
                //grid.Columns.Add(model => model.MEMBER_TYPE).Titled("Merchant Type");
                grid.Columns.Add(model => model.TRANSACTION_TYPE).Titled("Transaction Type");
                grid.Columns.Add(model => model.TRANSACTION_DATE).Titled("Transaction Date").Formatted("{0:yyyy-MM-dd}");
                grid.Columns.Add(model => model.OPENING).Titled("Opening");
                grid.Columns.Add(model => model.CR_Col).Titled("Cr");
                grid.Columns.Add(model => model.DR_Col).Titled("Dr");
                //grid.Columns.Add(model => model.NARRATION).Titled("Narration");
                grid.Columns.Add(model => model.CLOSING).Titled("Closing");
                grid.Columns.Add(model => model.COMM_AMT).Titled("Commission Amt.");
                grid.Pager = new GridPager<TBL_ACCOUNTS>(grid);
                grid.Processors.Add(grid.Pager);
                grid.Pager.RowsPerPage = 1000000;

                foreach (IGridColumn column in grid.Columns)
                {
                    column.Filter.IsEnabled = true;
                    column.Sort.IsEnabled = true;
                }

                return grid;
            }
            else if (statusval != "")
            {
                long mem_id = long.Parse(MemberCurrentUser.MEM_ID.ToString());
                var MerchantList = AdminTransactionViewModel.GetAdminCommissionReport(MemberCurrentUser.MEM_ID.ToString(), statusval, DateFrom, Date_To);

                IGrid<TBL_ACCOUNTS> grid = new Grid<TBL_ACCOUNTS>(MerchantList);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;

                grid.Columns.Add(model => model.SerialNo).Titled("Sln No.");
                grid.Columns.Add(model => model.UserName).Titled("User Name");
                //grid.Columns.Add(model => model.MEMBER_TYPE).Titled("Merchant Type");
                grid.Columns.Add(model => model.TRANSACTION_TYPE).Titled("Transaction Type");
                grid.Columns.Add(model => model.TRANSACTION_DATE).Titled("Transaction Date").Formatted("{0:yyyy-MM-dd}");
                grid.Columns.Add(model => model.OPENING).Titled("Opening");
                grid.Columns.Add(model => model.CR_Col).Titled("Cr");
                grid.Columns.Add(model => model.DR_Col).Titled("Dr");
                //grid.Columns.Add(model => model.NARRATION).Titled("Narration");
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
            else
            {
                long mem_id = long.Parse(MemberCurrentUser.MEM_ID.ToString());
                var MerchantList = AdminTransactionViewModel.GetAdminCommissionReport(MemberCurrentUser.MEM_ID.ToString(), statusval, DateFrom, Date_To);
                IGrid<TBL_ACCOUNTS> grid = new Grid<TBL_ACCOUNTS>(MerchantList);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;

                grid.Columns.Add(model => model.SerialNo).Titled("Sln No.");
                grid.Columns.Add(model => model.UserName).Titled("User Name");
                //grid.Columns.Add(model => model.MEMBER_TYPE).Titled("Merchant Type");
                grid.Columns.Add(model => model.TRANSACTION_TYPE).Titled("Transaction Type");
                grid.Columns.Add(model => model.TRANSACTION_DATE).Titled("Transaction Date").Formatted("{0:yyyy-MM-dd}");
                grid.Columns.Add(model => model.OPENING).Titled("Opening");
                grid.Columns.Add(model => model.CR_Col).Titled("Cr");
                grid.Columns.Add(model => model.DR_Col).Titled("Dr");
                //grid.Columns.Add(model => model.NARRATION).Titled("Narration");
                //grid.Columns.Add(model => model.OPENING).Titled("Opening");
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
        }
        // Distributor
        [HttpGet]
        public FileResult ExportIndexDistributorReport(string Super, string Disid, string statusval, string DateFrom = "", string Date_To = "")
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;
                var db = new DBContext();


                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_ACCOUNTS> grid = CreateExportDistributoTableGrid(Super,Disid, statusval, DateFrom, Date_To);
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
        private IGrid<TBL_ACCOUNTS> CreateExportDistributoTableGrid(string Super, string Disid, string statusval, string DateFrom = "", string Date_To = "")
        {
            var db = new DBContext();
            string[] SuperMemId = null;
            string[] DistributorMemId = null;
            string[] MerchantMemId = null;
            var MerchantList = new List<TBL_ACCOUNTS>();
            if (string.IsNullOrEmpty(Super) && string.IsNullOrEmpty(Disid) && string.IsNullOrEmpty(statusval))
            {
                MerchantList = AdminTransactionViewModel.GetAllDistributorCommissionReport(MemberCurrentUser.MEM_ID.ToString(), statusval, DateFrom, Date_To);
                
            }
            else if (string.IsNullOrEmpty(Super) && string.IsNullOrEmpty(Disid) && !string.IsNullOrEmpty(statusval))
            {
                MerchantList = AdminTransactionViewModel.GetAllDistributorCommissionReport(MemberCurrentUser.MEM_ID.ToString(), statusval, DateFrom, Date_To);
                
            }
            else if (!string.IsNullOrEmpty(Super) && !string.IsNullOrEmpty(Disid) && string.IsNullOrEmpty(statusval))
            {
                MerchantList = AdminTransactionViewModel.GetDistributorCommissionReport(Super, Disid.ToString(), statusval, DateFrom, Date_To);
              
            }
            else if (!string.IsNullOrEmpty(Super) && !string.IsNullOrEmpty(Disid) && !string.IsNullOrEmpty(statusval))
            {
                MerchantList = AdminTransactionViewModel.GetDistributorCommissionReport(Super, Disid.ToString(), statusval, DateFrom, Date_To);

            }
            else if (!string.IsNullOrEmpty(Super) && string.IsNullOrEmpty(Disid) && string.IsNullOrEmpty(statusval))
            {
                MerchantList = AdminTransactionViewModel.GetDistributorCommissionReport(Super, Disid.ToString(), statusval, DateFrom, Date_To);
                
            }
            else if (!string.IsNullOrEmpty(Super) && string.IsNullOrEmpty(Disid) && !string.IsNullOrEmpty(statusval))
            {
                MerchantList = AdminTransactionViewModel.GetDistributorCommissionReport(Super, Disid.ToString(), statusval, DateFrom, Date_To);
            }
            else if (string.IsNullOrEmpty(Super) && !string.IsNullOrEmpty(Disid) && string.IsNullOrEmpty(statusval))
            {
                MerchantList = AdminTransactionViewModel.GetAllDistributorCommissionReport(MemberCurrentUser.MEM_ID.ToString(), statusval, DateFrom, Date_To);

            }
            else if (string.IsNullOrEmpty(Super) && !string.IsNullOrEmpty(Disid) && !string.IsNullOrEmpty(statusval))
            {
                MerchantList = AdminTransactionViewModel.GetAllDistributorCommissionReport(MemberCurrentUser.MEM_ID.ToString(), statusval, DateFrom, Date_To);

            }

            long mem_id = long.Parse(MemberCurrentUser.MEM_ID.ToString());
            //var MerchantList = AdminTransactionViewModel.GetAllSuperCommissionReport(MemberCurrentUser.MEM_ID.ToString(), statusval);
            IGrid<TBL_ACCOUNTS> grid = new Grid<TBL_ACCOUNTS>(MerchantList);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;

            grid.Columns.Add(model => model.SerialNo).Titled("Sln No.");
            grid.Columns.Add(model => model.UserName).Titled("User Name");
            //grid.Columns.Add(model => model.MEMBER_TYPE).Titled("Merchant Type");
            grid.Columns.Add(model => model.TRANSACTION_TYPE).Titled("Transaction Type");
            grid.Columns.Add(model => model.TRANSACTION_DATE).Titled("Transaction Date").Formatted("{0:yyyy-MM-dd}");
            grid.Columns.Add(model => model.OPENING).Titled("Opening");
            grid.Columns.Add(model => model.CR_Col).Titled("Cr");
            grid.Columns.Add(model => model.DR_Col).Titled("Dr");
            //grid.Columns.Add(model => model.NARRATION).Titled("Narration");
            grid.Columns.Add(model => model.CLOSING).Titled("Closing");
            grid.Columns.Add(model => model.COMM_AMT).Titled("Commission Amt.");
            grid.Pager = new GridPager<TBL_ACCOUNTS>(grid);
            grid.Processors.Add(grid.Pager);
            grid.Pager.RowsPerPage = 1000000;

            foreach (IGridColumn column in grid.Columns)
            {
                column.Filter.IsEnabled = true;
                column.Sort.IsEnabled = true;
            }

            return grid;

            //if (Disid == "" && statusval == "")
            //{
            //    long mem_id = long.Parse(MemberCurrentUser.MEM_ID.ToString());
            //    var MerchantList = AdminTransactionViewModel.GetAllDistributorCommissionReport(MemberCurrentUser.MEM_ID.ToString(), statusval);
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
            //else if (Disid == "" && statusval != "")
            //{
            //    long mem_id = long.Parse(MemberCurrentUser.MEM_ID.ToString());
            //    var MerchantList = AdminTransactionViewModel.GetAllDistributorCommissionReport(MemberCurrentUser.MEM_ID.ToString(), statusval);

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
            //else if (Disid != "" && statusval == "")
            //{
            //    long mem_id = long.Parse(Disid.ToString());
            //    var MerchantList = AdminTransactionViewModel.GetDistributorCommissionReport(mem_id.ToString(), statusval);

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
            //else if (Disid != "" && statusval != "")
            //{
            //    long mem_id = long.Parse(Disid.ToString());
            //    var MerchantList = AdminTransactionViewModel.GetDistributorCommissionReport(mem_id.ToString(), statusval);
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
            //    //long mem_id = long.Parse(MemberCurrentUser.MEM_ID.ToString());
            //    //var MerchantList = AdminTransactionViewModel.GetAllSuperCommissionReport(MemberCurrentUser.MEM_ID.ToString(), statusval);
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
    }
}