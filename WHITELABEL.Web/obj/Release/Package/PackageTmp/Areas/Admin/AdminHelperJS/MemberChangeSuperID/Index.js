function GetDistributorInformation(Member_ID) {
    var idval = Member_ID;
    $.ajax({
        url: "/MemberChangeChannelLink/GetDistributorList?area=Admin",
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
            $('#txtDistributorName').val(data.UName);
            $('#txtDistributorEmail').val(data.EMAIL_ID);
            $('#txtDistributorMobile').val(data.MEMBER_MOBILE);
            $('#SuperIntroId').val(data.INTRODUCER);
            var Device_ID = data.MEM_ID;
            $('#DisID').val(data.MEM_ID);

            $('#MerchantMem_ID').val(data.MEM_ID);
        },
        error: function (xhr, status, error) {
            console.log(status);
        }
    });
}


function GetMerchantInformation(Member_ID) {    
    var idval = Member_ID;
    $.ajax({
        url: "/MemberChangeChannelLink/GetMerchantList?area=Admin",
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