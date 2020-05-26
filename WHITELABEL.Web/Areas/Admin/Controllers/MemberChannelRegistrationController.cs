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
    public class MemberChannelRegistrationController : AdminBaseController
    {
        // GET: Admin/MemberChannelRegistration
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        //public void initpage()
        //{
        //    try
        //    {
        //        ViewBag.ControllerName = "White Label";
        //        SystemClass sclass = new SystemClass();
        //        string userID = sclass.GetLoggedUser();
        //        long userid = long.Parse(userID);
        //        var dbmain = new DBContext();
        //        if (userID != null && userID != "")
        //        {
        //            TBL_MASTER_MEMBER currUser = dbmain.TBL_MASTER_MEMBER.SingleOrDefault(c => c.MEM_ID == userid && c.ACTIVE_MEMBER == true && c.MEMBER_ROLE == 1);
        //            if (currUser != null)
        //            {
        //                Session["WhiteLevelUserId"] = currUser.MEM_ID;
        //            }
        //            else
        //            {
        //                Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //                return;
        //            }
        //        }
        //        if (Session["WhiteLevelUserId"] == null)
        //        {
        //            Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //            return;
        //        }
        //        bool Islogin = false;

        //        if (Session["WhiteLevelUserId"] != null)
        //        {
        //            Islogin = true;
        //            ViewBag.CurrentUserId = MemberCurrentUser.MEM_ID;
        //        }
        //        ViewBag.Islogin = Islogin;

        //    }
        //    catch (Exception e)
        //    {
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

        public ActionResult ADDSUPERDISTRIBUTOR()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                var db = new DBContext();
                initpage();
                var StateName = db.TBL_STATES.ToList();
                ViewBag.StateNameList = new SelectList(StateName, "STATEID", "STATENAME");
                var memberrole = db.TBL_MASTER_MEMBER_ROLE.Where(x => x.ROLE_NAME == "SUPER DISTRIBUTOR").ToList();
                ViewBag.RoleDetails = new SelectList(memberrole, "ROLE_ID", "ROLE_NAME");
                var GSTValueID = db.TBL_TAX_MASTERS.Where(x => x.TAX_NAME == "GST").ToList();
                ViewBag.GSTValue = new SelectList(GSTValueID, "SLN", "TAX_NAME");
                var TDSValueID = db.TBL_TAX_MASTERS.Where(x => x.TAX_NAME == "TDS").ToList();
                ViewBag.TDSValue = new SelectList(TDSValueID, "SLN", "TAX_NAME");
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        //IEnumerable<HttpPostedFileBase> files
        //public async Task<JsonResult> POSTADDSUPERDistributor(TBL_MASTER_MEMBER objsupermem, HttpPostedFileBase AadhaarFile, HttpPostedFileBase PanFile)
        public async Task<JsonResult> POSTADDSUPERDistributor(TBL_MASTER_MEMBER objsupermem)
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
                    objsupermem.EMAIL_ID = objsupermem.EMAIL_ID.ToLower();
                    objsupermem.UNDER_WHITE_LEVEL = MemberCurrentUser.MEM_ID;
                    objsupermem.INTRODUCER = MemberCurrentUser.MEM_ID;
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
                    //return View("Index");
                    return Json("Super Added Successfully", JsonRequestBehavior.AllowGet);
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
        public ActionResult ADDDISTRIBUTOR()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                var db = new DBContext();
                initpage();
                var StateName = db.TBL_STATES.ToList();
                ViewBag.StateNameList = new SelectList(StateName, "STATEID", "STATENAME");
                var Supermember = db.TBL_MASTER_MEMBER.Where(x => x.UNDER_WHITE_LEVEL == MemberCurrentUser.MEM_ID && x.MEMBER_ROLE == 3).ToList();
                ViewBag.SUPERList = new SelectList(Supermember, "MEM_ID", "UName");

                var memberrole = db.TBL_MASTER_MEMBER_ROLE.Where(x => x.ROLE_NAME == "DISTRIBUTOR").ToList();
                ViewBag.RoleDetails = new SelectList(memberrole, "ROLE_ID", "ROLE_NAME");
                var GSTValueID = db.TBL_TAX_MASTERS.Where(x => x.TAX_NAME == "GST").ToList();
                ViewBag.GSTValue = new SelectList(GSTValueID, "SLN", "TAX_NAME");
                var TDSValueID = db.TBL_TAX_MASTERS.Where(x => x.TAX_NAME == "TDS").ToList();
                ViewBag.TDSValue = new SelectList(TDSValueID, "SLN", "TAX_NAME");
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<JsonResult> POSTADDDistributorDetails(TBL_MASTER_MEMBER objsupermem, HttpPostedFileBase AadhaarFile, HttpPostedFileBase PanFile)
        public async Task<JsonResult> POSTADDDistributorDetails(TBL_MASTER_MEMBER objsupermem)
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
                    objsupermem.EMAIL_ID = objsupermem.EMAIL_ID.ToLower();
                    objsupermem.UNDER_WHITE_LEVEL = MemberCurrentUser.MEM_ID;
                    objsupermem.INTRODUCER = MemberCurrentUser.MEM_ID;
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
                    return Json("Distributor Added Successfully", JsonRequestBehavior.AllowGet);
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


        public ActionResult ADDMERCHANT()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                var db = new DBContext();
                initpage();
                var StateName = db.TBL_STATES.ToList();
                ViewBag.StateNameList = new SelectList(StateName, "STATEID", "STATENAME");
                var DistributorList = db.TBL_MASTER_MEMBER.Where(x => x.UNDER_WHITE_LEVEL == MemberCurrentUser.MEM_ID && x.MEMBER_ROLE == 4).ToList();
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
        //public async Task<JsonResult> POSTADDMerchantDetails(TBL_MASTER_MEMBER objsupermem, HttpPostedFileBase AadhaarFile, HttpPostedFileBase PanFile)
        public async Task<JsonResult> POSTADDMerchantDetails(TBL_MASTER_MEMBER objsupermem)
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
                    objsupermem.EMAIL_ID = objsupermem.EMAIL_ID.ToLower();
                    objsupermem.UNDER_WHITE_LEVEL = MemberCurrentUser.MEM_ID;
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

    }
}