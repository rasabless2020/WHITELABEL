using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WHITELABEL.Web.Helper
{
    public class InstantPayError
    {
        public static string GetError(string code)
        {
            switch (code)
            {
                case "TXN":
                    return "Transaction Successful";
                case "TUP":
                    return "Transaction Under Process";
                case "RPI":
                    return "Request Parameters are Invalid or Incomplete";
                case "UAD":
                    return "User Access Denied";
                case "IAT":
                    return "Invalid Access Token";
                case "IAC":
                    return "Invalid Dealer Credentials";
                case "AAB":
                    return "Dealer Account Blocked, Contact Helpdesk";
                case "IAB":
                    return "Insufficient wallet Balance";
                case "ISP":
                    return "Invalid Service Provider";
                case "DID":
                    return "Duplicate Dealer Transaction ID";
                case "DTX":
                    return "Duplicate Transaction, Try after 15 Minutes";
                case "IAN":
                    return "Invalid Account Number";
                case "IRA":
                    return "Invalid Refill Amount";
                case "DTB":
                    return "Denomination Temporarily Barred";
                case "RBT":
                    return "Refill Barred Temporarily, Contact Service Provider";
                case "SPE":
                    return "Service Provider Error, Try Later";
                case "SPD":
                    return "Service Provider Downtime";
                case "UED":
                    return "Unknown Error Description, Contact Helpdesk";
                case "IEC":
                    return "Invalid Error Code";
                case "IRT":
                    return "Invalid Response Type";
                case "ITI":
                    return "Invalid Transaction ID";
                case "TSU":
                    return "Transaction Status Unavailable, Try Again";
                case "IPE":
                    return "Internal Processing Error, Try Later";
                case "ISE":
                    return "System Error, Try Later";
                case "TRP":
                    return "Transaction Refund Processed, Wallet Credited";
                case "OUI":
                    return "Outlet Unauthorized or Inactive";
                case "ODI":
                    return "Outlet Data Incorrect";
                case "TDE":
                    return "Transaction Dispute Error,Contact Helpdesk";
                case "DLS":
                    return "Dispute Logged Successfully";
                case "RNF":
                    return "Remitter Not Found";
                case "RAR":
                    return "Remitter Already Registered";
                case "IVC":
                    return "Invalid Verification Code or OTP";
                case "IUA":
                    return "Invalid User Account";
                case "SNA":
                    return "Service not Available";
                case "ERR":
                    return "Unknown Error";
                case "FAB":
                    return "Failure at Bank end";
                case "UFC":
                    return "Fare Has Changed";
                default:
                    return "Default case";
            }
        }
    }
}
