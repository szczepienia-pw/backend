using backend.Models.Accounts;

namespace backend.Dto.Responses.Doctor
{
    public class AuthenticateResponse : Responses.AuthenticateResponse
    {
        public DoctorModel Doctor { get; set; }

        public AuthenticateResponse(string token, DoctorModel doctor) : base(token)
        {
            this.Doctor = doctor;
        }
    }
}
