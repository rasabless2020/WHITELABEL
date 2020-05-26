using log4net;
using NonFactors.Mvc.Grid;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WHITELABEL.Data;
using WHITELABEL.Data.Models;
using WHITELABEL.Web.Areas.Admin.Models;
using WHITELABEL.Web.Helper;
using WHITELABEL.Web.Models;

namespace WHITELABEL.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class MemberCommissionSlabController : AdminBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        // GET: Admin/MemberCommissionSlab
        //public void initpage()
        //{
        //    try
        //    {
        //        ViewBag.ControllerName = "Bank List";
        //        SystemClass sclass = new SystemClass();
        //        string userID = sclass.GetLoggedUser();
        //        long userid = long.Parse(userID);
        //        var dbmain = new DBContext();
        //        if (userID != null && userID != "")
        //        {
        //            TBL_MASTER_MEMBER currUser = dbmain.TBL_MASTER_MEMBER.SingleOrDefault(c => c.MEM_ID == userid && c.ACTIVE_MEMBER == true && c.MEMBER_ROLE == 1);
        //            if (currUser != null)
        //            {
        //                Session["WhiteLevelUserId"] = currUser.MEM_ID;
        //            }
        //            else
        //            {
        //                Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //                return;
        //            }
        //        }
        //        if (Session["WhiteLevelUserId"] == null)
        //        {
        //            Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
        //            return;
        //        }
        //        bool Islogin = false;
        //        if (Session["WhiteLevelUserId"] != null)
        //        {
        //            Islogin = true;
        //            ViewBag.CurrentUserId = MemberCurrentUser.MEM_ID;
        //        }
        //        ViewBag.Islogin = Islogin;
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.InnerException);
        //        return;
        //    }
        //}
        //public void initpage()
        //{
        //    try
        //    {
        //        if (DomainNameCheck.DomainChecking(Session["DOMAINNAME"].ToString(), Request.Url.Host) == false)
        //        {
        //            Response.Redirect(Url.Action("DomainError", "Login", new { area = "" }));
        //        }
        //        ViewBag.ControllerName = "White Label";
        //        if (Session["WhiteLevelUserId"] == null)
        //        {
        //            Response.Redirect(Url.Action("Logout", "Login", new { area = "" }));
        //            return;
        //        }
        //        bool Islogin = false;

        //        if (Session["WhiteLevelUserId"] != null)
        //        {
        //            Islogin = true;
        //            ViewBag.CurrentUserId = MemberCurrentUser.MEM_ID;
        //        }
        //        ViewBag.Islogin = Islogin;
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.InnerException);
        //        return;
        //    }
        //}
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
                Console.WriteLine(e.InnerException);
                return;
            }
        }
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
                var db = new DBContext();
                //var slabInfo = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.ToList();
                //return PartialView(slabInfo);
                return PartialView(CreateExportableGrid());
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        private IGrid<TBL_WHITE_LEVEL_COMMISSION_SLAB> CreateExportableGrid()
        {
            var db = new DBContext();
            //var CommissionSlab = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x=>x.MEM_ID!=0).ToList();
            var CommissionSlab = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.MEM_ID ==MemberCurrentUser.MEM_ID && x.SLAB_STATUS==true).ToList();
            IGrid<TBL_WHITE_LEVEL_COMMISSION_SLAB> grid = new Grid<TBL_WHITE_LEVEL_COMMISSION_SLAB>(CommissionSlab);
            grid.ViewContext = new ViewContext { HttpContext = HttpContext };
            grid.Query = Request.QueryString;
            grid.Columns.Add(model => model.SLAB_NAME).Titled("Slab Name").Filterable(true).Sortable(true);          
            grid.Columns.Add(model => model.SLAB_DETAILS).Titled("Slab  Details").Filterable(true).Sortable(true);
            grid.Columns.Add(model => (db.TBL_SETTINGS_SERVICES_MASTER.Where(x => x.SLN == model.SLAB_TYPE).Select(z => z.SERVICE_NAME).FirstOrDefault())).Titled("Slab Type").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.SLAB_STATUS == true ? "Active" : "Deactive").Titled("Status").Filterable(true).Sortable(true);
            grid.Columns.Add(model => model.SLN).Titled("Edit").Encoded(false)
                .RenderedAs(model => "<div style='text-align:center;'><a href='" + @Url.Action("GenerateCommissionSlab", "MemberCommissionSlab", new { area = "Admin", SlabId = Encrypt.EncryptMe(model.SLN.ToString()) }) + "' Title='Edit'><i class='fa fa-edit'></i></a>");
            grid.Columns.Add(model => model.SLN).Titled("Action").Encoded(false).Filterable(false).Sortable(false)
             .RenderedAs(model => "<div style='text-align:center;'><a href='javascript:void(0)' onclick='DeactivateStatus(" + model.SLN + ");return 0;'  title='" + (model.SLAB_STATUS == true ? "Active" : "Deactive") + "'>" + (model.SLAB_STATUS == true ? "<span style='color:green;'><i class='fa fa-toggle-on fa-2x'></i></span>" : "<span style='color:red;'><i class='fa fa-toggle-off fa-2x'></i></span>") + "</a></div>");
            grid.Pager = new GridPager<TBL_WHITE_LEVEL_COMMISSION_SLAB>(grid);
            grid.Processors.Add(grid.Pager);
            grid.Pager.RowsPerPage = 6;

            //foreach (IGridColumn column in grid.Columns)
            //{
            //    column.Filter.IsEnabled = true;
            //    column.Sort.IsEnabled = true;
            //}

            return grid;
        }

        public ActionResult AddCommissionSlab(string SlabId = "")
        {
            initpage();
            if (Session["WhiteLevelUserId"] != null)
            {
                try
                {
                    if (Request.QueryString["Operatortype"] != null)
                    {

                    }
                    if (SlabId != "")
                    {

                        string decriptval = Decrypt.DecryptMe(SlabId.ToString());
                        long SlabIdVal = long.Parse(decriptval);
                        Session["IDval"] = SlabIdVal;
                        var db = new DBContext();
                        ////var listSlab = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.FirstOrDefault(x => x.SLN == SlabIdVal);
                        //var listSlab = (from x in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                        //                join y in db.TBL_SETTINGS_SERVICES_MASTER on x.SLAB_TYPE equals y.SLN
                        //                where x.SLN == SlabIdVal
                        //                select new
                        //                {
                        //                    SLAB_NAME = x.SLAB_NAME,
                        //                    SLAB_DETAILS=x.SLAB_DETAILS,
                        //                    Slab_TypeName=y.SERVICE_NAME,
                        //                    SLAB_STATUS=x.SLAB_STATUS,
                        //                    SLAB_TYPE=x.SLAB_TYPE,
                        //                }).AsEnumerable().Select(z=> new TBL_WHITE_LEVEL_COMMISSION_SLAB
                        //                {
                        //                    SLAB_NAME=z.SLAB_NAME,
                        //                    SLAB_DETAILS=z.SLAB_DETAILS,
                        //                    Slab_TypeName=z.Slab_TypeName,
                        //                    SLAB_STATUS=z.SLAB_STATUS,
                        //                    SLAB_TYPE=z.SLAB_TYPE
                        //                }).FirstOrDefault();
                        //ViewBag.checkStatus = "1";
                        //var memberService = (from x in db.TBL_SETTINGS_SERVICES_MASTER
                        //                     select new
                        //                     {
                        //                         MEM_ID = x.SLN,
                        //                         UName = x.SERVICE_NAME
                        //                     }).AsEnumerable().Select(z => new MemberView
                        //                     {
                        //                         IDValue = Encrypt.EncryptMe(z.MEM_ID.ToString()),
                        //                         TextValue = z.UName
                        //                     }).ToList().Distinct();



                        //return View(listSlab);
                        return View();
                    }
                    else
                    {
                        Session["IDval"] = null;
                        Session["IDval"] = null;
                        Session.Remove("IDval");
                        Session.Remove("IDval");
                        Session["IDval"] = null;
                        Session["IDval"] = "";
                        Session.Remove("IDval");
                        //Session.Abandon();
                        CommissoinManagmentmodel objmodel = new CommissoinManagmentmodel();
                        objmodel.OperatorDetails = null;
                        ViewBag.checkStatus = "0";
                        return View();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("Controller:-  MemberCommissionSlab(Admin), method:- AddCommissionSlab (GET) Line No:- 194", ex);                    
                    return RedirectToAction("Exception", "ErrorHandler", new { area = "" });
                    throw ex;
                }
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
                return View();
            }
        }

        [HttpGet]
        public JsonResult FetchData(long idval)
        {
            initpage();////
            try
            {
                var db = new DBContext();
                if (Session["IDval"] != null && Session["IDval"] != "")
                {
                    idval = long.Parse(Session["IDval"].ToString());
                }
                else
                {
                    idval = 0;
                }
                string slabtypename = string.Empty;
                var slabtype = db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == idval).FirstOrDefault();
                if (slabtype != null)
                {
                    if (slabtype.SLAB_TYPE == 1)
                    {
                        slabtypename = "MOBILE RECHARGE";
                        var valuelist = (from slabmaster in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                         where slabmaster.SLN == idval
                                         select new
                                         {
                                             SLN = slabmaster.SLN,
                                             SLAB_NAME = slabmaster.SLAB_NAME,
                                             SLAB_DETAILS = slabmaster.SLAB_DETAILS,
                                             SLAB_TYPE = slabmaster.SLAB_TYPE,
                                             SLAB_STATUS = slabmaster.SLAB_STATUS,
                                             SLAB_TDS = slabmaster.SLAB_TDS,
                                             //ASSIGNED_SLAB=slabmaster.ASSIGNED_SLAB,
                                             OperatorDetails = (from slablist in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                                                where slablist.SLAB_ID == idval
                                                                select new
                                                                {
                                                                    ID = slablist.SLN,
                                                                    SERVICE_NAME = slablist.OPERATOR_NAME,
                                                                    TYPE = slablist.OPERATOR_TYPE,
                                                                    SERVICE_KEY = slablist.OPERATOR_CODE,
                                                                    CommissionType=slablist.COMMISSION_TYPE,
                                                                    CommissionPercentage = slablist.COMM_PERCENTAGE,
                                                                    SuperDisPercentage=slablist.SUPER_COM_PER,
                                                                    DistriCommissionPer=slablist.DISTRIBUTOR_COM_PER,
                                                                    RetailerCommissionPer=slablist.MERCHANT_COM_PER,
                                                                }).AsEnumerable().Select(z => new CommissionListView
                                                                {
                                                                    ID = z.ID,
                                                                    SERVICE_NAME = z.SERVICE_NAME,
                                                                    TYPE = z.TYPE,
                                                                    SERVICE_KEY = z.SERVICE_KEY,
                                                                    CommissionType=z.CommissionType,
                                                                    CommissionPercentage = z.CommissionPercentage.ToString(),
                                                                    SuperDisPercentage=z.SuperDisPercentage.ToString(),
                                                                    DistriCommissionPer=z.DistriCommissionPer.ToString(),
                                                                    RetailerCommissionPer=z.RetailerCommissionPer.ToString()
                                                                }).ToList()
                                         }).AsEnumerable().Select(d => new CommissoinManagmentmodel
                                         {
                                             SLN = d.SLN,
                                             SLAB_NAME = d.SLAB_NAME,
                                             SLAB_DETAILS = d.SLAB_DETAILS,
                                             Slab_TypeName = slabtypename,
                                             SLAB_STATUS = d.SLAB_STATUS,
                                             SLAB_TYPE = d.SLAB_TYPE,
                                             SLAB_TDS=d.SLAB_TDS,
                                             //ASSIGNED_SLAB = d.ASSIGNED_SLAB,
                                             OperatorDetails = d.OperatorDetails.OrderByDescending(c => c.TYPE).ToList()
                                         });
                        return Json(valuelist, JsonRequestBehavior.AllowGet);
                    }
                    else if (slabtype.SLAB_TYPE == 2)
                    {
                        slabtypename = "UTILITY SERVICES";
                        var valuelist = (from slabmaster in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                         where slabmaster.SLN == idval
                                         select new
                                         {
                                             SLN = slabmaster.SLN,
                                             SLAB_NAME = slabmaster.SLAB_NAME,
                                             SLAB_DETAILS = slabmaster.SLAB_DETAILS,
                                             SLAB_TYPE = slabmaster.SLAB_TYPE,
                                             //Slab_TypeName = slabmaster.Slab_TypeName,
                                             SLAB_STATUS = slabmaster.SLAB_STATUS,
                                             SLAB_TDS = slabmaster.SLAB_TDS,
                                             //ASSIGNED_SLAB = slabmaster.ASSIGNED_SLAB,
                                             OperatorDetails = (from slablist in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                                                where slablist.SLAB_ID == idval
                                                                select new
                                                                {
                                                                    ID = slablist.SLN,
                                                                    SERVICE_NAME = slablist.OPERATOR_NAME,
                                                                    TYPE = slablist.OPERATOR_TYPE,
                                                                    SERVICE_KEY = slablist.OPERATOR_CODE,
                                                                    CommissionType = slablist.COMMISSION_TYPE,
                                                                    CommissionPercentage = slablist.COMM_PERCENTAGE,
                                                                    SuperDisPercentage = slablist.SUPER_COM_PER,
                                                                    DistriCommissionPer = slablist.DISTRIBUTOR_COM_PER,
                                                                    RetailerCommissionPer = slablist.MERCHANT_COM_PER,
                                                                }).AsEnumerable().Select(z => new CommissionListView
                                                                {
                                                                    ID = z.ID,
                                                                    SERVICE_NAME = z.SERVICE_NAME,
                                                                    TYPE = z.TYPE,
                                                                    SERVICE_KEY = z.SERVICE_KEY,
                                                                    CommissionType = z.CommissionType,
                                                                    CommissionPercentage = z.CommissionPercentage.ToString(),
                                                                    SuperDisPercentage = z.SuperDisPercentage.ToString(),
                                                                    DistriCommissionPer = z.DistriCommissionPer.ToString(),
                                                                    RetailerCommissionPer = z.RetailerCommissionPer.ToString()
                                                                }).ToList()
                                         }).AsEnumerable().Select(d => new CommissoinManagmentmodel
                                         {
                                             SLN = d.SLN,
                                             SLAB_NAME = d.SLAB_NAME,
                                             SLAB_DETAILS = d.SLAB_DETAILS,
                                             Slab_TypeName = slabtypename,
                                             SLAB_STATUS = d.SLAB_STATUS,
                                             SLAB_TYPE = d.SLAB_TYPE,
                                             SLAB_TDS=d.SLAB_TDS,
                                             //ASSIGNED_SLAB = d.ASSIGNED_SLAB,
                                             OperatorDetails = d.OperatorDetails.OrderByDescending(c => c.TYPE).ToList()
                                         });
                        return Json(valuelist, JsonRequestBehavior.AllowGet);
                    }
                    else if (slabtype.SLAB_TYPE == 3)
                    {
                        slabtypename = "DMR";
                        var valuelist = (from slabmaster in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                         where slabmaster.SLN == idval
                                         select new
                                         {
                                             SLN = slabmaster.SLN,
                                             SLAB_NAME = slabmaster.SLAB_NAME,
                                             SLAB_DETAILS = slabmaster.SLAB_DETAILS,
                                             SLAB_TYPE = slabmaster.SLAB_TYPE,
                                             //Slab_TypeName = slabmaster.Slab_TypeName,
                                             SLAB_STATUS = slabmaster.SLAB_STATUS,
                                             SLAB_TDS = slabmaster.SLAB_TDS,
                                             //ASSIGNED_SLAB = slabmaster.ASSIGNED_SLAB,
                                             OperatorDetails = (from slablist in db.TBL_COMM_SLAB_DMR_PAYMENT
                                                                where slablist.SLAB_ID == idval && slablist.DMT_TYPE == "DOMESTIC"
                                                                select new
                                                                {
                                                                    ID = slablist.SLN,
                                                                    SERVICE_NAME = "Money Transfer (Domestic)",
                                                                    TYPE = "REMITTANCE",
                                                                    DMRFrom = slablist.SLAB_FROM,
                                                                    DMRTo = slablist.SLAB_TO,
                                                                    SERVICE_KEY = "DMI",
                                                                    COMM_TYPE = slablist.COMM_TYPE,
                                                                    CommissionPercentage = slablist.COMM_PERCENTAGE,
                                                                    SuperComm=slablist.SUPER_COM_PER,
                                                                    DistributorCom=slablist.DISTRIBUTOR_COM_PER,
                                                                    MerchantComm=slablist.MERCHANT_COM_PER,
                                                                    DistributorComm_Type = slablist.DISTRIBUTOR_COMM_TYPE,
                                                                    MerchantComm_Type = slablist.MERCHANT_COMM_TYPE,
                                                                }).AsEnumerable().Select(z => new CommissionListView
                                                                {
                                                                    ID = z.ID,
                                                                    SERVICE_NAME = "Money Transfer (Domestic)",
                                                                    TYPE = "REMITTANCE",
                                                                    DMRFrom = z.DMRFrom,
                                                                    DMRTo = z.DMRTo,
                                                                    SERVICE_KEY = "DMI",
                                                                    COMM_TYPE = z.COMM_TYPE,
                                                                    CommissionPercentage = z.CommissionPercentage.ToString(),
                                                                    SuperDisPercentage =  z.SuperComm.ToString(),
                                                                    DistriCommissionPer=z.DistributorCom.ToString(),
                                                                    RetailerCommissionPer=z.MerchantComm.ToString(),
                                                                    DistributorComm_Type = z.DistributorComm_Type,
                                                                    MerchantComm_Type = z.MerchantComm_Type,
                                                                }).ToList(),
                                             ServiceInformationDMR = (from slablist in db.TBL_COMM_SLAB_DMR_PAYMENT
                                                                      where slablist.SLAB_ID == idval && slablist.DMT_TYPE == "FOREIGN"
                                                                      select new
                                                                      {
                                                                          ID = slablist.SLN,
                                                                          SERVICE_NAME = "Money Transfer (Nepal)",
                                                                          TYPE = "REMITTANCE",
                                                                          DMRFrom = slablist.SLAB_FROM,
                                                                          DMRTo = slablist.SLAB_TO,
                                                                          SERVICE_KEY = "PMT",
                                                                          COMM_TYPE = slablist.COMM_TYPE,
                                                                          CommissionPercentage = slablist.COMM_PERCENTAGE,
                                                                          SuperCommAmt = slablist.SUPER_COM_PER,
                                                                          DistributorComAmt = slablist.DISTRIBUTOR_COM_PER,
                                                                          MerchantCommAmt = slablist.MERCHANT_COM_PER,
                                                                          DistributorComm_Type = "FIXED",
                                                                          MerchantComm_Type = "FIXED",
                                                                      }).AsEnumerable().Select(z => new CommissionListView
                                                                      {
                                                                          ID = z.ID,
                                                                          SERVICE_NAME = "Money Transfer (Nepal)",
                                                                          TYPE = "REMITTANCE",
                                                                          DMRFrom = z.DMRFrom,
                                                                          DMRTo = z.DMRTo,
                                                                          SERVICE_KEY = "PMT",
                                                                          COMM_TYPE = z.COMM_TYPE,
                                                                          CommissionPercentage = z.CommissionPercentage.ToString(),
                                                                          SuperDisPercentage = z.SuperCommAmt.ToString(),
                                                                          DistriCommissionPer = z.DistributorComAmt.ToString(),
                                                                          RetailerCommissionPer = z.MerchantCommAmt.ToString(),
                                                                          DistributorComm_Type = z.DistributorComm_Type,
                                                                          MerchantComm_Type = z.MerchantComm_Type,
                                                                      }).ToList()
                                         }).AsEnumerable().Select(d => new CommissoinManagmentmodel
                                         {
                                             SLN = d.SLN,
                                             SLAB_NAME = d.SLAB_NAME,
                                             SLAB_DETAILS = d.SLAB_DETAILS,
                                             Slab_TypeName = slabtypename,
                                             SLAB_STATUS = d.SLAB_STATUS,
                                             SLAB_TYPE = d.SLAB_TYPE,
                                             SLAB_TDS=d.SLAB_TDS,
                                             //ASSIGNED_SLAB = d.ASSIGNED_SLAB,
                                             OperatorDetails = d.OperatorDetails,
                                             ServiceInformationDMR = d.ServiceInformationDMR
                                         });
                        return Json(valuelist, JsonRequestBehavior.AllowGet);

                    }

                }
                else
                {
                    CommissoinManagmentmodel objmodel = new CommissoinManagmentmodel();
                    return Json(objmodel, JsonRequestBehavior.AllowGet);
                }
                return Json("");
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MemberCommissionSlab(Admin), method:- FetchData (GET) Line No:- 398", ex);
                throw;
            }

            

        }

        [HttpPost]
        public async Task<JsonResult> AutoComplete(string prefix)
        {
            initpage();////
            try
            {
                var db = new DBContext();
                var OperatorValue = await (from oper in db.TBL_SETTINGS_SERVICES_MASTER
                                     where oper.SERVICE_NAME.StartsWith(prefix) && oper.ACTIVESTATUS==true
                                     select new
                                     {
                                         //label = oper.SERVICE_NAME + "-" + oper.RECHTYPE,
                                         label = oper.SERVICE_NAME,
                                         val = oper.SLN
                                     }).ToListAsync();
                return Json(OperatorValue);
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MemberCommissionSlab(Admin), method:- AutoComplete (GET) Line No:- 424", ex);
                throw ex;
            }

            //var db = new DBContext();
            //var OperatorValue = (from oper in db.TBL_OPERATOR_MASTER
            //                 where oper.OPERATORNAME.StartsWith(prefix) && oper.OPERATORTYPE== OperatorType
            //                     select new
            //                {
            //                    label = oper.OPERATORNAME +"-"+ oper.RECHTYPE,
            //                    val = oper.PRODUCTID
            //               }).ToList();


        }
        //End Mobile recharge section      

        [HttpPost]
        //public ActionResult AddCommissionSlab(TBL_WHITE_LEVEL_COMMISSION_SLAB objval)
        public async Task<JsonResult> AddCommissionSlab(CommissoinManagmentmodel objval)
        //public JsonResult AddCommissionSlab(dynamic objval)
        {
            initpage();////
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    long slnid = 0;
                    if (Session["IDval"] != null)
                    {
                        slnid = long.Parse(Session["IDval"].ToString());
                        Session["IDval"] = null;
                        Session["IDval"] = "";
                        Session.Remove("IDval");
                        //Session.Abandon();
                    }
                    else
                    {
                        slnid = 0;
                    }
                    //string OperatorName = Request.Form["OperatorName"];
                    //string ServiceId = Request.Form["ServiceTypeId"];
                    //long Serv_ID = long.Parse(ServiceId);

                    var checkid = await db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == slnid).FirstOrDefaultAsync();
                    if (checkid != null)
                    {
                        checkid.SLAB_NAME = objval.SLAB_NAME;
                        checkid.SLAB_DETAILS = objval.SLAB_DETAILS;
                        checkid.SLAB_STATUS = objval.SLAB_STATUS;
                        db.Entry(checkid).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();
                        ContextTransaction.Commit();
                        if (objval.Slab_TypeName == "MOBILE RECHARGE")
                        {
                            foreach (var moblist in objval.OperatorDetails)
                            {
                                var mob = await db.TBL_COMM_SLAB_MOBILE_RECHARGE.Where(x => x.SLN == moblist.ID).FirstOrDefaultAsync();
                                if (mob != null)
                                {
                                    mob.COMMISSION_TYPE = moblist.CommissionType;
                                    mob.COMM_PERCENTAGE =Convert.ToDecimal(moblist.CommissionPercentage);
                                    db.Entry(mob).State = System.Data.Entity.EntityState.Modified;
                                    await db.SaveChangesAsync();
                                }
                            }
                        }
                        else if (objval.Slab_TypeName == "UTILITY SERVICES")
                        {
                            foreach (var moblist in objval.OperatorDetails)
                            {
                                var mob = await db.TBL_COMM_SLAB_UTILITY_RECHARGE.Where(x => x.SLN == moblist.ID).FirstOrDefaultAsync();
                                if (mob != null)
                                {
                                    mob.COMMISSION_TYPE = moblist.CommissionType;
                                    mob.COMM_PERCENTAGE = Convert.ToDecimal(moblist.CommissionPercentage); 
                                    db.Entry(mob).State = System.Data.Entity.EntityState.Modified;
                                    await db.SaveChangesAsync();
                                }
                            }
                        }
                        else if (objval.Slab_TypeName == "DMR")
                        {
                            foreach (var item in objval.OperatorDetails)
                            {
                                var DMRDomestic = await db.TBL_COMM_SLAB_DMR_PAYMENT.Where(x => x.SLN == item.ID).FirstOrDefaultAsync();
                                if (DMRDomestic != null)
                                {
                                    DMRDomestic.SLAB_FROM = item.DMRFrom;
                                    DMRDomestic.SLAB_TO = item.DMRTo;
                                    DMRDomestic.COMM_PERCENTAGE = Convert.ToDecimal(item.CommissionPercentage);
                                    db.Entry(DMRDomestic).State = System.Data.Entity.EntityState.Modified;
                                    await db.SaveChangesAsync();
                                }
                            }
                            if (objval.ServiceInformationDMR != null)
                            {
                                foreach (var item in objval.ServiceInformationDMR)
                                {
                                    if (item.DMRTo > 0)
                                    {
                                        var DMRForeign = await db.TBL_COMM_SLAB_DMR_PAYMENT.Where(x => x.SLN == item.ID).FirstOrDefaultAsync();
                                        if (DMRForeign != null)
                                        {
                                            DMRForeign.SLAB_FROM = item.DMRFrom;
                                            DMRForeign.SLAB_TO = item.DMRTo;
                                            DMRForeign.COMM_PERCENTAGE = Convert.ToDecimal(item.CommissionPercentage);
                                            db.Entry(DMRForeign).State = System.Data.Entity.EntityState.Modified;
                                            await db.SaveChangesAsync();
                                        }
                                    }
                                }
                            }
                        }
                        ViewBag.msg = "One Record Updated...";
                        Session["IDval"] = null;
                        Session["IDval"] = "";
                        Session.Remove("IDval");
                        //Session.Abandon();
                        return Json(new { Result = "Updated" });
                    }
                    else
                    {
                        
                        if (objval.Slab_TypeName == "MOBILE RECHARGE")
                        {
                            foreach (var item in objval.OperatorDetails)
                            {
                                var checkvalid = await db.TBL_COMM_SLAB_MOBILE_RECHARGE.Where(x => x.SLN == item.ID).FirstOrDefaultAsync();
                                if (Convert.ToDecimal(item.CommissionPercentage)> checkvalid.COMM_PERCENTAGE)
                                {                                    
                                    return Json(new { Result = "Failure" });
                                    break;
                                }
                            }

                            TBL_WHITE_LEVEL_COMMISSION_SLAB objslabmaster = new TBL_WHITE_LEVEL_COMMISSION_SLAB()
                            {
                                SLAB_NAME = objval.SLAB_NAME,
                                SLAB_DETAILS = objval.SLAB_DETAILS,
                                SLAB_STATUS = objval.SLAB_STATUS,
                                SLAB_TYPE = objval.SLAB_TYPE,
                                DATE_CREATED = DateTime.Now,
                                MEM_ID = MemberCurrentUser.MEM_ID
                            };
                            db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Add(objslabmaster);
                            await db.SaveChangesAsync();

                            foreach (var item in objval.OperatorDetails)
                            {
                                TBL_COMM_SLAB_MOBILE_RECHARGE objcommobile = new TBL_COMM_SLAB_MOBILE_RECHARGE()
                                {
                                    SLAB_ID = objslabmaster.SLN,
                                    MEM_ID = MemberCurrentUser.MEM_ID,
                                    OPERATOR_CODE = item.SERVICE_KEY,
                                    OPERATOR_NAME = item.SERVICE_NAME,
                                    OPERATOR_TYPE = item.TYPE,
                                    COMM_TYPE = "PERCENTAGE",
                                    COMM_PERCENTAGE = Convert.ToDecimal(item.CommissionPercentage),
                                COMM_STATUS = true,
                                    CREATED_DATE = DateTime.Now,
                                    COMMISSION_TYPE = item.CommissionType
                                };
                                db.TBL_COMM_SLAB_MOBILE_RECHARGE.Add(objcommobile);
                                await db.SaveChangesAsync();
                            }
                        }
                        else if (objval.Slab_TypeName == "UTILITY SERVICES")
                        {
                            foreach (var item in objval.OperatorDetails)
                            {
                                var checkvalid = await db.TBL_COMM_SLAB_UTILITY_RECHARGE.Where(x => x.SLN == item.ID).FirstOrDefaultAsync();
                                if (Convert.ToDecimal(item.CommissionPercentage) > checkvalid.COMM_PERCENTAGE)
                                {
                                    return Json(new { Result = "Failure" });
                                    break;
                                }
                            }
                            TBL_WHITE_LEVEL_COMMISSION_SLAB objslabmaster = new TBL_WHITE_LEVEL_COMMISSION_SLAB()
                            {
                                SLAB_NAME = objval.SLAB_NAME,
                                SLAB_DETAILS = objval.SLAB_DETAILS,
                                SLAB_STATUS = objval.SLAB_STATUS,
                                SLAB_TYPE = objval.SLAB_TYPE,
                                DATE_CREATED = DateTime.Now,
                                MEM_ID = MemberCurrentUser.MEM_ID
                            };
                            db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Add(objslabmaster);
                            await db.SaveChangesAsync();
                            foreach (var item in objval.OperatorDetails)
                            {
                                TBL_COMM_SLAB_UTILITY_RECHARGE objcomutility = new TBL_COMM_SLAB_UTILITY_RECHARGE()
                                {
                                    SLAB_ID = objslabmaster.SLN,
                                    MEM_ID = MemberCurrentUser.MEM_ID,
                                    OPERATOR_CODE = item.SERVICE_KEY,
                                    OPERATOR_NAME = item.SERVICE_NAME,
                                    OPERATOR_TYPE = item.TYPE,
                                    COMM_TYPE = "PERCENTAGE",
                                    COMM_PERCENTAGE = Convert.ToDecimal(item.CommissionPercentage),
                                    COMM_STATUS = true,
                                    CREATED_DATE = DateTime.Now,
                                    COMMISSION_TYPE = item.CommissionType
                                };
                                db.TBL_COMM_SLAB_UTILITY_RECHARGE.Add(objcomutility);
                                await db.SaveChangesAsync();
                            }
                        }
                        else if (objval.Slab_TypeName == "DMR")
                        {
                            foreach (var item in objval.OperatorDetails)
                            {
                                var checkvalid = await db.TBL_COMM_SLAB_DMR_PAYMENT.Where(x => x.SLN == item.ID).FirstOrDefaultAsync();
                                if (Convert.ToDecimal(item.CommissionPercentage) > checkvalid.COMM_PERCENTAGE)
                                {
                                    return Json(new { Result = "Failure" });
                                    break;
                                }
                            }
                            foreach (var item in objval.ServiceInformationDMR)
                            {
                                var checkvalid = await db.TBL_COMM_SLAB_DMR_PAYMENT.Where(x => x.SLN == item.ID).FirstOrDefaultAsync();
                                if (Convert.ToDecimal(item.CommissionPercentage) > checkvalid.COMM_PERCENTAGE)
                                {
                                    return Json(new { Result = "Failure" });
                                    break;
                                }
                            }

                            TBL_WHITE_LEVEL_COMMISSION_SLAB objslabmaster = new TBL_WHITE_LEVEL_COMMISSION_SLAB()
                            {
                                SLAB_NAME = objval.SLAB_NAME,
                                SLAB_DETAILS = objval.SLAB_DETAILS,
                                SLAB_STATUS = objval.SLAB_STATUS,
                                SLAB_TYPE = objval.SLAB_TYPE,
                                DATE_CREATED = DateTime.Now,
                                MEM_ID = MemberCurrentUser.MEM_ID
                            };
                            db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Add(objslabmaster);
                            await db.SaveChangesAsync();
                            foreach (var item in objval.OperatorDetails)
                            {
                                TBL_COMM_SLAB_DMR_PAYMENT objcomDMR = new TBL_COMM_SLAB_DMR_PAYMENT()
                                {
                                    SLAB_ID = objslabmaster.SLN,
                                    MEM_ID = MemberCurrentUser.MEM_ID,
                                    SLAB_FROM = item.DMRFrom,
                                    SLAB_TO = item.DMRTo,
                                    COMM_TYPE = item.COMM_TYPE,
                                    COMM_PERCENTAGE = Convert.ToDecimal(item.CommissionPercentage),
                                    COMM_STATUS = true,
                                    DMT_TYPE = "DOMESTIC",
                                    CREATED_DATE = DateTime.Now
                                };
                                db.TBL_COMM_SLAB_DMR_PAYMENT.Add(objcomDMR);
                                await db.SaveChangesAsync();
                            }
                            if (objval.ServiceInformationDMR != null)
                            {
                                foreach (var item in objval.ServiceInformationDMR)
                                {
                                    if (item.DMRTo > 0)
                                    {
                                        TBL_COMM_SLAB_DMR_PAYMENT objcomDMRInter = new TBL_COMM_SLAB_DMR_PAYMENT()
                                        {
                                            SLAB_ID = objslabmaster.SLN,
                                            MEM_ID = MemberCurrentUser.MEM_ID,
                                            SLAB_FROM = item.DMRFrom,
                                            SLAB_TO = item.DMRTo,
                                            COMM_TYPE = item.COMM_TYPE,
                                            COMM_PERCENTAGE = Convert.ToDecimal(item.CommissionPercentage),
                                            COMM_STATUS = true,
                                            DMT_TYPE = "FOREIGN",
                                            CREATED_DATE = DateTime.Now
                                        };
                                        db.TBL_COMM_SLAB_DMR_PAYMENT.Add(objcomDMRInter);
                                        await db.SaveChangesAsync();
                                    }
                                }
                            }
                        }
                        ContextTransaction.Commit();
                        //ViewBag.msg = "One Record Inserted...";
                        Session["IDval"] = null;
                        Session["IDval"] = "";
                        Session.Remove("IDval");
                        //Session.Abandon();
                        return Json(new { Result = "Success" });
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("Controller:-  MemberCommissionSlab(Admin), method:- AddCommissionSlab (POST) Line No:- 709", ex);
                    ContextTransaction.Rollback();                    
                    throw ex;
                }
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeactiveOperator(string Id)
        {
            initpage();////
            try
            {
                var db = new DBContext();
                using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
                {
                    long MemID = long.Parse(Id);
                    var getdocinfo =await db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == MemID).FirstOrDefaultAsync();
                    if (getdocinfo != null)
                    {
                        if (getdocinfo.SLAB_STATUS == false)
                        {
                            getdocinfo.SLAB_STATUS = true;
                        }
                        else
                        {
                            getdocinfo.SLAB_STATUS = false;
                        }
                        db.Entry(getdocinfo).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();
                        ContextTransaction.Commit();
                        return Json(new { Result = "true" });
                    }
                    else
                    {
                        ContextTransaction.Rollback();
                        return Json(new { Result = "false" });
                    }
                }
                //string decrptMemId = Decrypt.DecryptMe(Id);
            }
            catch (Exception ex)
            {
                
                Logger.Error("Controller:-  MemberCommissionSlab(Admin), method:- DeactiveOperator (POST) Line No:- 727", ex);
                throw ex;
            }



        }

        [HttpPost]
        public JsonResult GetServiceProvider(string NewListId)
        {
            initpage();////
            try
            {
                            var db = new DBContext();
                //var OperatorList = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE.Contains(NewListId));
                //return Json("");

                string operatortype = string.Empty;
                string operatortype1 = string.Empty;
                string DTH = string.Empty;
                string OperatorTypeName = string.Empty;
                string DTHOperator = string.Empty;
                if (NewListId == "MOBILE RECHARGE")
                {
                    NewListId = "MOBILE RECHARGE";
                    operatortype = "PREPAID";
                    operatortype1 = "POSTPAID";
                    DTH = "DTH";
                    OperatorTypeName = "MobileOperator";
                    //var listinfo = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE.StartsWith(operatortype) || x.TYPE.StartsWith(operatortype1) || x.TYPE.StartsWith(DTH)).ToList();
                    var listinfo = db.TBL_SERVICE_PROVIDERS.Where(x => x.TYPE.StartsWith(OperatorTypeName)).ToList();
                    long memid = MemberCurrentUser.MEM_ID;
                    var OperatorList = (from WList in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB join commPO in db.TBL_COMM_SLAB_MOBILE_RECHARGE on WList.RECHARGE_SLAB equals commPO.SLAB_ID
                                        join slbmst in db.TBL_WHITE_LEVEL_COMMISSION_SLAB on WList.RECHARGE_SLAB equals slbmst.SLN 
                                        where WList.WHITE_LEVEL_ID==MemberCurrentUser.MEM_ID && WList.INTRODUCER_ID==0 && slbmst.SLAB_TYPE==1
                                        select new
                                        {
                                            ID = commPO.SLN,
                                            SERVICE_NAME = commPO.OPERATOR_NAME,
                                            TYPE = commPO.OPERATOR_TYPE,
                                            SERVICE_KEY = commPO.OPERATOR_CODE,
                                            BILLING_MODEL = "",
                                            HSN_SAC = "",
                                            TDS = "",
                                            CommissionType=commPO.COMMISSION_TYPE,
                                            CommissionPercentage=commPO.COMM_PERCENTAGE,
                                            SuperDisPercentage= "0",
                                            DistriCommissionPer ="0",
                                            RetailerCommissionPer = "0"
                                        }).AsEnumerable().Select(z => new CommissionListView
                                        {
                                            ID = z.ID,
                                            SERVICE_NAME = z.SERVICE_NAME,
                                            TYPE = z.TYPE,
                                            SERVICE_KEY = z.SERVICE_KEY,
                                            BILLING_MODEL = z.BILLING_MODEL,
                                            HSN_SAC = z.HSN_SAC,
                                            TDS = z.TDS,
                                            CommissionType=z.CommissionType,
                                            CommissionPercentage=z.CommissionPercentage.ToString(),
                                            SuperDisPercentage=z.SuperDisPercentage,
                                            DistriCommissionPer = z.DistriCommissionPer,
                                            RetailerCommissionPer = z.RetailerCommissionPer
                                        }).OrderByDescending(c => c.TYPE).ToList();
                    if (OperatorList.Count == 0)
                    { return Json("Failed", JsonRequestBehavior.AllowGet);
                        
                    }
                    else
                    {
                        CommissoinManagmentmodel objmodel = new CommissoinManagmentmodel();
                        objmodel.OperatorDetails = OperatorList.ToList();
                        return Json(OperatorList, JsonRequestBehavior.AllowGet);
                    }
                    
                }
                else if (NewListId == "UTILITY SERVICES")
                {
                    string Electricity = "ELECTRICITY";
                    string Broadband = "BROADBAND";
                    string GAS = "GAS";
                    string Landline = "LANDLINE";

                    DTHOperator = "DTHOPERATOR";
                    var OperatorList = (from WList in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                                        join commPO in db.TBL_COMM_SLAB_UTILITY_RECHARGE on WList.BILLPAYMENT_SLAB equals commPO.SLAB_ID
                                        join slbmst in db.TBL_WHITE_LEVEL_COMMISSION_SLAB on WList.BILLPAYMENT_SLAB equals slbmst.SLN
                                        where WList.WHITE_LEVEL_ID == MemberCurrentUser.MEM_ID && WList.INTRODUCER_ID == 0 && slbmst.SLAB_TYPE == 2 
                                        select new
                                        {
                                            ID = commPO.SLN,
                                            SERVICE_NAME = commPO.OPERATOR_NAME,
                                            TYPE = commPO.OPERATOR_TYPE,
                                            SERVICE_KEY = commPO.OPERATOR_CODE,
                                            BILLING_MODEL = "",
                                            HSN_SAC = "",
                                            TDS = "",
                                            CommissionType = commPO.COMMISSION_TYPE,
                                            CommissionPercentage = commPO.COMM_PERCENTAGE,
                                            SuperDisPercentage = "0",
                                            DistriCommissionPer = "0",
                                            RetailerCommissionPer = "0"
                                        }).AsEnumerable().Select(z => new CommissionListView
                                        {
                                            ID = z.ID,
                                            SERVICE_NAME = z.SERVICE_NAME,
                                            TYPE = z.TYPE,
                                            SERVICE_KEY = z.SERVICE_KEY,
                                            BILLING_MODEL = z.BILLING_MODEL,
                                            HSN_SAC = z.HSN_SAC,
                                            TDS = z.TDS,
                                            CommissionType=z.CommissionType,
                                            CommissionPercentage = z.CommissionPercentage.ToString(),
                                            SuperDisPercentage = z.SuperDisPercentage,
                                            DistriCommissionPer = z.DistriCommissionPer,
                                            RetailerCommissionPer = z.RetailerCommissionPer
                                        }).OrderByDescending(c=>c.TYPE).ToList();

                    CommissoinManagmentmodel objmodel = new CommissoinManagmentmodel();
                    objmodel.OperatorDetails = OperatorList.ToList();
                    return Json(OperatorList, JsonRequestBehavior.AllowGet);
                }
                else if (NewListId == "DMR")
                {
                    string Landline = "REMITTANCE";
                    //var OperatorList = (from x in db.TBL_SERVICE_PROVIDERS
                    //                    where x.TYPE.Contains(Landline) && x.SERVICE_NAME == "Money Transfer (Domestic)" || x.SERVICE_NAME == "Money Transfer (Nepal)"
                    //                    select new
                    //                    {
                    //                        ID = x.ID,
                    //                        SERVICE_NAME = x.SERVICE_NAME,
                    //                        TYPE = x.TYPE,
                    //                        SERVICE_KEY = x.SERVICE_KEY,
                    //                        BILLING_MODEL = x.BILLING_MODEL,
                    //                        HSN_SAC = x.HSN_SAC,
                    //                        TDS = x.TDS
                    //                    }).AsEnumerable().Select(z => new CommissionListView
                    //                    {
                    //                        ID = z.ID,
                    //                        SERVICE_NAME = z.SERVICE_NAME,
                    //                        TYPE = z.TYPE,
                    //                        SERVICE_KEY = z.SERVICE_KEY,
                    //                        BILLING_MODEL = z.BILLING_MODEL,
                    //                        HSN_SAC = z.HSN_SAC,
                    //                        TDS = z.TDS
                    //                    }).ToList().OrderByDescending(c => c.TYPE);
                    var OperatorList = (from detail in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                                        join dmrcom in db.TBL_COMM_SLAB_DMR_PAYMENT on detail.DMR_SLAB equals dmrcom.SLAB_ID
                                        join white in db.TBL_WHITE_LEVEL_COMMISSION_SLAB on detail.DMR_SLAB equals white.SLN
                                        where detail.WHITE_LEVEL_ID == MemberCurrentUser.MEM_ID && detail.INTRODUCER_ID == 0 && white.SLAB_TYPE == 3
                                        select new
                                        {
                                            ID = dmrcom.SLN,
                                            SERVICE_NAME = "",
                                            TYPE = "",
                                            SERVICE_KEY = "",
                                            BILLING_MODEL = "",
                                            HSN_SAC = "",
                                            TDS = "",
                                            OperatorDetails = (from DMcm in db.TBL_COMM_SLAB_DMR_PAYMENT
                                                               join dtlcom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB on DMcm.SLAB_ID equals dtlcom.DMR_SLAB
                                                               where dtlcom.WHITE_LEVEL_ID == MemberCurrentUser.MEM_ID && dtlcom.INTRODUCER_ID == 0 && DMcm.DMT_TYPE == "DOMESTIC"
                                                               select new
                                                               {
                                                                   ID = DMcm.SLN,
                                                                   SERVICE_NAME = "Money Transfer (Domestic)",
                                                                   TYPE = "REMITTANCE",
                                                                   DMRFrom = DMcm.SLAB_FROM,
                                                                   DMRTo = DMcm.SLAB_TO,
                                                                   SERVICE_KEY = "DMI",
                                                                   COMM_TYPE = DMcm.COMM_TYPE,
                                                                   CommissionPercentage = DMcm.COMM_PERCENTAGE,
                                                                   SuperComm = DMcm.SUPER_COM_PER,
                                                                   DistributorCom = DMcm.DISTRIBUTOR_COM_PER,
                                                                   MerchantComm = DMcm.MERCHANT_COM_PER,
                                                                   DistributorComm_Type = DMcm.DISTRIBUTOR_COMM_TYPE,
                                                                   MerchantComm_Type = DMcm.MERCHANT_COMM_TYPE,
                                                               }).AsEnumerable().Select(z => new CommissionListView
                                                               {
                                                                   ID = z.ID,
                                                                   SERVICE_NAME = "Money Transfer (Domestic)",
                                                                   TYPE = "REMITTANCE",
                                                                   DMRFrom = z.DMRFrom,
                                                                   DMRTo = z.DMRTo,
                                                                   SERVICE_KEY = "DMI",
                                                                   COMM_TYPE = z.COMM_TYPE,
                                                                   CommissionPercentage = z.CommissionPercentage.ToString(),
                                                                   SuperDisPercentage = z.SuperComm.ToString(),
                                                                   DistriCommissionPer = z.DistributorCom.ToString(),
                                                                   RetailerCommissionPer = z.MerchantComm.ToString(),
                                                                   DistributorComm_Type = z.DistributorComm_Type,
                                                                   MerchantComm_Type = z.MerchantComm_Type,
                                                               }).ToList(),
                                            //OperatorDetails = (from DMcm in db.TBL_COMM_SLAB_DMR_PAYMENT
                                            //                   join dtlcom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB on DMcm.SLAB_ID equals dtlcom.DMR_SLAB
                                            //                   where dtlcom.WHITE_LEVEL_ID == MemberCurrentUser.MEM_ID && dtlcom.INTRODUCER_ID == 0 && DMcm.DMT_TYPE == "DOMESTIC"
                                            //                   select new CommissionListView
                                            //                   {
                                            //                       ID = DMcm.SLN,
                                            //                       SERVICE_NAME = "Money Transfer (Domestic)",
                                            //                       TYPE = "REMITTANCE",
                                            //                       SERVICE_KEY = "DMI",
                                            //                       DMRFrom = DMcm.SLAB_FROM,
                                            //                       DMRTo = DMcm.SLAB_TO,
                                            //                       COMM_TYPE = DMcm.COMM_TYPE,
                                            //                       CommissionPercentage = DMcm.COMM_PERCENTAGE.ToString(),
                                            //                       DistributorComm_Type= DMcm.DISTRIBUTOR_COMM_TYPE,
                                            //                       DistriCommissionPer=DMcm.DISTRIBUTOR_COM_PER.ToString(),
                                            //                       MerchantComm_Type = DMcm.COMMISSION_TYPE,
                                            //                       RetailerCommissionPer = DMcm.MERCHANT_COM_PER.ToString(),                                                                   
                                            //                       SuperDisPercentage = DMcm.SUPER_COM_PER.ToString(),

                                            //                   }).ToList(),
                                            //ServiceInformationDMR = (from DMcm in db.TBL_COMM_SLAB_DMR_PAYMENT
                                            //                         join dtlcom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB on DMcm.SLAB_ID equals dtlcom.DMR_SLAB
                                            //                         where dtlcom.WHITE_LEVEL_ID == MemberCurrentUser.MEM_ID && dtlcom.INTRODUCER_ID == 0 && DMcm.DMT_TYPE == "FOREIGN"
                                            //                         select new CommissionListView
                                            //                         {
                                            //                             ID = DMcm.SLN,
                                            //                             SERVICE_NAME = "Money Transfer (Nepal)",
                                            //                             TYPE = "REMITTANCE",
                                            //                             SERVICE_KEY = "DMI",
                                            //                             DMRFrom = DMcm.SLAB_FROM,
                                            //                             DMRTo = DMcm.SLAB_TO,
                                            //                             COMM_TYPE = DMcm.COMM_TYPE,
                                            //                             CommissionPercentage = DMcm.COMM_PERCENTAGE.ToString(),
                                            //                             DistributorComm_Type = DMcm.DISTRIBUTOR_COMM_TYPE,
                                            //                             DistriCommissionPer = DMcm.DISTRIBUTOR_COM_PER.ToString(),
                                            //                             MerchantComm_Type = DMcm.COMMISSION_TYPE,
                                            //                             RetailerCommissionPer = DMcm.MERCHANT_COM_PER.ToString(),
                                            //                             SuperDisPercentage = DMcm.SUPER_COM_PER.ToString(),
                                            //                         }).ToList(),
                                        }).FirstOrDefault();
                    CommissoinManagmentmodel objmodel = new CommissoinManagmentmodel();
                    //objmodel.OperatorDetails = OperatorList.ToList();
                    return Json(OperatorList, JsonRequestBehavior.AllowGet);
                }
               
                return Json("");
                //}
            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MemberCommissionSlab(Admin), method:- GetServiceProvider (POST) Line No:- 1024", ex);
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult GetDMRCommissionValue()
        {
            initpage();////
            try
            {
                var db = new DBContext();
                var OperatorList = (from detail in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                                    join dmrcom in db.TBL_COMM_SLAB_DMR_PAYMENT on detail.DMR_SLAB equals dmrcom.SLAB_ID
                                    join white in db.TBL_WHITE_LEVEL_COMMISSION_SLAB on detail.DMR_SLAB equals white.SLN
                                    where detail.WHITE_LEVEL_ID == MemberCurrentUser.MEM_ID && detail.INTRODUCER_ID == 0 && white.SLAB_TYPE == 3
                                    select new
                                    {
                                        ID = dmrcom.SLN,
                                        SERVICE_NAME = "",
                                        TYPE = "",
                                        SERVICE_KEY = "",
                                        BILLING_MODEL = "",
                                        HSN_SAC = "",
                                        TDS = "",
                                        OperatorDetails = (from DMcm in db.TBL_COMM_SLAB_DMR_PAYMENT
                                                           join dtlcom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB on DMcm.SLAB_ID equals dtlcom.DMR_SLAB
                                                           where dtlcom.WHITE_LEVEL_ID == MemberCurrentUser.MEM_ID && dtlcom.INTRODUCER_ID == 0 && DMcm.DMT_TYPE == "DOMESTIC"
                                                           select new CommissionListView
                                                           {
                                                               ID = DMcm.SLN,
                                                               SERVICE_NAME = "Money Transfer (Domestic)",
                                                               TYPE = "REMITTANCE",
                                                               SERVICE_KEY = "DMI",
                                                               DMRFrom = DMcm.SLAB_FROM,
                                                               DMRTo = DMcm.SLAB_TO,
                                                               COMM_TYPE = DMcm.COMM_TYPE,
                                                               CommissionPercentage = DMcm.COMM_PERCENTAGE.ToString()
                                                           }).ToList(),
                                        ServiceInformationDMR = (from DMcm in db.TBL_COMM_SLAB_DMR_PAYMENT
                                                                 join dtlcom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB on DMcm.SLAB_ID equals dtlcom.DMR_SLAB
                                                                 where dtlcom.WHITE_LEVEL_ID == MemberCurrentUser.MEM_ID && dtlcom.INTRODUCER_ID == 0 && DMcm.DMT_TYPE == "FOREIGN"
                                                                 select new CommissionListView
                                                                 {
                                                                     ID = DMcm.SLN,
                                                                     SERVICE_NAME = "Money Transfer (Nepal)",
                                                                     TYPE = "REMITTANCE",
                                                                     SERVICE_KEY = "DMI",
                                                                     DMRFrom = DMcm.SLAB_FROM,
                                                                     DMRTo = DMcm.SLAB_TO,
                                                                     COMM_TYPE = DMcm.COMM_TYPE,
                                                                     CommissionPercentage = DMcm.COMM_PERCENTAGE.ToString()
                                                                 }).ToList(),
                                    }).FirstOrDefault();
                return Json(OperatorList.OperatorDetails.Select(z => z.DMRTo).FirstOrDefault(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost]

        public async Task<JsonResult> CheckCommPercentage(string CommID,string Amount)
        {
            initpage();////
            try
            {

                long comID_Val = long.Parse(CommID);
                decimal InputAmount = decimal.Parse(Amount);
                var db = new DBContext();
                var checkdata = await (from listoper in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                 join mstr in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB on listoper.SLAB_ID equals mstr.RECHARGE_SLAB
                                 where listoper.SLN == comID_Val && mstr.WHITE_LEVEL_ID == MemberCurrentUser.MEM_ID && mstr.INTRODUCER_ID == 0 && listoper.SLN == comID_Val
                                 select new
                                 {
                                     Amountval = listoper.COMM_PERCENTAGE
                                 }).FirstOrDefaultAsync();
                if (InputAmount > checkdata.Amountval)
                {
                    return Json("Exceed", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Error", JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                Logger.Error("Controller:-  MemberCommissionSlab(Admin), method:- CheckCommPercentage (POST) Line No:- 1034", ex);
                throw ex;
            }
            return Json("");
        }


        public ActionResult GenerateCommissionSlab(string SlabId="")
        {
            initpage();
            if (Session["WhiteLevelUserId"] != null)
            {
                try
                {
                    if (Request.QueryString["Operatortype"] != null)
                    {

                    }
                    if (SlabId != "")
                    {

                        string decriptval = Decrypt.DecryptMe(SlabId.ToString());
                        long SlabIdVal = long.Parse(decriptval);
                        Session["IDval"] = SlabIdVal;
                        var db = new DBContext();
                        

                        //return View(listSlab);
                        return View();
                    }
                    else
                    {
                        Session["IDval"] = null;
                        Session["IDval"] = null;
                        Session.Remove("IDval");
                        Session.Remove("IDval");
                        Session["IDval"] = null;
                        Session["IDval"] = "";
                        Session.Remove("IDval");
                        //Session.Abandon();
                        CommissoinManagmentmodel objmodel = new CommissoinManagmentmodel();
                        objmodel.OperatorDetails = null;
                        ViewBag.checkStatus = "0";
                        return View();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("Controller:-  MemberCommissionSlab(Admin), method:- GenerateCommissionSlab (GET) Line No:- 1131", ex);
                    return RedirectToAction("Exception", "ErrorHandler", new { area = "" });
                    throw ex;
                }
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
        public JsonResult GetMemberRole()
        {
            initpage();////
            var db = new DBContext();
            var Roleinfo = db.TBL_MASTER_MEMBER_ROLE.Where(x => x.ROLE_ID != 1 && x.ROLE_ID != 2).Select(x => new { x.ROLE_ID, x.ROLE_NAME }).ToList();

            return Json(Roleinfo, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult CheckDuplicateSlabName(string slabname)
        {
            initpage();////
            var db = new DBContext();
            var checkSlabName=db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x=>x.MEM_ID==MemberCurrentUser.MEM_ID && x.SLAB_NAME.Equals(slabname)).FirstOrDefault();
            if (checkSlabName != null)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]        
        public async Task<JsonResult> GenerateCommissionSlab(CommissoinManagmentmodel objval)
        {
            initpage();////
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    long slnid = 0;
                    if (Session["IDval"] != null)
                    {
                        slnid = long.Parse(Session["IDval"].ToString());
                        Session["IDval"] = null;
                        Session["IDval"] = "";
                        Session.Remove("IDval");
                        //Session.Abandon();
                    }
                    else
                    {
                        slnid = 0;
                    }
                    //string OperatorName = Request.Form["OperatorName"];
                    //string ServiceId = Request.Form["ServiceTypeId"];
                    //long Serv_ID = long.Parse(ServiceId);
                    var checkid = await db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Where(x => x.SLN == slnid).FirstOrDefaultAsync();
                    if (checkid != null)
                    {
                        checkid.SLAB_NAME = objval.SLAB_NAME;
                        checkid.SLAB_DETAILS = objval.SLAB_DETAILS;
                        checkid.SLAB_STATUS = objval.SLAB_STATUS;
                        checkid.SLAB_TDS = objval.SLAB_TDS;
                        db.Entry(checkid).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();
                        ContextTransaction.Commit();
                        if (objval.Slab_TypeName == "MOBILE RECHARGE")
                        {
                            foreach (var moblist in objval.OperatorDetails)
                            {
                                var mob = await db.TBL_COMM_SLAB_MOBILE_RECHARGE.Where(x => x.SLN == moblist.ID).FirstOrDefaultAsync();
                                if (mob != null)
                                {
                                    mob.COMMISSION_TYPE = moblist.CommissionType;
                                    mob.COMM_PERCENTAGE = Convert.ToDecimal(moblist.CommissionPercentage);
                                    mob.SUPER_COM_PER = Convert.ToDecimal(moblist.SuperDisPercentage);
                                    mob.DISTRIBUTOR_COM_PER = Convert.ToDecimal(moblist.DistriCommissionPer);
                                    mob.MERCHANT_COM_PER= Convert.ToDecimal(moblist.RetailerCommissionPer);
                                    db.Entry(mob).State = System.Data.Entity.EntityState.Modified;
                                    await db.SaveChangesAsync();
                                }
                            }
                        }
                        else if (objval.Slab_TypeName == "UTILITY SERVICES")
                        {
                            foreach (var moblist in objval.OperatorDetails)
                            {
                                var mob = await db.TBL_COMM_SLAB_MOBILE_RECHARGE.Where(x => x.SLN == moblist.ID).FirstOrDefaultAsync();
                                if (mob != null)
                                {
                                    mob.COMMISSION_TYPE = moblist.CommissionType;
                                    mob.COMM_PERCENTAGE = Convert.ToDecimal(moblist.CommissionPercentage);
                                    mob.SUPER_COM_PER = Convert.ToDecimal(moblist.SuperDisPercentage);
                                    mob.DISTRIBUTOR_COM_PER = Convert.ToDecimal(moblist.DistriCommissionPer);
                                    mob.MERCHANT_COM_PER = Convert.ToDecimal(moblist.RetailerCommissionPer);
                                    db.Entry(mob).State = System.Data.Entity.EntityState.Modified;
                                    await db.SaveChangesAsync();
                                }
                            }
                        }
                        else if (objval.Slab_TypeName == "DMR")
                        {
                            int cntval = 0;
                            string dmrfirst = "";
                            string dmrLast = "";
                            foreach (var lstval in objval.OperatorDetails)
                            {
                                cntval += 1;
                                if (cntval == 1)
                                {
                                    dmrfirst = Convert.ToString(lstval.DMRFrom);
                                }
                                if (cntval == 3)
                                {
                                    dmrLast = Convert.ToString(lstval.DMRTo);
                                }
                            }
                            if (dmrLast == "5000")
                            {
                                foreach (var item in objval.OperatorDetails)
                                {
                                    var DMRDomestic = await db.TBL_COMM_SLAB_DMR_PAYMENT.Where(x => x.SLN == item.ID).FirstOrDefaultAsync();
                                    if (DMRDomestic != null)
                                    {
                                        DMRDomestic.COMMISSION_TYPE = item.COMM_TYPE;
                                        DMRDomestic.SLAB_FROM = item.DMRFrom;
                                        DMRDomestic.SLAB_TO = item.DMRTo;
                                        DMRDomestic.SUPER_COM_PER = Convert.ToDecimal(item.SuperDisPercentage);
                                        DMRDomestic.DISTRIBUTOR_COM_PER = Convert.ToDecimal(item.DistriCommissionPer);
                                        DMRDomestic.MERCHANT_COM_PER = Convert.ToDecimal(item.RetailerCommissionPer);
                                        DMRDomestic.COMM_PERCENTAGE = Convert.ToDecimal(objval.ASSIGNED_SLAB);
                                        DMRDomestic.DISTRIBUTOR_COMM_TYPE = item.DistributorComm_Type;
                                        DMRDomestic.MERCHANT_COMM_TYPE = item.MerchantComm_Type;
                                        db.Entry(DMRDomestic).State = System.Data.Entity.EntityState.Modified;
                                        await db.SaveChangesAsync();
                                    }
                                }
                                if (objval.ServiceInformationDMR != null)
                                {
                                    foreach (var item in objval.ServiceInformationDMR)
                                    {
                                        if (item.DMRTo > 0)
                                        {
                                            var DMRForeign = await db.TBL_COMM_SLAB_DMR_PAYMENT.Where(x => x.SLN == item.ID).FirstOrDefaultAsync();
                                            if (DMRForeign != null)
                                            {
                                                DMRForeign.SLAB_FROM = item.DMRFrom;
                                                DMRForeign.SLAB_TO = item.DMRTo;
                                                DMRForeign.SUPER_COM_PER = Convert.ToDecimal(item.SuperDisPercentage);
                                                DMRForeign.DISTRIBUTOR_COM_PER = Convert.ToDecimal(item.DistriCommissionPer);
                                                DMRForeign.MERCHANT_COM_PER = Convert.ToDecimal(item.RetailerCommissionPer);
                                                DMRForeign.COMM_PERCENTAGE = Convert.ToDecimal(objval.ASSIGNED_SLAB);
                                                db.Entry(DMRForeign).State = System.Data.Entity.EntityState.Modified;
                                                await db.SaveChangesAsync();
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                return Json(new { Result = "FAILED" });
                            }
                        }
                        ViewBag.msg = "One Record Updated...";
                        Session["IDval"] = null;
                        Session["IDval"] = "";
                        Session.Remove("IDval");
                        //Session.Abandon();
                        return Json(new { Result = "Updated" });
                    }
                    else
                    {
                        if (objval.Slab_TypeName == "MOBILE RECHARGE")
                        {
                            foreach (var item in objval.OperatorDetails)
                            {
                                var checkvalid = await db.TBL_COMM_SLAB_MOBILE_RECHARGE.Where(x => x.SLN == item.ID).FirstOrDefaultAsync();
                                if (Convert.ToDecimal(item.CommissionPercentage) > checkvalid.COMM_PERCENTAGE)
                                {
                                    return Json(new { Result = "Failure" });
                                    break;
                                }
                            }
                            TBL_WHITE_LEVEL_COMMISSION_SLAB objslabmaster = new TBL_WHITE_LEVEL_COMMISSION_SLAB()
                            {
                                SLAB_NAME = objval.SLAB_NAME,
                                SLAB_DETAILS = objval.SLAB_DETAILS,
                                SLAB_STATUS = objval.SLAB_STATUS,
                                SLAB_TYPE = objval.SLAB_TYPE,
                                DATE_CREATED = DateTime.Now,
                                MEM_ID = MemberCurrentUser.MEM_ID  ,
                                SLAB_TDS=objval.SLAB_TDS
                                //ASSIGNED_SLAB=objval.ASSIGNED_SLAB
                            };
                            db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Add(objslabmaster);
                            await db.SaveChangesAsync();
                            foreach (var item in objval.OperatorDetails)
                            {
                                TBL_COMM_SLAB_MOBILE_RECHARGE objcommobile = new TBL_COMM_SLAB_MOBILE_RECHARGE()
                                {
                                    SLAB_ID = objslabmaster.SLN,
                                    MEM_ID = MemberCurrentUser.MEM_ID,
                                    OPERATOR_CODE = item.SERVICE_KEY,
                                    OPERATOR_NAME = item.SERVICE_NAME,
                                    OPERATOR_TYPE = item.TYPE,
                                    COMM_TYPE = "PERCENTAGE",
                                    COMM_PERCENTAGE = Convert.ToDecimal(item.CommissionPercentage),
                                    COMM_STATUS = true,
                                    CREATED_DATE = DateTime.Now,
                                    SUPER_ROLE_ID=3,
                                    DISTRIBUTOR_ROLE_ID=4,
                                    MERCHANT_ROLE_ID=5,
                                    COMMISSION_TYPE = item.CommissionType,
                                    SUPER_COM_PER = Convert.ToDecimal(item.SuperDisPercentage),
                                    DISTRIBUTOR_COM_PER=Convert.ToDecimal(item.DistriCommissionPer),
                                    MERCHANT_COM_PER=Convert.ToDecimal(item.RetailerCommissionPer)
                                };
                                db.TBL_COMM_SLAB_MOBILE_RECHARGE.Add(objcommobile);
                                await db.SaveChangesAsync();
                            }
                        }
                        else if (objval.Slab_TypeName == "UTILITY SERVICES")
                        {
                            foreach (var item in objval.OperatorDetails)
                            {
                                var checkvalid = await db.TBL_COMM_SLAB_UTILITY_RECHARGE.Where(x => x.SLN == item.ID).FirstOrDefaultAsync();
                                if (Convert.ToDecimal(item.CommissionPercentage) > checkvalid.COMM_PERCENTAGE)
                                {
                                    return Json(new { Result = "Failure" });
                                    break;
                                }
                            }
                            TBL_WHITE_LEVEL_COMMISSION_SLAB objslabmaster = new TBL_WHITE_LEVEL_COMMISSION_SLAB()
                            {
                                SLAB_NAME = objval.SLAB_NAME,
                                SLAB_DETAILS = objval.SLAB_DETAILS,
                                SLAB_STATUS = objval.SLAB_STATUS,
                                SLAB_TYPE = objval.SLAB_TYPE,
                                DATE_CREATED = DateTime.Now,
                                MEM_ID = MemberCurrentUser.MEM_ID,
                                SLAB_TDS=objval.SLAB_TDS
                                //ASSIGNED_SLAB = objval.ASSIGNED_SLAB
                            };
                            db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Add(objslabmaster);
                            await db.SaveChangesAsync();
                            foreach (var item in objval.OperatorDetails)
                            {
                                TBL_COMM_SLAB_MOBILE_RECHARGE objcomutility = new TBL_COMM_SLAB_MOBILE_RECHARGE()
                                {
                                    SLAB_ID = objslabmaster.SLN,
                                    MEM_ID = MemberCurrentUser.MEM_ID,
                                    OPERATOR_CODE = item.SERVICE_KEY,
                                    OPERATOR_NAME = item.SERVICE_NAME,
                                    OPERATOR_TYPE = item.TYPE,
                                    COMM_TYPE = "PERCENTAGE",
                                    COMM_PERCENTAGE = Convert.ToDecimal(item.CommissionPercentage),
                                    COMM_STATUS = true,
                                    CREATED_DATE = DateTime.Now,
                                    SUPER_ROLE_ID = 3,
                                    DISTRIBUTOR_ROLE_ID = 4,
                                    MERCHANT_ROLE_ID = 5,
                                    COMMISSION_TYPE = item.CommissionType,
                                    SUPER_COM_PER = Convert.ToDecimal(item.SuperDisPercentage),
                                    DISTRIBUTOR_COM_PER = Convert.ToDecimal(item.DistriCommissionPer),
                                    MERCHANT_COM_PER = Convert.ToDecimal(item.RetailerCommissionPer)
                                };
                                db.TBL_COMM_SLAB_MOBILE_RECHARGE.Add(objcomutility);
                                await db.SaveChangesAsync();
                            }
                        }
                        else if (objval.Slab_TypeName == "DMR")
                        {
                            int cntval = 0;
                            string dmrfirst = "";
                            string dmrLast = "";
                            foreach (var lstval in objval.OperatorDetails)
                            {
                                cntval += 1;
                                if (cntval == 1)
                                {
                                    dmrfirst = Convert.ToString(lstval.DMRFrom);
                                }
                                if (cntval == 3)
                                {
                                    dmrLast = Convert.ToString(lstval.DMRTo);
                                }
                            }
                            if (dmrLast == "5000")
                            {
                                TBL_WHITE_LEVEL_COMMISSION_SLAB objslabmaster = new TBL_WHITE_LEVEL_COMMISSION_SLAB()
                                {
                                    SLAB_NAME = objval.SLAB_NAME,
                                    SLAB_DETAILS = objval.SLAB_DETAILS,
                                    SLAB_STATUS = objval.SLAB_STATUS,
                                    SLAB_TYPE = objval.SLAB_TYPE,
                                    DATE_CREATED = DateTime.Now,
                                    MEM_ID = MemberCurrentUser.MEM_ID,
                                    SLAB_TDS = objval.SLAB_TDS
                                    //ASSIGNED_SLAB = objval.ASSIGNED_SLAB
                                };
                                db.TBL_WHITE_LEVEL_COMMISSION_SLAB.Add(objslabmaster);
                                await db.SaveChangesAsync();
                                foreach (var item in objval.OperatorDetails)
                                {
                                    TBL_COMM_SLAB_DMR_PAYMENT objcomDMR = new TBL_COMM_SLAB_DMR_PAYMENT()
                                    {
                                        SLAB_ID = objslabmaster.SLN,
                                        MEM_ID = MemberCurrentUser.MEM_ID,
                                        SLAB_FROM = item.DMRFrom,
                                        SLAB_TO = item.DMRTo,
                                        COMM_TYPE = item.COMM_TYPE,
                                        //COMM_PERCENTAGE = Convert.ToDecimal(objval.ASSIGNED_SLAB),
                                        COMM_PERCENTAGE = Convert.ToDecimal(item.CommissionPercentage),
                                        COMM_STATUS = true,
                                        DMT_TYPE = "DOMESTIC",
                                        CREATED_DATE = DateTime.Now,
                                        SUPER_ROLE_ID = 3,
                                        DISTRIBUTOR_ROLE_ID = 4,
                                        MERCHANT_ROLE_ID = 5,
                                        SUPER_COM_PER = Convert.ToDecimal(item.SuperDisPercentage),
                                        DISTRIBUTOR_COM_PER = Convert.ToDecimal(item.DistriCommissionPer),
                                        MERCHANT_COM_PER = Convert.ToDecimal(item.RetailerCommissionPer),
                                        COMMISSION_TYPE = item.COMM_TYPE,
                                        DISTRIBUTOR_COMM_TYPE = item.DistributorComm_Type,
                                        MERCHANT_COMM_TYPE = item.MerchantComm_Type
                                    };
                                    db.TBL_COMM_SLAB_DMR_PAYMENT.Add(objcomDMR);
                                    await db.SaveChangesAsync();
                                }
                                if (objval.ServiceInformationDMR != null)
                                {
                                    foreach (var item in objval.ServiceInformationDMR)
                                    {
                                        if (item.DMRTo > 0)
                                        {
                                            TBL_COMM_SLAB_DMR_PAYMENT objcomDMRInter = new TBL_COMM_SLAB_DMR_PAYMENT()
                                            {
                                                SLAB_ID = objslabmaster.SLN,
                                                MEM_ID = MemberCurrentUser.MEM_ID,
                                                SLAB_FROM = item.DMRFrom,
                                                SLAB_TO = item.DMRTo,
                                                COMM_TYPE = item.COMM_TYPE,
                                                COMM_PERCENTAGE = Convert.ToDecimal(objval.ASSIGNED_SLAB),
                                                COMM_STATUS = true,
                                                DMT_TYPE = "FOREIGN",
                                                CREATED_DATE = DateTime.Now,
                                                SUPER_ROLE_ID = 3,
                                                DISTRIBUTOR_ROLE_ID = 4,
                                                MERCHANT_ROLE_ID = 5,
                                                SUPER_COM_PER = Convert.ToDecimal(item.SuperDisPercentage),
                                                DISTRIBUTOR_COM_PER = Convert.ToDecimal(item.DistriCommissionPer),
                                                MERCHANT_COM_PER = Convert.ToDecimal(item.RetailerCommissionPer),
                                                COMMISSION_TYPE = item.COMM_TYPE
                                            };
                                            db.TBL_COMM_SLAB_DMR_PAYMENT.Add(objcomDMRInter);
                                            await db.SaveChangesAsync();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                return Json(new { Result = "FAILED" });
                            }
                            
                        }
                        ContextTransaction.Commit();
                        //ViewBag.msg = "One Record Inserted...";
                        Session["IDval"] = null;
                        Session["IDval"] = "";
                        Session.Remove("IDval");
                        //Session.Abandon();
                        return Json(new { Result = "Success" });
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("Controller:-  MemberCommissionSlab(Admin), method:- AddCommissionSlab (POST) Line No:- 709", ex);
                    ContextTransaction.Rollback();
                    throw ex;
                }
            }

        }


    }
}