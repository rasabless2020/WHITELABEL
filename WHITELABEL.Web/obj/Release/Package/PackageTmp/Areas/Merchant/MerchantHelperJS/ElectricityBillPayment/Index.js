$(function () {
    $("#txtElectricityOperator").autocomplete({
        source: function (request, response) {
            var OperatorType = "ELECTRICITY";
            $.ajax({
                url: '/MerchantRechargeService/AutoElectricityBillService',
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
            $('#txtElectricityOperator').val(ui.item.label);
            return false;
        },
        select: function (e, i) {
            $("#hfElectricityperator").val(i.item.val);
            return true ;
        },
        minLength: 1
    }).data("ui-autocomplete")._renderItem = function (ul, item) {
        return $("<li></li>")
            .data("item.autocomplete", item)
            .append("<a>" + "<img style='width:40px;height:40px' src='" + item.image + "' /> " + item.label + "</a>")
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

//var GetElectricityBillDetails = function () {    
function GetElectricityBillDetails() {
    $('#divElecBillDisplay').show();
    $('#Electricityprogress').show();
    var _accountNo = $('#CustomerId').val();
    var _mobileNo = $('#MobileNo').val();
    var _geoLocation = $('#GeolocationElectricity').val();
    var Service_Key = $('#hfElectricityperator').val();
    var FirstName = $('#CustomerFirstName').val();
    var LastName = $('#CustomerLastName').val();
    var EmailId = $('#CUSTOMER_EMAIL_ID').val();
    var PinCode = $('#txtPin').val();
    var UnitNo = "";
    var CityName = $('#txtCityName').val();
    if (Service_Key == "MDE") {
        UnitNo = $('#BillUnitData').val();
    }
    else { UnitNo = ""; }
    $.ajax({
        type: "POST",
        url: "/Merchant/MerchantRechargeService/GetBillInformation",
        data: { AccountNo: _accountNo, MobileNo: _mobileNo, GeoLocation: _geoLocation, ServiceKey: Service_Key, Unitno: UnitNo, CityName: CityName, FName: FirstName, LName: LastName, EmailId: EmailId, PinNo: PinCode },
        success: function (data) {
            debugger;
            //if (data == "Unauthorized Access") {
            //    bootbox.alert({
            //        message: data,
            //        backdrop: true
            //    });
            //    $('#Electricityprogress').hide();
            //}
            //else {


            //var status = Electricitybill.statuscode;
            //if (status === "TXN") {                    
            //    //document.getElementById("RechargeAmt").innerHTML = JSON.stringify(Electricitybill.data.dueamount);
            //    var amount = Electricitybill.data.dueamount;
            //    var elec_Ref = Electricitybill.data.reference_id;
            //    $('#BillAMount').val(amount);
            //    $('#ELECreferenceID').val(elec_Ref);
            //    $('#DueDate').html(Electricitybill.data.duedate);
            //    $('#CustomerName').html(Electricitybill.data.customername);
            //    $('#txtElecbillnumber').html(Electricitybill.data.billnumber);
            //    $('#txtElectricityRefNo').val(Electricitybill.data.reference_id);
            //    $('#btnEleDisplay').hide();
            //    $('#btnsubmitElectricity').show();
            //    $('#Errormsg').html('');

            //}
            var result = data.Result;
            //if (typeof(status) !== "undefined" && status !== null) {
            if (result == '0') {
                var Electricitybill = JSON.parse(data.data);
                var BillerId = Electricitybill.billerid;
                var amount = Electricitybill.billlist[0].net_billamount;
                var elec_Ref = Electricitybill.billlist[0].billid;
                $('#BillAMount').val(amount);
                $('#ELECreferenceID').val(elec_Ref);
                $('#DueDate').html(Electricitybill.billlist[0].billduedate);
                $('#CustomerName').html(Electricitybill.billlist[0].customer_name);
                //$('#txtElecbillnumber').html(Electricitybill.billlist[0].billnumber);
                $('#txtElecbillnumber').html(BillerId);
                $('#txtElectricityRefNo').val(Electricitybill.billlist[0].billid);
                $('#btnEleDisplay').hide();
                $('#btnsubmitElectricity').show();
                $('#Errormsg').html('');

            }
            else {
                var msg = data.data;
                bootbox.alert({
                    message: msg,
                    backdrop: true
                });
                //$('#Errormsg').html(Electricitybill.status);
                $('#Errormsg').html(data);
            }
            $('#Electricityprogress').hide();
            //}
        }
    })
}