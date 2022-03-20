using Microsoft.Extensions.Options;

using backend.Helpers;
using backend.Database;
using backend.Dto.Responses.Patient;
using backend.Models.Accounts;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Patient
{
    public class PatientAuthService : AuthService<PatientModel>
    {
        public PatientAuthService(JwtGenerator jwtGenerator, SecurePasswordHasher securePasswordHasher, DataContext dataContext)
            : base(jwtGenerator, securePasswordHasher, dataContext) { }

        protected override DbSet<PatientModel> GetDataContextDbSet()
        {
            return this.dataContext.Patients;
        }

        protected override AuthenticateResponse GetResponse(string token, PatientModel model)
        {
            this.dataContext.Entry(model)?.Reference("Address")?.Load();
            return new AuthenticateResponse(token, model);
        }
    }
}
