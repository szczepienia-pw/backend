using System.Linq;
using backend.Dto.Requests;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace backend_tests.Doctor;

public partial class DoctorVaccinationTest
{
    [Fact]
    public void SuccessfullyChangeSlot()
    {
        var d = this.dataContextMock.Object.Doctors.First();
        this.doctorVaccinationController.HttpContext.Items.Add("User", d);
        
        var rsp = this.doctorVaccinationController.ChangeSlot(1, new ChangeSlotRequest() { VaccinationSlotId = 1 }).Result;

        Assert.IsType<OkObjectResult>(rsp);
        Assert.Equal(200, ((OkObjectResult) rsp).StatusCode);
    }
}