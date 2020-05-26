namespace WHITELABEL.Web.Areas.Super.Models
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
    public class SuperCommissionTransactionModel
    {
        public static List<TBL_ACCOUNTS> GetAllSuperCommissionReport(string MemberId, string status)
        {
            var db = new DBContext();
            long mem_id = long.Parse(MemberId);
            if (status == "Mobile Recharge" || status == "DMR")
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
                                            where x.MEM_ID == mem_id && x.TRANSACTION_TYPE != "Mobile Recharge" && x.TRANSACTION_TYPE != "DMR"
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
                                            where x.MEM_ID == mem_id
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
            return null;
        }

        public static List<TBL_ACCOUNTS> GetAllSuperDistributorCommissionReport(string MemberId, string status)
        {
            var db = new DBContext();
            long mem_id = long.Parse(MemberId);
            string[] SuperMemId = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();
            string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(x => SuperMemId.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
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
                                            where SuperMemId.Contains(x.MEM_ID.ToString()) && x.TRANSACTION_TYPE != "Mobile Recharge" && x.TRANSACTION_TYPE != "DMR"
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
                                            where SuperMemId.Contains(x.MEM_ID.ToString())
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

        }
        public static List<TBL_ACCOUNTS> GetSuperDistributorCommissionReport(string MemberId, string status)
        {
            var db = new DBContext();
            //long mem_id = long.Parse(MemberId);
            if (!string.IsNullOrEmpty(MemberId) && ((status == "Mobile Recharge" || status == "DMR")))
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
            else if (!string.IsNullOrEmpty(MemberId) && status == "Requisition")
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
            else if (!string.IsNullOrEmpty(MemberId) && string.IsNullOrEmpty(status))
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
            return null;
        }
        public static List<TBL_ACCOUNTS> GetAllSuperMerchantCommissionReport(string MemberId, string status)
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
                                            where DistributorMemId.Contains(x.MEM_ID.ToString()) && x.TRANSACTION_TYPE == status
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
                                            where DistributorMemId.Contains(x.MEM_ID.ToString()) && x.TRANSACTION_TYPE != "Mobile Recharge" && x.TRANSACTION_TYPE != "DMR"
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
                                            where DistributorMemId.Contains(x.MEM_ID.ToString())
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
        }

        public static List<TBL_ACCOUNTS> GetSuperMerchantCommissionReport(string MemberId, string status)
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
            else if (!string.IsNullOrEmpty(MemberId) && status == "Requisition")
            {
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
            return null;
        }
    }
}