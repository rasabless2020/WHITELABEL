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
            url: '/AdminMonthlyGST/GetMonth_details',
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
            url: '/AdminMonthlyGST/MonthGetServiceproviderdetails',
            method: "POST",
        }).then(function (response) {
            $scope.ServiceProviderList = response.data;
        },
            function (response) {
                console.log(response.data);
            });
    }

    $scope.CaculateMonthlyGST = function () {
        var MonthValue = $scope.SetGSTMonth;
        var ServiceName = $scope.SERVICEPROVIDER;
        var YearVal = $scope.SetGSTYear;
        $http({
            url: '/AdminMonthlyGST/MonthlyGSTCalculateAmount/',
            method: "POST",
            data: { 'MonthID': MonthValue, 'ServiceName': ServiceName,'Year': YearVal }
        }).then(function (response) {
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
