$(function () {
    //$("#txtDTHOperator").autocomplete({
    //    source: function (request, response) {         
    //        $.ajax({
    //            url: '/MerchantRechargeService/AutoDTHRechargeComplete/',
    //            data: "{ 'prefix': '" + request.term + "'}",
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
    //        $("#hfDTHOperator").val(i.item.val);
    //    },
    //    minLength: 1
    //});

    $("#txtDTHOperator").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '/MerchantRechargeService/AutoDTHRechargeComplete/',
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
            $('#txtDTHOperator').val(ui.item.label);
            return false;
        },
        select: function (e, i) {
            $("#hfDTHOperator").val(i.item.val);
            return false;
        },
        minLength: 1
    }).data("ui-autocomplete")._renderItem = function (ul, item) {
        return $("<li></li>")
            .data("item.autocomplete", item)
            .append("<a>" + "<img style='width:40px;height:40px' src='data:image;base64," + item.image + "' /> " + item.label + "</a>")
            .appendTo(ul);
    };

    $("#txtDTHCircleName").autocomplete({
        source: function (request, response) {

            $.ajax({
                url: '/MerchantRechargeService/DTHCircleName/',
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
            $("#hfDTHCircleCode").val(i.item.val);
        },
        minLength: 1
    });

});