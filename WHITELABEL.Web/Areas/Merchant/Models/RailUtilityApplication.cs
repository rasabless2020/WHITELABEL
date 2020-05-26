namespace WHITELABEL.Web.Areas.Merchant.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using WHITELABEL.Data;
    using WHITELABEL.Data.Models;
    using WHITELABEL.Web.Models;
    using WHITELABEL.Web.Helper;
    using System.Data.Entity.Core;
    using WHITELABEL.Web.Areas.Merchant.Models;
    using WHITELABEL.Web.ServiceApi.RECHARGE.PORTIQUE;
    using static WHITELABEL.Web.Helper.InstantPayApi;
    using NonFactors.Mvc.Grid;
    using OfficeOpenXml;
    using System.Threading.Tasks;
    using System.Data.Entity;
    using System.Globalization;
    using log4net;
    using System.ComponentModel.DataAnnotations;

    public class RailUtilityApplication
    {
        public string Member_Name { get; set; }
        
        public string MemberRole { get; set; }
        [Required]
        [Display(Name = "GST No")]
        [MaxLength(50, ErrorMessage = "GST No is not greater then 50 digit")]
        [MinLength(5, ErrorMessage = "GST No is not less then 5 digit")]
        public string GST_No { get; set; }
        [Required]
        [Display(Name = "Aadhaar No")]
        [StringLength(12, ErrorMessage = "Aadhaar card must be 12 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Aadhaar Card must be number")]
        public string AadhaarCard { get; set; }
        [Required]
        [Display(Name = "Pan No")]
        [StringLength(10, ErrorMessage = "Pan card no must be 10 digit")]
        [RegularExpression("^[0-9a-zA-Z ]+$", ErrorMessage = "Enter valid Pan no")]
        public string PanNo { get; set; }

        
        public string EmailId { get; set; }
        
        public string Mobile { get; set; }
        public string Description { get; set; }
    }
}