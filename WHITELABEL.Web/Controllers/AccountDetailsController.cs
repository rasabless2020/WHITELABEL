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
    public class AccountDetailsController : Controller
    {
        // GET: AccountDetails
        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult IndexGrid()
        {
            try
            {
                //var db = new DBContext();
                //var accountstatement = (from mem in db.TBL_MASTER_MEMBER
                //                        join ac in db.TBL_ACCOUNTS on mem.MEM_ID equals ac.MEM_ID
                //                        join role in db.TBL_MASTER_MEMBER_ROLE on mem.MEMBER_ROLE equals role.ROLE_ID
                //                        select new
                //                        {
                //                            sln = ac.ACC_NO,
                //                            username = mem.UName,
                //                            memberType = ac.MEMBER_TYPE,
                //                            transtype = ac.TRANSACTION_TYPE,
                //                            transdate = ac.TRANSACTION_DATE,
                //                            transtime = ac.TRANSACTION_TIME,
                //                            TransDR_CR = ac.DR_CR,
                //                            amount = ac.AMOUNT,
                //                            Opening = ac.OPENING,
                //                            Closing = ac.CLOSING,
                //                            RecNo = ac.REC_NO,
                //                            Narration = ac.NARRATION
                //                        }).AsEnumerable().Select(z => new TBL_ACCOUNTS()
                //                        {
                //                            ACC_NO = z.sln,
                //                            UserName = z.username,
                //                            MEMBER_TYPE = z.memberType,
                //                            TRANSACTION_TYPE = z.transtype,
                //                            TRANSACTION_DATE = z.transdate,
                //                            timevalue = z.transtime.ToString("HH:mm:ss"),
                //                            DR_CR = z.TransDR_CR,
                //                            AMOUNT = z.amount,
                //                            OPENING = z.Opening,
                //                            CLOSING = z.Closing,
                //                            REC_NO = z.RecNo,
                //                            NARRATION = z.Narration
                //                        }).ToList();

                //return PartialView(accountstatement);
                return PartialView(CreateExportableGrid());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult CompanyAccountStatement()
        {
            //return View();
            return View();
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
                IGrid<TBL_ACCOUNTS> grid = CreateExportableGrid();
                ExcelWorksheet sheet = package.Workbook.Worksheets["Data"];

                foreach (IGridColumn column in grid.Columns)
                {
                    sheet.Cells[1, col].Value = column.Title;
                    sheet.Column(col++).Width = 18;
                }

                foreach (IGridRow<TBL_ACCOUNTS> gridRow in grid.Rows)
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

        private IGrid<TBL_ACCOUNTS> CreateExportableGrid()
        {
            var db = new DBContext();
            var accountstatement = (from mem in db.TBL_MASTER_MEMBER
                                    join ac in db.TBL_ACCOUNTS on mem.MEM_ID equals ac.MEM_ID
                                    join role in db.TBL_MASTER_MEMBER_ROLE on mem.MEMBER_ROLE equals role.ROLE_ID
                                    select new
                                    {
                                        sln = ac.ACC_NO,
                                        username = mem.UName,
                                        memberType = ac.MEMBER_TYPE,
                                        transtype = ac.TRANSACTION_TYPE,
                                        transdate = ac.TRANSACTION_DATE,
                                        transtime = ac.TRANSACTION_TIME,
                                        TransDR_CR = ac.DR_CR,
                                        amount = ac.AMOUNT,
                                        Opening = ac.OPENING,
                                        Closing = ac.CLOSING,
                                        RecNo = ac.REC_NO,
                                        Narration = ac.NARRATION,
                                        GST=ac.GST,
                                        TDS=ac.TDS
                                    }).AsEnumerable().Select(z => new TBL_ACCOUNTS()
                                    {
                                        ACC_NO = z.sln,
                                        UserName = z.username,
                                        MEMBER_TYPE = z.memberType,
                                        TRANSACTION_TYPE = z.transtype,
                                        TRANSACTION_DATE = z.transdate,
                                        timevalue = z.transtime.ToString("HH:mm:ss"),
                                        DR_CR = z.TransDR_CR,
                                        AMOUNT = z.amount,
                                        OPENING = z.Opening,
                                        CLOSING = z.Closing,
                                        REC_NO = z.RecNo,
                                        NARRATION = z.Narration,
                                        GST = z.GST,
                                        TDS = z.TDS
                                    }).ToList();


            IGrid<TBL_ACCOUNTS> grid = new Grid<TBL_ACCOUNTS>(accountstatement);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.ACC_NO).Titled("Trans Id");
            grid.Columns.Add(model => model.UserName).Titled("User Name");
            grid.Columns.Add(model => model.MEMBER_TYPE).Titled("Member");

            grid.Columns.Add(model => model.TRANSACTION_TYPE).Titled("Trans Type");
            grid.Columns.Add(model => model.AMOUNT).Titled("AMOUNT");
            grid.Columns.Add(model => model.TRANSACTION_DATE).Titled("Trans Date").Formatted("{0:d}").MultiFilterable(true); 
            grid.Columns.Add(model => model.timevalue).Titled("Time").Formatted("{0:d}").MultiFilterable(true); ;
            grid.Columns.Add(model => model.DR_CR).Titled("DR_CR");
            grid.Columns.Add(model => model.AMOUNT).Titled("AMOUNT");
            grid.Columns.Add(model => model.OPENING).Titled("OPENING");
            grid.Columns.Add(model => model.CLOSING).Titled("CLOSING");

            grid.Pager = new GridPager<TBL_ACCOUNTS>(grid);
            grid.Processors.Add(grid.Pager);
            grid.Pager.RowsPerPage = 6;

            foreach (IGridColumn column in grid.Columns)
            {
                column.Filter.IsEnabled = true;
                column.Sort.IsEnabled = true;
            }

            return grid;
        }

    }
}