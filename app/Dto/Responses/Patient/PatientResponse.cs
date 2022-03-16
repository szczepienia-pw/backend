using backend.Models.Accounts;
using backend.Models.Accounts.AdditionalData;

namespace backend.Dto.Responses.Patient
{
    public class PatientResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Pesel { get; set; }
        public string Email { get; set; }
        public AddressModel Address { get; set; }

        public PatientResponse(PatientModel model)
        {
            this.Id = model.Id;
            this.FirstName = model.FirstName;
            this.LastName = model.LastName; 
            this.Pesel = model.Pesel;
            this.Email = model.Email;
            this.Address = model.Address;
        }
    }
}
