using System.Diagnostics.CodeAnalysis;
using backend.Dto.Responses;

namespace backend.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class ConflictException : BasicException
    {
        public ConflictException(string message) : base(message) { }

        public override ErrorResponse Render(HttpResponse response)
        {
            response.StatusCode = StatusCodes.Status409Conflict;
            return new ErrorResponse(this.Message);
        }
    }
}
