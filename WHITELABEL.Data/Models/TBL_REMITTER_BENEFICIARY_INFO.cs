namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;

    [Table("REMITTER_BENEFICIARY_INFO")]
    public class TBL_REMITTER_BENEFICIARY_INFO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public string BeneficiaryID { get; set; }
        public string BeneficiaryName { get; set; }
        public string Mobile { get; set; }
        public string Account { get; set; }
        public string Bank { get; set; }
        public string IFSC { get; set; }
        public int Status { get; set; }
        public int IMPS { get; set; }
        public DateTime? Last_Success_Date { get; set; }
        public string Last_success_Name { get; set; }
        public string Last_Sucess_IMPS { get; set; }
        public string RemitterID { get; set; }
        public long MEM_ID { get; set; }
        public int IsActive { get; set; }
        public string Verification_Status { get; set;    }
        public string BankRefNo { get; set; }
        public string Ipay_Id { get; set; }
    }
}
