namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;

    [Table("DMR_REMITTER_INFORMATION")]
    public class TBL_DMR_REMITTER_INFORMATION
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public string RemitterID { get; set; }
        public string Name { get; set; }
        public string MobileNo { get; set; }
        public string Address { get; set;    }
        public string Pincode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int KYCStatus { get; set; }
        public decimal ConsumedLimited { get; set; }
        public decimal RemainingLimit { get;  set ; }
        public int Status { get; set; }
        public string StatusCode { get; set; }
        public long MEM_ID { get; set; }
        public DateTime InsertedDate { get; set; }
        public int UpdateStatus { get; set; }
        public decimal Total { get; set; }
        public string KYCDocs { get; set; }
        public decimal Perm_txn_limit { get; set; }
    }
}
