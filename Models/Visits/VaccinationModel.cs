using backend.Models.Accounts;
using backend.Models.Vaccines;

namespace backend.Models.Visits
{
    public class VaccinationModel
    {
        public int Id { get; set; }
        public VaccineModel Vaccine { get; set; }
        public VaccinationSlotModel VaccinationSlot { get; set; }
        public StatusEnum Status { get; set; }
        public PatientModel Patient { get; set; }
        public DoctorModel Doctor { get; set; }
    }
}
