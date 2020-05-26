namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("AUTH_AUDIT_LOG")]
    public class TBL_AUTH_AUDIT_LOG
    {
        public long SL_NO { get; set; }
        public long? MEM_ID { get; set; }
        public long? ROLE { get; set; }
        public DateTime? LOG_DATE_TIME { get; set; }
        public string LOG_TYPE { get; set; }
        public string USER_IP { get; set; }
        public string CITY { get; set; }
        public string STATE { get; set; }
        public string COUNTRY { get; set; }
        public long? WHITE_LEVEL_ID { get; set; }
    }
}