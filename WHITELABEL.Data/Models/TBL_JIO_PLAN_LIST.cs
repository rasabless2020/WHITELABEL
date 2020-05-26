namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("JIO_PLAN_LIST")]
    public class TBL_JIO_PLAN_LIST
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public string billingType { get; set; }
        public string circle { get; set; }
        public string name { get; set; }
        public string id_Val { get; set; }
        public string price { get; set; }
        public string description { get; set; }
        public DateTime? entrydate { get; set; }
        public string jio_user { get; set; }
    }
}
