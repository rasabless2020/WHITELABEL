namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("ACCOUNTS")]
    public class TBL_ACCOUNTS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ACC_NO { get; set; }
        public long API_ID { get; set; }
        public long MEM_ID { get; set; }
        public string MEMBER_TYPE { get; set; }
        public string TRANSACTION_TYPE { get; set; }
        public DateTime TRANSACTION_DATE { get; set; }
        public DateTime TRANSACTION_TIME { get; set; }
        public string DR_CR { get; set; }
        public decimal AMOUNT { get; set; }
        public string NARRATION { get; set; }
        public decimal OPENING { get; set; }
        public decimal CLOSING { get; set; }
        public long REC_NO { get; set; }
        public decimal COMM_AMT { get; set; }
        public double GST { get; set; }
        public double TDS { get; set; }
        public string IPAddress { get; set; }
        public decimal GST_PERCENTAGE { get; set; }
        public decimal TDS_PERCENTAGE { get; set; }
        public long WHITELEVEL_ID { get; set; }
        public long SUPER_ID { get; set; }
        public long DISTRIBUTOR_ID { get; set; }
        public long SERVICE_ID { get; set; }
        public string CORELATIONID { get; set; }
        public string REC_COMM_TYPE { get; set; }
        public decimal? COMM_VALUE { get; set; }
        public decimal? NET_COMM_AMT { get; set; }
        public decimal? TDS_DR_COMM_AMT { get; set; }
        public decimal? CGST_COMM_AMT_INPUT { get; set; }
        public decimal? CGST_COMM_AMT_OUTPUT { get; set; }
        public decimal? SGST_COMM_AMT_INPUT { get; set; }
        public decimal? SGST_COMM_AMT_OUTPUT { get; set; }
        public decimal? IGST_COMM_AMT_INPUT { get; set; }
        public decimal? IGST_COMM_AMT_OUTPUT { get; set; }
        public decimal? TOTAL_GST_COMM_AMT_INPUT { get; set; }
        public decimal? TOTAL_GST_COMM_AMT_OUTPUT { get; set; }
        public decimal? TDS_RATE { get; set; }
        public decimal? CGST_RATE { get; set; }
        public decimal? SGST_RATE { get; set; }
        public decimal? IGST_RATE { get; set; }
        public decimal? TOTAL_GST_RATE { get; set; }
        public long? COMM_SLAB_ID { get; set; }
        public long? STATE_ID { get; set; }
        public int? FLAG1 { get; set; }
        public int? FLAG2 { get; set; }
        public int? FLAG3 { get; set; }
        public int? FLAG4 { get; set; }
        public int? FLAG5 { get; set; }
        public int? FLAG6 { get; set; }
        public int? FLAG7 { get; set; }
        public int? FLAG8 { get; set; }
        public int? FLAG9 { get; set; }
        public int? FLAG10 { get; set; }
        public long? INVOICE_ID { get; set; }
        public string CANCEL_INVOICE { get; set; }
        public long? VENDOR_ID { get; set; }
        [NotMapped]
        public string UserName { get; set; }
        [NotMapped]
        public string RoleName { get; set; }
        [NotMapped]
        public string timevalue { get; set; }
        [NotMapped]
        public long SerialNo { get; set; }
        [NotMapped]
        public string FROM_DATE { get; set; }
        [NotMapped]
        public string TO_DATE { get; set; }
        [NotMapped]
        public long Serial_No { get; set; }
        [NotMapped]
        public string DR_Col { get; set; }
        [NotMapped]
        public string CR_Col { get; set; }
    }
}
