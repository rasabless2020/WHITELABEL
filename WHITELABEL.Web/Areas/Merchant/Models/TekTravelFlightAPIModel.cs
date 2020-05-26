namespace WHITELABEL.Web.Areas.Merchant.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;


    public class FlightSearch
    {
        public string TokenId { get; set; }
        public string AdultCount { get; set; }
        public string ChildCount { get; set; }
        public string InfantCount { get; set; }
        public string DirectFlight { get; set; }
        public string OneStopFlight { get; set; }
        public string JourneyType { get; set; }
        public string PreferredAirlines { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string FlightCabinClass { get; set; }
        public string PreferredDepartureTime { get; set; }
        public string PreferredArrivalTime { get; set; }
        public string Sources { get; set; }
    }
    public class AdvanceSearch
    {
        public string AdultCount { get; set; }
        public string ChildCount { get; set; }
        public string InfantCount { get; set; }
        public string TokenId { get; set; }
        public string TraceId { get; set; }
        public string ResultIndex { get; set; }
        public int Source { get; set; }
        public string IsLCC { get; set; }
        public string IsRefundable { get; set; }
        public string AirlineRemark { get; set; }
        public int TripIndicator { get; set; }
        public int SegmentIndicator { get; set; }
        public string AirlineCode { get; set; }
        public string AirlineName { get; set; }
        public string FlightNumber { get; set; }
        public string FareClass { get; set; }
        public string OperatingCarrier { get; set; }
    }

    public class GetCalanderModel
    {
        public string JourneyType { get; set; }
        public string TokenId { get; set; }
        public string PreferredAirlines { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string FlightCabinClass { get; set; }
        public string PreferredDepartureTime { get; set; }
        public string Sources { get; set; }
    }
    public class UpdateCalenderFareofDayModel
    {
        public string JourneyType { get; set; }
        public string TokenId { get; set; }
        public string PreferredAirlines { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string FlightCabinClass { get; set; }
        public string PreferredDepartureTime { get; set; }
        public string Sources { get; set; }
    }

    public class TekTravelFlightAPIModel
    {
        public string ResultIndex { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PaxType { get; set; }
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string PassportNo { get; set; }
        public string PassportExpiry { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Currency { get; set; }
        public string BaseFare { get; set; }
        public string Tax { get; set; }
        public string YQTax { get; set; }
        public string AdditionalTxnFeePub { get; set; }
        public string AdditionalTxnFeeOfrd { get; set; }
        public string OtherCharges { get; set; }
        public string Discount { get; set; }
        public string PublishedFare { get; set; }
        public string OfferedFare { get; set; }
        public string TdsOnCommission { get; set; }
        public string TdsOnPLB { get; set; }
        public string TdsOnIncentive { get; set; }
        public string ServiceFee { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string Nationality { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }
        public string IsLeadPax { get; set; }
        public string FFAirlineCode { get; set; }
        public string FFNumber { get; set; }
        public string GSTCompanyAddress { get; set; }
        public string GSTCompanyContactNumber { get; set; }
        public string GSTCompanyName { get; set; }
        public string GSTNumber { get; set; }
        public string GSTCompanyEmail { get; set; }
        public string TokenId { get; set; }
        public string TraceId { get; set; }
    }

    public class GetTicketMoel
    {
        public string PreferredCurrency { get; set; }
        public string ResultIndex { get; set; }
        public string AgentReferenceNo { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PaxType { get; set; }
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string PassportNo { get; set; }
        public string PassportExpiry { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string BaseFare { get; set; }
        public string Tax{ get; set; }
        public string YQTax { get; set; }
        public string AdditionalTxnFeePub { get; set; }
        public string AdditionalTxnFeeOfrd { get; set; }
        public string OtherCharges { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }
        public string IsLeadPax { get; set; }
        public string FFAirlineCode { get; set; }
        public string FFNumber { get; set; }
        public string BeggWayType { get; set; }
        public string BeggCode { get; set; }
        public string BeggDescription { get; set; }
        public string BeggWeight { get; set; }
        public string BeggBaseCurrencyPrice { get; set; }
        public string BeggBaseCurrency { get; set; }
        public string BeggCurrency { get; set; }
        public string BeggPrice { get; set; }
        public string BeggOrigin { get; set; }
        public string BeggDestination { get; set; }
        public string MealWayType { get; set; }
        public string MealCode { get; set; }
        public string MealDescription { get; set; }
        public string MealAirlineDescription { get; set; }
        public string MealQuantity { get; set; }
        public string MealBaseCurrency { get; set; }
        public string MealBaseCurrencyPrice { get; set; }
        public string MealCurrency { get; set; }
        public string MealPrice { get; set; }
        public string MealOrigin { get; set; }
        public string MealDestination { get; set; }
        public List<string> SeatDynamic { get; set; }
        public string GSTCompanyAddress { get; set;  }
        public string GSTCompanyContactNumber { get; set; }
        public string GSTCompanyName { get; set; }
        public string GSTNumber { get; set; }
        public string GSTCompanyEmail { get; set; }
        public string TokenId { get; set; }
        public string TraceId { get; set; }
    }

    public class SendCancelRequest
    {
        public string BookingId { get; set; }
        public int RequestType { get; set; }
        public int CancellationType { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public List<string> TicketId { get; set; }
        public string Remarks { get; set; }
        public string TokenId { get; set; }
    }

    public class FlightSearchParameter
    {
        public string Tripmode { get; set; }
        public string FromAirportsName { get; set; }
        public string FromCityCode { get; set; }
        public string TOAirportName { get; set; }
        public string TOAirportCode { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string TravelType { get; set; }
        public string Adult { get; set; }
        public string Child { get; set; }
        public string Infant { get; set; }
    }
}