using backend.Dto.Responses;

namespace backend.Exceptions
{
    public class UnauthorizedException : BasicException
    {
        public override ErrorResponse Render(HttpResponse response)
        {
            response.StatusCode = StatusCodes.Status401Unauthorized;
            return new ErrorResponse("Invalid credentials");
        }
    }
}
