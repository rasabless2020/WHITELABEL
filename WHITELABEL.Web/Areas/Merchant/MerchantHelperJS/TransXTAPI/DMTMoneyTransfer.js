function TransferAmountToReceipent(Recipientid, CustomerId) {
    var RecipientId = Recipientid;
    var CustomerId = CustomerId;
    var amount = $("#txtTransferAmt").val();
    var SenderMobileNo = $("#txtMobileNo").val();
    var SenderName = $("#txtSenderName").val();
    var RecipientMobileNo = $("#txtBeneficiaryMobile").val();
    var RecipientName = $("#txtBenificiaryName").val();
    var RecipientAccountNo = $("#txtBenificiaryAccountno").val();
    var RecipientIFSCCode = $("#txtBenificiaryIFSC").val();
    var geolocation = $("#GeoLocation").val();
    var IpAddress = $("#IpAddress").val();
    var PaymentMode = $("#ddlOptionMode").val();
    var BeneFiId = $("#BeneficiaryID").val();
    if (amount != '') {
        bootbox.confirm({
            //title: "Message",
            message: "Do you want to transfer amount",
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
                    //var $this = $(this);
                    //$this.button('loading');
                    $('#transactionvalueAdminidLoader').show();
                    $('#loading').show();
                    var data = {
                        recSeqId: RecipientId,
                        customerId: CustomerId,
                        amount: amount,
                        SenderMobileNo: SenderMobileNo,
                        SenderName: SenderName,
                        RecipientMobileNo: RecipientMobileNo,
                        RecipientName: RecipientName,
                        RecipientAccountNo: RecipientAccountNo,
                        RecipientIFSCCode: RecipientIFSCCode,
                        geolocation: geolocation,
                        IpAddress: IpAddress,
                        PaymentMode: PaymentMode,
                        BeneficiaryId: BeneFiId,
                    };
                    $.ajax({
                        url: "/Merchant/MerchantDMRSection/PostTransferAmountToRecipient",
                        data: data,
                        cache: false,
                        type: "POST",
                        dataType: "json",
                        beforeSend: function () {
                        },
                        success: function (data) {

                            $("#transactionvalueAdminid").modal("hide");
                            var message = data;
                            bootbox.alert({
                                message: message,
                                size: 'small',
                                callback: function () {
                                    console.log(message);
                                    var url = "/Merchant/MerchantDMRSection/CustomerDetails";
                                    window.location.href = url;
                                    //$("#DMTCustomerDetails").trigger("reset");
                                    //$(".modal").hide();
                                    //$(".modal").remove();
                                    //$("#transactionvalueAdminid").hide();
                                    //$('.mvc-grid').mvcgrid('reload');
                                    //$('#my-ajax-grid').mvcgrid('reload');                                            
                                }
                            })

                            $('#transactionvalueAdminidLoader').removeClass("hidden");
                            $('#transactionvalueAdminidLoader').hide();
                            $('#loading').removeClass("hidden");
                            $('#loading').hide();
                        },
                        error: function (xhr, status, error) {
                            console.log(status);
                        }
                    });
                }
            }
        });
    }
    else {
        bootbox.alert({
            message: "Please Enter Transfer Amount",
            backdrop: true
        });
    }
}
