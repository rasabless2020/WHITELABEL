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
    public class MemberProfileUpdateController : AdminBaseController
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
        //                // Session["UserName"] = currUser.UserName;
        //            }
        //            else
        //            {
        //                Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //                return;
        //            }
        //        }
        //        if (Session["WhiteLevelUserId"] == null)
        //        {
        //            //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
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
                ViewBag.ControllerName = "White Label";
                if (Session["WhiteLevelUserId"] == null)
                {
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
                Console.WriteLine(e.InnerException);
                return;
            }
        }
        // GET: Admin/MemberProfileUpdate
        public ActionResult Index()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                initpage();
                var db = new DBContext();
                long mem_id = MemberCurrentUser.MEM_ID;
                var userinformation = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == mem_id).FirstOrDefault();

                ManageUserProfileDetails userinfo = new ManageUserProfileDetails()
                {
                    UName = userinformation.UName,
                    MEM_ID = userinformation.MEM_ID,
                    MEMBER_MOBILE=userinformation.MEMBER_MOBILE,
                    MEMBER_NAME=userinformation.MEMBER_NAME,
                    COMPANY=userinformation.COMPANY,
                    ADDRESS=userinformation.ADDRESS,
                    CITY=userinformation.CITY,
                    PIN=userinformation.PIN,
                    EMAIL_ID=userinformation.EMAIL_ID
                };

                return View(userinfo);
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
        [ValidateAntiForgeryToken]
        [ValidateInput(true)]
        public async Task<ActionResult> Index(ManageUserProfileDetails objval)
        {
            initpage();////
               var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    long mem_id = MemberCurrentUser.MEM_ID;
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
                    Logger.Error("Controller:-  MemberChangePassword(Admin), method:- ChangePassword (POST) Line No:- 271", ex);
                    return RedirectToAction("Exception", "ErrorHandler", new { area = "" });
                    throw ex;
                    //return View("Error", new HandleErrorInfo(ex, "APILabel", "CreateMember"));               
                }
            }
            //return View();
        }
    }
}