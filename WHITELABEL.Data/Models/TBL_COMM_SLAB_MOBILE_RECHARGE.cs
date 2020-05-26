namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;

    [Table("COMM_SLAB_MOBILE_RECHARGE")]
    public class TBL_COMM_SLAB_MOBILE_RECHARGE
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SLN { get; set; }
        public long SLAB_ID { get; set; }
        public long MEM_ID { get; set; }
        public string OPERATOR_CODE { get; set; }
        public string OPERATOR_NAME { get; set; }
        public string OPERATOR_TYPE{ get; set; }
        public string COMM_TYPE { get; set; }
        public decimal COMM_PERCENTAGE { get; set; }
        public bool COMM_STATUS { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public long SUPER_ROLE_ID { get; set; }
        public long DISTRIBUTOR_ROLE_ID { get; set; }
        public long MERCHANT_ROLE_ID { get; set; }
        public decimal SUPER_COM_PER { get; set; }
        public decimal DISTRIBUTOR_COM_PER { get; set; }
        public decimal MERCHANT_COM_PER { get; set; }
        public string COMMISSION_TYPE { get; set; }
        [NotMapped]
        public string SLAB_NAME { get; set;  }
        //public decimal TDS { get; set; }
    }
}
