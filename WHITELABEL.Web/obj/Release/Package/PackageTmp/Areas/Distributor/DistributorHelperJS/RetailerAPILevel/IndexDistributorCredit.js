$(document).ready(function () {
    $('#chkMobile').prop('checked', false);
    $('#chkUtility').prop('checked', false);
    $('#chkDMT').prop('checked', false);
    $('#chkFlight').prop('checked', false);
    $('#chkRail').prop('checked', false);
    $('#chkBusBooking').prop('checked', false);
    $('#chkHotelBooking').prop('checked', false);
    $('#progressCreditLimitDistributor').removeClass("hidden");
    $('#progressCreditLimitDistributor').hide();
})

function SelectAll() {
    var CheckAllServices = document.getElementById("chkAllService");

    if (CheckAllServices.checked) {
        $('#chkMobile').prop('checked', true);
        $('#chkUtility').prop('checked', true);
        $('#chkDMT').prop('checked', true);
        $('#chkFlight').prop('checked', true);
        $('#chkRail').prop('checked', true);
        $('#chkBusBooking').prop('checked', true);
        $('#chkHotelBooking').prop('checked', true);
    }
    else {
        $('#chkMobile').prop('checked', false);
        $('#chkUtility').prop('checked', false);
        $('#chkDMT').prop('checked', false);
        $('#chkFlight').prop('checked', false);
        $('#chkRail').prop('checked', false);
        $('#chkBusBooking').prop('checked', false);
        $('#chkHotelBooking').prop('checked', false);
    }
    //dvPassport.style.display = PrepaidchkYes.checked ? "block" : "none";
}


$(document).ready(function () {
    $("#txtMemberaName").autocomplete({
        source: function (request, response) {
            
            $.ajax({
                url: '/Distributor/DistributorCreditManagment/GetMerchantMemberName/',
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
            $('#txtMemberaName').val(ui.item.label);
            return true;
        },
        select: function (e, i) {
            $("#hfOperator").val(i.item.val);
            return true;
        },
        minLength: 1
    });

    $("#ddlCreditNoteType").change(function () {
        var getvalue = $('#ddlCreditNoteType').val();
        if (getvalue == 'DR') {
            $('#ddlGSTValue').show();
        }
        else {
            $('#ddlGSTValue').hide();
        }
    });
});

$(document).ready(function () {


$("#txtMemberaName").change(function () {
    debugger;
    var MEM_ID = $('#hfOperator').val();
    $.ajax({
        url: "/DistributorCreditManagment/GetReservedCreditLimit?area=Distributor",
        data: {
            Mem_ID: MEM_ID
        },
        type: "post",
        datatype: "json",
        beforesend: function () {
        },
        success: function (data) {
            debugger;
            if (data == "0") {
                $('#txtDistReservedCreditLimit').text("0");
            }
            else {
                //var AvailableBal = data.CLOSING;                   
                var ReservedCreditLimitAmt = data.RESERVED_CREDIT_LIMIT;
                if (ReservedCreditLimitAmt == null) {
                    var AMtval = "0.00";
                    $('#txtDistReservedCreditLimit').val(AMtval);
                }
                else {
                    $('#txtDistReservedCreditLimit').val(ReservedCreditLimitAmt);
                }

            }
        },
        error: function (xhr, status, error) {
            console.log(status);
        }
    });
});
});


$(document).ready(function () {
    $("#txtRetailerMemberaName").autocomplete({
        source: function (request, response) {
            
            $.ajax({
                url: '/Distributor/DistributorCreditManagment/GetMerchantMemberName/',
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
            $('#txtRetailerMemberaName').val(ui.item.label);
            return true;
        },
        select: function (e, i) {
            $("#hfRetailerID").val(i.item.val);
            return true;
        },
        minLength: 1
    });

 
});