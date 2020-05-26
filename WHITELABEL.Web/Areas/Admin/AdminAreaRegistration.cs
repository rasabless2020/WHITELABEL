using System.Web.Mvc;

namespace WHITELABEL.Web.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                //new { controller= "WhiteLevelAdmin", action = "Index", id = UrlParameter.Optional }
                new { controller = "AdminLogin", action = "Index", id = UrlParameter.Optional }
            );         
        }
    }
}