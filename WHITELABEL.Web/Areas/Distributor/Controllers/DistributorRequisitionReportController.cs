using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WHITELABEL.Web.Areas.Distributor.Controllers
{
    public class DistributorRequisitionReportController : Controller
    {
        // GET: Distributor/DistributorRequisitionReport
        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult IndexGrid(string search = "")
        {
            return PartialView();
        }
    }
}