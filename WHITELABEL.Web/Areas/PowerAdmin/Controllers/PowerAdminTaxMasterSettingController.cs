using log4net;
using NonFactors.Mvc.Grid;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
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
    public class PowerAdminTaxMasterSettingController : PoweradminbaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        // GET: PowerAdmin/PowerAdminTaxMasterSetting
        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult IndexGrid()
        {
            try
            {
                var dbcontext = new DBContext();              
                var memberinfo = dbcontext.TBL_TAX_MASTERS.ToList();
                return PartialView("IndexGrid", memberinfo);
              
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpPost]
        public JsonResult PostTaxMasterSetting(TBL_TAX_MASTERS objTax)
        {
            try
            {
                var db = new DBContext();
                var checkTaxAvailable = db.TBL_TAX_MASTERS.FirstOrDefault(x => x.SLN == objTax.SLN);
                if (checkTaxAvailable == null)
                {
                    objTax.TAX_CREATED_DATE = DateTime.Now;
                    objTax.MEM_ID = CurrentUser.USER_ID;
                    db.TBL_TAX_MASTERS.Add(objTax);
                    db.SaveChanges();
                    return Json("Tax Information is saved.", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    checkTaxAvailable.TAX_NAME = objTax.TAX_NAME;
                    checkTaxAvailable.TAX_DESCRIPTION = objTax.TAX_DESCRIPTION;
                    checkTaxAvailable.TAX_MODE = objTax.TAX_MODE;
                    checkTaxAvailable.TAX_VALUE = objTax.TAX_VALUE;
                    checkTaxAvailable.TAX_STATUS = objTax.TAX_STATUS;
                    checkTaxAvailable.TAX_CREATED_DATE = DateTime.Now;
                    db.Entry(checkTaxAvailable).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json("Tax Information is saved.", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var msg = "Please try again after 15 minute";
                return Json(msg);
                Logger.Error("Controller:-  MerchantFingerprintdeviceController(Merchant), method:- AddFingerPrintDevice(POST)", ex);
                throw ex;
            }

            
        }
        [HttpPost]
        public JsonResult GetFingerPrintInfo(string TransId)
        {
            try
            {
                long valueid = long.Parse(TransId);
                var db = new DBContext();
                var BankMarginvaluebind = db.TBL_TAX_MASTERS.Where(x => x.SLN == valueid).FirstOrDefault();
                return Json(BankMarginvaluebind, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
}