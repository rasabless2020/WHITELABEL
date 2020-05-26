var app = angular.module('CommissionSlabTagging', [])
app.controller('TagCommissionSlab', function ($scope, $http, $window, $location) {

    $scope.WHITELEVELNAME1 = null;
    $scope.MobileRechargeSlabdetails = null;
    $scope.UtilityRechargeSlabdetails = null;
    $scope.DMRRechargeSlabdetails = null;
    $scope.AIRSlabdetailsList = null;
    $scope.BusSlabdetailsList = null;
    $scope.HotelSlabdetailsList = null;
    $scope.CashCardSlabdetailsList = null;
    $scope.INTRODUCE_TO_ID = null;
    $scope.INTRODUCER_ID = null;

    



    $scope.ServiceInformation = {
        WHITELEVELNAME1: '',
        RechargeName: '',
        BillName: '',
        DMR_SLAB_Name: '',
        AIR_SLAB_Name: '',
        BUS_SLAB_Name: '',
        CASHCARD_SLAB_Name: '',
        HOTEL_SLAB_Name: '',
        SL_NO: 0,
        WHITE_LEVEL_ID: 0,
        IDVal:0,
    };

    GETWhiteLevelName();  
    GetMobilerechargeSlab();
    GetUtilityRecharge();
    GetDMRRecharge();
    GetAIRSlabDetails();
    GetBusSlabDetails();
    GetHotelSlabDetails();
    GetCashCardSlabDetails();
    function GETWhiteLevelName() {
        
        $http({
            url: '/PowerAdminCommissionTag/AutoComplete',
            method: "POST",
            }).then(function (response) {
                $scope.WHITELEVELNAME = response.data;
                //console.log(response);
                // success   
            },
            function (response) {
                console.log(response.data);
                // optional
                // failed
            });
    }

    function GetMobilerechargeSlab() {
        
        $http({
            url: '/PowerAdminCommissionTag/MobileRechargeSlab',
            method: "POST",
        }).then(function (response) {
            $scope.MobileRecharge = response.data;
            //console.log(response);
            // success   
        },
            function (response) {
                console.log(response.data);
                // optional
                // failed
            });
    };
    function GetUtilityRecharge() {
        
        $http({
            url: '/PowerAdminCommissionTag/UtilityRechargeSlab',
            method: "POST",
        }).then(function (response) {
            $scope.UtilityRecharge = response.data;
            //console.log(response);
            // success   
        },
            function (response) {
                console.log(response.data);
                // optional
                // failed
            });
    };
    function GetDMRRecharge() {
        
        $http({
            url: '/PowerAdminCommissionTag/DMRRechargeSlab',
            method: "POST",
        }).then(function (response) {
            $scope.DMRRecharge = response.data;
            //console.log(response);
            // success   
        },
            function (response) {
                console.log(response.data);
                // optional
                // failed
            });
    };

    function GetAIRSlabDetails() {
        
        $http({
            url: '/PowerAdminCommissionTag/AirSlabDetails',
            method: "POST",
        }).then(function (response) {
            $scope.AIRSlabDetails = response.data;
            //console.log(response);
            // success   
        },
            function (response) {
                console.log(response.data);
                // optional
                // failed
            });
    };
    function GetBusSlabDetails() {
        
        $http({
            url: '/PowerAdminCommissionTag/BusSlabDetails',
            method: "POST",
        }).then(function (response) {
            $scope.BusSlabDetails = response.data;
            //console.log(response);
            // success   
        },
            function (response) {
                console.log(response.data);
                // optional
                // failed
            });
    };
    function GetHotelSlabDetails() {
        
        $http({
            url: '/PowerAdminCommissionTag/HotelSlabDetails',
            method: "POST",
        }).then(function (response) {
            $scope.HotelSlabDetails = response.data;
            //console.log(response);
            // success   
        },
            function (response) {
                console.log(response.data);
                // optional
                // failed
            });
    };

    function GetCashCardSlabDetails() {
        
        $http({
            url: '/PowerAdminCommissionTag/CashCardSlabDetails',
            method: "POST",
        }).then(function (response) {
            $scope.CashCardSlabDetails = response.data;
            //console.log(response);
            // success   
        },
            function (response) {
                console.log(response.data);
                // optional
                // failed
            });
    };





    $scope.FetchOperator = function () {
        //var ServiceName = '{NewListId: "' + $scope.ServiceType + '" }';
        $http({
            url: '/PowerAdminCommissionTag/GetListInformation/',
            method: "POST",
            //data: { 'NewListId': $scope.ServiceType }
        }).then(function (response) {
            $scope.ServiceInformation = response.data;
            //console.log(response.data);
            // success   
        },
            function (response) {
                console.log(response.data);
                // optional
                // failed
            });
    };

    $scope.FetchOperatorByName = function (idval) {
        var ServiceName = '{Mem_Id: "' + $scope.WHITELEVELNAME1 + '" }';
        //alert($scope.WHITELEVELNAME1);
        $http({
            url: '/PowerAdminCommissionTag/GetMemberListInformation/',
            method: "POST",
            data: { 'Mem_Id': $scope.WHITELEVELNAME1 }
        }).then(function (response) {
            $scope.ServiceInformation = response.data;
            //console.log(response.data);
            // success   
        },
            function (response) {
                console.log(response.data);
                // optional
                // failed
            });
    };

    $scope.EditValue = function (index) {
        //debugger;
        $scope.ServiceInformation[index].EditMode = true;
        var idvalinfo = $scope.ServiceInformation[index].SL_NO;
        var WhitelevelID = $scope.ServiceInformation[index].WHITE_LEVEL_ID;
        
        $http({
            url: '/PowerAdminCommissionTag/fetchMemCommInfo/',
            method: "POST",
            data: { 'Mem_Id': idvalinfo, 'WhitelevelId': WhitelevelID}
        }).then(function (response) {  
            
            $scope.IDVal = "1";
            $scope.SL_NO = "" + response.data.SL_NO + "";
            $scope.WHITELEVELNAME1 = "" + response.data.WHITE_LEVEL_ID + "";
            $scope.MobileRechargeSlabdetails = "" + response.data.MobileRechargeSlabdetails + "";
            $scope.UtilityRechargeSlabdetails = "" + response.data.UtilityRechargeSlabdetails + "";
            $scope.DMRRechargeSlabdetails = "" + response.data.DMRRechargeSlabdetails + "";
            $scope.AIRSlabdetailsList = "" + response.data.AIRSlabdetailsList + "";
            $scope.BusSlabdetailsList = "" + response.data.BusSlabdetailsList + "";
            $scope.HotelSlabdetailsList = "" + response.data.HotelSlabdetailsList + "";
            $scope.CashCardSlabdetailsList = "" + response.data.CashCardSlabdetailsList + "";
            //console.log(response.data.WHITELEVELNAME1); 
            //console.log(response.data.MobileRechargeSlabdetails);
            //$scope.ServiceInformation = response.data;
            //console.log(response.data);
            // success   
        },
            function (response) {
                console.log(response.data);
                // optional
                // failed
            });
    };
    $scope.SaveData = function () {
        
        //var ServiceName = '{NewListId: "' + $scope.ServiceType + '" }';
        //var SLAB_NAME = $scope.SLAB_NAME;
        //var SLAB_DETAILS = $scope.SLAB_DETAILS;
        //var ServiceType = $scope.ServiceType;
        //var SLAB_STATUS = $scope.SLAB_STATUS;
        //var SLAB_TYPE = $scope.SLAB_TYPE;
        //var operatortype = document.getElementById("hftServiceType").value;
        //alert($scope.WHITELEVELNAME1);
        if ($scope.WHITELEVELNAME1 != null)
        { 
        var SL_NO = $scope.SL_NO;
        var mem_id = $scope.WHITELEVELNAME1;
        var MobileId = $scope.MobileRechargeSlabdetails;
        var Utilityid = $scope.UtilityRechargeSlabdetails;
        var DMRID = $scope.DMRRechargeSlabdetails;
        var AIRID = $scope.AIRSlabdetailsList;
        var BusID = $scope.BusSlabdetailsList;
        var HotelId = $scope.HotelSlabdetailsList;
        var CashCardId = $scope.CashCardSlabdetailsList;
        if (MobileId === null || Utilityid === null || DMRID === null) {
            bootbox.alert({
                message: "Provide Mobile/Utility/DMR Slabs..",
                backdrop: true
            });
            return false;
        }
        var Introid = 0;
        $('#PA_progress').show();
        OperaotrType = $scope.ServiceInformation;
        var data = {
            SL_NO: SL_NO,
            WHITE_LEVEL_ID: mem_id,
            INTRODUCER_ID: 0,
            INTRODUCE_TO_ID: 0,
            RECHARGE_SLAB: MobileId,
            BILLPAYMENT_SLAB: Utilityid,
            DMR_SLAB: DMRID,
            AIR_SLAB: AIRID,
            BUS_SLAB: BusID,
            HOTEL_SLAB: HotelId,
            CASHCARD_SLAB: CashCardId,

        };
        $http({
            url: '/PowerAdminCommissionTag/SaveCommissionSlab/',
            method: "POST",
            data: data
        }).then(function (response) {
            
            $scope.ServiceInformation = response.data;
            $('#PA_progress').hide(); 
            if (response.data.Result === "Success") {
                bootbox.alert({
                    message: "Commission Inserted Successfully..",
                    backdrop: true
                });
                var URL = "/PowerAdmin/PowerAdminCommissionTag/Index/";
                //$location.path();
                $window.location.href = URL;
                //$scope.reDirect("/PowerAdminCommissionSlab/Index?area=PowerAdmin");
            }
            else if (response.data.Result === "Updated") {
                bootbox.alert({
                    message: "Commission Updated Successfully..",
                    backdrop: true
                });
                var URL = "/PowerAdmin/PowerAdminCommissionTag/Index/";
                //$location.path();
                $window.location.href = URL;
                //$location.path("/PowerAdminCommissionSlab/Index?area=PowerAdmin");
            }         
            else {
                alert("Error Occured");
            }
            //console.log(response);
            // success   
        },
            function (response) {
                console.log(response.data);
                // optional
                // failed
            });
    }
    else
        {
            bootbox.alert({
                message: "Please Enter Valid Data..",
                backdrop: true,
                callback: function (result) {
                    //alert("Please Enter Valid Data..");
                    var URL = "/PowerAdmin/PowerAdminCommissionTag/Index/";
                    //$location.path();
                    $window.location.href = URL;
                }
            });

            
            
    }
    };

});
