using backend.Database;
using backend.Dto.Responses;
using backend.Exceptions;

namespace backend.Services.Admin
{
    public class AdminDoctorsService
    {
        private readonly DataContext dataContext;

        public AdminDoctorsService(DataContext dataContext)
        {
            this.dataContext = dataContext;
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
    }
}
