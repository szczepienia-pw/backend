using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using backend.Helpers;
using backend.Database;
using backend.Dto.Requests;
using backend.Dto.Responses.Doctor;
using backend.Exceptions;

namespace backend.Services.Doctor
{
    public class DoctorAuthService
    {
        private readonly JwtSettings appSettings;
        private readonly DataContext dataContext;

        public DoctorAuthService(IOptions<JwtSettings> appSettings, DataContext dataContext)
        {
            this.appSettings = appSettings.Value;
            this.dataContext = dataContext;
        }

        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest request)
        {
            var user = this.dataContext.Doctors
                .SingleOrDefault(doctor => doctor.Email == request.Email && doctor.Password == request.Password);

            // Return null if doctor not found
            if (user == null) throw new UnauthorizedException();

            // Authentication successful so generate jwt token
            var token = this.GenerateJwtToken(user);

            return new AuthenticateResponse(token, user);
        }

        private string GenerateJwtToken(Models.Accounts.DoctorModel user)
        {
            // Generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(this.appSettings.SecretToken);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
