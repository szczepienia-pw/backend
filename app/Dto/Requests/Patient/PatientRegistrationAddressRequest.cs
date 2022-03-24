using System;
using System.ComponentModel.DataAnnotations;

namespace backend.Dto.Requests.Patient
{
	public class PatientRegistrationAddressRequest
	{
		[Required]
		public string City { get; set; }
		[Required]
		public string ZipCode { get; set; }
		[Required]
		public string Street { get; set; }
		[Required]
		public string HouseNumber { get; set; }
		public string LocalNumber { get; set; }
	}
}

