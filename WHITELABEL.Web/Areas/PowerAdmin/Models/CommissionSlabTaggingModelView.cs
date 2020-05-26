namespace WHITELABEL.Web.Areas.PowerAdmin.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;
    public class CommissionSlabTaggingModelView
    {
        public long SL_NO { get; set; }
        public long WHITE_LEVEL_ID { get; set; }
        public long INTRODUCE_TO_ID { get; set; }
        public long INTRODUCER_ID { get; set; }
        public long RECHARGE_SLAB { get; set; }
        public long BILLPAYMENT_SLAB { get; set; }
        public long DMR_SLAB { get; set; }
        public long AIR_SLAB { get; set; }
        public long BUS_SLAB { get; set; }
        public long HOTEL_SLAB { get; set; }
        public long CASHCARD_SLAB { get; set; }
        public bool STATUS { get; set; }
       
        public DateTime CREATED_DATE { get; set; }
        [NotMapped]
        public string WHITELEVELNAME1 { get; set; }
        [NotMapped]
        public string RechargeName { get; set; }
        [NotMapped]
        public string BillName { get; set; }
        [NotMapped]
        public string DMR_SLAB_Name { get; set; }
        [NotMapped]
        public string AIR_SLAB_Name { get; set; }
        [NotMapped]
        public string BUS_SLAB_Name { get; set; }
        [NotMapped]
        public string HOTEL_SLAB_Name { get; set; }
        [NotMapped]
        public string CASHCARD_SLAB_Name { get; set; }
        [NotMapped]
        public long MobileRechargeSlabdetails { get; set; }
        [NotMapped]
        public long UtilityRechargeSlabdetails { get; set; }
        [NotMapped]
        public long DMRRechargeSlabdetails { get; set; }
        [NotMapped]
        public long AIRSlabdetailsList { get; set; }
        [NotMapped]
        public long BusSlabdetailsList { get; set; }
        [NotMapped]
        public long HotelSlabdetailsList { get; set; }
        [NotMapped]
        public long CashCardSlabdetailsList { get; set; }


    }
}