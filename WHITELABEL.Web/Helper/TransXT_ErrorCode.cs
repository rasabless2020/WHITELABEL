using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using com.mobileware.transxt;

namespace WHITELABEL.Web.Helper
{
    public class TransXT_ErrorCode
    {
        public static string GetError(string code)
        {
            switch (code)
            {
                case "E0001":
                    return "JSON format of request in not correct";
                case "E0002":
                    return "Invalid credentials in request payload";
                case "E0003":
                    return "Session expired";
                case "E0004":
                    return "Invalid request";
                case "E0005":
                    return "Token manipulated";
                case "E0006":
                    return "Invalid token";
                case "E0007":
                    return "Corrupt or altered token";
                case "E0008":
                    return "Token expired";
                case "E0009":
                    return "Request is not generated from white listed IP";
                case "E0010":
                    return "Request sent for unauthorized service";
                case "E0011":
                    return "Invalid email format";
                case "E0012":
                    return "Invalid password format";
                case "E0013":
                    return "EMAIL_REGISTERED";
                case "E0014":
                    return "Could not register. Please try again later.";
                case "E0015":
                    return "Email could not be send. Please try again later.";
                case "E0016":
                    return "Invalid password";
                case "E0017":
                    return "Invalid service type";
                case "E0018":
                    return "PASS_MISMATCH";
                case "E0019":
                    return "INVALID_MOBILE_FORMAT";
                case "E0020":
                    return "MOBILE_REGISTERED";
                case "E0021":
                    return "NO_RECORD";
                case "E0022":
                    return "OTP_EXPIRIED";
                case "E0023":
                    return "INVALID_OTP_FORMAT";
                case "E0024":
                    return "OTP_ATTEMPTS_EXAUSTED";
                case "E0025":
                    return "INVALID_OTP";
                case "E0026":
                    return "USERNAME_REGISTERED";
                case "E0027":
                    return "INVALID_PAN";
                case "E0028":
                    return "INVALID_ADDRESS_FORMAT";
                case "E0029":
                    return "INVALID_ORGANIZATION_FORMAT";
                case "E0030":
                    return "INVALID_STATE_FORMAT";
                case "E0031":
                    return "INVALID_PINCODE_FORMAT";
                case "E0032":
                    return "INVALID_ISGST_FORMAT";
                case "E0033":
                    return "INVALID_GST_FORMAT";
                case "E0034":
                    return "INVALID_ADDRESS";
                case "E0035":
                    return "INVALID_USERNAME_FORMAT";
                case "E0036":
                    return "Invalid success URL";
                case "E0037":
                    return "Invalid failure URL";
                case "E0038":
                    return "Invalid aadhaar number";
                case "E0039":
                    return "Invalid modality (It can be BIOMETRIC or OTP)";
                case "E0040":
                    return "Invalid merchant request ID";
                case "E0041":
                    return "Invalid auth capture data";
                case "E0042":
                    return "Aadhaar bridge error type 1";
                case "E0043":
                    return "Aadhaar bridge error type 1";
                case "E0044":
                    return "VALIDATE_YOUR_EMAIL_ID";
                case "E0045":
                    return "Mobile Number Already Registered With Different Users";
                case "E0046":
                    return "Server error type 1";
                case "E0047":
                    return "Server error type 2";
                case "E0048":
                    return "Request sent for unauthorized service";
                case "E0049":
                    return "Insufficient funds with TransXT to process transaction";
                case "E0050":
                    return "Transaction status update failed";
                case "E0051":
                    return "Plan update failed";
                case "E0052":
                    return "Transaction Not Found";
                case "E0053":
                    return "Enter either Transaction ID or Client Reference ID";
                case "00":
                    return "SUCCESS";
                case "E1501":
                    return "PENDING: Please check with beneficiary";
                case "E1502":
                    return "FAILURE";
                case "E1503":
                    return "HashKey does not match";
                case "E1504":
                    return "Data not found in database";
                case "E1505":
                    return "Customer Id is already present";
                case "E1506":
                    return "Please check mobile number length";
                case "E1507":
                    return "Mobile number should only be in digits";
                case "E1508":
                    return "Recipient Id is already present";
                case "E1509":
                    return "Customer Id does not match to specified recipient";
                case "E1510":
                    return "HashKey field is blank";
                case "E1511":
                    return "Invalid Customer Id";
                case "E1512":
                    return "Invalid Transaction Id";
                case "E1513":
                    return "Customer Type is blank";
                case "E1514":
                    return "Please enter correct Customer type";
                case "E1515":
                    return "Initiator Id is blank";
                case "E1516":
                    return "Customer name is blank";
                case "E1517":
                    return "Account Number is blank";
                case "E1518":
                    return "Please enter Account Number in digit";
                case "E1519":
                    return "IFSC number is blank";
                case "E1520":
                    return "State field is blank";
                case "E1521":
                    return "Virtual Address is blank";
                case "E1522":
                    return "OTP is blank";
                case "E1523":
                    return "Customer Id is not present in Customer Database";
                case "E1524":
                    return "Client Ref Id and transaction Id does not match with the database";
                case "E1525":
                    return "Client Ref Id blank";
                case "E1526":
                    return "Client Ref Id blank";
                case "E1527":
                    return "Invalid bank code";
                case "E1528":
                    return "Please insert Correct OTPType";
                case "E1529":
                    return "Error in saving to database";
                case "E1530":
                    return "Invalid OTP";
                case "E1531":
                    return "Id does not match in OTP database";
                case "E1532":
                    return "Please insert Amount";
                case "E1533":
                    return "Please insert more than Zero Amount";
                case "E1534":
                    return "Please check Amount Digit";
                case "E1535":
                    return "MMID is blank";
                case "E1536":
                    return "MMID should contain 7 digits";
                case "E1537":
                    return "Cannot get Bank Name from Database";
                case "E1538":
                    return "Error in uploading file";
                case "E1539":
                    return "Error in saving file in Database";
                case "E1540":
                    return "Address field is blank";
                case "E1541":
                    return "Please enter 6 digit pin code number";
                case "E1542":
                    return "Please enter pin code in digits";
                case "E1543":
                    return "Initiator id is not found in merchant table";
                case "E1544":
                    return "Not getting value from IMPS Switch";
                case "E1545":
                    return "Error for Connecting IMPS Switch";
                case "E1546":
                    return "Merchant is inactive for transaction";
                case "E1547":
                    return "Agent Code is blank";
                case "E1548":
                    return "Problem in committing Add Recipient";
                case "E1549":
                    return "Transaction id is blank";
                case "E1550":
                    return "Narration is Blank";
                case "E1551":
                    return "Transaction Id is invalid";
                case "E1552":
                    return "Error in searching transaction id";
                case "E1553":
                    return "Transaction has already been refunded";
                case "E1554":
                    return "OTP has expired";
                case "E1555":
                    return "Recipient Modify Year limit End";
                case "E1556":
                    return "Recipient Modify Month limit End";
                case "E1557":
                    return "Please check MMID length";
                case "E1558":
                    return "Please enter correct IFSC code";
                case "E1559":
                    return "Please insert correct KYC Status";
                case "E1560":
                    return "Customer Id is not present";
                case "E1561":
                    return "Recipient Map Id is blank";                
                case "E1562":
                    return "Recipient Map Id should in digit";
                case "E1563":
                    return "This transaction is success not refunded";
                case "E1564":
                    return "This transaction is failed not refunded";
                case "E1565":
                    return "Error in finding transaction id";
                case "E1566":
                    return "Merchant code empty";
                case "E1567":
                    return "Merchant name empty";
                case "E1568":
                    return "Incorrect IP/IP empty";
                case "E1569":
                    return "Incorrect IP/IP empty";
                case "E1570":
                    return "Problem in committing add merchant";
                case "E1571":
                    return "Error in updating master table";
                case "E1572":
                    return "Initiator Id can only be in digits";
                case "E1573":
                    return "Please enter amount in digits";
                case "E1574":
                    return "Please check IFSC length";
                case "E1575":
                    return "Key cannot be blank";
                case "E1576":
                    return "Value cannot be blank";
                case "E1577":
                    return "Please enter channel";
                case "E1578":
                    return "Invalid channel";
                case "E1579":
                    return "Key does not match value in database";
                case "E1580":
                    return "Error in updating database";
                case "E1581":
                    return "Please enter correct Recipient type";
                case "E1582":
                    return "Currency cannot be blank";                
                case "E1583":
                    return "Value can only be digits";
                case "E1584":
                    return "Date of Birth is blank";
                case "E1585":
                    return "Date of Birth cannot be today's date";
                case "E1586":
                    return "Date of Birth can only be digits";
                case "E1587":
                    return "Error in adding customer to database";
                case "E1588":
                    return "No pending transactions available";
                case "E1589":
                    return "Error in getting transactions from database";
                case "E1590":
                    return "No transactions available";
                case "E1591":                    
                    return "Customer does not exist";
                case "E1592":
                    return "No merchants in database";
                case "E1593":
                    return "Error in retrieving merchants from database";
                case "E1594":
                    return "Merchant not present in database";
                case "E1595":
                    return "Error in getting merchant from database";
                case "E1596":
                    return "Error in updating merchant";
                case "E1597":
                    return "Merchant Id can only be digits";
                case "E1598":
                    return "Merchant Id is empty";
                case "E1599":
                    return "Amount cannot be more greater than 25000";
                case "E1600":
                    return "Error while inserting value in Database";
                case "E1601":
                    return "Error while uploading KYC documents";
                case "E1602":
                    return "Customer KYC status already updated";
                case "E1603":
                    return "Invalid IFSC code";
                case "E1604":
                    return "Error in getting value from database";
                case "E1605":
                    return "Invalid transaction id for refund";
                case "E1607":
                    return "Amount is greater than NON KYC Status";
                case "E1608":
                    return "Invalid IPAddress";
                case "E1609":
                    return "Invalid Token";
                case "E1610":
                    return "Non Kyc Customer transaction limit exceeded";
                case "E1611":
                    return "Kyc Customer transaction limit exceeded";
                case "E1612":
                    return "Invalid OTP";
                case "E1613":
                    return "Please check the mobile no length";
                case "E1614":
                    return "Please check the Mobile No digit";
                case "E1615":
                    return "Please insert 7 Digit MMID";
                case "E1616":
                    return "Please check MMID Digit";
                case "E1617":
                    return "Please insert 15 Digit Acc No";
                case "E1618":
                    return "Please check Acc No Digit";
                case "E1619":
                    return "Please insert 15 Digit Acc No";
                case "E1620":
                    return "Please check Acc No Digit";
                case "E1621":
                    return "Please insert Amount";
                case "E1622":
                    return "Please insert more than Zero Amount";
                case "E1623":
                    return "Please check Amount Digit";
                case "E1624":
                    return "Please insert 12 digit Aadhaar No";
                case "E1626":
                    return "Please check Aadhaar No digit";
                case "E1627":
                    return "Exception in cbs fund transfer";
                case "E1628":
                    return "MMID and Mobile Number Combination is Wrong";
                case "E1629":
                    return "Exception building msg";
                case "E1630":
                    return "Exception in message processing";
                case "E1631":
                    return "Error in finding NBIN";
                case "E1632":
                    return "Failure CBS Connection";
                case "E1633":
                    return "Account No not match";
                case "E1634":
                    return "Insuficient amount bal for transfer";
                case "E1635":
                    return "Error for CBS response";
                case "E1636":
                    return "MMID is not exist in CBS";
                case "E1637":
                    return "Error in Interbank Transaction";
                case "E1638":
                    return "Error in InterBank Refund";
                case "E1639":
                    return "Error in Intrabank Transaction";
                case "E1640":
                    return "SUCCESS";
                case "E1641":
                    return "Failure NPCI Connection";
                case "E1642":
                    return "Error for NPCI response";
                case "E1643":
                    return "Error for building ISO msg";
                case "E1644":
                    return "Failure for npci network echo message";
                case "E1645":
                    return "NPCI Decline the transaction successfully";
                case "E1646":
                    return "Not getting response from NPCI";
                case "E1647":
                    return "Exception in Sending Part";
                case "E1648":
                    return "Response Invalid Msg MTI";
                case "E1649":
                    return "Getting Response Network Msg";
                case "E1650":
                    return "Invalid response RRN";
                case "E1651":
                    return "Verification Msg Failure NPCI Connection";
                case "E1652":
                    return "Issue in IMPSEngine";
                case "E1653":
                    return "NBIN id not found In IMPS Switch";
                case "E1654":
                    return "Verification successful but original credit transaction declined";
                case "E1655":
                    return "If the mobile number/MMID combination entered by the customer is not registered or is mismatch";
                case "E1656":
                    return "Daily allowed amount limit for the beneficiary is exceeded";
                case "E1657":
                    return "Beneficiary account is frozen";
                case "E1658":
                    return "Beneficiary account is NRE account";
                case "E1659":
                    return "Invalid NBIN no";
                case "E1660":
                    return "Beneficiary account is closed";
                case "E1661":
                    return "When the limit set for the member bank as per Settlement guarantee fund has been exceeded";
                case "E1662":
                    return "No connectivity between beneficary bank and NPCI before creaditing message";
                case "E1663":
                    return "NPCI time out transaction";
                case "E1664":
                    return "Invalid response code";
                case "E1665":
                    return "Issuer node offline";
                case "E1666":
                    return "Unable to process";
                case "E1667":
                    return "Invalid Response code";
                case "E1668":
                    return "Host (CBS ) offline";
                case "E1669":
                    return "Acquiring bank declines the transaction, due to some error";
                case "E1670":
                    return "NPCI declines the transaction as acquiring bank has not yet implement customer initiated person-to-merchant transaction";
                case "E1671":
                    return "Acquiring bank identifies the payee to be an individual and not a merchant, and advises customer to use person-to-person form and not person-to-merchant form for making payment";
                case "E1672":
                    return "Acquiring bank identifies the payee to be a merchant and not an individual, and advises the customer to use person-to-merchant form and not person-to-person form for making payment";
                case "E1673":
                    return "Acquiring bank not able to send payment reference data to merchant URL or merchant URL not able to respond to payment reference data validation request from acquiring bank";
                case "E1674":
                    return "OTP Expired";
                case "E1675":
                    return "NPCI declines the transaction as issuing bank has not yet implemented merchant initiated person-to-merchant transaction";
                case "E1676":
                    return "Acquiring Bank CBS or node offline";
                case "E1677":
                    return "No action taken";
                case "E1678":
                    return "Invalid message format";
                case "E1679":
                    return "PIN block error";
                case "E1680":
                    return "Invalid response code";
                case "E1681":
                    return "Issuer or switch is inoperative";
                case "E1682":
                    return "System malfunction";
                case "E1683":
                    return "Not sufficient funds";
                case "E1684":
                    return "Transaction not permitted to account";
                case "E1685":
                    return "Invalid transaction";
                case "E1686":
                    return "Duplicate transaction";
                case "E1687":
                    return "E1687";
                case "E0064":
                    return "Access has been restricted";
                case "9000":
                    return "Error mapping not found";
                default:
                    return "";
            }
        }
    }
}
//