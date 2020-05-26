$(function () {
    $("#txtGassServiceOperator").autocomplete({
        source: function (request, response) {
            var OperatorType = "GAS";
            $.ajax({
                url: '/MerchantRechargeService/AutoGasBillService/',
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
            $('#txtGassServiceOperator').val(ui.item.label);
            return false;
        },
        select: function (e, i) {
            $("#hfGassServiceOperator").val(i.item.val);
            return false;
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

