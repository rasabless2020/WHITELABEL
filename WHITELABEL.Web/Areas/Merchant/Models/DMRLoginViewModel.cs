namespace WHITELABEL.Web.Areas.Merchant.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;


    public class DMRLoginViewModel
    {

        [Required]
        [Display(Name = "Mobile")]
        [MaxLength(15, ErrorMessage = "Mobile no not greater then 15 digit")]
        [MinLength(10, ErrorMessage = "Mobile no not less then 10 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile must be number")]
        public string MobileNo { get; set; }
    }
    public class DMRRegistrationViewModel
    {
        [Required]
        [Display(Name = "Mobile")]
        [MaxLength(15, ErrorMessage = "Mobile no. not greater then 15 digit")]
        [MinLength(10, ErrorMessage = "Mobile no. not less then 10 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile must be number")]
        public string MobileNo { get; set; }
        [Required]
        [Display(Name = "Name")]
        [MaxLength(255, ErrorMessage = "Name is not greater then 6 digit")]
        [MinLength(4, ErrorMessage = "Name is not less then 4 digit")]
        [RegularExpression("^[0-9a-zA-Z ]+$", ErrorMessage = "Enter valid Name")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Pincode")]
        [MaxLength(6, ErrorMessage = "Pincode is not greater then 6 digit")]
        [MinLength(4, ErrorMessage = "Pincode is not less then 4 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Pincode must be number")]
        public string Pincode { get; set; } 

    }
}