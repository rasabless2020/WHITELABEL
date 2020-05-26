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
using WHITELABEL.Web.Areas.Admin.Models;
using WHITELABEL.Web.Controllers;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;


namespace WHITELABEL.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class MemberTotalDistributorListController : AdminBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        // GET: Admin/MemberTotalDistributorList
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
            try
            {
                var db = new DBContext();

                string[] SuperMemId = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == MemberCurrentUser.MEM_ID).Select(a => a.MEM_ID.ToString()).ToArray();
                string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(x => SuperMemId.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                var memberService = (from x in db.TBL_MASTER_MEMBER
                                         //where DistributorMemId.Contains(x.INTRODUCER.ToString())
                                     where x.INTRODUCER == MemberCurrentUser.MEM_ID
                                     select new
                                     {
                                         MEM_ID = x.MEM_ID,
                                         UName = x.UName
                                     }).AsEnumerable().Select(z => new MemberView
                                     {
                                         IDValue = z.MEM_ID.ToString(),
                                         TextValue = z.UName
                                     }).ToList().Distinct();
                ViewBag.MemberService = new SelectList(memberService, "IDValue", "TextValue");

                //var memberService = (from x in db.TBL_MASTER_MEMBER
                //                     where x.INTRODUCER == MemberCurrentUser.MEM_ID
                //                     select new
                //                     {
                //                         MEM_ID = x.MEM_ID,
                //                         UName = x.UName
                //                     }).AsEnumerable().Select(z => new MemberView
                //                     {
                //                         IDValue = z.MEM_ID.ToString(),
                //                         TextValue = z.UName
                //                     }).ToList().Distinct();
                ViewBag.MemberService = new SelectList(memberService, "IDValue", "TextValue");
                return View();
            }
            catch (Exception ex)
            {

                throw;
            }
            initpage();
            
        }
        //public PartialViewResult IndexGrid(string search = "")
        public PartialViewResult IndexGrid(string search = "")
        {

            var db = new DBContext();
            long Mem_ID = 0;
            long.TryParse(search, out Mem_ID);
            if (search != "")
            {
                var GetDistList = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == Mem_ID).ToList();                
                return PartialView("IndexGrid", GetDistList);
            }
            else
            { 
                //var GetDistList = db.TBL_MASTER_MEMBER.Where(x => x.INTRODUCER == Mem_ID).ToList();
                var GetDistList = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).FirstOrDefault();
                return PartialView("IndexGrid", GetDistList);
            }
        }
        [HttpPost]
        public JsonResult GetSuperDistributor(long Disid)
        {
            //string countrystring = "select * from tbl_state where countrycode='" + id + "'";
            long dis_Id = long.Parse(Disid.ToString());
            var db = new DBContext();
            var memberService = (from x in db.TBL_MASTER_MEMBER
                                 where x.INTRODUCER == dis_Id
                                 select new
                                 {
                                     MEM_ID = x.MEM_ID,
                                     UName = x.UName
                                 }).AsEnumerable().Select(z => new MemberView
                                 {
                                     IDValue = z.MEM_ID.ToString(),
                                     TextValue = z.UName
                                 }).ToList().Distinct();
            return Json(memberService, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GetDistributorMerchant(long Disid)
        {
            //string countrystring = "select * from tbl_state where countrycode='" + id + "'";
            long dis_Id = long.Parse(Disid.ToString());
            var db = new DBContext();
            var memberService = (from x in db.TBL_MASTER_MEMBER
                                 where x.INTRODUCER == dis_Id
                                 select new
                                 {
                                     MEM_ID = x.MEM_ID,
                                     UName = x.UName
                                 }).AsEnumerable().Select(z => new MemberView
                                 {
                                     IDValue = z.MEM_ID.ToString(),
                                     TextValue = z.UName
                                 }).ToList().Distinct();
            return Json(memberService, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MerchantList()
        {
            initpage();
            return View();
        }
        public PartialViewResult MerchantGrid()
        {

            var db = new DBContext();
            var GetMerchatList = (from a in db.TBL_MASTER_MEMBER join b in db.TBL_MASTER_MEMBER on a.MEM_ID equals b.INTRODUCER
                                  join c in db.TBL_MASTER_MEMBER on b.MEM_ID equals c.INTRODUCER join
                                  d in db.TBL_MASTER_MEMBER on c.MEM_ID equals d.INTRODUCER where d.UNDER_WHITE_LEVEL == MemberCurrentUser.MEM_ID
                                  select new
                                  {
                                      MEM_ID = d.MEM_ID,
                                      User_Name = d.UName,
                                      MEMBER_NAME = d.MEMBER_NAME,
                                      MEMBER_MOBILE = d.MEMBER_MOBILE,
                                      EMAIL_ID = d.EMAIL_ID,
                                      PASSWORD_MD5 = d.PAN_FILE_NAME,
                                      BALANCE = d.BALANCE,
                                      COMPANY = d.COMPANY,
                                      BLOCKED_BALANCE = d.BLOCKED_BALANCE,
                                      ACTIVE_MEMBER = d.ACTIVE_MEMBER,
                                      JOINING_DATE = d.JOINING_DATE,
                                      SECURITY_PIN_MD5 = d.SECURITY_PIN_MD5
                                  }).AsEnumerable().Select(z => new TBL_MASTER_MEMBER
                                  {
                                      MEM_ID = z.MEM_ID,
                                      UName = z.User_Name,
                                      MEMBER_NAME = z.MEMBER_NAME,
                                      MEMBER_MOBILE = z.MEMBER_MOBILE,
                                      EMAIL_ID = z.EMAIL_ID,
                                      User_pwd = z.PASSWORD_MD5,
                                      BALANCE=z.BALANCE,
                                      BLOCKED_BALANCE=z.BLOCKED_BALANCE,
                                      COMPANY=z.COMPANY,
                                      ACTIVE_MEMBER=z.ACTIVE_MEMBER,
                                      JOINING_DATE=z.JOINING_DATE,
                                      SECURITY_PIN_MD5=z.SECURITY_PIN_MD5
                                }).ToList().Distinct();
            //long Mem_ID = 0;
            //long.TryParse(search, out Mem_ID);
            //var GetDistList = db.TBL_MASTER_MEMBER.Where(x => x.INTRODUCER == Mem_ID).ToList();
            return PartialView("IndexGrid", GetMerchatList);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetMemberPasswordDetails(string id="")
        {
         
            try
            {
                EmailHelper emailhelper = new EmailHelper();
                var db = new DBContext();
                long memId = long.Parse(id);
                var meminfo = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == memId).FirstOrDefaultAsync();
                if (meminfo != null)
                {
                    //string decriptpass = Decrypt.DecryptMe(meminfo.User_pwd);
                    string password = meminfo.User_pwd;

                }
                return Json(meminfo, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> MemberGetStatusUpdate(string id, string statusval)
        {
          
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    long memid = long.Parse(id);

                    bool memberstatus = false;
                    if (statusval == "True")
                    {
                        memberstatus = false;
                    }
                    else
                    {
                        memberstatus = true;
                    }
                    var memberlist = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == memid).FirstOrDefaultAsync();
                    if (statusval == "True")
                    {
                        memberlist.ACTIVE_MEMBER = false;
                    }
                    else
                    {
                        memberlist.ACTIVE_MEMBER = true;
                    }

                    memberlist.IS_DELETED = true;

                    db.Entry(memberlist).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();
                    ContextTransaction.Commit();
                    return Json(new { Result = "true" });
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    return Json(new { Result = "false" });
                }
            }


        }


        public async Task<ActionResult> UpdateMemberDetails(string memid = "")
        {
            initpage();////
            if (Session["WhiteLevelUserId"] != null)
            {
                try
                {
                    if (memid != "")
                    {                       
                        var dbcontext = new DBContext();
                        var model = new TBL_MASTER_MEMBER();
                        var memberrole = await dbcontext.TBL_MASTER_MEMBER_ROLE.Where(x => x.ROLE_NAME != "WHITE LEVEL" && x.ROLE_NAME != "API USER").ToListAsync();
                        ViewBag.RoleDetails = new SelectList(memberrole, "ROLE_ID", "ROLE_NAME");
                        ViewBag.checkstatus = "1";
                        string decrptSlId = Decrypt.DecryptMe(memid);
                        //long Memid = long.Parse(decrptSlId);
                        long idval = long.Parse(decrptSlId);
                        ViewBag.checkmail = true;
                        model = await dbcontext.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == idval);
                        if (model.MEMBER_ROLE == 3)
                        {
                            model.SuperName = "SUPER";
                        }
                        else if (model.MEMBER_ROLE == 4) { model.SuperName = "DISTRIBUTOR"; }
                        else if (model.MEMBER_ROLE == 5)
                        {
                            model.SuperName = "MERCHANT";
                        }
                        model.BLOCKED_BALANCE = Math.Round(Convert.ToDecimal(model.BLOCKED_BALANCE), 0); Session.Remove("msg");
                        model.GST_FLAG = model.GST_FLAG;
                        model.TDS_FLAG = model.TDS_FLAG;
                        Session.Remove("msg");
                        Session["msg"] = null;

                        var GSTValueID = dbcontext.TBL_TAX_MASTERS.Where(x => x.TAX_NAME == "GST").ToList();
                        ViewBag.GSTValue = new SelectList(GSTValueID, "SLN", "TAX_NAME");
                        var TDSValueID = dbcontext.TBL_TAX_MASTERS.Where(x => x.TAX_NAME == "TDS").ToList();
                        ViewBag.TDSValue = new SelectList(TDSValueID, "SLN", "TAX_NAME");
                        return View(model);
                        //return View("CreateMember", "MemberAPILabel", new {area ="Admin" },model);
                    }
                    else
                    {
                        var dbcontext = new DBContext();
                        ViewBag.checkstatus = "0";
                        //var memberrole = await dbcontext.TBL_MASTER_MEMBER_ROLE.Where(x => x.ROLE_NAME != "WHITE LEVEL" && x.ROLE_NAME != "API USER").ToListAsync();
                        var memberrole = await dbcontext.TBL_MASTER_MEMBER_ROLE.Where(x => x.ROLE_NAME == "SUPER DISTRIBUTOR").ToListAsync();
                        ViewBag.RoleDetails = new SelectList(memberrole, "ROLE_ID", "ROLE_NAME");
                        var user = new TBL_MASTER_MEMBER();
                        ViewBag.checkmail = false;
                        user.UName = "";
                        Session.Remove("msg");
                        Session["msg"] = null;
                        var GSTValueID = dbcontext.TBL_TAX_MASTERS.Where(x => x.TAX_NAME == "GST").ToList();
                        ViewBag.GSTValue = new SelectList(GSTValueID, "SLN", "TAX_NAME");
                        var TDSValueID = dbcontext.TBL_TAX_MASTERS.Where(x => x.TAX_NAME == "TDS").ToList();
                        ViewBag.TDSValue = new SelectList(TDSValueID, "SLN", "TAX_NAME");
                        ModelState.Clear();
                        return View();
                    }
                }
                catch (Exception ex)
                {
                    //throw ex;
                    Logger.Error("Controller:-  MemberAPILabel(Admin), method:- CreateMember (GET) Line No:- 140", ex);
                    return RedirectToAction("Exception", "ErrorHandler", new { area = "" });
                    //return RedirectToAction("Notfound", "ErrorHandler");
                }
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        //IEnumerable<HttpPostedFileBase> files
        //public async Task<JsonResult> POSTADDSUPERDistributor(TBL_MASTER_MEMBER objsupermem, HttpPostedFileBase AadhaarFile, HttpPostedFileBase PanFile)
        public async Task<JsonResult> PostUpdateMemberDetails(TBL_MASTER_MEMBER objsupermem)
        {
            initpage();////
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    long MemID = 0;                    
                    var GetMemberDetails = await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x=>x.MEM_ID==objsupermem.MEM_ID);
                    if (GetMemberDetails.MEMBER_ROLE == 3) {
                        GetMemberDetails.SuperName = "SUPER";
                    }
                    else if (GetMemberDetails.MEMBER_ROLE == 4) { GetMemberDetails.SuperName = "DISTRIBUTOR"; }
                    else if (GetMemberDetails.MEMBER_ROLE == 5)
                    {
                        GetMemberDetails.SuperName = "MERCHANT";
                    }
                    GetMemberDetails.MEMBER_MOBILE = objsupermem.MEMBER_MOBILE;
                    GetMemberDetails.MEMBER_NAME = objsupermem.MEMBER_NAME;
                    GetMemberDetails.COMPANY = objsupermem.COMPANY;
                    GetMemberDetails.ADDRESS = objsupermem.ADDRESS;
                    GetMemberDetails.CITY = objsupermem.CITY;
                    GetMemberDetails.PIN = objsupermem.PIN;
                    GetMemberDetails.SECURITY_PIN_MD5 = objsupermem.SECURITY_PIN_MD5;
                    GetMemberDetails.COMPANY_GST_NO = objsupermem.COMPANY_GST_NO;
                    GetMemberDetails.PIN = objsupermem.PIN;
                    GetMemberDetails.BLOCKED_BALANCE = objsupermem.BLOCKED_BALANCE;
                    GetMemberDetails.GST_FLAG = objsupermem.GST_FLAG;
                    GetMemberDetails.TDS_FLAG = objsupermem.TDS_FLAG;
                    db.Entry(GetMemberDetails).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();
                    ContextTransaction.Commit();
                    //return View("Index");
                    return Json("Information Update Successfully", JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    Logger.Error("Controller:-  MemberChannelRegistration(Admin), method:- ADDSUPERDISTRIBUTOR (POST) Line No:- 230", ex);
                    throw ex;
                    return Json("Please Try After Sometime", JsonRequestBehavior.AllowGet);

                    //return View("Error", new HandleErrorInfo(ex, "APILabel", "CreateMember"));
                }
            }
        }
    }
}