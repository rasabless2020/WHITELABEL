
function DeactivateTransactionlist(id) {
    bootbox.confirm({
        title: "Message",
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
                $.ajax({
                    //url: "@Url.Content("~/Requisition/DeactivateTransactionDetails")",
                    url: '/Requisition/DeactivateTransactionDetails',
                    data: {
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
                                message: "Informaiton status change successfully",
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


function updateStatus() {
    var trandate = $("#TransactionDate").val();
    //var TransationStatus = $("#TransationStatus").val();
    var TransationStatus = "1";
    var slnval = $("#sln").val();
    $.ajax({
        //url: "@Url.Content("~/Requisition/ChangeTransactionStatus")",
        url: '/Requisition/ChangeTransactionStatus',
        data: {
            trandate: trandate, TransationStatus: TransationStatus, slnval: slnval
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
                    message: "Informaiton status change successfully",
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


function TransactionDecline() {
  
    var trandate = $("#TransactionDate").val();
    //var TransationStatus = $("#TransationStatus").val();
    var TransationStatus = "0";
    var slnval = $("#sln").val();
    $.ajax({
        //url: "@Url.Content("~/Requisition/TransactionDecline")",
        url: '/Requisition/TransactionDecline',
        data: {
            trandate: trandate, TransationStatus: TransationStatus, slnval: slnval
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
                    message: "Informaiton status change successfully",
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


