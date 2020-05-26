using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WHITELABEL.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            //routes.MapRoute(
            //"Admin", // Route name
            //"Admin/AdminController",
            //new { controller = "AdminController", action = "Index" },
            //new[] { "Practise.Areas.SOProblems.Controllers.SubFolder" });
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                
                defaults: new { controller = "Login", action = "Index", id = UrlParameter.Optional }
                //defaults: new { controller = "CMS", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
