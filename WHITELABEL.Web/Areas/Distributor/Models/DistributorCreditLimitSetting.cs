namespace WHITELABEL.Web.Areas.Distributor.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;


    public class DistributorCreditLimitSetting
    {
        public long FROM_MEM_ID { get; set; }
        public decimal CREDIT_LIMIT_AMOUNT { get; set; }
        public string CREDIT_LIMIT_DIstription { get; set; }
        public bool MOBILE_RECHARGE { get; set; }
        public bool UTILITY_SERVICES { get; set; }
        public bool DMR { get; set; }
        public bool AIR_TICKET { get; set; }
        public bool BUS_TICKET { get; set; }
        public bool HOTEL_BOOKING { get; set; }
        public bool RAIL_UTILITY { get; set; }
        public decimal reservedCreditLimit { get; set; }
        [NotMapped]
        
        public string From_Member_Name { get; set; }
        [NotMapped]
        public bool AllServices { get; set; }
    }
}