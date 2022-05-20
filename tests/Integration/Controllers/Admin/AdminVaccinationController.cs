using backend.Dto.Requests;
using backend.Dto.Requests.Admin;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace backend_tests.Admin;

public partial class AdminVaccinationTest
{
    [Fact]
    public void SuccessfullyChangeSlot()
    {
        var rsp =this.adminVaccinationController.ChangeSlot(1, new ChangeSlotRequest() { VaccinationSlotId = 1 }).Result;

        Assert.IsType<OkObjectResult>(rsp);
        Assert.Equal(200, ((OkObjectResult) rsp).StatusCode);
    }

    [Fact]
    public void SuccessfullyGetVaccinations()
    {
        var rsp = this.adminVaccinationController.GetVaccinations(new FilterVaccinationsRequest() { Page = 1 }).Result;

        Assert.IsType<OkObjectResult>(rsp);
        Assert.Equal(200, ((OkObjectResult) rsp).StatusCode);
    }
}