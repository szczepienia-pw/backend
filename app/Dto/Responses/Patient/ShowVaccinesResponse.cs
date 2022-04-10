using System;
using backend.Models.Vaccines;

namespace backend.Dto.Responses.Patient
{
	public class ShowVaccinesResponse
	{
		public string[] Vaccines;

		public ShowVaccinesResponse(List<VaccineModel> availableVaccines)
		{
			this.Vaccines = availableVaccines.Select(x => x.Name).ToArray();
		}
	}
}

