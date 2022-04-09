using System;
using backend.Models.Vaccines;

namespace backend.Dto.Responses.Patient
{
	public class ShowVaccinesResponse
	{
		public List<VaccineModel> Vaccines;

		public ShowVaccinesResponse(List<VaccineModel> availableVaccines)
		{
			this.Vaccines = new List<VaccineModel>(availableVaccines);
		}
	}
}

