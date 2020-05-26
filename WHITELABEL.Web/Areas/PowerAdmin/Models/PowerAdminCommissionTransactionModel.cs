namespace WHITELABEL.Web.Areas.PowerAdmin.Models
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
    using WHITELABEL.Web.Areas.PowerAdmin.Models;
    using WHITELABEL.Web.ServiceApi.RECHARGE.PORTIQUE;
    using static WHITELABEL.Web.Helper.InstantPayApi;
    using NonFactors.Mvc.Grid;
    using OfficeOpenXml;
    using System.Threading.Tasks;
    using System.Data.Entity;
    using log4net;
    public class PowerAdminCommissionTransactionModel
    {
        // White Level commission Report
        public static List<TBL_ACCOUNTS> GetPowerAdminCommissionReport(string MemberId, string status)
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
                                                Trans_Time=x.TRANSACTION_TIME,
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
                                                TRANSACTION_TIME=z.Trans_Time,
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
                                                Trans_Time=x.TRANSACTION_TIME,
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
                                                TRANSACTION_TIME=z.Trans_Time,
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
        public static List<TBL_ACCOUNTS> GetAllPowerAdminCommissionReport(string MemberId, string status)
        {
            var db = new DBContext();
            long mem_id = long.Parse(MemberId);
            string[] WhiteLevelMemId = db.TBL_MASTER_MEMBER.Where(w => w.UNDER_WHITE_LEVEL == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();
            if (status == "Mobile Recharge" || status == "DMR")
            {
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where WhiteLevelMemId.Contains(x.MEM_ID.ToString()) && x.TRANSACTION_TYPE == status
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_Time=x.TRANSACTION_TIME,
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
                                                TRANSACTION_TIME=z.Trans_Time,
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
                                            where WhiteLevelMemId.Contains(x.MEM_ID.ToString()) && x.TRANSACTION_TYPE != "Mobile Recharge" && x.TRANSACTION_TYPE != "DMR"
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
                                            where WhiteLevelMemId.Contains(x.MEM_ID.ToString())
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
        // Super commission Report
        public static List<TBL_ACCOUNTS> GetAllPoweradminSuperCommissionReport(string MemberId, string status)
        {
            var db = new DBContext();
            long mem_id = long.Parse(MemberId);
            string[] searchTest = db.TBL_MASTER_MEMBER.Where(w => w.UNDER_WHITE_LEVEL == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();
            string[] SuperMemberIdTest = db.TBL_MASTER_MEMBER.Where(x => searchTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
            //string[] SuperMemId = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();
            if (status == "Mobile Recharge" || status == "DMR")
            {
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where SuperMemberIdTest.Contains(x.MEM_ID.ToString()) && x.TRANSACTION_TYPE == status
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
                                            where SuperMemberIdTest.Contains(x.MEM_ID.ToString()) && x.TRANSACTION_TYPE != "Mobile Recharge" && x.TRANSACTION_TYPE != "DMR"
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
                                            where SuperMemberIdTest.Contains(x.MEM_ID.ToString())
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_Time=x.TRANSACTION_TIME,
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
                                                TRANSACTION_TIME=z.Trans_Time,
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
        public static List<TBL_ACCOUNTS> GetPoweradminSuperCommissionReport(string whitelevel, string MemberId, string status)
        {
            var db = new DBContext();
            
            if (!string.IsNullOrEmpty(whitelevel) && !string.IsNullOrEmpty(MemberId) && ((status == "Mobile Recharge" || status == "DMR")))
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
                                                Trans_Time=x.TRANSACTION_TIME,
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
                                                TRANSACTION_TIME=z.Trans_Time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (!string.IsNullOrEmpty(whitelevel) && !string.IsNullOrEmpty(MemberId) && status == "Requisition")
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
                                                Trans_Time = x.TRANSACTION_TIME,
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
                                                TRANSACTION_TIME=z.Trans_Time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (!string.IsNullOrEmpty(whitelevel) && !string.IsNullOrEmpty(MemberId) && string.IsNullOrEmpty(status))
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
                                                Trans_Time = x.TRANSACTION_TIME,
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
                                                TRANSACTION_TIME=z.Trans_Time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (!string.IsNullOrEmpty(whitelevel) && string.IsNullOrEmpty(MemberId) && ((status == "Mobile Recharge" || status == "DMR")))
            {

                long mem_id = long.Parse(whitelevel);
                string[] WhiteLevelMemId = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where WhiteLevelMemId.Contains(x.MEM_ID.ToString()) && x.TRANSACTION_TYPE == status
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_Time = x.TRANSACTION_TIME,
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
                                                TRANSACTION_TIME=z.Trans_Time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (!string.IsNullOrEmpty(whitelevel) && string.IsNullOrEmpty(MemberId) && status == "Requisition")
            {
                long mem_id = long.Parse(whitelevel);
                string[] WhiteLevelMemId = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where WhiteLevelMemId.Contains(x.MEM_ID.ToString()) && x.TRANSACTION_TYPE != "Mobile Recharge" && x.TRANSACTION_TYPE != "DMR"
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_Time = x.TRANSACTION_TIME,
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
                                                TRANSACTION_TIME=z.Trans_Time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (!string.IsNullOrEmpty(whitelevel) && string.IsNullOrEmpty(MemberId) && string.IsNullOrEmpty(status))
            {
                long mem_id = long.Parse(whitelevel);
                string[] WhiteLevelMemId = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where WhiteLevelMemId.Contains(x.MEM_ID.ToString())
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_Time = x.TRANSACTION_TIME,
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
                                                TRANSACTION_TIME=z.Trans_Time,
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
        public static List<TBL_ACCOUNTS> GetAllPoweradminDistributorCommissionReport(string Whitelevel,string Supervalue, string MemberId, string status)
        {
            var db = new DBContext();
            long mem_id = long.Parse(MemberId);
            string[] searchTest = db.TBL_MASTER_MEMBER.Where(w => w.UNDER_WHITE_LEVEL == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();
            string[] SuperMemberIdTest = db.TBL_MASTER_MEMBER.Where(x => searchTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
            //string[] SuperMemId = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();
            string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(x => SuperMemberIdTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
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
                                                Trans_Time = x.TRANSACTION_TIME,
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
                                                TRANSACTION_TIME=z.Trans_Time,
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
                                                Trans_Time = x.TRANSACTION_TIME,
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
                                                TRANSACTION_TIME=z.Trans_Time,
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
                                                Trans_Time = x.TRANSACTION_TIME,
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
                                                TRANSACTION_TIME=z.Trans_Time,
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
        public static List<TBL_ACCOUNTS> GetPoweradminDistributorCommissionReport(string Whitelevel, string Supervalue, string MemberId, string status)
        {
            var db = new DBContext();
            //long mem_id = long.Parse(MemberId);
            if (!string.IsNullOrEmpty(Whitelevel) && string.IsNullOrEmpty(Supervalue) && string.IsNullOrEmpty(MemberId) && ((status == "Mobile Recharge" || status == "DMR")))
            {
                long mem_id = long.Parse(Whitelevel);
                string[] searchTest = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();                
                string[] SuperMemberIdTest = db.TBL_MASTER_MEMBER.Where(x => searchTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                //string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(x => SuperMemberIdTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where SuperMemberIdTest.Contains(x.MEM_ID.ToString()) && x.TRANSACTION_TYPE == status
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_Time = x.TRANSACTION_TIME,
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
                                                TRANSACTION_TIME=z.Trans_Time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (!string.IsNullOrEmpty(Whitelevel) && string.IsNullOrEmpty(Supervalue) && string.IsNullOrEmpty(MemberId) && status == "Requisition")
            {
                long mem_id = long.Parse(Whitelevel);
                string[] searchTest = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();
                string[] SuperMemberIdTest = db.TBL_MASTER_MEMBER.Where(x => searchTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where SuperMemberIdTest.Contains(x.MEM_ID.ToString()) && x.TRANSACTION_TYPE != "Mobile Recharge" && x.TRANSACTION_TYPE != "DMR"
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_Time = x.TRANSACTION_TIME,
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
                                                TRANSACTION_TIME=z.Trans_Time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (!string.IsNullOrEmpty(Whitelevel) && string.IsNullOrEmpty(Supervalue) && string.IsNullOrEmpty(MemberId) && string.IsNullOrEmpty(status))
            {
                long mem_id = long.Parse(Whitelevel);
                string[] searchTest = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();
                string[] SuperMemberIdTest = db.TBL_MASTER_MEMBER.Where(x => searchTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where SuperMemberIdTest.Contains(x.MEM_ID.ToString())
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_Time = x.TRANSACTION_TIME,
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
                                                TRANSACTION_TIME=z.Trans_Time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (!string.IsNullOrEmpty(Whitelevel) && !string.IsNullOrEmpty(Supervalue) && string.IsNullOrEmpty(MemberId) && ((status == "Mobile Recharge" || status == "DMR")))
            {
                long mem_id = long.Parse(Supervalue);
                string[] searchTest = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();
                //string[] SuperMemberIdTest = db.TBL_MASTER_MEMBER.Where(x => searchTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                //string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(x => SuperMemberIdTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where searchTest.Contains(x.MEM_ID.ToString()) && x.TRANSACTION_TYPE == status
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_Time = x.TRANSACTION_TIME,
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
                                                TRANSACTION_TIME=z.Trans_Time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (!string.IsNullOrEmpty(Whitelevel) && !string.IsNullOrEmpty(Supervalue) && string.IsNullOrEmpty(MemberId) && status == "Requisition")
            {
                long mem_id = long.Parse(Supervalue);
                string[] searchTest = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();
                //string[] SuperMemberIdTest = db.TBL_MASTER_MEMBER.Where(x => searchTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where searchTest.Contains(x.MEM_ID.ToString()) && x.TRANSACTION_TYPE != "Mobile Recharge" && x.TRANSACTION_TYPE != "DMR"
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_Time = x.TRANSACTION_TIME,
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
            else if (!string.IsNullOrEmpty(Whitelevel) && !string.IsNullOrEmpty(Supervalue) && string.IsNullOrEmpty(MemberId) && string.IsNullOrEmpty(status))
            {
                long mem_id = long.Parse(Supervalue);
                string[] searchTest = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();
                //string[] SuperMemberIdTest = db.TBL_MASTER_MEMBER.Where(x => searchTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                var transactionlistvalue = (from x in db.TBL_ACCOUNTS
                                            join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
                                            where searchTest.Contains(x.MEM_ID.ToString())
                                            select new
                                            {
                                                SLN = x.ACC_NO,
                                                MerchantName = y.UName,
                                                MemberType = x.MEMBER_TYPE,
                                                Trans_Type = x.TRANSACTION_TYPE,
                                                Trans_Date = x.TRANSACTION_DATE,
                                                Trans_Time = x.TRANSACTION_TIME,
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
                                                TRANSACTION_TIME=z.Trans_Time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }

            else if (!string.IsNullOrEmpty(Whitelevel) && !string.IsNullOrEmpty(Supervalue) && !string.IsNullOrEmpty(MemberId) && ((status == "Mobile Recharge" || status == "DMR")))
            {
                long mem_id = long.Parse(MemberId);
                string[] searchTest = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();
                //string[] SuperMemberIdTest = db.TBL_MASTER_MEMBER.Where(x => searchTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
                //string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(x => SuperMemberIdTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
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
                                                Trans_Time = x.TRANSACTION_TIME,
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
                                                TRANSACTION_TIME = z.Trans_Time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (!string.IsNullOrEmpty(Whitelevel) && !string.IsNullOrEmpty(Supervalue) && !string.IsNullOrEmpty(MemberId) && status == "Requisition")
            {
                long mem_id = long.Parse(Supervalue);
                string[] searchTest = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();
                //string[] SuperMemberIdTest = db.TBL_MASTER_MEMBER.Where(x => searchTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
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
                                                Trans_Time = x.TRANSACTION_TIME,
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
                                                TRANSACTION_TIME=z.Trans_Time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (!string.IsNullOrEmpty(Whitelevel) && !string.IsNullOrEmpty(Supervalue) && !string.IsNullOrEmpty(MemberId) && string.IsNullOrEmpty(status))
            {
                long mem_id = long.Parse(Supervalue);
                string[] searchTest = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();
                //string[] SuperMemberIdTest = db.TBL_MASTER_MEMBER.Where(x => searchTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
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
                                                Trans_Time = x.TRANSACTION_TIME,
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
                                                TRANSACTION_TIME=z.Trans_Time,
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
        public static List<TBL_ACCOUNTS> GetPoweradminAllMerchantCommissionReport(string MemberId, string status)
        {

            var db = new DBContext();
            long mem_id = long.Parse(MemberId);
            string[] searchTest = db.TBL_MASTER_MEMBER.Where(w => w.UNDER_WHITE_LEVEL == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();
            string[] SuperMemberIdTest = db.TBL_MASTER_MEMBER.Where(x => searchTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
            //string[] SuperMemId = db.TBL_MASTER_MEMBER.Where(w => w.INTRODUCER == mem_id).Select(a => a.MEM_ID.ToString()).ToArray();
            string[] DistributorMemId = db.TBL_MASTER_MEMBER.Where(x => SuperMemberIdTest.Contains(x.INTRODUCER.ToString())).Select(a => a.MEM_ID.ToString()).ToArray();
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
                                                Trans_Time = x.TRANSACTION_TIME,
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
                                                TRANSACTION_TIME=z.Trans_Time,
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
                                                Trans_Time = x.TRANSACTION_TIME,
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
                                                TRANSACTION_TIME=z.Trans_Time,
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
                                                Trans_Time = x.TRANSACTION_TIME,
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
                                                TRANSACTION_TIME=z.Trans_Time,
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
        public static List<TBL_ACCOUNTS> GetPoweradminMerchantCommissionReport(string Whitelevel, string Supervalue, string Distributor, string MemberId, string status)
        {
            var db = new DBContext();
            if (!string.IsNullOrEmpty(Whitelevel) && string.IsNullOrEmpty(Supervalue) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(MemberId) && ((status == "Mobile Recharge" || status == "DMR")))
            {
                long mem_id = long.Parse(Whitelevel);
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
                                                Trans_Time = x.TRANSACTION_TIME,
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
                                                TRANSACTION_TIME=z.Trans_Time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (!string.IsNullOrEmpty(Whitelevel) && string.IsNullOrEmpty(Supervalue) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(MemberId) && status == "Requisition")
            {
                long mem_id = long.Parse(Whitelevel);
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
                                                Trans_Time = x.TRANSACTION_TIME,
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
                                                TRANSACTION_TIME=z.Trans_Time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (!string.IsNullOrEmpty(Whitelevel) && string.IsNullOrEmpty(Supervalue) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(MemberId) && string.IsNullOrEmpty(status))
            {
                long mem_id = long.Parse(Whitelevel);
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
                                                Trans_Time = x.TRANSACTION_TIME,
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
                                                TRANSACTION_TIME=z.Trans_Time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }

            else if (!string.IsNullOrEmpty(Whitelevel) && !string.IsNullOrEmpty(Supervalue) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(MemberId) && ((status == "Mobile Recharge" || status == "DMR")))
            {
                long mem_id = long.Parse(Supervalue);
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
                                                Trans_Time = x.TRANSACTION_TIME,
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
                                                TRANSACTION_TIME=z.Trans_Time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (!string.IsNullOrEmpty(Whitelevel) && !string.IsNullOrEmpty(Supervalue) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(MemberId) && status == "Requisition")
            {
                long mem_id = long.Parse(Supervalue);
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
                                                Trans_Time = x.TRANSACTION_TIME,
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
                                                TRANSACTION_TIME = z.Trans_Time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (!string.IsNullOrEmpty(Whitelevel) && !string.IsNullOrEmpty(Supervalue) && string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(MemberId) && string.IsNullOrEmpty(status))
            {
                long mem_id = long.Parse(Supervalue);
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
                                                Trans_Time = x.TRANSACTION_TIME,
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
                                                TRANSACTION_TIME=z.Trans_Time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }

            else if (!string.IsNullOrEmpty(Whitelevel) && !string.IsNullOrEmpty(Supervalue) && !string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(MemberId) && ((status == "Mobile Recharge" || status == "DMR")))
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
                                                Trans_Time = x.TRANSACTION_TIME,
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
                                                TRANSACTION_TIME=z.Trans_Time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (!string.IsNullOrEmpty(Whitelevel) && !string.IsNullOrEmpty(Supervalue) && !string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(MemberId) && status == "Requisition")
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
                                                Trans_Time = x.TRANSACTION_TIME,
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
                                                TRANSACTION_TIME=z.Trans_Time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (!string.IsNullOrEmpty(Whitelevel) && !string.IsNullOrEmpty(Supervalue) && !string.IsNullOrEmpty(Distributor) && string.IsNullOrEmpty(MemberId) && string.IsNullOrEmpty(status))
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
                                                Trans_Time = x.TRANSACTION_TIME,
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
                                                TRANSACTION_TIME =z.Trans_Time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }

            else if (!string.IsNullOrEmpty(Whitelevel) && !string.IsNullOrEmpty(Supervalue) && !string.IsNullOrEmpty(Distributor) && !string.IsNullOrEmpty(MemberId) && ((status == "Mobile Recharge" || status == "DMR")))
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
                                                Trans_Time = x.TRANSACTION_TIME,
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
                                                TRANSACTION_TIME=z.Trans_Time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (!string.IsNullOrEmpty(Whitelevel) && !string.IsNullOrEmpty(Supervalue) && !string.IsNullOrEmpty(Distributor) && !string.IsNullOrEmpty(MemberId) && status == "Requisition")
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
                                                Trans_Time = x.TRANSACTION_TIME,
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
                                                TRANSACTION_TIME=z.Trans_Time,
                                                DR_CR = z.DR_CR,
                                                AMOUNT = z.Amount,
                                                NARRATION = z.Narration,
                                                OPENING = z.OpeningAmt,
                                                CLOSING = z.Closing,
                                                COMM_AMT = z.CommissionAmt
                                            }).ToList();
                return transactionlistvalue;
            }
            else if (!string.IsNullOrEmpty(Whitelevel) && !string.IsNullOrEmpty(Supervalue) && !string.IsNullOrEmpty(Distributor) && !string.IsNullOrEmpty(MemberId) && string.IsNullOrEmpty(status))
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
            //if (!string.IsNullOrEmpty(MemberId) && ((status == "Mobile Recharge" || status == "DMR")))
            //{
            //    var transactionlistvalue = (from x in db.TBL_ACCOUNTS
            //                                join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
            //                                where x.MEM_ID == mem_id && x.TRANSACTION_TYPE == status
            //                                select new
            //                                {
            //                                    SLN = x.ACC_NO,
            //                                    MerchantName = y.UName,
            //                                    MemberType = x.MEMBER_TYPE,
            //                                    Trans_Type = x.TRANSACTION_TYPE,
            //                                    Trans_Date = x.TRANSACTION_DATE,
            //                                    DR_CR = x.DR_CR,
            //                                    Amount = x.AMOUNT,
            //                                    Narration = x.NARRATION,
            //                                    OpeningAmt = x.OPENING,
            //                                    Closing = x.CLOSING,
            //                                    CommissionAmt = x.COMM_AMT
            //                                }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
            //                                {
            //                                    SerialNo = index + 1,
            //                                    ACC_NO = z.SLN,
            //                                    UserName = z.MerchantName,
            //                                    MEMBER_TYPE = z.MemberType,
            //                                    TRANSACTION_TYPE = z.Trans_Type,
            //                                    TRANSACTION_DATE = z.Trans_Date,
            //                                    DR_CR = z.DR_CR,
            //                                    AMOUNT = z.Amount,
            //                                    NARRATION = z.Narration,
            //                                    OPENING = z.OpeningAmt,
            //                                    CLOSING = z.Closing,
            //                                    COMM_AMT = z.CommissionAmt
            //                                }).ToList();
            //    return transactionlistvalue;
            //}
            //else if (!string.IsNullOrEmpty(MemberId) && status == "Requisition")
            //{
            //    var transactionlistvalue = (from x in db.TBL_ACCOUNTS
            //                                join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
            //                                where x.MEM_ID == mem_id && x.TRANSACTION_TYPE != "Mobile Recharge" && x.TRANSACTION_TYPE != "DMR"
            //                                select new
            //                                {
            //                                    SLN = x.ACC_NO,
            //                                    MerchantName = y.UName,
            //                                    MemberType = x.MEMBER_TYPE,
            //                                    Trans_Type = x.TRANSACTION_TYPE,
            //                                    Trans_Date = x.TRANSACTION_DATE,
            //                                    DR_CR = x.DR_CR,
            //                                    Amount = x.AMOUNT,
            //                                    Narration = x.NARRATION,
            //                                    OpeningAmt = x.OPENING,
            //                                    Closing = x.CLOSING,
            //                                    CommissionAmt = x.COMM_AMT
            //                                }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
            //                                {
            //                                    SerialNo = index + 1,
            //                                    ACC_NO = z.SLN,
            //                                    UserName = z.MerchantName,
            //                                    MEMBER_TYPE = z.MemberType,
            //                                    TRANSACTION_TYPE = z.Trans_Type,
            //                                    TRANSACTION_DATE = z.Trans_Date,
            //                                    DR_CR = z.DR_CR,
            //                                    AMOUNT = z.Amount,
            //                                    NARRATION = z.Narration,
            //                                    OPENING = z.OpeningAmt,
            //                                    CLOSING = z.Closing,
            //                                    COMM_AMT = z.CommissionAmt
            //                                }).ToList();
            //    return transactionlistvalue;
            //}
            //else if (!string.IsNullOrEmpty(MemberId) && string.IsNullOrEmpty(status))
            //{
            //    var transactionlistvalue = (from x in db.TBL_ACCOUNTS
            //                                join y in db.TBL_MASTER_MEMBER on x.MEM_ID equals y.MEM_ID
            //                                where x.MEM_ID == mem_id
            //                                select new
            //                                {
            //                                    SLN = x.ACC_NO,
            //                                    MerchantName = y.UName,
            //                                    MemberType = x.MEMBER_TYPE,
            //                                    Trans_Type = x.TRANSACTION_TYPE,
            //                                    Trans_Date = x.TRANSACTION_DATE,
            //                                    DR_CR = x.DR_CR,
            //                                    Amount = x.AMOUNT,
            //                                    Narration = x.NARRATION,
            //                                    OpeningAmt = x.OPENING,
            //                                    Closing = x.CLOSING,
            //                                    CommissionAmt = x.COMM_AMT
            //                                }).AsEnumerable().Select((z, index) => new TBL_ACCOUNTS
            //                                {
            //                                    SerialNo = index + 1,
            //                                    ACC_NO = z.SLN,
            //                                    UserName = z.MerchantName,
            //                                    MEMBER_TYPE = z.MemberType,
            //                                    TRANSACTION_TYPE = z.Trans_Type,
            //                                    TRANSACTION_DATE = z.Trans_Date,
            //                                    DR_CR = z.DR_CR,
            //                                    AMOUNT = z.Amount,
            //                                    NARRATION = z.Narration,
            //                                    OPENING = z.OpeningAmt,
            //                                    CLOSING = z.Closing,
            //                                    COMM_AMT = z.CommissionAmt
            //                                }).ToList();
            //    return transactionlistvalue;
            //}
            return null;
        }

    }
}