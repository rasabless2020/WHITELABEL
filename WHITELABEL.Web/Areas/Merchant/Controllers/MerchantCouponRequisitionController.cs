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
using System.Text.RegularExpressions;

namespace WHITELABEL.Web.Areas.Merchant.Controllers
{
    [Authorize]
    public class MerchantCouponRequisitionController : MerchantBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        // GET: Merchant/MerchantCouponRequisition
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
        public ActionResult Index()
        {
            if (Session["MerchantUserId"] != null)
            {
                initpage();
                return View();
            }
            else
            {
                Session["MerchantUserId"] = null;
                Session["MerchantUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("MerchantUserId");
                Session.Remove("MerchantUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        public PartialViewResult IndexGrid()
        {
            try
            {
                var db = new DBContext();
                initpage();
                var Tramslist = (from Cpn in db.TBL_COUPON_TRANSFER_LOGS
                                 where Cpn.FROM_MEMBER == CurrentMerchant.MEM_ID && Cpn.MEM_ROLE == 5
                                 select new
                                 {
                                     Sln = Cpn.SLN,
                                     From_Mem = Cpn.FROM_MEMBER,
                                     RefNo = Cpn.REFERENCE_NO,
                                     ReqDate = Cpn.REQUEST_DATE,
                                     MemBer_Name = db.TBL_MASTER_MEMBER.Where(a => a.MEM_ID == Cpn.FROM_MEMBER).Select(c => c.MEMBER_NAME).FirstOrDefault(),
                                     CouponType = Cpn.COUPON_TYPE,
                                     CouponName = db.TBL_COUPON_MASTER.Where(a => a.sln == Cpn.COUPON_TYPE).Select(c => c.couponType).FirstOrDefault(),
                                     Qty = Cpn.QTY,
                                     Status = Cpn.STATUS,
                                 }).AsEnumerable().Select(z => new TBL_COUPON_TRANSFER_LOGS
                                 {
                                     SLN = z.Sln,
                                     FROM_MEMBER = z.From_Mem,
                                     REFERENCE_NO = z.RefNo,
                                     REQUEST_DATE = z.ReqDate,
                                     Member_Name = z.MemBer_Name,
                                     COUPON_Name = z.CouponName,
                                     COUPON_TYPE = z.CouponType,
                                     QTY = z.Qty,
                                     STATUS = z.Status
                                 }).ToList();

                //var Tramslist1 = db.TBL_COUPON_TRANSFER_LOGS.Where(x => x.STATUS == "Pending" && x.MEM_ROLE == 3).ToList();
                return PartialView("IndexGrid", Tramslist);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult ApplyForCoupon()
        {
            if (Session["MerchantUserId"] != null)
            {
                initpage();
                var db = new DBContext();
                var BankInformation = (from x in db.TBL_COUPON_MASTER

                                       select new
                                       {
                                           SLn = x.sln,
                                           CouponName = x.couponType
                                       }).AsEnumerable().Select(z => new ViewcouponDetails
                                       {
                                           sln = z.SLn.ToString(),
                                           couponType = z.CouponName
                                       }).ToList().Distinct();
                ViewBag.BankInformation = new SelectList(BankInformation, "sln", "couponType");
                return View();
            }
            else
            {
                Session["MerchantUserId"] = null;
                Session["MerchantUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("MerchantUserId");
                Session.Remove("MerchantUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });
            }



        }
        public static string GetUniqueKey(string userID)
        {
            string resultString = Regex.Match(userID, @"\d+").Value;
            long ticks = DateTime.Now.Ticks;
            string result = resultString + ticks.ToString();
            return result.ToString();
        }

        [HttpPost]
        public async Task<JsonResult> PostMerchantCouponStockRequisition(TBL_COUPON_TRANSFER_LOGS objReq)
        {
            initpage();
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var getSuper = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == CurrentMerchant.MEM_ID);
                    long SuperId = 0;
                    long.TryParse(getSuper.INTRODUCER.ToString(), out SuperId);
                    var GetCouponType = db.TBL_COUPON_MASTER.FirstOrDefault(x => x.couponType == objReq.COUPON_Name);
                    string GetRefNo = GetUniqueKey(CurrentMerchant.MEM_ID.ToString());
                    TBL_COUPON_TRANSFER_LOGS objCoupon = new TBL_COUPON_TRANSFER_LOGS()
                    {
                        REFERENCE_NO = GetRefNo,
                        TO_MEMBER = 0,
                        FROM_MEMBER = CurrentMerchant.MEM_ID,
                        REQUEST_DATE = objReq.REQUEST_DATE,
                        REQUEST_TIME = DateTime.Now,
                        QTY = objReq.QTY,
                        COUPON_TYPE = GetCouponType.sln,
                        STATUS = "Pending",
                        APPROVED_BY = "",
                        REMARKS = objReq.REMARKS,
                        COMM_SLAB_ID = 0,
                        SELL_VALUE_RATE = 0,
                        GROSS_SELL_VALUE = 0,
                        NET_SELL_VALUE = 0,
                        GST_PERCENTAGE = 0,
                        GST_VALUE = 0,
                        TDS_PERCENTAGE = 0,
                        TDS_VALUE = 0,
                        GST_INPUT = 0,
                        GST_OUTPUT = 0,
                        IS_OVERRIDE_SELL_VALUE = false,
                        MEM_ROLE = 5
                    };
                    db.TBL_COUPON_TRANSFER_LOGS.Add(objCoupon);
                    db.SaveChanges();
                    ContextTransaction.Commit();
                    return Json("Requisition send to admin", JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    throw ex;
                }

            }
        }

        public ActionResult MerchantCouponStockReport()
        {
            if (Session["MerchantUserId"] != null)
            {
                initpage();
                return View();
            }
            else
            {
                Session["MerchantUserId"] = null;
                Session["MerchantUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("MerchantUserId");
                Session.Remove("MerchantUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        public PartialViewResult GetMerchantCouponStockIndexGrid()
        {
            initpage();
            try
            {
                var db = new DBContext();
                var Tramslist = (from Cpn in db.TBL_COUPON_STOCK
                                 where Cpn.couponHoderID == CurrentMerchant.MEM_ID
                                 select new
                                 {
                                     sln = Cpn.sln,
                                     HolderName = db.TBL_MASTER_MEMBER.Where(a => a.MEM_ID == Cpn.couponHoderID).Select(c => c.MEMBER_NAME).FirstOrDefault(),
                                     PurchaseDate = Cpn.puchaseDate,
                                     stockEntryDate = Cpn.stockEntryDate,
                                     CouponName = db.TBL_COUPON_MASTER.Where(a => a.sln == Cpn.couponType).Select(c => c.couponType).FirstOrDefault(),
                                     CouponId = Cpn.couponType,
                                     Qty = Cpn.couponQty,
                                     VendorName = Cpn.Vendor_Name,
                                 }).AsEnumerable().Select(z => new TBL_COUPON_STOCK
                                 {
                                     sln = z.sln,
                                     HolderName = z.HolderName,
                                     puchaseDate = z.PurchaseDate,
                                     stockEntryDate = z.stockEntryDate,
                                     CouponName = z.CouponName,
                                     couponType = z.CouponId,
                                     couponQty = z.Qty,
                                     Vendor_Name = z.VendorName
                                 }).ToList();
                return PartialView("GetMerchantCouponStockIndexGrid", Tramslist);
                //return PartialView(CreateExportableGrid());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult MerchantRequisitionCouponReport()
        {
            if (Session["MerchantUserId"] != null)
            {
                initpage();
                return View();
            }
            else
            {
                Session["MerchantUserId"] = null;
                Session["MerchantUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("MerchantUserId");
                Session.Remove("MerchantUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }

        public PartialViewResult MerchantRequisitionCouponReportIndexGrid()
        {
            initpage();
            try
            {
                var db = new DBContext();

                var Tramslist = (from Cpn in db.TBL_COUPON_TRANSFER_LOGS
                                 where Cpn.MEM_ROLE == 5 && Cpn.FROM_MEMBER == CurrentMerchant.MEM_ID
                                 select new
                                 {
                                     Sln = Cpn.SLN,
                                     From_Mem = Cpn.FROM_MEMBER,
                                     RefNo = Cpn.REFERENCE_NO,
                                     ReqDate = Cpn.REQUEST_DATE,
                                     MemBer_Name = db.TBL_MASTER_MEMBER.Where(a => a.MEM_ID == Cpn.FROM_MEMBER).Select(c => c.MEMBER_NAME).FirstOrDefault(),
                                     CouponType = Cpn.COUPON_TYPE,
                                     CouponName = db.TBL_COUPON_MASTER.Where(a => a.sln == Cpn.COUPON_TYPE).Select(c => c.couponType).FirstOrDefault(),
                                     Qty = Cpn.QTY,
                                     Status = Cpn.STATUS,
                                 }).AsEnumerable().Select(z => new TBL_COUPON_TRANSFER_LOGS
                                 {
                                     SLN = z.Sln,
                                     FROM_MEMBER = z.From_Mem,
                                     REFERENCE_NO = z.RefNo,
                                     REQUEST_DATE = z.ReqDate,
                                     Member_Name = z.MemBer_Name,
                                     COUPON_Name = z.CouponName,
                                     COUPON_TYPE = z.CouponType,
                                     QTY = z.Qty,
                                     STATUS = z.Status
                                 }).ToList();

                var Tramslist1 = db.TBL_COUPON_TRANSFER_LOGS.Where(x => x.STATUS == "Pending" && x.MEM_ROLE == 3).ToList();
                return PartialView("MerchantRequisitionCouponReportIndexGrid", Tramslist);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost]
        public JsonResult PrintMerchantcouponInvoice(string TransId = "")
        {
            initpage();////
            try
            {
                var db = new DBContext();
                long transid = long.Parse(TransId);
                var Tramslist = (from Cpn in db.TBL_COUPON_TRANSFER_LOGS
                                 join mem in db.TBL_MASTER_MEMBER on Cpn.FROM_MEMBER equals mem.MEM_ID
                                 where Cpn.STATUS == "Success" && Cpn.MEM_ROLE == 5 && Cpn.SLN == transid
                                 select new
                                 {
                                     Sln = Cpn.SLN,
                                     From_Mem = Cpn.FROM_MEMBER,
                                     RefNo = Cpn.REFERENCE_NO,
                                     ReqDate = Cpn.REQUEST_DATE,
                                     //MemBer_Name = db.TBL_MASTER_MEMBER.Where(a => a.MEM_ID == Cpn.FROM_MEMBER).Select(c => c.MEMBER_NAME).FirstOrDefault(),
                                     MemBer_Name = mem.MEMBER_NAME,
                                     CouponType = Cpn.COUPON_TYPE,
                                     CouponName = db.TBL_COUPON_MASTER.Where(a => a.sln == Cpn.COUPON_TYPE).Select(c => c.couponType).FirstOrDefault(),
                                     Qty = Cpn.QTY,
                                     Status = Cpn.STATUS,
                                     //CouponPrice = db.TBL_COUPON_COMMISSION_SLAB.Where(a => a.coupon_id == Cpn.COUPON_TYPE && a.Super_Role_Id == 3).Select(c => c.Super_Comm_Value).FirstOrDefault(),
                                     CouponPrice = Cpn.SELL_VALUE_RATE,
                                     GstNo = mem.COMPANY_GST_NO,
                                     CompanyName = mem.COMPANY,
                                     Address = mem.ADDRESS,
                                     Mobile = mem.MEMBER_MOBILE,
                                     Total = (Cpn.QTY * Cpn.SELL_VALUE_RATE),
                                     GST_Amount = Cpn.GST_VALUE,
                                     TDS_Amount = Cpn.TDS_VALUE,
                                     NetValue = Cpn.NET_SELL_VALUE,
                                     ImageVal = db.TBL_MASTER_MEMBER.Where(a => a.UNDER_WHITE_LEVEL == 0 && a.MEMBER_ROLE == 1).Select(c => c.LOGO).FirstOrDefault(),
                                 }).AsEnumerable().Select(z => new TBL_COUPON_TRANSFER_LOGS
                                 {
                                     SLN = z.Sln,
                                     FROM_MEMBER = z.From_Mem,
                                     REFERENCE_NO = z.RefNo,
                                     REQUEST_DATE = z.ReqDate,
                                     Member_Name = z.MemBer_Name,
                                     COUPON_Name = z.CouponName,
                                     COUPON_TYPE = z.CouponType,
                                     QTY = z.Qty,
                                     STATUS = z.Status,
                                     CouponPrice = Convert.ToDecimal(z.CouponPrice),
                                     GSTNo = z.GstNo,
                                     CompanyName = z.CompanyName,
                                     Address = z.Address,
                                     Mem_Mobile = z.Mobile,
                                     TotalAmount = z.Total,
                                     GST_VALUE = z.GST_Amount,
                                     TDS_VALUE = z.TDS_Amount,
                                     NET_SELL_VALUE = z.NetValue,
                                     Logo = z.ImageVal
                                 }).FirstOrDefault();

                return Json(Tramslist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var listdetails = new TBL_BALANCE_TRANSFER_LOGS();
                Logger.Error("Controller:-  MemberRequisition(Admin), method:- getTransdata (POST) Line No:- 433", ex);
                return Json(new { Result = "false", data = listdetails });
            }

        }

    }
}