using System.Linq;
using System.Threading.Tasks;
using backend.Controllers.Patient;
using backend.Database;
using backend.Dto.Requests;
using backend.Dto.Responses.Patient;
using backend.Exceptions;
using backend.Helpers;
using backend.Models.Accounts;
using backend.Services.Patient;
using backend_tests.Helpers;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace backend_tests.Patient
{
    public partial class PatientAuthTest
    {
        private readonly Mock<DataContext> dbContext;
        private readonly PatientAuthService patientAuthService;
        private readonly PatientAuthController patientAuthController;
        
        public PatientAuthTest()
        {
            this.dbContext = Helpers.DbHelper.GetMockedDataContextWithAccounts();
            this.patientAuthService = new PatientAuthService(
                new JwtGenerator(Options.Create(new JwtSettings() { SecretToken = "super-random-and-long-secret-token" })),
                SecurePasswordHasherHelper.Hasher,
                this.dbContext.Object
            );
            this.patientAuthController = new PatientAuthController(this.patientAuthService);
        }
        
        [Theory]
        [InlineData("john@patient.com", "password1")]
        [InlineData("john@patient1.com", "password")]
        [InlineData("john@doctor.com", "password")]
        [InlineData("john@admin.com", "password")]
        [InlineData("john3@admin.com", "password")] // Not verified
        public async Task UtTestAuthenticationWithWrongCredentials(string email, string password)
        {
            await Assert.ThrowsAsync<UnauthorizedException>(
                () => this.patientAuthService.Authenticate(new AuthenticateRequest()
                {
                    Email = email,
                    Password = password
                })
            );
        }

        [Theory]
        [InlineData("john@patient.com", "password")]
        public async Task UtTestAuthenticationWithCorrectCredentials(string email, string password)
        {
            PatientModel loggedPatient = this.dbContext.Object.Patients.First();
            var response = (AuthenticateResponse)await this.patientAuthService.Authenticate(new AuthenticateRequest()
            {
                Email = email,
                Password = password
            });

            Assert.IsType<AuthenticateResponse>(response);

            Assert.Same(loggedPatient.Email, response.Patient.Email);
            Assert.NotNull(response.Token);
        }
    }
}
