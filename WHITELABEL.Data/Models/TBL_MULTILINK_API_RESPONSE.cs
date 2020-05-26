namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("MULTILINK_API_RESPONSE")]
    public class TBL_MULTILINK_API_RESPONSE
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public string Status { get; set; }
        public string TransId { get; set; }
        public decimal Balance { get; set; }
        public string ServiceName { get; set; }
        public decimal Amount { get; set; }
        public string MobileNo { get; set; }
        public string Message { get; set; }
        public DateTime RechargeDate { get; set; }
        public string IpAddress { get; set; }
        public string RechargeStatus { get; set; }
        public long MEM_ID { get; set; }
        public string RechargeUniqueID { get; set; }
    }
}
