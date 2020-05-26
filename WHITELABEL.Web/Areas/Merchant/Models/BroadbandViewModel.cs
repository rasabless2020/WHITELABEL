namespace WHITELABEL.Web.Areas.Merchant.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;

    public class BroadbandViewModel
    {
        [Required]
        [Display(Name = "Phone No")]
        [MaxLength(13, ErrorMessage = "Mobile no not greater then 13 digit")]
        [MinLength(8, ErrorMessage = "Mobile no not less then 8 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Phone No must be number")]
        public string PhoneNo { get; set; }
        [Required]
        [Display(Name = "Account No")]
        [MaxLength(13, ErrorMessage = "Account No is not greater then 13 digit")]
        [MinLength(8, ErrorMessage = "Account No is not less then 8 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Contact No must be number")]
        public string AccountNo { get; set; }
        [Required]
        [Range(1, 9999)]
        [Display(Name = "Amount")]
        [RegularExpression(@"^\d*(\.\d{1,4})?$", ErrorMessage = "Enter Valid Amount")]
        public decimal RechargeAmount { get; set; }
        [Required]
        [Display(Name = "Service Provider")]
        public string ServiceName { get; set; }
        public string ServiceKey { get; set; }
        [NotMapped]
        public string geolocation { get; set; }
        [NotMapped]
        public string IpAddress { get; set; }
        [NotMapped]
        public string BroadbandrefNo { get; set; }
    }
}