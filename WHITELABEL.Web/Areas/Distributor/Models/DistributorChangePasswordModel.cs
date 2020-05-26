namespace WHITELABEL.Web.Areas.Distributor.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;



    public class DistributorChangePasswordModel
    {
        public long MEM_ID { get; set; }
        [Required]
        [Display(Name = "Old Password")]
        [DataType(DataType.Password)]
        [MaxLength(15, ErrorMessage = "Old Password not greater then 15 digit")]
        [MinLength(10, ErrorMessage = "Old Password not less then 10 digit")]
        public string OldUser_pwd { get; set; }
        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [MaxLength(12, ErrorMessage = "Password not greater then 12 digit")]
        [MinLength(10, ErrorMessage = "Password not less then 10 digit")]
        public string User_pwd { get; set; }
        [Required]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [StringLength(12, ErrorMessage = "Comfirm password must be at least 10 characters long", MinimumLength = 10)]
        [System.ComponentModel.DataAnnotations.Compare("User_pwd", ErrorMessage = "The new password and confirm passwords are not matching")]
        public string CONFIRMPASSWORD { get; set; }
    }
    public class ManageUserProfileDetails
    {
        public long MEM_ID { get; set; }
        [Required(ErrorMessage = "Username is required")]
        [RegularExpression("^[0-9a-zA-Z ]+$", ErrorMessage = "Enter valid user name")]
        [MaxLength(30, ErrorMessage = "User name is not greater then 30 digit")]
        [MinLength(5, ErrorMessage = "User name is not less then 5 digit")]
        public string UName { get; set; }
        [Required]
        [Display(Name = "Member Mobile")]
        [MaxLength(15, ErrorMessage = "Mobile no is not greater then 15 digit")]
        [MinLength(10, ErrorMessage = "Mobile no is not less then 10 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile must be number")]
        public string MEMBER_MOBILE { get; set; }
        [Required]
        [Display(Name = "Member Name")]
        [StringLength(255, ErrorMessage = "Member name must be 255 digit")]
        [RegularExpression("^[0-9a-zA-Z ]+$", ErrorMessage = "Enter valid member name")]
        [MaxLength(30, ErrorMessage = "Member name is not greater then 30 digit")]
        [MinLength(5, ErrorMessage = "Member name is not less then 5 digit")]
        public string MEMBER_NAME { get; set; }
        [MaxLength(150, ErrorMessage = "Company name is not greater then 150 digit")]
        [MinLength(5, ErrorMessage = "Company name is not less then 5 digit")]
        public string COMPANY { get; set; }
        [MaxLength(500, ErrorMessage = "Address is not greater then 500 digit")]
        [MinLength(10, ErrorMessage = "Address is not less then 10 digit")]
        public string ADDRESS { get; set; }
        [StringLength(50, ErrorMessage = "Character must be 50 digit")]
        //[RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Only Alphabet required")]
        [RegularExpression(@"[a-zA-Z ]*$", ErrorMessage = "Only Alphabet required")]
        public string CITY { get; set; }
        [MaxLength(10, ErrorMessage = "Pin no is not greater then 10 digit")]
        [MinLength(6, ErrorMessage = "Pin no is not less then 6 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Pin Code must be number")]
        public string PIN { get; set; }
        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [MaxLength(150, ErrorMessage = "Email is not greater then 150 digit")]
        [MinLength(10, ErrorMessage = "Email is not less then 10 digit")]
        [StringLength(255, ErrorMessage = "Email must be 255 digit")]
        public string EMAIL_ID { get; set; }
        public long? MODIFIED_BY { get; set; }
        public DateTime? LAST_MODIFIED_DATE { get; set; }
        public long? DELETED_BY { get; set; }
    }

    public class DropdownSelectbyMonth
    {
        public string MonthID { get; set; }
        public string MonthName { get; set; }
    }
}