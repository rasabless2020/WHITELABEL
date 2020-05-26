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
    public class PoweradminbaseController : Controller
    {
        public TBL_AUTH_ADMIN_USER CurrentUser
        {
            get
            {
                if (Session["PowerAdminUserId"] == null)
                {
                    return null;
                }
                var UserId = (long)Session["PowerAdminUserId"];
                var db = new DBContext();
                return db.TBL_AUTH_ADMIN_USERS.Find(UserId);
            }
            set
            {
                Session["PowerAdminUserId"] = value.USER_ID;
            }
        }
        //// GET: PowerAdmin/Poweradminbase
        //public ActionResult Index()
        //{
        //    return View();
        //}
    }
}