using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WHITELABEL.Data;
using WHITELABEL.Web.Areas.Merchant.Models;

namespace WHITELABEL.Web.Helper
{

    public static class Tek_TravelAPI
    {
        public static string root = "http://api.tektravels.com";
        public static string token = "4e364ee6-31c4-4311-aab0-3de3c6561b0c";
        public static string EndUserIp = "14.195.225.209";
        public static string ApiIntegrationNew = "ApiIntegrationNew";
        public static string TokenAgencyId = "46509";
        public static string TokenMemberId = "46483";

        public static dynamic FlightAPIGenerateToken(string UserName, string password)
        {
            string url = root + "/SharedServices/SharedData.svc/rest/Authenticate";
            var param = new Dictionary<string, string> {
                            { "ClientId",ApiIntegrationNew},
                            { "UserName" , UserName },
                            { "Password" , password },
                            {"EndUserIp",EndUserIp }
                        };
            string valueparameter = JsonConvert.SerializeObject(param);
            var res = GetResponse(valueparameter, url);
            if (res.Status == "1")
            {
                string token = res.TokenId;
                return res;
            }
            else
            {
                //return InstantPayError.GetError(res.statuscode.Value);
                return res;
            }
        }
        public static dynamic FlightAPILogout()
        {
            string url = root + "/SharedServices/SharedData.svc/rest/Logout";
            var param = new Dictionary<string, string> {
                            { "ClientId",ApiIntegrationNew},
                            { "TokenAgencyId" , TokenAgencyId },
                            { "TokenMemberId" , TokenMemberId },
                            {"EndUserIp",EndUserIp },
                            {"TokenId",token }
                        };
            string valueparameter = JsonConvert.SerializeObject(param);
            var res = GetResponse(valueparameter, url);
            if (res.Status == "1")
            {
                // string token = res.TokenId;
                return res;
            }
            else
            {
                //return InstantPayError.GetError(res.statuscode.Value);
                return res;
            }
        }
        public static dynamic GetAgentBalance()
        {
            string url = root + "/SharedServices/SharedData.svc/rest/GetAgencyBalance";
            var param = new Dictionary<string, string> {
                            { "ClientId",ApiIntegrationNew},
                            { "TokenAgencyId" , TokenAgencyId },
                            { "TokenMemberId" , TokenMemberId },
                            {"EndUserIp",EndUserIp },
                            {"TokenId",token }
                        };
            string valueparameter = JsonConvert.SerializeObject(param);
            var res = GetResponse(valueparameter, url);
            if (res.Status == "1")
            {
                // string token = res.TokenId;
                return res;
            }
            else
            {
                //return InstantPayError.GetError(res.statuscode.Value);
                return res;
            }
        }

