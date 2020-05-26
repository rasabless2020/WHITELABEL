$(function () {

    $('#txtReferenceNumber').on('input blur change keyup', function () {
        //debugger;
        if ($(this).val().length != 0) {
            var token = $(':input[name="__RequestVerificationToken"]').val();
            $.ajax({
                //url: "@Url.Action("CheckEmailAvailability")",
                url: '/SuperStockRequisition/CheckCouponReferenceNo?area=Super',
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
                        // debugger;
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
            // $('#btnsubmit').attr('disabled', true);
        }
    });

});



function GerRequisitioninformation(transid) {
    var idval = transid;
    $.ajax({
        url: "/SuperStockRequisition/getCouponReqTransdata?area=Super",
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
            debugger;
            //if (data.Result === "true") {
            var traninfo = data;
            //var dateval = new Date(traninfo.data.REQUEST_DATE)
            $('#txtusername').val(traninfo.Member_Name);
            $('#txtCouponType').val(traninfo.COUPON_Name);
            $('#hdnCouponId').val(traninfo.COUPON_TYPE);
            $('#txtReqDate').val(formatDate(traninfo.REQUEST_DATE));
            $('#sln').val(traninfo.SLN);
            $("#txtCouponQty").val(traninfo.QTY);
            $("#hdnCouponCommAmt").val(traninfo.CouponPrice);
            $("#txtAmount").val(traninfo.CouponPrice);
            $("#txtCompanyGstNo").val(traninfo.GSTNo);
            //document.getElementById("username").innerHTML = traninfo.data.AMOUNT;
            //}
            //else {
            //    $(".overlaydiv").fadeOut("slow");

            //}
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



function updateStatus(transid, CouponAmt, CoupSlabAMt, IsGstVal, IsTDSVal) {
    bootbox.confirm({
        title: "Message",
        message: "Do you want to Approve Requisition",
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
                var CpnAmt = CouponAmt;
                var CpnSlabAmt = CoupSlabAMt;
                var GstVal = IsGstVal;
                var TDSVal = IsTDSVal;
                var token = $(':input[name="__RequestVerificationToken"]').val();
                $.ajax({
                    url: "/SuperStockRequisition/ApproveRequisitionStatus?area=Super",
                    //url:"@Url.Action("ChangeTransactionStatus", "MemberRequisition", new {area = "Admin"})",
                    data: {
                        __RequestVerificationToken: token,
                        slnval: slnval,
                        CouponAmt: CpnAmt,
                        SlabAmt: CpnSlabAmt,
                        GstVal: GstVal,
                        TDSVal: TDSVal
                    },
                    cache: false,
                    type: "POST",
                    dataType: "json",
                    beforeSend: function () {
                    },
                    success: function (data) {
                        var msg = data;
                        $('.mvc-grid').mvcgrid('reload');
                        $(".overlaydiv").fadeOut("slow");
                        bootbox.alert({
                            size: "small",
                            message: msg,
                            backdrop: true
                        });
                        $('#transactionvalueAdminid').modal('hide');
                        
                        $("#transactionvalueAdminid .close").click()
                        $('#transactionvalueAdminid').hide();
                        $('.modal-backdrop').hide();
                        //window.location.reload();
                    },
                    error: function (xhr, status, error) {
                        bootbox.alert({
                            message: "there is some thing error",
                            backdrop: true
                        });
                    }
                });
            }
        }
    });
}

function TransactionDecline(transid) {
    bootbox.confirm({
        title: "Message",
        message: "Do you want to decline requisition",
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
                    url: "/SuperStockRequisition/RequisitionDecline?area=Super",
                    //url:"@Url.Action("TransactionDecline", "MemberRequisition", new {area = "Admin"})",
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
                        var Msg = data;
                        $('.mvc-grid').mvcgrid('reload');
                        $(".overlaydiv").fadeOut("slow");
                        bootbox.alert({
                            size: "small",
                            message: Msg,
                            backdrop: true
                        });
                        $('#transactmodal').modal('hide');


                    },
                    error: function (xhr, status, error) {
                        bootbox.alert({
                            size: "small",
                            message: "please try again after some time",
                            backdrop: true
                        });
                    }
                });
            }
        }
    });


}



function printInvoiceOfSuperDist(transid) {
    var idval = transid;
    $.ajax({
        url: "/SuperStockRequisition/PrintcouponInvoice?area=Super",
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
            debugger;
            var traninfo = data;
            $('#txtmemberName').html(traninfo.Member_Name);
            $('#txtCompanyName').html(traninfo.CompanyName);
            $('#txtAddress').html(traninfo.Address);
            $('#txtMember_mobile').html(traninfo.Mem_Mobile);
            $('#txtCouponName').html(traninfo.COUPON_Name);
            $('#txtInvoiceDate').html(formatDate(traninfo.REQUEST_DATE));
            $("#txtCouponQuantity").html(traninfo.QTY);
            $("#txtCouponSellValue").html(traninfo.CouponPrice);
            $("#txtCouponTotalAmt").html(traninfo.TotalAmount);
            $("#txtCompanyGSTNo").html(traninfo.GSTNo);
            $("#txtCouponSubTotalAmt").html(traninfo.TotalAmount);
            $("#txtCouponGstTotalAmt").html(traninfo.GST_VALUE);
            $("#txtCouponTDSAmt").html(traninfo.TDS_VALUE);
            $("#txtCouponGrandAmt").html(traninfo.NET_SELL_VALUE);
            //document.getElementById("txtImgPath").src = traninfo.Logo;
            //$("#txtImgPath").html(traninfo.Logo);
        },
        error: function (xhr, status, error) {
            console.log(status);
        }
    });
}
