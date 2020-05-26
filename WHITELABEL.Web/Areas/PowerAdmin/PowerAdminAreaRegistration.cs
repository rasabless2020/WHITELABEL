using System.Web.Mvc;

namespace WHITELABEL.Web.Areas.PowerAdmin
{
    public class PowerAdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "PowerAdmin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "PowerAdmin_default",
                "PowerAdmin/{controller}/{action}/{id}",
                //new { action = "Index", id = UrlParameter.Optional }
                new { controller = "PowerAdminLogin", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}