function FetchCustomerDetails(id) {    
    //$(".overlaydiv").fadeIn("slow");
    var CustomerId = id;
    $.ajax({
        url: "/MerchantDMRSection/GetCustomerInformation?areas=Merchant",
        data: {
            Custmerid: CustomerId
        },
        contentType: "application/json",
        cache: false,
        type: "post",
        dataType: "json",        
        beforeSend: function () {
        },
        success: function (data) {            
            if (data != 'Data not found in database') {
                if (data == 'Please check mobile number length' || data == 'Mobile number should only be in digits') {
                    bootbox.alert({
                        size: "small",
                        message: data,
                        backdrop: true
                    });
                    //$(".overlaydiv").fadeOut("slow");
                }
                else {
                    //$(".overlaydiv").fadeOut("slow");                    
                    var url = "/Merchant/MerchantDMRSection/CustomerDetails";
                    window.location.href = url;
                }
            }
            else {
                //$(".overlaydiv").fadeOut("slow");                
                var url = "/Merchant/MerchantDMRSection/CreateCustomer";
                window.location.href = url;
            }
        },
        error: function (xhr, status, error) {
            console.log(status);
        }
    });
}
function GenerateOTP(id) {
    $(".overlaydiv").fadeIn("slow");
    var CustomerId = id;
    $.ajax({
        url: "/MerchantDMRSection/GenerateOTP?area=Merchant",
        data: {
            Custmerid: CustomerId
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
            $(".overlaydiv").fadeOut("slow");
        },
        error: function (xhr, status, error) {
            console.log(status);
        }
    });
}
//function DeleteRecipientInformation(Recipientid, CustomerId) {
function DeleteBeneficiaryAccountInformation(Recipientid, CustomerId, IdVal) {
    debugger;
    $(".overlaydiv").fadeIn("slow");
    var RecipientId = Recipientid;
    var CustomerId = CustomerId;
    var BenId = IdVal;
    bootbox.confirm({
        //title: "Message",
        message: "Do you want to delete information",
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
                $.ajax({
                    url: "/MerchantDMRSection/DeleteRecipientInformation?area=Merchant",
                    data: {
                        RecipientId: RecipientId,
                        CustomerId: CustomerId
                    },
                    cache: false,
                    type: "POST",
                    dataType: "json",
                    beforeSend: function () {
                    },
                    success: function (data) {
                        debugger;

                        var message = data;
                        var BeneficiaryId = message.beneficiaryid;
                        var remitterid = message.remitterid;
                        var msgcode = message.msgcode;
                        var mess = message.status;
                        var OtpStatus = message.OTPID_Status;
                        //$('#txtDMTCustomerMobile').val(CustId);
                        $('#hdnRemitterId').val(remitterid);
                        $('#hdnBeneficairyId').val(BeneficiaryId);
                        $('#hdnBeneId').val(BenId);
                        if (msgcode == 'TXN') {
                            if (OtpStatus == '1') {
                                //$('.transdDMRBeneficiaryDeleteInfo').modal('show');
                                $('.transDeleteBene').modal('show');
                                
                            }
                            else {
                                bootbox.alert({
                                message: mess,
                                size: 'small',
                                callback: function () {
                                    console.log(message);
                                    //var url = "/Merchant/MerchantDMRSection/CustomerDetails?CustId=" + CustId;
                                    var url = "/Merchant/MerchantDMRSection/CustomerDetails";
                                    window.location.href = url;
                                }
                            })
                            }
                            //bootbox.alert({
                            //    message: mess,
                            //    size: 'small',
                            //    callback: function () {
                            //        console.log(message);
                            //        //var url = "/Merchant/MerchantDMRSection/CustomerDetails?CustId=" + CustId;
                            //        var url = "/Merchant/MerchantDMRSection/CustomerDetails";
                            //        window.location.href = url;
                            //    }
                            //})
                        }
                        else
                        {
                            //$('.transdDMRBeneficiaryDeleteInfo').modal('hide');
                            $('.transDeleteBene').modal('hide');
                            bootbox.alert({
                                message: mess,
                                size: 'small',
                                callback: function () {
                                    console.log(message);
                                    //var url = "/Merchant/MerchantDMRSection/CustomerDetails?CustId=" + CustId;
                                    var url = "/Merchant/MerchantDMRSection/CustomerDetails";
                                    window.location.href = url;
                                }
                            })
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


function DeleteBeneficiaryThroughOTP(Recipientid, CustomerId, IdVal) {
    debugger;
    $('#transactionvalueAdminidLoader').show();
    $('#hdnBeneficairyId').val(CustomerId);
    $('#hdnRemitterId').val(Recipientid);
    $('#hdnBeneId').val(CustomerId);    
    //$('.transDeleteBeneAcnt').modal('show');
    $.ajax({
        url: "/MerchantDMRSection/DeleteRecipientInformation?area=Merchant",
        data: {
            RecipientId: Recipientid,
            CustomerId: CustomerId
        },
        cache: false,
        type: "POST",
        dataType: "json",
        beforeSend: function () {
        },
        success: function (data) {
            debugger;
            var message = data;
            var BeneficiaryId = message.beneficiaryid;
            var remitterid = message.remitterid;
            var msgcode = message.msgcode;
            var mess = message.status;
            var OtpStatus = message.OTPID_Status;
            //$('#txtDMTCustomerMobile').val(CustId);
            $('#hdnRemitterId').val(remitterid);
            $('#hdnBeneficairyId').val(BeneficiaryId);
            $('#hdnBeneId').val(BeneficiaryId);
            if (msgcode == 'TXN') {
                if (OtpStatus == '1') {
                    $('#transactionvalueAdminidLoader').hide();
                    $('.transDeleteBeneAcnt').modal('show');
                }
                else {
                    bootbox.alert({
                        message: mess,
                        size: 'small',
                        callback: function () {
                            console.log(message);
                            //var url = "/Merchant/MerchantDMRSection/CustomerDetails?CustId=" + CustId;
                            var url = "/Merchant/MerchantDMRSection/CustomerDetails";
                            window.location.href = url;
                        }
                    })
                }
            }
            else {
                //$('.transdDMRBeneficiaryDeleteInfo').modal('hide');
                $('#transactionvalueAdminidLoader').hide();
                $('.transDeleteBeneAcnt').modal('hide');
                bootbox.alert({
                    message: mess,
                    size: 'small',
                    callback: function () {
                        console.log(message);
                        //var url = "/Merchant/MerchantDMRSection/CustomerDetails?CustId=" + CustId;
                        var url = "/Merchant/MerchantDMRSection/CustomerDetails";
                        window.location.href = url;
                    }
                })
            }
        },
        error: function (xhr, status, error) {
            console.log(status);
        }
    });
    
   
}


function RecipientEnquery(RecipientId, CustomerId,changeAmt) {
    
    $(".overlaydiv").fadeIn("slow");
    //var RecipientId = Recipientid;
    //var CustomerId = CustomerId;
    var CustomerId_val = CustomerId;
    //var msgGet = "Do you want to verify bank Information.It will charged " + changeAmt+" Rs. from your wallet."
    var msgGet = "Do you want to verify bank Information."
    bootbox.confirm({
        //title: "Message",
        //message: "Do you want to verify recipient bank information",
        message: msgGet,
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
                //var CustomerId = CustomerId;
                $(".overlaydiv").fadeIn("slow");
                $.ajax({
                    url: "/MerchantDMRSection/RecipientEnquiry?area=Merchant",
                    data: {
                        RecipientId: RecipientId,
                        CustomerId: CustomerId_val
                    },
                    cache: false,
                    type: "POST",
                    dataType: "json",
                    beforeSend: function () {
                    },
                    success: function (data) {
                        $('#my-ajax-grid').mvcgrid('reload');
                        if (data.Result === "true") {
                            $('.mvc-grid').mvcgrid('reload');
                            $(".overlaydiv").fadeOut("slow");
                            bootbox.alert({
                                size: "small",
                                message: "Recipient Bank Details Verified successfully",
                                backdrop: true
                            });
                        }
                        else if (data.Result === "Varified")
                        {
                            $(".overlaydiv").fadeOut("slow");
                            bootbox.alert({
                                message: "Account is already verified.",
                                backdrop: true
                            });
                        }
                        else {
                            $(".overlaydiv").fadeOut("slow");
                            bootbox.alert({
                                message: "there is some thing error",
                                backdrop: true
                            });
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

//function RecipientEnquery(RecipientId, CustomerId) {
//    var CustomerId = CustomerId;
//    $(".overlaydiv").fadeIn("slow");
//    $.ajax({
//        url: "/MerchantDMRSection/RecipientEnquiry?area=Merchant",
//        data: {
//            RecipientId: RecipientId,
//            CustomerId: CustomerId
//        },
//        cache: false,
//        type: "POST",
//        dataType: "json",
//        beforeSend: function () {
//        },
//        success: function (data) {
//            $('#my-ajax-grid').mvcgrid('reload');
//            if (data.Result === "true") {
//                $('.mvc-grid').mvcgrid('reload');
//                $(".overlaydiv").fadeOut("slow");
//                bootbox.alert({
//                    size: "small",
//                    message: "Recipient Bank Details Verified successfully",
//                    backdrop: true
//                });
//            }
//            else {
//                $(".overlaydiv").fadeOut("slow");
//                bootbox.alert({
//                    message: "there is some thing error",
//                    backdrop: true
//                });
//            }
//        },
//        error: function (xhr, status, error) {
//            console.log(status);
//        }
//    });
//}


function getbeneficiaryDetails(Recipientid, CustomerId) {    
    var RecipientId = Recipientid;
    var CustomerId = CustomerId;
    $.ajax({
        url: "/MerchantDMRSection/GetBeneficiaryInformation?area=Merchant",
        data: {
            RecipientId: RecipientId,
            CustomerId: CustomerId
        },
        cache: false,
        type: "POST",
        dataType: "json",
        beforeSend: function () {
        },
        success: function (data) {
            debugger;
            var traninfo = data;            
            $('#txtBenificiaryName').val(traninfo.BENEFICIARY_NAME);            
            $('#txtBenificiaryAccountno').val(traninfo.ACCOUNT_NO);
            $("#txtBenificiaryIFSC").val(traninfo.IFSC_CODE);
            $("#sln").val(traninfo.RECIPIENT_ID);
            $("#BeneficiaryID").val(traninfo.BENEFICIARY_ID);            
            $("#txtBeneficiaryMobile").val(traninfo.BENEFICIARY_MOBILE);
            $("#txtCustomerNo").val(traninfo.CUSTOMER_ID);
        },
        error: function (xhr, status, error) {
            console.log(status);
        }
    });
}


function getbeneficiaryVerificationDetails(Recipientid, CustomerId) {
    var RecipientId = Recipientid;
    var CustomerId = CustomerId;
    $.ajax({
        url: "/MerchantDMRSection/GetBeneficiaryValidationInformation?area=Merchant",
        data: {
            RecipientId: RecipientId,
            CustomerId: CustomerId
        },
        cache: false,
        type: "POST",
        dataType: "json",
        beforeSend: function () {
        },
        success: function (data) {
            debugger;
            var traninfo = data;
            $('#txtDMTBeneficiaryName').val(traninfo.BENEFICIARY_NAME);
            $('#txtDMTBeneficiaryAccount').val(traninfo.ACCOUNT_NO);
            //$("#txtBenificiaryIFSC").val(traninfo.IFSC_CODE);
            //$("#sln").val(traninfo.RECIPIENT_ID);
            $("#hdnRemitterId").val(traninfo.RECIPIENT_ID);
            $("#hdnBeneficairyId").val(traninfo.BENEFICIARY_ID);
            //$("#txtBeneficiaryMobile").val(traninfo.BENEFICIARY_MOBILE);
            //$("#txtCustomerNo").val(traninfo.CUSTOMER_ID);
        },
        error: function (xhr, status, error) {
            console.log(status);
        }
    });
}



//function validateFloatKeyPress(el) {
//    var v = parseFloat(el.value);
//    el.value = (isNaN(v)) ? '' : v.toFixed(2);
//}
function validatePrice(textBoxId) {
    var textVal = textBoxId.value;
    var regex = /^(\$|)([1-9]\d{0,2}(\,\d{3})*|([1-9]\d*))(\.\d{2})?$/;
    var passed = textVal.match(regex);
    if (passed == null) {
        alert("Enter price only. For example: 523.36 or $523.36");
        textBoxId.Value = "";
    }
}

function DeleteBeneficiaryData(RemitterId, BeneficairyId, OTP) {
    if (OTP != "") {
        debugger;
        var RemitterId_Val = RemitterId;
        var BeneficairyId_Val = BeneficairyId;
        var OTP_Val = OTP;
        var token = $(':input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: "/Merchant/MerchantDMRSection/ValidateToDeleteBeneficiary",
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
                        $('.transDeleteBene').modal('hide');
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
    else {
        bootbox.alert({
            size: "small",
            message: "Please Provide OTP for Beneficiary Registration.",
            backdrop: true
        });
    }
}


//function TransferAmountToReceipent(Recipientid, CustomerId) {
   
//    var RecipientId = Recipientid;
//    var CustomerId = CustomerId;
//    var amount = $("#txtTransferAmt").val();
//    var SenderMobileNo = $("#txtMobileNo").val();
//    var SenderName = $("#txtSenderName").val();
//    var RecipientMobileNo = $("#txtBeneficiaryMobile").val();
//    var RecipientName = $("#txtBenificiaryName").val();
//    var RecipientAccountNo = $("#txtBenificiaryAccountno").val();
//    var RecipientIFSCCode = $("#txtBenificiaryIFSC").val();
//    var geolocation = $("#GeoLocation").val();
//    var IpAddress = $("#IpAddress").val();
//    if (amount != '') {
//        bootbox.confirm({
//            title: "Message",
//            message: "Do you want to transfer amount",
//            buttons: {
//                confirm: {
//                    label: 'Confirm',
//                    className: 'btn-success btn-sm'
//                },
//                cancel: {
//                    label: 'Cancel',
//                    className: 'btn-danger btn-sm'
//                }
//            },
//            callback: function (result) {

//                if (result) {
//                    debugger;
//                    var $this = $(this);
//                    $this.button('loading');
                    
//                    var data = {
//                        recSeqId: RecipientId,
//                        customerId: CustomerId,
//                        amount: amount,
//                        SenderMobileNo: SenderMobileNo,
//                        SenderName: SenderName,
//                        RecipientMobileNo: RecipientMobileNo,
//                        RecipientName: RecipientName,
//                        RecipientAccountNo: RecipientAccountNo,
//                        RecipientIFSCCode: RecipientIFSCCode,
//                        geolocation: geolocation,
//                        IpAddress: IpAddress,
//                    };
//                    $.ajax({
//                        url: "/Merchant/MerchantDMRSection/PostTransferAmountToRecipient",
//                        data: data,
//                        cache: false,
//                        type: "POST",
//                        dataType: "json",
//                        beforeSend: function () {
//                        },
//                        success: function (data) {                           
                          
//                            $("#transactionvalueAdminid").modal("hide");                           
//                            var message = data;                            
//                            bootbox.alert({
//                                message: message,
//                                size: 'small',
//                                callback: function () {
//                                    console.log(message);
//                                    var url = "/Merchant/MerchantDMRSection/CustomerDetails";
//                                    window.location.href = url;
//                                    //$("#DMTCustomerDetails").trigger("reset");
//                                }
//                            })
//                            setTimeout(function () {
//                                $this.button('reset');
//                            }, 8000);
//                            $('#transactionvalueAdminid').removeClass("hidden");
//                            $('#transactionvalueAdminid').hide();
//                            $('#loading').removeClass("hidden");
//                            $('#loading').hide();
//                        },
//                        error: function (xhr, status, error) {
//                            console.log(status);
//                        }
//                    });
//                }
//            }
//        });
//    }
//    else {
//        bootbox.alert({
//            message: "Please Enter Transfer Amount",
//            backdrop: true
//        });
//    }
//}


$('#btnload').on('click', function () {
    debugger;
    var $this = $(this);
    $this.button('loading');
    setTimeout(function () {
        $this.button('reset');
    }, 8000);
});



