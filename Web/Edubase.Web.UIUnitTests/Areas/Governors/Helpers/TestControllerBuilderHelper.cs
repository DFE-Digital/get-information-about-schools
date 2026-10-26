using System.Web.Mvc;
using System.Web.Routing;

internal static class TestControllerBuilderHelper
{
    private static bool _routesRegistered = false;

    public static void RegisterRoutes()
    {
        if (_routesRegistered) return;

        // Remove routing collisions by using null route names
        RouteTable.Routes.Clear();

        RouteTable.Routes.MapRoute(
            name: null,
            url: "establishment/{id}",
            defaults: new { controller = "Establishment", action = "Details" }
        );

        RouteTable.Routes.MapRoute(
            name: null,
            url: "group/{id}",
            defaults: new { controller = "Group", action = "Details" }
        );

        _routesRegistered = true;
    }
}
