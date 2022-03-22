using backend.Database;
using backend.Dto.Requests.Admin;
using backend.Dto.Responses;
using backend.Exceptions;
using backend.Helpers;
using backend.Models.Accounts;

namespace backend.Services.Admin
{
    public class AdminDoctorsService
    {
        private readonly DataContext dataContext;
        private readonly SecurePasswordHasher securePasswordHasher;

        public AdminDoctorsService(DataContext dataContext, SecurePasswordHasher securePasswordHasher)
        {
            this.dataContext = dataContext;
            this.securePasswordHasher = securePasswordHasher;
        }

        public async Task<SuccessResponse> DeleteDoctor(int doctorId)
        {
            var doctor = this.dataContext.Doctors.FirstOrDefault(doctor => doctor.Id == doctorId);

            if (doctor == null)
                throw new NotFoundException();

            this.dataContext.Doctors.Remove(doctor);
            this.dataContext.SaveChanges();

            return new SuccessResponse();
        }

        public async Task<DoctorModel> CreateDoctor(CreateDoctorRequest request)
        {
            var existingDoctor = this.dataContext.Doctors.FirstOrDefault(doctor => doctor.Email == request.Email);

            if (existingDoctor != null)
                throw new ValidationException("E-mail is already in use.");

            DoctorModel doctor = new DoctorModel() {
                FirstName = request.FirstName,
                LastName = request.SecondName,
                Email = request.Email,
                Password = securePasswordHasher.Hash(request.Password)
            };

            this.dataContext.Add(doctor);
            this.dataContext.SaveChanges();

            return doctor;
        }
    }
}
