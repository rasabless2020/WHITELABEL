﻿function Filedownload(filename, id) {
    var type = filename;
    var idval = id;
    alert(type, idval);
    $.ajax({
        //url: '@Url.Action("downloadfiles", "KYCVerification")',
        url: '/KYCVerification/downloadfiles',
        type: 'GET',
        dataType: 'json',
        cache: false,
        data: { type: type, id: idval },
        success: function (results) {
            alert(results)
        },
        error: function () {
            alert('Error occured');
        }
    });
}
function showimages(imagepath) {
    var dialog = bootbox.dialog({
        title: 'Image Preview',
        message: "<center><p><img src=" + imagepath + " /></p></center>",
        buttons: {
            //cancel: {
            //    label: "cancel",
            //    classname: 'btn-danger',
            //    callback: function () {
            //        example.show('custom cancel clicked');
            //    }
            //}
            ok: {
                label: "Close",
                className: 'btn-info',
                callback: function () {
                    Example.show('Custom OK clicked');
                }
            }
        }
    });
}


function KYCDocApprove(id) {

    bootbox.confirm({
        title: "Message",
        message: "Do you want to Approve Document",
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
                    //url: "@Url.Content("~/KYCVerification/ApproveKYCDocument")",
                    url: '/KYCVerification/ApproveKYCDocument',
                    data: {
                        id: id
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

function KYCDocReject(id) {


    bootbox.confirm({
        title: "Message",
        message: "Do you want to Reject Document",
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
                    //url: "@Url.Content("~/KYCVerification/RejectKYCDocument")",
                    url: '/KYCVerification/RejectKYCDocument',
                    data: {
                        id: id
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
