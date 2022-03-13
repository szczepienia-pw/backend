using backend.Models.Accounts;

namespace backend.Helpers
{
    public static class AccountTypeFactory
    {
        public static AccountTypeEnum CreateAccountTypeEnum(Type type) =>
            type.Name switch
            {
                nameof(PatientModel) => AccountTypeEnum.Patient,
                nameof(DoctorModel) => AccountTypeEnum.Doctor,
                nameof(AdminModel) => AccountTypeEnum.Admin,
                _ => throw new ArgumentException("Invalid account type!")
            };
    }
}
