using System.ComponentModel.DataAnnotations;

namespace backend.Dto.Requests.Doctor.VaccinationSlot
{
    public class VaccinatePatientRequest
    {
        [Required]
        public string Status { get; set; }
    }
}
