var app = angular.module('GetGSTCalculationApp', [])
app.controller('GetGSTCalculation', function ($scope, $http, $window, $location, $filter) {

    $scope.WHITELEVELNAME1 = null;
    $scope.SERVICEPROVIDER = null;
    $scope.SetGSTMonth = null;
    $scope.SetGSTYear = null;
    GetMonth(); 
    GetServiceProvider();
    function GetMonth() {
        $http({
            url: '/SuperGSTCalculation/GetMonth_details',
            method: "POST",
        }).then(function (response) {
            $scope.WHITELEVELNAME = response.data;            
        },
            function (response) {
                console.log(response.data);
            });
    }
    function GetServiceProvider() {
        $http({
            url: '/SuperGSTCalculation/GetServiceproviderdetails',
            method: "POST",
        }).then(function (response) {
            $scope.ServiceProviderList = response.data;
        },
            function (response) {
                console.log(response.data);
            });
    }

    $scope.CaculateTDS = function () {
        var MonthValue = $scope.SetGSTMonth;
        var ServiceName = $scope.SERVICEPROVIDER;
        var YearVal = $scope.SetGSTYear;
        $http({
            url: '/SuperGSTCalculation/CalculateGSTAmount/',
            method: "POST",
            data: { 'MonthID': MonthValue, 'ServiceName': ServiceName, 'Year': YearVal }
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
