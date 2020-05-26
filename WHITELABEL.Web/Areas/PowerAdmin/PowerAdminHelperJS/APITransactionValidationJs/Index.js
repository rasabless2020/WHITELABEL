function GetAPIAccountVerifyAmt(transid) {
    var idval = transid;

    $.ajax({
        url: "/PowerAdminRecipientAccountValidation/GetAPIAccountVerifyAmt?area=PowerAdmin",
        data: {
            TransId: transid
        },
        cache: false,
        type: "POST",
        dataType: "json",
        beforeSend: function () {
        },
        success: function (data) {
            debugger;
            var traninfo = data;
            //var dateval = new Date(traninfo.data.REQUEST_DATE)
            $('#APITransAmt').val(data.RECIPIENT_ACNT_VRT_AMTt);
            $('#merchantAcntVerification').val(data.APPLIED_AMT_TO_MERCHANT);
            $('#txtAPIName').val(data.APINAME);            
            $('#STATUS').val(data.TDS_STATUS);
            
        },
        error: function (xhr, status, error) {
            console.log(status);
        }
    });
}

function SaveAPITransactionDetails(TdsVal, GstVal, TdsStatus, Gststatus) {
    debugger;
    if (TdsVal != "" && GstVal != "") {

        $.ajax({
            url: "/PowerAdminRecipientAccountValidation/POSTAPIAcntVarification?area=PowerAdmin",
            data: {
                APIAcntAmt: TdsVal,
                CustveriAmt: GstVal,
                ApiName: TdsStatus,
                ApiStatus: Gststatus
            },
            cache: false,
            type: "POST",
            dataType: "json",
            beforeSend: function () {
            },
            success: function (data) {
                $('.mvc-grid').mvcgrid('reload');
                $(".overlaydiv").fadeOut("slow");
                bootbox.alert({
                    size: "small",
                    message: "Api Account Validation Amount  Added",
                    backdrop: true
                });
                $('.transd').modal('hide');

            },
            error: function (xhr, status, error) {
                console.log(status);
            }
        });
    }
    else {
        bootbox.alert({
            size: "small",
            message: "Enter TDS and GST values",
            backdrop: false
        });

    }
}
