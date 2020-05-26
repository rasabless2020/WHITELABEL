
function DeleteInformation(id) {

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
                var token = $(':input[name="__RequestVerificationToken"]').val();
                $.ajax({
                    url: "/Retailer/DeleteInformation?area=Distributor",
                    //url:"@Url.Action("DeleteInformation", "Retailer", new {area = "Distributor"})",
                    data: {
                        __RequestVerificationToken: token,
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
                            bootbox.alert({
                                size: "small",
                                message: "Informaiton Deleted",
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

function MemberStatus(id, statusval) {
    var statuschk = statusval;
    var msg = "";
    if (statuschk == "True") {
        msg = "Are you sure to deactivate the user";
    }
    else {
        msg = "Are you sure to activate the user";
    }

    bootbox.confirm({
        //title: "Message",
        //message: "Do you want to Change Member Status",
        message: msg,
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
                    url: "/Retailer/MemberStatusUpdate?area=Distributor",
                    //url: "@Url.Action("MemberStatusUpdate", "Retailer", new {area= "Distributor" })",
                    data: {
                        __RequestVerificationToken: token,
                        id: id, statusval: statusval
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
                                message: "Status changed",
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
function SendMailToMember(id) {

    bootbox.confirm({
        //title: "Message",
        message: "Do you want to Send mail for Password to user",
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
                    url: "/Retailer/PasswordSendtoUser?area=Distributor",
                    //url: "@Url.Action("PasswordSendtoUser", "Retailer", new {area= "Distributor" })",
                    data: {
                        __RequestVerificationToken: token,
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
                            bootbox.alert({
                                size: "small",
                                message: "Password send successfully to the registered mail.",
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

function GetPassword(id) {
    var token = $(':input[name="__RequestVerificationToken"]').val();
    $.ajax({

        url: "/Retailer/GetMemberPassword?area=Distributor",
        data: {
            __RequestVerificationToken: token,
            id: id
        },
        cache: false,
        type: "POST",
        dataType: "json",
        beforeSend: function () {
        },
        success: function (data) {
            console.log(data);
            debugger;
            bootbox.alert({
                size: "small",
                message: "This is your password: <b>" + data.User_pwd + "</b>",
                backdrop: true
            });
        },
        error: function (xhr, status, error) {
            console.log(status);
        }
    });
}

$('#emailaddress').on('input blur change keyup', function () {

    if ($(this).val().length != 0) {
        var token = $(':input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: "/Retailer/CheckEmailAvailability?area=Distributor",
            //url: "@Url.Action("CheckEmailAvailability", "Retailer", new {area= "Distributor" })",
            data: {
                __RequestVerificationToken: token,
                emailid: $(this).val()
            },
            cache: false,
            type: "POST",
            success: function (data) {
                // DONE               
                if (data.result == "available") {
                    $('#emailaddress').css('border', '3px #090 solid');
                    $('#btnsubmit').attr('disabled', false);
                }
                else {
                    $('#emailaddress').css('border', '3px #C33 solid');
                    $('#btnsubmit').attr('disabled', true);
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
        $('#btnsubmit').attr('disabled', true);
    }
});