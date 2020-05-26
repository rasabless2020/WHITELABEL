namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("API_RESPONSE_OUTPUT")]
    public class TBL_API_RESPONSE_OUTPUT
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public string TXNID { get; set; }
        public string REQUESTID { get; set; }
        public string MOBILENO { get; set; }
        public long STATUSID { get; set; }
        public string DESCRIPTION { get; set;    }
        public decimal AMOUNT { get; set; }
        public decimal BALANCE { get; set; }
        public DateTime DATE { get; set; }
        public string OPREFNO { get; set; }
        public DateTime CREATEDATE { get; set; }
        public long MEM_ID { get; set; }
        public bool STATUS { get; set; }
        public string RECHARGETYPE { get; set; }
    }
}
