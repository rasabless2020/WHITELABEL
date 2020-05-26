function GetapplicationValue(transid, ApplcantID) {
    debugger;
    var idval = transid;    
    var AppttID = ApplcantID;   
    $('#ApplicantId').val(AppttID);
    $('#hdn_value').val(idval);
}


function ApplicationApprove(railId, RailPass, applicationId, Applent_Id) {    
    if (railId != '' && RailPass != "") {
        bootbox.confirm({        
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
                var TransationStatus = "1";
                var railId_Value = railId;
                var railpwd = RailPass;
                var Appl_Id = applicationId;
                var Mem_Id = Applent_Id;
                $.ajax({                    
                    url: '/PowerAdminRailUtilityApplication/ApproveRailUtilityApplication?area=PowerAdmin',
                    data: {                        
                        railId_Value: railId_Value, railpwd: railpwd, Appl_Id: Appl_Id,Mem_Id:Mem_Id
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
                                message: "Application approved successfully",
                                backdrop: true
                            });
                            $('#transactionvalueid').modal('hide');
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
    });}
    else {
        //document.getElementById("divRailId").style.borderColor = "thick solid #0000FF";
        document.getElementById("divRailId").style.borderColor = "lightblue";
        document.getElementById("divPwd").style.borderColor = "thick solid #0000FF";
    }
   
}

function ApplicationDeclined(transid) {
    bootbox.confirm({        
        message: "Do you want to decline application",
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
                var TransationStatus = "0";                
                var slnval = transid;
                
                $.ajax({                    
                    url: '/PowerAdminRailUtilityApplication/DeclineRailUtilityApplication?area=PowerAdmin',
                    data: {
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
                                message: "Application declined",
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
