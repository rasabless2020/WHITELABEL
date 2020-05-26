namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;

    [Table("DMR_FUND_TRANSFER_DETAILS")]
    public class TBL_DMR_FUND_TRANSFER_DETAILS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long TXNID { get; set;    }
        public string IPAY_ID { get; set; }
        public string REF_NO { get; set; }
        public string OPR_ID { get; set; }
        public string NAME { get; set;   }
        public decimal OPENING_BAL { get; set; }
        public decimal AMOUNT { get; set; }
        public decimal CHARGED_AMT { get; set; }
        public decimal LOCKED_AMT { get; set; }
        public string BANK_ALIAS { get; set; }
        public string STATUS { get; set; }
        public string STATUSCODE { get; set; }
        public DateTime TXNDATE { get; set; }        
        public string BANKREFNO { get; set; }
        public string REMARKS { get; set; }
        public string VERIFICATIONSTATUS { get; set;     }
        public long MEM_ID { get; set; }
        public DateTime? REQ_DATE { get; set; }
        public decimal REFUNF_ALLOWED { get; set; }
        public string REMITTER_ID { get; set; }
    }
}
