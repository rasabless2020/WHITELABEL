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
using System.Threading.Tasks;
using static WHITELABEL.Web.Helper.InstantPayApi;
using log4net;
using System.Data.Entity;

namespace WHITELABEL.Web.Areas.Merchant.Controllers
{
    [Authorize]
    public class MerchantDMRLoginController : MerchantBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        // GET: Merchant/MerchantDMRLogin
        public async Task<ActionResult> Index(string returnUrl)
        {
            try
            {
                if (Session["MerchantUserId"] != null)
                {
                    SystemClass sclass = new SystemClass();
                    //string userID = sclass.GetLoggedUser();
                    if (Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl))
                    {
                        ViewBag.ReturnURL = returnUrl;
                    }
                    if (Session["MerchantDMRId"] != null)
                    {
                        //Response.RedirectToRoute("Dashboard", "Index");
                        string username = Session["MerchantDMRId"].ToString();
                        var db = new DBContext();
                        var userinfo = await db.TBL_MASTER_MEMBER.Where(x => x.UName == username && x.MEMBER_ROLE == 5).FirstOrDefaultAsync();
                        Response.Redirect(Url.Action("Index", "MerchantDashboard", new { area = "Merchant" }));
                    }

                    DMRLoginViewModel model = new DMRLoginViewModel();
                    if (Request.Cookies["Login"] != null)
                    {
                        //model.Email = Request.Cookies["Login"].Values["EmailID"];
                        //model.Password = Request.Cookies["Login"].Values["Password"];
                    }
                    return View(model);
                }
                else
                {
                    return RedirectToAction("Index", "MerchantLogin", new { area = "Merchant" });
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MerchantDMRLogin(Merchant), method:- Index(Get) Line No:- 60", ex);
                throw;
            }

           

            
        }

        [HttpPost]
        public async Task<ActionResult> Index(DMRLoginViewModel User, string ReturnURL = "")
        {

            try
            {
                const string agentId = "2";
                //string OperatorName = Request.Form["OperatorName"];
                //string operatorId = Request.Form["OperatorId"];
                var db = new DBContext();
                var checkloginStatus = await db.TBL_DMR_REMITTER_INFORMATION.Where(x => x.MobileNo == User.MobileNo).FirstOrDefaultAsync();
                if (checkloginStatus.UpdateStatus == 0)
                {
                    var PaymentValidation = MoneyTransferAPI.RemitterDetails(User.MobileNo,"");
                    if (PaymentValidation.statuscode == "TXN")
                    {
                    checkloginStatus.Address = PaymentValidation.data.remitter.address;
                    checkloginStatus.City = PaymentValidation.data.remitter.city;
                    checkloginStatus.State = PaymentValidation.data.remitter.state;
                    checkloginStatus.State = PaymentValidation.data.remitter.state;
                    checkloginStatus.KYCStatus = PaymentValidation.data.remitter.kycstatus;
                    checkloginStatus.ConsumedLimited = Convert.ToDecimal(PaymentValidation.data.remitter.consumedlimit);
                    checkloginStatus.RemainingLimit = Convert.ToDecimal(PaymentValidation.data.remitter.remaininglimit);
                    var limit = PaymentValidation.data.remitter_limit[0];
                    var limitTotal = limit.limit.total;
                    checkloginStatus.Total = Convert.ToDecimal(limitTotal);
                    checkloginStatus.KYCDocs = PaymentValidation.data.remitter.kycdocs;
                    checkloginStatus.Perm_txn_limit = Convert.ToDecimal(PaymentValidation.data.remitter.perm_txn_limit);
                    checkloginStatus.UpdateStatus = 1;
                    db.Entry(checkloginStatus).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();
                    var beneficiarylist = PaymentValidation.data.beneficiary;
                    foreach (var listitem in beneficiarylist)
                    {
                        string beneid = listitem.id.Value;
                        var benelist = await db.TBL_REMITTER_BENEFICIARY_INFO.Where(x => x.BeneficiaryID == beneid).FirstOrDefaultAsync();
                        benelist.Bank = listitem.bank.Value;
                        db.Entry(benelist).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();

                    }
                        var ipat_id = PaymentValidation.data.remitter.id;

                        Session["MerchantDMRId"] = ipat_id.Value;
                        return RedirectToAction("DMRInformation", "MerchantDMRDashboard", new { area = "Merchant" });
                    }
                    else
                    {
                        //return Json(PaymentValidation);
                        ViewBag.Message = "Invalid Credential or Access Denied";
                        return View();
                    }
                }
                else
                {
                    Session["MerchantDMRId"] = checkloginStatus.RemitterID;
                    return RedirectToAction("DMRInformation", "MerchantDMRDashboard", new { area = "Merchant" });
                }

            }
            catch (Exception ex)
            {
                //throw ex;
                ViewBag.Message = "Invalid Credential or Access Denied";
                Logger.Error("Controller:-  MerchantDMRLogin(Merchant), method:- Index(POST) Line No:- 133", ex);
                return View();
            }
            return View();
           
        }
        [AllowAnonymous]
        public ActionResult DMRLogOut()
        {
            if (Session["MerchantDMRId"] != null)
            {
                Session["MerchantDMRId"] = null;               
                //Session.Clear();
                Session.Remove("MerchantDMRId");

                return RedirectToAction("Index", "MerchantDMRLogin", new { area = "Merchant" });
            }
            else
            {
                return RedirectToAction("Index", "MerchantDMRLogin", new { area = "Merchant" });
            }
        }

    }
}