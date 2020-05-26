namespace WHITELABEL.Web.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using WHITELABEL.Web.Helper;

    public class ConfigDetails
    {
        public static string Recharge_REQUEST_URLAPI
        {
            get { return Settings.GetValue("API_REQUESTURL"); }
        }
        public static string Recharge_LANDLINE_URLAPI
        {
            get { return Settings.GetValue("API_LANDLINEURL"); }
        }
        public static string Recharge_GETBALANCE_URLAPI
        {
            get { return Settings.GetValue("API_GET_BALANCE"); }
        }
        public static string Recharge_TRANSACTION_ENQUIRY_URLAPI
        {
            get { return Settings.GetValue("API_TRANSACTION_ENQUIRY"); }
        }
        public static string Recharge_BSNL_POSTPAID_URLAPI
        {
            get { return Settings.GetValue("API_BSNL_POSTPAID_URL"); }
        }
        public static string Recharge_MONEY_REMITTANCE_URLAPI
        {
            get { return Settings.GetValue("API_MONEY_REMITTANCE"); }
        }
        public static string Recharge_AUTHORIZATION_CODE_URLAPI
        {
            get { return Settings.GetValue("API_AUTHCODE"); }
        }
        public static string SMTP
        {
            get{ return "SMTP.gmail.com"; }
        }
        public static int SMTP_PORT
        {
            get { return 80; }
        }
        public static string ADMIN_EMAIL
        {
            get { return "test@test.com"; }
        }
        public static string ADMIN_USERNAME
        {
            get { return "adminuser"; }
        }
        public static string ADMIN_PASSWORD
        {
            get { return "1234567890"; }
        }
        public static string DMR_AddSenderCodeE06003
        { get { return "E06003"; } }
        public static string DMR_AddSenderCodeE06004
        { get { return "E06004"; } }
        public static string DMR_CheckAvailableSenderBalanceE06005
        { get { return "E06005"; } }
        public static string DMR_CheckAvailableSenderBalanceE06006
        { get { return "E06006"; } }
        public static string DMR_SenderExistsRequestE06007
        { get { return "E06007"; } }
        public static string DMR_SenderExistsRequestE06008
        { get { return "E06008"; } }
        public static string DMR_AddBenificiaryForSenderE06009
        { get { return "E06009"; } }
        public static string DMR_AddBenificiaryForSenderE06010
        { get { return "E06010"; } }
        public static string DMR_FetchBankDetailsE06029
        { get { return "E06029"; } }
        public static string DMR_FetchBankDetailsE06030
        { get { return "E06030"; } }
        public static string DMR_ValidateBeneficiaryAccountE06031
        { get { return "E06031"; } }
        public static string DMR_ValidateBeneficiaryAccountE06032
        { get { return "E06032"; } }
        public static string DMR_GetBeneficiaryDetailsE06011
        { get { return "E06011"; } }
        public static string DMR_GetBeneficiaryDetailsE06012
        { get { return "E06012"; } }
        public static string DMR_DeleteBeneficiaryE06013
        { get { return "E06013"; } }
        public static string DMR_DeleteBeneficiaryE06014
        { get { return "E06014"; } }
        public static string DMR_VerifyOTPE06021
        { get { return "E06021"; } }
        public static string DMR_VerifyOTPE06022
        { get { return "E06022"; } }
        public static string DMR_ResendOTPE06019
        { get { return "E06019"; } }
        public static string DMR_ResendOTPE06020
        { get { return "E06020"; } }
        public static string DMR_Transaction_Remit_Money_E06015
        { get { return "E06015"; } }
        public static string DMR_Transaction_Remit_Money_E06016
        { get { return "E06016"; } }
        public static string DMR_Get_Transaction_Enquiry_E06017
        { get { return "E06017"; } }
        public static string DMR_Get_Transaction_Enquiry_E06018
        { get { return "E06018"; } }
        public static string DMR_Get_Transaction_History_E06025
        { get { return "E06025"; } }
        public static string DMR_Get_Transaction_History_E06026
        { get { return "E06026"; } }

    }
}