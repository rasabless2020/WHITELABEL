using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Models;

namespace WHITELABEL.Web.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        public void initpage()
        {
            try
            {
                ViewBag.ControllerName = "White Level Details";
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

        public ActionResult Index()
        {
            try
            {
                initpage();
                var db = new DBContext();
                var Memblistdb = db.TBL_MASTER_MEMBER.Where(x => x.UNDER_WHITE_LEVEL == 0).Take(7).ToList().OrderByDescending(x=>x.JOINING_DATE);
                ViewBag.whitelevel = Memblistdb;
                var requisitionlist = (from acnt in db.TBL_BALANCE_TRANSFER_LOGS join mem in db.TBL_MASTER_MEMBER on acnt.FROM_MEMBER equals mem.MEM_ID
                                       where acnt.TO_MEMBER==0
                                       select new
                                       {
                                           tranid=acnt.SLN,
                                           transactionid=acnt.TransactionID,
                                           tranDate=acnt.REQUEST_DATE,
                                           userName=mem.UName
                                       }).AsEnumerable().Select(z=>new GetRequisitiondetails()
                                       {
                                           TransId= z.tranid.ToString(),
                                           TransactionID=z.transactionid,
                                           TransDate=z.tranDate.ToString("yyyy-MM-dd"),
                                           TransUserName=z.userName
                                       }).ToList().Take(6).OrderByDescending(z=>z.TransDate);
                ViewBag.RequisitionList = requisitionlist;
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}