        //public static dynamic SerachFlight( string tokenIdVal,string AdultCount, string ChildCount, string InfantCount, string DirectFlight, string OneStopFlight, string JourneyType, string PreferredAirlines,string Origin,string Destination,string FlightCabinClass,string PrefDeptTime, string PrefArrivTime,string Sources)
        public static dynamic SerachFlight(FlightSearch objsearch)
        {
            string url = root + "/BookingEngineService_Air/AirService.svc/rest/Search";
            dynamic product = new JObject();
            product.EndUserIp = EndUserIp;
            product.TokenId = objsearch.TokenId;
            product.AdultCount = objsearch.AdultCount;
            product.ChildCount = objsearch.ChildCount;
            product.InfantCount = objsearch.InfantCount;

            product.DirectFlight = objsearch.DirectFlight;
            product.OneStopFlight = objsearch.OneStopFlight;
            product.JourneyType = objsearch.JourneyType;
            product.PreferredAirlines = objsearch.PreferredAirlines;
            dynamic segmentval = new JObject();
            segmentval.Origin = objsearch.Origin;
            segmentval.Destination = objsearch.Destination;
            segmentval.FlightCabinClass = objsearch.FlightCabinClass;
            segmentval.PreferredDepartureTime = objsearch.PreferredDepartureTime;
            segmentval.PreferredArrivalTime = objsearch.PreferredArrivalTime;
            product.Segments = new JArray(segmentval);
            //product.Sources = new JArray(Sources);
            string SearchparamValue = JsonConvert.SerializeObject(product);

            //string valueparameter = JsonConvert.SerializeObject(SearchparamValue);
            //string SearchparamValue = JsonConvert.SerializeObject(product);
            var res = GetResponse(SearchparamValue, url);
            if (res.Response.ResponseStatus == "1")
            {
                // string token = res.TokenId;
                return res;
            }
            else
            {
                //return InstantPayError.GetError(res.statuscode.Value);
                return res;
            }
        }
        //public static dynamic AdvanceSearchFlightSearch(string tokenIdVal, string AdultCount, string ChildCount, string InfantCount,string TokenId,string TraceId,string ResultIndex,string Source,string IsLCC,string IsRefundable,string AirlineRemark,string TripIndicator,string SegmentIndicator,string AirlineCode,string AirlineName,string FlightNumber,string FareClass,string OperatingCarrier)
        public static dynamic AdvanceSearchFlightSearch(AdvanceSearch objadvance)
        {
            string url = root + "/BookingEngineService_Air/AirService.svc/rest/PriceRBD";
            //dynamic product = new JObject();
            //product.AdultCount = AdultCount;
            //product.ChildCount = ChildCount;
            //product.InfantCount = InfantCount;
            //product.EndUserIp = EndUserIp;
            //product.TokenId = TokenId;
            //product.TraceId = TraceId;            

            //dynamic AirSearchResult = new JObject();
            //AirSearchResult.ResultIndex = ResultIndex;
            //AirSearchResult.Source = Source;
            //AirSearchResult.IsLCC = IsLCC;
            //AirSearchResult.IsRefundable = IsRefundable;
            //AirSearchResult.AirlineRemark = AirlineRemark;            
            

            //dynamic Segments = new JObject();
            //Segments.TripIndicator = TripIndicator;
            //Segments.SegmentIndicator = SegmentIndicator;
            //dynamic segmentvalue = new JArray(Segments);
            //dynamic Airline = new JObject();
            //Airline.AirlineCode = AirlineCode;
            //Airline.AirlineName = AirlineName;
            //Airline.FlightNumber = FlightNumber;
            //Airline.FareClass = FareClass;
            //Airline.OperatingCarrier = OperatingCarrier;
            //Segments.Airline = new JObject(Airline);

            ////JArray array = JArray.FromObject(Segments);

            ////AirSearchResult.Segments[0] =new JArray(Segments);
            ////AirSearchResult.Segments = new JArray(jary);
            //product.AirSearchResult = new JArray(AirSearchResult);
            //string SearchparamValue = JsonConvert.SerializeObject(product);


            AdvanceSearchModel mainobj = new AdvanceSearchModel();
            mainobj.AdultCount = objadvance.AdultCount;
            mainobj.ChildCount = objadvance.ChildCount;
            mainobj.InfantCount = objadvance.InfantCount;
            mainobj.EndUserIp = EndUserIp;
            mainobj.TokenId = objadvance.TokenId;
            mainobj.TraceId = objadvance.TraceId;

            AirSearchResult objairsearch = new AirSearchResult();
            objairsearch.ResultIndex = objadvance.ResultIndex;
            objairsearch.IsLCC = objadvance.IsLCC;
            objairsearch.Source = Convert.ToInt32(objadvance.Source);
            objairsearch.IsRefundable = objadvance.IsRefundable;
            objairsearch.AirlineRemark = objadvance.AirlineRemark;

            var listAirSearchResult = new List<AirSearchResult>();
            listAirSearchResult.Add(objairsearch);
            var list= new List<List<Segment>>();

            Segment objsegment = new Segment();
            objsegment.TripIndicator = objadvance.TripIndicator;
            objsegment.SegmentIndicator = objadvance.SegmentIndicator;            
            Airline objairline = new Airline();
            objairline.AirlineCode = objadvance.AirlineCode;
            objairline.AirlineName = objadvance.AirlineName;
            objairline.FlightNumber = objadvance.FlightNumber;
            objairline.FareClass = objadvance.FareClass;
            objairline.OperatingCarrier = objadvance.OperatingCarrier;
            objsegment.Airline = objairline;

            list.Add(new List<Segment> { objsegment });
            objairsearch.Segments = list;
            mainobj.AirSearchResult = listAirSearchResult;

            string SearchparamValue1 = JsonConvert.SerializeObject(mainobj);


            var res = GetResponse(SearchparamValue1, url);
            if (res.Response.ResponseStatus == "1")
            {
                // string token = res.TokenId;
                return res;
            }
            else
            {
                //return InstantPayError.GetError(res.statuscode.Value);
                return res;
            }
        }


