using log4net;
using NonFactors.Mvc.Grid;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;

namespace WHITELABEL.Web.Areas.PowerAdmin.Controllers
{
    [Authorize]
    public class PowerAdminCommissionSettingController : PoweradminbaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        // GET: PowerAdmin/PowerAdminCommissionSetting
        public ActionResult Index()
        {
            if (Session["PowerAdminUserId"] != null)
            {
                return View();
            }
            else
            {
                Session["PowerAdminUserId"] = null;
                Session["PowerAdminUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("PowerAdminUserId");
                Session.Remove("PowerAdminUserName");
                Session.Remove("UserType");
                Response.Redirect(Url.Action("Index", "PowerAdminLogin", new { area = "PowerAdmin" }));
                return View();
            }
                
        }
    }
}