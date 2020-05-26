function CheckAvailableBalance() {
    
    var ID_Value = ID;
    $.ajax({
        url: "/MerchantDashboard/LoadAvailableBalance?area=Merchant",        
        cache: false,
        type: "post",
        datatype: "json",
        beforesend: function () {
        },
        success: function (data) {
            
            var AvailableBal = data.CLOSING;              
            $("#txtAvailableBal").val(AvailableBal);
            $('#txtAvailableBal').text(AvailableBal);
           
        },
        error: function (xhr, status, error) {
            console.log(status);
        }
    });
}