        //public static dynamic GetCalenderFare(string GetToken,string JourneyType,string PreferredAirlines, string Origin,string destination,string FlightCabinClass,string PreferredDepartureTime)
        public static dynamic GetCalenderFare(GetCalanderModel objgetcalender)
        {
            string url = root + "/BookingEngineService_Air/AirService.svc/rest/GetCalendarFare";
            dynamic product = new JObject();
            product.EndUserIp = EndUserIp;
            product.TokenId = objgetcalender.TokenId;                   
            product.JourneyType = objgetcalender.JourneyType;
            product.PreferredAirlines = objgetcalender.PreferredAirlines;
            dynamic segmentval = new JObject();
            segmentval.Origin = objgetcalender.Origin;
            segmentval.Destination = objgetcalender.Destination;
            segmentval.FlightCabinClass = objgetcalender.FlightCabinClass;            
            segmentval.PreferredDepartureTime = objgetcalender.PreferredDepartureTime;
            product.Segments = new JArray(segmentval);
            product.Sources = null;
            string SearchparamValue = JsonConvert.SerializeObject(product);            
            var res = GetResponse(SearchparamValue, url);
            if (res.Response.ResponseStatus == "1")
            {
                return res;
            }
            else
            {             
                return res;
            }
        }

        //public static dynamic Update_Calender_FateOfDay(string GetToken, string JourneyType, string PreferredAirlines, string Origin, string destination, string FlightCabinClass, string PreferredDepartureTime)
        public static dynamic Update_Calender_FateOfDay(UpdateCalenderFareofDayModel objupcomming)
        {
            string url = root + "/BookingEngineService_Air/AirService.svc/rest/UpdateCalendarFareOfDay";
            dynamic product = new JObject();
            product.EndUserIp = EndUserIp;
            product.TokenId = objupcomming.TokenId;
            product.JourneyType = objupcomming.JourneyType;
            product.PreferredAirlines = objupcomming.PreferredAirlines;
            dynamic segmentval = new JObject();
            segmentval.Origin = objupcomming.Origin;
            segmentval.Destination = objupcomming.Destination;
            segmentval.FlightCabinClass = objupcomming.FlightCabinClass;
            segmentval.PreferredDepartureTime = objupcomming.PreferredDepartureTime;
            product.Segments = new JArray(segmentval);
            product.Sources = null;
            string SearchparamValue = JsonConvert.SerializeObject(product);
            var res = GetResponse(SearchparamValue, url);
            if (res.Response.ResponseStatus == "1")
            {
                return res;
            }
            else
            {
                return res;
            }
        }

        public static dynamic Fare_Rule(string GetTokenId,string TraceId,string ResultIndex)
        {
            string url = root + "/BookingEngineService_Air/AirService.svc/rest/FareRule";
            var param = new Dictionary<string, string> {
                            { "EndUserIp",EndUserIp},
                            { "TokenId" , GetTokenId },
                            { "TraceId" , TraceId },
                            {"ResultIndex",ResultIndex }
                        };
            string valueparameter = JsonConvert.SerializeObject(param);
            var res = GetResponse(valueparameter, url);
            if (res.Response.ResponseStatus == "1")
            {
                // string token = res.TokenId;
                return res;
            }
            else
            {
                //return InstantPayError.GetError(res.statuscode.Value);
                return res;
            }
        }

        public static dynamic Fare_Quote(string GetTokenId, string TraceId, string ResultIndex)
        {
            string url = root + "/BookingEngineService_Air/AirService.svc/rest/FareQuote";
            var param = new Dictionary<string, string> {
                            { "EndUserIp",EndUserIp},
                            { "TokenId" , GetTokenId },
                            { "TraceId" , TraceId },
                            {"ResultIndex",ResultIndex }
                        };
            string valueparameter = JsonConvert.SerializeObject(param);
            var res = GetResponse(valueparameter, url);
            if (res.Response.ResponseStatus == "1")
            {
                return res;
            }
            else
            {
                return res;
            }
        }

