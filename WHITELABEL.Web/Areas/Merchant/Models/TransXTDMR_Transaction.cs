namespace WHITELABEL.Web.Areas.Merchant.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;
    public class TransXTDMR_Transaction
    {
        [Required(ErrorMessage = "Recipient Code is required")]
        public string recSeqId { get; set; }
        [Required(ErrorMessage = "Customer Code is required")]
        public string customerId { get; set; }
        [Required(ErrorMessage = "Transfer Amount is required")]
        [MaxLength(10, ErrorMessage = "Amount not greater then 10 digit")]
        [MinLength(2, ErrorMessage = "Amount not less then 2 digit")]
        //[RegularExpression(@"^\d*(\.\d{1,4})?$", ErrorMessage = "Enter Valid Amount")]
        //[RegularExpression(@"^\d+(.\d{1,2})?$", ErrorMessage = "Enter Valid Amount")]
        [Range(1.00, 5000.00, ErrorMessage = "Please enter a Amount between 1.00 and 5000.00")]
        public string amount { get; set; }
        [Required(ErrorMessage = "Sender mobile no is required")]
        [MaxLength(15, ErrorMessage = "Sender mobile no is not greater then 15 digit")]
        [MinLength(10, ErrorMessage = "Sender mobile no is not less then 10 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile must be number")]
        public string SenderMobileNo { get; set; }
        [Required(ErrorMessage = "Sender Name is required")]
        [MaxLength(250, ErrorMessage = "Sender name is not greater then 255 digit")]
        [MinLength(5, ErrorMessage = "Sender name is not less then 5 digit")]
        public string SenderName { get; set; }
        [Required(ErrorMessage = "Recipient mobile no is required")]
        public string RecipientMobileNo { get; set; }
        [Required(ErrorMessage = "Recipient name is required")]
        public string RecipientName { get; set; }
        [Required(ErrorMessage = "Recipient account no is required")]
        public string RecipientAccountNo { get; set; }
        [Required(ErrorMessage = "Recipient ifsc code is required")]
        public string RecipientIFSCCode { get; set; }
        [NotMapped]
        public string geolocation { get; set; }
        [NotMapped]
        public string IpAddress { get; set; }
        [NotMapped]
        public string PaymentMode { get; set; }
        [NotMapped]
        public string BeneficiaryId { get; set; }
    }
    public class GetDMRCustomerInfo {
        [Required(ErrorMessage = "Customer mobile no is required")]
        [MaxLength(10, ErrorMessage = "Customer mobile no is not greater then 10 digit")]
        [MinLength(10, ErrorMessage = "Customer mobile no is not less then 10 digit")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Mobile must be number")]
        public string CustomerMobileNo { get; set; }
    }
    public class DMR_API_Response
    {
        public string serviceTax { get;set;}
        public string clientRefId { get; set; }
        public string fee { get; set; }
        public string initiatorId { get; set; }
        public string accountNumber { get; set; }
        public string txnStatus { get; set; }
        public string name { get; set; }
        public string ifscCode { get; set; }
        public string impsRespCode { get; set; }
        public string impsRespMessage { get; set; }
        public string txnId { get; set; }
        public string timestamp { get; set; }
    }

    public class PrintInvoice
    {
        public string CompanyLogo { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string MobileNo { get; set; }
        public string CustomerName { get; set; }
        public string CustomerMobileNo { get; set; }
        public string BeneficiaryName { get; set; }
        public string BeneficiaryMobile { get; set; }
        public string BeneficiaryAccountNo { get; set; }
        public string BeneficiaryIFSCCode { get; set; }
        public string BeneficiaryNameAmount { get; set; }
        public string BankName { get; set; }
        public decimal TransferAmount { get; set; }
        public string TransferMode { get; set; }
        public string TransactionStatus { get; set; }
        public string TransactionDate { get; set; }
        public string PowerBy { get; set; }
        public string GSTNo { get; set; }
        public string EmailID { get; set; }
        public string Website { get; set; }
        public string SenderName { get; set; }
        public string SenderMobile { get; set; }
        public string TransactionId  { get; set; }
        
    }

}