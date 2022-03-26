using System.ComponentModel.DataAnnotations;

namespace backend.Dto.Requests
{
    public class SendBugRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
