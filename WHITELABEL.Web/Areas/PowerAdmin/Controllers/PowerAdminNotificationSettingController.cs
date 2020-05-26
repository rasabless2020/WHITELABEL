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
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;

namespace WHITELABEL.Web.Areas.PowerAdmin.Controllers
{
    [Authorize]
    public class PowerAdminNotificationSettingController : PoweradminbaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        public void initpage()
        {
            try
            {
                ViewBag.ControllerName = "Power Bank Details";
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
        // GET: PowerAdmin/PowerAdminNotificationSetting
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
                var NotificationDetails = db.TBL_NOTIFICATION_SETTING.Where(x => x.STATUS == 0).ToList();
                return PartialView("IndexGrid", NotificationDetails);
                
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public ActionResult AddNotification(string NofifyID="")
        {
            try
            {
                var db = new DBContext();

                if (NofifyID == "")
                {
                    ViewBag.checkStatus = "0";
                    Session.Remove("msg");
                    Session["msg"] = null;
                    return View();
                }
                else
                {
                    string Decriptme = Decrypt.DecryptMe(NofifyID);
                    long ID_val = long.Parse(Decriptme);
                    var notify = db.TBL_NOTIFICATION_SETTING.FirstOrDefault(x => x.ID ==ID_val);
                    Session.Remove("msg");
                    Session["msg"] = null;
                    ViewBag.checkStatus = "1";
                    return View(notify);
                }
                
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        [HttpPost]
        public ActionResult AddNotification(TBL_NOTIFICATION_SETTING objnotify)
        {
            try
            {
                var db = new DBContext();
                var info = db.TBL_NOTIFICATION_SETTING.FirstOrDefault(x => x.ID == objnotify.ID);
                if (info == null)
                {
                    TBL_NOTIFICATION_SETTING obj = new TBL_NOTIFICATION_SETTING()
                    {
                        MEM_ID = CurrentUser.USER_ID,
                        NOTIFICATION_SUBJECT = objnotify.NOTIFICATION_SUBJECT,
                        NOTIFICATION_DESCRIPTION = objnotify.NOTIFICATION_DESCRIPTION,
                        NOTIFICATION_DATE = objnotify.NOTIFICATION_DATE,
                        NOTIFICATION_TIME = Convert.ToDateTime(System.DateTime.Now.ToString("HH:mm:ss")),
                        STATUS = objnotify.STATUS
                    };
                    db.TBL_NOTIFICATION_SETTING.Add(obj);
                    db.SaveChanges();
                    ViewBag.Msg = "Notification Save Successfully";
                    Session["msg"] = "Notification Save Successfully";
                }
                else
                {
                    info.NOTIFICATION_SUBJECT = objnotify.NOTIFICATION_SUBJECT;
                    info.NOTIFICATION_DESCRIPTION = objnotify.NOTIFICATION_DESCRIPTION;
                    info.NOTIFICATION_DATE = objnotify.NOTIFICATION_DATE;
                    info.STATUS = objnotify.STATUS;
                    info.NOTIFICATION_TIME = Convert.ToDateTime(System.DateTime.Now.ToString("HH:mm:ss"));
                    db.Entry(info).State = System.Data.Entity.EntityState.Modified;                                        
                    db.SaveChanges();
                    ViewBag.Msg =  "Notification Update Successfully";
                    Session["msg"] = "Notification Update Successfully";
                }
                //return View("AddNotification");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }

    }
}