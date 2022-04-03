using backend.Dto.Responses;

namespace backend.Exceptions
{
    public class ValidationException : BasicException
    {
        public ValidationException() : base("Validation error") { }

        public ValidationException(string message) : base(message) { }

        public override ErrorResponse Render(HttpResponse response)
        {
            response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            return new ErrorResponse(this.Message);
        }
    }
}
