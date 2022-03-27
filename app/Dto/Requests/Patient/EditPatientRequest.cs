namespace backend.Dto.Requests.Patient
{
    public class EditPatientRequest
    {
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string? Pesel { get; set; }
		public string? Email { get; set; }
		public string? Password { get; set; }
		public EditPatientAddressRequest? Address { get; set; }
	}
}
