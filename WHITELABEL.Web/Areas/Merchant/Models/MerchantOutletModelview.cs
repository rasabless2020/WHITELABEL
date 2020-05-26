namespace WHITELABEL.Web.Areas.Merchant.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;

    public class MerchantOutletModelview
    {
        [Required]
        [Display(Name = "Member Mobile")]
        [MaxLength(15, ErrorMessage = "Mobile no is not greater then 15 digit")]
        [MinLength(10, ErrorMessage = "Mobile no is not less then 10 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile must be number")]
        public string Mobile { get; set; }
        [Required]
        [Display(Name = "OTP")]
        [MaxLength(6, ErrorMessage = "OTP is not greater then 6 digit")]
        [MinLength(6, ErrorMessage = "OTP is not less then 6 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "OTP must be number")]
        public string OTP { get; set; }
        public string Email { get; set; }
        public string Company { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Pincode { get; set; }
        public string Reg_Mobile { get; set; }
        [Required]
        [Display(Name = "Pan No")]
        [MaxLength(10, ErrorMessage = "Pan no is not greater then 10 digit")]
        [MinLength(10, ErrorMessage = "Pan no is not less then 10 digit")]        
        public string PanNo { get; set; }
    }
}