using System.Web;
using System.Web.Optimization;

namespace WHITELABEL.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/ApiLevel").Include(
                "~/HelperJS/ApiLevel/Index.js"));

            bundles.Add(new ScriptBundle("~/bundles/BankDetails").Include(
                "~/HelperJS/BankDetails/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/KYCVerification").Include(
               "~/HelperJS/KYCVerification/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/Service").Include(
               "~/HelperJS/Service/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/Requisition").Include(
              "~/HelperJS/Requisition/Index.js"));


            bundles.Add(new ScriptBundle("~/bundles/PowerAdminMember").Include(
              "~/Areas/PowerAdmin/PowerAdminHelperJS/PowerAdminMember/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/PowerAdminService").Include(
              "~/Areas/PowerAdmin/PowerAdminHelperJS/PowerAdminService/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/PowerAdminKYCVerification").Include(
              "~/Areas/PowerAdmin/PowerAdminHelperJS/PowerAdminKYCVerification/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/PowerAdminBank").Include(
              "~/Areas/PowerAdmin/PowerAdminHelperJS/PowerAdminBank/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/PowerAdminRequisition").Include(
              "~/Areas/PowerAdmin/PowerAdminHelperJS/PowerAdminRequisition/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/PowerAdminServiceMasterJs").Include(
              "~/Areas/PowerAdmin/PowerAdminHelperJS/PowerAdminServiceMasterJs/Index.js"));            
            bundles.Add(new ScriptBundle("~/bundles/PowerAdminSlabCommJsAutocomplete").Include(
              "~/Areas/PowerAdmin/PowerAdminHelperJS/PowerAdminSlabCommJs/ServiceTypeAutoComplete.js"));
            bundles.Add(new ScriptBundle("~/bundles/PowerAdminSlabCommJsDeactivateStatus").Include(
              "~/Areas/PowerAdmin/PowerAdminHelperJS/PowerAdminSlabCommJs/Index.js"));

            bundles.Add(new ScriptBundle("~/bundles/PowerAdminCommSlabSetting").Include(
              "~/Areas/PowerAdmin/PowerAdminHelperJS/PowerAdminCommSlabSetting/Index.js"));


            bundles.Add(new ScriptBundle("~/bundles/PowerAdminAutocompleteDomain").Include(
              "~/Areas/PowerAdmin/PowerAdminHelperJS/PowerAdminRequisition/PowerAdminAutocomplete.js"));

            //bundles.Add(new ScriptBundle("~/bundles/PowerAdminDMTBankMarginValue").Include(
            //  "~/Areas/PowerAdmin/PowerAdminHelperJS/PowerAdminDMTBankMargin/Index.js"));
			bundles.Add(new ScriptBundle("~/bundles/PowerAdminTaxMaster").Include(
			              "~/Areas/PowerAdmin/PowerAdminHelperJS/PoweradminTaxMaster/Index.js"));

            bundles.Add(new ScriptBundle("~/bundles/PowerAdminAPITransactionVerification").Include(
                          "~/Areas/PowerAdmin/PowerAdminHelperJS/APITransactionValidationJs/Index.js"));


            bundles.Add(new ScriptBundle("~/bundles/PowerAdminRailUtility").Include(
             "~/Areas/PowerAdmin/PowerAdminHelperJS/PowerAdminRailUTLJs/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/PowerAdminTaxMasterSetting").Include(
            "~/Areas/PowerAdmin/PowerAdminHelperJS/PowerAdminTaxSetting/Index.js"));


            bundles.Add(new ScriptBundle("~/bundles/MemberApilevel").Include(
              "~/Areas/Admin/AdminHelperJS/MemberApilevel/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/MemberService").Include(
              "~/Areas/Admin/AdminHelperJS/MemberService/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/MemberKYCVerification").Include(
              "~/Areas/Admin/AdminHelperJS/MemberKYCVerification/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/MemberBankDetails").Include(
              "~/Areas/Admin/AdminHelperJS/MemberBankDetails/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/MemberRequisition").Include(
              "~/Areas/Admin/AdminHelperJS/MemberRequisition/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/MemberCommissionTaggingJs").Include(
             "~/Areas/Admin/AdminHelperJS/MemberCommissionTaggingJs/Index.js"));

            bundles.Add(new ScriptBundle("~/bundles/MemberCommissionSlabDeactiveStatus").Include(
            "~/Areas/Admin/AdminHelperJS/MemberCommissionSlabJs/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/MemberCommissionSlabAutocompleteStatus").Include(
            "~/Areas/Admin/AdminHelperJS/MemberCommissionSlabJs/ServiceTypeAutoComplete.js"));

            bundles.Add(new ScriptBundle("~/bundles/MemberCommissionSlabSetting").Include(
            "~/Areas/Admin/AdminHelperJS/MemberCommSlabSetting/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/NeWMemberCommissionSlabAutocompleteJs").Include(
            "~/Areas/Admin/AdminHelperJS/MemberCommissionSlabJs/NewAutoCompleteProvider.js"));

            bundles.Add(new ScriptBundle("~/bundles/NeWMemberCommissionSlabSettingSettinigJs").Include(
           "~/Areas/Admin/AdminHelperJS/MemberCommSlabSetting/NewCommissionSlabSetting.js"));
            bundles.Add(new ScriptBundle("~/bundles/ChangeSuperDistributorID").Include(
           "~/Areas/Admin/AdminHelperJS/MemberChangeSuperID/Index.js"));

            bundles.Add(new ScriptBundle("~/bundles/CheckingEmailonRegistration").Include(
           "~/Areas/Admin/AdminHelperJS/MemberCheckEmail/Index.js"));





            bundles.Add(new ScriptBundle("~/bundles/SuperMemberfiles").Include(
            "~/Areas/Super/SuperHelperJS/SuperMember/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/SuperService").Include(
            "~/Areas/Super/SuperHelperJS/SuperService/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/SuperKYCVerification").Include(
            "~/Areas/Super/SuperHelperJS/SuperKYCVerification/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/SuperBankDetails").Include(
            "~/Areas/Super/SuperHelperJS/SuperBankDetails/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/SuperRequisition").Include(
            "~/Areas/Super/SuperHelperJS/SuperRequisition/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/SuperDstrCommissionSlabDeactivateJs").Include(
            "~/Areas/Super/SuperHelperJS/SuperDstrCommissionSlabJs/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/SuperDstrCommissionSlabsuperAutoCompleteJs").Include(
            "~/Areas/Super/SuperHelperJS/SuperDstrCommissionSlabJs/ServiceTypeAutoComplete.js"));

            bundles.Add(new ScriptBundle("~/bundles/SuperDstrCommSlabAddingSetting").Include(
            "~/Areas/Super/SuperHelperJS/SuperDstrCommSlabSetting/Index.js"));

            bundles.Add(new ScriptBundle("~/bundles/SuperCommissionTaggingSettingJs").Include(
            "~/Areas/Super/SuperHelperJS/SuperCommissionTaggingJs/Index.js"));

            bundles.Add(new ScriptBundle("~/bundles/CheckMerchantEmailOnSuperAdd").Include(
            "~/Areas/Super/SuperHelperJS/SuperAddMerchantEmailCheck/Index.js"));


            bundles.Add(new ScriptBundle("~/bundles/StockistAPILavel").Include(
             "~/Areas/SuperStockist/SuperStokistHelperJS/StockistAPILavel/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/StockistService").Include(
             "~/Areas/SuperStockist/SuperStokistHelperJS/StockistService/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/StockistBankDetails").Include(
             "~/Areas/SuperStockist/SuperStokistHelperJS/StockistBankDetails/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/StockistKYCVerification").Include(
             "~/Areas/SuperStockist/SuperStokistHelperJS/StockistKYCVerification/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/StockistRequisition").Include(
             "~/Areas/SuperStockist/SuperStokistHelperJS/StockistRequisition/Index.js"));



            bundles.Add(new ScriptBundle("~/bundles/NeWDistributorCommissionSlabAutocompleteJs").Include(
            "~/Areas/Distributor/DistributorHelperJS/DistributorCommissionSlabSetting/NewDistributorAutoCompleteJs.js"));

            bundles.Add(new ScriptBundle("~/bundles/RetailerAPILevel").Include(
             "~/Areas/Distributor/DistributorHelperJS/RetailerAPILevel/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/DistributorService").Include(
            "~/Areas/Distributor/DistributorHelperJS/DistributorService/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/DistributorKYCVerification").Include(
            "~/Areas/Distributor/DistributorHelperJS/DistributorKYCVerification/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/DistributorBankDetails").Include(
            "~/Areas/Distributor/DistributorHelperJS/DistributorBankDetails/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/DistributorRequisition").Include(
            "~/Areas/Distributor/DistributorHelperJS/DistributorRequisition/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/DistributorCommissionSlabDeactivateJs").Include(
           "~/Areas/Distributor/DistributorHelperJS/DistributorCommissionSlabJs/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/DistributorCommissionSlabAutocompleteJs").Include(
          "~/Areas/Distributor/DistributorHelperJS/DistributorCommissionSlabJs/ServiceTypeAutoComplete.js"));
            bundles.Add(new ScriptBundle("~/bundles/DistributorCommissionSlabSetting").Include(
         "~/Areas/Distributor/DistributorHelperJS/DistributorCommissionSlabSetting/Index.js"));

            bundles.Add(new ScriptBundle("~/bundles/DistributorCommissionTaggingSettingJs").Include(
        "~/Areas/Distributor/DistributorHelperJS/DistributorCommissionTaggingJs/Index.js"));


            // Merchant
            bundles.Add(new ScriptBundle("~/bundles/MerchantMobileRecharge").Include(
            "~/Areas/Merchant/MerchantHelperJS/MobileRecharge/MerchantMobileRecharge.js"));
            bundles.Add(new ScriptBundle("~/bundles/MerchantDTHRecharge").Include(
            "~/Areas/Merchant/MerchantHelperJS/DTHRecharge/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/MerchantLandlineRecharge").Include(
            "~/Areas/Merchant/MerchantHelperJS/LandlineRecharge/LandLineRecharge.js"));
            bundles.Add(new ScriptBundle("~/bundles/MerchantBroadbandRecharge").Include(
            "~/Areas/Merchant/MerchantHelperJS/BroadbandRecharge/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/MerchantElectricityRecharge").Include(
            "~/Areas/Merchant/MerchantHelperJS/ElectricityBillPayment/Index.js"));

            bundles.Add(new ScriptBundle("~/bundles/MerchantWaterSupplyRecharge").Include(
            "~/Areas/Merchant/MerchantHelperJS/WaterSuppyPayment/Index.js"));

            bundles.Add(new ScriptBundle("~/bundles/MerchantGasBillRecharge").Include(
            "~/Areas/Merchant/MerchantHelperJS/GasBillRecharge/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/MerchantInsurancePayment").Include(
            "~/Areas/Merchant/MerchantHelperJS/InsurancePayment/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/DeleteBeneficiary").Include(
            "~/Areas/Merchant/MerchantHelperJS/DeleteBeneficiary/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/VerifyBankAcocunt").Include(
            "~/Areas/Merchant/MerchantHelperJS/DeleteBeneficiary/VerifyBeneficiaryAccount.js"));

            bundles.Add(new ScriptBundle("~/bundles/MerchantRequisitionjs").Include(
            "~/Areas/Merchant/MerchantHelperJS/MerchantRequisition/Index.js"));

            bundles.Add(new ScriptBundle("~/bundles/PowerAdminDMTBankMarginJs").Include(
            "~/Areas/PowerAdmin/PowerAdminHelperJS/PowerAdminDMTBankMargin/Index.js"));


            bundles.Add(new ScriptBundle("~/bundles/MerchantDMRAPIAngularjsFiles").Include(
            "~/Areas/Merchant/MerchantHelperJS/TransXTAPI/Index.js"));
            bundles.Add(new ScriptBundle("~/bundles/MerchantDMRAPIJsFiles").Include(
            "~/Areas/Merchant/MerchantHelperJS/TransXTAPI/TransXTjsFile.js"));

            bundles.Add(new ScriptBundle("~/bundles/MerchantFingerPrint").Include(
            "~/Areas/Merchant/MerchantHelperJS/FingerPrinterJs/Index.js"));

            bundles.Add(new ScriptBundle("~/bundles/MerchantLeadLoanProcessing").Include(
            "~/Areas/Merchant/MerchantHelperJS/LeadLoan/Index.js"));

            bundles.Add(new ScriptBundle("~/bundles/ChannelLinkJsCall").Include(
              "~/Areas/Admin/AdminHelperJS/MemberApilevel/ChannelLinkJs.js"));


            BundleTable.EnableOptimizations = true;
            bundles.UseCdn = true;

        }
    }
}
