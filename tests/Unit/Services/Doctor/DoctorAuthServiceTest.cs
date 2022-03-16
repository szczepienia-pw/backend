using System.Linq;
using System.Threading.Tasks;
using backend.Dto.Requests;
using backend.Dto.Responses.Doctor;
using backend.Exceptions;
using backend.Helpers;
using backend.Models.Accounts;
using backend.Services.Doctor;
using backend_tests.Helpers;
using Microsoft.Extensions.Options;
using Xunit;

namespace backend_tests.Unit.Services.Doctor
{
    public class PatientAuthServiceTest
    {
        [Theory]
        [InlineData("john@doctor.com", "password1")]
        [InlineData("john@doctor1.com", "password")]
        [InlineData("john@patient.com", "password")]
        [InlineData("john@admin.com", "password")]
        public async Task TestAuthenticationWithWrongCredentials(string email, string password)
        {
            var service = new DoctorAuthService(
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
        [InlineData("john@doctor.com", "password")]
        public async Task TestAuthenticationWithCorrectCredentials(string email, string password)
        {
            var dataContext = DbHelper.GetMockedDataContextWithAccounts().Object;

            var service = new DoctorAuthService(
                Options.Create(new JwtSettings() { SecretToken = "super-random-and-long-secret-token" }),
                dataContext
            );

            var response = (AuthenticateResponse)await service.Authenticate(new AuthenticateRequest()
            {
                Email = email,
                Password = password
            });

            Assert.IsType<AuthenticateResponse>(response);
            DoctorModel loggedDoctor = dataContext.Doctors.First();

            Assert.Same(loggedDoctor, response.Doctor);
            Assert.NotNull(response.Token);
        }
    }
}
