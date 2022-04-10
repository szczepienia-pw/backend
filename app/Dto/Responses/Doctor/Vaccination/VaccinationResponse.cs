using backend.Dto.Responses.Common.Vaccination;
using backend.Models.Accounts;
using backend.Models.Visits;

namespace backend.Dto.Responses.Doctor.Vaccination
{
	public class VaccinationResponse
	{
		public int Id { get; set; }
		public VaccineResponse Vaccine { get; set; }
		public string Status { get; set; }
		public PatientModel Patient { get; set; }

		public VaccinationResponse(VaccinationModel vaccination)
		{
			this.Id = vaccination.Id;
			this.Vaccine = new VaccineResponse(vaccination.Vaccine);
			this.Status = vaccination.Status.ToString();
			this.Patient = vaccination.Patient;
		}
	}
}

