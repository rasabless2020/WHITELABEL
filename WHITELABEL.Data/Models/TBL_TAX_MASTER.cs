namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("TAX_MASTER")]
    public class TBL_TAX_MASTER
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public decimal TDS { get; set; }
        public decimal GST { get; set; }
        public int TDS_STATUS { get; set; }
        public int GST_STATUS { get; set; }
    }
}
