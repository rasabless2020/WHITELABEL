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


namespace WHITELABEL.Web.Areas.Merchant.Controllers
{
    public class MerchantDMRBankListController : MerchantBaseController
    {
        // GET: Merchant/MerchantDMRBankList
        public void initpage()
        {
            try
            {
                ViewBag.ControllerName = "Bank List";
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
        public ActionResult Index()
        {
            if (Session["MerchantUserId"] != null)
            {
                initpage();
                Session["MerchantDMRId"] = null;
                Session.Remove("MerchantDMRId");
                return View();
            }
            else
            {
                return RedirectToAction("Index", "MerchantLogin", new { area = "Merchant" });
            }
            
        }
        public PartialViewResult DMRBankList()
        {

            return PartialView(DisplayDMRBankList());
        }
        private IGrid<DMR_Bank_List> DisplayDMRBankList()
        {
            try
            {
                var db = new DBContext();
                //var remitterid = Session["MerchantDMRId"].ToString();
                var remitterid = "123456879";
                string accountNo = "7278526668";
                var PaymentValidation = MoneyTransferAPI.RemitterDetails(accountNo,"");
                List<DMR_Bank_List> objbanklist = new List<DMR_Bank_List>();
                if (PaymentValidation.data.beneficiary != null)
                {
                    foreach (var banklist in PaymentValidation.data.beneficiary)
                    {
                        DMR_Bank_List objval = new DMR_Bank_List()
                        {
                            ID=banklist.id,
                            Name=banklist.name,
                            BANK_IIN=banklist.account,
                            BANK_Sort_Name=banklist.mobile,
                            BANK_Name=banklist.bank,
                            BRANCK_Ifsc=banklist.ifsc,
                            IS_Down=banklist.imps
                        };
                        objbanklist.Add(objval);
                    }

                }
                var DMRTransaction = objbanklist.ToList();

                //var DMRTransaction = db.TBL_DMR_FUND_TRANSFER_DETAILS.Where(x => x.REMITTER_ID == remitterid).ToList();
                ////var bankdetails = db.TBL_SETTINGS_BANK_DETAILS.Where(x => x.ISDELETED == 0  && x.MEM_ID==MemberCurrentUser.MEM_ID).ToList();
                //var bankdetails = db.TBL_REMITTER_BENEFICIARY_INFO.Where(x => x.IsActive == 0 && x.RemitterID == remitterid).ToList();

                IGrid<DMR_Bank_List> grid = new Grid<DMR_Bank_List>(DMRTransaction);
                grid.ViewContext = new ViewContext { HttpContext = HttpContext };
                grid.Query = Request.QueryString;
                grid.Columns.Add(model => model.ID).Titled("ID").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.Name).Titled("NAME").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BANK_IIN).Titled("ACCOUNT").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BANK_Name).Titled("Bank Name").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BANK_Sort_Name).Titled("Bank Sort").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.BRANCK_Ifsc).Titled("IFSC").Filterable(true).Sortable(true);
                grid.Columns.Add(model => model.IS_Down).Titled("IMPS").Filterable(true).Sortable(true);

                //grid.Columns.Add(model => model.ID).Titled("").Encoded(false).Filterable(false).Sortable(false)
                //    .RenderedAs(model => "<a href='javascript:void(0)' class='btn btn-denger btn-xs' onclick='DeActivateBeneficiary(" + model.ID + ");return 0;'>DELETE</a>");
                grid.Pager = new GridPager<DMR_Bank_List>(grid);
                grid.Processors.Add(grid.Pager);
                grid.Pager.RowsPerPage = 6;

                //foreach (IGridColumn column in grid.Columns)
                //{
                //    column.Filter.IsEnabled = true;
                //    column.Sort.IsEnabled = true;
                //}

                return grid;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}