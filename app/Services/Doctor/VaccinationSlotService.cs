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
        private readonly int slotMarginMins = 15;
        private readonly DataContext dataContext;

        public VaccinationSlotService(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<NewVaccinationSlotResponse> AddNewSlot(NewVaccinationSlotRequest request, DoctorModel doctor)
        {
            // Validate date from request
            DateTime date;

            try
            {
                date = DateTime.Parse(request.Date);
            }
            catch (Exception e)
            {
                throw new ValidationException();
            }

            // Check for overlapping slots
            var doctorSlots = this.dataContext.VaccinationSlots.Where(slot => slot.Doctor.Id == doctor.Id &&
                                                                      slot.Date > date.AddMinutes(-slotMarginMins) &&
                                                                      slot.Date < date.AddMinutes(slotMarginMins));
            if (doctorSlots.Count() > 0)
                throw new ValidationException();

            // Add new slot to database
            VaccinationSlotModel slot = new VaccinationSlotModel { Date = date, Doctor = doctor, Reserved = false };
            this.dataContext.Add(slot);
            this.dataContext.SaveChanges();
            return new NewVaccinationSlotResponse();
        }
    }
}
