
function printInvoiceOfMerchantDist(transid) {
    var idval = transid;
    $.ajax({
        url: "/MerchantCouponRequisition/PrintMerchantcouponInvoice?area=Merchant",
        // url: "@Url.Action("getTransdata", "MemberRequisition", new {area="Admin"})",
        data: {
            TransId: transid
        },
        cache: false,
        type: "POST",
        dataType: "json",
        beforeSend: function () {
        },
        success: function (data) {
            debugger;
            var traninfo = data;
            $('#txtmemberName').html(traninfo.Member_Name);
            $('#txtCompanyName').html(traninfo.CompanyName);
            $('#txtAddress').html(traninfo.Address);
            $('#txtMember_mobile').html(traninfo.Mem_Mobile);
            $('#txtCouponName').html(traninfo.COUPON_Name);
            $('#txtInvoiceDate').html(formatDate(traninfo.REQUEST_DATE));
            $("#txtCouponQuantity").html(traninfo.QTY);
            $("#txtCouponSellValue").html(traninfo.CouponPrice);
            $("#txtCouponTotalAmt").html(traninfo.TotalAmount);
            $("#txtCompanyGSTNo").html(traninfo.GSTNo);
            $("#txtCouponSubTotalAmt").html(traninfo.TotalAmount);
            $("#txtCouponGstTotalAmt").html(traninfo.GST_VALUE);
            $("#txtCouponTDSAmt").html(traninfo.TDS_VALUE);
            $("#txtCouponGrandAmt").html(traninfo.NET_SELL_VALUE);
            //document.getElementById("txtImgPath").src = traninfo.Logo;
            //$("#txtImgPath").html(traninfo.Logo);
        },
        error: function (xhr, status, error) {
            console.log(status);
        }
    });
}


function formatDate(inputDate) {
    var value = new Date(parseInt(inputDate.replace(/(^.*\()|([+-].*$)/g, '')));
    var formattedDate = value.getMonth() + 1 + "/" + value.getDate() + "/" + value.getFullYear();
    return formattedDate;
}