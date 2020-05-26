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
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;

namespace WHITELABEL.Web.Areas.PowerAdmin.Controllers
{
    [Authorize]
    public class PowerAdminBankDetailsController : PoweradminbaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        public void initpage()
        {
            try
            {
                ViewBag.ControllerName = "Power Bank Details";
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
        // GET: PowerAdmin/PowerAdminBankDetails
        public ActionResult Index()
        {
            if (Session["PowerAdminUserId"] != null)
            {
                initpage();
                return View();
            }
            else
            {
                Session["PowerAdminUserId"] = null;
                Session["PowerAdminUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("PowerAdminUserId");
                Session.Remove("PowerAdminUserName");
                Session.Remove("UserType");
                Response.Redirect(Url.Action("Index", "PowerAdminLogin", new { area = "PowerAdmin" }));
                return View();
            }
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
        public ActionResult BankDetails(string Bankid = "")
        {
            //return RedirectToAction("Notfound", "ErrorHandler");
            if (Session["PowerAdminUserId"] != null)
            {
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
                    Logger.Error("Controller:-  PowerAdminBankDetails(PowerAdmin), method:- BankDetails (GET) Line No:- 147", ex);
                    return RedirectToAction("Exception", "ErrorHandler", new { area = "" });
                    //return RedirectToAction("Notfound", "ErrorHandler");
                }
            }
            else
            {
                Session["PowerAdminUserId"] = null;
                Session["PowerAdminUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("PowerAdminUserId");
                Session.Remove("PowerAdminUserName");
                Session.Remove("UserType");
                Response.Redirect(Url.Action("Index", "PowerAdminLogin", new { area = "PowerAdmin" }));
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(true)]
        public async Task<ActionResult> BankDetails(TBL_SETTINGS_BANK_DETAILS objval)
        {
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var checkbankinfo = await db.TBL_SETTINGS_BANK_DETAILS.Where(x => x.SL_NO == objval.SL_NO).FirstOrDefaultAsync();
                    if (checkbankinfo == null)
                    {
                        objval.CREATED_DATE = DateTime.Now;
                        objval.CREATED_BY = CurrentUser.USER_ID;
                        objval.CREATED_BY = 0;
                        objval.MEM_ID = 0;
                        objval.ISDELETED = 0;
                        //objval.STATE = objval.STATENAME;
                        db.TBL_SETTINGS_BANK_DETAILS.Add(objval);
                        await db.SaveChangesAsync();
                        //return RedirectToAction("Index");
                    }
                    else
                    {
                        checkbankinfo.MEM_ID = 0;
                        checkbankinfo.BANK = objval.BANK;
                        checkbankinfo.ACCOUNT_HOLDERNAME = objval.ACCOUNT_HOLDERNAME;
                        checkbankinfo.IFSC = objval.IFSC;
                        checkbankinfo.MICR_CODE = objval.MICR_CODE;
                        checkbankinfo.CITY = objval.CITY;
                        checkbankinfo.BRANCH = objval.BRANCH;
                        checkbankinfo.CONTACT = objval.CONTACT;
                        checkbankinfo.STATE = objval.STATE;
                        //checkbankinfo.STATE = objval.STATENAME;
                        checkbankinfo.ADDRESS = objval.ADDRESS;
                        checkbankinfo.DISTRICT = objval.DISTRICT;
                        checkbankinfo.ACCOUNT_NO = objval.ACCOUNT_NO;
                        db.Entry(checkbankinfo).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();
                        //return RedirectToAction("Index");
                    }
                    ContextTransaction.Commit();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    Logger.Error("Controller:-  PowerAdminBankDetails(PowerAdmin), method:- BankDetails (POST) Line No:- 212", ex);
                    return RedirectToAction("Exception", "ErrorHandler", new { area = "" });
                    
                    throw ex;
                }
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeactivateBankDetails(string id)
        {
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {

                    long idval = long.Parse(id);
                    var bankdeactive = await db.TBL_SETTINGS_BANK_DETAILS.Where(x => x.SL_NO == idval).FirstOrDefaultAsync();
                    if (bankdeactive.ISDELETED == 1)
                    {
                        bankdeactive.ISDELETED = 0;
                        bankdeactive.DELETED_BY = 0;
                    }
                    else
                    {
                        bankdeactive.ISDELETED = 1;
                        bankdeactive.DELETED_BY = 0;
                    }
                    //bankdeactive.ISDELETED = 1;
                    bankdeactive.DELETED_DATE = System.DateTime.Now;
                   // bankdeactive.DELETED_BY = 0;
                    db.Entry(bankdeactive).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();
                    ContextTransaction.Commit();
                    return Json(new { Result = "true" });
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    Logger.Error("Controller:-  PowerAdminBankDetails(PowerAdmin), method:- DeactivateBankDetails (POST) Line No:- 243", ex);                    
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
            var bankdetails = db.TBL_SETTINGS_BANK_DETAILS.Where(x => x.ISDELETED == 0 && x.MEM_ID == 0).ToList();
            IGrid<TBL_SETTINGS_BANK_DETAILS> grid = new Grid<TBL_SETTINGS_BANK_DETAILS>(bankdetails);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.ACCOUNT_HOLDERNAME).Titled("Holder Name").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.BANK).Titled("Bank").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.IFSC).Titled("Ifsc").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.ACCOUNT_NO).Titled("Account No").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.BRANCH).Titled("Branch").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.CONTACT).Titled("Contact").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.SL_NO).Titled("Edit").Encoded(false).Filterable(false).Sortable(false)
                .RenderedAs(model => "<div style='text-align:center'> <a href='" + @Url.Action("BankDetails", "PowerAdminBankDetails", new { area = "PowerAdmin", Bankid = Encrypt.EncryptMe(model.SL_NO.ToString()) }) + "' title='Edit'><i class='fa fa-edit'></i></a></div>");
            grid.Columns.Add(model => model.SL_NO).Titled("Action").Encoded(false).Filterable(false).Sortable(false)
                .RenderedAs(model => "<div style='text-align:center'><a href='javascript:void(0)' onclick='DeActivateBank(" + model.SL_NO + ");return 0;' title='" + (model.ISDELETED == 1 ? "Deactive" : "Active") + "'>" + (model.ISDELETED == 1 ? "<span style='color:red;'><i class='fa fa-toggle-off fa-2x'></i></span>" : "<span style='color:green;'><i class='fa fa-toggle-on fa-2x'></i></span>") + "</a></div>");
            grid.Pager = new GridPager<TBL_SETTINGS_BANK_DETAILS>(grid);
            grid.Processors.Add(grid.Pager);
            grid.Pager.RowsPerPage = 6;
            return grid;
        }

        public ActionResult GetStateName(string query)
        {
            return Json(_GetState(query), JsonRequestBehavior.AllowGet);
        }

        private List<Autocomplete> _GetState(string query)
        {
            List<Autocomplete> people = new List<Autocomplete>();
            var db = new DBContext();
            try
            {
                var results = (from p in db.TBL_STATES
                               where (p.STATENAME).Contains(query) 
                               orderby p.STATENAME
                               select p).ToList();
                foreach (var r in results)
                {
                    // create objects
                    Autocomplete Username = new Autocomplete();

                    //Username.FromUser = string.Format("{0} {1}", r.UName);
                    Username.Name = (r.STATENAME);
                    Username.Id = r.STATEID;

                    people.Add(Username);
                }

            }
            catch (EntityCommandExecutionException eceex)
            {
                if (eceex.InnerException != null)
                {
                    throw eceex.InnerException;
                }
                throw;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return people;
        }



    }
}