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
            // Validate provided doctorId
            var doctor = this.dataContext.Doctors.FirstOrDefault(doctor => doctor.Id == doctorId);

            if (doctor == null)
                throw new NotFoundException();

            // Remove connections to vaccination slots
            var freeSlots = this.dataContext.VaccinationSlots.Where(slot => slot.Doctor.Id == doctorId &&
                                                                            slot.Reserved == false);
            this.dataContext.RemoveRange(freeSlots);

            var reservedSlots = this.dataContext.VaccinationSlots.Where(slot => slot.Doctor.Id == doctorId &&
                                                                                slot.Reserved == true &&
                                                                                slot.Date > DateTime.Now);
            this.dataContext.RemoveRange(reservedSlots); // Add patient notification, when visit cancellation logic is implemented.

            // Remove doctor
            (doctor as ISoftDelete).SoftDelete();
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
