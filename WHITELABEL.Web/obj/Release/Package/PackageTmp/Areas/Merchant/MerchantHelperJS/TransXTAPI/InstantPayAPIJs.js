//function VerifyDMTCustomerMobileNo(MObileNo, RemitterId_Val, OTP) {
function VerifyDMTCustomerMobileNo(MObileNo, RemitterId_Val, OTP_Val) {
    debugger;
    if (MObileNo != "" && OTP_Val != "") {
        bootbox.confirm({
            //title: "Message",
            message: "Do you want to Validate customer",
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
                    $('#progressAddCustomer').show();
                    var RemitterId = RemitterId_Val;
                    var Mobile = MObileNo;
                    var OTP_Value = OTP_Val;
                    var token = $(':input[name="__RequestVerificationToken"]').val();
                    $.ajax({
                        url: "/MerchantDMRSection/ValidateCustomerMobileNo?area=Merchant",
                        data: {
                            __RequestVerificationToken: token,
                            MobileNo: Mobile,
                            RemitterId: RemitterId,
                            OTP_Val: OTP_Value
                        },
                        cache: false,
                        type: "POST",
                        dataType: "json",
                        beforeSend: function () {
                        },
                        success: function (data) {
                            var message = data;
                            if (message == "Customer Mobile No is verified") {
                                bootbox.alert({
                                    size: "small",
                                    message: "Customer Created Successfully",
                                    backdrop: true,
                                    callback: function () {
                                        var url = "/Merchant/MerchantDMRSection/CustomerDetails";
                                        window.location.href = url;
                                    }
                                });
                            }
                            else {
                                $('#progressAddCustomer').hide();
                                bootbox.alert({
                                    size: "small",
                                    message: message,
                                    backdrop: true
                                });
                            }                            
                            //$('.transdDMRCustRegForm').modal('hide');
                            //window.location.reload();
                        },
                        error: function (xhr, status, error) {
                            $('#progressAddCustomer').hide();
                            console.log(status);
                        }
                    });
                }
            }
        });
    }
    else {
        $('#progressAddCustomer').hide();
        bootbox.alert({
            size: "small",
            message: "Please Provide Mobile no and OTP for Customer Registration.",
            backdrop: true
        });
    }

}
function VerifyBeneficaryAccountNo(RemitterId, BeneficairyId, OTP) {    
    if ( OTP != "") {
        bootbox.confirm({
            //title: "Message",
            message: "Do you want to Validate customer",
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

                    var RemitterId_Val = RemitterId;
                    var BeneficairyId_Val = BeneficairyId;
                    var OTP_Val = OTP;
                    var token = $(':input[name="__RequestVerificationToken"]').val();
                    $.ajax({
                        url: "/MerchantDMRSection/ValidateBeneficiary?area=Merchant",
                        data: {
                            __RequestVerificationToken: token,
                            RemId: RemitterId_Val,
                            BeneId: BeneficairyId_Val,
                            OTP_Val: OTP_Val
                        },
                        cache: false,
                        type: "POST",
                        dataType: "json",
                        beforeSend: function () {
                        },
                        success: function (data) {
                            debugger;
                            var message = data;
                            bootbox.alert({
                                size: "small",
                                message: message,
                                backdrop: true,
                                callback: function () {
                                    console.log(message);
                                    $('.transdDMRBeneficiaryRegForm').modal('hide');
                                    //var url = "/Merchant/MerchantDMRSection/CustomerDetails?CustId=" + CustId;
                                    var url = "/Merchant/MerchantDMRSection/CustomerDetails";
                                    window.location.href = url;
                                }
                            });
                            
                            //window.location.reload();
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
            message: "Please Provide OTP for Beneficiary Registration.",
            backdrop: true
        });
    }

}



function GenerateOTP(RemitterId, BeneficiaryId) {
    //$(".overlaydiv").fadeIn("slow");
    var RemitterId = RemitterId;
    var BeneficiaryId = BeneficiaryId;
    $.ajax({
        url: "/MerchantDMRSection/ResendDMROTP?area=Merchant",
        data: {
            RemitterId: RemitterId,
            BeneficiaryId: BeneficiaryId
        },
        cache: false,
        type: "POST",
        dataType: "json",
        beforeSend: function () {
        },
        success: function (data) {
            if (data == 'OTP Send to your Mobile no.') {
                bootbox.alert({
                    size: "small",
                    message: data,
                    backdrop: true
                });
            }
            else {
                bootbox.alert({
                    size: "small",
                    message: data,
                    backdrop: true
                });
            }
        },
        error: function (xhr, status, error) {
            console.log(status);
        }
    })
}
    

function GETBankInformation(BeneficiaryACNo) {
    //$(".overlaydiv").fadeIn("slow");
    var BeneficiaryACId = BeneficiaryACNo;
    debugger;
    $.ajax({
        url: "/MerchantDMRSection/GetAccountInformation?area=Merchant",
        data: {
            AccountNo: BeneficiaryACId
        },
        cache: false,
        type: "POST",
        dataType: "json",
        beforeSend: function () {
        },
        success: function (data) {
            if (data == 'OTP Send to your Mobile no.') {
                bootbox.alert({
                    size: "small",
                    message: data,
                    backdrop: true
                });
            }
            else {
                bootbox.alert({
                    size: "small",
                    message: data,
                    backdrop: true
                });
            }
        },
        error: function (xhr, status, error) {
            console.log(status);
        }
    })
}


$(function () {
    $("#txtBankName").autocomplete({
        source: function (request, response) {

            $.ajax({
                url: '/MerchantDMRSection/BankNameList?area=Merchant',
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
                }
            });
        },
        select: function (e, i) {
            $("#hfBankNAme").val(i.item.val);
        },
        minLength: 1
    });

});