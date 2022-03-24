using System.Text.Json.Serialization;
using backend.Database;
using backend.Helpers;

namespace backend.Models.Accounts
{
    public class DoctorModel : AccountModel, ISoftDelete
    {
        [JsonIgnore]
        public bool IsDeleted { get; set; }

        public override AccountTypeEnum GetEnum()
        {
            return AccountTypeEnum.Doctor;
        }
    }
}
