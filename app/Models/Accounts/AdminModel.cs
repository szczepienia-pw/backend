using backend.Helpers;

namespace backend.Models.Accounts
{
    public class AdminModel : AccountModel
    {
        public override AccountTypeEnum GetEnum()
        {
            return AccountTypeEnum.Admin;
        }
    }
}
