namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;

    [Table("INSTANTPAY_RECHARGE_RESPONSE")]
    public class TBL_INSTANTPAY_RECHARGE_RESPONSE
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public string Ipay_Id { get; set; }
        public string AgentId { get; set; }
        public string Opr_Id { get; set; }
        public string AccountNo { get; set; }
        public string Sp_Key { get; set; }
        public decimal Trans_Amt { get; set; }
        public decimal Charged_Amt { get; set; }
        public decimal Opening_Balance { get; set; }
        public DateTime DateVal { get; set; }
        public string Status { get; set; }
        public string Res_Code { get; set; }
        public string res_msg { get; set; }
        public long Mem_ID { get; set; }
        public string RechargeType { get; set; }
        public string IpAddress { get; set; }
        public string API_Name { get; set; }
        public string RechargeResponse { get; set; }
        public string REC_COMM_TYPE { get; set; }
        public decimal MER_COMM_VALUE { get; set; }
        public decimal MER_COMM_AMT { get; set; }
        public decimal MER_TDS_DR_COMM_AMT { get; set; }
        public long DIST_ID { get; set; }
        public decimal DIST_COMM_VALUE { get; set; }
        public decimal DIST_COMM_AMT { get; set; }
        public decimal DIST_TDS_DR_COMM_AMT { get; set; }
        public long SUPER_ID { get; set; }
        public decimal SUPER_COMM_VALUE { get; set; }
        public decimal SUPER_COMM_AMT { get; set; }
        public decimal SUPER_TDS_DR_COMM_AMT { get; set; }
        public long WHITELABEL_ID { get; set; }
        public decimal WHITELABEL_VALUE { get; set; }
        public decimal WHITELABEL_COMM_AMT { get; set; }
        public decimal WHITELABEL_TDS_DR_COMM_AMT { get; set; }
        public decimal TDS_RATE { get; set; }
        public string CORELATIONID { get; set; }
        public string ERROR_TYPE { get; set; }
        public string ISREVERSE { get; set; }
        public string DOMAIN_NAME { get; set; }
        public string ISCOMMISSIONDISBURSE { get; set; }
        public DateTime COMMISSIONDISBURSEDATE { get; set; }
        public DateTime? REVERSE_DATE { get; set; }
        public decimal GST_RATE { get; set; }
        public decimal MER_COMM_GST_AMT { get; set; }
        public decimal DIST_COMM_GST_AMT { get; set; }
        public decimal SUPER_COMM_GST_AMT { get; set;    }
        public decimal WHITELABEL_COMM_GST_AMT { get; set;   }
        public long MER_INVOICE_ID { get; set;   }
        public string MER_CANCEL_INVOICE { get; set; }
        public long DIST_INVOICE_ID { get; set; }
        public string DIST_CANCEL_INVOICE { get; set; }
        public long SUPER_INVOICE_ID { get; set; }
        public string SUPER_CANCEL_INVOICE { get; set; }
        public long WHITELABEL_INVOICE_ID { get; set; }
        public string WHITELABEL_CANCEL_INVOICE { get; set;  }
        public long SLAB_ID { get; set; }
    }
}
