namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("DMR_RECIPIENT_DETAILS")]


    public class TBL_DMR_RECIPIENT_DETAILS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public long MEM_ID{ get; set; }
        [Required(ErrorMessage = "Customer Id is required")]
        public string CUSTOMER_ID { get; set; }
        [Required]
        [Display(Name = "Beneficiary Name")]
        [StringLength(255, ErrorMessage = "Beneficiary name must be 255 digit")]
        //[RegularExpression("^[0-9a-zA-Z ]+$", ErrorMessage = "Enter valid beneficiary name")]
        public string BENEFICIARY_NAME { get; set; }
        [Required]
        [Display(Name = "Account Number")]
        [MaxLength(18, ErrorMessage = "Account no is not greater then 18 digit")]
        [MinLength(10, ErrorMessage = "Account no is not less then 10 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Account No must be number")]
        public string ACCOUNT_NO { get; set; }


        [Required]
        [Display(Name = "Ifsc Code")]
        [MaxLength(15, ErrorMessage = "Ifsc code is not greater then 15 digit")]
        [MinLength(10, ErrorMessage = "Ifsc code is not less then 10 digit")]
        [RegularExpression("^[0-9a-zA-Z ]+$", ErrorMessage = "Ifsc code must be alfanumerics")]
        public string IFSC_CODE { get; set; }
        //[Required]
        //[Display(Name = "Beneficiary Mobile No")]
        //[MaxLength(15, ErrorMessage = "Beneficiary Mobile No is not greater then 15 digit")]
        //[MinLength(10, ErrorMessage = "Beneficiary Mobile No is not less then 10 digit")]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Beneficiary Mobile No must be number")]
        public string BENEFICIARY_MOBILE { get; set; }
        public string BENEFICIARY_TYPE { get; set; }
        public DateTime CREATE_DATE { get; set; }
        public int STATUS { get; set; }
        public string TRANSACTIONID { get; set; }
        public string RECIPIENT_ID { get; set; }
        public int ISVERIFIED { get; set; }
        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [MaxLength(250, ErrorMessage = "Email is not greater then 250 digit")]
        [MinLength(10, ErrorMessage = "Email is not less then 10 digit")]
        [StringLength(250, ErrorMessage = "Email must be 250 digit")]
        public string EMAIL_ID { get; set; }
        public string REMARKS { get; set; }
        public long? WLP_ID { get; set; }
        public long? SUPER_ID { get; set; }
        public long? DIST_ID { get; set; }
        public decimal? VERIFY_BENE_CHARGE { get; set; }
        public decimal? RETURN_BACK_TO_CUST_CHARGE { get; set; }
        public DateTime? TIMESTAMP { get; set; }
        public string CORELATION_ID { get; set; }
        public decimal? WLP_GST_OUTPUT { get; set; }
        public decimal? WLP_GST_INPUT { get; set; }
        public decimal? MER_GST_OUTPUT { get; set; }
        public decimal? MER_GST_INPUT { get; set; }
        public string API_RESPONSE { get; set; }
        public string BENEFICIARY_ID { get; set; }
        [NotMapped]
        public string ChargedAmount { get; set; }
        [NotMapped]
        public string BankNAme { get; set; }
        [NotMapped]
        public bool VerifyBeneficiary { get; set; }
        [NotMapped]        
        public string EnterOTP { get; set; }
        [NotMapped]       
        public string BENEFICIARY_LASTNAME { get; set; }
    }
}
