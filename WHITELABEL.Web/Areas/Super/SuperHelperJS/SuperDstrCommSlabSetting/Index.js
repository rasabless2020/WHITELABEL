
var app = angular.module('SuperCommissionSlabDistributionApp', [])
app.controller('SuperCommissionSlabDistribution', function ($scope, $http, $window, $location) {
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
    $scope.ServiceType = null;
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
        },
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
        },
        {
            ID: 2,
            SERVICE_NAME: 'Money Transfer (Nepal)',
            TYPE: 'REMITTANCE',
            SERVICE_KEY: 'PMT',
            COMM_TYPE: "FIXED",
            CommissionPercentage: 0,
            DMRFrom: 0,
            DMRTo: 0,
            DMT_TYPE: "Domestic"
        },
        {
            ID: 3,
            SERVICE_NAME: 'Money Transfer (Nepal)',
            TYPE: 'REMITTANCE',
            SERVICE_KEY: 'PMT',
            COMM_TYPE: "FIXED",
            CommissionPercentage: 0,
            DMRFrom: 0,
            DMRTo: 0,
            DMT_TYPE: "Domestic"
        }
    ];
    $scope.updateDMRFromAmount = function (index) {
        $scope.ServiceInformationDMR[index + 1].DMRFrom = $scope.ServiceInformationDMR[index].DMRTo + 1;
    };
    $scope.updateDMRInternationalAmount = function (index) {
        $scope.ServiceInformationInternational[index + 1].DMRFrom = $scope.ServiceInformationInternational[index].DMRTo + 1;
    };


    $scope.FetchOperator = function () {
        $scope.buttondisplay = true;
        var ServiceName = '{NewListId: "' + $scope.ServiceType + '" }';        
        $http({
            url: '/SuperCommissionSlab/GetServiceProvider/',
            method: "POST",
            data: { 'NewListId': $scope.ServiceType }
        }).then(function (response) {   
            if (response.data.length > 0)
            { $scope.buttondisplay = false; }    
            if ($scope.ServiceType === 'DMR') {
                $scope.OperatorDetails = response.data.OperatorDetails;
                $scope.ServiceInformationDMR = response.data.ServiceInformationDMR;
                $scope.buttondisplay = false; 
            }
            else {
                $scope.ServiceInformation = response.data;
            }
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
        
        var ServiceName = '{NewListId: "' + $scope.ServiceType + '" }';
        var SLAB_NAME = $scope.SLAB_NAME;
        var SLAB_DETAILS = $scope.SLAB_DETAILS;
        var ServiceType = $scope.ServiceType;
        //var SLAB_STATUS = $scope.SLAB_STATUS;
        var SLAB_STATUS = $scope.SLAB_STATUS == "Active" ? 1 : 0; 
        var SLAB_TYPE = $scope.SLAB_TYPE;
        var operatortype = document.getElementById("hftServiceType").value;
        if (SLAB_NAME === null || SLAB_DETAILS === null || ServiceType === null)
        {
            return false;
        }

        //OperaotrType = $scope.ServiceInformation;
        if ($scope.ServiceType === 'DMR') {
            OperaotrType = $scope.OperatorDetails;
        }
        else {
            OperaotrType = $scope.ServiceInformation;
        }
        $('#Super_progress').show();
        var data = {
            SLAB_NAME: SLAB_NAME,
            SLAB_DETAILS: SLAB_DETAILS,
            SLAB_TYPE: operatortype,
            Slab_TypeName: ServiceType,
            SLAB_STATUS: SLAB_STATUS,
            //OperatorDetails: $scope.ServiceInformation[0].TYPE == "REMITTANCE" ? $scope.ServiceInformationDMR : OperaotrType,
            OperatorDetails: OperaotrType,
            ServiceInformationDMR: $scope.ServiceInformationInternational
        };
        $http({
            url: '/Super/SuperCommissionSlab/AddCommissionSlab',
            method: "POST",
            data: data,
        }).then(function (response) {
            $scope.ServiceInformation = response.data;
            $('#Super_progress').hide();  
            if (response.data.Result === "Success") {
                bootbox.alert({
                    message: "Commission Inserted Successfully..",
                    callback: function () {
                        var URL = "/Super/SuperCommissionSlab/Index/";
                        $window.location.href = URL;
                        console.log('This was logged in the callback!');
                    }
                });
                //alert("Commission Inserted Successfully..");
                //var URL = "/Super/SuperCommissionSlab/Index/";                
                //$window.location.href = URL;
                
            }
            else if (response.data.Result === "Updated") {
                bootbox.alert({
                    message: "Commission Updated Successfully..",
                    callback: function () {
                        var URL = "/Super/SuperCommissionSlab/Index/";
                        $window.location.href = URL;
                        console.log('This was logged in the callback!');
                    }
                });
                //alert("Commission Update Successfully..");
                //var URL = "/Super/SuperCommissionSlab/Index/";                
                //$window.location.href = URL;                
            }
            else {
                bootbox.alert({
                    message: "Error Occured.",
                    callback: function () {
                        var URL = "/Super/SuperCommissionSlab/Index/";
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

    $scope.OnLoadDataBind = function () {
        //alert($scope.SLN);
        var ServiceName = '{idval: "' + $scope.SLN + '" }';
        $http({
            url: '/SuperCommissionSlab/FetchData/',
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
            //$scope.SLAB_STATUS = response.data[0].SLAB_STATUS;
            $scope.SLAB_STATUS = response.data[0].SLAB_STATUS ? "Active" : "Inactive";
            $scope.ServiceType = response.data[0].Slab_TypeName;
            
            if ($scope.ServiceType === 'DMR')
            {
            $scope.OperatorDetails = response.data[0].OperatorDetails;
            $scope.ServiceInformationDMR = response.data[0].ServiceInformationDMR;}
            else
            {
                $scope.ServiceInformation = response.data[0].OperatorDetails;
            }
            

            //alert(response);
            console.log(response);
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
        if (oldVal < amt) {
            $scope.ServiceInformation[index].CommissionPercentage = oldVal;
        }
    };
    $scope.DMRDomesticEditValue = function (oldVal, index) {

        var amt = $scope.OperatorDetails[index].CommissionPercentage;
        if (oldVal < amt) {
            $scope.OperatorDetails[index].CommissionPercentage = oldVal;
        }
    };
    $scope.DMRInternationalEditValue = function (oldVal, index) {

        var amt = $scope.ServiceInformationDMR[index].CommissionPercentage;
        if (oldVal < amt) {
            $scope.ServiceInformationDMR[index].CommissionPercentage = oldVal;
        }
    };

});

