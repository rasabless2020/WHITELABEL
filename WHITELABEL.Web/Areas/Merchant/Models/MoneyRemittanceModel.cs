namespace WHITELABEL.Web.Areas.Merchant.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;


    public class MoneyRemittanceModel
    {
        [NotMapped]
        public string BeneficiaryName { get; set; }
        [NotMapped]
        public string BeneficiaryAccountNo { get; set; }
        [Required]
        [Display(Name = "Amount")]
        [RegularExpression(@"^\d*(\.\d{1,4})?$", ErrorMessage = "Enter Valid Amount")]
        public decimal Amount { get; set; } 
        [Required]
        [Display(Name = "Security Pin")]        
        public string SecurityPin { get; set; }
        [Required]
        public string RemittanceType { get; set; }
    }
}