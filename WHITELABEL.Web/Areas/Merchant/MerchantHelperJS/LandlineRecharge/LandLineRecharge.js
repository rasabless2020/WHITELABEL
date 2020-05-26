$(function () {
    
    $("#txtLandlineOperator").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '/MerchantRechargeService/AutoLandlineRechargeComplete/',
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
            $('#txtLandlineOperator').val(ui.item.label);
            return false;
        },
        select: function (e, i) {
            $("#hfLandlineOperator").val(i.item.val);
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


function GetLandLINEBillDetails() {
    $('#divLandLineBillDisplay').show();
    $('#LandLineprogress').show();
   
    var _accountNo = $('#txtLandLineNumber').val();
    var _mobileNo = $('#txtLandlineMObileNo').val();
    var _geoLocation = $('#GeolocationLandline').val();
    var Service_Key = $('#hfLandlineOperator').val();
    var LandlineAmt = $('#txtLandlineAmt').val();
    var UnitNo = "";
    //var CityName = $('#txtCityName').val();
    var CityName = "";
    $.ajax({
        type: "POST",
        url: "/Merchant/MerchantRechargeService/GetLandlineBillInformation",
        data: { AccountNo: _accountNo, MobileNo: _mobileNo, GeoLocation: _geoLocation, ServiceKey: Service_Key, CityName: CityName, landlineAmt: LandlineAmt },
        success: function (data) {
            var LandLinebill = JSON.parse(data);
            debugger;
            var status = LandLinebill.statuscode;
            if (status === "TXN") {
                //document.getElementById("RechargeAmt").innerHTML = JSON.stringify(Electricitybill.data.dueamount);
                var amount = LandLinebill.data.dueamount;
                var elec_Ref = LandLinebill.data.reference_id;
                $('#txtLandlineAmt').val(amount);
                $('#LandlineCreferenceID').val(elec_Ref);
                $('#txtLandlineDueDate').html(LandLinebill.data.duedate);
                $('#txtLandlineCustomerName').html(LandLinebill.data.customername);
                $('#txtLandlineBillbillnumber').html(LandLinebill.data.billnumber);
                $('#txtLandlinecityRefNo').val(LandLinebill.data.reference_id);
                $('#btnLandlineDisplay').hide();
                $('#btnLandLinesubmit').show();
                $('#LandlineErrormsg').html('');
                $('#LandLineprogress').hide();
            }
            else {
                $('#LandlineErrormsg').html(LandLinebill.status);
            }
            $('#LandLineprogress').hide();

        }
    })
}



