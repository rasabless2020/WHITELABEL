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
using WHITELABEL.Web.Controllers;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;

namespace WHITELABEL.Web.Areas.Super.Controllers
{
    [Authorize]
    public class SuperHostingController : SuperBaseController
    {
        public void initpage()
        {
            try
            {
                ViewBag.ControllerName = "Super Hosting";
              
                if (Session["SuperDistributorId"] == null)
                {
                    //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
                    //Response.Redirect(Url.Action("Index", "StockistDashboard", new { area = "SuperStockist" }));
                    Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
                    return;
                }
                bool Islogin = false;

                if (Session["SuperDistributorId"] != null)
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
        // GET: Super/SuperHosting
        public ActionResult Index()
        {
            if (Session["SuperDistributorId"] != null)
            {
                initpage();
                return View();
            }
            else
            {
                Session["SuperDistributorId"] = null;
                Session["SuperDistributorUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("SuperDistributorId");
                Session.Remove("SuperDistributorUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "SuperLogin", new { area = "Super" });
            }
            
        }
        public PartialViewResult IndexGrid()
        {
            try
            {
                var dbcontext = new DBContext();
                //var memberinfo = (from x in dbcontext.TBL_WHITE_LEVEL_HOSTING_DETAILS join
                //                  y in dbcontext.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                //                  select new
                //                  {
                //                      MemberName=y.UName,
                //                      Company_Name=x.COMPANY_NAME,
                //                      Domain=x.DOMAIN,
                //                      LongCode=x.LONG_CODE,
                //                      SLN=x.SLN
                //                  }).AsEnumerable().Select(z => new TBL_WHITE_LEVEL_HOSTING_DETAILS
                //                  {
                //                      SLN=z.SLN,
                //                      COMPANY_NAME=z.Company_Name,
                //                      memberName=z.MemberName,
                //                      DOMAIN=z.Domain,
                //                      LONG_CODE=z.LongCode
                //                  }).ToList();
                //// Only grid query values will be available here.
                //return PartialView("IndexGrid", memberinfo);
                return PartialView(CreateExportableGrid());
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<ActionResult> HostingDetails(string memid = "")
        {
            if (Session["SuperDistributorId"] != null)
            {
                try
                {
                    if (memid != null)
                    {
                        string decrptSlId = Decrypt.DecryptMe(memid);
                        long Memid = long.Parse(decrptSlId);
                        var db = new DBContext();
                        var membInfo = await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == Memid);
                        ViewBag.MemberId = memid;
                        ViewBag.membername = membInfo.MEMBER_NAME;
                        //var hostingdetails = new TBL_WHITE_LEVEL_HOSTING_DETAILS();
                        var hostingdetails = await db.TBL_WHITE_LEVEL_HOSTING_DETAILS.FirstOrDefaultAsync(x => x.MEM_ID == Memid);
                        if (hostingdetails != null)
                        {
                            ViewBag.checkbtn = "1";
                            hostingdetails.MEM_ID = Memid;

                            return View(hostingdetails);
                        }
                        else
                        {
                            var hostingdetails1 = new TBL_WHITE_LEVEL_HOSTING_DETAILS();
                            ViewBag.checkbtn = "0";
                            hostingdetails1.MEM_ID = Memid;

                            return View(hostingdetails1);
                        }

                    }
                    return View();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                Session["SuperDistributorId"] = null;
                Session["SuperDistributorUserName"] = null;
                Session["UserType"] = null;
                Session.Remove("SuperDistributorId");
                Session.Remove("SuperDistributorUserName");
                Session.Remove("UserType");
                return RedirectToAction("Index", "SuperLogin", new { area = "Super" });
            }
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> HostingDetails(TBL_WHITE_LEVEL_HOSTING_DETAILS value, HttpPostedFileBase MotoFile, HttpPostedFileBase LogoFile, HttpPostedFileBase BannerFile)
        {
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var checkhosting =await db.TBL_WHITE_LEVEL_HOSTING_DETAILS.Where(x => x.MEM_ID == value.MEM_ID).FirstOrDefaultAsync();
                    if (checkhosting != null)
                    {
                        string Motofilename = string.Empty;
                        string Logofilename = string.Empty;
                        string Bannerfilename = string.Empty;
                        if (MotoFile != null)
                        {
                            string Motopath = Path.GetFileName(MotoFile.FileName);
                            string MotofileName = Motopath.Substring(Motopath.LastIndexOf(((char)92)) + 1);
                            int index = MotofileName.LastIndexOf('.');
                            string onyName = MotofileName.Substring(0, index);
                            string fileExtension = MotofileName.Substring(index + 1);

                            var MotoFileName = "Moto-" + value.MEM_ID + "_" + value.LONG_CODE + "." + fileExtension;
                            var MotoServerSavePath = Path.Combine(Server.MapPath(@"/MemberHostingFiles/") + MotoFileName);
                            Motofilename = "/MemberHostingFiles/" + MotoFileName;
                            MotoFile.SaveAs(MotoServerSavePath);
                            checkhosting.MOTO = Motofilename;
                        }
                        if (LogoFile != null)
                        {
                            string Logopath = Path.GetFileName(LogoFile.FileName);
                            string LogofileName = Logopath.Substring(Logopath.LastIndexOf(((char)92)) + 1);
                            int index = LogofileName.LastIndexOf('.');
                            string onyName = LogofileName.Substring(0, index);
                            string fileExtension = LogofileName.Substring(index + 1);

                            var LogoFileName = "Logo-" + value.MEM_ID + "_" + value.LONG_CODE + "." + fileExtension;
                            var LogoServerSavePath = Path.Combine(Server.MapPath(@"/MemberHostingFiles/") + LogoFileName);
                            Logofilename = "/MemberHostingFiles/" + LogoFileName;
                            LogoFile.SaveAs(LogoServerSavePath);
                            checkhosting.LOGO = Logofilename;
                        }
                        if (BannerFile != null)
                        {
                            string Bannerpath = Path.GetFileName(BannerFile.FileName);
                            string BannerfileName = Bannerpath.Substring(Bannerpath.LastIndexOf(((char)92)) + 1);
                            int index = BannerfileName.LastIndexOf('.');
                            string onyName = BannerfileName.Substring(0, index);
                            string fileExtension = BannerfileName.Substring(index + 1);

                            var BannerFileName = "Banner-" + value.MEM_ID + "_" + value.LONG_CODE + "." + fileExtension;
                            var BannerServerSavePath = Path.Combine(Server.MapPath(@"/MemberHostingFiles/") + BannerFileName);
                            Bannerfilename = "/MemberHostingFiles/" + BannerFileName;
                            BannerFile.SaveAs(BannerServerSavePath);
                            checkhosting.BANNER = Bannerfilename;
                        }
                        checkhosting.COMPANY_NAME = value.COMPANY_NAME;
                        checkhosting.DOMAIN = value.DOMAIN;
                        checkhosting.LONG_CODE = value.LONG_CODE;
                        db.Entry(checkhosting).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        string Motofilename = string.Empty;
                        string Logofilename = string.Empty;
                        string Bannerfilename = string.Empty;
                        if (MotoFile != null)
                        {
                            string Motopath = Path.GetFileName(MotoFile.FileName);
                            string MotofileName = Motopath.Substring(Motopath.LastIndexOf(((char)92)) + 1);
                            int index = MotofileName.LastIndexOf('.');
                            string onyName = MotofileName.Substring(0, index);
                            string fileExtension = MotofileName.Substring(index + 1);

                            var MotoFileName = "Moto-" + value.MEM_ID + "_" + value.LONG_CODE + "." + fileExtension;
                            var MotoServerSavePath = Path.Combine(Server.MapPath(@"/MemberHostingFiles/") + MotoFileName);
                            Motofilename = "/MemberHostingFiles/" + MotoFileName;
                            MotoFile.SaveAs(MotoServerSavePath);
                            value.MOTO = Motofilename;
                        }
                        if (LogoFile != null)
                        {
                            string Logopath = Path.GetFileName(LogoFile.FileName);
                            string LogofileName = Logopath.Substring(Logopath.LastIndexOf(((char)92)) + 1);
                            int index = LogofileName.LastIndexOf('.');
                            string onyName = LogofileName.Substring(0, index);
                            string fileExtension = LogofileName.Substring(index + 1);

                            var LogoFileName = "Logo-" + value.MEM_ID + "_" + value.LONG_CODE + "." + fileExtension;
                            var LogoServerSavePath = Path.Combine(Server.MapPath(@"/MemberHostingFiles/") + LogoFileName);
                            Logofilename = "/MemberHostingFiles/" + LogoFileName;
                            LogoFile.SaveAs(LogoServerSavePath);
                            value.LOGO = Logofilename;
                        }
                        if (BannerFile != null)
                        {
                            string Bannerpath = Path.GetFileName(BannerFile.FileName);
                            string BannerfileName = Bannerpath.Substring(Bannerpath.LastIndexOf(((char)92)) + 1);
                            int index = BannerfileName.LastIndexOf('.');
                            string onyName = BannerfileName.Substring(0, index);
                            string fileExtension = BannerfileName.Substring(index + 1);

                            var BannerFileName = "Banner-" + value.MEM_ID + "_" + value.LONG_CODE + "." + fileExtension;
                            var BannerServerSavePath = Path.Combine(Server.MapPath(@"/MemberHostingFiles/") + BannerFileName);
                            Bannerfilename = "/MemberHostingFiles/" + BannerFileName;
                            BannerFile.SaveAs(BannerServerSavePath);
                            value.BANNER = Bannerfilename;
                        }
                        db.TBL_WHITE_LEVEL_HOSTING_DETAILS.Add(value);
                        await db.SaveChangesAsync();
                    }
                    ContextTransaction.Commit();
                    return RedirectToAction("Index", "StockistHosting", new { area = "SuperStockist" });
                    //}
                    //return View();
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    return RedirectToAction("Notfound", "ErrorHandler");
                }

            }

        }

        public async Task<ActionResult> UpdateHostingDetails(string memid = "")
        {

            try
            {

                if (memid != null)
                {
                    string decrptSlId = Decrypt.DecryptMe(memid);
                    long Memid = long.Parse(decrptSlId);
                    var db = new DBContext();
                    var HostingInfo = await db.TBL_WHITE_LEVEL_HOSTING_DETAILS.FirstOrDefaultAsync(x => x.SLN == Memid);
                    var MemberInfo = await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == HostingInfo.MEM_ID);
                    ViewBag.MemberId = memid;
                    ViewBag.membername = MemberInfo.MEMBER_NAME;
                    var hostingdetails = new TBL_WHITE_LEVEL_HOSTING_DETAILS();
                    hostingdetails.MEM_ID = Memid;
                    return View(HostingInfo);
                }
                else
                {
                    return View();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public async Task<ActionResult> UpdateHostingDetails(TBL_WHITE_LEVEL_HOSTING_DETAILS value, HttpPostedFileBase MotoFile, HttpPostedFileBase LogoFile, HttpPostedFileBase BannerFile)
        {
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {

                    long slno = long.Parse(value.SLN.ToString());
                    var hostinginfo = await db.TBL_WHITE_LEVEL_HOSTING_DETAILS.Where(x => x.SLN == slno).FirstOrDefaultAsync();
                    string Motofilename = string.Empty;
                    string Logofilename = string.Empty;
                    string Bannerfilename = string.Empty;
                    if (MotoFile != null)
                    {
                        var MotoFileName = value.MEM_ID + "_" + value.LONG_CODE + "_" + System.DateTime.Now + "_" + Path.GetFileName(MotoFile.FileName);
                        var MotoServerSavePath = Path.Combine(Server.MapPath("~/MemberHostingFiles/") + MotoFileName);
                        Motofilename = "~/MemberHostingFiles/" + MotoFileName;
                        MotoFile.SaveAs(MotoServerSavePath);
                        //value.MOTO = Motofilename;
                        hostinginfo.MOTO = Motofilename;
                    }
                    if (LogoFile != null)
                    {
                        var LogoFileName = value.MEM_ID + "_" + value.LONG_CODE + "_" + System.DateTime.Now + "_" + Path.GetFileName(LogoFile.FileName);
                        var LogoServerSavePath = Path.Combine(Server.MapPath("~/MemberHostingFiles/") + LogoFileName);
                        Logofilename = "~/MemberHostingFiles/" + LogoFileName;
                        LogoFile.SaveAs(LogoServerSavePath);
                        //value.LOGO = Logofilename;
                        hostinginfo.LOGO = Logofilename;
                    }
                    if (BannerFile != null)
                    {
                        var BannerFileName = value.MEM_ID + "_" + value.LONG_CODE + "_" + System.DateTime.Now + "_" + Path.GetFileName(BannerFile.FileName);
                        var BannerServerSavePath = Path.Combine(Server.MapPath("~/MemberHostingFiles/") + BannerFileName);
                        Bannerfilename = "~/MemberHostingFiles/" + BannerFileName;
                        BannerFile.SaveAs(BannerServerSavePath);
                        //value.BANNER = Bannerfilename;
                        hostinginfo.BANNER = Bannerfilename;
                    }
                    hostinginfo.COMPANY_NAME = value.COMPANY_NAME;
                    hostinginfo.DOMAIN = value.DOMAIN;
                    hostinginfo.LONG_CODE = value.LONG_CODE;
                    db.Entry(hostinginfo).State = System.Data.Entity.EntityState.Modified;
                    await db.SaveChangesAsync();
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

        [HttpGet]
        public FileResult ExportIndex()
        {
            // Using EPPlus from nuget
            using (ExcelPackage package = new ExcelPackage())
            {
                Int32 row = 2;
                Int32 col = 1;

                package.Workbook.Worksheets.Add("Data");
                IGrid<TBL_WHITE_LEVEL_HOSTING_DETAILS> grid = CreateExportableGrid();
                ExcelWorksheet sheet = package.Workbook.Worksheets["Data"];

                foreach (IGridColumn column in grid.Columns)
                {
                    sheet.Cells[1, col].Value = column.Title;
                    sheet.Column(col++).Width = 18;
                }

                foreach (IGridRow<TBL_WHITE_LEVEL_HOSTING_DETAILS> gridRow in grid.Rows)
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

        private IGrid<TBL_WHITE_LEVEL_HOSTING_DETAILS> CreateExportableGrid()
        {
            long userid = MemberCurrentUser.MEM_ID;
            var dbcontext = new DBContext();
            var memberinfo = (from x in dbcontext.TBL_WHITE_LEVEL_HOSTING_DETAILS
                              join
                              y in dbcontext.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                              where y.INTRODUCER == userid
                              select new
                              {
                                  MemberName = y.UName,
                                  Company_Name = x.COMPANY_NAME,
                                  Domain = x.DOMAIN,
                                  LongCode = x.LONG_CODE,
                                  SLN = x.SLN
                              }).AsEnumerable().Select(z => new TBL_WHITE_LEVEL_HOSTING_DETAILS
                              {
                                  SLN = z.SLN,
                                  COMPANY_NAME = z.Company_Name,
                                  memberName = z.MemberName,
                                  DOMAIN = z.Domain,
                                  LONG_CODE = z.LongCode
                              }).ToList();


            IGrid<TBL_WHITE_LEVEL_HOSTING_DETAILS> grid = new Grid<TBL_WHITE_LEVEL_HOSTING_DETAILS>(memberinfo);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.memberName).Titled("Member").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.COMPANY_NAME).Titled("Company").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.DOMAIN).Titled("Domain").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.LONG_CODE).Titled("Long Code").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.SLN).Titled("").Encoded(false).Filterable(false).Sortable(false)
                .RenderedAs(model => "<a href='" + @Url.Action("UpdateHostingDetails", "SuperHosting", new { area = "Super", memid = Encrypt.EncryptMe(model.SLN.ToString()) }) + "' class='btn btn-primary btn-xs'>Edit</a>");

            grid.Pager = new GridPager<TBL_WHITE_LEVEL_HOSTING_DETAILS>(grid);
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