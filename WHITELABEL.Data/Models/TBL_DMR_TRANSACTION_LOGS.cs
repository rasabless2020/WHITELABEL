namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("DMR_TRANSACTION_LOGS")]
    public class TBL_DMR_TRANSACTION_LOGS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public string CUSTOMER_ID { get; set; }
        public string RECIPIENT_ID { get; set; }
        public string CLIENT_REF_ID { get; set; }
        public decimal RES_FEE { get; set; }
        public string INITIATOR_ID { get; set; }
        public string ACCOUNT_NO { get; set; }
        public string API_TXN_STATUS { get; set; }
        public string NAME { get; set; }
        public string IFSC_CODE { get; set; }
        public string IMPS_RES_CODE { get; set; }
        public string IMPS_RES_MSG { get; set; }
        public string TXN_ID { get; set; }
        public string TIMESTAMP { get; set; }
        public string SENDER_NAME { get; set; }
        public string SENDER_MOBILE_NO { get; set; }
        public DateTime TRANSACTION_DATE { get; set; }
        public string TRANSACTION_STATUS { get; set; }
        public decimal TRANSFER_AMT { get; set; }
        public long MER_ID { get; set; }
        public string MER_WLP_COMM_TYPE { get; set; }
        public decimal MER_WLP_COMM_RATE { get; set; }
        public string MER_TRANSFER_FEE_TYPE { get; set; }
        public decimal MER_TRANSFER_FEE_RATE { get; set; } 
        public decimal MER_TRANSFER_FEE { get; set; }
        public decimal MER_GST_INPUT { get; set; }
        public decimal MER_GST_OUTPUT { get; set; }
        public decimal MER_GROSS_COMM_AMT { get; set; }
        public decimal MER_TDS_DR_COMM_AMT { get; set; }
        public decimal MER_NET_COMM { get; set; }
        public long DIST_ID { get; set; }
        public string DIST_WLP_COMM_TYPE { get; set; }
        public decimal DIST_WLP_COMM_RATE { get; set; }
        public decimal DIST_GROSS_COMM_AMT { get; set; }
        public decimal DIST_TDS_DR_COMM_AMT { get; set; }
        public decimal DIST_NET_COMM { get; set; }
        public decimal DIST_GST_INPUT { get; set; }
        public decimal DIST_GST_OUTPUT { get; set; }
        public long SUPER_ID { get; set; }
        public string SPR_WLP_COMM_TYPE { get; set; }
        public decimal SPR_WLP_COMM_RATE { get; set; }
        public decimal SPR_GROSS_COMM_AMT { get; set; }
        public decimal SPR_TDS_DR_COMM_AMT { get; set; }
        public decimal SPR_NET_COMM { get; set; }
        public decimal SPR_GST_INPUT { get; set; }
        public decimal SPR_GST_OUTPUT { get; set; }
        public long WLP_ID { get; set; }
        public string WLP_PA_TRAN_RATE_TYPE { get; set; }
        public decimal WLP_PA_TRAN_RATE { get; set; }
        public string WLP_TRANSFER_FEE_TYPE { get; set; }
        public decimal WLP_TRANSFER_FEE_RATE { get; set; }
        public decimal WLP_TRANSFER_FEE { get; set; }
        public decimal WLP_GST_INPUT { get; set; }
        public decimal WLP_GST_OUTPUT { get; set; }
        public decimal WLP_GROSS_COMM_AMT { get; set; }
        public decimal WLP_TDS_DR_COMM_AMT { get; set; }
        public decimal WLP_NET_COMM { get; set; }
        public decimal TDS_RATE { get; set; }
        public decimal GST_RATE { get; set; }
        public string CORELATIONID { get; set; }
        public string ERROR_TYPE { get; set; }
        public bool ISREVERSE { get; set; }
        public string DOMAIN_NAME { get; set; }
        public bool ISCOMMISSIONDISBURSE { get; set; }
        public DateTime? COMMISSIONDISBURSEDATE { get; set; }
        public long? MER_INVOICE_ID { get; set; }
        public string MER_CANCEL_INVOICE { get; set; }
        public long? DIST_INVOICE_ID { get; set; }
        public string DIST_CANCEL_INVOICE { get; set; }
        public long? SUPER_INVOICE_ID { get; set; }
        public string SUPER_CANCEL_INVOICE { get; set; }
        public long? WHITELABEL_INVOICE_ID { get; set; }
        public string WHITELABEL_CANCEL_INVOICE { get; set; }
        public long SLAB_ID { get; set; }
        public string API_RESPONSE { get; set; }
    }
}
