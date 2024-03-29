﻿using System;
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
using System.Threading.Tasks;
using System.Data.Entity;
using log4net;
using System.Web.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Web.Script.Serialization;
using System.Device.Location;
using WHITELABEL.Web.Controllers;
using System.Web.UI;
using System.Text.RegularExpressions;


namespace WHITELABEL.Web.Areas.Merchant.Controllers
{
    public class MerchantCommissionListController : MerchantBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        
        public void initpage()
        {
            try
            {
                ViewBag.ControllerName = "Merchant";

                if (Session["MerchantUserId"] == null)
                {
                    //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
                    Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
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

        // GET: Merchant/MerchantCommissionList
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
                var memberinfo = dbcontext.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).FirstOrDefault();
                var MerchantComm=(from CommMer in dbcontext.TBL_COMM_SLAB_MOBILE_RECHARGE join WLComm in dbcontext.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                  on CommMer.SLAB_ID equals WLComm.SLN where CommMer.MEM_ID == memberinfo.UNDER_WHITE_LEVEL
                                  select new
                                  {
                                      SLAB_NAME = WLComm.SLAB_NAME,
                                      Operator_Name = CommMer.OPERATOR_NAME,
                                      Operator_Code = CommMer.OPERATOR_CODE,
                                      Operator_Type = CommMer.OPERATOR_TYPE,
                                      MER_COMMTYPE = CommMer.COMMISSION_TYPE,
                                      MER_COMMAMT = CommMer.MERCHANT_COM_PER
                                  }).AsEnumerable().Select(z => new TBL_COMM_SLAB_MOBILE_RECHARGE
                                  {
                                      COMM_TYPE = z.SLAB_NAME,
                                      OPERATOR_NAME = z.Operator_Name,
                                      OPERATOR_CODE = z.Operator_Code,
                                      OPERATOR_TYPE = z.Operator_Type,
                                        COMMISSION_TYPE = z.MER_COMMTYPE,
                                      MERCHANT_COM_PER = z.MER_COMMAMT
                                  }).ToList();

                return PartialView("IndexGrid", MerchantComm);
                //return PartialView(CreateExportableGrid());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public PartialViewResult DMTCommissionIndexGrid()
        {
            try
            {
                var db = new DBContext();
                var memberinfo = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == CurrentMerchant.MEM_ID).FirstOrDefault();
                var MerDMRComm = (from commdmr in db.TBL_COMM_SLAB_DMR_PAYMENT
                                  join WLCom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB on commdmr.SLAB_ID equals WLCom.SLN
                                  where commdmr.MEM_ID == memberinfo.UNDER_WHITE_LEVEL
                                  select new
                                  {
                                      SLAB_NAME = WLCom.SLAB_NAME,
                                      SLAB_FROM = commdmr.SLAB_FROM,
                                      SLAB_TO = commdmr.SLAB_TO,
                                      SLAB_TYPE = commdmr.COMM_TYPE,
                                      MER_COMMTYPE = commdmr.MERCHANT_COMM_TYPE,
                                      MER_COMMAMT = commdmr.MERCHANT_COM_PER
                                  }).AsEnumerable().Select(z => new TBL_COMM_SLAB_DMR_PAYMENT
                                  {
                                      DISTRIBUTOR_COMM_TYPE=z.SLAB_NAME,
                                      SLAB_FROM=z.SLAB_FROM,
                                      SLAB_TO=z.SLAB_TO,
                                      COMM_TYPE = z.SLAB_TYPE,
                                      MERCHANT_COMM_TYPE=z.MER_COMMTYPE,
                                      MERCHANT_COM_PER=z.MER_COMMAMT

                                  }).ToList();
                return PartialView("DMTCommissionIndexGrid", MerDMRComm);
            }
            catch (Exception ex)
            {

                throw ex; 
            }
            
        }

    }
}