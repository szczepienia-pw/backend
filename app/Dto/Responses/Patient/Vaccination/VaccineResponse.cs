using backend.Models.Vaccines;

namespace backend.Dto.Responses.Patient.Vaccination
{
	public class VaccineResponse
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Disease { get; set; }
		public int RequiredDoses { get; set; }

		public VaccineResponse(VaccineModel vaccine)
		{
			this.Id = vaccine.Id;
			this.Name = vaccine.Name;
			this.Disease = vaccine.Disease.GetDescription();
			this.RequiredDoses = vaccine.RequiredDoses;
		}
	}
}

