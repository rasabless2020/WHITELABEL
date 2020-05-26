$('#emailaddress').on('input blur change keyup', function () {

    if ($(this).val().length != 0) {
        var token = $(':input[name="__RequestVerificationToken"]').val();
        $.ajax({
            url: "/MemberChannelRegistration/CheckEmailAvailability?area=Admin",
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