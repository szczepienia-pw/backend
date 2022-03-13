using backend.Models.Accounts;

namespace backend.Dto.Responses.Admin
{
    public class AuthenticateResponse : Responses.AuthenticateResponse
    {
        public AdminModel Admin { get; set; }

        public AuthenticateResponse(string token, AdminModel admin) : base(token)
        {
            this.Admin = admin;
        }
    }
}
