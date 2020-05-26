namespace WHITELABEL.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("SETTING_USER_GROUP_MASTER")]
    public class TBL_SETTING_USER_GROUP_DETAILS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SLN { get; set; }
        public long? USER_GROUP { get; set; }
        public long? SCREEN_ID { get; set; }
        public bool? ADD_OPTION { get; set; }
        public bool? EDIT_OPTION { get; set; }
        public bool? DELETE_OPTION { get; set; }
        public bool? VIEW_OPTION { get; set; }
        public bool? EXPORT_OPTION { get; set; }
    }
}