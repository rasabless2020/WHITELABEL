namespace WHITELABEL.Web.Areas.Admin.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;

    public class CouponCommission
    {
        public long sln { get; set; }
        public long coupon_id { get; set; }
        public long mem_id { get; set; }
        public string Coupon_Name { get; set; }
        public string Comm_TYPE { get; set; }
        public decimal Comm_Value { get; set; }
        //public long Super_Role_Id { get; set; }
        public decimal Super_Comm_Value { get; set; }
        //public long Dist_Role_Id { get; set; }
        public decimal Dist_Comm_value { get; set; }
        //public long Merchant_Role_Id { get; set; }
        public decimal Merchant_Comm_Value { get; set; }

    }
}