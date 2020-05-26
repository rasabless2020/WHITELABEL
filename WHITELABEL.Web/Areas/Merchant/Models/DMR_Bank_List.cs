namespace WHITELABEL.Web.Areas.Merchant.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;

    public class DMR_Bank_List
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string BANK_Name { get; set; }
        public string IMPS_Enabled { get; set; }
        public string AEPS_Enabled { get; set; }
        public string BANK_Sort_Name { get; set; }
        public string BRANCK_Ifsc { get; set; }
        public string IFSC_Alias { get; set; }
        public string BANK_IIN { get; set; }
        public string IS_Down { get; set; }
        public string STATUS_Code { get; set; }
        public string Status { get; set; }
    }
}