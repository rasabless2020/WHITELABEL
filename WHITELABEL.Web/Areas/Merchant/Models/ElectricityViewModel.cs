namespace WHITELABEL.Web.Areas.Merchant.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;

    public class ElectricityViewModel
    {
        [Required]
        [Display(Name = "Account ID")]
        [MaxLength(15, ErrorMessage = "Customer ID is not greater then 15 digit")]
        [MinLength(10, ErrorMessage = "Customer ID is not less then 8 digit")]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Contact No must be number")]
        public string CustomerId { get; set; }
        [Required]
        [Display(Name = "Member Mobile")]
        [MaxLength(15, ErrorMessage = "Mobile no is not greater then 15 digit")]
        [MinLength(10, ErrorMessage = "Mobile no is not less then 10 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile must be number")]        
        public string MobileNo { get; set; }
        [Required]
        [Display(Name = "Service Name")]
        public string Service_Name { get; set; }
        public string Service_Key { get; set; }
        [Required]
        [Range(1, 9999)]
        [Display(Name = "Amount")]
        [RegularExpression(@"^\d*(\.\d{1,4})?$", ErrorMessage = "Enter Valid Amount")]
        //[RegularExpression(@"^[0-9]+(\.[0-9]{1,2})$", ErrorMessage = "Valid Decimal number with maximum 2 decimal places.")]
        public decimal RechargeAmt { get; set; }
        [Required]
        [MaxLength(10, ErrorMessage = "Pin no is not greater then 10 digit")]
        [MinLength(6, ErrorMessage = "Pin no is not less then 6 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Pin Code must be number")]
        public string PIN { get; set; }

        
        [MaxLength(6, ErrorMessage = "Bill Unit is not greater then 6 digit")]
        [MinLength(1, ErrorMessage = "Bill Unit is not less then 1 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Bill Unit must be number")]
        public string BillUnit { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [MaxLength(15, ErrorMessage = "First Name is not greater then 15 digit")]
        [MinLength(2, ErrorMessage = "First Name is not less then 2 digit")]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Contact No must be number")]
        public string CUSTOMER_FIRST_NAME { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        [MaxLength(15, ErrorMessage = "Last Name is not greater then 15 digit")]
        [MinLength(2, ErrorMessage = "Last Name is not less then 2 digit")]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Contact No must be number")]
        public string CUSTOMER_LAST_NAME { get; set; }
        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [MaxLength(150, ErrorMessage = "Email is not greater then 150 digit")]
        [MinLength(10, ErrorMessage = "Email is not less then 10 digit")]
        [StringLength(255, ErrorMessage = "Email must be 255 digit")]
        public string CUSTOMER_EMAIL_ID { get; set; }

        [NotMapped]
        public string geolocation { get; set; }
        [NotMapped]
        public string IpAddress { get; set; }
        [Required]
        [Display(Name = "City")]
        public string City { get; set; }
        [NotMapped]
        public string ElectricityRefNo { get; set; }

    }
}