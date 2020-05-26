namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("COUPON_TRANSFER_LOGS")]

    public class TBL_COUPON_TRANSFER_LOGS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SLN { get; set; }
        [Required]
        [Display(Name = "Reference")]
        public string REFERENCE_NO { get; set; }
        public long TO_MEMBER { get; set; }
        public long FROM_MEMBER { get; set; }
        [Required]
        [Display(Name = "Request Date")]
        public DateTime REQUEST_DATE { get; set; }
        public DateTime REQUEST_TIME { get; set; }
        [Required]
        [Display(Name = "Quantity")]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Quantity must be numeric")]
        [RegularExpression("^[0-9]{1,5}$", ErrorMessage = "Quantity must be numeric")]
        public int QTY { get; set; }
        [Required]
        [Display(Name = "Coupon Type")]
        public long COUPON_TYPE { get; set; }
        public string STATUS { get; set; }
        public string APPROVED_BY { get; set; }
        public DateTime? APPROVAL_DATE { get; set; }
        public DateTime? APPROVAL_TIME { get; set; }
        public string REMARKS { get; set; }
        public long COMM_SLAB_ID { get; set; }
        public decimal SELL_VALUE_RATE { get; set; }
        public decimal GROSS_SELL_VALUE { get; set; }
        public decimal NET_SELL_VALUE { get; set; }
        public bool IS_OVERRIDE_SELL_VALUE { get; set; }
        public long MEM_ROLE { get; set; }
        public string IS_GST_APPLIED { get; set; }
        public decimal GST_PERCENTAGE { get; set; }
        public decimal GST_VALUE { get; set; }
        public string TO_MEMBER_GST_NO { get; set; }
        public string FROM_MEMBER_GST_NO { get; set; }
        public string IS_TDS_APPLIED { get; set; }
        public decimal TDS_PERCENTAGE { get; set; }
        public decimal TDS_VALUE { get; set; }
        public DateTime? TDS_SUBMIT_DATE { get; set; }
        public string TDS_SUBMIT_REF_NO { get; set; }
        public DateTime? REAL_TRANSFER_DATE { get; set; }
        public DateTime? REAL_TRANSFER_TIME { get; set; }
        public string COUPON_START_NO { get; set; }
        public string COUPON_END_NO { get; set; }
        public string VENDOR_PAY_REF_NO { get; set; }
        public decimal GST_INPUT { get; set; }
        public decimal GST_OUTPUT { get; set; }

        [NotMapped]
        public string COUPON_Name { get; set; }
        [NotMapped]
        public decimal CouponPrice { get; set; }
        [NotMapped]
        public string Member_Name { get; set; }
        [NotMapped]
        public string GSTNo { get; set; }
        [NotMapped]
        public string CompanyName { get; set; }
        [NotMapped]
        public string Address { get; set; }
        [NotMapped]
        public string Mem_Mobile { get; set; }
        [NotMapped]
        public decimal TotalAmount { get; set; }
        [NotMapped]
        public string Logo { get; set; }
        [NotMapped]
        public decimal DistributorCommPrice { get; set; }
        [NotMapped]
        public decimal SuperCommPrice { get; set; }
        [NotMapped]
        public decimal DistGapCommPrice { get; set; }
        [NotMapped]
        public decimal SuperGapCommPrice { get; set; }

    }
}
