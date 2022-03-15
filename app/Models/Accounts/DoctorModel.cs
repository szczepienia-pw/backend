using backend.Helpers;

namespace backend.Models.Accounts
{
    public class DoctorModel : AccountModel
    {
        public override AccountTypeEnum GetEnum()
        {
            return AccountTypeEnum.Doctor;
        }
    }
}
