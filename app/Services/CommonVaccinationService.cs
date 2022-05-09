using backend.Database;
using backend.Models.Visits;
using backend.Dto.Responses.Patient.Vaccination;

namespace backend.Services
{
    public class CommonVaccinationService
    {
        private readonly DataContext dataContext;

        public CommonVaccinationService(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }
        public async Task<List<AvailableSlotResponse>> GetAvailableVaccinationSlots()
        {
            // Find available vaccination slots
            List<VaccinationSlotModel> slots = this.dataContext.VaccinationSlots
                                              .Where(slot => slot.Reserved == false &&
                                                             slot.Date >= DateTime.Now)
                                              .ToList();

            // Return list of slots
            return slots.Select(slot => new AvailableSlotResponse(slot)).ToList();
        }
    }
}