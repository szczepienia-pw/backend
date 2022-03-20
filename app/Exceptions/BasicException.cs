using backend.Dto.Responses;

namespace backend.Exceptions
{
    public class BasicException : Exception
    {
        public virtual ErrorResponse Render(HttpResponse response)
        {
            response.StatusCode = StatusCodes.Status500InternalServerError;
            return new ErrorResponse("General error");
        }
    }
}
