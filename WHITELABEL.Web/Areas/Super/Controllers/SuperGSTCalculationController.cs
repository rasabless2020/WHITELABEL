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
using System.Web.Security;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Areas.Super.Models;
using WHITELABEL.Web.Controllers;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;

namespace WHITELABEL.Web.Areas.Super.Controllers
{
    [Authorize]
    public class SuperGSTCalculationController : SuperBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        //public void initpage()
        //{
        //    try
        //    {
        //        ViewBag.ControllerName = "Super member Create";

        //        SystemClass sclass = new SystemClass();
        //        string userID = sclass.GetLoggedUser();
        //        long userid = long.Parse(userID);
        //        var dbmain = new DBContext();
        //        if (userID != null && userID != "")
        //        {
        //            TBL_MASTER_MEMBER currUser = dbmain.TBL_MASTER_MEMBER.SingleOrDefault(c => c.MEM_ID == userid && c.ACTIVE_MEMBER == true && c.MEMBER_ROLE == 3);
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



        // GET: Super/SuperGSTCalculation
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
        public JsonResult GetServiceproviderdetails()
        {
            initpage();////
            var db = new DBContext();
            var serviceprovider = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE == "MobileOperator").ToList();
            return Json(serviceprovider, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult CalculateGSTAmount(string MonthID = "", string ServiceName = "", string Year = "")
        {
            initpage();////
            if (MonthID != null)
            {
                var db = new DBContext();
                int Monthval = 0;
                int.TryParse(MonthID, out Monthval);

                long serviceid = 0;
                long.TryParse(ServiceName, out serviceid);

                var serviceId = db.TBL_SERVICE_PROVIDERS.Where(x => x.ID == serviceid).Select(c => c.SERVICE_NAME).FirstOrDefault();

                var GSTValu12e = (from x in db.TBL_ACCOUNTS
                                  where x.MEM_ID == MemberCurrentUser.MEM_ID && x.TRANSACTION_DATE.Month == Monthval && x.DR_CR == "CR" && x.TRANSACTION_TYPE == "Mobile Recharge" && x.SERVICE_ID == serviceid && x.MEMBER_TYPE == "SUPER DISTRIBUTOR" && x.NARRATION == "Commission"
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

                //var get = db.TBL_ACCOUNTS.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID && x.TRANSACTION_DATE.Month == Monthval && x.DR_CR == "CR" && x.TRANSACTION_TYPE == "Mobile Recharge" && x.SERVICE_ID == serviceid && x.MEMBER_TYPE == "SUPER DISTRIBUTOR" && x.NARRATION == "Commission").Sum(c => c.COMM_AMT);

                var GSTValue = (from x in db.TBL_ACCOUNTS
                                where x.MEM_ID == MemberCurrentUser.MEM_ID && x.TRANSACTION_DATE.Month == Monthval && x.DR_CR == "CR" && x.TRANSACTION_TYPE == "Mobile Recharge" && x.SERVICE_ID == serviceid && x.MEMBER_TYPE == "SUPER DISTRIBUTOR" && x.NARRATION == "Commission"
                                select new
                                {
                                    Operator = db.TBL_SERVICE_PROVIDERS.Where(d => d.ID == x.SERVICE_ID).Select(c => c.SERVICE_NAME).FirstOrDefault(),
                                    COMMISSIONAMT = x.COMM_AMT
                                }).Sum(c => (decimal?)c.COMMISSIONAMT) ?? 0;

                var getListGSTComm = db.TBL_ACCOUNTS.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID && x.TRANSACTION_DATE.Month == Monthval && x.DR_CR == "CR" && x.TRANSACTION_TYPE == "Mobile Recharge" && x.SERVICE_ID == serviceid && x.MEMBER_TYPE == "SUPER DISTRIBUTOR" && x.NARRATION == "Commission").FirstOrDefault();

                TBL_ACCOUNTS objval = new TBL_ACCOUNTS()
                {
                    MEMBER_TYPE = serviceId,
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