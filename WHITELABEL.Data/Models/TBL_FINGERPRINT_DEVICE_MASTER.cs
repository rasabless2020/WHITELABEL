namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("FINGERPRINT_DEVICE_MASTER")]
    public class TBL_FINGERPRINT_DEVICE_MASTER
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        [Required]
        [Display(Name = "Device Name")]
        [StringLength(250, ErrorMessage = "Device name must be 250 digit")]
        //[RegularExpression("^[0-9a-zA-Z ]+$", ErrorMessage = "Enter valid Device name")]        
        public string DEVICE_NAME { get; set; }
        [Required]
        [Display(Name = "Device Model No.")]
        [StringLength(150, ErrorMessage = "Device model no must be 150 digit")]
        public string DEVICE_MODELNO { get; set; }
        [Required]
        [Display(Name = "Device Code")]
        [StringLength(40, ErrorMessage = "Device code must be 40 digit")]
        public string DEVICE_CODE { get; set; }
        public int STATUS { get; set; }
        public long MEM_ID { get; set; }
        public DateTime CREATED_DATE { get; set; }
    }
}
