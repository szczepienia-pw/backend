namespace backend.Dto.Requests.Patient
{
    public class PatientAddressRequest
    {
		public virtual string? City { get; set; }
		public virtual string? ZipCode { get; set; }
		public virtual string? Street { get; set; }
		public virtual string? HouseNumber { get; set; }
		public virtual string? LocalNumber { get; set; }
	}
}
