using System.Web.Mvc;
using System.Web.Routing;

namespace Edubase.Web.UI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "Prototype",
            //    url: "prototype/{viewName}",
            //    defaults: new { controller = "Prototype", action = "Index", viewName = "TestView" }
            //);
            
            //routes.MapRoute(
            //    name: "Main",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Search", action = "Index", id = UrlParameter.Optional },
            //    namespaces: new[] { "Edubase.Web.UI.Controllers" }
            //);

            routes.MapMvcAttributeRoutes();


        }
    }
}
