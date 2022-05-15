using System.Text.Json.Serialization;
using backend.Database;
using backend.Helpers;
using backend.Models.Accounts.AdditionalData;

namespace backend.Models.Accounts
{
    public class PatientModel : AccountModel, ISoftDelete
    {
        public string Pesel { get; set; }
        public string? VerificationToken { get; set; }
        public virtual AddressModel Address { get; set; }
        // seeder purposes
        [JsonIgnore]
        public int AddressId { get; set; }
        
        [JsonIgnore]
        public bool IsDeleted { get; set; }

        public override AccountTypeEnum GetEnum()
        {
            return AccountTypeEnum.Patient;
        }
    }
}
