namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("CREDIT_LIMIT_BALANCE_DISTRIBUTION")]

    public class TBL_CREDIT_LIMIT_BALANCE_DISTRIBUTION
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SLN { get; set; }
        public long TO_MEM_ID { get; set; }
        [Required]
        [Display(Name = "Please choose distributor ")]
        public long FROM_MEM_ID { get; set; }
        public DateTime CREDIT_DATE { get; set; }
        [Required]
        [Display(Name = "Amount")]
        [Range(typeof(Decimal), "0", "9999999999", ErrorMessage = "{0} must be a decimal/number between {1} and {2}.")]
        public decimal CREDIT_AMOUNT { get; set; }
        public string CREDIT_NOTE_DESCRIPTION { get; set; }
        public bool CREDIT_STATUS { get; set; }
        public decimal? GST_VAL { get; set; }
        public decimal? GST_AMOUNT { get; set; }
        public decimal? TDS_VAL { get; set; }
        public decimal? TDS_AMOUNT { get; set; }
        public Decimal? CREDIT_OPENING { get; set; }
        public Decimal? CREDITCLOSING { get; set; }
        public string CREDIT_TRN_TYPE { get; set; }
        public string CORELATIONID { get; set; }
        [NotMapped]
        public string FromUser { get; set; }
        [NotMapped]
        public decimal Reserved_credit_limit { get; set; }
        [NotMapped]
        public string FROM_DATE { get; set; }
        [NotMapped]
        public string TO_DATE { get; set; }
        [NotMapped]
        public string DR_Col { get; set; }
        [NotMapped]
        public string CR_Col { get; set; }
    }
}
