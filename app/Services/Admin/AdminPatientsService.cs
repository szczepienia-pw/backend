using backend.Database;
using backend.Dto.Responses;
using backend.Dto.Responses.Patient;
using backend.Exceptions;
using backend.Helpers;
using backend.Models.Accounts;
using backend.Models.Visits;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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
            var patient = this.dataContext.Patients.Include(p => p.Address).FirstOrThrow((patient) => patient.Id == patientId, new NotFoundException());
            var address = patient.Address;
            var slots = this.dataContext.VaccinationSlots
                .Include(slot => slot.Doctor)
                .Join(
                    this.dataContext.Vaccinations,
                    (VaccinationSlotModel slot) => slot.Id,
                    (VaccinationModel vaccination) => vaccination.VaccinationSlot.Id,
                    (VaccinationSlotModel slot, VaccinationModel vaccination) =>
                        new { vaccinationSlot = slot, vaccination = vaccination }).AsQueryable()
                .Where(o => o.vaccination.Patient.Id == patientId && o.vaccinationSlot.Date >= DateTime.Now &&
                            o.vaccinationSlot.Reserved == true);
            
            // free all reserved slots
            foreach (var slot in slots)
            {
                slot.vaccination.Status = StatusEnum.Canceled;

                // Duplicate vaccination slot
                VaccinationSlotModel newSlot = new VaccinationSlotModel { Date = slot.vaccinationSlot.Date, Doctor = slot.vaccinationSlot.Doctor, Reserved = false };
                this.dataContext.Add(newSlot);
            }
            
            ((ISoftDelete)patient).SoftDelete();

            if ((this.dataContext.Patients.Where(p => p.AddressId == address.Id && p.Id != patient.Id)).Count() == 0)
                this.dataContext.Remove(address);                
            
            this.dataContext.SaveChanges();

            return new SuccessResponse();
        }
    }
}
