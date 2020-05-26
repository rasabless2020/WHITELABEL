namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("TRANSXT_DMR_TRANSACTION_LIST")]

    public class TBL_TRANSXT_DMR_TRANSACTION_LIST
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public string CustomerID { get; set; }
        public string RecipientID { get; set; }
        public decimal serviceTax { get; set; }
        public string clientRefId { get; set; }
        public decimal fee { get; set; }
        public string initiatorId { get; set; }
        public string accountNumber { get; set; }
        public string txnStatus { get; set; }
        public string name { get; set; }
        public string ifscCode { get; set; }
        public string impsRespCode { get; set; }
        public string impsRespMessage { get; set; }
        public string txnId { get; set; }
        public string timestamp { get; set; }
        public string SenderName { get; set; }
        public string SenderMobile { get; set; }
        public DateTime Transaction_Date { get; set; }
        public string TransactionStatus { get; set; }
        public long MEM_ID { get; set; }
        public decimal TransferAmount { get; set; }
        public decimal TRANSACTION_FEE { get; set; }
        //public decimal WHITELABEL_GST_LIABILITY { get; set; }
        public string MER_COMM_TYPE { get; set; }
        public decimal MER_COMM_VALUE { get; set; }
        public decimal MER_GROSS_COMM_AMT { get; set;    }
        public decimal MER_NET_COMM { get; set;  }
        public decimal MER_TDS_DR_COMM_AMT { get; set; }
        public decimal MER_COMM_GST { get; set; }
        public string DIST_COMM_TYPE { get; set; }
        public long DIST_ID { get; set;  }
        public decimal DIST_COMM_VALUE { get; set; }
        public decimal DIST_GROSS_COMM_AMT { get; set; }
        public decimal DIST_NET_COMM { get; set; }
        public decimal DIST_TDS_DR_COMM_AMT { get; set; }
        public decimal DIST_COMM_GST { get; set; }
        public string SPR_COMM_TYPE { get; set; }
        public long SUPER_ID { get; set; }
        public decimal SUPER_COMM_VALUE { get; set; }
        public decimal SUPER_GROSS_COMM_AMT { get; set; }
        public decimal SUPER_NET_COMM { get; set; }
        public decimal SUPER_TDS_DR_COMM_AMT { get; set; }
        public decimal SUPER_COMM_GST { get; set; }
        public string WHITELABEL_COMM_TYPE { get; set; }
        public long WHITELABEL_ID { get; set; }
        public decimal WHITELABEL_VALUE { get; set; }
        public decimal WHITELABEL_COMM_AMT { get; set; }
        public decimal WHITELABEL_NET_COMM { get; set; }
        public decimal WHITELABEL_TDS_DR_COMM_AMT { get; set; }
        public decimal WHITELABEL_COMM_GST { get; set; }
        public decimal TDS_RATE { get; set; }
        public decimal GST_RATE { get; set; }
        public string CORELATIONID { get; set; }
        public string ERROR_TYPE { get; set; }
        public string ISREVERSE { get; set; }
        public string DOMAIN_NAME { get; set; }
        public string ISCOMMISSIONDISBURSE { get; set; }
        public DateTime COMMISSIONDISBURSEDATE { get; set; }
    }
}
