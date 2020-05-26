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


namespace WHITELABEL.Web.Areas.Admin.Controllers
{
  
    public class AdminLoginController : Controller
    {
        // GET: Admin/AdminLogin   
       
        public ActionResult Index(string returnUrl)
        {
            SystemClass sclass = new SystemClass();
            //string userID = sclass.GetLoggedUser();
            if (Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl))
            {
                ViewBag.ReturnURL = returnUrl;
            }
            if (Session["WhiteLevelUserId"] != null)
            {
                //Response.RedirectToRoute("Dashboard", "Index");
                string username = Session["WhiteLevelUserId"].ToString();
                var db = new DBContext();
                var userinfo = db.TBL_MASTER_MEMBER.Where(x => x.UName == username).FirstOrDefault();
                Response.Redirect(Url.Action("Index", "WhiteLevelAdmin", new { area = "Admin" }));
            }
            LoginViewModel model = new LoginViewModel();
            if (Request.Cookies["Login"] != null)
            {
                model.Email = Request.Cookies["Login"].Values["EmailID"];
                model.Password = Request.Cookies["Login"].Values["Password"];
            }
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Index(LoginViewModel User, string ReturnURL = "")
        {
            SystemClass sclass = new SystemClass();            
            //////string userID = sclass.GetLoggedUser();
            //var userpass = "india123";
            //userpass = userpass.GetPasswordHash();
            if (Session["UserType"] != "White Level")
            {
                if (Session["WhiteLevelUserId"] == null)
                {
                    using (var db = new DBContext())
                    {
                        var GetUser = await db.TBL_MASTER_MEMBER.SingleOrDefaultAsync(x => x.EMAIL_ID == User.Email && x.User_pwd == User.Password && x.MEMBER_ROLE== 1 && x.ACTIVE_MEMBER == true);
                        if (GetUser != null)
                        {
                            //if (!GetUser.ACTIVE_MEMBER || !GetUser.User_pwd.VerifyHashedPassword(User.Password))
                            if (GetUser.ACTIVE_MEMBER == false || GetUser.User_pwd != User.Password)
                            {
                                ViewBag.Message = "Invalid Credential or Access Denied";
                                return View();
                            }
                            else
                            {
                                Session["WhiteLevelUserId"] = GetUser.MEM_ID;
                                Session["WhiteLevelUserName"] = GetUser.UName;
                                Session["UserType"] = "White Level";
                                HttpCookie AuthCookie;
                                System.Web.Security.FormsAuthentication.SetAuthCookie(GetUser.UName + "||" + Encrypt.EncryptMe(GetUser.MEM_ID.ToString()), true);
                                AuthCookie = System.Web.Security.FormsAuthentication.GetAuthCookie(GetUser.UName + "||" + Encrypt.EncryptMe(GetUser.MEM_ID.ToString()), true);
                                AuthCookie.Expires = DateTime.Now.Add(new TimeSpan(130, 0, 0, 0));
                                Response.Cookies.Add(AuthCookie);
                                //Response.Redirect(FormsAuthentication.GetRedirectUrl(GetUser.USER_NAME.ToString(), true));
                                return RedirectToAction("Index", "WhiteLevelAdmin", new { area = "Admin" });
                            }
                        }
                        else
                        {
                            ViewBag.Message = "Invalid Credential or Access Denied";
                            return View();
                        }
                    }
                   
                }
                else
                {
                    //Response.RedirectToRoute("AdminLogin", "Index",new {area="Admin" });                    
                    return RedirectToAction("Index", "AdminLogin", new { area = "Admin" });
                }
                return View();
            }
            else
            {
                FormsAuthentication.SignOut();               
                Session["WhiteLevelUserId"] = null;
                Session["WhiteLevelUserName"] = null;
                Session["IDval"] = null;                
                Session.Clear();
                Session.Remove("IDval");
                Session.Remove("WhiteLevelUserId");
                Session.Remove("WhiteLevelUserName");                
                return RedirectToAction("Index", "AdminLogin", new { area = "Admin" });
                //return View();
            }
        }
        [AllowAnonymous]
        public ActionResult LogOut()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                //FormsAuthentication.SignOut();
                //Session["UserId"] = null;
                //Session["UserName"] = null;
                //Session.Clear();
                //Session.Remove("UserId");
                //Session.Remove("UserName");
                //SystemClass sclass = new SystemClass();
                //return RedirectToAction("Index", "Login", new { area = "" });
                FormsAuthentication.SignOut();
                Session["WhiteLevelUserId"] = null;
                Session["WhiteLevelUserName"] = null;
                Session.Clear();
                Session.Remove("WhiteLevelUserId");
                Session.Remove("WhiteLevelUserName");
                return RedirectToAction("Index", "login", new { area = "" });
            }
            else
            {
                //return RedirectToAction("Index", "AdminLogin", new { area = "Admin" });
                return RedirectToAction("Index", "login", new { area = "" });
            }
        }
        [AllowAnonymous]
        public ActionResult Message()
        {
            FormsAuthentication.SignOut();
            Session["WhiteLevelUserId"] = null;
            Session["WhiteLevelUserName"] = null;
            Session.Clear();
            Session.Remove("WhiteLevelUserId");
            Session.Remove("WhiteLevelUserName");
            return View();
            //return RedirectToAction("Index", "AdminLogin", new { area = "Admin" });
        }
    }
}