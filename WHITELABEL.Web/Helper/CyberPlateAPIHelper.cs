using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CyberPlatOpenSSL;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace WHITELABEL.Web.Helper
{
    public static class CyberPlateAPIHelper
    {
        public static class CyberPlateAPI
        {
            //OpenSSL ssl = new OpenSSL();
            //StringBuilder str = new StringBuilder();
            public static string SessionNo = DateTime.Parse(DateTime.Now.ToString()).ToString("dddMMMddyyyyHHmmss");
            public static string CERTNO = System.Configuration.ConfigurationManager.AppSettings["CyberPlateCERTNo"];
            public static string CYBER_SDCode = System.Configuration.ConfigurationManager.AppSettings["CyberPlateSDCode"];
            public static string CYBER_APCode = System.Configuration.ConfigurationManager.AppSettings["CyberPlateAPCode"];
            public static string CYBER_OPCode = System.Configuration.ConfigurationManager.AppSettings["CyberPlateOPCode"];
            //public static string _strRequest(string SDCode, string APCode, string OPCode, string SessionNo, string txtMobileNo, string account, string authenticator3, string amount)
            //public static string _strValidationRequestLogs(string txtMobileNo, string account, string authenticator3, string amount)
            public static string _strValidationRequestLogs(string SessionID, string txtMobileNo, string amount,string OperatorName,string REQ_TYPE)
            {
                StringBuilder _reqStr = new StringBuilder();
                #region Create Request
                //_reqStr.Append("CERT=" + txtCERTNo.Text + Environment.NewLine);
                _reqStr.Append("CERT=" + CERTNO + Environment.NewLine);
                _reqStr.Append("SD=" + CYBER_SDCode + Environment.NewLine);
                _reqStr.Append("AP=" + CYBER_APCode + Environment.NewLine);
                _reqStr.Append("OP=" + CYBER_OPCode + Environment.NewLine);
                _reqStr.Append("SESSION=" + SessionID + Environment.NewLine);
                _reqStr.Append("NUMBER=" + txtMobileNo + Environment.NewLine);
                if (OperatorName == "Reliance JIO")
                {
                    _reqStr.Append("PlanOffer=" + REQ_TYPE + Environment.NewLine);
                }
                //_reqStr.Append("ACCOUNT=" + account + Environment.NewLine);
                //_reqStr.Append("Authenticator3=" + authenticator3 + Environment.NewLine);
                _reqStr.Append("AMOUNT=" + amount + Environment.NewLine);
                _reqStr.Append("TERM_ID=" + CYBER_APCode + Environment.NewLine);//Mostly value of TERM_ID=AP Code, but value may be vary.
                _reqStr.Append("COMMENT=Test recharge");
                #endregion
                return _reqStr.ToString();
            }
            public static string _strStatusRequestLogs()
            {
                StringBuilder _reqStr = new StringBuilder();
                #region Create Request
                //_reqStr.Append("CERT=" + txtCERTNo.Text + Environment.NewLine);
                _reqStr.Append("CERT=" + CERTNO + Environment.NewLine);
                _reqStr.Append("SD=" + CYBER_SDCode + Environment.NewLine);
                _reqStr.Append("AP=" + CYBER_APCode + Environment.NewLine);
                _reqStr.Append("OP=" + CYBER_OPCode + Environment.NewLine);
                _reqStr.Append("SESSION=" + SessionNo);
                #endregion
                return _reqStr.ToString();
            }
            public static string _strDelaerWalletBalanceCheck()
            {
                StringBuilder _reqStr = new StringBuilder();
                #region Create Request
                //_reqStr.Append("CERT=" + txtCERTNo.Text + Environment.NewLine);
                _reqStr.Append("CERT=" + CERTNO + Environment.NewLine);
                _reqStr.Append("SD=" + CYBER_SDCode + Environment.NewLine);
                _reqStr.Append("AP=" + CYBER_APCode + Environment.NewLine);
                _reqStr.Append("OP=" + CYBER_OPCode + Environment.NewLine);
                _reqStr.Append("SESSION=" + SessionNo);
                #endregion
                return _reqStr.ToString();
            }
        }
        public static class CyberPlatMobileRecharge
        {
            public static string SessionNo = DateTime.Parse(DateTime.Now.ToString()).ToString("dddMMMddyyyyHHmmss");
            public static string CERTNO = System.Configuration.ConfigurationManager.AppSettings["CyberPlateCERTNo"];
            public static string CYBER_SDCode = System.Configuration.ConfigurationManager.AppSettings["CyberPlateSDCode"];
            public static string CYBER_APCode = System.Configuration.ConfigurationManager.AppSettings["CyberPlateAPCode"];
            public static string CYBER_OPCode = System.Configuration.ConfigurationManager.AppSettings["CyberPlateOPCode"];
            //public static string _strValidationPrepaidMobileRequest(string txtMobileNo, string account, string authenticator3, string amount)
            public static string _strValidationPrepaidMobileRequest(string txtMobileNo, string amount)
            {
                StringBuilder _reqStr = new StringBuilder();
                #region Create Request
                //_reqStr.Append("CERT=" + txtCERTNo.Text + Environment.NewLine);
                _reqStr.Append("CERT=" + CERTNO + Environment.NewLine);
                _reqStr.Append("SD=" + CYBER_SDCode + Environment.NewLine);
                _reqStr.Append("AP=" + CYBER_APCode + Environment.NewLine);
                _reqStr.Append("OP=" + CYBER_OPCode + Environment.NewLine);
                _reqStr.Append("SESSION=" + SessionNo + Environment.NewLine);
                _reqStr.Append("NUMBER=" + txtMobileNo + Environment.NewLine);                
                _reqStr.Append("AMOUNT=" + amount + Environment.NewLine);
                _reqStr.Append("AMOUNT_ALL=" + amount + Environment.NewLine);
                _reqStr.Append("TERM_ID=" + CYBER_APCode + Environment.NewLine);//Mostly value of TERM_ID=AP Code, but value may be vary.
                //_reqStr.Append("COMMENT=test");
                #endregion
                return _reqStr.ToString();
            }
            public static string _strPaymentPrepaidMobileRequest(string txtMobileNo, string amount)
            {
                StringBuilder _reqStr = new StringBuilder();
                #region Create Request
                //_reqStr.Append("CERT=" + txtCERTNo.Text + Environment.NewLine);
                _reqStr.Append("CERT=" + CERTNO + Environment.NewLine);
                _reqStr.Append("SD=" + CYBER_SDCode + Environment.NewLine);
                _reqStr.Append("AP=" + CYBER_APCode + Environment.NewLine);
                _reqStr.Append("OP=" + CYBER_OPCode + Environment.NewLine);
                _reqStr.Append("SESSION=" + SessionNo + Environment.NewLine);
                _reqStr.Append("NUMBER=" + txtMobileNo + Environment.NewLine);
                _reqStr.Append("AMOUNT=" + amount + Environment.NewLine);
                _reqStr.Append("AMOUNT_ALL=" + amount + Environment.NewLine);
                _reqStr.Append("TERM_ID=" + CYBER_APCode + Environment.NewLine);//Mostly value of TERM_ID=AP Code, but value may be vary.
                _reqStr.Append("COMMENT=test");
                #endregion
                return _reqStr.ToString();
            }
            public static string _strStatusMobileRequestLogs(string txtMobileNo, string account, string authenticator3, string amount)
            {
                StringBuilder _reqStr = new StringBuilder();
                #region Create Request
                //_reqStr.Append("CERT=" + txtCERTNo.Text + Environment.NewLine);
                _reqStr.Append("CERT=" + CERTNO + Environment.NewLine);
                _reqStr.Append("SD=" + CYBER_SDCode + Environment.NewLine);
                _reqStr.Append("AP=" + CYBER_APCode + Environment.NewLine);
                _reqStr.Append("OP=" + CYBER_OPCode + Environment.NewLine);
                _reqStr.Append("SESSION=" + SessionNo + Environment.NewLine);
                #endregion
                return _reqStr.ToString();
            }
        }
        public static class CyberPlatDTHRecharge
        {
            public static string SessionNo = DateTime.Parse(DateTime.Now.ToString()).ToString("dddMMMddyyyyHHmmss");
            public static string CERTNO = System.Configuration.ConfigurationManager.AppSettings["CyberPlateCERTNo"];
            public static string CYBER_SDCode = System.Configuration.ConfigurationManager.AppSettings["CyberPlateSDCode"];
            public static string CYBER_APCode = System.Configuration.ConfigurationManager.AppSettings["CyberPlateAPCode"];
            public static string CYBER_OPCode = System.Configuration.ConfigurationManager.AppSettings["CyberPlateOPCode"];
            //public static string _strValidationPrepaidMobileRequest(string txtMobileNo, string account, string authenticator3, string amount)
            public static string _strValidationDTHRequest(string SessionId,string txtMobileNo, string amount,string AllAmount)
            {
                StringBuilder _reqStr = new StringBuilder();
                #region Create Request
                //_reqStr.Append("CERT=" + txtCERTNo.Text + Environment.NewLine);
                _reqStr.Append("CERT=" + CERTNO + Environment.NewLine);
                _reqStr.Append("SD=" + CYBER_SDCode + Environment.NewLine);
                _reqStr.Append("AP=" + CYBER_APCode + Environment.NewLine);
                _reqStr.Append("OP=" + CYBER_OPCode + Environment.NewLine);
                _reqStr.Append("SESSION=" + SessionId + Environment.NewLine);
                _reqStr.Append("NUMBER=" + txtMobileNo + Environment.NewLine);
                _reqStr.Append("AMOUNT=" + amount + Environment.NewLine);
                _reqStr.Append("AMOUNT_ALL=" + AllAmount + Environment.NewLine);
                _reqStr.Append("TERM_ID=" + CYBER_APCode + Environment.NewLine);//Mostly value of TERM_ID=AP Code, but value may be vary.
                _reqStr.Append("COMMENT=test");
                #endregion
                return _reqStr.ToString();
            }
            public static string _strPaymentDTHRequest(string SessionId, string txtMobileNo, string amount, string AllAmount)
            {
                StringBuilder _reqStr = new StringBuilder();
                #region Create Request
                //_reqStr.Append("CERT=" + txtCERTNo.Text + Environment.NewLine);
                _reqStr.Append("CERT=" + CERTNO + Environment.NewLine);
                _reqStr.Append("SD=" + CYBER_SDCode + Environment.NewLine);
                _reqStr.Append("AP=" + CYBER_APCode + Environment.NewLine);
                _reqStr.Append("OP=" + CYBER_OPCode + Environment.NewLine);
                _reqStr.Append("SESSION=" + SessionNo + Environment.NewLine);
                _reqStr.Append("NUMBER=" + txtMobileNo + Environment.NewLine);
                _reqStr.Append("AMOUNT=" + amount + Environment.NewLine);
                _reqStr.Append("AMOUNT_ALL=" + amount + Environment.NewLine);
                _reqStr.Append("TERM_ID=" + CYBER_APCode + Environment.NewLine);//Mostly value of TERM_ID=AP Code, but value may be vary.
                _reqStr.Append("COMMENT=test");
                #endregion
                return _reqStr.ToString();
            }
            public static string _strStatusDTHRequestLogs(string txtMobileNo, string account, string authenticator3, string amount)
            {
                StringBuilder _reqStr = new StringBuilder();
                #region Create Request
                //_reqStr.Append("CERT=" + txtCERTNo.Text + Environment.NewLine);
                _reqStr.Append("CERT=" + CERTNO + Environment.NewLine);
                _reqStr.Append("SD=" + CYBER_SDCode + Environment.NewLine);
                _reqStr.Append("AP=" + CYBER_APCode + Environment.NewLine);
                _reqStr.Append("OP=" + CYBER_OPCode + Environment.NewLine);
                _reqStr.Append("SESSION=" + SessionNo + Environment.NewLine);
                #endregion
                return _reqStr.ToString();
            }
        }
        public static class CyberPlatDataCardRecharge
        {
            public static string SessionNo = DateTime.Parse(DateTime.Now.ToString()).ToString("dddMMMddyyyyHHmmss");
            public static string CERTNO = System.Configuration.ConfigurationManager.AppSettings["CyberPlateCERTNo"];
            public static string CYBER_SDCode = System.Configuration.ConfigurationManager.AppSettings["CyberPlateSDCode"];
            public static string CYBER_APCode = System.Configuration.ConfigurationManager.AppSettings["CyberPlateAPCode"];
            public static string CYBER_OPCode = System.Configuration.ConfigurationManager.AppSettings["CyberPlateOPCode"];
            //public static string _strValidationPrepaidMobileRequest(string txtMobileNo, string account, string authenticator3, string amount)
            public static string _strValidationDataCardRequest(string txtMobileNo, string amount)
            {
                StringBuilder _reqStr = new StringBuilder();
                #region Create Request
                //_reqStr.Append("CERT=" + txtCERTNo.Text + Environment.NewLine);
                _reqStr.Append("CERT=" + CERTNO + Environment.NewLine);
                _reqStr.Append("SD=" + CYBER_SDCode + Environment.NewLine);
                _reqStr.Append("AP=" + CYBER_APCode + Environment.NewLine);
                _reqStr.Append("OP=" + CYBER_OPCode + Environment.NewLine);
                _reqStr.Append("SESSION=" + SessionNo + Environment.NewLine);
                _reqStr.Append("NUMBER=" + txtMobileNo + Environment.NewLine);
                _reqStr.Append("AMOUNT=" + amount + Environment.NewLine);
                _reqStr.Append("AMOUNT_ALL=" + amount + Environment.NewLine);
                _reqStr.Append("TERM_ID=" + CYBER_APCode + Environment.NewLine);//Mostly value of TERM_ID=AP Code, but value may be vary.
                _reqStr.Append("COMMENT=test");
                #endregion
                return _reqStr.ToString();
            }
            public static string _strPaymentDataCardRequest(string txtMobileNo, string amount)
            {
                StringBuilder _reqStr = new StringBuilder();
                #region Create Request
                //_reqStr.Append("CERT=" + txtCERTNo.Text + Environment.NewLine);
                _reqStr.Append("CERT=" + CERTNO + Environment.NewLine);
                _reqStr.Append("SD=" + CYBER_SDCode + Environment.NewLine);
                _reqStr.Append("AP=" + CYBER_APCode + Environment.NewLine);
                _reqStr.Append("OP=" + CYBER_OPCode + Environment.NewLine);
                _reqStr.Append("SESSION=" + SessionNo + Environment.NewLine);
                _reqStr.Append("NUMBER=" + txtMobileNo + Environment.NewLine);
                _reqStr.Append("AMOUNT=" + amount + Environment.NewLine);
                _reqStr.Append("AMOUNT_ALL=" + amount + Environment.NewLine);
                _reqStr.Append("TERM_ID=" + CYBER_APCode + Environment.NewLine);//Mostly value of TERM_ID=AP Code, but value may be vary.
                _reqStr.Append("COMMENT=test");
                #endregion
                return _reqStr.ToString();
            }
            public static string _strStatusDataCardRequestLogs(string txtMobileNo, string account, string authenticator3, string amount)
            {
                StringBuilder _reqStr = new StringBuilder();
                #region Create Request
                //_reqStr.Append("CERT=" + txtCERTNo.Text + Environment.NewLine);
                _reqStr.Append("CERT=" + CERTNO + Environment.NewLine);
                _reqStr.Append("SD=" + CYBER_SDCode + Environment.NewLine);
                _reqStr.Append("AP=" + CYBER_APCode + Environment.NewLine);
                _reqStr.Append("OP=" + CYBER_OPCode + Environment.NewLine);
                _reqStr.Append("SESSION=" + SessionNo + Environment.NewLine);
                #endregion
                return _reqStr.ToString();
            }
        }
        public static class CyberPlatRelianceJIORecharge
        {
            public static string SessionNo = DateTime.Parse(DateTime.Now.ToString()).ToString("dddMMMddyyyyHHmmss");
            public static string CERTNO = System.Configuration.ConfigurationManager.AppSettings["CyberPlateCERTNo"];
            public static string CYBER_SDCode = System.Configuration.ConfigurationManager.AppSettings["CyberPlateSDCode"];
            public static string CYBER_APCode = System.Configuration.ConfigurationManager.AppSettings["CyberPlateAPCode"];
            public static string CYBER_OPCode = System.Configuration.ConfigurationManager.AppSettings["CyberPlateOPCode"];
            public static string _strValidationRelianceJIOFetchPlanRequest(string txtMobileNo, string amount)
            {
                StringBuilder _reqStr = new StringBuilder();
                #region Create Request
                //_reqStr.Append("CERT=" + txtCERTNo.Text + Environment.NewLine);
                _reqStr.Append("CERT=" + CERTNO + Environment.NewLine);
                _reqStr.Append("SD=" + CYBER_SDCode + Environment.NewLine);
                _reqStr.Append("AP=" + CYBER_APCode + Environment.NewLine);
                _reqStr.Append("OP=" + CYBER_OPCode + Environment.NewLine);
                _reqStr.Append("SESSION=" + SessionNo + Environment.NewLine);
                _reqStr.Append("NUMBER=" + txtMobileNo + Environment.NewLine);
                _reqStr.Append("REQ_TYPE=" + "1" + Environment.NewLine);
                _reqStr.Append("AMOUNT=" + amount + Environment.NewLine);
                //_reqStr.Append("TERM_ID=" + CYBER_APCode + Environment.NewLine);//Mostly value of TERM_ID=AP Code, but value may be vary.
                _reqStr.Append("COMMENT=test");
                #endregion
                return _reqStr.ToString();
            }
            //public static string _strValidationPrepaidMobileRequest(string txtMobileNo, string account, string authenticator3, string amount)
            public static string _strValidationRelianceJIORequest(string txtMobileNo, string amount, string PlanOffer)
            {
                StringBuilder _reqStr = new StringBuilder();
                #region Create Request
                //_reqStr.Append("CERT=" + txtCERTNo.Text + Environment.NewLine);
                _reqStr.Append("CERT=" + CERTNO + Environment.NewLine);
                _reqStr.Append("SD=" + CYBER_SDCode + Environment.NewLine);
                _reqStr.Append("AP=" + CYBER_APCode + Environment.NewLine);
                _reqStr.Append("OP=" + CYBER_OPCode + Environment.NewLine);
                _reqStr.Append("SESSION=" + SessionNo + Environment.NewLine);
                _reqStr.Append("NUMBER=" + txtMobileNo + Environment.NewLine);
                _reqStr.Append("PlanOffer=" + PlanOffer + Environment.NewLine);
                _reqStr.Append("AMOUNT=" + amount + Environment.NewLine);
                
                _reqStr.Append("TERM_ID=" + CYBER_APCode + Environment.NewLine);//Mostly value of TERM_ID=AP Code, but value may be vary.
                _reqStr.Append("COMMENT=test");
                #endregion
                return _reqStr.ToString();
            }
            public static string _strPaymentRelianceJIORequest(string txtMobileNo, string amount,string PlanOffer)
            {
                StringBuilder _reqStr = new StringBuilder();
                #region Create Request
                //_reqStr.Append("CERT=" + txtCERTNo.Text + Environment.NewLine);
                _reqStr.Append("CERT=" + CERTNO + Environment.NewLine);
                _reqStr.Append("SD=" + CYBER_SDCode + Environment.NewLine);
                _reqStr.Append("AP=" + CYBER_APCode + Environment.NewLine);
                _reqStr.Append("OP=" + CYBER_OPCode + Environment.NewLine);
                _reqStr.Append("SESSION=" + SessionNo + Environment.NewLine);
                _reqStr.Append("NUMBER=" + txtMobileNo + Environment.NewLine);
                _reqStr.Append("PlanOffer=" + PlanOffer + Environment.NewLine);
                _reqStr.Append("AMOUNT=" + amount + Environment.NewLine);                
                //_reqStr.Append("TERM_ID=" + CYBER_APCode + Environment.NewLine);//Mostly value of TERM_ID=AP Code, but value may be vary.
                _reqStr.Append("COMMENT=test");
                #endregion
                return _reqStr.ToString();
            }
            public static string _strStatusRelianceJIORequestLogs(string txtMobileNo, string account, string authenticator3, string amount)
            {
                StringBuilder _reqStr = new StringBuilder();
                #region Create Request
                //_reqStr.Append("CERT=" + txtCERTNo.Text + Environment.NewLine);
                _reqStr.Append("CERT=" + CERTNO + Environment.NewLine);
                _reqStr.Append("SD=" + CYBER_SDCode + Environment.NewLine);
                _reqStr.Append("AP=" + CYBER_APCode + Environment.NewLine);
                _reqStr.Append("OP=" + CYBER_OPCode + Environment.NewLine);
                _reqStr.Append("SESSION=" + SessionNo + Environment.NewLine);
                #endregion
                return _reqStr.ToString();
            }
        }
        public static class CyberPlatMobileBBPSPostpaidRecharge
        {
            public static string SessionNo = DateTime.Parse(DateTime.Now.ToString()).ToString("dddMMMddyyyyHHmmss");
            public static string CERTNO = System.Configuration.ConfigurationManager.AppSettings["CyberPlateCERTNo"];
            public static string CYBER_SDCode = System.Configuration.ConfigurationManager.AppSettings["CyberPlateSDCode"];
            public static string CYBER_APCode = System.Configuration.ConfigurationManager.AppSettings["CyberPlateAPCode"];
            public static string CYBER_OPCode = System.Configuration.ConfigurationManager.AppSettings["CyberPlateOPCode"];
            //public static string _strValidationPrepaidMobileRequest(string txtMobileNo, string account, string authenticator3, string amount)
            public static string _strValidationMobilePostpaidRequest(string txtMobileNo, string amount,string AgentId,string fName, string lName,string Email,string benMobile,string GeoCode,string Pin,string TERMINAL)
            {
                StringBuilder _reqStr = new StringBuilder();
                #region Create Request
                //_reqStr.Append("CERT=" + txtCERTNo.Text + Environment.NewLine);
                _reqStr.Append("CERT=" + CERTNO + Environment.NewLine);
                _reqStr.Append("SD=" + CYBER_SDCode + Environment.NewLine);
                _reqStr.Append("AP=" + CYBER_APCode + Environment.NewLine);
                _reqStr.Append("OP=" + CYBER_OPCode + Environment.NewLine);
                _reqStr.Append("SESSION=" + SessionNo + Environment.NewLine);
                _reqStr.Append("AgentId=" + AgentId + Environment.NewLine);
                _reqStr.Append("Channel=" + "AGT" + Environment.NewLine);
                _reqStr.Append("fName=" + fName + Environment.NewLine);
                _reqStr.Append("lName=" + lName + Environment.NewLine);
                _reqStr.Append("PanCardNo=" + "NA" + Environment.NewLine);
                _reqStr.Append("Aadhar=" + "NA" + Environment.NewLine);
                _reqStr.Append("CardType=" + "NA" + Environment.NewLine);
                _reqStr.Append("Email=" + Email + Environment.NewLine);
                _reqStr.Append("benMobile=" + benMobile + Environment.NewLine);
                _reqStr.Append("GeoCode=" + GeoCode + Environment.NewLine);
                _reqStr.Append("Pin=" + Pin + Environment.NewLine);
                _reqStr.Append("TERMINAL_ID=" + TERMINAL + Environment.NewLine);
                _reqStr.Append("NUMBER=" + txtMobileNo + Environment.NewLine);
                _reqStr.Append("AMOUNT=" + amount + Environment.NewLine);
                _reqStr.Append("AMOUNT_ALL=" + amount + Environment.NewLine);
                _reqStr.Append("TERM_ID=" + CYBER_APCode + Environment.NewLine);//Mostly value of TERM_ID=AP Code, but value may be vary.
                _reqStr.Append("COMMENT=test");
                #endregion
                return _reqStr.ToString();
            }
            public static string _strPaymentMobilePostpaidRequest(string txtMobileNo, string amount)
            {
                StringBuilder _reqStr = new StringBuilder();
                #region Create Request
                //_reqStr.Append("CERT=" + txtCERTNo.Text + Environment.NewLine);
                _reqStr.Append("CERT=" + CERTNO + Environment.NewLine);
                _reqStr.Append("SD=" + CYBER_SDCode + Environment.NewLine);
                _reqStr.Append("AP=" + CYBER_APCode + Environment.NewLine);
                _reqStr.Append("OP=" + CYBER_OPCode + Environment.NewLine);
                _reqStr.Append("SESSION=" + SessionNo + Environment.NewLine);
                _reqStr.Append("NUMBER=" + txtMobileNo + Environment.NewLine);
                _reqStr.Append("AMOUNT=" + amount + Environment.NewLine);
                _reqStr.Append("AMOUNT_ALL=" + amount + Environment.NewLine);
                _reqStr.Append("TERM_ID=" + CYBER_APCode + Environment.NewLine);//Mostly value of TERM_ID=AP Code, but value may be vary.
                _reqStr.Append("COMMENT=test");
                #endregion
                return _reqStr.ToString();
            }
            public static string _strStatusMobilePostpaidRequestLogs(string txtMobileNo, string account, string authenticator3, string amount)
            {
                StringBuilder _reqStr = new StringBuilder();
                #region Create Request
                //_reqStr.Append("CERT=" + txtCERTNo.Text + Environment.NewLine);
                _reqStr.Append("CERT=" + CERTNO + Environment.NewLine);
                _reqStr.Append("SD=" + CYBER_SDCode + Environment.NewLine);
                _reqStr.Append("AP=" + CYBER_APCode + Environment.NewLine);
                _reqStr.Append("OP=" + CYBER_OPCode + Environment.NewLine);
                _reqStr.Append("SESSION=" + SessionNo + Environment.NewLine);
                #endregion
                return _reqStr.ToString();
            }
        }        
        public static class CyberPlatMobilePostpaidNonBBPSRecharge
        {
            public static string SessionNo = DateTime.Parse(DateTime.Now.ToString()).ToString("dddMMMddyyyyHHmmss");
            public static string CERTNO = System.Configuration.ConfigurationManager.AppSettings["CyberPlateCERTNo"];
            public static string CYBER_SDCode = System.Configuration.ConfigurationManager.AppSettings["CyberPlateSDCode"];
            public static string CYBER_APCode = System.Configuration.ConfigurationManager.AppSettings["CyberPlateAPCode"];
            public static string CYBER_OPCode = System.Configuration.ConfigurationManager.AppSettings["CyberPlateOPCode"];
            //public static string _strValidationPrepaidMobileRequest(string txtMobileNo, string account, string authenticator3, string amount)
            public static string _strValidationPostpaidNonBBPSRequest(string txtMobileNo, string amount)
            {
                StringBuilder _reqStr = new StringBuilder();
                #region Create Request
                //_reqStr.Append("CERT=" + txtCERTNo.Text + Environment.NewLine);
                _reqStr.Append("CERT=" + CERTNO + Environment.NewLine);
                _reqStr.Append("SD=" + CYBER_SDCode + Environment.NewLine);
                _reqStr.Append("AP=" + CYBER_APCode + Environment.NewLine);
                _reqStr.Append("OP=" + CYBER_OPCode + Environment.NewLine);
                _reqStr.Append("SESSION=" + SessionNo + Environment.NewLine);
                _reqStr.Append("NUMBER=" + txtMobileNo + Environment.NewLine);
                _reqStr.Append("AMOUNT=" + amount + Environment.NewLine);
                _reqStr.Append("AMOUNT_ALL=" + amount + Environment.NewLine);
                _reqStr.Append("TERM_ID=" + CYBER_APCode + Environment.NewLine);//Mostly value of TERM_ID=AP Code, but value may be vary.
                _reqStr.Append("COMMENT=test");
                #endregion
                return _reqStr.ToString();
            }
            public static string _strPaymentPostpaidNonBBPSRequest(string txtMobileNo, string amount)
            {
                StringBuilder _reqStr = new StringBuilder();
                #region Create Request
                //_reqStr.Append("CERT=" + txtCERTNo.Text + Environment.NewLine);
                _reqStr.Append("CERT=" + CERTNO + Environment.NewLine);
                _reqStr.Append("SD=" + CYBER_SDCode + Environment.NewLine);
                _reqStr.Append("AP=" + CYBER_APCode + Environment.NewLine);
                _reqStr.Append("OP=" + CYBER_OPCode + Environment.NewLine);
                _reqStr.Append("SESSION=" + SessionNo + Environment.NewLine);
                _reqStr.Append("NUMBER=" + txtMobileNo + Environment.NewLine);
                _reqStr.Append("AMOUNT=" + amount + Environment.NewLine);
                _reqStr.Append("AMOUNT_ALL=" + amount + Environment.NewLine);
                _reqStr.Append("TERM_ID=" + CYBER_APCode + Environment.NewLine);//Mostly value of TERM_ID=AP Code, but value may be vary.
                _reqStr.Append("COMMENT=test");
                #endregion
                return _reqStr.ToString();
            }
            public static string _strStatusPostpaidNonBBPSMobileRequestLogs(string txtMobileNo, string account, string authenticator3, string amount)
            {
                StringBuilder _reqStr = new StringBuilder();
                #region Create Request
                //_reqStr.Append("CERT=" + txtCERTNo.Text + Environment.NewLine);
                _reqStr.Append("CERT=" + CERTNO + Environment.NewLine);
                _reqStr.Append("SD=" + CYBER_SDCode + Environment.NewLine);
                _reqStr.Append("AP=" + CYBER_APCode + Environment.NewLine);
                _reqStr.Append("OP=" + CYBER_OPCode + Environment.NewLine);
                _reqStr.Append("SESSION=" + SessionNo + Environment.NewLine);
                #endregion
                return _reqStr.ToString();
            }
        }
        public static class CyberPlatBBPSBillUtilityRecharge
        {
            public static string SessionNo = DateTime.Parse(DateTime.Now.ToString()).ToString("dddMMMddyyyyHHmmss");
            public static string CERTNO = System.Configuration.ConfigurationManager.AppSettings["CyberPlateCERTNo"];
            public static string CYBER_SDCode = System.Configuration.ConfigurationManager.AppSettings["CyberPlateSDCode"];
            public static string CYBER_APCode = System.Configuration.ConfigurationManager.AppSettings["CyberPlateAPCode"];
            public static string CYBER_OPCode = System.Configuration.ConfigurationManager.AppSettings["CyberPlateOPCode"];
            //public static string _strValidationPrepaidMobileRequest(string txtMobileNo, string account, string authenticator3, string amount)
            public static string _strValidationBBPSFetchBillUtility(string SessionId,string txtMobileNo,string ACCOUNT, string amount, string AgentId, string fName, string lName, string Email, string benMobile, string GeoCode, string Pin, string TERMINAL)
            {
                StringBuilder _reqStr = new StringBuilder();
                #region Create Request
                //_reqStr.Append("CERT=" + txtCERTNo.Text + Environment.NewLine);
                _reqStr.Append("CERT=" + CERTNO + Environment.NewLine);
                _reqStr.Append("SD=" + CYBER_SDCode + Environment.NewLine);
                _reqStr.Append("AP=" + CYBER_APCode + Environment.NewLine);
                _reqStr.Append("OP=" + CYBER_OPCode + Environment.NewLine);
                _reqStr.Append("SESSION=" + SessionId + Environment.NewLine);
                _reqStr.Append("AgentId=" + AgentId + Environment.NewLine);
                _reqStr.Append("NUMBER=" + txtMobileNo + Environment.NewLine);
                _reqStr.Append("ACCOUNT=" + ACCOUNT + Environment.NewLine);
                _reqStr.Append("Authenticator3=" + "NA" + Environment.NewLine);
                _reqStr.Append("fName=" + fName + Environment.NewLine);
                _reqStr.Append("lName=" + lName + Environment.NewLine);
                _reqStr.Append("benMobile=" + benMobile + Environment.NewLine);
                _reqStr.Append("Email=" + Email + Environment.NewLine);
                _reqStr.Append("Channel=" + "Agent" + Environment.NewLine);
                _reqStr.Append("PaymentMethod=" + "Cash" + Environment.NewLine);
                _reqStr.Append("GeoCode=" + GeoCode + Environment.NewLine);
                _reqStr.Append("Pin=" + Pin + Environment.NewLine);
                //_reqStr.Append("PanCardNo=" + "NA" + Environment.NewLine);
                //_reqStr.Append("Aadhar=" + "NA" + Environment.NewLine);
                //_reqStr.Append("CardType=" + "NA" + Environment.NewLine);
                _reqStr.Append("TERMINAL_ID=" + TERMINAL + Environment.NewLine);
                _reqStr.Append("AMOUNT=" + amount + Environment.NewLine);
                _reqStr.Append("AMOUNT_ALL=" + amount + Environment.NewLine);
                _reqStr.Append("TERM_ID=" + CYBER_APCode + Environment.NewLine);//Mostly value of TERM_ID=AP Code, but value may be vary.
                _reqStr.Append("COMMENT=test");
                #endregion
                return _reqStr.ToString();
            }
            public static string _strValidationBBPSBillUtilityRequest(string txtMobileNo, string amount, string AgentId, string fName, string lName, string Email, string benMobile, string GeoCode, string Pin, string TERMINAL)
            {
                StringBuilder _reqStr = new StringBuilder();
                #region Create Request
                //_reqStr.Append("CERT=" + txtCERTNo.Text + Environment.NewLine);
                _reqStr.Append("CERT=" + CERTNO + Environment.NewLine);
                _reqStr.Append("SD=" + CYBER_SDCode + Environment.NewLine);
                _reqStr.Append("AP=" + CYBER_APCode + Environment.NewLine);
                _reqStr.Append("OP=" + CYBER_OPCode + Environment.NewLine);
                _reqStr.Append("SESSION=" + SessionNo + Environment.NewLine);
                _reqStr.Append("AgentId=" + AgentId + Environment.NewLine);
                _reqStr.Append("Channel=" + "AGT" + Environment.NewLine);
                _reqStr.Append("fName=" + fName + Environment.NewLine);
                _reqStr.Append("lName=" + lName + Environment.NewLine);
                _reqStr.Append("PanCardNo=" + "NA" + Environment.NewLine);
                _reqStr.Append("Aadhar=" + "NA" + Environment.NewLine);
                _reqStr.Append("CardType=" + "NA" + Environment.NewLine);
                _reqStr.Append("Email=" + Email + Environment.NewLine);
                _reqStr.Append("benMobile=" + benMobile + Environment.NewLine);
                _reqStr.Append("GeoCode=" + GeoCode + Environment.NewLine);
                _reqStr.Append("Pin=" + Pin + Environment.NewLine);
                _reqStr.Append("TERMINAL_ID=" + TERMINAL + Environment.NewLine);
                _reqStr.Append("NUMBER=" + txtMobileNo + Environment.NewLine);
                _reqStr.Append("AMOUNT=" + amount + Environment.NewLine);
                _reqStr.Append("AMOUNT_ALL=" + amount + Environment.NewLine);
                _reqStr.Append("TERM_ID=" + CYBER_APCode + Environment.NewLine);//Mostly value of TERM_ID=AP Code, but value may be vary.
                _reqStr.Append("COMMENT=test");
                #endregion
                return _reqStr.ToString();
            }
            public static string _strPaymentBBPSBillUtilityRequest(string txtMobileNo, string amount)
            {
                StringBuilder _reqStr = new StringBuilder();
                #region Create Request
                //_reqStr.Append("CERT=" + txtCERTNo.Text + Environment.NewLine);
                _reqStr.Append("CERT=" + CERTNO + Environment.NewLine);
                _reqStr.Append("SD=" + CYBER_SDCode + Environment.NewLine);
                _reqStr.Append("AP=" + CYBER_APCode + Environment.NewLine);
                _reqStr.Append("OP=" + CYBER_OPCode + Environment.NewLine);
                _reqStr.Append("SESSION=" + SessionNo + Environment.NewLine);
                _reqStr.Append("NUMBER=" + txtMobileNo + Environment.NewLine);
                _reqStr.Append("AMOUNT=" + amount + Environment.NewLine);
                _reqStr.Append("AMOUNT_ALL=" + amount + Environment.NewLine);
                _reqStr.Append("TERM_ID=" + CYBER_APCode + Environment.NewLine);//Mostly value of TERM_ID=AP Code, but value may be vary.
                _reqStr.Append("COMMENT=test");
                #endregion
                return _reqStr.ToString();
            }
            public static string _strStatusBBPSBillUtilityRequestLogs(string txtMobileNo, string account, string authenticator3, string amount)
            {
                StringBuilder _reqStr = new StringBuilder();
                #region Create Request
                //_reqStr.Append("CERT=" + txtCERTNo.Text + Environment.NewLine);
                _reqStr.Append("CERT=" + CERTNO + Environment.NewLine);
                _reqStr.Append("SD=" + CYBER_SDCode + Environment.NewLine);
                _reqStr.Append("AP=" + CYBER_APCode + Environment.NewLine);
                _reqStr.Append("OP=" + CYBER_OPCode + Environment.NewLine);
                _reqStr.Append("SESSION=" + SessionNo + Environment.NewLine);
                #endregion
                return _reqStr.ToString();
            }
        }

        public static class CyberPlatDMTAPICall
        {
            //public static string SessionNo = DateTime.Parse(DateTime.Now.ToString()).ToString("dddMMMddyyyyHHmmss");
            public static string CERTNO = System.Configuration.ConfigurationManager.AppSettings["CyberPlateCERTNo"];
            public static string CYBER_SDCode = System.Configuration.ConfigurationManager.AppSettings["CyberPlateSDCode"];
            public static string CYBER_APCode = System.Configuration.ConfigurationManager.AppSettings["CyberPlateAPCode"];
            public static string CYBER_OPCode = System.Configuration.ConfigurationManager.AppSettings["CyberPlateOPCode"];
            public static string _strGetRemitterDetails(string SessionNo, string Type, string NUMBER,string AMOUNT)
            {
                StringBuilder _reqStr = new StringBuilder();
                #region Create Request                
                _reqStr.Append("CERT=" + CERTNO + Environment.NewLine);
                _reqStr.Append("SD=" + CYBER_SDCode + Environment.NewLine);
                _reqStr.Append("AP=" + CYBER_APCode + Environment.NewLine);
                _reqStr.Append("OP=" + CYBER_OPCode + Environment.NewLine);
                _reqStr.Append("SESSION=" + SessionNo + Environment.NewLine);
                _reqStr.Append("Type=" + Type + Environment.NewLine);                
                _reqStr.Append("NUMBER=" + NUMBER + Environment.NewLine);
                _reqStr.Append("AMOUNT=" + AMOUNT + Environment.NewLine);//Mostly value of TERM_ID=AP Code, but value may be vary.
                _reqStr.Append("AMOUNT_ALL="+AMOUNT);
                #endregion
                return _reqStr.ToString();
            }
            public static string _strGetRemitterRegistration(string SessionNo, string Type, string NUMBER, string lName,string fName,string Pin, string AMOUNT,string AmountAll)
            {
                StringBuilder _reqStr = new StringBuilder();
                #region Create Request                
                _reqStr.Append("CERT=" + CERTNO + Environment.NewLine);
                _reqStr.Append("SD=" + CYBER_SDCode + Environment.NewLine);
                _reqStr.Append("AP=" + CYBER_APCode + Environment.NewLine);
                _reqStr.Append("OP=" + CYBER_OPCode + Environment.NewLine);
                _reqStr.Append("SESSION=" + SessionNo + Environment.NewLine);
                _reqStr.Append("Type=" + Type + Environment.NewLine);
                _reqStr.Append("NUMBER=" + NUMBER + Environment.NewLine);
                _reqStr.Append("lName=" + lName + Environment.NewLine);//Mostly value of TERM_ID=AP Code, but value may be vary.
                _reqStr.Append("fName=" + fName + Environment.NewLine);
                _reqStr.Append("Pin=" + Pin + Environment.NewLine);
                _reqStr.Append("AMOUNT=" + AMOUNT + Environment.NewLine);//Mostly value of TERM_ID=AP Code, but value may be vary.
                _reqStr.Append("AMOUNT_ALL=" + AmountAll);
                #endregion
                return _reqStr.ToString();
            }
            public static string _strGetRemitterRegistrationValidation(string SessionNo, string Type, string NUMBER, string remId, string otc, string AMOUNT)
            {
                StringBuilder _reqStr = new StringBuilder();
                #region Create Request                
                _reqStr.Append("CERT=" + CERTNO + Environment.NewLine);
                _reqStr.Append("SD=" + CYBER_SDCode + Environment.NewLine);
                _reqStr.Append("AP=" + CYBER_APCode + Environment.NewLine);
                _reqStr.Append("OP=" + CYBER_OPCode + Environment.NewLine);
                _reqStr.Append("SESSION=" + SessionNo + Environment.NewLine);
                _reqStr.Append("Type=" + Type + Environment.NewLine);
                _reqStr.Append("NUMBER=" + NUMBER + Environment.NewLine);
                _reqStr.Append("remId=" + remId + Environment.NewLine);//Mostly value of TERM_ID=AP Code, but value may be vary.
                _reqStr.Append("otc=" + otc + Environment.NewLine);                
                _reqStr.Append("AMOUNT=" + AMOUNT + Environment.NewLine);//Mostly value of TERM_ID=AP Code, but value may be vary.
                _reqStr.Append("AMOUNT_ALL=" + AMOUNT);
                #endregion
                return _reqStr.ToString();
            }

            public static string _strBeneficiaryRegistration(string SessionNo, string Type,string remId, string lName, string fName, string NUMBER, string benAccount, string benIFSC, string AMOUNT)
            {
                StringBuilder _reqStr = new StringBuilder();
                #region Create Request                
                _reqStr.Append("CERT=" + CERTNO + Environment.NewLine);
                _reqStr.Append("SD=" + CYBER_SDCode + Environment.NewLine);
                _reqStr.Append("AP=" + CYBER_APCode + Environment.NewLine);
                _reqStr.Append("OP=" + CYBER_OPCode + Environment.NewLine);
                _reqStr.Append("SESSION=" + SessionNo + Environment.NewLine);
                _reqStr.Append("Type=" + Type + Environment.NewLine);                
                _reqStr.Append("remId=" + remId + Environment.NewLine);//Mostly value of TERM_ID=AP Code, but value may be vary.
                _reqStr.Append("lName=" + lName + Environment.NewLine);
                _reqStr.Append("fName=" + fName + Environment.NewLine);
                _reqStr.Append("NUMBER=" + NUMBER + Environment.NewLine);
                _reqStr.Append("benAccount=" + benAccount + Environment.NewLine);
                _reqStr.Append("benIFSC=" + benIFSC + Environment.NewLine);                
                _reqStr.Append("AMOUNT=" + AMOUNT + Environment.NewLine);//Mostly value of TERM_ID=AP Code, but value may be vary.
                _reqStr.Append("AMOUNT_ALL=" + AMOUNT);
                #endregion
                return _reqStr.ToString();
            }

            public static string _strBeneficiaryAccountVerification(string SessionNo, string Type, string NUMBER, string benAccount, string benIFSC, string AMOUNT,string AMOUNT_ALL)
            {
                StringBuilder _reqStr = new StringBuilder();
                #region Create Request                
                _reqStr.Append("CERT=" + CERTNO + Environment.NewLine);
                _reqStr.Append("SD=" + CYBER_SDCode + Environment.NewLine);
                _reqStr.Append("AP=" + CYBER_APCode + Environment.NewLine);
                _reqStr.Append("OP=" + CYBER_OPCode + Environment.NewLine);
                _reqStr.Append("SESSION=" + SessionNo + Environment.NewLine);
                _reqStr.Append("Type=" + Type + Environment.NewLine);               
                _reqStr.Append("NUMBER=" + NUMBER + Environment.NewLine);
                _reqStr.Append("benAccount=" + benAccount + Environment.NewLine);
                _reqStr.Append("benIFSC=" + benIFSC + Environment.NewLine);
                _reqStr.Append("AMOUNT=" + AMOUNT + Environment.NewLine);//Mostly value of TERM_ID=AP Code, but value may be vary.
                _reqStr.Append("AMOUNT_ALL=" + AMOUNT_ALL);
                #endregion
                return _reqStr.ToString();
            }

            public static string _strFundTransfer(string SessionNo, string Type, string NUMBER, string routingType, string benId, string AMOUNT, string AMOUNT_ALL)
            {
                StringBuilder _reqStr = new StringBuilder();
                #region Create Request                
                _reqStr.Append("CERT=" + CERTNO + Environment.NewLine);
                _reqStr.Append("SD=" + CYBER_SDCode + Environment.NewLine);
                _reqStr.Append("AP=" + CYBER_APCode + Environment.NewLine);
                _reqStr.Append("OP=" + CYBER_OPCode + Environment.NewLine);
                _reqStr.Append("SESSION=" + SessionNo + Environment.NewLine);
                _reqStr.Append("Type=" + Type + Environment.NewLine);
                _reqStr.Append("NUMBER=" + NUMBER + Environment.NewLine);
                _reqStr.Append("routingType=" + routingType + Environment.NewLine);
                _reqStr.Append("benId=" + benId + Environment.NewLine);
                _reqStr.Append("AMOUNT=" + AMOUNT + Environment.NewLine);//Mostly value of TERM_ID=AP Code, but value may be vary.
                _reqStr.Append("AMOUNT_ALL=" + AMOUNT_ALL);
                #endregion
                return _reqStr.ToString();
            }


            public static string _strBeneficiaryDelete(string SessionNo, string Type, string remId,  string benId, string AMOUNT, string AMOUNT_ALL)
            {
                StringBuilder _reqStr = new StringBuilder();
                #region Create Request                
                _reqStr.Append("CERT=" + CERTNO + Environment.NewLine);
                _reqStr.Append("SD=" + CYBER_SDCode + Environment.NewLine);
                _reqStr.Append("AP=" + CYBER_APCode + Environment.NewLine);
                _reqStr.Append("OP=" + CYBER_OPCode + Environment.NewLine);
                _reqStr.Append("SESSION=" + SessionNo + Environment.NewLine);
                _reqStr.Append("Type=" + Type + Environment.NewLine);
                _reqStr.Append("remId=" + remId + Environment.NewLine);                
                _reqStr.Append("benId=" + benId + Environment.NewLine);
                _reqStr.Append("AMOUNT=" + AMOUNT + Environment.NewLine);//Mostly value of TERM_ID=AP Code, but value may be vary.
                _reqStr.Append("AMOUNT_ALL=" + AMOUNT_ALL);
                #endregion
                return _reqStr.ToString();
            }

            public static string _strBeneficiaryDeleteValidation(string SessionNo, string Type, string remId, string benId, string otc, string AMOUNT, string AMOUNT_ALL)
            {
                StringBuilder _reqStr = new StringBuilder();
                #region Create Request                
                _reqStr.Append("CERT=" + CERTNO + Environment.NewLine);
                _reqStr.Append("SD=" + CYBER_SDCode + Environment.NewLine);
                _reqStr.Append("AP=" + CYBER_APCode + Environment.NewLine);
                _reqStr.Append("OP=" + CYBER_OPCode + Environment.NewLine);
                _reqStr.Append("SESSION=" + SessionNo + Environment.NewLine);
                _reqStr.Append("Type=" + Type + Environment.NewLine);
                _reqStr.Append("remId=" + remId + Environment.NewLine);
                _reqStr.Append("benId=" + benId + Environment.NewLine);
                _reqStr.Append("otc=" + otc + Environment.NewLine);
                _reqStr.Append("AMOUNT=" + AMOUNT + Environment.NewLine);//Mostly value of TERM_ID=AP Code, but value may be vary.
                _reqStr.Append("AMOUNT_ALL=" + AMOUNT_ALL);
                #endregion
                return _reqStr.ToString();
            }
            public static string _strGetBankDetails(string SessionNo, string Type, string AMOUNT, string benAccount, string AMOUNT_ALL)
            {
                StringBuilder _reqStr = new StringBuilder();
                #region Create Request                
                _reqStr.Append("CERT=" + CERTNO + Environment.NewLine);
                _reqStr.Append("SD=" + CYBER_SDCode + Environment.NewLine);
                _reqStr.Append("AP=" + CYBER_APCode + Environment.NewLine);
                _reqStr.Append("OP=" + CYBER_OPCode + Environment.NewLine);
                _reqStr.Append("SESSION=" + SessionNo + Environment.NewLine);
                _reqStr.Append("Type=" + Type + Environment.NewLine);
                _reqStr.Append("benAccount=" + benAccount + Environment.NewLine);
                _reqStr.Append("AMOUNT=" + AMOUNT + Environment.NewLine);//Mostly value of TERM_ID=AP Code, but value may be vary.
                _reqStr.Append("AMOUNT_ALL=" + AMOUNT_ALL);
                #endregion
                return _reqStr.ToString();
            }
        }

    }
}