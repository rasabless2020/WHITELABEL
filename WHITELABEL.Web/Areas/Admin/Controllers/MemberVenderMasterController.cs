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
using System.Web.Security;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Controllers;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;

namespace WHITELABEL.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class MemberVenderMasterController : AdminBaseController
    {

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
                    //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
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
                //ViewBag.UserName = CurrentUser.UserId;
                Console.WriteLine(e.InnerException);
                return;
            }
        }

        // GET: Admin/MemberVenderMaster
        public ActionResult Index()
        {
            try
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
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public PartialViewResult IndexGrid()
        {
            try
            {
                var dbcontext = new DBContext();
                var memberinfo = dbcontext.TBL_VENDOR_MASTER.ToList();
                return PartialView("IndexGrid", memberinfo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult AddVendorMaster(string memid = "")
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                try
                {
                    var db = new DBContext();
                    long mem_ID = 0;

                    if (memid != "")
                    {
                        string decripttransId = Decrypt.DecryptMe(memid);
                        long.TryParse(decripttransId, out mem_ID);
                        var Modelval = db.TBL_VENDOR_MASTER.FirstOrDefault(x => x.ID == mem_ID);
                        ViewBag.btnStatus = "1";
                        return View(Modelval);
                    }
                    else
                    {
                        ViewBag.btnStatus = "0";
                        return View();
                    }
                }
                catch (Exception ex)
                {

                    throw;
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
        public async Task<JsonResult> PostAddVenderMaster(TBL_VENDOR_MASTER objVend)
        {
            initpage();
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var Vender_Info = db.TBL_VENDOR_MASTER.FirstOrDefault(x => x.ID == objVend.ID);
                    if (Vender_Info != null)
                    {
                        Vender_Info.VENDOR_NAME = objVend.VENDOR_NAME;
                        Vender_Info.VENDOR_TYPE = objVend.VENDOR_TYPE;
                        Vender_Info.STATUS = objVend.STATUS;
                        Vender_Info.CREATED_DATE = DateTime.Now;
                        db.Entry(Vender_Info).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        ContextTransaction.Commit();
                        return Json("Vender Information Updated", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        objVend.CREATED_DATE = DateTime.Now;
                        db.TBL_VENDOR_MASTER.Add(objVend);
                        db.SaveChanges();
                        ContextTransaction.Commit();
                        return Json("Vendor Information Add", JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    throw ex;
                }
            }
        }
    }
}