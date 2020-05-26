$(function () {   
    $("#txtBroadbandOperator").autocomplete({
        source: function (request, response) {
            var OperatorType = "BROADBAND";            
            $.ajax({
                url: '/MerchantRechargeService/AutoBroadbandRechargeComplete/',
                data: "{ 'prefix': '" + request.term + "'}",
                dataType: "json",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    response($.map(data, function (item) {
                        return item;
                    }))
                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        },
        focus: function (event, ui) {
            // this is for when focus of autocomplete item 
            $('#txtBroadbandOperator').val(ui.item.label);
            return false;
        },
        select: function (e, i) {
            $("#hfbroadbandperator").val(i.item.val);
            return false;
        },
        minLength: 1
    }).data("ui-autocomplete")._renderItem = function (ul, item) {
        return $("<li></li>")
            .data("item.autocomplete", item)
            .append("<a>" + "<img style='width:40px;height:40px' src='data:image;base64," + item.image + "' /> " + item.label + "</a>")
            .appendTo(ul);
    };
});

//$(document).ready(function () {
//    $("#PrepaidRecharge1").change(function () {
//        debugger;
//        $('#txtOperator').val('');
//    });
//    $("#PostpaidRecharge2").change(function () {

//        $('#txtOperator').val('');
//    });
//});




//var GetBroadbandBillDetails = function () {
function GetBroadbandBillDetails() {
    $('#progressBroadband').show();
    $('#divBroadBandBillDisplay').show();    
    var _accountNo = $('#BroadbandBillAccountNo').val();
    var _mobileNo = $('#BroadbandMobileno').val();
    var _geoLocation = $('#GeolocationBroadband').val();
    var Service_Key = $('#hfbroadbandperator').val();
    var BroadBandAmt = $('#txtBroadbandAmount').val();
    $.ajax({
        type: "POST",
        url: "/Merchant/MerchantRechargeService/GetBROADBANDBillInformation",
        data: { AccountNo: _accountNo, MobileNo: _mobileNo, GeoLocation: _geoLocation, ServiceKey: Service_Key, landlineAmt: BroadBandAmt },
        success: function (data) {            
            var BroadBandbill = JSON.parse(data);
            var status = BroadBandbill.statuscode;
            var statusvalue = BroadBandbill.status;
            if (status === "TXN") {
                //document.getElementById("RechargeAmt").innerHTML = JSON.stringify(BroadBandbill.data.dueamount);
                var amount = BroadBandbill.data.dueamount;
                var Gas_Reff = BroadBandbill.data.reference_id;
                $('#txtBroadbandAmount').val(amount);
                $('#Broadband_referenceID').val(Gas_Reff);
                $('#txtbroadbandDueDate').html(BroadBandbill.data.duedate);
                $('#txtBroadbandCustomerName').html(BroadBandbill.data.customername);
                $('#txtBroadBandBillbillnumber').html(BroadBandbill.data.billnumber);
                $('#txtBroadBandcityRefNo').val(Gas_Reff);
                
                $('#btnsubmitbroadband').show();
                $('#btnBroadbandDisplay').hide();
                $('#broadbandErrormsg').html('');
                $('#progressBroadband').hide();
            }
            else {
                $('#broadbandErrormsg').html(statusvalue);
            }
            $('#progressBroadband').hide();
        }
    })
}