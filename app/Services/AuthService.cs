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
        private readonly JwtGenerator jwtGenerator;
        private readonly SecurePasswordHasher securePasswordHasher;
        protected readonly DataContext dataContext;

        public AuthService(JwtGenerator jwtGenerator, SecurePasswordHasher securePasswordHasher, DataContext dataContext)
        {
            this.jwtGenerator = jwtGenerator;
            this.securePasswordHasher = securePasswordHasher;
            this.dataContext = dataContext;
        }

        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest request)
        {
            var user = this.GetDataContextDbSet().SingleOrDefault(user => user.Email == request.Email);

            // Return null if user not found or password is invalid
            if (user == null || !this.securePasswordHasher.Verify(request.Password, user.Password))
                throw new UnauthorizedException();

            // Authentication successful so generate jwt token
            var token = this.jwtGenerator.GenerateJwtToken(user.Id, user.GetEnum());

            return this.GetResponse(token, user);
        }

        protected abstract DbSet<T> GetDataContextDbSet();

        protected abstract AuthenticateResponse GetResponse(string token, T model);
    }
}
