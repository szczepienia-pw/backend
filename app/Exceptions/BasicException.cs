using backend.Dto.Responses;

namespace backend.Exceptions
{
    public class BasicException : Exception
    {
        public BasicException(string message) : base(message) { }

        public BasicException() { }

        public virtual ErrorResponse Render(HttpResponse response)
        {
            response.StatusCode = StatusCodes.Status500InternalServerError;
            return new ErrorResponse("General error");
        }
    }
}
