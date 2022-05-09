using backend.Dto.Requests;
using backend.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace backend_tests.Patient;

public partial class PatientAuthTest
{
    [Theory]
    [InlineData("john@patient.com", "password")]
    public void ItSuccessfullyAuthenticatesPatient(string email, string password)
    {
        var rsp = this.patientAuthController.Login(new AuthenticateRequest() { Email = email, Password = password })
            .Result;

        Assert.IsType<OkObjectResult>(rsp);
        Assert.Equal(200, ((OkObjectResult) rsp).StatusCode);
    }

    [Theory]
    [InlineData("invalid@credentials.com", "wrong_password")]
    public void ItFailedToAuthenticatePatient(string email, string password)
    {
        Assert.ThrowsAsync<UnauthorizedException>(() => this.patientAuthController.Login(new AuthenticateRequest()
            { Email = email, Password = password }));
    }
}