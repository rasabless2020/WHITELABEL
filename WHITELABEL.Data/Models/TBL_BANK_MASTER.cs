namespace WHITELABEL.Data.Models
{ 
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("BANK_MASTER")]
    public class TBL_BANK_MASTER
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string SLNO { get; set; }
        public string BankName { get; set; }
        public string BankCode { get; set; }
        public string IMPS { get; set; }
        public string NEFT { get; set; }
        public string AccountVerification { get; set; }
        public string BANK_IFSC { get; set; }
        public string BANK_IIN { get; set; }
        public string IS_DOWN { get; set; }
        public string IFSC_ALIAS { get; set; }
    }
}
