function getDMTvalue(transid) {
    var idval = transid;

    $.ajax({
        url: "/PowerAdminTaxMaster/GetTaxMaster?area=PowerAdmin",
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
            $('#TDS_Amount').val(data.TDS);
            $('#GST_Amount').val(data.GST);
            //if (data.TDS_STATUS == "1") {
            //    $('#TDS_Status_Val').val("Active")
            //}
            //else {
            //    $('#TDS_Status_Val').val("Active")
            //}
            //(data.TDS_STATUS);
            $('#TDS_Status_Val').val(data.TDS_STATUS);
            $('#GST_Status_Val').val(data.GST_STATUS);
        },
        error: function (xhr, status, error) {
            console.log(status);
        }
    });
}

function SaveTaxMasterDetails(TdsVal, GstVal, TdsStatus, Gststatus) {
    debugger;
    if (TdsVal != "" && GstVal != "") {

        $.ajax({
            url: "/PowerAdminTaxMaster/DMTBank_Margin?area=PowerAdmin",
            data: {
                TdsVal: TdsVal,
                GstVal: GstVal,
                TdsStatus: TdsStatus,
                Gststatus: Gststatus
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
                    message: "GST and TDS Added",
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
