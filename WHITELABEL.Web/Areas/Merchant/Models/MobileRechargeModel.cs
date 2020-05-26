namespace WHITELABEL.Web.Areas.Merchant.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;    

    public class MobileRechargeModel
    {
        public string PrepaidRecharge { get; set; }
        public string PostpaidRecharge { get; set; }
        [Required]
        [Display(Name = "Contact No")]
        [MaxLength(12, ErrorMessage = "Customer ID is not greater then 12 digit")]
        [MinLength(10, ErrorMessage = "Customer ID is not less then 10 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile must be number")]
        public string ContactNo { get; set; }
        [Required]
        [Display(Name = "Operator Name")]
        public string OperatorName { get; set; }
        public long PRODUCTID { get; set; }
        [Required]
        [Range(1, 9999)]
        [Display(Name = "Amount")]
        [RegularExpression(@"^\d*(\.\d{1,4})?$", ErrorMessage = "Enter Valid Amount")]
        //[RegularExpression(@"^[0-9]+(\.[0-9]{1,2})$", ErrorMessage = "Valid Decimal number with maximum 2 decimal places.")]
        public decimal RechargeAmt { get; set; }
        public string Service_Key { get; set; }
        [Required]
        [Display(Name = "Circle Name")]
        public string CircleName { get; set; }
        [NotMapped]
        public string geolocation { get; set; }
        [NotMapped]
        public string IpAddress { get; set; }
        [NotMapped]
        public string Reference_Id { get; set; }
        [NotMapped]
        public string JIO_Plan_Id { get; set; }
    }
    public class ServiceList
    {
        public string ServiceName { get; set; }
        public bool ServiceStatus { get; set; }
    }
}