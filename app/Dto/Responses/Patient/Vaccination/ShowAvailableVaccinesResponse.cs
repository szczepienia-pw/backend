using backend.Dto.Responses.Common.Vaccination;
using backend.Models.Vaccines;

namespace backend.Dto.Responses.Patient.Vaccination
{
	public class ShowAvailableVaccinesResponse
	{
		public List<VaccineResponse> Vaccines { get; set; }

		public ShowAvailableVaccinesResponse(List<VaccineModel> vaccines)
		{
			this.Vaccines = vaccines.Select(vaccine => new VaccineResponse(vaccine)).ToList();
		}
	}
}

