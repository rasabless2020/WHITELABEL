namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("DMT_BANK_MARGIN")]
    public class TBL_DMT_BANK_MARGIN
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set;   }        
        public decimal DMTBANK_MARGIN { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public long CREATED_BY { get; set;   }
    }
}
