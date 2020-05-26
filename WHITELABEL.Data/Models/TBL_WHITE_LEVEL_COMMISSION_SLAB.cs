namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;

    [Table("WHITE_LEVEL_COMMISSION_SLAB")]
    public class TBL_WHITE_LEVEL_COMMISSION_SLAB
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SLN { get; set; }
        [Required(ErrorMessage = "Slab name is required")]
        [RegularExpression("^[0-9a-zA-Z ]+$", ErrorMessage = "Enter valid slab name")]
        public string SLAB_NAME { get; set; }
        [Required(ErrorMessage = "Slab details is required")]
        [RegularExpression("^[0-9a-zA-Z ]+$", ErrorMessage = "Enter valid slab details")]
        public string SLAB_DETAILS { get; set; }
        [Required(ErrorMessage = "Slab type is required")]
        [RegularExpression("^[0-9a-zA-Z ]+$", ErrorMessage = "Enter valid slab type")]
        public long SLAB_TYPE { get; set; }
        public bool SLAB_STATUS { get; set; }
        public DateTime DATE_CREATED { get; set; }
        public long MEM_ID { get; set; }
        public string SLAB_TDS { get; set; }
        //public long ASSIGNED_SLAB { get; set; }
        [NotMapped]
        public string Slab_TypeName { get; set; }
    }
}
