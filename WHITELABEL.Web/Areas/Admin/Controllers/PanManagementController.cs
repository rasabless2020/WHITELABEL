using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WHITELABEL.Web.Areas.Admin.Controllers
{
    public class PanManagementController : Controller
    {
        // GET: Admin/PanManagement
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult MasterVendor()
        {
            return View();
        }
        public ActionResult MasterCouponEntry()
        {
            return View();
        }
        public ActionResult EnterCouponStock()
        {
            return View();
        }
        public ActionResult CouponCommMapping()
        {
            return View();
        }
        public ActionResult MerchantCouponRequisition()
        {
            return View();
        }
        public ActionResult CouponStockReport()
        {
            return View();
        }
        public ActionResult MerchantCouponRequisitionReport()
        {
            return View();
        }
    }
}