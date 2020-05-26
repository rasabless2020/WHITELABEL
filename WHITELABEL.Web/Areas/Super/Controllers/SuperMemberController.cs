﻿using log4net;
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
    public class SuperMemberController : SuperBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        //public void initpage()
        //{
        //    try
        //    {
        //        ViewBag.ControllerName = "Super member Create";

        //        SystemClass sclass = new SystemClass();
        //        string userID = sclass.GetLoggedUser();
        //        long userid = long.Parse(userID);
        //        var dbmain = new DBContext();
        //        if (userID != null && userID != "")
        //        {
        //            TBL_MASTER_MEMBER currUser = dbmain.TBL_MASTER_MEMBER.SingleOrDefault(c => c.MEM_ID == userid && c.ACTIVE_MEMBER == true && c.MEMBER_ROLE==3);
        //            if (currUser != null)
        //            {
        //                Session["SuperDistributorId"] = currUser.MEM_ID;
        //            }
        //            else
        //            {
        //                Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //                return;
        //            }
        //        }
        //        if (Session["SuperDistributorId"] == null)
        //        {
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
        //        Console.WriteLine(e.InnerException);
        //        return;
        //    }
        //}


        // GET: Super/SuperMember

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

        public PartialViewResult IndexGrid()
        {
            try
            {
                //var dbcontext = new DBContext();
                //var memberinfo = dbcontext.TBL_MASTER_MEMBER.Where(x=>x.IS_DELETED==false).ToList();
                //// Only grid query values will be available here.
                //return PartialView("IndexGrid", memberinfo);
                //return PartialView(CreateExportableGrid());
                long userid = MemberCurrentUser.MEM_ID;
                var dbcontext = new DBContext();
                //var memberinfo = dbcontext.TBL_MASTER_MEMBER.Where(x => x.IS_DELETED == false && x.CREATED_BY == userid).ToList();
                var memberinfo = dbcontext.TBL_MASTER_MEMBER.Where(x => x.INTRODUCER == userid).ToList();
                return PartialView("IndexGrid", memberinfo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ActionResult> CreateMember(string memid = "")
        {
            initpage();////
            //long userid = long.Parse(Session["UserId"].ToString());
            if (Session["SuperDistributorId"] != null)
            {
                try
                {
                    if (memid != "")
                    {
                        var dbcontext = new DBContext();

                        var model = new TBL_MASTER_MEMBER();
                        var memberrole = await dbcontext.TBL_MASTER_MEMBER_ROLE.Where(x => x.ROLE_NAME != "WHITE LEVEL" && x.ROLE_NAME != "API USER" && x.ROLE_NAME != "SUPER DISTRIBUTOR").ToListAsync();
                        ViewBag.RoleDetails = new SelectList(memberrole, "ROLE_ID", "ROLE_NAME");
                        ViewBag.checkstatus = "1";
                        string decrptSlId = Decrypt.DecryptMe(memid);
                        //long Memid = long.Parse(decrptSlId);
                        long idval = long.Parse(decrptSlId);
                        model = await dbcontext.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == idval);
                        model.BLOCKED_BALANCE = Math.Round(Convert.ToDecimal(model.BLOCKED_BALANCE), 0); Session.Remove("msg");
                        ViewBag.checkmail = true;
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
                        var memberrole = await dbcontext.TBL_MASTER_MEMBER_ROLE.Where(x => x.ROLE_NAME != "WHITE LEVEL" && x.ROLE_NAME != "API USER" && x.ROLE_NAME != "SUPER DISTRIBUTOR").ToListAsync();
                        ViewBag.RoleDetails = new SelectList(memberrole, "ROLE_ID", "ROLE_NAME");
                        var user = new TBL_MASTER_MEMBER();
                        user.UName = "";
                        ViewBag.checkmail = false;
                        Session.Remove("msg");
                        Session["msg"] = null;
                        var GSTValueID = dbcontext.TBL_TAX_MASTERS.Where(x => x.TAX_NAME == "GST").ToList();
                        ViewBag.GSTValue = new SelectList(GSTValueID, "SLN", "TAX_NAME");
                        var TDSValueID = dbcontext.TBL_TAX_MASTERS.Where(x => x.TAX_NAME == "TDS").ToList();
                        ViewBag.TDSValue = new SelectList(TDSValueID, "SLN", "TAX_NAME");
                        return View();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("Controller:-  SuperMember(Super), method:- CreateMember (GET) Line No:- 143", ex);
                    return RedirectToAction("Exception", "ErrorHandler", new { area = "" });
                    
                }
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(true)]
        public async Task<ActionResult> CreateMember(TBL_MASTER_MEMBER value, HttpPostedFileBase AadhaarFile, HttpPostedFileBase PanFile)
        {
            initpage();////
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var whiteleveluser = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).FirstOrDefaultAsync();

                    var CheckUser = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == value.MEM_ID).FirstOrDefaultAsync();
                    if (CheckUser == null)
                    {
                        if (AadhaarFile != null)
                        {
                            if (AadhaarFile == null)
                            {
                                ViewBag.checkstatus = "0";
                                ModelState.AddModelError("AADHAAR_NO", "Please Upload Aadhaar Card Image...");
                                return View("CreateMember", value);
                            }
                            else if (value.AADHAAR_NO == null)
                            {
                                ViewBag.checkstatus = "0";
                                ModelState.AddModelError("AADHAAR_NO", "Please give Aadhaar Card Number...");
                                return View("CreateMember", value);
                            }
                        }
                        if (PanFile != null)
                        {
                            if (PanFile == null)
                            {
                                ModelState.AddModelError("AADHAAR_NO", "Please Upload Pan Card Image...");
                                return View("CreateMember", value);
                            }
                            else if (value.PAN_NO == null)
                            {
                                ModelState.AddModelError("AADHAAR_NO", "Please give Pan Card Number...");
                                return View("CreateMember", value);
                            }

                        }
                        value.BALANCE = 0;
                        if (value.BLOCKED_BALANCE == null)
                        {
                            value.BLOCKED_BALANCE = 0;
                        }
                        else
                        {
                            value.BLOCKED_BALANCE = value.BLOCKED_BALANCE;
                        }
                        value.AADHAAR_NO = value.AADHAAR_NO;
                        value.PAN_NO = value.PAN_NO;
                        value.EMAIL_ID = value.EMAIL_ID.ToLower();
                        value.UNDER_WHITE_LEVEL = whiteleveluser.UNDER_WHITE_LEVEL;
                        value.INTRODUCER = MemberCurrentUser.MEM_ID;
                        //value.BLOCKED_BALANCE = 0;
                        value.ACTIVE_MEMBER = true;
                        value.IS_DELETED = false;
                        value.JOINING_DATE = System.DateTime.Now;
                        //value.CREATED_BY = long.Parse(Session["UserId"].ToString());
                        value.CREATED_BY = MemberCurrentUser.MEM_ID;
                        //value.CREATED_BY = CurrentUser.USER_ID;
                        value.LAST_MODIFIED_DATE = System.DateTime.Now;
                        value.GST_MODE = 1;
                        value.TDS_MODE = 1;
                        value.DUE_CREDIT_BALANCE = 0;
                        value.CREDIT_BALANCE = 0;
                        value.IS_TRAN_START = true;
                        db.TBL_MASTER_MEMBER.Add(value);
                        await db.SaveChangesAsync();
                        string aadhaarfilename = string.Empty;
                        string Pancardfilename = string.Empty;
                        //Checking file is available to save.  
                        if (AadhaarFile != null)
                        {
                            string aadharpath = Path.GetFileName(AadhaarFile.FileName);
                            string AadharfileName = aadharpath.Substring(aadharpath.LastIndexOf(((char)92)) + 1);
                            int index = AadharfileName.LastIndexOf('.');
                            string onyName = AadharfileName.Substring(0, index);
                            string fileExtension = AadharfileName.Substring(index + 1);

                            var AadhaarFileName = value.MEM_ID + "_" + value.AADHAAR_NO + "." + fileExtension;
                            //var AdharServerSavePath = Path.Combine(Server.MapPath("~/MemberFiles/") + AadhaarFileName);
                            var AdharServerSavePath = (Server.MapPath(@"/MemberFiles/") + AadhaarFileName);
                            aadhaarfilename = "/MemberFiles/" + AadhaarFileName;
                            AadhaarFile.SaveAs(AdharServerSavePath);
                        }
                        if (PanFile != null)
                        {
                            string Pannopath = Path.GetFileName(PanFile.FileName);
                            string PannofileName = Pannopath.Substring(Pannopath.LastIndexOf(((char)92)) + 1);
                            int index = PannofileName.LastIndexOf('.');
                            string onyName = PannofileName.Substring(0, index);
                            string PanfileExtension = PannofileName.Substring(index + 1);
                            var InputPanCard = value.MEM_ID + "_" + value.PAN_NO + "." + PanfileExtension;
                            //var PanserverSavePath = Path.Combine(Server.MapPath("~/MemberFiles/") + InputPanCard);
                            var PanserverSavePath = (Server.MapPath(@"/MemberFiles/") + InputPanCard);
                            Pancardfilename = "/MemberFiles/" + InputPanCard;
                            PanFile.SaveAs(PanserverSavePath);
                        }
                        var imageupload = db.TBL_MASTER_MEMBER.Find(value.MEM_ID);
                        imageupload.AADHAAR_FILE_NAME = aadhaarfilename;
                        imageupload.PAN_FILE_NAME = Pancardfilename;
                        db.Entry(imageupload).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();
                        var servlist = await db.TBL_SETTINGS_SERVICES_MASTER.ToListAsync();
                        foreach (var lst in servlist)
                        {
                            TBL_WHITELABLE_SERVICE objser = new TBL_WHITELABLE_SERVICE()
                            {
                                MEMBER_ID = long.Parse(value.MEM_ID.ToString()),
                                SERVICE_ID = long.Parse(lst.SLN.ToString()),
                                ACTIVE_SERVICE = false
                            };
                            db.TBL_WHITELABLE_SERVICE.Add(objser);
                            await db.SaveChangesAsync();
                        }
                        ViewBag.savemsg = "Data Saved Successfully";
                        Session["msg"] = "Data Saved Successfully";
                        //ContextTransaction.Commit();
                    }
                    else
                    {
                        ViewBag.checkstatus = "1";
                        string aadhaarfilename = string.Empty;
                        string Pancardfilename = string.Empty;
                        //Checking file is available to save.  
                        if (AadhaarFile != null)
                        {
                            string Pannopath = Path.GetFileName(PanFile.FileName);
                            string PannofileName = Pannopath.Substring(Pannopath.LastIndexOf(((char)92)) + 1);
                            int index = PannofileName.LastIndexOf('.');
                            string onyName = PannofileName.Substring(0, index);
                            string PanfileExtension = PannofileName.Substring(index + 1);


                            var AadhaarFileName = value.MEM_ID + "_" + value.AADHAAR_NO + "." + PanfileExtension;
                            //var AdharServerSavePath = Path.Combine(Server.MapPath("~/MemberFiles/") + AadhaarFileName);
                            var AdharServerSavePath = (Server.MapPath(@"/MemberFiles/") + AadhaarFileName);
                            aadhaarfilename = "/MemberFiles/" + AadhaarFileName;
                            AadhaarFile.SaveAs(AdharServerSavePath);
                            CheckUser.AADHAAR_FILE_NAME = aadhaarfilename;
                        }
                        if (PanFile != null)
                        {
                            string Pannopath = Path.GetFileName(PanFile.FileName);
                            string PannofileName = Pannopath.Substring(Pannopath.LastIndexOf(((char)92)) + 1);
                            int index = PannofileName.LastIndexOf('.');
                            string onyName = PannofileName.Substring(0, index);
                            string PanfileExtension = PannofileName.Substring(index + 1);

                            var InputPanCard = value.MEM_ID + "_" + value.PAN_NO + "." + PanfileExtension;
                            //var PanserverSavePath = Path.Combine(Server.MapPath("~/MemberFiles/") + InputPanCard);
                            var PanserverSavePath = (Server.MapPath(@"/MemberFiles/") + InputPanCard);
                            Pancardfilename = "/MemberFiles/" + InputPanCard;
                            PanFile.SaveAs(PanserverSavePath);
                            CheckUser.PAN_FILE_NAME = Pancardfilename;
                        }
                        CheckUser.UName = value.UName;
                        CheckUser.UNDER_WHITE_LEVEL = whiteleveluser.UNDER_WHITE_LEVEL;
                        CheckUser.INTRODUCER = MemberCurrentUser.MEM_ID;
                        CheckUser.AADHAAR_NO = value.AADHAAR_NO;
                        CheckUser.PAN_NO = value.PAN_NO;
                        CheckUser.MEMBER_MOBILE = value.MEMBER_MOBILE;
                        //CheckUser.UNDER_WHITE_LEVEL = value.UNDER_WHITE_LEVEL;
                        CheckUser.MEMBER_MOBILE = value.MEMBER_MOBILE;
                        CheckUser.MEMBER_NAME = value.MEMBER_NAME;
                        CheckUser.COMPANY = value.COMPANY;
                        CheckUser.MEMBER_ROLE = value.MEMBER_ROLE;
                        //CheckUser.INTRODUCER = value.INTRODUCER;
                        CheckUser.ADDRESS = value.ADDRESS;
                        CheckUser.CITY = value.CITY;
                        CheckUser.PIN = value.PIN;
                        //CheckUser.EMAIL_ID = value.EMAIL_ID;
                        CheckUser.SECURITY_PIN_MD5 = value.SECURITY_PIN_MD5;
                        CheckUser.BLOCKED_BALANCE = value.BLOCKED_BALANCE;
                        CheckUser.DUE_CREDIT_BALANCE = 0;
                        CheckUser.CREDIT_BALANCE = 0;
                        CheckUser.IS_TRAN_START = true;
                        db.Entry(CheckUser).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();
                        ViewBag.savemsg = "Data Updated Successfully";
                        Session["msg"] = "Data Updated Successfully";
                    }
                    //throw new Exception();
                    ContextTransaction.Commit();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {

                    ContextTransaction.Rollback();
                    Logger.Error("Controller:-  SuperMember(Super), method:- CreateMember (POST) Line No:- 344", ex);
                    return RedirectToAction("Exception", "ErrorHandler", new { area = "" });
                    throw ex;
                    //return View("Error", new HandleErrorInfo(ex, "APILabel", "CreateMember"));               
                }
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //public JsonResult DeleteInformation(int id)
        public async Task<JsonResult> DeleteInformation(string id)
        {
            initpage();////
            var context = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = context.Database.BeginTransaction())
            {
                try
                {

                    string decrptSlId = Decrypt.DecryptMe(id);
                    long Memid = long.Parse(decrptSlId);
                    var membinfo = await context.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == Memid).FirstOrDefaultAsync();
                    membinfo.IS_DELETED = true;
                    context.Entry(membinfo).State = System.Data.Entity.EntityState.Modified;
                    await context.SaveChangesAsync();
                    ContextTransaction.Commit();
                    return Json(new { Result = "true" });
                }
                catch (Exception ex)
                {
                    Logger.Error("Controller:-  SuperMember(Super), method:- DeleteInformation (POST) Line No:- 374", ex);
                    ContextTransaction.Rollback();
                    return Json(new { Result = "false" });
                }
            }


        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> MemberStatusUpdate(string id, string statusval)
        {
            initpage();////
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
                    Logger.Error("Controller:-  SuperMember(Super), method:- MemberStatusUpdate (POST) Line No:- 423", ex);
                    ContextTransaction.Rollback();
                    return Json(new { Result = "false" });
                }
            }


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> PasswordSendtoUser(string id)
        {
            initpage();////
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
                    string mailbody = "Hi " + meminfo.UName + ",<p>Your WHITE LABEL LOGIN USER ID:- " + meminfo.EMAIL_ID + " and  PASSWORD IS:- " + password + "</p>";
                    emailhelper.SendUserEmail(meminfo.EMAIL_ID, "White Label Password", mailbody);
                }
                return Json(new { Result = "true" });
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  SuperMember(Super), method:- PasswordSendtoUser (POST) Line No:- 453", ex);
                return Json(new { Result = "false" });
            }

        }


        [HttpGet]
        public FileResult ExportIndex()
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_MASTER_MEMBER> grid = CreateExportableGrid();
                ExcelWorksheet sheet = package.Workbook.Worksheets["Data"];

                foreach (IGridColumn column in grid.Columns)
                {
                    sheet.Cells[1, col].Value = column.Title;
                    sheet.Column(col++).Width = 18;
                }

                foreach (IGridRow<TBL_MASTER_MEMBER> gridRow in grid.Rows)
                {
                    col = 1;
                    foreach (IGridColumn column in grid.Columns)
                        sheet.Cells[row, col++].Value = column.ValueFor(gridRow);

                    row++;
                }

                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                //return File(fileContents: package.GetAsByteArray(), contentType: "application/unknown");
            }
        }

        private IGrid<TBL_MASTER_MEMBER> CreateExportableGrid()
        {
            long userid = MemberCurrentUser.MEM_ID;
            var dbcontext = new DBContext();
            //var memberinfo = dbcontext.TBL_MASTER_MEMBER.Where(x => x.IS_DELETED == false && x.CREATED_BY == userid).ToList();
            var memberinfo = dbcontext.TBL_MASTER_MEMBER.Where(x => x.CREATED_BY == userid).ToList();
            IGrid<TBL_MASTER_MEMBER> grid = new Grid<TBL_MASTER_MEMBER>(memberinfo);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.EMAIL_ID).Titled("Email").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.MEMBER_NAME).Titled("Name").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.COMPANY).Titled("Company").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.MEMBER_MOBILE).Titled("Mobile").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.BALANCE).Titled("Balance").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.BLOCKED_BALANCE).Titled("Balance").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.MEM_ID).Titled("").Encoded(false).Filterable(false).Sortable(false)
            //     .RenderedAs(model => "<div class='btn-group btn-group-xs' style='width:280px'><a href='javascript:void(0)' class='btn btn-denger' onclick='SendMailToMember(" + model.MEM_ID + ");return 0;'>Password</a><a href='MemberAPILabel/CreateMember?memid=" + Encrypt.EncryptMe(model.MEM_ID.ToString()) + "' class='btn btn-primary'>Edit</a><a href='Hosting/HostingDetails?memid=" + Encrypt.EncryptMe(model.MEM_ID.ToString()) + "' class='btn btn-primary'>Hosting</a><a href='Service/ServiceDetails?memid=" + Encrypt.EncryptMe(model.MEM_ID.ToString()) + "' class='btn btn-primary'>Service</a></div>");
            //grid.Columns.Add(model => model.MEM_ID).Titled("").Encoded(false).Filterable(false).Sortable(false)
            //     .RenderedAs(model => "<div class='btn-group btn-group-xs' style='width:280px'><a href='javascript:void(0)' class='btn btn-denger' onclick='SendMailToMember(" + model.MEM_ID + ");return 0;'>Password</a>&nbsp;<a href='" + @Url.Action("CreateMember", "StockistAPILavel", new { area = "SuperStockist", memid = Encrypt.EncryptMe(model.MEM_ID.ToString()) }) + "' class='btn btn-primary'>Edit</a><a href='" + @Url.Action("HostingDetails", "StockistHosting", new { area = "SuperStockist", memid = Encrypt.EncryptMe(model.MEM_ID.ToString()) }) + "' class='btn btn-primary'>Hosting</a><a href='" + @Url.Action("ServiceDetails", "StockistService", new { area = "SuperStockist", memid = Encrypt.EncryptMe(model.MEM_ID.ToString()) }) + "' class='btn btn-primary'>Service</a></div>");
            grid.Columns.Add(model => model.MEM_ID).Titled("").Encoded(false).Filterable(false).Sortable(false)
                 .RenderedAs(model => "<div class='btn-group btn-group-xs' style='width:280px'><a href='javascript:void(0)' class='btn btn-denger' onclick='SendMailToMember(" + model.MEM_ID + ");return 0;'>Password</a>&nbsp;<a href='" + @Url.Action("CreateMember", "SuperMember", new { area = "Super", memid = Encrypt.EncryptMe(model.MEM_ID.ToString()) }) + "' class='btn btn-primary'>Edit</a><a href='" + @Url.Action("ServiceDetails", "SuperService", new { area = "Super", memid = Encrypt.EncryptMe(model.MEM_ID.ToString()) }) + "' class='btn btn-primary'>Service</a></div>");

            grid.Columns.Add(model => (model.ACTIVE_MEMBER == true ? "Active" : "Deactive")).Titled("Status");
            grid.Columns.Add(model => model.MEM_ID).Titled("").Encoded(false).Filterable(false).Sortable(false)
              .RenderedAs(model => "<a href='javascript:void(0)' class='btn btn-denger btn-xs' onclick='MemberStatus(\"" + model.MEM_ID + "\",\"" + model.ACTIVE_MEMBER + "\");return 0;'>" + (model.ACTIVE_MEMBER == true ? "Deactive" : "Active") + "</a>");

            grid.Pager = new GridPager<TBL_MASTER_MEMBER>(grid);
            grid.Processors.Add(grid.Pager);
            grid.Pager.RowsPerPage = 6;

            //foreach (IGridColumn column in grid.Columns)
            //{
            //    column.Filter.IsEnabled = true;
            //    column.Sort.IsEnabled = true;
            //}

            return grid;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetMemberPassword(string id)
        {
            initpage();////
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
    }
}