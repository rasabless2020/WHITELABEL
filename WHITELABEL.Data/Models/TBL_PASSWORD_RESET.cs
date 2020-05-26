namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("PASSWORD_RESET")]
    public class TBL_PASSWORD_RESET
    {
        public string ID { get; set; }
        public string EmailID { get; set; }
        public DateTime Time { get; set; }
    }
}
