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
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;

namespace WHITELABEL.Web.Areas.PowerAdmin.Controllers
{
    [Authorize]
    public class PowerAdminDebitCreditRequestController : PoweradminbaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        public void initpage()
        {
            try
            {
                ViewBag.ControllerName = "Power Admin Home";
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
        // GET: PowerAdmin/PowerAdminDebitCreditRequest
        public ActionResult Index()
        {
            try
            {
                initpage();
                return View();
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  PowerAdminDebitCreditRequest, method:- AddService(POST)  Line No:- 187", ex);
                return RedirectToAction("Exception", "ErrorHandler", new { area = "" });                
                throw ex;
            }            
        }
        [HttpPost]
        public async Task<ActionResult> Index(TBL_BALANCE_TRANSFER_LOGS obj_bal)
        {
            try
            {
                var db = new DBContext();
                var memid = db.TBL_WHITE_LEVEL_HOSTING_DETAILS.Where(x => x.DOMAIN == obj_bal.FromUser).FirstOrDefault();
                //var checkAvailableMember = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == memid.MEM_ID).FirstOrDefault();
                if (memid != null)
                {

                    var obj = obj_bal;
                    var mem_id = Request.Form["memberDomainId"].ToString();
                    long memberid = long.Parse(mem_id);

                    decimal closingamt = 0;
                    decimal Openingamt = 0;
                    decimal transamt = 0;
                    decimal AddAmount = 0;
                    var amtval = db.TBL_ACCOUNTS.Where(x => x.MEM_ID == memberid).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                    if (amtval != null)
                    {
                        Openingamt = amtval.OPENING;
                        closingamt = amtval.CLOSING;
                        transamt = amtval.AMOUNT;
                        if (obj_bal.PAYMENT_METHOD == "CR")
                        {
                            AddAmount = decimal.Parse(obj_bal.AMOUNT.ToString()) + closingamt;
                        }
                        else
                        {
                            AddAmount = closingamt - decimal.Parse(obj_bal.AMOUNT.ToString());
                        }
                    }
                    TBL_ACCOUNTS objaccnt = new TBL_ACCOUNTS()
                    {
                        API_ID = 0,
                        MEM_ID = memberid,
                        MEMBER_TYPE = "WHITE LEVEL",
                        TRANSACTION_TYPE = "Cash Deposit in bank",
                        TRANSACTION_DATE = DateTime.Now,
                        TRANSACTION_TIME = DateTime.Now,
                        DR_CR = obj_bal.PAYMENT_METHOD,
                        AMOUNT = obj_bal.AMOUNT,
                        NARRATION = obj_bal.TRANSACTION_DETAILS,
                        CLOSING = AddAmount,
                        OPENING = closingamt,
                        REC_NO = 0,
                        COMM_AMT = 0,
                        GST=0,
                        TDS=0
                    };
                    db.TBL_ACCOUNTS.Add(objaccnt);
                    db.SaveChanges();
                    var memberlist = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == memberid).FirstOrDefaultAsync();
                    memberlist.BALANCE = AddAmount;
                    db.Entry(memberlist).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();
                    return View("DisplayAccount");
                }
                else
                {
                    ViewBag.msg = "Please provide valid white level user name";
                    return View("Index");
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public ActionResult DisplayAccount()
        {
           
            return View();
        }
        public PartialViewResult indexgrid()
        {
            var db = new DBContext();
            //var listaccount = db.TBL_ACCOUNTS.Where(x => x.MEMBER_TYPE == "WHITE LEVEL" && (x.TRANSACTION_TYPE != "Mobile Recharge" && x.TRANSACTION_TYPE != "DMR")).ToList();
            var listaccount = (from x in db.TBL_ACCOUNTS
                               join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                               where x.MEMBER_TYPE == "WHITE LEVEL" && (x.TRANSACTION_TYPE != "Mobile Recharge" && x.TRANSACTION_TYPE != "DMR")
                               select new
                               {
                                   UserName = y.UName,
                                   MEM_ID = x.MEM_ID,
                                   MEMBER_TYPE = x.MEMBER_TYPE,
                                   API_ID = x.API_ID,
                                   Transaction_TYpe = x.TRANSACTION_TYPE,
                                   TransactionDate = x.TRANSACTION_DATE,
                                   Transactiontime = x.TRANSACTION_TIME,
                                   DR_CR = x.DR_CR,
                                   Narration = x.NARRATION,
                                   OpeningAmt = x.OPENING,
                                   ClosingAmt = x.CLOSING,
                                   Amount = x.AMOUNT,
                                   Rec_No = x.REC_NO,
                                   CommAmt = x.COMM_AMT
                               }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                               {
                                   SerialNo = index + 1,
                                   UserName=z.UserName,
                                   MEM_ID = z.MEM_ID,
                                   API_ID = z.API_ID,
                                   MEMBER_TYPE = z.MEMBER_TYPE,
                                   TRANSACTION_DATE = z.TransactionDate,
                                   TRANSACTION_TIME = z.Transactiontime,
                                   TRANSACTION_TYPE = z.Transaction_TYpe,
                                   DR_CR = z.DR_CR,
                                   AMOUNT = z.Amount,
                                   OPENING = z.OpeningAmt,
                                   CLOSING = z.ClosingAmt,
                                   REC_NO = z.Rec_No,
                                   COMM_AMT = z.CommAmt

                               }).ToList();
            return PartialView("indexgrid", listaccount);
        }
    }
}