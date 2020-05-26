using log4net;
using NonFactors.Mvc.Grid;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Controllers;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;

namespace WHITELABEL.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class MemberIssueDebitCreditNoteController : AdminBaseController
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

        // GET: Admin/MemberIssueDebitCreditNote
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
                var dbcontext = new DBContext();
                var memberinfo = (from x in dbcontext.TBL_DEBIT_CREDIT_SETTIING
                                  join y in dbcontext.TBL_DEBIT_CREDIT_NOTE_REMARK_SETTING on x.TRANS_REMARKS equals y.SLN
                                  where x.MEM_TYPE !=1 
                                  select new
                                  {
                                      SLN = x.SLN,
                                      TRANS_REF_NO = x.TRANS_REF_NO,
                                      MemberTypeName = dbcontext.TBL_MASTER_MEMBER_ROLE.Where(c => c.ROLE_ID == x.MEM_TYPE).Select(a => a.ROLE_NAME).FirstOrDefault(),
                                      WLP_NAme = dbcontext.TBL_MASTER_MEMBER.Where(c => c.MEM_ID == x.WLP_ID).Select(a => a.UName).FirstOrDefault(),
                                      TRANS_TYPE = x.TRANS_TYPE,
                                      TRANS_AMOUNT = x.TRANS_AMOUNT,
                                      RemarkName = y.CR_DR_NOTE,
                                      TRANS_DATE = x.TRANS_DATE
                                  }).AsEnumerable().Select(z => new TBL_DEBIT_CREDIT_SETTIING
                                  {
                                      SLN = z.SLN,
                                      TRANS_REF_NO = z.TRANS_REF_NO,
                                      MemberTypeName = z.MemberTypeName,
                                      WLPName = z.WLP_NAme,
                                      TRANS_TYPE = z.TRANS_TYPE,
                                      TRANS_AMOUNT = z.TRANS_AMOUNT,
                                      RemarkName = z.RemarkName,
                                      TRANS_DATE = z.TRANS_DATE
                                  }).ToList();

                var memberinfo12 = dbcontext.TBL_DEBIT_CREDIT_SETTIING.Where(x => x.MEM_TYPE == 1).ToList();
                return PartialView("IndexGrid", memberinfo);
                //var dbcontext = new DBContext();
                //var memberinfo = dbcontext.TBL_DEBIT_CREDIT_SETTIING.ToList();
                //return PartialView("IndexGrid", memberinfo);                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult ApplyIssueDebitCreditNote()
        {
            if (Session["WhiteLevelUserId"] != null)
            {
                initpage();
                var dbcontext = new DBContext();
                var memberrole = dbcontext.TBL_MASTER_MEMBER_ROLE.Where(x => x.ROLE_NAME != "WHITE LEVEL" && x.ROLE_NAME != "API USER" && x.ROLE_NAME != "SUPER DISTRIBUTOR").ToList();
                ViewBag.RoleDetails = new SelectList(memberrole, "ROLE_ID", "ROLE_NAME");
                var memberDistributor = dbcontext.TBL_MASTER_MEMBER.Where(x => x.MEMBER_ROLE ==4).ToList();
                ViewBag.GetDistributor = new SelectList(memberDistributor, "MEM_ID", "UName");
                var memberMerchant= dbcontext.TBL_MASTER_MEMBER.Where(x => x.MEMBER_ROLE == 5).ToList();
                ViewBag.GetMerchant = new SelectList(memberMerchant, "MEM_ID", "UName");
                var DebitCreditRemarks = dbcontext.TBL_DEBIT_CREDIT_NOTE_REMARK_SETTING.ToList();
                ViewBag.GetRemarks = new SelectList(DebitCreditRemarks, "SLN", "CR_DR_NOTE");
                
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
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> POSTApplyIssueDebitCreditNote(TBL_DEBIT_CREDIT_SETTIING objDRCR)
        {
            initpage();////
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    string COrelationID = Settings.GetUniqueKey(MemberCurrentUser.MEM_ID.ToString());
                    long MemberType = 0;
                    long.TryParse(objDRCR.MEM_TYPE.ToString(), out MemberType);
                    long Dist_Id = 0;
                    long Mer_Id = 0;
                    long DR_CR_ISSUED=0;
                    string MEM_ROLE = string.Empty;
                    decimal TDS_Val = 0;
                    decimal.TryParse(objDRCR.TDS_AMOUNT.ToString(), out TDS_Val);
                    decimal DeductTDS = 0;
                    decimal Remain_AMt = 0;
                    decimal Trans_Amt = 0;
                    decimal SGST = 0;
                    decimal CGST = 0;
                    decimal IGST = 0;
                    decimal SGST_VAL = 0;
                    decimal CGST_VAL = 0;
                    decimal IGST_VAL = 0;
                    decimal TotalGST = 0;
                    if (MemberType == 4)
                    {
                        MEM_ROLE = "DISTRIBUTOR";
                        long.TryParse(objDRCR.DIST_ID.ToString(), out DR_CR_ISSUED);
                        Dist_Id = DR_CR_ISSUED;
                    }
                    else
                    {
                        MEM_ROLE = "MERCHANT";
                        long.TryParse(objDRCR.MER_ID.ToString(), out DR_CR_ISSUED);
                        long.TryParse(objDRCR.DIST_ID.ToString(), out Dist_Id);
                    }
                    if (objDRCR.MER_ID == null)
                    {
                        Mer_Id = 0;
                    }
                    else
                    {
                        long.TryParse(objDRCR.MER_ID.ToString(), out Mer_Id);
                    }

                    var checkAvailableMember = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == DR_CR_ISSUED).FirstOrDefault();
                    if (checkAvailableMember != null)
                    {
                        var obj = objDRCR;
                        //var mem_id = Request.Form["memberDomainId"].ToString();
                        long memberid = DR_CR_ISSUED;

                        decimal closingamt = 0;
                        decimal Openingamt = 0;
                        decimal transamt = 0;
                        decimal AddAmount = 0;
                        decimal SubAmt = 0;
                        //var amtval = db.TBL_ACCOUNTS.Where(x => x.MEM_ID == memberid).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        var amtval = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == memberid).FirstOrDefault();
                        if (amtval != null)
                        {
                            var MemAcnt = db.TBL_ACCOUNTS.Where(x => x.MEM_ID == memberid).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                            if (MemAcnt != null)
                            {
                                Openingamt = MemAcnt.OPENING;
                                closingamt = MemAcnt.CLOSING;
                                transamt = MemAcnt.AMOUNT;
                            }
                            else
                            {
                                Openingamt = 0;
                                closingamt = 0;
                                transamt = 0;
                            }
                            decimal.TryParse(objDRCR.TRANS_AMOUNT.ToString(), out Trans_Amt);
                            if (objDRCR.TDS_APPLICABLE_VALUE == "1")
                            {
                                DeductTDS = (Trans_Amt * TDS_Val) / 100;
                                Remain_AMt = Trans_Amt - DeductTDS;
                            }
                            else
                            {
                                DeductTDS = 0;
                                Remain_AMt = Trans_Amt - DeductTDS;
                            }
                            if (objDRCR.TDS_APPLICABLE_VALUE == "1")
                            {
                                var GetGST = db.TBL_STATES.Where(x => x.STATEID == checkAvailableMember.STATE_ID).FirstOrDefault();
                                if (GetGST != null)
                                {
                                    decimal.TryParse(GetGST.SGST.ToString(), out SGST);
                                    decimal.TryParse(GetGST.CGST.ToString(), out CGST);
                                    decimal.TryParse(GetGST.IGST.ToString(), out IGST);
                                    SGST_VAL = (Remain_AMt * SGST) / 100;
                                    CGST_VAL = (Remain_AMt * CGST) / 100;
                                    IGST_VAL = (Remain_AMt * IGST) / 100;
                                    TotalGST = SGST_VAL + CGST_VAL + IGST_VAL;
                                }
                            }
                            else
                            {
                                    SGST = 0;
                                    CGST = 0;
                                    IGST = 0;
                                    SGST_VAL = 0;
                                    CGST_VAL = 0;
                                    IGST_VAL = 0;
                                    TotalGST = SGST_VAL + CGST_VAL + IGST_VAL;
                            }
                            if (objDRCR.TRANS_TYPE == "CR")
                            {
                                //decimal.TryParse(objDRCR.TRANS_AMOUNT.ToString(), out SubAmt);
                                decimal.TryParse(Remain_AMt.ToString(), out SubAmt);
                                
                                AddAmount = SubAmt + closingamt;
                            }
                            else
                            {
                                //decimal.TryParse(objDRCR.TRANS_AMOUNT.ToString(), out SubAmt);
                                decimal.TryParse(Remain_AMt.ToString(), out SubAmt);

                                AddAmount = Convert.ToDecimal(closingamt - SubAmt);
                            }
                        }
                        TBL_ACCOUNTS objaccnt = new TBL_ACCOUNTS()
                        {
                            API_ID = 0,
                            MEM_ID = memberid,
                            MEMBER_TYPE = MEM_ROLE,
                            TRANSACTION_TYPE = "Cash Deposit in bank",
                            TRANSACTION_DATE = DateTime.Now,
                            TRANSACTION_TIME = DateTime.Now,
                            DR_CR = objDRCR.TRANS_TYPE,
                            AMOUNT = objDRCR.TRANS_AMOUNT,
                            NARRATION = objDRCR.TRANS_DETAILS,
                            CLOSING = AddAmount,
                            OPENING = closingamt,
                            REC_NO = 0,
                            COMM_AMT = 0,
                            GST = (float)TotalGST,
                            TDS =(float)DeductTDS,
                            SERVICE_ID = 0,
                            CORELATIONID=COrelationID,
                        };
                        db.TBL_ACCOUNTS.Add(objaccnt);
                        db.SaveChanges();
                        var memberlist = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == memberid).FirstOrDefaultAsync();
                        memberlist.BALANCE = AddAmount;
                        db.Entry(memberlist).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();
                        bool IsGstApply = false;
                        if (objDRCR.GST_APPLICABLE_VALUE == "1")
                        {
                            IsGstApply = true;
                        }
                        else
                        { IsGstApply = false; }
                        bool IsTdsApply = false;
                        if (objDRCR.TDS_APPLICABLE_VALUE == "1")
                        {
                            IsTdsApply = true;
                        }
                        else
                        { IsTdsApply = false; }
                        TBL_DEBIT_CREDIT_SETTIING ObjDRCRval = new TBL_DEBIT_CREDIT_SETTIING()
                        {
                            CORELLATION_ID = COrelationID,
                            MER_ID = Mer_Id,
                            DIST_ID = Dist_Id,
                            WLP_ID = MemberCurrentUser.MEM_ID,
                            MEM_TYPE = objDRCR.MEM_TYPE,
                            TRANS_AGAINST = DR_CR_ISSUED,
                            TRANS_ISSUED_BY = MemberCurrentUser.MEM_ID,
                            TRANS_DONE_BY = MemberCurrentUser.MEM_ID,
                            TRANS_TYPE = objDRCR.TRANS_TYPE,
                            TRANS_AMOUNT = objDRCR.TRANS_AMOUNT,
                            TRANS_REF_NO = objDRCR.TRANS_REF_NO,
                            TRANS_DETAILS = objDRCR.TRANS_DETAILS,
                            TRANS_REMARKS = objDRCR.TRANS_REMARKS,
                            GST_APPLICABLE = IsGstApply,
                            TDS_APPLICABLE = IsTdsApply,
                            TDS_PER = objDRCR.TDS_PER,
                            TDS_AMOUNT = DeductTDS,
                            TRANS_DATE = DateTime.Now,
                            STATUS = true
                        };
                        db.TBL_DEBIT_CREDIT_SETTIING.Add(ObjDRCRval);
                        await db.SaveChangesAsync();
                        ContextTransaction.Commit();

                        //#region For WhiteLevel
                        //decimal WLOpening = 0;
                        //decimal WLClosing = 0;
                        //decimal WLAmtvalue = 0;
                        //string WLP_TranType = string.Empty;
                        //decimal WLPBal = 0;
                        //if (objDRCR.TRANS_TYPE == "CR")
                        //{
                        //    WLP_TranType = "DR";
                        //    WLAmtvalue = objDRCR.TRANS_AMOUNT;
                        //}
                        //else
                        //{
                        //    WLP_TranType = "CR";
                        //    WLAmtvalue = objDRCR.TRANS_AMOUNT;
                        //}
                        //var wlingo = (from x in db.TBL_MASTER_MEMBER
                        //              join y in db.TBL_MASTER_MEMBER_ROLE on x.MEMBER_ROLE equals y.ROLE_ID
                        //              where x.MEM_ID == MemberCurrentUser.MEM_ID
                        //              select new
                        //              {
                        //                  MemberType = y.ROLE_NAME
                        //              }).FirstOrDefault();
                        //var tbl_accnt = await db.TBL_ACCOUNTS.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefaultAsync();
                        //if (tbl_accnt != null)
                        //{
                        //    WLOpening = tbl_accnt.OPENING;
                        //    WLClosing = tbl_accnt.CLOSING;
                        //    if (objDRCR.TRANS_TYPE == "CR")
                        //    {
                        //        WLP_TranType = "DR";
                        //        WLAmtvalue = WLClosing - decimal.Parse(objDRCR.TRANS_AMOUNT.ToString());
                        //    }
                        //    else
                        //    {
                        //        WLP_TranType = "CR";
                        //        WLAmtvalue = WLClosing + decimal.Parse(objDRCR.TRANS_AMOUNT.ToString());
                        //    }
                        //    TBL_ACCOUNTS objWL = new TBL_ACCOUNTS()
                        //    {
                        //        API_ID = 0,
                        //        MEM_ID = MemberCurrentUser.MEM_ID,
                        //        MEMBER_TYPE = wlingo.MemberType,
                        //        TRANSACTION_TYPE = "Cash Deposit in bank",
                        //        TRANSACTION_DATE = DateTime.Now,
                        //        TRANSACTION_TIME = DateTime.Now,
                        //        DR_CR = WLP_TranType,
                        //        AMOUNT = objDRCR.TRANS_AMOUNT,
                        //        NARRATION = objDRCR.TRANS_DETAILS,
                        //        CLOSING = WLAmtvalue,
                        //        OPENING = 0,
                        //        REC_NO = 0,
                        //        COMM_AMT = 0,
                        //        GST = 0,
                        //        TDS = 0,
                        //        SERVICE_ID = 0,
                        //    };
                        //    db.TBL_ACCOUNTS.Add(objWL);
                        //    db.SaveChanges();
                        //    decimal AddWLPBal =0;

                        //    var WLBalance = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).FirstOrDefaultAsync();
                        //    decimal.TryParse(WLBalance.BALANCE.ToString(), out WLPBal);
                        //    if (objDRCR.TRANS_TYPE == "CR")
                        //    {
                        //        AddWLPBal = WLPBal - WLAmtvalue;
                        //    }
                        //    else
                        //    {
                        //        AddWLPBal = WLPBal + WLAmtvalue;
                        //    }
                        //    WLBalance.BALANCE = AddWLPBal;
                        //    db.Entry(WLBalance).State = System.Data.Entity.EntityState.Modified;
                        //    await db.SaveChangesAsync();
                        //}
                        //else
                        //{
                        //    TBL_ACCOUNTS objWL = new TBL_ACCOUNTS()
                        //    {
                        //        API_ID = 0,
                        //        MEM_ID = MemberCurrentUser.MEM_ID,
                        //        MEMBER_TYPE = wlingo.MemberType,
                        //        TRANSACTION_TYPE = "Cash Deposit in bank",
                        //        TRANSACTION_DATE = DateTime.Now,
                        //        TRANSACTION_TIME = DateTime.Now,
                        //        DR_CR = "",
                        //        AMOUNT = objDRCR.TRANS_AMOUNT,
                        //        NARRATION = objDRCR.TRANS_DETAILS,
                        //        CLOSING = WLAmtvalue,
                        //        OPENING = WLClosing,
                        //        REC_NO = 0,
                        //        COMM_AMT = 0,
                        //        GST = 0,
                        //        TDS = 0,
                        //        SERVICE_ID = 0,
                        //    };
                        //    db.TBL_ACCOUNTS.Add(objWL);
                        //    db.SaveChanges();
                        //    decimal AddWLPBal = 0;
                        //    var WLBalance = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == MemberCurrentUser.MEM_ID).FirstOrDefaultAsync();
                        //    decimal.TryParse(WLBalance.BALANCE.ToString(), out WLPBal);
                        //    if (objDRCR.TRANS_TYPE == "CR")
                        //    {
                        //        AddWLPBal = WLPBal - WLAmtvalue;
                        //    }
                        //    else
                        //    {
                        //        AddWLPBal = WLPBal + WLAmtvalue;
                        //    }
                        //    WLBalance.BALANCE = AddWLPBal;
                        //    db.Entry(WLBalance).State = System.Data.Entity.EntityState.Modified;
                        //    await db.SaveChangesAsync();
                        //}
                        

                        //ContextTransaction.Commit();
                        //#endregion

                        return Json("Debit Credit Note Issued successfully.",JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ContextTransaction.Rollback();
                        string Msg = string.Empty;
                        Msg= "Please provide valid super distributor user name";
                        return Json(Msg, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    string Msg = string.Empty;
                    Msg = "Try After Some Time";
                    return Json(Msg, JsonRequestBehavior.AllowGet);
                    throw;
                }
            }
                return Json("");
        }
        [HttpPost]
        public JsonResult GetAdminMerchant(long Disid)
        {            
            long dis_Id = long.Parse(Disid.ToString());
            var db = new DBContext();
            var memberService = (from x in db.TBL_MASTER_MEMBER
                                 where x.INTRODUCER == dis_Id
                                 select new
                                 {
                                     MEM_ID = x.MEM_ID,
                                     UName = x.UName
                                 }).AsEnumerable().Select(z => new MemberView
                                 {
                                     IDValue = z.MEM_ID.ToString(),
                                     TextValue = z.UName
                                 }).ToList().Distinct();
            return Json(memberService, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> CheckReferenceNo(string referenceno)
        {
            initpage();////
            var context = new DBContext();
            var User = await context.TBL_DEBIT_CREDIT_SETTIING.Where(model => model.TRANS_REF_NO == referenceno).FirstOrDefaultAsync();
            if (User != null)
            {                
                return Json(new { result = "unavailable" });                
            }
            else
            {
                return Json(new { result = "available" });
            }
        }
    }
}