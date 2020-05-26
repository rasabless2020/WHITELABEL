namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("OPERATOR_MASTER")]
    public class TBL_OPERATOR_MASTER
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public string OPERATORNAME { get; set; }
        public string RECHTYPE { get; set; }
        public long PRODUCTID { get; set; }
        public string OPERATORTYPE { get; set;}
    }
}
