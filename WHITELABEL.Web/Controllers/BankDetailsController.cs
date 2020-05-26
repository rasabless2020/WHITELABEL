using NonFactors.Mvc.Grid;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;

namespace WHITELABEL.Web.Controllers
{
    public class BankDetailsController : BaseController
    {
        public void initpage()
        {
            try
            {
                ViewBag.ControllerName = "Bank Details";
                SystemClass sclass = new SystemClass();
                string userID = sclass.GetLoggedUser();
                long userid = long.Parse(userID);
                var dbmain = new DBContext();
                if (userID != null && userID != "")
                {
                    TBL_AUTH_ADMIN_USER currUser = dbmain.TBL_AUTH_ADMIN_USERS.SingleOrDefault(c => c.USER_ID == userid && c.ACTIVE_USER == true);

                    if (currUser != null)
                    {
                        Session["UserId"] = currUser.USER_ID;
                        // Session["UserName"] = currUser.UserName;
                    }
                }
                if (Session["UserId"] == null)
                {
                    Response.Redirect("~/Login/LogOut");
                    return;
                }
                bool Islogin = false;

                if (Session["UserId"] != null)
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



        // GET: BankDetails
        public ActionResult Index()
        {
            initpage();
            return View();
        }
        public PartialViewResult IndexGrid()
        {
            try
            {
                //var db = new DBContext();
                //var bankdetails = db.TBL_SETTINGS_BANK_DETAILS.Where(x=>x.ISDELETED==0).ToList();
                //return PartialView("IndexGrid", bankdetails);   
                return PartialView(CreateExportableGrid());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
        public ActionResult BankDetails(string Bankid="")
        {
            //return RedirectToAction("Notfound", "ErrorHandler");
            try
            {
                var db = new DBContext();
                if (Bankid == "")
                {

                    // var memberrole = db.TBL_MASTER_MEMBER.ToList();
                    var memberService = (from x in db.TBL_MASTER_MEMBER
                                         select new
                                         {
                                             MEM_ID = x.MEM_ID,
                                             UName = x.MEMBER_NAME
                                         }).AsEnumerable().Select(z => new MemberView
                                         {
                                             IDValue = z.MEM_ID.ToString(),
                                             TextValue = z.UName
                                         }).ToList().Distinct();
                    ViewBag.MemberService = new SelectList(memberService, "IDValue", "TextValue");
                    ViewBag.checkbank = "0";
                    //var memberrole = db.TBL_MASTER_MEMBER.Where(x => x.IS_DELETED == false).ToList();
                    //ViewBag.MamberName = new SelectList(memberrole, "MEM_ID", "MEMBER_NAME");
                    return View();
                }
                else
                {
                    string decriptval = Decrypt.DecryptMe(Bankid.ToString());
                    long bankid = long.Parse(decriptval);
                    var memberService = (from x in db.TBL_MASTER_MEMBER
                                         select new
                                         {
                                             MEM_ID = x.MEM_ID,
                                             UName = x.MEMBER_NAME
                                         }).AsEnumerable().Select(z => new MemberView
                                         {
                                             IDValue = z.MEM_ID.ToString(),
                                             TextValue = z.UName
                                         }).ToList().Distinct();
                    ViewBag.MemberService = new SelectList(memberService, "IDValue", "TextValue");
                    var Bankinfo = db.TBL_SETTINGS_BANK_DETAILS.Where(x => x.SL_NO == bankid).FirstOrDefault();
                    Bankinfo.UserName = Bankinfo.MEM_ID.ToString();
                    ViewBag.checkbank = "1";
                    return View(Bankinfo);
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Notfound", "ErrorHandler");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BankDetails(TBL_SETTINGS_BANK_DETAILS objval)
        {
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {

                    var checkbankinfo = db.TBL_SETTINGS_BANK_DETAILS.Where(x => x.SL_NO == objval.SL_NO).FirstOrDefault();
                    if (checkbankinfo == null)
                    {
                        objval.CREATED_DATE = DateTime.Now;                        
                        objval.CREATED_BY = CurrentUser.USER_ID;                        
                        objval.CREATED_BY = 0;
                        objval.MEM_ID = 0;                       
                        objval.ISDELETED = 0;
                        db.TBL_SETTINGS_BANK_DETAILS.Add(objval);
                        db.SaveChanges();
                        //return RedirectToAction("Index");
                    }
                    else
                    {
                        checkbankinfo.MEM_ID = 0;
                        checkbankinfo.BANK = objval.BANK;
                        checkbankinfo.IFSC = objval.IFSC;
                        checkbankinfo.MICR_CODE = objval.MICR_CODE;
                        checkbankinfo.CITY = objval.CITY;
                        checkbankinfo.BRANCH = objval.BRANCH;
                        checkbankinfo.CONTACT = objval.CONTACT;
                        checkbankinfo.STATE = objval.STATE;
                        checkbankinfo.ADDRESS = objval.ADDRESS;
                        checkbankinfo.DISTRICT = objval.DISTRICT;
                        checkbankinfo.ACCOUNT_NO = objval.ACCOUNT_NO;
                        db.Entry(checkbankinfo).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        //return RedirectToAction("Index");
                    }
                    ContextTransaction.Commit();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    throw ex;
                }
            }
        }


        [HttpPost]
        public JsonResult DeactivateBankDetails(string id)
        {
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {

                    long idval = long.Parse(id);
                    var bankdeactive = db.TBL_SETTINGS_BANK_DETAILS.Where(x => x.SL_NO == idval).FirstOrDefault();
                    bankdeactive.ISDELETED = 1;
                    bankdeactive.DELETED_DATE = System.DateTime.Now;
                    bankdeactive.DELETED_BY = 0;
                    db.Entry(bankdeactive).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    ContextTransaction.Commit();
                    return Json(new { Result = "true" });
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    return Json(new { Result = "false" });
                }
            }
                            
        }



        [HttpGet]
        public FileResult ExportIndex()
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_SETTINGS_BANK_DETAILS> grid = CreateExportableGrid();
                ExcelWorksheet sheet = package.Workbook.Worksheets["Data"];

                foreach (IGridColumn column in grid.Columns)
                {
                    sheet.Cells[1, col].Value = column.Title;
                    sheet.Column(col++).Width = 18;
                }

                foreach (IGridRow<TBL_SETTINGS_BANK_DETAILS> gridRow in grid.Rows)
                {
                    col = 1;
                    foreach (IGridColumn column in grid.Columns)
                        sheet.Cells[row, col++].Value = column.ValueFor(gridRow);

                    row++;
                }

                return File(package.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                //return File(fileContents: package.GetAsByteArray(), contentType: "application/unknown");
            }
        }

        private IGrid<TBL_SETTINGS_BANK_DETAILS> CreateExportableGrid()
        {
            var db = new DBContext();
            var bankdetails = db.TBL_SETTINGS_BANK_DETAILS.Where(x => x.ISDELETED == 0 && x.MEM_ID== 0).ToList();
            

            IGrid<TBL_SETTINGS_BANK_DETAILS> grid = new Grid<TBL_SETTINGS_BANK_DETAILS>(bankdetails);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.BANK).Titled("BANK").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.IFSC).Titled("IFSC").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.ACCOUNT_NO).Titled("ACCOUNT NO").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.BRANCH).Titled("BRANCH").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.CONTACT).Titled("CONTACT").Filterable(true).Sortable(true);
            //grid.Columns.Add(model => model.MEM_ID).Filterable(false).Sortable(false).RenderedAs(m => @Url.Action("BankDetails", "BankDetails", new { Bankid = Encrypt.EncryptMe(m.SL_NO.ToString()) }));
            grid.Columns.Add(model => model.SL_NO).Titled("").Encoded(false).Filterable(false).Sortable(false)
                .RenderedAs(model => "<a href='BankDetails/BankDetails?Bankid="+Encrypt.EncryptMe(model.SL_NO.ToString()) + "' class='btn btn-primary btn-xs'>Edit</a>"); 
            grid.Columns.Add(model => model.SL_NO).Titled("").Encoded(false).Filterable(false).Sortable(false)
                .RenderedAs(model => "<a href='javascript:void(0)' class='btn btn-denger btn-xs' onclick='DeActivateBank(" + model.SL_NO + ");return 0;'>Deactivate</a>");           
            grid.Pager = new GridPager<TBL_SETTINGS_BANK_DETAILS>(grid);
            grid.Processors.Add(grid.Pager);
            grid.Pager.RowsPerPage = 6;

            //foreach (IGridColumn column in grid.Columns)
            //{
            //    column.Filter.IsEnabled = true;
            //    column.Sort.IsEnabled = true;
            //}

            return grid;
        }


    }
}