function getCouponvalue(transid) {
    var idval = transid;

    $.ajax({
        //url: "/Merchant_Coupan_Master/GetDMT_BankMargin",
        url: "/MemberCouponMaster/GetCouponInfo?area=Admin",
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
            $('#txtCouponName').val(data.couponType);
            $('#txtCouponAmount').val(data.vendor_coupon_price);
            $('#hdnCouponId').val(data.sln);
        },
        error: function (xhr, status, error) {
            console.log(status);
        }
    });
}

function SaveCouponMaster(couponName,CouponAmt,transid) {
    if (couponName != "") {
        var couponN = couponName;
        var CoupAmt = CouponAmt;
        var idval = transid;
        $.ajax({
            //url: "/PowerAdminDMT_BankMargin/DMTBank_Margin",
            url: "/MemberCouponMaster/AddNewCoupon?area=Admin",
            data: { CouponName: couponN, couponAmt: CoupAmt, CouponId: idval },
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
                    message: "Coupon Added",
                    backdrop: true
                });
                $('#txtCouponName').val('');
                $('#txtCouponAmount').val('');
                $('#hdnCouponId').val('');
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
            message: "Enter some values",
            backdrop: false
        });

    }




}



function GerRequisitioninformation(transid) {
    var idval = transid;
    $.ajax({
        url: "/MemberCouponMaster/getCouponReqTransdata?area=Admin",
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
            $("#txtDistributorGapComm").val(traninfo.DistGapCommPrice);
            $("#txtSuperGapComm").val(traninfo.SuperGapCommPrice);
            $("#txtDistributorComm").val(traninfo.DistributorCommPrice);
            $("#txtSuperComm").val(traninfo.SuperCommPrice);
            

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



//function updateStatus(transid, CouponAmt, CoupSlabAMt, IsGstVal, IsTDSVal, CouponStartNo, CouponEndNo, UTINo, CouponEndNoDate) {
//function updateStatus(transid, CouponAmt, CoupSlabAMt, IsGstVal, IsTDSVal) {
function updateStatus(transid, CouponAmt, CoupSlabAMt, IsGstVal, IsTDSVal, CouponStartNo, CouponEndNo, DistributorGapCom, SuperGapCom) {
    if (CouponStartNo != '' && CouponEndNo!= '')
    {
        bootbox.confirm({
            //title: "Message",
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
                    var CouponStartNoVal = CouponStartNo;
                    var CouponEndNoVal = CouponEndNo;
                    var DistGapComm = DistributorGapCom;
                    var SuperGapComm = SuperGapCom;
                    //var DistGSTApply = ChkDistributorGST;
                    //var SuperGSTApply = ChkSuperGST;
                    //var CouponEndNo_Date = CouponEndNoDate;
                    //var UTIPaymentNo = UTINo;
                    var token = $(':input[name="__RequestVerificationToken"]').val();
                    $.ajax({
                        url: "/MemberCouponMaster/ApproveRequisitionStatus?area=Admin",
                        //url:"@Url.Action("ChangeTransactionStatus", "MemberRequisition", new {area = "Admin"})",
                        data: {
                            __RequestVerificationToken: token,
                            slnval: slnval,
                            CouponAmt: CpnAmt,
                            SlabAmt: CpnSlabAmt,
                            GstVal: GstVal,
                            TDSVal: TDSVal,
                            CouponStartNoVal: CouponStartNoVal,
                            CouponEndNoVal: CouponEndNoVal,
                            DistGapComm: DistGapComm,
                            SuperGapComm: SuperGapComm
                            //DistGSTApply: DistGSTApply,
                            //SuperGSTApply: SuperGSTApply
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
                            // window.location.reload();
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
    else
    {
        //$("#txtCouponStartNo").css("border", "1px solid red");
        //$("#txtCouponEndNo").css("border", "1px solid red");
        bootbox.alert({
            message: "Please Enter Coupon Start no. and Coupon End no.",
            backdrop: true
        });
    }
   
}

function TransactionDecline(transid) {
    bootbox.confirm({
        //title: "Message",
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
                    url: "/MemberCouponMaster/RequisitionDecline?area=Admin",
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
        url: "/MemberCouponMaster/PrintcouponInvoice?area=Admin",
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
