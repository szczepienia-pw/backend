using System.Diagnostics.CodeAnalysis;
using backend.Dto.Responses;

namespace backend.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class UnauthorizedException : BasicException
    {
        public UnauthorizedException(string message) : base(message) { }
        public UnauthorizedException() : base("Invalid credentials") { }
        
        public override ErrorResponse Render(HttpResponse response)
        {
            response.StatusCode = StatusCodes.Status401Unauthorized;
            return new ErrorResponse(this.Message);
        }
    }
}
