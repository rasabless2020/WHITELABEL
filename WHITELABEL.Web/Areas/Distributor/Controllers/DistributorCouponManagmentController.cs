using log4net;
using NonFactors.Mvc.Grid;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Controllers;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;

namespace WHITELABEL.Web.Areas.Distributor.Controllers
{
    [Authorize]
    public class DistributorCouponManagmentController : DistributorBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        public void initpage()
        {
            try
            {
                if (DomainNameCheck.DomainChecking(Session["DOMAINNAME"].ToString(), Request.Url.Host) == false)
                {
                    Response.Redirect(Url.Action("DomainError", "Login", new { area = "" }));
                }
                ViewBag.ControllerName = "Distributor";
                if (Session["DistributorUserId"] == null)
                {
                    Response.Redirect(Url.Action("Logout", "Login", new { area = "" }));
                    return;
                }
                bool Islogin = false;

                if (Session["DistributorUserId"] != null)
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

        // GET: Distributor/DistributorCouponManagment
        public ActionResult Index()
        {
            if (Session["DistributorUserId"] != null)
            {
                initpage();
                return View();
            }
            else
            {
                Session["DistributorUserId"] = null;
                Session["DistributorUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("DistributorUserId");
                Session.Remove("DistributorUserName");
                Session.Remove("UserType");
                //return RedirectToAction("Index", "Login", new { area = "" });
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        public PartialViewResult IndexGrid()
        {
            initpage();
            try
            {
                var db = new DBContext();

                var Tramslist = (from Cpn in db.TBL_COUPON_TRANSFER_LOGS
                                 where Cpn.FROM_MEMBER == MemberCurrentUser.MEM_ID && Cpn.MEM_ROLE == 4
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
            if (Session["DistributorUserId"] != null)
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
                Session["DistributorUserId"] = null;
                Session["DistributorUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("DistributorUserId");
                Session.Remove("DistributorUserName");
                Session.Remove("UserType");
                //return RedirectToAction("Index", "Login", new { area = "" });
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
        public async Task<JsonResult> PostDistributorCouponStockRequisition(TBL_COUPON_TRANSFER_LOGS objReq)
        {
            initpage();
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var getSuper = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == MemberCurrentUser.MEM_ID);
                    long SuperId = 0;
                    long.TryParse(getSuper.INTRODUCER.ToString(), out SuperId);
                    var GetCouponType = db.TBL_COUPON_MASTER.FirstOrDefault(x => x.couponType == objReq.COUPON_Name);
                    string GetRefNo = GetUniqueKey(MemberCurrentUser.MEM_ID.ToString());
                    TBL_COUPON_TRANSFER_LOGS objCoupon = new TBL_COUPON_TRANSFER_LOGS()
                    {
                        REFERENCE_NO = GetRefNo,
                        TO_MEMBER = SuperId,
                        FROM_MEMBER = MemberCurrentUser.MEM_ID,
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
                        MEM_ROLE = 4
                    };
                    db.TBL_COUPON_TRANSFER_LOGS.Add(objCoupon);
                    db.SaveChanges();
                    ContextTransaction.Commit();
                    return Json("Requisition send to super", JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    throw ex;
                }

            }
        }



        public ActionResult MerchantCouponRequest()
        {
            if (Session["DistributorUserId"] != null)
            {
                initpage();
                return View();
            }
            else
            {
                Session["DistributorUserId"] = null;
                Session["DistributorUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("DistributorUserId");
                Session.Remove("DistributorUserName");
                Session.Remove("UserType");
                //return RedirectToAction("Index", "Login", new { area = "" });
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        public PartialViewResult MerchantRequitionIndexGrid()
        {
            try
            {
                var db = new DBContext();
                initpage();
                var Tramslist = (from Cpn in db.TBL_COUPON_TRANSFER_LOGS
                                 join mem in db.TBL_MASTER_MEMBER on Cpn.FROM_MEMBER equals mem.MEM_ID
                                 join dist in db.TBL_MASTER_MEMBER on mem.INTRODUCER equals dist.MEM_ID
                                 where Cpn.TO_MEMBER == 0 && Cpn.MEM_ROLE == 5 && Cpn.STATUS == "Pending"
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
                return PartialView("MerchantRequitionIndexGrid", Tramslist);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult getCouponReqMerchantTransdata(string TransId = "")
        {
            initpage();////
            try
            {
                var db = new DBContext();
                long transid = long.Parse(TransId);
                //var listdetails = db.TBL_BALANCE_TRANSFER_LOGS.Where(x => x.SLN == transid).FirstOrDefault();
                var Tramslist = (from Cpn in db.TBL_COUPON_TRANSFER_LOGS
                                 where Cpn.STATUS == "Pending" && Cpn.MEM_ROLE == 5 && Cpn.SLN == transid
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
                                     CouponPrice = db.TBL_COUPON_COMMISSION_SLAB.Where(a => a.coupon_id == Cpn.COUPON_TYPE && a.Merchant_Role_Id == 5).Select(c => c.Merchant_Comm_Value).FirstOrDefault(),
                                     GstNo = db.TBL_MASTER_MEMBER.Where(a => a.MEM_ID == Cpn.FROM_MEMBER).Select(c => c.COMPANY_GST_NO).FirstOrDefault(),
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
                                     GSTNo = z.GstNo
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<JsonResult> ChangeTransactionStatus(string trandate = "", string TransationStatus = "", string slnval = "")
        public async Task<JsonResult> ApproveMerchantRequisitionStatus(string slnval = "", string CouponAmt = "0", string SlabAmt = "0", string GstVal = "", string TDSVal = "")
        {
            initpage();////
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var GSTTaxMode = db.TBL_TAX_MASTERS.Where(x => x.TAX_NAME == "GST").FirstOrDefault();
                    var TDSTaxMode = db.TBL_TAX_MASTERS.Where(x => x.TAX_NAME == "TDS").FirstOrDefault();
                    long sln = long.Parse(slnval);
                    decimal CouponAmt_Value = 0;
                    decimal SlabCouponAmt = 0;
                    decimal.TryParse(CouponAmt, out CouponAmt_Value);
                    decimal.TryParse(SlabAmt, out SlabCouponAmt);
                    decimal trans_bal = 0;
                    decimal availableamt = 0;
                    decimal addavilamt = 0;
                    decimal closingamt = 0;
                    decimal addclosingamt = 0;
                    int AdminAvailStk = 0;
                    int AdminRemStk = 0;
                    int SuperRemStk = 0;
                    int SuperAvailStk = 0;
                    decimal checkAmt = 0;
                    decimal GSTAMT = 0;
                    decimal GMT_GST_Val = 0;
                    decimal AddAmtWiithGST = 0;
                    decimal Tds_Amt_Val = 0;
                    decimal NetValue = 0;
                    decimal TDS_Amt = 0;
                    string COrelationID = Settings.GetUniqueKey(MemberCurrentUser.MEM_ID.ToString());
                    var transinfo = await db.TBL_COUPON_TRANSFER_LOGS.Where(x => x.SLN == sln).FirstOrDefaultAsync();
                    if (transinfo != null)
                    {
                        if (GstVal == "Yes" && TDSVal == "Yes")
                        {
                            Tds_Amt_Val = TDSTaxMode.TAX_VALUE;
                            GMT_GST_Val = GSTTaxMode.TAX_VALUE;
                            checkAmt = (transinfo.QTY * CouponAmt_Value);
                            GSTAMT = (checkAmt * GMT_GST_Val) / 100;
                            AddAmtWiithGST = checkAmt + GSTAMT;
                            TDS_Amt = (AddAmtWiithGST * Tds_Amt_Val) / 100;
                            NetValue = (AddAmtWiithGST - TDS_Amt);
                        }
                        else if (GstVal == "Yes" && TDSVal == "No")
                        {
                            Tds_Amt_Val = TDSTaxMode.TAX_VALUE;
                            GMT_GST_Val = GSTTaxMode.TAX_VALUE;
                            checkAmt = (transinfo.QTY * CouponAmt_Value);
                            GSTAMT = (checkAmt * GMT_GST_Val) / 100;
                            AddAmtWiithGST = checkAmt + GSTAMT;
                            NetValue = (AddAmtWiithGST);
                        }
                        else
                        {
                            Tds_Amt_Val = TDSTaxMode.TAX_VALUE;
                            checkAmt = (transinfo.QTY * CouponAmt_Value);
                            AddAmtWiithGST = checkAmt;
                            //TDS_Amt = (AddAmtWiithGST * Tds_Amt_Val) / 100;
                            TDS_Amt = 0;
                            NetValue = (AddAmtWiithGST);
                        }
                        ////checkAmt = (transinfo.QTY * CouponAmt_Value);
                        //if (GstVal == "Yes")
                        //{
                        //    Tds_Amt_Val = TDSTaxMode.TAX_VALUE;
                        //    GMT_GST_Val = GSTTaxMode.TAX_VALUE;
                        //    checkAmt = (transinfo.QTY * CouponAmt_Value);
                        //    GSTAMT = (checkAmt * GMT_GST_Val) / 100;
                        //    AddAmtWiithGST = checkAmt + GSTAMT;
                        //    TDS_Amt = (AddAmtWiithGST * Tds_Amt_Val) / 100;
                        //    NetValue = (AddAmtWiithGST - TDS_Amt);
                        //}
                        //else
                        //{
                        //    Tds_Amt_Val = TDSTaxMode.TAX_VALUE;
                        //    checkAmt = (transinfo.QTY * CouponAmt_Value);
                        //    AddAmtWiithGST = checkAmt;
                        //    TDS_Amt = (AddAmtWiithGST * Tds_Amt_Val) / 100;
                        //    NetValue = (AddAmtWiithGST - TDS_Amt);
                        //}
                        var Tomember = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == MemberCurrentUser.MEM_ID);
                        var FromAmount = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == transinfo.FROM_MEMBER);
                        if (checkAmt <= FromAmount.BALANCE)
                        {
                            var getCouponSlab = db.TBL_COUPON_COMMISSION_SLAB.Where(x => x.coupon_id == transinfo.COUPON_TYPE && x.Merchant_Role_Id == 5).FirstOrDefault();
                            var GetMember = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == MemberCurrentUser.MEM_ID);
                            var GetDist = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == GetMember.INTRODUCER);
                            var checkAvailAmtWL = db.TBL_COUPON_STOCK.Where(x => x.couponHoderID == transinfo.TO_MEMBER && x.couponType == transinfo.COUPON_TYPE).FirstOrDefault();
                            if (checkAvailAmtWL != null)
                            {
                                if (Convert.ToDecimal(transinfo.QTY) <= Convert.ToDecimal(checkAvailAmtWL.couponQty))
                                {
                                    transinfo.STATUS = "Success";
                                    transinfo.APPROVAL_DATE = DateTime.Now;
                                    transinfo.APPROVAL_TIME = DateTime.Now;
                                    transinfo.APPROVED_BY = GetDist.MEMBER_MOBILE;
                                    transinfo.COMM_SLAB_ID = getCouponSlab.sln;
                                    transinfo.SELL_VALUE_RATE = SlabCouponAmt;
                                    transinfo.GROSS_SELL_VALUE = AddAmtWiithGST;
                                    transinfo.NET_SELL_VALUE = NetValue;
                                    transinfo.IS_GST_APPLIED = GstVal;

                                    transinfo.GST_PERCENTAGE = GMT_GST_Val;
                                    transinfo.GST_VALUE = GSTAMT;
                                    transinfo.FROM_MEMBER_GST_NO = FromAmount.COMPANY_GST_NO;
                                    transinfo.TO_MEMBER_GST_NO = Tomember.COMPANY_GST_NO;
                                    transinfo.IS_TDS_APPLIED = TDSVal;
                                    transinfo.TDS_PERCENTAGE = Tds_Amt_Val;
                                    transinfo.TDS_VALUE = TDS_Amt;
                                    transinfo.TDS_SUBMIT_REF_NO = "";
                                    transinfo.GST_INPUT = GSTAMT;
                                    transinfo.GST_OUTPUT = GSTAMT;
                                    if (SlabCouponAmt == CouponAmt_Value)
                                    {
                                        transinfo.IS_OVERRIDE_SELL_VALUE = false;
                                    }
                                    else
                                    {
                                        transinfo.IS_OVERRIDE_SELL_VALUE = true;
                                    }
                                    db.Entry(transinfo).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();
                                    var Super_Stk = db.TBL_COUPON_STOCK.Where(x => x.couponHoderID == transinfo.FROM_MEMBER && x.couponType == transinfo.COUPON_TYPE).FirstOrDefault();
                                    if (Super_Stk == null)
                                    {
                                        TBL_COUPON_STOCK objsupstk = new TBL_COUPON_STOCK()
                                        {
                                            couponHoderID = transinfo.FROM_MEMBER,
                                            puchaseDate = DateTime.Now,
                                            stockEntryDate = DateTime.Now,
                                            couponType = transinfo.COUPON_TYPE,
                                            couponQty = transinfo.QTY,
                                            Status = transinfo.REFERENCE_NO,
                                            Vendor_Name = GetDist.MEMBER_MOBILE
                                        };
                                        db.TBL_COUPON_STOCK.Add(objsupstk);
                                        db.SaveChanges();
                                        //var AdminStockSub=db.TBL_COUPON_STOCK.Where(x=>x.couponHoderID==0 && x.couponType==transinfo.COUPON_TYPE)
                                        AdminAvailStk = checkAvailAmtWL.couponQty;
                                        AdminRemStk = AdminAvailStk - transinfo.QTY;
                                        checkAvailAmtWL.couponQty = AdminRemStk;
                                        db.Entry(checkAvailAmtWL).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();

                                        //Accounts  From Member
                                        decimal closingamt_val = 0;
                                        decimal Supclosing = 0;
                                        decimal opening = 0;
                                        decimal Bal_From = 0;
                                        trans_bal = NetValue;
                                        var amtobj = await db.TBL_ACCOUNTS.Where(x => x.MEM_ID == transinfo.FROM_MEMBER).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefaultAsync();
                                        if (amtobj != null)
                                        {
                                            closingamt_val = amtobj.CLOSING;
                                            opening = amtobj.CLOSING;
                                            Supclosing = amtobj.CLOSING - trans_bal;
                                        }
                                        else
                                        {
                                            opening = 0;
                                            closingamt_val = 0;
                                            Supclosing = trans_bal;
                                        }
                                        TBL_ACCOUNTS onjDistAcnt = new TBL_ACCOUNTS()
                                        {
                                            API_ID = 0,
                                            MEM_ID = transinfo.FROM_MEMBER,
                                            MEMBER_TYPE = "MERCHANT",
                                            TRANSACTION_TYPE = "WALLET",
                                            TRANSACTION_DATE = Convert.ToDateTime(transinfo.REQUEST_DATE),
                                            TRANSACTION_TIME = DateTime.Now,
                                            DR_CR = "DR",
                                            AMOUNT = trans_bal,
                                            NARRATION = "Coupon Buy from Distributor",
                                            OPENING = opening,
                                            CLOSING = Supclosing,
                                            REC_NO = 0,
                                            COMM_AMT = 0,
                                            GST = 0,
                                            TDS = 0,
                                            IPAddress = "",
                                            SERVICE_ID = 0,
                                            CORELATIONID = transinfo.REFERENCE_NO
                                        };
                                        db.TBL_ACCOUNTS.Add(onjDistAcnt);
                                        db.SaveChanges();
                                        decimal From_Memb_Amt = 0;
                                        decimal Rem_Bal = 0;
                                        var Member_Acnt = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == transinfo.FROM_MEMBER).FirstOrDefault();
                                        if (Member_Acnt.BALANCE == 0)
                                        {
                                            Member_Acnt.BALANCE = trans_bal;
                                        }
                                        else
                                        {
                                            From_Memb_Amt = Convert.ToDecimal(Member_Acnt.BALANCE);
                                            Rem_Bal = (From_Memb_Amt - trans_bal);
                                            Member_Acnt.BALANCE = Rem_Bal;
                                        }
                                        db.Entry(Member_Acnt).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                        //// to Member
                                        //decimal Toclosingamt_val = 0;
                                        //decimal ToSupclosing = 0;
                                        //decimal Toopening = 0;
                                        //decimal Bal_TO = 0;
                                        //var ToMemamtobj = await db.TBL_ACCOUNTS.Where(x => x.MEM_ID == transinfo.TO_MEMBER).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefaultAsync();
                                        //if (ToMemamtobj != null)
                                        //{
                                        //    Toclosingamt_val = ToMemamtobj.CLOSING;
                                        //    Toopening = ToMemamtobj.CLOSING;
                                        //    ToSupclosing = ToMemamtobj.CLOSING + trans_bal;
                                        //}
                                        //else
                                        //{
                                        //    Toopening = 0;
                                        //    Toclosingamt_val = 0;
                                        //    ToSupclosing = trans_bal;
                                        //}
                                        //TBL_ACCOUNTS onjSuperAcnt = new TBL_ACCOUNTS()
                                        //{
                                        //    API_ID = 0,
                                        //    MEM_ID = transinfo.FROM_MEMBER,
                                        //    MEMBER_TYPE = "DISTRIBUTOR",
                                        //    TRANSACTION_TYPE = "CASH",
                                        //    TRANSACTION_DATE = Convert.ToDateTime(transinfo.REQUEST_DATE),
                                        //    TRANSACTION_TIME = DateTime.Now,
                                        //    DR_CR = "CR",
                                        //    AMOUNT = trans_bal,
                                        //    NARRATION = "Coupon Buy from Admin",
                                        //    OPENING = Toopening,
                                        //    CLOSING = ToSupclosing,
                                        //    REC_NO = 0,
                                        //    COMM_AMT = 0,
                                        //    GST = 0,
                                        //    TDS = 0,
                                        //    IPAddress = "",
                                        //    SERVICE_ID = 0,
                                        //    CORELATIONID = transinfo.REFERENCE_NO
                                        //};
                                        //db.TBL_ACCOUNTS.Add(onjSuperAcnt);
                                        //db.SaveChanges();
                                        //decimal From_Avai_amt = 0;
                                        //decimal Rem_Bal_val = 0;
                                        //var SuperMember_Acnt = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == transinfo.TO_MEMBER).FirstOrDefault();
                                        //if (Member_Acnt.BALANCE == 0)
                                        //{
                                        //    SuperMember_Acnt.BALANCE = trans_bal;
                                        //}
                                        //else
                                        //{
                                        //    //Bal_TO = Convert.ToDecimal(Convert.ToDecimal(SuperMember_Acnt.BALANCE) + trans_bal);

                                        //    decimal.TryParse(Member_Acnt.BALANCE.ToString(), out From_Avai_amt);
                                        //    Rem_Bal_val = ((From_Avai_amt) - (trans_bal));
                                        //    SuperMember_Acnt.BALANCE = Rem_Bal_val;
                                        //}
                                        //db.Entry(SuperMember_Acnt).State = System.Data.Entity.EntityState.Modified;
                                        //db.SaveChanges();
                                        ContextTransaction.Commit();
                                        return Json("Requisition Approved.", JsonRequestBehavior.AllowGet);
                                    }
                                    else
                                    {
                                        //Admin
                                        AdminAvailStk = checkAvailAmtWL.couponQty;
                                        AdminRemStk = AdminAvailStk - transinfo.QTY;
                                        checkAvailAmtWL.couponQty = AdminRemStk;
                                        db.Entry(checkAvailAmtWL).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                        //Super
                                        SuperAvailStk = Super_Stk.couponQty;
                                        SuperRemStk = SuperAvailStk + transinfo.QTY;
                                        Super_Stk.couponQty = SuperRemStk;
                                        db.Entry(Super_Stk).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                        //Accounts  From Member
                                        decimal closingamt_val = 0;
                                        decimal Supclosing = 0;
                                        decimal opening = 0;
                                        decimal Bal_From = 0;
                                        decimal Bal_To = 0;
                                        trans_bal = NetValue;
                                        var amtobj = await db.TBL_ACCOUNTS.Where(x => x.MEM_ID == transinfo.FROM_MEMBER).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefaultAsync();
                                        if (amtobj != null)
                                        {
                                            closingamt_val = amtobj.CLOSING;
                                            opening = amtobj.CLOSING;
                                            Supclosing = amtobj.CLOSING - trans_bal;
                                        }
                                        else
                                        {
                                            opening = 0;
                                            closingamt_val = 0;
                                            Supclosing = trans_bal;
                                        }
                                        TBL_ACCOUNTS onjDistAcnt = new TBL_ACCOUNTS()
                                        {
                                            API_ID = 0,
                                            MEM_ID = transinfo.FROM_MEMBER,
                                            MEMBER_TYPE = "MERCHANT",
                                            TRANSACTION_TYPE = "WALLET",
                                            TRANSACTION_DATE = Convert.ToDateTime(transinfo.REQUEST_DATE),
                                            TRANSACTION_TIME = DateTime.Now,
                                            DR_CR = "DR",
                                            AMOUNT = trans_bal,
                                            NARRATION = "Coupon Buy from Distributor",
                                            OPENING = opening,
                                            CLOSING = Supclosing,
                                            REC_NO = 0,
                                            COMM_AMT = 0,
                                            GST = 0,
                                            TDS = 0,
                                            IPAddress = "",
                                            SERVICE_ID = 0,
                                            CORELATIONID = transinfo.REFERENCE_NO
                                        };
                                        db.TBL_ACCOUNTS.Add(onjDistAcnt);
                                        db.SaveChanges();
                                        decimal From_Avai_amt = 0;
                                        decimal Rem_Bal = 0;
                                        var Member_Acnt = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == transinfo.FROM_MEMBER).FirstOrDefault();
                                        if (Member_Acnt.BALANCE == 0)
                                        {
                                            Member_Acnt.BALANCE = trans_bal;
                                        }
                                        else
                                        {
                                            //From_Avai_amt = Convert.ToDecimal(Member_Acnt.BALANCE);
                                            decimal.TryParse(Member_Acnt.BALANCE.ToString(), out From_Avai_amt);
                                            Rem_Bal = ((From_Avai_amt) - (trans_bal));
                                            Member_Acnt.BALANCE = Rem_Bal;
                                        }
                                        db.Entry(Member_Acnt).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                        //// to Member
                                        //decimal Toclosingamt_val = 0;
                                        //decimal ToSupclosing = 0;
                                        //decimal Toopening = 0;
                                        //var ToMemamtobj = await db.TBL_ACCOUNTS.Where(x => x.MEM_ID == transinfo.TO_MEMBER).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefaultAsync();
                                        //if (ToMemamtobj != null)
                                        //{
                                        //    Toclosingamt_val = ToMemamtobj.CLOSING;
                                        //    Toopening = ToMemamtobj.CLOSING;
                                        //    ToSupclosing = ToMemamtobj.CLOSING + trans_bal;
                                        //}
                                        //else
                                        //{
                                        //    Toopening = 0;
                                        //    Toclosingamt_val = 0;
                                        //    ToSupclosing = trans_bal;
                                        //}
                                        //TBL_ACCOUNTS onjSuperAcnt = new TBL_ACCOUNTS()
                                        //{
                                        //    API_ID = 0,
                                        //    MEM_ID = transinfo.FROM_MEMBER,
                                        //    MEMBER_TYPE = "DISTRIBUTOR",
                                        //    TRANSACTION_TYPE = "CASH",
                                        //    TRANSACTION_DATE = Convert.ToDateTime(transinfo.REQUEST_DATE),
                                        //    TRANSACTION_TIME = DateTime.Now,
                                        //    DR_CR = "CR",
                                        //    AMOUNT = trans_bal,
                                        //    NARRATION = "Coupon Sell To Merchant",
                                        //    OPENING = Toopening,
                                        //    CLOSING = ToSupclosing,
                                        //    REC_NO = 0,
                                        //    COMM_AMT = 0,
                                        //    GST = 0,
                                        //    TDS = 0,
                                        //    IPAddress = "",
                                        //    SERVICE_ID = 0,
                                        //    CORELATIONID = transinfo.REFERENCE_NO
                                        //};
                                        //db.TBL_ACCOUNTS.Add(onjSuperAcnt);
                                        //db.SaveChanges();
                                        //var SuperMember_Acnt = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == transinfo.TO_MEMBER).FirstOrDefault();
                                        //if (Member_Acnt.BALANCE == 0)
                                        //{
                                        //    SuperMember_Acnt.BALANCE = trans_bal;
                                        //}
                                        //else
                                        //{
                                        //    Bal_To= Convert.ToDecimal(Convert.ToDecimal(SuperMember_Acnt.BALANCE) + trans_bal);
                                        //    SuperMember_Acnt.BALANCE = Bal_To;
                                        //}
                                        //db.Entry(SuperMember_Acnt).State = System.Data.Entity.EntityState.Modified;
                                        //db.SaveChanges();
                                        ContextTransaction.Commit();
                                        return Json("Requisition Approved.", JsonRequestBehavior.AllowGet);
                                    }
                                }
                                else
                                {
                                    return Json("You don't have sufficient coupon.", JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                return Json("You don't have sufficient coupon.", JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            string msgval = FromAmount.MEMBER_NAME + "don't have sufficient balance";
                            return Json(msgval, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json("Please try after some time.", JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    Logger.Error("Controller:-  MemberRequisition(Admin), method:- ChangeTransactionStatus (POST) Line No:- 599", ex);
                    return Json(new { Result = "false" });
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<JsonResult> TransactionDecline(string trandate = "", string TransationStatus = "", string slnval = "")
        public async Task<JsonResult> RequisitionMerchantDecline(string slnval = "")
        {
            initpage();////
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    //var db = new DBContext();
                    long sln = long.Parse(slnval);
                    var transinfo = await db.TBL_COUPON_TRANSFER_LOGS.Where(x => x.SLN == sln).FirstOrDefaultAsync();
                    //transinfo.REQUEST_DATE = Convert.ToDateTime(trandate);
                    transinfo.STATUS = "Decline";
                    transinfo.APPROVED_BY = "test";
                    transinfo.APPROVAL_DATE = DateTime.Now;
                    transinfo.APPROVAL_TIME = DateTime.Now;
                    transinfo.COMM_SLAB_ID = 0;

                    transinfo.SELL_VALUE_RATE = 0;
                    transinfo.SELL_VALUE_RATE = 0;
                    transinfo.GROSS_SELL_VALUE = 0;
                    transinfo.NET_SELL_VALUE = 0;

                    transinfo.MEM_ROLE = 4;
                    transinfo.GST_PERCENTAGE = 0;
                    transinfo.GST_VALUE = 0;

                    transinfo.IS_TDS_APPLIED = "No";
                    transinfo.TDS_PERCENTAGE = 0;
                    transinfo.TDS_VALUE = 0;
                    transinfo.TDS_SUBMIT_REF_NO = "";
                    transinfo.GST_INPUT = 0;
                    transinfo.GST_OUTPUT = 0;
                    transinfo.IS_OVERRIDE_SELL_VALUE = false;
                    db.Entry(transinfo).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();
                    //EmailHelper emailhelper = new EmailHelper();
                    //var memberlist = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == transinfo.FROM_MEMBER).FirstOrDefaultAsync();
                    //string mailbody = "Hi " + memberlist.UName + ",<p>Your requisition has been declined.</p>";
                    //emailhelper.SendUserEmail(memberlist.EMAIL_ID, "Requisition Decline", mailbody);
                    ContextTransaction.Commit();
                    return Json("Requisition Decline", JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Logger.Error("Controller:-  MemberRequisition(Admin), method:- TransactionDecline (POST) Line No:- 631", ex);
                    ContextTransaction.Rollback();
                    return Json(new { Result = "false " });
                }
            }


        }

        public ActionResult DistributorStockReport()
        {
            if (Session["DistributorUserId"] != null)
            {
                initpage();
                return View();
            }
            else
            {
                Session["DistributorUserId"] = null;
                Session["DistributorUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("DistributorUserId");
                Session.Remove("DistributorUserName");
                Session.Remove("UserType");
                //return RedirectToAction("Index", "Login", new { area = "" });
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        public PartialViewResult GetDistributorCouponStockIndexGrid()
        {
            try
            {
                initpage();
                var db = new DBContext();
                var Tramslist = (from Cpn in db.TBL_COUPON_STOCK
                                 where Cpn.couponHoderID == MemberCurrentUser.MEM_ID
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
                return PartialView("GetDistributorCouponStockIndexGrid", Tramslist);
                //return PartialView(CreateExportableGrid());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult DistributorRequisitionReport()
        {
            if (Session["DistributorUserId"] != null)
            {
                initpage();
                return View();
            }
            else
            {
                Session["DistributorUserId"] = null;
                Session["DistributorUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("DistributorUserId");
                Session.Remove("DistributorUserName");
                Session.Remove("UserType");
                //return RedirectToAction("Index", "Login", new { area = "" });
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        public PartialViewResult DistributorRequisitionReportIndexGrid()
        {
            initpage();
            try
            {
                var db = new DBContext();

                var Tramslist = (from Cpn in db.TBL_COUPON_TRANSFER_LOGS
                                 where Cpn.MEM_ROLE == 4 && Cpn.FROM_MEMBER == MemberCurrentUser.MEM_ID
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
                return PartialView("DistributorRequisitionReportIndexGrid", Tramslist);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ActionResult MerchantRequisitionCouponReport()
        {
            if (Session["DistributorUserId"] != null)
            {
                initpage();
                return View();
            }
            else
            {
                Session["DistributorUserId"] = null;
                Session["DistributorUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("DistributorUserId");
                Session.Remove("DistributorUserName");
                Session.Remove("UserType");
                //return RedirectToAction("Index", "Login", new { area = "" });
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
                                 where Cpn.MEM_ROLE == 5 && Cpn.TO_MEMBER == MemberCurrentUser.MEM_ID
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
        public JsonResult PrintDistributorcouponInvoice(string TransId = "")
        {
            initpage();////
            try
            {
                var db = new DBContext();
                long transid = long.Parse(TransId);
                var Tramslist = (from Cpn in db.TBL_COUPON_TRANSFER_LOGS
                                 join mem in db.TBL_MASTER_MEMBER on Cpn.FROM_MEMBER equals mem.MEM_ID
                                 where Cpn.STATUS == "Success" && Cpn.MEM_ROLE == 4 && Cpn.SLN == transid
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