namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("SETTINGS_SERVICES_MASTER")]
    public class TBL_SETTINGS_SERVICES_MASTER
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SLN { get; set; }
        [Required]
        [Display(Name = "Service Name")]
        [StringLength(255, ErrorMessage = "Service Name must be 255 digit")]
        [RegularExpression("^[0-9a-zA-Z ]+$", ErrorMessage = "Enter valid service name")]
        public string SERVICE_NAME { get; set; }
        public string SERVICE_DESC { get; set; }
        public string SERVICE_CREATOR { get; set; }
        public DateTime UPDATED_ON { get; set; }
        public long MEM_ID { get; set; }
        public bool ACTIVESTATUS { get; set; }
    }
}
