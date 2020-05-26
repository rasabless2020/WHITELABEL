namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;

    [Table("API_TOKEN")]
    public class TBL_API_TOKEN
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public string TOKEN { get; set; }
        public string APINAME { get; set; }
        public string TOKENTYPE { get; set; }
        public DateTime INSERTEDDATE { get; set; }
        public int STATUS { get; set; }
    }
}
