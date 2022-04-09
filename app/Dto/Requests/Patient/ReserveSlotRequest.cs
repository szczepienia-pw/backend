using System.ComponentModel.DataAnnotations;

namespace backend.Dto.Requests.Patient
{
	public class ReserveSlotRequest
	{
		[Required]
		public int VaccineId { get; set; }
	}
}

