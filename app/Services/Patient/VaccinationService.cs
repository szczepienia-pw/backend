using backend.Helpers;
using backend.Database;
using backend.Dto.Responses;
using backend.Models.Visits;
using backend.Exceptions;
using backend.Models.Accounts;
using backend.Models.Vaccines;

namespace backend.Services.Patient
{
	public class VaccinationService
	{
		private readonly DataContext dataContext;
        private readonly Mailer mailer;

        public VaccinationService(DataContext dataContext, Mailer mailer)
        {
            this.dataContext = dataContext;
            this.mailer = mailer;
        }

        public async Task<SuccessResponse> ReserveVaccinationSlot(PatientModel patient, int vaccinationSlotId, int vaccineId)
        {
            // Find matching vaccine
            VaccineModel vaccine = this.dataContext.Vaccines
                                       .FirstOrThrow(vaccine => vaccine.Id == vaccineId,
                                       new NotFoundException("Vaccine not found"));

            // Find slot with matching id
            VaccinationSlotModel slot = this.dataContext.VaccinationSlots
                                            .FirstOrThrow(slot => slot.Id == vaccinationSlotId,
                                            new NotFoundException("Slot not found"));

            // Check if slot is still available
            if(slot.Reserved)
            {
                throw new ConflictException("Slot is already taken.");
            }

            // Reserve slot
            slot.Reserved = true;

            // Create matching vaccine record
            VaccinationModel vaccination = new VaccinationModel()
            {
                VaccinationSlot = slot,
                Vaccine = vaccine,
                Doctor = slot.Doctor,
                Patient = patient,
                Status = StatusEnum.Planned
            };
            this.dataContext.Vaccinations.Add(vaccination);
            this.dataContext.SaveChanges();

            // Send email with confirmation
            await this.mailer.SendEmailAsync(
                patient.Email,
                "Vaccination visit confirmation",
                $"Your {vaccine.Disease.ToString()} vaccination visit on {slot.Date} is confirmed."
            );

            return new SuccessResponse();
        }
    }
}

