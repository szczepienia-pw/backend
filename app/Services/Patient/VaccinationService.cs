using System.Diagnostics;
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
using Microsoft.EntityFrameworkCore;
using backend.Dto.Requests.Patient;
using Microsoft.AspNetCore.Mvc;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

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
    }
}

