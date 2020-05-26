namespace WHITELABEL.Data.Models
{    
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("MASTER_MEMBER_ROLE")]
    public class TBL_MASTER_MEMBER_ROLE
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ROLE_ID { get; set; }
        [Required]
        public string ROLE_NAME { get; set; }
    }
}