        public static dynamic SSR(string GetTokenId, string TraceId, string ResultIndex)
        {
            string url = root + "/BookingEngineService_Air/AirService.svc/rest/SSR";
            var param = new Dictionary<string, string> {
                            { "EndUserIp",EndUserIp},
                            { "TokenId" , GetTokenId },
                            { "TraceId" , TraceId },
                            {"ResultIndex",ResultIndex }
                        };
            string valueparameter = JsonConvert.SerializeObject(param);
            var res = GetResponse(valueparameter, url);
            if (res.Response.ResponseStatus == "1")
            {
                return res;
            }
            else
            {
                return res;
            }
        }

        //Booking Api

        //public static dynamic Flight_Booking(string TokenidVal,string TraceId,string ResultIndex,string Title,string FirstName,string LastName,string PaxType,string DateOfBirth,string Gender,string PassportNo,string PassportExpiry,string AddressLine1,string AddressLine2,string Currency,string BaseFare,string Tax,string YQTax,string AdditionalTxnFeePub,string AdditionalTxnFeeOfrd,string OtherCharges,string Discount,string PublishedFare,string OfferedFare,string TdsOnCommission,string TdsOnPLB,string TdsOnIncentive,string ServiceFee,string City,string CountryCode,string CountryName,string Nationality,string ContactNo,string Email,string IsLeadPax,string FFAirlineCode,string FFNumber,string GSTCompanyAddress,string GSTCompanyContactNumber,string GSTCompanyName,string GSTNumber,string GSTCompanyEmail)
        public static dynamic Flight_Booking(TekTravelFlightAPIModel objbooking)
        {
            string url = root + "/BookingEngineService_Air/AirService.svc/rest/Book";
            dynamic product = new JObject();            
            product.ResultIndex = objbooking.ResultIndex;

            dynamic Passengers = new JObject();
            Passengers.Title = objbooking.Title;
            Passengers.FirstName = objbooking.FirstName;
            Passengers.LastName = objbooking.LastName;
            Passengers.PaxType = objbooking.PaxType;
            Passengers.DateOfBirth = objbooking.DateOfBirth;
            Passengers.Gender = objbooking.Gender;
            Passengers.PassportNo = objbooking.PassportNo;
            Passengers.PassportExpiry = objbooking.PassportExpiry;
            Passengers.AddressLine1 = objbooking.AddressLine1;
            Passengers.AddressLine2 = objbooking.AddressLine2;

            dynamic Fare = new JObject();
            Fare.Currency = objbooking.Currency;
            Fare.BaseFare = objbooking.BaseFare;
            Fare.Tax = objbooking.Tax;
            Fare.YQTax = objbooking.YQTax;
            Fare.AdditionalTxnFeePub = objbooking.AdditionalTxnFeePub;
            Fare.AdditionalTxnFeeOfrd = objbooking.AdditionalTxnFeeOfrd;
            Fare.OtherCharges = objbooking.OtherCharges;
            Fare.Discount = objbooking.Discount;
            Fare.PublishedFare = objbooking.PublishedFare;
            Fare.OfferedFare = objbooking.OfferedFare;
            Fare.TdsOnCommission = objbooking.TdsOnCommission;
            Fare.TdsOnPLB = objbooking.TdsOnPLB;
            Fare.TdsOnIncentive = objbooking.TdsOnIncentive;
            Fare.ServiceFee = objbooking.ServiceFee;
            Passengers.Fare = new JObject(Fare);

            Passengers.City = objbooking.City;
            Passengers.CountryCode = objbooking.CountryCode;
            Passengers.CountryName = objbooking.CountryName;
            Passengers.Nationality = objbooking.Nationality;
            Passengers.ContactNo = objbooking.ContactNo;
            Passengers.Email = objbooking.Email;
            Passengers.IsLeadPax = objbooking.IsLeadPax;
            Passengers.FFAirlineCode = objbooking.FFAirlineCode;
            Passengers.FFNumber = objbooking.FFNumber;
            Passengers.GSTCompanyAddress = objbooking.GSTCompanyAddress;
            Passengers.GSTCompanyContactNumber = objbooking.GSTCompanyContactNumber;
            Passengers.GSTCompanyName = objbooking.GSTCompanyName;
            Passengers.GSTNumber = objbooking.GSTNumber;
            Passengers.GSTCompanyEmail = objbooking.GSTCompanyEmail;
            product.Passengers = new JArray(Passengers);
            product.EndUserIp = EndUserIp;
            product.TokenId = objbooking.TokenId;
            product.TraceId = objbooking.TraceId;
            
            string SearchparamValue = JsonConvert.SerializeObject(product);            
            var res = GetResponse(SearchparamValue, url);
            if (res.Response.ResponseStatus == "1")
            {
                // string token = res.TokenId;
                return res;
            }
            else
            {
                //return InstantPayError.GetError(res.statuscode.Value);
                return res;
            }
        }

