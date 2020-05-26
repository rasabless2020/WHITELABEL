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
using WHITELABEL.Web.Areas.Distributor.Models;
using WHITELABEL.Web.Controllers;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;


namespace WHITELABEL.Web.Areas.Distributor.Controllers
{
    [Authorize]
    public class DistributorPasswordChangeController : DistributorBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        //public void initpage()
        //{
        //    try
        //    {
        //        ViewBag.ControllerName = "Distributor Requisition";
        //        SystemClass sclass = new SystemClass();
        //        string userID = sclass.GetLoggedUser();
        //        long userid = long.Parse(userID);
        //        var dbmain = new DBContext();
        //        if (userID != null && userID != "")
        //        {
        //            TBL_MASTER_MEMBER currUser = dbmain.TBL_MASTER_MEMBER.SingleOrDefault(c => c.MEM_ID == userid && c.ACTIVE_MEMBER == true && c.MEMBER_ROLE == 4);
        //            if (currUser != null)
        //            {
        //                Session["DistributorUserId"] = currUser.MEM_ID;
        //                // Session["UserName"] = currUser.UserName;
        //            }
        //            else
        //            {
        //                Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //                return;
        //            }
        //        }
        //        if (Session["DistributorUserId"] == null)
        //        {
        //            //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //            Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //            return;
        //        }
        //        bool Islogin = false;
        //        if (Session["DistributorUserId"] != null)
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
                ViewBag.ControllerName = "Distributor";
                if (Session["DistributorUserId"] == null)
                {
                    Response.Redirect(Url.Action("Logout", "Login", new { area = "" }));
                    return;
                }
                bool Islogin = false;

                if (Session["DistributorUserId"] != null)
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
        // GET: Distributor/DistributorPasswordChange
        public ActionResult Index()
        {
            if (Session["DistributorUserId"] != null)
            {
                initpage();
                return View();
            }
            else
            {
                Session["DistributorUserId"] = null;
                Session["DistributorUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("DistributorUserId");
                Session.Remove("DistributorUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(true)]
        //public async Task<ActionResult> ChangePassword(DistributorChangePasswordModel value)
        public async Task<JsonResult> ChangePassword(DistributorChangePasswordModel value)
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
                            Session["DistributorUserId"] = null;
                            Session["DistributorUserName"] = null;
                            Session.Clear();
                            Session.Remove("DistributorUserId");
                            Session.Remove("DistributorUserName");
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
    }
}