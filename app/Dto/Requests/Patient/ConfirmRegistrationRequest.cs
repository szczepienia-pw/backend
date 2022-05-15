using System.Diagnostics.CodeAnalysis;

namespace backend.Dto.Requests.Patient
{
    [ExcludeFromCodeCoverage]
    public class ConfirmRegistrationRequest
    {
        public string Token { get; set; }
    }
}

