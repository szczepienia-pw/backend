using backend.Database;
using backend.Dto.Requests.Admin;
using backend.Dto.Responses;
using backend.Dto.Responses.Admin.Vaccination;
using backend.Exceptions;
using backend.Helpers;
using backend.Helpers.PdfGenerators;
using backend.Models.Vaccines;
using backend.Models.Visits;
using Microsoft.EntityFrameworkCore;

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

            // Send email notification to patient
            _ = this.mailer.SendEmailAsync(
                    vaccination.Patient.Email,
                    "Vaccination visit slot changed",
                    $"Your {vaccination.Vaccine.Disease.ToString()} vaccination visit on {currentSlot.Date.ToShortDateString()} was changed to {newSlot.Date.ToShortDateString()} by System Administrator."
            );

            _ = this.mailer.SendEmailAsync(
                    vaccination.Doctor.Email,
                    "Vaccination visit slot changed",
                    $"Vaccination visit of {vaccination.Patient.FirstName} {vaccination.Patient.LastName} ({vaccination.Vaccine.Disease.ToString()}) was moved from {currentSlot.Date.ToShortDateString()} to {newSlot.Date.ToShortDateString()} by System Administrator."
            );

            return new SuccessResponse();
        }

        public async Task<PaginatedResponse<GetVaccinationsResponse, List<GetVaccinationsResponse>>> GetVaccinations(FilterVaccinationsRequest request)
        {
            var vaccinations = this.dataContext.Vaccinations
                                               .Include("Vaccine")
                                               .Include("VaccinationSlot")
                                               .Include("Patient")
                                               .Include("Patient.Address")
                                               .Include("Doctor")
                                               .AsQueryable();

            if (request.Disease != null)
            {
                DiseaseEnum diseaseEnum = DiseaseEnumAdapter.ToEnum(request.Disease);
                vaccinations = vaccinations.Where(visit => visit.Vaccine.Disease == diseaseEnum);
            }
            if (request.DoctorId != null)
                vaccinations = vaccinations.Where(visit => visit.Doctor.Id == request.DoctorId);
            if (request.PatientId != null)
                vaccinations = vaccinations.Where(visit => visit.Patient.Id == request.PatientId);

            var paginatedVaccinations = PaginatedList<GetVaccinationsResponse>.Paginate(vaccinations.Select(visit => new GetVaccinationsResponse(visit)), request.Page);

            return new PaginatedResponse<GetVaccinationsResponse, List<GetVaccinationsResponse>>(paginatedVaccinations, paginatedVaccinations);
        }

        public async Task<VaccinationsReportResponse> GetVaccinationsReport(VaccinationsReportRequest request)
        {
            var result = this.dataContext
                .Vaccinations
                .Where(vaccination => vaccination.Status == StatusEnum.Completed)
                .Where(vaccination => vaccination.VaccinationSlot.Date >= DateTime.Parse(request.StartDate))
                .Where(vaccination => vaccination.VaccinationSlot.Date <= DateTime.Parse(request.EndDate))
                .GroupBy(vaccination => new {vaccination.Vaccine.Disease, vaccination.Vaccine.Name})
                .Select(result => new
                    {disease = result.Key.Disease, vaccineName = result.Key.Name, count = result.Count()})
                .AsEnumerable()
                .GroupBy(result => result.disease);
            
            var diseasesReport = result
                .Select(disease => new DiseaseReportResponse(
                    disease.Key.ToString(),
                    disease.Count(),
                    disease.Select(vaccine => new VaccineReportResponse(vaccine.vaccineName, vaccine.count)).ToList()
                ))
                .ToList();

            return new VaccinationsReportResponse(diseasesReport);
        }
        
        public byte[] DownloadVaccinationsReport(VaccinationsReportRequest request)
        {
            var result = this.dataContext
                .Vaccinations
                .Where(vaccination => vaccination.Status == StatusEnum.Completed)
                .Where(vaccination => vaccination.VaccinationSlot.Date >= DateTime.Parse(request.StartDate))
                .Where(vaccination => vaccination.VaccinationSlot.Date <= DateTime.Parse(request.EndDate))
                .OrderBy(vaccination => vaccination.VaccinationSlot.Date)
                .ToList();

            // Generate PDF and return byte array
            return ReportGenerator.GeneratePDF(
                result, 
                DateTime.Parse(request.StartDate).ToString(), 
                DateTime.Parse(request.EndDate).ToString()
            );
        }
    }
}
