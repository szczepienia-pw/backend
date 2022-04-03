using System;
using System.ComponentModel.DataAnnotations;

namespace backend.Dto.Requests.Patient
{
	public class PatientRegistrationAddressRequest : PatientAddressRequest
	{
		[Required]
		public override string? City { get; set; }
		[Required]
		public override string? ZipCode { get; set; }
		[Required]
		public override string? Street { get; set; }
		[Required]
		public override string? HouseNumber { get; set; }
		public override string? LocalNumber { get; set; }
	}
}

