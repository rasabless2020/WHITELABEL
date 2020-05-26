namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;

    [Table("APPLICATION_FOR_RAIL_UTILITY")]
    public class TBL_APPLICATION_FOR_RAIL_UTILITY
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public long PA_ID { get; set; }
        public long ADMIN_ID { get; set; }
        public long SUPER_ID { get; set; }
        public long DIST_ID { get; set; }
        public long APPLIED_MER_ID { get; set; }
        public DateTime APPLICATION_DATE { get; set; }
        public int STATUS { get; set; }
        public DateTime? APPROVE_DATE { get; set; }
        public long? APPROVE_BY { get; set; }
        public DateTime? DECLINE_DATE { get; set; }
        public long? DECLINE_BY { get; set; }
        public long APPLIED_MEM_TYPE { get; set; }
        public string GST_NO { get; set; }
        public string PAN_NO { get; set; }
        public string AADHAAR_NO { get; set; }
        public string DESCRIPTION { get; set; }
        public string APPLICATION_STATUS { get; set; }
        [NotMapped]
        [Required(ErrorMessage = "Please confirm the agreement after applying railway utility. ")]
        [Display(Name = "Please Select Checkbox :")]
        public bool checkValue { get; set;   }
        [NotMapped]
        public string Member_Name { get; set; }
        [NotMapped]
        public string MemberRole { get; set; }
        [NotMapped]
        public string GST_No { get; set; }
        [NotMapped]
        public string AadhaarCard { get; set; }
        [NotMapped]
        public string PanNo { get; set; }
        
        [NotMapped]
        public string EmailId { get; set; }
        [NotMapped]
        public string Mobile { get; set; }
    }
}
