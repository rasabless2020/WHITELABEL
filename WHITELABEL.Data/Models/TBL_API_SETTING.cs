namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;
    [Table("API_SETTING")]
    public class TBL_API_SETTING
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public string NAME { get; set; }
        public string VALUE { get; set; }
        public string API_PROVIDER { get; set; }
    }
}
