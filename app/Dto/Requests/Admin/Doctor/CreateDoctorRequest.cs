using System;
using System.ComponentModel.DataAnnotations;

namespace backend.Dto.Requests.Admin
{
	public class CreateDoctorRequest
	{
		[Required]
		public string FirstName { get; set; }
		[Required]
		public string LastName { get; set; }
		[Required]
		public string Email { get; set; }
		[Required]
		public string Password { get; set; }
	}
}

