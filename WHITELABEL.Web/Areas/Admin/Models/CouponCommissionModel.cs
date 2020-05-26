namespace WHITELABEL.Web.Areas.Admin.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;
    public class CouponCommissionModel
    {
    }
    public class CouponCommissionListView
    {
        public long Sln { get; set; }
        public long coupon_id { get; set; }
        public string Coupon_Name { get; set; }
        public string Comm_TYPE { get; set; }
        public string Comm_Value { get; set; }
        //public string COMMERTIALS { get; set; }
        //public string BILLING_MODEL { get; set; }
        //public string HSN_SAC { get; set; }
        //public string TDS { get; set; }
        //public decimal DMRFrom { get; set; }
        //public decimal DMRTo { get; set; }
        //public string COMM_TYPE { get; set; }
        //public string DMT_TYPE { get; set; }
        //public string CommissionType { get; set; }
        public string Super_Comm_Value { get; set; }
        public string Dist_Comm_value { get; set; }
        public string Merchant_Comm_Value { get; set; }

    }
    public class CouponCommissoinManagmentmodel
    {
        public long SLN { get; set; }
        public string SLAB_NAME { get; set; }
        public string SLAB_DETAILS { get; set; }
        public long SLAB_TYPE { get; set; }
        public string Slab_TypeName { get; set; }
        public bool SLAB_STATUS { get; set; }
        public long MEM_ID { get; set; }
        public string ASSIGNED_SLAB { get; set; }
        public List<CouponCommissionListView> OperatorDetails { get; set; }



    }
}