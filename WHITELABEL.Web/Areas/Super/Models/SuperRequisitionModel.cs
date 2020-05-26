namespace WHITELABEL.Web.Areas.Super.Models
{using System;
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
    public class SuperRequisitionModel
    {
        public static List<TBL_BALANCE_TRANSFER_LOGS> GetSuperMerchantRequisitionReport(string MemberId, string status)
        {
            var db = new DBContext();

            long memid = long.Parse(MemberId);
            if(status=="")
            {
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.FROM_MEMBER == memid 
                                       select new
                                       {
                                           Touser = db.TBL_MASTER_MEMBER.Where(s=>s.MEM_ID==y.INTRODUCER).Select(a=>a.UName).FirstOrDefault(),
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
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
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
            else
            {
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.FROM_MEMBER == memid && x.STATUS==status
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
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
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
        }

        public static List<TBL_BALANCE_TRANSFER_LOGS> GetDistributorRequisitionReport(string MemberId, string status)
        {
            var db = new DBContext();
            if (MemberId != "" && status != "")
            {
                long memid = long.Parse(MemberId);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID 
                                       where x.FROM_MEMBER == memid && x.STATUS==status
                                       select new
                                       {
                                           Touser = db.TBL_MASTER_MEMBER.Where(s => s.MEM_ID == y.INTRODUCER).Select(a => a.UName).FirstOrDefault(),
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
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
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
            else if (MemberId != "" && status == "")
            {
                long memid = long.Parse(MemberId);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.FROM_MEMBER == memid 
                                       select new
                                       {
                                           Touser = db.TBL_MASTER_MEMBER.Where(s => s.MEM_ID == y.INTRODUCER).Select(a => a.UName).FirstOrDefault(),
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
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
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

        public static List<TBL_BALANCE_TRANSFER_LOGS> GetAllDistributorRequisitionReport(string MemberId, string status)
        {
            var db = new DBContext();
            if (MemberId != "" && status == "")
            {
                long memid = long.Parse(MemberId);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.TO_MEMBER == memid 
                                       select new
                                       {
                                           Touser = db.TBL_MASTER_MEMBER.Where(s => s.MEM_ID == y.INTRODUCER).Select(a => a.UName).FirstOrDefault(),
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
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
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
            else if (MemberId == "" && status != "")
            {
                long memid = long.Parse(MemberId);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.TO_MEMBER == memid && x.STATUS == status
                                       select new
                                       {
                                           Touser = db.TBL_MASTER_MEMBER.Where(s => s.MEM_ID == y.INTRODUCER).Select(a => a.UName).FirstOrDefault(),
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
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
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
            else if (MemberId != "" && status != "")
            {
                long memid = long.Parse(MemberId);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.TO_MEMBER == memid && x.STATUS==status
                                       select new
                                       {
                                           Touser = db.TBL_MASTER_MEMBER.Where(s => s.MEM_ID == y.INTRODUCER).Select(a => a.UName).FirstOrDefault(),
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
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
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

        public static List<TBL_BALANCE_TRANSFER_LOGS> GetAllRetailerRequisitionReport(string MemberId, string status)
        {
            var db = new DBContext();
            long mem_id = long.Parse(MemberId);
            string[] searchTest = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();
            string[] MerchantID = db.TBL_MASTER_MEMBER.Where( x=> searchTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();

            //string[] RetailerId = (from x in db.TBL_MASTER_MEMBER where searchTest.Contains(x.INTRODUCER.ToString())
            //                       select new
            //                       {
            //                           MEM_ID=x.MEM_ID
            //                       }).ToArray(); 
            if (status != "")
            {
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where MerchantID.Contains(x.FROM_MEMBER.ToString()) && x.STATUS==status
                                       select new
                                       {
                                           Touser = db.TBL_MASTER_MEMBER.Where(s => s.MEM_ID == y.INTRODUCER).Select(a => a.UName).FirstOrDefault(),
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
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
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
            else
            {
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where MerchantID.Contains(x.FROM_MEMBER.ToString())
                                       select new
                                       {
                                           Touser = db.TBL_MASTER_MEMBER.Where(s => s.MEM_ID == y.INTRODUCER).Select(a => a.UName).FirstOrDefault(),
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
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
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
            
            
        }
        public static List<TBL_BALANCE_TRANSFER_LOGS> GetMerchantRequisitionReport(string MemberId, string status)
        {
            var db = new DBContext();
            if (!string.IsNullOrEmpty(MemberId) && !string.IsNullOrEmpty(status))
            {
                long Userid = long.Parse(MemberId);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.FROM_MEMBER == Userid && x.STATUS == status
                                       select new
                                       {
                                           Touser = "White Label",
                                           transId = x.TransactionID,
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
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           //ToUser = z.Touser,
                                           TransactionID = z.transId,
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
            else if (!string.IsNullOrEmpty(MemberId) && string.IsNullOrEmpty(status))
            {
                long Userid = long.Parse(MemberId);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.FROM_MEMBER == Userid
                                       select new
                                       {
                                           Touser = "White Label",
                                           transId = x.TransactionID,
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
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           //ToUser = z.Touser,
                                           TransactionID = z.transId,
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
    }
}