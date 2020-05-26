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
using WHITELABEL.Web.Areas.Admin.Models;
using WHITELABEL.Web.Controllers;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;

namespace WHITELABEL.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class AdminMonthlyGSTController : AdminBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        //public void initpage()
        //{
        //    try
        //    {
        //        ViewBag.ControllerName = "Bank List";
        //        SystemClass sclass = new SystemClass();
        //        string userID = sclass.GetLoggedUser();
        //        long userid = long.Parse(userID);
        //        var dbmain = new DBContext();
        //        if (userID != null && userID != "")
        //        {
        //            TBL_MASTER_MEMBER currUser = dbmain.TBL_MASTER_MEMBER.SingleOrDefault(c => c.MEM_ID == userid && c.ACTIVE_MEMBER == true && c.MEMBER_ROLE == 1);
        //            if (currUser != null)
        //            {
        //                Session["WhiteLevelUserId"] = currUser.MEM_ID;
        //            }
        //            else
        //            {
        //                Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //                return;
        //            }
        //        }
        //        if (Session["WhiteLevelUserId"] == null)
        //        {
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
                    //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
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
                //ViewBag.UserName = CurrentUser.UserId;
                Console.WriteLine(e.InnerException);
                return;
            }
        }

        // GET: Admin/AdminMonthlyGST
        public ActionResult Index()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                initpage();
                return View();
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
        [HttpPost]
        public JsonResult GetMonth_details()
        {
            List<DropdownSelectbyMonth> monthObj = new List<DropdownSelectbyMonth>()
            {
                new DropdownSelectbyMonth() { MonthID="1",MonthName="JAN"},
                new DropdownSelectbyMonth() { MonthID="2",MonthName="FEB"},
                new DropdownSelectbyMonth() { MonthID="3",MonthName="MAR"},
                new DropdownSelectbyMonth() { MonthID="4",MonthName="APR"},
                new DropdownSelectbyMonth() { MonthID="5",MonthName="MAY"},
                new DropdownSelectbyMonth() { MonthID="6",MonthName="JUN"},
                new DropdownSelectbyMonth() { MonthID="7",MonthName="JLY"},
                new DropdownSelectbyMonth() { MonthID="8",MonthName="AUG"},
                new DropdownSelectbyMonth() { MonthID="9",MonthName="SEPT"},
                new DropdownSelectbyMonth() { MonthID="10",MonthName="OCT"},
                new DropdownSelectbyMonth() { MonthID="10",MonthName="NOV"},
                new DropdownSelectbyMonth() { MonthID="12",MonthName="DEC"},
            };
            return Json(monthObj, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult MonthGetServiceproviderdetails()
        {
            initpage();////
            var db = new DBContext();
            //var serviceprovider = db.TBL_ACCOUNTS.Select(x => x.TRANSACTION_TYPE).ToList().Distinct();
            var serviceprovider = db.TBL_ACCOUNTS.Where(y => y.TRANSACTION_TYPE != "Bank Account Verification" && y.TRANSACTION_TYPE != "Cash Deposit in bank" && y.TRANSACTION_TYPE != "IMPS" && y.TRANSACTION_TYPE != "NEFT" && y.TRANSACTION_TYPE != "RTGS").Select(x => x.TRANSACTION_TYPE).ToList().Distinct();
            return Json(serviceprovider, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult MonthlyGSTCalculateAmount(string MonthID = "", string ServiceName = "", string Year = "")
        {
            initpage();////
            if (MonthID != null)
            {
                var db = new DBContext();
                int Monthval = 0;
                int.TryParse(MonthID, out Monthval);

                long serviceid = 0;


                //var serviceId = db.TBL_SERVICE_PROVIDERS.Where(x => x.ID == serviceid).Select(c => c.SERVICE_NAME).FirstOrDefault();

                var GSTValu12e = (from x in db.TBL_ACCOUNTS
                                  where x.MEM_ID == MemberCurrentUser.MEM_ID && x.TRANSACTION_DATE.Month == Monthval && x.DR_CR == "CR" && x.TRANSACTION_TYPE == ServiceName && x.MEMBER_TYPE == "WHITE LEVEL" && x.NARRATION == "Commission"
                                  select new
                                  {
                                      MEM_ID = x.MEM_ID,
                                      TRANSACTION_TYPE = x.TRANSACTION_TYPE,
                                      TRANSACTION_DATE = x.TRANSACTION_DATE,
                                      AMOUNT = x.AMOUNT,
                                      GST = x.GST,
                                      GSTPERCENTAGE = x.GST_PERCENTAGE,
                                      TDS = x.TDS,
                                      TDSPERCENTAGE = x.TDS_PERCENTAGE,
                                  }).AsEnumerable().Select(z => new TBL_ACCOUNTS
                                  {
                                      MEM_ID = z.MEM_ID,
                                      TRANSACTION_TYPE = z.TRANSACTION_TYPE,
                                      timevalue = z.TRANSACTION_DATE.ToString(),
                                      //TRANSACTION_DATE = z.TRANSACTION_DATE,
                                      AMOUNT = z.AMOUNT,
                                      GST = z.GST,
                                      GST_PERCENTAGE = z.GSTPERCENTAGE,
                                      TDS = z.TDS,
                                      TDS_PERCENTAGE = z.TDSPERCENTAGE

                                  }).ToList().Distinct();
                //var GSTValue = (from x in db.TBL_ACCOUNTS
                //                where x.MEM_ID == MemberCurrentUser.MEM_ID && x.TRANSACTION_DATE.Month == Monthval && x.DR_CR == "CR" && x.TRANSACTION_TYPE == ServiceName && x.MEMBER_TYPE == "WHITE LEVEL" && x.NARRATION == "Commission"
                //                select new
                //                {
                //                    Operator = db.TBL_SERVICE_PROVIDERS.Where(d => d.ID == x.SERVICE_ID).Select(c => c.SERVICE_NAME).FirstOrDefault(),
                //                    COMMISSIONAMT = x.COMM_AMT
                //                }).Sum(c => (decimal?)c.COMMISSIONAMT) ?? 0;
                var GSTValue = (from x in db.TBL_DMR_TRANSACTION_LOGS
                                where x.WLP_ID == MemberCurrentUser.MEM_ID && x.TRANSACTION_DATE.Month == Monthval
                                select new
                                {
                                    Operator="DMT",
                                    COMMISSIONAMT=x.WLP_GST_OUTPUT
                                }).Sum(c => (decimal?)c.COMMISSIONAMT) ?? 0;

                var getListGSTComm = db.TBL_ACCOUNTS.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID && x.TRANSACTION_DATE.Month == Monthval && x.DR_CR == "CR" && x.TRANSACTION_TYPE == ServiceName && x.MEMBER_TYPE == "WHITE LEVEL" && x.NARRATION == "Commission").FirstOrDefault();

                TBL_ACCOUNTS objval = new TBL_ACCOUNTS()
                {
                    MEMBER_TYPE = ServiceName,
                    COMM_AMT = GSTValue,
                    timevalue = DateTime.Now.ToString("dd-MM-yyyy"),
                    GST_PERCENTAGE = 18
                };


                return Json(objval, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("There is no GST Available in this month", JsonRequestBehavior.AllowGet);
            }
        }

    }
}