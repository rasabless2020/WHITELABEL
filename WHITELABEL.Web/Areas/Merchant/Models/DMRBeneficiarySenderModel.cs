namespace WHITELABEL.Web.Areas.Merchant.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;
    public class DMRBeneficiarySenderModel
    {
        public string RemitterId { get; set; }
        [Required]
        [Display(Name = "Mobile No")]
        [MaxLength(10, ErrorMessage = "Mobile no is not greater then 10 digit")]
        [MinLength(10, ErrorMessage = "Mobile no is not less then 10 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile must be number")]
        public string MobileNo { get; set; }
        [Required]
        [Display(Name = "Name")]
        [StringLength(255, ErrorMessage = "Name must be 255 digit")]
        [RegularExpression("^[0-9a-zA-Z ]+$", ErrorMessage = "Enter valid name")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "IFSC Code")]
        public string IFSC { get; set; }
        [Required]
        [Display(Name = "Account No")]
        public string Account { get; set; }
    }
}