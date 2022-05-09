using backend.Models.Accounts;
using System.Text.Json.Serialization;

namespace backend.Models.Visits
{
    public class VaccinationSlotModel : BaseModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public bool Reserved { get; set; }
        public virtual DoctorModel Doctor { get; set; }
        public virtual VaccinationModel Vaccination { get; set; }
        
        // seeder purposes
        [JsonIgnore]
        public int DoctorId { get; set; }
    }
}
