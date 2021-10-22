using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Attributes
{
    public class EmailVerificationAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var isVerifiedEmail = bool.Parse(context.HttpContext.User.Claims.SingleOrDefault(claim => claim.Type == "email_verified").Value);

            if (!isVerifiedEmail)
            {
                context.Result = new UnauthorizedObjectResult($"Your email is not verified!");

                return;
            }

            await next();
        }
    }
}
