$(document).ready(function () {
    $("#txtMemberaName").autocomplete({
        source: function (request, response) {
            var MEm_Type = $('#txtRoleDetails').val();
            $.ajax({
                url: '/Admin/MemberCreditManagment/GetMemberName/',
                data: "{ 'prefix': '" + request.term + "','MemberType':'" + MEm_Type + "'}",
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