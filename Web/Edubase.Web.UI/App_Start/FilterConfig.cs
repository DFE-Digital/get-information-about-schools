using Edubase.Web.UI.App_Start;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI
{
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(MvcOptions options)
        {
            CoreFilterConfig.RegisterGlobalFilters(options);
        }
    }
}
