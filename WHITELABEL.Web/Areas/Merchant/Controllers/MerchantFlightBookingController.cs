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
using log4net;
using static WHITELABEL.Web.Helper.Tek_TravelAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WHITELABEL.Web.Areas.Merchant.Controllers
{
    [Authorize]
    public class MerchantFlightBookingController : MerchantBaseController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        public void initpage()
        {
            try
            {
                ViewBag.ControllerName = "Merchant Dashboard";
                SystemClass sclass = new SystemClass();
                string userID = sclass.GetLoggedUser();
                long userid = long.Parse(userID);
                var dbmain = new DBContext();
                if (userID != null && userID != "")
                {
                    TBL_MASTER_MEMBER currUser = dbmain.TBL_MASTER_MEMBER.SingleOrDefault(c => c.MEM_ID == userid && c.MEMBER_ROLE == 5 && c.ACTIVE_MEMBER == true);
                    if (currUser != null)
                    {
                        Session["MerchantUserId"] = currUser.MEM_ID;
                        // Session["UserName"] = currUser.UserName;
                    }
                    else
                    {
                        Response.Redirect(Url.Action("Index", "MerchantLogin", new { area = "Merchant" }));
                        return;
                    }
                }
                if (Session["MerchantUserId"] == null)
                {
                    //Response.Redirect(Url.Action("Index", "Login", new { area = "" }));
                    Response.Redirect(Url.Action("Index", "MerchantLogin", new { area = "Merchant" }));
                    return;
                }
                bool Islogin = false;
                if (Session["MerchantUserId"] != null)
                {
                    Islogin = true;
                    ViewBag.CurrentUserId = CurrentMerchant.MEM_ID;
                }
                ViewBag.Islogin = Islogin;
            }
            catch (Exception e)
            {
                //ViewBag.UserName = CurrentUser.UserId;
                Console.WriteLine(e.InnerException);
                return;
            }
        }
        public static string GetToken()
        {
            var db = new DBContext();
            //var getToken = db.TBL_API_TOKEN.FirstOrDefault();
            //string retturndate = string.Empty;
            //if (getToken != null)
            //{
            //    //DateTime tokendate = Convert.ToDateTime(getToken.INSERTEDDATE.ToString("yyyy-MM-dd"));
            //    //DateTime TodayDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
            //    //if (tokendate == TodayDate)
            //    //{
            //    //    retturndate = getToken.TOKEN;
            //    //}
            //    //else
            //    //{
            //    //    var genToken = Tek_TravelAPI.FlightAPIGenerateToken("ENCORE", "ENCORE@1234");
            //    //    var objvaltoken = db.TBL_API_TOKEN.FirstOrDefault();
            //    //    if (objvaltoken != null)
            //    //    {
            //    //        objvaltoken.TOKEN = genToken.TokenId;
            //    //        objvaltoken.INSERTEDDATE = DateTime.Now;
            //    //        db.Entry(objvaltoken).State = System.Data.Entity.EntityState.Modified;
            //    //        db.SaveChanges();
            //    //    }
            //    //    else
            //    //    {
            //    //        TBL_API_TOKEN objval = new TBL_API_TOKEN()
            //    //        {
            //    //            TOKEN = genToken.TokenId,
            //    //            APINAME = "TEKTravel",
            //    //            TOKENTYPE = "Authenticate",
            //    //            INSERTEDDATE = DateTime.Now,
            //    //            STATUS = 1
            //    //        };
            //    //        db.TBL_API_TOKEN.Add(objval);
            //    //        db.SaveChanges();
            //    //    }
            //    //    retturndate = genToken.TokenId;
            //    //}
            //}
            var genToken = Tek_TravelAPI.FlightAPIGenerateToken("ENCORE", "ENCORE@1234");
            return genToken.TokenId;
            //return retturndate;
        }
        // GET: Merchant/MerchantFlightBooking
        public ActionResult Index()
        {
            if (Session["MerchantUserId"] != null)
            {
                //Session["MerchantDMRId"] = null;
                //var param = new Dictionary<string, string> {
                //            { "ClientId","ApiIntegrationNew"},
                //            { "UserName" , "ENCORE" },
                //            { "Password" , "ENCORE@1234" },
                //            {"EndUserIp","14.195.224.166" }
                //        };   //14.140.162.238:80
                //string valueparameter = JsonConvert.SerializeObject(param);
                //string url = "http://api.tektravels.com/SharedServices/SharedData.svc/rest/Authenticate";
                //var flightstatus = Tek_TravelAPI.GetResponse(valueparameter, url);
                var db = new DBContext();

                // var genToken = Tek_TravelAPI.FlightAPIGenerateToken("ENCORE", "ENCORE@1234");                
                //var objvaltoken = db.TBL_API_TOKEN.FirstOrDefault();
                //if (objvaltoken != null)
                //{
                //    objvaltoken.TOKEN = genToken.TokenId;
                //    objvaltoken.INSERTEDDATE = DateTime.Now;
                //    db.Entry(objvaltoken).State = System.Data.Entity.EntityState.Modified;
                //    db.SaveChanges();
                //}
                //else
                //{
                //    TBL_API_TOKEN objval = new TBL_API_TOKEN()
                //    {
                //        TOKEN = genToken.TokenId,
                //        APINAME = "TEKTravel",
                //        TOKENTYPE = "Authenticate",
                //        INSERTEDDATE = DateTime.Now,
                //        STATUS = 1
                //    };
                //    db.TBL_API_TOKEN.Add(objval);
                //    db.SaveChanges();
                //}
                #region Generate Token
                var GetTokenValue = GetToken();
                //string tokenval = GetToken();
                //var Logout = Tek_TravelAPI.FlightAPILogout();
                #endregion

                #region Get AgentBalance
                //var GetAgentBalance = Tek_TravelAPI.GetAgentBalance();
                #endregion

                //#region Release PNR Request
                //var ReleaseRNRrequest1 = Tek_TravelAPI.ReleasePNRRequest(GetTokenValue, "1401539", "25");
                //#endregion


                #region All Flight Booking API

              

                //dynamic product = new JObject();
                //product.EndUserIp = "14.195.235.244";
                //product.TokenId = GetTokenValue;
                //product.AdultCount = "1";
                //product.ChildCount = "0";
                //product.InfantCount = "0";

                //product.DirectFlight = true;
                //product.OneStopFlight = false;
                //product.JourneyType = "1";
                //product.PreferredAirlines = null;
                //dynamic segmentval = new JObject();
                //segmentval.Origin = "CCU";
                //segmentval.Destination = "BOM";
                //segmentval.FlightCabinClass = "1";
                //segmentval.PreferredDepartureTime = "2018-09-27T00: 00: 00";
                //segmentval.PreferredArrivalTime =null;
                //product.Segments = new JArray(segmentval);
                //product.Sources = new JArray("6E");

                //#region Search Flight
                //DateTime fromdate = DateTime.Now.AddMonths(2);
                //string Date_from = fromdate.ToString("yyyy-MM-dd");
                //DateTime todate = DateTime.Now.AddMonths(2); 
                //string Date_to=todate.AddDays(1).ToString("yyyy-MM-dd");
                //string PreferredAirlines = null;
                //string datetovalue = null;

                //// Search Flight
                //FlightSearch objflightsearch = new FlightSearch();
                //objflightsearch.TokenId = GetTokenValue;
                //objflightsearch.AdultCount = "1";
                //objflightsearch.ChildCount = "0";
                //objflightsearch.InfantCount = "0";
                //objflightsearch.DirectFlight = "false";
                //objflightsearch.OneStopFlight = "false";
                //objflightsearch.JourneyType = "1";
                //objflightsearch.PreferredAirlines = null;
                //objflightsearch.Origin = "CCU";
                //objflightsearch.Destination = "BOM";
                //objflightsearch.FlightCabinClass = "1";
                //objflightsearch.PreferredDepartureTime = Date_from;
                //objflightsearch.PreferredArrivalTime = datetovalue;
                //objflightsearch.Sources = "6E";

                //var searchflight = Tek_TravelAPI.SerachFlight(objflightsearch);
                ////var searchflight = Tek_TravelAPI.SerachFlight(GetTokenValue,"1","0","0","true", "false", "1", PreferredAirlines, "CCU","BOM","1", Date_from, datetovalue, "6E");
                //string TraceId = searchflight.Response.TraceId;
                ////search flight by id to get particular details
                //#endregion

                //#region Advance Flight search                
                ////Advance Search Flight
                //AdvanceSearch objAdvance = new AdvanceSearch();
                //objAdvance.AdultCount = "1";
                //objAdvance.ChildCount = "0";
                //objAdvance.InfantCount = "0";
                //objAdvance.TokenId = GetTokenValue;
                //objAdvance.TraceId = TraceId;
                //objAdvance.ResultIndex = "OB6";
                //objAdvance.Source = 4;
                //objAdvance.IsLCC = "false";
                //objAdvance.IsRefundable = "true";
                //objAdvance.AirlineRemark = "this is a test from aditya";
                //objAdvance.TripIndicator = 1;
                //objAdvance.SegmentIndicator = 1;
                //objAdvance.AirlineCode = "9W";
                //objAdvance.AirlineName = "Jet Airways";
                //objAdvance.FlightNumber = "628";
                //objAdvance.FareClass = "B";
                //objAdvance.OperatingCarrier = "9W";
                //var AdvanceSearchFlight = Tek_TravelAPI.AdvanceSearchFlightSearch(objAdvance);
                ////var AdvanceSearchFlight = Tek_TravelAPI.AdvanceSearchFlightSearch(GetTokenValue,"1", "0", "0", "934e484d-2cf1-4914-856c-976d78c806e7", TraceId, "OB4", "4","false","true","","1","1", "9W", "Jet Airways", "616", "C","");
                //#endregion

                ////#region Get calender Fare and Update calenderfareday

                ////string preferredairline = null;
                ////DateTime Travelate = DateTime.Now.AddMonths(2);
                ////string TravelDatefrom = fromdate.ToString("yyyy-MM-dd");
                //////get calender fare

                ////GetCalanderModel objgetcalender = new GetCalanderModel();
                ////objgetcalender.JourneyType = "1";
                ////objgetcalender.TokenId = GetTokenValue;
                ////objgetcalender.PreferredAirlines = null; ;
                ////objgetcalender.Origin = "CCU";
                ////objgetcalender.Destination = "BOM";
                ////objgetcalender.FlightCabinClass = "1";
                ////objgetcalender.PreferredDepartureTime = TravelDatefrom;
                ////objgetcalender.Sources = null;
                ////var getCalenderFare = Tek_TravelAPI.GetCalenderFare(objgetcalender);
                //////var getCalenderFare = Tek_TravelAPI.GetCalenderFare(GetTokenValue, "1",preferredairline,"CCU","BOM","1", TravelDatefrom);

                //////update calender fare
                ////UpdateCalenderFareofDayModel objupcomming = new UpdateCalenderFareofDayModel();
                ////objupcomming.JourneyType = "1";
                ////objupcomming.TokenId =GetTokenValue;
                ////objupcomming.PreferredAirlines = null;
                ////objupcomming.Origin = "CCU";
                ////objupcomming.Destination = "BOM";
                ////objupcomming.FlightCabinClass = "1";
                ////objupcomming.PreferredDepartureTime = TravelDatefrom;
                ////objupcomming.Sources = null;
                ////var UpdateCalenderFareday = Tek_TravelAPI.Update_Calender_FateOfDay(objupcomming);
                //////var UpdateCalenderFareday = Tek_TravelAPI.Update_Calender_FateOfDay(GetTokenValue, "1", preferredairline, "CCU", "BOM", "1", TravelDatefrom);
                ////#endregion

                ////#region Get FareRule and fare Quotes and SSR
                //////get fareRule
                //var GetFareRule = Tek_TravelAPI.Fare_Rule(GetTokenValue, TraceId, "OB6");
                //var GetFareQuote = Tek_TravelAPI.Fare_Quote(GetTokenValue, TraceId, "OB6");
                //var GetSSR = Tek_TravelAPI.SSR(GetTokenValue, TraceId, "OB6");
                ////#endregion

                //#region Booking API
                ////filght booking class
                //string FFairlinecode = null;
                //TekTravelFlightAPIModel objbooking = new TekTravelFlightAPIModel();
                //objbooking.ResultIndex = "OB6";
                //objbooking.Title = "Mr";
                //objbooking.FirstName = "Rahul";
                //objbooking.LastName = "Sharma";
                //objbooking.PaxType = "1";
                //objbooking.DateOfBirth = "1987-07-10";
                //objbooking.Gender = "1";
                //objbooking.PassportNo = "";
                //objbooking.PassportExpiry = "";
                //objbooking.AddressLine1 = "Kolkata";
                //objbooking.AddressLine2 = "";
                //objbooking.Currency = "INR";
                //objbooking.BaseFare = "2100";
                //objbooking.Tax = "2288";
                //objbooking.YQTax = "1200";
                //objbooking.AdditionalTxnFeePub = "0";
                //objbooking.AdditionalTxnFeeOfrd = "0";
                //objbooking.OtherCharges = "0";
                //objbooking.Discount = "0";
                //objbooking.PublishedFare = "4527.24";
                //objbooking.OfferedFare = "4509.39";
                //objbooking.TdsOnCommission = "5.36";
                //objbooking.TdsOnPLB = "0";
                //objbooking.TdsOnIncentive = "0";
                //objbooking.ServiceFee = "0";
                //objbooking.City = "Kolkata";
                //objbooking.CountryCode = "IN";
                //objbooking.CountryName = "India";
                //objbooking.Nationality = "IN";
                //objbooking.ContactNo = "9879879877";
                //objbooking.Email = "harsh@tbtq.in";
                //objbooking.IsLeadPax = "true";
                //objbooking.FFAirlineCode = FFairlinecode;
                //objbooking.GSTCompanyAddress = "A-fhgjkhsjkfd";
                //objbooking.GSTCompanyContactNumber = "98881063278748979";
                //objbooking.GSTCompanyName = "nikhil";
                //objbooking.GSTNumber = "19AAECI7730G1ZG";
                //objbooking.GSTCompanyEmail = "nikhil123@gmail.com";
                //objbooking.TokenId = GetTokenValue;
                //objbooking.TraceId = TraceId;
                //var flightbooking = Tek_TravelAPI.Flight_Booking(objbooking);
                //#endregion
                ////var flightbooking = Tek_TravelAPI.Flight_Booking(GetTokenValue, TraceId, "OB1","Mr","Rahul","Sharma","1","1987-07-10","1","J4851326","2021-01-23","Kolkata","","INR", "2300", "2298", "1200","0.0","0.0", "139.24","0", "4737.24", "4717.69", "5.86","0","0","0","Kolkata","IN","India","IN","9903254540","rahul@test.com","true", FFairlinecode,"", "A-fhgjkhsjkfd", "98881063278748979", "nikhil", "700932234532413", "nikhil123@gmail.com");

                ////Get Ticket API
                //#region Ticket API                
                //GetTicketMoel ObjTicket = new GetTicketMoel();
                //ObjTicket.PreferredCurrency = null;
                //ObjTicket.ResultIndex = "OB6";
                //ObjTicket.AgentReferenceNo = "46483";
                //ObjTicket.Title = "Mr";
                //ObjTicket.FirstName = "Rahul";
                //ObjTicket.LastName = "Sharma";
                //ObjTicket.PaxType = "1";
                //ObjTicket.DateOfBirth = "1987-07-10";
                //ObjTicket.Gender = "1";
                //ObjTicket.PassportNo = "J4851326";
                //ObjTicket.PassportExpiry = "2021-01-23";
                //ObjTicket.AddressLine1 = "123 Test";
                //ObjTicket.AddressLine2 = "";
                //ObjTicket.BaseFare = "2100";
                //ObjTicket.Tax = "2288";
                //ObjTicket.YQTax = "1200";
                //ObjTicket.AdditionalTxnFeePub = "0";
                //ObjTicket.AdditionalTxnFeeOfrd = "0";
                //ObjTicket.OtherCharges = "139.24";
                //ObjTicket.City = "Kolkata";
                //ObjTicket.CountryCode = "IN";
                //ObjTicket.CountryName = "India";
                //ObjTicket.ContactNo = "9879879877";
                //ObjTicket.Email = "harsh@tbtq.in";
                //ObjTicket.IsLeadPax = "false";
                //ObjTicket.FFAirlineCode = "";
                //ObjTicket.FFNumber = "";
                //ObjTicket.BeggWayType = "2";
                //ObjTicket.BeggCode = "";
                //ObjTicket.BeggDescription = "2";
                //ObjTicket.BeggWeight = "0";
                //ObjTicket.BeggBaseCurrencyPrice = "0";
                //ObjTicket.BeggBaseCurrency = "INR";
                //ObjTicket.BeggCurrency = "INR";
                //ObjTicket.BeggPrice = "0";
                //ObjTicket.BeggOrigin = "CCU";
                //ObjTicket.BeggDestination = "BOM";
                //ObjTicket.MealWayType = "2";
                //ObjTicket.MealCode = "";
                //ObjTicket.MealDescription = "2";
                //ObjTicket.MealAirlineDescription = "CHICKEN KATHI ROLL";
                //ObjTicket.MealQuantity = "1";
                //ObjTicket.MealBaseCurrency = "INR";
                //ObjTicket.MealBaseCurrencyPrice= "125";
                //ObjTicket.MealCurrency = "INR";
                //ObjTicket.MealPrice = "125";
                //ObjTicket.MealOrigin = "CCU";
                //ObjTicket.MealDestination = "BOM";
                ////ObjTicket.SeatDynamic= null;
                //ObjTicket.GSTCompanyAddress = "Delhi";
                //ObjTicket.GSTCompanyContactNumber = "9874563210";
                //ObjTicket.GSTCompanyName = "Sonam";
                //ObjTicket.GSTNumber = "19AAECI7730G1ZG";
                //ObjTicket.GSTCompanyEmail = "sonam@travelboutiqueonline.com";
                //ObjTicket.TokenId = GetTokenValue;
                //ObjTicket.TraceId = TraceId;
                //var ticketobj = Tek_TravelAPI.Get_Ticket(ObjTicket);
                //#endregion

                //#region Get BookingStatus
                //// get Booking Details
                //var getBookingDetails = Tek_TravelAPI.GetBookingDetails(GetTokenValue, "J3IJXY", "1401539");
                //#endregion

                //// Release PNR Request
                //#region Release PNR Request
                //var ReleaseRNRrequest = Tek_TravelAPI.ReleasePNRRequest(GetTokenValue, "1401539", "25");
                //SendCancelRequest objcan = new SendCancelRequest();
                //objcan.BookingId = "";
                //objcan.RequestType = 1;
                //objcan.CancellationType = 3;
                //objcan.Origin = "CCU";
                //objcan.Destination = "BOM";
                //var ticketval = new List<string>();
                //ticketval.Add("12345");
                //ticketval.Add("1245");
                //objcan.TicketId = ticketval;
                //objcan.Remarks = "";
                //objcan.TokenId = GetTokenValue;

                //var sendCencelREquest = Tek_TravelAPI.SendChangeRequest(objcan);

                //var GetChangedStatus = Tek_TravelAPI.GetChangeRequestStatus("199350", GetTokenValue);
                //#endregion


                #endregion

                Session.Remove("MerchantDMRId");
                return View();
            }
            else
            {
                return RedirectToAction("Index", "MerchantLogin", new { area = "Merchant" });
            }
        }
        public ActionResult FlightSearch()
        {
            if (Session["MerchantUserId"] != null)
            {
                Session["MerchantDMRId"] = null;
                Session.Remove("MerchantDMRId");
                return View();
            }
            else
            {
                return RedirectToAction("Index", "MerchantLogin", new { area = "Merchant" });
            }
        }
        public JsonResult getjsonval()
        {
            return Json(JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllAirportName()
        {
            try
            {
                var db = new DBContext();
                //var airportlist = db.TBL_AIRPORT_DETAILS.Where(x => x.CITYNAME.Contains(pretext)).ToList();
                var airportlist = db.TBL_AIRPORT_DETAILS.Select(z => new
                {
                    ID = z.ID,
                    CITYCODE = z.CITYCODE,
                    CITYNAME = z.CITYNAME + " " + z.CITYCODE,
                    COUNTRYCODE = z.COUNTRYCODE,
                    AIRPORT_TYPE = z.AIRPORT_TYPE,
                    ISACTIVE = z.ISACTIVE
                }).ToList();
                return new JsonResult { Data = airportlist, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception ex)
            {
                throw ex;
            }    
        }

        [HttpPost]
        public JsonResult SerachFlight(FlightSearchParameter objserch)
        {
            try
            {
                string datefrm = null;
                string dateTo = null;

                if (objserch.Tripmode == "1")
                {
                    DateTime datefrm11 = Convert.ToDateTime(objserch.FromDate);
                    datefrm = datefrm11.ToString("yyyy-MM-dd");
                    dateTo = null;
                }
                else if (objserch.Tripmode == "2")
                {
                    DateTime datefrm11 = Convert.ToDateTime(objserch.FromDate);
                    datefrm = datefrm11.ToString("yyyy-MM-dd");

                    DateTime date_to = Convert.ToDateTime(objserch.ToDate);

                    dateTo = date_to.ToString("yyyy-MM-dd");
                }
                var GetTokenValue = GetToken();

                FlightSearch objflightsearch = new FlightSearch();
                objflightsearch.TokenId = GetTokenValue;
                objflightsearch.AdultCount = objserch.Adult;
                objflightsearch.ChildCount =objserch.Child;
                objflightsearch.InfantCount = objserch.Infant;
                objflightsearch.DirectFlight = "false";
                objflightsearch.OneStopFlight = "false";
                objflightsearch.JourneyType = objserch.Tripmode;
                objflightsearch.PreferredAirlines = null;
                objflightsearch.Origin = objserch.FromCityCode;
                objflightsearch.Destination =objserch.TOAirportCode;
                objflightsearch.FlightCabinClass = objserch.TravelType;
                objflightsearch.PreferredDepartureTime = datefrm;
                objflightsearch.PreferredArrivalTime = dateTo;
                objflightsearch.Sources = "6E";

                dynamic searchflight = Tek_TravelAPI.SerachFlight(objflightsearch);
                var data = JsonConvert.SerializeObject(searchflight);
                //var searchflight = Tek_TravelAPI.SerachFlight(GetTokenValue,"1","0","0","true", "false", "1", PreferredAirlines, "CCU","BOM","1", Date_from, datetovalue, "6E");
                string TraceId = searchflight.Response.TraceId;
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }



            
        }

        public ActionResult FlightSearchDetails()
        {
            return View();
        }

        public ActionResult FlightSearchList()
        {

            return View();
        }
        public ActionResult FlightSearchSingleList()
        {

            return View();
        }
    }
}