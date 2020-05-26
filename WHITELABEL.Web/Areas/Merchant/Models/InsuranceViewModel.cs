namespace WHITELABEL.Web.Areas.Merchant.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;
    public class InsuranceViewModel
    {
        [Required]
        [Display(Name = "Contact No")]
        [MaxLength(10, ErrorMessage = "Contact No is not greater then 10 digit")]
        [MinLength(10, ErrorMessage = "Contact No is not less then 10 digit")]        
        public string PolicyNo { get; set; }
        [Required]
        [Range(1, 9999)]
        [Display(Name = "Amount")]
        [RegularExpression(@"^\d*(\.\d{1,4})?$", ErrorMessage = "Enter Valid Amount")]
        public decimal PolicyAmount { get; set; }
        [Required]
        [Display(Name = "Service Name")]
        public string Service_Name { get; set; }
        public string Service_Key { get; set; }
    }
}