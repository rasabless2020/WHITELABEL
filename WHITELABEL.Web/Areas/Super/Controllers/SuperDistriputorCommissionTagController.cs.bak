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
using WHITELABEL.Web.Areas.Super.Models;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;

namespace WHITELABEL.Web.Areas.Super.Controllers
{
    [Authorize]
    public class SuperDistriputorCommissionTagController : SuperBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        public void initpage()
        {
            try
            {
                ViewBag.ControllerName = "Super Bank Details";
                SystemClass sclass = new SystemClass();
                string userID = sclass.GetLoggedUser();
                long userid = long.Parse(userID);
                var dbmain = new DBContext();
                if (userID != null && userID != "")
                {
                    TBL_MASTER_MEMBER currUser = dbmain.TBL_MASTER_MEMBER.SingleOrDefault(c => c.MEM_ID == userid && c.ACTIVE_MEMBER == true && c.MEMBER_ROLE == 3);
                    if (currUser != null)
                    {
                        Session["SuperDistributorId"] = currUser.MEM_ID;
                        // Session["UserName"] = currUser.UserName;
                    }
                    else
                    {
                        Response.Redirect(Url.Action("Index", "SuperLogin", new { area = "Super" }));
                        return;
                    }
                }
                if (Session["SuperDistributorId"] == null)
                {
                    //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
                    //Response.Redirect(Url.Action("Index", "StockistDashboard", new { area = "SuperStockist" }));
                    Response.Redirect(Url.Action("Index", "SuperLogin", new { area = "Super" }));
                    return;
                }
                bool Islogin = false;

                if (Session["SuperDistributorId"] != null)
                {
                    Islogin = true;
                    ViewBag.CurrentUserId = MemberCurrentUser.MEM_ID;
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
        // GET: Super/SuperDistriputorCommissionTag
        public ActionResult Index()
        {
            if (Session["SuperDistributorId"] != null)
            {
                initpage();
                return View();
            }
            else
            {
                Session["SuperDistributorId"] = null;
                Session["SuperDistributorUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("SuperDistributorId");
                Session.Remove("SuperDistributorUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "SuperLogin", new { area = "Super" });
            }
        }

        [HttpPost]
        public async Task<JsonResult> AutoComplete()
        {
            try
            {
                long currentuser = MemberCurrentUser.MEM_ID;
                var db = new DBContext();
                using (DBContext context = new DBContext())
                {
                    var ret = await context.TBL_MASTER_MEMBER.Where(x => x.INTRODUCER == currentuser).Select(x => new { x.MEM_ID, x.UName }).ToListAsync();
                    return Json(ret, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  SuperDistriputorCommissionTag(Super), method:- AutoComplete (GET) Line No:- 102", ex);
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
                    ////var ret = context.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLAB_TYPE == 1).Select(x => new { x.SLN, x.SLAB_NAME }).ToList();
                    //var ret = (from dtlslab in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                    //           join whitelebelslab in db.TBL_WHITE_LEVEL_COMMISSION_SLAB on dtlslab.RECHARGE_SLAB equals whitelebelslab.SLN
                    //           //where dtlslab.WHITE_LEVEL_ID == MemberCurrentUser.MEM_ID && whitelebelslab.SLAB_TYPE == 1 && dtlslab.INTRODUCER_ID == 0
                    //           where dtlslab.INTRODUCER_ID == MemberCurrentUser.MEM_ID && whitelebelslab.SLAB_TYPE == 1
                    //           select new
                    //           {
                    //               SLN = whitelebelslab.SLN,
                    //               SLAB_NAME = whitelebelslab.SLAB_NAME
                    //           }).ToList();
                    var ret = await (from whitelebelslab in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                               where whitelebelslab.MEM_ID == MemberCurrentUser.MEM_ID && whitelebelslab.SLAB_TYPE == 1
                               select new
                               {
                                   SLN = whitelebelslab.SLN,
                                   SLAB_NAME = whitelebelslab.SLAB_NAME
                               }).ToListAsync();
                    return Json(ret, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  SuperDistriputorCommissionTag(Super), method:- MobileRechargeSlab (GET) Line No:- 137", ex);
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
                    ////var ret = context.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLAB_TYPE == 2).Select(x => new { x.SLN, x.SLAB_NAME }).ToList();
                    //var ret = (from dtlslab in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                    //           join whitelebelslab in db.TBL_WHITE_LEVEL_COMMISSION_SLAB on dtlslab.BILLPAYMENT_SLAB equals whitelebelslab.SLN
                    //           //where dtlslab.WHITE_LEVEL_ID == MemberCurrentUser.MEM_ID && whitelebelslab.SLAB_TYPE == 2 && dtlslab.INTRODUCER_ID == 0
                    //           where dtlslab.INTRODUCER_ID == MemberCurrentUser.MEM_ID && whitelebelslab.SLAB_TYPE == 2
                    //           select new
                    //           {
                    //               SLN = whitelebelslab.SLN,
                    //               SLAB_NAME = whitelebelslab.SLAB_NAME
                    //           }).ToList();
                    var ret = await (from whitelebelslab in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                               where whitelebelslab.MEM_ID == MemberCurrentUser.MEM_ID && whitelebelslab.SLAB_TYPE == 2
                               select new
                               {
                                   SLN = whitelebelslab.SLN,
                                   SLAB_NAME = whitelebelslab.SLAB_NAME
                               }).ToListAsync();
                    return Json(ret, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  SuperDistriputorCommissionTag(Super), method:- UtilityRechargeSlab (GET) Line No:- 173", ex);
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
                    ////var ret = context.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLAB_TYPE == 3).Select(x => new { x.SLN, x.SLAB_NAME }).ToList();
                    //var ret = (from dtlslab in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                    //           join whitelebelslab in db.TBL_WHITE_LEVEL_COMMISSION_SLAB on dtlslab.DMR_SLAB equals whitelebelslab.SLN
                    //           //where dtlslab.WHITE_LEVEL_ID == MemberCurrentUser.MEM_ID && whitelebelslab.SLAB_TYPE == 3 && dtlslab.INTRODUCER_ID == 0
                    //           where dtlslab.INTRODUCER_ID == MemberCurrentUser.MEM_ID && whitelebelslab.SLAB_TYPE == 3
                    //           select new
                    //           {
                    //               SLN = whitelebelslab.SLN,
                    //               SLAB_NAME = whitelebelslab.SLAB_NAME
                    //           }).ToList();
                    var ret = await (from whitelebelslab in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                               where whitelebelslab.MEM_ID == MemberCurrentUser.MEM_ID && whitelebelslab.SLAB_TYPE == 3
                               select new
                               {
                                   SLN = whitelebelslab.SLN,
                                   SLAB_NAME = whitelebelslab.SLAB_NAME
                               }).ToListAsync();
                    return Json(ret, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  SuperDistriputorCommissionTag(Super), method:- DMRRechargeSlab (GET) Line No:- 209", ex);
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
                    //var ret = context.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLAB_TYPE == 4).Select(x => new { x.SLN, x.SLAB_NAME }).ToList();
                    var ret = await (from dtlslab in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                               join whitelebelslab in db.TBL_WHITE_LEVEL_COMMISSION_SLAB on dtlslab.AIR_SLAB equals whitelebelslab.SLN
                               //where dtlslab.WHITE_LEVEL_ID == MemberCurrentUser.MEM_ID && whitelebelslab.SLAB_TYPE == 4 && dtlslab.INTRODUCER_ID == 0
                               where dtlslab.INTRODUCER_ID == MemberCurrentUser.MEM_ID && whitelebelslab.SLAB_TYPE == 4
                               select new
                               {
                                   SLN = whitelebelslab.SLN,
                                   SLAB_NAME = whitelebelslab.SLAB_NAME
                               }).ToListAsync();
                    return Json(ret, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  SuperDistriputorCommissionTag(Super), method:- AirSlabDetails (GET) Line No:- 237", ex);
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
                    //var ret = context.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLAB_TYPE == 5).Select(x => new { x.SLN, x.SLAB_NAME }).ToList();
                    var ret = await (from dtlslab in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                               join whitelebelslab in db.TBL_WHITE_LEVEL_COMMISSION_SLAB on dtlslab.BUS_SLAB equals whitelebelslab.SLN
                               //where dtlslab.WHITE_LEVEL_ID == MemberCurrentUser.MEM_ID && whitelebelslab.SLAB_TYPE == 5 && dtlslab.INTRODUCER_ID == 0
                               where dtlslab.INTRODUCER_ID == MemberCurrentUser.MEM_ID && whitelebelslab.SLAB_TYPE == 5
                               select new
                               {
                                   SLN = whitelebelslab.SLN,
                                   SLAB_NAME = whitelebelslab.SLAB_NAME
                               }).ToListAsync();
                    return Json(ret, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  SuperDistriputorCommissionTag(Super), method:- BusSlabDetails (GET) Line No:- 264", ex);
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
                    //var ret = context.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLAB_TYPE == 6).Select(x => new { x.SLN, x.SLAB_NAME }).ToList();
                    var ret =await (from dtlslab in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                               join whitelebelslab in db.TBL_WHITE_LEVEL_COMMISSION_SLAB on dtlslab.HOTEL_SLAB equals whitelebelslab.SLN
                               //where dtlslab.WHITE_LEVEL_ID == MemberCurrentUser.MEM_ID && whitelebelslab.SLAB_TYPE == 6 && dtlslab.INTRODUCER_ID == 0
                               where dtlslab.INTRODUCER_ID == MemberCurrentUser.MEM_ID && whitelebelslab.SLAB_TYPE == 6
                               select new
                               {
                                   SLN = whitelebelslab.SLN,
                                   SLAB_NAME = whitelebelslab.SLAB_NAME
                               }).ToListAsync();
                    return Json(ret, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  SuperDistriputorCommissionTag(Super), method:- HotelSlabDetails (GET) Line No:- 291", ex);
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
                    //var ret = context.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLAB_TYPE == 7).Select(x => new { x.SLN, x.SLAB_NAME }).ToList();
                    var ret = await(from dtlslab in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                               join whitelebelslab in db.TBL_WHITE_LEVEL_COMMISSION_SLAB on dtlslab.CASHCARD_SLAB equals whitelebelslab.SLN
                               //where dtlslab.WHITE_LEVEL_ID == MemberCurrentUser.MEM_ID && whitelebelslab.SLAB_TYPE == 7 && dtlslab.INTRODUCER_ID == 0
                               where dtlslab.INTRODUCER_ID == MemberCurrentUser.MEM_ID && whitelebelslab.SLAB_TYPE == 7
                               select new
                               {
                                   SLN = whitelebelslab.SLN,
                                   SLAB_NAME = whitelebelslab.SLAB_NAME
                               }).ToListAsync();
                    return Json(ret, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  SuperDistriputorCommissionTag(Super), method:- CashCardSlabDetails (GET) Line No:- 317", ex);
                throw ex;
            }
        }
        [HttpPost]
        public async Task<JsonResult> SaveCommissionSlab(SuperCommissionSlabTaggingModelView objval)
        {

            try
            {
                var db = new DBContext();
                var checkVal =await  db.TBL_DETAILS_MEMBER_COMMISSION_SLAB.Where(x => x.SL_NO == objval.SL_NO).FirstOrDefaultAsync();
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
                        WHITE_LEVEL_ID = MemberCurrentUser.MEM_ID,
                        INTRODUCER_ID = objval.WHITE_LEVEL_ID,
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
                Logger.Error("Controller:-  SuperDistriputorCommissionTag(Super), method:- SaveCommissionSlab (POST) Line No:- 370", ex);
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
                            where valid.WHITE_LEVEL_ID == MemberCurrentUser.MEM_ID && valid.INTRODUCER_ID != 0
                            select new
                            {
                                ID = valid.SL_NO,
                                WHITE_LEVEL_ID = valid.INTRODUCER_ID,
                                WHITELEVELNAME1 = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == valid.INTRODUCER_ID).Select(z => z.MEMBER_NAME).FirstOrDefault(),
                                INTRODUCE_TO_ID = valid.INTRODUCE_TO_ID,
                                INTRODUCER_ID = valid.INTRODUCER_ID,
                                RechargeName = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.RECHARGE_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                BillName = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.BILLPAYMENT_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                DMR_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.DMR_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                AIR_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.AIR_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                BUS_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.BUS_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                HOTEL_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.HOTEL_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                CASHCARD_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.CASHCARD_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                            }).AsEnumerable().Select(s => new SuperCommissionSlabTaggingModelView
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
                Logger.Error("Controller:-  SuperDistriputorCommissionTag(Super), method:- GetListInformation (POST) Line No:- 418", ex);
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
                            where valid.WHITE_LEVEL_ID == MemberCurrentUser.MEM_ID && valid.INTRODUCER_ID == mem_idval
                            select new
                            {
                                ID = valid.SL_NO,
                                WHITE_LEVEL_ID = valid.INTRODUCER_ID,
                                WHITELEVELNAME1 = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == valid.INTRODUCER_ID).Select(z => z.MEMBER_NAME).FirstOrDefault(),
                                INTRODUCE_TO_ID = valid.INTRODUCE_TO_ID,
                                INTRODUCER_ID = valid.INTRODUCER_ID,
                                RechargeName = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.RECHARGE_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                BillName = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.BILLPAYMENT_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                DMR_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.DMR_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                AIR_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.AIR_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                BUS_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.BUS_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                HOTEL_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.HOTEL_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                                CASHCARD_SLAB_Name = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == valid.CASHCARD_SLAB).Select(z => z.SLAB_NAME).FirstOrDefault(),
                            }).AsEnumerable().Select(s => new SuperCommissionSlabTaggingModelView
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
                Logger.Error("Controller:-  SuperDistriputorCommissionTag(Super), method:- GetMemberListInformation (POST) Line No:- 466", ex);
                throw;
            }

            
        }
        [HttpPost]
        public JsonResult fetchMemCommInfo(string Mem_Id, string WhitelevelId)
        {
            try
            {
                long mem_idval = long.Parse(Mem_Id);
                long WhiteLevelID = long.Parse(WhitelevelId);
                var db = new DBContext();
                var listinfo = (from valid in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                                where valid.WHITE_LEVEL_ID == MemberCurrentUser.MEM_ID && valid.INTRODUCER_ID == WhiteLevelID && valid.SL_NO == mem_idval
                                select new
                                {
                                    ID = valid.SL_NO,
                                    WHITE_LEVEL_ID = valid.INTRODUCER_ID,
                                    WHITELEVELNAME1 = db.TBL_WHITE_LEVEL_HOSTING_DETAILS.Where(x => x.MEM_ID == valid.INTRODUCER_ID).Select(z => z.DOMAIN).FirstOrDefault(),
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

                                }).AsEnumerable().Select(s => new SuperCommissionSlabTaggingModelView
                                {
                                    SL_NO = s.ID,
                                    WHITE_LEVEL_ID = s.WHITE_LEVEL_ID,
                                    WHITELEVELNAME1 = s.WHITELEVELNAME1,
                                    INTRODUCE_TO_ID = s.INTRODUCE_TO_ID,
                                    INTRODUCER_ID = s.INTRODUCER_ID,
                                    MobileRechargeSlabdetails = s.MobileRechargeSlabdetails,
                                    UtilityRechargeSlabdetails = s.UtilityRechargeSlabdetails,
                                    DMRRechargeSlabdetails = s.DMRRechargeSlabdetails,
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
                Logger.Error("Controller:-  SuperDistriputorCommissionTag(Super), method:- fetchMemCommInfo (POST) Line No:- 531", ex);
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
                Logger.Error("Controller:-  SuperDistriputorCommissionTag(Super), method:- BindCommissionSlabTagg (POST) Line No:- 549", ex);
                throw ex;
            }
        }

    }
}