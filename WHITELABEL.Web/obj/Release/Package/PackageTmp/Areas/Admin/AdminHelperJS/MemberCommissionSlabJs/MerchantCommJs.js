
var app = angular.module('CommissionSlabDistributionApp', [])
app.controller('CommissionSlabDistribution', function ($scope, $http, $window, $location) {
    $scope.SLN = 0;
    $scope.IsVisible = false;
    //$scope.SERVICE_NAME = null;
    //$scope.ID = null;
    //$scope.TYPE = null;
    //$scope.SERVICE_KEY = null;
    //$scope.HSN_SAC = null;
    //$scope.BILLING_MODEL = null;
    //$scope.CommissionPercentage = 0;
    $scope.buttondisplay = true;
    $scope.SLAB_NAME = null;
    $scope.SLAB_DETAILS = null;
    $scope.SLAB_TYPE = 0;
    $scope.SLAB_STATUS = null;
    $scope.SLAB_STATUS_Value = null;
    $scope.ServiceType = null;
    $scope.SLAB_TDS = "";
    $scope.OperaotrType = {
        ID: 0,
        SERVICE_NAME: '',
        TYPE: '',
        SERVICE_KEY: '',
        HSN_SAC: '',
        BILLING_MODEL: '',
        COMM_TYPE: null,
        CommissionPercentage: 0,
        DMRFrom: 0,
        DMRTo: 0,
        DMT_TYPE: null
    };
    $scope.ServiceInformationDMR = [
        {
            ID: 1,
            SERVICE_NAME: 'Money Transfer (Domestic)',
            TYPE: 'REMITTANCE',
            SERVICE_KEY: 'DMI',
            COMM_TYPE: "FIXED",
            CommissionPercentage: 0,
            DMRFrom: 1,
            DMRTo: 0,
            DMT_TYPE: "Domestic"
        }
        ,
        {
            ID: 2,
            SERVICE_NAME: 'Money Transfer (Domestic)',
            TYPE: 'REMITTANCE',
            SERVICE_KEY: 'DMI',
            COMM_TYPE: "FIXED",
            CommissionPercentage: 0,
            DMRFrom: 0,
            DMRTo: 0,
            DMT_TYPE: "Domestic"
        },
        {
            ID: 3,
            SERVICE_NAME: 'Money Transfer (Domestic)',
            TYPE: 'REMITTANCE',
            SERVICE_KEY: 'DMI',
            COMM_TYPE: "FIXED",
            CommissionPercentage: 0,
            DMRFrom: 0,
            DMRTo: 0,
            DMT_TYPE: "Domestic"
        }
    ];

    $scope.ServiceInformationInternational = [
        {
            ID: 1,
            SERVICE_NAME: 'Money Transfer (Nepal)',
            TYPE: 'REMITTANCE',
            SERVICE_KEY: 'PMT',
            COMM_TYPE: "FIXED",
            CommissionPercentage: 0,
            DMRFrom: 1,
            DMRTo: 0,
            DMT_TYPE: "Domestic"
        }
        //,
        //{
        //    ID: 2,
        //    SERVICE_NAME: 'Money Transfer (Nepal)',
        //    TYPE: 'REMITTANCE',
        //    SERVICE_KEY: 'PMT',
        //    COMM_TYPE: "FIXED",
        //    CommissionPercentage: 0,
        //    DMRFrom: 0,
        //    DMRTo: 0,
        //    DMT_TYPE: "Domestic"
        //},
        //{
        //    ID: 3,
        //    SERVICE_NAME: 'Money Transfer (Nepal)',
        //    TYPE: 'REMITTANCE',
        //    SERVICE_KEY: 'PMT',
        //    COMM_TYPE: "FIXED",
        //    CommissionPercentage: 0,
        //    DMRFrom: 0,
        //    DMRTo: 0,
        //    DMT_TYPE: "Domestic"
        //}
    ];
    $scope.updateDMRFromAmount = function (index) {
        $scope.ServiceInformationDMR[index + 1].DMRFrom = $scope.ServiceInformationDMR[index].DMRTo + 1;
    };
    $scope.updateDMRInternationalAmount = function (index) {
        $scope.ServiceInformationInternational[index + 1].DMRFrom = $scope.ServiceInformationInternational[index].DMRTo + 1;
    };


    $scope.FetchOperator = function () {

        //$scope.val = $scope.SLAB_TDS;
        $scope.buttondisplay = true;
        var ServiceName = '{NewListId: "' + $scope.ServiceType + '" }';
        $http({
            url: '/MemberCommissionSetting/GetServiceProvider/',
            method: "POST",
            data: { 'NewListId': $scope.ServiceType }
        }).then(function (response) {

            $scope.ServiceInformation = response.data;
            if (response.data.length > 0)
            { $scope.buttondisplay = false; }

            console.log(response.data);
            // success   
        },
            function (response) {
                console.log(response.data);
                // optional
                // failed
            });
    };

    $scope.SaveData = function () {
        debugger;
        var ServiceName = '{NewListId: "' + $scope.ServiceType + '" }';
        var SLAB_NAME = $scope.SLAB_NAME;
        var SLAB_DETAILS = $scope.SLAB_DETAILS;
        var ServiceType = $scope.ServiceType;
        var SLAB_STATUS = $scope.SLAB_STATUS;   //?"Active":"Inactive";
        var SLAB_STATUS = $scope.SLAB_STATUS == "Active" ? 1 : 0;
        var SLAB_TYPE = $scope.SLAB_TYPE;
        var SLAB_TDS = $scope.SLAB_TDS;
        var operatortype = document.getElementById("hftServiceType").value;
        OperaotrType = $scope.ServiceInformation;
        if (SLAB_NAME === null || SLAB_DETAILS === null || ServiceType === null) {
            return false;
        }
        $('#WL_progress').show();
        var data = {
            SLAB_NAME: SLAB_NAME,
            SLAB_DETAILS: SLAB_DETAILS,
            SLAB_TYPE: operatortype,
            Slab_TypeName: ServiceType,
            SLAB_STATUS: SLAB_STATUS,
            SLAB_TDS: SLAB_TDS,
            OperatorDetails: $scope.ServiceInformation[0].TYPE == "REMITTANCE" ? $scope.ServiceInformationDMR : OperaotrType,
            ServiceInformationDMR: $scope.ServiceInformationInternational
        };
        $http({
            url: '/MemberCommissionSetting/AddCommissionSlab/',
            method: "POST",
            data: data
        }).then(function (response) {
            $scope.ServiceInformation = response.data;
            $('#WL_progress').hide();
            debugger;
            if (response.data.Result === "Success") {
                bootbox.alert({
                    message: "Commission Inserted Successfully..",
                    callback: function () {
                        var URL = "/Admin/MemberCommissionSetting/Index";
                        $window.location.href = URL;
                        console.log('This was logged in the callback!');
                    }
                });
                //alert("Commission Inserted Successfully..");
                //var URL = "/PowerAdmin/MemberCommissionSetting/Index/";                
                //$window.location.href = URL;

            }
            else if (response.data.Result === "Updated") {
                bootbox.alert({
                    message: "Commission Updated Successfully..",
                    callback: function () {
                        var URL = "/Admin/MemberCommissionSetting/Index";
                        $window.location.href = URL;
                        console.log('This was logged in the callback!');
                    }
                });
                //alert("Commission Update Successfully..");
                //var URL = "/PowerAdmin/MemberCommissionSetting/Index/";                
                //$window.location.href = URL;
                //$location.path("/MemberCommissionSetting/Index?area=PowerAdmin");
            }
            else {
                //alert("Error Occured");
                bootbox.alert({
                    message: "Error Occured",
                    callback: function () {
                        var URL = "/Admin/MemberCommissionSetting/Index";
                        $window.location.href = URL;
                        console.log('This was logged in the callback!');
                    }
                });
            }

            // success   
        },
            function (response) {
                console.log(response.data);
                // optional
                // failed
            });
    };

    $scope.OnLoadDataBind = function () {

        //alert($scope.SLN);
        var ServiceName = '{idval: "' + $scope.SLN + '" }';
        $http({
            url: '/MemberCommissionSetting/FetchData/',
            method: "POST",
            data: { 'idval': $scope.SLN }
        }).then(function (response) {

            //alert(JSON.stringify(response));
            $scope.SLN = response.data[0].SLN;
            //alert(response.data[0].SLAB_NAME);
            $scope.SLAB_NAME = response.data[0].SLAB_NAME;
            $scope.SLAB_DETAILS = response.data[0].SLAB_DETAILS;
            $scope.SLAB_TYPE = response.data[0].SLAB_TYPE;
            //alert(response.data[0].SLAB_STATUS);
            $scope.SLAB_STATUS = response.data[0].SLAB_STATUS ? "Active" : "Inactive";
            $scope.SLAB_TDS = response.data[0].SLAB_TDS;
            //$scope.SLAB_STATUS = response.data[0].SLAB_STATUS;
            $scope.ServiceType = response.data[0].Slab_TypeName;
            $scope.ServiceInformation = response.data[0].OperatorDetails;
            $scope.ServiceInformationDMR = response.data[0].OperatorDetails;
            $scope.ServiceInformationInternational = response.data[0].ServiceInformationDMR;

            //alert(response);
            //console.log(response);
            // success   
        },
            function (response) {
                console.log(response.data);
                // optional
                // failed
            });
    };

    $scope.EditValue = function (oldVal, index) {
        var amt = $scope.ServiceInformation[index].CommissionPercentage;
        var isValid = $scope.checkDecimal(amt);
        if (isValid) {
            $scope.ServiceInformation[index].CommissionPercentage = amt;
        }
        else {
            $scope.ServiceInformation[index].CommissionPercentage = '';
        }
    };
    $scope.DMRDomesticEditValue = function (oldVal, index) {
        var amt = $scope.ServiceInformationDMR[index].CommissionPercentage;
        var isValid = $scope.checkDecimal(amt);
        if (isValid) {
            $scope.ServiceInformationDMR[index].CommissionPercentage = amt;
        }
        else {
            $scope.ServiceInformationDMR[index].CommissionPercentage = '';
        }

    };
    $scope.DMRInternationalEditValue = function (oldVal, index) {
        var amt = $scope.ServiceInformationInternational[index].CommissionPercentage;
        var isValid = $scope.checkDecimal(amt);
        if (isValid) {
            $scope.ServiceInformationInternational[index].CommissionPercentage = amt;
        }
        else {
            $scope.ServiceInformationInternational[index].CommissionPercentage = '';
        }
    };

    $scope.checkDecimal = function (el) {
        var ex = /^[0-9]+\.?[0-9]*$/;
        if (ex.test(el) == false) {
            return false;
        }
        else {
            return true;
        }
    }

});

