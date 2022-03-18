using backend.Database;
using backend.Dto.Requests.Doctor;
using backend.Dto.Responses.Doctor;
using backend.Exceptions;
using backend.Models.Accounts;
using backend.Models.Visits;

namespace backend.Services.Doctor
{
    public class VaccinationSlotService
    {
        private readonly DataContext dataContext;
        public VaccinationSlotService(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<NewVaccinationSlotResponse> AddNewSlot(NewVaccinationSlotRequest request, DoctorModel doctor)
        {
            DateTime date;

            try
            {
                date = DateTime.Parse(request.Date);
            }
            catch (Exception e)
            {
                throw new ValidationException();
            }

            VaccinationSlotModel slot = new VaccinationSlotModel { Date = date, Doctor = doctor, Reserved = false };
            this.dataContext.Add(slot);
            this.dataContext.SaveChanges();
            return new NewVaccinationSlotResponse();
        }
    }
}
