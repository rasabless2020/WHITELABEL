namespace WHITELABEL.Web.Areas.Merchant.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;
    public class DMRAccountVerification
    {
        [Required]
        [Display(Name = "Mobile No")]
        [MaxLength(10, ErrorMessage = "Mobile No is not greater then 10 digit")]
        [MinLength(10, ErrorMessage = "Mobile No is not less then 10 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile must be number")]
        public string RemitterMobile { get; set; }
        [Required]
        [Display(Name = "Account No")]
        [MaxLength(15, ErrorMessage = "Account No is not greater then 15 digit")]
        [MinLength(10, ErrorMessage = "Account No is not less then 10 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile must be number")]
        public string Account { get; set; }
        [Required]
        [Display(Name = "IFSC Code")]
        [MaxLength(15, ErrorMessage = "IFSC Code is not greater then 15 digit")]
        [MinLength(10, ErrorMessage = "IFSC Code is not less then 10 digit")]
        public string IFSC { get; set; }
    }
}