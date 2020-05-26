namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("MERCHANT_OUTLET_INFORMATION")]

    public class TBL_MERCHANT_OUTLET_INFORMATION
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public long MEM_ID { get; set; }
        public string OUTLETID { get; set; }
        public string MOBILE { get; set; }
        public string EMAIL { get; set; }
        public string OUTLETNAME { get; set; }
        public string CONTACTPERSON { get; set; }
        public string AADHAARNO { get; set; }
        public string PANCARDNO { get; set; }
        public int KYC_STATUS { get; set; }
        public int OUTLET_STATUS { get; set; }
        public DateTime INSERTED_DATE { get; set; }
        public long INSERTED_BY { get; set; }
        public string PINCODE { get; set; }
    }
}
