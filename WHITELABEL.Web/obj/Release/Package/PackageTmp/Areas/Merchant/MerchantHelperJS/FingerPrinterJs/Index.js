function GetFingerPrinterInfo(transid) {    
    var idval = transid;
    $.ajax({
        url: "/MerchantFingerprintdevice/GetFingerPrintInfo?area=Merchant",
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
            $('#txtDeviceName').val(data.DEVICE_NAME);
            $('#txtDeviceModelNo').val(data.DEVICE_MODELNO);          
            $('#txtDeviceCode').val(data.DEVICE_CODE);
            $('#ddl_Status').val(data.STATUS);           
            var Device_ID = data.ID;
            $('#ID').val(Device_ID);
            $('#MEM_ID').val(data.MEM_ID);
        },
        error: function (xhr, status, error) {
            console.log(status);
        }
    });
}

function SaveFingerPrintInformation(Devicename, DeviceModel, DeviceCode, Status) {    
    if (TdsVal != "" && GstVal != "") {

        $.ajax({
            url: "/MerchantFingerprintdevice/AddFingerPrintDevice?area=Merchant",
            data: {
                Devicename: Devicename,
                DeviceModel: DeviceModel,
                DeviceCode: DeviceCode,
                Status: Status
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
                    message: "Finger Print Device Added",
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
            message: "Enter Devide Information",
            backdrop: false
        });

    }
}
