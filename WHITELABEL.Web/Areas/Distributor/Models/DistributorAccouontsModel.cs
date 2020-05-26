using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WHITELABEL.Web.Areas.Distributor.Models
{
    public class DistributorAccouontsModel
    {
        public long SlNo { get; set; }
        public string MEMBER_Name { get; set; }
        public string COMPANYNAME { get; set; }
        public string UserName { get; set; }
        public long MEM_ID { get; set; }
        public decimal OpeningAmt { get; set; }
        public decimal Trans_Amt { get; set; }
        public decimal ClosingAmt { get; set; }
        public string Trans_Date { get; set; }
        public DateTime Trans_Time { get; set; }
        public string DR_CR { get; set; }
    }
}