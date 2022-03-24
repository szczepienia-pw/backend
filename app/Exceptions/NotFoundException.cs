using backend.Dto.Responses;

namespace backend.Exceptions
{
    public class NotFoundException : BasicException
    {
        public override ErrorResponse Render(HttpResponse response)
        {
            response.StatusCode = StatusCodes.Status404NotFound;
            return new ErrorResponse("Model not found");
        }
    }
}
