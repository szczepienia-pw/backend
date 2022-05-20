using System.Collections.Generic;
using System.Linq;
using backend.Dto.Requests.Patient;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Moq;
using Xunit;

namespace backend_tests.Patient;

public partial class PatientTest
{
    [Theory]
    [InlineData("Warszawa", "00-528", "Hoża", "5a", "3", "BB", "BB", "bb@hot.com", "passwd", "67062675913")]
    public void ItSuccessfulyRegisterPatient(params string?[] input)
    {
        var patientRequest = this.TestInputParse<PatientRegistrationRequest, PatientRegistrationAddressRequest>(input);
        patientRequest.Password = this.securePasswordHasherMock.Hash(input[8]);
        var rsp = this.patientController.Registration(patientRequest).Result;

        Assert.IsType<OkObjectResult>(rsp);
        Assert.Equal(200, ((OkObjectResult) rsp).StatusCode);
    }
    
    [Theory]
    [InlineData("Warszawa", "00-528", "Hoża", "5a", "3", "BB", "BB", "bb@hot.com", "passwd", "00000000000")]
    public void ItFailToRegisterPatient(params string?[] input)
    {
        var patientRequest = this.TestInputParse<PatientRegistrationRequest, PatientRegistrationAddressRequest>(input);
        Assert.ThrowsAsync<ValidationException>(() => this.patientController.Registration(patientRequest));
    }

    [Theory]
    [InlineData(1, "BB")]
    public void ItSuccessfullyEditPatientsData(int patientsId, string name)
    {
        var patient = this.dataContextMock.Object.Patients.First(p => p.Id == patientsId);
        this.patientController.HttpContext.Items.Add("User", patient);

        var rsp = this.patientController.EditPatient(new PatientRequest() { FirstName = name }).Result;

        Assert.IsType<OkObjectResult>(rsp);
        Assert.Equal(200, ((OkObjectResult) rsp).StatusCode);
    }
    
    [Theory]
    [InlineData(1, "00000000000")]
    public void ItFailToEditPatientsData(int patientsId, string pesel)
    {
        var patient = this.dataContextMock.Object.Patients.First(p => p.Id == patientsId);
        this.patientController.HttpContext.Items.Add("User", patient);
        Assert.ThrowsAsync<ValidationException>(() => this.patientController.EditPatient(new PatientRequest() {Pesel = pesel}));
    }

    [Fact]
    public void SuccessfullyConfirmsRegistration()
    {
        var patient = this.dataContextMock.Object.Patients.First();
        var rsp = this.patientController
            .ConfirmRegistration(new ConfirmRegistrationRequest() { Token = patient.VerificationToken }).Result;

        Assert.IsType<OkObjectResult>(rsp);
        Assert.Equal(200, ((OkObjectResult) rsp).StatusCode);
    }
}