        //public static dynamic Get_Ticket(string PreferredCurrency,string ResultIndex,string AgentReferenceNo,string Title,string FirstName,string LastName,string PaxType,string DateOfBirth,string Gender,string PassportNo,string PassportExpiry,string AddressLine1,string AddressLine2,string BaseFare,string Tax,string YQTax,string AdditionalTxnFeePub,string AdditionalTxnFeeOfrd,string OtherCharges,string City,string CountryCode,string CountryName,string ContactNo,string Email,string IsLeadPax,string FFAirlineCode,string FFNumber,string WayType,string Code,string Description,string Weight,string BaseCurrencyPrice,string BaseCurrency,string Currency,string Price,string Origin,string Destination,string mealWayType, string mealCode, string mealDescription, string mealAirlineDescription,string mealQuantity,string MealBaseCurrency,string mealBaseCurrencyPrice,string mealCurrency,string mealPrice,string mealOrigin,string mealDestination, string SeatDynamic,string GSTCompanyAddress,string GSTCompanyContactNumber,string GSTCompanyName,string GSTNumber,string GSTCompanyEmail,string TokenId,string TraceId)
        public static dynamic Get_Ticket(GetTicketMoel objval)
        {
            string url = root + "/BookingEngineService_Air/AirService.svc/rest/Ticket";

            dynamic ticket = new JObject();
            ticket.PreferredCurrency = objval.PreferredCurrency;
            ticket.ResultIndex = objval.ResultIndex; 
            ticket.AgentReferenceNo = objval.AgentReferenceNo;


            dynamic Passengers = new JObject();
            Passengers.Title = objval.Title;
            Passengers.FirstName = objval.FirstName;
            Passengers.LastName = objval.LastName;
            Passengers.PaxType = objval.PaxType;
            Passengers.DateOfBirth = objval.DateOfBirth;
            Passengers.Gender = objval.Gender;
            Passengers.PassportNo = objval.PassportNo;
            Passengers.PassportExpiry = objval.PassportExpiry;
            Passengers.AddressLine1 = objval.AddressLine1;
            Passengers.AddressLine2 = objval.AddressLine2;

            dynamic Fare = new JObject();
            Fare.BaseFare = objval.BaseFare;
            Fare.Tax = objval.Tax;
            Fare.YQTax = objval.YQTax;
            Fare.AdditionalTxnFeePub = objval.AdditionalTxnFeePub;
            Fare.AdditionalTxnFeeOfrd = objval.AdditionalTxnFeeOfrd;
            Fare.OtherCharges = objval.OtherCharges;

            Passengers.Fare = new JObject(Fare);
            Passengers.City= objval.City;
            Passengers.CountryCode = objval.CountryCode;
            Passengers.CountryName = objval.CountryName;
            Passengers.ContactNo = objval.ContactNo;
            Passengers.Email = objval.Email;
            Passengers.IsLeadPax = objval.IsLeadPax;
            Passengers.FFAirlineCode = objval.FFAirlineCode;
            Passengers.FFNumber = objval.FFNumber;

            dynamic Baggage = new JObject();
            Baggage.WayType = objval.BeggWayType;
            Baggage.Code = objval.BeggCode;
            Baggage.Description = objval.BeggDescription;
            Baggage.Weight = objval.BeggWeight;
            Baggage.BaseCurrencyPrice = objval.BeggBaseCurrencyPrice;
            Baggage.BaseCurrency = objval.BeggBaseCurrency;
            Baggage.Currency = objval.BeggCurrency;
            Baggage.Price = objval.BeggPrice;
            Baggage.Origin = objval.BeggOrigin;
            Baggage.Destination = objval.BeggDestination;
            Passengers.Baggage = new JArray(Baggage);

            dynamic MealDynamic = new JObject();
            MealDynamic.WayType= objval.MealWayType;
            MealDynamic.Code = objval.MealCode;
            MealDynamic.Description = objval.MealDescription;
            MealDynamic.AirlineDescription = objval.MealAirlineDescription;
            MealDynamic.Quantity = objval.MealQuantity;
            MealDynamic.BaseCurrency = objval.MealBaseCurrency;
            MealDynamic.BaseCurrencyPrice = objval.MealBaseCurrencyPrice;
            MealDynamic.Currency = objval.MealCurrency;
            MealDynamic.Price = objval.MealPrice;
            MealDynamic.Origin = objval.MealOrigin;
            MealDynamic.Destination = objval.MealDestination;
            Passengers.MealDynamic = new JArray(MealDynamic);
            Passengers.SeatDynamic = new JArray();
            Passengers.GSTCompanyAddress = objval.GSTCompanyAddress;
            Passengers.GSTCompanyContactNumber = objval.GSTCompanyContactNumber;
            Passengers.GSTCompanyName = objval.GSTCompanyName;
            Passengers.GSTNumber = objval.GSTNumber;
            Passengers.GSTCompanyEmail = objval.GSTCompanyEmail;
            ticket.Passengers = new JArray(Passengers) ;
            ticket.EndUserIp = EndUserIp;
            ticket.TokenId = objval.TokenId;
            ticket.TraceId = objval.TraceId;
            string valueparameter = JsonConvert.SerializeObject(ticket);
            var res = GetResponse(valueparameter, url);
            if (res.Response.ResponseStatus == "1")
            {
                // string token = res.TokenId;
                return res;
            }
            else
            {
                //return InstantPayError.GetError(res.statuscode.Value);
                return res;
            }
        }


