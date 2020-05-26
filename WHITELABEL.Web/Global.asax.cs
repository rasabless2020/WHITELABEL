using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WHITELABEL.Web.Controllers;

namespace WHITELABEL.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
           AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            log4net.Config.XmlConfigurator.Configure();            
        }
        protected void Application_Error()
        {
            //if (Context.IsCustomErrorEnabled)
            //    ShowCustomErrorPage(Server.GetLastError());
            HttpContext httpContext = HttpContext.Current;
            if (httpContext != null)
            {
                RequestContext requestContext = ((MvcHandler)httpContext.CurrentHandler).RequestContext;
                //HttpContext.Request.Url.OriginalString
                string url = httpContext.Request.Url.OriginalString;
                if (httpContext.Response.StatusCode == 400)
                {
                    httpContext.Response.Redirect("~/ErrorHandler/Notfound");
                    //httpContext.Response.Clear();
                    //string controllerName = requestContext.RouteData.GetRequiredString("controller");
                    //IControllerFactory factory = ControllerBuilder.Current.GetControllerFactory();
                    //IController controller = factory.CreateController(requestContext, controllerName);
                    //ControllerContext controllerContext = new ControllerContext(requestContext, (ControllerBase)controller);

                    //JsonResult jsonResult = new JsonResult
                    //{
                    //    Data = new { success = false, serverError = "500" },
                    //    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    //};
                    //jsonResult.ExecuteResult(controllerContext);
                    //httpContext.Response.End();
                }
                else if (httpContext.Response.StatusCode == 404)
                {
                    httpContext.Response.Redirect("~/ErrorHandler/NotFound");
                }
                else if (httpContext.Response.StatusCode == 500)
                {
                    httpContext.Response.Redirect("~/ErrorHandler/Exception");
                }
                else if (httpContext.Response.StatusCode == 200)
                {
                    httpContext.Response.Redirect("~/ErrorHandler/Exception");
                }
                else
                {
                    httpContext.Response.Redirect("~/ErrorHandler/NotFound");
                }
            }
        }



        //private void ShowCustomErrorPage(Exception exception)
        //{
        //    HttpException httpException = exception as HttpException;
        //    if (httpException == null)
        //        httpException = new HttpException(500, "Internal Server Error", exception);

        //    Response.Clear();
        //    RouteData routeData = new RouteData();
        //    routeData.Values.Add("controller", "ErrorHandler");
        //    routeData.Values.Add("fromAppErrorEvent", true);

        //    switch (httpException.GetHttpCode())
        //    {
        //        case 403:
        //            routeData.Values.Add("action", "Exception");
        //            break;

        //        case 404:
        //            routeData.Values.Add("action", "Notfound");
        //            break;

        //        case 500:
        //            routeData.Values.Add("action", "Exception");
        //            break;

        //        default:
        //            routeData.Values.Add("action", "OtherHttpStatusCode");
        //            routeData.Values.Add("httpStatusCode", httpException.GetHttpCode());
        //            break;
        //    }

        //    Server.ClearError();

        //    IController controller = new ErrorHandlerController();
        //    controller.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
        //}

    }
}
