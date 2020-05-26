namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("API_CALLBACK_LOGS")]
    public class TBL_API_CALLBACK_LOGS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SLN { get; set; }
        public string CORELATIONID { get;  set ; }
        public string SERVICE_TYPE { get; set; }
        public string TRANSACTION_NO { get; set;     }
        public string OPERATOR_ID { get;   set ; }
        public string STATUS { get; set; }
        public string RES_CODE { get; set;   }
        public string RES_MSG { get; set;    }
        public DateTime CALLBACK_DATETIME { get; set; }
        public bool CALLBACK_DATAUPDATE { get; set;  }
        public string CALLBACK_BY { get; set; }
    }
}
