using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using backend.Helpers;
using backend.Database;

namespace backend.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate next;
        private readonly JwtSettings appSettings;

        public JwtMiddleware(RequestDelegate next, IOptions<JwtSettings> appSettings)
        {
            this.next = next;
            this.appSettings = appSettings.Value;
        }

        public async Task Invoke(HttpContext context, DataContext dataContext)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                this.AttachUserToContext(context, dataContext, token);

            await this.next(context);
        }

        private void AttachUserToContext(HttpContext context, DataContext dataContext, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(this.appSettings.SecretToken);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // Set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);
                var accountType = Enum.Parse(typeof(AccountTypeEnum), 
                    jwtToken.Claims.First(x => x.Type == "accountType").Value);

                // Attach user to context on successful jwt validation
                context.Items["User"] = dataContext.Doctors.First(doctor => doctor.Id == userId);
                context.Items["AccountType"] = accountType;
            }
            catch
            {
                // Do nothing if jwt validation fails
                // User is not attached to context so request won't have access to secure routes
            }
        }
    }
}
