using log4net;
using NonFactors.Mvc.Grid;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Controllers;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;

namespace WHITELABEL.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class MemberChangeChannelLinkController : AdminBaseController
    {
        // GET: Admin/MemberChangeChannelLink
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);

        public void initpage()
        {
            try
            {
                if (DomainNameCheck.DomainChecking(Session["DOMAINNAME"].ToString(), Request.Url.Host) == false)
                {
                    Response.Redirect(Url.Action("DomainError", "Login", new { area = "" }));
                }
                ViewBag.ControllerName = "White Label";
                if (Session["WhiteLevelUserId"] == null)
                {
                    Response.Redirect(Url.Action("Logout", "Login", new { area = "" }));
                    return;
                }
                bool Islogin = false;

                if (Session["WhiteLevelUserId"] != null)
                {
                    Islogin = true;
                    ViewBag.CurrentUserId = MemberCurrentUser.MEM_ID;
                }
                ViewBag.Islogin = Islogin;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException);
                return;
            }
        }

        public ActionResult Index()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                initpage();
                var db = new DBContext();
                var SuperList = db.TBL_MASTER_MEMBER.Where(x => x.UNDER_WHITE_LEVEL == MemberCurrentUser.MEM_ID && x.MEMBER_ROLE == 3).ToList();
                ViewBag.DistributorIntroducerList = new SelectList(SuperList, "MEM_ID", "UName");
                var DistributorList = db.TBL_MASTER_MEMBER.Where(x => x.UNDER_WHITE_LEVEL == MemberCurrentUser.MEM_ID && x.MEMBER_ROLE == 4).ToList();
                ViewBag.DistributorList = new SelectList(DistributorList, "MEM_ID", "UName");
                return View();
            }
            else
            {
                Session["WhiteLevelUserId"] = null;
                Session["WhiteLevelUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("WhiteLevelUserId");
                Session.Remove("WhiteLevelUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }

        public ActionResult ChangeDistributorLink()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                initpage();
                var db = new DBContext();
                var Supermember = db.TBL_MASTER_MEMBER.Where(x => x.UNDER_WHITE_LEVEL == MemberCurrentUser.MEM_ID && x.MEMBER_ROLE == 3).ToList();
                ViewBag.SUPERList = new SelectList(Supermember, "MEM_ID", "UName");
                return View();
            }
            else
            {
                Session["WhiteLevelUserId"] = null;
                Session["WhiteLevelUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("WhiteLevelUserId");
                Session.Remove("WhiteLevelUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });
            }

        }
        public PartialViewResult ChangeDistributorGridIndex()
        {
            try
            {
                var db = new DBContext();
                var DistributorInfo = (from x in db.TBL_MASTER_MEMBER
                                       where x.UNDER_WHITE_LEVEL == MemberCurrentUser.MEM_ID && x.MEMBER_ROLE == 4
                                       select new
                                       {
                                           Super_Name = db.TBL_MASTER_MEMBER.Where(a => a.MEM_ID == x.INTRODUCER && a.MEMBER_ROLE == 3).Select(q => q.UName).FirstOrDefault(),
                                           Super_ID = db.TBL_MASTER_MEMBER.Where(a => a.MEM_ID == x.INTRODUCER && a.MEMBER_ROLE == 3).Select(q => q.MEM_ID).FirstOrDefault(),
                                           DistributorId = x.MEM_ID,
                                           UserName = x.UName,
                                           EmailId = x.EMAIL_ID,
                                           MobileNo = x.MEMBER_MOBILE,
                                           JoiningDate = x.JOINING_DATE

                                       }).AsEnumerable().Select(z => new TBL_MASTER_MEMBER
                                       {
                                           SuperName = z.Super_Name,
                                           SUPER_ID = z.Super_ID,
                                           MEM_ID = z.DistributorId,
                                           UName = z.UserName,
                                           EMAIL_ID = z.EmailId,
                                           MEMBER_MOBILE = z.MobileNo,
                                           JOINING_DATE = z.JoiningDate
                                       }).ToList();
                return PartialView("ChangeDistributorGridIndex", DistributorInfo);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public ActionResult ChangeMerchantLink()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                initpage();
                var db = new DBContext();
                //var DistributorList = db.TBL_MASTER_MEMBER.Where(x => x.UNDER_WHITE_LEVEL == MemberCurrentUser.MEM_ID && x.MEMBER_ROLE == 4).ToList();
                //ViewBag.DistributorList = new SelectList(DistributorList, "MEM_ID", "UName");
                return View();
            }
            else
            {
                Session["WhiteLevelUserId"] = null;
                Session["WhiteLevelUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("WhiteLevelUserId");
                Session.Remove("WhiteLevelUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        public PartialViewResult ChangeMerchantGridIndex()
        {
            try
            {
                var db = new DBContext();
                var DistributorInfo = (from x in db.TBL_MASTER_MEMBER
                                       where x.UNDER_WHITE_LEVEL == MemberCurrentUser.MEM_ID && x.MEMBER_ROLE == 5
                                       select new
                                       {
                                           Super_Name = db.TBL_MASTER_MEMBER.Where(a => a.MEM_ID == x.INTRODUCER && a.MEMBER_ROLE == 4).Select(q => q.UName).FirstOrDefault(),
                                           Super_ID = db.TBL_MASTER_MEMBER.Where(a => a.MEM_ID == x.INTRODUCER && a.MEMBER_ROLE == 4).Select(q => q.MEM_ID).FirstOrDefault(),
                                           DistributorId = x.MEM_ID,
                                           UserName = x.UName,
                                           EmailId = x.EMAIL_ID,
                                           MobileNo = x.MEMBER_MOBILE,
                                           JoiningDate = x.JOINING_DATE

                                       }).AsEnumerable().Select(z => new TBL_MASTER_MEMBER
                                       {
                                           SuperName = z.Super_Name,
                                           SUPER_ID = z.Super_ID,
                                           MEM_ID = z.DistributorId,
                                           UName = z.UserName,
                                           EMAIL_ID = z.EmailId,
                                           MEMBER_MOBILE = z.MobileNo,
                                           JOINING_DATE = z.JoiningDate
                                       }).ToList();
                return PartialView("ChangeMerchantGridIndex", DistributorInfo);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        [HttpPost]
        public JsonResult GetDistributorList(string MemberID = "")
        {
            initpage();////
            try
            {
                var db = new DBContext();
                long memId = long.Parse(MemberID);
                var distributorlist = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == memId);
                return Json(distributorlist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
                throw ex;
            }

        }
        [HttpPost]
        public JsonResult GetMerchantList(string MemberID = "")
        {
            initpage();////
            try
            {
                var db = new DBContext();
                long memId = long.Parse(MemberID);
                var distributorlist = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == memId);
                return Json(distributorlist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("", JsonRequestBehavior.AllowGet);
                throw ex;
            }

        }
        [HttpPost]
        public JsonResult PostChangeSuperIDInformation(TBL_MASTER_MEMBER objvalchange)
        {
            initpage();////
            try
            {
                var db = new DBContext();
                var Distinfor = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == objvalchange.DISTRIBUTOR_ID);
                Distinfor.INTRODUCER = objvalchange.SUPER_ID;
                Distinfor.COMPANY_GST_NO = "1234567890";
                db.Entry(Distinfor).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json("Super id change successfully");
            }
            catch (Exception ex)
            {
                return Json("Issue arises please try after sometime");

                throw ex;
            }

        }

        [HttpPost]
        public JsonResult PostChangeMerchantInformation(TBL_MASTER_MEMBER objvalchange)
        {
            initpage();////
            try
            {
                var db = new DBContext();
                var Distinfor = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == objvalchange.MEM_ID);
                Distinfor.INTRODUCER = objvalchange.DISTRIBUTOR_ID;
                Distinfor.COMPANY_GST_NO = "1234567890";
                db.Entry(Distinfor).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json("Super id change successfully");
            }
            catch (Exception ex)
            {
                return Json("Issue arises please try after sometime");

                throw ex;
            }

        }


    }
}