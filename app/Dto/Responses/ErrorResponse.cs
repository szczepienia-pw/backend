namespace backend.Dto.Responses
{
    public class ErrorResponse
    {
        public bool Success { get; set; } = false;
        public string Msg { get; set; }

        public ErrorResponse(string? message = null)
        {
            this.Msg = message ?? "Undefined error";
        }
    }
}
