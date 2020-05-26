using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Models;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.ServiceApi.RECHARGE.PORTIQUE;

namespace WHITELABEL.Web.Areas.Merchant.Controllers
{
    public class MerchantCallBackURLController : MerchantBaseController
    {
        public void initpage()
        {
            try
            {
                ViewBag.ControllerName = "Merchant Dashboard";
                
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

        // GET: Merchant/MerchantCallBackURL
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CallbackURL(string accountId,string txid,string transtype)
        {
            return View();
        }
    }
}