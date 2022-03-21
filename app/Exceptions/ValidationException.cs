using backend.Dto.Responses;

namespace backend.Exceptions
{
    public class ValidationException : BasicException
    {
        public readonly string errorMessage = "Validation error";

        public ValidationException() { }

        public ValidationException(string errorMessage)
        {
            this.errorMessage = errorMessage;
        }

        public override ErrorResponse Render(HttpResponse response)
        {
            response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            return new ErrorResponse(this.errorMessage);
        }
    }
}
