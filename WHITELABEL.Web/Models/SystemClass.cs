namespace WHITELABEL.Web.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Security;
    using WHITELABEL.Web.Helper;

    public class SystemClass : IDisposable
    {
        public static string GetUserName()
        {
            string UserName = string.Empty;
            HttpCookie myCookie = HttpContext.Current.Request.Cookies[".ASPXAUTH"];
            if (myCookie != null)
            {
                string value = myCookie.Value;
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(value);
                string UserInformation = ticket.Name.ToString();
                string[] User = UserInformation.Split(new[] { "||" }, StringSplitOptions.None);
                UserName = User[0];

            }
            return UserName;
        }

        public string GetLoggedUser()
        {
            string userID = string.Empty;
            HttpCookie myCookie = HttpContext.Current.Request.Cookies[".ASPXAUTH"];
            if (myCookie != null)
            {
                string value = myCookie.Value;
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(value);
                string UserInformation = ticket.Name.ToString();
                string[] User = UserInformation.Split(new[] { "||" }, StringSplitOptions.None);
                userID = Decrypt.DecryptMe(User[1]);

            }
            return userID;
        }

        public Int32 LogoutUser()
        {
            Int32 userID = 0;
            HttpCookie myCookie = HttpContext.Current.Request.Cookies[".ASPXAUTH"];
            if (myCookie != null)
            {
                myCookie.Expires = DateTime.Now;
                userID = 1;
            }
            return userID;
        }

        public void Dispose()
        {
        }
    }
}