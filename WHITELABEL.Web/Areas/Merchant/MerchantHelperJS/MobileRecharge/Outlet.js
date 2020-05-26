function GenerateOutletOTP(id) {
    $(".overlaydiv").fadeIn("slow");    
    var CustomerId = id;
    if (CustomerId !== "") {
        $.ajax({
            url: "/MerchantOutlet/GenerateOutletOTP?area=Merchant",
            data: {
                Custmerid: CustomerId
            },
            cache: false,
            type: "POST",
            dataType: "json",
            beforeSend: function () {
            },
            success: function (data) {
                debugger;
                
                $('#btnOutletsubmit').prop('disabled', false);
                if (data == 'OTP Sent successfully') {
                    bootbox.alert({
                        size: "small",
                        message: data,
                        backdrop: true
                    });
                }
                $(".overlaydiv").fadeOut("slow");
                $('#btnOutletsubmit').prop('disabled', false);
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
            message: "Please Enter Mobile No. for OTP",
            backdrop: true
        });
    }
   
}

function GetOutletMerchantInformation() {
    debugger;
    $(".overlaydiv").fadeIn("slow");
    $.ajax({
        url: "/MerchantRechargeService/GetMerchantOutletInformation?area=Merchant",       
        cache: false,
        type: "POST",
        dataType: "json",
        beforeSend: function () {
        },
        success: function (data) {
            debugger;
            if (data != "NotFound") {
                $('.transdOutletRegForm').modal('show');
                $('#txtOutletMobileNo').val(data.MEMBER_MOBILE);
                $('#txtOutletCustomerName').val(data.MEMBER_NAME);
                $('#txtOutletCustomerAddress').val(data.ADDRESS);
                $('#txtOutletPincode').val(data.PIN);
                $('#txtOutletCustomerEmail').val(data.EMAIL_ID);
                $('#txtOutletCompanyName').val(data.COMPANY);
                $('#txtOutletPancardNo').val(data.PAN_NO);
                $(".overlaydiv").fadeOut("slow");
            }
            else {
                
                $('.transdOutletRegForm').modal('hide');
                
            }
        },
        error: function (xhr, status, error) {
            console.log(status);
        }
    });
}


function RegisteredOutletInformation(PanNo_Val, OTPvalue) {
    if (PanNo_Val != "" && OTPvalue != "") {
        bootbox.confirm({
            //title: "Message",
            message: "Do you want to Registered Outlet Id",
            buttons: {
                confirm: {
                    label: 'Confirm',
                    className: 'btn-success btn-sm'
                },
                cancel: {
                    label: 'Cancel',
                    className: 'btn-danger btn-sm'
                }
            },
            callback: function (result) {
                if (result) {
                    debugger;
                    //var trandate = $("#TransactionDate").val();
                    //var TransationStatus = $("#TransationStatus").val();
                    var TransationStatus = "1";
                    //var slnval = $("#sln").val();
                    var PanNo = PanNo_Val;
                    var OTP = OTPvalue;
                    var token = $(':input[name="__RequestVerificationToken"]').val();
                    $.ajax({
                        url: "/MerchantRechargeService/PostRegisterOutletData?area=Merchant",
                        data: {
                            __RequestVerificationToken: token,
                            PanNo: PanNo,
                            OTP: OTP
                        },
                        cache: false,
                        type: "POST",
                        dataType: "json",
                        beforeSend: function () {
                        },
                        success: function (data) {
                            if (data == "TXN") {
                                $('.mvc-grid').mvcgrid('reload');
                                $(".overlaydiv").fadeOut("slow");
                                bootbox.alert({
                                    size: "small",
                                    message: "Merchant Outlet id is generated.",
                                    backdrop: true
                                });
                                $('.transdOutletRegForm').modal('hide');
                                window.location.reload();
                            }
                            else {
                                bootbox.alert({
                                    size: "small",
                                    message: "Please Try Again After After 15 minute",
                                    backdrop: true
                                });
                                $('.transdOutletRegForm').modal('hide');
                            }
                        },
                        error: function (xhr, status, error) {
                            console.log(status);
                        }
                    });
                }
            }
        });
    }
    else {
        bootbox.alert({
            size: "small",
            message: "Please Provide Pancard no and OTP for Outlet Registration.",
            backdrop: true
        });
    }
    
}

