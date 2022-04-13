using backend.Helpers;
using backend.Database;
using backend.Dto.Requests.Patient;
using backend.Dto.Responses;
using backend.Models.Visits;
using backend.Exceptions;
using backend.Models.Accounts;
using backend.Models.Vaccines;
using backend.Dto.Responses.Patient.Vaccination;
using backend.Dto.Responses.Common.Vaccination;

namespace backend.Services.Patient
{
	public class VaccinationService
	{
        private static Semaphore semaphore = new Semaphore(1, 1);

        private readonly DataContext dataContext;
        private readonly Mailer mailer;


        public VaccinationService(DataContext dataContext, Mailer mailer)
        {
            this.dataContext = dataContext;
            this.mailer = mailer;
        }

        public async Task<List<VaccineResponse>> ShowAvailableVaccines(DiseaseEnum disease)
        {
            // Find vaccines for given disese
            List<VaccineModel> vaccines = this.dataContext.Vaccines
                                              .Where(vaccine => vaccine.Disease == disease)
                                              .ToList();

            // Return list of vaccines
            return vaccines.Select(vaccine => new VaccineResponse(vaccine)).ToList();
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

            // Block concurrent access
            VaccinationService.semaphore.WaitOne();

            // Check if slot is still available
            if (slot.Reserved)
            {
                VaccinationService.semaphore.Release();
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

            // Release semaphore
            VaccinationService.semaphore.Release();

            // Send email with confirmation
            await this.mailer.SendEmailAsync(
                patient.Email,
                "Vaccination visit confirmation",
                $"Your {vaccine.Disease.ToString()} vaccination visit on {slot.Date} is confirmed."
            );

            return new SuccessResponse();
        }

        public async Task<PaginatedResponse<VaccinationModel, List<VaccinationResponse>>> GetVaccinationsHistory(PatientModel patient, FilterVaccinationsRequest request)
        {
            var vaccinations = this.dataContext.Vaccinations.Where(vaccination => vaccination.PatientId == patient.Id);

            var paginatedVaccinations = PaginatedList<VaccinationModel>.Paginate(vaccinations, request.Page);

            return new PaginatedResponse<VaccinationModel, List<VaccinationResponse>>(
                paginatedVaccinations,
                paginatedVaccinations.Select(vaccination => new VaccinationResponse(vaccination)).ToList()
            );
        }
    }
}

