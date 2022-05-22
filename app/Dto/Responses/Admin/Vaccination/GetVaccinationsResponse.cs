using backend.Dto.Responses.Common.Vaccination;
using backend.Dto.Responses.Doctor;
using backend.Dto.Responses.Patient;
using backend.Models.Accounts;
using backend.Models.Visits;

namespace backend.Dto.Responses.Admin.Vaccination
{
	public class GetVaccinationsResponse
	{
		public int Id { get; set; }
		public VaccineResponse Vaccine { get; set; }
		public VaccinationSlotResponse VaccinationSlot { get; set; }
		public string Status { get; set; }
		public PatientResponse Patient { get; set; }
		public DoctorResponse Doctor { get; set; }

		public GetVaccinationsResponse(VaccinationModel vaccination)
		{
			this.Id = vaccination.Id;
			this.Vaccine = new VaccineResponse(vaccination.Vaccine);
			this.VaccinationSlot = new VaccinationSlotResponse(vaccination.VaccinationSlot);
			this.Status = vaccination.Status.ToString();
			this.Patient = new PatientResponse(vaccination.Patient);
			this.Doctor = new DoctorResponse(vaccination.Doctor);
		}
	}
}
