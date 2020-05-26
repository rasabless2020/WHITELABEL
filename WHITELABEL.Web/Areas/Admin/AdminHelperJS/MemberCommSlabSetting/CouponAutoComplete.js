$(function () {
    $("#txtNewServiceType").autocomplete({
        source: function (request, response) {

            $.ajax({
                url: '/MemberCouponMaster/CouponAutoComplete?area=Admin',
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
                },
                beforeSend: function () {
                    angular.element(document.getElementById('CouponCommissionMappingController')).scope().ClearData();
                },
            });
        },
        select: function (e, i) {
            $("#hftNewServiceType").val(i.item.val);
            angular.element(document.getElementById('CouponCommissionMappingController')).scope().FetchOperator(i.item.label);
        },
        minLength: 0
    });
});



