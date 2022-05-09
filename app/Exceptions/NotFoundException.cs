using System.Diagnostics.CodeAnalysis;
using backend.Dto.Responses;

namespace backend.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class NotFoundException : BasicException
    {
        public NotFoundException() : base("Model not found") { }

        public NotFoundException(string message) : base(message) { }

        public override ErrorResponse Render(HttpResponse response)
        {
            response.StatusCode = StatusCodes.Status404NotFound;
            return new ErrorResponse(this.Message);
        }
    }
}
