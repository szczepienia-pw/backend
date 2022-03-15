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
            bool authorized = true;

            // Check if user exists in database
            if (context.HttpContext.Items.ContainsKey("User"))
            {
                AccountModel? user = (AccountModel)context.HttpContext.Items["User"];
                if (user == null) authorized = false;
            }
            else authorized = false;

            // Check if requested access priviliges match account type
            if (context.HttpContext.Items.ContainsKey("AccountType"))
            {
                AccountTypeEnum? accountType = (AccountTypeEnum)context.HttpContext.Items["AccountType"];
                if (accountType == null || accountType != this.accountType) authorized = false;
            }
            else authorized = false;

            // Authorization failed
            if(!authorized)
            {
                context.Result = new JsonResult(new ErrorResponse("Unauthorized")) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}
