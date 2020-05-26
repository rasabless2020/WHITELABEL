namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("COUNTRY")]
    public class TBL_COUNTRY
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int COUNTRY_ID { get; set; }
        public string COUNTRY_NAME { get; set; }
        public string COUNTRY_ISO_CODE { get; set; }
        public string COUNTRY_ISD_CODE { get; set; }
    }
}
