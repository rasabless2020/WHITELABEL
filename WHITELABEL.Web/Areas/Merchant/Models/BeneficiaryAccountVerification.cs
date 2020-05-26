namespace WHITELABEL.Web.Areas.Merchant.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;
    public class BeneficiaryAccountVerification
    {
        [Required]
        [Display(Name = "ContactNo")]
        [MaxLength(15, ErrorMessage = "Mobile no not greater then 15 digit")]
        [MinLength(10, ErrorMessage = "Mobile no not less then 10 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile must be number")]
        public string RemitterMobileNo { get; set; }
        [Required]
        [Display(Name = "Beneficiary Account no.")]
        [MaxLength(15, ErrorMessage = "Account no not greater then 15 digit")]
        [MinLength(10, ErrorMessage = "Account no not less then 10 digit")]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Account must be number")]
        public string BankAccountNo { get; set; }
        [Required]
        [Display(Name = "IFSC Code")]
        [MaxLength(15, ErrorMessage = "IFSC Code not greater then 15 digit")]
        [MinLength(10, ErrorMessage = "IFSC Code not less then 10 digit")]
        public string IFSCCode { get; set; }
        [NotMapped]
        public string AgentID { get; set; }

    }

    public class DropdownSelectbyMonth
    {
        public string MonthID { get; set; }  
        public string MonthName { get; set; }
    }
}