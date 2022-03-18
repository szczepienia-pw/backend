using System.ComponentModel.DataAnnotations;

namespace backend.Dto.Requests.Doctor
{
    public class NewVaccinationSlotRequest
    {
        [Required]
        public string Date { get; set; }
    }
}
