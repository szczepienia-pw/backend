using System.Linq;
using System.Threading.Tasks;
using backend.Controllers.Admin;
using backend.Database;
using backend.Dto.Requests;
using backend.Dto.Responses.Admin;
using backend.Exceptions;
using backend.Helpers;
using backend.Models.Accounts;
using backend.Services.Admin;
using backend_tests.Helpers;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace backend_tests.Admin
{
    public partial class AdminAuthTest
    {
        private readonly AdminAuthController adminAuthController;
        private readonly AdminAuthService authService;
        private readonly Mock<DataContext> dbContext;

        public AdminAuthTest()
        {
            this.dbContext = DbHelper.GetMockedDataContextWithAccounts();
            
            this.authService = new AdminAuthService(
                new JwtGenerator(Options.Create(new JwtSettings() { SecretToken = "super-random-and-long-secret-token" })),
                SecurePasswordHasherHelper.Hasher,
                this.dbContext.Object
            );

            this.adminAuthController = new AdminAuthController(authService);
        }
        
        [Theory]
        [InlineData("john@admin.com", "password1")]
        [InlineData("john@admin1.com", "password")]
        [InlineData("john@patient.com", "password")]
        [InlineData("john@doctor.com", "password")]
        public async Task UtTestAuthenticationWithWrongCredentials(string email, string password)
        {
            await Assert.ThrowsAsync<UnauthorizedException>(
                () => this.authService.Authenticate(new AuthenticateRequest()
                {
                    Email = email,
                    Password = password
                })
            );
        }

        [Theory]
        [InlineData("john@admin.com", "password")]
        public async Task UtTestAuthenticationWithCorrectCredentials(string email, string password)
        {
            var response = (AuthenticateResponse)await this.authService.Authenticate(new AuthenticateRequest()
            {
                Email = email,
                Password = password
            });

            Assert.IsType<AuthenticateResponse>(response);
            AdminModel loggedAdmin = this.dbContext.Object.Admins.First();

            Assert.Same(loggedAdmin, response.Admin);
            Assert.NotNull(response.Token);
        }
    }
}
