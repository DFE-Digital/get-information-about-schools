using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Edubase.Web.UI.App_Start;

public sealed class CoreUnauthorizedAccessExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not UnauthorizedAccessException)
        {
            return;
        }

        context.Result = new RedirectResult("/Unauthorized");
        context.ExceptionHandled = true;
    }
}
