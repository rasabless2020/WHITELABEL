using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WHITELABEL.Web.Controllers
{
    [Authorize]
    
    public class ErrorHandlerController : Controller
    {
        // GET: ErrorHandler
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Notfound()
        {
            return View();
        }
        public ActionResult Exception()
        {
            return View();
        }
    }
}