namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("COUPON_COMMISSION_SLAB")]

    public class TBL_COUPON_COMMISSION_SLAB
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long sln { get; set; }
        [Required]
        public long coupon_id { get; set; }
        public long mem_Id { get; set; }
        public string Coupon_Name { get; set; }
        public string Comm_TYPE { get; set; }
        public decimal Comm_Value { get; set; }
        public long Super_Role_Id { get; set; }
        public decimal Super_Comm_Value { get; set; }
        public long Dist_Role_Id { get; set; }
        public decimal Dist_Comm_value { get; set; }
        public long Merchant_Role_Id { get; set; }
        public decimal Merchant_Comm_Value { get; set; }
        public bool Coupon_Status { get; set; }
        public DateTime Create_Date { get; set; }
        public string CouponSlab_Name { get; set; }
        public string CouponDetails { get; set; }
        public string Coupon_SlabType { get; set; }

    }
}
