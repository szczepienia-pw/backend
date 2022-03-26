using System;
using System.ComponentModel.DataAnnotations;

namespace backend.Dto.Requests.Patient
{
	public class PatientRegistrationRequest
	{
		[Required]
		public string FirstName { get; set; }
		[Required]
		public string LastName { get; set; }
		[Required]
		public string Pesel { get; set; }
		[Required]
		public string Email { get; set; }
		[Required]
		public string Password { get; set; }
		[Required]
		public PatientRegistrationAddressRequest Address { get; set; }
	}
}

