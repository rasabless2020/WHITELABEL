namespace WHITELABEL.Web.Areas.Admin.Models
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

    public class MemberRequisitionReportModel
    {
        public static List<TBL_BALANCE_TRANSFER_LOGS> GetAdminRequisitionReport(string MemberId, string status,string DateFrom,string Date_TO)
        {
            var db = new DBContext();
            if (status != "" && DateFrom == "" && Date_TO == "")
            {
                long UserID = long.Parse(MemberId);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.FROM_MEMBER == UserID && x.STATUS == status
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
                                           SLN = x.SLN,
                                           Pay_Mode = x.PAYMENT_METHOD
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           ToUser = z.Touser,
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
                                           SLN = z.SLN,
                                           PAYMENT_METHOD = z.Pay_Mode
                                       }).ToList();
                return transactionlist;
            }
            else if (status != "" && DateFrom != "" && Date_TO != "")
            {
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_TO.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                long UserID = long.Parse(MemberId);
                if (status == "Pending")
                {
                    var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                           join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                           where x.FROM_MEMBER == UserID && x.STATUS == status  && x.REQUEST_DATE>= Date_From_Val && x.REQUEST_DATE<= Date_To_Val
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
                                               SLN = x.SLN,
                                               Pay_Mode = x.PAYMENT_METHOD
                                           }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                           {
                                               Serial_No = index + 1,
                                               ToUser = z.Touser,
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
                                               SLN = z.SLN,
                                               PAYMENT_METHOD = z.Pay_Mode
                                           }).ToList();
                    return transactionlist;
                }
                else
                {
                    var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                           join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                           where x.FROM_MEMBER == UserID && x.STATUS == status && x.APPROVAL_DATE >= Date_From_Val && x.APPROVAL_DATE <= Date_To_Val
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
                                               SLN = x.SLN,
                                               Pay_Mode = x.PAYMENT_METHOD
                                           }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                           {
                                               Serial_No = index + 1,
                                               ToUser = z.Touser,
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
                                               SLN = z.SLN,
                                               PAYMENT_METHOD = z.Pay_Mode
                                           }).ToList();
                    return transactionlist;

                }
               
            }
            else if (status == "" && DateFrom != "" && Date_TO != "")
            {
                long UserID = long.Parse(MemberId);
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_TO.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.FROM_MEMBER == UserID && x.REQUEST_DATE>= Date_From_Val && x.REQUEST_DATE<= Date_To_Val
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
                                           SLN = x.SLN,
                                           Pay_Mode = x.PAYMENT_METHOD
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           ToUser = z.Touser,
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
                                           SLN = z.SLN,
                                           PAYMENT_METHOD = z.Pay_Mode
                                       }).ToList();
                return transactionlist;
            }
            else
            {
                DateTime Todaydate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                long UserID = long.Parse(MemberId);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.FROM_MEMBER == UserID && x.REQUEST_DATE == Todaydate
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
                                           SLN = x.SLN,
                                           Pay_Mode = x.PAYMENT_METHOD
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           ToUser = z.Touser,
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
                                           SLN = z.SLN,
                                           PAYMENT_METHOD = z.Pay_Mode
                                       }).ToList();
                return transactionlist;
            }
            return null;
        }
        public static List<TBL_BALANCE_TRANSFER_LOGS> GetSuperRequisitionReport(string MemberId, string status)
        {
            var db = new DBContext();

            if (status != "" && MemberId != "")
            {
                long UserID = long.Parse(MemberId);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.FROM_MEMBER == UserID && x.STATUS == status
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
                                           SLN = x.SLN,
                                           PAY_MODE=x.PAYMENT_METHOD
                                       }).AsEnumerable().Select((z,index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           ToUser = z.Touser,
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
                                           SLN = z.SLN,
                                           PAYMENT_METHOD=z.PAY_MODE
                                       }).ToList();
                return transactionlist;
            }
            else if (status == "" && MemberId != "")
            {
                long UserID = long.Parse(MemberId);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.FROM_MEMBER == UserID 
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
                                           SLN = x.SLN,
                                           PAY_MODE=x.PAYMENT_METHOD
                                       }).AsEnumerable().Select((z,index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           ToUser = z.Touser,
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
                                           SLN = z.SLN,
                                           PAYMENT_METHOD=z.PAY_MODE
                                       }).ToList();
                return transactionlist;
            }
            
            return null;
        }
        public static List<TBL_BALANCE_TRANSFER_LOGS> GetAllSuperRequisitionReport(string MemberId, string status)
        {
            var db = new DBContext();
            if (MemberId != "" && status != "") {
                long UserID = long.Parse(MemberId);
                string[] searchTest = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == UserID).Select(a => a.MEM_ID.ToString()).ToArray();
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where searchTest.Contains(x.FROM_MEMBER.ToString()) && x.STATUS == status
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
                                           SLN = x.SLN,
                                           Pay_mode=x.PAYMENT_METHOD
                                       }).AsEnumerable().Select((z,index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           ToUser = z.Touser,
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
                                           SLN = z.SLN,
                                           PAYMENT_METHOD=z.Pay_mode
                                       }).ToList();
                return transactionlist;
            }
            else if (MemberId != "" && status == "")
            {
                long UserID = long.Parse(MemberId);
                string[] searchTest = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == UserID).Select(a => a.MEM_ID.ToString()).ToArray();
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where searchTest.Contains(x.FROM_MEMBER.ToString())
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
                                           SLN = x.SLN,
                                           Pay_Mode=x.PAYMENT_METHOD
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           ToUser = z.Touser,
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
                                           SLN = z.SLN,
                                           PAYMENT_METHOD=z.Pay_Mode
                                       }).ToList();
                return transactionlist;
            }
                return null;
        }

        public static List<TBL_BALANCE_TRANSFER_LOGS> GetDistributorRequisitionReport(string Super, string MemberId, string status,string DateFrom,string Date_To)
        {
            var db = new DBContext();
            if (Super!="" && MemberId != "" && status != "" && DateFrom=="" && Date_To=="")
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
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           STATUS = x.STATUS,
                                           APPROVED_BY = x.APPROVED_BY,
                                           APPROVAL_DATE = x.APPROVAL_DATE,
                                           SLN = x.SLN,
                                           PAY_MODE=x.PAYMENT_METHOD
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           //ToUser = z.Touser,
                                           TransactionID = z.transId,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
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
            else if (Super != "" && MemberId != "" && status == "" && DateFrom == "" && Date_To == "")
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
                                           AMOUNT = x.AMOUNT,
                                           BANK_ACCOUNT = x.BANK_ACCOUNT,
                                           TRANSACTION_DETAILS = x.TRANSACTION_DETAILS,
                                           STATUS = x.STATUS,
                                           APPROVED_BY = x.APPROVED_BY,
                                           APPROVAL_DATE = x.APPROVAL_DATE,
                                           SLN = x.SLN,
                                           PAY_MODE=x.PAYMENT_METHOD
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           //ToUser = z.Touser,
                                           TransactionID = z.transId,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
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
            else if (Super != "" && MemberId == "" && status == "" && DateFrom == "" && Date_To == "")
            {
                long Userid = long.Parse(Super);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.TO_MEMBER == y.UNDER_WHITE_LEVEL
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
                                           SLN = x.SLN,
                                           PAY_MODE=x.PAYMENT_METHOD
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
                                           SLN = z.SLN,
                                           PAYMENT_METHOD=z.PAY_MODE
                                       }).ToList();
                return transactionlist;
            }
            else if (Super != "" && MemberId == "" && status != "" && DateFrom == "" && Date_To == "")
            {
                long Userid = long.Parse(Super);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.TO_MEMBER == y.UNDER_WHITE_LEVEL && x.STATUS==status
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
                                           SLN = x.SLN,
                                           PAY_MODE=x.PAYMENT_METHOD
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
                                           SLN = z.SLN,
                                           PAYMENT_METHOD=z.PAY_MODE
                                       }).ToList();
                return transactionlist;
            }
            else if (Super != "" && MemberId != "" && status != "" && DateFrom != "" && Date_To != "")
            {
                long Userid = long.Parse(MemberId);
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
                                           where x.FROM_MEMBER == Userid && x.STATUS == status && x.REQUEST_DATE>= Date_From_Val && x.REQUEST_DATE<= Date_To_Val
                                           select new
                                           {
                                               Touser = "White Label",
                                               transId = x.TransactionID,
                                               FromUser = y.UName,
                                               REQUEST_DATE = x.REQUEST_DATE,
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
                                               //ToUser = z.Touser,
                                               TransactionID = z.transId,
                                               FromUser = z.FromUser,
                                               AMOUNT = z.AMOUNT,
                                               REQUEST_DATE = z.REQUEST_DATE,
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
                                           where x.FROM_MEMBER == Userid && x.STATUS == status && x.APPROVAL_DATE >= Date_From_Val && x.APPROVAL_DATE <= Date_To_Val
                                           select new
                                           {
                                               Touser = "White Label",
                                               transId = x.TransactionID,
                                               FromUser = y.UName,
                                               REQUEST_DATE = x.REQUEST_DATE,
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
                                               //ToUser = z.Touser,
                                               TransactionID = z.transId,
                                               FromUser = z.FromUser,
                                               AMOUNT = z.AMOUNT,
                                               REQUEST_DATE = z.REQUEST_DATE,
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
            else if (Super != "" && MemberId == "" && status != "" && DateFrom != "" && Date_To != "")
            {
                //long Userid = long.Parse(MemberId);
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
                                           where x.TO_MEMBER == y.UNDER_WHITE_LEVEL && x.STATUS == status && x.REQUEST_DATE >= Date_From_Val && x.REQUEST_DATE <= Date_To_Val
                                           select new
                                           {
                                               Touser = "White Label",
                                               transId = x.TransactionID,
                                               FromUser = y.UName,
                                               REQUEST_DATE = x.REQUEST_DATE,
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
                                               //ToUser = z.Touser,
                                               TransactionID = z.transId,
                                               FromUser = z.FromUser,
                                               AMOUNT = z.AMOUNT,
                                               REQUEST_DATE = z.REQUEST_DATE,
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
                                           where x.TO_MEMBER == y.UNDER_WHITE_LEVEL && x.STATUS == status && x.APPROVAL_DATE >= Date_From_Val && x.APPROVAL_DATE <= Date_To_Val
                                           select new
                                           {
                                               Touser = "White Label",
                                               transId = x.TransactionID,
                                               FromUser = y.UName,
                                               REQUEST_DATE = x.REQUEST_DATE,
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
                                               //ToUser = z.Touser,
                                               TransactionID = z.transId,
                                               FromUser = z.FromUser,
                                               AMOUNT = z.AMOUNT,
                                               REQUEST_DATE = z.REQUEST_DATE,
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
            else if (Super != "" && MemberId == "" && status == "" && DateFrom != "" && Date_To != "")
            {
                //long Userid = long.Parse(MemberId);
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                
                    var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                           join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                           where x.TO_MEMBER == y.UNDER_WHITE_LEVEL  && x.REQUEST_DATE >= Date_From_Val && x.REQUEST_DATE <= Date_To_Val
                                           select new
                                           {
                                               Touser = "White Label",
                                               transId = x.TransactionID,
                                               FromUser = y.UName,
                                               REQUEST_DATE = x.REQUEST_DATE,
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
                                               //ToUser = z.Touser,
                                               TransactionID = z.transId,
                                               FromUser = z.FromUser,
                                               AMOUNT = z.AMOUNT,
                                               REQUEST_DATE = z.REQUEST_DATE,
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
            else if (Super != "" && MemberId != "" && status == "" && DateFrom != "" && Date_To != "")
            {
                long Userid = long.Parse(MemberId);
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);

                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.FROM_MEMBER == Userid && x.REQUEST_DATE >= Date_From_Val && x.REQUEST_DATE <= Date_To_Val
                                       select new
                                       {
                                           Touser = "White Label",
                                           transId = x.TransactionID,
                                           FromUser = y.UName,
                                           REQUEST_DATE = x.REQUEST_DATE,
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
                                           //ToUser = z.Touser,
                                           TransactionID = z.transId,
                                           FromUser = z.FromUser,
                                           AMOUNT = z.AMOUNT,
                                           REQUEST_DATE = z.REQUEST_DATE,
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

        public static List<TBL_BALANCE_TRANSFER_LOGS> GetAllDistributorRequisitionReport(string MemberId, string status,string DateFrom,string Date_To)
        {
            var db = new DBContext();

            long UserID = long.Parse(MemberId);
            string[] SuperMemId = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == UserID).Select(a => a.MEM_ID.ToString()).ToArray();
            string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(x => SuperMemId.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
            if (MemberId != "" && status != "" && DateFrom=="" && Date_To=="")
            {
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where DistributorMemId.Contains(x.FROM_MEMBER.ToString()) && x.STATUS == status
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
                                           SLN = x.SLN,
                                           PAY_MODE = x.PAYMENT_METHOD
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           ToUser = z.Touser,
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
                                           SLN = z.SLN,
                                           PAYMENT_METHOD = z.PAY_MODE
                                       }).ToList();
                return transactionlist;
            }
            else if (MemberId != "" && status != "" && DateFrom != "" && Date_To != "")
            {
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
                                           where DistributorMemId.Contains(x.FROM_MEMBER.ToString()) && x.STATUS == status && x.REQUEST_DATE>= Date_From_Val && x.REQUEST_DATE<= Date_To_Val
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
                                               SLN = x.SLN,
                                               PAY_MODE = x.PAYMENT_METHOD
                                           }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                           {
                                               Serial_No = index + 1,
                                               ToUser = z.Touser,
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
                                               SLN = z.SLN,
                                               PAYMENT_METHOD = z.PAY_MODE
                                           }).ToList();
                    return transactionlist;
                }
                else
                {
                    var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                           join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                           where DistributorMemId.Contains(x.FROM_MEMBER.ToString()) && x.STATUS == status && x.APPROVAL_DATE >= Date_From_Val && x.APPROVAL_DATE <= Date_To_Val
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
                                               SLN = x.SLN,
                                               PAY_MODE = x.PAYMENT_METHOD
                                           }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                           {
                                               Serial_No = index + 1,
                                               ToUser = z.Touser,
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
                                               SLN = z.SLN,
                                               PAYMENT_METHOD = z.PAY_MODE
                                           }).ToList();
                    return transactionlist;
                }


                
            }
            else if (MemberId != "" && status == "" && DateFrom != "" && Date_To != "")
            {
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where DistributorMemId.Contains(x.FROM_MEMBER.ToString())  && x.REQUEST_DATE >= Date_From_Val && x.REQUEST_DATE<= Date_To_Val
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
                                           SLN = x.SLN,
                                           PAY_MODE = x.PAYMENT_METHOD
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
                                           SLN = z.SLN,
                                           PAYMENT_METHOD = z.PAY_MODE
                                       }).ToList();
                return transactionlist;
            }
            else if (MemberId == "" && status != "" && DateFrom != "" && Date_To != "")
            {
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
                                           where DistributorMemId.Contains(x.FROM_MEMBER.ToString()) && x.STATUS == status && x.REQUEST_DATE >= Date_From_Val && x.REQUEST_DATE <= Date_To_Val
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
                                               SLN = x.SLN,
                                               PAY_MODE = x.PAYMENT_METHOD
                                           }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                           {
                                               Serial_No = index + 1,
                                               ToUser = z.Touser,
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
                                               SLN = z.SLN,
                                               PAYMENT_METHOD = z.PAY_MODE
                                           }).ToList();
                    return transactionlist;
                }
                else
                {
                    var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                           join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                           where DistributorMemId.Contains(x.FROM_MEMBER.ToString()) && x.STATUS == status && x.APPROVAL_DATE >= Date_From_Val && x.APPROVAL_DATE <= Date_To_Val
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
                                               SLN = x.SLN,
                                               PAY_MODE = x.PAYMENT_METHOD
                                           }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                           {
                                               Serial_No = index + 1,
                                               ToUser = z.Touser,
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
                                               SLN = z.SLN,
                                               PAYMENT_METHOD = z.PAY_MODE
                                           }).ToList();
                    return transactionlist;
                }



            }
            else if (MemberId == "" && status == "" && DateFrom != "" && Date_To != "")
            {
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where DistributorMemId.Contains(x.FROM_MEMBER.ToString()) && x.REQUEST_DATE >= Date_From_Val && x.REQUEST_DATE <= Date_To_Val
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
                                           SLN = x.SLN,
                                           PAY_MODE = x.PAYMENT_METHOD
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
                                           SLN = z.SLN,
                                           PAYMENT_METHOD = z.PAY_MODE
                                       }).ToList();
                return transactionlist;
            }
            else if (MemberId != "" && status == "" && DateFrom == "" && Date_To == "")
            {
                DateTime Todaydate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                       join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                       where x.TO_MEMBER== UserID && x.REQUEST_DATE == Todaydate
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
                                           SLN = x.SLN,
                                           PAY_MODE = x.PAYMENT_METHOD
                                       }).AsEnumerable().Select((z, index) => new TBL_BALANCE_TRANSFER_LOGS
                                       {
                                           Serial_No = index + 1,
                                           ToUser = z.Touser,
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
                                           SLN = z.SLN,
                                           PAYMENT_METHOD = z.PAY_MODE
                                       }).ToList();
                return transactionlist;
            }
            return null;
        }

        public static List<TBL_BALANCE_TRANSFER_LOGS> GetAllMerchantRequisitionReport(string MemberId, string status,string DateFrom,string Date_To)
        {
            var db = new DBContext();
            if (status != "")
            {
                long mem_id = long.Parse(MemberId);
                if (DateFrom != "" && Date_To != "")
                { //string[] SuperMemId = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);

                    //string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(x => SuperMemId.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                    string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();
                    //string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(x => SuperMemId.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                    string[] MerchantuserId = db.TBL_MASTER_MEMBER.Where(x => DistributorMemId.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                    if (status == "Pending")
                    {
                        var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                               join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                               where MerchantuserId.Contains(x.FROM_MEMBER.ToString()) && x.STATUS == status && x.REQUEST_DATE >= Date_From_Val && x.REQUEST_DATE <= Date_To_Val
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
                                                   SLN = x.SLN,
                                                   PAY_MODE = x.PAYMENT_METHOD
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
                                                   SLN = z.SLN,
                                                   PAYMENT_METHOD = z.PAY_MODE
                                               }).ToList();
                        return transactionlist;
                    }
                    else
                    {
                        var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                               join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                               where MerchantuserId.Contains(x.FROM_MEMBER.ToString()) && x.STATUS == status && x.APPROVAL_DATE >= Date_From_Val && x.APPROVAL_DATE <= Date_To_Val
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
                                                   SLN = x.SLN,
                                                   PAY_MODE = x.PAYMENT_METHOD
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
                                                   SLN = z.SLN,
                                                   PAYMENT_METHOD = z.PAY_MODE
                                               }).ToList();
                        return transactionlist;
                    }
                    
                }
                else
                { //string[] SuperMemId = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();
                  //string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(x => SuperMemId.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                    string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();
                    //string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(x => SuperMemId.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                    string[] MerchantuserId = db.TBL_MASTER_MEMBER.Where(x => DistributorMemId.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();                 


                        var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                               join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                               where MerchantuserId.Contains(x.FROM_MEMBER.ToString()) && x.STATUS == status
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
                                                   SLN = x.SLN,
                                                   PAY_MODE = x.PAYMENT_METHOD
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
                                                   SLN = z.SLN,
                                                   PAYMENT_METHOD = z.PAY_MODE
                                               }).ToList();
                        return transactionlist;
                    

                       
                }
               
            }
            else if (DateFrom != "" && Date_To != "")
            {
                long mem_id = long.Parse(MemberId);
                //string[] SuperMemId = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);

                //string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(x => SuperMemId.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();
                //string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(x => SuperMemId.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                string[] MerchantuserId = db.TBL_MASTER_MEMBER.Where(x => DistributorMemId.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
               
                    var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                           join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                           where MerchantuserId.Contains(x.FROM_MEMBER.ToString()) &&  x.APPROVAL_DATE >= Date_From_Val && x.APPROVAL_DATE <= Date_To_Val
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
                                               SLN = x.SLN,
                                               PAY_MODE = x.PAYMENT_METHOD
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
                                               SLN = z.SLN,
                                               PAYMENT_METHOD = z.PAY_MODE
                                           }).ToList();
                    return transactionlist;
                

            }
            else if (status != "" && DateFrom != "" && Date_To != "")
            {
                long mem_id = long.Parse(MemberId);
                //string[] SuperMemId = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();
                string FromDATE = string.Empty;
                string TO_DATE = string.Empty;
                FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                string From_TO = string.Empty;
                TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);

                //string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(x => SuperMemId.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();
                //string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(x => SuperMemId.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                string[] MerchantuserId = db.TBL_MASTER_MEMBER.Where(x => DistributorMemId.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                if (status == "Pending")
                {
                    var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                           join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                           where MerchantuserId.Contains(x.FROM_MEMBER.ToString()) && x.STATUS == status && x.REQUEST_DATE >= Date_From_Val && x.REQUEST_DATE <= Date_To_Val
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
                                               SLN = x.SLN,
                                               PAY_MODE = x.PAYMENT_METHOD
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
                                               SLN = z.SLN,
                                               PAYMENT_METHOD = z.PAY_MODE
                                           }).ToList();
                    return transactionlist;
                }
                else
                {
                    var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                           join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                           where MerchantuserId.Contains(x.FROM_MEMBER.ToString()) && x.STATUS==status && x.APPROVAL_DATE >= Date_From_Val && x.APPROVAL_DATE <= Date_To_Val
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
                                               SLN = x.SLN,
                                               PAY_MODE = x.PAYMENT_METHOD
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
                                               SLN = z.SLN,
                                               PAYMENT_METHOD = z.PAY_MODE
                                           }).ToList();
                    return transactionlist;
                }
               


            }
            else
            {
                long mem_id = long.Parse(MemberId);
                //string[] SuperMemId = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();
                //string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(x => SuperMemId.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();
                //string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(x => SuperMemId.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                string[] MerchantuserId = db.TBL_MASTER_MEMBER.Where(x => DistributorMemId.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                if (DateFrom != "" && Date_To != "")
                {
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                           join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                           where MerchantuserId.Contains(x.FROM_MEMBER.ToString()) && x.REQUEST_DATE >= Date_From_Val && x.REQUEST_DATE <= Date_To_Val
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
                                               SLN = x.SLN,
                                               PAY_MODE = x.PAYMENT_METHOD
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
                                               SLN = z.SLN,
                                               PAYMENT_METHOD = z.PAY_MODE
                                           }).ToList();
                    return transactionlist;
                }
                else
                {
                    DateTime Todaydate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                    var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                           join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                           where MerchantuserId.Contains(x.FROM_MEMBER.ToString()) && x.REQUEST_DATE == Todaydate
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
                                               SLN = x.SLN,
                                               PAY_MODE = x.PAYMENT_METHOD
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
                                               SLN = z.SLN,
                                               PAYMENT_METHOD = z.PAY_MODE
                                           }).ToList();
                    return transactionlist;
                }
                   
            }
            return null;
        }
        public static List<TBL_BALANCE_TRANSFER_LOGS> GetMerchantRequisitionReport(string Super, string Distributor, string MemberId, string status,string DateFrom,string Date_To)
        {
            var db = new DBContext();
            if (!string.IsNullOrEmpty(Super) && !string.IsNullOrEmpty(Distributor) && !string.IsNullOrEmpty(MemberId) && !string.IsNullOrEmpty(status))
            {
                long Userid = long.Parse(MemberId);
                if (DateFrom != "" && Date_To != "" && status == "Pending")
                {
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                           join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                           where x.FROM_MEMBER == Userid && x.STATUS == status && x.REQUEST_DATE >= Date_From_Val && x.REQUEST_DATE <= Date_To_Val
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
                                               SLN = x.SLN,
                                               PAY_MODE = x.PAYMENT_METHOD
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
                                               SLN = z.SLN,
                                               PAYMENT_METHOD = z.PAY_MODE
                                           }).ToList();
                    return transactionlist;
                }
                else if (DateFrom != "" && Date_To != "" && status != "Pending")
                {
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    DateTime OneDay = Convert.ToDateTime(Date_To);
                    DateTime AddoneDay= Convert.ToDateTime(OneDay.AddDays(1));


                    TO_DATE = DateTime.Parse(AddoneDay.ToString()).ToString("yyyy-MM-dd");

                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                           join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                           where x.FROM_MEMBER == Userid && x.STATUS == status && x.APPROVAL_DATE >= Date_From_Val && x.APPROVAL_DATE <= Date_To_Val
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
                                               SLN = x.SLN,
                                               PAY_MODE = x.PAYMENT_METHOD
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
                                               SLN = z.SLN,
                                               PAYMENT_METHOD = z.PAY_MODE
                                           }).ToList();
                    return transactionlist;
                }
                else
                {
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
                                               SLN = x.SLN,
                                               PAY_MODE = x.PAYMENT_METHOD
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
                                               SLN = z.SLN,
                                               PAYMENT_METHOD = z.PAY_MODE
                                           }).ToList();
                    return transactionlist;
                }

            }
            else if (!string.IsNullOrEmpty(Super) && !string.IsNullOrEmpty(Distributor) && !string.IsNullOrEmpty(MemberId) && string.IsNullOrEmpty(status))
            {
                long Userid = long.Parse(MemberId);
                if (DateFrom != "" && Date_To != "")
                {
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                           join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                           where x.FROM_MEMBER == Userid && x.REQUEST_DATE >= Date_From_Val && x.REQUEST_DATE <= Date_To_Val
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
                                               SLN = x.SLN,
                                               PAY_MODE = x.PAYMENT_METHOD
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
                                               SLN = z.SLN,
                                               PAYMENT_METHOD = z.PAY_MODE
                                           }).ToList();
                    return transactionlist;
                }
                else
                {
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
                                               SLN = x.SLN,
                                               PAY_MODE = x.PAYMENT_METHOD
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
                                               SLN = z.SLN,
                                               PAYMENT_METHOD = z.PAY_MODE
                                           }).ToList();
                    return transactionlist;
                }

                
            }

            else if (!string.IsNullOrEmpty(Super) && !string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(MemberId) && !string.IsNullOrEmpty(status))
            {
                long Userid = long.Parse(Distributor);
                if (DateFrom != "" && Date_To != "" && status != "Pending")
                {
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                           join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                           where x.FROM_MEMBER == Userid && x.STATUS == status && x.APPROVAL_DATE >= Date_From_Val && x.APPROVAL_DATE<= Date_To_Val
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
                                               SLN = x.SLN,
                                               PAY_MODE = x.PAYMENT_METHOD

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
                                               SLN = z.SLN,
                                               PAYMENT_METHOD = z.PAY_MODE
                                           }).ToList();
                    return transactionlist;
                }
                else if (DateFrom != "" && Date_To != "" && status == "Pending")
                {
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                           join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                           where x.FROM_MEMBER == Userid && x.STATUS == status  && x.REQUEST_DATE>= Date_From_Val && x.REQUEST_DATE <= Date_To_Val
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
                                               SLN = x.SLN,
                                               PAY_MODE = x.PAYMENT_METHOD

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
                                               SLN = z.SLN,
                                               PAYMENT_METHOD = z.PAY_MODE
                                           }).ToList();
                    return transactionlist;
                }
                else
                {
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
                                               SLN = x.SLN,
                                               PAY_MODE = x.PAYMENT_METHOD

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
                                               SLN = z.SLN,
                                               PAYMENT_METHOD = z.PAY_MODE
                                           }).ToList();
                    return transactionlist;
                }
                   
            }
            else if (!string.IsNullOrEmpty(Super) && !string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(MemberId) && string.IsNullOrEmpty(status))
            {
                long Userid = long.Parse(Distributor);
                if (DateFrom != "" && Date_To != "")
                {
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);

                    var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                           join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                           where x.FROM_MEMBER == Userid && x.REQUEST_DATE>= Date_From_Val && x.REQUEST_DATE<= Date_To_Val
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
                                               SLN = x.SLN,
                                               PAY_MODE = x.PAYMENT_METHOD
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
                                               SLN = z.SLN,
                                               PAYMENT_METHOD = z.PAY_MODE
                                           }).ToList();
                    return transactionlist;
                }
                else
                {
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
                                               SLN = x.SLN,
                                               PAY_MODE = x.PAYMENT_METHOD
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
                                               SLN = z.SLN,
                                               PAYMENT_METHOD = z.PAY_MODE
                                           }).ToList();
                    return transactionlist;
                }

               
            }

            else if (!string.IsNullOrEmpty(Super) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(MemberId) && !string.IsNullOrEmpty(status))
            {
                long Userid = long.Parse(Super);
                if (DateFrom != "" && Date_To != "" && status == "Pending")
                {
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                           join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                           where x.FROM_MEMBER == Userid && x.STATUS == status && x.REQUEST_DATE >= Date_From_Val && x.REQUEST_DATE <= Date_To_Val
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
                                               SLN = x.SLN,
                                               PAY_MODE = x.PAYMENT_METHOD
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
                                               SLN = z.SLN,
                                               PAYMENT_METHOD = z.PAY_MODE
                                           }).ToList();
                    return transactionlist;
                }
                else if (DateFrom != "" && Date_To != "" && status != "Pending")
                {
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                           join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                           where x.FROM_MEMBER == Userid && x.STATUS == status && x.APPROVAL_DATE>= Date_From_Val && x.APPROVAL_DATE<= Date_To_Val
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
                                               SLN = x.SLN,
                                               PAY_MODE = x.PAYMENT_METHOD
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
                                               SLN = z.SLN,
                                               PAYMENT_METHOD = z.PAY_MODE
                                           }).ToList();
                    return transactionlist;
                }
                else
                {
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
                                               SLN = x.SLN,
                                               PAY_MODE = x.PAYMENT_METHOD
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
                                               SLN = z.SLN,
                                               PAYMENT_METHOD = z.PAY_MODE
                                           }).ToList();
                    return transactionlist;
                }
                
            }
            else if (!string.IsNullOrEmpty(Super) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(MemberId) && string.IsNullOrEmpty(status))
            {
                long Userid = long.Parse(Super);
                if (DateFrom != "" && Date_To != "")
                {
                    string FromDATE = string.Empty;
                    string TO_DATE = string.Empty;
                    FromDATE = DateTime.Parse(DateFrom.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_From_Val = Convert.ToDateTime(FromDATE);
                    string From_TO = string.Empty;
                    TO_DATE = DateTime.Parse(Date_To.ToString()).ToString("yyyy-MM-dd");
                    DateTime Date_To_Val = Convert.ToDateTime(TO_DATE);
                    var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                           join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                           where x.TO_MEMBER == Userid && x.REQUEST_DATE>= Date_From_Val && x.REQUEST_DATE<= Date_To_Val
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
                                               SLN = x.SLN,
                                               PAY_MODE = x.PAYMENT_METHOD
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
                                               SLN = z.SLN,
                                               PAYMENT_METHOD = z.PAY_MODE
                                           }).ToList();
                    return transactionlist;
                }
                else
                {
                    var transactionlist = (from x in db.TBL_BALANCE_TRANSFER_LOGS
                                           join y in db.TBL_MASTER_MEMBER on x.FROM_MEMBER equals y.MEM_ID
                                           //where x.FROM_MEMBER == Userid
                                           where x.TO_MEMBER == Userid
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
                                               SLN = x.SLN,
                                               PAY_MODE = x.PAYMENT_METHOD
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
                                               SLN = z.SLN,
                                               PAYMENT_METHOD = z.PAY_MODE
                                           }).ToList();
                    return transactionlist;
                }
                
            }
            return null;
        }
    }
}