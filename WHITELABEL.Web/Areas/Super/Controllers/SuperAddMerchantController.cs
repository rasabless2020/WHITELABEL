using log4net;
using NonFactors.Mvc.Grid;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
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
    public class SuperAddMerchantController : SuperBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        //public void initpage()
        //{
        //    try
        //    {
        //        ViewBag.ControllerName = "Super Bank Details";
        //        SystemClass sclass = new SystemClass();
        //        string userID = sclass.GetLoggedUser();
        //        long userid = long.Parse(userID);
        //        var dbmain = new DBContext();
        //        if (userID != null && userID != "")
        //        {
        //            TBL_MASTER_MEMBER currUser = dbmain.TBL_MASTER_MEMBER.SingleOrDefault(c => c.MEM_ID == userid && c.ACTIVE_MEMBER == true && c.MEMBER_ROLE == 3);
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
        // GET: Super/SuperAddMerchant
        public ActionResult Index()
        {
            if (Session["SuperDistributorId"] != null)
            {
                initpage();
                var db = new DBContext();               
                var DistributorList = db.TBL_MASTER_MEMBER.Where(x => x.INTRODUCER == MemberCurrentUser.MEM_ID && x.MEMBER_ROLE == 4).ToList();
                ViewBag.DistributorList = new SelectList(DistributorList, "MEM_ID", "UName");
                var memberrole = db.TBL_MASTER_MEMBER_ROLE.Where(x => x.ROLE_NAME == "RETAILER").ToList();
                ViewBag.RoleDetails = new SelectList(memberrole, "ROLE_ID", "ROLE_NAME");
                var GSTValueID = db.TBL_TAX_MASTERS.Where(x => x.TAX_NAME == "GST").ToList();
                ViewBag.GSTValue = new SelectList(GSTValueID, "SLN", "TAX_NAME");
                var TDSValueID = db.TBL_TAX_MASTERS.Where(x => x.TAX_NAME == "TDS").ToList();
                ViewBag.TDSValue = new SelectList(TDSValueID, "SLN", "TAX_NAME");
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
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<JsonResult> POSTADDMerchantInformation(TBL_MASTER_MEMBER objsupermem, HttpPostedFileBase AadhaarFile, HttpPostedFileBase PanFile)
        public async Task<JsonResult> POSTADDMerchantInformation(TBL_MASTER_MEMBER objsupermem)
        {
            initpage();////
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    //if (objsupermem.AADHAAR_NO != null || AadhaarFile != null)
                    //{
                    //    if (AadhaarFile == null)
                    //    {
                    //        ViewBag.checkstatus = "0";
                    //        ModelState.AddModelError("AADHAAR_NO", "Please Upload Aadhaar Card Image...");
                    //        return Json("Please Upload Aadhaar Card Image...", JsonRequestBehavior.AllowGet);
                    //    }
                    //    else if (objsupermem.AADHAAR_NO == null)
                    //    {
                    //        ViewBag.checkstatus = "0";
                    //        ModelState.AddModelError("AADHAAR_NO", "Please give Aadhaar Card Number...");
                    //        return Json("Please give Aadhaar Card Number...", JsonRequestBehavior.AllowGet);
                    //    }
                    //}
                    //if (objsupermem.PAN_NO != null || PanFile != null)
                    //{
                    //    if (PanFile == null)
                    //    {
                    //        ModelState.AddModelError("AADHAAR_NO", "Please Upload Pan Card Image...");
                    //        return Json("Please Upload Pan Card Image...", JsonRequestBehavior.AllowGet);
                    //    }
                    //    else if (objsupermem.PAN_NO == null)
                    //    {
                    //        ModelState.AddModelError("AADHAAR_NO", "Please give Pan Card Number...");
                    //        return Json("Please give Pan Card Number...", JsonRequestBehavior.AllowGet);
                    //    }

                    //}
                    objsupermem.BALANCE = 0;
                    if (objsupermem.BLOCKED_BALANCE == null)
                    {
                        objsupermem.BLOCKED_BALANCE = 0;
                        objsupermem.BALANCE = 0;
                    }
                    else
                    {
                        objsupermem.BLOCKED_BALANCE = objsupermem.BLOCKED_BALANCE;
                        objsupermem.BALANCE = objsupermem.BLOCKED_BALANCE;
                    }
                    var underWhitelevel = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == MemberCurrentUser.MEM_ID);
                    objsupermem.EMAIL_ID = objsupermem.EMAIL_ID.ToLower();
                    objsupermem.UNDER_WHITE_LEVEL = underWhitelevel.INTRODUCER;
                    objsupermem.INTRODUCER = objsupermem.DISTRIBUTOR_ID;
                    //objsupermem.BLOCKED_BALANCE = 0;
                    objsupermem.ACTIVE_MEMBER = true;
                    objsupermem.IS_DELETED = false;
                    objsupermem.JOINING_DATE = System.DateTime.Now;

                    //objsupermem.CREATED_BY = long.Parse(Session["UserId"].ToString());
                    objsupermem.CREATED_BY = MemberCurrentUser.MEM_ID;
                    //objsupermem.CREATED_BY = CurrentUser.USER_ID;
                    objsupermem.LAST_MODIFIED_DATE = System.DateTime.Now;
                    objsupermem.GST_MODE = 1;
                    objsupermem.TDS_MODE = 1;
                    objsupermem.DUE_CREDIT_BALANCE = 0;
                    objsupermem.CREDIT_BALANCE = 0;
                    objsupermem.IS_TRAN_START = true;
                    db.TBL_MASTER_MEMBER.Add(objsupermem);
                    await db.SaveChangesAsync();
                    string aadhaarfilename = string.Empty;
                    string Pancardfilename = string.Empty;
                    ////Checking file is available to save.  
                    //if (AadhaarFile != null)
                    //{
                    //    string aadharpath = Path.GetFileName(AadhaarFile.FileName);
                    //    string AadharfileName = aadharpath.Substring(aadharpath.LastIndexOf(((char)92)) + 1);
                    //    int index = AadharfileName.LastIndexOf('.');
                    //    string onyName = AadharfileName.Substring(0, index);
                    //    string fileExtension = AadharfileName.Substring(index + 1);

                    //    var AadhaarFileName = objsupermem.MEM_ID + "_" + objsupermem.AADHAAR_NO + "." + fileExtension;
                    //    //var AdharServerSavePath = Path.Combine(Server.MapPath("~/MemberFiles/") + AadhaarFileName);
                    //    var AdharServerSavePath = (Server.MapPath(@"/MemberFiles/") + AadhaarFileName);
                    //    aadhaarfilename = "/MemberFiles/" + AadhaarFileName;
                    //    AadhaarFile.SaveAs(AdharServerSavePath);
                    //}
                    //if (PanFile != null)
                    //{
                    //    string Pannopath = Path.GetFileName(PanFile.FileName);
                    //    string PannofileName = Pannopath.Substring(Pannopath.LastIndexOf(((char)92)) + 1);
                    //    int index = PannofileName.LastIndexOf('.');
                    //    string onyName = PannofileName.Substring(0, index);
                    //    string PanfileExtension = PannofileName.Substring(index + 1);
                    //    var InputPanCard = objsupermem.MEM_ID + "_" + objsupermem.PAN_NO + "." + PanfileExtension;
                    //    //var PanserverSavePath = Path.Combine(Server.MapPath("~/MemberFiles/") + InputPanCard);
                    //    var PanserverSavePath = (Server.MapPath(@"/MemberFiles/") + InputPanCard);
                    //    Pancardfilename = "/MemberFiles/" + InputPanCard;
                    //    PanFile.SaveAs(PanserverSavePath);
                    //}
                    var imageupload = db.TBL_MASTER_MEMBER.Find(objsupermem.MEM_ID);
                    imageupload.AADHAAR_FILE_NAME = aadhaarfilename;
                    imageupload.PAN_FILE_NAME = Pancardfilename;
                    db.Entry(imageupload).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();
                    var servlist = await db.TBL_SETTINGS_SERVICES_MASTER.ToListAsync();
                    foreach (var lst in servlist)
                    {
                        TBL_WHITELABLE_SERVICE objser = new TBL_WHITELABLE_SERVICE()
                        {
                            MEMBER_ID = long.Parse(objsupermem.MEM_ID.ToString()),
                            SERVICE_ID = long.Parse(lst.SLN.ToString()),
                            ACTIVE_SERVICE = false
                        };
                        db.TBL_WHITELABLE_SERVICE.Add(objser);
                        await db.SaveChangesAsync();
                    }
                    //ViewBag.savemsg = "Data Saved Successfully";
                    //Session["msg"] = "Data Saved Successfully";
                    //ContextTransaction.Commit();
                    //throw new Exception();
                    ContextTransaction.Commit();
                    return Json("Merchant Added Successfully", JsonRequestBehavior.AllowGet);
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
        [HttpPost]
        public async Task<JsonResult> CheckEmailAvailability(string emailid)
        {
            initpage();////
            try
            {
                var context = new DBContext();
                var User = await context.TBL_MASTER_MEMBER.Where(model => model.EMAIL_ID == emailid).FirstOrDefaultAsync();
                if (User != null)
                {
                    return Json(new { result = "unavailable" });
                }
                else
                {
                    return Json(new { result = "available" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { result = "unavailable" });
                throw ex;
            }
        }

        public ActionResult ChangeMerchantDistributor()
        {
            initpage();////
            try
            {
                if (Session["SuperDistributorId"] != null)
                {
                    initpage();
                    var db = new DBContext();
                    //var DistributorList = db.TBL_MASTER_MEMBER.Where(x => x.INTRODUCER == MemberCurrentUser.MEM_ID && x.MEMBER_ROLE == 4).ToList();
                    //ViewBag.DistributorList = new SelectList(DistributorList, "MEM_ID", "UName");
                    //var memberrole = db.TBL_MASTER_MEMBER_ROLE.Where(x => x.ROLE_NAME == "RETAILER").ToList();
                    //ViewBag.RoleDetails = new SelectList(memberrole, "ROLE_ID", "ROLE_NAME");
                    //var GSTValueID = db.TBL_TAX_MASTERS.Where(x => x.TAX_NAME == "GST").ToList();
                    //ViewBag.GSTValue = new SelectList(GSTValueID, "SLN", "TAX_NAME");
                    //var TDSValueID = db.TBL_TAX_MASTERS.Where(x => x.TAX_NAME == "TDS").ToList();
                    //ViewBag.TDSValue = new SelectList(TDSValueID, "SLN", "TAX_NAME");
                    var DistributorList = db.TBL_MASTER_MEMBER.Where(x =>x.INTRODUCER  == MemberCurrentUser.MEM_ID && x.MEMBER_ROLE == 4).ToList();
                    ViewBag.DistributorList = new SelectList(DistributorList, "MEM_ID", "UName");
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
                    return RedirectToAction("Index", "Login", new { area = "" });
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return View();
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
        public JsonResult PostChangeMerchantInformation(TBL_MASTER_MEMBER objvalchange)
        {
            initpage();////
            try
            {
                var db = new DBContext();
                var Distinfor = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == objvalchange.MEM_ID);
                Distinfor.INTRODUCER = objvalchange.DISTRIBUTOR_ID;
                //Distinfor.COMPANY_GST_NO = "1234567890";
                db.Entry(Distinfor).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Json("Merchant Introducer id change successfully");
            }
            catch (Exception ex)
            {
                return Json("Issue arises please try after sometime");

                throw ex;
            }

        }

    }
}