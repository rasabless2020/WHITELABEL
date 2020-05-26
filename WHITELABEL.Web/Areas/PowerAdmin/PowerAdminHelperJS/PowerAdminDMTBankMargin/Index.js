function getDMTvalue(transid) {
    var idval = transid;

    $.ajax({
        url: "/PowerAdminDMT_BankMargin/GetDMT_BankMargin?area=PowerAdmin",
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
            //var dateval = new Date(traninfo.data.REQUEST_DATE)
            $('#DMTBANK_MARGIN').val(data.DMTBANK_MARGIN);
            $('#ID').val(data.ID);
        },
        error: function (xhr, status, error) {
            console.log(status);
        }
    });
}

function SaveDMTBankMargin(transid) {    
    if (transid!="")
    {
        var idval = transid;
        $.ajax({
            url: "/PowerAdminDMT_BankMargin/DMTBank_Margin?area=PowerAdmin",
            data: {
                TransId: transid
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
                    message: "DMT Bank Margin Added",
                    backdrop: true
                });
                $('.transd').modal('hide');

            },
            error: function (xhr, status, error) {
                console.log(status);
            }
        });
    }
    else
    {
        bootbox.alert({
            size: "small",
            message: "Enter some values",
            backdrop: false
        });

    }
       
   

    
}