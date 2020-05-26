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
using WHITELABEL.Data.Models;

namespace WHITELABEL.Web.Controllers
{
    public class LoginController : Controller
    {
     
        public ActionResult Index(string returnUrl)
        {
            var db = new DBContext();
            SystemClass sclass = new SystemClass();
            string userID = sclass.GetLoggedUser();
            if (Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl))
            {
                ViewBag.ReturnURL = returnUrl;
            }
            if (Session["UserId"] != null)
            {
                //Response.RedirectToRoute("Dashboard", "Index");
                string username = Session["UserId"].ToString();
                
                var userinfo = db.TBL_MASTER_MEMBER.Where(x => x.UName == username).FirstOrDefault();
                
                if (userinfo.MEMBER_ROLE == 1)
                {
                    Response.Redirect(Url.Action("Index", "WhiteLevelAdmin", new { area = "Admin" }));
                }
                else if (userinfo.MEMBER_ROLE == null)
                {
                    Response.Redirect(Url.Action("Index", "WhiteLevelAdmin", new { area = "Admin" }));
                }
                else if (userinfo.UNDER_WHITE_LEVEL == null)
                {
                    Response.RedirectToRoute("Dashboard", "Index");
                }
            }
            LoginViewModel model = new LoginViewModel();
            if (Request.Cookies["Login"] != null)
            {
                model.Email = Request.Cookies["Login"].Values["EmailID"];
                model.Password = Request.Cookies["Login"].Values["Password"];
            }


