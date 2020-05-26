namespace WHITELABEL.Data.Models
{    
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("SETTINGS_BANK_DETAILS")]
    public class TBL_SETTINGS_BANK_DETAILS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SL_NO { get; set; }
        [Required]
        [Display(Name = "Bank")]
        [StringLength(150, ErrorMessage = "Bank name should be 150 digit")]
        public string BANK { get; set; }
        [Required]
        [Display(Name = "Ifsc")]
        [MaxLength(15, ErrorMessage = "IFSC Code not greater then 15 digit")]
        [MinLength(11, ErrorMessage = "IFSC Code not less then 11 digit")]
        [RegularExpression("^[0-9a-zA-Z ]+$", ErrorMessage = "Enter valid user name")]
        public string IFSC { get; set; }
        [MaxLength(15, ErrorMessage = "MICR Code not greater then 15 digit")]
        [MinLength(9, ErrorMessage = "MICR Code  not less then 9 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Micr code must be number")]
        public string MICR_CODE { get; set; }
        [Required]
        [Display(Name = "Branch")]
        [StringLength(150, ErrorMessage = "Branch should be 150 digit")]
        public string BRANCH { get; set; }
        [StringLength(500, ErrorMessage = "Address should be 500 digit")]
        public string ADDRESS { get; set; }
        [Required]
        [Display(Name = "Contact No.")]
        [MaxLength(15, ErrorMessage = "Contact not greater then 15 digit")]
        [MinLength(10, ErrorMessage = "Contact not less then 10 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Contact no must be number")]
        public string CONTACT { get; set; }
        [StringLength(100, ErrorMessage = "City should be 100 digit")]
        public string CITY { get; set; }
        [StringLength(150, ErrorMessage = "District should be 150 digit")]
        public string DISTRICT { get; set; }
        public string STATE { get; set; }
        public long? MEM_ID { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public long CREATED_BY { get; set; }
        public DateTime? DELETED_DATE { get; set; }
        public long? DELETED_BY { get; set; }
        public int ISDELETED { get; set; }
        [Required]
        [Display(Name = "Account No")]
        [MaxLength(15, ErrorMessage = "Account no not greater then 15 digit")]
        [MinLength(11, ErrorMessage = "Account no not greater then 15 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Account no must be number")]
        public string ACCOUNT_NO { get; set; }
        [NotMapped]
        public string UserName { get; set; }
        [NotMapped]
        public string STATENAME { get; set; }
    }
}
