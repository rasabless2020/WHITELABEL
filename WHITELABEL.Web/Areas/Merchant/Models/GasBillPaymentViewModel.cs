namespace WHITELABEL.Web.Areas.Merchant.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;


    public class GasBillPaymentViewModel
    {
        [Required]
        [Display(Name = "Contact No")]
        [MaxLength(10, ErrorMessage = "Contact No is not greater then 10 digit")]
        [MinLength(10, ErrorMessage = "Contact No is not less then 10 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Contact No must be number")]
        public string ContactNo { get; set; }
        [Required]
        [Display(Name = "Customer ID")]
        [MaxLength(10, ErrorMessage = "Customer ID is not greater then 10 digit")]
        [MinLength(10, ErrorMessage = "Customer ID is not less then 10 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Contact No must be number")]
        public string CustomerID { get; set; }
        [Required]
        [Range(1, 9999)]
        [Display(Name = "Amount")]
        [RegularExpression(@"^\d*(\.\d{1,4})?$", ErrorMessage = "Enter Valid Amount")]
        public decimal RechargeAmount { get; set; }
        [Required]
        [Display(Name = "Service Name")]
        public string Service_Name { get; set; }
        public string Service_Key { get; set; }
        [Required]
        [MaxLength(10, ErrorMessage = "Pin no is not greater then 10 digit")]
        [MinLength(6, ErrorMessage = "Pin no is not less then 6 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Pin Code must be number")]
        public string PIN { get; set; }
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
        [NotMapped]
        public string GassReferenceNo { get; set; }
    }
}