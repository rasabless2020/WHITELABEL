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

namespace WHITELABEL.Web.Areas.Distributor.Controllers
{
    [Authorize]
    public class DistributorBaseController : Controller
    {
        public TBL_MASTER_MEMBER MemberCurrentUser
        {
            get
            {
                if (Session["DistributorUserId"] == null)
                {
                    return null;
                }
                var UserId = (long)Session["DistributorUserId"];
                var db = new DBContext();
                return db.TBL_MASTER_MEMBER.Find(UserId);
            }
            set
            {
                Session["DistributorUserId"] = value.MEM_ID;
            }
        }

        //// GET: Distributor/DistributorBase
        //public ActionResult Index()
        //{
        //    return View();
        //}
    }
}