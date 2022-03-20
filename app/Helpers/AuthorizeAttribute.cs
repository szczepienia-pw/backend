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
            if (
                !context.HttpContext.Items.ContainsKey("User")
                || context.HttpContext.Items["User"] == null
                || !context.HttpContext.Items.ContainsKey("AccountType")
                || context.HttpContext.Items["AccountType"] == null
                || (AccountTypeEnum) context.HttpContext.Items["AccountType"] != this.accountType
            )
            {
                context.Result = new JsonResult(new ErrorResponse("Unauthorized")) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}
