namespace WHITELABEL.Web.Areas.Merchant.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;
    public class DMRBeneficiaryAccountVerify
    {
       
       [Display(Name ="Bank Account")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter Bank Account no")]
        [MinLength(10, ErrorMessage = "Minimum 10 characters required")]
        [MaxLength(15, ErrorMessage = "Maximum 15 characters required")]
        public string BankAccountNo { get; set; }

        public string BankName { get; set; }
        [Required]
        [Display(Name ="Aadhaar Card")]
        [MaxLength(12, ErrorMessage = "Aadhaar Card is not greater than 12 Digit")]
        public string AadhaarCard { get; set; }
        [Required]
        [RegularExpression("[-_,A-Za-z0-9]$",ErrorMessage ="Special character are not allowed")]
        [MaxLength(10, ErrorMessage = "Pan Card is not greater than 10 Digit")]
        public string PanCard { get; set; }
    }
}