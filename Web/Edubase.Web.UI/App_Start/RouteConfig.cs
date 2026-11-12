using Edubase.Web.UI.App_Start;
using Microsoft.AspNetCore.Routing;

namespace Edubase.Web.UI
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(IEndpointRouteBuilder endpoints)
        {
            CoreRouteConfig.MapRoutes(endpoints);
        }
    }
}
