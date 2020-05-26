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
    public class PowerAdminRailUtilityApplicationController : PoweradminbaseController
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
        // GET: PowerAdmin/PowerAdminRailUtilityApplication
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
        public PartialViewResult Indexgrid()
        {
            var db = new DBContext();
            //var RailwayutilityApp = db.TBL_APPLICATION_FOR_RAIL_UTILITY.Where(x => x.STATUS != 0).ToList();
            var RailwayutilityApp = (from x in db.TBL_APPLICATION_FOR_RAIL_UTILITY
                                     join y in db.TBL_MASTER_MEMBER on x.APPLIED_MER_ID equals y.MEM_ID
                                     where x.STATUS !=0
                                     select new
                                     {
                                         Mem_Id=x.APPLIED_MER_ID,
                                         ID=x.ID,
                                         MemberName = y.MEMBER_NAME,
                                         MemberRole = (db.TBL_MASTER_MEMBER_ROLE.FirstOrDefault(s => s.ROLE_ID == x.APPLIED_MEM_TYPE).ROLE_NAME),
                                         AppliedDate = x.APPLICATION_DATE,
                                         Status = x.STATUS
                                     }).AsEnumerable().Select(z => new TBL_APPLICATION_FOR_RAIL_UTILITY
                                     {
                                         APPLIED_MER_ID=z.Mem_Id,
                                         ID=z.ID,
                                         Member_Name = z.MemberName,
                                         MemberRole = z.MemberRole,
                                         APPLICATION_DATE = z.AppliedDate,
                                         STATUS = z.Status
                                     }).ToList();
            return PartialView("Indexgrid", RailwayutilityApp);
        }

        [HttpPost]        
        public JsonResult ApproveRailUtilityApplication(string railId_Value = "",string railpwd="", string Appl_Id="",string Mem_Id="")
        {
            var db = new DBContext();

            try
            {
                long AppId = 0;
                long.TryParse(Appl_Id, out AppId);
                long MemId = 0;
                long.TryParse(Mem_Id, out MemId);

                var memberinfo = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == MemId);
                memberinfo.RAIL_ID = railId_Value;
                memberinfo.RAIL_PWD = railpwd;                
                db.Entry(memberinfo).State = System.Data.Entity.EntityState.Modified;
                var appliationstatus = db.TBL_APPLICATION_FOR_RAIL_UTILITY.FirstOrDefault(x => x.ID == AppId);
                appliationstatus.STATUS = 0;
                appliationstatus.APPROVE_DATE = DateTime.Now;
                appliationstatus.APPROVE_BY = 0;
                db.SaveChanges();
                return Json(new { Result = "true" });
            }
            catch (Exception ex)
            {

                return Json(new { Result = "false" });
            }

        }
        [HttpPost]        
        public JsonResult DeclineRailUtilityApplication(string slnval = "")
        {
            var db = new DBContext();

            try
            {
                long Id_Val = 0;
                long.TryParse(slnval, out Id_Val);
                var applicationDeclined = db.TBL_APPLICATION_FOR_RAIL_UTILITY.FirstOrDefault(x=>x.ID== Id_Val);
                applicationDeclined.STATUS = 2;
                applicationDeclined.DECLINE_BY = 0;
                applicationDeclined.DECLINE_DATE = DateTime.Now;
                db.Entry(applicationDeclined).State = System.Data.Entity.EntityState.Modified;
                 db.SaveChanges();

                return Json(new { Result = "true" });
            }
            catch (Exception ex)
            {

                return Json(new { Result = "false" });
            }

        }

    }
}