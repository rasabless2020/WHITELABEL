using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WHITELABEL.Web.Areas.PowerAdmin.Controllers
{
    public class PowerAdminErrorController : Controller
    {
        // GET: PowerAdmin/PowerAdminError
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult NotFound()
        {
            return View();

        }
        public ActionResult NotAuthorized()
        {
            return View();
        }
        public ActionResult Error()
        {
            return View();
        }
    }
}