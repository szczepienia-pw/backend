namespace backend.Dto.Requests.Patient
{
    public class PatientRequest
    {
		public virtual string? FirstName { get; set; }
		public virtual string? LastName { get; set; }
		public virtual string? Pesel { get; set; }
		public virtual string? Email { get; set; }
		public virtual string? Password { get; set; }
		public virtual PatientAddressRequest? Address { get; set; }
	}
}
