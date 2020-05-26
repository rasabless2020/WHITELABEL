namespace WHITELABEL.Web.Areas.Merchant.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;


    public class DTHRechargeModel
    {
        [Required]
        [Display(Name = "ContactNo")]
        [MaxLength(15, ErrorMessage = "Mobile no not greater then 15 digit")]
        [MinLength(10, ErrorMessage = "Mobile no not less then 10 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile must be number")]
        public string ContactNo { get; set; }
        [Required]
        [Display(Name = "Operator Name")]
        public string OperatorName { get; set; }
        public long PRODUCTID { get; set; }
        [Required]
        [Display(Name = "Amount")]
        [RegularExpression(@"^\d*(\.\d{1,4})?$", ErrorMessage = "Enter Valid Amount")]
        public decimal RechargeAmt { get; set; }
    }
}