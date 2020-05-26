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
using System.Web.Security;
using System.Threading.Tasks;
using WHITELABEL.Web.Areas.Merchant.Models;

namespace WHITELABEL.Web.Areas.Merchant.Controllers
{
    [Authorize]
    public class MerchantRailApplicationController : MerchantBaseController
    {
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
        // GET: Merchant/MerchantRailApplication
        public ActionResult Index()
        {
            if (Session["MerchantUserId"] != null)
            {
                if (Session["MerchantUserId"] != null)
                {
                    initpage();
                    var db = new DBContext();
                    // var checkApplication = db.TBL_APPLICATION_FOR_RAIL_UTILITY.FirstOrDefault(x => x.APPLIED_MER_ID == CurrentMerchant.MEM_ID);
                    var model = new RailUtilityApplication();
                    var Member_info = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
                    model.GST_No = Member_info.COMPANY_GST_NO;
                    model.AadhaarCard = Member_info.AADHAAR_NO;
                    model.PanNo = Member_info.PAN_NO;
                    model.Member_Name = Member_info.MEMBER_NAME;
                    model.EmailId = Member_info.EMAIL_ID;
                    model.Mobile = Member_info.MEMBER_MOBILE;

                    //if (checkApplication != null)
                    //{
                    //    ViewBag.CheckPage = true;
                    //}
                    //else
                    //{
                    //    ViewBag.CheckPage = false;
                    //}
                    return View(model);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<ActionResult> POSTApplyRailUtilityDetails(RailUtilityApplication objrail)
        public async Task<JsonResult> POSTApplyRailUtilityDetails(RailUtilityApplication objrail)
        {
            initpage();
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var checkApplication = db.TBL_APPLICATION_FOR_RAIL_UTILITY.FirstOrDefault(x => x.APPLIED_MER_ID == CurrentMerchant.MEM_ID );
                    if (checkApplication == null)
                    {
                        var Member_Info = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
                        long Mem_Role = 0;
                        long.TryParse(Member_Info.MEMBER_ROLE.ToString(), out Mem_Role);
                        long AdminId = 0;
                        long.TryParse(Member_Info.UNDER_WHITE_LEVEL.ToString(), out AdminId);
                        var dis_id = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == Member_Info.INTRODUCER);
                        long DisId = 0;
                        long.TryParse(dis_id.MEM_ID.ToString(), out DisId);
                        //var Super_id = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == dis_id.INTRODUCER);
                        long SupId = 0;
                        //long.TryParse(Super_id.MEM_ID.ToString(), out SupId);
                        TBL_APPLICATION_FOR_RAIL_UTILITY objutly = new TBL_APPLICATION_FOR_RAIL_UTILITY()
                        {
                            PA_ID = 0,
                            ADMIN_ID = AdminId,
                            SUPER_ID = SupId,
                            DIST_ID = DisId,
                            APPLIED_MER_ID = CurrentMerchant.MEM_ID,
                            APPLICATION_DATE = DateTime.Now,
                            STATUS = 1,
                            APPLIED_MEM_TYPE = Mem_Role,
                            GST_No= objrail.GST_No,
                            PanNo= objrail.PanNo,
                            AadhaarCard= objrail.AadhaarCard,
                            APPLICATION_STATUS="Pending",
                            DESCRIPTION= objrail.Description

                        };
                        db.TBL_APPLICATION_FOR_RAIL_UTILITY.Add(objutly);
                        await db.SaveChangesAsync();
                        Member_Info.COMPANY_GST_NO = objrail.GST_No;
                        Member_Info.AADHAAR_NO = objrail.AadhaarCard;
                        Member_Info.PAN_NO = objrail.PanNo;
                        db.Entry(Member_Info).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();
                        ContextTransaction.Commit();
                        return Json("You have applied for Rail Utility Application. We will contact soon.", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("Your Application Already send to Administratior.", JsonRequestBehavior.AllowGet);
                    }
                    //return Json("Merchant updated successfully.", JsonRequestBehavior.AllowGet);

                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    return Json("Please Try After Sometime", JsonRequestBehavior.AllowGet);
                    //return View();
                    throw ex;
                    //return View("Error", new HandleErrorInfo(ex, "APILabel", "CreateMember"));               
                }
            }
        }
    }
}