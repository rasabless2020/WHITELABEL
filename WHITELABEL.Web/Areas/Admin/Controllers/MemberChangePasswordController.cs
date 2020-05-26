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
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(true)]
        //public async Task<ActionResult> ChangePassword(MemberChangePasswordModel value)
        public async Task<JsonResult> ChangePassword(MemberChangePasswordModel value)
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
                        if (changepass.User_pwd == value.OldUser_pwd)
                        {
                            var userpass = value.User_pwd;
                            //userpass = userpass.GetPasswordHash();
                            changepass.User_pwd = userpass;
                            db.Entry(changepass).State = System.Data.Entity.EntityState.Modified;
                            await db.SaveChangesAsync();
                            //throw new Exception();
                            var token = TokenGenerator.GenerateToken();
                            var PasswordResetObj = new TBL_PASSWORD_RESET
                            {
                                ID = token,
                                EmailID = changepass.EMAIL_ID,
                                Time = DateTime.Now
                            };
                            db.TBL_PASSWORD_RESET.Add(PasswordResetObj);
                            db.SaveChanges();
                            ContextTransaction.Commit();
                            FormsAuthentication.SignOut();
                            FormsAuthentication.SignOut();
                            Session["WhiteLevelUserId"] = null;
                            Session["WhiteLevelUserName"] = null;
                            Session.Clear();
                            Session.Remove("WhiteLevelUserId");
                            Session.Remove("WhiteLevelUserName");
                            //return RedirectToAction("Message");
                            return Json("Password changed Successfully", JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json("Please Enter Valid Old Password ", JsonRequestBehavior.AllowGet);
                        }

                    }
                    return Json("OK", JsonRequestBehavior.AllowGet);

                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    Logger.Error("Controller:-  MemberChangePassword(Admin), method:- ChangePassword (POST) Line No:- 271", ex);

                    throw ex;
                    return Json("Try Again After Sometime", JsonRequestBehavior.AllowGet);
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
            return RedirectToAction("Index", "Login", new { area = "" });            
        }
    }
}