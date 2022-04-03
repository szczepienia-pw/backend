using System;
using System.ComponentModel.DataAnnotations;

namespace backend.Dto.Requests.Patient
{
	public class PatientRegistrationRequest : PatientRequest
	{
		[Required]
		public override string? FirstName { get; set; }
		[Required]
		public override string? LastName { get; set; }
		[Required]
		public override string? Pesel { get; set; }
		[Required]
		public override string? Email { get; set; }
		[Required]
		public override string? Password { get; set; }
		[Required]
		public override PatientAddressRequest? Address { get; set; }
	}
}

