namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("VENDOR_MASTER")]
    public class TBL_VENDOR_MASTER
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        [Required(ErrorMessage = "The Vender Name field is required.")]
        public string VENDOR_NAME { get; set; }
        [Required(ErrorMessage = "The Vender Type field is required.")]
        public string VENDOR_TYPE { get; set; }
       // public string VENDOR_DESCRIPTION { get; set; }
        public int STATUS { get; set; }
       public DateTime CREATED_DATE { get; set;     }
        //public long CREATED_BY { get; set; }
    }
}
