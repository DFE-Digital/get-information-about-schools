using System.Web.Http;

namespace Edubase.TexunaApi.Fake
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            config.Routes.MapHttpRoute(
                name: "CatchAll",
                routeTemplate: "{*uri}",
                defaults: new {controller = "Fake", uri = RouteParameter.Optional});
        }
    }
}
