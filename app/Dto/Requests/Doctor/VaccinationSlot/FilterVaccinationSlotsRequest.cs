using System.ComponentModel.DataAnnotations;

namespace backend.Dto.Requests.Doctor.VaccinationSlot
{
    public class FilterVaccinationSlotsRequest : PaginationRequest
    {
        [Range(0, 1)]
        public int? OnlyReserved { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
    }
}
