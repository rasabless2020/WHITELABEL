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

namespace WHITELABEL.Web.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        public TBL_AUTH_ADMIN_USER CurrentUser
        {
            get
            {
                if (Session["UserId"] == null)
                {
                    return null;
                }
                var UserId = (long)Session["UserId"];
                var db = new DBContext();
                return db.TBL_AUTH_ADMIN_USERS.Find(UserId);
            }
            set
            {
                Session["UserId"] = value.USER_ID;
            }
        }
        //// GET: BaseController
        //public ActionResult Index()
        //{
        //    return View();
        //}
    }
}