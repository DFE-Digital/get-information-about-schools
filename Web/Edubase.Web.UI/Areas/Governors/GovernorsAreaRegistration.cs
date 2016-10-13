using System.Web.Mvc;

namespace Edubase.Web.UI.Areas.Governors
{
    public class GovernorsAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Governors";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Governors_default2",
                "Governors/{action}/{id}",
                new { controller = "Search", action = "Index", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Governors_default",
                "Governors/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );

            
        }
    }
}