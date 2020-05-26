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
using System.Threading.Tasks;
using static WHITELABEL.Web.Helper.InstantPayApi;
using NonFactors.Mvc.Grid;
using log4net;

namespace WHITELABEL.Web.Areas.Merchant.Controllers
{
    
    public class MerchantDMRRegistrationController : MerchantBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        public void initpage()
        {
            try
            {
                ViewBag.ControllerName = "Merchant Dashboard";
                SystemClass sclass = new SystemClass();
                string userID = sclass.GetLoggedUser();
                long userid = long.Parse(userID);
                var dbmain = new DBContext();
                if (userID != null && userID != "")
                {
                    TBL_MASTER_MEMBER currUser = dbmain.TBL_MASTER_MEMBER.SingleOrDefault(c => c.MEM_ID == userid && c.MEMBER_ROLE == 5 && c.ACTIVE_MEMBER == true);
                    if (currUser != null)
                    {
                        Session["MerchantUserId"] = currUser.MEM_ID;
                        // Session["UserName"] = currUser.UserName;
                    }
                    else
                    {
                        Response.Redirect(Url.Action("Index", "MerchantLogin", new { area = "Merchant" }));
                        return;
                    }
                }
                if (Session["MerchantUserId"] == null)
                {
                    //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
                    Response.Redirect(Url.Action("Index", "MerchantLogin", new { area = "Merchant" }));
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

        // GET: Merchant/MerchantDMRRegistration
        public ActionResult Index()
        {
            if (Session["MerchantDMRId"] == null)
            {
                return View();
            }
            else
            {
                Session["MerchantDMRId"] = null;
                Session.Remove("MerchantDMRId");
                Response.Redirect(Url.Action("Index", "MerchantDMRLogin", new { area = "Merchant" }));
                return View();
            }
        }
        [HttpPost]
        public async Task<ActionResult> AddNewSender(DMRRegistrationViewModel objval)
        {
            try
            {
                const string agentId = "2";
                //string OperatorName = Request.Form["OperatorName"];
                //string operatorId = Request.Form["OperatorId"];
                long merchantid = 0;
                long.TryParse(Session["MerchantUserId"].ToString(), out merchantid);
                var db = new DBContext();
                var PaymentValidation = MoneyTransferAPI.RemitterRegistration(objval.MobileNo,objval.Name,"",objval.Pincode,"");
                if (PaymentValidation.statuscode == "TXN")
                {
                    var ipat_id = PaymentValidation.data.remitter.id;
                    var RemitterId = ipat_id.Value;
                    //double consumedlimit = double.Parse(Convert.ToDouble(PaymentValidation.data.remitter.consumedlimit.Value));
                    //double remaininglimit = double.Parse(Convert.ToDouble(PaymentValidation.data.remitter.remaininglimit.Value));
                    TBL_DMR_REMITTER_INFORMATION objremittar = new TBL_DMR_REMITTER_INFORMATION()
                    {
                        RemitterID = RemitterId,
                        Name = objval.Name,
                        MobileNo = objval.MobileNo,
                        Address = "",
                        Pincode = objval.Pincode,
                        City="",
                        State="",
                        KYCStatus=0,
                        ConsumedLimited=0,
                        RemainingLimit=0,
                        Status=0,
                        StatusCode="",
                        MEM_ID= merchantid,
                        InsertedDate=DateTime.Now,
                        UpdateStatus=0,
                        Total=0,
                        Perm_txn_limit=0,
                        KYCDocs=""

                    };
                    db.TBL_DMR_REMITTER_INFORMATION.Add(objremittar);
                    await db.SaveChangesAsync();
                    //TBL_API_RESPONSE_OUTPUT obj = new TBL_API_RESPONSE_OUTPUT()
                    //{
                    //    TXNID= RemitterId,
                    //    REQUESTID= RemitterId,
                    //    MOBILENO=objval.MobileNo,
                    //    STATUSID=0,
                    //    DESCRIPTION = "Remitter Registration",
                    //    AMOUNT =0,
                    //    BALANCE=0,
                    //    DATE=System.DateTime.Now,
                    //    OPREFNO="",
                    //    CREATEDATE=System.DateTime.Now,
                    //    MEM_ID=CurrentMerchant.MEM_ID,
                    //    STATUS=true,
                    //    RECHARGETYPE="DMR"
                    //};
                    //db.TBL_API_RESPONSE_OUTPUT.Add(obj);
                    //db.SaveChanges();
                    Session["MerchantDMRId"] = ipat_id.Value;
                    return RedirectToAction("DMRInformation", "MerchantDMRDashboard", new { area = "Merchant" });
                }
                else
                {
                    //return Json(PaymentValidation);
                    ViewBag.Message = "Invalid Credential or Access Denied";                    
                    return View();
                }
            }
            catch (Exception ex)
            {
                //throw ex;
                ViewBag.Message = "Invalid Credential or Access Denied";
                Logger.Error("Controller:-   MerchantDMRRegistration(Distributor), method:- AddNewSender (POST) Line No:- 158", ex);
                return RedirectToAction("Exception", "ErrorHandler", new { area = "" });
                //return View();
            }
        }



        public ActionResult SenderReverification()
        {
            return View();
        }
        public ActionResult DMRTransactionList()
        {
            //return View();
            if (Session["MerchantDMRId"] == null)
            {
                return View();
            }
            else
            {
                Session["MerchantDMRId"] = null;                
                Session.Remove("MerchantDMRId");
                Response.Redirect(Url.Action("Index", "MerchantDMRLogin", new { area = "Merchant" }));
                return View();
            }
        }

        public PartialViewResult PartialDMRList()
        {
            return PartialView(DMRTransaction());
        }
        private IGrid<TBL_DMR_FUND_TRANSFER_DETAILS> DMRTransaction()
        {
            try
            {
                var db = new DBContext();
                //var remitterid = Session["MerchantDMRId"].ToString();
                var DMRTransaction = db.TBL_DMR_FUND_TRANSFER_DETAILS.ToList();

                ////var bankdetails = db.TBL_SETTINGS_BANK_DETAILS.Where(x => x.ISDELETED == 0  && x.MEM_ID==MemberCurrentUser.MEM_ID).ToList();
                //var bankdetails = db.TBL_REMITTER_BENEFICIARY_INFO.Where(x => x.IsActive == 0 && x.RemitterID == remitterid).ToList();

                IGrid<TBL_DMR_FUND_TRANSFER_DETAILS> grid = new Grid<TBL_DMR_FUND_TRANSFER_DETAILS>(DMRTransaction);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.IPAY_ID).Titled("IPAY_ID").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.REF_NO).Titled("REF_NO").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.OPR_ID).Titled("OPR_ID").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.NAME).Titled("NAME").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.OPENING_BAL).Titled("OPENING_BAL").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.AMOUNT).Titled("AMOUNT").Filterable(true).Sortable(true);

                grid.Columns.Add(model => model.CHARGED_AMT).Titled("CHARGED_AMT").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.LOCKED_AMT).Titled("LOCKED_AMT").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BANK_ALIAS).Titled("BANK_ALIAS").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.TXNID).Titled("STATUS").Encoded(false).Filterable(false).Sortable(false)
                //    .RenderedAs(model => "<label class='label " + (model.STATUS == "Transaction Successful" ? "label-success" : "label-danger") +"'> "+ model.STATUS + " </label>");
                grid.Columns.Add(model => model.TXNID).Titled("STATUS").Encoded(false).Filterable(false).Sortable(false)
                    .RenderedAs(model => "<label class='label " + (model.STATUS == "Transaction Successful" ? "label-success" : model.STATUS == "PENDING" ? "label-danger" : "label-info") + "'> " + model.STATUS + " </label>");
                grid.Columns.Add(model => model.REQ_DATE).Titled("REQ_DATE").Filterable(true).Sortable(true).Formatted("{0:d}").MultiFilterable(true);
                grid.Columns.Add(model => model.REFUNF_ALLOWED).Titled("REFUNF_ALLOWED NAME").Filterable(true).Sortable(true);
                //grid.Columns.Add(model => model.ID).Titled("").Encoded(false).Filterable(false).Sortable(false)
                //    .RenderedAs(model => "<a href='javascript:void(0)' class='btn btn-denger btn-xs' onclick='DeActivateBeneficiary(" + model.ID + ");return 0;'>DELETE</a>");
                grid.Pager = new GridPager<TBL_DMR_FUND_TRANSFER_DETAILS>(grid);
                grid.Processors.Add(grid.Pager);
                grid.Pager.RowsPerPage = 6;
                grid.Rows.Sum(row => row.Model.AMOUNT);
                

                return grid;
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-   MerchantDMRRegistration(Distributor), method:- DMRTransaction (GET) Line No:- 232", ex);
                throw ex;
            }

        }

    }
}