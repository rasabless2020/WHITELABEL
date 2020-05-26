namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("TRACE_MEMBER_LOGIN_DETAILS")]
    public class TBL_TRACE_MEMBER_LOGIN_DETAILS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public long MEM_ID { get; set; }
        public DateTime LOGINTIME { get; set; }
        public DateTime? LOGOUTTIME { get; set; }
        public int STATUS { get; set; }
        public string IP_ADDRESS { get; set; }
    }
}
