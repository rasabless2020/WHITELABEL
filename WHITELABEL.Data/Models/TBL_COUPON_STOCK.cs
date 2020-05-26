namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("COUPON_STOCK")]

    public class TBL_COUPON_STOCK
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long sln { get; set; }
        [Required]
        public long couponHoderID { get; set; }
        [Required]
        [Display(Name = "Purchase Date")]
        public DateTime puchaseDate { get; set; }
        [Required]
        [Display(Name = "Stock Entry Date")]
        public DateTime stockEntryDate { get; set; }
        [Required]
        [Display(Name = "Coupon Type")]
        public long couponType { get; set; }
        [Required]

        //[RegularExpression("^[0-9]*$", ErrorMessage = "Quantity must be numeric")]
        [RegularExpression("^[0-9]{1,5}$", ErrorMessage = "Quantity must be numeric")]
        public int couponQty { get; set; }
        public string Status { get; set; }
        [Required]
        public string Vendor_Name { get; set; }
        [NotMapped]
        public string HolderName { get; set; }
        [NotMapped]
        public string CouponName { get; set; }
        [NotMapped]
        public decimal CouponAmount { get; set; }
    }
}