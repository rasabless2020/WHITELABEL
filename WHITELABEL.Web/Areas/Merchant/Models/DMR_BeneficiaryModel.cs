namespace WHITELABEL.Web.Areas.Merchant.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;
    public class DMR_BeneficiaryModel
    {
        [Required]
        [Display(Name ="Beneficiary Name")]
        public string BeneficiaryName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter Beneficiary Account no")]        
        [MinLength(10, ErrorMessage = "Minimum 10 characters required")]
        [MaxLength(15, ErrorMessage = "Maximum 15 characters required")]
        public string BeneficiaryAccount { get; set; }

        [Display(Name = "Re-enter Account No.")]
        [Compare("BeneficiaryAccount", ErrorMessage = "Confirm Beneficiary Account no and Beneficiary Account no do not match")]
        public string ConfirmBeneficiary { get; set; }
        [Required]
        [Display(Name = "Bank IFSC")]
        public string BankIFSC { get; set; }
    }
}