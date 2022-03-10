using backend.Models.Accounts;
using backend.Models.Vaccines;

namespace backend.Models.Visits
{
    public class Vaccination
    {
        public int Id { get; set; }
        public Vaccine Vaccine { get; set; }
        public VaccinationSlot VaccinationSlot { get; set; }
        public Status Status { get; set; }
        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
    }
}
