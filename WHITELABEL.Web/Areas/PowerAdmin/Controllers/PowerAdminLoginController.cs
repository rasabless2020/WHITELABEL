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

namespace WHITELABEL.Web.Areas.PowerAdmin.Controllers
{
    public class PowerAdminLoginController : Controller
    {
        // GET: PowerAdmin/PowerAdminLogin
        public ActionResult Index(string returnUrl)
        {
            SystemClass sclass = new SystemClass();
            //string userID = sclass.GetLoggedUser();
            if (Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl))
            {
                ViewBag.ReturnURL = returnUrl;
            }
            if (Session["PowerAdminUserId"] != null)
            {
                //Response.RedirectToRoute("Dashboard", "Index");
                string username = Session["PowerAdminUserId"].ToString();              
                var db = new DBContext();
                Response.Redirect(Url.Action("Index", "PowerAdminHome", new { area = "PowerAdmin" }));
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
            //var userpass = "poweradmin123";
            //userpass = userpass.GetPasswordHash();
            if (Session["PowerAdminUserId"] == null)
            {
                using (var db = new DBContext())
                {
                    var GetUser = await db.TBL_AUTH_ADMIN_USERS.SingleOrDefaultAsync(x => x.USER_EMAIL == User.Email);
                    if (GetUser != null)
                    {
                        if (!GetUser.ACTIVE_USER || !GetUser.USER_PASSWORD_MD5.VerifyHashedPassword(User.Password))
                        {
                            ViewBag.Message = "Invalid Credential or Access Denied";
                            return View();
                        }
                        else
                        {
                            Session["PowerAdminUserId"] = GetUser.USER_ID;
                            Session["PowerAdminUserName"] = GetUser.USER_NAME;
                            Session["UserType"] = "Power Admin";
                            HttpCookie AuthCookie;
                            System.Web.Security.FormsAuthentication.SetAuthCookie(GetUser.USER_NAME + "||" + Encrypt.EncryptMe(GetUser.USER_ID.ToString()), true);
                            AuthCookie = System.Web.Security.FormsAuthentication.GetAuthCookie(GetUser.USER_NAME + "||" + Encrypt.EncryptMe(GetUser.USER_ID.ToString()), true);
                            AuthCookie.Expires = DateTime.Now.Add(new TimeSpan(130, 0, 0, 0));
                            Response.Cookies.Add(AuthCookie);
                            return RedirectToAction("Index", "login", new { area = "" });
                            //string username = txtUsername.Text;
                            //string Password = txtPassword.Text;

                            //// Create Cookie and Store the Login Detail in it if check box is checked
                            //if ((keeeMeLoggedIn.Checked == true))
                            //{
                            //   HttpCookie mycookie = new HttpCookie("LoginDetail");
                            //   mycookie.Values("Username") = txtUsername.Text.Trim();
                            //   mycookie.Values("Password") = txtPassword.Text.Trim();
                            //   mycookie.Expires = System.DateTime.Now.AddDays(365);
                            //    Response.Cookies.Add(mycookie);
                            //}
                            //Response.Redirect("Home.aspx");
                            ////On page load,
                            ////check if cookie exist then login page from 
                            //if ((Response.Cookies("LoginDetail") != null))
                            //{
                            ////do stuff
                            //}
                            //else
                            //{
                            //  //redirect to login page
                            //}
                            //Response.Redirect(FormsAuthentication.GetRedirectUrl(GetUser.USER_NAME.ToString(), true));
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
                return RedirectToAction("Index", "PowerAdminHome", new { area = "PowerAdmin" });
                //Response.Redirect(Url.Action("Index", "PowerAdminLogin", new { area = "PowerAdmin" }));
            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult LogOut()
        {
            if (Session["PowerAdminUserId"] != null)
            {

                FormsAuthentication.SignOut();
                Session["PowerAdminUserId"] = null;
                Session["PowerAdminUserName"] = null;
                Session.Clear();
                Session.Remove("PowerAdminUserId");
                Session.Remove("PowerAdminUserName");
                SystemClass sclass = new SystemClass();
                //return RedirectToAction("Index", "Login");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
            else
            {
                //return RedirectToAction("Index", "Login");
                //return RedirectToAction("Index", "PowerAdminLogin", new { area = "PowerAdmin" });
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
    }
}