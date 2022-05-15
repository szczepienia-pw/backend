using backend.Database;
using backend.Dto.Responses;
using backend.Exceptions;
using backend.Helpers;
using backend.Models.Visits;

namespace backend.Services.Admin
{
    public class AdminVaccinationService
    {
        private readonly DataContext dataContext;
        private readonly Mailer mailer;

        public AdminVaccinationService(DataContext dataContext, Mailer mailer)
        {
            this.dataContext = dataContext;
            this.mailer = mailer;
        }

        public async Task<SuccessResponse> ChangeVaccinationSlot(int vaccinationId, int newVaccinationSlotId)
        {
            // Block concurrent access
            Semaphores.slotSemaphore.WaitOne();

            // Find vaccination visit with matching id
            VaccinationModel vaccination = this.dataContext
                .Vaccinations
                .FirstOrThrow(vaccination => vaccination.Id == vaccinationId, new NotFoundException("Vaccination visit not found"), true);

            // Get slot connected to found vaccination visit
            VaccinationSlotModel currentSlot = this.dataContext
                .VaccinationSlots
                .FirstOrThrow(slot => slot.Id == vaccination.VaccinationSlotId, new NotFoundException("Current vaccination slot not found"), true);

            // Get new slot
            VaccinationSlotModel newSlot = this.dataContext
                .VaccinationSlots
                .FirstOrThrow(slot => slot.Id == newVaccinationSlotId, new NotFoundException("New vaccination slot not found"), true);

            // Check if visit is still pending
            if (vaccination.Status != StatusEnum.Planned)
            {
                Semaphores.slotSemaphore.Release();
                throw new ConflictException("Visit process has already been finished.");
            }

            // Check if new slot is still available
            if (newSlot.Reserved)
            {
                Semaphores.slotSemaphore.Release();
                throw new ConflictException("Slot is already taken.");
            }

            // Change slots
            newSlot.Reserved = true;
            currentSlot.Reserved = false;

            vaccination.VaccinationSlot = newSlot;
            vaccination.VaccinationSlotId = newSlot.Id;

            currentSlot.Vaccination = null;
            newSlot.Vaccination = vaccination;

            vaccination.Doctor = newSlot.Doctor;
            vaccination.DoctorId = newSlot.Doctor.Id;

            // Save changes
            this.dataContext.SaveChanges();

            // Release semaphore
            Semaphores.slotSemaphore.Release();

            // Send email with confirmation
            _ = this.mailer.SendEmailAsync(
                    vaccination.Patient.Email,
                    "Vaccination visit slot changed",
                    $"Your {vaccination.Vaccine.Disease.ToString()} vaccination visit on {currentSlot.Date.ToShortDateString()} was changed to {newSlot.Date.ToShortDateString()} by System Administrator."
            );

            return new SuccessResponse();
        }
    }
}
