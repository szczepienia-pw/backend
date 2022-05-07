using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace backend_tests.Vaccination;

public partial class CommonVaccinationTest
{
    [Fact]
    public void SuccessfullyGetAvailableVaccinationSlots()
    {
        var rsp = this.commonVaccinationController.GetAvailableVaccinationSlots().Result;

        Assert.IsType<OkObjectResult>(rsp);
        Assert.Equal(200, ((OkObjectResult) rsp).StatusCode);
    }
}