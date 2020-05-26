using log4net;
using NonFactors.Mvc.Grid;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Areas.PowerAdmin.Models;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;

namespace WHITELABEL.Web.Areas.PowerAdmin.Controllers
{
    [Authorize]
    public class PowerAdminCommissionTagController : PoweradminbaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        // GET: PowerAdmin/PowerAdminCommissionTag
        public void initpage()
        {
            try
            {
                ViewBag.ControllerName = "Power Admin Commission Tag";
                SystemClass sclass = new SystemClass();
                string userID = sclass.GetLoggedUser();
                long userid = long.Parse(userID);
                var dbmain = new DBContext();
                if (userID != null && userID != "")
                {
                    TBL_AUTH_ADMIN_USER currUser = dbmain.TBL_AUTH_ADMIN_USERS.SingleOrDefault(c => c.USER_ID == userid && c.ACTIVE_USER == true);
                    if (currUser != null)
                    {
                        Session["PowerAdminUserId"] = currUser.USER_ID;
                        // Session["UserName"] = currUser.UserName;
                    }
                }
                if (Session["PowerAdminUserId"] == null)
                {
                    //Response.Redirect("~/Login/LogOut");
                    Response.Redirect(Url.Action("Index", "PowerAdminLogin", new { area = "PowerAdmin" }));
                    return;
                }
                bool Islogin = false;
                if (Session["PowerAdminUserId"] != null)
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
            if (Session["PowerAdminUserId"] != null)
            {
                var db = new DBContext();
                var memberService = (from x in db.TBL_WHITE_LEVEL_HOSTING_DETAILS
                                     select new
                                     {
                                         MEM_ID = x.MEM_ID,
                                         UName = x.DOMAIN
                                     }).AsEnumerable().Select(z => new MemberView
                                     {
                                         IDValue = Encrypt.EncryptMe(z.MEM_ID.ToString()),
                                         TextValue = z.UName
                                     }).ToList().Distinct();
                ViewBag.MemberService = new SelectList(memberService, "IDValue", "TextValue");
                return View();
            }
            else
            {
                Session["PowerAdminUserId"] = null;
                Session["PowerAdminUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("PowerAdminUserId");
                Session.Remove("PowerAdminUserName");
                Session.Remove("UserType");
                Response.Redirect(Url.Action("Index", "PowerAdminLogin", new { area = "PowerAdmin" }));
                return View();
            }
            //return View();
        }
        [HttpPost]
        public async Task<JsonResult> AutoComplete()
        {
            try
            {
                var db = new DBContext();
                using (DBContext context = new DBContext())
                {
                    var ret = await context.TBL_WHITE_LEVEL_HOSTING_DETAILS.Select(x => new { x.MEM_ID, x.DOMAIN }).ToListAsync();
                    return Json(ret, JsonRequestBehavior.AllowGet);
                }                
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  PowerAdminCommissionTag(Distributor), method:- AutoComplete (POST) Line No:- 103", ex);                
                throw ex;
            }
        }

        [HttpPost]
        public async Task<JsonResult> MobileRechargeSlab()
        {
            try
            {
                var db = new DBContext();
                using (DBContext context = new DBContext())
                {
                    var ret = await context.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLAB_TYPE == 1 && x.SLAB_STATUS == true).Select(x => new { x.SLN, x.SLAB_NAME }).ToListAsync();
                    return Json(ret, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  PowerAdminCommissionTag(Distributor), method:- MobileRechargeSlab (POST) Line No:- 123", ex);
                throw ex;
            }
            
        }

        [HttpPost]
        public async Task<JsonResult> UtilityRechargeSlab()
        {
            try
            {
                var db = new DBContext();
                using (DBContext context = new DBContext())
                {
                    var ret = await context.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLAB_TYPE == 2 && x.SLAB_STATUS == true).Select(x => new { x.SLN, x.SLAB_NAME }).ToListAsync();
                    return Json(ret, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  PowerAdminCommissionTag(Distributor), method:- UtilityRechargeSlab (POST) Line No:- 142", ex);
                throw ex;
            }

        }

        [HttpPost]
        public async Task<JsonResult> DMRRechargeSlab()
        {
            try
            {
                var db = new DBContext();
                using (DBContext context = new DBContext())
                {
                    var ret = await context.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLAB_TYPE == 3 && x.SLAB_STATUS==true).Select(x => new { x.SLN, x.SLAB_NAME }).ToListAsync();
                    return Json(ret, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  PowerAdminCommissionTag(Distributor), method:- DMRRechargeSlab (POST) Line No:- 162", ex);
                throw ex;
            }

        }
        [HttpPost]
        public async Task<JsonResult> AirSlabDetails()
        {
            try
            {
                var db = new DBContext();
                using (DBContext context = new DBContext())
                {
                    var ret = await context.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLAB_TYPE == 4).Select(x => new { x.SLN, x.SLAB_NAME }).ToListAsync  ();
                    return Json(ret, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  PowerAdminCommissionTag(Distributor), method:- AirSlabDetails (POST) Line No:- 181", ex);
                throw ex;
            }
        }
        [HttpPost]
        public async Task<JsonResult> BusSlabDetails()
        {
            try
            {
                var db = new DBContext();
                using (DBContext context = new DBContext())
                {
                    var ret = await context.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLAB_TYPE == 5).Select(x => new { x.SLN, x.SLAB_NAME }).ToListAsync();
                    return Json(ret, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  PowerAdminCommissionTag(Distributor), method:- BusSlabDetails (POST) Line No:- 199", ex);
                throw ex;
            }
        }
        [HttpPost]
        public async Task<JsonResult> HotelSlabDetails()
        {
            try
            {
                var db = new DBContext();
                using (DBContext context = new DBContext())
                {
                    var ret = await context.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLAB_TYPE == 6).Select(x => new { x.SLN, x.SLAB_NAME }).ToListAsync();
                    return Json(ret, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  PowerAdminCommissionTag(Distributor), method:- HotelSlabDetails (POST) Line No:- 217", ex);
                throw ex;
            }
        }
        [HttpPost]
        public async Task<JsonResult> CashCardSlabDetails()
        {
            try
            {
                var db = new DBContext();
                using (DBContext context = new DBContext())
                {
                    var ret = await context.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLAB_TYPE == 7).Select(x => new { x.SLN, x.SLAB_NAME }).ToListAsync();
                    return Json(ret, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  PowerAdminCommissionTag(Distributor), method:- CashCardSlabDetails (POST) Line No:- 235", ex);
                throw ex;
            }
        }

        [HttpPost]
        public async Task<JsonResult> SaveCommissionSlab(CommissionSlabTaggingModelView objval)
        {

            try
            {
               

                    var db = new DBContext();
                    var checkVal = await db.TBL_DETAILS_MEMBER_COMMISSION_SLAB.Where(x => x.SL_NO == objval.SL_NO).FirstOrDefaultAsync();
                    //var checkVal = db.TBL_DETAILS_MEMBER_COMMISSION_SLAB.Where(x => x.WHITE_LEVEL_ID == objval.WHITE_LEVEL_ID && x.RECHARGE_SLAB==objval.RECHARGE_SLAB && x.BILLPAYMENT_SLAB==objval.BILLPAYMENT_SLAB && x.DMR_SLAB==objval.DMR_SLAB ).FirstOrDefault();
                    if (checkVal != null)
                    {
                        checkVal.RECHARGE_SLAB = objval.RECHARGE_SLAB;
                        checkVal.BILLPAYMENT_SLAB = objval.BILLPAYMENT_SLAB;
                        checkVal.DMR_SLAB = objval.DMR_SLAB;
                        checkVal.AIR_SLAB = objval.AIR_SLAB;
                        checkVal.BUS_SLAB = objval.BUS_SLAB;
                        checkVal.HOTEL_SLAB = objval.HOTEL_SLAB;
                        checkVal.CASHCARD_SLAB = objval.CASHCARD_SLAB;
                        db.Entry(checkVal).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();
                        return Json(new { Result = "Updated" });
                    }
                    else
                    {
                        TBL_DETAILS_MEMBER_COMMISSION_SLAB objmodel = new TBL_DETAILS_MEMBER_COMMISSION_SLAB()
                        {
                            WHITE_LEVEL_ID = objval.WHITE_LEVEL_ID,
                            INTRODUCER_ID = objval.INTRODUCER_ID,
                            INTRODUCE_TO_ID = objval.INTRODUCE_TO_ID,
                            RECHARGE_SLAB = objval.RECHARGE_SLAB,
                            BILLPAYMENT_SLAB = objval.BILLPAYMENT_SLAB,
                            DMR_SLAB = objval.DMR_SLAB,
                            AIR_SLAB = objval.AIR_SLAB,
                            BUS_SLAB = objval.BUS_SLAB,
                            HOTEL_SLAB = objval.HOTEL_SLAB,
                            CASHCARD_SLAB = objval.CASHCARD_SLAB,
                            STATUS = true,
                            CREATED_DATE = DateTime.Now
                        };
                        db.TBL_DETAILS_MEMBER_COMMISSION_SLAB.Add(objmodel);
                        await db.SaveChangesAsync();
                        return Json(new { Result = "Success" });
                    }
                
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  PowerAdminCommissionTag(Distributor), method:- SaveCommissionSlab (POST) Line No:- 289", ex);
                throw ex;
            }

            return Json("");
        }

        [HttpPost]
        public JsonResult GetListInformation()
        {
            try
            {
                var db = new DBContext();
                var list = (from valid in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                            select new
                            {
                                ID = valid.SL_NO,
                                WHITE_LEVEL_ID = valid.WHITE_LEVEL_ID,
                                WHITELEVELNAME1 = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == valid.WHITE_LEVEL_ID).Select(z => z.MEMBER_NAME).FirstOrDefault(),
                                INTRODUCE_TO_ID = valid.INTRODUCE_TO_ID,
                                INTRODUCER_ID = valid.INTRODUCER_ID,
                                RechargeName = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.RECHARGE_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                BillName = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.BILLPAYMENT_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                DMR_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.DMR_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                AIR_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.AIR_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                BUS_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.BUS_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                HOTEL_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.HOTEL_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                CASHCARD_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.CASHCARD_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                            }).AsEnumerable().Select(s => new CommissionSlabTaggingModelView
                            {
                                SL_NO = s.ID,
                                WHITE_LEVEL_ID = s.WHITE_LEVEL_ID,
                                WHITELEVELNAME1 = s.WHITELEVELNAME1,
                                INTRODUCE_TO_ID = s.INTRODUCE_TO_ID,
                                INTRODUCER_ID = s.INTRODUCER_ID,
                                RechargeName = s.RechargeName,
                                BillName = s.BillName,
                                DMR_SLAB_Name = s.DMR_SLAB_Name,
                                AIR_SLAB_Name = s.AIR_SLAB_Name,
                                BUS_SLAB_Name = s.BUS_SLAB_Name,
                                HOTEL_SLAB_Name = s.HOTEL_SLAB_Name,
                                CASHCARD_SLAB_Name = s.CASHCARD_SLAB_Name
                            }).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  PowerAdminCommissionTag(Distributor), method:- GetListInformation (POST) Line No:- 336", ex);
                throw;
            }

            
        }

        [HttpPost]
        public JsonResult GetMemberListInformation(string Mem_Id)
        {
            try
            {
                long mem_idval = long.Parse(Mem_Id);
                var db = new DBContext();
                var list = (from valid in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                            where valid.WHITE_LEVEL_ID == mem_idval
                            select new
                            {
                                ID = valid.SL_NO,
                                WHITE_LEVEL_ID = valid.WHITE_LEVEL_ID,
                                WHITELEVELNAME1 = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == valid.WHITE_LEVEL_ID).Select(z => z.MEMBER_NAME).FirstOrDefault(),
                                INTRODUCE_TO_ID = valid.INTRODUCE_TO_ID,
                                INTRODUCER_ID = valid.INTRODUCER_ID,
                                RechargeName = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.RECHARGE_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                BillName = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.BILLPAYMENT_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                DMR_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.DMR_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                AIR_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.AIR_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                BUS_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.BUS_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                HOTEL_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.HOTEL_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                CASHCARD_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.CASHCARD_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                            }).AsEnumerable().Select(s => new CommissionSlabTaggingModelView
                            {
                                SL_NO = s.ID,
                                WHITE_LEVEL_ID = s.WHITE_LEVEL_ID,
                                WHITELEVELNAME1 = s.WHITELEVELNAME1,
                                INTRODUCE_TO_ID = s.INTRODUCE_TO_ID,
                                INTRODUCER_ID = s.INTRODUCER_ID,
                                RechargeName = s.RechargeName,
                                BillName = s.BillName,
                                DMR_SLAB_Name = s.DMR_SLAB_Name,
                                AIR_SLAB_Name = s.AIR_SLAB_Name,
                                BUS_SLAB_Name = s.BUS_SLAB_Name,
                                HOTEL_SLAB_Name = s.HOTEL_SLAB_Name,
                                CASHCARD_SLAB_Name = s.CASHCARD_SLAB_Name
                            }).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  PowerAdminCommissionTag(Distributor), method:- GetMemberListInformation (POST) Line No:- 385", ex);
                throw;
            }

            
        }
        [HttpPost]
        public JsonResult fetchMemCommInfo(string Mem_Id, string WhitelevelId)
        {
            try
            {
                long mem_idval = long.Parse(Mem_Id);
                long WhiteLevelID= long.Parse(WhitelevelId);
                var db = new DBContext();
                var listinfo = (from valid in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                                where valid.WHITE_LEVEL_ID == WhiteLevelID && valid.SL_NO==mem_idval
                                select new
                                {
                                    ID = valid.SL_NO,
                                    WHITE_LEVEL_ID = valid.WHITE_LEVEL_ID,
                                    WHITELEVELNAME1 = db.TBL_WHITE_LEVEL_HOSTING_DETAILS.Where(x => x.MEM_ID == valid.WHITE_LEVEL_ID).Select(z => z.DOMAIN).FirstOrDefault(),
                                    INTRODUCE_TO_ID = valid.INTRODUCE_TO_ID,
                                    INTRODUCER_ID = valid.INTRODUCER_ID,
                                    RechargeName = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.RECHARGE_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                    BillName = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.BILLPAYMENT_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                    DMR_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.DMR_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                    AIR_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.AIR_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                    BUS_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.BUS_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                    HOTEL_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.HOTEL_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                    CASHCARD_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.CASHCARD_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                    MobileRechargeSlabdetails = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.RECHARGE_SLAB).Select(z => z.SLN).FirstOrDefault(),
                                    UtilityRechargeSlabdetails = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.BILLPAYMENT_SLAB).Select(z => z.SLN).FirstOrDefault(),
                                    DMRRechargeSlabdetails = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.DMR_SLAB).Select(z => z.SLN).FirstOrDefault(),
                                    AIRSlabdetailsList = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.AIR_SLAB).Select(z => z.SLN).FirstOrDefault(),
                                    BusSlabdetailsList = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.BUS_SLAB).Select(z => z.SLN).FirstOrDefault(),
                                    HotelSlabdetailsList = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.HOTEL_SLAB).Select(z => z.SLN).FirstOrDefault(),
                                    CashCardSlabdetailsList = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.CASHCARD_SLAB).Select(z => z.SLN).FirstOrDefault(),

                                }).AsEnumerable().Select(s => new CommissionSlabTaggingModelView
                                {
                                    SL_NO = s.ID,
                                    WHITE_LEVEL_ID = s.WHITE_LEVEL_ID,
                                    WHITELEVELNAME1 = s.WHITELEVELNAME1,
                                    INTRODUCE_TO_ID = s.INTRODUCE_TO_ID,
                                    INTRODUCER_ID = s.INTRODUCER_ID,
                                    MobileRechargeSlabdetails=s.MobileRechargeSlabdetails,
                                    UtilityRechargeSlabdetails=s.UtilityRechargeSlabdetails,
                                    DMRRechargeSlabdetails=s.DMRRechargeSlabdetails,
                                    AIRSlabdetailsList = s.AIRSlabdetailsList,
                                    BusSlabdetailsList = s.BusSlabdetailsList,
                                    HotelSlabdetailsList = s.HotelSlabdetailsList,
                                    CashCardSlabdetailsList = s.CashCardSlabdetailsList,
                                    RechargeName = s.RechargeName,
                                    BillName = s.BillName,
                                    DMR_SLAB_Name = s.DMR_SLAB_Name,
                                    AIR_SLAB_Name = s.AIR_SLAB_Name,
                                    BUS_SLAB_Name = s.BUS_SLAB_Name,
                                    HOTEL_SLAB_Name = s.HOTEL_SLAB_Name,
                                    CASHCARD_SLAB_Name = s.CASHCARD_SLAB_Name
                                }).FirstOrDefault();

                return Json(listinfo, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  PowerAdminCommissionTag(Distributor), method:- GetMemberListInformation (POST) Line No:- 450", ex);
                throw ex;
            }
            
        }


        [HttpPost]
        public async Task<JsonResult> BindCommissionSlabTagg()
        {
            try
            {
                var db = new DBContext();
                var Commlist = await db.TBL_DETAILS_MEMBER_COMMISSION_SLAB.ToListAsync();
                return Json(Commlist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  PowerAdminCommissionTag(Distributor), method:- BindCommissionSlabTagg (POST) Line No:- 468", ex);
                throw ex;
            }
        }
    }
}