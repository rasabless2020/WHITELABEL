using System.ComponentModel.DataAnnotations;

namespace WHITELABEL.Web.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
    public class ForgettenPassword
    {
        [Required]
        [EmailAddress(ErrorMessage = "Not a valid email address")]
        public string Email { get; set; }
    }
    public class ResetPasswordModel
    {
        [Required]
        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "New password and confirmation does not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string ReturnToken { get; set; }
    }
    //public class TBL_PASSWORD_RESET
    //{
    //    public string ID { get; set; }
    //    public string EmailID { get; set; }
    //    public DateTime Time { get; set; }
    //}
}