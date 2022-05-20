using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace backend.Dto.Requests
{
    [ExcludeFromCodeCoverage]
    public class ChangeSlotRequest
    {
        [Required]
        public int VaccinationSlotId { get; set; }
    }
}
