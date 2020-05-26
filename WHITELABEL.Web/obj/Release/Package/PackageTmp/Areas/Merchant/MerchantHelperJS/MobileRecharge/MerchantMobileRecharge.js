$(function () {
   
    //$("#txtOperator").autocomplete({
    //    source: function (request, response) {
    //        var OperatorType = "";
    //        if (document.getElementById("PrepaidRecharge1").checked) {
    //            OperatorType = "Prepaid";
    //        }
    //        if (document.getElementById("PostpaidRecharge2").checked) {
    //            OperatorType = "POSTPAID";
    //        }
    //        $.ajax({
    //            url: '/MerchantRechargeService/AutoComplete/',
    //            data: "{ 'prefix': '" + request.term + "','OperatorType':'" + OperatorType + "'}",
    //            dataType: "json",
    //            type: "POST",
    //            contentType: "application/json; charset=utf-8",
    //            success: function (data) {
    //                response($.map(data, function (item) {
    //                    return item;
    //                }))
    //            },
    //            error: function (response) {
    //                alert(response.responseText);
    //            },
    //            failure: function (response) {
    //                alert(response.responseText);
    //            }
    //        });
    //    },
    //    select: function (e, i) {
    //        $("#hfOperator").val(i.item.val);
    //    },
    //    minLength: 1
    //});

    $("#txtOperator").autocomplete({
        source: function (request, response) {
            //var OperatorType = "";
            //if (document.getElementById("PrepaidRecharge1").checked) {
            //    OperatorType = "Prepaid";
            //}
            //if (document.getElementById("PostpaidRecharge2").checked) {
            //    OperatorType = "POSTPAID";
            //}
            var OperatorType = $('#txtPrepaidRecharge').val();
            $.ajax({
                url: '/MerchantRechargeService/AutoComplete/',
                data: "{ 'prefix': '" + request.term + "','OperatorType':'" + OperatorType + "'}",
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
            $('#txtOperator').val(ui.item.label);
            return true;
        },
        select: function (e, i) {
            $("#hfOperator").val(i.item.val);
            return true;
        },
        minLength: 1
    }).data("ui-autocomplete")._renderItem = function (ul, item) {
        return $("<li></li>")
            .data("item.autocomplete", item)
            .append("<a>" + "<img style='width:40px;height:40px' src='data:image;base64," + item.image + "' /> " + item.label + "</a>")
            .appendTo(ul);
    };
    


    $("#txtCircleName").autocomplete({
        source: function (request, response) {
            
            $.ajax({
                url: '/MerchantRechargeService/CircleName/',
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
        select: function (e, i) {
            $("#hfCircleCode").val(i.item.val);
        },
        minLength: 1
    });

});

$(document).ready(function () {
    $("#PrepaidRecharge1").change(function () {
        //debugger;
        $('#txtOperator').val('');
    });
    $("#PostpaidRecharge2").change(function () {

        $('#txtOperator').val('');
    });
});


function GetPostPaidBillDetails() {
    //GetPostPaidBillDetails = function () {
    $('#PostPaidDivElm').show();
    $('#progressMobile').show();    
    var _accountNo = $('#txtContactNo').val();
    var _mobileNo = $('#txtContactNo').val();
    var _geoLocation = $('#GeolocationMobile').val();
    var Service_Key = $('#hfOperator').val();
    var RechargeAmt = $('#txtRechargeAmt').val();
    $.ajax({
        type: "POST",
        url: "/Merchant/MerchantRechargeService/GetPostPaidBillInformation",
        data: { AccountNo: _accountNo, MobileNo: _mobileNo, GeoLocation: _geoLocation, ServiceKey: Service_Key, Amount: RechargeAmt },
        success: function (data) {
            var result = data.Result;
            if (result == '0') {
            }
            else {
                var msg = data.data;
                bootbox.alert({
                    message: msg,
                    backdrop: true
                });
                //$('#Errormsg').html(Electricitybill.status);
                $('#PostPaidErrormsg').html(data);
            }
            $('#progressMobile').hide();

            //var Electricitybill = JSON.parse(data);
            //if (Electricitybill != "Invalid outletid") {
            //    if (Electricitybill != "Invalid amount") {
            //        var status = Electricitybill.statuscode;
            //        var status = Electricitybill.statuscode;
            //        var statusvalue = Electricitybill.status;
            //        if (status === "TXN") {
            //            $('#btnsubmit').show();
            //            //document.getElementById("RechargeAmt").innerHTML = JSON.stringify(Electricitybill.data.dueamount);
            //            var amount = Electricitybill.data.dueamount;
            //            var Gas_Reff = Electricitybill.data.reference_id;
            //            $('#txtPostdueamount').text(amount);
            //            $('#txtpostpaidduedate').text(Electricitybill.data.duedate);
            //            $('#txtpostpaidbillNo').text(Electricitybill.data.billnumber);
            //            $('#txtPostpaidbilldate').text(Electricitybill.data.billdate);
            //            $('#txtPostpaidbillperiod').text(Electricitybill.data.billperiod);
            //            $('#txtpostpaidcustomerparamsdetails').text(Electricitybill.data.customerparamsdetails);
            //            $('#txtPostpaidcustomername').text(Electricitybill.data.customername);
            //            $('#hdnPostpaidreference_id').val(Electricitybill.data.reference_id);
            //            $('#progressMobile').hide();
            //            $('#btnDisplay').hide();
            //            $('#btnsubmit').show();
            //        }
            //        else {
            //            bootbox.alert({
            //                message: statusvalue,
            //                size: 'small',
            //                backdrop: true,
            //                callback: function () {
            //                    //var urlPath = "/Merchant/MerchantOutletRegistration/Index";
            //                    //window.location.href = urlPath;
            //                    console.log(Electricitybill);
            //                    $('#progressMobile').hide();
            //                    $('#PostPaidErrormsg').html(statusvalue);
            //                    $('#PostPaidDivElm').hide();
            //                }
            //            })
            //            $('#PostPaidErrormsg').html(statusvalue);
            //            $('#PostPaidDivElm').hide();
            //        }
            //    }
            //    else {
            //        bootbox.alert({
            //            message: Electricitybill,
            //            size: 'small',
            //            backdrop: true,
            //            callback: function () {
            //                //var urlPath = "/Merchant/MerchantOutletRegistration/Index";
            //                //window.location.href = urlPath;
            //                console.log(Electricitybill);
            //                $('#progressMobile').hide();
            //            }
            //        })
            //    }
            //    $('#progressMobile').hide();
            //}
            //else {
            //    bootbox.alert({
            //        message: "Please Create Outlet Id>This Outlet Id is Created from Landline Tab",
            //        size: 'small',
            //        backdrop: true,
            //        callback: function () {
            //            //var urlPath = "/Merchant/MerchantOutletRegistration/Index";
            //            //window.location.href = urlPath;
            //            console.log(Electricitybill);
            //            $('#progressMobile').hide();
            //        }
            //    })
            //}
            //$('#progressMobile').hide();

        }

    })
}

var ShowEmployee = function () {

    var OperatorType = $('#txtPrepaidRecharge').val();
    //var OperatorType = "";
    //if (document.getElementById("PrepaidRecharge1").checked) {
    //    OperatorType = "PREPAID";
    //}
    //if (document.getElementById("PostpaidRecharge2").checked) {
    //    OperatorType = "POSTPAID";
    //}
    var customerId = $("input[name='PrepaidRecharge']:checked").val();
    var url = "Merchant/MerchantRechargeService/CheckOperator?EmployeeId=" + customerId;
    $.ajax({

        type: "POST",
        url: "/Merchant/MerchantRechargeService/CheckOperator",
        data: { OperatorType: OperatorType },
        success: function (response) {
            $("#myModalBodyDiv").html(response);
            $("#myModal").modal("show");
        }
    })
}


var ShowJioPlan = function () {

    var ServiceName = $('#txtOperator').val();        
    var MobileNo = $('#txtContactNo').val();       
    var url = "Merchant/MerchantRechargeService/BrowseJioPlan?EmployeeId=" + ServiceName;
    $.ajax({

        type: "POST",
        url: "/Merchant/MerchantRechargeService/BrowseJioPlan",
        data: {
            ServiceName: ServiceName,
            MobileNo: MobileNo
        },
        success: function (response) {
            $("#myModalJioPlanList").html(response);
            $("#myJioPlanbrowseModal").modal("show");
        }
    })
}


function DisplayButton() {
    var PrepaidchkYes = document.getElementById("PrepaidRecharge1");
    var PostPaidchkYes = document.getElementById("PostpaidRecharge2");
    var dvPassport = document.getElementById("btnDisplay");
    if (PrepaidchkYes.checked) {
        $('#btnDisplay').hide();
    }
    else if (PostPaidchkYes.checked) {
        $('#btnDisplay').show();
        $('#btnsubmit').hide();
    }
    else {
        $('#btnsubmit').show();
    }
}
$(document).ready(function () {
    $("#txtPrepaidRecharge").change(function () {
        var OperatorType = $('#txtPrepaidRecharge').val();
        if (OperatorType == 'PREPAID') {
            $('#btnDisplay').hide();
            $('#btnsubmit').show();
        }
        else {
            $('#btnDisplay').show();
            $('#btnsubmit').hide();
        }
    });

    $("#transactionvalueid").click(function () {
        var radioValue = $("input[name='PrepaidRecharge']:checked").val();
        $.ajax({
            url: "/MerchantRechargeService/OpenAllProviderList?area=Merchant",
            data: {
                radioValue: radioValue
            },
            cache: false,
            type: "POST",
            dataType: "json",
            beforeSend: function () {
            },
            success: function (data) {
                if (data.Result === "true") {
                    //$('.mvc-grid').mvcgrid('reload');
                    //$(".overlaydiv").fadeOut("slow");
                    //bootbox.alert({
                    //    size: "small",
                    //    message: "Status changed successfully",
                    //    backdrop: true
                    //});
                    $('#transactmodal').trigger();
                }
                else {
                    //$(".overlaydiv").fadeOut("slow");
                    //bootbox.alert({
                    //    message: "there is some thing error",
                    //    backdrop: true
                    //});
                }
            },
            error: function (xhr, status, error) {
                console.log(status);
            }
        });



    });

});

