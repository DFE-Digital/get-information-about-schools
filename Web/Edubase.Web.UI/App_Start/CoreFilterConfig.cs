using Edubase.Web.UI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.App_Start;

public static class CoreFilterConfig
{
    public static void RegisterGlobalFilters(MvcOptions options)
    {
        options.Filters.Add<CoreUnauthorizedAccessExceptionFilter>();
        options.Filters.Add<ApiExceptionFilter>();
    }
}
