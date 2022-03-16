using System.Linq;
using System.Threading.Tasks;
using backend.Dto.Requests;
using backend.Dto.Responses.Admin;
using backend.Exceptions;
using backend.Helpers;
using backend.Models.Accounts;
using backend.Services.Admin;
using backend_tests.Helpers;
using Microsoft.Extensions.Options;
using Xunit;

namespace backend_tests.Unit.Services.Admin
{
    public class AdminAuthServiceTest
    {
        [Theory]
        [InlineData("john@admin.com", "password1")]
        [InlineData("john@admin1.com", "password")]
        [InlineData("john@patient.com", "password")]
        [InlineData("john@doctor.com", "password")]
        public async Task TestAuthenticationWithWrongCredentials(string email, string password)
        {
            var service = new AdminAuthService(
                new JwtGenerator(Options.Create(new JwtSettings() { SecretToken = "super-random-and-long-secret-token" })),
                SecurePasswordHasherHelper.Hasher,
                DbHelper.GetMockedDataContextWithAccounts().Object
            );

            await Assert.ThrowsAsync<UnauthorizedException>(
                () => service.Authenticate(new AuthenticateRequest()
                {
                    Email = email,
                    Password = password
                })
            );
        }

        [Theory]
        [InlineData("john@admin.com", "password")]
        public async Task TestAuthenticationWithCorrectCredentials(string email, string password)
        {
            var dataContext = DbHelper.GetMockedDataContextWithAccounts().Object;

            var service = new AdminAuthService(
                new JwtGenerator(Options.Create(new JwtSettings() { SecretToken = "super-random-and-long-secret-token" })),
                SecurePasswordHasherHelper.Hasher,
                dataContext
            );

            var response = (AuthenticateResponse)await service.Authenticate(new AuthenticateRequest()
            {
                Email = email,
                Password = password
            });

            Assert.IsType<AuthenticateResponse>(response);
            AdminModel loggedAdmin = dataContext.Admins.First();

            Assert.Same(loggedAdmin, response.Admin);
            Assert.NotNull(response.Token);
        }
    }
}
