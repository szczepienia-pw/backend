using backend.Helpers;
using backend.Database;
using backend.Dto.Requests.Patient;
using backend.Dto.Responses;
using backend.Models.Visits;
using backend.Exceptions;
using backend.Models.Accounts;
using backend.Models.Vaccines;
using backend.Dto.Responses.Patient.Vaccination;
using backend.Helpers.PdfGenerators;
using backend.Models.Accounts.AdditionalData;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.Patient
{
	public class PatientVaccinationService
	{
        private readonly DataContext dataContext;
        private readonly Mailer mailer;


        public PatientVaccinationService(DataContext dataContext, Mailer mailer)
        {
            this.dataContext = dataContext;
            this.mailer = mailer;
        }

        public async Task<ShowAvailableVaccinesResponse> ShowAvailableVaccines(ShowVaccinesRequest request)
        {
            List<DiseaseEnum> diseases = new List<DiseaseEnum>();
            foreach (var disease in request.Disease.Split(','))
                diseases.Add(DiseaseEnumAdapter.ToEnum(disease));

            // Find vaccines for given disese
            List<VaccineModel> vaccines = this.dataContext.Vaccines
                                              .Where(vaccine => diseases.Contains(vaccine.Disease))
                                              .ToList();

            // Return list of vaccines
            return new ShowAvailableVaccinesResponse(vaccines);
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
            Semaphores.slotSemaphore.WaitOne();

            // Check if slot is still available
            if (slot.Reserved)
            {
                Semaphores.slotSemaphore.Release();
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
            Semaphores.slotSemaphore.Release();

            // Send email with confirmation
            _ = this.mailer.SendEmailAsync(
                    patient.Email,
                    "Vaccination visit confirmation",
                    $"Your {vaccine.Disease.ToString()} vaccination visit on {slot.Date.ToShortDateString()} is confirmed."
            );

            return new SuccessResponse();
        }

        public async Task<SuccessResponse> CancelVaccinationSlot(PatientModel patient, int vaccinationSlotId)
        {
            // Find slot with matching id
            VaccinationSlotModel slot = this.dataContext
                .VaccinationSlots
                .FirstOrThrow(slot => slot.Id == vaccinationSlotId, new NotFoundException("Slot not found"));

            // Get vaccination connected to found vaccination slot
            VaccinationModel? vaccinationForSlot =
                this.dataContext
                    .Vaccinations
                    .Include(vaccination => vaccination.Vaccine)
                    .FirstOrThrow(
                        vaccination => vaccination.VaccinationSlotId == slot.Id && vaccination.PatientId == patient.Id && vaccination.Status == StatusEnum.Planned,
                        new ConflictException("Specified vaccination slot does not belong to you")
                    );

            if (!slot.Reserved)
                throw new ConflictException("You cannot cancel not reserved vaccination slot");
            
            vaccinationForSlot.Status = StatusEnum.Canceled;

            // Duplicate vaccination slot
            VaccinationSlotModel newSlot = new VaccinationSlotModel { Date = slot.Date, Doctor = slot.Doctor, Reserved = false };
            this.dataContext.Add(newSlot);
            
            this.dataContext.SaveChanges();

            // Send email with confirmation
            _ = this.mailer.SendEmailAsync(
                      patient.Email,
                      "Vaccination visit canceled",
                      $"Your {vaccinationForSlot.Vaccine.Disease.ToString()} vaccination visit on {slot.Date.ToShortDateString()} has been canceled."
            );

            return new SuccessResponse();
        }

        public async Task<PaginatedResponse<VaccinationModel, List<VaccinationResponse>>> GetVaccinationsHistory(PatientModel patient, FilterVaccinationsRequest request)
        {
            var vaccinations = this.dataContext
                .Vaccinations
                .Where(vaccination => vaccination.PatientId == patient.Id)
                .Include(vaccination => vaccination.VaccinationSlot)
                .OrderByDescending(vaccination => vaccination.Id);

            var paginatedVaccinations = PaginatedList<VaccinationModel>.Paginate(vaccinations, request.Page);

            return new PaginatedResponse<VaccinationModel, List<VaccinationResponse>>(
                paginatedVaccinations,
                paginatedVaccinations.Select(vaccination => new VaccinationResponse(vaccination)).ToList()
            );
        }

        public byte[] DownloadVaccinationCertificate(PatientModel patient, int vaccinationId, bool generateQrCode = true)
        {
            // Find vaccination with matching id
            VaccinationModel vaccination = this.dataContext.Vaccinations.FirstOrThrow(vaccination => vaccination.Id == vaccinationId && vaccination.Patient == patient,
                                                                                      new NotFoundException("Vaccination entry not found"));

            // Check if vaccination process is finished
            if (vaccination.Status != StatusEnum.Completed)
                throw new ConflictException("Vaccination has not been taken yet.");

            // Generate PDF and return byte array
            return CertificateGenerator.GeneratePDF(vaccination, generateQrCode);
        }
        
        public async Task<SuccessResponse> DeletePatient(PatientModel patient)
        {
            var slots = this.dataContext.VaccinationSlots
                .Include(vs => vs.Doctor)
                .Include(vs => vs.Vaccination)
                .Join(
                    this.dataContext.Vaccinations.AsQueryable(),
                    vaccinationSlot => vaccinationSlot.Id,
                    vaccination => vaccination.VaccinationSlotId,
                    (vaccinationSlot, vaccination) =>
                        new
                        {
                            PatientId = vaccination.PatientId, vaccination = vaccination,
                            vaccinationSlot = vaccinationSlot
                        })
                .Where(t =>
                    t.PatientId == patient.Id &&
                    t.vaccination.Status == StatusEnum.Planned &&
                    t.vaccinationSlot.Reserved &&
                    t.vaccinationSlot.Date >= DateTime.Now);
            var address = this.dataContext.Addresses.First(a => a.Id == patient.AddressId);

            // free all reserved slots
            foreach (var slot in slots)
            {
                slot.vaccination.Status = StatusEnum.Canceled;

                // Duplicate vaccination slot
                VaccinationSlotModel newSlot = new VaccinationSlotModel { Date = slot.vaccinationSlot.Date, Doctor = slot.vaccinationSlot.Doctor, Reserved = false };
                this.dataContext.Add(newSlot);
            }

            ((ISoftDelete)patient).SoftDelete();
            this.dataContext.SaveChanges();

            return new SuccessResponse();
        }
    }
}

