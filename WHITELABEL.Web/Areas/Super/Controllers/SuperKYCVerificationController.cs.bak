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
    public class SuperKYCVerificationController : SuperBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        public void initpage()
        {
            try
            {
                ViewBag.ControllerName = "Super KYC Verificatin";
                SystemClass sclass = new SystemClass();
                string userID = sclass.GetLoggedUser();
                long userid = long.Parse(userID);
                var dbmain = new DBContext();
                if (userID != null && userID != "")
                {
                    TBL_MASTER_MEMBER currUser = dbmain.TBL_MASTER_MEMBER.SingleOrDefault(c => c.MEM_ID == userid && c.ACTIVE_MEMBER == true && c.MEMBER_ROLE==3);
                    if (currUser != null)
                    {
                        Session["SuperDistributorId"] = currUser.MEM_ID;
                        // Session["UserName"] = currUser.UserName;
                    }
                    else
                    {
                        Response.Redirect(Url.Action("Index", "SuperLogin", new { area = "Super" }));
                        return;
                    }
                }
                if (Session["SuperDistributorId"] == null)
                {
                    //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
                    //Response.Redirect(Url.Action("Index", "StockistDashboard", new { area = "SuperStockist" }));
                    Response.Redirect(Url.Action("Index", "SuperLogin", new { area = "Super" }));
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
                //ViewBag.UserName = CurrentUser.UserId;
                Console.WriteLine(e.InnerException);
                return;
            }
        }
        // GET: Super/SuperKYCVerification
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
                var db = new DBContext();
                var documentlist = db.TBL_MASTER_MEMBER.Where(x => x.KYC_VERIFIED == null && (x.AADHAAR_NO != null || x.PAN_NO != null) && x.CREATED_BY == MemberCurrentUser.MEM_ID).ToList();
                return PartialView(documentlist);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public FileResult downloadfiles(string type, string memid)
        {
            try
            {
                var db = new DBContext();
                string decrptMemId = Decrypt.DecryptMe(memid);
                long fid = long.Parse(decrptMemId);
                var filename = new TBL_MASTER_MEMBER();
                string filepath = string.Empty;
                string fileNameinfo = string.Empty;
                if (type == "Pan")
                {
                    filename = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == fid).FirstOrDefault();
                    filepath = filename.PAN_FILE_NAME.ToString();
                    fileNameinfo = "PanCard";
                }
                else
                {
                    filename = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == fid).FirstOrDefault();
                    filepath = filename.AADHAAR_FILE_NAME.ToString();
                    fileNameinfo = "AadhaarCard";
                }

                //string contentType = "application/pdf";
                string contentType = string.Empty;

                string path = filepath;
                string fileName = path.Substring(path.LastIndexOf(((char)92)) + 1);
                int index = fileName.LastIndexOf('.');
                string onyName = fileName.Substring(0, index);
                string fileExtension = fileName.Substring(index + 1);
                if (fileExtension == "png" || fileExtension == "PNG")
                {
                    fileNameinfo = fileNameinfo + "." + fileExtension;
                    contentType = "Images/png";
                }
                else if (fileExtension == "jpg" || fileExtension == "JPG" || fileExtension == "jpeg")
                {
                    fileNameinfo = fileNameinfo + "." + fileExtension;
                    contentType = "Images/jpg";
                }
                else if (fileExtension == "pdf" || fileExtension == "pdf")
                {
                    fileNameinfo = fileNameinfo + "." + fileExtension;
                    contentType = "application/pdf";
                }
                //Parameters to file are
                //1. The File Path on the File Server
                //2. The content type MIME type
                //3. The parameter for the file save by the browser
                return File(filepath, contentType, fileNameinfo);
                //string filename = (from f in files
                //                   where f.FileId == fid
                //                   select f.FilePath).First();
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  SuperKYCVerification(Admin), method:- downloadfiles (POST) Line No:- 159", ex);
                
                throw ex;
            }

            

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ApproveKYCDocument(string Id)
        {
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    //string decrptMemId = Decrypt.DecryptMe(Id);
                    long MemID = long.Parse(Id);
                    var getdocinfo = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == MemID).FirstOrDefaultAsync();
                    if (getdocinfo != null)
                    {
                        getdocinfo.KYC_VERIFIED = true;
                        getdocinfo.KYC_VERIFIED_USER = long.Parse(Session["UserId"].ToString());
                        getdocinfo.VERIFICATION_DATE = System.DateTime.Now;
                        db.Entry(getdocinfo).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();
                        ContextTransaction.Commit();
                        return Json(new { Result = "true" });
                    }
                    else
                    {
                        return Json(new { Result = "false" });
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("Controller:-  SuperKYCVerification(Admin), method:- ApproveKYCDocument (POST) Line No:- 197", ex);
                    ContextTransaction.Rollback();
                    throw ex;
                }
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> RejectKYCDocument(string Id)
        {
            try
            {
                var db = new DBContext();
                using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
                {
                    long MemID = long.Parse(Id);
                    var getdocinfo = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == MemID).FirstOrDefaultAsync();
                    if (getdocinfo != null)
                    {
                        getdocinfo.KYC_VERIFIED = false;
                        getdocinfo.KYC_VERIFIED_USER = long.Parse(Session["UserId"].ToString());
                        getdocinfo.VERIFICATION_DATE = System.DateTime.Now;
                        db.Entry(getdocinfo).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();
                        ContextTransaction.Commit();
                        return Json(new { Result = "true" });
                    }
                    else
                    {
                        ContextTransaction.Rollback();
                        return Json(new { Result = "false" });
                    }
                }
                //string decrptMemId = Decrypt.DecryptMe(Id);
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  SuperKYCVerification(Admin), method:- RejectKYCDocument (POST) Line No:- 238", ex);
                throw;
            }
        }
    }
}