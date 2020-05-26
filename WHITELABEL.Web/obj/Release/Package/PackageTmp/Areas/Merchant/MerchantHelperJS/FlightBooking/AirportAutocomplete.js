var app = angular.module('AirportAutocompleteApp', ['angucomplete-alt'])
app.controller('AirportAutocompleteController', function ($scope, $http, $window, $location) {
    $scope.Airports = [];    
    $scope.SelectedAirport = null;
    $scope.AirportsName = '';
    $scope.ToAirportsName = '';
    $scope.AirportsCode = '';
    $scope.ToAirportsCode = '';
    $scope.ToAirportName = null;
    $scope.FromDate = '';
    $scope.ToDate = '';
    $scope.TravellType = '1';
    $scope.objListing =
                {                   
                    travels:
                    [
                        {                           
                            AirTravellers:
                            {
                                AirTotal: 1,
                                AirAdult: 1,
                                AirChildren: 0,                               
                                AirInfant: 0
                                
                            }
                        }
                    ],                    
                };
    $scope.formdisplay = true;

    //event fires when click on textbox  From AirportName
    $scope.SelectedAirport = function (selected) {
        if (selected) {
            $scope.SelectedAirport = selected.originalObject;
        }
    }
    // To airport Name
    $scope.ToAirportName = function (selected) {
        if (selected) {
            $scope.ToAirportName = selected.originalObject;
        }
    }
    //Gets data from the Database
    $http({
        method: 'GET',
        url: '/Merchant/MerchantFlightBooking/GetAllAirportName'
    }).then(function (data) {
        $scope.Airports = data.data;
    }, function () {
        alert('Error');
    })
    //Airport.TravellType
    
    $scope.SerachFlights = function () {
        $scope.formdisplay = true;
        var Tripmode = $scope.Airport.Tripmode;
        var FromAirportsName = $scope.SelectedAirport.CITYNAME; 
        var FromCityCode = $scope.SelectedAirport.CITYCODE;
        var TOAirportName = $scope.ToAirportName.CITYNAME;
        var TOAirportCode = $scope.ToAirportName.CITYCODE;
        var FromDate = $scope.FromDate;
        var ToDate = $scope.ToDate;        
        var TravelType = $scope.TravellType;
        var Adult = $scope.objListing.travels[0].AirTravellers.AirAdult;
        var Child = $scope.objListing.travels[0].AirTravellers.AirChildren;
        var Infant = $scope.objListing.travels[0].AirTravellers.AirInfant;
        
        
     
        var data = {
            Tripmode: Tripmode,
            FromAirportsName: FromAirportsName,
            FromCityCode: FromCityCode,
            TOAirportName: TOAirportName,
            TOAirportCode: TOAirportCode,
            FromDate: FromDate,
            ToDate: ToDate,
            TravelType: TravelType,
            Adult: Adult,
            Child: Child,
            Infant: Infant
        };
        $http({
            url: '/Merchant/MerchantFlightBooking/SerachFlight',
            method: "POST",
            data: data,
        }).then(function (response) {
            debugger;
            var searchresponse = JSON.parse(response.data);
            $scope.flightsearchResult = searchresponse.Response.Results;            
            $scope.formdisplay = false;
            console.log($scope.flightsearchResult);           
        },
            function (response) {
                console.log(response.data);               
            });
    };

    $scope.setDate = function () {
        $scope.FromDate = $("#datepickerFromDate").val();
    }
    $scope.setToDate = function () {
        $scope.ToDate = $("#datepickerToDate").val();
    }
    
});
app.directive('popoverpassengers', function ($compile) {
    return {
        restrict: 'A',
        link: function (scope, elem) {

            var content = $("#popover-content-popover-passengers").html();
            var compileContent = $compile(content)(scope);
            var options = {
                content: compileContent,
                html: true,
            };

            $(elem).popover(options);
        }
    }
})