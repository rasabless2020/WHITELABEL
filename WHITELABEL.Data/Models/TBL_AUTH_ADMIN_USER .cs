namespace WHITELABEL.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("AUTH_ADMIN_USER")]
    public class TBL_AUTH_ADMIN_USER
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long USER_ID { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string USER_NAME { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string USER_PASSWORD_MD5 { get; set; }

        [Required]
        [MaxLength(4), MinLength(4)]
        public string USER_AUTH_PIN { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string USER_EMAIL { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Mobile")]
        public string USER_MOBILE { get; set; }

        public bool ACTIVE_USER { get; set; }

        public long USER_GROUP { get; set; }
    }
}