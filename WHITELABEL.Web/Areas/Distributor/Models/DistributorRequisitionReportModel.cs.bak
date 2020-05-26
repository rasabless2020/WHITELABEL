namespace WHITELABEL.Web.Areas.Distributor.Models
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
    using log4net;


    public class DistributorRequisitionReportModel
    {
        public static List<TBL_BALANCE_TRANSFER_LOGS> GetDistributorMerchantRequisitionReport(string MemberId, string status)
        {
            var db = new DBContext();
            if (status != "" && MemberId != "")
            {
                long mem_id = long.Parse(MemberId);
                
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.FROM_MEMBER == mem_id && x.STATUS==status
                                       select new
                                       {
                                           Touser = "DISTRIBUTOR",
                                           transid = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           REQUEST_TIME = x.REQUEST_TIME,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           STATUS = x.STATUS,
                                           APPROVED_BY = x.APPROVED_BY,
                                           APPROVAL_DATE = x.APPROVAL_DATE,
                                           SLN = x.SLN
                                       }).AsEnumerable().Select(z => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           ToUser = z.Touser,
                                           TransactionID = z.transid,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           REQUEST_TIME = z.REQUEST_TIME,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           STATUS = z.STATUS,
                                           APPROVED_BY = z.APPROVED_BY,
                                           APPROVAL_DATE = z.APPROVAL_DATE,
                                           SLN = z.SLN
                                       }).ToList();
                return transactionlist;
            }
            else if (status == "" && MemberId != "")
            {
                long mem_id = long.Parse(MemberId);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.FROM_MEMBER == mem_id
                                       select new
                                       {
                                           Touser = "DISTRIBUTOR",
                                           transid = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           REQUEST_TIME = x.REQUEST_TIME,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           STATUS = x.STATUS,
                                           APPROVED_BY = x.APPROVED_BY,
                                           APPROVAL_DATE = x.APPROVAL_DATE,
                                           SLN = x.SLN
                                       }).AsEnumerable().Select(z => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           ToUser = z.Touser,
                                           TransactionID = z.transid,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           REQUEST_TIME = z.REQUEST_TIME,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           STATUS = z.STATUS,
                                           APPROVED_BY = z.APPROVED_BY,
                                           APPROVAL_DATE = z.APPROVAL_DATE,
                                           SLN = z.SLN
                                       }).ToList();
                return transactionlist;
            }           
           
                return null;
        }

        public static List<TBL_BALANCE_TRANSFER_LOGS> GetDistributorMerchantRequisitionReportList(string MemberId, string status)
        {
            var db = new DBContext();
            if (status == "" && MemberId != "")
            {
                long mem_id = long.Parse(MemberId);

                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.TO_MEMBER == mem_id 
                                       select new
                                       {
                                           Touser = "DISTRIBUTOR",
                                           transid = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           REQUEST_TIME = x.REQUEST_TIME,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           STATUS = x.STATUS,
                                           APPROVED_BY = x.APPROVED_BY,
                                           APPROVAL_DATE = x.APPROVAL_DATE,
                                           SLN = x.SLN
                                       }).AsEnumerable().Select((z,index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           ToUser = z.Touser,
                                           TransactionID = z.transid,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           REQUEST_TIME = z.REQUEST_TIME,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           STATUS = z.STATUS,
                                           APPROVED_BY = z.APPROVED_BY,
                                           APPROVAL_DATE = z.APPROVAL_DATE,
                                           SLN = z.SLN
                                       }).ToList();
                return transactionlist;
            }
            else if (status != "" && MemberId != "")
            {
                long mem_id = long.Parse(MemberId);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.TO_MEMBER == mem_id && x.STATUS==status
                                       select new
                                       {
                                           Touser = "DISTRIBUTOR",
                                           transid = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           REQUEST_TIME = x.REQUEST_TIME,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           STATUS = x.STATUS,
                                           APPROVED_BY = x.APPROVED_BY,
                                           APPROVAL_DATE = x.APPROVAL_DATE,
                                           SLN = x.SLN
                                       }).AsEnumerable().Select((z,index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           ToUser = z.Touser,
                                           TransactionID = z.transid,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           REQUEST_TIME = z.REQUEST_TIME,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           STATUS = z.STATUS,
                                           APPROVED_BY = z.APPROVED_BY,
                                           APPROVAL_DATE = z.APPROVAL_DATE,
                                           SLN = z.SLN
                                       }).ToList();
                return transactionlist;
            }
          
            return null;
        }
        public static List<TBL_BALANCE_TRANSFER_LOGS> GetDistributorRequisitionList(string MemberId, string status)
        {
            var db = new DBContext();
            if (status != "")
            {
                long mem_id = long.Parse(MemberId);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.FROM_MEMBER == mem_id && x.STATUS == status
                                       select new
                                       {
                                           Touser = "DISTRIBUTOR",
                                           transid = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           REQUEST_TIME = x.REQUEST_TIME,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           STATUS = x.STATUS,
                                           APPROVED_BY = x.APPROVED_BY,
                                           APPROVAL_DATE = x.APPROVAL_DATE,
                                           SLN = x.SLN
                                       }).AsEnumerable().Select((z,index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No=index+1,
                                           ToUser = z.Touser,
                                           TransactionID = z.transid,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           REQUEST_TIME = z.REQUEST_TIME,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           STATUS = z.STATUS,
                                           APPROVED_BY = z.APPROVED_BY,
                                           APPROVAL_DATE = z.APPROVAL_DATE,
                                           SLN = z.SLN
                                       }).ToList();
                return transactionlist;
            }
            else
            {
                long mem_id = long.Parse(MemberId);
                
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.FROM_MEMBER == mem_id
                                       select new
                                       {
                                           Touser = db.TBL_MASTER_MEMBER.Where(v=>v.MEM_ID==x.TO_MEMBER).Select(a=>a.UName).FirstOrDefault(),
                                           transid = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
                                           REQUEST_TIME = x.REQUEST_TIME,
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           STATUS = x.STATUS,
                                           APPROVED_BY = x.APPROVED_BY,
                                           APPROVAL_DATE = x.APPROVAL_DATE,
                                           SLN = x.SLN
                                       }).AsEnumerable().Select((z,index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No=index+1,
                                           ToUser = z.Touser,
                                           TransactionID = z.transid,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
                                           REQUEST_TIME = z.REQUEST_TIME,
                                           BANK_ACCOUNT = z.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = z.TRANSACTION_DETAILS,
                                           STATUS = z.STATUS,
                                           APPROVED_BY = z.APPROVED_BY,
                                           APPROVAL_DATE = z.APPROVAL_DATE,
                                           SLN = z.SLN
                                       }).ToList();
                return transactionlist;

            }
        }
    }
}