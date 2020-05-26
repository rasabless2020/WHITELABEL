$(function () {
    //debugger;
    //var PrepaidRecharge = $('#PrepaidRecharge1').val(); 
    //var PostpaidRecharge = $('#PostpaidRecharge2').val();
    //var OperatorType = "";
    //if (document.getElementById("PrepaidRecharge1").checked)
    //{
    //    OperatorType = "Prepaid";
    //}
    //if (document.getElementById("PostpaidRecharge2").checked) {
    //    OperatorType = "Postpaid";
    //}
    $("#txtMemberDomain").autocomplete({
        source: function (request, response) {
            //var OperatorType = "";
            //if (document.getElementById("PrepaidRecharge1").checked) {
            //    OperatorType = "Prepaid";
            //}
            //if (document.getElementById("PostpaidRecharge2").checked) {
            //    OperatorType = "POSTPAID";
            //}
            $.ajax({
                url: '/PowerAdminRequisition/AutoComplete/',
                data: "{ 'query': '" + request.term + "'}",
                dataType: "json",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    response($.map(data, function (item) {
                        return item;
                    }))
                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        },
        select: function (e, i) {
            $("#hfmemberDomain").val(i.item.val);
        },
        minLength: 1
    });
});

$(document).ready(function () {
    $("#PrepaidRecharge1").change(function () {
        $('#txtOperator').val('');
    });
    $("#PostpaidRecharge2").change(function () {

        $('#txtOperator').val('');
    });
});

$(function () {

    $('#txtReferenceNumber').on('input blur change keyup', function () {

        if ($(this).val().length != 0) {
            var token = $(':input[name="__RequestVerificationToken"]').val();
            $.ajax({
                //url: "@Url.Action("CheckEmailAvailability")",
                url: '/PowerAdminRequisition/CheckReferenceNo?area=PowerAdmin',
                data: {
                    referenceno: $(this).val(),
                    __RequestVerificationToken: token
                },
                cache: false,
                type: "POST",
                success: function (data) {
                    // DONE

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