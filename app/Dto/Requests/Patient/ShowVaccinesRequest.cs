using System.ComponentModel.DataAnnotations;
using backend.Models.Vaccines;

namespace backend.Dto.Requests.Patient
{
	public class ShowVaccinesRequest
	{
		[Required]
		public string Disease { get; set; }
	}
}

