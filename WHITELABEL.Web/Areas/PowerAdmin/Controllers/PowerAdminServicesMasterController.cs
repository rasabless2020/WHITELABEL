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
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;


namespace WHITELABEL.Web.Areas.PowerAdmin.Controllers
{
    [Authorize]
    public class PowerAdminServicesMasterController : PoweradminbaseController
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
        // GET: PowerAdmin/PowerAdminServicesMaster  FirstOrDefaultAsync
        public ActionResult Index()
        {
            if (Session["PowerAdminUserId"] != null)
            {
                try
                {
                    initpage();
                    var db = new DBContext();
                    var ServiceInfo = db.TBL_SETTINGS_SERVICES_MASTER.Where(x=>x.ACTIVESTATUS==true).ToList();
                    return View();
                }
                catch (Exception ex)
                {
                    Logger.Error("Controller:- PowerAdminServicesMaster, method:- Index(GET)  Line No:- 74", ex);
                    return RedirectToAction("Exception", "ErrorHandler", new { area = "" });
                    throw ex;
                }
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
            var db = new DBContext();
            var ServiceInfo = db.TBL_SETTINGS_SERVICES_MASTER.Where(x => x.ACTIVESTATUS == true).ToList();            
            return PartialView(ServiceInfo);
        }

        public ActionResult AddService(string ServiceId = "")
        {
            if (Session["PowerAdminUserId"] != null)
            {
                try
                {
                    var db = new DBContext();
                    if (ServiceId != "")
                    {
                        string decrptSlId = Decrypt.DecryptMe(ServiceId);
                        long ServId = long.Parse(decrptSlId);
                        var ServiceInfo = db.TBL_SETTINGS_SERVICES_MASTER.FirstOrDefault(x => x.SLN == ServId);
                        ViewBag.checkStatus = "1";
                        return View(ServiceInfo);
                    }
                    else
                    {
                        ViewBag.checkStatus = "0";
                        return View();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("Controller:- PowerAdminServicesMaster, method:- AddService(GET)  Line No:- 122", ex);
                    return RedirectToAction("Exception", "ErrorHandler", new { area = "" });
                    throw ex;
                }
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

        [HttpPost]
        public async Task<ActionResult> AddService(TBL_SETTINGS_SERVICES_MASTER objval)
        {
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    bool statuscheck = false;
                    if (objval.ACTIVESTATUS == true)
                    {
                        statuscheck = true;
                    }
                    else
                    {
                        statuscheck = false;
                    }
                    var servicelist = await db.TBL_SETTINGS_SERVICES_MASTER.Where(x => x.SLN == objval.SLN).FirstOrDefaultAsync();
                    if (servicelist != null)
                    {
                        
                        //servicelist.SERVICE_NAME = objval.SERVICE_NAME;
                        servicelist.SERVICE_DESC = objval.SERVICE_DESC;
                        servicelist.ACTIVESTATUS = statuscheck;
                        db.Entry(servicelist).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();
                        ContextTransaction.Commit();
                        ViewBag.msg = "Data Updated...";
                        return View();
                    }
                    else
                    {
                        var checkduplicateentry = db.TBL_SETTINGS_SERVICES_MASTER.Where(x => x.SERVICE_NAME.Contains(objval.SERVICE_NAME)).FirstOrDefault();
                        if (checkduplicateentry == null)
                        {
                            objval.SERVICE_CREATOR = "";
                            objval.UPDATED_ON = DateTime.Now;
                            objval.MEM_ID = CurrentUser.USER_ID;
                            db.TBL_SETTINGS_SERVICES_MASTER.Add(objval);
                            await db.SaveChangesAsync();
                            ContextTransaction.Commit();
                            ViewBag.msg = "One Record Inserted...";
                            return View();
                        }
                        else
                        {
                            ViewBag.checkStatus = "0";
                            ViewBag.msg = "Duplicate entry are not allowed..";
                            return View();

                        }
                    }
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    Logger.Error("Controller:- PowerAdminServicesMaster, method:- AddService(POST)  Line No:- 187", ex);
                    return RedirectToAction("Exception", "ErrorHandler", new { area = "" });
                    throw ex;
                }
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeactiveOperator(string Id)
        {
            try
            {
                var db = new DBContext();
                using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
                {
                    long MemID = long.Parse(Id);
                    var getdocinfo = await db.TBL_SETTINGS_SERVICES_MASTER.Where(x => x.SLN == MemID).FirstOrDefaultAsync();
                    if (getdocinfo != null)
                    {
                        if (getdocinfo.ACTIVESTATUS == true)
                        {
                            getdocinfo.ACTIVESTATUS = false;
                        }
                        else
                        {
                            getdocinfo.ACTIVESTATUS = true;
                        }
                        getdocinfo.UPDATED_ON = DateTime.Now;
                        db.Entry(getdocinfo).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();
                        ContextTransaction.Commit();
                        return Json(new { Result = "true" });
                    }
                    else
                    {
                        ContextTransaction.Rollback();
                        return Json(new { Result = "false" });
                    }
                }
                //string decrptMemId = Decrypt.DecryptMe(Id);
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:- PowerAdminServicesMaster, method:- DeactiveOperator(POST)  Line No:- 225", ex);
                throw;
            }
        }
    }
}