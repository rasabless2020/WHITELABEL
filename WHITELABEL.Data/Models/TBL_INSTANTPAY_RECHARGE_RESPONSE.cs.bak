namespace WHITELABEL.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;

    [Table("INSTANTPAY_RECHARGE_RESPONSE")]
    public class TBL_INSTANTPAY_RECHARGE_RESPONSE
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public string Ipay_Id { get; set; }
        public string AgentId { get; set; }
        public string Opr_Id { get; set; }
        public string AccountNo { get; set; }
        public string Sp_Key { get; set; }
        public decimal Trans_Amt { get; set; }
        public decimal Charged_Amt { get; set; }
        public decimal Opening_Balance { get; set; }
        public DateTime DateVal { get; set; }
        public string Status { get; set; }
        public string Res_Code { get; set; }
        public string res_msg { get; set; }
        public long Mem_ID { get; set; }
        public string RechargeType { get; set; }
    }
}
