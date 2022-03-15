using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using backend.Helpers;
using backend.Database;
using backend.Dto.Requests;
using backend.Dto.Responses;
using backend.Exceptions;
using backend.Models.Accounts;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public abstract class AuthService<T> where T : AccountModel
    {
        protected readonly JwtSettings appSettings;
        protected readonly DataContext dataContext;

        public AuthService(IOptions<JwtSettings> appSettings, DataContext dataContext)
        {
            this.appSettings = appSettings.Value;
            this.dataContext = dataContext;
        }

        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest request)
        {
            var user = this.GetDataContextDbSet().Single(user => user.Email == request.Email);

            // Return null if user not found or password is invalid
            if (user == null || !SecurePasswordHasher.Verify(request.Password, user.Password))
                throw new UnauthorizedException();

            // Authentication successful so generate jwt token
            var token = this.GenerateJwtToken(user);

            return this.GetResponse(token, user);
        }

        private string GenerateJwtToken(T user)
        {
            // Generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(this.appSettings.SecretToken);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()), 
                                                     new Claim("accountType", user.GetEnum().ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        protected abstract DbSet<T> GetDataContextDbSet();

        protected abstract AuthenticateResponse GetResponse(string token, T model);
    }
}
