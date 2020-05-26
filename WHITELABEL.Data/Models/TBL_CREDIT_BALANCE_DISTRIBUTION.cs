namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("CREDIT_BALANCE_DISTRIBUTION")]
    public class TBL_CREDIT_BALANCE_DISTRIBUTION
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SLN { get; set; }
        public long TO_MEM_ID { get; set; }
        public long FROM_MEM_ID { get; set; }
        public long MEMBER_ROLE { get; set; }
        public DateTime CREDIT_DATE { get; set; }
        public string CREDIT_TYPE { get; set; }
        public decimal CREDIT_AMOUNT { get; set; }
        public string CREDIT_NOTE_DESCRIPTION { get; set; }
        public bool CREDIT_STATUS { get; set; }
        public decimal GST_VAL { get; set; }
        public decimal GST_AMOUNT { get; set; }
        public decimal TDS_VAL { get; set; }
        public decimal TDS_AMOUNT { get; set; }
        [NotMapped]
        public string Member_RoleName { get; set; }
        [NotMapped]
        public string FromUser { get; set; }
        [NotMapped]
        public string FROM_DATE { get; set; }
        [NotMapped]
        public string TO_DATE { get; set; }
    }
}
