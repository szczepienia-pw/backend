namespace backend.Dto.Requests.Patient
{
    public class EditPatientAddressRequest
    {
		public string? City { get; set; }
		public string? ZipCode { get; set; }
		public string? Street { get; set; }
		public string? HouseNumber { get; set; }
		public string? LocalNumber { get; set; }
	}
}
