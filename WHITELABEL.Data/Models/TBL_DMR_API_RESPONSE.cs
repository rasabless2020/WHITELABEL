namespace WHITELABEL.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [Table("DMR_API_RESPONSE")]
    public class TBL_DMR_API_RESPONSE
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID{ get; set; }
        public string DMR_TYPE { get; set; }
        public string DMR_API_RESPONSE { get; set; }
        public string DMR_TRANS_ID { get; set; }
        public string DMR_REQ_ID { get; set; }
        public DateTime DMR_RESPONSE_DATE { get; set; }
        public DateTime DMR_RESPONSE_TIME { set; get; }
        public long DMR_USER_ID { get; set; }
        public int DMR_STATUS { get; set; }
        public DateTime DMR_MODEFIED_DATE { get; set; }
        public DateTime DMR_MODEFIED_TIME { get; set; }
        public string DMR_API_RESPONSE_TYPE { get; set; }
    }
}
