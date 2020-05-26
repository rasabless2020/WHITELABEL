
function DeactivateTransactionlist(id) {
    bootbox.confirm({
        //title: "Message",
        message: "Do you want to deactivate Transaction information",
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
                var token = $(':input[name="__RequestVerificationToken"]').val();
                $.ajax({
                    url: "/MemberRequisition/DeactivateTransactionDetails?area=Admin",
                    //url: "@Url.Action("DeactivateTransactionDetails", "MemberRequisition", new {area="Admin"})",
                    data: {    
                        __RequestVerificationToken: token,
                        transid: id
                    },
                    cache: false,
                    type: "POST",
                    dataType: "json",
                    beforeSend: function () {
                    },
                    success: function (data) {

                        if (data.Result === "true") {
                            $('.mvc-grid').mvcgrid('reload');
                            $(".overlaydiv").fadeOut("slow");
                            bootbox.alert({
                                size: "small",
                                message: "Status changed successfully",
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


function updateStatus(transid,PaymentTrnDetails) {  
    bootbox.confirm({
        //title: "Message",
        message: "Do you want to Approve Transaction",
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
                //var trandate = $("#TransactionDate").val();
                //var TransationStatus = $("#TransationStatus").val();
                var TransationStatus = "1";
                //var slnval = $("#sln").val();
                var slnval = transid;
                var PaymentTrnId = PaymentTrnDetails;

                var SettlementType = "";
                if (document.getElementById("chkpartialsSettlement").checked) {
                    SettlementType = "Partial";
                }
                if (document.getElementById("chkFullSettlement").checked) {
                    SettlementType = "Full";
                }

                var token = $(':input[name="__RequestVerificationToken"]').val();
                $.ajax({
                    url: "/MemberRequisition/ChangeTransactionStatus?area=Admin",
                    //url:"@Url.Action("ChangeTransactionStatus", "MemberRequisition", new {area = "Admin"})",
                    data: {
                        __RequestVerificationToken: token,
                        slnval: slnval,
                        SettlementTYPE: SettlementType,
                        PaymentTrnId: PaymentTrnId
                    },
                    cache: false,
                    type: "POST",
                    dataType: "json",
                    beforeSend: function () {
                    },
                    success: function (data) {
                        var messageval = data;
                        $('.mvc-grid').mvcgrid('reload');
                        $(".overlaydiv").fadeOut("slow");
                        bootbox.alert({
                            size: "small",
                            message: messageval,
                            backdrop: true,
                            callback: function () {
                                $('#transactionvalueAdminid').modal('hide');
                                $('.transd').modal('hide');
                                window.location.reload();
                            }
                        });
                        $(".overlaydiv").fadeOut("slow");
                        //if (data.Result === "true") {
                        //    $('.mvc-grid').mvcgrid('reload');
                        //    $(".overlaydiv").fadeOut("slow");
                        //    bootbox.alert({
                        //        size: "small",
                        //        message: "Transaction approved",
                        //        backdrop: true
                        //    });
                        //    $('#transactionvalueAdminid').modal('hide');
                        //    $('.transd').modal('hide');
                        //    window.location.reload();
                        //}
                        //else if (data.Result === "Pending") {
                        //    $(".overlaydiv").fadeOut("slow");
                        //    bootbox.alert({
                        //        message: "No succificient balance to transfer",
                        //        backdrop: true
                        //    });
                        //}
                        //else {
                        //    $(".overlaydiv").fadeOut("slow");
                        //    bootbox.alert({
                        //        message: "there is some thing error",
                        //        backdrop: true
                        //    });
                        //}
                    },
                    error: function (xhr, status, error) {
                        console.log(status);
                    }
                });
            }
        }
    });


    //var trandate = $("#TransactionDate").val();
    ////var TransationStatus = $("#TransationStatus").val();
    //var TransationStatus = "1";
    //var slnval = $("#sln").val(); 
    //var token = $(':input[name="__RequestVerificationToken"]').val();
    //$.ajax({
    //    url: "/MemberRequisition/ChangeTransactionStatus?area=Admin",
    //    //url:"@Url.Action("ChangeTransactionStatus", "MemberRequisition", new {area = "Admin"})",
    //    data: {    
    //        __RequestVerificationToken: token,
    //        trandate: trandate, TransationStatus: TransationStatus, slnval: slnval
    //    },
    //    cache: false,
    //    type: "POST",
    //    dataType: "json",
    //    beforeSend: function () {
    //    },
    //    success: function (data) {            
    //        if (data.Result === "true") 
    //        {
    //            $('.mvc-grid').mvcgrid('reload');
    //            $(".overlaydiv").fadeOut("slow");
    //            bootbox.alert({
    //                size: "small",
    //                message: "Transaction approved",
    //                backdrop: true
    //            });
    //            $('#transactmodal').modal('hide');
    //        }
    //        else if (data.Result === "Pending")
    //        {
    //            $(".overlaydiv").fadeOut("slow");
    //            bootbox.alert({
    //                message: "No succificient balance to transfer",
    //                backdrop: true
    //            });
    //        }
    //        else {
    //            $(".overlaydiv").fadeOut("slow");
    //            bootbox.alert({
    //                message: "there is some thing error",
    //                backdrop: true
    //            });
    //        }
    //    },
    //    error: function (xhr, status, error) {
    //        console.log(status);
    //    }
    //});
}

function TransactionDecline(transid, PaymentTrnDetails) {  
    bootbox.confirm({
        //title: "Message",
        message: "Do you want to deactivate Transaction information",
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
                //var trandate = $("#TransactionDate").val();
                //var TransationStatus = $("#TransationStatus").val();
                var TransationStatus = "0";
                //var slnval = $("#sln").val();
                var slnval = transid;
                var token = $(':input[name="__RequestVerificationToken"]').val();
                $.ajax({
                    url: "/MemberRequisition/TransactionDecline?area=Admin",
                    //url:"@Url.Action("TransactionDecline", "MemberRequisition", new {area = "Admin"})",
                    data: {
                        __RequestVerificationToken: token,
                        slnval: slnval,
                        PaymentTrnDetails: PaymentTrnDetails
                    },
                    cache: false,
                    type: "POST",
                    dataType: "json",
                    beforeSend: function () {
                    },
                    success: function (data) {
                        if (data.Result === "true") {
                            $('.mvc-grid').mvcgrid('reload');
                            $(".overlaydiv").fadeOut("slow");
                            bootbox.alert({
                                size: "small",
                                message: "Transaction declined",
                                backdrop: true
                            });
                            $('#transactionvalueAdminid').modal('hide');
                            $('.transd').modal('hide');
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



    //var trandate = $("#TransactionDate").val();
    ////var TransationStatus = $("#TransationStatus").val();
    //var TransationStatus = "0";
    //var slnval = $("#sln").val();  
    //var token = $(':input[name="__RequestVerificationToken"]').val();
    //$.ajax({
    //    url: "/MemberRequisition/TransactionDecline?area=Admin",
    //    //url:"@Url.Action("TransactionDecline", "MemberRequisition", new {area = "Admin"})",
    //    data: {     
    //        __RequestVerificationToken: token,
    //        trandate: trandate, TransationStatus: TransationStatus, slnval: slnval
    //    },
    //    cache: false,
    //    type: "POST",
    //    dataType: "json",
    //    beforeSend: function () {
    //    },
    //    success: function (data) {            
    //        if (data.Result === "true") {
    //            $('.mvc-grid').mvcgrid('reload');
    //            $(".overlaydiv").fadeOut("slow");
    //            bootbox.alert({
    //                size: "small",
    //                message: "Transaction declined",
    //                backdrop: true
    //            });
    //            $('#transactmodal').modal('hide');
    //        }
    //        else {
    //            $(".overlaydiv").fadeOut("slow");
    //            bootbox.alert({
    //                message: "there is some thing error",
    //                backdrop: true
    //            });
    //        }
    //    },
    //    error: function (xhr, status, error) {
    //        console.log(status);
    //    }
    //});
}

function getvalue(transid) {
    var idval = transid;
    $.ajax({
        url: "/MemberRequisition/getTransdata?area=Admin",
       // url: "@Url.Action("getTransdata", "MemberRequisition", new {area="Admin"})",
        data: {
        TransId: transid
    },
    cache: false,
        type: "POST",
            dataType: "json",
                beforeSend: function () {
                },
    success: function (data) {
        if (data.Result === "true") {            
            var traninfo = data;
            //var dateval = new Date(traninfo.data.REQUEST_DATE)
            $('#username').val(traninfo.data.FromUser);
            $('#TransactionDate').val(formatDate(traninfo.data.REQUEST_DATE));
            $('#sln').val(traninfo.data.SLN);
            $("#BankDetails").val(traninfo.data.BANK_ACCOUNT);
            $("#Amount").val(traninfo.data.AMOUNT);
            //document.getElementById("username").innerHTML = traninfo.data.AMOUNT;
        }
        else {
            $(".overlaydiv").fadeOut("slow");

        }
    },
    error: function (xhr, status, error) {
        console.log(status);
    }
});
    }

function formatDate(inputDate) {
    var value = new Date(parseInt(inputDate.replace(/(^.*\()|([+-].*$)/g, '')));
    var formattedDate = value.getMonth() + 1 + "/" + value.getDate() + "/" + value.getFullYear();
    return formattedDate;
}

$(function () {

    $('#txtReferenceNumber').on('input blur change keyup', function () {
        //debugger;
        if ($(this).val().length != 0) {
            var token = $(':input[name="__RequestVerificationToken"]').val();
            $.ajax({
                //url: "@Url.Action("CheckEmailAvailability")",
                url: '/MemberRequisition/CheckReferenceNo?area=Admin',
                data: {
                    referenceno: $(this).val(),
                    __RequestVerificationToken: token
                },
                cache: false,
                type: "POST",
                success: function (data) {
                    // DONE
                    //debugger;
                    if (data.result == "available") {
                        $('#txtReferenceNumber').css('border', '3px #090 solid');
                        //$('#btnsubmit').attr('disabled', false);
                    }
                    else {
                        //debugger;
                        $('#txtReferenceNumber').css('border', '3px #C33 solid');
                        //$('#txtMemberDomain').val(data.Mem_Name);                    
                        //$('#txtMem_ID').val(data.mem_Id);
                        //document.getElementById('txtMem_ID').value = data.mem_Id;
                        //$('#txtAmount').val(data.amt);
                        //$('#txtRequestDate').val(data.Req_Date);
                        //$('#BankID').val(data.Bankid);
                        //$('#Paymentmethod').val(data.paymethod);
                        //$('#txtTransactiondetails').val(data.Transdetails);
                        //$('#txtBankCharges').val(data.BankCharges);

                        //$('#btnsubmit').attr('disabled', true);
                        //alert("This email id is already registered");
                    }
                },
                error: function (xhr, status, error) {
                    console.log(xhr.responseText);
                    alert("message : \n" + "An error occurred" + "\n status : \n" + status + " \n error : \n" + error);
                }
            });
        }
        else {
            // $('#btnsubmit').attr('disabled', true);
        }
    });

});


$(function () {

    $('#txtRequestReferenceNumber').on('input blur change keyup', function () {
        //debugger;
        if ($(this).val().length != 0) {
            var token = $(':input[name="__RequestVerificationToken"]').val();
            $.ajax({
                //url: "@Url.Action("CheckEmailAvailability")",
                url: '/RequestRequisition/CheckReferenceNo?area=Admin',
                data: {
                    referenceno: $(this).val(),
                    __RequestVerificationToken: token
                },
                cache: false,
                type: "POST",
                success: function (data) {
                    // DONE
                    //debugger;
                    if (data.result == "available") {
                        $('#txtRequestReferenceNumber').css('border', '3px #090 solid');
                        //$('#btnsubmit').attr('disabled', false);
                    }
                    else {
                        //debugger;
                        $('#txtRequestReferenceNumber').css('border', '3px #C33 solid');
                        //$('#txtMemberDomain').val(data.Mem_Name);                    
                        //$('#txtMem_ID').val(data.mem_Id);
                        //document.getElementById('txtMem_ID').value = data.mem_Id;
                        //$('#txtAmount').val(data.amt);
                        //$('#txtRequestDate').val(data.Req_Date);
                        //$('#BankID').val(data.Bankid);
                        //$('#Paymentmethod').val(data.paymethod);
                        //$('#txtTransactiondetails').val(data.Transdetails);
                        //$('#txtBankCharges').val(data.BankCharges);

                        //$('#btnsubmit').attr('disabled', true);
                        //alert("This email id is already registered");
                    }
                },
                error: function (xhr, status, error) {
                    console.log(xhr.responseText);
                    alert("message : \n" + "An error occurred" + "\n status : \n" + status + " \n error : \n" + error);
                }
            });
        }
        else {
            // $('#btnsubmit').attr('disabled', true);
        }
    });

});


function ActivateRequisition(transid) {
    bootbox.confirm({
        //title: "Message",
        message: "Do you want to activate requisition",
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
                //var trandate = $("#TransactionDate").val();
                //var TransationStatus = $("#TransationStatus").val();
                var TransationStatus = "1";
                //var slnval = $("#sln").val();
                var slnval = transid;
                var token = $(':input[name="__RequestVerificationToken"]').val();
                $.ajax({
                    //url: "@Url.Content("~/Requisition/ChangeTransactionStatus")",
                    url: '/MemberRequisition/ActivateRequisition?area=Admin',
                    data: {
                        __RequestVerificationToken: token,
                        //trandate: trandate, TransationStatus: TransationStatus, slnval: slnval
                        slnval: slnval
                    },
                    cache: false,
                    type: "POST",
                    dataType: "json",
                    beforeSend: function () {
                    },
                    success: function (data) {

                        if (data.Result === "true") {
                            $('.mvc-grid').mvcgrid('reload');
                            $(".overlaydiv").fadeOut("slow");
                            bootbox.alert({
                                size: "small",
                                message: "Requisition Activated.",
                                backdrop: true
                            });
                            $('#modalpopup').modal('hide');

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


function DistributeCommission(CorrelationId) {
    bootbox.confirm({        
        message: "Do you want to distribute commission",
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
               
                var TransationStatus = "1";
                var slnval = CorrelationId;
                var token = $(':input[name="__RequestVerificationToken"]').val();
                $.ajax({
                    url: "/MemberCommDisburseSetting/DistributeCommission?area=Admin",
                    data: {
                        __RequestVerificationToken: token,
                        slnval: slnval
                    },
                    cache: false,
                    type: "POST",
                    dataType: "json",
                    beforeSend: function () {
                    },
                    success: function (data) {
                        var message = data;
                        bootbox.alert({
                            message: message,
                            size: 'small',
                            callback: function () {
                                console.log(message);
                            }
                        })
                        $('.mvc-grid').mvcgrid('reload');
                    },
                    error: function (xhr, status, error) {
                        console.log(status);
                    }
                });
            }
        }
    });


}


