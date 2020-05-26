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
using System.Web.Security;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Areas.Admin.Models;
using WHITELABEL.Web.Controllers;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;


namespace WHITELABEL.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class MemberChangePasswordController : AdminBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        public void initpage()
        {
            try
            {
                ViewBag.ControllerName = "Bank List";
                SystemClass sclass = new SystemClass();
                string userID = sclass.GetLoggedUser();
                long userid = long.Parse(userID);
                var dbmain = new DBContext();
                if (userID != null && userID != "")
                {
                    TBL_MASTER_MEMBER currUser = dbmain.TBL_MASTER_MEMBER.SingleOrDefault(c => c.MEM_ID == userid && c.ACTIVE_MEMBER == true && c.MEMBER_ROLE == 1);
                    if (currUser != null)
                    {
                        Session["WhiteLevelUserId"] = currUser.MEM_ID;
                        // Session["UserName"] = currUser.UserName;
                    }
                    else
                    {
                        Response.Redirect(Url.Action("Index", "AdminLogin", new { area = "Area" }));
                        return;
                    }
                }
                if (Session["WhiteLevelUserId"] == null)
                {
                    //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
                    Response.Redirect(Url.Action("Index", "AdminLogin", new { area = "Area" }));
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

        // GET: Admin/MemberChangePassword
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
                return RedirectToAction("Index", "AdminLogin", new { area = "Admin" });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(true)]
        public async Task<ActionResult> ChangePassword(MemberChangePasswordModel value)
        {
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {                    
                    long mem_id = MemberCurrentUser.MEM_ID;
                    var changepass = db.TBL_MASTER_MEMBER.Where(x=>x.MEM_ID==mem_id).FirstOrDefault();
                    if (changepass != null)
                    {
                        changepass.User_pwd = value.User_pwd;
                        db.Entry(changepass).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();
                    }

                    //throw new Exception();
                    ContextTransaction.Commit();
                    FormsAuthentication.SignOut();
                    Session["WhiteLevelUserId"] = null;
                    Session["WhiteLevelUserName"] = null;
                    Session.Clear();
                    Session.Remove("WhiteLevelUserId");
                    Session.Remove("WhiteLevelUserName");
                    //return RedirectToAction("Message");
                    return RedirectToAction("Message", "AdminLogin", new { area = "Admin" });
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    Logger.Error("Controller:-  MemberChangePassword(Admin), method:- ChangePassword (POST) Line No:- 271", ex);
                    return RedirectToAction("Exception", "ErrorHandler", new { area = "" });
                    throw ex;
                    //return View("Error", new HandleErrorInfo(ex, "APILabel", "CreateMember"));               
                }
            }

        }
        public ActionResult Message()
        {
            FormsAuthentication.SignOut();
            Session["WhiteLevelUserId"] = null;
            Session["WhiteLevelUserName"] = null;
            Session.Clear();
            Session.Remove("WhiteLevelUserId");
            Session.Remove("WhiteLevelUserName");
            return RedirectToAction("Index", "AdminLogin", new { area = "Admin" });            
        }
    }
}