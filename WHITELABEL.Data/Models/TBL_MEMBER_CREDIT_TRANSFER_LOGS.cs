namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("MEMBER_CREDIT_TRANSFER_LOGS")]

    public class TBL_MEMBER_CREDIT_TRANSFER_LOGS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SLN { get; set; }
        public long TO_CREDIT_ID { get; set; }
        public long TO_CREDIT_MEM_ROLE { get; set; }
        public string TO_CREDIT_MEM_TYPE { get; set; }
        public long FROM_CREDIT_ID { get; set; }
        public long FROM_CREDIT_MEM_ROLE { get; set; }
        public string FROM_CREDIT_MEM_TYPE { get; set; }
        public DateTime CREDIT_START_DATE { get; set; }
        public DateTime CREDIT_END_DATE { get; set; }
        public string CREDIT_TYPE { get; set; }
        public bool CREDIT_STATUS { get; set; }
        public bool CREDIT_EXPIRE { get; set; }
        public decimal USEDCREDIT_AMOUNT { get; set; }
    public string IPADDRESS { get; set; }
        public long WLP_ID { get; set; }
        public long SUPER_ID { get; set; }
        public long DIST_ID { get; set; }
        public long MER_ID { get; set; }
        public string NARRATION { get; set;  }
    }
}
