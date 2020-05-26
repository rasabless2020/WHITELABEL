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
using System.Web.UI;
using System.Text.RegularExpressions;


namespace WHITELABEL.Web.Areas.Merchant.Controllers
{
    public class MerchantOutletController : MerchantBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        // GET: Merchant/MerchantOutlet
        public void initpage()
        {
            try
            {
                ViewBag.ControllerName = "Merchant";

                if (Session["MerchantUserId"] == null)
                {
                    //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
                    Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
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
                //ViewBag.UserName = CurrentUser.UserId;
                Console.WriteLine(e.InnerException);
                return;
            }
        }
        public ActionResult Index()
        {
            initpage();
            if (Session["MerchantUserId"] != null)
            {
                var db = new DBContext();
                
                var retailerlist = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).FirstOrDefault();
                MerchantOutletModelview objval = new MerchantOutletModelview()
                {
                    Name=retailerlist.MEMBER_NAME,
                    Email = retailerlist.EMAIL_ID,
                    Address = retailerlist.ADDRESS,
                    Pincode = retailerlist.PIN,
                    Company = retailerlist.COMPANY
                };
                //ViewBag.ActiveServiceList = checkList;
                Session["MerchantDMRId"] = null;
                Session.Remove("MerchantDMRId");
                return View(objval);
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
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> PostOutletInformation(MerchantOutletModelview Objval)
        {
            try
            {
                var db = new DBContext();
                //string mobileno = Request.Form["Reg_Mobile"].Remove(',');
                var PaymentValidation = OutletApi.RegisterOutlet(Objval.Reg_Mobile, Objval.OTP, Objval.Email, Objval.Company, Objval.Name, Objval.Address, Objval.Pincode, Objval.PanNo);
                if (PaymentValidation.statuscode == "TXN")
                {
                    string statuscode = PaymentValidation.statuscode;
                    string outletid = PaymentValidation.data.outlet_id;
                    var checkoutletid = db.TBL_MERCHANT_OUTLET_INFORMATION.Where(x => x.OUTLETID == outletid).FirstOrDefault();
                    if (checkoutletid == null)
                    {
                        TBL_MERCHANT_OUTLET_INFORMATION objmsg = new TBL_MERCHANT_OUTLET_INFORMATION()
                        {
                            MEM_ID = CurrentMerchant.MEM_ID,
                            OUTLETID = PaymentValidation.data.outlet_id.Value,
                            MOBILE = Objval.Reg_Mobile,
                            EMAIL = Objval.Email,
                            OUTLETNAME = PaymentValidation.data.outlet_name.Value,
                            CONTACTPERSON = PaymentValidation.data.contact_person.Value,
                            //AADHAARNO = PaymentValidation.data.aadhaar_no.Value,
                            PANCARDNO = PaymentValidation.data.pan_no.Value,
                            KYC_STATUS = int.Parse(PaymentValidation.data.kyc_status.Value),
                            OUTLET_STATUS = int.Parse(PaymentValidation.data.outlet_status.Value),
                            INSERTED_DATE = System.DateTime.Now,
                            INSERTED_BY = CurrentMerchant.MEM_ID,
                            PINCODE = Objval.Pincode

                        };
                        db.TBL_MERCHANT_OUTLET_INFORMATION.Add(objmsg);
                        await db.SaveChangesAsync();
                    }
                    return Json(statuscode);
                }
                else
                {
                    string statuscode = PaymentValidation.status;
                    return Json(statuscode);
                }
            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return Json(msg);
                throw;
            }
        
            
        }

        [HttpPost]
        public JsonResult GenerateOutletOTP(string Custmerid)
        {
            initpage();
            try
            {
                initpage();
                var PaymentValidation = OutletApi.VerifyOutlet(Custmerid);
                if (PaymentValidation.statuscode == "TXN")
                {                  
                    var data = JsonConvert.SerializeObject(PaymentValidation);
                    string msgdata = data.status;
                    return Json(msgdata,JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var data = JsonConvert.SerializeObject(PaymentValidation);
                    var msgdata = data.status;
                    return Json(msgdata, JsonRequestBehavior.AllowGet);
                    //return Json(data);
                }
                //var db = new DBContext();
                //var OTPCall = TransXT_DMR_API.GenerateOTP(Custmerid, "1", "");
                //var errmsg = OTPCall.errorMsg.Value;
                //if (errmsg == "SUCCESS")
                //{
                //    return Json("OTP Send to your Mobile no.", JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //    return Json(errmsg, JsonRequestBehavior.AllowGet);
                //}

            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return Json(msg);
                Logger.Error("Controller:-  PostOutletInformation(Merchant Outlet), method:- GenerateOutletOTP(POST) Line No:- 118", ex);
                throw ex;
            }

        }

    }
}