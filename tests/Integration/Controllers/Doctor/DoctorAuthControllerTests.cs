using backend.Dto.Requests;
using backend.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace backend_tests.Doctor;

public partial class DoctorAuthTest
{
    [Theory]
    [InlineData("john@doctor.com", "password")]
    public void ItSuccessfullyAuthenticate(string email, string password)
    {
        var rsp = this.doctorAuthController.Login(new AuthenticateRequest() { Email = email, Password = password })
            .Result;

        Assert.IsType<OkObjectResult>(rsp);
        Assert.Equal(200, ((OkObjectResult) rsp).StatusCode);
    }

    [Theory]
    [InlineData("invalid@login.com", "wrong_password")]
    public void ItFailedToAuthenticate(string email, string password)
    {
        Assert.ThrowsAsync<UnauthorizedException>(() => this.doctorAuthController.Login(new AuthenticateRequest() {Email = email, Password = password}));
    }
}