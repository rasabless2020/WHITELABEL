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

namespace WHITELABEL.Web.Areas.Super.Controllers
{
    [Authorize]
    public class SuperServiceController : SuperBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        //public void initpage()
        //{
        //    try
        //    {
        //        ViewBag.ControllerName = "Stockist API Label";

        //        SystemClass sclass = new SystemClass();
        //        string userID = sclass.GetLoggedUser();
        //        long userid = long.Parse(userID);
        //        var dbmain = new DBContext();
        //        if (userID != null && userID != "")
        //        {
        //            TBL_MASTER_MEMBER currUser = dbmain.TBL_MASTER_MEMBER.SingleOrDefault(c => c.MEM_ID == userid && c.ACTIVE_MEMBER == true);
        //            if (currUser != null)
        //            {
        //                Session["SuperDistributorId"] = currUser.MEM_ID;
        //                // Session["UserName"] = currUser.UserName;
        //            }
        //            else
        //            {
        //                Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //                return;
        //            }
        //        }
        //        if (Session["SuperDistributorId"] == null)
        //        {
        //            //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //            //Response.Redirect(Url.Action("Index", "StockistDashboard", new { area = "SuperStockist" }));
        //            Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //            return;
        //        }
        //        bool Islogin = false;

        //        if (Session["SuperDistributorId"] != null)
        //        {
        //            Islogin = true;
        //            ViewBag.CurrentUserId = MemberCurrentUser.MEM_ID;
        //        }
        //        ViewBag.Islogin = Islogin;

        //    }
        //    catch (Exception e)
        //    {
        //        //ViewBag.UserName = CurrentUser.UserId;
        //        Console.WriteLine(e.InnerException);
        //        return;

        //    }
        //}

