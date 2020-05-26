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
        public static List<TBL_BALANCE_TRANSFER_LOGS> GetDistributorMerchantRequisitionReport(string MemberId, string status,string DateFrom,string Date_To)
        {
            var db = new DBContext();
            if (status != "" && MemberId != "" && DateFrom != "" && Date_To != "")
            {
                long mem_id = long.Parse(MemberId);
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                if (status == "Pending")
                {
                    var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                           join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                           where x.FROM_MEMBER == mem_id && x.STATUS == status && x.REQUEST_DATE >= Date_From_Val && x.REQUEST_DATE <= Date_To_Val
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
                                               SLN = x.SLN,
                                               PAY_MODE = x.PAYMENT_METHOD
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
                                               SLN = z.SLN,
                                               PAYMENT_METHOD = z.PAY_MODE,

                                           }).ToList();
                    return transactionlist;
                }
                else if (status == "Approve")
                {
                    var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                           join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                           where x.FROM_MEMBER == mem_id && x.STATUS == status && x.REQUEST_DATE >= Date_From_Val && x.REQUEST_DATE <= Date_To_Val
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
                                               SLN = x.SLN,
                                               PAY_MODE = x.PAYMENT_METHOD
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
                                               SLN = z.SLN,
                                               PAYMENT_METHOD = z.PAY_MODE,

                                           }).ToList();
                    return transactionlist;
                }
                else if (status == "Decline")
                {
                    var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                           join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                           where x.FROM_MEMBER == mem_id && x.STATUS == status && x.REQUEST_DATE >= Date_From_Val && x.REQUEST_DATE <= Date_To_Val
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
                                               SLN = x.SLN,
                                               PAY_MODE = x.PAYMENT_METHOD
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
                                               SLN = z.SLN,
                                               PAYMENT_METHOD = z.PAY_MODE,

                                           }).ToList();
                    return transactionlist;
                }
                else
                {
                    var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                           join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                           where x.FROM_MEMBER == mem_id && x.STATUS == status && x.APPROVAL_DATE >= Date_From_Val && x.APPROVAL_DATE <= Date_To_Val
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
                                               SLN = x.SLN,
                                               PAY_MODE = x.PAYMENT_METHOD
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
                                               SLN = z.SLN,
                                               PAYMENT_METHOD = z.PAY_MODE,

                                           }).ToList();
                    return transactionlist;
                }


            }
            else if (status == "" && MemberId != "" && DateFrom != "" && Date_To != "")
            {
                long mem_id = long.Parse(MemberId);
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.FROM_MEMBER == mem_id && x.REQUEST_DATE >= Date_From_Val && x.REQUEST_DATE <= Date_To_Val
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
                                           SLN = x.SLN,
                                           PAY_MODE = x.PAYMENT_METHOD
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
                                           SLN = z.SLN,
                                           PAYMENT_METHOD = z.PAY_MODE
                                       }).ToList();
                return transactionlist;
            }
            else if (DateFrom != "" && Date_To != "")
            {
                long mem_id = long.Parse(MemberId);
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.FROM_MEMBER == mem_id && x.REQUEST_DATE >= Date_From_Val && x.REQUEST_DATE <= Date_To_Val
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
                                           SLN = x.SLN,
                                           PAY_MODE = x.PAYMENT_METHOD
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
                                           SLN = z.SLN,
                                           PAYMENT_METHOD = z.PAY_MODE,

                                       }).ToList();
                return transactionlist;
            }
            else if (status == "Pending" && MemberId != "" && DateFrom == "" && Date_To == "")
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
                                           SLN = x.SLN,
                                           PAY_MODE = x.PAYMENT_METHOD
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
                                           SLN = z.SLN,
                                           PAYMENT_METHOD = z.PAY_MODE,

                                       }).ToList();
                return transactionlist;
            }
            else if (status == "Approve" && MemberId != "" && DateFrom == "" && Date_To == "")
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
                                           SLN = x.SLN,
                                           PAY_MODE = x.PAYMENT_METHOD
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
                                           SLN = z.SLN,
                                           PAYMENT_METHOD = z.PAY_MODE,

                                       }).ToList();
                return transactionlist;
            }
            else if (status == "Decline" && MemberId != "" && DateFrom == "" && Date_To == "")
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
                                           SLN = x.SLN,
                                           PAY_MODE = x.PAYMENT_METHOD
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
                                           SLN = z.SLN,
                                           PAYMENT_METHOD = z.PAY_MODE,

                                       }).ToList();
                return transactionlist;
            }

            return null;
        }

        public static List<TBL_BALANCE_TRANSFER_LOGS> GetDistributorMerchantRequisitionReportList(string MemberId, string status,string DateFrom,string Date_To)
        {
            var db = new DBContext();
            if (status == "" && MemberId != "" && DateFrom != "" && Date_To != "")
            {
                long mem_id = long.Parse(MemberId);
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);

                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.TO_MEMBER == mem_id && x.REQUEST_DATE >= Date_From_Val && x.REQUEST_DATE <= Date_To_Val
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
                                           SLN = x.SLN,
                                           PAY_MODE = x.PAYMENT_METHOD
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
                                           SLN = z.SLN,
                                           PAYMENT_METHOD = z.PAY_MODE
                                       }).ToList();
                return transactionlist;
            }
            else if (status == "Pending" && MemberId != "" && DateFrom == "" && Date_To == "")
            {
                long mem_id = long.Parse(MemberId);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.TO_MEMBER == mem_id && x.STATUS == status
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
                                           SLN = x.SLN,
                                           PAY_MODE = x.PAYMENT_METHOD
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
                                           SLN = z.SLN,
                                           PAYMENT_METHOD = z.PAY_MODE
                                       }).ToList();
                return transactionlist;

            }
            else if (status == "Approve" && MemberId != "" && DateFrom == "" && Date_To == "")
            {
                long mem_id = long.Parse(MemberId);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.TO_MEMBER == mem_id && x.STATUS == status
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
                                           SLN = x.SLN,
                                           PAY_MODE = x.PAYMENT_METHOD
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
                                           SLN = z.SLN,
                                           PAYMENT_METHOD = z.PAY_MODE
                                       }).ToList();
                return transactionlist;

            }
            else if (status == "Decline" && MemberId != "" && DateFrom == "" && Date_To == "")
            {
                long mem_id = long.Parse(MemberId);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.TO_MEMBER == mem_id && x.STATUS == status
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
                                           SLN = x.SLN,
                                           PAY_MODE = x.PAYMENT_METHOD
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
                                           SLN = z.SLN,
                                           PAYMENT_METHOD = z.PAY_MODE
                                       }).ToList();
                return transactionlist;

            }
            else if (status != "" && MemberId != "" && DateFrom != "" && Date_To != "")
            {
                long mem_id = long.Parse(MemberId);
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                if (status == "Pending")
                {
                    var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                           join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                           where x.TO_MEMBER == mem_id && x.STATUS == status && x.REQUEST_DATE >= Date_From_Val && x.REQUEST_DATE <= Date_To_Val
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
                                               SLN = x.SLN,
                                               PAY_MODE = x.PAYMENT_METHOD
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
                                               SLN = z.SLN,
                                               PAYMENT_METHOD = z.PAY_MODE
                                           }).ToList();
                    return transactionlist;
                }
                else if (status == "Approve")
                {
                    var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                           join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                           where x.TO_MEMBER == mem_id && x.STATUS == status && x.REQUEST_DATE >= Date_From_Val && x.REQUEST_DATE <= Date_To_Val
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
                                               SLN = x.SLN,
                                               PAY_MODE = x.PAYMENT_METHOD
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
                                               SLN = z.SLN,
                                               PAYMENT_METHOD = z.PAY_MODE
                                           }).ToList();
                    return transactionlist;
                }
                else if (status == "Decline")
                {
                    var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                           join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                           where x.TO_MEMBER == mem_id && x.STATUS == status && x.REQUEST_DATE >= Date_From_Val && x.REQUEST_DATE <= Date_To_Val
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
                                               SLN = x.SLN,
                                               PAY_MODE = x.PAYMENT_METHOD
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
                                               SLN = z.SLN,
                                               PAYMENT_METHOD = z.PAY_MODE
                                           }).ToList();
                    return transactionlist;
                }

                else
                {
                    var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                           join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                           where x.TO_MEMBER == mem_id && x.STATUS == status && x.APPROVAL_DATE >= Date_From_Val && x.APPROVAL_DATE <= Date_To_Val
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
                                               SLN = x.SLN,
                                               PAY_MODE = x.PAYMENT_METHOD
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
                                               SLN = z.SLN,
                                               PAYMENT_METHOD = z.PAY_MODE
                                           }).ToList();
                    return transactionlist;

                }

               
            }
            else
            {
                long mem_id = long.Parse(MemberId);
                DateTime Todaydate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       //where x.TO_MEMBER == mem_id && x.STATUS == status
                                       where x.TO_MEMBER == mem_id && x.REQUEST_DATE == Todaydate
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
                                           SLN = x.SLN,
                                           PAY_MODE = x.PAYMENT_METHOD
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
                                           SLN = z.SLN,
                                           PAYMENT_METHOD = z.PAY_MODE
                                       }).ToList();
                return transactionlist;

            }

            return null;
        }
        public static List<TBL_BALANCE_TRANSFER_LOGS> GetDistributorRequisitionList(string MemberId, string status,string DateFrom,string Date_To)
        {
            var db = new DBContext();
            if (status != "" && DateFrom!="" && Date_To!="")
            {
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                long mem_id = long.Parse(MemberId);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.FROM_MEMBER == mem_id && x.STATUS == status && x.REQUEST_DATE>= Date_From_Val  && x.REQUEST_DATE<= Date_To_Val
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
                                           SLN = x.SLN,
                                           PAY_MODE=x.PAYMENT_METHOD
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
                                           SLN = z.SLN,
                                           PAYMENT_METHOD=z.PAY_MODE
                                       }).ToList();
                return transactionlist;
            }
            else if (status != "" && DateFrom == "" && Date_To == "")
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
                                           SLN = x.SLN,
                                           PAY_MODE = x.PAYMENT_METHOD
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
                                           SLN = z.SLN,
                                           PAYMENT_METHOD = z.PAY_MODE
                                       }).ToList();
                return transactionlist;
            }
            else if (status == "" && DateFrom != "" && Date_To != "")
            {
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                long mem_id = long.Parse(MemberId);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.FROM_MEMBER == mem_id && x.STATUS == status && x.REQUEST_DATE >= Date_From_Val && x.REQUEST_DATE <= Date_To_Val
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
                                           SLN = x.SLN,
                                           PAY_MODE = x.PAYMENT_METHOD
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
                                           SLN = z.SLN,
                                           PAYMENT_METHOD = z.PAY_MODE
                                       }).ToList();
                return transactionlist;
            }
            else
            {
                long mem_id = long.Parse(MemberId);
                DateTime Todaydate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.FROM_MEMBER == mem_id && x.REQUEST_DATE == Todaydate
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
                                           SLN = x.SLN,
                                           PAY_MODE = x.PAYMENT_METHOD
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
                                           SLN = z.SLN,
                                           PAYMENT_METHOD = z.PAY_MODE
                                       }).ToList();
                return transactionlist;

            }
        }
    }
}