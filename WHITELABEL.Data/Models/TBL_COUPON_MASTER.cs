
namespace WHITELABEL.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("COUPON_MASTER")]


    public class TBL_COUPON_MASTER
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long sln { get; set; }
        [Required]
        [Display(Name = "Coupon Type")]
        public string couponType { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        [RegularExpression(@"^\d*(\.\d{1,4})?$", ErrorMessage = "Enter Valid Coupon Amount")]
        public decimal vendor_coupon_price { get; set; }
    }
}
