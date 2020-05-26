using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Models;
using WHITELABEL.Web.Helper;
using System.Data.Entity.Core;
using WHITELABEL.Web.Areas.Merchant.Models;
using WHITELABEL.Web.ServiceApi.RECHARGE.PORTIQUE;
using static WHITELABEL.Web.Helper.InstantPayApi;
using NonFactors.Mvc.Grid;
using OfficeOpenXml;
using System.Threading.Tasks;
using System.Data.Entity;
using log4net;
using System.Web.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Web.Script.Serialization;
using System.Device.Location;
using WHITELABEL.Web.Controllers;

namespace WHITELABEL.Web.Areas.Merchant.Controllers
{
    [Authorize]
    public class MerchantFingerprintdeviceController : MerchantBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        //public void initpage()
        //{
        //    try
        //    {
        //        ViewBag.ControllerName = "Merchant Dashboard";
        //        SystemClass sclass = new SystemClass();
        //        string userID = sclass.GetLoggedUser();
        //        long userid = long.Parse(userID);
        //        var dbmain = new DBContext();
        //        if (userID != null && userID != "")
        //        {
        //            TBL_MASTER_MEMBER currUser = dbmain.TBL_MASTER_MEMBER.SingleOrDefault(c => c.MEM_ID == userid && c.MEMBER_ROLE == 5 && c.ACTIVE_MEMBER == true);
        //            if (currUser != null)
        //            {
        //                Session["MerchantUserId"] = currUser.MEM_ID;
        //                // Session["UserName"] = currUser.UserName;
        //            }
        //            else
        //            {
        //                Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //                return;
        //            }
        //        }
        //        if (Session["MerchantUserId"] == null)
        //        {
        //            //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //            Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //            return;
        //        }
        //        bool Islogin = false;
        //        if (Session["MerchantUserId"] != null)
        //        {
        //            Islogin = true;
        //            ViewBag.CurrentUserId = CurrentMerchant.MEM_ID;
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
                ViewBag.ControllerName = "Merchant";
                if (Session["MerchantUserId"] == null)
                {
                    Response.Redirect(Url.Action("Logout", "Login", new { area = "" }));
                    return;
                }
                bool Islogin = false;

                if (Session["MerchantUserId"] != null)
                {
                    Islogin = true;
                    ViewBag.CurrentUserId = CurrentMerchant.MEM_ID;
                }
                ViewBag.Islogin = Islogin;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException);
                return;
            }
        }
        // GET: Merchant/MerchantFingerprintdevice
        public ActionResult Index()
        {
            if (Session["MerchantUserId"] != null)
            {
                initpage();
                return View();
            }
            else
            {
                FormsAuthentication.SignOut();
                Session["MerchantUserId"] = null;
                Session["MerchantUserName"] = null;
                Session.Clear();
                Session.Remove("MerchantUserId");
                Session.Remove("MerchantUserName");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        public PartialViewResult IndexGrid()
        {
            try
            {
                var db = new DBContext();
                var fingerPrinterlist = db.TBL_FINGERPRINT_DEVICE_MASTER.ToList();
                return PartialView("IndexGrid", fingerPrinterlist);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        //public JsonResult AddFingerPrintDevice(string Devicename, string DeviceModel, string DeviceCode, string Status)
        public JsonResult AddFingerPrintDevice(TBL_FINGERPRINT_DEVICE_MASTER objfng)
        {
            initpage();
            try
            {
                var db = new DBContext();
                var deviceInfo = db.TBL_FINGERPRINT_DEVICE_MASTER.FirstOrDefault(x => x.ID == objfng.ID);
                if (deviceInfo != null)
                {
                    deviceInfo.DEVICE_NAME = objfng.DEVICE_NAME;
                    deviceInfo.DEVICE_MODELNO= objfng.DEVICE_MODELNO;
                    deviceInfo.DEVICE_CODE= objfng.DEVICE_CODE;
                    deviceInfo.STATUS= objfng.STATUS;
                    db.Entry(deviceInfo).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json("Device Information Saved.", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    TBL_FINGERPRINT_DEVICE_MASTER objtax = new TBL_FINGERPRINT_DEVICE_MASTER()
                    {
                        DEVICE_NAME = objfng.DEVICE_NAME,
                        DEVICE_MODELNO = objfng.DEVICE_MODELNO,
                        DEVICE_CODE = objfng.DEVICE_CODE,
                        STATUS = objfng.STATUS,
                        CREATED_DATE = DateTime.Now,
                        MEM_ID = CurrentMerchant.MEM_ID
                    };
                    db.TBL_FINGERPRINT_DEVICE_MASTER.Add(objtax);
                    db.SaveChanges();
                    return Json("Device Information Saved.", JsonRequestBehavior.AllowGet);
                }                
            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return Json(msg);
                Logger.Error("Controller:-  MerchantFingerprintdeviceController(Merchant), method:- AddFingerPrintDevice(POST)", ex);
                throw ex;
            }
        }
        [HttpPost]
        public JsonResult GetFingerPrintInfo(string TransId)
        {
            initpage();
            try
            {
                long valueid = long.Parse(TransId);
                var db = new DBContext();
                var BankMarginvaluebind = db.TBL_FINGERPRINT_DEVICE_MASTER.Where(x => x.ID == valueid).FirstOrDefault();
                return Json(BankMarginvaluebind, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}