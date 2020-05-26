namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("NOTIFICATION_SETTING")]
    public class TBL_NOTIFICATION_SETTING
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set;   }
        public long MEM_ID { get; set; }
        [Required]
        [Display(Name = "Notification Subject")]
        [StringLength(500, ErrorMessage = "Subject must be 500 digit")]
        public string NOTIFICATION_SUBJECT { get; set; }
        [Required]
        [Display(Name = "Notification Description")]
        [DataType(DataType.MultilineText)]
        public string NOTIFICATION_DESCRIPTION { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime? NOTIFICATION_DATE { get; set; }
        public DateTime? NOTIFICATION_TIME { get; set; }
        [Required ]
        [Display(Name = "Notification Status")]
        public int STATUS { get; set; }
    }
}
