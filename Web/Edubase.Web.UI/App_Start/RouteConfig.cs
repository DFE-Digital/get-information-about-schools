using System.Web.Mvc;
using System.Web.Routing;

namespace Edubase.Web.UI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();
            routes.MapRoute("prototype", "prototype/{viewName}", new { controller = "Prototype", action = "Index", area = "" });
        }
    }

}
