namespace WHITELABEL.Web.Areas.Merchant.Models
{
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
    using System.Threading.Tasks;
    using System.Data.Entity;
    using System.Globalization;
    using log4net;

    public class CommissionDistributionHelper
    {
        //public string CommissionDistributon(long Mem_ID, string status, decimal Trans_Amt, decimal ChargeAmt, decimal OpeningAmt, string serviceprovider, string rechargeType)
        //{
        //    var db = new DbContext();
        //    return "";
        //}
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        #region Commission Distribution setting
        public string DistributeCommission(long Mem_ID, string status, decimal Trans_Amt, decimal ChargeAmt, decimal OpeningAmt, string serviceprovider, string rechargeType,string IpAddress)
        {
            var db = new DBContext();
            var taxMaster = db.TBL_TAX_MASTER.FirstOrDefault();
            decimal GST_AMount = 0;
            decimal.TryParse(taxMaster.TDS.ToString(), out GST_AMount);
            decimal TDS_AMount = 0;
            //decimal GST_AMount = decimal.Parse(System.Configuration.ConfigurationManager.AppSettings["GST_Amount"]);
            //decimal TDS_AMount = decimal.Parse(System.Configuration.ConfigurationManager.AppSettings["TDS_Amount"]);
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var DIS_MEM_ID = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == Mem_ID).Select(z => z.INTRODUCER).FirstOrDefault();
                    var SUP_MEM_ID = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == DIS_MEM_ID).Select(z => z.INTRODUCER).FirstOrDefault();
                    var WHT_MEM_ID = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == SUP_MEM_ID).Select(z => z.UNDER_WHITE_LEVEL).FirstOrDefault();
                    if (status == "MOBILE")
                    {
                        var MerchantComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                            join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                            on detailscom.SLN equals commslabMob.SLAB_ID
                                            join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                                            where commslabMob.OPERATOR_CODE == serviceprovider && detailscom.SLAB_TYPE == 1 && mem.MEMBER_ROLE == 5
                                            select new
                                            {
                                                SLN = commslabMob.SLN,
                                                commPer = commslabMob.MERCHANT_COM_PER,
                                                commType = commslabMob.COMMISSION_TYPE
                                            }).FirstOrDefault();

                        //var MerchantComm2 = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                    join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                        //                    on detailscom.RECHARGE_SLAB equals commslabMob.SLAB_ID
                        //                    where detailscom.WHITE_LEVEL_ID == DIS_MEM_ID && detailscom.INTRODUCER_ID == Mem_ID && commslabMob.OPERATOR_CODE == serviceprovider
                        //                    select new
                        //                    {
                        //                        SLN = commslabMob.SLN,
                        //                        commPer = commslabMob.COMM_PERCENTAGE
                        //                    }).FirstOrDefault();
                        //var MerchantComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                    join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                        //                    on detailscom.RECHARGE_SLAB equals commslabMob.SLAB_ID
                        //                    where detailscom.WHITE_LEVEL_ID == DIS_MEM_ID && detailscom.INTRODUCER_ID == Mem_ID && commslabMob.OPERATOR_CODE == serviceprovider
                        //                    select new
                        //                    {
                        //                        SLN = commslabMob.SLN,
                        //                        commPer = commslabMob.COMM_PERCENTAGE
                        //                    }).FirstOrDefault();
                        #region Retailer Commission                        
                        var membtype = (from mm in db.TBL_MASTER_MEMBER
                                        join
                                            rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                        where mm.MEM_ID == Mem_ID
                                        select new
                                        {
                                            RoleId = mm.MEMBER_ROLE,
                                            roleName = rm.ROLE_NAME,
                                            Amount = mm.BALANCE
                                        }).FirstOrDefault();
                        //var tbl_account = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == Mem_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        var tbl_account = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == Mem_ID).OrderByDescending(z => z.ACC_NO).FirstOrDefault();
                        if (tbl_account != null)
                        {
                            decimal ClosingAmt = tbl_account.CLOSING;
                            decimal SubAmt = ClosingAmt - Trans_Amt;
                            TBL_ACCOUNTS objaccnt = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Mem_ID,
                                MEMBER_TYPE = membtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "DR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Recharge",
                                OPENING = ClosingAmt,
                                CLOSING = SubAmt,
                                REC_NO = 0,
                                COMM_AMT = 0,
                                GST = 0,
                                TDS = 0,
                                IPAddress=IpAddress
                            };
                            db.TBL_ACCOUNTS.Add(objaccnt);
                            db.SaveChanges();


                            decimal getPer_Val = 0;
                            //float FloatgetPer_Val = float.Parse(getPer_Val.ToString());
                            // Get TDS and GST
                            decimal RetailerTDS_AmtValue = 0;
                            decimal getPer = 0;
                            if (MerchantComm.commType == "FIXED")
                            {
                                getPer_Val = MerchantComm.commPer;
                                RetailerTDS_AmtValue = decimal.Round(((getPer_Val * GST_AMount) / 100),2);
                                getPer = getPer_Val - RetailerTDS_AmtValue;
                            }
                            else
                            {
                                getPer_Val = (Trans_Amt * MerchantComm.commPer) / 100;
                                RetailerTDS_AmtValue = decimal.Round(((getPer_Val * GST_AMount) / 100), 2);
                                getPer = getPer_Val - RetailerTDS_AmtValue;
                            }
                            //float RetailerTDS_AmtValue = (FloatgetPer_Val * GST_AMount) / 100;
                            //double getPer = float.Parse(getPer_Val) - RetailerTDS_AmtValue;

                            decimal CommAddClosing = SubAmt + getPer;
                            TBL_ACCOUNTS objCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Mem_ID,
                                MEMBER_TYPE = membtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = SubAmt,
                                CLOSING = CommAddClosing,
                                REC_NO = 0,
                                COMM_AMT = getPer,
                                GST = 0,
                                TDS = (float) RetailerTDS_AmtValue,
                                IPAddress = IpAddress
                            };
                            db.TBL_ACCOUNTS.Add(objCommPer);
                            db.SaveChanges();
                        }
                        #endregion

                        var DistributorComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                               join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                               on detailscom.SLN equals commslabMob.SLAB_ID
                                               join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                                               where commslabMob.OPERATOR_CODE == serviceprovider && detailscom.SLAB_TYPE == 1 && mem.MEMBER_ROLE == 4
                                               select new
                                               {
                                                   SLN = commslabMob.SLN,
                                                   commPer = commslabMob.DISTRIBUTOR_COM_PER,
                                                   commType=commslabMob.COMMISSION_TYPE
                                               }).FirstOrDefault();
                        //var DistributorComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                       join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                        //                       on detailscom.RECHARGE_SLAB equals commslabMob.SLAB_ID
                        //                       where detailscom.WHITE_LEVEL_ID == SUP_MEM_ID && detailscom.INTRODUCER_ID == DIS_MEM_ID && commslabMob.OPERATOR_CODE == serviceprovider
                        //                       select new
                        //                       {
                        //                           SLN = commslabMob.SLN,
                        //                           commPer = commslabMob.COMM_PERCENTAGE
                        //                       }).FirstOrDefault();
                        #region Distributor Commission
                        //decimal DisGapComm = decimal.Parse(DistributorComm.commPer.ToString()) - decimal.Parse(MerchantComm.commPer.ToString());

                        //var tbl_accountDistributor = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == DIS_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        var tbl_accountDistributor = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == DIS_MEM_ID).OrderByDescending(z => z.ACC_NO).FirstOrDefault();
                        if (tbl_accountDistributor != null)
                        {
                            var CheckDisCommission = db.TBL_DETAILS_MEMBER_COMMISSION_SLAB.Where(x => x.INTRODUCER_ID == DIS_MEM_ID).FirstOrDefault();
                            var Dismembtype = (from mm in db.TBL_MASTER_MEMBER
                                               join
                                                   rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                               where mm.MEM_ID == DIS_MEM_ID
                                               select new
                                               {
                                                   RoleId = mm.MEMBER_ROLE,
                                                   roleName = rm.ROLE_NAME,
                                                   Amount = mm.BALANCE
                                               }).FirstOrDefault();
                            decimal DisClosingAmt = tbl_accountDistributor.CLOSING;
                            //decimal SubAmt = ClosingAmt - Trans_Amt;                   
                            //decimal DisgetPer = (Trans_Amt * checkDiscomm.CommPercentage) / 100;

                            //  Distributor GST and TDS
                            //decimal CommDisAddClosing = tbl_accountDistributor.CLOSING + DisGapCommAmt;

                            decimal DisGapComm = 0;
                            //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;
                            decimal DisGapCommAmt_Val = 0;
                            decimal DisGapCommAmt_TDS = 0;
                            decimal DisGapCommAmt = 0;
                            if (DistributorComm.commType == "FIXED")
                            {
                                DisGapComm = decimal.Parse(DistributorComm.commPer.ToString());
                                DisGapCommAmt_Val = DisGapComm;
                                DisGapCommAmt_TDS = decimal.Round(((DisGapCommAmt_Val * GST_AMount) / 100), 2);
                                DisGapCommAmt = DisGapCommAmt_Val - DisGapCommAmt_TDS;
                            }
                            else
                            {
                                DisGapComm = decimal.Parse(DistributorComm.commPer.ToString());
                                //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;
                                DisGapCommAmt_Val = (Trans_Amt * DisGapComm) / 100;
                                DisGapCommAmt_TDS = decimal.Round(((DisGapCommAmt_Val * GST_AMount) / 100), 2);
                                DisGapCommAmt = DisGapCommAmt_Val - DisGapCommAmt_TDS;
                            }

                            
                            decimal CommDisAddClosing = tbl_accountDistributor.CLOSING + DisGapCommAmt;



                            long Dis_ID = long.Parse(DIS_MEM_ID.ToString());
                            TBL_ACCOUNTS objDisCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Dis_ID,
                                MEMBER_TYPE = Dismembtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = DisClosingAmt,
                                CLOSING = CommDisAddClosing,
                                REC_NO = 0,
                                COMM_AMT = DisGapCommAmt,
                                GST = 0,
                                TDS =(float) DisGapCommAmt_TDS,
                                IPAddress = IpAddress
                            };
                            db.TBL_ACCOUNTS.Add(objDisCommPer);
                            db.SaveChanges();
                        }
                        #endregion
                        var SuperComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                         join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                          on detailscom.SLN equals commslabMob.SLAB_ID
                                         join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                                         where commslabMob.OPERATOR_CODE == serviceprovider && detailscom.SLAB_TYPE == 1 && mem.MEMBER_ROLE == 3
                                         select new
                                         {
                                             SLN = commslabMob.SLN,
                                             commPer = commslabMob.SUPER_COM_PER,
                                             commType= commslabMob.COMMISSION_TYPE
                                         }).FirstOrDefault();
                        //var SuperComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                 join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                        //                  on detailscom.RECHARGE_SLAB equals commslabMob.SLAB_ID
                        //                 where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == SUP_MEM_ID && commslabMob.OPERATOR_CODE == serviceprovider
                        //                 select new
                        //                 {
                        //                     SLN = commslabMob.SLN,
                        //                     commPer = commslabMob.COMM_PERCENTAGE
                        //                 }).FirstOrDefault();
                        //decimal SupGapComm = decimal.Parse(SuperComm.commPer.ToString()) - decimal.Parse(DistributorComm.commPer.ToString());

                        #region Super Commission
                        //var tbl_accountSuper = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == SUP_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        var tbl_accountSuper = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == SUP_MEM_ID).OrderByDescending(z => z.ACC_NO).FirstOrDefault();
                        if (tbl_accountSuper != null)
                        {
                            var Supmembtype = (from mm in db.TBL_MASTER_MEMBER
                                               join
                                                   rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                               where mm.MEM_ID == SUP_MEM_ID
                                               select new
                                               {
                                                   RoleId = mm.MEMBER_ROLE,
                                                   roleName = rm.ROLE_NAME,
                                                   Amount = mm.BALANCE
                                               }).FirstOrDefault();
                            decimal SupClosingAmt = tbl_accountSuper.CLOSING;

                            decimal SupGapComm = 0;
                            //decimal SupGapCommAmt = (Trans_Amt * SupGapComm) / 100;
                            // GST and TDS Calculation for Super
                            decimal SupGapCommAmt_val = 0;
                            decimal Sp_TDSAmt = 0;
                            decimal SupGapCommAmt = 0;
                            if (SuperComm.commType == "FIXED")
                            {
                                SupGapComm = decimal.Parse(SuperComm.commPer.ToString());
                                //decimal SupGapCommAmt = (Trans_Amt * SupGapComm) / 100;
                                // GST and TDS Calculation for Super
                                SupGapCommAmt_val = SupGapComm;
                                Sp_TDSAmt = decimal.Round(((SupGapCommAmt_val * GST_AMount) / 100), 2);
                                SupGapCommAmt = SupGapCommAmt_val - Sp_TDSAmt;
                            }
                            else
                            {
                                SupGapComm = decimal.Parse(SuperComm.commPer.ToString());
                                //decimal SupGapCommAmt = (Trans_Amt * SupGapComm) / 100;
                                // GST and TDS Calculation for Super
                                SupGapCommAmt_val = (Trans_Amt * SupGapComm) / 100;
                                Sp_TDSAmt = decimal.Round(((SupGapCommAmt_val * GST_AMount) / 100), 2);
                                SupGapCommAmt = SupGapCommAmt_val - Sp_TDSAmt;
                            }                           

                            decimal CommSupAddClosing = tbl_accountSuper.CLOSING + SupGapCommAmt;
                            long Sup_ID = long.Parse(SUP_MEM_ID.ToString());
                            TBL_ACCOUNTS objSupCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Sup_ID,
                                MEMBER_TYPE = Supmembtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = SupClosingAmt,
                                CLOSING = CommSupAddClosing,
                                REC_NO = 0,
                                COMM_AMT = SupGapCommAmt,
                                GST = 0,
                                TDS = (float) Sp_TDSAmt,
                                IPAddress = IpAddress
                            };
                            db.TBL_ACCOUNTS.Add(objSupCommPer);
                            db.SaveChanges();
                        }
                        #endregion                        
                        var WhiteComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                                         join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                          on detailscom.RECHARGE_SLAB equals commslabMob.SLAB_ID
                                         where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == 0 && commslabMob.OPERATOR_CODE == serviceprovider
                                         select new
                                         {
                                             SLN = commslabMob.SLN,
                                             commPer = commslabMob.COMM_PERCENTAGE,
                                             commType=commslabMob.COMMISSION_TYPE
                                         }).FirstOrDefault();
                        decimal WLComm = decimal.Parse(MerchantComm.commPer.ToString()) + decimal.Parse(DistributorComm.commPer.ToString()) + decimal.Parse(SuperComm.commPer.ToString());

                        //decimal WTLGapComm = decimal.Parse(WhiteComm.commPer.ToString());
                        //decimal WTLGapComm =WLComm;

                        #region White level Commission payment Structure
                        //var tbl_accountWhiteLevel = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == WHT_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        var tbl_accountWhiteLevel = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == WHT_MEM_ID).OrderByDescending(z => z.ACC_NO).FirstOrDefault();
                        if (tbl_accountWhiteLevel != null)
                        {
                            var WLmembtype = (from mm in db.TBL_MASTER_MEMBER
                                              join
                                                  rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                              where mm.MEM_ID == WHT_MEM_ID
                                              select new
                                              {
                                                  RoleId = mm.MEMBER_ROLE,
                                                  roleName = rm.ROLE_NAME,
                                                  Amount = mm.BALANCE
                                              }).FirstOrDefault();
                            decimal WLClosingAmt = tbl_accountWhiteLevel.CLOSING;
                            //decimal SubAmt = ClosingAmt - Trans_Amt;                  
                            // GST And TDS WhiteLevel Calculation
                            decimal WTLGapComm = decimal.Parse(WhiteComm.commPer.ToString()) - WLComm;
                            decimal WTLGapCommAmt_AMt = 0;
                            decimal WL_TDSCalculation = 0;
                            decimal WTLGapCommAmt = 0;
                            if (WhiteComm.commType == "FIXED")
                            {
                                WTLGapCommAmt_AMt = WTLGapComm;
                                WL_TDSCalculation = decimal.Round(((WTLGapCommAmt_AMt * GST_AMount) / 100), 2);
                                WTLGapCommAmt = WTLGapCommAmt_AMt - WL_TDSCalculation;
                            }
                            else
                            {
                                WTLGapCommAmt_AMt = (Trans_Amt * WTLGapComm) / 100;
                                WL_TDSCalculation = decimal.Round(((WTLGapCommAmt_AMt * GST_AMount) / 100), 2);
                                WTLGapCommAmt = WTLGapCommAmt_AMt - WL_TDSCalculation;
                            }
                            //decimal WTLGapCommAmt = (Trans_Amt * WTLGapComm) / 100;
                            decimal CommWLAddClosing = tbl_accountWhiteLevel.CLOSING + WTLGapCommAmt;
                            long WL_ID = long.Parse(WHT_MEM_ID.ToString());
                            TBL_ACCOUNTS objWLCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = WL_ID,
                                MEMBER_TYPE = WLmembtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = WLClosingAmt,
                                CLOSING = CommWLAddClosing,
                                REC_NO = 0,
                                COMM_AMT = WTLGapCommAmt,
                                GST = 0,
                                TDS = (float) WL_TDSCalculation,
                                IPAddress = IpAddress
                            };
                            db.TBL_ACCOUNTS.Add(objWLCommPer);
                            db.SaveChanges();
                        }
                        #endregion
                        ContextTransaction.Commit();
                        return "Success";
                    }
                    else if (status == "UTILITY")
                    {
                        var MerchantComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                            join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                            on detailscom.SLN equals commslabMob.SLAB_ID
                                            join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                                            where commslabMob.OPERATOR_CODE == serviceprovider && detailscom.SLAB_TYPE == 2 && mem.MEMBER_ROLE == 5
                                            select new
                                            {
                                                SLN = commslabMob.SLN,
                                                commPer = commslabMob.MERCHANT_COM_PER,
                                                commType=commslabMob.COMMISSION_TYPE
                                            }).FirstOrDefault();
                        //var MerchantComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                    join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
                        //                    on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
                        //                    where detailscom.WHITE_LEVEL_ID == DIS_MEM_ID && detailscom.INTRODUCER_ID == Mem_ID && commslabMob.OPERATOR_CODE == serviceprovider
                        //                    select new
                        //                    {
                        //                        SLN = commslabMob.SLN,
                        //                        commPer = commslabMob.COMM_PERCENTAGE
                        //                    }).FirstOrDefault();
                        #region Retailer Commission                        
                        var membtype = (from mm in db.TBL_MASTER_MEMBER
                                        join
                                            rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                        where mm.MEM_ID == Mem_ID
                                        select new
                                        {
                                            RoleId = mm.MEMBER_ROLE,
                                            roleName = rm.ROLE_NAME,
                                            Amount = mm.BALANCE
                                        }).FirstOrDefault();
                        //var tbl_account = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == Mem_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        var tbl_account = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == Mem_ID).OrderByDescending(z => z.ACC_NO).FirstOrDefault();
                        if (tbl_account != null)
                        {
                            decimal ClosingAmt = tbl_account.CLOSING;
                            decimal SubAmt = ClosingAmt - Trans_Amt;
                            TBL_ACCOUNTS objaccnt = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Mem_ID,
                                MEMBER_TYPE = membtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "DR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Recharge",
                                OPENING = ClosingAmt,
                                CLOSING = SubAmt,
                                REC_NO = 0,
                                COMM_AMT = 0,
                                GST = 0,
                                TDS = 0,
                                IPAddress = IpAddress
                            };
                            db.TBL_ACCOUNTS.Add(objaccnt);
                            db.SaveChanges();

                            //GST And TDS Calculation Section for Retailer
                            //decimal getPer = (Trans_Amt * MerchantComm.commPer) / 100;

                            decimal getPer_Value = 0;
                            decimal MerchantTDSValue =0;
                            decimal getPer = 0;
                            if (MerchantComm.commType == "FIXED")
                            {
                                 getPer_Value = MerchantComm.commPer;
                                 MerchantTDSValue = decimal.Round(((getPer_Value * GST_AMount) / 100), 2);
                                 getPer = getPer_Value - MerchantTDSValue;
                            }
                            else
                            {
                                 getPer_Value = (Trans_Amt * MerchantComm.commPer) / 100;
                                 MerchantTDSValue = decimal.Round(((getPer_Value * GST_AMount) / 100), 2);
                                 getPer = getPer_Value - MerchantTDSValue;
                            }

                            

                            decimal CommAddClosing = SubAmt + getPer;


                            TBL_ACCOUNTS objCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Mem_ID,
                                MEMBER_TYPE = membtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = SubAmt,
                                CLOSING = CommAddClosing,
                                REC_NO = 0,
                                COMM_AMT = getPer,
                                GST = 0,
                                TDS = (float) MerchantTDSValue,
                                IPAddress = IpAddress
                            };
                            db.TBL_ACCOUNTS.Add(objCommPer);
                            db.SaveChanges();
                        }
                        #endregion

                        var DistributorComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                               join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                               on detailscom.SLN equals commslabMob.SLAB_ID
                                               join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                                               where commslabMob.OPERATOR_CODE == serviceprovider && detailscom.SLAB_TYPE == 2 && mem.MEMBER_ROLE == 4
                                               select new
                                               {
                                                   SLN = commslabMob.SLN,
                                                   commPer = commslabMob.DISTRIBUTOR_COM_PER,
                                                   commType=commslabMob.COMMISSION_TYPE
                                               }).FirstOrDefault();
                        //var DistributorComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                       join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
                        //                       on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
                        //                       where detailscom.WHITE_LEVEL_ID == SUP_MEM_ID && detailscom.INTRODUCER_ID == DIS_MEM_ID && commslabMob.OPERATOR_CODE == serviceprovider
                        //                       select new
                        //                       {
                        //                           SLN = commslabMob.SLN,
                        //                           commPer = commslabMob.COMM_PERCENTAGE
                        //                       }).FirstOrDefault();
                        #region Distributor Commission
                        //decimal DisGapComm = decimal.Parse(DistributorComm.commPer.ToString()) - decimal.Parse(MerchantComm.commPer.ToString());

                        //var tbl_accountDistributor = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == DIS_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        var tbl_accountDistributor = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == DIS_MEM_ID).OrderByDescending(z => z.ACC_NO).FirstOrDefault();
                        if (tbl_accountDistributor != null)
                        {
                            var CheckDisCommission = db.TBL_DETAILS_MEMBER_COMMISSION_SLAB.Where(x => x.INTRODUCER_ID == DIS_MEM_ID).FirstOrDefault();
                            var Dismembtype = (from mm in db.TBL_MASTER_MEMBER
                                               join
                                                   rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                               where mm.MEM_ID == DIS_MEM_ID
                                               select new
                                               {
                                                   RoleId = mm.MEMBER_ROLE,
                                                   roleName = rm.ROLE_NAME,
                                                   Amount = mm.BALANCE
                                               }).FirstOrDefault();
                            decimal DisClosingAmt = tbl_accountDistributor.CLOSING;
                            //decimal SubAmt = ClosingAmt - Trans_Amt;                   
                            //decimal DisgetPer = (Trans_Amt * checkDiscomm.CommPercentage) / 100;

                            // GST and TDS Calculation for Distributor

                            decimal DisGapComm = 0;
                            //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;
                            decimal UDisGapCommAmt_Value =0;
                            decimal UDis_TDSCal = 0;
                            decimal DisGapCommAmt = 0;

                            if (DistributorComm.commType == "FIXED")
                            {
                                 DisGapComm = decimal.Parse(DistributorComm.commPer.ToString());
                                //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;
                                 UDisGapCommAmt_Value = DisGapComm;
                                 UDis_TDSCal = decimal.Round(((UDisGapCommAmt_Value * GST_AMount) / 100), 2);
                                 DisGapCommAmt = UDisGapCommAmt_Value - UDis_TDSCal;
                            }
                            else
                            {
                                 DisGapComm = decimal.Parse(DistributorComm.commPer.ToString());
                                //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;
                                 UDisGapCommAmt_Value = (Trans_Amt * DisGapComm) / 100;
                                 UDis_TDSCal = decimal.Round(((UDisGapCommAmt_Value * GST_AMount) / 100), 2);
                                 DisGapCommAmt = UDisGapCommAmt_Value - UDis_TDSCal;
                            }
                       

                            decimal CommDisAddClosing = tbl_accountDistributor.CLOSING + DisGapCommAmt;
                            long Dis_ID = long.Parse(DIS_MEM_ID.ToString());
                            TBL_ACCOUNTS objDisCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Dis_ID,
                                MEMBER_TYPE = Dismembtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = DisClosingAmt,
                                CLOSING = CommDisAddClosing,
                                REC_NO = 0,
                                COMM_AMT = DisGapCommAmt,
                                GST = 0,
                                TDS = (float)UDis_TDSCal,
                                IPAddress = IpAddress
                            };
                            db.TBL_ACCOUNTS.Add(objDisCommPer);
                            db.SaveChanges();
                        }
                        #endregion
                        //var SuperComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                 join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
                        //                  on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
                        //                 where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == SUP_MEM_ID && commslabMob.OPERATOR_CODE == serviceprovider
                        //                 select new
                        //                 {
                        //                     SLN = commslabMob.SLN,
                        //                     commPer = commslabMob.COMM_PERCENTAGE
                        //                 }).FirstOrDefault();
                        var SuperComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                         join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                          on detailscom.SLN equals commslabMob.SLAB_ID
                                         join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                                         where commslabMob.OPERATOR_CODE == serviceprovider && detailscom.SLAB_TYPE == 2 && mem.MEMBER_ROLE == 3
                                         select new
                                         {
                                             SLN = commslabMob.SLN,
                                             commPer = commslabMob.SUPER_COM_PER,
                                             commType=commslabMob.COMMISSION_TYPE
                                         }).FirstOrDefault();
                        //decimal SupGapComm = decimal.Parse(SuperComm.commPer.ToString()) - decimal.Parse(DistributorComm.commPer.ToString());

                        #region Super Commission
                        //var tbl_accountSuper = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == SUP_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        var tbl_accountSuper = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == SUP_MEM_ID).OrderByDescending(z => z.ACC_NO).FirstOrDefault();
                        if (tbl_accountSuper != null)
                        {
                            var Supmembtype = (from mm in db.TBL_MASTER_MEMBER
                                               join
                                                   rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                               where mm.MEM_ID == SUP_MEM_ID
                                               select new
                                               {
                                                   RoleId = mm.MEMBER_ROLE,
                                                   roleName = rm.ROLE_NAME,
                                                   Amount = mm.BALANCE
                                               }).FirstOrDefault();
                            decimal SupClosingAmt = tbl_accountSuper.CLOSING;
                            //GST and TDS Calculation for Super
                            decimal SupGapComm = 0;
                            //decimal SupGapCommAmt = (Trans_Amt * SupGapComm) / 100;
                            decimal USupGapCommAmt_Val = 0;
                            decimal USuper_TDSCal = 0;
                            decimal SupGapCommAmt = 0;
                            if (SuperComm.commType == "FIXED")
                            {
                                SupGapComm = decimal.Parse(SuperComm.commPer.ToString());
                                //decimal SupGapCommAmt = (Trans_Amt * SupGapComm) / 100;
                                 USupGapCommAmt_Val = SupGapComm;
                                 USuper_TDSCal = decimal.Round(((USupGapCommAmt_Val * GST_AMount) / 100), 2);
                                 SupGapCommAmt = USupGapCommAmt_Val - USuper_TDSCal;
                            }
                            else
                            {
                                 SupGapComm = decimal.Parse(SuperComm.commPer.ToString());
                                //decimal SupGapCommAmt = (Trans_Amt * SupGapComm) / 100;
                                 USupGapCommAmt_Val = (Trans_Amt * SupGapComm) / 100;
                                 USuper_TDSCal = decimal.Round(((USupGapCommAmt_Val * GST_AMount) / 100), 2);
                                 SupGapCommAmt = USupGapCommAmt_Val - USuper_TDSCal;
                            }
                            decimal CommSupAddClosing = tbl_accountSuper.CLOSING + SupGapCommAmt;
                            long Sup_ID = long.Parse(SUP_MEM_ID.ToString());
                            TBL_ACCOUNTS objSupCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Sup_ID,
                                MEMBER_TYPE = Supmembtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = SupClosingAmt,
                                CLOSING = CommSupAddClosing,
                                REC_NO = 0,
                                COMM_AMT = SupGapCommAmt,
                                GST = 0,
                                TDS = (float)USuper_TDSCal,
                                IPAddress = IpAddress
                            };
                            db.TBL_ACCOUNTS.Add(objSupCommPer);
                            db.SaveChanges();
                        }
                        #endregion                        
                        var WhiteComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                                         join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
                                          on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
                                         where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == 0 && commslabMob.OPERATOR_CODE == serviceprovider
                                         select new
                                         {
                                             SLN = commslabMob.SLN,
                                             commPer = commslabMob.COMM_PERCENTAGE,
                                             commType=commslabMob.COMMISSION_TYPE
                                         }).FirstOrDefault();
                        decimal WL_Com = decimal.Parse(MerchantComm.commPer.ToString()) + decimal.Parse(DistributorComm.commPer.ToString()) + decimal.Parse(SuperComm.commPer.ToString());

                        decimal WTLGapComm = decimal.Parse(WhiteComm.commPer.ToString()) - WL_Com;
                        //decimal WTLGapComm = decimal.Parse(WhiteComm.commPer.ToString()) - decimal.Parse(SuperComm.commPer.ToString());
                        //decimal WTLGapCommAmt = (Trans_Amt * WTLGapComm) / 100;
                        #region White level Commission payment Structure
                        //var tbl_accountWhiteLevel = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == WHT_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        var tbl_accountWhiteLevel = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == WHT_MEM_ID).OrderByDescending(z => z.ACC_NO).FirstOrDefault();
                        if (tbl_accountWhiteLevel != null)
                        {
                            var WLmembtype = (from mm in db.TBL_MASTER_MEMBER
                                              join
                                                  rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                              where mm.MEM_ID == WHT_MEM_ID
                                              select new
                                              {
                                                  RoleId = mm.MEMBER_ROLE,
                                                  roleName = rm.ROLE_NAME,
                                                  Amount = mm.BALANCE
                                              }).FirstOrDefault();
                            decimal WLClosingAmt = tbl_accountWhiteLevel.CLOSING;
                            //TDS And Gst Calculation fir whitelevel
                            //decimal WTLGapCommAmt = (Trans_Amt * WTLGapComm) / 100;
                            decimal UWTLGap_CommAmtValue = 0;
                            decimal UWhiteLTDS =0;
                            decimal WTLGapCommAmt = 0;
                            if (WhiteComm.commType == "FIXED")
                            {
                                 UWTLGap_CommAmtValue = WTLGapComm;
                                 UWhiteLTDS = decimal.Round(((UWTLGap_CommAmtValue * GST_AMount) / 100), 2);
                                 WTLGapCommAmt = UWTLGap_CommAmtValue - UWhiteLTDS;
                            }
                            else
                            {
                                 UWTLGap_CommAmtValue = (Trans_Amt * WTLGapComm) / 100;
                                 UWhiteLTDS = decimal.Round(((UWTLGap_CommAmtValue * GST_AMount) / 100), 2);
                                 WTLGapCommAmt = UWTLGap_CommAmtValue - UWhiteLTDS;
                            }
                            //decimal SubAmt = ClosingAmt - Trans_Amt;                  
                            decimal CommWLAddClosing = tbl_accountWhiteLevel.CLOSING + WTLGapCommAmt;
                            long WL_ID = long.Parse(WHT_MEM_ID.ToString());
                            TBL_ACCOUNTS objWLCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = WL_ID,
                                MEMBER_TYPE = WLmembtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = WLClosingAmt,
                                CLOSING = CommWLAddClosing,
                                REC_NO = 0,
                                COMM_AMT = WTLGapCommAmt,
                                GST = 0,
                                TDS = (float)UWhiteLTDS,
                                IPAddress = IpAddress
                            };
                            db.TBL_ACCOUNTS.Add(objWLCommPer);
                            db.SaveChanges();
                        }
                        #endregion
                        ContextTransaction.Commit();
                        return "Success";
                    }
                    else if (status == "WATER")
                    {
                        //var MerchantComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                    join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
                        //                    on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
                        //                    where detailscom.WHITE_LEVEL_ID == DIS_MEM_ID && detailscom.INTRODUCER_ID == Mem_ID && commslabMob.OPERATOR_CODE == serviceprovider
                        //                    select new
                        //                    {
                        //                        SLN = commslabMob.SLN,
                        //                        commPer = commslabMob.COMM_PERCENTAGE
                        //                    }).FirstOrDefault();
                        var MerchantComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                            join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                            on detailscom.SLN equals commslabMob.SLAB_ID
                                            join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                                            where commslabMob.OPERATOR_CODE == serviceprovider && mem.MEMBER_ROLE == 5
                                            select new
                                            {
                                                SLN = commslabMob.SLN,
                                                commPer = commslabMob.MERCHANT_COM_PER,
                                                commType=commslabMob.COMMISSION_TYPE
                                            }).FirstOrDefault();
                        #region Retailer Commission                        
                        var membtype = (from mm in db.TBL_MASTER_MEMBER
                                        join
                                            rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                        where mm.MEM_ID == Mem_ID
                                        select new
                                        {
                                            RoleId = mm.MEMBER_ROLE,
                                            roleName = rm.ROLE_NAME,
                                            Amount = mm.BALANCE
                                        }).FirstOrDefault();
                        var tbl_account = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == Mem_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        if (tbl_account != null)
                        {
                            decimal ClosingAmt = tbl_account.CLOSING;
                            decimal SubAmt = ClosingAmt - Trans_Amt;
                            TBL_ACCOUNTS objaccnt = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Mem_ID,
                                MEMBER_TYPE = membtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "DR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Recharge",
                                OPENING = ClosingAmt,
                                CLOSING = SubAmt,
                                REC_NO = 0,
                                COMM_AMT = 0,
                                GST = 0,
                                TDS = 0,
                                IPAddress = IpAddress
                            };
                            db.TBL_ACCOUNTS.Add(objaccnt);
                            db.SaveChanges();
                            // For TDS and GST Calculaiton for Merchant
                            //decimal getPer = (Trans_Amt * MerchantComm.commPer) / 100;
                            decimal WA_getPer_Value = 0;
                            decimal WA_Merchant_TDS = 0;
                            decimal getPer = 0;
                            if (MerchantComm.commType == "FIXED")
                            {
                                 WA_getPer_Value = MerchantComm.commPer;
                                 WA_Merchant_TDS = decimal.Round(((WA_getPer_Value * GST_AMount) / 100), 2);
                                 getPer = WA_getPer_Value - WA_Merchant_TDS;
                            }
                            else
                            {
                                 WA_getPer_Value = (Trans_Amt * MerchantComm.commPer) / 100;
                                 WA_Merchant_TDS = decimal.Round(((WA_getPer_Value * GST_AMount) / 100), 2);
                                 getPer = WA_getPer_Value - WA_Merchant_TDS;
                            }

                            decimal CommAddClosing = SubAmt + getPer;
                            TBL_ACCOUNTS objCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Mem_ID,
                                MEMBER_TYPE = membtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = SubAmt,
                                CLOSING = CommAddClosing,
                                REC_NO = 0,
                                COMM_AMT = getPer,
                                GST = 0,
                                TDS = (float)WA_Merchant_TDS,
                                IPAddress = IpAddress
                            };
                            db.TBL_ACCOUNTS.Add(objCommPer);
                            db.SaveChanges();
                        }
                        #endregion

                        //var DistributorComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                       join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
                        //                       on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
                        //                       where detailscom.WHITE_LEVEL_ID == SUP_MEM_ID && detailscom.INTRODUCER_ID == DIS_MEM_ID && commslabMob.OPERATOR_CODE == serviceprovider
                        //                       select new
                        //                       {
                        //                           SLN = commslabMob.SLN,
                        //                           commPer = commslabMob.COMM_PERCENTAGE
                        //                       }).FirstOrDefault();
                        var DistributorComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                               join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                               on detailscom.SLN equals commslabMob.SLAB_ID
                                               join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                                               where commslabMob.OPERATOR_CODE == serviceprovider && mem.MEMBER_ROLE == 4
                                               select new
                                               {
                                                   SLN = commslabMob.SLN,
                                                   commPer = commslabMob.DISTRIBUTOR_COM_PER,
                                                   commType=commslabMob.COMMISSION_TYPE
                                               }).FirstOrDefault();
                        #region Distributor Commission
                        //decimal DisGapComm = decimal.Parse(DistributorComm.commPer.ToString()) - decimal.Parse(MerchantComm.commPer.ToString());
                        decimal DisGapComm = decimal.Parse(DistributorComm.commPer.ToString());
                        //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;
                        var tbl_accountDistributor = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == DIS_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        if (tbl_accountDistributor != null)
                        {
                            var CheckDisCommission = db.TBL_DETAILS_MEMBER_COMMISSION_SLAB.Where(x => x.INTRODUCER_ID == DIS_MEM_ID).FirstOrDefault();
                            var Dismembtype = (from mm in db.TBL_MASTER_MEMBER
                                               join
                                                   rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                               where mm.MEM_ID == DIS_MEM_ID
                                               select new
                                               {
                                                   RoleId = mm.MEMBER_ROLE,
                                                   roleName = rm.ROLE_NAME,
                                                   Amount = mm.BALANCE
                                               }).FirstOrDefault();
                            decimal DisClosingAmt = tbl_accountDistributor.CLOSING;
                            //decimal SubAmt = ClosingAmt - Trans_Amt;                   
                            //decimal DisgetPer = (Trans_Amt * checkDiscomm.CommPercentage) / 100;
                            // For GST AND TDS calculation for Dister for water
                            //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;

                            decimal WA_DisGapComm_Amt = 0;
                            decimal WA_Dis_TDS_CAL = 0;
                            decimal DisGapCommAmt =0;
                            if (DistributorComm.commType == "FIXED")
                            {
                                 WA_DisGapComm_Amt = DistributorComm.commPer;
                                 WA_Dis_TDS_CAL = decimal.Round(((WA_DisGapComm_Amt * GST_AMount) / 100), 2);
                                 DisGapCommAmt = WA_DisGapComm_Amt - WA_Dis_TDS_CAL;
                            }
                            else {
                                 WA_DisGapComm_Amt = (Trans_Amt * DisGapComm) / 100;
                                 WA_Dis_TDS_CAL = decimal.Round(((WA_DisGapComm_Amt * GST_AMount) / 100), 2);
                                 DisGapCommAmt = WA_DisGapComm_Amt - WA_Dis_TDS_CAL;
                            }

                            decimal CommDisAddClosing = tbl_accountDistributor.CLOSING + DisGapCommAmt;
                            long Dis_ID = long.Parse(DIS_MEM_ID.ToString());
                            TBL_ACCOUNTS objDisCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Dis_ID,
                                MEMBER_TYPE = Dismembtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = DisClosingAmt,
                                CLOSING = CommDisAddClosing,
                                REC_NO = 0,
                                COMM_AMT = DisGapCommAmt,
                                GST = 0,
                                TDS = (float)WA_Dis_TDS_CAL,
                                IPAddress = IpAddress
                            };
                            db.TBL_ACCOUNTS.Add(objDisCommPer);
                            db.SaveChanges();
                        }
                        #endregion
                        //var SuperComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                 join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
                        //                  on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
                        //                 where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == SUP_MEM_ID && commslabMob.OPERATOR_CODE == serviceprovider
                        //                 select new
                        //                 {
                        //                     SLN = commslabMob.SLN,
                        //                     commPer = commslabMob.COMM_PERCENTAGE
                        //                 }).FirstOrDefault();
                        var SuperComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                         join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                          on detailscom.SLN equals commslabMob.SLAB_ID
                                         join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                                         where commslabMob.OPERATOR_CODE == serviceprovider && mem.MEMBER_ROLE == 3
                                         select new
                                         {
                                             SLN = commslabMob.SLN,
                                             commPer = commslabMob.SUPER_COM_PER,
                                             commType=commslabMob.COMMISSION_TYPE
                                         }).FirstOrDefault();
                        //decimal SupGapComm = decimal.Parse(SuperComm.commPer.ToString()) - decimal.Parse(DistributorComm.commPer.ToString());
                        decimal SupGapComm = decimal.Parse(SuperComm.commPer.ToString());
                        //decimal SupGapCommAmt = (Trans_Amt * SupGapComm) / 100;
                        #region Super Commission
                        var tbl_accountSuper = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == SUP_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        if (tbl_accountSuper != null)
                        {
                            var Supmembtype = (from mm in db.TBL_MASTER_MEMBER
                                               join
                                                   rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                               where mm.MEM_ID == SUP_MEM_ID
                                               select new
                                               {
                                                   RoleId = mm.MEMBER_ROLE,
                                                   roleName = rm.ROLE_NAME,
                                                   Amount = mm.BALANCE
                                               }).FirstOrDefault();
                            decimal SupClosingAmt = tbl_accountSuper.CLOSING;
                            // TDS and GST CAlculation for Super for Water
                            decimal WA_Sup_Gap_CommAmt = 0;
                            decimal WA_Sup_TDS_CAL_Val =0;
                            decimal SupGapCommAmt = 0;
                            if (SuperComm.commType == "FIXED")
                            {
                                WA_Sup_Gap_CommAmt = SuperComm.commPer;
                                WA_Sup_TDS_CAL_Val = decimal.Round(((WA_Sup_Gap_CommAmt * GST_AMount) / 100), 2);
                                SupGapCommAmt = (WA_Sup_Gap_CommAmt - WA_Sup_TDS_CAL_Val);
                            }
                            else
                            {
                                WA_Sup_Gap_CommAmt = (Trans_Amt * SupGapComm) / 100;
                                WA_Sup_TDS_CAL_Val = decimal.Round(((WA_Sup_Gap_CommAmt * GST_AMount) / 100), 2);
                                SupGapCommAmt = (WA_Sup_Gap_CommAmt - WA_Sup_TDS_CAL_Val);
                            }

                            decimal CommSupAddClosing = tbl_accountSuper.CLOSING + SupGapCommAmt;
                            long Sup_ID = long.Parse(SUP_MEM_ID.ToString());
                            TBL_ACCOUNTS objSupCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Sup_ID,
                                MEMBER_TYPE = Supmembtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = SupClosingAmt,
                                CLOSING = CommSupAddClosing,
                                REC_NO = 0,
                                COMM_AMT = SupGapCommAmt,
                                GST = 0,
                                TDS = (float)WA_Sup_TDS_CAL_Val,
                                IPAddress = IpAddress
                            };
                            db.TBL_ACCOUNTS.Add(objSupCommPer);
                            db.SaveChanges();
                        }
                        #endregion                        
                        var WhiteComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                                         join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
                                          on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
                                         where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == 0 && commslabMob.OPERATOR_CODE == serviceprovider
                                         select new
                                         {
                                             SLN = commslabMob.SLN,
                                             commPer = commslabMob.COMM_PERCENTAGE,
                                             commType=commslabMob.COMMISSION_TYPE
                                         }).FirstOrDefault();
                        decimal Gap_WLlevel = decimal.Parse(MerchantComm.commPer.ToString()) + decimal.Parse(DistributorComm.commPer.ToString()) + decimal.Parse(SuperComm.commPer.ToString());

                        //decimal WTLGapComm = decimal.Parse(WhiteComm.commPer.ToString()) - decimal.Parse(SuperComm.commPer.ToString());
                        decimal WTLGapComm = decimal.Parse(WhiteComm.commPer.ToString()) - Gap_WLlevel;
                        //decimal WTLGapCommAmt = (Trans_Amt * WTLGapComm) / 100;
                        #region White level Commission payment Structure
                        var tbl_accountWhiteLevel = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == WHT_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        if (tbl_accountWhiteLevel != null)
                        {
                            var WLmembtype = (from mm in db.TBL_MASTER_MEMBER
                                              join
                                                  rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                              where mm.MEM_ID == WHT_MEM_ID
                                              select new
                                              {
                                                  RoleId = mm.MEMBER_ROLE,
                                                  roleName = rm.ROLE_NAME,
                                                  Amount = mm.BALANCE
                                              }).FirstOrDefault();
                            decimal WLClosingAmt = tbl_accountWhiteLevel.CLOSING;
                            //decimal SubAmt = ClosingAmt - Trans_Amt;                  
                            // TDS and GST Calculation for white level Water
                            decimal WTL_Gap_Comm_Amt_Val = 0;
                            decimal WL_TDSCAl_VAlue = 0;
                            decimal WTLGapCommAmt =0;
                            if (WhiteComm.commType == "FIXED")
                            {
                                WTL_Gap_Comm_Amt_Val = WTLGapComm;
                                WL_TDSCAl_VAlue = decimal.Round(((WTL_Gap_Comm_Amt_Val * GST_AMount) / 100), 2);
                                WTLGapCommAmt = WTL_Gap_Comm_Amt_Val - WL_TDSCAl_VAlue;
                            }
                            else
                            {
                                WTL_Gap_Comm_Amt_Val = (Trans_Amt * WTLGapComm) / 100;
                                WL_TDSCAl_VAlue = decimal.Round(((WTL_Gap_Comm_Amt_Val * GST_AMount) / 100), 2);
                                WTLGapCommAmt = WTL_Gap_Comm_Amt_Val - WL_TDSCAl_VAlue;
                            }
                            decimal CommWLAddClosing = tbl_accountWhiteLevel.CLOSING + WTLGapCommAmt;
                            long WL_ID = long.Parse(WHT_MEM_ID.ToString());
                            TBL_ACCOUNTS objWLCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = WL_ID,
                                MEMBER_TYPE = WLmembtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = WLClosingAmt,
                                CLOSING = CommWLAddClosing,
                                REC_NO = 0,
                                COMM_AMT = WTLGapCommAmt,
                                GST = 0,
                                TDS = (float)WL_TDSCAl_VAlue,
                                IPAddress = IpAddress
                            };
                            db.TBL_ACCOUNTS.Add(objWLCommPer);
                            db.SaveChanges();
                        }
                        #endregion
                        ContextTransaction.Commit();
                        return "Success";
                    }
                    else if (status == "ELECTRICITY")
                    {
                        //var MerchantComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                    join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
                        //                    on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
                        //                    where detailscom.WHITE_LEVEL_ID == DIS_MEM_ID && detailscom.INTRODUCER_ID == Mem_ID && commslabMob.OPERATOR_CODE == serviceprovider
                        //                    select new
                        //                    {
                        //                        SLN = commslabMob.SLN,
                        //                        commPer = commslabMob.COMM_PERCENTAGE
                        //                    }).FirstOrDefault();
                        var MerchantComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                            join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                            on detailscom.SLN equals commslabMob.SLAB_ID
                                            join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                                            where commslabMob.OPERATOR_CODE == serviceprovider && mem.MEMBER_ROLE == 5
                                            select new
                                            {
                                                SLN = commslabMob.SLN,
                                                commPer = commslabMob.MERCHANT_COM_PER,
                                                commType=commslabMob.COMMISSION_TYPE
                                            }).FirstOrDefault();
                        #region Retailer Commission                        
                        var membtype = (from mm in db.TBL_MASTER_MEMBER
                                        join
                                            rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                        where mm.MEM_ID == Mem_ID
                                        select new
                                        {
                                            RoleId = mm.MEMBER_ROLE,
                                            roleName = rm.ROLE_NAME,
                                            Amount = mm.BALANCE
                                        }).FirstOrDefault();
                        var tbl_account = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == Mem_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        if (tbl_account != null)
                        {
                            decimal ClosingAmt = tbl_account.CLOSING;
                            decimal SubAmt = ClosingAmt - Trans_Amt;
                            TBL_ACCOUNTS objaccnt = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Mem_ID,
                                MEMBER_TYPE = membtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "DR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Recharge",
                                OPENING = ClosingAmt,
                                CLOSING = SubAmt,
                                REC_NO = 0,
                                COMM_AMT = 0,
                                GST = 0,
                                TDS = 0,
                                IPAddress = IpAddress
                            };
                            db.TBL_ACCOUNTS.Add(objaccnt);
                            db.SaveChanges();
                            // TDS and GST CAlculation for Merchant for electricity
                            //decimal getPer = (Trans_Amt * MerchantComm.commPer) / 100;
                            decimal ELE_getPer_Val = 0;
                            decimal ELE_TDS_Value = 0;
                            decimal getPer = 0;

                            if (MerchantComm.commType == "FIXED")
                            {
                                ELE_getPer_Val = MerchantComm.commPer;
                                 ELE_TDS_Value = decimal.Round(((ELE_getPer_Val * GST_AMount) / 100), 2);
                                 getPer = (ELE_getPer_Val - ELE_TDS_Value);
                            }
                            else
                            {
                                 ELE_getPer_Val = (Trans_Amt * MerchantComm.commPer) / 100;
                                 ELE_TDS_Value = decimal.Round(((ELE_getPer_Val * GST_AMount) / 100), 2);
                                 getPer = (ELE_getPer_Val - ELE_TDS_Value);
                            }

                            decimal CommAddClosing = SubAmt + getPer;
                            TBL_ACCOUNTS objCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Mem_ID,
                                MEMBER_TYPE = membtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = SubAmt,
                                CLOSING = CommAddClosing,
                                REC_NO = 0,
                                COMM_AMT = getPer,
                                GST = 0,
                                TDS = (float)ELE_TDS_Value,
                                IPAddress = IpAddress
                            };
                            db.TBL_ACCOUNTS.Add(objCommPer);
                            db.SaveChanges();
                        }
                        #endregion

                        //var DistributorComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                       join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
                        //                       on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
                        //                       where detailscom.WHITE_LEVEL_ID == SUP_MEM_ID && detailscom.INTRODUCER_ID == DIS_MEM_ID && commslabMob.OPERATOR_CODE == serviceprovider
                        //                       select new
                        //                       {
                        //                           SLN = commslabMob.SLN,
                        //                           commPer = commslabMob.COMM_PERCENTAGE
                        //                       }).FirstOrDefault();
                        var DistributorComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                               join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                               on detailscom.SLN equals commslabMob.SLAB_ID
                                               join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                                               where commslabMob.OPERATOR_CODE == serviceprovider && mem.MEMBER_ROLE == 4
                                               select new
                                               {
                                                   SLN = commslabMob.SLN,
                                                   commPer = commslabMob.DISTRIBUTOR_COM_PER,
                                                   commType=commslabMob.COMMISSION_TYPE
                                               }).FirstOrDefault();
                        #region Distributor Commission
                        //decimal DisGapComm = decimal.Parse(DistributorComm.commPer.ToString()) - decimal.Parse(MerchantComm.commPer.ToString());
                        decimal DisGapComm = decimal.Parse(DistributorComm.commPer.ToString());
                        //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;
                        var tbl_accountDistributor = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == DIS_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        if (tbl_accountDistributor != null)
                        {
                            var CheckDisCommission = db.TBL_DETAILS_MEMBER_COMMISSION_SLAB.Where(x => x.INTRODUCER_ID == DIS_MEM_ID).FirstOrDefault();
                            var Dismembtype = (from mm in db.TBL_MASTER_MEMBER
                                               join
                                                   rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                               where mm.MEM_ID == DIS_MEM_ID
                                               select new
                                               {
                                                   RoleId = mm.MEMBER_ROLE,
                                                   roleName = rm.ROLE_NAME,
                                                   Amount = mm.BALANCE
                                               }).FirstOrDefault();
                            decimal DisClosingAmt = tbl_accountDistributor.CLOSING;
                            //decimal SubAmt = ClosingAmt - Trans_Amt;                   
                            //decimal DisgetPer = (Trans_Amt * checkDiscomm.CommPercentage) / 100;

                            // For TDS and GST CAlculation for Electricity
                            //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;
                            decimal ELE_DisGap_CommAmt = 0;
                            decimal ELE_TDSCALCULATION = 0;
                            decimal DisGapCommAmt = 0;
                            if (DistributorComm.commType == "FIXED")
                            {
                                 ELE_DisGap_CommAmt = DisGapComm;
                                 ELE_TDSCALCULATION = decimal.Round(((ELE_DisGap_CommAmt * GST_AMount) / 100), 2);
                                 DisGapCommAmt = ELE_DisGap_CommAmt - ELE_TDSCALCULATION;
                            }
                            else
                            {
                                 ELE_DisGap_CommAmt = (Trans_Amt * DisGapComm) / 100;
                                 ELE_TDSCALCULATION = decimal.Round(((ELE_DisGap_CommAmt * GST_AMount) / 100), 2);
                                 DisGapCommAmt = ELE_DisGap_CommAmt - ELE_TDSCALCULATION;
                            }

                            decimal CommDisAddClosing = tbl_accountDistributor.CLOSING + DisGapCommAmt;
                            long Dis_ID = long.Parse(DIS_MEM_ID.ToString());
                            TBL_ACCOUNTS objDisCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Dis_ID,
                                MEMBER_TYPE = Dismembtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = DisClosingAmt,
                                CLOSING = CommDisAddClosing,
                                REC_NO = 0,
                                COMM_AMT = DisGapCommAmt,
                                GST = 0,
                                TDS = (float)ELE_TDSCALCULATION,
                                IPAddress = IpAddress
                            };
                            db.TBL_ACCOUNTS.Add(objDisCommPer);
                            db.SaveChanges();
                        }
                        #endregion
                        //var SuperComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                 join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
                        //                  on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
                        //                 where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == SUP_MEM_ID && commslabMob.OPERATOR_CODE == serviceprovider
                        //                 select new
                        //                 {
                        //                     SLN = commslabMob.SLN,
                        //                     commPer = commslabMob.COMM_PERCENTAGE
                        //                 }).FirstOrDefault();
                        var SuperComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                         join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                          on detailscom.SLN equals commslabMob.SLAB_ID
                                         join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                                         where commslabMob.OPERATOR_CODE == serviceprovider && mem.MEMBER_ROLE == 3
                                         select new
                                         {
                                             SLN = commslabMob.SLN,
                                             commPer = commslabMob.SUPER_COM_PER,
                                             commType=commslabMob.COMMISSION_TYPE
                                         }).FirstOrDefault();
                        //decimal SupGapComm = decimal.Parse(SuperComm.commPer.ToString()) - decimal.Parse(DistributorComm.commPer.ToString());
                        decimal SupGapComm = decimal.Parse(SuperComm.commPer.ToString());
                        //decimal SupGapCommAmt = (Trans_Amt * SupGapComm) / 100;
                        #region Super Commission
                        var tbl_accountSuper = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == SUP_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        if (tbl_accountSuper != null)
                        {
                            var Supmembtype = (from mm in db.TBL_MASTER_MEMBER
                                               join
                                                   rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                               where mm.MEM_ID == SUP_MEM_ID
                                               select new
                                               {
                                                   RoleId = mm.MEMBER_ROLE,
                                                   roleName = rm.ROLE_NAME,
                                                   Amount = mm.BALANCE
                                               }).FirstOrDefault();
                            decimal SupClosingAmt = tbl_accountSuper.CLOSING;
                            // TDS AND GST CALCULATION FOR SUPER FOR ELECTRICITY
                            decimal ELE_Sup_Gap_Comm_Amt = 0;
                            decimal ELE_SUP_TDS_CAL = 0;
                            decimal SupGapCommAmt =0;
                            if (SuperComm.commType == "FIXED")
                            {
                                 ELE_Sup_Gap_Comm_Amt = SupGapComm;
                                 ELE_SUP_TDS_CAL = decimal.Round(((ELE_Sup_Gap_Comm_Amt * GST_AMount) / 100), 2);
                                 SupGapCommAmt = ELE_Sup_Gap_Comm_Amt - ELE_SUP_TDS_CAL;
                            }
                            else
                            {
                                 ELE_Sup_Gap_Comm_Amt = (Trans_Amt * SupGapComm) / 100;
                                 ELE_SUP_TDS_CAL = decimal.Round(((ELE_Sup_Gap_Comm_Amt * GST_AMount) / 100), 2);
                                 SupGapCommAmt = ELE_Sup_Gap_Comm_Amt - ELE_SUP_TDS_CAL;
                            }

                            decimal CommSupAddClosing = tbl_accountSuper.CLOSING + SupGapCommAmt;
                            long Sup_ID = long.Parse(SUP_MEM_ID.ToString());
                            TBL_ACCOUNTS objSupCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Sup_ID,
                                MEMBER_TYPE = Supmembtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = SupClosingAmt,
                                CLOSING = CommSupAddClosing,
                                REC_NO = 0,
                                COMM_AMT = SupGapCommAmt,
                                GST = 0,
                                TDS = (float)ELE_SUP_TDS_CAL,
                                IPAddress = IpAddress
                            };
                            db.TBL_ACCOUNTS.Add(objSupCommPer);
                            db.SaveChanges();
                        }
                        #endregion                        
                        var WhiteComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                                         join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
                                          on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
                                         where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == 0 && commslabMob.OPERATOR_CODE == serviceprovider
                                         select new
                                         {
                                             SLN = commslabMob.SLN,
                                             commPer = commslabMob.COMM_PERCENTAGE,
                                             commType=commslabMob.COMMISSION_TYPE
                                         }).FirstOrDefault();
                        decimal WL_Com = decimal.Parse(MerchantComm.commPer.ToString()) + decimal.Parse(DistributorComm.commPer.ToString()) + decimal.Parse(SuperComm.commPer.ToString());
                        //decimal WTLGapComm = decimal.Parse(WhiteComm.commPer.ToString()) - decimal.Parse(SuperComm.commPer.ToString());
                        decimal WTLGapComm = decimal.Parse(WhiteComm.commPer.ToString()) - WL_Com;
                        //decimal WTLGapCommAmt = (Trans_Amt * WTLGapComm) / 100;
                        #region White level Commission payment Structure
                        var tbl_accountWhiteLevel = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == WHT_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        if (tbl_accountWhiteLevel != null)
                        {
                            var WLmembtype = (from mm in db.TBL_MASTER_MEMBER
                                              join
                                                  rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                              where mm.MEM_ID == WHT_MEM_ID
                                              select new
                                              {
                                                  RoleId = mm.MEMBER_ROLE,
                                                  roleName = rm.ROLE_NAME,
                                                  Amount = mm.BALANCE
                                              }).FirstOrDefault();
                            decimal WLClosingAmt = tbl_accountWhiteLevel.CLOSING;
                            //decimal SubAmt = ClosingAmt - Trans_Amt;           
                            // GST And TDS Calculation for WL 
                            //decimal WTLGapCommAmt = (Trans_Amt * WTLGapComm) / 100;
                            decimal ELE_WTL_Gap_CommAmt = 0;
                            decimal ELE_WL_TDSCal = 0;
                            decimal WTLGapCommAmt = 0;
                            if (WhiteComm.commType == "FIXED")
                            {
                                 ELE_WTL_Gap_CommAmt = WTLGapComm;
                                 ELE_WL_TDSCal = decimal.Round(((ELE_WTL_Gap_CommAmt * GST_AMount) / 100), 2);
                                 WTLGapCommAmt = ELE_WTL_Gap_CommAmt - ELE_WL_TDSCal;
                            }
                            else
                            {
                                 ELE_WTL_Gap_CommAmt = (Trans_Amt * WTLGapComm) / 100;
                                 ELE_WL_TDSCal = decimal.Round(((ELE_WTL_Gap_CommAmt * GST_AMount) / 100), 2);
                                 WTLGapCommAmt = ELE_WTL_Gap_CommAmt - ELE_WL_TDSCal;
                            }
                            decimal CommWLAddClosing = tbl_accountWhiteLevel.CLOSING + WTLGapCommAmt;
                            long WL_ID = long.Parse(WHT_MEM_ID.ToString());
                            TBL_ACCOUNTS objWLCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = WL_ID,
                                MEMBER_TYPE = WLmembtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = WLClosingAmt,
                                CLOSING = CommWLAddClosing,
                                REC_NO = 0,
                                COMM_AMT = WTLGapCommAmt,
                                GST = 0,
                                TDS = (float)ELE_WL_TDSCal,
                                IPAddress = IpAddress
                            };
                            db.TBL_ACCOUNTS.Add(objWLCommPer);
                            db.SaveChanges();
                        }
                        #endregion
                        ContextTransaction.Commit();
                        return "Success";
                    }
                    else if (status == "DMR")
                    {
                        var MerchantComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                                            join commslabMob in db.TBL_COMM_SLAB_DMR_PAYMENT
                                            on detailscom.DMR_SLAB equals commslabMob.SLAB_ID
                                            where detailscom.WHITE_LEVEL_ID == DIS_MEM_ID && detailscom.INTRODUCER_ID == Mem_ID && commslabMob.DMT_TYPE == "DOMESTIC"
                                            select new
                                            {
                                                SLN = commslabMob.SLN,
                                                Slab_ID = commslabMob.SLAB_ID,
                                                Slab_From = commslabMob.SLAB_FROM,
                                                Slab_To = commslabMob.SLAB_TO,
                                                commPer = commslabMob.COMM_PERCENTAGE,

                                            }).ToList();
                        decimal commamt = 0;

                        foreach (var comslab in MerchantComm)
                        {
                            if (Trans_Amt >= comslab.Slab_From && Trans_Amt <= comslab.Slab_To)
                            {
                                commamt = comslab.commPer;

                            }
                        }

                        #region Retailer Commission                        
                        var membtype = (from mm in db.TBL_MASTER_MEMBER
                                        join
                                            rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                        where mm.MEM_ID == Mem_ID
                                        select new
                                        {
                                            RoleId = mm.MEMBER_ROLE,
                                            roleName = rm.ROLE_NAME,
                                            Amount = mm.BALANCE
                                        }).FirstOrDefault();
                        //var tbl_account = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == Mem_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        var tbl_account = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == Mem_ID).OrderByDescending(z => z.ACC_NO).FirstOrDefault();
                        if (tbl_account != null)
                        {
                            decimal ClosingAmt = tbl_account.CLOSING;
                            decimal SubAmt = ClosingAmt - Trans_Amt;
                            TBL_ACCOUNTS objaccnt = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Mem_ID,
                                MEMBER_TYPE = membtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "DR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "DMT",
                                OPENING = ClosingAmt,
                                CLOSING = SubAmt,
                                REC_NO = 0,
                                COMM_AMT = 0,
                                GST = 0,
                                TDS = 0,
                                IPAddress = IpAddress
                            };
                            db.TBL_ACCOUNTS.Add(objaccnt);
                            db.SaveChanges();
                            //decimal getPer = (Trans_Amt * MerchantComm.commPer) / 100;
                            decimal getPer = 0;

                            decimal CommAddClosing = SubAmt - commamt;
                            TBL_ACCOUNTS objCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Mem_ID,
                                MEMBER_TYPE = membtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "DR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = SubAmt,
                                CLOSING = CommAddClosing,
                                REC_NO = 0,
                                COMM_AMT = commamt,
                                GST = 0,
                                TDS = 0,
                                IPAddress = IpAddress
                            };
                            db.TBL_ACCOUNTS.Add(objCommPer);
                            db.SaveChanges();
                        }
                        #endregion

                        var DistributorComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                                               join commslabMob in db.TBL_COMM_SLAB_DMR_PAYMENT
                                               on detailscom.DMR_SLAB equals commslabMob.SLAB_ID
                                               where detailscom.WHITE_LEVEL_ID == SUP_MEM_ID && detailscom.INTRODUCER_ID == DIS_MEM_ID && commslabMob.DMT_TYPE == "DOMESTIC"
                                               select new
                                               {
                                                   SLN = commslabMob.SLN,
                                                   Slab_ID = commslabMob.SLAB_ID,
                                                   Slab_From = commslabMob.SLAB_FROM,
                                                   Slab_To = commslabMob.SLAB_TO,
                                                   commPer = commslabMob.COMM_PERCENTAGE,
                                               }).ToList();
                        decimal DistributorDMRComm = 0;
                        foreach (var comslab in DistributorComm)
                        {
                            if (Trans_Amt >= comslab.Slab_From && Trans_Amt <= comslab.Slab_To)
                            {
                                DistributorDMRComm = comslab.commPer;
                            }
                        }

                        decimal DmrDisgapcomm = 0;
                        DmrDisgapcomm = commamt - DistributorDMRComm;

                        #region Distributor DMR Commission                        
                        var DIsmembtype = (from mm in db.TBL_MASTER_MEMBER
                                           join
                                               rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                           where mm.MEM_ID == DIS_MEM_ID
                                           select new
                                           {
                                               RoleId = mm.MEMBER_ROLE,
                                               roleName = rm.ROLE_NAME,
                                               Amount = mm.BALANCE
                                           }).FirstOrDefault();
                        //var tbl_accountDMRDis = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == DIS_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        var tbl_accountDMRDis = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == DIS_MEM_ID).OrderByDescending(z => z.ACC_NO).FirstOrDefault();
                        if (tbl_accountDMRDis != null)
                        {
                            decimal DMRDISClosingAmt = tbl_accountDMRDis.CLOSING;
                            //decimal DMRDISSubAmt = DMRDISClosingAmt - Trans_Amt;

                            decimal getPer = 0;

                            decimal DMRDisCommAddClosing = DMRDISClosingAmt - DmrDisgapcomm;
                            long dis_idval = long.Parse(DIS_MEM_ID.ToString());
                            TBL_ACCOUNTS objCommPerDis = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = dis_idval,
                                MEMBER_TYPE = DIsmembtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "DR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = DMRDISClosingAmt,
                                CLOSING = DMRDisCommAddClosing,
                                REC_NO = 0,
                                COMM_AMT = DmrDisgapcomm,
                                GST = 0,
                                TDS = 0,
                                IPAddress = IpAddress
                            };
                            db.TBL_ACCOUNTS.Add(objCommPerDis);
                            db.SaveChanges();
                        }
                        #endregion



                        var SuperComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                                         join commslabMob in db.TBL_COMM_SLAB_DMR_PAYMENT
                                          on detailscom.DMR_SLAB equals commslabMob.SLAB_ID
                                         where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == SUP_MEM_ID && commslabMob.DMT_TYPE == "DOMESTIC"
                                         select new
                                         {
                                             SLN = commslabMob.SLN,
                                             Slab_ID = commslabMob.SLAB_ID,
                                             Slab_From = commslabMob.SLAB_FROM,
                                             Slab_To = commslabMob.SLAB_TO,
                                             commPer = commslabMob.COMM_PERCENTAGE,

                                         }).ToList();
                        decimal SuperDMRComm = 0;
                        foreach (var comslab in SuperComm)
                        {
                            if (Trans_Amt >= comslab.Slab_From && Trans_Amt <= comslab.Slab_To)
                            {
                                SuperDMRComm = comslab.commPer;
                            }
                        }
                        decimal DmrSUPgapcomm = 0;
                        DmrSUPgapcomm = DistributorDMRComm - SuperDMRComm;
                        #region Super DMR Commission                        
                        var Supmembtype = (from mm in db.TBL_MASTER_MEMBER
                                           join
                                               rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                           where mm.MEM_ID == SUP_MEM_ID
                                           select new
                                           {
                                               RoleId = mm.MEMBER_ROLE,
                                               roleName = rm.ROLE_NAME,
                                               Amount = mm.BALANCE
                                           }).FirstOrDefault();
                        //var tbl_accountSupDis = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == SUP_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        var tbl_accountSupDis = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == SUP_MEM_ID).OrderByDescending(z => z.ACC_NO).FirstOrDefault();
                        if (tbl_accountSupDis != null)
                        {
                            decimal DMRSupClosingAmt = tbl_accountSupDis.CLOSING;
                            //decimal DMRDISSubAmt = DMRDISClosingAmt - Trans_Amt;

                            decimal getPer = 0;

                            decimal DMRSUPCommAddClosing = DMRSupClosingAmt - DmrSUPgapcomm;
                            long sup_idval = long.Parse(SUP_MEM_ID.ToString());
                            TBL_ACCOUNTS objCommPerSup = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = sup_idval,
                                MEMBER_TYPE = Supmembtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "DR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = DMRSupClosingAmt,
                                CLOSING = DMRSUPCommAddClosing,
                                REC_NO = 0,
                                COMM_AMT = DmrSUPgapcomm,
                                GST = 0,
                                TDS = 0,
                                IPAddress = IpAddress
                            };
                            db.TBL_ACCOUNTS.Add(objCommPerSup);
                            db.SaveChanges();
                        }
                        #endregion



                        var WhiteComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                                         join commslabMob in db.TBL_COMM_SLAB_DMR_PAYMENT
                                          on detailscom.DMR_SLAB equals commslabMob.SLAB_ID
                                         where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == 0 && commslabMob.DMT_TYPE == "DOMESTIC"
                                         select new
                                         {
                                             SLN = commslabMob.SLN,
                                             Slab_ID = commslabMob.SLAB_ID,
                                             Slab_From = commslabMob.SLAB_FROM,
                                             Slab_To = commslabMob.SLAB_TO,
                                             commPer = commslabMob.COMM_PERCENTAGE,

                                         }).ToList();

                        decimal WhiteDMRComm = 0;
                        foreach (var comslab in WhiteComm)
                        {
                            if (Trans_Amt >= comslab.Slab_From && Trans_Amt <= comslab.Slab_To)
                            {
                                WhiteDMRComm = comslab.commPer;
                            }
                        }
                        decimal DmrWhitegapcomm = 0;
                        DmrWhitegapcomm = SuperDMRComm - WhiteDMRComm;
                        #region White DMR Commission                        
                        var Whitemembtype = (from mm in db.TBL_MASTER_MEMBER
                                             join
                                                 rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                             where mm.MEM_ID == WHT_MEM_ID
                                             select new
                                             {
                                                 RoleId = mm.MEMBER_ROLE,
                                                 roleName = rm.ROLE_NAME,
                                                 Amount = mm.BALANCE
                                             }).FirstOrDefault();
                        //var tbl_accountWhiteDis = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == WHT_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        var tbl_accountWhiteDis = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == WHT_MEM_ID).OrderByDescending(z => z.ACC_NO).FirstOrDefault();
                        if (tbl_accountWhiteDis != null)
                        {
                            decimal DMRWgiteClosingAmt = tbl_accountWhiteDis.CLOSING;
                            //decimal DMRDISSubAmt = DMRDISClosingAmt - Trans_Amt;

                            decimal getPer = 0;

                            decimal DMRWhiteCommAddClosing = DMRWgiteClosingAmt - DmrWhitegapcomm;
                            long Wht_idval = long.Parse(WHT_MEM_ID.ToString());
                            TBL_ACCOUNTS objCommPerWL = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Wht_idval,
                                MEMBER_TYPE = Whitemembtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "DR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = DMRWgiteClosingAmt,
                                CLOSING = DMRWhiteCommAddClosing,
                                REC_NO = 0,
                                COMM_AMT = DmrWhitegapcomm,
                                GST = 0,
                                TDS = 0,
                                IPAddress = IpAddress
                            };
                            db.TBL_ACCOUNTS.Add(objCommPerWL);
                            db.SaveChanges();
                        }
                        #endregion
                        ContextTransaction.Commit();
                        return "Success";
                    }
                    else if (status == "LANDLINE")
                    {
                        var MerchantComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                            join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                            on detailscom.SLN equals commslabMob.SLAB_ID
                                            join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                                            where commslabMob.OPERATOR_CODE == serviceprovider && detailscom.SLAB_TYPE == 1 && mem.MEMBER_ROLE == 5
                                            select new
                                            {
                                                SLN = commslabMob.SLN,
                                                commPer = commslabMob.MERCHANT_COM_PER,
                                                commType=commslabMob.COMMISSION_TYPE
                                            }).FirstOrDefault();

                        //var MerchantComm2 = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                    join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                        //                    on detailscom.RECHARGE_SLAB equals commslabMob.SLAB_ID
                        //                    where detailscom.WHITE_LEVEL_ID == DIS_MEM_ID && detailscom.INTRODUCER_ID == Mem_ID && commslabMob.OPERATOR_CODE == serviceprovider
                        //                    select new
                        //                    {
                        //                        SLN = commslabMob.SLN,
                        //                        commPer = commslabMob.COMM_PERCENTAGE
                        //                    }).FirstOrDefault();
                        //var MerchantComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                    join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                        //                    on detailscom.RECHARGE_SLAB equals commslabMob.SLAB_ID
                        //                    where detailscom.WHITE_LEVEL_ID == DIS_MEM_ID && detailscom.INTRODUCER_ID == Mem_ID && commslabMob.OPERATOR_CODE == serviceprovider
                        //                    select new
                        //                    {
                        //                        SLN = commslabMob.SLN,
                        //                        commPer = commslabMob.COMM_PERCENTAGE
                        //                    }).FirstOrDefault();
                        #region Retailer Commission                        
                        var membtype = (from mm in db.TBL_MASTER_MEMBER
                                        join
                                            rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                        where mm.MEM_ID == Mem_ID
                                        select new
                                        {
                                            RoleId = mm.MEMBER_ROLE,
                                            roleName = rm.ROLE_NAME,
                                            Amount = mm.BALANCE
                                        }).FirstOrDefault();
                        var tbl_account = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == Mem_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        if (tbl_account != null)
                        {
                            decimal ClosingAmt = tbl_account.CLOSING;
                            decimal SubAmt = ClosingAmt - Trans_Amt;
                            TBL_ACCOUNTS objaccnt = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Mem_ID,
                                MEMBER_TYPE = membtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "DR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Recharge",
                                OPENING = ClosingAmt,
                                CLOSING = SubAmt,
                                REC_NO = 0,
                                COMM_AMT = 0,
                                GST = 0,
                                TDS = 0,
                                IPAddress = IpAddress
                            };
                            db.TBL_ACCOUNTS.Add(objaccnt);
                            db.SaveChanges();

                            decimal getPer_Val = 0;
                            // Get TDS and GST
                            decimal RetailerTDS_AmtValue = 0;                           
                            decimal getPer = 0;
                            if (MerchantComm.commType == "FIXED")
                            {
                                 getPer_Val = MerchantComm.commPer;
                                // Get TDS and GST
                                 RetailerTDS_AmtValue = decimal.Round(((getPer_Val * GST_AMount) / 100), 2);
                                 getPer = getPer_Val - RetailerTDS_AmtValue;
                            }
                            else
                            {
                                 getPer_Val = (Trans_Amt * MerchantComm.commPer) / 100;
                                // Get TDS and GST
                                 RetailerTDS_AmtValue = decimal.Round(((getPer_Val * GST_AMount) / 100), 2);
                                 getPer = getPer_Val - RetailerTDS_AmtValue;
                            }

                            decimal CommAddClosing = SubAmt + getPer;
                            TBL_ACCOUNTS objCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Mem_ID,
                                MEMBER_TYPE = membtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = SubAmt,
                                CLOSING = CommAddClosing,
                                REC_NO = 0,
                                COMM_AMT = getPer,
                                GST = 0,
                                TDS = (float)RetailerTDS_AmtValue,
                                IPAddress = IpAddress
                            };
                            db.TBL_ACCOUNTS.Add(objCommPer);
                            db.SaveChanges();
                        }
                        #endregion

                        var DistributorComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                               join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                               on detailscom.SLN equals commslabMob.SLAB_ID
                                               join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                                               where commslabMob.OPERATOR_CODE == serviceprovider && detailscom.SLAB_TYPE == 1 && mem.MEMBER_ROLE == 4
                                               select new
                                               {
                                                   SLN = commslabMob.SLN,
                                                   commPer = commslabMob.DISTRIBUTOR_COM_PER,
                                                   commType=commslabMob.COMMISSION_TYPE
                                               }).FirstOrDefault();
                        //var DistributorComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                       join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                        //                       on detailscom.RECHARGE_SLAB equals commslabMob.SLAB_ID
                        //                       where detailscom.WHITE_LEVEL_ID == SUP_MEM_ID && detailscom.INTRODUCER_ID == DIS_MEM_ID && commslabMob.OPERATOR_CODE == serviceprovider
                        //                       select new
                        //                       {
                        //                           SLN = commslabMob.SLN,
                        //                           commPer = commslabMob.COMM_PERCENTAGE
                        //                       }).FirstOrDefault();
                        #region Distributor Commission
                        //decimal DisGapComm = decimal.Parse(DistributorComm.commPer.ToString()) - decimal.Parse(MerchantComm.commPer.ToString());

                        var tbl_accountDistributor = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == DIS_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        if (tbl_accountDistributor != null)
                        {
                            var CheckDisCommission = db.TBL_DETAILS_MEMBER_COMMISSION_SLAB.Where(x => x.INTRODUCER_ID == DIS_MEM_ID).FirstOrDefault();
                            var Dismembtype = (from mm in db.TBL_MASTER_MEMBER
                                               join
                                                   rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                               where mm.MEM_ID == DIS_MEM_ID
                                               select new
                                               {
                                                   RoleId = mm.MEMBER_ROLE,
                                                   roleName = rm.ROLE_NAME,
                                                   Amount = mm.BALANCE
                                               }).FirstOrDefault();
                            decimal DisClosingAmt = tbl_accountDistributor.CLOSING;
                            //decimal SubAmt = ClosingAmt - Trans_Amt;                   
                            //decimal DisgetPer = (Trans_Amt * checkDiscomm.CommPercentage) / 100;

                            //  Distributor GST and TDS
                            //decimal CommDisAddClosing = tbl_accountDistributor.CLOSING + DisGapCommAmt;
                            decimal DisGapComm = decimal.Parse(DistributorComm.commPer.ToString());
                            //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;
                            decimal DisGapCommAmt_Val = 0;
                            decimal DisGapCommAmt_TDS = 0;
                            decimal DisGapCommAmt = 0;
                            if(DistributorComm.commType=="FIXED")
                            {
                                DisGapCommAmt_Val = DisGapComm;
                                DisGapCommAmt_TDS = decimal.Round(((DisGapCommAmt_Val * GST_AMount) / 100), 2);
                                DisGapCommAmt = DisGapCommAmt_Val - DisGapCommAmt_TDS;
                            }
                            else
                            {
                                 DisGapCommAmt_Val = (Trans_Amt * DisGapComm) / 100;
                                DisGapCommAmt_TDS = decimal.Round(((DisGapCommAmt_Val * GST_AMount) / 100), 2);
                                 DisGapCommAmt = DisGapCommAmt_Val - DisGapCommAmt_TDS;
                            }

                            decimal CommDisAddClosing = tbl_accountDistributor.CLOSING + DisGapCommAmt;
                            long Dis_ID = long.Parse(DIS_MEM_ID.ToString());
                            TBL_ACCOUNTS objDisCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Dis_ID,
                                MEMBER_TYPE = Dismembtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = DisClosingAmt,
                                CLOSING = CommDisAddClosing,
                                REC_NO = 0,
                                COMM_AMT = DisGapCommAmt,
                                GST = 0,
                                TDS = (float)DisGapCommAmt_TDS,
                                IPAddress = IpAddress
                            };
                            db.TBL_ACCOUNTS.Add(objDisCommPer);
                            db.SaveChanges();
                        }
                        #endregion
                        var SuperComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                         join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                          on detailscom.SLN equals commslabMob.SLAB_ID
                                         join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                                         where commslabMob.OPERATOR_CODE == serviceprovider && detailscom.SLAB_TYPE == 1 && mem.MEMBER_ROLE == 3
                                         select new
                                         {
                                             SLN = commslabMob.SLN,
                                             commPer = commslabMob.SUPER_COM_PER,
                                             commType=commslabMob.COMMISSION_TYPE
                                         }).FirstOrDefault();
                        //var SuperComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                 join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                        //                  on detailscom.RECHARGE_SLAB equals commslabMob.SLAB_ID
                        //                 where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == SUP_MEM_ID && commslabMob.OPERATOR_CODE == serviceprovider
                        //                 select new
                        //                 {
                        //                     SLN = commslabMob.SLN,
                        //                     commPer = commslabMob.COMM_PERCENTAGE
                        //                 }).FirstOrDefault();
                        //decimal SupGapComm = decimal.Parse(SuperComm.commPer.ToString()) - decimal.Parse(DistributorComm.commPer.ToString());

                        #region Super Commission
                        var tbl_accountSuper = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == SUP_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        if (tbl_accountSuper != null)
                        {
                            var Supmembtype = (from mm in db.TBL_MASTER_MEMBER
                                               join
                                                   rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                               where mm.MEM_ID == SUP_MEM_ID
                                               select new
                                               {
                                                   RoleId = mm.MEMBER_ROLE,
                                                   roleName = rm.ROLE_NAME,
                                                   Amount = mm.BALANCE
                                               }).FirstOrDefault();
                            decimal SupClosingAmt = tbl_accountSuper.CLOSING;

                            decimal SupGapComm = decimal.Parse(SuperComm.commPer.ToString());
                            //decimal SupGapCommAmt = (Trans_Amt * SupGapComm) / 100;
                            // GST and TDS Calculation for Super
                            decimal SupGapCommAmt_val = 0;
                            decimal Sp_TDSAmt = 0;
                            decimal SupGapCommAmt = 0;
                            if(SuperComm.commType=="FIXED")
                            {
                                 SupGapCommAmt_val = SupGapComm;
                                 Sp_TDSAmt = decimal.Round(((SupGapCommAmt_val * GST_AMount) / 100), 2);
                                 SupGapCommAmt = SupGapCommAmt_val - Sp_TDSAmt;
                            }
                            else
                            {
                                 SupGapCommAmt_val = (Trans_Amt * SupGapComm) / 100;
                                 Sp_TDSAmt = decimal.Round(((SupGapCommAmt_val * GST_AMount) / 100), 2);
                                 SupGapCommAmt = SupGapCommAmt_val - Sp_TDSAmt;
                            }

                            decimal CommSupAddClosing = tbl_accountSuper.CLOSING + SupGapCommAmt;
                            long Sup_ID = long.Parse(SUP_MEM_ID.ToString());
                            TBL_ACCOUNTS objSupCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Sup_ID,
                                MEMBER_TYPE = Supmembtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = SupClosingAmt,
                                CLOSING = CommSupAddClosing,
                                REC_NO = 0,
                                COMM_AMT = SupGapCommAmt,
                                GST = 0,
                                TDS = (float)Sp_TDSAmt,
                                IPAddress = IpAddress
                            };
                            db.TBL_ACCOUNTS.Add(objSupCommPer);
                            db.SaveChanges();
                        }
                        #endregion                        
                        var WhiteComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                                         join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                          on detailscom.RECHARGE_SLAB equals commslabMob.SLAB_ID
                                         where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == 0 && commslabMob.OPERATOR_CODE == serviceprovider
                                         select new
                                         {
                                             SLN = commslabMob.SLN,
                                             commPer = commslabMob.COMM_PERCENTAGE,
                                             commType=commslabMob.COMMISSION_TYPE
                                         }).FirstOrDefault();
                        decimal WLComm = decimal.Parse(MerchantComm.commPer.ToString()) + decimal.Parse(DistributorComm.commPer.ToString()) + decimal.Parse(SuperComm.commPer.ToString());
                        decimal WTLGapComm = decimal.Parse(WhiteComm.commPer.ToString()) - WLComm;
                        //decimal WTLGapComm = decimal.Parse(WhiteComm.commPer.ToString());
                        //decimal WTLGapComm =WLComm;

                        #region White level Commission payment Structure
                        var tbl_accountWhiteLevel = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == WHT_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        if (tbl_accountWhiteLevel != null)
                        {
                            var WLmembtype = (from mm in db.TBL_MASTER_MEMBER
                                              join
                                                  rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                              where mm.MEM_ID == WHT_MEM_ID
                                              select new
                                              {
                                                  RoleId = mm.MEMBER_ROLE,
                                                  roleName = rm.ROLE_NAME,
                                                  Amount = mm.BALANCE
                                              }).FirstOrDefault();
                            decimal WLClosingAmt = tbl_accountWhiteLevel.CLOSING;
                            //decimal SubAmt = ClosingAmt - Trans_Amt;                  
                            // GST And TDS WhiteLevel Calculation

                            decimal WTLGapCommAmt_AMt = 0;
                            decimal WL_TDSCalculation = 0;
                            decimal WTLGapCommAmt = 0;
                            if (WhiteComm.commType == "FIXED")
                            {
                                 WTLGapCommAmt_AMt = WTLGapComm;
                                 WL_TDSCalculation = decimal.Round(((WTLGapCommAmt_AMt * GST_AMount) / 100), 2);
                                 WTLGapCommAmt = WTLGapCommAmt_AMt - WL_TDSCalculation;
                            }
                            else
                            {
                                 WTLGapCommAmt_AMt = (Trans_Amt * WTLGapComm) / 100;
                                 WL_TDSCalculation = decimal.Round(((WTLGapCommAmt_AMt * GST_AMount) / 100), 2);
                                 WTLGapCommAmt = WTLGapCommAmt_AMt - WL_TDSCalculation;
                            }

                            //decimal WTLGapCommAmt = (Trans_Amt * WTLGapComm) / 100;

                            decimal CommWLAddClosing = tbl_accountWhiteLevel.CLOSING + WTLGapCommAmt;
                            long WL_ID = long.Parse(WHT_MEM_ID.ToString());
                            TBL_ACCOUNTS objWLCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = WL_ID,
                                MEMBER_TYPE = WLmembtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = WLClosingAmt,
                                CLOSING = CommWLAddClosing,
                                REC_NO = 0,
                                COMM_AMT = WTLGapCommAmt,
                                GST = 0,
                                TDS = (float)WL_TDSCalculation,
                                IPAddress = IpAddress

                            };
                            db.TBL_ACCOUNTS.Add(objWLCommPer);
                            db.SaveChanges();
                        }
                        #endregion
                        ContextTransaction.Commit();
                        return "Success";
                    }
                    else if (status == "GAS")
                    {
                        //var MerchantComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                    join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
                        //                    on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
                        //                    where detailscom.WHITE_LEVEL_ID == DIS_MEM_ID && detailscom.INTRODUCER_ID == Mem_ID && commslabMob.OPERATOR_CODE == serviceprovider
                        //                    select new
                        //                    {
                        //                        SLN = commslabMob.SLN,
                        //                        commPer = commslabMob.COMM_PERCENTAGE
                        //                    }).FirstOrDefault();
                        var MerchantComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                            join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                            on detailscom.SLN equals commslabMob.SLAB_ID
                                            join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                                            where commslabMob.OPERATOR_CODE == serviceprovider && mem.MEMBER_ROLE == 5
                                            select new
                                            {
                                                SLN = commslabMob.SLN,
                                                commPer = commslabMob.MERCHANT_COM_PER,
                                                commType=commslabMob.COMMISSION_TYPE
                                            }).FirstOrDefault();
                        #region Retailer Commission                        
                        var membtype = (from mm in db.TBL_MASTER_MEMBER
                                        join
                                            rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                        where mm.MEM_ID == Mem_ID
                                        select new
                                        {
                                            RoleId = mm.MEMBER_ROLE,
                                            roleName = rm.ROLE_NAME,
                                            Amount = mm.BALANCE
                                        }).FirstOrDefault();
                        var tbl_account = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == Mem_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        if (tbl_account != null)
                        {
                            decimal ClosingAmt = tbl_account.CLOSING;
                            decimal SubAmt = ClosingAmt - Trans_Amt;
                            TBL_ACCOUNTS objaccnt = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Mem_ID,
                                MEMBER_TYPE = membtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "DR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Recharge",
                                OPENING = ClosingAmt,
                                CLOSING = SubAmt,
                                REC_NO = 0,
                                COMM_AMT = 0,
                                GST = 0,
                                TDS = 0,
                                IPAddress = IpAddress
                            };
                            db.TBL_ACCOUNTS.Add(objaccnt);
                            db.SaveChanges();
                            // TDS and GST CAlculation for Merchant for electricity
                            //decimal getPer = (Trans_Amt * MerchantComm.commPer) / 100;
                            decimal ELE_getPer_Val = 0;
                            decimal ELE_TDS_Value = 0;
                            decimal getPer = 0;
                            if(MerchantComm.commType=="FIXED")
                            {
                                 ELE_getPer_Val = MerchantComm.commPer;
                                 ELE_TDS_Value = decimal.Round(((ELE_getPer_Val * GST_AMount) / 100), 2);
                                 getPer = (ELE_getPer_Val - ELE_TDS_Value);
                            }
                            else
                            {
                                 ELE_getPer_Val = (Trans_Amt * MerchantComm.commPer) / 100;
                                 ELE_TDS_Value = decimal.Round(((ELE_getPer_Val * GST_AMount) / 100), 2);
                                 getPer = (ELE_getPer_Val - ELE_TDS_Value);
                            }
                            decimal CommAddClosing = SubAmt + getPer;
                            TBL_ACCOUNTS objCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Mem_ID,
                                MEMBER_TYPE = membtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = SubAmt,
                                CLOSING = CommAddClosing,
                                REC_NO = 0,
                                COMM_AMT = getPer,
                                GST = 0,
                                TDS = (float)ELE_TDS_Value,
                                IPAddress = IpAddress
                            };
                            db.TBL_ACCOUNTS.Add(objCommPer);
                            db.SaveChanges();
                        }
                        #endregion

                        //var DistributorComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                       join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
                        //                       on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
                        //                       where detailscom.WHITE_LEVEL_ID == SUP_MEM_ID && detailscom.INTRODUCER_ID == DIS_MEM_ID && commslabMob.OPERATOR_CODE == serviceprovider
                        //                       select new
                        //                       {
                        //                           SLN = commslabMob.SLN,
                        //                           commPer = commslabMob.COMM_PERCENTAGE
                        //                       }).FirstOrDefault();
                        var DistributorComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                               join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                               on detailscom.SLN equals commslabMob.SLAB_ID
                                               join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                                               where commslabMob.OPERATOR_CODE == serviceprovider && mem.MEMBER_ROLE == 4
                                               select new
                                               {
                                                   SLN = commslabMob.SLN,
                                                   commPer = commslabMob.DISTRIBUTOR_COM_PER,
                                                   commType=commslabMob.COMMISSION_TYPE
                                               }).FirstOrDefault();
                        #region Distributor Commission
                        //decimal DisGapComm = decimal.Parse(DistributorComm.commPer.ToString()) - decimal.Parse(MerchantComm.commPer.ToString());
                        decimal DisGapComm = decimal.Parse(DistributorComm.commPer.ToString());
                        //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;
                        var tbl_accountDistributor = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == DIS_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        if (tbl_accountDistributor != null)
                        {
                            var CheckDisCommission = db.TBL_DETAILS_MEMBER_COMMISSION_SLAB.Where(x => x.INTRODUCER_ID == DIS_MEM_ID).FirstOrDefault();
                            var Dismembtype = (from mm in db.TBL_MASTER_MEMBER
                                               join
                                                   rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                               where mm.MEM_ID == DIS_MEM_ID
                                               select new
                                               {
                                                   RoleId = mm.MEMBER_ROLE,
                                                   roleName = rm.ROLE_NAME,
                                                   Amount = mm.BALANCE
                                               }).FirstOrDefault();
                            decimal DisClosingAmt = tbl_accountDistributor.CLOSING;
                            //decimal SubAmt = ClosingAmt - Trans_Amt;                   
                            //decimal DisgetPer = (Trans_Amt * checkDiscomm.CommPercentage) / 100;

                            // For TDS and GST CAlculation for Electricity
                            //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;
                            decimal ELE_DisGap_CommAmt = 0;
                            decimal ELE_TDSCALCULATION =0 ;
                            decimal DisGapCommAmt = 0;
                            if(DistributorComm.commType=="FIXED")
                            {
                                 ELE_DisGap_CommAmt = DisGapComm;
                                 ELE_TDSCALCULATION = decimal.Round(((ELE_DisGap_CommAmt * GST_AMount) / 100), 2);
                                 DisGapCommAmt = ELE_DisGap_CommAmt - ELE_TDSCALCULATION;
                            }
                            else
                            {
                                 ELE_DisGap_CommAmt = (Trans_Amt * DisGapComm) / 100;
                                 ELE_TDSCALCULATION = decimal.Round(((ELE_DisGap_CommAmt * GST_AMount) / 100), 2);
                                 DisGapCommAmt = ELE_DisGap_CommAmt - ELE_TDSCALCULATION;
                            }
                            decimal CommDisAddClosing = tbl_accountDistributor.CLOSING + DisGapCommAmt;
                            long Dis_ID = long.Parse(DIS_MEM_ID.ToString());
                            TBL_ACCOUNTS objDisCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Dis_ID,
                                MEMBER_TYPE = Dismembtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = DisClosingAmt,
                                CLOSING = CommDisAddClosing,
                                REC_NO = 0,
                                COMM_AMT = DisGapCommAmt,
                                GST = 0,
                                TDS = (float)ELE_TDSCALCULATION,
                                IPAddress = IpAddress
                            };
                            db.TBL_ACCOUNTS.Add(objDisCommPer);
                            db.SaveChanges();
                        }
                        #endregion
                        //var SuperComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                 join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
                        //                  on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
                        //                 where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == SUP_MEM_ID && commslabMob.OPERATOR_CODE == serviceprovider
                        //                 select new
                        //                 {
                        //                     SLN = commslabMob.SLN,
                        //                     commPer = commslabMob.COMM_PERCENTAGE
                        //                 }).FirstOrDefault();
                        var SuperComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                         join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                          on detailscom.SLN equals commslabMob.SLAB_ID
                                         join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                                         where commslabMob.OPERATOR_CODE == serviceprovider && mem.MEMBER_ROLE == 3
                                         select new
                                         {
                                             SLN = commslabMob.SLN,
                                             commPer = commslabMob.SUPER_COM_PER,
                                             commType=commslabMob.COMMISSION_TYPE
                                         }).FirstOrDefault();
                        //decimal SupGapComm = decimal.Parse(SuperComm.commPer.ToString()) - decimal.Parse(DistributorComm.commPer.ToString());
                        decimal SupGapComm = decimal.Parse(SuperComm.commPer.ToString());
                        //decimal SupGapCommAmt = (Trans_Amt * SupGapComm) / 100;
                        #region Super Commission
                        var tbl_accountSuper = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == SUP_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        if (tbl_accountSuper != null)
                        {
                            var Supmembtype = (from mm in db.TBL_MASTER_MEMBER
                                               join
                                                   rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                               where mm.MEM_ID == SUP_MEM_ID
                                               select new
                                               {
                                                   RoleId = mm.MEMBER_ROLE,
                                                   roleName = rm.ROLE_NAME,
                                                   Amount = mm.BALANCE
                                               }).FirstOrDefault();
                            decimal SupClosingAmt = tbl_accountSuper.CLOSING;
                            // TDS AND GST CALCULATION FOR SUPER FOR ELECTRICITY
                            decimal ELE_Sup_Gap_Comm_Amt = 0;
                            decimal ELE_SUP_TDS_CAL = 0;
                            decimal SupGapCommAmt = 0;
                            if(SuperComm.commType=="FIXED")
                            {
                                 ELE_Sup_Gap_Comm_Amt = SupGapComm;
                                 ELE_SUP_TDS_CAL = decimal.Round(((ELE_Sup_Gap_Comm_Amt * GST_AMount) / 100), 2);
                                 SupGapCommAmt = ELE_Sup_Gap_Comm_Amt - ELE_SUP_TDS_CAL;
                            }
                            else
                            {
                                 ELE_Sup_Gap_Comm_Amt = (Trans_Amt * SupGapComm) / 100;
                                 ELE_SUP_TDS_CAL = decimal.Round(((ELE_Sup_Gap_Comm_Amt * GST_AMount) / 100), 2);
                                 SupGapCommAmt = ELE_Sup_Gap_Comm_Amt - ELE_SUP_TDS_CAL;
                            }
                            decimal CommSupAddClosing = tbl_accountSuper.CLOSING + SupGapCommAmt;
                            long Sup_ID = long.Parse(SUP_MEM_ID.ToString());
                            TBL_ACCOUNTS objSupCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Sup_ID,
                                MEMBER_TYPE = Supmembtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = SupClosingAmt,
                                CLOSING = CommSupAddClosing,
                                REC_NO = 0,
                                COMM_AMT = SupGapCommAmt,
                                GST = 0,
                                TDS = (float)ELE_SUP_TDS_CAL,
                                IPAddress = IpAddress
                            };
                            db.TBL_ACCOUNTS.Add(objSupCommPer);
                            db.SaveChanges();
                        }
                        #endregion                        
                        var WhiteComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                                         join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
                                          on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
                                         where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == 0 && commslabMob.OPERATOR_CODE == serviceprovider
                                         select new
                                         {
                                             SLN = commslabMob.SLN,
                                             commPer = commslabMob.COMM_PERCENTAGE,
                                             commType=commslabMob.COMMISSION_TYPE
                                         }).FirstOrDefault();
                        decimal WL_Com = decimal.Parse(MerchantComm.commPer.ToString()) + decimal.Parse(DistributorComm.commPer.ToString()) + decimal.Parse(SuperComm.commPer.ToString());
                        //decimal WTLGapComm = decimal.Parse(WhiteComm.commPer.ToString()) - decimal.Parse(SuperComm.commPer.ToString());
                        decimal WTLGapComm = decimal.Parse(WhiteComm.commPer.ToString()) - WL_Com;
                        //decimal WTLGapCommAmt = (Trans_Amt * WTLGapComm) / 100;
                        #region White level Commission payment Structure
                        var tbl_accountWhiteLevel = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == WHT_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        if (tbl_accountWhiteLevel != null)
                        {
                            var WLmembtype = (from mm in db.TBL_MASTER_MEMBER
                                              join
                                                  rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                              where mm.MEM_ID == WHT_MEM_ID
                                              select new
                                              {
                                                  RoleId = mm.MEMBER_ROLE,
                                                  roleName = rm.ROLE_NAME,
                                                  Amount = mm.BALANCE
                                              }).FirstOrDefault();
                            decimal WLClosingAmt = tbl_accountWhiteLevel.CLOSING;
                            //decimal SubAmt = ClosingAmt - Trans_Amt;           
                            // GST And TDS Calculation for WL 
                            //decimal WTLGapCommAmt = (Trans_Amt * WTLGapComm) / 100;
                            decimal ELE_WTL_Gap_CommAmt = 0;
                            decimal ELE_WL_TDSCal = 0;
                            decimal WTLGapCommAmt = 0;
                            if(WhiteComm.commType=="FIXED")
                            {
                                 ELE_WTL_Gap_CommAmt = WTLGapComm;
                                 ELE_WL_TDSCal = decimal.Round(((ELE_WTL_Gap_CommAmt * GST_AMount) / 100), 2);
                                 WTLGapCommAmt = ELE_WTL_Gap_CommAmt - ELE_WL_TDSCal;
                            }
                            else
                            {
                                 ELE_WTL_Gap_CommAmt = (Trans_Amt * WTLGapComm) / 100;
                                 ELE_WL_TDSCal = decimal.Round(((ELE_WTL_Gap_CommAmt * GST_AMount) / 100), 2);
                                 WTLGapCommAmt = ELE_WTL_Gap_CommAmt - ELE_WL_TDSCal;
                            }
                            decimal CommWLAddClosing = tbl_accountWhiteLevel.CLOSING + WTLGapCommAmt;
                            long WL_ID = long.Parse(WHT_MEM_ID.ToString());
                            TBL_ACCOUNTS objWLCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = WL_ID,
                                MEMBER_TYPE = WLmembtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = WLClosingAmt,
                                CLOSING = CommWLAddClosing,
                                REC_NO = 0,
                                COMM_AMT = WTLGapCommAmt,
                                GST = 0,
                                TDS = (float)ELE_WL_TDSCal,
                                IPAddress = IpAddress
                            };
                            db.TBL_ACCOUNTS.Add(objWLCommPer);
                            db.SaveChanges();
                        }
                        #endregion
                        ContextTransaction.Commit();
                        return "Success";
                    }

                }
                catch (Exception ex)
                {
                    return "Fail";
                    ContextTransaction.Rollback();
                    throw ex;
                }
            }
            return "";
        }
        #endregion
        #region Another Commission method  MAIN With GST AND TDS
        //public bool AllMemberCommissionDistribution(long Mem_ID, string status, decimal Trans_Amt, decimal ChargeAmt, decimal OpeningAmt, string serviceprovider, string rechargeType, string IpAddress)
        //{

        //    var db = new DBContext();
        //    //var taxMaster = db.TBL_TAX_MASTER.FirstOrDefault();
        //    //GST cal
        //    var operatorID = db.TBL_SERVICE_PROVIDERS.Where(x => x.SERVICE_NAME == serviceprovider && x.TYPE == "MobileOperator").FirstOrDefault();
        //    var MemberGST = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == Mem_ID).Select(c=>c.COMPANY_GST_NO).FirstOrDefault();
        //    var GST_Master = db.TBL_TAX_MASTERS.Where(x => x.TAX_NAME == "GST").FirstOrDefault();
        //    var Gst_Mode = GST_Master.TAX_MODE;
        //    decimal gst_VAlue = 0;
        //    decimal.TryParse(GST_Master.TAX_VALUE.ToString(), out gst_VAlue);
        //    //TAX cal
        //    var taxMaster = db.TBL_TAX_MASTERS.Where(x=>x.TAX_NAME=="TDS").FirstOrDefault();
        //    decimal GST_AMount = 0;
        //    string TaxMode = taxMaster.TAX_MODE;
        //    decimal.TryParse(taxMaster.TAX_VALUE.ToString(), out GST_AMount);
        //    decimal TDS_AMount = 0;
        //    //decimal GST_AMount = decimal.Parse(System.Configuration.ConfigurationManager.AppSettings["GST_Amount"]);
        //    //decimal TDS_AMount = decimal.Parse(System.Configuration.ConfigurationManager.AppSettings["TDS_Amount"]);
        //    using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            var DIS_MEM_ID = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == Mem_ID).Select(z => z.INTRODUCER).FirstOrDefault();
        //            var SUP_MEM_ID = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == DIS_MEM_ID).Select(z => z.INTRODUCER).FirstOrDefault();
        //            var WHT_MEM_ID = db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == SUP_MEM_ID).Select(z => z.UNDER_WHITE_LEVEL).FirstOrDefault();
        //            if (status == "MOBILE")
        //            {
        //                var MerchantComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
        //                                    join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
        //                                    on detailscom.SLN equals commslabMob.SLAB_ID
        //                                    join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
        //                                    where commslabMob.OPERATOR_CODE == serviceprovider && detailscom.SLAB_TYPE == 1 && mem.MEMBER_ROLE == 5
        //                                    select new
        //                                    {
        //                                        SLN = commslabMob.SLN,
        //                                        commPer = commslabMob.MERCHANT_COM_PER,
        //                                        commType = commslabMob.COMMISSION_TYPE
        //                                    }).FirstOrDefault();

        //                //var MerchantComm2 = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
        //                //                    join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
        //                //                    on detailscom.RECHARGE_SLAB equals commslabMob.SLAB_ID
        //                //                    where detailscom.WHITE_LEVEL_ID == DIS_MEM_ID && detailscom.INTRODUCER_ID == Mem_ID && commslabMob.OPERATOR_CODE == serviceprovider
        //                //                    select new
        //                //                    {
        //                //                        SLN = commslabMob.SLN,
        //                //                        commPer = commslabMob.COMM_PERCENTAGE
        //                //                    }).FirstOrDefault();
        //                //var MerchantComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
        //                //                    join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
        //                //                    on detailscom.RECHARGE_SLAB equals commslabMob.SLAB_ID
        //                //                    where detailscom.WHITE_LEVEL_ID == DIS_MEM_ID && detailscom.INTRODUCER_ID == Mem_ID && commslabMob.OPERATOR_CODE == serviceprovider
        //                //                    select new
        //                //                    {
        //                //                        SLN = commslabMob.SLN,
        //                //                        commPer = commslabMob.COMM_PERCENTAGE
        //                //                    }).FirstOrDefault();
        //                #region Retailer Commission                        
        //                var membtype = (from mm in db.TBL_MASTER_MEMBER
        //                                join
        //                                    rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
        //                                where mm.MEM_ID == Mem_ID
        //                                select new
        //                                {
        //                                    RoleId = mm.MEMBER_ROLE,
        //                                    roleName = rm.ROLE_NAME,
        //                                    Amount = mm.BALANCE
        //                                }).FirstOrDefault();
        //                var tbl_account = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == Mem_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
        //                if (tbl_account != null)
        //                {
        //                    decimal ClosingAmt = tbl_account.CLOSING;
        //                    decimal SubAmt = ClosingAmt - Trans_Amt;
        //                    TBL_ACCOUNTS objaccnt = new TBL_ACCOUNTS()
        //                    {
        //                        API_ID = 0,
        //                        MEM_ID = Mem_ID,
        //                        MEMBER_TYPE = membtype.roleName,
        //                        TRANSACTION_TYPE = rechargeType,
        //                        TRANSACTION_DATE = System.DateTime.Now,
        //                        TRANSACTION_TIME = DateTime.Now,
        //                        DR_CR = "DR",
        //                        AMOUNT = Trans_Amt,
        //                        NARRATION = "Recharge",
        //                        OPENING = ClosingAmt,
        //                        CLOSING = SubAmt,
        //                        REC_NO = 0,
        //                        COMM_AMT = 0,
        //                        GST = 0,
        //                        TDS = 0,
        //                        IPAddress = IpAddress,
        //                        TDS_PERCENTAGE = 0,
        //                        GST_PERCENTAGE = 0,
        //                        WHITELEVEL_ID = (long)WHT_MEM_ID,
        //                        SUPER_ID = (long)SUP_MEM_ID,
        //                        DISTRIBUTOR_ID = (long)DIS_MEM_ID,
        //                        SERVICE_ID= operatorID.ID
        //                    };
        //                    db.TBL_ACCOUNTS.Add(objaccnt);
        //                    db.SaveChanges();


        //                    decimal getPer_Val = 0;
        //                    //float FloatgetPer_Val = float.Parse(getPer_Val.ToString());
        //                    // Get TDS and GST
        //                    decimal RetailerTDS_AmtValue = 0;
        //                    decimal getPer = 0;
        //                    decimal GStCal = 0;
        //                    decimal CalculateGST = 0;
        //                    if (MerchantComm.commType == "FIXED")
        //                    {
        //                        getPer_Val = MerchantComm.commPer;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode== "PERCENTAGE")
        //                        {

        //                            RetailerTDS_AmtValue = decimal.Round(((getPer_Val * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                CalculateGST = decimal.Round(((getPer_Val * gst_VAlue) / 100), 2);
        //                                //getPer = getPer_Val + CalculateGST - RetailerTDS_AmtValue;
        //                                getPer = getPer_Val  - RetailerTDS_AmtValue;
        //                            }
        //                            else
        //                            {
        //                                CalculateGST =0;
        //                                //getPer = getPer_Val + CalculateGST - RetailerTDS_AmtValue;
        //                                getPer = getPer_Val - RetailerTDS_AmtValue;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            RetailerTDS_AmtValue = decimal.Round(((getPer_Val - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                CalculateGST = decimal.Round(((getPer_Val - gst_VAlue)));
        //                                //getPer = getPer_Val + CalculateGST - RetailerTDS_AmtValue;
        //                                getPer = getPer_Val - RetailerTDS_AmtValue;
        //                            }
        //                            else
        //                            {
        //                                CalculateGST = 0;
        //                                //getPer = getPer_Val + CalculateGST - RetailerTDS_AmtValue;
        //                                getPer = getPer_Val  - RetailerTDS_AmtValue;
        //                            }

        //                        }
        //                    }
        //                    else
        //                    {
        //                        getPer_Val = (Trans_Amt * MerchantComm.commPer) / 100;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {

        //                            RetailerTDS_AmtValue = decimal.Round(((getPer_Val * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                CalculateGST = decimal.Round(((getPer_Val * gst_VAlue) / 100), 2);
        //                                //getPer = getPer_Val + CalculateGST - RetailerTDS_AmtValue;
        //                                getPer = getPer_Val  - RetailerTDS_AmtValue;
        //                            }
        //                            else
        //                            {
        //                                CalculateGST =0;
        //                                //getPer = getPer_Val + CalculateGST - RetailerTDS_AmtValue;
        //                                getPer = getPer_Val  - RetailerTDS_AmtValue;
        //                            }

        //                        }
        //                        else
        //                        {                                    
        //                            RetailerTDS_AmtValue = decimal.Round(((getPer_Val - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                CalculateGST = decimal.Round(((getPer_Val - gst_VAlue)));
        //                                //getPer = getPer_Val + CalculateGST - RetailerTDS_AmtValue;
        //                                getPer = getPer_Val - RetailerTDS_AmtValue;
        //                            }
        //                            else
        //                            {
        //                                CalculateGST =0;
        //                                //getPer = getPer_Val + CalculateGST - RetailerTDS_AmtValue;
        //                                getPer = getPer_Val  - RetailerTDS_AmtValue;
        //                            }

        //                        }
        //                        //RetailerTDS_AmtValue = decimal.Round(((getPer_Val * GST_AMount) / 100), 2);
        //                        //getPer = getPer_Val - RetailerTDS_AmtValue;
        //                    }
        //                    //float RetailerTDS_AmtValue = (FloatgetPer_Val * GST_AMount) / 100;
        //                    //double getPer = float.Parse(getPer_Val) - RetailerTDS_AmtValue;

        //                    decimal CommAddClosing = SubAmt + getPer;
        //                    TBL_ACCOUNTS objCommPer = new TBL_ACCOUNTS()
        //                    {
        //                        API_ID = 0,
        //                        MEM_ID = Mem_ID,
        //                        MEMBER_TYPE = membtype.roleName,
        //                        TRANSACTION_TYPE = rechargeType,
        //                        TRANSACTION_DATE = System.DateTime.Now,
        //                        TRANSACTION_TIME = DateTime.Now,
        //                        DR_CR = "CR",
        //                        AMOUNT = Trans_Amt,
        //                        NARRATION = "Commission",
        //                        OPENING = SubAmt,
        //                        CLOSING = CommAddClosing,
        //                        REC_NO = 0,
        //                        COMM_AMT = getPer,
        //                        GST = (float)CalculateGST,
        //                        TDS = (float)RetailerTDS_AmtValue,
        //                        IPAddress = IpAddress,
        //                        TDS_PERCENTAGE= taxMaster.TAX_VALUE,
        //                        GST_PERCENTAGE=GST_Master.TAX_VALUE,
        //                        WHITELEVEL_ID= (long)WHT_MEM_ID,
        //                        SUPER_ID=(long)SUP_MEM_ID,
        //                        DISTRIBUTOR_ID=(long)DIS_MEM_ID,
        //                        SERVICE_ID= operatorID.ID
        //                    };
        //                    db.TBL_ACCOUNTS.Add(objCommPer);
        //                    db.SaveChanges();
        //                }
        //                #endregion

        //                var DistributorComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
        //                                       join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
        //                                       on detailscom.SLN equals commslabMob.SLAB_ID
        //                                       join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
        //                                       where commslabMob.OPERATOR_CODE == serviceprovider && detailscom.SLAB_TYPE == 1 && mem.MEMBER_ROLE == 4
        //                                       select new
        //                                       {
        //                                           SLN = commslabMob.SLN,
        //                                           commPer = commslabMob.DISTRIBUTOR_COM_PER,
        //                                           commType = commslabMob.COMMISSION_TYPE
        //                                       }).FirstOrDefault();
        //                //var DistributorComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
        //                //                       join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
        //                //                       on detailscom.RECHARGE_SLAB equals commslabMob.SLAB_ID
        //                //                       where detailscom.WHITE_LEVEL_ID == SUP_MEM_ID && detailscom.INTRODUCER_ID == DIS_MEM_ID && commslabMob.OPERATOR_CODE == serviceprovider
        //                //                       select new
        //                //                       {
        //                //                           SLN = commslabMob.SLN,
        //                //                           commPer = commslabMob.COMM_PERCENTAGE
        //                //                       }).FirstOrDefault();
        //                #region Distributor Commission
        //                //decimal DisGapComm = decimal.Parse(DistributorComm.commPer.ToString()) - decimal.Parse(MerchantComm.commPer.ToString());

        //                var tbl_accountDistributor = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == DIS_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
        //                if (tbl_accountDistributor != null)
        //                {
        //                    var CheckDisCommission = db.TBL_DETAILS_MEMBER_COMMISSION_SLAB.Where(x => x.INTRODUCER_ID == DIS_MEM_ID).FirstOrDefault();
        //                    var Dismembtype = (from mm in db.TBL_MASTER_MEMBER
        //                                       join
        //                                           rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
        //                                       where mm.MEM_ID == DIS_MEM_ID
        //                                       select new
        //                                       {
        //                                           RoleId = mm.MEMBER_ROLE,
        //                                           roleName = rm.ROLE_NAME,
        //                                           Amount = mm.BALANCE
        //                                       }).FirstOrDefault();
        //                    decimal DisClosingAmt = tbl_accountDistributor.CLOSING;
        //                    //decimal SubAmt = ClosingAmt - Trans_Amt;                   
        //                    //decimal DisgetPer = (Trans_Amt * checkDiscomm.CommPercentage) / 100;

        //                    //  Distributor GST and TDS
        //                    //decimal CommDisAddClosing = tbl_accountDistributor.CLOSING + DisGapCommAmt;

        //                    decimal DisGapComm = 0;
        //                    //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;
        //                    decimal DisGapCommAmt_Val = 0;
        //                    decimal DisGapCommAmt_TDS = 0;
        //                    decimal DisGapCommAmt = 0;

        //                    decimal DIS_GST_Amt = 0;
        //                    decimal Dis_Calculate_GST = 0;


        //                    if (DistributorComm.commType == "FIXED")
        //                    {
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {
        //                            DisGapComm = decimal.Parse(DistributorComm.commPer.ToString());
        //                            DisGapCommAmt_Val = DisGapComm;                                    
        //                            DisGapCommAmt_TDS = decimal.Round(((DisGapCommAmt_Val * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                Dis_Calculate_GST = decimal.Round(((DisGapCommAmt_Val * gst_VAlue) / 100), 2);
        //                                //DisGapCommAmt = DisGapCommAmt_Val + Dis_Calculate_GST - DisGapCommAmt_TDS;
        //                                DisGapCommAmt = DisGapCommAmt_Val  - DisGapCommAmt_TDS;
        //                            }
        //                            else {
        //                                Dis_Calculate_GST = 0;
        //                                //DisGapCommAmt = DisGapCommAmt_Val + Dis_Calculate_GST - DisGapCommAmt_TDS;
        //                                DisGapCommAmt = DisGapCommAmt_Val  - DisGapCommAmt_TDS;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            DisGapComm = decimal.Parse(DistributorComm.commPer.ToString());
        //                            DisGapCommAmt_Val = DisGapComm;                                    
        //                            DisGapCommAmt_TDS = decimal.Round(((DisGapCommAmt_Val - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                Dis_Calculate_GST = decimal.Round(((DisGapCommAmt_Val - gst_VAlue)));
        //                                //DisGapCommAmt = DisGapCommAmt_Val + Dis_Calculate_GST - DisGapCommAmt_TDS;
        //                                DisGapCommAmt = DisGapCommAmt_Val - DisGapCommAmt_TDS;
        //                            }
        //                            else
        //                            {
        //                                Dis_Calculate_GST = 0;
        //                                //DisGapCommAmt = DisGapCommAmt_Val + Dis_Calculate_GST - DisGapCommAmt_TDS;
        //                                DisGapCommAmt = DisGapCommAmt_Val - DisGapCommAmt_TDS;
        //                            }

        //                        }
        //                    }
        //                    else
        //                    {
        //                        DisGapComm = decimal.Parse(DistributorComm.commPer.ToString());
        //                        //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;
        //                        DisGapCommAmt_Val = (Trans_Amt * DisGapComm) / 100;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                   
        //                            DisGapCommAmt_TDS = decimal.Round(((DisGapCommAmt_Val * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                Dis_Calculate_GST = decimal.Round(((DisGapCommAmt_Val * gst_VAlue) / 100), 2);
        //                                //DisGapCommAmt = DisGapCommAmt_Val + Dis_Calculate_GST - DisGapCommAmt_TDS;
        //                                DisGapCommAmt = DisGapCommAmt_Val  - DisGapCommAmt_TDS;
        //                            }
        //                            else
        //                            {
        //                                Dis_Calculate_GST = 0;
        //                                //DisGapCommAmt = DisGapCommAmt_Val + Dis_Calculate_GST - DisGapCommAmt_TDS;
        //                                DisGapCommAmt = DisGapCommAmt_Val  - DisGapCommAmt_TDS;
        //                            }
        //                        }
        //                        else
        //                        {                                    
        //                            DisGapCommAmt_TDS = decimal.Round(((DisGapCommAmt_Val - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                Dis_Calculate_GST = decimal.Round(((DisGapCommAmt_Val - gst_VAlue)));
        //                                //DisGapCommAmt = DisGapCommAmt_Val + Dis_Calculate_GST - DisGapCommAmt_TDS;
        //                                DisGapCommAmt = DisGapCommAmt_Val - DisGapCommAmt_TDS;
        //                            }
        //                            else
        //                            {
        //                                Dis_Calculate_GST = 0;
        //                                //DisGapCommAmt = DisGapCommAmt_Val + Dis_Calculate_GST - DisGapCommAmt_TDS;
        //                                DisGapCommAmt = DisGapCommAmt_Val - DisGapCommAmt_TDS;
        //                            }
        //                        }

        //                    }
        //                    decimal CommDisAddClosing = tbl_accountDistributor.CLOSING + DisGapCommAmt;
        //                    long Dis_ID = long.Parse(DIS_MEM_ID.ToString());
        //                    TBL_ACCOUNTS objDisCommPer = new TBL_ACCOUNTS()
        //                    {
        //                        API_ID = 0,
        //                        MEM_ID = Dis_ID,
        //                        MEMBER_TYPE = Dismembtype.roleName,
        //                        TRANSACTION_TYPE = rechargeType,
        //                        TRANSACTION_DATE = System.DateTime.Now,
        //                        TRANSACTION_TIME = DateTime.Now,
        //                        DR_CR = "CR",
        //                        AMOUNT = Trans_Amt,
        //                        NARRATION = "Commission",
        //                        OPENING = DisClosingAmt,
        //                        CLOSING = CommDisAddClosing,
        //                        REC_NO = 0,
        //                        COMM_AMT = DisGapCommAmt,
        //                        GST = (float)Dis_Calculate_GST,
        //                        TDS = (float)DisGapCommAmt_TDS,
        //                        IPAddress = IpAddress,
        //                        TDS_PERCENTAGE = taxMaster.TAX_VALUE,
        //                        GST_PERCENTAGE = GST_Master.TAX_VALUE,
        //                        WHITELEVEL_ID = (long)WHT_MEM_ID,
        //                        SUPER_ID = (long)SUP_MEM_ID,
        //                        DISTRIBUTOR_ID = (long)DIS_MEM_ID,
        //                        SERVICE_ID = operatorID.ID
        //                    };
        //                    db.TBL_ACCOUNTS.Add(objDisCommPer);
        //                    db.SaveChanges();
        //                }
        //                #endregion
        //                var SuperComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
        //                                 join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
        //                                  on detailscom.SLN equals commslabMob.SLAB_ID
        //                                 join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
        //                                 where commslabMob.OPERATOR_CODE == serviceprovider && detailscom.SLAB_TYPE == 1 && mem.MEMBER_ROLE == 3
        //                                 select new
        //                                 {
        //                                     SLN = commslabMob.SLN,
        //                                     commPer = commslabMob.SUPER_COM_PER,
        //                                     commType = commslabMob.COMMISSION_TYPE
        //                                 }).FirstOrDefault();
        //                //var SuperComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
        //                //                 join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
        //                //                  on detailscom.RECHARGE_SLAB equals commslabMob.SLAB_ID
        //                //                 where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == SUP_MEM_ID && commslabMob.OPERATOR_CODE == serviceprovider
        //                //                 select new
        //                //                 {
        //                //                     SLN = commslabMob.SLN,
        //                //                     commPer = commslabMob.COMM_PERCENTAGE
        //                //                 }).FirstOrDefault();
        //                //decimal SupGapComm = decimal.Parse(SuperComm.commPer.ToString()) - decimal.Parse(DistributorComm.commPer.ToString());

        //                #region Super Commission
        //                var tbl_accountSuper = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == SUP_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
        //                if (tbl_accountSuper != null)
        //                {
        //                    var Supmembtype = (from mm in db.TBL_MASTER_MEMBER
        //                                       join
        //                                           rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
        //                                       where mm.MEM_ID == SUP_MEM_ID
        //                                       select new
        //                                       {
        //                                           RoleId = mm.MEMBER_ROLE,
        //                                           roleName = rm.ROLE_NAME,
        //                                           Amount = mm.BALANCE
        //                                       }).FirstOrDefault();
        //                    decimal SupClosingAmt = tbl_accountSuper.CLOSING;

        //                    decimal SupGapComm = 0;
        //                    //decimal SupGapCommAmt = (Trans_Amt * SupGapComm) / 100;
        //                    // GST and TDS Calculation for Super
        //                    decimal SupGapCommAmt_val = 0;
        //                    decimal Sp_TDSAmt = 0;
        //                    decimal SupGapCommAmt = 0;
        //                    decimal SUP_GstAmt = 0;
        //                    decimal Sup_CalculateGST = 0;
        //                    if (SuperComm.commType == "FIXED")
        //                    {
        //                        SupGapComm = decimal.Parse(SuperComm.commPer.ToString());
        //                        //decimal SupGapCommAmt = (Trans_Amt * SupGapComm) / 100;
        //                        // GST and TDS Calculation for Super
        //                        SupGapCommAmt_val = SupGapComm;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                    
        //                            Sp_TDSAmt = decimal.Round(((SupGapCommAmt_val * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                Sup_CalculateGST = decimal.Round(((SupGapCommAmt_val * gst_VAlue) / 100), 2);
        //                                //SupGapCommAmt = SupGapCommAmt_val + Sup_CalculateGST - Sp_TDSAmt;
        //                                SupGapCommAmt = SupGapCommAmt_val - Sp_TDSAmt;
        //                            }
        //                            else
        //                            {
        //                                Sup_CalculateGST = 0;
        //                                //SupGapCommAmt = SupGapCommAmt_val + Sup_CalculateGST - Sp_TDSAmt;
        //                                SupGapCommAmt = SupGapCommAmt_val  - Sp_TDSAmt;
        //                            }
        //                        }
        //                        else
        //                        {                                    
        //                            Sp_TDSAmt = decimal.Round(((SupGapCommAmt_val - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                Sup_CalculateGST = decimal.Round(((SupGapCommAmt_val - gst_VAlue)));
        //                                //SupGapCommAmt = SupGapCommAmt_val + Sup_CalculateGST - Sp_TDSAmt;
        //                                SupGapCommAmt = SupGapCommAmt_val - Sp_TDSAmt;
        //                            }
        //                            else
        //                            {
        //                                Sup_CalculateGST = 0;
        //                                //SupGapCommAmt = SupGapCommAmt_val + Sup_CalculateGST - Sp_TDSAmt;
        //                                SupGapCommAmt = SupGapCommAmt_val - Sp_TDSAmt;
        //                            }

        //                        }   
        //                    }
        //                    else
        //                    {
        //                        SupGapComm = decimal.Parse(SuperComm.commPer.ToString());
        //                        //decimal SupGapCommAmt = (Trans_Amt * SupGapComm) / 100;
        //                        // GST and TDS Calculation for Super
        //                        SupGapCommAmt_val = (Trans_Amt * SupGapComm) / 100;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                    
        //                            Sp_TDSAmt = decimal.Round(((SupGapCommAmt_val * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                Sup_CalculateGST = decimal.Round(((SupGapCommAmt_val * gst_VAlue) / 100), 2);
        //                                //SupGapCommAmt = SupGapCommAmt_val + Sup_CalculateGST - Sp_TDSAmt;
        //                                SupGapCommAmt = SupGapCommAmt_val  - Sp_TDSAmt;
        //                            }
        //                            else
        //                            {
        //                                Sup_CalculateGST = 0;
        //                                //SupGapCommAmt = SupGapCommAmt_val + Sup_CalculateGST - Sp_TDSAmt;
        //                                SupGapCommAmt = SupGapCommAmt_val - Sp_TDSAmt;
        //                            }                                    
        //                        }
        //                        else
        //                        {                                    
        //                            Sp_TDSAmt = decimal.Round(((SupGapCommAmt_val - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                Sup_CalculateGST = decimal.Round(((SupGapCommAmt_val - gst_VAlue)));
        //                                //SupGapCommAmt = SupGapCommAmt_val + Sup_CalculateGST - Sp_TDSAmt;
        //                                SupGapCommAmt = SupGapCommAmt_val  - Sp_TDSAmt;
        //                            }
        //                            else
        //                            {
        //                                Sup_CalculateGST = 0;
        //                                //SupGapCommAmt = SupGapCommAmt_val + Sup_CalculateGST - Sp_TDSAmt;
        //                                SupGapCommAmt = SupGapCommAmt_val - Sp_TDSAmt;
        //                            }
        //                        }
        //                    }

        //                    decimal CommSupAddClosing = tbl_accountSuper.CLOSING + SupGapCommAmt;
        //                    long Sup_ID = long.Parse(SUP_MEM_ID.ToString());
        //                    TBL_ACCOUNTS objSupCommPer = new TBL_ACCOUNTS()
        //                    {
        //                        API_ID = 0,
        //                        MEM_ID = Sup_ID,
        //                        MEMBER_TYPE = Supmembtype.roleName,
        //                        TRANSACTION_TYPE = rechargeType,
        //                        TRANSACTION_DATE = System.DateTime.Now,
        //                        TRANSACTION_TIME = DateTime.Now,
        //                        DR_CR = "CR",
        //                        AMOUNT = Trans_Amt,
        //                        NARRATION = "Commission",
        //                        OPENING = SupClosingAmt,
        //                        CLOSING = CommSupAddClosing,
        //                        REC_NO = 0,
        //                        COMM_AMT = SupGapCommAmt,
        //                        GST = (float)Sup_CalculateGST,
        //                        TDS = (float)Sp_TDSAmt,
        //                        IPAddress = IpAddress,
        //                        TDS_PERCENTAGE = taxMaster.TAX_VALUE,
        //                        GST_PERCENTAGE = GST_Master.TAX_VALUE,
        //                        WHITELEVEL_ID = (long)WHT_MEM_ID,
        //                        SUPER_ID = (long)SUP_MEM_ID,
        //                        DISTRIBUTOR_ID = (long)DIS_MEM_ID,
        //                        SERVICE_ID = operatorID.ID
        //                    };
        //                    db.TBL_ACCOUNTS.Add(objSupCommPer);
        //                    db.SaveChanges();
        //                }
        //                #endregion                        
        //                var WhiteComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
        //                                 join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
        //                                  on detailscom.RECHARGE_SLAB equals commslabMob.SLAB_ID
        //                                 where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == 0 && commslabMob.OPERATOR_CODE == serviceprovider
        //                                 select new
        //                                 {
        //                                     SLN = commslabMob.SLN,
        //                                     commPer = commslabMob.COMM_PERCENTAGE,
        //                                     commType = commslabMob.COMMISSION_TYPE
        //                                 }).FirstOrDefault();
        //                decimal WLComm = decimal.Parse(MerchantComm.commPer.ToString()) + decimal.Parse(DistributorComm.commPer.ToString()) + decimal.Parse(SuperComm.commPer.ToString());

        //                //decimal WTLGapComm = decimal.Parse(WhiteComm.commPer.ToString());
        //                //decimal WTLGapComm =WLComm;

        //                #region White level Commission payment Structure
        //                var tbl_accountWhiteLevel = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == WHT_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
        //                if (tbl_accountWhiteLevel != null)
        //                {
        //                    var WLmembtype = (from mm in db.TBL_MASTER_MEMBER
        //                                      join
        //                                          rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
        //                                      where mm.MEM_ID == WHT_MEM_ID
        //                                      select new
        //                                      {
        //                                          RoleId = mm.MEMBER_ROLE,
        //                                          roleName = rm.ROLE_NAME,
        //                                          Amount = mm.BALANCE
        //                                      }).FirstOrDefault();
        //                    decimal WLClosingAmt = tbl_accountWhiteLevel.CLOSING;
        //                    //decimal SubAmt = ClosingAmt - Trans_Amt;                  
        //                    // GST And TDS WhiteLevel Calculation
        //                    decimal WTLGapComm = decimal.Parse(WhiteComm.commPer.ToString()) - WLComm;
        //                    decimal WTLGapCommAmt_AMt = 0;
        //                    decimal WL_TDSCalculation = 0;
        //                    decimal WTLGapCommAmt = 0;

        //                    decimal WL_GST_AMT = 0;
        //                    decimal WL_CalculateGST_AMT = 0;

        //                    if (WhiteComm.commType == "FIXED")
        //                    {
        //                        WTLGapCommAmt_AMt = WTLGapComm;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                    
        //                            WL_TDSCalculation = decimal.Round(((WTLGapCommAmt_AMt * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                WL_CalculateGST_AMT = decimal.Round(((WTLGapCommAmt_AMt * gst_VAlue) / 100), 2);
        //                                //WTLGapCommAmt = WTLGapCommAmt_AMt + WL_CalculateGST_AMT - WL_TDSCalculation;
        //                                WTLGapCommAmt = WTLGapCommAmt_AMt - WL_TDSCalculation;
        //                            }
        //                            else
        //                            {
        //                                WL_CalculateGST_AMT = 0;
        //                                //WTLGapCommAmt = WTLGapCommAmt_AMt + WL_CalculateGST_AMT - WL_TDSCalculation;
        //                                WTLGapCommAmt = WTLGapCommAmt_AMt - WL_TDSCalculation;
        //                            }                                    
        //                        }
        //                        else
        //                        {                                    
        //                            WL_TDSCalculation = decimal.Round(((WTLGapCommAmt_AMt - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                WL_CalculateGST_AMT = decimal.Round(((WTLGapCommAmt_AMt - gst_VAlue)));
        //                                //WTLGapCommAmt = WTLGapCommAmt_AMt + WL_CalculateGST_AMT - WL_TDSCalculation;
        //                                WTLGapCommAmt = WTLGapCommAmt_AMt - WL_TDSCalculation;
        //                            }
        //                            else
        //                            {
        //                                WL_CalculateGST_AMT = 0;
        //                                //WTLGapCommAmt = WTLGapCommAmt_AMt + WL_CalculateGST_AMT - WL_TDSCalculation;
        //                                WTLGapCommAmt = WTLGapCommAmt_AMt- WL_TDSCalculation;
        //                            }                                    
        //                        }
        //                    }
        //                    else
        //                    {
        //                        WTLGapCommAmt_AMt = (Trans_Amt * WTLGapComm) / 100;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                    
        //                            WL_TDSCalculation = decimal.Round(((WTLGapCommAmt_AMt * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                WL_CalculateGST_AMT = decimal.Round(((WTLGapCommAmt_AMt * gst_VAlue) / 100), 2);
        //                                //WTLGapCommAmt = WTLGapCommAmt_AMt + WL_CalculateGST_AMT - WL_TDSCalculation;
        //                                WTLGapCommAmt = WTLGapCommAmt_AMt  - WL_TDSCalculation;
        //                            }
        //                            else
        //                            {
        //                                WL_CalculateGST_AMT = 0;
        //                                //WTLGapCommAmt = WTLGapCommAmt_AMt + WL_CalculateGST_AMT - WL_TDSCalculation;
        //                                WTLGapCommAmt = WTLGapCommAmt_AMt  - WL_TDSCalculation;
        //                            }

        //                        }
        //                        else
        //                        {                                 
        //                            WL_TDSCalculation = decimal.Round(((WTLGapCommAmt_AMt - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                WL_CalculateGST_AMT = decimal.Round(((WTLGapCommAmt_AMt - gst_VAlue)));
        //                                //WTLGapCommAmt = WTLGapCommAmt_AMt + WL_CalculateGST_AMT - WL_TDSCalculation;
        //                                WTLGapCommAmt = WTLGapCommAmt_AMt - WL_TDSCalculation;
        //                            }
        //                            else
        //                            {
        //                                WL_CalculateGST_AMT =0;
        //                                //WTLGapCommAmt = WTLGapCommAmt_AMt + WL_CalculateGST_AMT - WL_TDSCalculation;
        //                                WTLGapCommAmt = WTLGapCommAmt_AMt  - WL_TDSCalculation;
        //                            }

        //                        }

        //                    }
        //                    //decimal WTLGapCommAmt = (Trans_Amt * WTLGapComm) / 100;
        //                    decimal CommWLAddClosing = tbl_accountWhiteLevel.CLOSING + WTLGapCommAmt;
        //                    long WL_ID = long.Parse(WHT_MEM_ID.ToString());
        //                    TBL_ACCOUNTS objWLCommPer = new TBL_ACCOUNTS()
        //                    {
        //                        API_ID = 0,
        //                        MEM_ID = WL_ID,
        //                        MEMBER_TYPE = WLmembtype.roleName,
        //                        TRANSACTION_TYPE = rechargeType,
        //                        TRANSACTION_DATE = System.DateTime.Now,
        //                        TRANSACTION_TIME = DateTime.Now,
        //                        DR_CR = "CR",
        //                        AMOUNT = Trans_Amt,
        //                        NARRATION = "Commission",
        //                        OPENING = WLClosingAmt,
        //                        CLOSING = CommWLAddClosing,
        //                        REC_NO = 0,
        //                        COMM_AMT = WTLGapCommAmt,
        //                        GST = (float)WL_CalculateGST_AMT,
        //                        TDS = (float)WL_TDSCalculation,
        //                        IPAddress = IpAddress,
        //                        TDS_PERCENTAGE = taxMaster.TAX_VALUE,
        //                        GST_PERCENTAGE = GST_Master.TAX_VALUE,
        //                        WHITELEVEL_ID = (long)WHT_MEM_ID,
        //                        SUPER_ID = (long)SUP_MEM_ID,
        //                        DISTRIBUTOR_ID = (long)DIS_MEM_ID,
        //                        SERVICE_ID = operatorID.ID
        //                    };
        //                    db.TBL_ACCOUNTS.Add(objWLCommPer);
        //                    db.SaveChanges();
        //                }
        //                #endregion
        //                ContextTransaction.Commit();
        //                return true;
        //            }
        //            else if (status == "UTILITY")
        //            {
        //                var MerchantComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
        //                                    join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
        //                                    on detailscom.SLN equals commslabMob.SLAB_ID
        //                                    join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
        //                                    where commslabMob.OPERATOR_CODE == serviceprovider && detailscom.SLAB_TYPE == 2 && mem.MEMBER_ROLE == 5
        //                                    select new
        //                                    {
        //                                        SLN = commslabMob.SLN,
        //                                        commPer = commslabMob.MERCHANT_COM_PER,
        //                                        commType = commslabMob.COMMISSION_TYPE
        //                                    }).FirstOrDefault();
        //                //var MerchantComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
        //                //                    join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
        //                //                    on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
        //                //                    where detailscom.WHITE_LEVEL_ID == DIS_MEM_ID && detailscom.INTRODUCER_ID == Mem_ID && commslabMob.OPERATOR_CODE == serviceprovider
        //                //                    select new
        //                //                    {
        //                //                        SLN = commslabMob.SLN,
        //                //                        commPer = commslabMob.COMM_PERCENTAGE
        //                //                    }).FirstOrDefault();
        //                #region Retailer Commission                        
        //                var membtype = (from mm in db.TBL_MASTER_MEMBER
        //                                join
        //                                    rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
        //                                where mm.MEM_ID == Mem_ID
        //                                select new
        //                                {
        //                                    RoleId = mm.MEMBER_ROLE,
        //                                    roleName = rm.ROLE_NAME,
        //                                    Amount = mm.BALANCE
        //                                }).FirstOrDefault();
        //                var tbl_account = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == Mem_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
        //                if (tbl_account != null)
        //                {
        //                    decimal ClosingAmt = tbl_account.CLOSING;
        //                    decimal SubAmt = ClosingAmt - Trans_Amt;
        //                    TBL_ACCOUNTS objaccnt = new TBL_ACCOUNTS()
        //                    {
        //                        API_ID = 0,
        //                        MEM_ID = Mem_ID,
        //                        MEMBER_TYPE = membtype.roleName,
        //                        TRANSACTION_TYPE = rechargeType,
        //                        TRANSACTION_DATE = System.DateTime.Now,
        //                        TRANSACTION_TIME = DateTime.Now,
        //                        DR_CR = "DR",
        //                        AMOUNT = Trans_Amt,
        //                        NARRATION = "Recharge",
        //                        OPENING = ClosingAmt,
        //                        CLOSING = SubAmt,
        //                        REC_NO = 0,
        //                        COMM_AMT = 0,
        //                        GST = 0,
        //                        TDS = 0,
        //                        IPAddress = IpAddress,
        //                        TDS_PERCENTAGE = 0,
        //                        GST_PERCENTAGE = 0,
        //                        WHITELEVEL_ID = (long)WHT_MEM_ID,
        //                        SUPER_ID = (long)SUP_MEM_ID,
        //                        DISTRIBUTOR_ID = (long)DIS_MEM_ID,
        //                        SERVICE_ID = operatorID.ID
        //                    };
        //                    db.TBL_ACCOUNTS.Add(objaccnt);
        //                    db.SaveChanges();

        //                    //GST And TDS Calculation Section for Retailer
        //                    //decimal getPer = (Trans_Amt * MerchantComm.commPer) / 100;

        //                    decimal getPer_Value = 0;
        //                    decimal MerchantTDSValue = 0;
        //                    decimal getPer = 0;
        //                    decimal mer_GST_AMT = 0;
        //                    decimal MerCalculate_GST_AMT = 0;
        //                    if (MerchantComm.commType == "FIXED")
        //                    {
        //                        getPer_Value = MerchantComm.commPer;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {

        //                            MerchantTDSValue = decimal.Round(((getPer_Value * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                MerCalculate_GST_AMT = decimal.Round(((getPer_Value * gst_VAlue) / 100), 2);
        //                                //getPer = getPer_Value + MerCalculate_GST_AMT - MerchantTDSValue;
        //                                getPer = getPer_Value - MerchantTDSValue;
        //                            }
        //                            else
        //                            {
        //                                MerCalculate_GST_AMT = 0;
        //                                //getPer = getPer_Value + MerCalculate_GST_AMT - MerchantTDSValue;
        //                                getPer = getPer_Value - MerchantTDSValue;
        //                            }                                    
        //                        }
        //                        else
        //                        {                                    
        //                            MerchantTDSValue = decimal.Round(((getPer_Value - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                MerCalculate_GST_AMT = decimal.Round(((getPer_Value - gst_VAlue)));
        //                                //getPer = getPer_Value + MerCalculate_GST_AMT - MerchantTDSValue;
        //                                getPer = getPer_Value  - MerchantTDSValue;
        //                            }
        //                            else
        //                            {
        //                                MerCalculate_GST_AMT =0;
        //                                //getPer = getPer_Value + MerCalculate_GST_AMT - MerchantTDSValue;
        //                                getPer = getPer_Value  - MerchantTDSValue;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        getPer_Value = (Trans_Amt * MerchantComm.commPer) / 100;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                    
        //                            MerchantTDSValue = decimal.Round(((getPer_Value * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                MerCalculate_GST_AMT = decimal.Round(((getPer_Value * gst_VAlue) / 100), 2);
        //                                //getPer = getPer_Value + MerCalculate_GST_AMT - MerchantTDSValue;
        //                                getPer = getPer_Value - MerchantTDSValue;
        //                            }
        //                            else
        //                            {
        //                                MerCalculate_GST_AMT = 0;
        //                                //getPer = getPer_Value + MerCalculate_GST_AMT - MerchantTDSValue;
        //                                getPer = getPer_Value  - MerchantTDSValue;
        //                            }
        //                        }
        //                        else
        //                        {                                    
        //                            MerchantTDSValue = decimal.Round(((getPer_Value- GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                MerCalculate_GST_AMT = decimal.Round(((getPer_Value - gst_VAlue)));
        //                                //getPer = getPer_Value + MerCalculate_GST_AMT - MerchantTDSValue;
        //                                getPer = getPer_Value  - MerchantTDSValue;
        //                            }
        //                            else
        //                            {
        //                                MerCalculate_GST_AMT =0;
        //                                //getPer = getPer_Value + MerCalculate_GST_AMT - MerchantTDSValue;
        //                                getPer = getPer_Value - MerchantTDSValue;
        //                            }                                    
        //                        }                                
        //                    }
        //                    decimal CommAddClosing = SubAmt + getPer;
        //                    TBL_ACCOUNTS objCommPer = new TBL_ACCOUNTS()
        //                    {
        //                        API_ID = 0,
        //                        MEM_ID = Mem_ID,
        //                        MEMBER_TYPE = membtype.roleName,
        //                        TRANSACTION_TYPE = rechargeType,
        //                        TRANSACTION_DATE = System.DateTime.Now,
        //                        TRANSACTION_TIME = DateTime.Now,
        //                        DR_CR = "CR",
        //                        AMOUNT = Trans_Amt,
        //                        NARRATION = "Commission",
        //                        OPENING = SubAmt,
        //                        CLOSING = CommAddClosing,
        //                        REC_NO = 0,
        //                        COMM_AMT = getPer,
        //                        GST = (float)MerCalculate_GST_AMT,
        //                        TDS = (float)MerchantTDSValue,
        //                        IPAddress = IpAddress,
        //                        TDS_PERCENTAGE = taxMaster.TAX_VALUE,
        //                        GST_PERCENTAGE = GST_Master.TAX_VALUE,
        //                        WHITELEVEL_ID = (long)WHT_MEM_ID,
        //                        SUPER_ID = (long)SUP_MEM_ID,
        //                        DISTRIBUTOR_ID = (long)DIS_MEM_ID,
        //                        SERVICE_ID = operatorID.ID
        //                    };
        //                    db.TBL_ACCOUNTS.Add(objCommPer);
        //                    db.SaveChanges();
        //                }
        //                #endregion

        //                var DistributorComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
        //                                       join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
        //                                       on detailscom.SLN equals commslabMob.SLAB_ID
        //                                       join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
        //                                       where commslabMob.OPERATOR_CODE == serviceprovider && detailscom.SLAB_TYPE == 2 && mem.MEMBER_ROLE == 4
        //                                       select new
        //                                       {
        //                                           SLN = commslabMob.SLN,
        //                                           commPer = commslabMob.DISTRIBUTOR_COM_PER,
        //                                           commType = commslabMob.COMMISSION_TYPE
        //                                       }).FirstOrDefault();
        //                //var DistributorComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
        //                //                       join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
        //                //                       on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
        //                //                       where detailscom.WHITE_LEVEL_ID == SUP_MEM_ID && detailscom.INTRODUCER_ID == DIS_MEM_ID && commslabMob.OPERATOR_CODE == serviceprovider
        //                //                       select new
        //                //                       {
        //                //                           SLN = commslabMob.SLN,
        //                //                           commPer = commslabMob.COMM_PERCENTAGE
        //                //                       }).FirstOrDefault();
        //                #region Distributor Commission
        //                //decimal DisGapComm = decimal.Parse(DistributorComm.commPer.ToString()) - decimal.Parse(MerchantComm.commPer.ToString());

        //                var tbl_accountDistributor = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == DIS_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
        //                if (tbl_accountDistributor != null)
        //                {
        //                    var CheckDisCommission = db.TBL_DETAILS_MEMBER_COMMISSION_SLAB.Where(x => x.INTRODUCER_ID == DIS_MEM_ID).FirstOrDefault();
        //                    var Dismembtype = (from mm in db.TBL_MASTER_MEMBER
        //                                       join
        //                                           rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
        //                                       where mm.MEM_ID == DIS_MEM_ID
        //                                       select new
        //                                       {
        //                                           RoleId = mm.MEMBER_ROLE,
        //                                           roleName = rm.ROLE_NAME,
        //                                           Amount = mm.BALANCE
        //                                       }).FirstOrDefault();
        //                    decimal DisClosingAmt = tbl_accountDistributor.CLOSING;
        //                    //decimal SubAmt = ClosingAmt - Trans_Amt;                   
        //                    //decimal DisgetPer = (Trans_Amt * checkDiscomm.CommPercentage) / 100;

        //                    // GST and TDS Calculation for Distributor

        //                    decimal DisGapComm = 0;
        //                    //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;
        //                    decimal UDisGapCommAmt_Value = 0;
        //                    decimal UDis_TDSCal = 0;
        //                    decimal DisGapCommAmt = 0;
        //                    decimal Dist_GST_AMt = 0;
        //                    decimal Dist_CalculateGST_AMt = 0;
        //                    if (DistributorComm.commType == "FIXED")
        //                    {
        //                        DisGapComm = decimal.Parse(DistributorComm.commPer.ToString());
        //                        //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;
        //                        UDisGapCommAmt_Value = DisGapComm;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                    
        //                            UDis_TDSCal = decimal.Round(((UDisGapCommAmt_Value * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                Dist_CalculateGST_AMt = decimal.Round(((UDisGapCommAmt_Value * gst_VAlue) / 100), 2);
        //                                //DisGapCommAmt = UDisGapCommAmt_Value + Dist_CalculateGST_AMt - UDis_TDSCal;
        //                                DisGapCommAmt = UDisGapCommAmt_Value  - UDis_TDSCal;
        //                            }
        //                            else
        //                            {
        //                                Dist_CalculateGST_AMt = 0;
        //                                //DisGapCommAmt = UDisGapCommAmt_Value + Dist_CalculateGST_AMt - UDis_TDSCal;
        //                                DisGapCommAmt = UDisGapCommAmt_Value  - UDis_TDSCal;
        //                            }
        //                        }
        //                        else
        //                        {                                    
        //                            UDis_TDSCal = decimal.Round(((UDisGapCommAmt_Value - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                Dist_CalculateGST_AMt = decimal.Round(((UDisGapCommAmt_Value - gst_VAlue)));
        //                                //DisGapCommAmt = UDisGapCommAmt_Value + Dist_CalculateGST_AMt - UDis_TDSCal;
        //                                DisGapCommAmt = UDisGapCommAmt_Value  - UDis_TDSCal;
        //                            }
        //                            else
        //                            {
        //                                Dist_CalculateGST_AMt = 0;
        //                                //DisGapCommAmt = UDisGapCommAmt_Value + Dist_CalculateGST_AMt - UDis_TDSCal;
        //                                DisGapCommAmt = UDisGapCommAmt_Value - UDis_TDSCal;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        DisGapComm = decimal.Parse(DistributorComm.commPer.ToString());
        //                        //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;
        //                        UDisGapCommAmt_Value = (Trans_Amt * DisGapComm) / 100;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                    
        //                            UDis_TDSCal = decimal.Round(((UDisGapCommAmt_Value * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                Dist_CalculateGST_AMt = decimal.Round(((UDisGapCommAmt_Value * gst_VAlue) / 100), 2);
        //                                //DisGapCommAmt = UDisGapCommAmt_Value + Dist_CalculateGST_AMt - UDis_TDSCal;
        //                                DisGapCommAmt = UDisGapCommAmt_Value - UDis_TDSCal;
        //                            }
        //                            else
        //                            {
        //                                Dist_CalculateGST_AMt = 0;
        //                                //DisGapCommAmt = UDisGapCommAmt_Value + Dist_CalculateGST_AMt - UDis_TDSCal;
        //                                DisGapCommAmt = UDisGapCommAmt_Value  - UDis_TDSCal;
        //                            }

        //                        }
        //                        else
        //                        {                                    
        //                            UDis_TDSCal = decimal.Round(((UDisGapCommAmt_Value - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                Dist_CalculateGST_AMt = decimal.Round(((UDisGapCommAmt_Value - gst_VAlue)));
        //                                //DisGapCommAmt = UDisGapCommAmt_Value + Dist_CalculateGST_AMt - UDis_TDSCal;
        //                                DisGapCommAmt = UDisGapCommAmt_Value  - UDis_TDSCal;
        //                            }
        //                            else
        //                            {
        //                                Dist_CalculateGST_AMt = 0;
        //                                //DisGapCommAmt = UDisGapCommAmt_Value + Dist_CalculateGST_AMt - UDis_TDSCal;
        //                                DisGapCommAmt = UDisGapCommAmt_Value  - UDis_TDSCal;
        //                            }                                    
        //                        }                                
        //                    }
        //                    decimal CommDisAddClosing = tbl_accountDistributor.CLOSING + DisGapCommAmt;
        //                    long Dis_ID = long.Parse(DIS_MEM_ID.ToString());
        //                    TBL_ACCOUNTS objDisCommPer = new TBL_ACCOUNTS()
        //                    {
        //                        API_ID = 0,
        //                        MEM_ID = Dis_ID,
        //                        MEMBER_TYPE = Dismembtype.roleName,
        //                        TRANSACTION_TYPE = rechargeType,
        //                        TRANSACTION_DATE = System.DateTime.Now,
        //                        TRANSACTION_TIME = DateTime.Now,
        //                        DR_CR = "CR",
        //                        AMOUNT = Trans_Amt,
        //                        NARRATION = "Commission",
        //                        OPENING = DisClosingAmt,
        //                        CLOSING = CommDisAddClosing,
        //                        REC_NO = 0,
        //                        COMM_AMT = DisGapCommAmt,
        //                        GST =(float)Dist_CalculateGST_AMt,
        //                        TDS = (float)UDis_TDSCal,
        //                        IPAddress = IpAddress,
        //                        TDS_PERCENTAGE = taxMaster.TAX_VALUE,
        //                        GST_PERCENTAGE = GST_Master.TAX_VALUE,
        //                        WHITELEVEL_ID = (long)WHT_MEM_ID,
        //                        SUPER_ID = (long)SUP_MEM_ID,
        //                        DISTRIBUTOR_ID = (long)DIS_MEM_ID,
        //                        SERVICE_ID = operatorID.ID
        //                    };
        //                    db.TBL_ACCOUNTS.Add(objDisCommPer);
        //                    db.SaveChanges();
        //                }
        //                #endregion
        //                //var SuperComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
        //                //                 join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
        //                //                  on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
        //                //                 where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == SUP_MEM_ID && commslabMob.OPERATOR_CODE == serviceprovider
        //                //                 select new
        //                //                 {
        //                //                     SLN = commslabMob.SLN,
        //                //                     commPer = commslabMob.COMM_PERCENTAGE
        //                //                 }).FirstOrDefault();
        //                var SuperComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
        //                                 join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
        //                                  on detailscom.SLN equals commslabMob.SLAB_ID
        //                                 join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
        //                                 where commslabMob.OPERATOR_CODE == serviceprovider && detailscom.SLAB_TYPE == 2 && mem.MEMBER_ROLE == 3
        //                                 select new
        //                                 {
        //                                     SLN = commslabMob.SLN,
        //                                     commPer = commslabMob.SUPER_COM_PER,
        //                                     commType = commslabMob.COMMISSION_TYPE
        //                                 }).FirstOrDefault();
        //                //decimal SupGapComm = decimal.Parse(SuperComm.commPer.ToString()) - decimal.Parse(DistributorComm.commPer.ToString());

        //                #region Super Commission
        //                var tbl_accountSuper = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == SUP_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
        //                if (tbl_accountSuper != null)
        //                {
        //                    var Supmembtype = (from mm in db.TBL_MASTER_MEMBER
        //                                       join
        //                                           rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
        //                                       where mm.MEM_ID == SUP_MEM_ID
        //                                       select new
        //                                       {
        //                                           RoleId = mm.MEMBER_ROLE,
        //                                           roleName = rm.ROLE_NAME,
        //                                           Amount = mm.BALANCE
        //                                       }).FirstOrDefault();
        //                    decimal SupClosingAmt = tbl_accountSuper.CLOSING;
        //                    //GST and TDS Calculation for Super
        //                    decimal SupGapComm = 0;
        //                    //decimal SupGapCommAmt = (Trans_Amt * SupGapComm) / 100;
        //                    decimal USupGapCommAmt_Val = 0;
        //                    decimal USuper_TDSCal = 0;
        //                    decimal SupGapCommAmt = 0;
        //                    decimal Sup_CalCulate_GST = 0;
        //                    if (SuperComm.commType == "FIXED")
        //                    {
        //                        SupGapComm = decimal.Parse(SuperComm.commPer.ToString());
        //                        //decimal SupGapCommAmt = (Trans_Amt * SupGapComm) / 100;
        //                        USupGapCommAmt_Val = SupGapComm;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                    
        //                            USuper_TDSCal = decimal.Round(((USupGapCommAmt_Val * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                Sup_CalCulate_GST = decimal.Round(((USupGapCommAmt_Val * gst_VAlue) / 100), 2);
        //                                //SupGapCommAmt = USupGapCommAmt_Val + Sup_CalCulate_GST - USuper_TDSCal;
        //                                SupGapCommAmt = USupGapCommAmt_Val   - USuper_TDSCal;
        //                            }
        //                            else
        //                            {
        //                                Sup_CalCulate_GST =0;
        //                                //SupGapCommAmt = USupGapCommAmt_Val + Sup_CalCulate_GST - USuper_TDSCal;
        //                                SupGapCommAmt = USupGapCommAmt_Val  - USuper_TDSCal;
        //                            }                                    
        //                        }
        //                        else
        //                        {                                    
        //                            USuper_TDSCal = decimal.Round(((USupGapCommAmt_Val - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                Sup_CalCulate_GST = decimal.Round(((USupGapCommAmt_Val - gst_VAlue)));
        //                                //SupGapCommAmt = USupGapCommAmt_Val + Sup_CalCulate_GST - USuper_TDSCal;
        //                                SupGapCommAmt = USupGapCommAmt_Val - USuper_TDSCal;
        //                            }
        //                            else
        //                            {
        //                                Sup_CalCulate_GST = 0;
        //                                //SupGapCommAmt = USupGapCommAmt_Val + Sup_CalCulate_GST - USuper_TDSCal;
        //                                SupGapCommAmt = USupGapCommAmt_Val - USuper_TDSCal;
        //                            }                                    
        //                        }
        //                    }
        //                    else
        //                    {
        //                        SupGapComm = decimal.Parse(SuperComm.commPer.ToString());
        //                        //decimal SupGapCommAmt = (Trans_Amt * SupGapComm) / 100;
        //                        USupGapCommAmt_Val = (Trans_Amt * SupGapComm) / 100;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                   
        //                            USuper_TDSCal = decimal.Round(((USupGapCommAmt_Val * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                Sup_CalCulate_GST = decimal.Round(((USupGapCommAmt_Val * gst_VAlue) / 100), 2);
        //                                //SupGapCommAmt = USupGapCommAmt_Val + Sup_CalCulate_GST - USuper_TDSCal;
        //                                SupGapCommAmt = USupGapCommAmt_Val - USuper_TDSCal;
        //                            }
        //                            else
        //                            {
        //                                Sup_CalCulate_GST = 0;
        //                                //SupGapCommAmt = USupGapCommAmt_Val + Sup_CalCulate_GST - USuper_TDSCal;
        //                                SupGapCommAmt = USupGapCommAmt_Val - USuper_TDSCal;
        //                            }
        //                        }
        //                        else
        //                        {                                    
        //                            USuper_TDSCal = decimal.Round(((USupGapCommAmt_Val * GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                Sup_CalCulate_GST = decimal.Round(((USupGapCommAmt_Val - gst_VAlue)));
        //                                //SupGapCommAmt = USupGapCommAmt_Val + Sup_CalCulate_GST - USuper_TDSCal;
        //                                SupGapCommAmt = USupGapCommAmt_Val  - USuper_TDSCal;
        //                            }
        //                            else
        //                            {
        //                                Sup_CalCulate_GST = 0;
        //                                //SupGapCommAmt = USupGapCommAmt_Val + Sup_CalCulate_GST - USuper_TDSCal;
        //                                SupGapCommAmt = USupGapCommAmt_Val - USuper_TDSCal;
        //                            }
        //                        }
        //                    }
        //                    decimal CommSupAddClosing = tbl_accountSuper.CLOSING + SupGapCommAmt;
        //                    long Sup_ID = long.Parse(SUP_MEM_ID.ToString());
        //                    TBL_ACCOUNTS objSupCommPer = new TBL_ACCOUNTS()
        //                    {
        //                        API_ID = 0,
        //                        MEM_ID = Sup_ID,
        //                        MEMBER_TYPE = Supmembtype.roleName,
        //                        TRANSACTION_TYPE = rechargeType,
        //                        TRANSACTION_DATE = System.DateTime.Now,
        //                        TRANSACTION_TIME = DateTime.Now,
        //                        DR_CR = "CR",
        //                        AMOUNT = Trans_Amt,
        //                        NARRATION = "Commission",
        //                        OPENING = SupClosingAmt,
        //                        CLOSING = CommSupAddClosing,
        //                        REC_NO = 0,
        //                        COMM_AMT = SupGapCommAmt,
        //                        GST = (float)Sup_CalCulate_GST,
        //                        TDS = (float)USuper_TDSCal,
        //                        IPAddress = IpAddress,
        //                        TDS_PERCENTAGE = taxMaster.TAX_VALUE,
        //                        GST_PERCENTAGE = GST_Master.TAX_VALUE,
        //                        WHITELEVEL_ID = (long)WHT_MEM_ID,
        //                        SUPER_ID = (long)SUP_MEM_ID,
        //                        DISTRIBUTOR_ID = (long)DIS_MEM_ID,
        //                        SERVICE_ID = operatorID.ID
        //                    };
        //                    db.TBL_ACCOUNTS.Add(objSupCommPer);
        //                    db.SaveChanges();
        //                }
        //                #endregion                        
        //                var WhiteComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
        //                                 join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
        //                                  on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
        //                                 where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == 0 && commslabMob.OPERATOR_CODE == serviceprovider
        //                                 select new
        //                                 {
        //                                     SLN = commslabMob.SLN,
        //                                     commPer = commslabMob.COMM_PERCENTAGE,
        //                                     commType = commslabMob.COMMISSION_TYPE
        //                                 }).FirstOrDefault();
        //                decimal WL_Com = decimal.Parse(MerchantComm.commPer.ToString()) + decimal.Parse(DistributorComm.commPer.ToString()) + decimal.Parse(SuperComm.commPer.ToString());

        //                decimal WTLGapComm = decimal.Parse(WhiteComm.commPer.ToString()) - WL_Com;
        //                //decimal WTLGapComm = decimal.Parse(WhiteComm.commPer.ToString()) - decimal.Parse(SuperComm.commPer.ToString());
        //                //decimal WTLGapCommAmt = (Trans_Amt * WTLGapComm) / 100;
        //                #region White level Commission payment Structure
        //                var tbl_accountWhiteLevel = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == WHT_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
        //                if (tbl_accountWhiteLevel != null)
        //                {
        //                    var WLmembtype = (from mm in db.TBL_MASTER_MEMBER
        //                                      join
        //                                          rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
        //                                      where mm.MEM_ID == WHT_MEM_ID
        //                                      select new
        //                                      {
        //                                          RoleId = mm.MEMBER_ROLE,
        //                                          roleName = rm.ROLE_NAME,
        //                                          Amount = mm.BALANCE
        //                                      }).FirstOrDefault();
        //                    decimal WLClosingAmt = tbl_accountWhiteLevel.CLOSING;
        //                    //TDS And Gst Calculation fir whitelevel
        //                    //decimal WTLGapCommAmt = (Trans_Amt * WTLGapComm) / 100;
        //                    decimal UWTLGap_CommAmtValue = 0;
        //                    decimal UWhiteLTDS = 0;
        //                    decimal WTLGapCommAmt = 0;
        //                    decimal WL_CalculateGST = 0;
        //                    if (WhiteComm.commType == "FIXED")
        //                    {
        //                        UWTLGap_CommAmtValue = WTLGapComm;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {

        //                            UWhiteLTDS = decimal.Round(((UWTLGap_CommAmtValue * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                WL_CalculateGST = decimal.Round(((UWTLGap_CommAmtValue * gst_VAlue) / 100), 2);
        //                                //WTLGapCommAmt = UWTLGap_CommAmtValue + WL_CalculateGST - UWhiteLTDS;
        //                                WTLGapCommAmt = UWTLGap_CommAmtValue  - UWhiteLTDS;
        //                            }
        //                            else
        //                            {
        //                                WL_CalculateGST = 0;
        //                                //WTLGapCommAmt = UWTLGap_CommAmtValue + WL_CalculateGST - UWhiteLTDS;
        //                                WTLGapCommAmt = UWTLGap_CommAmtValue - UWhiteLTDS;
        //                            }

        //                        }
        //                        else
        //                        {                                    
        //                            UWhiteLTDS = decimal.Round(((UWTLGap_CommAmtValue - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                WL_CalculateGST = decimal.Round(((UWTLGap_CommAmtValue - gst_VAlue)));
        //                                //WTLGapCommAmt = UWTLGap_CommAmtValue + WL_CalculateGST - UWhiteLTDS;
        //                                WTLGapCommAmt = UWTLGap_CommAmtValue  - UWhiteLTDS;
        //                            }
        //                            else
        //                            {
        //                                WL_CalculateGST = 0;
        //                                //WTLGapCommAmt = UWTLGap_CommAmtValue + WL_CalculateGST - UWhiteLTDS;
        //                                WTLGapCommAmt = UWTLGap_CommAmtValue - UWhiteLTDS;
        //                            }                                    
        //                        }                                
        //                    }
        //                    else
        //                    {
        //                        UWTLGap_CommAmtValue = (Trans_Amt * WTLGapComm) / 100;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                    
        //                            UWhiteLTDS = decimal.Round(((UWTLGap_CommAmtValue * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                WL_CalculateGST = decimal.Round(((UWTLGap_CommAmtValue * gst_VAlue) / 100), 2);
        //                                //WTLGapCommAmt = UWTLGap_CommAmtValue + WL_CalculateGST - UWhiteLTDS;
        //                                WTLGapCommAmt = UWTLGap_CommAmtValue  - UWhiteLTDS;
        //                            }
        //                            else
        //                            {
        //                                WL_CalculateGST = 0;
        //                                //WTLGapCommAmt = UWTLGap_CommAmtValue + WL_CalculateGST - UWhiteLTDS;
        //                                WTLGapCommAmt = UWTLGap_CommAmtValue  - UWhiteLTDS;
        //                            }                                    
        //                        }
        //                        else
        //                        {                                    
        //                            UWhiteLTDS = decimal.Round(((UWTLGap_CommAmtValue - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                WL_CalculateGST = decimal.Round(((UWTLGap_CommAmtValue - gst_VAlue)));
        //                                //WTLGapCommAmt = UWTLGap_CommAmtValue + WL_CalculateGST - UWhiteLTDS;
        //                                WTLGapCommAmt = UWTLGap_CommAmtValue  - UWhiteLTDS;
        //                            }
        //                            else
        //                            {
        //                                WL_CalculateGST = 0;
        //                                //WTLGapCommAmt = UWTLGap_CommAmtValue + WL_CalculateGST - UWhiteLTDS;
        //                                WTLGapCommAmt = UWTLGap_CommAmtValue  - UWhiteLTDS;
        //                            }                                    
        //                        }
        //                    }
        //                    //decimal SubAmt = ClosingAmt - Trans_Amt;                  
        //                    decimal CommWLAddClosing = tbl_accountWhiteLevel.CLOSING + WTLGapCommAmt;
        //                    long WL_ID = long.Parse(WHT_MEM_ID.ToString());
        //                    TBL_ACCOUNTS objWLCommPer = new TBL_ACCOUNTS()
        //                    {
        //                        API_ID = 0,
        //                        MEM_ID = WL_ID,
        //                        MEMBER_TYPE = WLmembtype.roleName,
        //                        TRANSACTION_TYPE = rechargeType,
        //                        TRANSACTION_DATE = System.DateTime.Now,
        //                        TRANSACTION_TIME = DateTime.Now,
        //                        DR_CR = "CR",
        //                        AMOUNT = Trans_Amt,
        //                        NARRATION = "Commission",
        //                        OPENING = WLClosingAmt,
        //                        CLOSING = CommWLAddClosing,
        //                        REC_NO = 0,
        //                        COMM_AMT = WTLGapCommAmt,
        //                        GST = (float)WL_CalculateGST,
        //                        TDS = (float)UWhiteLTDS,
        //                        IPAddress = IpAddress,
        //                        TDS_PERCENTAGE = taxMaster.TAX_VALUE,
        //                        GST_PERCENTAGE = GST_Master.TAX_VALUE,
        //                        WHITELEVEL_ID = (long)WHT_MEM_ID,
        //                        SUPER_ID = (long)SUP_MEM_ID,
        //                        DISTRIBUTOR_ID = (long)DIS_MEM_ID,
        //                        SERVICE_ID = operatorID.ID
        //                    };
        //                    db.TBL_ACCOUNTS.Add(objWLCommPer);
        //                    db.SaveChanges();
        //                }
        //                #endregion
        //                ContextTransaction.Commit();
        //                return true;
        //            }
        //            else if (status == "WATER")
        //            {
        //                //var MerchantComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
        //                //                    join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
        //                //                    on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
        //                //                    where detailscom.WHITE_LEVEL_ID == DIS_MEM_ID && detailscom.INTRODUCER_ID == Mem_ID && commslabMob.OPERATOR_CODE == serviceprovider
        //                //                    select new
        //                //                    {
        //                //                        SLN = commslabMob.SLN,
        //                //                        commPer = commslabMob.COMM_PERCENTAGE
        //                //                    }).FirstOrDefault();
        //                var MerchantComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
        //                                    join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
        //                                    on detailscom.SLN equals commslabMob.SLAB_ID
        //                                    join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
        //                                    where commslabMob.OPERATOR_CODE == serviceprovider && mem.MEMBER_ROLE == 5
        //                                    select new
        //                                    {
        //                                        SLN = commslabMob.SLN,
        //                                        commPer = commslabMob.MERCHANT_COM_PER,
        //                                        commType = commslabMob.COMMISSION_TYPE
        //                                    }).FirstOrDefault();
        //                #region Retailer Commission                        
        //                var membtype = (from mm in db.TBL_MASTER_MEMBER
        //                                join
        //                                    rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
        //                                where mm.MEM_ID == Mem_ID
        //                                select new
        //                                {
        //                                    RoleId = mm.MEMBER_ROLE,
        //                                    roleName = rm.ROLE_NAME,
        //                                    Amount = mm.BALANCE
        //                                }).FirstOrDefault();
        //                var tbl_account = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == Mem_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
        //                if (tbl_account != null)
        //                {
        //                    decimal ClosingAmt = tbl_account.CLOSING;
        //                    decimal SubAmt = ClosingAmt - Trans_Amt;
        //                    TBL_ACCOUNTS objaccnt = new TBL_ACCOUNTS()
        //                    {
        //                        API_ID = 0,
        //                        MEM_ID = Mem_ID,
        //                        MEMBER_TYPE = membtype.roleName,
        //                        TRANSACTION_TYPE = rechargeType,
        //                        TRANSACTION_DATE = System.DateTime.Now,
        //                        TRANSACTION_TIME = DateTime.Now,
        //                        DR_CR = "DR",
        //                        AMOUNT = Trans_Amt,
        //                        NARRATION = "Recharge",
        //                        OPENING = ClosingAmt,
        //                        CLOSING = SubAmt,
        //                        REC_NO = 0,
        //                        COMM_AMT = 0,
        //                        GST = 0,
        //                        TDS = 0,
        //                        IPAddress = IpAddress,
        //                        TDS_PERCENTAGE = 0,
        //                        GST_PERCENTAGE = 0,
        //                        WHITELEVEL_ID = (long)WHT_MEM_ID,
        //                        SUPER_ID = (long)SUP_MEM_ID,
        //                        DISTRIBUTOR_ID = (long)DIS_MEM_ID
        //                    };
        //                    db.TBL_ACCOUNTS.Add(objaccnt);
        //                    db.SaveChanges();
        //                    // For TDS and GST Calculaiton for Merchant
        //                    //decimal getPer = (Trans_Amt * MerchantComm.commPer) / 100;
        //                    decimal WA_getPer_Value = 0;
        //                    decimal WA_Merchant_TDS = 0;
        //                    decimal getPer = 0;
        //                    decimal WAMer_CalculateGST = 0;
        //                    if (MerchantComm.commType == "FIXED")
        //                    {
        //                        WA_getPer_Value = MerchantComm.commPer;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                   
        //                            WA_Merchant_TDS = decimal.Round(((WA_getPer_Value * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                WAMer_CalculateGST = decimal.Round(((WA_getPer_Value * gst_VAlue) / 100), 2);
        //                                getPer = WA_getPer_Value + WAMer_CalculateGST - WA_Merchant_TDS;
        //                            }
        //                            else
        //                            {
        //                                WAMer_CalculateGST = 0;
        //                                getPer = WA_getPer_Value + WAMer_CalculateGST - WA_Merchant_TDS;
        //                            }

        //                        }
        //                        else
        //                        {                                    
        //                            WA_Merchant_TDS = decimal.Round(((WA_getPer_Value * GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                WAMer_CalculateGST = decimal.Round(((WA_getPer_Value - gst_VAlue)));
        //                                getPer = WA_getPer_Value + WAMer_CalculateGST - WA_Merchant_TDS;
        //                            }
        //                            else
        //                            {
        //                                WAMer_CalculateGST =0;
        //                                getPer = WA_getPer_Value + WAMer_CalculateGST - WA_Merchant_TDS;
        //                            }

        //                        }
        //                    }
        //                    else
        //                    {
        //                        WA_getPer_Value = (Trans_Amt * MerchantComm.commPer) / 100;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                    
        //                            WA_Merchant_TDS = decimal.Round(((WA_getPer_Value * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                WAMer_CalculateGST = decimal.Round(((WA_getPer_Value * gst_VAlue) / 100), 2);
        //                                getPer = WA_getPer_Value + WAMer_CalculateGST - WA_Merchant_TDS;
        //                            }
        //                            else
        //                            {
        //                                WAMer_CalculateGST = 0;
        //                                getPer = WA_getPer_Value + WAMer_CalculateGST - WA_Merchant_TDS;
        //                            }                                    
        //                        }
        //                        else
        //                        {                                   
        //                            WA_Merchant_TDS = decimal.Round(((WA_getPer_Value - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                WAMer_CalculateGST = decimal.Round(((WA_getPer_Value - gst_VAlue)));
        //                                getPer = WA_getPer_Value + WAMer_CalculateGST - WA_Merchant_TDS;
        //                            }
        //                            else
        //                            {
        //                                WAMer_CalculateGST = 0;
        //                                getPer = WA_getPer_Value + WAMer_CalculateGST - WA_Merchant_TDS;
        //                            }
        //                        }
        //                    }
        //                    decimal CommAddClosing = SubAmt + getPer;
        //                    TBL_ACCOUNTS objCommPer = new TBL_ACCOUNTS()
        //                    {
        //                        API_ID = 0,
        //                        MEM_ID = Mem_ID,
        //                        MEMBER_TYPE = membtype.roleName,
        //                        TRANSACTION_TYPE = rechargeType,
        //                        TRANSACTION_DATE = System.DateTime.Now,
        //                        TRANSACTION_TIME = DateTime.Now,
        //                        DR_CR = "CR",
        //                        AMOUNT = Trans_Amt,
        //                        NARRATION = "Commission",
        //                        OPENING = SubAmt,
        //                        CLOSING = CommAddClosing,
        //                        REC_NO = 0,
        //                        COMM_AMT = getPer,
        //                        GST = (float)WAMer_CalculateGST,
        //                        TDS = (float)WA_Merchant_TDS,
        //                        IPAddress = IpAddress,
        //                        TDS_PERCENTAGE = taxMaster.TAX_VALUE,
        //                        GST_PERCENTAGE = GST_Master.TAX_VALUE,
        //                        WHITELEVEL_ID = (long)WHT_MEM_ID,
        //                        SUPER_ID = (long)SUP_MEM_ID,
        //                        DISTRIBUTOR_ID = (long)DIS_MEM_ID
        //                    };
        //                    db.TBL_ACCOUNTS.Add(objCommPer);
        //                    db.SaveChanges();
        //                }
        //                #endregion

        //                //var DistributorComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
        //                //                       join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
        //                //                       on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
        //                //                       where detailscom.WHITE_LEVEL_ID == SUP_MEM_ID && detailscom.INTRODUCER_ID == DIS_MEM_ID && commslabMob.OPERATOR_CODE == serviceprovider
        //                //                       select new
        //                //                       {
        //                //                           SLN = commslabMob.SLN,
        //                //                           commPer = commslabMob.COMM_PERCENTAGE
        //                //                       }).FirstOrDefault();
        //                var DistributorComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
        //                                       join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
        //                                       on detailscom.SLN equals commslabMob.SLAB_ID
        //                                       join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
        //                                       where commslabMob.OPERATOR_CODE == serviceprovider && mem.MEMBER_ROLE == 4
        //                                       select new
        //                                       {
        //                                           SLN = commslabMob.SLN,
        //                                           commPer = commslabMob.DISTRIBUTOR_COM_PER,
        //                                           commType = commslabMob.COMMISSION_TYPE
        //                                       }).FirstOrDefault();
        //                #region Distributor Commission
        //                //decimal DisGapComm = decimal.Parse(DistributorComm.commPer.ToString()) - decimal.Parse(MerchantComm.commPer.ToString());
        //                decimal DisGapComm = decimal.Parse(DistributorComm.commPer.ToString());
        //                //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;
        //                var tbl_accountDistributor = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == DIS_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
        //                if (tbl_accountDistributor != null)
        //                {
        //                    var CheckDisCommission = db.TBL_DETAILS_MEMBER_COMMISSION_SLAB.Where(x => x.INTRODUCER_ID == DIS_MEM_ID).FirstOrDefault();
        //                    var Dismembtype = (from mm in db.TBL_MASTER_MEMBER
        //                                       join
        //                                           rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
        //                                       where mm.MEM_ID == DIS_MEM_ID
        //                                       select new
        //                                       {
        //                                           RoleId = mm.MEMBER_ROLE,
        //                                           roleName = rm.ROLE_NAME,
        //                                           Amount = mm.BALANCE
        //                                       }).FirstOrDefault();
        //                    decimal DisClosingAmt = tbl_accountDistributor.CLOSING;
        //                    //decimal SubAmt = ClosingAmt - Trans_Amt;                   
        //                    //decimal DisgetPer = (Trans_Amt * checkDiscomm.CommPercentage) / 100;
        //                    // For GST AND TDS calculation for Dister for water
        //                    //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;

        //                    decimal WA_DisGapComm_Amt = 0;
        //                    decimal WA_Dis_TDS_CAL = 0;
        //                    decimal DisGapCommAmt = 0;
        //                    decimal WADIST_GSTCal = 0 ;
        //                    if (DistributorComm.commType == "FIXED")
        //                    {
        //                        WA_DisGapComm_Amt = DistributorComm.commPer;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                    
        //                            WA_Dis_TDS_CAL = decimal.Round(((WA_DisGapComm_Amt * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                WADIST_GSTCal = decimal.Round(((WA_DisGapComm_Amt * gst_VAlue) / 100), 2);
        //                                DisGapCommAmt = WA_DisGapComm_Amt + WADIST_GSTCal - WA_Dis_TDS_CAL;
        //                            }
        //                            else
        //                            {
        //                                WADIST_GSTCal = 0;
        //                                DisGapCommAmt = WA_DisGapComm_Amt + WADIST_GSTCal - WA_Dis_TDS_CAL;
        //                            }                                    
        //                        }
        //                        else
        //                        {                                   
        //                            WA_Dis_TDS_CAL = decimal.Round(((WA_DisGapComm_Amt - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                WADIST_GSTCal = decimal.Round(((WA_DisGapComm_Amt - gst_VAlue)));
        //                                DisGapCommAmt = WA_DisGapComm_Amt + WADIST_GSTCal - WA_Dis_TDS_CAL;
        //                            }
        //                            else
        //                            {
        //                                WADIST_GSTCal = 0;
        //                                DisGapCommAmt = WA_DisGapComm_Amt + WADIST_GSTCal - WA_Dis_TDS_CAL;
        //                            }                                    
        //                        }
        //                    }
        //                    else
        //                    {
        //                        WA_DisGapComm_Amt = (Trans_Amt * DisGapComm) / 100;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                    
        //                            WA_Dis_TDS_CAL = decimal.Round(((WA_DisGapComm_Amt * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                WADIST_GSTCal = decimal.Round(((WA_DisGapComm_Amt * gst_VAlue) / 100), 2);
        //                                DisGapCommAmt = WA_DisGapComm_Amt + WADIST_GSTCal - WA_Dis_TDS_CAL;
        //                            }
        //                            else
        //                            {
        //                                WADIST_GSTCal = 0;
        //                                DisGapCommAmt = WA_DisGapComm_Amt + WADIST_GSTCal - WA_Dis_TDS_CAL;
        //                            }

        //                        }
        //                        else
        //                        {                                    
        //                            WA_Dis_TDS_CAL = decimal.Round(((WA_DisGapComm_Amt - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                WADIST_GSTCal = decimal.Round(((WA_DisGapComm_Amt - gst_VAlue)));
        //                                DisGapCommAmt = WA_DisGapComm_Amt + WADIST_GSTCal - WA_Dis_TDS_CAL;
        //                            }
        //                            else
        //                            {
        //                                WADIST_GSTCal = 0;
        //                                DisGapCommAmt = WA_DisGapComm_Amt + WADIST_GSTCal - WA_Dis_TDS_CAL;
        //                            }
        //                        }

        //                    }

        //                    decimal CommDisAddClosing = tbl_accountDistributor.CLOSING + DisGapCommAmt;
        //                    long Dis_ID = long.Parse(DIS_MEM_ID.ToString());
        //                    TBL_ACCOUNTS objDisCommPer = new TBL_ACCOUNTS()
        //                    {
        //                        API_ID = 0,
        //                        MEM_ID = Dis_ID,
        //                        MEMBER_TYPE = Dismembtype.roleName,
        //                        TRANSACTION_TYPE = rechargeType,
        //                        TRANSACTION_DATE = System.DateTime.Now,
        //                        TRANSACTION_TIME = DateTime.Now,
        //                        DR_CR = "CR",
        //                        AMOUNT = Trans_Amt,
        //                        NARRATION = "Commission",
        //                        OPENING = DisClosingAmt,
        //                        CLOSING = CommDisAddClosing,
        //                        REC_NO = 0,
        //                        COMM_AMT = DisGapCommAmt,
        //                        GST = (float)WADIST_GSTCal,
        //                        TDS = (float)WA_Dis_TDS_CAL,
        //                        IPAddress = IpAddress,
        //                        TDS_PERCENTAGE = taxMaster.TAX_VALUE,
        //                        GST_PERCENTAGE = GST_Master.TAX_VALUE,
        //                        WHITELEVEL_ID = (long)WHT_MEM_ID,
        //                        SUPER_ID = (long)SUP_MEM_ID,
        //                        DISTRIBUTOR_ID = (long)DIS_MEM_ID
        //                    };
        //                    db.TBL_ACCOUNTS.Add(objDisCommPer);
        //                    db.SaveChanges();
        //                }
        //                #endregion
        //                //var SuperComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
        //                //                 join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
        //                //                  on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
        //                //                 where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == SUP_MEM_ID && commslabMob.OPERATOR_CODE == serviceprovider
        //                //                 select new
        //                //                 {
        //                //                     SLN = commslabMob.SLN,
        //                //                     commPer = commslabMob.COMM_PERCENTAGE
        //                //                 }).FirstOrDefault();
        //                var SuperComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
        //                                 join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
        //                                  on detailscom.SLN equals commslabMob.SLAB_ID
        //                                 join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
        //                                 where commslabMob.OPERATOR_CODE == serviceprovider && mem.MEMBER_ROLE == 3
        //                                 select new
        //                                 {
        //                                     SLN = commslabMob.SLN,
        //                                     commPer = commslabMob.SUPER_COM_PER,
        //                                     commType = commslabMob.COMMISSION_TYPE
        //                                 }).FirstOrDefault();
        //                //decimal SupGapComm = decimal.Parse(SuperComm.commPer.ToString()) - decimal.Parse(DistributorComm.commPer.ToString());
        //                decimal SupGapComm = decimal.Parse(SuperComm.commPer.ToString());
        //                //decimal SupGapCommAmt = (Trans_Amt * SupGapComm) / 100;
        //                #region Super Commission
        //                var tbl_accountSuper = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == SUP_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
        //                if (tbl_accountSuper != null)
        //                {
        //                    var Supmembtype = (from mm in db.TBL_MASTER_MEMBER
        //                                       join
        //                                           rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
        //                                       where mm.MEM_ID == SUP_MEM_ID
        //                                       select new
        //                                       {
        //                                           RoleId = mm.MEMBER_ROLE,
        //                                           roleName = rm.ROLE_NAME,
        //                                           Amount = mm.BALANCE
        //                                       }).FirstOrDefault();
        //                    decimal SupClosingAmt = tbl_accountSuper.CLOSING;
        //                    // TDS and GST CAlculation for Super for Water
        //                    decimal WA_Sup_Gap_CommAmt = 0;
        //                    decimal WA_Sup_TDS_CAL_Val = 0;
        //                    decimal SupGapCommAmt = 0;
        //                    decimal WASuper_GSTCAl = 0;
        //                    if (SuperComm.commType == "FIXED")
        //                    {
        //                        WA_Sup_Gap_CommAmt = SuperComm.commPer;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                    
        //                            WA_Sup_TDS_CAL_Val = decimal.Round(((WA_Sup_Gap_CommAmt * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                WASuper_GSTCAl = decimal.Round(((WA_Sup_Gap_CommAmt * gst_VAlue) / 100), 2);
        //                                SupGapCommAmt = (WA_Sup_Gap_CommAmt + WASuper_GSTCAl - WA_Sup_TDS_CAL_Val);
        //                            }
        //                            else
        //                            {
        //                                WASuper_GSTCAl = 0;
        //                                SupGapCommAmt = (WA_Sup_Gap_CommAmt + WASuper_GSTCAl - WA_Sup_TDS_CAL_Val);
        //                            }                                    
        //                        }
        //                        else
        //                        {                                   
        //                            WA_Sup_TDS_CAL_Val = decimal.Round(((WA_Sup_Gap_CommAmt - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                WASuper_GSTCAl = decimal.Round(((WA_Sup_Gap_CommAmt - gst_VAlue)));
        //                                SupGapCommAmt = (WA_Sup_Gap_CommAmt + WASuper_GSTCAl - WA_Sup_TDS_CAL_Val);
        //                            }
        //                            else
        //                            {
        //                                WASuper_GSTCAl =0;
        //                                SupGapCommAmt = (WA_Sup_Gap_CommAmt + WASuper_GSTCAl - WA_Sup_TDS_CAL_Val);
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        WA_Sup_Gap_CommAmt = (Trans_Amt * SupGapComm) / 100;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                    
        //                            WA_Sup_TDS_CAL_Val = decimal.Round(((WA_Sup_Gap_CommAmt * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                WASuper_GSTCAl = decimal.Round(((WA_Sup_Gap_CommAmt * gst_VAlue) / 100), 2);
        //                                SupGapCommAmt = (WA_Sup_Gap_CommAmt + WASuper_GSTCAl - WA_Sup_TDS_CAL_Val);
        //                            }
        //                            else
        //                            {
        //                                WASuper_GSTCAl = 0;
        //                                SupGapCommAmt = (WA_Sup_Gap_CommAmt + WASuper_GSTCAl - WA_Sup_TDS_CAL_Val);
        //                            }

        //                        }
        //                        else
        //                        {                                   
        //                            WA_Sup_TDS_CAL_Val = decimal.Round(((WA_Sup_Gap_CommAmt - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                WASuper_GSTCAl = decimal.Round(((WA_Sup_Gap_CommAmt - gst_VAlue)));
        //                                SupGapCommAmt = (WA_Sup_Gap_CommAmt + WASuper_GSTCAl - WA_Sup_TDS_CAL_Val);
        //                            }
        //                            else
        //                            {
        //                                WASuper_GSTCAl = 0;
        //                                SupGapCommAmt = (WA_Sup_Gap_CommAmt + WASuper_GSTCAl - WA_Sup_TDS_CAL_Val);
        //                            }                                    
        //                        }

        //                    }

        //                    decimal CommSupAddClosing = tbl_accountSuper.CLOSING + SupGapCommAmt;
        //                    long Sup_ID = long.Parse(SUP_MEM_ID.ToString());
        //                    TBL_ACCOUNTS objSupCommPer = new TBL_ACCOUNTS()
        //                    {
        //                        API_ID = 0,
        //                        MEM_ID = Sup_ID,
        //                        MEMBER_TYPE = Supmembtype.roleName,
        //                        TRANSACTION_TYPE = rechargeType,
        //                        TRANSACTION_DATE = System.DateTime.Now,
        //                        TRANSACTION_TIME = DateTime.Now,
        //                        DR_CR = "CR",
        //                        AMOUNT = Trans_Amt,
        //                        NARRATION = "Commission",
        //                        OPENING = SupClosingAmt,
        //                        CLOSING = CommSupAddClosing,
        //                        REC_NO = 0,
        //                        COMM_AMT = SupGapCommAmt,
        //                        GST = (float)WASuper_GSTCAl,
        //                        TDS = (float)WA_Sup_TDS_CAL_Val,
        //                        IPAddress = IpAddress,
        //                        TDS_PERCENTAGE = taxMaster.TAX_VALUE,
        //                        GST_PERCENTAGE = GST_Master.TAX_VALUE,
        //                        WHITELEVEL_ID = (long)WHT_MEM_ID,
        //                        SUPER_ID = (long)SUP_MEM_ID,
        //                        DISTRIBUTOR_ID = (long)DIS_MEM_ID
        //                    };
        //                    db.TBL_ACCOUNTS.Add(objSupCommPer);
        //                    db.SaveChanges();
        //                }
        //                #endregion                        
        //                var WhiteComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
        //                                 join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
        //                                  on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
        //                                 where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == 0 && commslabMob.OPERATOR_CODE == serviceprovider
        //                                 select new
        //                                 {
        //                                     SLN = commslabMob.SLN,
        //                                     commPer = commslabMob.COMM_PERCENTAGE,
        //                                     commType = commslabMob.COMMISSION_TYPE
        //                                 }).FirstOrDefault();
        //                decimal Gap_WLlevel = decimal.Parse(MerchantComm.commPer.ToString()) + decimal.Parse(DistributorComm.commPer.ToString()) + decimal.Parse(SuperComm.commPer.ToString());

        //                //decimal WTLGapComm = decimal.Parse(WhiteComm.commPer.ToString()) - decimal.Parse(SuperComm.commPer.ToString());
        //                decimal WTLGapComm = decimal.Parse(WhiteComm.commPer.ToString()) - Gap_WLlevel;
        //                //decimal WTLGapCommAmt = (Trans_Amt * WTLGapComm) / 100;
        //                #region White level Commission payment Structure
        //                var tbl_accountWhiteLevel = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == WHT_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
        //                if (tbl_accountWhiteLevel != null)
        //                {
        //                    var WLmembtype = (from mm in db.TBL_MASTER_MEMBER
        //                                      join
        //                                          rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
        //                                      where mm.MEM_ID == WHT_MEM_ID
        //                                      select new
        //                                      {
        //                                          RoleId = mm.MEMBER_ROLE,
        //                                          roleName = rm.ROLE_NAME,
        //                                          Amount = mm.BALANCE
        //                                      }).FirstOrDefault();
        //                    decimal WLClosingAmt = tbl_accountWhiteLevel.CLOSING;
        //                    //decimal SubAmt = ClosingAmt - Trans_Amt;                  
        //                    // TDS and GST Calculation for white level Water
        //                    decimal WTL_Gap_Comm_Amt_Val = 0;
        //                    decimal WL_TDSCAl_VAlue = 0;
        //                    decimal WTLGapCommAmt = 0;
        //                    decimal WA_WLGSTCalculation = 0;
        //                    if (WhiteComm.commType == "FIXED")
        //                    {
        //                        WTL_Gap_Comm_Amt_Val = WTLGapComm;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                    
        //                            WL_TDSCAl_VAlue = decimal.Round(((WTL_Gap_Comm_Amt_Val * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                WA_WLGSTCalculation = decimal.Round(((WTL_Gap_Comm_Amt_Val * gst_VAlue) / 100), 2);
        //                                WTLGapCommAmt = WTL_Gap_Comm_Amt_Val + WA_WLGSTCalculation - WL_TDSCAl_VAlue;
        //                            }
        //                            else
        //                            {
        //                                WA_WLGSTCalculation = 0;
        //                                WTLGapCommAmt = WTL_Gap_Comm_Amt_Val + WA_WLGSTCalculation - WL_TDSCAl_VAlue;
        //                            }                                    
        //                        }
        //                        else
        //                        {                                    
        //                            WL_TDSCAl_VAlue = decimal.Round(((WTL_Gap_Comm_Amt_Val - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                WA_WLGSTCalculation = decimal.Round(((WTL_Gap_Comm_Amt_Val - gst_VAlue)));
        //                                WTLGapCommAmt = WTL_Gap_Comm_Amt_Val + WA_WLGSTCalculation - WL_TDSCAl_VAlue;
        //                            }
        //                            else
        //                            {
        //                                WA_WLGSTCalculation = 0;
        //                                WTLGapCommAmt = WTL_Gap_Comm_Amt_Val + WA_WLGSTCalculation - WL_TDSCAl_VAlue;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        WTL_Gap_Comm_Amt_Val = (Trans_Amt * WTLGapComm) / 100;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                    
        //                            WL_TDSCAl_VAlue = decimal.Round(((WTL_Gap_Comm_Amt_Val * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                WA_WLGSTCalculation = decimal.Round(((WTL_Gap_Comm_Amt_Val * gst_VAlue) / 100), 2);
        //                                WTLGapCommAmt = WTL_Gap_Comm_Amt_Val + WA_WLGSTCalculation - WL_TDSCAl_VAlue;
        //                            }
        //                            else
        //                            {
        //                                WA_WLGSTCalculation = 0;
        //                                WTLGapCommAmt = WTL_Gap_Comm_Amt_Val + WA_WLGSTCalculation - WL_TDSCAl_VAlue;
        //                            }                                    
        //                        }
        //                        else
        //                        {                                  
        //                            WL_TDSCAl_VAlue = decimal.Round(((WTL_Gap_Comm_Amt_Val - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                WA_WLGSTCalculation = decimal.Round(((WTL_Gap_Comm_Amt_Val - gst_VAlue)));
        //                                WTLGapCommAmt = WTL_Gap_Comm_Amt_Val + WA_WLGSTCalculation - WL_TDSCAl_VAlue;
        //                            }
        //                            else
        //                            {
        //                                WA_WLGSTCalculation =0;
        //                                WTLGapCommAmt = WTL_Gap_Comm_Amt_Val + WA_WLGSTCalculation - WL_TDSCAl_VAlue;
        //                            }
        //                        }
        //                    }
        //                    decimal CommWLAddClosing = tbl_accountWhiteLevel.CLOSING + WTLGapCommAmt;
        //                    long WL_ID = long.Parse(WHT_MEM_ID.ToString());
        //                    TBL_ACCOUNTS objWLCommPer = new TBL_ACCOUNTS()
        //                    {
        //                        API_ID = 0,
        //                        MEM_ID = WL_ID,
        //                        MEMBER_TYPE = WLmembtype.roleName,
        //                        TRANSACTION_TYPE = rechargeType,
        //                        TRANSACTION_DATE = System.DateTime.Now,
        //                        TRANSACTION_TIME = DateTime.Now,
        //                        DR_CR = "CR",
        //                        AMOUNT = Trans_Amt,
        //                        NARRATION = "Commission",
        //                        OPENING = WLClosingAmt,
        //                        CLOSING = CommWLAddClosing,
        //                        REC_NO = 0,
        //                        COMM_AMT = WTLGapCommAmt,
        //                        GST = (float)WA_WLGSTCalculation,
        //                        TDS = (float)WL_TDSCAl_VAlue,
        //                        IPAddress = IpAddress,
        //                        TDS_PERCENTAGE = taxMaster.TAX_VALUE,
        //                        GST_PERCENTAGE = GST_Master.TAX_VALUE,
        //                        WHITELEVEL_ID = (long)WHT_MEM_ID,
        //                        SUPER_ID = (long)SUP_MEM_ID,
        //                        DISTRIBUTOR_ID = (long)DIS_MEM_ID
        //                    };
        //                    db.TBL_ACCOUNTS.Add(objWLCommPer);
        //                    db.SaveChanges();
        //                }
        //                #endregion
        //                ContextTransaction.Commit();
        //                return true;
        //            }
        //            else if (status == "ELECTRICITY")
        //            {
        //                //var MerchantComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
        //                //                    join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
        //                //                    on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
        //                //                    where detailscom.WHITE_LEVEL_ID == DIS_MEM_ID && detailscom.INTRODUCER_ID == Mem_ID && commslabMob.OPERATOR_CODE == serviceprovider
        //                //                    select new
        //                //                    {
        //                //                        SLN = commslabMob.SLN,
        //                //                        commPer = commslabMob.COMM_PERCENTAGE
        //                //                    }).FirstOrDefault();
        //                var MerchantComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
        //                                    join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
        //                                    on detailscom.SLN equals commslabMob.SLAB_ID
        //                                    join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
        //                                    where commslabMob.OPERATOR_CODE == serviceprovider && mem.MEMBER_ROLE == 5
        //                                    select new
        //                                    {
        //                                        SLN = commslabMob.SLN,
        //                                        commPer = commslabMob.MERCHANT_COM_PER,
        //                                        commType = commslabMob.COMMISSION_TYPE
        //                                    }).FirstOrDefault();
        //                #region Retailer Commission                        
        //                var membtype = (from mm in db.TBL_MASTER_MEMBER
        //                                join
        //                                    rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
        //                                where mm.MEM_ID == Mem_ID
        //                                select new
        //                                {
        //                                    RoleId = mm.MEMBER_ROLE,
        //                                    roleName = rm.ROLE_NAME,
        //                                    Amount = mm.BALANCE
        //                                }).FirstOrDefault();
        //                var tbl_account = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == Mem_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
        //                if (tbl_account != null)
        //                {
        //                    decimal ClosingAmt = tbl_account.CLOSING;
        //                    decimal SubAmt = ClosingAmt - Trans_Amt;
        //                    TBL_ACCOUNTS objaccnt = new TBL_ACCOUNTS()
        //                    {
        //                        API_ID = 0,
        //                        MEM_ID = Mem_ID,
        //                        MEMBER_TYPE = membtype.roleName,
        //                        TRANSACTION_TYPE = rechargeType,
        //                        TRANSACTION_DATE = System.DateTime.Now,
        //                        TRANSACTION_TIME = DateTime.Now,
        //                        DR_CR = "DR",
        //                        AMOUNT = Trans_Amt,
        //                        NARRATION = "Recharge",
        //                        OPENING = ClosingAmt,
        //                        CLOSING = SubAmt,
        //                        REC_NO = 0,
        //                        COMM_AMT = 0,
        //                        GST = 0,
        //                        TDS = 0,
        //                        IPAddress = IpAddress,
        //                        TDS_PERCENTAGE = 0,
        //                        GST_PERCENTAGE = 0,
        //                        WHITELEVEL_ID = (long)WHT_MEM_ID,
        //                        SUPER_ID = (long)SUP_MEM_ID,
        //                        DISTRIBUTOR_ID = (long)DIS_MEM_ID
        //                    };
        //                    db.TBL_ACCOUNTS.Add(objaccnt);
        //                    db.SaveChanges();
        //                    // TDS and GST CAlculation for Merchant for electricity
        //                    //decimal getPer = (Trans_Amt * MerchantComm.commPer) / 100;
        //                    decimal ELE_getPer_Val = 0;
        //                    decimal ELE_TDS_Value = 0;
        //                    decimal getPer = 0;
        //                    decimal ELE_MER_GST_Cal = 0;
        //                    if (MerchantComm.commType == "FIXED")
        //                    {
        //                        ELE_getPer_Val = MerchantComm.commPer;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                   
        //                            ELE_TDS_Value = decimal.Round(((ELE_getPer_Val * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                ELE_MER_GST_Cal = decimal.Round(((ELE_getPer_Val * gst_VAlue) / 100), 2);
        //                                getPer = (ELE_getPer_Val + ELE_MER_GST_Cal - ELE_TDS_Value);
        //                            }
        //                            else
        //                            {
        //                                ELE_MER_GST_Cal = 0;
        //                                getPer = (ELE_getPer_Val + ELE_MER_GST_Cal - ELE_TDS_Value);
        //                            }                                    
        //                        }
        //                        else
        //                        {                                   
        //                            ELE_TDS_Value = decimal.Round(((ELE_getPer_Val - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                ELE_MER_GST_Cal = decimal.Round(((ELE_getPer_Val - gst_VAlue)));
        //                                getPer = (ELE_getPer_Val + ELE_MER_GST_Cal - ELE_TDS_Value);
        //                            }
        //                            else
        //                            {
        //                                ELE_MER_GST_Cal = 0;
        //                                getPer = (ELE_getPer_Val + ELE_MER_GST_Cal - ELE_TDS_Value);
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        ELE_getPer_Val = (Trans_Amt * MerchantComm.commPer) / 100;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {

        //                            ELE_TDS_Value = decimal.Round(((ELE_getPer_Val * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                ELE_MER_GST_Cal = decimal.Round(((ELE_getPer_Val * gst_VAlue) / 100), 2);
        //                                getPer = (ELE_getPer_Val + ELE_MER_GST_Cal - ELE_TDS_Value);
        //                            }
        //                            else
        //                            {
        //                                ELE_MER_GST_Cal = 0;
        //                                getPer = (ELE_getPer_Val + ELE_MER_GST_Cal - ELE_TDS_Value);
        //                            }
        //                        }
        //                        else
        //                        {                                   
        //                            ELE_TDS_Value = decimal.Round(((ELE_getPer_Val * GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                ELE_MER_GST_Cal = decimal.Round(((ELE_getPer_Val * gst_VAlue)));
        //                                getPer = (ELE_getPer_Val + ELE_MER_GST_Cal - ELE_TDS_Value);
        //                            }
        //                            else
        //                            {
        //                                ELE_MER_GST_Cal = 0;
        //                                getPer = (ELE_getPer_Val + ELE_MER_GST_Cal - ELE_TDS_Value);
        //                            }
        //                        }
        //                    }

        //                    decimal CommAddClosing = SubAmt + getPer;
        //                    TBL_ACCOUNTS objCommPer = new TBL_ACCOUNTS()
        //                    {
        //                        API_ID = 0,
        //                        MEM_ID = Mem_ID,
        //                        MEMBER_TYPE = membtype.roleName,
        //                        TRANSACTION_TYPE = rechargeType,
        //                        TRANSACTION_DATE = System.DateTime.Now,
        //                        TRANSACTION_TIME = DateTime.Now,
        //                        DR_CR = "CR",
        //                        AMOUNT = Trans_Amt,
        //                        NARRATION = "Commission",
        //                        OPENING = SubAmt,
        //                        CLOSING = CommAddClosing,
        //                        REC_NO = 0,
        //                        COMM_AMT = getPer,
        //                        GST = (float)ELE_MER_GST_Cal,
        //                        TDS = (float)ELE_TDS_Value,
        //                        IPAddress = IpAddress,
        //                        TDS_PERCENTAGE = taxMaster.TAX_VALUE,
        //                        GST_PERCENTAGE = GST_Master.TAX_VALUE,
        //                        WHITELEVEL_ID = (long)WHT_MEM_ID,
        //                        SUPER_ID = (long)SUP_MEM_ID,
        //                        DISTRIBUTOR_ID = (long)DIS_MEM_ID
        //                    };
        //                    db.TBL_ACCOUNTS.Add(objCommPer);
        //                    db.SaveChanges();
        //                }
        //                #endregion

        //                //var DistributorComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
        //                //                       join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
        //                //                       on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
        //                //                       where detailscom.WHITE_LEVEL_ID == SUP_MEM_ID && detailscom.INTRODUCER_ID == DIS_MEM_ID && commslabMob.OPERATOR_CODE == serviceprovider
        //                //                       select new
        //                //                       {
        //                //                           SLN = commslabMob.SLN,
        //                //                           commPer = commslabMob.COMM_PERCENTAGE
        //                //                       }).FirstOrDefault();
        //                var DistributorComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
        //                                       join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
        //                                       on detailscom.SLN equals commslabMob.SLAB_ID
        //                                       join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
        //                                       where commslabMob.OPERATOR_CODE == serviceprovider && mem.MEMBER_ROLE == 4
        //                                       select new
        //                                       {
        //                                           SLN = commslabMob.SLN,
        //                                           commPer = commslabMob.DISTRIBUTOR_COM_PER,
        //                                           commType = commslabMob.COMMISSION_TYPE
        //                                       }).FirstOrDefault();
        //                #region Distributor Commission
        //                //decimal DisGapComm = decimal.Parse(DistributorComm.commPer.ToString()) - decimal.Parse(MerchantComm.commPer.ToString());
        //                decimal DisGapComm = decimal.Parse(DistributorComm.commPer.ToString());
        //                //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;
        //                var tbl_accountDistributor = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == DIS_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
        //                if (tbl_accountDistributor != null)
        //                {
        //                    var CheckDisCommission = db.TBL_DETAILS_MEMBER_COMMISSION_SLAB.Where(x => x.INTRODUCER_ID == DIS_MEM_ID).FirstOrDefault();
        //                    var Dismembtype = (from mm in db.TBL_MASTER_MEMBER
        //                                       join
        //                                           rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
        //                                       where mm.MEM_ID == DIS_MEM_ID
        //                                       select new
        //                                       {
        //                                           RoleId = mm.MEMBER_ROLE,
        //                                           roleName = rm.ROLE_NAME,
        //                                           Amount = mm.BALANCE
        //                                       }).FirstOrDefault();
        //                    decimal DisClosingAmt = tbl_accountDistributor.CLOSING;
        //                    //decimal SubAmt = ClosingAmt - Trans_Amt;                   
        //                    //decimal DisgetPer = (Trans_Amt * checkDiscomm.CommPercentage) / 100;

        //                    // For TDS and GST CAlculation for Electricity
        //                    //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;
        //                    decimal ELE_DisGap_CommAmt = 0;
        //                    decimal ELE_TDSCALCULATION = 0;
        //                    decimal DisGapCommAmt = 0;
        //                    decimal ELE_DIST_GSTCalculation = 0;
        //                    if (DistributorComm.commType == "FIXED")
        //                    {
        //                        ELE_DisGap_CommAmt = DisGapComm;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                    
        //                            ELE_TDSCALCULATION = decimal.Round(((ELE_DisGap_CommAmt * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                ELE_DIST_GSTCalculation = decimal.Round(((ELE_DisGap_CommAmt * gst_VAlue) / 100), 2);
        //                                DisGapCommAmt = ELE_DisGap_CommAmt + ELE_DIST_GSTCalculation - ELE_TDSCALCULATION;
        //                            }
        //                            else
        //                            {
        //                                ELE_DIST_GSTCalculation = 0;
        //                                DisGapCommAmt = ELE_DisGap_CommAmt + ELE_DIST_GSTCalculation - ELE_TDSCALCULATION;
        //                            }                                    
        //                        }
        //                        else
        //                        {                                    
        //                            ELE_TDSCALCULATION = decimal.Round(((ELE_DisGap_CommAmt - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                ELE_DIST_GSTCalculation = decimal.Round(((ELE_DisGap_CommAmt - gst_VAlue)));
        //                                DisGapCommAmt = ELE_DisGap_CommAmt + ELE_DIST_GSTCalculation - ELE_TDSCALCULATION;
        //                            }
        //                            else
        //                            {
        //                                ELE_DIST_GSTCalculation = 0;
        //                                DisGapCommAmt = ELE_DisGap_CommAmt + ELE_DIST_GSTCalculation - ELE_TDSCALCULATION;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        ELE_DisGap_CommAmt = (Trans_Amt * DisGapComm) / 100;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                    
        //                            ELE_TDSCALCULATION = decimal.Round(((ELE_DisGap_CommAmt * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                ELE_DIST_GSTCalculation = decimal.Round(((ELE_DisGap_CommAmt * gst_VAlue) / 100), 2);
        //                                DisGapCommAmt = ELE_DisGap_CommAmt + ELE_DIST_GSTCalculation - ELE_TDSCALCULATION;
        //                            }
        //                            else
        //                            {
        //                                ELE_DIST_GSTCalculation =0;
        //                                DisGapCommAmt = ELE_DisGap_CommAmt + ELE_DIST_GSTCalculation - ELE_TDSCALCULATION;
        //                            }                                    
        //                        }
        //                        else
        //                        {                                    
        //                            ELE_TDSCALCULATION = decimal.Round(((ELE_DisGap_CommAmt - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                ELE_DIST_GSTCalculation = decimal.Round(((ELE_DisGap_CommAmt - gst_VAlue)));
        //                                DisGapCommAmt = ELE_DisGap_CommAmt + ELE_DIST_GSTCalculation - ELE_TDSCALCULATION;
        //                            }
        //                            else
        //                            {
        //                                ELE_DIST_GSTCalculation = 0;
        //                                DisGapCommAmt = ELE_DisGap_CommAmt + ELE_DIST_GSTCalculation - ELE_TDSCALCULATION;
        //                            }
        //                        }                                    
        //                    }
        //                    decimal CommDisAddClosing = tbl_accountDistributor.CLOSING + DisGapCommAmt;
        //                    long Dis_ID = long.Parse(DIS_MEM_ID.ToString());
        //                    TBL_ACCOUNTS objDisCommPer = new TBL_ACCOUNTS()
        //                    {
        //                        API_ID = 0,
        //                        MEM_ID = Dis_ID,
        //                        MEMBER_TYPE = Dismembtype.roleName,
        //                        TRANSACTION_TYPE = rechargeType,
        //                        TRANSACTION_DATE = System.DateTime.Now,
        //                        TRANSACTION_TIME = DateTime.Now,
        //                        DR_CR = "CR",
        //                        AMOUNT = Trans_Amt,
        //                        NARRATION = "Commission",
        //                        OPENING = DisClosingAmt,
        //                        CLOSING = CommDisAddClosing,
        //                        REC_NO = 0,
        //                        COMM_AMT = DisGapCommAmt,
        //                        GST = (float)ELE_DIST_GSTCalculation,
        //                        TDS = (float)ELE_TDSCALCULATION,
        //                        IPAddress = IpAddress,
        //                        TDS_PERCENTAGE = taxMaster.TAX_VALUE,
        //                        GST_PERCENTAGE = GST_Master.TAX_VALUE,
        //                        WHITELEVEL_ID = (long)WHT_MEM_ID,
        //                        SUPER_ID = (long)SUP_MEM_ID,
        //                        DISTRIBUTOR_ID = (long)DIS_MEM_ID
        //                    };
        //                    db.TBL_ACCOUNTS.Add(objDisCommPer);
        //                    db.SaveChanges();
        //                }
        //                #endregion
        //                //var SuperComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
        //                //                 join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
        //                //                  on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
        //                //                 where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == SUP_MEM_ID && commslabMob.OPERATOR_CODE == serviceprovider
        //                //                 select new
        //                //                 {
        //                //                     SLN = commslabMob.SLN,
        //                //                     commPer = commslabMob.COMM_PERCENTAGE
        //                //                 }).FirstOrDefault();
        //                var SuperComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
        //                                 join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
        //                                  on detailscom.SLN equals commslabMob.SLAB_ID
        //                                 join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
        //                                 where commslabMob.OPERATOR_CODE == serviceprovider && mem.MEMBER_ROLE == 3
        //                                 select new
        //                                 {
        //                                     SLN = commslabMob.SLN,
        //                                     commPer = commslabMob.SUPER_COM_PER,
        //                                     commType = commslabMob.COMMISSION_TYPE
        //                                 }).FirstOrDefault();
        //                //decimal SupGapComm = decimal.Parse(SuperComm.commPer.ToString()) - decimal.Parse(DistributorComm.commPer.ToString());
        //                decimal SupGapComm = decimal.Parse(SuperComm.commPer.ToString());
        //                //decimal SupGapCommAmt = (Trans_Amt * SupGapComm) / 100;
        //                #region Super Commission
        //                var tbl_accountSuper = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == SUP_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
        //                if (tbl_accountSuper != null)
        //                {
        //                    var Supmembtype = (from mm in db.TBL_MASTER_MEMBER
        //                                       join
        //                                           rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
        //                                       where mm.MEM_ID == SUP_MEM_ID
        //                                       select new
        //                                       {
        //                                           RoleId = mm.MEMBER_ROLE,
        //                                           roleName = rm.ROLE_NAME,
        //                                           Amount = mm.BALANCE
        //                                       }).FirstOrDefault();
        //                    decimal SupClosingAmt = tbl_accountSuper.CLOSING;
        //                    // TDS AND GST CALCULATION FOR SUPER FOR ELECTRICITY
        //                    decimal ELE_Sup_Gap_Comm_Amt = 0;
        //                    decimal ELE_SUP_TDS_CAL = 0;
        //                    decimal SupGapCommAmt = 0;
        //                    decimal ELE_SUperGSTCalcu = 0;
        //                    if (SuperComm.commType == "FIXED")
        //                    {
        //                        ELE_Sup_Gap_Comm_Amt = SupGapComm;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                    
        //                            ELE_SUP_TDS_CAL = decimal.Round(((ELE_Sup_Gap_Comm_Amt * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                ELE_SUperGSTCalcu = decimal.Round(((ELE_Sup_Gap_Comm_Amt * gst_VAlue) / 100), 2);
        //                                SupGapCommAmt = ELE_Sup_Gap_Comm_Amt + ELE_SUperGSTCalcu - ELE_SUP_TDS_CAL;
        //                            }
        //                            else
        //                            {
        //                                ELE_SUperGSTCalcu = 0;
        //                                SupGapCommAmt = ELE_Sup_Gap_Comm_Amt + ELE_SUperGSTCalcu - ELE_SUP_TDS_CAL;
        //                            }
        //                        }
        //                        else
        //                        {                                    
        //                            ELE_SUP_TDS_CAL = decimal.Round(((ELE_Sup_Gap_Comm_Amt * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                ELE_SUperGSTCalcu = decimal.Round(((ELE_Sup_Gap_Comm_Amt * gst_VAlue) / 100), 2);
        //                                SupGapCommAmt = ELE_Sup_Gap_Comm_Amt + ELE_SUperGSTCalcu - ELE_SUP_TDS_CAL;
        //                            }
        //                            else
        //                            {
        //                                ELE_SUperGSTCalcu =0;
        //                                SupGapCommAmt = ELE_Sup_Gap_Comm_Amt + ELE_SUperGSTCalcu - ELE_SUP_TDS_CAL;
        //                            }
        //                        }                                    
        //                    }
        //                    else
        //                    {
        //                        ELE_Sup_Gap_Comm_Amt = (Trans_Amt * SupGapComm) / 100;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {

        //                            ELE_SUP_TDS_CAL = decimal.Round(((ELE_Sup_Gap_Comm_Amt * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                ELE_SUperGSTCalcu = decimal.Round(((ELE_Sup_Gap_Comm_Amt * gst_VAlue) / 100), 2);
        //                                SupGapCommAmt = ELE_Sup_Gap_Comm_Amt + ELE_SUperGSTCalcu - ELE_SUP_TDS_CAL;
        //                            }
        //                            else
        //                            {
        //                                ELE_SUperGSTCalcu = 0;
        //                                SupGapCommAmt = ELE_Sup_Gap_Comm_Amt + ELE_SUperGSTCalcu - ELE_SUP_TDS_CAL;
        //                            }

        //                        }
        //                        else
        //                        {

        //                            ELE_SUP_TDS_CAL = decimal.Round(((ELE_Sup_Gap_Comm_Amt * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                ELE_SUperGSTCalcu = decimal.Round(((ELE_Sup_Gap_Comm_Amt * gst_VAlue) / 100), 2);
        //                                SupGapCommAmt = ELE_Sup_Gap_Comm_Amt + ELE_SUperGSTCalcu - ELE_SUP_TDS_CAL;
        //                            }
        //                            else
        //                            {
        //                                ELE_SUperGSTCalcu =0;
        //                                SupGapCommAmt = ELE_Sup_Gap_Comm_Amt + ELE_SUperGSTCalcu - ELE_SUP_TDS_CAL;
        //                            }

        //                        }
        //                    }

        //                    decimal CommSupAddClosing = tbl_accountSuper.CLOSING + SupGapCommAmt;
        //                    long Sup_ID = long.Parse(SUP_MEM_ID.ToString());
        //                    TBL_ACCOUNTS objSupCommPer = new TBL_ACCOUNTS()
        //                    {
        //                        API_ID = 0,
        //                        MEM_ID = Sup_ID,
        //                        MEMBER_TYPE = Supmembtype.roleName,
        //                        TRANSACTION_TYPE = rechargeType,
        //                        TRANSACTION_DATE = System.DateTime.Now,
        //                        TRANSACTION_TIME = DateTime.Now,
        //                        DR_CR = "CR",
        //                        AMOUNT = Trans_Amt,
        //                        NARRATION = "Commission",
        //                        OPENING = SupClosingAmt,
        //                        CLOSING = CommSupAddClosing,
        //                        REC_NO = 0,
        //                        COMM_AMT = SupGapCommAmt,
        //                        GST = (float)ELE_SUperGSTCalcu,
        //                        TDS = (float)ELE_SUP_TDS_CAL,
        //                        IPAddress = IpAddress,
        //                        TDS_PERCENTAGE = taxMaster.TAX_VALUE,
        //                        GST_PERCENTAGE = GST_Master.TAX_VALUE,
        //                        WHITELEVEL_ID = (long)WHT_MEM_ID,
        //                        SUPER_ID = (long)SUP_MEM_ID,
        //                        DISTRIBUTOR_ID = (long)DIS_MEM_ID
        //                    };
        //                    db.TBL_ACCOUNTS.Add(objSupCommPer);
        //                    db.SaveChanges();
        //                }
        //                #endregion                        
        //                var WhiteComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
        //                                 join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
        //                                  on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
        //                                 where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == 0 && commslabMob.OPERATOR_CODE == serviceprovider
        //                                 select new
        //                                 {
        //                                     SLN = commslabMob.SLN,
        //                                     commPer = commslabMob.COMM_PERCENTAGE,
        //                                     commType = commslabMob.COMMISSION_TYPE
        //                                 }).FirstOrDefault();
        //                decimal WL_Com = decimal.Parse(MerchantComm.commPer.ToString()) + decimal.Parse(DistributorComm.commPer.ToString()) + decimal.Parse(SuperComm.commPer.ToString());
        //                //decimal WTLGapComm = decimal.Parse(WhiteComm.commPer.ToString()) - decimal.Parse(SuperComm.commPer.ToString());
        //                decimal WTLGapComm = decimal.Parse(WhiteComm.commPer.ToString()) - WL_Com;
        //                //decimal WTLGapCommAmt = (Trans_Amt * WTLGapComm) / 100;
        //                #region White level Commission payment Structure
        //                var tbl_accountWhiteLevel = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == WHT_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
        //                if (tbl_accountWhiteLevel != null)
        //                {
        //                    var WLmembtype = (from mm in db.TBL_MASTER_MEMBER
        //                                      join
        //                                          rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
        //                                      where mm.MEM_ID == WHT_MEM_ID
        //                                      select new
        //                                      {
        //                                          RoleId = mm.MEMBER_ROLE,
        //                                          roleName = rm.ROLE_NAME,
        //                                          Amount = mm.BALANCE
        //                                      }).FirstOrDefault();
        //                    decimal WLClosingAmt = tbl_accountWhiteLevel.CLOSING;
        //                    //decimal SubAmt = ClosingAmt - Trans_Amt;           
        //                    // GST And TDS Calculation for WL 
        //                    //decimal WTLGapCommAmt = (Trans_Amt * WTLGapComm) / 100;
        //                    decimal ELE_WTL_Gap_CommAmt = 0;
        //                    decimal ELE_WL_TDSCal = 0;
        //                    decimal WTLGapCommAmt = 0;
        //                    decimal ELE_WL_GSTCAlculation = 0;
        //                    if (WhiteComm.commType == "FIXED")
        //                    {
        //                        ELE_WTL_Gap_CommAmt = WTLGapComm;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                    
        //                            ELE_WL_TDSCal = decimal.Round(((ELE_WTL_Gap_CommAmt * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                ELE_WL_GSTCAlculation = decimal.Round(((ELE_WTL_Gap_CommAmt * gst_VAlue) / 100), 2);
        //                                WTLGapCommAmt = ELE_WTL_Gap_CommAmt + ELE_WL_GSTCAlculation - ELE_WL_TDSCal;
        //                            }
        //                            else
        //                            {
        //                                ELE_WL_GSTCAlculation =0;
        //                                WTLGapCommAmt = ELE_WTL_Gap_CommAmt + ELE_WL_GSTCAlculation - ELE_WL_TDSCal;
        //                            }

        //                        }
        //                        else
        //                        {                                    
        //                            ELE_WL_TDSCal = decimal.Round(((ELE_WTL_Gap_CommAmt * GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                ELE_WL_GSTCAlculation = decimal.Round(((ELE_WTL_Gap_CommAmt * gst_VAlue)));
        //                                WTLGapCommAmt = ELE_WTL_Gap_CommAmt + ELE_WL_GSTCAlculation - ELE_WL_TDSCal;
        //                            }
        //                            else
        //                            {
        //                                ELE_WL_GSTCAlculation = 0;
        //                                WTLGapCommAmt = ELE_WTL_Gap_CommAmt + ELE_WL_GSTCAlculation - ELE_WL_TDSCal;
        //                            }

        //                        }
        //                    }
        //                    else
        //                    {
        //                        ELE_WTL_Gap_CommAmt = (Trans_Amt * WTLGapComm) / 100;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                   
        //                            ELE_WL_TDSCal = decimal.Round(((ELE_WTL_Gap_CommAmt * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                ELE_WL_GSTCAlculation = decimal.Round(((ELE_WTL_Gap_CommAmt * gst_VAlue) / 100), 2);
        //                                WTLGapCommAmt = ELE_WTL_Gap_CommAmt + ELE_WL_GSTCAlculation - ELE_WL_TDSCal;
        //                            }
        //                            else
        //                            {
        //                                ELE_WL_GSTCAlculation = 0;
        //                                WTLGapCommAmt = ELE_WTL_Gap_CommAmt + ELE_WL_GSTCAlculation - ELE_WL_TDSCal;
        //                            }
        //                        }
        //                        else
        //                        {                                    
        //                            ELE_WL_TDSCal = decimal.Round(((ELE_WTL_Gap_CommAmt * GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                ELE_WL_GSTCAlculation = decimal.Round(((ELE_WTL_Gap_CommAmt * gst_VAlue)));
        //                                WTLGapCommAmt = ELE_WTL_Gap_CommAmt + ELE_WL_GSTCAlculation - ELE_WL_TDSCal;
        //                            }
        //                            else
        //                            {
        //                                ELE_WL_GSTCAlculation =0;
        //                                WTLGapCommAmt = ELE_WTL_Gap_CommAmt + ELE_WL_GSTCAlculation - ELE_WL_TDSCal;
        //                            }

        //                        }                                
        //                    }
        //                    decimal CommWLAddClosing = tbl_accountWhiteLevel.CLOSING + WTLGapCommAmt;
        //                    long WL_ID = long.Parse(WHT_MEM_ID.ToString());
        //                    TBL_ACCOUNTS objWLCommPer = new TBL_ACCOUNTS()
        //                    {
        //                        API_ID = 0,
        //                        MEM_ID = WL_ID,
        //                        MEMBER_TYPE = WLmembtype.roleName,
        //                        TRANSACTION_TYPE = rechargeType,
        //                        TRANSACTION_DATE = System.DateTime.Now,
        //                        TRANSACTION_TIME = DateTime.Now,
        //                        DR_CR = "CR",
        //                        AMOUNT = Trans_Amt,
        //                        NARRATION = "Commission",
        //                        OPENING = WLClosingAmt,
        //                        CLOSING = CommWLAddClosing,
        //                        REC_NO = 0,
        //                        COMM_AMT = WTLGapCommAmt,
        //                        GST = (float)ELE_WL_GSTCAlculation,
        //                        TDS = (float)ELE_WL_TDSCal,
        //                        IPAddress = IpAddress,
        //                        TDS_PERCENTAGE = taxMaster.TAX_VALUE,
        //                        GST_PERCENTAGE = GST_Master.TAX_VALUE,
        //                        WHITELEVEL_ID = (long)WHT_MEM_ID,
        //                        SUPER_ID = (long)SUP_MEM_ID,
        //                        DISTRIBUTOR_ID = (long)DIS_MEM_ID
        //                    };
        //                    db.TBL_ACCOUNTS.Add(objWLCommPer);
        //                    db.SaveChanges();
        //                }
        //                #endregion
        //                ContextTransaction.Commit();
        //                return true;
        //            }
        //            else if (status == "DMR")
        //            {
        //                decimal Super_Com_pr = 0;
        //                decimal Distributor_Com_pr = 0;
        //                decimal Merchant_Com_pr = 0;
        //                var MerchantComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
        //                                    join commslabMob in db.TBL_COMM_SLAB_DMR_PAYMENT
        //                                    on detailscom.SLN equals commslabMob.SLAB_ID
        //                                    //join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
        //                                    where  detailscom.SLAB_TYPE == 3 && commslabMob.MERCHANT_ROLE_ID == 5 && commslabMob.DMT_TYPE == "DOMESTIC"
        //                                    select new
        //                                    {
        //                                        SLN = commslabMob.SLN,
        //                                        Slab_ID = commslabMob.SLAB_ID,
        //                                        Slab_From = commslabMob.SLAB_FROM,
        //                                        Slab_To = commslabMob.SLAB_TO,
        //                                        commPer = commslabMob.MERCHANT_COM_PER,
        //                                        commType=commslabMob.COMMISSION_TYPE
        //                                    }).ToList();

        //                //var MerchantComm11 = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
        //                //                    join commslabMob in db.TBL_COMM_SLAB_DMR_PAYMENT
        //                //                    on detailscom.DMR_SLAB equals commslabMob.SLAB_ID
        //                //                    where detailscom.WHITE_LEVEL_ID == DIS_MEM_ID && detailscom.INTRODUCER_ID == Mem_ID && commslabMob.DMT_TYPE == "DOMESTIC"
        //                //                    select new
        //                //                    {
        //                //                        SLN = commslabMob.SLN,
        //                //                        Slab_ID = commslabMob.SLAB_ID,
        //                //                        Slab_From = commslabMob.SLAB_FROM,
        //                //                        Slab_To = commslabMob.SLAB_TO,
        //                //                        commPer = commslabMob.COMM_PERCENTAGE,

        //                //                    }).ToList();
        //                decimal commamt = 0;
        //                string CommTypeVal = string.Empty;
        //                foreach (var comslab in MerchantComm)
        //                {
        //                    if (Trans_Amt >= comslab.Slab_From && Trans_Amt <= comslab.Slab_To)
        //                    {
        //                        commamt = comslab.commPer;
        //                        CommTypeVal = comslab.commType;
        //                    }
        //                }
        //                Merchant_Com_pr = commamt;
        //                #region Retailer Commission                        
        //                var membtype = (from mm in db.TBL_MASTER_MEMBER
        //                                join
        //                                    rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
        //                                where mm.MEM_ID == Mem_ID
        //                                select new
        //                                {
        //                                    RoleId = mm.MEMBER_ROLE,
        //                                    roleName = rm.ROLE_NAME,
        //                                    Amount = mm.BALANCE
        //                                }).FirstOrDefault();
        //                var tbl_account = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == Mem_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
        //                if (tbl_account != null)
        //                {
        //                    decimal ClosingAmt = tbl_account.CLOSING;
        //                    decimal SubAmt = ClosingAmt - Trans_Amt;
        //                    TBL_ACCOUNTS objaccnt = new TBL_ACCOUNTS()
        //                    {
        //                        API_ID = 0,
        //                        MEM_ID = Mem_ID,
        //                        MEMBER_TYPE = membtype.roleName,
        //                        TRANSACTION_TYPE = rechargeType,
        //                        TRANSACTION_DATE = System.DateTime.Now,
        //                        TRANSACTION_TIME = DateTime.Now,
        //                        DR_CR = "DR",
        //                        AMOUNT = Trans_Amt,
        //                        NARRATION = "DMT",
        //                        OPENING = ClosingAmt,
        //                        CLOSING = SubAmt,
        //                        REC_NO = 0,
        //                        COMM_AMT = 0,
        //                        GST = 0,
        //                        TDS = 0,
        //                        IPAddress = IpAddress,
        //                        TDS_PERCENTAGE = 0,
        //                        GST_PERCENTAGE = 0,
        //                        WHITELEVEL_ID = (long)WHT_MEM_ID,
        //                        SUPER_ID = (long)SUP_MEM_ID,
        //                        DISTRIBUTOR_ID = (long)DIS_MEM_ID
        //                    };
        //                    db.TBL_ACCOUNTS.Add(objaccnt);
        //                    db.SaveChanges();
        //                    //decimal getPer = (Trans_Amt * MerchantComm.commPer) / 100;
        //                    decimal getPer = 0;

        //                    //decimal CommAddClosing = SubAmt - commamt;
        //                    //// DMR Commission GST Addition and TDS Subtraction
        //                    decimal getPer_Val = 0;
        //                    //float FloatgetPer_Val = float.Parse(getPer_Val.ToString());
        //                    // Get TDS and GST
        //                    decimal RetailerTDS_AmtValue = 0;                            
        //                    decimal GStCal = 0;
        //                    decimal CalculateGST = 0;
        //                    if (CommTypeVal == "FIXED")
        //                    {
        //                        getPer_Val = commamt;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {

        //                            RetailerTDS_AmtValue = decimal.Round(((getPer_Val * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                CalculateGST = decimal.Round(((getPer_Val * gst_VAlue) / 100), 2);
        //                                //getPer = getPer_Val + CalculateGST - RetailerTDS_AmtValue;
        //                                getPer = getPer_Val - RetailerTDS_AmtValue;
        //                            }
        //                            else
        //                            {
        //                                CalculateGST = 0;
        //                                //getPer = getPer_Val + CalculateGST - RetailerTDS_AmtValue;
        //                                getPer = getPer_Val - RetailerTDS_AmtValue;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            RetailerTDS_AmtValue = decimal.Round(((getPer_Val - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                CalculateGST = decimal.Round(((getPer_Val - gst_VAlue)));
        //                                //getPer = getPer_Val + CalculateGST - RetailerTDS_AmtValue;
        //                                getPer = getPer_Val - RetailerTDS_AmtValue;
        //                            }
        //                            else
        //                            {
        //                                CalculateGST = 0;
        //                                //getPer = getPer_Val + CalculateGST - RetailerTDS_AmtValue;
        //                                getPer = getPer_Val - RetailerTDS_AmtValue;
        //                            }

        //                        }
        //                    }
        //                    else
        //                    {
        //                        getPer_Val = (Trans_Amt * commamt) / 100;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {

        //                            RetailerTDS_AmtValue = decimal.Round(((getPer_Val * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                CalculateGST = decimal.Round(((getPer_Val * gst_VAlue) / 100), 2);
        //                                //getPer = getPer_Val + CalculateGST - RetailerTDS_AmtValue;
        //                                getPer = getPer_Val - RetailerTDS_AmtValue;
        //                            }
        //                            else
        //                            {
        //                                CalculateGST = 0;
        //                                //getPer = getPer_Val + CalculateGST - RetailerTDS_AmtValue;
        //                                getPer = getPer_Val - RetailerTDS_AmtValue;
        //                            }

        //                        }
        //                        else
        //                        {
        //                            RetailerTDS_AmtValue = decimal.Round(((getPer_Val - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                CalculateGST = decimal.Round(((getPer_Val - gst_VAlue)));
        //                                //getPer = getPer_Val + CalculateGST - RetailerTDS_AmtValue;
        //                                getPer = getPer_Val - RetailerTDS_AmtValue;
        //                            }
        //                            else
        //                            {
        //                                CalculateGST = 0;
        //                                //getPer = getPer_Val + CalculateGST - RetailerTDS_AmtValue;
        //                                getPer = getPer_Val - RetailerTDS_AmtValue;
        //                            }

        //                        }
        //                        //RetailerTDS_AmtValue = decimal.Round(((getPer_Val * GST_AMount) / 100), 2);
        //                        //getPer = getPer_Val - RetailerTDS_AmtValue;
        //                    }
        //                    //DMR Commission GST Addition and TDS Substration
        //                    decimal CommAddClosing = SubAmt + getPer;
        //                    TBL_ACCOUNTS objCommPer = new TBL_ACCOUNTS()
        //                    {
        //                        API_ID = 0,
        //                        MEM_ID = Mem_ID,
        //                        MEMBER_TYPE = membtype.roleName,
        //                        TRANSACTION_TYPE = rechargeType,
        //                        TRANSACTION_DATE = System.DateTime.Now,
        //                        TRANSACTION_TIME = DateTime.Now,
        //                        DR_CR = "CR",
        //                        AMOUNT = Trans_Amt,
        //                        NARRATION = "Commission",
        //                        OPENING = SubAmt,
        //                        CLOSING = CommAddClosing,
        //                        REC_NO = 0,
        //                        COMM_AMT = commamt,
        //                        GST = (float)CalculateGST,
        //                        TDS = (float)RetailerTDS_AmtValue,
        //                        IPAddress = IpAddress,
        //                        TDS_PERCENTAGE = taxMaster.TAX_VALUE,
        //                        GST_PERCENTAGE = GST_Master.TAX_VALUE,
        //                        WHITELEVEL_ID = (long)WHT_MEM_ID,
        //                        SUPER_ID = (long)SUP_MEM_ID,
        //                        DISTRIBUTOR_ID = (long)DIS_MEM_ID,
        //                        SERVICE_ID = 0
        //                    };
        //                    db.TBL_ACCOUNTS.Add(objCommPer);
        //                    db.SaveChanges();
        //                }
        //                #endregion


        //                var MerchantComm12 = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
        //                                    join commslabMob in db.TBL_COMM_SLAB_DMR_PAYMENT
        //                                    on detailscom.SLN equals commslabMob.SLAB_ID
        //                                    //join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
        //                                    where detailscom.SLAB_TYPE == 3 && commslabMob.MERCHANT_ROLE_ID == 4 && commslabMob.DMT_TYPE == "DOMESTIC"
        //                                    select new
        //                                    {
        //                                        SLN = commslabMob.SLN,
        //                                        Slab_ID = commslabMob.SLAB_ID,
        //                                        Slab_From = commslabMob.SLAB_FROM,
        //                                        Slab_To = commslabMob.SLAB_TO,
        //                                        commPer = commslabMob.MERCHANT_COM_PER,
        //                                        commType = commslabMob.COMMISSION_TYPE
        //                                    }).ToList();

        //                var DistributorComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
        //                                    join commslabMob in db.TBL_COMM_SLAB_DMR_PAYMENT
        //                                    on detailscom.SLN equals commslabMob.SLAB_ID
        //                                    //join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
        //                                    where detailscom.SLAB_TYPE == 3 && commslabMob.DISTRIBUTOR_ROLE_ID == 4 && commslabMob.DMT_TYPE == "DOMESTIC"
        //                                    select new
        //                                    {
        //                                        SLN = commslabMob.SLN,
        //                                        Slab_ID = commslabMob.SLAB_ID,
        //                                        Slab_From = commslabMob.SLAB_FROM,
        //                                        Slab_To = commslabMob.SLAB_TO,
        //                                        commPer = commslabMob.DISTRIBUTOR_COM_PER,
        //                                        commType = commslabMob.COMMISSION_TYPE
        //                                    }).ToList();
        //                //var DistributorComm12 = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
        //                //                       join commslabMob in db.TBL_COMM_SLAB_DMR_PAYMENT
        //                //                       on detailscom.DMR_SLAB equals commslabMob.SLAB_ID
        //                //                       where detailscom.WHITE_LEVEL_ID == SUP_MEM_ID && detailscom.INTRODUCER_ID == DIS_MEM_ID && commslabMob.DMT_TYPE == "DOMESTIC"
        //                //                       select new
        //                //                       {
        //                //                           SLN = commslabMob.SLN,
        //                //                           Slab_ID = commslabMob.SLAB_ID,
        //                //                           Slab_From = commslabMob.SLAB_FROM,
        //                //                           Slab_To = commslabMob.SLAB_TO,
        //                //                           commPer = commslabMob.COMM_PERCENTAGE,
        //                //                       }).ToList();
        //                decimal DistributorDMRComm = 0;
        //                string DistCommType = string.Empty;
        //                foreach (var comslab in DistributorComm)
        //                {
        //                    if (Trans_Amt >= comslab.Slab_From && Trans_Amt <= comslab.Slab_To)
        //                    {
        //                        DistributorDMRComm = comslab.commPer;
        //                        DistCommType = comslab.commType;
        //                    }
        //                }
        //                Distributor_Com_pr = DistributorDMRComm;
        //                decimal DmrDisgapcomm = 0;
        //                DmrDisgapcomm = commamt - DistributorDMRComm;

        //                #region Distributor DMR Commission   
        //                var Dismembtype12 = (from mm in db.TBL_MASTER_MEMBER
        //                                   join
        //                                       rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
        //                                   where mm.MEM_ID == DIS_MEM_ID
        //                                   select new
        //                                   {
        //                                       RoleId = mm.MEMBER_ROLE,
        //                                       roleName = rm.ROLE_NAME,
        //                                       Amount = mm.BALANCE
        //                                   }).FirstOrDefault();

        //                var DIsmembtype = (from mm in db.TBL_MASTER_MEMBER
        //                                   join
        //                                       rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
        //                                   where mm.MEM_ID == DIS_MEM_ID
        //                                   select new
        //                                   {
        //                                       RoleId = mm.MEMBER_ROLE,
        //                                       roleName = rm.ROLE_NAME,
        //                                       Amount = mm.BALANCE
        //                                   }).FirstOrDefault();
        //                var tbl_accountDMRDis = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == DIS_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
        //                if (tbl_accountDMRDis != null)
        //                {
        //                    decimal DMRDISClosingAmt = tbl_accountDMRDis.CLOSING;
        //                    //decimal DMRDISSubAmt = DMRDISClosingAmt - Trans_Amt;

        //                    decimal getPer = 0;

        //                    decimal DisGapComm = 0;
        //                    //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;
        //                    decimal DisGapCommAmt_Val = 0;
        //                    decimal DisGapCommAmt_TDS = 0;
        //                    decimal DisGapCommAmt = 0;

        //                    decimal DIS_GST_Amt = 0;
        //                    decimal Dis_Calculate_GST = 0;


        //                    if (DistCommType == "FIXED")
        //                    {
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {
        //                            DisGapComm = decimal.Parse(DistributorDMRComm.ToString());
        //                            DisGapCommAmt_Val = DisGapComm;
        //                            DisGapCommAmt_TDS = decimal.Round(((DisGapCommAmt_Val * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                Dis_Calculate_GST = decimal.Round(((DisGapCommAmt_Val * gst_VAlue) / 100), 2);
        //                                //DisGapCommAmt = DisGapCommAmt_Val + Dis_Calculate_GST - DisGapCommAmt_TDS;
        //                                DisGapCommAmt = DisGapCommAmt_Val - DisGapCommAmt_TDS;
        //                            }
        //                            else
        //                            {
        //                                Dis_Calculate_GST = 0;
        //                                //DisGapCommAmt = DisGapCommAmt_Val + Dis_Calculate_GST - DisGapCommAmt_TDS;
        //                                DisGapCommAmt = DisGapCommAmt_Val - DisGapCommAmt_TDS;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            DisGapComm = decimal.Parse(DistributorDMRComm.ToString());
        //                            DisGapCommAmt_Val = DisGapComm;
        //                            DisGapCommAmt_TDS = decimal.Round(((DisGapCommAmt_Val - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                Dis_Calculate_GST = decimal.Round(((DisGapCommAmt_Val - gst_VAlue)));
        //                                //DisGapCommAmt = DisGapCommAmt_Val + Dis_Calculate_GST - DisGapCommAmt_TDS;
        //                                DisGapCommAmt = DisGapCommAmt_Val - DisGapCommAmt_TDS;
        //                            }
        //                            else
        //                            {
        //                                Dis_Calculate_GST = 0;
        //                                //DisGapCommAmt = DisGapCommAmt_Val + Dis_Calculate_GST - DisGapCommAmt_TDS;
        //                                DisGapCommAmt = DisGapCommAmt_Val - DisGapCommAmt_TDS;
        //                            }

        //                        }
        //                    }
        //                    else
        //                    {
        //                        DisGapComm = decimal.Parse(DistributorDMRComm.ToString());
        //                        //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;
        //                        DisGapCommAmt_Val = (Trans_Amt * DisGapComm) / 100;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {
        //                            DisGapCommAmt_TDS = decimal.Round(((DisGapCommAmt_Val * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                Dis_Calculate_GST = decimal.Round(((DisGapCommAmt_Val * gst_VAlue) / 100), 2);
        //                                //DisGapCommAmt = DisGapCommAmt_Val + Dis_Calculate_GST - DisGapCommAmt_TDS;
        //                                DisGapCommAmt = DisGapCommAmt_Val - DisGapCommAmt_TDS;
        //                            }
        //                            else
        //                            {
        //                                Dis_Calculate_GST = 0;
        //                                //DisGapCommAmt = DisGapCommAmt_Val + Dis_Calculate_GST - DisGapCommAmt_TDS;
        //                                DisGapCommAmt = DisGapCommAmt_Val - DisGapCommAmt_TDS;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            DisGapCommAmt_TDS = decimal.Round(((DisGapCommAmt_Val - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                Dis_Calculate_GST = decimal.Round(((DisGapCommAmt_Val - gst_VAlue)));
        //                                //DisGapCommAmt = DisGapCommAmt_Val + Dis_Calculate_GST - DisGapCommAmt_TDS;
        //                                DisGapCommAmt = DisGapCommAmt_Val - DisGapCommAmt_TDS;
        //                            }
        //                            else
        //                            {
        //                                Dis_Calculate_GST = 0;
        //                                //DisGapCommAmt = DisGapCommAmt_Val + Dis_Calculate_GST - DisGapCommAmt_TDS;
        //                                DisGapCommAmt = DisGapCommAmt_Val - DisGapCommAmt_TDS;
        //                            }
        //                        }

        //                    }
        //                    decimal CommDisAddClosing = DMRDISClosingAmt + DisGapCommAmt;
        //                    long Dis_ID = long.Parse(DIS_MEM_ID.ToString());


        //                    //decimal DMRDisCommAddClosing = DMRDISClosingAmt - DmrDisgapcomm;
        //                    long dis_idval = long.Parse(DIS_MEM_ID.ToString());
        //                    TBL_ACCOUNTS objCommPerDis = new TBL_ACCOUNTS()
        //                    {
        //                        API_ID = 0,
        //                        MEM_ID = dis_idval,
        //                        MEMBER_TYPE = DIsmembtype.roleName,
        //                        TRANSACTION_TYPE = rechargeType,
        //                        TRANSACTION_DATE = System.DateTime.Now,
        //                        TRANSACTION_TIME = DateTime.Now,
        //                        DR_CR = "CR",
        //                        AMOUNT = Trans_Amt,
        //                        NARRATION = "Commission",
        //                        OPENING = DMRDISClosingAmt,
        //                        CLOSING = CommDisAddClosing,
        //                        REC_NO = 0,
        //                        COMM_AMT = DisGapCommAmt,
        //                        GST = (float)Dis_Calculate_GST,
        //                        TDS = (float)DisGapCommAmt_TDS,
        //                        IPAddress = IpAddress,
        //                        TDS_PERCENTAGE = taxMaster.TAX_VALUE,
        //                        GST_PERCENTAGE = GST_Master.TAX_VALUE,
        //                        WHITELEVEL_ID = (long)WHT_MEM_ID,
        //                        SUPER_ID = (long)SUP_MEM_ID,
        //                        DISTRIBUTOR_ID = (long)DIS_MEM_ID,
        //                        SERVICE_ID = 0
        //                    };
        //                    db.TBL_ACCOUNTS.Add(objCommPerDis);
        //                    db.SaveChanges();
        //                }
        //                #endregion


        //                var SuperComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
        //                                       join commslabMob in db.TBL_COMM_SLAB_DMR_PAYMENT
        //                                       on detailscom.SLN equals commslabMob.SLAB_ID
        //                                       //join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
        //                                       where detailscom.SLAB_TYPE == 3 && commslabMob.SUPER_ROLE_ID == 3 && commslabMob.DMT_TYPE == "DOMESTIC"
        //                                       select new
        //                                       {
        //                                           SLN = commslabMob.SLN,
        //                                           Slab_ID = commslabMob.SLAB_ID,
        //                                           Slab_From = commslabMob.SLAB_FROM,
        //                                           Slab_To = commslabMob.SLAB_TO,
        //                                           commPer = commslabMob.SUPER_COM_PER,
        //                                           commType = commslabMob.COMMISSION_TYPE
        //                                       }).ToList();

        //                //var SuperComm_12 = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
        //                //                 join commslabMob in db.TBL_COMM_SLAB_DMR_PAYMENT
        //                //                  on detailscom.DMR_SLAB equals commslabMob.SLAB_ID
        //                //                 where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == SUP_MEM_ID && commslabMob.DMT_TYPE == "DOMESTIC"
        //                //                 select new
        //                //                 {
        //                //                     SLN = commslabMob.SLN,
        //                //                     Slab_ID = commslabMob.SLAB_ID,
        //                //                     Slab_From = commslabMob.SLAB_FROM,
        //                //                     Slab_To = commslabMob.SLAB_TO,
        //                //                     commPer = commslabMob.COMM_PERCENTAGE,
        //                //                 }).ToList();
        //                decimal SuperDMRComm = 0;
        //                string SuperCommType = string.Empty;
        //                foreach (var comslab in SuperComm)
        //                {
        //                    if (Trans_Amt >= comslab.Slab_From && Trans_Amt <= comslab.Slab_To)
        //                    {
        //                        SuperDMRComm = comslab.commPer;
        //                        SuperCommType = comslab.commType;
        //                    }
        //                }
        //                Super_Com_pr = SuperDMRComm;
        //                decimal DmrSUPgapcomm = 0;
        //                DmrSUPgapcomm = DistributorDMRComm - SuperDMRComm;
        //                #region Super DMR Commission                        
        //                var Supmembtype = (from mm in db.TBL_MASTER_MEMBER
        //                                   join
        //                                       rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
        //                                   where mm.MEM_ID == SUP_MEM_ID
        //                                   select new
        //                                   {
        //                                       RoleId = mm.MEMBER_ROLE,
        //                                       roleName = rm.ROLE_NAME,
        //                                       Amount = mm.BALANCE
        //                                   }).FirstOrDefault();
        //                var tbl_accountSupDis = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == SUP_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
        //                if (tbl_accountSupDis != null)
        //                {
        //                    decimal DMRSupClosingAmt = tbl_accountSupDis.CLOSING;
        //                    //decimal DMRDISSubAmt = DMRDISClosingAmt - Trans_Amt;

        //                    decimal getPer = 0;
        //                    // Calculate GST Value and TDS
        //                    decimal SupClosingAmt = tbl_accountSupDis.CLOSING;

        //                    decimal SupGapComm = 0;
        //                    //decimal SupGapCommAmt = (Trans_Amt * SupGapComm) / 100;
        //                    // GST and TDS Calculation for Super
        //                    decimal SupGapCommAmt_val = 0;
        //                    decimal Sp_TDSAmt = 0;
        //                    decimal SupGapCommAmt = 0;
        //                    decimal SUP_GstAmt = 0;
        //                    decimal Sup_CalculateGST = 0;
        //                    if (SuperCommType == "FIXED")
        //                    {
        //                        SupGapComm = decimal.Parse(SuperDMRComm.ToString());
        //                        //decimal SupGapCommAmt = (Trans_Amt * SupGapComm) / 100;
        //                        // GST and TDS Calculation for Super
        //                        SupGapCommAmt_val = SupGapComm;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {
        //                            Sp_TDSAmt = decimal.Round(((SupGapCommAmt_val * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                Sup_CalculateGST = decimal.Round(((SupGapCommAmt_val * gst_VAlue) / 100), 2);
        //                                //SupGapCommAmt = SupGapCommAmt_val + Sup_CalculateGST - Sp_TDSAmt;
        //                                SupGapCommAmt = SupGapCommAmt_val - Sp_TDSAmt;
        //                            }
        //                            else
        //                            {
        //                                Sup_CalculateGST = 0;
        //                                //SupGapCommAmt = SupGapCommAmt_val + Sup_CalculateGST - Sp_TDSAmt;
        //                                SupGapCommAmt = SupGapCommAmt_val - Sp_TDSAmt;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            Sp_TDSAmt = decimal.Round(((SupGapCommAmt_val - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                Sup_CalculateGST = decimal.Round(((SupGapCommAmt_val - gst_VAlue)));
        //                                //SupGapCommAmt = SupGapCommAmt_val + Sup_CalculateGST - Sp_TDSAmt;
        //                                SupGapCommAmt = SupGapCommAmt_val - Sp_TDSAmt;
        //                            }
        //                            else
        //                            {
        //                                Sup_CalculateGST = 0;
        //                                //SupGapCommAmt = SupGapCommAmt_val + Sup_CalculateGST - Sp_TDSAmt;
        //                                SupGapCommAmt = SupGapCommAmt_val - Sp_TDSAmt;
        //                            }

        //                        }
        //                    }
        //                    else
        //                    {
        //                        SupGapComm = decimal.Parse(SuperDMRComm.ToString());
        //                        //decimal SupGapCommAmt = (Trans_Amt * SupGapComm) / 100;
        //                        // GST and TDS Calculation for Super
        //                        SupGapCommAmt_val = (Trans_Amt * SupGapComm) / 100;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {
        //                            Sp_TDSAmt = decimal.Round(((SupGapCommAmt_val * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                Sup_CalculateGST = decimal.Round(((SupGapCommAmt_val * gst_VAlue) / 100), 2);
        //                                //SupGapCommAmt = SupGapCommAmt_val + Sup_CalculateGST - Sp_TDSAmt;
        //                                SupGapCommAmt = SupGapCommAmt_val - Sp_TDSAmt;
        //                            }
        //                            else
        //                            {
        //                                Sup_CalculateGST = 0;
        //                                //SupGapCommAmt = SupGapCommAmt_val + Sup_CalculateGST - Sp_TDSAmt;
        //                                SupGapCommAmt = SupGapCommAmt_val - Sp_TDSAmt;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            Sp_TDSAmt = decimal.Round(((SupGapCommAmt_val - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                Sup_CalculateGST = decimal.Round(((SupGapCommAmt_val - gst_VAlue)));
        //                                //SupGapCommAmt = SupGapCommAmt_val + Sup_CalculateGST - Sp_TDSAmt;
        //                                SupGapCommAmt = SupGapCommAmt_val - Sp_TDSAmt;
        //                            }
        //                            else
        //                            {
        //                                Sup_CalculateGST = 0;
        //                                //SupGapCommAmt = SupGapCommAmt_val + Sup_CalculateGST - Sp_TDSAmt;
        //                                SupGapCommAmt = SupGapCommAmt_val - Sp_TDSAmt;
        //                            }
        //                        }
        //                    }

        //                    decimal CommSupAddClosing = tbl_accountSupDis.CLOSING + SupGapCommAmt;
        //                    long Sup_ID = long.Parse(SUP_MEM_ID.ToString());

        //                    // Calculate GST and TDS



        //                    //decimal DMRSUPCommAddClosing = DMRSupClosingAmt - DmrSUPgapcomm;
        //                    long sup_idval = long.Parse(SUP_MEM_ID.ToString());
        //                    TBL_ACCOUNTS objCommPerSup = new TBL_ACCOUNTS()
        //                    {
        //                        API_ID = 0,
        //                        MEM_ID = sup_idval,
        //                        MEMBER_TYPE = Supmembtype.roleName,
        //                        TRANSACTION_TYPE = rechargeType,
        //                        TRANSACTION_DATE = System.DateTime.Now,
        //                        TRANSACTION_TIME = DateTime.Now,
        //                        DR_CR = "CR",
        //                        AMOUNT = Trans_Amt,
        //                        NARRATION = "Commission",
        //                        OPENING = DMRSupClosingAmt,
        //                        CLOSING = CommSupAddClosing,
        //                        REC_NO = 0,
        //                        COMM_AMT = SupGapCommAmt,
        //                        GST = (float)Sup_CalculateGST,
        //                        TDS = (float)Sp_TDSAmt,
        //                        IPAddress = IpAddress,
        //                        TDS_PERCENTAGE = taxMaster.TAX_VALUE,
        //                        GST_PERCENTAGE = GST_Master.TAX_VALUE,
        //                        WHITELEVEL_ID = (long)WHT_MEM_ID,
        //                        SUPER_ID = (long)SUP_MEM_ID,
        //                        DISTRIBUTOR_ID = (long)DIS_MEM_ID,
        //                        SERVICE_ID = 0
        //                    };
        //                    db.TBL_ACCOUNTS.Add(objCommPerSup);
        //                    db.SaveChanges();
        //                }
        //                #endregion


        //                var WhiteComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
        //                                 join commslabMob in db.TBL_COMM_SLAB_DMR_PAYMENT
        //                                 on detailscom.SLN equals commslabMob.SLAB_ID
        //                                 join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
        //                                 where detailscom.SLAB_TYPE == 3 && mem.MEMBER_ROLE == 1 && commslabMob.DMT_TYPE == "DOMESTIC"
        //                                 select new
        //                                 {
        //                                     SLN = commslabMob.SLN,
        //                                     Slab_ID = commslabMob.SLAB_ID,
        //                                     Slab_From = commslabMob.SLAB_FROM,
        //                                     Slab_To = commslabMob.SLAB_TO,
        //                                     commPer = commslabMob.COMM_PERCENTAGE
        //                                 }).ToList();


        //                //var WhiteComm_12 = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
        //                //                 join commslabMob in db.TBL_COMM_SLAB_DMR_PAYMENT
        //                //                  on detailscom.DMR_SLAB equals commslabMob.SLAB_ID
        //                //                 where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == 0 && commslabMob.DMT_TYPE == "DOMESTIC"
        //                //                 select new
        //                //                 {
        //                //                     SLN = commslabMob.SLN,
        //                //                     Slab_ID = commslabMob.SLAB_ID,
        //                //                     Slab_From = commslabMob.SLAB_FROM,
        //                //                     Slab_To = commslabMob.SLAB_TO,
        //                //                     commPer = commslabMob.COMM_PERCENTAGE,
        //                //                 }).ToList();

        //                decimal WLComm = decimal.Parse(Super_Com_pr.ToString()) + decimal.Parse(Distributor_Com_pr.ToString()) + decimal.Parse(Merchant_Com_pr.ToString());

        //                var WLcommissionList = db.TBL_COMM_SLAB_DMR_PAYMENT.Where(x => x.MEM_ID == 0 && x.DMT_TYPE == "DOMESTIC" && x.SUPER_ROLE_ID == 0 && x.DISTRIBUTOR_ROLE_ID == 0 && x.MERCHANT_ROLE_ID == 0).FirstOrDefault();

        //                decimal WTLGapComm = decimal.Parse(WLcommissionList.COMM_PERCENTAGE.ToString()) - WLComm;

        //                decimal WhiteDMRComm = 0;
        //                string WhiteLevelCommType = "FIXED";

        //                decimal DmrWhitegapcomm = 0;
        //                DmrWhitegapcomm = SuperDMRComm - WhiteDMRComm;
        //                #region White DMR Commission                        
        //                var Whitemembtype = (from mm in db.TBL_MASTER_MEMBER
        //                                     join
        //                                         rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
        //                                     where mm.MEM_ID == WHT_MEM_ID
        //                                     select new
        //                                     {
        //                                         RoleId = mm.MEMBER_ROLE,
        //                                         roleName = rm.ROLE_NAME,
        //                                         Amount = mm.BALANCE
        //                                     }).FirstOrDefault();
        //                var tbl_accountWhiteDis = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == WHT_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
        //                if (tbl_accountWhiteDis != null)
        //                {
        //                    decimal DMRWgiteClosingAmt = tbl_accountWhiteDis.CLOSING;
        //                    //decimal DMRDISSubAmt = DMRDISClosingAmt - Trans_Amt;

        //                    decimal getPer = 0;
        //                    //  WL Commission GST ANd TDS Calculation

        //                    decimal WTLGapCommAmt_AMt = 0;
        //                    decimal WL_TDSCalculation = 0;
        //                    decimal WTLGapCommAmt = 0;

        //                    decimal WL_GST_AMT = 0;
        //                    decimal WL_CalculateGST_AMT = 0;

        //                    if (WhiteLevelCommType == "FIXED")
        //                    {
        //                        WTLGapCommAmt_AMt = WTLGapComm;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {
        //                            WL_TDSCalculation = decimal.Round(((WTLGapCommAmt_AMt * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                WL_CalculateGST_AMT = decimal.Round(((WTLGapCommAmt_AMt * gst_VAlue) / 100), 2);
        //                                //WTLGapCommAmt = WTLGapCommAmt_AMt + WL_CalculateGST_AMT - WL_TDSCalculation;
        //                                WTLGapCommAmt = WTLGapCommAmt_AMt - WL_TDSCalculation;
        //                            }
        //                            else
        //                            {
        //                                WL_CalculateGST_AMT = 0;
        //                                //WTLGapCommAmt = WTLGapCommAmt_AMt + WL_CalculateGST_AMT - WL_TDSCalculation;
        //                                WTLGapCommAmt = WTLGapCommAmt_AMt - WL_TDSCalculation;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            WL_TDSCalculation = decimal.Round(((WTLGapCommAmt_AMt - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                WL_CalculateGST_AMT = decimal.Round(((WTLGapCommAmt_AMt - gst_VAlue)));
        //                                //WTLGapCommAmt = WTLGapCommAmt_AMt + WL_CalculateGST_AMT - WL_TDSCalculation;
        //                                WTLGapCommAmt = WTLGapCommAmt_AMt - WL_TDSCalculation;
        //                            }
        //                            else
        //                            {
        //                                WL_CalculateGST_AMT = 0;
        //                                //WTLGapCommAmt = WTLGapCommAmt_AMt + WL_CalculateGST_AMT - WL_TDSCalculation;
        //                                WTLGapCommAmt = WTLGapCommAmt_AMt - WL_TDSCalculation;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        WTLGapCommAmt_AMt = (Trans_Amt * WTLGapComm) / 100;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {
        //                            WL_TDSCalculation = decimal.Round(((WTLGapCommAmt_AMt * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                WL_CalculateGST_AMT = decimal.Round(((WTLGapCommAmt_AMt * gst_VAlue) / 100), 2);
        //                                //WTLGapCommAmt = WTLGapCommAmt_AMt + WL_CalculateGST_AMT - WL_TDSCalculation;
        //                                WTLGapCommAmt = WTLGapCommAmt_AMt - WL_TDSCalculation;
        //                            }
        //                            else
        //                            {
        //                                WL_CalculateGST_AMT = 0;
        //                                //WTLGapCommAmt = WTLGapCommAmt_AMt + WL_CalculateGST_AMT - WL_TDSCalculation;
        //                                WTLGapCommAmt = WTLGapCommAmt_AMt - WL_TDSCalculation;
        //                            }

        //                        }
        //                        else
        //                        {
        //                            WL_TDSCalculation = decimal.Round(((WTLGapCommAmt_AMt - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                WL_CalculateGST_AMT = decimal.Round(((WTLGapCommAmt_AMt - gst_VAlue)));
        //                                //WTLGapCommAmt = WTLGapCommAmt_AMt + WL_CalculateGST_AMT - WL_TDSCalculation;
        //                                WTLGapCommAmt = WTLGapCommAmt_AMt - WL_TDSCalculation;
        //                            }
        //                            else
        //                            {
        //                                WL_CalculateGST_AMT = 0;
        //                                //WTLGapCommAmt = WTLGapCommAmt_AMt + WL_CalculateGST_AMT - WL_TDSCalculation;
        //                                WTLGapCommAmt = WTLGapCommAmt_AMt - WL_TDSCalculation;
        //                            }

        //                        }

        //                    }
        //                    //decimal WTLGapCommAmt = (Trans_Amt * WTLGapComm) / 100;
        //                    decimal CommWLAddClosing = tbl_accountWhiteDis.CLOSING + WTLGapCommAmt;
        //                    long WL_ID = long.Parse(WHT_MEM_ID.ToString());

        //                    // WL Commission GST and TDS Calculation
        //                    //decimal DMRWhiteCommAddClosing = DMRWgiteClosingAmt - DmrWhitegapcomm;
        //                    long Wht_idval = long.Parse(WHT_MEM_ID.ToString());
        //                    TBL_ACCOUNTS objCommPerWL = new TBL_ACCOUNTS()
        //                    {
        //                        API_ID = 0,
        //                        MEM_ID = Wht_idval,
        //                        MEMBER_TYPE = Whitemembtype.roleName,
        //                        TRANSACTION_TYPE = rechargeType,
        //                        TRANSACTION_DATE = System.DateTime.Now,
        //                        TRANSACTION_TIME = DateTime.Now,
        //                        DR_CR = "CR",
        //                        AMOUNT = Trans_Amt,
        //                        NARRATION = "Commission",
        //                        OPENING = DMRWgiteClosingAmt,
        //                        CLOSING = CommWLAddClosing,
        //                        REC_NO = 0,
        //                        COMM_AMT = WTLGapCommAmt,
        //                        GST = (float)WL_CalculateGST_AMT,
        //                        TDS = (float)WL_TDSCalculation,
        //                        IPAddress = IpAddress,
        //                        TDS_PERCENTAGE = taxMaster.TAX_VALUE,
        //                        GST_PERCENTAGE = GST_Master.TAX_VALUE,
        //                        WHITELEVEL_ID = (long)WHT_MEM_ID,
        //                        SUPER_ID = (long)SUP_MEM_ID,
        //                        DISTRIBUTOR_ID = (long)DIS_MEM_ID,
        //                        SERVICE_ID = 0
        //                    };
        //                    db.TBL_ACCOUNTS.Add(objCommPerWL);
        //                    db.SaveChanges();
        //                }
        //                #endregion
        //                ContextTransaction.Commit();
        //                return true;
        //            }
        //            else if (status == "LANDLINE")
        //            {
        //                var MerchantComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
        //                                    join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
        //                                    on detailscom.SLN equals commslabMob.SLAB_ID
        //                                    join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
        //                                    where commslabMob.OPERATOR_CODE == serviceprovider && detailscom.SLAB_TYPE == 1 && mem.MEMBER_ROLE == 5
        //                                    select new
        //                                    {
        //                                        SLN = commslabMob.SLN,
        //                                        commPer = commslabMob.MERCHANT_COM_PER,
        //                                        commType = commslabMob.COMMISSION_TYPE
        //                                    }).FirstOrDefault();

        //                //var MerchantComm2 = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
        //                //                    join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
        //                //                    on detailscom.RECHARGE_SLAB equals commslabMob.SLAB_ID
        //                //                    where detailscom.WHITE_LEVEL_ID == DIS_MEM_ID && detailscom.INTRODUCER_ID == Mem_ID && commslabMob.OPERATOR_CODE == serviceprovider
        //                //                    select new
        //                //                    {
        //                //                        SLN = commslabMob.SLN,
        //                //                        commPer = commslabMob.COMM_PERCENTAGE
        //                //                    }).FirstOrDefault();
        //                //var MerchantComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
        //                //                    join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
        //                //                    on detailscom.RECHARGE_SLAB equals commslabMob.SLAB_ID
        //                //                    where detailscom.WHITE_LEVEL_ID == DIS_MEM_ID && detailscom.INTRODUCER_ID == Mem_ID && commslabMob.OPERATOR_CODE == serviceprovider
        //                //                    select new
        //                //                    {
        //                //                        SLN = commslabMob.SLN,
        //                //                        commPer = commslabMob.COMM_PERCENTAGE
        //                //                    }).FirstOrDefault();
        //                #region Retailer Commission                        
        //                var membtype = (from mm in db.TBL_MASTER_MEMBER
        //                                join
        //                                    rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
        //                                where mm.MEM_ID == Mem_ID
        //                                select new
        //                                {
        //                                    RoleId = mm.MEMBER_ROLE,
        //                                    roleName = rm.ROLE_NAME,
        //                                    Amount = mm.BALANCE
        //                                }).FirstOrDefault();
        //                var tbl_account = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == Mem_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
        //                if (tbl_account != null)
        //                {
        //                    decimal ClosingAmt = tbl_account.CLOSING;
        //                    decimal SubAmt = ClosingAmt - Trans_Amt;
        //                    TBL_ACCOUNTS objaccnt = new TBL_ACCOUNTS()
        //                    {
        //                        API_ID = 0,
        //                        MEM_ID = Mem_ID,
        //                        MEMBER_TYPE = membtype.roleName,
        //                        TRANSACTION_TYPE = rechargeType,
        //                        TRANSACTION_DATE = System.DateTime.Now,
        //                        TRANSACTION_TIME = DateTime.Now,
        //                        DR_CR = "DR",
        //                        AMOUNT = Trans_Amt,
        //                        NARRATION = "Recharge",
        //                        OPENING = ClosingAmt,
        //                        CLOSING = SubAmt,
        //                        REC_NO = 0,
        //                        COMM_AMT = 0,
        //                        GST = 0,
        //                        TDS = 0,
        //                        IPAddress = IpAddress,
        //                        TDS_PERCENTAGE = 0,
        //                        GST_PERCENTAGE = 0,
        //                        WHITELEVEL_ID = (long)WHT_MEM_ID,
        //                        SUPER_ID = (long)SUP_MEM_ID,
        //                        DISTRIBUTOR_ID = (long)DIS_MEM_ID
        //                    };
        //                    db.TBL_ACCOUNTS.Add(objaccnt);
        //                    db.SaveChanges();

        //                    decimal getPer_Val = 0;
        //                    // Get TDS and GST
        //                    decimal RetailerTDS_AmtValue = 0;
        //                    decimal getPer = 0;
        //                    decimal LandMErGSTCAl = 0;
        //                    if (MerchantComm.commType == "FIXED")
        //                    {
        //                        getPer_Val = MerchantComm.commPer;
        //                        // Get TDS and GST
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {
        //                            RetailerTDS_AmtValue = decimal.Round(((getPer_Val * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                LandMErGSTCAl = decimal.Round(((getPer_Val * gst_VAlue) / 100), 2);
        //                                getPer = getPer_Val + LandMErGSTCAl - RetailerTDS_AmtValue;
        //                            }
        //                            else
        //                            {
        //                                LandMErGSTCAl = 0;
        //                                getPer = getPer_Val + LandMErGSTCAl - RetailerTDS_AmtValue;
        //                            }
        //                        }
        //                        else
        //                        {                                  
        //                            RetailerTDS_AmtValue = decimal.Round(((getPer_Val- GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                LandMErGSTCAl = decimal.Round(((getPer_Val - gst_VAlue)));
        //                                getPer = getPer_Val + LandMErGSTCAl - RetailerTDS_AmtValue;
        //                            }
        //                            else
        //                            {
        //                                LandMErGSTCAl = 0;
        //                                getPer = getPer_Val + LandMErGSTCAl - RetailerTDS_AmtValue;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        getPer_Val = (Trans_Amt * MerchantComm.commPer) / 100;
        //                        // Get TDS and GST
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                   
        //                            RetailerTDS_AmtValue = decimal.Round(((getPer_Val * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                LandMErGSTCAl = decimal.Round(((getPer_Val * gst_VAlue) / 100), 2);
        //                                getPer = getPer_Val + LandMErGSTCAl - RetailerTDS_AmtValue;
        //                            }
        //                            else
        //                            {
        //                                LandMErGSTCAl = 0;
        //                                getPer = getPer_Val + LandMErGSTCAl - RetailerTDS_AmtValue;
        //                            }

        //                        }
        //                        else
        //                        {                                    
        //                            RetailerTDS_AmtValue = decimal.Round(((getPer_Val - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                LandMErGSTCAl = decimal.Round(((getPer_Val - gst_VAlue)));
        //                                getPer = getPer_Val + LandMErGSTCAl - RetailerTDS_AmtValue;
        //                            }
        //                            else
        //                            {
        //                                LandMErGSTCAl =0;
        //                                getPer = getPer_Val + LandMErGSTCAl - RetailerTDS_AmtValue;
        //                            }

        //                        }

        //                    }

        //                    decimal CommAddClosing = SubAmt + getPer;
        //                    TBL_ACCOUNTS objCommPer = new TBL_ACCOUNTS()
        //                    {
        //                        API_ID = 0,
        //                        MEM_ID = Mem_ID,
        //                        MEMBER_TYPE = membtype.roleName,
        //                        TRANSACTION_TYPE = rechargeType,
        //                        TRANSACTION_DATE = System.DateTime.Now,
        //                        TRANSACTION_TIME = DateTime.Now,
        //                        DR_CR = "CR",
        //                        AMOUNT = Trans_Amt,
        //                        NARRATION = "Commission",
        //                        OPENING = SubAmt,
        //                        CLOSING = CommAddClosing,
        //                        REC_NO = 0,
        //                        COMM_AMT = getPer,
        //                        GST = (float)LandMErGSTCAl,
        //                        TDS = (float)RetailerTDS_AmtValue,
        //                        IPAddress = IpAddress,
        //                        TDS_PERCENTAGE = taxMaster.TAX_VALUE,
        //                        GST_PERCENTAGE = GST_Master.TAX_VALUE,
        //                        WHITELEVEL_ID = (long)WHT_MEM_ID,
        //                        SUPER_ID = (long)SUP_MEM_ID,
        //                        DISTRIBUTOR_ID = (long)DIS_MEM_ID
        //                    };
        //                    db.TBL_ACCOUNTS.Add(objCommPer);
        //                    db.SaveChanges();
        //                }
        //                #endregion

        //                var DistributorComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
        //                                       join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
        //                                       on detailscom.SLN equals commslabMob.SLAB_ID
        //                                       join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
        //                                       where commslabMob.OPERATOR_CODE == serviceprovider && detailscom.SLAB_TYPE == 1 && mem.MEMBER_ROLE == 4
        //                                       select new
        //                                       {
        //                                           SLN = commslabMob.SLN,
        //                                           commPer = commslabMob.DISTRIBUTOR_COM_PER,
        //                                           commType = commslabMob.COMMISSION_TYPE
        //                                       }).FirstOrDefault();
        //                //var DistributorComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
        //                //                       join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
        //                //                       on detailscom.RECHARGE_SLAB equals commslabMob.SLAB_ID
        //                //                       where detailscom.WHITE_LEVEL_ID == SUP_MEM_ID && detailscom.INTRODUCER_ID == DIS_MEM_ID && commslabMob.OPERATOR_CODE == serviceprovider
        //                //                       select new
        //                //                       {
        //                //                           SLN = commslabMob.SLN,
        //                //                           commPer = commslabMob.COMM_PERCENTAGE
        //                //                       }).FirstOrDefault();
        //                #region Distributor Commission
        //                //decimal DisGapComm = decimal.Parse(DistributorComm.commPer.ToString()) - decimal.Parse(MerchantComm.commPer.ToString());

        //                var tbl_accountDistributor = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == DIS_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
        //                if (tbl_accountDistributor != null)
        //                {
        //                    var CheckDisCommission = db.TBL_DETAILS_MEMBER_COMMISSION_SLAB.Where(x => x.INTRODUCER_ID == DIS_MEM_ID).FirstOrDefault();
        //                    var Dismembtype = (from mm in db.TBL_MASTER_MEMBER
        //                                       join
        //                                           rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
        //                                       where mm.MEM_ID == DIS_MEM_ID
        //                                       select new
        //                                       {
        //                                           RoleId = mm.MEMBER_ROLE,
        //                                           roleName = rm.ROLE_NAME,
        //                                           Amount = mm.BALANCE
        //                                       }).FirstOrDefault();
        //                    decimal DisClosingAmt = tbl_accountDistributor.CLOSING;
        //                    //decimal SubAmt = ClosingAmt - Trans_Amt;                   
        //                    //decimal DisgetPer = (Trans_Amt * checkDiscomm.CommPercentage) / 100;

        //                    //  Distributor GST and TDS
        //                    //decimal CommDisAddClosing = tbl_accountDistributor.CLOSING + DisGapCommAmt;
        //                    decimal DisGapComm = decimal.Parse(DistributorComm.commPer.ToString());
        //                    //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;
        //                    decimal DisGapCommAmt_Val = 0;
        //                    decimal DisGapCommAmt_TDS = 0;
        //                    decimal DisGapCommAmt = 0;
        //                    decimal LND_DIST_GSTCAl = 0;
        //                    if (DistributorComm.commType == "FIXED")
        //                    {
        //                        DisGapCommAmt_Val = DisGapComm;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {

        //                            DisGapCommAmt_TDS = decimal.Round(((DisGapCommAmt_Val * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                LND_DIST_GSTCAl = decimal.Round(((DisGapCommAmt_Val * gst_VAlue) / 100), 2);
        //                                DisGapCommAmt = DisGapCommAmt_Val + LND_DIST_GSTCAl - DisGapCommAmt_TDS;
        //                            }
        //                            else {
        //                                LND_DIST_GSTCAl = 0;
        //                                DisGapCommAmt = DisGapCommAmt_Val + LND_DIST_GSTCAl - DisGapCommAmt_TDS;
        //                            }

        //                        }
        //                        else
        //                        {                                    
        //                            DisGapCommAmt_TDS = decimal.Round(((DisGapCommAmt_Val - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                LND_DIST_GSTCAl = decimal.Round(((DisGapCommAmt_Val - gst_VAlue)));
        //                                DisGapCommAmt = DisGapCommAmt_Val + LND_DIST_GSTCAl - DisGapCommAmt_TDS;
        //                            }
        //                            else
        //                            {
        //                                LND_DIST_GSTCAl = 0;
        //                                DisGapCommAmt = DisGapCommAmt_Val + LND_DIST_GSTCAl - DisGapCommAmt_TDS;
        //                            }                                   
        //                        }
        //                    }
        //                    else
        //                    {
        //                        DisGapCommAmt_Val = (Trans_Amt * DisGapComm) / 100;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                   
        //                            DisGapCommAmt_TDS = decimal.Round(((DisGapCommAmt_Val * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                LND_DIST_GSTCAl = decimal.Round(((DisGapCommAmt_Val * gst_VAlue) / 100), 2);
        //                                DisGapCommAmt = DisGapCommAmt_Val + LND_DIST_GSTCAl - DisGapCommAmt_TDS;
        //                            }
        //                            else
        //                            {
        //                                LND_DIST_GSTCAl = 0;
        //                                DisGapCommAmt = DisGapCommAmt_Val + LND_DIST_GSTCAl - DisGapCommAmt_TDS;
        //                            }

        //                        }
        //                        else
        //                        {                                    
        //                            DisGapCommAmt_TDS = decimal.Round(((DisGapCommAmt_Val - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                LND_DIST_GSTCAl = decimal.Round(((DisGapCommAmt_Val - gst_VAlue)));
        //                                DisGapCommAmt = DisGapCommAmt_Val + LND_DIST_GSTCAl - DisGapCommAmt_TDS;
        //                            }
        //                            else
        //                            {
        //                                LND_DIST_GSTCAl =0;
        //                                DisGapCommAmt = DisGapCommAmt_Val + LND_DIST_GSTCAl - DisGapCommAmt_TDS;
        //                            }

        //                        }

        //                    }

        //                    decimal CommDisAddClosing = tbl_accountDistributor.CLOSING + DisGapCommAmt;
        //                    long Dis_ID = long.Parse(DIS_MEM_ID.ToString());
        //                    TBL_ACCOUNTS objDisCommPer = new TBL_ACCOUNTS()
        //                    {
        //                        API_ID = 0,
        //                        MEM_ID = Dis_ID,
        //                        MEMBER_TYPE = Dismembtype.roleName,
        //                        TRANSACTION_TYPE = rechargeType,
        //                        TRANSACTION_DATE = System.DateTime.Now,
        //                        TRANSACTION_TIME = DateTime.Now,
        //                        DR_CR = "CR",
        //                        AMOUNT = Trans_Amt,
        //                        NARRATION = "Commission",
        //                        OPENING = DisClosingAmt,
        //                        CLOSING = CommDisAddClosing,
        //                        REC_NO = 0,
        //                        COMM_AMT = DisGapCommAmt,
        //                        GST = (float)LND_DIST_GSTCAl,
        //                        TDS = (float)DisGapCommAmt_TDS,
        //                        IPAddress = IpAddress,
        //                        TDS_PERCENTAGE = taxMaster.TAX_VALUE,
        //                        GST_PERCENTAGE = GST_Master.TAX_VALUE,
        //                        WHITELEVEL_ID = (long)WHT_MEM_ID,
        //                        SUPER_ID = (long)SUP_MEM_ID,
        //                        DISTRIBUTOR_ID = (long)DIS_MEM_ID
        //                    };
        //                    db.TBL_ACCOUNTS.Add(objDisCommPer);
        //                    db.SaveChanges();
        //                }
        //                #endregion
        //                var SuperComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
        //                                 join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
        //                                  on detailscom.SLN equals commslabMob.SLAB_ID
        //                                 join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
        //                                 where commslabMob.OPERATOR_CODE == serviceprovider && detailscom.SLAB_TYPE == 1 && mem.MEMBER_ROLE == 3
        //                                 select new
        //                                 {
        //                                     SLN = commslabMob.SLN,
        //                                     commPer = commslabMob.SUPER_COM_PER,
        //                                     commType = commslabMob.COMMISSION_TYPE
        //                                 }).FirstOrDefault();
        //                //var SuperComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
        //                //                 join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
        //                //                  on detailscom.RECHARGE_SLAB equals commslabMob.SLAB_ID
        //                //                 where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == SUP_MEM_ID && commslabMob.OPERATOR_CODE == serviceprovider
        //                //                 select new
        //                //                 {
        //                //                     SLN = commslabMob.SLN,
        //                //                     commPer = commslabMob.COMM_PERCENTAGE
        //                //                 }).FirstOrDefault();
        //                //decimal SupGapComm = decimal.Parse(SuperComm.commPer.ToString()) - decimal.Parse(DistributorComm.commPer.ToString());

        //                #region Super Commission
        //                var tbl_accountSuper = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == SUP_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
        //                if (tbl_accountSuper != null)
        //                {
        //                    var Supmembtype = (from mm in db.TBL_MASTER_MEMBER
        //                                       join
        //                                           rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
        //                                       where mm.MEM_ID == SUP_MEM_ID
        //                                       select new
        //                                       {
        //                                           RoleId = mm.MEMBER_ROLE,
        //                                           roleName = rm.ROLE_NAME,
        //                                           Amount = mm.BALANCE
        //                                       }).FirstOrDefault();
        //                    decimal SupClosingAmt = tbl_accountSuper.CLOSING;

        //                    decimal SupGapComm = decimal.Parse(SuperComm.commPer.ToString());
        //                    //decimal SupGapCommAmt = (Trans_Amt * SupGapComm) / 100;
        //                    // GST and TDS Calculation for Super
        //                    decimal SupGapCommAmt_val = 0;
        //                    decimal Sp_TDSAmt = 0;
        //                    decimal SupGapCommAmt = 0;
        //                    decimal LNDSUpER_GSTCAl = 0;
        //                    if (SuperComm.commType == "FIXED")
        //                    {
        //                        SupGapCommAmt_val = SupGapComm;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                    
        //                            Sp_TDSAmt = decimal.Round(((SupGapCommAmt_val * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                LNDSUpER_GSTCAl = decimal.Round(((SupGapCommAmt_val * gst_VAlue) / 100), 2);
        //                                SupGapCommAmt = SupGapCommAmt_val + LNDSUpER_GSTCAl - Sp_TDSAmt;
        //                            }
        //                            else
        //                            {
        //                                LNDSUpER_GSTCAl = 0;
        //                                SupGapCommAmt = SupGapCommAmt_val + LNDSUpER_GSTCAl - Sp_TDSAmt;
        //                            }                                    
        //                        }
        //                        else
        //                        {                                    
        //                            Sp_TDSAmt = decimal.Round(((SupGapCommAmt_val - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                LNDSUpER_GSTCAl = decimal.Round(((SupGapCommAmt_val - gst_VAlue) / 100), 2);
        //                                SupGapCommAmt = SupGapCommAmt_val + LNDSUpER_GSTCAl - Sp_TDSAmt;
        //                            }
        //                            else
        //                            {
        //                                LNDSUpER_GSTCAl = 0;
        //                                SupGapCommAmt = SupGapCommAmt_val + LNDSUpER_GSTCAl - Sp_TDSAmt;
        //                            }
        //                        }                                
        //                    }
        //                    else
        //                    {
        //                        SupGapCommAmt_val = (Trans_Amt * SupGapComm) / 100;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                    
        //                            Sp_TDSAmt = decimal.Round(((SupGapCommAmt_val * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                LNDSUpER_GSTCAl = decimal.Round(((SupGapCommAmt_val * gst_VAlue) / 100), 2);
        //                                SupGapCommAmt = SupGapCommAmt_val + LNDSUpER_GSTCAl - Sp_TDSAmt;
        //                            }
        //                            else
        //                            {
        //                                LNDSUpER_GSTCAl = 0;
        //                                SupGapCommAmt = SupGapCommAmt_val + LNDSUpER_GSTCAl - Sp_TDSAmt;
        //                            }
        //                        }
        //                        else
        //                        {                                   
        //                            Sp_TDSAmt = decimal.Round(((SupGapCommAmt_val * GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                LNDSUpER_GSTCAl = decimal.Round(((SupGapCommAmt_val * gst_VAlue)));
        //                                SupGapCommAmt = SupGapCommAmt_val + LNDSUpER_GSTCAl - Sp_TDSAmt;
        //                            }
        //                            else
        //                            {
        //                                LNDSUpER_GSTCAl = 0;
        //                                SupGapCommAmt = SupGapCommAmt_val + LNDSUpER_GSTCAl - Sp_TDSAmt;
        //                            }
        //                        }                                
        //                    }
        //                    decimal CommSupAddClosing = tbl_accountSuper.CLOSING + SupGapCommAmt;
        //                    long Sup_ID = long.Parse(SUP_MEM_ID.ToString());
        //                    TBL_ACCOUNTS objSupCommPer = new TBL_ACCOUNTS()
        //                    {
        //                        API_ID = 0,
        //                        MEM_ID = Sup_ID,
        //                        MEMBER_TYPE = Supmembtype.roleName,
        //                        TRANSACTION_TYPE = rechargeType,
        //                        TRANSACTION_DATE = System.DateTime.Now,
        //                        TRANSACTION_TIME = DateTime.Now,
        //                        DR_CR = "CR",
        //                        AMOUNT = Trans_Amt,
        //                        NARRATION = "Commission",
        //                        OPENING = SupClosingAmt,
        //                        CLOSING = CommSupAddClosing,
        //                        REC_NO = 0,
        //                        COMM_AMT = SupGapCommAmt,
        //                        GST = (float)LNDSUpER_GSTCAl,
        //                        TDS = (float)Sp_TDSAmt,
        //                        IPAddress = IpAddress,
        //                        TDS_PERCENTAGE = taxMaster.TAX_VALUE,
        //                        GST_PERCENTAGE = GST_Master.TAX_VALUE,
        //                        WHITELEVEL_ID = (long)WHT_MEM_ID,
        //                        SUPER_ID = (long)SUP_MEM_ID,
        //                        DISTRIBUTOR_ID = (long)DIS_MEM_ID
        //                    };
        //                    db.TBL_ACCOUNTS.Add(objSupCommPer);
        //                    db.SaveChanges();
        //                }
        //                #endregion                        
        //                var WhiteComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
        //                                 join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
        //                                  on detailscom.RECHARGE_SLAB equals commslabMob.SLAB_ID
        //                                 where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == 0 && commslabMob.OPERATOR_CODE == serviceprovider
        //                                 select new
        //                                 {
        //                                     SLN = commslabMob.SLN,
        //                                     commPer = commslabMob.COMM_PERCENTAGE,
        //                                     commType = commslabMob.COMMISSION_TYPE
        //                                 }).FirstOrDefault();
        //                decimal WLComm = decimal.Parse(MerchantComm.commPer.ToString()) + decimal.Parse(DistributorComm.commPer.ToString()) + decimal.Parse(SuperComm.commPer.ToString());
        //                decimal WTLGapComm = decimal.Parse(WhiteComm.commPer.ToString()) - WLComm;
        //                //decimal WTLGapComm = decimal.Parse(WhiteComm.commPer.ToString());
        //                //decimal WTLGapComm =WLComm;

        //                #region White level Commission payment Structure
        //                var tbl_accountWhiteLevel = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == WHT_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
        //                if (tbl_accountWhiteLevel != null)
        //                {
        //                    var WLmembtype = (from mm in db.TBL_MASTER_MEMBER
        //                                      join
        //                                          rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
        //                                      where mm.MEM_ID == WHT_MEM_ID
        //                                      select new
        //                                      {
        //                                          RoleId = mm.MEMBER_ROLE,
        //                                          roleName = rm.ROLE_NAME,
        //                                          Amount = mm.BALANCE
        //                                      }).FirstOrDefault();
        //                    decimal WLClosingAmt = tbl_accountWhiteLevel.CLOSING;
        //                    //decimal SubAmt = ClosingAmt - Trans_Amt;                  
        //                    // GST And TDS WhiteLevel Calculation

        //                    decimal WTLGapCommAmt_AMt = 0;
        //                    decimal WL_TDSCalculation = 0;
        //                    decimal WTLGapCommAmt = 0;
        //                    decimal LNDWL_GSTCAL = 0;
        //                    if (WhiteComm.commType == "FIXED")
        //                    {
        //                        WTLGapCommAmt_AMt = WTLGapComm;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                  
        //                            WL_TDSCalculation = decimal.Round(((WTLGapCommAmt_AMt * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                LNDWL_GSTCAL = decimal.Round(((WTLGapCommAmt_AMt * gst_VAlue) / 100), 2);
        //                                WTLGapCommAmt = WTLGapCommAmt_AMt + LNDWL_GSTCAL - WL_TDSCalculation;
        //                            }
        //                            else
        //                            {
        //                                LNDWL_GSTCAL = 0;
        //                                WTLGapCommAmt = WTLGapCommAmt_AMt + LNDWL_GSTCAL - WL_TDSCalculation;
        //                            }

        //                        }
        //                        else
        //                        {                                    
        //                            WL_TDSCalculation = decimal.Round(((WTLGapCommAmt_AMt - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                LNDWL_GSTCAL = decimal.Round(((WTLGapCommAmt_AMt - gst_VAlue)));
        //                                WTLGapCommAmt = WTLGapCommAmt_AMt + LNDWL_GSTCAL - WL_TDSCalculation;
        //                            }
        //                            else
        //                            {
        //                                LNDWL_GSTCAL = 0;
        //                                WTLGapCommAmt = WTLGapCommAmt_AMt + LNDWL_GSTCAL - WL_TDSCalculation;
        //                            }

        //                        }

        //                    }
        //                    else
        //                    {
        //                        WTLGapCommAmt_AMt = (Trans_Amt * WTLGapComm) / 100;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                    
        //                            WL_TDSCalculation = decimal.Round(((WTLGapCommAmt_AMt * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                LNDWL_GSTCAL = decimal.Round(((WTLGapCommAmt_AMt * gst_VAlue) / 100), 2);
        //                                WTLGapCommAmt = WTLGapCommAmt_AMt + LNDWL_GSTCAL - WL_TDSCalculation;
        //                            }
        //                            else
        //                            {
        //                                LNDWL_GSTCAL = 0;
        //                                WTLGapCommAmt = WTLGapCommAmt_AMt + LNDWL_GSTCAL - WL_TDSCalculation;
        //                            }

        //                        }
        //                        else
        //                        {                                   
        //                            WL_TDSCalculation = decimal.Round(((WTLGapCommAmt_AMt - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                LNDWL_GSTCAL = decimal.Round(((WTLGapCommAmt_AMt - gst_VAlue)));
        //                                WTLGapCommAmt = WTLGapCommAmt_AMt + LNDWL_GSTCAL - WL_TDSCalculation;
        //                            }
        //                            else
        //                            {
        //                                LNDWL_GSTCAL = 0;
        //                                WTLGapCommAmt = WTLGapCommAmt_AMt + LNDWL_GSTCAL - WL_TDSCalculation;
        //                            }                                   
        //                        }                                    
        //                    }

        //                    //decimal WTLGapCommAmt = (Trans_Amt * WTLGapComm) / 100;

        //                    decimal CommWLAddClosing = tbl_accountWhiteLevel.CLOSING + WTLGapCommAmt;
        //                    long WL_ID = long.Parse(WHT_MEM_ID.ToString());
        //                    TBL_ACCOUNTS objWLCommPer = new TBL_ACCOUNTS()
        //                    {
        //                        API_ID = 0,
        //                        MEM_ID = WL_ID,
        //                        MEMBER_TYPE = WLmembtype.roleName,
        //                        TRANSACTION_TYPE = rechargeType,
        //                        TRANSACTION_DATE = System.DateTime.Now,
        //                        TRANSACTION_TIME = DateTime.Now,
        //                        DR_CR = "CR",
        //                        AMOUNT = Trans_Amt,
        //                        NARRATION = "Commission",
        //                        OPENING = WLClosingAmt,
        //                        CLOSING = CommWLAddClosing,
        //                        REC_NO = 0,
        //                        COMM_AMT = WTLGapCommAmt,
        //                        GST = (float)LNDWL_GSTCAL,
        //                        TDS = (float)WL_TDSCalculation,
        //                        IPAddress = IpAddress,
        //                        TDS_PERCENTAGE = taxMaster.TAX_VALUE,
        //                        GST_PERCENTAGE = GST_Master.TAX_VALUE,
        //                        WHITELEVEL_ID = (long)WHT_MEM_ID,
        //                        SUPER_ID = (long)SUP_MEM_ID,
        //                        DISTRIBUTOR_ID = (long)DIS_MEM_ID

        //                    };
        //                    db.TBL_ACCOUNTS.Add(objWLCommPer);
        //                    db.SaveChanges();
        //                }
        //                #endregion
        //                ContextTransaction.Commit();
        //                return true;
        //            }
        //            else if (status == "GAS")
        //            {

        //                var MerchantComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
        //                                    join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
        //                                    on detailscom.SLN equals commslabMob.SLAB_ID
        //                                    join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
        //                                    where commslabMob.OPERATOR_CODE == serviceprovider && mem.MEMBER_ROLE == 5
        //                                    select new
        //                                    {
        //                                        SLN = commslabMob.SLN,
        //                                        commPer = commslabMob.MERCHANT_COM_PER,
        //                                        commType = commslabMob.COMMISSION_TYPE
        //                                    }).FirstOrDefault();
        //                #region Retailer Commission                        
        //                var membtype = (from mm in db.TBL_MASTER_MEMBER
        //                                join
        //                                    rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
        //                                where mm.MEM_ID == Mem_ID
        //                                select new
        //                                {
        //                                    RoleId = mm.MEMBER_ROLE,
        //                                    roleName = rm.ROLE_NAME,
        //                                    Amount = mm.BALANCE
        //                                }).FirstOrDefault();
        //                var tbl_account = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == Mem_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
        //                if (tbl_account != null)
        //                {
        //                    decimal ClosingAmt = tbl_account.CLOSING;
        //                    decimal SubAmt = ClosingAmt - Trans_Amt;
        //                    TBL_ACCOUNTS objaccnt = new TBL_ACCOUNTS()
        //                    {
        //                        API_ID = 0,
        //                        MEM_ID = Mem_ID,
        //                        MEMBER_TYPE = membtype.roleName,
        //                        TRANSACTION_TYPE = rechargeType,
        //                        TRANSACTION_DATE = System.DateTime.Now,
        //                        TRANSACTION_TIME = DateTime.Now,
        //                        DR_CR = "DR",
        //                        AMOUNT = Trans_Amt,
        //                        NARRATION = "Recharge",
        //                        OPENING = ClosingAmt,
        //                        CLOSING = SubAmt,
        //                        REC_NO = 0,
        //                        COMM_AMT = 0,
        //                        GST = 0,
        //                        TDS = 0,
        //                        IPAddress = IpAddress,
        //                        TDS_PERCENTAGE = 0,
        //                        GST_PERCENTAGE =0,
        //                        WHITELEVEL_ID = (long)WHT_MEM_ID,
        //                        SUPER_ID = (long)SUP_MEM_ID,
        //                        DISTRIBUTOR_ID = (long)DIS_MEM_ID
        //                    };
        //                    db.TBL_ACCOUNTS.Add(objaccnt);
        //                    db.SaveChanges();
        //                    // TDS and GST CAlculation for Merchant for electricity
        //                    //decimal getPer = (Trans_Amt * MerchantComm.commPer) / 100;
        //                    decimal ELE_getPer_Val = 0;
        //                    decimal ELE_TDS_Value = 0;
        //                    decimal getPer = 0;
        //                    decimal WL_GSTCAl = 0;
        //                    if (MerchantComm.commType == "FIXED")
        //                    {
        //                        ELE_getPer_Val = MerchantComm.commPer;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                    
        //                            ELE_TDS_Value = decimal.Round(((ELE_getPer_Val * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                WL_GSTCAl = decimal.Round(((ELE_getPer_Val * gst_VAlue) / 100), 2);
        //                                getPer = (ELE_getPer_Val + WL_GSTCAl - ELE_TDS_Value);
        //                            }
        //                            else
        //                            {
        //                                WL_GSTCAl = 0;
        //                                getPer = (ELE_getPer_Val + WL_GSTCAl - ELE_TDS_Value);
        //                            }

        //                        }
        //                        else
        //                        {                                    
        //                            ELE_TDS_Value = decimal.Round(((ELE_getPer_Val- GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                WL_GSTCAl = decimal.Round(((ELE_getPer_Val - gst_VAlue)));
        //                                getPer = (ELE_getPer_Val + WL_GSTCAl - ELE_TDS_Value);
        //                            }
        //                            else
        //                            {
        //                                WL_GSTCAl = 0;
        //                                getPer = (ELE_getPer_Val + WL_GSTCAl - ELE_TDS_Value);
        //                            }

        //                        }

        //                    }
        //                    else
        //                    {
        //                        ELE_getPer_Val = (Trans_Amt * MerchantComm.commPer) / 100;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                    
        //                            ELE_TDS_Value = decimal.Round(((ELE_getPer_Val * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                WL_GSTCAl = decimal.Round(((ELE_getPer_Val * gst_VAlue) / 100), 2);
        //                                getPer = (ELE_getPer_Val + WL_GSTCAl - ELE_TDS_Value);
        //                            }
        //                            else
        //                            {
        //                                WL_GSTCAl = 0;
        //                                getPer = (ELE_getPer_Val + WL_GSTCAl - ELE_TDS_Value);
        //                            }

        //                        }
        //                        else
        //                        {

        //                            ELE_TDS_Value = decimal.Round(((ELE_getPer_Val - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                WL_GSTCAl = decimal.Round(((ELE_getPer_Val - gst_VAlue)));
        //                                getPer = (ELE_getPer_Val + WL_GSTCAl - ELE_TDS_Value);
        //                            }
        //                            else
        //                            {
        //                                WL_GSTCAl = 0;
        //                                getPer = (ELE_getPer_Val + WL_GSTCAl - ELE_TDS_Value);
        //                            }

        //                        }

        //                    }
        //                    decimal CommAddClosing = SubAmt + getPer;
        //                    TBL_ACCOUNTS objCommPer = new TBL_ACCOUNTS()
        //                    {
        //                        API_ID = 0,
        //                        MEM_ID = Mem_ID,
        //                        MEMBER_TYPE = membtype.roleName,
        //                        TRANSACTION_TYPE = rechargeType,
        //                        TRANSACTION_DATE = System.DateTime.Now,
        //                        TRANSACTION_TIME = DateTime.Now,
        //                        DR_CR = "CR",
        //                        AMOUNT = Trans_Amt,
        //                        NARRATION = "Commission",
        //                        OPENING = SubAmt,
        //                        CLOSING = CommAddClosing,
        //                        REC_NO = 0,
        //                        COMM_AMT = getPer,
        //                        GST = (float)WL_GSTCAl,
        //                        TDS = (float)ELE_TDS_Value,
        //                        IPAddress = IpAddress,
        //                        TDS_PERCENTAGE = taxMaster.TAX_VALUE,
        //                        GST_PERCENTAGE = GST_Master.TAX_VALUE,
        //                        WHITELEVEL_ID = (long)WHT_MEM_ID,
        //                        SUPER_ID = (long)SUP_MEM_ID,
        //                        DISTRIBUTOR_ID = (long)DIS_MEM_ID
        //                    };
        //                    db.TBL_ACCOUNTS.Add(objCommPer);
        //                    db.SaveChanges();
        //                }
        //                #endregion

        //                //var DistributorComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
        //                //                       join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
        //                //                       on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
        //                //                       where detailscom.WHITE_LEVEL_ID == SUP_MEM_ID && detailscom.INTRODUCER_ID == DIS_MEM_ID && commslabMob.OPERATOR_CODE == serviceprovider
        //                //                       select new
        //                //                       {
        //                //                           SLN = commslabMob.SLN,
        //                //                           commPer = commslabMob.COMM_PERCENTAGE
        //                //                       }).FirstOrDefault();
        //                var DistributorComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
        //                                       join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
        //                                       on detailscom.SLN equals commslabMob.SLAB_ID
        //                                       join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
        //                                       where commslabMob.OPERATOR_CODE == serviceprovider && mem.MEMBER_ROLE == 4
        //                                       select new
        //                                       {
        //                                           SLN = commslabMob.SLN,
        //                                           commPer = commslabMob.DISTRIBUTOR_COM_PER,
        //                                           commType = commslabMob.COMMISSION_TYPE
        //                                       }).FirstOrDefault();
        //                #region Distributor Commission
        //                //decimal DisGapComm = decimal.Parse(DistributorComm.commPer.ToString()) - decimal.Parse(MerchantComm.commPer.ToString());
        //                decimal DisGapComm = decimal.Parse(DistributorComm.commPer.ToString());
        //                //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;
        //                var tbl_accountDistributor = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == DIS_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
        //                if (tbl_accountDistributor != null)
        //                {
        //                    var CheckDisCommission = db.TBL_DETAILS_MEMBER_COMMISSION_SLAB.Where(x => x.INTRODUCER_ID == DIS_MEM_ID).FirstOrDefault();
        //                    var Dismembtype = (from mm in db.TBL_MASTER_MEMBER
        //                                       join
        //                                           rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
        //                                       where mm.MEM_ID == DIS_MEM_ID
        //                                       select new
        //                                       {
        //                                           RoleId = mm.MEMBER_ROLE,
        //                                           roleName = rm.ROLE_NAME,
        //                                           Amount = mm.BALANCE
        //                                       }).FirstOrDefault();
        //                    decimal DisClosingAmt = tbl_accountDistributor.CLOSING;
        //                    //decimal SubAmt = ClosingAmt - Trans_Amt;                   
        //                    //decimal DisgetPer = (Trans_Amt * checkDiscomm.CommPercentage) / 100;

        //                    // For TDS and GST CAlculation for Electricity
        //                    //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;
        //                    decimal ELE_DisGap_CommAmt = 0;
        //                    decimal ELE_TDSCALCULATION = 0;
        //                    decimal DisGapCommAmt = 0;
        //                    decimal GasDist_GSTCAl = 0;
        //                    if (DistributorComm.commType == "FIXED")
        //                    {
        //                        ELE_DisGap_CommAmt = DisGapComm;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                   
        //                            ELE_TDSCALCULATION = decimal.Round(((ELE_DisGap_CommAmt * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                GasDist_GSTCAl = decimal.Round(((ELE_DisGap_CommAmt * gst_VAlue) / 100), 2);
        //                                DisGapCommAmt = ELE_DisGap_CommAmt + GasDist_GSTCAl - ELE_TDSCALCULATION;
        //                            }
        //                            else
        //                            {
        //                                GasDist_GSTCAl = 0;
        //                                DisGapCommAmt = ELE_DisGap_CommAmt + GasDist_GSTCAl - ELE_TDSCALCULATION;
        //                            }

        //                        }
        //                        else
        //                        {

        //                            ELE_TDSCALCULATION = decimal.Round(((ELE_DisGap_CommAmt - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                GasDist_GSTCAl = decimal.Round(((ELE_DisGap_CommAmt - gst_VAlue)));
        //                                DisGapCommAmt = ELE_DisGap_CommAmt + GasDist_GSTCAl - ELE_TDSCALCULATION;
        //                            }
        //                            else
        //                            {
        //                                GasDist_GSTCAl = 0;
        //                                DisGapCommAmt = ELE_DisGap_CommAmt + GasDist_GSTCAl - ELE_TDSCALCULATION;
        //                            }

        //                        }

        //                    }
        //                    else
        //                    {
        //                        ELE_DisGap_CommAmt = (Trans_Amt * DisGapComm) / 100;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                    
        //                            ELE_TDSCALCULATION = decimal.Round(((ELE_DisGap_CommAmt * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                GasDist_GSTCAl = decimal.Round(((ELE_DisGap_CommAmt * gst_VAlue) / 100), 2);
        //                                DisGapCommAmt = ELE_DisGap_CommAmt + GasDist_GSTCAl - ELE_TDSCALCULATION;
        //                            }
        //                            else
        //                            {
        //                                GasDist_GSTCAl = 0;
        //                                DisGapCommAmt = ELE_DisGap_CommAmt + GasDist_GSTCAl - ELE_TDSCALCULATION;
        //                            }

        //                        }
        //                        else
        //                        {                                    
        //                            ELE_TDSCALCULATION = decimal.Round(((ELE_DisGap_CommAmt * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                GasDist_GSTCAl = decimal.Round(((ELE_DisGap_CommAmt - gst_VAlue)));
        //                                DisGapCommAmt = ELE_DisGap_CommAmt + GasDist_GSTCAl - ELE_TDSCALCULATION;
        //                            }
        //                            else
        //                            {
        //                                GasDist_GSTCAl =0;
        //                                DisGapCommAmt = ELE_DisGap_CommAmt + GasDist_GSTCAl - ELE_TDSCALCULATION;
        //                            }

        //                        }

        //                    }
        //                    decimal CommDisAddClosing = tbl_accountDistributor.CLOSING + DisGapCommAmt;
        //                    long Dis_ID = long.Parse(DIS_MEM_ID.ToString());
        //                    TBL_ACCOUNTS objDisCommPer = new TBL_ACCOUNTS()
        //                    {
        //                        API_ID = 0,
        //                        MEM_ID = Dis_ID,
        //                        MEMBER_TYPE = Dismembtype.roleName,
        //                        TRANSACTION_TYPE = rechargeType,
        //                        TRANSACTION_DATE = System.DateTime.Now,
        //                        TRANSACTION_TIME = DateTime.Now,
        //                        DR_CR = "CR",
        //                        AMOUNT = Trans_Amt,
        //                        NARRATION = "Commission",
        //                        OPENING = DisClosingAmt,
        //                        CLOSING = CommDisAddClosing,
        //                        REC_NO = 0,
        //                        COMM_AMT = DisGapCommAmt,
        //                        GST = (float)GasDist_GSTCAl,
        //                        TDS = (float)ELE_TDSCALCULATION,
        //                        IPAddress = IpAddress,
        //                        TDS_PERCENTAGE = taxMaster.TAX_VALUE,
        //                        GST_PERCENTAGE = GST_Master.TAX_VALUE,
        //                        WHITELEVEL_ID = (long)WHT_MEM_ID,
        //                        SUPER_ID = (long)SUP_MEM_ID,
        //                        DISTRIBUTOR_ID = (long)DIS_MEM_ID
        //                    };
        //                    db.TBL_ACCOUNTS.Add(objDisCommPer);
        //                    db.SaveChanges();
        //                }
        //                #endregion
        //                //var SuperComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
        //                //                 join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
        //                //                  on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
        //                //                 where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == SUP_MEM_ID && commslabMob.OPERATOR_CODE == serviceprovider
        //                //                 select new
        //                //                 {
        //                //                     SLN = commslabMob.SLN,
        //                //                     commPer = commslabMob.COMM_PERCENTAGE
        //                //                 }).FirstOrDefault();
        //                var SuperComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
        //                                 join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
        //                                  on detailscom.SLN equals commslabMob.SLAB_ID
        //                                 join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
        //                                 where commslabMob.OPERATOR_CODE == serviceprovider && mem.MEMBER_ROLE == 3
        //                                 select new
        //                                 {
        //                                     SLN = commslabMob.SLN,
        //                                     commPer = commslabMob.SUPER_COM_PER,
        //                                     commType = commslabMob.COMMISSION_TYPE
        //                                 }).FirstOrDefault();
        //                //decimal SupGapComm = decimal.Parse(SuperComm.commPer.ToString()) - decimal.Parse(DistributorComm.commPer.ToString());
        //                decimal SupGapComm = decimal.Parse(SuperComm.commPer.ToString());
        //                //decimal SupGapCommAmt = (Trans_Amt * SupGapComm) / 100;
        //                #region Super Commission
        //                var tbl_accountSuper = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == SUP_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
        //                if (tbl_accountSuper != null)
        //                {
        //                    var Supmembtype = (from mm in db.TBL_MASTER_MEMBER
        //                                       join
        //                                           rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
        //                                       where mm.MEM_ID == SUP_MEM_ID
        //                                       select new
        //                                       {
        //                                           RoleId = mm.MEMBER_ROLE,
        //                                           roleName = rm.ROLE_NAME,
        //                                           Amount = mm.BALANCE
        //                                       }).FirstOrDefault();
        //                    decimal SupClosingAmt = tbl_accountSuper.CLOSING;
        //                    // TDS AND GST CALCULATION FOR SUPER FOR ELECTRICITY
        //                    decimal ELE_Sup_Gap_Comm_Amt = 0;
        //                    decimal ELE_SUP_TDS_CAL = 0;
        //                    decimal SupGapCommAmt = 0;
        //                    decimal GAsSupGST_CAl = 0;
        //                    if (SuperComm.commType == "FIXED")
        //                    {
        //                        ELE_Sup_Gap_Comm_Amt = SupGapComm;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                    
        //                            ELE_SUP_TDS_CAL = decimal.Round(((ELE_Sup_Gap_Comm_Amt * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                GAsSupGST_CAl = decimal.Round(((ELE_Sup_Gap_Comm_Amt * gst_VAlue) / 100), 2);
        //                                SupGapCommAmt = ELE_Sup_Gap_Comm_Amt + GAsSupGST_CAl - ELE_SUP_TDS_CAL;
        //                            }
        //                            else
        //                            {
        //                                GAsSupGST_CAl = 0;
        //                                SupGapCommAmt = ELE_Sup_Gap_Comm_Amt + GAsSupGST_CAl - ELE_SUP_TDS_CAL;
        //                            }

        //                        }
        //                        else
        //                        {

        //                            ELE_SUP_TDS_CAL = decimal.Round(((ELE_Sup_Gap_Comm_Amt - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                GAsSupGST_CAl = decimal.Round(((ELE_Sup_Gap_Comm_Amt - gst_VAlue)));
        //                                SupGapCommAmt = ELE_Sup_Gap_Comm_Amt + GAsSupGST_CAl - ELE_SUP_TDS_CAL;
        //                            }
        //                            else
        //                            {
        //                                GAsSupGST_CAl = 0;
        //                                SupGapCommAmt = ELE_Sup_Gap_Comm_Amt + GAsSupGST_CAl - ELE_SUP_TDS_CAL;
        //                            }

        //                        }

        //                    }
        //                    else
        //                    {
        //                        ELE_Sup_Gap_Comm_Amt = (Trans_Amt * SupGapComm) / 100;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {

        //                            ELE_SUP_TDS_CAL = decimal.Round(((ELE_Sup_Gap_Comm_Amt * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                GAsSupGST_CAl = decimal.Round(((ELE_Sup_Gap_Comm_Amt * gst_VAlue) / 100), 2);
        //                                SupGapCommAmt = ELE_Sup_Gap_Comm_Amt + GAsSupGST_CAl - ELE_SUP_TDS_CAL;
        //                            }
        //                            else
        //                            {
        //                                GAsSupGST_CAl = 0;
        //                                SupGapCommAmt = ELE_Sup_Gap_Comm_Amt + GAsSupGST_CAl - ELE_SUP_TDS_CAL;
        //                            }

        //                        }
        //                        else
        //                        {

        //                            ELE_SUP_TDS_CAL = decimal.Round(((ELE_Sup_Gap_Comm_Amt - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                GAsSupGST_CAl = decimal.Round(((ELE_Sup_Gap_Comm_Amt - gst_VAlue)));
        //                                SupGapCommAmt = ELE_Sup_Gap_Comm_Amt + GAsSupGST_CAl - ELE_SUP_TDS_CAL;
        //                            }
        //                            else
        //                            {
        //                                GAsSupGST_CAl = 0;
        //                                SupGapCommAmt = ELE_Sup_Gap_Comm_Amt + GAsSupGST_CAl - ELE_SUP_TDS_CAL;
        //                            }

        //                        }
        //                    }
        //                    decimal CommSupAddClosing = tbl_accountSuper.CLOSING + SupGapCommAmt;
        //                    long Sup_ID = long.Parse(SUP_MEM_ID.ToString());
        //                    TBL_ACCOUNTS objSupCommPer = new TBL_ACCOUNTS()
        //                    {
        //                        API_ID = 0,
        //                        MEM_ID = Sup_ID,
        //                        MEMBER_TYPE = Supmembtype.roleName,
        //                        TRANSACTION_TYPE = rechargeType,
        //                        TRANSACTION_DATE = System.DateTime.Now,
        //                        TRANSACTION_TIME = DateTime.Now,
        //                        DR_CR = "CR",
        //                        AMOUNT = Trans_Amt,
        //                        NARRATION = "Commission",
        //                        OPENING = SupClosingAmt,
        //                        CLOSING = CommSupAddClosing,
        //                        REC_NO = 0,
        //                        COMM_AMT = SupGapCommAmt,
        //                        GST = (float)GAsSupGST_CAl,
        //                        TDS = (float)ELE_SUP_TDS_CAL,
        //                        IPAddress = IpAddress,
        //                        TDS_PERCENTAGE = taxMaster.TAX_VALUE,
        //                        GST_PERCENTAGE = GST_Master.TAX_VALUE,
        //                        WHITELEVEL_ID = (long)WHT_MEM_ID,
        //                        SUPER_ID = (long)SUP_MEM_ID,
        //                        DISTRIBUTOR_ID = (long)DIS_MEM_ID
        //                    };
        //                    db.TBL_ACCOUNTS.Add(objSupCommPer);
        //                    db.SaveChanges();
        //                }
        //                #endregion                        
        //                var WhiteComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
        //                                 join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
        //                                  on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
        //                                 where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == 0 && commslabMob.OPERATOR_CODE == serviceprovider
        //                                 select new
        //                                 {
        //                                     SLN = commslabMob.SLN,
        //                                     commPer = commslabMob.COMM_PERCENTAGE,
        //                                     commType = commslabMob.COMMISSION_TYPE
        //                                 }).FirstOrDefault();
        //                decimal WL_Com = decimal.Parse(MerchantComm.commPer.ToString()) + decimal.Parse(DistributorComm.commPer.ToString()) + decimal.Parse(SuperComm.commPer.ToString());
        //                //decimal WTLGapComm = decimal.Parse(WhiteComm.commPer.ToString()) - decimal.Parse(SuperComm.commPer.ToString());
        //                decimal WTLGapComm = decimal.Parse(WhiteComm.commPer.ToString()) - WL_Com;
        //                //decimal WTLGapCommAmt = (Trans_Amt * WTLGapComm) / 100;
        //                #region White level Commission payment Structure
        //                var tbl_accountWhiteLevel = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == WHT_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
        //                if (tbl_accountWhiteLevel != null)
        //                {
        //                    var WLmembtype = (from mm in db.TBL_MASTER_MEMBER
        //                                      join
        //                                          rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
        //                                      where mm.MEM_ID == WHT_MEM_ID
        //                                      select new
        //                                      {
        //                                          RoleId = mm.MEMBER_ROLE,
        //                                          roleName = rm.ROLE_NAME,
        //                                          Amount = mm.BALANCE
        //                                      }).FirstOrDefault();
        //                    decimal WLClosingAmt = tbl_accountWhiteLevel.CLOSING;
        //                    //decimal SubAmt = ClosingAmt - Trans_Amt;           
        //                    // GST And TDS Calculation for WL 
        //                    //decimal WTLGapCommAmt = (Trans_Amt * WTLGapComm) / 100;
        //                    decimal ELE_WTL_Gap_CommAmt = 0;
        //                    decimal ELE_WL_TDSCal = 0;
        //                    decimal WTLGapCommAmt = 0;
        //                    decimal GasWTLGST_Cal = 0;
        //                    if (WhiteComm.commType == "FIXED")
        //                    {
        //                        ELE_WTL_Gap_CommAmt = WTLGapComm;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                    
        //                            ELE_WL_TDSCal = decimal.Round(((ELE_WTL_Gap_CommAmt * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                GasWTLGST_Cal = decimal.Round(((ELE_WTL_Gap_CommAmt * gst_VAlue) / 100), 2);
        //                                WTLGapCommAmt = ELE_WTL_Gap_CommAmt + GasWTLGST_Cal - ELE_WL_TDSCal;
        //                            }
        //                            else
        //                            {
        //                                GasWTLGST_Cal = 0;
        //                                WTLGapCommAmt = ELE_WTL_Gap_CommAmt + GasWTLGST_Cal - ELE_WL_TDSCal;
        //                            }

        //                        }
        //                        else
        //                        {                                    
        //                            ELE_WL_TDSCal = decimal.Round(((ELE_WTL_Gap_CommAmt - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                GasWTLGST_Cal = decimal.Round(((ELE_WTL_Gap_CommAmt - gst_VAlue)));
        //                                WTLGapCommAmt = ELE_WTL_Gap_CommAmt + GasWTLGST_Cal - ELE_WL_TDSCal;
        //                            }
        //                            else
        //                            {
        //                                GasWTLGST_Cal =0;
        //                                WTLGapCommAmt = ELE_WTL_Gap_CommAmt + GasWTLGST_Cal - ELE_WL_TDSCal;
        //                            }

        //                        }

        //                    }
        //                    else
        //                    {
        //                        ELE_WTL_Gap_CommAmt = (Trans_Amt * WTLGapComm) / 100;
        //                        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
        //                        {                                 
        //                            ELE_WL_TDSCal = decimal.Round(((ELE_WTL_Gap_CommAmt * GST_AMount) / 100), 2);
        //                            if (MemberGST != null)
        //                            {
        //                                GasWTLGST_Cal = decimal.Round(((ELE_WTL_Gap_CommAmt * gst_VAlue) / 100), 2);
        //                                WTLGapCommAmt = ELE_WTL_Gap_CommAmt + GasWTLGST_Cal - ELE_WL_TDSCal;
        //                            }
        //                            else
        //                            {
        //                                GasWTLGST_Cal = 0;
        //                                WTLGapCommAmt = ELE_WTL_Gap_CommAmt + GasWTLGST_Cal - ELE_WL_TDSCal;
        //                            }

        //                        }
        //                        else
        //                        {                                    
        //                            ELE_WL_TDSCal = decimal.Round(((ELE_WTL_Gap_CommAmt - GST_AMount)));
        //                            if (MemberGST != null)
        //                            {
        //                                GasWTLGST_Cal = decimal.Round(((ELE_WTL_Gap_CommAmt - gst_VAlue)));
        //                                WTLGapCommAmt = ELE_WTL_Gap_CommAmt + GasWTLGST_Cal - ELE_WL_TDSCal;
        //                            }
        //                            else
        //                            {
        //                                GasWTLGST_Cal =0;
        //                                WTLGapCommAmt = ELE_WTL_Gap_CommAmt + GasWTLGST_Cal - ELE_WL_TDSCal;
        //                            }

        //                        }

        //                    }
        //                    decimal CommWLAddClosing = tbl_accountWhiteLevel.CLOSING + WTLGapCommAmt;
        //                    long WL_ID = long.Parse(WHT_MEM_ID.ToString());
        //                    TBL_ACCOUNTS objWLCommPer = new TBL_ACCOUNTS()
        //                    {
        //                        API_ID = 0,
        //                        MEM_ID = WL_ID,
        //                        MEMBER_TYPE = WLmembtype.roleName,
        //                        TRANSACTION_TYPE = rechargeType,
        //                        TRANSACTION_DATE = System.DateTime.Now,
        //                        TRANSACTION_TIME = DateTime.Now,
        //                        DR_CR = "CR",
        //                        AMOUNT = Trans_Amt,
        //                        NARRATION = "Commission",
        //                        OPENING = WLClosingAmt,
        //                        CLOSING = CommWLAddClosing,
        //                        REC_NO = 0,
        //                        COMM_AMT = WTLGapCommAmt,
        //                        GST = (float)GasWTLGST_Cal,
        //                        TDS = (float)ELE_WL_TDSCal,
        //                        IPAddress = IpAddress,
        //                        TDS_PERCENTAGE = taxMaster.TAX_VALUE,
        //                        GST_PERCENTAGE = GST_Master.TAX_VALUE,
        //                        WHITELEVEL_ID = (long)WHT_MEM_ID,
        //                        SUPER_ID = (long)SUP_MEM_ID,
        //                        DISTRIBUTOR_ID = (long)DIS_MEM_ID
        //                    };
        //                    db.TBL_ACCOUNTS.Add(objWLCommPer);
        //                    db.SaveChanges();
        //                }
        //                #endregion
        //                ContextTransaction.Commit();
        //                return true;
        //            }
        //            else {
        //                return true;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            return false;
        //            Logger.Error("Commission Distribution:-   AllMemberCommissionDistribution", ex);
        //            ContextTransaction.Rollback();
        //            //throw ex;
        //        }
        //    }

        //}
        public async Task<bool> AllMemberCommissionDistribution(long Mem_ID, string status, decimal Trans_Amt, decimal ChargeAmt, decimal OpeningAmt, string serviceprovider, string rechargeType, string IpAddress,string sRandomOTP, DMR_API_Response apiRES = null ,string APIResponse=null )
        {

            var db = new DBContext();
            //var taxMaster = db.TBL_TAX_MASTER.FirstOrDefault();
            //GST cal
            long Oper_Id_Val = 0;
            //var operatorID = await db.TBL_SERVICE_PROVIDERS.Where(x => x.SERVICE_NAME == serviceprovider && x.TYPE == "MobileOperator").FirstOrDefaultAsync();
            var operatorID = await db.TBL_SERVICE_PROVIDERS.Where(x => x.SERVICE_KEY == serviceprovider).FirstOrDefaultAsync();
            if (operatorID == null)
            {
                //var Oper_Value = await db.TBL_SERVICE_PROVIDERS.Where(x => x.SERVICE_KEY == serviceprovider).FirstOrDefaultAsync();
                Oper_Id_Val = 0;
            }
            else
            {
                Oper_Id_Val = operatorID.ID;
            }
            var MemberGST = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == Mem_ID).Select(c => c.COMPANY_GST_NO).FirstOrDefaultAsync();
            var GST_Master = await db.TBL_TAX_MASTERS.Where(x => x.TAX_NAME == "GST").FirstOrDefaultAsync();
            var Gst_Mode = GST_Master.TAX_MODE;
            decimal gst_VAlue = 0;
            decimal.TryParse(GST_Master.TAX_VALUE.ToString(), out gst_VAlue);
            //TAX cal
            var taxMaster = await db.TBL_TAX_MASTERS.Where(x => x.TAX_NAME == "TDS").FirstOrDefaultAsync();
            decimal GST_AMount = 0;
            string TaxMode = taxMaster.TAX_MODE;
            decimal.TryParse(taxMaster.TAX_VALUE.ToString(), out GST_AMount);
            decimal TDS_AMount = 0;
            var VendorInfo = db.TBL_VENDOR_MASTER.FirstOrDefault(x => x.ID == 1);
            //decimal GST_AMount = decimal.Parse(System.Configuration.ConfigurationManager.AppSettings["GST_Amount"]);
            //decimal TDS_AMount = decimal.Parse(System.Configuration.ConfigurationManager.AppSettings["TDS_Amount"]);

            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    decimal MER_Comm_Amt = 0;
                    decimal MER_TaxAMT = 0;
                    decimal DEST_Comm_Amt = 0;
                    decimal DEST_TaxAMT = 0;
                    decimal SPR_Comm_Amt = 0;
                    decimal SPR_TaxAMT = 0;
                    decimal WHITE_Comm_Amt = 0;
                    decimal WHITE_TaxAMT = 0;
                    decimal MER_GST_Comm_Amt=0;
                    decimal DIST_GST_Comm_Amt = 0;
                    decimal SUPER_GST_Comm_Amt = 0;
                    decimal WHILE_GST_Comm_Amt = 0;
                    decimal FixedSErviceAmt = 0;
                    decimal MerComm_Slab_Amt = 0;
                    decimal DISTRIBUTOR_GAPP_CoMMISSIOM = 0;
                    decimal WLLMAIn_TDSVAL = 0;
                    decimal CALCUWLLPCOMM = 0;
                    decimal WLLMAINTDSTAMT = 0;
                    var DIS_MEM_ID =await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == Mem_ID).Select(z => z.INTRODUCER).FirstOrDefaultAsync();
                    //var SUP_MEM_ID = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == DIS_MEM_ID).Select(z => z.INTRODUCER).FirstOrDefaultAsync();
                    var WHT_MEM_ID = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == DIS_MEM_ID).Select(z => z.UNDER_WHITE_LEVEL).FirstOrDefaultAsync();
                    if (status == "MOBILE")
                    {
                        //var MerchantComm = await (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                        //                    join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                        //                    on detailscom.SLN equals commslabMob.SLAB_ID
                        //                    join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                        //                    where commslabMob.OPERATOR_CODE == serviceprovider && detailscom.SLAB_TYPE == 1 && mem.MEMBER_ROLE == 5 && commslabMob.MEM_ID== WHT_MEM_ID
                        //                          select new
                        //                    {
                        //                        SLN = commslabMob.SLN,
                        //                        SLAB_ID=commslabMob.SLAB_ID,
                        //                        commPer = commslabMob.MERCHANT_COM_PER,
                        //                        commType = commslabMob.COMMISSION_TYPE
                        //                    }).FirstOrDefaultAsync();
                        var MerchantComm = await (from WLPCom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB join  detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB on WLPCom.SLN equals detailscom.RECHARGE_SLAB
                                                  join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                                  on detailscom.RECHARGE_SLAB equals commslabMob.SLAB_ID
                                                  join mem in db.TBL_MASTER_MEMBER on detailscom.INTRODUCER_ID equals mem.MEM_ID
                                                  where commslabMob.OPERATOR_CODE == serviceprovider && mem.MEMBER_ROLE == 5 && detailscom.INTRODUCER_ID == Mem_ID
                                                  select new
                                                  {
                                                      SLN = commslabMob.SLN,
                                                      SLAB_ID = commslabMob.SLAB_ID,
                                                      commPer = commslabMob.MERCHANT_COM_PER,
                                                      commType = commslabMob.COMMISSION_TYPE,
                                                      SlabTDS= WLPCom.SLAB_TDS
                                                  }).FirstOrDefaultAsync();
                        #region WhiteLabel Service Charge Add
                        //var WLCOMM =await (from WLCOMTag in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                        //              join WLCOMMVAL in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //              on WLCOMTag.SLAB_ID equals WLCOMMVAL.RECHARGE_SLAB
                        //              where WLCOMMVAL.WHITE_LEVEL_ID == WHT_MEM_ID && WLCOMTag.OPERATOR_CODE == serviceprovider
                        //              select new
                        //              {
                        //                  COMMTYPE = WLCOMTag.COMMISSION_TYPE,
                        //                  COMM_VALUE = WLCOMTag.COMM_PERCENTAGE
                        //              }).FirstOrDefaultAsync();

                        var WLCOMM = await (from WLLMain in db.TBL_WHITE_LEVEL_COMMISSION_SLAB join WLCOMTag in db.TBL_COMM_SLAB_MOBILE_RECHARGE on WLLMain.SLN equals WLCOMTag.SLAB_ID
                                            join WLCOMMVAL in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                                            on WLCOMTag.SLAB_ID equals WLCOMMVAL.RECHARGE_SLAB
                                            where WLCOMMVAL.WHITE_LEVEL_ID == WHT_MEM_ID && WLCOMTag.OPERATOR_CODE == serviceprovider
                                            select new
                                            {
                                                COMMTYPE = WLCOMTag.COMMISSION_TYPE,
                                                COMM_VALUE = WLCOMTag.COMM_PERCENTAGE,
                                                SLAB_TDS= WLLMain.SLAB_TDS
                                            }).FirstOrDefaultAsync();

                        decimal WLserviceAmtAdd = 0;
                        decimal AddWL_ServiceAmtAdd = 0;
                        decimal WLService_AMT=0;
                        decimal ServiceAddAmt = 0;
                        //var WL_Wallet =await db.TBL_ACCOUNTS.FirstOrDefaultAsync(x => x.MEM_ID == WHT_MEM_ID);
                        var WL_Wallet = await db.TBL_ACCOUNTS.Where(x => x.MEM_ID == WHT_MEM_ID).OrderByDescending(z => z.ACC_NO).FirstOrDefaultAsync(); ;
                        //////var MER_ACCOUNT = await db.TBL_ACCOUNTS.Where(x => x.MEM_ID == transinfo.FROM_MEMBER).OrderByDescending(z => z.ACC_NO).FirstOrDefaultAsync();
                        if (WL_Wallet != null)
                        {
                            WLserviceAmtAdd = WL_Wallet.CLOSING;
                            WLService_AMT = WLCOMM.COMM_VALUE;
                            if (WLCOMM.COMMTYPE == "FIXED")
                            {
                                if (WLCOMM.SLAB_TDS == "Yes")
                                {
                                    WLLMAIn_TDSVAL = decimal.Round(((WLService_AMT * GST_AMount) / 100), 2);
                                    //ServiceAddAmt = WLService_AMT;
                                    ServiceAddAmt = WLService_AMT - WLLMAIn_TDSVAL;
                                    AddWL_ServiceAmtAdd = WLserviceAmtAdd + ServiceAddAmt;
                                }
                                else
                                {
                                    WLLMAIn_TDSVAL = 0;
                                    //ServiceAddAmt = WLService_AMT - WLLMAIn_TDSVAL;
                                    //ServiceAddAmt = WLService_AMT;
                                    ServiceAddAmt = WLService_AMT - WLLMAIn_TDSVAL;
                                    AddWL_ServiceAmtAdd = WLserviceAmtAdd + ServiceAddAmt - WLLMAIn_TDSVAL;
                                }

                            }
                            else
                            {
                                if (WLCOMM.SLAB_TDS == "Yes")
                                {
                                    CALCUWLLPCOMM = (Trans_Amt * WLService_AMT) / 100;

                                    //ServiceAddAmt = (Trans_Amt * WLService_AMT) / 100;
                                    //WLLMAIn_TDSVAL = decimal.Round(((ServiceAddAmt * GST_AMount) / 100), 2);
                                    WLLMAIn_TDSVAL = decimal.Round(((CALCUWLLPCOMM * GST_AMount) / 100), 2);
                                    ServiceAddAmt = CALCUWLLPCOMM - WLLMAIn_TDSVAL;
                                    //AddWL_ServiceAmtAdd = WLserviceAmtAdd + ServiceAddAmt;
                                    AddWL_ServiceAmtAdd = WLserviceAmtAdd + CALCUWLLPCOMM - WLLMAIn_TDSVAL;
                                }
                                else
                                {
                                    ServiceAddAmt = (Trans_Amt * WLService_AMT) / 100;
                                    WLLMAIn_TDSVAL = 0;
                                    AddWL_ServiceAmtAdd = WLserviceAmtAdd + ServiceAddAmt - WLLMAIn_TDSVAL;
                                }

                            }
                            TBL_ACCOUNTS WLobjval = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = (long)WHT_MEM_ID,
                                MEMBER_TYPE = "WHITE LEVEL",
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = ServiceAddAmt,
                                NARRATION = "Cash Back To WLP",
                                OPENING = WL_Wallet.CLOSING,
                                CLOSING = AddWL_ServiceAmtAdd,
                                REC_NO = 0,
                                COMM_AMT = ServiceAddAmt,
                                GST = 0,
                                TDS = (double)WLLMAIn_TDSVAL,
                                IPAddress = IpAddress,
                                TDS_PERCENTAGE = 0,
                                GST_PERCENTAGE = 0,
                                WHITELEVEL_ID = (long)WHT_MEM_ID,
                                SUPER_ID = 0,
                                DISTRIBUTOR_ID = (long)DIS_MEM_ID,
                                SERVICE_ID = Oper_Id_Val,
                                CORELATIONID = sRandomOTP
                            };
                            db.TBL_ACCOUNTS.Add(WLobjval);
                            await db.SaveChangesAsync();
                            decimal Update_WhiteLBLBal = 0;
                            decimal WLAvaiBal = 0;
                            var UpdateWLBalance = await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == WHT_MEM_ID);
                            if (UpdateWLBalance != null)
                            {
                                decimal.TryParse(UpdateWLBalance.BALANCE.ToString(), out WLAvaiBal);
                                //Update_WhiteLBLBal = WLAvaiBal + AddWL_ServiceAmtAdd;
                                Update_WhiteLBLBal = WLAvaiBal + ServiceAddAmt;
                                UpdateWLBalance.BALANCE = Update_WhiteLBLBal;
                                db.Entry(UpdateWLBalance).State = System.Data.Entity.EntityState.Modified;
                                await db.SaveChangesAsync();
                            }
                            else
                            {
                                UpdateWLBalance.BALANCE = WLAvaiBal;
                                db.Entry(UpdateWLBalance).State = System.Data.Entity.EntityState.Modified;
                                await db.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            WLserviceAmtAdd = 0;
                            WLService_AMT = WLCOMM.COMM_VALUE;
                            if (WLCOMM.COMMTYPE == "FIXED")
                            {
                                if (WLCOMM.SLAB_TDS == "Yes")
                                {
                                    WLLMAIn_TDSVAL = decimal.Round(((WLService_AMT * GST_AMount) / 100), 2);
                                    //ServiceAddAmt = WLService_AMT;
                                    ServiceAddAmt = WLService_AMT - WLLMAIn_TDSVAL;
                                    AddWL_ServiceAmtAdd = WLserviceAmtAdd + ServiceAddAmt;
                                }
                                else
                                {
                                    WLLMAIn_TDSVAL = 0;
                                    //ServiceAddAmt = WLService_AMT - WLLMAIn_TDSVAL;
                                    //ServiceAddAmt = WLService_AMT;
                                    ServiceAddAmt = WLService_AMT - WLLMAIn_TDSVAL;
                                    AddWL_ServiceAmtAdd = WLserviceAmtAdd + ServiceAddAmt - WLLMAIn_TDSVAL;
                                }

                            }
                            else
                            {
                                if (WLCOMM.SLAB_TDS == "Yes")
                                {
                                    CALCUWLLPCOMM = (Trans_Amt * WLService_AMT) / 100;

                                    //ServiceAddAmt = (Trans_Amt * WLService_AMT) / 100;
                                    //WLLMAIn_TDSVAL = decimal.Round(((ServiceAddAmt * GST_AMount) / 100), 2);
                                    WLLMAIn_TDSVAL = decimal.Round(((CALCUWLLPCOMM * GST_AMount) / 100), 2);
                                    ServiceAddAmt = CALCUWLLPCOMM - WLLMAIn_TDSVAL;
                                    //AddWL_ServiceAmtAdd = WLserviceAmtAdd + ServiceAddAmt;
                                    AddWL_ServiceAmtAdd = WLserviceAmtAdd + CALCUWLLPCOMM - WLLMAIn_TDSVAL;
                                }
                                else
                                {
                                    ServiceAddAmt = (Trans_Amt * WLService_AMT) / 100;
                                    WLLMAIn_TDSVAL = 0;
                                    AddWL_ServiceAmtAdd = WLserviceAmtAdd + ServiceAddAmt - WLLMAIn_TDSVAL;
                                }

                            }
                            TBL_ACCOUNTS WLobjval = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = (long)WHT_MEM_ID,
                                MEMBER_TYPE = "WHITE LEVEL",
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = ServiceAddAmt,
                                NARRATION = "Cash Back To WLP",
                                OPENING = 0,
                                CLOSING = AddWL_ServiceAmtAdd,
                                REC_NO = 0,
                                COMM_AMT = ServiceAddAmt,
                                GST = 0,
                                TDS = (double)WLLMAIn_TDSVAL,
                                IPAddress = IpAddress,
                                TDS_PERCENTAGE = 0,
                                GST_PERCENTAGE = 0,
                                WHITELEVEL_ID = (long)WHT_MEM_ID,
                                SUPER_ID = 0,
                                DISTRIBUTOR_ID = (long)DIS_MEM_ID,
                                SERVICE_ID = Oper_Id_Val,
                                CORELATIONID = sRandomOTP
                            };
                            db.TBL_ACCOUNTS.Add(WLobjval);
                            await db.SaveChangesAsync();
                            decimal Update_WhiteLBLBal = 0;
                            decimal WLAvaiBal = 0;
                            var UpdateWLBalance = await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == WHT_MEM_ID);
                            if (UpdateWLBalance != null)
                            {
                                decimal.TryParse(UpdateWLBalance.BALANCE.ToString(), out WLAvaiBal);
                                //Update_WhiteLBLBal = WLAvaiBal + AddWL_ServiceAmtAdd;
                                Update_WhiteLBLBal = WLAvaiBal + ServiceAddAmt;
                                UpdateWLBalance.BALANCE = Update_WhiteLBLBal;
                                db.Entry(UpdateWLBalance).State = System.Data.Entity.EntityState.Modified;
                                await db.SaveChangesAsync();
                            }
                            else
                            {
                                UpdateWLBalance.BALANCE = WLAvaiBal;
                                db.Entry(UpdateWLBalance).State = System.Data.Entity.EntityState.Modified;
                                await db.SaveChangesAsync();
                            }
                        }

                        #endregion
                        #region Retailer Commission                        
                        var membtype = await (from mm in db.TBL_MASTER_MEMBER
                                        join
                                            rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                        where mm.MEM_ID == Mem_ID
                                        select new
                                        {
                                            StateId=mm.STATE_ID,
                                            RoleId = mm.MEMBER_ROLE,
                                            roleName = rm.ROLE_NAME,
                                            Amount = mm.BALANCE
                                        }).FirstOrDefaultAsync();
                        //var tbl_account = await db.TBL_ACCOUNTS.Where(z => z.MEM_ID == Mem_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefaultAsync();
                        var tbl_account = await db.TBL_ACCOUNTS.Where(z => z.MEM_ID == Mem_ID).OrderByDescending(z => z.ACC_NO).FirstOrDefaultAsync();
                        if (tbl_account != null)
                        {
                            #region MyRegion                            
                            var MerGSTSetVal = (from GstMem in db.TBL_MASTER_MEMBER
                                                join StGstVal in db.TBL_STATES on GstMem.STATE_ID equals StGstVal.STATEID
                                                where GstMem.MEM_ID == Mem_ID
                                                select new
                                                {
                                                    StateGstVal = StGstVal.SGST,
                                                    CentralGstVal = StGstVal.CGST,
                                                    IGstVal = StGstVal.IGST
                                                }).FirstOrDefault();
                            decimal MembStateGstVal = 0;
                            decimal MembCentralGstVal = 0;
                            decimal MembIntsGstVal = 0;
                            decimal.TryParse(MerGSTSetVal.StateGstVal.ToString(),out MembStateGstVal);
                            decimal.TryParse(MerGSTSetVal.CentralGstVal.ToString(), out MembCentralGstVal);
                            decimal.TryParse(MerGSTSetVal.IGstVal.ToString(), out MembIntsGstVal);
                            decimal MembCAlCStateGSt = 0;
                            decimal MembCAlCCenterGSt = 0;
                            decimal MembCAlCInstGSt = 0;
                            #endregion
                            decimal ClosingAmt = tbl_account.CLOSING;
                            decimal SubAmt = ClosingAmt - Trans_Amt;                           
                            decimal getPer_Val = 0;
                          
                            //float FloatgetPer_Val = float.Parse(getPer_Val.ToString());
                            // Get TDS and GST
                            decimal RetailerTDS_AmtValue = 0;
                            decimal getPer = 0;
                            decimal GStCal = 0;
                            decimal CalculateGST = 0;
                            decimal.TryParse(MerchantComm.commPer.ToString(), out MerComm_Slab_Amt);
                            if (MerchantComm.commType == "FIXED")
                            {
                                getPer_Val = MerchantComm.commPer;
                                if (MerchantComm.SlabTDS == "Yes")
                                {
                                    if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                    {

                                        RetailerTDS_AmtValue = decimal.Round(((getPer_Val * GST_AMount) / 100), 2);
                                        if (MemberGST != null)
                                        {
                                            MembCAlCStateGSt = Math.Round(((getPer_Val * MembStateGstVal) / 100), 2);
                                            MembCAlCCenterGSt = Math.Round(((getPer_Val * MembCentralGstVal) / 100), 2);
                                            MembCAlCInstGSt = Math.Round(((getPer_Val * MembIntsGstVal) / 100), 2);
                                            //CalculateGST = decimal.Round(((getPer_Val * gst_VAlue) / 100), 2);                                        
                                            getPer = getPer_Val - RetailerTDS_AmtValue;
                                        }
                                        else
                                        {
                                            //CalculateGST = 0;                                        
                                            MembCAlCStateGSt = 0;
                                            MembCAlCCenterGSt = 0;
                                            MembCAlCInstGSt = 0;
                                            getPer = getPer_Val - RetailerTDS_AmtValue;
                                        }
                                    }
                                    else
                                    {
                                        RetailerTDS_AmtValue = decimal.Round(((getPer_Val - GST_AMount)));
                                        if (MemberGST != null)
                                        {
                                            MembCAlCStateGSt = Math.Round(((getPer_Val - MembStateGstVal)), 2);
                                            MembCAlCCenterGSt = Math.Round(((getPer_Val - MembCentralGstVal)), 2);
                                            MembCAlCInstGSt = Math.Round(((getPer_Val - MembIntsGstVal)), 2);
                                            //CalculateGST = decimal.Round(((getPer_Val - gst_VAlue)));                                        
                                            getPer = getPer_Val - RetailerTDS_AmtValue;
                                        }
                                        else
                                        {
                                            //CalculateGST = 0;
                                            MembCAlCStateGSt = 0;
                                            MembCAlCCenterGSt = 0;
                                            MembCAlCInstGSt = 0;
                                            getPer = getPer_Val - RetailerTDS_AmtValue;
                                        }
                                    }
                                }
                                else
                                {
                                    //RetailerTDS_AmtValue = decimal.Round(((getPer_Val - GST_AMount)));
                                    RetailerTDS_AmtValue =0;
                                    if (MemberGST != null)
                                    {
                                        MembCAlCStateGSt = Math.Round(((getPer_Val - MembStateGstVal)), 2);
                                        MembCAlCCenterGSt = Math.Round(((getPer_Val - MembCentralGstVal)), 2);
                                        MembCAlCInstGSt = Math.Round(((getPer_Val - MembIntsGstVal)), 2);
                                        //CalculateGST = decimal.Round(((getPer_Val - gst_VAlue)));                                        
                                        getPer = getPer_Val - RetailerTDS_AmtValue;
                                    }
                                    else
                                    {
                                        //CalculateGST = 0;
                                        MembCAlCStateGSt = 0;
                                        MembCAlCCenterGSt = 0;
                                        MembCAlCInstGSt = 0;
                                        getPer = getPer_Val - RetailerTDS_AmtValue;
                                    }
                                }                                
                            }
                            else
                            {
                                getPer_Val = (Trans_Amt * MerchantComm.commPer) / 100;
                                if (MerchantComm.SlabTDS == "Yes")
                                {
                                    if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                    {

                                        RetailerTDS_AmtValue = decimal.Round(((getPer_Val * GST_AMount) / 100), 2);
                                        if (MemberGST != null)
                                        {
                                            MembCAlCStateGSt = Math.Round(((getPer_Val * MembStateGstVal) / 100), 2);
                                            MembCAlCCenterGSt = Math.Round(((getPer_Val * MembCentralGstVal) / 100), 2);
                                            MembCAlCInstGSt = Math.Round(((getPer_Val * MembIntsGstVal) / 100), 2);
                                            //CalculateGST = decimal.Round(((getPer_Val * gst_VAlue) / 100), 2);                                        
                                            getPer = getPer_Val - RetailerTDS_AmtValue;
                                        }
                                        else
                                        {
                                            //CalculateGST = 0;
                                            MembCAlCStateGSt = 0;
                                            MembCAlCCenterGSt = 0;
                                            MembCAlCInstGSt = 0;
                                            getPer = getPer_Val - RetailerTDS_AmtValue;
                                        }
                                    }
                                    else
                                    {
                                        RetailerTDS_AmtValue = decimal.Round(((getPer_Val - GST_AMount)));
                                        if (MemberGST != null)
                                        {
                                            //CalculateGST = decimal.Round(((getPer_Val - gst_VAlue)));
                                            MembCAlCStateGSt = Math.Round(((getPer_Val - MembStateGstVal)), 2);
                                            MembCAlCCenterGSt = Math.Round(((getPer_Val - MembCentralGstVal)), 2);
                                            MembCAlCInstGSt = Math.Round(((getPer_Val - MembIntsGstVal)), 2);
                                            getPer = getPer_Val - RetailerTDS_AmtValue;
                                        }
                                        else
                                        {
                                            //CalculateGST = 0;

                                            MembCAlCStateGSt = 0;
                                            MembCAlCCenterGSt = 0;
                                            MembCAlCInstGSt = 0;
                                            getPer = getPer_Val - RetailerTDS_AmtValue;
                                        }
                                    }
                                }
                                else
                                {
                                    //RetailerTDS_AmtValue = decimal.Round(((getPer_Val - GST_AMount)));
                                    RetailerTDS_AmtValue = 0;
                                    if (MemberGST != null)
                                    {
                                        //CalculateGST = decimal.Round(((getPer_Val - gst_VAlue)));
                                        MembCAlCStateGSt = Math.Round(((getPer_Val - MembStateGstVal)), 2);
                                        MembCAlCCenterGSt = Math.Round(((getPer_Val - MembCentralGstVal)), 2);
                                        MembCAlCInstGSt = Math.Round(((getPer_Val - MembIntsGstVal)), 2);
                                        getPer = getPer_Val - RetailerTDS_AmtValue;
                                    }
                                    else
                                    {
                                        //CalculateGST = 0;

                                        MembCAlCStateGSt = 0;
                                        MembCAlCCenterGSt = 0;
                                        MembCAlCInstGSt = 0;
                                        getPer = getPer_Val - RetailerTDS_AmtValue;
                                    }
                                }
                                //RetailerTDS_AmtValue = decimal.Round(((getPer_Val * GST_AMount) / 100), 2);
                                //getPer = getPer_Val - RetailerTDS_AmtValue;
                            }
                            //float RetailerTDS_AmtValue = (FloatgetPer_Val * GST_AMount) / 100;
                            //double getPer = float.Parse(getPer_Val) - RetailerTDS_AmtValue;
                            MER_Comm_Amt = getPer;
                            MER_TaxAMT = RetailerTDS_AmtValue;
                            //MER_GST_Comm_Amt = CalculateGST;
                            MER_GST_Comm_Amt = (MembCAlCStateGSt+ MembCAlCCenterGSt+ MembCAlCInstGSt);
                            decimal CommAddClosing = SubAmt + getPer;
                            TBL_ACCOUNTS objCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Mem_ID,
                                MEMBER_TYPE = membtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = getPer,
                                NARRATION = "Commission To Retailer",
                                OPENING = SubAmt,
                                CLOSING = CommAddClosing,
                                REC_NO = 0,
                                COMM_AMT = getPer,
                                //GST = (float)CalculateGST,
                                GST = (float)MER_GST_Comm_Amt,
                                TDS = (float)RetailerTDS_AmtValue,
                                IPAddress = IpAddress,
                                TDS_PERCENTAGE = taxMaster.TAX_VALUE,
                                GST_PERCENTAGE = GST_Master.TAX_VALUE,
                                WHITELEVEL_ID = (long)WHT_MEM_ID,
                                //SUPER_ID = (long)SUP_MEM_ID,
                                SUPER_ID = 0,
                                DISTRIBUTOR_ID = (long)DIS_MEM_ID,
                                SERVICE_ID = Oper_Id_Val,
                                CORELATIONID = sRandomOTP,
                                REC_COMM_TYPE = MerchantComm.commType,
                                COMM_VALUE = MerchantComm.commPer,
                                NET_COMM_AMT = MER_Comm_Amt,
                                TDS_DR_COMM_AMT = MER_TaxAMT,
                                CGST_COMM_AMT_INPUT = MembCAlCCenterGSt,
                                CGST_COMM_AMT_OUTPUT = 0,
                                SGST_COMM_AMT_INPUT = MembCAlCStateGSt,
                                SGST_COMM_AMT_OUTPUT = 0,
                                IGST_COMM_AMT_INPUT = MembCAlCInstGSt,
                                IGST_COMM_AMT_OUTPUT = 0,
                                TOTAL_GST_COMM_AMT_INPUT = MER_GST_Comm_Amt,
                                TOTAL_GST_COMM_AMT_OUTPUT = 0,
                                TDS_RATE = GST_AMount,
                                CGST_RATE= MembCentralGstVal,
                                SGST_RATE= MembStateGstVal,
                                IGST_RATE= MembIntsGstVal,
                                TOTAL_GST_RATE=(MembCentralGstVal+ MembStateGstVal+ MembIntsGstVal),
                                COMM_SLAB_ID= Oper_Id_Val,
                                STATE_ID= membtype.StateId,
                                FLAG1=0,
                                FLAG2=0,
                                FLAG3=0,
                                FLAG4=0,
                                FLAG5=0,
                                FLAG6=0,
                                FLAG7=0,
                                FLAG8=0,
                                FLAG9=0,
                                FLAG10=0,
                                VENDOR_ID= VendorInfo.ID
                            };
                            db.TBL_ACCOUNTS.Add(objCommPer);
                            await db.SaveChangesAsync();

                            decimal MerBal = 0;
                            decimal UpdateMerBal = 0;
                            var MechntBal =await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == Mem_ID);
                            if (MechntBal != null)
                            {
                                decimal.TryParse(MechntBal.BALANCE.ToString(), out MerBal);
                                UpdateMerBal = MerBal + getPer;
                                MechntBal.BALANCE = UpdateMerBal;
                                db.Entry(MechntBal).State = System.Data.Entity.EntityState.Modified;
                                await db.SaveChangesAsync();
                            }
                            else
                            {
                                MechntBal.BALANCE = MerBal;
                                db.Entry(MechntBal).State = System.Data.Entity.EntityState.Modified;
                                await db.SaveChangesAsync();
                            }

                        }
                        #endregion

                        //var DistributorComm =await (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                        //                       join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                        //                       on detailscom.SLN equals commslabMob.SLAB_ID
                        //                       join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                        //                       where commslabMob.OPERATOR_CODE == serviceprovider && detailscom.SLAB_TYPE == 1 && mem.MEMBER_ROLE == 4 && commslabMob.MEM_ID == WHT_MEM_ID
                        //                            select new
                        //                       {
                        //                           SLN = commslabMob.SLN,
                        //                           commPer = commslabMob.DISTRIBUTOR_COM_PER,
                        //                           commType = commslabMob.COMMISSION_TYPE
                        //                       }).FirstOrDefaultAsync();

                        var DistributorComm = await (from WLPCom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                                  join detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB on WLPCom.SLN equals detailscom.RECHARGE_SLAB
                                                  join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                                  on detailscom.RECHARGE_SLAB equals commslabMob.SLAB_ID
                                                  join mem in db.TBL_MASTER_MEMBER on detailscom.INTRODUCER_ID equals mem.MEM_ID
                                                  where commslabMob.OPERATOR_CODE == serviceprovider && mem.MEMBER_ROLE == 4 && detailscom.INTRODUCER_ID == DIS_MEM_ID
                                                     select new
                                                  {
                                                      SLN = commslabMob.SLN,
                                                      SLAB_ID = commslabMob.SLAB_ID,
                                                      commPer = commslabMob.DISTRIBUTOR_COM_PER,
                                                      commType = commslabMob.COMMISSION_TYPE,
                                                      SlabTDS = WLPCom.SLAB_TDS
                                                  }).FirstOrDefaultAsync();

                        //var DistributorComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                       join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                        //                       on detailscom.RECHARGE_SLAB equals commslabMob.SLAB_ID
                        //                       where detailscom.WHITE_LEVEL_ID == SUP_MEM_ID && detailscom.INTRODUCER_ID == DIS_MEM_ID && commslabMob.OPERATOR_CODE == serviceprovider
                        //                       select new
                        //                       {
                        //                           SLN = commslabMob.SLN,
                        //                           commPer = commslabMob.COMM_PERCENTAGE
                        //                       }).FirstOrDefault();
                        #region Distributor Commission
                        //decimal DisGapComm = decimal.Parse(DistributorComm.commPer.ToString()) - decimal.Parse(MerchantComm.commPer.ToString());

                        //var tbl_accountDistributor =await db.TBL_ACCOUNTS.Where(z => z.MEM_ID == DIS_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefaultAsync();
                        var tbl_accountDistributor = await db.TBL_ACCOUNTS.Where(z => z.MEM_ID == DIS_MEM_ID).OrderByDescending(z => z.ACC_NO).FirstOrDefaultAsync();
                        if (tbl_accountDistributor != null)
                        {
                            var CheckDisCommission =await db.TBL_DETAILS_MEMBER_COMMISSION_SLAB.Where(x => x.INTRODUCER_ID == DIS_MEM_ID).FirstOrDefaultAsync();
                            var Dismembtype =await (from mm in db.TBL_MASTER_MEMBER
                                               join
                                                   rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                               where mm.MEM_ID == DIS_MEM_ID
                                               select new
                                               {
                                                   StateId=mm.STATE_ID,
                                                   RoleId = mm.MEMBER_ROLE,
                                                   roleName = rm.ROLE_NAME,
                                                   Amount = mm.BALANCE
                                               }).FirstOrDefaultAsync();
                            decimal DisClosingAmt = tbl_accountDistributor.CLOSING;


                            #region Distributor Commission
                            var DistGSTSetVal = (from GstMem in db.TBL_MASTER_MEMBER
                                                join StGstVal in db.TBL_STATES on GstMem.STATE_ID equals StGstVal.STATEID
                                                where GstMem.MEM_ID == DIS_MEM_ID
                                                 select new
                                                {
                                                    StateGstVal = StGstVal.SGST,
                                                    CentralGstVal = StGstVal.CGST,
                                                    IGstVal = StGstVal.IGST
                                                }).FirstOrDefault();
                            decimal DistStateGstVal = 0;
                            decimal DistCentralGstVal = 0;
                            decimal DistIntsGstVal = 0;
                            decimal.TryParse(DistGSTSetVal.StateGstVal.ToString(), out DistStateGstVal);
                            decimal.TryParse(DistGSTSetVal.CentralGstVal.ToString(), out DistCentralGstVal);
                            decimal.TryParse(DistGSTSetVal.IGstVal.ToString(), out DistIntsGstVal);
                            decimal DistCAlCStateGSt = 0;
                            decimal DistCAlCCenterGSt = 0;
                            decimal DistCAlCInstGSt = 0;
                            #endregion


                            decimal DisGapComm = 0;                            
                            decimal DisGapCommAmt_Val = 0;
                            decimal DisGapCommAmt_TDS = 0;
                            decimal DisGapCommAmt = 0;
                            
                            decimal DIS_GST_Amt = 0;
                            decimal Dis_Calculate_GST = 0;
                            
                            decimal.TryParse(DistributorComm.commPer.ToString(), out DISTRIBUTOR_GAPP_CoMMISSIOM);
                            DisGapComm = DISTRIBUTOR_GAPP_CoMMISSIOM - MerComm_Slab_Amt;
                            if (DistributorComm.commType == "FIXED")
                            {
                                if (DistributorComm.SlabTDS == "Yes")
                                {
                                    if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                    {
                                        ////DisGapComm = decimal.Parse(DistributorComm.commPer.ToString());
                                        DisGapCommAmt_Val = DisGapComm;
                                        DisGapCommAmt_TDS = decimal.Round(((DisGapCommAmt_Val * GST_AMount) / 100), 2);
                                        if (MemberGST != null)
                                        {
                                            //Dis_Calculate_GST = decimal.Round(((DisGapCommAmt_Val * gst_VAlue) / 100), 2);

                                            DistCAlCStateGSt = Math.Round(((DisGapCommAmt_Val * DistStateGstVal) / 100), 2);
                                            DistCAlCCenterGSt = Math.Round(((DisGapCommAmt_Val * DistCentralGstVal) / 100), 2);
                                            DistCAlCInstGSt = Math.Round(((DisGapCommAmt_Val * DistIntsGstVal) / 100), 2);

                                            DisGapCommAmt = DisGapCommAmt_Val - DisGapCommAmt_TDS;
                                        }
                                        else
                                        {
                                            //Dis_Calculate_GST = 0;
                                            DistCAlCStateGSt = 0;
                                            DistCAlCCenterGSt = 0;
                                            DistCAlCInstGSt = 0;
                                            DisGapCommAmt = DisGapCommAmt_Val - DisGapCommAmt_TDS;
                                        }
                                    }
                                    else
                                    {
                                        ////DisGapComm = decimal.Parse(DistributorComm.commPer.ToString());
                                        DisGapCommAmt_Val = DisGapComm;
                                        DisGapCommAmt_TDS = decimal.Round(((DisGapCommAmt_Val - GST_AMount)));
                                        if (MemberGST != null)
                                        {
                                            //Dis_Calculate_GST = decimal.Round(((DisGapCommAmt_Val - gst_VAlue)));
                                            DistCAlCStateGSt = Math.Round(((DisGapCommAmt_Val - DistStateGstVal)), 2);
                                            DistCAlCCenterGSt = Math.Round(((DisGapCommAmt_Val - DistCentralGstVal)), 2);
                                            DistCAlCInstGSt = Math.Round(((DisGapCommAmt_Val - DistIntsGstVal)), 2);
                                            DisGapCommAmt = DisGapCommAmt_Val - DisGapCommAmt_TDS;
                                        }
                                        else
                                        {
                                            //Dis_Calculate_GST = 0;
                                            DistCAlCStateGSt = 0;
                                            DistCAlCCenterGSt = 0;
                                            DistCAlCInstGSt = 0;
                                            DisGapCommAmt = DisGapCommAmt_Val - DisGapCommAmt_TDS;
                                        }

                                    }
                                }
                                else
                                {////DisGapComm = decimal.Parse(DistributorComm.commPer.ToString());
                                    DisGapCommAmt_Val = DisGapComm;
                                    //DisGapCommAmt_TDS = decimal.Round(((DisGapCommAmt_Val - GST_AMount)));
                                    DisGapCommAmt_TDS = 0;
                                    if (MemberGST != null)
                                    {
                                        //Dis_Calculate_GST = decimal.Round(((DisGapCommAmt_Val - gst_VAlue)));
                                        DistCAlCStateGSt = Math.Round(((DisGapCommAmt_Val - DistStateGstVal)), 2);
                                        DistCAlCCenterGSt = Math.Round(((DisGapCommAmt_Val - DistCentralGstVal)), 2);
                                        DistCAlCInstGSt = Math.Round(((DisGapCommAmt_Val - DistIntsGstVal)), 2);
                                        DisGapCommAmt = DisGapCommAmt_Val - DisGapCommAmt_TDS;
                                    }
                                    else
                                    {
                                        //Dis_Calculate_GST = 0;
                                        DistCAlCStateGSt = 0;
                                        DistCAlCCenterGSt = 0;
                                        DistCAlCInstGSt = 0;
                                        DisGapCommAmt = DisGapCommAmt_Val - DisGapCommAmt_TDS;
                                    }
                                }
                               
                            }
                            else
                            {
                                ////DisGapComm = decimal.Parse(DistributorComm.commPer.ToString());
                                //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;
                                if (DistributorComm.SlabTDS == "Yes")
                                {
                                    DisGapCommAmt_Val = (Trans_Amt * DisGapComm) / 100;
                                    if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                    {
                                        DisGapCommAmt_TDS = decimal.Round(((DisGapCommAmt_Val * GST_AMount) / 100), 2);
                                        if (MemberGST != null)
                                        {
                                            //Dis_Calculate_GST = decimal.Round(((DisGapCommAmt_Val * gst_VAlue) / 100), 2);

                                            DistCAlCStateGSt = Math.Round(((DisGapCommAmt_Val * DistStateGstVal) / 100), 2);
                                            DistCAlCCenterGSt = Math.Round(((DisGapCommAmt_Val * DistCentralGstVal) / 100), 2);
                                            DistCAlCInstGSt = Math.Round(((DisGapCommAmt_Val * DistIntsGstVal) / 100), 2);

                                            DisGapCommAmt = DisGapCommAmt_Val - DisGapCommAmt_TDS;
                                        }
                                        else
                                        {
                                            //Dis_Calculate_GST = 0;
                                            DistCAlCStateGSt = 0;
                                            DistCAlCCenterGSt = 0;
                                            DistCAlCInstGSt = 0;
                                            DisGapCommAmt = DisGapCommAmt_Val - DisGapCommAmt_TDS;
                                        }
                                    }
                                    else
                                    {
                                        DisGapCommAmt_TDS = decimal.Round(((DisGapCommAmt_Val - GST_AMount)));
                                        if (MemberGST != null)
                                        {
                                            //Dis_Calculate_GST = decimal.Round(((DisGapCommAmt_Val - gst_VAlue)));
                                            DistCAlCStateGSt = Math.Round(((DisGapCommAmt_Val - DistStateGstVal)), 2);
                                            DistCAlCCenterGSt = Math.Round(((DisGapCommAmt_Val - DistCentralGstVal)), 2);
                                            DistCAlCInstGSt = Math.Round(((DisGapCommAmt_Val - DistIntsGstVal)), 2);
                                            DisGapCommAmt = DisGapCommAmt_Val - DisGapCommAmt_TDS;
                                        }
                                        else
                                        {
                                            //Dis_Calculate_GST = 0;
                                            DistCAlCStateGSt = 0;
                                            DistCAlCCenterGSt = 0;
                                            DistCAlCInstGSt = 0;
                                            DisGapCommAmt = DisGapCommAmt_Val - DisGapCommAmt_TDS;
                                        }
                                    }
                                }
                                else
                                {
                                    //DisGapCommAmt_TDS = decimal.Round(((DisGapCommAmt_Val - GST_AMount)));
                                    DisGapCommAmt_TDS = 0;
                                    if (MemberGST != null)
                                    {
                                        //Dis_Calculate_GST = decimal.Round(((DisGapCommAmt_Val - gst_VAlue)));
                                        DistCAlCStateGSt = Math.Round(((DisGapCommAmt_Val - DistStateGstVal)), 2);
                                        DistCAlCCenterGSt = Math.Round(((DisGapCommAmt_Val - DistCentralGstVal)), 2);
                                        DistCAlCInstGSt = Math.Round(((DisGapCommAmt_Val - DistIntsGstVal)), 2);
                                        DisGapCommAmt = DisGapCommAmt_Val - DisGapCommAmt_TDS;
                                    }
                                    else
                                    {
                                        //Dis_Calculate_GST = 0;
                                        DistCAlCStateGSt = 0;
                                        DistCAlCCenterGSt = 0;
                                        DistCAlCInstGSt = 0;
                                        DisGapCommAmt = DisGapCommAmt_Val - DisGapCommAmt_TDS;
                                    }
                                }
                            }
                            DEST_Comm_Amt = DisGapCommAmt;
                            DEST_TaxAMT = DisGapCommAmt_TDS;
                            //DIST_GST_Comm_Amt = Dis_Calculate_GST;
                            DIST_GST_Comm_Amt = (DistCAlCStateGSt+ DistCAlCCenterGSt + DistCAlCInstGSt);
                            decimal CommDisAddClosing = tbl_accountDistributor.CLOSING + DisGapCommAmt;
                            long Dis_ID = long.Parse(DIS_MEM_ID.ToString());
                            TBL_ACCOUNTS objDisCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Dis_ID,
                                MEMBER_TYPE = Dismembtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = DisGapCommAmt,
                                NARRATION = "Commission To Distributor",
                                OPENING = DisClosingAmt,
                                CLOSING = CommDisAddClosing,
                                REC_NO = 0,
                                COMM_AMT = DisGapCommAmt,
                                //GST = (float)Dis_Calculate_GST,
                                GST = (float)DIST_GST_Comm_Amt,
                                TDS = (float)DisGapCommAmt_TDS,
                                IPAddress = IpAddress,
                                TDS_PERCENTAGE = taxMaster.TAX_VALUE,
                                GST_PERCENTAGE = GST_Master.TAX_VALUE,
                                WHITELEVEL_ID = (long)WHT_MEM_ID,
                                //SUPER_ID = (long)SUP_MEM_ID,
                                SUPER_ID = 0,
                                DISTRIBUTOR_ID = (long)DIS_MEM_ID,
                                SERVICE_ID = Oper_Id_Val,
                                CORELATIONID = sRandomOTP,
                                REC_COMM_TYPE = DistributorComm.commType,
                                COMM_VALUE = DistributorComm.commPer,
                                NET_COMM_AMT = DEST_Comm_Amt,
                                TDS_DR_COMM_AMT = DEST_TaxAMT,
                                CGST_COMM_AMT_INPUT = DistCAlCCenterGSt,
                                CGST_COMM_AMT_OUTPUT = 0,
                                SGST_COMM_AMT_INPUT = DistCAlCStateGSt,
                                SGST_COMM_AMT_OUTPUT = 0,
                                IGST_COMM_AMT_INPUT = DistCAlCInstGSt,
                                IGST_COMM_AMT_OUTPUT = 0,
                                TOTAL_GST_COMM_AMT_INPUT = DIST_GST_Comm_Amt,
                                TOTAL_GST_COMM_AMT_OUTPUT = 0,
                                TDS_RATE = GST_AMount,
                                CGST_RATE = DistCentralGstVal,
                                SGST_RATE = DistStateGstVal,
                                IGST_RATE = DistIntsGstVal,
                                TOTAL_GST_RATE = (DistCentralGstVal + DistStateGstVal + DistIntsGstVal),
                                COMM_SLAB_ID = Oper_Id_Val,
                                STATE_ID = Dismembtype.StateId,
                                FLAG1 = 0,
                                FLAG2 = 0,
                                FLAG3 = 0,
                                FLAG4 = 0,
                                FLAG5 = 0,
                                FLAG6 = 0,
                                FLAG7 = 0,
                                FLAG8 = 0,
                                FLAG9 = 0,
                                FLAG10 = 0,
                                VENDOR_ID = VendorInfo.ID
                            };
                            db.TBL_ACCOUNTS.Add(objDisCommPer);
                            await db.SaveChangesAsync();


                            decimal DistBal = 0;
                            decimal UpdateDistBal = 0;
                            var DistributorBal = await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == Dis_ID);
                            if (DistributorBal != null)
                            {
                                decimal.TryParse(DistributorBal.BALANCE.ToString(), out DistBal);
                                UpdateDistBal = DistBal + DisGapCommAmt;
                                DistributorBal.BALANCE = UpdateDistBal;
                                db.Entry(DistributorBal).State = System.Data.Entity.EntityState.Modified;
                                await db.SaveChangesAsync();
                            }
                            else
                            {
                                DistributorBal.BALANCE = DistBal;
                                db.Entry(DistributorBal).State = System.Data.Entity.EntityState.Modified;
                                await db.SaveChangesAsync();
                            }
                        }
                        #endregion
                        var SuperComm = await (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                         join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                          on detailscom.SLN equals commslabMob.SLAB_ID
                                         join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                                         where commslabMob.OPERATOR_CODE == serviceprovider && detailscom.SLAB_TYPE == 1 && mem.MEMBER_ROLE == 3 && commslabMob.MEM_ID == WHT_MEM_ID
                                               select new
                                         {
                                             SLN = commslabMob.SLN,
                                             commPer = commslabMob.SUPER_COM_PER,
                                             commType = commslabMob.COMMISSION_TYPE
                                         }).FirstOrDefaultAsync();


                        #region Super Commission
                        //var tbl_accountSuper = await db.TBL_ACCOUNTS.Where(z => z.MEM_ID == SUP_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefaultAsync();
                        //if (tbl_accountSuper != null)
                        //{
                        //    var Supmembtype =await (from mm in db.TBL_MASTER_MEMBER
                        //                       join
                        //                           rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                        //                       where mm.MEM_ID == SUP_MEM_ID
                        //                       select new
                        //                       {
                        //                           RoleId = mm.MEMBER_ROLE,
                        //                           roleName = rm.ROLE_NAME,
                        //                           Amount = mm.BALANCE
                        //                       }).FirstOrDefaultAsync();
                        //    decimal SupClosingAmt = tbl_accountSuper.CLOSING;

                        //    decimal SupGapComm = 0;
                        //    //decimal SupGapCommAmt = (Trans_Amt * SupGapComm) / 100;
                        //    // GST and TDS Calculation for Super
                        //    decimal SupGapCommAmt_val = 0;
                        //    decimal Sp_TDSAmt = 0;
                        //    decimal SupGapCommAmt = 0;
                        //    decimal SUP_GstAmt = 0;
                        //    decimal Sup_CalculateGST = 0;
                        //    if (SuperComm.commType == "FIXED")
                        //    {
                        //        SupGapComm = decimal.Parse(SuperComm.commPer.ToString());
                        //        //decimal SupGapCommAmt = (Trans_Amt * SupGapComm) / 100;
                        //        // GST and TDS Calculation for Super
                        //        SupGapCommAmt_val = SupGapComm;
                        //        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                        //        {
                        //            Sp_TDSAmt = decimal.Round(((SupGapCommAmt_val * GST_AMount) / 100), 2);
                        //            if (MemberGST != null)
                        //            {
                        //                Sup_CalculateGST = decimal.Round(((SupGapCommAmt_val * gst_VAlue) / 100), 2);
                        //                //SupGapCommAmt = SupGapCommAmt_val + Sup_CalculateGST - Sp_TDSAmt;
                        //                SupGapCommAmt = SupGapCommAmt_val - Sp_TDSAmt;
                        //            }
                        //            else
                        //            {
                        //                Sup_CalculateGST = 0;
                        //                //SupGapCommAmt = SupGapCommAmt_val + Sup_CalculateGST - Sp_TDSAmt;
                        //                SupGapCommAmt = SupGapCommAmt_val - Sp_TDSAmt;
                        //            }
                        //        }
                        //        else
                        //        {
                        //            Sp_TDSAmt = decimal.Round(((SupGapCommAmt_val - GST_AMount)));
                        //            if (MemberGST != null)
                        //            {
                        //                Sup_CalculateGST = decimal.Round(((SupGapCommAmt_val - gst_VAlue)));
                        //                //SupGapCommAmt = SupGapCommAmt_val + Sup_CalculateGST - Sp_TDSAmt;
                        //                SupGapCommAmt = SupGapCommAmt_val - Sp_TDSAmt;
                        //            }
                        //            else
                        //            {
                        //                Sup_CalculateGST = 0;
                        //                //SupGapCommAmt = SupGapCommAmt_val + Sup_CalculateGST - Sp_TDSAmt;
                        //                SupGapCommAmt = SupGapCommAmt_val - Sp_TDSAmt;
                        //            }

                        //        }
                        //    }
                        //    else
                        //    {
                        //        SupGapComm = decimal.Parse(SuperComm.commPer.ToString());
                        //        //decimal SupGapCommAmt = (Trans_Amt * SupGapComm) / 100;
                        //        // GST and TDS Calculation for Super
                        //        SupGapCommAmt_val = (Trans_Amt * SupGapComm) / 100;
                        //        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                        //        {
                        //            Sp_TDSAmt = decimal.Round(((SupGapCommAmt_val * GST_AMount) / 100), 2);
                        //            if (MemberGST != null)
                        //            {
                        //                Sup_CalculateGST = decimal.Round(((SupGapCommAmt_val * gst_VAlue) / 100), 2);
                        //                //SupGapCommAmt = SupGapCommAmt_val + Sup_CalculateGST - Sp_TDSAmt;
                        //                SupGapCommAmt = SupGapCommAmt_val - Sp_TDSAmt;
                        //            }
                        //            else
                        //            {
                        //                Sup_CalculateGST = 0;
                        //                //SupGapCommAmt = SupGapCommAmt_val + Sup_CalculateGST - Sp_TDSAmt;
                        //                SupGapCommAmt = SupGapCommAmt_val - Sp_TDSAmt;
                        //            }
                        //        }
                        //        else
                        //        {
                        //            Sp_TDSAmt = decimal.Round(((SupGapCommAmt_val - GST_AMount)));
                        //            if (MemberGST != null)
                        //            {
                        //                Sup_CalculateGST = decimal.Round(((SupGapCommAmt_val - gst_VAlue)));
                        //                //SupGapCommAmt = SupGapCommAmt_val + Sup_CalculateGST - Sp_TDSAmt;
                        //                SupGapCommAmt = SupGapCommAmt_val - Sp_TDSAmt;
                        //            }
                        //            else
                        //            {
                        //                Sup_CalculateGST = 0;
                        //                //SupGapCommAmt = SupGapCommAmt_val + Sup_CalculateGST - Sp_TDSAmt;
                        //                SupGapCommAmt = SupGapCommAmt_val - Sp_TDSAmt;
                        //            }
                        //        }
                        //    }
                        //    SPR_Comm_Amt = SupGapCommAmt;
                        //    SPR_TaxAMT = Sp_TDSAmt;
                        //    SUPER_GST_Comm_Amt = Sup_CalculateGST;
                        //    decimal CommSupAddClosing = tbl_accountSuper.CLOSING + SupGapCommAmt;
                        //    long Sup_ID = long.Parse(SUP_MEM_ID.ToString());
                        //    TBL_ACCOUNTS objSupCommPer = new TBL_ACCOUNTS()
                        //    {
                        //        API_ID = 0,
                        //        MEM_ID = Sup_ID,
                        //        MEMBER_TYPE = Supmembtype.roleName,
                        //        TRANSACTION_TYPE = rechargeType,
                        //        TRANSACTION_DATE = System.DateTime.Now,
                        //        TRANSACTION_TIME = DateTime.Now,
                        //        DR_CR = "CR",
                        //        AMOUNT = SupGapCommAmt,
                        //        NARRATION = "Commission To Super",
                        //        OPENING = SupClosingAmt,
                        //        CLOSING = CommSupAddClosing,
                        //        REC_NO = 0,
                        //        COMM_AMT = SupGapCommAmt,
                        //        GST = (float)Sup_CalculateGST,
                        //        TDS = (float)Sp_TDSAmt,
                        //        IPAddress = IpAddress,
                        //        TDS_PERCENTAGE = taxMaster.TAX_VALUE,
                        //        GST_PERCENTAGE = GST_Master.TAX_VALUE,
                        //        WHITELEVEL_ID = (long)WHT_MEM_ID,
                        //        SUPER_ID = (long)SUP_MEM_ID,
                        //        DISTRIBUTOR_ID = (long)DIS_MEM_ID,
                        //        SERVICE_ID = Oper_Id_Val,
                        //        CORELATIONID = sRandomOTP
                        //    };
                        //    db.TBL_ACCOUNTS.Add(objSupCommPer);
                        //    await db.SaveChangesAsync();

                        //    decimal SprBal = 0;
                        //    decimal UpdatSprBal = 0;
                        //    var SuperBal =await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == Sup_ID);
                        //    if (SuperBal != null)
                        //    {
                        //        decimal.TryParse(SuperBal.BALANCE.ToString(), out SprBal);
                        //        UpdatSprBal = SprBal + SupGapCommAmt;
                        //        SuperBal.BALANCE = UpdatSprBal;
                        //        db.Entry(SuperBal).State = System.Data.Entity.EntityState.Modified;
                        //        await db.SaveChangesAsync();
                        //    }
                        //    else
                        //    {
                        //        SuperBal.BALANCE = SprBal;
                        //        db.Entry(SuperBal).State = System.Data.Entity.EntityState.Modified;
                        //        await db.SaveChangesAsync();
                        //    }

                        //}
                        #endregion


                        var WhiteComm = await (from WLPCom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                               join detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB on WLPCom.SLN equals detailscom.RECHARGE_SLAB
                                               join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                               on detailscom.RECHARGE_SLAB equals commslabMob.SLAB_ID
                                               join mem in db.TBL_MASTER_MEMBER on detailscom.WHITE_LEVEL_ID equals mem.MEM_ID
                                               where commslabMob.OPERATOR_CODE == serviceprovider && detailscom.INTRODUCER_ID == 0 && detailscom.WHITE_LEVEL_ID == WHT_MEM_ID
                                               select new
                                               {
                                                   SLN = commslabMob.SLN,
                                                   SLAB_ID = commslabMob.SLAB_ID,
                                                   commPer = commslabMob.COMM_PERCENTAGE,
                                                   commType = commslabMob.COMMISSION_TYPE,
                                                   SlabTDS = WLPCom.SLAB_TDS
                                               }).FirstOrDefaultAsync();

                        //var WhiteComm = await (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                 join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                        //                  on detailscom.RECHARGE_SLAB equals commslabMob.SLAB_ID
                        //                 where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == 0 && commslabMob.OPERATOR_CODE == serviceprovider
                        //                 select new
                        //                 {
                        //                     SLN = commslabMob.SLN,
                        //                     commPer = commslabMob.COMM_PERCENTAGE,
                        //                     commType = commslabMob.COMMISSION_TYPE
                        //                 }).FirstOrDefaultAsync();

                        //decimal WLComm = decimal.Parse(MerchantComm.commPer.ToString()) + decimal.Parse(DistributorComm.commPer.ToString()) + decimal.Parse(SuperComm.commPer.ToString());
                        decimal WLComm = decimal.Parse(MerchantComm.commPer.ToString()) + decimal.Parse(DistributorComm.commPer.ToString()) ;
                        //decimal WTLGapComm = decimal.Parse(WhiteComm.commPer.ToString());
                        //decimal WTLGapComm =WLComm;

                        #region White level Commission payment Structure
                        //var tbl_accountWhiteLevel =await db.TBL_ACCOUNTS.Where(z => z.MEM_ID == WHT_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefaultAsync();
                        var tbl_accountWhiteLevel = await db.TBL_ACCOUNTS.Where(z => z.MEM_ID == WHT_MEM_ID).OrderByDescending(z => z.ACC_NO).FirstOrDefaultAsync();
                        if (tbl_accountWhiteLevel != null)
                        {
                            var WLmembtype =await (from mm in db.TBL_MASTER_MEMBER
                                              join
                                                  rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                              where mm.MEM_ID == WHT_MEM_ID
                                              select new
                                              {
                                                  Stateid=mm.STATE_ID,
                                                  RoleId = mm.MEMBER_ROLE,
                                                  roleName = rm.ROLE_NAME,
                                                  Amount = mm.BALANCE
                                              }).FirstOrDefaultAsync();
                            decimal WLClosingAmt = tbl_accountWhiteLevel.CLOSING;


                            #region Whitelable Commission
                            var WLPGSTSetVal = (from GstMem in db.TBL_MASTER_MEMBER
                                                 join StGstVal in db.TBL_STATES on GstMem.STATE_ID equals StGstVal.STATEID
                                                 where GstMem.MEM_ID == WHT_MEM_ID
                                                select new
                                                 {
                                                     StateGstVal = StGstVal.SGST,
                                                     CentralGstVal = StGstVal.CGST,
                                                     IGstVal = StGstVal.IGST
                                                 }).FirstOrDefault();
                            decimal WLPStateGstVal = 0;
                            decimal WLPCentralGstVal = 0;
                            decimal WLPIntsGstVal = 0;
                            decimal.TryParse(WLPGSTSetVal.StateGstVal.ToString(), out WLPStateGstVal);
                            decimal.TryParse(WLPGSTSetVal.CentralGstVal.ToString(), out WLPCentralGstVal);
                            decimal.TryParse(WLPGSTSetVal.IGstVal.ToString(), out WLPIntsGstVal);
                            decimal WLPCAlCStateGSt = 0;
                            decimal WLPCAlCCenterGSt = 0;
                            decimal WLPCAlCInstGSt = 0;
                            #endregion


                            // GST And TDS WhiteLevel Calculation
                            decimal WTLGapComm = decimal.Parse(WhiteComm.commPer.ToString()) - WLComm;
                            decimal WTLGapCommAmt_AMt = 0;
                            decimal WL_TDSCalculation = 0;
                            decimal WTLGapCommAmt = 0;

                            decimal WL_GST_AMT = 0;
                            decimal WL_CalculateGST_AMT = 0;

                            if (WhiteComm.commType == "FIXED")
                            {
                                if (WhiteComm.SlabTDS == "Yes")
                                {

                                    WTLGapCommAmt_AMt = WTLGapComm;
                                    if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                    {
                                        WL_TDSCalculation = decimal.Round(((WTLGapCommAmt_AMt * GST_AMount) / 100), 2);
                                        if (MemberGST != null)
                                        {
                                            WLPCAlCStateGSt = Math.Round(((WTLGapCommAmt_AMt * WLPStateGstVal) / 100), 2);
                                            WLPCAlCCenterGSt = Math.Round(((WTLGapCommAmt_AMt * WLPCentralGstVal) / 100), 2);
                                            WLPCAlCInstGSt = Math.Round(((WTLGapCommAmt_AMt * WLPIntsGstVal) / 100), 2);
                                            //WL_CalculateGST_AMT = decimal.Round(((WTLGapCommAmt_AMt * gst_VAlue) / 100), 2);                                        
                                            WTLGapCommAmt = WTLGapCommAmt_AMt - WL_TDSCalculation;
                                        }
                                        else
                                        {
                                            //WL_CalculateGST_AMT = 0;                                        
                                            WLPCAlCStateGSt = 0;
                                            WLPCAlCCenterGSt = 0;
                                            WLPCAlCInstGSt = 0;
                                            WTLGapCommAmt = WTLGapCommAmt_AMt - WL_TDSCalculation;
                                        }
                                    }
                                    else
                                    {
                                        WL_TDSCalculation = decimal.Round(((WTLGapCommAmt_AMt - GST_AMount)));
                                        if (MemberGST != null)
                                        {
                                            //WL_CalculateGST_AMT = decimal.Round(((WTLGapCommAmt_AMt - gst_VAlue)));                                        
                                            WLPCAlCStateGSt = Math.Round(((WTLGapCommAmt_AMt - WLPStateGstVal)), 2);
                                            WLPCAlCCenterGSt = Math.Round(((WTLGapCommAmt_AMt - WLPCentralGstVal)), 2);
                                            WLPCAlCInstGSt = Math.Round(((WTLGapCommAmt_AMt - WLPIntsGstVal)), 2);
                                            WTLGapCommAmt = WTLGapCommAmt_AMt - WL_TDSCalculation;
                                        }
                                        else
                                        {
                                            //WL_CalculateGST_AMT = 0;                                        
                                            WLPCAlCStateGSt = 0;
                                            WLPCAlCCenterGSt = 0;
                                            WLPCAlCInstGSt = 0;
                                            WTLGapCommAmt = WTLGapCommAmt_AMt - WL_TDSCalculation;
                                        }
                                    }
                                }
                                else
                                {
                                    //WL_TDSCalculation = decimal.Round(((WTLGapCommAmt_AMt - GST_AMount)));
                                    WL_TDSCalculation = 0;
                                    if (MemberGST != null)
                                    {
                                        //WL_CalculateGST_AMT = decimal.Round(((WTLGapCommAmt_AMt - gst_VAlue)));                                        
                                        WLPCAlCStateGSt = Math.Round(((WTLGapCommAmt_AMt - WLPStateGstVal)), 2);
                                        WLPCAlCCenterGSt = Math.Round(((WTLGapCommAmt_AMt - WLPCentralGstVal)), 2);
                                        WLPCAlCInstGSt = Math.Round(((WTLGapCommAmt_AMt - WLPIntsGstVal)), 2);
                                        WTLGapCommAmt = WTLGapCommAmt_AMt - WL_TDSCalculation;
                                    }
                                    else
                                    {
                                        //WL_CalculateGST_AMT = 0;                                        
                                        WLPCAlCStateGSt = 0;
                                        WLPCAlCCenterGSt = 0;
                                        WLPCAlCInstGSt = 0;
                                        WTLGapCommAmt = WTLGapCommAmt_AMt - WL_TDSCalculation;
                                    }
                                }

                            }
                            else
                            {
                                WTLGapCommAmt_AMt = (Trans_Amt * WTLGapComm) / 100;
                                if (WhiteComm.SlabTDS == "Yes")
                                {
                                    if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                    {
                                        WL_TDSCalculation = decimal.Round(((WTLGapCommAmt_AMt * GST_AMount) / 100), 2);
                                        if (MemberGST != null)
                                        {
                                            //WL_CalculateGST_AMT = decimal.Round(((WTLGapCommAmt_AMt * gst_VAlue) / 100), 2);                                        
                                            WLPCAlCStateGSt = Math.Round(((WTLGapCommAmt_AMt * WLPStateGstVal) / 100), 2);
                                            WLPCAlCCenterGSt = Math.Round(((WTLGapCommAmt_AMt * WLPCentralGstVal) / 100), 2);
                                            WLPCAlCInstGSt = Math.Round(((WTLGapCommAmt_AMt * WLPIntsGstVal) / 100), 2);
                                            WTLGapCommAmt = WTLGapCommAmt_AMt - WL_TDSCalculation;
                                        }
                                        else
                                        {
                                            //WL_CalculateGST_AMT = 0;                                        
                                            WLPCAlCStateGSt = 0;
                                            WLPCAlCCenterGSt = 0;
                                            WLPCAlCInstGSt = 0;
                                            WTLGapCommAmt = WTLGapCommAmt_AMt - WL_TDSCalculation;
                                        }

                                    }
                                    else
                                    {
                                        WL_TDSCalculation = decimal.Round(((WTLGapCommAmt_AMt - GST_AMount)));
                                        if (MemberGST != null)
                                        {
                                            //WL_CalculateGST_AMT = decimal.Round(((WTLGapCommAmt_AMt - gst_VAlue)));
                                            WLPCAlCStateGSt = Math.Round(((WTLGapCommAmt_AMt - WLPStateGstVal)), 2);
                                            WLPCAlCCenterGSt = Math.Round(((WTLGapCommAmt_AMt - WLPCentralGstVal)), 2);
                                            WLPCAlCInstGSt = Math.Round(((WTLGapCommAmt_AMt - WLPIntsGstVal)), 2);
                                            WTLGapCommAmt = WTLGapCommAmt_AMt - WL_TDSCalculation;
                                        }
                                        else
                                        {
                                            //WL_CalculateGST_AMT = 0;
                                            WLPCAlCStateGSt = 0;
                                            WLPCAlCCenterGSt = 0;
                                            WLPCAlCInstGSt = 0;
                                            WTLGapCommAmt = WTLGapCommAmt_AMt - WL_TDSCalculation;
                                        }
                                    }
                                }
                                else
                                {
                                    //WL_TDSCalculation = decimal.Round(((WTLGapCommAmt_AMt - GST_AMount)));
                                    WL_TDSCalculation = 0;
                                    if (MemberGST != null)
                                    {
                                        //WL_CalculateGST_AMT = decimal.Round(((WTLGapCommAmt_AMt - gst_VAlue)));
                                        WLPCAlCStateGSt = Math.Round(((WTLGapCommAmt_AMt - WLPStateGstVal)), 2);
                                        WLPCAlCCenterGSt = Math.Round(((WTLGapCommAmt_AMt - WLPCentralGstVal)), 2);
                                        WLPCAlCInstGSt = Math.Round(((WTLGapCommAmt_AMt - WLPIntsGstVal)), 2);
                                        WTLGapCommAmt = WTLGapCommAmt_AMt - WL_TDSCalculation;
                                    }
                                    else
                                    {
                                        //WL_CalculateGST_AMT = 0;
                                        WLPCAlCStateGSt = 0;
                                        WLPCAlCCenterGSt = 0;
                                        WLPCAlCInstGSt = 0;
                                        WTLGapCommAmt = WTLGapCommAmt_AMt - WL_TDSCalculation;
                                    }
                                }                               
                            }
                            WHITE_Comm_Amt = WTLGapCommAmt;
                            WHITE_TaxAMT = WL_TDSCalculation;
                            //WHILE_GST_Comm_Amt = WL_CalculateGST_AMT;
                            WHILE_GST_Comm_Amt = (WLPCAlCStateGSt+ WLPCAlCCenterGSt+ WLPCAlCInstGSt);
                            //decimal WTLGapCommAmt = (Trans_Amt * WTLGapComm) / 100;
                            decimal CommWLAddClosing = tbl_accountWhiteLevel.CLOSING + WTLGapCommAmt;
                            long WL_ID = long.Parse(WHT_MEM_ID.ToString());
                            TBL_ACCOUNTS objWLCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = WL_ID,
                                MEMBER_TYPE = WLmembtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = WTLGapCommAmt,
                                NARRATION = "Commission To WLP",
                                OPENING = WLClosingAmt,
                                CLOSING = CommWLAddClosing,
                                REC_NO = 0,
                                COMM_AMT = WTLGapCommAmt,
                                //GST = (float)WL_CalculateGST_AMT,
                                GST = (float)WHILE_GST_Comm_Amt,
                                TDS = (float)WL_TDSCalculation,
                                IPAddress = IpAddress,
                                TDS_PERCENTAGE = taxMaster.TAX_VALUE,
                                GST_PERCENTAGE = GST_Master.TAX_VALUE,
                                WHITELEVEL_ID = (long)WHT_MEM_ID,
                                //SUPER_ID = (long)SUP_MEM_ID,
                                SUPER_ID = 0,
                                DISTRIBUTOR_ID = (long)DIS_MEM_ID,
                                SERVICE_ID = Oper_Id_Val,
                                CORELATIONID = sRandomOTP,
                                REC_COMM_TYPE = WhiteComm.commType,
                                COMM_VALUE = WhiteComm.commPer,
                                NET_COMM_AMT = WHITE_Comm_Amt,
                                TDS_DR_COMM_AMT = WHITE_TaxAMT,
                                CGST_COMM_AMT_INPUT = WLPCAlCCenterGSt,
                                CGST_COMM_AMT_OUTPUT = 0,
                                SGST_COMM_AMT_INPUT = WLPCAlCStateGSt,
                                SGST_COMM_AMT_OUTPUT = 0,
                                IGST_COMM_AMT_INPUT = WLPCAlCInstGSt,
                                IGST_COMM_AMT_OUTPUT = 0,
                                TOTAL_GST_COMM_AMT_INPUT = WHILE_GST_Comm_Amt,
                                TOTAL_GST_COMM_AMT_OUTPUT = 0,
                                TDS_RATE = GST_AMount,
                                CGST_RATE = WLPCentralGstVal,
                                SGST_RATE = WLPStateGstVal,
                                IGST_RATE = WLPIntsGstVal,
                                TOTAL_GST_RATE = (WLPCentralGstVal + WLPStateGstVal + WLPIntsGstVal),
                                COMM_SLAB_ID = Oper_Id_Val,
                                STATE_ID = WLmembtype.Stateid,
                                FLAG1 = 0,
                                FLAG2 = 0,
                                FLAG3 = 0,
                                FLAG4 = 0,
                                FLAG5 = 0,
                                FLAG6 = 0,
                                FLAG7 = 0,
                                FLAG8 = 0,
                                FLAG9 = 0,
                                FLAG10 = 0,
                                VENDOR_ID = VendorInfo.ID
                            };
                            db.TBL_ACCOUNTS.Add(objWLCommPer);
                            await db.SaveChangesAsync();

                            decimal WHTLBal = 0;
                            decimal UpdatWHITEBal = 0;
                            var WHITELabelBal =await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == WL_ID);
                            if (WHITELabelBal != null)
                            {
                                decimal.TryParse(WHITELabelBal.BALANCE.ToString(), out WHTLBal);
                                UpdatWHITEBal = WHTLBal + WTLGapCommAmt;
                                WHITELabelBal.BALANCE = UpdatWHITEBal;
                                db.Entry(WHITELabelBal).State = System.Data.Entity.EntityState.Modified;
                                await db.SaveChangesAsync();
                            }
                            else
                            {
                                WHITELabelBal.BALANCE = WHTLBal;
                                db.Entry(WHITELabelBal).State = System.Data.Entity.EntityState.Modified;
                                await db.SaveChangesAsync();
                            }

                        }
                        #endregion


                        #region Update InstantpayTable Recharge And Response Table

                        var updateInstantres = await db.TBL_INSTANTPAY_RECHARGE_RESPONSE.FirstOrDefaultAsync(x => x.CORELATIONID == sRandomOTP);
                        if (updateInstantres != null)
                        {
                            updateInstantres.REC_COMM_TYPE = MerchantComm.commType;
                            updateInstantres.MER_COMM_VALUE = MerchantComm.commPer;
                            updateInstantres.MER_COMM_AMT = MER_Comm_Amt;
                            updateInstantres.MER_TDS_DR_COMM_AMT = MER_TaxAMT;
                            updateInstantres.DIST_ID = long.Parse(DIS_MEM_ID.ToString());
                            updateInstantres.DIST_COMM_VALUE = DistributorComm.commPer;
                            updateInstantres.DIST_COMM_AMT = DEST_Comm_Amt;
                            updateInstantres.DIST_TDS_DR_COMM_AMT = DEST_TaxAMT;
                            //updateInstantres.SUPER_ID = long.Parse(SUP_MEM_ID.ToString());
                            //updateInstantres.SUPER_COMM_VALUE = SuperComm.commPer;
                            //updateInstantres.SUPER_COMM_AMT = SPR_Comm_Amt;
                            //updateInstantres.SUPER_TDS_DR_COMM_AMT = SPR_TaxAMT;
                            updateInstantres.SUPER_ID = 0;
                            updateInstantres.SUPER_COMM_VALUE = 0;
                            updateInstantres.SUPER_COMM_AMT = 0;
                            updateInstantres.SUPER_TDS_DR_COMM_AMT = 0;
                            updateInstantres.WHITELABEL_ID = long.Parse(WHT_MEM_ID.ToString());
                            updateInstantres.WHITELABEL_VALUE = WhiteComm.commPer;
                            updateInstantres.WHITELABEL_COMM_AMT = WHITE_Comm_Amt;
                            updateInstantres.WHITELABEL_TDS_DR_COMM_AMT = WHITE_TaxAMT;
                            updateInstantres.TDS_RATE = taxMaster.TAX_VALUE;
                            updateInstantres.CORELATIONID = sRandomOTP;
                            updateInstantres.ERROR_TYPE = "Pending";
                            updateInstantres.ISREVERSE = "No";
                            updateInstantres.DOMAIN_NAME = IpAddress;
                            updateInstantres.ISCOMMISSIONDISBURSE = "No";
                            updateInstantres.COMMISSIONDISBURSEDATE = DateTime.Now;
                            updateInstantres.GST_RATE = GST_Master.TAX_VALUE;
                            updateInstantres.MER_COMM_GST_AMT = MER_GST_Comm_Amt;
                            updateInstantres.DIST_COMM_GST_AMT = DIST_GST_Comm_Amt;
                            updateInstantres.SUPER_COMM_GST_AMT = SUPER_GST_Comm_Amt;
                            updateInstantres.WHITELABEL_COMM_GST_AMT = WHILE_GST_Comm_Amt;
                            updateInstantres.MER_INVOICE_ID = 0;
                            updateInstantres.MER_CANCEL_INVOICE = "";
                            updateInstantres.DIST_INVOICE_ID = 0;
                            updateInstantres.DIST_CANCEL_INVOICE = "";
                            updateInstantres.SUPER_INVOICE_ID = 0;
                            updateInstantres.SUPER_CANCEL_INVOICE = "";
                            updateInstantres.WHITELABEL_INVOICE_ID = 0;
                            updateInstantres.WHITELABEL_CANCEL_INVOICE = "";
                            updateInstantres.SLAB_ID = MerchantComm.SLAB_ID;

                            db.Entry(updateInstantres).State = System.Data.Entity.EntityState.Modified;
                            await db.SaveChangesAsync();
                        }


                        #endregion

                        ContextTransaction.Commit();
                        return true;
                    }
                    else if (status == "UTILITY")
                    {
                        var UtilityId = await db.TBL_SERVICE_PROVIDERS.Where(x => x.SERVICE_KEY == serviceprovider).FirstOrDefaultAsync();
                        //var MerchantComm = await (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                        //                    join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                        //                    on detailscom.SLN equals commslabMob.SLAB_ID
                        //                    join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                        //                    where commslabMob.OPERATOR_CODE == serviceprovider && detailscom.SLAB_TYPE == 2 && mem.MEMBER_ROLE == 5 && commslabMob.MEM_ID == WHT_MEM_ID
                        //                          select new
                        //                    {
                        //                        SLN = commslabMob.SLN,
                        //                        SLABID=commslabMob.SLAB_ID,
                        //                        commPer = commslabMob.MERCHANT_COM_PER,
                        //                        commType = commslabMob.COMMISSION_TYPE
                        //                    }).FirstOrDefaultAsync();

                        var MerchantComm = await (from WLPCom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                                  join detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB on WLPCom.SLN equals detailscom.BILLPAYMENT_SLAB
                                                  join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                                  on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
                                                  join mem in db.TBL_MASTER_MEMBER on detailscom.INTRODUCER_ID equals mem.MEM_ID
                                                  where commslabMob.OPERATOR_CODE == serviceprovider && mem.MEMBER_ROLE == 5 && detailscom.INTRODUCER_ID == Mem_ID
                                                  select new
                                                  {
                                                      SLN = commslabMob.SLN,
                                                      SLAB_ID = commslabMob.SLAB_ID,
                                                      commPer = commslabMob.MERCHANT_COM_PER,
                                                      commType = commslabMob.COMMISSION_TYPE,
                                                      SlabTDS = WLPCom.SLAB_TDS
                                                  }).FirstOrDefaultAsync();




                        #region WhiteLabel Service Charge Add
                        //var WLCOMM = await (from WLCOMTag in db.TBL_COMM_SLAB_UTILITY_RECHARGE
                        //                    join WLCOMMVAL in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                    on WLCOMTag.SLAB_ID equals WLCOMMVAL.BILLPAYMENT_SLAB
                        //                    where WLCOMMVAL.WHITE_LEVEL_ID == WHT_MEM_ID && WLCOMTag.OPERATOR_CODE == serviceprovider
                        //                    select new
                        //                    {
                        //                        COMMTYPE = WLCOMTag.COMMISSION_TYPE,
                        //                        COMM_VALUE = WLCOMTag.COMM_PERCENTAGE
                        //                    }).FirstOrDefaultAsync();
                        var WLCOMM = await (from WLLMain in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                            join  WLCOMTag in db.TBL_COMM_SLAB_UTILITY_RECHARGE on WLLMain.SLN equals WLCOMTag.SLAB_ID
                                            join WLCOMMVAL in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                                            on WLCOMTag.SLAB_ID equals WLCOMMVAL.BILLPAYMENT_SLAB
                                            where WLCOMMVAL.WHITE_LEVEL_ID == WHT_MEM_ID && WLCOMTag.OPERATOR_CODE == serviceprovider
                                            select new
                                            {
                                                COMMTYPE = WLCOMTag.COMMISSION_TYPE,
                                                COMM_VALUE = WLCOMTag.COMM_PERCENTAGE,
                                                SLAB_TDS=WLLMain.SLAB_TDS
                                            }).FirstOrDefaultAsync();
                        //var WLCOMM = await (from WLLMain in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                        //                    join WLCOMTag in db.TBL_COMM_SLAB_MOBILE_RECHARGE on WLLMain.SLN equals WLCOMTag.SLAB_ID
                        //                    join WLCOMMVAL in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                    on WLCOMTag.SLAB_ID equals WLCOMMVAL.RECHARGE_SLAB
                        //                    where WLCOMMVAL.WHITE_LEVEL_ID == WHT_MEM_ID && WLCOMTag.OPERATOR_CODE == serviceprovider
                        //                    select new
                        //                    {
                        //                        COMMTYPE = WLCOMTag.COMMISSION_TYPE,
                        //                        COMM_VALUE = WLCOMTag.COMM_PERCENTAGE,
                        //                        SLAB_TDS = WLLMain.SLAB_TDS
                        //                    }).FirstOrDefaultAsync();


                        //var WLCOMM =await (from WLCOMTag in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                        //              join WLCOMMVAL in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //              on WLCOMTag.SLAB_ID equals WLCOMMVAL.BILLPAYMENT_SLAB
                        //              where WLCOMMVAL.WHITE_LEVEL_ID == WHT_MEM_ID && WLCOMTag.OPERATOR_CODE == serviceprovider
                        //              select new
                        //              {
                        //                  COMMTYPE = WLCOMTag.COMM_TYPE,
                        //                  COMM_VALUE = WLCOMTag.COMM_PERCENTAGE
                        //              }).FirstOrDefaultAsync();
                        decimal WLserviceAmtAdd = 0;
                        decimal AddWL_ServiceAmtAdd = 0;
                        decimal WLService_AMT = 0;
                        decimal ServiceAddAmt = 0;
                        //var WL_Wallet =await db.TBL_ACCOUNTS.FirstOrDefaultAsync(x => x.MEM_ID == WHT_MEM_ID);
                        var WL_Wallet = await db.TBL_ACCOUNTS.Where(x => x.MEM_ID == WHT_MEM_ID).OrderByDescending(z => z.ACC_NO).FirstOrDefaultAsync();
                        //var MER_ACCOUNT = await db.TBL_ACCOUNTS.Where(x => x.MEM_ID == transinfo.FROM_MEMBER).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefaultAsync();
                        if (WL_Wallet != null)
                        {
                            WLserviceAmtAdd = WL_Wallet.CLOSING;
                            WLService_AMT = WLCOMM.COMM_VALUE;
                            if (WLCOMM.COMMTYPE == "FIXED")
                            {
                                if (WLCOMM.SLAB_TDS == "Yes")
                                {
                                    WLLMAIn_TDSVAL = decimal.Round(((WLService_AMT * GST_AMount) / 100), 2);
                                    //ServiceAddAmt = WLService_AMT;
                                    ServiceAddAmt = WLService_AMT - WLLMAIn_TDSVAL;
                                    AddWL_ServiceAmtAdd = WLserviceAmtAdd + ServiceAddAmt - WLLMAIn_TDSVAL;
                                    //ServiceAddAmt = WLService_AMT;
                                    //AddWL_ServiceAmtAdd = WLserviceAmtAdd + ServiceAddAmt;
                                }
                                else
                                {
                                    WLLMAIn_TDSVAL = 0;
                                    ServiceAddAmt = WLService_AMT - WLLMAIn_TDSVAL;
                                    AddWL_ServiceAmtAdd = WLserviceAmtAdd + ServiceAddAmt - WLLMAIn_TDSVAL;
                                }

                            }
                            else
                            {

                                if (WLCOMM.SLAB_TDS == "Yes")
                                {
                                    CALCUWLLPCOMM = (Trans_Amt * WLService_AMT) / 100;


                                    WLLMAIn_TDSVAL = decimal.Round(((CALCUWLLPCOMM * GST_AMount) / 100), 2);
                                    ServiceAddAmt = CALCUWLLPCOMM - WLLMAIn_TDSVAL;
                                    AddWL_ServiceAmtAdd = WLserviceAmtAdd + CALCUWLLPCOMM - WLLMAIn_TDSVAL;

                                    //ServiceAddAmt = (Trans_Amt * WLService_AMT) / 100;
                                    //WLLMAIn_TDSVAL = decimal.Round(((ServiceAddAmt * GST_AMount) / 100), 2);
                                    //AddWL_ServiceAmtAdd = WLserviceAmtAdd + WLService_AMT- WLLMAIn_TDSVAL;

                                }
                                else
                                {
                                    ServiceAddAmt = (Trans_Amt * WLService_AMT) / 100;
                                    WLLMAIn_TDSVAL = 0;
                                    //AddWL_ServiceAmtAdd = WLserviceAmtAdd + WLService_AMT- WLLMAIn_TDSVAL;
                                    AddWL_ServiceAmtAdd = WLserviceAmtAdd + ServiceAddAmt - WLLMAIn_TDSVAL;
                                }

                                //ServiceAddAmt = (Trans_Amt * WLService_AMT) / 100;
                                //AddWL_ServiceAmtAdd = WLserviceAmtAdd + WLService_AMT;
                            }
                            TBL_ACCOUNTS WLobjval = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = (long)WHT_MEM_ID,
                                MEMBER_TYPE = "WHITE LEVEL",
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = ServiceAddAmt,
                                NARRATION = "Cash Back TO WLP",
                                OPENING = WL_Wallet.CLOSING,
                                CLOSING = AddWL_ServiceAmtAdd,
                                REC_NO = 0,
                                COMM_AMT = 0,
                                GST = 0,
                                TDS = (double)WLLMAIn_TDSVAL,
                                IPAddress = IpAddress,
                                TDS_PERCENTAGE = 0,
                                GST_PERCENTAGE = 0,
                                WHITELEVEL_ID = (long)WHT_MEM_ID,
                                //SUPER_ID = (long)SUP_MEM_ID,
                                SUPER_ID = 0,
                                DISTRIBUTOR_ID = (long)DIS_MEM_ID,
                                SERVICE_ID = UtilityId.ID,
                                CORELATIONID = sRandomOTP
                            };
                            db.TBL_ACCOUNTS.Add(WLobjval);
                            await db.SaveChangesAsync();
                            decimal Update_WhiteLBLBal = 0;
                            decimal WLAvaiBal = 0;
                            var UpdateWLBalance = await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == WHT_MEM_ID);
                            if (UpdateWLBalance != null)
                            {
                                decimal.TryParse(UpdateWLBalance.BALANCE.ToString(), out WLAvaiBal);
                                Update_WhiteLBLBal = WLAvaiBal + AddWL_ServiceAmtAdd;
                                UpdateWLBalance.BALANCE = Update_WhiteLBLBal;
                                db.Entry(UpdateWLBalance).State = System.Data.Entity.EntityState.Modified;
                                await db.SaveChangesAsync();
                            }
                            else
                            {
                                UpdateWLBalance.BALANCE = AddWL_ServiceAmtAdd;
                                db.Entry(UpdateWLBalance).State = System.Data.Entity.EntityState.Modified;
                                await db.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            WLserviceAmtAdd = 0;
                            WLService_AMT = WLCOMM.COMM_VALUE;
                            if (WLCOMM.COMMTYPE == "FIXED")
                            {
                                if (WLCOMM.SLAB_TDS == "Yes")
                                {
                                    WLLMAIn_TDSVAL = decimal.Round(((WLService_AMT * GST_AMount) / 100), 2);
                                    //ServiceAddAmt = WLService_AMT;
                                    ServiceAddAmt = WLService_AMT - WLLMAIn_TDSVAL;
                                    AddWL_ServiceAmtAdd = WLserviceAmtAdd + ServiceAddAmt - WLLMAIn_TDSVAL;
                                    //ServiceAddAmt = WLService_AMT;
                                    //AddWL_ServiceAmtAdd = WLserviceAmtAdd + ServiceAddAmt;
                                }
                                else
                                {
                                    WLLMAIn_TDSVAL = 0;
                                    ServiceAddAmt = WLService_AMT - WLLMAIn_TDSVAL;
                                    AddWL_ServiceAmtAdd = WLserviceAmtAdd + ServiceAddAmt - WLLMAIn_TDSVAL;
                                }

                            }
                            else
                            {

                                if (WLCOMM.SLAB_TDS == "Yes")
                                {
                                    CALCUWLLPCOMM = (Trans_Amt * WLService_AMT) / 100;


                                    WLLMAIn_TDSVAL = decimal.Round(((CALCUWLLPCOMM * GST_AMount) / 100), 2);
                                    ServiceAddAmt = CALCUWLLPCOMM - WLLMAIn_TDSVAL;
                                    AddWL_ServiceAmtAdd = WLserviceAmtAdd + CALCUWLLPCOMM - WLLMAIn_TDSVAL;

                                    //ServiceAddAmt = (Trans_Amt * WLService_AMT) / 100;
                                    //WLLMAIn_TDSVAL = decimal.Round(((ServiceAddAmt * GST_AMount) / 100), 2);
                                    //AddWL_ServiceAmtAdd = WLserviceAmtAdd + WLService_AMT- WLLMAIn_TDSVAL;

                                }
                                else
                                {
                                    ServiceAddAmt = (Trans_Amt * WLService_AMT) / 100;
                                    WLLMAIn_TDSVAL = 0;
                                    //AddWL_ServiceAmtAdd = WLserviceAmtAdd + WLService_AMT- WLLMAIn_TDSVAL;
                                    AddWL_ServiceAmtAdd = WLserviceAmtAdd + ServiceAddAmt - WLLMAIn_TDSVAL;
                                }

                                //ServiceAddAmt = (Trans_Amt * WLService_AMT) / 100;
                                //AddWL_ServiceAmtAdd = WLserviceAmtAdd + WLService_AMT;
                            }
                            TBL_ACCOUNTS WLobjval = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = (long)WHT_MEM_ID,
                                MEMBER_TYPE = "WHITE LEVEL",
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = ServiceAddAmt,
                                NARRATION = "Cash Back TO WLP",
                                OPENING = 0,
                                CLOSING = AddWL_ServiceAmtAdd,
                                REC_NO = 0,
                                COMM_AMT = 0,
                                GST = 0,
                                TDS = (double)WLLMAIn_TDSVAL,
                                IPAddress = IpAddress,
                                TDS_PERCENTAGE = 0,
                                GST_PERCENTAGE = 0,
                                WHITELEVEL_ID = (long)WHT_MEM_ID,
                                //SUPER_ID = (long)SUP_MEM_ID,
                                SUPER_ID = 0,
                                DISTRIBUTOR_ID = (long)DIS_MEM_ID,
                                SERVICE_ID = UtilityId.ID,
                                CORELATIONID = sRandomOTP
                            };
                            db.TBL_ACCOUNTS.Add(WLobjval);
                            await db.SaveChangesAsync();
                            decimal Update_WhiteLBLBal = 0;
                            decimal WLAvaiBal = 0;
                            var UpdateWLBalance = await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == WHT_MEM_ID);
                            if (UpdateWLBalance != null)
                            {
                                decimal.TryParse(UpdateWLBalance.BALANCE.ToString(), out WLAvaiBal);
                                Update_WhiteLBLBal = WLAvaiBal + AddWL_ServiceAmtAdd;
                                UpdateWLBalance.BALANCE = Update_WhiteLBLBal;
                                db.Entry(UpdateWLBalance).State = System.Data.Entity.EntityState.Modified;
                                await db.SaveChangesAsync();
                            }
                            else
                            {
                                UpdateWLBalance.BALANCE = AddWL_ServiceAmtAdd;
                                db.Entry(UpdateWLBalance).State = System.Data.Entity.EntityState.Modified;
                                await db.SaveChangesAsync();
                            }
                        }
                        #endregion

                        //var MerchantComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                    join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
                        //                    on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
                        //                    where detailscom.WHITE_LEVEL_ID == DIS_MEM_ID && detailscom.INTRODUCER_ID == Mem_ID && commslabMob.OPERATOR_CODE == serviceprovider
                        //                    select new
                        //                    {
                        //                        SLN = commslabMob.SLN,
                        //                        commPer = commslabMob.COMM_PERCENTAGE
                        //                    }).FirstOrDefault();
                        #region Retailer Commission                        
                        var membtype =await (from mm in db.TBL_MASTER_MEMBER
                                        join
                                            rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                        where mm.MEM_ID == Mem_ID
                                        select new
                                        {
                                            StateId=mm.STATE_ID,
                                            RoleId = mm.MEMBER_ROLE,
                                            roleName = rm.ROLE_NAME,
                                            Amount = mm.BALANCE
                                        }).FirstOrDefaultAsync();
                        //var tbl_account =await db.TBL_ACCOUNTS.Where(z => z.MEM_ID == Mem_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefaultAsync();
                        var tbl_account = await db.TBL_ACCOUNTS.Where(z => z.MEM_ID == Mem_ID).OrderByDescending(z => z.ACC_NO).FirstOrDefaultAsync();
                        if (tbl_account != null)
                        {
                            decimal ClosingAmt = tbl_account.CLOSING;
                            decimal SubAmt = ClosingAmt - Trans_Amt;
                            //GST And TDS Calculation Section for Retailer
                            //decimal getPer = (Trans_Amt * MerchantComm.commPer) / 100;

                            #region MyRegion                            
                            var UTLMerGSTSetVal = (from GstMem in db.TBL_MASTER_MEMBER
                                                join StGstVal in db.TBL_STATES on GstMem.STATE_ID equals StGstVal.STATEID
                                                where GstMem.MEM_ID == Mem_ID
                                                select new
                                                {
                                                    StateGstVal = StGstVal.SGST,
                                                    CentralGstVal = StGstVal.CGST,
                                                    IGstVal = StGstVal.IGST
                                                }).FirstOrDefault();
                            decimal UTLMembStateGstVal = 0;
                            decimal UTLMembCentralGstVal = 0;
                            decimal UTLMembIntsGstVal = 0;
                            decimal.TryParse(UTLMerGSTSetVal.StateGstVal.ToString(), out UTLMembStateGstVal);
                            decimal.TryParse(UTLMerGSTSetVal.CentralGstVal.ToString(), out UTLMembCentralGstVal);
                            decimal.TryParse(UTLMerGSTSetVal.IGstVal.ToString(), out UTLMembIntsGstVal);
                            decimal UTLMembCAlCStateGSt = 0;
                            decimal UTLMembCAlCCenterGSt = 0;
                            decimal UTLMembCAlCInstGSt = 0;
                            #endregion



                            decimal getPer_Value = 0;
                            decimal MerchantTDSValue = 0;
                            decimal getPer = 0;
                            decimal mer_GST_AMT = 0;
                            decimal MerCalculate_GST_AMT = 0;

                            decimal.TryParse(MerchantComm.commPer.ToString(), out MerComm_Slab_Amt);

                            if (MerchantComm.commType == "FIXED")
                            {
                                getPer_Value = MerchantComm.commPer;
                                if (MerchantComm.SlabTDS == "Yes")
                                {
                                    if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                    {

                                        MerchantTDSValue = decimal.Round(((getPer_Value * GST_AMount) / 100), 2);
                                        if (MemberGST != null)
                                        {
                                            UTLMembCAlCStateGSt = Math.Round(((getPer_Value * UTLMembStateGstVal) / 100), 2);
                                            UTLMembCAlCCenterGSt = Math.Round(((getPer_Value * UTLMembCentralGstVal) / 100), 2);
                                            UTLMembCAlCInstGSt = Math.Round(((getPer_Value * UTLMembIntsGstVal) / 100), 2);
                                            //MerCalculate_GST_AMT = decimal.Round(((getPer_Value * gst_VAlue) / 100), 2);                                        
                                            getPer = getPer_Value - MerchantTDSValue;
                                        }
                                        else
                                        {
                                            //MerCalculate_GST_AMT = 0;                                        
                                            UTLMembCAlCStateGSt = 0;
                                            UTLMembCAlCCenterGSt = 0;
                                            UTLMembCAlCInstGSt = 0;
                                            getPer = getPer_Value - MerchantTDSValue;
                                        }
                                    }
                                    else
                                    {
                                        MerchantTDSValue = decimal.Round(((getPer_Value - GST_AMount)));
                                        if (MemberGST != null)
                                        {
                                            UTLMembCAlCStateGSt = Math.Round(((getPer_Value - UTLMembStateGstVal)), 2);
                                            UTLMembCAlCCenterGSt = Math.Round(((getPer_Value - UTLMembCentralGstVal)), 2);
                                            UTLMembCAlCInstGSt = Math.Round(((getPer_Value - UTLMembIntsGstVal)), 2);
                                            //MerCalculate_GST_AMT = decimal.Round(((getPer_Value - gst_VAlue)));                                        
                                            getPer = getPer_Value - MerchantTDSValue;
                                        }
                                        else
                                        {
                                            //MerCalculate_GST_AMT = 0;                                        
                                            UTLMembCAlCStateGSt = 0;
                                            UTLMembCAlCCenterGSt = 0;
                                            UTLMembCAlCInstGSt = 0;
                                            getPer = getPer_Value - MerchantTDSValue;
                                        }
                                    }
                                }
                                else
                                {
                                    //MerchantTDSValue = decimal.Round(((getPer_Value - GST_AMount)));
                                    MerchantTDSValue = 0;
                                    if (MemberGST != null)
                                    {
                                        UTLMembCAlCStateGSt = Math.Round(((getPer_Value - UTLMembStateGstVal)), 2);
                                        UTLMembCAlCCenterGSt = Math.Round(((getPer_Value - UTLMembCentralGstVal)), 2);
                                        UTLMembCAlCInstGSt = Math.Round(((getPer_Value - UTLMembIntsGstVal)), 2);
                                        //MerCalculate_GST_AMT = decimal.Round(((getPer_Value - gst_VAlue)));                                        
                                        getPer = getPer_Value - MerchantTDSValue;
                                    }
                                    else
                                    {
                                        //MerCalculate_GST_AMT = 0;                                        
                                        UTLMembCAlCStateGSt = 0;
                                        UTLMembCAlCCenterGSt = 0;
                                        UTLMembCAlCInstGSt = 0;
                                        getPer = getPer_Value - MerchantTDSValue;
                                    }
                                }
                            }
                            else
                            {
                                getPer_Value = (Trans_Amt * MerchantComm.commPer) / 100;
                                if (MerchantComm.SlabTDS == "Yes")
                                {
                                    if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                    {
                                        MerchantTDSValue = decimal.Round(((getPer_Value * GST_AMount) / 100), 2);
                                        if (MemberGST != null)
                                        {
                                            UTLMembCAlCStateGSt = Math.Round(((getPer_Value * UTLMembStateGstVal) / 100), 2);
                                            UTLMembCAlCCenterGSt = Math.Round(((getPer_Value * UTLMembCentralGstVal) / 100), 2);
                                            UTLMembCAlCInstGSt = Math.Round(((getPer_Value * UTLMembIntsGstVal) / 100), 2);
                                            //MerCalculate_GST_AMT = decimal.Round(((getPer_Value * gst_VAlue) / 100), 2);
                                            getPer = getPer_Value - MerchantTDSValue;
                                        }
                                        else
                                        {
                                            //MerCalculate_GST_AMT = 0;
                                            UTLMembCAlCStateGSt = 0;
                                            UTLMembCAlCCenterGSt = 0;
                                            UTLMembCAlCInstGSt = 0;
                                            getPer = getPer_Value - MerchantTDSValue;
                                        }
                                    }
                                    else
                                    {
                                        MerchantTDSValue = decimal.Round(((getPer_Value - GST_AMount)));
                                        if (MemberGST != null)
                                        {
                                            //MerCalculate_GST_AMT = decimal.Round(((getPer_Value - gst_VAlue)));
                                            UTLMembCAlCStateGSt = Math.Round(((getPer_Value - UTLMembStateGstVal)), 2);
                                            UTLMembCAlCCenterGSt = Math.Round(((getPer_Value - UTLMembCentralGstVal)), 2);
                                            UTLMembCAlCInstGSt = Math.Round(((getPer_Value - UTLMembIntsGstVal)), 2);
                                            getPer = getPer_Value - MerchantTDSValue;
                                        }
                                        else
                                        {
                                            //MerCalculate_GST_AMT = 0;
                                            UTLMembCAlCStateGSt = 0;
                                            UTLMembCAlCCenterGSt = 0;
                                            UTLMembCAlCInstGSt = 0;
                                            getPer = getPer_Value - MerchantTDSValue;
                                        }
                                    }
                                }
                                else
                                {
                                    // MerchantTDSValue = decimal.Round(((getPer_Value - GST_AMount)));
                                    MerchantTDSValue = 0;
                                    if (MemberGST != null)
                                    {
                                        //MerCalculate_GST_AMT = decimal.Round(((getPer_Value - gst_VAlue)));
                                        UTLMembCAlCStateGSt = Math.Round(((getPer_Value - UTLMembStateGstVal)), 2);
                                        UTLMembCAlCCenterGSt = Math.Round(((getPer_Value - UTLMembCentralGstVal)), 2);
                                        UTLMembCAlCInstGSt = Math.Round(((getPer_Value - UTLMembIntsGstVal)), 2);
                                        getPer = getPer_Value - MerchantTDSValue;
                                    }
                                    else
                                    {
                                        //MerCalculate_GST_AMT = 0;
                                        UTLMembCAlCStateGSt = 0;
                                        UTLMembCAlCCenterGSt = 0;
                                        UTLMembCAlCInstGSt = 0;
                                        getPer = getPer_Value - MerchantTDSValue;
                                    }
                                }
                              
                            }
                            MER_Comm_Amt = getPer;
                            MER_TaxAMT = MerchantTDSValue;
                            //MER_GST_Comm_Amt = MerCalculate_GST_AMT;
                            MER_GST_Comm_Amt = (UTLMembCAlCStateGSt+ UTLMembCAlCCenterGSt+ UTLMembCAlCInstGSt);
                            decimal CommAddClosing = SubAmt + getPer;
                            TBL_ACCOUNTS objCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Mem_ID,
                                MEMBER_TYPE = membtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = SubAmt,
                                CLOSING = CommAddClosing,
                                REC_NO = 0,
                                COMM_AMT = getPer,
                                GST = (float)MerCalculate_GST_AMT,
                                TDS = (float)MerchantTDSValue,
                                IPAddress = IpAddress,
                                TDS_PERCENTAGE = taxMaster.TAX_VALUE,
                                GST_PERCENTAGE = GST_Master.TAX_VALUE,
                                WHITELEVEL_ID = (long)WHT_MEM_ID,
                                //SUPER_ID = (long)SUP_MEM_ID,
                                SUPER_ID = 0,
                                DISTRIBUTOR_ID = (long)DIS_MEM_ID,
                                SERVICE_ID = UtilityId.ID,
                                CORELATIONID=sRandomOTP,
                                REC_COMM_TYPE = MerchantComm.commType,
                                COMM_VALUE = MerchantComm.commPer,
                                NET_COMM_AMT = MER_Comm_Amt,
                                TDS_DR_COMM_AMT = MER_TaxAMT,
                                CGST_COMM_AMT_INPUT = UTLMembCAlCCenterGSt,
                                CGST_COMM_AMT_OUTPUT = 0,
                                SGST_COMM_AMT_INPUT = UTLMembCAlCStateGSt,
                                SGST_COMM_AMT_OUTPUT = 0,
                                IGST_COMM_AMT_INPUT = UTLMembCAlCInstGSt,
                                IGST_COMM_AMT_OUTPUT = 0,
                                TOTAL_GST_COMM_AMT_INPUT = MER_GST_Comm_Amt,
                                TOTAL_GST_COMM_AMT_OUTPUT = 0,
                                TDS_RATE = GST_AMount,
                                CGST_RATE = UTLMembCentralGstVal,
                                SGST_RATE = UTLMembStateGstVal,
                                IGST_RATE = UTLMembIntsGstVal,
                                TOTAL_GST_RATE = (UTLMembCentralGstVal + UTLMembStateGstVal + UTLMembIntsGstVal),
                                COMM_SLAB_ID = Oper_Id_Val,
                                STATE_ID = membtype.StateId,
                                FLAG1 = 0,
                                FLAG2 = 0,
                                FLAG3 = 0,
                                FLAG4 = 0,
                                FLAG5 = 0,
                                FLAG6 = 0,
                                FLAG7 = 0,
                                FLAG8 = 0,
                                FLAG9 = 0,
                                FLAG10 = 0,
                                VENDOR_ID = VendorInfo.ID
                            };
                            db.TBL_ACCOUNTS.Add(objCommPer);
                            await db.SaveChangesAsync();
                            decimal MerBal = 0;
                            decimal UpdateMerBal = 0;
                            var MechntBal =await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == Mem_ID);
                            if (MechntBal != null)
                            {
                                decimal.TryParse(MechntBal.BALANCE.ToString(), out MerBal);
                                UpdateMerBal = MerBal + getPer;
                                MechntBal.BALANCE = UpdateMerBal;
                                db.Entry(MechntBal).State = System.Data.Entity.EntityState.Modified;
                                await db.SaveChangesAsync();
                            }
                            else
                            {
                                MechntBal.BALANCE = MerBal;
                                db.Entry(MechntBal).State = System.Data.Entity.EntityState.Modified;
                                await db.SaveChangesAsync();
                            }
                        }
                        #endregion

                        //var DistributorComm =await (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                        //                       join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                        //                       on detailscom.SLN equals commslabMob.SLAB_ID
                        //                       join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                        //                       where commslabMob.OPERATOR_CODE == serviceprovider && detailscom.SLAB_TYPE == 2 && mem.MEMBER_ROLE == 4 && commslabMob.MEM_ID == WHT_MEM_ID
                        //                            select new
                        //                       {
                        //                           SLN = commslabMob.SLN,
                        //                           commPer = commslabMob.DISTRIBUTOR_COM_PER,
                        //                           commType = commslabMob.COMMISSION_TYPE
                        //                       }).FirstOrDefaultAsync();

                        var DistributorComm = await (from WLPCom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                                     join detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB on WLPCom.SLN equals detailscom.BILLPAYMENT_SLAB
                                                     join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                                     on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
                                                     join mem in db.TBL_MASTER_MEMBER on detailscom.INTRODUCER_ID equals mem.MEM_ID
                                                     where commslabMob.OPERATOR_CODE == serviceprovider && mem.MEMBER_ROLE == 4 && detailscom.INTRODUCER_ID == DIS_MEM_ID
                                                     select new
                                                     {
                                                         SLN = commslabMob.SLN,
                                                         SLAB_ID = commslabMob.SLAB_ID,
                                                         commPer = commslabMob.DISTRIBUTOR_COM_PER,
                                                         commType = commslabMob.COMMISSION_TYPE,
                                                         SlabTDS = WLPCom.SLAB_TDS
                                                     }).FirstOrDefaultAsync();

                        //var DistributorComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                       join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
                        //                       on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
                        //                       where detailscom.WHITE_LEVEL_ID == SUP_MEM_ID && detailscom.INTRODUCER_ID == DIS_MEM_ID && commslabMob.OPERATOR_CODE == serviceprovider
                        //                       select new
                        //                       {
                        //                           SLN = commslabMob.SLN,
                        //                           commPer = commslabMob.COMM_PERCENTAGE
                        //                       }).FirstOrDefault();
                        #region Distributor Commission
                        //decimal DisGapComm = decimal.Parse(DistributorComm.commPer.ToString()) - decimal.Parse(MerchantComm.commPer.ToString());

                        //var tbl_accountDistributor =await db.TBL_ACCOUNTS.Where(z => z.MEM_ID == DIS_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefaultAsync();
                        var tbl_accountDistributor = await db.TBL_ACCOUNTS.Where(z => z.MEM_ID == DIS_MEM_ID).OrderByDescending(z => z.ACC_NO).FirstOrDefaultAsync();
                        if (tbl_accountDistributor != null)
                        {
                            var CheckDisCommission =await db.TBL_DETAILS_MEMBER_COMMISSION_SLAB.Where(x => x.INTRODUCER_ID == DIS_MEM_ID).FirstOrDefaultAsync();
                            var Dismembtype =await (from mm in db.TBL_MASTER_MEMBER
                                               join
                                                   rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                               where mm.MEM_ID == DIS_MEM_ID
                                               select new
                                               {
                                                   StateId=mm.STATE_ID,
                                                   RoleId = mm.MEMBER_ROLE,
                                                   roleName = rm.ROLE_NAME,
                                                   Amount = mm.BALANCE
                                               }).FirstOrDefaultAsync();
                            decimal DisClosingAmt = tbl_accountDistributor.CLOSING;

                            #region Distributor Commission
                            var UTLDistGSTSetVal = (from GstMem in db.TBL_MASTER_MEMBER
                                                 join StGstVal in db.TBL_STATES on GstMem.STATE_ID equals StGstVal.STATEID
                                                 where GstMem.MEM_ID == DIS_MEM_ID
                                                 select new
                                                 {
                                                     StateGstVal = StGstVal.SGST,
                                                     CentralGstVal = StGstVal.CGST,
                                                     IGstVal = StGstVal.IGST
                                                 }).FirstOrDefault();
                            decimal UTLDistStateGstVal = 0;
                            decimal UTLDistCentralGstVal = 0;
                            decimal UTLDistIntsGstVal = 0;
                            decimal.TryParse(UTLDistGSTSetVal.StateGstVal.ToString(), out UTLDistStateGstVal);
                            decimal.TryParse(UTLDistGSTSetVal.CentralGstVal.ToString(), out UTLDistCentralGstVal);
                            decimal.TryParse(UTLDistGSTSetVal.IGstVal.ToString(), out UTLDistIntsGstVal);
                            decimal UTLDistCAlCStateGSt = 0;
                            decimal UTLDistCAlCCenterGSt = 0;
                            decimal UTLDistCAlCInstGSt = 0;
                            #endregion

                            // GST and TDS Calculation for Distributor

                            decimal DisGapComm = 0;
                            //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;
                            decimal UDisGapCommAmt_Value = 0;
                            decimal UDis_TDSCal = 0;
                            decimal DisGapCommAmt = 0;
                            decimal Dist_GST_AMt = 0;
                            decimal Dist_CalculateGST_AMt = 0;

                            decimal.TryParse(DistributorComm.commPer.ToString(), out DISTRIBUTOR_GAPP_CoMMISSIOM);
                            DisGapComm = DISTRIBUTOR_GAPP_CoMMISSIOM - MerComm_Slab_Amt;

                            if (DistributorComm.commType == "FIXED")
                            {
                                //DisGapComm = decimal.Parse(DistributorComm.commPer.ToString());
                                //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;
                                if (DistributorComm.SlabTDS == "Yes")
                                {
                                    UDisGapCommAmt_Value = DisGapComm;
                                    if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                    {
                                        UDis_TDSCal = decimal.Round(((UDisGapCommAmt_Value * GST_AMount) / 100), 2);
                                        if (MemberGST != null)
                                        {
                                            //Dist_CalculateGST_AMt = decimal.Round(((UDisGapCommAmt_Value * gst_VAlue) / 100), 2);                                        
                                            UTLDistCAlCStateGSt = Math.Round(((UDisGapCommAmt_Value * UTLDistStateGstVal) / 100), 2);
                                            UTLDistCAlCCenterGSt = Math.Round(((UDisGapCommAmt_Value * UTLDistCentralGstVal) / 100), 2);
                                            UTLDistCAlCInstGSt = Math.Round(((UDisGapCommAmt_Value * UTLDistIntsGstVal) / 100), 2);
                                            DisGapCommAmt = UDisGapCommAmt_Value - UDis_TDSCal;
                                        }
                                        else
                                        {
                                            //Dist_CalculateGST_AMt = 0;                                        
                                            UTLDistCAlCStateGSt = 0;
                                            UTLDistCAlCCenterGSt = 0;
                                            UTLDistCAlCInstGSt = 0;
                                            DisGapCommAmt = UDisGapCommAmt_Value - UDis_TDSCal;
                                        }
                                    }
                                    else
                                    {
                                        UDis_TDSCal = decimal.Round(((UDisGapCommAmt_Value - GST_AMount)));
                                        if (MemberGST != null)
                                        {
                                            //Dist_CalculateGST_AMt = decimal.Round(((UDisGapCommAmt_Value - gst_VAlue)));                                        
                                            UTLDistCAlCStateGSt = Math.Round(((UDisGapCommAmt_Value - UTLDistStateGstVal)), 2);
                                            UTLDistCAlCCenterGSt = Math.Round(((UDisGapCommAmt_Value - UTLDistCentralGstVal)), 2);
                                            UTLDistCAlCInstGSt = Math.Round(((UDisGapCommAmt_Value - UTLDistIntsGstVal)), 2);
                                            DisGapCommAmt = UDisGapCommAmt_Value - UDis_TDSCal;
                                        }
                                        else
                                        {
                                            //Dist_CalculateGST_AMt = 0;
                                            UTLDistCAlCStateGSt = 0;
                                            UTLDistCAlCCenterGSt = 0;
                                            UTLDistCAlCInstGSt = 0;
                                            DisGapCommAmt = UDisGapCommAmt_Value - UDis_TDSCal;
                                        }
                                    }
                                }
                                else
                                {
                                    //UDis_TDSCal = decimal.Round(((UDisGapCommAmt_Value - GST_AMount)));
                                    UDis_TDSCal = 0;
                                    if (MemberGST != null)
                                    {
                                        //Dist_CalculateGST_AMt = decimal.Round(((UDisGapCommAmt_Value - gst_VAlue)));                                        
                                        UTLDistCAlCStateGSt = Math.Round(((UDisGapCommAmt_Value - UTLDistStateGstVal)), 2);
                                        UTLDistCAlCCenterGSt = Math.Round(((UDisGapCommAmt_Value - UTLDistCentralGstVal)), 2);
                                        UTLDistCAlCInstGSt = Math.Round(((UDisGapCommAmt_Value - UTLDistIntsGstVal)), 2);
                                        DisGapCommAmt = UDisGapCommAmt_Value - UDis_TDSCal;
                                    }
                                    else
                                    {
                                        //Dist_CalculateGST_AMt = 0;
                                        UTLDistCAlCStateGSt = 0;
                                        UTLDistCAlCCenterGSt = 0;
                                        UTLDistCAlCInstGSt = 0;
                                        DisGapCommAmt = UDisGapCommAmt_Value - UDis_TDSCal;
                                    }
                                }
                                
                            }
                            else
                            {
                                //DisGapComm = decimal.Parse(DistributorComm.commPer.ToString());
                                //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;
                                if (DistributorComm.SlabTDS == "Yes")
                                {
                                    UDisGapCommAmt_Value = (Trans_Amt * DisGapComm) / 100;
                                    if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                    {
                                        UDis_TDSCal = decimal.Round(((UDisGapCommAmt_Value * GST_AMount) / 100), 2);
                                        if (MemberGST != null)
                                        {
                                            //Dist_CalculateGST_AMt = decimal.Round(((UDisGapCommAmt_Value * gst_VAlue) / 100), 2);
                                            UTLDistCAlCStateGSt = Math.Round(((UDisGapCommAmt_Value * UTLDistStateGstVal) / 100), 2);
                                            UTLDistCAlCCenterGSt = Math.Round(((UDisGapCommAmt_Value * UTLDistCentralGstVal) / 100), 2);
                                            UTLDistCAlCInstGSt = Math.Round(((UDisGapCommAmt_Value * UTLDistIntsGstVal) / 100), 2);
                                            DisGapCommAmt = UDisGapCommAmt_Value - UDis_TDSCal;
                                        }
                                        else
                                        {
                                            //Dist_CalculateGST_AMt = 0;
                                            UTLDistCAlCStateGSt = 0;
                                            UTLDistCAlCCenterGSt = 0;
                                            UTLDistCAlCInstGSt = 0;
                                            DisGapCommAmt = UDisGapCommAmt_Value - UDis_TDSCal;
                                        }

                                    }
                                    else
                                    {
                                        UDis_TDSCal = decimal.Round(((UDisGapCommAmt_Value - GST_AMount)));
                                        if (MemberGST != null)
                                        {
                                            //Dist_CalculateGST_AMt = decimal.Round(((UDisGapCommAmt_Value - gst_VAlue)));
                                            UTLDistCAlCStateGSt = Math.Round(((UDisGapCommAmt_Value - UTLDistStateGstVal)), 2);
                                            UTLDistCAlCCenterGSt = Math.Round(((UDisGapCommAmt_Value - UTLDistCentralGstVal)), 2);
                                            UTLDistCAlCInstGSt = Math.Round(((UDisGapCommAmt_Value - UTLDistIntsGstVal)), 2);
                                            DisGapCommAmt = UDisGapCommAmt_Value - UDis_TDSCal;
                                        }
                                        else
                                        {
                                            //Dist_CalculateGST_AMt = 0;
                                            UTLDistCAlCStateGSt = 0;
                                            UTLDistCAlCCenterGSt = 0;
                                            UTLDistCAlCInstGSt = 0;
                                            DisGapCommAmt = UDisGapCommAmt_Value - UDis_TDSCal;
                                        }
                                    }
                                }
                                else
                                {
                                    //UDis_TDSCal = decimal.Round(((UDisGapCommAmt_Value - GST_AMount)));
                                    UDis_TDSCal = 0;
                                    if (MemberGST != null)
                                    {
                                        //Dist_CalculateGST_AMt = decimal.Round(((UDisGapCommAmt_Value - gst_VAlue)));
                                        UTLDistCAlCStateGSt = Math.Round(((UDisGapCommAmt_Value - UTLDistStateGstVal)), 2);
                                        UTLDistCAlCCenterGSt = Math.Round(((UDisGapCommAmt_Value - UTLDistCentralGstVal)), 2);
                                        UTLDistCAlCInstGSt = Math.Round(((UDisGapCommAmt_Value - UTLDistIntsGstVal)), 2);
                                        DisGapCommAmt = UDisGapCommAmt_Value - UDis_TDSCal;
                                    }
                                    else
                                    {
                                        //Dist_CalculateGST_AMt = 0;
                                        UTLDistCAlCStateGSt = 0;
                                        UTLDistCAlCCenterGSt = 0;
                                        UTLDistCAlCInstGSt = 0;
                                        DisGapCommAmt = UDisGapCommAmt_Value - UDis_TDSCal;
                                    }
                                }

                               
                            }
                            DEST_Comm_Amt = DisGapCommAmt;
                            DEST_TaxAMT = UDis_TDSCal;
                            //DIST_GST_Comm_Amt = Dist_CalculateGST_AMt;
                            DIST_GST_Comm_Amt = (UTLDistCAlCStateGSt+ UTLDistCAlCCenterGSt + UTLDistCAlCInstGSt);
                            decimal CommDisAddClosing = tbl_accountDistributor.CLOSING + DisGapCommAmt;
                            long Dis_ID = long.Parse(DIS_MEM_ID.ToString());
                            TBL_ACCOUNTS objDisCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Dis_ID,
                                MEMBER_TYPE = Dismembtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = DisClosingAmt,
                                CLOSING = CommDisAddClosing,
                                REC_NO = 0,
                                COMM_AMT = DisGapCommAmt,
                                //GST = (float)Dist_CalculateGST_AMt,
                                GST = (float)DIST_GST_Comm_Amt,
                                TDS = (float)UDis_TDSCal,
                                IPAddress = IpAddress,
                                TDS_PERCENTAGE = taxMaster.TAX_VALUE,
                                GST_PERCENTAGE = GST_Master.TAX_VALUE,
                                WHITELEVEL_ID = (long)WHT_MEM_ID,
                                //SUPER_ID = (long)SUP_MEM_ID,
                                SUPER_ID = 0,
                                DISTRIBUTOR_ID = (long)DIS_MEM_ID,
                                SERVICE_ID = UtilityId.ID,
                                CORELATIONID=sRandomOTP,
                                REC_COMM_TYPE = DistributorComm.commType,
                                COMM_VALUE = DistributorComm.commPer,
                                NET_COMM_AMT = DEST_Comm_Amt,
                                TDS_DR_COMM_AMT = DEST_TaxAMT,
                                CGST_COMM_AMT_INPUT = UTLDistCAlCCenterGSt,
                                CGST_COMM_AMT_OUTPUT = 0,
                                SGST_COMM_AMT_INPUT = UTLDistCAlCStateGSt,
                                SGST_COMM_AMT_OUTPUT = 0,
                                IGST_COMM_AMT_INPUT = UTLDistCAlCInstGSt,
                                IGST_COMM_AMT_OUTPUT = 0,
                                TOTAL_GST_COMM_AMT_INPUT = DIST_GST_Comm_Amt,
                                TOTAL_GST_COMM_AMT_OUTPUT = 0,
                                TDS_RATE = GST_AMount,
                                CGST_RATE = UTLDistCentralGstVal,
                                SGST_RATE = UTLDistStateGstVal,
                                IGST_RATE = UTLDistIntsGstVal,
                                TOTAL_GST_RATE = (UTLDistCentralGstVal + UTLDistStateGstVal + UTLDistIntsGstVal),
                                COMM_SLAB_ID = Oper_Id_Val,
                                STATE_ID = Dismembtype.StateId,
                                FLAG1 = 0,
                                FLAG2 = 0,
                                FLAG3 = 0,
                                FLAG4 = 0,
                                FLAG5 = 0,
                                FLAG6 = 0,
                                FLAG7 = 0,
                                FLAG8 = 0,
                                FLAG9 = 0,
                                FLAG10 = 0,
                                VENDOR_ID = VendorInfo.ID
                            };
                            db.TBL_ACCOUNTS.Add(objDisCommPer);
                            await db.SaveChangesAsync();
                            decimal DistBal = 0;
                            decimal UpdateDistBal = 0;
                            var DistributorBal =await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == Dis_ID);
                            if (DistributorBal != null)
                            {
                                decimal.TryParse(DistributorBal.BALANCE.ToString(), out DistBal);
                                UpdateDistBal = DistBal + DisGapCommAmt;
                                DistributorBal.BALANCE = UpdateDistBal;
                                db.Entry(DistributorBal).State = System.Data.Entity.EntityState.Modified;
                                await db.SaveChangesAsync();
                            }
                            else
                            {
                                DistributorBal.BALANCE = DistBal;
                                db.Entry(DistributorBal).State = System.Data.Entity.EntityState.Modified;
                                await db.SaveChangesAsync();
                            }
                        }
                        #endregion
                        //var SuperComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                 join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
                        //                  on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
                        //                 where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == SUP_MEM_ID && commslabMob.OPERATOR_CODE == serviceprovider
                        //                 select new
                        //                 {
                        //                     SLN = commslabMob.SLN,
                        //                     commPer = commslabMob.COMM_PERCENTAGE
                        //                 }).FirstOrDefault();
                        var SuperComm =await (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                         join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                          on detailscom.SLN equals commslabMob.SLAB_ID
                                         join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                                         where commslabMob.OPERATOR_CODE == serviceprovider && detailscom.SLAB_TYPE == 2 && mem.MEMBER_ROLE == 3 && commslabMob.MEM_ID == WHT_MEM_ID
                                              select new
                                         {
                                             SLN = commslabMob.SLN,
                                             commPer = commslabMob.SUPER_COM_PER,
                                             commType = commslabMob.COMMISSION_TYPE
                                         }).FirstOrDefaultAsync();
                        //decimal SupGapComm = decimal.Parse(SuperComm.commPer.ToString()) - decimal.Parse(DistributorComm.commPer.ToString());

                        #region Super Commission
                        //var tbl_accountSuper =await db.TBL_ACCOUNTS.Where(z => z.MEM_ID == SUP_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefaultAsync();
                        //if (tbl_accountSuper != null)
                        //{
                        //    var Supmembtype =await (from mm in db.TBL_MASTER_MEMBER
                        //                       join
                        //                           rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                        //                       where mm.MEM_ID == SUP_MEM_ID
                        //                       select new
                        //                       {
                        //                           RoleId = mm.MEMBER_ROLE,
                        //                           roleName = rm.ROLE_NAME,
                        //                           Amount = mm.BALANCE
                        //                       }).FirstOrDefaultAsync();
                        //    decimal SupClosingAmt = tbl_accountSuper.CLOSING;
                        //    //GST and TDS Calculation for Super
                        //    decimal SupGapComm = 0;
                        //    //decimal SupGapCommAmt = (Trans_Amt * SupGapComm) / 100;
                        //    decimal USupGapCommAmt_Val = 0;
                        //    decimal USuper_TDSCal = 0;
                        //    decimal SupGapCommAmt = 0;
                        //    decimal Sup_CalCulate_GST = 0;
                        //    if (SuperComm.commType == "FIXED")
                        //    {
                        //        SupGapComm = decimal.Parse(SuperComm.commPer.ToString());
                        //        //decimal SupGapCommAmt = (Trans_Amt * SupGapComm) / 100;
                        //        USupGapCommAmt_Val = SupGapComm;
                        //        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                        //        {
                        //            USuper_TDSCal = decimal.Round(((USupGapCommAmt_Val * GST_AMount) / 100), 2);
                        //            if (MemberGST != null)
                        //            {
                        //                Sup_CalCulate_GST = decimal.Round(((USupGapCommAmt_Val * gst_VAlue) / 100), 2);
                        //                //SupGapCommAmt = USupGapCommAmt_Val + Sup_CalCulate_GST - USuper_TDSCal;
                        //                SupGapCommAmt = USupGapCommAmt_Val - USuper_TDSCal;
                        //            }
                        //            else
                        //            {
                        //                Sup_CalCulate_GST = 0;
                        //                //SupGapCommAmt = USupGapCommAmt_Val + Sup_CalCulate_GST - USuper_TDSCal;
                        //                SupGapCommAmt = USupGapCommAmt_Val - USuper_TDSCal;
                        //            }
                        //        }
                        //        else
                        //        {
                        //            USuper_TDSCal = decimal.Round(((USupGapCommAmt_Val - GST_AMount)));
                        //            if (MemberGST != null)
                        //            {
                        //                Sup_CalCulate_GST = decimal.Round(((USupGapCommAmt_Val - gst_VAlue)));
                        //                //SupGapCommAmt = USupGapCommAmt_Val + Sup_CalCulate_GST - USuper_TDSCal;
                        //                SupGapCommAmt = USupGapCommAmt_Val - USuper_TDSCal;
                        //            }
                        //            else
                        //            {
                        //                Sup_CalCulate_GST = 0;
                        //                //SupGapCommAmt = USupGapCommAmt_Val + Sup_CalCulate_GST - USuper_TDSCal;
                        //                SupGapCommAmt = USupGapCommAmt_Val - USuper_TDSCal;
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        SupGapComm = decimal.Parse(SuperComm.commPer.ToString());
                        //        //decimal SupGapCommAmt = (Trans_Amt * SupGapComm) / 100;
                        //        USupGapCommAmt_Val = (Trans_Amt * SupGapComm) / 100;
                        //        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                        //        {
                        //            USuper_TDSCal = decimal.Round(((USupGapCommAmt_Val * GST_AMount) / 100), 2);
                        //            if (MemberGST != null)
                        //            {
                        //                Sup_CalCulate_GST = decimal.Round(((USupGapCommAmt_Val * gst_VAlue) / 100), 2);
                        //                //SupGapCommAmt = USupGapCommAmt_Val + Sup_CalCulate_GST - USuper_TDSCal;
                        //                SupGapCommAmt = USupGapCommAmt_Val - USuper_TDSCal;
                        //            }
                        //            else
                        //            {
                        //                Sup_CalCulate_GST = 0;
                        //                //SupGapCommAmt = USupGapCommAmt_Val + Sup_CalCulate_GST - USuper_TDSCal;
                        //                SupGapCommAmt = USupGapCommAmt_Val - USuper_TDSCal;
                        //            }
                        //        }
                        //        else
                        //        {
                        //            USuper_TDSCal = decimal.Round(((USupGapCommAmt_Val * GST_AMount)));
                        //            if (MemberGST != null)
                        //            {
                        //                Sup_CalCulate_GST = decimal.Round(((USupGapCommAmt_Val - gst_VAlue)));
                        //                //SupGapCommAmt = USupGapCommAmt_Val + Sup_CalCulate_GST - USuper_TDSCal;
                        //                SupGapCommAmt = USupGapCommAmt_Val - USuper_TDSCal;
                        //            }
                        //            else
                        //            {
                        //                Sup_CalCulate_GST = 0;
                        //                //SupGapCommAmt = USupGapCommAmt_Val + Sup_CalCulate_GST - USuper_TDSCal;
                        //                SupGapCommAmt = USupGapCommAmt_Val - USuper_TDSCal;
                        //            }
                        //        }
                        //    }
                        //    SPR_Comm_Amt = SupGapCommAmt;
                        //    SPR_TaxAMT = USuper_TDSCal;
                        //    SUPER_GST_Comm_Amt = Sup_CalCulate_GST;
                        //    decimal CommSupAddClosing = tbl_accountSuper.CLOSING + SupGapCommAmt;
                        //    long Sup_ID = long.Parse(SUP_MEM_ID.ToString());
                        //    TBL_ACCOUNTS objSupCommPer = new TBL_ACCOUNTS()
                        //    {
                        //        API_ID = 0,
                        //        MEM_ID = Sup_ID,
                        //        MEMBER_TYPE = Supmembtype.roleName,
                        //        TRANSACTION_TYPE = rechargeType,
                        //        TRANSACTION_DATE = System.DateTime.Now,
                        //        TRANSACTION_TIME = DateTime.Now,
                        //        DR_CR = "CR",
                        //        AMOUNT = Trans_Amt,
                        //        NARRATION = "Commission",
                        //        OPENING = SupClosingAmt,
                        //        CLOSING = CommSupAddClosing,
                        //        REC_NO = 0,
                        //        COMM_AMT = SupGapCommAmt,
                        //        GST = (float)Sup_CalCulate_GST,
                        //        TDS = (float)USuper_TDSCal,
                        //        IPAddress = IpAddress,
                        //        TDS_PERCENTAGE = taxMaster.TAX_VALUE,
                        //        GST_PERCENTAGE = GST_Master.TAX_VALUE,
                        //        WHITELEVEL_ID = (long)WHT_MEM_ID,
                        //        SUPER_ID = (long)SUP_MEM_ID,
                        //        DISTRIBUTOR_ID = (long)DIS_MEM_ID,
                        //        SERVICE_ID = UtilityId.ID,
                        //        CORELATIONID=sRandomOTP
                        //    };
                        //    db.TBL_ACCOUNTS.Add(objSupCommPer);
                        //    await db.SaveChangesAsync();
                        //    decimal SprBal = 0;
                        //    decimal UpdatSprBal = 0;
                        //    var SuperBal =await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == Sup_ID);
                        //    if (SuperBal != null)
                        //    {
                        //        decimal.TryParse(SuperBal.BALANCE.ToString(), out SprBal);
                        //        UpdatSprBal = SprBal + SupGapCommAmt;
                        //        SuperBal.BALANCE = UpdatSprBal;
                        //        db.Entry(SuperBal).State = System.Data.Entity.EntityState.Modified;
                        //        await db.SaveChangesAsync();
                        //    }
                        //    else
                        //    {
                        //        SuperBal.BALANCE = SprBal;
                        //        db.Entry(SuperBal).State = System.Data.Entity.EntityState.Modified;
                        //        await db.SaveChangesAsync();
                        //    }
                        //}
                        #endregion
                        //var WhiteComm =await (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                 join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
                        //                  on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
                        //                 where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == 0 && commslabMob.OPERATOR_CODE == serviceprovider
                        //                 select new
                        //                 {
                        //                     SLN = commslabMob.SLN,
                        //                     commPer = commslabMob.COMM_PERCENTAGE,
                        //                     commType = commslabMob.COMMISSION_TYPE
                        //                 }).FirstOrDefaultAsync();

                        //var WhiteComm11 = await (from WLPCom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                        //                             //join detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB on WLPCom.SLN equals detailscom.BILLPAYMENT_SLAB
                        //                         join detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB on WLPCom.SLN equals detailscom.BILLPAYMENT_SLAB
                        //                         join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
                        //                       on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
                        //                         join mem in db.TBL_MASTER_MEMBER on detailscom.WHITE_LEVEL_ID equals mem.MEM_ID
                        //                         where commslabMob.OPERATOR_CODE == serviceprovider && detailscom.INTRODUCER_ID == 0 && detailscom.WHITE_LEVEL_ID == WHT_MEM_ID
                        //                         select new
                        //                         {
                        //                             SLN = commslabMob.SLN,
                        //                             SLAB_ID = commslabMob.SLAB_ID,
                        //                             commPer = commslabMob.COMM_PERCENTAGE,
                        //                             commType = commslabMob.COMMISSION_TYPE,
                        //                             SlabTDS = WLPCom.SLAB_TDS
                        //                         }).FirstOrDefaultAsync();



                        var WhiteComm = await (from WLLMain in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                            join WLCOMTag in db.TBL_COMM_SLAB_UTILITY_RECHARGE on WLLMain.SLN equals WLCOMTag.SLAB_ID
                                            join WLCOMMVAL in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                                            on WLCOMTag.SLAB_ID equals WLCOMMVAL.BILLPAYMENT_SLAB
                                            where WLCOMMVAL.WHITE_LEVEL_ID == WHT_MEM_ID && WLCOMTag.OPERATOR_CODE == serviceprovider && WLCOMMVAL.INTRODUCER_ID == 0
                                               select new
                                            {
                                                //COMMTYPE = WLCOMTag.COMMISSION_TYPE,
                                                //COMM_VALUE = WLCOMTag.COMM_PERCENTAGE,
                                                //SLAB_TDS = WLLMain.SLAB_TDS
                                                SLN = WLCOMTag.SLN,
                                                   SLAB_ID = WLCOMTag.SLAB_ID,
                                                   commPer = WLCOMTag.COMM_PERCENTAGE,
                                                   commType = WLCOMTag.COMMISSION_TYPE,
                                                   SlabTDS = WLLMain.SLAB_TDS
                                            }).FirstOrDefaultAsync();

                        //decimal WL_Com = decimal.Parse(MerchantComm.commPer.ToString()) + decimal.Parse(DistributorComm.commPer.ToString()) + decimal.Parse(SuperComm.commPer.ToString());
                        decimal WL_Com = decimal.Parse(MerchantComm.commPer.ToString()) + decimal.Parse(DistributorComm.commPer.ToString()) ;
                        decimal WTLGapComm = decimal.Parse(WhiteComm.commPer.ToString()) - WL_Com;
                        //decimal WTLGapComm = decimal.Parse(WhiteComm.commPer.ToString()) - decimal.Parse(SuperComm.commPer.ToString());
                        //decimal WTLGapCommAmt = (Trans_Amt * WTLGapComm) / 100;
                        #region White level Commission payment Structure
                        //var tbl_accountWhiteLevel =await db.TBL_ACCOUNTS.Where(z => z.MEM_ID == WHT_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefaultAsync();
                        var tbl_accountWhiteLevel = await db.TBL_ACCOUNTS.Where(z => z.MEM_ID == WHT_MEM_ID).OrderByDescending(z => z.ACC_NO).FirstOrDefaultAsync();
                        if (tbl_accountWhiteLevel != null)
                        {
                            var WLmembtype =await (from mm in db.TBL_MASTER_MEMBER
                                              join
                                                  rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                              where mm.MEM_ID == WHT_MEM_ID
                                              select new
                                              {
                                                  StateId=mm.STATE_ID,
                                                  RoleId = mm.MEMBER_ROLE,
                                                  roleName = rm.ROLE_NAME,
                                                  Amount = mm.BALANCE
                                              }).FirstOrDefaultAsync();
                            decimal WLClosingAmt = tbl_accountWhiteLevel.CLOSING;
                            //TDS And Gst Calculation fir whitelevel

                            #region Whitelable Commission
                            var UTLWLPGSTSetVal = (from GstMem in db.TBL_MASTER_MEMBER
                                                join StGstVal in db.TBL_STATES on GstMem.STATE_ID equals StGstVal.STATEID
                                                where GstMem.MEM_ID == WHT_MEM_ID
                                                select new
                                                {
                                                    StateGstVal = StGstVal.SGST,
                                                    CentralGstVal = StGstVal.CGST,
                                                    IGstVal = StGstVal.IGST
                                                }).FirstOrDefault();
                            decimal UTLWLPStateGstVal = 0;
                            decimal UTLWLPCentralGstVal = 0;
                            decimal UTLWLPIntsGstVal = 0;
                            decimal.TryParse(UTLWLPGSTSetVal.StateGstVal.ToString(), out UTLWLPStateGstVal);
                            decimal.TryParse(UTLWLPGSTSetVal.CentralGstVal.ToString(), out UTLWLPCentralGstVal);
                            decimal.TryParse(UTLWLPGSTSetVal.IGstVal.ToString(), out UTLWLPIntsGstVal);
                            decimal UTLWLPCAlCStateGSt = 0;
                            decimal UTLWLPCAlCCenterGSt = 0;
                            decimal UTLWLPCAlCInstGSt = 0;
                            #endregion


                            decimal UWTLGap_CommAmtValue = 0;
                            decimal UWhiteLTDS = 0;
                            decimal WTLGapCommAmt = 0;
                            decimal WL_CalculateGST = 0;
                            if (WhiteComm.commType == "FIXED")
                            {
                                UWTLGap_CommAmtValue = WTLGapComm;
                                if (WhiteComm.SlabTDS == "Yes")
                                {
                                    if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                    {

                                        UWhiteLTDS = decimal.Round(((UWTLGap_CommAmtValue * GST_AMount) / 100), 2);
                                        if (MemberGST != null)
                                        {
                                            UTLWLPCAlCStateGSt = Math.Round(((UWTLGap_CommAmtValue * UTLWLPStateGstVal) / 100), 2);
                                            UTLWLPCAlCCenterGSt = Math.Round(((UWTLGap_CommAmtValue * UTLWLPCentralGstVal) / 100), 2);
                                            UTLWLPCAlCInstGSt = Math.Round(((UWTLGap_CommAmtValue * UTLWLPIntsGstVal) / 100), 2);
                                            //WL_CalculateGST = decimal.Round(((UWTLGap_CommAmtValue * gst_VAlue) / 100), 2);                                        
                                            WTLGapCommAmt = UWTLGap_CommAmtValue - UWhiteLTDS;
                                        }
                                        else
                                        {
                                            //WL_CalculateGST = 0;
                                            UTLWLPCAlCStateGSt = 0;
                                            UTLWLPCAlCCenterGSt = 0;
                                            UTLWLPCAlCInstGSt = 0;
                                            WTLGapCommAmt = UWTLGap_CommAmtValue - UWhiteLTDS;
                                        }

                                    }
                                    else
                                    {
                                        UWhiteLTDS = decimal.Round(((UWTLGap_CommAmtValue - GST_AMount)));
                                        if (MemberGST != null)
                                        {
                                            //WL_CalculateGST = decimal.Round(((UWTLGap_CommAmtValue - gst_VAlue)));
                                            UTLWLPCAlCStateGSt = Math.Round(((UWTLGap_CommAmtValue - UTLWLPStateGstVal)), 2);
                                            UTLWLPCAlCCenterGSt = Math.Round(((UWTLGap_CommAmtValue - UTLWLPCentralGstVal)), 2);
                                            UTLWLPCAlCInstGSt = Math.Round(((UWTLGap_CommAmtValue - UTLWLPIntsGstVal)), 2);
                                            WTLGapCommAmt = UWTLGap_CommAmtValue - UWhiteLTDS;
                                        }
                                        else
                                        {
                                            //WL_CalculateGST = 0;
                                            UTLWLPCAlCStateGSt = 0;
                                            UTLWLPCAlCCenterGSt = 0;
                                            UTLWLPCAlCInstGSt = 0;
                                            WTLGapCommAmt = UWTLGap_CommAmtValue - UWhiteLTDS;
                                        }
                                    }
                                }
                                else
                                {
                                    UWhiteLTDS = 0;
                                    if (MemberGST != null)
                                    {
                                        //WL_CalculateGST = decimal.Round(((UWTLGap_CommAmtValue - gst_VAlue)));
                                        UTLWLPCAlCStateGSt = Math.Round(((UWTLGap_CommAmtValue - UTLWLPStateGstVal)), 2);
                                        UTLWLPCAlCCenterGSt = Math.Round(((UWTLGap_CommAmtValue - UTLWLPCentralGstVal)), 2);
                                        UTLWLPCAlCInstGSt = Math.Round(((UWTLGap_CommAmtValue - UTLWLPIntsGstVal)), 2);
                                        WTLGapCommAmt = UWTLGap_CommAmtValue - UWhiteLTDS;
                                    }
                                    else
                                    {
                                        //WL_CalculateGST = 0;
                                        UTLWLPCAlCStateGSt = 0;
                                        UTLWLPCAlCCenterGSt = 0;
                                        UTLWLPCAlCInstGSt = 0;
                                        WTLGapCommAmt = UWTLGap_CommAmtValue - UWhiteLTDS;
                                    }
                                }
                               
                            }
                            else
                            {
                                UWTLGap_CommAmtValue = (Trans_Amt * WTLGapComm) / 100;
                                if (WhiteComm.SlabTDS == "Yes")
                                {
                                    if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                    {
                                        UWhiteLTDS = decimal.Round(((UWTLGap_CommAmtValue * GST_AMount) / 100), 2);
                                        if (MemberGST != null)
                                        {
                                            UTLWLPCAlCStateGSt = Math.Round(((UWTLGap_CommAmtValue * UTLWLPStateGstVal) / 100), 2);
                                            UTLWLPCAlCCenterGSt = Math.Round(((UWTLGap_CommAmtValue * UTLWLPCentralGstVal) / 100), 2);
                                            UTLWLPCAlCInstGSt = Math.Round(((UWTLGap_CommAmtValue * UTLWLPIntsGstVal) / 100), 2);
                                            //WL_CalculateGST = decimal.Round(((UWTLGap_CommAmtValue * gst_VAlue) / 100), 2);
                                            WTLGapCommAmt = UWTLGap_CommAmtValue - UWhiteLTDS;
                                        }
                                        else
                                        {
                                            //WL_CalculateGST = 0;
                                            UTLWLPCAlCStateGSt = 0;
                                            UTLWLPCAlCCenterGSt = 0;
                                            UTLWLPCAlCInstGSt = 0;
                                            WTLGapCommAmt = UWTLGap_CommAmtValue - UWhiteLTDS;
                                        }
                                    }
                                    else
                                    {
                                        UWhiteLTDS = decimal.Round(((UWTLGap_CommAmtValue - GST_AMount)));
                                        if (MemberGST != null)
                                        {
                                            //WL_CalculateGST = decimal.Round(((UWTLGap_CommAmtValue - gst_VAlue)));
                                            UTLWLPCAlCStateGSt = Math.Round(((UWTLGap_CommAmtValue - UTLWLPStateGstVal)), 2);
                                            UTLWLPCAlCCenterGSt = Math.Round(((UWTLGap_CommAmtValue - UTLWLPCentralGstVal)), 2);
                                            UTLWLPCAlCInstGSt = Math.Round(((UWTLGap_CommAmtValue - UTLWLPIntsGstVal)), 2);
                                            WTLGapCommAmt = UWTLGap_CommAmtValue - UWhiteLTDS;
                                        }
                                        else
                                        {
                                            //WL_CalculateGST = 0;
                                            UTLWLPCAlCStateGSt = 0;
                                            UTLWLPCAlCCenterGSt = 0;
                                            UTLWLPCAlCInstGSt = 0;
                                            WTLGapCommAmt = UWTLGap_CommAmtValue - UWhiteLTDS;
                                        }
                                    }
                                }
                                else
                                {
                                    UWhiteLTDS = 0;
                                    if (MemberGST != null)
                                    {
                                        //WL_CalculateGST = decimal.Round(((UWTLGap_CommAmtValue - gst_VAlue)));
                                        UTLWLPCAlCStateGSt = Math.Round(((UWTLGap_CommAmtValue - UTLWLPStateGstVal)), 2);
                                        UTLWLPCAlCCenterGSt = Math.Round(((UWTLGap_CommAmtValue - UTLWLPCentralGstVal)), 2);
                                        UTLWLPCAlCInstGSt = Math.Round(((UWTLGap_CommAmtValue - UTLWLPIntsGstVal)), 2);
                                        WTLGapCommAmt = UWTLGap_CommAmtValue - UWhiteLTDS;
                                    }
                                    else
                                    {
                                        //WL_CalculateGST = 0;
                                        UTLWLPCAlCStateGSt = 0;
                                        UTLWLPCAlCCenterGSt = 0;
                                        UTLWLPCAlCInstGSt = 0;
                                        WTLGapCommAmt = UWTLGap_CommAmtValue - UWhiteLTDS;
                                    }
                                }
                                
                            }
                            WHITE_Comm_Amt = WTLGapCommAmt;
                            WHITE_TaxAMT = UWhiteLTDS;
                            //WHILE_GST_Comm_Amt = WL_CalculateGST;
                            WHILE_GST_Comm_Amt = (UTLWLPCAlCStateGSt+ UTLWLPCAlCCenterGSt + UTLWLPCAlCInstGSt);
                            //decimal SubAmt = ClosingAmt - Trans_Amt;                  
                            decimal CommWLAddClosing = tbl_accountWhiteLevel.CLOSING + WTLGapCommAmt;
                            long WL_ID = long.Parse(WHT_MEM_ID.ToString());
                            TBL_ACCOUNTS objWLCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = WL_ID,
                                MEMBER_TYPE = WLmembtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = WLClosingAmt,
                                CLOSING = CommWLAddClosing,
                                REC_NO = 0,
                                COMM_AMT = WTLGapCommAmt,
                                //GST = (float)WL_CalculateGST,
                                GST = (float)WHILE_GST_Comm_Amt,
                                TDS = (float)UWhiteLTDS,
                                IPAddress = IpAddress,
                                TDS_PERCENTAGE = taxMaster.TAX_VALUE,
                                GST_PERCENTAGE = GST_Master.TAX_VALUE,
                                WHITELEVEL_ID = (long)WHT_MEM_ID,
                                //SUPER_ID = (long)SUP_MEM_ID,
                                SUPER_ID = 0,
                                DISTRIBUTOR_ID = (long)DIS_MEM_ID,
                                SERVICE_ID = UtilityId.ID,
                                CORELATIONID=sRandomOTP,
                                REC_COMM_TYPE = WhiteComm.commType,
                                COMM_VALUE = WhiteComm.commPer,
                                NET_COMM_AMT = WHITE_Comm_Amt,
                                TDS_DR_COMM_AMT = WHITE_TaxAMT,
                                CGST_COMM_AMT_INPUT = UTLWLPCAlCCenterGSt,
                                CGST_COMM_AMT_OUTPUT = 0,
                                SGST_COMM_AMT_INPUT = UTLWLPCAlCStateGSt,
                                SGST_COMM_AMT_OUTPUT = 0,
                                IGST_COMM_AMT_INPUT = UTLWLPCAlCInstGSt,
                                IGST_COMM_AMT_OUTPUT = 0,
                                TOTAL_GST_COMM_AMT_INPUT = WHILE_GST_Comm_Amt,
                                TOTAL_GST_COMM_AMT_OUTPUT = 0,
                                TDS_RATE = GST_AMount,
                                CGST_RATE = UTLWLPCentralGstVal,
                                SGST_RATE = UTLWLPStateGstVal,
                                IGST_RATE = UTLWLPIntsGstVal,
                                TOTAL_GST_RATE = (UTLWLPCentralGstVal + UTLWLPStateGstVal + UTLWLPIntsGstVal),
                                COMM_SLAB_ID = Oper_Id_Val,
                                STATE_ID = WLmembtype.StateId,
                                FLAG1 = 0,
                                FLAG2 = 0,
                                FLAG3 = 0,
                                FLAG4 = 0,
                                FLAG5 = 0,
                                FLAG6 = 0,
                                FLAG7 = 0,
                                FLAG8 = 0,
                                FLAG9 = 0,
                                FLAG10 = 0,
                                VENDOR_ID = VendorInfo.ID
                            };
                            db.TBL_ACCOUNTS.Add(objWLCommPer);
                            await db.SaveChangesAsync();
                            decimal WHTLBal = 0;
                            decimal UpdatWHITEBal = 0;
                            var WHITELabelBal =await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == WL_ID);
                            if (WHITELabelBal != null)
                            {
                                decimal.TryParse(WHITELabelBal.BALANCE.ToString(), out WHTLBal);
                                UpdatWHITEBal = WHTLBal + WTLGapCommAmt;
                                WHITELabelBal.BALANCE = UpdatWHITEBal;
                                db.Entry(WHITELabelBal).State = System.Data.Entity.EntityState.Modified;
                                await db.SaveChangesAsync();
                            }
                            else
                            {
                                WHITELabelBal.BALANCE = WHTLBal;
                                db.Entry(WHITELabelBal).State = System.Data.Entity.EntityState.Modified;
                                await db.SaveChangesAsync();
                            }
                        }
                        #endregion

                        #region Update InstantpayTable Recharge And Response Table

                        var updateInstantres =await db.TBL_INSTANTPAY_RECHARGE_RESPONSE.FirstOrDefaultAsync(x => x.CORELATIONID == sRandomOTP);
                        if (updateInstantres != null)
                        {
                            updateInstantres.Ipay_Id = apiRES.txnId;
                            updateInstantres.AgentId = "";
                            updateInstantres.Opr_Id = apiRES.name;
                            updateInstantres.AccountNo = apiRES.accountNumber;
                            updateInstantres.Sp_Key = apiRES.name;
                            updateInstantres.Trans_Amt = decimal.Parse(apiRES.serviceTax.ToString());
                            updateInstantres.Charged_Amt = decimal.Parse(apiRES.ifscCode.ToString());
                            updateInstantres.Opening_Balance = decimal.Parse(apiRES.impsRespCode.ToString());
                            updateInstantres.DateVal = System.DateTime.Now;
                            //updateInstantres.Status = apiRES.txnStatus;
                            updateInstantres.Status = "SUCCESS";
                            //SUCCESS
                            updateInstantres.Res_Code = apiRES.impsRespMessage;
                            //updateInstantres.res_msg = apiRES.timestamp;
                            updateInstantres.res_msg = "Transaction Successful";
                            updateInstantres.RechargeType = rechargeType;
                            updateInstantres.RechargeResponse = APIResponse;
                            updateInstantres.REC_COMM_TYPE = MerchantComm.commType;
                            updateInstantres.MER_COMM_VALUE = MerchantComm.commPer;
                            updateInstantres.MER_COMM_AMT = MER_Comm_Amt;
                            updateInstantres.MER_TDS_DR_COMM_AMT = MER_TaxAMT;
                            updateInstantres.DIST_ID = long.Parse(DIS_MEM_ID.ToString());
                            updateInstantres.DIST_COMM_VALUE = DistributorComm.commPer;
                            updateInstantres.DIST_COMM_AMT = DEST_Comm_Amt;
                            updateInstantres.DIST_TDS_DR_COMM_AMT = DEST_TaxAMT;
                            //updateInstantres.SUPER_ID = long.Parse(SUP_MEM_ID.ToString());
                            //updateInstantres.SUPER_COMM_VALUE = SuperComm.commPer;
                            //updateInstantres.SUPER_COMM_AMT = SPR_Comm_Amt;
                            //updateInstantres.SUPER_TDS_DR_COMM_AMT = SPR_TaxAMT;
                            updateInstantres.SUPER_ID = 0;
                            updateInstantres.SUPER_COMM_VALUE = 0;
                            updateInstantres.SUPER_COMM_AMT = 0;
                            updateInstantres.SUPER_TDS_DR_COMM_AMT = 0;
                            updateInstantres.WHITELABEL_ID = long.Parse(WHT_MEM_ID.ToString());
                            updateInstantres.WHITELABEL_VALUE = WhiteComm.commPer;
                            updateInstantres.WHITELABEL_COMM_AMT = WHITE_Comm_Amt;
                            updateInstantres.WHITELABEL_TDS_DR_COMM_AMT = WHITE_TaxAMT;
                            updateInstantres.TDS_RATE = taxMaster.TAX_VALUE;
                            updateInstantres.CORELATIONID = sRandomOTP;
                            updateInstantres.ERROR_TYPE = null;
                            updateInstantres.ISREVERSE = "No";
                            updateInstantres.DOMAIN_NAME = IpAddress;
                            updateInstantres.ISCOMMISSIONDISBURSE = "Yes";
                            updateInstantres.COMMISSIONDISBURSEDATE = DateTime.Now;
                            updateInstantres.GST_RATE = GST_Master.TAX_VALUE;
                            updateInstantres.MER_COMM_GST_AMT = MER_GST_Comm_Amt;
                            updateInstantres.DIST_COMM_GST_AMT = DIST_GST_Comm_Amt;
                            updateInstantres.SUPER_COMM_GST_AMT = SUPER_GST_Comm_Amt;
                            updateInstantres.WHITELABEL_COMM_GST_AMT = WHILE_GST_Comm_Amt;
                            updateInstantres.MER_INVOICE_ID = 0;
                            updateInstantres.MER_CANCEL_INVOICE = "";
                            updateInstantres.DIST_INVOICE_ID = 0;
                            updateInstantres.DIST_CANCEL_INVOICE = "";
                            updateInstantres.SUPER_INVOICE_ID = 0;
                            updateInstantres.SUPER_CANCEL_INVOICE = "";
                            updateInstantres.WHITELABEL_INVOICE_ID = 0;
                            updateInstantres.WHITELABEL_CANCEL_INVOICE = "";
                            updateInstantres.SLAB_ID = MerchantComm.SLAB_ID;
                            db.Entry(updateInstantres).State = System.Data.Entity.EntityState.Modified;
                            await db.SaveChangesAsync();
                        }
                        #endregion

                        ContextTransaction.Commit();
                        return true;
                    }
                    else if (status == "WATER")
                    {
                        //var MerchantComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                    join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
                        //                    on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
                        //                    where detailscom.WHITE_LEVEL_ID == DIS_MEM_ID && detailscom.INTRODUCER_ID == Mem_ID && commslabMob.OPERATOR_CODE == serviceprovider
                        //                    select new
                        //                    {
                        //                        SLN = commslabMob.SLN,
                        //                        commPer = commslabMob.COMM_PERCENTAGE
                        //                    }).FirstOrDefault();
                        var MerchantComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                            join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                            on detailscom.SLN equals commslabMob.SLAB_ID
                                            join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                                            where commslabMob.OPERATOR_CODE == serviceprovider && mem.MEMBER_ROLE == 5
                                            select new
                                            {
                                                SLN = commslabMob.SLN,
                                                commPer = commslabMob.MERCHANT_COM_PER,
                                                commType = commslabMob.COMMISSION_TYPE
                                            }).FirstOrDefault();
                        #region Retailer Commission                        
                        var membtype = (from mm in db.TBL_MASTER_MEMBER
                                        join
                                            rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                        where mm.MEM_ID == Mem_ID
                                        select new
                                        {
                                            RoleId = mm.MEMBER_ROLE,
                                            roleName = rm.ROLE_NAME,
                                            Amount = mm.BALANCE
                                        }).FirstOrDefault();
                        var tbl_account = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == Mem_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        if (tbl_account != null)
                        {
                            decimal ClosingAmt = tbl_account.CLOSING;
                            decimal SubAmt = ClosingAmt - Trans_Amt;
                            TBL_ACCOUNTS objaccnt = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Mem_ID,
                                MEMBER_TYPE = membtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "DR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Recharge",
                                OPENING = ClosingAmt,
                                CLOSING = SubAmt,
                                REC_NO = 0,
                                COMM_AMT = 0,
                                GST = 0,
                                TDS = 0,
                                IPAddress = IpAddress,
                                TDS_PERCENTAGE = 0,
                                GST_PERCENTAGE = 0,
                                WHITELEVEL_ID = (long)WHT_MEM_ID,
                                //SUPER_ID = (long)SUP_MEM_ID,
                                SUPER_ID = 0,
                                DISTRIBUTOR_ID = (long)DIS_MEM_ID
                            };
                            db.TBL_ACCOUNTS.Add(objaccnt);
                            db.SaveChanges();
                            // For TDS and GST Calculaiton for Merchant
                            //decimal getPer = (Trans_Amt * MerchantComm.commPer) / 100;
                            decimal WA_getPer_Value = 0;
                            decimal WA_Merchant_TDS = 0;
                            decimal getPer = 0;
                            decimal WAMer_CalculateGST = 0;
                            if (MerchantComm.commType == "FIXED")
                            {
                                WA_getPer_Value = MerchantComm.commPer;
                                if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                {
                                    WA_Merchant_TDS = decimal.Round(((WA_getPer_Value * GST_AMount) / 100), 2);
                                    if (MemberGST != null)
                                    {
                                        WAMer_CalculateGST = decimal.Round(((WA_getPer_Value * gst_VAlue) / 100), 2);
                                        getPer = WA_getPer_Value + WAMer_CalculateGST - WA_Merchant_TDS;
                                    }
                                    else
                                    {
                                        WAMer_CalculateGST = 0;
                                        getPer = WA_getPer_Value + WAMer_CalculateGST - WA_Merchant_TDS;
                                    }

                                }
                                else
                                {
                                    WA_Merchant_TDS = decimal.Round(((WA_getPer_Value * GST_AMount)));
                                    if (MemberGST != null)
                                    {
                                        WAMer_CalculateGST = decimal.Round(((WA_getPer_Value - gst_VAlue)));
                                        getPer = WA_getPer_Value + WAMer_CalculateGST - WA_Merchant_TDS;
                                    }
                                    else
                                    {
                                        WAMer_CalculateGST = 0;
                                        getPer = WA_getPer_Value + WAMer_CalculateGST - WA_Merchant_TDS;
                                    }

                                }
                            }
                            else
                            {
                                WA_getPer_Value = (Trans_Amt * MerchantComm.commPer) / 100;
                                if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                {
                                    WA_Merchant_TDS = decimal.Round(((WA_getPer_Value * GST_AMount) / 100), 2);
                                    if (MemberGST != null)
                                    {
                                        WAMer_CalculateGST = decimal.Round(((WA_getPer_Value * gst_VAlue) / 100), 2);
                                        getPer = WA_getPer_Value + WAMer_CalculateGST - WA_Merchant_TDS;
                                    }
                                    else
                                    {
                                        WAMer_CalculateGST = 0;
                                        getPer = WA_getPer_Value + WAMer_CalculateGST - WA_Merchant_TDS;
                                    }
                                }
                                else
                                {
                                    WA_Merchant_TDS = decimal.Round(((WA_getPer_Value - GST_AMount)));
                                    if (MemberGST != null)
                                    {
                                        WAMer_CalculateGST = decimal.Round(((WA_getPer_Value - gst_VAlue)));
                                        getPer = WA_getPer_Value + WAMer_CalculateGST - WA_Merchant_TDS;
                                    }
                                    else
                                    {
                                        WAMer_CalculateGST = 0;
                                        getPer = WA_getPer_Value + WAMer_CalculateGST - WA_Merchant_TDS;
                                    }
                                }
                            }
                            decimal CommAddClosing = SubAmt + getPer;
                            TBL_ACCOUNTS objCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Mem_ID,
                                MEMBER_TYPE = membtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = SubAmt,
                                CLOSING = CommAddClosing,
                                REC_NO = 0,
                                COMM_AMT = getPer,
                                GST = (float)WAMer_CalculateGST,
                                TDS = (float)WA_Merchant_TDS,
                                IPAddress = IpAddress,
                                TDS_PERCENTAGE = taxMaster.TAX_VALUE,
                                GST_PERCENTAGE = GST_Master.TAX_VALUE,
                                WHITELEVEL_ID = (long)WHT_MEM_ID,
                                //SUPER_ID = (long)SUP_MEM_ID,
                                SUPER_ID =0,
                                DISTRIBUTOR_ID = (long)DIS_MEM_ID
                            };
                            db.TBL_ACCOUNTS.Add(objCommPer);
                            db.SaveChanges();
                        }
                        #endregion

                        //var DistributorComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                       join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
                        //                       on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
                        //                       where detailscom.WHITE_LEVEL_ID == SUP_MEM_ID && detailscom.INTRODUCER_ID == DIS_MEM_ID && commslabMob.OPERATOR_CODE == serviceprovider
                        //                       select new
                        //                       {
                        //                           SLN = commslabMob.SLN,
                        //                           commPer = commslabMob.COMM_PERCENTAGE
                        //                       }).FirstOrDefault();
                        var DistributorComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                               join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                               on detailscom.SLN equals commslabMob.SLAB_ID
                                               join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                                               where commslabMob.OPERATOR_CODE == serviceprovider && mem.MEMBER_ROLE == 4
                                               select new
                                               {
                                                   SLN = commslabMob.SLN,
                                                   commPer = commslabMob.DISTRIBUTOR_COM_PER,
                                                   commType = commslabMob.COMMISSION_TYPE
                                               }).FirstOrDefault();
                        #region Distributor Commission
                        //decimal DisGapComm = decimal.Parse(DistributorComm.commPer.ToString()) - decimal.Parse(MerchantComm.commPer.ToString());
                        decimal DisGapComm = decimal.Parse(DistributorComm.commPer.ToString());
                        //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;
                        var tbl_accountDistributor = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == DIS_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        if (tbl_accountDistributor != null)
                        {
                            var CheckDisCommission = db.TBL_DETAILS_MEMBER_COMMISSION_SLAB.Where(x => x.INTRODUCER_ID == DIS_MEM_ID).FirstOrDefault();
                            var Dismembtype = (from mm in db.TBL_MASTER_MEMBER
                                               join
                                                   rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                               where mm.MEM_ID == DIS_MEM_ID
                                               select new
                                               {
                                                   RoleId = mm.MEMBER_ROLE,
                                                   roleName = rm.ROLE_NAME,
                                                   Amount = mm.BALANCE
                                               }).FirstOrDefault();
                            decimal DisClosingAmt = tbl_accountDistributor.CLOSING;
                            //decimal SubAmt = ClosingAmt - Trans_Amt;                   
                            //decimal DisgetPer = (Trans_Amt * checkDiscomm.CommPercentage) / 100;
                            // For GST AND TDS calculation for Dister for water
                            //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;

                            decimal WA_DisGapComm_Amt = 0;
                            decimal WA_Dis_TDS_CAL = 0;
                            decimal DisGapCommAmt = 0;
                            decimal WADIST_GSTCal = 0;
                            if (DistributorComm.commType == "FIXED")
                            {
                                WA_DisGapComm_Amt = DistributorComm.commPer;
                                if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                {
                                    WA_Dis_TDS_CAL = decimal.Round(((WA_DisGapComm_Amt * GST_AMount) / 100), 2);
                                    if (MemberGST != null)
                                    {
                                        WADIST_GSTCal = decimal.Round(((WA_DisGapComm_Amt * gst_VAlue) / 100), 2);
                                        DisGapCommAmt = WA_DisGapComm_Amt + WADIST_GSTCal - WA_Dis_TDS_CAL;
                                    }
                                    else
                                    {
                                        WADIST_GSTCal = 0;
                                        DisGapCommAmt = WA_DisGapComm_Amt + WADIST_GSTCal - WA_Dis_TDS_CAL;
                                    }
                                }
                                else
                                {
                                    WA_Dis_TDS_CAL = decimal.Round(((WA_DisGapComm_Amt - GST_AMount)));
                                    if (MemberGST != null)
                                    {
                                        WADIST_GSTCal = decimal.Round(((WA_DisGapComm_Amt - gst_VAlue)));
                                        DisGapCommAmt = WA_DisGapComm_Amt + WADIST_GSTCal - WA_Dis_TDS_CAL;
                                    }
                                    else
                                    {
                                        WADIST_GSTCal = 0;
                                        DisGapCommAmt = WA_DisGapComm_Amt + WADIST_GSTCal - WA_Dis_TDS_CAL;
                                    }
                                }
                            }
                            else
                            {
                                WA_DisGapComm_Amt = (Trans_Amt * DisGapComm) / 100;
                                if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                {
                                    WA_Dis_TDS_CAL = decimal.Round(((WA_DisGapComm_Amt * GST_AMount) / 100), 2);
                                    if (MemberGST != null)
                                    {
                                        WADIST_GSTCal = decimal.Round(((WA_DisGapComm_Amt * gst_VAlue) / 100), 2);
                                        DisGapCommAmt = WA_DisGapComm_Amt + WADIST_GSTCal - WA_Dis_TDS_CAL;
                                    }
                                    else
                                    {
                                        WADIST_GSTCal = 0;
                                        DisGapCommAmt = WA_DisGapComm_Amt + WADIST_GSTCal - WA_Dis_TDS_CAL;
                                    }

                                }
                                else
                                {
                                    WA_Dis_TDS_CAL = decimal.Round(((WA_DisGapComm_Amt - GST_AMount)));
                                    if (MemberGST != null)
                                    {
                                        WADIST_GSTCal = decimal.Round(((WA_DisGapComm_Amt - gst_VAlue)));
                                        DisGapCommAmt = WA_DisGapComm_Amt + WADIST_GSTCal - WA_Dis_TDS_CAL;
                                    }
                                    else
                                    {
                                        WADIST_GSTCal = 0;
                                        DisGapCommAmt = WA_DisGapComm_Amt + WADIST_GSTCal - WA_Dis_TDS_CAL;
                                    }
                                }

                            }

                            decimal CommDisAddClosing = tbl_accountDistributor.CLOSING + DisGapCommAmt;
                            long Dis_ID = long.Parse(DIS_MEM_ID.ToString());
                            TBL_ACCOUNTS objDisCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Dis_ID,
                                MEMBER_TYPE = Dismembtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = DisClosingAmt,
                                CLOSING = CommDisAddClosing,
                                REC_NO = 0,
                                COMM_AMT = DisGapCommAmt,
                                GST = (float)WADIST_GSTCal,
                                TDS = (float)WA_Dis_TDS_CAL,
                                IPAddress = IpAddress,
                                TDS_PERCENTAGE = taxMaster.TAX_VALUE,
                                GST_PERCENTAGE = GST_Master.TAX_VALUE,
                                WHITELEVEL_ID = (long)WHT_MEM_ID,
                                //SUPER_ID = (long)SUP_MEM_ID,
                                SUPER_ID = 0,
                                DISTRIBUTOR_ID = (long)DIS_MEM_ID
                            };
                            db.TBL_ACCOUNTS.Add(objDisCommPer);
                            db.SaveChanges();
                        }
                        #endregion
                        //var SuperComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                 join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
                        //                  on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
                        //                 where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == SUP_MEM_ID && commslabMob.OPERATOR_CODE == serviceprovider
                        //                 select new
                        //                 {
                        //                     SLN = commslabMob.SLN,
                        //                     commPer = commslabMob.COMM_PERCENTAGE
                        //                 }).FirstOrDefault();
                        var SuperComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                         join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                          on detailscom.SLN equals commslabMob.SLAB_ID
                                         join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                                         where commslabMob.OPERATOR_CODE == serviceprovider && mem.MEMBER_ROLE == 3
                                         select new
                                         {
                                             SLN = commslabMob.SLN,
                                             commPer = commslabMob.SUPER_COM_PER,
                                             commType = commslabMob.COMMISSION_TYPE
                                         }).FirstOrDefault();
                        //decimal SupGapComm = decimal.Parse(SuperComm.commPer.ToString()) - decimal.Parse(DistributorComm.commPer.ToString());
                        decimal SupGapComm = decimal.Parse(SuperComm.commPer.ToString());
                        //decimal SupGapCommAmt = (Trans_Amt * SupGapComm) / 100;
                        //#region Super Commission
                        //var tbl_accountSuper = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == SUP_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        //if (tbl_accountSuper != null)
                        //{
                        //    var Supmembtype = (from mm in db.TBL_MASTER_MEMBER
                        //                       join
                        //                           rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                        //                       where mm.MEM_ID == SUP_MEM_ID
                        //                       select new
                        //                       {
                        //                           RoleId = mm.MEMBER_ROLE,
                        //                           roleName = rm.ROLE_NAME,
                        //                           Amount = mm.BALANCE
                        //                       }).FirstOrDefault();
                        //    decimal SupClosingAmt = tbl_accountSuper.CLOSING;
                        //    // TDS and GST CAlculation for Super for Water
                        //    decimal WA_Sup_Gap_CommAmt = 0;
                        //    decimal WA_Sup_TDS_CAL_Val = 0;
                        //    decimal SupGapCommAmt = 0;
                        //    decimal WASuper_GSTCAl = 0;
                        //    if (SuperComm.commType == "FIXED")
                        //    {
                        //        WA_Sup_Gap_CommAmt = SuperComm.commPer;
                        //        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                        //        {
                        //            WA_Sup_TDS_CAL_Val = decimal.Round(((WA_Sup_Gap_CommAmt * GST_AMount) / 100), 2);
                        //            if (MemberGST != null)
                        //            {
                        //                WASuper_GSTCAl = decimal.Round(((WA_Sup_Gap_CommAmt * gst_VAlue) / 100), 2);
                        //                SupGapCommAmt = (WA_Sup_Gap_CommAmt + WASuper_GSTCAl - WA_Sup_TDS_CAL_Val);
                        //            }
                        //            else
                        //            {
                        //                WASuper_GSTCAl = 0;
                        //                SupGapCommAmt = (WA_Sup_Gap_CommAmt + WASuper_GSTCAl - WA_Sup_TDS_CAL_Val);
                        //            }
                        //        }
                        //        else
                        //        {
                        //            WA_Sup_TDS_CAL_Val = decimal.Round(((WA_Sup_Gap_CommAmt - GST_AMount)));
                        //            if (MemberGST != null)
                        //            {
                        //                WASuper_GSTCAl = decimal.Round(((WA_Sup_Gap_CommAmt - gst_VAlue)));
                        //                SupGapCommAmt = (WA_Sup_Gap_CommAmt + WASuper_GSTCAl - WA_Sup_TDS_CAL_Val);
                        //            }
                        //            else
                        //            {
                        //                WASuper_GSTCAl = 0;
                        //                SupGapCommAmt = (WA_Sup_Gap_CommAmt + WASuper_GSTCAl - WA_Sup_TDS_CAL_Val);
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        WA_Sup_Gap_CommAmt = (Trans_Amt * SupGapComm) / 100;
                        //        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                        //        {
                        //            WA_Sup_TDS_CAL_Val = decimal.Round(((WA_Sup_Gap_CommAmt * GST_AMount) / 100), 2);
                        //            if (MemberGST != null)
                        //            {
                        //                WASuper_GSTCAl = decimal.Round(((WA_Sup_Gap_CommAmt * gst_VAlue) / 100), 2);
                        //                SupGapCommAmt = (WA_Sup_Gap_CommAmt + WASuper_GSTCAl - WA_Sup_TDS_CAL_Val);
                        //            }
                        //            else
                        //            {
                        //                WASuper_GSTCAl = 0;
                        //                SupGapCommAmt = (WA_Sup_Gap_CommAmt + WASuper_GSTCAl - WA_Sup_TDS_CAL_Val);
                        //            }

                        //        }
                        //        else
                        //        {
                        //            WA_Sup_TDS_CAL_Val = decimal.Round(((WA_Sup_Gap_CommAmt - GST_AMount)));
                        //            if (MemberGST != null)
                        //            {
                        //                WASuper_GSTCAl = decimal.Round(((WA_Sup_Gap_CommAmt - gst_VAlue)));
                        //                SupGapCommAmt = (WA_Sup_Gap_CommAmt + WASuper_GSTCAl - WA_Sup_TDS_CAL_Val);
                        //            }
                        //            else
                        //            {
                        //                WASuper_GSTCAl = 0;
                        //                SupGapCommAmt = (WA_Sup_Gap_CommAmt + WASuper_GSTCAl - WA_Sup_TDS_CAL_Val);
                        //            }
                        //        }

                        //    }

                        //    decimal CommSupAddClosing = tbl_accountSuper.CLOSING + SupGapCommAmt;
                        //    long Sup_ID = long.Parse(SUP_MEM_ID.ToString());
                        //    TBL_ACCOUNTS objSupCommPer = new TBL_ACCOUNTS()
                        //    {
                        //        API_ID = 0,
                        //        MEM_ID = Sup_ID,
                        //        MEMBER_TYPE = Supmembtype.roleName,
                        //        TRANSACTION_TYPE = rechargeType,
                        //        TRANSACTION_DATE = System.DateTime.Now,
                        //        TRANSACTION_TIME = DateTime.Now,
                        //        DR_CR = "CR",
                        //        AMOUNT = Trans_Amt,
                        //        NARRATION = "Commission",
                        //        OPENING = SupClosingAmt,
                        //        CLOSING = CommSupAddClosing,
                        //        REC_NO = 0,
                        //        COMM_AMT = SupGapCommAmt,
                        //        GST = (float)WASuper_GSTCAl,
                        //        TDS = (float)WA_Sup_TDS_CAL_Val,
                        //        IPAddress = IpAddress,
                        //        TDS_PERCENTAGE = taxMaster.TAX_VALUE,
                        //        GST_PERCENTAGE = GST_Master.TAX_VALUE,
                        //        WHITELEVEL_ID = (long)WHT_MEM_ID,
                        //        SUPER_ID = (long)SUP_MEM_ID,
                        //        DISTRIBUTOR_ID = (long)DIS_MEM_ID
                        //    };
                        //    db.TBL_ACCOUNTS.Add(objSupCommPer);
                        //    db.SaveChanges();
                        //}
                        //#endregion                        
                        var WhiteComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                                         join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
                                          on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
                                         where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == 0 && commslabMob.OPERATOR_CODE == serviceprovider
                                         select new
                                         {
                                             SLN = commslabMob.SLN,
                                             commPer = commslabMob.COMM_PERCENTAGE,
                                             commType = commslabMob.COMMISSION_TYPE
                                         }).FirstOrDefault();
                        //decimal Gap_WLlevel = decimal.Parse(MerchantComm.commPer.ToString()) + decimal.Parse(DistributorComm.commPer.ToString()) + decimal.Parse(SuperComm.commPer.ToString());
                        decimal Gap_WLlevel = decimal.Parse(MerchantComm.commPer.ToString()) + decimal.Parse(DistributorComm.commPer.ToString());
                        //decimal WTLGapComm = decimal.Parse(WhiteComm.commPer.ToString()) - decimal.Parse(SuperComm.commPer.ToString());
                        decimal WTLGapComm = decimal.Parse(WhiteComm.commPer.ToString()) - Gap_WLlevel;
                        //decimal WTLGapCommAmt = (Trans_Amt * WTLGapComm) / 100;
                        #region White level Commission payment Structure
                        var tbl_accountWhiteLevel = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == WHT_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        if (tbl_accountWhiteLevel != null)
                        {
                            var WLmembtype = (from mm in db.TBL_MASTER_MEMBER
                                              join
                                                  rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                              where mm.MEM_ID == WHT_MEM_ID
                                              select new
                                              {
                                                  RoleId = mm.MEMBER_ROLE,
                                                  roleName = rm.ROLE_NAME,
                                                  Amount = mm.BALANCE
                                              }).FirstOrDefault();
                            decimal WLClosingAmt = tbl_accountWhiteLevel.CLOSING;
                            //decimal SubAmt = ClosingAmt - Trans_Amt;                  
                            // TDS and GST Calculation for white level Water
                            decimal WTL_Gap_Comm_Amt_Val = 0;
                            decimal WL_TDSCAl_VAlue = 0;
                            decimal WTLGapCommAmt = 0;
                            decimal WA_WLGSTCalculation = 0;
                            if (WhiteComm.commType == "FIXED")
                            {
                                WTL_Gap_Comm_Amt_Val = WTLGapComm;
                                if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                {
                                    WL_TDSCAl_VAlue = decimal.Round(((WTL_Gap_Comm_Amt_Val * GST_AMount) / 100), 2);
                                    if (MemberGST != null)
                                    {
                                        WA_WLGSTCalculation = decimal.Round(((WTL_Gap_Comm_Amt_Val * gst_VAlue) / 100), 2);
                                        WTLGapCommAmt = WTL_Gap_Comm_Amt_Val + WA_WLGSTCalculation - WL_TDSCAl_VAlue;
                                    }
                                    else
                                    {
                                        WA_WLGSTCalculation = 0;
                                        WTLGapCommAmt = WTL_Gap_Comm_Amt_Val + WA_WLGSTCalculation - WL_TDSCAl_VAlue;
                                    }
                                }
                                else
                                {
                                    WL_TDSCAl_VAlue = decimal.Round(((WTL_Gap_Comm_Amt_Val - GST_AMount)));
                                    if (MemberGST != null)
                                    {
                                        WA_WLGSTCalculation = decimal.Round(((WTL_Gap_Comm_Amt_Val - gst_VAlue)));
                                        WTLGapCommAmt = WTL_Gap_Comm_Amt_Val + WA_WLGSTCalculation - WL_TDSCAl_VAlue;
                                    }
                                    else
                                    {
                                        WA_WLGSTCalculation = 0;
                                        WTLGapCommAmt = WTL_Gap_Comm_Amt_Val + WA_WLGSTCalculation - WL_TDSCAl_VAlue;
                                    }
                                }
                            }
                            else
                            {
                                WTL_Gap_Comm_Amt_Val = (Trans_Amt * WTLGapComm) / 100;
                                if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                {
                                    WL_TDSCAl_VAlue = decimal.Round(((WTL_Gap_Comm_Amt_Val * GST_AMount) / 100), 2);
                                    if (MemberGST != null)
                                    {
                                        WA_WLGSTCalculation = decimal.Round(((WTL_Gap_Comm_Amt_Val * gst_VAlue) / 100), 2);
                                        WTLGapCommAmt = WTL_Gap_Comm_Amt_Val + WA_WLGSTCalculation - WL_TDSCAl_VAlue;
                                    }
                                    else
                                    {
                                        WA_WLGSTCalculation = 0;
                                        WTLGapCommAmt = WTL_Gap_Comm_Amt_Val + WA_WLGSTCalculation - WL_TDSCAl_VAlue;
                                    }
                                }
                                else
                                {
                                    WL_TDSCAl_VAlue = decimal.Round(((WTL_Gap_Comm_Amt_Val - GST_AMount)));
                                    if (MemberGST != null)
                                    {
                                        WA_WLGSTCalculation = decimal.Round(((WTL_Gap_Comm_Amt_Val - gst_VAlue)));
                                        WTLGapCommAmt = WTL_Gap_Comm_Amt_Val + WA_WLGSTCalculation - WL_TDSCAl_VAlue;
                                    }
                                    else
                                    {
                                        WA_WLGSTCalculation = 0;
                                        WTLGapCommAmt = WTL_Gap_Comm_Amt_Val + WA_WLGSTCalculation - WL_TDSCAl_VAlue;
                                    }
                                }
                            }
                            decimal CommWLAddClosing = tbl_accountWhiteLevel.CLOSING + WTLGapCommAmt;
                            long WL_ID = long.Parse(WHT_MEM_ID.ToString());
                            TBL_ACCOUNTS objWLCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = WL_ID,
                                MEMBER_TYPE = WLmembtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = WLClosingAmt,
                                CLOSING = CommWLAddClosing,
                                REC_NO = 0,
                                COMM_AMT = WTLGapCommAmt,
                                GST = (float)WA_WLGSTCalculation,
                                TDS = (float)WL_TDSCAl_VAlue,
                                IPAddress = IpAddress,
                                TDS_PERCENTAGE = taxMaster.TAX_VALUE,
                                GST_PERCENTAGE = GST_Master.TAX_VALUE,
                                WHITELEVEL_ID = (long)WHT_MEM_ID,
                                //SUPER_ID = (long)SUP_MEM_ID,
                                SUPER_ID = 0,
                                DISTRIBUTOR_ID = (long)DIS_MEM_ID
                            };
                            db.TBL_ACCOUNTS.Add(objWLCommPer);
                            db.SaveChanges();
                        }
                        #endregion
                        ContextTransaction.Commit();
                        return true;
                    }
                    else if (status == "ELECTRICITY")
                    {
                        //var MerchantComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                    join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
                        //                    on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
                        //                    where detailscom.WHITE_LEVEL_ID == DIS_MEM_ID && detailscom.INTRODUCER_ID == Mem_ID && commslabMob.OPERATOR_CODE == serviceprovider
                        //                    select new
                        //                    {
                        //                        SLN = commslabMob.SLN,
                        //                        commPer = commslabMob.COMM_PERCENTAGE
                        //                    }).FirstOrDefault();
                        var MerchantComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                            join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                            on detailscom.SLN equals commslabMob.SLAB_ID
                                            join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                                            where commslabMob.OPERATOR_CODE == serviceprovider && mem.MEMBER_ROLE == 5
                                            select new
                                            {
                                                SLN = commslabMob.SLN,
                                                commPer = commslabMob.MERCHANT_COM_PER,
                                                commType = commslabMob.COMMISSION_TYPE
                                            }).FirstOrDefault();
                        #region Retailer Commission                        
                        var membtype = (from mm in db.TBL_MASTER_MEMBER
                                        join
                                            rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                        where mm.MEM_ID == Mem_ID
                                        select new
                                        {
                                            RoleId = mm.MEMBER_ROLE,
                                            roleName = rm.ROLE_NAME,
                                            Amount = mm.BALANCE
                                        }).FirstOrDefault();
                        var tbl_account = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == Mem_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        if (tbl_account != null)
                        {
                            decimal ClosingAmt = tbl_account.CLOSING;
                            decimal SubAmt = ClosingAmt - Trans_Amt;
                            TBL_ACCOUNTS objaccnt = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Mem_ID,
                                MEMBER_TYPE = membtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "DR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Recharge",
                                OPENING = ClosingAmt,
                                CLOSING = SubAmt,
                                REC_NO = 0,
                                COMM_AMT = 0,
                                GST = 0,
                                TDS = 0,
                                IPAddress = IpAddress,
                                TDS_PERCENTAGE = 0,
                                GST_PERCENTAGE = 0,
                                WHITELEVEL_ID = (long)WHT_MEM_ID,
                                //SUPER_ID = (long)SUP_MEM_ID,
                                SUPER_ID = 0,
                                DISTRIBUTOR_ID = (long)DIS_MEM_ID
                            };
                            db.TBL_ACCOUNTS.Add(objaccnt);
                            db.SaveChanges();
                            // TDS and GST CAlculation for Merchant for electricity
                            //decimal getPer = (Trans_Amt * MerchantComm.commPer) / 100;
                            decimal ELE_getPer_Val = 0;
                            decimal ELE_TDS_Value = 0;
                            decimal getPer = 0;
                            decimal ELE_MER_GST_Cal = 0;
                            if (MerchantComm.commType == "FIXED")
                            {
                                ELE_getPer_Val = MerchantComm.commPer;
                                if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                {
                                    ELE_TDS_Value = decimal.Round(((ELE_getPer_Val * GST_AMount) / 100), 2);
                                    if (MemberGST != null)
                                    {
                                        ELE_MER_GST_Cal = decimal.Round(((ELE_getPer_Val * gst_VAlue) / 100), 2);
                                        getPer = (ELE_getPer_Val + ELE_MER_GST_Cal - ELE_TDS_Value);
                                    }
                                    else
                                    {
                                        ELE_MER_GST_Cal = 0;
                                        getPer = (ELE_getPer_Val + ELE_MER_GST_Cal - ELE_TDS_Value);
                                    }
                                }
                                else
                                {
                                    ELE_TDS_Value = decimal.Round(((ELE_getPer_Val - GST_AMount)));
                                    if (MemberGST != null)
                                    {
                                        ELE_MER_GST_Cal = decimal.Round(((ELE_getPer_Val - gst_VAlue)));
                                        getPer = (ELE_getPer_Val + ELE_MER_GST_Cal - ELE_TDS_Value);
                                    }
                                    else
                                    {
                                        ELE_MER_GST_Cal = 0;
                                        getPer = (ELE_getPer_Val + ELE_MER_GST_Cal - ELE_TDS_Value);
                                    }
                                }
                            }
                            else
                            {
                                ELE_getPer_Val = (Trans_Amt * MerchantComm.commPer) / 100;
                                if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                {

                                    ELE_TDS_Value = decimal.Round(((ELE_getPer_Val * GST_AMount) / 100), 2);
                                    if (MemberGST != null)
                                    {
                                        ELE_MER_GST_Cal = decimal.Round(((ELE_getPer_Val * gst_VAlue) / 100), 2);
                                        getPer = (ELE_getPer_Val + ELE_MER_GST_Cal - ELE_TDS_Value);
                                    }
                                    else
                                    {
                                        ELE_MER_GST_Cal = 0;
                                        getPer = (ELE_getPer_Val + ELE_MER_GST_Cal - ELE_TDS_Value);
                                    }
                                }
                                else
                                {
                                    ELE_TDS_Value = decimal.Round(((ELE_getPer_Val * GST_AMount)));
                                    if (MemberGST != null)
                                    {
                                        ELE_MER_GST_Cal = decimal.Round(((ELE_getPer_Val * gst_VAlue)));
                                        getPer = (ELE_getPer_Val + ELE_MER_GST_Cal - ELE_TDS_Value);
                                    }
                                    else
                                    {
                                        ELE_MER_GST_Cal = 0;
                                        getPer = (ELE_getPer_Val + ELE_MER_GST_Cal - ELE_TDS_Value);
                                    }
                                }
                            }

                            decimal CommAddClosing = SubAmt + getPer;
                            TBL_ACCOUNTS objCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Mem_ID,
                                MEMBER_TYPE = membtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = SubAmt,
                                CLOSING = CommAddClosing,
                                REC_NO = 0,
                                COMM_AMT = getPer,
                                GST = (float)ELE_MER_GST_Cal,
                                TDS = (float)ELE_TDS_Value,
                                IPAddress = IpAddress,
                                TDS_PERCENTAGE = taxMaster.TAX_VALUE,
                                GST_PERCENTAGE = GST_Master.TAX_VALUE,
                                WHITELEVEL_ID = (long)WHT_MEM_ID,
                                //SUPER_ID = (long)SUP_MEM_ID,
                                SUPER_ID = 0,
                                DISTRIBUTOR_ID = (long)DIS_MEM_ID
                            };
                            db.TBL_ACCOUNTS.Add(objCommPer);
                            db.SaveChanges();
                        }
                        #endregion

                        //var DistributorComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                       join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
                        //                       on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
                        //                       where detailscom.WHITE_LEVEL_ID == SUP_MEM_ID && detailscom.INTRODUCER_ID == DIS_MEM_ID && commslabMob.OPERATOR_CODE == serviceprovider
                        //                       select new
                        //                       {
                        //                           SLN = commslabMob.SLN,
                        //                           commPer = commslabMob.COMM_PERCENTAGE
                        //                       }).FirstOrDefault();
                        var DistributorComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                               join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                               on detailscom.SLN equals commslabMob.SLAB_ID
                                               join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                                               where commslabMob.OPERATOR_CODE == serviceprovider && mem.MEMBER_ROLE == 4
                                               select new
                                               {
                                                   SLN = commslabMob.SLN,
                                                   commPer = commslabMob.DISTRIBUTOR_COM_PER,
                                                   commType = commslabMob.COMMISSION_TYPE
                                               }).FirstOrDefault();
                        #region Distributor Commission
                        //decimal DisGapComm = decimal.Parse(DistributorComm.commPer.ToString()) - decimal.Parse(MerchantComm.commPer.ToString());
                        decimal DisGapComm = decimal.Parse(DistributorComm.commPer.ToString());
                        //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;
                        var tbl_accountDistributor = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == DIS_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        if (tbl_accountDistributor != null)
                        {
                            var CheckDisCommission = db.TBL_DETAILS_MEMBER_COMMISSION_SLAB.Where(x => x.INTRODUCER_ID == DIS_MEM_ID).FirstOrDefault();
                            var Dismembtype = (from mm in db.TBL_MASTER_MEMBER
                                               join
                                                   rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                               where mm.MEM_ID == DIS_MEM_ID
                                               select new
                                               {
                                                   RoleId = mm.MEMBER_ROLE,
                                                   roleName = rm.ROLE_NAME,
                                                   Amount = mm.BALANCE
                                               }).FirstOrDefault();
                            decimal DisClosingAmt = tbl_accountDistributor.CLOSING;
                            //decimal SubAmt = ClosingAmt - Trans_Amt;                   
                            //decimal DisgetPer = (Trans_Amt * checkDiscomm.CommPercentage) / 100;

                            // For TDS and GST CAlculation for Electricity
                            //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;
                            decimal ELE_DisGap_CommAmt = 0;
                            decimal ELE_TDSCALCULATION = 0;
                            decimal DisGapCommAmt = 0;
                            decimal ELE_DIST_GSTCalculation = 0;
                            if (DistributorComm.commType == "FIXED")
                            {
                                ELE_DisGap_CommAmt = DisGapComm;
                                if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                {
                                    ELE_TDSCALCULATION = decimal.Round(((ELE_DisGap_CommAmt * GST_AMount) / 100), 2);
                                    if (MemberGST != null)
                                    {
                                        ELE_DIST_GSTCalculation = decimal.Round(((ELE_DisGap_CommAmt * gst_VAlue) / 100), 2);
                                        DisGapCommAmt = ELE_DisGap_CommAmt + ELE_DIST_GSTCalculation - ELE_TDSCALCULATION;
                                    }
                                    else
                                    {
                                        ELE_DIST_GSTCalculation = 0;
                                        DisGapCommAmt = ELE_DisGap_CommAmt + ELE_DIST_GSTCalculation - ELE_TDSCALCULATION;
                                    }
                                }
                                else
                                {
                                    ELE_TDSCALCULATION = decimal.Round(((ELE_DisGap_CommAmt - GST_AMount)));
                                    if (MemberGST != null)
                                    {
                                        ELE_DIST_GSTCalculation = decimal.Round(((ELE_DisGap_CommAmt - gst_VAlue)));
                                        DisGapCommAmt = ELE_DisGap_CommAmt + ELE_DIST_GSTCalculation - ELE_TDSCALCULATION;
                                    }
                                    else
                                    {
                                        ELE_DIST_GSTCalculation = 0;
                                        DisGapCommAmt = ELE_DisGap_CommAmt + ELE_DIST_GSTCalculation - ELE_TDSCALCULATION;
                                    }
                                }
                            }
                            else
                            {
                                ELE_DisGap_CommAmt = (Trans_Amt * DisGapComm) / 100;
                                if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                {
                                    ELE_TDSCALCULATION = decimal.Round(((ELE_DisGap_CommAmt * GST_AMount) / 100), 2);
                                    if (MemberGST != null)
                                    {
                                        ELE_DIST_GSTCalculation = decimal.Round(((ELE_DisGap_CommAmt * gst_VAlue) / 100), 2);
                                        DisGapCommAmt = ELE_DisGap_CommAmt + ELE_DIST_GSTCalculation - ELE_TDSCALCULATION;
                                    }
                                    else
                                    {
                                        ELE_DIST_GSTCalculation = 0;
                                        DisGapCommAmt = ELE_DisGap_CommAmt + ELE_DIST_GSTCalculation - ELE_TDSCALCULATION;
                                    }
                                }
                                else
                                {
                                    ELE_TDSCALCULATION = decimal.Round(((ELE_DisGap_CommAmt - GST_AMount)));
                                    if (MemberGST != null)
                                    {
                                        ELE_DIST_GSTCalculation = decimal.Round(((ELE_DisGap_CommAmt - gst_VAlue)));
                                        DisGapCommAmt = ELE_DisGap_CommAmt + ELE_DIST_GSTCalculation - ELE_TDSCALCULATION;
                                    }
                                    else
                                    {
                                        ELE_DIST_GSTCalculation = 0;
                                        DisGapCommAmt = ELE_DisGap_CommAmt + ELE_DIST_GSTCalculation - ELE_TDSCALCULATION;
                                    }
                                }
                            }
                            decimal CommDisAddClosing = tbl_accountDistributor.CLOSING + DisGapCommAmt;
                            long Dis_ID = long.Parse(DIS_MEM_ID.ToString());
                            TBL_ACCOUNTS objDisCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Dis_ID,
                                MEMBER_TYPE = Dismembtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = DisClosingAmt,
                                CLOSING = CommDisAddClosing,
                                REC_NO = 0,
                                COMM_AMT = DisGapCommAmt,
                                GST = (float)ELE_DIST_GSTCalculation,
                                TDS = (float)ELE_TDSCALCULATION,
                                IPAddress = IpAddress,
                                TDS_PERCENTAGE = taxMaster.TAX_VALUE,
                                GST_PERCENTAGE = GST_Master.TAX_VALUE,
                                WHITELEVEL_ID = (long)WHT_MEM_ID,
                                //SUPER_ID = (long)SUP_MEM_ID,
                                SUPER_ID = 0,
                                DISTRIBUTOR_ID = (long)DIS_MEM_ID
                            };
                            db.TBL_ACCOUNTS.Add(objDisCommPer);
                            db.SaveChanges();
                        }
                        #endregion
                        //var SuperComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                 join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
                        //                  on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
                        //                 where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == SUP_MEM_ID && commslabMob.OPERATOR_CODE == serviceprovider
                        //                 select new
                        //                 {
                        //                     SLN = commslabMob.SLN,
                        //                     commPer = commslabMob.COMM_PERCENTAGE
                        //                 }).FirstOrDefault();
                        var SuperComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                         join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                          on detailscom.SLN equals commslabMob.SLAB_ID
                                         join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                                         where commslabMob.OPERATOR_CODE == serviceprovider && mem.MEMBER_ROLE == 3
                                         select new
                                         {
                                             SLN = commslabMob.SLN,
                                             commPer = commslabMob.SUPER_COM_PER,
                                             commType = commslabMob.COMMISSION_TYPE
                                         }).FirstOrDefault();
                        //decimal SupGapComm = decimal.Parse(SuperComm.commPer.ToString()) - decimal.Parse(DistributorComm.commPer.ToString());
                        decimal SupGapComm = decimal.Parse(SuperComm.commPer.ToString());
                        //decimal SupGapCommAmt = (Trans_Amt * SupGapComm) / 100;
                        //#region Super Commission
                        //var tbl_accountSuper = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == SUP_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        //if (tbl_accountSuper != null)
                        //{
                        //    var Supmembtype = (from mm in db.TBL_MASTER_MEMBER
                        //                       join
                        //                           rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                        //                       where mm.MEM_ID == SUP_MEM_ID
                        //                       select new
                        //                       {
                        //                           RoleId = mm.MEMBER_ROLE,
                        //                           roleName = rm.ROLE_NAME,
                        //                           Amount = mm.BALANCE
                        //                       }).FirstOrDefault();
                        //    decimal SupClosingAmt = tbl_accountSuper.CLOSING;
                        //    // TDS AND GST CALCULATION FOR SUPER FOR ELECTRICITY
                        //    decimal ELE_Sup_Gap_Comm_Amt = 0;
                        //    decimal ELE_SUP_TDS_CAL = 0;
                        //    decimal SupGapCommAmt = 0;
                        //    decimal ELE_SUperGSTCalcu = 0;
                        //    if (SuperComm.commType == "FIXED")
                        //    {
                        //        ELE_Sup_Gap_Comm_Amt = SupGapComm;
                        //        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                        //        {
                        //            ELE_SUP_TDS_CAL = decimal.Round(((ELE_Sup_Gap_Comm_Amt * GST_AMount) / 100), 2);
                        //            if (MemberGST != null)
                        //            {
                        //                ELE_SUperGSTCalcu = decimal.Round(((ELE_Sup_Gap_Comm_Amt * gst_VAlue) / 100), 2);
                        //                SupGapCommAmt = ELE_Sup_Gap_Comm_Amt + ELE_SUperGSTCalcu - ELE_SUP_TDS_CAL;
                        //            }
                        //            else
                        //            {
                        //                ELE_SUperGSTCalcu = 0;
                        //                SupGapCommAmt = ELE_Sup_Gap_Comm_Amt + ELE_SUperGSTCalcu - ELE_SUP_TDS_CAL;
                        //            }
                        //        }
                        //        else
                        //        {
                        //            ELE_SUP_TDS_CAL = decimal.Round(((ELE_Sup_Gap_Comm_Amt * GST_AMount) / 100), 2);
                        //            if (MemberGST != null)
                        //            {
                        //                ELE_SUperGSTCalcu = decimal.Round(((ELE_Sup_Gap_Comm_Amt * gst_VAlue) / 100), 2);
                        //                SupGapCommAmt = ELE_Sup_Gap_Comm_Amt + ELE_SUperGSTCalcu - ELE_SUP_TDS_CAL;
                        //            }
                        //            else
                        //            {
                        //                ELE_SUperGSTCalcu = 0;
                        //                SupGapCommAmt = ELE_Sup_Gap_Comm_Amt + ELE_SUperGSTCalcu - ELE_SUP_TDS_CAL;
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        ELE_Sup_Gap_Comm_Amt = (Trans_Amt * SupGapComm) / 100;
                        //        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                        //        {

                        //            ELE_SUP_TDS_CAL = decimal.Round(((ELE_Sup_Gap_Comm_Amt * GST_AMount) / 100), 2);
                        //            if (MemberGST != null)
                        //            {
                        //                ELE_SUperGSTCalcu = decimal.Round(((ELE_Sup_Gap_Comm_Amt * gst_VAlue) / 100), 2);
                        //                SupGapCommAmt = ELE_Sup_Gap_Comm_Amt + ELE_SUperGSTCalcu - ELE_SUP_TDS_CAL;
                        //            }
                        //            else
                        //            {
                        //                ELE_SUperGSTCalcu = 0;
                        //                SupGapCommAmt = ELE_Sup_Gap_Comm_Amt + ELE_SUperGSTCalcu - ELE_SUP_TDS_CAL;
                        //            }

                        //        }
                        //        else
                        //        {

                        //            ELE_SUP_TDS_CAL = decimal.Round(((ELE_Sup_Gap_Comm_Amt * GST_AMount) / 100), 2);
                        //            if (MemberGST != null)
                        //            {
                        //                ELE_SUperGSTCalcu = decimal.Round(((ELE_Sup_Gap_Comm_Amt * gst_VAlue) / 100), 2);
                        //                SupGapCommAmt = ELE_Sup_Gap_Comm_Amt + ELE_SUperGSTCalcu - ELE_SUP_TDS_CAL;
                        //            }
                        //            else
                        //            {
                        //                ELE_SUperGSTCalcu = 0;
                        //                SupGapCommAmt = ELE_Sup_Gap_Comm_Amt + ELE_SUperGSTCalcu - ELE_SUP_TDS_CAL;
                        //            }

                        //        }
                        //    }

                        //    decimal CommSupAddClosing = tbl_accountSuper.CLOSING + SupGapCommAmt;
                        //    long Sup_ID = long.Parse(SUP_MEM_ID.ToString());
                        //    TBL_ACCOUNTS objSupCommPer = new TBL_ACCOUNTS()
                        //    {
                        //        API_ID = 0,
                        //        MEM_ID = Sup_ID,
                        //        MEMBER_TYPE = Supmembtype.roleName,
                        //        TRANSACTION_TYPE = rechargeType,
                        //        TRANSACTION_DATE = System.DateTime.Now,
                        //        TRANSACTION_TIME = DateTime.Now,
                        //        DR_CR = "CR",
                        //        AMOUNT = Trans_Amt,
                        //        NARRATION = "Commission",
                        //        OPENING = SupClosingAmt,
                        //        CLOSING = CommSupAddClosing,
                        //        REC_NO = 0,
                        //        COMM_AMT = SupGapCommAmt,
                        //        GST = (float)ELE_SUperGSTCalcu,
                        //        TDS = (float)ELE_SUP_TDS_CAL,
                        //        IPAddress = IpAddress,
                        //        TDS_PERCENTAGE = taxMaster.TAX_VALUE,
                        //        GST_PERCENTAGE = GST_Master.TAX_VALUE,
                        //        WHITELEVEL_ID = (long)WHT_MEM_ID,
                        //        SUPER_ID = (long)SUP_MEM_ID,
                        //        DISTRIBUTOR_ID = (long)DIS_MEM_ID
                        //    };
                        //    db.TBL_ACCOUNTS.Add(objSupCommPer);
                        //    db.SaveChanges();
                        //}
                        //#endregion                        
                        var WhiteComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                                         join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
                                          on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
                                         where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == 0 && commslabMob.OPERATOR_CODE == serviceprovider
                                         select new
                                         {
                                             SLN = commslabMob.SLN,
                                             commPer = commslabMob.COMM_PERCENTAGE,
                                             commType = commslabMob.COMMISSION_TYPE
                                         }).FirstOrDefault();
                        //decimal WL_Com = decimal.Parse(MerchantComm.commPer.ToString()) + decimal.Parse(DistributorComm.commPer.ToString()) + decimal.Parse(SuperComm.commPer.ToString());
                        decimal WL_Com = decimal.Parse(MerchantComm.commPer.ToString()) + decimal.Parse(DistributorComm.commPer.ToString()) ;
                        //decimal WTLGapComm = decimal.Parse(WhiteComm.commPer.ToString()) - decimal.Parse(SuperComm.commPer.ToString());
                        decimal WTLGapComm = decimal.Parse(WhiteComm.commPer.ToString()) - WL_Com;
                        //decimal WTLGapCommAmt = (Trans_Amt * WTLGapComm) / 100;
                        #region White level Commission payment Structure
                        var tbl_accountWhiteLevel = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == WHT_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        if (tbl_accountWhiteLevel != null)
                        {
                            var WLmembtype = (from mm in db.TBL_MASTER_MEMBER
                                              join
                                                  rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                              where mm.MEM_ID == WHT_MEM_ID
                                              select new
                                              {
                                                  RoleId = mm.MEMBER_ROLE,
                                                  roleName = rm.ROLE_NAME,
                                                  Amount = mm.BALANCE
                                              }).FirstOrDefault();
                            decimal WLClosingAmt = tbl_accountWhiteLevel.CLOSING;
                            //decimal SubAmt = ClosingAmt - Trans_Amt;           
                            // GST And TDS Calculation for WL 
                            //decimal WTLGapCommAmt = (Trans_Amt * WTLGapComm) / 100;
                            decimal ELE_WTL_Gap_CommAmt = 0;
                            decimal ELE_WL_TDSCal = 0;
                            decimal WTLGapCommAmt = 0;
                            decimal ELE_WL_GSTCAlculation = 0;
                            if (WhiteComm.commType == "FIXED")
                            {
                                ELE_WTL_Gap_CommAmt = WTLGapComm;
                                if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                {
                                    ELE_WL_TDSCal = decimal.Round(((ELE_WTL_Gap_CommAmt * GST_AMount) / 100), 2);
                                    if (MemberGST != null)
                                    {
                                        ELE_WL_GSTCAlculation = decimal.Round(((ELE_WTL_Gap_CommAmt * gst_VAlue) / 100), 2);
                                        WTLGapCommAmt = ELE_WTL_Gap_CommAmt + ELE_WL_GSTCAlculation - ELE_WL_TDSCal;
                                    }
                                    else
                                    {
                                        ELE_WL_GSTCAlculation = 0;
                                        WTLGapCommAmt = ELE_WTL_Gap_CommAmt + ELE_WL_GSTCAlculation - ELE_WL_TDSCal;
                                    }

                                }
                                else
                                {
                                    ELE_WL_TDSCal = decimal.Round(((ELE_WTL_Gap_CommAmt * GST_AMount)));
                                    if (MemberGST != null)
                                    {
                                        ELE_WL_GSTCAlculation = decimal.Round(((ELE_WTL_Gap_CommAmt * gst_VAlue)));
                                        WTLGapCommAmt = ELE_WTL_Gap_CommAmt + ELE_WL_GSTCAlculation - ELE_WL_TDSCal;
                                    }
                                    else
                                    {
                                        ELE_WL_GSTCAlculation = 0;
                                        WTLGapCommAmt = ELE_WTL_Gap_CommAmt + ELE_WL_GSTCAlculation - ELE_WL_TDSCal;
                                    }

                                }
                            }
                            else
                            {
                                ELE_WTL_Gap_CommAmt = (Trans_Amt * WTLGapComm) / 100;
                                if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                {
                                    ELE_WL_TDSCal = decimal.Round(((ELE_WTL_Gap_CommAmt * GST_AMount) / 100), 2);
                                    if (MemberGST != null)
                                    {
                                        ELE_WL_GSTCAlculation = decimal.Round(((ELE_WTL_Gap_CommAmt * gst_VAlue) / 100), 2);
                                        WTLGapCommAmt = ELE_WTL_Gap_CommAmt + ELE_WL_GSTCAlculation - ELE_WL_TDSCal;
                                    }
                                    else
                                    {
                                        ELE_WL_GSTCAlculation = 0;
                                        WTLGapCommAmt = ELE_WTL_Gap_CommAmt + ELE_WL_GSTCAlculation - ELE_WL_TDSCal;
                                    }
                                }
                                else
                                {
                                    ELE_WL_TDSCal = decimal.Round(((ELE_WTL_Gap_CommAmt * GST_AMount)));
                                    if (MemberGST != null)
                                    {
                                        ELE_WL_GSTCAlculation = decimal.Round(((ELE_WTL_Gap_CommAmt * gst_VAlue)));
                                        WTLGapCommAmt = ELE_WTL_Gap_CommAmt + ELE_WL_GSTCAlculation - ELE_WL_TDSCal;
                                    }
                                    else
                                    {
                                        ELE_WL_GSTCAlculation = 0;
                                        WTLGapCommAmt = ELE_WTL_Gap_CommAmt + ELE_WL_GSTCAlculation - ELE_WL_TDSCal;
                                    }

                                }
                            }
                            decimal CommWLAddClosing = tbl_accountWhiteLevel.CLOSING + WTLGapCommAmt;
                            long WL_ID = long.Parse(WHT_MEM_ID.ToString());
                            TBL_ACCOUNTS objWLCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = WL_ID,
                                MEMBER_TYPE = WLmembtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = WLClosingAmt,
                                CLOSING = CommWLAddClosing,
                                REC_NO = 0,
                                COMM_AMT = WTLGapCommAmt,
                                GST = (float)ELE_WL_GSTCAlculation,
                                TDS = (float)ELE_WL_TDSCal,
                                IPAddress = IpAddress,
                                TDS_PERCENTAGE = taxMaster.TAX_VALUE,
                                GST_PERCENTAGE = GST_Master.TAX_VALUE,
                                WHITELEVEL_ID = (long)WHT_MEM_ID,
                                //SUPER_ID = (long)SUP_MEM_ID,
                                SUPER_ID =0,
                                DISTRIBUTOR_ID = (long)DIS_MEM_ID
                            };
                            db.TBL_ACCOUNTS.Add(objWLCommPer);
                            db.SaveChanges();
                        }
                        #endregion
                        ContextTransaction.Commit();
                        return true;
                    }
                    else if (status == "DMR")
                    {
                        var DMRVendorInfo = db.TBL_VENDOR_MASTER.FirstOrDefault(x => x.ID == 2);
                        decimal MerGrossComm = 0;
                        decimal DISTGrossComm = 0;
                        decimal SUPERGrossComm = 0;
                        decimal Super_Com_pr = 0;
                        decimal MER_RetailerTDS_AmtValue = 0;
                        decimal MERGROSSComm = 0;
                        decimal MERNETComm = 0;

                        decimal DIST_RetailerTDS_AmtValue = 0;
                        decimal DISTGROSSComm = 0;
                        decimal DISTNETComm = 0;
                        decimal SPR_RetailerTDS_AmtValue = 0;
                        decimal SPRGROSSComm = 0;
                        decimal SPRTNETComm = 0;


                        decimal Distributor_Com_pr = 0;
                        decimal Merchant_Com_pr = 0;
                        #region DMR Comm Calculation        
                        decimal MERCHANT_TotalComm_AMT = 0;                        
                        decimal AddCOMM_TotalVal = 0;
                        string DMRGSTPerValue = System.Configuration.ConfigurationManager.AppSettings["DMRGSTPERCENTAGEComm"];
                        decimal SerFee = 0;
                        decimal.TryParse(DMRGSTPerValue,out SerFee);
                        MERCHANT_TotalComm_AMT = ((Trans_Amt * SerFee) / 100);
                        AddCOMM_TotalVal = Trans_Amt + MERCHANT_TotalComm_AMT;
                        // GEt WHITE Label 18 % GST AMOUNt
                        #endregion
                        long Oper_ID = 0;
                        if (operatorID != null)
                        {
                            long.TryParse(operatorID.ID.ToString(), out Oper_ID);
                        }
                        else
                        {
                            Oper_ID = 0;
                        }

                        #region WhiteLabel Service Charge Add
                        //var WLCOMM =await (from WLCOMTag in db.TBL_COMM_SLAB_DMR_PAYMENT
                        //              join WLCOMMVAL in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //              on WLCOMTag.SLAB_ID equals WLCOMMVAL.DMR_SLAB
                        //              where WLCOMMVAL.WHITE_LEVEL_ID == WHT_MEM_ID
                        //              //where WLCOMMVAL.WHITE_LEVEL_ID == WHT_MEM_ID && WLCOMTag.OPERATOR_CODE == serviceprovider
                        //              select new
                        //              {
                        //                  COMMTYPE = WLCOMTag.COMM_TYPE,
                        //                  COMM_VALUE = WLCOMTag.COMM_PERCENTAGE
                        //              }).FirstOrDefaultAsync();

                        var WLCOMM = await (from WLCOMTag in db.TBL_COMM_SLAB_DMR_PAYMENT
                                            join WLCOMMVAL in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                                            on WLCOMTag.SLAB_ID equals WLCOMMVAL.DMR_SLAB join WLmain in db.TBL_WHITE_LEVEL_COMMISSION_SLAB on WLCOMMVAL.DMR_SLAB equals WLmain.SLN
                                            where WLCOMMVAL.WHITE_LEVEL_ID == WHT_MEM_ID && WLCOMMVAL.INTRODUCER_ID==0
                                            //where WLCOMMVAL.WHITE_LEVEL_ID == WHT_MEM_ID && WLCOMTag.OPERATOR_CODE == serviceprovider
                                            select new
                                            {
                                                COMMTYPE = WLCOMTag.COMM_TYPE,
                                                COMM_VALUE = WLCOMTag.COMM_PERCENTAGE,
                                                SLABTDS= WLmain.SLAB_TDS
                                            }).FirstOrDefaultAsync();

                        decimal WLserviceAmtAdd = 0;
                        decimal AddWL_ServiceAmtAdd = 0;
                        decimal WLService_AMT = 0;
                        decimal ServiceAddAmt = 0;
                        decimal WLServ_DeductAmt = 0;
                        decimal DeductWLTaxonSErvAmt = 0;
                        decimal WLTaxonSErvAmt = 0;
                        decimal WLGSTValue = 0;
                        decimal WLAMTTransfer = 0;
                        var WLTransactionChargeAmt =await db.TBL_DMR_TRANSACTION_LOGS.FirstOrDefaultAsync(x => x.CORELATIONID == sRandomOTP);
                        if (WLTransactionChargeAmt != null)
                        {
                            WLServ_DeductAmt = WLTransactionChargeAmt.WLP_NET_COMM;
                        }
                        else
                        {
                            WLServ_DeductAmt = WLTransactionChargeAmt.WLP_NET_COMM;
                        }
                        WLAMTTransfer = WLTransactionChargeAmt.TRANSFER_AMT + WLTransactionChargeAmt.MER_TRANSFER_FEE;

                        decimal TOtalAmountTransfer = 0;

                        //string DMRGSTPerValue = System.Configuration.ConfigurationManager.AppSettings["DMRGSTPERCENTAGEComm"];
                        string DMRGSTFixedValue = System.Configuration.ConfigurationManager.AppSettings["DMRGSTFixedComm"];
                        string FixedService_Type = "";
                        double MERGSTCAl = 0;
                        double MER_GST_Val = 0;
                        double ValGSTVal = 0;
                        decimal servGSt_amt = 0;
                        if (Trans_Amt < 1000)
                        {

                            decimal.TryParse(DMRGSTFixedValue, out servGSt_amt);
                            FixedSErviceAmt = servGSt_amt;
                            TOtalAmountTransfer = Trans_Amt + FixedSErviceAmt;
                            FixedService_Type = "FIXED";
                        }
                        else
                        {
                            decimal.TryParse(DMRGSTPerValue, out servGSt_amt);
                            FixedSErviceAmt = (Trans_Amt * servGSt_amt) / 100;
                            TOtalAmountTransfer = Trans_Amt + FixedSErviceAmt;
                            FixedService_Type = "PERCENTAGE";
                        }
                        // calculate WL commission
                        double valuecalGST = 0;
                        decimal ServiceGSTval = 0;
                        decimal SERVICEWLAMT = 0;
                        decimal WLCOMM_TDSVal = 0;
                        decimal WL_NET_COmm = 0;
                        decimal TAX_Amt = 0;
                        decimal WLGROSS_Value = 0;
                        decimal WLP_GST = 0;
                        decimal WL_MER_GSTvalue = 0;
                        double GetWLPCommissionVal = 0;

                        decimal.TryParse(taxMaster.TAX_VALUE.ToString(), out TAX_Amt);
                        double.TryParse(FixedSErviceAmt.ToString(), out valuecalGST);  // ex.  50rs on 5000
                        ValGSTVal = 1.18;
                        MERGSTCAl = (valuecalGST / ValGSTVal);
                        if (WLCOMM.COMMTYPE == "PERCENTAGE")
                        {
                            double WLPPComm = 0;
                            double.TryParse(WLCOMM.COMM_VALUE.ToString(), out WLPPComm);
                            GetWLPCommissionVal = (MERGSTCAl / (WLPPComm));
                            WLGROSS_Value = Convert.ToDecimal(MERGSTCAl) - Convert.ToDecimal(GetWLPCommissionVal);
                        }
                        else
                        {
                            double WLPPComm = 0;
                            double.TryParse(WLCOMM.COMM_VALUE.ToString(), out WLPPComm);
                            GetWLPCommissionVal = (MERGSTCAl - (WLPPComm));
                            WLGROSS_Value = Convert.ToDecimal(MERGSTCAl) - Convert.ToDecimal(GetWLPCommissionVal);
                        }
                        //WLGROSS_Value = Convert.ToDecimal(MERGSTCAl) - WLCOMM.COMM_VALUE;
                        MER_GST_Val = valuecalGST - MERGSTCAl;
                        WLP_GST = FixedSErviceAmt - Convert.ToDecimal(MERGSTCAl);
                        decimal.TryParse(MER_GST_Val.ToString(), out ServiceGSTval);
                        decimal.TryParse(MERGSTCAl.ToString(), out SERVICEWLAMT);

                        if (WLCOMM.SLABTDS == "Yes")
                        {
                            WLCOMM_TDSVal = ((WLGROSS_Value * TAX_Amt) / 100);
                            WL_NET_COmm = WLGROSS_Value - WLCOMM_TDSVal;
                        }
                        else
                        {
                            WLCOMM_TDSVal = 0;
                            WL_NET_COmm = WLGROSS_Value - WLCOMM_TDSVal;
                        }
                        //WLCOMM_TDSVal = ((WLGROSS_Value * TAX_Amt) / 100);
                        //WL_NET_COmm = WLGROSS_Value - WLCOMM_TDSVal;

                     

                        WL_MER_GSTvalue = decimal.Round(ServiceGSTval, 2);
                        // end of wl commission
                        decimal WLColsingAmtonServi = 0;
                        //var WL_Wallet =await db.TBL_ACCOUNTS.FirstOrDefaultAsync(x => x.MEM_ID == WHT_MEM_ID);
                        var WL_Wallet = await db.TBL_ACCOUNTS.Where(x => x.MEM_ID == WHT_MEM_ID).OrderByDescending(z => z.ACC_NO).FirstOrDefaultAsync(); ;
                        ////var MER_ACCOUNT = await db.TBL_ACCOUNTS.Where(x => x.MEM_ID == transinfo.FROM_MEMBER).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefaultAsync();
                        if (WL_Wallet != null)
                        {
                            WLserviceAmtAdd = WL_Wallet.CLOSING;
                            WLService_AMT = WLCOMM.COMM_VALUE;
                            WLColsingAmtonServi = WLserviceAmtAdd + WL_NET_COmm;

                            TBL_ACCOUNTS WLobjval = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = (long)WHT_MEM_ID,
                                MEMBER_TYPE = "WHITE LEVEL",
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = WL_NET_COmm,
                                NARRATION = "Cash Back To WLP",
                                OPENING = WL_Wallet.CLOSING,
                                CLOSING = WLColsingAmtonServi,
                                REC_NO = 0,
                                COMM_AMT = WL_NET_COmm,
                                GST = (double)gst_VAlue,
                                //TDS = (double)GST_AMount,
                                TDS = (double)WLCOMM_TDSVal,
                                IPAddress = IpAddress,
                                //TDS_PERCENTAGE = WLCOMM_TDSVal,
                                TDS_PERCENTAGE = GST_AMount,
                                GST_PERCENTAGE = WLP_GST,
                                WHITELEVEL_ID = (long)WHT_MEM_ID,
                                //SUPER_ID = (long)SUP_MEM_ID,
                                SUPER_ID = 0,
                                DISTRIBUTOR_ID = (long)DIS_MEM_ID,
                                SERVICE_ID = Oper_ID,
                                CORELATIONID = sRandomOTP
                            };
                            db.TBL_ACCOUNTS.Add(WLobjval);
                            await db.SaveChangesAsync();
                            decimal Update_WhiteLBLBal = 0;
                            decimal WLAvaiBal = 0;
                            var UpdateWLBalance = await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == WHT_MEM_ID);
                            if (UpdateWLBalance != null)
                            {
                                decimal.TryParse(UpdateWLBalance.BALANCE.ToString(), out WLAvaiBal);
                                //Update_WhiteLBLBal = WLAvaiBal + AddWL_ServiceAmtAdd;
                                Update_WhiteLBLBal = WLAvaiBal + WL_NET_COmm;
                                UpdateWLBalance.BALANCE = Update_WhiteLBLBal;
                                db.Entry(UpdateWLBalance).State = System.Data.Entity.EntityState.Modified;
                                await db.SaveChangesAsync();
                            }
                            else
                            {
                                UpdateWLBalance.BALANCE = WL_NET_COmm;
                                db.Entry(UpdateWLBalance).State = System.Data.Entity.EntityState.Modified;
                                await db.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            WLserviceAmtAdd = 0;
                            WLService_AMT = WLCOMM.COMM_VALUE;
                            WLColsingAmtonServi = WLserviceAmtAdd + WL_NET_COmm;

                            TBL_ACCOUNTS WLobjval = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = (long)WHT_MEM_ID,
                                MEMBER_TYPE = "WHITE LEVEL",
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = WL_NET_COmm,
                                NARRATION = "Cash Back To WLP",
                                OPENING = 0,
                                CLOSING = WLColsingAmtonServi,
                                REC_NO = 0,
                                COMM_AMT = WL_NET_COmm,
                                GST = (double)gst_VAlue,
                                //TDS = (double)GST_AMount,
                                TDS = (double)WLCOMM_TDSVal,
                                IPAddress = IpAddress,
                                //TDS_PERCENTAGE = WLCOMM_TDSVal,
                                TDS_PERCENTAGE = GST_AMount,
                                GST_PERCENTAGE = WLP_GST,
                                WHITELEVEL_ID = (long)WHT_MEM_ID,
                                //SUPER_ID = (long)SUP_MEM_ID,
                                SUPER_ID = 0,
                                DISTRIBUTOR_ID = (long)DIS_MEM_ID,
                                SERVICE_ID = Oper_ID,
                                CORELATIONID = sRandomOTP
                            };
                            db.TBL_ACCOUNTS.Add(WLobjval);
                            await db.SaveChangesAsync();
                            decimal Update_WhiteLBLBal = 0;
                            decimal WLAvaiBal = 0;
                            var UpdateWLBalance = await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == WHT_MEM_ID);
                            if (UpdateWLBalance != null)
                            {
                                decimal.TryParse(UpdateWLBalance.BALANCE.ToString(), out WLAvaiBal);
                                //Update_WhiteLBLBal = WLAvaiBal + AddWL_ServiceAmtAdd;
                                Update_WhiteLBLBal = WLAvaiBal + WL_NET_COmm;
                                UpdateWLBalance.BALANCE = Update_WhiteLBLBal;
                                db.Entry(UpdateWLBalance).State = System.Data.Entity.EntityState.Modified;
                                await db.SaveChangesAsync();
                            }
                            else
                            {
                                UpdateWLBalance.BALANCE = WL_NET_COmm;
                                db.Entry(UpdateWLBalance).State = System.Data.Entity.EntityState.Modified;
                                await db.SaveChangesAsync();
                            }
                        }
                        #endregion
                        //var MerchantComm =await (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                        //                    join commslabMob in db.TBL_COMM_SLAB_DMR_PAYMENT
                        //                    on detailscom.SLN equals commslabMob.SLAB_ID
                        //                    //join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                        //                    where detailscom.SLAB_TYPE == 3 && commslabMob.MERCHANT_ROLE_ID == 5 && commslabMob.DMT_TYPE == "DOMESTIC" && detailscom.MEM_ID== WHT_MEM_ID
                        //                    select new
                        //                    {
                        //                        SLN = commslabMob.SLN,
                        //                        Slab_ID = commslabMob.SLAB_ID,
                        //                        Slab_From = commslabMob.SLAB_FROM,
                        //                        Slab_To = commslabMob.SLAB_TO,
                        //                        commPer = commslabMob.MERCHANT_COM_PER,
                        //                        commType = commslabMob.MERCHANT_COMM_TYPE
                        //                    }).ToListAsync();

                        var MerchantComm = (from WLPComm in db.TBL_WHITE_LEVEL_COMMISSION_SLAB join detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB on WLPComm.SLN equals detailscom.DMR_SLAB
                                            join commslabMob in db.TBL_COMM_SLAB_DMR_PAYMENT
                                            on detailscom.DMR_SLAB equals commslabMob.SLAB_ID
                                            where detailscom.WHITE_LEVEL_ID == DIS_MEM_ID && detailscom.INTRODUCER_ID == Mem_ID && commslabMob.DMT_TYPE == "DOMESTIC"
                                            select new
                                            {
                                                //SLN = commslabMob.SLN,
                                                //Slab_ID = commslabMob.SLAB_ID,
                                                //Slab_From = commslabMob.SLAB_FROM,
                                                //Slab_To = commslabMob.SLAB_TO,
                                                //commPer = commslabMob.COMM_PERCENTAGE,
                                                SLN = commslabMob.SLN,
                                                Slab_ID = commslabMob.SLAB_ID,
                                                Slab_From = commslabMob.SLAB_FROM,
                                                Slab_To = commslabMob.SLAB_TO,
                                                commPer = commslabMob.MERCHANT_COM_PER,
                                                commType = commslabMob.MERCHANT_COMM_TYPE,
                                                SLAB_TDS= WLPComm.SLAB_TDS

                                            }).ToList();


                        decimal commamt = 0;
                        string CommTypeVal = string.Empty;
                        string TDSSlabApplied = string.Empty;
                        foreach (var comslab in MerchantComm)
                        {
                            if (Trans_Amt >= comslab.Slab_From && Trans_Amt <= comslab.Slab_To)
                            {
                                commamt = comslab.commPer;
                                CommTypeVal = comslab.commType;
                                TDSSlabApplied = comslab.SLAB_TDS;
                            }
                        }
                        Merchant_Com_pr = commamt;
                        #region Retailer Commission                        
                        var membtype =await (from mm in db.TBL_MASTER_MEMBER
                                        join
                                            rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                        where mm.MEM_ID == Mem_ID
                                        select new
                                        {
                                            RoleId = mm.MEMBER_ROLE,
                                            roleName = rm.ROLE_NAME,
                                            Amount = mm.BALANCE
                                        }).FirstOrDefaultAsync();
                        //var tbl_account =await db.TBL_ACCOUNTS.Where(z => z.MEM_ID == Mem_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefaultAsync();
                        var tbl_account = await db.TBL_ACCOUNTS.Where(z => z.MEM_ID == Mem_ID).OrderByDescending(z => z.ACC_NO).FirstOrDefaultAsync();
                        if (tbl_account != null)
                        {
                            decimal ClosingAmt = tbl_account.CLOSING;
                            //decimal SubAmt = ClosingAmt - Trans_Amt;
                            decimal SubAmt = ClosingAmt ;
                            //TBL_ACCOUNTS objaccnt = new TBL_ACCOUNTS()
                            //{
                            //    API_ID = 0,
                            //    MEM_ID = Mem_ID,
                            //    MEMBER_TYPE = membtype.roleName,
                            //    TRANSACTION_TYPE = rechargeType,
                            //    TRANSACTION_DATE = System.DateTime.Now,
                            //    TRANSACTION_TIME = DateTime.Now,
                            //    DR_CR = "DR",
                            //    AMOUNT = Trans_Amt,
                            //    NARRATION = "DMT",
                            //    OPENING = ClosingAmt,
                            //    CLOSING = SubAmt,
                            //    REC_NO = 0,
                            //    COMM_AMT = 0,
                            //    GST = 0,
                            //    TDS = 0,
                            //    IPAddress = IpAddress,
                            //    TDS_PERCENTAGE = 0,
                            //    GST_PERCENTAGE = 0,
                            //    WHITELEVEL_ID = (long)WHT_MEM_ID,
                            //    SUPER_ID = (long)SUP_MEM_ID,
                            //    DISTRIBUTOR_ID = (long)DIS_MEM_ID
                            //};
                            //db.TBL_ACCOUNTS.Add(objaccnt);
                            //db.SaveChanges();
                            ////decimal getPer = (Trans_Amt * MerchantComm.commPer) / 100;
                            decimal getPer = 0;

                            //decimal CommAddClosing = SubAmt - commamt;
                            //// DMR Commission GST Addition and TDS Subtraction
                            decimal getPer_Val = 0;
                            //float FloatgetPer_Val = float.Parse(getPer_Val.ToString());
                            // Get TDS and GST
                            decimal RetailerTDS_AmtValue = 0;
                            decimal GStCal = 0;
                            decimal CalculateGST = 0;
                            if (CommTypeVal == "FIXED")
                            {
                                if (TDSSlabApplied == "Yes")
                                {
                                    getPer_Val = commamt;
                                    getPer = getPer_Val;
                                    MERGROSSComm = getPer;
                                    RetailerTDS_AmtValue = decimal.Round(((MERGROSSComm * GST_AMount) / 100), 2);
                                    MER_RetailerTDS_AmtValue = RetailerTDS_AmtValue;
                                    MERNETComm = MERGROSSComm - MER_RetailerTDS_AmtValue;
                                }
                                else
                                {
                                    getPer_Val = commamt;
                                    getPer = getPer_Val;
                                    MERGROSSComm = getPer;
                                    RetailerTDS_AmtValue = 0;
                                    MER_RetailerTDS_AmtValue = RetailerTDS_AmtValue;
                                    MERNETComm = MERGROSSComm - MER_RetailerTDS_AmtValue;
                                }

                                
                                //if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                //{

                                //    RetailerTDS_AmtValue = decimal.Round(((getPer_Val * GST_AMount) / 100), 2);
                                //    if (MemberGST != null)
                                //    {
                                //        CalculateGST = decimal.Round(((getPer_Val * gst_VAlue) / 100), 2);                                        
                                //        getPer = getPer_Val - RetailerTDS_AmtValue;
                                //    }
                                //    else
                                //    {
                                //        CalculateGST = 0;                                        
                                //        getPer = getPer_Val - RetailerTDS_AmtValue;
                                //    }
                                //}
                                //else
                                //{
                                //    RetailerTDS_AmtValue = decimal.Round(((getPer_Val - GST_AMount)));
                                //    if (MemberGST != null)
                                //    {
                                //        CalculateGST = decimal.Round(((getPer_Val - gst_VAlue)));                                        
                                //        getPer = getPer_Val - RetailerTDS_AmtValue;
                                //    }
                                //    else
                                //    {
                                //        CalculateGST = 0;

                                //        getPer = getPer_Val - RetailerTDS_AmtValue;
                                //    }

                                //}
                            }
                            else
                            {
                                //FixedSErviceAmt
                                //getPer_Val = (Trans_Amt * commamt) / 100;
                                if (TDSSlabApplied == "Yes")
                                {
                                    getPer_Val = (FixedSErviceAmt * commamt) / 100;
                                    getPer = getPer_Val;
                                    MERGROSSComm = getPer;
                                    RetailerTDS_AmtValue = decimal.Round(((MERGROSSComm * GST_AMount) / 100), 2);
                                    MER_RetailerTDS_AmtValue = RetailerTDS_AmtValue;
                                    MERNETComm = MERGROSSComm - MER_RetailerTDS_AmtValue;
                                }
                                else
                                {
                                    getPer_Val = (FixedSErviceAmt * commamt) / 100;
                                    getPer = getPer_Val;
                                    MERGROSSComm = getPer;
                                    RetailerTDS_AmtValue = 0;
                                    MER_RetailerTDS_AmtValue = RetailerTDS_AmtValue;
                                    MERNETComm = MERGROSSComm - MER_RetailerTDS_AmtValue;
                                }
                               

                                //if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                //{

                                //    RetailerTDS_AmtValue = decimal.Round(((getPer_Val * GST_AMount) / 100), 2);
                                //    if (MemberGST != null)
                                //    {
                                //        CalculateGST = decimal.Round(((getPer_Val * gst_VAlue) / 100), 2);                                        
                                //        getPer = getPer_Val - RetailerTDS_AmtValue;
                                //    }
                                //    else
                                //    {
                                //        CalculateGST = 0;                                        
                                //        getPer = getPer_Val - RetailerTDS_AmtValue;
                                //    }

                                //}
                                //else
                                //{
                                //    RetailerTDS_AmtValue = decimal.Round(((getPer_Val - GST_AMount)));
                                //    if (MemberGST != null)
                                //    {
                                //        CalculateGST = decimal.Round(((getPer_Val - gst_VAlue)));                                        
                                //        getPer = getPer_Val - RetailerTDS_AmtValue;
                                //    }
                                //    else
                                //    {
                                //        CalculateGST = 0;                                        
                                //        getPer = getPer_Val - RetailerTDS_AmtValue;
                                //    }

                                //}

                            }
                            //DMR Commission GST Addition and TDS Substration
                            MerGrossComm = getPer;
                            decimal CommAddClosing = SubAmt + getPer;
                            TBL_ACCOUNTS objCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Mem_ID,
                                MEMBER_TYPE = membtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = getPer,
                                NARRATION = "Commission To Retailer",
                                OPENING = SubAmt,
                                CLOSING = CommAddClosing,
                                REC_NO = 0,
                                COMM_AMT = getPer,
                                GST = (float)CalculateGST,
                                TDS = (float)RetailerTDS_AmtValue,
                                IPAddress = IpAddress,
                                TDS_PERCENTAGE = taxMaster.TAX_VALUE,
                                GST_PERCENTAGE = GST_Master.TAX_VALUE,
                                WHITELEVEL_ID = (long)WHT_MEM_ID,
                                //SUPER_ID = (long)SUP_MEM_ID,
                                SUPER_ID = 0,
                                DISTRIBUTOR_ID = (long)DIS_MEM_ID,
                                SERVICE_ID = 0,
                                CORELATIONID = sRandomOTP
                            };
                            db.TBL_ACCOUNTS.Add(objCommPer);
                            await db.SaveChangesAsync();
                            decimal mer_balance = 0;
                            decimal AddCMmer_balance = 0;
                            var MerchantBal = await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == Mem_ID);
                            if (MerchantBal != null)
                            {
                                mer_balance = Convert.ToDecimal(MerchantBal.BALANCE);
                                AddCMmer_balance = mer_balance + MERNETComm;
                            }
                            else
                            {
                                mer_balance = Convert.ToDecimal(MerchantBal.BALANCE);
                                AddCMmer_balance = MERNETComm;
                            }
                            MerchantBal.BALANCE = AddCMmer_balance;
                            db.Entry(MerchantBal).State = System.Data.Entity.EntityState.Modified;
                            await db.SaveChangesAsync();
                        }
                        #endregion


                        var MerchantComm12 =await (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                              join commslabMob in db.TBL_COMM_SLAB_DMR_PAYMENT
                                              on detailscom.SLN equals commslabMob.SLAB_ID
                                              //join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                                              where detailscom.SLAB_TYPE == 3 && commslabMob.MERCHANT_ROLE_ID == 4 && commslabMob.DMT_TYPE == "DOMESTIC"
                                              select new
                                              {
                                                  SLN = commslabMob.SLN,
                                                  Slab_ID = commslabMob.SLAB_ID,
                                                  Slab_From = commslabMob.SLAB_FROM,
                                                  Slab_To = commslabMob.SLAB_TO,
                                                  commPer = commslabMob.MERCHANT_COM_PER,
                                                  commType = commslabMob.COMMISSION_TYPE
                                              }).ToListAsync();

                        //var DistributorComm =await (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                        //                       join commslabMob in db.TBL_COMM_SLAB_DMR_PAYMENT
                        //                       on detailscom.SLN equals commslabMob.SLAB_ID
                        //                       //join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                        //                       where detailscom.SLAB_TYPE == 3 && commslabMob.DISTRIBUTOR_ROLE_ID == 4 && commslabMob.DMT_TYPE == "DOMESTIC" && detailscom.MEM_ID == WHT_MEM_ID
                        //                       select new
                        //                       {
                        //                           SLN = commslabMob.SLN,
                        //                           Slab_ID = commslabMob.SLAB_ID,
                        //                           Slab_From = commslabMob.SLAB_FROM,
                        //                           Slab_To = commslabMob.SLAB_TO,
                        //                           commPer = commslabMob.DISTRIBUTOR_COM_PER,
                        //                           commType = commslabMob.DISTRIBUTOR_COMM_TYPE
                        //                       }).ToListAsync();

                        var DistributorComm = (from WLPComm in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                            join detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB on WLPComm.SLN equals detailscom.DMR_SLAB
                                            join commslabMob in db.TBL_COMM_SLAB_DMR_PAYMENT
                                            on detailscom.DMR_SLAB equals commslabMob.SLAB_ID
                                            where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == DIS_MEM_ID && commslabMob.DMT_TYPE == "DOMESTIC"
                                            select new
                                            {
                                                //SLN = commslabMob.SLN,
                                                //Slab_ID = commslabMob.SLAB_ID,
                                                //Slab_From = commslabMob.SLAB_FROM,
                                                //Slab_To = commslabMob.SLAB_TO,
                                                //commPer = commslabMob.COMM_PERCENTAGE,
                                                SLN = commslabMob.SLN,
                                                Slab_ID = commslabMob.SLAB_ID,
                                                Slab_From = commslabMob.SLAB_FROM,
                                                Slab_To = commslabMob.SLAB_TO,
                                                commPer = commslabMob.DISTRIBUTOR_COM_PER,
                                                commType = commslabMob.DISTRIBUTOR_COMM_TYPE,
                                                SLAB_TDS = WLPComm.SLAB_TDS

                                            }).ToList();


                        //var DistributorComm12 = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                       join commslabMob in db.TBL_COMM_SLAB_DMR_PAYMENT
                        //                       on detailscom.DMR_SLAB equals commslabMob.SLAB_ID
                        //                       where detailscom.WHITE_LEVEL_ID == SUP_MEM_ID && detailscom.INTRODUCER_ID == DIS_MEM_ID && commslabMob.DMT_TYPE == "DOMESTIC"
                        //                       select new
                        //                       {
                        //                           SLN = commslabMob.SLN,
                        //                           Slab_ID = commslabMob.SLAB_ID,
                        //                           Slab_From = commslabMob.SLAB_FROM,
                        //                           Slab_To = commslabMob.SLAB_TO,
                        //                           commPer = commslabMob.COMM_PERCENTAGE,
                        //                       }).ToList();
                        decimal DistributorDMRComm = 0;
                        string DistCommType = string.Empty;
                        string DistTDS_APPLIED = string.Empty;
                        foreach (var comslab in DistributorComm)
                        {
                            if (Trans_Amt >= comslab.Slab_From && Trans_Amt <= comslab.Slab_To)
                            {
                                DistributorDMRComm = comslab.commPer;
                                DistCommType = comslab.commType;
                                DistTDS_APPLIED = comslab.SLAB_TDS;
                            }
                        }
                        Distributor_Com_pr = DistributorDMRComm;
                        decimal DmrDisgapcomm = 0;
                        //DmrDisgapcomm = commamt - DistributorDMRComm;
                        DmrDisgapcomm = DistributorDMRComm - commamt;

                        #region Distributor DMR Commission   
                        var Dismembtype12 =await (from mm in db.TBL_MASTER_MEMBER
                                             join
                                                 rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                             where mm.MEM_ID == DIS_MEM_ID
                                             select new
                                             {
                                                 RoleId = mm.MEMBER_ROLE,
                                                 roleName = rm.ROLE_NAME,
                                                 Amount = mm.BALANCE
                                             }).FirstOrDefaultAsync();

                        var DIsmembtype =await (from mm in db.TBL_MASTER_MEMBER
                                           join
                                               rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                           where mm.MEM_ID == DIS_MEM_ID
                                           select new
                                           {
                                               RoleId = mm.MEMBER_ROLE,
                                               roleName = rm.ROLE_NAME,
                                               Amount = mm.BALANCE
                                           }).FirstOrDefaultAsync();
                        //var tbl_accountDMRDis =await db.TBL_ACCOUNTS.Where(z => z.MEM_ID == DIS_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefaultAsync();
                        var tbl_accountDMRDis = await db.TBL_ACCOUNTS.Where(z => z.MEM_ID == DIS_MEM_ID).OrderByDescending(z => z.ACC_NO).FirstOrDefaultAsync();
                        if (tbl_accountDMRDis != null)
                        {
                            decimal DMRDISClosingAmt = tbl_accountDMRDis.CLOSING;
                            //decimal DMRDISSubAmt = DMRDISClosingAmt - Trans_Amt;

                            decimal getPer = 0;

                            decimal DisGapComm = 0;
                            //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;
                            decimal DisGapCommAmt_Val = 0;
                            decimal DisGapCommAmt_TDS = 0;
                            decimal DisGapCommAmt = 0;

                            decimal DIS_GST_Amt = 0;
                            decimal Dis_Calculate_GST = 0;


                            if (DistCommType == "FIXED")
                            {

                                ////DisGapCommAmt_Val = DistributorDMRComm;
                                if (DistTDS_APPLIED == "Yes")
                                {
                                    DisGapCommAmt_Val = DmrDisgapcomm;
                                    DisGapCommAmt = DisGapCommAmt_Val;
                                    DISTGROSSComm = DisGapCommAmt;
                                    DIST_RetailerTDS_AmtValue = decimal.Round(((DISTGROSSComm * GST_AMount) / 100), 2);
                                    DISTNETComm = DISTGROSSComm - DIST_RetailerTDS_AmtValue;
                                }
                                else
                                {
                                    DisGapCommAmt_Val = DmrDisgapcomm;
                                    DisGapCommAmt = DisGapCommAmt_Val;
                                    DISTGROSSComm = DisGapCommAmt;
                                    DIST_RetailerTDS_AmtValue = 0;
                                    DISTNETComm = DISTGROSSComm - DIST_RetailerTDS_AmtValue;
                                }
                                
                                //if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                //{
                                //    DisGapComm = decimal.Parse(DistributorDMRComm.ToString());
                                //    DisGapCommAmt_Val = DisGapComm;
                                //    DisGapCommAmt_TDS = decimal.Round(((DisGapCommAmt_Val * GST_AMount) / 100), 2);
                                //    if (MemberGST != null)
                                //    {
                                //        Dis_Calculate_GST = decimal.Round(((DisGapCommAmt_Val * gst_VAlue) / 100), 2);                                        
                                //        DisGapCommAmt = DisGapCommAmt_Val - DisGapCommAmt_TDS;
                                //    }
                                //    else
                                //    {
                                //        Dis_Calculate_GST = 0;                                        
                                //        DisGapCommAmt = DisGapCommAmt_Val - DisGapCommAmt_TDS;
                                //    }
                                //}
                                //else
                                //{
                                //    DisGapComm = decimal.Parse(DistributorDMRComm.ToString());
                                //    DisGapCommAmt_Val = DisGapComm;
                                //    DisGapCommAmt_TDS = decimal.Round(((DisGapCommAmt_Val - GST_AMount)));
                                //    if (MemberGST != null)
                                //    {
                                //        Dis_Calculate_GST = decimal.Round(((DisGapCommAmt_Val - gst_VAlue)));
                                //        //DisGapCommAmt = DisGapCommAmt_Val + Dis_Calculate_GST - DisGapCommAmt_TDS;
                                //        DisGapCommAmt = DisGapCommAmt_Val - DisGapCommAmt_TDS;
                                //    }
                                //    else
                                //    {
                                //        Dis_Calculate_GST = 0;
                                //        //DisGapCommAmt = DisGapCommAmt_Val + Dis_Calculate_GST - DisGapCommAmt_TDS;
                                //        DisGapCommAmt = DisGapCommAmt_Val - DisGapCommAmt_TDS;
                                //    }

                                //}
                            }
                            else
                            {
                                //DisGapComm = decimal.Parse(DistributorDMRComm.ToString());

                                if (DistTDS_APPLIED == "Yes")
                                {
                                    DisGapComm = decimal.Parse(DmrDisgapcomm.ToString());
                                    DisGapCommAmt_Val = (FixedSErviceAmt * DisGapComm) / 100;
                                    DisGapCommAmt = DisGapCommAmt_Val;
                                    DISTGROSSComm = DisGapCommAmt;
                                    DIST_RetailerTDS_AmtValue = decimal.Round(((DISTGROSSComm * GST_AMount) / 100), 2);
                                    DISTNETComm = DISTGROSSComm - DIST_RetailerTDS_AmtValue;
                                }
                                else
                                {
                                    DisGapComm = decimal.Parse(DmrDisgapcomm.ToString());
                                    DisGapCommAmt_Val = (FixedSErviceAmt * DisGapComm) / 100;
                                    DisGapCommAmt = DisGapCommAmt_Val;
                                    DISTGROSSComm = DisGapCommAmt;
                                    DIST_RetailerTDS_AmtValue = 0;
                                    DISTNETComm = DISTGROSSComm - DIST_RetailerTDS_AmtValue;
                                }
                                

                                //if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                //{
                                //    DisGapCommAmt_TDS = decimal.Round(((DisGapCommAmt_Val * GST_AMount) / 100), 2);
                                //    if (MemberGST != null)
                                //    {
                                //        Dis_Calculate_GST = decimal.Round(((DisGapCommAmt_Val * gst_VAlue) / 100), 2);                                        
                                //        DisGapCommAmt = DisGapCommAmt_Val - DisGapCommAmt_TDS;
                                //    }
                                //    else
                                //    {
                                //        Dis_Calculate_GST = 0;
                                //        DisGapCommAmt = DisGapCommAmt_Val - DisGapCommAmt_TDS;
                                //    }
                                //}
                                //else
                                //{
                                //    DisGapCommAmt_TDS = decimal.Round(((DisGapCommAmt_Val - GST_AMount)));
                                //    if (MemberGST != null)
                                //    {
                                //        Dis_Calculate_GST = decimal.Round(((DisGapCommAmt_Val - gst_VAlue)));                                        
                                //        DisGapCommAmt = DisGapCommAmt_Val - DisGapCommAmt_TDS;
                                //    }
                                //    else
                                //    {
                                //        Dis_Calculate_GST = 0;                                        
                                //        DisGapCommAmt = DisGapCommAmt_Val - DisGapCommAmt_TDS;
                                //    }
                                //}

                            }
                            DISTGrossComm = DisGapCommAmt;
                            decimal CommDisAddClosing = DMRDISClosingAmt + DisGapCommAmt;
                            long Dis_ID = long.Parse(DIS_MEM_ID.ToString());


                            //decimal DMRDisCommAddClosing = DMRDISClosingAmt - DmrDisgapcomm;
                            long dis_idval = long.Parse(DIS_MEM_ID.ToString());
                            TBL_ACCOUNTS objCommPerDis = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = dis_idval,
                                MEMBER_TYPE = DIsmembtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = DisGapCommAmt,
                                NARRATION = "Commission to Distributor",
                                OPENING = DMRDISClosingAmt,
                                CLOSING = CommDisAddClosing,
                                REC_NO = 0,
                                //COMM_AMT = DisGapCommAmt,
                                COMM_AMT= DisGapCommAmt,
                                GST = (float)Dis_Calculate_GST,
                                TDS = (float)DisGapCommAmt_TDS,
                                IPAddress = IpAddress,
                                TDS_PERCENTAGE = taxMaster.TAX_VALUE,
                                GST_PERCENTAGE = GST_Master.TAX_VALUE,
                                WHITELEVEL_ID = (long)WHT_MEM_ID,
                                //SUPER_ID = (long)SUP_MEM_ID,
                                SUPER_ID = 0,
                                DISTRIBUTOR_ID = (long)DIS_MEM_ID,
                                SERVICE_ID = 0,
                                CORELATIONID = sRandomOTP
                            };
                            db.TBL_ACCOUNTS.Add(objCommPerDis);
                            await db.SaveChangesAsync();

                            decimal DistB_balance = 0;
                            decimal AddCDistB_balance = 0;
                            var DistributorBal = await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == dis_idval);
                            if (DistributorBal != null)
                            {
                                DistB_balance = Convert.ToDecimal(DistributorBal.BALANCE);
                                AddCDistB_balance = DistB_balance + DISTNETComm;
                            }
                            else
                            {
                                DistB_balance = Convert.ToDecimal(DistributorBal.BALANCE);
                                AddCDistB_balance = MERNETComm;
                            }
                            DistributorBal.BALANCE = AddCDistB_balance;
                            db.Entry(DistributorBal).State = System.Data.Entity.EntityState.Modified;
                            await db.SaveChangesAsync();


                        }
                        #endregion


                        var SuperComm =await (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                         join commslabMob in db.TBL_COMM_SLAB_DMR_PAYMENT
                                         on detailscom.SLN equals commslabMob.SLAB_ID
                                         //join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                                         where detailscom.SLAB_TYPE == 3 && commslabMob.SUPER_ROLE_ID == 3 && commslabMob.DMT_TYPE == "DOMESTIC" && detailscom.MEM_ID == WHT_MEM_ID
                                         select new
                                         {
                                             SLN = commslabMob.SLN,
                                             Slab_ID = commslabMob.SLAB_ID,
                                             Slab_From = commslabMob.SLAB_FROM,
                                             Slab_To = commslabMob.SLAB_TO,
                                             commPer = commslabMob.SUPER_COM_PER,
                                             commType = commslabMob.COMMISSION_TYPE
                                         }).ToListAsync();

                        //var SuperComm_12 = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                 join commslabMob in db.TBL_COMM_SLAB_DMR_PAYMENT
                        //                  on detailscom.DMR_SLAB equals commslabMob.SLAB_ID
                        //                 where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == SUP_MEM_ID && commslabMob.DMT_TYPE == "DOMESTIC"
                        //                 select new
                        //                 {
                        //                     SLN = commslabMob.SLN,
                        //                     Slab_ID = commslabMob.SLAB_ID,
                        //                     Slab_From = commslabMob.SLAB_FROM,
                        //                     Slab_To = commslabMob.SLAB_TO,
                        //                     commPer = commslabMob.COMM_PERCENTAGE,
                        //                 }).ToList();
                        decimal SuperDMRComm = 0;
                        string SuperCommType = string.Empty;
                        foreach (var comslab in SuperComm)
                        {
                            if (Trans_Amt >= comslab.Slab_From && Trans_Amt <= comslab.Slab_To)
                            {
                                SuperDMRComm = comslab.commPer;
                                SuperCommType = comslab.commType;
                            }
                        }
                        Super_Com_pr = SuperDMRComm;
                        decimal DmrSUPgapcomm = 0;
                        DmrSUPgapcomm = DistributorDMRComm - SuperDMRComm;
                        #region Super DMR Commission                        
                        //var Supmembtype =await (from mm in db.TBL_MASTER_MEMBER
                        //                   join
                        //                       rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                        //                   where mm.MEM_ID == SUP_MEM_ID
                        //                   select new
                        //                   {
                        //                       RoleId = mm.MEMBER_ROLE,
                        //                       roleName = rm.ROLE_NAME,
                        //                       Amount = mm.BALANCE
                        //                   }).FirstOrDefaultAsync();
                        //var tbl_accountSupDis =await db.TBL_ACCOUNTS.Where(z => z.MEM_ID == SUP_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefaultAsync();
                        //if (tbl_accountSupDis != null)
                        //{
                        //    decimal DMRSupClosingAmt = tbl_accountSupDis.CLOSING;
                        //    //decimal DMRDISSubAmt = DMRDISClosingAmt - Trans_Amt;

                        //    decimal getPer = 0;
                        //    // Calculate GST Value and TDS
                        //    decimal SupClosingAmt = tbl_accountSupDis.CLOSING;

                        //    decimal SupGapComm = 0;
                        //    //decimal SupGapCommAmt = (Trans_Amt * SupGapComm) / 100;
                        //    // GST and TDS Calculation for Super
                        //    decimal SupGapCommAmt_val = 0;
                        //    decimal Sp_TDSAmt = 0;
                        //    decimal SupGapCommAmt = 0;
                        //    decimal SUP_GstAmt = 0;
                        //    decimal Sup_CalculateGST = 0;
                        //    if (SuperCommType == "FIXED")
                        //    {
                        //        SupGapComm = decimal.Parse(SuperDMRComm.ToString());
                        //        SupGapCommAmt = SupGapComm;
                        //        SPRGROSSComm = SupGapCommAmt;
                        //        SPR_RetailerTDS_AmtValue = decimal.Round(((SPRGROSSComm * GST_AMount) / 100), 2);
                        //        SPRTNETComm = SPRGROSSComm - SPR_RetailerTDS_AmtValue;
                        //        //// GST and TDS Calculation for Super
                        //        //SupGapCommAmt_val = SupGapComm;
                        //        //if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                        //        //{
                        //        //    Sp_TDSAmt = decimal.Round(((SupGapCommAmt_val * GST_AMount) / 100), 2);
                        //        //    if (MemberGST != null)
                        //        //    {
                        //        //        Sup_CalculateGST = decimal.Round(((SupGapCommAmt_val * gst_VAlue) / 100), 2);                                        
                        //        //        SupGapCommAmt = SupGapCommAmt_val - Sp_TDSAmt;
                        //        //    }
                        //        //    else
                        //        //    {
                        //        //        Sup_CalculateGST = 0;                                        
                        //        //        SupGapCommAmt = SupGapCommAmt_val - Sp_TDSAmt;
                        //        //    }
                        //        //}
                        //        //else
                        //        //{
                        //        //    Sp_TDSAmt = decimal.Round(((SupGapCommAmt_val - GST_AMount)));
                        //        //    if (MemberGST != null)
                        //        //    {
                        //        //        Sup_CalculateGST = decimal.Round(((SupGapCommAmt_val - gst_VAlue)));
                        //        //        SupGapCommAmt = SupGapCommAmt_val - Sp_TDSAmt;
                        //        //    }
                        //        //    else
                        //        //    {
                        //        //        Sup_CalculateGST = 0;
                        //        //        SupGapCommAmt = SupGapCommAmt_val - Sp_TDSAmt;
                        //        //    }

                        //        //}
                        //    }
                        //    else
                        //    {
                        //        SupGapComm = decimal.Parse(SuperDMRComm.ToString());

                        //        //// GST and TDS Calculation for Super
                        //        //SupGapCommAmt_val = (Trans_Amt * SupGapComm) / 100;
                        //        SupGapCommAmt_val = (FixedSErviceAmt * SupGapComm) / 100;
                        //        SupGapCommAmt = SupGapCommAmt_val;

                        //        SPRGROSSComm = SupGapCommAmt;
                        //        SPR_RetailerTDS_AmtValue = decimal.Round(((SPRGROSSComm * GST_AMount) / 100), 2);
                        //        SPRTNETComm = SPRGROSSComm - SPR_RetailerTDS_AmtValue;

                        //        //if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                        //        //{
                        //        //    Sp_TDSAmt = decimal.Round(((SupGapCommAmt_val * GST_AMount) / 100), 2);
                        //        //    if (MemberGST != null)
                        //        //    {
                        //        //        Sup_CalculateGST = decimal.Round(((SupGapCommAmt_val * gst_VAlue) / 100), 2);
                        //        //        SupGapCommAmt = SupGapCommAmt_val - Sp_TDSAmt;
                        //        //    }
                        //        //    else
                        //        //    {
                        //        //        Sup_CalculateGST = 0;
                        //        //        SupGapCommAmt = SupGapCommAmt_val - Sp_TDSAmt;
                        //        //    }
                        //        //}
                        //        //else
                        //        //{
                        //        //    Sp_TDSAmt = decimal.Round(((SupGapCommAmt_val - GST_AMount)));
                        //        //    if (MemberGST != null)
                        //        //    {
                        //        //        Sup_CalculateGST = decimal.Round(((SupGapCommAmt_val - gst_VAlue)));
                        //        //        SupGapCommAmt = SupGapCommAmt_val - Sp_TDSAmt;
                        //        //    }
                        //        //    else
                        //        //    {
                        //        //        Sup_CalculateGST = 0;
                        //        //        SupGapCommAmt = SupGapCommAmt_val - Sp_TDSAmt;
                        //        //    }
                        //        //}
                        //    }
                        //    SUPERGrossComm = SupGapCommAmt;
                        //    decimal CommSupAddClosing = tbl_accountSupDis.CLOSING + SupGapCommAmt;
                        //    long Sup_ID = long.Parse(SUP_MEM_ID.ToString());
                        //    // Calculate GST and TDS

                        //    //decimal DMRSUPCommAddClosing = DMRSupClosingAmt - DmrSUPgapcomm;
                        //    long sup_idval = long.Parse(SUP_MEM_ID.ToString());
                        //    TBL_ACCOUNTS objCommPerSup = new TBL_ACCOUNTS()
                        //    {
                        //        API_ID = 0,
                        //        MEM_ID = sup_idval,
                        //        MEMBER_TYPE = Supmembtype.roleName,
                        //        TRANSACTION_TYPE = rechargeType,
                        //        TRANSACTION_DATE = System.DateTime.Now,
                        //        TRANSACTION_TIME = DateTime.Now,
                        //        DR_CR = "CR",
                        //        AMOUNT = SupGapCommAmt,
                        //        NARRATION = "Commission To Super",
                        //        OPENING = DMRSupClosingAmt,
                        //        CLOSING = CommSupAddClosing,
                        //        REC_NO = 0,
                        //        COMM_AMT = SupGapCommAmt,
                        //        GST = (float)Sup_CalculateGST,
                        //        TDS = (float)Sp_TDSAmt,
                        //        IPAddress = IpAddress,
                        //        TDS_PERCENTAGE = taxMaster.TAX_VALUE,
                        //        GST_PERCENTAGE = GST_Master.TAX_VALUE,
                        //        WHITELEVEL_ID = (long)WHT_MEM_ID,
                        //        SUPER_ID = (long)SUP_MEM_ID,
                        //        DISTRIBUTOR_ID = (long)DIS_MEM_ID,
                        //        SERVICE_ID = 0,
                        //        CORELATIONID = sRandomOTP
                        //    };
                        //    db.TBL_ACCOUNTS.Add(objCommPerSup);
                        //    await db.SaveChangesAsync();

                        //    decimal Supr_balance = 0;
                        //    decimal AddCSupr_balance = 0;
                        //    var SPR_rBal = await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == sup_idval);
                        //    if (SPR_rBal != null)
                        //    {
                        //        Supr_balance = Convert.ToDecimal(SPR_rBal.BALANCE);
                        //        AddCSupr_balance = Supr_balance + SPRTNETComm;
                        //    }
                        //    else
                        //    {
                        //        Supr_balance = Convert.ToDecimal(SPR_rBal.BALANCE);
                        //        AddCSupr_balance = SPRTNETComm;
                        //    }
                        //    SPR_rBal.BALANCE = AddCSupr_balance;
                        //    db.Entry(SPR_rBal).State = System.Data.Entity.EntityState.Modified;
                        //    await db.SaveChangesAsync();

                        //}
                        #endregion



                        #region Update TRANSXT_DMR_TRANSACTION_LIST  UPDATE ON CoRelation ID

                        var Remitterno =await db.TBL_DMR_RECIPIENT_DETAILS.FirstOrDefaultAsync(x => x.MEM_ID == Mem_ID);

                        
                        decimal Res_Fee = 0;
                        decimal.TryParse(apiRES.fee,out Res_Fee);

                        WLTransactionChargeAmt.CUSTOMER_ID = Remitterno.CUSTOMER_ID;
                        WLTransactionChargeAmt.RECIPIENT_ID = Remitterno.RECIPIENT_ID;
                        WLTransactionChargeAmt.CLIENT_REF_ID = apiRES.clientRefId;
                        WLTransactionChargeAmt.RES_FEE = Res_Fee;
                        WLTransactionChargeAmt.INITIATOR_ID = apiRES.initiatorId;
                        WLTransactionChargeAmt.ACCOUNT_NO = apiRES.accountNumber;
                        WLTransactionChargeAmt.API_TXN_STATUS = apiRES.txnStatus;
                        WLTransactionChargeAmt.NAME = apiRES.name;
                        WLTransactionChargeAmt.IFSC_CODE = apiRES.ifscCode;
                        WLTransactionChargeAmt.IMPS_RES_CODE = apiRES.impsRespCode;
                        WLTransactionChargeAmt.IMPS_RES_MSG = apiRES.impsRespMessage;
                        WLTransactionChargeAmt.TXN_ID = apiRES.txnId;
                        WLTransactionChargeAmt.TIMESTAMP = apiRES.timestamp;
                        WLTransactionChargeAmt.SENDER_NAME = "Retailer";
                        WLTransactionChargeAmt.SENDER_MOBILE_NO = Remitterno.CUSTOMER_ID;
                        WLTransactionChargeAmt.TRANSACTION_DATE = DateTime.Now;
                        WLTransactionChargeAmt.TRANSACTION_STATUS = apiRES.impsRespMessage;
                        WLTransactionChargeAmt.MER_WLP_COMM_TYPE = CommTypeVal;
                        WLTransactionChargeAmt.MER_WLP_COMM_RATE = Merchant_Com_pr;
                        WLTransactionChargeAmt.MER_GROSS_COMM_AMT = MERGROSSComm;
                        WLTransactionChargeAmt.MER_TDS_DR_COMM_AMT = MER_RetailerTDS_AmtValue;
                        WLTransactionChargeAmt.MER_NET_COMM = MERNETComm;
                        WLTransactionChargeAmt.MER_TRANSFER_FEE_TYPE = FixedService_Type;
                        WLTransactionChargeAmt.MER_TRANSFER_FEE_RATE = servGSt_amt;
                        WLTransactionChargeAmt.MER_TRANSFER_FEE = FixedSErviceAmt;
                        WLTransactionChargeAmt.MER_GST_INPUT = WL_MER_GSTvalue;
                        WLTransactionChargeAmt.DIST_WLP_COMM_TYPE = DistCommType;
                        WLTransactionChargeAmt.DIST_WLP_COMM_RATE = DistributorDMRComm;
                        WLTransactionChargeAmt.DIST_GROSS_COMM_AMT = DISTGROSSComm;
                        WLTransactionChargeAmt.DIST_TDS_DR_COMM_AMT = DIST_RetailerTDS_AmtValue;
                        WLTransactionChargeAmt.DIST_NET_COMM = DISTNETComm;
                        //WLTransactionChargeAmt.SPR_WLP_COMM_TYPE = SuperCommType;
                        //WLTransactionChargeAmt.SPR_WLP_COMM_RATE = SuperDMRComm;
                        //WLTransactionChargeAmt.SPR_GROSS_COMM_AMT = SPRGROSSComm;
                        //WLTransactionChargeAmt.SPR_TDS_DR_COMM_AMT = SPR_RetailerTDS_AmtValue;
                        //WLTransactionChargeAmt.SPR_NET_COMM = SPRTNETComm;
                        WLTransactionChargeAmt.SPR_WLP_COMM_TYPE = "";
                        WLTransactionChargeAmt.SPR_WLP_COMM_RATE = 0;
                        WLTransactionChargeAmt.SPR_GROSS_COMM_AMT = 0;
                        WLTransactionChargeAmt.SPR_TDS_DR_COMM_AMT = 0;
                        WLTransactionChargeAmt.SPR_NET_COMM = 0;
                        WLTransactionChargeAmt.WLP_PA_TRAN_RATE_TYPE = WLCOMM.COMMTYPE;
                        WLTransactionChargeAmt.WLP_PA_TRAN_RATE = WLCOMM.COMM_VALUE;
                        WLTransactionChargeAmt.WLP_TRANSFER_FEE_TYPE = FixedService_Type;
                        WLTransactionChargeAmt.WLP_TRANSFER_FEE_RATE = servGSt_amt;
                        WLTransactionChargeAmt.WLP_GST_OUTPUT = (decimal)MER_GST_Val;
                        WLTransactionChargeAmt.WLP_GROSS_COMM_AMT = WLGROSS_Value;
                        WLTransactionChargeAmt.WLP_TDS_DR_COMM_AMT = WLCOMM_TDSVal;
                        WLTransactionChargeAmt.WLP_NET_COMM = WL_NET_COmm;
                        WLTransactionChargeAmt.WLP_TRANSFER_FEE_TYPE = FixedService_Type;
                        WLTransactionChargeAmt.WLP_TRANSFER_FEE_RATE = servGSt_amt;
                        WLTransactionChargeAmt.WLP_TRANSFER_FEE = FixedSErviceAmt;
                        WLTransactionChargeAmt.WLP_GST_OUTPUT = WL_MER_GSTvalue;
                        WLTransactionChargeAmt.TDS_RATE = GST_AMount;
                        WLTransactionChargeAmt.GST_RATE = gst_VAlue;
                        WLTransactionChargeAmt.CORELATIONID = sRandomOTP;
                        WLTransactionChargeAmt.ERROR_TYPE = null;
                        WLTransactionChargeAmt.ISREVERSE = false;
                        WLTransactionChargeAmt.COMMISSIONDISBURSEDATE = DateTime.Now;
                        WLTransactionChargeAmt.ISCOMMISSIONDISBURSE =true;                        
                        WLTransactionChargeAmt.SLAB_ID = Convert.ToInt64(MerchantComm.Select(c=>c.Slab_ID).FirstOrDefault());
                        WLTransactionChargeAmt.API_RESPONSE = APIResponse;
                        db.Entry(WLTransactionChargeAmt).State = System.Data.Entity.EntityState.Modified;
                        await db.SaveChangesAsync();

                        #endregion



                        ContextTransaction.Commit();
                        return true;
                    }
                    else if (status == "LANDLINE")
                    {
                        var MerchantComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                            join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                            on detailscom.SLN equals commslabMob.SLAB_ID
                                            join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                                            where commslabMob.OPERATOR_CODE == serviceprovider && detailscom.SLAB_TYPE == 1 && mem.MEMBER_ROLE == 5
                                            select new
                                            {
                                                SLN = commslabMob.SLN,
                                                commPer = commslabMob.MERCHANT_COM_PER,
                                                commType = commslabMob.COMMISSION_TYPE
                                            }).FirstOrDefault();

                        //var MerchantComm2 = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                    join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                        //                    on detailscom.RECHARGE_SLAB equals commslabMob.SLAB_ID
                        //                    where detailscom.WHITE_LEVEL_ID == DIS_MEM_ID && detailscom.INTRODUCER_ID == Mem_ID && commslabMob.OPERATOR_CODE == serviceprovider
                        //                    select new
                        //                    {
                        //                        SLN = commslabMob.SLN,
                        //                        commPer = commslabMob.COMM_PERCENTAGE
                        //                    }).FirstOrDefault();
                        //var MerchantComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                    join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                        //                    on detailscom.RECHARGE_SLAB equals commslabMob.SLAB_ID
                        //                    where detailscom.WHITE_LEVEL_ID == DIS_MEM_ID && detailscom.INTRODUCER_ID == Mem_ID && commslabMob.OPERATOR_CODE == serviceprovider
                        //                    select new
                        //                    {
                        //                        SLN = commslabMob.SLN,
                        //                        commPer = commslabMob.COMM_PERCENTAGE
                        //                    }).FirstOrDefault();
                        #region Retailer Commission                        
                        var membtype = (from mm in db.TBL_MASTER_MEMBER
                                        join
                                            rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                        where mm.MEM_ID == Mem_ID
                                        select new
                                        {
                                            RoleId = mm.MEMBER_ROLE,
                                            roleName = rm.ROLE_NAME,
                                            Amount = mm.BALANCE
                                        }).FirstOrDefault();
                        var tbl_account = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == Mem_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        if (tbl_account != null)
                        {
                            decimal ClosingAmt = tbl_account.CLOSING;
                            decimal SubAmt = ClosingAmt - Trans_Amt;
                            TBL_ACCOUNTS objaccnt = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Mem_ID,
                                MEMBER_TYPE = membtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "DR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Recharge",
                                OPENING = ClosingAmt,
                                CLOSING = SubAmt,
                                REC_NO = 0,
                                COMM_AMT = 0,
                                GST = 0,
                                TDS = 0,
                                IPAddress = IpAddress,
                                TDS_PERCENTAGE = 0,
                                GST_PERCENTAGE = 0,
                                WHITELEVEL_ID = (long)WHT_MEM_ID,
                                //SUPER_ID = (long)SUP_MEM_ID,
                                SUPER_ID = 0,
                                DISTRIBUTOR_ID = (long)DIS_MEM_ID
                            };
                            db.TBL_ACCOUNTS.Add(objaccnt);
                            db.SaveChanges();

                            decimal getPer_Val = 0;
                            // Get TDS and GST
                            decimal RetailerTDS_AmtValue = 0;
                            decimal getPer = 0;
                            decimal LandMErGSTCAl = 0;
                            if (MerchantComm.commType == "FIXED")
                            {
                                getPer_Val = MerchantComm.commPer;
                                // Get TDS and GST
                                if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                {
                                    RetailerTDS_AmtValue = decimal.Round(((getPer_Val * GST_AMount) / 100), 2);
                                    if (MemberGST != null)
                                    {
                                        LandMErGSTCAl = decimal.Round(((getPer_Val * gst_VAlue) / 100), 2);
                                        getPer = getPer_Val + LandMErGSTCAl - RetailerTDS_AmtValue;
                                    }
                                    else
                                    {
                                        LandMErGSTCAl = 0;
                                        getPer = getPer_Val + LandMErGSTCAl - RetailerTDS_AmtValue;
                                    }
                                }
                                else
                                {
                                    RetailerTDS_AmtValue = decimal.Round(((getPer_Val - GST_AMount)),2);
                                    if (MemberGST != null)
                                    {
                                        LandMErGSTCAl = decimal.Round(((getPer_Val - gst_VAlue)),2);
                                        getPer = getPer_Val + LandMErGSTCAl - RetailerTDS_AmtValue;
                                    }
                                    else
                                    {
                                        LandMErGSTCAl = 0;
                                        getPer = getPer_Val + LandMErGSTCAl - RetailerTDS_AmtValue;
                                    }
                                }
                            }
                            else
                            {
                                getPer_Val = (Trans_Amt * MerchantComm.commPer) / 100;
                                // Get TDS and GST
                                if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                {
                                    RetailerTDS_AmtValue = decimal.Round(((getPer_Val * GST_AMount) / 100), 2);
                                    if (MemberGST != null)
                                    {
                                        LandMErGSTCAl = decimal.Round(((getPer_Val * gst_VAlue) / 100), 2);
                                        getPer = getPer_Val + LandMErGSTCAl - RetailerTDS_AmtValue;
                                    }
                                    else
                                    {
                                        LandMErGSTCAl = 0;
                                        getPer = getPer_Val + LandMErGSTCAl - RetailerTDS_AmtValue;
                                    }

                                }
                                else
                                {
                                    RetailerTDS_AmtValue = decimal.Round(((getPer_Val - GST_AMount)),2);
                                    if (MemberGST != null)
                                    {
                                        LandMErGSTCAl = decimal.Round(((getPer_Val - gst_VAlue)),2);
                                        getPer = getPer_Val + LandMErGSTCAl - RetailerTDS_AmtValue;
                                    }
                                    else
                                    {
                                        LandMErGSTCAl = 0;
                                        getPer = getPer_Val + LandMErGSTCAl - RetailerTDS_AmtValue;
                                    }

                                }

                            }

                            decimal CommAddClosing = SubAmt + getPer;
                            TBL_ACCOUNTS objCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Mem_ID,
                                MEMBER_TYPE = membtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = SubAmt,
                                CLOSING = CommAddClosing,
                                REC_NO = 0,
                                COMM_AMT = getPer,
                                GST = (float)LandMErGSTCAl,
                                TDS = (float)RetailerTDS_AmtValue,
                                IPAddress = IpAddress,
                                TDS_PERCENTAGE = taxMaster.TAX_VALUE,
                                GST_PERCENTAGE = GST_Master.TAX_VALUE,
                                WHITELEVEL_ID = (long)WHT_MEM_ID,
                                //SUPER_ID = (long)SUP_MEM_ID,
                                SUPER_ID = 0,
                                DISTRIBUTOR_ID = (long)DIS_MEM_ID
                            };
                            db.TBL_ACCOUNTS.Add(objCommPer);
                            db.SaveChanges();
                        }
                        #endregion

                        var DistributorComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                               join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                               on detailscom.SLN equals commslabMob.SLAB_ID
                                               join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                                               where commslabMob.OPERATOR_CODE == serviceprovider && detailscom.SLAB_TYPE == 1 && mem.MEMBER_ROLE == 4
                                               select new
                                               {
                                                   SLN = commslabMob.SLN,
                                                   commPer = commslabMob.DISTRIBUTOR_COM_PER,
                                                   commType = commslabMob.COMMISSION_TYPE
                                               }).FirstOrDefault();
                        //var DistributorComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                       join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                        //                       on detailscom.RECHARGE_SLAB equals commslabMob.SLAB_ID
                        //                       where detailscom.WHITE_LEVEL_ID == SUP_MEM_ID && detailscom.INTRODUCER_ID == DIS_MEM_ID && commslabMob.OPERATOR_CODE == serviceprovider
                        //                       select new
                        //                       {
                        //                           SLN = commslabMob.SLN,
                        //                           commPer = commslabMob.COMM_PERCENTAGE
                        //                       }).FirstOrDefault();
                        #region Distributor Commission
                        //decimal DisGapComm = decimal.Parse(DistributorComm.commPer.ToString()) - decimal.Parse(MerchantComm.commPer.ToString());

                        var tbl_accountDistributor = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == DIS_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        if (tbl_accountDistributor != null)
                        {
                            var CheckDisCommission = db.TBL_DETAILS_MEMBER_COMMISSION_SLAB.Where(x => x.INTRODUCER_ID == DIS_MEM_ID).FirstOrDefault();
                            var Dismembtype = (from mm in db.TBL_MASTER_MEMBER
                                               join
                                                   rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                               where mm.MEM_ID == DIS_MEM_ID
                                               select new
                                               {
                                                   RoleId = mm.MEMBER_ROLE,
                                                   roleName = rm.ROLE_NAME,
                                                   Amount = mm.BALANCE
                                               }).FirstOrDefault();
                            decimal DisClosingAmt = tbl_accountDistributor.CLOSING;
                            //decimal SubAmt = ClosingAmt - Trans_Amt;                   
                            //decimal DisgetPer = (Trans_Amt * checkDiscomm.CommPercentage) / 100;

                            //  Distributor GST and TDS
                            //decimal CommDisAddClosing = tbl_accountDistributor.CLOSING + DisGapCommAmt;
                            decimal DisGapComm = decimal.Parse(DistributorComm.commPer.ToString());
                            //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;
                            decimal DisGapCommAmt_Val = 0;
                            decimal DisGapCommAmt_TDS = 0;
                            decimal DisGapCommAmt = 0;
                            decimal LND_DIST_GSTCAl = 0;
                            if (DistributorComm.commType == "FIXED")
                            {
                                DisGapCommAmt_Val = DisGapComm;
                                if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                {

                                    DisGapCommAmt_TDS = decimal.Round(((DisGapCommAmt_Val * GST_AMount) / 100), 2);
                                    if (MemberGST != null)
                                    {
                                        LND_DIST_GSTCAl = decimal.Round(((DisGapCommAmt_Val * gst_VAlue) / 100), 2);
                                        DisGapCommAmt = DisGapCommAmt_Val + LND_DIST_GSTCAl - DisGapCommAmt_TDS;
                                    }
                                    else
                                    {
                                        LND_DIST_GSTCAl = 0;
                                        DisGapCommAmt = DisGapCommAmt_Val + LND_DIST_GSTCAl - DisGapCommAmt_TDS;
                                    }

                                }
                                else
                                {
                                    DisGapCommAmt_TDS = decimal.Round(((DisGapCommAmt_Val - GST_AMount)));
                                    if (MemberGST != null)
                                    {
                                        LND_DIST_GSTCAl = decimal.Round(((DisGapCommAmt_Val - gst_VAlue)));
                                        DisGapCommAmt = DisGapCommAmt_Val + LND_DIST_GSTCAl - DisGapCommAmt_TDS;
                                    }
                                    else
                                    {
                                        LND_DIST_GSTCAl = 0;
                                        DisGapCommAmt = DisGapCommAmt_Val + LND_DIST_GSTCAl - DisGapCommAmt_TDS;
                                    }
                                }
                            }
                            else
                            {
                                DisGapCommAmt_Val = (Trans_Amt * DisGapComm) / 100;
                                if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                {
                                    DisGapCommAmt_TDS = decimal.Round(((DisGapCommAmt_Val * GST_AMount) / 100), 2);
                                    if (MemberGST != null)
                                    {
                                        LND_DIST_GSTCAl = decimal.Round(((DisGapCommAmt_Val * gst_VAlue) / 100), 2);
                                        DisGapCommAmt = DisGapCommAmt_Val + LND_DIST_GSTCAl - DisGapCommAmt_TDS;
                                    }
                                    else
                                    {
                                        LND_DIST_GSTCAl = 0;
                                        DisGapCommAmt = DisGapCommAmt_Val + LND_DIST_GSTCAl - DisGapCommAmt_TDS;
                                    }

                                }
                                else
                                {
                                    DisGapCommAmt_TDS = decimal.Round(((DisGapCommAmt_Val - GST_AMount)));
                                    if (MemberGST != null)
                                    {
                                        LND_DIST_GSTCAl = decimal.Round(((DisGapCommAmt_Val - gst_VAlue)));
                                        DisGapCommAmt = DisGapCommAmt_Val + LND_DIST_GSTCAl - DisGapCommAmt_TDS;
                                    }
                                    else
                                    {
                                        LND_DIST_GSTCAl = 0;
                                        DisGapCommAmt = DisGapCommAmt_Val + LND_DIST_GSTCAl - DisGapCommAmt_TDS;
                                    }

                                }

                            }

                            decimal CommDisAddClosing = tbl_accountDistributor.CLOSING + DisGapCommAmt;
                            long Dis_ID = long.Parse(DIS_MEM_ID.ToString());
                            TBL_ACCOUNTS objDisCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Dis_ID,
                                MEMBER_TYPE = Dismembtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = DisClosingAmt,
                                CLOSING = CommDisAddClosing,
                                REC_NO = 0,
                                COMM_AMT = DisGapCommAmt,
                                GST = (float)LND_DIST_GSTCAl,
                                TDS = (float)DisGapCommAmt_TDS,
                                IPAddress = IpAddress,
                                TDS_PERCENTAGE = taxMaster.TAX_VALUE,
                                GST_PERCENTAGE = GST_Master.TAX_VALUE,
                                WHITELEVEL_ID = (long)WHT_MEM_ID,
                                //SUPER_ID = (long)SUP_MEM_ID,
                                SUPER_ID =0,
                                DISTRIBUTOR_ID = (long)DIS_MEM_ID
                            };
                            db.TBL_ACCOUNTS.Add(objDisCommPer);
                            db.SaveChanges();
                        }
                        #endregion
                        var SuperComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                         join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                          on detailscom.SLN equals commslabMob.SLAB_ID
                                         join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                                         where commslabMob.OPERATOR_CODE == serviceprovider && detailscom.SLAB_TYPE == 1 && mem.MEMBER_ROLE == 3
                                         select new
                                         {
                                             SLN = commslabMob.SLN,
                                             commPer = commslabMob.SUPER_COM_PER,
                                             commType = commslabMob.COMMISSION_TYPE
                                         }).FirstOrDefault();
                        

                        //#region Super Commission
                        //var tbl_accountSuper = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == SUP_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        //if (tbl_accountSuper != null)
                        //{
                        //    var Supmembtype = (from mm in db.TBL_MASTER_MEMBER
                        //                       join
                        //                           rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                        //                       where mm.MEM_ID == SUP_MEM_ID
                        //                       select new
                        //                       {
                        //                           RoleId = mm.MEMBER_ROLE,
                        //                           roleName = rm.ROLE_NAME,
                        //                           Amount = mm.BALANCE
                        //                       }).FirstOrDefault();
                        //    decimal SupClosingAmt = tbl_accountSuper.CLOSING;

                        //    decimal SupGapComm = decimal.Parse(SuperComm.commPer.ToString());
                        //    //decimal SupGapCommAmt = (Trans_Amt * SupGapComm) / 100;
                        //    // GST and TDS Calculation for Super
                        //    decimal SupGapCommAmt_val = 0;
                        //    decimal Sp_TDSAmt = 0;
                        //    decimal SupGapCommAmt = 0;
                        //    decimal LNDSUpER_GSTCAl = 0;
                        //    if (SuperComm.commType == "FIXED")
                        //    {
                        //        SupGapCommAmt_val = SupGapComm;
                        //        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                        //        {
                        //            Sp_TDSAmt = decimal.Round(((SupGapCommAmt_val * GST_AMount) / 100), 2);
                        //            if (MemberGST != null)
                        //            {
                        //                LNDSUpER_GSTCAl = decimal.Round(((SupGapCommAmt_val * gst_VAlue) / 100), 2);
                        //                SupGapCommAmt = SupGapCommAmt_val + LNDSUpER_GSTCAl - Sp_TDSAmt;
                        //            }
                        //            else
                        //            {
                        //                LNDSUpER_GSTCAl = 0;
                        //                SupGapCommAmt = SupGapCommAmt_val + LNDSUpER_GSTCAl - Sp_TDSAmt;
                        //            }
                        //        }
                        //        else
                        //        {
                        //            Sp_TDSAmt = decimal.Round(((SupGapCommAmt_val - GST_AMount)));
                        //            if (MemberGST != null)
                        //            {
                        //                LNDSUpER_GSTCAl = decimal.Round(((SupGapCommAmt_val - gst_VAlue) / 100), 2);
                        //                SupGapCommAmt = SupGapCommAmt_val + LNDSUpER_GSTCAl - Sp_TDSAmt;
                        //            }
                        //            else
                        //            {
                        //                LNDSUpER_GSTCAl = 0;
                        //                SupGapCommAmt = SupGapCommAmt_val + LNDSUpER_GSTCAl - Sp_TDSAmt;
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        SupGapCommAmt_val = (Trans_Amt * SupGapComm) / 100;
                        //        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                        //        {
                        //            Sp_TDSAmt = decimal.Round(((SupGapCommAmt_val * GST_AMount) / 100), 2);
                        //            if (MemberGST != null)
                        //            {
                        //                LNDSUpER_GSTCAl = decimal.Round(((SupGapCommAmt_val * gst_VAlue) / 100), 2);
                        //                SupGapCommAmt = SupGapCommAmt_val + LNDSUpER_GSTCAl - Sp_TDSAmt;
                        //            }
                        //            else
                        //            {
                        //                LNDSUpER_GSTCAl = 0;
                        //                SupGapCommAmt = SupGapCommAmt_val + LNDSUpER_GSTCAl - Sp_TDSAmt;
                        //            }
                        //        }
                        //        else
                        //        {
                        //            Sp_TDSAmt = decimal.Round(((SupGapCommAmt_val * GST_AMount)));
                        //            if (MemberGST != null)
                        //            {
                        //                LNDSUpER_GSTCAl = decimal.Round(((SupGapCommAmt_val * gst_VAlue)));
                        //                SupGapCommAmt = SupGapCommAmt_val + LNDSUpER_GSTCAl - Sp_TDSAmt;
                        //            }
                        //            else
                        //            {
                        //                LNDSUpER_GSTCAl = 0;
                        //                SupGapCommAmt = SupGapCommAmt_val + LNDSUpER_GSTCAl - Sp_TDSAmt;
                        //            }
                        //        }
                        //    }
                        //    decimal CommSupAddClosing = tbl_accountSuper.CLOSING + SupGapCommAmt;
                        //    long Sup_ID = long.Parse(SUP_MEM_ID.ToString());
                        //    TBL_ACCOUNTS objSupCommPer = new TBL_ACCOUNTS()
                        //    {
                        //        API_ID = 0,
                        //        MEM_ID = Sup_ID,
                        //        MEMBER_TYPE = Supmembtype.roleName,
                        //        TRANSACTION_TYPE = rechargeType,
                        //        TRANSACTION_DATE = System.DateTime.Now,
                        //        TRANSACTION_TIME = DateTime.Now,
                        //        DR_CR = "CR",
                        //        AMOUNT = Trans_Amt,
                        //        NARRATION = "Commission",
                        //        OPENING = SupClosingAmt,
                        //        CLOSING = CommSupAddClosing,
                        //        REC_NO = 0,
                        //        COMM_AMT = SupGapCommAmt,
                        //        GST = (float)LNDSUpER_GSTCAl,
                        //        TDS = (float)Sp_TDSAmt,
                        //        IPAddress = IpAddress,
                        //        TDS_PERCENTAGE = taxMaster.TAX_VALUE,
                        //        GST_PERCENTAGE = GST_Master.TAX_VALUE,
                        //        WHITELEVEL_ID = (long)WHT_MEM_ID,
                        //        SUPER_ID = (long)SUP_MEM_ID,
                        //        DISTRIBUTOR_ID = (long)DIS_MEM_ID
                        //    };
                        //    db.TBL_ACCOUNTS.Add(objSupCommPer);
                        //    db.SaveChanges();
                        //}
                        //#endregion                        
                        var WhiteComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                                         join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                          on detailscom.RECHARGE_SLAB equals commslabMob.SLAB_ID
                                         where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == 0 && commslabMob.OPERATOR_CODE == serviceprovider
                                         select new
                                         {
                                             SLN = commslabMob.SLN,
                                             commPer = commslabMob.COMM_PERCENTAGE,
                                             commType = commslabMob.COMMISSION_TYPE
                                         }).FirstOrDefault();
                        //decimal WLComm = decimal.Parse(MerchantComm.commPer.ToString()) + decimal.Parse(DistributorComm.commPer.ToString()) + decimal.Parse(SuperComm.commPer.ToString());
                        decimal WLComm = decimal.Parse(MerchantComm.commPer.ToString()) + decimal.Parse(DistributorComm.commPer.ToString()) ;
                        decimal WTLGapComm = decimal.Parse(WhiteComm.commPer.ToString()) - WLComm;
                        //decimal WTLGapComm = decimal.Parse(WhiteComm.commPer.ToString());
                        //decimal WTLGapComm =WLComm;

                        #region White level Commission payment Structure
                        var tbl_accountWhiteLevel = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == WHT_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        if (tbl_accountWhiteLevel != null)
                        {
                            var WLmembtype = (from mm in db.TBL_MASTER_MEMBER
                                              join
                                                  rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                              where mm.MEM_ID == WHT_MEM_ID
                                              select new
                                              {
                                                  RoleId = mm.MEMBER_ROLE,
                                                  roleName = rm.ROLE_NAME,
                                                  Amount = mm.BALANCE
                                              }).FirstOrDefault();
                            decimal WLClosingAmt = tbl_accountWhiteLevel.CLOSING;
                            //decimal SubAmt = ClosingAmt - Trans_Amt;                  
                            // GST And TDS WhiteLevel Calculation

                            decimal WTLGapCommAmt_AMt = 0;
                            decimal WL_TDSCalculation = 0;
                            decimal WTLGapCommAmt = 0;
                            decimal LNDWL_GSTCAL = 0;
                            if (WhiteComm.commType == "FIXED")
                            {
                                WTLGapCommAmt_AMt = WTLGapComm;
                                if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                {
                                    WL_TDSCalculation = decimal.Round(((WTLGapCommAmt_AMt * GST_AMount) / 100), 2);
                                    if (MemberGST != null)
                                    {
                                        LNDWL_GSTCAL = decimal.Round(((WTLGapCommAmt_AMt * gst_VAlue) / 100), 2);
                                        WTLGapCommAmt = WTLGapCommAmt_AMt + LNDWL_GSTCAL - WL_TDSCalculation;
                                    }
                                    else
                                    {
                                        LNDWL_GSTCAL = 0;
                                        WTLGapCommAmt = WTLGapCommAmt_AMt + LNDWL_GSTCAL - WL_TDSCalculation;
                                    }

                                }
                                else
                                {
                                    WL_TDSCalculation = decimal.Round(((WTLGapCommAmt_AMt - GST_AMount)));
                                    if (MemberGST != null)
                                    {
                                        LNDWL_GSTCAL = decimal.Round(((WTLGapCommAmt_AMt - gst_VAlue)));
                                        WTLGapCommAmt = WTLGapCommAmt_AMt + LNDWL_GSTCAL - WL_TDSCalculation;
                                    }
                                    else
                                    {
                                        LNDWL_GSTCAL = 0;
                                        WTLGapCommAmt = WTLGapCommAmt_AMt + LNDWL_GSTCAL - WL_TDSCalculation;
                                    }

                                }

                            }
                            else
                            {
                                WTLGapCommAmt_AMt = (Trans_Amt * WTLGapComm) / 100;
                                if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                {
                                    WL_TDSCalculation = decimal.Round(((WTLGapCommAmt_AMt * GST_AMount) / 100), 2);
                                    if (MemberGST != null)
                                    {
                                        LNDWL_GSTCAL = decimal.Round(((WTLGapCommAmt_AMt * gst_VAlue) / 100), 2);
                                        WTLGapCommAmt = WTLGapCommAmt_AMt + LNDWL_GSTCAL - WL_TDSCalculation;
                                    }
                                    else
                                    {
                                        LNDWL_GSTCAL = 0;
                                        WTLGapCommAmt = WTLGapCommAmt_AMt + LNDWL_GSTCAL - WL_TDSCalculation;
                                    }

                                }
                                else
                                {
                                    WL_TDSCalculation = decimal.Round(((WTLGapCommAmt_AMt - GST_AMount)));
                                    if (MemberGST != null)
                                    {
                                        LNDWL_GSTCAL = decimal.Round(((WTLGapCommAmt_AMt - gst_VAlue)));
                                        WTLGapCommAmt = WTLGapCommAmt_AMt + LNDWL_GSTCAL - WL_TDSCalculation;
                                    }
                                    else
                                    {
                                        LNDWL_GSTCAL = 0;
                                        WTLGapCommAmt = WTLGapCommAmt_AMt + LNDWL_GSTCAL - WL_TDSCalculation;
                                    }
                                }
                            }

                            //decimal WTLGapCommAmt = (Trans_Amt * WTLGapComm) / 100;

                            decimal CommWLAddClosing = tbl_accountWhiteLevel.CLOSING + WTLGapCommAmt;
                            long WL_ID = long.Parse(WHT_MEM_ID.ToString());
                            TBL_ACCOUNTS objWLCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = WL_ID,
                                MEMBER_TYPE = WLmembtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = WLClosingAmt,
                                CLOSING = CommWLAddClosing,
                                REC_NO = 0,
                                COMM_AMT = WTLGapCommAmt,
                                GST = (float)LNDWL_GSTCAL,
                                TDS = (float)WL_TDSCalculation,
                                IPAddress = IpAddress,
                                TDS_PERCENTAGE = taxMaster.TAX_VALUE,
                                GST_PERCENTAGE = GST_Master.TAX_VALUE,
                                WHITELEVEL_ID = (long)WHT_MEM_ID,
                                //SUPER_ID = (long)SUP_MEM_ID,
                                SUPER_ID = 0,
                                DISTRIBUTOR_ID = (long)DIS_MEM_ID

                            };
                            db.TBL_ACCOUNTS.Add(objWLCommPer);
                            db.SaveChanges();
                        }
                        #endregion
                        ContextTransaction.Commit();
                        return true;
                    }
                    else if (status == "GAS")
                    {

                        var MerchantComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                            join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                            on detailscom.SLN equals commslabMob.SLAB_ID
                                            join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                                            where commslabMob.OPERATOR_CODE == serviceprovider && mem.MEMBER_ROLE == 5
                                            select new
                                            {
                                                SLN = commslabMob.SLN,
                                                commPer = commslabMob.MERCHANT_COM_PER,
                                                commType = commslabMob.COMMISSION_TYPE
                                            }).FirstOrDefault();
                        #region Retailer Commission                        
                        var membtype = (from mm in db.TBL_MASTER_MEMBER
                                        join
                                            rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                        where mm.MEM_ID == Mem_ID
                                        select new
                                        {
                                            RoleId = mm.MEMBER_ROLE,
                                            roleName = rm.ROLE_NAME,
                                            Amount = mm.BALANCE
                                        }).FirstOrDefault();
                        var tbl_account = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == Mem_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        if (tbl_account != null)
                        {
                            decimal ClosingAmt = tbl_account.CLOSING;
                            decimal SubAmt = ClosingAmt - Trans_Amt;
                            TBL_ACCOUNTS objaccnt = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Mem_ID,
                                MEMBER_TYPE = membtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "DR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Recharge",
                                OPENING = ClosingAmt,
                                CLOSING = SubAmt,
                                REC_NO = 0,
                                COMM_AMT = 0,
                                GST = 0,
                                TDS = 0,
                                IPAddress = IpAddress,
                                TDS_PERCENTAGE = 0,
                                GST_PERCENTAGE = 0,
                                WHITELEVEL_ID = (long)WHT_MEM_ID,
                                //SUPER_ID = (long)SUP_MEM_ID,
                                SUPER_ID =0,
                                DISTRIBUTOR_ID = (long)DIS_MEM_ID
                            };
                            db.TBL_ACCOUNTS.Add(objaccnt);
                            db.SaveChanges();
                            // TDS and GST CAlculation for Merchant for electricity
                            //decimal getPer = (Trans_Amt * MerchantComm.commPer) / 100;
                            decimal ELE_getPer_Val = 0;
                            decimal ELE_TDS_Value = 0;
                            decimal getPer = 0;
                            decimal WL_GSTCAl = 0;
                            if (MerchantComm.commType == "FIXED")
                            {
                                ELE_getPer_Val = MerchantComm.commPer;
                                if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                {
                                    ELE_TDS_Value = decimal.Round(((ELE_getPer_Val * GST_AMount) / 100), 2);
                                    if (MemberGST != null)
                                    {
                                        WL_GSTCAl = decimal.Round(((ELE_getPer_Val * gst_VAlue) / 100), 2);
                                        getPer = (ELE_getPer_Val + WL_GSTCAl - ELE_TDS_Value);
                                    }
                                    else
                                    {
                                        WL_GSTCAl = 0;
                                        getPer = (ELE_getPer_Val + WL_GSTCAl - ELE_TDS_Value);
                                    }

                                }
                                else
                                {
                                    ELE_TDS_Value = decimal.Round(((ELE_getPer_Val - GST_AMount)));
                                    if (MemberGST != null)
                                    {
                                        WL_GSTCAl = decimal.Round(((ELE_getPer_Val - gst_VAlue)));
                                        getPer = (ELE_getPer_Val + WL_GSTCAl - ELE_TDS_Value);
                                    }
                                    else
                                    {
                                        WL_GSTCAl = 0;
                                        getPer = (ELE_getPer_Val + WL_GSTCAl - ELE_TDS_Value);
                                    }

                                }

                            }
                            else
                            {
                                ELE_getPer_Val = (Trans_Amt * MerchantComm.commPer) / 100;
                                if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                {
                                    ELE_TDS_Value = decimal.Round(((ELE_getPer_Val * GST_AMount) / 100), 2);
                                    if (MemberGST != null)
                                    {
                                        WL_GSTCAl = decimal.Round(((ELE_getPer_Val * gst_VAlue) / 100), 2);
                                        getPer = (ELE_getPer_Val + WL_GSTCAl - ELE_TDS_Value);
                                    }
                                    else
                                    {
                                        WL_GSTCAl = 0;
                                        getPer = (ELE_getPer_Val + WL_GSTCAl - ELE_TDS_Value);
                                    }

                                }
                                else
                                {

                                    ELE_TDS_Value = decimal.Round(((ELE_getPer_Val - GST_AMount)));
                                    if (MemberGST != null)
                                    {
                                        WL_GSTCAl = decimal.Round(((ELE_getPer_Val - gst_VAlue)));
                                        getPer = (ELE_getPer_Val + WL_GSTCAl - ELE_TDS_Value);
                                    }
                                    else
                                    {
                                        WL_GSTCAl = 0;
                                        getPer = (ELE_getPer_Val + WL_GSTCAl - ELE_TDS_Value);
                                    }

                                }

                            }
                            decimal CommAddClosing = SubAmt + getPer;
                            TBL_ACCOUNTS objCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Mem_ID,
                                MEMBER_TYPE = membtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = SubAmt,
                                CLOSING = CommAddClosing,
                                REC_NO = 0,
                                COMM_AMT = getPer,
                                GST = (float)WL_GSTCAl,
                                TDS = (float)ELE_TDS_Value,
                                IPAddress = IpAddress,
                                TDS_PERCENTAGE = taxMaster.TAX_VALUE,
                                GST_PERCENTAGE = GST_Master.TAX_VALUE,
                                WHITELEVEL_ID = (long)WHT_MEM_ID,
                                //SUPER_ID = (long)SUP_MEM_ID,
                                SUPER_ID = 0,
                                DISTRIBUTOR_ID = (long)DIS_MEM_ID
                            };
                            db.TBL_ACCOUNTS.Add(objCommPer);
                            db.SaveChanges();
                        }
                        #endregion

                        //var DistributorComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                       join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
                        //                       on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
                        //                       where detailscom.WHITE_LEVEL_ID == SUP_MEM_ID && detailscom.INTRODUCER_ID == DIS_MEM_ID && commslabMob.OPERATOR_CODE == serviceprovider
                        //                       select new
                        //                       {
                        //                           SLN = commslabMob.SLN,
                        //                           commPer = commslabMob.COMM_PERCENTAGE
                        //                       }).FirstOrDefault();
                        var DistributorComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                               join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                               on detailscom.SLN equals commslabMob.SLAB_ID
                                               join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                                               where commslabMob.OPERATOR_CODE == serviceprovider && mem.MEMBER_ROLE == 4
                                               select new
                                               {
                                                   SLN = commslabMob.SLN,
                                                   commPer = commslabMob.DISTRIBUTOR_COM_PER,
                                                   commType = commslabMob.COMMISSION_TYPE
                                               }).FirstOrDefault();
                        #region Distributor Commission
                        //decimal DisGapComm = decimal.Parse(DistributorComm.commPer.ToString()) - decimal.Parse(MerchantComm.commPer.ToString());
                        decimal DisGapComm = decimal.Parse(DistributorComm.commPer.ToString());
                        //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;
                        var tbl_accountDistributor = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == DIS_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        if (tbl_accountDistributor != null)
                        {
                            var CheckDisCommission = db.TBL_DETAILS_MEMBER_COMMISSION_SLAB.Where(x => x.INTRODUCER_ID == DIS_MEM_ID).FirstOrDefault();
                            var Dismembtype = (from mm in db.TBL_MASTER_MEMBER
                                               join
                                                   rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                               where mm.MEM_ID == DIS_MEM_ID
                                               select new
                                               {
                                                   RoleId = mm.MEMBER_ROLE,
                                                   roleName = rm.ROLE_NAME,
                                                   Amount = mm.BALANCE
                                               }).FirstOrDefault();
                            decimal DisClosingAmt = tbl_accountDistributor.CLOSING;
                            //decimal SubAmt = ClosingAmt - Trans_Amt;                   
                            //decimal DisgetPer = (Trans_Amt * checkDiscomm.CommPercentage) / 100;

                            // For TDS and GST CAlculation for Electricity
                            //decimal DisGapCommAmt = (Trans_Amt * DisGapComm) / 100;
                            decimal ELE_DisGap_CommAmt = 0;
                            decimal ELE_TDSCALCULATION = 0;
                            decimal DisGapCommAmt = 0;
                            decimal GasDist_GSTCAl = 0;
                            if (DistributorComm.commType == "FIXED")
                            {
                                ELE_DisGap_CommAmt = DisGapComm;
                                if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                {
                                    ELE_TDSCALCULATION = decimal.Round(((ELE_DisGap_CommAmt * GST_AMount) / 100), 2);
                                    if (MemberGST != null)
                                    {
                                        GasDist_GSTCAl = decimal.Round(((ELE_DisGap_CommAmt * gst_VAlue) / 100), 2);
                                        DisGapCommAmt = ELE_DisGap_CommAmt + GasDist_GSTCAl - ELE_TDSCALCULATION;
                                    }
                                    else
                                    {
                                        GasDist_GSTCAl = 0;
                                        DisGapCommAmt = ELE_DisGap_CommAmt + GasDist_GSTCAl - ELE_TDSCALCULATION;
                                    }

                                }
                                else
                                {

                                    ELE_TDSCALCULATION = decimal.Round(((ELE_DisGap_CommAmt - GST_AMount)));
                                    if (MemberGST != null)
                                    {
                                        GasDist_GSTCAl = decimal.Round(((ELE_DisGap_CommAmt - gst_VAlue)));
                                        DisGapCommAmt = ELE_DisGap_CommAmt + GasDist_GSTCAl - ELE_TDSCALCULATION;
                                    }
                                    else
                                    {
                                        GasDist_GSTCAl = 0;
                                        DisGapCommAmt = ELE_DisGap_CommAmt + GasDist_GSTCAl - ELE_TDSCALCULATION;
                                    }

                                }

                            }
                            else
                            {
                                ELE_DisGap_CommAmt = (Trans_Amt * DisGapComm) / 100;
                                if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                {
                                    ELE_TDSCALCULATION = decimal.Round(((ELE_DisGap_CommAmt * GST_AMount) / 100), 2);
                                    if (MemberGST != null)
                                    {
                                        GasDist_GSTCAl = decimal.Round(((ELE_DisGap_CommAmt * gst_VAlue) / 100), 2);
                                        DisGapCommAmt = ELE_DisGap_CommAmt + GasDist_GSTCAl - ELE_TDSCALCULATION;
                                    }
                                    else
                                    {
                                        GasDist_GSTCAl = 0;
                                        DisGapCommAmt = ELE_DisGap_CommAmt + GasDist_GSTCAl - ELE_TDSCALCULATION;
                                    }

                                }
                                else
                                {
                                    ELE_TDSCALCULATION = decimal.Round(((ELE_DisGap_CommAmt * GST_AMount) / 100), 2);
                                    if (MemberGST != null)
                                    {
                                        GasDist_GSTCAl = decimal.Round(((ELE_DisGap_CommAmt - gst_VAlue)));
                                        DisGapCommAmt = ELE_DisGap_CommAmt + GasDist_GSTCAl - ELE_TDSCALCULATION;
                                    }
                                    else
                                    {
                                        GasDist_GSTCAl = 0;
                                        DisGapCommAmt = ELE_DisGap_CommAmt + GasDist_GSTCAl - ELE_TDSCALCULATION;
                                    }

                                }

                            }
                            decimal CommDisAddClosing = tbl_accountDistributor.CLOSING + DisGapCommAmt;
                            long Dis_ID = long.Parse(DIS_MEM_ID.ToString());
                            TBL_ACCOUNTS objDisCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = Dis_ID,
                                MEMBER_TYPE = Dismembtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = DisClosingAmt,
                                CLOSING = CommDisAddClosing,
                                REC_NO = 0,
                                COMM_AMT = DisGapCommAmt,
                                GST = (float)GasDist_GSTCAl,
                                TDS = (float)ELE_TDSCALCULATION,
                                IPAddress = IpAddress,
                                TDS_PERCENTAGE = taxMaster.TAX_VALUE,
                                GST_PERCENTAGE = GST_Master.TAX_VALUE,
                                WHITELEVEL_ID = (long)WHT_MEM_ID,
                                //SUPER_ID = (long)SUP_MEM_ID,
                                SUPER_ID =0,
                                DISTRIBUTOR_ID = (long)DIS_MEM_ID
                            };
                            db.TBL_ACCOUNTS.Add(objDisCommPer);
                            db.SaveChanges();
                        }
                        #endregion
                        //var SuperComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                        //                 join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
                        //                  on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
                        //                 where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == SUP_MEM_ID && commslabMob.OPERATOR_CODE == serviceprovider
                        //                 select new
                        //                 {
                        //                     SLN = commslabMob.SLN,
                        //                     commPer = commslabMob.COMM_PERCENTAGE
                        //                 }).FirstOrDefault();
                        var SuperComm = (from detailscom in db.TBL_WHITE_LEVEL_COMMISSION_SLAB
                                         join commslabMob in db.TBL_COMM_SLAB_MOBILE_RECHARGE
                                          on detailscom.SLN equals commslabMob.SLAB_ID
                                         join mem in db.TBL_MASTER_MEMBER on detailscom.MEM_ID equals mem.UNDER_WHITE_LEVEL
                                         where commslabMob.OPERATOR_CODE == serviceprovider && mem.MEMBER_ROLE == 3
                                         select new
                                         {
                                             SLN = commslabMob.SLN,
                                             commPer = commslabMob.SUPER_COM_PER,
                                             commType = commslabMob.COMMISSION_TYPE
                                         }).FirstOrDefault();
                        //decimal SupGapComm = decimal.Parse(SuperComm.commPer.ToString()) - decimal.Parse(DistributorComm.commPer.ToString());
                        decimal SupGapComm = decimal.Parse(SuperComm.commPer.ToString());
                        //decimal SupGapCommAmt = (Trans_Amt * SupGapComm) / 100;
                        //#region Super Commission
                        //var tbl_accountSuper = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == SUP_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        //if (tbl_accountSuper != null)
                        //{
                        //    var Supmembtype = (from mm in db.TBL_MASTER_MEMBER
                        //                       join
                        //                           rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                        //                       where mm.MEM_ID == SUP_MEM_ID
                        //                       select new
                        //                       {
                        //                           RoleId = mm.MEMBER_ROLE,
                        //                           roleName = rm.ROLE_NAME,
                        //                           Amount = mm.BALANCE
                        //                       }).FirstOrDefault();
                        //    decimal SupClosingAmt = tbl_accountSuper.CLOSING;
                        //    // TDS AND GST CALCULATION FOR SUPER FOR ELECTRICITY
                        //    decimal ELE_Sup_Gap_Comm_Amt = 0;
                        //    decimal ELE_SUP_TDS_CAL = 0;
                        //    decimal SupGapCommAmt = 0;
                        //    decimal GAsSupGST_CAl = 0;
                        //    if (SuperComm.commType == "FIXED")
                        //    {
                        //        ELE_Sup_Gap_Comm_Amt = SupGapComm;
                        //        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                        //        {
                        //            ELE_SUP_TDS_CAL = decimal.Round(((ELE_Sup_Gap_Comm_Amt * GST_AMount) / 100), 2);
                        //            if (MemberGST != null)
                        //            {
                        //                GAsSupGST_CAl = decimal.Round(((ELE_Sup_Gap_Comm_Amt * gst_VAlue) / 100), 2);
                        //                SupGapCommAmt = ELE_Sup_Gap_Comm_Amt + GAsSupGST_CAl - ELE_SUP_TDS_CAL;
                        //            }
                        //            else
                        //            {
                        //                GAsSupGST_CAl = 0;
                        //                SupGapCommAmt = ELE_Sup_Gap_Comm_Amt + GAsSupGST_CAl - ELE_SUP_TDS_CAL;
                        //            }

                        //        }
                        //        else
                        //        {

                        //            ELE_SUP_TDS_CAL = decimal.Round(((ELE_Sup_Gap_Comm_Amt - GST_AMount)));
                        //            if (MemberGST != null)
                        //            {
                        //                GAsSupGST_CAl = decimal.Round(((ELE_Sup_Gap_Comm_Amt - gst_VAlue)));
                        //                SupGapCommAmt = ELE_Sup_Gap_Comm_Amt + GAsSupGST_CAl - ELE_SUP_TDS_CAL;
                        //            }
                        //            else
                        //            {
                        //                GAsSupGST_CAl = 0;
                        //                SupGapCommAmt = ELE_Sup_Gap_Comm_Amt + GAsSupGST_CAl - ELE_SUP_TDS_CAL;
                        //            }

                        //        }

                        //    }
                        //    else
                        //    {
                        //        ELE_Sup_Gap_Comm_Amt = (Trans_Amt * SupGapComm) / 100;
                        //        if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                        //        {

                        //            ELE_SUP_TDS_CAL = decimal.Round(((ELE_Sup_Gap_Comm_Amt * GST_AMount) / 100), 2);
                        //            if (MemberGST != null)
                        //            {
                        //                GAsSupGST_CAl = decimal.Round(((ELE_Sup_Gap_Comm_Amt * gst_VAlue) / 100), 2);
                        //                SupGapCommAmt = ELE_Sup_Gap_Comm_Amt + GAsSupGST_CAl - ELE_SUP_TDS_CAL;
                        //            }
                        //            else
                        //            {
                        //                GAsSupGST_CAl = 0;
                        //                SupGapCommAmt = ELE_Sup_Gap_Comm_Amt + GAsSupGST_CAl - ELE_SUP_TDS_CAL;
                        //            }

                        //        }
                        //        else
                        //        {

                        //            ELE_SUP_TDS_CAL = decimal.Round(((ELE_Sup_Gap_Comm_Amt - GST_AMount)));
                        //            if (MemberGST != null)
                        //            {
                        //                GAsSupGST_CAl = decimal.Round(((ELE_Sup_Gap_Comm_Amt - gst_VAlue)));
                        //                SupGapCommAmt = ELE_Sup_Gap_Comm_Amt + GAsSupGST_CAl - ELE_SUP_TDS_CAL;
                        //            }
                        //            else
                        //            {
                        //                GAsSupGST_CAl = 0;
                        //                SupGapCommAmt = ELE_Sup_Gap_Comm_Amt + GAsSupGST_CAl - ELE_SUP_TDS_CAL;
                        //            }

                        //        }
                        //    }
                        //    decimal CommSupAddClosing = tbl_accountSuper.CLOSING + SupGapCommAmt;
                        //    long Sup_ID = long.Parse(SUP_MEM_ID.ToString());
                        //    TBL_ACCOUNTS objSupCommPer = new TBL_ACCOUNTS()
                        //    {
                        //        API_ID = 0,
                        //        MEM_ID = Sup_ID,
                        //        MEMBER_TYPE = Supmembtype.roleName,
                        //        TRANSACTION_TYPE = rechargeType,
                        //        TRANSACTION_DATE = System.DateTime.Now,
                        //        TRANSACTION_TIME = DateTime.Now,
                        //        DR_CR = "CR",
                        //        AMOUNT = Trans_Amt,
                        //        NARRATION = "Commission",
                        //        OPENING = SupClosingAmt,
                        //        CLOSING = CommSupAddClosing,
                        //        REC_NO = 0,
                        //        COMM_AMT = SupGapCommAmt,
                        //        GST = (float)GAsSupGST_CAl,
                        //        TDS = (float)ELE_SUP_TDS_CAL,
                        //        IPAddress = IpAddress,
                        //        TDS_PERCENTAGE = taxMaster.TAX_VALUE,
                        //        GST_PERCENTAGE = GST_Master.TAX_VALUE,
                        //        WHITELEVEL_ID = (long)WHT_MEM_ID,
                        //        SUPER_ID = (long)SUP_MEM_ID,
                        //        DISTRIBUTOR_ID = (long)DIS_MEM_ID
                        //    };
                        //    db.TBL_ACCOUNTS.Add(objSupCommPer);
                        //    db.SaveChanges();
                        //}
                        //#endregion                        
                        var WhiteComm = (from detailscom in db.TBL_DETAILS_MEMBER_COMMISSION_SLAB
                                         join commslabMob in db.TBL_COMM_SLAB_UTILITY_RECHARGE
                                          on detailscom.BILLPAYMENT_SLAB equals commslabMob.SLAB_ID
                                         where detailscom.WHITE_LEVEL_ID == WHT_MEM_ID && detailscom.INTRODUCER_ID == 0 && commslabMob.OPERATOR_CODE == serviceprovider
                                         select new
                                         {
                                             SLN = commslabMob.SLN,
                                             commPer = commslabMob.COMM_PERCENTAGE,
                                             commType = commslabMob.COMMISSION_TYPE
                                         }).FirstOrDefault();
                        //decimal WL_Com = decimal.Parse(MerchantComm.commPer.ToString()) + decimal.Parse(DistributorComm.commPer.ToString()) + decimal.Parse(SuperComm.commPer.ToString());
                        decimal WL_Com = decimal.Parse(MerchantComm.commPer.ToString()) + decimal.Parse(DistributorComm.commPer.ToString()) ;
                        decimal WTLGapComm = decimal.Parse(WhiteComm.commPer.ToString()) - WL_Com;
                        //decimal WTLGapCommAmt = (Trans_Amt * WTLGapComm) / 100;
                        #region White level Commission payment Structure
                        var tbl_accountWhiteLevel = db.TBL_ACCOUNTS.Where(z => z.MEM_ID == WHT_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefault();
                        if (tbl_accountWhiteLevel != null)
                        {
                            var WLmembtype = (from mm in db.TBL_MASTER_MEMBER
                                              join
                                                  rm in db.TBL_MASTER_MEMBER_ROLE on mm.MEMBER_ROLE equals rm.ROLE_ID
                                              where mm.MEM_ID == WHT_MEM_ID
                                              select new
                                              {
                                                  RoleId = mm.MEMBER_ROLE,
                                                  roleName = rm.ROLE_NAME,
                                                  Amount = mm.BALANCE
                                              }).FirstOrDefault();
                            decimal WLClosingAmt = tbl_accountWhiteLevel.CLOSING;
                            //decimal SubAmt = ClosingAmt - Trans_Amt;           
                            // GST And TDS Calculation for WL 
                            //decimal WTLGapCommAmt = (Trans_Amt * WTLGapComm) / 100;
                            decimal ELE_WTL_Gap_CommAmt = 0;
                            decimal ELE_WL_TDSCal = 0;
                            decimal WTLGapCommAmt = 0;
                            decimal GasWTLGST_Cal = 0;
                            if (WhiteComm.commType == "FIXED")
                            {
                                ELE_WTL_Gap_CommAmt = WTLGapComm;
                                if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                {
                                    ELE_WL_TDSCal = decimal.Round(((ELE_WTL_Gap_CommAmt * GST_AMount) / 100), 2);
                                    if (MemberGST != null)
                                    {
                                        GasWTLGST_Cal = decimal.Round(((ELE_WTL_Gap_CommAmt * gst_VAlue) / 100), 2);
                                        WTLGapCommAmt = ELE_WTL_Gap_CommAmt + GasWTLGST_Cal - ELE_WL_TDSCal;
                                    }
                                    else
                                    {
                                        GasWTLGST_Cal = 0;
                                        WTLGapCommAmt = ELE_WTL_Gap_CommAmt + GasWTLGST_Cal - ELE_WL_TDSCal;
                                    }

                                }
                                else
                                {
                                    ELE_WL_TDSCal = decimal.Round(((ELE_WTL_Gap_CommAmt - GST_AMount)));
                                    if (MemberGST != null)
                                    {
                                        GasWTLGST_Cal = decimal.Round(((ELE_WTL_Gap_CommAmt - gst_VAlue)));
                                        WTLGapCommAmt = ELE_WTL_Gap_CommAmt + GasWTLGST_Cal - ELE_WL_TDSCal;
                                    }
                                    else
                                    {
                                        GasWTLGST_Cal = 0;
                                        WTLGapCommAmt = ELE_WTL_Gap_CommAmt + GasWTLGST_Cal - ELE_WL_TDSCal;
                                    }

                                }

                            }
                            else
                            {
                                ELE_WTL_Gap_CommAmt = (Trans_Amt * WTLGapComm) / 100;
                                if (TaxMode == "PERCENTAGE" && Gst_Mode == "PERCENTAGE")
                                {
                                    ELE_WL_TDSCal = decimal.Round(((ELE_WTL_Gap_CommAmt * GST_AMount) / 100), 2);
                                    if (MemberGST != null)
                                    {
                                        GasWTLGST_Cal = decimal.Round(((ELE_WTL_Gap_CommAmt * gst_VAlue) / 100), 2);
                                        WTLGapCommAmt = ELE_WTL_Gap_CommAmt + GasWTLGST_Cal - ELE_WL_TDSCal;
                                    }
                                    else
                                    {
                                        GasWTLGST_Cal = 0;
                                        WTLGapCommAmt = ELE_WTL_Gap_CommAmt + GasWTLGST_Cal - ELE_WL_TDSCal;
                                    }

                                }
                                else
                                {
                                    ELE_WL_TDSCal = decimal.Round(((ELE_WTL_Gap_CommAmt - GST_AMount)));
                                    if (MemberGST != null)
                                    {
                                        GasWTLGST_Cal = decimal.Round(((ELE_WTL_Gap_CommAmt - gst_VAlue)));
                                        WTLGapCommAmt = ELE_WTL_Gap_CommAmt + GasWTLGST_Cal - ELE_WL_TDSCal;
                                    }
                                    else
                                    {
                                        GasWTLGST_Cal = 0;
                                        WTLGapCommAmt = ELE_WTL_Gap_CommAmt + GasWTLGST_Cal - ELE_WL_TDSCal;
                                    }

                                }

                            }
                            decimal CommWLAddClosing = tbl_accountWhiteLevel.CLOSING + WTLGapCommAmt;
                            long WL_ID = long.Parse(WHT_MEM_ID.ToString());
                            TBL_ACCOUNTS objWLCommPer = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = WL_ID,
                                MEMBER_TYPE = WLmembtype.roleName,
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "CR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Commission",
                                OPENING = WLClosingAmt,
                                CLOSING = CommWLAddClosing,
                                REC_NO = 0,
                                COMM_AMT = WTLGapCommAmt,
                                GST = (float)GasWTLGST_Cal,
                                TDS = (float)ELE_WL_TDSCal,
                                IPAddress = IpAddress,
                                TDS_PERCENTAGE = taxMaster.TAX_VALUE,
                                GST_PERCENTAGE = GST_Master.TAX_VALUE,
                                WHITELEVEL_ID = (long)WHT_MEM_ID,
                                //SUPER_ID = (long)SUP_MEM_ID,
                                SUPER_ID = 0,
                                DISTRIBUTOR_ID = (long)DIS_MEM_ID
                            };
                            db.TBL_ACCOUNTS.Add(objWLCommPer);
                            db.SaveChanges();
                        }
                        #endregion
                        ContextTransaction.Commit();
                        return true;
                    }
                    else
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    if (status == "DMR")
                    {
                        var WLTransactionChargeAmt = db.TBL_DMR_TRANSACTION_LOGS.FirstOrDefault(x => x.CORELATIONID == sRandomOTP);
                        WLTransactionChargeAmt.ERROR_TYPE = "ERROR Occured";
                        db.Entry(WLTransactionChargeAmt).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    return false;
                    Logger.Error("Commission Distribution:-   AllMemberCommissionDistribution", ex);
                    ContextTransaction.Rollback();
                    //throw ex;
                }
            }

        }
        #endregion

        #region Deduct Merchant WalletAmount
        public async Task<bool> DeductAmountFromMerchant(long Mem_ID, string status, decimal Trans_Amt, decimal ChargeAmt, decimal OpeningAmt, string serviceprovider, string rechargeType, string IpAddress,string CORelationId,string contactNo,string APIName,string RchrgType)
        {
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    long Oper_ID = 0;
                    var GST_Master = await db.TBL_TAX_MASTERS.Where(x => x.TAX_NAME == "GST").FirstOrDefaultAsync();
                    var taxMaster = await db.TBL_TAX_MASTERS.Where(x => x.TAX_NAME == "TDS").FirstOrDefaultAsync();
                    var operatorID = await db.TBL_SERVICE_PROVIDERS.Where(x => x.SERVICE_NAME == serviceprovider && x.TYPE == RchrgType).FirstOrDefaultAsync();
                    if (operatorID != null)
                    {
                        long.TryParse(operatorID.ID.ToString(),out Oper_ID);
                    }
                    else
                    {
                        Oper_ID = 0;
                    }
                    
                    var DIS_MEM_ID = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == Mem_ID).Select(z => z.INTRODUCER).FirstOrDefaultAsync();
                    //var SUP_MEM_ID = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == DIS_MEM_ID).Select(z => z.INTRODUCER).FirstOrDefaultAsync();
                    var WHT_MEM_ID = await db.TBL_MASTER_MEMBER.Where(x => x.MEM_ID == DIS_MEM_ID).Select(z => z.UNDER_WHITE_LEVEL).FirstOrDefaultAsync();
                    //var tbl_account = await db.TBL_ACCOUNTS.Where(z => z.MEM_ID == Mem_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefaultAsync();
                    var tbl_account = await db.TBL_ACCOUNTS.Where(z => z.MEM_ID == Mem_ID).OrderByDescending(z => z.ACC_NO).FirstOrDefaultAsync();
                    if (tbl_account != null)
                    {
                        #region FIXED SERVICE FEE Calculation
                        decimal TOtalAmountTransfer = 0;
                        decimal FixedSErviceAmt = 0;
                        string DMRGSTPerValue = System.Configuration.ConfigurationManager.AppSettings["DMRGSTPERCENTAGEComm"];
                        string DMRGSTFixedValue = System.Configuration.ConfigurationManager.AppSettings["DMRGSTFixedComm"];
                        string FixedService_Type = "";
                        double MERGSTCAl = 0;
                        double MER_GST_Val = 0;
                        double ValGSTVal = 0;
                        decimal servGSt_amt = 0;
                       
                        if (Trans_Amt < 1000)
                        {
                           
                            decimal.TryParse(DMRGSTFixedValue,out servGSt_amt);
                            FixedSErviceAmt = servGSt_amt;
                            TOtalAmountTransfer = Trans_Amt + FixedSErviceAmt;
                            FixedService_Type = "FIXED";
                        }
                        else
                        {
                                                   
                            decimal.TryParse(DMRGSTPerValue, out servGSt_amt);
                            FixedSErviceAmt = (Trans_Amt * servGSt_amt) / 100;
                            TOtalAmountTransfer = Trans_Amt + FixedSErviceAmt;
                            FixedService_Type = "PERCENTAGE";
                        }

                        //GST CALL
                        //double valuecalGST = 0;
                        //decimal ServiceGSTval = 0;
                        //decimal WL_MER_GSTvalue = 0;
                        //decimal SERVICEWLAMT = 0;
                        //decimal WLCOMM_TDSVal = 0;
                        //decimal WL_NET_COmm = 0;
                        //decimal TAX_Amt = 0;
                        //decimal.TryParse(taxMaster.TAX_VALUE.ToString(), out TAX_Amt);
                        //double.TryParse(FixedSErviceAmt.ToString(), out valuecalGST);
                        //ValGSTVal = 1.18;
                        //MERGSTCAl = (valuecalGST / ValGSTVal);
                        //MER_GST_Val = valuecalGST - MERGSTCAl;
                        
                        //decimal.TryParse(MER_GST_Val.ToString(), out ServiceGSTval);
                        //decimal.TryParse(MERGSTCAl.ToString(), out SERVICEWLAMT);
                        //WLCOMM_TDSVal = ((SERVICEWLAMT * TAX_Amt) / 100);
                        //WL_NET_COmm = SERVICEWLAMT - WLCOMM_TDSVal;
                        //WL_MER_GSTvalue = decimal.Round(ServiceGSTval, 2);
                        #endregion


                        #region for Merchant Calculation Deduct Wallet amount                        
                        decimal ClosingAmt = tbl_account.CLOSING;

                        decimal SubAmt = 0;
                        if (status == "DMR")
                        {
                            SubAmt = ClosingAmt - TOtalAmountTransfer;  
                            //SubAmt = ClosingAmt - Trans_Amt;
                            Trans_Amt = Trans_Amt;
                        }
                        else
                        {
                            Trans_Amt = Trans_Amt;
                            SubAmt = ClosingAmt - Trans_Amt;
                        }
                        TBL_ACCOUNTS objaccnt = new TBL_ACCOUNTS()
                        {
                            API_ID = 0,
                            MEM_ID = Mem_ID,
                            MEMBER_TYPE = "RETAILER",
                            TRANSACTION_TYPE = rechargeType,
                            TRANSACTION_DATE = System.DateTime.Now,
                            TRANSACTION_TIME = DateTime.Now,
                            DR_CR = "DR",
                            AMOUNT = Trans_Amt,
                            NARRATION = "Debit "+status+" Transaction Amount",
                            OPENING = ClosingAmt,
                            CLOSING = SubAmt,
                            REC_NO = 0,
                            COMM_AMT = 0,
                            GST = 0,
                            TDS = 0,
                            IPAddress = IpAddress,
                            TDS_PERCENTAGE = 0,
                            GST_PERCENTAGE = 0,
                            WHITELEVEL_ID = (long)WHT_MEM_ID,
                            //SUPER_ID = (long)SUP_MEM_ID,
                            SUPER_ID = 0,
                            DISTRIBUTOR_ID = (long)DIS_MEM_ID,
                            SERVICE_ID = Oper_ID,
                            CORELATIONID = CORelationId
                        };
                        db.TBL_ACCOUNTS.Add(objaccnt);
                        await db.SaveChangesAsync();
                        #endregion
                        #region For Whitelable Wallet amount deduct  calculation 
                        decimal WLClosing_Amount = 0;
                        decimal WLSUBAmount = 0;
                        //var WhiteLabelWallet= await db.TBL_ACCOUNTS.Where(z => z.MEM_ID == WHT_MEM_ID).OrderByDescending(z => z.TRANSACTION_TIME).FirstOrDefaultAsync();
                        var WhiteLabelWallet = await db.TBL_ACCOUNTS.Where(z => z.MEM_ID == WHT_MEM_ID).OrderByDescending(z => z.ACC_NO).FirstOrDefaultAsync();
                        if (WhiteLabelWallet != null)
                        {
                            WLClosing_Amount = WhiteLabelWallet.CLOSING;

                            if (status == "DMR")
                            {
                                WLSUBAmount = WLClosing_Amount - TOtalAmountTransfer;
                                //WLSUBAmount = WLClosing_Amount - Trans_Amt;
                                Trans_Amt = Trans_Amt;

                            }
                            else
                            {
                                Trans_Amt = Trans_Amt;
                                WLSUBAmount = WLClosing_Amount - Trans_Amt;
                            }
                            long WLID = 0;
                            long.TryParse(WHT_MEM_ID.ToString(), out WLID);
                            TBL_ACCOUNTS objWLvalue = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = WLID,
                                MEMBER_TYPE = "WHITE LEVEL",
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "DR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Debit " + status + " Transaction Amount",
                                OPENING = WLClosing_Amount,
                                CLOSING = WLSUBAmount,
                                REC_NO = 0,
                                COMM_AMT = 0,
                                GST = 0,
                                TDS = 0,
                                IPAddress = IpAddress,
                                TDS_PERCENTAGE = 0,
                                GST_PERCENTAGE = 0,
                                WHITELEVEL_ID = (long)WHT_MEM_ID,
                                //SUPER_ID = (long)SUP_MEM_ID,
                                SUPER_ID = 0,
                                DISTRIBUTOR_ID = (long)DIS_MEM_ID,
                                SERVICE_ID = Oper_ID,
                                CORELATIONID = CORelationId
                            };
                            db.TBL_ACCOUNTS.Add(objWLvalue);
                            await db.SaveChangesAsync();

                            decimal WLBalance = 0;
                            decimal WLSUB_AMT = 0;
                            var WLbalanceUpdate = await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == WHT_MEM_ID);
                            if (WLbalanceUpdate != null)
                            {
                                decimal.TryParse(WLbalanceUpdate.BALANCE.ToString(), out WLBalance);
                                if (status == "DMR")
                                {
                                    WLSUB_AMT = WLBalance - TOtalAmountTransfer;
                                    //WLSUB_AMT = WLBalance - Trans_Amt;
                                }
                                else
                                {
                                    WLSUB_AMT = WLBalance - Trans_Amt;
                                }

                                WLbalanceUpdate.BALANCE = WLSUB_AMT;
                                db.Entry(WLbalanceUpdate).State = System.Data.Entity.EntityState.Modified;
                                await db.SaveChangesAsync();
                            }
                            else
                            {
                                WLbalanceUpdate.BALANCE = Trans_Amt;
                                db.Entry(WLbalanceUpdate).State = System.Data.Entity.EntityState.Modified;
                                await db.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            WLClosing_Amount = 0;
                            var WLbalanceUpdate_Baln = await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == WHT_MEM_ID);
                            if (status == "DMR")
                            {
                                WLSUBAmount = Convert.ToDecimal(WLbalanceUpdate_Baln.BALANCE)- TOtalAmountTransfer;
                                //WLSUBAmount = WLClosing_Amount - Trans_Amt;
                                Trans_Amt = Trans_Amt;

                            }
                            else
                            {
                                Trans_Amt = Trans_Amt;
                                WLSUBAmount =  Trans_Amt;
                            }
                            long WLID = 0;
                            long.TryParse(WHT_MEM_ID.ToString(), out WLID);
                            TBL_ACCOUNTS objWLvalue = new TBL_ACCOUNTS()
                            {
                                API_ID = 0,
                                MEM_ID = WLID,
                                MEMBER_TYPE = "WHITE LEVEL",
                                TRANSACTION_TYPE = rechargeType,
                                TRANSACTION_DATE = System.DateTime.Now,
                                TRANSACTION_TIME = DateTime.Now,
                                DR_CR = "DR",
                                AMOUNT = Trans_Amt,
                                NARRATION = "Debit " + status + " Transaction Amount",
                                OPENING = WLClosing_Amount,
                                CLOSING = WLSUBAmount,
                                REC_NO = 0,
                                COMM_AMT = 0,
                                GST = 0,
                                TDS = 0,
                                IPAddress = IpAddress,
                                TDS_PERCENTAGE = 0,
                                GST_PERCENTAGE = 0,
                                WHITELEVEL_ID = (long)WHT_MEM_ID,
                                //SUPER_ID = (long)SUP_MEM_ID,
                                SUPER_ID = 0,
                                DISTRIBUTOR_ID = (long)DIS_MEM_ID,
                                SERVICE_ID = Oper_ID,
                                CORELATIONID = CORelationId
                            };
                            db.TBL_ACCOUNTS.Add(objWLvalue);
                            await db.SaveChangesAsync();

                            decimal WLBalance = 0;
                            decimal WLSUB_AMT = 0;
                            var WLbalanceUpdate = await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == WHT_MEM_ID);
                            if (WLbalanceUpdate != null)
                            {
                                decimal.TryParse(WLbalanceUpdate.BALANCE.ToString(), out WLBalance);
                                if (status == "DMR")
                                {
                                    WLSUB_AMT = WLBalance - TOtalAmountTransfer;
                                    //WLSUB_AMT = WLBalance - Trans_Amt;
                                }
                                else
                                {
                                    WLSUB_AMT = WLBalance - Trans_Amt;
                                }

                                WLbalanceUpdate.BALANCE = WLSUB_AMT;
                                db.Entry(WLbalanceUpdate).State = System.Data.Entity.EntityState.Modified;
                                await db.SaveChangesAsync();
                            }
                            else
                            {
                                WLbalanceUpdate.BALANCE = Trans_Amt;
                                db.Entry(WLbalanceUpdate).State = System.Data.Entity.EntityState.Modified;
                                await db.SaveChangesAsync();
                            }
                        }

                        #endregion  
                        decimal MemBalance = 0;
                        decimal UpdateBalance = 0;
                        var MerchantbalanceUpdate = await db.TBL_MASTER_MEMBER.FirstOrDefaultAsync(x => x.MEM_ID == Mem_ID);
                        if (MerchantbalanceUpdate != null)
                        {
                            decimal.TryParse(MerchantbalanceUpdate.BALANCE.ToString(), out MemBalance);
                            if (status == "DMR")
                            {
                                UpdateBalance = MemBalance - TOtalAmountTransfer;
                            }
                            else
                            {
                                UpdateBalance = MemBalance - Trans_Amt;
                            }
                            MerchantbalanceUpdate.BALANCE = UpdateBalance;
                            db.Entry(MerchantbalanceUpdate).State = System.Data.Entity.EntityState.Modified;
                            await db.SaveChangesAsync();
                        }
                        else
                        {
                            MerchantbalanceUpdate.BALANCE = MemBalance;
                            db.Entry(MerchantbalanceUpdate).State = System.Data.Entity.EntityState.Modified;
                            await db.SaveChangesAsync();
                        }

                        if (status != "DMR")
                        {
                            TBL_INSTANTPAY_RECHARGE_RESPONSE objinst = new TBL_INSTANTPAY_RECHARGE_RESPONSE()
                            {
                                Ipay_Id = "",
                                AgentId = "",
                                Opr_Id = serviceprovider,
                                AccountNo = contactNo,
                                Sp_Key = serviceprovider,
                                Trans_Amt = Trans_Amt,
                                Charged_Amt = 0,
                                Opening_Balance = 0,
                                DateVal = DateTime.Now,
                                Status = "",
                                Res_Code = "",
                                res_msg = "",
                                Mem_ID = Mem_ID,
                                RechargeType = rechargeType,
                                IpAddress = IpAddress,
                                API_Name = APIName,
                                RechargeResponse = "",
                                REC_COMM_TYPE = "",
                                MER_COMM_VALUE = 0,
                                MER_COMM_AMT = 0,
                                MER_TDS_DR_COMM_AMT = 0,
                                DIST_ID = (long)DIS_MEM_ID,
                                DIST_COMM_VALUE = 0,
                                DIST_COMM_AMT = 0,
                                DIST_TDS_DR_COMM_AMT = 0,
                                //SUPER_ID = (long)SUP_MEM_ID,
                                SUPER_ID = 0,
                                SUPER_COMM_VALUE = 0,
                                SUPER_COMM_AMT = 0,
                                SUPER_TDS_DR_COMM_AMT = 0,
                                WHITELABEL_ID = (long)WHT_MEM_ID,
                                WHITELABEL_VALUE = 0,
                                WHITELABEL_COMM_AMT = 0,
                                WHITELABEL_TDS_DR_COMM_AMT = 0,
                                TDS_RATE = 0,
                                CORELATIONID = CORelationId,
                                ERROR_TYPE = "Pending",
                                ISREVERSE = "No",
                                DOMAIN_NAME = IpAddress,
                                ISCOMMISSIONDISBURSE = "No",
                                COMMISSIONDISBURSEDATE = DateTime.Now,
                                GST_RATE = GST_Master.TAX_VALUE,
                                MER_COMM_GST_AMT = 0,
                                DIST_COMM_GST_AMT = 0,
                                SUPER_COMM_GST_AMT = 0,
                                WHITELABEL_COMM_GST_AMT = 0,
                                MER_INVOICE_ID = 0,
                                MER_CANCEL_INVOICE = "",
                                DIST_INVOICE_ID = 0,
                                DIST_CANCEL_INVOICE = "",
                                SUPER_INVOICE_ID = 0,
                                SUPER_CANCEL_INVOICE = "",
                                WHITELABEL_INVOICE_ID = 0,
                                WHITELABEL_CANCEL_INVOICE = "",
                                SLAB_ID = 0
                            };
                            db.TBL_INSTANTPAY_RECHARGE_RESPONSE.Add(objinst);
                            await db.SaveChangesAsync();
                        }
                        else
                        {
                            //decimal MER_GSTval = (ChargeAmt*1) / 100;
                            TBL_DMR_TRANSACTION_LOGS objDMRLog = new TBL_DMR_TRANSACTION_LOGS()
                            {
                                CUSTOMER_ID = APIName,
                                RECIPIENT_ID = contactNo,
                                CLIENT_REF_ID = "",
                                RES_FEE = 0,
                                INITIATOR_ID = "",
                                ACCOUNT_NO = "",
                                API_TXN_STATUS = "",
                                NAME = "",
                                IFSC_CODE = "",
                                IMPS_RES_CODE = "",
                                IMPS_RES_MSG = "",
                                TXN_ID = "",
                                TIMESTAMP = "",
                                SENDER_NAME = "",
                                SENDER_MOBILE_NO = "",
                                TRANSACTION_DATE = DateTime.Now,
                                TRANSACTION_STATUS = "Pending",
                                TRANSFER_AMT = Trans_Amt,
                                MER_ID = Mem_ID,
                                MER_WLP_COMM_TYPE = "",
                                MER_WLP_COMM_RATE = 0,
                                MER_TRANSFER_FEE_TYPE = null,
                                MER_TRANSFER_FEE_RATE = 0,
                                MER_TRANSFER_FEE = 0,
                                MER_GST_INPUT = 0,
                                MER_GST_OUTPUT = 0,
                                MER_GROSS_COMM_AMT = 0,
                                MER_TDS_DR_COMM_AMT = 0,
                                MER_NET_COMM = 0,
                                DIST_ID = (long)DIS_MEM_ID,
                                DIST_WLP_COMM_TYPE = "",
                                DIST_WLP_COMM_RATE = 0,
                                DIST_GROSS_COMM_AMT = 0,
                                DIST_TDS_DR_COMM_AMT = 0,
                                DIST_NET_COMM = 0,
                                DIST_GST_INPUT = 0,
                                DIST_GST_OUTPUT = 0,
                                //SUPER_ID = (long)SUP_MEM_ID,
                                SUPER_ID = 0,
                                SPR_WLP_COMM_TYPE = "",
                                SPR_WLP_COMM_RATE = 0,
                                SPR_GROSS_COMM_AMT = 0,
                                SPR_TDS_DR_COMM_AMT = 0,
                                SPR_NET_COMM = 0,
                                SPR_GST_INPUT = 0,
                                SPR_GST_OUTPUT = 0,
                                WLP_ID = (long)WHT_MEM_ID,
                                WLP_PA_TRAN_RATE_TYPE = "",
                                WLP_PA_TRAN_RATE = 0,
                                WLP_TRANSFER_FEE_TYPE = null,
                                WLP_TRANSFER_FEE_RATE = 0,
                                WLP_TRANSFER_FEE = 0,
                                WLP_GST_INPUT = 0,
                                WLP_GST_OUTPUT = 0,
                                WLP_GROSS_COMM_AMT = 0,
                                WLP_TDS_DR_COMM_AMT = 0,
                                WLP_NET_COMM = 0,
                                TDS_RATE = taxMaster.TAX_VALUE,
                                GST_RATE= GST_Master.TAX_VALUE,
                                CORELATIONID=CORelationId,
                                ERROR_TYPE=null,
                                ISREVERSE=false,
                                DOMAIN_NAME=IpAddress,
                                ISCOMMISSIONDISBURSE=false,
                                //COMMISSIONDISBURSEDATE=DateTime.Now,
                                //MER_INVOICE_ID=0,
                                //MER_CANCEL_INVOICE ="",
                                //DIST_INVOICE_ID = 0,
                                //DIST_CANCEL_INVOICE = "",
                                //SUPER_INVOICE_ID = 0,
                                //SUPER_CANCEL_INVOICE="",
                                //WHITELABEL_INVOICE_ID=0,
                                //WHITELABEL_CANCEL_INVOICE="",
                                SLAB_ID=0
                            };
                            db.TBL_DMR_TRANSACTION_LOGS.Add(objDMRLog);
                            await db.SaveChangesAsync();
                        }                        
                        ContextTransaction.Commit();
                        return true;
                    }
                    else
                    {
                        ContextTransaction.Rollback();
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                    ContextTransaction.Rollback();
                    throw;
                }
            }
        }
        #endregion

        #region if Recharge Fail Then deduct amount from wallet Merchant, Distributor, Super, WhiteLabel

        public string RefundCommissionInWallet(string CoRelationId,string OperationType, string APIRESPONSE=null)        
        {
            var db = new DBContext();
            using (System.Data.Entity.DbContextTransaction ContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (OperationType != "DMR")
                    {
                        var RchgWallet = db.TBL_INSTANTPAY_RECHARGE_RESPONSE.FirstOrDefault(x => x.CORELATIONID == CoRelationId);
                        #region Deduct Merchant and Whitelabel Wallet Amount
                        decimal MerOpen_Wal = 0.00M;
                        decimal MerClosing_Wal = 0.00M;
                        var MerWallet = db.TBL_ACCOUNTS.FirstOrDefault(x => x.CORELATIONID == CoRelationId && x.MEMBER_TYPE == "RETAILER");
                        MerOpen_Wal = MerWallet.CLOSING;
                        MerClosing_Wal = MerWallet.CLOSING + MerWallet.AMOUNT;
                        TBL_ACCOUNTS MerObjWal = new TBL_ACCOUNTS()
                        {
                            API_ID = 0,
                            MEM_ID = (long)MerWallet.MEM_ID,
                            MEMBER_TYPE = MerWallet.MEMBER_TYPE,
                            TRANSACTION_TYPE = MerWallet.TRANSACTION_TYPE,
                            TRANSACTION_DATE = System.DateTime.Now,
                            TRANSACTION_TIME = DateTime.Now,
                            DR_CR = "CR",
                            AMOUNT = MerWallet.AMOUNT,
                            NARRATION = "Transaction Failed. Transaction amount " + MerWallet.AMOUNT + " is created against Ref No. " + CoRelationId,
                            OPENING = MerOpen_Wal,
                            CLOSING = MerClosing_Wal,
                            REC_NO = 0,
                            COMM_AMT = 0,
                            GST = MerWallet.GST,
                            TDS = MerWallet.TDS,
                            IPAddress = MerWallet.IPAddress,
                            TDS_PERCENTAGE = 0,
                            GST_PERCENTAGE = 0,
                            WHITELEVEL_ID = MerWallet.WHITELEVEL_ID,
                            SUPER_ID = MerWallet.SUPER_ID,
                            DISTRIBUTOR_ID = MerWallet.DISTRIBUTOR_ID,
                            SERVICE_ID = MerWallet.SERVICE_ID,
                            CORELATIONID = CoRelationId,
                            REC_COMM_TYPE = "",
                            COMM_VALUE = 0,
                            NET_COMM_AMT = 0,
                            TDS_DR_COMM_AMT = 0,
                            CGST_COMM_AMT_INPUT = 0,
                            CGST_COMM_AMT_OUTPUT = 0,
                            SGST_COMM_AMT_INPUT = 0,
                            SGST_COMM_AMT_OUTPUT = 0,
                            IGST_COMM_AMT_INPUT = 0,
                            IGST_COMM_AMT_OUTPUT = 0,
                            TOTAL_GST_COMM_AMT_INPUT = 0,
                            TOTAL_GST_COMM_AMT_OUTPUT = 0,
                            TDS_RATE = 0,
                            CGST_RATE = 0,
                            SGST_RATE = 0,
                            IGST_RATE = 0,
                            TOTAL_GST_RATE =0,
                            COMM_SLAB_ID = 0,
                            STATE_ID = 0,
                            FLAG1 = 0,
                            FLAG2 = 0,
                            FLAG3 = 0,
                            FLAG4 = 0,
                            FLAG5 = 0,
                            FLAG6 = 0,
                            FLAG7 = 0,
                            FLAG8 = 0,
                            FLAG9 = 0,
                            FLAG10 = 0
                        };
                        db.TBL_ACCOUNTS.Add(MerObjWal);
                        db.SaveChanges();
                        decimal MerUpBal = 0.00M;
                        var merBalupdate = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == MerWallet.MEM_ID);
                        MerUpBal = Convert.ToDecimal(merBalupdate.BALANCE) + MerWallet.AMOUNT;
                        merBalupdate.BALANCE = MerUpBal;
                        db.Entry(merBalupdate).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        decimal WLClosing = 0.00M;
                        decimal WLOpening = 0.00M;
                        var WHTLBWallet = db.TBL_ACCOUNTS.FirstOrDefault(x => x.CORELATIONID == CoRelationId && x.MEMBER_TYPE == "WHITE LEVEL");
                        WLOpening = WHTLBWallet.CLOSING;
                        WLClosing = WHTLBWallet.CLOSING + WHTLBWallet.AMOUNT;
                        TBL_ACCOUNTS WLBObjWal = new TBL_ACCOUNTS()
                        {
                            API_ID = 0,
                            MEM_ID = (long)WHTLBWallet.MEM_ID,
                            MEMBER_TYPE = WHTLBWallet.MEMBER_TYPE,
                            TRANSACTION_TYPE = WHTLBWallet.TRANSACTION_TYPE,
                            TRANSACTION_DATE = System.DateTime.Now,
                            TRANSACTION_TIME = DateTime.Now,
                            DR_CR = "CR",
                            AMOUNT = WHTLBWallet.AMOUNT,
                            NARRATION = "Transaction Failed. Transaction amount "+ WHTLBWallet.AMOUNT + " is created against Ref No. " + CoRelationId,
                            OPENING = WLOpening,
                            CLOSING = WLClosing,
                            REC_NO = 0,
                            COMM_AMT = 0,
                            GST = WHTLBWallet.GST,
                            TDS = WHTLBWallet.TDS,
                            IPAddress = WHTLBWallet.IPAddress,
                            TDS_PERCENTAGE = 0,
                            GST_PERCENTAGE = 0,
                            WHITELEVEL_ID = WHTLBWallet.WHITELEVEL_ID,
                            SUPER_ID = WHTLBWallet.SUPER_ID,
                            DISTRIBUTOR_ID = WHTLBWallet.DISTRIBUTOR_ID,
                            SERVICE_ID = WHTLBWallet.SERVICE_ID,
                            CORELATIONID = CoRelationId,
                            REC_COMM_TYPE = "",
                            COMM_VALUE = 0,
                            NET_COMM_AMT = 0,
                            TDS_DR_COMM_AMT = 0,
                            CGST_COMM_AMT_INPUT = 0,
                            CGST_COMM_AMT_OUTPUT = 0,
                            SGST_COMM_AMT_INPUT = 0,
                            SGST_COMM_AMT_OUTPUT = 0,
                            IGST_COMM_AMT_INPUT = 0,
                            IGST_COMM_AMT_OUTPUT = 0,
                            TOTAL_GST_COMM_AMT_INPUT = 0,
                            TOTAL_GST_COMM_AMT_OUTPUT = 0,
                            TDS_RATE = 0,
                            CGST_RATE = 0,
                            SGST_RATE = 0,
                            IGST_RATE = 0,
                            TOTAL_GST_RATE = 0,
                            COMM_SLAB_ID = 0,
                            STATE_ID = 0,
                            FLAG1 = 0,
                            FLAG2 = 0,
                            FLAG3 = 0,
                            FLAG4 = 0,
                            FLAG5 = 0,
                            FLAG6 = 0,
                            FLAG7 = 0,
                            FLAG8 = 0,
                            FLAG9 = 0,
                            FLAG10 = 0
                        };
                        db.TBL_ACCOUNTS.Add(WLBObjWal);
                        db.SaveChanges();
                        decimal BALWLUp = 0.00M;
                        decimal UpWLBAl = 0.00M;
                        var WLBalUpdate = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == WHTLBWallet.MEM_ID);
                        decimal.TryParse(WLBalUpdate.BALANCE.ToString(), out BALWLUp);
                        UpWLBAl = BALWLUp + WHTLBWallet.AMOUNT;
                        WLBalUpdate.BALANCE = UpWLBAl;
                        db.Entry(WLBalUpdate).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        RchgWallet.ERROR_TYPE = "FAILED";
                        RchgWallet.ISREVERSE = "Yes";
                        RchgWallet.ISCOMMISSIONDISBURSE = "No";
                        RchgWallet.REVERSE_DATE = DateTime.Now;
                        db.Entry(RchgWallet).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        #endregion
                    }
                    else
                    {
                        decimal TOtalAmountTransfer = 0;
                        decimal FixedSErviceAmt = 0;
                        double MERGSTCAl = 0;
                        double MER_GST_Val = 0;
                        double ValGSTVal = 0;
                        decimal servGSt_amt = 0;
                        string DMRGSTPerValue = System.Configuration.ConfigurationManager.AppSettings["DMRGSTPERCENTAGEComm"];
                        string DMRGSTFixedValue = System.Configuration.ConfigurationManager.AppSettings["DMRGSTFixedComm"];

                        var RchgWallet = db.TBL_DMR_TRANSACTION_LOGS.FirstOrDefault(x => x.CORELATIONID == CoRelationId);
                        #region Deduct Merchant and Whitelabel Wallet Amount
                        decimal MerOpen_Wal = 0.00M;
                        decimal MerClosing_Wal = 0.00M;
                        var MerWallet = db.TBL_ACCOUNTS.FirstOrDefault(x => x.CORELATIONID == CoRelationId && x.MEMBER_TYPE == "RETAILER");

                        if (MerWallet.AMOUNT < 1000)
                        {

                            decimal.TryParse(DMRGSTFixedValue, out servGSt_amt);
                            FixedSErviceAmt = servGSt_amt;
                            TOtalAmountTransfer = MerWallet.AMOUNT + FixedSErviceAmt;
                            //FixedService_Type = "FIXED";
                        }
                        else
                        {

                            decimal.TryParse(DMRGSTPerValue, out servGSt_amt);
                            FixedSErviceAmt = (MerWallet.AMOUNT * servGSt_amt) / 100;
                            TOtalAmountTransfer = MerWallet.AMOUNT + FixedSErviceAmt;
                            //FixedService_Type = "PERCENTAGE";
                        }


                        MerOpen_Wal = MerWallet.CLOSING;
                        //MerClosing_Wal = MerWallet.CLOSING + MerWallet.AMOUNT;

                        MerClosing_Wal = MerWallet.CLOSING + TOtalAmountTransfer;



                        TBL_ACCOUNTS MerObjWal = new TBL_ACCOUNTS()
                        {
                            API_ID = 0,
                            MEM_ID = (long)MerWallet.MEM_ID,
                            MEMBER_TYPE = MerWallet.MEMBER_TYPE,
                            TRANSACTION_TYPE = MerWallet.TRANSACTION_TYPE,
                            TRANSACTION_DATE = System.DateTime.Now,
                            TRANSACTION_TIME = DateTime.Now,
                            DR_CR = "CR",
                            AMOUNT = MerWallet.AMOUNT,
                            NARRATION = "Transaction Failed. Transaction amount " + MerWallet.AMOUNT + " is created against Ref No. " + CoRelationId,
                            OPENING = MerOpen_Wal,
                            CLOSING = MerClosing_Wal,
                            REC_NO = 0,
                            COMM_AMT = 0,
                            GST = MerWallet.GST,
                            TDS = MerWallet.TDS,
                            IPAddress = MerWallet.IPAddress,
                            TDS_PERCENTAGE = 0,
                            GST_PERCENTAGE = 0,
                            WHITELEVEL_ID = MerWallet.WHITELEVEL_ID,
                            SUPER_ID = MerWallet.SUPER_ID,
                            DISTRIBUTOR_ID = MerWallet.DISTRIBUTOR_ID,
                            SERVICE_ID = MerWallet.SERVICE_ID,
                            CORELATIONID = CoRelationId,
                            REC_COMM_TYPE = "",
                            COMM_VALUE = 0,
                            NET_COMM_AMT = 0,
                            TDS_DR_COMM_AMT = 0,
                            CGST_COMM_AMT_INPUT = 0,
                            CGST_COMM_AMT_OUTPUT = 0,
                            SGST_COMM_AMT_INPUT = 0,
                            SGST_COMM_AMT_OUTPUT = 0,
                            IGST_COMM_AMT_INPUT = 0,
                            IGST_COMM_AMT_OUTPUT = 0,
                            TOTAL_GST_COMM_AMT_INPUT = 0,
                            TOTAL_GST_COMM_AMT_OUTPUT = 0,
                            TDS_RATE = 0,
                            CGST_RATE = 0,
                            SGST_RATE = 0,
                            IGST_RATE = 0,
                            TOTAL_GST_RATE = 0,
                            COMM_SLAB_ID = 0,
                            STATE_ID = 0,
                            FLAG1 = 0,
                            FLAG2 = 0,
                            FLAG3 = 0,
                            FLAG4 = 0,
                            FLAG5 = 0,
                            FLAG6 = 0,
                            FLAG7 = 0,
                            FLAG8 = 0,
                            FLAG9 = 0,
                            FLAG10 = 0
                        };
                        db.TBL_ACCOUNTS.Add(MerObjWal);
                        db.SaveChanges();
                        decimal MerUpBal = 0.00M;
                        var merBalupdate = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == MerWallet.MEM_ID);
                        //MerUpBal = Convert.ToDecimal(merBalupdate.BALANCE) + MerWallet.AMOUNT;
                        MerUpBal = Convert.ToDecimal(merBalupdate.BALANCE) + TOtalAmountTransfer;
                        merBalupdate.BALANCE = MerUpBal;
                        db.Entry(merBalupdate).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        decimal WLClosing = 0.00M;
                        decimal WLOpening = 0.00M;
                        string WLPTransytpe = "";
                        var WHTLBWallet = db.TBL_ACCOUNTS.FirstOrDefault(x => x.CORELATIONID == CoRelationId && x.MEMBER_TYPE == "WHITE LEVEL");
                        if (WHTLBWallet == null)
                        {
                            WLOpening = 0;
                            WLClosing = 0;
                            WLPTransytpe = MerWallet.TRANSACTION_TYPE;
                        }
                        else
                        {
                            WLOpening = WHTLBWallet.CLOSING;
                            WLClosing = WHTLBWallet.CLOSING + WHTLBWallet.AMOUNT;
                            WLPTransytpe = WHTLBWallet.TRANSACTION_TYPE;
                        }
                        
                        TBL_ACCOUNTS WLBObjWal = new TBL_ACCOUNTS()
                        {
                            API_ID = 0,
                            MEM_ID = (long)merBalupdate.UNDER_WHITE_LEVEL,
                            MEMBER_TYPE = "WHITE LEVEL",
                            TRANSACTION_TYPE = WLPTransytpe,
                            TRANSACTION_DATE = System.DateTime.Now,
                            TRANSACTION_TIME = DateTime.Now,
                            DR_CR = "CR",
                            AMOUNT = WHTLBWallet.AMOUNT,
                            NARRATION = "Transaction Failed. Transaction amount " + WHTLBWallet.AMOUNT + " is created against Ref No. " + CoRelationId,
                            OPENING = WLOpening,
                            CLOSING = WLClosing,
                            REC_NO = 0,
                            COMM_AMT = 0,
                            GST = WHTLBWallet.GST,
                            TDS = WHTLBWallet.TDS,
                            IPAddress = WHTLBWallet.IPAddress,
                            TDS_PERCENTAGE = 0,
                            GST_PERCENTAGE = 0,
                            WHITELEVEL_ID = WHTLBWallet.WHITELEVEL_ID,
                            SUPER_ID = WHTLBWallet.SUPER_ID,
                            DISTRIBUTOR_ID = WHTLBWallet.DISTRIBUTOR_ID,
                            SERVICE_ID = WHTLBWallet.SERVICE_ID,
                            CORELATIONID = CoRelationId,
                            REC_COMM_TYPE = "",
                            COMM_VALUE = 0,
                            NET_COMM_AMT = 0,
                            TDS_DR_COMM_AMT = 0,
                            CGST_COMM_AMT_INPUT = 0,
                            CGST_COMM_AMT_OUTPUT = 0,
                            SGST_COMM_AMT_INPUT = 0,
                            SGST_COMM_AMT_OUTPUT = 0,
                            IGST_COMM_AMT_INPUT = 0,
                            IGST_COMM_AMT_OUTPUT = 0,
                            TOTAL_GST_COMM_AMT_INPUT = 0,
                            TOTAL_GST_COMM_AMT_OUTPUT = 0,
                            TDS_RATE = 0,
                            CGST_RATE = 0,
                            SGST_RATE = 0,
                            IGST_RATE = 0,
                            TOTAL_GST_RATE = 0,
                            COMM_SLAB_ID = 0,
                            STATE_ID = 0,
                            FLAG1 = 0,
                            FLAG2 = 0,
                            FLAG3 = 0,
                            FLAG4 = 0,
                            FLAG5 = 0,
                            FLAG6 = 0,
                            FLAG7 = 0,
                            FLAG8 = 0,
                            FLAG9 = 0,
                            FLAG10 = 0
                        };
                        db.TBL_ACCOUNTS.Add(WLBObjWal);
                        db.SaveChanges();
                        decimal BALWLUp = 0.00M;
                        decimal UpWLBAl = 0.00M;
                        var WLBalUpdate = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == WHTLBWallet.MEM_ID);
                        decimal.TryParse(WLBalUpdate.BALANCE.ToString(), out BALWLUp);
                        UpWLBAl = BALWLUp + WHTLBWallet.AMOUNT;
                        WLBalUpdate.BALANCE = UpWLBAl;
                        db.Entry(WLBalUpdate).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        RchgWallet.ERROR_TYPE = "FAILED";
                        RchgWallet.ISREVERSE = true;
                        RchgWallet.ISCOMMISSIONDISBURSE =false;
                        RchgWallet.API_RESPONSE = APIRESPONSE;
                        db.Entry(RchgWallet).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        #endregion
                    }


                    //#region Merchant Wallet Amt Deduct
                    //decimal clsamt = 0.00M;
                    //decimal Opnamt = 0.00M;
                    //var MerWalletAmt = db.TBL_ACCOUNTS.FirstOrDefault(x => x.MEM_ID == RchgWallet.Mem_ID && x.CORELATIONID==CoRelationId);
                    //if (MerWalletAmt != null)
                    //{
                    //    clsamt = MerWalletAmt.CLOSING - RchgWallet.MER_COMM_AMT;
                    //    Opnamt = MerWalletAmt.OPENING + RchgWallet.MER_COMM_AMT;
                    //}
                    //var MerBalan = db.TBL_MASTER_MEMBER.FirstOrDefault(x=>x.MEM_ID== RchgWallet.Mem_ID);
                    //MerBalan.BALANCE = MerBalan.BALANCE + RchgWallet.MER_COMM_AMT;
                    //db.Entry(MerBalan).State = System.Data.Entity.EntityState.Modified;
                    //db.SaveChanges();
                    //TBL_ACCOUNTS objMerWal = new TBL_ACCOUNTS()
                    //{
                    //    API_ID = 0,
                    //    MEM_ID = RchgWallet.Mem_ID,
                    //    MEMBER_TYPE = MerWalletAmt.MEMBER_TYPE,
                    //    TRANSACTION_TYPE = MerWalletAmt.TRANSACTION_TYPE,
                    //    TRANSACTION_DATE = System.DateTime.Now,
                    //    TRANSACTION_TIME = DateTime.Now,
                    //    DR_CR = "DR",
                    //    AMOUNT = MerWalletAmt.AMOUNT,
                    //    NARRATION = "Commission",
                    //    OPENING = Opnamt,
                    //    CLOSING = clsamt,
                    //    REC_NO = 0,
                    //    COMM_AMT = RchgWallet.MER_COMM_AMT,
                    //    GST = MerWalletAmt.GST,
                    //    TDS = MerWalletAmt.TDS,
                    //    IPAddress = MerWalletAmt.IPAddress,
                    //    TDS_PERCENTAGE = MerWalletAmt.TDS_PERCENTAGE,
                    //    GST_PERCENTAGE = MerWalletAmt.GST_PERCENTAGE,
                    //    WHITELEVEL_ID = MerWalletAmt.WHITELEVEL_ID,
                    //    SUPER_ID = MerWalletAmt.SUPER_ID,
                    //    DISTRIBUTOR_ID = MerWalletAmt.DISTRIBUTOR_ID,
                    //    SERVICE_ID = MerWalletAmt.SERVICE_ID,
                    //    CORELATIONID = CoRelationId
                    //};
                    //db.TBL_ACCOUNTS.Add(objMerWal);
                    //db.SaveChanges();
                    //#endregion
                    //#region Distributor Wallet Amt Deduct
                    //decimal Distclsamt = 0;
                    //decimal DistOpnamt = 0;
                    //var DistWalletAmt = db.TBL_ACCOUNTS.FirstOrDefault(x => x.MEM_ID == RchgWallet.DIST_ID && x.CORELATIONID == CoRelationId);
                    //if (DistWalletAmt != null)
                    //{
                    //    Distclsamt = DistWalletAmt.CLOSING - RchgWallet.DIST_COMM_AMT;
                    //    DistOpnamt = DistWalletAmt.OPENING + RchgWallet.DIST_COMM_AMT;
                    //}
                    //var DistBalan = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == RchgWallet.DIST_ID);
                    //DistBalan.BALANCE = DistBalan.BALANCE + RchgWallet.DIST_COMM_AMT;
                    //db.Entry(DistBalan).State = System.Data.Entity.EntityState.Modified;
                    //db.SaveChanges();
                    //TBL_ACCOUNTS objDistWal = new TBL_ACCOUNTS()
                    //{
                    //    API_ID = 0,
                    //    MEM_ID = RchgWallet.DIST_ID,
                    //    MEMBER_TYPE = DistWalletAmt.MEMBER_TYPE,
                    //    TRANSACTION_TYPE = DistWalletAmt.TRANSACTION_TYPE,
                    //    TRANSACTION_DATE = System.DateTime.Now,
                    //    TRANSACTION_TIME = DateTime.Now,
                    //    DR_CR = "DR",
                    //    AMOUNT = DistWalletAmt.AMOUNT,
                    //    NARRATION = "Commission",
                    //    OPENING = DistOpnamt,
                    //    CLOSING = Distclsamt,
                    //    REC_NO = 0,
                    //    COMM_AMT = RchgWallet.DIST_COMM_AMT,
                    //    GST = DistWalletAmt.GST,
                    //    TDS = DistWalletAmt.TDS,
                    //    IPAddress = DistWalletAmt.IPAddress,
                    //    TDS_PERCENTAGE = DistWalletAmt.TDS_PERCENTAGE,
                    //    GST_PERCENTAGE = DistWalletAmt.GST_PERCENTAGE,
                    //    WHITELEVEL_ID = DistWalletAmt.WHITELEVEL_ID,
                    //    SUPER_ID = DistWalletAmt.SUPER_ID,
                    //    DISTRIBUTOR_ID = DistWalletAmt.DISTRIBUTOR_ID,
                    //    SERVICE_ID = DistWalletAmt.SERVICE_ID,
                    //    CORELATIONID = CoRelationId
                    //};
                    //db.TBL_ACCOUNTS.Add(objDistWal);
                    //db.SaveChanges();
                    //#endregion
                    //#region Super Wallet AMT Deduct
                    //decimal Superclsamt = 0;
                    //decimal SuperOpnamt = 0;
                    //var SuperWalletAmt = db.TBL_ACCOUNTS.FirstOrDefault(x => x.MEM_ID == RchgWallet.SUPER_ID && x.CORELATIONID == CoRelationId);
                    //if (SuperWalletAmt != null)
                    //{
                    //    Superclsamt = SuperWalletAmt.CLOSING - RchgWallet.SUPER_COMM_AMT;
                    //    SuperOpnamt = SuperWalletAmt.OPENING + RchgWallet.SUPER_COMM_AMT;
                    //}
                    //var SuperBalan = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == RchgWallet.SUPER_ID);
                    //SuperBalan.BALANCE = SuperBalan.BALANCE + RchgWallet.SUPER_COMM_AMT;
                    //db.Entry(SuperBalan).State = System.Data.Entity.EntityState.Modified;
                    //db.SaveChanges();
                    //TBL_ACCOUNTS objSuperWal = new TBL_ACCOUNTS()
                    //{
                    //    API_ID = 0,
                    //    MEM_ID = RchgWallet.SUPER_ID,
                    //    MEMBER_TYPE = SuperWalletAmt.MEMBER_TYPE,
                    //    TRANSACTION_TYPE = SuperWalletAmt.TRANSACTION_TYPE,
                    //    TRANSACTION_DATE = System.DateTime.Now,
                    //    TRANSACTION_TIME = DateTime.Now,
                    //    DR_CR = "DR",
                    //    AMOUNT = SuperWalletAmt.AMOUNT,
                    //    NARRATION = "Commission",
                    //    OPENING = SuperOpnamt,
                    //    CLOSING = Superclsamt,
                    //    REC_NO = 0,
                    //    COMM_AMT = RchgWallet.SUPER_COMM_AMT,
                    //    GST = SuperWalletAmt.GST,
                    //    TDS = SuperWalletAmt.TDS,
                    //    IPAddress = SuperWalletAmt.IPAddress,
                    //    TDS_PERCENTAGE = SuperWalletAmt.TDS_PERCENTAGE,
                    //    GST_PERCENTAGE = SuperWalletAmt.GST_PERCENTAGE,
                    //    WHITELEVEL_ID = SuperWalletAmt.WHITELEVEL_ID,
                    //    SUPER_ID = SuperWalletAmt.SUPER_ID,
                    //    DISTRIBUTOR_ID = SuperWalletAmt.DISTRIBUTOR_ID,
                    //    SERVICE_ID = SuperWalletAmt.SERVICE_ID,
                    //    CORELATIONID = CoRelationId
                    //};
                    //db.TBL_ACCOUNTS.Add(objSuperWal);
                    //db.SaveChanges();
                    //#endregion
                    //#region WhiteLabel Wallet AMT Deduct
                    //decimal WhiteLabelclsamt = 0;
                    //decimal WhiteLabelOpnamt = 0;
                    //var WhiteLabelWalletAmt = db.TBL_ACCOUNTS.FirstOrDefault(x => x.MEM_ID == RchgWallet.WHITELABEL_ID && x.CORELATIONID == CoRelationId);
                    //if (WhiteLabelWalletAmt != null)
                    //{
                    //    WhiteLabelclsamt = WhiteLabelWalletAmt.CLOSING - RchgWallet.WHITELABEL_COMM_AMT;
                    //    WhiteLabelOpnamt = WhiteLabelWalletAmt.OPENING + RchgWallet.WHITELABEL_COMM_AMT;
                    //}
                    //var WhiteLBalan = db.TBL_MASTER_MEMBER.FirstOrDefault(x => x.MEM_ID == RchgWallet.WHITELABEL_ID);
                    //WhiteLBalan.BALANCE = WhiteLBalan.BALANCE + RchgWallet.WHITELABEL_COMM_AMT;
                    //db.Entry(SuperBalan).State = System.Data.Entity.EntityState.Modified;
                    //db.SaveChanges();
                    //TBL_ACCOUNTS objWHITELWal = new TBL_ACCOUNTS()
                    //{
                    //    API_ID = 0,
                    //    MEM_ID = RchgWallet.WHITELABEL_ID,
                    //    MEMBER_TYPE = WhiteLabelWalletAmt.MEMBER_TYPE,
                    //    TRANSACTION_TYPE = WhiteLabelWalletAmt.TRANSACTION_TYPE,
                    //    TRANSACTION_DATE = System.DateTime.Now,
                    //    TRANSACTION_TIME = DateTime.Now,
                    //    DR_CR = "DR",
                    //    AMOUNT = WhiteLabelWalletAmt.AMOUNT,
                    //    NARRATION = "Commission",
                    //    OPENING = WhiteLabelOpnamt,
                    //    CLOSING = WhiteLabelclsamt,
                    //    REC_NO = 0,
                    //    COMM_AMT = RchgWallet.WHITELABEL_ID,
                    //    GST = WhiteLabelWalletAmt.GST,
                    //    TDS = WhiteLabelWalletAmt.TDS,
                    //    IPAddress = WhiteLabelWalletAmt.IPAddress,
                    //    TDS_PERCENTAGE = WhiteLabelWalletAmt.TDS_PERCENTAGE,
                    //    GST_PERCENTAGE = WhiteLabelWalletAmt.GST_PERCENTAGE,
                    //    WHITELEVEL_ID = WhiteLabelWalletAmt.WHITELEVEL_ID,
                    //    SUPER_ID = WhiteLabelWalletAmt.SUPER_ID,
                    //    DISTRIBUTOR_ID = WhiteLabelWalletAmt.DISTRIBUTOR_ID,
                    //    SERVICE_ID = WhiteLabelWalletAmt.SERVICE_ID,
                    //    CORELATIONID = CoRelationId
                    //};
                    //db.TBL_ACCOUNTS.Add(objWHITELWal);
                    //db.SaveChanges();
                    //#endregion

                    
                    ContextTransaction.Commit();
                    return "Return Success";
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    return "ISSUE";
                    throw ex;
                }
            }
        }
        #endregion

    }

}