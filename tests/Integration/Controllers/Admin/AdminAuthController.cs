using backend.Controllers.Admin;
using backend.Dto.Requests;
using backend.Exceptions;
using backend.Helpers;
using backend.Services.Admin;
using backend_tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Xunit;

namespace backend_tests.Admin;

public partial class AdminAuthTest
{
    [Theory]
    [InlineData("john@admin.com", "password")]
    public void ItAuthenticateWithGoodCredentials(string email, string password)
    {
        var response = this.adminAuthController.Login(new AuthenticateRequest() { Email = email, Password = password })
            .Result;

        Assert.IsType<OkObjectResult>(response);
        Assert.Equal(200, ((OkObjectResult) response).StatusCode);
    }

    [Theory]
    [InlineData("invalid@in.valid", "wrongpasswd")]
    public void ItAuthenticationFailureForBadCredentials(string email, string password)
    {
        Assert.ThrowsAsync<UnauthorizedException>(() =>
            this.adminAuthController.Login(new AuthenticateRequest() { Email = email, Password = password }));
    }
}