namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;
    [Table("API_COMMISION_STRUCTURE")]
    public class TBL_API_COMMISION_STRUCTURE
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public long WHITELEVEL_ID { get; set; }
        public long MEM_ID { get; set; }
        public long ROLE_ID { get; set; }
        public long SERVICE_ID { get; set; }
        public string OPERATOR_TYPE { get; set; }
        public string OPERATOR_NAME { get; set; }
        public string COMM_PERCENTAGE { get; set; }
        public bool ACTIVESTATUS { get; set; }
        public DateTime INSERTED_DATE { get; set; }
        public long CREATED_BY { get; set; }
        public DateTime UPDATE_DATE{ get; set; }
        public long UPDATEB_BY { get; set; }
        public int ISDELETE { get; set; }
    }
}
