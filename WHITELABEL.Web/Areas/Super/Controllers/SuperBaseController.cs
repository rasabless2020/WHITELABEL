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

namespace WHITELABEL.Web.Areas.Super.Controllers
{
    [Authorize]
    public class SuperBaseController : Controller
    {
        public TBL_MASTER_MEMBER MemberCurrentUser
        {
            get
            {
                if (Session["SuperDistributorId"] == null)
                {
                    return null;
                }
                var UserId = (long)Session["SuperDistributorId"];
                var db = new DBContext();
                return db.TBL_MASTER_MEMBER.Find(UserId);
            }
            set
            {
                Session["SuperDistributorId"] = value.MEM_ID;
            }
        }
        //// GET: Super/SuperBase
        //public ActionResult Index()
        //{
        //    return View();
        //}
    }
}