namespace WHITELABEL.Web.Areas.Merchant.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;
    public class MoneyTransferModelView
    {
        public string RemitterId { get; set; }
        public string BeneficiaryID { get; set; }
        public string BeneficiaryName { get; set; }
        public string BeneficiaryAccount { get; set; }
        public string BeneficiaryBankName { get; set; }
        public string AgentID { get; set; }
        public string RemitterMobileNo { get; set; }
        public string BeneficiaryIFSC { get; set; }
        [Required]
        [Display(Name = "Amount")]
        [RegularExpression(@"^\d*(\.\d{1,4})?$", ErrorMessage = "Enter Valid Amount")]
        public decimal Amount { get; set; }
        [Required]
        public string PaymentMode { get; set; }
        
    }
}