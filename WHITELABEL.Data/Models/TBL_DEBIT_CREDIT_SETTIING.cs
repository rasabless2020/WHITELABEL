namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("DEBIT_CREDIT_SETTIING")]
    public class TBL_DEBIT_CREDIT_SETTIING
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SLN { get; set; }
        public string CORELLATION_ID { get; set; }
        public long? MER_ID { get; set; }
        public long? DIST_ID { get; set; }
        public long? WLP_ID { get; set; }
        [Required(ErrorMessage = "Member type is required")]
        //[RegularExpression("^[0-9a-zA-Z ]+$", ErrorMessage = "Enter valid user name")]
        //[MaxLength(45, ErrorMessage = "User name is not greater then 45 digit")]
        //[MinLength(5, ErrorMessage = "User name is not less then 5 digit")]
        public long MEM_TYPE { get; set; }
        public long TRANS_AGAINST { get; set; }
        public long TRANS_ISSUED_BY { get; set; }
        public long TRANS_DONE_BY { get; set; }
        [Required(ErrorMessage = "Transaction type is required")]
        //[RegularExpression("^[0-9a-zA-Z ]+$", ErrorMessage = "Enter valid user name")]
        //[MaxLength(45, ErrorMessage = "User name is not greater then 45 digit")]
        //[MinLength(5, ErrorMessage = "User name is not less then 5 digit")]
        public string TRANS_TYPE { get; set; }
        //[DataType(DataType.Currency)]
        //[RegularExpression(@"^\d*(\.\d{1,4})?$", ErrorMessage = "Enter Valid Amount")]
        //[MaxLength(8, ErrorMessage = "Amount is not greater then 8 digit")]
        //[MinLength(2, ErrorMessage = "Amount is not less then 2 digit")]
        [Required]
        [Display(Name = "Amount")]
        [Range(typeof(Decimal), "0", "999999", ErrorMessage = "{0} must be a decimal/number between {1} and {2}.")]
        public decimal TRANS_AMOUNT { get; set; }
        [Required(ErrorMessage = "Reference no is required")]
        //[RegularExpression("^[0-9a-zA-Z ]+$", ErrorMessage = "Enter valid user name")]
        [MaxLength(10, ErrorMessage = "Reference no is not greater then 10 digit")]
        [MinLength(5, ErrorMessage = "Reference no is not less then 5 digit")]
        public string TRANS_REF_NO { get; set; }
        [Required(ErrorMessage = "Enter transaction details")]
        //[RegularExpression("^[0-9a-zA-Z ]+$", ErrorMessage = "Enter valid user name")]
        [MaxLength(500, ErrorMessage = "Transaction details is not greater then 500 digit")]
        [MinLength(5, ErrorMessage = "Transaction details is not less then 5 digit")]
        public string TRANS_DETAILS { get; set; }
        [Required]
        [Display(Name = "Debit/Credit Apply For ")]
        public long TRANS_REMARKS { get; set; }
        public bool GST_APPLICABLE { get; set; }
        public bool TDS_APPLICABLE { get; set; }
        public decimal TDS_PER { get; set; }
        public decimal TDS_AMOUNT { get; set; }
        [Required]
        [Display(Name = "Transaction Date ")]
        public DateTime? TRANS_DATE { get; set; }
        public bool STATUS { get; set; }
        [NotMapped]
        public string GST_APPLICABLE_VALUE { get; set; }
        [NotMapped]
        public string TDS_APPLICABLE_VALUE { get; set; }
        [NotMapped]
        public string MemberTypeName { get; set; }
        [NotMapped]
        public string WLPName { get; set; }
        [NotMapped]
        public string RemarkName { get; set; }
    }
}
