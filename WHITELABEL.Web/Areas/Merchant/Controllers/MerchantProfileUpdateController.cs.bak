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
using System.Web.Security;

namespace WHITELABEL.Web.Areas.Merchant.Controllers
{
    [Authorize]
    public class MerchantProfileUpdateController : MerchantBaseController
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
        // GET: Merchant/MerchantProfileUpdate
        public ActionResult Index()
        {
            if (Session["MerchantUserId"] != null)
            {
                initpage();
                var db = new DBContext();
                long mem_id = CurrentMerchant.MEM_ID;
                var userinformation = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == mem_id).FirstOrDefault();

                ManageUserProfileDetails userinfo = new ManageUserProfileDetails()
                {
                    UName = userinformation.UName,
                    MEM_ID = userinformation.MEM_ID,
                    MEMBER_MOBILE = userinformation.MEMBER_MOBILE,
                    MEMBER_NAME = userinformation.MEMBER_NAME,
                    COMPANY = userinformation.COMPANY,
                    ADDRESS = userinformation.ADDRESS,
                    CITY = userinformation.CITY,
                    PIN = userinformation.PIN,
                    EMAIL_ID = userinformation.EMAIL_ID
                };

                return View(userinfo);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(true)]
        public async Task<ActionResult> Index(ManageUserProfileDetails objval)
        {
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    long mem_id = CurrentMerchant.MEM_ID;
                    var changepass = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == mem_id).FirstOrDefault();
                    if (changepass != null)
                    {
                        changepass.UName = objval.UName;
                        changepass.COMPANY = objval.COMPANY;
                        changepass.MEMBER_MOBILE = objval.MEMBER_MOBILE;
                        changepass.MEMBER_NAME = objval.MEMBER_NAME;
                        changepass.ADDRESS = objval.ADDRESS;
                        changepass.CITY = objval.CITY;
                        changepass.PIN = objval.PIN;
                        db.Entry(changepass).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();
                    }

                    //throw new Exception();
                    ContextTransaction.Commit();
                    ViewBag.Message = "Profile Updated Successfully...";
                    return View();

                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    Logger.Error("Controller:-  MerchantProfileUpdate(Merchant), method:- Index (POST) Line No:- 141", ex);
                    return RedirectToAction("Exception", "ErrorHandler", new { area = "" });
                    throw ex;
                    //return View("Error", new HandleErrorInfo(ex, "APILabel", "CreateMember"));               
                }
            }
            //return View();
        }
    }
}