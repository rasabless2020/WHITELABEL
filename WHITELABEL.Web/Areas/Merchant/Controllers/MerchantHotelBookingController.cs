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

namespace WHITELABEL.Web.Areas.Merchant.Controllers
{
    [Authorize]
    public class MerchantHotelBookingController : MerchantBaseController
    {
        // GET: Merchant/MerchantHotelBooking

        public void initpage()
        {
            try
            {
                ViewBag.ControllerName = "Hotel Dashboard";
                SystemClass sclass = new SystemClass();
                string userID = sclass.GetLoggedUser();
                long userid = long.Parse(userID);
                var dbmain = new DBContext();
                if (userID != null && userID != "")
                {
                    TBL_MASTER_MEMBER currUser = dbmain.TBL_MASTER_MEMBER.SingleOrDefault(c => c.MEM_ID == userid && c.MEMBER_ROLE == 5 && c.ACTIVE_MEMBER == true);
                    if (currUser != null)
                    {
                        Session["MerchantUserId"] = currUser.MEM_ID;
                        // Session["UserName"] = currUser.UserName;
                    }
                    else
                    {
                        Response.Redirect(Url.Action("Index", "MerchantLogin", new { area = "Merchant" }));
                        return;
                    }
                }
                if (Session["MerchantUserId"] == null)
                {
                    //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
                    Response.Redirect(Url.Action("Index", "MerchantLogin", new { area = "Merchant" }));
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
            if (Session["MerchantUserId"] != null)
            {
                initpage();
                Session["MerchantDMRId"] = null;
                Session.Remove("MerchantDMRId");
                return View();
            }
            else
            {
                return RedirectToAction("Index", "MerchantLogin", new { area = "Merchant" });
            }
        }
    }
}