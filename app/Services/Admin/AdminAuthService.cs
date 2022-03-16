using Microsoft.Extensions.Options;

using backend.Helpers;
using backend.Database;
using backend.Dto.Responses.Admin;
using backend.Models.Accounts;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Admin
{
    public class AdminAuthService : AuthService<AdminModel>
    {
        public AdminAuthService(JwtGenerator jwtGenerator, SecurePasswordHasher securePasswordHasher, DataContext dataContext) 
            : base(jwtGenerator, securePasswordHasher, dataContext) { }

        protected override DbSet<AdminModel> GetDataContextDbSet()
        {
            return this.dataContext.Admins;
        }

        protected override AuthenticateResponse GetResponse(string token, AdminModel model)
        {
            return new AuthenticateResponse(token, model);
        }
    }
}
