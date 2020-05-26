namespace WHITELABEL.Web.Areas.Distributor.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;

    public class DISTReservedCreditLimitModel
    {
        [Required]
        [Display(Name = "Please Enter Merchant Name")]
        public long FROM_MEM_ID { get; set; }
        [Required]
        [Display(Name = "Reserved Credit Limit Amount")]
        [Range(typeof(Decimal), "0", "999999", ErrorMessage = "{0} must be a decimal/number between {1} and {2}.")]
        public decimal ReservedCreditLimit { get; set; }
        [NotMapped]
        public string From_Member_Name { get; set; }
    }
}