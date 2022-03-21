using System.ComponentModel.DataAnnotations;

namespace backend.Dto.Requests.Doctor.VaccinationSlot
{
    public class FilterVaccinationSlotsRequest
    {
        [Required]
        public int Page { get; set; } = 1;
        [Range(0, 1)]
        public int? OnlyReserved { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
    }
}
