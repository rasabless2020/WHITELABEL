namespace WHITELABEL.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("SETTING_USER_GROUP_MASTER")]
    public class TBL_SETTING_USER_GROUP_MASTER
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long USER_GROUP { get; set; }
        [Required]
        [Display(Name = "Group name")]
        public string USER_GROUP_NAME { get; set; }
    }
}