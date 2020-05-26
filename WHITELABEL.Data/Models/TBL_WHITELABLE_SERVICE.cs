namespace WHITELABEL.Data.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("WHITELABLE_SERVICE")]
    public class TBL_WHITELABLE_SERVICE
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SL_NO { get; set; }
        public long MEMBER_ID { get; set; }
        public long SERVICE_ID { get; set; }
        public bool ACTIVE_SERVICE { get; set; }
        public bool? CREDIT_LIMITSTATUS { get; set; }
        [NotMapped]
        public string ServiceName { get; set; }
        [NotMapped]
        public string UserName { get; set; }
    }
}
