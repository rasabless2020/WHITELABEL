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
    public class ServiceController : BaseController
    {
        public void initpage()
        {
            try
            {
                ViewBag.ControllerName = "Service Details";
                SystemClass sclass = new SystemClass();
                string userID = sclass.GetLoggedUser();
                long userid = long.Parse(userID);
                var dbmain = new DBContext();
                if (userID != null && userID != "")
                {
                    TBL_AUTH_ADMIN_USER currUser = dbmain.TBL_AUTH_ADMIN_USERS.SingleOrDefault(c => c.USER_ID == userid && c.ACTIVE_USER == true);

                    if (currUser != null)
                    {
                        Session["UserId"] = currUser.USER_ID;
                        // Session["UserName"] = currUser.UserName;
                    }
                }
                if (Session["UserId"] == null)
                {
                    Response.Redirect("~/Login/LogOut");
                    return;
                }
                bool Islogin = false;

                if (Session["UserId"] != null)
                {
                    Islogin = true;
                    ViewBag.CurrentUserId = CurrentUser.USER_ID;
                }
                ViewBag.Islogin = Islogin;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException);
                return;

            }
        }
        // GET: Service
        public ActionResult Index()
        {
            initpage();
            return View();
        }
        public ActionResult ServiceDetails(string memid = "")
        {
            try
            {
                var db = new DBContext();
                if (memid != "")
                {
                    string decrptSlId = Decrypt.DecryptMe(memid);
                    long Memid = long.Parse(decrptSlId);
                    var userinfo = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == Memid);
                    ViewBag.decrptSlId = Encrypt.EncryptMe(Memid.ToString());
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
                    ViewBag.checkVal = "1";
                    var memberService = (from x in db.TBL_MASTER_MEMBER
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
                    var memberService = (from x in db.TBL_MASTER_MEMBER select new
                            {
                        MEM_ID =x.MEM_ID,
                        UName=x.MEMBER_NAME
                    }).AsEnumerable().Select(z=>new MemberView
                    {
                        IDValue= Encrypt.EncryptMe(z.MEM_ID.ToString()),
                        TextValue=z.UName
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
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult UpdateActiveService(string memberId, string Id,bool isActive)
        {
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {

                    string decrptMemId = Decrypt.DecryptMe(memberId);
                    long MembId = long.Parse(decrptMemId);
                    string decrptSlID = Decrypt.DecryptMe(Id);
                    long SlId = long.Parse(decrptSlID);
                    var updateServiceStatus = db.TBL_WHITELABLE_SERVICE.Where(x => x.SL_NO == SlId && x.MEMBER_ID == MembId).FirstOrDefault();
                    if (updateServiceStatus != null)
                    {
                        updateServiceStatus.ACTIVE_SERVICE = isActive;
                        db.Entry(updateServiceStatus).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        ContextTransaction.Commit();
                        return Json(new { Result = "true" });
                    }
                    return Json(new { Result = "true" });
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    return Json(new { Result = "false" });
                }
            }
               
        }

        public JsonResult GetserviceInfoMemberWise(string memid = "")
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

    }
}