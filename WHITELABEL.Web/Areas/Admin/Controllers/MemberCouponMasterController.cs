using log4net;
using NonFactors.Mvc.Grid;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Areas.Admin.Models;
using WHITELABEL.Web.Controllers;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;
using static WHITELABEL.Web.Helper.CyberPlateAPIHelper;
using CyberPlatOpenSSL;
using System.Security.Cryptography.X509Certificates;

namespace WHITELABEL.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class MemberCouponMasterController : AdminBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        public static string CYBER_Password = System.Configuration.ConfigurationManager.AppSettings["CyberPlateCERTPASSWORD"];
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
                    //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
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
                //ViewBag.UserName = CurrentUser.UserId;
                Console.WriteLine(e.InnerException);
                return;
            }
        }

        // GET: Admin/Merchant_Coupan_Master
        public ActionResult Index()
        {
            string URL = "https://in.cyberplat.com/cgi-bin/at/at_pay_check.cgi";            
            //OpenSSL ssl = new OpenSSL();
            //string ValidateRequest = CyberPlateAPI._strValidationRequestLogs("", "", "", "");
            //string keyPath = Server.MapPath("Test CERT\\myprivatekey.pfx");
            //ssl.message = ssl.Sign_With_PFX(ValidateRequest, keyPath, CYBER_Password);
            //ssl.htmlText = ssl.CallCryptoAPI(ssl.message, URL);

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
                var dbcontext = new DBContext();
                var memberinfo = dbcontext.TBL_COUPON_MASTER.ToList();
                return PartialView("IndexGrid", memberinfo);
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult AddNewCoupon(string CouponName,string couponAmt,string CouponId)
        {
            initpage();
            try
            {
                var db = new DBContext();
                if (CouponId == "")
                {                    
                    decimal CouponAmt_Val = decimal.Parse(couponAmt);
                    //db.Database.ExecuteSqlCommand("TRUNCATE TABLE DMT_BANK_MARGIN");
                    TBL_COUPON_MASTER objCoupon = new TBL_COUPON_MASTER()
                    {
                        couponType = CouponName,
                        vendor_coupon_price = CouponAmt_Val,
                        
                    };
                    db.TBL_COUPON_MASTER.Add(objCoupon);
                    db.SaveChanges();
                    return Json("true", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    long cpnid = 0;
                    long.TryParse(CouponId, out cpnid);
                    decimal CouponAmt_Val = decimal.Parse(couponAmt);
                    var GETCoupon = db.TBL_COUPON_MASTER.Where(x => x.sln == cpnid).FirstOrDefault();
                    GETCoupon.vendor_coupon_price = CouponAmt_Val;
                    db.Entry(GETCoupon).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json("true", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }
        [HttpPost]
        public JsonResult GetCouponInfo(string TransId)
        {
            try
            {
                long valueid = long.Parse(TransId);
                var db = new DBContext();
                var BankMarginvaluebind = db.TBL_COUPON_MASTER.Where(x => x.sln == valueid).FirstOrDefault();
                return Json(BankMarginvaluebind, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public ActionResult CouponStockMaster()
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
        public PartialViewResult StockGrid()
        {
            initpage();
            try
            {
                var dbcontext = new DBContext();
                //var memberinfo = dbcontext.TBL_MASTER_MEMBER.Where(x=>x.IS_DELETED==false).ToList();
                //// Only grid query values will be available here.
                ////return PartialView("IndexGrid", memberinfo);
                ///

                var memberinfo = (from x in dbcontext.TBL_COUPON_STOCK   join y in dbcontext.TBL_COUPON_MASTER on x.couponType equals y.sln                                                                          
                                       select new
                                       {
                                           HolderName = "Admin",
                                           puchaseDate = x.puchaseDate,
                                           stockEntryDate = x.stockEntryDate,
                                           Status = x.Status,
                                           Sln = x.sln,
                                           Qty=x.couponQty,
                                           CouponName = y.couponType,
                                           CouponAmt= y.vendor_coupon_price,
                                       }).AsEnumerable().Select(z => new TBL_COUPON_STOCK
                                       {
                                           sln = z.Sln,
                                           CouponName = z.CouponName,
                                           puchaseDate = z.puchaseDate,
                                           stockEntryDate = z.stockEntryDate,
                                           CouponAmount = z.CouponAmt,
                                           couponQty=z.Qty,
                                           Status = z.Status,
                                           HolderName = z.HolderName
                                           
                                       }).ToList().OrderByDescending(x => x.sln);



                //var memberinfo = dbcontext.TBL_COUPON_STOCK.ToList();
                return PartialView("StockGrid", memberinfo);
                //return PartialView(CreateExportableGrid());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult AddCouponStock(string memid ="")
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                initpage();
                var db = new DBContext();
                using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        long MEm_Id = 0;
                        string decrptSlId = Decrypt.DecryptMe(memid);
                        long.TryParse(decrptSlId, out MEm_Id);
                        var CouponList = db.TBL_COUPON_MASTER.ToList();
                        ViewBag.CouponList = new SelectList(CouponList, "sln", "couponType");
                        var VendorName = db.TBL_VENDOR_MASTER.ToList();
                        ViewBag.VendorName = new SelectList(VendorName, "ID", "VENDOR_NAME");
                        if (memid == "")
                        {
                            ViewBag.btnStatus = "0";
                            return View();
                        }
                        else
                        {
                            ViewBag.btnStatus = "1";
                            var model = new TBL_COUPON_STOCK();
                            model = db.TBL_COUPON_STOCK.Where(x => x.sln == MEm_Id).FirstOrDefault();
                            return View(model);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
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
        public static string GetUniqueKey(string userID)
        {
            string resultString = Regex.Match(userID, @"\d+").Value;

            long ticks = DateTime.Now.Ticks;
            string result = resultString + ticks.ToString();
            return result.ToString();
        }
        [HttpPost]
        public async Task<JsonResult> PostAddCouponStock(TBL_COUPON_STOCK objstk)
        {
            initpage();
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var couponMaster = db.TBL_COUPON_MASTER.Where(x => x.sln == objstk.couponType).FirstOrDefault();
                    if (objstk.sln == 0)
                    {
                        int StkQty = 0;
                        int AddStkQty = 0;
                        string GetRefNo = GetUniqueKey(MemberCurrentUser.MEM_ID.ToString());
                        var checkStock = db.TBL_COUPON_STOCK.Where(x => x.couponHoderID == 0 && x.couponType==objstk.couponType).FirstOrDefault();
                        if (checkStock == null)
                        {
                            objstk.couponHoderID = 0;
                            objstk.Status = GetRefNo;
                            //objstk.Vendor_Name = "UTIITSL";
                            db.TBL_COUPON_STOCK.Add(objstk);
                            db.SaveChanges();
                        }
                        else
                        {
                            StkQty = checkStock.couponQty;
                            AddStkQty = StkQty + objstk.couponQty;
                            checkStock.couponQty = AddStkQty;
                            checkStock.stockEntryDate = objstk.stockEntryDate;
                            checkStock.Vendor_Name = objstk.Vendor_Name;
                            db.Entry(checkStock).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                        TBL_COUPON_TRANSFER_LOGS objTrans = new TBL_COUPON_TRANSFER_LOGS()
                        {
                            REFERENCE_NO= GetRefNo,
                            TO_MEMBER=0,
                            FROM_MEMBER=0,
                            REQUEST_DATE=objstk.stockEntryDate,
                            REQUEST_TIME=DateTime.Now,
                            QTY=objstk.couponQty,
                            COUPON_TYPE=objstk.couponType,
                            STATUS= "Success",
                            APPROVED_BY="Admin",
                            APPROVAL_DATE=DateTime.Now,
                            APPROVAL_TIME=DateTime.Now,
                            REMARKS="Coupon Add By Admin",
                            COMM_SLAB_ID=0,
                            
                            SELL_VALUE_RATE = 0,
                            GROSS_SELL_VALUE = 0,
                            NET_SELL_VALUE = 0,
                            GST_PERCENTAGE = 0,
                            GST_VALUE = 0,
                            TDS_PERCENTAGE = 0,
                            TDS_VALUE = 0,
                            GST_INPUT = 0,
                            GST_OUTPUT = 0,
                            IS_OVERRIDE_SELL_VALUE =false,
                            MEM_ROLE=1
                        };
                        db.TBL_COUPON_TRANSFER_LOGS.Add(objTrans);
                        db.SaveChanges();
                        ContextTransaction.Commit();
                        return Json("Coupon Stock Added Successfully", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var Couponedit = db.TBL_COUPON_STOCK.FirstOrDefault(x => x.sln == objstk.sln);
                        Couponedit.puchaseDate = objstk.puchaseDate;
                        Couponedit.stockEntryDate = objstk.stockEntryDate;                        
                        Couponedit.couponQty = objstk.couponQty;
                        db.Entry(Couponedit).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        var update_Trans = db.TBL_COUPON_TRANSFER_LOGS.Where(x => x.REFERENCE_NO == Couponedit.Status).FirstOrDefault();
                        update_Trans.QTY = objstk.couponQty;
                        update_Trans.REQUEST_DATE = objstk.stockEntryDate;
                        db.Entry(update_Trans).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        ContextTransaction.Commit();
                        return Json(" Coupon Stock updated", JsonRequestBehavior.AllowGet);
                    }

                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    throw ex;
                }
            }

              


           
        }


        public ActionResult CouponCommissionMapping(string coupid = "")
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                initpage();
                try
                {
                    if (coupid != "")
                    {
                        string decriptval = Decrypt.DecryptMe(coupid.ToString());
                        long SlabIdVal = long.Parse(decriptval);
                        Session["IDval"] = SlabIdVal;
                        var db = new DBContext();
                        //return View(listSlab);
                        return View();
                    }
                    else
                    {
                        Session["IDval"] = null;
                        Session["IDval"] = null;
                        Session.Remove("IDval");
                        Session.Remove("IDval");
                        Session["IDval"] = null;
                        Session["IDval"] = "";
                        Session.Remove("IDval");
                        //Session.Abandon();
                        CommissoinManagmentmodel objmodel = new CommissoinManagmentmodel();
                        objmodel.OperatorDetails = null;
                        ViewBag.checkStatus = "0";
                        return View();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("Controller:-  MemberCommissionSlab(Admin), method:- GenerateCommissionSlab (GET) Line No:- 1131", ex);
                    return RedirectToAction("Exception", "ErrorHandler", new { area = "" });
                    throw ex;
                }

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
        [HttpPost]
        public async Task<JsonResult> CouponAutoComplete(string prefix)
        {
            initpage();////
            try
            {
                var dbcontext = new DBContext();
                //var memberinfo = (from x in dbcontext.TBL_COUPON_MASTER
                //                  select new
                //                  {
                //                      Sln = x.sln,
                //                      CouponType = x.couponType,
                //                      CouponAmt = x.vendor_coupon_price,

                //                  }).AsEnumerable().Select(z => new CouponCommission
                //                  {
                //                      sln = z.Sln,
                //                      Coupon_Name = z.CouponType,
                //                      Comm_Value = z.CouponAmt,
                //                      Comm_TYPE = "FIXED",
                //                      Super_Comm_Value = 0,
                //                      Dist_Comm_value = 0,
                //                      Merchant_Comm_Value = 0
                //                  }).ToList();


                //var db = new DBContext();
                var OperatorValue = await (from oper in dbcontext.TBL_COUPON_MASTER
                                           where oper.couponType.StartsWith(prefix)
                                           select new
                                           {
                                               //label = oper.SERVICE_NAME + "-" + oper.RECHTYPE,
                                               label = oper.couponType,
                                               val = oper.sln
                                           }).ToListAsync();
                return Json(OperatorValue);
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MemberCommissionSlab(Admin), method:- AutoComplete (GET) Line No:- 424", ex);
                throw ex;
            }

            //var db = new DBContext();
            //var OperatorValue = (from oper in db.TBL_OPERATOR_MASTER
            //                 where oper.OPERATORNAME.StartsWith(prefix) && oper.OPERATORTYPE== OperatorType
            //                     select new
            //                {
            //                    label = oper.OPERATORNAME +"-"+ oper.RECHTYPE,
            //                    val = oper.PRODUCTID
            //               }).ToList();


        }

        [HttpPost]
        public JsonResult GetCouponCommProvider(string NewListId)
        {
            initpage();////
            try
            {
                var db = new DBContext();
                //var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE.Contains(NewListId));
                //return Json("");

                string operatortype = string.Empty;
                string operatortype1 = string.Empty;
                string DTH = string.Empty;
                string OperatorTypeName = string.Empty;
                string DTHOperator = string.Empty;

                var OperatorList = (from x in db.TBL_COUPON_MASTER where x.couponType== NewListId
                                    select new
                                  {
                                      Sln = x.sln,
                                      CouponType = x.couponType,
                                      CouponAmt = x.vendor_coupon_price,

                                  }).AsEnumerable().Select(z => new CouponCommission
                                  {
                                      coupon_id = z.Sln,
                                      Coupon_Name = z.CouponType,
                                      Comm_Value = z.CouponAmt,
                                      Comm_TYPE = "FIXED",
                                      Super_Comm_Value = 0,
                                      Dist_Comm_value = 0,
                                      Merchant_Comm_Value = 0
                                  }).ToList();

                return Json(OperatorList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MemberCommissionSlab(Admin), method:- GetServiceProvider (POST) Line No:- 1024", ex);
                throw ex;
            }
        }

        public ActionResult CouponCommissionList()
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
        public PartialViewResult CouponCommIndexGrid()
        {
            initpage();
            try
            {
                var dbcontext = new DBContext();
                var memberinfo = dbcontext.TBL_COUPON_COMMISSION_SLAB.ToList();
                return PartialView("CouponCommIndexGrid", memberinfo);
                //return PartialView(CreateExportableGrid());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult FetchCouponSlabData(long idval)
        {
            initpage();////
            try
            {
                var db = new DBContext();
                if (Session["IDval"] != null && Session["IDval"] != "")
                {
                    idval = long.Parse(Session["IDval"].ToString());
                }
                else
                {
                    idval = 0;
                }

                var valuelist = (from commslb in db.TBL_COUPON_COMMISSION_SLAB where commslb.sln == idval
                                 select new
                                 {
                                     sln = commslb.sln,
                                     slab_name = commslb.CouponSlab_Name,
                                     slab_details = commslb.CouponDetails,
                                     Coupon_Name = commslb.Coupon_Name,
                                     couponstatus = commslb.Coupon_Status,
                                     OperatorDetails = (from childcommslb in db.TBL_COUPON_COMMISSION_SLAB where childcommslb.sln == idval
                                                        select new
                                                        {
                                                            sln = childcommslb.sln,
                                                            coupon_id = childcommslb.coupon_id,
                                                            coupon_n = childcommslb.Coupon_Name,
                                                            Comm_Type = childcommslb.Comm_TYPE,
                                                            Comm_Value = childcommslb.Comm_Value,
                                                            super_Comm_val = childcommslb.Super_Comm_Value,
                                                            distComm_Val = childcommslb.Dist_Comm_value,
                                                            Merc_comm_val = childcommslb.Merchant_Comm_Value,
                                                        }).AsEnumerable().Select(z => new CouponCommissionListView
                                                        {
                                                            Sln = z.sln,
                                                            coupon_id = z.coupon_id,
                                                            Coupon_Name = z.coupon_n,
                                                            Comm_TYPE = z.Comm_Type,
                                                            Comm_Value = z.Comm_Value.ToString(),
                                                         Super_Comm_Value = z.super_Comm_val.ToString(),
                                                         Dist_Comm_value = z.distComm_Val.ToString(),
                                                         Merchant_Comm_Value = z.Merc_comm_val.ToString()
                                                     }).ToList()
                                }).AsEnumerable().Select(d => new CouponCommissoinManagmentmodel
                                {
                                    SLN = d.sln,
                                    SLAB_NAME = d.slab_name,
                                    SLAB_DETAILS = d.slab_details,
                                    Slab_TypeName =d.Coupon_Name,
                                    SLAB_STATUS = d.couponstatus,
                                    //ASSIGNED_SLAB = d.ASSIGNED_SLAB,
                                    OperatorDetails = d.OperatorDetails.ToList()
                                });
                return Json(valuelist, JsonRequestBehavior.AllowGet);
                //string slabtypename = string.Empty;
                //var slabtype = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == idval).FirstOrDefault();
                //if (slabtype != null)
                //{
                //    if (slabtype.SLAB_TYPE == 1)
                //    {
                //        slabtypename = "MOBILE RECHARGE";
                //        var valuelist = (from slabmaster in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                //                         where slabmaster.SLN == idval
                //                         select new
                //                         {
                //                             SLN = slabmaster.SLN,
                //                             SLAB_NAME = slabmaster.SLAB_NAME,
                //                             SLAB_DETAILS = slabmaster.SLAB_DETAILS,
                //                             SLAB_TYPE = slabmaster.SLAB_TYPE,
                //                             SLAB_STATUS = slabmaster.SLAB_STATUS,
                //                             //ASSIGNED_SLAB=slabmaster.ASSIGNED_SLAB,
                //                             OperatorDetails = (from slablist in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                //                                                where slablist.SLAB_ID == idval
                //                                                select new
                //                                                {
                //                                                    ID = slablist.SLN,
                //                                                    SERVICE_NAME = slablist.OPERATOR_NAME,
                //                                                    TYPE = slablist.OPERATOR_TYPE,
                //                                                    SERVICE_KEY = slablist.OPERATOR_CODE,
                //                                                    CommissionType = slablist.COMMISSION_TYPE,
                //                                                    CommissionPercentage = slablist.COMM_PERCENTAGE,
                //                                                    SuperDisPercentage = slablist.SUPER_COM_PER,
                //                                                    DistriCommissionPer = slablist.DISTRIBUTOR_COM_PER,
                //                                                    RetailerCommissionPer = slablist.MERCHANT_COM_PER,
                //                                                }).AsEnumerable().Select(z => new CommissionListView
                //                                                {
                //                                                    ID = z.ID,
                //                                                    SERVICE_NAME = z.SERVICE_NAME,
                //                                                    TYPE = z.TYPE,
                //                                                    SERVICE_KEY = z.SERVICE_KEY,
                //                                                    CommissionType = z.CommissionType,
                //                                                    CommissionPercentage = z.CommissionPercentage.ToString(),
                //                                                    SuperDisPercentage = z.SuperDisPercentage.ToString(),
                //                                                    DistriCommissionPer = z.DistriCommissionPer.ToString(),
                //                                                    RetailerCommissionPer = z.RetailerCommissionPer.ToString()
                //                                                }).ToList()
                //                         }).AsEnumerable().Select(d => new CommissoinManagmentmodel
                //                         {
                //                             SLN = d.SLN,
                //                             SLAB_NAME = d.SLAB_NAME,
                //                             SLAB_DETAILS = d.SLAB_DETAILS,
                //                             Slab_TypeName = slabtypename,
                //                             SLAB_STATUS = d.SLAB_STATUS,
                //                             SLAB_TYPE = d.SLAB_TYPE,
                //                             //ASSIGNED_SLAB = d.ASSIGNED_SLAB,
                //                             OperatorDetails = d.OperatorDetails.OrderByDescending(c => c.TYPE).ToList()
                //                         });
                //        return Json(valuelist, JsonRequestBehavior.AllowGet);
                //    }
                //    else if (slabtype.SLAB_TYPE == 2)
                //    {
                //        slabtypename = "UTILITY SERVICES";
                //        var valuelist = (from slabmaster in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                //                         where slabmaster.SLN == idval
                //                         select new
                //                         {
                //                             SLN = slabmaster.SLN,
                //                             SLAB_NAME = slabmaster.SLAB_NAME,
                //                             SLAB_DETAILS = slabmaster.SLAB_DETAILS,
                //                             SLAB_TYPE = slabmaster.SLAB_TYPE,
                //                             //Slab_TypeName = slabmaster.Slab_TypeName,
                //                             SLAB_STATUS = slabmaster.SLAB_STATUS,
                //                             //ASSIGNED_SLAB = slabmaster.ASSIGNED_SLAB,
                //                             OperatorDetails = (from slablist in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                //                                                where slablist.SLAB_ID == idval
                //                                                select new
                //                                                {
                //                                                    ID = slablist.SLN,
                //                                                    SERVICE_NAME = slablist.OPERATOR_NAME,
                //                                                    TYPE = slablist.OPERATOR_TYPE,
                //                                                    SERVICE_KEY = slablist.OPERATOR_CODE,
                //                                                    CommissionType = slablist.COMMISSION_TYPE,
                //                                                    CommissionPercentage = slablist.COMM_PERCENTAGE,
                //                                                    SuperDisPercentage = slablist.SUPER_COM_PER,
                //                                                    DistriCommissionPer = slablist.DISTRIBUTOR_COM_PER,
                //                                                    RetailerCommissionPer = slablist.MERCHANT_COM_PER,
                //                                                }).AsEnumerable().Select(z => new CommissionListView
                //                                                {
                //                                                    ID = z.ID,
                //                                                    SERVICE_NAME = z.SERVICE_NAME,
                //                                                    TYPE = z.TYPE,
                //                                                    SERVICE_KEY = z.SERVICE_KEY,
                //                                                    CommissionType = z.CommissionType,
                //                                                    CommissionPercentage = z.CommissionPercentage.ToString(),
                //                                                    SuperDisPercentage = z.SuperDisPercentage.ToString(),
                //                                                    DistriCommissionPer = z.DistriCommissionPer.ToString(),
                //                                                    RetailerCommissionPer = z.RetailerCommissionPer.ToString()
                //                                                }).ToList()
                //                         }).AsEnumerable().Select(d => new CommissoinManagmentmodel
                //                         {
                //                             SLN = d.SLN,
                //                             SLAB_NAME = d.SLAB_NAME,
                //                             SLAB_DETAILS = d.SLAB_DETAILS,
                //                             Slab_TypeName = slabtypename,
                //                             SLAB_STATUS = d.SLAB_STATUS,
                //                             SLAB_TYPE = d.SLAB_TYPE,
                //                             //ASSIGNED_SLAB = d.ASSIGNED_SLAB,
                //                             OperatorDetails = d.OperatorDetails.OrderByDescending(c => c.TYPE).ToList()
                //                         });
                //        return Json(valuelist, JsonRequestBehavior.AllowGet);
                //    }
                //    else if (slabtype.SLAB_TYPE == 3)
                //    {
                //        slabtypename = "DMR";
                //        var valuelist = (from slabmaster in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                //                         where slabmaster.SLN == idval
                //                         select new
                //                         {
                //                             SLN = slabmaster.SLN,
                //                             SLAB_NAME = slabmaster.SLAB_NAME,
                //                             SLAB_DETAILS = slabmaster.SLAB_DETAILS,
                //                             SLAB_TYPE = slabmaster.SLAB_TYPE,
                //                             //Slab_TypeName = slabmaster.Slab_TypeName,
                //                             SLAB_STATUS = slabmaster.SLAB_STATUS,
                //                             //ASSIGNED_SLAB = slabmaster.ASSIGNED_SLAB,
                //                             OperatorDetails = (from slablist in db.TBL_COMM_SLAB_DMR_PAYMENT
                //                                                where slablist.SLAB_ID == idval && slablist.DMT_TYPE == "DOMESTIC"
                //                                                select new
                //                                                {
                //                                                    ID = slablist.SLN,
                //                                                    SERVICE_NAME = "Money Transfer (Domestic)",
                //                                                    TYPE = "REMITTANCE",
                //                                                    DMRFrom = slablist.SLAB_FROM,
                //                                                    DMRTo = slablist.SLAB_TO,
                //                                                    SERVICE_KEY = "DMI",
                //                                                    COMM_TYPE = slablist.COMM_TYPE,
                //                                                    CommissionPercentage = slablist.COMM_PERCENTAGE,
                //                                                    SuperComm = slablist.SUPER_COM_PER,
                //                                                    DistributorCom = slablist.DISTRIBUTOR_COM_PER,
                //                                                    MerchantComm = slablist.MERCHANT_COM_PER,
                //                                                    DistributorComm_Type = slablist.DISTRIBUTOR_COMM_TYPE,
                //                                                    MerchantComm_Type = slablist.MERCHANT_COMM_TYPE,
                //                                                }).AsEnumerable().Select(z => new CommissionListView
                //                                                {
                //                                                    ID = z.ID,
                //                                                    SERVICE_NAME = "Money Transfer (Domestic)",
                //                                                    TYPE = "REMITTANCE",
                //                                                    DMRFrom = z.DMRFrom,
                //                                                    DMRTo = z.DMRTo,
                //                                                    SERVICE_KEY = "DMI",
                //                                                    COMM_TYPE = z.COMM_TYPE,
                //                                                    CommissionPercentage = z.CommissionPercentage.ToString(),
                //                                                    SuperDisPercentage = z.SuperComm.ToString(),
                //                                                    DistriCommissionPer = z.DistributorCom.ToString(),
                //                                                    RetailerCommissionPer = z.MerchantComm.ToString(),
                //                                                    DistributorComm_Type = z.DistributorComm_Type,
                //                                                    MerchantComm_Type = z.MerchantComm_Type,
                //                                                }).ToList(),
                //                             ServiceInformationDMR = (from slablist in db.TBL_COMM_SLAB_DMR_PAYMENT
                //                                                      where slablist.SLAB_ID == idval && slablist.DMT_TYPE == "FOREIGN"
                //                                                      select new
                //                                                      {
                //                                                          ID = slablist.SLN,
                //                                                          SERVICE_NAME = "Money Transfer (Nepal)",
                //                                                          TYPE = "REMITTANCE",
                //                                                          DMRFrom = slablist.SLAB_FROM,
                //                                                          DMRTo = slablist.SLAB_TO,
                //                                                          SERVICE_KEY = "PMT",
                //                                                          COMM_TYPE = slablist.COMM_TYPE,
                //                                                          CommissionPercentage = slablist.COMM_PERCENTAGE,
                //                                                          SuperCommAmt = slablist.SUPER_COM_PER,
                //                                                          DistributorComAmt = slablist.DISTRIBUTOR_COM_PER,
                //                                                          MerchantCommAmt = slablist.MERCHANT_COM_PER,
                //                                                          DistributorComm_Type = "FIXED",
                //                                                          MerchantComm_Type = "FIXED",
                //                                                      }).AsEnumerable().Select(z => new CommissionListView
                //                                                      {
                //                                                          ID = z.ID,
                //                                                          SERVICE_NAME = "Money Transfer (Nepal)",
                //                                                          TYPE = "REMITTANCE",
                //                                                          DMRFrom = z.DMRFrom,
                //                                                          DMRTo = z.DMRTo,
                //                                                          SERVICE_KEY = "PMT",
                //                                                          COMM_TYPE = z.COMM_TYPE,
                //                                                          CommissionPercentage = z.CommissionPercentage.ToString(),
                //                                                          SuperDisPercentage = z.SuperCommAmt.ToString(),
                //                                                          DistriCommissionPer = z.DistributorComAmt.ToString(),
                //                                                          RetailerCommissionPer = z.MerchantCommAmt.ToString(),
                //                                                          DistributorComm_Type = z.DistributorComm_Type,
                //                                                          MerchantComm_Type = z.MerchantComm_Type,
                //                                                      }).ToList()
                //                         }).AsEnumerable().Select(d => new CommissoinManagmentmodel
                //                         {
                //                             SLN = d.SLN,
                //                             SLAB_NAME = d.SLAB_NAME,
                //                             SLAB_DETAILS = d.SLAB_DETAILS,
                //                             Slab_TypeName = slabtypename,
                //                             SLAB_STATUS = d.SLAB_STATUS,
                //                             SLAB_TYPE = d.SLAB_TYPE,
                //                             //ASSIGNED_SLAB = d.ASSIGNED_SLAB,
                //                             OperatorDetails = d.OperatorDetails,
                //                             ServiceInformationDMR = d.ServiceInformationDMR
                //                         });
                //        return Json(valuelist, JsonRequestBehavior.AllowGet);

                //    }

                //}
                //else
                //{
                //    CommissoinManagmentmodel objmodel = new CommissoinManagmentmodel();
                //    return Json(objmodel, JsonRequestBehavior.AllowGet);
                //}
                
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MemberCommissionSlab(Admin), method:- FetchData (GET) Line No:- 398", ex);
                throw;
            }



        }
        
        [HttpPost]
        public async Task<JsonResult> GenerateCouponCommissionSlab(CouponCommissoinManagmentmodel objval)
        {
            initpage();////
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var couponlist in objval.OperatorDetails)
                    {
                        decimal CoupnVal = 0;
                        decimal SuperComVal = 0;
                        decimal DistCommVal = 0;
                        decimal MerCommVal = 0;
                        decimal.TryParse(couponlist.Comm_Value, out CoupnVal);
                        decimal.TryParse(couponlist.Super_Comm_Value, out SuperComVal);
                        decimal.TryParse(couponlist.Dist_Comm_value, out DistCommVal);
                        decimal.TryParse(couponlist.Merchant_Comm_Value, out MerCommVal);
                        var GetCouponSlab = db.TBL_COUPON_COMMISSION_SLAB.Where(x => x.sln == couponlist.Sln).FirstOrDefault();
                        if (GetCouponSlab == null)
                        {
                           
                            TBL_COUPON_COMMISSION_SLAB objcpnslab = new TBL_COUPON_COMMISSION_SLAB()
                            {
                                coupon_id = couponlist.coupon_id,
                                mem_Id = 0,
                                Coupon_Name = couponlist.Coupon_Name,
                                Comm_TYPE = couponlist.Comm_TYPE,
                                Comm_Value = CoupnVal,
                                Super_Role_Id = 3,
                                Dist_Role_Id = 4,
                                Merchant_Role_Id = 5,
                                Super_Comm_Value = SuperComVal,
                                Dist_Comm_value = DistCommVal,
                                Merchant_Comm_Value = MerCommVal,
                                Coupon_Status = objval.SLAB_STATUS,
                                Create_Date = DateTime.Now,
                                CouponSlab_Name = objval.SLAB_NAME,
                                CouponDetails = objval.SLAB_DETAILS,
                                Coupon_SlabType = objval.Slab_TypeName
                            };
                            db.TBL_COUPON_COMMISSION_SLAB.Add(objcpnslab);
                            db.SaveChanges();
                            ContextTransaction.Commit();
                            return Json(new { Result = "Success" });
                        }
                        else
                        {
                            GetCouponSlab.Super_Comm_Value = SuperComVal;
                            GetCouponSlab.Dist_Comm_value = DistCommVal;
                            GetCouponSlab.Merchant_Comm_Value = MerCommVal;
                            GetCouponSlab.Create_Date = DateTime.Now;
                            GetCouponSlab.Coupon_Status = objval.SLAB_STATUS;
                            db.Entry(GetCouponSlab).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            ContextTransaction.Commit();
                            return Json(new { Result = "Updated" });
                        }
                    }
                    return Json(new { Result = "" });
                }
                catch (Exception ex)
                {
                    return Json(new { Result = "Failure" });
                    ContextTransaction.Rollback();
                    throw;
                }               
            }
           
        }


        public ActionResult GetAllSuperRequisitionList()
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
        [HttpPost]
        public JsonResult getCouponReqTransdata(string TransId = "")
        {
            initpage();////
            try
            {
                var db = new DBContext();
                long transid = long.Parse(TransId);              
                var Tramslist = (from Cpn in db.TBL_COUPON_TRANSFER_LOGS
                                     //where Cpn.STATUS == "Pending" && Cpn.MEM_ROLE == 3 && Cpn.SLN== transid
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
                                     //CouponPrice= db.TBL_COUPON_COMMISSION_SLAB.Where(a => a.coupon_id == Cpn.COUPON_TYPE && a.Super_Role_Id==3).Select(c => c.Super_Comm_Value).FirstOrDefault(),
                                     CouponPrice = db.TBL_COUPON_COMMISSION_SLAB.Where(a => a.coupon_id == Cpn.COUPON_TYPE && a.Merchant_Role_Id == 5).Select(c => c.Merchant_Comm_Value).FirstOrDefault(),
                                     DistributorCommPrice = db.TBL_COUPON_COMMISSION_SLAB.Where(a => a.coupon_id == Cpn.COUPON_TYPE && a.Dist_Role_Id == 4).Select(c => c.Dist_Comm_value).FirstOrDefault(),
                                     SuperCommPrice = db.TBL_COUPON_COMMISSION_SLAB.Where(a => a.coupon_id == Cpn.COUPON_TYPE && a.Super_Role_Id == 3).Select(c => c.Super_Comm_Value).FirstOrDefault(),
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
                                     CouponPrice= Convert.ToDecimal(z.CouponPrice),
                                     DistributorCommPrice=Convert.ToDecimal(z.DistributorCommPrice),
                                     SuperCommPrice = Convert.ToDecimal(z.SuperCommPrice),
                                     DistGapCommPrice= (Convert.ToDecimal(z.CouponPrice)- Convert.ToDecimal(z.DistributorCommPrice)),
                                     SuperGapCommPrice= (Convert.ToDecimal(z.DistributorCommPrice) - Convert.ToDecimal(z.SuperCommPrice)),
                                     GSTNo =z.GstNo
                                 }).FirstOrDefault();

                return Json(Tramslist,JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var listdetails = new TBL_BALANCE_TRANSFER_LOGS();
                Logger.Error("Controller:-  MemberRequisition(Admin), method:- getTransdata (POST) Line No:- 433", ex);
                return Json(new { Result = "false", data = listdetails });
            }

        }
        public PartialViewResult SuperRequisitionIndexGrid()
        {
            initpage();
            try
            {
                var db = new DBContext();

                var Tramslist = (from Cpn in db.TBL_COUPON_TRANSFER_LOGS
                                 where Cpn.STATUS == "Pending" && Cpn.MEM_ROLE == 3
                                 select new
                                 {
                                     Sln = Cpn.SLN,
                                     From_Mem=Cpn.FROM_MEMBER,
                                     RefNo = Cpn.REFERENCE_NO,
                                     ReqDate = Cpn.REQUEST_DATE,
                                     MemBer_Name = db.TBL_MASTER_MEMBER.Where(a => a.MEM_ID == Cpn.FROM_MEMBER).Select(c => c.MEMBER_NAME).FirstOrDefault(),
                                     CouponType = Cpn.COUPON_TYPE,
                                     CouponName = db.TBL_COUPON_MASTER.Where(a => a.sln == Cpn.COUPON_TYPE).Select(c => c.couponType).FirstOrDefault(),
                                     Qty=Cpn.QTY,
                                     Status=Cpn.STATUS,
                                 }).AsEnumerable().Select(z => new TBL_COUPON_TRANSFER_LOGS
                                 {
                                     SLN=z.Sln,
                                     FROM_MEMBER=z.From_Mem,
                                     REFERENCE_NO=z.RefNo,
                                     REQUEST_DATE=z.ReqDate,
                                     Member_Name=z.MemBer_Name,
                                     COUPON_Name=z.CouponName,
                                     COUPON_TYPE=z.CouponType,
                                     QTY=z.Qty,
                                     STATUS=z.Status
                                 }).ToList();

                var Tramslist1 = db.TBL_COUPON_TRANSFER_LOGS.Where(x => x.STATUS == "Pending" && x.MEM_ROLE == 3).ToList(); 
                return PartialView("SuperRequisitionIndexGrid", Tramslist);                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult GetAllMerchantRequisition()
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
        public PartialViewResult MerchantRequisitionIndexGrid()
        {
            initpage();
            try
            {
                var db = new DBContext();

                var Tramslist = (from Cpn in db.TBL_COUPON_TRANSFER_LOGS
                                 where Cpn.STATUS == "Pending" && Cpn.MEM_ROLE == 5
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
                return PartialView("SuperRequisitionIndexGrid", Tramslist);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<JsonResult> ChangeTransactionStatus(string trandate = "", string TransationStatus = "", string slnval = "")
        //public async Task<JsonResult> ApproveRequisitionStatus(string slnval = "", string CouponAmt="0",string SlabAmt = "0",string GstVal="", string TDSVal = "", string CouponStartNoVal = "", string CouponEndNoVal = "", string UTIPaymentNo="", string CouponEndNoDate = "")
        //public async Task<JsonResult> ApproveRequisitionStatus(string slnval = "", string CouponAmt = "0", string SlabAmt = "0", string GstVal = "", string TDSVal = "")
        public async Task<JsonResult> ApproveRequisitionStatus(string slnval = "", string CouponAmt = "0", string SlabAmt = "0", string GstVal = "", string TDSVal = "0", string CouponStartNoVal = "", string CouponEndNoVal = "", string DistGapComm = "", string SuperGapComm = "")
        {
            initpage();////
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    DateTime RealDate = new DateTime();
                    //if (CouponEndNoDate != "")
                    //{
                    //    RealDate = Convert.ToDateTime(CouponEndNoDate);
                    //}
                    
                    long sln = long.Parse(slnval);
                    decimal CouponAmt_Value = 0;
                    decimal SlabCouponAmt = 0;
                    decimal.TryParse(CouponAmt, out CouponAmt_Value);
                    decimal.TryParse(SlabAmt, out SlabCouponAmt);
                    //var GSTTaxMode = db.TBL_TAX_MASTERS.Where(x => x.TAX_NAME == "GST").FirstOrDefault();  
                 
                    var TDSTaxMode = db.TBL_TAX_MASTERS.Where(x => x.TAX_NAME == "TDS").FirstOrDefault();
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
                    decimal GSTAMT =0;
                    decimal GMT_GST_Val = 0;
                    decimal AddAmtWiithGST = 0;
                    decimal Tds_Amt_Val = 0;
                    decimal NetValue = 0;
                    decimal TDS_Amt = 0;
                    decimal MemberStateGST = 0;
                    decimal MemberCenterGST = 0;
                    decimal MemberInternationalGST = 0;
                    decimal MemberStateGSTVal = 0;
                    decimal MemberCenterGSTVal = 0;
                    decimal MemberInternationalGSTVal = 0;
                    decimal TotalGst = 0;
                    string COrelationID = Settings.GetUniqueKey(MemberCurrentUser.MEM_ID.ToString());
                    var transinfo = await db.TBL_COUPON_TRANSFER_LOGS.Where(x => x.SLN == sln).FirstOrDefaultAsync();
                    if (transinfo != null)
                    {
                        var meminfo = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == transinfo.FROM_MEMBER);
                        var GSTTaxMode = db.TBL_STATES.Where(x => x.STATEID == meminfo.STATE_ID).FirstOrDefault();
                        MemberStateGST = decimal.Parse(GSTTaxMode.SGST.ToString());
                        MemberCenterGST = decimal.Parse(GSTTaxMode.CGST.ToString());
                        MemberInternationalGST = decimal.Parse(GSTTaxMode.IGST.ToString());
                        //if (GstVal == "Yes" && TDSVal == "Yes")
                        //{
                        //    //Tds_Amt_Val = TDSTaxMode.TAX_VALUE;
                        //    //GMT_GST_Val = GSTTaxMode.TAX_VALUE;
                        //    //checkAmt = (transinfo.QTY * CouponAmt_Value);
                        //    //GSTAMT = (checkAmt * GMT_GST_Val) / 100;
                        //    //AddAmtWiithGST = checkAmt + GSTAMT;
                        //    //TDS_Amt = (AddAmtWiithGST * Tds_Amt_Val) / 100;
                        //    //NetValue = (AddAmtWiithGST - TDS_Amt);
                        //    Tds_Amt_Val = TDSTaxMode.TAX_VALUE;                            
                        //    checkAmt = (transinfo.QTY * CouponAmt_Value);
                        //    MemberStateGSTVal = (checkAmt * MemberStateGST) / 100;
                        //    MemberCenterGSTVal = (checkAmt * MemberCenterGST) / 100;
                        //    MemberInternationalGSTVal = (checkAmt * MemberInternationalGST) / 100;
                        //    TotalGst = MemberStateGSTVal + MemberCenterGSTVal + MemberInternationalGSTVal;
                        //    AddAmtWiithGST = checkAmt + TotalGst;
                        //    TDS_Amt = (AddAmtWiithGST * Tds_Amt_Val) / 100;
                        //    NetValue = (AddAmtWiithGST - TDS_Amt);
                        //}
                        if (GstVal == "Yes" )
                        {
                            Tds_Amt_Val = TDSTaxMode.TAX_VALUE;
                            //GMT_GST_Val = GSTTaxMode.TAX_VALUE;
                            //MemberStateGST = decimal.Parse(GSTTaxMode.SGST.ToString());
                            //MemberCenterGST = decimal.Parse(GSTTaxMode.CGST.ToString());
                            //MemberInternationalGST = decimal.Parse(GSTTaxMode.IGST.ToString());
                            checkAmt = (transinfo.QTY * CouponAmt_Value);
                            MemberStateGSTVal = (checkAmt * MemberStateGST) / 100;
                            MemberCenterGSTVal = (checkAmt * MemberCenterGST) / 100;
                            MemberInternationalGSTVal = (checkAmt * MemberInternationalGST) / 100;
                            TotalGst = MemberStateGSTVal + MemberCenterGSTVal + MemberInternationalGSTVal;
                            
                            AddAmtWiithGST = checkAmt + TotalGst;                            
                            NetValue = (AddAmtWiithGST);
                            //Tds_Amt_Val = TDSTaxMode.TAX_VALUE;
                            //GMT_GST_Val = GSTTaxMode.TAX_VALUE;
                            //checkAmt = (transinfo.QTY * CouponAmt_Value);
                            //GSTAMT = (checkAmt * GMT_GST_Val) / 100;
                            //AddAmtWiithGST = checkAmt + GSTAMT;                            
                            //NetValue = (AddAmtWiithGST);
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

                        var Tomember = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEMBER_ROLE == 1);
                        var FromAmount = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == transinfo.FROM_MEMBER);
                        if (checkAmt <= FromAmount.BALANCE)
                        {
                            var getCouponSlab = db.TBL_COUPON_COMMISSION_SLAB.Where(x => x.coupon_id == transinfo.COUPON_TYPE && x.Super_Role_Id == 3).FirstOrDefault();
                            var checkAvailAmtWL = db.TBL_COUPON_STOCK.Where(x => x.couponHoderID == 0 && x.couponType == transinfo.COUPON_TYPE).FirstOrDefault();
                            if (checkAvailAmtWL != null)
                            {
                                if (Convert.ToDecimal(transinfo.QTY) <= Convert.ToDecimal(checkAvailAmtWL.couponQty))
                                {
                                    transinfo.STATUS = "Success";
                                    transinfo.APPROVAL_DATE = DateTime.Now;
                                    transinfo.APPROVAL_TIME = DateTime.Now;
                                    transinfo.APPROVED_BY = "Admin";
                                    transinfo.COMM_SLAB_ID = getCouponSlab.sln;
                                    transinfo.SELL_VALUE_RATE = SlabCouponAmt;
                                    transinfo.GROSS_SELL_VALUE = checkAmt;
                                    transinfo.NET_SELL_VALUE = NetValue;
                                    transinfo.IS_GST_APPLIED = GstVal;                                    
                                    transinfo.GST_PERCENTAGE = GMT_GST_Val;
                                    transinfo.GST_VALUE = TotalGst;
                                    transinfo.FROM_MEMBER_GST_NO = FromAmount.COMPANY_GST_NO;
                                    transinfo.TO_MEMBER_GST_NO = Tomember.COMPANY_GST_NO;
                                    transinfo.IS_TDS_APPLIED = TDSVal;
                                    transinfo.TDS_PERCENTAGE = Tds_Amt_Val;
                                    transinfo.TDS_VALUE = TDS_Amt;
                                    transinfo.TDS_SUBMIT_REF_NO = "";
                                    transinfo.GST_INPUT = TotalGst;
                                    transinfo.GST_OUTPUT = TotalGst;                                     
                                    transinfo.REAL_TRANSFER_TIME =DateTime.Now;
                                    transinfo.COUPON_START_NO = CouponStartNoVal;
                                    transinfo.COUPON_END_NO = CouponEndNoVal;
                                    transinfo.VENDOR_PAY_REF_NO = "";
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
                                            Vendor_Name = "Admin"
                                        };
                                        db.TBL_COUPON_STOCK.Add(objsupstk);
                                        db.SaveChanges();
                                        //var AdminStockSub=db.TBL_COUPON_STOCK.Where(x=>x.couponHoderID==0 && x.couponType==transinfo.COUPON_TYPE)
                                        AdminAvailStk = checkAvailAmtWL.couponQty;
                                        AdminRemStk = AdminAvailStk - transinfo.QTY;
                                        checkAvailAmtWL.couponQty = AdminRemStk;
                                        db.Entry(checkAvailAmtWL).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();

                                        // Merchant Accounts
                                        #region Merchant Requisition

                                      
                                        decimal closingamt_val = 0;
                                        decimal Supclosing = 0;
                                        decimal opening = 0;
                                        trans_bal = NetValue;
                                        var amtobj = await db.TBL_ACCOUNTS.Where(x => x.MEM_ID == transinfo.FROM_MEMBER).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefaultAsync();
                                        if (amtobj != null)
                                        {
                                            closingamt_val = amtobj.CLOSING;
                                            opening = amtobj.CLOSING;
                                            Supclosing = amtobj.CLOSING - NetValue;
                                        }
                                        else
                                        {
                                            opening = 0;
                                            closingamt_val = 0;
                                            Supclosing = NetValue;
                                        }
                                        TBL_ACCOUNTS onjSupAcnt = new TBL_ACCOUNTS()
                                        {
                                            API_ID = 0,
                                            MEM_ID = transinfo.FROM_MEMBER,
                                            MEMBER_TYPE = "MERCHANT",
                                            TRANSACTION_TYPE = "WALLET",
                                            TRANSACTION_DATE = Convert.ToDateTime(transinfo.REQUEST_DATE),
                                            TRANSACTION_TIME = DateTime.Now,
                                            DR_CR = "DR",
                                            AMOUNT = NetValue,
                                            NARRATION = "Coupon Buy from Admin",
                                            OPENING = opening,
                                            CLOSING = Supclosing,
                                            REC_NO = 0,
                                            COMM_AMT = 0,
                                            GST = 0,
                                            TDS = 0,
                                            IPAddress = "",
                                            SERVICE_ID = 0,
                                            CORELATIONID = transinfo.REFERENCE_NO,
                                            REC_COMM_TYPE = getCouponSlab.Comm_TYPE,
                                            COMM_VALUE = getCouponSlab.Comm_Value,
                                            NET_COMM_AMT = 0,
                                            TDS_DR_COMM_AMT = TDS_Amt,
                                            CGST_COMM_AMT_INPUT = MemberCenterGSTVal,
                                            CGST_COMM_AMT_OUTPUT = 0,
                                            SGST_COMM_AMT_INPUT = MemberStateGSTVal,
                                            SGST_COMM_AMT_OUTPUT = 0,
                                            IGST_COMM_AMT_INPUT= MemberInternationalGSTVal,
                                            IGST_COMM_AMT_OUTPUT=0,
                                            TOTAL_GST_COMM_AMT_INPUT= TotalGst,
                                            TOTAL_GST_COMM_AMT_OUTPUT=0,
                                            TDS_RATE= Tds_Amt_Val,
                                            CGST_RATE= MemberCenterGST,
                                            SGST_RATE = MemberStateGST,
                                            IGST_RATE = MemberInternationalGST,
                                            TOTAL_GST_RATE= MemberCenterGST+ MemberStateGST+ MemberInternationalGST,
                                            COMM_SLAB_ID= getCouponSlab.sln,
                                            STATE_ID= meminfo.STATE_ID,
                                            FLAG1=0,
                                            FLAG2 = 0,
                                            FLAG3 = 0,
                                            FLAG4 = 0,
                                            FLAG5 = 0,
                                            FLAG6 = 0,
                                            FLAG7 = 0,
                                            FLAG8 = 0,
                                            FLAG9 = 0,
                                            FLAG10 = 0,
                                            INVOICE_ID=0,
                                            CANCEL_INVOICE="",
                                            VENDOR_ID=0
                                        };
                                        db.TBL_ACCOUNTS.Add(onjSupAcnt);
                                        db.SaveChanges();
                                        decimal From_Memb_Amt_val = 0;
                                        decimal Rem_Bal = 0;
                                        var Member_Acnt = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == transinfo.FROM_MEMBER).FirstOrDefault();
                                        if (Member_Acnt.BALANCE == 0)
                                        {
                                            ///*Member_Acnt.BALANCE = trans_bal;*/
                                            Member_Acnt.BALANCE = NetValue; 
                                        }
                                        else
                                        {
                                            decimal.TryParse(Member_Acnt.BALANCE.ToString(),out From_Memb_Amt_val);
                                            From_Memb_Amt_val = From_Memb_Amt_val;
                                            Rem_Bal = ((From_Memb_Amt_val) - (trans_bal));
                                            Member_Acnt.BALANCE = Rem_Bal;
                                        }
                                        db.Entry(Member_Acnt).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                        #endregion

                                        #region Distributor Gsp Comm
                                        decimal DistStateGst = 0;
                                        decimal DistCenterGst = 0;
                                        decimal DistinternationalGst = 0;
                                        var DistInfo = db.TBL_MASTER_MEMBER.FirstOrDefault(x=>x.MEM_ID==meminfo.INTRODUCER);
                                        var DistStateGSt = db.TBL_STATES.FirstOrDefault(x=>x.STATEID== DistInfo.STATE_ID);
                                        decimal.TryParse(DistStateGSt.SGST.ToString(), out DistStateGst);
                                        decimal.TryParse(DistStateGSt.CGST.ToString(), out DistCenterGst);
                                        decimal.TryParse(DistStateGSt.IGST.ToString(), out DistinternationalGst);
                                        decimal DistStateGstVal = 0;
                                        decimal DistCenterGstVal = 0;
                                        decimal DistinternationalGstVal = 0;
                                        decimal DistTotalGSt = 0;
                                        decimal DistGapCommVal = 0;
                                        decimal TotalcouponQty = 0;
                                        decimal TotalDistComm = 0;
                                        decimal NetDistCommVal = 0;
                                        decimal DistTDS = 0;
                                        decimal DistNetCommAmount = 0;
                                        decimal.TryParse(transinfo.QTY.ToString(), out TotalcouponQty);
                                        decimal.TryParse(DistGapComm,out DistGapCommVal);
                                        TotalDistComm = DistGapCommVal * TotalcouponQty;
                                        //if (DistGSTApply == "Yes")
                                        //{
                                        //    DistStateGstVal = ((TotalDistComm * DistStateGst) / 100);
                                        //    DistCenterGstVal = ((TotalDistComm * DistCenterGst) / 100);
                                        //    DistinternationalGstVal = ((TotalDistComm * DistinternationalGst) / 100);
                                        //}
                                        //else
                                        //{
                                        //    DistStateGstVal =0;
                                        //    DistCenterGstVal =0;
                                        //    DistinternationalGstVal = 0;
                                        //}
                                        //DistTotalGSt = DistStateGstVal + DistCenterGstVal + DistinternationalGstVal;  TDSVal
                                        decimal TDS_AMOUNT = 0;
                                        decimal.TryParse(TDSVal, out TDS_AMOUNT);
                                        NetDistCommVal = TotalDistComm;
                                        if (TDS_AMOUNT != null)
                                        {
                                            DistTDS = (NetDistCommVal * TDS_AMOUNT) / 100;
                                            DistNetCommAmount = NetDistCommVal - DistTDS;
                                        }
                                        else
                                        {
                                            DistTDS = 0;
                                            DistNetCommAmount = NetDistCommVal - DistTDS;
                                        }
                                        decimal DistopeningBal = 0;
                                        decimal DistClosingingBal = 0;
                                        decimal DistAddgBal = 0;
                                        var DistAccount = await db.TBL_ACCOUNTS.Where(x => x.MEM_ID == DistInfo.MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefaultAsync();
                                        if (DistAccount != null)
                                        {
                                            decimal.TryParse(DistAccount.OPENING.ToString(), out DistopeningBal);
                                            decimal.TryParse(DistAccount.CLOSING.ToString(), out DistClosingingBal);
                                            DistAddgBal = DistClosingingBal + DistNetCommAmount;
                                        }
                                        else
                                        {
                                            DistAddgBal = DistNetCommAmount;
                                            DistClosingingBal = 0;
                                        }
                                        TBL_ACCOUNTS DistAcntAdd = new TBL_ACCOUNTS()
                                        {
                                            API_ID = 0,
                                            MEM_ID = DistInfo.MEM_ID,
                                            MEMBER_TYPE = "DISTRIBUTOR",
                                            TRANSACTION_TYPE = "WALLET",
                                            TRANSACTION_DATE = Convert.ToDateTime(transinfo.REQUEST_DATE),
                                            TRANSACTION_TIME = DateTime.Now,
                                            DR_CR = "CR",
                                            AMOUNT = DistNetCommAmount,
                                            NARRATION = "Commission Disburse To Distributor",
                                            OPENING = DistClosingingBal,
                                            CLOSING = DistAddgBal,
                                            REC_NO = 0,
                                            COMM_AMT = 0,
                                            GST = 0,
                                            TDS = 0,
                                            IPAddress = "",
                                            SERVICE_ID = 0,
                                            CORELATIONID = transinfo.REFERENCE_NO,
                                            REC_COMM_TYPE = getCouponSlab.Comm_TYPE,
                                            COMM_VALUE = getCouponSlab.Comm_Value,
                                            NET_COMM_AMT = 0,
                                            TDS_DR_COMM_AMT = DistTDS,
                                            CGST_COMM_AMT_INPUT = DistStateGstVal,
                                            CGST_COMM_AMT_OUTPUT = 0,
                                            SGST_COMM_AMT_INPUT = DistCenterGstVal,
                                            SGST_COMM_AMT_OUTPUT = 0,
                                            IGST_COMM_AMT_INPUT = DistinternationalGstVal,
                                            IGST_COMM_AMT_OUTPUT = 0,
                                            TOTAL_GST_COMM_AMT_INPUT = DistTotalGSt,
                                            TOTAL_GST_COMM_AMT_OUTPUT = 0,
                                            TDS_RATE = Tds_Amt_Val,
                                            CGST_RATE = DistCenterGst,
                                            SGST_RATE = DistStateGst,
                                            IGST_RATE = DistinternationalGst,
                                            TOTAL_GST_RATE = DistCenterGst + DistStateGst + DistinternationalGst,
                                            COMM_SLAB_ID = getCouponSlab.sln,
                                            STATE_ID = DistInfo.STATE_ID,
                                            FLAG1 = 0,
                                            FLAG2 = 0,
                                            FLAG3 = 0,
                                            FLAG4 = 0,
                                            FLAG5 = 0,
                                            FLAG6 = 0,
                                            FLAG7 = 0,
                                            FLAG8 = 0,
                                            FLAG9 = 0,
                                            FLAG10 = 0,
                                            INVOICE_ID = 0,
                                            CANCEL_INVOICE = "",
                                            VENDOR_ID = 0
                                        };
                                        db.TBL_ACCOUNTS.Add(DistAcntAdd);
                                        db.SaveChanges();
                                        decimal DistAddMainBalance = 0;
                                        decimal DistmainBalance = 0;
                                        decimal.TryParse(DistInfo.BALANCE.ToString(), out DistmainBalance);
                                        DistAddMainBalance = DistmainBalance + DistNetCommAmount;
                                        DistInfo.BALANCE = DistAddMainBalance;
                                        db.Entry(DistInfo).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                        #endregion

                                        #region Super Gsp Comm
                                        decimal SuperStateGst = 0;
                                        decimal SuperCenterGst = 0;
                                        decimal SuperinternationalGst = 0;
                                        var SuperInfo = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == DistInfo.INTRODUCER);
                                        var SuperStateGSt = db.TBL_STATES.FirstOrDefault(x => x.STATEID == SuperInfo.STATE_ID);
                                        decimal.TryParse(SuperStateGSt.SGST.ToString(), out SuperStateGst);
                                        decimal.TryParse(SuperStateGSt.CGST.ToString(), out SuperCenterGst);
                                        decimal.TryParse(SuperStateGSt.IGST.ToString(), out SuperinternationalGst);
                                        decimal SuperStateGstVal = 0;
                                        decimal SuperCenterGstVal = 0;
                                        decimal SuperinternationalGstVal = 0;
                                        decimal SuperTotalGSt = 0;
                                        decimal SuperGapCommVal = 0;
                                        decimal SupercouponQty = 0;
                                        decimal TotalSuperComm = 0;
                                        decimal NetSuperCommVal = 0;
                                        decimal SuperTDS = 0;
                                        decimal SuperNetCommAmount = 0;
                                        decimal.TryParse(transinfo.QTY.ToString(), out SupercouponQty);
                                        decimal.TryParse(SuperGapComm, out SuperGapCommVal);
                                        TotalSuperComm = SuperGapCommVal * SupercouponQty;
                                        //if (SuperGSTApply == "Yes")
                                        //{
                                        //    SuperStateGstVal = ((TotalSuperComm * SuperStateGst) / 100);
                                        //    SuperCenterGstVal = ((TotalSuperComm * SuperCenterGst) / 100);
                                        //    SuperinternationalGstVal = ((TotalSuperComm * SuperinternationalGst) / 100);
                                        //}
                                        //else
                                        //{
                                        //    SuperStateGstVal = 0;
                                        //    SuperCenterGstVal = 0;
                                        //    SuperinternationalGstVal = 0;
                                        //}
                                        //SuperTotalGSt = SuperStateGstVal + SuperCenterGstVal + SuperinternationalGstVal;
                                        decimal SuperTDS_AMOUNT = 0;
                                        decimal.TryParse(TDSVal, out SuperTDS_AMOUNT);
                                       // NetDistCommVal = TotalDistComm;

                                        NetSuperCommVal = TotalSuperComm ;
                                        if (SuperTDS_AMOUNT != null)
                                        {
                                            SuperTDS = (NetSuperCommVal * SuperTDS_AMOUNT) / 100;
                                            SuperNetCommAmount = NetSuperCommVal - SuperTDS;
                                        }
                                        else
                                        {
                                            SuperTDS = 0;
                                            SuperNetCommAmount = NetSuperCommVal - SuperTDS;
                                        }
                                        decimal SuperopeningBal = 0;
                                        decimal SuperClosingingBal = 0;
                                        decimal SuperAddgBal = 0;
                                        var SuperAccount = await db.TBL_ACCOUNTS.Where(x => x.MEM_ID == SuperInfo.MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefaultAsync();
                                        if (SuperAccount != null)
                                        {
                                            decimal.TryParse(SuperAccount.OPENING.ToString(), out SuperopeningBal);
                                            decimal.TryParse(SuperAccount.CLOSING.ToString(), out SuperClosingingBal);
                                            SuperAddgBal = SuperClosingingBal + SuperNetCommAmount;
                                        }
                                        else
                                        {
                                            SuperAddgBal = SuperNetCommAmount;
                                            SuperClosingingBal = 0;
                                        }
                                        TBL_ACCOUNTS SuperAcntAdd = new TBL_ACCOUNTS()
                                        {
                                            API_ID = 0,
                                            MEM_ID = SuperInfo.MEM_ID,
                                            MEMBER_TYPE = "SUPER",
                                            TRANSACTION_TYPE = "WALLET",
                                            TRANSACTION_DATE = Convert.ToDateTime(transinfo.REQUEST_DATE),
                                            TRANSACTION_TIME = DateTime.Now,
                                            DR_CR = "CR",
                                            AMOUNT = SuperNetCommAmount,
                                            NARRATION = "Commission Disburse To Super",
                                            OPENING = SuperClosingingBal,
                                            CLOSING = SuperAddgBal,
                                            REC_NO = 0,
                                            COMM_AMT = 0,
                                            GST = 0,
                                            TDS = 0,
                                            IPAddress = "",
                                            SERVICE_ID = 0,
                                            CORELATIONID = transinfo.REFERENCE_NO,
                                            REC_COMM_TYPE = getCouponSlab.Comm_TYPE,
                                            COMM_VALUE = getCouponSlab.Comm_Value,
                                            NET_COMM_AMT = 0,
                                            TDS_DR_COMM_AMT = SuperTDS,
                                            CGST_COMM_AMT_INPUT = SuperStateGstVal,
                                            CGST_COMM_AMT_OUTPUT = 0,
                                            SGST_COMM_AMT_INPUT = SuperCenterGstVal,
                                            SGST_COMM_AMT_OUTPUT = 0,
                                            IGST_COMM_AMT_INPUT = SuperinternationalGstVal,
                                            IGST_COMM_AMT_OUTPUT = 0,
                                            TOTAL_GST_COMM_AMT_INPUT = SuperTotalGSt,
                                            TOTAL_GST_COMM_AMT_OUTPUT = 0,
                                            TDS_RATE = Tds_Amt_Val,
                                            CGST_RATE = SuperCenterGst,
                                            SGST_RATE = SuperStateGst,
                                            IGST_RATE = SuperinternationalGst,
                                            TOTAL_GST_RATE = SuperCenterGst + SuperStateGst + SuperinternationalGst,
                                            COMM_SLAB_ID = getCouponSlab.sln,
                                            STATE_ID = SuperInfo.STATE_ID,
                                            FLAG1 = 0,
                                            FLAG2 = 0,
                                            FLAG3 = 0,
                                            FLAG4 = 0,
                                            FLAG5 = 0,
                                            FLAG6 = 0,
                                            FLAG7 = 0,
                                            FLAG8 = 0,
                                            FLAG9 = 0,
                                            FLAG10 = 0,
                                            INVOICE_ID = 0,
                                            CANCEL_INVOICE = "",
                                            VENDOR_ID = 0
                                        };
                                        db.TBL_ACCOUNTS.Add(SuperAcntAdd);
                                        db.SaveChanges();
                                        decimal SuperAddMainBalance = 0;
                                        decimal SupermainBalance = 0;
                                        decimal.TryParse(SuperInfo.BALANCE.ToString(), out SupermainBalance);
                                        SuperAddMainBalance = SupermainBalance + SuperNetCommAmount;
                                        SuperInfo.BALANCE = SuperAddMainBalance;
                                        db.Entry(SuperInfo).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                        #endregion
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

                                        //Accounts
                                        decimal closingamt_val = 0;
                                        decimal Supclosing = 0;
                                        decimal opening = 0;
                                        trans_bal = NetValue;
                                        var amtobj = await db.TBL_ACCOUNTS.Where(x => x.MEM_ID == transinfo.FROM_MEMBER).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefaultAsync();
                                        if (amtobj != null)
                                        {
                                            closingamt_val = amtobj.CLOSING;
                                            opening = amtobj.CLOSING;
                                            Supclosing = amtobj.CLOSING - NetValue;
                                        }
                                        else
                                        {
                                            opening = 0;
                                            closingamt_val = 0;
                                            Supclosing = NetValue;
                                        }
                                        TBL_ACCOUNTS onjSupAcnt = new TBL_ACCOUNTS()
                                        {
                                            API_ID = 0,
                                            MEM_ID = transinfo.FROM_MEMBER,
                                            MEMBER_TYPE = "MERCHANT",
                                            TRANSACTION_TYPE = "WALLET",
                                            TRANSACTION_DATE = Convert.ToDateTime(transinfo.REQUEST_DATE),
                                            TRANSACTION_TIME = DateTime.Now,
                                            DR_CR = "DR",
                                            AMOUNT = trans_bal,
                                            NARRATION = "Coupon Buy from Admin",
                                            OPENING = opening,
                                            CLOSING = Supclosing,
                                            REC_NO = 0,
                                            COMM_AMT = 0,
                                            GST = 0,
                                            TDS = 0,
                                            IPAddress = "",
                                            SERVICE_ID = 0,
                                            CORELATIONID = transinfo.REFERENCE_NO,
                                            REC_COMM_TYPE = getCouponSlab.Comm_TYPE,
                                            COMM_VALUE = getCouponSlab.Comm_Value,
                                            NET_COMM_AMT = 0,
                                            TDS_DR_COMM_AMT = TDS_Amt,
                                            CGST_COMM_AMT_INPUT = MemberCenterGSTVal,
                                            CGST_COMM_AMT_OUTPUT = 0,
                                            SGST_COMM_AMT_INPUT = MemberStateGSTVal,
                                            SGST_COMM_AMT_OUTPUT = 0,
                                            IGST_COMM_AMT_INPUT = MemberInternationalGSTVal,
                                            IGST_COMM_AMT_OUTPUT = 0,
                                            TOTAL_GST_COMM_AMT_INPUT = TotalGst,
                                            TOTAL_GST_COMM_AMT_OUTPUT = 0,
                                            TDS_RATE = Tds_Amt_Val,
                                            CGST_RATE = MemberCenterGST,
                                            SGST_RATE = MemberStateGST,
                                            IGST_RATE = MemberInternationalGST,
                                            TOTAL_GST_RATE = MemberCenterGST + MemberStateGST + MemberInternationalGST,
                                            COMM_SLAB_ID = getCouponSlab.sln,
                                            STATE_ID = meminfo.STATE_ID,
                                            FLAG1 = 0,
                                            FLAG2 = 0,
                                            FLAG3 = 0,
                                            FLAG4 = 0,
                                            FLAG5 = 0,
                                            FLAG6 = 0,
                                            FLAG7 = 0,
                                            FLAG8 = 0,
                                            FLAG9 = 0,
                                            FLAG10 = 0,
                                            INVOICE_ID = 0,
                                            CANCEL_INVOICE = "",
                                            VENDOR_ID = 0
                                        };
                                        db.TBL_ACCOUNTS.Add(onjSupAcnt);
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
                                            decimal.TryParse(Member_Acnt.BALANCE.ToString(), out From_Avai_amt);                                            
                                            From_Avai_amt = From_Avai_amt;                                            
                                            Rem_Bal = ((From_Avai_amt) - (trans_bal));
                                            Member_Acnt.BALANCE = Rem_Bal;
                                        }
                                        db.Entry(Member_Acnt).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();

                                        #region Distributor Gsp Comm
                                        decimal DistStateGst = 0;
                                        decimal DistCenterGst = 0;
                                        decimal DistinternationalGst = 0;
                                        var DistInfo = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == meminfo.INTRODUCER);
                                        var DistStateGSt = db.TBL_STATES.FirstOrDefault(x => x.STATEID == DistInfo.STATE_ID);
                                        decimal.TryParse(DistStateGSt.SGST.ToString(), out DistStateGst);
                                        decimal.TryParse(DistStateGSt.CGST.ToString(), out DistCenterGst);
                                        decimal.TryParse(DistStateGSt.IGST.ToString(), out DistinternationalGst);
                                        decimal DistStateGstVal = 0;
                                        decimal DistCenterGstVal = 0;
                                        decimal DistinternationalGstVal = 0;
                                        decimal DistTotalGSt = 0;
                                        decimal DistGapCommVal = 0;
                                        decimal TotalcouponQty = 0;
                                        decimal TotalDistComm = 0;
                                        decimal NetDistCommVal = 0;
                                        decimal DistTDS = 0;
                                        decimal DistNetCommAmount = 0;
                                        decimal.TryParse(transinfo.QTY.ToString(), out TotalcouponQty);
                                        decimal.TryParse(DistGapComm, out DistGapCommVal);
                                        TotalDistComm = DistGapCommVal * TotalcouponQty;
                                        //if (DistGSTApply == "Yes")
                                        //{
                                        //    DistStateGstVal = ((TotalDistComm * DistStateGst) / 100);
                                        //    DistCenterGstVal = ((TotalDistComm * DistCenterGst) / 100);
                                        //    DistinternationalGstVal = ((TotalDistComm * DistinternationalGst) / 100);
                                        //}
                                        //else
                                        //{
                                        //    DistStateGstVal = 0;
                                        //    DistCenterGstVal = 0;
                                        //    DistinternationalGstVal = 0;
                                        //}
                                        //DistTotalGSt = DistStateGstVal + DistCenterGstVal + DistinternationalGstVal;
                                        decimal DistTDS_AMOUNT = 0;
                                        decimal.TryParse(TDSVal, out DistTDS_AMOUNT);

                                        NetDistCommVal = TotalDistComm ;
                                        if (DistTDS_AMOUNT != null)
                                        {
                                            DistTDS = (NetDistCommVal * DistTDS_AMOUNT) / 100;
                                            DistNetCommAmount = NetDistCommVal - DistTDS;
                                        }
                                        else
                                        {
                                            DistTDS = 0;
                                            DistNetCommAmount = NetDistCommVal - DistTDS;
                                        }
                                        decimal DistopeningBal = 0;
                                        decimal DistClosingingBal = 0;
                                        decimal DistAddgBal = 0;
                                        var DistAccount = await db.TBL_ACCOUNTS.Where(x => x.MEM_ID == DistInfo.MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefaultAsync();
                                        if (DistAccount != null)
                                        {
                                            decimal.TryParse(DistAccount.OPENING.ToString(), out DistopeningBal);
                                            decimal.TryParse(DistAccount.CLOSING.ToString(), out DistClosingingBal);
                                            DistAddgBal = DistClosingingBal + DistNetCommAmount;
                                        }
                                        else
                                        {
                                            DistClosingingBal = 0;
                                            DistAddgBal = DistNetCommAmount;
                                        }
                                        TBL_ACCOUNTS DistAcntAdd = new TBL_ACCOUNTS()
                                        {
                                            API_ID = 0,
                                            MEM_ID = DistInfo.MEM_ID,
                                            MEMBER_TYPE = "DISTRIBUTOR",
                                            TRANSACTION_TYPE = "WALLET",
                                            TRANSACTION_DATE = Convert.ToDateTime(transinfo.REQUEST_DATE),
                                            TRANSACTION_TIME = DateTime.Now,
                                            DR_CR = "CR",
                                            AMOUNT = DistNetCommAmount,
                                            NARRATION = "Commission Disburse To Distributor",
                                            OPENING = DistClosingingBal,
                                            CLOSING = DistAddgBal,
                                            REC_NO = 0,
                                            COMM_AMT = 0,
                                            GST = 0,
                                            TDS = 0,
                                            IPAddress = "",
                                            SERVICE_ID = 0,
                                            CORELATIONID = transinfo.REFERENCE_NO,
                                            REC_COMM_TYPE = getCouponSlab.Comm_TYPE,
                                            COMM_VALUE = getCouponSlab.Comm_Value,
                                            NET_COMM_AMT = 0,
                                            TDS_DR_COMM_AMT = DistTDS,
                                            CGST_COMM_AMT_INPUT = DistStateGstVal,
                                            CGST_COMM_AMT_OUTPUT = 0,
                                            SGST_COMM_AMT_INPUT = DistCenterGstVal,
                                            SGST_COMM_AMT_OUTPUT = 0,
                                            IGST_COMM_AMT_INPUT = DistinternationalGstVal,
                                            IGST_COMM_AMT_OUTPUT = 0,
                                            TOTAL_GST_COMM_AMT_INPUT = DistTotalGSt,
                                            TOTAL_GST_COMM_AMT_OUTPUT = 0,
                                            TDS_RATE = Tds_Amt_Val,
                                            CGST_RATE = DistCenterGst,
                                            SGST_RATE = DistStateGst,
                                            IGST_RATE = DistinternationalGst,
                                            TOTAL_GST_RATE = DistCenterGst + DistStateGst + DistinternationalGst,
                                            COMM_SLAB_ID = getCouponSlab.sln,
                                            STATE_ID = DistInfo.STATE_ID,
                                            FLAG1 = 0,
                                            FLAG2 = 0,
                                            FLAG3 = 0,
                                            FLAG4 = 0,
                                            FLAG5 = 0,
                                            FLAG6 = 0,
                                            FLAG7 = 0,
                                            FLAG8 = 0,
                                            FLAG9 = 0,
                                            FLAG10 = 0,
                                            INVOICE_ID = 0,
                                            CANCEL_INVOICE = "",
                                            VENDOR_ID = 0
                                        };
                                        db.TBL_ACCOUNTS.Add(DistAcntAdd);
                                        db.SaveChanges();
                                        decimal DistAddMainBalance = 0;
                                        decimal DistmainBalance = 0;
                                        decimal.TryParse(DistInfo.BALANCE.ToString(), out DistmainBalance);
                                        DistAddMainBalance = DistmainBalance + DistNetCommAmount;
                                        DistInfo.BALANCE = DistAddMainBalance;
                                        db.Entry(DistInfo).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                        #endregion

                                        #region Super Gsp Comm
                                        decimal SuperStateGst = 0;
                                        decimal SuperCenterGst = 0;
                                        decimal SuperinternationalGst = 0;
                                        var SuperInfo = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == DistInfo.INTRODUCER);
                                        var SuperStateGSt = db.TBL_STATES.FirstOrDefault(x => x.STATEID == SuperInfo.STATE_ID);
                                        decimal.TryParse(SuperStateGSt.SGST.ToString(), out SuperStateGst);
                                        decimal.TryParse(SuperStateGSt.CGST.ToString(), out SuperCenterGst);
                                        decimal.TryParse(SuperStateGSt.IGST.ToString(), out SuperinternationalGst);
                                        decimal SuperStateGstVal = 0;
                                        decimal SuperCenterGstVal = 0;
                                        decimal SuperinternationalGstVal = 0;
                                        decimal SuperTotalGSt = 0;
                                        decimal SuperGapCommVal = 0;
                                        decimal SupercouponQty = 0;
                                        decimal TotalSuperComm = 0;
                                        decimal NetSuperCommVal = 0;
                                        decimal SuperTDS = 0;
                                        decimal SuperNetCommAmount = 0;
                                        decimal.TryParse(transinfo.QTY.ToString(), out SupercouponQty);
                                        decimal.TryParse(SuperGapComm, out SuperGapCommVal);
                                        TotalSuperComm = SuperGapCommVal * SupercouponQty;
                                        //if (SuperGSTApply == "Yes")
                                        //{
                                        //    SuperStateGstVal = ((TotalSuperComm * SuperStateGst) / 100);
                                        //    SuperCenterGstVal = ((TotalSuperComm * SuperCenterGst) / 100);
                                        //    SuperinternationalGstVal = ((TotalSuperComm * SuperinternationalGst) / 100);
                                        //}
                                        //else
                                        //{
                                        //    SuperStateGstVal = 0;
                                        //    SuperCenterGstVal = 0;
                                        //    SuperinternationalGstVal = 0;
                                        //}
                                        //SuperTotalGSt = SuperStateGstVal + SuperCenterGstVal + SuperinternationalGstVal;

                                        decimal SUPER_TDS_AMOUNT = 0;
                                        decimal.TryParse(TDSVal, out SUPER_TDS_AMOUNT);

                                        NetSuperCommVal = TotalSuperComm ;
                                        if (SUPER_TDS_AMOUNT != null)
                                        {
                                            SuperTDS = (NetSuperCommVal * SUPER_TDS_AMOUNT) / 100;
                                            SuperNetCommAmount = NetSuperCommVal - SuperTDS;
                                        }
                                        else
                                        {
                                            SuperTDS = 0;
                                            SuperNetCommAmount = NetSuperCommVal - SuperTDS;
                                        }
                                        decimal SuperopeningBal = 0;
                                        decimal SuperClosingingBal = 0;
                                        decimal SuperAddgBal = 0;
                                        var SuperAccount = await db.TBL_ACCOUNTS.Where(x => x.MEM_ID == SuperInfo.MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefaultAsync();
                                        if (SuperAccount != null)
                                        {
                                            decimal.TryParse(SuperAccount.OPENING.ToString(), out SuperopeningBal);
                                            decimal.TryParse(SuperAccount.CLOSING.ToString(), out SuperClosingingBal);
                                            SuperAddgBal = SuperClosingingBal + SuperNetCommAmount;
                                        }
                                        else
                                        {
                                            SuperAddgBal = SuperNetCommAmount;
                                            SuperClosingingBal = 0;
                                        }
                                        TBL_ACCOUNTS SuperAcntAdd = new TBL_ACCOUNTS()
                                        {
                                            API_ID = 0,
                                            MEM_ID = SuperInfo.MEM_ID,
                                            MEMBER_TYPE = "SUPER",
                                            TRANSACTION_TYPE = "WALLET",
                                            TRANSACTION_DATE = Convert.ToDateTime(transinfo.REQUEST_DATE),
                                            TRANSACTION_TIME = DateTime.Now,
                                            DR_CR = "CR",
                                            AMOUNT = SuperNetCommAmount,
                                            NARRATION = "Commission Disburse To Super",
                                            OPENING = SuperClosingingBal,
                                            CLOSING = SuperAddgBal,
                                            REC_NO = 0,
                                            COMM_AMT = 0,
                                            GST = 0,
                                            TDS = 0,
                                            IPAddress = "",
                                            SERVICE_ID = 0,
                                            CORELATIONID = transinfo.REFERENCE_NO,
                                            REC_COMM_TYPE = getCouponSlab.Comm_TYPE,
                                            COMM_VALUE = getCouponSlab.Comm_Value,
                                            NET_COMM_AMT = 0,
                                            TDS_DR_COMM_AMT = SuperTDS,
                                            CGST_COMM_AMT_INPUT = SuperStateGstVal,
                                            CGST_COMM_AMT_OUTPUT = 0,
                                            SGST_COMM_AMT_INPUT = SuperCenterGstVal,
                                            SGST_COMM_AMT_OUTPUT = 0,
                                            IGST_COMM_AMT_INPUT = SuperinternationalGstVal,
                                            IGST_COMM_AMT_OUTPUT = 0,
                                            TOTAL_GST_COMM_AMT_INPUT = SuperTotalGSt,
                                            TOTAL_GST_COMM_AMT_OUTPUT = 0,
                                            TDS_RATE = Tds_Amt_Val,
                                            CGST_RATE = SuperCenterGst,
                                            SGST_RATE = SuperStateGst,
                                            IGST_RATE = SuperinternationalGst,
                                            TOTAL_GST_RATE = SuperCenterGst + SuperStateGst + SuperinternationalGst,
                                            COMM_SLAB_ID = getCouponSlab.sln,
                                            STATE_ID = SuperInfo.STATE_ID,
                                            FLAG1 = 0,
                                            FLAG2 = 0,
                                            FLAG3 = 0,
                                            FLAG4 = 0,
                                            FLAG5 = 0,
                                            FLAG6 = 0,
                                            FLAG7 = 0,
                                            FLAG8 = 0,
                                            FLAG9 = 0,
                                            FLAG10 = 0,
                                            INVOICE_ID = 0,
                                            CANCEL_INVOICE = "",
                                            VENDOR_ID = 0
                                        };
                                        db.TBL_ACCOUNTS.Add(SuperAcntAdd);
                                        db.SaveChanges();
                                        decimal SuperAddMainBalance = 0;
                                        decimal SupermainBalance = 0;
                                        decimal.TryParse(SuperInfo.BALANCE.ToString(), out SupermainBalance);
                                        SuperAddMainBalance = SupermainBalance + SuperNetCommAmount;
                                        SuperInfo.BALANCE = SuperAddMainBalance;
                                        db.Entry(SuperInfo).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                        #endregion

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
        public async Task<JsonResult> RequisitionDecline(string slnval = "")
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
                    return Json("Requisition Decline",JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Logger.Error("Controller:-  MemberRequisition(Admin), method:- TransactionDecline (POST) Line No:- 631", ex);
                    ContextTransaction.Rollback();
                    return Json(new { Result = "false " });
                }
            }


        }


        public ActionResult GetAdminCouponStock()
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
        public PartialViewResult GetAdminCouponStockIndexGrid()
        {
            initpage();
            try
            {
                var db = new DBContext();
                var Tramslist = (from Cpn in db.TBL_COUPON_STOCK
                                 where Cpn.couponHoderID == 0
                                 select new
                                 {
                                     sln=Cpn.sln,
                                     HolderName="Admin",
                                     PurchaseDate=Cpn.puchaseDate,
                                     stockEntryDate = Cpn.stockEntryDate,
                                     CouponName= db.TBL_COUPON_MASTER.Where(a => a.sln == Cpn.couponType).Select(c => c.couponType).FirstOrDefault(),
                                     CouponId = Cpn.couponType,
                                     Qty=Cpn.couponQty,
                                    VendorName=Cpn.Vendor_Name,
                                 }).AsEnumerable().Select(z => new TBL_COUPON_STOCK
                                 {
                                     sln=z.sln,
                                     HolderName=z.HolderName,
                                     puchaseDate=z.PurchaseDate,
                                     stockEntryDate=z.stockEntryDate,
                                     CouponName=z.CouponName,
                                     couponType=z.CouponId,
                                     couponQty=z.Qty ,
                                     Vendor_Name=z.VendorName
                                 }).ToList();
                return PartialView("GetAdminCouponStockIndexGrid", Tramslist);
                //return PartialView(CreateExportableGrid());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PartialViewResult AllSuperRequisitionIndexGrid()
        {
            initpage();
            try
            {
                var db = new DBContext();

                var Tramslist = (from Cpn in db.TBL_COUPON_TRANSFER_LOGS
                                 where Cpn.STATUS == "Pending" && Cpn.MEM_ROLE == 3
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
                return PartialView("SuperRequisitionIndexGrid", Tramslist);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult SuperStocktransaction()
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
        public PartialViewResult SuperStocktransactionIndexGrid()
        {
            initpage();
            try
            {
                var db = new DBContext();

                var Tramslist = (from Cpn in db.TBL_COUPON_TRANSFER_LOGS
                                 where Cpn.MEM_ROLE == 1
                                 select new
                                 {
                                     Sln = Cpn.SLN,
                                     From_Mem = Cpn.FROM_MEMBER,
                                     RefNo = Cpn.REFERENCE_NO,
                                     ReqDate = Cpn.REQUEST_DATE,
                                     MemBer_Name = "Admin",
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
                return PartialView("SuperStocktransactionIndexGrid", Tramslist);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public ActionResult SuperPurchaseCouponReport()
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

        public PartialViewResult SuperPurchaseIndexGrid()
        {
            initpage();
            try
            {
                var db = new DBContext();

                var Tramslist = (from Cpn in db.TBL_COUPON_TRANSFER_LOGS
                                 where   Cpn.MEM_ROLE == 3
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
                return PartialView("SuperPurchaseIndexGrid", Tramslist);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost]
        public JsonResult PrintcouponInvoice(string TransId = "")
        {
            initpage();////
            try
            {
                var db = new DBContext();
                long transid = long.Parse(TransId);
                var Tramslist = (from Cpn in db.TBL_COUPON_TRANSFER_LOGS join mem in db.TBL_MASTER_MEMBER on Cpn.FROM_MEMBER equals mem.MEM_ID
                                 where Cpn.STATUS == "Success" && Cpn.MEM_ROLE == 3 && Cpn.SLN == transid
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
                                     CompanyName=mem.COMPANY,
                                     Address=mem.ADDRESS,
                                     Mobile=mem.MEMBER_MOBILE,
                                     Total= (Cpn.QTY* Cpn.SELL_VALUE_RATE),
                                     GST_Amount=Cpn.GST_VALUE,
                                     TDS_Amount=Cpn.TDS_VALUE,
                                     NetValue=Cpn.NET_SELL_VALUE,
                                     ImageVal= db.TBL_MASTER_MEMBER.Where(a => a.UNDER_WHITE_LEVEL == 0 && a.MEMBER_ROLE==1).Select(c => c.LOGO).FirstOrDefault(),
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
                                     CompanyName=z.CompanyName,
                                     Address=z.Address,
                                     Mem_Mobile=z.Mobile,
                                     TotalAmount=z.Total,
                                     GST_VALUE=z.GST_Amount,
                                     TDS_VALUE=z.TDS_Amount,
                                     NET_SELL_VALUE=z.NetValue,
                                     Logo=z.ImageVal
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

        public ActionResult GetAllSuccessRequisition()
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
        public PartialViewResult MerchantSuccessRequisitionIndexGrid()
        {
            initpage();
            try
            {
                var db = new DBContext();

                var Tramslist = (from Cpn in db.TBL_COUPON_TRANSFER_LOGS
                                 where Cpn.STATUS == "Success" && Cpn.MEM_ROLE == 5
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

                var Tramslist1 = db.TBL_COUPON_TRANSFER_LOGS.Where(x => x.STATUS == "Success" && x.MEM_ROLE == 3).ToList();
                return PartialView("MerchantSuccessRequisitionIndexGrid", Tramslist);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}