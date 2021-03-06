using backend.Models.Accounts;
using backend.Models.Vaccines;

namespace backend.Models.Visits
{
    public class VaccinationModel : BaseModel
    {
        public int Id { get; set; }
        public virtual VaccineModel Vaccine { get; set; }
        public virtual VaccinationSlotModel VaccinationSlot { get; set; }
        public StatusEnum Status { get; set; }
        public virtual PatientModel Patient { get; set; }
        public virtual DoctorModel Doctor { get; set; }

        // seeder purposes
        public int VaccineId { get; set; }
        public int VaccinationSlotId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
    }
}
