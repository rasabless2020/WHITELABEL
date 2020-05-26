using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WHITELABEL.Data;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;

namespace WHITELABEL.Web.Areas.Super.Controllers
{
    public class SuperLoginController : Controller
    {
        // GET: Super/SuperLogin
        public ActionResult Index(string returnUrl)
        {
            SystemClass sclass = new SystemClass();
            string userID = sclass.GetLoggedUser();
            if (Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl))
            {
                ViewBag.ReturnURL = returnUrl;
            }
            if (Session["SuperDistributorId"] != null)
            {
                //Response.RedirectToRoute("Dashboard", "Index");
                string username = Session["SuperDistributorId"].ToString();
                var db = new DBContext();
                var userinfo = db.TBL_MASTER_MEMBER.Where(x => x.UName == username).FirstOrDefault();
                Response.Redirect(Url.Action("Index", "SuperDashboard", new { area = "Super" }));
            }

            LoginViewModel model = new LoginViewModel();
            if (Request.Cookies["Login"] != null)
            {
                model.Email = Request.Cookies["Login"].Values["EmailID"];
                model.Password = Request.Cookies["Login"].Values["Password"];
            }
            return View(model);
            //return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index(LoginViewModel User, string ReturnURL = "")
        {
            SystemClass sclass = new SystemClass();
            string userID = sclass.GetLoggedUser();
            //var userpass = "india123";
            //userpass = userpass.GetPasswordHash();
            if (Session["SuperDistributorId"] == null || Session["DistributorUserId"] == null)
            {
                using (var db = new DBContext())
                {
                    var GetMember = await db.TBL_MASTER_MEMBER.SingleOrDefaultAsync(x => x.EMAIL_ID == User.Email && x.User_pwd == User.Password && x.MEMBER_ROLE== 3 && x.ACTIVE_MEMBER == true);
                    if(GetMember!=null)
                    {
                        if (GetMember.MEMBER_ROLE == 3)
                        {
                            if (GetMember.ACTIVE_MEMBER == false || GetMember.User_pwd != User.Password)
                            {
                                ViewBag.Message = "Invalid Credential or Access Denied";
                                return View();
                            }
                            else
                            {
                                Session["SuperDistributorId"] = GetMember.MEM_ID;
                                Session["SuperDistributorUserName"] = GetMember.UName;
                                Session["UserType"] = "Super Distributor";
                                HttpCookie AuthCookie;
                                System.Web.Security.FormsAuthentication.SetAuthCookie(GetMember.UName + "||" + Encrypt.EncryptMe(GetMember.MEM_ID.ToString()), true);
                                AuthCookie = System.Web.Security.FormsAuthentication.GetAuthCookie(GetMember.UName + "||" + Encrypt.EncryptMe(GetMember.MEM_ID.ToString()), true);
                                AuthCookie.Expires = DateTime.Now.Add(new TimeSpan(130, 0, 0, 0));
                                Response.Cookies.Add(AuthCookie);
                                return RedirectToAction("Index", "SuperDashboard", new { area = "Super" });
                                //Response.Redirect(FormsAuthentication.GetRedirectUrl(GetUser.USER_NAME.ToString(), true));
                            }
                        }
                        else
                        {
                            ViewBag.Message = "Invalid Credential or Access Denied";
                            return View();
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Invalid Credential or Access Denied";
                        return View();
                    }
                   
                    //ViewBag.Message = "Invalid Credential or Access Denied";
                    //return View();
                }
            }
            else
            {
                Response.RedirectToRoute("Home", "Index");
            }
            return View();
        }


        [AllowAnonymous]
        public ActionResult LogOut()
        {
            if (Session["SuperDistributorId"] != null)
            {

                FormsAuthentication.SignOut();
                Session["SuperDistributorId"] = null;
                Session["SuperDistributorUserName"] = null;
                Session.Clear();
                Session.Remove("SuperDistributorId");
                Session.Remove("SuperDistributorUserName");
                SystemClass sclass = new SystemClass();
                return RedirectToAction("Index", "login", new { area = "" });
            }
            else
            {
                return RedirectToAction("Index", "login", new { area = "" });
            }
        }
        [AllowAnonymous]
        public ActionResult Message()
        {
            FormsAuthentication.SignOut();
            Session["SuperDistributorId"] = null;
            Session["SuperDistributorUserName"] = null;
            Session.Clear();
            Session.Remove("SuperDistributorId");
            Session.Remove("SuperDistributorUserName");
            SystemClass sclass = new SystemClass();
            return View();
            //return RedirectToAction("Index", "AdminLogin", new { area = "Admin" });
        }
    }
}