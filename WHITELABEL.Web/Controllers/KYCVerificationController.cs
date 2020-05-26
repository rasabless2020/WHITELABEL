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
    public class KYCVerificationController : BaseController
    {
        public void initpage()
        {
            try
            {
                ViewBag.ControllerName = "KYC Verification";
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
        // GET: KYCVerification
        public ActionResult Index()
        {
            initpage();
            return View();
        }
        public PartialViewResult IndexGrid()
        {
            try
            {
                var db = new DBContext();
                var documentlist =db.TBL_MASTER_MEMBER.Where(x=>x.KYC_VERIFIED ==  null && (x.AADHAAR_NO!=null || x.PAN_NO!=null )).ToList();
                return PartialView(documentlist);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
        public FileResult downloadfiles(string type, string memid)
        {
            var db = new DBContext();
            string decrptMemId = Decrypt.DecryptMe(memid);
            long fid = long.Parse(decrptMemId);
            var filename= new TBL_MASTER_MEMBER();
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
                fileNameinfo = fileNameinfo + "."+ fileExtension;
                contentType = "Images/png";
            }
            else if (fileExtension == "jpg" || fileExtension == "JPG" || fileExtension == "jpeg")
            {
                fileNameinfo = fileNameinfo + "."+ fileExtension;
                contentType = "Images/jpg";
            }
            else if (fileExtension == "pdf" || fileExtension == "pdf")
            {
                fileNameinfo = fileNameinfo + "."+ fileExtension;
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

        [HttpPost]
        public JsonResult ApproveKYCDocument(string Id)
        {
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    //string decrptMemId = Decrypt.DecryptMe(Id);
                    long MemID = long.Parse(Id);
                    var getdocinfo = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == MemID).FirstOrDefault();
                    if (getdocinfo != null)
                    {
                        getdocinfo.KYC_VERIFIED = true;
                        getdocinfo.KYC_VERIFIED_USER = long.Parse(Session["UserId"].ToString());
                        getdocinfo.VERIFICATION_DATE = System.DateTime.Now;
                        db.Entry(getdocinfo).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
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
                    ContextTransaction.Rollback();
                    throw ex;
                }
            }
                
                
            
        }
        [HttpPost]
        public JsonResult RejectKYCDocument(string Id)
        {
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                long MemID = long.Parse(Id);
                var getdocinfo = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == MemID).FirstOrDefault();
                if (getdocinfo != null)
                {
                    getdocinfo.KYC_VERIFIED = false;
                    getdocinfo.KYC_VERIFIED_USER = long.Parse(Session["UserId"].ToString());
                    getdocinfo.VERIFICATION_DATE = System.DateTime.Now;
                    db.Entry(getdocinfo).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
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
    }
}