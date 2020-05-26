namespace WHITELABEL.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("SETTINGS_SCREEN")]
    public class TBL_SETTINGS_SCREEN
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SCREEN_ID { get; set; }
        [Required]
        public string SCREEN_NAME { get; set; }
        public string SCREEN_DETAILS { get; set; }
    }

}