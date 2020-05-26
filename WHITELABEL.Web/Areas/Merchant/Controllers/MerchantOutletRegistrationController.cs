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


namespace WHITELABEL.Web.Areas.Merchant.Controllers
{
    [Authorize]
    public class MerchantOutletRegistrationController : MerchantBaseController
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

        // GET: Merchant/MerchantOutletRegistration
        public ActionResult Index()
        {
            if (Session["MerchantUserId"] != null)
            {
                var db = new DBContext();
                initpage();
                var retailerlist = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).FirstOrDefault();
                MerchantOutletModelview objval = new MerchantOutletModelview()
                {
                    //Name=retailerlist.MEMBER_NAME,
                    Email=retailerlist.EMAIL_ID,
                    Address=retailerlist.ADDRESS,
                    Pincode=retailerlist.PIN,
                    Company=retailerlist.COMPANY
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
        public async Task<JsonResult> OutletOTPSend(MerchantOutletModelview objval)
        {
            initpage();
            var PaymentValidation = OutletApi.VerifyOutlet(objval.Mobile);
            if (PaymentValidation.statuscode == "TXN")
            {
                //string statuscode = PaymentValidation;
                //string response = JObject.Parse(PaymentValidation);
                //return Json( new { statuscode = PaymentValidation.statuscode.Value, mobileno= PaymentValidation.data.mobile_number.Value });
                var data = JsonConvert.SerializeObject(PaymentValidation);
                return Json(data);
            }
            else
            {
                //dynamic response = JObject.Parse(PaymentValidation);
                //string statuscode = PaymentValidation.status;
                var data = JsonConvert.SerializeObject(PaymentValidation);
                return Json(data);
            }
            
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> OutletRegistration(MerchantOutletModelview objval)
        {
            initpage();
            try
            {
                var db = new DBContext();
                //string mobileno = Request.Form["Reg_Mobile"].Remove(',');
                var PaymentValidation = OutletApi.RegisterOutlet(objval.Reg_Mobile, objval.OTP, objval.Email, objval.Company, objval.Name, objval.Address, objval.Pincode, objval.PanNo);
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
                            MOBILE = objval.Reg_Mobile,
                            EMAIL = objval.Email,
                            OUTLETNAME = PaymentValidation.data.outlet_name.Value,
                            CONTACTPERSON = PaymentValidation.data.contact_person.Value,
                            //AADHAARNO = PaymentValidation.data.aadhaar_no.Value,
                            PANCARDNO = PaymentValidation.data.pan_no.Value,
                            KYC_STATUS = int.Parse(PaymentValidation.data.kyc_status.Value),
                            OUTLET_STATUS = int.Parse(PaymentValidation.data.outlet_status.Value),
                            INSERTED_DATE = System.DateTime.Now,
                            INSERTED_BY = CurrentMerchant.MEM_ID,
                            PINCODE = objval.Pincode

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
                throw ex;
            }

            


        }
    }
}