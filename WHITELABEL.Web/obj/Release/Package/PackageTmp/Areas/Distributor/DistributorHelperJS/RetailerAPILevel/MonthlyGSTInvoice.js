var app = angular.module('GetMonthlyGSTCalculationApp', [])
app.controller('GetMonthlyGSTCalculation', function ($scope, $http, $window, $location, $filter) {

    $scope.WHITELEVELNAME1 = null;
    $scope.SERVICEPROVIDER = null;
    $scope.SetGSTMonth = null;
    $scope.SetGSTYear = null;
    GetMonth();
    GetMonthlyServiceProvider();
    function GetMonth() {
        $http({
            url: '/DistributorMonthWiseGST/GetMonth_details',
            method: "POST",
        }).then(function (response) {
            $scope.WHITELEVELNAME = response.data;
        },
            function (response) {
                console.log(response.data);
            });
    }
    function GetMonthlyServiceProvider() {
        $http({
            url: '/DistributorMonthWiseGST/MonthGetServiceproviderdetails',
            method: "POST",
        }).then(function (response) {
            debugger;
            $scope.ServiceProviderList = response.data;
        },
            function (response) {
                console.log(response.data);
            });
    }

    $scope.CaculateMonthlyGST = function () {
        debugger;
        var MonthValue = $scope.SetGSTMonth;
        var ServiceName = $scope.SERVICEPROVIDER;
        var YearVal = $scope.SetGSTYear;
        $http({
            url: '/DistributorMonthWiseGST/MonthlyGSTCalculateAmount/',
            method: "POST",
            data: { 'MonthID': MonthValue, 'ServiceName': ServiceName,'Year': YearVal }
        }).then(function (response) {
            debugger;
            $scope.ServiceInformation = response.data;
            //console.log(response.data);
        },
            function (response) {
                console.log(response.data);
            });
    };
    $scope.formatDate = function (date) {
        var dateOut = new Date(date);
        return dateOut;
    };
});
