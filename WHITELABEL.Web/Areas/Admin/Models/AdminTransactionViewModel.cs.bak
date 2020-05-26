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
    
    public class AdminTransactionViewModel
    {
        // White Level commission Report
        public static List<TBL_ACCOUNTS> GetAdminCommissionReport(string MemberId, string status)
        {
            var db = new DBContext();
            long mem_id = long.Parse(MemberId);
            if (status == "Mobile Recharge" || status == "DMR")
            {
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where x.MEM_ID == mem_id && x.TRANSACTION_TYPE==status
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_time = x.TRANSACTION_TIME,
                                                DR_CR = x.DR_CR,
                                                Amount = x.AMOUNT,
                                                Narration = x.NARRATION,
                                                OpeningAmt = x.OPENING,
                                                Closing = x.CLOSING,
                                                CommissionAmt = x.COMM_AMT
                                            }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                            {
                                                SerialNo = index + 1,
                                                ACC_NO = z.SLN,
                                                UserName = z.MerchantName,
                                                MEMBER_TYPE = z.MemberType,
                                                TRANSACTION_TYPE = z.Trans_Type,
                                                TRANSACTION_DATE = z.Trans_Date,
                                                TRANSACTION_TIME=z.Trans_time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (status == "Requisition")
            {
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where x.MEM_ID == mem_id && x.TRANSACTION_TYPE!= "Mobile Recharge" && x.TRANSACTION_TYPE != "DMR"
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_time = x.TRANSACTION_TIME,
                                                DR_CR = x.DR_CR,
                                                Amount = x.AMOUNT,
                                                Narration = x.NARRATION,
                                                OpeningAmt = x.OPENING,
                                                Closing = x.CLOSING,
                                                CommissionAmt = x.COMM_AMT
                                            }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                            {
                                                SerialNo = index + 1,
                                                ACC_NO = z.SLN,
                                                UserName = z.MerchantName,
                                                MEMBER_TYPE = z.MemberType,
                                                TRANSACTION_TYPE = z.Trans_Type,
                                                TRANSACTION_DATE = z.Trans_Date,
                                                TRANSACTION_TIME=z.Trans_time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else
            {
                
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where x.MEM_ID == mem_id
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_time=x.TRANSACTION_TIME,
                                                DR_CR = x.DR_CR,
                                                Amount = x.AMOUNT,
                                                Narration = x.NARRATION,
                                                OpeningAmt = x.OPENING,
                                                Closing = x.CLOSING,
                                                CommissionAmt = x.COMM_AMT
                                            }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                            {
                                                SerialNo = index + 1,
                                                ACC_NO = z.SLN,
                                                UserName = z.MerchantName,
                                                MEMBER_TYPE = z.MemberType,
                                                TRANSACTION_TYPE = z.Trans_Type,
                                                TRANSACTION_DATE = z.Trans_Date,
                                                TRANSACTION_TIME=z.Trans_time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            return null;
        }
        // Super commission Report
        public static List<TBL_ACCOUNTS> GetAllSuperCommissionReport(string MemberId, string status)
        {
            var db = new DBContext();
            long mem_id = long.Parse(MemberId);
            string[] SuperMemId = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();
            if (status == "Mobile Recharge" || status == "DMR")
            {                
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where SuperMemId.Contains(x.MEM_ID.ToString()) && x.TRANSACTION_TYPE == status
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_time = x.TRANSACTION_TIME,
                                                DR_CR = x.DR_CR,
                                                Amount = x.AMOUNT,
                                                Narration = x.NARRATION,
                                                OpeningAmt = x.OPENING,
                                                Closing = x.CLOSING,
                                                CommissionAmt = x.COMM_AMT
                                            }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                            {
                                                SerialNo = index + 1,
                                                ACC_NO = z.SLN,
                                                UserName = z.MerchantName,
                                                MEMBER_TYPE = z.MemberType,
                                                TRANSACTION_TYPE = z.Trans_Type,
                                                TRANSACTION_DATE = z.Trans_Date,
                                                TRANSACTION_TIME=z.Trans_time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (status == "Requisition")
            {
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where SuperMemId.Contains(x.MEM_ID.ToString()) && x.TRANSACTION_TYPE != "Mobile Recharge" && x.TRANSACTION_TYPE!="DMR"
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_time = x.TRANSACTION_TIME,
                                                DR_CR = x.DR_CR,
                                                Amount = x.AMOUNT,
                                                Narration = x.NARRATION,
                                                OpeningAmt = x.OPENING,
                                                Closing = x.CLOSING,
                                                CommissionAmt = x.COMM_AMT
                                            }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                            {
                                                SerialNo = index + 1,
                                                ACC_NO = z.SLN,
                                                UserName = z.MerchantName,
                                                MEMBER_TYPE = z.MemberType,
                                                TRANSACTION_TYPE = z.Trans_Type,
                                                TRANSACTION_DATE = z.Trans_Date,
                                                TRANSACTION_TIME=z.Trans_time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else
            {
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where SuperMemId.Contains(x.MEM_ID.ToString())
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_time = x.TRANSACTION_TIME,
                                                DR_CR = x.DR_CR,
                                                Amount = x.AMOUNT,
                                                Narration = x.NARRATION,
                                                OpeningAmt = x.OPENING,
                                                Closing = x.CLOSING,
                                                CommissionAmt = x.COMM_AMT
                                            }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                            {
                                                SerialNo = index + 1,
                                                ACC_NO = z.SLN,
                                                UserName = z.MerchantName,
                                                MEMBER_TYPE = z.MemberType,
                                                TRANSACTION_TYPE = z.Trans_Type,
                                                TRANSACTION_DATE = z.Trans_Date,
                                                TRANSACTION_TIME=z.Trans_time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            return null;
        }
        public static List<TBL_ACCOUNTS> GetSuperCommissionReport(string MemberId, string status)
        {
            var db = new DBContext();
            long mem_id = long.Parse(MemberId);
            if (!string.IsNullOrEmpty(MemberId) && ((status == "Mobile Recharge" || status == "DMR")))
            {
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where x.MEM_ID == mem_id && x.TRANSACTION_TYPE == status
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_time = x.TRANSACTION_TIME,
                                                DR_CR = x.DR_CR,
                                                Amount = x.AMOUNT,
                                                Narration = x.NARRATION,
                                                OpeningAmt = x.OPENING,
                                                Closing = x.CLOSING,
                                                CommissionAmt = x.COMM_AMT
                                            }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                            {
                                                SerialNo = index + 1,
                                                ACC_NO = z.SLN,
                                                UserName = z.MerchantName,
                                                MEMBER_TYPE = z.MemberType,
                                                TRANSACTION_TYPE = z.Trans_Type,
                                                TRANSACTION_DATE = z.Trans_Date,
                                                TRANSACTION_TIME=z.Trans_time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (!string.IsNullOrEmpty(MemberId) && status == "Requisition")
            {
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where x.MEM_ID == mem_id && x.TRANSACTION_TYPE != "Mobile Recharge" && x.TRANSACTION_TYPE!= "DMR"
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_time = x.TRANSACTION_TIME,
                                                DR_CR = x.DR_CR,
                                                Amount = x.AMOUNT,
                                                Narration = x.NARRATION,
                                                OpeningAmt = x.OPENING,
                                                Closing = x.CLOSING,
                                                CommissionAmt = x.COMM_AMT
                                            }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                            {
                                                SerialNo = index + 1,
                                                ACC_NO = z.SLN,
                                                UserName = z.MerchantName,
                                                MEMBER_TYPE = z.MemberType,
                                                TRANSACTION_TYPE = z.Trans_Type,
                                                TRANSACTION_DATE = z.Trans_Date,
                                                TRANSACTION_TIME=z.Trans_time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (!string.IsNullOrEmpty(MemberId) && string.IsNullOrEmpty(status))
            {
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where x.MEM_ID == mem_id 
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_time = x.TRANSACTION_TIME,
                                                DR_CR = x.DR_CR,
                                                Amount = x.AMOUNT,
                                                Narration = x.NARRATION,
                                                OpeningAmt = x.OPENING,
                                                Closing = x.CLOSING,
                                                CommissionAmt = x.COMM_AMT
                                            }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                            {
                                                SerialNo = index + 1,
                                                ACC_NO = z.SLN,
                                                UserName = z.MerchantName,
                                                MEMBER_TYPE = z.MemberType,
                                                TRANSACTION_TYPE = z.Trans_Type,
                                                TRANSACTION_DATE = z.Trans_Date,
                                                TRANSACTION_TIME=z.Trans_time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            return null;
        }
        //Distributor commission Report
        public static List<TBL_ACCOUNTS> GetAllDistributorCommissionReport(string MemberId, string status)
        {
            var db = new DBContext();
            long mem_id = long.Parse(MemberId);
            string[] SuperMemId = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();
            string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(x => SuperMemId.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
            if (status == "Mobile Recharge" || status == "DMR")
            {
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where DistributorMemId.Contains(x.MEM_ID.ToString()) && x.TRANSACTION_TYPE == status
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_time = x.TRANSACTION_TIME,
                                                DR_CR = x.DR_CR,
                                                Amount = x.AMOUNT,
                                                Narration = x.NARRATION,
                                                OpeningAmt = x.OPENING,
                                                Closing = x.CLOSING,
                                                CommissionAmt = x.COMM_AMT
                                            }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                            {
                                                SerialNo = index + 1,
                                                ACC_NO = z.SLN,
                                                UserName = z.MerchantName,
                                                MEMBER_TYPE = z.MemberType,
                                                TRANSACTION_TYPE = z.Trans_Type,
                                                TRANSACTION_DATE = z.Trans_Date,
                                                TRANSACTION_TIME=z.Trans_time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (status == "Requisition")
            {
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where DistributorMemId.Contains(x.MEM_ID.ToString()) && x.TRANSACTION_TYPE != "Mobile Recharge" && x.TRANSACTION_TYPE != "DMR"
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_time = x.TRANSACTION_TIME,
                                                DR_CR = x.DR_CR,
                                                Amount = x.AMOUNT,
                                                Narration = x.NARRATION,
                                                OpeningAmt = x.OPENING,
                                                Closing = x.CLOSING,
                                                CommissionAmt = x.COMM_AMT
                                            }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                            {
                                                SerialNo = index + 1,
                                                ACC_NO = z.SLN,
                                                UserName = z.MerchantName,
                                                MEMBER_TYPE = z.MemberType,
                                                TRANSACTION_TYPE = z.Trans_Type,
                                                TRANSACTION_DATE = z.Trans_Date,
                                                TRANSACTION_TIME=z.Trans_time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else
            {
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where DistributorMemId.Contains(x.MEM_ID.ToString())
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_time = x.TRANSACTION_TIME,
                                                DR_CR = x.DR_CR,
                                                Amount = x.AMOUNT,
                                                Narration = x.NARRATION,
                                                OpeningAmt = x.OPENING,
                                                Closing = x.CLOSING,
                                                CommissionAmt = x.COMM_AMT
                                            }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                            {
                                                SerialNo = index + 1,
                                                ACC_NO = z.SLN,
                                                UserName = z.MerchantName,
                                                MEMBER_TYPE = z.MemberType,
                                                TRANSACTION_TYPE = z.Trans_Type,
                                                TRANSACTION_DATE = z.Trans_Date,
                                                TRANSACTION_TIME=z.Trans_time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            
        }
        public static List<TBL_ACCOUNTS> GetDistributorCommissionReport(string Super, string MemberId, string status)
        {
            var db = new DBContext();
            //long mem_id = long.Parse(MemberId);
            if (!string.IsNullOrEmpty(Super) && !string.IsNullOrEmpty(MemberId) && ((status == "Mobile Recharge" || status == "DMR")))
            {
                long mem_id = long.Parse(MemberId);
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where x.MEM_ID == mem_id && x.TRANSACTION_TYPE == status
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_time = x.TRANSACTION_TIME,
                                                DR_CR = x.DR_CR,
                                                Amount = x.AMOUNT,
                                                Narration = x.NARRATION,
                                                OpeningAmt = x.OPENING,
                                                Closing = x.CLOSING,
                                                CommissionAmt = x.COMM_AMT
                                            }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                            {
                                                SerialNo = index + 1,
                                                ACC_NO = z.SLN,
                                                UserName = z.MerchantName,
                                                MEMBER_TYPE = z.MemberType,
                                                TRANSACTION_TYPE = z.Trans_Type,
                                                TRANSACTION_DATE = z.Trans_Date,
                                                TRANSACTION_TIME=z.Trans_time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (!string.IsNullOrEmpty(Super) && !string.IsNullOrEmpty(MemberId) && status == "Requisition")
            {
                long mem_id = long.Parse(MemberId);
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where x.MEM_ID == mem_id && x.TRANSACTION_TYPE != "Mobile Recharge" && x.TRANSACTION_TYPE != "DMR"
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_time = x.TRANSACTION_TIME,
                                                DR_CR = x.DR_CR,
                                                Amount = x.AMOUNT,
                                                Narration = x.NARRATION,
                                                OpeningAmt = x.OPENING,
                                                Closing = x.CLOSING,
                                                CommissionAmt = x.COMM_AMT
                                            }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                            {
                                                SerialNo = index + 1,
                                                ACC_NO = z.SLN,
                                                UserName = z.MerchantName,
                                                MEMBER_TYPE = z.MemberType,
                                                TRANSACTION_TYPE = z.Trans_Type,
                                                TRANSACTION_DATE = z.Trans_Date,
                                                TRANSACTION_TIME=z.Trans_time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (!string.IsNullOrEmpty(Super) && !string.IsNullOrEmpty(MemberId) && string.IsNullOrEmpty(status))
            {
                long mem_id = long.Parse(MemberId);
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where x.MEM_ID == mem_id
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_time = x.TRANSACTION_TIME,
                                                DR_CR = x.DR_CR,
                                                Amount = x.AMOUNT,
                                                Narration = x.NARRATION,
                                                OpeningAmt = x.OPENING,
                                                Closing = x.CLOSING,
                                                CommissionAmt = x.COMM_AMT
                                            }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                            {
                                                SerialNo = index + 1,
                                                ACC_NO = z.SLN,
                                                UserName = z.MerchantName,
                                                MEMBER_TYPE = z.MemberType,
                                                TRANSACTION_TYPE = z.Trans_Type,
                                                TRANSACTION_DATE = z.Trans_Date,
                                                TRANSACTION_TIME=z.Trans_time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }

            else if (!string.IsNullOrEmpty(Super) && string.IsNullOrEmpty(MemberId) && ((status == "Mobile Recharge" || status == "DMR")))
            {
                long mem_id = long.Parse(Super);
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where x.MEM_ID == mem_id && x.TRANSACTION_TYPE == status
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_time = x.TRANSACTION_TIME,
                                                DR_CR = x.DR_CR,
                                                Amount = x.AMOUNT,
                                                Narration = x.NARRATION,
                                                OpeningAmt = x.OPENING,
                                                Closing = x.CLOSING,
                                                CommissionAmt = x.COMM_AMT
                                            }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                            {
                                                SerialNo = index + 1,
                                                ACC_NO = z.SLN,
                                                UserName = z.MerchantName,
                                                MEMBER_TYPE = z.MemberType,
                                                TRANSACTION_TYPE = z.Trans_Type,
                                                TRANSACTION_DATE = z.Trans_Date,
                                                TRANSACTION_TIME=z.Trans_time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (!string.IsNullOrEmpty(Super) && string.IsNullOrEmpty(MemberId) && status == "Requisition")
            {
                long mem_id = long.Parse(Super);
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where x.MEM_ID == mem_id && x.TRANSACTION_TYPE != "Mobile Recharge" && x.TRANSACTION_TYPE != "DMR"
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_time = x.TRANSACTION_TIME,
                                                DR_CR = x.DR_CR,
                                                Amount = x.AMOUNT,
                                                Narration = x.NARRATION,
                                                OpeningAmt = x.OPENING,
                                                Closing = x.CLOSING,
                                                CommissionAmt = x.COMM_AMT
                                            }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                            {
                                                SerialNo = index + 1,
                                                ACC_NO = z.SLN,
                                                UserName = z.MerchantName,
                                                MEMBER_TYPE = z.MemberType,
                                                TRANSACTION_TYPE = z.Trans_Type,
                                                TRANSACTION_DATE = z.Trans_Date,
                                                TRANSACTION_TIME=z.Trans_time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (!string.IsNullOrEmpty(Super) && string.IsNullOrEmpty(MemberId) && string.IsNullOrEmpty(status))
            {
                long mem_id = long.Parse(Super);
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where x.MEM_ID == mem_id
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_time = x.TRANSACTION_TIME,
                                                DR_CR = x.DR_CR,
                                                Amount = x.AMOUNT,
                                                Narration = x.NARRATION,
                                                OpeningAmt = x.OPENING,
                                                Closing = x.CLOSING,
                                                CommissionAmt = x.COMM_AMT
                                            }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                            {
                                                SerialNo = index + 1,
                                                ACC_NO = z.SLN,
                                                UserName = z.MerchantName,
                                                MEMBER_TYPE = z.MemberType,
                                                TRANSACTION_TYPE = z.Trans_Type,
                                                TRANSACTION_DATE = z.Trans_Date,
                                                TRANSACTION_TIME=z.Trans_time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            return null;
        }
        ////Merchant commission Report

        public static List<TBL_ACCOUNTS> GetAllMerchantCommissionReport(string MemberId, string status)
        {

            var db = new DBContext();
            long mem_id = long.Parse(MemberId);
            string[] SuperMemId = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();
            string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(x => SuperMemId.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
            string[] MerchantMemId = db.TBL_MASTER_MEMBER.Where(x => DistributorMemId.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
            if (status == "Mobile Recharge" || status == "DMR")
            {
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where MerchantMemId.Contains(x.MEM_ID.ToString()) && x.TRANSACTION_TYPE == status
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                DR_CR = x.DR_CR,
                                                Amount = x.AMOUNT,
                                                Narration = x.NARRATION,
                                                OpeningAmt = x.OPENING,
                                                Closing = x.CLOSING,
                                                CommissionAmt = x.COMM_AMT
                                            }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                            {
                                                SerialNo = index + 1,
                                                ACC_NO = z.SLN,
                                                UserName = z.MerchantName,
                                                MEMBER_TYPE = z.MemberType,
                                                TRANSACTION_TYPE = z.Trans_Type,
                                                TRANSACTION_DATE = z.Trans_Date,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (status == "Requisition")
            {
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where MerchantMemId.Contains(x.MEM_ID.ToString()) && x.TRANSACTION_TYPE != "Mobile Recharge" && x.TRANSACTION_TYPE != "DMR"
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                DR_CR = x.DR_CR,
                                                Amount = x.AMOUNT,
                                                Narration = x.NARRATION,
                                                OpeningAmt = x.OPENING,
                                                Closing = x.CLOSING,
                                                CommissionAmt = x.COMM_AMT
                                            }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                            {
                                                SerialNo = index + 1,
                                                ACC_NO = z.SLN,
                                                UserName = z.MerchantName,
                                                MEMBER_TYPE = z.MemberType,
                                                TRANSACTION_TYPE = z.Trans_Type,
                                                TRANSACTION_DATE = z.Trans_Date,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else
            {
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where MerchantMemId.Contains(x.MEM_ID.ToString())
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_time = x.TRANSACTION_TIME,
                                                DR_CR = x.DR_CR,
                                                Amount = x.AMOUNT,
                                                Narration = x.NARRATION,
                                                OpeningAmt = x.OPENING,
                                                Closing = x.CLOSING,
                                                CommissionAmt = x.COMM_AMT
                                            }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                            {
                                                SerialNo = index + 1,
                                                ACC_NO = z.SLN,
                                                UserName = z.MerchantName,
                                                MEMBER_TYPE = z.MemberType,
                                                TRANSACTION_TYPE = z.Trans_Type,
                                                TRANSACTION_DATE = z.Trans_Date,
                                                TRANSACTION_TIME=z.Trans_time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
        }

        public static List<TBL_ACCOUNTS> GetMerchantCommissionReport(string Super,string Distributor, string MemberId, string status)
        {
            var db = new DBContext();
            
            if (!string.IsNullOrEmpty(Super) && !string.IsNullOrEmpty(Distributor) && !string.IsNullOrEmpty(MemberId) && ((status == "Mobile Recharge" || status == "DMR")))
            {
                long mem_id = long.Parse(MemberId);
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where x.MEM_ID == mem_id && x.TRANSACTION_TYPE == status
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_time = x.TRANSACTION_TIME,
                                                DR_CR = x.DR_CR,
                                                Amount = x.AMOUNT,
                                                Narration = x.NARRATION,
                                                OpeningAmt = x.OPENING,
                                                Closing = x.CLOSING,
                                                CommissionAmt = x.COMM_AMT
                                            }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                            {
                                                SerialNo = index + 1,
                                                ACC_NO = z.SLN,
                                                UserName = z.MerchantName,
                                                MEMBER_TYPE = z.MemberType,
                                                TRANSACTION_TYPE = z.Trans_Type,
                                                TRANSACTION_DATE = z.Trans_Date,
                                                TRANSACTION_TIME=z.Trans_time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (!string.IsNullOrEmpty(Super) && !string.IsNullOrEmpty(Distributor) && !string.IsNullOrEmpty(MemberId) && status == "Requisition")
            {
                long mem_id = long.Parse(MemberId);
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where x.MEM_ID == mem_id && x.TRANSACTION_TYPE != "Mobile Recharge" && x.TRANSACTION_TYPE != "DMR"
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_time = x.TRANSACTION_TIME,
                                                DR_CR = x.DR_CR,
                                                Amount = x.AMOUNT,
                                                Narration = x.NARRATION,
                                                OpeningAmt = x.OPENING,
                                                Closing = x.CLOSING,
                                                CommissionAmt = x.COMM_AMT
                                            }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                            {
                                                SerialNo = index + 1,
                                                ACC_NO = z.SLN,
                                                UserName = z.MerchantName,
                                                MEMBER_TYPE = z.MemberType,
                                                TRANSACTION_TYPE = z.Trans_Type,
                                                TRANSACTION_DATE = z.Trans_Date,
                                                TRANSACTION_TIME=z.Trans_time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (!string.IsNullOrEmpty(Super) && !string.IsNullOrEmpty(Distributor) && !string.IsNullOrEmpty(MemberId) && string.IsNullOrEmpty(status))
            {
                long mem_id = long.Parse(MemberId);
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where x.MEM_ID == mem_id
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_time = x.TRANSACTION_TIME,
                                                DR_CR = x.DR_CR,
                                                Amount = x.AMOUNT,
                                                Narration = x.NARRATION,
                                                OpeningAmt = x.OPENING,
                                                Closing = x.CLOSING,
                                                CommissionAmt = x.COMM_AMT
                                            }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                            {
                                                SerialNo = index + 1,
                                                ACC_NO = z.SLN,
                                                UserName = z.MerchantName,
                                                MEMBER_TYPE = z.MemberType,
                                                TRANSACTION_TYPE = z.Trans_Type,
                                                TRANSACTION_DATE = z.Trans_Date,
                                                TRANSACTION_TIME=z.Trans_time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }

            else if (!string.IsNullOrEmpty(Super) && !string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(MemberId) && ((status == "Mobile Recharge" || status == "DMR")))
            {
                long mem_id = long.Parse(Distributor);
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where x.MEM_ID == mem_id && x.TRANSACTION_TYPE == status
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_time = x.TRANSACTION_TIME,
                                                DR_CR = x.DR_CR,
                                                Amount = x.AMOUNT,
                                                Narration = x.NARRATION,
                                                OpeningAmt = x.OPENING,
                                                Closing = x.CLOSING,
                                                CommissionAmt = x.COMM_AMT
                                            }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                            {
                                                SerialNo = index + 1,
                                                ACC_NO = z.SLN,
                                                UserName = z.MerchantName,
                                                MEMBER_TYPE = z.MemberType,
                                                TRANSACTION_TYPE = z.Trans_Type,
                                                TRANSACTION_DATE = z.Trans_Date,
                                                TRANSACTION_TIME=z.Trans_time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (!string.IsNullOrEmpty(Super) && !string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(MemberId) && status == "Requisition")
            {
                long mem_id = long.Parse(Distributor);
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where x.MEM_ID == mem_id && x.TRANSACTION_TYPE != "Mobile Recharge" && x.TRANSACTION_TYPE != "DMR"
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_time = x.TRANSACTION_TIME,
                                                DR_CR = x.DR_CR,
                                                Amount = x.AMOUNT,
                                                Narration = x.NARRATION,
                                                OpeningAmt = x.OPENING,
                                                Closing = x.CLOSING,
                                                CommissionAmt = x.COMM_AMT
                                            }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                            {
                                                SerialNo = index + 1,
                                                ACC_NO = z.SLN,
                                                UserName = z.MerchantName,
                                                MEMBER_TYPE = z.MemberType,
                                                TRANSACTION_TYPE = z.Trans_Type,
                                                TRANSACTION_DATE = z.Trans_Date,
                                                TRANSACTION_TIME=z.Trans_time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (!string.IsNullOrEmpty(Super) && !string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(MemberId) && string.IsNullOrEmpty(status))
            {
                long mem_id = long.Parse(Distributor);
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where x.MEM_ID == mem_id
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_time = x.TRANSACTION_TIME,
                                                DR_CR = x.DR_CR,
                                                Amount = x.AMOUNT,
                                                Narration = x.NARRATION,
                                                OpeningAmt = x.OPENING,
                                                Closing = x.CLOSING,
                                                CommissionAmt = x.COMM_AMT
                                            }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                            {
                                                SerialNo = index + 1,
                                                ACC_NO = z.SLN,
                                                UserName = z.MerchantName,
                                                MEMBER_TYPE = z.MemberType,
                                                TRANSACTION_TYPE = z.Trans_Type,
                                                TRANSACTION_DATE = z.Trans_Date,
                                                TRANSACTION_TIME=z.Trans_time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }

            else if (!string.IsNullOrEmpty(Super) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(MemberId) && ((status == "Mobile Recharge" || status == "DMR")))
            {
                long mem_id = long.Parse(Super);
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where x.MEM_ID == mem_id && x.TRANSACTION_TYPE == status
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_time = x.TRANSACTION_TIME,
                                                DR_CR = x.DR_CR,
                                                Amount = x.AMOUNT,
                                                Narration = x.NARRATION,
                                                OpeningAmt = x.OPENING,
                                                Closing = x.CLOSING,
                                                CommissionAmt = x.COMM_AMT
                                            }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                            {
                                                SerialNo = index + 1,
                                                ACC_NO = z.SLN,
                                                UserName = z.MerchantName,
                                                MEMBER_TYPE = z.MemberType,
                                                TRANSACTION_TYPE = z.Trans_Type,
                                                TRANSACTION_DATE = z.Trans_Date,
                                                TRANSACTION_TIME=z.Trans_time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (!string.IsNullOrEmpty(Super) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(MemberId) && status == "Requisition")
            {
                long mem_id = long.Parse(Super);
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where x.MEM_ID == mem_id && x.TRANSACTION_TYPE != "Mobile Recharge" && x.TRANSACTION_TYPE != "DMR"
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_time = x.TRANSACTION_TIME,
                                                DR_CR = x.DR_CR,
                                                Amount = x.AMOUNT,
                                                Narration = x.NARRATION,
                                                OpeningAmt = x.OPENING,
                                                Closing = x.CLOSING,
                                                CommissionAmt = x.COMM_AMT
                                            }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                            {
                                                SerialNo = index + 1,
                                                ACC_NO = z.SLN,
                                                UserName = z.MerchantName,
                                                MEMBER_TYPE = z.MemberType,
                                                TRANSACTION_TYPE = z.Trans_Type,
                                                TRANSACTION_DATE = z.Trans_Date,
                                                TRANSACTION_TIME=z.Trans_time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (!string.IsNullOrEmpty(Super) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(MemberId) && string.IsNullOrEmpty(status))
            {
                long mem_id = long.Parse(Super);
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where x.MEM_ID == mem_id
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_time = x.TRANSACTION_TIME,
                                                DR_CR = x.DR_CR,
                                                Amount = x.AMOUNT,
                                                Narration = x.NARRATION,
                                                OpeningAmt = x.OPENING,
                                                Closing = x.CLOSING,
                                                CommissionAmt = x.COMM_AMT
                                            }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
                                            {
                                                SerialNo = index + 1,
                                                ACC_NO = z.SLN,
                                                UserName = z.MerchantName,
                                                MEMBER_TYPE = z.MemberType,
                                                TRANSACTION_TYPE = z.Trans_Type,
                                                TRANSACTION_DATE = z.Trans_Date,
                                                TRANSACTION_TIME=z.Trans_time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            return null;
        }

    
    }
}