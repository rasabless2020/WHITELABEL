using log4net;
using NonFactors.Mvc.Grid;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Areas.Merchant.Models;
using WHITELABEL.Web.Controllers;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;


namespace WHITELABEL.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class MemberCommDisburseSettingController : AdminBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        //public void initpage()
        //{
        //    try
        //    {
        //        ViewBag.ControllerName = "Bank List";
        //        SystemClass sclass = new SystemClass();
        //        string userID = sclass.GetLoggedUser();
        //        long userid = long.Parse(userID);
        //        var dbmain = new DBContext();
        //        if (userID != null && userID != "")
        //        {
        //            TBL_MASTER_MEMBER currUser = dbmain.TBL_MASTER_MEMBER.SingleOrDefault(c => c.MEM_ID == userid && c.ACTIVE_MEMBER == true && c.MEMBER_ROLE == 1);
        //            if (currUser != null)
        //            {
        //                Session["WhiteLevelUserId"] = currUser.MEM_ID;
        //                // Session["UserName"] = currUser.UserName;
        //            }
        //            else
        //            {
        //                Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //                return;
        //            }
        //        }
        //        if (Session["WhiteLevelUserId"] == null)
        //        {
        //            //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //            Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //            return;
        //        }
        //        bool Islogin = false;
        //        if (Session["WhiteLevelUserId"] != null)
        //        {
        //            Islogin = true;
        //            ViewBag.CurrentUserId = MemberCurrentUser.MEM_ID;
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
                ViewBag.ControllerName = "White Label";
                if (Session["WhiteLevelUserId"] == null)
                {
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
                Console.WriteLine(e.InnerException);
                return;
            }
        }

        // GET: Admin/MemberCommDisburseSetting
        public ActionResult Index()
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
        public PartialViewResult IndexGrid()
        {
            try
            {
                var db = new DBContext();
                //var RechargeTransaction = db.TBL_INSTANTPAY_RECHARGE_RESPONSE.Where(x => x.Mem_ID == CurrentMerchant.MEM_ID).ToList();
                var GetRechargeMerchantList = (from a in db.TBL_MASTER_MEMBER
                                               join b in db.TBL_MASTER_MEMBER on a.MEM_ID equals b.INTRODUCER
                                               join c in db.TBL_MASTER_MEMBER on b.MEM_ID equals c.INTRODUCER
                                               join d in db.TBL_MASTER_MEMBER on c.MEM_ID equals d.INTRODUCER
                                               join respinsta in db.TBL_INSTANTPAY_RECHARGE_RESPONSE on d.MEM_ID equals respinsta.Mem_ID
                                               where d.UNDER_WHITE_LEVEL == MemberCurrentUser.MEM_ID && respinsta.ERROR_TYPE== "Pending"

                                               select new
                                               {
                                                   ID = respinsta.ID,
                                                   Ipay_Id = respinsta.Ipay_Id,
                                                   AgentId = respinsta.AgentId,
                                                   Opr_Id = respinsta.Opr_Id,
                                                   AccountNo = respinsta.AccountNo,
                                                   Sp_Key = respinsta.Sp_Key,
                                                   Trans_Amt = respinsta.Trans_Amt,
                                                   Charged_Amt = respinsta.Charged_Amt,
                                                   Opening_Balance = respinsta.Opening_Balance,
                                                   DateVal = respinsta.DateVal,
                                                   Status = respinsta.Status,
                                                   Res_Code = respinsta.Res_Code,
                                                   res_msg = respinsta.res_msg,
                                                   Mem_ID = respinsta.Mem_ID,
                                                   RechargeType = respinsta.RechargeType,
                                                   IpAddress = respinsta.IpAddress,
                                                   CORELATIONID = respinsta.CORELATIONID,
                                                   ERROR_TYPE = respinsta.ERROR_TYPE,
                                                   ISREVERSE = respinsta.ISREVERSE,
                                                   ISCOMMISSIONDISBURSE = respinsta.ISCOMMISSIONDISBURSE,
                                                   COMMISSIONDISBURSEDATE = respinsta.COMMISSIONDISBURSEDATE
                                               }).AsEnumerable().Select(z => new TBL_INSTANTPAY_RECHARGE_RESPONSE
                                               {
                                                   ID=z.ID,
                                                   Ipay_Id = z.Ipay_Id,
                                                   AgentId = z.AgentId,
                                                   Opr_Id = z.Opr_Id,
                                                   AccountNo = z.AccountNo,
                                                   Sp_Key = z.Sp_Key,
                                                   Trans_Amt = z.Trans_Amt,
                                                   Charged_Amt = z.Charged_Amt,
                                                   Opening_Balance = z.Opening_Balance,
                                                   DateVal = z.DateVal,
                                                   Status = z.Status,
                                                   Res_Code = z.Res_Code,
                                                   res_msg = z.res_msg,
                                                   Mem_ID = z.Mem_ID,
                                                   RechargeType = z.RechargeType,
                                                   IpAddress = z.IpAddress,
                                                   CORELATIONID = z.CORELATIONID,
                                                   ERROR_TYPE = z.ERROR_TYPE,
                                                   ISREVERSE = z.ISREVERSE,
                                                   ISCOMMISSIONDISBURSE = z.ISCOMMISSIONDISBURSE,
                                                   COMMISSIONDISBURSEDATE = z.COMMISSIONDISBURSEDATE
                                               }).ToList();

                return PartialView("IndexGrid", GetRechargeMerchantList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<JsonResult> DistributeCommission(string slnval = "")
        {
            initpage();////
            try
            {
                var db = new DBContext();
                if (slnval != null)
                {
                    long ID_Val = 0;
                    long.TryParse(slnval, out ID_Val);
                    string CommType = string.Empty;
                    var membercomm = db.TBL_INSTANTPAY_RECHARGE_RESPONSE.FirstOrDefault(x => x.ID == ID_Val);
                    if (membercomm.RechargeType == "Prepaid-Mobile" || membercomm.RechargeType == "Postpaid-Mobile")
                    {
                        CommType = "MOBILE";
                    }
                    else if (membercomm.RechargeType == "DMR")
                    {
                        CommType = "DMR";
                    }
                    else
                    {
                        CommType = "UTILITY";
                    }
                        CommissionDistributionHelper objComm = new CommissionDistributionHelper();
                    bool checkComm =await objComm.AllMemberCommissionDistribution(membercomm.Mem_ID, CommType, membercomm.Trans_Amt, membercomm.Trans_Amt, membercomm.Trans_Amt, membercomm.Sp_Key, "Mobile Recharge", membercomm.IpAddress, membercomm.CORELATIONID);
                    if (checkComm == true)
                    {
                        var ApiResponse = db.TBL_INSTANTPAY_RECHARGE_RESPONSE.FirstOrDefault(x => x.CORELATIONID == membercomm.CORELATIONID);
                        ApiResponse.Ipay_Id = membercomm.Ipay_Id;
                        ApiResponse.AgentId = "";
                        ApiResponse.Opr_Id = membercomm.Sp_Key;
                        ApiResponse.AccountNo = membercomm.AccountNo;
                        ApiResponse.Sp_Key = membercomm.Sp_Key;
                        ApiResponse.Trans_Amt = decimal.Parse(membercomm.Trans_Amt.ToString());
                        ApiResponse.Charged_Amt = decimal.Parse(membercomm.Trans_Amt.ToString());
                        ApiResponse.Opening_Balance = decimal.Parse(membercomm.Trans_Amt.ToString());
                        ApiResponse.DateVal = System.DateTime.Now;
                        ApiResponse.Status = membercomm.Status;
                        ApiResponse.Res_Code = membercomm.Status;
                        ApiResponse.res_msg = membercomm.res_msg;
                        ApiResponse.RechargeType = membercomm.RechargeType;
                        ApiResponse.RechargeResponse = membercomm.RechargeResponse;
                        ApiResponse.ERROR_TYPE = "SUCCESS";
                        ApiResponse.ISREVERSE = "Yes";
                        ApiResponse.ISCOMMISSIONDISBURSE = "Yes";
                        db.Entry(ApiResponse).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        return Json("Commission Distributed Successfully", JsonRequestBehavior.AllowGet);
                    }
                    else {
                        return Json("Issue arises on commission distribution.", JsonRequestBehavior.AllowGet);
                    }
                    
                }
                else
                {
                    return Json("Issue arises on commission distribution.", JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json("Issue arises on commission distribution.", JsonRequestBehavior.AllowGet);
                throw ex;
            }

        }
    }
}