        public static dynamic GetBookingDetails(string TokenId, string PNR, string BookingId)
        {
            string url = root + "/BookingEngineService_Air/AirService.svc/rest/GetBookingDetails";
            var param = new Dictionary<string, string> {
                            { "EndUserIp",EndUserIp},
                            { "TokenId" , TokenId },
                            { "PNR" , PNR },
                            {"BookingId",BookingId }
                        };
            string valueparameter = JsonConvert.SerializeObject(param);
            var res = GetResponse(valueparameter, url);
            if (res.Response.ResponseStatus == "1")
            {
                // string token = res.TokenId;
                return res;
            }
            else
            {
                //return InstantPayError.GetError(res.statuscode.Value);
                return res;
            }
        }

        public static dynamic ReleasePNRRequest(string TokenId,string BookingId,string Source)
        {
            string url = root + "/BookingEngineService_Air/AirService.svc/rest/ReleasePNRRequest";
            var param = new Dictionary<string, string>{
                { "EndUserIp",EndUserIp},
                {"TokenId",TokenId },
                {"BookingId",BookingId },
                {"Source",Source }
            };
            var RequestParameter = JsonConvert.SerializeObject(param);
            var res = GetResponse(RequestParameter,url);
            if (res.Response.ResponseStatus == "1")
            {
                return res;
            }
            else
            {
                return res;
            }
        }


        public static dynamic SendChangeRequest(SendCancelRequest objChangeRequest)
        {
            string url = root + "/BookingEngineService_Air/AirService.svc/rest/SendChangeRequest";

            dynamic canclestatus = new JObject();
            canclestatus.BookingId = objChangeRequest.BookingId;
            canclestatus.RequestType = objChangeRequest.RequestType;
            canclestatus.CancellationType = objChangeRequest.CancellationType;
            dynamic Sectors = new JObject();
            Sectors.Origin = objChangeRequest.Origin;
            Sectors.Destination = objChangeRequest.Destination;
            canclestatus.Sectors = new JArray(Sectors);
            canclestatus.TicketId = null;
            canclestatus.Remarks = objChangeRequest.Remarks;
            canclestatus.EndUserIp = EndUserIp;
            canclestatus.TokenId = objChangeRequest.TokenId;

            string valueparameter = JsonConvert.SerializeObject(canclestatus);
            var res = GetResponse(valueparameter, url);
            if (res.Response.ResponseStatus == "1")
            {
                // string token = res.TokenId;
                return res;
            }
            else
            {
                //return InstantPayError.GetError(res.statuscode.Value);
                return res;
            }
        }

