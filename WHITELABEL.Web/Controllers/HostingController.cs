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
    [Authorize]
    public class HostingController : BaseController
    {
        public void initpage()
        {
            try
            {
                ViewBag.ControllerName = "Hosting Details";
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



        // GET: Hosting
        public ActionResult Index()
        {
            initpage();
            return View();
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
        public ActionResult HostingDetails(string memid = "")
        {
            try
            {
                if (memid != null)
                {
                    string decrptSlId = Decrypt.DecryptMe(memid);
                    long Memid = long.Parse(decrptSlId);
                    var db = new DBContext();
                    var membInfo = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == Memid);
                    ViewBag.MemberId = memid;
                    ViewBag.membername = membInfo.MEMBER_NAME;
                    //var hostingdetails = new TBL_WHITE_LEVEL_HOSTING_DETAILS();
                    var hostingdetails = db.TBL_WHITE_LEVEL_HOSTING_DETAILS.FirstOrDefault(x=>x.MEM_ID==Memid);
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
        [HttpPost]
        public ActionResult HostingDetails(TBL_WHITE_LEVEL_HOSTING_DETAILS value, HttpPostedFileBase MotoFile, HttpPostedFileBase LogoFile, HttpPostedFileBase BannerFile)
        {
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var checkhosting = db.TBL_WHITE_LEVEL_HOSTING_DETAILS.Where(x => x.MEM_ID == value.MEM_ID).FirstOrDefault();
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
                        db.SaveChanges();
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
                        db.SaveChanges();
                    }
                    ContextTransaction.Commit();
                    return RedirectToAction("Index");
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

        public ActionResult UpdateHostingDetails(string memid = "")
        {
            
                try
                {

                    if (memid != null)
                    {
                        string decrptSlId = Decrypt.DecryptMe(memid);
                        long Memid = long.Parse(decrptSlId);
                        var db = new DBContext();
                        var HostingInfo = db.TBL_WHITE_LEVEL_HOSTING_DETAILS.FirstOrDefault(x => x.SLN == Memid);
                        var MemberInfo = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == HostingInfo.MEM_ID);
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
        public ActionResult UpdateHostingDetails(TBL_WHITE_LEVEL_HOSTING_DETAILS value, HttpPostedFileBase MotoFile, HttpPostedFileBase LogoFile, HttpPostedFileBase BannerFile)
        {
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {

                    long slno = long.Parse(value.SLN.ToString());
                    var hostinginfo = db.TBL_WHITE_LEVEL_HOSTING_DETAILS.Where(x => x.SLN == slno).FirstOrDefault();
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
                    db.SaveChanges();
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
            var dbcontext = new DBContext();
            var memberinfo = (from x in dbcontext.TBL_WHITE_LEVEL_HOSTING_DETAILS
                              join
                              y in dbcontext.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
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
                .RenderedAs(model => "<a href='Hosting/UpdateHostingDetails?memid=" + Encrypt.EncryptMe(model.SLN.ToString()) + "' class='btn btn-primary btn-xs'>Edit</a>");
       
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