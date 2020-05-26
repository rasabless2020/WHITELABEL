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
using System.Web.Security;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Areas.Distributor.Models;
using WHITELABEL.Web.Controllers;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;

namespace WHITELABEL.Web.Areas.Distributor.Controllers
{
    [Authorize]
    public class DistributorCreditManagmentController : DistributorBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        // GET: Distributor/DistributorCreditManagment
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
        public ActionResult Index()
        {
            if (Session["DistributorUserId"] != null)
            {
                initpage();
                var db = new DBContext();
                var memberrole = db.TBL_MASTER_MEMBER_ROLE.Where(x => x.ROLE_ID ==5).ToList();
                ViewBag.RoleDetails = new SelectList(memberrole, "ROLE_ID", "ROLE_NAME");
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
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> PostCREDITBALANCE(DistributorCreditViewModel objCredit)
        {
            var DBcon = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = DBcon.Database.BeginTransaction())
            {
                try
                {
                    decimal DIST_CLOSINGAMT = 0;
                    decimal DIST_OPENINGAMT = 0;
                    var MerberName = DBcon.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == objCredit.FROM_MEM_ID);
                    var TDSVal = DBcon.TBL_TAX_MASTERS.FirstOrDefault(x => x.SLN == 2).TAX_VALUE;
                    string COrelationID = Settings.GetUniqueKey(MemberCurrentUser.MEM_ID.ToString());
                    var member_Role = DBcon.TBL_MASTER_MEMBER_ROLE.FirstOrDefault(x => x.ROLE_ID == objCredit.MEMBER_ROLE).ROLE_NAME;
                    //var DistAccount = DBcon.TBL_ACCOUNTS.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                    var DistAccount = DBcon.TBL_ACCOUNTS.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).OrderByDescending(z => z.ACC_NO).FirstOrDefault();
                    if (DistAccount != null)
                    {
                        decimal.TryParse(DistAccount.CLOSING.ToString(),out DIST_CLOSINGAMT);
                        decimal.TryParse(DistAccount.OPENING.ToString(), out DIST_OPENINGAMT);
                    }
                    else
                    {
                         DIST_CLOSINGAMT = 0;
                         DIST_OPENINGAMT = 0;
                    }
                    TBL_ACCOUNTS GetDistributor = new TBL_ACCOUNTS()
                    {
                        API_ID = 0,
                        MEM_ID = MemberCurrentUser.MEM_ID,
                        MEMBER_TYPE = "DISTRIBUTOR",
                        TRANSACTION_TYPE = "CREDIT NOTES",
                        TRANSACTION_DATE = DateTime.Now,
                        TRANSACTION_TIME = DateTime.Now,
                        DR_CR = "DR",
                        AMOUNT = objCredit.CREDIT_AMOUNT,
                        OPENING = DIST_OPENINGAMT,
                        CLOSING = DIST_CLOSINGAMT,
                        REC_NO = 0,
                        COMM_AMT = 0,
                        GST = 0,
                        TDS = 0,
                        IPAddress = "",
                        GST_PERCENTAGE = 0,
                        TDS_PERCENTAGE = 0,
                        WHITELEVEL_ID = 0,
                        SUPER_ID = 0,
                        DISTRIBUTOR_ID = 0,
                        SERVICE_ID = 0,
                        CORELATIONID = COrelationID
                    };
                    DBcon.TBL_ACCOUNTS.Add(GetDistributor);
                    DBcon.SaveChanges();

                    decimal TdsVal = 0;
                    decimal GstVal = 0;
                    decimal TotalGST = 0;
                    decimal TDS_Amount = 0;
                    decimal GST_Amount = 0;
                    decimal TotalCreditAmt = 0;
                    decimal CreditAmtWhithTDS = 0;
                    decimal AddMainBalance = 0;
                    decimal UpdateMainBalance = 0;
                    decimal.TryParse(TDSVal.ToString(), out TdsVal);
                    if (objCredit.CREDIT_TYPE == "DR")
                    {
                        var GetGSTVAlue = DBcon.TBL_STATES.FirstOrDefault(x => x.STATEID == MerberName.STATE_ID);
                        TotalGST = GetGSTVAlue.SGST + GetGSTVAlue.IGST + GetGSTVAlue.CGST;
                        decimal.TryParse(TotalGST.ToString(), out GstVal);
                        if (objCredit.GSTAPPLY == "Yes")
                        {
                            GST_Amount = ((objCredit.CREDIT_AMOUNT) * TotalGST) / 100;
                            TotalCreditAmt = objCredit.CREDIT_AMOUNT + GST_Amount;
                        }
                        else
                        {
                            GST_Amount = 0;
                            TotalCreditAmt = objCredit.CREDIT_AMOUNT + GST_Amount;
                        }
                    }
                    else
                    {
                        GST_Amount = 0;
                        TotalCreditAmt = objCredit.CREDIT_AMOUNT + GST_Amount;
                    }
                    if (objCredit.TDSAPPLY == "Yes")
                    {
                        TDS_Amount = ((objCredit.CREDIT_AMOUNT) * TdsVal) / 100;
                        CreditAmtWhithTDS = objCredit.CREDIT_AMOUNT - TDS_Amount;
                        TotalCreditAmt = objCredit.CREDIT_AMOUNT + GST_Amount - TDS_Amount;
                    }
                    else
                    {
                        TDS_Amount = 0;
                        CreditAmtWhithTDS = objCredit.CREDIT_AMOUNT - TDS_Amount;
                        TotalCreditAmt = objCredit.CREDIT_AMOUNT + GST_Amount - TDS_Amount;
                    }
                    AddMainBalance = Convert.ToDecimal(MerberName.BALANCE);
                    if (objCredit.CREDIT_TYPE == "DR")
                    {
                        UpdateMainBalance = AddMainBalance - TotalCreditAmt;
                    }
                    else
                    {
                        UpdateMainBalance = AddMainBalance + TotalCreditAmt;
                    }
                    TBL_CREDIT_BALANCE_DISTRIBUTION objcr = new TBL_CREDIT_BALANCE_DISTRIBUTION()
                    {
                        TO_MEM_ID = MemberCurrentUser.MEM_ID,
                        FROM_MEM_ID = objCredit.FROM_MEM_ID,
                        MEMBER_ROLE = objCredit.MEMBER_ROLE,
                        CREDIT_DATE = DateTime.Now,
                        CREDIT_TYPE = objCredit.CREDIT_TYPE,
                        //CREDIT_AMOUNT = objCredit.CREDIT_AMOUNT,
                        CREDIT_AMOUNT = TotalCreditAmt,
                        GST_VAL = TotalGST,
                        GST_AMOUNT = GST_Amount,
                        TDS_VAL = TdsVal,
                        TDS_AMOUNT = TDS_Amount,
                        CREDIT_NOTE_DESCRIPTION = objCredit.CREDIT_NOTE_DESCRIPTION,
                        CREDIT_STATUS = true
                    };
                    DBcon.TBL_CREDIT_BALANCE_DISTRIBUTION.Add(objcr);
                    DBcon.SaveChanges();
                    MerberName.BALANCE = UpdateMainBalance;
                    DBcon.Entry(MerberName).State = System.Data.Entity.EntityState.Modified;
                    DBcon.SaveChanges();
                    decimal Frm_OpeningAmt = 0;
                    decimal Frm_ClosingAmt = 0;
                    decimal Frm_AddOpeningAmt = 0;
                    decimal Frm_AddClosingAmt = 0;
                    //var FromMemberamtobj = DBcon.TBL_ACCOUNTS.Where(x => x.MEM_ID == objCredit.FROM_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                    var FromMemberamtobj = DBcon.TBL_ACCOUNTS.Where(x => x.MEM_ID == objCredit.FROM_MEM_ID).OrderByDescending(z => z.ACC_NO).FirstOrDefault();
                    if (FromMemberamtobj != null)
                    {
                        decimal.TryParse(FromMemberamtobj.CLOSING.ToString(), out Frm_ClosingAmt);
                        if (objCredit.CREDIT_TYPE == "DR")
                        {
                            Frm_AddClosingAmt = Frm_ClosingAmt - TotalCreditAmt;
                        }
                        else
                        {
                            Frm_AddClosingAmt = Frm_ClosingAmt + TotalCreditAmt;
                        }
                    }
                    else
                    {
                        Frm_AddClosingAmt = TotalCreditAmt;
                    }
                    TBL_ACCOUNTS objacnt = new TBL_ACCOUNTS()
                    {
                        API_ID = 0,
                        MEM_ID = objCredit.FROM_MEM_ID,
                        MEMBER_TYPE = member_Role,
                        TRANSACTION_TYPE = "CREDIT NOTES",
                        TRANSACTION_DATE = DateTime.Now,
                        TRANSACTION_TIME = DateTime.Now,
                        DR_CR = objCredit.CREDIT_TYPE,
                        AMOUNT = (decimal)TotalCreditAmt,
                        NARRATION = objCredit.CREDIT_NOTE_DESCRIPTION,
                        OPENING = Frm_ClosingAmt,
                        CLOSING = Frm_AddClosingAmt,
                        REC_NO = 0,
                        COMM_AMT = 0,
                        GST = (double)GST_Amount,
                        TDS = (double)TDS_Amount,
                        IPAddress = "",
                        SERVICE_ID = 0,
                        CORELATIONID = COrelationID
                    };
                    DBcon.TBL_ACCOUNTS.Add(objacnt);
                    DBcon.SaveChanges();

                     #region fOR dISTRIBUTOR Wallet Deduct Amount
                    decimal DistMainbal = 0;
                    decimal DistAddMainBalance = 0;
                    decimal DistOpeningAmt = 0;
                    decimal DistClosingAmt = 0;
                    decimal DistAdjustClosingAmt = 0;
                    var dist_mainInfor = DBcon.TBL_MASTER_MEMBER.FirstOrDefault(x=>x.MEM_ID==MemberCurrentUser.MEM_ID);
                    decimal.TryParse(dist_mainInfor.BALANCE.ToString(), out DistMainbal);
                    if (objCredit.CREDIT_TYPE == "CR")
                    {
                        DistAddMainBalance = DistMainbal - TotalCreditAmt;
                    }
                    else
                    {
                        DistAddMainBalance = DistMainbal + TotalCreditAmt;
                    }
                    dist_mainInfor.BALANCE = DistAddMainBalance;
                    DBcon.Entry(dist_mainInfor).State = System.Data.Entity.EntityState.Modified;
                    DBcon.SaveChanges();

                    //var DistAccount = DBcon.TBL_ACCOUNTS.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                    
                    if (DistAccount != null)
                    {
                        decimal.TryParse(DistAccount.CLOSING.ToString(), out DistClosingAmt);

                    }
                    else
                    {
                        //DistAdjustClosingAmt = TotalCreditAmt;//TotalCreditAmt
                        DistAdjustClosingAmt = objCredit.CREDIT_AMOUNT;//TotalCreditAmt
                    }
                    if (objCredit.CREDIT_TYPE == "CR")
                    {
                        //DistAdjustClosingAmt = DistClosingAmt - TotalCreditAmt;
                        DistAdjustClosingAmt = DistClosingAmt - objCredit.CREDIT_AMOUNT;
                    }
                    else
                    {
                        //DistAdjustClosingAmt = DistClosingAmt + TotalCreditAmt;
                        DistAdjustClosingAmt = DistClosingAmt + objCredit.CREDIT_AMOUNT;
                    }
                   
                    ContextTransaction.Commit();
                    var GDistAcnt = DBcon.TBL_ACCOUNTS.FirstOrDefault(x => x.CORELATIONID == COrelationID);                    
                    GDistAcnt.CLOSING = DistAdjustClosingAmt;
                    DBcon.Entry(GDistAcnt).State = System.Data.Entity.EntityState.Modified;
                    DBcon.SaveChanges();
                    //TBL_ACCOUNTS objDistacntSettelment = new TBL_ACCOUNTS()
                    //{
                    //    API_ID = 0,
                    //    MEM_ID = MemberCurrentUser.MEM_ID,
                    //    MEMBER_TYPE = "DISTRIBUTOR",
                    //    TRANSACTION_TYPE = "CREDIT NOTES",
                    //    TRANSACTION_DATE = DateTime.Now,
                    //    TRANSACTION_TIME = DateTime.Now,
                    //    DR_CR = "DR",
                    //    AMOUNT = (decimal)TotalCreditAmt,
                    //    NARRATION = objCredit.CREDIT_NOTE_DESCRIPTION,
                    //    OPENING = DistClosingAmt,
                    //    CLOSING = DistAdjustClosingAmt,
                    //    REC_NO = 0,
                    //    COMM_AMT = 0,
                    //    GST = 0,
                    //    TDS = 0,
                    //    IPAddress = "",
                    //    SERVICE_ID = 0,
                    //    CORELATIONID = COrelationID
                    //};
                    //DBcon.TBL_ACCOUNTS.Add(objDistacntSettelment);
                    //DBcon.SaveChanges();                    
                    #endregion

                    //var member_Role = DBcon.TBL_MASTER_MEMBER_ROLE.FirstOrDefault(x => x.ROLE_ID == objCredit.MEMBER_ROLE).ROLE_NAME;
                    return Json("Credit balance given to " + MerberName.UName + ". ");
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    return Json("Try again after some time");
                    throw;
                }
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetMemberName(string prefix, string MemberType)
        {
            try
            {
                var db = new DBContext();
                long MEm_RoleId = 0;
                long.TryParse(MemberType, out MEm_RoleId);
                var OperatorValue = await (from oper in db.TBL_MASTER_MEMBER
                                           where oper.UName.StartsWith(prefix) && oper.MEMBER_ROLE == MEm_RoleId
                                           select new
                                           {
                                               //label = oper.SERVICE_NAME + "-" + oper.RECHTYPE,
                                               label = oper.UName + " - " + oper.MEMBER_MOBILE + " - " + oper.COMPANY,
                                               val = oper.MEM_ID
                                           }).ToListAsync();
                return Json(OperatorValue);
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- AutoComplete(POST) Line No:- 252", ex);
                throw ex;
            }
        }

        public ActionResult GetCreditNoteList()
        {
            if (Session["DistributorUserId"] != null)
            {
                try
                {
                    initpage();
                    return View();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            else
            {
                Session["DistributorUserId"] = null;
                Session["DistributorUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("DistributorUserId");
                Session.Remove("DistributorUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        public PartialViewResult IndexGrid(string DateFrom = "", string Date_To = "")
        {
            try
            {
                var dbcontext = new DBContext();
                if (DateFrom != "" && Date_To != "")
                {
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    DateTime To_Date_Val = Date_To_Val.AddDays(1);
                    var memberinfo = (from tblcre in dbcontext.TBL_CREDIT_BALANCE_DISTRIBUTION
                                      join mem in dbcontext.TBL_MASTER_MEMBER on tblcre.FROM_MEM_ID equals mem.MEM_ID
                                      join MemRol in dbcontext.TBL_MASTER_MEMBER_ROLE on mem.MEMBER_ROLE equals MemRol.ROLE_ID
                                      where tblcre.TO_MEM_ID == MemberCurrentUser.MEM_ID && tblcre.CREDIT_DATE >= Date_From_Val && tblcre.CREDIT_DATE <= To_Date_Val
                                      select new
                                      {
                                          sln = tblcre.SLN,
                                          MemberRole = MemRol.ROLE_NAME,
                                          Mem_Name = mem.UName,
                                          CrediType = tblcre.CREDIT_TYPE,
                                          Credit_note = tblcre.CREDIT_NOTE_DESCRIPTION,
                                          CreditNoteDate = tblcre.CREDIT_DATE,
                                          CreditAmount = tblcre.CREDIT_AMOUNT,
                                          creditStatus = tblcre.CREDIT_STATUS
                                      }).AsEnumerable().Select(z => new TBL_CREDIT_BALANCE_DISTRIBUTION
                                      {
                                          SLN = z.sln,
                                          FromUser = z.Mem_Name,
                                          Member_RoleName = z.MemberRole,
                                          CREDIT_DATE = z.CreditNoteDate,
                                          CREDIT_TYPE = z.CrediType,
                                          CREDIT_AMOUNT = z.CreditAmount,
                                          CREDIT_NOTE_DESCRIPTION = z.Credit_note,
                                          CREDIT_STATUS = z.creditStatus
                                      }).ToList();
                    return PartialView("IndexGrid", memberinfo);
                }
                else
                {
                    DateTime NowDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));

                    DateTime Todaydate = Convert.ToDateTime(DateTime.Now.AddDays(1));
                    DateTime Today_date = Convert.ToDateTime(Todaydate.ToString("yyyy-MM-dd"));
                    var memberinfo = (from tblcre in dbcontext.TBL_CREDIT_BALANCE_DISTRIBUTION
                                      join mem in dbcontext.TBL_MASTER_MEMBER on tblcre.FROM_MEM_ID equals mem.MEM_ID
                                      join MemRol in dbcontext.TBL_MASTER_MEMBER_ROLE on mem.MEMBER_ROLE equals MemRol.ROLE_ID
                                      where tblcre.TO_MEM_ID == MemberCurrentUser.MEM_ID && tblcre.CREDIT_DATE >= NowDate && tblcre.CREDIT_DATE <= Today_date
                                      select new
                                      {
                                          sln = tblcre.SLN,
                                          MemberRole = MemRol.ROLE_NAME,
                                          Mem_Name = mem.UName,
                                          CrediType = tblcre.CREDIT_TYPE,
                                          Credit_note = tblcre.CREDIT_NOTE_DESCRIPTION,
                                          CreditNoteDate = tblcre.CREDIT_DATE,
                                          CreditAmount = tblcre.CREDIT_AMOUNT,
                                          creditStatus = tblcre.CREDIT_STATUS
                                      }).AsEnumerable().Select(z => new TBL_CREDIT_BALANCE_DISTRIBUTION
                                      {
                                          SLN = z.sln,
                                          FromUser = z.Mem_Name,
                                          Member_RoleName = z.MemberRole,
                                          CREDIT_DATE = z.CreditNoteDate,
                                          CREDIT_TYPE = z.CrediType,
                                          CREDIT_AMOUNT = z.CreditAmount,
                                          CREDIT_NOTE_DESCRIPTION = z.Credit_note,
                                          CREDIT_STATUS = z.creditStatus
                                      }).ToList();
                    return PartialView("IndexGrid", memberinfo);
                }

                    
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //////////////////Credit Limit Setting///////////////////

        public ActionResult CreditLimitList()
        {
            if (Session["DistributorUserId"] != null)
            {
                try
                {
                    initpage();
                    return View();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            else
            {
                Session["DistributorUserId"] = null;
                Session["DistributorUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("DistributorUserId");
                Session.Remove("DistributorUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        public PartialViewResult CREDITLimitIndexGrid(string DateFrom = "", string Date_To = "")
        {
            try
            {
                var dbcontext = new DBContext();
                if (DateFrom != "" && Date_To != "")
                {
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    DateTime To_Date_Val = Date_To_Val.AddDays(1);
                    var memberinfo = (from tblcre in dbcontext.TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      join mem in dbcontext.TBL_MASTER_MEMBER on tblcre.FROM_MEM_ID equals mem.MEM_ID
                                      join MemRol in dbcontext.TBL_MASTER_MEMBER_ROLE on mem.MEMBER_ROLE equals MemRol.ROLE_ID
                                      where tblcre.TO_MEM_ID == MemberCurrentUser.MEM_ID && tblcre.CREDIT_DATE>= Date_From_Val && tblcre.CREDIT_DATE<= To_Date_Val
                                      select new
                                      {
                                          sln = tblcre.SLN,
                                          Mem_Name = mem.UName,
                                          Credit_note = tblcre.CREDIT_NOTE_DESCRIPTION,
                                          CreditNoteDate = tblcre.CREDIT_DATE,
                                          CreditAmount = tblcre.CREDIT_AMOUNT,
                                          creditStatus = tblcre.CREDIT_STATUS,
                                          DR_CR = tblcre.CREDIT_AMOUNT,
                                          OpeningAmt = tblcre.CREDIT_OPENING,
                                          Closingamt = tblcre.CREDITCLOSING,
                                          creditType = tblcre.CREDIT_TRN_TYPE
                                      }).AsEnumerable().Select(z => new TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      {
                                          SLN = z.sln,
                                          FromUser = z.Mem_Name,
                                          CREDIT_DATE = z.CreditNoteDate,
                                          //CREDIT_AMOUNT = z.CreditAmount,
                                          CREDIT_NOTE_DESCRIPTION = z.Credit_note,
                                          CREDIT_STATUS = z.creditStatus,
                                          CREDIT_AMOUNT = z.DR_CR,
                                          CREDITCLOSING = z.Closingamt,
                                          CR_Col = (z.creditType == "CR" ? z.CreditAmount.ToString() : "0"),
                                          DR_Col = (z.creditType == "DR" ? z.CreditAmount.ToString() : "0"),
                                          CREDIT_OPENING = z.OpeningAmt,
                                          CREDIT_TRN_TYPE = z.creditType
                                      }).ToList().OrderByDescending(a => a.CREDIT_DATE);
                    return PartialView("CREDITLimitIndexGrid", memberinfo);
                }
                else
                {
                    DateTime NowDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                    DateTime Todaydate = Convert.ToDateTime(DateTime.Now.AddDays(1));
                    DateTime Today_date = Convert.ToDateTime(Todaydate.ToString("yyyy-MM-dd"));
                    var memberinfo = (from tblcre in dbcontext.TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      join mem in dbcontext.TBL_MASTER_MEMBER on tblcre.FROM_MEM_ID equals mem.MEM_ID
                                      join MemRol in dbcontext.TBL_MASTER_MEMBER_ROLE on mem.MEMBER_ROLE equals MemRol.ROLE_ID
                                      where tblcre.TO_MEM_ID == MemberCurrentUser.MEM_ID  && tblcre.CREDIT_DATE >= NowDate && tblcre.CREDIT_DATE <= Today_date
                                      select new
                                      {
                                          sln = tblcre.SLN,
                                          Mem_Name = mem.UName,
                                          Credit_note = tblcre.CREDIT_NOTE_DESCRIPTION,
                                          CreditNoteDate = tblcre.CREDIT_DATE,
                                          CreditAmount = tblcre.CREDIT_AMOUNT,
                                          creditStatus = tblcre.CREDIT_STATUS,
                                          DR_CR = tblcre.CREDIT_AMOUNT,
                                          OpeningAmt = tblcre.CREDIT_OPENING,
                                          Closingamt = tblcre.CREDITCLOSING,
                                          creditType = tblcre.CREDIT_TRN_TYPE
                                      }).AsEnumerable().Select(z => new TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      {
                                          SLN = z.sln,
                                          FromUser = z.Mem_Name,
                                          CREDIT_DATE = z.CreditNoteDate,
                                          //CREDIT_AMOUNT = z.CreditAmount,
                                          CREDIT_NOTE_DESCRIPTION = z.Credit_note,
                                          CREDIT_STATUS = z.creditStatus,
                                          CREDIT_AMOUNT = z.DR_CR,
                                          CREDITCLOSING = z.Closingamt,
                                          CR_Col = (z.creditType == "CR" ? z.CreditAmount.ToString() : "0"),
                                          DR_Col = (z.creditType == "DR" ? z.CreditAmount.ToString() : "0"),
                                          CREDIT_OPENING = z.OpeningAmt,
                                          CREDIT_TRN_TYPE = z.creditType
                                      }).ToList().OrderByDescending(a => a.CREDIT_DATE);
                    return PartialView("CREDITLimitIndexGrid", memberinfo);
                }

                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult DistributCreditLimit()
        {
            if (Session["DistributorUserId"] != null)
            {
                try
                {
                    initpage();
                    var db = new DBContext();
                    var memberInfo = db.TBL_MASTER_MEMBER.Where(x => x.INTRODUCER == MemberCurrentUser.MEM_ID).ToList();
                    //"UName" + "'-'" + "MEMBER_MOBILE"
                    ViewBag.MemberDetails = new SelectList(memberInfo, "MEM_ID", "UName" + "'-'" + "MEMBER_MOBILE");

                    //var OperatorValue = (from oper in db.TBL_MASTER_MEMBER
                    //                          where oper.INTRODUCER==MemberCurrentUser.MEM_ID
                    //                          select new
                    //                          {
                    //                              //label = oper.SERVICE_NAME + "-" + oper.RECHTYPE,
                    //                              Text = oper.UName + " - " + oper.MEMBER_MOBILE + " - " + oper.MEM_ID,
                    //                              Value = oper.MEM_ID
                    //                          }).ToList();
                    //ViewBag.MemberDetails = new SelectList(OperatorValue, "MEM_ID", "FromUser");
                    var Reserved_creditlimitDist = db.TBL_MASTER_MEMBER.Where(x => x.INTRODUCER == MemberCurrentUser.MEM_ID).FirstOrDefault().RESERVED_CREDIT_LIMIT;
                    DistributorCreditLimitSetting model = new DistributorCreditLimitSetting();
                    model.MOBILE_RECHARGE = true;
                    model.UTILITY_SERVICES = true;
                    model.DMR = true;
                    model.AIR_TICKET = true;
                    model.BUS_TICKET = true;
                    model.HOTEL_BOOKING = true;
                    model.RAIL_UTILITY = true;
                    model.reservedCreditLimit = Convert.ToDecimal(Reserved_creditlimitDist);
                    return View(model);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            else
            {
                Session["DistributorUserId"] = null;
                Session["DistributorUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("DistributorUserId");
                Session.Remove("DistributorUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });
            }

            
        }

        [HttpPost]
        public async Task<JsonResult> GetMerchantMemberName(string prefix)
        {
            try
            {
                var db = new DBContext();
                long MEm_RoleId = 0;                
                var OperatorValue = await (from oper in db.TBL_MASTER_MEMBER
                                           where oper.UName.StartsWith(prefix) && oper.INTRODUCER==MemberCurrentUser.MEM_ID
                                           select new
                                           {
                                               //label = oper.SERVICE_NAME + "-" + oper.RECHTYPE,
                                               label = oper.UName + " - " + oper.MEMBER_MOBILE + " - " + oper.COMPANY,
                                               val = oper.MEM_ID
                                           }).ToListAsync();
                return Json(OperatorValue);
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MerchantRechargeService(Merchant), method:- AutoComplete(POST) Line No:- 252", ex);
                throw ex;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> PostCREDITLimitBALANCESetting(DistributorCreditLimitSetting objCredit)
        {
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {

                try
                {
                    var DistributorBalanceCheckInfo = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == MemberCurrentUser.MEM_ID).BALANCE;
                    if (DistributorBalanceCheckInfo >= objCredit.CREDIT_LIMIT_AMOUNT)
                    {


                        var MerchantServices = db.TBL_WHITELABLE_SERVICE.Where(x => x.MEMBER_ID == objCredit.FROM_MEM_ID).ToList();
                        foreach (var Serlst in MerchantServices)
                        {
                            if (Serlst.SERVICE_ID == 1)
                            {
                                var MobServ = db.TBL_WHITELABLE_SERVICE.FirstOrDefault(x => x.MEMBER_ID == objCredit.FROM_MEM_ID && x.SERVICE_ID == 1);
                                MobServ.CREDIT_LIMITSTATUS = objCredit.MOBILE_RECHARGE;
                                db.Entry(MobServ).State = System.Data.Entity.EntityState.Modified;
                                //db.SaveChanges();
                            }
                            else if (Serlst.SERVICE_ID == 2)
                            {
                                var MobServ = db.TBL_WHITELABLE_SERVICE.FirstOrDefault(x => x.MEMBER_ID == objCredit.FROM_MEM_ID && x.SERVICE_ID == 2);
                                MobServ.CREDIT_LIMITSTATUS = objCredit.UTILITY_SERVICES;
                                db.Entry(MobServ).State = System.Data.Entity.EntityState.Modified;
                                //db.SaveChanges();
                            }
                            else if (Serlst.SERVICE_ID == 3)
                            {
                                var MobServ = db.TBL_WHITELABLE_SERVICE.FirstOrDefault(x => x.MEMBER_ID == objCredit.FROM_MEM_ID && x.SERVICE_ID == 3);
                                MobServ.CREDIT_LIMITSTATUS = objCredit.DMR;
                                db.Entry(MobServ).State = System.Data.Entity.EntityState.Modified;
                                //db.SaveChanges();
                            }
                            else if (Serlst.SERVICE_ID == 4)
                            {
                                var MobServ = db.TBL_WHITELABLE_SERVICE.FirstOrDefault(x => x.MEMBER_ID == objCredit.FROM_MEM_ID && x.SERVICE_ID == 4);
                                MobServ.CREDIT_LIMITSTATUS = objCredit.AIR_TICKET;
                                db.Entry(MobServ).State = System.Data.Entity.EntityState.Modified;
                                //db.SaveChanges();
                            }
                            else if (Serlst.SERVICE_ID == 5)
                            {
                                var MobServ = db.TBL_WHITELABLE_SERVICE.FirstOrDefault(x => x.MEMBER_ID == objCredit.FROM_MEM_ID && x.SERVICE_ID == 5);
                                MobServ.CREDIT_LIMITSTATUS = objCredit.BUS_TICKET;
                                db.Entry(MobServ).State = System.Data.Entity.EntityState.Modified;
                                //db.SaveChanges();
                            }
                            else if (Serlst.SERVICE_ID == 6)
                            {
                                var MobServ = db.TBL_WHITELABLE_SERVICE.FirstOrDefault(x => x.MEMBER_ID == objCredit.FROM_MEM_ID && x.SERVICE_ID == 6);
                                MobServ.CREDIT_LIMITSTATUS = objCredit.HOTEL_BOOKING;
                                db.Entry(MobServ).State = System.Data.Entity.EntityState.Modified;
                                //db.SaveChanges();
                            }
                            else if (Serlst.SERVICE_ID == 7)
                            {
                                var MobServ = db.TBL_WHITELABLE_SERVICE.FirstOrDefault(x => x.MEMBER_ID == objCredit.FROM_MEM_ID && x.SERVICE_ID == 7);
                                MobServ.CREDIT_LIMITSTATUS = objCredit.RAIL_UTILITY;
                                db.Entry(MobServ).State = System.Data.Entity.EntityState.Modified;
                                //db.SaveChanges();
                            }
                        }


                        decimal CreditLimitAmt = 0;
                        decimal.TryParse(objCredit.CREDIT_LIMIT_AMOUNT.ToString(), out CreditLimitAmt);
                        decimal MerchantMainBal = 0;
                        decimal DistributorMainBal = 0;
                        decimal DistributorCreditLimit = 0;
                        decimal SUBDistributorCreditLimit = 0;
                        decimal AddMerchantMainBal = 0;
                        decimal MerchantCreditLimit = 0;
                        decimal AddMerchantCreditLimit = 0;

                        decimal SubDistributorMainBal = 0;
                        decimal MerchantClosingAmt = 0;
                        decimal DistributorClosingAmt = 0;
                        decimal MerchantAddClosingAmt = 0;
                        decimal DistributorSubClosingAmt = 0;
                        string COrelationID = Settings.GetUniqueKey(MemberCurrentUser.MEM_ID.ToString());
                        //var distributorAcnt = db.TBL_ACCOUNTS.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        var distributorAcnt = db.TBL_ACCOUNTS.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).OrderByDescending(z => z.ACC_NO).FirstOrDefault();
                        if (distributorAcnt != null)
                        {
                            decimal.TryParse(distributorAcnt.CLOSING.ToString(), out DistributorClosingAmt);
                        }
                        else
                        {
                            DistributorClosingAmt = 0;
                        }

                        DistributorSubClosingAmt = DistributorClosingAmt - CreditLimitAmt;
                        TBL_ACCOUNTS objMDistributorAcnt = new TBL_ACCOUNTS()
                        {
                            API_ID = 0,
                            MEM_ID = MemberCurrentUser.MEM_ID,
                            MEMBER_TYPE = "DISTRIBUTOR",
                            TRANSACTION_TYPE = "CREDIT LIMIT",
                            TRANSACTION_DATE = DateTime.Now,
                            TRANSACTION_TIME = DateTime.Now,
                            DR_CR = "DR",
                            AMOUNT = CreditLimitAmt,
                            NARRATION = objCredit.CREDIT_LIMIT_DIstription,
                            OPENING = DistributorClosingAmt,
                            CLOSING = DistributorSubClosingAmt,
                            REC_NO = 0,
                            COMM_AMT = 0,
                            GST = 0,
                            TDS = 0,
                            IPAddress = "",
                            SERVICE_ID = 0,
                            CORELATIONID = COrelationID

                        };
                        db.TBL_ACCOUNTS.Add(objMDistributorAcnt);
                        //db.SaveChanges();


                        decimal CR_Opening = 0;
                        decimal CR_Closinging = 0;
                        decimal ADD_CR_Closinging = 0;
                        var CreditLimit_Val = db.TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION.Where(x => x.FROM_MEM_ID == objCredit.FROM_MEM_ID).OrderByDescending(c => c.SLN).FirstOrDefault();
                        if (CreditLimit_Val != null)
                        {
                            CR_Opening = (decimal)CreditLimit_Val.CREDIT_OPENING;
                            CR_Closinging = (decimal)CreditLimit_Val.CREDITCLOSING;
                            ADD_CR_Closinging = CR_Closinging + CreditLimitAmt;
                        }
                        else
                        {
                            CR_Closinging = 0;
                            ADD_CR_Closinging = CreditLimitAmt;
                        }

                        TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION objMerLimit = new TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION()
                        {
                            TO_MEM_ID = MemberCurrentUser.MEM_ID,
                            FROM_MEM_ID = objCredit.FROM_MEM_ID,
                            CREDIT_DATE = DateTime.Now,
                            //CREDIT_AMOUNT = objCredit.CREDIT_AMOUNT,
                            CREDIT_AMOUNT = CreditLimitAmt,
                            GST_VAL = 0,
                            GST_AMOUNT = 0,
                            TDS_VAL = 0,
                            TDS_AMOUNT = 0,
                            CREDIT_NOTE_DESCRIPTION = objCredit.CREDIT_LIMIT_DIstription,
                            CREDIT_STATUS = true,
                            CREDIT_OPENING = CR_Closinging,
                            CREDITCLOSING = ADD_CR_Closinging,
                            CREDIT_TRN_TYPE = "CR",
                            CORELATIONID = COrelationID
                        };
                        db.TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION.Add(objMerLimit);
                        // db.SaveChanges();
                        var DistributorInfo = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == MemberCurrentUser.MEM_ID);
                        decimal.TryParse(DistributorInfo.BALANCE.ToString(), out DistributorMainBal);
                        decimal.TryParse(DistributorInfo.CREDIT_LIMIT.ToString(), out DistributorCreditLimit);
                        SubDistributorMainBal = DistributorMainBal - CreditLimitAmt;
                        SUBDistributorCreditLimit = DistributorCreditLimit - CreditLimitAmt;
                        DistributorInfo.BALANCE = SubDistributorMainBal;
                        //DistributorInfo.CREDIT_LIMIT = SUBDistributorCreditLimit;
                        db.Entry(DistributorInfo).State = System.Data.Entity.EntityState.Modified;
                        //db.SaveChanges();

                        var MerchantInfo = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == objCredit.FROM_MEM_ID);
                        decimal.TryParse(MerchantInfo.BALANCE.ToString(), out MerchantMainBal);
                        decimal.TryParse(MerchantInfo.CREDIT_LIMIT.ToString(), out MerchantCreditLimit);
                        AddMerchantMainBal = MerchantMainBal + CreditLimitAmt;
                        AddMerchantCreditLimit = MerchantCreditLimit + CreditLimitAmt;
                        MerchantInfo.BALANCE = AddMerchantMainBal;
                        MerchantInfo.CREDIT_LIMIT = AddMerchantCreditLimit;
                        db.Entry(MerchantInfo).State = System.Data.Entity.EntityState.Modified;
                        // db.SaveChanges();

                        decimal MEr_OpeningAmt = 0;
                        //var MerchantAcount=db.TBL_ACCOUNTS.Where(x=>x.MEM_ID==objCredit.FROM_MEM_ID ).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        var MerchantAcount = db.TBL_ACCOUNTS.Where(x => x.MEM_ID == objCredit.FROM_MEM_ID).OrderByDescending(u => u.ACC_NO).FirstOrDefault();
                        if (MerchantAcount == null)
                        {
                            MerchantClosingAmt = CreditLimitAmt;
                            MEr_OpeningAmt = 0;
                            MerchantAddClosingAmt = MerchantClosingAmt;
                        }
                        else
                        {
                            decimal.TryParse(MerchantAcount.CLOSING.ToString(), out MEr_OpeningAmt);
                            decimal.TryParse(MerchantAcount.CLOSING.ToString(), out MerchantClosingAmt);
                            MerchantAddClosingAmt = MerchantClosingAmt + CreditLimitAmt;
                        }

                       
                        TBL_ACCOUNTS objMerchantAcnt = new TBL_ACCOUNTS()
                        {
                            API_ID = 0,
                            MEM_ID = objCredit.FROM_MEM_ID,
                            MEMBER_TYPE = "MERCHANT",
                            TRANSACTION_TYPE = "CREDIT LIMIT",
                            TRANSACTION_DATE = DateTime.Now,
                            TRANSACTION_TIME = DateTime.Now,
                            DR_CR = "CR",
                            AMOUNT = CreditLimitAmt,
                            NARRATION = objCredit.CREDIT_LIMIT_DIstription,
                            OPENING = MEr_OpeningAmt,
                            CLOSING = MerchantAddClosingAmt,
                            REC_NO = 0,
                            COMM_AMT = 0,
                            GST = 0,
                            TDS = 0,
                            IPAddress = "",
                            SERVICE_ID = 0,
                            CORELATIONID = COrelationID
                        };
                        db.TBL_ACCOUNTS.Add(objMerchantAcnt);
                        db.SaveChanges();

                        ContextTransaction.Commit();
                        return Json("Credit limit is given to merchant");
                    }
                    else
                    {
                        return Json("Distributor don't have sufficient balance to approve this transaction");
                    }
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    return Json("try again after some time");
                    throw ex;
                }
            }
        }

        public ActionResult GetMerchantReservedCreditLimit()
        {
            if (Session["DistributorUserId"] != null)
            {
                try
                {
                    initpage();
                    var db = new DBContext();
                    var memberrole = db.TBL_MASTER_MEMBER.Where(x => x.INTRODUCER == MemberCurrentUser.MEM_ID).ToList();
                    ViewBag.RoleDetails = new SelectList(memberrole, "MEM_ID", "UName");
                    return View();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            else
            {
                Session["DistributorUserId"] = null;
                Session["DistributorUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("DistributorUserId");
                Session.Remove("DistributorUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> PostReservedMerchantCREDITBALANCE(DISTReservedCreditLimitModel objCredit)
        {
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var SetReservedcreditLimit = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == objCredit.FROM_MEM_ID);
                    SetReservedcreditLimit.RESERVED_CREDIT_LIMIT = Convert.ToDecimal(objCredit.ReservedCreditLimit);
                    db.Entry(SetReservedcreditLimit).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    ContextTransaction.Commit();
                    return Json("Reserved limit is saved to merchant.");
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    return Json("try again after some time");
                    throw;
                }
            }

        }
        public ActionResult DistributorCreditLimitReport()
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
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }
        public PartialViewResult DistributorCreditindexgrid(string DateFrom = "", string Date_To = "")
        {
            try
            {
                var dbcontext = new DBContext();
                if (DateFrom != "" && Date_To != "")
                {
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    DateTime To_Date_Val = Date_To_Val.AddDays(1);
                    var memberinfo = (from tblcre in dbcontext.TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      join mem in dbcontext.TBL_MASTER_MEMBER on tblcre.TO_MEM_ID equals mem.MEM_ID
                                      where tblcre.TO_MEM_ID == MemberCurrentUser.MEM_ID && tblcre.CREDIT_DATE>= Date_From_Val && tblcre.CREDIT_DATE<= To_Date_Val
                                      select new
                                      {
                                          sln = tblcre.SLN,
                                          ToMember = tblcre.TO_MEM_ID,
                                          From_Member = tblcre.FROM_MEM_ID,
                                          Mem_Name = dbcontext.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == tblcre.FROM_MEM_ID).UName,
                                          Credit_note = tblcre.CREDIT_NOTE_DESCRIPTION,
                                          DR_CR = tblcre.CREDIT_AMOUNT,
                                          CreditNoteDate = tblcre.CREDIT_DATE,
                                          CreditAmount = tblcre.CREDIT_AMOUNT,
                                          creditStatus = tblcre.CREDIT_STATUS,
                                          OpeningAmt = tblcre.CREDIT_OPENING,
                                          Closingamt = tblcre.CREDITCLOSING,
                                          creditType = tblcre.CREDIT_TRN_TYPE
                                      }).AsEnumerable().Select(z => new TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      {
                                          SLN = z.sln,
                                          FromUser = z.Mem_Name,
                                          CREDIT_DATE = z.CreditNoteDate,
                                          //CREDIT_AMOUNT = z.CreditAmount,
                                          CREDIT_AMOUNT = z.DR_CR,
                                          CREDIT_NOTE_DESCRIPTION = z.Credit_note,
                                          CREDIT_STATUS = z.creditStatus,
                                          CREDITCLOSING = z.Closingamt,
                                          CR_Col = (z.creditType == "CR" ? z.CreditAmount.ToString() : "0"),
                                          DR_Col = (z.creditType == "DR" ? z.CreditAmount.ToString() : "0"),
                                          CREDIT_OPENING = z.OpeningAmt,
                                          CREDIT_TRN_TYPE = z.creditType
                                      }).ToList();
                    return PartialView("DistributorCreditindexgrid", memberinfo);
                }
                else
                {
                    DateTime NowDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                    DateTime Todaydate = Convert.ToDateTime(DateTime.Now.AddDays(1));
                    DateTime Today_date = Convert.ToDateTime(Todaydate.ToString("yyyy-MM-dd"));
                    var memberinfo = (from tblcre in dbcontext.TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      join mem in dbcontext.TBL_MASTER_MEMBER on tblcre.TO_MEM_ID equals mem.MEM_ID
                                      where tblcre.TO_MEM_ID == MemberCurrentUser.MEM_ID && tblcre.CREDIT_DATE >= NowDate && tblcre.CREDIT_DATE <= Today_date
                                      select new
                                      {
                                          sln = tblcre.SLN,
                                          ToMember = tblcre.TO_MEM_ID,
                                          From_Member = tblcre.FROM_MEM_ID,
                                          Mem_Name = dbcontext.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == tblcre.FROM_MEM_ID).UName,
                                          Credit_note = tblcre.CREDIT_NOTE_DESCRIPTION,
                                          DR_CR = tblcre.CREDIT_AMOUNT,
                                          CreditNoteDate = tblcre.CREDIT_DATE,
                                          CreditAmount = tblcre.CREDIT_AMOUNT,
                                          creditStatus = tblcre.CREDIT_STATUS,
                                          OpeningAmt = tblcre.CREDIT_OPENING,
                                          Closingamt = tblcre.CREDITCLOSING,
                                          creditType = tblcre.CREDIT_TRN_TYPE
                                      }).AsEnumerable().Select(z => new TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      {
                                          SLN = z.sln,
                                          FromUser = z.Mem_Name,
                                          CREDIT_DATE = z.CreditNoteDate,
                                          //CREDIT_AMOUNT = z.CreditAmount,
                                          CREDIT_AMOUNT = z.DR_CR,
                                          CREDIT_NOTE_DESCRIPTION = z.Credit_note,
                                          CREDIT_STATUS = z.creditStatus,
                                          CREDITCLOSING = z.Closingamt,
                                          CR_Col = (z.creditType == "CR" ? z.CreditAmount.ToString() : "0"),
                                          DR_Col = (z.creditType == "DR" ? z.CreditAmount.ToString() : "0"),
                                          CREDIT_OPENING = z.OpeningAmt,
                                          CREDIT_TRN_TYPE = z.creditType
                                      }).ToList();
                    return PartialView("DistributorCreditindexgrid", memberinfo);
                }
               
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public ActionResult DistributorCreditLimitList()
        {
            if (Session["DistributorUserId"] != null)
            {
                initpage();
                var db = new DBContext();
               
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
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }

        public PartialViewResult DistributorCreditReportindexgrid(string DateFrom = "", string Date_To = "")
        {
            try
            {
                var dbcontext = new DBContext();
                if (DateFrom != "" && Date_To != "")
                {
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    DateTime To_Date_Val = Date_To_Val.AddDays(1);
                    var Mem_info = dbcontext.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == MemberCurrentUser.MEM_ID).INTRODUCER;
                    var memberinfo = (from tblcre in dbcontext.TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      join mem in dbcontext.TBL_MASTER_MEMBER on tblcre.FROM_MEM_ID equals mem.MEM_ID
                                      where tblcre.TO_MEM_ID == Mem_info && tblcre.FROM_MEM_ID == MemberCurrentUser.MEM_ID && tblcre.CREDIT_DATE>= Date_From_Val && tblcre.CREDIT_DATE<= To_Date_Val
                                      select new
                                      {
                                          sln = tblcre.SLN,
                                          ToMember = tblcre.TO_MEM_ID,
                                          From_Member = tblcre.FROM_MEM_ID,
                                          Mem_Name = mem.UName,
                                          Credit_note = tblcre.CREDIT_NOTE_DESCRIPTION,
                                          DR_CR = tblcre.CREDIT_AMOUNT,
                                          CreditNoteDate = tblcre.CREDIT_DATE,
                                          CreditAmount = tblcre.CREDIT_AMOUNT,
                                          creditStatus = tblcre.CREDIT_STATUS,
                                          OpeningAmt = tblcre.CREDIT_OPENING,
                                          Closingamt = tblcre.CREDITCLOSING,
                                          creditType = tblcre.CREDIT_TRN_TYPE
                                      }).AsEnumerable().Select(z => new TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      {
                                          SLN = z.sln,
                                          FromUser = z.Mem_Name,
                                          CREDIT_DATE = z.CreditNoteDate,
                                          //CREDIT_AMOUNT = z.CreditAmount,
                                          CREDIT_AMOUNT = z.DR_CR,
                                          CREDIT_NOTE_DESCRIPTION = z.Credit_note,
                                          CREDIT_STATUS = z.creditStatus,
                                          CREDITCLOSING = z.Closingamt,
                                          CR_Col = (z.creditType == "CR" ? z.CreditAmount.ToString() : "0"),
                                          DR_Col = (z.creditType == "DR" ? z.CreditAmount.ToString() : "0"),
                                          CREDIT_OPENING = z.OpeningAmt,
                                          CREDIT_TRN_TYPE = z.creditType
                                      }).ToList();
                    return PartialView("DistributorCreditindexgrid", memberinfo);
                }
                else
                {
                    DateTime NowDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                    DateTime Todaydate = Convert.ToDateTime(DateTime.Now.AddDays(1));
                    DateTime Today_date = Convert.ToDateTime(Todaydate.ToString("yyyy-MM-dd"));
                    var Mem_info = dbcontext.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == MemberCurrentUser.MEM_ID).INTRODUCER;
                    var memberinfo = (from tblcre in dbcontext.TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      join mem in dbcontext.TBL_MASTER_MEMBER on tblcre.FROM_MEM_ID equals mem.MEM_ID
                                      where tblcre.TO_MEM_ID == Mem_info && tblcre.FROM_MEM_ID == MemberCurrentUser.MEM_ID && tblcre.CREDIT_DATE >= NowDate && tblcre.CREDIT_DATE <= Today_date
                                      select new
                                      {
                                          sln = tblcre.SLN,
                                          ToMember = tblcre.TO_MEM_ID,
                                          From_Member = tblcre.FROM_MEM_ID,
                                          Mem_Name = mem.UName,
                                          Credit_note = tblcre.CREDIT_NOTE_DESCRIPTION,
                                          DR_CR = tblcre.CREDIT_AMOUNT,
                                          CreditNoteDate = tblcre.CREDIT_DATE,
                                          CreditAmount = tblcre.CREDIT_AMOUNT,
                                          creditStatus = tblcre.CREDIT_STATUS,
                                          OpeningAmt = tblcre.CREDIT_OPENING,
                                          Closingamt = tblcre.CREDITCLOSING,
                                          creditType = tblcre.CREDIT_TRN_TYPE
                                      }).AsEnumerable().Select(z => new TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
                                      {
                                          SLN = z.sln,
                                          FromUser = z.Mem_Name,
                                          CREDIT_DATE = z.CreditNoteDate,
                                          //CREDIT_AMOUNT = z.CreditAmount,
                                          CREDIT_AMOUNT = z.DR_CR,
                                          CREDIT_NOTE_DESCRIPTION = z.Credit_note,
                                          CREDIT_STATUS = z.creditStatus,
                                          CREDITCLOSING = z.Closingamt,
                                          CR_Col = (z.creditType == "CR" ? z.CreditAmount.ToString() : "0"),
                                          DR_Col = (z.creditType == "DR" ? z.CreditAmount.ToString() : "0"),
                                          CREDIT_OPENING = z.OpeningAmt,
                                          CREDIT_TRN_TYPE = z.creditType
                                      }).ToList();
                    return PartialView("DistributorCreditindexgrid", memberinfo);
                }
                
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        [HttpPost]
        public JsonResult GetReservedCreditLimit(string Mem_ID)
        {
            initpage();////
            try
            {
                var db = new DBContext();
                long MEmber_Id = 0;
                long.TryParse(Mem_ID, out MEmber_Id); 
                var walletamount = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == MEmber_Id).FirstOrDefault();
                if (walletamount != null)
                {

                    return Json(walletamount, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("0", JsonRequestBehavior.AllowGet);
                }                
            }
            catch (Exception ex)
            {
                return Json("");
                throw ex;
            }
        }


        public ActionResult DeductMerchantCreditAmt()
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
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> PostDeductCREDITLimitBALANCESetting(DistributorCreditLimitSetting objCredit)
        {
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {

                try
                {
                    var DistributorBalanceCheckInfo = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == MemberCurrentUser.MEM_ID).BALANCE;
                    if (DistributorBalanceCheckInfo >= objCredit.CREDIT_LIMIT_AMOUNT)
                    {
                        decimal CreditLimitAmt = 0;
                        decimal.TryParse(objCredit.CREDIT_LIMIT_AMOUNT.ToString(), out CreditLimitAmt);
                        decimal MerchantMainBal = 0;
                        decimal DistributorMainBal = 0;
                        decimal DistributorCreditLimit = 0;
                        decimal SUBDistributorCreditLimit = 0;
                        decimal AddMerchantMainBal = 0;
                        decimal MerchantCreditLimit = 0;
                        decimal AddMerchantCreditLimit = 0;

                        decimal SubDistributorMainBal = 0;
                        decimal MerchantClosingAmt = 0;
                        decimal DistributorClosingAmt = 0;
                        decimal MerchantAddClosingAmt = 0;
                        decimal DistributorSubClosingAmt = 0;
                        string COrelationID = Settings.GetUniqueKey(MemberCurrentUser.MEM_ID.ToString());
                        //var distributorAcnt = db.TBL_ACCOUNTS.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        var distributorAcnt = db.TBL_ACCOUNTS.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).OrderByDescending(z => z.ACC_NO).FirstOrDefault();
                        if (distributorAcnt != null)
                        {
                            decimal.TryParse(distributorAcnt.CLOSING.ToString(), out DistributorClosingAmt);
                        }
                        else
                        {
                            DistributorClosingAmt = 0;
                        }

                        DistributorSubClosingAmt = DistributorClosingAmt + CreditLimitAmt;
                        TBL_ACCOUNTS objMDistributorAcnt = new TBL_ACCOUNTS()
                        {
                            API_ID = 0,
                            MEM_ID = MemberCurrentUser.MEM_ID,
                            MEMBER_TYPE = "DISTRIBUTOR",
                            TRANSACTION_TYPE = "DEDUCT CREDIT LIMIT",
                            TRANSACTION_DATE = DateTime.Now,
                            TRANSACTION_TIME = DateTime.Now,
                            DR_CR = "CR",
                            AMOUNT = CreditLimitAmt,
                            NARRATION = objCredit.CREDIT_LIMIT_DIstription,
                            OPENING = DistributorClosingAmt,
                            CLOSING = DistributorSubClosingAmt,
                            REC_NO = 0,
                            COMM_AMT = 0,
                            GST = 0,
                            TDS = 0,
                            IPAddress = "",
                            SERVICE_ID = 0,
                            CORELATIONID = COrelationID

                        };
                        db.TBL_ACCOUNTS.Add(objMDistributorAcnt);
                        //db.SaveChanges();


                        decimal CR_Opening = 0;
                        decimal CR_Closinging = 0;
                        decimal ADD_CR_Closinging = 0;
                        var CreditLimit_Val = db.TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION.Where(x => x.FROM_MEM_ID == objCredit.FROM_MEM_ID).OrderByDescending(c => c.SLN).FirstOrDefault();
                        if (CreditLimit_Val != null)
                        {
                            CR_Opening = (decimal)CreditLimit_Val.CREDIT_OPENING;
                            CR_Closinging = (decimal)CreditLimit_Val.CREDITCLOSING;
                            ADD_CR_Closinging = CR_Closinging - CreditLimitAmt;
                        }
                        else
                        {
                            CR_Closinging = 0;
                            ADD_CR_Closinging = CreditLimitAmt;
                        }

                        TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION objMerLimit = new TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION()
                        {
                            TO_MEM_ID = MemberCurrentUser.MEM_ID,
                            FROM_MEM_ID = objCredit.FROM_MEM_ID,
                            CREDIT_DATE = DateTime.Now,
                            //CREDIT_AMOUNT = objCredit.CREDIT_AMOUNT,
                            CREDIT_AMOUNT = CreditLimitAmt,
                            GST_VAL = 0,
                            GST_AMOUNT = 0,
                            TDS_VAL = 0,
                            TDS_AMOUNT = 0,
                            CREDIT_NOTE_DESCRIPTION = objCredit.CREDIT_LIMIT_DIstription,
                            CREDIT_STATUS = true,
                            CREDIT_OPENING = CR_Closinging,
                            CREDITCLOSING = ADD_CR_Closinging,
                            CREDIT_TRN_TYPE = "DR",
                            CORELATIONID = COrelationID
                        };
                        db.TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION.Add(objMerLimit);
                        // db.SaveChanges();
                        var DistributorInfo = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == MemberCurrentUser.MEM_ID);
                        decimal.TryParse(DistributorInfo.BALANCE.ToString(), out DistributorMainBal);
                        decimal.TryParse(DistributorInfo.CREDIT_LIMIT.ToString(), out DistributorCreditLimit);
                        SubDistributorMainBal = DistributorMainBal + CreditLimitAmt;
                        SUBDistributorCreditLimit = DistributorCreditLimit + CreditLimitAmt;
                        DistributorInfo.BALANCE = SubDistributorMainBal;
                        //DistributorInfo.CREDIT_LIMIT = SUBDistributorCreditLimit;
                        db.Entry(DistributorInfo).State = System.Data.Entity.EntityState.Modified;
                        //db.SaveChanges();

                        var MerchantInfo = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == objCredit.FROM_MEM_ID);
                        decimal.TryParse(MerchantInfo.BALANCE.ToString(), out MerchantMainBal);
                        decimal.TryParse(MerchantInfo.CREDIT_LIMIT.ToString(), out MerchantCreditLimit);
                        AddMerchantMainBal = MerchantMainBal - CreditLimitAmt;
                        AddMerchantCreditLimit = MerchantCreditLimit - CreditLimitAmt;
                        MerchantInfo.BALANCE = AddMerchantMainBal;
                        MerchantInfo.CREDIT_LIMIT = AddMerchantCreditLimit;
                        db.Entry(MerchantInfo).State = System.Data.Entity.EntityState.Modified;
                        // db.SaveChanges();

                        decimal MEr_OpeningAmt = 0;
                        //var MerchantAcount=db.TBL_ACCOUNTS.Where(x=>x.MEM_ID==objCredit.FROM_MEM_ID ).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        var MerchantAcount = db.TBL_ACCOUNTS.Where(x => x.MEM_ID == objCredit.FROM_MEM_ID).OrderByDescending(u => u.ACC_NO).FirstOrDefault();
                        if (MerchantAcount == null)
                        {
                            MerchantClosingAmt = CreditLimitAmt;
                            MEr_OpeningAmt = 0;
                            MerchantAddClosingAmt = MerchantClosingAmt;
                        }
                        else
                        {
                            decimal.TryParse(MerchantAcount.CLOSING.ToString(), out MEr_OpeningAmt);
                            decimal.TryParse(MerchantAcount.CLOSING.ToString(), out MerchantClosingAmt);
                            MerchantAddClosingAmt = MerchantClosingAmt - CreditLimitAmt;
                        }


                        TBL_ACCOUNTS objMerchantAcnt = new TBL_ACCOUNTS()
                        {
                            API_ID = 0,
                            MEM_ID = objCredit.FROM_MEM_ID,
                            MEMBER_TYPE = "MERCHANT",
                            TRANSACTION_TYPE = "DEDUCT CREDIT LIMIT",
                            TRANSACTION_DATE = DateTime.Now,
                            TRANSACTION_TIME = DateTime.Now,
                            DR_CR = "DR",
                            AMOUNT = CreditLimitAmt,
                            NARRATION = objCredit.CREDIT_LIMIT_DIstription,
                            OPENING = MEr_OpeningAmt,
                            CLOSING = MerchantAddClosingAmt,
                            REC_NO = 0,
                            COMM_AMT = 0,
                            GST = 0,
                            TDS = 0,
                            IPAddress = "",
                            SERVICE_ID = 0,
                            CORELATIONID = COrelationID
                        };
                        db.TBL_ACCOUNTS.Add(objMerchantAcnt);
                        db.SaveChanges();

                        ContextTransaction.Commit();
                        return Json("Credit limit is deducted from merchant");
                    }
                    else
                    {
                        return Json("Distributor don't have sufficient balance to approve this transaction");
                    }
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    return Json("try again after some time");
                    throw ex;
                }
            }
        }


    }
}