function DeActivateBank(id) {
    bootbox.confirm({
        title: "Message",
        message: "Do you want to deactivate bank information",
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
                    url: '/BankDetails/DeactivateBankDetails',
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