function GetTAXMasterSetting(transid) {
    var idval = transid;
    $.ajax({
        url: "/PowerAdminTaxMasterSetting/GetFingerPrintInfo?area=PowerAdmin",
        data: {
            TransId: transid
        },
        cache: false,
        type: "POST",
        dataType: "json",
        beforeSend: function () {
        },
        success: function (data) {
            var traninfo = data;
            $('#txtTaxName').val(data.TAX_NAME);
            $('#txtDescriptionNo').val(data.TAX_DESCRIPTION);
            $('#txttaxModeCode').val(data.TAX_MODE);
            $('#ddl_TaxStatus').val(data.TAX_STATUS);
            $('#txttaxValueCode').val(data.TAX_VALUE);
            var Device_ID = data.SLN;
            $('#SLN').val(Device_ID);
            $('#MEM_ID').val(data.MEM_ID);
        },
        error: function (xhr, status, error) {
            console.log(status);
        }
    });
}