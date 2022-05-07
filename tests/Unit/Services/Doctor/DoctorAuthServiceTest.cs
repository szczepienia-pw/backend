using System.Linq;
using System.Threading.Tasks;
using backend.Controllers.Doctor;
using backend.Database;
using backend.Dto.Requests;
using backend.Dto.Responses.Doctor;
using backend.Exceptions;
using backend.Helpers;
using backend.Models.Accounts;
using backend.Services.Doctor;
using backend_tests.Helpers;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace backend_tests.Doctor
{
    public partial class DoctorAuthTest
    {
        private readonly Mock<DataContext> dbContext;
        private readonly DoctorAuthService doctorAuthService;
        private readonly DoctorAuthController doctorAuthController;
        
        public DoctorAuthTest()
        {
            this.dbContext = DbHelper.GetMockedDataContextWithAccounts();
            
            this.doctorAuthService = new DoctorAuthService(
                new JwtGenerator(Options.Create(new JwtSettings() { SecretToken = "super-random-and-long-secret-token" })),
                SecurePasswordHasherHelper.Hasher,
                this.dbContext.Object
            );

            this.doctorAuthController = new DoctorAuthController(this.doctorAuthService);
        }
        
        [Theory]
        [InlineData("john@doctor.com", "password1")]
        [InlineData("john@doctor1.com", "password")]
        [InlineData("john@patient.com", "password")]
        [InlineData("john@admin.com", "password")] 
        public async Task UtTestAuthenticationWithWrongCredentials(string email, string password)
        {
            await Assert.ThrowsAsync<UnauthorizedException>(
                () => this.doctorAuthService.Authenticate(new AuthenticateRequest()
                {
                    Email = email,
                    Password = password
                })
            );
        }

        [Theory]
        [InlineData("john@doctor.com", "password")]
        public async Task UtTestAuthenticationWithCorrectCredentials(string email, string password)
        {
            var response = (AuthenticateResponse)await this.doctorAuthService.Authenticate(new AuthenticateRequest()
            {
                Email = email,
                Password = password
            });

            Assert.IsType<AuthenticateResponse>(response);
            DoctorModel loggedDoctor = this.dbContext.Object.Doctors.First();

            Assert.Same(loggedDoctor, response.Doctor);
            Assert.NotNull(response.Token);
        }
    }
}
