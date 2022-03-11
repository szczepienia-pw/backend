using backend.Models.Accounts.AdditionalData;

namespace backend.Models.Accounts
{
    public class PatientModel : AccountModel
    {
        public string Pesel { get; set; }
        public AddressModel Address { get; set; }
    }
}
