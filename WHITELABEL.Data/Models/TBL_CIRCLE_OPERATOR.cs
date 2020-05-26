namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("CIRCLE_OPERATOR")]
    public class TBL_CIRCLE_OPERATOR
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string CIRCLE_NAME { get; set; }
        public string CIRCLE_CODE { get; set; }
        public int STATUS { get; set; }
        public DateTime DATE_VALUE { get; set; }
    }
}
