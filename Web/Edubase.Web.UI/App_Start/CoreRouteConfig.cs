using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Edubase.Web.UI.App_Start;

public static class CoreRouteConfig
{
    public static void MapRoutes(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapControllers();
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
    }
}
