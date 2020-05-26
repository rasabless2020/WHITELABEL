using System.Web.Mvc;

namespace WHITELABEL.Web.Areas.Super
{
    public class SuperAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Super";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Super_default",
                "Super/{controller}/{action}/{id}",
                new { controller = "SuperLogin", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}