
$(function () {

    $('#txtReferenceNumber').on('input blur change keyup', function () {        
        if ($(this).val().length != 0) {
            var token = $(':input[name="__RequestVerificationToken"]').val();
            $.ajax({
                //url: "@Url.Action("CheckEmailAvailability")",
                url: '/MerchantRequisition/CheckReferenceNo?area=Merchant',
                data: {
                    referenceno: $(this).val(),
                    __RequestVerificationToken: token
                },
                cache: false,
                type: "POST",
                success: function (data) {
                    // DONE
                    debugger;
                    if (data.result == "available") {
                        $('#txtReferenceNumber').css('border', '3px #090 solid');
                        //$('#btnsubmit').attr('disabled', false);
                    }
                    else {
                        debugger;
                        $('#txtReferenceNumber').css('border', '3px #C33 solid');
                        //$('#txtMemberDomain').val(data.Mem_Name);                    
                        //$('#txtMem_ID').val(data.mem_Id);
                        //document.getElementById('txtMem_ID').value = data.mem_Id;
                        //$('#txtAmount').val(data.amt);
                        //$('#txtRequestDate').val(data.Req_Date);
                        //$('#BankID').val(data.Bankid);
                        //$('#Paymentmethod').val(data.paymethod);
                        //$('#txtTransactiondetails').val(data.Transdetails);
                        //$('#txtBankCharges').val(data.BankCharges);

                        //$('#btnsubmit').attr('disabled', true);
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
            // $('#btnsubmit').attr('disabled', true);
        }
    });

});
