namespace backend.Dto.Responses
{
    public class AuthenticateResponse
    {
        public string Token { get; set; }

        public AuthenticateResponse(string token)
        {
            this.Token = token;
        }
    }
}
