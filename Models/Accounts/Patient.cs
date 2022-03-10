using backend.Models.Accounts.AdditionalData;

namespace backend.Models.Accounts
{
    public class Patient : Account
    {
        public string Pesel { get; set; }
        public Address Address { get; set; }
    }
}
