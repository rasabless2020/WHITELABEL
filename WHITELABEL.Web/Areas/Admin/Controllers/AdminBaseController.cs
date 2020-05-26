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


namespace WHITELABEL.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class AdminBaseController : Controller
    {
        public TBL_MASTER_MEMBER MemberCurrentUser
        {
            get
            {
                if (Session["WhiteLevelUserId"] == null)
                {
                    return null;
                }
                var UserId = (long)Session["WhiteLevelUserId"];
                var db = new DBContext();
                return db.TBL_MASTER_MEMBER.Find(UserId);
            }
            set
            {
                Session["WhiteLevelUserId"] = value.MEM_ID;
            }
        }

        

        //// GET: Admin/AdminBase
        //public ActionResult Index()
        //{
        //    return View();
        //}
    }
}