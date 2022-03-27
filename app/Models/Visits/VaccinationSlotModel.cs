using backend.Models.Accounts;
using System.Text.Json.Serialization;

namespace backend.Models.Visits
{
    public class VaccinationSlotModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public bool Reserved { get; set; }
        public DoctorModel Doctor { get; set; }
        [JsonIgnore]
        public int DoctorId { get; set; }
    }
}
