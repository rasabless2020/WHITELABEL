namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("DEBIT_CREDIT_NOTE_REMARK_SETTING")]
    public class TBL_DEBIT_CREDIT_NOTE_REMARK_SETTING
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SLN { get; set; }
        public string CR_DR_NOTE { get; set; }
    }
}
