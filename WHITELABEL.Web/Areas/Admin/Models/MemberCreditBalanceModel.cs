namespace WHITELABEL.Web.Areas.Admin.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;


    public class MemberCreditBalanceModel
    {
        public long SLN { get; set; }
        public long MEMBER_ROLE { get; set; }
        public long FROM_MEM_ID { get; set; }
        [NotMapped]
        [Required]
        [Display(Name = "user name")]
        public string FromUser { get; set; }
        [Required]
        [Display(Name = "Credit type")]
        public string CREDIT_TYPE { get; set; }
        [Required]
        [Display(Name = "Amount")]
        [Range(typeof(Decimal), "0", "999999", ErrorMessage = "{0} must be a decimal/number between {1} and {2}.")]               
        public decimal CREDIT_AMOUNT { get; set; }
        public string CREDIT_NOTE_DESCRIPTION { get; set; }
        [NotMapped]
        public string Member_RoleName { get; set; }
        public string GSTAPPLY { get; set; }
        public string TDSAPPLY { get; set; }
    }
}