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
    public class MerchantTransactionReportController : MerchantBaseController
    {
        // GET: Merchant/MerchantTransactionReport
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        //public void initpage()
        //{
        //    try
        //    {
        //        ViewBag.ControllerName = "Merchant Dashboard";
        //        SystemClass sclass = new SystemClass();
        //        string userID = sclass.GetLoggedUser();
        //        long userid = long.Parse(userID);
        //        var dbmain = new DBContext();
        //        if (userID != null && userID != "")
        //        {
        //            TBL_MASTER_MEMBER currUser = dbmain.TBL_MASTER_MEMBER.SingleOrDefault(c => c.MEM_ID == userid && c.MEMBER_ROLE == 5 && c.ACTIVE_MEMBER == true);
        //            if (currUser != null)
        //            {
        //                Session["MerchantUserId"] = currUser.MEM_ID;
        //                // Session["UserName"] = currUser.UserName;
        //            }
        //            else
        //            {
        //                Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //                return;
        //            }
        //        }
        //        if (Session["MerchantUserId"] == null)
        //        {
        //            //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //            Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //            return;
        //        }
        //        bool Islogin = false;
        //        if (Session["MerchantUserId"] != null)
        //        {
        //            Islogin = true;
        //            ViewBag.CurrentUserId = CurrentMerchant.MEM_ID;
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
                ViewBag.ControllerName = "Merchant";
              
                if (Session["MerchantUserId"] == null)
                {
                    //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
                    Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
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
        public ActionResult Index()
        {
            if (Session["MerchantUserId"] != null)
            {
                initpage();
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
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }

        public PartialViewResult IndexGrid(string search = "")
        {
            var db = new DBContext();
            var transactionlistvalue = MerchantDailyTransactionClass.GetTransactionReport(search, CurrentMerchant.MEM_ID);
            return PartialView("IndexGrid", transactionlistvalue);
        }

        // Admin/WL
        public FileResult ExportIndexMerchantTransReport(string statusval)
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;
                var db = new DBContext();


                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_ACCOUNTS> grid = CreateExportMerchantTableGrid(statusval);
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

        private IGrid<TBL_ACCOUNTS> CreateExportMerchantTableGrid(string statusval)
        {
            var db = new DBContext();
             var  transactionlistvalue = MerchantDailyTransactionClass.GetTransactionReport(statusval, CurrentMerchant.MEM_ID);            
                long mem_id = long.Parse(CurrentMerchant.MEM_ID.ToString());
                
                IGrid<TBL_ACCOUNTS> grid = new Grid<TBL_ACCOUNTS>(transactionlistvalue);
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

        public ActionResult DailyTransaction()
        {
            initpage();
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
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        public PartialViewResult DailyTransactiongrid(string search = "")
        {
            var db = new DBContext();
            var transactionlistvalue = MerchantDailyTransactionClass.GetTransactionReport(search, CurrentMerchant.MEM_ID);
            return PartialView("DailyTransactiongrid", transactionlistvalue);
            
        }
        public ActionResult _GridFooter()
        {
            var db = new DBContext();
            var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                        join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                        where x.MEM_ID == CurrentMerchant.MEM_ID
                                        select new
                                        {
                                            CommissionAmt = x.COMM_AMT
                                        }).AsEnumerable().Select(z => new TBL_ACCOUNTS
                                        {                                           
                                            COMM_AMT = z.CommissionAmt
                                        }).ToList();
            return PartialView("_GridFooter", transactionlistvalue);
        }

        public ActionResult DMRTransactionList()
        {
            initpage();
            return View();
        }
        public PartialViewResult DMR_TransactionGridView()
        {
            try
            {
                var db = new DBContext();
                var GetTransaction = db.TBL_DMR_TRANSACTION_LOGS.Where(x => x.MER_ID == CurrentMerchant.MEM_ID).ToList();
                return PartialView("DMR_TransactionGridView", GetTransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }
        public PartialViewResult DMRGridview()
        {
            try
            {
                var db = new DBContext();
                var GetTransaction = db.TBL_DMR_TRANSACTION_LOGS.Where(x => x.MER_ID == CurrentMerchant.MEM_ID).ToList();
                return PartialView("DMRGridview", GetTransaction);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public ActionResult MerchantCreditLimitList()
        {
            if (Session["MerchantUserId"] != null)
            {
                initpage();
                var db = new DBContext();
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
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }

        public PartialViewResult MerchantCreditReportindexgrid(string DateFrom = "", string Date_To = "")
        {
            try
            {
                var dbcontext = new DBContext();
                if (DateFrom != "" && Date_To != "")
                {
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    DateTime To_Date_Val = Date_To_Val.AddDays(1);
                    var Mem_info = dbcontext.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID).INTRODUCER;
                    var memberinfo = (from tblcre in dbcontext.TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      join mem in dbcontext.TBL_MASTER_MEMBER on tblcre.FROM_MEM_ID equals mem.MEM_ID
                                      where tblcre.TO_MEM_ID == Mem_info && tblcre.FROM_MEM_ID == CurrentMerchant.MEM_ID && tblcre.CREDIT_DATE >= Date_From_Val && tblcre.CREDIT_DATE <= To_Date_Val
                                      select new
                                      {
                                          sln = tblcre.SLN,
                                          ToMember = tblcre.TO_MEM_ID,
                                          From_Member = tblcre.FROM_MEM_ID,
                                          Mem_Name = mem.UName,
                                          Credit_note = tblcre.CREDIT_NOTE_DESCRIPTION,
                                          CreditNoteDate = tblcre.CREDIT_DATE,
                                          CreditAmount = tblcre.CREDIT_AMOUNT,
                                          DR_CR = tblcre.CREDIT_AMOUNT,
                                          creditStatus = tblcre.CREDIT_STATUS,
                                          OpeningAmt = tblcre.CREDIT_OPENING,
                                          Closingamt = tblcre.CREDITCLOSING,
                                          creditType = tblcre.CREDIT_TRN_TYPE
                                      }).AsEnumerable().Select(z => new TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      {
                                          SLN = z.sln,
                                          FromUser = z.Mem_Name,
                                          CREDIT_DATE = z.CreditNoteDate,
                                          //CREDIT_AMOUNT = z.CreditAmount,
                                          CREDIT_NOTE_DESCRIPTION = z.Credit_note,
                                          CREDIT_STATUS = z.creditStatus,
                                          CREDIT_AMOUNT = z.DR_CR,
                                          CREDITCLOSING = z.Closingamt,
                                          CR_Col = (z.creditType == "CR" ? z.CreditAmount.ToString() : "0"),
                                          DR_Col = (z.creditType == "DR" ? z.CreditAmount.ToString() : "0"),
                                          CREDIT_OPENING = z.OpeningAmt,
                                          CREDIT_TRN_TYPE = z.creditType
                                      }).ToList();
                    return PartialView("MerchantCreditReportindexgrid", memberinfo);
                }
                else
                {
                    DateTime NowDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                    DateTime Todaydate = Convert.ToDateTime(DateTime.Now.AddDays(1));
                    DateTime Today_date = Convert.ToDateTime(Todaydate.ToString("yyyy-MM-dd"));
                    var Mem_info = dbcontext.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID).INTRODUCER;
                    var memberinfo = (from tblcre in dbcontext.TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      join mem in dbcontext.TBL_MASTER_MEMBER on tblcre.FROM_MEM_ID equals mem.MEM_ID
                                      where tblcre.TO_MEM_ID == Mem_info && tblcre.FROM_MEM_ID == CurrentMerchant.MEM_ID && tblcre.CREDIT_DATE >= NowDate && tblcre.CREDIT_DATE <= Today_date
                                      select new
                                      {
                                          sln = tblcre.SLN,
                                          ToMember = tblcre.TO_MEM_ID,
                                          From_Member = tblcre.FROM_MEM_ID,
                                          Mem_Name = mem.UName,
                                          Credit_note = tblcre.CREDIT_NOTE_DESCRIPTION,
                                          CreditNoteDate = tblcre.CREDIT_DATE,
                                          CreditAmount = tblcre.CREDIT_AMOUNT,
                                          DR_CR = tblcre.CREDIT_AMOUNT,
                                          creditStatus = tblcre.CREDIT_STATUS,
                                          OpeningAmt = tblcre.CREDIT_OPENING,
                                          Closingamt = tblcre.CREDITCLOSING,
                                          creditType = tblcre.CREDIT_TRN_TYPE
                                      }).AsEnumerable().Select(z => new TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      {
                                          SLN = z.sln,
                                          FromUser = z.Mem_Name,
                                          CREDIT_DATE = z.CreditNoteDate,
                                          //CREDIT_AMOUNT = z.CreditAmount,
                                          CREDIT_NOTE_DESCRIPTION = z.Credit_note,
                                          CREDIT_STATUS = z.creditStatus,
                                          CREDIT_AMOUNT = z.DR_CR,
                                          CREDITCLOSING = z.Closingamt,
                                          CR_Col = (z.creditType == "CR" ? z.CreditAmount.ToString() : "0"),
                                          DR_Col = (z.creditType == "DR" ? z.CreditAmount.ToString() : "0"),
                                          CREDIT_OPENING = z.OpeningAmt,
                                          CREDIT_TRN_TYPE = z.creditType
                                      }).ToList();
                    return PartialView("MerchantCreditReportindexgrid", memberinfo);
                }

            }
            catch (Exception ex)
            {

                throw;
            }
        }

    }
}