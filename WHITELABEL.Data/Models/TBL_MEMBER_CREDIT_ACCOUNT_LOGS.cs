namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("MEMBER_CREDIT_ACCOUNT_LOGS")]
    public class TBL_MEMBER_CREDIT_ACCOUNT_LOGS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SLN { get; set; }
        public long MEM_ID { get; set; }
        public long CREDIT_ID { get; set; }
        public string CORELATIONID { get; set; }
        public string CREDIT_TRAN_TYPE { get; set; }
        public decimal USED_CREDIT_AMOUNT { get; set; }
        public decimal CREDIT_OPENING_BALANCE { get; set; }
        public decimal CREDIT_CLOSING_BALANCE { get; set; }
        public DateTime CREDIT_USED_DATE { get; set; }
        public string IPADDRESS { get; set; }
        public long WLP_ID { get; set; }
        public long SUPER_ID { get; set; }
        public long DIST_ID { get; set; }
        public long MER_ID { get; set; }
        public string NARRATION { get; set;  }
    }
}