        public static dynamic GetChangeRequestStatus(string ChangeRequestId, string TokenId)
        {
            string url = root + "/BookingEngineService_Air/AirService.svc/rest/GetChangeRequestStatus";
            var param = new Dictionary<string, string>{
                { "EndUserIp",EndUserIp},
                {"ChangeRequestId",ChangeRequestId },
                {"TokenId",TokenId }
            };
            var RequestParameter = JsonConvert.SerializeObject(param);
            var res = GetResponse(RequestParameter, url);
            if (res.Response.ResponseStatus == "1")
            {
                return res;
            }
            else
            {
                return res;
            }
        }

        public static dynamic GetResponse(string requestData, string url)
        {
            string responseXML = string.Empty;
            dynamic responsesult = null;
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(requestData);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers.Add("Accept-Encoding", "gzip");
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(data, 0, data.Length);
                dataStream.Close();
                WebResponse webResponse = request.GetResponse();
                var rsp = webResponse.GetResponseStream();
                if (rsp == null)
                {
                    //throw exception
                }
                using (StreamReader readStream = new StreamReader(new GZipStream(rsp, CompressionMode.Decompress)))
                {
                    responsesult = JsonConvert.DeserializeObject(readStream.ReadToEnd());
                    //responseXML = JsonConvert.DeserializeXmlNode(readStream.ReadToEnd()).InnerXml;
                }
                return responsesult;
            }
            catch (WebException webEx)
            {
                //get the response stream
                WebResponse response = webEx.Response;
                Stream stream = response.GetResponseStream();
                String responseMessage = new StreamReader(stream).ReadToEnd();
                return responseMessage;
            }
        }

        
        //public class Flightsearchparameter
        //{
        //    public string EndUserIp { get; set; }
        //    public string TokenId { get; set; }
        //    public string AdultCount { get; set; }
        //    public string ChildCount { get; set; }
        //    public string InfantCount { get; set; }
        //    public string DirectFlight { get; set; }
        //    public string OneStopFlight { get; set; }
        //    public string JourneyType { get; set; }
        //    public string PreferredAirlines { get; set; }
        //    public Segmentslist Segments;
        //    public string[] Sources { get; set; }
        //}
        //public class Segmentslist
        //{
        //    public string Origin { get; set; }
        //    public string Destination { get; set; }
        //    public string FlightCabinClass { get; set; }
        //    public string PreferredDepartureTime { get; set; }
        //    public string PreferredArrivalTime { get; set; }
        //}


        public partial class AdvanceSearchModel
        {

            public string AdultCount { get; set; }            
            public string ChildCount { get; set; }
            public string InfantCount { get; set; }            
            public string EndUserIp { get; set; }            
            public string TokenId { get; set; }            
            public string TraceId { get; set; }            
            public List<AirSearchResult> AirSearchResult { get; set; }
        }

        public partial class AirSearchResult
        {
            
            public string ResultIndex { get; set; }            
            public int Source { get; set; }            
            public string IsLCC { get; set; }            
            public string IsRefundable { get; set; }            
            public string AirlineRemark { get; set; }            
            //public Segment[][] Segments1 { get; set; }
            public List<List<Segment>> Segments { get; set; }
        }

        public partial class Segment
        {
            public int TripIndicator { get; set; }
            public int SegmentIndicator { get; set; }
            public Airline Airline { get; set; }
        }

        public partial class Airline
        {
            public string AirlineCode { get; set; }
            
            public string AirlineName { get; set; }
            public string FlightNumber { get; set; }
            public string FareClass { get; set; }
            public string OperatingCarrier { get; set; }
        }


    }
}