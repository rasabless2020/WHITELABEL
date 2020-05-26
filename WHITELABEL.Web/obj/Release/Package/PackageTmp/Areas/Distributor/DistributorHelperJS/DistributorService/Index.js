
    function ActiveServiceUpdate(memberId, id, $this) {
        bootbox.confirm({
            //title: "Message",
            message: (document.getElementById($this).checked) ? "Do you want to activate service" : "Do you want to De-activate service",
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
                        url: "/DistributorService/UpdateActiveService?area=Distributor",
                        //url: "@Url.Action("UpdateActiveService", "DistributorService", new {area= "Distributor" })",
                        data: {
                            __RequestVerificationToken: token,
                            memberId: memberId,
                            Id: id,
                            isActive: document.getElementById($this).checked
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
                                //bootbox.alert({
                                //    size: "small",
                                //    message: "Informaiton Update successfully",
                                //    backdrop: true
                                //});
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
                else {
                    if (document.getElementById($this).checked) {
                        $("#" + $this).prop('checked', false);
                    }
                    else {
                        $("#" + $this).prop('checked', true);
                    }

                }
            }
        });

    }
    $("#MemberList").change(function () {


        var MemberId = $('#MemberList').val();
        var url = "/Distributor/DistributorService/ServiceDetails?memid=" + MemberId;
        window.location = url;
       // alert(c);
    });

    function getParameterByName(name) {
        var match = RegExp('[?&]' + name + '=([^&]*)').exec(window.location.search);
        return match && decodeURIComponent(match[1]);
    }
