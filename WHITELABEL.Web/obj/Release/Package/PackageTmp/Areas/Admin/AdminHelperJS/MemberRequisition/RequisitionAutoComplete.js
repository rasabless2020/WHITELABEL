$(document).ready(function () {
    $("#FromUser").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '/MemberRequisition/GetChannelMembername?area=Admin',
                type: "POST",
                dataType: "json",
                data: { Prefix: request.term },
                success: function (data) {
                    response($.map(data, function (item) {
                        return { label: item.UName, value: item.MEM_ID };
                    }))

                }
            })
        },
        messages: {
            noResults: "", results: ""
        }
    });
}) 