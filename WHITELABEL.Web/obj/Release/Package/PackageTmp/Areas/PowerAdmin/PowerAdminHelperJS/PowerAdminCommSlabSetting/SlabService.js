app.service("SlabServiceList", function ($http) {
    this.getEmployee = function (employeeID) {
        var response = $http({
            method: "post",
            url: "PowerAdminCommissionSlab/AddCommissionSlab",
            params: {
                id: JSON.stringify(employeeID)
            }
        });
        return response;
    }
})