        public void initpage()
        {
            try
            {
                if (DomainNameCheck.DomainChecking(Session["DOMAINNAME"].ToString(), Request.Url.Host) == false)
                {
                    Response.Redirect(Url.Action("DomainError", "Login", new { area = "" }));
                }
                ViewBag.ControllerName = "Super";
                if (Session["SuperDistributorId"] == null)
                {
                    Response.Redirect(Url.Action("Logout", "Login", new { area = "" }));
                    return;
                }
                bool Islogin = false;

                if (Session["SuperDistributorId"] != null)
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
        // GET: Super/SuperService
        public ActionResult Index()
        {
            if (Session["SuperDistributorId"] != null)
            {
                initpage();
                return View();
            }
            else
            {
                Session["SuperDistributorId"] = null;
                Session["SuperDistributorUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("SuperDistributorId");
                Session.Remove("SuperDistributorUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "SuperLogin", new { area = "Super" });
            }
            
        }

        public ActionResult ServiceDetails(string memid = "")
        {
            initpage();////
            try
            {
                var db = new DBContext();
                long UserId = MemberCurrentUser.MEM_ID;
                if (memid != "")
                {
                    string decrptSlId = Decrypt.DecryptMe(memid);
                    long Memid = long.Parse(decrptSlId);

                    var userinfo = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.UNDER_WHITE_LEVEL == Memid);
                    ViewBag.decrptSlId = Encrypt.EncryptMe(Memid.ToString());
                    //var ServiceInfo = db.TBL_WHITELABLE_SERVICE.Where(x => x.MEMBER_ID == Memid).FirstOrDefault();
                    var memberinfo = (from x in db.TBL_WHITELABLE_SERVICE
                                      join
                                        y in db.TBL_SETTINGS_SERVICES_MASTER on x.SERVICE_ID equals y.SLN
                                      join
                                        w in db.TBL_MASTER_MEMBER on x.MEMBER_ID equals w.MEM_ID
                                      where x.MEMBER_ID == Memid && y.ACTIVESTATUS==true
                                      select new
                                      {
                                          Member_ID = x.MEMBER_ID,
                                          UserName = w.UName,
                                          ServiceName = y.SERVICE_NAME,
                                          SLN = x.SL_NO,
                                          ActiveService = x.ACTIVE_SERVICE

                                      }).AsEnumerable().Select(z => new TBL_WHITELABLE_SERVICE
                                      {
                                          MEMBER_ID = z.Member_ID,
                                          UserName = z.UserName,
                                          ServiceName = z.ServiceName,
                                          SL_NO = z.SLN,
                                          ACTIVE_SERVICE = z.ActiveService
                                      }).ToList();
                    ViewBag.checkVal = "1";
                    var memberService = (from x in db.TBL_MASTER_MEMBER
                                         where x.INTRODUCER == UserId
                                         select new
                                         {
                                             MEM_ID = x.MEM_ID,
                                             UName = x.MEMBER_NAME
                                         }).AsEnumerable().Select(z => new MemberView
                                         {
                                             IDValue = Encrypt.EncryptMe(z.MEM_ID.ToString()),
                                             TextValue = z.UName
                                         }).ToList().Distinct();
                    ViewBag.MemberService = new SelectList(memberService, "IDValue", "TextValue");
                    return View(memberinfo);
                }
                else
                {

                    //var memberService = db.TBL_MASTER_MEMBER.ToList().Distinct();
                    var memberService = (from x in db.TBL_MASTER_MEMBER
                                         where x.INTRODUCER == UserId
                                         select new
                                         {
                                             MEM_ID = x.MEM_ID,
                                             UName = x.MEMBER_NAME
                                         }).AsEnumerable().Select(z => new MemberView
                                         {
                                             IDValue = Encrypt.EncryptMe(z.MEM_ID.ToString()),
                                             TextValue = z.UName
                                         }).ToList().Distinct();
                    ViewBag.MemberService = new SelectList(memberService, "IDValue", "TextValue");
                    ViewBag.checkVal = "0";
                    var memberinfo = new List<TBL_WHITELABLE_SERVICE>();
                    return View(memberinfo);
                }
                //return View();
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  SuperService(Super), method:- ServiceDetails (GET) Line No:- 167", ex);
                return RedirectToAction("Exception", "ErrorHandler", new { area = "" });
                throw ex;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UpdateActiveService(string memberId, string Id, bool isActive)
        {
            initpage();////
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    string decrptMemId = Decrypt.DecryptMe(memberId);
                    long MembId = long.Parse(decrptMemId);
                    string decrptSlID = Decrypt.DecryptMe(Id);
                    long SlId = long.Parse(decrptSlID);
                    var updateServiceStatus = await db.TBL_WHITELABLE_SERVICE.Where(x => x.SL_NO == SlId && x.MEMBER_ID == MembId).FirstOrDefaultAsync();
                    if (updateServiceStatus != null)
                    {
                        updateServiceStatus.ACTIVE_SERVICE = isActive;
                        db.Entry(updateServiceStatus).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();
                        ContextTransaction.Commit();
                        return Json(new { Result = "true" });
                    }
                    return Json(new { Result = "true" });
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    Logger.Error("Controller:-  SuperService(Super), method:- UpdateActiveService (POST) Line No:- 199", ex);
                    return Json(new { Result = "false" });
                }
            }

        }

        public JsonResult GetserviceInfoMemberWise(string memid = "")
        {
            initpage();////
            try
            {
                if (!string.IsNullOrEmpty(memid))
                {
                    var db = new DBContext();
                    string decrptSlId = Decrypt.DecryptMe(memid);
                    long Memid = long.Parse(decrptSlId);
                    var userinfo = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == Memid);
                    ViewBag.UserName = userinfo.UName;
                    //var ServiceInfo = db.TBL_WHITELABLE_SERVICE.Where(x => x.MEMBER_ID == Memid).FirstOrDefault();
                    var memberinfo = (from x in db.TBL_WHITELABLE_SERVICE
                                      join
                                        y in db.TBL_SETTINGS_SERVICES_MASTER on x.SERVICE_ID equals y.SLN
                                      join
                                        w in db.TBL_MASTER_MEMBER on x.MEMBER_ID equals w.MEM_ID
                                      where x.MEMBER_ID == Memid
                                      select new
                                      {
                                          Member_ID = x.MEMBER_ID,
                                          UserName = w.UName,
                                          ServiceName = y.SERVICE_NAME,
                                          SLN = x.SL_NO,
                                          ActiveService = x.ACTIVE_SERVICE

                                      }).AsEnumerable().Select(z => new TBL_WHITELABLE_SERVICE
                                      {
                                          MEMBER_ID = z.Member_ID,
                                          UserName = z.UserName,
                                          ServiceName = z.ServiceName,
                                          SL_NO = z.SLN,
                                          ACTIVE_SERVICE = z.ActiveService
                                      }).ToList();
                    return Json(new { Result = "true", infor = memberinfo });
                }
                return Json(new { Result = "true" });
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  SuperService(Super), method:- GetserviceInfoMemberWise (POST) Line No:- 247", ex);
                throw;
            }

        }

    }
}