
$(document).ready(function () {
    $("#ddlDistributorList").change(function () {
        $("#ddlMerchantList").empty();
        //$("#MemberList").val("--Select--");
        $.ajax({
            type: 'POST',
            //url: '@Url.Action("GetAdminMerchant")',
            url: '/MemberIssueDebitCreditNote/GetAdminMerchant?area=Admin',
            dataType: 'json',
            data: { Disid: $("#ddlDistributorList").val() },
            success: function (states) {
                $("#ddlMerchantList").append('<option selected value="">--Select Merchant--</option>');
                $.each(states, function (i, state) {
                    $("#ddlMerchantList").append('<option value="' + state.IDValue + '">' +
                        state.TextValue + '</option>');
                });
            },
            error: function (ex) {
                $("#MemberList").append('<option selected value="">--Select Super--</option>');
                //alert('Failed to retrieve data.' + ex);
            }
        });
        return false;
    })
});
$(document).on('change', '#ddlRoleDetails', function () {
    var GetRole = $("#ddlRoleDetails").val();
    if (GetRole == "4") {
        $('#divMainMember').show();
        $('#divDistributor').show();
        $('#divMerchant').hide();
    }
    else if (GetRole == "5") {
        $('#divMainMember').show();
        $('#divDistributor').show();
        $('#divMerchant').show();
    }
    else {
        $('#divMainMember').hide();
    }
});
$(document).on('change', '#ddlTDSApplication', function () {
    debugger;
    var GetTDS = $("#ddlTDSApplication").val();
    if (GetTDS == "1") {
        $("#txtTDSPercentage").prop('disabled', false);
    }
    else {
        $("#txtTDSPercentage").prop('disabled', true);
    }
});


$(document).on('change', '#ddlMerchantList', function () {
    debugger;
    var GetMer_ID = $("#ddlMerchantList").val();
    $('#ddlMerchantId_Val').val(GetMer_ID);
});
$(function () {
    $('#txtReferenceNumber').on('input blur change keyup', function () {
        //debugger;
        if ($(this).val().length != 0) {
            var token = $(':input[name="__RequestVerificationToken"]').val();
            $.ajax({
                url: '/MemberIssueDebitCreditNote/CheckReferenceNo?area=Admin',
                data: {
                    referenceno: $(this).val(),
                    __RequestVerificationToken: token
                },
                cache: false,
                type: "POST",
                success: function (data) {
                    if (data.result == "available") {
                        $('#txtReferenceNumber').css('border', '3px #090 solid');
                    }
                    else {
                        $('#txtReferenceNumber').css('border', '3px #C33 solid');
                    }
                },
                error: function (xhr, status, error) {
                    console.log(xhr.responseText);
                    alert("message : \n" + "An error occurred" + "\n status : \n" + status + " \n error : \n" + error);
                }
            });
        }
        else {
        }
    });

});


