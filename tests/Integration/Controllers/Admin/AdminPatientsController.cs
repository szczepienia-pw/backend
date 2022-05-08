using backend.Dto.Requests.Admin.Patient;
using backend.Dto.Requests.Patient;
using backend.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace backend_tests.Admin;

public partial class AdminPatientsTest
{
    [Fact]
    public void ItSuccessfullyShowAllPatientsData()
    {
        var rsp = this.adminPatientController.ShowPatients(new ShowPatientsRequest() { Page = 1 }).Result;

        Assert.IsType<OkObjectResult>(rsp);
        Assert.Equal(200, ((OkObjectResult) rsp).StatusCode);
    }

    [Theory]
    [InlineData(1)]
    public void ItSuccessfullyShowPatientsData(int patientsId)
    {
        var rsp = this.adminPatientController.ShowPatient(patientsId).Result;

        Assert.IsType<OkObjectResult>(rsp);
        Assert.Equal(200, ((OkObjectResult) rsp).StatusCode);
    }
    
    [Theory]
    [InlineData(100)]
    public void ItFailToShowPatientsData(int patientsId)
    {
        Assert.ThrowsAsync<NotFoundException>(() => this.adminPatientController.ShowPatient(patientsId));
    }

    [Theory]
    [InlineData(1)]
    public void ItSuccessfullyDeletesAPatient(int patientsId)
    {
        var rsp = this.adminPatientController.DeletePatient(patientsId).Result;

        Assert.IsType<OkObjectResult>(rsp);
        Assert.Equal(200, ((OkObjectResult) rsp).StatusCode);
    }

    [Theory]
    [InlineData(100)]
    public void ItFailureToDeleteAPatient(int patientsId)
    {
        Assert.ThrowsAsync<NotFoundException>(() => this.adminPatientController.DeletePatient(patientsId));
    }

    [Theory]
    [InlineData("john@patient2.com", 1)]
    public void ItSuccessfullyUpdatePatient(string email, int patientsId)
    {
        var rsp = this.adminPatientController.EditPatient(patientsId, new PatientRequest() { Email = email }).Result;

        Assert.IsType<OkObjectResult>(rsp);
        Assert.Equal(200, ((OkObjectResult) rsp).StatusCode);        
    }

    [Theory]
    [InlineData("alsdkjf;lasdkf@lakdjfl.com", 100)]
    public void ItFailureToUpdatePatient(string email, int patientsId)
    {
        Assert.ThrowsAsync<NotFoundException>(() =>
            this.adminPatientController.EditPatient(patientsId, new PatientRequest() { Email = email }));
    }
}