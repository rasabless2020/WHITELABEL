namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;

    [Table("DETAILS_MEMBER_COMMISSION_SLAB")]
    public class TBL_DETAILS_MEMBER_COMMISSION_SLAB
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SL_NO { get; set; }
        public long WHITE_LEVEL_ID { get; set; }
        public long INTRODUCER_ID { get; set; }
        public long RECHARGE_SLAB { get; set; }
        public long BILLPAYMENT_SLAB { get; set; }
        public long DMR_SLAB { get; set; }
        public long AIR_SLAB { get; set; }
        public long BUS_SLAB { get; set; }
        public long HOTEL_SLAB { get; set; }
        public long CASHCARD_SLAB { get; set; }
        public bool STATUS { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public long INTRODUCE_TO_ID { get; set; }
        [NotMapped]
        public long MEM_ID { get; set; }
        [NotMapped]
        public long MEM_Name { get; set; }
    }
}
