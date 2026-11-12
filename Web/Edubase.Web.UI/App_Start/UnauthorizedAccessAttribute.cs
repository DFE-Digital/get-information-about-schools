using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Edubase.Web.UI
{
    public class UnauthorizedAccessAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is UnauthorizedAccessException)
            {
                context.Result = new RedirectResult("/Unauthorized");
                context.ExceptionHandled = true;
            }
        }
    }
}
