var app = angular.module('CouponCommissionMappingApp', []);
app.controller('CouponCommissionMappingController', ['$scope', '$http', '$window', '$location', function ($scope, $http, $window, $location) {
    $scope.SLN = "0";
    // GetMemberRole();

    $scope.IsVisible = false;
    //$scope.SERVICE_NAME = null;
    //$scope.ID = null;
    //$scope.TYPE = null;
    //$scope.SERVICE_KEY = null;
    //$scope.HSN_SAC = null;
    //$scope.BILLING_MODEL = null;
    //$scope.CommissionPercentage = 0;  
    $scope.buttondisplay = false;
    $scope.SLAB_NAME = null;
    $scope.SLAB_DETAILS = null;
    $scope.SLAB_TYPE = 0;
    $scope.SLAB_STATUS = null;
    $scope.ServiceType = null;
    $scope.TransactionRate = null;
    $scope.ClearData = function () {
        $scope.ServiceInformation = null;
    };
    //$scope.OperaotrType = {
    //    ID: 0,
    //    SERVICE_NAME: '',
    //    TYPE: '',
    //    SERVICE_KEY: '',
    //    HSN_SAC: '',
    //    BILLING_MODEL: '',
    //    COMM_TYPE: null,
    //    CommissionPercentage: "0",
    //    DMRFrom: 0,
    //    DMRTo: 0,
    //    DMT_TYPE: null
    //};
    $scope.OperaotrType = {
        sln:0,
        coupon_id: 0,
        Coupon_Name: '',
        Comm_TYPE: '',
        Comm_Value: '',
        Super_Comm_Value: 0,
        Dist_Comm_value: 0,
        Merchant_Comm_Value: 0
    };
    //$scope.ServiceInformationDMR = [
    //    {
    //        ID: 1,
    //        //SERVICE_NAME: 'Money Transfer (Domestic)',
    //        SERVICE_NAME: 'Label 1',
    //        TYPE: 'REMITTANCE',
    //        SERVICE_KEY: 'DMI',
    //        COMM_TYPE: "FIXED",
    //        CommissionPercentage: "0",
    //        DMRFrom: 1,
    //        DMRTo: 0,
    //        DMT_TYPE: "Domestic",
    //        // New Commission
    //        SuperDisPercentage: 0,
    //        DistributorComm_Type: "FIXED",
    //        DistriCommissionPer: 0,
    //        MerchantComm_Type: "FIXED",
    //        RetailerCommissionPer: 0,
    //    },
    //    {
    //        ID: 2,
    //        //SERVICE_NAME: 'Money Transfer (Domestic)',
    //        SERVICE_NAME: 'Label 2',
    //        TYPE: 'REMITTANCE',
    //        SERVICE_KEY: 'DMI',
    //        COMM_TYPE: "FIXED",
    //        CommissionPercentage: "0",
    //        DMRFrom: 0,
    //        DMRTo: 0,
    //        DMT_TYPE: "Domestic",
    //        // New Commission
    //        SuperDisPercentage: 0,
    //        DistributorComm_Type: "FIXED",
    //        DistriCommissionPer: 0,
    //        MerchantComm_Type: "FIXED",
    //        RetailerCommissionPer: 0,
    //    },
    //    {
    //        ID: 3,
    //        //SERVICE_NAME: 'Money Transfer (Domestic)',
    //        SERVICE_NAME: 'Label 3',
    //        TYPE: 'REMITTANCE',
    //        SERVICE_KEY: 'DMI',
    //        COMM_TYPE: "FIXED",
    //        CommissionPercentage: "0",
    //        DMRFrom: 0,
    //        DMRTo: 0,
    //        DMT_TYPE: "Domestic",
    //        // New Commission
    //        SuperDisPercentage: 0,
    //        DistributorComm_Type: "FIXED",
    //        DistriCommissionPer: 0,
    //        MerchantComm_Type: "FIXED",
    //        RetailerCommissionPer: 0,
    //    }
    //];
    //$scope.ServiceInformationInternational = [
    //    {
    //        ID: 1,
    //        SERVICE_NAME: 'Money Transfer (Nepal)',
    //        TYPE: 'REMITTANCE',
    //        SERVICE_KEY: 'PMT',
    //        COMM_TYPE: "FIXED",
    //        CommissionPercentage: 0,
    //        DMRFrom: 1,
    //        DMRTo: 0,
    //        DMT_TYPE: "Domestic"
    //    },
    //    {
    //        ID: 2,
    //        SERVICE_NAME: 'Money Transfer (Nepal)',
    //        TYPE: 'REMITTANCE',
    //        SERVICE_KEY: 'PMT',
    //        COMM_TYPE: "FIXED",
    //        CommissionPercentage: 0,
    //        DMRFrom: 0,
    //        DMRTo: 0,
    //        DMT_TYPE: "Domestic"
    //    },
    //    {
    //        ID: 3,
    //        SERVICE_NAME: 'Money Transfer (Nepal)',
    //        TYPE: 'REMITTANCE',
    //        SERVICE_KEY: 'PMT',
    //        COMM_TYPE: "FIXED",
    //        CommissionPercentage: 0,
    //        DMRFrom: 0,
    //        DMRTo: 0,
    //        DMT_TYPE: "Domestic"
    //    }
    //];
    //$scope.updateDMRFromAmount = function (index) {
    //    debugger;
    //    var total_percentage = $scope.CommissionPer;
    //    var toDMRAmt = $scope.ServiceInformationDMR[index].DMRTo;
    //    if (index < 2) {
    //        $scope.ServiceInformationDMR[index + 1].DMRFrom = $scope.ServiceInformationDMR[index].DMRTo + 1;
    //    }
    //    if (toDMRAmt == total_percentage) {
    //        $scope.buttondisplay = false;
    //    }
    //    else
    //    {
    //        //$scope.buttondisplay = true;
    //        $scope.buttondisplay = false;
    //    }
    //};
    //$scope.updateDMRInternationalAmount = function (index) {

    //    $scope.ServiceInformationInternational[index + 1].DMRFrom = $scope.ServiceInformationInternational[index].DMRTo + 1;
    //};
    

    $scope.FetchOperator = function (val) {
        debugger;
        $scope.buttondisplay = true;
        $scope.ServiceType = val;
        var ServiceName = { NewListId: val };
        $http({
            url: '/MemberCouponMaster/GetCouponCommProvider',
            method: "POST",
            data: ServiceName
        }).then(function (response) {
            debugger;
            $scope.ServiceInformation = response.data;
            console.log(response.data);
        },
            function (response) {
                console.log(response.data);
            });
    };
    //$scope.FetchOperator = function () {        
    //        $scope.buttondisplay = true;
    //    var ServiceName = '{NewListId: "' + $scope.ServiceType + '" }';
    //    $http({
    //        url: '/MemberCommissionSlab/GetServiceProvider/',
    //        method: "POST",
    //        data: { 'NewListId': $scope.ServiceType }
    //    }).then(function (response) {
    //        if (response.data.length > 0)
    //        { $scope.buttondisplay = false; }
    //        if ($scope.ServiceType === 'DMR') {
    //            $scope.OperatorDetails = response.data.OperatorDetails;
    //            $scope.ServiceInformationDMR = response.data.ServiceInformationDMR;
    //            $scope.buttondisplay = false;
    //        }
    //        else {
    //            $scope.ServiceInformation = response.data;
    //        }
    //        console.log(response.data);
    //        // success   
    //    },
    //        function (response) {
    //            console.log(response.data);
    //        });
    //};

    $scope.OnLoadDataBind = function () {
        debugger;
       
        var url = "/Admin/MemberCouponMaster/FetchCouponSlabData?idval=" + $scope.SLN;
            $http.get(url)
                .then(function (response) {
                    console.log(response);
                    $scope.SLN = response.data[0].SLN;
                    $scope.SLAB_NAME = response.data[0].SLAB_NAME;
                    $scope.SLAB_DETAILS = response.data[0].SLAB_DETAILS;
                    $scope.SLAB_TYPE = response.data[0].SLAB_TYPE;                    
                    $scope.SLAB_STATUS = response.data[0].SLAB_STATUS ? "Active" : "Inactive";
                    $scope.ServiceType = response.data[0].Slab_TypeName;
                    $scope.MemberRoleInfor = "" + response.data[0].ASSIGNED_SLAB + "";  
                        $scope.ServiceInformation = response.data[0].OperatorDetails;                    
                    console.log($scope.MemberRoleInfor);
                });
       
            
    };
    function GetMemberRole() {
        $http({
            url: '/Admin/MemberCommissionSlab/GetMemberRole',
            method: "POST",
        }).then(function (response) {
            $scope.MemberRoleDetails = response.data;
            //console.log(response);
            // success   
        },
            function (response) {
                console.log(response.data);
                // optional
                // failed
            });
    }

    //$scope.CheckSlabName = function () {
    //    debugger;
    //    var SlabName = $scope.SLAB_NAME;
    //    $http({
    //        url: '/Admin/MemberCommissionSlab/CheckDuplicateSlabName',
    //        method: "POST",
    //        data: { slabname: SlabName }
    //    }).then(function (response) {
    //        debugger;
    //        $scope.DuplicationCheck = response.data;
    //        var checkstatus = response.data;
    //        if (checkstatus == true) {
    //            bootbox.alert({
    //                size: "small",
    //                message: "Slab Already Available.",
    //                backdrop: true
    //            });
    //            $scope.SLAB_NAME = "";

    //        }
    //        console.log(response.data);
    //        //GetMemberRole();
    //        //console.log($scope.$MemberRoleInfor);
    //        //var member_role = $("#Memberrole").val();
    //        //$scope.MemberRoleInfor = member_role;
    //        // success   
    //    },
    //        function (response) {
    //            console.log(response.data);
    //            // optional
    //            // failed
    //        });
    //};

    $scope.SaveData = function () {
        
        var RolceMember = "";
        var ServiceType = $scope.ServiceType;
       
        var ServiceName = '{NewListId: "' + $scope.ServiceType + '" }';
        var SLAB_NAME = $scope.SLAB_NAME;
        var SLAB_DETAILS = $scope.SLAB_DETAILS;
       
        //var SLAB_STATUS = $scope.SLAB_STATUS;
        var SLAB_STATUS = $scope.SLAB_STATUS == "Active" ? 1 : 0;
        var SLAB_TYPE = $scope.SLAB_TYPE;
        var operatortype = document.getElementById("hftNewServiceType").value;
        if (SLAB_NAME === null || SLAB_DETAILS === null || ServiceType === null) {
            return false;
        }
        $('#WL_progress').show();
        
        OperaotrType = $scope.ServiceInformation;
        var data = {
            SLAB_NAME: SLAB_NAME,
            SLAB_DETAILS: SLAB_DETAILS,
            SLAB_TYPE: operatortype,
            Slab_TypeName: ServiceType,
            SLAB_STATUS: SLAB_STATUS,
            ASSIGNED_SLAB: RolceMember,
            OperatorDetails: OperaotrType            
        };
        $http({
            url: '/Admin/MemberCouponMaster/GenerateCouponCommissionSlab',
            method: "POST",
            data: data,
        }).then(function (response) {
            debugger;
            $scope.ServiceInformation = response.data;
            $('#WL_progress').hide();
            if (response.data.Result === "Success") {
                bootbox.alert({
                    size: "small",
                    message: "Commission Inserted Successfully..",
                    backdrop: true,
                    callback: function () {
                        var URL = "/Admin/MemberCouponMaster/CouponCommissionList/";
                        $window.location.href = URL;
                        console.log('This was logged in the callback!');
                    }
                });                
            }
            else if (response.data.Result === "Updated") {
                bootbox.alert({
                    size: "small",
                    message: "Commission Updated Successfully..",
                    backdrop: true,
                    callback: function () {
                        var URL = "/Admin/MemberCouponMaster/CouponCommissionList";
                        $window.location.href = URL;
                        console.log('This was logged in the callback!');
                    }
                });
                //alert("Commission Update Successfully..");
                //var URL = "/Admin/MemberCommissionSlab/Index/";                    
                //$window.location.href = URL;                    
            }
            else if (response.data.Result === "Failure") {
                bootbox.alert({
                    size: "small",
                    message: "Commission Percentage is not greater than predefined commission percentage.",
                    backdrop: true,
                    callback: function () {
                        var URL = "/Admin/MemberCouponMaster/CouponCommissionList";
                        $window.location.href = URL;
                        console.log('This was logged in the callback!');
                    }
                });
                //alert("Commission Percentage is not greater than predefined commission percentage.");
            }
            else if (response.data.Result==="FAILED")
            {
                bootbox.alert({
                    title: "Alert! Failed",
                    message: "One time maximum transaction value is Rs. 5000 (INR)",
                    backdrop: true
                });
                
            }
            else {
                //alert("Error Occured");
              
                bootbox.alert({
                    size: "small",
                    message: "Error Occured.",
                    backdrop: true,
                    callback: function () {
                        var URL = "/Admin/MemberCouponMaster/CouponCommissionList";
                        $window.location.href = URL;
                        console.log('This was logged in the callback!');
                    }
                });
            }
            console.log(response);
            // success   
        },
            function (response) {
                console.log(response.data);
                // optional
                // failed
            });
    };

    //$scope.CheckcommissionValidation = function (index, idval) {
    //    var total_percentage = $scope.ServiceInformation[index].CommissionPercentage;
    //    var supercomm = $scope.ServiceInformation[index].SuperDisPercentage;       
    //    var Distributorcomm = $scope.ServiceInformation[index].DistriCommissionPer;
    //    var Retailercomm = $scope.ServiceInformation[index].RetailerCommissionPer;
    //    var add_Percentage = parseFloat(supercomm) + parseFloat(Distributorcomm) + parseFloat(Retailercomm);
    //    if (parseFloat(add_Percentage) > parseFloat(total_percentage)) {
    //        bootbox.alert({
    //            size: "small",
    //            message: "Commission distributon is alway less or equal to " + total_percentage + "",
    //            backdrop: true
    //        });
    //        if (idval == 1) {
    //            $scope.ServiceInformation[index].SuperDisPercentage = 0;
    //        }
    //        else if (idval == 2) {
    //            $scope.ServiceInformation[index].DistriCommissionPer = 0;
    //        }
    //        else if (idval == 3) {
    //            $scope.ServiceInformation[index].RetailerCommissionPer = 0;
    //        }

    //    }
    //    //alert(supercomm + ',' + Distributorcomm + ',' + Retailercomm);
    //};


    //$scope.CheckDMRcommissionAmount = function (index, idval) {
     
    //    var total_percentage = $scope.CommissionPer;
    //    var total_commValue = total_percentage.replace(/"/g, "");
    //    $scope.CommissionPercentage = total_commValue;
    //    var supercomm = $scope.ServiceInformationDMR[index].SuperDisPercentage;
    //    var Distributorcomm = $scope.ServiceInformationDMR[index].DistriCommissionPer;
    //    var Retailercomm = $scope.ServiceInformationDMR[index].RetailerCommissionPer;
    //    var add_Percentage = parseFloat(supercomm) + parseFloat(Distributorcomm) + parseFloat(Retailercomm);
    //    if (parseFloat(add_Percentage) > parseFloat(total_commValue)) {
    //        bootbox.alert({
    //            size: "small",
    //            message: "Commission distributon is alway less or equal to " + total_percentage + "",
    //            backdrop: true
    //        });
    //        if (idval == 1) {
    //            $scope.ServiceInformation[index].SuperDisPercentage = 0;
    //        }
    //        else if (idval == 2) {
    //            $scope.ServiceInformation[index].DistriCommissionPer = 0;
    //        }
    //        else if (idval == 3) {
    //            $scope.ServiceInformation[index].RetailerCommissionPer = 0;
    //        }

    //    }
    //    //alert(supercomm + ',' + Distributorcomm + ',' + Retailercomm);
    //};

    //function GetCompanyDMRCommission() {
    //    debugger;
    //    $http({
    //        url: '/Admin/MemberCommissionSlab/GetDMRCommissionValue',
    //        method: "POST",
    //    }).then(function (response) {
    //        debugger;
    //        $scope.CommissionPer = response.data;
    //        //console.log(response);
    //        // success   
    //    },
    //        function (response) {
    //            console.log(response.data);
    //            console.log(CommissionPer);
    //            // optional
    //            // failed
    //        });
    //}


}]);