            string host = Request.Url.Host;
            //string host = "www.ramkrushnaharitravels.co.in";
            var logochecking = (from x in db.TBL_MASTER_MEMBER
                                join y in db.TBL_WHITE_LEVEL_HOSTING_DETAILS
                                 on x.MEM_ID equals y.MEM_ID
                                //where y.DOMAIN == DomaineName && y.STATUS == 1
                                where y.DOMAIN.Contains(host) && y.STATUS == 1
                                select new
                                {
                                    logoPath = x.LOGO,
                                    LogoStyle = x.LOGO_STYLE,
                                    CompanyName = x.COMPANY
                                }).FirstOrDefault();
            if (logochecking != null)
            {
                if (logochecking.logoPath != null)
                {
                    if (logochecking.logoPath != "")
                    {
                        ViewBag.Logopath = Url.Content(logochecking.logoPath);
                        ViewBag.LogoStyle = logochecking.LogoStyle;
                        ViewBag.CompanyName = logochecking.CompanyName;
                    }
                    else
                    {
                        ViewBag.Logopath = "";
                        ViewBag.LogoStyle = "";
                        ViewBag.CompanyName = "";
                    }
                   
                }
                else
                {
                    ViewBag.Logopath = "";
                    ViewBag.LogoStyle = "";
                    ViewBag.CompanyName = "";
                }
            }
            else
            {
                return RedirectToAction("DomainError", "Login");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Index(LoginViewModel User, string ReturnURL = "")
        {
            SystemClass sclass = new SystemClass();
            string userID = sclass.GetLoggedUser();
            //var userpass = "india123";
            //userpass = userpass.GetPasswordHash();
            //if (Session["PowerAdminUserId"] ==null)
            //{
            using (var db = new DBContext())
            {
                //var GetUser = await db.TBL_AUTH_ADMIN_USERS.FirstOrDefaultAsync(x => x.USER_EMAIL == User.Email && x.USER_PASSWORD_MD5==User.Password);
                var GetUser = await db.TBL_AUTH_ADMIN_USERS.FirstOrDefaultAsync(x => x.USER_EMAIL == User.Email);
                if (GetUser != null)
                {
                    if (!GetUser.ACTIVE_USER || !GetUser.USER_PASSWORD_MD5.VerifyHashedPassword(User.Password))
                    {
                        ViewBag.Message = "Invalid Credential or Access Denied";
                        FormsAuthentication.SignOut();
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
                        //return RedirectToAction("Index", "login", new { area = "" });
                        //Response.Redirect(FormsAuthentication.GetRedirectUrl(GetUser.USER_NAME.ToString(), true));
                        return RedirectToAction("Index", "PowerAdminHome", new { area = "PowerAdmin" });
                    }
                }
                else
                {                    
                    string DomaineName = Request.Url.Host;
                    var logochecking = (from x in db.TBL_MASTER_MEMBER
                                        join y in db.TBL_WHITE_LEVEL_HOSTING_DETAILS
                                         on x.MEM_ID equals y.MEM_ID
                                        //where y.DOMAIN == DomaineName && y.STATUS == 1
                                        where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1
                                        select new
                                        {
                                            logoPath = x.LOGO,
                                            LogoStyle = x.LOGO_STYLE,
                                            CompanyName = x.COMPANY
                                        }).FirstOrDefault();
                    if (logochecking != null)
                    {
                        if (logochecking.logoPath != null)
                        {
                            if (logochecking.logoPath != "")
                            {
                                Session["LogoPath"] = Url.Content(logochecking.logoPath);
                                Session["LogoStyle"] = logochecking.LogoStyle;
                                Session["CompanyName"] = logochecking.CompanyName;
                            }
                            else {
                                Session["LogoPath"] = "";
                                Session["LogoStyle"] = "";
                                Session["CompanyName"] = "";
                            }
                            
                        }
                        else
                        {
                            Session["LogoPath"] = "";
                            Session["LogoStyle"] = "";
                            Session["CompanyName"] = "";
                        }
                    }
                    else
                    {
                        return RedirectToAction("DomainError", "Login");
                    }
                    var GetMember = await db.TBL_MASTER_MEMBER.SingleOrDefaultAsync(x => x.EMAIL_ID == User.Email && x.User_pwd == User.Password && x.ACTIVE_MEMBER == true);
                    if (GetMember != null)
                    {
                        if (GetMember.MEMBER_ROLE == 1)
                        {
                            var GETWHITELevelDOMAIn = (from x in db.TBL_MASTER_MEMBER
                                                       join y in db.TBL_WHITE_LEVEL_HOSTING_DETAILS
                                                        on x.MEM_ID equals y.MEM_ID
                                                       //where y.DOMAIN == DomaineName && y.STATUS == 1
                                                       //where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1 && x.EMAIL_ID == User.Email && x.User_pwd == User.Password && x.ACTIVE_MEMBER == true
                                                       where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1 && y.MEM_ID == GetMember.MEM_ID
                                                       select new
                                                       {
                                                           MEM_ID = x.MEM_ID,
                                                           MEMBER_ROLE = x.MEMBER_ROLE,
                                                           ACTIVE_MEMBER = x.ACTIVE_MEMBER,
                                                           User_pwd = x.User_pwd,
                                                           UName = x.UName,
                                                           DOMAIN = y.DOMAIN
                                                       }).FirstOrDefault();
                            if (GETWHITELevelDOMAIn != null)
                            {
                                Session["DOMAINNAME"] = GETWHITELevelDOMAIn.DOMAIN;

                                if (GetMember.ACTIVE_MEMBER == false || GetMember.User_pwd != User.Password)
                                {
                                    ViewBag.Message = "Invalid Credential or Access Denied";
                                    FormsAuthentication.SignOut();
                                    return View();
                                }
                                else
                                {
                                    Session["WhiteLevelUserId"] = GetMember.MEM_ID;
                                    Session["WhiteLevelUserName"] = GetMember.UName;
                                    Session["UserType"] = "White Level";

                                    HttpCookie AuthCookie;
                                    System.Web.Security.FormsAuthentication.SetAuthCookie(GetMember.UName + "||" + Encrypt.EncryptMe(GetMember.MEM_ID.ToString()), true);
                                    AuthCookie = System.Web.Security.FormsAuthentication.GetAuthCookie(GetMember.UName + "||" + Encrypt.EncryptMe(GetMember.MEM_ID.ToString()), true);
                                    AuthCookie.Expires = DateTime.Now.Add(new TimeSpan(130, 0, 0, 0));
                                    Response.Cookies.Add(AuthCookie);
                                    //Response.Redirect(FormsAuthentication.GetRedirectUrl(GetUser.USER_NAME.ToString(), true));
                                    return RedirectToAction("Index", "WhiteLevelAdmin", new { area = "Admin" });
                                }
                            }
                            else
                            {
                                return RedirectToAction("DomainError", "Login");
                            }
                        }
                        else if (GetMember.MEMBER_ROLE == 2)
                        {


                            if (GetMember.ACTIVE_MEMBER == false || GetMember.User_pwd != User.Password)
                            {
                                ViewBag.Message = "Invalid Credential or Access Denied";
                                FormsAuthentication.SignOut();
                                return View();
                            }
                            else
                            {
                                Session["UserId"] = GetMember.MEM_ID;
                                Session["UserName"] = GetMember.UName;

                                HttpCookie AuthCookie;
                                System.Web.Security.FormsAuthentication.SetAuthCookie(GetMember.UName + "||" + Encrypt.EncryptMe(GetMember.MEM_ID.ToString()), true);
                                AuthCookie = System.Web.Security.FormsAuthentication.GetAuthCookie(GetMember.UName + "||" + Encrypt.EncryptMe(GetMember.MEM_ID.ToString()), true);
                                AuthCookie.Expires = DateTime.Now.Add(new TimeSpan(130, 0, 0, 0));
                                Response.Cookies.Add(AuthCookie);
                                return RedirectToAction("Index", "WhiteLevelAdmin", new { area = "Admin" });
                                //Response.Redirect(FormsAuthentication.GetRedirectUrl(GetUser.USER_NAME.ToString(), true));
                            }


                        }
                        else if (GetMember.MEMBER_ROLE == 3)
                        {
                            var GETWHITELevelDOMAIn = (from x in db.TBL_MASTER_MEMBER
                                                       join y in db.TBL_WHITE_LEVEL_HOSTING_DETAILS
                                                        on x.UNDER_WHITE_LEVEL equals y.MEM_ID
                                                       //where y.DOMAIN == DomaineName && y.STATUS == 1
                                                       //where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1 && x.EMAIL_ID == User.Email && x.User_pwd == User.Password && x.ACTIVE_MEMBER == true
                                                       where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1 && x.MEM_ID == GetMember.MEM_ID
                                                       select new
                                                       {
                                                           MEM_ID = x.MEM_ID,
                                                           MEMBER_ROLE = x.MEMBER_ROLE,
                                                           ACTIVE_MEMBER = x.ACTIVE_MEMBER,
                                                           User_pwd = x.User_pwd,
                                                           UName = x.UName,
                                                           DOMAIN = y.DOMAIN
                                                       }).FirstOrDefault();
                            if (GETWHITELevelDOMAIn != null)
                            {
                                Session["DOMAINNAME"] = GETWHITELevelDOMAIn.DOMAIN;

                                if (GetMember.ACTIVE_MEMBER == false || GetMember.User_pwd != User.Password)
                                {
                                    ViewBag.Message = "Invalid Credential or Access Denied";
                                    FormsAuthentication.SignOut();
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
                                    //System.Web.Security.FormsAuthentication.SetAuthCookie(GetMember.EMAIL_ID +"||"+GetMember.User_pwd +"||" + Encrypt.EncryptMe(GetMember.MEM_ID.ToString()), true);
                                    //AuthCookie = System.Web.Security.FormsAuthentication.GetAuthCookie(GetMember.UName + "||" + GetMember.User_pwd + "||" + Encrypt.EncryptMe(GetMember.MEM_ID.ToString()), true);

                                    AuthCookie.Expires = DateTime.Now.Add(new TimeSpan(130, 0, 0, 0));
                                    Response.Cookies.Add(AuthCookie);
                                    return RedirectToAction("Index", "SuperDashboard", new { area = "Super" });
                                    //Response.Redirect(FormsAuthentication.GetRedirectUrl(GetUser.USER_NAME.ToString(), true));
                                }
                            }
                            else
                            {
                                return RedirectToAction("DomainError", "Login");
                            }
                        }
                        else if (GetMember.MEMBER_ROLE == 4)
                        {
                            var GETWHITELevelDOMAIn = (from x in db.TBL_MASTER_MEMBER
                                                       join y in db.TBL_WHITE_LEVEL_HOSTING_DETAILS
                                                        on x.UNDER_WHITE_LEVEL equals y.MEM_ID
                                                       //where y.DOMAIN == DomaineName && y.STATUS == 1
                                                       //where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1 && x.EMAIL_ID == User.Email && x.User_pwd == User.Password && x.ACTIVE_MEMBER == true
                                                       where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1 && x.MEM_ID == GetMember.MEM_ID
                                                       select new
                                                       {
                                                           MEM_ID = x.MEM_ID,
                                                           MEMBER_ROLE = x.MEMBER_ROLE,
                                                           ACTIVE_MEMBER = x.ACTIVE_MEMBER,
                                                           User_pwd = x.User_pwd,
                                                           UName = x.UName,
                                                           DOMAIN = y.DOMAIN
                                                       }).FirstOrDefault();
                            if (GETWHITELevelDOMAIn != null)
                            {
                                Session["DOMAINNAME"] = GETWHITELevelDOMAIn.DOMAIN;
                                if (GetMember.ACTIVE_MEMBER == false || GetMember.User_pwd != User.Password)
                                {
                                    ViewBag.Message = "Invalid Credential or Access Denied";
                                    FormsAuthentication.SignOut();
                                    return View();
                                }
                                else
                                {
                                    Session["DistributorUserId"] = GetMember.MEM_ID;
                                    Session["DistributorUserName"] = GetMember.UName;
                                    Session["UserType"] = "Distributor";

                                    HttpCookie AuthCookie;
                                    System.Web.Security.FormsAuthentication.SetAuthCookie(GetMember.UName + "||" + Encrypt.EncryptMe(GetMember.MEM_ID.ToString()), true);
                                    AuthCookie = System.Web.Security.FormsAuthentication.GetAuthCookie(GetMember.UName + "||" + Encrypt.EncryptMe(GetMember.MEM_ID.ToString()), true);
                                    AuthCookie.Expires = DateTime.Now.Add(new TimeSpan(130, 0, 0, 0));
                                    Response.Cookies.Add(AuthCookie);
                                    return RedirectToAction("Index", "DistributorDashboard", new { area = "Distributor" });
                                    //Response.Redirect(FormsAuthentication.GetRedirectUrl(GetUser.USER_NAME.ToString(), true));
                                }
                            }
                            else
                            {
                                return RedirectToAction("DomainError", "Login");
                            }

                        }
                        else if (GetMember.MEMBER_ROLE == 5)
                        {
                            var GETWHITELevelDOMAIn = (from x in db.TBL_MASTER_MEMBER
                                                       join y in db.TBL_WHITE_LEVEL_HOSTING_DETAILS
                                                        on x.UNDER_WHITE_LEVEL equals y.MEM_ID
                                                       //where y.DOMAIN == DomaineName && y.STATUS == 1
                                                       //where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1 && x.EMAIL_ID == User.Email && x.User_pwd == User.Password && x.ACTIVE_MEMBER == true
                                                       where y.DOMAIN.Contains(DomaineName) && y.STATUS == 1 && x.MEM_ID == GetMember.MEM_ID
                                                       select new
                                                       {
                                                           MEM_ID = x.MEM_ID,
                                                           MEMBER_ROLE = x.MEMBER_ROLE,
                                                           ACTIVE_MEMBER = x.ACTIVE_MEMBER,
                                                           User_pwd = x.User_pwd,
                                                           UName = x.UName,
                                                           DOMAIN = y.DOMAIN
                                                       }).FirstOrDefault();
                            if (GETWHITELevelDOMAIn != null)
                            {
                                Session["DOMAINNAME"] = GETWHITELevelDOMAIn.DOMAIN;
                                if (GetMember.ACTIVE_MEMBER == false || GetMember.User_pwd != User.Password)
                                {
                                    ViewBag.Message = "Invalid Credential or Access Denied";
                                    FormsAuthentication.SignOut();
                                    return View();
                                }
                                else
                                {
                                    Session["MerchantUserId"] = GetMember.MEM_ID;
                                    Session["MerchantUserName"] = GetMember.UName;
                                    Session["UserType"] = "Merchant";
                                    HttpCookie AuthCookie;
                                    System.Web.Security.FormsAuthentication.SetAuthCookie(GetMember.UName + "||" + Encrypt.EncryptMe(GetMember.MEM_ID.ToString()), true);
                                    AuthCookie = System.Web.Security.FormsAuthentication.GetAuthCookie(GetMember.UName + "||" + Encrypt.EncryptMe(GetMember.MEM_ID.ToString()), true);
                                    AuthCookie.Expires = DateTime.Now.Add(new TimeSpan(130, 0, 0, 0));
                                    Response.Cookies.Add(AuthCookie);
                                    return RedirectToAction("Index", "MerchantDashboard", new { area = "Merchant" });
                                    //Response.Redirect(FormsAuthentication.GetRedirectUrl(GetUser.USER_NAME.ToString(), true));
                                }
                            }
                            else
                            {
                                return RedirectToAction("DomainError", "Login");
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
                    //}
                    //else
                    //{
                    //    return RedirectToAction("DomainError", "Login");
                    //}
                    //ViewBag.Message = "Invalid Credential or Access Denied";
                    //return View();
                }
            }
            //}
            //else
            //{
            //    Response.RedirectToRoute("Home", "Index");
            //}
            //return View();
        }
        public ActionResult DomainError()
        {
            Session.Abandon();
            Session.RemoveAll();
            Session.Clear();
            return View();
        }
        [AllowAnonymous]
        public ActionResult LogOut()
        {
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            Response.Redirect(FormsAuthentication.LoginUrl + "?ReturnURL=" + Url.Action("Index", "Login"));
            return RedirectToAction("Index", "Login");
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgettenPassword user)
        {
            if (ModelState.IsValid)
            {
                var db = new DBContext();
                var UserProfile = db.TBL_MASTER_MEMBER.Where(x => x.EMAIL_ID == user.Email && x.ACTIVE_MEMBER == true).FirstOrDefault();
                if (UserProfile != null)
                {
                    var token = TokenGenerator.GenerateToken();
                    var PasswordResetObj = new TBL_PASSWORD_RESET
                    {
                        ID = token,
                        EmailID = user.Email,
                        Time = DateTime.Now
                    };
                    db.TBL_PASSWORD_RESET.Add(PasswordResetObj);
                    db.SaveChanges();
                    string resetLink = "<a href='"
                           + Url.Action("ResetPassword", "Login", new { rt = token }, "http")
                           + "'>Password Reset</a>";
                    string nameurl = Request.Url.GetLeftPart(UriPartial.Authority);
                    string Domain = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);

                    string firstname = UserProfile.MEMBER_NAME;

                    //string fullName = firstname + " " + lastname;
                    string fullName = UserProfile.UName;
                    string Email = user.Email;
                    string name = fullName;

                    EmailHelper emailHelper = new EmailHelper();
                    string Subject = "Password Reset";
                    string mailBody = emailHelper.GetEmailTemplate(fullName, "Someone has asked to reset the password on your Urwex account. <br /><br />If you didn’t request a password reset, you can disregard this email. <strong>No changes have been made to your account</strong>.<br /> <br />To reset your password, click this link: " + resetLink + " or copy and paste it into your browser.The password reset link is only valid for the next 90 minutes.", "WelcomeTemplate.html");
                    emailHelper.SendUserEmail(Email, Subject, mailBody);
                    ViewBag.Message = "Please check your email for further instructions";
                    return View(user);
                }
                else
                {
                    var UserProfilevale= db.TBL_AUTH_ADMIN_USERS.FirstOrDefault(x => x.USER_EMAIL == user.Email);
                    if (UserProfilevale != null)
                    {
                        var token = TokenGenerator.GenerateToken();
                        var PasswordResetObj = new TBL_PASSWORD_RESET
                        {
                            ID = token,
                            EmailID = user.Email,
                            Time = DateTime.Now
                        };
                        db.TBL_PASSWORD_RESET.Add(PasswordResetObj);
                        db.SaveChanges();
                        string resetLink = "<a href='"
                               + Url.Action("ResetPassword", "Login", new { rt = token }, "http")
                               + "'>Password Reset</a>";
                        string nameurl = Request.Url.GetLeftPart(UriPartial.Authority);
                        string Domain = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);

                        string firstname = UserProfile.MEMBER_NAME;

                        //string fullName = firstname + " " + lastname;
                        string fullName = UserProfile.UName;
                        string Email = user.Email;
                        string name = fullName;

                        EmailHelper emailHelper = new EmailHelper();
                        string Subject = "Password Reset";
                        string mailBody = emailHelper.GetEmailTemplate(fullName, "Someone has asked to reset the password on your Urwex account. <br /><br />If you didn’t request a password reset, you can disregard this email. <strong>No changes have been made to your account</strong>.<br /> <br />To reset your password, click this link: " + resetLink + " or copy and paste it into your browser.The password reset link is only valid for the next 90 minutes.", "WelcomeTemplate.html");
                        emailHelper.SendUserEmail(Email, Subject, mailBody);
                        ViewBag.Message = "Please check your email for further instructions";
                        return View(user);
                    }
                    else {
                        ViewBag.Message = "Email is not available";
                    }
                                        
                }
            }
            //ViewBag.Message = "Please check your email for further instructions";
            return View(user);

        }
        [AllowAnonymous]
        public ActionResult ResetPassword(string rt)
        {
            ResetPasswordModel model = new ResetPasswordModel();
            model.ReturnToken = rt;
            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var db = new DBContext();
                var passwordset = db.TBL_PASSWORD_RESET.Where(x => x.ID == model.ReturnToken).FirstOrDefault();
                if (passwordset != null)
                {
                    var UserInfo = db.TBL_MASTER_MEMBER.Where(x => x.EMAIL_ID == passwordset.EmailID && x.ACTIVE_MEMBER == true).FirstOrDefault();
                    if (UserInfo != null)
                    {
                        UserInfo.User_pwd = model.Password;
                        db.Entry(UserInfo).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        // SEND EMAIL AFTER RESET
                        string Email = passwordset.EmailID;
                        string name = UserInfo.MEMBER_NAME;

                        EmailHelper emailHelper = new EmailHelper();
                        string Subject = "Your Password Has Been Successfully Reset";
                        string mailBody = emailHelper.GetEmailTemplate(UserInfo.MEMBER_NAME, "<p>Your password reset has been reset successfully.</p> <br />  If you weren’t the one who reset your password, please contact our support team at <a href='mailto: support@urwex.com ? subject = Support % 20Query' style='font - size: 17px; background - color: #ffffff;'>support@urwex.com immediately</a></p>", "WelcomeTemplate.html");
                        emailHelper.SendUserEmail(Email, Subject, mailBody);

                        // SEND UI MESSAGE
                        ViewBag.Message = "Password Reset successful. Please login";
                        return View("Index");
                    }
                    else
                    {
                        ViewBag.Message = "Access denied!";
                    }
                }
                else
                {
                    ViewBag.Message = "Access denied!";
                }
            }
            return View(model);
        }

        public ActionResult NewLogin()
        {
            return View();
        }
    }
}
