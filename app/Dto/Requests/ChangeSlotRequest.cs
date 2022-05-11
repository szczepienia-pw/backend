using System.ComponentModel.DataAnnotations;

namespace backend.Dto.Requests
{
    public class ChangeSlotRequest
    {
        [Required]
        public int VaccinationSlotId { get; set; }
    }
}
