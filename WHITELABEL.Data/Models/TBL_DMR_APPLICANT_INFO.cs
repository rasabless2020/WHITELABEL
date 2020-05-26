namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("DMR_APPLICANT_INFO")]
    public class TBL_DMR_APPLICANT_INFO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        [Required]
        [Display(Name = "Mobile No.")]
        [MaxLength(15, ErrorMessage = "Mobile no not greater then 15 digit")]
        [MinLength(10, ErrorMessage = "Mobile no not less then 10 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile must be number")]
        public string MOBILE_NO { get; set; }
        public DateTime INSERTED_DATE { get; set; }
        public long INSERTED_BY { get; set; }
        public int STATUS { get; set; }
        public DateTime UPDATED_DATE { get; set; }
        public long UPDATED_BY { get; set; }
        [Required]
        [Display(Name = "DMR Sender Name")]
        [StringLength(255, ErrorMessage = "Member name must be 255 digit")]
        [RegularExpression("^[0-9a-zA-Z ]+$", ErrorMessage = "Enter valid DMR Sender name")]
        public string DMR_SENDER_NAME { get; set; }
    }
}
