using backend.Dto.Responses.Common.Vaccination;
using backend.Models.Accounts;
using backend.Models.Visits;

namespace backend.Dto.Responses.Patient.Vaccination
{
	public class VaccinationResponse
	{
		public int Id { get; set; }
		public VaccineResponse Vaccine { get; set; }
        public AvailableSlotResponse VaccinationSlot { get; set; }
		public string Status { get; set; }
		public PatientModel Patient { get; set; }
		public DoctorModel Doctor { get; set; }

		public VaccinationResponse(VaccinationModel vaccination)
		{
			this.Id = vaccination.Id;
			this.Vaccine = new VaccineResponse(vaccination.Vaccine);
            this.VaccinationSlot = new AvailableSlotResponse(vaccination.VaccinationSlot);
			this.Status = vaccination.Status.ToString();
			this.Patient = vaccination.Patient;
            this.Doctor = vaccination.Doctor;
        }
	}
}

