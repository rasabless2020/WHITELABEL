namespace WHITELABEL.Web.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;


    public class MemberView
    {
        public string IDValue { get; set;   }
        public string TextValue { get; set; }
    }
    public class ViewBankDetails
    {
        public string BankID { get; set; }
        public string BankName { get; set; }
    }
    public class GetRequisitiondetails
    {
        public string TransactionID { get; set; }
        public string TransId { get; set; }
        public string TransDate { get; set; }
        public string TransUserName { get; set; }
    }
    public class ViewcouponDetails
    {
        public string sln { get; set; }
        public string couponType { get; set; }
    }
}