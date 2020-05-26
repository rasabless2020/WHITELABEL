namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("TAX_MASTERS")]
    public class TBL_TAX_MASTERS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SLN { get; set; }
        [Required(ErrorMessage = "Tax name is required")]
        [MaxLength(250, ErrorMessage = "Tax name is not greater then 250 digit")]
        [MinLength(3, ErrorMessage = "Tax name is not less then 3 digit")]
        public string TAX_NAME { get; set; }
        [Required(ErrorMessage = "Tax description is required")]
        public string TAX_DESCRIPTION { get; set; }
        public int TAX_STATUS { get; set; }
        [Required(ErrorMessage = "Tax mode is required")]
        public string TAX_MODE { get; set; }
        [Required(ErrorMessage = "Tax amount is required.")]
        [RegularExpression(@"^[0-9]+(\.[0-9]{1,2})$", ErrorMessage = "Tax amount decimal number with maximum 2 decimal places.")]
        public decimal TAX_VALUE { get; set; }
        public DateTime TAX_CREATED_DATE { get; set; }
        public long MEM_ID { get; set; }
    }
}
