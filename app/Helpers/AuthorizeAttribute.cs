using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using backend.Models.Accounts;
using backend.Dto.Responses;

namespace backend.Helpers
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private AccountTypeEnum accountType;

        public AuthorizeAttribute(AccountTypeEnum accountType)
        {
            this.accountType = accountType;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = (DoctorModel)context.HttpContext.Items["User"];
            var accountType = (AccountTypeEnum)context.HttpContext.Items["AccountType"];
            if (user == null || accountType != this.accountType)
            {
                // Not logged in
                context.Result = new JsonResult(new ErrorResponse("Unauthorized")) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}
