$(document).ready(function () {
    $("#txtMemberMerchantaName").autocomplete({
        source: function (request, response) {
            //var MEm_Type = $('#txtRoleDetails').val();
            $.ajax({
                url: '/Distributor/DistributorRDSInformation/GetDistMerchantMemberName',
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
            $('#txtMemberMerchantaName').val(ui.item.label);
            return true;
        },
        select: function (e, i) {
            $("#hfdMerchantID").val(i.item.val);
            return true;
        },
        minLength: 1
    });

    $("#txtCancellationMemberMerchantaName").autocomplete({
        source: function (request, response) {
            //var MEm_Type = $('#txtRoleDetails').val();
            $.ajax({
                url: '/Distributor/DistributorRDSInformation/GetDistMerchantMemberName',
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
            $('#txtCancellationMemberMerchantaName').val(ui.item.label);
            return true;
        },
        select: function (e, i) {
            $("#hfdMerchantCancellationID").val(i.item.val);
            return true;
        },
        minLength: 1
    });
});