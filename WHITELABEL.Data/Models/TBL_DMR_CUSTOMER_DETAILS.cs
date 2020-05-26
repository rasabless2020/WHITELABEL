namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("DMR_CUSTOMER_DETAILS")]
    public class TBL_DMR_CUSTOMER_DETAILS
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public long MEM_ID { get; set; }
        public string CUSTOMER_MOBILE { get; set; }       
        public string CUSTOMER_NAME { get; set; }        
        public string ADDRESS { get; set; }        
        public string DOB { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public int STATUS { get; set; }
        public decimal TRANSACTIONLIMIT { get; set; }
        public string REMITTER_ID { get; set; }
        public string API_RESPONSE { get; set; }
        public decimal REMAINING_LIMIT { get; set; }
        public decimal CONSUMED_LIMIT { get; set; }
    }
}
