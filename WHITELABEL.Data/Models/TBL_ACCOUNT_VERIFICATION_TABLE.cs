namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("ACCOUNT_VERIFICATION_TABLE")]
    public class TBL_ACCOUNT_VERIFICATION_TABLE
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public long MEM_ID { get; set; }
        public decimal RECIPIENT_ACNT_VRT_AMTt { get; set; }
        public decimal APPLIED_AMT_TO_MERCHANT { get; set; }
        public DateTime CREATE_DATE { get; set; }
        public int STATUS { get; set; }
        public string APINAME { get; set; }
    }
}
