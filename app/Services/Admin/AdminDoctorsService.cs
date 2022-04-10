using backend.Database;
using backend.Dto.Requests.Admin;
using backend.Dto.Requests.Admin.Doctor;
using backend.Dto.Responses;
using backend.Dto.Responses.Doctor;
using backend.Exceptions;
using backend.Helpers;
using backend.Models.Accounts;

namespace backend.Services.Admin
{
    public class AdminDoctorsService
    {
        private readonly DataContext dataContext;
        private readonly SecurePasswordHasher securePasswordHasher;

        private void ValidateEmail(string email, int? id = null)
        {
            var existingDoctor = this.dataContext.Doctors.FirstOrDefault(doctor => doctor.Email == email);

            if (existingDoctor != null)
            {
                if (id != null && id == existingDoctor.Id)
                    // allow to change the e-mail to the same e-mail if the object being updated is the same
                    return;

                throw new ValidationException($"E-mail address: {email} is already in use.");
            }
        }

        public AdminDoctorsService(DataContext dataContext, SecurePasswordHasher securePasswordHasher)
        {
            this.dataContext = dataContext;
            this.securePasswordHasher = securePasswordHasher;
        }

        public async Task<SuccessResponse> DeleteDoctor(int doctorId)
        {
            // Validate provided doctorId
            var doctor = this.dataContext.Doctors.FirstOrThrow(doctor => doctor.Id == doctorId,
                                                               new NotFoundException());

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
            this.dataContext.Doctors.CheckDuplicate(doctor => doctor.Email == request.Email,
                                                    new ValidationException("E-mail is already in use."));

            DoctorModel doctor = new DoctorModel() {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = securePasswordHasher.Hash(request.Password)
            };

            this.dataContext.Add(doctor);
            this.dataContext.SaveChanges();

            return doctor;
        }

        public async Task<DoctorModel> EditDoctor(int doctorId, EditDoctorRequest request)
        {
            var doctor = this.dataContext.Doctors.FirstOrThrow(doctor => doctor.Id == doctorId,
                                                               new NotFoundException());

            if (request.FirstName != null)
                doctor.FirstName = request.FirstName;

            if (request.LastName != null)
                doctor.LastName = request.LastName;

            if (request.Email != null)
            {
                this.ValidateEmail(request.Email, doctorId);
                doctor.Email = request.Email;
            }

            this.dataContext.Doctors.Update(doctor);
            this.dataContext.SaveChanges();

            return doctor;
        }

        public async Task<DoctorResponse> ShowDoctor(int doctorId)
        {
            var doctor = this.dataContext.Doctors.FirstOrThrow(doctor => doctor.Id == doctorId,
                                                               new NotFoundException());
            return new DoctorResponse(doctor);
        }

        public async Task<PaginatedResponse<DoctorModel, List<DoctorResponse>>> ShowDoctors(int page)
        {
            var doctors = this.dataContext.Doctors;
            var doctorList = PaginatedList<DoctorModel>.Paginate(doctors, page);

            return new PaginatedResponse<DoctorModel, List<DoctorResponse>>(
                doctorList,
                doctorList.Select(doctor => new DoctorResponse(doctor)).ToList()
            );
        }
    }
}
