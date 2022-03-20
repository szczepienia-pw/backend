using backend.Dto.Responses;

namespace backend.Exceptions
{
    public class ValidationException : BasicException
    {
        public override ErrorResponse Render(HttpResponse response)
        {
            response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            return new ErrorResponse("Validation error");
        }
    }
}
