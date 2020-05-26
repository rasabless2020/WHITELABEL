namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;

    [Table("COMM_SLAB_DMR_PAYMENT")]
    public class TBL_COMM_SLAB_DMR_PAYMENT
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SLN { get; set; }
        public long SLAB_ID { get; set; }
        public long MEM_ID { get; set; }
        public decimal SLAB_FROM { get; set; }
        public decimal SLAB_TO { get; set; }
        public string COMM_TYPE { get; set; }
        public decimal COMM_PERCENTAGE { get; set; }
        public bool COMM_STATUS { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string DMT_TYPE { get; set; }
    }
}
