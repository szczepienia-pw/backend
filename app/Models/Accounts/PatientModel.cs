using backend.Helpers;
using backend.Models.Accounts.AdditionalData;

namespace backend.Models.Accounts
{
    public class PatientModel : AccountModel
    {
        public string Pesel { get; set; }
        public AddressModel Address { get; set; }
        public int AddressId { get; set; }

        public override AccountTypeEnum GetEnum()
        {
            return AccountTypeEnum.Patient;
        }
    }
}
