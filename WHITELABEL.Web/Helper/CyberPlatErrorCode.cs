using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHITELABEL.Web.Helper
{
    public class CyberPlatErrorCode
    {
        public static string GetCyberPlatGenericError(string code)
        {
            switch (code)
            {
                case "0":
                    return "Successfully completed.";
                case "1":
                    return "Session with this number already exists.";
                case "2":
                    return "Invalid Dealer code.";
                case "3":
                    return "Invalid acceptance outlet code.";
                case "4":
                    return "Invalid Operator code.";
                case "5":
                    return "Invalid session code format.";
                case "6":
                    return "Invalid EDS.";
                case "7":
                    return "Invalid amount format or amount value is out of admissible range.";
                case "8":
                    return "Invalid phone number format.";
                case "9":
                    return "Invalid format of personal account number.";
                case "10":
                    return "Invalid request message format.";
                case "11":
                    return "Session with such a number does not exist.";
                case "12":
                    return "The request is made from an unregistered IP.";
                case "13":
                    return "The outlet is not registered with the Service Provider.";                
                case "15":
                    return "Payments to the benefit of this operator are not supported by the system.";                
                case "17":
                    return "The phone number does not match the previously entered number.";
                case "18":
                    return "The payment amount does not match the previously entered amount.";
                case "19":
                    return "The account number does not match the previously entered number.";
                case "20":
                    return "The payment is being completed.";
                case "21":
                    return "Not enough funds for effecting the payment.";
                case "22":
                    return "The payment has not been accepted. Funds transfer error.";
                case "23":
                    return "Invalid Mobile Number. Make sure your number belongs to this provider.";
                case "24":
                    return "Error of connection with the provider’s server or a routine break in CyberPlat®.";
                case "25":
                    return "Effecting of this type of payments is suspended.";
                case "26":
                    return "Payments of this Dealer are temporarily blocked";
                case "27":
                    return "Operations with this account are suspended";              
                case "30":
                    return "General system failure.";
                case "31":
                    return "Exceeded number of simultaneously processed requests.";
                case "32":
                    return "Repeated payment within 60 minutes from the end of payment effecting process";
                case "33":
                    return "This denomination is applicable in <Flexi OR Special> category";
                case "34":
                    return "Transaction with such number could not be found.";
                case "35":
                    return "Payment status alteration error.";
                case "36":
                    return "Invalid payment status.";
                case "37":
                    return "An attempt of referring to the gateway that is different from the gateway at the previous";
                case "38":
                    return "Invalid date. The effective period of the payment may have expired.";
                case "39":
                    return "Invalid account number.";
                case "40":
                    return "The card of the specified value is not registered in the system";
                case "41":
                    return "Error in saving the payment in the system.";
                case "42":
                    return "Error in saving the receipt to the database.";
                case "43":
                    return "Your working session in the system is invalid (the duration of the session may have expired), try to re-enter.";
                case "44":
                    return "The Client cannot operate on this trading server.";
                case "45":
                    return "No license is available for accepting payments to the benefit of this operator.";
                case "46":
                    return "Could not complete the erroneous payment.";
                case "47":
                    return "Time limitation of access rights has been activated.";
                case "48":
                    return "Error in saving the session data to the database.";
                
                case "50":
                    return "Effecting payments in the system is temporarily impossible.";
                case "51":
                    return "Data are not found in the system.";
                case "52":
                    return "The Dealer may be blocked. The Dealer’s current status does not allow effecting payments";
                case "53":
                    return "The Acceptance Outlet may be blocked. The Acceptance Outlet’s current status does not allow effecting payments.";
                case "54":
                    return "The Operator may be blocked. The Operator’s current status does not allow effecting payments";
                case "55":
                    return "The Dealer Type does not allow effecting payments.";
                case "56":
                    return "An Acceptance Outlet of another type was expected. This Acceptance Outlet type does not allow effecting payments.";
                case "57":
                    return "An Operator of another type was expected. This Operator type does not allow effecting payments";
                case "81":
                    return "Exceeded the maximum payment amount.";
                case "82":
                    return "Daily debit amount has been exceeded.";
                case "83":
                    return "Maximum payment amount for the outlet has been exceeded.";
                case "84":
                    return "Daily total amount for the outlet has been exceeded.";
                case "85":
                    return "AMOUNT ALL is invalid";
                case "86":
                    return "Invalid rate value";
                case "87":
                    return "Beneficiary is blocked";
                case "88":
                    return "Duplicate Transaction (Same Mobile Number)";
                case "89":
                    return "A limit by a beneficiary is reached";
                case "134":
                    return "Wrong Key";
                case "137":
                    return "Wrong key or passphrase";
                case "224":
                    return "Operator system is down. Please try again later.";
                case "333":
                    return "Unknown error";
                case "171":
                    return "Manually Cancelled";
                case "270":
                    return "Transaction limit on same number reached";
                default:
                    return "Default case";
            }
        }
        public static string GETCyberPlatDMT2ErrorCode(string code)
        {
            switch (code)
            {
                case "0":
                    return "";                
                default:
                    return "Default case";
            }
        }
        public static string GETCyberPlatBBPSErrorCode(string code)
        {
            switch (code)
            {
                case "23":
                    return "Either no bill pending/ due date has passed/ invalid biller details/ Arrears or Notice bill/ Credit bill~ Or Please note that due date for the latest bill available online has already past.~ Or Customer ACCOUNT is missed Or Wrong Authenticator3 value";
                case "270":
                    return "Transaction limit on same number reached";
                default:
                    return "Default case";
            }
        }
        public static string GETCyberPlatUPIErrorCode(string code)
        {
            switch (code)
            {
                case "0":
                    return "Transaction successful";
                case "224":
                    return "Insufficient data Or Transaction cannot be processed Or  Invalid Request Or Invalid Merchant ID Or Invalid Data Or Merchant TranId is not available Or Service unavailable. Please try later Or Invalid VPA Or Insufficient Amount .";
                case "7":
                    return "Denomination Not Found";
                case "88":
                    return "This transaction is already processed (Online duplicate transaction) Or Request has already been initiated for this transaction (Offline duplicate transaction)";
                default:
                    return "Default case";
            }
        }
    }
}