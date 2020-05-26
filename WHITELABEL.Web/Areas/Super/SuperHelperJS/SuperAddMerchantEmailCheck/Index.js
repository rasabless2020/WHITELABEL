$('#emailaddress').on('input blur change keyup', function () {

    if ($(this).val().length != 0) {
        var token = $(':input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: "/SuperAddMerchant/CheckEmailAvailability?area=Super",
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



function GetMerchantInformation(Member_ID) {
    var idval = Member_ID;
    $.ajax({
        url: "/SuperAddMerchant/GetMerchantList?area=Super",
        data: {
            MemberID: Member_ID
        },
        cache: false,
        type: "POST",
        dataType: "json",
        beforeSend: function () {
        },
        success: function (data) {
            var traninfo = data;
            $('#txtMerchantName').val(data.UName);
            $('#txtMerchantEmail').val(data.EMAIL_ID);
            $('#txtMerchantMobile').val(data.MEMBER_MOBILE);
            $('#DistributorIntroId').val(data.INTRODUCER);
            var Device_ID = data.MEM_ID;
            $('#Merchant_MID').val(data.MEM_ID);

            $('#MerchantMem_ID').val(data.MEM_ID);
        },
        error: function (xhr, status, error) {
            console.log(status);
        }
    });
}