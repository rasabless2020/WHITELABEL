namespace WHITELABEL.Web.Areas.Merchant.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;


    public class DatacardRechargeModel
    {
        [Required]
        [Display(Name = "Datacard No")]                
        public string DatacardID { get; set; }
        [Required]
        [Display(Name = "Operator Name")]
        public string OperatorName { get; set; }
        public long PRODUCTID { get; set; }
        [Required]
        [Display(Name = "Operator Circle Name")]
        public string OperatorCircleName { get; set; }
        [Required]
        [Display(Name = "Amount")]
        [RegularExpression(@"^\d*(\.\d{1,4})?$", ErrorMessage = "Enter Valid Amount")]
        public decimal RechargeAmt { get; set; }
    }
}