using System.ComponentModel.DataAnnotations;

namespace backend.Dto.Requests
{
    public class PaginationRequest
    {
        [Required]
        public int Page { get; set; } = 1;
    }
}
