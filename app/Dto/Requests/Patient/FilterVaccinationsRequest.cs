using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace backend.Dto.Requests.Patient
{
    [ExcludeFromCodeCoverage]
    public class FilterVaccinationsRequest : PaginationRequest
    {
        [Range(0, 1)]
        public int? OnlyReserved { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
    }
}
