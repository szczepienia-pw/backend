using System.Linq;
using System.Threading.Tasks;
using backend.Dto.Requests;
using backend.Dto.Responses.Patient;
using backend.Exceptions;
using backend.Helpers;
using backend.Models.Accounts;
using backend.Services.Patient;
using backend_tests.Helpers;
using Microsoft.Extensions.Options;
using Xunit;

namespace backend_tests.Unit.Services.Patient
{
    public class PatientAuthServiceTest
    {
        [Theory]
        [InlineData("john@patient.com", "password1")]
        [InlineData("john@patient1.com", "password")]
        [InlineData("john@doctor.com", "password")]
        [InlineData("john@admin.com", "password")]
        public async Task TestAuthenticationWithWrongCredentials(string email, string password)
        {
            var service = new PatientAuthService(
                Options.Create(new JwtSettings() { SecretToken = "super-random-and-long-secret-token" }), 
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
        [InlineData("john@patient.com", "password")]
        public async Task TestAuthenticationWithCorrectCredentials(string email, string password)
        {
            var dataContextMock = DbHelper.GetMockedDataContextWithAccounts();
            PatientModel loggedPatient = dataContextMock.Object.Patients.First();

            var service = new PatientAuthService(
                Options.Create(new JwtSettings() { SecretToken = "super-random-and-long-secret-token" }),
                dataContextMock.Object
            );

            var response = (AuthenticateResponse)await service.Authenticate(new AuthenticateRequest()
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
