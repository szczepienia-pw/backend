using backend.Models.Accounts;

namespace backend.Dto.Responses.Patient
{
    public class AuthenticateResponse : Responses.AuthenticateResponse
    {
        public PatientResponse Patient { get; set; }

        public AuthenticateResponse(string token, PatientModel patient) : base(token)
        {
            this.Patient = new PatientResponse(patient);
        }
    }
}