app.directive('ngDecimal', function () {
    return {
        restrict: 'A',
        link: function ($scope, $element, $attributes) {
            var limit = $attributes.ngDecimal;
            function caret(node) {
                if (node.selectionStart) {
                    return node.selectionStart;
                }
                else if (!document.selection) {
                    return 0;
                }
                //node.focus();
                var c = "\001";
                var sel = document.selection.createRange();
                var txt = sel.text;
                var dul = sel.duplicate();
                var len = 0;
                try { dul.moveToElementText(node); } catch (e) { return 0; }
                sel.text = txt + c;
                len = (dul.text.indexOf(c));
                sel.moveStart('character', -1);
                sel.text = "";
                return len;
            }
            $element.bind('keypress', function (event) {
                var charCode = (event.which) ? event.which : event.keyCode;
                var elem = document.getElementById($element.attr("id"));
                if (charCode == 45) {
                    var caretPosition = caret(elem);
                    if (caretPosition == 0) {
                        if ($element.val().charAt(0) != "-") {
                            if ($element.val() <= limit) {
                                $element.val("-" + $element.val());
                            }
                        }
                        if ($element.val().indexOf("-") != -1) {
                            event.preventDefault();
                            return false;
                        }
                    }
                    else {
                        event.preventDefault();
                    }
                }
                if (charCode == 46) {
                    if ($element.val().length > limit - 1) {
                        event.preventDefault();
                        return false;
                    }
                    if ($element.val().indexOf('.') != -1) {
                        event.preventDefault();
                        return false;
                    }
                    return true;
                }
                if (charCode != 45 && charCode != 46 && charCode > 31 && (charCode < 48 || charCode > 57)) {
                    event.preventDefault();
                    return false;
                }
                if ($element.val().length > limit - 1) {
                    event.preventDefault();
                    return false;
                }
                return true;
            });
        }
    };
});
