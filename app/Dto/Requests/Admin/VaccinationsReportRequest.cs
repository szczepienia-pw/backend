using System.Diagnostics.CodeAnalysis;

namespace backend.Dto.Requests.Admin
{
    [ExcludeFromCodeCoverage]
    public class VaccinationsReportRequest
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
