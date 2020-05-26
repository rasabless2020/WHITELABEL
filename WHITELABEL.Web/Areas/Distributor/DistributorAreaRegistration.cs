using System.Web.Mvc;

namespace WHITELABEL.Web.Areas.Distributor
{
    public class DistributorAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Distributor";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
               "Distributor_default",
                 //"Distributor/{controller}/{action}/{id}",
                 "Distributor/{controller}/{action}/{id}",
                 new
                 {                  
                    controller = "DistributorLogin",
                    action = "Index",
                    id = UrlParameter.Optional
                 }
            );
        }
    }
}