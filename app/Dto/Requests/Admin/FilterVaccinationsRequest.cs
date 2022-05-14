using System.Diagnostics.CodeAnalysis;

namespace backend.Dto.Requests.Admin
{
    [ExcludeFromCodeCoverage]
    public class FilterVaccinationsRequest : PaginationRequest
    {
        public string? Disease { get; set; }
        public int? DoctorId { get; set; }
        public int? PatientId { get; set; }
    }
}
