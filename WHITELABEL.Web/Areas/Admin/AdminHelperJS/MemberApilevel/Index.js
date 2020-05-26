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
                    url: "/MemberAPILabel/DeleteInformation?area=Admin",
                    //"@Url.Action("DeleteInformation", "MemberAPILabel", new {area = "Admin"})",
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
                                message: "Informaiton Deleted successfully",
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
                    //@*url:"@Url.Action("MemberStatusUpdate", "MemberAPILabel" , new {area = "Admin"})",*@
                    url: "/MemberAPILabel/MemberStatusUpdate?area=Admin",
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
                    //url: "@Url.Action("PasswordSendtoUser", "MemberAPILabel" , new {area="Admin"})",
                    //url: "/Admin/MemberAPILabel/PasswordSendtoUser",
                    url: "/MemberAPILabel/PasswordSendtoUser?area=Admin",
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

        url: "/MemberAPILabel/GetMemberPassword?area=Admin",
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
            url: "/MemberAPILabel/CheckEmailAvailability?area=Admin",
            //url: "@Url.Action("CheckEmailAvailability", "MemberAPILabel", new {area="Admin"})",
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

$('#emailaddressDistributor').on('input blur change keyup', function () {

    if ($(this).val().length != 0) {
        var token = $(':input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: "/MemberAPILabel/CheckEmailAvailability?area=Admin",
            //url: "@Url.Action("CheckEmailAvailability", "MemberAPILabel", new {area="Admin"})",
            data: {
                __RequestVerificationToken: token,
                emailid: $(this).val()
            },
            cache: false,
            type: "POST",
            success: function (data) {
                // DONE                        
                if (data.result == "available") {
                    $('#emailaddressDistributor').css('border', '3px #090 solid');
                    $('#btnsubmitDis').attr('disabled', false);
                }
                else {
                    $('#emailaddressDistributor').css('border', '3px #C33 solid');
                    $('#btnsubmitDis').attr('disabled', true);
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
        $('#btnsubmitDis').attr('disabled', true);
    }
});


$('#emailaddressMerchant').on('input blur change keyup', function () {

    if ($(this).val().length != 0) {
        var token = $(':input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: "/MemberAPILabel/CheckEmailAvailability?area=Admin",
            //url: "@Url.Action("CheckEmailAvailability", "MemberAPILabel", new {area="Admin"})",
            data: {
                __RequestVerificationToken: token,
                emailid: $(this).val()
            },
            cache: false,
            type: "POST",
            success: function (data) {
                // DONE                        
                if (data.result == "available") {
                    $('#emailaddressMerchant').css('border', '3px #090 solid');
                    $('#btnsubmitMer').attr('disabled', false);
                }
                else {
                    $('#emailaddressMerchant').css('border', '3px #C33 solid');
                    $('#btnsubmitMer').attr('disabled', true);
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
        $('#btnsubmitMer').attr('disabled', true);
    }
});


var _validFileExtensions = [".jpg", ".jpeg", ".bmp", ".gif", ".png"];
function ValidateUploadlogoInput(oInput) {
    debugger;
    var FileSize = oInput.files[0].size / 1024 / 1024; // in MB
    if (FileSize < .512) {
        if (oInput.type == "file") {
            var sFileName = oInput.value;
            if (sFileName.length > 0) {
                var blnValid = false;
                for (var j = 0; j < _validFileExtensions.length; j++) {
                    var sCurExtension = _validFileExtensions[j];
                    if (sFileName.substr(sFileName.length - sCurExtension.length, sCurExtension.length).toLowerCase() == sCurExtension.toLowerCase()) {
                        blnValid = true;
                        break;
                    }
                }
                if (!blnValid) {
                    var msg_alert = "Sorry, " + sFileName + " is invalid, allowed extensions are: " + _validFileExtensions.join(", ");
                    bootbox.alert({
                        size: "small",
                        message: msg_alert,
                        backdrop: true
                    });
                    //alert("Sorry, " + sFileName + " is invalid, allowed extensions are: " + _validFileExtensions.join(", "));
                    oInput.value = "";
                    return false;
                }
            }
        }
        return true;
    }
    else {
        bootbox.alert({
            size: "small",
            message: "File Size Less Than 512 KB",
            backdrop: true
        });
        oInput.value = "";
        return false;
    }
}