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
    public class PowerAdminRecipientAccountValidationController : PoweradminbaseController
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
        // GET: PowerAdmin/PowerAdminRecipientAccountValidation
        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult IndexGrid()
        {
            try
            {
                var db = new DBContext();
                var GetAcntVerification = db.TBL_ACCOUNT_VERIFICATION_TABLE.ToList();
                return PartialView("IndexGrid", GetAcntVerification);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        [HttpPost]
        public JsonResult POSTAPIAcntVarification(string APIAcntAmt, string CustveriAmt, string ApiName, string ApiStatus)
        {
            try
            {
                if (APIAcntAmt != "" && CustveriAmt != "")
                {
                    var db = new DBContext();
                    decimal Api_AcntVal = decimal.Parse(APIAcntAmt);
                    decimal APIAccountTns = decimal.Parse(APIAcntAmt);
                    decimal Merchanttrans = decimal.Parse(CustveriAmt);
                    string APIName = ApiName;
                    int tds_ststus = 0;
                    int Gst_ststus = 0;

                    if (ApiStatus == "Active")
                    {
                        Gst_ststus = 1;
                    }
                    else
                    { Gst_ststus = 0; }
                    db.Database.ExecuteSqlCommand("TRUNCATE TABLE ACCOUNT_VERIFICATION_TABLE");

                    TBL_ACCOUNT_VERIFICATION_TABLE objAPIVeri = new TBL_ACCOUNT_VERIFICATION_TABLE()
                    {
                        MEM_ID = CurrentUser.USER_ID,
                        RECIPIENT_ACNT_VRT_AMTt = Api_AcntVal,
                        APPLIED_AMT_TO_MERCHANT = Merchanttrans,
                        APINAME = APIName,
                        STATUS= Gst_ststus,
                        CREATE_DATE=DateTime.Now
                    };
                    db.TBL_ACCOUNT_VERIFICATION_TABLE.Add(objAPIVeri);
                    db.SaveChanges();
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
        public JsonResult GetAPIAccountVerifyAmt(string TransId)
        {
            try
            {
                long valueid = long.Parse(TransId);
                var db = new DBContext();
                var BankMarginvaluebind = db.TBL_ACCOUNT_VERIFICATION_TABLE.Where(x => x.ID == valueid).FirstOrDefault();
                return Json(BankMarginvaluebind, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

    }
}