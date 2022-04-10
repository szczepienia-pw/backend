using System;
using backend.Models.Accounts;
using backend.Models.Vaccines;
using backend.Models.Visits;

namespace backend.Dto.Responses.Doctor.Vaccination
{
	public class VaccinationResponse
	{
		public int Id { get; set; }
		public VaccineModel Vaccine { get; set; }
		public string Status { get; set; }
		public PatientModel Patient { get; set; }

		public VaccinationResponse(VaccinationModel vaccination)
		{
			this.Id = vaccination.Id;
			this.Vaccine = vaccination.Vaccine;
			this.Status = vaccination.Status.ToString();
			this.Patient = vaccination.Patient;
		}
	}
}

