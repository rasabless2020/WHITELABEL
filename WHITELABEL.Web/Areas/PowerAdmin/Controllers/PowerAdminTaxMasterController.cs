using log4net;
using NonFactors.Mvc.Grid;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;


namespace WHITELABEL.Web.Areas.PowerAdmin.Controllers
{
    [Authorize]
    public class PowerAdminTaxMasterController : PoweradminbaseController
    {
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
        // GET: PowerAdmin/PowerAdminTaxMaster
        public ActionResult Index()
        {
            if (Session["PowerAdminUserId"] != null)
            {
                initpage();
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
        public PartialViewResult IndexGrid()
        {
            try
            {
                var db = new DBContext();
                var getTaxMargin_Info = db.TBL_TAX_MASTER.ToList();
                return PartialView("IndexGrid", getTaxMargin_Info);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        [HttpPost]
        public JsonResult DMTBank_Margin(string TdsVal, string GstVal, string TdsStatus, string Gststatus)
        {
            try
            {
                if (TdsVal != ""&& GstVal!="")
                {
                    var db = new DBContext();
                    decimal BankMarginAmt = decimal.Parse(TdsVal);
                    decimal TDSamt = decimal.Parse(TdsVal);
                    decimal GSTamt = decimal.Parse(GstVal);
                    int tds_ststus = 0;
                    int Gst_ststus = 0;
                    if (TdsStatus == "Active")
                    {
                        tds_ststus = 1;
                    }
                    if (Gststatus == "Active")
                    {
                        Gst_ststus = 1;
                    }
                    db.Database.ExecuteSqlCommand("TRUNCATE TABLE TAX_MASTER");

                    TBL_TAX_MASTER objtax = new TBL_TAX_MASTER()
                    {
                        TDS=TDSamt,
                        GST= GSTamt,
                        TDS_STATUS= tds_ststus,
                        GST_STATUS= Gst_ststus
                    };
                    db.TBL_TAX_MASTER.Add(objtax);
                    db.SaveChanges();

                    

                    //TBL_TAX_MASTER objbank = new TBL_TAX_MASTER()
                    //{
                    //    DMTBANK_MARGIN = BankMarginAmt,
                    //    CREATED_DATE = DateTime.Now,
                    //    CREATED_BY = CurrentUser.USER_ID
                    //};
                    //db.TBL_DMT_BANK_MARGIN.Add(objbank);
                    //db.SaveChanges();

                    return Json("true", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("false", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }
        [HttpPost]
        public JsonResult GetTaxMaster(string TransId)
        {
            try
            {
                long valueid = long.Parse(TransId);
                var db = new DBContext();
                var BankMarginvaluebind = db.TBL_TAX_MASTER.Where(x => x.ID == valueid).FirstOrDefault();
                return Json(BankMarginvaluebind, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}