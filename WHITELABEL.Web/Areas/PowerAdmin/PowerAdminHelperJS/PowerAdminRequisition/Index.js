function DeactivateTransactionlist(id) {
    bootbox.confirm({
        title: "Message",
        message: "Do you want to activate/deactivate transaction status",
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
                    //url: "@Url.Content("~/Requisition/DeactivateTransactionDetails")",
                    url: '/PowerAdminRequisition/DeactivateTransactionDetails?area=PowerAdmin',
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

function updateStatus(transid) {
    bootbox.confirm({
        title: "Message",
        message: "Do you want to approve transaction",
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
                    url: '/PowerAdminRequisition/ChangeTransactionStatus?area=PowerAdmin',
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
                                message: "Transaction approved successfully",
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



    //var trandate = $("#TransactionDate").val();
    ////var TransationStatus = $("#TransationStatus").val();
    //var TransationStatus = "1";
    ////var slnval = $("#sln").val();
    //var slnval = transid;
    //var token = $(':input[name="__RequestVerificationToken"]').val();
    //$.ajax({
    //    //url: "@Url.Content("~/Requisition/ChangeTransactionStatus")",
    //    url: '/PowerAdminRequisition/ChangeTransactionStatus?area=PowerAdmin',
    //    data: {
    //        __RequestVerificationToken: token,
    //        //trandate: trandate, TransationStatus: TransationStatus, slnval: slnval
    //        slnval: slnval
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
    //                message: "Transaction approved successfully",
    //                backdrop: true
    //            });
    //            $('#modalpopup').modal('hide');

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

function TransactionDecline(transid) {
    bootbox.confirm({
        title: "Message",
        message: "Do you want to decline transaction",
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
                    //url: "@Url.Content("~/Requisition/TransactionDecline")",
                    url: '/PowerAdminRequisition/TransactionDecline?area=PowerAdmin',
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

                        if (data.Result === "true") {
                            $('.mvc-grid').mvcgrid('reload');
                            $(".overlaydiv").fadeOut("slow");
                            bootbox.alert({
                                size: "small",
                                message: "Transaction declined",
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




    //var trandate = $("#TransactionDate").val();
    ////var TransationStatus = $("#TransationStatus").val();
    //var TransationStatus = "0";
    //var slnval = $("#sln").val();
    //var token = $(':input[name="__RequestVerificationToken"]').val();
    //$.ajax({
    //    //url: "@Url.Content("~/Requisition/TransactionDecline")",
    //    url: '/PowerAdminRequisition/TransactionDecline?area=PowerAdmin',
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
    //            $('#modalpopup').modal('hide');
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

        url: '/PowerAdminRequisition/getTransdata?area=PowerAdmin',
        //url: "@Url.Action("getTransdata", "PowerAdminRequisition", new {area="PowerAdmin"})",
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


function ActivateRequisition(transid) {
    bootbox.confirm({
        title: "Message",
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
                    url: '/PowerAdminRequisition/ActivateRequisition?area=PowerAdmin',
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
