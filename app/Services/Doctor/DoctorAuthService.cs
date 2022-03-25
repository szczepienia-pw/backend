using Microsoft.Extensions.Options;

using backend.Helpers;
using backend.Database;
using backend.Dto.Responses.Doctor;
using backend.Models.Accounts;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Doctor
{
    public class DoctorAuthService : AuthService<DoctorModel>
    {


        public DoctorAuthService(JwtGenerator jwtGenerator, SecurePasswordHasher securePasswordHasher, DataContext dataContext)
            : base(jwtGenerator, securePasswordHasher, dataContext) { }

        protected override DbSet<DoctorModel> GetDataContextDbSet()
        {
            return this.dataContext.Doctors;
        }

        protected override AuthenticateResponse GetResponse(string token, DoctorModel model)
        {
            return new AuthenticateResponse(token, model);
        }
    }
}
