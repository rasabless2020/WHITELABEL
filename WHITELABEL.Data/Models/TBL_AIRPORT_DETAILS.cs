namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("AIRPORT_DETAILS")]

    public class TBL_AIRPORT_DETAILS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set;}
        public string CITYCODE { get; set; }
        public string CITYNAME { get; set; }
        public string COUNTRYCODE { get; set;}
        public string AIRPORT_TYPE { get; set; }
        public bool ISACTIVE { get; set; }
        public DateTime DELETED_DATE { get; set; }
        public long DELETED_BY { get; set; }
    }
}

