using backend.Database;
using backend.Dto.Responses;
using backend.Dto.Responses.Patient;
using backend.Exceptions;
using backend.Helpers;
using backend.Models.Accounts;
using backend.Models.Visits;
using System.Linq;

namespace backend.Services.Admin
{
    public class AdminPatientsService
    {
        private readonly DataContext dataContext;
        public AdminPatientsService(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<PatientResponse> ShowPatient(int patientId)
        {
            var patient = this.dataContext.Patients.FirstOrThrow((patient) => patient.Id == patientId, new NotFoundException());
            return new PatientResponse(patient);
        }

        public async Task<PaginatedResponse<PatientModel, List<PatientResponse>>> ShowPatients(int page)
        {
            var patients = this.dataContext.Patients;
            var patientList = PaginatedList<PatientModel>.Paginate(patients, page);

            // load all addresses
            // EF lazy loading of related fields is enabled - if not loaded here, will cause db connection err later
            foreach (var patient in patientList)
                _ = patient.Address;

            return new PaginatedResponse<PatientModel, List<PatientResponse>>(
                patientList,
                patients.Select(patient => new PatientResponse(patient)).ToList()
            );
        }

        public async Task<SuccessResponse> DeletePatient(int patientId)
        {
            var patient = this.dataContext.Patients.FirstOrThrow((patient) => patient.Id == patientId, new NotFoundException());
            var slots = this.dataContext.VaccinationSlots
                .Join(
                    this.dataContext.Vaccinations,
                    (VaccinationSlotModel slot) => slot.Id,
                    (VaccinationModel vaccination) => vaccination.VaccinationSlot.Id,
                    (VaccinationSlotModel slot, VaccinationModel vaccination) => new { slot = slot, vaccination = vaccination }).AsQueryable()
                .Where(o => o.vaccination.Patient.Id == patientId && o.slot.Date >= DateTime.Now && o.slot.Reserved == true)
                .Select(o => o.slot).ToList();

            this.dataContext.Remove(patient);
            this.dataContext.RemoveRange(slots);
            this.dataContext.SaveChanges();

            return new SuccessResponse();
        }
    }
}
